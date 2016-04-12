﻿"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
     success(function (data, status, headers, config) {
         me.comboTPGO = data;
     });
    $http.post('sp.api/Combo/Months?').
    success(function (data, status, headers, config) {
        me.Months = data;
    });

    $http.post('sp.api/Combo/Years?').
    success(function (data, status, headers, config) {
        me.Years = data;
    });
    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
            return;
        var prm = [
                   ((me.data.Months) + "-01-" + (me.data.Years))
        ];
        Wx.showPdfReport({
            id: "SpRpSum008",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.Months = new Date().getMonth() + 1;
        me.data.Years = new Date().getFullYear();


        $http.get('breeze/sparepart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });

        me.isPrintAvailable = true;
    }
    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Daftar Perubahan Harga Potongan, Beli dan Jual",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span3 full", disable: "isPrintAvailable" },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span3 full", disable: "isPrintAvailable" },
                        { name: "Month", model: "data.Months", text: "Month", type: "select2", cls: "span3", optionalText: "-- SELECT MONTH --", datasource: "Months" },
                        { name: "Year", model: "data.Years", text: "Year", type: "select2", cls: "span2", optionalText: "-- SELECT YEAR --", datasource: "Years" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("spRptLstMstSparePart");
    }
});