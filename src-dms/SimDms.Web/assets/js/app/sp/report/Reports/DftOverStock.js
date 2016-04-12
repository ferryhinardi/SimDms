"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
     success(function (data, status, headers, config) {
         me.comboTPGO = data;
     });
    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
            return;
        var prm = [
                   me.data.TypeOfGoods
        ];
        Wx.showPdfReport({
            id: "SpRpSum015",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.Period = new Date();
        me.data.OrderBy = '';
        //me.data.WarehouseCode = '';

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
        title: "Laporan Rekapitulasi Omzet",
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
                        { name: "TypeOfGoods", model: "data.TypeOfGoods", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Type Part", datasource: "comboTPGO" }
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