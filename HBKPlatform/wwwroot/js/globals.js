const Globals = {
    BaseUrl : "https://localhost:7251",
    TreatmentRequestability : {ClientAndPrac : 2, PracOnly : 1, None : 0},
    HBKFlasher: function (message, status) {
        // Create flasher, flash it.
        $("body").prepend($(`<div id=\"hbkFlasher\" class=\"${status}\"><div id=\"hbkFlasherMessage\">${message}</div></div>`));
        const me = $("#hbkFlasher");
        me.fadeOut(100).fadeIn(100).fadeOut(100).fadeIn(100);

        function dismiss() {
            me.fadeOut(300);
            setTimeout(function () { me.remove(); }, 300);
        }

        // Add click on dismiss
        me.on("click", function(e) {
            dismiss();
        });

        // Fade out after time
        setTimeout(dismiss, 4000);
    }
};

export default Globals;