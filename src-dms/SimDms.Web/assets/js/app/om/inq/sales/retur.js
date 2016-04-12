"use strict";

function omInquiryReturn($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Approved' },
       { "value": '3', "text": 'Cenceled' },
       { "value": '9', "text": 'Finised' }
    ];

    me.InvoiceNo = function () {
        var lookup = Wx.blookup({
            name: "SalesReturnLookup",
            title: "Invoice",
            manager: spSalesManager,
            query: "SalesReturnLookup",
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "Invoice No" },
                { field: "InvoiceDate", title: "Invoice Date", template: "#= moment(InvoiceDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InvoiceNo = data.InvoiceNo;
                me.Apply();
            }
        });

    }

    me.InvoiceNoTo = function () {
        var lookup = Wx.blookup({
            name: "SalesReturnLookup",
            title: "Invoice",
            manager: spSalesManager,
            query: "SalesReturnLookup",
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "Invoice No" },
                { field: "InvoiceDate", title: "Invoice Date", template: "#= moment(InvoiceDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InvoiceNoTo = data.InvoiceNo;
                me.Apply();
            }
        });

    }

    me.ReturnNo = function () {
        var lookup = Wx.blookup({
            name: "SalesReturnLookup",
            title: "Return",
            manager: spSalesManager,
            query: "SalesReturnLookup",
            defaultSort: "ReturnNo asc",
            columns: [
                { field: "ReturnNo", title: "Return No" },
                { field: "ReturnDate", title: "Return Date", template: "#= moment(ReturnDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReturnNo = data.ReturnNo;
                me.Apply();
            }
        });

    }

    me.ReturnNoTo = function () {
        var lookup = Wx.blookup({
            name: "SalesReturnLookup",
            title: "Return",
            manager: spSalesManager,
            query: "SalesReturnLookup",
            defaultSort: "ReturnNo asc",
            columns: [
                { field: "ReturnNo", title: "Return No" },
                { field: "ReturnDate", title: "Return Date", template: "#= moment(ReturnDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReturnNoTo = data.ReturnNo;
                me.Apply();
            }
        });

    }

    me.CustomerCode = function () {
        var lookup = Wx.blookup({
            name: "SoLookup",
            title: "Customer",
            manager: spSalesManager,
            query: "SOCustomerLookup",
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Customer Code" },
                { field: "CustomerName", title: "Customer Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
                me.Apply();
            }
        });

    }

    me.CustomerCodeTo = function () {
        var lookup = Wx.blookup({
            name: "SoLookup",
            title: "Customer",
            manager: spSalesManager,
            query: "SOCustomerLookup",
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Customer Code" },
                { field: "CustomerName", title: "Customer Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCodeTo = data.CustomerCode;
                me.data.CustomerNameTo = data.CustomerName;
                me.Apply();
            }
        });

    }

    me.WareHouseCode = function () {
        var lookup = Wx.blookup({
            name: "WhareHouseLookup",
            title: "WhareHouse",
            manager: spSalesManager,
            query: "WhareHouseLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "WareHouse Code" },
                { field: "RefferenceDesc1", title: "WareHouse Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.WareHouseCode = data.RefferenceCode;
                me.data.WareHouseName = data.RefferenceDesc1;
                me.Apply();
            }
        });

    }

    me.WareHouseCodeTo = function () {
        var lookup = Wx.blookup({
            name: "WhareHouseLookup",
            title: "WhareHouse",
            manager: spSalesManager,
            query: "WhareHouseLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "WareHouse Code" },
                { field: "RefferenceDesc1", title: "WareHouse Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.WareHouseCodeTo = data.RefferenceCode;
                me.data.WareHouseNameTo = data.RefferenceDesc1;
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

        $http.post('om.api/InquirySales/searchSalesReturn?Status=' + me.data.Status + '&ReturnDate=' + ReturnDate + '&ReturnDateTo=' + ReturnDateTo
                                                + '&InvoiceNo=' + me.data.InvoiceNo + '&InvoiceNoTo=' + me.data.InvoiceNoTo
                                                + '&ReturnNo=' + me.data.ReturnNo + '&ReturnNoTo=' + me.data.ReturnNoTo
                                                + '&CustomerCode=' + me.data.CustomerCode + '&CustomerCodeTo=' + me.data.CustomerCodeTo
                                                + '&WareHouseCode=' + me.data.WareHouseCode + '&WareHouseCodeTo=' + me.data.WareHouseCodeTo).
          success(function (data, status, headers, config) {
              me.loadTableData(me.gridReturn, data);

              me.clearTable(me.gridDetaileReturn);
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
            { id: "ReturnNo", header: "No. Retrn", width: 150 },
            { id: "ReturnDate", header: "Tgl. Return", width: 150 },
            { id: "ReferenceNo", header: "No. Ref", width: 150 },
            { id: "ReferenceDate", header: "Tgl. Ref", width: 150 },
            { id: "InvoiceNo", header: "No. Invoice", width: 150 },
            { id: "InvoiceDate", header: "Tgl. Invoice", width: 150 },
            { id: "FakturPajakNo", header: "No. Faktur Pajak", width: 150 },
            { id: "FakturPajakDate", header: "Tgl. Faktur Pajak", width: 150 },
            { id: "CustomerCode", header: "Kode", width: 150 },
            { id: "CustomerName", header: "Pelanggan", width: 200 },
            { id: "Address", header: "Alamat", width: 300 },
            { id: "WareHouseName", header: "Gudang", width: 200 },
            { id: "Remark", header: "Keterangan", width: 150 },
            { id: "Status", header: "Status", width: 150 }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridReturn.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridReturn.getSelectedId().id);

                    var datas = {
                        "ReturnNo": data.ReturnNo
                    }

                    $http.post('om.api/InquirySales/ReturnDetailModel', datas)
                    .success(function (data, status, headers, config) {
                        $(".panel.tabpage1").hide();
                        $(".panel.tabpage1.tB").show();
                        me.loadTableData(me.gridDetailReturn, data);
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.gridDetailReturn = new webix.ui({
        container: "DetailReturn",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 150 },
            { id: "Quantity", header: "Jumlah", width: 100 },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPnBM", header: "PPnBM Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersDPP", header: "DPP Lain-Lain", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersPPn", header: "PPn Lain-Lain", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No. Rangka", width: 150 },
            { id: "Remark", header: "Keterangan", width: 150 }
        ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridReturn.adjust(); break;
            case "tB": me.gridDetailReturn.adjust(); break;
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

    $("[name = 'isNoInvoice']").on('change', function () {
        me.data.isNoInvoice = $('#isNoInvoice').prop('checked');
        me.data.InvoiceNo = "";
        me.data.InvoiceNoTo = "";
        me.Apply();
    });

    $("[name = 'isNoReturn']").on('change', function () {
        me.data.isNoReturn = $('#isNoReturn').prop('checked');
        me.data.ReturnNo = "";
        me.data.ReturnNoTo = "";
        me.Apply();
    });

    $("[name = 'isActivePelanggan']").on('change', function () {
        me.data.isActivePelanggan = $('#isActivePelanggan').prop('checked');
        me.data.CustomerCode = "";
        me.data.CustomerName = "";
        me.data.CustomerCodeTo = "";
        me.data.CustomerNameTo = "";
        me.Apply();
    });

    $("[name = 'isActiveGudang']").on('change', function () {
        me.data.isActiveGudang = $('#isActiveGudang').prop('checked');
        me.data.WareHouseCode = "";
        me.data.WareHouseName = "";
        me.data.WareHouseCodeTo = "";
        me.data.WareHouseNameTo = "";
        me.Apply();
    });


    me.initialize = function () {
        me.data.Status = '';
        me.data.InvoiceNo = '';
        me.data.InvoiceNoTo = '';
        me.data.ReturnNo = '';
        me.data.ReturnNoTo = '';
        me.data.CustomerCode = '';
        me.data.CustomerCodeTo = '';
        me.data.WareHouseCode = '';
        me.data.WareHouseCodeTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoInvoice').prop('checked', false);
        me.data.isNoInvoice = false;
        $('#isNoReturn').prop('checked', false);
        me.data.isNoReturn = false;
        $('#isActivePelanggan').prop('checked', false);
        me.data.isActivePelanggan = false;
        $('#isActiveGudang').prop('checked', false);
        me.data.isActiveGudang = false;
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
        title: "Inquiry Return",
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
                         text: "No. Invoice",
                         type: "controls",
                         items: [
                                 { name: 'isNoInvoice', type: 'check', cls: "span1", text: "", float: 'left' },
                                 { name: "InvoiceNo", cls: "span2", type: "popup", click: "InvoiceNo()", disable: "data.isNoInvoice == false" },
                                 { name: "InvoiceNoTo", cls: "span2", type: "popup", click: "InvoiceNoTo()", disable: "data.isNoInvoice == false" },
                         ]
                     },
                     {
                         text: "No. Return",
                         type: "controls",
                         items: [
                                 { name: 'isNoReturn', type: 'check', cls: "span1", text: "", float: 'left' },
                                 { name: "ReturnNo", cls: "span2", type: "popup", click: "ReturnNo()", disable: "data.isNoReturn == false" },
                                 { name: "ReturnNoTo", cls: "span2", type: "popup", click: "ReturnNoTo()", disable: "data.isNoReturn == false" },
                         ]
                     },
                     { name: 'isActivePelanggan', type: 'check', cls: "", text: "Pelanggan", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "CustomerCode", cls: "span2", type: "popup", btnName: "btnCustomerCode", click: "CustomerCode()", disable: "data.isActivePelanggan == false" },
                                { name: "CustomerName", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "CustomerCodeTo", cls: "span2", type: "popup", btnName: "btnCustomerCodeTo", click: "CustomerCodeTo()", disable: "data.isActivePelanggan == false" },
                                { name: "CustomerNameTo", cls: "span4", readonly: true },
                            ]
                        },
                     { name: 'isActiveGudang', type: 'check', cls: "", text: "Gudang", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "WareHouseCode", cls: "span2", type: "popup", btnName: "btnWareHouseCode", click: "WareHouseCode()", disable: "data.isActiveGudang == false" },
                                { name: "WareHouseName", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "WareHouseCodeTo", cls: "span2", type: "popup", btnName: "btnWareHouseCodeTo", click: "WareHouseCodeTo()", disable: "data.isActiveGudang == false" },
                                { name: "WareHouseNameTo", cls: "span4", readonly: true },
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
                    { name: "tA", text: "Return", cls: "active" },
                    { name: "tB", text: "Detail Return" },
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
                title: "Detai Return",
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
    }



});