var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
   
         
    me.initialize = function () {
        me.data = {};
        me.data.ProfitCenter = localStorage.getItem('ProfitCode');
        me.data.BeginDate = me.now();
        me.data.EndDate = me.now();
        me.hasChanged = false;
    }

    me.cancel = function () {
        
    }

    me.save = function (e, param) {
        if (me.data.isCity == 1) { me.data.isCity = true; } else { me.data.isCity = false; }
        $http.post('gn.api/Employee/Save2', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    localStorage.setItem("CloseInterval", true);
                    localStorage.setItem("RefreshGrid", true);
                    me.startEditing();
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
        title: "Add Holidays",
        xtype: "panels",
        toolbars: "",
        panels: [
            {
                name: "pnlInfo",
                title: "",
                items: [
                        { name: "BeginDate", type: "ng-datepicker", text: "Date", cls: "span4" },
                         { name: "EndDate", type: "ng-datepicker", text: "s/d", cls: "span4" },
                         { name: 'isMonday', type: 'check', text: 'Monday', cls: 'span2', float: 'left' },
                        { name: 'isTuesday', type: 'check', text: 'Tuesday', cls: 'span2', float: 'left' },
                        { name: 'isWednesday', type: 'check', text: 'Wednesday', cls: 'span2', float: 'left' },
                        { name: 'isThursday', type: 'check', text: 'Thursday', cls: 'span2', float: 'left' },
                        { name: 'isFriday', type: 'check', text: 'Friday', cls: 'span2', float: 'left' },
                        { name: 'isSaturday', type: 'check', text: 'Saturday', cls: 'span2', float: 'left' },
                        { name: 'isSunday', type: 'check', text: 'Sunday', cls: 'span2', float: 'left' },
                        { name: "Notes", type: "text", text: "Notes", cls: "span4 full" },
                {
                type: "buttons",
                cls: "span4",
                items: [
                        { name: "btnSave", text: "Save", icon: "icon-plus", cls: "btn btn-info", click: "save()" },
                        //{ name: "btnCancel", text: "Cancel", icon: "icon-plus", cls: "btn btn-info", click: "cancel()" }
                ]
            },
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