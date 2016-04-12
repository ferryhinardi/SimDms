"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
            return;
        var prm = [];
        Wx.showPdfReport({
            id: "SpRpTrn024",
            pparam: "",
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
       // me.data.WarehouseCode = '';

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
        title: "Daftar Outstanding Inventory Form/ Tag",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span3 full", disable: "isPrintAvailable" },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span3 full", disable: "isPrintAvailable" },
                        { name: "WarehouseCode", model: "data.WarehouseCode", text: "Warehouse", cls: "span3 full", disable: "isPrintAvailable" },
                    ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("spRptLstMstSparePart");
    }
});