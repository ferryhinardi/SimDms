var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var sendData = '';
var enabled = true;

"use strict";

function spentryOrderController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.lookupAfterSelect = function (value) {
        me.isLoadData = true;
        me.data = value;
        me.detail.POSNo = me.data.POSNo;
    }

    me.LoadSODetails = function () {
        $http.post("sp.api/PembelianEntryOrderSparepart/GetSODetailAOS", { POSNo: me.data.POSNo, branch: me.data.Branch }).
        success(function (data, status, headers, config) {
            me.lookupAfterSelect(data.dataHeader);
            me.data.SupplierName = data.supplierName;
            me.data.jumlahQty = data.totalorder;
            me.data.jumlahTotal = data.totalamt;
            me.grid1.detail = data.data;
            me.loadTableData(me.grid1, me.grid1.detail);

            if (me.data.isGenPORDD == false) {
                $('#pnlPrint').show();
            } else {
                $('#pnlPrint').hide();
            }

        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
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

    $('.btn-group > label').click(function () {
        $(this).removeClass("active");
        me.initialize();
    });

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
        me.detail = {};
        me.detail.DiscPct = 0.00;
        me.IsShowPanelB = false;
        me.data.POSDate = me.now();
        //me.data.Status = -1;
        me.isPrintAvailable = true;
        me.clearTable(me.grid1);
        me.isBO = "0";
        me.DataSelected = false;

        me.data.POSNo = localStorage.getItem('POSNo');
        me.data.Branch = localStorage.getItem('Branch');
        if (me.data.POSNo != undefined && me.data.Branch != undefined)
        {
            console.log("POS NO: " + me.data.noPOS)
            console.log("Branch: " + me.data.Branch)
            $('#btnSuplier').hide();
            $('#btnCreatePOS').hide();
            $('#btnCancelPOS').hide();
            $('#btnBrowse').hide();
            $("[ng-model='isBO']").attr('disabled', true);
            $('#SupplierCode, #btnSupplierCode, #OrderType').attr('disabled', 'disabled');
            
            me.LoadSODetails();

        } 
    }

    me.createPRODS = function () {
        me.data.Branch = localStorage.getItem('Branch');
        $http.post('sp.api/PembelianEntryOrderSparepart/CreatePORDSAOS', { model: me.data, Branch: me.data.Branch })
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
        window.location = "sp.api/PembelianEntryOrderSparepart/DownloadFileAOS?SupplierCode=" + me.data.SupplierCode + "&POSNo=" + me.data.detail.POSNo + "&Branch=" + me.data.detail.Branch;
    }

    SendPopup = function () {
        $http.post('sp.api/PembelianEntryOrderSparepart/ValidateHeaderFileAOS', { model: me.data, Branch: me.data.detail.Branch })
        .success(function (e) {
            if (!e.success) {
                MsgConfirm(e.message, function (result) {
                    if (result) {
                        MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                            if (result) {
                                $http.post('sp.api/PembelianEntryOrderSparepart/SendFileAOS', { model: me.data, Branch: me.data.detail.Branch })
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
            else {
                MsgConfirm("Apakah anda yakin ingin mengirim data ini ?", function (result) {
                    if (result) {
                        $http.post('sp.api/PembelianEntryOrderSparepart/SendFileAOS', { model: me.data, Branch: me.data.detail.Branch })
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
        //toolbars: WxButtons,
        panels: [
                   {
                        name: "pnlPrint",
                        title: " ",
                        items: [
                        { name: "StatusSO", text: "", cls: "span3", readonly: true, type: "label" },
                        {
                            type: "buttons", cls: "span5",
                            items: [
                                        { name: "btnCreatePRODS", text: "Create PORDS", icon: "icon-plus", cls: "btn btn-success", click: "createPRODS()" }
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
                                                { name: "0", text: "Back Order" },
                                                { name: "1", text: "Non Back Order" }
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