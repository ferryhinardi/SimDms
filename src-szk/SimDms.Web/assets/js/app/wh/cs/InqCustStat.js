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
                           name: "InqType", cls: "span4 fullfill", placeHolder: "Inqury Type", readonly: true, type: "select", required:true, items: [
                                 { value: "A", text: "Customer Data" },
                                 { value: "B", text: "Customer Data with transaction" },
                                 { value: "C", text: "Customer Suzuki" },
                                 //{ value: "D", text: "Customer Suzuki with last transaction" },
                                 { value: "E", text: "Customer Suzuki Detail" },
                           ]
                       }]
                   },
                    {
                        text: "Period",
                        type: "controls",
                        cls: "PeriodMonthly",
                        items: [
                            { name: "Month", text: "Month", cls: "span2", type: "select", required:true },
                            { name: "Year", text: "Year", cls: "span2", type: "select", required: true },
                        ]
                    },
                    {
                        text: "Period",
                        type: "controls",
                        cls: "PeriodDaily",
                        items: [
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
                                  { value: "A", text: "Buy Unit & Service" },
                                  { value: "B", text: "Buy Unit Only" },
                                  { value: "C", text: "Service Only" },
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
        LoadDefault();
        widget.select({ selector: "#Month", url: "cs.api/Combo/ListOfMonth" });
        widget.select({ selector: "#Year", url: "cs.api/Combo/ListOfYear" });
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
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });


        //set value default
        $("#CustType option").each(function () {
            if ($(this).text() == "-- SELECT ALL --") {
                $(this).val("%");
            }
        });
        $('.DetLast, .PeriodMonthly, .PeriodDaily').hide();
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
        } else if (inqType === "E") {
            $('.DetLast, .PeriodDaily').slideDown();
            $('.PeriodMonthly').slideUp();
        } else {
            $('.PeriodDaily,.DetLast').slideUp();
            $('.PeriodMonthly').slideDown();
        }

        if (inqType == "A") {
            $('#BranchCode').val("%").attr("selected", "selected");
            $('#BranchCode').attr("disabled", "disabled");
        }
        else {
            $('#BranchCode').removeAttr("disabled");
        }
    });

    function refreshGrid() {

        var params;
        if ($('#InqType').val() === "D") {
            params = {
                BranchCode: $("#BranchCode").val(),
                InqType: $('#InqType').val(),
                DateFrom: $('input[name="DateFrom"]').val(),
                DateTo: $('input[name="DateTo"]').val()
            }
        } else {
            params = {
                BranchCode: $("#BranchCode").val(),
                InqType: $('#InqType').val(),
                Month: $('#Month').val(),
                Year: $('#Year').val()
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
        } else if(params.InqType === "D"){
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
        }else {
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
        var fileName = "Customer_with_status";
        var InqType = $('#InqType').val();
        if (InqType === "A") {
            widget.exportXls({
                name: "pnlKGrid",
                type: "kgrid",
                fileName: fileName,
                items: [
                        { field: "CompanyName", title: "Dealer Name", width: 200 },
                        { field: "NoOfUnitService", title: "Total Unit Service", width: 200 },
                        { field: "NoOfUnit", title: "Total Unit", width: 200 },
                        { field: "NoOfService", title: "Total Service", width: 200 },
                        { field: "NoOfSparePart", title: "Total Sparepart", width: 200 }, ]
            });
        }
        else if (InqType === "D") {
            widget.exportXls({
                name: "pnlKGrid",
                type: "kgrid",
                fileName: fileName,
                items: [
                    { field: "BranchName", title: "Branch", width: 280 },
                    { field: "NoOfUnitService", title: "Total Unit Service", width: 200 },
                    { field: "NoOfUnit", title: "Total Unit", width: 200 },
                    { field: "NoOfService", title: "Total Service", width: 200 }, ]
            });
        }
        else if (InqType === "E") {
            widget.exportXls({
                name: "pnlKGrid",
                type: "kgrid",
                fileName: fileName,
                items: [
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
                { field: "BirthDate", title: "Birthdate", width: 120, type:"date"},
                { field: "IsPkp", title: "PKP", width: 120 },
                { field: "NPWPNo", title: "NPWP No", width: 200 },
                { field: "NPWPDate", title: "NPWP Date", width: 120, type:"date"},
                { field: "SKPNo", title: "SKP No", width: 170 },
                { field: "SKPDate", title: "SKP Date", width: 160, type:"date"},
                { field: "CustomerType", title: "Customer Type", width: 200 },
                { field: "Gender", title: "Gender", width: 100 },
                { field: "KTP", title: "KTP", width: 200 },
                { field: "LastTransactionDate", title: "Transaction Date", width: 140, type: "date" } ]
            });
        }
        else {
            widget.exportXls({
                name: "pnlKGrid",
                type: "kgrid",
                fileName: fileName,
                items: [
                        { field: "BranchName", title: "Branch", width: 280 },
                        { field: "NoOfUnitService", title: "Total Unit Service", width: 200 },
                        { field: "NoOfUnit", title: "Total Unit", width: 200 },
                        { field: "NoOfService", title: "Total Service", width: 200 },
                        { field: "NoOfSparePart", title: "Total Sparepart", width: 200 }, ]
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
