"use strict";
function RptClaimVendorForm($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.UpdatePrintSeq = function () {
        $http.post('sp.api/reportpenerimaan/UpdatePrintSeq?DocNo=' + me.data.ClaimNo + "&DocNo1=" + me.data.ClaimNo1 + "&table=spTrnPClaimHdr&column=ClaimNo").
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

    me.ClaimNoBrowse = function (x) {
        var lookup = Wx.blookup({
            name: "ClaimNo",
            title: "ClaimNo Lookup",
            manager: spReportPenerimaanManager,
            query: "ClaimNoBrwose",
            defaultSort: "ClaimNo asc",
            columns: [
                { field: "ClaimNo", title: "No. Claim" },
                { field: "ClaimDate", title: "Tgl. Claim" }
            ],
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.ClaimNo = data.ClaimNo;
                    me.data.ClaimNo1 = data.ClaimNo;
                }
                else {
                    me.data.ClaimNo1 = data.ClaimNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {
        var data = me.data.ClaimNo + "," + me.data.ClaimNo1 + ", profitcenter, typeofgoods";
        var rparam = "";

        Wx.showPdfReport({
            id: "SpRpTrn005",
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
        title: "Claim Vendor Form",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "data.ClaimNo == undefined" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "ClaimNo",
                        text: "No. HPP",
                        cls: "span4",
                        placeHolder: "No. Claim",
                        readonly: true,
                        type: "popup",
                        click: "ClaimNoBrowse(1)"
                    },
                    {
                        name: "ClaimNo1",
                        text: "S/D",
                        cls: "span4",
                        placeHolder: "No. Claim",
                        readonly: true,
                        disable: "data.ClaimNo == undefined",
                        type: "popup",
                        click: "ClaimNoBrowse(2)"
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
        SimDms.Angular("RptClaimVendorForm");

    }



});