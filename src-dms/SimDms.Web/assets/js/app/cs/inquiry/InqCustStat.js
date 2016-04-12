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
                           { name: "CompanyCode", text: "CompanyCode", cls: "span2", type: "text", readonly: true },
                           { name: "CompanyName", text: "CompanyName", cls: "span4", type: "text", readonly: true },
                       ]
                   },
                   {
                       text: "Branch",
                       type: "controls", items: [
                           { name: "BranchCode", text: "BranchCode", cls: "span6", type: "select" },
                       ]
                   },
                   {
                       text: "Inquiry Type",
                       type: "controls",
                       items: [{
                           name: "InqType", cls: "span4 fullfill", placeHolder: "Inqury Type", type: "select", required:true, items: [
                                 { value: "A", text: "Customer Data" },
                                 { value: "B", text: "Customer Data with transaction" },
                                 { value: "C", text: "Customer Suzuki" },
                                 { value: "D", text: "Customer Suzuki with last transaction" },
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
                            name: "CustType", cls: "span4 fullfill", placeHolder: "Inqury Type", type: "select", opt_text: "-- SELECT ALL --", items: [
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
                name: "pnlKGrid",
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
        LoadDefault();
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ isPeriod: false });
        widget.select({ selector: "#Month", url: "cs.api/Combo/ListOfMonth" });
        widget.select({ selector: "#Year", url: "cs.api/Combo/ListOfYear" });
        $('input[name="DateFrom"]').attr("disabled", "disabled").val("");
        $('input[name="DateTo"]').attr("disabled", "disabled").val("");
        $('#Month,#Year').attr('disabled', 'disabled');
        widget.select({ selector: "#BranchCode", url: "cs.api/Combo/ListBranchCode" }, function () {
            $("#BranchCode option").each(function () {
                if ($(this).text() == "-- SELECT ONE --") {
                    $(this).val("%");
                    $(this).text("-- SELECT ALL --");
                    console.log("SET SELECT ALL");
                }
            });
        });
        //set initial date
        


        //set value default
        $("#CustType option").each(function () {
            if ($(this).text() == "-- SELECT ALL --") {
                $(this).val("%");
            }
        });
        $('.DetLast, .PeriodMonthly, .PeriodDaily').hide();
    });

    $('input[name="isPeriodDate"]').on("change", function () {
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        if ($('input[name="DateFrom"]').attr("disabled") === "disabled") {
            widget.populate({ DateFrom: date2, DateTo: date1 });
            $('input[name="DateFrom"]').removeAttr("disabled");
            $('input[name="DateTo"]').removeAttr("disabled");
        } else {
            console.log($(this).val());
            $('input[name="DateFrom"]').attr("disabled", "disabled").val("");
            $('input[name="DateTo"]').attr("disabled", "disabled").val("");
        }
    });

    $('input[name="isPeriod"]').on("change", function () {
        if ($('#Month, #Year').attr("disabled") === "disabled") {
            $('#Month, #Year').removeAttr("disabled");
            $('label[for="Year"],label[for="Month"]').show();
        } else {
            $('#Month, #Year').attr("disabled", "disabled");
            $('label[for="Year"],label[for="Month"]').hide();
            $('label[for="Year"],label[for="Month"]').val("");
        }
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
        if (inqType === "D") {
            $('.PeriodDaily').slideDown();
            $('.DetLast, .PeriodMonthly').slideUp();

            $('#btnRefresh').slideDown();
            var valid = $(".main form").valid();
            if (valid) {
                refreshGrid();
            }
            $('#pnlKGrid').slideDown();
        } else if (inqType === "E") {
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

    function refreshGrid() {

        var date = new Date();
        var params;
        if ($('#InqType').val() === "D") {
            var DateFrom;
            var DateTo;
            if ($('input[name="DateFrom"]').attr("disabled") === undefined) {
                DateFrom = $('input[name="DateFrom"]').val();
                DateTo = $('input[name="DateTo"]').val();
            } else {
                DateFrom = "01-01-1900";
                DateTo = "12-12-2100";
            }
            params = {
                CompanyCode: $('#CompanyCode').val(),
                BranchCode: $("#BranchCode").val(),
                InqType: $('#InqType').val(),
                DateFrom: DateFrom,
                DateTo: DateTo
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
                //yearFrom = "1900";
                //yearTo = "2100";
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
                url: "cs.api/inquiry/CustomerStatus",
                name: "pnlKGrid",
                params: params,
                columns: [
                    { field: "CompanyName", title: "Dealer Name", width: 200 },
                    { field: "NoOfUnitService", title: "Total Unit Service", width: 200 },
                    { field: "NoOfUnit", title: "Total Unit", width: 200 },
                    { field: "NoOfService", title: "Total Service", width: 200 },
                    { field: "NoOfSparePart", title: "Total Sparepart", width: 200 },
                ],
            });
            console.log('a');
        } else if (params.InqType === "E") {
            refreshGridDetails();
        } else if (params.InqType === "D") {
            widget.kgrid({
                url: "cs.api/inquiry/CustomerStatus",
                name: "pnlKGrid",
                params: params,
                columns: [
                    { field: "BranchName", title: "Branch", width: 280 },
                    { field: "NoOfUnitService", title: "Total Unit Service", width: 200 },
                    { field: "NoOfUnit", title: "Total Unit", width: 200 },
                    { field: "NoOfService", title: "Total Service", width: 200 },
                ],
            });
        } else {
            widget.kgrid({
                url: "cs.api/inquiry/CustomerStatus",
                name: "pnlKGrid",
                params: params,
                columns: [
                    { field: "BranchName", title: "Branch", width: 280 },
                    { field: "NoOfUnitService", title: "Total Unit Service", width: 200 },
                    { field: "NoOfUnit", title: "Total Unit", width: 200 },
                    { field: "NoOfService", title: "Total Service", width: 200 },
                    { field: "NoOfSparePart", title: "Total Sparepart", width: 200 },
                ],
            });
        }
    }

    function refreshGridDetails() {
        var parameters = {
            BranchCode: $("#BranchCode").val(),
            CustType: $('#CustType').val(),
            TransDateStart: $('input[name="DateFrom"]').val(),
            TransDateEnd: $('input[name="DateTo"]').val()
        }
        //console.log(parameters);
        widget.kgrid({
            url: "cs.api/inquiry/CustomerStatusDetail",
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
        var date = new Date();
        console.log(date.getFullYear());
        if (inqType === "A" || inqType === "B" || inqType === "C") {
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
            console.log(params);
            widget.showReport({
                id: "CsCustData",
                par: [$('[name="InqType"]').val(), monthTo, yearTo, yearFrom, $('[name="BranchCode"]').val()],
                panel: "pnlKGrid",
                type: "export",
                filename: "InquiryCustomer",
            });
        }
        else if (inqType === "D") {
            var DateFrom;
            var DateTo;
            if ($('input[name="DateFrom"]').attr("disabled") === undefined) {
                DateFrom = $('input[name="DateFrom"]').val();
                DateTo = $('input[name="DateTo"]').val();
            } else {
                DateFrom = "01-01-1900";
                DateTo = "12-12-2100";
            }

            widget.showReport({
                id: "CsCustDataTrans",
                par: [$('[name="CompanyCode"]').val(), $('[name="BranchCode"]').val(), DateFrom, DateTo],
                panel: "pnlKGrid",
                type: "export",
                filename: "InquiryCustomer",
            });
        }
        else if (inqType === "E") {
            var DateFrom;
            var DateTo;
            if ($('input[name="DateFrom"]').attr("disabled") === undefined) {
                DateFrom = $('input[name="DateFrom"]').val();
                DateTo = $('input[name="DateTo"]').val();
            } else {
                DateFrom = "01-01-1900";
                DateTo = "12-12-2100";
            }

            widget.showReport({
                id: "CsCustDataTransDtl",
                par: [$('[name="CompanyCode"]').val(), $('[name="BranchCode"]').val(), $('[name="CustType"]').val(), DateFrom, DateTo],
                panel: "pnlKGrid",
                type: "export",
                filename: "InqcuiryCustomer",
            });
        }
    }

    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }

    function LoadDefault() {
        var url = "cs.api/CustBirthday/Default";
        widget.post(url, function (result) {
            if (widget.isNullOrEmpty(result) == false) {
                variables.CompanyCode = result.CompanyCode;
                variables.CompanyName = result.CompanyName;
                widget.populate(result);
            }
        });
    }
});
