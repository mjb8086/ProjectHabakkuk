let toast; // Init in app root element so it will bind with the Toast placeholder.

const Notification = {
    success: function (summary, detail) {
        summary = !summary || summary.length < 1 ? 'Success' : summary;
        toast.add({ severity: 'success', summary: summary, detail: detail, life: 3000 });
    },
    info : function (summary, detail) {
        summary = !summary || summary.length < 1 ? 'Information' : summary;
        toast.add({ severity: 'info', summary: summary, detail: detail, life: 3000 });
    },
    warning : function (summary, detail) {
        summary = !summary || summary.length < 1 ? 'Warning' : summary;
        toast.add({ severity: 'warning', summary: summary, detail: detail, life: 3000 });
    },
    error : function (summary, detail) {
        summary = !summary || summary.length < 1 ? 'Error' : summary;
        toast.add({ severity: 'error', summary: summary, detail: detail, life: 3000 });
    }
}

export const InitToast = function (toastService) {
    if (!toast) toast = toastService;
}

export default Notification;