"use strict";

function omInquiryPerlengkapanOut($scope, $http, $injector) {

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

    me.NoPerlengkapan = function () {
        var lookup = Wx.blookup({
            name: "PerlengkapanoutLookup",
            title: "PerlengkapanOut",
            manager: spSalesManager,
            query: "PerlengkapanoutLookup",
            defaultSort: "PerlengkapanNo asc",
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
            name: "PerlengkapanoutLookup",
            title: "PerlengkapanOut",
            manager: spSalesManager,
            query: "PerlengkapanoutLookup",
            defaultSort: "PerlengkapanNo asc",
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
        
        $http.post('om.api/InquirySales/searchPerlengkapanOut?Status=' + me.data.Status + '&PerlengkapanType=' + me.data.PerlengkapanType
                                                + '&PerlengkapanDate=' + PerlengkapanDate + '&PerlengkapanDateTo=' + PerlengkapanDateTo
                                                + '&NoPerlengkapan=' + me.data.NoPerlengkapan + '&NoPerlengkapanTo=' + me.data.NoPerlengkapanTo).
          success(function (data, status, headers, config) {
              $(".panel.tabpage1").hide();
              $(".panel.tabpage1.tA").show();
              me.clearTable(me.gridPerlengkapanOut);
              me.loadTableData(me.gridPerlengkapanOut, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridPerlengkapanOut = new webix.ui({
        container: "PerlengkapanOut",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "PerlengkapanType", header: "Tipe", width: 50 },
            { id: "PerlengkapanNo", header: "No.Perlengkapan", width: 150 },
            { id: "PerlengkapanDate", header: "Tgl.Perlengkapan", width: 150 },
            { id: "RefferenceNo", header: "No.Ref", width: 150 },
            { id: "RefferenceDate", header: "Tgl.Ref", width: 150 },
            { id: "CustomerCode", header: "Kode", width: 150 },
            { id: "CustomerName", header: "Pelanggan", width: 200 },
            { id: "Address", header: "Alamat", width: 300 },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "Status", header: "Status", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridPerlengkapanOut.getSelectedId() !== undefined) {
                    var data = this.getItem(me.gridPerlengkapanOut.getSelectedId().id);
                    var datas = {
                        "PerlengkapanNo": data.PerlengkapanNo
                    }

                    $http.post('om.api/InquirySales/GetDetailPerlengkapanOut', datas)
                    .success(function (e) {
                        if (e.success) {
                            $(".panel.tabpage1").hide();
                            $(".panel.tabpage1.tB").show();
                            me.loadTableData(me.griddetailPerlengkapanOut, e.detail);
                            me.loadTableData(me.griddetailPerlengkapan);
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

    me.griddetailPerlengkapanOut = new webix.ui({
        container: "DetailPerlengkapanOut",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 250 },
            { id: "Quantity", header: "Jumlah", width: 150, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", width: 650 },
        ]
    });

    me.griddetailPerlengkapan = new webix.ui({
        container: "DetailPerlengkapan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "", header: "Kode", width: 150 },
            { id: "", header: "Nama Perlengkapan", width: 150 },
            { id: "", header: "Jumlah Standar", width: 150, css: { "text-align": "right" } },
            { id: "", header: "Jumlah", width: 150, css: { "text-align": "right" } },
            { id: "", header: "Keterangan", width: 350 },
        ]
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridPerlengkapanOut.adjust(); break;
            case "tB": me.griddetailPerlengkapanOut.adjust(); me.griddetailPerlengkapan.adjust(); break;
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

    $("[name = 'isNoPerlengkapan']").on('change', function () {
        me.data.isNoPerlengkapan = $('#isNoPerlengkapan').prop('checked');
        me.data.NoPerlengkapan = "";
        me.data.NoPerlengkapanTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.PerlengkapanType = '';
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
        me.gridPerlengkapanOut.adjust();
    }
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Inquiry Perlengkapan Out",
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
                            { name: "tA", text: "Perlengkapan Out", cls: "active" },
                            { name: "tB", text: "Detail Perlengkapan Out" },
                        ]
                    },
                    {
                        name: "PerlengkapanOut",
                        title: "Perlengkapan Out",
                        cls: "tabpage1 tA",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailPerlengkapanOut",
                        title: "Detail Perlengkapan out",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
                    {
                        name: "DetailPerlengkapan",
                        title: "Detail Perlengkapan",
                        cls: "tabpage1 tB",
                        xtype: "wxtable"
                    },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryPerlengkapanOut");
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
    }



});