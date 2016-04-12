"use strict";

function omInquiryPerlengkapan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Open' },
       { "value": '1', "text": 'Printed' },
       { "value": '2', "text": 'Deleted' },
       { "value": '3', "text": 'Canceled' },
       { "value": '9', "text": 'Finished' }
    ];

    me.TipePerlengkapan = [
      { "value": '1', "text": 'BPU' },
      { "value": '2', "text": 'Transfer' },
      { "value": '3', "text": 'Return' }
    ];


    me.NoReff = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "Refference",
            manager: spSalesManager,
            query: "PerlengkapanInLookup",
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
            query: "PerlengkapanInLookup",
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

    me.NoPerlengkapan = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "Refference",
            manager: spSalesManager,
            query: "PerlengkapanInLookup",
            defaultSort: "RefferenceNo asc",
            columns: [
                { field: "PerlengkapanNo", title: "No.Perlengkapan" },
                { field: "PerlengkapanDate", title: "Tgl.Perlengkapan", template: "#= moment(PerlengkapanDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoPerlengkapan = data.PerlengkapanNo;
                me.Apply();
            }
        });

    }

    me.NoPerlengkapanTo = function () {
        var lookup = Wx.blookup({
            name: "RefferenceLookup",
            title: "Refference",
            manager: spSalesManager,
            query: "PerlengkapanInLookup",
            defaultSort: "RefferenceNo asc",
            columns: [
                { field: "PerlengkapanNo", title: "No.Perlengkapan" },
                { field: "PerlengkapanDate", title: "Tgl.Perlengkapan", template: "#= moment(PerlengkapanDate).format('DD MMM YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NoPerlengkapanTo = data.PerlengkapanNo;
                me.Apply();
            }
        });

    }

    me.CariData = function () {
        var PerlengkapanDate = '1/1/1900';
        var PerlengkapanDateTo = '1/1/1900';

        if (me.data.PerlengkapanDate) {
            var PerlengkapanDate = new Date(me.data.PerlengkapanDate).getMonth() + 1 + '/' + new Date(me.data.PerlengkapanDate).getDate() + '/' + new Date(me.data.PerlengkapanDate).getFullYear();
            var PerlengkapanDateTo = new Date(me.data.PerlengkapanDateTo).getMonth() + 1 + '/' + new Date(me.data.PerlengkapanDateTo).getDate() + '/' + new Date(me.data.PerlengkapanDateTo).getFullYear();
        }

        $http.post('om.api/InquiryPurchase/searchPerlengkapanIn?Status=' + me.data.Status + '&PerlengkapanType=' + me.data.PerlengkapanType
                                                + '&PerlengkapanDate=' + PerlengkapanDate + '&PerlengkapanDateTo=' + PerlengkapanDateTo
                                                + '&NoReff=' + me.data.NoReff + '&NoReffTo=' + me.data.NoReffTo
                                                + '&NoPerlengkapan=' + me.data.NoPerlengkapan + '&NoPerlengkapanTo=' + me.data.NoPerlengkapanTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.gridPerlengkapanIn);
              me.loadTableData(me.gridPerlengkapanIn, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridPerlengkapanIn = new webix.ui({
        container: "PerlengkapanIn",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "PerlengkapanNo", header: "No.Perlengkapan", width: 150 },
            { id: "PerlengkapanDate", header: "Tgl.Perlengkapan", width: 150 },
            { id: "RefferenceNo", header: "No.Ref", width: 150 },
            { id: "RefferenceDate", header: "Tgl.Ref", width: 150 },
            { id: "PerlengkapanType", header: "Tipe", width: 150 },
            { id: "SourceDoc", header: "No Sumber Dok,", width: 150 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridPerlengkapanIn.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridPerlengkapanIn.getSelectedId().id);
                    var datas = {
                        "PerlengkapanNo": data.PerlengkapanNo
                    }

                    $http.post('om.api/InquiryPurchase/GetDetailPerlengkapanIn', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.griddetailPerlengkapanIn, e.detail);
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

    me.griddetailPerlengkapanIn = new webix.ui({
        container: "DetailPerlengkapanIn",
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
            case "tA": me.gridPerlengkapanIn.adjust(); break;
            case "tB": me.griddetailPerlengkapanIn.adjust(); break;
            default:
        }
    };


    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.Status = "";
        me.Apply();
    });

    $("[name = 'isActiveType']").on('change', function () {
        me.data.isActiveType = $('#isActiveType').prop('checked');
        me.data.TipePerlengkapan = "";
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

    $("[name = 'isNoPerlengkapan']").on('change', function () {
        me.data.isNoPerlengkapan = $('#isNoPerlengkapan').prop('checked');
        me.data.NoPerlengkapan = "";
        me.data.NoPerlengkapanTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.PerlengkapanType = '';
        me.data.NoReff = '';
        me.data.NoReffTo = '';
        me.data.NoPerlengkapan = '';
        me.data.NoPerlengkapanTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveType').prop('checked', false);
        me.data.isActiveType = false;
        $('#isActiveDate').prop('checked', false);
        me.data.isActiveDate = false;
        $('#isNoReff').prop('checked', false);
        me.data.isNoReff = false;
        $('#isNoPerlengkapan').prop('checked', false);
        me.data.isNoPerlengkapan = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridPerlengkapanIn.adjust();
    }
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Inquiry Perlengkapan In",
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
                        text: "Tipe Perlengkapan",
                        type: "controls",
                        items: [
                            { name: 'isActiveType', type: 'check', cls: "span1", text: "Status", float: 'left' },
                            { name: "PerlengkapanType", opt_text: "", cls: "span3", type: "select2", text: "", datasource: "TipePerlengkapan", disable: "data.isActiveType == false" },

                        ]
                    },
                    {
                        text: "Tgl. Perlengkapan",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "PerlengkapanDate", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "PerlengkapanDateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
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
                        text: "No.Perlengkapan",
                        type: "controls",
                        items: [
                                { name: 'isNoPerlengkapan', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "NoPerlengkapan", cls: "span2", type: "popup", btnName: "btnNoPerlengkapan", click: "NoPerlengkapan()", disable: "data.isNoPerlengkapan == false" },
                                { name: "NoPerlengkapanTo", cls: "span2", type: "popup", btnName: "btnNoPerlengkapanTo", click: "NoPerlengkapanTo()", disable: "data.isNoPerlengkapan == false" },
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
                            { name: "tA", text: "Perlengkapan In", cls: "active" },
                            { name: "tB", text: "Detail Perlengkapan In" },
                        ]
                    },
                    {
                        name: "PerlengkapanIn",
                        title: "Perlengkapan In",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailPerlengkapanIn",
                        title: "Detail Perlengkapan In",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryPerlengkapan");
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
    }



});