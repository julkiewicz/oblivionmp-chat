const CAPACITY = 100;

// How long the log stays up after the last thing happened, once chat is closed.
const FADE_AFTER_MS = 8000;

const lines = document.getElementById('lines');
const entry = document.getElementById('entry');

let fadeTimer = 0;

const clock = new Intl.DateTimeFormat(undefined, { hour: '2-digit', minute: '2-digit', hour12: false });

// Shows the log and restarts the countdown to it fading. While chat is open the log stays up, so there is
// nothing to count down to.
function wake() {
  document.body.classList.remove('idle');
  clearTimeout(fadeTimer);

  if (!document.body.classList.contains('open')) {
    fadeTimer = setTimeout(() => document.body.classList.add('idle'), FADE_AFTER_MS);
  }
}

// OPEN MEANS FOCUSED. The game gives the page the keyboard and then says so; the page answers by focusing its
// entry line, which is the only moment it can, because losing focus deliberately clears whatever was focused.
// Nothing here closes chat by itself: while the entry line is up every key belongs to it, including the ones
// the game would otherwise take.
function setOpen(open) {
  document.body.classList.toggle('open', open);

  if (open) {
    wake();
    entry.focus();

    // OPEN MUST MEAN FOCUSED. If the entry did not take the keyboard there is no blur to come later and
    // nothing here could hear a key, so the game would hold input for a page that cannot ask for it back.
    if (document.activeElement !== entry) readym.sendClose();
    return;
  }

  entry.value = '';
  entry.blur();

  // PUT AWAY AT ONCE when chat is closed, rather than lingering. The countdown above is for a line that
  // arrived while you were playing; a chat you just dismissed should go.
  clearTimeout(fadeTimer);
  document.body.classList.add('idle');
}

function show(batch, replace) {
  if (replace) lines.replaceChildren();

  for (const line of batch) {
    const item = document.createElement('li');
    item.className = line.kind;

    // A local line from another mod can ask for its own color over the one its kind gets.
    if (line.color) item.style.color = line.color;

    const time = document.createElement('span');
    time.className = 'time';
    time.textContent = `[${clock.format(new Date(line.at))}] `;
    item.append(time);

    if (line.kind === 'Player') {
      const who = document.createElement('span');
      who.className = 'nickname';
      who.textContent = line.nickname;
      item.append(who, document.createTextNode(`: ${line.text}`));
    } else {
      item.append(line.text);
    }

    lines.append(item);
  }

  while (lines.children.length > CAPACITY) lines.firstChild.remove();
  lines.scrollTop = lines.scrollHeight;

  wake();
}

readym.on('Line', (json) => show(JSON.parse(json), false));
readym.on('History', (json) => show(JSON.parse(json), true));
readym.on('Open', (open) => setOpen(open));

readym.sendRequestHistory();

// ANYTHING TAKING THE KEYBOARD ENDS CHAT. The game hands input to the UI while chat is up and takes it back
// only when told, and the only thing that can tell it is a page that still has the keyboard. So a page that
// quietly lost focus would leave input with the UI and nothing able to give it back.
//
// ONCE, not per frame: the game answers by closing, which clears the class below before blurring the entry
// itself, so its own blur says nothing.
entry.addEventListener('blur', () => {
  if (document.body.classList.contains('open')) readym.sendClose();
});

entry.addEventListener('keydown', (e) => {
  // ESCAPE IS THE ONLY WAY OUT, and it is caught here rather than bound in the game: the game binds one key,
  // the one that opens chat, and everything while chat is up belongs to the page.
  if (e.key === 'Escape') {
    readym.sendClose();
    return;
  }

  if (e.key !== 'Enter') return;

  // SENDS WITHOUT CLOSING. Chat that closed itself on every line would hand the keyboard back mid-conversation,
  // and the next key you typed would reach the game instead.
  const text = entry.value.trim();
  entry.value = '';

  if (text) readym.sendSubmit(text);
});

wake();
