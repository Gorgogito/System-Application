// Editor de documentos seguros — contenteditable + execCommand
// Módulo ES sin dependencias externas

let _activeEditorId    = null;
let _dotNetRef         = null;
let _selectionHandler  = null;

// ── Inicialización ───────────────────────────────────────────────────

export function initEditor(editorId, dotNetRef) {
    _cleanupListeners();

    _activeEditorId = editorId;
    _dotNetRef      = dotNetRef ?? null;

    const el = document.getElementById(editorId);
    if (!el) return;

    el.contentEditable = 'true';
    el.spellcheck      = true;

    // Paste limpia: conserva solo HTML básico
    el.addEventListener('paste', (e) => {
        e.preventDefault();
        const html = e.clipboardData?.getData('text/html');
        const text = e.clipboardData?.getData('text/plain') || '';
        if (html) {
            document.execCommand('insertHTML', false, sanitizeHtml(html));
        } else {
            document.execCommand('insertText', false, text);
        }
    });

    // Tab inserta sangría
    el.addEventListener('keydown', (e) => {
        if (e.key === 'Tab') {
            e.preventDefault();
            document.execCommand('insertHTML', false, '&nbsp;&nbsp;&nbsp;&nbsp;');
        }
    });

    // Notificación al mover cursor / selección — debounced 60 ms
    if (_dotNetRef) {
        let timer;
        _selectionHandler = () => {
            const editor = document.getElementById(_activeEditorId);
            if (!editor) return;
            const sel = window.getSelection();
            if (!sel || !editor.contains(sel.anchorNode)) return;
            clearTimeout(timer);
            timer = setTimeout(async () => {
                try {
                    await _dotNetRef.invokeMethodAsync('UpdateFormatState', _getFormatState());
                } catch { }
            }, 60);
        };
        document.addEventListener('selectionchange', _selectionHandler);
    }
}

export function disposeEditor() {
    _cleanupListeners();
    _dotNetRef      = null;
    _activeEditorId = null;
}

function _cleanupListeners() {
    if (_selectionHandler) {
        document.removeEventListener('selectionchange', _selectionHandler);
        _selectionHandler = null;
    }
}

// ── Contenido ────────────────────────────────────────────────────────

export function getContent(editorId) {
    return document.getElementById(editorId)?.innerHTML ?? '';
}

export function setContent(editorId, html) {
    const el = document.getElementById(editorId);
    if (el) el.innerHTML = html ?? '';
}

export function clearContent(editorId) {
    const el = document.getElementById(editorId);
    if (el) { el.innerHTML = ''; el.focus(); }
}

// ── Comandos de formato ──────────────────────────────────────────────

export function execCmd(command, value) {
    document.execCommand(command, false, value ?? null);
    if (_activeEditorId) document.getElementById(_activeEditorId)?.focus();
}

export function setFontSize(size) {
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) return;
    const range = sel.getRangeAt(0);
    if (range.collapsed) return;
    const span = document.createElement('span');
    span.style.fontSize = size;
    try { range.surroundContents(span); } catch { /* selección parcial */ }
    sel.removeAllRanges();
    if (_activeEditorId) document.getElementById(_activeEditorId)?.focus();
}

export function setForeColor(color) {
    document.execCommand('foreColor', false, color);
    if (_activeEditorId) document.getElementById(_activeEditorId)?.focus();
}

export function setBackColor(color) {
    document.execCommand('hiliteColor', false, color);
    if (_activeEditorId) document.getElementById(_activeEditorId)?.focus();
}

export function insertTable(rows, cols) {
    let html = '<table style="border-collapse:collapse;width:100%;margin:8px 0">';
    for (let r = 0; r < rows; r++) {
        html += '<tr>';
        for (let c = 0; c < cols; c++)
            html += '<td style="border:1px solid #bbb;padding:6px 10px;min-width:80px;">&nbsp;</td>';
        html += '</tr>';
    }
    html += '</table><br>';
    document.execCommand('insertHTML', false, html);
    if (_activeEditorId) document.getElementById(_activeEditorId)?.focus();
}

export function insertLink(url, text) {
    const html = `<a href="${url}" target="_blank" rel="noopener">${text || url}</a>`;
    document.execCommand('insertHTML', false, html);
    if (_activeEditorId) document.getElementById(_activeEditorId)?.focus();
}

// ── Estado de formato ────────────────────────────────────────────────

export function getFormatState() {
    return _getFormatState();
}

function _getFormatState() {
    return {
        bold:         document.queryCommandState('bold'),
        italic:       document.queryCommandState('italic'),
        underline:    document.queryCommandState('underline'),
        strike:       document.queryCommandState('strikeThrough'),
        alignLeft:    document.queryCommandState('justifyLeft'),
        alignCenter:  document.queryCommandState('justifyCenter'),
        alignRight:   document.queryCommandState('justifyRight'),
        alignJustify: document.queryCommandState('justifyFull'),
        bulletList:   document.queryCommandState('insertUnorderedList'),
        numberList:   document.queryCommandState('insertOrderedList'),
        fontSize:     _getCurrentFontSize(),
        foreColor:    _getCurrentForeColor()
    };
}

function _getCurrentFontSize() {
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) return null;
    const editor = _activeEditorId ? document.getElementById(_activeEditorId) : null;
    let el = sel.getRangeAt(0).startContainer;
    if (el.nodeType === 3) el = el.parentElement;
    while (el && el !== editor) {
        if (el.style && el.style.fontSize) return el.style.fontSize;
        el = el.parentElement;
    }
    return null;
}

function _getCurrentForeColor() {
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) return null;
    const editor = _activeEditorId ? document.getElementById(_activeEditorId) : null;
    let el = sel.getRangeAt(0).startContainer;
    if (el.nodeType === 3) el = el.parentElement;
    while (el && el !== editor) {
        if (el.style && el.style.color) return _rgbToHex(el.style.color) || el.style.color;
        el = el.parentElement;
    }
    const qc = document.queryCommandValue('foreColor');
    if (qc) { const hex = _rgbToHex(qc); if (hex && hex !== '#000000') return hex; }
    return null;
}

function _rgbToHex(rgb) {
    const m = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    if (!m) return null;
    return '#' + [1, 2, 3].map(i => parseInt(m[i]).toString(16).padStart(2, '0')).join('');
}

// ── Impresión ────────────────────────────────────────────────────────

export function printDocument(title, isEncrypted, content, versionInfo) {
    const win = window.open('', '_blank', 'width=900,height=700');
    if (!win) { alert('Permite ventanas emergentes para imprimir.'); return; }

    const badgeClass = isEncrypted ? 'badge-enc' : 'badge-dec';
    const badgeText  = isEncrypted ? '🔒 Versión cifrada — AES-256' : '🔓 Versión descifrada';

    const bodyHtml = isEncrypted
        ? `<p class="enc-notice">Este documento está protegido con cifrado AES-256.<br>
           El contenido solo es legible con las credenciales correctas.</p>
           <p class="enc-sub">Vista previa del texto cifrado:</p>
           <div class="enc-block">${_escHtml(content)}</div>`
        : `<div class="doc-content">${content}</div>`;

    win.document.write(`<!DOCTYPE html>
<html lang="es"><head><meta charset="utf-8">
<title>${_escHtml(title)}</title>
<style>
  *{box-sizing:border-box;margin:0;padding:0}
  body{font-family:'Calibri','Georgia',serif;font-size:11pt;line-height:1.6;color:#1a1a1a;padding:2.2cm 2.5cm}
  .ph{border-bottom:2.5px solid #1565c0;padding-bottom:14px;margin-bottom:28px}
  .ph-title{font-size:1.45rem;font-weight:700;color:#1a1a1a;margin-bottom:6px}
  .ph-badge{display:inline-block;font-size:.62rem;font-weight:700;text-transform:uppercase;
            letter-spacing:.07em;padding:3px 10px;border-radius:12px;margin-bottom:8px}
  .badge-enc{background:#fef3c7;color:#92400e;border:1px solid #fde68a}
  .badge-dec{background:#dcfce7;color:#166534;border:1px solid #86efac}
  .ph-meta{font-size:.75rem;color:#6b7280;line-height:1.7}
  .enc-notice{font-size:.85rem;color:#374151;background:#f9fafb;padding:14px 16px;
              border-radius:6px;border:1px solid #e5e7eb;margin-bottom:16px;line-height:1.6}
  .enc-sub{font-size:.68rem;font-weight:700;text-transform:uppercase;letter-spacing:.06em;
           color:#9ca3af;margin-bottom:6px}
  .enc-block{font-family:'Courier New',monospace;font-size:.62rem;word-break:break-all;
             background:#f4f6f9;padding:12px 14px;border-radius:4px;border:1px solid #e5e7eb;
             color:#374151;max-height:220px;overflow:hidden}
  .doc-content{font-family:'Calibri','Georgia',serif;font-size:11pt;line-height:1.7}
  .doc-content table{border-collapse:collapse;width:100%;margin:8px 0}
  .doc-content td,.doc-content th{border:1px solid #bbb;padding:6px 10px}
  .footer{margin-top:36px;padding-top:10px;border-top:1px solid #e5e7eb;
          font-size:.65rem;color:#9ca3af;display:flex;justify-content:space-between}
  @media print{body{padding:1cm 1.5cm}.footer{position:fixed;bottom:0;left:1.5cm;right:1.5cm}}
</style></head>
<body>
<div class="ph">
  <div class="ph-title">${_escHtml(title)}</div>
  <div><span class="ph-badge ${badgeClass}">${badgeText}</span></div>
  <div class="ph-meta">${versionInfo}</div>
</div>
${bodyHtml}
<div class="footer">
  <span>BDAplication · Documentos Seguros</span>
  <span>Impreso: ${new Date().toLocaleString('es-PE', {dateStyle:'short',timeStyle:'short'})}</span>
</div>
<script>window.onload=()=>{setTimeout(()=>window.print(),350)}<\/script>
</body></html>`);
    win.document.close();
}

function _escHtml(t) {
    return String(t ?? '').replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

// ── Helpers ─────────────────────────────────────────────────────────

function sanitizeHtml(html) {
    const div = document.createElement('div');
    div.innerHTML = html;
    div.querySelectorAll('script,style,[onclick],[onerror],[onload]').forEach(el => el.remove());
    div.querySelectorAll('*').forEach(el => {
        [...el.attributes].forEach(attr => {
            if (attr.name.startsWith('on')) el.removeAttribute(attr.name);
        });
    });
    return div.innerHTML;
}
