"use strict"; //Reportid OmRpMst007
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    me.Status = [
           { "value": '2', "text": 'All' },
           { "value": '0', "text": 'Tidak Aktif' },
           { "value": '1', "text": 'Aktif' },
    ];

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

    $("[name='SupplierCode']").on('blur', function () {
        if (me.data.SupplierCode != "") {
            $http.post('om.api/MstKaroseri/SupplierCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.SupplierCode = data.data.SupplierCode;
                       me.data.SupplierName = data.data.SupplierName;
                   }
                   else {
                       me.data.SupplierCode = "";
                       me.data.SupplierName = "";
                       me.SupplierCode();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='SupplierCodeTo']").on('blur', function () {
        if (me.data.SupplierCodeTo != "") {
            $http.post('om.api/MstKaroseri/SupplierCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.SupplierCodeTo = data.data.SupplierCode;
                       me.data.SupplierNameTo = data.data.SupplierName;
                   }
                   else {
                       me.data.SupplierCodeTo = "";
                       me.data.SupplierNameTo = "";
                       me.SupplierCodeTo();
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

    $("[name='SalesModelCodeBaru']").on('blur', function () {
        if (me.data.SalesModelCodeBaru != "") {
            $http.post('om.api/MstKaroseri/ModelCodeBaru', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.SalesModelCodeBaru = data.data.SalesModelCode;
                       me.data.SalesModelDescBaru = data.data.SalesModelDesc;
                       me.Apply();
                   }
                   else {
                       me.data.SalesModelCodeBaru = "";
                       me.data.SalesModelDescBaru = "";
                       me.SalesModelCodeBaru();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='SalesModelCodeBaruTo']").on('blur', function () {
        if (me.data.SalesModelCodeBaruTo != "") {
            $http.post('om.api/MstKaroseri/ModelCodeBaru', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.SalesModelCodeBaruTo = data.data.SalesModelCode;
                       me.data.SalesModelDescBaruTo = data.data.SalesModelDesc;
                       me.Apply();
                   }
                   else {
                       me.data.SalesModelCodeBaruTo = "";
                       me.data.SalesModelDescBaruTo = "";
                       me.SalesModelCodeBaruTo();
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
                    me.data.SupplierCode,
                    me.data.SupplierCodeTo,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: "OmRpMst007",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.SupplierCode = "";
        me.data.SupplierName = "";
        me.data.SupplierCodeTo = "";
        me.data.SupplierNameTo = "";
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

    $("[name = 'isActiveSalesModelCodeBaru']").on('change', function () {
        me.data.isActiveSalesModelCodeBaru = $('#isActiveSalesModelCodeBaru').prop('checked');
        me.data.SupplierCode = "";
        me.data.SalesModelDescBaru = "";
        me.data.SalesModelCodeBaruTo = "";
        me.data.SalesModelDescBaruTo = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data = {};
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveSalesModelCode').prop('checked', false);
        me.data.isActiveSalesModelCode = false;
        $('#isActiveSalesModelCodeBaru').prop('checked', false);
        me.data.isActiveSalesModelCodeBaru = false;
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
        title: "Report Karoseri",
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
                        { name: 'isActive', type: 'check', cls: 'span2', text: "Pemasok", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SupplierCode", cls: "span2", type: "popup", btnName: "btnSupplierCode", click: "SupplierCode()", disable: "data.isActive == false" },
                                { name: "SupplierName", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SupplierCodeTo", cls: "span2", type: "popup", btnName: "btnSupplierCodeTo", click: "SupplierCodeTo()", disable: "data.isActive == false" },
                                { name: "SupplierNameTo", cls: "span4", readonly: true },
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
                        { name: 'isActiveSalesModelCodeBaru', type: 'check', cls: 'span2', text: "Sales Model Baru", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCodeBaru", cls: "span2", type: "popup", btnName: "btnSalesModelCodeBaru", click: "SalesModelCodeBaru()", disable: "data.isActiveSalesModelCodeBaru == false" },
                                { name: "SalesModelDescBaru", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCodeBaruTo", cls: "span2", type: "popup", btnName: "btnSalesModelCodeBaruTo", click: "SalesModelCodeBaruTo()", disable: "data.isActiveSalesModelCodeBaru == false" },
                                { name: "SalesModelDescBaruTo", cls: "span4", readonly: true },
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
        SimDms.Angular("spRptMstSales");

    }
});