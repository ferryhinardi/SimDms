"use strict";
function spRptLstMOnOrderPart($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.printPreview = function () {

      
        var isBo = '';
        if (me.data.BO && !me.data.NONBO)
            isBo = "1"
        else if (!me.data.BO && me.data.NONBO)
            isBo = "0"
        else if (me.data.BO && me.data.NONBO)
            isBo = "";
        

        Wx.showPdfReport({
            id: "SpRpTrn037",
            pparam:isBo,        
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.BO = false;
        me.data.NONBO = false;
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
        title: "Daftar On Order Part",
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
                        { name: "BO", text:"BO",model: "data.Bo", type: "switch", cls: "full", float: "left"},
                        { name: "NONBO", text:"Non BO",model: "data.NonBo ", type: "switch", cls: "full", float: "left" }
                       ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {        
        SimDms.Angular("spRptLstMOnOrderPart");
    }

});