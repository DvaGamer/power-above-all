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
function build(root, label, date, verdict = 'GREEN') {
  const directory = path.join(root, 'output', 'verify', label);
  const executable = path.join(directory, 'player-build', 'Power Above All.exe');
  const assembly = path.join(directory, 'player-build', 'Power Above All_Data', 'Managed', 'PowerAboveAll.Runtime.dll');
  fs.mkdirSync(path.dirname(assembly), { recursive: true });
  fs.writeFileSync(executable, 'fixture player ' + label);
  fs.writeFileSync(assembly, 'fixture runtime ' + label);
  const hash = file => crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex');
  fs.writeFileSync(path.join(directory, 'build-result.json'), '\uFEFF' + JSON.stringify({
    playerPath: executable, builtUtc: date, playerSha256: hash(executable), assemblySha256: hash(assembly)
  }));
  const gates = Object.fromEntries(['Preflight', 'EditMode', 'Build', 'Player', 'Frames', 'Browser'].map(key => [key, 'PASSED']));
  fs.writeFileSync(path.join(directory, 'result.json'), JSON.stringify({ verdict, artifacts: directory, gates }));
  return { directory, executable, assembly };
}

test('newest complete GREEN build wins; newer RED and PARTIAL are excluded', () => {
  const root = fixture();
  const good = build(root, 'good', '2026-09-05T20:00:00Z');
  build(root, 'failed', '2026-09-05T21:00:00Z', 'RED');
  build(root, 'partial', '2026-09-05T22:00:00Z', 'PARTIAL');
  assert.equal(findVerifiedPlayer(root).executable, good.executable);
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
