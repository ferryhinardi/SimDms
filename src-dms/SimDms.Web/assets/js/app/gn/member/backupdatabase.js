var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        
        me.data.suffix="";
    }

    
    me.process = function (e, param) {
        $http.post('gn.api/Backup/BackupDB', me.data).
            success(function (data, status, headers, config) {
                me.data.message = data.message;
                if (data.success) {                    
                    MsgBox(data.message, MSG_INFO);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.start();

}
$(document).ready(function () {
    var options = {
        title: "Backup Database",
        xtype: "panels",
        toolbars: "",//WxButtons,
        panels: [
            {
                name: "pnlInfo",
                title: "",
                items: [
                          
                          { name: "Sequence", model:"data.suffix", type: "text", text: "Sufix Label", cls: "span5" },
                          
                          {
                              type: "buttons",
                              items: [
                                      { name: "btnProcess", text: "Process", icon: "icon-save", cls: "btn btn-info", click: "process()" },
                              ]
                          },
                          {type:"hr"},
                          { name: "Description", model: 'data.message', type: "textarea", text: "", cls: "span5" },
                ]
            },
            
        ]
    };
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spItemPriceController");
    }
});