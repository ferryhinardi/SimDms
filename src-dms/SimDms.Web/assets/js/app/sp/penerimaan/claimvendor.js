var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


"use strict";

function spclaimvendorController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {

        var lookup = Wx.blookup({
            name: "browse",
            title: "Received Claim Vendor",
            manager: spPenerimaanManager,
            query: "RecClaimVendorBrowse",
            defaultSort: "ClaimReceivedNo desc",
            columns: [
            { field: "ClaimReceivedNo", title: "Claim Received No" },
            { field: "ClaimNo", title: "Claim No" },
            { field: "ClaimReceivedDate", title: "Claim Received Date", type:"date", format:"{0:dd-MMM-yyyy}" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.solo = data;
                me.opti.opt = "1";
                me.lookupAfterSelect(data);
                me.LoadEntryCS(data.ClaimNo, "");
                me.LoadDataGrid(data.ClaimReceivedNo);
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.LoadEntryCS = function (ClaimNo, Str) {
        var src = "sp.api/EntryClaimSupplier/EntryCSLoad?ClaimNo=" + ClaimNo;
        $http.post(src)
            .success(function (data, status, headers, config) {
                
                me.reqWRS = data.isi;
                me.detWRS = data.WRSDet;
                me.detSup = data.SuppDet;
                if (Str == "claim") {
                   // me.isSave = true;
                   // me.Apply();
                } else {
                    me.readyToModified();
                }
                me.ReformatNumber();
            }
         );
    }

    me.LoadDataGrid = function (ClaimReceivedNo) {
        var src = "sp.api/ReceivingClaimVendor/dataGridDetail?ClaimReceivedNo=" + ClaimReceivedNo;
        $http.post(src)
            .success(function (data, status, headers, config) {
                me.LoadGridDetail(data);
            }
         );
    }

    me.PartOrderDetail = function (PartNo) {
        var src = "sp.api/ReceivingClaimVendor/PartOrderDetail?PartNo=" + PartNo + "&ClaimNo=" + me.data.ClaimNo;
        $http.post(src)
            .success(function (data, status, headers, config) {
                me.detail2 = data;
            }
         );
    }

    me.ClaimNoBrowse = function () {

        var lookup = Wx.blookup({
            name: "btnClaimNo",
            title: "Claim No",
            manager: spPenerimaanManager,
            query: "RecClaimNo",
            defaultSort: "ClaimNo ASC",
            columns: [
            { field: "ClaimNo", title: "Claim No" },
            { field: "ClaimDate", title: "Claim Date" },

            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {

                //me.lookupAfterSelect(data);
                
                me.data = data;
                me.LoadEntryCS(data.ClaimNo, "claim");
                //me.isSave = true;
                //me.renderGrid();
//                me.Apply();

            }
        });
    }

    me.PartNoBrowse = function () {
        var lookup = Wx.blookup({
            name: "btnPartNo",
            title: "Part Order LookUp",
            manager: spPenerimaanManager,
            query: new breeze.EntityQuery.from("SpPartOrderBrowse").withParameters({ "CLM": me.data.ClaimNo }),
            defaultSort: "PartNo asc",
            columns: [
            { field: "PartNo", title: "Part No" },
            { field: "PartName", title: "Part Name" },
            { field: "DocNo", title: "POS/BPS" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail = data;
                me.PartOrderDetail(data.PartNo);
                me.isSave = false;
                me.Apply();
            }


        });
    }

    me.saveData = function (e, param) {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.solo);
        angular.extend(me.savemodel, me.detWRS);
        angular.extend(me.savemodel, me.reqWRS);
        angular.extend(me.savemodel, me.detSup);
       // console.log(me.savemodel);
        $http.post('sp.api/ReceivingClaimVendor/save', me.savemodel).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.opti.opt = "1";
                    me.solo.ClaimReceivedNo = data.clm;
                    me.LoadDataGrid("");
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }



    me.save2 = function (e, param) {
            me.savemodeldtl = angular.copy(me.data);
            angular.extend(me.savemodeldtl, me.solo);
            angular.extend(me.savemodeldtl, me.detail);
            angular.extend(me.savemodeldtl, me.detail2);
        
            var Shortage = me.detail2.ShortageQty === undefined ? 0 : me.detail2.ShortageQty;
            var Overtage = me.detail2.OvertageQty === undefined ? 0 : me.detail2.OvertageQty;
            var Demage = me.detail2.DemageQty === undefined ? 0 : me.detail2.DemageQty;
            var Wrong = me.detail2.WrongQty === undefined ? 0 : me.detail2.WrongQty;
            var RcvShortage = me.detail.RcvShortageQty === undefined ? 0 : me.detail.RcvShortageQty;
            var RcvOvertage = me.detail.RcvOvertageQty === undefined ? 0 : me.detail.RcvOvertageQty;
            var RcvDamage = me.detail.RcvDamageQty === undefined ? 0 : me.detail.RcvDamageQty;
            var RcvWrong = me.detail.RcvWrongQty === undefined ? 0 : me.detail.RcvWrongQty;
            
            if (RcvShortage > Shortage) {
                MsgBox("Jumlah Shortage Rcv melebihi Jumlah Rcv Outstanding", MSG_ERROR);
                return false;
            }
            if (RcvWrong > Wrong) {
                MsgBox("Jumlah Wrong Rcv melebihi Jumlah Wrong Outstanding", MSG_ERROR);
                return false;
            }
            if (RcvOvertage > Overtage) {
                MsgBox("Jumlah Over Rcv melebihi Jumlah Over Outstanding", MSG_ERROR);
                return false;
            }
            if (RcvDamage > Demage) {
                MsgBox("Jumlah Damage Rcv melebihi Jumlah Damage Outstanding", MSG_ERROR);
                return false;
            }
            
        
        $http.post('sp.api/ReceivingClaimVendor/SaveDetail', me.savemodeldtl).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.LoadGridDetail(data.data);
                    // me.loadTableData(me.grid1, data.data);
                   
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }


    me.delete2 = function () {
        me.savemodeldtl = angular.copy(me.data);
        angular.extend(me.savemodeldtl, me.solo);
        angular.extend(me.savemodeldtl, me.detail);
        angular.extend(me.savemodeldtl, me.detail2);

        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/ReceivingClaimVendor/delete', me.savemodeldtl).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        //me.init();
                        Wx.Info("Record has been deleted...");
                        me.LoadGridDetail(dl.data);
                        me.CloseModel();
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
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.solo);
        angular.extend(me.savemodel, me.detWRS);
        angular.extend(me.savemodel, me.reqWRS);
        angular.extend(me.savemodel, me.detSup);
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/ReceivingClaimVendor/deleteData', me.savemodel).
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

    me.printPreview = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.solo);
        angular.extend(me.savemodel, me.detWRS);
        angular.extend(me.savemodel, me.reqWRS);
        angular.extend(me.savemodel, me.detSup);
        $http.post('sp.api/ReceivingClaimVendor/Print', me.savemodel).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.data.Status = data.isi.Status;
                    if (data.datdet == null || data.datdet == "") {
                        MsgBox("Document tidak dapat dicetak karena tidak memiliki data detail", MSG_ERROR);
                        return false;
                    }
                    var pparam = me.solo.ClaimReceivedNo + "," + me.solo.ClaimReceivedNo + "," + "profitcenter" + "," + "typeofgoods";
                    var rparam = "admin";
					
					Wx.showPdfReport({
						id: "SpRpTrn027",
						pparam: pparam,
						rparam: rparam,
                        textprint:true,
						type: "devex"
					});
                    
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.CloseClaim = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.solo);
        angular.extend(me.savemodel, me.detWRS);
        angular.extend(me.savemodel, me.reqWRS);
        angular.extend(me.savemodel, me.detSup);
        angular.extend(me.savemodel, me.detail);
        MsgConfirm("Are you sure?", function (result) {
            if (result) {
                $http.post('sp.api/ReceivingClaimVendor/CloseClaim', me.savemodel).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        MsgBox(dl.pesan);

                    } else {
                        MsgBox(dl.pesan, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.CancelClaim = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.solo);
        angular.extend(me.savemodel, me.detWRS);
        angular.extend(me.savemodel, me.reqWRS);
        angular.extend(me.savemodel, me.detSup);
        angular.extend(me.savemodel, me.detail);
        MsgConfirm("Proses penghapusan qty yang di Claim" + "\r\n" +
        "Apakah anda yakin?" + "\r\n\r\n" +
        "(Tidak terbentuk Jurnal Pembatalan, silahkan dibuatkan)" + "\r\n" +
        "(Jurnal Pembatalan di Jurnal Memorial GL)"+
        "Proses Pembatalan Claim", function (result) {
            if (result) {
                $http.post('sp.api/ReceivingClaimVendor/CancelClaim', me.savemodel).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        MsgBox(dl.pesan);

                    } else {
                        MsgBox(dl.pesan, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.LoadGridDetail = function (data) {
            me.grid.detail = data;
            me.loadTableData(me.grid1, me.grid.detail);
            me.readyToModified();
    }

    me.grid1 = new webix.ui({
        container: "partnp",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "PartNo", header: "Part Order", fillspace: true },
            { id: "DocNo", header: "POS/BPS", fillspace: true },
            { id: "PartNoWrong", header: "Part No Wrong", fillspace: true },
            { id: "RcvShortageQty", header: "Receive Shortage Qty", css: "rightcell", format: me.decimalFormat },
            { id: "RcvOvertageQty", header: "Receive Over Qty", css: "rightcell", format: me.decimalFormat },
            { id: "RcvDamageQty", header: "Receive Demage Qty", css: "rightcell", format: me.decimalFormat },
            { id: "RcvWrongQty", header: "Receive Wrong Qty", css: "rightcell", format: me.decimalFormat }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    me.detail.old = me.grid1.getSelectedId();
                    me.detail2.old = me.grid1.getSelectedId();
                    me.PartOrderDetail(me.detail.PartNo);
                    me.Apply();
                }
            }
        }
    });

    me.CloseModel = function () {
        me.detail = {};
        me.detail2 = {};
        me.grid1.clearSelection();
    }


    me.initialize = function () {
        me.grid = {};
        me.clearTable(me.grid1);
        me.detail = {};
        me.detail2 = {};
        me.reqWRS = {};
        me.solo = {};
        me.detWRS = {};
        me.detSup = {};
        me.opti = {};
        me.data.ClaimDate = me.now();
        me.reqWRS.WRSDate = me.now();
        me.detWRS.ReferenceDate = me.now();
        me.solo.ClaimReceivedDate = me.now();
        me.solo.ClaimReceivedNo = "CLR/XX/YYYYYY";
        $scope.renderGrid();
        me.isPrintAvailable = true;
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    $scope.renderGrid = function () {

        setTimeout(function () {
            me.grid1.adjust();
        }, 200);

    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Receiving Claim Vendor",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                //show: "pilihan == 'v1'",
                items: [
                    { name: "opt", model: "opti.opt", show: "data2.e = 0" },
                    { name: "Status", show: "data2.e = 0" },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnCloseHpp1", text: "Closing", icon: "icon icon-remove", cls: "btn btn-warning", click: "CloseClaim()", show: "data.Status != undefined", disable: "data.Status == 0" },
                                { name: "btnCN", text: "CN/Batal", icon: "icon icon-remove", cls: "btn btn-warning", click: "CancelClaim()", show: "data.Status != undefined", disable: "data.Status == 0" },
                                { name: "btnCloseHpp2", text: "OPEN", cls: "btn btn-success", show: "data.Status == 0", disable: "data.Status == 0" },
                                { name: "btnCloseHpp3", text: "PRINTED", cls: "btn btn-info", show: "data.Status == 1", disable: "data.Status == 1" }
                        ]
                    }, { type: "hr", show: "data.Status != undefined" },
                        {
                            name: "ClaimReceivedNo",
                            text: "Claim Received No",
                            cls: "span4 ",
                            model: "solo.ClaimReceivedNo",
                            readonly: true
                        },
                        {
                            name: "ClaimReceivedDate",
                            text: "Claim Received Date",
                            type: "ng-datepicker",
                            model: "solo.ClaimReceivedDate",
                            cls: "span4  "
                        },
                        {
                            name: "ClaimNo",
                            text: "Claim No",
                            cls: "span4  ",
                            type: "popup",
                            btnName: "btnClaimNo",
                            click: "ClaimNoBrowse()",
                            validasi: "required",
                            required: true,
                            readonly: true
                        },
                        {
                            name: "ClaimDate",
                            text: "Claim Date",
                            type: "ng-datepicker",
                            cls: "span4  ",
                            readonly:true
                        },
                        {
                            name: "WRSNo",
                            text: "No. Draft WRS",
                            model: "reqWRS.WRSNo",
                            cls: "span4",
                            readonly: true
                        },
                        {
                            name: "WRSDate",
                            text: "WRS Date",
                            type: "ng-datepicker",
                            model: "reqWRS.WRSDate",
                            cls: "span4  ",
                            readonly: true
                        },
                        {
                            name: "SupplierCode",
                            text: "Supplier Code",
                            model: "detSup.SupplierCode",
                            cls: "span3  ",
                            readonly: true
                        },
                        {
                            name: "SupplierName",
                            text: "Supplier Name",
                            model: "detSup.SupplierName",
                            cls: "span5  ",
                            readonly: true
                        },
                        {
                            name: "ReferenceNo",
                            text: "Reference No",
                            model: "detWRS.ReferenceNo",
                            cls: "span4  ",
                            readonly: true
                        },
                        {
                            name: "ReferenceDate",
                            text: "Reference Date",
                            type: "ng-datepicker",
                            model: "detWRS.ReferenceDate",
                            cls: "span4  ",
                            readonly: true
                        },                        
                ]
            },
            {
                name: "pnlB",
                show: "opti.opt == 1",
                title: "",
                items: [
                        {
                            name: "PartNo",
                            text: "Part No",
                            cls: "span4",
                            type: "popup",
                            model: "detail.PartNo",
                            btnName: "btnPartNo",
                            click: "PartNoBrowse()",
                            readonly: true
                        },
                        {
                            name: "DocNo",
                            text: "POS/BPS",
                            cls: "span4",
                            model: "detail.DocNo",
                            readonly: true
                        },
                        {
                            name: "PartNoWrong",
                            text: "Part No Wrong",
                            model: "detail2.PartNoWrong",
                            readonly: true
                        },
                        {
                            name: "ShortageQty",
                            model: "detail2.ShortageQty",
                            text: "Shortage",
                            cls: "span2 number-int",
                            readonly: true
                        },
                        {
                            name: "OvertageQty",
                            text: "Overtage",
                            cls: "span2 number-int",
                            model: "detail2.OvertageQty",
                            readonly: true
                        },
                        {
                            name: "DemageQty",
                            text: "Damage",
                            cls: "span2 number-int",
                            model: "detail2.DemageQty",
                            readonly: true
                        },
                        {
                            name: "WrongQty",
                            text: "Wrong",
                            cls: "span2 number-int",
                            model: "detail2.WrongQty",
                            readonly: true
                        },
                        {
                            name: "RcvShortageQty",
                            model: "detail.RcvShortageQty",
                            text: "Receive Shortage",
                            cls: "span2 number-int"
                        },
                        {
                            name: "RcvOvertageQty",
                            text: "Receive Overtage",
                            cls: "span2 number-int",
                            model: "detail.RcvOvertageQty"
                        },
                        {
                            name: "RcvDamageQty",
                            text: "Receive Damage",
                            cls: "span2 number-int",
                            model: "detail.RcvDamageQty"
                        },
                        {
                            name: "RcvWrongQty",
                            text: "Receive Wrong",
                            cls: "span2 number-int",
                            model: "detail.RcvWrongQty"
                        },                        
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddModel", text: "Add Detail", icon: "icon-plus", cls: "btn btn-info", click: "save2()", show: "detail.old === undefined", disable: "detail.PartNo === undefined" },
                                { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "save2()", show: "detail.old !== undefined" },
                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.old !== undefined" },
                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.old !== undefined" }
                        ]
                    },
                    {
                        title: "Tabel",
                        name: "partnp",
                        type: "wxdiv",
                    },
                ]
            },
            

        ]
    };


    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spclaimvendorController");
    }

});