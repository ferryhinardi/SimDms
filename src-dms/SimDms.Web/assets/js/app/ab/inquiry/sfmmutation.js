$(document).ready(function () {
    var options = {
        title: "Data Mutation",
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
                            { name: "MutaDate", text: "Refference Date", cls: "span2", type: "datepicker" }
                        ]
                    },
                ],
            },
            {
                name: "SfmMutation",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "Year", url: "ab.api/combo/years", optionalText: "-- SELECT YEAR --" },
    ]);
    widget.default = { MutaDate: new Date() };
    widget.render(function () {
        widget.populate(widget.default)
        widget.select({ selector: "[name=Position]", url: "ab.api/combo/positions", params: { id: "SALES" }, optionalText: "-- SELECT ALL --" });
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $("select[name=Position],[name=MutaDate]").on("change", refreshGrid);

        setTimeout(refreshGrid, 500);
    });
    function refreshGrid() {
        widget.kgrid({
            url: "ab.api/inquiry/sfmmutation",
            name: "SfmMutation",
            params: $("#pnlFilter").serializeObject(),
            pageable: false,
            pageSize: 100,
            columns: [
                { field: "BranchName", title: "Branch (Outlet)", width: 380 },
                {
                    field: "Muta01", title: "Awal", width: 80,
                    template: "<div class='right'>#=Muta01#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta02", title: "Join", width: 80,
                    template: "<div class='right'>#=Muta02#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta03", title: "Mutasi (In)", width: 90,
                    template: "<div class='right'>#=Muta03#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta04", title: "Resign", width: 80,
                    template: "<div class='right'>#=Muta04#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta05", title: "Mutasi (Out)", width: 100,
                    template: "<div class='right'>#=Muta05#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta06", title: "Akhir", width: 80,
                    template: "<div class='right'>#=Muta06#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
            ],
            aggregate: [
                   { field: "Muta01", aggregate: "sum" },
                   { field: "Muta02", aggregate: "sum" },
                   { field: "Muta03", aggregate: "sum" },
                   { field: "Muta04", aggregate: "sum" },
                   { field: "Muta05", aggregate: "sum" },
                   { field: "Muta06", aggregate: "sum" },
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
            name: "SfmMutation",
            type: "kgrid",
            fileName: "pers_mutation",
            items: [
                { name: "BranchName", text: "Branch" },
                { name: "Muta01", text: "Awal" },
                { name: "Muta02", text: "Join" },
                { name: "Muta03", text: "Mutasi (In)" },
                { name: "Muta04", text: "Resign" },
                { name: "Muta05", text: "Mutasi (Out)" },
                { name: "Muta06", text: "Akhir" },
            ]
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
