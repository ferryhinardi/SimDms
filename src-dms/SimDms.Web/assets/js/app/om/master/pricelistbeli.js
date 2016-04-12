var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterPriceListJualController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.$watch('isActive', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            if (newValue == '1') { me.data.Total = 0; }
            if (newValue == '0') { me.data.DPP = 0; me.data.PPn = 0; }
        }
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "PriceListBeliBrowse",
            title: "PriceList Beli Browse",
            manager: spSalesManager,
            query: "PriceListBeliBrowse",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" },
                { field: "SalesModelYear", title: "Sales Model Year" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.data.readonly = true;
                me.isSave = false;
                me.Apply();
                me.data.readonly = true;
            }

        });
    }

    me.SupplierCodeLookup = function () {
        var lookup = Wx.blookup({
            name: "SupplierCodeLookup",
            title: "Supplier Code",
            manager: spSalesManager,
            query: "SupplierCodeLookup",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "SupplierName", title: "Supplier Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.Apply();
                $('#SupplierCode').attr('disabled', 'disabled');
            }
        });

    }

    me.SalesModelCodeLookup = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCode = data.SalesModelCode;
                me.data.SalesModelDesc = data.SalesModelDesc;
                me.Apply();
                $('#SalesModelCode').attr('disabled', 'disabled');
            }

            //reset value
            me.data.SalesModelYear = "";
        });

    }

    me.SalesModelYearLookup = function () {
        var lookup = Wx.blookup({
            name: "SalesModelYearLookup",
            title: "Model year",
            manager: spSalesManager,
            //query: "SalesModelYearLookup",
            query: "SalesModelYearLookup?SalesModelCode=" + me.data.SalesModelCode,
            defaultSort: "SalesModelYear asc",
            columns: [
                { field: "SalesModelYear", title: "Sales Model Year" },
                { field: "SalesModelDesc", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelYear = data.SalesModelYear;
                me.Apply();
                $http.post('om.api/MstPriceListBeli/PriceListBeli', me.data).
                success(function (data, status, headers, config) {
                    //me.data = data.data;
                    if (data.success) {
                        me.data.PPnBMPaid = data.data.PPnBMPaid;
                        me.data.isActive = data.data.isActive;
                        me.data.DPP = data.data.DPP;
                        me.data.PPn = data.data.PPn;
                        me.data.PPnBM = data.data.PPnBM;
                        me.data.Total = data.data.Total;
                        me.data.Remark = data.data.Remark;
                        me.data.Status = data.data.Status;
                        $('#SalesModelCode').attr('disabled', 'disabled');
                        me.Apply();
                        me.ReformatNumber();
                    }
                }).
                error(function (data, status, headers, config) {
                    alert('error');
                });
            }
        });

    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstPriceListBeli/Delete', me.data).
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
        var data = {
            DPP: me.data.DPP,
            PPN: me.data.PPN,
            PPnBMPaid: me.data.PPnBMPaid,
            SalesModelCode: me.data.SalesModelCode,
            SalesModelDesc: me.data.SalesModelDesc,
            SalesModelYear: me.data.SalesModelYear,
            Status: me.data.Status,
            SupplierCode: me.data.SupplierCode,
            SupplierName: me.data.SupplierName,
            Total: me.data.Total,
            isActive: me.data.isActive
        }

        $http.post('om.api/MstPriceListBeli/Save', data).
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

    me.initialize = function () {
        var date = new Date;
        me.hasChanged = false;
        me.isActive = "1";
        $('#SalesModelCode').removeAttr('disabled');
        $('#SupplierCode').removeAttr('disabled');
        $('#isActive').prop('checked', true);
        me.data.isActive = true;
        me.data.Status = true;
    }

    $("[name='SupplierCode']").on('blur', function () {
        if (me.data.SupplierCode != null) {
            $http.post('om.api/MstPriceListBeli/SupplierCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.SupplierName = data.data.SupplierName;
                       $('#SupplierCode').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.SupplierCode = "";
                       me.data.SupplierName = "";
                       me.SupplierCodeLookup();
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    $("[name='SalesModelCode']").on('blur', function () {
        if (me.data.SalesModelCode != null) {
            $http.post('om.api/MstPriceListBeli/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.SalesModelDesc = data.data.SalesModelDesc;
                       $('#SalesModelCode').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.SalesModelCode = "";
                       me.data.SalesModelDesc = "";
                       me.SalesModelCodeLookup();
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    var w, x, y, z;

    $("[name='DPP']").on('blur', function () {
        CalculateJumlah();
    });

    $("[name='PPn']").on('blur', function () {
        CalculateJumlah();
    });

    $("[name='PPnBM']").on('blur', function () {
        CalculateJumlah();
    });

    $("[name='Total']").on('blur', function () {
        if (me.data.SupplierCode != undefined || me.data.SalesModelCode != undefined || me.data.SalesModelYear != undefined) {
            $http.post('om.api/MstPriceListBeli/txtTotal_Validated', me.data).
                   success(function (data, status, headers, config) {
                       if (data.success) {
                           me.data.DPP = data.DPP;
                           me.data.PPn = data.PPn;
                           me.$apply;
                           me.ReformatNumber();
                       }
                   }).
                   error(function (data, status, headers, config) {
                       alert('error');
                   });
        }
    });

    function CalculateJumlah() {
        x = $("[name='DPP']").val();
        y = $("[name='PPn']").val();
        z = $("[name='PPnBM']").val();
        x = x.split(',').join('');
        y = y.split(',').join('');
        z = z.split(',').join('');
        var n = parseFloat(x) * 10 / 100;
        var m = parseFloat(x) + (n) + parseFloat(z);
        //var temp1 = m.toString();
        //var jum1 = temp1.substring(0, 4);
        //alert(w);
        $("[name='PPn']").val(n);
        $("[name='Total']").val(m);
        me.data.PPN = (n);
        me.data.Total = (m);

    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Price List Beli",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "PriceListJual",
                title: "Price List Beli",
                items: [
                        {
                            text: "Pemasok",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SupplierCode", cls: "span2", placeHolder: "SupplierCode", type: "popup", btnName: "btnSupplierCode", click: "SupplierCodeLookup()", required: true, validasi: "required" },
                                { name: "SupplierName", cls: "span4", placeHolder: "SupplierName", model: "data.SupplierName", readonly: true },
                            ]
                        },
                        {
                            text: "Sales Model Code",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SalesModelCode", cls: "span2", placeHolder: "SalesModelCode", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCodeLookup()", disable: "data.SupplierCode == undefined", required: true, validasi: "required" },
                                { name: "SalesModelDesc ", cls: "span4", placeHolder: "SalesModelDesc", model: "data.SalesModelDesc", readonly: true },
                            ]
                        },
                        {
                            text: "Sales Model Year",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SalesModelYear", cls: "span2", placeHolder: "SalesModelYear", readonly: true, type: "popup", btnName: "btnSalesModelYear", click: "SalesModelYearLookup()", disable: "data.SalesModelCode == undefined", required: true, validasi: "required" },
                            ]
                        },
                        { name: "PPnBMPaid", text: "PPn BM Paid", cls: "span4 number-int full", value: 0, required: true },
                        { type: "hr" },
                        {
                            type: "optionbuttons",
                            name: "isActive",
                            model: "isActive",
                            items: [
                                { name: "1", text: "Aktif" },
                                { name: "0", text: "Tidak Aktif" },
                            ]
                        },
                        //{ name: 'isActive', type: 'check', cls: "span1", text: "", float: 'left' },
                        { name: "DPP", text: "DPP", cls: "span4 number-int full", value: 0, disable: "isActive == 0", required: true },
                        { name: "PPn", text: "PPn", cls: "span4 number-int full", value: 0, disable: "isActive == 0" },
                        { name: "PPnBM", text: "PPn BM", cls: "span4 number-int full", value: 0, disable: "isActive == 0" },
                        { name: "Total", text: "Jumlah", cls: "span4 number-int full", value: 0, disable: "isActive == 1", required: true },
                        { name: "Remark", text: "Keterangan", cls: "span8", maxlength: 100 },
                        { name: "Status", text: "Status", type: "x-switch", cls: "span4" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterPriceListJualController");
    }

});