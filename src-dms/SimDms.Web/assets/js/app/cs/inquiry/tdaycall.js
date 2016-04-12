$(document).ready(function () {
    var options = {
        title: "Inquiry - Rekap 3 Days Call",
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
                name: "TDayCall",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });

        setTimeout(refreshGrid, 1000);
    });
    $(document).on("click", "#btnRefresh", refreshGrid);
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
            url: "cs.api/inquiry/CsTDaysCall",
            name: "TDayCall",
            params: params,
            columns: [
                { field: "CreatedDate", title: "Input Date", width: 150, template: "#= (CreatedDate == undefined) ? '' : moment(CreatedDate).format('DD MMM YYYY') #" },
                { field: "DeliveryDate", title: "Delivery Date", width: 120, template: "#= (DeliveryDate == undefined) ? '' : moment(DeliveryDate).format('DD MMM YYYY') #" },
                { field: "BranchCode", title: "Branch Code", width: 150 },
                { field: "CustomerCode", title: "Cust. Code", width: 120 },
                { field: "CustomerName", title: "Customer Name", width: 280 },
                { field: "Address", title: "Address", width: 700 },
                { field: "PhoneNo", title: "PhoneNo", width: 180 },
                { field: "HPNo", title: "HPNo", width: 180 },
                { field: "AddPhone1", title: "Additional Phone 1", width: 180 },
                { field: "AddPhone2", title: "Additional Phone 2", width: 180 },
                { field: "BirthDate", title: "Birth Date", width: 120, template: "#= (BirthDate == undefined) ? '' : moment(BirthDate).format('DD MMM YYYY') #" },
                { field: "Religion", title: "Religion", width: 100 },
                { field: "CarType", title: "Car Type", width: 580 },
                { field: "Color", title: "Color", width: 280 },
                { field: "PoliceRegNo", title: "Police No", width: 100 },
                { field: "Engine", title: "Engine", width: 140 },
                { field: "Chassis", title: "Chassis", width: 180 },
                { field: "Salesman", title: "Salesman", width: 280 },
                { field: "IsDeliveredA", title: "Penjelasan Isi Buku Manual", width: 250 },
                { field: "IsDeliveredB", title: "Penjelasan Fitur keamanan", width: 250 },
                { field: "IsDeliveredC", title: "Penjelasan Jadwal servis berkala", width: 280 },
                { field: "IsDeliveredD", title: "Penjelasan Garansi", width: 200 },
                { field: "IsDeliveredE", title: "Kartu nama PIC Servis / bengkel", width: 280 },
                { field: "IsDeliveredF", title: "Customer Feedback Card", width: 240 },
                { field: "IsDeliveredG", title: "Thank You Letter", width: 240 },
                { field: "Comment", title: "Customer comments", width: 1500 },
                { field: "Additional", title: "Additional inquiries", width: 500 },
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "TDayCall",
            type: "kgrid",
            items: [
                { name: "CreatedDate", type: "date", text: "Input Date", width: 120, template: "#= (CreatedDate == undefined) ? '' : moment(CreatedDate).format('DD MMM YYYY') #" },
                { name: "DeliveryDate", type: "date", text: "Delivery Date", width: 120, template: "#= (DeliveryDate == undefined) ? '' : moment(DeliveryDate).format('DD MMM YYYY') #" },
                { name: "BranchCode", text: "Branch Code", width: 150 },
                { name: "CustomerCode", text: "Cust. Code", width: 120 },
                { name: "CustomerName", text: "Customer Name", width: 280 },
                { name: "Address", text: "Address", width: 700 },
                { name: "PhoneNo", type: "text", text: "PhoneNo", width: 180, type: "text" },
                { name: "HPNo", type: "text", text: "HPNo", width: 180, type: "text" },
                { name: "AddPhone1", type: "text", text: "Additional Phone 1", width: 180, type: "text" },
                { name: "AddPhone2", type: "text", text: "Additional Phone 2", width: 180, type: "text" },
                { name: "BirthDate", type: "date", text: "Birth Date", width: 120, template: "#= (BirthDate == undefined) ? '' : moment(BirthDate).format('DD MMM YYYY') #" },
                { name: "Religion", text: "Religion", width: 100 },
                { name: "CarType", text: "Car Type", width: 180 },
                { name: "Color", text: "Color", width: 80 },
                { name: "PoliceRegNo", type: "text", text: "Police No", width: 100 },
                { name: "Engine", type: "text", text: "Engine", width: 140 },
                { name: "Chassis", type: "text", text: "Chassis", width: 180 },
                { name: "Salesman", text: "Salesman", width: 280 },
                { name: "IsDeliveredA", text: "Penjelasan Isi Buku Manual", width: 250 },
                { name: "IsDeliveredB", text: "Penjelasan Fitur keamanan", width: 250 },
                { name: "IsDeliveredC", text: "Penjelasan Jadwal servis berkala", width: 280 },
                { name: "IsDeliveredD", text: "Penjelasan Garansi", width: 200 },
                { name: "IsDeliveredE", text: "Kartu nama PIC Servis / bengkel", width: 280 },
                { name: "IsDeliveredF", text: "Customer Feedback Card", width: 240 },
                { name: "IsDeliveredG", text: "Thank You Letter", width: 240 },
                { name: "Comment", text: "Customer comments", width: 500 },
                { name: "Additional", text: "Additional inquiries", width: 500 },
            ]
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
