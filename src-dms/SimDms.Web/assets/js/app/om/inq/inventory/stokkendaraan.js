"use strict";

function omInquiryStokKendaraan($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    $http.post('om.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.Months = data;
    });

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
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" }
            ]
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
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCodeTo = data.SalesModelCode;
                me.data.SalesModelDescTo = data.SalesModelDesc;
                me.Apply();
            }
        });
    }

    me.ColourCode = function () {
        var lookup = Wx.blookup({
            name: "ColourCodeLookup",
            title: "Colour",
            manager: spSalesManager,
            query: "ColourCodeLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Colour Code" },
                { field: "RefferenceDesc1", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ColourCode = data.RefferenceCode;
                me.data.ColourDesc = data.RefferenceDesc1;
                me.Apply();
            }
        });
    }

    me.ColourCodeTo = function () {
        var lookup = Wx.blookup({
            name: "ColourCodeLookup",
            title: "Colour",
            manager: spSalesManager,
            query: "ColourCodeLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Colour Code" },
                { field: "RefferenceDesc1", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ColourCodeTo = data.RefferenceCode;
                me.data.ColourDescTo = data.RefferenceDesc1;
                me.Apply();
            }
        });
    }

    me.gridStokKendaraan = new webix.ui({
        container: "wxStokKendaraan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        height: 350,
        autoHeight: false,
        columns: [
            { id: "Year", header: "Tahun", width: 100 },
            { id: "Month", header: "Bulan", width: 100 },
            { id: "WareHouseName", header: "Gudang", width: 150 },
            { id: "SalesModelCode", header: "Kode Model", width: 150 },
            { id: "SalesModelDesc", header: "Deskripsi", width: 150 },
            { id: "ModelYear", header: "Tahun", width: 100 },
            { id: "ColourName", header: "Warna", width: 300 },
            { id: "BeginningAV", header: "Saldo Awal", width: 100 },
            { id: "QtyIn", header: "Masuk", width: 100 },
            { id: "Alocation", header: "Alokasi", width: 100 },
            { id: "QtyOut", header: "Keluar", width: 100 },
            { id: "EndingAV", header: "Saldo Akhir", width: 100 }
        ]
    });

    me.CariData = function () {
        $http.post('om.api/InquiryInventory/searchStokKendaraan?Year=' + me.data.Year + '&Month=' + me.data.Months
                                                + '&WarehouseCode=' + me.data.WareHouseCode + '&WarehouseCodeTo=' + me.data.WareHouseCodeTo
                                                + '&SalesModelCode=' + me.data.SalesModelCode + '&SalesModelCodeTo=' + me.data.SalesModelCodeTo
                                                + '&SalesModelYear=' + me.data.SalesModelYear + '&SalesModelYearTo=' + me.data.SalesModelYearTo
                                                + '&ColourCode=' + me.data.ColourCode + '&ColourCodeTo=' + me.data.ColourCodeTo).
          success(function (data, status, headers, config) {
              me.loadTableData(me.gridStokKendaraan, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    $("[name = 'isActiveYear']").on('change', function () {
        me.data.isActiveYear = $('#isActiveYear').prop('checked');
        me.data.Year = "";
        me.Apply();
    });

    $("[name = 'isActiveMonth']").on('change', function () {
        me.data.isActiveMonth = $('#isActiveMonth').prop('checked');
        me.data.Months = "";
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

    $("[name = 'isColourCode']").on('change', function () {
        me.data.isColourCode = $('#isColourCode').prop('checked');
        me.data.ColourCode = "";
        me.data.ColourCodeTo = "";
        me.data.ColourDesc = "";
        me.data.ColourDescTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Year = '';
        me.data.Months = '';
        me.data.WareHouseCode = '';
        me.data.WareHouseCodeTo = '';
        me.data.SalesModelCode = '';
        me.data.SalesModelCodeTo = '';
        me.data.SalesModelYear = '';
        me.data.SalesModelYearTo = '';
        me.data.ColourCode = '';
        me.data.ColourCodeTo = '';
        $('#isActiveYear').prop('checked', false);
        me.data.isActiveYear = false;
        $('#isActiveMonth').prop('checked', false);
        me.data.isActiveMonth = false;
        $('#isWareHouseCode').prop('checked', false);
        me.data.isWareHouseCode = false;
        $('#isSalesModelCode').prop('checked', false);
        me.data.isSalesModelCode = false;
        $('#isSalesModelYear').prop('checked', false);
        me.data.isSalesModelYear = false;
        $('#isColourCode').prop('checked', false);
        me.data.isColourCode = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridStokKendaraan.adjust();
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Stok Kendaraan",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        text: "Tahun",
                        type: "controls",
                        items: [
                            { name: 'isActiveYear', type: 'check', cls: "span1", float: 'left' },
                            { name: "Year", cls: "span2", type: "select2", datasource: "Year", text: "Year", disable: "data.isActiveYear == false" },

                        ]
                    },
                    {
                        text: "Bulan",
                        type: "controls",
                        items: [
                            { name: 'isActiveMonth', type: 'check', cls: "span1", float: 'left' },
                            { name: "Months", cls: "span2", type: "select2", datasource: "Months", text: "Year", disable: "data.isActiveMonth == false" },

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
                    { name: 'isSalesModelYear', type: 'check', cls: "", text: "Sales Model Year", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelYear", cls: "span2", type: "select2", datasource: "Year", text: "Year", disable: "data.isSalesModelYear == false" },
                                { name: "SalesModelYearTo", cls: "span2", type: "select2", datasource: "Year", text: "Year", disable: "data.isSalesModelYear == false" },
                            ]
                        },
                    { name: 'isColourCode', type: 'check', cls: "", text: "Warna", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "ColourCode", cls: "span2", type: "popup", btnName: "btnColourCode", click: "ColourCode()", disable: "data.isColourCode == false" },
                                { name: "ColourDesc", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "ColourCodeTo", cls: "span2", type: "popup", btnName: "btnColourCodeTo", click: "ColourCodeTo()", disable: "data.isColourCode == false" },
                                { name: "ColourDescTo", cls: "span4", readonly: true },
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
                        name: "wxStokKendaraan",
                        xtype: "wxtable",
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryStokKendaraan");
    }



});