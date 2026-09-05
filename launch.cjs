const http = require('node:http');
const { spawn } = require('node:child_process');
const path = require('node:path');
const url = 'http://127.0.0.1:1789';
function inspectServer() {
  return new Promise(resolve => {
    const request = http.get(url, response => {
      let text = '';
      response.setEncoding('utf8');
      response.on('data', chunk => { text += chunk; });
      response.on('end', () => resolve(text.includes('<title>Power Above All') ? 'ready' : 'occupied'));
      response.on('error', () => resolve('absent'));
    });
    request.setTimeout(700, () => { request.destroy(); resolve('absent'); });
    request.on('error', () => resolve('absent'));
  });
}
async function main() {
  let status = await inspectServer();
  if (status === 'occupied') throw new Error('Port 1789 is occupied by another application. Close it before launching.');
  if (status !== 'ready') {
    const server = spawn(process.execPath, [path.join(__dirname, 'server.cjs')], {
      cwd: __dirname, detached: true, stdio: 'ignore', windowsHide: true,
      env: { ...process.env, PORT: '1789' }
    });
    server.unref();
    for (let attempt = 0; attempt < 20; attempt++) {
      await new Promise(resolve => setTimeout(resolve, 250));
      status = await inspectServer();
      if (status === 'ready') break;
      if (status === 'occupied') throw new Error('Port 1789 is occupied by another application.');
    }
  }
  if (status !== 'ready') throw new Error('Could not start the local game server. Run node server.cjs to see details.');
  if (process.argv.includes('--check')) { console.log(`Ready: ${url}`); return; }
  const browser = process.platform === 'win32'
    ? spawn('cmd.exe', ['/d', '/c', 'start', '', url], { detached: true, stdio: 'ignore', windowsHide: true })
    : spawn(process.platform === 'darwin' ? 'open' : 'xdg-open', [url], { detached: true, stdio: 'ignore' });
  browser.on('error', () => console.log(`Open ${url} in your browser.`));
  browser.unref();
  console.log(`Power Above All: ${url}`);
}
main().catch(error => { console.error(error.message); process.exitCode = 1; });
