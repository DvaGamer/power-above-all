const fs = require('node:fs');
const path = require('node:path');
const { spawn } = require('node:child_process');
const { findPlayer } = require('./tools/verified-player.cjs');
const selected = findPlayer(__dirname);
if (!selected) {
  console.error('Windows oyunu henüz derlenmedi. Unity menüsünden Power Above All > Build Windows seçin.');
  process.exitCode = 1;
} else if (process.argv.includes('--check')) {
  console.log(JSON.stringify(selected, null, 2));
} else {
  fs.mkdirSync(path.join(__dirname, 'output'), { recursive: true });
  const child = spawn(selected.executable, ['-screen-width', '1440', '-screen-height', '900', '-screen-fullscreen', '0',
    '-logFile', path.join(__dirname, 'output', 'player.log')], { detached: true, stdio: 'ignore', windowsHide: false });
  child.on('error', error => { console.error(error.message); process.exitCode = 1; });
  child.unref();
  console.log('Power Above All — PID ' + child.pid);
  if (selected.verified) console.log('Doğrulama: ' + selected.evidence);
}
