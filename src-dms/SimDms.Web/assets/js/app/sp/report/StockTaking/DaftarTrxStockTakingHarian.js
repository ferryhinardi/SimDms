"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.OrderBy = [
          { "value": 'PartNo', "text": 'Part Number' },
          { "value": 'Location', "text": 'Location' }
    ];
    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
            return;
        var prm = [
                   ((me.data.Period.getMonth() + 1)) + "-01-" + me.data.Period.getFullYear(),
                   me.data.WarehouseCode,
                   me.data.OrderBy
        ];
        Wx.showPdfReport({
            id: "SpRpTrn025",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.Period = new Date();
        me.data.OrderBy = '';
        //me.data.WarehouseCode = '';

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
        title: "Daftar Transaksi Stock Taking Harian",
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
                        { name: "Period", model: "data.Period", text: "Bulan Tahun", cls: "span3", type: "ng-datepicker",format:'MMMM yyyy' },                        
                        { name: "WarehouseCode", model: "data.WarehouseCode", text: "Warehouse", cls: "span3 full", disable: "isPrintAvailable" },
                        { name: "OrderBy", opt_text: "", cls: "span3 full", type: "select2", text: "Order By", datasource: "OrderBy" }
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