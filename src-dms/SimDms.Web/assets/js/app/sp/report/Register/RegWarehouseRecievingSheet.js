var report = "SpRpRgs013";
var transcode = "1";
var transcodename = "PEMBELIAN";
var istrans = false;
"use strict"

function RptRegisterWarehouseRecievingSheet($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TTWR').
    success(function (data, status, headers, config) {
       me.comboTransType = data;
       //var tran = document.getElementById('TransCode')
       //tran.remove(0);
   });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('PartType')
        part.options[0].text = 'SELECT ALL';
    });

    me.$watch('TransType', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    $scope.$on('1', function (e) {
        transcode = e.name
        transcodename = "PEMBELIAN";
        me.data.pembelian = false;
    });

    $scope.$on('2', function (e) {
        transcode = e.name
        transcodename = "NON PEMBELIAN";
        me.data.pembelian = true;
    });

    $('#isTransCodeN').on('change', function (e) {
        me.data.trans = true; // tidak
        istrans = false;
        me.Apply();
    });

    $('#isTransCodeY').on('change', function (e) {
        me.data.trans = false; // ya
        istrans = true;
        me.Apply();
    });


    me.printPreview = function () {
        var comboParameter = "";
        var trans = $('#TransCode').select2('val');
        trans = trans == "" ? "ALL" : trans;
        var part = $('#PartType').select2('val');
        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');

        var periode = "PERIODE : " + moment(me.data.FirstPeriod).format('DD-MMMM-YYYY') + " S/D " + moment(me.data.EndPeriod).format('DD-MMMM-YYYY');
        var sTrans = $('#TransCode').select2('val') == "" ? " (ALL)" : $('#TransCode').select2('data').text;
        var comboParameter = part == "" ? "SEMUA" : $('#PartType').select2('data').text;

        if (transcode == "2" && istrans) {
            if ($('#TransCode').select2('val') == "") {
                Wx.alert("Ada informasi yang belum lengkap");
            }
            else
            {
                comboParameter = comboParameter + '(' + sTrans + ')';
            }
        }
        else if (transcode == "2" && !istrans)
        {
            comboParameter = comboParameter + "(ALL)"
        }

        var par = firstPeriod + "," + endPeriod + "," + transcode + "," + part + "," + trans;
        var rparam = comboParameter + "," + periode + "," + transcodename;

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
        me.data.trans = false;
        me.data.pembelian = false;

        me.isPrintAvailable = true;
    }

    me.start();
    me.TransType = "1";
    me.data.trans = false;
    me.data.pembelian = false;
}

$(document).ready(function () {
    var options = {
        title: "Register Warehouse Recieving Sheet",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlRegisterWarehouseRecievingSheet",
                items: [
                   { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },
                   
                   {
                       type: "optionbuttons",
                       name: "tabpage1",
                       model: "TransType",
                       text: "Tipe Transaksi",
                       items: [
                           { name: "1", text: "Pembelian" },
                           { name: "2", text: "Non Pembelian" },
                       ]
                   },
                   {
                       text: "Kode Transaksi",
                       type: "controls",
                       show: "data.pembelian",
                       items: [
                             { name: "isTransCode", text: "Kode Transaksi", cls: "span1 left", type: "switch" },
                             { name: "TransCode", text: "Kode Transaksi", cls: "span4", type: "select2", datasource: "comboTransType", disable: "data.trans" },
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
        SimDms.Angular("RptRegisterWarehouseRecievingSheet");
    }
});