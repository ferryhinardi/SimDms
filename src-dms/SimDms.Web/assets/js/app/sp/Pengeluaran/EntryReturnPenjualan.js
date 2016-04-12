var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spEntryReturnPenjualanController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        me.init();
        var lookup = Wx.blookup({
            name: "CustPickingListLookup",
            title: "Pencarian Pelanggan",
            manager: spManager,
            query: "Get4BrowseRtrFakturPenjualan",
            defaultSort: "ReturnDate desc",
            columns: [
            { field: "ReturnNo", title: "No. Retur" },
            { field: "ReturnDate", title: "Tgl. Return Date", template: "#= (ReturnDate == undefined) ? '' : moment(ReturnDate).format('DD MMM YYYY') #" },
            { field: "FPJNo", title: "No. Faktur Pajak" },
            { field: "FPJDate", title: "Tgl. Faktur Pajak", template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                //me.data = result;
                me.lookupAfterSelect(result);
                me.control.IsShowPanel = true;
                me.loadDetail();
                me.PopulateReturPartDetail();
                setTimeout(function () { me.CheckReturnStatus(me.data.ReturnNo); }, 1000);
                me.Apply();
            }
        });
    }

    me.delete = function () {
        if (confirm("Apakah Anda yakin menghapus data ini ?")) {
            $http.post('sp.api/EntryReturnPenjualan/DeleteReturHdr', { "ReturnNo": me.data.ReturnNo }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.CheckReturnStatus(me.data.ReturnNo);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }


    me.loadDetail = function () {
        $http.post('sp.api/EntryReturnPenjualan/PopulateCustomerDetails', me.data).
        success(function (data, status, headers, config) {
            if (data.success) {
                me.detail = data.data;
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }


    me.save = function (e, param) {
        var valid = $(".main form").valid();
        if (valid) {
            $http.post('sp.api/EntryReturnPenjualan/SaveReturHdr', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success(data.Message);
                    me.data.ReturnNo = data.ReturnNo;
                    me.control.IsShowPanel = true;
                    me.loadDetail();
                    me.PopulateReturPartDetail();
                    setTimeout(function () { me.CheckReturnStatus(me.data.ReturnNo) }, 1000);
                } else {
                    MsgBox(data.Message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }

    me.LinkDetail = function () {
        me.detail.Month = me.data.Month;
        me.detail.Year = me.data.Year;
    }

    me.initialize = function () {
        me.control = {};
        me.detail2 = {};
        me.print = {};
        me.control.IsShowPanel = false;
        me.control.IsDisableRef = false;
        me.control.IsDisablePrint = true;
        me.control.IsDisablePosting = true;
        me.control.IsShowTypePrint = false;
        //me.print.DocPrintType 
        me.statusCode = 0;

        $http.post('sp.api/SpInquiry/Default')
        .success(function (result) {
            me.currentDate = result.currentDate;
            me.data.ReturnDate = me.data.FPJDate = me.data.ReferenceDate = me.currentDate;
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

        me.clearTable(me.grid1);
        me.detail = {};
        $('#ReturnStatus').html("");
    }


    me.grid1 = new webix.ui({
        container: "wxreturdetail",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "NoUrut", header: "No.", fillspace: true },
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "PartNoOriginal", header: "Part No Original", fillspace: true },
            { id: "DocNo", header: "Doc No", fillspace: true },
            { id: "QtyBill", header: "Qty Faktur", fillspace: true },
            { id: "QtyReturn", header: "Qty Return", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    if (me.statusCode != "2") {
                        var detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail2 = detail;
                        me.Apply();
                    }
                }
            }
        }
    });

    me.FPJLookup = function () {
        var lookup = Wx.blookup({
            name: "CustPickingListLookup",
            title: "Pencarian Pelanggan",
            manager: spManager,
            query: "Get4FakturPenjualan",
            defaultSort: "FPJDate desc",
            columns: [
            { field: "FPJNo", title: "No. Faktur Pajak" },
            { field: "FPJDate", title: "Tgl. Faktur Pajak", template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #" },
            { field: "CustomerCode", title: "Customer Code" },
            { field: "CustomerName", title: "Customer Name" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.FPJNo = result.FPJNo;
                me.data.FPJDate = result.FPJDate;
                me.data.CustomerCode = result.CustomerCode;
                me.Apply();
            }
        });
    }

    me.PostingReturn = function () {
        if (confirm("Apakah anda yakin ?")) {
            $http.post('sp.api/EntryReturnPenjualan/PostingReturnPartDetails', me.data).
           success(function (data, status, headers, config) {
               if (data.success) {
                   me.CheckReturnStatus(me.data.ReturnNo);
                   me.ClearDtl();
               } else {
                   MsgBox(data.message, MSG_ERROR);
               }
           }).
           error(function (data, status, headers, config) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
           });
        }
    }

    me.PopulateReturPartDetail = function () {
        $http.post('sp.api/EntryReturnPenjualan/PopulatePartReturDetails', { "ReturnNo": me.data.ReturnNo }).
        success(function (data, status, headers, config) {
            me.grid1.detail = data;
            me.loadTableData(me.grid1, me.grid1.detail);
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.DeleteDtl = function () {
        if (confirm("Apakah Anda ingin mengapus data ini ?")) {
            $http.post('sp.api/EntryReturnPenjualan/DeletePartReturDetails', { "model": me.detail2, "ReturnNo": me.data.ReturnNo }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data has been deleted...");
                    me.PopulateReturPartDetail();
                    setTimeout(function () { me.CheckReturnStatus(me.data.ReturnNo); }, 1000);
                    me.detail2 = {};
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    }

    me.CheckReturnStatus = function (ReturnNo) {
        $http.post('sp.api/Pengeluaran/CheckStatus', { WhereValue: ReturnNo, Table: "spTrnSRturHdr", ColumnName: "ReturnNo" })
        .success(function (v, status, headers, config) {
            if (v.success) {
                $('#ReturnStatus').html('<span style="font-size:28px;color:red;font-weight:bold">' + v.statusPrint.toUpperCase() + "</span>");
                if (v.statusCode === "0") {
                    me.control.IsDisablePrint = false;
                    me.control.IsDisablePosting = true;
                    if (me.grid1.detail.length > 0) {
                        me.control.IsDisableRef = true;
                    }
                    else {

                        me.control.IsDisablePrint = true;
                        me.control.IsDisablePosting = true;
                    }
                    //me.hasChanged = false;
                    //me.isInitialize = true;
                } else if (v.statusCode === "1") {
                    me.control.IsDisablePrint = false;
                    me.control.IsDisablePosting = false;
                    me.control.IsDisableRef = true;
                    //me.hasChanged = false;
                    //me.isInitialize = true;
                } else if (v.statusCode === "2" || v.statusCode === "3") {
                    me.control.IsDisablePrint = true;
                    me.control.IsDisablePosting = true;
                    me.control.IsDisableRef = true;
                    //me.hasChanged = false;
                    //me.isInitialize = true;
                }
                me.statusCode = v.statusCode;
            } else {
                // show an error message
                MsgBox(v.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.SaveDtl = function () {
        $http.post('sp.api/EntryReturnPenjualan/SavePartReturDetails', { "model": me.detail2, "ReturnNo": me.data.ReturnNo, "FPJNo": me.data.FPJNo }).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data has been saved...");
                me.PopulateReturPartDetail();
                setTimeout(function () { me.CheckReturnStatus(me.data.ReturnNo); }, 1000);
                me.detail2 = {};
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.ClearDtl = function () {
        me.detail2 = {};
        me.loadTableData(me.grid1, me.grid1.detail);
    }

    me.PartLookup = function () {
        var lookup = Wx.blookup({
            name: "PartLookup",
            title: "Pencarian No. Part",
            manager: spManager,
            query: new breeze.EntityQuery.from("GetPartReturDetails").withParameters({ fpjNo: me.data.FPJNo }),
            defaultSort: "PartNo desc",
            columns: [
            { field: "PartNo", title: "No. Part" },
            { field: "PartNoOriginal", title: "No. Part Original" },
            { field: "QtyBill", title: "Qty. Faktur" },
            { field: "DocNo", title: "No. Dokumen" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.detail2 = result;
                me.Apply();
            }
        });
    }

    me.PrintReturn = function () {
        if (me.print.DocPrintType === "" || me.print.DocPrintType === undefined) {
            me.control.IsShowTypePrint = true;
        } else {
            $http.post('sp.api/EntryReturnPenjualan/PrintRetur', {"ReturnNo": me.data.ReturnNo}).
           success(function (data, status, headers, config) {
               if (data.success) {
                   var data = me.data.ReturnNo + "," + me.data.ReturnNo + "," + "profitcenter" + "," + "typeofgoods";
                   var rparam = "ga";
                   console.log(me.print.DocPrintType);
                   Wx.showPdfReport({
                       id: me.print.DocPrintType,
                       pparam: data,
                       rparam: rparam,
                       textprint:true,
                       type: "devex"
                   });

                   me.print = {};
                   me.control.IsShowTypePrint = false;
                   me.CheckReturnStatus(me.data.ReturnNo);
               } else {
                   MsgBox(data.message, MSG_ERROR);
               }
           }).
           error(function (data, status, headers, config) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
           });

        }
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Entry Sales Return",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlStatus",
                title: " ",
                items: [
                { name: "ReturnStatus", text: "", cls: "span2", readonly: true, type: "label" },
                {
                    type: "buttons", cls: "span3", items: [
                      { name: "btnPrint", text: "Print", click: "PrintReturn()", disable: "control.IsDisablePrint" },
                      { name: "btnPosting", text: "Posting", click: "PostingReturn()", disable: "control.IsDisablePosting" },
                    ]
                },
                {
                    name: "DocType", model: "print.DocPrintType", text: "Document Type", cls: "span3", type: "select", required: true, show: "control.IsShowTypePrint",
                    items: [
                        { value: "SpRpTrn012", text: "Sales Retur Memo", required: true },
                        { value: "GnRpTrn001", text: "Nota Retur", required: true },
                    ]
                },
                ]
            },
            {
                name: "pnlA",
                title: "",
                items: [
                        { name: "ReturnNo", text: "No. Retur", cls: "span3", placeHolder: "RTR/XX/YYYY", readonly: true },
                        { name: "ReturnDate", text: "Tgl. Retur", cls: "span3", placeHolder: "DD/MM/YYYY", type: "ng-datepicker", disable: "control.IsShowPanel" },
                        { name: "FPJNo", text: "No. FPJ", cls: "span3", placeHolder: "", type: "popup", btnName: "btnFPJLookup", readonly: true, click: "FPJLookup()" },
                        { name: "FPJDate", text: "Tgl. FPJ", cls: "span3", placeHolder: " ", type: "ng-datepicker", readonly: true },
                        { name: "ReferenceNo", text: "No. Reference", cls: "span3", placeHolder: " ", required: "required", disable: "control.IsDisableRef" },
                        { name: "ReferenceDate", text: "Tgl. Reference", cls: "span3", placeHolder: " ", type: "ng-datepicker", disable: "control.IsDisableRef" },
                ]
            },
            {
                name: "pnlB",
                title: "Customer Info",
                show: "control.IsShowPanel",
                items: [
                        {
                            text: "Customer",
                            type: "controls",
                            items: [
                                { name: "CustomerCode", cls: "span3", placeHolder: "Category Code", btnName: "btnCategoryCode", readonly: false, click: "CategoryCode()", disable: "data.Month === undefined || data.Year === undefined" },
                                { name: "CustomerName", model: "detail.CustomerName", cls: "span3", placeHolder: "Category Name", readonly: true }
                            ]
                        },
                        { name: "Address1", model: "detail.Address1", text: "Alamat", cls: "span6", placeHolder: "Address 1", readonly: true },
                        { name: "Address2", model: "detail.Address2", text: "", cls: "span6", placeHolder: "Address 2", readonly: true },
                        { name: "Address3", model: "detail.Address3", text: "", cls: "span6", placeHolder: "Address 3", readonly: true },
                        {
                            text: "Customer Contact",
                            type: "controls",
                            items: [
                                { name: "PhoneNo", model: "detail.PhoneNo", text: "No. Telp", cls: "span3", placeHolder: "No. Telp", readonly: true },
                                { name: "FaxNo", model: "detail.FaxNo", text: "No. Fax", cls: "span3", placeHolder: "No. Fax", readonly: true },
                            ]
                        },
                        { name: "PickingSlipNo", model: "detail.PickingSlipNo", text: "No. PL", cls: "span3", placeHolder: "No. PL", readonly: true },
                        { name: "OrderType", model: "detail.OrderType", text: "Order Type", cls: "span3", placeHolder: "Order Type", readonly: true },
                        { name: "TOPCode", model: "detail.TOPCode", text: "TOP Code", cls: "span2", placeHolder: "TOP Code", readonly: true },
                ]
            },
            {
                name: "pnlC",
                title: "Retur Detail",
                show: "control.IsShowPanel",
                items: [
                            
                                { name: "PartNo", text: "No. Part",model: "detail2.PartNo", cls: "span4", type: "popup", placeHolder: "Part No", btnName: "btnLookupPart", readonly: true, click: "PartLookup()" },
                                { name: "PartNoOriginal",text: "No. Part Original", model: "detail2.PartNoOriginal", cls: "span4", placeHolder: "Part No Original", readonly: true },
                                { name: "DocNo", text:"No. Dokumen",model: "detail2.DocNo", cls: "span4", placeHolder: "Doc No", readonly: true },
                                { name: "QtyBill",text:"Qty. Faktur", model: "detail2.QtyBill", cls: "span2", placeHolder: "Qty Bill", readonly: true },
                                { name: "QtyReturn",text:"Qty. Return", model: "detail2.QtyReturn", cls: "span2", placeHolder: "Qty Return", disable: "detail2.PartNo === undefined" },
                        {
                            type: "buttons", cls: "span6", items: [
                                { name: "btnAdd", text: "Save", icon: "icon-plus", click: "SaveDtl()", cls: "btn btn-primary", disable: "detail2.PartNo === undefined" },
                                { name: "btnDlt", text: "Delete", icon: "icon-remove", click: "DeleteDtl()", cls: "btn btn-danger", disable: "detail2.PartNo === undefined" },
                                { name: "BtnClr", text: "Clear", icon: "icon-remove", click: "ClearDtl()", cls: "btn btn-danger", disable: "detail2.PartNo === undefined" },
                            ]
                        },
                        {
                            name: "wxreturdetail",
                            type: "wxdiv",
                        },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spEntryReturnPenjualanController");
    }

});