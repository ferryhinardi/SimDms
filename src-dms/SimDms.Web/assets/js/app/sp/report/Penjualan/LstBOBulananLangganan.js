"use strict";
function spRptLstBOBulananLangganan($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
        success(function (data, status, headers, config) {
        me.comboTPGO = data;
    });

    me.printPreview = function () {        
        var prm = [me.data.Period.getFullYear() + '/' + (me.data.Period.getMonth() + 1) + "/20", (me.data.TypeOfGoods == "" ? "%" : me.data.TypeOfGoods)];        
        Wx.showPdfReport({
            id: "SpRpTrn015",
            pparam:prm,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.isPrintAvailable = true;
        me.data.Period =new Date( me.now());        
        me.data.TypeOfGoods='';
    }


    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Daftar Back Order Bulanan Per Pelanggan",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "Period", model: "data.Period", text: "Bulan Tahun", cls: "span3", type: "ng-datepicker", format: 'MMMM yyyy' },
                    { name: "TypeOfGoods", model: "data.TypeOfGoods", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Tipe Part", datasource: "comboTPGO" },

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("spRptLstBOBulananLangganan");
    }




});