(function (root, factory) {
  'use strict';
  const core = factory();
  if (typeof module === 'object' && module.exports) module.exports = core;
  if (root) root.GameCore = core;
})(typeof globalThis !== 'undefined' ? globalThis : this, function () {
  'use strict';
  // Regions and every numeric value below are deliberately simplified game data.
  const regions = [
    ['brittany', 'Бретань', 'Bretagne', 'Ренн',185,288,800000,24,18,['normandy','orleans','poitou'], 'Порты и сельские общины западного побережья.'],
    ['normandy','Нормандия','Normandie','Руан',323,240,1000000,32,20,['brittany','picardy','ile','orleans'],'Торговые города и плодородные земли у Ла-Манша.'],
    ['picardy','Пикардия','Picardie','Амьен',449,173,700000,23,17,['normandy','ile','champagne'],'Северные дороги связывают столицу с приграничьем.'],
    ['ile','Иль-де-Франс','Île-de-France','Париж',439,267,1300000,48,8,['normandy','picardy','champagne','burgundy','orleans'],'Столица, казна и центр политической борьбы.'],
    ['champagne','Шампань','Champagne','Реймс',534,252,600000,25,15,['picardy','ile','lorraine','burgundy'],'На восточных дорогах нарастает сопротивление властям.'],
    ['lorraine','Лотарингия','Lorraine','Нанси',618,254,650000,24,12,['champagne','burgundy'],'Восточная граница и ремесленные центры.'],
    ['burgundy','Бургундия','Bourgogne','Дижон',548,367,800000,29,16,['ile','champagne','lorraine','orleans','languedoc','provence'],'Внутренние торговые пути соединяют север и юг.'],
    ['orleans','Орлеане','Orléanais','Орлеан',391,353,650000,23,21,['brittany','normandy','ile','burgundy','poitou','guyenne'],'Зерно и переправы через Луару питают столицу.'],
    ['poitou','Пуату','Poitou','Пуатье',304,409,600000,19,19,['brittany','orleans','guyenne'],'Сельская область с сильными местными интересами.'],
    ['guyenne','Гиень','Guyenne','Бордо',349,511,950000,32,16,['poitou','orleans','languedoc'],'Атлантическая торговля приносит доход юго-западу.'],
    ['languedoc','Лангедок','Languedoc','Тулуза',474,543,1000000,28,18,['guyenne','burgundy','provence'],'Южные города и земледельческие районы.'],
    ['provence','Прованс','Provence','Марсель',588,517,800000,33,11,['burgundy','languedoc'],'Средиземноморские порты и торговые связи.']
  ].map((r,i) => Object.freeze({id:r[0],name:r[1],mapName:r[2],city:r[3],x:r[4],y:r[5],population:r[6],income:r[7],grain:r[8],neighbors:Object.freeze(r[9]),description:r[10],color:['#ab9970','#82999b','#8e9a86','#c4a36c','#ad897b','#9d927f','#a7a27a','#b1ab83','#899975','#8b9f93','#b99b77','#bc987f'][i]}));
  Object.freeze(regions);
  const byId = Object.freeze(Object.assign(Object.create(null),Object.fromEntries(regions.map(r => [r.id,r]))));
  const clamp = (v,min,max) => Math.min(max,Math.max(min,v));
  const reply = (ok,message,extra) => Object.assign({ok,message},extra);
  function log(s,text) { s.journal.unshift({week:s.week,text}); s.journal = s.journal.slice(0,30); }
  function event() { return {id:'grain-petition',title:'Хлеб и влияние',copy:'Игровое событие: парижские представители требуют дополнительных поставок хлеба. Ваш ответ определит, чьей поддержкой вы заручитесь.',choices:[{id:'relief',label:'Открыть запасы',description:'−60 продовольствия; поддержка народа +15, Собрания +5; волнения во всех областях −8.'},{id:'negotiate',label:'Договориться с Собранием',description:'Поддержка Собрания +12, короны −8; волнения в столице −10.'},{id:'refuse',label:'Сохранить резервы',description:'Поддержка короны +8, народа −10; волнения во всех областях +5.'}]}; }
  function createState() {
    const unrest = [38,30,42,48,69,47,33,27,41,35,44,52];
    return {version:1,week:0,gold:840,food:360,troops:1200,army:'ile',moves:2,selected:'ile',support:{crown:65,assembly:45,people:35},regions:Object.fromEntries(regions.map((r,i)=>[r.id,{unrest:unrest[i],used:{}}])),journal:[{week:0,text:'5 мая 1789. Генеральные штаты открылись в Версале. Начинается ваша борьба за власть.'}],pendingEvent:null,finished:false,seed:1789,resolvedBattles:[]};
  }
  function averageUnrest(s) { return Math.round(regions.reduce((sum,r)=>sum+s.regions[r.id].unrest,0)/regions.length); }
  // Weekly tax: base income × (1 − unrest/150) × (0.75 + assembly/200).
  // Grain: base production × (1 − unrest/200). Upkeep: ceil(troops/12).
  // Consumption: civilian ration 110 + ceil(troops/30).
  function rates(s) {
    const income = Math.round(regions.reduce((n,r)=>n+r.income*(1-s.regions[r.id].unrest/150),0)*(.75+s.support.assembly/200));
    const upkeep = Math.ceil(s.troops/12), production = Math.round(regions.reduce((n,r)=>n+r.grain*(1-s.regions[r.id].unrest/200),0)), consumption=110+Math.ceil(s.troops/30);
    return {income,upkeep,netGold:income-upkeep,production,consumption,netFood:production-consumption};
  }
  function action(s,type,id) {
    if (!byId[id]) return reply(false,'Неизвестная область.');
    if (!['bread','tax','recruit'].includes(type)) return reply(false,'Неизвестный приказ.');
    const r=s.regions[id];
    if(r.used[type]) return reply(false,'Этот приказ уже выполнен здесь на этой неделе.');
    if(type==='bread' && s.food<40) return reply(false,'Нужно 40 продовольствия.');
    if(type==='recruit' && id!==s.army) return reply(false,'Набор доступен только в области вашей армии.');
    if(type==='recruit' && (s.gold<120 || s.food<20)) return reply(false,'Для набора нужно 120 казны и 20 продовольствия.');
    let message;
    if(type==='bread') { s.food-=40;r.unrest=clamp(r.unrest-15,0,100);s.support.people=clamp(s.support.people+2,0,100);message='Хлеб распределён: волнения −15.'; }
    if(type==='tax') { s.gold+=100;r.unrest=clamp(r.unrest+12,0,100);s.support.people=clamp(s.support.people-2,0,100);message='Чрезвычайный налог: казна +100, волнения +12.'; }
    if(type==='recruit') { s.gold-=120;s.food-=20;s.troops+=200;message='В армию вступили 200 новобранцев.'; }
    r.used[type]=true; log(s,byId[id].name+': '+message);return reply(true,message);
  }
  function canMarch(s,to) {
    if(!byId[to]) return reply(false,'Неизвестная область.',{battle:false});
    if(s.troops<=0) return reply(false,'Для похода нужны солдаты.',{battle:false});
    if(s.moves<=0) return reply(false,'Походы на этой неделе исчерпаны.',{battle:false});
    if(!byId[s.army].neighbors.includes(to)) return reply(false,'Армия может перейти только в соседнюю область.',{battle:false});
    const battle=s.regions[to].unrest>=65;
    return reply(true,battle?'В области вооружённое сопротивление: предстоит бой.':'Путь свободен.',{battle});
  }
  function march(s,to) {
    const check=canMarch(s,to);if(!check.ok)return check;
    if(check.battle)return reply(false,'Сначала разрешите сражение.',{battle:true});
    s.army=to;s.moves--;log(s,'Армия прибыла: '+byId[to].name+'.');return reply(true,'Армия прибыла в область.');
  }
  function resolveBattle(s,to,result) {
    if(!result || typeof result.battleId!=='string' || !result.battleId.length || typeof result.won!=='boolean' || !Number.isInteger(result.casualties) || result.casualties<0 || result.casualties>s.troops) return reply(false,'Некорректный результат сражения.');
    if(s.resolvedBattles.includes(result.battleId))return reply(false,'Результат этого сражения уже применён.');
    const check=canMarch(s,to);if(!check.ok)return check;
    if(!check.battle)return reply(false,'В области нет вооружённого сопротивления.');
    if(result.battleId!==`battle-${s.week}-${s.moves}-${s.army}-${to}`)return reply(false,'Результат относится к другому походу.');
    s.troops-=result.casualties;s.moves--;s.resolvedBattles.push(result.battleId);
    s.regions[to].unrest=clamp(s.regions[to].unrest+(result.won?-30:5),0,100);
    if(result.won)s.army=to;
    const message=(result.won?'Победа':'Поражение')+' — '+byId[to].name+'. Потери: '+result.casualties+'.';log(s,message);return reply(true,message);
  }
  function chooseEvent(s,choiceId) {
    if(!s.pendingEvent)return reply(false,'Нет события для решения.');
    if(!s.pendingEvent.choices.some(c=>c.id===choiceId))return reply(false,'Неизвестное решение.');
    if(choiceId==='relief' && s.food<60)return reply(false,'Для открытия запасов нужно 60 продовольствия.');
    if(choiceId==='relief'){s.food-=60;s.support.people=clamp(s.support.people+15,0,100);s.support.assembly=clamp(s.support.assembly+5,0,100);regions.forEach(r=>s.regions[r.id].unrest=clamp(s.regions[r.id].unrest-8,0,100));}
    if(choiceId==='negotiate'){s.support.assembly=clamp(s.support.assembly+12,0,100);s.support.crown=clamp(s.support.crown-8,0,100);s.regions.ile.unrest=clamp(s.regions.ile.unrest-10,0,100);}
    if(choiceId==='refuse'){s.support.crown=clamp(s.support.crown+8,0,100);s.support.people=clamp(s.support.people-10,0,100);regions.forEach(r=>s.regions[r.id].unrest=clamp(s.regions[r.id].unrest+5,0,100));}
    const message='Решение: '+s.pendingEvent.choices.find(c=>c.id===choiceId).label+'.';s.pendingEvent=null;log(s,message);return reply(true,message);
  }
  function nextWeek(s) {
    if(s.pendingEvent)return reply(false,'Сначала примите решение по текущему событию.',{result:null});
    const r=rates(s), hunger=s.food+r.netFood<0, unpaid=s.gold+r.netGold<0;
    s.gold=Math.max(0,s.gold+r.netGold);s.food=Math.max(0,s.food+r.netFood);s.week++;s.moves=2;
    if(hunger||unpaid)s.troops=Math.max(0,s.troops-Math.ceil(s.troops*(hunger?.08:.04)));
    // Low popular support increases unrest; the army's presence reduces it locally.
    regions.forEach(region=>{const p=s.regions[region.id];p.used={};p.unrest=clamp(p.unrest+(s.support.people<40?2:s.support.people>=60?-1:0)+(hunger?8:0)+(unpaid?4:0)-(region.id===s.army&&s.troops>0?3:0),0,100);});
    if(s.week===2)s.pendingEvent=event();
    let result=null;
    if(s.week>=8&&!s.finished){result=s.gold>0&&s.troops>0&&averageUnrest(s)<55?'victory':'defeat';s.finished=true;}
    const message='Неделя '+s.week+': доход '+r.income+', содержание '+r.upkeep+'.'+(hunger?' Нехватка хлеба: волнения и дезертирство.':'')+(unpaid?' Жалование не выплачено.':'')+(result?(result==='victory'?' Восьминедельный сценарий пройден.':' Восьминедельный сценарий проигран.')+' Можно продолжить свободную игру.':'');
    log(s,message);return reply(true,message,{result});
  }
  function deserialize(text) {
    if(typeof text!=='string'||text.length>2000000)throw new Error('Некорректный размер сохранения.');
    let s;try{s=JSON.parse(text);}catch(_){throw new Error('Сохранение не является JSON.');}
    const obj=v=>v!==null&&typeof v==='object'&&!Array.isArray(v);
    const num=(v,min,max)=>Number.isSafeInteger(v)&&v>=min&&v<=max;
    const fail=()=>{throw new Error('Сохранение повреждено или имеет несовместимую версию.');};
    if(!obj(s)||s.version!==1||!num(s.week,0,1000000)||!num(s.gold,0,Number.MAX_SAFE_INTEGER)||!num(s.food,0,Number.MAX_SAFE_INTEGER)||!num(s.troops,0,Number.MAX_SAFE_INTEGER)||!num(s.moves,0,2)||!num(s.seed,0,4294967295)||!byId[s.army]||!byId[s.selected]||typeof s.finished!=='boolean')fail();
    if(!obj(s.support)||!['crown','assembly','people'].every(k=>num(s.support[k],0,100)))fail();
    if(!obj(s.regions)||Object.keys(s.regions).length!==regions.length)fail();
    for(const region of regions){const r=s.regions[region.id];if(!obj(r)||!num(r.unrest,0,100)||!obj(r.used)||!Object.keys(r.used).every(k=>['bread','tax','recruit'].includes(k)&&r.used[k]===true))fail();}
    if(!Array.isArray(s.journal)||s.journal.length>30||!s.journal.every(j=>obj(j)&&num(j.week,0,s.week)&&typeof j.text==='string'&&j.text.length<=2000))fail();
    if(!Array.isArray(s.resolvedBattles)||new Set(s.resolvedBattles).size!==s.resolvedBattles.length||!s.resolvedBattles.every(id=>{
      if(typeof id!=='string')return false;const match=/^battle-(\d+)-([12])-([a-z]+)-([a-z]+)$/.exec(id);
      return !!match&&num(Number(match[1]),0,s.week)&&!!byId[match[3]]&&byId[match[3]].neighbors.includes(match[4]);
    }))fail();
    if(s.pendingEvent!==null){if(!obj(s.pendingEvent)||JSON.stringify(s.pendingEvent)!==JSON.stringify(event())||s.week!==2)fail();}
    if((s.week<8&&s.finished)||(s.week>=8&&!s.finished))fail();
    return s;
  }
  function serialize(s){const text=JSON.stringify(s);deserialize(text);return text;}
  return {regions,createState,rates,averageUnrest,action,canMarch,march,resolveBattle,nextWeek,chooseEvent,serialize,deserialize};
});
