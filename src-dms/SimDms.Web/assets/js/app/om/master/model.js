var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var isread = true;


"use strict";

function ModulController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/Combo/LoadComboData?CodeId=PRDT').
    success(function (data, status, headers, config) {
        me.comboProductType = data;
    });

    me.initialize = function () {
        me.hasChanged = false;
        me.data.IsChassis = false;
        me.data.IsCbu = false;
        me.data.Status = "1";
        me.data.PpnBmPctPrincipal = 0;
        me.data.PpnBmPctBuy = 0;
        me.data.PpnBmPctSell = 0;
        $('#SalesModelCode').removeAttr('disabled');
        me.Apply();
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "ModulBrowse",
            title: "Modul Browse",
            manager: spSalesManager,
            query: "ModulBrowse",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" }
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                $('#SalesModelCode').prop('disabled', true);
                me.Apply();
                //$('#SalesModelCode').attr('disabled', 'disabled');
                
            }

        });

    }

    me.PpnBmCodePrincipal = function () {
        var lookup = Wx.blookup({
            name: "TaxCodeLookup",
            title: "Tax Code",
            manager: spSalesManager,
            query: "TaxCodeLookup",
            defaultSort: "TaxCode asc",
            columns: [
                { field: "TaxCode", title: "Tax Code" },
                {
                    field: "TaxPct", title: "Tax Pct",
                    template: '<div style="text-align:right;">#= kendo.toString(TaxPct, "n2") #</div>'
                }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PpnBmCodePrincipal = data.TaxCode
                me.data.PpnBmPctPrincipal = data.TaxPct;
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.PpnBmCodeBuy = function () {
        var lookup = Wx.blookup({
            name: "TaxCodeLookup",
            title: "Tax Code",
            manager: spSalesManager,
            query: "TaxCodeLookup",
            defaultSort: "TaxCode asc",
            columns: [
                { field: "TaxCode", title: "Tax Code" },
                {
                    field: "TaxPct", title: "Tax Pct",
                    template: '<div style="text-align:right;">#= kendo.toString(TaxPct, "n2") #</div>'
                }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PpnBmCodeBuy = data.TaxCode
                me.data.PpnBmPctBuy = data.TaxPct;
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.PpnBmCodeSell = function () {
        var lookup = Wx.blookup({
            name: "TaxCodeLookup",
            title: "Tax Code",
            manager: spSalesManager,
            query: "TaxCodeLookup",
            defaultSort: "TaxCode asc",
            columns: [
                { field: "TaxCode", title: "Tax Code" },
                {
                    field: "TaxPct", title: "Tax Pct",
                    template: '<div style="text-align:right;">#= kendo.toString(TaxPct, "n2") #</div>'
                }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PpnBmCodeSell = data.TaxCode
                me.data.PpnBmPctSell = data.TaxPct;
                me.isSave = false;
                me.Apply();
            }
        });
    }


    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstModel/Delete', me.data).
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
        $http.post('om.api/MstModel/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    $('#SalesModelCode').prop('disabled', true);

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                MsgBox("Delete Gagal", MSG_ERROR);
                console.log(e);
            });
    }

    $("[name='SalesModelCode']").on('blur', function () {
        if (me.data.SalesModelCode != null) {
            $http.post('om.api/MstModel/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data = data.data;
                       $('#SalesModelCode').prop('disabled', true);
                       me.Apply();

                       me.isLoadData = true;
                     
                           me.hasChanged = false;
                           me.startEditing();
                           me.isSave = false;
                           $scope.$apply();
                       
                      // $('#SalesModelCode').attr('disabled', 'disabled');
                   }
                   else {
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    me.start();
}

$(document).ready(function () {

    var options = {
        title: "Model",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlmodul",
                title: "Model",
                items: [
                        { name: "SalesModelCode", text: "Sales Model Code", cls: "span4", required: true, validasi: "required", maxlength:20, disabled:false },
                        { name: "SalesModelDesc", text: "Sales Model Desc", cls: "span4", required: true, validasi: "required", maxlength:100 },
                        { name: "EngineCode", text: "Engine Code", cls: "span4", required: true, validasi: "required", maxlength:15 },
                        { name: "FakturPolisiDesc", text: "Faktur Polisi Desc", cls: "span4", required: true, validasi: "required", maxlength:100 },
                        {
                            text: "PPN BM Principal",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "PpnBmCodePrincipal", cls: "span2", readonly: true, placeHolder: "PPN BM Principal", type: "popup", btnName: "btnPpnBmCodePrincipal", click: "PpnBmCodePrincipal()", required: true, validasi: "required" },
                                //{ name: "PpnBmPctPrincipal", cls: "span4 number", readonly: true }
                            { name: "PpnBmPctPrincipal", type: "decimal", cls: "span4", readonly: true }
                            ]
                        },
                        {
                            text: "PPN BM Beli",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "PpnBmCodeBuy", cls: "span2", readonly: true, placeHolder: "PPN BM Beli", type: "popup", btnName: "btnPpnBmCodeBuy", click: "PpnBmCodeBuy()", required: true, validasi: "required" },
                                //{ name: "PpnBmPctBuy", cls: "span4 number", readonly: true }
                            { name: "PpnBmPctBuy", type: "decimal", cls: "span4", readonly: true }
                            ]
                        },
                        {
                            text: "PPN BM Jual",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "PpnBmCodeSell", cls: "span2", readonly: true, placeHolder: "PPN BM Jual", type: "popup", btnName: "btnPpnBmCodeSell", click: "PpnBmCodeSell()", required: true, validasi: "required" },
                                //{ name: "PpnBmPctSell", cls: "span4 number", readonly: true } decimal
                                { name: "PpnBmPctSell", type:"decimal",cls: "span4", readonly: true }
                            ]
                        },
                        { name: "BasicModel", text: "Basic Model", cls: "span4", required: true, validasi: "required", maxlength: 15 },
                        { name: "TechnicalModelCode", text: "Technical Model", cls: "span4", required: true, validasi: "required", maxlength: 15 },
                        { name: "ProductType", cls: "span4 full", type: "select2", text: "Product Type", datasource: "comboProductType", required: true, validasi: "required" },
                        { name: "TransmissionType", text: "Transmission Type", cls: "span4 full", required: true, validasi: "required", maxlength: 10 },
                        { name: "IsChassis", text: "Chassis", type: "x-switch", cls: "span2"},
                        { name: "IsCbu", text: "CBU", type: "x-switch", cls: "span2"},
                        { name: "SMCModelCode", text: "SMC Model Code", cls: "span4", maxlength: 15 },
                        { name: "ModelLine", text: "Model Line", cls: "span4", maxlength: 100 },
                        { name: "GroupCode", text: "Group Code", cls: "span4", maxlength: 20 },
                        { name: "TypeCode", text: "Type Code", cls: "span4", maxlength: 50 },
                ]
            },
            {
                items: [
                        { name: "CylinderCapacity", text: "Cylinder Capacity"  , cls: "span4 number-int", value: 0, maxlength: 6 },
                        { name: "fuel", text: "fuel", cls: "span4", maxlength: 100 },
                        { name: "ModelPrincipal", text: "Model Principal", cls: "span4", maxlength: 100 },
                        { name: "Specification", text: "Specification", cls: "span4", maxlength: 100 },
                        { name: "Remark", text: "Keterangan", cls: "span8", maxlength: 100 },
                        {
                            type: "optionbuttons", name: "Status", model: "data.Status", text: "Status",
                            items: [
                                { name: "0", text: "Tidak Aktif" },
                                { name: "1", text: "Aktif" },
                                { name: "2", text: "Diskontinu" },
                            ]
                        },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("ModulController");
    }

});