var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.Printer = [
                { "value": "0", "text": "Microsoft XPS Document Writer" },
                { "value": "2", "text": "Fax" },
                { "value": "3", "text": "doPDF v7" },
    ];

    me.browse = function () {
        //MsgBox("Belum Dapat Browse Setting Printer", MSG_ERROR);
        //var lookup = Wx.blookup({
        //    name: "CustomersBrowse",
        //    title: "Customers Utility Browse",
        //    manager: gnManager,
        //    query: "CustomerUtility",
        //    defaultSort: "CompanyCode asc",
        //    columns: [
        //    { field: "CompanyCode", title: "Company Code" },
        //    { field: "BranchCode", title: "Branch Code" },
        //     { field: "GenarateNo", title: "Generate Number" },
        //    ]
        //});
        //lookup.dblClick(function (data) {
        //    if (data != null) {
        //        me.data = data;
        //        me.isSave = false;
        //        me.Apply();
        //    }
        //});
    }


    me.initialize = function () {
        me.data.Location = '0';
    }

    me.save = function (e, param) {
        MsgBox("Belum Dapat Insert Setting Printer", MSG_ERROR);
        //$http.post('gn.api/AutonoCustomer/Save', me.data).
        //    success(function (data, status, headers, config) {
        //        if (data.status) {
        //            Wx.Success("Data saved...");
        //            me.startEditing();
        //        } else {
        //            MsgBox(data.message, MSG_ERROR);
        //        }
        //    }).
        //    error(function (data, status, headers, config) {
        //        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        //    });
    };

    me.start();

}
$(document).ready(function () {
    var options = {
        title: "Printer Setup",
        xtype: "panels",
        toolbars: [
            { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },

        ],//WxButtons,
        panels: [
            {
                name: "pnlInfo",
                title: "Printer Data",
                items: [
                          { name: "PrinterName", type: "text", text: "Printer Name :", cls: "span4 full", required: true, validasi: "required" },
                          { name: "PrinterType", type: "text", text: "Printer Type :", cls: "span4 full", required: true, validasi: "required" },
                          { name: "Location", text: "Printer Location :", type: "select2", cls: "span4", datasource: 'Printer' },
                          {
                              type: "buttons",
                              items: [
                                      { name: "btnSave", text: "Input", icon: "icon-save", cls: "btn btn-info", click: "save()" },
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