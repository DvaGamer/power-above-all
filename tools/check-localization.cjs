'use strict';
// Tabloları oyun açmadan denetler; ekran taşmasını veya çeviri kalitesini ölçmez.
const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');
const directory = path.resolve(__dirname, '../Unity/Assets/Resources/Localization');
const definitions = new Map();
const assetGuids = new Map();
let entries = 0;
function placeholders(text) {
  return [...new Set([...text.replaceAll('{{', '').replaceAll('}}', '').matchAll(/\{(\d+)(?:[^{}]*)\}/g)].map(match => match[1]))].sort();
}
for (const file of fs.readdirSync(directory).filter(file => file.endsWith('.json')).sort()) {
  const metadata = fs.readFileSync(path.join(directory, file + '.meta'), 'utf8');
  const guid = metadata.match(/^guid:[ \t]+([a-fA-F0-9]{32})[ \t]*\r?$/m);
  assert.ok(guid, `${file}: Unity metadata requires an exact 32-digit hexadecimal GUID`);
  const assetGuid = guid[1].toLowerCase();
  assert.ok(!assetGuids.has(assetGuid), `${file}: duplicate asset GUID also used by ${assetGuids.get(assetGuid)}`);
  assetGuids.set(assetGuid, file);
  const table = JSON.parse(fs.readFileSync(path.join(directory, file), 'utf8').replace(/^\uFEFF/, ''));
  assert.ok(Array.isArray(table.entries), `${file}: entries array is missing`);
  for (const entry of table.entries) {
    assert.match(entry.key, /^[a-zA-Z0-9_.-]+$/, `${file}: invalid localization key`);
    assert.ok(!definitions.has(entry.key), `${file}: duplicate ${entry.key}, first defined in ${definitions.get(entry.key)}`);
    for (const language of ['ru', 'tr']) {
      assert.equal(typeof entry[language], 'string', `${file}: missing ${language} ${entry.key}`);
      assert.ok(entry[language].trim(), `${file}: empty ${language} ${entry.key}`);
      assert.ok(!/[\u0000-\u0008\u000b\u000c\u000e-\u001f]/.test(entry[language]), `${file}: control character ${entry.key}`);
    }
    assert.deepEqual(placeholders(entry.ru), placeholders(entry.tr), `${file}: RU/TR argument mismatch ${entry.key}`);
    definitions.set(entry.key, file);
    entries++;
  }
}
console.log(`PASS: ${entries} unique RU/TR entries in ${assetGuids.size} distinct valid Unity assets; text and placeholder parity checked.`);
