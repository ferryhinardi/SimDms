$(document).ready(function () {
    var options = {
        title: "Data Trend",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Position",
                        type: "controls",
                        items: [
                            { name: "Position", text: "Position", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                            { name: "Year", text: "Reference Year", cls: "span2", type: "select" },
                        ]
                    },
                ],
            },
            {
                name: "SfmTrend",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
        ],
    }
    var widget = new SimDms.Widget(options);
    //widget.setSelect([{ name: "Year", url: "ab.api/combo/years", optionalText: "-- SELECT YEAR --" }]);
    widget.default = { Year: new Date().getFullYear() };
    widget.render(function () {
        widget.populate(widget.default);
        widget.select({ selector: "[name=Position]", url: "ab.api/combo/positions", params: { id: "SALES" }, optionalText: "-- SELECT ALL --" });
        widget.select({ selector: "[name=Year]", url: "ab.api/combo/years", optionalText: "-- SELECT YEAR --", callbackValue: new Date().getFullYear() });
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $("#pnlFilter select").on("change", refreshGrid);

        setTimeout(refreshGrid, 500);
    });

    function refreshGrid() {
        widget.kgrid({
            url: "ab.api/inquiry/sfmtrend",
            name: "SfmTrend",
            params: $("#pnlFilter").serializeObject(),
            pageable: false,
            pageSize: 100,
            columns: [
                { field: "BranchName", title: "Branch (Outlet)", width: 700 },
                {
                    field: "Month01", title: "Jan", width: 88,
                    template: "<div class='right'>#=Month01#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month02", title: "Feb", width: 88,
                    template: "<div class='right'>#=Month02#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month03", title: "Mar", width: 88,
                    template: "<div class='right'>#=Month03#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month04", title: "Apr", width: 88,
                    template: "<div class='right'>#=Month04#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month05", title: "May", width: 88,
                    template: "<div class='right'>#=Month05#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month06", title: "Jun", width: 88,
                    template: "<div class='right'>#=Month06#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month07", title: "Jul", width: 88,
                    template: "<div class='right'>#=Month07#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month08", title: "Aug", width: 88,
                    template: "<div class='right'>#=Month08#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month09", title: "Sep", width: 88,
                    template: "<div class='right'>#=Month09#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month10", title: "Oct", width: 88,
                    template: "<div class='right'>#=Month10#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month11", title: "Nov", width: 88,
                    template: "<div class='right'>#=Month11#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month12", title: "Dec", width: 88,
                    template: "<div class='right'>#=Month12#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
            ],
            aggregate: [
                   { field: "Month01", aggregate: "sum" },
                   { field: "Month02", aggregate: "sum" },
                   { field: "Month03", aggregate: "sum" },
                   { field: "Month04", aggregate: "sum" },
                   { field: "Month05", aggregate: "sum" },
                   { field: "Month06", aggregate: "sum" },
                   { field: "Month07", aggregate: "sum" },
                   { field: "Month08", aggregate: "sum" },
                   { field: "Month09", aggregate: "sum" },
                   { field: "Month10", aggregate: "sum" },
                   { field: "Month11", aggregate: "sum" },
                   { field: "Month12", aggregate: "sum" },
            ],
            dataBound: function () {
                var list = $(".k-grid-header-wrap th").find(".k-link");
                for (var i = 1; i < list.length; i++) {
                    $(list[i]).addClass("right");
                }
            }
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "SfmTrend",
            type: "kgrid",
            fileName: "pers_trend",
            items: [
                { name: "BranchName", text: "Branch" },
                { name: "Month01", text: "Jan" },
                { name: "Month02", text: "Feb" },
                { name: "Month03", text: "Mar" },
                { name: "Month04", text: "Apr" },
                { name: "Month05", text: "May" },
                { name: "Month06", text: "Jun" },
                { name: "Month07", text: "Jul" },
                { name: "Month08", text: "Aug" },
                { name: "Month09", text: "Sep" },
                { name: "Month10", text: "Oct" },
                { name: "Month11", text: "Nov" },
                { name: "Month12", text: "Dec" },
            ]
        });
    }
});
