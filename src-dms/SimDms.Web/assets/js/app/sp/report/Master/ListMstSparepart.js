"use strict";
function spRptLstMstSparePart($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

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

    me.SwitchChange = function (o, v) {
        console.log(o);
        console.log(v);
    }


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
        var prm = [($("#StatusPartY").prop('checked') ? 'AKTIF' : 'ALL'),
                   me.data.WarehouseCode,
                   ($("#OnHandY").prop('checked') ? '1' : '0'),
                   me.data.TypeOfGoods,
                   me.data.PartNo1,
                   me.data.PartNo2,
                   0];
        
        Wx.showPdfReport({
            id: "SpRpMst001",
            pparam:prm.join(','),
        rparam: "semua",
        type: "devex"
    });
}

me.initialize = function ()
{
    me.data = {};
    me.data.PartNo = true;
    me.data.PartNo1 = '';
    me.data.PartNo2 = '';
    me.data.OnHand = false;
    me.data.StatusPart = false;
    me.data.WarehouseCode='';

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
        title: "Daftar Master Sparepart",
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
                        { name: "WarehouseCode", model:"data.WarehouseCode", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Gudang", datasource: "comboWRCD" ,validasi:"required"},
                        { name: "TypeOfGoods", model:"data.TypeOfGoods",opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Tipe Part", datasource: "comboTPGO" },
                        
                        {
                            text: "Status No Part", type: "controls",
                            items: [
                                { name: "StatusPart",  model:"data.StatusPart",type: "switch", cls: "full", float: "left" },
                                { type: "label", text: "* Check for active", cls: "span1 switchlabel" }

                            ]
                        },
                        {
                            text: "OnHand", type: "controls",
                            items: [
                                { name: "OnHand", model:"data.OnHand",type: "switch", cls: "full", float: "left" },
                                { type: "label", text: "* Check for onhand >0", cls: "span1 switchlabel" }

                            ]},
                        { text: "No. Part",type: "controls",             
                            items: [
                                    { name: "PartNo",  text: "No. Part", cls:"span1",type: "switch", float: "left" },                                    
                                    { name: "PartNo1", model: "data.PartNo1", cls: "span1", placeHolder: "Part No", readonly: true, type: "popup", click: "PartLkp(1)", disable: "data.PartNo" },
                                    { name: "PartNo2", model: "data.PartNo2", cls: "span1", placeHolder: "Part No", readonly: true, type: "popup", click: "PartLkp(2)", disable: "data.PartNo" }
                            ]}
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