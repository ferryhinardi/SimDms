"use strict"

function sendcustomerdata($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.filterSource = [
        { value: 0, text: "All Customer Data" },
        { value: 1, text: "Only New Customer" }
    ]

    me.filterOnChange = function () {
        me.data.filter = $('#filter').select2("val");
        me.isLocked = me.data.filter == 0;

    }

    me.Inquiry = function () {
        if (me.data.filter == "") $('#filter').select2("val", "0");
        var data = {
            isLocked: me.isLocked
        }
        $http.post('sv.api/SendCustomerData/Inquiry', data).
        success(function (data, status, headers, config) {
            me.data.resultArea = data.result;
        }).error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.SaveFile = function () {
        if (me.data.filter == "") $('#filter').select2("val", "0");
        if (me.data.resultArea == "") return;
        var data = {
            text: me.data.resultArea
        }
        $http.post('sv.api/SendCustomerData/PrepareFile', data).
            success(function (data, status, headers, config) {
                var sessionName = data.sessionName;
                location.href = 'sv.api/SendCustomerData/DownloadFile?sessionName=' + sessionName;
                me.sessionName = sessionName;
        });

    }

    me.SendToDCS = function () {
        if (me.data.filter == "") $('#filter').select2("val", "0");
        if (me.data.resultArea == "") return;
        var data = {
            text: me.data.resultArea,
            filter: me.data.filter
        }
        $http.post('sv.api/SendCustomerData/SendToDCS', data).
            success(function (data, status, headers, config) {
                if (data.message != "") {
                    MsgBox("WSMRD gagal di-generate: " + data.message);
                } else {
                    MsgBox("WSMRD berhasil di-upload");
                }
            }).error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.initialize = function () {
        me.data.filter = "0";
        me.data.resultArea = "";
        me.sessionName = "";
        me.isLocked = false;
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Send Customer Data",
        xtype: "panels",
        panels: [
            {
                items: [
                    { name: "filter", cls: "span4", type: "select2", required: "required", datasource: "filterSource", change: "filterOnChange()" },
                    { name: "resultArea", cls: "span8", type: "textarea", readonly: "readonly", style: "height:500px;resize:none;overflow-y:scroll;overflow-x:scroll;white-space:nowrap" },
                    {
                        type: "buttons",
                        items: [
                            { text: "Inquiry", cls: "btn btn-success", icon: "icon-search", click: "Inquiry()" },
                            { text: "Save File", cls: "btn btn-success", icon: "icon-save", click: "SaveFile()" },
                            { text: "Send to DCS", cls: "btn btn-success", icon: "icon-suitcase", click: "SendToDCS()" }
                        ]
                    }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("sendcustomerdata");
    }
});