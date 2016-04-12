"use strict";

function spmaintenancePOController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    var SupplierCode = '';
    me.clear = function () {
        me.data.PosNo = null;
        me.data.PartNo = null;
        me.data.PartName = null;
        me.data.SeqNo = null;
        me.data.OnOrder = 0.00;

        me.isSave = false;
    };

    me.posbrowse = function () {
        var lookup = Wx.blookup({
            name: "btnPOSNOView",
            title: "POS Lookup",
            manager: spManager,
            query: "PosLookup",
            defaultSort: "PosNo asc",
            columns: [
                { field: "PosNo", title: "POSNo" },
                { field: "SupplierCode", title: "SupplierCode" },
                { field: "SupplierName", title: "SupplierName" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PosNo = data.PosNo;
                me.data.PartNo = "";
                me.data.PartName = "";
                me.data.SeqNo = 0;
                me.data.OnOrder = 0.00;
                me.data.SupplierCode = data.SupplierCode;

                me.isSave = false;
                me.Apply();
            }
        });
    };

    me.PartBrowse = function () {
        if (me.data.PosNo != null) {
            var lookup = Wx.blookup({
                name: "btnPartView",
                title: "Part Lookup",
                manager: spManager,
                query: new breeze.EntityQuery.from("OrderPartLookup").withParameters({ PosNo: me.data.PosNo }),
                defaultSort: "SeqNo asc",
                columns: [
                    { field: "SeqNo", title: "SeqNo" },
                    { field: "PartNo", title: "PartNo" },
                    { field: "PartName", title: "PartName" },
                    { field: "OnOrder", title: "OnOrder" },
                    { field: "SupplierCode", title: "SupplierCode" }
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.data.PartNo = data.PartNo;
                    me.data.PartName = data.PartName;
                    me.data.SeqNo = data.SeqNo;
                    me.data.OnOrder = data.OnOrder;
                    me.data.SupplierCode = data.SupplierCode;

                    me.isSave = false;
                    me.Apply();
                }
            });
        }
    };

    me.saveData = function (e, param) {
        if (me.data.OnOrder < 0)
        { me.data.OnOrder = 0.00; return; }

        $http.post('sp.api/MaintenanceOrderPo/Save', me.data).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success(data.message);
                me.clear();
                //me.startEditing();
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox("Connection to the server failed...., status " + status, MSG_ERROR);
        });
    };

    me.initialize = function () {
        me.clear();
    };
    me.start();
};

$(document).ready(function () {
    var options = {
        title: "Maintenance On Order PO",
        xtype: "panels",
        //toolbars:WxButtons,
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-info", icon: "icon-new", click: "cancelOrClose()" },
            { name: "btnSave",   text: "Save",   cls:"btn btn-success", icon: "icon-save", click: "save()", disable: "!isSave" }
        ],
        panels: [
            {
                name: "pnlA",
                title: "Order Part",
                items: [
                    { name: "PosNo", model: "data.PosNo", text: "No. POS", cls: "span3", type: "popup", btnName: "btnPOSNo", click: "posbrowse()", validasi: "required", readonly: true },
                    {
                        text: "No. Part",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "PartNo", model: "data.PartNo", cls: "span2", placeHolder: "Part No", type: "popup", btnName: "btnPartNo", click: "PartBrowse()", validasi: "required", readonly: true },
                            { name: "PartName", model: "data.PartName", cls: "span6", placeHolder: "Part Name", readonly: true }
                        ]
                    },
                    { name: "SeqNo", model: "data.SeqNo", text: "No. Urut", cls: "span3", disable: true },
                    { name: "OnOrder", model: "data.OnOrder", text: "Jml. Order", placeHolder: "0.00", cls: "span3 number" }
                ]   
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spmaintenancePOController");
    }
});