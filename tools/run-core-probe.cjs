'use strict';
// Saf Core incelemesi: Unity açılmaz; her koşu ayrı ikili, kaynak özeti ve günlük bırakır.
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const { spawnSync } = require('node:child_process');
const root = path.resolve(__dirname, '..');
const name = process.argv[2];
if (!name || !/^[A-Za-z][A-Za-z0-9_]*$/.test(name)) throw new Error('Usage: node tools/run-core-probe.cjs WorkNotesProbeClassName');
const data = 'C:/Users/USER/Tools/Unity/6000.3.23f1/Editor/Data/MonoBleedingEdge';
const mono = path.join(data, 'bin', 'mono.exe');
const compiler = path.join(data, 'lib', 'mono', '4.5', 'mcs.exe');
const sources = ['CampaignCore', 'CampaignRoles', 'CampaignPatronTrust', 'CampaignRegionalAccords', 'CampaignVictoryDecisions', 'CampaignArchive']
  .map(file => path.join(root, 'Unity/Assets/Scripts/Core', file + '.cs'));
sources.push(path.join(root, 'Unity/WorkNotes', name + '.cs'));
for (const file of [mono, compiler, ...sources]) if (!fs.statSync(file).isFile()) throw new Error('Required file missing: ' + file);
const started = new Date().toISOString();
const out = path.join(root, 'output', 'core-probes', name + '-' + started.replace(/[:.]/g, '-') + '-' + crypto.randomBytes(4).toString('hex'));
fs.mkdirSync(out, { recursive: true });
const receipt = { kind: 'pure-core-probe', unityOrPlayerVerified: false, startedUtc: started, sources: sources.map(file => ({ path: path.relative(root, file).replaceAll('\\', '/'), sha256: crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex') })) };
function run(stage, args) {
  const result = spawnSync(mono, args, { cwd: root, encoding: 'utf8', windowsHide: true, timeout: 60000, maxBuffer: 8 * 1024 * 1024 });
  for (const stream of ['stdout', 'stderr']) fs.writeFileSync(path.join(out, stage + '.' + stream + '.log'), result[stream] || '', { flag: 'wx' });
  receipt[stage] = { exitCode: result.status, signal: result.signal, error: result.error ? result.error.message : null };
  if (result.error || result.status !== 0) throw new Error(stage + ' failed; see ' + out);
}
try {
  const executable = path.join(out, name + '.exe');
  run('compile', [compiler, '-nologo', '-r:System.Runtime.Serialization', '-out:' + executable, ...sources]);
  run('probe', [executable]);
  receipt.verdict = 'PASS';
} catch (error) {
  receipt.verdict = 'FAILED'; receipt.failure = error.message; process.exitCode = 1;
} finally {
  receipt.completedUtc = new Date().toISOString();
  fs.writeFileSync(path.join(out, 'result.json'), JSON.stringify(receipt, null, 2) + '\n', { flag: 'wx' });
  console.log(receipt.verdict + ': ' + out);
  if (receipt.failure) console.error(receipt.failure);
}
