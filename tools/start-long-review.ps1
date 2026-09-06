param([Parameter(Mandatory=$true)][string]$PlayerPath,[switch]$VisiblePlayer)
$ErrorActionPreference='Stop'
if(-not $VisiblePlayer){throw 'Long gameplay review requires explicit -VisiblePlayer.'}
. (Join-Path $PSScriptRoot 'verify-support.ps1')
$repo=Split-Path -Parent $PSScriptRoot
$PlayerPath=[IO.Path]::GetFullPath($PlayerPath)
$root=[IO.Path]::GetFullPath((Join-Path $repo 'output\verify'))+'\'
if(-not $PlayerPath.StartsWith($root,[StringComparison]::OrdinalIgnoreCase)){throw 'Use a player under output/verify.'}
[void](Assert-ReviewProtocol $PlayerPath)
$label='natural-live-'+[Guid]::NewGuid().ToString('N').Substring(0,8)
$arguments=@('-NoProfile','-ExecutionPolicy','Bypass','-File',(Join-Path $PSScriptRoot 'review-player.ps1'),'-PlayerPath',$PlayerPath,'-Label',$label,'-ScriptPath',(Join-Path $PSScriptRoot 'realtime-natural.script'),'-VisiblePlayer','-TimeoutSeconds','1200')
$quoted=($arguments | ForEach-Object {'"'+$_+'"'}) -join ' '
# Ebeveynin çıkışıyla kapanan boru/stdio devralınmaz. Sahip kendi REPORT/result dosyasını yazar.
$owner=Start-Process -FilePath (Join-Path $PSHOME 'powershell.exe') -ArgumentList $quoted -WorkingDirectory $repo -WindowStyle Hidden -PassThru
try {
    $handle=$owner.Handle
    Start-Sleep -Milliseconds 1500
    if($owner.HasExited){throw 'Long review owner exited before readiness.'}
    Write-Output ('Owned review helper PID '+$owner.Id+'; report directory prefix: '+(Join-Path $root $label))
} finally {$owner.Dispose()}
