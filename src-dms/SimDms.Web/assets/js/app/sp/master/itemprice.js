var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "itemPriceBrowse",
            title: "Item Price Browse",
            manager: spManager,
            query: "ItemPriceBrowse",
            defaultSort: "PartNo asc",
            columns: [
            { field: "PartNo", title: "PartNo", width: 160 },
            { field: "PartName", title: "Part Name", width: 280 },
            {
                field: "PurchasePrice", title: "Purchase Price", width: 130,
            template: '<div style="text-align:right;">#= kendo.toString(PurchasePrice, "n0") #</div>'        },
            {
                field: "RetailPriceInclTax", title: "Retail Price Incl. Tax", width: 130,
            template: '<div style="text-align:right;">#= kendo.toString(RetailPriceInclTax, "n0") #</div>'        },
            { field: "IsGenuinePart", title: "IsGenuinePart", width: 130 },
            { field: "ProductType", title: "Product Type", width: 130 },
            { field: "PartCategory", title: "Part Category", width: 120 },
            { field: "CategoryName", title: "Category Name", width: 140 }

            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.PartNo = function () {
        var lookup = Wx.klookup({
            name: "PartNoLookup",
            title: "Lookup PartNo",
            url: "sp.api/ItemPrice/Select4Lookup",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "PartNo", title: "Part No", width: 160 },
                { field: "PartName", title: "Part Name", width: 280 },
                { field: "SupplierCode", title: "Supplier Code", width: 130 },
                { field: "IsGenuinePart", title: "IsGenuine Part", width: 130 },
                { field: "ProductType", title: "Product Type", width: 130 },
                { field: "PartCategory", title: "Part Category", width: 130 },
                { field: "CategoryName", title: "Category Name", width: 150 }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PartNo = data.PartNo;
                me.data.PartName = data.PartName;
                me.data.SupplierCode = data.SupplierCode;
                me.getInfoPart();
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.harga = function () { 

        me.data.RetailPrice = Math.round(me.data.RetailPriceInclTax / 1.1).toFixed();
        //me.ReformatNumber();
    }

    me.hargaCek = function () { 

         if( me.data.PurchasePrice > me.data.RetailPrice  ){
            MsgBox("Harga jual lebih kecil dari harga beli...", MSG_WARNING);
          }    
    }

    me.saveData = function (e, param) {
        $http.post("sp.api/itemprice/save", me.data)
        .success(function (result) {
            if (result.success) {
                Wx.Success("Data saved...");
                me.startEditing();
            }
            else {
                MsgBox(result.message, MSG_ERROR);
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/itemprice/delete', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        Wx.Info("Record has been deleted...");

                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    console.log(e); 
                });
            }
        });
    }

    me.getInfoPart = function () {
        var params = {
            PartNo: me.data.PartNo
        }

        $http.post("sp.api/itemprice/getRecord", params)
        .success(function (result) {
            if (result.success) {
                result.data.PartName = result.PartName;
                //me.lookupAfterSelect(result.data);
                me.data = result.data;
            }
            else {
                //$('#PartNo').val('');
                me.data.PartNo = '';
                me.data.PartName = '';
                me.data.PurchasePrice = 0;
                me.data.RetailPriceInclTax = 0;
                me.data.RetailPrice = 0;
                me.data.CostPrice = 0;
                me.data.LastPurchaseUpdate = '';
                me.data.LastRetailPriceUpdate = '';
                me.data.OldPurchasePrice = 0;
                me.data.OldCostPrice = 0;
                me.data.OldRetailPrice = 0;
                me.PartNo();
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.initialize = function () {
        $("[name = 'PartNo']").on('blur', function () {
            if ($('#PartNo').val() || $('#PartNo').val() != '') {
                me.getInfoPart();
            } else {
                me.data = {};
                //me.data.PartNo = '';
                //me.data.PartName = '';
                //me.data.PurchasePrice = '';
                //me.data.RetailPriceInclTax = '';
                //me.data.RetailPrice = '';
                //me.data.CostPrice = '';
                //me.data.LastPurchaseUpdate = '';
                //me.data.LastRetailPriceUpdate = '';
                //me.data.OldPurchasePrice = '';
                //me.data.OldCostPrice = '';
                //me.data.OldRetailPrice = '';
                me.PartNo();
            }
        });
    };

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Item Price",
        xtype: "panels",
        toolbars:WxButtons,
        panels: [
            {
                name: "pnlInfoPart",
                title: "Price Information Part",
				items: [
                        {
                            text: "Part No",
                            type: "controls",
							required: true,
                            items: [
                                { name: "PartNo", cls: "span2", placeHolder: "Part No", type: "popup", btnName: "btnPartNo", readonly: false, validasi: "required", required: true,click:"PartNo()" },
                                { name: "PartName", cls: "span6", placeHolder: "Part Name", readonly: true , required: true}
                            ]
                        },						
					   { name: "PurchasePrice", text: "Purchase Price",cls:" number-int span4", placeHolder: "0" },
					   { name: "RetailPriceInclTax", text: "Retail Price Incl Tax", cls: " number-int span4", placeHolder: "0", change:"harga()",event:"{blur:'hargaCek()'}" },
					   { name: "CostPrice", text: "Cost Price", cls: "span4  number-int", readonly: true, placeHolder: "0" },
					   { name: "RetailPrice", text: "Retail Price", cls: "span4  number-int", readonly: true, placeHolder: "0" },
					   { name: "LastPurchaseUpdate", text: "Last Purchase Update", type: "ng-datepicker", cls: "span4" },
					   { name: "LastRetailPriceUpdate", text: "Last Retail Price Update", type: "ng-datepicker", cls: "span4" }			   
                ]
            },
            {
                name: "pnlHistoryHarga",
                title: "History Price",
                items: [
                   { name: "OldPurchasePrice", text: "Purchase Price old", cls: " number-int span4", readonly: true, placeHolder: "0" },
				   { name: "OldCostPrice", text: "CostPrice Old", cls: " number-int span4", readonly: true, placeHolder: "0" },
                   { name: "OldRetailPrice", text: "RetailPrice Old", cls: " number-int span4", readonly: true, placeHolder: "0" }
                ]
            },

 
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spItemPriceController");
    }

});