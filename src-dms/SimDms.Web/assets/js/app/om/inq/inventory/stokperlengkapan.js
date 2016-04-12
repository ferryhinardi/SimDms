"use strict";

function omInquiryStokPerlengkapan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
       { "value": '0', "text": 'Non Active' },
       { "value": '1', "text": 'Active' }
    ];

    $http.post('om.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    $http.post('om.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.Months = data;
    });

    me.PerlengkapanCode = function () {
        var lookup = Wx.blookup({
            name: "InquiryPerlengkapanLookup",
            title: "Perlengkapan",
            manager: spSalesManager,
            query: "InquiryPerlengkapanLookup",
            defaultSort: "PerlengkapanCode asc",
            columns: [
                { field: "PerlengkapanCode", title: "Perlengkapan Code" },
                { field: "PerlengkapanName", title: "Perlengkapan Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PerlengkapanCode = data.PerlengkapanCode;
                me.data.PerlengkapanName = data.PerlengkapanName;
                me.Apply();
            }
        });

    }

    me.PerlengkapanCodeTo = function () {
        var lookup = Wx.blookup({
            name: "InquiryPerlengkapanLookup",
            title: "Perlengkapan",
            manager: spSalesManager,
            query: "InquiryPerlengkapanLookup",
            defaultSort: "PerlengkapanCode asc",
            columns: [
                { field: "PerlengkapanCode", title: "Perlengkapan Code" },
                { field: "PerlengkapanName", title: "Perlengkapan Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PerlengkapanCodeTo = data.PerlengkapanCode;
                me.data.PerlengkapanNameTo = data.PerlengkapanName;
                me.Apply();
            }
        });

    }

    me.CariData = function () {
        $http.post('om.api/InquiryInventory/searchStokPerlengkapan?Status=' + me.data.Status + '&Year=' + me.data.Year + '&Month=' + me.data.Months
                                                + '&PerlengkapanCode=' + me.data.PerlengkapanCode + '&PerlengkapanCodeTo=' + me.data.PerlengkapanCodeTo).
          success(function (data, status, headers, config) {
              me.loadTableData(me.gridStokPerlengkapan, data);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.gridStokPerlengkapan = new webix.ui({
        container: "wxStokPerlengkapan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "Year", header: "Tahun", width: 100 },
            { id: "Month", header: "Bulan", width: 100 },
            { id: "PerlengkapanCode", header: "Kode", width: 150 },
            { id: "PerlengkapanName", header: "Perlengkapan", width: 150 },
            { id: "QuantityBeginning", header: "Saldo Awal", width: 100 },
            { id: "QuantityIn", header: "Masuk", width: 100 },
            { id: "QuantityOut", header: "Keluar", width: 100 },
            { id: "QuantityEnding", header: "Saldo Akhir", width: 100 },
            { id: "Status", header: "Status", width: 150 }
        ]
    });

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.Status = "";
        me.Apply();
    });

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

    $("[name = 'isPerlengkapanCode']").on('change', function () {
        me.data.isPerlengkapanCode = $('#isPerlengkapanCode').prop('checked');
        me.data.PerlengkapanCode = "";
        me.data.PerlengkapanCodeTo = "";
        me.data.PerlengkapanName = "";
        me.data.PerlengkapanNameTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.Status = '';
        me.data.Year = '';
        me.data.Months = '';
        me.data.PerlengkapanCode = '';
        me.data.PerlengkapanCodeTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveYear').prop('checked', false);
        me.data.isActiveYear = false;
        $('#isActiveMonth').prop('checked', false);
        me.data.isActiveMonth = false;
        $('#isPerlengkapanCode').prop('checked', false);
        me.data.isPerlengkapanCode = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.gridStokPerlengkapan.adjust();
    }
    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Inquiry Stok Perlengkapan",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                     {
                         text: "Status",
                         type: "controls",
                         items: [
                             { name: 'isActive', type: 'check', cls: "span1", float: 'left' },
                             { name: "Status", cls: "span2", type: "select2", datasource: "Status", text: "Year", disable: "data.isActive == false" },

                         ]
                     },
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
                    { name: 'isPerlengkapanCode', type: 'check', cls: "", text: "Perlengkapan", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "PerlengkapanCode", cls: "span2", type: "popup", btnName: "btnPerlengkapanCode", click: "PerlengkapanCode()", disable: "data.isPerlengkapanCode == false" },
                                { name: "PerlengkapanName", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "PerlengkapanCodeTo", cls: "span2", type: "popup", btnName: "btnPerlengkapanCodeTo", click: "PerlengkapanCodeTo()", disable: "data.isPerlengkapanCode == false" },
                                { name: "PerlengkapanNameTo", cls: "span4", readonly: true },
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
                name: "wxStokPerlengkapan",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omInquiryStokPerlengkapan");
    }



});