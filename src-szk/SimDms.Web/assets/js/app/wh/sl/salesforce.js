$('head').append("<link href='assets/css/vendor/common.css' rel='stylesheet'>");
$(document).ready(function () {
    var options = {
        title: "SALES FORCE ID CARD",
        xtype: "panels",
        toolbars: [
            { name: "btnRefresh", text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
            { name: "btnProcess", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        panels: [{
            name: "pnlFilter",
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
            ]
        },
        {
            name: "pnlResult",
            xtype: "k-grid",
        },
      ]
    }

    var widget = new SimDms.Widget(options);
    var paramsSelect = [        
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
    
    $("#btnProcess").on("click", function () { generateReport(); });
    $("#btnRefresh").on("click", function () { refreshGrid(); });

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

    function generateReport() {
        var area = $('[name="Area"]').val();
        var dealer = $('[name="Dealer"]').val();
        var outlet = $('[name="Outlet"]').val();
        var bm = $('[name="BranchManager"]').val();
        var sh = $('[name="SalesHead"]').val();
        var sl = $('[name="Salesman"]').val();
        var startdate = getSqlDate($('[name="DateFrom"]').val());
        var enddate = getSqlDate($('[name="DateTo"]').val());
        var reporttype = $('[name="ReportType"]').val();
        sdms.info("Please wait...");
        var url = "";
        var params = "&area=" + area;
        params += "&dealer=" + dealer; //+ $('[name="CompanyCode"]').val();
        params += "&outlet=" + outlet;//+ $('[name="BranchCode"]').val();
        params += "&bm=" + bm;
        params += "&sh=" + sh;//+ $('[name="Month"]').val();
        params += "&sl=" + sl;//+ $('[name="Year"]').val();
        params += "&startdate=" + startdate;//+ $('[name="Year"]').val();
        params += "&enddate=" + enddate;
        params += "&createby=";
        params += "&typerpt=" + reporttype;
        //params += "&istype=0"; //+ $('[name="Penjualan"]').val();
        url = "wh.api/InquiryProd/GenerateSalesForceId?";
        console.log(params);
        url = url + params;
        window.location.href = url
    };

    function refreshGrid() {
        var filter = widget.serializeObject('pnlFilter');

        widget.kgrid({
            url: "wh.api/inquiryprod/salesforceid",
            name: "pnlResult",
            params: filter,
            serverBinding: true,
            sort: [{ field: "tipekendaraan", dir: "asc" }],
            columns: [
                { field: "tipekendaraan", width: 200, title: "TYPE" },
                { field: "spkminsatu", width: 100, title: "FRESH\nSPK(N-1)" },
                { field: "p", width: 50, title: "P" },
                { field: "hp", width: 50, title: "HP" },
                { field: "spk", width: 50, title: "SPK" },
                { field: "doo", width: 50, title: "DO" },
                { field: "delivery", width: 75, title: "DELIVERY" },
                { field: "lost", width: 50, title: "LOST" },
                { field: "gt", width: 100, title: "GRAND TOTAL" },
                { field: "outp", width: 50, title: "OUT P" },
                { field: "outhp", width: 50, title: "OUT HP" },
                { field: "outspk", width: 50, title: "OUT SPK" },
                //{ field: "Odometer", width: 100, title: "KM", type: 'number' },
            ],
        });

    };

    function getSqlDate(value) {
        console.log(value);
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});