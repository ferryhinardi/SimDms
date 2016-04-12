var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spSwitchTypePartUserController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboTPGO = data;          
    });  


 

    me.initialize = function()
    {

        $http.get('breeze/sparepart/CurrentUserInfo').
       success(function (dl, status, headers, config) {           
           me.data.typePart = dl.TypeOfGoods;           
       });              
 
    }


    me.Process = function () {
        $http.post('sp.api/Utility/ChangePartType?partype=' + me.data.typePart).
        success(function (dl, status, headers, config) {
            if (dl.success) {
                var usrinf = $("#currentpage .user-info");
                var tmp = usrinf.html().split('|');                
                tmp[4] = '&nbsp;'+dl.data.LookUpValueName;
                usrinf.html(tmp.join('|'));
                MsgBox(dl.message);

            }
            else {
                MsgBox(dl.message, MSG_ERROR);
            }
        });       
    } 
    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Switch Type Part User",
        xtype: "panels",

        panels: [
            {
                name: "pnlA",
                title: "P E R U B A H A N  &nbsp;&nbsp;    T Y P E    &nbsp;&nbsp;  P A R T",
                items: [
                       
                        { name: "typePart",   cls: "span3",  type: "select2", text: "Type Part", datasource: "comboTPGO" },
 
                        {
                            type: "buttons", cls: "span2", items: [
                                { name: "btnProcess", text: "   OK", icon: "icon-ok", click:"Process()", cls: "button small btn btn-sucess"},
                            ]
                        },  


                    ]   
            },
             

        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spSwitchTypePartUserController");
    }

});