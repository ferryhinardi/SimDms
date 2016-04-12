"use strict";
function RptWRSPembelian($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Trans = [
        { value: '0', text: 'TRANSFER STOCK' },
        { value: '1', text: 'PEMINJAMAN' },
        { value: '2', text: 'PENGEMBALIAN' },
        { value: '3', text: 'LAIN-LAIN' },
        { value: '4', text: 'PEMBELIAN' },
        { value: '5', text: 'INTERNAL' },
        { value: '5', text: 'SEMUA NON PEMBELIAN' },
    ];

    me.WRSNoBrowse = function (x) {
        var lookup = Wx.blookup({
            name: "WRSNo",
            title: "Pencarian No. WRS",
            manager: spReportPenerimaanManager,
            query: new breeze.EntityQuery.from("WRSBrowse").withParameters({ "TransType": me.data.TransType }),
            defaultSort: "WRSNo asc",
            columns: [
                { field: "WRSNo", title: "No. Suggor" },
                { field: "WRSDate", title: "Tgl. Suggor" },
                { field: "BinningNo", title: "No. Binning" },
                { field: "BinningDate", title: "Tgl. Binning" }
            ],
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.WRSNo = data.WRSNo;
                    me.data.WRSNo1 = data.WRSNo;
                }
                else {
                    me.data.WRSNo1 = data.WRSNo;
                }
                me.Apply();
            }
        });
    }

    me.SetIdParam = function (param) {
        switch (param) {
            case "POS":
                return "SpRpTrn002";
                break;
            case "WRL":
                return "SpRpTrn004";
                break;
            case "WRN":
                return "SpRpTrn004";
                break;
            case "HPP":
                return "SpRpTrn026";
                break;
            case "SOC":
                return "SpRpTrn031";
                break;
            case "BPS":
                return "SpRpTrn009";
                break;
            case "PLS":
                return "SpRpTrn033";
                break;
            case "LMP":
                return "SpRpTrn028";
                break;
            case "STR":
                return "SpRpTrn013";
                break;
            case "SSS":
                return "SpRpTrn039";
                //SetParameterReport(false, "(SERVICE)");
                break;
            case "SSU":
                return "SpRpTrn039";
               // SetParameterReport(false, "(UNIT)");
                break;
            case "POSN":
                return "SpRpTrn002";
               // SetParameterReport(false, "(UNIT)");
                break;
            case "WRLN":
                return "SpRpTrn004";
                break;
            case "WRNN":
                return "SpRpTrn004";
                break;
        }
    }


    me.printPreview = function () {

        var str = me.data.WRSNo.substr(0, 3);
        $http.post('sp.api/reportpenerimaan/Signature?DocumentType=' + str).
            success(function (data, status, headers, config) {
                if (data.success) {
                    
                    $http.post('sp.api/reportpenerimaan/SetID').
                    success(function (data, status, headers, config) {
                        if (data.success) {
                            var ide = "";
                            var documentFlagType = (me.data.TransType == 4) ? "WRLN" : "WRNN";
                            if (data.status == "") {
                                ide = me.SetIdParam(str);
                            } else if (status == "0") {
                                ide = me.SetIdParam(str);
                            } else {
                                ide = me.SetIdParam(documentFlagType);
                            }

                            var tipe = (me.data.TransType == 4) ? "PEMBELIAN" : "NON PEMBELIAN";
                            var data = me.data.WRSNo + "," + me.data.WRSNo1 + ", typeofgoods";
                            var rparam = data.signName + "," + data.titleSign + "," + tipe;

                            Wx.showPdfReport({
                                id: ide,
                                pparam: data,
                                rparam: rparam,
                                type: "devex"
                            });

                            me.UpdatePrintSeq();
                        } else {

                        }
                    }).error(function (data, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
                    

                    
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });


        
    }

    me.UpdatePrintSeq = function () {
        $http.post('sp.api/reportpenerimaan/UpdatePrintSeq?DocNo=' + me.data.WRSNo + "&DocNo1=" + me.data.WRSNo1 + "&table=spTrnPRcvHdr&column=WRSNo").
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.init();
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
        title: "WRS Pembelian",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "data.WRSNo == undefined" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "TransType",
                        opt_text: "[SELECT ALL]",
                        cls: "span5 full",
                        type: "select2",
                        text: "Tipe Transaksi",
                        datasource: "Trans"
                    },
                    {
                        name: "WRSNo",
                        text: "No. WRS",
                        cls: "span4",
                        placeHolder: "No. WRS",
                        readonly: true,
                        disable: "data.TransType == undefined",
                        type: "popup",
                        click: "WRSNoBrowse(1)"
                    },
                    {
                        name: "WRSNo1",
                        text: "S/D",
                        cls: "span4",
                        placeHolder: "No. WRS",
                        readonly: true,
                        disable: "data.WRSNo == undefined",
                        type: "popup",
                        click: "WRSNoBrowse(2)"
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
        SimDms.Angular("RptWRSPembelian");

    }



});