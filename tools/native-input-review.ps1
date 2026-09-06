# Gerçek Windows fare/tuş yolu için dar inceleme yardımcısı.
# Yalnız bu yardımcının başlattığı, output/verify altındaki player'a girdi gönderir.
param(
    [Parameter(Mandatory = $true)][ValidateSet('Start', 'Inspect', 'Click', 'ShiftClick', 'RightMouse', 'Key', 'Scroll', 'MiddleDrag', 'RightDrag')][string]$Action,
    [string]$PlayerPath,
    [string]$ScriptPath,
    [string]$ReceiptPath,
    [double]$X = 0,
    [double]$Y = 0,
    [double]$EndX = 0,
    [double]$EndY = 0,
    [ValidateRange(70,2000)][int]$HoldMilliseconds = 70,
    [ValidateSet('Enter', 'Escape', 'Right', 'Left', 'Up', 'Down', 'Space', 'Digit1', 'Digit2', 'Digit3', 'Digit4', 'W', 'A', 'S', 'D', 'Q', 'E', 'F', 'G', 'Home', 'Tab')][string]$Key = 'Enter',
    [ValidateRange(-10, 10)][int]$Wheel = 0,
    [ValidateRange(180, 300)][int]$PlayerTimeoutSeconds = 180,
    [switch]$VisiblePlayer
)
$ErrorActionPreference = 'Stop'
trap {
    [Console]::Out.WriteLine('Native input review failed: ' + $_.Exception.Message)
    [Console]::Out.WriteLine([string]$_.ScriptStackTrace)
    exit 1
}
. (Join-Path $PSScriptRoot 'native-input-owner.ps1') -PlayerPath $PlayerPath -VisiblePlayer:$VisiblePlayer -PlayerTimeoutSeconds $PlayerTimeoutSeconds
$repo = Split-Path -Parent $PSScriptRoot
$reviewRoot = [IO.Path]::GetFullPath((Join-Path $repo 'output\verify')) + '\'
Add-Type @'
using System;
using System.Runtime.InteropServices;
namespace PowerAboveAllReview {
    public static class InputWindow {
        [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
        [StructLayout(LayoutKind.Sequential)] public struct Rect { public int Left, Top, Right, Bottom; }
        [StructLayout(LayoutKind.Sequential)] public struct Point { public int X, Y; }
        [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr h, out Rect r);
        [DllImport("user32.dll")] public static extern bool ClientToScreen(IntPtr h, ref Point p);
        [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
        [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")] public static extern bool GetCursorPos(out Point p);
        [DllImport("user32.dll")] public static extern IntPtr WindowFromPoint(Point p);
        [DllImport("user32.dll")] public static extern IntPtr GetAncestor(IntPtr h, uint flags);
        [DllImport("user32.dll")] public static extern short GetAsyncKeyState(int key);
        [DllImport("user32.dll")] public static extern void mouse_event(uint flags, uint x, uint y, int data, UIntPtr extra);
        [DllImport("user32.dll")] public static extern void keybd_event(byte key, byte scan, uint flags, UIntPtr extra);
    }
}
'@
[void][PowerAboveAllReview.InputWindow]::SetProcessDPIAware()

function Assert-NativePointerTarget([IntPtr]$Handle, [int]$ExpectedX, [int]$ExpectedY) {
    $cursor = New-Object PowerAboveAllReview.InputWindow+Point
    if (-not [PowerAboveAllReview.InputWindow]::GetCursorPos([ref]$cursor) -or [Math]::Abs($cursor.X - $ExpectedX) -gt 1 -or [Math]::Abs($cursor.Y - $ExpectedY) -gt 1) { throw 'Cursor moved or was clipped; no click sent.' }
    $underPointer = [PowerAboveAllReview.InputWindow]::WindowFromPoint($cursor)
    if ($underPointer -eq [IntPtr]::Zero -or [PowerAboveAllReview.InputWindow]::GetAncestor($underPointer, 2) -ne $Handle) { throw 'Cursor is over another window; no click sent.' }
    if ([PowerAboveAllReview.InputWindow]::GetForegroundWindow() -ne $Handle) { throw 'Focus changed; no click sent.' }
}

if ($Action -eq 'Start') {
    if (-not $VisiblePlayer) { throw 'Native input inspection requires explicit -VisiblePlayer.' }
    $PlayerPath = Assert-NativeReviewPath $PlayerPath
    $ScriptPath = [IO.Path]::GetFullPath($ScriptPath)
    if (-not $PlayerPath.StartsWith($reviewRoot, [StringComparison]::OrdinalIgnoreCase)) { throw 'Only a review build in output/verify can be launched.' }
    [void](Assert-ReviewProtocol $PlayerPath)
    if (-not (Test-Path -LiteralPath $ScriptPath -PathType Leaf)) { throw 'Review script is missing.' }
    $out = Join-Path $reviewRoot ('native-input-' + [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8))
    [void][IO.Directory]::CreateDirectory($out)
    $scriptCopy = Join-Path $out 'review.script'
    [IO.File]::Copy($ScriptPath, $scriptCopy, $false)
    $plan = Get-ReviewPlan $scriptCopy
    if ($plan.Captures[0] -ne '00-start.png') { throw 'Native review needs 00-start as its first capture.' }
    $nativeArgs = ((Get-NativeOwnerArguments $PlayerPath $out $PlayerTimeoutSeconds) | ForEach-Object { ConvertTo-NativeArgument $_ }) -join ' '
    # Tek gizli sahip, orijinal handle'i tutar; oyuncu butcesi +60 saniyede raporlar.
    # Ebeveyn cikinca kapanacak yonlendirilmis boru acma; sahip kendi raporunu yazar.
    $owner = Start-Process -FilePath (Join-Path $PSHOME 'powershell.exe') -ArgumentList $nativeArgs -WorkingDirectory $repo -WindowStyle Hidden -PassThru
    $ReceiptPath = Join-Path $out 'owned-process.json'
    $readyUntil = [DateTime]::UtcNow.AddSeconds(12)
    $ready = $false
    try {
        $ownerHandle = $owner.Handle
        do {
            Start-Sleep -Milliseconds 150
            $owner.Refresh()
            if ($owner.HasExited) { throw "Native owner exited before readiness. See $out\owner.log and result.json." }
            if ((Test-Path -LiteralPath $ReceiptPath -PathType Leaf) -and (Test-NativeStartFrame (Join-Path $out 'shots\00-start.png'))) {
                $probe = Get-NativeOwnedPlayer $ReceiptPath
                try { $ready = $probe.MainWindowHandle -ne [IntPtr]::Zero } finally { $probe.Dispose() }
            }
        } while (-not $ready -and [DateTime]::UtcNow -lt $readyUntil)
        if (-not $ready -or [DateTime]::UtcNow -ge $readyUntil) { throw "Native review was not ready within 12 seconds; no input sent. The bounded owner will finish independently. Artifacts: $out" }
    } finally { $owner.Dispose() }
    Write-Output "Receipt: $ReceiptPath"
}

$owned = Get-NativeOwnedPlayer $ReceiptPath
try {
$handle = $owned.MainWindowHandle
if ($handle -eq [IntPtr]::Zero) { throw 'Owned player has no window.' }
[void][PowerAboveAllReview.InputWindow]::SetForegroundWindow($handle)
Start-Sleep -Milliseconds 100
if ([PowerAboveAllReview.InputWindow]::GetForegroundWindow() -ne $handle) { throw 'Owned player is not foreground; no input sent.' }
$bounds = New-Object PowerAboveAllReview.InputWindow+Rect
$origin = New-Object PowerAboveAllReview.InputWindow+Point
if (-not [PowerAboveAllReview.InputWindow]::GetClientRect($handle, [ref]$bounds) -or -not [PowerAboveAllReview.InputWindow]::ClientToScreen($handle, [ref]$origin)) { throw 'Cannot read client bounds.' }
$width = $bounds.Right - $bounds.Left
$height = $bounds.Bottom - $bounds.Top
if ($width -lt 640 -or $height -lt 400) { throw 'Unexpected or minimized player client.' }

if ($Action -in @('Click', 'ShiftClick', 'RightMouse', 'Scroll', 'MiddleDrag', 'RightDrag')) {
    if ([double]::IsNaN($X) -or [double]::IsInfinity($X) -or [double]::IsNaN($Y) -or [double]::IsInfinity($Y) -or $X -lt 0 -or $X -ge 1440 -or $Y -lt 0 -or $Y -ge 900) { throw 'Coordinates must be finite points inside the 1440x900 design canvas.' }
    # ViewLayout yatay/dikey boşluk eklediği için en-boy oranını koru.
    $scale = [Math]::Min($width / 1440.0, $height / 900.0)
    $screenX = $origin.X + ($width - 1440 * $scale) / 2 + $X * $scale
    $screenY = $origin.Y + ($height - 900 * $scale) / 2 + $Y * $scale
    if (-not [PowerAboveAllReview.InputWindow]::SetCursorPos([int]$screenX, [int]$screenY)) { throw 'Cursor positioning failed; no input sent.' }
    Assert-NativePointerTarget $handle ([int]$screenX) ([int]$screenY)
    if ($Action -in @('MiddleDrag', 'RightDrag')) {
        if ([double]::IsNaN($EndX) -or [double]::IsInfinity($EndX) -or [double]::IsNaN($EndY) -or [double]::IsInfinity($EndY) -or $EndX -lt 0 -or $EndX -ge 1440 -or $EndY -lt 0 -or $EndY -ge 900) { throw 'Drag endpoint must remain in the design canvas.' }
        $endScreenX = $origin.X + ($width - 1440 * $scale) / 2 + $EndX * $scale
        $endScreenY = $origin.Y + ($height - 900 * $scale) / 2 + $EndY * $scale
        $mouseDown = 32; $mouseUp = 64
        if ($Action -eq 'RightDrag') { $mouseDown = 8; $mouseUp = 16 }
        try {
            [PowerAboveAllReview.InputWindow]::mouse_event($mouseDown, 0, 0, 0, [UIntPtr]::Zero)
            for ($step = 1; $step -le 20; $step++) {
                if ([PowerAboveAllReview.InputWindow]::GetForegroundWindow() -ne $handle) { throw 'Focus changed during drag; releasing button.' }
                $nextX = [int]($screenX + ($endScreenX - $screenX) * $step / 20)
                $nextY = [int]($screenY + ($endScreenY - $screenY) * $step / 20)
                [void][PowerAboveAllReview.InputWindow]::SetCursorPos($nextX, $nextY)
                Assert-NativePointerTarget $handle $nextX $nextY
                Start-Sleep -Milliseconds 20
            }
        } finally { [PowerAboveAllReview.InputWindow]::mouse_event($mouseUp, 0, 0, 0, [UIntPtr]::Zero) }
    } elseif ($Action -eq 'ShiftClick') {
        $shiftInput = Get-NativeShiftClickDescriptor (([PowerAboveAllReview.InputWindow]::GetAsyncKeyState(0xa0) -band 0x8000) -ne 0) (([PowerAboveAllReview.InputWindow]::GetAsyncKeyState(0xa1) -band 0x8000) -ne 0)
        Invoke-NativeShiftClickSequence -Send {
            param([string]$Step)
            switch ($Step) {
                'shift-down' { [PowerAboveAllReview.InputWindow]::keybd_event($shiftInput.VirtualKey, $shiftInput.ScanCode, $shiftInput.DownFlags, [UIntPtr]::Zero) }
                'mouse-down' { [PowerAboveAllReview.InputWindow]::mouse_event(2, 0, 0, 0, [UIntPtr]::Zero) }
                'mouse-up' { [PowerAboveAllReview.InputWindow]::mouse_event(4, 0, 0, 0, [UIntPtr]::Zero) }
                'shift-up' { [PowerAboveAllReview.InputWindow]::keybd_event($shiftInput.VirtualKey, $shiftInput.ScanCode, $shiftInput.UpFlags, [UIntPtr]::Zero) }
            }
        } -CheckTarget { Assert-NativePointerTarget $handle ([int]$screenX) ([int]$screenY) } -Delay { Start-Sleep -Milliseconds 70 }
    } elseif ($Action -in @('Click', 'RightMouse')) {
        $mouseDown = 2; $mouseUp = 4
        if ($Action -eq 'RightMouse') { $mouseDown = 8; $mouseUp = 16 }
        try {
            [PowerAboveAllReview.InputWindow]::mouse_event($mouseDown, 0, 0, 0, [UIntPtr]::Zero)
            Start-Sleep -Milliseconds 70
        } finally { [PowerAboveAllReview.InputWindow]::mouse_event($mouseUp, 0, 0, 0, [UIntPtr]::Zero) }
    } else { [PowerAboveAllReview.InputWindow]::mouse_event(0x800, 0, 0, ($Wheel * 120), [UIntPtr]::Zero) }
} elseif ($Action -eq 'Key') {
    $keyInput = Get-NativeKeyDescriptor $Key
    if ([PowerAboveAllReview.InputWindow]::GetForegroundWindow() -ne $handle) { throw 'Focus changed before key press; no input sent.' }
    if (([PowerAboveAllReview.InputWindow]::GetAsyncKeyState($keyInput.VirtualKey) -band 0x8000) -ne 0) { throw 'Key is already held; no input sent.' }
    try {
        [PowerAboveAllReview.InputWindow]::keybd_event($keyInput.VirtualKey, $keyInput.ScanCode, $keyInput.DownFlags, [UIntPtr]::Zero)
        $remaining = $HoldMilliseconds
        while ($remaining -gt 0) {
            if ([PowerAboveAllReview.InputWindow]::GetForegroundWindow() -ne $handle) { throw 'Focus changed during key hold; releasing key.' }
            $pause = [Math]::Min(50, $remaining); Start-Sleep -Milliseconds $pause; $remaining -= $pause
        }
    } finally { [PowerAboveAllReview.InputWindow]::keybd_event($keyInput.VirtualKey, $keyInput.ScanCode, $keyInput.UpFlags, [UIntPtr]::Zero) }
}
Start-Sleep -Milliseconds 300
Write-Output ("Owned PID {0}; client {1},{2},{3},{4}; action {5}" -f $owned.Id, $origin.X, $origin.Y, $width, $height, $Action)
$captureHelper = Join-Path ([Environment]::GetFolderPath('UserProfile')) '.codex\skills\screenshot\scripts\take_screenshot.ps1'
& $captureHelper -Mode temp -Region ("{0},{1},{2},{3}" -f $origin.X, $origin.Y, $width, $height)
} finally { $owned.Dispose() }
