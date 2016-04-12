"use strict";

function omInquiryPerlengkapanAdj($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Deleted' },
       { "value": '3', "text": 'Canceled' },
       { "value": '9', "text": 'Finished' }
    ];

    me.NoReff = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "Refference",
            manager: spSalesManager,
            query: "PerlengkapanAdjustmentLookup",
            defaultSort: "RefferenceNo asc",
            columns: [
                { field: "RefferenceNo", title: "Reff No" },
                { field: "RefferenceDate", title: "Reff Date", template: "#= moment(RefferenceDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoReff = data.RefferenceNo;
                me.Apply();
            }
        });

    }

    me.NoReffTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "Refference",
            manager: spSalesManager,
            query: "PerlengkapanAdjustmentLookup",
            defaultSort: "RefferenceNo asc",
            columns: [
                { field: "RefferenceNo", title: "Reff No" },
                { field: "RefferenceDate", title: "Reff Date", template: "#= moment(RefferenceDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoReffTo = data.RefferenceNo;
                me.Apply();
            }
        });

    }

    me.NoAdjustment = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "Adjustment",
            manager: spSalesManager,
            query: "PerlengkapanAdjustmentLookup",
            defaultSort: "AdjustmentNo asc",
            columns: [
                { field: "AdjustmentNo", title: "No.Adjustment" },
                { field: "AdjustmentDate", title: "Tgl.Adjustment", template: "#= moment(AdjustmentDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoAdjustment = data.AdjustmentNo;
                me.Apply();
            }
        });

    }

    me.NoAdjustmentTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "Adjustment",
            manager: spSalesManager,
            query: "PerlengkapanAdjustmentLookup",
            defaultSort: "AdjustmentNo asc",
            columns: [
                { field: "AdjustmentNo", title: "No.Adjustment" },
                { field: "AdjustmentDate", title: "Tgl.Adjustment", template: "#= moment(AdjustmentDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoAdjustmentTo = data.AdjustmentNo;
                me.Apply();
            }
        });

    }

    me.CariData = function () {
        var AdjustmentDate = '1/1/1900';
        var AdjustmentDateTo = '1/1/1900';

        if (me.data.AdjustmentDate) {
            var AdjustmentDate = new Date(me.data.AdjustmentDate).getMonth() + 1 + '/' + new Date(me.data.AdjustmentDate).getDate() + '/' + new Date(me.data.AdjustmentDate).getFullYear();
            var AdjustmentDateTo = new Date(me.data.AdjustmentDateTo).getMonth() + 1 + '/' + new Date(me.data.AdjustmentDateTo).getDate() + '/' + new Date(me.data.AdjustmentDateTo).getFullYear();
        }
        
        $http.post('om.api/InquiryPurchase/searchPerlengkapanAdjustment?Status=' + me.data.Status
                                                + '&AdjustmentDate=' + AdjustmentDate + '&AdjustmentDateTo=' + AdjustmentDateTo
                                                + '&NoReff=' + me.data.NoReff + '&NoReffTo=' + me.data.NoReffTo
                                                + '&NoAdjustment=' + me.data.NoAdjustment + '&NoAdjustmentTo=' + me.data.NoAdjustmentTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.griddetailPerlengkapanAdjustment);
              me.loadTableData(me.gridPerlengkapanAdjustment, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridPerlengkapanAdjustment = new webix.ui({
        container: "PerlengkapanAdjustment",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "AdjustmentNo", header: "No.Adjustment", width: 150 },
            { id: "AdjustmentDate", header: "Tgl.Adjustment", width: 150 },
            { id: "RefferenceNo", header: "No.Ref", width: 150 },
            { id: "RefferenceDate", header: "Tgl.Ref", width: 150 },
            { id: "Remark", header: "Keterangan", width: 350 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridPerlengkapanAdjustment.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridPerlengkapanAdjustment.getSelectedId().id);
                    var datas = {
                        "AdjustmentNo": data.AdjustmentNo
                    }

                    $http.post('om.api/InquiryPurchase/GetDetailPerlengkapanAdjustment', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.griddetailPerlengkapanAdjustment, e.detail);
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

    me.griddetailPerlengkapanAdjustment = new webix.ui({
        container: "DetailPerlengkapanAdjustment",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "PerlengkapanName", header: "Perlengkapan", width: 250 },
            { id: "Quantity", header: "Jumlah", width: 150, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", width: 650 },
        ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridPerlengkapanAdjustment.adjust(); break;
            case "tB": me.griddetailPerlengkapanAdjustment.adjust(); break;
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

    $("[name = 'isNoReff']").on('change', function () {
        me.data.isNoReff = $('#isNoReff').prop('checked');
        me.data.NoReff = "";
        me.data.NoReffTo = "";
        me.Apply();
    });

    $("[name = 'isNoAjustment']").on('change', function () {
        me.data.isNoAjustment = $('#isNoAjustment').prop('checked');
        me.data.NoAdjustment = "";
        me.data.NoAdjustmentTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.NoReff = '';
        me.data.NoReffTo = '';
        me.data.NoAdjustment = '';
        me.data.NoAdjustmentTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoReff').prop('checked', false);
        me.data.isNoReff = false;
        $('#isNoAjustment').prop('checked', false);
        me.data.isNoAjustment = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridPerlengkapanAdjustment.adjust();
    }
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Inquiry Perlengkapan Adjustment",
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
                        text: "Tgl. Adjustment",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "AdjustmentDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "AdjustmentDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No.Ref",
                        type: "controls",
                        items: [
                                { name: 'isNoReff', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoReff", cls: "span2", type: "popup", btnName: "btnNoReff", click: "NoReff()", disable: "data.isNoReff == false" },
                                { name: "NoReffTo", cls: "span2", type: "popup", btnName: "btnNoReffTo", click: "NoReffTo()", disable: "data.isNoReff == false" },
                        ]
                    },
                    {
                        text: "No.Adjustment",
                        type: "controls",
                        items: [
                                { name: 'isNoAjustment', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoAdjustment", cls: "span2", type: "popup", btnName: "btnNoAdjustment", click: "NoAdjustment()", disable: "data.isNoAjustment == false" },
                                { name: "NoAdjustmentTo", cls: "span2", type: "popup", btnName: "btnNoAdjustmentTo", click: "NoAdjustmentTo()", disable: "data.isNoAjustment == false" },
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
                            { name: "tA", text: "Perlengkapan Adjustment", cls: "active" },
                            { name: "tB", text: "Detail Perlengkapan Adjustment" },
                        ]
                    },
                    {
                        name: "PerlengkapanAdjustment",
                        title: "Perlengkapan Adjustment",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailPerlengkapanAdjustment",
                        title: "Detail Perlengkapan Adjustment",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryPerlengkapanAdj");
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
    }



});