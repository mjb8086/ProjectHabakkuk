// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
$(document).ready(function () {
   // HBK Flasher 
    function flashMessage(message, type) {
        // Create flasher, flash it.
        $("body").prepend($(`<div id=\"hbkFlasher\"><div id=\"hbkFlasherMessage\">${message}</div></div>`));
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
    
    console.log("Global Hello World!");
    if (window.hbkFlasher && window.hbkFLasher != "") flashMessage(window.hbkFlasher, "");
})