"use strict"; //Reportid OmRpSalesRgs001
function RptRegisterHarianPenerbitan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.printPreview = function () {

        var ReportId = 'OmRpSalRgs017Web';
        var par = [
                       moment(me.data.PeriodeFrom).format('YYYYMMDD'),
                       moment(me.data.PeriodeTo).format('YYYYMMDD')
        ]
        var rparam = 'PERIODE : ' + moment(me.data.PeriodeFrom).format('DD-MMM-YYYY') + ' S/D ' + moment(me.data.PeriodeTo).format('DD-MMM-YYYY');

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            textprint: true,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        $http.get('breeze/sales/CurrentUserInfo').
              success(function (dl, status, headers, config) {
                  me.data.CompanyCode = dl.CompanyCode;
                  me.data.BranchCode = dl.BranchCode;
              });
        $http.get('breeze/sales/ProfitCenter').
        success(function (dl, status, headers, config) {
            me.data.ProfitCenterCode = dl.ProfitCenter;
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
        title: "Report Register Harian Penerbitan Faktur Polis",
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
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("RptRegisterHarianPenerbitan");
    }



});