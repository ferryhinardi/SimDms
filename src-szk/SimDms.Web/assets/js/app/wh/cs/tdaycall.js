$(document).ready(function () {
    var options = {
        title: "Inquiry - Rekap 3 Days Call",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "CompanyCode", text: "Dealer", type: "select", cls: "span6" },
                    { name: "BranchCode", text: "Outlet", type: "select", cls: "span6" },
                    {
                        text: "Input Date",
                        type: "controls", items: [
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                        ]
                    }
                ],
            },
            {
                name: "TDayCall",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { name: "btnExportXls", action: 'export', text: "Export (xls)", icon: "fa fa-file-excel-o" },
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
        widget.selectparam({
            name: "BranchCode", url: "wh.api/combo/branchs",
            params: [{ name: "CompanyCode", param: "comp" }],
            optionalText: "-- SELECT ALL --"
        });
    });
    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    $("#pnlFilter [name=BranchCode],#pnlFilter [name=DateFrom],#pnlFilter [name=DateTo]").on("change", refreshGrid);

    function refreshGrid() {
        var params = {
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val()),
            CompanyCode: $("[name=CompanyCode]").val(),
            BranchCode: $("[name=BranchCode]").val()
        }
        widget.kgrid({
            url: "wh.api/inquiry/Cs3DaysCall",
            name: "TDayCall",
            params: params,
            columns: [
                { field: "CreatedDate", title: "Input Date", width: 180, template: "#= (CreatedDate == undefined) ? '' : moment(CreatedDate).format('YYYY-MM-DD  HH:mm:ss') #" },
                { field: "DeliveryDate", title: "Delivery Date", width: 110, template: "#= (DeliveryDate == undefined) ? '' : moment(DeliveryDate).format('YYYY-MM-DD') #" },
                { field: "BranchCode", title: "Branch Code", width: 120 },
                { field: "CustomerCode", title: "Cust Code", width: 120 },
                { field: "CustomerName", title: "Customer Name", width: 380 },
                { field: "Address", title: "Address", width: 700 },
                { field: "PhoneNo", title: "PhoneNo", width: 180 },
                { field: "HPNo", title: "HPNo", width: 180 },
                { field: "AddPhone1", title: "Add Phone 1", width: 180 },
                { field: "AddPhone2", title: "Add Phone 2", width: 180 },
                { field: "BirthDate", title: "Birth Date", width: 110, template: "#= (BirthDate == undefined) ? '' : moment(BirthDate).format('YYYY-MM-DD') #" },
                { field: "CarType", title: "Car TYpe", width: 180 },
                { field: "Color", title: "Color", width: 80 },
                { field: "PoliceRegNo", title: "Police No", width: 100 },
                { field: "Engine", title: "Engine", width: 140 },
                { field: "Chassis", title: "Chassis", width: 180 },
                { field: "Salesman", title: "Salesman", width: 280 },
                { field: "IsDeliveredA", title: "Penjelasan Isi Buku Manual", width: 200 },
                { field: "IsDeliveredB", title: "Penjelasan Fitur keamanan", width: 200 },
                { field: "IsDeliveredC", title: "Penjelasan Jadwal servis berkala", width: 240 },
                { field: "IsDeliveredD", title: "Penjelasan Garansi", width: 180 },
                { field: "IsDeliveredE", title: "Pemberian kartu nama", width: 180 },
                { field: "IsDeliveredF", title: "Customer Feedback Card", width: 190 },
                { field: "IsDeliveredG", title: "Thank you letter", width: 160 },
                { field: "Comment", title: "Customer comments", width: 1200 },
                { field: "Additional", title: "Additional inquiries", width: 240 },
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "TDayCall",
            type: "kgrid",
            items: [
                { name: "CreatedDate", text: "Input Date", width: 120, type: "datetime" },
                { name: "DeliveryDate", text: "Delivery Date", width: 100, type: "date" },
                { name: "BranchCode", text: "Branch Code", width: 100 },
                { name: "CustomerCode", text: "Cust Code", width: 120 },
                { name: "CustomerName", text: "Customer Name", width: 280 },
                { name: "Address", text: "Address", width: 900 },
                { name: "PhoneNo", text: "PhoneNo", width: 180, type: "text" },
                { name: "HPNo", text: "HPNo", width: 180, type: "text" },
                { name: "AddPhone1", text: "Add Phone 1", width: 180, type: "text" },
                { name: "AddPhone2", text: "Add Phone 2", width: 180, type: "text" },
                { name: "BirthDate", text: "Birth Date", width: 110, type: "date" },
                { name: "CarType", text: "Car TYpe", width: 180 },
                { name: "Color", text: "Color", width: 80 },
                { name: "PoliceRegNo", text: "Police No", width: 100, type: "text" },
                { name: "Engine", text: "Engine", width: 140 },
                { name: "Chassis", text: "Chassis", width: 180 },
                { name: "Salesman", text: "Salesman", width: 280 },
                { name: "IsDeliveredA", text: "Penjelasan Isi Buku Manual", width: 200 },
                { name: "IsDeliveredB", text: "Penjelasan Fitur keamanan", width: 200 },
                { name: "IsDeliveredC", text: "Penjelasan Jadwal servis berkala", width: 240 },
                { name: "IsDeliveredD", text: "Penjelasan Garansi", width: 180 },
                { name: "IsDeliveredE", text: "Pemberian kartu nama", width: 180 },
                { name: "IsDeliveredF", text: "Customer Feedback Card", width: 190 },
                { name: "IsDeliveredG", text: "Thank you letter", width: 160 },
                { name: "Comment", text: "Customer comments", width: 900 },
                { name: "Additional", text: "Additional inquiries", width: 900 },
            ]
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
