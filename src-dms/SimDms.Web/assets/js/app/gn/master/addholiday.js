var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
   
    me.save = function (e, param) {
        $http.post('gn.api/calender/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    localStorage.setItem("CloseInterval", true);
                    localStorage.setItem("RefreshGrid",true);
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };
    
    me.initialize = function () {
        me.data.CalendarDate = me.now();
        me.data.ProfitCenter = localStorage.getItem('ProfitCenter');
        me.data.ProfitCenterCode = localStorage.getItem('ProfitCenterCode');
        me.hasChanged = false;
        me.isSave = false;
        ChangePassword();
    }
    me.start();
}
$(document).ready(function () {
    window.UserInfo = {};

    var options = {
        title: "Add Holiday",
        xtype: "panels",
        toolbars: "",
        panels: [
            {
                name: "pnlInfo",
                title: "",
                items: [
                        { name: "ProfitCenterCode", model: "data.ProfitCenterCode", type: "hidden", text: "", cls: "span4 full", readonly: true },
                        { name: "ProfitCenter",type: "text", text: "ProfitCenter", cls: "span4 full", readonly : true },
                        { name: "CalendarDate", model: "data.CalendarDate", type: "ng-datepicker", text: "Date", cls: "span4 full" },
                        { name: "CalendarDescription ", model: "data.CalendarDescription", type: "text", text: "Notes", cls: "span4 full", validasi: "required", required : true },
                {
                type: "buttons",
                cls: "span4",
                items: [
                        { name: "btnSave", text: "Save", icon: "icon-plus", cls: "btn btn-info", click: "save()" },
                        //{ name: "btnCancel", text: "Cancel", icon: "icon-plus", cls: "button small btn btn-danger", click: "cancel()" }
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