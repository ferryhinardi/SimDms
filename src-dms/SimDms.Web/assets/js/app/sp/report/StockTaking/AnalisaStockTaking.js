"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {

    var me = $scope;
    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboTPGO = data;
    });
    $injector.invoke(BaseController, this, { $scope: me });
    me.MovingCode = [
           { "value": '0', "text": '0' },
           { "value": '1', "text": '1' },
           { "value": '2', "text": '2' },
           { "value": '3', "text": '3' },
           { "value": '4', "text": '4' },
           { "value": '5', "text": '5' }
    ];
    me.Status = [
           { "value": '1', "text": 'BALANCE' },
           { "value": '2', "text": 'UNBALANCE' }
    ];
    me.PartLkp = function (x) {
        var lookup = Wx.blookup({
            name: "SparepartLookup",
            title: "Item Lookup",
            manager: spManager,
            query: "SparePartLookup",
            defaultSort: "PartNo asc",
            columns: [
                { field: "PartNo", title: "No Part" },
                { field: "PartName", title: "Nama Part" },
                { field: "SupplierCode", title: "Kode Suplier" },
                { field: "IsGenuinePart", title: "Prd Suzuki" },
                { field: "ProductType", title: "Tipe Produk" },
                { field: "PartCategory", title: "Kategori" },
                { field: "CategoryName", title: "Nama Kategori" },
                { field: "TypeOfGoods", title: "Tipe Part" }
            ],
        });

        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PartNo = data.PartNo;
                me.Apply();
            }
        });
    }

    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
            return;
        var prm = [
                    me.data.PartNo,
                    me.data.WarehouseCode,
                    me.data.MovingCode,
                    me.data.TypeOfGoods,
                    me.data.Status
        ];
        Wx.showPdfReport({
            id: "SpRpTrn022",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.PartNo = '';
        me.data.StockingDate = '';
        //me.data.WarehouseCode = '';
        me.data.MovingCode = '';
        me.data.Status = '';
        me.data.TypeOfGoods = '';

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
        title: "Analisa Stock Taking",
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
                        { name: "PartNo", model: "data.PartNo", text: "No.Taking Stock ", cls: "span3 full", placeHolder: "Part No", type: "popup", click: "PartLkp(1)", disable: "data.PartNo" },
                        { name: "StockingDate", model: "data.StockingDate", text: "Stocking Date", cls: "span3 full", disable: "isPrintAvailable" },
                        { name: "WarehouseCode", model: "data.WarehouseCode", text: "Warehouse", cls: "span3 full", disable: "isPrintAvailable" },
                        { name: "MovingCode", opt_text: "", cls: "span3 full", type: "select2", text: "Moving Code", datasource: "MovingCode" },
                        { name: "Status", opt_text: "", cls: "span3 full", type: "select2", text: "Status", datasource: "Status" },
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