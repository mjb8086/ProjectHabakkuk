import Globals from './globals.js';

$(document).ready(function () {
    $("#btnGoBack").on('click', function (e) {
        e.preventDefault();
        window.location = `${Globals.BaseUrl}/clinic/room/list`;
    });
    
    // Listeners
    
    //Override POST when there is a value of save - only way to do it without going insane from asp.net tag helpers
    $("#btnSave").on('click', function (e) {
        if (e.target.dataset.id > 0) {
            e.preventDefault();
            const deets = $("#roomDetails");
            deets.attr({"action": `${Globals.BaseUrl}/clinic/room/doupdateroom?roomId=${e.target.dataset.id}`, "method": "post"} );
            deets.submit();
        }
    });
});
