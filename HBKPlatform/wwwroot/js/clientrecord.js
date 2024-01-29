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
        contentType: "application/json",
        method: "PUT",
        data: JSON.stringify({noteBody: $("#noteBody").val()}),
        success: function (data) { $("#noteBody").val(data); }
    });
};
    function createNote(clientId) {
        $.ajax({
            url: `${Globals.BaseUrl}/mynd/record/createrecord`,
            contentType: "application/json",
            method: "POST",
            data: JSON.stringify({
                title: $("#noteTitle").val(),
                noteBody: $("#noteBody").val(),
                visibility: 0,
                isPriority: false,
                clientId: clientId
            }),
            success: function (data) {  }
        });
    };
    function deleteNote(noteId, clientId) {
         window.location = `${Globals.BaseUrl}/mynd/record/delete?recordId=${noteId}&clientId=${clientId}`;
    };

// Listeners
    $("#btnSave").on('click', function (e) {
        e.preventDefault();
        if (e.target.dataset.id > 0) {
            updateNote(e.target.dataset.id);
        } else {
            createNote(e.target.dataset.clientid)
        }
    });
    $("#btnDelete").on('click', function (e) {
        e.preventDefault();
        if (e.target.dataset.id > 0) deleteNote(e.target.dataset.id, e.target.dataset.clientid);
    });
    $("#btnAllNotes").on('click', function (e) {
        window.location = `${Globals.BaseUrl}/mynd/record/clientRecords?clientId=${e.target.dataset.id}`;
    });
});
