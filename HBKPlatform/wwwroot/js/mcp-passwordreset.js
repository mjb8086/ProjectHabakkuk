import Globals from './globals.js';

$(document).ready(function () {
    // Functions
    function updatePracList(pracs) {
        $('#pracSelect').empty();
        pracs.forEach(function(e) {
            $('#pracSelect').append($('<option>',{text: e.name, value: e.id}));
        });
    }
    
    function fetchClinicPracs(id) {
        $.ajax({
            url: `${Globals.BaseUrl}/mcp/clinicmanagement/getclinicpracs?clinicId=${id}`,
            contentType: "application/json",
            method: "get",
            success: function (data) {
                updatePracList(data.pracs);
            }
        });
    }
    
    // Listeners
    $("#clinicSelect").on('change', function (e) {
        fetchClinicPracs(e.currentTarget.value);
        $('#doIt').removeAttr("disabled");
        $('#pracSelect').removeAttr("disabled");
    });
    
    $('#clinicSelect').change();
});
