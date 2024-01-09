import Globals from './globals.js';

$(document).ready(function () {
// Functions
// Listeners
    $("#btnAdd").on('click', function (e) {
        window.location = `${Globals.BaseUrl}/mynd/record/clientrecord?clientId=${e.target.dataset.clientid}`;
    });
});
