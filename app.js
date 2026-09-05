'use strict';
(() => {
  const core = window.GameCore;
  const $ = id => document.getElementById(id);
  const number = value => Math.round(value).toLocaleString('ru-RU');
  const signed = value => `${value >= 0 ? '+' : '−'}${number(Math.abs(value))}`;
  const escape = value => String(value).replace(/[&<>"']/g, char => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[char]));
  const AUTO_KEY = 'power-above-all.autosave.v1';
  const SAVE_KEY = 'power-above-all.manual.v1';
  const svgNS = 'http://www.w3.org/2000/svg';
  let state = core.createState();
  let layer = 'political';
  let tab = 'map';
  let toastTimer;
  let battleActive = false;
  let storageFailed = false;
  let restored = false;
  try {
    const saved = localStorage.getItem(AUTO_KEY);
    if (saved) { state = core.deserialize(saved); restored = true; }
  } catch { storageFailed = true; }

  function notify(text) {
    $('toast').textContent = text;
    $('toast').classList.add('visible');
    clearTimeout(toastTimer);
    toastTimer = setTimeout(() => $('toast').classList.remove('visible'), 4800);
  }
  function persist() {
    try { localStorage.setItem(AUTO_KEY, core.serialize(state)); storageFailed = false; }
    catch { storageFailed = true; }
  }
  function campaignDate(week = state.week) {
    const date = new Date(Date.UTC(1789, 4, 5));
    date.setUTCDate(date.getUTCDate() + week * 7);
    return date;
  }
  function dateLabel(week = state.week, year = true) {
    return new Intl.DateTimeFormat('ru-RU', { day: 'numeric', month: 'long', ...(year ? { year: 'numeric' } : {}), timeZone: 'UTC' }).format(campaignDate(week)).replace(/\s*г\.$/, '');
  }
  function svg(tag, attributes = {}, text) {
    const element = document.createElementNS(svgNS, tag);
    for (const [key, value] of Object.entries(attributes)) element.setAttribute(key, value);
    if (text !== undefined) element.textContent = text;
    return element;
  }
  function voronoiCell(region) {
    let polygon = [[0, 0], [900, 0], [900, 780], [0, 780]];
    for (const other of core.regions) {
      if (region.id === other.id) continue;
      const a = other.x - region.x;
      const b = other.y - region.y;
      const c = (other.x ** 2 + other.y ** 2 - region.x ** 2 - region.y ** 2) / 2;
      const output = [];
      for (let i = 0; i < polygon.length; i++) {
        const start = polygon[i], end = polygon[(i + 1) % polygon.length];
        const ds = a * start[0] + b * start[1] - c;
        const de = a * end[0] + b * end[1] - c;
        if (ds <= 0) output.push(start);
        if ((ds <= 0) !== (de <= 0)) {
          const t = ds / (ds - de);
          output.push([start[0] + t * (end[0] - start[0]), start[1] + t * (end[1] - start[1])]);
        }
      }
      polygon = output;
    }
    return `M${polygon.map(point => point.map(n => n.toFixed(1)).join(',')).join('L')}Z`;
  }
  function chooseRegion(id) {
    if (battleActive) return;
    state.selected = id;
    if (tab !== 'map') switchTab('map');
    render();
  }
  function prepareMap() {
    for (const region of core.regions) {
      const path = svg('path', { d: voronoiCell(region), class: 'region-shape', id: `shape-${region.id}`, role: 'button', tabindex: '0', 'aria-label': `Выбрать область: ${region.name}` });
      path.addEventListener('click', () => chooseRegion(region.id));
      path.addEventListener('keydown', event => {
        if (event.key === 'Enter' || event.key === ' ') { event.preventDefault(); chooseRegion(region.id); }
      });
      $('region-shapes').append(path);
      const label = svg('g', { class: 'map-label-group', id: `label-${region.id}`, 'aria-hidden': 'true' });
      const title = svg('text', { x: region.x, y: region.y - 11, 'text-anchor': 'middle', class: 'region-label' }, region.mapName);
      label.append(title);
      label.append(svg('circle', { cx: region.x, cy: region.y + 2, r: region.id === 'ile' ? 4 : 2.7, fill: region.id === 'ile' ? '#854235' : '#505b45' }));
      label.append(svg('text', { x: region.x, y: region.y + 20, 'text-anchor': 'middle', class: 'city-label' }, region.city));
      label.addEventListener('click', () => chooseRegion(region.id));
      $('map-labels').append(label);
    }
    const mountains = [[614,392],[603,408],[620,432],[610,454],[604,475],[328,576],[346,586],[370,591],[396,600],[417,603],[438,610],[484,445],[475,468],[464,490],[496,460]];
    mountains.forEach(([x,y], index) => {
      const size = index < 5 ? 11 : 8;
      $('mountains').append(svg('path', { d: `M${x-size} ${y+size}L${x} ${y-size}L${x+size} ${y+size}M${x} ${y-size}l2 ${size+3} 4 -2`, fill: 'none', stroke: '#8c8b6e', 'stroke-width': '1.1', opacity: '.5' }));
    });
  }
  function renderMap() {
    document.querySelector('.map-legend').innerHTML = layer === 'unrest'
      ? '<span><i class="legend-calm"></i>Спокойно</span><span><i class="legend-tense"></i>Брожение</span><span><i class="legend-revolt"></i>Беспорядки</span>'
      : '<span>Щёлкните по области, чтобы отдать приказ</span>';
    for (const region of core.regions) {
      const unrest = state.regions[region.id].unrest;
      const path = $(`shape-${region.id}`);
      path.setAttribute('fill', layer === 'unrest' ? (unrest >= 65 ? '#b9785d' : unrest >= 40 ? '#cfb67a' : '#a8b78b') : region.color);
      path.classList.toggle('selected', state.selected === region.id);
      path.setAttribute('aria-pressed', String(state.selected === region.id));
      $(`label-${region.id}`).classList.toggle('selected', state.selected === region.id);
    }
    const region = core.regions.find(r => r.id === state.army);
    const target = core.regions.find(r => r.id === state.selected);
    const marker = $('army-marker');
    marker.replaceChildren();
    if (target.id !== region.id && region.neighbors.includes(target.id)) {
      marker.append(svg('path', { d: `M${region.x + 26} ${region.y+26}Q${(region.x+target.x)/2+15} ${(region.y+target.y)/2-20} ${target.x} ${target.y+27}`, fill: 'none', stroke: '#465c60', 'stroke-width': '2', 'stroke-dasharray': '5 6', opacity: '.8' }));
    }
    const token = svg('g', { transform: `translate(${region.x + 35},${region.y + 32})`, class: 'army-token' });
    token.append(svg('ellipse', { cx: '0', cy: '14', rx: '26', ry: '8', fill: '#465749', opacity: '.16' }));
    token.append(svg('path', { d: 'M-18-13H18V10L0 21-18 10Z', fill: '#2f4b5a', stroke: '#eee4bd', 'stroke-width': '2' }));
    token.append(svg('path', { d: 'M-7-5 8 9M7-5-8 9M-10 5l6 5m8 0 6-5', stroke: '#eddfb2', 'stroke-width': '1.5', fill: 'none' }));
    token.append(svg('rect', { x: '-20', y: '21', width: '40', height: '17', rx: '3', fill: '#f1e9cf', stroke: '#a8a387', 'stroke-width': '.6' }));
    token.append(svg('text', { x: '0', y: '33', 'text-anchor': 'middle', 'font-family': 'Arial,sans-serif', 'font-size': '10', fill: '#344d4b', 'font-weight': '700' }, number(state.troops)));
    marker.append(token);
  }
  function render() {
    const region = core.regions.find(r => r.id === state.selected);
    const local = state.regions[region.id];
    const rates = core.rates(state);
    const armyRegion = core.regions.find(r => r.id === state.army);
    $('gold').textContent = number(state.gold);
    $('food').textContent = number(state.food);
    $('troops').textContent = number(state.troops);
    $('unrest').textContent = number(core.averageUnrest(state));
    $('gold-rate').textContent = `${signed(rates.netGold)}/нед.`;
    $('food-rate').textContent = `${signed(rates.netFood)}/нед.`;
    $('gold-rate').classList.toggle('negative', rates.netGold < 0);
    $('food-rate').classList.toggle('negative', rates.netFood < 0);
    $('region-index').textContent = `${String(core.regions.indexOf(region) + 1).padStart(2,'0')} / 12`;
    $('region-name').textContent = region.name;
    $('region-city').textContent = `${region.city} · ${region.id === 'ile' ? 'столица королевства' : 'областной центр'}`;
    $('region-pop').textContent = `${(region.population / 1000000).toLocaleString('ru-RU', { maximumFractionDigits: 1 })} млн`;
    $('region-income').textContent = `${region.income} ливров · база`;
    $('region-grain').textContent = `${region.grain} ед. · база`;
    $('region-unrest').textContent = `${Math.round(local.unrest)} / 100`;
    $('region-unrest-bar').style.width = `${local.unrest}%`;
    $('region-description').textContent = region.description;
    $('region-status').textContent = local.unrest >= 65 ? 'Беспорядки' : local.unrest >= 40 ? 'Неспокойно' : 'Спокойно';
    $('region-status').className = `status-tag ${local.unrest >= 65 ? 'danger' : local.unrest >= 40 ? 'warning' : 'calm'}`;
    $('army-location').textContent = armyRegion.city;
    $('army-caption').textContent = `${number(state.troops)} солдат · переходов осталось: ${state.moves}`;
    const check = core.canMarch(state, region.id);
    $('march').disabled = !check.ok;
    $('march').textContent = region.id === state.army ? 'Армия в этой области' : check.ok ? (check.battle ? 'Вступить в сражение' : 'Отправить армию') : (state.troops === 0 ? 'Нужны солдаты' : state.moves === 0 ? 'Дождитесь следующей недели' : 'Только в соседнюю область');
    $('march').title = check.message || '';
    $('bread').disabled = state.food < 40 || Boolean(local.used.bread);
    $('tax').disabled = Boolean(local.used.tax);
    $('recruit').disabled = state.gold < 120 || state.food < 20 || state.army !== region.id || Boolean(local.used.recruit);
    for (const type of ['bread', 'tax', 'recruit']) $(type).title = local.used[type] ? 'Распоряжение уже выполнено на этой неделе' : type === 'recruit' && region.id !== state.army ? 'Набор доступен в области с вашей армией' : '';
    $('date').textContent = dateLabel();
    const month = campaignDate().getUTCMonth();
    $('season').textContent = `${month >= 5 && month <= 7 ? 'ЛЕТО' : month >= 8 && month <= 10 ? 'ОСЕНЬ' : month === 11 || month <= 1 ? 'ЗИМА' : 'ВЕСНА'} · ХОД ${state.week + 1}`;
    $('objective-progress').style.width = `${Math.min(state.week / 8, 1) * 100}%`;
    $('objective-weeks').textContent = state.finished ? 'Сценарий завершён · свободная игра' : `${state.week} / 8 недель`;
    $('journal').replaceChildren();
    state.journal.slice(0, 5).forEach(entry => {
      const li = document.createElement('li');
      const time = document.createElement('time'); time.textContent = dateLabel(entry.week, false);
      const text = document.createElement('p'); text.textContent = entry.text;
      li.append(time, text); $('journal').append(li);
    });
    renderEvent();
    renderMap();
    if (tab !== 'map') renderOverlay();
    $('load').disabled = !hasManualSave();
  }
  function hasManualSave() { try { return !!localStorage.getItem(SAVE_KEY); } catch { return false; } }
  function renderEvent() {
    const event = state.pendingEvent;
    $('event-actions').replaceChildren();
    if (event) {
      $('event-date').textContent = 'РЕШЕНИЕ СОВЕТА · СЦЕНАРНОЕ СОБЫТИЕ';
      $('event-title').textContent = event.title;
      $('event-copy').textContent = event.copy;
      event.choices.forEach(choice => {
        const button = document.createElement('button'); button.className = 'event-choice';
        const label = document.createElement('span'); label.textContent = choice.label;
        const detail = document.createElement('small'); detail.textContent = choice.description;
        button.append(label, detail);
        button.addEventListener('click', () => {
          const result = core.chooseEvent(state, choice.id);
          notify(result.message); if (result.ok) { persist(); render(); }
        });
        $('event-actions').append(button);
      });
    } else if (state.week === 0) {
      $('event-date').textContent = '5 МАЯ · ВЕРСАЛЬ';
      $('event-title').textContent = 'Генеральные штаты';
      $('event-copy').textContent = 'В Версале собрались представители трёх сословий. Франция ждёт ответа на финансовый кризис.';
    } else {
      $('event-date').textContent = 'ДОНЕСЕНИЕ СОВЕТА';
      $('event-title').textContent = core.averageUnrest(state) >= 55 ? 'Королевство неспокойно' : 'Хрупкое равновесие';
      $('event-copy').textContent = core.averageUnrest(state) >= 55 ? 'Напряжение растёт. Откройте слой недовольства: хлеб и присутствие армии помогут удержать области.' : 'У вас есть время для решений. Следите за запасами зерна и не превращайте временные налоги в постоянную политику.';
    }
  }
  function switchTab(next) {
    tab = next;
    document.querySelectorAll('[data-tab]').forEach(button => {
      button.classList.toggle('active', button.dataset.tab === tab);
      button.setAttribute('aria-pressed', String(button.dataset.tab === tab));
    });
    $('strategy-overlay').hidden = tab === 'map';
    if (tab !== 'map') renderOverlay();
  }
  function renderOverlay() {
    const overlay = $('strategy-overlay');
    const rates = core.rates(state);
    const heading = `<div class="overlay-header"><div><div class="overlay-kicker eyebrow">КОРОЛЕВСКИЙ СОВЕТ · ${escape(dateLabel())}</div><h2>${tab === 'economy' ? 'Цена власти' : 'Равновесие сил'}</h2></div><button class="overlay-close secondary-button" id="back-to-map">К карте ↗</button></div>`;
    if (tab === 'economy') {
      overlay.innerHTML = heading + `<p class="overlay-note">Каждая армия требует хлеба. Каждый налог имеет политическую цену.</p><div class="economy-grid"><article class="economy-card"><span class="eyebrow">ДОХОДЫ</span><strong>+${number(rates.income)}</strong><p>Налоги после влияния недовольства</p></article><article class="economy-card"><span class="eyebrow">РАСХОДЫ</span><strong>−${number(rates.upkeep)}</strong><p>Содержание армии и управления</p></article><article class="economy-card"><span class="eyebrow">БАЛАНС ЗЕРНА</span><strong>${signed(rates.netFood)}</strong><p>Производство ${number(rates.production)} · потребление ${number(rates.consumption)}</p></article></div><h3>Области королевства</h3><div class="table-scroll"><table class="economy-table"><thead><tr><th>Область</th><th>Базовый налог</th><th>Зерно</th><th>Недовольство</th></tr></thead><tbody>${core.regions.map(region => `<tr><td><button class="region-select" data-region="${region.id}">${escape(region.name)}</button></td><td>${region.income}</td><td>${region.grain}</td><td>${Math.round(state.regions[region.id].unrest)}%</td></tr>`).join('')}</tbody></table></div><p class="overlay-note">Показатели — игровые условности. Во время хода налоги уменьшаются из-за недовольства; нехватка денег и хлеба усиливает кризис.</p>`;
    } else {
      const factions = [{ id: 'crown', title: 'Корона', subtitle: 'ДВОР И КОРОЛЕВСКАЯ ВЛАСТЬ', text: 'Порядок, доходы и сохранение полномочий. Чрезвычайные сборы укрепляют казну, но создают противников.', color:'#8c7246' }, { id: 'assembly', title: 'Представители сословий', subtitle: 'ДЕПУТАТЫ В ВЕРСАЛЕ', text: 'Реформы и участие в принятии решений. Политические события изменяют поддержку совета.', color:'#546f76' }, { id: 'people', title: 'Народ', subtitle: 'ГОРОДА И ДЕРЕВНИ', text: 'Доступный хлеб и облегчение повинностей. Раздача продовольствия снижает недовольство областей.', color:'#9e5c47' }];
      overlay.innerHTML = heading + `<p class="overlay-note">Власть держится на интересах, которые редко совпадают. Это первый набросок поддержки трёх политических сил.</p><div class="politics-grid">${factions.map(faction => `<article class="faction-card"><div class="eyebrow">${faction.subtitle}</div><h3>${faction.title}</h3><p>${faction.text}</p><div class="faction-support"><span>Поддержка совета</span><strong>${Math.round(state.support[faction.id])} / 100</strong></div><div class="meter"><i style="width:${state.support[faction.id]}%;background:${faction.color}"></i></div></article>`).join('')}</div><div class="objective-card"><div class="eyebrow">СЛЕДУЮЩИЕ ЭТАПЫ</div><h3>Лица за решениями</h3><p>Персонажи, министры, политические клубы и личные амбиции появятся после проверки основы кампании. Пока решения принимаются через распоряжения областям и события совета.</p></div>`;
    }
    $('back-to-map').addEventListener('click', () => switchTab('map'));
    overlay.querySelectorAll('[data-region]').forEach(button => button.addEventListener('click', () => chooseRegion(button.dataset.region)));
  }
  function startMarch() {
    if (battleActive) return;
    const to = state.selected;
    const check = core.canMarch(state, to);
    if (!check.ok) { notify(check.message); return; }
    if (!check.battle) {
      const result = core.march(state, to); notify(result.message); persist(); render(); return;
    }
    const battleId = `battle-${state.week}-${state.moves}-${state.army}-${to}`;
    persist();
    battleActive = true;
    const region = core.regions.find(r => r.id === to);
    window.Battle.start({ regionName: region.name, troops: state.troops, unrest: state.regions[to].unrest, seed: state.seed + state.week * 100 + state.moves,
      onComplete(outcome) {
        battleActive = false;
        const result = core.resolveBattle(state, to, { ...outcome, battleId });
        persist(); render(); notify(result.message);
      }
    });
  }
  for (const type of ['bread', 'tax', 'recruit']) $(type).addEventListener('click', () => {
    if (battleActive) return;
    const result = core.action(state, type, state.selected);
    notify(result.message);
    if (result.ok) { persist(); render(); }
  });
  $('march').addEventListener('click', startMarch);
  $('end-turn').addEventListener('click', () => {
    if (battleActive) return;
    const result = core.nextWeek(state);
    notify(result.message);
    if (!result.ok) { document.querySelector('.event-card').scrollIntoView({ behavior: 'smooth', block: 'nearest' }); return; }
    persist(); render();
    if (result.result) {
      $('result-title').textContent = result.result === 'victory' ? 'Порядок удержан' : 'Кризис оказался сильнее';
      $('result-copy').textContent = `Прошло восемь недель. Казна: ${number(state.gold)} ливров. Армия: ${number(state.troops)} солдат. Недовольство: ${number(core.averageUnrest(state))} из 100. ${result.result === 'victory' ? 'Совет сохранил контроль над королевством.' : 'Для победы нужны положительная казна, действующая армия и недовольство ниже 55.'} Можно продолжить свободную игру или начать заново.`;
      $('result-dialog').showModal();
    }
  });
  document.querySelectorAll('[data-tab]').forEach(button => button.addEventListener('click', () => switchTab(button.dataset.tab)));
  document.querySelectorAll('[data-layer]').forEach(button => button.addEventListener('click', () => {
    layer = button.dataset.layer;
    document.querySelectorAll('[data-layer]').forEach(control => { control.classList.toggle('active', control.dataset.layer === layer); control.setAttribute('aria-pressed', String(control.dataset.layer === layer)); });
    renderMap();
  }));
  document.querySelectorAll('[data-close]').forEach(button => button.addEventListener('click', () => $(button.dataset.close).close()));
  $('help').addEventListener('click', () => $('info-dialog').showModal());
  $('restart').addEventListener('click', () => $('confirm-dialog').showModal());
  $('confirm-restart').addEventListener('click', () => {
    window.Battle.stop(); battleActive = false; state = core.createState();
    try { localStorage.removeItem(SAVE_KEY); } catch { /* storage status is reported by persist */ }
    persist(); switchTab('map'); render(); $('confirm-dialog').close(); notify('Новая кампания. Франция, 5 мая 1789 года.');
  });
  $('save').addEventListener('click', () => {
    try { localStorage.setItem(SAVE_KEY, core.serialize(state)); persist(); render(); notify('Кампания сохранена в этом браузере.'); }
    catch { notify('Браузер не разрешил сохранение. Попробуйте запуск через локальный сервер.'); }
  });
  $('load').addEventListener('click', () => {
    try {
      const saved = localStorage.getItem(SAVE_KEY);
      if (!saved) { notify('Сначала сохраните кампанию.'); return; }
      const loaded = core.deserialize(saved);
      window.Battle.stop(); battleActive = false; state = loaded; persist(); render(); notify('Ручное сохранение загружено.');
    } catch { notify('Сохранение повреждено или несовместимо. Текущая кампания сохранена без изменений.'); }
  });
  prepareMap(); render();
  window.PowerAboveAll = { getState: () => JSON.parse(core.serialize(state)), version: '0.1.0' };
  if (restored) notify('Кампания восстановлена. Королевство ждёт ваших решений.');
  else if (storageFailed) notify('Автосохранение недоступно или повреждено. Начата новая кампания.');
})();
