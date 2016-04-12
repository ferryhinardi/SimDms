$(document).ready(function () {
    var options = {
        title: "Inquiry - STNK Extention",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    //{ name: "BranchCode", type: "select", text: "Outlet", cls: "span5", opt_text: "-- SELECT ALL --" },
                    {
                        name: "IsStnkExtension", type: "select", text: "Is STNK Ext.", opt_text: "-- SELECT ALL --", cls: "span5", items: [
                            { value: "1", text: "Ya" },
                            { value: "0", text: "Tidak" },
                        ]
                    },
                    {
                        text: "STNK Expired Date",
                        type: "controls", items: [
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                        ]
                    }
                ],
            },
            {
                name: "StnkExt",
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
    $("[name='BranchCode'], [name='IsStnkExtension'], [name='BranchCode']").on("change", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
    $("#pnlFilter [name=DateFrom],#pnlFilter [name=DateTo]").on("change", refreshGrid);

    function refreshGrid() {
        var params = {
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val()),
            BranchCode: $("[name='BranchCode']").val(),
            IsStnkExtension: $("[name='IsStnkExtension']").val(),
            Outstanding: 'N'
        }
        widget.kgrid({
            url: "cs.api/inquiry/CsStnkExtension",
            name: "StnkExt",
            params: params,
            columns: [
                { field: "CustomerCode", title: "ID Cust", width: 100 },
                { field: "CustomerName", title: "Customer Name", width: 280 },
                { field: "Address", title: "Address", width: 700 },
                { field: "PhoneNo", title: "Telephone", width: 180 },
                { field: "HpNo", title: "Handphone", width: 180 },
                { field: "BpkbDate", title: "Tgl Bpkb", width: 120, template: "#= (BpkbDate == undefined) ? '' : moment(BpkbDate).format('DD MMM YYYY') #" },
                { field: "StnkDate", title: "Tgl Stnk", width: 120, template: "#= (StnkDate == undefined) ? '' : moment(StnkDate).format('DD MMM YYYY') #" },
                { field: "CustCtgDesc", title: "Leasing/Cash", width: 120 },
                { field: "LeasingDesc", title: "Finance Institution", width: 380 },
                { field: "Tenor", title: "Tenor Credit", width: 140 },
                { field: "StnkExtend", title: "Stnk Extension", width: 120 },
                { field: "ReqStnkDesc", title: "Stnk Requirement", width: 140 },
                { field: "SalesModelDesc", title: "Car Type", width: 300 },
                { field: "ColourDesc", title: "Car Color", width: 200 },
                { field: "PoliceRegNo", title: "Police No", width: 120 },
                { field: "Engine", title: "Engine", width: 160 },
                { field: "Chassis", title: "Chassis", width: 180 },
                { field: "SalesmanName", title: "Salesman", width: 240 },
                { field: "Comment", title: "Customer Comments", width: 300 },
                { field: "Additional", title: "Additional Inquiries", width: 300 },
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "StnkExt",
            type: "kgrid",
            items: [
                { name: "CustomerCode", text: "ID Cust", width: 100 },
                { name: "CustomerName", text: "Customer Name", width: 280 },
                //{ name: "Address", text: "Address", width: 700 },
                { name: "PhoneNo", type: "text", text: "Telephone", width: 180 },
                { name: "HpNo", type: "text", text: "Handphone", width: 180 },
                { name: "BpkbDate", text: "Tgl Bpkb", width: 120, type: "date" },
                { name: "StnkDate", text: "Tgl Stnk", width: 120, type: "date" },
                { name: "CustCtgDesc", text: "Leasing/Cash", width: 120 },
                { name: "LeasingDesc", text: "Finance Institution", width: 380 },
                { name: "Tenor", text: "Tenor Credit", width: 140 },
                { name: "StnkExtend", type: "text", text: "Stnk Extension", width: 120 },
                { name: "ReqStnkDesc", type: "text", text: "Stnk Requirement", width: 140 },
                { name: "SalesModelDesc", text: "Car Type", width: 300 },
                { name: "ColourDesc", text: "Car Color", width: 200 },
                { name: "PoliceRegNo", text: "Police No", width: 120 },
                { name: "Engine", text: "Engine", width: 160 },
                { name: "Chassis", text: "Chassis", width: 180 },
                { name: "SalesmanName", text: "Salesman", width: 240 },
                { name: "Comment", text: "Customer Comments", width: 300 },
                { name: "Additional", text: "Additional Inquiries", width: 300 },
            ]
        });
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
