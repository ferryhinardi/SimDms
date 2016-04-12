var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterBBNKIRController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "BBNKIRBrowse",
            title: "BBN dan KIR Browse",
            manager: spSalesManager,
            query: "BBNKIRBrowse",
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
                { field: "CityCode", title: "Kode Kota" },
                { field: "CityName", title: "Nama Kota" },
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelYear", title: "Sales Model Year" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.data.SalesModelYearDesc = result.SalesModelDesc;
                me.data.Status = result.Status == '0' ? false : true;
                me.isSave = false;
                me.Apply();

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
                cekdata();
                $('#SupplierCode').attr('disabled', 'disabled');
            }
        });

    }


    me.CityCodeLookup = function () {
        var lookup = Wx.blookup({
            name: "CityCodeLookup",
            title: "City Code",
            manager: spSalesManager,
            query: "CityCodeLookup",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "City Code" },
                { field: "LookUpValueName", title: "City Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CityCode = data.LookUpValue;
                me.data.CityName = data.LookUpValueName;
                me.isSave = false;
                me.Apply();
                cekdata();
                $('#CityCode').attr('disabled', 'disabled');
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
                cekdata();
                $('#SalesModelCode').attr('disabled', 'disabled');
            }

            //reset value
            me.data.SalesModelYear = "";
            me.data.SalesModelYearDesc = "";
        });

    }

    me.SalesModelYearLookup = function () {
        var lookup = Wx.blookup({
            name: "SalesModelYearLookup",
            title: "Marke tModel",
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
                cekdata();
                me.data.SalesModelYear = data.SalesModelYear;
                me.data.SalesModelYearDesc = data.SalesModelDesc;
                me.isSave = false;
                me.Apply();
                $('#SalesModelYear').attr('disabled', 'disabled');
            }
        });

    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstBBNKIR/Delete', me.data).
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
        $http.post('om.api/MstBBNKIR/Save', me.data).
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
        me.data.Status = true;
        $('#SupplierCode').removeAttr('disabled');
        $('#SalesModelCode').removeAttr('disabled');
        $('#CityCode').removeAttr('disabled');
        $('#SalesModelYear').removeAttr('disabled');
    }


    $("[name='SupplierCode']").on('blur', function () {
        if (me.data.SupplierCode != null) {
            $http.post('om.api/MstBBNKIR/SupplierCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       cekdata();
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

    $("[name='CityCode']").on('blur', function () {
        if (me.data.CityCode != null) {
            $http.post('om.api/MstBBNKIR/CityCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       cekdata();
                       me.data.CityName = data.data.LookUpValueName;
                       $('#SupplierCode').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.CityCode = "";
                       me.data.CityName = "";
                       me.CityCodeLookup();
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
            $http.post('om.api/MstBBNKIR/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       cekdata();
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

    $("[name='SalesModelYear']").on('blur', function () {
        if (me.data.SalesModelYear != null) {
            $http.post('om.api/MstBBNKIR/ModelYear', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       cekdata();
                       me.data.SalesModelYearDesc = data.data.SalesModelDesc;
                       $('#SalesModelYear').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.SalesModelYear = "";
                       me.data.SalesModelYearDesc = "";
                       me.SalesModelYearLookup();
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    function cekdata(data) {

        $http.post('om.api/MstBBNKIR/CekData', me.data).
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.data.BBN = data.data.BBN;
                   me.data.KIR = data.data.KIR;
                   me.data.Remark = data.data.Remark;
                   me.data.Status = data.data.Status == 0 ? false : true;
                   me.Apply();
                   me.ReformatNumber();
               }
               else
               {
                   me.data.BBN = "";
                   me.data.KIR = "";
                   me.data.Status = "true";
               }
           }).
           error(function (data, status, headers, config) {
               alert('error');
           });
    }

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "BBN dan KIR",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "BBNKIR",
                title: "BBN dan KIR",
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
                            text: "Kota",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "CityCode", cls: "span2", placeHolder: "CityCode", type: "popup", btnName: "btnCityCode", click: "CityCodeLookup()", required: true, validasi: "required" },
                                { name: "CityName ", cls: "span4", placeHolder: "CityName", model: "data.CityName", readonly: true },
                            ]
                        },
                        {
                            text: "Sales Model Code",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SalesModelCode", cls: "span2", placeHolder: "SalesModelCode", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCodeLookup()", required: true, validasi: "required" },
                                { name: "SalesModelDesc ", cls: "span4", placeHolder: "SalesModelDesc", model: "data.SalesModelDesc", readonly: true },
                            ]
                        },
                        {
                            text: "Sales Model Year",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SalesModelYear", cls: "span2", placeHolder: "SalesModelYear", type: "popup", btnName: "btnSalesModelYear", click: "SalesModelYearLookup()", disable: "data.SalesModelCode == undefined", required: true, validasi: "required" },
                                { name: "SalesModelYearDesc ", cls: "span4", placeHolder: "SalesModelYearDesc", model: "data.SalesModelYearDesc", readonly: true },
                            ]
                        },
                        { name: "BBN", cls: "span3 number-int full", value: 0, text: "BBN", required: true, validasi: "required" },
                        { name: "KIR", cls: "span3 number-int full", value: 0, text: "KIR", required: true, validasi: "required" },
                        { name: "Remark", cls: "span3 full", text: "Keterangan", maxlength:100 },
                        { name: "Status", text: "Status", type: "ng-switch", cls: "span2" },

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterBBNKIRController");
    }

});