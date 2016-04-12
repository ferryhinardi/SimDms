var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spentryOrderController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {

        var lookup = Wx.blookup({
            name: "browse",
            title: "Entry HPP",
            manager: spPenerimaanManager,
            query: "EntryCSBrowse",
            defaultSort: "ClaimNo desc",
            columns: [
            { field: "ClaimNo", title: "Claim No" },
            { field: "ClaimDate", title: "Claim Date", type: "date", format: "{0:dd-MMM-yyyy}" }

            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.opti.opt = "1";
                me.lookupAfterSelect(data);
                me.LoadEntryCS(data.ClaimNo);
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.LoadEntryCS = function (ClaimNo) {
        var src = "sp.api/EntryClaimSupplier/EntryCSLoad?ClaimNo=" + ClaimNo;
        $http.post(src)
            .success(function (data, status, headers, config) {
                me.LoadEntryCSResult(data);
            }            
         );
    }

    me.LoadEntryCSResult = function (data) {
        me.data.ClaimDate = data.isi.ClaimDate;
        me.reqWRS = data.isi;
        me.detWRS = data.WRSDet;
        me.detSup = data.SuppDet;
        me.detTType = data.TransDet;
        me.loadTableData(me.grid1, data.GridPartNo);
        me.loadTableData(me.grid2, data.GridPartNoWrong);
        //me.LoadGridDetail(data.GridPartNo);
        me.renderGrid();
        me.ReformatNumber();
    }

    me.LoadWRSDetail = function (WRSNo) {
        var src = "sp.api/EntryClaimSupplier/WRSNoDetail?WRSNo=" + WRSNo;
        $http.post(src)
            .success(function (data, status, headers, config) {
                me.detWRS = data.WRSDet;
                me.detSup = data.SuppDet;
                me.detTType = data.TransDet;
                me.ReformatNumber();
            }
         );
    }

    me.Reason = [
        { value: 'CORR', text: 'KOREKSI STOK' },
        { value: 'DMG', text: 'RUSAK STOK' },
        { value: 'SCR', text: 'DIHANCURKAN' }
    ];

    me.WRSNoBrowse = function () {
        var lookup = Wx.blookup({
            name: "btnWRSNo",
            title: "WRS Lookup",
            manager: spPenerimaanManager,
            query: "WRSNoBrowse",
            defaultSort: "WRSNo DESC",
            columns: [
            { field: "WRSNo", title: "WRS No" },
            { field: "WRSDate", title: "WRS Date", type: "date", format: "{0:dd-MMM-yyyy}" },
            
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {

                //me.lookupAfterSelect(data);
                me.reqWRS = data;
                me.LoadWRSDetail(data.WRSNo);
                me.isSave = true;
                me.renderGrid();
                me.Apply();

            }
        });
    }
    
    me.WrongPartNoBrowse = function () {
        var lookup = Wx.blookup({
            name: "btnPartNoWrong",
            title: "Wrong Part LookUp",
            manager: spPenerimaanManager,
            query: "WrongPartNo",
            defaultSort: "PartNo asc",
            columns: [
            { field: "PartNoWrong", title: "Part No Wrong" },
            { field: "PartName", title: "Part Name" },
            { field: "SupplierCode", title: "Supplier Code" },
            { field: "IsGenuinePart", title: "Prd Suzuki" },
            { field: "ProductType", title: "Product Type" },
            { field: "PartCategory", title: "Category" },
            { field: "CategoryName", title: "Category Name" }
            ]
        });
        lookup.dblClick(function (data) {
            me.detail3 = data;
            me.isSave = false;
            me.Apply();
        });
    }

    me.PartNoBrowse = function () {
        var lookup = Wx.blookup({
            name: "btnPartNo",
            title: "Part LookUp",
            manager: spPenerimaanManager,
            query: new breeze.EntityQuery.from("PartNoBrowse").withParameters({ "WRS": me.reqWRS.WRSNo }),
            defaultSort: "PartNo asc",
            columns: [
            { field: "PartNo", title: "Part No" },
            { field: "PartName", title: "Part Name" },
            { field: "DocNo", title: "POS/BPS" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                if (me.pilihan == 'v1') {
                    me.detail2 = {};
                    me.detail3 = {};
                    me.detail = data;
                } else {
                    me.detail2 = data;
                    me.detail = {};
                }
                me.isSave = false;
                me.Apply();
            }
                
            
        });
    }

    me.saveData = function (e, param) {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detWRS);
        angular.extend(me.savemodel, me.reqWRS);
        me.savemodel.ClaimDate = me.data.ClaimDate;
        var claimDateEnabled = (me.data.ClaimNo == 'CLM/XX/YYYYYY') ? true : false;

        $http.post('sp.api/EntryClaimSupplier/save', { model: me.savemodel, ClaimDateEnabled: claimDateEnabled }).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                me.opti.opt = "1";
                me.data.ClaimNo = data.clm;
                me.data.Status = data.status;
                me.startEditing();
                $scope.renderGrid();
            } else {
                MsgBox(data.message, MSG_ERROR);
                if (data.claimData != undefined) {
                    console.log('me.LoadEntryCSResult(data.claimData);');
                    me.LoadEntryCSResult(data.claimData);
                }
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.save2 = function (e, param) {
        if (me.pilihan == "v1") {
            me.savemodeldtl = angular.copy(me.data);
            angular.extend(me.savemodeldtl, me.reqWRS);
            angular.extend(me.savemodeldtl, me.detail);
        } else {
            me.savemodeldtl = angular.copy(me.data);
            angular.extend(me.savemodeldtl, me.reqWRS);
            angular.extend(me.savemodeldtl, me.detail3);
            angular.extend(me.savemodeldtl, me.detail2);
            
        }
        var qtyRcv = me.detail.ReceivedQty === undefined ? 0 : me.detail.ReceivedQty;
        var qtyRcv2 = me.detail2.ReceivedQty === undefined ? 0 : me.detail2.ReceivedQty;
        var shortage = me.detail.ShortageQty === undefined ? 0 : me.detail.ShortageQty;
        var over = me.detail.OvertageQty === undefined ? 0 : me.detail.OvertageQty;
        var damage = me.detail.DemageQty === undefined ? 0 : me.detail.DemageQty;
        //var total = shortage + over + damage;
        var total = parseFloat(shortage) + parseFloat(over) + parseFloat(damage);
        var qtyWrong = me.detail2.WrongQty === undefined ? 0 : me.detail2.WrongQty;

        if (me.pilihan == "v1") {
            if (qtyRcv < total) {
                MsgBox("Receive qty is less than the qty total", MSG_ERROR);
                console.log(qtyRcv, total);
                return false;
            }
            if (shortage > qtyRcv) {
                MsgBox("Total shortage more than qty receive", MSG_ERROR);
                return false;
            }
            if (over > qtyRcv) {
                MsgBox("Total over more than qty receive", MSG_ERROR);
                return false;
            }
            if (damage > qtyRcv) {
                MsgBox("Total damage more than qty receive", MSG_ERROR);
                return false;
            }
            if (me.detail.ReasonCode == null) {
                MsgBox("Please select reason code", MSG_ERROR);
                return false;
            }
            if (total <= 0) {
                MsgBox("Total qty is less or equal than 0", MSG_ERROR);
                return false;
            }
        } else {
            if (qtyWrong <= 0) {
                MsgBox("Qty is 0 or less", MSG_ERROR);
                return false;
            } else if (qtyWrong > qtyRcv2) {
                MsgBox("Qty more than qty Receive", MSG_ERROR);
                return false;
            } else if (me.detail2.ReasonCode == null) {
                MsgBox("Please select reason code", MSG_ERROR);
                return false;
            }
        }
        $http.post('sp.api/EntryClaimSupplier/SaveDetail?pil='+me.pilihan, me.savemodeldtl).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                me.LoadGridDetail(data.data, me.pilihan);
                me.CloseModel();
                // me.loadTableData(me.grid1, data.data);
                me.startEditing();
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.delete2 = function () {
        if (me.pilihan == "v1") {
            me.savemodeldtl = angular.copy(me.data);
            angular.extend(me.savemodeldtl, me.detail);
        } else {
            me.savemodeldtl = angular.copy(me.data);
            angular.extend(me.savemodeldtl, me.detail3);
            angular.extend(me.savemodeldtl, me.detail2);

        }
        MsgConfirm("Apakah Anda Yakin???", function (result) {
            if (result) {
                $http.post('sp.api/EntryClaimSupplier/delete?pil='+me.pilihan, me.savemodeldtl).
                success(function (result, status, headers, config) {
                    if (result.success) {
                        //me.init();
                        Wx.Info("Data berhasil dihapus...");
                        me.LoadGridDetail(result.data, me.pilihan);
                        me.CloseModel();
                        me.data.Status = result.status;
                    } else {
                        MsgBox(result.message, MSG_ERROR);
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
        angular.extend(me.savemodel, me.detWRS);
        angular.extend(me.savemodel, me.reqWRS);
        MsgConfirm("Apakah Anda Yakin???", function (result) {
            if (result) {
                $http.post('sp.api/EntryClaimSupplier/deleteData', me.savemodel).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        Wx.Info("Data berhasil dihapus...");

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
        angular.extend(me.savemodel, me.detWRS);
        angular.extend(me.savemodel, me.reqWRS);
        $http.post('sp.api/EntryClaimSupplier/Print', me.savemodel).
        success(function (data, status, headers, config) {
            if (data.success) {
                me.data.Status = data.status;
                //var data = me.data.ClaimNo + "," + me.data.ClaimNo + "," + "profitcenter" + "," + "typeofgoods";
                var data = [
                    me.data.ClaimNo,
                    me.data.ClaimNo,
                    '300',
                    'typeofgoods'
                ]; 
                var rparam = "admin";
                console.log(data);
				Wx.showPdfReport({
					id: "SpRpTrn005",
					//pparam: data,
					pparam: data.join(','),
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

    me.CreateClaim = function () {
        var claimDateEnabled = (me.data.ClaimNo == 'CLM/XX/YYYYYY') ? true : false;
        if (me.pilihan == "v1") {
            me.savemodel = angular.copy(me.data);
            angular.extend(me.savemodel, me.detWRS);
            angular.extend(me.savemodel, me.reqWRS);
            angular.extend(me.savemodel, me.detail);
        } else {
            me.savemodel = angular.copy(me.data);
            angular.extend(me.savemodel, me.detWRS);
            angular.extend(me.savemodel, me.reqWRS);
            angular.extend(me.savemodel, me.detail2);
            angular.extend(me.savemodel, me.detail3);
        }
        MsgConfirm("Apakah Anda yakin?", function (result) {
            if (result) {
                var params = {
                    model: me.data,
                    pil: me.pilihan, 
                    model2: me.detail
                }
                $http.post('sp.api/EntryClaimSupplier/CreateClaim?pil=' + me.pilihan + '&ClaimDateEnabled=' + claimDateEnabled, me.savemodel).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.data.Status = data.status;
                        MsgBox(data.message);
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.LoadGridDetail = function (data, param) {
        if (param == "v1") {
            me.grid.detail = data;
            me.loadTableData(me.grid1, me.grid.detail);
        } else {
            me.grid.detail2 = data;
            me.loadTableData(me.grid2, me.grid.detail2);
        }
        
    }

    me.grid1 = new webix.ui({
        container: "partnp",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "DocNo", header: "POS/BPS", fillspace: true },
            { id: "ReceivedQty", header: "Received Qty", fillspace: true, format: me.intFormat, css: "text-right" },
            { id: "ShortageQty", header: "Shortage", fillspace: true, format: me.decimalFormat, css: "text-right" },
            { id: "OvertageQty", header: "Over", fillspace: true, format: me.decimalFormat, css: "text-right" },
            { id: "DemageQty", header: "Demage", fillspace: true, format: me.decimalFormat, css: "text-right" },
            { id: "ReasonCodeStr", header: "Reason", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    me.detail.old = me.grid1.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.grid2 = new webix.ui({
        container: "wrongpart",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "DocNo", header: "POS/BPS", fillspace: true },
            { id: "ReceivedQty", header: "Received Qty", fillspace: true, format: me.intFormat, css: "text-right" },
            { id: "PartNoWrong", header: "Part No Wrong", fillspace: true, format: me.decimalFormat, css: "text-right" },
            { id: "PurchasePrice", header: "Purchase Price", fillspace: true, format: me.decimalFormat, css: "text-right" },
            { id: "WrongQty", header: "Qty", fillspace: true, format: me.decimalFormat, css: "text-right" },
            { id: "ReasonCodeStr", header: "Reason", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid2.getSelectedId() !== undefined) {
                    me.detail2 = this.getItem(me.grid2.getSelectedId().id);
                    me.detail3 = this.getItem(me.grid2.getSelectedId().id);
                    me.detail2.old = me.grid2.getSelectedId();
                    me.detail3.old = me.grid2.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.CloseModel = function () {
        if (me.pilihan == "v1") {
            me.detail = {};
            me.grid1.clearSelection();
        } else {
            me.detail2 = {};
            me.detail3 = {};
            me.grid2.clearSelection();
        }
    }
    
    me.initialize = function () {
        me.grid = {};
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);
        me.detail = {};
        me.detail2 = {};
        me.detail3 = {};
        me.opti = {};
        me.reqWRS = {};
        me.detWRS = {};
        me.detSup = {};
        me.detTType = {};
        me.data.ClaimDate = me.now();
        me.reqWRS.WRSDate = me.now();
        me.detWRS.ReferenceDate = me.now();
        me.data.ClaimNo = "CLM/XX/YYYYYY";
        me.data.Status = undefined;
        me.pilihan = "v1";
        $scope.renderGrid();
        me.isPrintAvailable = true;
    }
    
    webix.event(window, "resize", function () {
        me.grid1.adjust();
        me.grid2.adjust();
    })

    $scope.renderGrid = function () {

        setTimeout(function () {
            me.grid1.adjust();
            me.grid2.adjust();
        }, 200);

    }

    me.start();

    me.$watch('data.Status', function (nVal, oVal) {
        if (nVal != undefined) {
            me.isSave = false;
        }
        else {
            me.isSave = true;
        }
    });

    me.$watch('reqWRS', function (newValue, oldValue) {
        if (!me.isInProcess) {

            var eq = (newValue == oldValue);

            // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
            if (!(_.isEmpty(newValue)) && !eq) {
                if (!me.hasChanged && !me.isLoadData) {
                    me.hasChanged = true;
                    me.isLoadData = false;
                }
                if (!me.isSave) {
                    me.isSave = true;
                    me.hasChanged = true;
                    if (me.data.ClaimNo != 'CLM/XX/YYYYYY') {
                        me.isLoadData = true;
                    }
                    else {
                        me.isLoadData = false;
                    }
                }
            } else {
                me.hasChanged = false;
                me.isSave = false;
            }
        }

    }, true);
}

$(document).ready(function () {
    var options = {
        title: "Entry Claim Supplier",
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
                  //  { name: "Pol", model:"Pol", cls: "hide" },
                        {
                            type: "buttons",
                            items: [
                                    { name: "btnCloseHpp1", text: "Create Claim", icon: "icon icon-plus", cls: "btn btn-warning", click: "CreateClaim()", show: "data.Status != undefined", disable: "data.Status != 1" },
                                    { name: "btnCloseHpp2", text: "OPEN", cls: "btn btn-success", show: "data.Status == 0", disable: "data.Status == 0" },
                                    { name: "btnCloseHpp3", text: "PRINTED", cls: "btn btn-info", show: "data.Status == 1", disable: "data.Status == 1" },
                                    { name: "btnCloseHpp4", text: "POSTED / CLOSED", cls: "btn btn-danger", show: "data.Status == 2", disable: "data.Status == 2" }
                ]
                        }, { type: "hr", show: "data.Status != undefined" },
                        {
                            name: "ClaimNo",
                            text: "Claim No",
                            cls: "span4 ",
                            readonly:true
                        }, 
                        {
                            name: "ClaimDate",
                            text: "Claim Date",
                            type: "ng-datepicker",
                            cls: "span4  ",
                            disable: "data.ClaimNo != undefined && data.ClaimNo != 'CLM/XX/YYYYYY'"
                        },
                        {
                            name: "WRSNo",
                            text: "WRS No",
                            cls: "span4  ",
                            type: "popup",
                            model:"reqWRS.WRSNo",
                            btnName: "btnWRSNo",
                            click: "WRSNoBrowse()",
                            validasi: "required",
                            readonly: true, 
                            disable: "data.Status != undefined"
                        },
                        {
                            name: "WRSDate",
                            text: "WRS Date",
                            type: "ng-datepicker",
                            model:"reqWRS.WRSDate",
                            cls: "span4  ",
                            readonly: true
                        },
                        {
                            name: "SupplierCode",
                            text: "SupplierCode",
                            model: "detWRS.SupplierCode",
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
                            name: "TransType",
                            text: "TransType",
                            model: "detWRS.TransType",
                            cls: "span3  ",
                            readonly: true
                        },
                        {
                            name: "LookUpValueName",
                            text: "Type Description",
                            model: "detTType.LookUpValueName",
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
                        {
                            name: "TotWRSAmt",
                            text: "Total WRS",
                            model: "detWRS.TotWRSAmt",
                            cls: "span4  ",
                            readonly: true
                        },
                        {
                            type: "optionbuttons",
                            name: "opsitest",
                            text: "Pilihan",
                            model: "pilihan",
                            show:"opti.opt ==1",
                            items: [
                                    { name: "v1", text: "Shortage, Damage, Over", cls: "btn-default", click: "renderGrid()" },
                                    { name: "v2", text: "Wrong Part", cls: "btn-default", click: "renderGrid()" },
                            ]
                        }, { type: "div" },
                        
                ]
            },

            {
                name: "pnlB",
                show: "pilihan == 'v1' && opti.opt == 1",
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
                            readonly: true,
                            disable: "data.Status == 2"
                        },

                        {
                            name: "DocNo",
                            text: "POS/BPS",
                            cls: "span4",
                            model: "detail.DocNo",
                            readonly: true
                        },
                        {
                            name: "ReceivedQty",
                            text: "Received Qty",
                            cls: "span2 number-int",
                            model: "detail.ReceivedQty",
                            readonly: true
                        },
//                        { name: "ShortageQty", text: "Shortage Qty", cls: "span2", show: "data.ReasonCode == 'DMG'" },
                        {
                            name: "ShortageQty",
                            model: "detail.ShortageQty",
                            text: "Shortage Qty",
                            cls: "span2 number-int",
                            disable: "data.Status == 2"
                          //  show: "pilihan == 'v3'"
                        },
                        {
                            name: "OvertageQty",
                            text: "Overtage Qty",
                            cls: "span2 number-int",
                            model: "detail.OvertageQty",
                            disable: "data.Status == 2"
                          //  show: "pilihan == 'v4'"
                        },
                        {
                            name: "DemageQty",
                            text: "Damage Qty",
                            cls: "span2 number-int",
                            model: "detail.DemageQty",
                            disable: "data.Status == 2"
                          //  show: "pilihan == 'v5'"
                        },
                        {
                            name: "ReasonCode",
                            text: "Reason",
                            cls: "span4",
                            type: "select2",
                            model: "detail.ReasonCode",
                            datasource: "Reason",
                            disable: "data.Status == 2"
                        },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddModel", text: "Add Detail", icon: "icon-plus", cls: "btn btn-info", click: "save2()", show: "detail.old === undefined", disable: "detail.PartNo === undefined || data.Status == 2" },
                                { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "save2()", show: "detail.old !== undefined", disable: "data.Status == 2" },
                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.old !== undefined", disable: "data.Status == 2" },
                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.old !== undefined" }
                        ]
                    },
                    {
                        title:"Tabel",
                        name: "partnp",
                        type: "wxdiv",
                        disable: "data.Status == 2"
                    },
                ]
            },
            {
                name: "pnlC",
                show: "pilihan == 'v2' && opti.opt == 1",
                title: "",
                items: [
                        {
                            name: "PartNo",
                            text: "Part No",
                            cls: "span4",
                            type: "popup",
                            model: "detail2.PartNo",
                            btnName: "btnPartNo",
                            click: "PartNoBrowse()",
                            disable: "data.Status == 2"
                        },

                        {
                            name: "DocNo",
                            text: "POS/BPS",
                            cls: "span4",
                            model: "detail2.DocNo",
                            readonly: true
                        },
                        {
                            name: "ReceivedQty",
                            text: "Received Qty",
                            cls: "span2 number-int",
                            model: "detail2.ReceivedQty",
                            readonly: true
                        },
                        {
                            name: "PurchasePrice",
                            model: "detail2.PurchasePrice",
                            text: "Purchase Price",
                            cls: "span2 number-int",
                            readonly:true
                        },
                        {
                            name: "WrongQty",
                            text: "Qty",
                            cls: "span2 number-int",
                            model: "detail2.WrongQty",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "PartNoWrong",
                            text: "Part No Wrong",
                            cls: "span4",
                            type: "popup",
                            model: "detail3.PartNoWrong",
                            btnName: "btnPartNoWrong",
                            click: "WrongPartNoBrowse()",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "ReasonCode",
                            text: "Reason",
                            cls: "span4",
                            type: "select2",
                            model: "detail2.ReasonCode",
                            datasource: "Reason",
                            disable: "data.Status == 2"
                        },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddModel", text: "Add Detail", icon: "icon-plus", cls: "btn btn-info", click: "save2()", show: "detail2.old === undefined", disable: "detail2.PartNo === undefined || data.Status == 2" },
                                { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "save2()", show: "detail2.old !== undefined", disable: "data.Status == 2" },
                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail2.old !== undefined", disable: "data.Status == 2" },
                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail2.old !== undefined", disable: "data.Status == 2" }
                        ]
                    },
                    {
                        title: "Tabel",
                        name: "wrongpart",
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
        SimDms.Angular("spentryOrderController");
    }

});