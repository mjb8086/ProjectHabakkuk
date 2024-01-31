import Globals from './globals.js';

$(document).ready(function () {
    var updatedAvailability = {};
    const btnDiscard = $("#btnDiscard");
    
    // Listeners
    $(".ava-ts").on('click', function (e) {
        $('.changeBtn').prop('disabled', false);
        $('#changesMade').show();
        if (this.dataset.available == "true") {
            updatedAvailability[this.dataset.tsid] = false;
            this.dataset.available = "false";
            this.classList.remove("available");
            this.classList.add("unavailable");
        } else {
            updatedAvailability[this.dataset.tsid] = true;
            this.dataset.available = "true";
            this.classList.remove("unavailable");
            this.classList.add("available");
        }
    });
    
    btnDiscard.on('click', function() {
        location.reload();
    });
    
    $("#btnExit").on('click', function() {
        window.location.href = `${Globals.BaseUrl}/mynd/appointment/availabilitymanagement`;
    })

});
