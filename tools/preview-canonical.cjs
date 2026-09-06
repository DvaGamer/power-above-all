// Kaynak SVG'leri değiştirmeden özgün alfabenin görsel inceleme sayfasını üretir.
const fs=require('node:fs'),path=require('node:path'),{spawnSync}=require('node:child_process');
const repo=path.resolve(__dirname,'..');
const out=path.join(repo,'output','canonical-preview-'+Date.now());fs.mkdirSync(out,{recursive:true});
const names=['tree','building','road','river','cloud','soldier','musket_smoke','ui_icon','document_ornament'];
const cards=names.map(name=>`<section><h2>${name.replaceAll('_',' ')}</h2><div style="background:${name==='cloud'?'#83B0B6':name==='musket_smoke'?'#A9BA88':'transparent'}">${fs.readFileSync(path.join(repo,'Art','Canonical',name+'.svg'),'utf8')}</div><aside style="background:${name==='cloud'?'#83B0B6':name==='musket_smoke'?'#A9BA88':'transparent'}">32px: ${fs.readFileSync(path.join(repo,'Art','Canonical',name+'.svg'),'utf8')}</aside></section>`).join('');
const page=path.join(out,'alphabet.html'),shot=path.join(out,'alphabet.png');
fs.writeFileSync(page,`<!doctype html><meta charset="utf-8"><style>*{box-sizing:border-box}body{margin:0;padding:30px;background:#243B37;color:#F3E7CA;font:16px Georgia}h1{font-size:28px;font-weight:normal;margin:0 0 8px}p{margin:0 0 24px}main{display:grid;grid-template-columns:repeat(3,1fr);gap:16px}section{background:#F3E7CA;color:#243B37;height:270px;padding:16px}h2{font-size:19px;font-weight:normal;margin:0}section div{height:170px;display:flex;align-items:center;justify-content:center}svg{display:block;height:145px;max-width:100%}aside{display:flex;align-items:center;gap:10px;font:12px sans-serif}aside svg{height:32px;width:80px}</style><h1>POWER ABOVE ALL · Görsel alfabe</h1><p>Dokuz özgün SVG · elle belirlenmiş biçim ve kontrollü asimetri · kaynak incelemesi, oyun screenshot'ı değildir</p><main>${cards}</main>`);
const chrome='C:/Program Files/Google/Chrome/Application/chrome.exe';
const result=spawnSync(chrome,['--headless=new','--disable-gpu','--no-first-run','--disable-background-networking','--user-data-dir='+path.join(out,'browser-profile'),'--window-size=1320,1020','--screenshot='+shot,'file:///'+page.replaceAll('\\','/')],{windowsHide:true,timeout:45000,encoding:'utf8'});
if(result.error||result.status!==0||!fs.existsSync(shot)){console.error(result.error||result.stderr);process.exit(1);}
console.log(shot);
