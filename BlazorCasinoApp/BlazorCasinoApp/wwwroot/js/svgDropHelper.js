window.svgDropHelper = {
    init: function (dotNetRef, normalElementId, mechaElementId) {

        function setupZone(elementId, fieldName) {
            const el = document.getElementById(elementId);
            if (!el) return;

            el.addEventListener('dragover', e => e.preventDefault());
            el.addEventListener('dragenter', e => e.preventDefault());
            el.addEventListener('drop', e => {
                e.preventDefault();
                const file = e.dataTransfer.files[0];
                if (!file) return;

                const reader = new FileReader();
                reader.onload = ev => {
                    dotNetRef.invokeMethodAsync('ReceiveSvgContent', fieldName, ev.target.result);
                };
                reader.readAsText(file);
            });
        }

        setupZone(normalElementId, 'normal');
        setupZone(mechaElementId, 'mecha');
    }
};