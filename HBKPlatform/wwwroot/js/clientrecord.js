//import Quill from '../node_modules/quill/quill.js';
import Globals from './globals.js';

$(document).ready(function () {
// Broken, probably need to use Webpack. Deep Sigh.
    /*
const quill = new Quill('#clientRecord', {
    modules: {
        toolbar: true
    },
    theme: 'snow'
});
*/

// Functions
function updateNote(id) {
    $.ajax({
        url: `${Globals.BaseUrl}/mynd/record/updaterecordbody?recordId=${id}`,
        dataType: "text",
        contentType: "application/json",
        method: "PUT",
        data: JSON.stringify({noteBody: $("#noteBody").val()}),
        success: function (data) { $("#noteBody").val(data); }
    });
};

// Listeners
    $("#btnSave").on('click', function (e) {
        console.log("click");
        e.preventDefault();
        updateNote(e.target.dataset.id);
    });
});
