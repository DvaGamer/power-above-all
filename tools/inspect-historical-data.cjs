// Public dataset catalogue acquisition through the Windows network stack.
const fs = require('node:fs');
const path = require('node:path');
(async () => {
  const url = 'https://dataverse.harvard.edu/api/datasets/:persistentId/?persistentId=doi:10.7910/DVN/T8UXHK';
  const r = await fetch(url);
  if (!r.ok) throw new Error(`Catalogue HTTP ${r.status}`);
  const data = await r.json();
  const dest = path.join(__dirname, '../output/gis/bailliages-catalogue.json');
  fs.mkdirSync(path.dirname(dest), {recursive:true});
  fs.writeFileSync(dest, JSON.stringify(data, null, 2));
  const v = data.data.latestVersion;
  console.log('VERSION', v.versionNumber, v.versionMinorNumber, 'LICENSE', v.license);
  for (const entry of v.files) { const f = entry.dataFile; console.log(f.id, f.filename, f.filesize); }
  if (process.argv.includes('--download')) {
    const names = ['FRANCE_1789_BRETTE.zip', 'BAILLIAGES_1789_BRETTE.zip', 'README.txt'];
    for (const entry of v.files.filter(e => names.includes(e.dataFile.filename))) {
      const f = entry.dataFile;
      const response = await fetch(`https://dataverse.harvard.edu/api/access/datafile/${f.id}`);
      if (!response.ok) throw new Error(`Download ${f.id}: HTTP ${response.status}`);
      const bytes = Buffer.from(await response.arrayBuffer());
      const algorithm = f.checksum.type.toLowerCase().replace('-', '');
      const hash = require('node:crypto').createHash(algorithm).update(bytes).digest('hex');
      if (hash !== f.checksum.value.toLowerCase()) throw new Error(`Checksum mismatch ${f.filename}`);
      fs.writeFileSync(path.join(path.dirname(dest), f.filename), bytes);
      console.log('VERIFIED', f.filename, bytes.length, hash);
    }
  }
})().catch(e => { console.error(e.message); process.exitCode=1; });
