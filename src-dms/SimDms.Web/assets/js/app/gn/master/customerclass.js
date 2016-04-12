var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterCustomersController($scope, $http, $injector) {

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
                //alert(me.data.ProfitCenterCodeDesc);
            } else {
                $('#ProfitCenterCode').val('');
                $('#ProfitCenterCodeDesc').val('');
                me.ProfitCenterCodeDesc();
            }
        });
    }

    function accountName(name, AccountNo, browse) {
        $http.post('gn.api/Lookup/getAccountName?AccountNo=' + AccountNo).
        success(function (v, status, headers, config) {
            if (v.TitleName != '') {
                switch (name) {
                    case 'ReceivableAccNoDesc':
                        //me.data.ReceivableAccNoDesc = v.TitleName;
                        $('#ReceivableAccNoDesc').val(v.TitleName);
                        break;
                    case 'DownPaymentAccNoDesc':
                        //me.data.DownPaymentAccNoDesc = v.TitleName;
                        $('#DownPaymentAccNoDesc').val(v.TitleName);
                        break;
                    case 'InterestAccNoDesc':
                        //me.data.InterestAccNoDesc = v.TitleName;
                        $('#InterestAccNoDesc').val(v.TitleName);
                        break;
                    case 'TaxOutAccNoDesc':
                        //me.data.TaxInAccNoDesc = v.TitleName;
                        $('#TaxOutAccNoDesc').val(v.TitleName);
                        break;
                    case 'LuxuryTaxAccNoDesc':
                        //me.data.WitholdingTaxAccNoDesc = v.TitleName;
                        $('#LuxuryTaxAccNoDesc').val(v.TitleName);
                        break;
                }
            } else if (!browse) {
                switch (name) {
                    case 'ReceivableAccNoDesc':
                        $('#ReceivableAccNo').val('');
                        $('#ReceivableAccNoDesc').val('');
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
                    case 'TaxOutAccNoDesc':
                        $('#TaxOutAccNo').val('');
                        $('#TaxOutAccNoDesc').val('');
                        me.Account3();
                        break;
                    case 'LuxuryTaxAccNoDesc':
                        $('#LuxuryTaxAccNo').val('');
                        $('#LuxuryTaxAccNoDesc').val('');
                        me.Account4();
                        break;
                }
            }
        });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "CustomersClassBrowse",
            title: "Customers Class Browse",
            manager: gnManager,
            query: "CustomerClass",
            defaultSort: "CustomerClass asc",
            columns: [
            { field: "CustomerClass", title: "Customer Code" },
            { field: "CustomerClassName", title: "Customer Name" },
            { field: "TypeOfGoods", title: "Type Part" },
            { field: "ProfitCenterCode", title: "Profit Center Code" },
            { field: "ReceivableAccNo", title: "No. Acc Piutang" },
            { field: "DownPaymentAccNo", title: "No. Acc Uang Muka" },
            { field: "InterestAccNo", title: "No. Acc Bunga" },
            { field: "TaxOutAccNo", title: "No. Acc Pajak Keluaran" },
            { field: "LuxuryTaxAccNo", title: "No. Acc Pajak Barang Mewah" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                //me.GetCustomerInfo(data.CustomerCode);
                me.isSave = false;
                me.Apply();
                //lookupName('PFCN', $('#ProfitCenterCode').val(), true);
                //accountName('ReceivableAccNoDesc', $('#ReceivableAccNo').val(), true);
                //accountName('DownPaymentAccNoDesc', $('#DownPaymentAccNo').val(), true);
                //accountName('InterestAccNoDesc', $('#InterestAccNo').val(), true);
                //accountName('TaxOutAccNoDesc', $('#TaxOutAccNo').val(), true);
                //accountName('LuxuryTaxAccNo', $('#LuxuryTaxAccNo').val(), true);
            }
        });
    }


    me.ProfitCenterCodeDesc = function () {
        var lookup = Wx.blookup({
            name: "ProfitCenterCodeDescLookup",
            title: "Lookup ProfitCenterCodeDesc",
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
            title: "Account Piutang Dagang",
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
                me.data.ReceivableAccNo = data.AccountNo;
                me.data.ReceivableAccNoDesc = data.Description;
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
            title: "Account Pajak Keluaran",
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
                me.data.TaxOutAccNo = data.AccountNo;
                me.data.TaxOutAccNoDesc = data.Description;
                me.Apply();
            }
        });
    };

    me.Account4 = function () {
        var lookup = Wx.blookup({
            name: "AccountLookup",
            title: "Account Pajak Barang Mewah",
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
                me.data.LuxuryTaxAccNo = data.AccountNo;
                me.data.LuxuryTaxAccNoDesc = data.Description;
                me.Apply();
            }
        });
    };

    $("[name = 'ProfitCenterCode']").on('blur', function () {

        if ($('#ProfitCenterCode').val() || $('#ProfitCenterCode').val() != '') {
            lookupName('PFCN', $('#ProfitCenterCode').val());
        } else {
            $('#ProfitCenterCodeDesc').val('');
            me.Apply();
        }
    });

    $("[name = 'ReceivableAccNo']").on('blur', function () {
        if ($('#ReceivableAccNo').val() || $('#ReceivableAccNo').val() != '') {
            accountName('ReceivableAccNoDesc', $('#ReceivableAccNo').val());
            me.Apply();
        } else {
            $('#ReceivableAccNoDesc').val('');
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

    $("[name = 'TaxOutAccNo']").on('blur', function () {
        if ($('#TaxOutAccNo').val() || $('#TaxOutAccNo').val() != '') {
            accountName('TaxOutAccNoDesc', $('#TaxOutAccNo').val());
        } else {
            $('#TaxOutAccNoDesc').val('');
        }
    });

    $("[name = 'LuxuryTaxAccNo']").on('blur', function () {
        if ($('#LuxuryTaxAccNo').val() || $('#LuxuryTaxAccNo').val() != '') {
            accountName('LuxuryTaxAccNoDesc', $('#LuxuryTaxAccNo').val());
        } else {
            $('#LuxuryTaxAccNoDesc').val('');
        }
    });

    me.initialize = function () {
        me.data.TypeOfGoods = "0";
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/CustomerClass/Delete', me.data).
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

        $http.post('gn.api/CustomerClass/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    me.startEditing();
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
        title: "Customer Class",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataCustomerClass",
                title: "Customer Class Setting",
                items: [
                    
                    { name: "CustomerClass", type: "text", text: "Customer Class Code", cls: "span6", disable: "IsEditing() || testDisabled", validasi: "required,max(50)" },
                    { name: "CustomerClassName", type: "text", text: "Customer Class Name ", cls: "span6", validasi: "required,max(50)" },
                    { name: "TypeOfGoods", model: "data.TypeOfGoods", opt_text: "-- SELECT ALL --", cls: "span4 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Part Type", datasource: "comboTPGO", required: true, validasi: "required" },
                    {
                        type: "controls", text: "Profit Center", required: true, items: [
                             { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Profit Center Code", type: "popup", cls: "span3", required: true, validasi: "required" , click: "ProfitCenterCodeDesc()" },
                             { name: "ProfitCenterCodeDesc", model: "data.ProfitCenterCodeDesc", text: "Profit Center Name", cls: "span5", readonly: true, required: false },
                        ]
                    },
                    {
                        type: "controls", text: "Receivable Acc No.", items: [
                            { name: "ReceivableAccNo", model: "data.ReceivableAccNo", type: "popup", text: "", cls: "span3", click: "Account()" },
                            { name: "ReceivableAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                        ]
                    },
                   {
                       type: "controls", text: "Down Payment Acc No.", items: [
                           { name: "DownPaymentAccNo", model: "data.DownPaymentAccNo", type: "popup", text: "", cls: "span3", click: "Account1()" },
                           { name: "DownPaymentAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                       ]
                   },
                   {
                       type: "controls", text: "Interest Acc No.", items: [
                           { name: "InterestAccNo", model: "data.InterestAccNo", type: "popup", text: "", cls: "span3", click: "Account2()" },
                           { name: "InterestAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                       ]
                   },
                   {
                       type: "controls", text: "Tax Out Acc No.", items: [
                           { name: "TaxOutAccNo", model: "data.TaxOutAccNo", type: "popup", text: "", cls: "span3", click: "Account3()" },
                           { name: "TaxOutAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
                       ]
                   },
                   {
                       type: "controls", text: "Luxury Tax Acc No.", items: [
                           { name: "LuxuryTaxAccNo", model: "data.LuxuryTaxAccNo", type: "popup", text: "", cls: "span3", click: "Account4()" },
                           { name: "LuxuryTaxAccNoDesc", type: "text", text: "", cls: "span5", readonly: true },
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
        SimDms.Angular("gnMasterCustomersController");
    }




});