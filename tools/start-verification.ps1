param(
  [Parameter(Mandatory=$true)][string]$ProjectPath,
  [Parameter(Mandatory=$true)][string]$ScriptPath,
  [ValidatePattern('^[a-zA-Z0-9_-]+$')][string]$Label='review',
  [switch]$VisiblePlayer
)
$ErrorActionPreference='Stop'
if(-not $VisiblePlayer){throw 'This review requires explicit -VisiblePlayer.'}
$repo=Split-Path -Parent $PSScriptRoot
$ProjectPath=[IO.Path]::GetFullPath($ProjectPath)
$ScriptPath=[IO.Path]::GetFullPath($ScriptPath)
$isolated=[IO.Path]::GetFullPath((Join-Path $repo 'output\isolated'))+'\'
if(-not $ProjectPath.StartsWith($isolated,[StringComparison]::OrdinalIgnoreCase)){throw 'Use an isolated review copy.'}
if(-not (Test-Path -LiteralPath $ScriptPath -PathType Leaf)){throw 'Review script missing.'}
. (Join-Path $PSScriptRoot 'verify-support.ps1')
[void](Get-ReviewPlan $ScriptPath)
$arguments=@('-NoProfile','-ExecutionPolicy','Bypass','-File',(Join-Path $PSScriptRoot 'verify.ps1'),'-ProjectPath',$ProjectPath,'-Label',$Label,'-ScriptPath',$ScriptPath,'-VisiblePlayer')
$quoted=($arguments | ForEach-Object {ConvertTo-NativeArgument $_}) -join ' '
# Yardımcı, ebeveyn aracın kısa ömürlü stdout borusunu devralmaz; sonucu verify.ps1 yazar.
$owner=Start-Process -FilePath (Join-Path $PSHOME 'powershell.exe') -ArgumentList $quoted -WorkingDirectory $repo -WindowStyle Hidden -PassThru
try {
  $handle=$owner.Handle
  Start-Sleep -Milliseconds 1500
  if($owner.HasExited){throw 'Verification helper exited before readiness.'}
  Write-Output ('Owned verification helper PID '+$owner.Id+'; report prefix '+$Label)
} finally {$owner.Dispose()}
