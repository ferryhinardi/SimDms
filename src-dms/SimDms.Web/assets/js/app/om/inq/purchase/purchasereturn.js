var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omInquiryReturn($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Approved' },
       { "value": '3', "text": 'Canceled' },
       { "value": '9', "text": 'Finished' }
    ];

    me.NoHPP = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "No.HPP",
            manager: spSalesManager,
            query: "ReturnLookup",
            defaultSort: "HPPNo asc",
            columns: [
                { field: "HPPNo", title: "No.HPP" },
                { field: "HPPDate", title: "Tgl.HPP", template: "#= moment(HPPDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoHPP = data.HPPNo;
                me.Apply();
            }
        });

    }

    me.NoHPPTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "No.HPP",
            manager: spSalesManager,
            query: "ReturnLookup",
            defaultSort: "HPPNo asc",
            columns: [
                { field: "HPPNo", title: "No.HPP" },
                { field: "HPPDate", title: "Tgl.HPP", template: "#= moment(HPPDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoHPPTo = data.HPPNo;
                me.Apply();
            }
        });

    }

    me.NoReturn = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "No.HPP",
            manager: spSalesManager,
            query: "ReturnLookup",
            defaultSort: "ReturnNo asc",
            columns: [
                { field: "ReturnNo", title: "No.Return" },
                { field: "ReturnDate", title: "Tgl.Return", template: "#= moment(ReturnDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoReturn = data.ReturnNo;
                me.Apply();
            }
        });

    }

    me.NoReturnTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "No.HPP",
            manager: spSalesManager,
            query: "ReturnLookup",
            defaultSort: "ReturnNo asc",
            columns: [
                { field: "ReturnNo", title: "No.Return" },
                { field: "ReturnDate", title: "Tgl.Return", template: "#= moment(ReturnDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoReturnTo = data.ReturnNo;
                me.Apply();
            }
        });

    }

    me.CariData = function () {
        var ReturnDate = '1/1/1900';
        var ReturnDateTo = '1/1/1900';

        if (me.data.ReturnDate) {
            var ReturnDate = new Date(me.data.ReturnDate).getMonth() + 1 + '/' + new Date(me.data.ReturnDate).getDate() + '/' + new Date(me.data.ReturnDate).getFullYear();
            var ReturnDateTo = new Date(me.data.ReturnDateTo).getMonth() + 1 + '/' + new Date(me.data.ReturnDateTo).getDate() + '/' + new Date(me.data.ReturnDateTo).getFullYear();
        }

        $http.post('om.api/InquiryPurchase/searchPurchaseReturn?Status=' + me.data.Status + '&ReturnDate=' + ReturnDate + '&ReturnDateTo=' + ReturnDateTo
                                                + '&NoHPP=' + me.data.NoHPP + '&NoHPPTo=' + me.data.NoHPPTo
                                                + '&NoReturn=' + me.data.NoReturn + '&NoReturnTo=' + me.data.NoReturnTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.griddetailReturn);
              me.clearTable(me.griddetailModel);
              me.loadTableData(me.gridReturn, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridReturn = new webix.ui({
        container: "Return",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "ReturnNo", header: "No.Return", width: 150 },
            { id: "ReturnDate", header: "Tgl.Return", width: 150 },
            { id: "RefferenceInvoiceNo", header: "No.Ref", width: 150 },
            { id: "RefferenceInvoiceDate", header: "Tgl.Ref", width: 150 },
            { id: "HPPNo", header: "No.HPP", width: 150 },
            { id: "RefferenceFakturPajakNo", header: "Tgl.Ref.FP", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridReturn.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridReturn.getSelectedId().id);
                    var datas = {
                        "ReturnNo": data.ReturnNo
                    }

                    $http.post('om.api/InquiryPurchase/GetDetailReturn', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.griddetailReturn, e.detail);
                            me.loadTableData(me.griddetailModel, e.detail);
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.griddetailReturn = new webix.ui({
        container: "DetailReturn",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "ReturnNo", header: "No.BPU", width: 150 },
            { id: "Remark", header: "Keterangan", width: 900 },
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetailReturn.getSelectedId() !== undefined) {
                    var data = this.getItem(me.griddetailReturn.getSelectedId().id);
                    var datas = {
                        "ReturnNo": data.ReturnNo,
                        "BPUNo": data.BPUNo
                    }

                    $http.post('om.api/InquiryPurchase/GetDetailModel', datas)
                    .success(function (e) {
                        if (e.success) {
                            me.loadTableData(me.griddetailModel, e.detail);
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.griddetailModel = new webix.ui({
        container: "DetailModel",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 150 },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No. Rangka", width: 150 },
            { id: "EngineCode", header: "Kode Mesin", width: 200 },
            { id: "EngineNo", header: "No. Mesin", width: 200 },
            { id: "ColourCode", header: "Kode Warna", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 },
        ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridReturn.adjust(); break;
            case "tB": me.griddetailReturn.adjust(); me.griddetailModel.adjust(); break;
            default:
        }
    };


    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.Status = "";
        me.Apply();
    });

    $("[name = 'isActiveDate']").on('change', function () {
        me.data.isActiveDate = $('#isActiveDate').prop('checked');
        me.Apply();
    });

    $("[name = 'isNoHPP']").on('change', function () {
        me.data.isNoHPP = $('#isNoHPP').prop('checked');
        me.data.NoHPP = "";
        me.data.NoHPPTo = "";
        me.Apply();
    });

    $("[name = 'isNoReturn']").on('change', function () {
        me.data.isNoReturn = $('#isNoReturn').prop('checked');
        me.data.NoReturn = "";
        me.data.NoReturnTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.NoHPP = '';
        me.data.NoHPPTo = '';
        me.data.NoReturn = '';
        me.data.NoReturnTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoReturn').prop('checked', false);
        me.data.isNoReturn = false;
        $('#isNoHPP').prop('checked', false);
        me.data.isNoHPP = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridReturn.adjust();
    }
    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Inquiry Purchase Return",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        text: "Status",
                        type: "controls",
                        items: [
                            { name: 'isActive', type: 'check', cls: "span1", text: "Status", float: 'left' },
                            { name: "Status", opt_text: "", cls: "span3", type: "select2", text: "", datasource: "Status", disable: "data.isActive == false" },

                        ]
                    },
                    {
                        text: "Tgl. Return",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "ReturnDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "ReturnDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No.HPP",
                        type: "controls",
                        items: [
                                { name: 'isNoHPP', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoHPP", cls: "span2", type: "popup", btnName: "btnNoHPP", click: "NoHPP()", disable: "data.isNoHPP == false" },
                                { name: "NoHPPTo", cls: "span2", type: "popup", btnName: "btnNoHPPTo", click: "NoHPPTo()", disable: "data.isNoHPP == false" },
                        ]
                    },
                    {
                        text: "No.Return",
                        type: "controls",
                        items: [
                                { name: 'isNoReturn', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoReturn", cls: "span2", type: "popup", btnName: "btnNoReturn", click: "NoReturn()", disable: "data.isNoReturn == false" },
                                { name: "NoReturnTo", cls: "span2", type: "popup", btnName: "btnNoReturn", click: "NoReturnTo()", disable: "data.isNoReturn == false" },
                        ]
                    },
                    {
                        type: "buttons", cls: "span2", items: [
                            { name: "btnCari", text: "   Telusuri", icon: "icon-search", click: "CariData()", cls: "button small btn btn-success" },
                        ]
                    },
                ]
            },
                    {
                        xtype: "tabs",
                        name: "tabpage1",
                        items: [
                            { name: "tA", text: "Purchase Return", cls: "active" },
                            { name: "tB", text: "Detail Purchase Return" },
                        ]
                    },
                    {
                        name: "Return",
                        title: "Return",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailReturn",
                        title: "Detail Return",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailModel",
                        title: "Detail Model",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryReturn");
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
    }




});