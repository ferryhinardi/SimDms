var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterKaroseriController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "KaroseriBrowse",
            title: "Karoseri Browse",
            manager: spSalesManager,
            query: "KaroseriBrowse",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Desc" },
                { field: "SalesModelCodeNew", title: "Sales Model Code New" },
                { field: "SalesModelDescNew", title: "Sales Model Desc New" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                me.data.Status = result.Status == 0 ? false : true;
                me.Apply();
                $('#SupplierCode').attr('disabled', 'disabled');
                $('#btnSupplierCode').attr('disabled', 'disabled');
                $('#SalesModelCode').attr('disabled', 'disabled');
                $('#btnSalesModelCode').attr('disabled', 'disabled');

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
                me.isSave = false;
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
                me.isSave = false;
                me.Apply();
                $('#SalesModelCode').attr('disabled', 'disabled');
                $http.post('om.api/MstKaroseri/KaroseriView', me.data).
                success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data = data.data;
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

    me.SalesModelCodeNewLookup = function () {
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
            if (data != null ) {
                me.data.SalesModelCodeNew = data.SalesModelCode;
                me.data.SalesModelDescNew = data.SalesModelDesc;
                me.isSave = false;
                me.Apply();
                $('#SalesModelCodeNew').attr('disabled', 'disabled');
            }

            if (me.data.SalesModelCodeNew == me.data.SalesModelCode) {
                MsgBox("Sales Model lama tidak boleh sama dengan Sales Model baru", MSG_ERROR);
                me.data.SalesModelCodeNew = "";
                me.data.SalesModelDescNew = "";
                me.Apply();

                //$('#SalesModelCodeNew').val(); 
            }
        });

    }


    me.initialize = function () {
        var date = new Date;
        me.hasChanged = false;
        me.data.Status = true;
        $('#SupplierCode').removeAttr('disabled');
        $('#btnSupplierCode').removeAttr('disabled');
        $('#SalesModelCode').removeAttr('disabled');
        //$('#btnSalesModelCode').removeAttr('disabled');
        $('#SalesModelCodeNew').removeAttr('disabled');

    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstKaroseri/Delete', me.data).
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
        $http.post('om.api/MstKaroseri/Save', me.data).
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

    $("[name='SupplierCode']").on('blur', function () {
        if (me.data.SupplierCode != null) {
            $http.post('om.api/MstKaroseri/SupplierCode', me.data).
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
            $http.post('om.api/MstKaroseri/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.SalesModelDesc = data.data.SalesModelDesc;
                       $('#SalesModelCode').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.SalesModelCode = "";
                       me.data.SalesModelDesc = "";
                       me.SalesModelCodeNewLookup();
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
            $http.post('om.api/MstKaroseri/KaroseriView', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data = data.data;
                   }
                   else {
                       me.data.SalesModelCode = "";
                       me.data.SalesModelDesc = "";
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='SalesModelCodeNew']").on('blur', function () {
        if (me.data.SalesModelCodeNew != null) {
            $http.post('om.api/MstKaroseri/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.SalesModelDescNew = data.data.SalesModelDesc;
                       $('#SalesModelCodeNew').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.SalesModelCodeNew = "";
                       me.data.SalesModelDescNew = "";
                       me.SalesModelCodeNewLookup();
                   }

                   if (me.data.SalesModelCodeNew == me.data.SalesModelCode) {
                       MsgBox("Sales Model lama tidak boleh sama dengan Sales Model baru", MSG_ERROR);
                       me.data.SalesModelCodeNew = "";
                       me.data.SalesModelDescNew = "";
                       me.Apply();
                       $('#SalesModelCodeNew').removeAttr('disabled');
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    $("[name='DPPMaterial']").on('blur', function () {
        CalculateJumlah();
    });

    $("[name='DPPFee']").on('blur', function () {
        CalculateJumlah();
    });

    $("[name='DPPOthers']").on('blur', function () {
        CalculateJumlah();
    });


    function CalculateJumlah() {
        var a = $("[name='DPPMaterial']").val();
        var b = $("[name='DPPFee']").val();
        var c = $("[name='DPPOthers']").val();
        a = a.split(',').join('');
        b = b.split(',').join('');
        c = c.split(',').join('');

        var y = (parseFloat(a) + parseFloat(b) + parseFloat(c)) / 10;
        var z = parseFloat(a) + parseFloat(b) + parseFloat(c) + (y);
        
        $("[name='PPn']").val(y);
        $("[name='Total']").val(z);

        me.data.PPN = (y);
        me.data.Total = (z);

    }

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Karoseri",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "Karoseri",
                title: "Karoseri",
                items: [
                        {
                            text: "Pemasok",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SupplierCode", cls: "span2", placeHolder: "SupplierCode", type: "popup", btnName: "btnSupplierCode", click: "SupplierCodeLookup()" , required: true, validasi: "required" },
                                { name: "SupplierName", cls: "span4", placeHolder: "SupplierName", model: "data.SupplierName", readonly: true },
                            ]
                        },
                        {
                            text: "Sales Model Lama",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SalesModelCode", cls: "span2", placeHolder: "SalesModelCode", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCodeLookup()", disable: "data.SupplierCode === undefined", required: true, validasi: "required" },
                                { name: "SalesModelDesc ", cls: "span4", placeHolder: "SalesModelDesc", model: "data.SalesModelDesc", readonly: true },
                            ]
                        },
                        {
                            text: "Sales Model Baru",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SalesModelCodeNew", cls: "span2", placeHolder: "SalesModelCodeNew", type: "popup", btnName: "btnSalesModelCodeNew", click: "SalesModelCodeNewLookup()", disable: "data.SalesModelCode == undefined", required: true, validasi: "required" },
                                { name: "SalesModelDescNew ", cls: "span4", placeHolder: "SalesModelDescNew", model: "data.SalesModelDescNew", readonly: true },
                            ]
                        },
                        { name: "DPPMaterial", cls: "span4 number-int full", text: "DPP Material", required: true, validasi: "required", value: 0, maxlength: 18 },
                        { name: "DPPFee", cls: "span4 number-int full", text: "DPP Fee", value: 0, maxlength: 18 },
                        { name: "DPPOthers", cls: "span4 number-int full", text: "DPP Others", value: 0, maxlength: 18 },
                        { name: "PPn", cls: "span4 number-int full", text: "PPn", value: 0, maxlength: 18 },
                        { name: "Total", cls: "span4 total number-int full", text: "Total", value: 0, readonly: "true", maxlength: 18 },
                        { name: "Remark", cls: "span4 full", text: "Keterangan", maxlength:100},
                        { name: "Status", text: "Status", type: "x-switch", cls: "span2" },

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterKaroseriController");
    }

});