var report = "SpRpRgs024";
"use strict"

function RptRegisterBonPengeluaranSparePartOutstanding($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('PartType')
        part.options[0].text = 'SELECT ALL';
    });

    me.$watch('ReportType', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    $scope.$on('Summary', function () {
        report = "SpRpRgs024";
    });

    $scope.$on('Detail', function () {
        report = "SpRpRgs025";
    });


    me.printPreview = function () {
        var part = $('#PartType').select2('val');
        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');
        var par = firstPeriod + "," + endPeriod + "," + part;
        var periode = "PERIODE : " + moment(me.data.FirstPeriod).format('DD-MMMM-YYYY') + " S/D " + moment(me.data.EndPeriod).format('DD-MMMM-YYYY');
        var rparam = periode;
        Wx.showPdfReport({
            id: report,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

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


    me.initialize = function () {
        me.data.FirstPeriod = me.data.EndPeriod = me.now();
        me.isPrintAvailable = true;
    }

    me.start();
    me.ReportType = "Summary";
}

$(document).ready(function () {
    var options = {
        title: "Register Bon Pengeluaran Sparepart Outstanding",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlBonPengeluaranSparepartOut",
                items: [
                    { name: "TransCode", text: "Kode Transaksi", cls: "span4 full", value: "Transfer Stock", disable:true },
                    { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                    { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                    { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },
                    {
                        type: "optionbuttons",
                        name: "tabpage1",
                        model: "ReportType",
                        text: "Report Type",
                        items: [
                            { name: "Summary", text: "Summary" },
                            { name: "Detail", text: "Detail" },
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
        SimDms.Angular("RptRegisterBonPengeluaranSparePartOutstanding");
    }
});