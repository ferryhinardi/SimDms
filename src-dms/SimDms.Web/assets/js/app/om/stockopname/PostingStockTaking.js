var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


"use strict";

function PostingStokTaking($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.STHdrNoBrowse = function (param) {
        var lookup = Wx.blookup({
            name: "btnSTHdrNo",
            title: "Stock Taking Lookup",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("STHdrNoBrowse").withParameters({ "pil": "posting" }),
            defaultSort: "STHdrNo asc",
            columns: [
                { field: "STHdrNo", title: "No. Stok Taking" },
                { field: "STDate", title: "Tanggal", type: "date", format: "{0:dd-MMM-yyyy}" },
               // { field: "Status", title: "Status" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                if (param == 'from') {
                    me.data.STHdrNo = data.STHdrNo;
                } else {
                    me.data.STHdrNoTo = data.STHdrNo;
                }
                me.Apply();
            }
        });
    }

    me.ProsesStock = function () {
        me.savemodel = angular.copy(me.data);
        MsgConfirm("Anda yakin?", function (e) {
            if (e) {
                $http.post('om.api/SalesStockOpname/PostingStokTaking', me.savemodel).
                success(function (result) {
                    if (result.success) {
                        MsgBox(result.message, MSG_SUCCESS);
                        me.initialize();
                    } else {
                        MsgBox(result.message, MSG_ERROR);
                        console.log(result.error_log);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox("Connecting server error", MSG_ERROR);
                });
            }
        });
    }

    me.initialize = function () {
        me.data = {};
    }
    
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Posting Stok Taking",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                    {
                        name: "STHdrNo",
                        click: "STHdrNoBrowse('from')",
                        cls: "span5",
                        type: "popup",
                        text: "No. Stock Taking From",
                        btnName: "btnSTHdrNo",
                        readonly: true,
                    },
                    {
                        name: "STHdrNoTo",
                        click: "STHdrNoBrowse('to')",
                        cls: "span5",
                        type: "popup",
                        text: "No. Stock Taking To",
                        btnName: "btnSTHdrNoTo",
                        readonly: true,
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnProses", text: "Process", cls: "btn btn-info", click: "ProsesStock()", disable: "data.STHdrNo == undefined || data.STHdrNoTo == undefined " },
                        ]
                    },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);
    
    function init() {
        SimDms.Angular("PostingStokTaking");
    }
});