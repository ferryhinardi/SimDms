"use strict";
function RptEntryHPP($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.UpdatePrintSeq = function () {
        $http.post('sp.api/reportpenerimaan/UpdatePrintSeq?DocNo=' + me.data.HPPNo + "&DocNo1=" + me.data.HPPNo1 + "&table=spTrnPHPP&column=HPPNo").
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

    me.HPPNoBrowse = function (x) {
        var lookup = Wx.blookup({
            name: "HPPNo",
            title: "HPPNo Lookup",
            manager: spReportPenerimaanManager,
            query: "EntryHPPBrwose",
            defaultSort: "HPPNo asc",
            columns: [
                { field: "HPPNo", title: "No. HPP" },
                { field: "HPPDate", title: "Tgl. HPP" },
                { field: "WRSNo", title: "No. WRS" },
                { field: "WRSDate", title: "Tgl. WRS" },
                { field: "Status", title: "Status" }
            ],
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.HPPNo = data.HPPNo;
                    me.data.HPPNo1 = data.HPPNo;
                }
                else {
                    me.data.HPPNo1 = data.HPPNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {
        var data = me.data.HPPNo + "," + me.data.HPPNo1 + ", profitcenter, typeofgoods";
        var rparam = "HARGA SATUAN BELUM TERMASUK PPN";

        Wx.showPdfReport({
            id: "SpRpTrn026",
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
        title: "Entry HPP",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "data.HPPNo == undefined" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "HPPNo",
                        text: "No. HPP",
                        cls: "span4",
                        placeHolder: "No. HPP",
                        readonly: true,
                        type: "popup",
                        click: "HPPNoBrowse(1)"
                    },
                    {
                        name: "HPPNo1",
                        text: "S/D",
                        cls: "span4",
                        placeHolder: "No. HPP",
                        readonly: true,
                        disable: "data.HPPNo == undefined",
                        type: "popup",
                        click: "HPPNoBrowse(2)"
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
        SimDms.Angular("RptEntryHPP");

    }



});