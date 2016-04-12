"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
     success(function (data, status, headers, config) {
         me.comboTPGO = data;
     });
    me.Pilihan = [
          { "value": 'CLASS', "text": 'CLASS' },
          { "value": 'AREA', "text": 'AREA' },
          { "value": 'KOTA', "text": 'KOTA' },
          { "value": 'LOKASI', "text": 'LOKASI' }
    ];
    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
            return;
        var prm = [
                   me.data.Pilihan,
                   me.data.Period1,
                   me.data.Period2,
                   me.data.Code1,
                   me.data.Code2,
                   me.data.TypeOfGoods
        ];
        Wx.showPdfReport({
            id: "SpRpSum013",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.Period1 = '1-' + me.now("MMM-YYYY");
        me.data.Period2 = me.now("DD-MMM-YYYY");
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
        title: "Laporan Penjualan per Class Area dan Lokasi",
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
                        { name: "Period1", model: "data.Period1", text: "Periode", cls: "span3", type: "datepicker", format: 'dd MMMM yyyy' },
                        { name: "Period2", model: "data.Period2", text: "Sampai Dengan", cls: "span3", type: "datepicker", format: 'dd MMMM yyyy' },
                        { name: "Pilihan", model: "data.Pilihan", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Pilihan", datasource: "Pilihan" },
                        { name: "Code1", model: "data.Code1", text: "Kode", cls: "span3", type: "select2"},
                        { name: "Code2", model: "data.Code2", text: "Sampai Dengan", cls: "span3", type: "select2"},
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