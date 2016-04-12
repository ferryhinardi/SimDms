"use strict"
var IsPeriod = false;
var IsBranch = false;

function svReportSrvFSCCampListController($scope, $http, $injector) {

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
   
    me.printPreview = function () {

        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod > endPeriod) {
            Wx.alert('Tanggal dari tidak boleh lebih besar dengan tanggal sampai !');
        }
        else {

            var ReportId = "SvRpReport033";
            var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
            var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');

            var par = [
            'producttype',
            firstPeriod,
            endPeriod
            ];

            var rparam = firstPeriod + " s/d " + endPeriod;
            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                rparam: rparam,
                type: "devex"
            });
        }
    }

    me.default = function () {
        $http.post('sv.api/report/FSCCamp').
          success(function (e) {
              me.data.FirstPeriod = e.FirstPeriod;
              me.data.EndPeriod = e.EndPeriod;
              if (!e.Enable) {
                  $('#FirstPeriod, #EndPeriod').prop('disabled', false);
              }
              else {
                  $('#FirstPeriod, #EndPeriod').prop('disabled', true);
              }
          });
    }

    me.initialize = function () {
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "FSC Campaign List",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [

            {
                name: "pnlA",
                items: [
                   { name: "FirstPeriod",text: "Periode", cls: "span3", type: "ng-datepicker", disable: true },
                   { name: "EndPeriod", text: "S/D", cls: "span3", type: "ng-datepicker", disable: true },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svReportSrvFSCCampListController");
    }
});