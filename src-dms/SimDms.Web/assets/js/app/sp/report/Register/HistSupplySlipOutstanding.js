"use strict"

function RptRegisterHistSupplySlipOutstanding($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/Years').
    success(function (data, status, headers, config) {
       me.comboYear = data;
    });

    $http.post('sp.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    me.default = function () {
       $http.post('sp.api/reportregister/default').
       success(function (data, status, headers, config) {
           $('#Month').select2('val', data.Month);
           $('#Year').select2('val', data.Year);
       });
    }

    me.printPreview = function () {
        var month = $('#Month').select2('val');
        var year = $('#Year').select2('val');
        var par = 'producttype' + "," + month + "," + year;
        var rparam = "PERIODE : " + $('#Month').select2('data').text + " - " + year;

        Wx.showPdfReport({
            id: "SpRpRgs002A",
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
        title: "History Supply Slip Outstanding",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlHistSSOutstanding",
                items: [
                  { name: "Month", required: true, cls: "span4", text: "Bulan", type: "select2", datasource: "comboMonth" },
                  { name: "Year", required: true, cls: "span4", text: "-", type:"select2", datasource: "comboYear" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterHistSupplySlipOutstanding");
    }
});