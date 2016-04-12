var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var id = '0';

"use strict";

function spWarehouseTransferController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=RSWT').
    success(function (data, status, headers, config) {
        me.comboReasonCode = data;
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "WHTransferBrowse",
            title: "Warehouse Transfer Browse",
            manager: spPersediaanManager,
            query: "lnk5002Browse",
            defaultSort: "WHTrfNo asc",
            columns: [
                { field: "WHTrfNo", title: "No. WH Transfer" },
                { field: "WHTrfDate", title: "Tgl. WH Transfer" , template: "#= (WHTrfDate == undefined) ? '' : moment(WHTrfDate).format('DD MMM YYYY') #" },
                { field: "ReferenceNo", title: "No. Reference" },
                { field: "ReferenceDate", title: "Tgl. Reference" , template: "#= (ReferenceDate == undefined) ? '' : moment(ReferenceDate).format('DD MMM YYYY') #"}
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                me.Apply();
                me.loadDetail(result);
            }

        });
    };

    me.printPreview = function () {
        $http.post('sp.api/persediaan/printlnk5002', { whTrfNo: me.data.WHTrfNo })
       .success(function (e) {
           if (e.success) {

               var par = me.data.WHTrfNo + "," + me.data.WHTrfNo + "," + "typeofgoods";
               var rparam = "Print SpRpTrn007";
               Wx.showPdfReport({
                   id: "SpRpTrn007",
                   pparam: par,
                   rparam: rparam,
                   textprint:true,
                   type: "devex"
               });

               $('#WHStatus').html(e.lblstatus);
               $('#btnPosting').removeAttr('disabled');
           }
           else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) { });
    }

    me.loadDetail = function (data) {

        $http.post('sp.api/persediaan/getdatatablelnk5002', me.data).
            success(function (data, status, headers, config) {
                me.grid.detail = data.table;
                me.loadTableData(me.grid1, me.grid.detail);

                $('#WHStatus').html(data.lblstatus);
                if (data.lblstatus == 'Printed')
                {
                    $('#btnPosting').removeAttr('disabled');
                }
                else {
                    $('#btnPosting').attr('disabled', 'disabled');
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
            query: new breeze.EntityQuery.from("WarehouseCode").withParameters({ Id: id ,param: me.detail.PartNo }),
            defaultSort: "warehousecode asc",
            columns: [
                { field: "warehousecode", title: "Kode Gurang" },
                { field: "lookupvaluename", title: "Nama Gudang" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.FromWarehouseCode = data.warehousecode;
                me.detail.FromWarehouseName = data.lookupvaluename;
                me.isSave = false;
                me.Apply();
            }
        });
    };

    me.ToWarehouseCode = function () {
        id = '1';
        var lookup = Wx.blookup({
            name: "WarehouseCodeLookup",
            title: "Lookup Warehouse To",
            manager: spPersediaanManager,
            query: new breeze.EntityQuery.from("WarehouseCode").withParameters({ Id: id , param: me.detail.FromWarehouseCode }),
            defaultSort: "warehousecode asc",
            columns: [
                { field: "warehousecode", title: "Kode Gudang" },
                { field: "lookupvaluename", title: "Nama Gudang" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.ToWarehouseCode = data.warehousecode;
                me.detail.ToWarehouseName = data.lookupvaluename;
                me.isSave = false;
                me.Apply();
            }
        });
    };

    me.saveData = function (e, param) {
        $http.post('sp.api/Persediaan/Savelnk5002', me.data)
             .success(function (v, status, headers, config) {
                 if (v.success) {
                     Wx.Success(v.message);
                     me.startEditing();
                     me.loadDetail(me.data);
                     me.data.WHTrfNo = v.data.WHTrfNo;
                 } else {
                     MsgBox(v.message, MSG_ERROR);
                 }
             })
             .error(function (data, status, headers, config) {
                 MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
             });

    };

    me.savePosting = function (e, param) {
        $http.post('sp.api/Persediaan/Postinglnk5002', me.data)
        .success(function (e) {
            if (e.success) {
                Wx.Success(e.message);
                $('#WHStatus').html(e.lblstatus);
                $('#btnPartNo').attr('disabled', 'disabled');
                $('#btnPosting').attr('disabled', 'disabled');
                $('#btnFromWarehouseCode').attr('disabled', 'disabled');
                $('#btnToWarehouseCode').attr('disabled', 'disabled');
                $('#wxWHTransfer').attr('disabled', 'disabled');
            } else {
                MsgBox(e.message, MSG_ERROR);
            }
        })
        .error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    };

    me.UpdateGridDetail = function (data) {
        me.grid.detail = data;
        me.loadTableData(me.grid1, me.grid.detail);
    };

    me.AddEditModel = function () {

        if (parseFloat(me.detail.Qty) <= 0) {
            MsgBox("Jumlah part yang akan di transfer harus lebih besar dari 0", MSG_ERROR);
            return;
        }

        me.LinkDetail();

        me.detail.WHTrfNo = me.data.WHTrfNo;

        $http.post('sp.api/persediaan/validatedtllnk5002', me.detail)
        .success(function (e) {
            if (!e.success) {
                MsgBox(e.message, MSG_ERROR);
                return;
            }
            $http.post('sp.api/Persediaan/SaveDetailslnk5002', me.detail).
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
        })
        .error(function (e) { MsgBox('Terjadi Kesalahan, Hubungi SDMS Support'); });
    };

    me.UpdateModel = function () {
 
        if (me.detail.PartNo === undefined || me.detail.PartNo == null) {
            MsgBox("PartNo is required!!!", MSG_ERROR);
            return;
        }

        me.LinkDetail();

        me.detail.WHTrfNo = me.data.WHTrfNo;
        $http.post('sp.api/Persediaan/UpdateDetailslnk5002', me.detail).
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
                $http.post('sp.api/Persediaan/Deletelnk5002', me.data).
                    success(function (dl, status, headers, config) {
                        if (dl.success) {
                            Wx.Info("Record has been deleted...");
                            $('#WHStatus').html(data.lblstatus);
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
                var maindata = $.extend(me.data, me.detail);
                $http.post('sp.api/Persediaan/DeleteDetailslnk5002', maindata).
                    success(function (dl, status, headers, config) {
                        if (dl.success) {

                            me.detail = {};
                            Wx.Info("Record has been deleted...");

                            if (dl.count > 0)
                                me.UpdateGridDetail(dl.data);
                            else
                                me.clearTable(me.grid1);


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

    me.Closedetail = function()
    {
        me.detail = {};
        me.grid1.clearSelection();
    };
    
    me.initialize = function() {
        me.data.WHTrfDate=me.now();
        me.data.ReferenceDate=me.now();
        me.clearTable(me.grid1);
        me.detail = {};
        me.isPrintAvailable = true;

        $('#WHStatus').html("");
        $('#WHStatus').css(
            {
                "font-size": "28px",
                "color": "red",
                "font-weight": "bold",
                "text-align": "right"
            });
        $('#btnPosting').attr('disabled', 'disabled');
        //$('#wxWHTransfer').removeAttr('disabled');

        $('#wxWHTransfer').attr('disabled', 'disabled');
    };

    me.grid1 = new webix.ui({
        container: "wxWHTransfer",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "PartNo", header: "No. Part", fillspace: true },
            { id: "PartName", header: "Nama Part", width: 300 },
            //{ id: "FromWarehouseCode", header: "Kode Gudang",   width:100 },
            { id: "FromWarehouseName", header: "Gudang Asal", fillspace: true },
            { id: "ToWarehouseCode", header: "Gudang Tujuan", fillspace: true },
            { id: "Qty", header: "Jumlah", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "ReasonCode", header: "Alasan", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    me.detail.oid = me.grid1.getSelectedId();
                    me.Apply();
                }
            },
             scrollX:true    
        }
    });

    webix.event(window, "resize", function() {
        me.grid1.adjust();
    });

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Warehouse Transfer",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
             {
                 name: "pnlStatus",
                 items: [
                     { name: "WHStatus", text: "", cls: "span4", readonly: true, type: "label" },
                     {
                         type: "buttons", cls: "span4", items: [
                             { name: "btnPosting", text: "Posting", icon: "icon-gear", click: "savePosting()", cls: "btn btn-warning", disable: true },
                         ]
                     }
                 ]
             },
             {
                name: "pnlA",
                title: "",
                items: [

                        { name: "WHTrfNo", text: "No. WH Transfer", cls: "span4 ", placeHolder: "WTR/XX/YYYYYY", disable: true, validasi: "required" },
                        { name: "WHTrfDate", text: "Tgl. WH Transfer", cls: "span4", type: "ng-datepicker",  validasi:"required" },
                        { name: "ReferenceNo", text: "No. Reference", cls: "span4",  validasi:"required" },
                        { name: "ReferenceDate", text: "Tgl. Reference", cls: "span4", type: "ng-datepicker" ,  validasi:"required"},
                ]
            },
            {
                name: "pnlB",
                show: "data.WHTrfNo != undefined",
                title: "",

                items: [
                        {
                            text: "No. Part",
                            type: "controls",
                            items: [
                                { name: "PartNo", model: "detail.PartNo", cls: "span2 ", placeHolder: "No. Part", type: "popup", btnName: "btnPartNo", readonly: true, click: "PartNo()" },
                                { name: "PartName", model: "detail.PartName", cls: "span6 ", placeHolder: "Nama Part", readonly: true }
                            ]
                        },
                        {
                            text: "Gudang Asal",
                            type: "controls",
                            items: [
                                { name: "FromWarehouseCode", model: "detail.FromWarehouseCode", cls: "span2", placeHolder: "From Warehouse Code", type: "popup", disable: "detail.PartNo === undefined", readonly: true, click: "WarehouseCode()" },
                                { name: "FromWarehouseName", model: "detail.FromWarehouseName", cls: "span6", placeHolder: "Warehouse Name", readonly: true }
                            ]
                        },
                        {
                            text: "Gudang Tujuan",
                            type: "controls",
                            items: [
                                { name: "ToWarehouseCode", model: "detail.ToWarehouseCode", cls: "span2 ", placeHolder: "To Warehouse Code", type: "popup", disable: "detail.FromWarehouseCode === undefined", readonly: true, click: "ToWarehouseCode()" },
                                { name: "ToWarehouseName", model: "detail.ToWarehouseName", cls: "span6", placeHolder: "Warehouse Name", readonly: true }
                            ]
                        },
                        { name: "Qty", model: "detail.Qty", text: "Jumlah", cls: "span2 number", placeHolder: "0", disable: "detail.ReasonCode === undefined" },
                        { name: "ReasonCode", model: "detail.ReasonCode", text: "Alasan", cls: "span3 ", type: "select2", datasource: "comboReasonCode", disable: "detail.ToWarehouseCode === undefined" },

                {
                    type: "buttons",
                    items: [
                            { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditModel()", show: "detail.oid === undefined", disable: "detail.Qty === undefined" },
                            { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddEditModel()", show: "detail.oid !== undefined" },
                            { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                            { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "Closedetail()", show: "detail.oid !== undefined" }
                    ]
                },
                    {
                        name: "wxWHTransfer",
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
        SimDms.Angular("spWarehouseTransferController");
    }

});