'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const source = path.resolve(process.argv[2] || '');
const root = path.resolve(__dirname, '..');
const destination = path.join(root, 'Unity', 'Assets', 'Resources', 'Art', 'PoliticalPortraits-v1.png');
if (!process.argv[2] || !fs.existsSync(source)) throw new Error('Generated portrait source is required');
if (fs.existsSync(destination)) throw new Error('Portrait version already exists; preserve it and use a new version');
fs.mkdirSync(path.dirname(destination), { recursive: true });
fs.copyFileSync(source, destination, fs.constants.COPYFILE_EXCL);
console.log(JSON.stringify({ destination, bytes: fs.statSync(destination).size,
  sha256: crypto.createHash('sha256').update(fs.readFileSync(destination)).digest('hex') }, null, 2));
