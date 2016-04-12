var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    me.Fiscalstate = [
        { "value": "0", "text": "Non Active" },
        { "value": "1", "text": "Active" }
    ];


    me.customer = function () {
        var lookup = Wx.blookup({
            name: "CustomersBrowse",
            title: "Customers Browse",
            manager: gnManager,
            query: new breeze.EntityQuery().from("profitcenterCustomer").withParameters({ profitcentercode: me.data.ProfitCenterCode }),
            defaultSort: "CustomerCode asc",
            columns: [
            { field: "CustomerCode", title: "Customer Code" },
            { field: "CustomerName", title: "Customer Name" },
             { field: "Address", title: "Address" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.ProfitCenterCode = function () {
        var lookup = Wx.blookup({
            name: "ProfitCenterCodeLookup",
            title: "Lookup ProfitCenterCode",
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
                me.data.ProfitCenterName = data.LookUpValueName;
                $('#btnCheck').prop('disabled', false);
                me.Apply();
            }
        });
    };
    me.initialize = function () {
        me.clearTable(me.grid1);
        me.data.isAllCustomer = true;
        $('#isAllCustomer').prop('checked', true);
        $('#btnCheck').prop('disabled', true);
    }

    $("[name = 'isAllCustomer']").on('change', function () {
        me.data.isAllCustomer = $('#isAllCustomer').prop('checked');
        me.Apply();
        //alert($('#isAllCustomer').prop('checked'));
    });

    $("[name = 'ProfitCenterCode']").on('blur', function () {
        if ($('#ProfitCenterCode').val() || $('#ProfitCenterCode').val() != '') {
            $http.post('gn.api/Lookup/getLookupName?CodeId=PFCN&LookupValue=' + $('#ProfitCenterCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.ProfitCenterName = v.TitleName;
                } else {
                    $('#ProfitCenterCode').val('');
                    $('#ProfitCenterName').val('');
                    me.customer();
                }
            });
        } else {
            me.data.ProfitCenterName = '';
            me.customer();
        }
    });

    $("[name = 'CustomerCode']").on('blur', function () {
        if (!me.data.ProfitCenterCode || me.data.ProfitCenterCode == '') {
            SimDms.Warning('You Must fiil Profit Center!');
            $('#CustomerCode').val('');
            $('#isAllCustomer').prop('checked', true);
            me.data.isAllCustomer = true;
            me.Apply();

        }else{
        if ($('#CustomerCode').val() || $('#CustomerCode').val() != '') {
            $http.post('gn.api/Lookup/CustomerName?CustomerCode=' + $('#CustomerCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.CustomerName = v.TitleName;
                } else {
                    $('#CustomerCode').val('');
                    $('#CustomerName').val('');
                    me.customer();
                }
            });
        } else {
            me.data.CustomerName = '';
            me.customer();
}
}
    });

    me.check = function (data) {
        if (!me.data.CustomerCode) { me.data.CustomerCode = "";}
        $http.get('breeze/gnMaster/CreditLimitViews?profitcentercode=' + me.data.ProfitCenterCode + '&customercode='+ me.data.CustomerCode).
           success(function (data, status, headers, config) {
               me.grid.data = data; 
               me.loadTableData(me.grid1, me.grid.data); //loadnya
           }).
           error(function (e, status, headers, config) {
               //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               console.log(e);
           });
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxsalestarget",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "Nomor", header: "No.", width: 50 },
                { id: "ProfitCenterCode", header: "Profit Center", width: 125 },
                { id: "CustomerCode", header: "Customer Code", width: 125 },
                { id: "CustomerName", header: "Customer Name", width: 200 },
                { id: "PaymentBy", header: "How to Pay", width: 125 },
                { id: "CreditLimit", header: "Limit Receivable", width: 200, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "SalesAmt", header: "Used Credit ", width: 200, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "ReceivedAmt", header: "Paid", width: 200, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "Credit", header: "Rest Debt", width: 200, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
                { id: "BalanceAmt", header: "Rest Limit", width: 200, format: webix.i18n.numberFormat, css: { "text-align": "right" } }
            ],

            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        me.Apply();
                    }
                }
            }
        });
    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

}
$(document).ready(function () {
    var options = {
        title: "Inquiry Credit Limit",
        xtype: "panels",
        toolbars: [{ name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" }],//WxButtons,
        panels: [
            {
                name: "pnlInfo",
                title: "",
                items: [
                         {
                             type: "controls", text: "Profit Center", required: true, items: [
                                 { name: "ProfitCenterCode", model: "data.ProfitCenterCode", type: "popup", text: "Profit Center", cls: "span3", click: "ProfitCenterCode()", validasi: "required" },
                                 { name: "ProfitCenterName", model: "data.ProfitCenterName", text: "Profit Center Name", cls: "span5", readonly: true },
                             ],
                         },
                         { name: 'isAllCustomer', type: 'check', text: 'AllCustomer', cls: 'span2', float: 'left' },
                         {
                             name: "CustomerCategory", type: "controls", text: "Customer Code", items: [
                                 { name: "CustomerCode", type: "popup", text: "Customer Code", cls: "span2", validasi: "max(15)", disable: "data.isAllCustomer == true", click: "customer()" },
                                 { name: "CustomerName", type: "text", text: "Customer Name", cls: "span6", disable:true },
                             ]
                         },
                          {
                              type: "buttons",
                              items: [
                                      { name: "btnCheck", text: "Check", icon: "icon-plus", cls: "btn btn-info", click: "check()" },
                                     ]
                          },
                ]
            },
            {
                name: "wxsalestarget",
                xtype: "wxtable",
            },

        ]
    };
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spItemPriceController");
    }
});