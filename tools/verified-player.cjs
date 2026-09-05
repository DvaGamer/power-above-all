'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');

function readJson(file) { return JSON.parse(fs.readFileSync(file, 'utf8').replace(/^\uFEFF/, '')); }
function digest(file) {
  const hash = crypto.createHash('sha256');
  const buffer = Buffer.allocUnsafe(1024 * 1024);
  const handle = fs.openSync(file, 'r');
  try {
    let size;
    while ((size = fs.readSync(handle, buffer, 0, buffer.length, null)) > 0) hash.update(buffer.subarray(0, size));
    return hash.digest('hex');
  } finally { fs.closeSync(handle); }
}
function samePath(a, b) { return path.resolve(a).toLowerCase() === path.resolve(b).toLowerCase(); }

function shippedFiles(directory) {
  if (!fs.lstatSync(directory).isDirectory() || fs.lstatSync(directory).isSymbolicLink()) throw new Error('Linked build directory');
  const files = new Map();
  const pending = [directory];
  while (pending.length) {
    const parent = pending.pop();
    for (const entry of fs.readdirSync(parent, { withFileTypes: true })) {
      const full = path.join(parent, entry.name);
      const stat = fs.lstatSync(full);
      if (stat.isSymbolicLink()) throw new Error('Linked build artifact');
      if (stat.isDirectory()) pending.push(full);
      else if (stat.isFile()) {
        const relative = path.relative(directory, full).split(path.sep).join('/');
        files.set(relative.toLowerCase(), { full, size: stat.size });
      } else throw new Error('Unsupported build artifact');
    }
  }
  return files;
}

function manifestMatches(directory, build) {
  if (build.manifestVersion !== 1 || !Array.isArray(build.files) || build.files.length === 0) return false;
  const actual = shippedFiles(directory);
  if (actual.size !== build.files.length) return false;
  const listed = new Set();
  for (const file of build.files) {
    if (!file || typeof file.path !== 'string' || /[\\:\x00]/.test(file.path) ||
        file.path.split('/').some(part => !part || part === '.' || part === '..') ||
        !Number.isSafeInteger(file.size) || file.size < 0 || !/^[a-fA-F0-9]{64}$/.test(file.sha256)) return false;
    const key = file.path.toLowerCase();
    const artifact = actual.get(key);
    if (listed.has(key) || !artifact || artifact.size !== file.size || digest(artifact.full) !== file.sha256.toLowerCase()) return false;
    listed.add(key);
  }
  return ['power above all.exe', 'unityplayer.dll', 'power above all_data/managed/poweraboveall.runtime.dll',
    'power above all_data/globalgamemanagers', 'power above all_data/resources.assets'].every(name => listed.has(name));
}

// Yeni makbuz bütün dosyalara, eski makbuz yalnız exe/runtime kanıtına sahiptir.
function findVerifiedPlayer(root) {
  const verification = path.join(root, 'output', 'verify');
  if (!fs.existsSync(verification)) return null;
  const candidates = [];
  for (const entry of fs.readdirSync(verification, { withFileTypes: true })) {
    if (!entry.isDirectory()) continue;
    const directory = path.join(verification, entry.name);
    try {
      const result = readJson(path.join(directory, 'result.json'));
      const build = readJson(path.join(directory, 'build-result.json'));
      if (result.verdict !== 'GREEN' || !samePath(result.artifacts, directory)) continue;
      const executable = path.join(directory, 'player-build', 'Power Above All.exe');
      const assembly = path.join(directory, 'player-build', 'Power Above All_Data', 'Managed', 'PowerAboveAll.Runtime.dll');
      if (!samePath(build.playerPath, executable)) continue;
      const required = ['Preflight', 'EditMode', 'Build', 'Player', 'Frames', 'Browser'];
      if (!result.gates || required.some(key => !String(result.gates[key]).startsWith('PASSED'))) continue;
      const builtAt = Date.parse(build.builtUtc);
      if (!Number.isFinite(builtAt)) continue;
      candidates.push({ executable, assembly, directory, builtAt, build });
    } catch { /* Eksik veya henüz süren koşu seçilmez. */ }
  }
  candidates.sort((a, b) => b.builtAt - a.builtAt);
  for (const candidate of candidates) {
    try {
      if (digest(candidate.executable) !== String(candidate.build.playerSha256).toLowerCase() ||
          digest(candidate.assembly) !== String(candidate.build.assemblySha256).toLowerCase()) continue;
      const hasManifest = Object.hasOwn(candidate.build, 'manifestVersion') || Object.hasOwn(candidate.build, 'files');
      if (hasManifest && !manifestMatches(path.dirname(candidate.executable), candidate.build)) continue;
      return { executable: candidate.executable, verified: true, integrity: hasManifest ? 'complete-build' : 'executable-and-runtime',
        evidence: path.join(candidate.directory, 'REPORT.md') };
    } catch { /* Silinmiş veya değiştirilmiş derleme yerine önceki sağlam koşu denenir. */ }
  }
  return null;
}

function findPlayer(root) {
  const verified = findVerifiedPlayer(root);
  if (verified) return verified;
  const legacy = ['WindowsPolish', 'Windows'].map(folder => path.join(root, 'Unity', 'Builds', folder, 'Power Above All.exe'))
    .filter(file => fs.existsSync(file)).sort((a, b) => fs.statSync(b).mtimeMs - fs.statSync(a).mtimeMs)[0];
  return legacy ? { executable: legacy, verified: false, integrity: 'unverified', evidence: null } : null;
}

module.exports = { findPlayer, findVerifiedPlayer };
