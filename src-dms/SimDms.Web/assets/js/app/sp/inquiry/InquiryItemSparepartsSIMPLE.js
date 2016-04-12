var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spInquiryItemSparepartsSIMPLEController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SparepartBrowse",
            title: "Sparepart Inquiry",
            manager: spManager,
            query: "SparePartInquiry",
            defaultSort: "PartNo asc",
            resizable: true,
            columns: [
                { field: "PartNo", title: "Part No", width: 140 },
                { field: "PartName", title: "Part Name", width: 200 },
                { field: "WarehouseCode", title: "Warehouse Code", width: 120 },
                { field: "LocationCode", title: "Location Code", width: 110 },
                {
                    field: "QtyAvail", title: "Qty Available", width: 100, type: "number",
                    template: '<div style="text-align:right;">#= kendo.toString(QtyAvail, "n0") #</div>'
                },
                {
                    field: "OnOrder", title: "On Order", width: 90, type: "number",
                    template: '<div style="text-align:right;">#= kendo.toString(OnOrder, "n0") #</div>'
                },
               {
                   field: "OrderUnit", title: "Order Unit", width: 90, type: "number",
                   template: '<div style="text-align:right;">#= kendo.toString(OrderUnit, "n0") #</div>'
               },
                {
                    field: "RetailPriceInclTax", title: "Harga + PPn", width: 115, type: "number",
                    template: '<div style="text-align:right;">#= kendo.toString(RetailPriceInclTax, "n0") #</div>'
                },
                { field: "IsGenuinePart", title: "SGP", width: 50 },
                {
                    field: "RetailPrice", title: "Retail Price", width: 100,
                    template: '<div style="text-align:right;">#= kendo.toString(RetailPriceInclTax, "n0") #</div>'
                },
                { field: "ProductType", title: "Product Type", width: 100 },
               { field: "PartCategory", title: "Part Category", width: 110 },
               { field: "CategoryName", title: "Category Name", width: 120 },
               { field: "IsActive", title: "IsActive", width: 150 },
               { field: "SupplierName", title: "Supplier Name", width: 250 },
               { field: "TypeOfGoods", title: "TypeOfGoods", width: 50 },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.GetPartInfo(result.PartNo);
                me.isSave = false;
                me.Apply();
            }
        });
    };

    me.GetPartInfo = function(partNo)
    {

        var src = "sp.api/inquiry/DetailOfPartInquiry?HQ=0&PartNo=" + partNo;    

        $http.post(src)
            .success(function(v, status, headers, config){
                if(v.success)
                {
                    angular.extend(me.data,v.data);
                    angular.extend(me.data,v.info);
                    angular.extend(me.data,v.price);
                    me.data.QtyAlokasi = (me.data.AllocationSP + me.data.AllocationSL + me.data.AllocationSR);
                    me.data.QtyAvailable = (me.data.OnHand - me.data.QtyAlokasi - me.data.ReservedSL - me.data.ReservedSP - me.data.ReservedSR);
                    me.data.DemandAverageDay = (me.data.DemandAverage);
                    me.data.DemandAverageMonth = (me.data.DemandAverage * 30);
                    me.data.SupplierName = v.SupplierName;
                    Wx.Success("Loading data (" + me.data.PartNo + ") ...");                    
                } else 
                {
                    // show an error message
                    MsgBox(v.message, MSG_ERROR);
                }
            }).error(function(e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
        });    
    }

    me.PartValidate = function()
    {
        me.GetPartInfo(me.data.PartNo);
    }


    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Inquiry Item Spareparts SIMPLE",
        xtype: "panel",
        items: [
               
                { name: "PartNo", text: "No. Part", cls: "span4 ", type: "popup", btnName: "btnX",  click:"browse()", keypress:"{13: 'PartValidate()'}"},
                { name: "PartName", text: "Nama Part", cls: "span8", readonly: true },
                { type: "divider" },               
                { name: "MovingCode", text: "Moving Code", cls: "span4", readonly: true },
                { name: "BornDate", text: "Tgl. Beli Pertama", cls: "span4", readonly: true, type: "ng-datepicker"  },
                { name: "ABCClass", text: "ABC Class", cls: "span4", readonly: true },
                { name: "LastPurchaseDate", text: "Tgl. Beli Terakhir", cls: "span4", readonly: true, type: "ng-datepicker"  },
                { name: "OrderUnit", text: "Unit Beli", cls: "span4 number", readonly: true },
                { name: "LastDemandDate", text: "Tgl. Permintaan Terakhir", cls: "span4", readonly: true, type: "ng-datepicker"  },
                { name: "SalesUnit", text: "Unit Jual", cls: "span4 number", readonly: true },
                { name: "DemandAverage", text: "Rata-rata Permintaan", cls: "span4 number", readonly: true },
                { name: "OrderPointQty", text: "Qty Max", cls: "span4 number", readonly: true },
                { name: "PurchasePrice", text: "Harga Beli", cls: "span4 number", readonly: true },
                { name: "SafetyStockQty", text: "Qty Min", cls: "span4 number", readonly: true },
                { name: "RetailPrice", text: "Harga Jual", cls: "span4 number", readonly: true },
                { name: "OrderPointQty", text: "Qty O/P", cls: "span4 number", readonly: true },
                { name: "RetailPriceInclTax", text: "Harga Jual + PPn", cls: "span4 number", readonly: true },
                { name: "SupplierCode", text: "Pemasok", cls: "span4", readonly: true },
                { name: "CostPrice", text: "Harga Pokok", cls: "span4 number", readonly: true },
                { name: "SupplierName", text: "Nama Pemasok", cls: "span8", readonly: true },
                { name: "OnHand", text: "Qty On Hand", cls: "span4 number", readonly: true },
                { name: "QtyAlokasi", text: "Qty Alokasi", cls: "span4 number", readonly: true },
                { name: "QtyAvailable", text: "Qty Tersedia", cls: "span4 number", readonly: true },
                { name: "OnOrder", text: "Qty On Order", cls: "span4 number", readonly: true },         
            ] 
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spInquiryItemSparepartsSIMPLEController");
    }

});