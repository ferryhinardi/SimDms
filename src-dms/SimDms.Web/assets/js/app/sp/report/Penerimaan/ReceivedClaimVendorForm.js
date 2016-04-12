"use strict";
function RptReceivedClaimVendorForm($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.UpdatePrintSeq = function () {
        $http.post('sp.api/reportpenerimaan/UpdatePrintSeq?DocNo=' + me.data.ClaimReceivedNo + "&DocNo1=" + me.data.ClaimReceivedNo1 + "&table=spTrnPRcvClaimHdr&column=ClaimReceivedNo").
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

    me.ClaimReceivedNoBrowse = function (x) {
        var lookup = Wx.blookup({
            name: "ClaimReceivedNo",
            title: "ClaimReceivedNo Lookup",
            manager: spReportPenerimaanManager,
            query: "ReceivedClaimNoBrwose",
            defaultSort: "ClaimReceivedNo asc",
            columns: [
                { field: "ClaimReceivedNo", title: "No. Received Claim" },
                { field: "ClaimReceivedDate", title: "Tgl. Received Claim" },
                { field: "ClaimNo", title: "No. Claim" }
            ],
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.ClaimReceivedNo = data.ClaimReceivedNo;
                    me.data.ClaimReceivedNo1 = data.ClaimReceivedNo;
                }
                else {
                    me.data.ClaimReceivedNo1 = data.ClaimReceivedNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {
        var data = me.data.ClaimReceivedNo + "," + me.data.ClaimReceivedNo1 + ", profitcenter, typeofgoods";
        var rparam = "";

        Wx.showPdfReport({
            id: "SpRpTrn027",
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
        title: "Received Claim Vendor Form",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "data.ClaimNo == undefined" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "ClaimReceivedNo",
                        text: "No. Received Claim",
                        cls: "span4",
                        placeHolder: "No. Received Claim",
                        readonly: true,
                        type: "popup",
                        click: "ClaimReceivedNoBrowse(1)"
                    },
                    {
                        name: "ClaimReceivedNo1",
                        text: "S/D",
                        cls: "span4",
                        placeHolder: "No. Received Claim",
                        readonly: true,
                        disable: "data.ClaimReceivedNo == undefined",
                        type: "popup",
                        click: "ClaimReceivedNoBrowse(2)"
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
        SimDms.Angular("RptReceivedClaimVendorForm");

    }



});