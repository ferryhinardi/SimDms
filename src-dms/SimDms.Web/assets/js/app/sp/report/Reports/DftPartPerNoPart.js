"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
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
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PartNo = data.PartNo;
                me.Apply();
            }
        });
    }
    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
     success(function (data, status, headers, config) {
         me.comboTPGO = data;
     });
    $http.post('sp.api/Combo/LoadComboData?CodeId=WRCD').
    success(function (data, status, headers, config) {
    data = $(data).filter(function () {
        return (this.value <= '99');
    });
    me.comboWRCD = data;
    });

    me.ABCCode = [
       { "value": 'A', "text": 'GOOD' },
       { "value": 'B', "text": 'MODERATE' },
       { "value": 'C', "text": 'POOR' }
    ];
    me.MovingCode = [
         { "value": '0', "text": '0' },
         { "value": '1', "text": '1' },
         { "value": '2', "text": '2' },
         { "value": '3', "text": '3' },
         { "value": '4', "text": '4' },
         { "value": '5', "text": '5' }
    ];
    me.OrderBy = [
      { "value": 'PartNo', "text": 'Part Number' },
      { "value": 'Location', "text": 'Location' }
    ];
    me.Comparison = [
     { "value": '> 0', "text": '> 0' },
     { "value": '>= 0', "text": '>= 0' },
     { "value": '= 0', "text": '= 0' },
     { "value": '<> 0', "text": '<> 0' },
     { "value": '< 0', "text": '< 0' },
     { "value": '<= 0', "text": '<= 0' }
    ];
    me.printPreview = function () {
       
        var prm = [
                   me.data.PartNo,
                   me.data.Location,
                   me.data.WarehouseCode,
                   me.data.MovingCode,
                   me.data.ABCCode,
                   me.data.OnHand,
                   me.data.HargaJual,
                   me.data.HargaCost,
                   me.data.OrderBy,
                   me.data.TypeOfGoods
        ];
        Wx.showPdfReport({
            id: "SpRpSum011",
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.PartNo = '%';
        me.data.Location = '%';
        me.data.OrderBy = 'PartNo';
        me.data.OnHand = '> 0';
        me.data.HargaJual = '> 0';
        me.data.HargaCost = '> 0';
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
        title: "Daftar Part Per Nomor Part",
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
                        { text: "No. PART", name: "PartNo", model: "data.PartNo", cls: "span3", placeHolder: "Part No", type: "popup", click: "PartLkp(1)" },
                        { name: "Location", model: "data.Location", text: "Location", cls: "span3 full" },
                        { name: "WarehouseCode", model: "data.WarehouseCode", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Gudang", datasource: "comboWRCD"},
                        { name: "MovingCode", opt_text: "", cls: "span3 full", type: "select2", text: "Moving Code", datasource: "MovingCode" },
                        { name: "ABCCode", opt_text: "", cls: "span3 full", type: "select2", text: "ABC Code", datasource: "ABCCode" },
                        { name: "OnHand", opt_text: "", cls: "span3 full", type: "select2", text: "On Hand", datasource: "Comparison" },
                        { name: "HargaJual", opt_text: "", cls: "span3 full", type: "select2", text: "Harga Jual", datasource: "Comparison" },
                        { name: "HargaCost", opt_text: "", cls: "span3 full", type: "select2", text: "Harga Cost", datasource: "Comparison" },
                        { name: "OrderBy", opt_text: "", cls: "span3 full", type: "select2", text: "Sort By", datasource: "OrderBy" },
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