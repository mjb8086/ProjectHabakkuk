import Globals from './globals.js';

$(document).ready(function () {
// Functions
    function updateTreatment(id) {
        $.ajax({
            url: `${Globals.BaseUrl}/mynd/appointment/doupdatetreatment`,
            contentType: "application/json",
            method: "PUT",
            data: JSON.stringify({
                id: +id,
                title: $("#title").val(),
                description: $("#description").val(),
                cost: parseFloat($("#cost").val()),
                requestability: +$("input[name='requestability']:checked").val()
            }),
            success: function (data) {  }
        });
    };

// Listeners
    $("#btnSave").on('click', function (e) {
        if (e.target.dataset.id > 0) {
            e.preventDefault();
            updateTreatment(e.target.dataset.id);
        }
    });
    $("#btnGoBack").on('click', function (e) {
        e.preventDefault();
        window.location = `${Globals.BaseUrl}/mynd/appointment/treatmentmanagement`
    });
});
