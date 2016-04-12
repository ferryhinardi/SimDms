$(document).ready(function () {
    var options = {
        title: "Man Power Dashboard",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Dealer", cls: "span7", type: "select" },
                        ]
                    },
                    {
                        text: "Outlet Name",
                        type: "controls",
                        items: [
                            { name: "BranchCode", text: "Outlet", cls: "span7", type: "select" },
                        ]
                    },
                    {
                        text: "Periode",
                        type: "controls",
                        items: [
                            { name: "Periode", text: "Periode", cls: "span2", type: "datepicker" },
                        ]
                    },
                ],
            },
            {
                name: "InqPers",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/DealerList", optionalText: "-- SELECT ONE --", params: { LinkedModule: "MP" } }]);
    widget.default = { Status: "1" };
    widget.render(function () {
        renderCallback();
    });

    function renderCallback() {
        widget.populate(widget.default);

        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/Branchs", params: { comp: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=Department]", url: "wh.api/combo/departments", params: { comp: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Department]").change();
        });
        $("[name=Department]").on("change", function () {
            widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("[name=CompanyCode]").val(), dept: $("[name=Department]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Position]").change();
        });
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $('[name="Periode"]').val(moment(new Date()).format('DD-MMM-YYYY'));
    }

    function refreshGrid() {
        if (widget.isNullOrEmpty($('[name="CompanyCode"]').val()) == false) {
            widget.kgrid({
                url: "wh.api/inquiry/MpDashboard",
                name: "InqPers",
                params: $("#pnlFilter").serializeObject(),
                //serverBinding: true,
                columns: [
                    { field: "BranchName", width: 500, title: "Outlet" },
                    { field: "BranchManager", width: 140, title: "Branch Manager", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SalesHead", width: 140, title: "Sales Head", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SalesCoordinator", width: 140, title: "Sales Coordinator", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "Salesman", width: 140, title: "Salesman", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SalesmanPlatinum", width: 140, title: "Salesman Platinum", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SalesmanGold", width: 140, title: "Salesman Gold", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SalesmanSilver", width: 140, title: "Salesman Silver", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SalesmanTrainee", width: 140, title: "Salesman Trainee", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "TotalSalesForce", width: 140, title: "Total Sales Force", footerTemplate: '#=sum#', type: 'align-right' },
                ],
                aggregate: [
                    { field: "BranchManager", aggregate: "sum" },
                    { field: "SalesHead", aggregate: "sum" },
                    { field: "SalesCoordinator", aggregate: "sum" },
                    { field: "Salesman", aggregate: "sum" },
                    { field: "SalesmanPlatinum", aggregate: "sum" },
                    { field: "SalesmanGold", aggregate: "sum" },
                    { field: "SalesmanSilver", aggregate: "sum" },
                    { field: "SalesmanTrainee", aggregate: "sum" },
                    { field: "TotalSalesForce", aggregate: "sum" },
                ],
            });
        }
    }

    function exportXls() {
        console.log('Exporting xls ...');
    }
});
