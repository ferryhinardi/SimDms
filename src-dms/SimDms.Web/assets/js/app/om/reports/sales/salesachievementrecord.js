"use strict"; //Reportid OmRpSalAch
function RptSalesAchievementRecord($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.EmployeeCodeFrom = function () {
        var lookup = Wx.blookup({
            name: "SalesmanIDLookup",
            title: "Salesman",
            manager: spSalesManager,
            query: "SalesmanIDLookup",
            defaultSort: "EmployeeID asc",
            columns: [
                { field: "EmployeeID", title: "Kode Sales" },
                { field: "EmployeeName", title: "Nama Sales" },
                { field: "LookUpValueName", title: "Jabatan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.EmployeeCodeFrom = data.EmployeeID;
                me.data.LookupNameFrom = data.EmployeeName;
                me.Apply();
            }
        });

    }

    me.EmployeeCodeTo = function () {
        var lookup = Wx.blookup({
            name: "SalesmanIDLookup",
            title: "Salesman",
            manager: spSalesManager,
            query: "SalesmanIDLookup",
            defaultSort: "EmployeeID asc",
            columns: [
                { field: "EmployeeID", title: "Kode Sales" },
                { field: "EmployeeName", title: "Nama Sales" },
                { field: "LookUpValueName", title: "Jabatan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.EmployeeCodeTo = data.EmployeeID;
                me.data.LookupNameTo = data.EmployeeName;
                me.Apply();
            }
        });

    }

    me.LookupCodeFrom = function () {
        var lookup = Wx.blookup({
            name: "GroupARLookup",
            title: "Sales",
            manager: spSalesManager,
            query: "GroupARLookup",
            defaultSort: "ParaValue asc",
            columns: [
                { field: "ParaValue", title: "Kode Kategori" },
                { field: "LookUpValueName", title: "Nama Kategori" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.LookupCodeFrom = data.ParaValue;
                me.data.LookupNameFrom = data.LookUpValueName;
                me.Apply();
            }
        });

    }

    me.LookupCodeTo = function () {
        var lookup = Wx.blookup({
            name: "GroupARLookup",
            title: "Sales",
            manager: spSalesManager,
            query: "GroupARLookup",
            defaultSort: "ParaValue asc",
            columns: [
                { field: "ParaValue", title: "Kode Kategori" },
                { field: "LookUpValueName", title: "Nama Kategori" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.LookupCodeTo = data.ParaValue;
                me.data.LookupNameTo = data.LookUpValueName;
                me.Apply();
            }
        });

    }

    me.ModelCodeFrom = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Model",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model" },
                { field: "SalesModelDesc", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ModelCodeFrom = data.SalesModelCode;
                me.data.ModelNameFrom = data.SalesModelDesc;
                me.Apply();
            }
        });

    }

    me.ModelCodeTo = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Model",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model" },
                { field: "SalesModelDesc", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ModelCodeTo = data.SalesModelCode;
                me.data.ModelNameTo = data.SalesModelDesc;
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        $http.post('om.api/ReportSales/ValidatePrintSalesAchievement', me.data)
           .success(function (e) {
               if (e.success) {
                   if (me.kreteria == '0') {
                       var ReportId = 'OmRpSalAch';
                       var par = [
                           moment(me.data.PeriodeFrom).format('YYYYMMDD'),
                           moment(me.data.PeriodeTo).format('YYYYMMDD'),
                           me.data.EmployeeCodeFrom,
                           me.data.EmployeeCodeTo,
                           me.data.ModelCodeFrom,
                           me.data.ModelCodeTo,
                           '0'
                       ]
                       var rparam = 'PERIODE : ' + moment(me.data.PeriodeFrom).format('MM-YYYY') + ' S/D ' + moment(me.data.PeriodeTo).format('MM-YYYY')
                   }
                   else {
                       var ReportId = 'OmRpSalAch';
                       var par = [
                           moment(me.data.PeriodeFrom).format('YYYYMMDD'),
                           moment(me.data.PeriodeTo).format('YYYYMMDD'),
                           me.data.LookupCodeFrom,
                           me.data.LookupCodeTo,
                           me.data.ModelCodeFrom,
                           me.data.ModelCodeTo,
                           '1'
                       ]
                       var rparam = 'PERIODE : ' + moment(me.data.PeriodeFrom).format('MM-YYYY') + ' S/D ' + moment(me.data.PeriodeTo).format('MM-YYYY')
                   }

                   Wx.showPdfReport({
                       id: ReportId,
                       pparam: par.join(','),
                       textprint: true,
                       rparam: rparam,
                       type: "devex"
                   });
               } else {
                   MsgBox(e.message, MSG_ERROR);
                   return;
               }
           })
           .error(function (e) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });

    }

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.data.EmployeeCodeFrom = "";
        me.data.LookupCodeFrom = "";
        me.data.LookupNameFrom = "";
        me.data.EmployeeCodeTo = "";
        me.data.LookupCodeTo = "";
        me.data.LookupNameTo = "";
        me.Apply();
    });

    $("[name = 'isActiveModel']").on('change', function () {
        me.data.isActiveModel = $('#isActiveModel').prop('checked');
        me.data.ModelCodeFrom = "";
        me.data.ModelNameFrom = "";
        me.data.ModelCodeTo = "";
        me.data.ModelNameTo = "";
        me.Apply();
    });

    me.initialize = function () {

        me.kreteria = "0";
        me.data.EmployeeCodeFrom = '';
        me.data.EmployeeCodeTo = '';
        me.data.LookupCodeFrom = '';
        me.data.LookupCodeTo = '';
        me.data.ModelCodeFrom = '';
        me.data.ModelCodeTo = '';
        $('#isActive').prop('checked', false);
        me.data.isActive = false;
        $('#isActiveModel').prop('checked', false);
        me.data.isActiveModel = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.PeriodeFrom = e.DateFrom;
              me.data.PeriodeTo = e.DateTo;
          });
        me.Apply();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Sales Achievement Record",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    {
                         text: "Periode",
                         type: "controls",
                         items: [
                             { name: "PeriodeFrom", cls: "span2", type: "ng-datepicker" },
                             { type: "label", text: "S/D", cls: "span1 mylabel" },
                             { name: "PeriodeTo", cls: "span2", type: "ng-datepicker" },
                         ]
                    },
                    {
                        text: "Kreteria",
                        type: "optionbuttons",
                        model: "kreteria",
                        name: "kreteria",
                        items: [
                            { name: "0", text: "Salesman" },
                            { name: "1", text: "Kelompok AR" },
                        ]
                    },
                    { name: 'isActive', type: 'check', cls: "", text: "Sales/Kategori", float: 'left' },
                    {
                        type: "controls",
                        items: [
                            { name: "EmployeeCodeFrom", cls: "span2", type: "popup", click: "EmployeeCodeFrom()", disable: "data.isActive == false", show: "kreteria == 0" },
                            { name: "LookupCodeFrom", cls: "span2", type: "popup", click: "LookupCodeFrom()", disable: "data.isActive == false", show: "kreteria == 1" },
                            { name: "LookupNameFrom", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        type: "controls",
                        items: [
                            { name: "EmployeeCodeTo", cls: "span2", type: "popup", click: "EmployeeCodeTo()", disable: "data.isActive == false", show: "kreteria == 0" },
                            { name: "LookupCodeTo", cls: "span2", type: "popup", click: "LookupCodeTo()", disable: "data.isActive == false", show: "kreteria == 1" },
                            { name: "LookupNameTo", cls: "span4", readonly: true },
                        ]
                    },
                    { name: 'isActiveModel', type: 'check', cls: "", text: "Model", float: 'left' },
                    {
                        type: "controls",
                        items: [
                            { name: "ModelCodeFrom", cls: "span2", type: "popup", click: "ModelCodeFrom()", disable: "data.isActiveModel == false" },
                            { name: "ModelNameFrom", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        type: "controls",
                        items: [
                            { name: "ModelCodeTo", cls: "span2", type: "popup", click: "ModelCodeTo()", disable: "data.isActiveModel == false" },
                            { name: "ModelNameTo", cls: "span4", readonly: true },
                        ]
                    },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("RptSalesAchievementRecord");
    }



});