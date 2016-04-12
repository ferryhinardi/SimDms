var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spglAccMappingController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.combo = {};

    me.initialize = function ()
    {
       $http.get('breeze/sparepart/CurrentUserInfo').
       success(function (dl, status, headers, config) {
           me.data.CompanyCode = dl.CompanyCode;
           me.data.BranchCode = dl.BranchCode;
       });

        $("[name = 'TypeOfGoods']").prop('disabled', false);
        me.combo = {
            SalesAccNoName: "", COGSAccNoName: "", InventoryAccNoName: "",
            DiscAccNoName: "", ReturnAccNoName: "", ReturnPybAccNoName: "",
            OtherIncomeAccNoName: "", OtherReceivableAccNoName: "", InTransitAccNoName: ""
        };
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "glACCMappingBrowse",
            title: "GL ACC Mapping Browse",
            manager: spManager,
            query: "GLAccMappingBrowse",
            defaultSort: "TypeOfGoods asc",
            columns: [
            { field: "TypeOfGoods", title: "Type Of Goods" },
            { field: "NameOfGoods", title: "Name Of Goods" } 
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.InitAccDesc(data);
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.TypeOfGoods = function () {
        var lookup = Wx.blookup({
            name: "TypeOfGoodsLookup",
            title: "Type Of Goods Lookup",
            manager: spManager,
            query: "GLAccMappingBrowse",
            defaultSort: "TypeOfGoods asc",
            columns: [
            { field: "TypeOfGoods", title: "Type Of Goods" },
            { field: "NameOfGoods", title: "Name Of Goods" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.InitAccDesc(data);  
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.getInfoAcc = function (AccNo, name) {
        //var id = {
        //    'CompanyCode': me.data.CompanyCode,
        //    'BranchCode': me.data.BranchCode,
        //    'Search': AccNo
        //};
        //$http.post("gn.api/GLAccMapping/GLAccScalar?id=" + "'" + me.data.CompanyCode + "'" + ", " + "'" + me.data.BranchCode + "'" + ", " + "'" + AccNo + "'").
        //$http.post("gn.api/GLAccMapping/GLAccScalar", id).
        $http.get("gn.api/GLAccMapping/GLAccScalar?id=" + AccNo).
            success(function (v, status, headers, config) {
                //alert(v);
                //alert(name);
                if (v) {
                    switch (name) {
                        case "SalesAccNoName":
                            me.combo.SalesAccNoName = v;
                            break;
                        case "COGSAccNoName":
                            me.combo.COGSAccNoName = v;
                            break;
                        case "InventoryAccNoName":
                            me.combo.InventoryAccNoName = v;
                            break;
                        case "DiscAccNoName":
                            me.combo.DiscAccNoName = v;
                            break;
                        case "ReturnAccNoName":
                            me.combo.ReturnAccNoName = v;
                            break;
                        case "ReturnPybAccNoName":
                            me.combo.ReturnPybAccNoName = v;
                            break;
                        case "OtherIncomeAccNoName":
                            me.combo.OtherIncomeAccNoName = v;
                            break;
                        case "OtherReceivableAccNoName":
                            me.combo.OtherReceivableAccNoName = v;
                            break;
                        case "InTransitAccNoName":
                            me.combo.InTransitAccNoName = v;
                            break;
                    }
                } else {
                    switch (name) {
                        case "SalesAccNoName":
                            me.data.SalesAccNo = '';
                            me.combo.SalesAccNoName = '';
                            me.loadAccount('SalesAccNo');
                            break;
                        case "COGSAccNoName":
                            me.data.COGSAccNo = '';
                            me.combo.COGSAccNoName = '';
                            me.loadAccount('COGSAccNo');
                            break;
                        case "InventoryAccNoName":
                            me.data.InventoryAccNo = '';
                            me.combo.InventoryAccNoName = '';
                            me.loadAccount('InventoryAccNo');
                            break;
                        case "DiscAccNoName":
                            me.data.DiscAccNo = '';
                            me.combo.DiscAccNoName = '';
                            me.loadAccount('DiscAccNo');
                            break;
                        case "ReturnAccNoName":
                            me.data.ReturnAccNo = '';
                            me.combo.ReturnAccNoName = '';
                            me.loadAccount('ReturnAccNo');
                            break;
                        case "ReturnPybAccNoName":
                            me.data.ReturnPybAccNo = '';
                            me.combo.ReturnPybAccNoName = '';
                            me.loadAccount('ReturnPybAccNo');
                            break;
                        case "OtherIncomeAccNoName":
                            me.data.OtherIncomeAccNo = '';
                            me.combo.OtherIncomeAccNoName = '';
                            me.loadAccount('OtherIncomeAccNo');
                            break;
                        case "OtherReceivableAccNoName":
                            me.data.OtherReceivableAccNo = '';
                            me.combo.OtherReceivableAccNoName = '';
                            me.loadAccount('OtherReceivableAccNo');
                            break;
                        case "InTransitAccNoName":
                            me.data.InTransitAccNo = '';
                            me.combo.InTransitAccNoName = '';
                            me.loadAccount('InTransitAccNo');
                            break;
                    }
                }
                //me.Apply();
            });
    }

    me.getInfoAllAcc = function (TypeOfGoods, name) {
        $http.post('gn.api/GLAccMapping/getRecord?TypeOfGoods=' + TypeOfGoods).
            success(function (v, status, headers, config) {
                if (v.success) {
                    me.lookupAfterSelect(v.data);
                    me.data.NameOfGoods = name;
                    me.InitAccDesc(v.data);
                    //me.Apply();
                } else {
                    //$('#PartNo').val('');
                    me.data.TypeOfGoods = '';
                    me.data.NameOfGoods = '';
                    me.TypeOfGoods();
                }
            });
    }

    me.getNameBlur = function (xNo, xName) {
        if ($('#' + xNo).val() || $('#' + xNo).val() != '') {
            me.getInfoAcc($('#' + xNo).val(), xName);
        } else {
            $('#' + xNo).val('');
            $('#' + xName).val('');
            me.loadAccount(xNo);
        }
    }

    $("[name = 'TypeOfGoods']").on('blur', function () {
        if ($('#TypeOfGoods').val() || $('#TypeOfGoods').val() != '') {
            $http.post('gn.api/masteritem/GetLookupValueName?VarGroup=TPGO&varCode=' + $('#TypeOfGoods').val()).// 2parameter
             success(function (v, status, headers, config) {
                 if (v != "") {
                     $("[name = 'TypeOfGoods']").prop('disabled', true);
                     me.getInfoAllAcc($('#TypeOfGoods').val(), v);
               } else {
                   me.data.TypeOfGoods = '';
                   me.data.NameOfGoods = '';
                   me.TypeOfGoods();
               }
           });
        } else {
            me.data.TypeOfGoods = '';
            me.data.NameOfGoods = '';
            me.TypeOfGoods();
        }
    });

    $("[name = 'SalesAccNo']").on('blur', function () {
        if ($('#SalesAccNo').val() || $('#SalesAccNo').val() != '') {
            me.getInfoAcc($('#SalesAccNo').val(), 'SalesAccNoName');
        } else {
            $('#SalesAccNo').val('');
            $('#SalesAccNoName').val('');
            me.loadAccount('SalesAccNo');
        }
    });

    $("[name = 'COGSAccNo']").on('blur', function () {
        if ($('#COGSAccNo').val() || $('#COGSAccNo').val() != '') {
            me.getInfoAcc($('#COGSAccNo').val(), 'COGSAccNoName');
        } else {
            $('#COGSAccNo').val('');
            $('#COGSAccNoName').val('');
            me.loadAccount('COGSAccNo');
        }
    });

    $("[name = 'InventoryAccNo']").on('blur', function () {
        if ($('#InventoryAccNo').val() || $('#InventoryAccNo').val() != '') {
            me.getInfoAcc($('#InventoryAccNo').val(), 'InventoryAccNoName');
        } else {
            $('#InventoryAccNo').val('');
            $('#InventoryAccNoName').val('');
            me.loadAccount('InventoryAccNo');
        }
    });

    $("[name = 'DiscAccNo']").on('blur', function () {
        if ($('#DiscAccNo').val() || $('#DiscAccNo').val() != '') {
            me.getInfoAcc($('#DiscAccNo').val(), 'DiscAccNoName');
        } else {
            $('#DiscAccNo').val('');
            $('#DiscAccNoName').val('');
            me.loadAccount('DiscAccNo');
        }
    });

    $("[name = 'ReturnAccNo']").on('blur', function () {
        if ($('#ReturnAccNo').val() || $('#ReturnAccNo').val() != '') {
            me.getInfoAcc($('#ReturnAccNo').val(), 'ReturnAccNoName');
        } else {
            $('#ReturnAccNo').val('');
            $('#ReturnAccNoName').val('');
            me.loadAccount('ReturnAccNo');
        }
    });

    $("[name = 'ReturnPybAccNo']").on('blur', function () {
        if ($('#ReturnPybAccNo').val() || $('#ReturnPybAccNo').val() != '') {
            me.getInfoAcc($('#ReturnPybAccNo').val(), 'ReturnPybAccNoName');
        } else {
            $('#ReturnPybAccNo').val('');
            $('#ReturnPybAccNoName').val('');
            me.loadAccount('ReturnPybAccNo');
        }
    });

    $("[name = 'OtherIncomeAccNo']").on('blur', function () {
        if ($('#OtherIncomeAccNo').val() || $('#OtherIncomeAccNo').val() != '') {
            me.getInfoAcc($('#OtherIncomeAccNo').val(), 'OtherIncomeAccNoName');
        } else {
            $('#OtherIncomeAccNo').val('');
            $('#OtherIncomeAccNoName').val('');
            me.loadAccount('OtherIncomeAccNo');
        }
    });

    $("[name = 'OtherReceivableAccNo']").on('blur', function () {
        if ($('#OtherReceivableAccNo').val() || $('#OtherReceivableAccNo').val() != '') {
            me.getInfoAcc($('#OtherReceivableAccNo').val(), 'OtherReceivableAccNoName');
        } else {
            $('#OtherReceivableAccNo').val('');
            $('#OtherReceivableAccNoName').val('');
            me.loadAccount('OtherReceivableAccNo');
        }
    });

    $("[name = 'InTransitAccNo']").on('blur', function () {
        if ($('#InTransitAccNo').val() || $('#InTransitAccNo').val() != '') {
            me.getInfoAcc($('#InTransitAccNo').val(), 'InTransitAccNoName');
        } else {
            $('#InTransitAccNo').val('');
            $('#InTransitAccNoName').val('');
            me.loadAccount('InTransitAccNo');
        }
    });

    me.InitAccDesc = function (v) {
        var m = { model: v };
        $http.post("sp.api/glaccmapping/GLAccScalarAll", m).
            success(function (data, status, headers, config) {
                me.combo = data;
                //me.Apply();
            });
    }

    me.loadAccount = function (s) {

        var lookup = Wx.blookup({
            name: "glAccountBrowse",
            title: "Lookup Account",
            manager: spManager,
            query: "glaccbrowse",
            defaultSort: "AccountNo asc",
            columns: [
            { field: "AccountNo", title: "AccountNo" },
            { field: "Description", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {            
                me.data[s] = data.AccountNo
                me.combo[s + 'Name'] = data.Description
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.saveData = function (e, param) {
        $http.post('sp.api/glaccmapping/save', me.data).
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


    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/glaccmapping/delete', me.data).
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

    me.InitAccDesc(null);

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "GL Account Mapping",
        xtype: "panels",
        toolbars:WxButtons,
        panels: [     
            {
                name: "pnlAc",
                title: "",
                items: [
                {
                    text: "Type Of Goods",
                    type: "controls",
                    required : true,
                    items: [
                      { name: "TypeOfGoods", cls: "span2", text: "Type Of Goods", type: "popup", btnName: "btnTypeOfGoods", readonly: false, click: "TypeOfGoods()", required: true, validasi: "required" },//, disable:"data.TypeOfGoods != undefined" },
                      { name: "NameOfGoods", cls: "span6", placeHolder: "Name Of Goods", readonly: true, disable: "data.TypeOfGoods != undefined" }
                    ]
                },
                ]
            },
            {
                name: "pnlAcc",
                title: "Account Details",
                items: [
                        {
                            text: "Sales Acc No",
                            type: "controls",
                            items: [
                                {
                                    name: "SalesAccNo", cls: "span4", placeHolder: "Sales Acc No", type: "popup", btnName: "btnSalesAccNo", readonly: false
                                    , click: "TypeOfGoods()"
                                    , click: "loadAccount('SalesAccNo')"
                                    //, event: "{blur:'getNameBlur(me.data.SalesAccNo)'}"
                                },
                                { name: "SalesAccNoName", cls: "span4", placeHolder: "SalesAcc Name", readonly: true, model: "combo.SalesAccNoName" }
                            ]
                        },
                        {
                            text: "COGS Acc No",
                            type: "controls",

                            items: [
                                {
                                    name: "COGSAccNo", cls: "span4", placeHolder: "COGS Acc No", type: "popup", btnName: "btnCOGSAccNo", readonly: false
                                    , click: "loadAccount('COGSAccNo')"
                                   // , event: "{blur:'getNameBlur('COGSAccNo','COGSAccNoName')'}"
                                },
                                { name: "COGSAccNoName", model: "combo.COGSAccNoName", cls: "span4", placeHolder: "COGSAcc Name", readonly: true }
                            ]
                        },
                         {
                             text: "Inventory Acc No",
                             type: "controls",

                             items: [
                                 { name: "InventoryAccNo", cls: "span4", placeHolder: "Inventory Acc No", type: "popup", btnName: "btnInventoryAccNo", readonly: false, click: "loadAccount('InventoryAccNo')" },
                                 { name: "InventoryAccNoName", model: "combo.InventoryAccNoName", cls: "span4", placeHolder: "InventoryAcc Name", readonly: true }
                             ]
                         },
                        {
                            text: "Disc Acc No",
                            type: "controls",
                            items: [                                                                                                                                                                                                                                                                                                         
                                { name: "DiscAccNo", cls: "span4", placeHolder: "Disc Acc No", type: "popup", btnName: "btnDiscAccNo", readonly: false, click: "loadAccount('DiscAccNo')" },
                                { name: "DiscAccNoName", model: "combo.DiscAccNoName", cls: "span4", placeHolder: "DiscAcc Name", readonly: true }
                            ]
                        },
                        {
                            text: "Return Acc No",
                            type: "controls",

                            items: [
                                { name: "ReturnAccNo", cls: "span4", placeHolder: "Return Acc No", type: "popup", btnName: "btnReturnAccNo", readonly: false, click: "loadAccount('ReturnAccNo')" },
                                { name: "ReturnAccNoName", model: "combo.ReturnAccNoName", cls: "span4", placeHolder: "ReturnAcc Name", readonly: true }
                            ]
                        },
                        {
                            text: "Return PybAcc No",
                            type: "controls",

                            items: [
                                { name: "ReturnPybAccNo", cls: "span4", placeHolder: "Return Pyb Acc No", type: "popup", btnName: "btnReturnPybAccNo", readonly: false, click: "loadAccount('ReturnPybAccNo')" },
                                { name: "ReturnPybAccNoName", model: "combo.ReturnPybAccNoName", cls: "span4", placeHolder: "ReturnPybAcc Name", readonly: true }
                            ]
                        },
                        {
                            text: "Other Income Acc No",
                            type: "controls",

                            items: [
                                { name: "OtherIncomeAccNo", cls: "span4", placeHolder: "Other Income Acc No", type: "popup", btnName: "btnOtherIncomeAccNo", readonly: false, click: "loadAccount('OtherIncomeAccNo')" },
                                { name: "OtherIncomeAccNoName", model: "combo.OtherIncomeAccNoName", cls: "span4", placeHolder: "OtherIncomeAcc Name", readonly: true }
                            ]
                        },
                        {
                            text: "Other Receivable Acc No",
                            type: "controls",

                            items: [
                                { name: "OtherReceivableAccNo", cls: "span4", placeHolder: "Other Receivable Acc No", type: "popup", btnName: "btnOtherReceivableAccNo", readonly: false, click: "loadAccount('OtherReceivableAccNo')" },
                                { name: "OtherReceivableAccNoName", model: "combo.OtherReceivableAccNoName", cls: "span4", placeHolder: "OtherReceivableAcc Name", readonly: true }
                            ]
                        },
                        {
                            text: "InTransit Acc No", 
                            type: "controls", 

                            items: [
                                { name: "InTransitAccNo", cls: "span4", placeHolder: "In Transit Acc No", type: "popup", btnName: "btnInTransitAccNo", readonly: false, click: "loadAccount('InTransitAccNo')" },
                                { name: "InTransitAccNoName", model: "combo.InTransitAccNoName", cls: "span4", placeHolder: "InTransitAcc Name", readonly: true }
                            ]
                        },
                ]
            },
        ]
    };
   
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spglAccMappingController");
    }

});