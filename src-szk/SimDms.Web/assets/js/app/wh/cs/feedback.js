$(document).ready(function () {
    var options = {
        title: "Inquiry - Customer Feedback",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        name: "CompanyCode",
                        text: "Dealer",
                        type: "select",
                        cls: "span6"
                    },
                    {
                        text: "Delivery Date",
                        type: "controls", items: [
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                        ]
                    }
                ],
            },
            {
                name: "Feedback",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { name: "btnExportXls", action: 'export', text: "Export (Xls)", icon: "fa fa-file-excel-o" }
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'export', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'export', 'collapse']);
                    break;
                default:
                    break;
            }
        },
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);

        widget.populate({ DateFrom: date2, DateTo: date1 });
        widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList?LinkedModule=CS" });

        setTimeout(refreshGrid, 1000);
    });
    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    $("#pnlFilter [name=DateFrom],#pnlFilter [name=DateTo]").on("change", refreshGrid);

    function refreshGrid() {
        var params = {
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val()),
            CompanyCode: $("[name=CompanyCode]").val()
        }
        widget.kgrid({
            url: "wh.api/inquiry/CsFeedback",
            name: "Feedback",
            //serverBinding: true,
            pageSize: 10,
            params: params,
            columns: [
                { field: "CustomerCode", title: "ID Cust", width: 100 },
                { field: "CustomerName", title: "Customer Name", width: 280 },
                { field: "Address", title: "Address", width: 700 },
                { field: "HPNo", title: "Telephon", width: 180 },
                { field: "SalesModelYear", title: "Model Year", width: 100 },
                { field: "DODate", title: "Delivery Date", width: 120, template: "#= (DODate == undefined) ? '' : moment(DODate).format('DD MMM YYYY') #" },
                { field: "PoliceRegNo", title: "No Polisi", width: 180 },
                { field: "Chassis", title: "Vin No", width: 180 },
                { field: "SalesModelCode", title: "Sales Model", width: 160 },
                { field: "Feedback", title: "Feedback", width: 120 },
                { field: "Feedback01", title: "Feedback 01", width: 120 },
                { field: "Feedback02", title: "Feedback 02", width: 120 },
                { field: "Feedback03", title: "Feedback 03", width: 120 },
                { field: "Feedback04", title: "Feedback 04", width: 120 },
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "Feedback",
            type: "kgrid",
            fileName: "cust_feedback",
            items: [
                { name: "CustomerCode", text: "ID Cust", width: 100 },
                { name: "CustomerName", text: "Customer Name", width: 280 },
                //{ name: "Address", text: "Address", width: 700 },
                { name: "HPNo", text: "Telephon", width: 180 },
                { name: "SalesModelYear", text: "Model Year", width: 100 },
                { name: "DODate", text: "Delivery Date", width: 120, type: "date" },
                { name: "PoliceRegNo", text: "No Polisi", width: 180 },
                { name: "Chassis", text: "Vin No", width: 180 },
                { name: "SalesModelCode", text: "Sales Model", width: 160 },
                { name: "Feedback", text: "Feedback", width: 120 },
                { name: "Feedback01", text: "Feedback 01", width: 120 },
                { name: "Feedback02", text: "Feedback 02", width: 120 },
                { name: "Feedback03", text: "Feedback 03", width: 120 },
                { name: "Feedback04", text: "Feedback 04", width: 120 },
            ]
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
