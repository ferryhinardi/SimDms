var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterModelAccountController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "ModelAccountBrowse",
            title: "Model Account Browse",
            manager: spSalesManager,
            query: "ModelAccountBrowse",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                me.Apply();

            }

        });
    }

    me.SalesModelCodeLookup = function () {
        me.data = {};
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Model Sales Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "SalesModelCode" },
                { field: "SalesModelDesc", title: "SalesModelDesc" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCode = data.SalesModelCode;
                me.data.SalesModelDesc = data.SalesModelDesc;
                me.Apply();
                $('#SalesModelCode').attr('disabled', 'disabled');
                $http.post('om.api/MstModelAccount/ModelAccount', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        if (data.record != "") {
                            me.data = data.record[0];
                        } else {
                            me.data.SalesModelCode;
                            me.data.SalesModelDesc;
                        }
                       $('#SalesModelCode').attr('disabled', 'disabled');
                   }
                }).
                error(function (data, status, headers, config) {
                   alert('error');
                });
            }
        });

    }

    me.SalesAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesAccNo = data.AccountNo;
                me.data.SalesAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.DiscountAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DiscountAccNo = data.AccountNo;
                me.data.DiscountAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.COGsAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.COGsAccNo = data.AccountNo;
                me.data.COGsAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.InventoryAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InventoryAccNo = data.AccountNo;
                me.data.InventoryAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.ReturnAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReturnAccNo = data.AccountNo;
                me.data.ReturnAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.HReturnAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.HReturnAccNo = data.AccountNo;
                me.data.HReturnAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.SalesAccNoAksLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesAccNoAks = data.AccountNo;
                me.data.SalesAccDescAks = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.ReturnAccNoAksLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReturnAccNoAks = data.AccountNo;
                me.data.ReturnAccDescAks = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.DiscountAccNoAksLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DiscountAccNoAks = data.AccountNo;
                me.data.DiscountAccDescAks = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.ShipAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ShipAccNo = data.AccountNo;
                me.data.ShipAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.DepositAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DepositAccNo = data.AccountNo;
                me.data.DepositAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.OthersAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.OthersAccNo = data.AccountNo;
                me.data.OthersAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.BBNAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BBNAccNo = data.AccountNo;
                me.data.BBNAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.KIRAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.KIRAccNo = data.AccountNo;
                me.data.KIRAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.PReturnAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PReturnAccNo = data.AccountNo;
                me.data.PReturnAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.InTransitTransferStockAccNoLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Description" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InTransitTransferStockAccNo = data.AccountNo;
                me.data.IntransitAccDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.initialize = function () {
        me.hasChanged = false;
        me.data.IsActive = true;
        $('#SalesModelCode').removeAttr('disabled');
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstModelAccount/Delete', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.init();
                        Wx.Success("Data deleted...");
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.saveData = function (e, param) {
        $http.post('om.api/MstModelAccount/Save', me.data).
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
    }

    $("[name='SalesModelCode']").on('blur', function () {
        if (me.data.SalesModelCode != null) {
            $http.post('om.api/MstModelAccount/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.SalesModelDesc = data.data.SalesModelDesc;
                       $('#SalesModelCode').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.SalesModelCode = "";
                       me.data.SalesModelDesc = "";
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
            $http.post('om.api/MstModelAccount/ModelAccount', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data = data.data;
                       $('#SalesModelCode').attr('disabled', 'disabled');
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });



    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Model Account",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "ModelAccount",
                title: "Model Account",
                items: [
                        {
                            text: "Sales Model Code",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SalesModelCode", cls: "span2", placeHolder: "SalesModelCode", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCodeLookup()", required: true, validasi: "required" },
                                { name: "SalesModelDesc ", cls: "span6", placeHolder: "SalesModelDesc", model: "data.SalesModelDesc", readonly: true },
                            ]
                        },
                ]
            },
            {
                name: "AccountUnit",
                title: "Account Unit",
                items: [
                        {
                            text: "No Acc. Penjualan/Sales",
                            type: "controls",
                            items: [
                                { name: "SalesAccNo", cls: "span4", placeHolder: "SalesAccNo", readonly: true, type: "popup", btnName: "btnSalesAccNo", click: "SalesAccNoLookup()" },
                                { name: "SalesAccDesc ", cls: "span4", placeHolder: "SalesAccDesc", model: "data.SalesAccDesc", readonly: true },
                            ]
                        },
                        {
                            text: "No.Acc.Potongan/Diskon",
                            type: "controls",
                            items: [
                                { name: "DiscountAccNo", cls: "span4", placeHolder: "DiscountAccNo", readonly: true, type: "popup", btnName: "btnDiscountAccNo", click: "DiscountAccNoLookup()" },
                                { name: "DiscountAccDesc ", cls: "span4", placeHolder: "DiscountAccDesc", model: "data.DiscountAccDesc", readonly: true },
                            ]
                        },                      
                        {
                            text: "No Acc. COGs",
                            type: "controls",
                            items: [
                                { name: "COGsAccNo", cls: "span4", placeHolder: "COGsAccNo", readonly: true, type: "popup", btnName: "btnCOGsAccNo", click: "COGsAccNoLookup()" },
                                { name: "COGsAccDesc ", cls: "span4", placeHolder: "COGsAccDesc", model: "data.COGsAccDesc", readonly: true },
                            ]
                        },
                        {
                            text: "No Acc. Inventori",
                            type: "controls",
                            items: [
                                { name: "InventoryAccNo", cls: "span4", placeHolder: "InventoryAccNo", readonly: true, type: "popup", btnName: "btnInventoryAccNo", click: "InventoryAccNoLookup()" },
                                { name: "InventoryAccDesc ", cls: "span4", placeHolder: "InventoryAccDesc", model: "data.InventoryAccDesc", readonly: true },
                            ]
                        },
                        {
                             text: "No Acc. Retur",
                             type: "controls",
                             items: [
                                 { name: "ReturnAccNo", cls: "span4", placeHolder: "ReturnAccNo", readonly: true, type: "popup", btnName: "btnReturnAccNo", click: "ReturnAccNoLookup()" },
                                 { name: "ReturnAccDesc ", cls: "span4", placeHolder: "ReturnAccDesc", model: "data.ReturnAccDesc", readonly: true },
                             ]
                        },
                        {
                            text: "No Acc. Hutang Retur",
                            type: "controls",
                            items: [
                                { name: "HReturnAccNo", cls: "span4", placeHolder: "HReturnAccNo", readonly: true, type: "popup", btnName: "btnHReturnAccNo", click: "HReturnAccNoLookup()" },
                                { name: "HReturnAccDesc ", cls: "span4", placeHolder: "HReturnAccDesc", model: "data.HReturnAccDesc", readonly: true },
                            ]
                        },
                ]
            },
            {
                name: "AccountAksesoris",
                title: "Account Aksesoris",
                items: [
                        {
                            text: "No Acc. Penjualan/Sales",
                            type: "controls",
                            items: [
                                { name: "SalesAccNoAks", cls: "span4", placeHolder: "SalesAccNoAks", readonly: true, type: "popup", btnName: "btnSalesAccNoAks", click: "SalesAccNoAksLookup()" },
                                { name: "SalesAccDescAks ", cls: "span4", placeHolder: "SalesAccDescAks", model: "data.SalesAccDescAks", readonly: true },
                            ]
                        },
                        {
                            text: "No Acc. Retur",
                            type: "controls",
                            items: [
                                { name: "ReturnAccNoAks", cls: "span4", placeHolder: "ReturnAccNoAks", readonly: true, type: "popup", btnName: "btnReturnAccNoAks", click: "ReturnAccNoAksLookup()" },
                                { name: "ReturnAccDescAks ", cls: "span4", placeHolder: "ReturnAccDescAks", model: "data.ReturnAccDescAks", readonly: true },
                            ]
                        },
                        {
                            text: "No Acc. Potongan/Diskon",
                            type: "controls",
                            items: [
                                { name: "DiscountAccNoAks", cls: "span4", placeHolder: "DiscountAccNoAks", readonly: true, type: "popup", btnName: "btnDiscountAccNoAks", click: "DiscountAccNoAksLookup()" },
                                { name: "DiscountAccDescAks ", cls: "span4", placeHolder: "DiscountAccDescAks", model: "data.DiscountAccDescAks", readonly: true },
                            ]
                        },
                ]
            },
             {
                 name: "AccountLainLain",
                 title: "Account Lain-Lain (NON TAX)",
                 items: [
                         {
                             text: "No Acc. Ongkos Kirim",
                             type: "controls",
                             items: [
                                 { name: "ShipAccNo", cls: "span4", placeHolder: "ShipAccNo", readonly: true, type: "popup", btnName: "btnShipAccNo", click: "ShipAccNoLookup()" },
                                 { name: "ShipAccDesc ", cls: "span4", placeHolder: "ShipAccDesc", model: "data.ShipAccDesc", readonly: true },
                             ]
                         },
                         {
                             text: "No Acc. Deposit",
                             type: "controls",
                             items: [
                                 { name: "DepositAccNo", cls: "span4", placeHolder: "DepositAccNo", readonly: true, type: "popup", btnName: "btnDepositAccNo", click: "DepositAccNoLookup()" },
                                 { name: "DepositAccDesc ", cls: "span4", placeHolder: "DepositAccDesc", model: "data.DepositAccDesc", readonly: true },
                             ]
                         },
                         {
                             text: "No Acc. Lain - Lain",
                             type: "controls",
                             items: [
                                 { name: "OthersAccNo", cls: "span4", placeHolder: "OthersAccNo", readonly: true, type: "popup", btnName: "btnOthersAccNo", click: "OthersAccNoLookup()" },
                                 { name: "OthersAccDesc ", cls: "span4", placeHolder: "OthersAccDesc", model: "data.OthersAccDesc", readonly: true },
                             ]
                         },
                         {
                             text: "No Acc. B B N",
                             type: "controls",
                             items: [
                                 { name: "BBNAccNo", cls: "span4", placeHolder: "BBNAccNo", readonly: true, type: "popup", btnName: "btnBBNAccNo", click: "BBNAccNoLookup()" },
                                 { name: "BBNAccDesc ", cls: "span4", placeHolder: "BBNAccDesc", model: "data.BBNAccDesc", readonly: true },
                             ]
                         },
                         {
                             text: "No Acc. K I R",
                             type: "controls",
                             items: [
                                 { name: "KIRAccNo", cls: "span4", placeHolder: "KIRAccNo", readonly: true, type: "popup", btnName: "btnKIRAccNo", click: "KIRAccNoLookup()" },
                                 { name: "KIRAccDesc ", cls: "span4", placeHolder: "KIRAccDesc", model: "data.KIRAccDesc", readonly: true },
                             ]
                         },
                 ]
             },
             {
                 name: "AccountPembelian",
                 title: "Account Pembelian",
                 items: [
                         {
                             text: "Acc. Persediaan Sementara",
                             type: "controls",
                             items: [
                                 { name: "PReturnAccNo", cls: "span4", placeHolder: "PReturnAccNo", readonly: true, type: "popup", btnName: "btnPReturnAccNo", click: "PReturnAccNoLookup()" },
                                 { name: "PReturnAccDesc ", cls: "span4", placeHolder: "PReturnAccDesc", model: "data.PReturnAccDesc", readonly: true },
                             ]
                         },
                 ]
             },
             {
                 name: "AccountTransferStock",
                 title: "ACCOUNT TRANSFER STOCK ANTAR CABANG",
                 items: [
                         {
                             text: "Acc. Intransit Transfer Stock",
                             type: "controls",
                             items: [
                                 { name: "InTransitTransferStockAccNo", cls: "span4", placeHolder: "InTransitTransferStockAccNo", readonly: true, type: "popup", btnName: "btnInTransitTransferStockAccNo", click: "InTransitTransferStockAccNoLookup()" },
                                 { name: "IntransitAccDesc ", cls: "span4", placeHolder: "IntransitAccDesc", model: "data.IntransitAccDesc", readonly: true },
                             ]
                         },
                 ]
             },
             {
                 name: "Keterangan",
                 title: "KETERANGAN",
                 items: [
                         { name: "Remark", text: "Keterangan", cls: "span8", maxlength: 100 },
                         { name: "IsActive", text: "Status", type: "x-switch", cls: "span2" }
                 ]
             },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterModelAccountController");
    }

});