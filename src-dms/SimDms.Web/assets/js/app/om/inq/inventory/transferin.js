"use strict";

function omInquiryTransferIn($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Deleted' },
       { "value": '3', "text": 'Canceled' }
    ];

    me.TransferInNo = function () {
        var lookup = Wx.blookup({
            name: "TransfeInLookup",
            title: "No Transfer Int",
            manager: spSalesManager,
            query: "TransfeInLookup",
            defaultSort: "TransferInNo asc",
            columns: [
                { field: "TransferInNo", title: "No Transfer" },
                {
                    field: "TransferInDate", title: "tgl Transfer",
                    template: "#= moment(TransferInDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.TransferInNo = data.TransferInNo;
                me.Apply();
            }
        });

    }

    me.TransferInNoTo = function () {
        var lookup = Wx.blookup({
            name: "TransfeInLookup",
            title: "No Transfer Int",
            manager: spSalesManager,
            query: "TransfeInLookup",
            defaultSort: "TransferInNo asc",
            columns: [
                { field: "TransferInNo", title: "No Transfer" },
                {
                    field: "TransferInDate", title: "tgl Transfer",
                    template: "#= moment(TransferInDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.TransferInNoTo = data.TransferInNo;
                me.Apply();
            }
        });

    }

    me.TransferOutNo = function () {
        var lookup = Wx.blookup({
            name: "TransfeInLookup",
            title: "No Transfer Int",
            manager: spSalesManager,
            query: "TransfeInLookup",
            defaultSort: "TransferOutNo asc",
            columns: [
                { field: "TransferOutNo", title: "No Transfer" },
                {
                    field: "TransferOutNo", title: "tgl Transfer",
                    template: "#= moment(TransferOutNo).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.TransferOutNo = data.TransferOutNo;
                me.Apply();
            }
        });

    }

    me.TransferOutNoTo = function () {
        var lookup = Wx.blookup({
            name: "TransfeInLookup",
            title: "No Transfer Int",
            manager: spSalesManager,
            query: "TransfeInLookup",
            defaultSort: "TransferOutNo asc",
            columns: [
                { field: "TransferOutNo", title: "No Transfer" },
                {
                    field: "TransferOutNo", title: "tgl Transfer",
                    template: "#= moment(TransferOutNo).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.TransferOutNoTo = data.TransferOutNo;
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
        var TransferInDate = '1/1/1900';
        var TransferInDateTo = '1/1/1900';

        if (me.data.TransferInDate) {
            var TransferInDate = new Date(me.data.TransferInDate).getMonth() + 1 + '/' + new Date(me.data.TransferInDate).getDate() + '/' + new Date(me.data.TransferInDate).getFullYear();
            var TransferInDateTo = new Date(me.data.TransferInDateTo).getMonth() + 1 + '/' + new Date(me.data.TransferInDateTo).getDate() + '/' + new Date(me.data.TransferInDateTo).getFullYear();
        }

        $http.post('om.api/InquiryInventory/searchTRansferIn?Status=' + me.data.Status + '&TransferInDate=' + TransferInDate + '&TransferInDateTo=' + TransferInDateTo
                                                + '&TransferInNo=' + me.data.TransferInNo + '&TransferInNoTo=' + me.data.TransferInNoTo
                                                + '&TransferOutNo=' + me.data.TransferOutNo + '&TransferOutNoTo=' + me.data.TransferOutNoTo
                                                + '&WareHouseCodeTo=' + me.data.WareHouseCodeTo + '&WareHouseCodeToLast=' + me.data.WareHouseCodeToLast).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.gridDetailTransferIn);
              me.loadTableData(me.gridTransferIn, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridTransferIn = new webix.ui({
        container: "TransferIn",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "TransferInNo", header: "No.Transfer In", width: 150 },
            { id: "TransferInDate", header: "Tgl.Transfer", width: 150 },
            { id: "TransferOutNo", header: "No.Transfer Out", width: 150 },
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
                if (me.gridTransferIn.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridTransferIn.getSelectedId().id);
                    var datas = {
                        "TransferInNo": data.TransferInNo
                    }

                    $http.post('om.api/InquiryInventory/GetTRansferInDetail', datas)
                    .success(function (data, status, headers, config) {
                        $(".panel.tabpage1").hide();
                        $(".panel.tabpage1.tB").show();
                        me.loadTableData(me.gridDetailTransferIn, data);
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.gridDetailTransferIn = new webix.ui({
        container: "DetailTransferIn",
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

    $("[name = 'isTransferInNo']").on('change', function () {
        me.data.isTransferInNo = $('#isTransferInNo').prop('checked');
        me.data.TransferInNo = "";
        me.data.TransferInNoTo = "";
        me.Apply();
    });

    $("[name = 'isTransferOutNo']").on('change', function () {
        me.data.isTransferOutNo = $('#isTransferOutNo').prop('checked');
        me.data.TrasferOutNo = "";
        me.data.TrasferOutNoTo = "";
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
        me.data.TransferInNo = '';
        me.data.TransferInNoTo = '';
        me.data.TransferOutNo = '';
        me.data.TransferOutNoTo = '';
        me.data.WareHouseCodeTo = '';
        me.data.WareHouseCodeToLast = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isTransferInNo').prop('checked', false);
        me.data.isTransferInNo = false;
        $('#isTransferOutNo').prop('checked', false);
        me.data.isTransferOutNo = false;
        $('#isWareHouseCodeTo').prop('checked', false);
        me.data.isWareHouseCodeTo = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridTransferIn.adjust();
    }
    me.start();

}
$(document).ready(function () {
    var options = {
        title: "Inquiry Transfer In",
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
                        text: "Tgl. Transfer In",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "TransferInDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "TransferInDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No.Transfer in",
                        type: "controls",
                        items: [
                                { name: 'isTransferInNo', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "TransferInNo", cls: "span2", type: "popup", btnName: "btnTransferInNo", click: "TransferInNo()", disable: "data.isTransferInNo == false" },
                                { name: "TransferInNoTo", cls: "span2", type: "popup", btnName: "btnTransferInNoTo", click: "TransferInNoTo()", disable: "data.isTransferInNo == false" },
                        ]
                    },
                    {
                        text: "No.Transfer Out",
                        type: "controls",
                        items: [
                                { name: 'isTransferOutNo', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "TransferOutNo", cls: "span2", type: "popup", btnName: "btnTransferOutNo", click: "TransferOutNo()", disable: "data.isTransferOutNo == false" },
                                { name: "TransferOutNoTo", cls: "span2", type: "popup", btnName: "btnTransferOutNoTo", click: "TransferOutNoTo()", disable: "data.isTransferOutNo == false" },
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
                    { name: "tA", text: "Transfer In", cls: "active" },
                    { name: "tB", text: "Detail Transfer In" },
                ]
            },
                    {
                        name: "TransferIn",
                        title: "Transfer In",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailTransferIn",
                        title: "Detail Transfer In",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryTransferIn");
    }



});