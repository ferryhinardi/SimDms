$(document).ready(function () {
    var options = {
        title: "Dashboard Chart",
        xtype: "panels",
        panels: [
            {
                title: "Employee Distribution",
                xtype: "chart",
                charts: [
                    { name: "EmplByDept1", cls: "span4" },
                    { name: "EmplByDept2", cls: "span4" },
                ]
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        widget.chart({
            name: "EmplByDept1",
            url: "ab.api/chart/EmplByDept",
        });
        widget.chart({
            name: "EmplByDept2",
            ctype: "hchart",
            url: "ab.api/chart/EmplByDept",
        });
    });
});