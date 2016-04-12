var report = "SpRpTrn032";
"use strict"

function RegPendingDocument($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.default = function () {
        $http.post('sp.api/reportregister/PendingDocumentdefault').
          success(function (e) {
              if (e.success) {
                  me.data = e;
              }
              else {
                  Wx.alert(e.message);
              }
          });
    }

    me.printPreview = function () {
        var periodBeg = moment(me.data.PeriodBeg).format('YYYYMMDD');
        var periodEnd = moment(me.data.PeriodEnd).format('YYYYMMDD');

        var par = periodBeg + ',' + periodEnd;
        var rparam = me.data.PeriodName

        Wx.showPdfReport({
            id: report,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.isPrintAvailable = true;
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Register Dokument Pending",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlPendingDocument",
                items: [
                    { name: "FiscalYear", text: "Tahun Fiskal", cls: "span4", readonly: true },
                    { name: "FiscalMonth", text: "Bulan Fiskal", cls: "span4", readonly: true },
                    { name: "FiscalPeriod", text: "Periode", cls: "span4", readonly: true },
                    { name: "PeriodName", text: "Nama Periode", cls: "span4", readonly: true },
                    { name: "PeriodBeg", cls: "hide", readonly: true },
                    { name: "PeriodEnd", cls: "hide", readonly: true },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RegPendingDocument");
    }
});