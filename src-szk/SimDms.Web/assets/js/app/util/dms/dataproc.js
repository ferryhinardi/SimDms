$(document).ready(function () {
    var options = {
        title: "SDMS Data Process",
        xtype: "panels",
        panels: [
            {
                title: "Filter Data",
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.render();
});