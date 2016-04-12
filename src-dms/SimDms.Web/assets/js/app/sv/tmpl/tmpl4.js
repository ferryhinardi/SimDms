$(document).ready(function () {
    var options = {
        title: "Select & Switch",
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
            { name: "field06", text: "Field 06", type: "spinner" },
            { name: "field07", text: "Field 07", type: "datepicker" },
            { name: "field08", text: "Field 08", type: "datepicker" },
            { name: "field09", text: "Field 09", type: "select" },
            { name: "field10", text: "Field 10", type: "switch", float: "left" },
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.render()
});