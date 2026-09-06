# Gorunur inceleme oyuncusunun tek sahibi. Dot-source yalniz denetim islevlerini yukler.
param([switch]$Run, [string]$PlayerPath, [string]$OutputDirectory, [switch]$VisiblePlayer,
    [ValidateRange(180, 300)][int]$PlayerTimeoutSeconds = 180)
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'verify-support.ps1')
$nativeRepo = Split-Path -Parent $PSScriptRoot
$nativeReviewRoot = [IO.Path]::GetFullPath((Join-Path $nativeRepo 'output\verify')).TrimEnd('\') + '\'
$nativeOwnerScript = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot 'native-input-owner.ps1'))

if (-not ('PowerAboveAllReview.NativeArguments' -as [type])) {
    Add-Type @'
using System;
using System.Runtime.InteropServices;
namespace PowerAboveAllReview {
    public static class NativeArguments {
        [DllImport("user32.dll", ExactSpelling=true)] public static extern uint MapVirtualKeyW(uint code, uint mapType);
        [DllImport("shell32.dll", SetLastError=true, CharSet=CharSet.Unicode)]
        static extern IntPtr CommandLineToArgvW(string command, out int count);
        [DllImport("kernel32.dll")] static extern IntPtr LocalFree(IntPtr memory);
        public static string[] Split(string command) {
            int count;
            IntPtr block = CommandLineToArgvW(command, out count);
            if (block == IntPtr.Zero) throw new InvalidOperationException("Cannot parse process command line.");
            try {
                var values = new string[count];
                for (int i=0; i<count; i++) values[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(block, i * IntPtr.Size));
                return values;
            } finally { LocalFree(block); }
        }
    }
}
'@
}

function Get-NativeKeyDescriptor([ValidateSet('Enter', 'Escape', 'Right', 'Left', 'Space', 'Digit1', 'Digit2', 'Digit3', 'Digit4')][string]$Key) {
    $virtualKey = @{ Enter = 13; Escape = 27; Right = 39; Left = 37; Space = 32; Digit1 = 49; Digit2 = 50; Digit3 = 51; Digit4 = 52 }[$Key]
    $mapped = [PowerAboveAllReview.NativeArguments]::MapVirtualKeyW($virtualKey, 4)
    if ($mapped -eq 0) { throw "No hardware scan code mapping for $Key; no input sent." }
    $prefix = $mapped -band 0xff00
    if ($prefix -ne 0 -and $prefix -ne 0xe000) { throw "Unsupported keyboard scan prefix for $Key; no input sent." }
    # Microsoft: ok tuslari E0 ister; aksi halde NumPad tusu olarak yorumlanabilir.
    # https://github.com/microsoft/PowerToys/blob/main/doc/devdocs/modules/keyboardmanager/keyboardmanager.md
    $downFlags = 0
    if ($prefix -eq 0xe000 -or $Key -in @('Left', 'Right')) { $downFlags = 1 }
    return [pscustomobject]@{ VirtualKey = [byte]$virtualKey; ScanCode = [byte]($mapped -band 0xff); DownFlags = [uint32]$downFlags; UpFlags = [uint32]($downFlags -bor 2) }
}

function Assert-NativeReviewPath([string]$Path) {
    if ([string]::IsNullOrWhiteSpace($Path)) { throw 'Review provenance path is missing.' }
    $full = [IO.Path]::GetFullPath($Path)
    if (-not $full.StartsWith($nativeReviewRoot, [StringComparison]::OrdinalIgnoreCase)) { throw 'Native review paths must remain inside output/verify.' }
    # Junction/symlink ile izinli klasor disina cikilmasina izin verme.
    $cursor = $full
    while ($cursor) {
        if (Test-Path -LiteralPath $cursor) {
            if ((Get-Item -LiteralPath $cursor -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) { throw "Review path contains a reparse point: $cursor" }
        }
        $cursor = Split-Path -Parent $cursor
    }
    return $full
}

function Get-NativePlayerArguments([string]$Folder) {
    return @('-shots', (Join-Path $Folder 'shots'), '-script', (Join-Path $Folder 'review.script'), '-logFile', (Join-Path $Folder 'player.log'), '-screen-width', '1440', '-screen-height', '900', '-screen-fullscreen', '0')
}

function Get-NativeOwnerArguments([string]$Player, [string]$Folder, [ValidateRange(180, 300)][int]$PlayerTimeoutSeconds = 180) {
    return @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $nativeOwnerScript, '-Run', '-PlayerPath', $Player, '-OutputDirectory', $Folder, '-VisiblePlayer', '-PlayerTimeoutSeconds', [string]$PlayerTimeoutSeconds)
}

function Assert-NativeCommandLine([string]$CommandLine, [string[]]$Expected) {
    if ([string]::IsNullOrWhiteSpace($CommandLine)) { throw 'Live process command line is unavailable.' }
    $actual = [PowerAboveAllReview.NativeArguments]::Split($CommandLine)
    if ($actual.Count -ne $Expected.Count) { throw 'Live process argument count differs from isolated launch.' }
    for ($index = 0; $index -lt $Expected.Count; $index++) {
        if (-not [string]::Equals($actual[$index], $Expected[$index], [StringComparison]::OrdinalIgnoreCase)) { throw "Live process argument $index differs from isolated launch." }
    }
}

function Assert-NativeReceiptLayout($Record, [string]$Receipt) {
    $receiptFull = Assert-NativeReviewPath $Receipt
    $folder = Assert-NativeReviewPath $Record.outputPath
    $player = Assert-NativeReviewPath $Record.playerPath
    if ($Record.launchedBy -ne 'PowerAboveAllNativeReview2' -or $receiptFull -ne (Join-Path $folder 'owned-process.json')) { throw 'Receipt is not the original owned native review record.' }
    if ($Record.scriptPath -ne (Join-Path $folder 'review.script') -or $Record.ownerScript -ne $nativeOwnerScript -or $Record.ownerPath -ne (Join-Path $PSHOME 'powershell.exe')) { throw 'Receipt script or owner provenance differs from this tool.' }
    $playerSeconds = $Record.playerTimeoutSeconds
    $ownerSeconds = $Record.ownerTimeoutSeconds
    if (($playerSeconds -isnot [int] -and $playerSeconds -isnot [long]) -or ($ownerSeconds -isnot [int] -and $ownerSeconds -isnot [long]) -or
        $playerSeconds -lt 180 -or $playerSeconds -gt 300 -or $ownerSeconds -ne ($playerSeconds + 60) -or
        $Record.processId -le 0 -or $Record.ownerProcessId -le 0 -or $Record.processId -eq $Record.ownerProcessId) { throw 'Receipt owner identity or lifetime is invalid.' }
    $data = Join-Path (Split-Path -Parent $player) (([IO.Path]::GetFileNameWithoutExtension($player)) + '_Data')
    if ($Record.assemblyPath -ne (Join-Path $data 'Managed\PowerAboveAll.Runtime.dll')) { throw 'Receipt runtime assembly path differs from the launched player.' }
    foreach ($path in @($Record.scriptPath, $Record.assemblyPath)) { $null = Assert-NativeReviewPath $path }
    foreach ($hash in @($Record.playerSha256, $Record.assemblySha256, $Record.scriptSha256, $Record.ownerSha256)) {
        if ($hash -notmatch '\A[0-9a-fA-F]{64}\z') { throw 'Receipt fingerprint is missing or malformed.' }
    }
    return $folder
}

function Assert-NativeLiveProcess([int]$ProcessId, [string]$StartUtc, [string]$Executable, [string[]]$Arguments, [int]$ExpectedParent = 0) {
    $live = Get-Process -Id $ProcessId -ErrorAction Stop
    try {
        $null = $live.Handle
        if ($live.HasExited -or $live.StartTime.ToUniversalTime().ToString('O') -ne $StartUtc -or $live.Path -ne $Executable) { throw 'Live process identity differs from the launch receipt.' }
        $cim = Get-CimInstance Win32_Process -Filter "ProcessId=$ProcessId" -ErrorAction Stop
        if ($null -eq $cim -or $cim.ExecutablePath -ne $Executable) { throw 'Live executable provenance is unavailable.' }
        if ($ExpectedParent -gt 0 -and $cim.ParentProcessId -ne $ExpectedParent) { throw 'Player was not launched by the receipt owner.' }
        Assert-NativeCommandLine $cim.CommandLine (@($Executable) + $Arguments)
        $live.Refresh()
        if ($live.HasExited) { throw 'Owned process exited during identity check.' }
        return $live
    } catch { $live.Dispose(); throw }
}

function Get-NativeOwnedPlayer([string]$Receipt) {
    $receiptFull = Assert-NativeReviewPath $Receipt
    $record = [IO.File]::ReadAllText($receiptFull) | ConvertFrom-Json
    $folder = Assert-NativeReceiptLayout $record $receiptFull
    if ((Test-Path -LiteralPath (Join-Path $folder 'native-exit.json')) -or (Test-Path -LiteralPath (Join-Path $folder 'result.json'))) { throw 'Native review has ended; no further input is allowed.' }
    if ([DateTime]::UtcNow -gt ([DateTime]::Parse($record.ownerStartUtc).ToUniversalTime().AddSeconds($record.ownerTimeoutSeconds))) { throw 'Native review owner deadline has elapsed.' }
    foreach ($pair in @(@($record.playerPath, $record.playerSha256), @($record.assemblyPath, $record.assemblySha256), @($record.scriptPath, $record.scriptSha256), @($record.ownerScript, $record.ownerSha256))) {
        if ((Get-FileHash -LiteralPath $pair[0] -Algorithm SHA256).Hash -ne $pair[1]) { throw "Review provenance file changed: $($pair[0])" }
    }
    $owner = Assert-NativeLiveProcess $record.ownerProcessId $record.ownerStartUtc $record.ownerPath (Get-NativeOwnerArguments $record.playerPath $folder $record.playerTimeoutSeconds)
    try { return Assert-NativeLiveProcess $record.processId $record.startUtc $record.playerPath (Get-NativePlayerArguments $folder) -ExpectedParent $record.ownerProcessId }
    finally { $owner.Dispose() }
}

function Write-NativeEvidence([string]$Path, $Value) {
    $null = Assert-NativeReviewPath $Path
    $temporary = $Path + '.' + [Guid]::NewGuid().ToString('N') + '.pending'
    [IO.File]::WriteAllText($temporary, ($Value | ConvertTo-Json -Depth 7), [Text.Encoding]::UTF8)
    # Aynı klasorde atomik yayin; eski kanit asla uzerine yazilmaz.
    [IO.File]::Move($temporary, $Path)
}

function Test-NativeStartFrame([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { return $false }
    try {
        $bytes = [IO.File]::ReadAllBytes($Path)
        return $bytes.Length -gt 24 -and [BitConverter]::ToString($bytes, 0, 8) -eq '89-50-4E-47-0D-0A-1A-0A' -and [BitConverter]::ToString($bytes, 12, 4) -eq '49-48-44-52' -and [BitConverter]::ToString($bytes, 16, 8) -eq '00-00-05-A0-00-00-03-84'
    } catch { return $false }
}

if (-not $Run) { return }
if (-not $VisiblePlayer) { throw 'Owner launch requires explicit -Run -VisiblePlayer.' }
$OutputDirectory = Assert-NativeReviewPath $OutputDirectory
$PlayerPath = Assert-NativeReviewPath $PlayerPath
if (-not (Test-Path -LiteralPath $OutputDirectory -PathType Container)) { throw 'Parent must create a fresh review directory first.' }
foreach ($name in @('owned-process.json', 'native-exit.json', 'result.json', 'shots', 'player.log')) {
    if (Test-Path -LiteralPath (Join-Path $OutputDirectory $name)) { throw "Refusing to reuse native review evidence: $name" }
}
$started = [DateTime]::UtcNow
$ownerTimeoutSeconds = $PlayerTimeoutSeconds + 60
$budget = [Diagnostics.Stopwatch]::StartNew()
$owned = $null; $nativeExit = $null; $timedOut = $false; $record = $null
$failures = New-Object 'Collections.Generic.List[string]'
$gates = [ordered]@{ Preflight = 'NOT RUN'; EditMode = 'SKIPPED'; Build = 'SKIPPED: existing player; current source not verified'; Player = 'NOT RUN'; Frames = 'NOT RUN'; Browser = 'SKIPPED' }
$activeGate = 'Preflight'
try {
    $scriptCopy = Join-Path $OutputDirectory 'review.script'
    $plan = Get-ReviewPlan $scriptCopy
    if ($plan.Captures[0] -ne '00-start.png') { throw 'Native review needs 00-start as its first capture.' }
    $assembly = Assert-ReviewProtocol $PlayerPath
    $self = Get-Process -Id $PID
    try {
        $record = [ordered]@{ launchedBy = 'PowerAboveAllNativeReview2'; outputPath = $OutputDirectory; playerPath = $PlayerPath; scriptPath = $scriptCopy; assemblyPath = $assembly; playerSha256 = (Get-FileHash -LiteralPath $PlayerPath -Algorithm SHA256).Hash; assemblySha256 = (Get-FileHash -LiteralPath $assembly -Algorithm SHA256).Hash; scriptSha256 = (Get-FileHash -LiteralPath $scriptCopy -Algorithm SHA256).Hash; ownerSha256 = (Get-FileHash -LiteralPath $nativeOwnerScript -Algorithm SHA256).Hash; ownerScript = $nativeOwnerScript; ownerPath = $self.Path; ownerProcessId = $PID; ownerStartUtc = $self.StartTime.ToUniversalTime().ToString('O'); playerTimeoutSeconds = $PlayerTimeoutSeconds; ownerTimeoutSeconds = $ownerTimeoutSeconds }
    } finally { $self.Dispose() }
    $gates.Preflight = 'PASSED: explicit visible player; isolated protocol 2; script/runtime/executable fingerprints'
    $activeGate = 'Player'
    $argsText = ((Get-NativePlayerArguments $OutputDirectory) | ForEach-Object { ConvertTo-NativeArgument $_ }) -join ' '
    $owned = Start-Process -FilePath $PlayerPath -ArgumentList $argsText -WorkingDirectory $nativeRepo -WindowStyle Normal -PassThru
    # PID tekrar kullanimina karsi orijinal handle tum yasam boyunca saklanir.
    $ownedHandle = $owned.Handle
    $record.processId = $owned.Id
    $record.startUtc = $owned.StartTime.ToUniversalTime().ToString('O')
    Write-NativeEvidence (Join-Path $OutputDirectory 'owned-process.json') $record
    if (-not $owned.WaitForExit($PlayerTimeoutSeconds * 1000)) {
        $timedOut = $true
        $owned.Kill()
        if (-not $owned.WaitForExit(5000)) { throw 'Owned player did not stop after its bounded timeout.' }
    }
    $owned.Refresh()
    if ($null -eq $owned.ExitCode) { throw 'Owned player native exit code is unavailable.' }
    $nativeExit = [int]$owned.ExitCode
    Write-NativeEvidence (Join-Path $OutputDirectory 'native-exit.json') ([ordered]@{ processId = $owned.Id; startUtc = $record.startUtc; exitCode = $nativeExit; timedOut = $timedOut; completedUtc = [DateTime]::UtcNow.ToString('O') })
    if ($timedOut) { throw "Owned player exceeded $PlayerTimeoutSeconds seconds; only its original handle was stopped." }
    $null = Assert-CleanLog (Join-Path $OutputDirectory 'player.log')
    $summary = Assert-ReviewResult (Join-Path $OutputDirectory 'shots') $plan $nativeExit
    $gates.Player = "PASSED: $summary; native exit $nativeExit"
    $activeGate = 'Frames'
    $remaining = [Math]::Min(60, [Math]::Floor($ownerTimeoutSeconds - 5 - $budget.Elapsed.TotalSeconds))
    if ($remaining -lt 1) { throw 'Owner review budget exhausted before frame validation.' }
    $frameSummary = Invoke-FrameReview (Join-Path $PSScriptRoot 'shot-check.py') (Join-Path $OutputDirectory 'shots') $OutputDirectory -TimeoutSeconds ([int]$remaining)
    $gates.Frames = "PASSED: $frameSummary"
    foreach ($pair in @(@($record.playerPath, $record.playerSha256), @($record.assemblyPath, $record.assemblySha256), @($record.scriptPath, $record.scriptSha256))) {
        if ((Get-FileHash -LiteralPath $pair[0] -Algorithm SHA256).Hash -ne $pair[1]) { throw 'Review executable, runtime or script changed before completion.' }
    }
} catch {
    $gates[$activeGate] = 'FAILED'
    $failures.Add($_.Exception.Message)
    Write-Output "Native review failed: $($_.Exception.Message)"
} finally {
    if ($null -ne $owned) {
        try {
            if (-not $owned.HasExited) { $owned.Kill(); $null = $owned.WaitForExit(5000) }
            $owned.Refresh()
            if ($owned.HasExited -and $null -ne $owned.ExitCode) { $nativeExit = [int]$owned.ExitCode }
            if (-not (Test-Path -LiteralPath (Join-Path $OutputDirectory 'native-exit.json'))) {
                Write-NativeEvidence (Join-Path $OutputDirectory 'native-exit.json') ([ordered]@{ processId = $owned.Id; startUtc = $record.startUtc; exitCode = $nativeExit; timedOut = $timedOut; completedUtc = [DateTime]::UtcNow.ToString('O') })
            }
        } catch { $failures.Add("Owner cleanup: $($_.Exception.Message)") }
        finally { $owned.Dispose() }
    }
    $verdict = 'RED'
    if ($failures.Count -eq 0 -and $gates.Player.StartsWith('PASSED') -and $gates.Frames.StartsWith('PASSED')) { $verdict = 'PARTIAL' }
    $result = [ordered]@{ verdict = $verdict; reusedBuild = $true; currentSourceVerified = $false; gates = $gates; failures = $failures.ToArray(); playerPath = $PlayerPath; playerWindow = 'Visible (explicit authorization)'; playerExitCode = $nativeExit; playerTimeoutSeconds = $PlayerTimeoutSeconds; ownerTimeoutSeconds = $ownerTimeoutSeconds; elapsedSeconds = [Math]::Round($budget.Elapsed.TotalSeconds, 2); artifacts = $OutputDirectory }
    Write-NativeEvidence (Join-Path $OutputDirectory 'result.json') $result
    $report = @('# Yerel girdi incelemesi', '', "Sonuc: $verdict; native cikis: $nativeExit", "Sure siniri: player $PlayerTimeoutSeconds saniye; owner $ownerTimeoutSeconds saniye.", '', 'Yalniz mevcut oyuncu ve gercek fare/tus yolu incelendi. Yeni kaynak derlemesi, Unity testleri ve tarayici atlandi. Bu rapor GREEN olamaz.', '', '| Kontrol | Sonuc |', '| --- | --- |')
    foreach ($gate in $gates.GetEnumerator()) { $report += "| $($gate.Key) | $($gate.Value) |" }
    foreach ($failure in $failures) { $report += "`nHata: $failure" }
    [IO.File]::WriteAllLines((Join-Path $OutputDirectory 'REPORT.md'), $report, [Text.Encoding]::UTF8)
    [IO.File]::WriteAllLines((Join-Path $OutputDirectory 'owner.log'), @("Started UTC: $($started.ToString('O'))", "Native exit: $nativeExit", "Verdict: $verdict") + $failures.ToArray(), [Text.Encoding]::UTF8)
    Write-Output "Native review: $verdict; artifacts: $OutputDirectory"
}
if ($failures.Count -gt 0) { exit 1 }
exit 0
