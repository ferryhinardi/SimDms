var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


function SpMstItemsController($scope, $http, $injector, $compile) {

    // Untuk mempermudah penulisan kode program, define variable me sebagai $scope (Alias)
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    /* Preparing data source for combobox 1..3*/
    $http.post('sp.api/Combo/LoadComboData?CodeId=PRCT').
    success(function (data, status, headers, config) {
        me.comboPRCT = data;
    });

    $http.post('sp.api/Combo/LoadComboData?CodeId=UOMC').
    success(function (data, status, headers, config) {
        me.comboUOMC = data;
    });

    $http.post('sp.api/Combo/LoadComboData?CodeId=STPR').
    success(function (data, status, headers, config) {
        me.comboSTPR = data;
    });


    //me.browse = function(s)
    //{       
    //    if (s === undefined)
    //    {
    //        s = "Browse Master Item";
    //    } else {
    //        s = "Sparepart Lookup";
    //    }

    //    var lookup = Wx.blookup({
    //        name: "SparepartLookup",
    //        title: "Item Lookup",
    //        manager: spManager,
    //        query: "SparePartLookupNew",
    //        defaultSort: "PartNo asc",
    //        columns: [
    //            {field: "PartNo", title: "No Part", width: 160},
    //            {field: "PartName", title: "Nama Part", width: 300 },
    //            { field: "SupplierCode", title: "Kode Suplier", width: 120 },
    //            { field: "IsGenuinePart", title: "Prd Suzuki", width: 110 },
    //            { field: "ProductType", title: "Tipe Produk", width: 110 },
    //            { field: "PartCategory", title: "Kategori", width: 110 },
    //            { field: "CategoryName", title: "Nama Kategori", width: 130 },
    //            { field: "TypeOfGoods", title: "Tipe Part", width: 150 }
    //        ],
    //    });

    //    // fungsi untuk menghandle double click event pada k-grid / select button
    //    lookup.dblClick(function (data) {
    //        if (data != null) {
    //            me.isInProcess = true;
    //            me.lookupAfterSelect(data);
    //            me.GetPartInfo(data.PartNo);                 
    //        }
    //    });

    //    lookup.onCancel(function (s) {
    //        console.log(s)
    //    })
    //}

    me.browse = function (s) {
        if (s === undefined) {
            s = "Browse Master Item";
        } else {
            s = "Sparepart Lookup";
        }

        var lookup = Wx.klookup({
            name: "SparepartLookup",
            title: "Item Lookup",
            url: "sp.api/Grid/SparePartLookupNewV2?cols=" + 8,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "PartNo", title: "No Part", width: 160 },
                { field: "PartName", title: "Nama Part", width: 300 },
                { field: "SupplierCode", title: "Kode Suplier", width: 120 },
                { field: "IsGenuinePart", title: "Prd Suzuki", width: 110 },
                { field: "ProductType", title: "Tipe Produk", width: 110 },
                { field: "PartCategory", title: "Kategori", width: 110 },
                { field: "CategoryName", title: "Nama Kategori", width: 130 },
                { field: "TypeOfGoods", title: "Tipe Part", width: 150 }
            ],
        });
        // fungsi untuk menghandle double click event pada k-grid / select button
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isInProcess = true;
                me.lookupAfterSelect(data);
                me.GetPartInfo(data.PartNo);
            }
        });
    }

    // save data, posting data from user cache to the server
    me.saveData = function (e, param) {
        var purcDiscPct = parseFloat(me.data.PurcDiscPct);
        if (purcDiscPct == 0) {
            MsgConfirm("Yakin Discount Supplier (Purc.): <b style=\"font-size:16px;color:blue;\">0%</b> ?", function (result) {
                if (result) {
                    me.saveDataItem();
                }
                else {
                    setTimeout(function () {
                        $('#PurcDiscPct').focus();
                    }, 200);
                }
            });
        }
        else {
            me.saveDataItem();
        }
    }

    me.saveDataItem = function () {
        // Using $http provider from angular to posting data to the server  
        $http.post('sp.api/masteritem/save', me.data).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                me.startEditing();
            } else {
                // show an error message
                MsgBox(data.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            // MsgBox(e, MSG_ERROR);
            console.log(e);
        });

    }

    me.delete = function () {
        // show confirmation dialog to the user
        // check respond, if true, notify to the server to remove current record
        MsgConfirm("Are you sure to delete a current record?", function (result) {
            if (result) {
                // call web api by $http provider (async mode)
                $http.post('sp.api/MasterItem/delete', me.data)
                    .success(function (v, status, headers, config) {
                        if (v.success) {
                            Wx.Info("Record has been deleted...");
                            me.init();
                        } else {
                            // show an error message
                            MsgBox(v.message, MSG_ERROR);
                        }
                    }).error(function (e, status, headers, config) {
                        // MsgBox(e, MSG_ERROR);
                        console.log(e);
                    });
            }
        })
    }

    me.initialize = function () {
        me.data = {};
        $http.post('sp.api/MasterItem/default')
        .success(function (v, status, headers, config) {
            me.data.TypeOfGoods = v.PartType;
            me.data.ProductType = v.ProductType;
            me.data.ProductTypeDesc = v.ProductTypeDesc;
            me.UserInfo = v.UserInfo;
        });
        me.record = {};
        me.recInfo = {};
        me.recModel = {};
        //me.UserInfo = {};
        me.data.IsGenuinePart = true;
        me.data.BornDate = me.now();
        me.data.LastPurchaseDate = me.now();
        me.data.LastDemandDate = me.now();
        me.data.LastSalesDate = me.now();
        me.data.OrderUnit = 1;
        me.data.SalesUnit = 1;
        me.data.MovingCode = "0";
        me.data.PurcDiscPct = 0;
        me.data.DiscPct = 0;
        //me.data.TypeOfGoods = me.UserInfo.TypeOfGoodsName;
        //me.data.ProductType = me.UserInfo.ProductType;
        //me.data.ProductTypeDesc = me.UserInfo.ProductTypeName;

        me.grid.alokasi = {};
        me.clearTable(me.grid1);

        me.grid.model = {};
        me.clearTable(me.grid2);

    }

    $("[name = 'PartNo']").on('blur', function () {
        if ($('#PartNo').val() || $('#PartNo').val() != '') {
            me.GetPartInfo($('#PartNo').val());
        } else {
            me.initialize();
        }
    });

    $("[name = 'SupplierCode']").on('blur', function () {
        if ($('#SupplierCode').val() || $('#SupplierCode').val() != '') {
            $http.post('gn.api/Lookup/SupplierName?SupplierClass=' + $('#SupplierCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.SupplierName = v.TitleName;
                } else {
                    $('#SupplierCode').val('');
                    $('#SupplierName').val('');
                    me.browseSupplier();
                }
            });
        } else {
            me.data.SupplierCode = '';
            me.data.SupplierName = '';
            me.browseSupplier();
        }
    });

    me.GetPartInfo = function (partNo) {
        me.initialize();
        var src = "sp.api/masteritem/CheckItem?PartNo=" + partNo;

        $http.post(src)
            .success(function (v, status, headers, config) {
                console.log(v);
                if (v.success) {
                    me.currentrecord = v;
                    me.backup = me.data;
                    switch (v.mode) {
                        case 0:
                            MsgBox(v.message, MSG_WARNING);
                            me.init();
                            break;
                        case 2:
                            var info = v.masterinfo;
                            if (info.ProductType == me.UserInfo.CoProfile.ProductType) {
                                me.data.PartNo = info.PartNo;
                                me.data.PartName = info.PartName;
                                me.data.SupplierCode = info.SupplierCode;

                                me.data.OrderUnit = info.OrderUnit;
                                me.data.SalesUnit = (info.SalesUnit);
                                me.data.DiscPct = (info.DiscPct);
                                me.data.Utility1 = ("");
                                me.data.Utility2 = ("");
                                me.data.Utility3 = ("");
                                me.data.Utility4 = ("");
                                me.data.PartCategory = (info.PartCategory);
                                me.data.UOMCode = (info.UOMCode);
                                me.data.IsGenuinePart = info.IsGenuinePart;
                                me.recordParam = v.orderparam;
                            } else {
                                me.init();
                            }
                            me.data.DemandAverage = 0;
                            me.readyToSave();
                            me.allowEdit();
                            //me.isLoading = true;
                            //me.isSave = me.isEdit = true;
                            //me.startEditing();
                            break;
                        case 3:
                            me.data = v.data;
                            me.data.PartName = me.backup.PartName;
                            me.data.SupplierCode = me.backup.SupplierCode;
                            me.data.SupplierName = me.backup.SupplierName;

                            me.GetLookupValue("TPGO", me.data.TypeOfGoods, "TypeOfGoods");
                            me.GetLookupValue("PRDT", me.data.ProductType, "ProductTypeDesc");

                            var info = v.masterinfo;
                            if (info != null) {
                                me.data.IsGenuinePart = (info.IsGenuinePart);
                                me.data.DiscPct = (info.DiscPct);
                                me.data.PartCategory = (info.PartCategory);
                                me.data.UOMCode = (info.UOMCode);
                                me.data.PartName = info.PartName;
                                me.data.SupplierCode = info.SupplierCode;
                                me.data.SupplierCode = info.SupplierCode;
                                me.data.SupplierName = v.SupplierName;
                            }
                            me.recordParam = {};
                            me.recordParam.OrderPointQty = (me.data.OrderPointQty);
                            me.recordParam.SafetyStock = (me.data.SafetyStock);
                            me.recordParam.OrderCycle = (me.data.OrderCycle);
                            me.recordParam.LeadTime = (me.data.LeadTime);
                            var discount = 0;

                            if (info != null) {
                                var profit = v.profit;
                                if (profit != null) {
                                    if (me.data.PurcDiscPct > 0)
                                        discount = me.data.PurcDiscPct;
                                    else {
                                        discount = profit.DiscPct;

                                        var d2 = parseFloat(v.discount2);
                                        discount += d2;

                                        if (v.campaign != null) {
                                            discount += v.campaign.DiscPct;
                                        }
                                    }
                                }
                            }

                            me.recordPrice = v.price;

                            if (me.recordPrice != null) {
                                var decPotongan = me.recordPrice.PurchasePrice * (discount / 100);
                                me.recordPrice.Discount = (decPotongan);
                                me.recordPrice.DiscountPercentage = discount;

                                var decHargaBeli = me.recordPrice.PurchasePrice - decPotongan;
                                me.recordPrice.HargaBeli = (decHargaBeli);
                            }

                            me.grid.alokasi = v.alokasi;
                            me.loadTableData(me.grid1, me.grid.alokasi);

                            me.grid.model = v.model;
                            me.loadTableData(me.grid2, me.grid.model);
                            me.startEditing();

                            break;
                        default:
                    }
                } else {
                    // show an error message
                    //MsgBox(v.message, MSG_ERROR);
                    me.data.PartNo = partNo;
                }

                me.delayEditing();
            }).error(function (e, status, headers, config) {
                //MsgBox(e, MSG_ERROR);
                console.log(e);
                me.delayEditing();
            });
    }

    me.browseSupplier = function () {
        var lookup = Wx.blookup({
            name: "suppliercodeBrowse",
            title: "Supplier Code Browse",
            manager: spManager,
            query: "Suppliers",
            defaultSort: "SupplierCode asc",
            columns: [
             { field: 'SupplierCode', title: 'Supplier Code' },
             { field: 'SupplierName', title: 'Supplier Name' }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.browseModel = function () {
        var lookup = Wx.blookup({
            name: "ModelBrowse",
            title: "Model Browse",
            manager: spManager,
            query: "ModelLookup",
            defaultSort: "ModelCode asc",
            columns: [
             { field: 'ModelCode', title: 'Model Code' },
             { field: 'ModelName', title: 'Model Name' }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                me.recModel.ModelCode = data.ModelCode;
                me.recModel.ModelName = data.ModelName;
                me.Apply();
            }
        });
    }

    me.AddModel = function () {
        var src = "sp.api/masteritem/savemodel";
        $.ajax({
            async: false,
            type: "POST",
            data: {
                PartNo: me.data.PartNo,
                ModelCode: me.recModel.ModelCode,
                PartCtg: me.data.PartCategory
            },
            url: src
        }).done(function (data) {
            if (data.success) {
                Wx.Success("Model " + me.recModel.ModelCode + " has been added into current part");
                me.grid.model = data.result;
                me.loadTableData(me.grid2, me.grid.model);
                me.recModel = {};
            }
        });
    }

    me.UpdateModel = function () {
        var src = "sp.api/masteritem/updatemodel";
        $.ajax({
            async: false,
            type: "POST",
            data: {
                PartNo: me.data.PartNo,
                ModelCode: me.recModel.ModelCode,
                OldModel: me.recModel.old,
                PartCtg: me.data.PartCategory
            },
            url: src
        }).done(function (data) {
            if (data.success) {
                Wx.Success("Model " + me.recModel.old + " has been changed with " + me.recModel.ModelCode);
                me.grid.model = data.result;
                me.loadTableData(me.grid2, me.grid.model);
                me.recModel = {};
            }
        });
    }
    me.DeleteModel = function () {
        var src = "sp.api/masteritem/DeleteModel";
        $.ajax({
            async: false,
            type: "POST",
            data: {
                PartNo: me.data.PartNo,
                ModelCode: me.recModel.old,
                PartCtg: me.data.PartCategory
            },
            url: src
        }).done(function (data) {
            if (data.success) {
                Wx.Success("Model " + me.recModel.old + " has been remove from current part");
                me.clearTable(me.grid2);
                me.grid.model = data.result;
                me.loadTableData(me.grid2, me.grid.model);
                me.recModel = {};
            }
        });
    }

    me.CloseModel = function () {
        me.recModel = {};
        me.grid2.clearSelection();
    }

    me.grid1 = new webix.ui({
        container: "wxlocation",
        view: "wxtable",
        columns: [
            { id: "WarehouseCode", header: "Warehouse Code", fillspace: true },
            { id: "WarehouseName", header: "Warehouse Name", fillspace: true },
            { id: "LocationCode", header: "Location Code", fillspace: true },
            { id: "OnHand", header: "On Hand", fillspace: true },
            { id: "AllocationSP", header: "Sparepart Allocation", fillspace: true },
            { id: "AllocationSR", header: "Bengkel Allocation", fillspace: true },
            { id: "AllocationSL", header: "Unit Allocation", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    me.detail.oid = me.grid1.getSelectedId();
                    me.Apply();
                }
            }
        }
    });


    me.grid2 = new webix.ui({
        container: "tblModel",
        view: "wxtable",
        columns: [
            { id: "ModelCode", header: "Model Code", fillspace: true },
            { id: "ModelName", header: "Model Name", fillspace: true, format: me.replaceNull }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid2.getSelectedId() !== undefined) {
                    me.recModel = this.getItem(me.grid2.getSelectedId().id);
                    me.recModel.oid = me.grid2.getSelectedId();
                    me.recModel.old = me.recModel.ModelCode;
                    me.Apply();
                }
            }
        }
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.OnTabChange = function (e, id) {
        if (id === "StockAlokasi") {
            me.grid1.adjust();
        }

        if (id === "ModelCode") {
            me.grid2.adjust();
        }
    }

    // start up point */
    me.start();

    me.startMonitoring('recModel');

}

$(document).ready(function () {
    var options = {
        title: "Master Item",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlMasterItem",
                title: "Master Item",
                items: [
                    {
                        name: "PartNo", text: "Part No.", cls: "span4", type: "popup",
                        btnName: "btnPartNo", readonly: false, required: true, validasi: "required", click: "browse(1)"
                    },
                    { name: "TypeOfGoods", text: "Part Type", cls: "span4", readonly: true },
                    { name: "PartName", text: "Part Name", cls: "span9", readonly: false, validasi: "required" },
                    {
                        text: "Supplier",
                        type: "controls",
                        cls: "span8",
                        items: [
                                { name: "SupplierCode", click: "browseSupplier()", cls: "span2", placeHolder: "Kode Pemasok", type: "popup", btnName: "btnSupplierCode", validasi: "required" },
                                { name: "SupplierName", cls: "span6", placeHolder: "Nama Pemasok", readonly: true, validasi: "required" }
                        ]
                    },
                    { name: "BornDate", text: "Inventory Date", type: "ng-datepicker", cls: "span4" },
                    { name: "IsGenuinePart", text: "Is SUZUKI Product ?", type: "x-switch", cls: "span4" },
                    { name: "PartCount", type: "hidden" }
                ]
            },
            {
                name: "pnlMasterItemDetails",
                title: "Item Details",
                items: [

                    { name: "OrderUnit", text: "Order Unit", cls: "span4 number-int", readonly: false },
                    { name: "SalesUnit", text: "Sales Unit", cls: "span4 number-int", readonly: false },
                    { name: "MovingCode", text: "Moving Code", cls: "span4", readonly: true },
                    { name: "PurcDiscPct", text: "Disc. Supl (Purc.) %", cls: "span4 number", readonly: false },
                    { name: "LastPurchaseDate", text: "Last Purchase Date", type: "ng-datepicker", cls: "span4", disable: true },
                    { name: "DiscPct", text: "Disc. Part (Part.) %", cls: "span4 number", readonly: false },
                    { name: "LastDemandDate", text: "Last Demand Date", type: "ng-datepicker", cls: "span4", disable: true },
                    {
                        text: "Product Type",
                        type: "controls",
                        cls: "span4",
                        items: [
                                    { name: "ProductType", cls: "col-x-4", placeHolder: "4W", readonly: true },
                                    { name: "ProductTypeDesc", cls: "col-x-16", placeHolder: "4WHEEL", readonly: true }
                        ]
                    },

                    { name: "LastSalesDate", text: "Last Sales Date", type: "ng-datepicker", cls: "span4", disable: true },
                    { name: "PartCategory", text: "Part Category", cls: "span4", validasi: "required", type: "select2", datasource: "comboPRCT" },

                    { name: "ABCClass", text: "ABC Class", cls: "span4", readonly: true },
                    { name: "UOMCode", text: "UOM", cls: "span4", type: "select2", datasource: "comboUOMC" },

                    { name: "DemandAverage", text: "Demand Average", cls: "span4 number", readonly: true },
                    { name: "Status", text: "Status", cls: "span4", validasi: "required", type: "select2", datasource: "comboSTPR" },

                    { name: "Utility1", text: "Utility 1", cls: "span4", readonly: false },
                    { name: "Utility2", text: "Utility 2", cls: "span4", readonly: false },
                    { name: "Utility3", text: "Utility 3", cls: "span4", readonly: false },
                    { name: "Utility4", text: "Utility 4", cls: "span4", readonly: false },
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                model: "testTabModel",
                items: [
                    { name: "ItemInventory", text: "Inventory" },
                    { name: "StockAlokasi", text: "Stock & Alokasi" },
                    { name: "HPemPenj", text: "Harga Pembelian & Penjualan" },
                    { name: "OrderParam", text: "Order Param" },
                    { name: "ModelCode", text: "Model Code" },
                ]
            },
            {
                name: "ItemInventory",
                title: "Inventory",
                cls: "tabpage1 ItemInventory",
                items: [
                    { name: "OnOrder", text: "On Order", cls: "span4 number-int", readonly: true },
                    { name: "InTransit", text: "In Transit", cls: "span4 number-int", readonly: true },
                    { name: "BorrowQty", text: "Borrow Qty", cls: "span4 number-int", readonly: true },
                    { name: "BorrowedQty", text: "Borrowed Qty", cls: "span4 number-int", readonly: true },
                    { name: "BackOrderSR", text: "BO Service", cls: "span4 number-int", readonly: true },
                    { name: "ReservedSR", text: "Reserved Service", cls: "span4 number-int", readonly: true },
                    { name: "BackOrderSP", text: "BO Sparepart", cls: "span4 number-int", readonly: true },
                    { name: "ReservedSP", text: "Reserved Sparepart", cls: "span4 number-int", readonly: true },
                    { name: "BackOrderSL", text: "BO Unit", cls: "span4 number-int", readonly: true },
                    { name: "ReservedSL", text: "Reserved Unit", cls: "span4 number-int", readonly: true },
                ]
            },
            {
                name: "wxlocation",
                title: "Stock & Alokasi",
                cls: "tabpage1 StockAlokasi",
                xtype: "wxtable"
            },
            {
                name: "HPemPenj",
                title: "Purchase & Sales Price",
                cls: "tabpage1 HPemPenj",
                items: [
                    { name: "HargaSupplier", model: "recordPrice.PurchasePrice", text: "Supplier Price", cls: "span4  number-int ", readonly: true },
                    {
                        text: "Purchase Discount",
                        type: "controls",
                        items: [
                            { name: "Potongan", model: "recordPrice.Discount", cls: "span1  number-int ", placeHolder: "0", readonly: true },
                            { name: "Percentage", model: "recordPrice.DiscountPercentage", cls: "span1 number ", placeHolder: "0", readonly: true },
                            { cls: "span1 label-valign", text: "%", type: "label" }
                        ]
                    },
                    { name: "HargaBeli", model: "recordPrice.HargaBeli", text: "Order Price", cls: "span4  number-int ", readonly: true },
                    { name: "HargaJual", model: "recordPrice.RetailPrice", text: "Sales Price", cls: "span4  number-int ", readonly: true },
                    { name: "HargaJualPlusPajak", model: "recordPrice.RetailPriceInclTax", text: "Sales Price + Tax", cls: "span4  number-int ", readonly: true },
                    { name: "AverageCost", model: "recordPrice.CostPrice", text: "Average Cost", cls: "span4 number ", readonly: true }
                ]
            },
            {
                name: "OrderParam",
                title: "Order Parameters",
                cls: "tabpage1 OrderParam",
                items: [
                    { name: "OrderPointQty", model: "recordParam.OrderPointQty", text: "Order Point (Qty)", cls: "span4  number-int", readonly: true },
                    { name: "OrderCycle", model: "recordParam.OrderCycle", text: "Order Cycle (Day)", cls: "span4 number-int", readonly: true },
                    { name: "SafetyStock", model: "recordParam.SafetyStock", text: "Safety Stock (Day)", cls: "span4 number-int", readonly: true },
                    { name: "LeadTime", model: "recordParam.LeadTime", text: "Lead Time (Day)", cls: "span4 number-int", readonly: true }
                ]
            },
            {
                name: "ModelCode",
                title: "Model Code",
                cls: "tabpage1 ModelCode",
                items: [
                            {
                                text: "Model",
                                type: "controls",
                                cls: "span8",
                                items: [
                                        {
                                            name: "KodeModel", model: "recModel.ModelCode", cls: "span3",
                                            placeHolder: "Kode Model", disable: "data.PartNo === undefined",
                                            type: "popup", btnName: "btnKodeModel", readonly: false, click: "browseModel()", readonly: "readonly"
                                        },
                                        { name: "NamaModel", model: "recModel.ModelName", cls: "span5", placeHolder: "Nama Model", readonly: false, readonly: "readonly" }
                                ]
                            },
                            {
                                type: "buttons",
                                items: [
                                        { name: "btnAddModel", text: "Add Model", icon: "icon-plus", cls: "btn btn-info", click: "AddModel()", show: "recModel.old === undefined", disable: "recModel.ModelCode === undefined" },
                                        { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "UpdateModel()", show: "recModel.old !== undefined" },
                                        { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteModel()", show: "recModel.old !== undefined" },
                                        { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "recModel.old !== undefined" }
                                ]
                            }
                ]
            },
            {
                title: "Daftar Model",
                cls: "tabpage1 ModelCode",
                xtype: "wxtable",
                name: "tblModel"
            },

        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("SpMstItemsController");
    }

});

