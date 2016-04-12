var cabang;
var Company;
var Outlet;
var RoleID = "";

$(document).ready(function () {
    var options = {
        title: "Inquiry Indent",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Dealer",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Dealer Name", cls: "span6", type: "select", opt_text: "-- SELECT ALL -- " },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Period",
                        items: [
                            { name: "PeriodYear", id: "PeriodYear", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                            { name: "PeriodMonth", id: "PeriodMonth", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    //{
                    //    text: "Outlet",
                    //    type: "controls",
                    //    cls: 'span8',
                    //    items: [
                    //        { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span6' }
                    //    ]
                    //},
                    // {
                    //     text: "Period Indent From",
                    //     type: "controls",
                    //     cls: "span8",
                    //     items: [
                    //         { name: "DateFrom", cls: "span6", type: "datepicker" }
                    //     ]
                    // },
                    //{
                    //    text: "Period Indent To",
                    //    type: "controls",
                    //    cls: "span8",
                    //    items: [
                    //        { name: "DateTo", cls: "span6", type: "datepicker" }
                    //    ]
                    //},
                ],
            },
            {
                name: "InqPers",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export Quota (xls)", icon: "fa fa-file-excel-o" },
            { name: "btnExportDtlXls", text: "Export Detail Indent (xls)", icon: "fa fa-file-excel-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
    // widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/organizations", optionalText: "-- SELECT ONE --" }]);
    // widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/DealerList", optionalText: "-- SELECT ONE --", params: { LinkedModule: "MP" } }]);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);

    widget.render(function () {
        var filter = {
            DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
            DateTo: new Date(),
            //GroupArea: 100,
            //CompanyCode: "6006400001",
            //Outlet: "6006400106"
        }

        widget.select({ selector: "[name=PeriodYear]", url: "wh.api/combo/years", optionalText: "-- SELECT ALL --" });
        widget.select({ selector: "[name=PeriodMonth]", url: "wh.api/combo/listofmonth", optionalText: "-- SELECT ALL --" });

        $("[name=GroupArea]").on("change", function () {
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/Branches", params: { id: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            //$("[name=Outlet]").prop("selectedIndex", 0);
           // $("[name=Outlet]").change();
        });
        widget.post('wh.api/indent/default', function (r) {
            if (r.data.CompanyCode && r.data.CompanyCode != "") {
                //if (r.data.RoleID == 'DEV-ADMIN' || r.data.RoleID == 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                //    $('#btnExportXls').show();
                //} else {
                //    $('#btnExportXls').hide();
                //}
                widget.select({
                    selector: "select[name=GroupArea]",
                    url: "wh.api/combo/GroupAreas",
                }, function () {
                    $('#GroupArea').select2('val', r.data.GroupArea);
                    if (r.data.RoleID != 'DEV-ADMIN' && r.data.RoleID != 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                        $('#GroupArea').prop('disabled', 'disabled');
                    }
                });
                widget.select({
                    selector: "select[name=CompanyCode]",
                    url: "wh.api/combo/DealerList",
                }, function () {
                    $('#CompanyCode').select2('val', r.data.CompanyCode);
                    if (r.data.RoleID != 'DEV-ADMIN' && r.data.RoleID != 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                        $('#CompanyCode').prop('disabled', 'disabled');
                    }
                    widget.select({
                        selector: "select[name=Outlet]",
                        url: "wh.api/combo/Branches",
                        params: { id: r.data.CompanyCode }
                    }, function () {
                        if (r.data.Outlet) {
                            $('#Outlet').select2('val', r.data.Outlet);
                            if (r.data.RoleID != 'DEV-ADMIN' && r.data.RoleID != 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                                $('#Outlet').prop('disabled', 'disabled');
                            }
                        }
                    });
                });
            } else {
                //if (r.data.RoleID == 'DEV-ADMIN' || r.data.RoleID == 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                    //$('#btnExportXls').show();
                    widget.select({
                        selector: "select[name=GroupArea]",
                        url: "wh.api/combo/GroupAreas",
                    })
                //} else {
                //    $('#btnRefresh, #btnExportXls').prop('disabled', 'disabled');
                //    $('#Outlet, #CompanyCode, #GroupArea, #TypeReport, #DateFrom, #DateTo').prop('disabled', 'disabled');
                //    sdms.info({ type: "error", text: "User bukan Admin/ Dealer user belum di set! " });
                //}
            }
        });
        widget.populate(filter);
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $("#btnExportDtlXls").on("click", exportDtlXls);
    });

    function refreshGrid() {
        console.log('refreshGrid');
        console.log($('[name="DateFrom"]').val());
        widget.kgrid({
            url: "wh.api/inquiry/inqIndent",
            name: "InqPers",
            params: $("#pnlFilter").serializeObject(),
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "GroupNo", width: 1, title: "" },
                { field: "CompanyCode", width: 1, title: "" },
                { field: "Area", width: 120, title: "Area" },
                { field: "DealerName", width: 100, title: "Dealer" },
                //{ field: "OutletAbbreviation", width: 100, title: "Outlet" },
                { field: "PeriodYear", width: 70, title: "Year" },
                { field: "PeriodMonth", width: 50, title: "Month" },
                { field: "TipeKendaraan", width: 200, title: "Tipe Kendaraan" },
                { field: "Variant", width: 100, title: "Variant" },
                { field: "Transmisi", width: 50, title: "TM" },
                { field: "ColourCode", width: 200, title: "Color" },
                { field: "QuotaQty", width: 50, title: "Quota" },
                { field: "IndentQty", width: 50, title: "Indent" },
                //{ field: "IndentDate", width: 160, title: "Indent Date", template: "#= ((IndentDate === undefined) ? \"\" : moment(IndentDate).format('DD MMM YYYY')) #" },
                //{ field: "EmployeeName", width: 250, title: "Wiraniaga"}, //filterable: { extra: false, operators: { string: { contains: "Contains", startswith: "Starts with", } } } },
            ],
            detailInit: detailInit
        });
    }

    function detailInit(e) {
        console.log($('[name="GroupArea"]').select2('val'));
        //widget.post("wh.api/inquiry/inqIndentSub", { IndentNumber: e.data.IndentNumber }, function (data) {
        var Area = $('[name="GroupArea"]').select2('val');
        var Company = $('[name="CompanyCode"]').select2('val');
        var params = {
            GroupArea: e.data.GroupNo
            , CompanyCode: e.data.CompanyCode
            , PeriodYear: e.data.PeriodYear
            , PeriodMonth: e.data.PeriodMonth
            , TipeKendaraan: e.data.TipeKendaraan
            , Variant: e.data.Variant
            , ColourCode: e.data.ColourCode
        };
        widget.post("wh.api/inquiry/inqIndentSub", params, function (data) {
            if (data.length > 0) {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: data, pageSize: 10 },
                    pageable: true,
                    columns: [
                        { field: "OutletAbbreviation", width: 120, title: "Dealer" },
                        { field: "IndentNumber", width: 120, title: "Indent Number" },
                        { field: "IndentDate", width: 160, title: "Indent Date", template: "#= ((IndentDate === undefined) ? \"\" : moment(IndentDate).format('DD MMM YYYY')) #" },
                        { field: "EmployeeName", width: 250, title: "Wiraniaga" },
                        { field: "QuantityInquiry", width: 100, title: "Qty Inquiry" },
                        { field: "TipeKendaraan", title: "Tipe Kendaraan" },
                        { field: "LastProgress", title: "Last Progress" },
                        { field: "InquiryNumber", width: 120, title: "Inquiry Number" },
                        { field: "NamaProspek", width: 150, title: "Nama Prospek" },
                        { field: "AlamatProspek", width: 250, title: "Alamat Prospek" },
                        { field: "Handphone", width: 150, title: "Handphone" },
                        { field: "Variant", width: 150, title: "Varian" },
                        { field: "Transmisi", width: 150, title: "Transmisi" },
                        { field: "ColourCode", width: 180, title: "Warna" },
                        { field: "CaraPembayaran", width: 125, title: "Cara Pembayaran" },
                        { field: "StatusProspek", width: 125, title: "Status Prospek" },
                    ]
                });
            }
            else {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: [{ Info: "Data ini tidak memiliki detail" }] },
                    columns: [{ field: "Info", title: "Info" }]
                });
            }
        })
    }

    function exportDtlXls() {
        var data = $("#pnlFilter").serializeObject();
        console.log(data);
        console.log(data.GroupArea);
        console.log(data.CompanyCode);
        console.log(data.PeriodYear);
        console.log(data.PeriodMonth);

        var url = "wh.api/inquiryprod/inqIndentXls?";
        url += "&GroupArea=" + data.GroupArea;
        url += "&CompanyCode=" + data.CompanyCode;
        url += "&PeriodYear=" + data.PeriodYear;
        url += "&PeriodMonth=" + data.PeriodMonth;
        //url += "&Outlet=" + data.Outlet;
        //url += "&DateFrom=" + data.DateFrom;
        //url += "&DateTo=" + data.DateTo;
        //url += "&DateFrName=" + $("[name=DateFrom]").val();
        //url += "&DateToName=" + $("[name=DateTo]").val();
        window.location = url;

        if (RoleID == 'DEV-ADMIN' || RoleID == 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
            var url = "wh.api/inquiryprod/inqQuotaXls?";
            url += "&GroupArea=" + data.GroupArea;
            url += "&CompanyCode=" + data.CompanyCode;
            url += "&PeriodYear=" + data.PeriodYear;
            url += "&PeriodMonth=" + data.PeriodMonth;
            setTimeout(function () {
                window.location = url;
            }, 10);
        }
    }

    function exportXls() {
        var data = $("#pnlFilter").serializeObject();

        var url = "wh.api/inquiryprod/inqQuotaXls?";
        url += "&GroupArea=" + data.GroupArea;
        url += "&CompanyCode=" + data.CompanyCode;
        url += "&PeriodYear=" + data.PeriodYear;
        url += "&PeriodMonth=" + data.PeriodMonth;
        window.location = url;
    }
});
/*
 o.OutletAbbreviation
		, IndentNumber
		, IndentDate
		, n.EmployeeName
		, QuantityInquiry
		, TipeKendaraan 
		, LastProgress 
		, InquiryNumber
		, NamaProspek
		, AlamatProspek
		, Handphone
		, Variant
		, Transmisi
		, ColourCode
		, CaraPembayaran
		, StatusProspek
*/