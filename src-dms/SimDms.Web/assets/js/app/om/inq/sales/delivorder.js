"use strict";

function omInquiryDO($scope, $http, $injector) {

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
            name: "DOLookup",
            title: "SO",
            manager: spSalesManager,
            query: "DOLookup",
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
            name: "DOLookup",
            title: "SO",
            manager: spSalesManager,
            query: "DOLookup",
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
            name: "DOLookup",
            title: "DO",
            manager: spSalesManager,
            query: "DOLookup",
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
            name: "DOLookup",
            title: "DO",
            manager: spSalesManager,
            query: "DOLookup",
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

    me.CariData = function () {
        var DODate = '1/1/1900';
        var DODateTo = '1/1/1900';

        if (me.data.DODate) {
            var DODate = new Date(me.data.DODate).getMonth() + 1 + '/' + new Date(me.data.DODate).getDate() + '/' + new Date(me.data.DODate).getFullYear();
            var DODateTo = new Date(me.data.DODateTo).getMonth() + 1 + '/' + new Date(me.data.DODateTo).getDate() + '/' + new Date(me.data.DODateTo).getFullYear();
        }

        $http.post('om.api/InquirySales/searchDO?Status=' + me.data.Status + '&DODate=' + DODate + '&DODateTo=' + DODateTo
                                                + '&SONo=' + me.data.SONo + '&SONoTo=' + me.data.SONoTo
                                                + '&DONo=' + me.data.DONo + '&DONoTo=' + me.data.DONoTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.loadTableData(me.gridDO, data);

              me.clearTable(me.gridDetailDO);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridDO = new webix.ui({
        container: "DO",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "DONo", header: "No. DO", width: 150 },
            { id: "SalesType", header: "Tipe", width: 50 },
            { id: "DODate", header: "Tgl. DO", width: 150 },
            { id: "SONo", header: "No.SO", width: 150 },
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
                if (me.gridDO.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridDO.getSelectedId().id);

                    var datas = {
                        "DONo": data.DONo
                    }

                    $http.post('om.api/InquirySales/DetailDO', datas)
                    .success(function (data, status, headers, config) {
                        //if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.gridDetailDO, data);
                        //} else {
                        //    MsgBox(e.message, MSG_ERROR);
                        //}
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.gridDetailDO = new webix.ui({
        container: "DetailDO",
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
            { id: "Remark", header: "Keterangan", width: 150 }
        ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridDO.adjust(); break;
            case "tB": me.gridDetailDO.adjust(); break;
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

    me.initialize = function () {
        me.data.Status = '';
        me.data.SONo = '';
        me.data.SONoTo = '';
        me.data.DONo = '';
        me.data.DONoTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoSO').prop('checked', false);
        me.data.isNoSO = false;
        $('#isNoDO').prop('checked', false);
        me.data.isNoDO = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridDO.adjust();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Delivery Order",
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
                        text: "Tgl. DO",
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
                                { name: "SONo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNo", click: "SONo()", disable: "data.isNoSO == false" },
                                { name: "SONoTo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNoTo", click: "SONoTo()", disable: "data.isNoSO == false" },
                        ]
                    },
                    {
                        text: "No DO",
                        type: "controls",
                        items: [
                                { name: 'isNoDO', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "DONo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNo", click: "DONo()", disable: "data.isNoDO == false" },
                                { name: "DONoTo", cls: "span2", type: "popup", btnName: "btnKaroseriSPKNoTo", click: "DONoTo()", disable: "data.isNoDO == false" },
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
                    { name: "tA", text: "D.O", cls: "active" },
                    { name: "tB", text: "Detai D.O" },
                ]
            },
                    {
                        name: "DO",
                        title: "D.O",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailDO",
                        title: "Detail D.O",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },                    
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryDO");
    }



});