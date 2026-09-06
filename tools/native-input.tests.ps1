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
$defaultOwnerArguments = @(Get-NativeOwnerArguments $player $folder)
Check ($defaultOwnerArguments[-2] -eq '-PlayerTimeoutSeconds' -and $defaultOwnerArguments[-1] -eq '180') 'Existing native reviews keep their default 180-second player deadline'
$extended = [ordered]@{}; foreach ($entry in $record.GetEnumerator()) { $extended[$entry.Key] = $entry.Value }
$extended.playerTimeoutSeconds = 240; $extended.ownerTimeoutSeconds = 300
Check ((Assert-NativeReceiptLayout ([pscustomobject]$extended) $receipt) -eq $folder) 'Natural battle review has a bounded 240-second player and 60-second owner allowance'
$extendedOwnerArguments = @((Join-Path $PSHOME 'powershell.exe')) + @(Get-NativeOwnerArguments $player $folder 240)
$extendedCommand = ($extendedOwnerArguments | ForEach-Object { ConvertTo-NativeArgument $_ }) -join ' '
Assert-NativeCommandLine $extendedCommand $extendedOwnerArguments
Check $true 'Explicit longer deadline is part of the exact owned command line'
Reject { Assert-NativeCommandLine $extendedCommand (@((Join-Path $PSHOME 'powershell.exe')) + $defaultOwnerArguments) } 'Receipt cannot claim a different lifetime than the live owner launch'
$extended.playerTimeoutSeconds = 300; $extended.ownerTimeoutSeconds = 360
Check ((Assert-NativeReceiptLayout ([pscustomobject]$extended) $receipt) -eq $folder) 'Maximum supported review lifetime remains bounded at 300 plus 60 seconds'
$extended.playerTimeoutSeconds = 240
$extended.ownerTimeoutSeconds = 301
Reject { Assert-NativeReceiptLayout ([pscustomobject]$extended) $receipt } 'Owner allowance cannot grow independently of the player deadline'
$extended.playerTimeoutSeconds = 179; $extended.ownerTimeoutSeconds = 239
Reject { Assert-NativeReceiptLayout ([pscustomobject]$extended) $receipt } 'Receipt below the supported player deadline range is rejected'
$extended.playerTimeoutSeconds = 301; $extended.ownerTimeoutSeconds = 361
Reject { Assert-NativeReceiptLayout ([pscustomobject]$extended) $receipt } 'Receipt above the maximum player deadline is rejected'
$extended.playerTimeoutSeconds = 240.5; $extended.ownerTimeoutSeconds = 300.5
Reject { Assert-NativeReceiptLayout ([pscustomobject]$extended) $receipt } 'Fractional receipt deadlines are not silently rounded'
Reject { Get-NativeOwnerArguments $player $folder 301 } 'Invalid launch timeout is rejected before any process can start'
$nativeVictoryPlan = Get-ReviewPlan (Join-Path $PSScriptRoot 'native-victory.script')
Check ($nativeVictoryPlan.Captures[0] -eq '00-start.png' -and $nativeVictoryPlan.Captures.Count -eq 9 -and $nativeVictoryPlan.States.Count -eq 8 -and $nativeVictoryPlan.Assertions -ge 18) 'Native victory plan preserves readiness and combat/decision evidence in its receipt'
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
$shift = Get-NativeShiftClickDescriptor $false $false
Check ($shift.VirtualKey -eq 0xa0 -and $shift.ScanCode -eq 0x2a -and $shift.DownFlags -eq 0 -and $shift.UpFlags -eq 2) 'Shift-click uses physical left Shift with a matching nonextended release'
Reject { Get-NativeShiftClickDescriptor $true $false } 'Already-held left Shift is preserved without generating input'
Reject { Get-NativeShiftClickDescriptor $false $true } 'Already-held right Shift cannot masquerade as the tested modifier'
$events = New-Object 'Collections.Generic.List[string]'
Invoke-NativeShiftClickSequence -Send { param($step) $events.Add($step) } -CheckTarget { $events.Add('target-check') } -Delay { $events.Add('delay') }
Check (($events -join ',') -eq 'shift-down,delay,target-check,mouse-down,delay,mouse-up,delay,shift-up') 'Shift remains held through mouse-up and an input frame before release'
$events.Clear()
Reject { Invoke-NativeShiftClickSequence -Send { param($step) $events.Add($step) } -CheckTarget { throw 'Simulated focus loss after modifier down' } -Delay {} } 'Focus failure after modifier press remains an error'
Check (($events -join ',') -eq 'shift-down,shift-up') 'Focus failure releases owned Shift without pressing the mouse'
$events.Clear()
Reject { Invoke-NativeShiftClickSequence -Send { param($step) $events.Add($step); if ($step -eq 'mouse-down') { throw 'Simulated input failure' } } -CheckTarget {} -Delay {} } 'Failure after mouse press remains an error'
Check (($events -join ',') -eq 'shift-down,mouse-down,mouse-up,shift-up') 'Failure after mouse press releases both owned inputs'
$events.Clear()
Reject { Invoke-NativeShiftClickSequence -Send { param($step) $events.Add($step); if ($step -eq 'mouse-up') { throw 'Simulated mouse cleanup failure' } } -CheckTarget {} -Delay {} } 'Mouse release failure is not hidden'
Check ($events[-1] -eq 'shift-up') 'Nested finally still releases Shift if mouse cleanup fails'
$nativeVolleyPlan = Get-ReviewPlan (Join-Path $PSScriptRoot 'native-volley.script')
Check ($nativeVolleyPlan.Captures[0] -eq '00-start.png' -and $nativeVolleyPlan.Captures.Count -eq 6 -and $nativeVolleyPlan.States.Count -eq 6) 'Native group and volley receipt includes both observed phases and paused comparison'
Write-Output "$count native input checks passed; no player or input launched."
