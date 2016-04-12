var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spInventoryAdjustmentController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=RSAD').
    success(function (data, status, headers, config) {
        me.comboReasonCode = data;
    });

    $http.post('sp.api/Combo/LoadComboData?CodeId=ADJS').
    success(function (data, status, headers, config) {
        me.comboAdjustmentCode = data;
    });

    me.browseForm = function () {
        Wx.showForm({ url: "gn/master/customer2" });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "InventoryAdjustBrowse",
            title: "Inventory Adjustment Browse",
            manager: spPersediaanManager,
            query: "lnk5001Browse",
            defaultSort: "AdjustmentNo asc",
            columns: [
                { field: "AdjustmentNo", title: "No. Adjustment" },
                { field: "AdjustmentDate", title: "Tgl. Adjustment", template: "#= (AdjustmentDate == undefined) ? '' : moment(AdjustmentDate).format('DD MMM YYYY') #" },
                { field: "ReferenceNo", title: "No. Reference" },
                { field: "ReferenceDate", title: "Tgl. Reference", template: "#= (ReferenceDate == undefined) ? '' : moment(ReferenceDate).format('DD MMM YYYY') #" }
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                me.Apply();
                me.loadDetail(result);

                console.log(me.detail.AdjustmentCode);
            }

        });
    };

    me.printPreview = function () {
        $http.post('sp.api/persediaan/printlnk5001', { adjustmentNo: me.data.AdjustmentNo })
       .success(function (e) {
           if (e.success) {

               var par = me.data.AdjustmentNo + "," + me.data.AdjustmentNo + "," + "typeofgoods";
               var rparam = "Print SpRpTrn006";
               Wx.showPdfReport({
                   id: "SpRpTrn006",
                   pparam: par,
                   rparam: rparam,
                   textprint:true,
                   type: "devex"
               });

               $('#IAStatus').html(e.lblstatus);
               $('#btnPosting').removeAttr('disabled');
           }
           else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) { });
    }

    me.loadDetail = function (data) {
        $http.post('sp.api/persediaan/getdatatablelnk5001', me.data).
            success(function (data, status, headers, config) {
                me.grid.detail = data.table;
                me.loadTableData(me.grid1, me.grid.detail);

                $('#IAStatus').html(data.lblstatus);
                if (data.table.length > 0) {
                    me.isPrintAvailable = true;
                } else {
                    me.isPrintAvailable = false;
                }
                if (data.lblstatus == 'Printed') {
                    //$('#btnPosting').removeAttr('disabled');
                    me.control.DisablePosting = false;
                }
                else {
                    me.control.DisablePosting = true;
                    //$('#btnPosting').attr('disabled', 'disabled');
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.PartNo = function () {
        var lookup = Wx.blookup({
            name: "PartNoLookup",
            title: "Lookup Part",
            manager: spPersediaanManager,
            query: "TransPartNo",
            defaultSort: "PartNo asc",
            columns: [
                { field: "PartNo", title: "No. Part", width: 150 },
                {
                    field: "AvailQty", title: "Avail Qty.", width: 100,
                    template: '<div style="text-align:right;">#= kendo.toString(AvailQty, "n2") #</div>'
                },
                {
                    field: "RetailPriceInclTax", title: "Harga Jual + Pajak", width: 150,
                    template: '<div style="text-align:right;">#= kendo.toString(RetailPriceInclTax, "n0") #</div>'
                },
                {
                    field: "RetailPrice", title: "Harga Jual", width: 105,
                    template: '<div style="text-align:right;">#= kendo.toString(RetailPrice, "n0") #</div>'
                },
                { field: "MovingCode", title: "MC", width: 80, template: '<div style="text-align:right;">#= kendo.toString(MovingCode, "n0") #</div>' },
                { field: "ABCClass", title: "ABC" },
                { field: "PartName", title: "Nama Part", width: 250 }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.PartNo = data.PartNo;
                me.detail.PartName = data.PartName;
                me.isSave = false;
                me.Apply();
            }
        });
    };

    me.WarehouseCode = function () {
        id = '0';
        var lookup = Wx.blookup({
            name: "WarehouseCodeLookup",
            title: "Lookup Warehouse",
            manager: spPersediaanManager,
            query: new breeze.EntityQuery.from("WarehouseCode").withParameters({ Id: id, param: me.detail.PartNo }),
            defaultSort: "warehousecode asc",
            columns: [
                { field: "warehousecode", title: "Kode Gudang" },
                { field: "lookupvaluename", title: "Nama Gudang" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.WarehouseCode = data.warehousecode;
                me.detail.WarehouseName = data.lookupvaluename;
                me.isSave = false;
                me.Apply();
            }
        });
    };

    me.saveData = function (e, param) {
        $http.post('sp.api/Persediaan/Savelnk5001', me.data).
            success(function (v, status, headers, config) {
                if (v.success) {
                    Wx.Success(v.message);
                    me.startEditing();
                    me.loadDetail(me.data);
                    me.data.AdjustmentNo = v.data.AdjustmentNo;
                    //me.Apply();
                } else {
                    MsgBox(v.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.savePosting = function (e, param) {
        MsgConfirm("Apakah anda yakin?", function (e) {
            if (e) {
                $http.post('sp.api/Persediaan/Postinglnk5001', me.data).
                success(function (v, status, headers, config) {
                    if (v.success) {
                        Wx.Success("Posting saved...");
                        $("#IAStatus").html(v.lblstatus);
                        $('#btnPartNo').attr('disabled', 'disabled');
                        me.isPrintAvailable = false;
                        //$('#btnPosting').attr('disabled', 'disabled');
                        me.control.DisablePosting = true;
                        me.startEditing();
                    } else {
                        MsgBox(v.message, MSG_ERROR);
                    }
                }).
                    error(function (data, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
            }
        });
    };

    me.UpdateGridDetail = function (data) {
        me.grid.detail = data;
        if (data.length > 0) {
            me.isPrintAvailable = true;
        } else {
            me.isPrintAvailable = false;
        }
        me.loadTableData(me.grid1, me.grid.detail);
    };

    me.AddEditModel = function () {
        console.log(me.detail.AdjustmentCode, me.detail.ReasonCode);
        if (me.detail.PartNo === undefined || me.detail.PartNo == null) {
            MsgBox("PartNo is required!!!", MSG_ERROR);
            return;
        }        

        if (me.detail.AdjustmentCode === "" || me.detail.AdjustmentCode === undefined || me.detail.AdjustmentCode == null) {
            MsgBox("Kode Adjustment harus pilih salah satu!!!", MSG_ERROR);
            return;
        }

        if (me.detail.ReasonCode === "" || me.detail.ReasonCode === undefined || me.detail.ReasonCode == null) {
            MsgBox("Alasan harus pilih salah satu!!!", MSG_ERROR);
            return;
        }

        if (me.detail.QtyAdjustment === undefined || me.detail.QtyAdjustment == null) {
            MsgBox("Nilai Jumlah tidak boleh 0", MSG_ERROR);
            return;
        }        

        me.LinkDetail();

        me.detail.AdjustmentNo = me.data.AdjustmentNo;

        $http.post('sp.api/Persediaan/SaveDetailslnk5001', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    $('#IAStatus').html(data.status);
                    me.UpdateGridDetail(data.data);
                    if (data.status == "Open") {
                        me.control.DisablePosting = true;
                    }
                    me.startEditing();
                    me.detail = {};
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.UpdateModel = function () {

        if (me.detail.PartNo === undefined || me.detail.PartNo == null) {
            MsgBox("PartNo is required!!!", MSG_ERROR);
            return;
        }

        me.LinkDetail();

        me.detail.AdjustmentNo = me.data.AdjustmentNo;
        $http.post('sp.api/Persediaan/UpdateDetailslnk5001', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");

                    me.UpdateGridDetail(data.data);

                    me.startEditing();
                    me.detail = {};

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.LinkDetail = function () {
        me.detail.AdjustmentNo = me.data.AdjustmentNo;
    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/Persediaan/Deletelnk5001', me.data).
                    success(function (dl, status, headers, config) {
                        if (dl.success) {
                            Wx.Info("Record has been deleted...");
                            $('#IAStatus').html(dl.lblstatus);
                            //me.init();
                        } else {
                            MsgBox(dl.message, MSG_ERROR);
                        }
                    }).
                    error(function (e, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
            }
        });
    };

    me.delete2 = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                me.LinkDetail();
                $http.post('sp.api/Persediaan/DeleteDetailslnk5001', me.detail).
                    success(function (dl, status, headers, config) {
                        if (dl.success) {
                            me.detail = {};
                            Wx.Info("Record has been deleted...");

                            if (dl.count > 0) {
                                me.UpdateGridDetail(dl.data);
                                me.isPrintAvailable = true;
                            }
                            else {
                                me.clearTable(me.grid1);
                                me.isPrintAvailable = false;
                            }
                        } else {
                            MsgBox(dl.message, MSG_ERROR);
                        }
                    }).
                    error(function (e, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
            }
        });
    };

    me.Closedetail = function () {
        me.detail = {};
        me.grid1.clearSelection();
    }

    me.initialize = function () {
        me.data.AdjustmentDate = me.now();
        me.data.ReferenceDate = me.now();
        me.clearTable(me.grid1);
        me.detail = {};
        me.isPrintAvailable = true;
        me.control = {};
        me.control.DisablePosting = true;

        $('#IAStatus').html("");
        $('#IAStatus').css(
        {
            "font-size": "28px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "right"
        });
        //$('#btnPosting').attr('disabled', 'disabled');
    };

    me.grid1 = new webix.ui({
        container: "wxInventoryAdjust",
        view: "wxtable", css:"alternating",
        columns: [
            //{ id: "No", header: "No", width:50 },
            { id: "PartNo", header: "No. Part", fillspace: true },
            { id: "PartName", header: "Nama Part", width: 300 },
            { id: "WarehouseCode", header: "Kode Gudang", fillspace: true },
            { id: "WarehouseName", header: "Nama Gudang", fillspace: true },
            //{ id: "AdjustmentCode", header: "Adj. Code", fillspace: true },
            { id: "AdjustmentDesc", header: "Kode Adjustment", fillspace: true },
            { id: "ReasonDesc", header: "Alasan", fillspace: true },
            { id: "QtyAdjustment", header: "Jumlah", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    me.detail.oid = me.grid1.getSelectedId();
                    console.log(me.detail);
                    me.Apply();
                }
            },
        },
        scrollX: true
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inventory Adjustment",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
             {
                 name: "pnlStatus",
                 items: [
                     { name: "IAStatus", text: "", cls: "span4", readonly: true, type: "label" },
                     {
                         type: "buttons", cls: "span4", items: [
                             { name: "btnPosting", text: "Posting", icon: "icon-gear", click: "savePosting()", cls: "btn btn-warning", disable: "control.DisablePosting" },
                         ]
                     }
                 ]
             },
             {
                 name: "pnlA",
                 title: "",
                 items: [

                         { name: "AdjustmentNo", text: "No. Adjustment", cls: "span4 ", placeHolder: "ADJ/XX/XXXXXX", disable: true, validasi: "required" },
                         { name: "AdjustmentDate", text: "Tgl. Adjustment", cls: "span4", type: "ng-datepicker", validasi: "required" },
                         { name: "ReferenceNo", text: "No. Reference", cls: "span4", validasi: "required" },
                         { name: "ReferenceDate", text: "Tgl. Reference", cls: "span4", type: "ng-datepicker", validasi: "required" },
                 ]
             },
             {
                 name: "pnlB",
                 show: "data.AdjustmentNo != undefined",
                 title: "",
                 items: [
                         {
                             text: "No. Part",
                             type: "controls",
                             items: [
                                 { name: "PartNo", model: "detail.PartNo", cls: "span2", placeHolder: "No. Part", type: "popup", btnName: "btnPartNo", readonly: true, click: "PartNo()" },
                                 { name: "PartName", model: "detail.PartName", cls: "span6", placeHolder: "Nama Part", readonly: true }
                             ]
                         },
                         {
                             text: "Kode Gudang",
                             type: "controls",
                             items: [
                                 { name: "WarehouseCode", model: "detail.WarehouseCode", cls: "span2", placeHolder: "Kode Gudang", type: "popup", btnName: "btnWarehouseCode", disable: "detail.PartNo === undefined", readonly: true, click: "WarehouseCode()" },
                                 { name: "WarehouseName", model: "detail.WarehouseName", cls: "span6", placeHolder: "Nama Gudang", readonly: true }
                             ]
                         },
                         { name: "AdjustmentCode", model: "detail.AdjustmentCode", text: "Kode Adjustment", cls: "span3 ", type: "select2", datasource: "comboAdjustmentCode", disable: "detail.WarehouseCode === undefined", },
                         { name: "ReasonCode", model: "detail.ReasonCode", text: "Alasan", cls: "span3 ", type: "select2", datasource: "comboReasonCode", disable: "detail.WarehouseCode === undefined", },
                         { name: "QtyAdjustment", model: "detail.QtyAdjustment", text: "Jumlah", cls: "span2  number", placeHolder: "0", disable: "detail.WarehouseCode === undefined", },
                         {
                             type: "buttons",
                             items: [
                                     { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditModel()", show: "detail.oid === undefined", disable: "detail.AdjustmentCode === undefined" },
                                     { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddEditModel()", show: "detail.oid !== undefined" },
                                     { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                     { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "Closedetail()", show: "detail.oid !== undefined" }
                             ]
                         },
                         {
                             name: "wxInventoryAdjust",
                             type: "wxdiv",
                         }
                 ]
             }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spInventoryAdjustmentController");
    }

});