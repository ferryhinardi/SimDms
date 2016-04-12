var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omInquiryProdSalesController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {

        me.data.ProductType = '0';
        me.data.DealerOption = '3';
        me.is1 = me.is2 = me.is3 = true;
        me.is4 = me.is5 = me.is6 = me.is7 = me.is8 = me.is9 = me.is10 = false;

        $('#isArea').prop('checked', true);
        me.data.isArea = true;
        $('#isDealer').prop('checked', true);
        me.data.isDealer = true;
        $('#isOutlet').prop('checked', true);
        me.data.isOutlet = true;
        $('#isBranchHead').prop('checked', false);
        me.data.isBranchHead = false;
        $('#isSalesHead').prop('checked', false);
        me.data.isSalesHead = false;
        $('#isSalesCoordinator').prop('checked', false);
        me.data.isSalesCoordinator = false;
        $('#isWiraniaga').prop('checked', false);
        me.data.isWiraniaga = false;

        $('#chkArea').attr('disabled', 'disabled');
        $('#chkDealer').attr('disabled', 'disabled');
        $('#chkArea').prop('checked', true);
        me.data.chkArea = true;
        $('#chkDealer').prop('checked', true);
        me.data.chkDealer = true;
        $('#chkOutlet').prop('checked', false);
        me.data.chkOutlet = false;

        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.StartDate = e.DateFrom;
              me.data.EndDate = e.DateTo;
          });
        $http.post('om.api/ReportSales/CheckArea').
          success(function (e) {
              me.data.Area = e.Area;
              me.data.Dealer = e.DealerName;
              me.data.DealerID = e.Dealer;
          });

        me.data.Outlet = "<----Select All---->";

        me.gridDetailProdSales.adjust();

    }

    me.ProductType = [
       { "value": '0', "text": 'Productivity Sales Force' },
    ];

    $("[name = 'isArea']").on('change', function () {
        me.data.isArea = $('#isArea').prop('checked');
        me.data.Area = "";
        me.Apply();
    });

    $("[name = 'isDealer']").on('change', function () {
        me.data.isDealer = $('#isDealer').prop('checked');
        me.data.Dealer = "";
        me.Apply();
    });

    $("[name = 'isOutlet']").on('change', function () {
        me.data.isOutlet = $('#isOutlet').prop('checked');
        me.data.Outlet = "";
        me.Apply();
    });

    $("[name = 'isBranchHead']").on('change', function () {
        me.data.isBranchHead = $('#isBranchHead').prop('checked');
        me.data.BranchHead = "";
        me.Apply();
    });

    $("[name = 'isSalesHead']").on('change', function () {
        me.data.isSalesHead = $('#isSalesHead').prop('checked');
        me.data.SalesHead = "";
        me.Apply();
    });

    $("[name = 'isSalesCoordinator']").on('change', function () {
        me.data.isSalesCoordinator = $('#isSalesCoordinator').prop('checked');
        me.data.SalesCoordinator = "";
        me.Apply();
    });

    $("[name = 'isWiraniaga']").on('change', function () {
        me.data.isWiraniaga = $('#isWiraniaga').prop('checked');
        me.data.Wiraniaga = "";
        me.Apply();
    });

    me.Pivot = function () {
        $http.post('om.api/InquirySales/InqProdSales', me.data)
                    .success(function (e) {
                        if (e.success) {
                            me.loadTableData(me.gridDetailProdSales, e.grid);
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
    }

    me.gridDetailProdSales = new webix.ui({
        container: "wxDetailProdSales",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "CompanyName", header: "Company Name", width: 200 },
            { id: "BranchName", header: "Outlet Name", width: 200 },
            { id: "JobCode", header: "Job Code", width: 200 },
            { id: "EmployeeName", header: "Salesman Name", width: 300 },
            { id: "Grade", header: "Grade", width: 200 },
            { id: "Year", header: "Year", width: 150 },
            { id: "InvoiceDate", header: "Productivity", width: 100 },
            { id: "SoldTotal", header: "SoldTotal", width: 100 },

        ]
    });

    me.Area = function () {
        var lookup = Wx.blookup({
            name: "GetInquiryBtn",
            title: "Area",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquiryBtn").withParameters({ Area: "", Dealer: me.data.Dealer, Outlet: "", Detail: "1" }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "Area", title: "Area" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Area = data.Area;
                me.Apply();
            }
        });
    };

    me.Dealer = function () {
        var lookup = Wx.blookup({
            name: "GetInquiryBtn",
            title: "Dealer",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquiryBtn").withParameters({ Area: me.data.Area, Dealer: me.data.Dealer, Outlet: "", Detail: "2" }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "DealerCode", title: "Branch ID" },
                { field: "DealerName", title: "Branch Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Dealer = data.DealerCode;
                me.Apply();
            }
        });
    };

    me.Outlet = function () {
        var lookup = Wx.blookup({
            name: "GetInquiryBtn",
            title: "Outlet",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquiryBtn").withParameters({ Area: me.data.Area, Dealer: me.data.Dealer, Outlet: "", Detail: "3" }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "OutletCode", title: "Outlet ID" },
                { field: "OutletName", title: "Outlet Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Outlet = data.OutletCode;
                me.Apply();
            }
        });
    };

    me.BranchHead = function () {
        var salesType = ""
        if (me.data.DealerOption == '0') { salesType = "SALES" }
        if (me.data.DealerOption == '1') { salesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { salesType = "RETAIL" }
        if (me.data.DealerOption == '3') { salesType = "FPOL" }
        if (me.data.DealerOption == '4') { salesType = "REGPOL" }

        var lookup = Wx.blookup({
            name: "GetInquirySalesLookUpBtn",
            title: "BrachHead",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                .withParameters({ startDate: me.data.StartDate, endDate: me.data.EndDate, area: me.data.Area, dealer: me.data.Dealer, outlet: me.data.Outlet, branchHead: "", salesHead: "", salesCoordinator: "", salesman: "", detail: "4", salesType: salesType }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "BranchHeadID", title: "BrachHead ID" },
                { field: "BranchHeadName", title: "BrachHead Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchHead = data.BranchHeadID;
                me.Apply();
            }
        });
    };

    me.SalesHead = function () {
        var salesType = ""
        if (me.data.DealerOption == '0') { salesType = "SALES" }
        if (me.data.DealerOption == '1') { salesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { salesType = "RETAIL" }
        if (me.data.DealerOption == '3') { salesType = "FPOL" }
        if (me.data.DealerOption == '4') { salesType = "REGPOL" }

        var lookup = Wx.blookup({
            name: "GetInquirySalesLookUpBtn",
            title: "SalesHead",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                .withParameters({ startDate: me.data.StartDate, endDate: me.data.EndDate, area: me.data.Area, dealer: me.data.Dealer, outlet: me.data.Outlet, branchHead: me.data.BranchHead, salesHead: "", salesCoordinator: "", salesman: "", detail: "5", salesType: salesType }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "SalesHeadID", title: "SalesHead ID" },
                { field: "SalesHeadName", title: "SalesHead Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesHead = data.SalesHeadID;
                me.Apply();
            }
        });
    };

    me.SalesCoordinator = function () {
        var salesType = ""
        if (me.data.DealerOption == '0') { salesType = "SALES" }
        if (me.data.DealerOption == '1') { salesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { salesType = "RETAIL" }
        if (me.data.DealerOption == '3') { salesType = "FPOL" }
        if (me.data.DealerOption == '4') { salesType = "REGPOL" }

        var lookup = Wx.blookup({
            name: "GetInquirySalesLookUpBtn",
            title: "SalesCoorditator",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                .withParameters({ startDate: me.data.StartDate, endDate: me.data.EndDate, area: me.data.Area, dealer: me.data.Dealer, outlet: me.data.Outlet, branchHead: me.data.BranchHead, salesHead: me.data.SalesHead, salesCoordinator: "", salesman: "", detail: "6", salesType: salesType }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "SalesCoordinatorID", title: "SalesCoordinator ID" },
                { field: "SalesCoordinatorName", title: "SalesCoordinator Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesCoordinator = data.SalesCoordinatorID;
                me.Apply();
            }
        });
    };

    me.Wiraniaga = function () {
        var salesType = ""
        if (me.data.DealerOption == '0') { salesType = "SALES" }
        if (me.data.DealerOption == '1') { salesType = "WHOLESALE" }
        if (me.data.DealerOption == '2') { salesType = "RETAIL" }
        if (me.data.DealerOption == '3') { salesType = "FPOL" }
        if (me.data.DealerOption == '4') { salesType = "REGPOL" }

        var lookup = Wx.blookup({
            name: "GetInquirySalesLookUpBtn",
            title: "Salesman",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("GetInquirySalesLookUpBtn")
                .withParameters({ startDate: me.data.StartDate, endDate: me.data.EndDate, area: me.data.Area, dealer: me.data.Dealer, outlet: me.data.Outlet, branchHead: me.data.BranchHead, salesHead: me.data.SalesHead, salesCoordinator: me.data.SalesCoorditator, salesman: "", detail: "7", salesType: salesType }),
            defaultSort: "GroupNo asc",
            columns: [
                { field: "SalesmanID", title: "Salesman ID" },
                { field: "SalesmanName", title: "Salesman Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Wiraniaga = data.SalesmanID;
                me.Apply();
            }
        });
    };

    me.printPreview = function () {
        if (me.data.chkOutlet == true) {
            me.data.chkOutlet = "1";
        } else {
            me.data.chkOutlet = "0";
        }
        alert(me.data.chkOutlet);
        var url = "om.api/Inquiry/PrintInqProdSales?";
        var params = "&StartDate=" + $('[name="StartDate"]').val();
        params += "&EndDate=" + $('[name="EndDate"]').val();
        params += "&Area=" + $('[name="Area"]').val();
        params += "&Dealer=" + $('[name="DealerID"]').val();
        params += "&Outlet=" + $('[name="OutletID"]').val();
        params += "&BranchHead=" + $('[name="BranchHeadID"]').val();
        params += "&SalesHead=" + $('[name="SalesHeadID"]').val();
        params += "&SalesCoordinator=" + $('[name="SalesCoordinatorID"]').val();
        params += "&Salesman=" + $('[name="SalesmanID"]').val();
        params += "&SalesCoordinator=" + $('[name="SalesCoordinatorID"]').val();
        params += "&OutletStat=" + me.data.chkOutlet;
        url = url + params;
        window.location = url;
    }

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Inquiry Productivity Sales Force",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" }
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        text: "Periode",
                        type: "controls",
                        cls: "span5 full",
                        items: [
                            { name: "StartDate", cls: "span3", placeHolder: "", type: "ng-datepicker", disable: "AllowPeriod()" },
                            { type: "label", text: "s.d", cls: "span1 mylabel" },
                            { name: "EndDate", cls: "span3", placeHolder: "", type: "ng-datepicker", disable: "AllowPeriod()" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Area",
                        items: [
                            { name: "isArea", type: "check", cls: "span1", float: "left" },
                            { name: "Area", cls: "span7 ", type: "popup", click: "Area()", readonly: true, disable: "data.isArea == false" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Sales Head",
                        items: [
                            { name: "isSalesHead", type: "check", cls: "span1", float: "left" },
                            { name: "SalesHead", cls: "span7 ", type: "popup", click: "SalesHead()", readonly: true, disable: "data.isSalesHead == false" },
                            { name: "SalesHeadID", cls: "span7 ", show: false },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Dealer",
                        items: [
                            { name: "isDealer", type: "check", cls: "span1", float: "left" },
                            { name: "Dealer", cls: "span7 ", type: "popup", click: "Dealer()", readonly: true, disable: "data.isDealer == false" },
                            { name: "DealerID", cls: "span7 ", show: false },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Sales Coordinator",
                        items: [
                            { name: "isSalesCoordinator", type: "check", cls: "span1", float: "left" },
                            { name: "SalesCoordinator", cls: "span7 ", type: "popup", click: "SalesCoordinator()", readonly: true, disable: "data.isSalesCoordinator == false" },
                            { name: "SalesCoordinatorID", cls: "span7 ", show: false },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Outlet",
                        items: [
                            { name: "isOutlet", type: "check", cls: "span1", float: "left" },
                            { name: "Outlet", cls: "span7 ", type: "popup", click: "Outlet()", readonly: true, disable: "data.isOutlet == false" },
                            { name: "OutletID", cls: "span7 ", show: false },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Wiraniaga",
                        items: [
                            { name: "isWiraniaga", type: "check", cls: "span1", float: "left" },
                            { name: "Wiraniaga", cls: "span7 ", type: "popup", click: "Wiraniaga()", readonly: true, disable: "data.isWiraniaga == false" },
                            { name: "WiraniagaID", cls: "span7 ", show: false },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Brach Head",
                        items: [
                            { name: "isBranchHead", type: "check", cls: "span1", float: "left" },
                            { name: "BranchHead", cls: "span7 ", type: "popup", click: "BranchHead()", readonly: true, disable: "data.isBranchHead == false" },
                            { name: "BranchHeadID", cls: "span7 ", show: false },
                        ]
                    },
                    { type: "hr" },
                    { name: "ProductType", cls: "span4 full", type: "select2", text: "", datasource: "ProductType" },
                    {
                        type: "controls",
                        cls: "span3 full",
                        name: "ctrl1",
                        show: "is1",
                        items: [
                            { name: "chkArea", type: "check", text: "Area", cls: "span1", float: "left" },
                            { type: "label", text: "Area", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        name: "ctrl2",
                        show: "is2",
                        items: [
                            { name: "chkDealer", type: "check", text: "Dealer", cls: "span1", float: "left" },
                            { type: "label", text: "Dealer", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        name: "ctrl3",
                        items: [
                            { name: "chkOutlet", type: "check", text: "Outlet", cls: "span1", float: "left" },
                            { type: "label", text: "Outlet", cls: "span7 mylabel" },
                        ]
                    },
                    { type: "hr" },
                    {
                        type: "controls",
                        cls: "span6 full",
                        items: [
                            {
                                type: "buttons", cls: "span2", items: [
                                    { name: "btnPivot", text: "Pivot", icon: "icon-ok", click: "Pivot()", cls: "button small btn btn-success" },
                                ]
                            },
                        ]
                    },
                    {
                        name: "wxDetailProdSales",
                        type: "wxdiv"
                    },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("omInquiryProdSalesController");
    }



});