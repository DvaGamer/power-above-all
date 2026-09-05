'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');

function readJson(file) { return JSON.parse(fs.readFileSync(file, 'utf8').replace(/^\uFEFF/, '')); }
function digest(file) { return crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex'); }
function samePath(a, b) { return path.resolve(a).toLowerCase() === path.resolve(b).toLowerCase(); }

// Yalnız aynı koşuda bütün kapıları geçmiş, içeriği değişmemiş oyuncu seçilir.
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
      return { executable: candidate.executable, verified: true, evidence: path.join(candidate.directory, 'REPORT.md') };
    } catch { /* Silinmiş veya değiştirilmiş derleme yerine önceki sağlam koşu denenir. */ }
  }
  return null;
}

function findPlayer(root) {
  const verified = findVerifiedPlayer(root);
  if (verified) return verified;
  const legacy = ['WindowsPolish', 'Windows'].map(folder => path.join(root, 'Unity', 'Builds', folder, 'Power Above All.exe'))
    .filter(file => fs.existsSync(file)).sort((a, b) => fs.statSync(b).mtimeMs - fs.statSync(a).mtimeMs)[0];
  return legacy ? { executable: legacy, verified: false, evidence: null } : null;
}

module.exports = { findPlayer, findVerifiedPlayer };
