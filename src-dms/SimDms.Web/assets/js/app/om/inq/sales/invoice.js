"use strict";

function omInquiryInvoice($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Approved' },
       { "value": '3', "text": 'Cenceled' },
       { "value": '9', "text": 'Finised' }
    ];

    me.SONo = function () {
        var lookup = Wx.blookup({
            name: "SoLookup",
            title: "SO",
            manager: spSalesManager,
            query: "InvoiceLookup",
            defaultSort: "SONo asc",
            columns: [
                { field: "SONo", title: "SO No" },
                { field: "SODate", title: "SO Date", template: "#= moment(SODate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SONo = data.SONo;
                me.Apply();
            }
        });

    }

    me.SONoTo = function () {
        var lookup = Wx.blookup({
            name: "SoLookup",
            title: "SO",
            manager: spSalesManager,
            query: "InvoiceLookup",
            defaultSort: "SONo asc",
            columns: [
                { field: "SONo", title: "SO No" },
                { field: "SODate", title: "SO Date", template: "#= moment(SODate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SONoTo = data.SONo;
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

    me.InvoiceNo = function () {
        var lookup = Wx.blookup({
            name: "InvoiceLookup",
            title: "Invoice",
            manager: spSalesManager,
            query: "InvoiceLookup",
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "Invoice No" },
                { field: "InvoiceDate", title: "Invoice Date" },
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
            name: "InvoiceLookup",
            title: "Invoice",
            manager: spSalesManager,
            query: "InvoiceLookup",
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "Invoice No" },
                { field: "InvoiceDate", title: "Invoice Date" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InvoiceNoTo = data.InvoiceNo;
                me.Apply();
            }
        });

    }

    me.CariData = function () {
        var InvoiceDate = '1/1/1900';
        var InvoiceDateTo = '1/1/1900';

        if (me.data.InvoiceDate) {
            var InvoiceDate = new Date(me.data.InvoiceDate).getMonth() + 1 + '/' + new Date(me.data.InvoiceDate).getDate() + '/' + new Date(me.data.InvoiceDate).getFullYear();
            var InvoiceDateTo = new Date(me.data.InvoiceDateTo).getMonth() + 1 + '/' + new Date(me.data.InvoiceDateTo).getDate() + '/' + new Date(me.data.InvoiceDateTo).getFullYear();
        }

        if (me.data.StatusInvoice = "1") {
            $http.post('om.api/InquirySales/searchInvoice?Status=' + me.data.Status + '&InvoiceDate=' + InvoiceDate + '&InvoiceDateTo=' + InvoiceDateTo
                                                    + '&SONo=' + me.data.SONo + '&SONoTo=' + me.data.SONoTo
                                                    + '&CustomerCode=' + me.data.CustomerCode + '&CustomerCodeTo=' + me.data.CustomerCodeTo
                                                    + '&InvoiceNo=' + me.data.InvoiceNo + '&InvoiceNoTo=' + me.data.InvoiceNoTo
                                                    + '&SKPKNo=' + me.data.SKPKNo + '&SKPKNoTo=' + me.data.SKPKNoTo).
              success(function (data, status, headers, config) {
                  $(".panel.tabpage1").hide();
                  $(".panel.tabpage1.tA").show();
                  me.loadTableData(me.gridInvoice, data);

                  //me.clearTable(me.gridDebetNote);
              }).
              error(function (e, status, headers, config) {
                  console.log(e);
              });
        }
        else {
            $http.post('om.api/InquirySales/searchDN?Status=' + me.data.Status + '&InvoiceDate=' + InvoiceDate + '&InvoiceDateTo=' + InvoiceDateTo
                                                    + '&SONo=' + me.data.SONo + '&SONoTo=' + me.data.SONoTo
                                                    + '&CustomerCode=' + me.data.CustomerCode + '&CustomerCodeTo=' + me.data.CustomerCodeTo
                                                    + '&InvoiceNo=' + me.data.InvoiceNo + '&InvoiceNoTo=' + me.data.InvoiceNoTo
                                                    + '&SKPKNo=' + me.data.SKPKNo + '&SKPKNoTo=' + me.data.SKPKNoTo).
              success(function (data, status, headers, config) {
                  $(".panel.tabpage1").hide();
                  $(".panel.tabpage1.tA").show();
                  me.loadTableData(me.gridDebetNote, data);

                 //me.clearTable(me.gridInvoice);
              }).
              error(function (e, status, headers, config) {
                  console.log(e);
              });
        }
    }

    me.gridInvoice = new webix.ui({
        container: "Invoice",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "InvoiceNo", header: "No. Invoice", width: 150 },
            { id: "SalesType", header: "Tipe", width: 50 },
            { id: "InvoiceDate", header: "Tgl. Invoice", width: 150 },
            { id: "SKPKNo", header: "No.SKPK", width: 150 },
            { id: "RefferenceNo", header: "No. Reff", width: 150 },
            { id: "DNNo", header: "No.DN", width: 150 },
            { id: "SONo", header: "No. SO", width: 150 },
            { id: "CustomerCode", header: "Kode", width: 150 },
            { id: "CustomerName", header: "Pelanggan", width: 200 },
            { id: "Address", header: "Alamat", width: 300 },
            { id: "DONo", header: "No.DO", width: 150 },
            { id: "DODate", header: "Tgl. DO", width: 150 },
            { id: "BPKNo", header: "No.BPK", width: 150 },
            { id: "BPKDate", header: "Tgl. BPK", width: 150 },
            { id: "BillTo", header: "Tagih Ke.", width: 200 },
            { id: "FakturPajakNo", header: "No. Paktur Pajak", width: 150 },
            { id: "FakturPajakDate", header: "Tgl. Paktur Pajak", width: 150 },
            { id: "DueDate", header: "Jatuh Tempo", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 150 }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridInvoice.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridInvoice.getSelectedId().id);

                    var datas = {
                        "InvoiceNo": data.InvoiceNo
                    }

                    $http.post('om.api/InquirySales/BPK', datas)
                    .success(function (data, status, headers, config) {
                        $(".panel.tabpage1").hide();
                        $(".panel.tabpage1.tB").show();
                        me.loadTableData(me.gridBPK, data.detail);
                        me.gridInvoicePart.adjust();
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.gridDebetNote = new webix.ui({
        container: "DebetNote",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "", header: "No. DN", width: 150 },
            { id: "", header: "Tipe", width: 50 },
            { id: "", header: "Tgl. DN", width: 150 },
            { id: "", header: "No.SKPK", width: 150 },
            { id: "", header: "No. Reff", width: 150 },
            { id: "", header: "No.Invoice", width: 150 },
            { id: "", header: "No. SO", width: 150 },
            { id: "", header: "Kode", width: 150 },
            { id: "", header: "Pelanggan", width: 200 },
            { id: "", header: "Alamat", width: 300 },
            { id: "", header: "No.DO", width: 150 },
            { id: "", header: "Tgl. DO", width: 150 },
            { id: "", header: "No.BPK", width: 150 },
            { id: "", header: "Tgl. BPK", width: 150 },
            { id: "", header: "Alamat", width: 250 },
            { id: "", header: "Tagih Ke.", width: 200 },
            { id: "", header: "No. Paktur Pajak", width: 150 },
            { id: "", header: "Tgl. Paktur Pajak", width: 150 },
            { id: "", header: "Jatuh Tempo", width: 150 },
            { id: "", header: "Keterangan", width: 200 },
            { id: "", header: "Status", width: 150 }
        ]
    });

    me.gridBPK = new webix.ui({
        container: "BPK",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "BPKNo", header: "No. BPK", width: 150 },
            { id: "Keterangan", header: "Keterangan", width: 850 }
        ],
        on: {
        onSelectChange: function () {
            if (me.gridBPK.getSelectedId() !== undefined) {
                var data = this.getItem(me.gridBPK.getSelectedId().id);

                var datas = {
                    "InvoiceNo": data.InvoiceNo,
                    "BPKNo": data.BPKNo
                }

                $http.post('om.api/InquirySales/InvoiceVin', datas)
                .success(function (data, status, headers, config) {
                    $(".panel.tabpage1").hide();
                    $(".panel.tabpage1.tC").show();
                    me.loadTableData(me.gridInvoiceModel, data.detail);
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
        }
      }
    });

    me.gridInvoicePart = new webix.ui({
        container: "DetailPart",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "", header: "No Part", width: 150 },
            { id: "", header: "Nama Part", width: 150 },
            { id: "", header: "Jumlah", width: 100 },
            { id: "", header: "DPP", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "", header: "PPn", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "", header: "Total", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
        ]
    });

    me.gridInvoiceModel = new webix.ui({
        container: "DetailModel",
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
            { id: "Remark", header: "Keterangan", width: 150 }
        ]
    });

    me.$watch("StatusInvoice", function (a, b) {
        if (a != b) {
            if (a == "1") {
                $(".panel.tabpage1").hide();
                $(".panel.tabpage1.tA").show();
                $('#DebetNote').hide();
                $('#Invoice').show();
                me.gridInvoice.adjust();
                me.clearTable(me.gridDebetNote);
            } else if (a == "2") {
                $(".panel.tabpage1").hide();
                $(".panel.tabpage1.tA").show();
                $('#DebetNote').show();
                $('#Invoice').hide();
                me.gridDebetNote.adjust();
                me.clearTable(me.gridInvoice);
            } 
        }
    })

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridInvoice.adjust(); break;
            case "tB": me.gridBPK.adjust(); me.gridInvoicePart.adjust(); break;
            case "tA": me.gridInvoiceModel.adjust(); break;
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

    $("[name = 'isNoSO']").on('change', function () {
        me.data.isNoSO = $('#isNoSO').prop('checked');
        me.data.SONo = "";
        me.data.SONoTo = "";
        me.Apply();
    });

    $("[name = 'isActivePelanggan']").on('change', function () {
        me.data.isActivePelanggan = $('#isActivePelanggan').prop('checked');
        me.data.CustomerCode = "";
        me.data.CustomerCodeTo = "";
        me.Apply();
    });

    $("[name = 'isNoInvoice']").on('change', function () {
        me.data.isNoInvoice = $('#isNoInvoice').prop('checked');
        me.data.InvoiceNo = "";
        me.data.InvoiceNoTo = "";
        me.Apply();
    });

    $("[name = 'isDNNo']").on('change', function () {
        me.data.isDNNo = $('#isDNNo').prop('checked');
        me.data.DNNo = "";
        me.data.DNNoTo = "";
        me.Apply();
    });

    $("[name = 'isNoSKPK']").on('change', function () {
        me.data.isNoSKPK = $('#isNoSKPK').prop('checked');
        me.data.SKPKNo = "";
        me.data.SKPKNoTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.StatusInvoice = "1";
        me.data.Status = '';
        me.data.SONo = '';
        me.data.SONoTo = '';
        me.data.CustomerCode = '';
        me.data.CustomerCodeTo = '';
        me.data.InvoiceNo = '';
        me.data.InvoiceNoTo = '';
        me.data.SKPKNo = '';
        me.data.SKPKNoTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoSO').prop('checked', false);
        me.data.isNoSO = false;
        $('#isActivePelanggan').prop('checked', false);
        me.data.isActivePelanggan = false;
        $('#isNoInvoice').prop('checked', false);
        me.data.isNoInvoice = false;
        $('#isDNNo').prop('checked', false);
        me.data.isDNNo = false;
        $('#isNoSKPK').prop('checked', false);
        me.data.isNoSKPK = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridInvoice.adjust();
        $('#DebetNote').hide();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Invoice",
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
                        text: "Tgl. Invoice",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "InvoiceDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "InvoiceDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No SO",
                        type: "controls",
                        items: [
                                { name: 'isNoSO', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "SONo", cls: "span2", type: "popup", click: "SONo()", disable: "data.isNoSO == false" },
                                { name: "SONoTo", cls: "span2", type: "popup", click: "SONoTo()", disable: "data.isNoSO == false" },
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
                    {
                        type: "optionbuttons", name: "StatusInvoice", text: "No Invoce", model: "StatusInvoice", click: "StatusInvoice()",
                        items: [
                            { name: "1", text: "Invoice" },
                            { name: "2", text: "Debet Note" },
                        ]
                    },
                     {
                         text: "",
                         type: "controls",
                         show: "StatusInvoice == 1",
                         items: [
                                 { name: 'isNoInvoice', type: 'check', cls: "span1", text: "", float: 'left'},
                                 { name: "InvoiceNo", cls: "span2", type: "popup", click: "InvoiceNo()", disable: "data.isNoInvoice == false" },
                                 { name: "InvoiceNoTo", cls: "span2", type: "popup", click: "InvoiceNoTo()", disable: "data.isNoInvoice == false" },
                         ]
                     },
                     {
                         text: "",
                         type: "controls",
                         show: "StatusInvoice == 2",
                         items: [
                                 { name: 'isDNNo', type: 'check', cls: "span1", text: "", float: 'left' },
                                 { name: "DNNo", cls: "span2", type: "popup", click: "DNNo()", disable: "data.isNoInvoice == false" },
                                 { name: "DNNoTo", cls: "span2", type: "popup", click: "DNNoTo()", disable: "data.isNoInvoice == false" },
                         ]
                     },
                     {
                         text: "No. SKPK",
                         type: "controls",
                         items: [
                                 { name: 'isNoSKPK', type: 'check', cls: "span1", text: "", float: 'left' },
                                 { name: "SKPKNo", cls: "span2", type: "popup", click: "SKPKNo()", disable: "data.isNoSKPK == false" },
                                 { name: "SKPKNoTo", cls: "span2", type: "popup", click: "SKPKNoTo()", disable: "data.isNoSKPK == false" },
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
                    { name: "tA", text: "Invoice", cls: "active" },
                    { name: "tB", text: "Detai Invoice" },
                    { name: "tC", text: "Detai Sales Model" },
                ]
            },
            {
                name: "Invoice",
                title: "Invoice",
                cls: "tabpage1 tA",
                xtype: "wxtable"
            },
            {
                name: "DebetNote",
                title: "DebetNote",
                cls: "tabpage1 tA",
                xtype: "wxtable"
            },
            {
                name: "BPK",
                title: "BPK",
                cls: "tabpage1 tB",
                xtype: "wxtable"
            },
            {
                name: "DetailPart",
                title: "DetailPart",
                cls: "tabpage1 tB",
                xtype: "wxtable"
            },
            {
                name: "DetailModel",
                title: "Detai Sales Model",
                cls: "tabpage1 tC",
                xtype: "wxtable"
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    

    function init(s) {
        SimDms.Angular("omInquiryInvoice");
    }



});