"use strict"
var IsPeriod = false;
var IsBranch = false;

function svReportSrvPDIFSCDealerClaimController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $('#FirstPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod > endPeriod) { $('#EndPeriod').val(firstPeriod) }
    });

    $('#EndPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod < endPeriod) { $('#FirstPeriod').val(endPeriod) }
    });

    me.$watch('data.IsPeriod', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            if (newValue) {
                $('#FirstPeriod, #EndPeriod').prop('disabled', false);
            }
            else {
                $('#FirstPeriod, #EndPeriod').prop('disabled', true);
            }
        }
        IsPeriod = newValue;
    });

    me.$watch('data.IsBranch', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            if (newValue) {
                $('#btnBranchCodeFrom, #btnBranchCodeTo').prop('disabled', false);
            }
            else {
                $('#btnBranchCodeFrom, #btnBranchCodeTo').prop('disabled', true);
                $('#BranchCodeFrom, #BranchCodeTo').val('');
            }
        }
        IsBranch = newValue;
    });

    me.printPreview = function () {
        var ReportId = "SvRpReport028";
        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');

        var par = [
            'nocode',
            IsBranch ? me.data.BranchCodeTo : "",
            IsBranch ? me.data.BranchCodeFrom : "",
            IsPeriod ? firstPeriod : "",
            IsPeriod ? endPeriod : "",
            0
        ];

        var rparam = [me.data.UserId, "PDI & FSC DEALER CLAIM"];
        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.browseBranch = function (e) {
        var lookup = Wx.blookup({
            name: "BranchCodeBrowse",
            title: "Cabang",
            manager: ReportService,
            query: "BrowseBranch",
            defaultSort: "BranchCode asc",
            columns: [
               { field: 'BranchCode', title: 'Kode Cabang' },
               { field: 'CompanyName', title: 'Nama Cabang' }
            ]
        });
        lookup.dblClick(function (data) {
            if (e == 0) {
                me.data.BranchCodeFrom = data.BranchCode;
                if (me.data.BranchCodeTo === undefined) { me.data.BranchCodeTo = data.BranchCode };
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeTo = data.BranchCode };
            }
            else {
                me.data.BranchCodeTo = data.BranchCode;
                if (me.data.BranchCodeFrom === undefined) { me.data.BranchCodeFrom = data.BranchCode };
                if (me.data.BranchCodeTo < me.data.BranchCodeFrom) { me.data.BranchCodeFrom = data.BranchCode };
            }

            me.save = false;
            me.Apply();
        });
    }

    me.default = function () {
        $http.post('sv.api/report/default').
          success(function (e) {
              me.data.UserId = e.UserId;
          });
    }

    me.initialize = function () {
        me.default();
        var ym = me.now("YYYY-MM") + "-01";
        me.data.FirstPeriod = moment(ym);
        me.data.EndPeriod = moment(ym).add("months", 1).add("days", -1);
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "PDI & FSC Dealer Claim",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            
            {
                name: "pnlA",
                items: [
                   { name: "IsPeriod", text: "Period", cls: "span2", type: "x-switch" },
                   { name: "FirstPeriod", cls: "span3", type: "ng-datepicker", disable: true },
                   { name: "EndPeriod", text: "S/D", cls: "span3", type: "ng-datepicker", disable: true },
                   { name: "IsBranch", text: "Kode Branch", cls: "span2", type: "ng-switch", style: "margin-bottom:15px" },
                   { name: "BranchCodeFrom", placeHolder: "", cls: "span3", type: "popup", click: "browseBranch(0)", disable: true },
                   { name: "BranchCodeTo", text: "S/D", placeHolder: " ", cls: "span3", type: "popup", click: "browseBranch(1)",disable:true },

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svReportSrvPDIFSCDealerClaimController");
    }
});