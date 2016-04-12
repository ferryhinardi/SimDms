"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
     success(function (data, status, headers, config) {
         me.comboTPGO = data;
     });
    me.CodeTrx = [
         { "value": 10, "text": 'EXTERNAL' },
         { "value": 20, "text": 'INTERNAL' },
         { "value": 30, "text": 'FSC/KSG' },
         { "value": 40, "text": 'WARANTY' }
    ];
    me.SalesType = [
         { "value": 0, "text": 'Service' },
         { "value": 1, "text": 'Unit Order' }
        
    ];
    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
            return;
        var prm = [
                   me.data.Period1,
                   me.data.Period2,
                   me.data.CodeTrx,
                   me.data.SalesType,
                   me.data.TypeOfGoods
        ];
        Wx.showPdfReport({
            id: "SpRpRgs003",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.Period1 = '1-' + me.now("MMM-YYYY");
        me.data.Period2 = me.now("DD-MMM-YYYY");
        me.data.SalesType = 0;

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
        title: "Lampiran Faktur untuk Supply Slip dan Unit",
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
                        { name: "SalesType", model: "data.SalesType", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Sales Type", datasource: "SalesType" },
                        { name: "Period1", model: "data.Period1", text: "Periode", cls: "span3", type: "datepicker", format: 'dd MMMM yyyy' },
                        { name: "Period2", model: "data.Period2", text: "Sampai Dengan", cls: "span3", type: "datepicker", format: 'dd MMMM yyyy' },
                        { name: "CodeTrx", model: "data.CodeTrx", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Transaction Code", datasource: "CodeTrx" },
                        { name: "TypeOfGoods", model: "data.TypeOfGoods", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Type Part", datasource: "comboTPGO" }
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