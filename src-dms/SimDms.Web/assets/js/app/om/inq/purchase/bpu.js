var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function omInquiryBPU($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Approved' },
       { "value": '3', "text": 'Canceled' },
       { "value": '9', "text": 'Finished' }
    ];

    me.BPUType = [
       { "value": '0', "text": 'DO' },
       { "value": '1', "text": 'SJ' },
       { "value": '2', "text": 'DO & SJ' },
       { "value": '3', "text": 'SJ Booking' },
    ];

    me.NoPO = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "PO",
            manager: spSalesManager,
            query: "POBrowse",
            defaultSort: "PONo asc",
            columns: [
                { field: "PONo", title: "PO No" },
                {
                    field: "PODate", title: "PO Date",
                    template: "#= moment(PODate).format('DD MMM YYYY') #"
                },
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
                {
                    field: "PODate", title: "PO Date",
                    template: "#= moment(PODate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoPOTo = data.PONo;
                me.Apply();
            }
        });

    }

    me.NoRefDO = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "NO.DO",
            manager: spSalesManager,
            query: "BPULookup",
            defaultSort: "RefferenceDONo asc",
            columns: [
                { field: "RefferenceDONo", title: "No Ref DO" },
                { field: "RefferenceDODate", title: "Tgl Ref DO", template: "#= moment(RefferenceDODate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoRefDO = data.RefferenceDONo;
                me.Apply();
            }
        });

    }

    me.NoRefDOTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "NO.DO",
            manager: spSalesManager,
            query: "BPULookup",
            defaultSort: "RefferenceDONo asc",
            columns: [
                { field: "RefferenceDONo", title: "No Ref DO" },
                { field: "RefferenceDODate", title: "Tgl Ref DO", template: "#= moment(RefferenceDODate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoRefDOTo = data.RefferenceDONo;
                me.Apply();
            }
        });

    }

    me.NoRefSJ = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "NO.SJ",
            manager: spSalesManager,
            query: "BPULookup",
            defaultSort: "RefferenceSJNo Desc",
            columns: [
                { field: "RefferenceSJNo", title: "No Ref SJ" },
                { field: "RefferenceSJDate", title: "Tgl Ref SJ", template: "#= moment(RefferenceSJDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoRefSJ = data.RefferenceSJNo;
                me.Apply();
            }
        });

    }

    me.NoRefSJTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "NO.SJ",
            manager: spSalesManager,
            query: "BPULookup",
            defaultSort: "RefferenceSJNo Desc",
            columns: [
                { field: "RefferenceSJNo", title: "No Ref SJ" },
                { field: "RefferenceSJDate", title: "Tgl Ref SJ", template: "#= moment(RefferenceSJDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoRefSJTo = data.RefferenceSJNo;
                me.Apply();
            }
        });

    }

    me.NoBPU = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "NO.SJ",
            manager: spSalesManager,
            query: "BPULookup",
            defaultSort: "RefferenceSJNo Asc",
            columns: [
                { field: "BPUNo", title: "No BPU" },
                { field: "BPUDate", title: "Tgl BPU", template: "#= moment(BPUDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoBPU = data.BPUNo;
                me.Apply();
            }
        });

    }

    me.NoBPUTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "NO.SJ",
            manager: spSalesManager,
            query: "BPULookup",
            defaultSort: "RefferenceSJNo Asc",
            columns: [
                { field: "BPUNo", title: "No BPU" },
                { field: "BPUDate", title: "Tgl BPU", template: "#= moment(BPUDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoBPUTo = data.BPUNo;
                me.Apply();
            }
        });

    }

    me.CariData = function () {
        var BPUDate = '1/1/1900';
        var BPUDateTo = '1/1/1900';

        if (me.data.BPUDate) {
            var BPUDate = new Date(me.data.BPUDate).getMonth() + 1 + '/' + new Date(me.data.BPUDate).getDate() + '/' + new Date(me.data.BPUDate).getFullYear();
            var BPUDateTo = new Date(me.data.BPUDateTo).getMonth() + 1 + '/' + new Date(me.data.BPUDateTo).getDate() + '/' + new Date(me.data.BPUDateTo).getFullYear();
        }

        $http.post('om.api/InquiryPurchase/searchBPU?Status=' + me.data.Status + '&BPUType=' + me.data.BPUType
                                                + '&BPUDate=' + BPUDate + '&BPUDateTo=' + BPUDateTo
                                                + '&NoPO=' + me.data.NoPO + '&NoPOTo=' + me.data.NoPOTo
                                                + '&NoRefDO=' + me.data.NoRefDO + '&NoRefDOTo=' + me.data.NoRefDOTo
                                                + '&NoRefSJ=' + me.data.NoRefSJ + '&NoRefSJTo=' + me.data.NoRefSJTo
                                                + '&NoBPU=' + me.data.NoBPU + '&NoBPU=' + me.data.NoBPUTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.griddetailBPU);
              me.loadTableData(me.gridBPU, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridBPU = new webix.ui({
        container: "BPU",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "BPUType", header: "Tipe", width: 50 },
            { id: "BPUNo", header: "BPU", width: 150 },
            { id: "BPUDate", header: "Tgl.BPU", width: 150 },
            { id: "RefferenceDONo", header: "No.Ref.DO", width: 150 },
            { id: "RefferenceDODate", header: "Tgl.Ref.DO", width: 150 },
            { id: "RefferenceSJNo", header: "No.Ref.SJ", width: 150 },
            { id: "RefferenceSJDate", header: "Tgl.Ref.SJ", width: 150 },
            { id: "SupplierName", header: "Pemasok", width: 200 },
            { id: "ShipTo", header: "Kirim Ke", width: 200 },
            { id: "ExpeditionName", header: "Expedisi", width: 200 },
            { id: "WareHouseName", header: "Gudang", width: 200 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridBPU.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridBPU.getSelectedId().id);

                    var datas = {
                        "BPUNo": data.BPUNo
                    }

                    $http.post('om.api/InquiryPurchase/GetDetailBPU', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.griddetailBPU, e.detail);
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

    me.griddetailBPU = new webix.ui({
        container: "DetailBPU",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 100 },
            { id: "SalesModelDesc", header: "Sales Model Desc", width: 150 },
            { id: "ColourCode", header: "Kode Warna", width: 150 },
            { id: "ColourName", header: "Nama Warna", width: 150 },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No. Rangka", width: 150 },
            { id: "EngineCode", header: "Kode Mesin", width: 200 },
            { id: "EngineNo", header: "No. Mesin", width: 200 },
            { id: "ServiceBookNo", header: "No. Buku Service", width: 200 },
            { id: "KeyNo", header: "No. Kunci", width: 200 },
            { id: "Remark", header: "Keterangan", width: 200 },
        ]
    });

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.Status = "";
        me.Apply();
    });

    $("[name = 'isActiveBPUType']").on('change', function () {
        me.data.isActiveBPUType = $('#isActiveBPUType').prop('checked');
        me.data.BPUType = "";
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

    $("[name = 'isNoRefDO']").on('change', function () {
        me.data.isNoRefDO = $('#isNoRefDO').prop('checked');
        me.data.NoRefDO = "";
        me.data.NoRefDOTo = "";
        me.Apply();
    });

    $("[name = 'isNoRefSJ']").on('change', function () {
        me.data.isNoRefSJ = $('#isNoRefSJ').prop('checked');
        me.data.NoRefSJ = "";
        me.data.NoRefSJTo = "";
        me.Apply();
    });

    $("[name = 'isNoBPU']").on('change', function () {
        me.data.isNoBPU = $('#isNoBPU').prop('checked');
        me.data.NoBPU = "";
        me.data.NoBPUTo = "";
        me.Apply();
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridBPU.adjust(); break;
            case "tB": me.griddetailBPU.adjust(); break;
            default:
        }
    };

    me.initialize = function () {
        me.data.Status = '';
        me.data.BPUType = '';
        me.data.NoPO = '';
        me.data.NoPOTo = '';
        me.data.NoRefDO = '';
        me.data.NoRefDOTo = '';
        me.data.NoRefSJ = '';
        me.data.NoRefSJTo = '';
        me.data.NoBPU = '';
        me.data.NoBPUTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveBPUType').prop('checked', false);
        me.data.isActiveBPUType = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoPO').prop('checked', false);
        me.data.isNoPO = false;
        $('#isNoRefDO').prop('checked', false);
        me.data.isNoRefDO = false;
        $('#isNoRefSJ').prop('checked', false);
        me.data.isNoRefSJ = false;
        $('#isNoBPU').prop('checked', false);
        me.data.isNoBPU = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridBPU.adjust();
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Inquiry BPU",
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
                        text: "Tipe. BPU",
                        type: "controls",
                        items: [
                            { name: 'isActiveBPUType', type: 'check', cls: "span1", text: "Status", float: 'left' },
                            { name: "BPUType", opt_text: "", cls: "span3", type: "select2", text: "", datasource: "BPUType", disable: "data.isActiveBPUType == false" },

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
                        text: "No.PO",
                        type: "controls",
                        items: [
                                { name: 'isNoPO', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoPO", cls: "span2", type: "popup", btnName: "btnNoPO", click: "NoPO()", disable: "data.isNoPO == false" },
                                { name: "NoPOTo", cls: "span2", type: "popup", btnName: "btnNoPOTo", click: "NoPOTo()", disable: "data.isNoPO == false" },
                        ]
                    },
                    {
                        text: "No.Ref.DO",
                        type: "controls",
                        items: [
                                { name: 'isNoRefDO', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoRefDO", cls: "span2", type: "popup", btnName: "btnNoRefDO", click: "NoRefDO()", disable: "data.isNoRefDO == false" },
                                { name: "NoRefDOTo", cls: "span2", type: "popup", btnName: "btnNoRefDOTo", click: "NoRefDOTo()", disable: "data.isNoRefDO == false" },
                        ]
                    },
                    {
                        text: "No.Ref.SJ",
                        type: "controls",
                        items: [
                                { name: 'isNoRefSJ', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoRefSJ", cls: "span2", type: "popup", btnName: "btnNoRefSJ", click: "NoRefSJ()", disable: "data.isNoRefSJ == false" },
                                { name: "NoRefSJTo", cls: "span2", type: "popup", btnName: "btnNoRefSJTo", click: "NoRefSJTo()", disable: "data.isNoRefSJ == false" },
                        ]
                    },
                    {
                        text: "No.BPU",
                        type: "controls",
                        items: [
                                { name: 'isNoBPU', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoBPU", cls: "span2", type: "popup", btnName: "btnNoBPU", click: "NoBPU()", disable: "data.isNoBPU == false" },
                                { name: "NoBPUTo", cls: "span2", type: "popup", btnName: "btnNoBPUTo", click: "NoBPUTo()", disable: "data.isNoBPU == false" },
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
                            { name: "tA", text: "BPU", cls: "active" },
                            { name: "tB", text: "BPU Detail" },
                        ]
                    },
                    {
                        name: "BPU",
                        title: "BPU",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailBPU",
                        title: "Detail BPU",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryBPU");
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
    }



});