import Globals from './globals.js';

$(document).ready(function () {
    // Model
    var managerData = {};
    var isDisabled = true;
    
    // Elements
    const clinicSelect = $("#clinicSelect");
    const ls = $("#lockoutStatus");
    const le = $("#lockoutEnd");
    const mn = $("#managerName");
    
    // Functions
    function displayLockout() {
       mn.text(managerData.name);
       if(managerData.hasLockout) {
            ls.text("Lockout");
            le.text(managerData.lockoutEnd);
            ls.removeClass('active');
            ls.addClass('inactive');
       } else {
            ls.text("Unlocked (Active)");
            le.text("")
            ls.removeClass('inactive');
            ls.addClass('active');
        }
    }
    
    function fetchLeadManager(id) {
        $.ajax({
            url: `${Globals.BaseUrl}/mcp/clinicmanagement/getleadmanager?clinicId=${id}`,
            contentType: "application/json",
            method: "get",
            success: function (data) {
                managerData = data;
                displayLockout();
            },
            error: function() {
                Globals.HBKFlasher("ERROR!", "error");
            }
            
        });
    }
    
    // Listeners
    clinicSelect.on('change', function (e) {
        fetchLeadManager(e.target.value);
        if (isDisabled) {
            $('#doIt').removeAttr("disabled");
            isDisabled = false;
        }
    });
    
    clinicSelect.trigger("change");
});
