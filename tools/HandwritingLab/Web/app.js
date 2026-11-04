'use strict';
const $ = id => document.getElementById(id);
const board = $('board');
const context = board.getContext('2d');
const state = { catalog: null, target: null, active: 0, drawings: [], stroke: null, pointer: null,
  result: null, revision: 0, request: null, busy: false, source: 'handwritten', pointerTypes: new Set() };
const issues = {
  MissingDrawing: 'Chưa có nét viết.', WrongStrokeCount: 'Chưa đúng số nét.', DrawingTooSmall: 'Chữ quá nhỏ để chấm ổn định.',
  WrongStrokeOrder: 'Thứ tự các nét chưa đúng.', WrongDirection: 'Nét đang đi ngược chiều.',
  StrokeTooShort: 'Nét quá ngắn.', StrokeTooLong: 'Đường nét quá dài hoặc có nét thừa.', WrongShape: 'Hình hoặc vị trí tương đối của nét chưa khớp.'
};
const templates = () => state.target.characterIds.map(id => state.catalog.dataset.characters.find(x => x.id === id));
const message = (text, error = false) => { $('connection').textContent = text; $('connection').classList.toggle('error', error); };
const clone = value => JSON.parse(JSON.stringify(value));

function invalidate() {
  state.revision++; state.request?.abort(); state.request = null; state.busy = false; state.result = null;
  $('grade').disabled = false; $('grade').textContent = 'Chấm bài ↗'; renderFeedback();
}

function selectTarget(target) {
  if (state.pointer !== null) return;
  invalidate(); state.target = target; state.active = 0; state.drawings = target.characterIds.map(() => []);
  state.source = 'handwritten'; state.pointerTypes.clear(); $('human-label').value = 'unknown';
  document.querySelectorAll('.target').forEach(button => button.classList.toggle('active', button.dataset.id === target.id));
  renderTabs(); renderBoard(); message('Viết tự nhiên trong ô. Nét và vị trí có sai số cho phép.');
}

function renderTabs() {
  const parent = $('character-tabs'); parent.replaceChildren();
  templates().forEach((template, index) => {
    const button = document.createElement('button'); button.type = 'button'; button.className = 'character-tab';
    button.textContent = `${index + 1}. ${template.symbol}`;
    button.classList.toggle('active', index === state.active);
    button.setAttribute('aria-pressed', String(index === state.active));
    button.addEventListener('click', () => { if (state.pointer !== null) return; state.active = index; renderTabs(); renderBoard(); });
    parent.append(button);
  });
  $('prompt').textContent = `Viết ${templates()[state.active].symbol}`;
}

function path(points, color, width, dashed = false) {
  if (!points.length) return;
  const size = board.getBoundingClientRect().width;
  context.beginPath(); context.strokeStyle = color; context.lineWidth = width; context.lineCap = 'round'; context.lineJoin = 'round';
  context.setLineDash(dashed ? [4, 5] : []); context.moveTo(points[0].x * size, points[0].y * size);
  if (points.length === 1) context.lineTo(points[0].x * size + 0.1, points[0].y * size);
  for (const point of points.slice(1)) context.lineTo(point.x * size, point.y * size);
  context.stroke(); context.setLineDash([]);
}

function renderBoard() {
  if (!state.target) return;
  const size = board.getBoundingClientRect().width;
  const ratio = window.devicePixelRatio || 1;
  if (board.width !== Math.round(size * ratio)) { board.width = Math.round(size * ratio); board.height = board.width; }
  context.setTransform(ratio, 0, 0, ratio, 0, 0); context.clearRect(0, 0, size, size);
  context.lineWidth = 1; context.strokeStyle = '#e9eee5'; context.setLineDash([4, 6]); context.beginPath();
  context.moveTo(size / 2, 0); context.lineTo(size / 2, size); context.moveTo(0, size / 2); context.lineTo(size, size / 2); context.stroke(); context.setLineDash([]);
  if ($('show-template').checked) templates()[state.active].strokes.forEach((stroke, index) => {
    path(stroke, '#b9ccb580', 5);
    context.fillStyle = '#829c77'; context.font = '11px system-ui';
    context.fillText(String(index + 1), stroke[0].x * size - 12, stroke[0].y * size - 8);
  });
  state.drawings[state.active].forEach((stroke, index) => {
    const result = state.result?.characters[state.active]?.strokes[index];
    path(stroke, result ? (result.passed ? '#337952' : '#c26d4b') : '#274b3a', 4);
  });
  if (state.stroke) path(state.stroke, '#274b3a', 4);
  $('stroke-count').textContent = `${state.drawings[state.active].length} nét đã viết · mẫu ${templates()[state.active].strokes.length} nét`;
}

function point(event) {
  const box = board.getBoundingClientRect();
  return { x: Math.round(Math.min(1, Math.max(0, (event.clientX - box.left) / box.width)) * 1e6) / 1e6,
    y: Math.round(Math.min(1, Math.max(0, (event.clientY - box.top) / box.height)) * 1e6) / 1e6 };
}

function finishStroke(event, cancelled = false) {
  if (event.pointerId !== state.pointer) return;
  if (!cancelled) {
    state.stroke.push(point(event));
    // Keep event capture bounded while preserving first/last points. Server resamples again by arc length.
    const limit = state.catalog.policy.maximumPointsPerStroke;
    const stroke = state.stroke.length <= limit ? state.stroke : Array.from({ length: limit }, (_, i) => state.stroke[Math.round(i * (state.stroke.length - 1) / (limit - 1))]);
    state.drawings[state.active].push(stroke);
  }
  state.stroke = null; state.pointer = null; renderBoard();
}

board.addEventListener('pointerdown', event => {
  if (!state.target || state.pointer !== null || event.button !== 0) return;
  event.preventDefault(); invalidate();
  if (state.source !== 'handwritten') state.source = 'mixed';
  state.pointerTypes.add(event.pointerType || 'mouse');
  state.pointer = event.pointerId; state.stroke = [point(event)]; board.setPointerCapture(event.pointerId); board.focus(); renderBoard();
});
board.addEventListener('pointermove', event => {
  if (event.pointerId !== state.pointer) return;
  event.preventDefault();
  const events = event.getCoalescedEvents?.() || [event];
  for (const sample of events.length ? events : [event]) {
    const next = point(sample), previous = state.stroke.at(-1);
    if (Math.hypot(next.x - previous.x, next.y - previous.y) > 0.0005) state.stroke.push(next);
  }
  if (state.stroke.length > 4096) state.stroke = state.stroke.filter((_, i) => i % 2 === 0 || i === state.stroke.length - 1);
  renderBoard();
});
board.addEventListener('pointerup', event => finishStroke(event));
board.addEventListener('pointercancel', event => finishStroke(event, true));
board.addEventListener('lostpointercapture', event => finishStroke(event, true));
$('show-template').addEventListener('change', renderBoard);
new ResizeObserver(renderBoard).observe(board.parentElement);

function undo() { if (!state.target || state.pointer !== null) return; invalidate(); state.drawings[state.active].pop(); renderBoard(); }
$('undo').addEventListener('click', undo);
$('clear').addEventListener('click', () => { if (!state.target || state.pointer !== null) return; invalidate(); state.drawings[state.active] = []; renderBoard(); });
board.addEventListener('keydown', event => { if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'z') { event.preventDefault(); undo(); } });

function renderFeedback() {
  const parent = $('feedback'); parent.replaceChildren();
  if (!state.result) {
    const mark = document.createElement('div'); mark.className = 'empty-mark'; mark.textContent = '✳';
    const title = document.createElement('h2'); title.textContent = 'Đến lượt bạn.';
    const text = document.createElement('p'); text.textContent = 'Viết xong rồi bấm “Chấm bài”. Kết quả sẽ chỉ rõ từng chữ và từng nét.';
    parent.append(mark, title, text); return;
  }
  const status = document.createElement('span'); status.className = 'result-label' + (state.result.passed ? '' : ' fail');
  status.textContent = state.result.passed ? 'Đạt mẫu nét' : 'Cần viết lại';
  const score = document.createElement('div'); score.className = 'score'; score.textContent = `${state.result.matchScore} `;
  const unit = document.createElement('small'); unit.textContent = '/ 100'; score.append(unit);
  const label = document.createElement('p'); label.textContent = 'Mức khớp hình học · mọi chữ và mọi nét phải đạt.';
  parent.append(status, score, label);
  state.result.characters.forEach(character => {
    const section = document.createElement('div'); section.className = 'character-result';
    const title = document.createElement('strong'); title.textContent = `${character.symbol} · ${character.actualStrokeCount}/${character.expectedStrokeCount} nét · ${character.passed ? 'Đạt' : 'Cần sửa'}`;
    section.append(title);
    if (character.issues.length) { const text = document.createElement('div'); text.className = 'issues'; text.textContent = character.issues.map(code => issues[code] || code).join(' '); section.append(text); }
    const list = document.createElement('ol'); list.className = 'stroke-list';
    character.strokes.forEach(stroke => {
      const item = document.createElement('li'); item.className = stroke.passed ? '' : 'bad';
      item.textContent = stroke.passed ? `Đạt · ${stroke.matchScore}/100` : stroke.issues.map(code => issues[code] || code).join(' ');
      list.append(item);
    });
    section.append(list); parent.append(section);
  });
}

$('grade').addEventListener('click', async () => {
  if (!state.target || state.pointer !== null || state.busy) return;
  const revision = state.revision;
  state.request?.abort(); const controller = new AbortController(); state.request = controller; state.busy = true;
  $('grade').disabled = true; $('grade').textContent = 'Đang chấm…';
  try {
    const response = await fetch('/api/v1/handwriting-lab/grade', { method: 'POST', signal: controller.signal,
      headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ targetId: state.target.id, characters: state.drawings }) });
    const result = await response.json();
    if (!response.ok) throw new Error(result.detail || 'Không chấm được bài viết.');
    if (revision !== state.revision) return;
    state.result = result; renderFeedback(); renderBoard(); message('Đã chấm bằng dữ liệu nét hiện tại. Bạn có thể sửa và thử lại.');
  } catch (error) { if (error.name !== 'AbortError') message(error.message, true); }
  finally { if (revision === state.revision) { state.busy = false; $('grade').disabled = false; $('grade').textContent = 'Chấm bài ↗'; } }
});

document.querySelectorAll('[data-scenario]').forEach(button => button.addEventListener('click', () => {
  if (!state.target || state.pointer !== null) return;
  invalidate(); state.source = 'generated'; state.drawings = templates().map(template => clone(template.strokes));
  const scenario = button.dataset.scenario;
  if (scenario === 'reverse') state.drawings[0][0].reverse();
  if (scenario === 'order') [state.drawings[0][0], state.drawings[0][1]] = [state.drawings[0][1], state.drawings[0][0]];
  if (scenario === 'confusable') {
    const pair = { u3042: 'u304a', u304a: 'u3042', u306c: 'u3081', u3081: 'u306c' };
    state.drawings[0] = clone(state.catalog.dataset.characters.find(x => x.id === pair[templates()[0].id]).strokes);
  }
  $('human-label').value = 'unknown'; state.active = 0; renderTabs(); renderBoard(); message('Đang dùng mẫu sinh sẵn để kiểm tra. Bấm “Chấm bài” để xem kết quả.');
}));

$('export').addEventListener('click', () => {
  if (!state.target || state.pointer !== null) return;
  const capture = { formatVersion: 1, targetId: state.target.id, characters: state.drawings, humanLabel: $('human-label').value,
    drawingSource: state.source, pointerTypes: [...state.pointerTypes], algorithmVersion: state.catalog.policy.version,
    templateRevision: state.catalog.dataset.sourceRevision, capturedAt: new Date().toISOString() };
  const url = URL.createObjectURL(new Blob([JSON.stringify(capture, null, 2)], { type: 'application/json' }));
  const link = document.createElement('a'); link.href = url; link.download = `realityloop-handwriting-${state.target.id}-${Date.now()}.json`; link.click();
  setTimeout(() => URL.revokeObjectURL(url), 1000); message('Đã xuất mẫu vào máy của bạn; nhãn dùng để hiệu chỉnh vẫn cần người kiểm chứng.');
});
$('import').addEventListener('click', () => $('import-file').click());
$('import-file').addEventListener('change', async event => {
  const file = event.target.files[0]; event.target.value = ''; if (!file || !state.catalog || state.pointer !== null) return;
  try {
    if (file.size > 262144) throw new Error('Mẫu JSON phải nhỏ hơn 256 KiB.');
    const capture = JSON.parse(await file.text());
    const target = state.catalog.targets.find(x => x.id === capture.targetId);
    if (capture.formatVersion !== 1 || !target || capture.templateRevision !== state.catalog.dataset.sourceRevision ||
      !Array.isArray(capture.characters) || capture.characters.length !== target.characterIds.length) throw new Error('Mẫu không đúng định dạng hoặc khác phiên bản template.');
    let total = 0;
    for (const strokes of capture.characters) {
      if (!Array.isArray(strokes) || strokes.length > state.catalog.policy.maximumStrokesPerCharacter) throw new Error('Số nét không hợp lệ.');
      for (const points of strokes) {
        if (!Array.isArray(points) || points.length < 1 || points.length > state.catalog.policy.maximumPointsPerStroke) throw new Error('Số điểm không hợp lệ.');
        total += points.length;
        if (points.some(p => !p || !Number.isFinite(p.x) || !Number.isFinite(p.y) || p.x < 0 || p.x > 1 || p.y < 0 || p.y > 1)) throw new Error('Tọa độ mẫu không hợp lệ.');
      }
    }
    if (total > state.catalog.policy.maximumTotalPoints) throw new Error('Mẫu có quá nhiều điểm.');
    selectTarget(target); state.drawings = capture.characters.map(strokes => strokes.map(points => points.map(({ x, y }) => ({ x, y }))));
    state.source = ['handwritten', 'generated', 'mixed'].includes(capture.drawingSource) ? capture.drawingSource : 'imported';
    $('human-label').value = ['correct', 'incorrect'].includes(capture.humanLabel) ? capture.humanLabel : 'unknown';
    renderBoard(); message('Đã mở mẫu từ máy. Bấm “Chấm bài” để chấm lại với bộ chấm hiện tại.');
  } catch (error) { message(error.message, true); }
});

async function start() {
  try {
    const response = await fetch('/api/v1/handwriting-lab/templates');
    if (!response.ok) throw new Error('Không tải được mẫu nét.');
    state.catalog = await response.json();
    $('source-link').href = state.catalog.dataset.sourceUrl;
    $('versions').textContent = `Bộ chấm: ${state.catalog.policy.version} · Ngưỡng nét: ${state.catalog.policy.shapeTolerance} · Template: ${state.catalog.dataset.sourceRevision}`;
    for (const target of state.catalog.targets) {
      const button = document.createElement('button'); button.type = 'button'; button.className = 'target'; button.dataset.id = target.id;
      const symbol = document.createElement('span'); symbol.textContent = target.text;
      const note = document.createElement('small'); note.textContent = target.characterIds.length === 1 ? '01 CHỮ' : '02 CHỮ';
      button.append(symbol, note); button.addEventListener('click', () => selectTarget(target)); $('targets').append(button);
    }
    selectTarget(state.catalog.targets[0]);
  } catch (error) { message(error.message, true); }
}
start();
