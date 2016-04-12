"use strict";
function spRptPeseananSparepart($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.POLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Pencarian NO. PO",
            manager: spManager,
            query: "PosHdrLookup",
            columns: [
                { field: "PosNo", title: "No. PO" },
                { field: "PosDate", title: "Tgl PO", template: "#= (PosDate == undefined) ? '' : moment(PosDate).format('DD MMM YYYY HH:mm:ss') #" },
                { field: "SupplierCode", title: "Kode Suplier" }
            ],
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                if (x == 1) {
                    me.data.POSNo1 = data.PosNo;
                    me.data.POSNo2 = data.PosNo;
                }
                else {
                    me.data.POSNo2 = data.PosNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {



        var prm = [me.data.POSNo1,
                    me.data.POSNo2,
                    '300',
                    'typeofgoods'
                    ];

        Wx.showPdfReport({
            id: "SpRpTrn002",
            pparam: prm.join(','),
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.SuggorNo1 = '';
        me.data.SuggorNo2 = '';

        $http.get('breeze/sparepart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        me.isPrintAvailable = true;
    }
    me.start();
}
$(document).ready(function () {
    var options = {
        title: "Daftar Pesanan Sparepart",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        {
                            text: "No. PO", type: "controls",
                            items: [
                                    { name: "POSNo1", model: "data.POSNo1", cls: "span1", placeHolder: "No PO", type: "popup", click: "POLkp(1)" },
                                    { type: "label", text: "S/D", cls: "span1 mylabel" },
                                    { name: "POSNo2", model: "data.POSNo2", cls: "span1", placeHolder: "No PO", type: "popup", click: "POLkp(2)" }
                            ]
                        }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px")
        SimDms.Angular("spRptPeseananSparepart");
    }



});