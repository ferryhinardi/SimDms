"use strict";

function omInquiryDataKendaraan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Ready' },
       { "value": '1', "text": 'Karoseri' },
       { "value": '2', "text": 'Return' },
       { "value": '3', "text": 'Order' },
       { "value": '4', "text": 'DO' },
       { "value": '5', "text": 'BPK' },
       { "value": '6', "text": 'Sales' },
       { "value": '7', "text": 'Transfer' }
    ];

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

    me.SalesModelCode = function () {
        var lookup = Wx.blookup({
            name: "InquiryDataKendaraanLookup",
            title: "Model",
            manager: spSalesManager,
            query: "InquiryDataKendaraanModelLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCode = data.SalesModelCode;
                me.data.SalesModelDesc = data.SalesModelDesc;
                me.Apply();
            }
        });

    }

    me.SalesModelCodeTo = function () {
        var lookup = Wx.blookup({
            name: "InquiryDataKendaraanLookup",
            title: "Model",
            manager: spSalesManager,
            query: "InquiryDataKendaraanModelLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCodeTo = data.SalesModelCode;
                me.data.SalesModelDescTo = data.SalesModelDesc;
                me.Apply();
            }
        });

    }

    me.ChassisCode = function () {
        var lookup = Wx.blookup({
            name: "InquiryDataKendaraanCHassisCodeLookup",
            title: "Kode Rangka",
            manager: spSalesManager,
            query: "InquiryDataKendaraanCHassisCodeLookup",
            defaultSort: "ChassisCode asc",
            columns: [
                { field: "ChassisCode", title: "Kode Rangka" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ChassisCode = data.ChassisCode;
                me.Apply();
            }
        });

    }

    me.ChassisCodeTo = function () {
        var lookup = Wx.blookup({
            name: "InquiryDataKendaraanCHassisCodeLookup",
            title: "Kode Rangka",
            manager: spSalesManager,
            query: "InquiryDataKendaraanCHassisCodeLookup",
            defaultSort: "ChassisCode asc",
            columns: [
                { field: "ChassisCode", title: "Kode Rangka" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ChassisCodeTo = data.ChassisCode;
                me.Apply();
            }
        });

    }

    me.ChassisNo = function () {
        var lookup = Wx.blookup({
            name: "InquiryDataKendaraanCHassisNoLookup",
            title: "No Rangka",
            manager: spSalesManager,
            query: "InquiryDataKendaraanCHassisNoLookup",
            defaultSort: "ChassisNo asc",
            columns: [
                { field: "ChassisNo", title: "No Rangka" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ChassisNo = data.ChassisNo;
                me.Apply();
            }
        });

    }

    me.ChassisNoTo = function () {
        var lookup = Wx.blookup({
            name: "InquiryDataKendaraanCHassisNoLookup",
            title: "No Rangka",
            manager: spSalesManager,
            query: "InquiryDataKendaraanCHassisNoLookup",
            defaultSort: "ChassisNo asc",
            columns: [
                { field: "ChassisNo", title: "No Rangka" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ChassisNoTo = data.ChassisNo;
                me.Apply();
            }
        });

    }

    $http.post('om.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    me.CariData = function () {
        //$http.post('om.api/InquiryInventory/searchDataKendaraan?Status=' + me.data.Status
        //                                        + '&WareHouseCode=' + me.data.WareHouseCode + '&WareHouseCodeTo=' + me.data.WareHouseCodeTo
        //                                        + '&SalesModelCode=' + me.data.SalesModelCode + '&SalesModelCodeTo=' + me.data.SalesModelCodeTo
        //                                        + '&SalesModelYear=' + me.data.SalesModelYear + '&SalesModelYearTo=' + me.data.SalesModelYearTo
        //                                        + '&ChassisCode=' + me.data.ChassisCode + '&ChassisCodeTo=' + me.data.ChassisCode
        //                                        + '&ChassisNo=' + me.data.ChassisNo + '&ChassisNoTo=' + me.data.ChassisNoTo
        //                                        ).

        var params = {
            Status: me.data.Status, 
            WareHouseCode: me.data.WareHouseCode,  
            WareHouseCodeTo: me.data.WareHouseCodeTo, 
            SalesModelCode: me.data.SalesModelCode, 
            SalesModelCodeTo: me.data.SalesModelCodeTo, 
            SalesModelYear: me.data.SalesModelYear, 
            SalesModelYearTo: me.data.SalesModelYearTo, 
            ChassisCode: me.data.ChassisCode, 
            ChassisCodeTo: me.data.ChassisCodeTo, 
            ChassisNo: me.data.ChassisNo, 
            ChassisNoTo: me.data.ChassisNoTo
        }
        
            $http.post('om.api/InquiryInventory/searchDataKendaraan', params).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              $("p[data-name='tA']").addClass('active');
              $("p[data-name='tB']").removeClass('active');
              me.clearTable(me.gridDetailTransferIn);
              me.loadTableData(me.gridDataKendaraan, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridDataKendaraan = new webix.ui({
        container: "DataKendaraan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        height: 350,
        autoHeight: false,
        columns: [
            { id: "SalesModelCode", header: "Kode Model", width: 150 },
            { id: "SalesModelDesc", header: "Deskripsi", width: 250 },
            { id: "SalesModelYear", header: "Tahun", width: 75 },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No Rangka", width: 150 },
            { id: "EngineCode", header: "Kode Mesin", width: 150 },
            { id: "EngineNo", header: "No Mesin", width: 150 },
            { id: "Status", header: "Status", width: 150 },
            { id: "IsActive", header: "Aktif", width: 150 }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDataKendaraan.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridDataKendaraan.getSelectedId().id);
                    var datas = {
                        "ChassisCode": data.ChassisCode,
                        "ChassisNo": data.ChassisNo
                    }

                    $http.post('om.api/InquiryInventory/GetDetailDataKendaraan', datas)
                    .success(function (data, status, headers, config) {
                        $(".panel.tabpage1").hide();
                        $(".panel.tabpage1.tB").show();
                        $("p[data-name='tB']").addClass('active');
                        $("p[data-name='tA']").removeClass('active');
                        me.detail = data;
                       // alert(data[0].warehouseName);
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.Status = "";
        me.Apply();
    });

    $("[name = 'isWareHouseCode']").on('change', function () {
        me.data.isWareHouseCode = $('#isWareHouseCode').prop('checked');
        me.data.WareHouseCode = "";
        me.data.WareHouseCodeTo = "";
        me.data.WareHouseName = "";
        me.data.WareHouseNameTo = "";
        me.Apply();
    });

    $("[name = 'isSalesModelCode']").on('change', function () {
        me.data.isSalesModelCode = $('#isSalesModelCode').prop('checked');
        me.data.SalesModelCode = "";
        me.data.SalesModelCodeTo = "";
        me.data.SalesModelDesc = "";
        me.data.SalesModelDescTo = "";
        me.Apply();
    });

    $("[name = 'isSalesModelYear']").on('change', function () {
        me.data.isSalesModelYear = $('#isSalesModelYear').prop('checked');
        me.data.SalesModelYear = "";
        me.data.SalesModelYearTo = "";
        me.Apply();
    });

    $("[name = 'isCodeChassis']").on('change', function () {
        me.data.isCodeChassis = $('#isCodeChassis').prop('checked');
        me.data.ChassisCode = "";
        me.data.ChassisCodeTo = "";
        me.Apply();
    });

    $("[name = 'isNoChassis']").on('change', function () {
        me.data.isNoChassis = $('#isNoChassis').prop('checked');
        me.data.ChassisNo = "";
        me.data.ChassisNoTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.WareHouseCode = '';
        me.data.WareHouseCodeTo = '';
        me.data.SalesModelCode = '';
        me.data.SalesModelCodeTo = '';
        me.data.SalesModelYear = '';
        me.data.SalesModelYearTo = '';
        me.data.ChassisCode = '';
        me.data.ChassisCodeTo = '';
        me.data.ChassisNo = '';
        me.data.ChassisNoTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isWareHouseCode').prop('checked', false);
        me.data.isWareHouseCode = false;
        $('#isSalesModelCode').prop('checked', false);
        me.data.isSalesModelCode = false;
        $('#isSalesModelYear').prop('checked', false);
        me.data.isSalesModelYear = false;
        $('#isCodeChassis').prop('checked', false);
        me.data.isCodeChassis = false;
        $('#isNoChassis').prop('checked', false);
        me.data.isNoChassis = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridDataKendaraan.adjust();
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Data Kendaraan",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        text: "Status",
                        type: "controls",
                        items: [
                            { name: 'isActive', type: 'check', cls: "span1", float: 'left' },
                            { name: "Status", cls: "span2", type: "select2", datasource: "Status", text: "Year", disable: "data.isActive == false" },

                        ]
                    },
                    { name: 'isWareHouseCode', type: 'check', cls: "", text: "Gudang", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "WareHouseCode", cls: "span2", type: "popup", btnName: "btnWareHouseCode", click: "WareHouseCode()", disable: "data.isWareHouseCode == false" },
                                { name: "WareHouseName", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "WareHouseCodeTo", cls: "span2", type: "popup", btnName: "btnSWareHouseCodeTo", click: "WareHouseCodeTo()", disable: "data.isWareHouseCode == false" },
                                { name: "WareHouseNameTo", cls: "span4", readonly: true },
                            ]
                        },
                    { name: 'isSalesModelCode', type: 'check', cls: "", text: "Sales Model Code", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCode", cls: "span2", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCode()", disable: "data.isSalesModelCode == false" },
                                { name: "SalesModelDesc", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCodeTo", cls: "span2", type: "popup", btnName: "btnSalesModelCodeTo", click: "SalesModelCodeTo()", disable: "data.isSalesModelCode == false" },
                                { name: "SalesModelDescTo", cls: "span4", readonly: true },
                            ]
                        },
                    {
                        text: "Sales Model Year",
                            type: "controls",
                            items: [
                                { name: 'isSalesModelYear', type: 'check', cls: "span1", text: "Sales Model Year", float: 'left' },
                                { name: "SalesModelYear", cls: "span2", type: "select2", datasource: "Year", text: "Year", disable: "data.isSalesModelYear == false" },
                                { name: "SalesModelYearTo", cls: "span2", type: "select2", datasource: "Year", text: "Year", disable: "data.isSalesModelYear == false" },
                            ]
                        },
                    {
                        text: "Kode.Rangka",
                        type: "controls",
                        items: [
                                { name: 'isCodeChassis', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "ChassisCode", cls: "span2", type: "popup", btnName: "btnChassisCode", click: "ChassisCode()", disable: "data.isCodeChassis == false" },
                                { name: "ChassisCodeTo", cls: "span2", type: "popup", btnName: "btnChassisCodeTo", click: "ChassisCodeTo()", disable: "data.isCodeChassis == false" },
                        ]
                    },
                    {
                        text: "No.Rangka",
                        type: "controls",
                        items: [
                                { name: 'isNoChassis', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "ChassisNo", cls: "span2", type: "popup", btnName: "btnChassisNo", click: "ChassisNo()", disable: "data.isNoChassis == false" },
                                { name: "ChassisNoTo", cls: "span2", type: "popup", btnName: "btnChassisNoTo", click: "ChassisNoTo()", disable: "data.isNoChassis == false" },
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
                    { name: "tA", text: "Data Kendaraan", cls: "active" },
                    { name: "tB", text: "Detail Kendaraan" },
                ]
            },
            {
                name: "DataKendaraan",
                title: "Data Kendaraan",
                cls: "tabpage1 tA",
                xtype: "wxtable"
            },
            {
                title: "",
                cls: "tabpage1 tB",
                items: [ 
                    { name: "warehouseName", model: "detail.warehouseName", text: "Gudang", cls: "span4 full", readonly: true },

                    { name: "ColourName", model: "detail.ColourName", text: "Warna", cls: "span4", readonly: true },
                    { name: "cogsunit", model: "detail.cogsunit", text: "COGS Unit", cls: "span2 number-int", readonly: true },
                    { name: "dpp", model: "detail.dpp", text: "Harga Jual (DPP)", cls: "span2 number-int", readonly: true },

                    { name: "servicebookno", model: "detail.servicebookno", text: "No. Buku Service", cls: "span4", readonly: true },
                    { name: "cogsOthers", model: "detail.cogsOthers", text: "COGS Karoseri", cls: "span2 number-int", readonly: true },
                    { name: "ppn", model: "detail.ppn", text: "Harga Jual (PPN)", cls: "span2 number-int", readonly: true },

                    { name: "keyno", model: "detail.keyno", text: "No. Kunci", cls: "span4", readonly: true },
                    { name: "cogsKaroseri", model: "detail.cogsKaroseri", text: "COGS Lain-Lian", cls: "span2 number-int", readonly: true },
                    { name: "bbn", model: "detail.bbn", text: "Harga BBN", cls: "span2 number-int", readonly: true },

                    { name: "ChassisCode", model: "detail.ChassisCode", text: "Kode Rangka", cls: "span4", readonly: true },
                    { name: "SalesModelCode", model: "detail.SalesModelCode", text: "Kode Model", cls: "span4", readonly: true },
                    { name: "ChassisNo", model: "detail.ChassisNo", text: "No Rangka", cls: "span4", readonly: true },
                    { name: "EngineNo", model: "detail.EngineNo", text: "No Engine", cls: "span4", readonly: true }
                ]
            },
            {
                title: "",
                cls: "tabpage1 tB",
                items: [
                    { name: "pono", model: "detail.pono", text: "No. PO", cls: "span4", readonly: true },
                    { name: "podate", model: "detail.podate", text: "Tgl. PO", cls: "span4", readonly: true },

                    { name: "bpuno", model: "detail.bpuno", text: "No. BPU", cls: "span4", readonly: true },
                    { name: "bpudate", model: "detail.bpudate", text: "Tgl. BPU", cls: "span4", readonly: true },

                    { name: "SKPKNo", model: "detail.SKPKNo", text: "No. SKPK", cls: "span4", readonly: true },
                    { name: "SKPKDate", model: "detail.SKPKDate", text: "Tgl. SKPK", cls: "span4", readonly: true },

                    { name: "sono", model: "detail.sono", text: "No. SO", cls: "span4", readonly: true },
                    { name: "sodate", model: "detail.sodate", text: "Tgl. SO", cls: "span4", readonly: true },

                    { name: "dono", model: "detail.dono", text: "No. DO", cls: "span4", readonly: true },
                    { name: "dodate", model: "detail.dodate", text: "Tgl. DO", cls: "span4", readonly: true },

                    { name: "bpkno", model: "detail.bpkno", text: "No. BPK", cls: "span4", readonly: true },
                    { name: "bpkdate", model: "detail.bpkdate", text: "Tgl. BPK", cls: "span4", readonly: true },

                    { name: "SPKNo", model: "detail.SPKNo", text: "No. SPK", cls: "span4", readonly: true },
                    { name: "SPKDate", model: "detail.SPKDate", text: "Tgl. SPK", cls: "span4", readonly: true },

                    { name: "invoiceNo", model: "detail.invoiceNo", text: "No. Invoice", cls: "span4", readonly: true },
                    { name: "invoiceDate", model: "detail.invoiceDate", text: "Tgl. Invoice", cls: "span4", readonly: true },

                    { name: "RefferenceSJNo", model: "detail.RefferenceSJNo", text: "No. SJ", cls: "span4", readonly: true },
                    { name: "RefferenceSJDate", model: "detail.RefferenceSJDate", text: "Tgl. SJ", cls: "span4", readonly: true },

                    { name: "hppno", model: "detail.hppno", text: "No. HPP", cls: "span4", readonly: true },
                    { name: "hppdate", model: "detail.hppdate", text: "Tgl. HPP", cls: "span4", readonly: true },

                    { name: "reqOutNo", model: "detail.reqOutNo", text: "No. Prem Faktur", cls: "span4", readonly: true },
                    { name: "reqdate", model: "detail.reqdate", text: "Tgl. Prem Faktur", cls: "span4", readonly: true },

                    { name: "reffinv", model: "detail.reffinv", text: "No. Ref Invoice", cls: "span4", readonly: true },
                    { name: "reffinvdate", model: "detail.reffinvdate", text: "Tgl. Ref Invoice", cls: "span4", readonly: true },

                    { name: "refffp", model: "detail.refffp", text: "No. Ref Fak pajak", cls: "span4", readonly: true },
                    { name: "refffpdate", model: "detail.refffpdate", text: "Tgl. Ref Fak pajak", cls: "span4", readonly: true },

                    { name: "policeregno", model: "detail.policeregno", text: "No. Polis", cls: "span4", readonly: true },
                    { name: "policeregdate", model: "detail.policeregdate", text: "Tgl. Polis", cls: "span4", readonly: true },
                ]
            },
            {
                title: "",
                cls: "tabpage1 tB",
                items: [
                    { name: "Customer", model: "detail.Customer", text: "Pelanggan", cls: "span8", readonly: true },
                    { name: "Address", model: "detail.Address", text: "Alamat", cls: "span8", readonly: true },
                    { name: "Salesman", model: "detail.Salesman", text: "Salesman", cls: "span8", readonly: true },
                    { name: "Leasing", model: "detail.Leasing", text: "Leasing", cls: "span4", readonly: true },
                    { name: "KelAR", model: "detail.KelAR", text: "KelompokAR", cls: "span4", readonly: true }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryDataKendaraan");
    }



});