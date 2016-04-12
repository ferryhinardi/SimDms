//global variable 
var variables = {};

$(document).ready(function () {
    var options = {
        title: "Inquiry - Customer with status",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                   {
                       text: "Company",
                       type: "controls", items: [
                           { name: "CompanyCode", text: "CompanyCode", cls: "span6", type: "select" },
                       ]
                   },
                   {
                       text: "Branch",
                       type: "controls", items: [
                           { name: "BranchCode", text: "BranchCode", cls: "span6", type: "select", opt_text: "-- SELECT ALL --", opt_val: "%" },
                       ]
                   },
                   {
                       text: "Inquiry Type",
                       type: "controls",
                       items: [{
                           name: "InqType", cls: "span4 fullfill", placeHolder: "Inqury Type", readonly: true, type: "select", required: true, items: [
                                 { value: "A", text: "Customer by Dealer" },
                                 { value: "B", text: "Customer by Transaction" },
                                 { value: "C", text: "Customer Suzuki" },
                                 { value: "D", text: "Customer Suzuki with last 3 years transaction" },
                                 { value: "E", text: "Customer Suzuki Detail with last transaction" },
                           ]
                       }]
                   },
                    {
                        text: "Period",
                        type: "controls",
                        cls: "PeriodMonthly",
                        items: [
                            { name: "isPeriod", text: "", cls: "span1", type: "switch" },
                            { name: "Month", text: "Month", cls: "span2", type: "select", required: true },
                            { name: "Year", text: "Year", cls: "span2", type: "select", required: true },
                        ]
                    },
                    {
                        text: "Period",
                        type: "controls",
                        cls: "PeriodDaily",
                        items: [
                            { name: "isPeriodDate", text: "", cls: "span1", type: "switch" },
                            { name: "DateFrom", text: "DateFrom", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "DateTo", cls: "span2", type: "datepicker" },
                        ]
                    },
                    {
                        text: "Status",
                        type: "controls",
                        cls: "DetLast",
                        items: [{
                            name: "CustType", cls: "span4 fullfill", placeHolder: "Inqury Type", readonly: true, type: "select", opt_text: "-- SELECT ALL --", items: [
                                  { value: "UNITSERVICE", text: "Buy Unit & Service" },
                                  { value: "UNIT", text: "Buy Unit Only" },
                                  { value: "SERVICE", text: "Service Only" },
                                  { value: "D", text: "Not Buy Unit & Service" },
                            ]
                        }]
                    },
                ],
            },
            {
                title: "Customer Status",
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
        widget.select({ selector: "#Month", url: "wh.api/Combo/ListOfMonth" });
        widget.select({ selector: "#Year", url: "wh.api/Combo/ListOfYear" });
        widget.select({ selector: "#CompanyCode", url: "wh.api/combo/DealerList?LinkedModule=", optionalValue: "%", optionalText: "-- SELECT ALL --" });
        //set initial date
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        $('input[name="DateFrom"]').attr("disabled", "disabled").val("");
        $('input[name="DateTo"]').attr("disabled", "disabled").val("");
        //$('input[name="isPeriod"]').val("true");
        //$('input[name="isPeriodY"]').val("true");
        widget.populate({ isPeriod: false });
        $('#Month,#Year').attr('disabled', 'disabled');
        $('input[name=isPeriodDate]').on('change', function () {
            if ($('input[name=DateFrom]').attr('disabled') === 'disabled') {
                widget.populate({ DateFrom: date2, DateTo: date1 });
                $('input[name=DateFrom]').removeAttr('disabled');
                $('input[name=DateTo]').removeAttr('disabled');
            } else {
                console.log($(this).val());
                $('input[name=DateFrom]').attr('disabled', 'disabled').val('');
                $('input[name=DateTo]').attr('disabled', 'disabled').val('');
            }
        });
        $('input[name=isPeriod]').on('change', function () {
            if ($('#Month, #Year').attr('disabled') === 'disabled') {
                $('#Month, #Year').removeAttr('disabled');
                $('label[for=Year],label[for=Month]').show();
            } else {
                $('#Month, #Year').attr('disabled', 'disabled');
                $('label[for=Year],label[for=Month]').hide();
                $('label[for=Year],label[for=Month]').val('');
            }
        });
        $('#BranchCode option').each(function () {
            if ($(this).text() == '-- SELECT ALL --') {
                $(this).val('%');
                $(this).text('-- SELECT ALL --');
                console.log('SET SELECT ALL');
            }
        });
        $('#CompanyCode option').each(function () {
            if ($(this).text() == '-- SELECT ONE --') {
                $(this).val('%');
                $(this).text('-- SELECT ALL --');
                console.log('SET SELECT ALL CompanyCode');
            }
        });

        //set value default
        $("#CustType option").each(function () {
            if ($(this).text() == "-- SELECT ALL --") {
                $(this).val("%");
            }
        });
        $('.DetLast, .PeriodMonthly, .PeriodDaily').hide();
    });

    $('[name="CompanyCode"]').on('change', function () {
        widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" }, function () {
            $("#BranchCode option").each(function () {
                if ($(this).text() == "-- SELECT ONE --" || $(this).text() == "-- SELECT ALL --") {
                    $(this).val("%");
                    $(this).text("-- SELECT ALL --");
                    console.log("SET SELECT ALL");
                }
            });
        });
    });

    $("#btnRefresh").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            refreshGrid();
        }
    });

    $("#btnExportXls").on("click", exportXls);

    $("#InqType").change(function () {
        var inqType = $('#InqType').val();
        //if (inqType === "D") {
        //    $('.PeriodDaily').slideDown();
        //    $('.DetLast, .PeriodMonthly').slideUp();

        //    $('#btnRefresh').slideDown();
        //    var valid = $(".main form").valid();
        //    if (valid) {
        //        refreshGrid();
        //    }
        //    $('#pnlKGrid').slideDown();
        //} else
        if (inqType === "E") {
            $('.DetLast, .PeriodDaily').slideDown();
            $('.PeriodMonthly').slideUp();
            $('#btnRefresh').slideUp();
            $('#pnlKGrid').slideUp();
        } else {
            $('.PeriodDaily,.DetLast').slideUp();
            $('.PeriodMonthly').slideDown();

            $('#btnRefresh').slideDown();
            var valid = $(".main form").valid();
            if (valid) {
                refreshGrid();
            }
            $('#pnlKGrid').slideDown();
        }

        if (inqType == "A") {
            $('#BranchCode').val("%").attr("selected", "selected");
            $('#BranchCode').attr("disabled", "disabled");
        }
        else {
            $('#BranchCode').removeAttr("disabled");
        }
        //console.log($("#InqType option:selected").text());
        $('.subtitle').text($("#InqType option:selected").text());
    });

    $('#pnlFilter select').on('change', function () {
        setTimeout(refreshGrid, 800);
    });

    function refreshGrid() {

        var date = new Date();
        var params;
        if ($('#InqType').val() === "D") {
            var Month = ($('#Month').attr('disabled') == undefined) ? $('#Month').val() : moment().format('MM');
            var Year = ($('#Month').attr('disabled') == undefined) ? $('#Year').val() : moment().format('YYYY');

            params = {
                CompanyCode: $('#CompanyCode').val(),
                BranchCode: $("#BranchCode").val(),
                Year: Year,
                Month: Month,
                InqType: 'D',
            }
        } else {
            var monthTo = "";
            var yearTo = "";
            var yearFrom = "";

            if ($('#Month, #Year').attr("disabled") === undefined) {
                monthTo = $('#Month').val();
                yearTo = 0;
                yearFrom = $('#Year').val();
            } else {
                monthTo = date.getMonth() + 1;
                yearTo = 0;
                yearFrom = date.getFullYear();
            }
            params = {
                CompanyCode: $("#CompanyCode").val(),
                BranchCode: $("#BranchCode").val(),
                InqType: $('#InqType').val(),
                Month: monthTo,
                YearTo: yearTo,
                YearFrom: yearFrom
            }
        }
        var url = "";

        if (params.InqType === "A") {
            widget.kgrid({
                url: "wh.api/inquiry/CustomerStatus",
                name: "pnlKGrid",
                params: params,
                columns: [
                    { field: "CompanyName", title: "Dealer Name", width: 300 },
                    { field: "NoOfUnitService", title: "Total Unit Service", width: 160, type: 'number' },
                    { field: "NoOfUnit", title: "Total Unit", width: 160, type: 'number' },
                    { field: "NoOfService", title: "Total Service", width: 160, type: 'number' },
                    { field: "NoOfSparePart", title: "Total Sparepart", width: 160, type: 'number' },
                ],
            });
        } else if (params.InqType === "E") {
            //refreshGridDetails();
        } else if (params.InqType === "D") {
            widget.kgrid({
                url: "wh.api/inquiry/CustomerStatus",
                name: "pnlKGrid",
                params: params,
                columns: [
                    { field: "BranchName", title: "Branch", width: 480 },
                    { field: "NoOfUnitService", title: "Total Unit Service", width: 160, type: 'number' },
                    { field: "NoOfUnit", title: "Total Unit", width: 160, type: 'number' },
                    { field: "NoOfService", title: "Total Service", width: 160, type: 'number' },
                ],
            });
        } else {
            widget.kgrid({
                url: "wh.api/inquiry/CustomerStatus",
                name: "pnlKGrid",
                params: params,
                columns: [
                    { field: "BranchName", title: "Branch", width: 480 },
                    { field: "NoOfUnitService", title: "Total Unit Service", width: 160, type: 'number' },
                    { field: "NoOfUnit", title: "Total Unit", width: 160, type: 'number' },
                    { field: "NoOfService", title: "Total Service", width: 160, type: 'number' },
                    { field: "NoOfSparePart", title: "Total Sparepart", width: 160, type: 'number' },
                ],
            });
        }
    }

    function refreshGridDetails() {
        var parameters = {
            CompanyCode: $("#CompanyCode").val(),
            BranchCode: $("#BranchCode").val(),
            CustType: $('#CustType').val(),
            TransDateStart: $('input[name="DateFrom"]').val(),
            TransDateEnd: $('input[name="DateTo"]').val()
        }
        //console.log(parameters);
        widget.kgrid({
            url: "wh.api/inquiry/CustomerStatusDetail",
            name: "pnlKGrid",
            params: parameters,
            columns: [
                { field: "BranchName", title: "Branch", width: 380 },
                { field: "CustomerCode", title: "Customer Code", width: 130 },
                { field: "CustomerName", title: "Customer Name", width: 180 },
                { field: "CustomerGovName", title: "Customer Gov Name", width: 180 },
                { field: "CustomerStatus", title: "Customer Status", width: 150 },
                { field: "Address", title: "Address", width: 350 },
                { field: "ProvinceName", title: "Province", width: 200 },
                { field: "CityName", title: "City", width: 200 },
                { field: "ZipNo", title: "Zip No", width: 100 },
                { field: "KelurahanDesa", title: "Kelurahan Desa", width: 140 },
                { field: "KecamatanDistrik", title: "Kecamatan", width: 120 },
                { field: "KotaKabupaten", title: "Kota/Kabupaten", width: 140 },
                { field: "IbuKota", title: "Ibu Kota", width: 120 },
                { field: "PhoneNo", title: "Phone No", width: 120 },
                { field: "HpNo", title: "Hp No", width: 120 },
                { field: "FaxNo", title: "Fax No", width: 120 },
                { field: "OfficePhoneNo", title: "Office Phone", width: 120 },
                { field: "Email", title: "Email", width: 200 },
                { field: "BirthDate", title: "Birthdate", width: 120, template: "#= (BirthDate == undefined) ? '' : moment(BirthDate).format('DD MMM YYYY') #" },
                { field: "IsPkp", title: "PKP", width: 120 },
                { field: "NPWPNo", title: "NPWP No", width: 200 },
                { field: "NPWPDate", title: "NPWP Date", width: 120, template: "#= (NPWPDate == undefined) ? '' : moment(NPWPDate).format('DD MMM YYYY') #" },
                { field: "SKPNo", title: "SKP No", width: 170 },
                { field: "SKPDate", title: "SKP Date", width: 160, template: "#= (SKPDate == undefined) ? '' : moment(SKPDate).format('DD MMM YYYY') #" },
                { field: "CustomerType", title: "Customer Type", width: 200 },
                { field: "Gender", title: "Gender", width: 100 },
                { field: "KTP", title: "KTP", width: 200 },
                { field: "LastTransactionDate", title: "Transaction Date", width: 140, template: "#= (LastTransactionDate == undefined) ? '' : moment(LastTransactionDate).format('DD MMM YYYY') #" }
            ],
        });
    }

    function exportXls() {
        var inqType = $('#InqType').val();
        if (inqType === "A" || inqType === "B" || inqType === "C" || inqType === "D") {
            sdms.report({
                url: 'wh.api/inquiry/CustomerSuzukiXls',
                type: 'xlsx',
                params: {
                    CompanyCode: $('[name=CompanyCode]').val(),
                    BranchCode: $('[name=BranchCode]').val(),
                    InqType: inqType,
                    Year: (($('#Month').attr('disabled') == undefined) ? $('#Year').val() : moment().format('YYYY')),
                    Month: (($('#Month').attr('disabled') == undefined) ? $('#Month').val() : moment().format('MM'))
                }
            });
        }
        else if (inqType === "E") {
            sdms.report({
                url: 'wh.api/inquiry/CustomerDealer',
                type: 'xlsx',
                params: {
                    CompanyCode: $('[name=CompanyCode]').val(),
                    BranchCode: $('[name=BranchCode]').val(),
                    CustType: $('[name=CustType]').val(),
                    DateFrom: ($('[name=DateFrom]').attr('disabled') == undefined) ? (moment($('[name=DateFrom]').val(), 'DD-MMM-YYYY').format('YYYY-MM-DD')) : '1900-01-01',
                    DateTo: ($('[name=DateTo]').attr('disabled') == undefined) ? (moment($('[name=DateTo]').val(), 'DD-MMM-YYYY').format('YYYY-MM-DD')) : '2100-01-01'
                }
            });
        }
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
