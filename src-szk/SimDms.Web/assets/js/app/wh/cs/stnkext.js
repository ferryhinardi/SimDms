$(document).ready(function () {
    var options = {
        title: "Inquiry - STNK Extension",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    {
                        name: "IsStnkExt", text: "Stnk Extension", type: "select", cls: "span6", items: [
                            { value: "", text: "-- SELECT ALL --" },
                            { value: "1", text: "Ya" },
                            { value: "0", text: "Tidak" },
                        ],
                        fullItem: true
                    },
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
                name: "StnkExt",
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
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.render(function () {
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);

        widget.populate({ DateFrom: date2, DateTo: date1 });
        $("[name=GroupArea]").on("change", function () {
            //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            //widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { companyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });

        $('#CompanyCode').attr('disabled', 'disabled');
        $('#BranchCode').attr('disabled', 'disabled');
    });

    $('#GroupArea').on('change', function () {
        if ($('#GroupArea').val() != "") {
            $('#CompanyCode').removeAttr('disabled');
        } else {
            $('#CompanyCode').attr('disabled', 'disabled');
            $('#BranchCode').attr('disabled', 'disabled');
            $('#CompanyCode').select2('val', "");
            $('#BranchCode').select2('val', "");
        }
        $('#CompanyCode').select2('val', "");
        $('#BranchCode').select2('val', "");
    });

    $('#CompanyCode').on('change', function () {
        if ($('#CompanyCode').val() != "") {
            $('#BranchCode').removeAttr('disabled');
        } else {
            $('#BranchCode').attr('disabled', 'disabled');
        }
        $('#BranchCode').select2('val', "");
    });

    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);

    function refreshGrid() {
        var params = {
            DateFrom: getSqlDate($("[name=DateFrom]").val()),
            DateTo: getSqlDate($("[name=DateTo]").val()),
            GroupArea: $("[name=GroupArea]").val(),
            CompanyCode: $("[name=CompanyCode]").val(),
            BranchCode: $("[name=BranchCode]").val(),
            StnkExt: $("[name=IsStnkExt]").val()
        }
        widget.kgrid({
            url: "wh.api/inquiry/StnkExtension",
            name: "StnkExt",
            params: params,
            columns: [
                { field: "InputDate", title: "Input Date", width: 180, template: "#= (InputDate == undefined) ? '' : moment(InputDate).format('YYYY-MM-DD  HH:mm:ss') #" },
                { field: "StnkDate", title: "Tgl Stnk", width: 120, template: "#= (StnkDate == undefined) ? '' : moment(StnkDate).format('DD MMM YYYY') #" },
                { field: "BranchCode", title: "Branch Code", width: 120 },
                { field: "CustomerCode", title: "Cust Code", width: 120 },
                { field: "CustomerName", title: "Customer Name", width: 280 },
                { field: "Address", title: "Address", width: 900 },
                { field: "PhoneNo", title: "PhoneNo", width: 180 },
                { field: "HPNo", title: "HPNo", width: 180 },
                { field: "AddPhone1", title: "Add Phone 1", width: 180 },
                { field: "AddPhone2", title: "Add Phone 2", width: 180 },
                { field: "BpkbDate", title: "Tgl Bpkb", width: 120, template: "#= (BpkbDate == undefined) ? '' : moment(BpkbDate).format('DD MMM YYYY') #" },
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
            name: "StnkExt",
            type: "kgrid",
            fileName: "STNK_Extension",
            items: [
                { name: "BranchCode", text: "Branch Code", width: 120, type: "text" },
                { name: "CustomerCode", text: "Cust Code", width: 120, type: "text" },
                { name: "CustomerName", text: "Customer Name", width: 280 },
                { name: "Address", text: "Address", width: 900 },
                { name: "PhoneNo", text: "PhoneNo", width: 180, type: "text" },
                { name: "HPNo", text: "HPNo", width: 180, type: "text" },
                { name: "AddPhone1", text: "Add Phone 1", width: 180, type: "text" },
                { name: "AddPhone2", text: "Add Phone 2", width: 180, type: "text" },
                { name: "BpkbDate", text: "Tgl Bpkb", width: 120, type: "date" },
                { name: "StnkDate", text: "Tgl Stnk", width: 120, type: "date" },
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
