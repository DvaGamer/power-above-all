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

function Invoke-FrameReview([string]$CheckerPath, [string]$Folder, [string]$OutputDirectory, [ValidateRange(1, 300)][int]$TimeoutSeconds = 300, [int]$Width=1440, [int]$Height=900) {
  if (-not (Test-Path -LiteralPath $CheckerPath -PathType Leaf)) { throw "Frame checker missing: $CheckerPath" }
  $pythonPath = (Get-Command python.exe -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
  $stdoutPath = Join-Path $OutputDirectory 'frames.log'
  $stderrPath = Join-Path $OutputDirectory 'frames.stderr.log'
  # Yerel pipeline yerine sahip olunan gizli surec: cikis kodu ve iki akis ayri kanit olur.
  $frameExit = Invoke-OwnedProcess $pythonPath @('-X', 'utf8', $CheckerPath, $Folder, '--width', [string]$Width, '--height', [string]$Height) $TimeoutSeconds (Split-Path -Parent $CheckerPath) -StdoutPath $stdoutPath -StderrPath $stderrPath
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
    if ($frame.width -ne $Width -or $frame.height -ne $Height -or @($frame.problems).Count -ne 0) { throw "Frame checker reported a broken image: $($frame.name)" }
  }
  $sheet = Join-Path $Folder 'contact-sheet.jpg'
  if (-not (Test-Path -LiteralPath $sheet -PathType Leaf) -or (Get-Item -LiteralPath $sheet).Length -eq 0) { throw 'Frame contact sheet is missing or empty.' }
  return "$($frames.Count) frames; automated image checks; visual review remains separate"
}

function Assert-CleanLog([string]$Path) {
  if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "Required log missing: $Path" }
  $logText = [IO.File]::ReadAllText($Path)
  if ([string]::IsNullOrWhiteSpace($logText)) { throw "Required log is empty: $Path" }
  $pattern = '(?im)(?:\berror CS\d+:|\b(?:[A-Za-z_][\w.]*Exception):|Unhandled exception|Shader error|Crash!!!|Assertion failed|^\s*(?:Error|Exception|Assert):|Auto shots failed:|UNKNOWN COMMAND|^FAILED |does not have a valid GUID[^\r\n]*\bAsset file will be ignored\b)'
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

function Assert-BattleReviewCommand([string]$Line) {
  $parts = @($Line -split '\s+')
  if ($parts.Count -lt 2) { throw 'Battle subcommand missing.' }
  $valid = $false
  switch ($parts[1]) {
    'select' { $valid = $parts.Count -eq 4 -and $parts[2] -match '^[1-4]$' -and $parts[3] -in @('replace', 'add', 'toggle') }
    'formation' { $valid = $parts.Count -eq 3 -and $parts[2] -in @('line', 'column', 'square') }
    'intent' { $valid = $parts.Count -eq 3 -and $parts[2] -in @('hold', 'reserve', 'flank') }
    'hq' { $valid = $parts.Count -eq 4 -and $parts[2] -match '^-?[0-9]+(?:\.[0-9]+)?$' -and $parts[3] -match '^-?[0-9]+(?:\.[0-9]+)?$' }
    'fire' { $valid = $parts.Count -eq 3 -and $parts[2] -in @('hold', 'free') }
    'pause' { $valid = $parts.Count -eq 3 -and $parts[2] -in @('on', 'off') }
    'volley' { $valid = $parts.Count -eq 2 }
    'verify-return' { $valid = $parts.Count -eq 2 }
    'state' { $valid = $parts.Count -eq 3 -and $parts[2] -match '\A[a-zA-Z0-9][a-zA-Z0-9_-]{0,79}\z' }
    'move' {
      $valid = $parts.Count -eq 4
      if ($valid) {
        foreach ($token in $parts[2..3]) {
          [float]$coordinate = 0
          if (-not [float]::TryParse($token, [Globalization.NumberStyles]::Float, [Globalization.CultureInfo]::InvariantCulture, [ref]$coordinate) -or [float]::IsNaN($coordinate) -or [float]::IsInfinity($coordinate)) { $valid = $false }
        }
      }
    }
    'wait' {
      [float]$duration = 0
      $valid = $parts.Count -eq 4 -and $parts[2] -in @('active', 'arrived', 'volley-ready', 'ended')
      if ($valid) { $valid = [float]::TryParse($parts[3], [Globalization.NumberStyles]::Float, [Globalization.CultureInfo]::InvariantCulture, [ref]$duration) -and -not [float]::IsNaN($duration) -and -not [float]::IsInfinity($duration) -and $duration -gt 0 -and $duration -le 120 }
    }
  }
  if (-not $valid) { throw "Unsupported or invalid battle command: $Line" }
}

function Get-ReviewPlan([string]$Path) {
  if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "Review script missing: $Path" }
  $lines = @([IO.File]::ReadAllLines($Path) | ForEach-Object { $_.Trim() } | Where-Object { $_ -and -not $_.StartsWith('#') })
  if ($lines.Count -lt 2 -or $lines[0] -ne 'new' -or $lines[-1] -ne 'quit' -or @($lines | Where-Object { $_ -eq 'quit' }).Count -ne 1) { throw "Review must start with new and have one final quit." }
  $captures = @(); $states = @(); $assertions = 0
  foreach ($line in $lines) {
    if ($line -match '^time(?:\s|$)' -and $line -cnotmatch '^time (pause|1|2|3)$') { throw "Unsupported world speed: $line" }
    if ($line -match '^world(?:\s|$)' -and $line -cnotmatch '^world (focus|close|retreat|supply|stock|supplypanel|convoy|unit [a-zA-Z0-9_-]+|wait (contact|ended|arrived|delivered) [0-9]+(?:\.[0-9]+)?)$') { throw "Unsupported world review command: $line" }
    if ($line -match '^world wait \w+ ([0-9]+(?:\.[0-9]+)?)$') {
      [double]$worldWait=[double]::Parse($Matches[1],[Globalization.CultureInfo]::InvariantCulture)
      if($worldWait -le 0 -or $worldWait -gt 1200){throw "World wait must be between 0 and 1200 seconds: $line"}
    }
    if ($line -match '^desk(?:\s|$)' -and $line -cnotmatch '^desk (open|view (report|outbox|draft)|(bread|tax|order|report) (strict|mission) (normal|express))$') { throw "Unsupported cabinet correspondence command: $line" }
    if ($line -match '^atlas(?:\s|$)' -and $line -cnotmatch '^atlas (world|europe|france|region|oblique|clean|panels)$') { throw "Unsupported atlas review view: $line" }
    if ($line -match '^accord(?:\s|$)' -and $line -cne 'accord grant') { throw "Unsupported regional accord order: $line" }
    if ($line -match '^scroll(?:\s|$)') {
      if ($line -cnotmatch '^scroll (document|province) (\S+)$') { throw "Unsupported review scroll: $line" }
      [float]$scrollOffset = 0
      if (-not [float]::TryParse($Matches[2], [Globalization.NumberStyles]::Float, [Globalization.CultureInfo]::InvariantCulture, [ref]$scrollOffset) -or [float]::IsNaN($scrollOffset) -or [float]::IsInfinity($scrollOffset) -or $scrollOffset -lt 0 -or $scrollOffset -gt 5000) { throw "Review scroll offset must be finite and between 0 and 5000: $line" }
    }
    if ($line -match '^battle(?:\s|$)') { Assert-BattleReviewCommand $line }
    if ($line -match '^victory(?:\s|$)' -and $line -cnotmatch '^victory (recognize|bonus|decline)$') { throw "Unsupported victory decision: $line" }
    if ($line -match '^victory-close(?:\s|$)' -and $line -cne 'victory-close') { throw "Victory close takes no arguments: $line" }
    if ($line -match '^panel(?:\s|$)' -and $line -cnotmatch '^panel (council|economy|journal|mandate|accord|victory|initiative|establishment|officers|reform)$') { throw "Unsupported review panel: $line" }
    if ($line -match '^reform(?:\s|$)' -and $line -cnotmatch '^reform ((draft|begin) (provisioning|commerce)|end)$') { throw "Unsupported regional reform order: $line" }
    if ($line -match '^expect\s+HasRegionalReform(?:\s|$)' -and $line -cnotmatch '^expect HasRegionalReform (True|False)$') { throw "Regional reform assertion requires True or False: $line" }
    if ($line -match '^expect\s+ReformStatus(?:\s|$)' -and $line -cnotmatch '^expect ReformStatus (closed|proposed|pending|blocked|active)$') { throw "Unsupported regional reform status assertion: $line" }
    if ($line -match '^commission(?:\s|$)' -and $line -cnotmatch '^commission (grant|recruit|revoke)$') { throw "Unsupported officer commission order: $line" }
    if ($line -match '^expect\s+(HasOfficerCommission|DumasOfficerCommission|DumasExtraRecruitUsed)(?:\s|$)' -and $line -cnotmatch '^expect (HasOfficerCommission|DumasOfficerCommission|DumasExtraRecruitUsed) (True|False)$') { throw "Officer commission assertion requires True or False: $line" }
    if ($line -match '^expect\s+ResistanceActive(?:\s|$)' -and $line -cnotmatch '^expect ResistanceActive (True|False)$') { throw "Resistance assertion requires True or False: $line" }
    if ($line -match '^establishment(?:\s|$)') {
      if ($line -cnotmatch '^establishment (campaign 0|budget [0-9]+)$') { throw "Unsupported army establishment order: $line" }
      [int]$armyTarget = 0
      if (-not [int]::TryParse(($line -split ' ')[2], [ref]$armyTarget) -or $armyTarget -gt 100000000) { throw "Army establishment target is outside the supported range: $line" }
    }
    if ($line -match '^expect\s+HasArmyEstablishment(?:\s|$)' -and $line -cnotmatch '^expect HasArmyEstablishment (True|False)$') { throw "Army establishment assertion requires True or False: $line" }
    if ($line -match '^forage(?:\s|$)' -and $line -cne 'forage veto') { throw "Unsupported Dumas response: $line" }
    if ($line -match '^expect\s+HasDumasInitiative(?:\s|$)' -and $line -cnotmatch '^expect HasDumasInitiative (True|False)$') { throw "Dumas initiative assertion requires True or False: $line" }
    if ($line -match '^expect\s+HasPendingVictory(?:\s|$)' -and $line -cnotmatch '^expect HasPendingVictory (True|False)$') { throw "Pending victory assertion requires True or False: $line" }
    if ($line -match '^(shot|state|battle\s+state)\s+(.+)$') {
      $kind = $Matches[1]; $name = $Matches[2]
      if ($name -notmatch '\A[a-zA-Z0-9][a-zA-Z0-9_-]{0,79}\z') { throw "Unsafe artifact name: $name" }
      if ($kind -eq 'shot') { $captures += "$name.png" } else { $states += "$name.json" }
    }
    if ($line -match '^(expect|same)\s+' -or $line -eq 'battle verify-return' -or $line -match '^world wait ') { $assertions++ }
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
