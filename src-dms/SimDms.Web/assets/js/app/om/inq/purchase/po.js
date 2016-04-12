var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omInquiryPurchaseOrder($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Approved' },
       { "value": '3', "text": 'Canceled' },
       { "value": '9', "text": 'Finished' }
    ];

    me.SupplierCode = function () {
        var lookup = Wx.blookup({
            name: "SupplierCodeLookup",
            title: "Supplier Code",
            manager: spSalesManager,
            query: "SupplierCodeLookup",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "SupplierName", title: "Supplier Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.Apply();
            }
        });

    }

    me.SupplierCodeTo = function () {
        var lookup = Wx.blookup({
            name: "SupplierCodeLookup",
            title: "Supplier Code",
            manager: spSalesManager,
            query: "SupplierCodeLookup",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "SupplierName", title: "Supplier Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCodeTo = data.SupplierCode;
                me.data.SupplierNameTo = data.SupplierName;
                me.Apply();
            }
        });

    }

    me.NoReff = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "Refference",
            manager: spSalesManager,
            query: "POBrowse",
            defaultSort: "RefferenceNo asc",
            columns: [
                { field: "RefferenceNo", title: "Reff No" },
                { field: "RefferenceDate", title: "Reff Date", template: "#= moment(RefferenceDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoReff = data.RefferenceNo;
                me.Apply();
            }
        });

    }

    me.NoReffTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "Refference",
            manager: spSalesManager,
            query: "POBrowse",
            defaultSort: "RefferenceNo asc",
            columns: [
                { field: "RefferenceNo", title: "Reff No" },
                { field: "RefferenceDate", title: "Reff Date", template: "#= moment(RefferenceDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoReffTo = data.RefferenceNo;
                me.Apply();
            }
        });

    }

    me.NoPO = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "PO",
            manager: spSalesManager,
            query: "POBrowse",
            defaultSort: "PONo asc",
            columns: [
                { field: "PONo", title: "PO No" },
                { field: "PODate", title: "PO Date", template: "#= moment(PODate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoPO = data.PONo;
                me.Apply();
            }
        });

    }

    me.NoPOTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "PO",
            manager: spSalesManager,
            query: "POBrowse",
            defaultSort: "PONo asc",
            columns: [
                { field: "PONo", title: "PO No" },
                { field: "PODate", title: "PO Date", template: "#= moment(PODate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoPOTo = data.PONo;
                me.Apply();
            }
        });

    }

    me.CariData = function () {
        var PODate = '1/1/1900';
        var PODateTo = '1/1/1900';

        if (me.data.PODate) {
            var PODate = new Date(me.data.PODate).getMonth() + 1 + '/' + new Date(me.data.PODate).getDate() + '/' + new Date(me.data.PODate).getFullYear();
            var PODateTo = new Date(me.data.PODateTo).getMonth() + 1 + '/' + new Date(me.data.PODateTo).getDate() + '/' + new Date(me.data.PODateTo).getFullYear();
        }
        
        $http.post('om.api/InquiryPurchase/searchPO?Status=' + me.data.Status + '&PODate=' + PODate + '&PODateTo=' + PODateTo
                                                + '&NoReff=' + me.data.NoReff + '&NoReffTo=' + me.data.NoReffTo
                                                + '&NoPO=' + me.data.NoPO + '&NoPOTo=' + me.data.NoPOTo
                                                + '&SupplierCode=' + me.data.SupplierCode + '&SupplierCodeTo=' + me.data.SupplierCodeTo).
          success(function (data, status, headers, config) {
              me.loadTableData(me.gridPO, data);

              me.clearTable(me.griddetailPO);
              me.clearTable(me.griddetailcolourPO);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridPO = new webix.ui({
        container: "PO",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "PONo", header: "No.PO", width: 150 },
            { id: "PODate", header: "Tgl.PO", width: 150 },
            { id: "RefferenceNo", header: "No.Ref", width: 150 },
            { id: "RefferenceDate", header: "Tgl.Ref", width: 150 },
            { id: "SupplierName", header: "Pemasok", width: 250 },
            { id: "BillTo", header: "Bayar ke", width: 200 },
            { id: "ShipTo", header: "Kirim Ke", width: 200 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridPO.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridPO.getSelectedId().id);

                    var datas = {
                        "PONo": data.PONo
                    }

                    $http.post('om.api/InquiryPurchase/GetPO', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.griddetailPO, e.grid);
                            me.loadTableData(me.griddetailcolourPO, e.grid);
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

    me.griddetailPO = new webix.ui({
        container: "DetailPO",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 140, },
            { id: "SalesModelYear", header: "Sales Model Year", width: 140, },
            { id: "QuantityPO", header: "Jumlah PO", width: 100, css: { "text-align": "right" } },
            { id: "AfterDiscTotal", header: "Harga Setelah Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPnBM", header: "PPnBMSetelah Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersDPP", header: "DPP Lain-lain", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersPPn", header: "PPn Lain-lain", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", width: 140 },
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetailPO.getSelectedId() !== undefined) {
                    var data = this.getItem(me.griddetailPO.getSelectedId().id);

                    var datas = {
                        "PONo": data.PONo,
                        "SalesModelCode": data.SalesModelCode
                    }

                    $http.post('om.api/InquiryPurchase/GetDetailPOColour', datas)
                    .success(function (e) {
                        if (e.success) {
                            me.loadTableData(me.griddetailcolourPO, e.grid);
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

    me.griddetailcolourPO = new webix.ui({
        container: "DetailPO",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        columns: [
            { id: "ColourCode", header: "Warna", fillspace: true },
            { id: "Quantity", header: "Jumlah PO", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", fillspace: true },
         ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridPO.adjust(); break;
            case "tB": me.griddetailPO.adjust(); me.griddetailcolourPO.adjust(); break;
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

    $("[name = 'isNoReff']").on('change', function () {
        me.data.isNoReff = $('#isNoReff').prop('checked');
        me.data.NoReff = "";
        me.data.NoReffTo = "";
        me.Apply();
    });

    $("[name = 'isNoPO']").on('change', function () {
        me.data.isNoPO = $('#isNoPO').prop('checked');
        me.data.NoPO = "";
        me.data.NoPOTo = "";
        me.Apply();
    });

    $("[name = 'isActiveSupplier']").on('change', function () {
        me.data.isActiveSupplier = $('#isActiveSupplier').prop('checked');
        me.data.SupplierCode = "";
        me.data.SupplierName = "";
        me.data.SupplierCodeTo = "";
        me.data.SupplierNameTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.NoReff = '';
        me.data.NoReffTo = '';
        me.data.NoPO = '';
        me.data.NoPOTo ='';
        me.data.SupplierCode='';
        me.data.SupplierCodeTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveSupplier').prop('checked', false);
        me.data.isActiveSupplier = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoReff').prop('checked', false);
        me.data.isNoReff = false;
        $('#isNoPO').prop('checked', false);
        me.data.isNoPO = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridPO.adjust();
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Inquiry Purchase Order",
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
                        text: "Tgl. PO",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },                   
                                { name: "PODate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "PODateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No.Ref",
                        type: "controls",
                        items: [
                                { name: 'isNoReff', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoReff", cls: "span2", type: "popup", btnName: "btnNoReff", click: "NoReff()", disable: "data.isNoReff == false" },
                                { name: "NoReffTo", cls: "span2", type: "popup", btnName: "btnNoReffTo", click: "NoReffTo()", disable: "data.isNoReff == false" },
                        ]
                    },
                    {
                        text: "No.PO",
                        type: "controls",
                        items: [
                                { name: 'isNoPO', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoPO", cls: "span2", type: "popup", btnName: "btnNoPO", click: "NoPO()", disable: "data.isNoPO == false" },
                                { name: "NoPOTo", cls: "span2", type: "popup", btnName: "btnNoPOTo", click: "NoPOTo()", disable: "data.isNoPO == false" },
                        ]
                    },
                    { name: 'isActiveSupplier', type: 'check', cls: "", text: "Pemasok", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SupplierCode", cls: "span2", type: "popup", btnName: "btnSupplierCode", click: "SupplierCode()", disable: "data.isActiveSupplier == false" },
                                { name: "SupplierName", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SupplierCodeTo", cls: "span2", type: "popup", btnName: "btnSupplierCodeTo", click: "SupplierCodeTo()", disable: "data.isActiveSupplier == false" },
                                { name: "SupplierNameTo", cls: "span4", readonly: true },
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
                     { name: "tA", text: "Purchase Order", cls: "active" },
                     { name: "tB", text: "Detail Purchase Order" },
                 ]
             },
                    {
                        name: "PO",
                        title: "Purchase Order",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailPO",
                        title: "Detail Purchase Order",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailColourPO",
                        title: "Detail Purchase Order",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryPurchaseOrder");
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
    }



});