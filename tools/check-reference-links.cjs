const fs=require('node:fs'),path=require('node:path');
const repo=path.resolve(__dirname,'..');let count=0,missing=[];
function walk(folder){return fs.readdirSync(folder,{withFileTypes:true}).flatMap(e=>e.name==='_local_media'?[]:e.isDirectory()?walk(path.join(folder,e.name)):e.name.endsWith('.md')?[path.join(folder,e.name)]:[]);}
for(const file of [path.join(repo,'REFERENCES.md'),...walk(path.join(repo,'References'))]){
  const body=fs.readFileSync(file,'utf8');
  for(const match of body.matchAll(/\]\(([^)]+)\)/g)){
    let link=match[1].replace(/^<|>$/g,'').split('#')[0];if(!link||/^\w+:/.test(link))continue;
    count++;if(!fs.existsSync(path.resolve(path.dirname(file),decodeURIComponent(link))))missing.push(path.relative(repo,file)+': '+link);
  }
}
if(missing.length){console.error(missing.join('\n'));process.exit(1);}
console.log(count+' local reference links resolve. External URLs and historical claims require source review.');
