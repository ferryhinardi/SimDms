var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spResetStockOpnameController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.ResetStokOpname = function () {
        $http.post('sp.api/Utility/ResetStockTaking?sthdrno=' + me.sthdr).
          success(function (rslt, status, headers, config) {
              if (rslt.success) {
                  $("#lblInfo").html('');
                  me.sthdr = undefined;
                  MsgBox(rslt.message);                                 
              } else {
                  MsgBox(rslt.message, MSG_ERROR);
              }
              $(".ajax-loader").hide();
          }).
          error(function (data, status, headers, config) {
              MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
              $(".ajax-loader").hide();
          });
    }

    me.initialize = function()
    {
        me.detail = {};
        $(".ajax-loader").show();
        $http.post('sp.api/Utility/IsStockTaking').
          success(function (rslt, status, headers, config) {
              if (rslt.success) {                  
                  $("#lblInfo").html(rslt.message);
                  me.sthdr = rslt.STHDRNO;
                  
              } else {                  
                  $("#lblInfo").html(rslt.message);
              }

              $(".ajax-loader").hide();
          }).
          error(function (data, status, headers, config) {
              MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
              $(".ajax-loader").hide();
          });
        
    }



    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Reset Stock Opname",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "Informasi Stock Opname",
                items: [                       
                         { name: "lblInfo", text: "", type: "label" },
                         {
                             type: "buttons", cls: "span3", items: [
                             { name: "btnReset", text: "Reset", icon: "icon-save", click: "ResetStokOpname()", cls: "btn btn-success", disable: "sthdr  == undefined" },
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
        $("#lblInfo").attr("style", "height:30px");
        $(".toolbar button").hide();
        SimDms.Angular("spResetStockOpnameController");
    }

});