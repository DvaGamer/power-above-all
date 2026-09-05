/* Standalone tactical sketch. All combat advances in fixed simulation steps. */
(() => {
  'use strict';
  const byId = id => document.getElementById(id);
  const dialog = byId('battle-dialog');
  const canvas = byId('battle-canvas');
  const ctx = canvas.getContext('2d');
  const pauseButton = byId('battle-pause');
  const volleyButton = byId('battle-volley');
  const retreatButton = byId('battle-retreat');
  const resultBox = byId('battle-result');
  const STEP = 1 / 30;
  const WIDTH = canvas.width;
  const HEIGHT = canvas.height;
  let current = null;
  let frameId = 0;
  const clamp = (value, min, max) => Math.max(min, Math.min(max, value));
  const distance = (a, b) => Math.hypot(a.x - b.x, a.y - b.y);

  function generator(seed) {
    let value = Number(seed) >>> 0;
    return () => {
      value += 0x6D2B79F5;
      let t = value;
      t = Math.imul(t ^ t >>> 15, t | 1);
      t ^= t + Math.imul(t ^ t >>> 7, t | 61);
      return ((t ^ t >>> 14) >>> 0) / 4294967296;
    };
  }

  function units(count, side, rng) {
    return Array.from({ length: count }, (_, index) => {
      const row = index % 6;
      const column = Math.floor(index / 6);
      return {
        x: side === 'ally' ? 188 + column * 22 : 697 - column * 22,
        y: HEIGHT / 2 + (row - 2.5) * 25,
        slotX: (column - (Math.ceil(count / 6) - 1) / 2) * 22,
        slotY: (row - 2.5) * 25,
        hp: 1, side, cooldown: 0.2 + rng() * 1.4, angle: side === 'ally' ? 0 : Math.PI
      };
    });
  }

  function nearest(unit, opponents) {
    let target = null;
    let best = Infinity;
    for (const candidate of opponents) {
      if (candidate.hp <= 0) continue;
      const d = distance(unit, candidate);
      if (d < best) { best = d; target = candidate; }
    }
    return { target, distance: best };
  }

  function fire(state, unit, target, bonus = false) {
    unit.angle = Math.atan2(target.y - unit.y, target.x - unit.x);
    state.shots.push({ x: unit.x, y: unit.y, tx: target.x, ty: target.y, age: 0, side: unit.side });
    if (state.rng() < (bonus ? 0.95 : 0.76)) {
      target.hp = Math.max(0, target.hp - (bonus ? 0.34 : unit.side === 'ally' ? 0.25 : 0.22));
    }
    unit.cooldown = 1.6 + state.rng() * 0.6;
  }

  function advanceUnit(unit, destination, speed, dt) {
    const d = distance(unit, destination);
    if (d < 2) return;
    const step = Math.min(d, speed * dt);
    unit.angle = Math.atan2(destination.y - unit.y, destination.x - unit.x);
    unit.x = clamp(unit.x + (destination.x - unit.x) / d * step, 24, WIDTH - 24);
    unit.y = clamp(unit.y + (destination.y - unit.y) / d * step, 27, HEIGHT - 27);
  }

  function update(state, dt) {
    state.elapsed += dt;
    state.volleyCooldown = Math.max(0, state.volleyCooldown - dt);
    for (const unit of [...state.allies, ...state.enemies]) {
      if (unit.hp <= 0) continue;
      unit.cooldown = Math.max(0, unit.cooldown - dt);
      const found = nearest(unit, unit.side === 'ally' ? state.enemies : state.allies);
      if (!found.target) continue;
      if (unit.side === 'ally') {
        advanceUnit(unit, { x: state.order.x + unit.slotX, y: state.order.y + unit.slotY }, 43, dt);
      } else if (found.distance > 150) {
        advanceUnit(unit, found.target, 31, dt);
      }
      const range = unit.side === 'ally' ? 182 : 165;
      if (distance(unit, found.target) <= range && unit.cooldown <= 0) fire(state, unit, found.target);
    }
    state.shots.forEach(shot => { shot.age += dt; });
    state.shots = state.shots.filter(shot => shot.age < 0.8);
    updateControls(state);
    const alive = state.allies.some(unit => unit.hp > 0);
    const enemiesAlive = state.enemies.some(unit => unit.hp > 0);
    if (!alive || !enemiesAlive) finish(alive && !enemiesAlive, false);
  }

  function updateControls(state) {
    byId('ally-count').textContent = state.allies.filter(unit => unit.hp > 0).length;
    byId('enemy-count').textContent = state.enemies.filter(unit => unit.hp > 0).length;
    if (state.ended) return;
    byId('battle-state').textContent = state.paused ? 'Пауза' : `Бой · ${Math.floor(state.elapsed)} с`;
    pauseButton.textContent = state.paused ? 'Продолжить' : 'Пауза';
    volleyButton.disabled = state.paused || state.volleyCooldown > 0;
    volleyButton.textContent = state.volleyCooldown > 0 ? `Залп · ${Math.ceil(state.volleyCooldown)} с` : 'Прицельный залп';
  }

  function drawTerrain(state) {
    ctx.fillStyle = '#b4bb8b';
    ctx.fillRect(0, 0, WIDTH, HEIGHT);
    for (const field of state.fields) {
      ctx.fillStyle = field.color;
      ctx.fillRect(field.x, field.y, field.w, field.h);
      ctx.strokeStyle = 'rgba(78,89,51,.10)';
      ctx.lineWidth = 1;
      for (let y = field.y + 9; y < field.y + field.h; y += 12) {
        ctx.beginPath(); ctx.moveTo(field.x, y); ctx.lineTo(field.x + field.w, y); ctx.stroke();
      }
    }
    ctx.strokeStyle = '#c9bf98'; ctx.lineWidth = 28;
    ctx.beginPath(); ctx.moveTo(360, 0); ctx.bezierCurveTo(460, 130, 428, 290, 528, HEIGHT); ctx.stroke();
    ctx.strokeStyle = 'rgba(104,98,64,.17)'; ctx.lineWidth = 2;
    ctx.beginPath(); ctx.moveTo(353, 0); ctx.bezierCurveTo(452, 130, 420, 290, 520, HEIGHT); ctx.stroke();
    for (const tree of state.trees) {
      ctx.fillStyle = 'rgba(36,55,35,.16)';
      ctx.beginPath(); ctx.ellipse(tree.x + 5, tree.y + 5, tree.r, tree.r * 0.8, 0, 0, Math.PI * 2); ctx.fill();
      ctx.fillStyle = tree.color;
      ctx.beginPath(); ctx.arc(tree.x, tree.y, tree.r, 0, Math.PI * 2); ctx.fill();
      ctx.fillStyle = 'rgba(213,217,157,.15)';
      ctx.beginPath(); ctx.arc(tree.x - 2, tree.y - 3, tree.r * 0.55, 0, Math.PI * 2); ctx.fill();
    }
    ctx.fillStyle = 'rgba(39,50,37,.5)'; ctx.font = '11px sans-serif';
    ctx.fillText('КОРОЛЕВСКАЯ АРМИЯ', 30, 26);
    ctx.textAlign = 'right'; ctx.fillText('ПОВСТАНЦЫ', WIDTH - 30, 26); ctx.textAlign = 'left';
  }

  function render(state) {
    drawTerrain(state);
    if (!state.ended) {
      ctx.save(); ctx.translate(state.order.x, state.order.y);
      ctx.strokeStyle = 'rgba(39,78,106,.6)'; ctx.lineWidth = 1.5; ctx.setLineDash([5, 5]);
      ctx.strokeRect(-49, -86, 98, 172); ctx.setLineDash([]);
      ctx.beginPath(); ctx.moveTo(-8, 0); ctx.lineTo(8, 0); ctx.moveTo(0, -8); ctx.lineTo(0, 8); ctx.stroke(); ctx.restore();
    }
    const soldiers = [...state.allies, ...state.enemies].sort((a, b) => a.y - b.y);
    for (const unit of soldiers) {
      ctx.save(); ctx.translate(unit.x, unit.y);
      if (unit.hp <= 0) {
        ctx.globalAlpha = 0.28; ctx.fillStyle = '#554d3c';
        ctx.fillRect(-5, -2, 10, 4); ctx.restore(); continue;
      }
      if (unit.side === 'ally') {
        ctx.strokeStyle = 'rgba(192,223,241,.7)'; ctx.lineWidth = 1.2;
        ctx.beginPath(); ctx.ellipse(0, 3, 9, 5, 0, 0, Math.PI * 2); ctx.stroke();
      }
      ctx.fillStyle = 'rgba(35,43,25,.23)'; ctx.beginPath(); ctx.ellipse(2, 4, 6, 4, 0, 0, Math.PI * 2); ctx.fill();
      ctx.save(); ctx.rotate(unit.angle);
      ctx.strokeStyle = '#504533'; ctx.lineWidth = 2; ctx.beginPath(); ctx.moveTo(1, 3); ctx.lineTo(12, 3); ctx.stroke();
      ctx.fillStyle = unit.side === 'ally' ? '#2c587e' : '#a54f40'; ctx.fillRect(-5, -4, 9, 8);
      ctx.strokeStyle = '#e9debd'; ctx.lineWidth = 1; ctx.beginPath(); ctx.moveTo(-4, -3); ctx.lineTo(3, 3); ctx.stroke();
      ctx.fillStyle = '#dcc39d'; ctx.beginPath(); ctx.arc(0, 0, 3, 0, Math.PI * 2); ctx.fill();
      ctx.fillStyle = '#292d29'; ctx.beginPath(); ctx.moveTo(-5, -3); ctx.lineTo(5, -3); ctx.lineTo(0, 3); ctx.closePath(); ctx.fill(); ctx.restore();
      ctx.fillStyle = 'rgba(36,45,31,.5)'; ctx.fillRect(-6, -13, 12, 2);
      ctx.fillStyle = unit.side === 'ally' ? '#c9e1e3' : '#efc1a0'; ctx.fillRect(-6, -13, 12 * unit.hp, 2);
      ctx.restore();
    }
    for (const shot of state.shots) {
      if (shot.age < 0.12) {
        ctx.strokeStyle = `rgba(255,239,175,${1 - shot.age / 0.12})`; ctx.lineWidth = 1.5;
        ctx.beginPath(); ctx.moveTo(shot.x, shot.y); ctx.lineTo(shot.tx, shot.ty); ctx.stroke();
      }
      ctx.fillStyle = `rgba(239,235,214,${0.45 * (1 - shot.age / 0.8)})`;
      ctx.beginPath(); ctx.arc(shot.x + shot.age * 9, shot.y - shot.age * 8, 3 + shot.age * 13, 0, Math.PI * 2); ctx.fill();
    }
    if (state.paused && !state.ended) {
      ctx.fillStyle = 'rgba(24,36,31,.25)'; ctx.fillRect(0, 0, WIDTH, HEIGHT);
      ctx.fillStyle = '#fff5db'; ctx.font = '26px Georgia'; ctx.textAlign = 'center';
      ctx.fillText('ПАУЗА', WIDTH / 2, HEIGHT / 2); ctx.textAlign = 'left';
    }
  }

  function tick(timestamp) {
    const state = current;
    if (!state || state.ended) { frameId = 0; return; }
    const delta = state.previous === null ? 0 : Math.min(0.1, (timestamp - state.previous) / 1000);
    state.previous = timestamp;
    if (!state.paused && !document.hidden) {
      state.accumulator = Math.min(state.accumulator + delta, STEP * 3);
      let steps = 0;
      while (state.accumulator >= STEP && steps++ < 3 && !state.ended) {
        update(state, STEP); state.accumulator -= STEP;
      }
    } else state.accumulator = 0;
    render(state);
    if (!state.ended) frameId = requestAnimationFrame(tick);
    else frameId = 0;
  }

  function finish(won, retreat) {
    const state = current;
    if (!state || state.ended) return;
    state.ended = true;
    const dead = state.allies.filter(unit => unit.hp <= 0).length;
    const deadRatio = dead / state.allies.length;
    const penalty = retreat ? (1 - deadRatio) * 0.08 : 0;
    const casualties = clamp(Math.round(state.troops * (deadRatio + penalty)), 0, state.troops);
    state.outcome = { won, casualties };
    cancelAnimationFrame(frameId); frameId = 0;
    pauseButton.disabled = true; volleyButton.disabled = true; retreatButton.disabled = true;
    byId('battle-state').textContent = won ? 'Победа' : retreat ? 'Отступление' : 'Поражение';
    const card = document.createElement('div'); card.className = 'battle-result-card';
    const title = document.createElement('h3'); title.textContent = won ? 'Поле осталось за вами' : retreat ? 'Армия отступает' : 'Армия разбита';
    const summary = document.createElement('p');
    summary.textContent = `Потери армии: ${casualties.toLocaleString('ru-RU')} чел. ${won ? 'Порядок в области восстановлен.' : 'Недовольство в области усилится.'}`;
    const accept = document.createElement('button'); accept.type = 'button'; accept.className = 'primary-button';
    accept.textContent = 'Вернуться на карту'; accept.dataset.battleAccept = 'true';
    card.append(title, summary, accept); resultBox.replaceChildren(card); resultBox.hidden = false;
    byId('battle-hint').textContent = 'Один значок представляет часть армии. Потери будут учтены после возвращения на карту.';
    render(state); accept.focus();
  }

  function stop() {
    cancelAnimationFrame(frameId); frameId = 0; current = null;
    if (dialog.open) dialog.close();
  }

  function start({ regionName, troops, unrest, seed = 1789, onComplete }) {
    stop();
    const total = Math.max(0, Math.floor(Number(troops) || 0));
    const rng = generator(seed);
    const allySize = clamp(Math.ceil(total / 50), 1, 24);
    const enemySize = clamp(12 + Math.floor((Number(unrest) || 0) / 13), 12, 20);
    current = {
      troops: total, onComplete, rng,
      allies: units(allySize, 'ally', rng), enemies: units(enemySize, 'enemy', rng),
      order: { x: 220, y: HEIGHT / 2 }, shots: [], fields: [], trees: [],
      paused: false, ended: false, accepted: false, elapsed: 0, volleyCooldown: 0, accumulator: 0, previous: null
    };
    // Decoration uses a separate generator so scenery never changes combat rolls.
    const scenery = generator((Number(seed) || 1789) + 997);
    for (let row = 0; row < 3; row++) {
      for (let col = 0; col < 5; col++) current.fields.push({
        x: col * 184 + 4, y: row * 151 + 3, w: 175, h: 140,
        color: ['#b9be8f', '#acb583', '#c3c394', '#adb790'][Math.floor(scenery() * 4)]
      });
    }
    for (let i = 0; i < 58; i++) current.trees.push({
      x: scenery() * WIDTH, y: i % 2 ? 44 + scenery() * 22 : HEIGHT - 42 + scenery() * 20,
      r: 6 + scenery() * 10, color: scenery() > 0.5 ? '#71825a' : '#7f8f65'
    });
    byId('battle-title').textContent = `Стычка: ${regionName || 'область'}`;
    resultBox.hidden = true; resultBox.replaceChildren();
    pauseButton.disabled = false; retreatButton.disabled = false; volleyButton.disabled = false;
    byId('battle-hint').textContent = 'Щёлкните по полю — синие солдаты займут позицию. Стрельба автоматическая; стрелки — движение, пробел — пауза.';
    updateControls(current); render(current); dialog.showModal(); canvas.focus();
    if (total === 0) finish(false, false);
    else frameId = requestAnimationFrame(tick);
  }

  function togglePause() {
    if (!current || current.ended) return;
    current.paused = !current.paused; current.previous = null; current.accumulator = 0;
    updateControls(current); render(current);
  }
  function command(x, y) {
    if (!current || current.ended) return;
    current.order = { x: clamp(x, 70, WIDTH - 70), y: clamp(y, 95, HEIGHT - 95) };
  }
  canvas.addEventListener('click', event => {
    const rect = canvas.getBoundingClientRect();
    command((event.clientX - rect.left) * WIDTH / rect.width, (event.clientY - rect.top) * HEIGHT / rect.height);
    canvas.focus();
  });
  canvas.addEventListener('keydown', event => {
    if (!current || current.ended) return;
    if (event.code === 'Space') { event.preventDefault(); if (!event.repeat) togglePause(); return; }
    const directions = { ArrowLeft: [-24, 0], ArrowRight: [24, 0], ArrowUp: [0, -24], ArrowDown: [0, 24] };
    const direction = directions[event.key];
    if (direction) { event.preventDefault(); command(current.order.x + direction[0], current.order.y + direction[1]); }
  });
  pauseButton.addEventListener('click', togglePause);
  retreatButton.addEventListener('click', () => finish(false, true));
  volleyButton.addEventListener('click', () => {
    const state = current;
    if (!state || state.ended || state.paused || state.volleyCooldown > 0) return;
    let fired = false;
    for (const unit of state.allies) {
      if (unit.hp <= 0) continue;
      const found = nearest(unit, state.enemies);
      if (found.target && found.distance <= 200) { fire(state, unit, found.target, true); fired = true; }
    }
    if (fired) {
      state.volleyCooldown = 6; updateControls(state);
      if (!state.enemies.some(unit => unit.hp > 0)) finish(true, false);
    } else byId('battle-hint').textContent = 'Противник вне дальности залпа. Подведите синие отряды ближе; залп не потрачен.';
  });
  resultBox.addEventListener('click', event => {
    if (!event.target.closest('[data-battle-accept]') || !current || !current.ended || current.accepted) return;
    const state = current; state.accepted = true;
    const callback = state.onComplete; const outcome = state.outcome;
    stop();
    if (typeof callback === 'function') callback(outcome);
  });
  dialog.addEventListener('cancel', event => {
    event.preventDefault();
    if (current && !current.ended && !current.paused) togglePause();
  });
  document.addEventListener('visibilitychange', () => {
    if (current) { current.previous = null; current.accumulator = 0; }
  });
  window.Battle = { start, stop };
})();
