const test = require('node:test');
const assert = require('node:assert/strict');
const G = require('../simulation.js');
test('graph is symmetric and all references resolve',()=>{
  for(const r of G.regions)for(const id of r.neighbors)assert.ok(G.regions.find(n=>n.id===id).neighbors.includes(r.id));
});
test('resource failures are atomic and weekly actions cannot repeat',()=>{
  const s=G.createState();s.food=39;let before=JSON.stringify(s);
  assert.equal(G.action(s,'bread','ile').ok,false);assert.equal(JSON.stringify(s),before);
  s.food=40;assert.equal(G.action(s,'bread','ile').ok,true);assert.equal(s.food,0);
  before=JSON.stringify(s);assert.equal(G.action(s,'bread','ile').ok,false);assert.equal(JSON.stringify(s),before);
  assert.equal(G.action(s,'recruit','normandy').ok,false);
  s.gold=119;s.food=20;before=JSON.stringify(s);assert.equal(G.action(s,'recruit','ile').ok,false);assert.equal(JSON.stringify(s),before);
  s.gold=120;assert.equal(G.action(s,'recruit','ile').ok,true);assert.equal(s.gold,0);assert.equal(s.food,0);assert.equal(s.troops,1400);
  G.nextWeek(s);assert.deepEqual(s.regions.ile.used,{});
});
test('movement enforces adjacency, army strength and remaining moves',()=>{
  const s=G.createState();assert.equal(G.canMarch(s,'provence').ok,false);
  assert.equal(G.canMarch(s,'toString').ok,false);assert.equal(G.action(s,'bread','__proto__').ok,false);
  assert.equal(G.march(s,'normandy').ok,true);assert.equal(s.moves,1);
  assert.equal(G.march(s,'ile').ok,true);assert.equal(G.march(s,'normandy').ok,false);
  s.moves=2;s.troops=0;assert.equal(G.canMarch(s,'normandy').ok,false);
});
test('battle cannot be bypassed and result applies once',()=>{
  const s=G.createState();assert.equal(G.canMarch(s,'champagne').battle,true);assert.equal(G.march(s,'champagne').ok,false);
  const result={won:true,casualties:120,battleId:'battle-0-2-ile-champagne'};
  assert.equal(G.resolveBattle(s,'champagne',{...result,casualties:-2}).ok,false);
  assert.equal(G.resolveBattle(s,'champagne',{...result,battleId:'battle-0-1-ile-champagne'}).ok,false);
  assert.equal(G.resolveBattle(s,'champagne',result).ok,true);assert.equal(s.army,'champagne');assert.equal(s.troops,1080);assert.equal(s.moves,1);
  const loaded=G.deserialize(G.serialize(s)),before=JSON.stringify(loaded);
  assert.equal(G.resolveBattle(loaded,'champagne',result).ok,false);assert.equal(JSON.stringify(loaded),before);
});
test('defeat consumes a march and soldiers but keeps the army in place',()=>{
  const s=G.createState();assert.equal(G.resolveBattle(s,'champagne',{won:false,casualties:300,battleId:'battle-0-2-ile-champagne'}).ok,true);
  assert.equal(s.army,'ile');assert.equal(s.moves,1);assert.equal(s.troops,900);assert.equal(s.regions.champagne.unrest,74);
});
test('event blocks time, choices validate resources, and are consumed once',()=>{
  const s=G.createState();G.nextWeek(s);G.nextWeek(s);assert.equal(s.pendingEvent.id,'grain-petition');
  const before=G.serialize(s);assert.equal(G.nextWeek(s).ok,false);assert.equal(G.serialize(s),before);
  s.food=59;assert.equal(G.chooseEvent(s,'relief').ok,false);assert.ok(s.pendingEvent);
  s.food=60;assert.equal(G.chooseEvent(s,'relief').ok,true);assert.equal(s.food,0);assert.equal(s.pendingEvent,null);assert.equal(G.chooseEvent(s,'relief').ok,false);
  assert.equal(G.nextWeek(s).ok,true);assert.equal(s.week,3);
});
test('invalid saves reject malformed bounds, references and battle ledger',()=>{
  const fresh=G.createState();assert.deepEqual(G.deserialize(G.serialize(fresh)),fresh);
  const changes=[s=>s.version=2,s=>s.gold=-1,s=>s.food=null,s=>s.moves=3,s=>s.army='unknown',s=>s.selected='toString',s=>s.troops=2.5,s=>s.regions.ile.unrest=101,s=>s.regions.ile.used.cheat=true,s=>delete s.regions.ile,s=>s.pendingEvent={},s=>s.resolvedBattles=['battle-0-2-ile-provence'],s=>s.resolvedBattles=['battle-0-2-ile-champagne','battle-0-2-ile-champagne'],s=>s.finished=true];
  for(const mutate of changes){const s=G.createState();mutate(s);assert.throws(()=>G.deserialize(JSON.stringify(s)));}
  assert.throws(()=>G.deserialize('{broken'));
});
test('eight-week result occurs once and sandbox can continue',()=>{
  const s=G.createState();let results=[];
  for(let i=0;i<12;i++){if(s.pendingEvent)G.chooseEvent(s,'relief');const r=G.nextWeek(s);assert.ok(r.ok);if(r.result)results.push(r.result);}
  assert.deepEqual(results,['victory']);assert.equal(s.week,12);assert.equal(s.finished,true);
});
test('shortages have consequences without negative stocks',()=>{
  const s=G.createState();s.gold=0;s.food=0;s.troops=12000;G.nextWeek(s);
  assert.equal(s.gold,0);assert.equal(s.food,0);assert.ok(s.troops<12000);assert.ok(s.regions.provence.unrest>52);
});
test('long campaigns are deterministic, bounded and saveable',()=>{
  function run(){let s=G.createState();for(let i=0;i<200;i++){if(s.pendingEvent)G.chooseEvent(s,'negotiate');G.action(s,'bread',G.regions[i%12].id);G.nextWeek(s);s=G.deserialize(G.serialize(s));}return s;}
  const a=run();assert.deepEqual(a,run());assert.equal(a.week,200);assert.ok(a.journal.length<=30);assert.ok(a.gold>=0&&a.food>=0&&a.troops>=0);
});
