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
            name: "PriceListJualBrowse",
            title: "PriceList Jual Browse",
            manager: spSalesManager,
            query: "PriceListJualBrowse",
            defaultSort: "GroupPriceCode asc",
            columns: [
                { field: "GroupPriceCode", title: "Group Price" },
                { field: "GroupPriceName", title: "Keterangan" },
                { field: "SalesModelCode", title: "Kode Model" },
                { field: "SalesModelDesc", title: "Diskripsi" },
                { field: "SalesModelYear", title: "Tahun Model" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.data.Status = result.Status == 0 ? false : true;
                me.isSave = false;
                me.Apply();

            }

        });
    }

    me.GroupPriceCodeLookup = function () {
        var lookup = Wx.blookup({
            name: "GroupPriceCodeLookup",
            title: "Group Price",
            manager: spSalesManager,
            query: "GroupPriceCodeLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Kode Reff." },
                { field: "RefferenceDesc1", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.GroupPriceCode = data.RefferenceCode;
                me.data.GroupPriceName = data.RefferenceDesc1;
                me.Apply();
                if (me.data.SalesModelCode != undefined && me.data.SalesModelYear != undefined) {
                    cekdata(data);
                }
                $('#GroupPriceCode').attr('disabled', 'disabled');
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
                if (me.data.GroupPriceCode != undefined && me.data.SalesModelYear != undefined) {
                    cekdata(data);
                }
                $('#SalesModelCode').attr('disabled', 'disabled');
            }
            //reset value
            me.data.SalesModelYear = "";
            me.data.TaxCode = "";
            me.data.TaxPct = "";
            me.isActive = "1";
            me.data.Status = "true";
            me.data.DPP = 0;
            me.data.PPn = 0;
            me.data.PPnBM = 0;
            me.data.Total = 0;
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
                me.data.SalesModelYear = data.SalesModelYear;
                me.data.SalesModelYearDesc = data.SalesModelDesc;
                me.Apply();
                if (me.data.SalesModelCode != undefined && me.data.GroupPriceCode != undefined) {
                    cekdata(data);
                }
            }
        });

    }

    me.TaxCodeLookup = function () {
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
                me.data.TaxCode = data.TaxCode
                me.data.TaxPct = data.TaxPct;
                me.Apply();
                $('#TaxCode').attr('disabled', 'disabled');
            }
        });

    }


    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstPriceListJual/Delete', me.data).
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
        $http.post('om.api/MstPriceListJual/save', me.data).
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
        me.data.Status = "true";
        me.data.DPP = 0;
        me.data.PPn = 0;
        me.data.PPnBM = 0;
        $('#GroupPriceCode').removeAttr('disabled');
        $('#SalesModelCode').removeAttr('disabled');
        $('#TaxCode').removeAttr('disabled');
    }

    $("[name='GroupPriceCode']").on('blur', function () {
        if (me.data.GroupPriceCode != null) {
            $http.post('om.api/MstPriceListJual/GroupPriceCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.GroupPriceName = data.data.RefferenceDesc1;
                       $('#GroupPriceCode').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.GroupPriceCode = "";
                       me.data.GroupPriceName = "";
                       me.GroupPriceCodeLookup();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='SalesModelCode']").on('blur', function () {
        if (me.data.SalesModelCode != null) {
            $http.post('om.api/MstPriceListJual/ModelCode', me.data).
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

    $("[name='TaxCode']").on('blur', function () {
        if (me.data.TaxCode != null) {
            $http.post('om.api/MstPriceListJual/TaxCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.TaxPct = data.data.TaxPct;
                       $('#TaxCode').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.TaxCode = "";
                       me.data.TaxPct = "";
                       me.TaxCodeLookup();
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
        $http.post('om.api/MstPriceListJual/txtTotal_Validated', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.DPP = data.DPP;
                       me.data.PPn = data.PPn;
                       me.Apply();
                       me.ReformatNumber();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
    });

    function CalculateJumlah() {
        w = me.data.TaxPct;
        x = $("[name='DPP']").val();
        y = $("[name='PPn']").val();
        z = $("[name='PPnBM']").val();
        x = x.split(',').join('');
        y = y.split(',').join('');
        z = z.split(',').join('');
        var n = parseFloat(x) * parseFloat(w) / 100;
        var m = parseFloat(x) + (n) + parseFloat(z);
        //var temp1 = m.toString();
        //var jum1 = temp1.substring(0, 4);
        $("[name='PPn']").val(n);
        $("[name='Total']").val(m);
        me.data.PPn = n;
        me.data.Total = m;
        me.data.PPn = n;
        me.data.Total = m;
        me.Apply();
    }

    function cekdata (data) {

        $http.post('om.api/MstPriceListJual/CekData', me.data).
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.data.TaxCode = data.data.TaxCode;
                   me.data.TaxPct = data.data.TaxPct;
                   me.data.DPP = data.data.DPP;
                   me.data.PPn = data.data.PPn;
                   me.data.PPnBM = data.data.PPnBM;
                   me.data.Total = data.data.Total;
                   me.data.Remark = data.data.Remark;
                   me.data.TotalMinStaff = data.data.TotalMinStaff;
                   me.data.Status = data.data.Status == 0 ? false : true;
                   me.Apply();
                   me.ReformatNumber();
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
        title: "Price List Jual",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "PriceListJual",
                title: "Price List Jual",
                items: [
                        {
                            text: "Group Price",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "GroupPriceCode", cls: "span2", placeHolder: "GroupPriceCode", type: "popup", btnName: "btnGroupPriceCode", click: "GroupPriceCodeLookup()", required: true, validasi: "required" },
                                { name: "GroupPriceName", cls: "span4", placeHolder: "GroupPriceName", model: "data.GroupPriceName", readonly: true },
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
                                { name: "SalesModelYear", cls: "span2", placeHolder: "SalesModelYear", readonly: true, type: "popup", btnName: "btnSalesModelYear", click: "SalesModelYearLookup()", disable: "data.SalesModelCode == undefined", required: true, validasi: "required" },
                            ]
                        },
                        { type: "hr" },
                         {
                             text: "PPn",
                             type: "controls",
                             required: true,
                             items: [
                                 { name: "TaxCode", cls: "span2", placeHolder: "TaxCode", type: "popup", btnName: "btnTaxCode", click: "TaxCodeLookup()", required: true, validasi: "required" },
                                 { name: "TaxPct ", cls: "span2", value: 0, placeHolder: "TaxPct", model: "data.TaxPct", readonly: true, type: "decimal" },
                             ]
                         },
                         {
                             type: "optionbuttons",
                             name: "isActive",
                             model: "isActive",
                             items: [
                                 { name: "1", text: "Aktif" },
                                 { name: "0", text: "Tidak Aktif" },
                             ]
                         },
                        { name: "DPP", text: "DPP", cls: "span4 number-int full", value: 0, disable: "isActive == 0", required: true },
                        { name: "PPn", model: "data.PPn", text: "PPn", cls: "span4 number-int full", value: 0, disable: "isActive == 0" },
                        { name: "PPnBM", model: "data.PPnBM", text: "PPn BM", cls: "span4 number-int full", value: 0, disable: "isActive == 0" },
                        { name: "Total", model: "data.Total", text: "Jumlah", cls: "span4 number-int full", value: 0, disable: "isActive == 1", required: true },
                        { name: "Remark", text: "Keterangan", cls: "span8", maxlength: 100 },
                        { type: "hr" },
                        { name: "TotalMinStaff", text: "Batas Harga Bawah", cls: "span4 number-int full", value: 0 },
                        { type: "hr" },
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