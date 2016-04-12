$(document).ready(function () {
    var options = {
        title: "Personal Information",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span6", type: "select" },
                        ]
                    },
                    {
                        text: "Outlet Name",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Position",
                        type: "controls",
                        items: [
                            { name: "Position", text: "Position", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                            {
                                name: "Status", text: "Status", cls: "span2", type: "select", opt_text: "ALL STATUS",
                                items: [
                                    { text: "AKTIF", value: "1" },
                                    { text: "NON AKTIF", value: "2" },
                                    { text: "KELUAR", value: "3" },
                                    { text: "PENSIUN", value: "4" },
                                ]
                            },
                        ]
                    },
                ],
            },
            { name: "SfmPersInfo", xtype: "k-grid", },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/ComboDealerList?LinkedModule=mp", optionalText: "-- SELECT ONE --" }]);
    widget.default = { Status: "1" };
    widget.render(function () {
        widget.populate(widget.default);
        $("#pnlFilter [name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("#pnlFilter [name=CompanyCode]").val(), dept: "SALES" }, optionalText: "-- SELECT ALL --" });
        });
    });
    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    // $("#pnlFilter select").on("change", refreshGrid);

    function refreshGrid() {
        var params = {
            CompanyCode: $("[name=CompanyCode]").val(),
            Branch: $("[name=BranchCode]").val(),
            Position: $("[name=Position]").val(),
            Status: $("[name=Status]").val(),
        }
        widget.kgrid({
            url: "wh.api/inquiry/sfmpersinfo",
            name: "SfmPersInfo",
            params: params,
            columns: [
                { field: "BranchName", title: "Branch (Outlet)", width: 380 },
                { field: "EmployeeID", title: "Empl ID", width: 100 },
                { field: "SalesID", title: "Sales ID", width: 100 },
                { field: "EmployeeName", title: "Employee Name", width: 250 },
                { field: "Position", title: "Position", width: 300 },
                { field: "Status", title: "Status", width: 120 },
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "SfmPersInfo",
            type: "kgrid",
            items: [
                { name: "BranchCode", text: "Outlet", width: 380 },
                { name: "EmployeeID", text: "Empl ID", width: 100 },
                { name: "SalesID", text: "Sales ID", width: 100 },
                { name: "EmployeeName", text: "Employee Name", width: 250 },
                { name: "Position", text: "Position", width: 300 },
                { name: "Status", text: "Status", width: 120 },
            ]
        });
    }
});
