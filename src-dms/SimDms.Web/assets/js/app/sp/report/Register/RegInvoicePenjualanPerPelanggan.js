var report = "SpRpRgs017B";
var pyCode = "0";
var pyName = "(TUNAI)";

"use strict"

function RptRegisterInvoicePenjualanPerPelanggan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('PartType')
        part.options[0].text = 'SELECT ALL';
    });

    me.$watch('TransCode', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    $scope.$on('0', function () {
        pyCode = "0";
        pyName = "(TUNAI)";
    });

    $scope.$on('1', function () {
        pyCode = "1";
        pyName = "(KREDIT)";
    });

    $scope.$on('2', function () {
        pyCode = "2";
        pyName = "(KREDIT & TUNAI)";
    });

    me.printPreview = function () {
        var part = $('#PartType').select2('val');
        var sPart = "TIPE PART : "
        sPart += part == "" ? "SEMUA" : $('#PartType').select2('data').text;
        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');

        var par = firstPeriod + "," + endPeriod + "," + pyCode + "," + part + "," + me.data.CustomerCodeFrom + ',' + me.data.CustomerCodeTo;
        var periode = "PERIODE : " + moment(me.data.FirstPeriod).format('DD-MMMM-YYYY') + " S/D " + moment(me.data.EndPeriod).format('DD-MMMM-YYYY');
        var rparam = periode + "," + pyName + "," + sPart + "," + me.data.costprice;

        Wx.showPdfReport({
            id: report,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.browseCustomer = function (e) {
        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');

        var lookup = Wx.blookup({
            name: "CustomerCodeBrowse",
            title: "Pelanggan",
            manager: spRptRegisterManager,
            query: new breeze.EntityQuery.from("BrowseCustomerRptRgs")
                .withParameters({ dateFrom: firstPeriod, dateTo: endPeriod, paymentCode: pyCode, partType: $('#PartType').select2('val') }),
            defaultSort: "CustomerCode asc",
            columns: [
               { field: 'CustomerCode', title: 'Customer' },
               { field: 'CustomerName', title: 'Customer Name' },
            ]
        });
        lookup.dblClick(function (data) {
            if (e == 0) {
                me.data.CustomerCodeFrom = data.CustomerCode;
                me.data.CustomerNameFrom = data.CustomerName;
                if (me.data.CustomerCodeTo === undefined) { me.data.CustomerCodeTo = data.CustomerCode; me.data.CustomerNameTo = data.CustomerName };
                if (me.data.CustomerCodeTo < me.data.CustomerCodeFrom) { me.data.CustomerCodeTo = data.CustomerCode };
            }
            else {
                me.data.CustomerCodeTo = data.CustomerCode;
                me.data.CustomerNameTo = data.CustomerName;
                if (me.data.CustomerCodeFrom === undefined) { me.data.CustomerCodeFrom = data.CustomerCode; me.data.CustomerNameFrom = data.CustomerName };
                if (me.data.CustomerCodeTo < me.data.CustomerCodeFrom) { me.data.CustomerCodeFrom = data.CustomerCode; me.data.CustomerNameFrom = data.CustomerName };
            }
            me.save = false;
            me.Apply();
        });
    }

    $('#CostPriceN').on('change', function (e) {
        me.data.costprice = false;
    });

    $('#CostPriceY').on('change', function (e) {
        me.data.costprice = true;
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


    me.initialize = function () {
        me.data.FirstPeriod = me.data.EndPeriod = me.now();
        me.data.costprice = false;
        me.isPrintAvailable = true;
    }

    me.start();
    me.TransCode = "0";
}

$(document).ready(function () {
    var options = {
        title: "Register Faktur Penjualan Per Pelanggan (HQ)",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlFakturPenjualanPerPelanggan",
                items: [
                   { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                    { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                    { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        items: [
                            { name: "CustomerCodeFrom", cls: "span2", placeHolder: "Kode Customer", readonly: true, type: "popup", click: "browseCustomer(0)" },
                            { name: "CustomerNameFrom", cls: "span6", placeHolder: "Nama Customer", readonly: true }
                        ]
                    },
                    {
                        text: "S/D",
                        type: "controls",
                        items: [
                            { name: "CustomerCodeTo", cls: "span2", placeHolder: "Kode Customer", readonly: true, type: "popup", click: "browseCustomer(1)" },
                            { name: "CustomerNameTo", cls: "span6", placeHolder: "Nama Customer", readonly: true }
                        ]
                    },
                    {
                        type: "optionbuttons",
                        name: "tabpage1",
                        model: "TransCode",
                        text: "Kode Transaksi",
                        items: [
                            { name: "0", text: "Tunai" },
                            { name: "1", text: "Kredit" },
                            { name: "2", text: "Kredit & Tunai" },
                        ]
                    },
                    { name: "CostPrice", text: "Harga Pokok", cls: "span2", type: "switch" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterInvoicePenjualanPerPelanggan");
    }
});