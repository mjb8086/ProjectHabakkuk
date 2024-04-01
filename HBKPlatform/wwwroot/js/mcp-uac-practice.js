import Globals from './globals.js';

$(document).ready(function () {
    // Model
    var practitionerData = {};
    var isDisabled = true;
    
    // Elements
    const practiceSelect = $("#practiceSelect");
    const practitionerSelect = $("#practitionerSelect");
    const ls = $("#lockoutStatus");
    const le = $("#lockoutEnd");
    
    // Functions
    function updatePractitionerList(thisPracticePractitioners) {
        practitionerSelect.empty();
        var isFirst = true;
        const practitionerKeys = Object.keys(thisPracticePractitioners);
        practitionerKeys.forEach(function(e) {
           if (isFirst) {
               practitionerSelect.append($('<option>',{text: thisPracticePractitioners[e].name, value: e, selected: "selected"}));
               isFirst = false;
           } else {
               practitionerSelect.append($('<option>',{text: thisPracticePractitioners[e].name, value: e}));
           }
        });
        displayLockout(thisPracticePractitioners[practitionerKeys[0]].id);
    }
    
    function displayLockout(practitionerId) {
        if(!practitionerData[practitionerId]) { 
            console.error(`pracID ${practitionerId} not in pracdata`);
            return;
        }
        console.log(`FOUND: ${JSON.stringify(practitionerData[practitionerId])}`);
       if(practitionerData[practitionerId].hasLockout) {
            ls.text("Lockout");
            le.text(practitionerData[practitionerId].lockoutEnd);
            ls.removeClass('active');
            ls.addClass('inactive');
       } else {
            ls.text("Unlocked (Active)");
            le.text("")
            ls.removeClass('inactive');
            ls.addClass('active');
        }
    }
    
    function fetchPracticePracs(id) {
        $.ajax({
            url: `${Globals.BaseUrl}/mcp/practicemanagement/getpracticepracs?practiceId=${id}`,
            contentType: "application/json",
            method: "get",
            success: function (data) {
                practitionerData = data.users;
                updatePractitionerList(practitionerData);
            }
        });
    }
    
    // Listeners
    practiceSelect.on('change', function (e) {
        fetchPracticePracs(e.target.value);
        if (isDisabled) {
            $('#doIt').removeAttr("disabled");
            practitionerSelect.removeAttr("disabled");
            isDisabled = false;
        }
    });
    
    practitionerSelect.on('change', function (e) {
       displayLockout(e.target.value);
    });
    
    practiceSelect.trigger("change");
});
