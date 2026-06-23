import Globals from './globals.js';

$(document).ready(function () {
    $("#btnGoBack").on('click', function (e) {
        e.preventDefault();
        window.location = `${Globals.BaseUrl}/mynd/client/allclients`;
    });
    
    // Listeners
    
    //Override POST when there is a value of save - only way to do it without going insane from asp.net tag helpers
    $("#btnSave").on('click', function (e) {
        if (e.target.dataset.id > 0) {
            e.preventDefault();
            const deets = $("#treatmentDetails");
            deets.attr({"action": `${Globals.BaseUrl}/mynd/client/doeditclient?clientId=${e.target.dataset.id}`, "method": "post"} );
            deets.submit();
        }
    });
});
