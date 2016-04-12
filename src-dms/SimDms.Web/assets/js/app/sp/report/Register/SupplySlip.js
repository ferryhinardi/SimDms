var report = '';

"use strict";
function RptRegisterSupplySlip($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TTSR').
    success(function (data, status, headers, config) {
        me.comboTransType = data;
        var tran = document.getElementById('TransType')
        tran.options[0].text = 'SELECT ALL';
    });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('PartType')
        part.options[0].text = 'SELECT ALL';
    });

    $http.post('sp.api/reportregister/SupplySlip').
        success(function (e) {
            $('#SSReportStatus').html(e.status);
            report = e.report;
        });

    me.printPreview = function () {
        var trans = $('#TransType').select2('val');
        var part = $('#PartType').select2('val');
        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');
        var par = firstPeriod + "," + endPeriod + "," + trans + "," + part;

        var periode = "PERIODE : " + moment(me.data.FirstPeriod).format('DD-MMMM-YYYY') + " S/D " + moment(me.data.EndPeriod).format('DD-MMMM-YYYY');
        var sTrans = "TIPE TRANSAKSI : ";
        var sPart = "TIPE PART : ";
        sTrans += $('#TransType').select2('val') == "" ? "SEMUA" : $('#TransType').select2('data').text;
        sPart += $('#PartType').select2('val') == "" ? "SEMUA" : $('#PartType').select2('data').text;
        var rparam = periode + "," + sPart + "," + sTrans;

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

        $('#SSReportStatus').html("");
        $('#SSReportStatus').css(
        {
            "font-size": "28px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "right"
        });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Supply Slip",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()"},
        ],
        panels: [
             {
                 name: "pnlStatus",
                 items: [
                     { name: "SSReportStatus", text: "", cls: "span4", readonly: true, type: "label" },
                 ]
             },
            {
                name: "pnlSupplySlip",
                items: [
                   { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "TransType", text: "Tipe Transaksi", cls: "span4 full ", type: "select2", datasource: "comboTransType" },
                   { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterSupplySlip");

    }
});