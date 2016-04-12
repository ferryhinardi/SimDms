"use strict";
function spRptLstPerubahanOutStanding($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    me.printPreview = function () {
        var prm = [me.data.StatusPart, me.data.WarehouseCode, me.data.OnHand, me.data.TypeOfGoods, me.data.PartNo1, me.data.PartNo2, 0];
        Wx.showPdfReport({
            id: "SpRpMst003",
            pparam: (me.data.Period.getMonth()+1)+','+me.data.Period.getFullYear(),
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.isPrintAvailable = true;
        $http.get('breeze/sparepart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });
    }


    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Daftar Perubahan Outstanding",
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
                        { name: "Period", model: "data.Period", text: "Bulan Tahun", cls: "span3", type: "ng-datepicker",format:'MMMM yyyy' }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {       
        SimDms.Angular("spRptLstPerubahanOutStanding");
    }



});