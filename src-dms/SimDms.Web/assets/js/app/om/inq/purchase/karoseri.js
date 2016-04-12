"use strict";

function omInquiryKaroseri($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Approved' },
       { "value": '3', "text": 'Cenceled' }
    ];

    $http.post('om.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    me.SalesModelCodeOld = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeOldLookup",
            title: "Model",
            manager: spSalesManager,
            query: "KaroseriLookup",
            defaultSort: "SalesModelCodeOld asc",
            columns: [
                { field: "SalesModelCodeOld", title: "Kode Sales Model Lama" },
                { field: "Remark", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCodeOld = data.SalesModelCodeOld;
                me.Apply();
            }
        });

    }

    me.SalesModelCodeOldTo = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeOldLookup",
            title: "Model",
            manager: spSalesManager,
            query: "KaroseriLookup",
            defaultSort: "SalesModelCodeOld asc",
            columns: [
                { field: "SalesModelCodeOld", title: "Kode Sales Model Lama" },
                { field: "Remark", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCodeOldTo = data.SalesModelCodeOld;
                me.Apply();
            }
        });

    }

    me.KaroseriSPKNo = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeOldLookup",
            title: "Karoseri SPK No",
            manager: spSalesManager,
            query: "KaroseriLookup",
            defaultSort: "KaroseriSPKNo asc",
            columns: [
                { field: "KaroseriSPKNo", title: "No.Karoseri SPK" },
                { field: "KaroseriSPKDate", title: "Tgl.KaroseriSPK", template: "#= moment(KaroseriSPKDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.KaroseriSPKNo = data.KaroseriSPKNo;
                me.Apply();
            }
        });

    }

    me.KaroseriSPKNoTo = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeOldLookup",
            title: "Karoseri SPK No",
            manager: spSalesManager,
            query: "KaroseriLookup",
            defaultSort: "KaroseriSPKNo asc",
            columns: [
                { field: "KaroseriSPKNo", title: "No.Karoseri SPK" },
                { field: "KaroseriSPKDate", title: "Tgl.KaroseriSPK", template: "#= moment(KaroseriSPKDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.KaroseriSPKNoTo = data.KaroseriSPKNo;
                me.Apply();
            }
        });

    }

    me.SupplierCode = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeOldLookup",
            title: "Pemasok",
            manager: spSalesManager,
            query: "KaroseriLookup",
            defaultSort: "KaroseriSPKNo asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
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
            name: "SalesModelCodeOldLookup",
            title: "Pemasok",
            manager: spSalesManager,
            query: "KaroseriLookup",
            defaultSort: "KaroseriSPKNo asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
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
        var KaroseriSPKtDate = '1/1/1900';
        var KaroseriSPKtDateTo = '1/1/1900';

        if (me.data.KaroseriSPKtDate) {
            var KaroseriSPKtDate = new Date(me.data.KaroseriSPKtDate).getMonth() + 1 + '/' + new Date(me.data.KaroseriSPKtDate).getDate() + '/' + new Date(me.data.KaroseriSPKtDate).getFullYear();
            var KaroseriSPKtDateTo = new Date(me.data.KaroseriSPKtDateTo).getMonth() + 1 + '/' + new Date(me.data.KaroseriSPKtDateTo).getDate() + '/' + new Date(me.data.KaroseriSPKtDateTo).getFullYear();
        }

        $http.post('om.api/InquiryPurchase/searchKaroseri?Status=' + me.data.Status
                                                + '&KaroseriSPKtDate=' + KaroseriSPKtDate + '&KaroseriSPKtDateTo=' + KaroseriSPKtDateTo
                                                + '&SalesModelCodeOld=' + me.data.SalesModelCodeOld + '&SalesModelCodeOldTo=' + me.data.SalesModelCodeOldTo
                                                + '&SalesModelYear=' + me.data.SalesModelYear + '&SalesModelYearTo=' + me.data.SalesModelYearTo
                                                + '&NoSPKKaroseri=' + me.data.KaroseriSPKNo + '&NoSPKKaroseriTo=' + me.data.KaroseriSPKNoTo
                                                + '&SupplierCode=' + me.data.SupplierCode + '&SupplierCodeTo=' + me.data.SupplierCodeTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.griddetailKaroseri);
              me.loadTableData(me.gridKaroseri, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridKaroseri = new webix.ui({
        container: "Karoseri",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "KaroseriSPKNo", header: "No.SPK", width: 150 },
            { id: "KaroseriSPKDate", header: "Tgl.SPK", width: 150 },
            { id: "RefferenceNo", header: "No.Ref", width: 150 },
            { id: "RefferenceDate", header: "Tgl.Ref", width: 150 },
            { id: "SupplierName", header: "Pemasok", width: 250 },
            { id: "SalesModelCodeOld", header: "Sales Model Code lama", width: 150 },
            { id: "SalesModelYear", header: "Salaes Model Year", width: 150 },
            { id: "SalesModelNew", header: "Sales Model COde Baru", width: 150 },
            { id: "AdjustmentNo", header: "Quantity", width: 150 },
            { id: "DPPMaterial", header: "DPP Material", width: 150 },
            { id: "DPPFee", header: "DPP Fee", width: 150 },
            { id: "DPPOther", header: "DPP Lain-Lain", width: 150 },
            { id: "PPn", header: "PPn", width: 150 },
            { id: "Total", header: "Total", width: 150 },
            { id: "DurationDays", header: "Durasi", width: 150 },
            { id: "Remark", header: "Keterangan", width: 350 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridKaroseri.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridKaroseri.getSelectedId().id);
                    var datas = {
                        "KaroseriSPKNo": data.KaroseriSPKNo
                    }

                    $http.post('om.api/InquiryPurchase/GetKaroseriDetail', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.griddetailKaroseri, e.detail);
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

    me.griddetailKaroseri = new webix.ui({
        container: "DetailKaroseri",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "PerlengkapanName", header: "No. Rangka", width: 250 },
            { id: "Quantity", header: "No. Mesin", width: 150 },
            { id: "PerlengkapanName", header: "Kode Warna Lama", width: 250 },
            { id: "Quantity", header: "Kode Warna Baru", width: 150 },
            { id: "Remark", header: "Keterangan", width: 250 },
        ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridKaroseri.adjust(); break;
            case "tB": me.griddetailKaroseri.adjust(); break;
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

    $("[name = 'isNoModel']").on('change', function () {
        me.data.isNoModel = $('#isNoModel').prop('checked');
        me.data.SalesModelCodeOld = "";
        me.data.SalesModelCodeOldTo = "";
        me.Apply();
    });

    $("[name = 'isNoYear']").on('change', function () {
        me.data.isNoYear = $('#isNoYear').prop('checked');
        me.data.SalesModelYear = "";
        me.data.SalesModelYearTo = "";
        me.Apply();
    });

    $("[name = 'isNoKaroseri']").on('change', function () {
        me.data.isNoKaroseri = $('#isNoKaroseri').prop('checked');
        me.data.KaroseriSPKNo = "";
        me.data.KaroseriSPKNoTo = "";
        me.Apply();
    });

    $("[name = 'isActiveSupplier']").on('change', function () {
        me.data.isActiveSupplier = $('#isActiveSupplier').prop('checked');
        me.data.SupplierCode = "";
        me.data.SupplierCodeTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.SalesModelCodeOld = '';
        me.data.SalesModelCodeOldTo = '';
        me.data.SalesModelYear = '';
        me.data.SalesModelYearTo = '';
        me.data.KaroseriSPKNo = '';
        me.data.KaroseriSPKNoTo = '';
        me.data.SupplierCode = '';
        me.data.SupplierCodeTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoModel').prop('checked', false);
        me.data.isNoModel = false;
        $('#isNoYear').prop('checked', false);
        me.data.isNoYear = false;
        $('#isNoKaroseri').prop('checked', false);
        me.data.isNoKaroseri = false;
        $('#isActiveSupplier').prop('checked', false);
        me.data.isActiveSupplier = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridKaroseri.adjust();
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Karoseri",
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
                        text: "Tgl. Karoseri",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "KaroseriSPKtDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "KaroseriSPKDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "Sales Model Code",
                        type: "controls",
                        items: [
                                { name: 'isNoModel', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "SalesModelCodeOld", cls: "span2", type: "popup", btnName: "btnSalesModelCodeOld", click: "SalesModelCodeOld()", disable: "data.isNoModel == false" },
                                { name: "SalesModelCodeOldTo", cls: "span2", type: "popup", btnName: "btnSalesModelCodeOldTo", click: "SalesModelCodeOld()", disable: "data.isNoModel == false" },
                        ]
                    },
                    {
                        text: "Sales Model Year",
                        type: "controls",
                        items: [
                                { name: 'isNoYear', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "SalesModelYear", cls: "span2", type: "select2", datasource: "Year", disable: "data.isNoYear == false" },
                                { name: "SalesModelYearTo", cls: "span2", type: "select2", datasource: "Year", disable: "data.isNoYear == false" },
                        ]
                    },
                    {
                        text: "No SPK Karoseri",
                        type: "controls",
                        items: [
                                { name: 'isNoKaroseri', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "KaroseriSPKNo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNo", click: "KaroseriSPKNo()", disable: "data.isNoKaroseri == false" },
                                { name: "KaroseriSPKNoTo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNoTo", click: "KaroseriSPKNoTo()", disable: "data.isNoKaroseri == false" },
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
                    { name: "tA", text: "Karoseri", cls: "active" },
                    { name: "tB", text: "Detail Detail Karoseri" },
                ]
            },
                    {
                        name: "Karoseri",
                        title: "Karosei",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailKaroseri",
                        title: "Detail Karoseri",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryKaroseri");
    }



});