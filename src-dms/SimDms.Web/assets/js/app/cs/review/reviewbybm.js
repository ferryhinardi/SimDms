$(document).ready(function () {
    var options = {
        title: "Reviews by Branch Managers",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Input Date",
                        type: "controls", items: [
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                        ],
                    },
                ],
            },
            {
                name: "Review",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });

        setTimeout(refreshGrid, 1000);
    });
    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    $("#pnlFilter [name=DateFrom],#pnlFilter [name=DateTo]").on("change", refreshGrid);
    widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
    $("[name='BranchCode']").on("change", refreshGrid);

    function refreshGrid() {
        var params = {
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val()),
            BranchCode: $("[name='BranchCode']").val()
        }
        widget.kgrid({
            url: "cs.api/review/Reviews",
            name: "Review",
            params: params,
            columns: [
                { field: "ReviewDate", title: "Review Date", width: "150px", template: "#= (ReviewDate == undefined) ? '' : moment(ReviewDate).format('DD MMM YYYY') #" },
                { field: "Plan", title: "Plan", width: "150px" },
                { field: "Do", title: "Do", width: "200px" },
                { field: "Check", title: "Check", width: "300px" },
                { field: "Action", title: "Action", width: "300px" },
                { field: "PIC", title: "PIC" }
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "Review",
            type: "kgrid",
            items: [
                { field: "ReviewDate", title: "Review Date", width: "100px", template: "#= (ReviewDate == undefined) ? '' : moment(ReviewDate).format('DD MMM YYYY') #" },
                { field: "Plan", title: "Plan", width: "150px" },
                { field: "Do", title: "Do", width: "200px" },
                { field: "Check", title: "Check", width: "300px" },
                { field: "Action", title: "Action", width: "300px" },
                { field: "PIC", title: "PIC" }
            ]
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
