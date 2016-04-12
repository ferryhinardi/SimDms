var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterCustomersController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.data.isDate = 'CreateDate';
        me.data.FromDate = me.now();
        me.data.EndDate = me.now();
        me.hasChanged = false;
    }
    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxsalestarget",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [

                { id: "CompanyCode", header: "Company Code", width: 250 },
                { id: "Module", header: "Module", width: 250 },
                { id: "Menu", header: "Menu", width: 250 }
            ],

            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        me.data = this.getItem(me.grid1.getSelectedId().id);
                        me.data.oid = me.grid1.getSelectedId();
                        me.Apply();
                    }
                }
            }
        });
    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Admin & Audit",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "Queryanalizer",
                title: "",
                items: [
                      {
                          type: "optionbuttons", name: "isDate", model: "data.isDate", text: "", cls: "span3",
                          items: [
                              { name: "0", text: "Menu Statik" },
                              { name: "1", text: "User Statik" },
                          ]
                      },
                  
                     {
                         type: "controls", text: "Transaction Date", cls: "span5", items: [
                            { name: "FromDate", type: "ng-datepicker", text: "From Date", cls: "span4" },
                            { name: "EndDate", type: "ng-datepicker", text: "End Date", cls: "span4" },
                         ]
                     },
                     {
                         type: "buttons",
                         items: [
                                 { name: "btnReload", text: "Reload", icon: "icon-plus", cls: "btn btn-info", click: "reload()" },
                         ]
                     },
                ]
            },
             {
                 name: "wxsalestarget",
                 xtype: "wxtable",
             },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterCustomersController");
    }

});