'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const { findVerifiedPlayer } = require('./verified-player.cjs');

function fixture() {
  const directory = path.join(__dirname, '..', 'output', 'launcher-tests');
  fs.mkdirSync(directory, { recursive: true });
  return fs.mkdtempSync(path.join(directory, 'run-'));
}
function build(root, label, date, verdict = 'GREEN', manifest = true) {
  const directory = path.join(root, 'output', 'verify', label);
  const executable = path.join(directory, 'player-build', 'Power Above All.exe');
  const assembly = path.join(directory, 'player-build', 'Power Above All_Data', 'Managed', 'PowerAboveAll.Runtime.dll');
  fs.mkdirSync(path.dirname(assembly), { recursive: true });
  fs.writeFileSync(executable, 'fixture player ' + label);
  fs.writeFileSync(assembly, 'fixture runtime ' + label);
  const engine = path.join(directory, 'player-build', 'UnityPlayer.dll');
  const resources = path.join(directory, 'player-build', 'Power Above All_Data', 'resources.assets');
  const globals = path.join(directory, 'player-build', 'Power Above All_Data', 'globalgamemanagers');
  fs.writeFileSync(engine, 'fixture engine ' + label);
  fs.writeFileSync(resources, 'fixture resources ' + label);
  fs.writeFileSync(globals, 'fixture globals ' + label);
  const hash = file => crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex');
  const files = [executable, assembly, engine, resources, globals].map(file => ({
    path: path.relative(path.dirname(executable), file).split(path.sep).join('/'), size: fs.statSync(file).size, sha256: hash(file)
  }));
  fs.writeFileSync(path.join(directory, 'build-result.json'), '\uFEFF' + JSON.stringify({
    playerPath: executable, builtUtc: date, playerSha256: hash(executable), assemblySha256: hash(assembly),
    ...(manifest ? { manifestVersion: 1, files } : {})
  }));
  const gates = Object.fromEntries(['Preflight', 'EditMode', 'Build', 'Player', 'Frames', 'Browser'].map(key => [key, 'PASSED']));
  fs.writeFileSync(path.join(directory, 'result.json'), JSON.stringify({ verdict, artifacts: directory, gates }));
  return { directory, executable, assembly, engine, resources };
}

function changeReceipt(candidate, action) {
  const file = path.join(candidate.directory, 'build-result.json');
  const receipt = JSON.parse(fs.readFileSync(file, 'utf8').replace(/^\uFEFF/, ''));
  action(receipt);
  fs.writeFileSync(file, JSON.stringify(receipt));
}

test('newest complete GREEN build wins; newer RED and PARTIAL are excluded', () => {
  const root = fixture();
  const good = build(root, 'good', '2026-09-05T20:00:00Z');
  build(root, 'failed', '2026-09-05T21:00:00Z', 'RED');
  build(root, 'partial', '2026-09-05T22:00:00Z', 'PARTIAL');
  assert.equal(findVerifiedPlayer(root).executable, good.executable);
  assert.equal(findVerifiedPlayer(root).integrity, 'complete-build');
});
test('modified latest runtime falls back to previous verified build', () => {
  const root = fixture();
  const old = build(root, 'old', '2026-09-05T20:00:00Z');
  const changed = build(root, 'changed', '2026-09-05T21:00:00Z');
  fs.appendFileSync(changed.assembly, 'unverified changes');
  assert.equal(findVerifiedPlayer(root).executable, old.executable);
});
test('missing gate is excluded even if receipt incorrectly says GREEN', () => {
  const root = fixture();
  const candidate = build(root, 'incomplete', '2026-09-05T21:00:00Z');
  const resultPath = path.join(candidate.directory, 'result.json');
  const result = JSON.parse(fs.readFileSync(resultPath, 'utf8'));
  result.gates.Player = 'NOT RUN';
  fs.writeFileSync(resultPath, JSON.stringify(result));
  assert.equal(findVerifiedPlayer(root), null);
});
test('missing artifacts yield no verified player without throwing', () => {
  assert.equal(findVerifiedPlayer(fixture()), null);
});
test('missing engine DLL rejects latest build and preserves older selection', () => {
  const root = fixture();
  const old = build(root, 'old', '2026-09-05T20:00:00Z');
  const changed = build(root, 'changed', '2026-09-05T21:00:00Z');
  // Yalniz bu testin tek kullanımlik fixture dosyasi; klasor silinmez.
  fs.unlinkSync(changed.engine);
  assert.equal(findVerifiedPlayer(root).executable, old.executable);
});
test('same-size modified resources are rejected by content hash', () => {
  const root = fixture();
  const candidate = build(root, 'assets', '2026-09-05T21:00:00Z');
  const original = fs.readFileSync(candidate.resources);
  original[0] ^= 1;
  fs.writeFileSync(candidate.resources, original);
  assert.equal(findVerifiedPlayer(root), null);
});
test('unlisted added artifacts and incomplete manifests are rejected', () => {
  const root = fixture();
  const candidate = build(root, 'extra', '2026-09-05T21:00:00Z');
  fs.writeFileSync(path.join(path.dirname(candidate.executable), 'extra.dll'), 'unverified library');
  assert.equal(findVerifiedPlayer(root), null);
  const missingRoot = fixture();
  const incomplete = build(missingRoot, 'incomplete', '2026-09-05T21:00:00Z');
  changeReceipt(incomplete, receipt => { receipt.files.pop(); });
  assert.equal(findVerifiedPlayer(missingRoot), null);
});
test('unsafe and duplicate manifest paths are rejected without fallback to old proof', () => {
  for (const pathValue of ['../outside.dll', '/root.dll', 'C:/outside.dll', 'nested\\outside.dll']) {
    const root = fixture();
    const candidate = build(root, 'unsafe', '2026-09-05T21:00:00Z');
    changeReceipt(candidate, receipt => { receipt.files[0].path = pathValue; });
    assert.equal(findVerifiedPlayer(root), null);
  }
  const root = fixture();
  const candidate = build(root, 'duplicate', '2026-09-05T21:00:00Z');
  changeReceipt(candidate, receipt => { receipt.files[1] = receipt.files[0]; });
  assert.equal(findVerifiedPlayer(root), null);
});
test('premanifest GREEN baseline remains selectable with limited integrity label', () => {
  const root = fixture();
  const candidate = build(root, 'baseline', '2026-09-05T21:00:00Z', 'GREEN', false);
  const selected = findVerifiedPlayer(root);
  assert.equal(selected.executable, candidate.executable);
  assert.equal(selected.integrity, 'executable-and-runtime');
});
