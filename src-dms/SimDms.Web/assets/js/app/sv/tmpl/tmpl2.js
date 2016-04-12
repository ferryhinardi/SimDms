$(document).ready(function () {
    var options = {
        title: "Textbox & Textarea",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
        ],
        items: [
            { name: "field01", text: "Field 01" },
            { name: "field02", text: "Field 02" },
            { name: "field03", text: "Field 03" },
            { name: "field04", text: "Field 04", type: "textarea" },
            { name: "field05", text: "Field 05", type: "textarea" },
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.render()
});