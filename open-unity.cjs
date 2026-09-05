const fs = require('node:fs');
const path = require('node:path');
const { spawn } = require('node:child_process');
const version = '6000.3.23f1';
const home = process.env.USERPROFILE || '';
const candidates = [process.env.UNITY_EDITOR,
  path.join(home, 'Tools', 'Unity', version, 'Editor', 'Unity.exe'),
  path.join(process.env.ProgramFiles || 'C:\\Program Files', 'Unity', 'Hub', 'Editor', version, 'Editor', 'Unity.exe')];
const editor = candidates.find(file => file && fs.existsSync(file));
if (!editor) {
  console.error('Unity 6000.3.23f1 bulunamadı. Hub içinden Unity klasörünü açın veya UNITY_EDITOR yolunu ayarlayın.');
  process.exitCode = 1;
} else {
  const child = spawn(editor, ['-projectPath', path.join(__dirname, 'Unity')], { detached: true, stdio: 'ignore', windowsHide: false });
  child.on('error', error => { console.error(error.message); process.exitCode = 1; });
  child.unref();
}
