// Bitácora Diaria — drag&drop, pegado desde portapapeles y subida directa a Blob Storage.
window.bitacora = {
    // Conecta un contenedor (drop zone) a un <input type=file> subyacente: arrastrar
    // archivos o pegar (Ctrl+V) una imagen los inyecta en el mismo input, que dispara
    // el evento 'change' normal de Blazor (InputFile.OnChange) como si el usuario
    // los hubiera elegido con el explorador de archivos.
    wireDropZone: function (dropZoneElement, inputElement) {
        if (!dropZoneElement || !inputElement) return;

        const stop = e => { e.preventDefault(); e.stopPropagation(); };
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(evt =>
            dropZoneElement.addEventListener(evt, stop));

        dropZoneElement.addEventListener('drop', e => {
            if (e.dataTransfer && e.dataTransfer.files && e.dataTransfer.files.length > 0) {
                inputElement.files = e.dataTransfer.files;
                inputElement.dispatchEvent(new Event('change', { bubbles: true }));
            }
        });

        dropZoneElement.setAttribute('tabindex', '0');
        dropZoneElement.addEventListener('paste', e => {
            const items = (e.clipboardData || window.clipboardData)?.items;
            if (!items) return;
            const dt = new DataTransfer();
            for (const item of items) {
                if (item.kind === 'file') {
                    const file = item.getAsFile();
                    if (file) dt.items.add(file);
                }
            }
            if (dt.files.length > 0) {
                inputElement.files = dt.files;
                inputElement.dispatchEvent(new Event('change', { bubbles: true }));
            }
        });
    },

    // Sube el archivo en la posición `index` del input directo a Blob Storage vía SAS,
    // sin pasar los bytes por el circuito de Blazor Server (necesario para video).
    uploadToBlob: async function (inputElement, index, sasUrl, contentType) {
        const file = inputElement?.files?.[index];
        if (!file) throw new Error('No se encontró el archivo a subir');

        const resp = await fetch(sasUrl, {
            method: 'PUT',
            headers: {
                'x-ms-blob-type': 'BlockBlob',
                'Content-Type': contentType
            },
            body: file
        });

        if (!resp.ok) throw new Error(`Error al subir el archivo (HTTP ${resp.status})`);
        return true;
    },

    openInNewTab: function (url) {
        window.open(url, '_blank');
    },

    clickElement: function (element) {
        element?.click();
    }
};
