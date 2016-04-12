var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterSuppliersController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('gn.api/Combo/LoadLookup?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboTPGO = data;
    });

    function lookupName(codeid, value) {
        $http.post('gn.api/Lookup/getLookupName?CodeId=' + codeid + '&LookupValue=' + value).
        success(function (v, status, headers, config) {
            if (v.TitleName != '') {
                //me.data.ProfitCenterCodeDesc = v.TitleName;
                $('#ProfitCenterCodeDesc').val(v.TitleName);
            } else {
                     $('#ProfitCenterCode').val('');
                     $('#ProfitCenterCodeDesc').val('');
                        me.ProfitCenterCodeDisc();
                }        
        });
    }

    function accountName(name, AccountNo, browse) {
        $http.post('gn.api/Lookup/getAccountName?AccountNo=' + AccountNo).
        success(function (v, status, headers, config) {
            if (v.TitleName != '') {
                switch (name) {
                    case 'PayableAccNoDesc':
                        $('#PayableAccNoDesc').val(v.TitleName);//me.data.PayableAccNoDesc = v.TitleName;
                        break;
                    case 'PayableAccNoDesc':
                        $('#PayableAccNoDesc').val(v.TitleName);//me.data.DownPaymentAccNoDesc = v.TitleName;
                        break;
                    case 'InterestAccNoDesc':
                        $('#InterestAccNoDesc').val(v.TitleName);//me.data.InterestAccNoDesc = v.TitleName;
                        break;
                    case 'TaxInAccNoDesc':
                        $('#TaxInAccNoDesc').val(v.TitleName); //me.data.TaxInAccNoDesc = v.TitleName;
                        break;
                    case 'WitholdingTaxAccNoDesc':
                        $('#WitholdingTaxAccNoDesc').val(v.TitleName); //me.data.WitholdingTaxAccNoDesc = v.TitleName;
                        break;
                    case 'OtherAccNoDesc':
                        $('#OtherAccNoDesc').val(v.TitleName); //me.data.OtherAccNoDesc = v.TitleName;
                        break;
                    case 'DownPaymentAccNoDesc':
                        $('#DownPaymentAccNoDesc').val(v.TitleName); //me.data.OtherAccNoDesc = v.TitleName;
                        break;
                }
            } else if(!browse){
                switch (name) {
                    case 'PayableAccNoDesc':
                        $('#PayableAccNo').val('');
                        $('#PayableAccNoDesc').val('');
                        me.Account();
                        break;
                    case 'DownPaymentAccNoDesc':
                        $('#DownPaymentAccNo').val('');
                        $('#DownPaymentAccNoDesc').val('');
                        me.Account1();
                        break;
                    case 'InterestAccNoDesc':
                        $('#InterestAccNo').val('');
                        $('#InterestAccNoDesc').val('');
                        me.Account2();
                        break;
                    case 'TaxInAccNoDesc':
                        $('#TaxInAccNo').val('');
                        $('#TaxInAccNoDesc').val('');
                        me.Account3();
                        break;
                    case 'WitholdingTaxAccNoDesc':
                        $('#WitholdingTaxAccNo').val('');
                        $('#WitholdingTaxAccNoDesc').val('');
                        me.Account4();
                        break;
                    case 'OtherAccNoDesc':
                        $('#OtherAccNo').val('');
                        $('#OtherAccNoDesc').val('');
                        me.Account5();
                        break;
                }
            }
        });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SuppliersClassBrowse",
            title: "Suppliers Class Browse",
            manager: gnManager,
            query: "SupplierClass",
            defaultSort: "SupplierClass asc",
            columns: [
            { field: "SupplierClass", title: "Supplier Code" },
            { field: "SupplierClassName", title: "Supplier Name" },
            { field: "TypeOfGoods", title: "Type Part" },
            { field: "ProfitCenterCode", title: "Profit Center Code" },
            { field: "PayableAccNo", title: "No. Acc. Hutang Dagang " },
            { field: "DownPaymentAccNo", title: "No. Uang Muka" },
            { field: "InterestAccNo", title: "No. Acc. Bunga" },
            { field: "OtherAccNo", title: "No. Acc. Lain-lain" },
            { field: "TaxInAccNo", title: "No. Acc. Pajak Masukan" },
            { field: "WitholdingTaxAccNo", title: "No. Acc. Pajak Penghasilan" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                // me.GetSupplierInfo(data.SupplierCode);
                me.isSave = false;
                me.Apply();
                //lookupName('PFCN', $('#ProfitCenterCode').val(),true);
                //accountName('PayableAccNoDesc', $('#PayableAccNo').val(), true);
                //accountName('DownPaymentAccNoDesc', $('#DownPaymentAccNo').val(), true);
                //accountName('InterestAccNoDesc', $('#InterestAccNo').val(), true);
                //accountName('TaxInAccNoDesc', $('#TaxInAccNo').val(), true);
                //accountName('WitholdingTaxAccNoDesc', $('#WitholdingTaxAccNo').val(), true);
                //accountName('OtherAccNoDesc', $('#OtherAccNo').val(), true);
            }
        });
    }

    me.ProfitCenterCodeDisc = function () {
        var lookup = Wx.blookup({
            name: "ProfitCenterCodeDiscLookup",
            title: "Lookup ProfitCenterCodeDisc",
            manager: gnManager,
            query: "ProfitCenters",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "ProfitCenter Code" },
                { field: "LookUpValueName", title: "ProfitCenter Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.ProfitCenterCode = data.LookUpValue;
                me.data.ProfitCenterCodeDesc = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    me.Account = function () {
        var lookup = Wx.blookup({
            name: "AccountLookup",
            title: "Account Hutang Dagang",
            manager: gnManager,
            query: "Account",
            defaultSort: "AccountNo",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Nama Account" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.PayableAccNo = data.AccountNo;
                me.data.PayableAccNoDesc = data.Description;
                me.Apply();
            }
        });
    };

    me.Account1 = function () {
        var lookup = Wx.blookup({
            name: "AccountLookup",
            title: "Account Uang Muka",
            manager: gnManager,
            query: "Account",
            defaultSort: "AccountNo",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Nama Account" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.DownPaymentAccNo = data.AccountNo;
                me.data.DownPaymentAccNoDesc = data.Description;
                me.Apply();
            }
        });
    };
    
    me.Account2 = function () {
        var lookup = Wx.blookup({
            name: "AccountLookup",
            title: "Account Bunga",
            manager: gnManager,
            query: "Account",
            defaultSort: "AccountNo",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Nama Account" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.InterestAccNo = data.AccountNo;
                me.data.InterestAccNoDesc = data.Description;
                me.Apply();
            }
        });
    };

    me.Account3 = function () {
        var lookup = Wx.blookup({
            name: "AccountLookup",
            title: "Account Pajak Masukan",
            manager: gnManager,
            query: "Account",
            defaultSort: "AccountNo",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Nama Account" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.TaxInAccNo = data.AccountNo;
                me.data.TaxInAccNoDesc = data.Description;
                me.Apply();
            }
        });
    };

    me.Account4 = function () {
        var lookup = Wx.blookup({
            name: "AccountLookup",
            title: "Account Pajak Penghasilan",
            manager: gnManager,
            query: "Account",
            defaultSort: "AccountNo",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Nama Account" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.WitholdingTaxAccNo = data.AccountNo;
                me.data.WitholdingTaxAccNoDesc = data.Description;
                me.Apply();
            }
        });
    };

    me.Account5 = function () {
        var lookup = Wx.blookup({
            name: "AccountLookup",
            title: "Account Lain-lain",
            manager: gnManager,
            query: "Account",
            defaultSort: "AccountNo",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Nama Account" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.OtherAccNo = data.AccountNo;
                me.data.OtherAccNoDesc = data.Description;
                me.Apply();
            }
        });
    };

    $("[name = 'ProfitCenterCode']").on('blur', function () {
        if ($('#ProfitCenterCode').val() || $('#ProfitCenterCode').val() != '') {
            lookupName('PFCN', $('#ProfitCenterCode').val());
        } else {
            $('#ProfitCenterCodeDesc').val('');
        }
    });

    $("[name = 'PayableAccNo']").on('blur', function () {
        if ($('#PayableAccNo').val() || $('#PayableAccNo').val() != '') {
            accountName('PayableAccNoDesc', $('#PayableAccNo').val());
        } else {
            $('#PayableAccNoDesc').val('');
        }
    });

    $("[name = 'DownPaymentAccNo']").on('blur', function () {
        if ($('#DownPaymentAccNo').val() || $('#DownPaymentAccNo').val() != '') {
            accountName('DownPaymentAccNoDesc', $('#DownPaymentAccNo').val());
        } else {
            $('#DownPaymentAccNoDesc').val('');
        }
    });

    $("[name = 'InterestAccNo']").on('blur', function () {
        if ($('#InterestAccNo').val() || $('#InterestAccNo').val() != '') {
            accountName('InterestAccNoDesc', $('#InterestAccNo').val());
        } else {
            $('#InterestAccNoDesc').val('');
        }
    });

    $("[name = 'TaxInAccNo']").on('blur', function () {
        if ($('#TaxInAccNo').val() || $('#TaxInAccNo').val() != '') {
            accountName('TaxInAccNoDesc', $('#TaxInAccNo').val());
        } else {
            $('#TaxInAccNoDesc').val('');
        }
    });

    $("[name = 'WitholdingTaxAccNo']").on('blur', function () {
        if ($('#WitholdingTaxAccNo').val() || $('#WitholdingTaxAccNo').val() != '') {
            accountName('WitholdingTaxAccNoDesc', $('#WitholdingTaxAccNo').val());
        } else {
            $('#WitholdingTaxAccNoDesc').val('');
        }
    });

    $("[name = 'OtherAccNo']").on('blur', function () {
        if ($('#OtherAccNo').val() || $('#OtherAccNo').val() != '') {
            accountName('OtherAccNoDesc', $('#OtherAccNo').val());
        } else {
            $('#OtherAccNoDesc').val('');
        }
    });

    me.initialize = function () {
        me.data.TypeOfGoods = "0";
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/SupplierClass/Delete', me.data).
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
        });
    }

    me.saveData = function (e, param) {

        $http.post('gn.api/SupplierClass/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    //me.data.SupplierCode = data.data.SupplierCode;
                    //me.data.StandardCode = data.data.StandardCode;

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Supplier Class",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataSupplierClass",
                title: "Data Supplier Class",
                items: [
                    
                    { name: "SupplierClass", type: "text", text: "Supplier Code Class", cls: "span6", disable: "IsEditing() || testDisabled", validasi: "required,max(50)" },
                    { name: "SupplierClassName", type: "text", text: "Supplier Name Class", cls: "span6", validasi: "required,max(50)" },
                    { name: "TypeOfGoods", model: "data.TypeOfGoods", opt_text: "-- SELECT ALL --", cls: "span4 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Tipe Part", datasource: "comboTPGO", required: true, validasi: "required" },
                    {
                        type: "controls", text: "Profit Center", required: true, items: [
                             { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Profit Center Code", type: "popup", cls: "span3", required: true, validasi: "required", click: "ProfitCenterCodeDisc()" },
                             { name: "ProfitCenterCodeDesc", model: "data.ProfitCenterCodeDesc", text: "Profit Center Name", cls: "span5", readonly: true, required: false },
                        ]
                    },
                    {
                        type: "controls", text: "No. Acc Hutang Dagang", items: [
                            { name: "PayableAccNo", model: "data.PayableAccNo", type: "popup", text: "", cls: "span3",  click: "Account()" },
                            { name: "PayableAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                        ]
                    },
                   {
                       type: "controls", text: "No. Uang Muka", items: [
                           { name: "DownPaymentAccNo", model: "data.DownPaymentAccNo", type: "popup", text: "", cls: "span3",  click: "Account1()" },
                           { name: "DownPaymentAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                       ]
                   },
                   {
                       type: "controls", text: "No. Acc Bunga", items: [
                           { name: "InterestAccNo", model: "data.InterestAccNo", type: "popup", text: "", cls: "span3",  click: "Account2()" },
                           { name: "InterestAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                       ]
                   },
                    {
                        type: "controls", text: "No. Acc Lain-lain", items: [
                            { name: "OtherAccNo", model: "data.OtherAccNo", type: "popup", text: "", cls: "span3",  click: "Account5()" },
                            { name: "OtherAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                        ]
                    },
                   {
                       type: "controls", text: "No. Acc Pajak Masukan", items: [
                           { name: "TaxInAccNo", model: "data.TaxInAccNo", type: "popup", text: "", cls: "span3",  click: "Account3()" },
                           { name: "TaxInAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                       ]
                   },
                   {
                       type: "controls", text: "No. Acc Pajak Penghasilan", items: [
                           { name: "WitholdingTaxAccNo", model: "data.WitholdingTaxAccNo", type: "popup", text: "", cls: "span3",  click: "Account4()" },
                           { name: "WitholdingTaxAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                       ]
                   }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterSuppliersController");
    }

});