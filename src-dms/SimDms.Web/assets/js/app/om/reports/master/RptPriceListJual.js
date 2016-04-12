"use strict"; //Reportid OmRpMst009
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    me.Status = [
           { "value": '2', "text": 'All' },
           { "value": '0', "text": 'Tidak Aktif' },
           { "value": '1', "text": 'Aktif' },
    ];

    me.GroupPriceCode = function () {
        var lookup = Wx.blookup({
            name: "GroupPriceCodeLookup",
            title: "Group Price",
            manager: spSalesManager,
            query: "GroupPriceCodeLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Kode Reff." },
                { field: "RefferenceDesc1", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.GroupPriceCode = data.RefferenceCode;
                me.data.GroupPriceName = data.RefferenceDesc1;
                me.Apply();
            }
        });
    }

    me.GroupPriceCodeTo = function () {
        var lookup = Wx.blookup({
            name: "GroupPriceCodeLookup",
            title: "Group Price",
            manager: spSalesManager,
            query: "GroupPriceCodeLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Kode Reff." },
                { field: "RefferenceDesc1", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.GroupPriceCodeTo = data.RefferenceCode;
                me.data.GroupPriceNameTo = data.RefferenceDesc1;
                me.Apply();
            }
        });
    }

    me.SalesModelCode = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCode = data.SalesModelCode;
                me.data.SalesModelDesc = data.SalesModelDesc;
                me.Apply();
            }
        });
    }

    me.SalesModelCodeTo = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCodeTo = data.SalesModelCode;
                me.data.SalesModelDescTo = data.SalesModelDesc;
                me.Apply();
            }
        });
    }

    me.SalesModelCodeBaru = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCodeBaru = data.SalesModelCode;
                me.data.SalesModelDescBaru = data.SalesModelDesc;
                me.Apply();
            }
        });
    }

    me.SalesModelCodeBaruTo = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCodeBaruTo = data.SalesModelCode;
                me.data.SalesModelDescBaruTo = data.SalesModelDesc;
                me.Apply();
            }
        });
    }

    $("[name='GroupPriceCode']").on('blur', function () {
        if (me.data.GroupPriceCode != "") {
            $http.post('om.api/MstPriceListJual/GroupPriceCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.GroupPriceCode = data.data.RefferenceCode;
                       me.data.GroupPriceName = data.data.RefferenceDesc1;
                   }
                   else {
                       me.data.GroupPriceCode = "";
                       me.data.GroupPriceName = "";
                       me.GroupPriceCode();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='GroupPriceCodeTo']").on('blur', function () {
        if (me.data.GroupPriceCode != "") {
            $http.post('om.api/MstPriceListJual/GroupPriceCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.GroupPriceCodeTo = data.data.RefferenceCode;
                       me.data.GroupPriceNameTo = data.data.RefferenceDesc1;
                   }
                   else {
                       me.data.GroupPriceCodeTo = "";
                       me.data.GroupPriceNameTo = "";
                       me.GroupPriceCodeTo();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='SalesModelCode']").on('blur', function () {
        if (me.data.SalesModelCode != "") {
            $http.post('om.api/MstModelColor/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.SalesModelCode = data.data.SalesModelCode;
                       me.data.SalesModelDesc = data.data.SalesModelDesc;
                       me.Apply();
                   }
                   else {
                       me.data.SalesModelCode = "";
                       me.data.SalesModelDesc = "";
                       me.SalesModelCode();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='SalesModelCodeTo']").on('blur', function () {
        if (me.data.SalesModelCodeTo != "") {
            $http.post('om.api/MstModelColor/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.SalesModelCodeTo = data.data.SalesModelCode;
                       me.data.SalesModelDescTo = data.data.SalesModelDesc;
                       me.Apply();
                   }
                   else {
                       me.data.SalesModelCodeTo = "";
                       me.data.SalesModelDescTo = "";
                       me.SalesModelCodeTo();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });


    me.printPreview = function () {


        if (me.data.Status == '2') {
            me.data.Status = '';
        }


        var prm = [
                    me.data.SupplierCode,
                    me.data.SupplierCodeTo,
                    me.data.SalesModelCode,
                    me.data.SalesModelCodeTo,
                    me.data.Year,
                    me.data.YearTo,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: "OmRpMst009",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    $http.post('om.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.GroupPriceCode = "";
        me.data.GroupPriceName = "";
        me.data.GroupPriceCodeTo = "";
        me.data.GroupPriceNameTo = "";
        me.Apply();
    });

    $("[name = 'isActiveSalesModelCode']").on('change', function () {
        me.data.isActiveSalesModelCode = $('#isActiveSalesModelCode').prop('checked');
        me.data.SalesModelCode = "";
        me.data.SalesModelDesc = "";
        me.data.SalesModelCodeTo = "";
        me.data.SalesModelDescTo = "";
        me.Apply();
    });

    $("[name = 'isActiveYear']").on('change', function () {
        me.data.isActiveYear = $('#isActiveYear').prop('checked');
        me.Apply();
    });

    me.initialize = function () {
        me.data = {};
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveSalesModelCode').prop('checked', false);
        me.data.isActiveSalesModelCode = false;
        $('#isActiveYear').prop('checked', false);
        me.data.isActiveYear = false;
        me.data.Status = '2';
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });

        me.isPrintAvailable = true;
    }


    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Report PriceList Jual",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: 'isActive', type: 'check', cls: 'span2', text: "Group Price", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "GroupPriceCode", cls: "span2", type: "popup", btnName: "btnGroupPriceCode", click: "GroupPriceCode()", disable: "data.isActive == false" },
                                { name: "GroupPriceName", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "GroupPriceCodeTo", cls: "span2", type: "popup", btnName: "btnGroupPriceCodeTo", click: "GroupPriceCodeTo()", disable: "data.isActive == false" },
                                { name: "GroupPriceNameTo", cls: "span4", readonly: true },
                            ]
                        },
                        { name: 'isActiveSalesModelCode', type: 'check', cls: 'span2', text: "Sales Model Lama", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCode", cls: "span2", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCode()", disable: "data.isActiveSalesModelCode == false" },
                                { name: "SalesModelDesc", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCodeTo", cls: "span2", type: "popup", btnName: "btnSalesModelCodeTo", click: "SalesModelCodeTo()", disable: "data.isActiveSalesModelCode == false" },
                                { name: "SalesModelDescTo", cls: "span4", readonly: true },
                            ]
                        },
                        { name: 'isActiveYear', type: 'check', cls: 'span2', text: "Tahun", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "Year", cls: "span2", type: "select2", datasource: "Year", disable: "data.isActiveYear == false" },
                                { type: "label", text: "S/D", cls: "span1 mylabel" },
                                { name: "YearTo", cls: "span2", type: "select2", datasource: "Year", disable: "data.isActiveYear == false" },
                            ]
                        },
                        { name: "Status", opt_text: "", cls: "span4", type: "select2", text: "Status", datasource: "Status" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("spRptMstSales");

    }
});