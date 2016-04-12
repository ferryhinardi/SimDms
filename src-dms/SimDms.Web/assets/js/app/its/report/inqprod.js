var empID = "";
var pstID = "";
var cmpCode = "";
var OutletCode = "";
var nationalSLS = '';
var pType = "";
var branch = "";
var isCEO = false;
"use strict"

function inqprod($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('its.api/inquiry/reporttypes').
    success(function (data, status, headers, config) {
        me.reportTypes = data;
    });

    $http.post('its.api/inquiry/productivityby').
    success(function (data, status, headers, config) {
        me.productivityBy = data;
    });

    $http.post('its.api/inquiry/productivityby').
    success(function (data, status, headers, config) {
        me.BranchManager = data;
    });

    $http.post('its.api/inquiry/productivityby').
   success(function (data, status, headers, config) {
       me.productivityBy = data;
   });

    me.initialize = function () {
        //$(".frame").css({ top: 250 });
        me.getUserProperties();
        me.isComboShow = pType === '4W' ? true : false;
        me.data.ReportType = "0";
        me.data.ProductType = "0";
       
    }

    me.Report = [
        { "value": '0', "text": 'By Dealer' },
        { "value": '1', "text": 'By Type' }
    ];

    me.Type = [
        { "value": '0', "text": 'Summary' },
        { "value": '1', "text": 'Detail' }
    ];

    me.getUserProperties = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/InquiryIts/itsUserProperties',
            success: function (dt) {
                pType = dt.data.ProductType;
                branch = dt.data.Branch;
                isCEO = dt.data.isCOO;
            }
        });
        if (branch == null && isCEO == false) {
            MsgBox("Cabang tempat employee bekerja belum di set (Manpower Management di tab Mutation)!", MSG_INFO);
            $("#btnProcess, #Area, #Dealer, #Outlet, #DateFrom, #DateTo, #BranchManager, #SalesHead, #SalesCoor, #Salesman, #btnBranchManager, #btnSalesHead, #btnSalesCoor, #btnSalesman, #ReportType, #ProductType").attr("disabled", "disabled");
            //$("[name='pnlFilter]").attr("disabled", "disabled");
        } else {
            $.ajax({
                async: false,
                type: "POST",
                url: 'its.api/inquiryits/defaultproductivity',
                success: function (result) {
                    if (result.success) {
                        nationalSLS = result.data.NationalSLS;
                        empID = result.data.EmployeeID;
                        pstID = result.data.PositionID;
                        me.data = result.data;
                        me.Apply();
                        if (nationalSLS == "1") {
                            $('#Dealer, #Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
                        }
                        else {
                            Wx.enable({ value: false, items: ["Area", "Dealer"] });
                            Wx.select({
                                name: "Area",
                                url: "its.api/inquiry/dealermappingareas",
                                selected: result.data.Area
                            });

                            Wx.select({
                                name: "Dealer",
                                url: "its.api/inquiry/dealermappingdealers",
                                selected: result.data.Dealer
                            });
                        }

                        if (result.data.IsBranch || result.data.IsGM) {
                            Wx.enable({ value: false, items: ["Outlet"] });
                            Wx.select({
                                name: "Outlet",
                                url: "its.api/inquiry/outlets",
                                selected: result.data.Outlet
                            });
                        }
                        if (pstID != 'GM' && pstID != 'BM' && pstID != 'SH' && pstID != 'S' && pstID != 'CEO' && pstID != 'COO') {
                            $('#Area, #Dealer, #Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
                            $('#btnBranchManager, #btnSalesHead, #btnSalesCoord, #btnSalesman').attr('disabled', 'disabled');
                        }
                        if (result.data.IsGM) {
                            $('#Area, #Dealer, #Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').removeAttr('disabled');
                            $('#btnBranchManager, #btnSalesHead, #btnSalesCoord, #btnSalesman').removeAttr('disabled');
                        } else {
                            if (result.data.PositionID == 'BM') {
                                $('#Area, #Dealer, #Outlet, #BranchManager').attr('disabled', 'disabled');
                                $('#btnBranchManager').attr('disabled', 'disabled');
                            }
                            if (result.data.PositionID == 'SH') {
                                $('#Area, #Dealer, #Outlet, #BranchManager, #SalesHead, #SalesCoord').attr('disabled', 'disabled');
                                $('#btnBranchManager, #btnSalesHead, #btnSalesCoord').attr('disabled', 'disabled');
                            }
                            if (result.data.PositionID == 'S') {
                                $('#Area, #Dealer, #Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
                                $('#btnBranchManager, #btnSalesHead, #btnSalesCoord, #btnSalesman').attr('disabled', 'disabled');
                            }
                        }
                    }
                    else {
                        Wx.alert(result.message);
                    }
                },
                error: function () {
                    if (result.message != "") {
                        Wx.alert(result.message);
                    }
                    else {
                        Wx.alert("User belum terdaftar di Master Position !");
                    }

                    Wx.showToolbars([]);
                }
            });
        }
        
    }

    $("#btnProcess").on("click", function () { showReport(); });

    me.BM = function () {
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "SalesHead",
            url: "its.api/grid/BranchManager",
            //params: { name: "controls", items: [{ name: "NikSH", param: "NikSH" }, { name: "NikSC", param: "NikSC" }] },
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "EmployeeID", title: "ID" },
                { field: "EmployeeName", title: "Salesman" },
                { field: "TitleName", title: "Jabatan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchManager = data.EmployeeID;
                me.data.BranchManagerName = data.EmployeeName;
                me.data.SalesHead = undefined;
                me.data.SalesHeadName = undefined;
                me.data.Salesman = undefined;
                me.data.SalesmanName = undefined;
                me.Apply();
            }
        });
    }

    me.SalesHead = function () {
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "SalesHead",
            url: "its.api/grid/SalesHead",
            params: { name: "controls", items: [{ name: "BranchManager", param: "BranchManager" }] },
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "EmployeeID", title: "ID" },
                { field: "EmployeeName", title: "Salesman" },
                { field: "TitleName", title: "Jabatan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesHead = data.EmployeeID;
                me.data.SalesHeadName = data.EmployeeName;
                me.data.Salesman = undefined;
                me.data.SalesmanName = undefined;
                me.Apply();
            }
        });
    }

    me.Salesman = function () {
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "Salesman",
            url: "its.api/grid/Salesman",
            params: { name: "controls", items: [{ name: "SalesHead", param: "SalesHead" }, { name: "SalesCoor", param: "SalesCoor" }] },
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "EmployeeID", title: "ID" },
                { field: "EmployeeName", title: "Salesman" },
                { field: "TitleName", title: "Jabatan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Salesman = data.EmployeeID;
                me.data.SalesmanName = data.EmployeeName;
                me.Apply();
            }
        });
    }
    $("[name='SalesHead']").on('blur', function () {
        if ($("[name='SalesHead']").val() == "") {
            me.data.SalesHeadName = undefined;
            $("[name='SalesHeadName']").val("");
        }
    });

    $("[name='SalesCoor']").on('blur', function () {
        if ($("[name='SalesCoor']").val() == "") {
            me.data.SalesCoorName = undefined;
            $("[name='SalesCoorName']").val("");
        }
    });

    $("[name='BranchManager']").on('blur', function () {
        if ($("[name='BranchManager']").val() == "") {
            me.data.BranchManagerName = undefined;
            $("[name='BranchManagerName']").val("");
        }
    });

    $("[name='Salesman']").on('blur', function () {
        if ($("[name='Salesman']").val() == "") {
            me.data.SalesmanName = undefined;
            $("[name='SalesmanName']").val("");
        }
    });
    //$('#Area').on('change', function (e) {
    //    if (nationalSLS == "1") {
    //        if ($('#Area').val() == "") {
    //            $('#Dealer, #Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
    //        }
    //        else {
    //            $('#Dealer').removeAttr('disabled', 'disabled');
    //        }
    //    }
    //});

    //$('#Dealer').on('change', function (e) {
    //    if (nationalSLS == "1") {
    //        if ($('#Dealer').val() == "") {
    //            $('#Outlet, #BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
    //        }
    //        else {
    //            $('#Outlet').removeAttr('disabled', 'disabled');
    //        }
    //    }
    //});

    //$('#Outlet').on('change', function (e) {
    //    if (nationalSLS == "1") {
    //        if ($('#Outlet').val() == "") {
    //            $('#BranchManager, #SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
    //        }
    //        else {
    //            $('#BranchManager').removeAttr('disabled', 'disabled');
    //        }
    //    }
    //});

    //$('#BranchManager').on('change', function (e) {
    //    if ($('#BranchManager').val() == "") {
    //        $('#SalesHead, #SalesCoord, #Salesman').attr('disabled', 'disabled');
    //        if (nationalSLS == "0") {
    //            widget.select({ selector: "select[name=SalesCoord]", url: "its.api/inquiry/combosalesman?employeeid=" + $('#BranchManager').val() });
    //            widget.select({ selector: "select[name=Salesman]", url: "its.api/inquiry/combosalesman?employeeid=" + $('#BranchManager').val() });
    //            widget.post("its.api/inquiry/combosalesman?employeeid=" + $('#BranchManager').val(), function (result) {
    //                $('#SalesHead, #SalesCoord, #Salesman').val(result[0].value);
    //            });
    //        }
    //    }
    //    else {
    //        $('#SalesHead').removeAttr('disabled', 'disabled');
    //    }
    //    //}

    //});

    //$('#SalesCoord').on('change', function (e) {
    //    //if (nationalSLS == "1") {
    //    //    widget.select({ selector: "select[name=Salesman]", url: "its.api/inquiry/combo?lookup=10&positionid=50&employeeid=" + $('#SalesCoord').val(), optionalText: "--SELECT ALL--" });
    //    //}
    //    //else {
    //    if ($('#SalesCoord').val() == "") {
    //        $('#Salesman').attr('disabled', 'disabled');
    //        if (nationalSLS == "0") {
    //            widget.post("its.api/inquiry/combosalesman?employeeid=" + $('#SalesCoord').val(), function (result) {
    //                $('#Salesman').val(result[0].value);
    //            });
    //        }
    //    }
    //    else {
    //        $('#Salesman').removeAttr('disabled', 'disabled');
    //    }
    //    //}        
    //});

    //$('#SalesHead').on('change', function (e) {
    //    //if (nationalSLS == "1") {
    //    //    widget.select({ selector: "select[name=SalesCoord]", url: "its.api/inquiry/combo?lookup=20&positionid=50&employeeid=" + $('#SalesHead').val(), optionalText: "--SELECT ALL--" });
    //    //    widget.select({ selector: "select[name=Salesman]", url: "its.api/inquiry/combo?lookup=10&positionid=50&employeeid=''", optionalText: "--SELECT ALL--" });
    //    //}
    //    //else {
    //    if ($('#SalesHead').val() == "") {
    //        $('#SalesCoord, #Salesman').attr('disabled', 'disabled');
    //        if (nationalSLS == "0") {
    //            widget.select({ selector: "select[name=Salesman]", url: "its.api/inquiry/combosalesman?employeeid=" + $('#SalesHead').val() });
    //            widget.post("its.api/inquiry/combosalesman?employeeid=" + $('#SalesHead').val(), function (result) {
    //                $('#SalesCoord, #Salesman').val(result[0].value);
    //            });
    //        }
    //    }
    //    else {
    //        $('#SalesCoord').removeAttr('disabled', 'disabled');
    //        $('#Salesman').attr('disabled', 'disabled');
    //        if (nationalSLS == "0") {
    //            widget.select({ selector: "select[name=Salesman]", url: "its.api/inquiry/combosalesman" });
    //            widget.post("its.api/inquiry/combosalesman", function (result) {
    //                $('#Salesman').val(result[0].value);
    //            });
    //        }
    //    }
    //    //}

    //});

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
        var DateFrom = $('input[name="DateFrom"]').val();
        var DateTo = $('input[name="DateTo"]').val();
        var Area = $("#Area").val();
        var Dealer = $("#Dealer").val();
        var Outlet = $("#Outlet").val();
        var BranchManager = $("#BranchManager").val();
        var upperSales = pType == "4W" ? $("#SalesHead").val() : $("#SalesCoord").val();
        var Salesman = $("#Salesman").val();
        var ReportType = $("#ReportType").val();
        var ProductType = $("#ProductType").val();

        Wx.showReport({
            //id: 'ItsSumInqProdBySales',
            //par: ['2015-04-01', '2015-04-30', $("#Area").val(), $("#Dealer").val(), $("#Outlet").val(),
            //       $("#BranchManager").val(), $("#SalesHead").val(), $("#Salesman").val(), $("#ReportType").val(), $("#ProductType").val()],
            ////par: ['2015-04-01', '2015-04-30', 'JABODETABEK', '6641401', '664140101', 'S136', '', '', '0', '0'],
            //type: 'export',
            //filename: idReport,
            //rparam : ''
            id: idReport,
            par: [DateFrom, DateTo, Area, Dealer, Outlet, BranchManager, upperSales, Salesman, ReportType, ProductType],
            type: 'export',
            filename: idReport,
            rparam : ''
        });
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry By Productivity",
        xtype: "panels",
        //xtype: "report",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" },
        ],
        panels: [
        {
        items: [
             { name: "PositionID", type: "text", cls: "hide" },
             { name: "EmployeeID", type: "text", cls: "hide" },

             { name: "DateFrom", text: "Periode From", cls: "span4", type: "ng-datepicker" },
             { name: "DateTo", text: "to", cls: "span4", type: "ng-datepicker" },
             { name: "Area", text: "Area", cls: "span4", type: "select" },
             //{ name: "BranchManager", text: "Branch Manager", cls: "span4", type: "select" },
             {
                 type: "controls", text: "Branch Manager", cls: "span4", items: [
                     { name: "BranchManager", model: "data.BranchManager", type: "popup", text: "Branch Manager", cls: "span3", click: "BM()" },
                     { name: "BranchManagerName", model: "data.BranchManagerName", type: "", text: "Branch Manager Name", cls: "span5", readonly: true },
                 ],
             },
             { name: "Dealer", text: "Dealer", cls: "span4", type: "select" },

             //{ name: "SalesHead", text: "Sales Head", cls: "span4", type: "select"},
              {
                  type: "controls", text: "Sales Head", cls: "span4", show: "isComboShow", items: [
                      { name: "SalesHead", model: "data.SalesHead", type: "popup", text: "Sales Head", cls: "span3", click: "SalesHead()" },
                      { name: "SalesHeadName", model: "data.SalesHeadName", type: "", text: "Sales Head Name", cls: "span5", readonly: true },
                  ],
              },

               {
                   type: "controls", text: "Sales Kordinator", cls: "span4", show: "!isComboShow", items: [
                       { name: "SalesCoor", model: "data.SalesCoor", type: "popup", text: "Sales Coor", cls: "span3", click: "SalesHead()" },
                       { name: "SalesCoorName", model: "data.SalesCoorName", type: "", text: "Sales Coor Name", cls: "span5", readonly: true },
                   ],
               },
             { name: "Outlet", text: "Outlet", cls: "span4", type: "select" },
             //{ name: "Salesman", text: "Salesman", cls: "span4", type: "select" },
             {
                 type: "controls", text: "Salesman", cls: "span4", items: [
                     { name: "Salesman", model: "data.Salesman", type: "popup", text: "Salesman", cls: "span3", click: "Salesman()" },
                     { name: "SalesmanName", model: "data.SalesmanName", type: "", text: "Salesman Name", cls: "span5", readonly: true },
                 ],
             },
             { name: "ReportType", text: "Report Type", cls: "span4", type: "select2", datasource: "reportTypes" },


             { name: "ProductType", text: "Productivity By", type: "select2", datasource: "productivityBy" },
        ]
    }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("inqprod");
    }
});

