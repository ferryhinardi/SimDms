var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omInquiryHPP($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Approved' },
       { "value": '3', "text": 'Canceled' },
       { "value": '9', "text": 'Finished' }
    ];

    me.NoReff = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "No.Reff",
            manager: spSalesManager,
            query: "HPPLookup",
            defaultSort: "RefferenceInvoiceNo asc",
            columns: [
                { field: "RefferenceInvoiceNo", title: "No.Ref.Inv" },
                { field: "RefferenceInvoiceDate", title: "Tgl.Ref.Inv", template: "#= moment(RefferenceInvoiceDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoReff = data.RefferenceInvoiceNo;
                me.Apply();
            }
        });

    }

    me.NoReffTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "No.Reff",
            manager: spSalesManager,
            query: "HPPLookup",
            defaultSort: "RefferenceInvoiceNo asc",
            columns: [
                { field: "RefferenceInvoiceNo", title: "No.Ref.Inv" },
                { field: "RefferenceInvoiceDate", title: "Tgl.Ref.Inv", template: "#= moment(RefferenceInvoiceDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoReffTo = data.RefferenceInvoiceNo;
                me.Apply();
            }
        });

    }

    me.NoHPP = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "No.Reff",
            manager: spSalesManager,
            query: "HPPLookup",
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
            title: "No.Reff",
            manager: spSalesManager,
            query: "HPPLookup",
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

    me.CariData = function () {
        var HPPDate = '1/1/1900';
        var HPPDateTo = '1/1/1900';

        if (me.data.HPPDate) {
            var HPPDate = new Date(me.data.HPPDate).getMonth() + 1 + '/' + new Date(me.data.HPPDate).getDate() + '/' + new Date(me.data.HPPDate).getFullYear();
            var HPPDateTo = new Date(me.data.HPPDateTo).getMonth() + 1 + '/' + new Date(me.data.HPPDateTo).getDate() + '/' + new Date(me.data.HPPDateTo).getFullYear();
        }

        $http.post('om.api/InquiryPurchase/searchHPP?Status=' + me.data.Status + '&HPPDate=' + HPPDate + '&HPPDateTo=' + HPPDate
                                                + '&NoReff=' + me.data.NoReff + '&NoReffTo=' + me.data.NoReffTo
                                                + '&NoHPP=' + me.data.NoHPP + '&NoHPPTo=' + me.data.NoHPPTo
                                                + '&NoPO=' + me.data.NoPO + '&NoPOTo=' + me.data.NoPOTo
                                                + '&SupplierCode=' + me.data.SupplierCode + '&SupplierCodeTo=' + me.data.SupplierCodeTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.griddetailHPP);
              me.clearTable(me.gridModel);
              me.clearTable(me.griddetailModel);
              me.loadTableData(me.gridHPP, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridHPP = new webix.ui({
        container: "HPP",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "HPPNo", header: "No.HPP", width: 150 },
            { id: "HPPDate", header: "Tgl.HPP", width: 150 },
            { id: "PONo", header: "No.PO", width: 150 },
            { id: "RefferenceInvoiceNo", header: "No.Ref.Inv", width: 150 },
            { id: "RefferenceInvoiceDate", header: "Tgl.Ref.Inv", width: 150 },
            { id: "RefferenceFakturPajakNo", header: "No.Ref.FP", width: 150 },
            { id: "RefferenceFakturPajakDate", header: "Tgl.Ref.FP", width: 150 },
            { id: "SupplierName", header: "Pemasok", width: 200 },
            { id: "BillTo", header: "Bayar Ke", width: 200 },
            { id: "DueDate", header: "Jatuh Tempo", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridHPP.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridHPP.getSelectedId().id);
                    //alert(data.HPPNo);
                    var datas = {
                        "HPPNo": data.HPPNo
                    }

                    $http.post('om.api/InquiryPurchase/GetDetailHPP', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.griddetailHPP, e.detail);
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

    me.griddetailHPP = new webix.ui({
        container: "DetailHPP",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        columns: [
            { id: "BPUNo", header: "No.BPU", width: 150 },
            { id: "RefferenceDONo", header: "No.DO", width: 150 },
            { id: "RefferenceSJNo", header: "No.SJ", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 },
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetailHPP.getSelectedId() !== undefined) {
                    var data = this.getItem(me.griddetailHPP.getSelectedId().id);
                    //alert(data.BPUNo);
                    var datas = {
                        "BPUNo": data.BPUNo
                    }

                    $http.post('om.api/InquiryPurchase/GetModel', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tC").show();
                            me.loadTableData(me.gridModel, e.detail);
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

    me.gridModel = new webix.ui({
        container: "Model",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 150 },
            { id: "Quantity", header: "Jumlah", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscTotal", header: "Harga Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPnBM", header: "PPnBM Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersDPP", header: "DPP Lain-Lain", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersPPn", header: "PPn Lain-Lain", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", width: 200 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridModel.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridModel.getSelectedId().id);
                    //alert(data.BPUNo);
                    var datas = {
                        "BPUNo": data.BPUNo,
                        "SalesModelCode": data.SalesModelCode
                    }

                    $http.post('om.api/InquiryPurchase/GetDetailModelHPP', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tD").show();
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
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No Rangka", width: 100 },
            { id: "EngineCode", header: "Kode Mesin", width: 150 },
            { id: "EngineNo", header: "No Mesin", width: 150 },
            { id: "ColourCode", header: "Kode Warna", width: 150 },
        ]
    });

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.Status = "";
        me.Apply();
    });

    $("[name = 'isActiveDate']").on('change', function () {
        me.data.isActiveDate = $('#isActiveDate').prop('checked');
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

    $("[name = 'isNoReff']").on('change', function () {
        me.data.isNoReff = $('#isNoReff').prop('checked');
        me.data.NoReff = "";
        me.data.NoReffTo = "";
        me.Apply();
    });

    $("[name = 'isNoHPP']").on('change', function () {
        me.data.isNoHPP = $('#isNoHPP').prop('checked');
        me.data.NoHPP = "";
        me.data.NoHPPTo = "";
        me.Apply();
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridHPP.adjust(); break;
            case "tB": me.griddetailHPP.adjust(); break;
            case "tC": me.gridModel.adjust(); break;
            case "tD": me.griddetailModel.adjust(); break;
            default:
        }
    };

    me.initialize = function () {
        me.data.Status = '';
        me.data.HPPDate = '';
        me.data.HPPDateTo = '';
        me.data.NoReff = '';
        me.data.NoReffTo = '';
        me.data.NoHPP = '';
        me.data.NoHPPTo = '';
        me.data.NoPO = '';
        me.data.NoPOTo = '';
        me.data.SupplierCode = '';
        me.data.SupplierCodeTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoPO').prop('checked', false);
        me.data.isNoPO = false;
        $('#isActiveSupplier').prop('checked', false);
        me.data.isActiveSupplier = false;
        $('#isNoReff').prop('checked', false);
        me.data.isNoReff = false;
        $('#isNoHPP').prop('checked', false);
        me.data.isNoHPP = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridHPP.adjust();
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Inquiry HPP",
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
                        text: "Tgl. BPU",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "BPUDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "BPUDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
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
                        text: "No.HPP",
                        type: "controls",
                        items: [
                                { name: 'isNoHPP', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoHPP", cls: "span2", type: "popup", btnName: "btnNoHPP", click: "NoHPP()", disable: "data.isNoHPP == false" },
                                { name: "NoHPPTo", cls: "span2", type: "popup", btnName: "btnNoHPPTo", click: "NoHPPTo()", disable: "data.isNoHPP == false" },
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
                            { name: "tA", text: "HPP", cls: "active" },
                            { name: "tB", text: "HPP Detail" },
                            { name: "tC", text: "Model" },
                            { name: "tD", text: "Detail Model" },
                        ]
                    },
                    {
                        name: "HPP",
                        title: "HPP",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailHPP",
                        title: "Detail HPP",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
                    {
                        name: "Model",
                        title: "Model",
                        cls: "tabpage1 tC",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailModel",
                        title: "Detail Model",
                        cls: "tabpage1 tD",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryHPP");
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
    }



});