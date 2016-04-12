var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


"use strict";

function spItemLokasiController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    //me.browse = function () {
    //    var lookup = Wx.blookup({
    //        name: "ItemLocationBrowse",
    //        title: "Item Location Browse",
    //        manager: spManager,
    //        query: "MasterItemLocationBrowse",
    //        defaultSort: "PartNo asc",
    //        columns: [
    //            { field: "PartNo", title: "Part No" },
    //            { field: "PartName", title: "Part Name" },
    //            { field: "SupplierCode", title: "Supplier Code" },
    //            { field: "WarehouseCode", title: "Warehouse Code" },
    //            { field: "LocationCode", title: "Location Code" },
    //            { field: "PartCategory", title: "Part Category" },
    //        ]
    //    });

    //    lookup.dblClick(function (result) {
    //        if (result != null) {
    //            me.lookupAfterSelect(result);
    //            me.isSave = false;
    //            me.Apply();

    //        }

    //    });
    //};

    me.browse = function () {

        var lookup = Wx.klookup({
            name: "ItemLocationBrowse",
            title: "Item Location Browse",
            url: "sp.api/Grid/MasterItemLocationBrowseV2?cols=" + 7,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "PartNo", title: "Part No" },
                { field: "PartName", title: "Part Name" },
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "WarehouseCode", title: "Warehouse Code" },
                { field: "WarehouseName", title: "Warehouse Name" },
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

    //me.PartNo = function () {
    //    var lookup = Wx.blookup({
    //        name: "PartNoLookup",
    //        title: "Lookup Part",
    //        manager: spManager,
    //        query: "SparePartLocationLookup",
    //        defaultSort: "PartNo asc",
    //        columns: [
    //            { field: "PartNo", title: "Part No" },
    //            { field: "AvailQty", title: "Avail Qty.", format: "{0:#,###.00}" },
    //            { field: "RetailPrice", title: "Harga Jual", format: "{0:#,###.00}" },
    //            { field: "MovingCode", title: "Mv. CD" },
    //            { field: "ABCClass", title: "ABC Class" },
    //            { field: "PartName", title: "Part Name" },
    //            //{field: "SupplierCode", title: "Supplier Code"},
    //            //{field: "IsGenuinePart", title: "IsGenuine Part"},
    //            //{field: "ProductType", title: "Product Type"},
    //            //{field: "PartCategory", title: "Part Category"},
    //            //{field: "CategoryName", title: "Category Name"}
    //        ],
    //    });
    //    lookup.dblClick(function (data) {
    //        if (data != null) {
    //            me.data.PartNo = data.PartNo;
    //            me.data.PartName = data.PartName;
    //            me.isSave = false;
    //            me.Apply();
    //        }
    //    });
    //};

    me.PartNo = function () {

        var lookup = Wx.klookup({
            name: "PartNoLookup",
            title: "Lookup Part",
            url: "sp.api/Grid/SparePartLocationLookupV2?cols=" + 6,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "PartNo", title: "Part No" },
                { field: "AvailQty", title: "Avail Qty.", format: "{0:#,###.00}" },
                { field: "RetailPrice", title: "Harga Jual", format: "{0:#,###.00}" },
                { field: "MovingCode", title: "Mv. CD" },
                { field: "ABCClass", title: "ABC Class" },
                { field: "PartName", title: "Part Name" },
                //{field: "SupplierCode", title: "Supplier Code"},
                //{field: "IsGenuinePart", title: "IsGenuine Part"},
                //{field: "ProductType", title: "Product Type"},
                //{field: "PartCategory", title: "Part Category"},
                //{field: "CategoryName", title: "Category Name"}
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
            query: new breeze.EntityQuery().from("WarehouseCode").withParameters({ id: "", param: "" }),//"WarehouseCode",
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
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e);
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
                        //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                        console.log(e);
                    });
            }
        });
    };

    me.initialize = function () {
        me.detail = {};
    };

    $("[name = 'PartNo']").on('blur', function () {
        if ($('#PartNo').val() || $('#PartNo').val() != '') {
            $http.post('gn.api/masteritem/CheckItem?PartNo=' + $('#PartNo').val()).
            success(function (v, status, headers, config) {
                if (v.masterinfo) {
                    me.data.PartName = v.masterinfo.PartName;
                } else {
                    $('#PartNo').val('');
                    $('#PartName').val('');
                    me.PartNo();
                }
            });
        } else {
            me.data.PartNo = '';
            me.data.PartName = '';
            me.PartNo();
        }
    });

    $("[name = 'WarehouseCode']").on('blur', function () {
        if ($('#WarehouseCode').val() || $('#WarehouseCode').val() != '') {
            $http.post('gn.api/masteritem/GetLookupValueName?VarGroup=WRCD&varCode=' + $('#WarehouseCode').val()).// 2parameter
            success(function (v, status, headers, config) {
                if (v != "") {
                    me.data.WarehouseName = v;
                } else {
                    $('#WarehouseCode').val('');
                    $('#WarehouseName').val('');
                    me.WarehouseCode();
                }
            });
        } else {
            me.data.WarehouseCode = '';
            me.data.WarehouseName = '';
            me.WarehouseCode();
        }
    });

    me.start();

    me.testX = function () {
        Wx.showForm({ url: 'sp/penerimaan/entryclaimsupplier' });
    }
}

$(document).ready(function () {

    var options = {
        title: "Item Location",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlInfoPart",
                title: "Information Part",
                items: [
                        {
                            text: "Part No",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "PartNo", cls: "span2", placeHolder: "Part No", type: "popup", btnName: "btnPartNo", click: "PartNo()", required: true, validasi: "required" },
                                { name: "PartName", cls: "span6", placeHolder: "Part Name", readonly: true, required: true }
                            ]
                        },
					    {
					        text: "Warehouse",
					        type: "controls",
					        required: true,
					        items: [
                                {
                                    name: "WarehouseCode", cls: "span2", placeHolder: "Warehouse Code", type: "popup", btnName: "btnWarehouseCode",
                                    click: "WarehouseCode()",
                                    validasi: "required,maxVal(99),regex(^[^X]+,Kode Gudang tidak boleh dimulai dengan huruf X)"
                                },
                                { name: "WarehouseName", cls: "span6", placeHolder: "Warehouse Name", readonly: true }
					        ]
					    }
                ]
            },
            {
                name: "pnlInfoPartLokasi",
                title: "Information Location",
                items: [
				   {
				       name: "LocationCode", text: "General Location", cls: "span4 full",
				       placeHolder: "General Location", required: true, validasi: "required"
				   },
                   { name: "LocationSub1", text: "Location Sub 1", cls: "span4" },
				   { name: "LocationSub4", text: "Location Sub 4", cls: "span4" },
                   { name: "LocationSub2", text: "Location Sub 2", cls: "span4" },
                   { name: "LocationSub5", text: "Location Sub 5", cls: "span4" },
				   { name: "LocationSub3", text: "Location Sub 3", cls: "span4" },
                   { name: "LocationSub6", text: "Location Sub 6", cls: "span4" },
                   { type: "div", name: "samplewx", cls: "span8" }

                ]
            },
        ],
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spItemLokasiController");
        Wx.submit();
    }

});