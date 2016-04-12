$(document).ready(function () {
    var options = {
        title: "Reviews",
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
                    { name: "BranchCode", id: "BranchCode", text: "Branch/Outlet", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                ],
            },
            {
                name: "Review",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });
        var params = "";
        widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
        $.ajax({
            type: "POST",
            url: 'cs.api/review/Branchs',
            dataType: 'json',
            data: params
                , success: function (response) {
                    if (response.success) {
                        $("[name='BranchCode']").val(response.Branch);
                        $("[name='BranchCode']").attr('disabled', 'disabled');
                    } else {
                        $("[name='BranchCode']").val();
                    }
                }
        });
        setTimeout(refreshGrid, 1000);
    });
    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    $("#pnlFilter [name=BranchCode],#pnlFilter [name=DateFrom],#pnlFilter [name=DateTo]").on("change", refreshGrid);

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
                { field: "BranchCode", title: "OutletCode", hide: true },
                { field: "OutletAbbreviation", title: "Outlet", width: "200px" },
                { field: "DateFrom", title: "Date From", width: "150px", template: "#= (DateFrom == undefined) ? '' : moment(DateFrom).format('DD MMM YYYY') #" },
                { field: "DateTo", title: "Date To", width: "150px", template: "#= (DateTo == undefined) ? '' : moment(DateTo).format('DD MMM YYYY') #" },
                { field: "Plan", title: "Plan", width: "150px" },
                { field: "Do", title: "Do", width: "200px" },
                { field: "Check", title: "Check", width: "300px" },
                { field: "Action", title: "Action", width: "300px" },
                { field: "PIC", title: "PIC" },
                { field: "CommentbyGM", title: "Comment by GM", width: "300px" },
                { field: "CommentbySIS", title: "Comment by SIS", width: "300px" }
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "Review",
            type: "kgrid",
            items: [
                { field: "BranchCode", title: "OutletCode", hide: true },
                { field: "OutletAbbreviation", title: "Outlet", width: "200px" },
                { field: "DateFrom", title: "Date From", width: "150px", template: "#= (DateFrom == undefined) ? '' : moment(DateFrom).format('DD MMM YYYY') #" },
                { field: "DateTo", title: "Date To", width: "150px", template: "#= (DateTo == undefined) ? '' : moment(DateTo).format('DD MMM YYYY') #" },
                { field: "Plan", title: "Plan", width: "150px" },
                { field: "Do", title: "Do", width: "200px" },
                { field: "Check", title: "Check", width: "300px" },
                { field: "Action", title: "Action", width: "300px" },
                { field: "PIC", title: "PIC" },
                { field: "CommentbyGM", title: "Comment by GM", width: "300px" },
                { field: "CommentbySIS", title: "Comment by SIS", width: "300px" }
            ]
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});