var codeID = "";
var salesType = "";
"use strict"

function RptRegisterNonSalesInvoiceAttachments($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
            success(function (data, status, headers, config) {
                me.comboPartType = data;
                var part = document.getElementById('PartType')
                part.options[0].text = 'SELECT ALL';
            });

    me.$watch('TransType', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    $scope.$on('Service', function () {
        codeID = "TTSR";
        salesType = "2";
        $http.post('sp.api/Combo/LoadComboData?CodeId=TTSR').
             success(function (data, status, headers, config) {
                 me.comboTransType = data;
                 var tran = document.getElementById('TransType')
                 tran.options[0].text = 'SELECT ALL';
             });
    });

    $scope.$on('UnitOrder', function () {
        codeID = "TTSL";
        salesType = "3";
        $http.post('sp.api/Combo/LoadComboData?CodeId=TTSL').
         success(function (data, status, headers, config) {
             me.comboTransType = data;
             var part = document.getElementById('PartType')
             part.options[0].text = 'SELECT ALL';
         });
    });

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
        var trans = $('#TransType').select2('val') == "" ? "ALL" : $('#TransType').select2('val');
        var part = $('#PartType').select2('val');
        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');
        var par = firstPeriod + "," + endPeriod + "," + trans + "," + codeID + "," + salesType + "," + part;

        var sTrans = trans == "" ? "SEMUA TRANSAKSI" : $('#TransType').select2('data').text;
        var sPart = part == "" ? "SEMUA" : $('#PartType').select2('data').text;
        var speriode = trans == "" ? "SELURUH TRANSAKSI PERIODE : " : sTrans + " TRANSAKSI PERIODE : ";
        var periode = speriode + moment(me.data.FirstPeriod).format('DD-MMMM-YYYY') + " S/D " + moment(me.data.EndPeriod).format('DD-MMMM-YYYY');
        var rparam = periode + "," + sPart + "," + sTrans;

        Wx.showPdfReport({
            id: "SpRpRgs003",
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.isPrintAvailable = true;
        me.data.FirstPeriod = me.data.EndPeriod = me.now();
    }

    me.start();
    me.$broadcast('Service');
    me.TransType = 'Service';
}

$(document).ready(function () {
    var options = {
        title: "Register Lampiran Faktur Non Penjualan",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlRegNonSalesInvoiceAttchment",
                title: "",
                items: [
                      {
                          type: "optionbuttons",
                          name: "tabpage1",
                          model: "TransType",
                          text: "Tipe Transaksi",
                          items: [
                              { name: "Service", text: "Service"},
                              { name: "UnitOrder", text: "Unit Order" },
                          ]
                      },
                      { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                      { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                      { name: "TransType", text: "Tipe Transaksi", cls: "span4 full ", type: "select2", datasource: "comboTransType" },
                      { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterNonSalesInvoiceAttachments");
    }
});