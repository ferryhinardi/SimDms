var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


"use strict";
function RincianVehHilang($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.PeriodeBrowse = function () {
        var lookup = Wx.blookup({
            name: "btnPeriode",
            title: "Periode Lookup",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("RincianVehHilang").withParameters({ "year": me.data.FiscalYear }),
            defaultSort: "PeriodeNum asc",
            columns: [
                { field: "Periode", title: "No. Stok Taking" },
                { field: "FromDate", title: "Dari Tanggal", type: "date", format: "{0:dd-MMM-yyyy}" },
                { field: "EndDate", title: "Sampai Tanggal", type: "date", format: "{0:dd-MMM-yyyy}" },
                { field: "Status", title: "Status" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.Periode = data.Periode;
                me.data.PeriodeDesc = "Dari Tanggal " + moment(data.FromDate).format("DD-MMM-YYYY") + " s.d Tanggal " + moment(data.EndDate).format("DD-MMM-YYYY");
                me.data.PMonth = moment(data.FromDate).format("MM");
                me.data.PYear = moment(data.FromDate).format("YYYY");
                me.data.PMonthName = data.PeriodeName;
                me.Apply();
            }
        });
    }

    me.printPreview = function () {
        if (me.data.FiscalYear == "" || me.data.FiscalYear  == undefined) {
            MsgBox("Tahun Fiskal harus terisi terlebih dahulu!!!", MSG_ERROR); return false;
        }

        if (me.data.Periode == "" || me.data.Periode == null) {
            MsgBox("Periode harus terisi terlebih dahulu!!!", MSG_ERROR); return false;
        }
        var data = me.data.PMonth + "," + me.data.PYear;
        var rparam = "PERIODE:" + me.data.PMonthName;

        Wx.showPdfReport({
            id: "OmRpStock003",
            pparam: data,
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.FiscalYear = moment().format("YYYY");
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Daftar Rincian Kendaraan Hilang",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "PMonth",
                        show: "data.a == 1"
                    },
                    {
                        name: "PMonthName",
                        show: "data.a == 1"
                    },
                    {
                        name: "PYear",
                        show: "data.a == 1"
                    },
                    {
                        name: "FiscalYear",
                        cls: "span3",
                        text: "Tahun Fiskal",
                        validasi: "required",
                        type: "numeric",
                        maxlength: 4
                    },
                    {
                        text: "Periode",
                        type: "controls",
                        cls: "span8",
                        items: [
                            {
                                name: "Periode",
                                click: "PeriodeBrowse()",
                                cls: "span2",
                                type: "popup",
                                text: "Periode",
                                btnName: "btnPeriode",
                                validasi: "required",
                                readonly: true
                            },
                            {
                                name: "PeriodeDesc",
                                cls: "span6",
                                text: "Periode Description",
                                readonly: true,
                                disable: "data.isColor == false"
                            }
                        ]
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("RincianVehHilang");
    }
});