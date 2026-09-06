param([ValidatePattern('^[a-zA-Z0-9_-]+$')][string]$Label='atlas')
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent $PSScriptRoot
$root=[IO.Path]::GetFullPath((Join-Path $repo 'output\isolated'))
$target=[IO.Path]::GetFullPath((Join-Path $root ($Label+'-'+[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss')+'-'+[Guid]::NewGuid().ToString('N').Substring(0,8))))
if(-not $target.StartsWith($root+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)) { throw 'Copy destination escaped output/isolated.' }
if(Test-Path -LiteralPath $target) { throw 'Review destination already exists.' }
$null=New-Item -ItemType Directory -Path $target
foreach($folder in @('Assets','Packages','ProjectSettings')) {
  Copy-Item -LiteralPath (Join-Path (Join-Path $repo 'Unity') $folder) -Destination $target -Recurse
}
Write-Output $target
