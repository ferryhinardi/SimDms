var report = "SpRpRgs021"
"use strict"

function RptRegisterHPPSparepart($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('PartType')
        part.options[0].text = 'SELECT ALL';
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

    me.browseSupplier = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "Supplier",
            manager: spRptRegisterManager,
            query: "BrowseSupplier",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: 'SupplierCode', title: 'Kode Pemasok' },
                { field: 'SupplierName', title: 'Nama Pemasok' },
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            me.save = false;
            me.Apply();
        });
    }

    me.printPreview = function () {
        var part = $('#PartType').select2('val');
        part = part == "" ? "%" : part;
        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');
        var supplierCode = me.data.SupplierCode;
        supplierCode = supplierCode == undefined ? "%" : supplierCode

        var par = firstPeriod + "," + endPeriod + "," + part+ "," + supplierCode;

        var periode = "PERIODE : " + moment(me.data.FirstPeriod).format('DD-MMMM-YYYY') + " S/D " + moment(me.data.EndPeriod).format('DD-MMMM-YYYY');
        var sPart = "TIPE PART : ";
        sPart += $('#PartType').select2('val') == "" ? "SEMUA" : $('#PartType').select2('data').text;
        var rparam = periode + "," + sPart;

        Wx.showPdfReport({
            id: report,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data.FirstPeriod = me.data.EndPeriod = me.now();
        me.isPrintAvailable = true;
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Register HPP Sparepart",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlRegisterHPPSparepart",
                items: [
                    { name: "FirstPeriod", text: "Periode", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker", validasi: "required" },
                   {
                       text: "Pemasok",
                       type: "controls",
                       items: [
                           { name: "SupplierCode", cls: "span2", placeHolder: "Kode Pemasok", readonly: true, type: "popup",click: "browseSupplier()", },
                           { name: "SupplierName", cls: "span6", placeHolder: "Nama Pemasok", readonly: true }
                       ]
                   },
                   { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterHPPSparepart");
    }
});