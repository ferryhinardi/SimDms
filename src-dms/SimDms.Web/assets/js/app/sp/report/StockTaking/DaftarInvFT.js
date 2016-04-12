"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.OrderBy = [
          { "value": 'PartNo', "text": 'Part Number' },
          { "value": 'Location', "text": 'Location' }
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
                if (x == 1) {
                    me.data.PartNo1 = data.PartNo;                    
                }
                else {                    
                    me.data.PartNo2 = data.PartNo;
                }
                me.Apply();
            }
        });
    }


    me.printPreview = function () {
        if (me.data.WarehouseCode == '')
            return;
        var prm = [
                   me.data.WarehouseCode,
                   me.data.OrderBy,
                   me.data.PartNo1,
                   me.data.PartNo2
                   ];
        Wx.showPdfReport({
            id: "SpRpTrn023",
            pparam:prm.join(','),
        rparam: "semua",
        type: "devex"
    });
}

me.initialize = function ()
{
    me.data = {};
    me.data.PartNo1 = '';
    me.data.PartNo2 = '';
    //me.data.WarehouseCode='';

    $http.get('breeze/sparepart/CurrentUserInfo').
      success(function (dl, status, headers, config) {
          me.data.CompanyCode =  dl.CompanyCode;
          me.data.BranchCode = dl.BranchCode; 
      });

    me.isPrintAvailable = true;

    $(".switch:last").on("click", function () {
        var name = 'PartNo';
        var value = $("#" + name + "Y").is(':checked');
        $("input[name='" + name + "']").val(value);
        if (value)
            me.data.PartNo = false
        else
            me.data.PartNo = true;

        me.Apply();
    });
}
me.start();
}


$(document).ready(function () {
    var options = {
        title: "Daftar Inventory Form/ Tag",
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
                        { name: "WarehouseCode", model: "data.WarehouseCode", text: "Warehouse", cls: "span3 full", disable: "isPrintAvailable" },
                        { text: "No. Form/ Tag", name: "PartNo1", model: "data.PartNo1", cls: "span3", placeHolder: "Part No", type: "popup", click: "PartLkp(1)" },
                        { text: "sampai dengan", name: "PartNo2", model: "data.PartNo2", cls: "span3", placeHolder: "Part No", type: "popup", click: "PartLkp(2)" },
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