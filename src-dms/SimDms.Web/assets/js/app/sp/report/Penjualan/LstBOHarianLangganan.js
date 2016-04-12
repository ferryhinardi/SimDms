"use strict";
function LstBOHarianLangganan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Trans = [
        { value: '%', text: 'SEMUA' },
        { value: '0', text: 'SGP' },
        { value: '1', text: 'SGO' },
        { value: '2', text: 'SGA' },
        { value: '3', text: 'NON SGP' },
        { value: '4', text: 'NON SGA' },
        { value: '5', text: 'OTHERS' },
    ];


    me.printPreview = function () {
        me.savemodel = angular.copy(me.data);
        $http.post('sp.api/reportpenerimaan/Convert', me.savemodel).
            success(function (data, status, headers, config) {
                if (data.success) {
                    var prm = data.date + "," + me.data.PartType;
                    var pprtipe = "";
                    switch (me.data.PartType) {
                        case "%": pprtipe = "SEMUA";
                            break;
                        case "0": pprtipe = "SGP";
                            break;
                        case "1": pprtipe = "SGO";
                            break;
                        case "2": pprtipe = "SGA";
                            break;
                        case "3": pprtipe = "NON SGP";
                            break;
                        case "4": pprtipe = "NON SGA";
                            break;
                        case "5": pprtipe = "OTHERS";
                            break;
                    }
                    Wx.showPdfReport({
                        id: "SpRpTrn034",
                        pparam: prm,
                        rparam: data.date + ",TIPE PART : " + pprtipe,
                        type: "devex"
                    });
                } else {
                    return false;
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });

    }

    me.initialize = function () {
        me.data = {};

        me.isPrintAvailable = true;

    }


    me.start();

}
$(document).ready(function () {
    var options = {
        title: "Daily Back Order Customer List",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "data.DocPeriod == undefined" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "DocPeriod",
                        text: "Periode Dokumen",
                        cls: "span5",
                        type: "ng-datepicker"
                    },
                    {
                        name: "PartType",
                        cls: "span5 full",
                        type: "select2",
                        text: "Tipe Part",
                        datasource: "Trans"
                    },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {


        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("LstBOHarianLangganan");

    }



});