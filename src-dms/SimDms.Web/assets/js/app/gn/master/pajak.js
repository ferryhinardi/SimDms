var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterTaxController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MasterTaxBrowse",
            title: "Tax Browse",
            manager: gnManager,
            query: "Taxes",
            defaultSort: "TaxCode asc",
            columns: [
            { field: "TaxCode", title: "Tax Code", width: 150 },
            { field: "TaxPct", title: "Tax Percentage", width: 150, template: '<div style="text-align:right;">#= kendo.toString(TaxPct, "n2") #</div>' },
            { field: "Description", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                //me.GetCustomerInfo(data.CustomerCode);
                me.isSave = false;
                me.Apply();

            }
        });
    }


    me.initialize = function () {
        me.hasChanged = false;
        me.data.TaxPct = 0;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('gn.api/Tax/Delete', me.data).
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

        $http.post('gn.api/Tax/Save', me.data).
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
        title: "Pajak",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataCustomerClass",
                title: "Pajak",
                items: [
                    { name: "TaxCode", type: "text", text: "Kode Pajak", cls: "span4 full", disable: "IsEditing() || testDisabled", validasi: "required" },
                    { name: "TaxPct", model: "data.TaxPct", text: "% Pajak", placeHolder: "0.00", cls: "span4 full", validasi: "required", type: "decimal", min: 0, max: 100 },
                    { name: "Description", type: "textarea", text: "Keterangan", maxlength: 100 },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};    
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterTaxController");
    }




});