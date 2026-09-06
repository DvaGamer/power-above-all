# Mevcut oyuncunun bagimsiz incelemesi. Unity/editor/build/tarayici baslatilmaz.
# Basarili kosu PARTIAL'dir; yeni kaynak veya tam derleme kapisi olarak secilemez.
param(
  [Parameter(Mandatory = $true)][string]$PlayerPath,
  [ValidatePattern('^[A-Za-z0-9][A-Za-z0-9_-]{0,63}$')][string]$Label = 'player-review',
  [string]$ScriptPath = '',
  [ValidateSet('Default', 'Direct3D11', 'Direct3D12')][string]$GraphicsApi = 'Default',
  [switch]$VisiblePlayer,
  [ValidateRange(640,7680)][int]$Width=1440,
  [ValidateRange(480,4320)][int]$Height=900
)
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'verify-support.ps1')
$repo = Split-Path -Parent $PSScriptRoot
if (-not $ScriptPath) { $ScriptPath = Join-Path $PSScriptRoot 'shots.script' }
$PlayerPath = [IO.Path]::GetFullPath($PlayerPath)
$ScriptPath = [IO.Path]::GetFullPath($ScriptPath)
$started = [DateTime]::UtcNow
$runName = $Label + '-' + $started.ToString('yyyyMMdd-HHmmss-fff') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$out = Join-Path (Join-Path $repo 'output\verify') $runName
New-Item -ItemType Directory -Path $out | Out-Null
$failures = New-Object 'Collections.Generic.List[string]'
$notes = New-Object 'Collections.Generic.List[string]'
$gates = [ordered]@{ Preflight = 'NOT RUN'; EditMode = 'SKIPPED: player-only review'; Build = 'SKIPPED: explicit existing build; current source not verified'; Player = 'NOT RUN'; Frames = 'NOT RUN'; Browser = 'SKIPPED: player-only review'; BuildUnchanged = 'NOT RUN' }
$playerWindow = 'Hidden'
if ($VisiblePlayer) { $playerWindow = 'Visible (explicit -VisiblePlayer)' }
function Say([string]$Text) { $notes.Add($Text); Write-Output $Text }
function Failed([string]$Gate, [string]$Reason) { $gates[$Gate] = 'FAILED'; $failures.Add("${Gate}: $Reason"); Say "${Gate}: FAILED - $Reason" }
Say "Artifacts: $out"
Say "Reused player: $PlayerPath; graphics: $GraphicsApi; window: $playerWindow"
$playerExit = $null
try {
  $assembly = Assert-ReviewProtocol $PlayerPath
  $buildDir = Split-Path -Parent $PlayerPath
  $before = @(Get-BuildFileManifest $buildDir)
  $beforeJson = ConvertTo-Json -InputObject $before -Depth 4 -Compress
  $scriptCopy = Join-Path $out 'review.script'
  [IO.File]::Copy($ScriptPath, $scriptCopy, $false)
  $plan = Get-ReviewPlan $scriptCopy
  $receipt = [ordered]@{ reusedBuild = $true; currentSourceVerified = $false; playerPath = $PlayerPath; assemblyPath = $assembly; capturedUtc = $started.ToString('O'); playerSha256 = (Get-FileHash -LiteralPath $PlayerPath -Algorithm SHA256).Hash; assemblySha256 = (Get-FileHash -LiteralPath $assembly -Algorithm SHA256).Hash; manifestVersion = 1; files = $before; scriptSource = $ScriptPath; scriptSha256 = (Get-FileHash -LiteralPath $scriptCopy -Algorithm SHA256).Hash; requestedGraphicsApi = $GraphicsApi }
  [IO.File]::WriteAllText((Join-Path $out 'reused-build.json'), ($receipt | ConvertTo-Json -Depth 5), [Text.Encoding]::UTF8)
  $gates.Preflight = "PASSED: isolated-save protocol 2; $($before.Count) reused build files recorded"
} catch { Failed 'Preflight' $_.Exception.Message }

if ($gates.Preflight.StartsWith('PASSED')) {
  $shotDir = Join-Path $out 'shots'
  $playerLog = Join-Path $out 'player.log'
  try {
    $arguments = @('-shots', $shotDir, '-script', $scriptCopy, '-logFile', $playerLog, '-screen-width', [string]$Width, '-screen-height', [string]$Height, '-screen-fullscreen', '0') + @(Get-ReviewGraphicsArguments $GraphicsApi)
    $playerExit = Invoke-OwnedProcess $PlayerPath $arguments 300 $repo -Visible:$VisiblePlayer
    Say "Player native exit: $playerExit"
    $playerText = Assert-CleanLog $playerLog
    Assert-ReviewGraphics $playerText $GraphicsApi
    $summary = Assert-ReviewResult $shotDir $plan $playerExit
    $gates.Player = "PASSED: $summary; native exit 0"
    Say $gates.Player
  } catch { Failed 'Player' $_.Exception.Message }
  # Cikis hatasi oyuncu kapisini kirmaya devam eder; var olan kareler tani icin ayri kontrol edilir.
  if (Test-Path -LiteralPath $shotDir -PathType Container) {
    try {
      $frameSummary = Invoke-FrameReview (Join-Path $PSScriptRoot 'shot-check.py') $shotDir $out -Width $Width -Height $Height
      $gates.Frames = "PASSED: $frameSummary"
      Say $gates.Frames
    } catch { Failed 'Frames' $_.Exception.Message }
  } else { $gates.Frames = 'NOT RUN: no capture directory' }
  try {
    $after = @(Get-BuildFileManifest $buildDir)
    if ((ConvertTo-Json -InputObject $after -Depth 4 -Compress) -ne $beforeJson) { throw 'Reused build files changed during review; evidence is not a stable same-build comparison.' }
    $gates.BuildUnchanged = "PASSED: $($before.Count) file paths, sizes and SHA256 values unchanged"
  } catch { Failed 'BuildUnchanged' $_.Exception.Message }
}
$verdict = 'PARTIAL'
if ($failures.Count -gt 0) { $verdict = 'RED' }
$elapsed = [int]([DateTime]::UtcNow - $started).TotalSeconds
$result = [ordered]@{ label = $Label; verdict = $verdict; reusedBuild = $true; gates = $gates; failures = $failures.ToArray(); artifacts = $out; playerPath = $PlayerPath; playerWindow = $playerWindow; requestedGraphicsApi = $GraphicsApi; playerExitCode = $playerExit; elapsedSeconds = $elapsed }
$report = @("# Oyuncu incelemesi - $Label", '', "UTC: $($started.ToString('O')); sure: ${elapsed}s; sonuc: $verdict", '', 'Mevcut derleme yeniden kullanildi. Unity testleri, yeni kaynak derlemesi ve tarayici testleri calistirilmadi.', "Oyuncu: $PlayerPath", "Grafik: $GraphicsApi; pencere: $playerWindow; native cikis: $playerExit", '', '| Kontrol | Sonuc |', '| --- | --- |')
foreach ($gate in $gates.GetEnumerator()) { $report += "| $($gate.Key) | $($gate.Value) |" }
$report += @('', '## Hatalar')
foreach ($failure in $failures) { $report += "- $failure" }
$report += @('', '## Gunluk', '```') + $notes.ToArray() + @('```')
[IO.File]::WriteAllLines((Join-Path $out 'REPORT.md'), $report, [Text.Encoding]::UTF8)
[IO.File]::WriteAllText((Join-Path $out 'result.json'), ($result | ConvertTo-Json -Depth 5), [Text.Encoding]::UTF8)
Say "Verdict: $verdict ($out\REPORT.md)"
if ($verdict -eq 'RED') { exit 1 }
exit 0
