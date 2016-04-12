var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var pType = "4W";
"use strict";


$(document).ready(function () {
    var options = {
        title: "Inquiry Sales",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                     {
                         text: "Periode",
                         type: "controls",
                         cls: 'span8',
                         items: [
                             { name: 'StartDate', text: 'Date From', type: 'datepicker', cls: 'span2' },
                             { type: "label", text: "s.d", cls: "span1 mylabel" },
                             { name: 'EndDate', text: 'Date To', type: 'datepicker', cls: 'span2' },
                         ]
                     },
                    { name: "Area", text: "Area", cls: "span4 full", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Dealer", text: "Dealer", cls: "span4 full", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Outlet", text: "Outlet", cls: "span4 full", type: "select", opt_text: "-- SELECT ALL --" },
                    {
                        text: "Branch Head",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "BranchHead", cls: "span2 ", text: "Branch Head", type: "popup", readonly: true },
                            { name: "BranchHeadName", cls: "span6 ", readonly: true },
                        ]
                    },
                    {
                        text: "Sales Head",
                        type: "controls",
                        cls: "span8",
                        show: "SHSow",
                        items: [
                            { name: "SalesHead", cls: "span2 ", text: "Sales Head", type: "popup",  readonly: true, show: "SHSow" },
                            { name: "SalesHeadName", cls: "span6 ", readonly: true },
                        ]
                    },                    
                    {
                        text: "Wiraniaga/Sales",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "Wiraniaga", cls: "span2 ", text: "Wiraniaga/Sales", type: "popup",  readonly: true },
                            { name: "WiraniagaName", cls: "span6 ", readonly: true },
                        ]
                    },
                    { name: "Print", text: "Print", cls: "span4 full", type: "select" },
                    {
                        text: "Option",
                        type: "controls",
                        cls: "span8",
                        items: [
                                    { name: "isArea", type: "check", text: "Area", cls: "span2",readonly:true },
                                    { name: "isDealer", type: "check", text: "Dealer", cls: "span2" },
                                    { name: "isOutlet", type: "check", text: "Outlet", cls: "span2" },
                                    { name: "isBrachHead", type: "check", text: "BranchHead", cls: "span2" },
                                    { name: "isSalesHead", type: "check", text: "SalesHead", cls: "span2" },
                                    { name: "isSalesCoordinator", type: "check", text: "SalesCoordinator", cls: "span2" },
                                    { name: "isWiraniaga", type: "check", text: "Wiraniaga", cls: "span2" },
                                    { name: "isGrade", type: "check", text: "Grade", cls: "span2" },
                                    { name: "isWarna", type: "check", text: "Warna", cls: "span2" },
                        ]
                    },                 
                    {
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "isPreType", type: "check", text: "", cls: "span1" },
                            { type: "label", text: "Per-Type", cls: "span2" },
                            { name: "PerType", cls: "span5 ", text: "Per-Type", type: "select" },
                        ]
                    },

                    { name: "SalesType", text: "Dealer", cls: "span4 full", type: "select" },
                    { type: "hr" },
                    {
                        type: "controls",
                        cls: "span6 full",
                        items: [
                            {
                                type: "buttons", cls: "span2", items: [
                                    { name: "btnExcel", text: "Excel", icon: "icon-ok", click: "ExcelData()", cls: "button small btn btn-success" },
                                ]
                            },
                            {
                                type: "buttons", cls: "span2", items: [
                                    { name: "btnRowData", text: "Raw Data", icon: "icon-ok", cls: "button small btn btn-success" },
                                ]
                            },
                            {
                                type: "buttons", cls: "span2", items: [
                                    //{ name: "btnPivot", text: "Pivot", icon: "icon-ok", click: "PivotData()", cls: "button small btn btn-success" },
                                ]
                            },
                        ]
                    },
                ]
            },
              { name: "gridRaw", xtype: "k-grid", cls: 'hide' },
        ]
    };

    
    Wx = new SimDms.Widget(options);
    Wx.default = {};

    
    var setoption=function(x)
    {
        if (x == 0) { //sales report
            $("#isArea").parent().show();
            $("#isDealer").parent().show();
            $("#isOutlet").parent().show();
            $("#isBrachHead").parent().hide();
            $("#isSalesHead").parent().hide();
            $("#isSalesCoordinator").parent().hide();
            $("#isWiraniaga").parent().hide();
            $("#isGrade").parent().hide();
            $("#isWarna").parent().hide();
            //$("#isType").closest("[data-panel]").hide();
            $("#isPreType").closest("[data-panel]").hide();
            $("#SalesType").parent().parent().show();
            

            $("#isOutlet").prop('checked', false).prop('disabled', false);
            $("#isWarna,#isPreType").prop('disabled', false);
            
        }
        else if(x==1) //sales trend report
        {
            $("#isArea").parent().show();
            $("#isDealer").parent().show();
            $("#isOutlet").parent().show();
            $("#isBrachHead").parent().hide();
            $("#isSalesHead").parent().hide();
            $("#isSalesCoordinator").parent().hide();
            $("#isWiraniaga").parent().hide();
            $("#isGrade").parent().hide();
            $("#isWarna").parent().show();
            //$("#isType").closest("[data-panel]").show();
            $("#isPreType").closest("[data-panel]").show();
            $("#SalesType").parent().parent().show();

            $("#isOutlet").prop('checked', false).prop('disabled', false);
            $("#isWarna,#isPreType").prop('disabled', false);
        }
        else if (x == 2) { //productivity Trend Report
            $("#isArea").parent().hide();
            $("#isDealer").parent().hide();
            $("#isOutlet").parent().show();
            $("#isBrachHead").parent().hide();
            $("#isSalesHead").parent().show();
            $("#isSalesCoordinator").parent().show();
            $("#isWiraniaga").parent().show();
            $("#isGrade").parent().show();
            $("#isWarna").parent().show();
            //$("#isType").closest("[data-panel]").show();
            $("#isPreType").closest("[data-panel]").show();
            $("#SalesType").parent().parent().show();

            $("#isOutlet").prop('checked', true).prop('disabled', true);
            $("#isWarna,#isPreType").prop('disabled', false);;
        }
        else if (x == 3) { //comparison : wholesale vs Retail
            $("#isArea").parent().show();
            $("#isDealer").parent().show();
            $("#isOutlet").parent().show();
            $("#isBrachHead").parent().hide();
            $("#isSalesHead").parent().hide();
            $("#isSalesCoordinator").parent().hide();
            $("#isWiraniaga").parent().hide();
            $("#isGrade").parent().hide();
            $("#isWarna").parent().hide();
            //$("#isType").closest("[data-panel]").hide();
            $("#isPreType").closest("[data-panel]").show();
            $("#SalesType").parent().parent().hide();
            $("#isWarna,#isPreType").prop('disabled', false);
        }
        else if (x == 4) { //Comparison : actual vs Target
            $("#isArea").parent().show();
            $("#isDealer").parent().show();
            $("#isOutlet").parent().show();
            $("#isBrachHead").parent().hide();
            $("#isSalesHead").parent().hide();
            $("#isSalesCoordinator").parent().hide();
            $("#isWiraniaga").parent().hide();
            $("#isGrade").parent().hide();
            $("#isWarna").parent().hide();
            //$("#isType").closest("[data-panel]").hide();
            $("#isPreType").closest("[data-panel]").show();
            $("#SalesType").parent().parent().show();
            $("#isWarna,#isPreType").prop('disabled', false);
        }
        else { //month to month & Year to year
            $("#isArea").parent().show();
            $("#isDealer").parent().show();
            $("#isOutlet").parent().show();
            $("#isBrachHead").parent().hide();
            $("#isSalesHead").parent().hide();
            $("#isSalesCoordinator").parent().hide();
            $("#isWiraniaga").parent().hide();
            $("#isGrade").parent().hide();
            $("#isWarna").parent().hide();
            //$("#isType").closest("[data-panel]").hide();
            $("#isPreType").closest("[data-panel]").show();
            $("#SalesType").parent().parent().show();
            $("#isWarna,#isPreType").prop('disabled', false);
        }

    }


    
    
    var lkpBranch = function () {
        sdms.lookup({
            title: 'Lookup Branch Head',
            url: 'wh.api/Inquiry/GetBranchHead?Area=' + $("#Area").val() + '&Dealer=' + $("#Dealer").val() + "&Outlet=" + $("#Outlet").val(),
            sort: [{ field: 'EmployeeId', dir: 'desc' }],
            fields: [
                    { name: 'CompanyCode', text: 'CompanyCode', width: 220 },
                    { name: 'BranchCode', text: 'Branch Code', width: 220 },
                    { name: 'EmployeeId', text: 'Branch Head ID', width: 150 },
                    { name: 'EmployeeName', text: 'Branch Head Name', width: 220 },                    
            ],
            dblclick: function (x) {                
                $("#BranchHead").val(x.EmployeeId)
                $("#BranchHeadName").val(x.EmployeeName)
            },
            onclick: function (x) {
                $("#BranchHead").val(x.EmployeeId)
                $("#BranchHeadName").val(x.EmployeeName)
            }
        });
    }


    var lkpSC = function () {
        sdms.lookup({
            title: 'Lookup Sales Head',
            url: 'wh.api/Inquiry/GetSH?Area=' + $("#Area").val() + '&Dealer=' + $("#Dealer").val() + "&Outlet=" + $("#Outlet").val() + "&BM=" + $("#BranchHead").val(),
            sort: [{ field: 'EmployeeId', dir: 'desc' }],
            fields: [
                    { name: 'CompanyCode', text: 'CompanyCode', width: 220 },
                    { name: 'BranchCode', text: 'Branch Code', width: 220 },
                    { name: 'BranchHeadName', text: 'Branch Head', width: 220 },
                    { name: 'SalesHeadID', text: 'Sales Head ID', width: 150 },
                    { name: 'SalesHeadName', text: 'Sales Head Name', width: 220 },
            ],
            dblclick: function (x) {
                $("#SalesHead").val(x.SalesHeadID)
                $("#SalesHeadName").val(x.SalesHeadName)
            },
            onclick: function (x) {
                $("#SalesHead").val(x.SalesHeadID)
                $("#SalesHeadName").val(x.SalesHeadName)
            }
        });
    }


    var lkpSls = function () {
        sdms.lookup({
            title: 'Lookup Wiraniaga',
            url: 'wh.api/Inquiry/GetSalesman?Area=' + $("#Area").val() + '&Dealer=' + $("#Dealer").val() + "&Outlet=" + $("#Outlet").val() + "&BM=" + $("#BranchHead").val() + "&SH=" + $("#SalesHead").val(),
            sort: [{ field: 'EmployeeId', dir: 'desc' }],
            fields: [
                    { name: 'CompanyCode', text: 'CompanyCode', width: 220 },
                    { name: 'BranchCode', text: 'Branch Code', width: 220 },
                    { name: 'SalesHeadName', text: 'Sales Head Name', width: 220 },
                    { name: 'EmployeeId', text: 'Branch Head ID', width: 150 },
                    { name: 'EmployeeName', text: 'Branch Head Name', width: 220 },
            ],
            dblclick: function (x) {
                $("#Wiraniaga").val(x.EmployeeId)
                $("#WiraniagaName").val(x.EmployeeName)
            },
            onclick: function (x) {
                $("#Wiraniaga").val(x.EmployeeId)
                $("#WiraniagaName").val(x.EmployeeName)
            }
        });
    }



    Wx.render(function () {
        $("#Print,#SalesType").select2('destroy');
        $("#Print option,#SalesType option").remove();        
        $("#isArea,#isDealer,#isSalesHead,#isSalesCoordinator").prop('checked', true).prop('disabled', true);
        $("#PerType").prop('disabled', true);
        setoption(0);

        var lstPrint = [    
            { text: "Sales Report", value: "0" },
            { text: "Sales Trend Report", value: "1" },
            { text: "Productivity Trend Report", value: "2" },
            { text: "Comparation: WholeSale vs Retail", value: "3" },
            { text: "Comparation: Actual vs Target", value: "4" },
            { text: "Comparation: Month to Month & Year to Year", value: "5" },
        ];

        var lstDealer = [
                { text: "Sales", value: "SALES" },
                { text: "Wholesale", value: "WHOLESALE" },
                { text: "Retail Sales", value: "RETAIL" },
                { text: "Faktur Polisi", value: "FPOL" },
        ];


        $.each(lstPrint, function (k, v) {
            $('#Print')
                .append($("<option></option>")
                .attr("value", v.value)
                .text(v.text));
        });

        $.each(lstDealer, function (k, v) {
            $('#SalesType')
                .append($("<option></option>")
                .attr("value", v.value)
                .text(v.text));
        });
        $("#Print").select2().on('change', function () {            
            setoption($(this).val());
        });

        $("#SalesType").select2();

        $('#Area').select2().on('change', function () {          
            //Dealer
            $('#Dealer option').remove();
            $('#Dealer')
            .append($("<option></option>")
            .attr("value", "")
            .text("-- SELECT ALL --"));
            $("#Dealer,#Outlet").select2('val', '');
            Wx.post('wh.api/combo/ListDealersNew?id='+$('#Area').val()+"&Area="+$('#Area').val(), function (result) {
                $.each(result, function (k, v) {
                    $('#Dealer')
                        .append($("<option></option>")
                        .attr("value", v.value)
                        .text(v.text));
                });

            });
        });

        $('#Dealer').select2().on('change', function () {
            $('#Outlet option').remove();
            $('#Outlet')
            .append($("<option></option>")
            .attr("value", "")
            .text("-- SELECT ALL --"));
            $("#Outlet").select2('val', '');

            Wx.post('wh.api/combo/listbranchesNew?area=' + $('#Area').val() + '&comp=' + $('#Dealer option:selected').text()+'&id=' +$('#Dealer').val(), function (result) {
                $.each(result, function (k, v) {
                    $('#Outlet')
                        .append($("<option></option>")
                        .attr("value", v.value)
                        .text(v.text));
                });
            });
        });

        $("#isPreType").on('change', function () {
            $("#PerType").prop('disabled',!$(this).is(':checked'))
        });

        $("#isGrade").on('change', function () {
            if ($(this).is(':checked'))
            {
                $("#isWarna,#isPreType").prop('checked', false).prop('disabled', true);;
                $("#isPreType").change();
                $('#isWiraniaga').prop('checked', true).prop('disabled', true);
            }
            else
            {
                $("#isWarna,#isPreType").prop('disabled', false);
                $('#isWiraniaga').prop('disabled', false);
            }
        });

        $("#isWarna").on('change', function () {
            if ($(this).is(':checked')) {
                $("#isPreType").prop('checked', 'checked');
                $("#isPreType").change();
                $("#isPreType").prop('disabled',true);                
            }
            else
            {
                $("#isPreType").prop('disabled', false);
            }
        });

        //Area
        Wx.post('wh.api/combo/Areas', function (result) {
            $.each(result, function (k, v) {
                $('#Area')
                    .append($("<option></option>")
                    .attr("value", v.value)
                    .text(v.text));
            });

        });


        //Mddel
        Wx.post('wh.api/Combo/InqSalesModel', function (result) {
            $.each(result, function (k, v) {
                $('#PerType')
                    .append($("<option></option>")
                    .attr("value", v.value)
                    .text(v.text));
            });

        });


        //default
        Wx.post('wh.api/Inquiry/DefaultInqSales', function (result) {          
            //$("input[name='StartDate']").val(moment(result.StartDate).format('DD-MMM-YYYY'));
            $("input[name='StartDate']").val('01-Jan-2016');
            $("input[name='EndDate']").val(moment(result.EndDate).format('DD-MMM-YYYY'));
        });

        //lookup
        $("#btnBranchHead").on('click', function () {
            lkpBranch();
        });
        
        $("#btnSalesHead").on('click', function () {
            lkpSC();
        });

        $("#btnWiraniaga").on('click', function () {
            lkpSls();
        });


        //btn
        $("#btnRowData").on('click', function () {       
                var params = {
                    startDate: $("input[name='StartDate']").val(),
                    endDate: $("input[name='EndDate']").val(),
                    area: $("#Area").val(),
                    dealer: $("#Dealer").val(),
                    outlet: $("#Outlet").val(),
                    branchHead: $("#BranchHead").val(),
                    salesHead: $("#SalesHead").val(),
                    salesman: $("#Wiraniaga").val(),
                    SalesType: $("#SalesType").val()
                }

                Wx.kgrid({  
                    url: "wh.api/inquiry/GetReportOmRpSalRgs039Web",
                    name: "gridRaw",
                    params: params,
                    columns: [    
                        { field: "CompanyName", title: "Dealer", width: 300 },
                        { field: "BranchName", title: "Outlet", width: 300 },
                        { field: "BranchHeadName", title: "Branch Head", width: 200 },
                        { field: "SalesHeadName", title: "Sales Head", width: 200 },                        
                        { field: "SalesmanName", title: "Wiraniaga", width: 200 },
                        { field: "Grade", title: "Grade", width: 100 },
                        { field: "ModelCatagory", title: "Category", width: 100 },
                        { field: "SalesModelCode", title: "Sales Model Code", width: 200 },
                        { field: "SalesModelYear", title: "SalesModelYear", Year: 100 },
                        { field: "SalesModelDesc", title: "Sales Model Desc", width: 300 },
                        { field: "FakturPolisiDesc", title: "Faktur Polisi Desc", width: 200 },
                        { field: "MarketModel", title: "Market Model Desc", width: 200 },
                        { field: "GroupMarketModel", title: "Group Market Model", width: 200 },
                        { field: "ColumnMarketModel", title: "ColumnMarketModel", width: 200 },
                        { field: "ColourCode", title: "Colour Code", width: 100 },
                        { field: "ColourName", title: "Colour Name", width: 200 },
                        { field: "SONo", title: "SoNo", width: 200 },
                        { field: "InvoiceNo", title: "Invoice No", width: 200 },
                        { field: "InvoiceDate", title: "Invoice Date", width: 120, template: "#= (InvoiceDate == undefined) ? '-':moment(InvoiceDate).format('DD MMM YYYY') #" },
                        { field: "FakturPolisiNo", title: "Faktur Polis No", width: 120 },
                        { field: "FakturPolisiDate", title: "Faktur Polis Date", width: 120, template: "#= (FakturPolisiDate == undefined) ? '-':moment(FakturPolisiDate).format('DD MMM YYYY') #" },

                    ],
                });
        });     
  
        $("#btnExcel").on('click', function () {
            var params = {
                startDate:  $("input[name='StartDate']").val(),
                endDate: $("input[name='EndDate']").val(),
                area: $("#Area").val(),
                dealer: $("#Dealer").val(),
                outlet: $("#Outlet").val(),
                branchHead: $("#BranchHead").val(),
                salesHead: $("#SalesHead").val(),
                salesman: $("#Wiraniaga").val(),
                SalesType: $("#SalesType").val(),
                isArea:$("#isArea").val(),
                isDealer:$("#isDealer").val(),
                isBrachHead:$("#isBrachHead").val(),
                isSalesHead:$("#isSalesHead").val(),
                isSalesCoordinator:$("#isSalesCoordinator").val(),
                isWiraniaga:$("#isWiraniaga").val(),
                isGrade:$("#isGrade").val(),                
                isWarna:$("#isWarna").val(),
                isModel:$("#isPreType").val(),
                sModel:$("#PerType").val(),
                isoutlet: $("#isOutlet").prop('checked'),
                printType:  $("#Print").val()
            }


            //window.location.href
            window.open('wh.api/Inquiry/GenXlsInqSales' +
            '?startDate=' + $("input[name='StartDate']").val() +
            '&endDate=' + $("input[name='EndDate']").val() +
            '&area=' + $("#Area").val() +
            '&dealer=' + $("#Dealer").val() +
            '&outlet=' + $("#Outlet").val() +
            '&branchHead=' + $("#BranchHead").val() +
            '&salesHead=' + $("#SalesHead").val() +
            '&salesman=' + $("#Wiraniaga").val() +
            '&SalesType=' + $("#SalesType").val() +
            '&isArea=' + $("#isArea").prop('checked') +
            '&isDealer=' + $("#isArea").prop('checked') +
            '&isoutlet=' + $("#isOutlet").prop('checked') +
            '&isBrachHead=' + $("#isBrachHead").prop('checked') +
            '&isSalesHead=' + $("#isSalesHead").prop('checked') +
            '&isSalesCoordinator=' + $("#isSalesCoordinator").prop('checked') +            
            '&isWiraniaga=' + $("#isWiraniaga").prop('checked') +
            '&isGrade=' + $("#isGrade").prop('checked') +
            '&isModel=' + $("#isPreType").prop('checked') +
            '&isWarna=' + $("#isWarna").prop('checked') +
            '&sModel=' + $("#PerType").val() +
            '&dealername=' + $("#Dealer option:selected").text() +
            '&printType=' + $("#Print").val(),
            
            '_blank');

            //StartDate=' + '01-' + $('[name=StartDate]').val() + '&model=' + $("#Variant").select2('val') + '&groupmodel=' + $("#Model").select2('val') + '&variant=' + $("#MVariant").select2('val');
            //Wx.post('wh.api/Inquiry/GenXlsInqSales', params,function (result) {                                
            //});


        });
    });
});


