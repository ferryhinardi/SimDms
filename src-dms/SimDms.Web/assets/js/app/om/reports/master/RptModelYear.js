"use strict"; //Reportid OmRpMst004
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Status = [
        { "value": '2', "text": 'All' },
        { "value": '0', "text": 'Tidak Aktif' },
        { "value": '1', "text": 'Aktif' }
    ];

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
        if (me.data.SalesModelCode != "") {
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

                    me.data.SalesModelCode,
                    me.data.SalesModelCodeTo,
                    me.data.SalesModelYear,
                    me.data.SalesModelYearTo,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: "OmRpMst004",
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
        title: "Report Model Year",
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
                        { name: 'isActive', type: 'check', cls: 'span2', text: "Sales Model Code", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCode", cls: "span2", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCode()", disable: "data.isActive == false" },
                                { name: "SalesModelDesc", cls: "span4", readonly: true },
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelCodeTo", cls: "span2", type: "popup", btnName: "btnSalesModelCodeTo", click: "SalesModelCodeTo()", disable: "data.isActive == false" },
                                { name: "SalesModelDescTo", cls: "span4", readonly: true },
                            ]
                        },
                        { name: 'isActiveYear', type: 'check', cls: 'span2', text: "Tahun", float: 'left' },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                { name: "SalesModelYear", cls: "span2", type: "select2", datasource: "Year", disable: "data.isActiveYear == false" },
                                { type: "label", text: "S/D", cls: "span1 mylabel" },
                                { name: "SalesModelYearTo", cls: "span2", type: "select2", datasource: "Year", disable: "data.isActiveYear == false" },
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