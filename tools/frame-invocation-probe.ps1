# Unity/player acmadan mevcut karelerin KOPYASIYLA eski pipeline davranisini inceler.
param([Parameter(Mandatory = $true)][string]$SourceShots)
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'verify-support.ps1')
$repo = Split-Path -Parent $PSScriptRoot
$out = Join-Path $repo ('output\frame-invocation-probe-' + [Guid]::NewGuid().ToString('N'))
$shots = Join-Path $out 'shots'
New-Item -ItemType Directory -Path $shots | Out-Null
foreach ($frame in Get-ChildItem -LiteralPath $SourceShots -File -Filter '*.png') { [IO.File]::Copy($frame.FullName, (Join-Path $shots $frame.Name), $false) }
Write-Output "Probe: $out"
$python = Get-Command python -ErrorAction Stop
Write-Output "Python command: $($python.CommandType); $($python.Source)"
$checker = Join-Path $PSScriptRoot 'shot-check.py'
try {
  Write-Output 'Before inline Python invocation.'
  $checkOutput = @(& python $checker $shots 2>&1)
  Write-Output "Returned from inline Python: LASTEXITCODE=$LASTEXITCODE; lines=$($checkOutput.Count)"
  [IO.File]::WriteAllLines((Join-Path $out 'frames.log'), [string[]]$checkOutput, [Text.Encoding]::UTF8)
  Write-Output 'Captured output written.'
} catch {
  [IO.File]::WriteAllText((Join-Path $out 'caught.txt'), ($_ | Out-String), [Text.Encoding]::UTF8)
  Write-Output "Caught: $($_.Exception.Message); $($_.ScriptStackTrace)"
} finally {
  [IO.File]::WriteAllText((Join-Path $out 'finally.txt'), [DateTime]::UtcNow.ToString('O'), [Text.Encoding]::UTF8)
  Write-Output 'Finally reached.'
}
