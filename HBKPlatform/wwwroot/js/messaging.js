
$(document).ready(function () {
    // when press return (kc 13) send message
    $("#messageBody").on('keypress', function (e) {
        if (e.which === 13 && !e.shiftKey) { // enter but no shift
            e.preventDefault(); // do not make a newline
            $("#messageInput").submit();
        }
    });
});

