"use strict"; //Reportid OmRpMst010
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

    me.CityCode = function () {
        var lookup = Wx.blookup({
            name: "CityCodeLookup",
            title: "City Code",
            manager: spSalesManager,
            query: "CityCodeLookup",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "City Code" },
                { field: "LookUpValueName", title: "City Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CityCode = data.LookUpValue;
                me.data.CityName = data.LookUpValueName;
                me.Apply();
            }
        });

    }

    me.CityCodeTo = function () {
        var lookup = Wx.blookup({
            name: "CityCodeLookup",
            title: "City Code",
            manager: spSalesManager,
            query: "CityCodeLookup",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "City Code" },
                { field: "LookUpValueName", title: "City Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CityCodeTo = data.LookUpValue;
                me.data.CityNameTo = data.LookUpValueName;
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

    $("[name='CityCode']").on('blur', function () {
        if (me.data.CityCode != "") {
            $http.post('om.api/MstBBNKIR/CityCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.CityCode = data.data.LookUpValue;
                       me.data.CityName = data.data.LookUpValueName;
                   }
                   else {
                       me.data.CityCode = "";
                       me.data.CityName = "";
                       me.CityCode();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='CityCodeTo']").on('blur', function () {
        if (me.data.CityCodeTo != "") {
            $http.post('om.api/MstBBNKIR/CityCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.CityCodeTo = data.data.LookUpValue;
                       me.data.CityNameTo = data.data.LookUpValueName;
                   }
                   else {
                       me.data.CityCodeTo = "";
                       me.data.CityNameTo = "";
                       me.CityCodeTo();
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
                    me.data.CityCode,
                    me.data.CityCodeTo,
                    me.data.SalesModelCode,
                    me.data.SalesModelCodeTo,
                    me.data.Year,
                    me.data.YearTo,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: "OmRpMst010",
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
        me.data.SupplierCode = "";
        me.data.SupplierName = "";
        me.data.SupplierCodeTo = "";
        me.data.SupplierNameTo = "";
        me.Apply();
    });

    $("[name = 'isActiveCity']").on('change', function () {
        me.data.isActiveCity = $('#isActiveCity').prop('checked');
        me.data.CityCode = "";
        me.data.CityName = "";
        me.data.CityCodeTo = "";
        me.data.CityNameTo = "";
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
        $('#isActiveCity').prop('checked', false);
        me.data.isActiveCity = false;
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
        title: "Report BBN dan KIR",
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
                        { name: 'isActiveCity', type: 'check', cls: 'span2', text: "Kota", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "CityCode", cls: "span2", type: "popup", btnName: "btnCityCode", click: "CityCode()", disable: "data.isActiveCity == false" },
                                { name: "CityName ", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "CityCodeTo", cls: "span2", type: "popup", btnName: "btnCityCodeTo", click: "CityCodeTo()", disable: "data.isActiveCity == false" },
                                { name: "CityNameTo ", cls: "span4", readonly: true },
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