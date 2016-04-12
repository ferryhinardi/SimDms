var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spPembatalanBlankInventoryFormTagController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });   
   
    me.ProcessCancel = function (e, param) {
        $http.post('sp.api/StockOpname/BatalInvForm', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    MsgBox(data.message);
                    me.blankexist = false;
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }


    me.initialize = function()
    {
        $http.post('sp.api/StockOpname/CheckBatalInventory').
           success(function (rslt, status, headers, config) {
               if (rslt.success) {
                   me.blankexist = true;
                   me.data = rslt.data;
               } else {
                   MsgBox(rslt.message, MSG_ERROR);
               }
           }).
           error(function (data, status, headers, config) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
           });
    }
    
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Cancel Blank Inventory Form/Tag",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "PEMBATALAN BLANK INVENTORY TAG/FORM",
                items: [                        
                        {
                          type: "buttons", cls: "span3", items: [
                                 { name: "btnAdd", text: "Proses", icon: "icon-save", click: "ProcessCancel()", cls: "btn btn-primary", disable: "!blankexist" }
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
        $(".toolbar button").hide();
        SimDms.Angular("spPembatalanBlankInventoryFormTagController");

    }

});