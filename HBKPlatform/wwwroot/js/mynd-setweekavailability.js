import Globals from './globals.js';

$(document).ready(function () {
    // Model
    var updatedAvailability = {};
    
    // Elements
    const btnDiscard = $("#btnDiscard");
    const allTs = $(".ava-ts");
    
    // Functions
    function revertAll()
    {
        allTs.removeClass("unavailable");
        if(isIndefView) allTs.removeClass("indefinite");
        allTs.addClass("available");
        allTs.attr("data-available", "true");
    }
    
    function toggleIndefinite(self, isUnavailable)
    {
        if (self.dataset.isindef !== "true") return;
        if(self.classList.contains("indefinite"))
        {
            self.classList.remove("indefinite");
        }
        else
        {
            self.classList.add("indefinite");
        }
    }
    
    // Listeners
    allTs.on('click', function (e) {
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
        
        if(isIndefView && this.classList.contains("available"))
        {
            this.classList.remove("indefinite");
        }
    });
    
    btnDiscard.on('click', function() {
        location.reload();
    });
    
    $("#btnExit").on('click', function() {
        window.location.href = `${Globals.BaseUrl}/mynd/appointment/availabilitymanagement`;
    });
    
    $("#btnApply").on('click', function(e) {
        $.ajax({
            url: `${Globals.BaseUrl}/mynd/appointment/dosetavailability` + (e.target.dataset.weeknum ? `?weekNum=${e.target.dataset.weeknum}` : ""),
            contentType: "application/json",
            method: "POST",
            data: JSON.stringify({
                updated: updatedAvailability
            }),
            success: function (data) {
                Globals.HBKFlasher("Successfully updated availability.");
                $('#changesMade').hide();
            },
            error: function () {
                Globals.HBKFlasher("ERROR", "error");
            }
        });
    });
    
    $("#btnRevert").on('click', function(e) {
        $.ajax({
            url: `${Globals.BaseUrl}/mynd/appointment/dorevertavailability` + (e.target.dataset.weeknum ? `?weekNum=${e.target.dataset.weeknum}` : ""),
            method: "GET",
            success: function (data) {
                Globals.HBKFlasher("Successfully reverted availability. All time periods are now available for this week.");
                revertAll();
                $('#changesMade').hide();
            },
            error: function () {
                Globals.HBKFlasher("ERROR", "error");
            }
        });
    });

});
