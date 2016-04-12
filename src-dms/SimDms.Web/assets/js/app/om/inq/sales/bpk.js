"use strict";

function omInquiryBPK($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Approved' },
       { "value": '3', "text": 'Cenceled' },
       { "value": '9', "text": 'Finised' }
    ];

    me.SONo = function () {
        var lookup = Wx.blookup({
            name: "BPKLookup",
            title: "SO",
            manager: spSalesManager,
            query: "BPKLookup",
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
            name: "BPKLookup",
            title: "SO",
            manager: spSalesManager,
            query: "BPKLookup",
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

    me.DONo = function () {
        var lookup = Wx.blookup({
            name: "BPKLookup",
            title: "DO",
            manager: spSalesManager,
            query: "BPKLookup",
            defaultSort: "DONo asc",
            columns: [
                { field: "DONo", title: "DO No" },
                { field: "DODate", title: "DO Date", template: "#= moment(DODate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DONo = data.DONo;
                me.Apply();
            }
        });

    }

    me.DONoTo = function () {
        var lookup = Wx.blookup({
            name: "BPKLookup",
            title: "DO",
            manager: spSalesManager,
            query: "BPKLookup",
            defaultSort: "DONo asc",
            columns: [
                { field: "DONo", title: "DO No" },
                { field: "DODate", title: "DO Date", template: "#= moment(DODate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DONoTo = data.DONo;
                me.Apply();
            }
        });

    }

    me.BPKNo = function () {
        var lookup = Wx.blookup({
            name: "BPKLookup",
            title: "BPK",
            manager: spSalesManager,
            query: "BPKLookup",
            defaultSort: "BPKNo asc",
            columns: [
                { field: "BPKNo", title: "BPK No" },
                { field: "BPKDate", title: "BPK Date", template: "#= moment(BPKDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BPKNo = data.BPKNo;
                me.Apply();
            }
        });

    }

    me.BPKNoTo = function () {
        var lookup = Wx.blookup({
            name: "BPKLookup",
            title: "BPK",
            manager: spSalesManager,
            query: "BPKLookup",
            defaultSort: "BPKNo asc",
            columns: [
                { field: "BPKNo", title: "BPK No" },
                { field: "BPKDate", title: "BPK Date", template: "#= moment(BPKDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BPKNoTo = data.DONo;
                me.Apply();
            }
        });


    }
    me.CariData = function () {
        var BPKDate = '1/1/1900';
        var BPKDateTo = '1/1/1900';

        if (me.data.BPKDate) {
            var BPKDate = new Date(me.data.BPKDate).getMonth() + 1 + '/' + new Date(me.data.BPKDate).getDate() + '/' + new Date(me.data.BPKDate).getFullYear();
            var BPKDateTo = new Date(me.data.BPKDateTo).getMonth() + 1 + '/' + new Date(me.data.BPKDateTo).getDate() + '/' + new Date(me.data.BPKDateTo).getFullYear();
        }

        $http.post('om.api/InquirySales/searchBPK?Status=' + me.data.Status + '&BPKDate=' + BPKDate + '&BPKDateTo=' + BPKDateTo
                                                + '&SONo=' + me.data.SONo + '&SONoTo=' + me.data.SONoTo
                                                + '&DONo=' + me.data.DONo + '&DONoTo=' + me.data.DONoTo
                                                + '&BPKNo=' + me.data.BPKNo + '&BPKNoTo=' + me.data.BPKNoTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.loadTableData(me.gridBPK, data);

              me.clearTable(me.gridDetailBPK);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridBPK = new webix.ui({
        container: "BPK",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "BPKNo", header: "No. BPK", width: 150 },
            { id: "SalesType", header: "Tipe", width: 50 },
            { id: "BPKDate", header: "Tgl. BPK", width: 150 },
            { id: "SONo", header: "No.SO", width: 150 },
            { id: "DONo", header: "No. DO", width: 150 },
            { id: "CustomerCode", header: "Kode", width: 150 },
            { id: "CustomerName", header: "Pelanggan", width: 200 },
            { id: "Address", header: "Alamat", width: 300 },
            { id: "ShipTo", header: "Kirim Ke.", width: 200 },
            { id: "WareHouseName", header: "Gudang", width: 200 },
            { id: "Expedition", header: "Expedisi", width: 150 },
            { id: "Status", header: "Status", width: 150 }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridBPK.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridBPK.getSelectedId().id);

                    var datas = {
                        "BPKNo": data.BPKNo
                    }

                    $http.post('om.api/InquirySales/DetailBPK', datas)
                    .success(function (data, status, headers, config) {
                        $(".panel.tabpage1").hide();
                        $(".panel.tabpage1.tB").show();
                        me.loadTableData(me.gridDetailBPK, data);
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.gridDetailBPK = new webix.ui({
        container: "DetailBPK",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 150 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 150 },
            { id: "SalesModelDesc", header: "Sales Model Desc", width: 250 },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChassisNo", header: "No. Rangka", width: 150 },
            { id: "EngineCode", header: "Kode Mesin", width: 200 },
            { id: "EngineNo", header: "No.Mesin", width: 300 },
            { id: "ColourCode", header: "Kode Warna", width: 200 },
            { id: "ColourName", header: "Nama Warna", width: 200 },
            { id: "StatusPDI", header: "PDI", width: 150 },
            { id: "Remark", header: "Keterangan", width: 150 }
        ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridBPK.adjust(); break;
            case "tB": me.gridDetailBPK.adjust(); break;
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

    $("[name = 'isNoDO']").on('change', function () {
        me.data.isNoDO = $('#isNoDO').prop('checked');
        me.data.DONo = "";
        me.data.DONoTo = "";
        me.Apply();
    });

    $("[name = 'isNoBPK']").on('change', function () {
        me.data.isNoBPK = $('#isNoBPK').prop('checked');
        me.data.BPKNo = "";
        me.data.BPKNoTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.SONo = '';
        me.data.SONoTo = '';
        me.data.DONo = '';
        me.data.DONoTo = '';
        me.data.BPKNo = '';
        me.data.BPKNoTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoSO').prop('checked', false);
        me.data.isNoSO = false;
        $('#isNoDO').prop('checked', false);
        me.data.isNoDO = false;
        $('#isNoBPK').prop('checked', false);
        me.data.isNoBPK = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridBPK.adjust();
    }

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Inquiry BPK",
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
                        text: "Tgl. BPK",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "DODate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "DODateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No SO",
                        type: "controls",
                        items: [
                                { name: 'isNoSO', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "SONo", cls: "span2", type: "popup", click: "SONo()", disable: "data.isNoSO == false" },
                                { name: "SONoTo", cls: "span2", type: "popup", click: "SONoTo()", disable: "data.isNoSO == false" },
                        ]
                    },
                    {
                        text: "No DO",
                        type: "controls",
                        items: [
                                { name: 'isNoDO', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "DONo", cls: "span2", type: "popup", click: "DONo()", disable: "data.isNoDO == false" },
                                { name: "DONoTo", cls: "span2", type: "popup", click: "DONoTo()", disable: "data.isNoDO == false" },
                        ]
                    },
                    {
                        text: "No BPK",
                        type: "controls",
                        items: [
                                { name: 'isNoBPK', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "BPKNo", cls: "span2", type: "popup", click: "BPKNo()", disable: "data.isNoBPK == false" },
                                { name: "BPKNoTo", cls: "span2", type: "popup", click: "BPKNoTo()", disable: "data.isNoBPK == false" },
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
                    { name: "tA", text: "BPK", cls: "active" },
                    { name: "tB", text: "Detai BPK" },
                ]
            },
                    {
                        name: "BPK",
                        title: "BPK",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailBPK",
                        title: "Detail BPK",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryBPK");
    }



});