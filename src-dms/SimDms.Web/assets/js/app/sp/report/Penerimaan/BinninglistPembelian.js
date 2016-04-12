"use strict";
function RptBinningListPembelian($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.UpdatePrintSeq = function () {
        $http.post('sp.api/reportpenerimaan/UpdatePrintSeq?DocNo=' + me.data.BinningNo + "&DocNo1=" + me.data.BinningNo1 + "&table=spTrnPBinnHdr&column=BinningNo").
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

    me.BinningBrowse = function (x) {
        var lookup = Wx.blookup({
            name: "BinningList",
            title: "Binning List",
            manager: spReportPenerimaanManager,
            query: "BinningListBrowse",
            defaultSort: "BinningNo asc",
            columns: [
                { field: "BinningNo", title: "No. Binning" },
                { field: "BinningDate", title: "Tgl. Binning" },
                { field: "Status", title: "Status" },
                { field: "ReferenceNo", title: "No. Referensi" },
                { field: "DNSupplierNo", title: "No. DN" }
            ],
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.BinningNo = data.BinningNo;
                    me.data.BinningNo1 = data.BinningNo;
                }
                else {
                    me.data.BinningNo1 = data.BinningNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {
        var data = me.data.BinningNo + "," + me.data.BinningNo1 + ", profitcenter, typeofgoods";
        var rparam = "admin";

        Wx.showPdfReport({
            id: "SpRpTrn003A",
            pparam: data,
            rparam: rparam,
            type: "devex"
        });

        me.UpdatePrintSeq();
    }

    me.initialize = function () {
        me.data = {};

        me.isPrintAvailable = true;

    }


    me.start();

}
$(document).ready(function () {
    var options = {
        title: "Binning List Pembelian",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "data.BinningNo == undefined" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "BinningNo",
                        text: "No. Binning",
                        cls: "span4",
                        placeHolder: "No. Binning",
                        readonly: true,
                        type: "popup",
                        click: "BinningBrowse(1)"
                    },
                    {
                        name: "BinningNo1",
                        text: "S/D",
                        cls: "span4",
                        placeHolder: "No. Binning",
                        readonly: true,
                        disable: "data.BinningNo == undefined",
                        type: "popup",
                        click: "BinningBrowse(2)"
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
        SimDms.Angular("RptBinningListPembelian");

    }



});