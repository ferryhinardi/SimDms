var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "Supplier Utility Browse",
            manager: gnManager,
            query: "SupplierUtility",
            defaultSort: "CompanyCode asc",
            columns: [
            { field: "CompanyCode", title: "Company Code" },
            { field: "BranchCode", title: "Branch Code" },
             { field: "GenarateNo", title: "Generate Number" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data = data;
                me.isSave = false;
                me.Apply();
            }
        });
    }


    me.initialize = function () {
        $http.post('gn.api/AutonoSupplier/LoadInfo').
        success(function (dl, status, headers, config) {
            console.log(dl);
            me.data.CompanyCode = dl.CompanyCode;
            me.data.BranchCode = dl.BranchCode;
            me.data.Sequence = dl.Sequence;
            me.data.isAutoGenerate = dl.IsAutoGenerate;
            //localStorage.setItem('Sequence', me.data.Sequence);
            if (me.data.isAutoGenerate == true) {
                $('#isAutoGenerate').prop('checked', true);
            }
        });
    }

    $("[name = 'isAutoGenerate']").on('change', function () {
        me.data.isAutoGenerate = $('#isAutoGenerate').prop('checked');
        if (me.data.isAutoGenerate == false) {
            me.data.Sequence = me.data.Sequence;//'';
        } else {
            me.data.Sequence = me.data.Sequence;//localStorage.getItem('Sequence');
        }
        me.Apply();
        //alert($('#isAllCustomer').prop('checked'));
    });

    me.save = function (e, param) {
        $http.post('gn.api/AutonoSupplier/Save', me.data).
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
        title: "Auto No. Supplier",
        xtype: "panels",
        toolbars: [
            { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },

        ],//WxButtons,
        panels: [
            {
                name: "pnlInfo",
                title: "",
                items: [
                          { name: "CompanyCode", type: "text", text: "Company Code", cls: "span4 full", readonly: true },
                          { name: "BranchCode", type: "text", text: "Branch Code", cls: "span4 full", readonly: true },
                          { name: "Sequence", type: "text", text: "Last Number", cls: "span4", disable: "data.isAutoGenerate == false", },
                           { name: 'isAutoGenerate', type: 'check', text: 'Auto', cls: 'span2', float: 'left' },
                          {
                              type: "buttons",
                              items: [
                                      { name: "btnSave", text: "Save", icon: "icon-save", cls: "btn btn-info", click: "save()" },
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
        SimDms.Angular("spItemPriceController");
    }
});