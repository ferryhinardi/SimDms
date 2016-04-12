var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spProsesTutupBulanController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.loadDetail = function()
    {
        $http.post('sp.api/TutupPeriod/ReloadAll', me.savemodel).
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    me.data.Year = dl.YearClosed;
                    me.data.Month = dl.MonthClosed;
                    me.data.Periode = dl.Periode;
                    me.data.PeriodName = dl.NmPeriode;
                    //me.refreshGrid(dl.dataGrid);
                } else {
                    MsgBox("Periode Fiscal belum diseting di Master Company", MSG_ERROR);
                    me.data.btnCls = "0";
                }
            }).
            error(function (e, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.LoadGrid = function () {
        $http.post('sp.api/TutupPeriod/LoadGrid', me.savemodel).
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    me.refreshGrid(dl.record);
                } else {
                    MsgBox(dl.message, MSG_ERROR);
                    me.data.btnCls = "0";
                }
            }).
            error(function (e, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.printPreview = function () {
        $http.post('sp.api/TutupPeriod/Print', me.savemodel).
            success(function (data, status, headers, config) {
                if (data) {
                    
                    var data = data.PeriodBeg + "," + data.PeriodEnd;
                    var rparam = "admin";
					
					Wx.showPdfReport({
						id: "SpRpTrn032",
						pparam: data,
						rparam: rparam,
						type: "devex"
					});
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.ClosePeriod = function () {
        me.savemodel = angular.copy(me.data);
        MsgConfirm("Apakah anda sudah membackup data transaksi ?", function (result) {
            if (result) {
                MsgConfirm("Apakah anda yakin?", function (op) {
                    if (op) {
                        $http.post('sp.api/TutupPeriod/ClosePeriod', me.savemodel).
                        success(function (dl, status, headers, config) {
                        if (dl.success) {
                            MsgBox(dl.message);
                            $('#btnClose').attr('disabled', true);
                            $http.post('sp.api/TutupPeriod/ReloadAll', me.savemodel).
                               success(function (dl, status, headers, config) {
                                   if (dl.success) {
                                       me.data.Year = dl.YearClosed;
                                       me.data.Month = dl.MonthClosed;
                                       me.data.Periode = dl.Periode;
                                       me.data.PeriodName = dl.NmPeriode;
                                   }
                               });
                        } else {
                            MsgBox(dl.message, MSG_ERROR);
                            me.LoadGrid();
                            $('#btnPrintPreview').show();
                        }
                        }).
                        error(function (e, status, headers, config) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                            $('#btnPrintPreview').hide();
                });
                    }
                });
            }
        });
    }


    me.initialize = function()
    {
        me.data = {};
        me.loadDetail();
        $('#btnPrintPreview').hide();
    }

    me.refreshGrid = function (result) {
        Wx.kgrid({
            data: result,
            scrollable: true,
            name: "wXgrid1",
            serverBinding: false,
            resizable: true,
            columns: [
                    { field: "DocumentNo", title: "Document No", width: 50 },
                    { field: "Status", title: "Status", width: 100 },
                    { field: "TableName", title: "Table Name", width: 50 },
                    { field: "ProfitCenter", title: "Profit Center", width: 25 },
                    { field: "TipePart", title: "Tipe Part", width: 25 }
                ]
        });
    }

    me.cancelOrClose = function () {
        result = [];
        me.refreshGrid(result);
    }

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Proses Tutup Bulan",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnNew", text: "Clear", cls: "btn btn-primary", icon: "", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                        {
                            name: "btnCls", show: "data2.e = 0"
                        },
                        {
                            name: "Year",
                            cls: "span4",
                            readonly: true,
                            text: "Tahun Fiskal"
                        },
                        {
                            name: "Month",
                            cls: "span4",
                            readonly: true,
                            text: "Bulan Fiskal"
                        },
                        {
                            name:"Periode",
                            text:"Periode",
                            cls:"span4",
                            readonly:true
                        },
                        {
                            name:"PeriodName",
                            text:"Nama Periode",
                            cls:"span4",
                            readonly:true
                        },
                        {
                        type: "buttons",
                        items: [
                                { name: "btnClose", text: "Tutup Bulan", icon: "icon", cls: "btn btn-info", click: "ClosePeriod()", disable: "data.btnCls == 0" }
                         ]
                        },
                        {
                            type:"div"
                        }
                     
                    ]   
            },
            {
                name: "wXgrid1",
                xtype: "k-grid",
            },

        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spProsesTutupBulanController");
    }

});