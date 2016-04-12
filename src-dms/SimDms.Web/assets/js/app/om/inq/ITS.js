var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omInquiryITSController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {

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
        me.data.OutletID = "";

        me.gridDetailProdSales.adjust();

    }

    me.Excel = function () {
        var url = "om.api/Inquiry/PrintInqITS?";
        var params = "&StartDate=" + $('[name="StartDate"]').val();
        params += "&EndDate=" + $('[name="EndDate"]').val();
        params += "&Area=" + $('[name="Area"]').val();
        params += "&Dealer=" + $('[name="DealerID"]').val();
        params += "&Outlet=" + $('[name="OutletID"]').val();
        url = url + params;
        window.location = url;
    }

    me.Query = function () {
        $http.post('om.api/InquirySales/ITS', me.data)
                    .success(function (e) {
                        if (e.success) {
                            if (e.grid != "") {
                                me.loadTableData(me.gridDetailProdSales, e.grid);
                            } else {
                                MsgBox("Tidak Ada Data", MSG_ERROR);
                            }
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
    }

    me.gridDetailProdSales = new webix.ui({
        container: "wxDetailITS",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "DealerAbbreviation", header: "Dealer Name", width: 200 },
            { id: "OutletAbbreviation", header: "Outlet Name", width: 200 },
            { id: "TipeKendaraan", header: "Tipe Kendaraan", width: 200 },
            { id: "Variant", header: "Variant", width: 200 },
            { id: "NewINQ", header: "New INQ", width: 200 },
            { id: "HPNewINQ", header: "HP from INQ", width: 200 },
            { id: "PrcntHPNewINQ", header: "%", width: 200 },
            { id: "SPKfrNI", header: "SPK from NI", width: 200 },
            { id: "PrcntNewINQ", header: "%", width: 200 },
            { id: "OutsINQ", header: "Outs INQ", width: 200 },
            { id: "HPOutsINQ", header: "HP Outs INQ", width: 200 },
            { id: "PrcntHPOutsINQ", header: "%", width: 200 },
            { id: "SPKfrOutsNI", header: "SPK from Outs NI", width: 200 },
            { id: "PrcntOutsINQ", header: "%", width: 200 },
            { id: "TotalINQ", header: "Total INQ", width: 200 },
            { id: "TotalHP", header: "Total HP", width: 200 },
            { id: "PrcntTotalHP", header: "%", width: 200 },
            { id: "TotalSPK", header: "Total SPK", width: 200 },
            { id: "PrcntTotalSPK", header: "%", width: 200 },
            { id: "Cancel", header: "Cancel", width: 200 },
            { id: "Lost", header: "Lost", width: 200 },
            { id: "TestDrive", header: "Test Drive", width: 200 },
            { id: "FakturPolisi", header: "Faktur Polisi", width: 200 },
            { id: "SOH", header: "SOH", width: 200 },

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

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Inquiry ITS",
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
                        cls: "span4 full",
                        text: "Area",
                        items: [
                            { name: "Area", cls: "span7 ", type: "popup", click: "Area()", readonly: true, disable: "data.isArea == false" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4 full",
                        text: "Dealer",
                        items: [
                            { name: "Dealer", cls: "span7 ", type: "popup", click: "Dealer()", readonly: true, disable: "data.isDealer == false" },
                            { name: "DealerID", cls: "span7 ", show: false },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4 full",
                        text: "Outlet",
                        items: [
                            { name: "Outlet", cls: "span7 ", type: "popup", click: "Outlet()", readonly: true, disable: "data.isOutlet == false" },
                            { name: "OutletID", cls: "span7 ", show: false },
                        ]
                    },
                    { type: "hr" },
                    {
                        type: "buttons", cls: "span2", items: [
                            { name: "btnQuery", text: "Query", icon: "icon-ok", click: "Query()", cls: "button small btn btn-success" },
                        ]
                    },
                    {
                        type: "buttons", cls: "span2", items: [
                            { name: "btnExcel", text: "Excel", icon: "icon-ok", click: "Excel()", cls: "button small btn btn-success" },
                        ]
                    },
                    {
                        name: "wxDetailITS",
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
        SimDms.Angular("omInquiryITSController");
    }



});