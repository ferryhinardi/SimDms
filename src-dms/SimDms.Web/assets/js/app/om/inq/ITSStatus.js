var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omInquiryITSStatusController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Report = [
       { "value": '0', "text": 'By Dealer' },
       { "value": '0', "text": 'By Type' },
    ];

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
        me.data.GroupModel = "<----Select All---->";
        me.data.GroupModelID = "";
        me.data.TipeKendaraan = "<----Select All---->";
        me.data.TipeKendaraanID = "";
        me.data.Variant = "<----Select All---->";
        me.data.VariantID = "";

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
        container: "wxDetailITSStatus",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "CompanyName", header: "Dealer Name", width: 200 },
            { id: "OutletName", header: "Outlet Name", width: 200 },
            { id: "TipeKendaraan", header: "Tipe Kendaraan", width: 200 },
            { id: "Variant", header: "Variant", width: 200 },
            { id: "LastProgress", header: "LastProgress", width: 200 },
            { id: "SaldoAwal", header: "SaldoAwal", width: 200 },
            { id: "WeekOuts1", header: "WeekOuts1", width: 200 },
            { id: "WeekOuts2", header: "WeekOuts2", width: 200 },
            { id: "WeekOuts3", header: "WeekOuts3", width: 200 },
            { id: "WeekOuts4", header: "WeekOuts4", width: 200 },
            { id: "WeekOuts5", header: "WeekOuts5", width: 200 },
            { id: "WeekOuts6", header: "WeekOuts6", width: 200 },
            { id: "TotalWeekOuts", header: "TotalWeekOuts", width: 200 },
            { id: "Week1", header: "Week1", width: 200 },
            { id: "Week2", header: "Week2", width: 200 },
            { id: "week3", header: "week3", width: 200 },
            { id: "Week4", header: "Week4", width: 200 },
            { id: "Week5", header: "Week5", width: 200 },
            { id: "Week6", header: "Week6", width: 200 },
            { id: "TotalWeek", header: "TotalWeek", width: 200 },
            { id: "Total", header: "Total", width: 200 },

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

    me.GroupModel = function () {
        var lookup = Wx.blookup({
            name: "GroupModel4LookUp",
            title: "Group Model",
            manager: spSalesManager,
            query: "GroupModel4LookUp",
            defaultSort: "GroupModel asc",
            columns: [
                { field: "GroupModel", title: "Group Model" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.GroupModel = data.GroupModel;
                me.Apply();
            }
        });
    };    

    me.TipeKendaraan = function () {
        var lookup = Wx.blookup({
            name: "TipeKendaraan4LookUp",
            title: "Tipe Kendaraan",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("TipeKendaraan4LookUp").withParameters({ TipeKendaraan: me.data.GroupModel, Variant: "" }),
            defaultSort: "TipeKendaraan asc",
            columns: [
                { field: "TipeKendaraan", title: "Tipe Kendaraan" },
                { field: "Variant", title: "Variant" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.TipeKendaraan = data.TipeKendaraan;
                me.Apply();
            }
        });
    };
    
    me.Variant = function () {
        var lookup = Wx.blookup({
            name: "TipeKendaraan4LookUp",
            title: "Tipe Kendaraan",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("TipeKendaraan4LookUp").withParameters({ TipeKendaraan: me.data.TipeKendaraan, Variant: "" }),
            defaultSort: "Variant asc",
            columns: [
                { field: "TipeKendaraan", title: "Tipe Kendaraan" },
                { field: "Variant", title: "Variant" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Variant = data.Variant;
                me.Apply();
            }
        });
    };

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Inquiry ITS with Status",
        xtype: "panels",
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
                            { name: "Area", cls: "span7 ", type: "popup", click: "Area()", readonly: true, disable: "data.isArea == false" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Group Model",
                        items: [
                            { name: "GroupModel", cls: "span7 ", type: "popup", click: "GroupModel()", readonly: true, disable: "data.isArea == false" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Dealer",
                        items: [
                            { name: "Dealer", cls: "span7 ", type: "popup", click: "Dealer()", readonly: true, disable: "data.isDealer == false" },
                            { name: "DealerID", cls: "span7 ", show: false },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Tipe kendaraan",
                        items: [
                            { name: "TipeKendaraan", cls: "span7 ", type: "popup", click: "TipeKendaraan()", readonly: true, disable: "data.isDealer == false" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Outlet",
                        items: [
                            { name: "Outlet", cls: "span7 ", type: "popup", click: "Outlet()", readonly: true, disable: "data.isOutlet == false" },
                            { name: "OutletID", cls: "span7 ", show: false },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Variant",
                        items: [
                            { name: "Variant", cls: "span7 ", type: "popup", click: "Variant()", readonly: true, disable: "data.isOutlet == false" },
                        ]
                    },
                    { type: "hr" },
                    { name: "Report", cls: "span4 full", type: "select2", text: "Report", datasource: "Report" },
                    {
                        type: "optionbuttons", name: "TypeOption", model: "data.TypeOption", text: "Type",
                        items: [
                            { name: "0", text: "Summary" },
                            { name: "1", text: "Detail" },
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
                        name: "wxDetailITSStatus",
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
        SimDms.Angular("omInquiryITSStatusController");
    }



});