# Unity/player baslatmadan gercek Python kontrolcusunun surec ve rapor siniri.
param([string]$SourceShots = '')
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'verify-support.ps1')
$repo = Split-Path -Parent $PSScriptRoot
$out = Join-Path $repo ('output\frame-check-tests-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $out | Out-Null
$checker = Join-Path $PSScriptRoot 'shot-check.py'
$count = 0
function Check([bool]$Condition, [string]$Name) {
  if (-not $Condition) { throw "FAIL: $Name" }
  $script:count++; Write-Output "PASS: $Name"
}
foreach ($kind in @('empty', 'missing')) {
  $caseDirectory = Join-Path $out $kind
  $shots = Join-Path $caseDirectory 'shots'
  New-Item -ItemType Directory -Path $caseDirectory | Out-Null
  if ($kind -eq 'empty') { New-Item -ItemType Directory -Path $shots | Out-Null }
  $failure = ''
  try { $null = Invoke-FrameReview $checker $shots $caseDirectory } catch { $failure = $_.Exception.Message }
  if (-not (Test-Path -LiteralPath (Join-Path $caseDirectory 'frames-process.json'))) { throw "Frame process receipt missing; failure: $failure" }
  $process = [IO.File]::ReadAllText((Join-Path $caseDirectory 'frames-process.json')) | ConvertFrom-Json
  Check ($failure -like 'Frame checker exited 1*' -and $process.exitCode -eq 1) "$kind folder keeps real checker failure and process receipt"
  if ($kind -eq 'empty') {
    Check ([IO.File]::ReadAllText((Join-Path $caseDirectory 'frames.log')).Contains('no frames found')) 'Checker stdout survives nonzero exit'
  } else {
    Check ([IO.File]::ReadAllText((Join-Path $caseDirectory 'frames.stderr.log')).Contains('FileNotFoundError')) 'Python exception survives in stderr without terminating parent PowerShell'
  }
}
if ($SourceShots) {
  $caseDirectory = Join-Path $out 'actual-copies'
  $shots = Join-Path $caseDirectory 'shots'
  New-Item -ItemType Directory -Path $shots | Out-Null
  $source = @(Get-ChildItem -LiteralPath $SourceShots -File -Filter '*.png')
  Check ($source.Count -gt 0) 'Source has captured frames to copy'
  foreach ($frame in $source) { [IO.File]::Copy($frame.FullName, (Join-Path $shots $frame.Name), $false) }
  $summary = Invoke-FrameReview $checker $shots $caseDirectory
  $process = [IO.File]::ReadAllText((Join-Path $caseDirectory 'frames-process.json')) | ConvertFrom-Json
  $parsedFrames = [IO.File]::ReadAllText((Join-Path $shots 'frames.json')) | ConvertFrom-Json
  $frames = @($parsedFrames)
  Check ($process.exitCode -eq 0 -and $frames.Count -eq $source.Count) "Actual captured frame copies complete with all results: $summary"
}
Write-Output "$count frame process checks passed. Original frames preserved; artifacts: $out"
