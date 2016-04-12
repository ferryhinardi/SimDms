"use strict"

function svRptDiffYearlySalesController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.default = function () {
        $http.post('sv.api/report/default').
        success(function (data, status, headers, config) {
            $('#Year').select2('val', data.Year);
            me.data.Year = data.Year;
            me.UserId = data.UserId;
        });
    }

    me.printPreview = function () {
        var reportId = me.options == "1" ? "SvRpReport00601" : me.options == "2" ? "SvRpReport00602" : "SvRpReport00603";

        var par = [
            'producttype',
            me.data.Year,
            me.options
        ];

        var rparam = [me.UserId,reportId,reportId];

        console.log(rparam);

        Wx.showPdfReport({
            id: reportId,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    $http.post('sv.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.comboYear = data;
    });

    me.initialize = function () {
        me.default();
    }

    me.options = "1";
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Perbandingan Penjualan Service Bulanan",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlDiffYearlySales",
                items: [
                      { name: "Year", required: true, cls: "span3", text: "Tahun", type: "select2", datasource: "comboYear" },
                      {
                          type: "optionbuttons",
                          name: "tabpageoptions",
                          model: "options",
                          text: "Dicetak Per",
                          items: [
                              { name: "1", text: "Kategori" },
                              { name: "2", text: "Basic Model" },
                              { name: "3", text: "Jenis Pekerjaan" },
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
        SimDms.Angular("svRptDiffYearlySalesController");
    }
});