var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spReservedSparepartController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.comboOptCode = [{ value: '+', text: '+' },
                    { value: '-', text: '-' }];

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "ReservedSparepartBrowse",
            title: "Reserved Sparepart Browse",
            manager: spPersediaanManager,
            query: "lnk5003Browse",
            defaultSort: "ReservedNo asc",
            columns: [
                { field: "ReservedNo", title: "No. Reserved" }, 
                { field: "ReservedDate", title: "Tgl. Reserved" , template: "#= (ReservedDate == undefined) ? '' : moment(ReservedDate).format('DD MMM YYYY') #" },
                { field: "ReferenceNo", title: "No. Referensi" },
                { field: "ReferenceDate", title: "Tgl. Referensi" , template: "#= (ReferenceDate == undefined) ? '' : moment(ReferenceDate).format('DD MMM YYYY') #"}
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                me.Apply();
                me.loadDetail(result);

                me.data.OprCode=result.OprCode;
            }
        });
    };

    me.printPreview = function () {
        $http.post('sp.api/persediaan/printlnk5003', { reservedNo: me.data.ReservedNo })
       .success(function (e) {
           if (e.success) {

               var par = me.data.ReservedNo + "," + me.data.ReservedNo + "," + "typeofgoods";
               var rparam = "Print SpRpTrn030";
               Wx.showPdfReport({
                   id: "SpRpTrn030",
                   par: par,
                   rparam: rparam,
                   textprint:true,
                   type: "devex"
               });

               $('#RSStatus').html(e.lblstatus);
               $('#btnPosting').removeAttr('disabled');
           }
           else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) { });
    }

    me.loadDetail = function (data) {
        $http.post('sp.api/persediaan/getdatatablelnk5003', me.data).
            success(function (data, status, headers, config) {
                $('#RSStatus').html(data.lblstatus);
                if (data == ""){
                    $('#btnPrintPreview').attr('disabled', 'disabled');
                }
                else {
                    $('#btnPrintPreview').removeAttr('disabled');
                }
                if (data.lblstatus == 'Printed') {
                    $('#btnPosting').removeAttr('disabled');
                }
                else {
                    $('#btnPosting').attr('disabled', 'disabled');
                }
                me.grid.detail = data.table;
                me.loadTableData(me.grid1, me.grid.detail);
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
                me.detail.AvailQty=data.AvailQty;
                me.Apply();
            }
        });
    }

    me.saveData = function (e, param) {
        $http.post('sp.api/Persediaan/Savelnk5003', me.data).
            success(function (v, status, headers, config) {
                if (v.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    me.loadDetail(me.data);
                    me.data.ReservedNo =v.data.ReservedNo ;
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.savePosting = function (e, param) {
        MsgConfirm("Apakah anda yakin?", function (e) {
            if (e) {
                $http.post('sp.api/Persediaan/Postinglnk5003', me.data).
                    success(function (v, status, headers, config) {
                        if (v.success) {
                            Wx.Success("Posting saved...");
                            $("#RSStatus").html(v.lblstatus);
                            $('#btnPartNo').attr('disabled', 'disabled');
                            $('#btnPosting').attr('disabled', 'disabled');
                        } else {
                            MsgBox(data.message, MSG_ERROR);
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
        me.loadTableData(me.grid1, me.grid.detail);
    };

    me.AddEditModel = function () {
        if (me.detail.PartNo === undefined || me.detail.PartNo == null) {
            MsgBox("PartNo is required!!!", MSG_ERROR);
            return;
        }
        if (parseInt(me.detail.ReservedQty) < 0) {
            MsgBox("Jumlah part yang akan di reserve harus lebih besar dari 0", MSG_WARNING)
            return;
        }

        if (parseInt(me.detail.ReservedQty) > parseInt(me.detail.AvailQty)) {
            MsgBox("Data tidak dapat disimpan karena available part tidak mencukupi \n Qty reserved maksimal =" + me.detail.AvailQty, MSG_WARNING);
            return;
        }

        me.LinkDetail();

        me.detail.ReservedNo = me.data.ReservedNo;
        me.detail.WarehouseCode = "00";
        $http.post('sp.api/Persediaan/SaveDetailslnk5003', me.detail).
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
        me.detail.ReservedNo = me.data.ReservedNo;
    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/Persediaan/Deletelnk5003', me.data).
                    success(function (dl, status, headers, config) {
                        if (dl.success) {
                            Wx.Info("Record has been deleted...");
                            $('#RSStatus').html(dl.lblstatus);
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

                $http.post('sp.api/Persediaan/DeleteDetailslnk5003', me.detail).
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

    me.initialize = function () {
        me.data.ReservedDate = me.now();
        me.data.ReferenceDate = me.now();
        me.clearTable(me.grid1);
        me.detail = {};
        me.isPrintAvailable = true;

        $('#RSStatus').html("");
        $('#RSStatus').css(
            {
                "font-size": "28px",
                "color": "red",
                "font-weight": "bold",
                "text-align": "right"
            });
    }

    me.grid1 = new webix.ui({
        container: "wxReservedSP",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "PartNo", header: "No. Part", fillspace: true },
            { id: "PartName", header: "Nama Part", fillspace: true },
            { id: "ReservedQty", header: "Jumlah Reserved", fillspace: true, format: webix.i18n.numberFormat, css: { "text-align": "right" } }
        ],
        on: {
            onItemClick: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    me.detail.oid = me.grid1.getSelectedId();
                    me.loadPartInfo(me.detail.PartNo);
                    me.Apply();
                }
            }
        }
    });

    me.loadPartInfo = function (PartNo) {
        $http.post('sp.api/persediaan/PartInfo?PartNo='+ PartNo).
            success(function (v, status, headers, config) {
            angular.extend(me.detail,v.data1[0]);
            angular.extend(me.detail,v.data2[0]);
            angular.extend(me.detail,v.data3[0]);
            me.detail.AvailQty=v.dataQty[0].AvailQty;
 
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.Closedetail = function()
    {
        me.detail = {};
        me.grid1.clearSelection();
    };
    
    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Reserved Sparepart",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlStatus",
                items: [
                    { name: "RSStatus", text: "", cls: "span4", readonly: true, type: "label" },
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

                        { name: "ReservedNo", text: "No. Reserved", cls: "span4", placeHolder:"RSV/XX/XXXXXX" ,disable:true,  validasi:"required"},
                        { name: "ReservedDate", text: "Tgl. Reserved", cls: "span4", type: "ng-datepicker",  validasi:"required" },
                        { name: "ReferenceNo", text: "No. Referensi", cls: "span4" ,  validasi:"required" },
                        { name: "ReferenceDate", text: "Tgl Referensi", cls: "span4", type: "ng-datepicker" ,  validasi:"required" },
                        { name: "OprCode",  text: "Kode Operasi", cls: "span4 ", type: "select2", datasource: "comboOptCode" ,  validasi:"required" },
                ]
            },
            {
                xtype: "tabs",
                show: "data.ReservedNo != undefined",      
                name: "tabRS",
                items: [
                    { name: "tabRQ", text: "Reserved Qty" },
                    { name: "tabPI", text: "Informasi Part" },
                ]
            },
            {
                name: "pnlB",
                show: "data.ReservedNo != undefined",      
                cls: "tabRS tabRQ",
                title: "",

                items: [
                        {
                            text: "No. Part",
                            type: "controls",
                            items: [
                                { name: "PartNo", model: "detail.PartNo", cls: "span2 ", placeHolder: "No. Part", type: "popup", btnName: "btnPartNo", readonly: false, click: "PartNo()" },
                                { name: "PartName", model: "detail.PartName", cls: "span6 ", placeHolder: "Nama Part", readonly: true }
                            ]
                        },
 
                         { name: "ReservedQty", model: "detail.ReservedQty", text: "Jumlah Reserved", cls: "span2 number", placeHolder: "0" },
                    {
                        type: "buttons",
                        items: [
                                {name: "btnAddModel", text: "Add", icon: "icon-plus", cls:"btn btn-info", click: "AddEditModel()", show: "detail.oid === undefined", disable:"detail.PartNo === undefined"},
                                {name: "btnUpdateModel", text: "Update", icon: "icon-save", cls:"btn btn-success", click: "AddEditModel()", show: "detail.oid !== undefined" },
                                {name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls:"btn btn-danger", click: "delete2()", show: "detail.oid !== undefined"},
                                {name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls:"btn btn-warning", click: "Closedetail()", show: "detail.oid !== undefined"}
                            ]
                    },
                    {
                        name: "wxReservedSP",
                        show: "data.ReservedNo != undefined",  
                        type: "wxdiv",
                    },
                ]
            },            
            {
                name: "pnlB",
                show: "data.ReservedNo != undefined",  
                cls: "tabRS tabPI",
                title: "",
                items: [
                     { name: "OnHand", model: "detail.OnHand", text: "On Hand", cls: "span2 ", placeHolder: "0", readonly: true },
                     { name: "OnOrder", model: "detail.OnOrder", cls: "span2 ", text: "On Order", placeHolder: "0", readonly: true },
                     { name: "InTransit", model: "detail.InTransit", cls: "span2 ", text: "In Transit", placeHolder: "0", readonly: true },
                     { name: "AllocationSP", model: "detail.AllocationSP", cls: "span2 ", text: "Alokasi Sparepart", placeHolder: "0", readonly: true },
                     { name: "AllocationSR", model: "detail.AllocationSR", cls: "span2 ", text: "Alokasi Bengkel", placeHolder: "0", readonly: true },
                     { name: "AllocationSL", model: "detail.AllocationSL", cls: "span2 ", text: "Alokasi Unit", placeHolder: "0", readonly: true },
                     { name: "BackOrderSP", model: "detail.BackOrderSP", cls: "span2 ", text: "BO Sparepart", placeHolder: "0", readonly: true },
                     { name: "BackOrderSR", model: "detail.BackOrderSR", cls: "span2 ", text: "BO Bengkel", placeHolder: "0", readonly: true },
                     { name: "BackOrderSL", model: "detail.BackOrderSL", cls: "span2 ", text: "BO Unit", placeHolder: "0", readonly: true },
                     { name: "ReservedSP", model: "detail.ReservedSP", cls: "span2 ", text: "Reserved Sparepart", placeHolder: "0", readonly: true },
                     { name: "ReservedSL", model: "detail.ReservedSL", cls: "span2 ", text: "Reserved Bengkel", placeHolder: "0", readonly: true },
                     { name: "ReservedSR", model: "detail.ReservedSR", cls: "span2 ", text: "Reserved Service", placeHolder: "0", readonly: true },
                     { name: "MovingCode", model: "detail.MovingCode", cls: "span2 ", text: "Moving Code", placeHolder: "0", readonly: true },
                     { name: "ABCClass", model: "detail.ABCClass", cls: "span2 ", text: "ABC Class", placeHolder: "0", readonly: true },
                     {
                         text: "Pemasok",
                         type: "controls",
                         items: [
                             { name: "SupplierCode", model: "detail.SupplierCode", cls: "span2", placeHolder: "Pemasok", readonly: true},
                             { name: "SupplierName", model: "detail.SupplierName", cls: "span6", placeHolder: "Nama Pemasok", readonly: true }
                         ]
                     },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spReservedSparepartController");
    }

});