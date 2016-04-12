var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterSalesTargetController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.Month = data;
    });

    $http.post('om.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    me.OutLetCode = function () {
        var lookup = Wx.blookup({
            name: "OutLetLookup",
            title: "OutLet",
            manager: spSalesManager,
            query: "OutLetLookup",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Branch Code" },
                { field: "BranchName", title: "Branch Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.OutletCode = data.BranchCode;
                me.data.BranchName = data.BranchName;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    $("[name = 'isActive']").on('change', function () {
        me.data.isActive = $('#isActive').prop('checked');
        me.Apply();
    });

    me.Print = function () {
        var ReportId = 'OmRpMst012';
        var par = [
           'companycode', me.data.OutletCode, me.data.Year, me.data.Month
        ]
        var rparam = 'PERIODE : ' + moment(Date.now()).format('DD-MMMM-YYYY');

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        var date = new Date;
        $('#isActive').prop('checked', true);
        me.data.isActive = true;
        me.data.Month = date.getMonth() + 1;
        me.data.Year = date.getFullYear();
        $http.get('breeze/sales/CurrentUserInfo').
        success(function (dl, status, headers, config) {
            me.data.DealerCode = dl.CompanyCode;
            me.data.OutletCode = dl.BranchCode;
            me.data.CompanyName = dl.CompanyGovName;
            me.data.BranchName = dl.CompanyName;

        });
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Print Sales Target",
        xtype: "panels",
        panels: [
            {
                name: "SalesTarget",
                title: "Filter",
                items: [
                        {
                            text: "OutLet",
                            type: "controls",
                            required: true,
                            items: [
                                { name: 'isActive', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "OutletCode", cls: "span2", placeHolder: "DealerCode", type: "popup", btnName: "btnDealerCode", click: "DealerCode()", disable: "data.isActive == false" },
                                { name: "BranchName ", cls: "span4", placeHolder: "CompanyName", model: "data.CompanyName", readonly: true },
                            ]
                        },
                        {
                            text: "Periode",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "Month", placeHolder: "Month", cls: "span2", type: "select2", datasource: "Month", opt_text: "MONTH?" },
                                { name: "Year", placeHolder: "Year", cls: "span2", type: "select2", datasource: "Year", opt_text: "YEAR?" },
                            ]
                        },

                        {
                            type: "buttons", cls: "span2", items: [
                                { name: "btnPrint", text: "Print", icon: "icon-search", click: "Print()", cls: "btn btn-primary", icon: "icon-print" },
                            ]
                        },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterSalesTargetController");
    }

});