$(document).ready(function () {
    var options = {
        title: "Inquiry - Customer Holiday",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "DateFrom", text: "Date From", cls: "span3", type: "datepicker" },
                    { name: "DateTo", text: "Date To", cls: "span3", type: "datepicker" },
                ],
            },
            {
                name: "TDayCall",
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
    });

    $("#btnRefresh").on("click", function () {
        refreshGrid();
    });

    function refreshGrid() {
        var params = {
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val())
        }
        widget.kgrid({
            url: "cs.api/inquiry/tdaycall",
            name: "TDayCall",
            params: params,
            columns: [
                { field: "CustomerCode", title: "ID Cust", width: 120 },
                { field: "CustomerName", title: "Customer Name", width: 280 },
                { field: "Address1", title: "Address1", width: 360 },
                { field: "Address2", title: "Address2", width: 300 },
                { field: "PhoneNo", title: "PhoneNo", width: 180 },
                { field: "HPNo", title: "HPNo", width: 180 },
                { field: "CarType", title: "Car TYpe", width: 180 },
                { field: "Color", title: "Color", width: 80 },
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
                { field: "Comment", title: "Customer comments", width: 240 },
                { field: "Additional", title: "Additional inquiries", width: 240 },
            ],
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
