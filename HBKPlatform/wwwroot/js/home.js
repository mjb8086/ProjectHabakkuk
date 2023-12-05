// ghastly.js

$(document).ready(function () {
    let topResult = null;

    const doAutocomplete = function (request, response) {
        $.ajax({
            type: "GET",
            url: "/search/lite?query=" + request["term"],
            success: function (data) { 
                console.log("data:" + JSON.stringify(data));
                response(data['specialties']); 
            },
            error: function () { response({"specialities": []})}
        }); 
     };

    $("#mainSearchBox").autocomplete({
        minLength: 2,
        delay: 100,
        source: doAutocomplete,
        select: function (event, selection) {
            console.log("Selection made. " + JSON.stringify(selection));
            console.log(selection.item.value);
            $("#mainSearchBox").val(selection.item.value);
            $("#mainSearchForm").submit();
        },
        response: function (e, ui) {
            if (ui.content.length > 0) {
                topResult = ui.content[0].value;
                console.log(topResult);
            }
        }
    });


    // when press return (kc 13) search for topmost value (wip)

    $("#mainSearchBox").on('keypress', function (e) {
        if (e.which === 13) { // enter
            if (topResult != null) {
                e.preventDefault();
                console.log(topResult);
                $("#mainSearch").val(topResult);
            }
            $(this).autocomplete("close");
            $("#mainSearchForm").submit();
        }
    });

    $("#mainSearchBox").on('keyup', function (e) {
        console.log("all clear");
        if (e.which === 8) { // backspace
            // FIXME: should clear top result and not nav when enter is struck after clearing
            topResult = null;
        }
    });
});

