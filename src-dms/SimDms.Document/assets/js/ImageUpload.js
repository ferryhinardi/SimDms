/// <reference path="jquery-1.10.2.js" />
var imageData = '';
$(document).ready(function () 
{
    $(".editor-wysiwyg").wysihtml5();

    $("#addImageBtn").click(function () {
        $(".image-upload-modal").modal("show");
    });

    $("#imageFile").change();
});

