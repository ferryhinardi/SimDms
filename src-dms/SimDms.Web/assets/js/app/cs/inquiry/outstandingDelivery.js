$(document).ready(function () {
    var options = {
        title: "Inquiry - Outstanding Delivery",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        name: "BranchCode", type: "select", text: "Outlet", opt_text: "-- SELECT ALL --", cls: "span5"
                    },
                    {
                        text: "Input Date",
                        type: "controls", items: [
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                        ],
                    },
                    {
                        name: "Status", type: "select", text: "Status", opt_text: "-- SELECT ALL --", cls: "span5", items: [
                            { value: "0", text: "Outstanding" },
                            { value: "1", text: "Delivery" },
                        ]
                    },
                ],
            },
            {
                name: "OutDlvry",
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

        widget.post("cs.api/Combo/IsHolding", {}, function (result) {
            if (result)
                widget.select({ selector: "#BranchCode", url: "cs.api/Combo/ListBranchCode" }, function (data) {
                    $("#Status").val("0");
                    refreshGrid();
                });
            else {
                widget.select({ selector: "#BranchCode", url: "cs.api/Combo/CurrentBranchCode" }, function (data) {
                    $("#Status").val("0");
                    if (data.length)
                        $("#BranchCode").val(data[0].value);
                    $("#BranchCode").prop("disabled", true);
                    refreshGrid();
                });
            }
        });

        widget.populate({ DateFrom: date2, DateTo: date1 });

        $(document).on("click", "#btnRefresh", refreshGrid)
                .on("click", "#btnExportXls", exportXls);
    });
    
    function refreshGrid() {
        var params = {
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val()),
            BranchCode: $("[name='BranchCode']").val(),
            Status: $("[name='Status']").val()
        }
        widget.kgrid({
            url: "cs.api/inquiry/OutstandingDelivery",
            name: "OutDlvry",
            params: params,
            columns: [
                { field: "No", title: "No", width: 65 },
                { field: "BranchCode", title: "Branch Code", width: 120 },
                { field: "OutletAbbreviation", title: "Outlet", width: 180 },
                { field: "BPKNo", title: "BPK No", width: 140 },
                { field: "BPKDate", title: "BPK Date", width: 120, template: "#= (BPKDate == undefined) ? '' : moment(BPKDate).format('DD MMM YYYY') #" },
                { field: "DeliveryDate", title: "Delivery Date", width: 140, template: "#= (DeliveryDate == undefined || DeliveryDate == '') ? '' : moment(DeliveryDate).format('DD MMM YYYY') #" },
                { field: "CustomerCode", title: "Customer Code", width: 140 },
                { field: "CustomerName", title: "Customer Name", width: 250 },
                { field: "SalesModelCode", title: "Model Code", width: 180 },
                { field: "SalesModelYear", title: "Model Year", width: 120 },
                { field: "ChassisCode", title: "Chassis Code", width: 120 },
                { field: "ChassisNo", title: "ChassisNo", width: 120 },
                { field: "EngineCode", title: "Engine Code", width: 100 },
                { field: "EngineNo", title: "Engine No", width: 120 },
            ],
        });
    }

    function exportXls() {
        var params = {
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val()),
            BranchCode: $("[name='BranchCode']").val(),
            Status: $("[name='Status']").val()
        }
        widget.post("cs.api/excel/Printxls", params, function (response) {
            if (response.message == "") {
                location.href = 'cs.api/excel/DownloadExcelFile?key=' + response.value + '&filename=Outstanding Delivery';
            } else {
                sdms.info(response.message, "Error");
            }
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});