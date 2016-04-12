"use strict";

function spMaintenanceSubtitusiController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.clear = function () {
        me.data.POSNo = null;
        me.data.PartNo = null;
        me.data.PartName = null;
        me.data.OrderQty = 0.00;
        me.data.newPartNo = null;
        me.data.newPartName = null;
        me.data.newQty = 0.00;

        me.isSave = false;
    };

    me.posbrowse = function () {
        var lookup = Wx.blookup({
            name: "btnPOSNOView",
            title: "POS Lookup",
            manager: spManager,
            query: "PosLookup",
            defaultSort: "PosNo desc",
            columns: [
                { field: "PosNo", title: "POSNo" },
                { field: "SupplierCode", title: "SupplierCode" },
                { field: "SupplierName", title: "SupplierName" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.POSNo = data.PosNo;
                me.data.PartNo = null;
                me.data.PartName = null;
                me.data.OrderQty = 0.00;
                me.data.newPartNo = null;
                me.data.newPartName = null;
                me.data.newQty = 0.00;

                me.isSave = false;
                me.Apply();
            }
        });
    };

    me.PartBrowse = function () {
        if (me.data.POSNo != null) {
            var lookup = Wx.blookup({
                name: "btnPartView",
                title: "Part Lookup",
                manager: spManager,
                query: new breeze.EntityQuery.from("OrderPartLookup").withParameters({ PosNo: me.data.POSNo }),
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
                    me.data.OrderQty = data.OnOrder;
                    me.data.newPartNo = null;
                    me.data.newPartName = null;
                    me.data.newQty = 0.00;

                    me.isSave = false;
                    me.Apply();
                }
            });
        }
    };

    me.Part2Browse = function () {
        if (me.data.PartNo != null) {
            var lookup = Wx.blookup({
                name: "btnPartView",
                title: "Part Lookup",
                manager: spManager,
                query: new breeze.EntityQuery.from("OrderPartLookupByPart").withParameters({ PartNo: me.data.PartNo }),
                defaultSort: "PartNo asc",
                columns: [
                    { field: "PartNo", title: "PartNo" },
                    { field: "PartName", title: "PartName" }
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.data.newPartNo = data.PartNo;
                    me.data.newPartName = data.PartName;
                    me.data.newQty = 0.00;

                    me.isSave = false;
                    me.Apply();
                }
            });
        }
    };

    me.saveData = function (e, param) {
        me.data.newQty = $('#newQty').val();
        // try to convert decimal for Qty1 and Qty2
        var v1 =  me.data.OrderQty * 1;
        var v2 =  me.data.newQty * 1;
        if (v2 > v1)
        {
            MsgBox("Quantity subtitusi tidak boleh lebih besar dari quantity asal", MSG_ERROR);
            return;
        }
        else
        {
            // jika Qty2 kurang dari sama dengan 0
            if (v2 <= 0)
            {
                MsgBox("Order Qty tidak boleh kurang atau sama dengan nol", MSG_ERROR);
                return;
            }
        }
        var dat = {
            'posNo': me.data.POSNo,
            'partNo': me.data.PartNo
        };

        $http.post('sp.api/MaintenanceOrderSubtitusi/ValidateInput',dat).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.confirm(data.message,me.postData);
            } else {
                if (!data.success) 
                    MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox("Connection to the server failed..., status " + status, MSG_ERROR);
        });
    };
 
    me.postData = function (btn) {
        if (btn == 'Yes') {
           var allData = {
                'record': me.data,
                'newPartNo': me.data.newPartNo,
                'newQty': me.data.newQty * 1
            };
            
            $http.post('sp.api/MaintenanceOrderSubtitusi/Save', allData).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success(data.message);
                    me.clear();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox("Connection to the server failed..., status " + status, MSG_ERROR);
            });
        }
    };

    me.initialize = function() {
        me.clear();
    };

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Maintenance On Order Substitusi",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-info", icon: "icon-new", click: "cancelOrClose()" },
            { name: "btnSave",   text: "Save",   cls:"btn btn-success", icon: "icon-save", click: "save()", disable: "!isSave" }
        ],
        panels: [
            {
                name: "pnlA",
                title: "Order Part",
                items: [
                    { name: "POSNo", text: "POS No.", cls: "span3 ", type: "popup", btnName: "btnPOSNo", click: "posbrowse()", readonly: true, validasi: "required" },
                    {
                        text: "Part No",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "PartNo", cls: "span2", placeHolder: "Part No", type: "popup", btnName: "btnPartNo", click: "PartBrowse()", readonly: true, validasi: "required" },
                            { name: "PartName", cls: "span6", placeHolder: "Part Name", readonly: true }
                        ]
                    },
                    { name: "OrderQty", text: "OrderQty", placeHolder: "0.00", cls: "span3 number", readonly: true  }
                ]   
            },
            {
                name: "pnlB",
                title: "Subtitusi Part",
                items: [
                    {
                        text: "Part No",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "newPartNo", cls: "span2", placeHolder: "Part No", type: "popup", btnName: "btnNoPart", click: "Part2Browse()", readonly: true, validasi: "required" },
                            { name: "newPartName", cls: "span6", placeHolder: "Part Name", readonly: true }
                        ]
                    },
                    { name: "newQty", model: "data.newQty", text: "OrderQty", placeHolder: "0.00", cls: "span3 number" }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spMaintenanceSubtitusiController");
    }
});