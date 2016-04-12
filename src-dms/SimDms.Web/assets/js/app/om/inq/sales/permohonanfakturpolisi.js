"use strict";

function omInquiryPermohonanFakturPolis($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Deleted' },
       { "value": '3', "text": 'Canceled' },
       { "value": '9', "text": 'Finished' }
    ];

    me.Jenis = [
       { "value": '0', "text": 'Non Faktur' },
       { "value": '1', "text": 'Faktur' }
    ];

    me.ReqNo = function () {
        var lookup = Wx.blookup({
            name: "SalesReqLookup",
            title: "No Permohonan",
            manager: spSalesManager,
            query: "SalesReqLookup",
            defaultSort: "ReqNo asc",
            columns: [
                { field: "ReqNo", title: "No Permohonan" },
                { field: "ReqDate", title: "tgl permohonan", template: "#= moment(ReqDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReqNo = data.ReqNo;
                me.Apply();
            }
        });

    }

    me.ReqNoTo = function () {
        var lookup = Wx.blookup({
            name: "SalesReqLookup",
            title: "No Permohonan",
            manager: spSalesManager,
            query: "SalesReqLookup",
            defaultSort: "ReqNo asc",
            columns: [
                { field: "ReqNo", title: "No Permohonan" },
                { field: "ReqDate", title: "tgl permohonan", template: "#= moment(ReqDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReqNoTo = data.ReqNo;
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


    me.CariData = function () {
        var ReqDate = '1/1/1900';
        var ReqDateTo = '1/1/1900';

        if (me.data.ReqDate) {
            var ReqDate = new Date(me.data.ReqDate).getMonth() + 1 + '/' + new Date(me.data.ReqDate).getDate() + '/' + new Date(me.data.ReqDate).getFullYear();
            var ReqDateTo = new Date(me.data.ReqDateTo).getMonth() + 1 + '/' + new Date(me.data.ReqDateTo).getDate() + '/' + new Date(me.data.ReqDateTo).getFullYear();
        }

        $http.post('om.api/InquirySales/searchPermohonan?Status=' + me.data.Status + '&Jenis=' + me.data.Jenis
                                                + '&ReqDate=' + ReqDate + '&ReqDateTo=' + ReqDateTo
                                                + '&ReqNo=' + me.data.ReqNo + '&ReqNoTo=' + me.data.ReqNoTo
                                                + '&CustomerCode=' + me.data.CustomerCode + '&CustomerCodeTo=' + me.data.CustomerCodeTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.gridDetailPermohonan);
              me.loadTableData(me.gridPermohonan, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridPermohonan = new webix.ui({
        container: "FakturPolis",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "ReqNo", header: "No.Permohonan", width: 150 },
            { id: "ReqDate", header: "Tgl.Permohonan", width: 150 },
            { id: "ReffNo", header: "No.Ref", width: 150 },
            { id: "ReffDate", header: "Tgl.Ref", width: 150 },
            { id: "SubDealerName", header: "Penjual", width: 200 },
            { id: "StatusFaktur", header: "Status Fakur", width: 100 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridPermohonan.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridPermohonan.getSelectedId().id);
                    var datas = {
                        "ReqNo": data.ReqNo
                    }

                    $http.post('om.api/InquirySales/GetDetailPermohonan', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.gridDetailPermohonan, e.detail);
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

    me.gridDetailPermohonan = new webix.ui({
        container: "DetailFakturPolis",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "SONo", header: "No.SO", width: 150 },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No.Rangka", width: 150 },
            { id: "BPKNo", header: "No Faktur Polis", width: 150 },
            { id: "SalesmanName", header: "Salesman", width: 150 },
            { id: "SKPKNo", header: "SKPK No", width: 150 },
            { id: "SKPKName", header: "SKPK Name", width: 150 },
            { id: "SKPKAddress1", header: "SKPK Alamat 1", width: 150 },
            { id: "SKPKAddress2", header: "SKPK Alamat 2", width: 150 },
            { id: "SKPKAddress3", header: "SKPK Alamat 3", width: 150 },
            { id: "SKPKTelp1", header: "SKPK Telp 1", width: 150 },
            { id: "SKPKTelp1", header: "SKPK Telp 2", width: 150 },
            { id: "SKPKHP", header: "SKPK Hp", width: 150 },
            { id: "SKPKCity", header: "SKPK Kota", width: 150 },
            { id: "SKPKBirthday", header: "SKPK Tanggal Lahir", width: 150 },
            { id: "FakturPolisiName", header: "Faktur Atas Nama", width: 150 },
            { id: "FakturPolisiAddress1", header: "Faktur Alamat 1", width: 150 },
            { id: "FakturPolisiAddress2", header: "Faktur Alamat 2", width: 150 },
            { id: "FakturPolisiAddress3", header: "Faktur Alamat 3", width: 150 },
            { id: "FakturPolisiTelp1", header: "Faktur Telp 1", width: 150 },
            { id: "FakturPolisiTelp2", header: "Faktur Telp 2", width: 150 },
            { id: "FakturPolisiHP", header: "Faktur Hp", width: 150 },
            { id: "PostalCode", header: "Faktur Kode Pos", width: 150 },
            { id: "PostalCodeDesc", header: "Faktur Nama Kode", width: 150 },
            { id: "FakturPolisiCity", header: "Faktur Kota", width: 150 },
            { id: "FakturPolisiBirthday", header: "Faktur Tanggal Lahir", width: 150 },
            { id: "IsCityTransport", header: "Faktur Untuk Angkot", width: 150 },
            { id: "ReasonCode", header: "Reason Code", width: 150 },
            { id: "", header: "Reason Code Desc", width: 150 },
        ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridPermohonan.adjust(); break;
            case "tB": me.gridDetailPermohonan.adjust(); break;
        }
    };

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.Status = "";
        me.Apply();
    });

    $("[name = 'isActiveJenis']").on('change', function () {
        me.data.isActiveJenis = $('#isActiveJenis').prop('checked');
        me.data.Jenis = "";
        me.Apply();
    });

    $("[name = 'isActiveDate']").on('change', function () {
        me.data.isActiveDate = $('#isActiveDate').prop('checked');
        me.Apply();
    });

    $("[name = 'isNoPermohonan']").on('change', function () {
        me.data.isNoPermohonan = $('#isNoPermohonan').prop('checked');
        me.data.ReqNo = "";
        me.data.ReqNoTo = "";
        me.Apply();
    });

    $("[name = 'isActivePelanggan']").on('change', function () {
        me.data.isActivePelanggan = $('#isActivePelanggan').prop('checked');
        me.data.CustomerCode = "";
        me.data.CustomerCodeTo = "";
        me.data.CustomerName = "";
        me.data.CustomerNameTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.Jenis = '';
        me.data.ReqNo = '';
        me.data.ReqNoTo = '';
        me.data.CustomerCode = '';
        me.data.CustomerCodeTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isActiveJenis').prop('checked', false);
        me.data.isActiveJenis = false;
        $('#isNoPermohonan').prop('checked', false);
        me.data.isNoPermohonan = false;
        $('#isActivePelanggan').prop('checked', false);
        me.data.isActivePelanggan = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridPermohonan.adjust();
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Inquiry Permohonan Faktur Polis",
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
                        text: "Tgl. Perlengkapan",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "ReqDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "ReqDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "Jenis",
                        type: "controls",
                        items: [
                            { name: 'isActiveJenis', type: 'check', cls: "span1", text: "Jenis", float: 'left' },
                            { name: "Jenis", opt_text: "", cls: "span3", type: "select2", text: "", datasource: "Jenis", disable: "data.isActiveJenis == false" },

                        ]
                    },
                    {
                        text: "No.Permohonan",
                        type: "controls",
                        items: [
                                { name: 'isNoPermohonan', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "ReqNo", cls: "span2", type: "popup", btnName: "btnReqNo", click: "ReqNo()", disable: "data.isNoPermohonan == false" },
                                { name: "ReqNoTo", cls: "span2", type: "popup", btnName: "btnNReqNoTo", click: "ReqNoTo()", disable: "data.isNoPermohonan == false" },
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
                    { name: "tA", text: "Faktur Polis", cls: "active" },
                    { name: "tB", text: "Detail Faktur polis" },
                ]
            },
                    {
                        name: "FakturPolis",
                        title: "Faktur Polis",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailFakturPolis",
                        title: "Detail Faktur Polis",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryPermohonanFakturPolis");
    }



});