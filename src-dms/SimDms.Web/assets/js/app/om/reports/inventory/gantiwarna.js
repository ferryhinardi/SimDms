"use strict"

function omReportGantiWarnaController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/Combo/Status').
         success(function (data) {
             me.comboStatus = data;
             var tran = document.getElementById('Status')
             tran.options[0].text = 'ALL';
         });

    me.printPreview = function () {
        var status = $('#Status').select2('val');
        status = status == "" ? "5" : status == 5 ? "4" : status;
        var ReportId = "OmRpInvRgs007";

        var par = [
            me.data.DateFrom,
            me.data.DateTo,
            status,
        ]
       
        var rparam = [
            $('#Status').select2('data').text,
            moment(me.data.DateFrom).format('DD-MMM-YYYY'),
            moment(me.data.DateTo).format('DD-MMM-YYYY'),
        ]

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.default = function () {
        $http.post('om.api/reportinventory/Transfer').
          success(function (e) {
              me.data.DateFrom = e.DateFrom;
              me.data.DateTo = e.DateTo;
          });
        me.Apply();
    }

    me.initialize = function () {
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Ganti Warna",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlGantiWarna",
                items: [
                 { name: "Status", text: "Status", cls: "span4 full ", type: "select2", datasource: "comboStatus" },
                 { name: "DateFrom", text: "Date", cls: "span4", type: "ng-datepicker" },
                 { name: "DateTo", text: "S/D", cls: "span4", type: "ng-datepicker" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("omReportGantiWarnaController");
    }
});