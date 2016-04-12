$(document).ready(function () {
    var options = {
        title: "Inquiry - BPKB Reminder",
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
                    },
                ],
            },
            {
                name: "pnlKGrid",
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
        widget.selectparam({
            name: "BranchCode", url: "wh.api/combo/branchs",
            params: [{ name: "CompanyCode", param: "comp" }],
            optionalText: "-- SELECT ALL --"
        });
    });
    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    $("[name=BranchCode],[name=DateFrom],[name=DateTo]").on("change", refreshGrid);

    function refreshGrid() {
        var params = {
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val()),
            CompanyCode: $("[name=CompanyCode]").val(),
            BranchCode: $("[name=BranchCode]").val()
        }
        widget.kgrid({
            url: "wh.api/inquiry/BpkbReminder",
            name: "pnlKGrid",
            params: params,
            columns: [
                { field: "CreatedDate", title: "Input Date", width: 180, template: "#= (CreatedDate == undefined) ? '' : moment(CreatedDate).format('YYYY-MM-DD  HH:mm:ss') #" },
                { field: "BranchCode", title: "Branch Code", width: 120 },
                { field: "CustomerCode", title: "Cust Code", width: 120 },
                { field: "CustomerName", title: "Customer Name", width: 280 },
                { field: "BpkbReadyDate", title: "Bpkb Ready", width: 120, template: "#= (BpkbReadyDate == undefined) ? '' : moment(BpkbReadyDate).format('DD MMM YYYY') #" },
                { field: "BpkbPickUp", title: "Bpkb Pickup", width: 120, template: "#= (BpkbPickUp == undefined) ? '' : moment(BpkbPickUp).format('DD MMM YYYY') #" },
                { field: "Address", title: "Address", width: 900 },
                { field: "PhoneNo", title: "PhoneNo", width: 180 },
                { field: "HPNo", title: "HPNo", width: 180 },
                { field: "AddPhone1", title: "Add Phone 1", width: 180 },
                { field: "AddPhone2", title: "Add Phone 2", width: 180 },
                { field: "LeasingDesc", title: "Leasing/Cash", width: 120 },
                { field: "LeasingName", title: "Finance Institution", width: 380 },
                { field: "Tenor", title: "Tenor Credit", width: 140 },
                { field: "StnkExtend", title: "Stnk Extension", width: 120 },
                { field: "ReqStnkDesc", title: "Stnk Requirement", width: 140 },
                { field: "SalesModelCode", title: "Car Type", width: 180 },
                { field: "ColourCode", title: "Car Color", width: 100 },
                { field: "PoliceRegNo", title: "Police No", width: 120 },
                { field: "Engine", title: "Engine", width: 160 },
                { field: "Chassis", title: "Chassis", width: 180 },
                { field: "SalesmanName", title: "Salesman", width: 240 },
                { field: "Comment", title: "Customer Comments", width: 700 },
                { field: "Additional", title: "Additional Inquiries", width: 300 },
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "pnlKGrid",
            type: "kgrid",
            fileName: "bpkb_reminder",
            items: [
                { name: "BranchCode", text: "Branch Code", width: 120, type: "text" },
                { name: "CustomerCode", text: "Cust Code", width: 120, type: "text" },
                { name: "CustomerName", text: "Customer Name", width: 280 },
                { name: "Address", text: "Address", width: 900 },
                { name: "PhoneNo", text: "PhoneNo", width: 180, type: "text" },
                { name: "HPNo", text: "HPNo", width: 180, type: "text" },
                { name: "AddPhone1", text: "Add Phone 1", width: 180, type: "text" },
                { name: "AddPhone2", text: "Add Phone 2", width: 180, type: "text" },
                { name: "BpkbReadyDate", text: "Bpkb Ready", width: 120, type: "date" },
                { name: "BpkbPickUp", text: "Bpkb Pickup", width: 120, type: "date" },
                { name: "LeasingDesc", text: "Leasing/Cash", width: 120 },
                { name: "LeasingName", text: "Finance Institution", width: 380, type: "text" },
                { name: "Tenor", text: "Tenor Credit", width: 140 },
                { name: "StnkExtend", text: "Stnk Extension", width: 120 },
                { name: "ReqStnkDesc", text: "Stnk Requirement", width: 140 },
                { name: "SalesModelCode", text: "Car Type", width: 180 },
                { name: "ColourCode", text: "Car Color", width: 100 },
                { name: "PoliceRegNo", text: "Police No", width: 120, type: "text" },
                { name: "Engine", text: "Engine", width: 160 },
                { name: "Chassis", text: "Chassis", width: 180 },
                { name: "SalesmanName", text: "Salesman", width: 240 },
                { name: "Comment", text: "Customer Comments", width: 700 },
                { name: "Additional", text: "Additional Inquiries", width: 300 },
            ]
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
