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
                { id: "Nomor", header: "No.", width: 50 },
                { id: "Tablename", header: "Table Name", width: 250 },
                { id: "NData", header: "N Data", width: 250 },
                { id: "LastSendData", header: "Last Send Data", width: 250 }
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
        title: "Backup Branch Data",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "Queryanalizer",
                title: "",
                items: [
                    {
                        type: "controls", text: "Select Data", items: [
                             { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "", type: "text", cls: "span3" },
                             { name: "ProfitCenterNameDisc", model: "data.ProfitCenterNameDisc", text: "", cls: "span5" },
                        ]
                    },
                    {
                        type: "optionbuttons", name: "isDate", model: "data.isDate", text: "Select Date", cls : "span3",
                        items: [
                            { name: "CreateDate", text: "Created Date" },
                            { name: "LastUpdateDate", text: "Last Update Date" },
                        ]
                    },
                     {
                         type: "controls", text: "", cls: "span5", items: [
                            { name: "FromDate", type: "ng-datepicker", text: "From Date", cls: "span4" },
                            { name: "EndDate", type: "ng-datepicker", text: "End Date", cls: "span4" },
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