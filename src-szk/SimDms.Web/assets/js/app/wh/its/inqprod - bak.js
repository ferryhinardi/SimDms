$(document).ready(function () {
    var options = {
        title: "Inquiry By Productivity",
        //xtype: "report",
        toolbars: [            
            { name: "btnProcess", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        items: [
             { name: "PositionID", type: "text", cls: "hide" },
             { name: "EmployeeID", type: "text", cls: "hide" },

             { name: "DateFrom", text: "Periode From", cls: "span4", type: "datepicker" },
             { name: "DateTo", text: "to", cls: "span4", type: "datepicker" },

             { name: "Area", text: "Area", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
             { name: "BranchManager", text: "Branch Manager", cls: "span4", type: "select" },

             { name: "Dealer", text: "Dealer", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
             { name: "SalesHead", text: "Sales Head", cls: "span4", type: "select" },

             { name: "Outlet", text: "Outlet", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
             { name: "Salesman", text: "Salesman", cls: "span4", type: "select" },
             //{ name: "SalesCoord", text: "Sales Coord", cls: "span4", type: "select" },

             { name: "ReportType", text: "Report Type", cls: "span4", type: "select" },
             { name: "ProductType", text: "Productivity By", cls: "span4", type: "select" },            
            ]
    }

    var widget = new SimDms.Widget(options);
    /*
    var paramsSelect = [       
       {
           name: "Salesman", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=10",
            optionalText: "--SELECT ALL--",
            cascade: 
                {
                    name: "SalesHead",
                    additionalParams: [
                        { name: "EmployeeID", source: "SalesHead", type: "select" },
                        { name: "Dealer", source: "Dealer", type: "select" },
                        { name: "Outlet", source: "Outlet", type: "select" }
                    ]
                }            
        },
        //{
        //    name: "SalesCoord", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=20",
        //    optionalText: "--SELECT ALL--",
        //    cascade: {
        //        name: "SalesHead",
        //        additionalParams: [
        //            { name: "EmployeeID", source: "SalesHead", type: "select" },
        //            { name: "Dealer", source: "Dealer", type: "select" },
        //            { name: "Outlet", source: "Outlet", type: "select" }
        //        ]
        //    }
        //},
        {
            name: "SalesHead", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=30",
            optionalText: "--SELECT ALL--",
            cascade: {
                name: "BranchManager",
                additionalParams: [
                    { name: "EmployeeID", source: "BranchManager", type: "select" },
                    { name: "Dealer", source: "Dealer", type: "select" },
                    { name: "Outlet", source: "Outlet", type: "select" }
                ]
            }
        },       
         //{
         //    name: "Outlet", url: "wh.api/inquiryprod/outlets",
         //    optionalText: "--SELECT ALL--",
         //    cascade: {
         //        name: "Dealer",
         //        additionalParams: [
         //            { name: "Area", source: "Area", type: "select" },
         //            { name: "Dealer", source: "Dealer", type: "select" }
         //        ]
         //    }
         //},
         //{
         //    name: "Dealer", url: "wh.api/inquiryprod/DealerMappingDealers",
         //    optionalText: "--SELECT ALL--",
         //    cascade: {
         //        name: "Area",
         //        additionalParams: [
         //            { name: "Area", source: "Area", type: "select" }                    
         //        ]
         //    }
         //},
         {
             name: "Outlet", url: "wh.api/combo/ListBranchesNew",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Dealer",
                 additionalParams: [
                     { name: "Area", source: "Area", type: "select", as: "area" },
                     { name: "Dealer", source: "Dealer", type: "select", as: "comp" }
                 ]
             }
         },
         {
             name: "Dealer", url: "wh.api/combo/ListDealersNew",
             optionalText: "--SELECT ALL--",
             dataAttr: "DealerCode",
             cascade: {
                 name: "Area",
                 additionalParams: [
                     { name: "Area", source: "Area", type: "select", as: "area" }
                 ]
             }
         },
         {
             name: "BranchManager", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=40",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Outlet",
                 additionalParams: [                     
                     { name: "Dealer", source: "Dealer", type: "select" },
                     { name: "Outlet", source: "Outlet", type: "select" }
                 ]
             }
         },
    ];

    widget.setSelect(paramsSelect);
    */
    var nationalSLS = '';

    widget.render(function () {
        $('#Dealer, #Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
        $(".frame").css({ top: 240 });
        widget.post("wh.api/inquiryprod/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);

                widget.select({ selector: "select[name=ReportType]", url: "wh.api/inquiryprod/reporttypes", selected: "0" });
                widget.select({ selector: "select[name=ProductType]", url: "wh.api/inquiryprod/productivityby", selected: "0" });
                //widget.select({ selector: "select[name=Area]", url: "wh.api/inquiryprod/dealermappingareas", optionalText: "--SELECT ALL--" });
            }
        });

        newCombo();
    });
    
    $("#btnProcess").on("click", function () { showReport(); });

    $('#Area').on('change', function (e) {
            if ($('#Area').val() == "") {
                $('#Dealer, #Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
            }
            else {
                $('#Dealer').removeAttr('disabled', 'disabled');
            }
    });

    $('#Dealer').on('change', function (e) {
            if ($('#Dealer').val() == "") {
                $('#Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
            }
            else {
                $('#Outlet').removeAttr('disabled', 'disabled');
            }
    });

    $('#Outlet').on('change', function (e) {
        if ($('#Outlet').val() == "") {
            $('#BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
        }
        else {
            $('#BranchManager').removeAttr('disabled', 'disabled');
            var CompanyCode = $('[name=Dealer] option:selected').attr("data-attr");
            var BranchCode = $('#Outlet').val();
            widget.select({ selector: "[name=BranchManager]", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=40", params: { dealer: CompanyCode, outlet: BranchCode } });
        }
    });

    $('#BranchManager').on('change', function (e) {
        if ($('#BranchManager').val() == "") {
            $('#SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
        }
        else {
            $('#SalesHead').removeAttr('disabled', 'disabled');
            var CompanyCode = $('[name=Dealer] option:selected').attr("data-attr");
            var BranchCode = $('#Outlet').val();
            var EmployeeID = $(this).val();
            widget.select({ selector: "[name=SalesHead]", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=30", params: { dealer: CompanyCode, outlet: BranchCode, EmployeeID: EmployeeID } });
        }
    });
    /*
    $('#SalesCoord').on('change', function (e) {        
        if ($('#SalesCoord').val() == "") {
            $('#Salesman').attr('disabled', 'disabled');
        }
        else {
            $('#Salesman').removeAttr('disabled', 'disabled');
        }           
    });
    */
    $('#SalesHead').on('change', function (e) {
        if ($('#SalesHead').val() == "") {
            $('#Salesman').attr('disabled', 'disabled');
        }
        else {
            $('#Salesman').removeAttr('disabled', 'disabled');
            var CompanyCode = $('[name=Dealer] option:selected').attr("data-attr");
            var BranchCode = $('#Outlet').val();
            var EmployeeID = $(this).val();
            widget.select({ selector: "[name=Salesman]", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=10", params: { dealer: CompanyCode, outlet: BranchCode, EmployeeID: EmployeeID } });
        }
    });

    function showReport() {
        var idReport = "";
        if ($("#ProductType").val() == "0") {
            if ($("#ReportType").val() == "0") {
                idReport = "ItsSumInqProdBySales";
            }
            else if ($("#ReportType").val() == "1") {
                idReport = "ItsSaldoInqProdBySales";
            }
        }
        else if ($("#ProductType").val() == "1") {
            if ($("#ReportType").val() == "0") {
                idReport = "ItsSumInqProdByVeh";
            }
            else if ($("#ReportType").val() == "1") {
                idReport = "ItsSaldoInqProdByVeh";
            }
        }
        else if ($("#ProductType").val() == "2") {
            if ($("#ReportType").val() == "0") {
                idReport = "ItsSumInqProdBySource";
            }
            else if ($("#ReportType").val() == "1") {
                idReport = "ItsSaldoInqProdBySource";
            }
        }
        // console.log($('input[name="DateFrom"]').val(), $('input[name="DateTo"]').val(), $("#Area").val(), $('[name=Dealer] option:selected').attr("data-attr"), $("#Outlet").val(), $("#BranchManager").val(), $("#SalesHead").val(), $("#Salesman").val(), $("#ReportType").val(), $("#ProductType").val());
        //$(".ajax-loader").show();
        //$.post("/Reports/Viewer.aspx?rpt=ItsSumInqProdBySales&par=01-Feb-2016;01-Feb-2016;CABANG;6006400001;;;;;0;0&type=export&filename=ItsSumInqProdBySales", '', { responseType: 'arraybuffer' })
        //           .success(function (data) {

        //               var getNow = moment(new Date()).format('DDMMYYYY_HHmmss');
        //               var file = new Blob([data], { type: 'application/vnd.ms-excel' });
        //               saveAs(file, 'sss' + getNow + '.xlsx');

        //               $(".ajax-loader").hide();
        //           })
        //           .error(function (status) {
        //               $(".ajax-loader").hide();
        //           });
        
        widget.showReport({
            id: idReport,
            par: [$('input[name="DateFrom"]').val(), $('input[name="DateTo"]').val(), $("#Area").val(), $('[name=Dealer] option:selected').attr("data-attr"), $("#Outlet").val(),
                $("#BranchManager").val(), $("#SalesHead").val(), $("#Salesman").val(), $("#ReportType").val(), $("#ProductType").val()],
            type: 'export',
            fileName: idReport
        });
        
    }

    function newCombo() {
        widget.select({ selector: "[name=Area]", url: "wh.api/Combo/Areas", optionalText: "--SELECT ALL--" }, function (data) {
            $.each($("[name=Area] option:not(:first)") || [], function (idx, item) { // (not(:first) for remove select all option)
                $(item).attr("data-attr", data[idx].groupNo);
                // $("[name=Area] option:selected").attr("data-attr") // to get groupNo area with selected option
            });
        });

        $("[name=Area]").change(function (e) {
            var id = $(this).val();
            console.log(id);
            widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/ListDealersNew", params: { area: id }, optionalText: "--SELECT ALL--" }, function (data) {
                $.each($("[name=Dealer] option:not(:first)") || [], function (idx, item) { // (not(:first) for remove select all option)
                    $(item).attr("data-attr", data[idx].DealerCode);
                });
            });
            $("[name=Dealer]").prop("selectedIndex", 0);
            $("[name=Dealer]").change();
        });

        $("[name=Dealer]").change(function (e) {
            var id = $(this).val();
            widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/ListBranchesNew", params: { area: $("[name=Area]").val(), comp: id }, optionalText: "--SELECT ALL--" });
            $("[name=Outlet]").prop("selectedIndex", 0);
            $("[name=Outlet]").change();
        });
    }
});