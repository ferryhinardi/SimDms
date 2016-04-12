$(document).ready(function () {
    var options = {
        title: "Dashboard Chart",
        xtype: "panels",
        panels: [
            {
                title: "Employee Distribution",
                xtype: "chart",
                charts: [
                    { name: "EmplByDept", cls: "span4" },
                    { name: "EmplByLocation", cls: "span4" }
                ]
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        widget.chart({
            name: "EmplByDept",
            url: "ab.api/chart/EmplByDept",
        });
        widget.chart({
            name: "EmplByLocation",
            url: "ab.api/chart/EmplByLocation",
        });
    });
});