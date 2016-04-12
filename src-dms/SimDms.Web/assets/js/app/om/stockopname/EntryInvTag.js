var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


"use strict";

function EntryInvTag($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.STHdrNoBrowse = function () {
        var lookup = Wx.blookup({
            name: "btnSTHdrNo",
            title: "Stock Taking Lookup",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from("STHdrNoBrowse").withParameters({ "pil": "entry" }),
            defaultSort: "STHdrNo asc",
            columns: [
                { field: "STHdrNo", title: "No. Stok Taking" },
                { field: "STDate", title: "Tanggal", type: "date", format: "{0:dd-MMM-yyyy}" },
                { field: "Status", title: "Status" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data = data;
                me.isProcessing = true;
                me.Apply();
            }
        });
    }

    me.PreviewStock = function () {
        var params = {
            STHdrNo: me.data.STHdrNo
        };

        $http.post("om.api/SalesStockOpname/Select4View", params)
        .success(function (result) {
            me.detail = result.data;
            if (result.success) {
                if (result.data.length > 0 ) {
                    me.allowProcess = true;
                    me.isProcessing = false;
                }
                else{
                    me.allowProcess = false;
                    me.isProcessing = true;
                }
                me.loadTableData(me.grid1, result.data);
            }
            else {
                me.allowProcess = false;
                MsgBox(result.message, MSG_ERROR);
            }
        })
        .error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.ProsesStock = function () {
        if (me.detail.length < 1)
            return;
            
        MsgConfirm("Anda yakin?", function (e) {
            if (e) {
                $("#btnPreview").attr("disabled", "disabled");
                me.isProcessing = true;
                me.Apply();

                var datDetail = [];

                $.each(me.detail, function (key, val) {
                    var arr = {
                        "STNo": val["STNo"],
                        "Status": val["Status"]
                    }
                    datDetail.push(arr);
                });

                var dat = {};
                dat["STHdrNo"] = me.data.STHdrNo;
                dat["model"] = datDetail;
                var JSONData = JSON.stringify(dat);

                $http.post('om.api/SalesStockOpname/ProsesEntryInventoryTag', JSONData).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        MsgBox("Proses Stock Taking berhasil");
                        $("#btnPreview").removeAttr("disabled");
                        me.isProcessing = true;
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                        console.log(dl.error_log);
                        $("#btnPreview").removeAttr("disabled");
                        me.isProcessing = false;
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox("Connecting server error", MSG_ERROR);
                    $("#btnPreview").removeAttr("disabled");
                    me.isProcessing = false;
                });
            }
        });
    };

    me.grid1 = new webix.ui({
        container: "gridStockPreview",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 360,
        checkboxRefresh: true,
        columns: [
            { id: "Status", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 40 },
            { id: "STNo", header: "Tag", width: 50 },
            { id: "WareHouseCode", header: "Gudang", width: 70 },
            { id: "SalesModelCode", header: "Model", width: 220 },
            { id: "SalesModelYear", header: "Tahun", width: 60 },
            { id: "ColourCode", header: "Warna", width: 80 },
            { id: "ChassisCode", header: "Kode Rangka", width: 150 },
            { id: "ChasssisNo", header: "No Rangka", width: 120 },
            { id: "EngineCode", header: "Kode Mesin", width: 120 },
            { id: "EngineNo", header: "No Mesin", width: 120 }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    //me.detail = this.getItem(me.grid1.getSelectedId().id);
                    //me.detail.oid = me.grid1.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.initialize = function () {
        me.data = {};
        me.detail = {};
        me.allowProcess = false;
        me.isProcessing = false;
        me.clearTable(me.grid1);
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Entry Inventory Tag",
        xtype: "panels",
        panels: [{
            name: "pnloptions",
            cls: "full",
            items: [
                {
                    name: "STHdrNo",
                    click: "STHdrNoBrowse()",
                    cls: "span4",
                    type: "popup",
                    text: "No. Stock Taking",
                    btnName: "btnSTHdrNo",
                    readonly: true,
                },
                {
                    type: "buttons",
                    items: [
                        { name: "btnPreview", text: "Preview", cls: "btn btn-info", click: "PreviewStock()", disable: "data.STHdrNo == undefined" },
                        { name: "btnProses", text: "Process", cls: "btn btn-info", click: "ProsesStock()", show: "allowProcess", disable: "isProcessing" },
                    ]
                },
                {
                    name: "gridStockPreview",
                    type: "wxdiv",
                }
            ]
        }]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("EntryInvTag");
    }
});