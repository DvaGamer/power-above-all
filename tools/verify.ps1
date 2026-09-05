# Guvenli dogrulama kapisi. ASCII: Windows PowerShell 5.1 uyumlulugu.
# powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\verify.ps1 -Label baseline
# -ProjectPath kapali veya yalitilmis Unity projesini secer. Acik editorler kapatilmaz.
# -SkipBuild -PlayerPath <exe> onceki oyuncuyu acikca secer; sonuc PARTIAL olur.
# -VisiblePlayer yalniz onceden yetkilendirilmis gorunur oyun onizlemesini acar; yardimcilar gizli kalir.
param(
  [string]$ProjectPath = '',
  [ValidatePattern('^[A-Za-z0-9][A-Za-z0-9_-]{0,63}$')][string]$Label = 'verify',
  [string]$UnityPath = 'C:\Users\USER\Tools\Unity\6000.3.23f1\Editor\Unity.exe',
  [string]$PlayerPath = '',
  [string]$ScriptPath = '',
  [switch]$VisiblePlayer,
  [switch]$SkipShots,
  [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'verify-support.ps1')
$repo = Split-Path -Parent $PSScriptRoot
if (-not $ProjectPath) { $ProjectPath = Join-Path $repo 'Unity' }
if (-not $ScriptPath) { $ScriptPath = Join-Path $PSScriptRoot 'shots.script' }
$ProjectPath = [IO.Path]::GetFullPath($ProjectPath)
$runName = $Label + '-' + [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss-fff') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$out = Join-Path (Join-Path $repo 'output\verify') $runName
New-Item -ItemType Directory -Path $out | Out-Null
$started = [DateTime]::UtcNow
$failures = New-Object 'Collections.Generic.List[string]'
$notes = New-Object 'Collections.Generic.List[string]'
$gates = [ordered]@{ Preflight = 'NOT RUN'; EditMode = 'NOT RUN'; Build = 'NOT RUN'; Player = 'NOT RUN'; Frames = 'NOT RUN'; Browser = 'NOT RUN' }
function Say([string]$Text) { $notes.Add($Text); Write-Output $Text }
function Failed([string]$Gate, [string]$Reason) { $gates[$Gate] = 'FAILED'; $failures.Add("${Gate}: $Reason"); Say "${Gate}: FAILED - $Reason" }

Say "Artifacts: $out"
$playerWindow = 'Hidden'
if ($VisiblePlayer) { $playerWindow = 'Visible (explicit -VisiblePlayer)' }
Say "Requested player window: $playerWindow; editor/test/helper windows remain hidden."
try {
try {
  if (-not (Test-Path -LiteralPath $UnityPath -PathType Leaf)) { throw "Unity executable missing: $UnityPath" }
  if (-not (Test-Path -LiteralPath (Join-Path $ProjectPath 'ProjectSettings\ProjectVersion.txt') -PathType Leaf)) { throw "Not a Unity project: $ProjectPath" }
  if (Test-Path -LiteralPath (Join-Path $ProjectPath 'Temp\UnityLockfile')) { throw "Project lock exists. Leave the user's editor running and select an isolated -ProjectPath, or close that project deliberately before retrying. No process was stopped." }
  if ($PlayerPath -and -not $SkipBuild) { throw '-PlayerPath requires -SkipBuild; a new build chooses its own unique output.' }
  if ($SkipBuild -and -not $SkipShots -and -not $PlayerPath) { throw '-SkipBuild with player review requires an explicit -PlayerPath.' }
  if (-not $SkipShots) { $plan = Get-ReviewPlan $ScriptPath }
  $gates.Preflight = 'PASSED'
} catch { Failed 'Preflight' $_.Exception.Message }

if ($gates.Preflight -eq 'PASSED') {
  try {
    Say 'Running EditMode tests in the selected closed project...'
    $testXml = Join-Path $out 'edit-tests.xml'
    $testLog = Join-Path $out 'edit-tests.log'
    $testExit = Invoke-OwnedProcess $UnityPath @('-runTests', '-batchmode', '-nographics', '-projectPath', $ProjectPath, '-testPlatform', 'EditMode', '-testResults', $testXml, '-logFile', $testLog) 1200 $repo
    $summary = Get-EditTestSummary $testXml $testExit
    $null = Assert-CleanLog $testLog
    $gates.EditMode = "PASSED: $summary"
    Say "EditMode: $summary"
  } catch { Failed 'EditMode' $_.Exception.Message }

  if ($SkipBuild) {
    $gates.Build = 'SKIPPED: explicitly requested; current source is not build-verified'
    if ($PlayerPath) { $PlayerPath = [IO.Path]::GetFullPath($PlayerPath) }
  } elseif ($gates.EditMode.StartsWith('PASSED')) {
    try {
      $buildDir = Join-Path $out 'player-build'
      if (Test-Path -LiteralPath $buildDir) { throw 'Fresh build directory unexpectedly exists; preserving it and refusing reuse.' }
      $buildLog = Join-Path $out 'build.log'
      Say "Building a fresh Windows player: $buildDir"
      $buildExit = Invoke-OwnedProcess $UnityPath @('-batchmode', '-nographics', '-quit', '-projectPath', $ProjectPath, '-logFile', $buildLog, '-executeMethod', 'PowerAboveAll.Editor.BuildTools.BuildWindowsVerification', '-verificationBuildPath', $buildDir) 1800 $repo
      $buildText = Assert-CleanLog $buildLog
      if ($buildExit -ne 0 -or $buildText -notmatch 'Power Above All build succeeded:') { throw "Build failed or missing completion marker (exit $buildExit); see $buildLog" }
      $PlayerPath = Join-Path $buildDir 'Power Above All.exe'
      $assembly = Assert-ReviewProtocol $PlayerPath
      $buildFiles = @(Get-BuildFileManifest $buildDir)
      $buildReceipt = [ordered]@{ sourceProject = $ProjectPath; playerPath = $PlayerPath; assemblyPath = $assembly; builtUtc = [DateTime]::UtcNow.ToString('O'); playerSha256 = (Get-FileHash -LiteralPath $PlayerPath -Algorithm SHA256).Hash; assemblySha256 = (Get-FileHash -LiteralPath $assembly -Algorithm SHA256).Hash; manifestVersion = 1; files = $buildFiles }
      [IO.File]::WriteAllText((Join-Path $out 'build-result.json'), ($buildReceipt | ConvertTo-Json -Depth 5), [Text.Encoding]::UTF8)
      $gates.Build = 'PASSED: fresh build in this run'
      Say "Build: PASSED (fresh output, review protocol 2, $($buildFiles.Count) shipped-file hashes)."
    } catch { Failed 'Build' $_.Exception.Message }
  } else { $gates.Build = 'NOT RUN: EditMode gate failed' }

  if ($SkipShots) {
    $gates.Player = 'SKIPPED: explicitly requested'; $gates.Frames = 'SKIPPED: explicitly requested'
  } elseif (($SkipBuild -or $gates.Build.StartsWith('PASSED')) -and $gates.EditMode.StartsWith('PASSED')) {
    try {
      $assembly = Assert-ReviewProtocol $PlayerPath
      Say "Reviewing player SHA256 $((Get-FileHash -LiteralPath $PlayerPath -Algorithm SHA256).Hash); assembly SHA256 $((Get-FileHash -LiteralPath $assembly -Algorithm SHA256).Hash)"
      $shotDir = Join-Path $out 'shots'
      $playerLog = Join-Path $out 'player.log'
      $playerExit = Invoke-OwnedProcess $PlayerPath @('-shots', $shotDir, '-script', $ScriptPath, '-logFile', $playerLog, '-screen-width', '1440', '-screen-height', '900', '-screen-fullscreen', '0') 300 $repo -Visible:$VisiblePlayer
      $null = Assert-CleanLog $playerLog
      $shotSummary = Assert-ReviewResult $shotDir $plan $playerExit
      $gates.Player = "PASSED: $shotSummary"
      Say "Player: $shotSummary"
    } catch { Failed 'Player' $_.Exception.Message }
    if ($gates.Player.StartsWith('PASSED')) {
      try {
        $checker = Join-Path $PSScriptRoot 'shot-check.py'
        $frameSummary = Invoke-FrameReview $checker $shotDir $out
        $gates.Frames = "PASSED: $frameSummary"
        Say "Frames: $frameSummary"
      } catch { Failed 'Frames' $_.Exception.Message }
    } else { $gates.Frames = 'NOT RUN: player gate failed' }
  } else { $gates.Player = 'NOT RUN: tests/build failed'; $gates.Frames = 'NOT RUN: player gate did not run' }
}

# Tarayici referans testi Unity basarisindan bagimsizdir; surec gizli baslatilir.
try {
  $nodePath = (Get-Command node.exe -ErrorAction Stop).Source
  $browserXml = Join-Path $out 'browser-tests.xml'
  $browserExit = Invoke-OwnedProcess $nodePath @('--test', '--test-reporter=junit', "--test-reporter-destination=$browserXml", (Join-Path $repo 'tests\simulation.test.cjs')) 120 $repo
  if ($browserExit -ne 0 -or -not (Test-Path -LiteralPath $browserXml -PathType Leaf)) { throw "Browser test runner failed or result missing (exit $browserExit)." }
  [xml]$browserResult = [IO.File]::ReadAllText($browserXml)
  $browserCases = @($browserResult.SelectNodes('//testcase'))
  if ($browserCases.Count -eq 0 -or @($browserResult.SelectNodes('//failure|//error|//skipped')).Count -gt 0) { throw 'Browser tests failed, skipped, or ran zero cases.' }
  $gates.Browser = "PASSED: $($browserCases.Count) tests"
  Say $gates.Browser
} catch { Failed 'Browser' $_.Exception.Message }
} catch {
  Failed 'Verifier' ($_.Exception.Message + ' at ' + $_.ScriptStackTrace)
} finally {
# Beklenmeyen PowerShell hatasi da tamamlanan/atlanmis kontrolleri raporda birakir.
$verdict = 'GREEN'
if ($failures.Count -gt 0) { $verdict = 'RED' }
elseif (@($gates.Values | Where-Object { -not $_.StartsWith('PASSED') }).Count -gt 0) { $verdict = 'PARTIAL' }
$elapsed = [int]([DateTime]::UtcNow - $started).TotalSeconds
$report = @("# Dogrulama raporu - $Label", '', "UTC: $($started.ToString('O')); sure: ${elapsed}s; sonuc: $verdict", '', "Proje: $ProjectPath", "Istenen oyuncu penceresi: $playerWindow", '', '| Kontrol | Sonuc |', '| --- | --- |')
foreach ($gate in $gates.GetEnumerator()) { $report += "| $($gate.Key) | $($gate.Value) |" }
$report += @('', 'Goruntu kontrolleri insan tarafindan gorsel kabul veya ses dinlemesi anlamina gelmez.', '', '## Hatalar')
foreach ($failure in $failures) { $report += "- $failure" }
$report += @('', '## Gunluk', '```') + $notes.ToArray() + @('```')
$reportPath = Join-Path $out 'REPORT.md'
[IO.File]::WriteAllLines($reportPath, $report, [Text.Encoding]::UTF8)
[IO.File]::WriteAllText((Join-Path $out 'result.json'), ([ordered]@{ label = $Label; verdict = $verdict; gates = $gates; failures = $failures.ToArray(); artifacts = $out; playerWindow = $playerWindow; elapsedSeconds = $elapsed } | ConvertTo-Json -Depth 5), [Text.Encoding]::UTF8)
Say "Verdict: $verdict ($reportPath)"
}
if ($verdict -eq 'RED') { exit 1 }
exit 0
