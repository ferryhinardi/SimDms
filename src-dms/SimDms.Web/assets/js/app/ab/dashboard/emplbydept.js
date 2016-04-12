$(document).ready(function () {
    var options = {
        title: "Employee By Dept",
        xtype: "panels",
        panels: [
            {
                title: "Employee Distribution",
                xtype: "chart",
                charts: [
                    { name: "EmplByDept1" },
                    { name: "EmplByDept2" },
                ]
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        widget.chart({
            name: "EmplByDept1",
            url: "ab.api/chart/emplbydept",
            params: { ChartType: "column" },
            template: "#= value #"
        });
        widget.chart({
            name: "EmplByDept2",
            url: "ab.api/chart/emplbydept",
            params: { ChartType: "bar" },
            template: "#= series.name #: #= value #"
        });
    });
});