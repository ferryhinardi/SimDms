$('head').append("<link href='assets/css/vendor/common.css' rel='stylesheet'>");
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

             { name: "Area", text: "Area", cls: "span4", type: "select" },
             { name: "BranchManager", text: "Branch Manager", cls: "span4", type: "select" },

             { name: "Dealer", text: "Dealer", cls: "span4", type: "select" },
             { name: "SalesHead", text: "Sales Head", cls: "span4", type: "select" },

             { name: "Outlet", text: "Outlet", cls: "span4", type: "select" },
             { name: "Salesman", text: "Salesman", cls: "span4", type: "select" },
             //{ name: "SalesCoord", text: "Sales Coord", cls: "span4", type: "select" },

             { name: "ReportType", text: "Report Type", cls: "span4", type: "select" },
             { name: "ProductType", text: "Productivity By", cls: "span4", type: "select" },            
            ]
    }

    var widget = new SimDms.Widget(options);

    var paramsSelect = [       
       //{
       //    name: "Salesman", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=10",
       //     optionalText: "--SELECT ALL--",
       //     cascade: 
       //         {
       //             name: "SalesHead",
       //             additionalParams: [
       //                 { name: "EmployeeID", source: "SalesHead", type: "select" },
       //                 { name: "Dealer", source: "Dealer", type: "select" },
       //                 { name: "Outlet", source: "Outlet", type: "select" }
       //             ]
       //         }            
       // },
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
        //{
        //    name: "SalesHead", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=30",
        //    optionalText: "--SELECT ALL--",
        //    cascade: {
        //        name: "BranchManager",
        //        additionalParams: [
        //            { name: "EmployeeID", source: "BranchManager", type: "select" },
        //            { name: "Dealer", source: "Dealer", type: "select" },
        //            { name: "Outlet", source: "Outlet", type: "select" }
        //        ]
        //    }
        //},       
         {
             name: "Outlet", url: "wh.api/combo/ComboOutletList",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Dealer",
                 additionalParams: [
                     { name: "companyCode", source: "Dealer", type: "select" }
                 ]
             }
         },
         {
             name: "Dealer", url: "wh.api/combo/ComboDealerList",
             optionalText: "--SELECT ALL--",
             cascade: {
                 name: "Area",
                 additionalParams: [
                     { name: "groupArea", source: "Area", type: "select" }                    
                 ]
             }
         },         
         //{
         //    name: "BranchManager", url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=40",
         //    optionalText: "--SELECT ALL--",
         //    cascade: {
         //        name: "Outlet",
         //        additionalParams: [                     
         //            { name: "Dealer", source: "Dealer", type: "select" },
         //            { name: "Outlet", source: "Outlet", type: "select" }
         //        ]
         //    }
         //},
    ];

    widget.setSelect(paramsSelect);
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
                widget.select({ selector: "select[name=Area]", url: "wh.api/combo/GroupAreas", optionalText: "--SELECT ALL--" });
            }
        });        
    });
    
    $("#btnProcess").on("click", function () { showReport(); });

    $('#Area').on('change', function (e) {
            if ($('#Area').val() == "") {
                $('#Dealer, #Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
                $("#BranchManager").select2('val', '');
                $("#SalesHead").select2('val', '');
                $("#Salesman").select2('val', '');
            }
            else {
                $('#Dealer').removeAttr('disabled', 'disabled');
            }
    });

    $('#Dealer').on('change', function (e) {
            if ($('#Dealer').val() == "") {
                $('#Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
                $("#BranchManager").select2('val', '');
                $("#SalesHead").select2('val', '');
                $("#Salesman").select2('val', '');
            }
            else {
                $('#Outlet').removeAttr('disabled', 'disabled');
            }
    });

    $('#Outlet').on('change', function (e) {
            if ($('#Outlet').val() == "") {
                $('#BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
                $("#BranchManager").select2('val', '');
                $("#SalesHead").select2('val', '');
                $("#Salesman").select2('val', '');
            }
            else {
                $('#BranchManager').removeAttr('disabled', 'disabled');
                widget.select({
                    selector: "select[name=BranchManager]",
                    url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=40&dealer=" + $('#Dealer').val() + "&outlet=" + $('#Outlet').val()
                });
            }
    });

    $('#BranchManager').on('change', function (e) {
        if ($('#BranchManager').val() == "") {
            $('#SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
            $("#SalesHead").select2('val', '');
            $("#Salesman").select2('val', '');
        }
        else {
            $('#SalesHead').removeAttr('disabled', 'disabled');
            widget.select({
                selector: "select[name=SalesHead]",
                url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=30&dealer=" + $('#Dealer').val() + "&outlet=" + $('#Outlet').val() + "&employeeid=" + $('#BranchManager').val()
            });
        }
    });

    $('#SalesCoord').on('change', function (e) {        
        if ($('#SalesCoord').val() == "") {
            $('#Salesman').attr('disabled', 'disabled');
        }
        else {
            $('#Salesman').removeAttr('disabled', 'disabled');
        }           
    });

    $('#SalesHead').on('change', function (e) {
        if ($('#SalesHead').val() == "") {
            $('#Salesman').attr('disabled', 'disabled');
            $("#Salesman").select2('val', '');
        }
        else {
            $('#Salesman').removeAttr('disabled', 'disabled');
            widget.select({
                selector: "select[name=Salesman]",
                url: "wh.api/inquiryprod/ComboSalesmanHrEmp?lookup=10&dealer=" + $('#Dealer').val() + "&outlet=" + $('#Outlet').val() + "&employeeid=" + $('#SalesHead').val()
            });
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

        $(".ajax-loader").show();
        $.post('wh.api/InquiryProd/GenerateInquiryProd?StartDate=' + $('input[name="DateFrom"]').val() + '&endDate=' + $('input[name="DateTo"]').val() +
            '&area=' + $("#Area").val() + '&dealerCode=' + $("#Dealer").val() + '&outletCode=' + $("#Outlet").val() + '&branchHead=' + $("#BranchManager").val() +
            '&salesHead=' + $("#SalesHead").val() + '&salesman=' + $("#Salesman").val() + '&typeReport=' + $("#ReportType").val() + '&productivityBy=' + $("#ProductType").val() +
            '&idReport=' + idReport)
        .success(function (data) {
            if (!data.success) {
                $(".ajax-loader").hide();
                MsgBox("Tidak ada data yang ditampilkan");
                return;
            }
            else {
                window.location.href = 'wh.api/InquiryProd/GenerateInquiryProductivity?StartDate=' + $('input[name="DateFrom"]').val() + '&endDate=' + $('input[name="DateTo"]').val() +
          '&area=' + $("#Area").val() + '&dealerCode=' + $("#Dealer").val() + '&outletCode=' + $("#Outlet").val() + '&branchHead=' + $("#BranchManager").val() +
          '&salesHead=' + $("#SalesHead").val() + '&salesman=' + $("#Salesman").val() + '&typeReport=' + $("#ReportType").val() + '&productivityBy=' + $("#ProductType").val() +
          '&idReport=' + idReport;
            }
            $(".ajax-loader").hide();
        })
        .error(function (status) {
            $(".ajax-loader").hide();
        });

        //widget.showReport({
        //    id: idReport,
        //    par: [$('input[name="DateFrom"]').val(), $('input[name="DateTo"]').val(), $("#Area").val(), $("#Dealer").val(), $("#Outlet").val(),
        //        $("#BranchManager").val(), $("#SalesHead").val(), $("#Salesman").val(), $("#ReportType").val(), $("#ProductType").val()],
        //    type: 'export',
        //    fileName: idReport
        //});

       
    }    
});