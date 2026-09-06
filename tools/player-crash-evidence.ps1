# Yalniz belirtilen oyuncu ve dar zaman araligi icin salt okunur tani.
param(
  [Parameter(Mandatory = $true)][string]$PlayerPath,
  [Parameter(Mandatory = $true)][DateTimeOffset]$AtUtc,
  [ValidateRange(1, 15)][int]$RadiusMinutes = 3
)
$ErrorActionPreference = 'Stop'
$target = [IO.Path]::GetFullPath($PlayerPath)
$name = [IO.Path]::GetFileName($target)
$startUtc = $AtUtc.UtcDateTime.AddMinutes(-$RadiusMinutes)
$endUtc = $AtUtc.UtcDateTime.AddMinutes($RadiusMinutes)
Write-Output "Target: $target"
Write-Output "UTC interval: $($startUtc.ToString('O')) to $($endUtc.ToString('O'))"
try {
  $events = @(Get-WinEvent -FilterHashtable @{ LogName = 'Application'; Id = 1000, 1001; StartTime = $startUtc.ToLocalTime(); EndTime = $endUtc.ToLocalTime() } -ErrorAction Stop)
  $matched = 0
  foreach ($event in $events) {
    $xml = $event.ToXml()
    if ($xml.IndexOf($target, [StringComparison]::OrdinalIgnoreCase) -lt 0) { continue }
    $matched++
    [xml]$document = $xml
    $data = [ordered]@{ eventId = $event.Id; utc = $event.TimeCreated.ToUniversalTime().ToString('O'); provider = $event.ProviderName }
    foreach ($item in $document.Event.EventData.Data) { $data[[string]$item.Name] = [string]$item.'#text' }
    Write-Output ($data | ConvertTo-Json -Depth 4)
  }
  Write-Output "Exact-path Application events: $matched"
} catch {
  if ($_.FullyQualifiedErrorId -like 'NoMatchingEventsFound*') { Write-Output 'No Application crash events in interval.' }
  else { Write-Output "Application event query unavailable: $($_.Exception.Message)" }
}
$werRoots = @(
  (Join-Path $env:ProgramData 'Microsoft\Windows\WER\ReportArchive'),
  (Join-Path $env:ProgramData 'Microsoft\Windows\WER\ReportQueue'),
  (Join-Path $env:LOCALAPPDATA 'Microsoft\Windows\WER\ReportArchive'),
  (Join-Path $env:LOCALAPPDATA 'Microsoft\Windows\WER\ReportQueue')
)
foreach ($directory in $werRoots) {
  if (-not (Test-Path -LiteralPath $directory -PathType Container)) { continue }
  try {
    foreach ($folder in Get-ChildItem -LiteralPath $directory -Directory -ErrorAction Stop) {
      if ($folder.LastWriteTimeUtc -lt $startUtc -or $folder.LastWriteTimeUtc -gt $endUtc -or $folder.Name -notlike 'AppCrash_Power*') { continue }
      $report = Join-Path $folder.FullName 'Report.wer'
      if (-not (Test-Path -LiteralPath $report -PathType Leaf)) { continue }
      $body = [IO.File]::ReadAllText($report)
      if ($body.IndexOf($target, [StringComparison]::OrdinalIgnoreCase) -lt 0) { continue }
      Write-Output "WER: $report"
      $body -split '\r?\n' | Where-Object { $_ -match '^(EventType|AppName|AppPath|ReportIdentifier|Sig\[\d+\]\.(Name|Value))=' } | Write-Output
    }
  } catch { Write-Output "WER directory unavailable: $directory ($($_.Exception.Message))" }
}
$dumps = Join-Path $env:LOCALAPPDATA 'CrashDumps'
if (Test-Path -LiteralPath $dumps -PathType Container) {
  Get-ChildItem -LiteralPath $dumps -File -Filter "$name*.dmp" -ErrorAction SilentlyContinue |
    Where-Object { $_.LastWriteTimeUtc -ge $startUtc -and $_.LastWriteTimeUtc -le $endUtc } |
    Select-Object FullName, Length, LastWriteTimeUtc | ConvertTo-Json
}
