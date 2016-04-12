var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


"use strict";

function spItemLokasiController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.$watch('pilihan', function(newValue,oldValue)
    {
         if (newValue !== oldValue)
         {
             me.$broadcast(newValue);   
             // switch (newValue)
             // {
             //    case 'test1':
             //        MsgBox("tab test1 dipilih (switch)");
             //        // do work
             //        break;
             //    default:
             // }         
         }   
    });

      me.$on('test1', function() {
            // MsgBox("tab test1 dipilih");
            Wx.showPdfReport({
                id:"SpRpTrn011Short",
                pparam: "'6006406','6006401','FPJ/11/000025','FPJ/11/000025','300', '10','0'",
                rparam: "?"
            });
      });      


      me.$on('test2', function() {
            MsgBox("tab test2 dipilih", MSG_PRIMARY);
      });

      me.$on('test3', function() {
            
            layout.loadAjaxLoader();
      });

      me.$on('test4', function() {
            MsgBox("tab test4 dipilih", MSG_ERROR);
      });

      me.$on('test5', function() {
            MsgBox("tab test5 dipilih", MSG_SUCCESS);
      });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "ItemLocationBrowse",
            title: "Item Location Browse",
            manager: spManager,
            query: "MasterItemLocationBrowse",
            defaultSort: "PartNo asc",
            columns: [
                { field: "PartNo", title: "Part No" },
                { field: "PartName", title: "Part Name" },
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "WarehouseCode", title: "Warehouse Code" },
                { field: "LocationCode", title: "Location Code" },
                { field: "PartCategory", title: "Part Category" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                me.Apply();               
            }
        });
    };


    me.PartNo = function () {
        var lookup = Wx.blookup({
            name: "PartNoLookup",
            title: "Lookup Part",
            manager: spManager,
            query: "SparePartLookup",
            defaultSort: "PartNo asc",
            columns: [
                {field: "PartNo", title: "No Part"},
                {field: "PartName", title: "Nama Part"},
                {field: "SupplierCode", title: "Kode Suplier"},
                {field: "IsGenuinePart", title: "Prd Suzuki"},
                {field: "ProductType", title: "Tipe Produk"},
                {field: "PartCategory", title: "Kategori"},
                {field: "CategoryName", title: "Nama Kategori"}
            ],            
        });
        lookup.dblClick(function (data) {
            if (data != null) {              
                me.data.PartNo = data.PartNo;
                me.data.PartName = data.PartName;
                me.isSave = false;
                me.Apply();
            }
        });
    };

    me.WarehouseCode = function () {

        var lookup = Wx.blookup({
            name: "WarehouseCodeLookup",
            title: "Lookup Warehouse",
            manager: spPersediaanManager,
            query: "WarehouseCode",
            defaultSort: "warehousecode asc",
            columns: [
                { field: "warehousecode", title: "Warehouse Code" },
                { field: "lookupvaluename", title: "Warehouse Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.WarehouseCode = data.warehousecode;
                me.data.WarehouseName = data.lookupvaluename;
                me.isSave = false;
                me.Apply();
            }
        });
    };


    me.saveData = function (e, param) {

        if (me.data.WarehouseCode.substr(0, 1) == "X") {
            Wx.Error("Warehouse code is invalid (Must not starting with X)");
            return false;
        }

        if (me.data.WarehouseCode > 99) {
            Wx.Error("Warehouse code must not greater than 99");
            return false;
        }

        $http.post('sp.api/MstItemLoc/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                   
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox("Connection to the server failed...", MSG_ERROR);
            });
    };

 



    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/MstItemLoc/delete', me.data).
                    success(function (dl, status, headers, config) {
                        if (dl.success) {
                            Wx.Info("Record has been deleted...");
                            me.init();
                        } else {
                            MsgBox(dl.message, MSG_ERROR);
                        }
                    }).
                    error(function (e, status, headers, config) {
                        MsgBox("Connection to the server failed...", MSG_ERROR);
                    });
            }
        });
    };


    me.initialize = function () {
        me.detail = {};
        // display print button when data loaded
        me.isPrintAvailable = true;
    };

    me.start();

    me.testX = function()
    {
        Wx.showForm({url:"gn/master/customer2"});
    }


    me.printPreview = function()
    {
        Wx.showPdfReport ({
            id:"SpRpTrn011Short",
            pparam: "'6006406','6006401','FPJ/11/000025','FPJ/11/000025','300', '10','0'",
            rparam: "?"
        });
    }

}

$(document).ready(function () {		
    var options = {
        title: "Item Lokasi",
        xtype: "panels",
        toolbars:WxButtons,
        panels: [
            {
                name: "pnlInfoPart",
                title: "Informasi Part",
				items: [
                        {
                            type: "optionbuttons",
                            name: "pilih",
                            model: "pilihan",
                            cls: "span8",
                            items: [
                                { name: "test1", text: "Opsi 1" },
                                { name: "test2", text: "Opsi 2" },
                                { name: "test3", text: "Opsi 3" },
                                { name: "test4", text: "Opsi 4" },
                                { name: "test5", text: "Opsi 5" },
                            ]
                        }, 
                        {
                            text: "No. Part",
                            type: "controls",
							required: true,
                            items: [
                                { name: "PartNo", cls: "span2", placeHolder: "No. Part", type: "popup", btnName: "btnPartNo", click:"PartNo()", required: true , validasi: "required"},
                                { name: "PartName", cls: "span6", placeHolder: "Nama Part", readonly: true , required: true}
                            ]
                        },
					    {
                            text: "Gudang",
                            type: "controls",
							required: true,
                            items: [
                                { name: "WarehouseCode", 
                                cls: "span2", placeHolder: "Kode Gudang", type: "popup", btnName: "btnWarehouseCode", 
                                click: "testX()", required: true, validasi: "required,regex(^[^X],Do not start with X),maxVal(99)" },
                                { name: "WarehouseName", cls: "span6", placeHolder: "Nama Gudang", readonly: true, required: true }
                            ]
                        }
                ]
            },
            {
                name: "pnlInfoPartLokasi",
                title: "Informasi Lokasi",
                items: [
				   { name: "LocationCode",  text: "Lokasi Utama", cls: "span4 full", 
                     placeHolder: "Lokasi Utama", required: true, validasi: "required"},  				   
                   { name: "LocationSub1", text: "Sub Lokasi 1", cls: "span4", validasi: "required,min(5)" },
				   { name: "LocationSub4", text: "Sub Lokasi 4", cls: "span4", validasi: "number,minVal(10),maxVal(50)" }, 	
                   { name: "LocationSub2", text: "Sub Lokasi 2", cls: "span4", validasi: "required,number" },
                   { name: "LocationSub5", text: "Sub Lokasi 5", cls: "span4", validasi: "required,max(10)" },
				   { name: "LocationSub3", text: "Sub Lokasi 3", cls: "span4", validasi: "required,number" },                                   
                   { name: "LocationSub6", text: "Sub Lokasi 6", cls: "span4" },
                   { type: "div", name: "samplewx", cls: "span8" }

                ]
            }, 
            {
                name: "TargetExtBody",
                xtype: "wxtable"
            }
        ],
    };  

	Wx = new SimDms.Widget(options);
	Wx.default = {};
	Wx.render(init);

	function init(s) {
	    SimDms.Angular("spItemLokasiController");
	}

});

//   renderTo: 'ext-target',
$("div#ext-target").html("");

Ext.create('Ext.Component', {
    id: "widget",
    renderTo: 'ext-target',
    html: '<span>My Content</span>'
  });
