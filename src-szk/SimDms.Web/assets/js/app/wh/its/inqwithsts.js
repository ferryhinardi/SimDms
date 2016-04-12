$(document).ready(function () {
    var options = {
        title: "Inquiry with Status",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Generate Excel", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "PositionID", type: "text", cls: "hide" },
                    { name: "EmployeeID", type: "text", cls: "hide" },

                    { name: "DateFrom", text: "Periode From", cls: "span4", type: "datepicker" },
                    { name: "DateTo", text: "to", cls: "span4", type: "datepicker" },

                    { name: "Area", text: "Area", cls: "span4", type: "select" },
                    { name: "GroupModel", text: "Group Model", cls: "span4", type: "select" },

                    { name: "Dealer", text: "Dealer", cls: "span4", type: "select" },
                    { name: "ModelType", text: "Tipe Kendaraan", cls: "span4", type: "select" },

                    { name: "Outlet", text: "Outlet", cls: "span4", type: "select" },
                    { name: "Variant", text: "Variant", cls: "span4", type: "select" },

                    { name: "Report", text: "Report", cls: "span4", type: "select" },
                    { name: "ReportType", text: "Type", cls: "span4", type: "select" },
                ]          
            },
        ]
    }

    var widget = new SimDms.Widget(options);

    var paramsSelect = [
        {
            //name: "Area", url: "wh.api/inquiryprod/dealermappingareas",
            name: "Area",
            url: "wh.api/combo/GroupAreas",
            optionalText: "--SELECT ALL--"
        },
         {
             name: "Dealer",
             //url: "wh.api/inquiryprod/dealermappingdealers",
             url: "wh.api/combo/ComboDealerList",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Area",
                 additionalParams: [
                     { name: "groupArea", source: "Area", type: "select" }
                 ]
             }
         },
         {
             name: "Outlet",
             //url: "wh.api/inquiryprod/outlets",
             url: "wh.api/combo/ComboOutletList",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Dealer",
                 additionalParams: [
                     { name: "companyCode", source: "Dealer", type: "select" },
                     //{ name: "Dealer", source: "Dealer", type: "select" }
                 ]
             }
         },
         {
             name: "GroupModel", url: "wh.api/inquiryprod/groupmodels",
             optionalText: "--SELECT ALL--"            
         },
         {
             name: "ModelType",
             url: "wh.api/inquiryprod/modeltypes",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "GroupModel",
                 additionalParams: [
                     { name: "GroupModel", source: "GroupModel", type: "select" }
                 ]
             }
         },
         {
             name: "Variant", url: "wh.api/inquiryprod/carvariants",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "ModelType",
                 additionalParams: [
                     { name: "Dealer", source: "Dealer", type: "select" },
                     { name: "GroupModel", source: "GroupModel", type: "select" },
                     { name: "ModelType", source: "ModelType", type: "select" }
                 ]
             }
         },
    ];
    console.log(paramsSelect);
    widget.setSelect(paramsSelect);
    var nationalSLS = '';

    widget.render(function () {
        $('#Dealer, #Outlet, #ModelType, #Variant').attr('disabled', 'disabled');
        widget.post("wh.api/inquiryprod/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);

                widget.select({ selector: "select[name=Report]", url: "wh.api/inquiryprod/ReportInqWihtSts", selected: "0" });
                widget.select({ selector: "select[name=ReportType]", url: "wh.api/inquiryprod/ReportTypeInqWihtSts", selected: "0" });
            }           
        });
    });

    $('#Area').on('change', function (e) {
            if ($('#Area').val() == "") {
                $('#Dealer, #Outlet').attr('disabled', 'disabled');
            }
            else {
                $('#Dealer').removeAttr('disabled', 'disabled');
            }
    });

    $('#Dealer').on('change', function (e) {
            if ($('#Dealer').val() == "") {
                $('#Outlet').attr('disabled', 'disabled');
            }
            else {
                $('#Outlet').removeAttr('disabled', 'disabled');
            }
    });

    $('#GroupModel').on('change', function (e) {
        if ($('#GroupModel').val() == "") {
            $('#ModelType').attr('disabled', 'disabled');
        }
        else {
            $('#ModelType').removeAttr('disabled', 'disabled');
        }
    });

    $('#ModelType').on('change', function (e) {
        if ($('#ModelType').val() == "") {
            $('#Variant').attr('disabled', 'disabled');
        }
        else {
            $('#Variant').removeAttr('disabled', 'disabled');
        }
    });

    $('#btnProcess').on('click', function (e) {
        if ($("[name=ReportType]").val() == '') {
            alert("Tipe Report wajib dipilih");
            return;
        }

        if (moment($("[name=DateFrom]").val(), "DD-MMM-YYYY").format("YYYYMMDD") > moment($("[name=DateTo]").val(), "DD-MMM-YYYY").format("YYYYMMDD")) {
            alert("Tanggal akhir harus lebih besar dari tanggal awal periode");
            return;
        }

        var dtStart = moment($("[name=DateFrom]").val(), "DD-MMM-YYYY").format("YYYYMMDD");
        var yearStart = dtStart.toString().substring(0, 4);
        var monthStart = dtStart.toString().substring(6, 4);
        var dtStart = moment($("[name=DateTo]").val(), "DD-MMM-YYYY").format("YYYYMMDD");
        var yearEnd = dtStart.toString().substring(0, 4);
        var monthEnd = dtStart.toString().substring(6, 4);       
        if (yearStart != yearEnd || monthStart != monthEnd) {
            alert("Bulan Periode harus sama");
        }
        sdms.info("Please wait. The process might take more than 5 minutes.");
        window.location.href = "wh.api/inquiryprod/CreateExcelInqWithSts?DateFrom=" + $("[name=DateFrom]").val() + '&DateTo=' + $("[name=DateTo]").val() + '&Area=' + $("[name=Area]").val()
            + '&Dealer=' + $("[name=Dealer]").val() + '&Outlet=' + $("[name=Outlet]").val() + '&GroupModel=' + $("[name=GroupModel]").val()
            + '&ModelType=' + $("[name=ModelType]").val() + '&Variant=' + $("[name=Variant]").val() + '&Report=' + $("[name=Report]").val()
            + '&ReportType=' + $("[name=ReportType]").val();
    });
});