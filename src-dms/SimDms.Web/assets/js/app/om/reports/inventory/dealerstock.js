"use strict"

function omReportDealerStockController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.printPreview = function () {
        var ReportId = "OmRpStock005";

        $http.post('om.api/reportinventory/default').
          success(function (e) {
              var rparam = ["Periode : " + moment(me.data.Date).format('DD-MMM-YYYY'),
                           e.CompanyCode + " - " + e.CompanyName
              ]

              Wx.showPdfReport({
                  id: ReportId,
                  pparam: moment(me.data.Date).format('DD-MMM-YYYY'),
                  rparam: rparam,
                  type: "devex"
              });
          });
    }
    
    me.initialize = function () {
        me.data.Date = me.now();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Dealer Stock",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlDealerStock",
                items: [
                  { name: "Date", text: "Date", cls: "span4", type: "ng-datepicker" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omReportDealerStockController");
    }
});