import Globals from './globals.js';

$(document).ready(function () {
    $("#btnGoBack").on('click', function (e) {
        e.preventDefault();
        window.location = `${Globals.BaseUrl}/client/reception`;
    });

});
