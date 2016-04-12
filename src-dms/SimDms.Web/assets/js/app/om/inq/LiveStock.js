"use strict";

function omInquiryLiveStockController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.listType = {};
    me.listTransmission = [
        { "value": "AT", "text": "AT" },
        { "value": "MT", "text": "MT" },
    ];
    me.listVariant = {};
    me.listColour = {};
    me.status = "";
    me.initialize = function () {
        $http.get('om.api/Inquiry/GetComboLiveStock?cboType=Type').
          success(function (e) {
              me.listType = angular.copy(e);
          });
        $http.get('om.api/Inquiry/GetComboLiveStock?cboType=Variant').
          success(function (e) {
              me.listVariant = angular.copy(e);
          });
        $http.get('om.api/Inquiry/GetComboLiveStock?cboType=Colour').
          success(function (e) {
              me.listColour = angular.copy(e);
          });
        $http.get('om.api/Inquiry/GetComboLiveStock?cboType=Status').
          success(function (e) {
              if (e.length)
                me.status = angular.copy(e[0].Status);
          });
        me.data.Transmission = "";

        me.gridLiveStock.adjust();
    }

    $("[name=Type]").on('change', function (e) { me.data.Type = $(this).val(); });
    $("[name=Transmission]").on('change', function (e) { me.data.Transmission = $(this).val(); });
    $("[name=Variant]").on('change', function (e) { me.data.Variant = $(this).val(); });
    $("[name=Colour]").on('change', function (e) { me.data.Colour = $(this).val(); });

    me.gridLiveStock = new webix.ui({
        container: "wxLiveStock",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "Type", header: "Type", width: 200 },
            { id: "Variant", header: "Variant", width: 200 },
            { id: "Transmission", header: "Transmission", width: 200 },
            { id: "Colour", header: "Colour", width: 200 },
            { id: "Qty", header: "Qty", width: 200 },
        ],
    });

    me.refresh = function () {
        $http.post('om.api/Inquiry/GetLiveStock', me.data)
                    .success(function (e) {
                        if (e.success) {
                            if (e.grid != "") {
                                me.loadTableData(me.gridLiveStock, e.grid);
                            } else {
                                MsgBox("Tidak Ada Data", MSG_ERROR);
                            }
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Live Stock",
        xtype: "panels",
        toolbars: [
            { text: "Refresh", name: "refresh", action: "refresh", icon: "icon-refresh", click: "refresh()" }
        ],
        panels: [
            {
                name: "pnlLiveStock",
                items: [{ 
                        type: "controls",
                        cls: "span4",
                        text: "Type",
                        items: [
                            { name: "Type", cls: "span7 full", type: "select2", opt_text: "-- SELECT ALL --", datasource: "listType" },
                        ]
                    }, {
                        type: "controls",
                        cls: "span4",
                        text: "Transmission",
                        items: [
                            { name: "Transmission", cls: "span7 full", type: "select2", opt_text: "-- SELECT ALL --", datasource: "listTransmission" },
                        ]
                    }, {
                        type: "controls",
                        cls: "span4",
                        text: "Variant",
                        items: [
                            { name: "Variant", cls: "span7 full", type: "select2", opt_text: "-- SELECT ALL --", datasource: "listVariant" },
                        ]
                    }, {
                        type: "controls",
                        cls: "span4",
                        text: "Colour",
                        items: [
                            { name: "Colour", cls: "span7 full", type: "select2", opt_text: "-- SELECT ALL --", datasource: "listColour" },
                        ]
                    }, {
                        type: "controls",
                        cls: "span4",
                        text: "Status",
                        items: [
                            { name: "Status", cls: "span7 full", type: "label", text: "{{status}}" },
                        ]
                    },
                    {
                        name: "wxLiveStock",
                        type: "wxdiv"
                    },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("omInquiryLiveStockController");
    }
});