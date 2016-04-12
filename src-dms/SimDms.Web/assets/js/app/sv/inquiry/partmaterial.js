$(document).ready(function () {
    var options = {
        title: "Part and Material",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: 'btnBrowse', text: 'Browse', icon: 'icon-search' }
        ],
        panels: [
            {
                name: "pnlPartMaterialInfo",
                title: "Part and Material Information",
                items: [
                    { name: "PartNo", text: "Part No.", cls: "span4", type: "popup", readonly: true },
                    { name: "PartName", text: "Part Name", cls: "span8", readonly: true },
                    { name: "LocationCode", text: "Location", cls: "span4", readonly: true },
                    { name: "ProductType", text: "Product Type", cls: "span4", readonly: true },
                    { name: "LocationSub1", cls: "span4", readonly: true },
                    { name: "TypeOfGoodsDesc", text: "Part Type", cls: "span4", readonly: true },
                    { name: "LocationSub2", cls: "span4 full", readonly: true },
                    { name: "LocationSub3", cls: "span4", readonly: true },
                    { name: "MovingCode", text: "MC", cls: "span4", readonly: true },
                    { name: "LocationSub4", cls: "span4", readonly: true },
                    { name: "ABCClass", text: "ABC Class", cls: "span4", readonly: true },
                    { name: "LocationSub5", cls: "span4", readonly: true },
                    { name: "StatusDesc", text: "Status", cls: "span4", readonly: true }
                ]
            },
            {
                title: "Model Information",
                xtype: "table",
                tblname: "tblModelnfo",
                columns: [
                    { name: "Code", text: "Code", width: 50 },
                    { name: "Description", text: "Description" },
                ]
            },
            {
                name: "pnlStockInfo",
                title: "Stock Information",
                items: [
                    { name: "OnHand", text: "On Hand", cls: "span4 number", readonly: true },
                    { name: "PartBO", text: "Back Order (Parts)", cls: "span4 number", readonly: true },
                    { name: "PartAllocated", text: "Allocated (Parts)", cls: "span4 number", readonly: true },
                    { name: "ServiceBO", text: "Back Order (Service)", cls: "span4 number", readonly: true },
                    { name: "ServiceAllocated", text: "Allocated (Service)", cls: "span4 number", readonly: true },
                    { name: "UnitsBO", text: "Back Order (Units)", cls: "span4 number", readonly: true },
                    { name: "UnitsAllocated", text: "Allocated (Units)", cls: "span4 number", readonly: true },
                    { name: "PartReserved", text: "Reserved (Parts)", cls: "span4 number", readonly: true },
                    { name: "OnOrder", text: "On Order", cls: "span4 number", readonly: true },
                    { name: "ServiceReserved", text: "Reserved (Service)", cls: "span4 number", readonly: true },
                    { name: "Intransit", text: "Intransit", cls: "span4 number", readonly: true },
                    { name: "UnitsReserved", text: "Reserved (Units)", cls: "span4 number", readonly: true }
                ]
            },
            {
                name: "pnlPriceInfo",
                title: "Price Information",
                items: [
                    { name: "RetailPriceInclTax", text: "Price + Tax", cls: "span4 number", readonly: true }
                ]
            },
            {
                title: "Subtitude / Modification / Interchange",
                xtype: "table",
                tblname: "tblInterchange",
                columns: [
                    { name: "No", text: "No.", width: 25 },
                    { name: "InterChangeCode", text: "Interchange" },
                    { name: "NewPartNo", text: "New Part" },
                    { name: "UnitConversion", text: "Qty" }
                ]
            },
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.default = {};

    widget.render(function () {
        $.post('sv.api/partmaterial/default', function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    $('#btnNew').on('click', function (e) {
        widget.clearForm()
    });
    $('#btnPartNo,#btnBrowse').on('click', browsePart);

    function browsePart() {
        var lookup = widget.klookup({
            name: "tblPartLookup",
            title: "Master Inquiry Part Lookup",
            url: "sv.api/grid/inquirypart",
            serverBinding: true,
            filters: [
                { name: "ftlPartNo", text: "Part No.", cls: "span4 full" },
                { name: "ftlPartType", text: "Part Type", cls: "span4 full" },
                { name: "ftlPartName", text: "Part Name", cls: "span4 full" },
                { name: "ftlStatus", text: "Status", cls: "span4 full" },
                {
                    text: "Dealer",
                    type: "controls",
                    items: [
                        {
                            name: "fltDealerPart", type: "select", text: "Status", cls: "span2", items: [
                                    { value: "0", text: "SHOW ALL" },
                                    { value: "1", text: "SHOW ONLY DEALER PART" }
                            ]
                        },
                    ]
                }
            ],
            columns: [
                { field: 'PartNo', title: 'Part No.' },
                { field: 'PartName', title: 'Part Name' },
                { field: 'Available', title: 'Available' },
                { field: 'RetailPriceInclTax', title: 'Price + Tax' },
                { field: 'ProductType', title: 'Product Type' },
                { field: 'TypeOfGoodsDesc', title: 'Part Type' },
                { field: 'StatusDesc', title: 'Status' }
            ]
        });

        lookup.dblClick(function (data) {
            widget.post('sv.api/partmaterial/get', { PartNo: data.PartNo }, function (result) {
                populateData(data, result);
            });
        });
    }

    function populateData(data, result) {
        var items = result.item || {};
        var itemlocs = result.itemloc || {};
        var iteminfos = result.iteminfo || {};
        var itemprices = result.itemprice || {};

        var datas = {
            PartNo: items.PartNo,
            PartName: iteminfos.PartName,
            TypeOfGoodsDesc: data.TypeOfGoodsDesc,
            MovingCode: items.MovingCode,
            ABCClass: items.ABCClass,
            StatusDesc: data.StatusDesc,
            ProductType: items.ProductType,
            LocationCode: itemlocs.LocationCode,
            LocationSub1: itemlocs.LocationSub1,
            LocationSub2: itemlocs.LocationSub2,
            LocationSub3: itemlocs.LocationSub3,
            LocationSub4: itemlocs.LocationSub4,
            LocationSub5: itemlocs.LocationSub5,
            OnHand: items.OnHand,
            OnOrder: items.OnOrder,
            Intransit: items.InTransit,
            PartBO: itemlocs.BackOrderSP,
            ServiceBO: itemlocs.BackOrderSR,
            UnitsBO: itemlocs.BackOrderSL,
            PartAllocated: itemlocs.AllocationSP,
            ServiceAllocated: itemlocs.AllocationSR,
            UnitsAllocated: itemlocs.AllocationSL,
            PartReserved: itemlocs.ReservedSP,
            ServiceReserved: itemlocs.ReservedSR,
            UnitsReserved: itemlocs.ReservedSL,
            RetailPriceInclTax: itemprices.RetailPriceInclTax
        }

        widget.populate(datas);
        widget.populateTable({ selector: "#tblModelnfo", data: result.modelinfo });
        widget.populateTable({ selector: "#tblInterchange", data: result.itemmod });
        
    }
});
