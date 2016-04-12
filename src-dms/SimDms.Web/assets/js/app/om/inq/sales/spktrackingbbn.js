"use strict";

function omInquirySPK($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Deleted' },
       { "value": '3', "text": 'Canceled' },
       { "value": '9', "text": 'Finished' }
    ];

    me.SPKNo = function () {
        var lookup = Wx.blookup({
            name: "SPKLookup",
            title: "No SPK",
            manager: spSalesManager,
            query: "SPKLookup",
            defaultSort: "SPKNo asc",
            columns: [
                { field: "SPKNo", title: "No SPK" },
                { field: "SPKDate", title: "tgl SPK", template: "#= moment(SPKDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SPKNo = data.SPKNo;
                me.Apply();
            }
        });

    }

    me.SPKNoTo = function () {
        var lookup = Wx.blookup({
            name: "SPKLookup",
            title: "No SPK",
            manager: spSalesManager,
            query: "SPKLookup",
            defaultSort: "SPKNo asc",
            columns: [
                { field: "SPKNo", title: "No SPK" },
                { field: "SPKDate", title: "tgl SPK", template: "#= moment(SPKDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SPKNoTo = data.SPKNo;
                me.Apply();
            }
        });

    }

    me.ChassisNo = function () {
        var lookup = Wx.blookup({
            name: "ChassisLookup",
            title: "No Rangka",
            manager: spSalesManager,
            query: "ChassisLookup",
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
            name: "ChassisLookup",
            title: "No Rangka",
            manager: spSalesManager,
            query: "ChassisLookup",
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
        var SPKDate = '1/1/1900';
        var SPKDateTo = '1/1/1900';

        if (me.data.SPKDate) {
            var SPKDate = new Date(me.data.SPKDate).getMonth() + 1 + '/' + new Date(me.data.SPKDate).getDate() + '/' + new Date(me.data.SPKDate).getFullYear();
            var SPKDateTo = new Date(me.data.SPKDateTo).getMonth() + 1 + '/' + new Date(me.data.SPKDateTo).getDate() + '/' + new Date(me.data.SPKDateTo).getFullYear();
        }

        $http.post('om.api/InquirySales/searchSPK?Status=' + me.data.Status + '&SPKDate=' + SPKDate + '&SPKDateTo=' + SPKDateTo
                                                + '&SPKNo=' + me.data.SPKNo + '&SPKNoTo=' + me.data.SPKNoTo
                                                + '&ChassisNo=' + me.data.ChassisNo + '&ChassisNoTo=' + me.data.ChassisNoTo
                                                + '&SupplierCode=' + me.data.SupplierCode + '&SupplierCodeTo=' + me.data.SupplierCodeTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.gridDetailSPK);
              me.loadTableData(me.gridSPK, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridSPK = new webix.ui({
        container: "SPK",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "SPKNo", header: "No.SPK", width: 150 },
            { id: "SPKDate", header: "Tgl.SPK", width: 150 },
            { id: "RefferenceNo", header: "No.Ref", width: 150 },
            { id: "RefferenceDate", header: "Tgl.Ref", width: 150 },
            { id: "SupplierName", header: "Pemasok", width: 300 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridSPK.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridSPK.getSelectedId().id);
                    var datas = {
                        "SPKNo": data.SPKNo
                    }

                    $http.post('om.api/InquirySales/GetDetailSPK', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.gridDetailSPK, e.detail);
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

    me.gridDetailSPK = new webix.ui({
        container: "DetailSPK",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No Rangka", width: 150 },
            { id: "ReqInNo", header: "Reques Faktur", width: 150 },
            { id: "FakturPolisiNo", header: "Faktur Polis", width: 150 },
            { id: "Remark", header: "Keterangan", width: 350 }
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

    $("[name = 'isNoSPK']").on('change', function () {
        me.data.isNoSPK = $('#isNoSPK').prop('checked');
        me.data.SPKNo = "";
        me.data.SPKNoTo = "";
        me.Apply();
    });

    $("[name = 'isNoChassis']").on('change', function () {
        me.data.isNoChassis = $('#isNoChassis').prop('checked');
        me.data.ChassisNo = "";
        me.data.ChassisNoTo = "";
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

    me.initialize = function () {
        me.data.Status = '';
        me.data.SPKNo = '';
        me.data.SPKNoTo = '';
        me.data.ChassisNo = '';
        me.data.ChassisNoTo = '';
        me.data.SupplierCode = '';
        me.data.SupplierCodeTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoSPK').prop('checked', false);
        me.data.isNoSPK = false;
        $('#isNoChassis').prop('checked', false);
        me.data.isNoChassis = false;
        $('#isActiveSupplier').prop('checked', false);
        me.data.isActiveSupplier = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridSPK.adjust();
    }
    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Inquiry SPK & Tracking BBN",
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
                        text: "Tgl. SPK",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "SPKDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "SPKDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No.SPK",
                        type: "controls",
                        items: [
                                { name: 'isNoSPK', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "SPKNo", cls: "span2", type: "popup", btnName: "btnSPKNo", click: "SPKNo()", disable: "data.isNoSPK == false" },
                                { name: "SPKNoTo", cls: "span2", type: "popup", btnName: "btnSPKNoTo", click: "SPKNoTo()", disable: "data.isNoSPK == false" },
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
                    { name: "tA", text: "SPK", cls: "active" },
                    { name: "tB", text: "Detail SPK" },
                ]
            },
                    {
                        name: "SPK",
                        title: "SPK",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailSPK",
                        title: "Detail SPK",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquirySPK");
    }



});