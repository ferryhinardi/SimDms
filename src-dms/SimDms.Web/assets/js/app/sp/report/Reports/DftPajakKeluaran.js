"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    $http.post('sp.api/Combo/Years?').
   success(function (data, status, headers, config) {
       me.Years = data;
   });
    $http.post('sp.api/Combo/Months?').
success(function (data, status, headers, config) {
    me.Months = data;
});
    me.Status = [
         { "value": '0', "text": '0' },
         { "value": '1', "text": '1' }
    ];
    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
            return;
        var prm = [
                   me.data.Months,
                   me.data.Years,
                   me.data.Status 
        ];
        Wx.showPdfReport({
            id: "SpRpSum010",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.Years = new Date().getFullYear();
        me.data.Months = (new Date().getMonth()+1);
        me.data.Status='0'

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
        title: "Daftar Pajak Keluaran",
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
                        { name: 'Month', model: "data.Months", text: "Month", type: "select2", cls: "span3", optionalText: "-- SELECT MONTH --", datasource: "Months" },
                        { name: 'Years', model: "data.Years", text: "Year", type: "select2", cls: "span2", optionalText: "-- SELECT YEAR --", datasource: "Years" },
                        { name: "Status", model: "data.Status", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Status", datasource: "Status" }
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