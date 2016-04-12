var params = {};
$(document).ready(function () {
    var options = {
        title: "Live Stock Portal List",
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
                        text: "Outlet",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span6', opt_text: "-- SELECT ALL -- " }
                        ]
                    },
                    /*{
                        text: "Customer Code", type: "controls", cls: 'span8', items: [
                            //{ name: 'checkCustomer', text: '', type: 'check', cls: 'span1' },
                            { name: "CustomerCode", text: "Customer Code", type: "popup", cls: "span2", readonly: 'readonly' },
                            { name: "CustomerName", text: "Customer Name", cls: "span4", readonly: 'readonly' }
                        ]
                    },*/
                    {
                         text: "Period Indent From",
                         type: "controls",
                         cls: "span8",
                         items: [
                             { name: "DateFrom", cls: "span6", type: "datepicker" }
                         ]
                     },
                    {
                        text: "Period Indent To",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "DateTo", cls: "span6", type: "datepicker" }
                        ]
                    },
                     {
                         text: "Mode Report", type: "controls", cls: 'span8', items: [
                            { name: 'Mode', text: 'Mode Report', type: 'radio-switch', cls: 'span4', option: { Y: 'Inquiry Mode', N: 'Selling Mode' } },
                         ]
                     },
                    /*{
                        name: 'TypeReport', text: 'Format Tampilan', type: 'select', cls: 'span5 full',
                        items: [
                            { value: 'Txt', text: 'Text' },
                            { value: 'Xls', text: 'Excel' },

                        ]
                    },*/
                ],
            },
            {
                name: "InqPers",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Generate", icon: "fa fa-file-excel-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
   
    widget.render(function () {
        var filter = {
            DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
            DateTo: new Date(),
            //TypeReport: 'Txt',
        }
        widget.post('wh.api/indent/default', function (r) {
            params.GroupArea = r.data.GroupArea;
            params.CompanyCode = r.data.CompanyCode;
            params.OutletCode = r.data.Outlet;
            if (r.data.CompanyCode && r.data.CompanyCode != "") {
                widget.select({
                    selector: "select[name=GroupArea]",
                    url: "wh.api/combo/GroupAreas",
                    optionalText: "--SELECT ALL--"
                }, function (data) {
                    $('[name=GroupArea]').select2('val', r.data.GroupArea);
                    if (r.data.RoleID != 'DEV-ADMIN' && r.data.RoleID != 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                        $('#GroupArea').prop('disabled', 'disabled');
                    }
                    widget.select({
                        selector: "select[name=CompanyCode]",
                        url: "wh.api/combo/ComboDealerList",
                        params: { GroupArea: r.data.GroupArea },
                        optionalText: "--SELECT ALL--"
                    }, function (data) {
                        var search = ""
                        if (data != null) {
                            if (data.length > 0) {
                                search = $.grep(data || [], function (el, idx) {
                                    var elCompanyCode = el.value.substring((el.value.indexOf("|") + 1), el.value.length);
                                    return elCompanyCode == r.data.CompanyCode;
                                });
                            }
                        }
                        $('#CompanyCode').select2('val', (search.length > 0) ? search[0].value : search);
                        if (r.data.RoleID != 'DEV-ADMIN' && r.data.RoleID != 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                            $('#CompanyCode').prop('disabled', 'disabled');
                        }
                        widget.select({
                            selector: "select[name=Outlet]",
                            url: "wh.api/combo/ComboOutletList",
                            params: { companyCode: $("[name=CompanyCode]").val() },
                            optionalText: "--SELECT ALL--"
                        }, function (data) {
                            if (r.data.Outlet) {
                                $('#Outlet').select2('val', r.data.Outlet);
                                if (r.data.RoleID != 'DEV-ADMIN' && r.data.RoleID != 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                                    $('#Outlet').prop('disabled', 'disabled');
                                }
                            }
                            disabledCombo($("[name=GroupArea]"), $("[name=CompanyCode]"));
                            disabledCombo($("[name=CompanyCode]"), $("[name=Outlet]"));
                        });
                    });
                });
            } else {
                if (r.data.RoleID == 'DEV-ADMIN' || r.data.RoleID == 'B13ADCEA-E538-49FD-B9E0-34901CE26E2E') {
                    widget.select({
                        selector: "select[name=GroupArea]",
                        url: "wh.api/combo/GroupAreas",
                        optionalText: "--SELECT ALL--"
                    })
                } else {
                    $('#btnCustomerCode,#btnRefresh, #btnExportXls').prop('disabled', 'disabled');
                    $('#Outlet, #CompanyCode, #GroupArea, #TypeReport, #Mode, #DateFrom, #DateTo').prop('disabled', 'disabled');
                    sdms.info({ type: "error", text: "User bukan Admin/ Dealer user belum di set! "});
                }
            }
        });
        widget.populate(filter);
        newCombo();
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
    });
    
    function refreshGrid() {
        var data = $("#pnlFilter").serializeObject();
        data.CompanyCode = data.CompanyCode.substring(data.CompanyCode.indexOf("|") + 1, data.CompanyCode.length)
        data.DateFrom = moment(data.DateFrom).format('YYYYMMDD');
        data.DateTo = moment(data.DateTo).format('YYYYMMDD');
        widget.kgrid({
            url: "wh.api/inquiry/SpLogReport",
            name: "InqPers",
            params: data,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "date_access", width: 130, title: "date_access"}, // template: "#= ((date_access === undefined) ? \"\" : moment(date_access).format('DD MMM YYYY')) #" },
                { field: "part_no", width: 200, title: "part_no" },
                { field: "part_name", width: 200, title: "part_name" },
                { field: "model_name", width: 250, title: "model_name" },
                { field: "search_by_dealer", width: 120, title: "search_by_dealer" },
                { field: "area", width: 150, title: "area" },
            ],
            //detailInit: detailInit
        });
    }
    /*
    function detailInit(e) {
        console.log('ini indentnumber : '+e.data.IndentNumber);
        widget.post("wh.api/inquiry/inqIndentSub", { IndentNumber: e.data.IndentNumber }, function (data) {
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
    */
    function exportXls() {
        var data = $("#pnlFilter").serializeObject();
        var data1 = widget.serializeObject();
        var mode = (data.Mode == "false") ? 'SLS' : 'INQ';
        var GroupArea = "", GroupAreaMapping = "", CompanyCode = "", OutletCode = "";
        var position = data.CompanyCode.indexOf("|");
        if (position > 0) {
            GroupAreaMapping = data.CompanyCode.substring(0, position);
            CompanyCode = data.CompanyCode.substring(position+1, data.CompanyCode.length);
        }
        if (!$.isEmptyObject(params)) {
            GroupArea = params.GroupArea;
            CompanyCode = params.CompanyCode;
            OutletCode = params.OutletCode;
        }
        else {
            GroupArea = data.GroupArea;
            OutletCode = data.Outlet;
        }
        var url = "wh.api/inquiryprod/SpLogReportXls?";
        url += "&GroupArea=" + GroupArea;
        url += "&CompanyCode=" + CompanyCode;
        url += "&Outlet=" + OutletCode;
        url += "&DateFrom=" + moment(data.DateFrom).format('YYYYMMDD');
        url += "&DateTo=" + moment(data.DateTo).format('YYYYMMDD');
        url += "&Mode=" + mode;
        url += "&DateFrName=" + $("[name=DateFrom]").val();
        url += "&DateToName=" + $("[name=DateTo]").val();
        window.location = url;
    }

    function newCombo() {
        widget.select({ selector: "[name=GroupArea]", url: "wh.api/Combo/GroupAreas", optionalText: "--SELECT ALL--" });
        
        $("[name=GroupArea]").change(function (e) {
            var id = $(this).val();
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: id }, optionalText: "--SELECT ALL--" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
            disabledCombo($("[name=GroupArea]"), $("[name=CompanyCode]"));
        });

        $("[name=CompanyCode]").change(function (e) {
            var id = $(this).val();
            widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/ComboOutletList", params: { companyCode: id }, optionalText: "--SELECT ALL--" });
            $("[name=Outlet]").prop("selectedIndex", 0);
            $("[name=Outlet]").change();
            disabledCombo($("[name=CompanyCode]"), $("[name=Outlet]"));
        });

        $("[name=Outlet]").change(function (e) {
            params = {};
        });
        disabledCombo($("[name=GroupArea]"), $("[name=CompanyCode]"));
        disabledCombo($("[name=CompanyCode]"), $("[name=Outlet]"));
    }

    function disabledCombo(el1, el2) {
        if (el1.val() == "")
            el2.prop("disabled", true);
        else
            el2.prop("disabled", false);
    }
});