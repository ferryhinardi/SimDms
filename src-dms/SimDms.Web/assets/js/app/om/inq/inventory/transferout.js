"use strict";

function omInquiryTransferOut($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Deleted' },
       { "value": '3', "text": 'Canceled' },
       { "value": '9', "text": 'Finished' }
    ];

    me.TrasferOutNo = function () {
        var lookup = Wx.blookup({
            name: "TransferOutLookup",
            title: "No Transfer Out",
            manager: spSalesManager,
            query: "TransferOutLookup",
            defaultSort: "TransferOutNo asc",
            columns: [
                { field: "TransferOutNo", title: "No Transfer" },
                {
                    field: "TransferOutDate", title: "tgl Transfer",
                    template: "#= moment(TransferOutDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.TrasferOutNo = data.TransferOutNo;
                me.Apply();
            }
        });

    }

    me.TrasferOutNoTo = function () {
        var lookup = Wx.blookup({
            name: "TransferOutLookup",
            title: "No Transfer Out",
            manager: spSalesManager,
            query: "TransferOutLookup",
            defaultSort: "TransferOutNo asc",
            columns: [
                { field: "TransferOutNo", title: "No Transfer" },
                {
                    field: "TransferOutDate", title: "tgl Transfer",
                    template: "#= moment(TransferOutDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.TrasferOutNoTo = data.TransferOutNo;
                me.Apply();
            }
        });

    }

    me.WareHouseCodeFrom = function () {
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
                me.data.WareHouseCodeFrom = data.RefferenceCode;
                me.data.WareHouseNameFrom = data.RefferenceDesc1;
                me.Apply();
            }
        });

    }

    me.WareHouseCodeFromTo = function () {
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
                me.data.WareHouseCodeFromTo = data.RefferenceCode;
                me.data.WareHouseNameFromTo = data.RefferenceDesc1;
                me.Apply();
            }
        });

    }

    me.BranchCodeTo = function () {
        var lookup = Wx.blookup({
            name: "BranchLookup",
            title: "BranchCode",
            manager: spSalesManager,
            query: "BranchLookup",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Branch Code" },
                { field: "BranchName", title: "Branch Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCodeTo = data.BranchCode;
                me.data.BranchNameTo = data.BranchName;
                me.Apply();
            }
        });

    }

    me.BranchCodeToLast = function () {
        var lookup = Wx.blookup({
            name: "BranchLookup",
            title: "BranchCode",
            manager: spSalesManager,
            query: "BranchLookup",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Branch Code" },
                { field: "BranchName", title: "Branch Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCodeToLast = data.BranchCode;
                me.data.BranchNameToLast = data.BranchName;
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

    me.WareHouseCodeToLast = function () {
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
                me.data.WareHouseCodeToLast = data.RefferenceCode;
                me.data.WareHouseNameToLast = data.RefferenceDesc1;
                me.Apply();
            }
        });

    }

    me.CariData = function () {
        var TransferOutDate = '1/1/1900';
        var TransferOutDateTo = '1/1/1900';

        if (me.data.TransferOutDate) {
            var TransferOutDate = new Date(me.data.TransferOutDate).getMonth() + 1 + '/' + new Date(me.data.TransferOutDate).getDate() + '/' + new Date(me.data.TransferOutDate).getFullYear();
            var TransferOutDateTo = new Date(me.data.TransferOutDateTo).getMonth() + 1 + '/' + new Date(me.data.TransferOutDateTo).getDate() + '/' + new Date(me.data.TransferOutDateTo).getFullYear();
        }

        $http.post('om.api/InquiryInventory/searchTRansferOut?Status=' + me.data.Status + '&TransferOutDate=' + TransferOutDate + '&TransferOutDateTo=' + TransferOutDateTo
                                                + '&TrasferOutNo=' + me.data.TrasferOutNo + '&TrasferOutNoTo=' + me.data.TrasferOutNoTo
                                                + '&WareHouseCodeFrom=' + me.data.WareHouseCodeFrom + '&WareHouseCodeFromTo=' + me.data.WareHouseCodeFromTo
                                                + '&BranchCodeTo=' + me.data.BranchCodeTo + '&BranchCodeToLast=' + me.data.BranchCodeToLast
                                                + '&WareHouseCodeTo=' + me.data.WareHouseCodeTo + '&WareHouseCodeToLast=' + me.data.WareHouseCodeToLast).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.gridDetailTransferOut);
              me.loadTableData(me.gridTransferOut, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridTransferOut = new webix.ui({
        container: "TransferOut",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "TransferOutNo", header: "No.Transfer", width: 150 },
            { id: "TransferOutDate", header: "Tgl.Transfer", width: 150 },
            { id: "RefferenceNo", header: "No.Ref", width: 150 },
            { id: "RefferenceDate", header: "Tgl.Ref", width: 150 },
            { id: "BranchNameFrom", header: "Cabang Asal", width: 150 },
            { id: "WareHouseNameFrom", header: "Gudang Asal", width: 150 },
            { id: "BranchNameTo", header: "Cabang Tujuan", width: 150 },
            { id: "WareHouseNameTo", header: "Gudang Tujuan", width: 150 },
            { id: "ReturnDate", header: "Tgl. Return", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridTransferOut.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridTransferOut.getSelectedId().id);
                    var datas = {
                        "TransferOutNo": data.TransferOutNo
                    }

                    $http.post('om.api/InquiryInventory/GetTRansferOutDetail', datas)
                    .success(function (data, status, headers, config) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.gridDetailTransferOut, data);
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.gridDetailTransferOut = new webix.ui({
        container: "DetailTransferOut",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 150 },
            { id: "SalesModelDesc", header: "Sales Model Desc", width: 150 },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No Rangka", width: 150 },
            { id: "EngineCode", header: "Kode Mesin", width: 150 },
            { id: "EngineNo", header: "No Mesin", width: 150 },
            { id: "ColourCode", header: "Kode Warna", width: 150 },
            { id: "ColourName", header: "Nama Warna", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 }
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

    $("[name = 'isTrasferOutNo']").on('change', function () {
        me.data.isTrasferOutNo = $('#isTrasferOutNo').prop('checked');
        me.data.TrasferOutNo = "";
        me.data.TrasferOutNoTo = "";
        me.Apply();
    });

    $("[name = 'isWareHouseCodeFrom']").on('change', function () {
        me.data.isWareHouseCodeFrom = $('#isWareHouseCodeFrom').prop('checked');
        me.data.WareHouseCodeFrom = "";
        me.data.WareHouseCodeFromTo = "";
        me.data.WareHouseNameFrom = "";
        me.data.WareHouseNameFromTo = "";
        me.Apply();
    });

    $("[name = 'isBranchCodeTo']").on('change', function () {
        me.data.isBranchCodeTo = $('#isBranchCodeTo').prop('checked');
        me.data.BranchCodeTo = "";
        me.data.BranchCodeToLast = "";
        me.data.BranchNameTo = "";
        me.data.BranchNameToLast = "";
        me.Apply();
    });

    $("[name = 'isWareHouseCodeTo']").on('change', function () {
        me.data.isWareHouseCodeTo = $('#isWareHouseCodeTo').prop('checked');
        me.data.WareHouseCodeTo = "";
        me.data.WareHouseCodeToLast = "";
        me.data.WareHouseNameTo = "";
        me.data.WareHouseNameToLast = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.TrasferOutNo = '';
        me.data.TrasferOutNoTo = '';
        me.data.WareHouseCodeFrom = '';
        me.data.WareHouseCodeFromTo = '';
        me.data.BranchCodeTo = '';
        me.data.BranchCodeToLast = '';
        me.data.WareHouseCodeTo = '';
        me.data.WareHouseCodeToLast = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isTrasferOutNo').prop('checked', false);
        me.data.isTrasferOutNo = false;
        $('#isWareHouseCodeFrom').prop('checked', false);
        me.data.isWareHouseCodeFrom = false;
        $('#isBranchCodeTo').prop('checked', false);
        me.data.isBranchCodeTo = false;
        $('#isWareHouseCodeTo').prop('checked', false);
        me.data.isWareHouseCodeTo = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridTransferOut.adjust();
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Transfer Out",
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
                        text: "Tgl. TransferOut",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "TransferOutDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "TransferOutDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No.Transfer Out",
                        type: "controls",
                        items: [
                                { name: 'isTrasferOutNo', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "TrasferOutNo", cls: "span2", type: "popup", btnName: "btnTrasferOutNo", click: "TrasferOutNo()", disable: "data.isTrasferOutNo == false" },
                                { name: "TrasferOutNoTo", cls: "span2", type: "popup", btnName: "btnTrasferOutNoTo", click: "TrasferOutNoTo()", disable: "data.isTrasferOutNo == false" },
                        ]
                    },
                    { name: 'isWareHouseCodeFrom', type: 'check', cls: "", text: "Gudang Asal", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "WareHouseCodeFrom", cls: "span2", type: "popup", btnName: "btnSupplierCode", click: "WareHouseCodeFrom()", disable: "data.isWareHouseCodeFrom == false" },
                                { name: "WareHouseNameFrom", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "WareHouseCodeFromTo", cls: "span2", type: "popup", btnName: "btnSupplierCodeTo", click: "WareHouseCodeFromTo()", disable: "data.isWareHouseCodeFrom == false" },
                                { name: "WareHouseNameFromTo", cls: "span4", readonly: true },
                            ]
                        },
                    { name: 'isBranchCodeTo', type: 'check', cls: "", text: "Cabang Tujuan", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "BranchCodeTo", cls: "span2", type: "popup", btnName: "btnBranchCodeTo", click: "BranchCodeTo()", disable: "data.isBranchCodeTo == false" },
                                { name: "BranchNameTo", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "BranchCodeToLast", cls: "span2", type: "popup", btnName: "btnSupplierCodeTo", click: "BranchCodeToLast()", disable: "data.isBranchCodeTo == false" },
                                { name: "BranchNameToLast", cls: "span4", readonly: true },
                            ]
                        },
                    { name: 'isWareHouseCodeTo', type: 'check', cls: "", text: "Gudang Tujuan", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "WareHouseCodeTo", cls: "span2", type: "popup", btnName: "btnWareHouseCodeTo", click: "WareHouseCodeTo()", disable: "data.isWareHouseCodeTo == false" },
                                { name: "WareHouseNameTo", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "WareHouseCodeToLast", cls: "span2", type: "popup", btnName: "btnSWareHouseCodeToLast", click: "WareHouseCodeToLast()", disable: "data.isWareHouseCodeTo == false" },
                                { name: "WareHouseNameToLast", cls: "span4", readonly: true },
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
                    { name: "tA", text: "Transfer Out", cls: "active" },
                    { name: "tB", text: "Detail Transfer Out" },
                ]
            },
                    {
                        name: "TransferOut",
                        title: "Transfer Out",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailTransferOut",
                        title: "Detail Transfer Out",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryTransferOut");
    }



});