"use strict"; //Reportid SpRpTrn017
function spRptLstMstSparePart($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/Months?').
    success(function (data, status, headers, config) {
        me.Months = data;
    });

    $http.post('sp.api/Combo/Years?').
   success(function (data, status, headers, config) {
       me.Years = data;
   });


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


    me.TypeReports = [
        {"value":'0', "text":'QTY'},
        {"value":'1', "text":'Nilai' }
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
                    me.data.Months + '/ 01/' + me.data.Years,
                    me.data.WarehouseCode,
                    me.data.TypeOfGoods,
                    me.data.TypeReports
                  ];
        Wx.showPdfReport({
            id: "SpRpTrn017",
            pparam:prm.join(','),
        rparam: "semua",
        type: "devex"
    });
}

me.initialize = function ()
{
    me.data = {};
    me.data.WarehouseCode = '';
    me.data.Months = new Date().getMonth() + 1;
    me.data.Years = new Date().getFullYear();
    me.data.TypeReports = '0';
    $http.get('breeze/sparepart/CurrentUserInfo').
      success(function (dl, status, headers, config) {
          me.data.CompanyCode =  dl.CompanyCode;
          me.data.BranchCode = dl.BranchCode;
              
      });

    me.isPrintAvailable = true;
}


me.start();

}


$(document).ready(function () {
    var options = {
        title: "ANALISA SPAREPART MENURUT ABC CLASS",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlA",                
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: 'Month', model: "data.Months", text: "Month", type: "select2", cls: "span4", optionalText: "-- SELECT MONTH --", datasource: "Months" },
                        { name: 'Year', model: "data.Years", text: "Year", type: "select2", cls: "span2", optionalText: "-- SELECT YEAR --", datasource: "Years" },
                        { name: "WarehouseCode", model: "data.WarehouseCode", cls: "span4 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Gudang", datasource: "comboWRCD", validasi: "required", opt_text: "Pilih Gudang" },
                        { name: "TypeOfGoods", model:"data.TypeOfGoods", opt_text: "-- SELECT ALL --", cls: "span4 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Tipe Part", datasource: "comboTPGO" },
                        { name: "TypeReports", opt_text: "", cls: "span4 full", type: "select2", text: "Type Report", datasource: "TypeReports" }
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