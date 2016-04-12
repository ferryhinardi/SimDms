"use strict"

function svServiceBookController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.grid1 = new webix.ui({
        container: "wxData",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "SeqNo", header: "No." },
            { id: "PreviousData", header: "Data Sebelumnya", fillspace: true },
            { id: "ChangeCode", header: "Kode Perubahan", fillspace: true },
            { id: "LastUpdateBy", header: "Perubahan Oleh", fillspace: true },
            { id: "LastUpdateDate", header: "Tgl. Perubahan", type: "dateTime", fillspace: true },

        ]
    });

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Update Service Book No",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "wxData",
                xtype: "wxtable",
                tblname: "tblPart",
            },
            {typr: "hr"},
        ],
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svServiceBookController");
    }
});