$(document).ready(function () {
    var options = {
        title: "Data Trend",
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
                        text: "Position",
                        type: "controls",
                        items: [
                            { name: "Position", text: "Position", cls: "span3", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Periode",
                        type: "controls",
                        items: [
                            { name: "Year", text: "Year", cls: "span3", type: "select", opt_text: "-- SELECT ONE --" },
                            { name: "Month", text: "Month", cls: "span3", type: "select", opt_text: "-- SELECT ONE --" },
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
        var currentYear = (new Date()).getFullYear();
        var currentMonth = (new Date()).getMonth() + 1;

        widget.populate(widget.default);
        widget.select(
            {
                selector: "[name=Position]", url: "wh.api/combo/positions",
                params: { comp: $("[name=CompanyCode]").val(), dept: 'SALES' },
                optionalText: "-- SELECT ALL --"
            });
        widget.select(
            { selector: "[name=Year]", url: "wh.api/combo/MpListOfYear", params: {}, optionalText: "-- SELECT ONE --" },
            function () {
                $('[name="Year"]').val(currentYear);
            }
        );
        widget.select(
            { selector: "[name=Month]", url: "wh.api/combo/ListOfMonth", params: {}, optionalText: "-- SELECT ONE --" },
            function () {
                $('[name="Month"]').val(currentMonth);
            }
        );

        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/Branchs", params: { comp: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
        });
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);

        $('select').on('change', refreshGrid);
    }

    function refreshGrid() {
        if (widget.isNullOrEmpty($('[name="CompanyCode"]').val()) == false) {
            widget.kgrid({
                url: "wh.api/inquiry/MpDataTrend",
                name: "InqPers",
                params: $("#pnlFilter").serializeObject(),
                serverBinding: true,
                columns: [
                    { field: "BranchName", width: 500, title: "Outlet" },
                    { field: "January", width: 100, title: "January" },
                    { field: "February", width: 100, title: "February" },
                    { field: "March", width: 100, title: "March" },
                    { field: "April", width: 100, title: "April" },
                    { field: "May", width: 100, title: "May" },
                    { field: "June", width: 100, title: "June" },
                    { field: "July", width: 100, title: "July" },
                    { field: "August", width: 100, title: "August" },
                    { field: "September", width: 100, title: "September" },
                    { field: "October", width: 100, title: "October" },
                    { field: "November", width: 100, title: "November" },
                    { field: "December", width: 100, title: "December" },
                ],
            });
        }
    }

    function exportXls() {
        console.log('Export Xls');
    }
});
