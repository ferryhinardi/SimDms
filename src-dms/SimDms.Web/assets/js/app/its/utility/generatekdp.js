"use strict"

function generatekdp($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    
    me.Generate = function () {
        $http.post('its.api/GenerateKdp/GenerateKdp', { dateFrom: me.data.DateFrom, dateTo: me.data.DateTo }).
        success(function (data, status, headers, config) {
            if (data.message == "") {
                var sessionName = data.sessionName;
                location.href = 'its.api/GenerateKdp/DownloadFile?sessionName=' + sessionName;
                MsgBox("Generate KDP sukses.\n" + data.rowCount + " KDP telah tergenerate");
            } else {
                MsgBox(data.message);
            }
        }).error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.$watch('data.DateFrom', function (newValue, oldValue) {
        if (me.data.DateFrom > me.data.DateTo) {
            me.data.DateTo = me.data.DateFrom;
        }
    }, true);

    me.$watch('data.DateTo', function (newValue, oldValue) {
        if (me.data.DateTo < me.data.DateFrom) {
            me.data.DateFrom = me.data.DateTo;
        }
    }, true);

    me.initialize = function () {
        me.data.DateFrom = new Date();
        me.data.DateTo = new Date();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Generate KDP",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" }
        ],
        panels: [
            {
                items: [
                    {
                        text: "Periode",
                        type: "controls",
                        items: [
                            { name: "DateFrom", type: "ng-datepicker", cls: "span1" },
                            { type: "label", text: "s/d", cls: "span1", style: "line-height: 33px" },
                            { name: "DateTo", text: "S/D", type: "ng-datepicker", cls: "span1" },
                            {
                                type: "buttons",
                                items: [
                                    { name: "btnGenerate", text: " Generate KDP", cls: "btn btn-info", icon: "icon-gear", style: "margin-top:1px;height: 32px;line-height:6px", click: "Generate()" }
                                ]
                            }
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
        SimDms.Angular("generatekdp");
    }
});