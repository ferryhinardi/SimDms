var report = "SpRpRgs004";
var code = "Y";
"use strict"

function RptRegisterPenjualanTunaiKredit($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TTPJ').
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

    me.$watch('Code', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    $scope.$on('Show', function () {
        code = "Y";
    });

    $scope.$on('NotShow', function () {
        code = "N";
    });

    me.printPreview = function () {
        var trans = $('#TransType').select2('val');
        var sTrans = trans == "" ? "SEMUA" : $('#TransType').select2('data').text;

        trans = trans == "" ? "%" : trans;

        var part = $('#PartType').select2('val');
        var sPart = part == "" ? "SEMUA" : $('#PartType').select2('data').text;

        part = part == "" ? "%%" : "%" + part + "%";

        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');
        var par = firstPeriod + "," + endPeriod + "," + trans + "," + part + "," + code;
        var periode = "PERIODE : " + moment(me.data.FirstPeriod).format('DD-MMMM-YYYY') + " S/D " + moment(me.data.EndPeriod).format('DD-MMMM-YYYY');
        var rparam = sTrans + "," + sPart + "," + periode;
    
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
    me.Code = "Show";
}

$(document).ready(function () {
    var options = {
        title: "Register Penjualan (Tunai & Kredit)",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlRegisterPenjualanTunaiKredit",
                items: [
                     { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "TransType", text: "Tipe Transaksi", cls: "span4 full ", type: "select2", datasource: "comboTransType" },
                   { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },
                   {
                       type: "optionbuttons",
                       name: "tabpage1",
                       model: "Code",
                       text: "Kode Pelanggan",
                       items: [
                           { name: "Show", text: "Tampilkan" },
                           { name: "NotShow", text: "Tidak" },
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
        SimDms.Angular("RptRegisterPenjualanTunaiKredit");
    }
});