var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spMaintainHargaPokokController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.Month = data;
    });

    $http.post('sp.api/Combo/YearsOld').
    success(function (data, status, headers, config) {
        me.Years = data;
    });

    $http.post('sp.api/Combo/TypePart').
    success(function (data, status, headers, config) {
        me.TypePart = data;
    });

    me.PartNo = function () {

        var lookup = Wx.blookup({
            name: "btnPartNo",
            title: "Part No Lookup",
            manager: SpUtilityManager,
            query: "PartNoMntHargaPokok",
            defaultSort: "PartNo ASC",
            columns: [
            { field: "PartNo", title: "No. Part" },
            { field: "PartName", title: "Nama Part" },
            { field: "WarehouseCode", title: "Gudang" },
            { field: "LocationCode", title: "Lokasi" },
            { field: "QtyAvail", title: "Qty Avail" },
            { field: "RetailPriceInclTax", title: "Harga Jual + Pajak" },
            { field: "RetailPrice", title: "Harga Jual" },
            { field: "SupplierCode", title: "Kode Supl." },
            { field: "IsGenuinePart", title: "Prd Suzuki" },
            { field: "ProductType", title: "Tipe Produk" },
            { field: "PartCategory", title: "Kategori" },
            { field: "CategoryName", title: "Nama Kategori" },
            
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {

                //me.lookupAfterSelect(data);
                me.data.PartNo = data.PartNo;
                me.PopulateData();
                me.Apply();

            }
        });
    }

    me.PopulateData = function () {
        $http.post('sp.api/MaintainHargaPokok/PopulateData?PartNo='+me.data.PartNo, me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    //console.log(data.dt);
                    me.data = data.dt[0];
                    me.data.NewCostPrice = "0.00"; 
                    me.data.CostPrices = (data.dt[0].CostPrice == null || data.dt[0].CostPrice == "") ? "0.00" : data.dt[0].CostPrice;
                    me.data.Onhand = (data.dt[0].Onhand == null || data.dt[0].Onhand == "") ? "0.00" : data.dt[0].Onhand;
                    var bothFlag1 = (data.dt[0].WarehouseCode != null || data.dt[0].WarehouseCode != "") ? true : false;
                    var bothFlag2 = (data.dt[0].RetailPrice != null || data.dt[0].RetailPrice != "") ? true : false;
                    me.data.MaterItemLoc = bothFlag1;
                    me.data.MasterItemPrice = bothFlag2;

                    if (bothFlag1 == false || bothFlag2 == false) {
                        MsgBox("Periksa kembali setting master untuk part : " + data.dt[0].PartNo + " !", MSG_ERROR);
                    }


                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

 
    me.saveData = function (e, param) {
        MsgConfirm("Apakah anda yakin, ingin me-maintain harga pokok untuk part : " + me.data.PartNo + " ?", function (result) {
            if (result) {
                $http.post('sp.api/MaintainHargaPokok/save', me.data).
                success(function (data, status, headers, config) {
                if (data.success) {
                    MsgBox("Berhasil maintain average cost untuk part : " + me.data.PartNo);
                    me.init();
                } else {
                    MsgBox("Gagal Maintain", MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
            }
        });
        
    }


    me.printPreview = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        $http.post('sp.api/MaintainHargaPokok/Print', me.savemodel).
            success(function (data, status, headers, config) {
                if (data.success) {
                    
                    var data = data.month + "," + data.year + "," + data.periode + "," + data.typeOfGoods;
                    var rparam = "admin";
					Wx.showPdfReport({
						id: "SpRpRgs029",
						pparam: data,
						rparam: rparam,
						type: "devex"
					});
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.pushprint = function () {
        me.detail.pushprint = "1";
        var date = new Date;
        me.detail.MonthPeriod = date.getMonth() + 1;
        me.detail.YearPeriod = date.getFullYear();
    }

    me.$watch("detail.IsPeriod", function (a, b) {
        if (a != b) {
            if (a == true) {
                me.detail.disHide = "1";
            } else {
                me.detail.disHide = "0";
            }
        } 
    })
    me.$watch("detail.IsType", function (a, b) {
        if (a != b) {
            if (a == true) {
                me.detail.pushHide = "1";
            } else {
                me.detail.pushHide = "0";
            }
        }
    })




    me.initialize = function()
    {
        
       
        me.data = {};
        me.detail = {};
        me.detail.Periode = me.now();
        me.data.NewCostPrice = "0.00";
        me.detail.disHide = "0";
        me.detail.pushHide = "0";
        me.detail.pushprint = "0";
    }




    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Maintenance HPP",
        xtype: "panels",
        toolbars: [
            { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "pushprint()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlHide",
                title: "",
                show: "detail.pushprint == 1",
                items: [
                    { name: "disHide", model: "detail.disHide", show: "data.feryyp != undefined" },
                    { name: "pushHide", model: "detail.pushHide", show: "data.feryyp != undefined" },
                    {
                        name: "IsPeriod",
                        text: "Period",
                        model: "detail.IsPeriod",
                        type: "x-switch",
                        cls: "span2",
                    },
                    {
                        name: "MonthPeriod",
                        text: "Month",
                        opt_text: "MONTH?",
                        cls: "span3",
                        type: "select2",
                        model: "detail.MonthPeriod",
                        datasource: "Month",
                        disable: "detail.disHide == 0"
                    },
                    {
                        name: "YearPeriod",
                        text: "Year",
                        opt_text: "YEAR?",
                        cls: "span3",
                        type: "select2",
                        model: "detail.YearPeriod",
                        datasource: "Years",
                        disable: "detail.disHide == 0"
                    },
                    {
                        name: "IsType",
                        text: "Tipe Part",
                        model: "detail.IsType",
                        type: "x-switch",
                        cls: "span4",
                    },
                    {
                        name: "PartType",
                        text: "Part Type",
                        cls: "span4",
                        type: "select2",
                        model: "detail.PartType",
                        opt_text:"SELECT ALL",
                        datasource: "TypePart",
                        disable: "detail.pushHide == 0"
                    },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnCloseHpp1", text: "PRINT", icon: "icon icon-print", cls: "btn btn-info", click: "printPreview()" },
                                { name: "btnCloseHpp2", text: "CANCEL", cls: "btn btn-warning", click: "initialize()" },
                        ]
                    }
                ]
            },
            {
                name: "pnlA",
                title: "",
                show: "detail.pushprint == 0",
                items: [
                        { name: "pushprint", model: "detail.pushprint", show:"data.feryyp != undefined" },
                        {
                            name: "Periode",
                            text: "Periode",
                            type: "ng-datepicker",
                            model: "detail.Periode",
                            cls: "span4",
                            readonly:true
                        },
                        {
                            text: "Part No",
                            type: "controls",
                            items: [
                                {
                                    name: "PartNo",
                                    cls: "span3",
                                    placeHolder: "Part Code",
                                    type: "popup",
                                    btnName: "btnPartNo",
                                    click: "PartNo()",
                                    validasi: "required",
                                    readonly :true
                                },
                                {
                                    name: "PartName",
                                    cls: "span5",
                                    placeHolder: "Part Name",
                                    readonly: true
                                }
                            ]
                        },
                        {
                            name: "NewCostPrice",
                            text: "Harga Pokok Baru",
                            cls: "span4 number",
                            validasi: "required",
                        },
  
                    ]   
            },
            {
                name: "pnlB",              
                title: "Informasi Part",
                show: "detail.pushprint == 0",
                 items: [
                        {
                            text: "Supplier Code",
                            type: "controls",
                            items: [
                                {
                                    name: "SupplierCode",
                                    cls: "span3",
                                    placeHolder: "Supplier Code",
                                    type: "text",
                                    readonly: true,
                                },
                                {
                                    name: "SupplierName",
                                    cls: "span5",
                                    placeHolder: "Supplier Name",
                                    readonly: true
                                }
                            ]
                        },
                        {
                            name: "TypeOfGoods",
                            cls: "span4",
                            text: "TypePart",
                            readonly: true
                        },
                        {
                            name: "CostPrices",
                            cls: "span4 number",
                            text: "Harga Pokok Current",
                            readonly: true
                        },
                        {
                            name: "Onhand",
                            cls: "span4 number",
                            text: "Qty On Hand",
                            readonly: true
                        },
                        {
                            name: "MaterItemLoc",
                            text: "Setting Item Location",
                            type: "x-switch",
                            cls: "span2",
                            disable: "detail.Periode != undefined"
                        },
                        {
                            name: "MasterItemPrice",
                            text: "Setting Item Harga",
                            type: "x-switch",
                            cls: "span2",
                            disable: "detail.Periode != undefined"
                        },

 
                ]
            },    

        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spMaintainHargaPokokController");
    }

});