# Yalniz parser/provenance kontrolleri; oyuncu, pencere veya girdi baslatilmaz.
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'native-input-owner.ps1')
$count = 0
function Check([bool]$Condition, [string]$Name) {
    if (-not $Condition) { throw "FAIL: $Name" }
    $script:count++; Write-Output "PASS: $Name"
}
function Reject([scriptblock]$Action, [string]$Name) {
    $failed = $false
    try { $null = & $Action } catch { $failed = $true }
    Check $failed $Name
}
foreach ($file in @('native-input-owner.ps1', 'native-input-review.ps1', 'native-input.tests.ps1', 'verify-support.ps1')) {
    $tokens = $null; $errors = $null
    $null = [Management.Automation.Language.Parser]::ParseFile((Join-Path $PSScriptRoot $file), [ref]$tokens, [ref]$errors)
    Check ($errors.Count -eq 0) "PowerShell syntax: $file $errors"
}
$folder = Join-Path $nativeReviewRoot 'native-input-pure-fixture'
$player = Join-Path $nativeReviewRoot 'build with spaces\Power Above All.exe'
$argsForPlayer = @($player) + @(Get-NativePlayerArguments $folder)
$command = ($argsForPlayer | ForEach-Object { ConvertTo-NativeArgument $_ }) -join ' '
Assert-NativeCommandLine $command $argsForPlayer
Check $true 'Windows quoted player and exact isolated arguments accepted'
Reject { Assert-NativeCommandLine (ConvertTo-NativeArgument $player) $argsForPlayer } 'Human launch of same executable without isolated arguments rejected'
Reject { Assert-NativeCommandLine ($command + ' -shots C:\human-save') $argsForPlayer } 'Appended duplicate shots path rejected'
Reject { Assert-NativeCommandLine ($command.Replace('review.script', 'different.script')) $argsForPlayer } 'Changed script argument rejected'
Reject { Assert-NativeCommandLine ($command.Replace('native-input-pure-fixture', 'another-run')) $argsForPlayer } 'Same player aimed at another review directory rejected'
$escaped = @('C:\Program Files\tool.exe', 'trailing\', 'quote"literal')
Assert-NativeCommandLine (($escaped | ForEach-Object { ConvertTo-NativeArgument $_ }) -join ' ') $escaped
Check $true 'Windows native parser preserves embedded quotes and trailing slash'
Reject { Assert-NativeCommandLine '' $argsForPlayer } 'Unavailable live command line fails closed'
Reject { Assert-NativeReviewPath (Join-Path $nativeRepo 'output\verify-elsewhere\receipt.json') } 'Similar folder prefix does not authorize another root'
Reject { Assert-NativeReviewPath (Join-Path $nativeReviewRoot '..\outside.json') } 'Parent traversal cannot escape review root'
$fingerprint = 'a' * 64
$record = [ordered]@{
    launchedBy = 'PowerAboveAllNativeReview2'; outputPath = $folder; playerPath = $player
    scriptPath = Join-Path $folder 'review.script'; assemblyPath = Join-Path (Split-Path -Parent $player) 'Power Above All_Data\Managed\PowerAboveAll.Runtime.dll'
    ownerScript = $nativeOwnerScript; ownerPath = Join-Path $PSHOME 'powershell.exe'
    processId = 101; ownerProcessId = 202; playerTimeoutSeconds = 180; ownerTimeoutSeconds = 240
    playerSha256 = $fingerprint; assemblySha256 = $fingerprint; scriptSha256 = $fingerprint; ownerSha256 = $fingerprint
}
$receipt = Join-Path $folder 'owned-process.json'
Check ((Assert-NativeReceiptLayout ([pscustomobject]$record) $receipt) -eq $folder) 'Receipt derives all artifacts from its exact own folder'
Reject { Assert-NativeReceiptLayout ([pscustomobject]$record) (Join-Path $nativeReviewRoot 'copied\owned-process.json') } 'Copied receipt cannot control the original review process'
$bad = [ordered]@{}; foreach ($entry in $record.GetEnumerator()) { $bad[$entry.Key] = $entry.Value }
$bad.scriptPath = Join-Path $nativeRepo 'human.script'
Reject { Assert-NativeReceiptLayout ([pscustomobject]$bad) $receipt } 'Receipt cannot redirect script provenance outside the isolated run'
$bad.scriptPath = $record.scriptPath; $bad.launchedBy = 'PowerAboveAllNativeReview1'
Reject { Assert-NativeReceiptLayout ([pscustomobject]$bad) $receipt } 'Old receipt without retained process owner cannot send further input'
$bad.launchedBy = $record.launchedBy; $bad.ownerTimeoutSeconds = 3600
Reject { Assert-NativeReceiptLayout ([pscustomobject]$bad) $receipt } 'Unbounded owner receipt rejected'
$bad.ownerTimeoutSeconds = 240; $bad.assemblyPath = Join-Path $folder 'other.dll'
Reject { Assert-NativeReceiptLayout ([pscustomobject]$bad) $receipt } 'Receipt cannot substitute another runtime assembly'
Check (-not (Test-NativeStartFrame (Join-Path $folder 'missing.png'))) 'Missing readiness frame is not accepted'
Reject { Invoke-FrameReview 'unused' 'unused' 'unused' -TimeoutSeconds 0 } 'Frame timeout cannot be unbounded'
$left = Get-NativeKeyDescriptor 'Left'; $right = Get-NativeKeyDescriptor 'Right'
Check ($left.VirtualKey -eq 0x25 -and $left.ScanCode -eq 0x4b -and $left.DownFlags -eq 1 -and $left.UpFlags -eq 3) 'Left is E0 navigation scan code on both press and release, not NumPad4'
Check ($right.VirtualKey -eq 0x27 -and $right.ScanCode -eq 0x4d -and $right.DownFlags -eq 1 -and $right.UpFlags -eq 3) 'Right is E0 navigation scan code on both press and release, not NumPad6'
$enter = Get-NativeKeyDescriptor 'Enter'
Check ($enter.ScanCode -eq 0x1c -and $enter.DownFlags -eq 0 -and $enter.UpFlags -eq 2) 'Enter remains main keyboard Enter rather than extended NumPad Enter'
foreach ($number in 1..4) {
    $digit = Get-NativeKeyDescriptor "Digit$number"
    Check ($digit.VirtualKey -eq (48 + $number) -and $digit.ScanCode -eq (1 + $number) -and $digit.DownFlags -eq 0 -and $digit.UpFlags -eq 2) "Digit$number maps the exact top-row physical key, not NumPad"
}
Reject { Get-NativeKeyDescriptor 'Control' } 'Unrequested modifiers remain outside the native review interface'
Write-Output "$count native input checks passed; no player or input launched."
