const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');
const folder = path.resolve(process.argv[2]);
const names = ['00-strict-start','04-mission-start','08-wait-start','01-strict-unreported','02-strict-reply','05-mission-unreported','06-mission-reply','09-wait-new-information','03-strict-four-weeks','07-mission-four-weeks','11-wait-four-weeks'];
const states = names.map(name => ({name, state: JSON.parse(fs.readFileSync(path.join(folder,name+'.json'),'utf8').replace(/^\uFEFF/,''))}));
assert.deepEqual(states[0].state, states[1].state, 'strict and mission must have identical starting states');
assert.deepEqual(states[0].state, states[2].state, 'waiting must have the same starting state');
console.log('| Scenario | Week | Actual unrest | Known unrest | Control | Gold | Power | Delmas loyalty / ambition |');
console.log('|---|---:|---:|---:|---:|---:|---:|---|');
for(const {name,state:s} of states.slice(3)) {
  const r=s.Regions.find(r=>r.Id==='guyenne'), d=s.Correspondence[0];
  console.log(`| ${name} | ${s.Week} | ${r.Unrest} | ${d.LastReport.Unrest} | ${r.Control} | ${s.Gold} | ${s.Power} | ${d.Loyalty} / ${d.Ambition} |`);
}
console.log('Verified identical starts. The table reports observed outcomes, not enjoyment or general balance.');
