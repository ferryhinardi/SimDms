var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var sendData = '';

"use strict";

function spEntryRequestController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.showSupplier = function () {
        Wx.loadForm();
        Wx.showForm({ url: "gn/master/supplier" });
    }

    me.grid1 = new webix.ui({
        container: "wxReqDtl",
        view: "wxtable", css: "alternating", scrollX: true,
        columns: [
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "OrderQty", header: "Jumlah Order", fillspace: true, css: { 'text-align': 'right' } },
            { id: "PartName", header: "Nama Part", fillspace: true },
            { id: "Note", header: "Note", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    if (me.data.Status < 2) {
                        var details = this.getItem(me.grid1.getSelectedId().id);
                        me.detail = details;
                        me.detail.DataSelected = true;
                        me.Apply();
                    }
                }
            }
        }
    });

    me.initialize = function () {
        me.detail = {};
        me.IsShowPanelB = false;
        me.data.REQDate = me.now();
        me.data.Status = 0;
        me.isPrintAvailable = true;
        me.clearTable(me.grid1);
        me.DataSelected = false;
        $('#StatusREQ').html('');
        $('#SupplierCode, #btnSupplierCode').removeAttr('disabled');
        $('#PartNo, #btnPartNo, #REQDate').attr('disabled', 'disabled');
    }

    me.browse = function () {
        var lookup = Wx.klookup({
            name: "btnPosView",
            title: "REQ Code Lookup",
            url: "sp.api/EntryRequestSparepart/spTrnPREQHdrBrowse",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { field: 'REQNo', dir: 'asc' }
            ],
            columns: [
            { field: "REQNo", title: "REQNo" },
            {
                field: "REQDate", title: "Req Date", width: "130px",
                template: "#= (REQDate == undefined) ? '' : moment(REQDate).format('DD MMM YYYY') #"
            },
            { field: "StatusDesc", title: "Status" },
            { field: "SupplierCode", title: "SupplierCode" },
            { field: "SupplierName", title: "SupplierName" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                console.log(data);
                $('div.btn-group > label').removeClass("active");
                me.LoadSODetails(data.REQNo);
                me.CheckSO(data.REQNo);
                me.startEditing();
                me.data.REQNo = data.REQNo;
                $('#PartNo, #btnPartNo').removeAttr('disabled');
            }
        });

    }

    me.LoadSODetails = function (REQNo) {
        $http.post("sp.api/EntryRequestSparepart/GetREQDetail", { "REQNo": REQNo }).
        success(function (data, status, headers, config) {
            me.lookupAfterSelect(data.dataHeader);
            me.data.SupplierName = data.supplierName;
            me.data.jumlahQty = data.totalorder;
            me.grid1.detail = data.data;
            me.loadTableData(me.grid1, me.grid1.detail);
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.CheckSO = function (REQNo) {
        $http.post('sp.api/EntryRequestSparepart/CheckStatus', { "REQNo": REQNo })
        .success(function (v, status, headers, config) {
            if (v.success) {
                me.data.Status = v.statusCode;
                $('#StatusREQ').html('<span style="font-size:28px;color:red;font-weight:bold">' + v.statusPrint.toUpperCase() + "</span>");
                if (me.data.Status == '2') {
                    $('#SupplierCode, #btnSupplierCode').attr('disabled', true);
                    me.isLoadData = false;
                }
                else {
                    $('#SupplierCode, #btnSupplierCode').removeAttr('disabled');
                    me.isLoadData = true;
                }
            } else {
                MsgBox(v.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.SupplierCodeBrowse = function () {
        var lookup = Wx.klookup({
            name: "btnSupplierCodeView",
            title: "Supplier Lookup",
            url: "sp.api/grid/SupplierReqLookup",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { field: 'SupplierCode', dir: 'asc' }
            ],
            columns: [
            { field: "SupplierCode", title: "Supplier Code", width: 150 },
            { field: "SupplierName", title: "Supplier Name", width: 280 },
            { field: "Alamat", title: "Alamat", width: 680 },
            { field: "Status", title: "Status", width: 140 }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.PartBrowse = function () {
        var lookup = Wx.klookup({
            name: "btnPartView",
            title: "Part Lookup",
            url: "sp.api/grid/OrderSparepartLookup",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { field: 'PartNo', dir: 'asc' }
            ],
            columns: [
            { field: "PartNo", title: "PartNo" },
            { field: "PartName", title: "PartName", width: 380 },
            { field: "AvailQty", title: "AvailQty", attributes: { style: "text-align:right;" } },
            { field: "OnOrder", title: "OnOrder", attributes: { style: "text-align:right;" } }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.PartNo = data.PartNo;
                me.detail.Note = "-";
                me.Apply();
            }
        });
    }

    me.saveData = function (e, param) {
        $http.post('sp.api/EntryRequestSparepart/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.data.REQNo = data.data.REQNo;
                    me.detail.REQNo = data.data.REQNo;
                    me.lookupAfterSelect(data.data);
                    me.startEditing();
                    $('#PartNo, #btnPartNo').removeAttr('disabled');
                } else {
                    MsgBox(data.message, MSG_INFO);
                }
            }).
            error(function (data, status, headers, config) {
                console.log(data);
            });
    }

    me.savedetail = function (e, param) {
        if (me.detail.PartNo == undefined) {
            MsgBox("Part No tidak boleh kosong", MSG_ERROR);
        }
        else if (me.detail.OrderQty == undefined || me.detail.OrderQty == 0) {
            MsgBox("Jumlah Order tidak boleh kosong dan lebih dari 0", MSG_ERROR);
        }
        else {
            $http.post('sp.api/EntryRequestSparepart/SaveDetails', { "model": me.detail, "REQNo": me.data.REQNo }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data Part Detail saved...");
                    me.detail = {};
                    me.LoadSODetails(me.data.REQNo);
                    me.CheckSO(me.data.REQNo);
                    me.lookupAfterSelect(data.data);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }

    me.delete = function (e, param) {
        MsgConfirm("Are you sure to delete current record?", function (event) {
            $http.post('sp.api/EntryRequestSparepart/Delete', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data has been deleted...");
                    me.data = {};
                    me.detail = {};
                    me.data.REQDate = me.now();
                    me.data.Status = 0;
                    me.isPrintAvailable = true;
                    me.clearTable(me.grid1);
                    me.DataSelected = false;
                    $('#StatusREQ').html('');
                    $('#SupplierCode, #btnSupplierCode').removeAttr('disabled');
                    $('#PartNo, #btnPartNo, #REQDate').attr('disabled', 'disabled');
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.Deletedetail = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/EntryRequestSparepart/DeleteDetail', { "model": me.detail, "REQNo": me.data.REQNo }).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        Wx.Info("Record has been deleted...");
                        me.LoadSODetails(me.data.REQNo);
                        me.CheckSO(me.data.REQNo);
                        me.Canceldetail();
                        me.lookupAfterSelect(dl.data);
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.Canceldetail = function () {
        me.detail = {};
        me.loadTableData(me.grid1, me.grid1.detail);
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.printPreview = function () {
        $http.post('sp.api/EntryRequestSparepart/PrintREQ', { REQNo: me.data.REQNo }).
        success(function (data, status, headers, config) {
            if (data.success) {

                Wx.showPdfReport({
                    id: "SpRpTrn002Req",
                    pparam: "" + me.data.REQNo + "," + me.data.REQNo ,
                    textprint: true,
                    type: "devex"
                });

                Wx.Success("Print");
                me.CheckSO(me.data.REQNo);
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
       error(function (data, status, headers, config) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }

    me.createREQ = function () {
        MsgConfirm("Are you sure to continue this process ?", function (result) {
            $http.post('sp.api/EntryRequestSparepart/CreateREQ', me.data)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    me.CheckSO(me.data.REQNo);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).error(function (e, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.cancelREQ = function () {
        MsgConfirm("Are you sure to continue this process?", function (result) {
            if (result) {
                $http.post('sp.api/EntryRequestSparepart/CancelREQ', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        Wx.Info("REQ Has been canceled");
                        me.CheckSO(me.data.REQNo);
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.start();

    me.HideForm = function () {
        $(".body > .panel").fadeOut();
    }
}

$(document).ready(function () {
    var options = {
        title: "Input Permohonan Sparepart",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
                   {
                       name: "pnlPrint",
                       title: " ",
                       items: [
                       { name: "StatusREQ", text: "", cls: "span4", readonly: true, type: "label" },
                       {
                           type: "buttons", cls: "span4",
                           items: [
                                       { name: "btnSuplier", text: "Supplier", icon: "icon-user", cls: "btn btn-info", click: "showSupplier()" },
                                       { name: "btnCreateREQ", text: "Create REQ", icon: "icon-plus", cls: "btn btn-success", click: "createREQ()", disable: "data.Status != 1" },
                                       { name: "btnCancelREQ", text: "Cancel REQ", icon: "icon-remove", cls: "btn btn-danger", click: "cancelREQ()", disable: "data.Status != 2" },
                           ]
                       },
                       ]
                   },
                    {
                        name: "pnlA",
                        title: "",
                        items: [
                                { name: "REQNo", text: "REQNo", cls: "span3 ", placeHolder: "REQ/XX/XXXXXX", disable: true, validasi: "required" },
                                { name: "REQDate", text: "REQDate", type: "ng-datetimepicker", cls: "span3  " },
                                { name: "SupplierCode", text: "Supplier Code", cls: "span3  ", type: "popup", click: "SupplierCodeBrowse()", required: true, validasi: "required" },
                                { name: "SupplierName", text: "Supplier Name", cls: "span5  ", disable: true, required: true, validasi: "required" },
                                { name: "Remark", text: "Remark", cls: "spa6  ", type: "textarea", disable: "data.Status == 2" },
                        ]
                    },
                    {
                        name: "pnlB1",

                        title: "",
                        items: [
                                { name: "jumlahQty", text: "Jumlah Qty", cls: "span8  number", placeHolder: "0", readonly: "readonly" },
                        ]
                    },
                    {
                        name: "pnlB",
                        show: "true",
                        title: "",
                        items: [
                                { name: "PartNo", text: "Part No", model: "detail.PartNo", cls: "span3", type: "popup", btnName: "btnPartNo", click: "PartBrowse()" },
                                { name: "OrderQty", text: "Jumlah Order", model: "detail.OrderQty", cls: "span2 number", placeHolder: "0", required: true },
                                { name: "Note", text: "Note", model: "detail.Note", cls: "span3" },
                                {
                                    type: "buttons",
                                    items: [
                                                { name: "btnAddModel", text: "Save", icon: "icon-plus", cls: "btn btn-info", click: "savedetail()", disable: "detail.PartNo === undefined || data.REQNo===undefined" },
                                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "Deletedetail()", show: "detail.DataSelected" },
                                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "Canceldetail()", show: "detail.PartNo !== undefined" }
                                    ]
                                },
                        ]

                    },
                    {
                        name: "wxReqDtl",
                        xtype: "wxtable",
                    },

        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spEntryRequestController");
    }




});