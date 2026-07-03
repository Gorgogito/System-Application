window.TaskPlanner = {
    dragData: null,

    setDragData: function (type, id) {
        window.TaskPlanner.dragData = { type, id };
    },

    getDragData: function () {
        return window.TaskPlanner.dragData;
    },

    clearDragData: function () {
        window.TaskPlanner.dragData = null;
    },

    initDrag: function (elementId, type, id) {
        const el = document.getElementById(elementId);
        if (!el) return;
        el.setAttribute('draggable', 'true');
        el.ondragstart = (e) => {
            window.TaskPlanner.setDragData(type, id);
            e.dataTransfer.effectAllowed = 'move';
            el.classList.add('dragging');
        };
        el.ondragend = () => {
            el.classList.remove('dragging');
            window.TaskPlanner.clearDragData();
        };
    },

    initDrop: function (elementId, dotnetRef, day, month, year) {
        const el = document.getElementById(elementId);
        if (!el) return;
        el.ondragover = (e) => {
            e.preventDefault();
            e.dataTransfer.dropEffect = 'move';
            el.classList.add('drag-over');
        };
        el.ondragleave = () => el.classList.remove('drag-over');
        el.ondrop = (e) => {
            e.preventDefault();
            el.classList.remove('drag-over');
            const data = window.TaskPlanner.getDragData();
            if (data) {
                dotnetRef.invokeMethodAsync('OnDropped', data.type, data.id, day, month, year);
                window.TaskPlanner.clearDragData();
            }
        };
    }
};
