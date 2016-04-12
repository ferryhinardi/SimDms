var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spInquiryItemSparepartController($scope, $http, $injector) {

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

                    me.loadTableData(me.gridLokasi, v.lokasi);
                    me.loadTableData(me.gridSubsitusi, v.subsitusi);  
                    me.loadTableData(me.gridModel, v.model);

                    if (v.demandandsales.length > 10 )
                    {
                        me.gridDemandAndSales.define("autoheight",false);
                        me.gridDemandAndSales.define("height",300);
                        me.gridDemandAndSales.define("scrollY",true);

                        me.gridDemandAndSales.clearSelection();   
                        if (me.gridDemandAndSales.clearAll) {
                            me.gridDemandAndSales.clearAll();        
                        }

                        me.gridDemandAndSales.define("data", v.demandandsales);
                        me.gridDemandAndSales.resize();
                    } else 
                        me.loadTableData(me.gridDemandAndSales, v.demandandsales);

                    console.log ('grid on demand: ' + v.demandandsales.length);



                    me.loadTableData(me.gridOnOrder, v.onorder);  

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


    me.gridLokasi = new webix.ui({
        container:"Lokasi",
        view:"wxtable", 
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 320,
        columns:[
            { id:"WarehouseCode",  header:"WH Code", width:100 },
            { id:"LookUpValueName",  header:"Warehouse Name", width:200 },
            { id:"LocationCode",  header:"Location Code", width:120 },
            { id:"OnHand",  header:"OnHand", width:100, css:"rightcell", format: me.decimalFormat },
            { id: "AllocationSP", header: "Allocation SP", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "AllocationSP", header: "Allocation SP", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "AllocationSL", header: "Allocation SL", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "AllocationSR", header: "Allocation SR", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "OnOrder", header: "On Order", width: 100, css: "rightcell", format: me.decimalFormat },
            { id: "InTransit", header: "In Transit", width: 100, css: "rightcell", format: me.decimalFormat },
            { id: "BorrowQty", header: "Borrow Qty", width: 110, css: "rightcell", format: me.decimalFormat },
            { id: "BorrowedQty", header: "Borrowed Qty", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "BackOrderSP", header: "BackOrder SP", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "BackOrderSL", header: "BackOrder SL", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "BackOrderSR", header: "BackOrder SR", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "ReservedSP", header: "Reserved SP", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "ReservedSL", header: "Reserved SL", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "ReservedSR", header: "Reserved SR", width: 120, css: "rightcell", format: me.decimalFormat, fillspace: true }
        ]      
    });

    me.gridSubsitusi = new webix.ui({
        container:"Subsitusi",
        view:"wxtable", 
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 320,
        columns:[
            { id:"No",  header:"No", width:60, css:"rightcell"  },
            { id:"PartNo",  header:"PartNo", width:170 },
            { id:"PartName",  header:"Part Name", width:250 },
            { id:"InterchangeCode",  header:"Interchange Code", width:100 },
            { id:"UnitConversion",  header:"Unit Conversion", width:100, css:"rightcell"  },
            { id:"OnHand",  header:"On Hand", width:100, css:"rightcell"  },
            { id:"AllocationSP",  header:"Allocation SP", width:100, css:"rightcell"  },
            { id:"OnOrder",  header:"On Order", width:100, css:"rightcell"  },
            { id:"InTransit",  header:"InTransit", width:100 , css:"rightcell" },
            { id:"Received",  header:"Received", width:100, css:"rightcell"  },
            { id:"isRegister",  header:"Is Register?", width:100, fillspace:true } 
        ]      
    });


    me.gridDemandAndSales = new webix.ui({
        container:"dnsales",
        view:"wxtable", 
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 320,
        columns:[
            { id:"Year",  header:"Year", width:100, css:"centrecell"  },
            { id:"Month",  header:"Month", width:100, css:"centrecell"  },
            { id: "DemandFreq", header: "Demand Freq", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "DemandQty", header: "Demand Qty", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "SalesFreq", header: "Sales Freq", width: 120, css: "rightcell", format: me.decimalFormat },
            { id: "SalesQty", header: "Sales Qty", width: 120, css: "rightcell", format: me.decimalFormat, fillspace: true }
        ]      
    });


    me.gridModel = new webix.ui({
        container:"Model",
        view:"wxtable", 
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 320,
        columns:[
            {id: "ModelCode", header: "Model Code", fillspace: true },
            {id: "ModelName", header: "Model Name", fillspace: true } 
        ]      
    });

    me.gridOnOrder = new webix.ui({
        container:"onOrder",
        view:"wxtable", 
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 320,
        scheme:{
            $init:function(obj){
                obj.POS_Date = obj.POSDate.replace('T',' ');
            }
        },
        columns:[
            { id:"POSNo",  header:"POS No.", width:120, css:"centrecell"  },
            { id:"POS_Date",  header:"POS Date", width:120, format:webix.Date.dateToStr("%d %M %Y"), sort:"date" },
            { id: "OnOrder", header: "On Order", width: 100, css: "rightcell", format: me.decimalFormat },
            { id: "InTransit", header: "InTransit", width: 100, css: "rightcell", format: me.decimalFormat },
            { id: "Received", header: "Received", width: 100, css: "rightcell", format: me.decimalFormat },
            { id:"WRSNo",  header:"WRS No.", width:120 },
            { id:"SupplierName",  header:"Supplier Name", fillspace:true }
        ]      
    });

    me.OnTabChange = function (e, id) {
        switch (id) {
            case "tA": me.gridLokasi.adjust(); break;
            case "tB": me.gridSubsitusi.adjust(); break;
            case "tC": me.gridDemandAndSales.adjust(); break;
            case "tD": me.gridModel.adjust(); break;
            case "tE": me.gridOnOrder.adjust(); break;
            default:
        }
    };

    me.initialize = function () {
        me.gridLokasi.adjust();
        me.gridSubsitusi.adjust();
        me.gridDemandAndSales.adjust();
        me.gridModel.adjust();
        me.gridOnOrder.adjust();
    };

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Inquiry Item Spareparts",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "PartNo", text: "No. Part", cls: "span4 ", type: "popup", btnName: "btnX",  click:"browse()", keypress:"{13: 'PartValidate()'}"},
                        { name: "PartName", text: "Nama Part", cls: "span8", readonly: true },
                    ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [ 
                    {name: "tA", text: "Lokasi"},
                    {name: "tB", text: "Subsitusi"},
                    {name: "tC", text: "Demand and Sales"},
                    {name: "tD", text: "Model"},
                    {name: "tE", text: "On Order"},
                    {name: "tI", text: "Informasi Item"},
                ] 
            },
            {
                name: "Lokasi",
                title: "Lokasi",
                cls: "tabpage1 tA",
                xtype: "wxtable"
            }, 
            {
                name: "Subsitusi",
                title: "Subsitusi",
                cls: "tabpage1 tB",
                xtype: "wxtable"
            },
            {
                name: "dnsales",
                title: "Demand and Sales",
                cls: "tabpage1 tC",
                xtype: "wxtable"
            },
            {
                name: "Model",
                title: "Model",
                cls: "tabpage1 tD",
                xtype: "wxtable"
            },
            {
                name: "onOrder",
                title: "On Order",
                cls: "tabpage1 tE",
                xtype: "wxtable"
            },
            {
                name: "InformasiItem",
                title: "Informasi Item",
                cls: "tabpage1 tI",
                items: [                            
                    { name: "MovingCode", text: "Moving Code", cls: "span4", readonly: true },
                    { name: "BornDate", text: "Tgl. Beli Pertama", cls: "span4", readonly: true, type: "ng-datepicker"  },
                    { name: "ABCClass", text: "ABC Class", cls: "span4", readonly: true },
                    { name: "LastPurchaseDate", text: "Tgl. Beli Terakhir", cls: "span4", readonly: true, type: "ng-datepicker"  },
                    { name: "OrderUnit", text: "Unit Beli", cls: "span4 number", readonly: true },
                    { name: "LastDemandDate", text: "Tgl. Permintaan Terakhir", cls: "span4", readonly: true, type: "ng-datepicker"  },
                    { name: "SalesUnit", text: "Unit Jual", cls: "span4 number", readonly: true },
                    {
                        text: "Rata-rata Permintaan",
                        type: "controls",
                        cls:  "span4",
                        items:[
                            { name: "DemandAverageDay", text: "", cls: "span3", readonly: true },
                            { name: "DemandAverageDayLabel", text: "(hari)", cls: "span1 label-valign", type: "label" },                            
                            { name: "DemandAverageMonth", text: "", cls: "span3", readonly: true },
                            { name: "DemandAverageMonthLabel", text: "(bulan)", cls: "span1 label-valign", type: "label" },
                        ]
                    },
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
             },
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spInquiryItemSparepartController"); 
    }

});