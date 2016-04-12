var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterTaxController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MasterMessageBrowse",
            title: "Message Browse",
            manager: gnManager,
            query: "Messages",
            defaultSort: "MessageHeader asc",
            columns: [
            { field: "MessageHeader", title: "Header" },
            { field: "MessageText", title: "Message" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.data.MessageID = data.MessageID
                me.isSave = false;
                me.Apply();

            }
        });
    }

    me.initialize = function () {
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/Message/Delete', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.init();
                    Wx.Success("Data deleted...");
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.saveData = function (e, param) {

        $http.post('gn.api/Message/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
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
        title: "Message Boards",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "MessageBoards",
                title: "Message Boards",
                items: [
                    { name: "MessageID", type: "hidden" },
                    { name: "MessageHeader", type: "text", text: "Subject", cls: "span8", validasi: "required", require : true },
                    { name: "MessageText", type: "textarea", text: "Message", validasi: "required", require: true },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterTaxController");
    }
});