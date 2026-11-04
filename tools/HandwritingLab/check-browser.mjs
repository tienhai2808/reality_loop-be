// Browser smoke checks using a fresh headless Chrome profile and Node's built-in CDP/WebSocket APIs.
// No npm packages, personal browser profile, DB or external site is used.
import { spawn } from 'node:child_process';
import { mkdtemp, readFile, writeFile, mkdir, readdir, rm } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { resolve, join } from 'node:path';
import { setTimeout as delay } from 'node:timers/promises';

const base = process.argv[2] || 'http://127.0.0.1:5387';
if (!['127.0.0.1', 'localhost'].includes(new URL(base).hostname)) throw new Error('Use a loopback lab URL.');
const profile = await mkdtemp(join(tmpdir(), 'realityloop-handwriting-browser-'));
const output = resolve('artifacts/handwriting-lab');
await mkdir(output, { recursive: true });
const chrome = spawn(process.env.HANDWRITING_CHROME || '/usr/bin/google-chrome', [
  '--headless', '--no-sandbox', '--disable-gpu', '--disable-dev-shm-usage', '--no-first-run',
  '--no-default-browser-check', '--disable-background-networking', '--remote-debugging-address=127.0.0.1',
  '--remote-debugging-port=0', `--user-data-dir=${profile}`, 'about:blank'
], { stdio: 'ignore' });
let socket;
let checks = 0;
const pending = new Map();
let nextId = 1;
let sessionId;
const errors = [];
function check(value, message) { if (!value) throw new Error(message); checks++; }
async function waitFor(test, message) {
  for (let i = 0; i < 150; i++) { if (await test()) return; await delay(100); }
  throw new Error(message);
}
function send(method, params = {}, session = sessionId) {
  const id = nextId++;
  return new Promise((resolve, reject) => {
    const timer = setTimeout(() => { pending.delete(id); reject(new Error(`CDP timeout: ${method}`)); }, 15000);
    pending.set(id, { resolve, reject, timer });
    socket.send(JSON.stringify({ id, method, params, ...(session ? { sessionId: session } : {}) }));
  });
}
async function evaluate(expression) {
  const result = await send('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true });
  if (result.exceptionDetails) throw new Error(result.exceptionDetails.exception?.description || result.exceptionDetails.text);
  return result.result.value;
}
async function resultContains(text) {
  await waitFor(() => evaluate(`document.getElementById('feedback').textContent.includes(${JSON.stringify(text)})`), `Missing feedback: ${text}`);
  check(true, text);
}
async function scenario(name, expected) {
  await evaluate(`document.querySelector('[data-scenario="${name}"]').click(); document.getElementById('grade').click()`);
  await resultContains(expected);
}
async function draw(strokes, touch = false) {
  const box = await evaluate(`(() => { const b=document.getElementById('board').getBoundingClientRect(); return {x:b.x,y:b.y,w:b.width,h:b.height}; })()`);
  for (const stroke of strokes) {
    const points = stroke.filter((_, i) => i % 3 === 0 || i === stroke.length - 1);
    for (let i = 0; i < points.length; i++) {
      const x = box.x + points[i].x * box.w, y = box.y + points[i].y * box.h;
      if (touch) await send('Input.dispatchTouchEvent', { type: i === 0 ? 'touchStart' : 'touchMove', touchPoints: [{ x, y, id: 0 }] });
      else await send('Input.dispatchMouseEvent', { type: i === 0 ? 'mousePressed' : 'mouseMoved', x, y, button: 'left', buttons: 1, clickCount: 1 });
    }
    const last = points.at(-1);
    if (touch) await send('Input.dispatchTouchEvent', { type: 'touchEnd', touchPoints: [] });
    else await send('Input.dispatchMouseEvent', { type: 'mouseReleased', x: box.x + last.x * box.w, y: box.y + last.y * box.h, button: 'left', buttons: 0, clickCount: 1 });
  }
}
try {
  let portFile;
  await waitFor(async () => { try { portFile = await readFile(join(profile, 'DevToolsActivePort'), 'utf8'); return true; } catch { return false; } }, 'Chrome did not start.');
  const [port, route] = portFile.trim().split('\n');
  socket = new WebSocket(`ws://127.0.0.1:${port}${route}`);
  await new Promise((resolve, reject) => { socket.addEventListener('open', resolve, { once: true }); socket.addEventListener('error', reject, { once: true }); });
  socket.addEventListener('message', event => {
    const message = JSON.parse(event.data);
    if (message.id && pending.has(message.id)) {
      const request = pending.get(message.id); pending.delete(message.id); clearTimeout(request.timer);
      if (message.error) request.reject(new Error(JSON.stringify(message.error))); else request.resolve(message.result);
    }
    if (message.method === 'Runtime.exceptionThrown') errors.push(message.params.exceptionDetails.exception?.description || message.params.exceptionDetails.text);
  });
  const target = await send('Target.createTarget', { url: 'about:blank' }, null);
  sessionId = (await send('Target.attachToTarget', { targetId: target.targetId, flatten: true }, null)).sessionId;
  await send('Runtime.enable'); await send('Page.enable');
  await send('Emulation.setDeviceMetricsOverride', { width: 1365, height: 1100, deviceScaleFactor: 1, mobile: false });
  await send('Page.navigate', { url: base });
  await waitFor(() => evaluate(`document.querySelectorAll('.target').length === 5`), 'Lab did not load all targets.');
  check((await evaluate('document.title')).includes('RealityLoop'), 'Lab title.');
  await scenario('correct', 'Đạt mẫu nét');
  await scenario('reverse', 'Nét đang đi ngược chiều');
  await scenario('order', 'Thứ tự các nét chưa đúng');
  await scenario('confusable', 'Cần viết lại');
  await evaluate(`document.querySelector('.target[data-id="ao"]').click()`);
  check(await evaluate(`document.querySelectorAll('.character-tab').length === 2`), 'Two independent character cells for ao.');
  await scenario('correct', 'Đạt mẫu nét');
  check(await evaluate(`document.querySelectorAll('.character-result').length === 2`), 'Word grades both characters.');
  await evaluate(`document.querySelector('.character-tab:nth-child(2)').click(); document.getElementById('clear').click(); document.getElementById('grade').click()`);
  await resultContains('Chưa có nét viết');

  // Actual browser pointer events, not just filling the model through the diagnostic controls.
  const catalog = await (await fetch(base + '/api/v1/handwriting-lab/templates')).json();
  await evaluate(`document.querySelector('.target[data-id="me"]').click()`);
  await draw(catalog.dataset.characters.find(x => x.id === 'u3081').strokes);
  check(await evaluate(`document.getElementById('stroke-count').textContent.startsWith('2 nét')`), 'Mouse drawing captures pen lifts.');
  await evaluate(`document.getElementById('grade').click()`);
  await resultContains('Đạt mẫu nét');
  await send('Browser.setDownloadBehavior', { behavior: 'allow', downloadPath: output }, null);
  const previousFiles = new Set(await readdir(output));
  await evaluate(`document.getElementById('human-label').value='correct'; document.getElementById('export').click()`);
  let captureName;
  await waitFor(async () => {
    captureName = (await readdir(output)).find(name => name.endsWith('.json') && !previousFiles.has(name));
    return !!captureName;
  }, 'Capture JSON was not downloaded.');
  const capture = JSON.parse(await readFile(join(output, captureName), 'utf8'));
  check(capture.formatVersion === 1 && capture.targetId === 'me' && capture.humanLabel === 'correct' &&
    capture.drawingSource === 'handwritten' && capture.pointerTypes.includes('mouse'), 'Export retains strokes, label and drawing provenance.');
  const desktop = await send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: true });
  await writeFile(join(output, 'desktop.png'), Buffer.from(desktop.data, 'base64'));
  await evaluate(`document.getElementById('undo').click()`);
  check(await evaluate(`document.getElementById('stroke-count').textContent.startsWith('1 nét') && !document.getElementById('feedback').textContent.includes('Đạt mẫu nét')`), 'Undo removes a stroke and clears stale grade.');
  await evaluate(`document.getElementById('clear').click()`);
  check(await evaluate(`document.getElementById('stroke-count').textContent.startsWith('0 nét')`), 'Clear empties current cell.');
  const document = await send('DOM.getDocument');
  const input = await send('DOM.querySelector', { nodeId: document.root.nodeId, selector: '#import-file' });
  await send('DOM.setFileInputFiles', { nodeId: input.nodeId, files: [join(output, captureName)] });
  await waitFor(() => evaluate(`document.getElementById('stroke-count').textContent.startsWith('2 nét')`), 'Import did not restore strokes.');
  await evaluate(`document.getElementById('grade').click()`);
  await resultContains('Đạt mẫu nét');
  check(await evaluate(`document.getElementById('human-label').value === 'correct'`), 'Import restores the annotation label.');

  await send('Emulation.setDeviceMetricsOverride', { width: 390, height: 844, deviceScaleFactor: 2, mobile: true });
  await send('Emulation.setTouchEmulationEnabled', { enabled: true, maxTouchPoints: 1 });
  await evaluate(`document.querySelector('.target[data-id="nu"]').click(); window.scrollTo(0,0)`);
  check(await evaluate('document.documentElement.scrollWidth <= window.innerWidth + 1'), 'No horizontal overflow at 390 px.');
  await draw(catalog.dataset.characters.find(x => x.id === 'u306c').strokes, true);
  check(await evaluate(`document.getElementById('stroke-count').textContent.startsWith('2 nét')`), 'Touch drawing captures separate strokes.');
  await evaluate(`document.getElementById('grade').click()`);
  await resultContains('Đạt mẫu nét');
  const mobile = await send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: true });
  await writeFile(join(output, 'mobile.png'), Buffer.from(mobile.data, 'base64'));
  check(errors.length === 0, `No browser runtime errors: ${errors.join('\n')}`);
  console.log(`Passed ${checks} browser checks (mouse, touch, word cells, feedback, undo, responsive layout).`);
  console.log(`Screenshots: ${output}`);
} finally {
  if (socket?.readyState === WebSocket.OPEN) {
    try { await send('Browser.close', {}, null); } catch {}
    socket.close();
  }
  for (const request of pending.values()) { clearTimeout(request.timer); request.reject(new Error('Browser closed.')); }
  if (chrome.exitCode === null) chrome.kill('SIGTERM');
  await delay(300);
  await rm(profile, { recursive: true, force: true, maxRetries: 3 }).catch(() => {});
}
