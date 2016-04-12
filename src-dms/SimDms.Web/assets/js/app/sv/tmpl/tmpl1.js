$(document).ready(function () {
    var options = {
        title: "Title & Toolbar",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render()
});