$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Refreshing...",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
            },
        ]
    });

    widget.render(function () {
        var mylink = $("#btnRefresh").data("link");
        if (mylink !== undefined) {
            var link = ($("#btnRefresh").data("link"));
            location.href = link;
        }
    });


});