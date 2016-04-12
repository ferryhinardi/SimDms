var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var sendData = '';
var enabled = true;

"use strict";

function spentryOrderController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/ComboOrderType').
    success(function (data, status, headers, config) {
        me.comboOrderType = data;
    });

    $http.post('sp.api/PembelianEntryOrderSparepart/GetLookupEOS_FLAG')
    .success(function (v, status, headers, config) {
        if (v.success) {
            me.enabled = v.enabled;
            console.log(me.enabled);
        }
    });

    me.browse = function () { 
        var lookup = Wx.klookup({
            name: "btnPosView",
            title: "POS Code Lookup",
            url: "sp.api/PembelianEntryOrderSparepart/spTrnPPOSHdrBrowse",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { field: 'POSNo', dir: 'asc' }
            ],
            columns: [
            { field: "POSNo", title: "POSNo" },
            {
                field: "POSDate", title: "Pos Date", width: "130px",
                template: "#= (POSDate == undefined) ? '' : moment(POSDate).format('DD MMM YYYY') #"
            },
            { field: "Status", title: "Status", attributes: { style: "text-align:right;" } },
            { field: "SupplierCode", title: "SupplierCode" },
            { field: "SupplierName", title: "SupplierName" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                console.log(data);
                //$('div.btn-group > label').removeClass("active");
                //me.lookupAfterSelect(data);
                me.LoadSODetails(data.POSNo);
                me.CheckSO(data.POSNo);
                console.log(data.isBO);
                if (data.isBO == true)
                    me.isBO = "1";
                else
                    me.isBO = "0";
                me.DataSelected = false;
                
            }
        });
    }

    //me.lookupAfterSelect = function (value) {
    //    me.isLoadData = true;
    //    me.data = value;
    //    //alert(me.data.isBO);
    //    //if (me.data.isBO == "true")
    //    //    me.isBO = "1";
    //    //else
    //    //    me.isBO = "0";

    //    me.detail.POSNo = me.data.POSNo;
    //    setTimeout(function () {
    //        me.hasChanged = false;
    //        me.startEditing();
    //        me.isSave = false;
    //        $scope.$apply();

    //        me.ReformatNumber();

    //    }, 200);
    //}

    me.LoadSODetails = function (POSNo) {
        $http.post("sp.api/PembelianEntryOrderSparepart/GetSODetail", { "POSNo": POSNo }).
        success(function (data, status, headers, config) {
            me.lookupAfterSelect(data.dataHeader);
            me.data.SupplierName = data.supplierName;
            me.data.jumlahQty = data.totalorder;
            me.data.jumlahTotal = data.totalamt;
            me.grid1.detail = data.data;
            me.loadTableData(me.grid1, me.grid1.detail);
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
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
            { field: "OnOrder", title: "OnOrder", attributes: { style: "text-align:right;" } },
            { field: "RetailPriceInclTax", title: "RetailPriceInclTax", attributes: { style: "text-align:right;" } },
            { field: "RetailPrice", title: "RetailPrice", attributes: { style: "text-align:right;" } },
            { field: "MovingCode", title: "MovingCode", attributes: { style: "text-align:right;" } },
            { field: "ABCClass", title: "ABCClass" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.checkOTValue(data.DiscPct);
                me.detail.PartNo = data.PartNo;
                me.detail.PartName = data.PartName;
                me.detail.PurchasePrice = data.PurchasePrice;
                me.detail.Note = "-";
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.checkOTValue = function (discPct) {
        $http.post('sp.api/PembelianEntryOrderSparepart/CheckOTValue', {lookupValue: me.data.OrderType})
       .success(function (e) {
           me.detail.DiscPct = parseFloat(discPct) + parseFloat(e.paravalue);
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.SupplierCodeBrowse = function () {
        console.log(me.isBO)
        if (me.isBO === undefined) {
            MsgBox("Silahkan pilih tipe back order", MSG_ERROR);
        }else
        {
            var lookup = Wx.klookup({
                name: "btnSupplierCodeView",
                title: "Supplier Lookup",
                url: "sp.api/grid/SupplierLookup",
                serverBinding: true,
                pageSize: 10,
                sort: [
                    { field: 'SupplierCode', dir: 'asc' }
                ],
                columns: [
                { field: "SupplierCode", title: "Supplier Code", width: 150 },
                { field: "SupplierName", title: "Supplier Name", width: 280 },
                { field: "Alamat", title: "Alamat", width: 680 },
                { field: "Diskon", title: "Diskon", width: 90, attributes: { style: "text-align:right;" } },
                { field: "Profit", title: "Profit Center", width: 140 }
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
    }

    me.saveData = function (e, param) {
        me.data.isBO = parseInt(me.isBO);
        console.log(me.data.isBO);
        $http.post('sp.api/PembelianEntryOrderSparepart/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.data.POSNo = data.data.POSNo;
                    me.detail.POSNo = data.data.POSNo;
                    me.lookupAfterSelect(data.data);
                   
                   

                    me.startEditing();
                    //setTimeout(function(){me.startEditing()},1500);
                } else {
                    MsgBox(data.message, MSG_INFO);
                }
            }).
            error(function (data, status, headers, config) {
                console.log(data);
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.savedetail = function (e, param) {
        me.detail.DiscPct = me.detail.DiscPct == undefined ? 0 : me.detail.DiscPct;
        if (me.detail.PartNo == undefined) {
            MsgBox("Part No tidak boleh kosong", MSG_ERROR);
        }
        else if (me.detail.OrderQty == undefined || me.detail.OrderQty == 0) {
            MsgBox("Jumlah Order tidak boleh kosong dan lebih dari 0", MSG_ERROR);
        }
        //else if (me.detail.DiscPct == undefined) {
        //    //console.log(me.detail.DiscPct);
        //    MsgBox("Diskon tidak boleh kosong", MSG_ERROR);
        //}
        else {
            $http.post('sp.api/PembelianEntryOrderSparepart/SaveDetails', { "model": me.detail, "POSNo": me.data.POSNo }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data Part Detail saved...");
                    me.detail = {};
                    me.LoadSODetails(me.data.POSNo);
                    me.CheckSO(me.data.POSNo);
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

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (event) {
            $http.post('sp.api/PembelianEntryOrderSparepart/Delete', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data has been deleted...");
                    //me.startEditing();
                    me.detail = {};
                    me.LoadSODetails(me.data.POSNo);
                    me.CheckSO(me.data.POSNo);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.printPreview = function () {
        $http.post('sp.api/PembelianEntryOrderSparepart/PrintSO', {POSNo:me.data.POSNo}).
        success(function (data, status, headers, config) {
            if (data.success) {

                Wx.showPdfReport({
                    id: "sprptrn002srt",
                    pparam: ""+me.data.POSNo+","+me.data.POSNo+",profitcenter,typeofgoods",
                    rparam: "admin",
                    textprint:true,
                    type:"devex"
                });

               Wx.Success("Print");
               me.CheckSO(me.data.POSNo);
           } else {
               MsgBox(data.message, MSG_ERROR);
           }
       }).
       error(function (data, status, headers, config) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }

    me.Deletedetail = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/PembelianEntryOrderSparepart/DeleteDetailPO', {"PODtl":me.detail, "POSNo":me.data.POSNo}).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        Wx.Info("Record has been deleted...");
                        me.LoadSODetails(me.data.POSNo);
                        me.Canceldetail();
                        me.lookupAfterSelect(dl.data);
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                    setTimeout(function () {
                        //me.startEditing();
                        //me.isPrintAvailable = true;
                    }, 200);
                }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.showSupplier = function () {
        Wx.loadForm();
        Wx.showForm({ url: "gn/master/supplier" });
    }

    me.cancelPOS = function () {
        MsgConfirm("Are you sure to continue this process?", function (result) {
            if (result) {
                $http.post('sp.api/PembelianEntryOrderSparepart/CancelPOS', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        Wx.Info("SO Has been canceled");
                        me.CheckSO(me.data.POSNo);
                        //me.startEditing();
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

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/PembelianEntryOrderSparepart/Delete', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        Wx.Info("Record has been deleted...");

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

    $('.btn-group > label').click(function () {
        $(this).removeClass("active");
        me.initialize();
    });

    me.createPOS = function () {
        MsgConfirm("Are you sure to continue this process ?", function (result) {
            $http.post('sp.api/PembelianEntryOrderSparepart/CreatePOS', me.data)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    me.CheckSO(me.data.POSNo);
                } else {
                    // show an error message
                    MsgBox(data.message, MSG_ERROR);
                }
            }).error(function (e, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        });
    }

    me.CheckSO = function (POSNo) {
        $http.post('sp.api/Pengeluaran/CheckStatus', { WhereValue: POSNo, Table: "SpTrnPPOSHdr", ColumnName: "POSNo" })
        .success(function (v, status, headers, config) {
            if (v.success) {
                me.data.Status = v.statusCode;
                $('#StatusSO').html('<span style="font-size:28px;color:red;font-weight:bold">' + v.statusPrint.toUpperCase() + "</span>");
                if (me.data.Status == '2') {
                    $('#SupplierCode, #btnSupplierCode, #OrderType, #btnDelete').attr('disabled', true);
                }
                else {
                    $('#SupplierCode, #btnSupplierCode, #OrderType').removeAttr('disabled');
                    $('#btnDelete').removeAttr('disabled');
                }
                me.isLoadData = true;
                me.lookupAfterSelect(me.data);

            } else {
                // show an error message
                MsgBox(v.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.grid1 = new webix.ui({
        container: "wxsalestarget",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "OrderQty", header: "Jumlah Order", fillspace: true, css: { 'text-align': 'right' } },
            { id: "DiscPct", header: "Diskon %", fillspace: true, css: { 'text-align': 'right' } },
            { id: "PurchasePrice", header: "Harga Beli", fillspace: true, css: { 'text-align': 'right' } },
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

    $('.btn-group').click(function () {
        me.data.isBO = me.isBO;
        console.log(me.data.isBO);
    });

    me.Canceldetail = function () {
        me.detail = {};
        me.loadTableData(me.grid1, me.grid1.detail);
        //me.detail.DataSelected = false;
    }

    me.initialize = function () {
        //$('div.btn-group > label').removeClass("active");
        me.detail = {};
        me.data = {};
        me.detail.DiscPct = 0.00;
        me.IsShowPanelB = false;
        $http.post('sp.api/PembelianEntryOrderSparepart/Default')
        .success(function (result) {
            me.currentDate = result.currentDate;
            me.data.POSDate = me.currentDate;
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

        me.data.Status = -1;
        me.isPrintAvailable = true;
        me.clearTable(me.grid1);
        me.isBO =  me.data.isBO = "1";
        me.DataSelected = false;
        $('#StatusSO').html('');
        $('#SupplierCode, #btnSupplierCode, #OrderType').removeAttr('disabled');
       
    }

    me.createPRODS = function () {
        $http.post('sp.api/PembelianEntryOrderSparepart/CreatePORDS', me.data)
        .success(function (v, status, headers, config) {
            if (v.success) {
                Wx.showFlatFile({ data: v.data });
            } else {
                // show an error message
                MsgBox(v.message, MSG_ERROR);
                console.log(v.err);
            }
            me.startEditing();
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    SavePopup = function () {
        window.location = "sp.api/PembelianEntryOrderSparepart/DownloadFile?SupplierCode="+me.data.SupplierCode+"&POSNo="+me.data.POSNo;
    }

    SendPopup = function () {
        $http.post('sp.api/PembelianEntryOrderSparepart/ValidateHeaderFile', me.data)
        .success(function (e) {
            if (!e.success)
            {
                MsgConfirm(e.message, function (result) {
                    if (result) {
                        MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                            if (result) {
                                $http.post('sp.api/PembelianEntryOrderSparepart/SendFile', me.data)
                                .success(function (data, status, headers, config) {
                                    if (data.success) {
                                        Wx.Success(data.message);
                                        me.HideForm();
                                    }
                                    else {
                                        MsgBox(data.message, MSG_ERROR);
                                    }
                                }).error(function (e, status, headers, config) {
                                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                                });
                            }
                        });
                    }
                });
            }
            else
            {
                MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                    if (result) {
                        $http.post('sp.api/PembelianEntryOrderSparepart/SendFile', me.data)
                        .success(function (data, status, headers, config) {
                            if (data.success) {
                                Wx.Success(data.message);
                                me.HideForm();
                            }
                            else {
                                MsgBox(data.message, MSG_ERROR);
                            }
                        }).error(function (e, status, headers, config) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                        });
                    }
                });
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

    me.HideForm = function () {
        $(".body > .panel").fadeOut();
    }
}


$(document).ready(function () {
    var options = {
        title: "Entry Order Sparepart",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
                   {
                        name: "pnlPrint",
                        title: " ",
                        items: [
                        { name: "StatusSO", text: "", cls: "span3", readonly: true, type: "label" },
                        {
                            type: "buttons", cls: "span5",
                            items: [
                                        { name: "btnSuplier", text: "Supplier", icon: "icon-user", cls: "btn btn-info", click: "showSupplier()" },
                                        { name: "btnCreatePOS", text: "Create POS", icon: "icon-plus", cls: "btn btn-success", click: "createPOS()", disable: "data.Status != 1" },
                                        { name: "btnCancelPOS", text: "Cancel POS", icon: "icon-remove", cls: "btn btn-danger", click: "cancelPOS()", disable: "data.Status != 2" },
                                        { name: "btnCreatePRODS", text: "Create PORDS", icon: "icon-plus", cls: "btn btn-success", click: "createPRODS()", disable: "data.Status < 1" }
                            ]
                        },
                        ]
                    },
                    {
                        name: "pnlA",
                        title: "",
                        items: [

                                {
                                    type: "optionbuttons",
                                    name: "tabpage1",
                                    model: "isBO",
                                    text: "Back Order",
                                    items: [
                                                { name: "1", text: "Back Order" },
                                                { name: "0", text: "Non Back Order" }
                                    ]
                                },
                                { name: "POSNo", text: "POSNo", cls: "span3 ", placeHolder: "POS/XX/XXXXXX", disable: true, validasi: "required" },
                                { name: "POSDate", text: "POSDate", type: "ng-datetimepicker", cls: "span3  " },
                                { name: "SupplierCode", text: "Supplier Code", cls: "span3  ", type: "popup", click: "SupplierCodeBrowse()", required: true, validasi: "required" },
                                { name: "SupplierName", text: "Supplier Name", cls: "span5  ", disable: true, required: true, validasi: "required" },
                                { name: "OrderType", text: "OrderType", cls: "span3  ", type: "select2", datasource: "comboOrderType" },
                                { name: "Transportation", text: "Transportation", cls: "span5", disable: "data.Status == 2" },
                                { name: "Remark", text: "Remark", cls: "spa6  ", type: "textarea", disable: "data.Status == 2" },
                        ]
                    },
                    {
                        name: "pnlB1",

                        title: "",
                        items: [
                                { name: "jumlahQty", text: "Jumlah Qty", cls: "span2  number", placeHolder: "0", readonly: "readonly" },
                                { name: "jumlahTotal", text: "Jumlah Total", cls: "span2  number", placeHolder: "0", readonly: "readonly" },
                        ]
                    },
                    {
                        name: "pnlB",
                        show: "true",
                        title: "",
                        items: [
                                { name: "PartNo", text: "Part No", model: "detail.PartNo", cls: "span3", type: "popup", btnName: "btnPartNo", click: "PartBrowse()", disable: "data.POSNo === undefined || data.Status == '2' " },
                                { name: "OrderQty", text: "Jumlah Order", model: "detail.OrderQty", cls: "span3 number", placeHolder: "0", required: true },
                                { name: "DiscPct", text: "Diskon", model: "detail.DiscPct", cls: "span2 number", placeHolder: "0", disable: "enabled == false", value: 0 },
                                { name: "PurchasePrice", text: "Price", model: "detail.PurchasePrice", cls: "span3 number", placeHolder: "0", readonly: true, required: true },
                                { name: "Note", text: "Note", model: "detail.Note", cls: "span5" },
                                {
                                    type: "buttons",
                                    items: [
                                                { name: "btnAddModel", text: "Save", icon: "icon-plus", cls: "btn btn-info", click: "savedetail()", disable: "detail.PartNo === undefined || data.POSNo===undefined" },
                                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "Deletedetail()", show: "detail.DataSelected" },
                                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "Canceldetail()", show: "detail.PartNo !== undefined" }
                                    ]
                                },
                        ]

                    },
                    {
                        name: "wxsalestarget",
                        xtype: "wxtable",
                    },

        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spentryOrderController");
    }




});