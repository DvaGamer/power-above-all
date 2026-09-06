# Unity veya oyuncu baslatmadan guvenlik ve sonuc ayrisma kontrolleri (ASCII).
param([string]$PlayerPath = '')
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'verify-support.ps1')
$testCount = 0
function Check([bool]$Condition, [string]$Name) {
  if (-not $Condition) { throw "FAIL: $Name" }
  $script:testCount++; Write-Output "PASS: $Name"
}
function Reject([scriptblock]$Action, [string]$Name) {
  $rejected = $false
  try { $null = & $Action } catch { $rejected = $true }
  Check $rejected $Name
}
foreach ($file in @('verify.ps1', 'verify-support.ps1', 'verify-support.tests.ps1', 'review-player.ps1', 'player-crash-evidence.ps1', 'frame-check.tests.ps1', 'frame-invocation-probe.ps1')) {
  $parseTokens = $null; $parseErrors = $null
  $null = [Management.Automation.Language.Parser]::ParseFile((Join-Path $PSScriptRoot $file), [ref]$parseTokens, [ref]$parseErrors)
  Check ($parseErrors.Count -eq 0) "PowerShell parser: $file $parseErrors"
}
Check ((ConvertTo-NativeArgument 'C:\a b\') -eq '"C:\a b\\"') 'Trailing backslash is escaped inside quoted Windows argument'
Check ((ConvertTo-NativeArgument 'a"b') -eq '"a\"b"') 'Embedded quote is escaped'
Reject { ConvertTo-NativeArgument "bad`nargument" } 'Newline argument rejected'
Check (@(Get-ReviewGraphicsArguments 'Default').Count -eq 0) 'Default graphics adds no override'
Check ((Get-ReviewGraphicsArguments 'Direct3D11') -eq '-force-d3d11') 'Direct3D11 uses the explicit native override'
Reject { Get-ReviewGraphicsArguments '-batchmode' } 'Unrecognized graphics override is rejected'
Assert-ReviewGraphics "Direct3D:`n    Version: Direct3D 11.0 [level 11.1]" 'Direct3D11'
Check $true 'Requested primary graphics version is accepted'
Reject { Assert-ReviewGraphics "Direct3D:`n    Version: Direct3D 12.0`nD3D11 device created for Microsoft Media Foundation video decoding." 'Direct3D11' } 'Secondary video decoding device cannot prove requested primary renderer'
Check ((Invoke-OwnedProcess (Join-Path $PSHOME 'powershell.exe') @('-NoProfile', '-Command', 'exit 7') 10 $PSScriptRoot) -eq 7) 'Owned helper process preserves a nonzero exit code'
$repo = Split-Path -Parent $PSScriptRoot
$fixtureDir = Join-Path $repo ('output\verify-support-tests-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $fixtureDir | Out-Null
$childStdout = Join-Path $fixtureDir 'child-output.log'
$childStderr = Join-Path $fixtureDir 'child-error.log'
$childExit = Invoke-OwnedProcess (Join-Path $PSHOME 'powershell.exe') @('-NoProfile', '-Command', '[Console]::Out.WriteLine("sample output"); [Console]::Error.WriteLine("sample error"); exit 9') 10 $PSScriptRoot -StdoutPath $childStdout -StderrPath $childStderr
Check ($childExit -eq 9 -and [IO.File]::ReadAllText($childStdout).Contains('sample output') -and [IO.File]::ReadAllText($childStderr).Contains('sample error')) "Owned subprocess preserves stdout, stderr and nonzero exit without parent native pipeline (exit=$childExit)"
Reject { Invoke-OwnedProcess (Join-Path $PSHOME 'powershell.exe') @('-NoProfile', '-Command', 'exit 0') 10 $PSScriptRoot -StdoutPath $childStdout } 'Existing process evidence is not overwritten'
function Fixture([string]$Name, [string]$Text) {
  $path = Join-Path $fixtureDir $Name
  [IO.File]::WriteAllText($path, $Text, [Text.Encoding]::UTF8)
  return $path
}
$goodXml = Fixture 'pass.xml' '<test-run total="1" passed="1" failed="0" result="Passed"><test-case fullname="sample" result="Passed"/></test-run>'
Check ((Get-EditTestSummary $goodXml 0) -eq '1/1 passed') 'Complete passing NUnit result accepted'
Reject { Get-EditTestSummary $goodXml 1 } 'Nonzero test process exit rejected despite passing XML'
$zeroXml = Fixture 'zero.xml' '<test-run total="0" passed="0" failed="0" result="Passed"/>'
Reject { Get-EditTestSummary $zeroXml 0 } 'Zero-test result rejected'
$falseXml = Fixture 'incomplete.xml' '<test-run total="2" passed="2" failed="0" result="Passed"><test-case result="Passed"/></test-run>'
Reject { Get-EditTestSummary $falseXml 0 } 'Inconsistent test count rejected'
$skipXml = Fixture 'skip.xml' '<test-run total="1" passed="0" failed="0" result="Passed"><test-case result="Skipped"/></test-run>'
Reject { Get-EditTestSummary $skipXml 0 } 'Skipped test result rejected'
Reject { Get-EditTestSummary (Join-Path $fixtureDir 'missing.xml') 0 } 'Missing test XML rejected'
$cleanLog = Fixture 'clean.log' 'Power Above All build succeeded: a fresh folder'
Check ((Assert-CleanLog $cleanLog).Contains('succeeded')) 'Clean log accepted'
$badLog = Fixture 'runtime.log' "Frame started`nNullReferenceException: sample failure"
Reject { Assert-CleanLog $badLog } 'Runtime exception marker rejected'
$compilerLog = Fixture 'compile.log' 'Assets/Test.cs(1,2): error CS1002: ; expected'
Reject { Assert-CleanLog $compilerLog } 'C# compile error marker rejected'
$plan = Get-ReviewPlan (Join-Path $PSScriptRoot 'shots.script')
Check ($plan.Captures.Count -eq 27 -and $plan.Assertions -gt 20 -and $plan.States.Count -eq 3) 'Full review has frames, state evidence and assertions'
$journeyPlan = Get-ReviewPlan (Join-Path $PSScriptRoot 'long-campaign.script')
Check ($journeyPlan.Captures.Count -eq 12 -and $journeyPlan.Assertions -gt 25 -and $journeyPlan.States.Count -eq 4) 'Long campaign review parses six-week evidence and save assertions'
$tacticalPlan = Get-ReviewPlan (Join-Path $PSScriptRoot 'tactical-campaign.script')
Check ($tacticalPlan.Captures.Count -eq 9 -and $tacticalPlan.States.Count -eq 8 -and $tacticalPlan.Assertions -ge 12) 'Natural tactical review includes all battle snapshots and return assertion in receipt plan'
foreach ($victoryScript in @('victory-campaign.script', 'victory-recognize.script')) {
  $victoryPlan = Get-ReviewPlan (Join-Path $PSScriptRoot $victoryScript)
  Check ($victoryPlan.Captures.Count -eq 13 -and $victoryPlan.States.Count -eq 12 -and $victoryPlan.Assertions -ge 25) "Natural victory decision receipt includes combat, pending load and committed save evidence: $victoryScript"
}
$badVictory = Fixture 'injected-victory.script' "new`nexpect HasPendingVictory True`nvictory bonus 0`nshot sample`nquit"
Reject { Get-ReviewPlan $badVictory } 'Decision review cannot override the domain-computed price'
$badPending = Fixture 'ambiguous-victory.script' "new`nexpect HasPendingVictory maybe`nshot sample`nquit"
Reject { Get-ReviewPlan $badPending } 'Ambiguous pending-offer expectation is rejected before a natural battle runs'
$dumasPlan = Fixture 'dumas-response.script' "new`nexpect HasDumasInitiative True`npanel initiative`nforage veto`nshot sample`nquit"
Check ((Get-ReviewPlan $dumasPlan).Assertions -eq 1) 'Dumas review opens the actual document and sends only the supported veto response'
$badDumas = Fixture 'injected-dumas.script' "new`nexpect HasDumasInitiative True`nforage gather 40`nshot sample`nquit"
Reject { Get-ReviewPlan $badDumas } 'Review cannot force or price an autonomous Dumas collection'
$ambiguousDumas = Fixture 'ambiguous-dumas.script' "new`nexpect HasDumasInitiative maybe`nshot sample`nquit"
Reject { Get-ReviewPlan $ambiguousDumas } 'Ambiguous Dumas state is rejected before player launch'
$badBattle = Fixture 'invalid-battle.script' "new`nexpect Week 0`nbattle finish true`nshot sample`nquit"
Reject { Get-ReviewPlan $badBattle } 'Review grammar cannot inject a battle outcome'
$badWait = Fixture 'unbounded-battle.script' "new`nexpect Week 0`nbattle wait ended 121`nshot sample`nquit"
Reject { Get-ReviewPlan $badWait } 'Battle condition wait is bounded before player launch'
$badPoint = Fixture 'nonfinite-battle.script' "new`nexpect Week 0`nbattle move NaN 6`nshot sample`nquit"
Reject { Get-ReviewPlan $badPoint } 'Nonfinite battlefield destination is rejected'
$duplicateBattleState = Fixture 'duplicate-battle-state.script' "new`nexpect Week 0`nstate report`nbattle state report`nshot sample`nquit"
Reject { Get-ReviewPlan $duplicateBattleState } 'Campaign and battle state artifact names cannot collide'
$badScript = Fixture 'unsafe.script' "new`nexpect Week 0`nshot ../human-file`nquit"
Reject { Get-ReviewPlan $badScript } 'Artifact path traversal rejected'
$duplicateScript = Fixture 'duplicate.script' "new`nexpect Week 0`nshot sample`nshot sample`nquit"
Reject { Get-ReviewPlan $duplicateScript } 'Duplicate artifacts rejected'
$earlyQuit = Fixture 'early-quit.script' "new`nexpect Week 0`nshot sample`nquit`nwait 1`nquit"
Reject { Get-ReviewPlan $earlyQuit } 'Early quit rejected'
Reject { Assert-ReviewResult $fixtureDir $plan 0 } 'Missing completion receipt rejected'
Reject { Assert-ReviewResult $fixtureDir $plan -1073741819 } 'Native access violation remains a failed player result'
$fixturePlayer = Fixture 'fixture.exe' 'Verification fixture only; never executed.'
$fixtureManaged = Join-Path $fixtureDir 'fixture_Data\Managed'
New-Item -ItemType Directory -Path $fixtureManaged | Out-Null
$fixtureAssembly = Join-Path $fixtureManaged 'PowerAboveAll.Runtime.dll'
[IO.File]::WriteAllText($fixtureAssembly, 'AUTO_SHOTS_PROTOCOL 2', [Text.Encoding]::Unicode)
Check ((Assert-ReviewProtocol $fixturePlayer) -eq $fixtureAssembly) 'Runtime asmdef assembly is found and protocol verified'
$legacyPlayer = Fixture 'legacy.exe' 'Verification fixture only; never executed.'
$legacyManaged = Join-Path $fixtureDir 'legacy_Data\Managed'
New-Item -ItemType Directory -Path $legacyManaged | Out-Null
[IO.File]::WriteAllText((Join-Path $legacyManaged 'PowerAboveAll.Runtime.dll'), 'old harness', [Text.Encoding]::Unicode)
Reject { Assert-ReviewProtocol $legacyPlayer } 'Legacy runtime assembly is rejected before any launch'
$fixtureManifest = @(Get-BuildFileManifest $fixtureDir)
$runtimeEntry = @($fixtureManifest | Where-Object { $_.path -eq 'fixture_Data/Managed/PowerAboveAll.Runtime.dll' })
Check ($runtimeEntry.Count -eq 1 -and $runtimeEntry[0].size -eq (Get-Item -LiteralPath $fixtureAssembly).Length -and $runtimeEntry[0].sha256 -match '^[a-f0-9]{64}$') 'Manifest includes nested normalized path, size and SHA256'
Check (@($fixtureManifest | Where-Object { $_.path.Contains('\') -or $_.path.StartsWith('/') }).Count -eq 0) 'Manifest contains relative slash-normalized paths only'
if ($PlayerPath) {
  Check ((Assert-ReviewProtocol $PlayerPath).EndsWith('PowerAboveAll.Runtime.dll')) 'Existing real player protocol verified without launch'
  $realManifest = @(Get-BuildFileManifest (Split-Path -Parent $PlayerPath))
  Check (@($realManifest | Where-Object { $_.path -eq 'UnityPlayer.dll' -or $_.path -eq 'Power Above All_Data/resources.assets' }).Count -eq 2) 'Real built engine and resource packages are covered by manifest'
  Write-Output "Actual player manifest: $($realManifest.Count) files hashed without changing the build."
}
Write-Output "$testCount safety checks passed. Fixtures preserved: $fixtureDir"
