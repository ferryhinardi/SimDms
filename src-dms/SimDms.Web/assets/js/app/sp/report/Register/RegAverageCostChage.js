var report = "SpRpRgs029";
"use strict"

function RptRegisterAverageCostChage($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('PartType')
        part.options[0].text = 'SELECT ALL';
    });

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
            me.data.Month = data.Month;
            me.data.Year = data.Year;
        });
    }

    $('#isPeriodN').on('change', function (e) {
        me.data.period = true;
        me.Apply();
    });

    $('#isPeriodY').on('change', function (e) {
        me.data.period = false;
        me.Apply();
    });

    $('#isPartTypeN').on('change', function (e) {
        me.data.parttype = true;
        me.Apply();
    });

    $('#isPartTypeY').on('change', function (e) {
        me.data.parttype = false;
        me.Apply();
    });

    me.printPreview = function () {
        var month = !me.data.period ? me.data.Month : 0;
        var smonth = $('#Month').select2('data').text;
        var year = $('#Year').select2('val');
        var periode = !me.data.period ? smonth.substring(0,3) + " " + year : "ALL";
        var part = !me.data.parttype ? $('#PartType').select2('val') : "%";
        
        var par = month + "," + year + "," + periode + "," + part;
        var rparam = periode + "," + year;

        Wx.showPdfReport({
            id: report,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.isPrintAvailable = true;
        me.data.period = true;
        me.data.parttype = true;
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Register Perubahan Average Cost",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            
            {
                name: "pnlRegisterAverageCostChange",
                items: [
                    { name: "isPeriod", text: "Periode", cls: "span2 left full", type: "switch" },
                    { name: "Month", required: true, cls: "span4", text: "Bulan", type: "select2", datasource: "comboMonth", disable: "data.period" },
                    { name: "Year", required: true, cls: "span4", text: "-", type: "select2", datasource: "comboYear", disable: "data.period" },
                    { name: "isPartType", text: "Tipe Part", cls: "span2 left", type: "switch" },
                    { name: "PartType", text: "Tipe Part", cls: "span4 full ", type: "select2", datasource: "comboPartType", disable: "data.parttype" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("RptRegisterAverageCostChage");
    }
});