"use strict";
function spRptLstMstSubstitusi($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=INCD').
    success(function (data, status, headers, config) {
        me.comboINCD = data;        
    });



    me.printPreview = function () {        
        Wx.showPdfReport({
            id: "SpRpMst002",
            pparam: 'companycode,' + (me.data.InterchangeCode == undefined ? 'ALL' : me.data.InterchangeCode),
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.Apply();
        
    }


    me.start();

}
$(document).ready(function () {
    var options = {
        title: "List Master Substitusi",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [                        
                        { name: "InterchangeCode", model: "data.InterchangeCode", text: "Interchange Code", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", datasource: "comboINCD" }

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("spRptLstMstSubstitusi");
    }



});