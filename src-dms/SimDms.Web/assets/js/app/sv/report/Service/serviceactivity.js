"use strict"

function svRptSrvServiceActivityController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/Combo/Years').
     success(function (data, status, headers, config) {
         me.comboYear = data;
     });

    $http.post('sv.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    me.$watch('options', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    me.$watch('data.IsBranch', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            if (newValue) {
                $('#btnBranchCodeFrom, #btnBranchCodeTo').prop('disabled', false);
            }
            else {
                $('#btnBranchCodeFrom, #btnBranchCodeTo').prop('disabled', true);
                $('#BranchCodeFrom, #BranchCodeTo').val('');
                me.data.BranchCodeFrom = '';
                me.data.BranchCodeTo = '';
            }
        }
    });

    me.browseBranch = function (e) {
        var lookup = Wx.blookup({
            name: "BranchCodeBrowse",
            title: "Cabang",
            manager: ReportService,
            query: "BrowseBranch",
            defaultSort: "BranchCode asc",
            columns: [
               { field: 'BranchCode', title: 'Kode Cabang' },
               { field: 'CompanyName', title: 'Nama Cabang' },
            ]
        });
        lookup.dblClick(function (data) {
            if (e == 0) {
                me.data.BranchCodeFrom = data.BranchCode;
                me.data.BranchCodeFromDesc = data.CompanyName;
                me.data.AddressFrom1 = data.Address1;
                me.data.AddressFrom2 = data.Address2;
                if (me.data.BranchCodeTo === undefined) { me.data.BranchCodeTo = data.BranchCode };
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeTo = data.BranchCode };
            }
            else {
                me.data.BranchCodeTo = data.BranchCode;
                me.data.BranchCodeToDesc = data.CompanyName;
                me.data.AddressTo1 = data.Address1;
                me.data.AddressTo2 = data.Address2;
                if (me.data.BranchCodeFrom === undefined) { me.data.BranchCodeFrom = data.BranchCode };
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeFrom = data.BranchCode };
            }

            me.save = false;
            me.Apply();
        });
    }

    me.printPreview = function () {
        var par = {
            branchFrom: me.data.BranchCodeFrom,
            branchTo: me.data.BranchCodeTo,
            month: me.data.Month,
            year: me.data.Year,
            options: me.options
        };

        Wx.XlsxReport({
            url: 'sv.api/report/activityservice',
            type: 'xlsx',
            params: par
        });

    }

    me.default = function () {
        $http.post('sv.api/report/default').
          success(function (e) {
              me.data.IsHolding = e.IsHolding;
              $('#Month').select2('val', e.Month);
              $('#Year').select2('val', e.Year);
              me.data.Month = e.Month;
              me.data.Year = e.Year
          });
    }

    me.initialize = function () {
        me.data.Period = me.now();
        me.default();
        me.data.BranchCodeFrom = '';
        me.data.BranchCodeTo = '';
    }

    me.options = "0";
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Aktivitas Service By Mekanik & Unit",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlServiceActivity",
                items: [
                   { name: "Month", cls: "span4", text: "Bulan", type: "select2", datasource: "comboMonth" },
                   { name: "Year", required: true, cls: "span4", text: "-", type: "select2", datasource: "comboYear" },
                   { name: "IsBranch", text: "Kode Branch", cls: "span2", type: "ng-switch", style: "margin-bottom:15px" },
                   { name: "BranchCodeFrom", placeHolder: "", cls: "span3", type: "popup", click: "browseBranch(0)", disable: true },
                   { name: "BranchCodeTo", text: "S/D", placeHolder: " ", cls: "span3", type: "popup", click: "browseBranch(1)", disable: true },
                   {
                       type: "optionbuttons",
                       name: "tabpageoptions",
                       model: "options",
                       text: "Parameter",
                       items: [
                           { name: "0", text: "Unit" },
                           { name: "1", text: "Nilai Rupiah" },
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
        SimDms.Angular("svRptSrvServiceActivityController");
    }
});