# Yalniz dogrulama yardimcilari. ASCII: Windows PowerShell 5.1 uyumlulugu.
Set-StrictMode -Version Latest

function ConvertTo-NativeArgument([string]$Value) {
  if ($Value.Contains([char]0) -or $Value.Contains("`n") -or $Value.Contains("`r")) { throw "Invalid native argument." }
  $escaped = [regex]::Replace($Value, '(\\*)"', '$1$1\"')
  $escaped = [regex]::Replace($escaped, '(\\+)$', '$1$1')
  return '"' + $escaped + '"'
}

function Invoke-OwnedProcess([string]$FilePath, [string[]]$Arguments, [int]$TimeoutSeconds, [string]$WorkingDirectory, [switch]$Visible, [string]$StdoutPath = '', [string]$StderrPath = '') {
  $nativeArguments = ($Arguments | ForEach-Object { ConvertTo-NativeArgument $_ }) -join ' '
  $windowStyle = 'Hidden'
  if ($Visible) { $windowStyle = 'Normal' }
  $startArguments = @{ FilePath = $FilePath; ArgumentList = $nativeArguments; WorkingDirectory = $WorkingDirectory; WindowStyle = $windowStyle; PassThru = $true }
  foreach ($logPath in @($StdoutPath, $StderrPath)) {
    if ($logPath -and (Test-Path -LiteralPath $logPath)) { throw "Process log already exists; preserving evidence: $logPath" }
  }
  if ($StdoutPath) { $startArguments.RedirectStandardOutput = $StdoutPath }
  if ($StderrPath) { $startArguments.RedirectStandardError = $StderrPath }
  $ownedProcess = Start-Process @startArguments
  try {
    # Windows PowerShell'in yonlendirilmis surecinde ExitCode handle korunmazsa null olabilir.
    $ownedHandle = $ownedProcess.Handle
    if (-not $ownedProcess.WaitForExit($TimeoutSeconds * 1000)) {
      # Yalniz bu cagrida olusturulan surecin handle'i sonlandirilir.
      $ownedProcess.Kill()
      $ownedProcess.WaitForExit()
      throw "Owned process timed out after ${TimeoutSeconds}s: $FilePath"
    }
    $ownedProcess.WaitForExit()
    $ownedProcess.Refresh()
    if ($null -eq $ownedProcess.ExitCode) { throw "Owned process exit code is unavailable: $FilePath" }
    return [int]$ownedProcess.ExitCode
  } finally { $ownedProcess.Dispose() }
}

function Invoke-FrameReview([string]$CheckerPath, [string]$Folder, [string]$OutputDirectory) {
  if (-not (Test-Path -LiteralPath $CheckerPath -PathType Leaf)) { throw "Frame checker missing: $CheckerPath" }
  $pythonPath = (Get-Command python.exe -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
  $stdoutPath = Join-Path $OutputDirectory 'frames.log'
  $stderrPath = Join-Path $OutputDirectory 'frames.stderr.log'
  # Yerel pipeline yerine sahip olunan gizli surec: cikis kodu ve iki akis ayri kanit olur.
  $frameExit = Invoke-OwnedProcess $pythonPath @('-X', 'utf8', $CheckerPath, $Folder) 300 (Split-Path -Parent $CheckerPath) -StdoutPath $stdoutPath -StderrPath $stderrPath
  $receiptPath = Join-Path $OutputDirectory 'frames-process.json'
  [IO.File]::WriteAllText($receiptPath, ([ordered]@{ executable = $pythonPath; exitCode = $frameExit; completedUtc = [DateTime]::UtcNow.ToString('O'); stdout = $stdoutPath; stderr = $stderrPath } | ConvertTo-Json), [Text.Encoding]::UTF8)
  if ($frameExit -ne 0) { throw "Frame checker exited $frameExit; see frames.log and frames.stderr.log." }
  $null = Assert-CleanLog $stdoutPath
  if ((Get-Item -LiteralPath $stderrPath).Length -gt 0) { $null = Assert-CleanLog $stderrPath }
  $frameReport = Join-Path $Folder 'frames.json'
  if (-not (Test-Path -LiteralPath $frameReport -PathType Leaf)) { throw 'Frame checker result missing despite native exit 0.' }
  $parsedFrames = [IO.File]::ReadAllText($frameReport) | ConvertFrom-Json
  $frames = @($parsedFrames)
  if ($frames.Count -eq 0) { throw 'Frame checker reported zero frames.' }
  foreach ($frame in $frames) {
    if ($frame.width -ne 1440 -or $frame.height -ne 900 -or @($frame.problems).Count -ne 0) { throw "Frame checker reported a broken image: $($frame.name)" }
  }
  $sheet = Join-Path $Folder 'contact-sheet.jpg'
  if (-not (Test-Path -LiteralPath $sheet -PathType Leaf) -or (Get-Item -LiteralPath $sheet).Length -eq 0) { throw 'Frame contact sheet is missing or empty.' }
  return "$($frames.Count) frames; automated image checks; visual review remains separate"
}

function Assert-CleanLog([string]$Path) {
  if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "Required log missing: $Path" }
  $logText = [IO.File]::ReadAllText($Path)
  if ([string]::IsNullOrWhiteSpace($logText)) { throw "Required log is empty: $Path" }
  $pattern = '(?im)(?:\berror CS\d+:|\b(?:[A-Za-z_][\w.]*Exception):|Unhandled exception|Shader error|Crash!!!|Assertion failed|^\s*(?:Error|Exception|Assert):|Auto shots failed:|UNKNOWN COMMAND|^FAILED )'
  $errors = [regex]::Matches($logText, $pattern)
  if ($errors.Count -gt 0) { throw "Error marker '$($errors[0].Value)' in $Path" }
  return $logText
}

function Get-ReviewGraphicsArguments([ValidateSet('Default', 'Direct3D11', 'Direct3D12')][string]$GraphicsApi = 'Default') {
  if ($GraphicsApi -eq 'Direct3D11') { return '-force-d3d11' }
  if ($GraphicsApi -eq 'Direct3D12') { return '-force-d3d12' }
}

function Assert-ReviewGraphics([string]$LogText, [ValidateSet('Default', 'Direct3D11', 'Direct3D12')][string]$GraphicsApi = 'Default') {
  # Video cozumleme icin kurulan ikinci D3D11 aygiti ana renderer kaniti degildir.
  if ($GraphicsApi -eq 'Default') { return }
  $version = '11'
  if ($GraphicsApi -eq 'Direct3D12') { $version = '12' }
  if ($LogText -notmatch ('(?m)^\s*Version:\s+Direct3D\s+' + $version + '(?:\.|\s)')) { throw "Requested $GraphicsApi renderer was not confirmed in player log." }
}

function Get-EditTestSummary([string]$Path, [int]$ExitCode) {
  if ($ExitCode -ne 0) { throw "EditMode process exited $ExitCode." }
  if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "EditMode result XML missing: $Path" }
  [xml]$testDocument = [IO.File]::ReadAllText($Path)
  $run = $testDocument.SelectSingleNode('/test-run')
  if ($null -eq $run) { throw "EditMode result has no test-run root." }
  foreach ($attribute in @('total', 'passed', 'failed', 'result')) {
    if (-not $run.HasAttribute($attribute)) { throw "EditMode result is missing $attribute." }
  }
  $total = [int]$run.GetAttribute('total')
  $passed = [int]$run.GetAttribute('passed')
  $failed = [int]$run.GetAttribute('failed')
  $cases = @($testDocument.SelectNodes('//test-case'))
  $passingCases = @($testDocument.SelectNodes('//test-case[@result="Passed"]'))
  if ($total -le 0 -or $passed -ne $total -or $failed -ne 0 -or $run.GetAttribute('result') -ne 'Passed' -or $cases.Count -ne $total -or $passingCases.Count -ne $total) {
    throw "EditMode tests incomplete or failed: $passed/$total passed, $failed failed, $($cases.Count) test cases."
  }
  return "$passed/$total passed"
}

function Get-ReviewPlan([string]$Path) {
  if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "Review script missing: $Path" }
  $lines = @([IO.File]::ReadAllLines($Path) | ForEach-Object { $_.Trim() } | Where-Object { $_ -and -not $_.StartsWith('#') })
  if ($lines.Count -lt 2 -or $lines[0] -ne 'new' -or $lines[-1] -ne 'quit' -or @($lines | Where-Object { $_ -eq 'quit' }).Count -ne 1) { throw "Review must start with new and have one final quit." }
  $captures = @(); $states = @(); $assertions = 0
  foreach ($line in $lines) {
    if ($line -match '^(shot|state)\s+(.+)$') {
      $kind = $Matches[1]; $name = $Matches[2]
      if ($name -notmatch '\A[a-zA-Z0-9][a-zA-Z0-9_-]{0,79}\z') { throw "Unsafe artifact name: $name" }
      if ($kind -eq 'shot') { $captures += "$name.png" } else { $states += "$name.json" }
    }
    if ($line -match '^(expect|same)\s+') { $assertions++ }
  }
  if ($captures.Count -eq 0 -or $assertions -eq 0) { throw "Review needs frames and assertions." }
  if (@($captures | Select-Object -Unique).Count -ne $captures.Count -or @($states | Select-Object -Unique).Count -ne $states.Count) { throw "Duplicate artifact names." }
  return [pscustomobject]@{ Commands = $lines.Count; Assertions = $assertions; Captures = $captures; States = $states }
}

function Assert-ReviewProtocol([string]$PlayerPath) {
  if (-not (Test-Path -LiteralPath $PlayerPath -PathType Leaf)) { throw "Player executable missing: $PlayerPath" }
  $dataDirectory = Join-Path (Split-Path -Parent $PlayerPath) (([IO.Path]::GetFileNameWithoutExtension($PlayerPath)) + '_Data')
  $assemblyPath = Join-Path $dataDirectory 'Managed\PowerAboveAll.Runtime.dll'
  if (-not (Test-Path -LiteralPath $assemblyPath -PathType Leaf)) { throw "Player assembly missing: $assemblyPath" }
  $bytes = [IO.File]::ReadAllBytes($assemblyPath)
  $even = [Text.Encoding]::Unicode.GetString($bytes)
  $odd = [Text.Encoding]::Unicode.GetString($bytes, 1, $bytes.Length - 1)
  if (-not $even.Contains('AUTO_SHOTS_PROTOCOL 2') -and -not $odd.Contains('AUTO_SHOTS_PROTOCOL 2')) { throw "Legacy player review harness rejected; rebuild with isolated-save protocol 2." }
  return $assemblyPath
}

function Get-BuildFileManifest([string]$Directory) {
  $buildRoot = [IO.Path]::GetFullPath($Directory).TrimEnd('\', '/')
  $rootInfo = Get-Item -LiteralPath $buildRoot
  if (-not $rootInfo.PSIsContainer -or ($rootInfo.Attributes -band [IO.FileAttributes]::ReparsePoint)) { throw 'Build manifest requires an ordinary local directory.' }
  $pending = New-Object 'Collections.Generic.Stack[string]'
  $files = New-Object 'Collections.Generic.List[object]'
  $pending.Push($buildRoot)
  while ($pending.Count -gt 0) {
    foreach ($entry in Get-ChildItem -LiteralPath $pending.Pop() -Force) {
      if ($entry.Attributes -band [IO.FileAttributes]::ReparsePoint) { throw "Build contains a link/reparse point: $($entry.FullName)" }
      if ($entry.PSIsContainer) { $pending.Push($entry.FullName); continue }
      $relative = $entry.FullName.Substring($buildRoot.Length + 1).Replace('\', '/')
      # Get-FileHash dosyayi akisla okur; buyuk kaynak paketleri RAM'e yuklenmez.
      $files.Add([pscustomobject]@{ path = $relative; size = [long]$entry.Length; sha256 = (Get-FileHash -LiteralPath $entry.FullName -Algorithm SHA256).Hash.ToLowerInvariant() })
    }
  }
  if ($files.Count -eq 0) { throw 'Build manifest cannot be empty.' }
  return @($files | Sort-Object path)
}

function Assert-ReviewResult([string]$Folder, $Plan, [int]$ExitCode) {
  if ($ExitCode -ne 0) { throw "Player review exited $ExitCode." }
  $receiptPath = Join-Path $Folder 'shots-result.json'
  if (-not (Test-Path -LiteralPath $receiptPath -PathType Leaf)) { throw "Player completion receipt missing." }
  $receipt = [IO.File]::ReadAllText($receiptPath) | ConvertFrom-Json
  if ($receipt.protocolVersion -ne 2 -or $receipt.success -ne $true -or @($receipt.failures).Count -ne 0) { throw "Player reported a failed or unsupported review." }
  if ($receipt.commands -ne $Plan.Commands -or $receipt.assertions -ne $Plan.Assertions) { throw "Player did not execute every command/assertion." }
  $expectedCampaign = [IO.Path]::GetFullPath((Join-Path $Folder '.campaign\campaign-v1.json'))
  if ([IO.Path]::GetFullPath($receipt.campaignPath) -ne $expectedCampaign -or -not (Test-Path -LiteralPath $expectedCampaign -PathType Leaf)) { throw "Isolated campaign evidence missing or incorrect." }
  if (@(Compare-Object -ReferenceObject @($Plan.Captures) -DifferenceObject @($receipt.captures)).Count -ne 0) { throw "Capture manifest differs from review plan." }
  $observedStates = @($receipt.states)
  if ($observedStates.Count -ne $Plan.States.Count -or ($observedStates.Count -gt 0 -and @(Compare-Object -ReferenceObject @($Plan.States) -DifferenceObject $observedStates).Count -ne 0)) { throw "State manifest differs from review plan." }
  foreach ($name in @($Plan.Captures) + @($Plan.States)) {
    $artifact = Join-Path $Folder $name
    if (-not (Test-Path -LiteralPath $artifact -PathType Leaf) -or (Get-Item -LiteralPath $artifact).Length -eq 0) { throw "Required artifact missing or empty: $name" }
  }
  $reviewLog = Assert-CleanLog (Join-Path $Folder 'shots.log')
  if ($reviewLog -notmatch '(?m)^PASS completed ') { throw "Player did not finish the script successfully." }
  return "$($Plan.Captures.Count) frames; $($Plan.Assertions) assertions; $($Plan.States.Count) states"
}
