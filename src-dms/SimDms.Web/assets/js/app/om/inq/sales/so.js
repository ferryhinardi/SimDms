"use strict";

function omInquirySO($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Approved' },
       { "value": '3', "text": 'Cenceled' }
    ];

    me.SONo = function () {
        var lookup = Wx.blookup({
            name: "SoLookup",
            title: "SO",
            manager: spSalesManager,
            query: "SOLookup",
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
            query: "SOLookup",
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

    me.Salesman = function () {
        var lookup = Wx.blookup({
            name: "SoLookup",
            title: "Salesman",
            manager: spSalesManager,
            query: "SOSalesmanLookup",
            defaultSort: "Salesman asc",
            columns: [
                { field: "Salesman", title: "Salesman ID" },
                { field: "SalesmanName", title: "SalesmanName Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Salesman = data.Salesman;
                me.data.EmployeeName = data.SalesmanName;
                me.Apply();
            }
        });

    }

    me.SalesmanTo = function () {
        var lookup = Wx.blookup({
            name: "SoLookup",
            title: "Salesman",
            manager: spSalesManager,
            query: "SOSalesmanLookup",
            defaultSort: "Salesman asc",
            columns: [
                { field: "Salesman", title: "Salesman ID" },
                { field: "SalesmanName", title: "SalesmanName Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesmanTo = data.Salesman;
                me.data.EmployeeNameTo = data.SalesmanName;
                me.Apply();
            }
        });

    }

    me.CariData = function () {
        var SODate = '1/1/1900';
        var SODateTo = '1/1/1900';

        if (me.data.SODate) {
            var SODate = new Date(me.data.SODate).getMonth() + 1 + '/' + new Date(me.data.SODate).getDate() + '/' + new Date(me.data.SODate).getFullYear();
            var SODateTo = new Date(me.data.SODateTo).getMonth() + 1 + '/' + new Date(me.data.SODateTo).getDate() + '/' + new Date(me.data.SODateTo).getFullYear();
        }

        $http.post('om.api/InquirySales/searchSO?Status=' + me.data.Status + '&SODate=' + SODate + '&SODateTo=' + SODateTo
                                                + '&SONo=' + me.data.SONo + '&SONoTo=' + me.data.SONoTo
                                                + '&CustomerCode=' + me.data.CustomerCode + '&CustomerCodeTo=' + me.data.CustomerCodeTo
                                                + '&Salesman=' + me.data.Salesman + '&SalesmanTo=' + me.data.SalesmanTo).
          success(function (data, status, headers, config) {
              me.loadTableData(me.gridSO, data);

              me.clearTable(me.gridSalesModel);
              me.clearTable(me.gridInformasiWarna);
              me.clearTable(me.gridInformasiRangka);
              me.clearTable(me.gridAksesoris);
              me.clearTable(me.gridPart);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridSO = new webix.ui({
        container: "SalesOrder",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "SONo", header: "No. SO", width: 150 },
            { id: "SalesType", header: "Tipe", width: 50 },
            { id: "SODate", header: "Tgl. SO", width: 150 },
            { id: "RefferenceNo", header: "No.Ref", width: 150 },
            { id: "RefferenceDate", header: "Tgl.Ref", width: 150 },
            { id: "CustomerCode", header: "Kode", width: 150 },
            { id: "CustomerName", header: "Pelanggan", width: 200 },
            { id: "Address", header: "Alamat", width: 300 },
            { id: "SalesmanName", header: "Salesman", width: 200 },
            { id: "WareHouseName", header: "Gudang", width: 200 },
            { id: "BillTo", header: "Tagih Ke.", width: 200 },
            { id: "ShipTo", header: "Kirim Ke.", width: 200 },
            { id: "GroupPriceName", header: "Group Price", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridSO.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridSO.getSelectedId().id);

                    var datas = {
                        "SONo": data.SONo
                    }

                    $http.post('om.api/InquirySales/DetailSOModel', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.gridSalesModel, e.detail);
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

    me.gridSalesModel = new webix.ui({
        container: "SalesModel",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 150 },
            { id: "QuantitySO", header: "Jumlah", width: 150 },
            { id: "AfterDiscTotal", header: "Harga Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPnBM", header: "PPnBM Setelah Diskon", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "DPPOther", header: "DPP Lain-Lain", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "PPnOther", header: "PPn Lain-Lain", width: 150, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", width: 250 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridSalesModel.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridSalesModel.getSelectedId().id);

                    var datas = {
                        "SONo": data.SONo,
                        "SalesModelCode": data.SalesModelCode
                    }

                    $http.post('om.api/InquirySales/DetailSOModelColour', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tC").show();
                            me.loadTableData(me.gridInformasiWarna, e.detail);
                            me.loadTableData(me.gridInformasiRangka);
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

    me.gridInformasiWarna = new webix.ui({
        container: "InformasiWarna",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "ColourCode", header: "Warna", width: 150 },
            { id: "Quantity", header: "Jumlah", width: 150 },
            { id: "Remark", header: "Keterangan", width: 250 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridInformasiWarna.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridInformasiWarna.getSelectedId().id);

                    var datas = {
                        "SONo": data.SONo,
                        "SalesModelCode": data.SalesModelCode,
                        "ColourCode": data.ColourCode
                    }

                    $http.post('om.api/InquirySales/DetailSOVin', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tC").show();
                            me.loadTableData(me.gridInformasiRangka, e.detail);
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

    me.gridInformasiRangka = new webix.ui({
        container: "InformasiRangka",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No Rangka", width: 150 },
            { id: "EndUserName", header: "Nama STNK", width: 200 },
            { id: "EndUserAddress1", header: "Alamat #1", width: 200 },
            { id: "EndUserAddress2", header: "Alamat #2", width: 200 },
            { id: "EndUserAddress3", header: "Alamat #3", width: 200 },
            { id: "SupplierBBN", header: "Pemasok BBN", width: 200 },
            { id: "CityCode", header: "Kota", width: 150 },
            { id: "BBN", header: "BBN", width: 150 },
            { id: "KIR", header: "KIR", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 },
        ]
    });

    me.gridAksesoris = new webix.ui({
        container: "Aksesoris",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "", header: "Kode Aks.Lain", width: 150 },
            { id: "", header: "Nama Aks.Lain", width: 150 },
            { id: "", header: "DPP Lain", width: 150 },
            { id: "", header: "PPn Lain", width: 150 },
            { id: "", header: "Keterangan", width: 200 },
        ]
    });

    me.gridPart = new webix.ui({
        container: "Part",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "", header: "No Part", width: 150 },
            { id: "", header: "Nama Part", width: 150 },
            { id: "", header: "Jumlah", width: 150 }
        ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridSO.adjust(); break;
            case "tB": me.gridSalesModel.adjust(); break;
            case "tC": me.gridInformasiWarna.adjust(); me.gridInformasiRangka.adjust(); break;
            case "tD": me.gridAksesoris.adjust(); me.gridPart.adjust(); break;
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
        me.data.CustomerName = "";
        me.data.CustomerCodeTo = "";
        me.data.CustomerNameTo = "";
        me.Apply();
    });

    $("[name = 'isActiveSalesman']").on('change', function () {
        me.data.isActiveSalesman = $('#isActiveSalesman').prop('checked');
        me.data.Salesman = "";
        me.data.SalesmanTo = "";
        me.data.EmployeeName = "";
        me.data.EmployeeNameTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.SONo = '';
        me.data.SONoTo = '';
        me.data.CustomerCode = '';
        me.data.CustomerCodeTo = '';
        me.data.Salesman = '';
        me.data.SalesmanTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoSO').prop('checked', false);
        me.data.isNoSO = false;
        $('#isActivePelanggan').prop('checked', false);
        me.data.isActivePelanggan = false;
        $('#isActiveSalesman').prop('checked', false);
        me.data.isActiveSalesman = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridSO.adjust();
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Sales Order",
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
                        text: "Tgl. SO",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "SODate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "SODateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No SO",
                        type: "controls",
                        items: [
                                { name: 'isNoSO', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "SONo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNo", click: "SONo()", disable: "data.isNoSO == false" },
                                { name: "SONoTo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNoTo", click: "SONoTo()", disable: "data.isNoSO == false" },
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
                    { name: 'isActiveSalesman', type: 'check', cls: "", text: "Salesman", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "Salesman", cls: "span2", type: "popup", btnName: "btnSalesman", click: "Salesman()", disable: "data.isActiveSalesman == false" },
                                { name: "EmployeeName", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesmanTo", cls: "span2", type: "popup", btnName: "btnSalesmanTo", click: "SalesmanTo()", disable: "data.isActiveSalesman == false" },
                                { name: "EmployeeNameTo", cls: "span4", readonly: true },
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
                    { name: "tA", text: "Sales Order", cls: "active" },
                    { name: "tB", text: "Sales Model" },
                    { name: "tC", text: "Informasi Kendaraan" },
                    { name: "tD", text: "Aksesoris & Part" },
                ]
            },
                    {
                        name: "SalesOrder",
                        title: "Sales Order",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "SalesModel",
                        title: "SalesModel",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
                    {
                        name: "InformasiWarna",
                        title: "Informasi Warna",
                        cls: "tabpage1 tC",
                        xtype: "wxtable"
                    },
                    {
                        name: "InformasiRangka",
                        title: "Informasi Rangka",
                        cls: "tabpage1 tC",
                        xtype: "wxtable"
                    },
                    {
                        name: "Aksesoris",
                        title: "Aksesoris",
                        cls: "tabpage1 tD",
                        xtype: "wxtable"
                    },
                    {
                        name: "Part",
                        title: "Part",
                        cls: "tabpage1 tD",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquirySO");
    }



});