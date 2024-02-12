import Globals from './globals.js';

$(document).ready(function () {
    // Model
    var pracData = {};
    
    // Elements
    const ls = $("#lockoutStatus");
    const psel = $("#pracSelect");
    const csel = $("#clinicSelect");
    
    // Functions
    function updatePracList(pracData) {
        psel.empty();
        Object.keys(pracData).forEach(function(e) {
            psel.append($('<option>',{text: pracData[e].name, value: e}));
        });
    }
    
    function displayLockout(pracId) {
       if(pracData[pracId].hasLockout) {
            ls.text("Lockout");
            ls.removeClass('active');
            ls.addClass('inactive');
       } else {
            ls.text("Unlocked (Active)");
            ls.removeClass('inactive');
            ls.addClass('active');
        }
    }
    
    function fetchClinicPracs(id) {
        $.ajax({
            url: `${Globals.BaseUrl}/mcp/clinicmanagement/getclinicpracs?clinicId=${id}`,
            contentType: "application/json",
            method: "get",
            success: function (data) {
                pracData = data.pracs;
                updatePracList(pracData);
            }
        });
    }
    
    // Listeners
    csel.on('change', function (e) {
        fetchClinicPracs(e.currentTarget.value);
        $('#doIt').removeAttr("disabled");
        psel.removeAttr("disabled");
        psel.change();
    });
    
    psel.on('change', function (e) {
       displayLockout(psel.val());
    });
    
    csel.change();
});
