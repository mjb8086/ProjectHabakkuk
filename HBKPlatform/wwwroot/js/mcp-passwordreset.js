import Globals from './globals.js';

$(document).ready(function () {
    // Model
    var pracData = {};
    var isDisabled = true;
    
    // Elements
    const csel = $("#clinicSelect");
    const psel = $("#pracSelect");
    const ls = $("#lockoutStatus");
    const le = $("#lockoutEnd");
    
    // Functions
    function updatePracList(myPracData) {
        psel.empty();
        var isFirst = true;
        const pracKeys = Object.keys(myPracData);
        pracKeys.forEach(function(e) {
           if (isFirst) {
               psel.append($('<option>',{text: myPracData[e].name, value: e, selected: "selected"}));
               isFirst = false;
           } else {
               psel.append($('<option>',{text: myPracData[e].name, value: e}));
           }
        });
        displayLockout(myPracData[pracKeys[0]].id);
    }
    
    function displayLockout(pracId) {
        if(!pracData[pracId]) { 
            console.error(`pracID ${pracId} not in pracdata`);
            return;
        }
        console.log(`FOUND: ${JSON.stringify(pracData[pracId])}`);
       if(pracData[pracId].hasLockout) {
            ls.text("Lockout");
            le.text(pracData[pracId].lockoutEnd);
            ls.removeClass('active');
            ls.addClass('inactive');
       } else {
            ls.text("Unlocked (Active)");
            le.text("")
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
                console.log(`Finished fetching prac data, result: ${JSON.stringify(data)} `);
                pracData = data.pracs;
                updatePracList(pracData);
            }
        });
    }
    
    // Listeners
    csel.on('change', function (e) {
        fetchClinicPracs(e.target.value);
        if (isDisabled) {
            $('#doIt').removeAttr("disabled");
            psel.removeAttr("disabled");
            isDisabled = false;
        }
    });
    
    psel.on('change', function (e) {
       displayLockout(e.target.value);
    });
    
    csel.trigger("change");
});
