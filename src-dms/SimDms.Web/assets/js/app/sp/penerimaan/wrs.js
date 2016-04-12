var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


"use strict";

function spWRSController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me }   );

    $http.post('sp.api/EntryWRS/Default')
    .success(function (result) {
        me.currentDate = result.currentDate;
    })
    .error(function (data, status, headers, config) {
        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
    });

    me.browse = function () {

        var lookup = Wx.blookup({
            name: "browse",
            title: "WRS Lookup",
            manager: spPenerimaanManager,
            query: new breeze.EntityQuery.from("EntryWRSBrowse").withParameters({ "RecType": me.pilihan }),
            defaultSort: "WRSNo desc",
            columns: [
            { field: "WRSNo", title: "No. WRS" },
            { field: "WRSDate", title: "Tgl. WRS", type: "date", format: "{0:dd-MMM-yyyy}" },
            
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isInProcess = true;
                me.data = data;
                me.detail = data;
                me.data.opt = "1";

                me.LoadWRSDetail(data.WRSNo, data.SupplierCode, data.TransType, data.BinningNo);
                
                
            }
        });
    }

    me.LoadWRSDetail = function (WRSNo, SupplierCode, TransType, BinningNo) {
        var src = "sp.api/EntryWRS/loadData?WRSNo=" + WRSNo + "&SupplierCode=" + SupplierCode + "&TransType=" + TransType + "&BinningNo=" + BinningNo;
        $http.post(src)
            .success(function (data, status, headers, config) {

                if (WRSNo == "") {
                    me.data.SupplierName = data.Supp["SupplierName"];
                    me.data.ReceivedQty = data.QtyTot["ReceivedQty"];
                    me.data.TransType = data.detVal["LookUpValueName"];
                    me.ReformatNumber();
                } else {
                    me.data.SupplierName = data.Supp["SupplierName"];
                    me.data.ReceivedQty = data.QtyTot["ReceivedQty"];
                    me.data.TransType = data.detVal["LookUpValueName"];
                    me.LoadGridDetail(data.DatGrid);
                    me.ReformatNumber();
                }
                
            }
         );
    }
   
    me.BinningNoBrowse = function () {

        var lookup = Wx.blookup({
            name: "btnBinningNo",
            title: "WRS Lookup",
            manager: spPenerimaanManager,
            query: new breeze.EntityQuery.from("BinningNoBrowse").withParameters({ "RecType": me.pilihan }),
            defaultSort: "BinningNo desc",
            columns: [
            { field: "BinningNo", title: "Binning No" },
            { field: "BinningDate", title: "Binning Date", type: "date", format: "{0:dd-MMM-yyyy}" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {

                //me.lookupAfterSelect(data);
                
                me.data = data;
                if (me.pilihan == "1") {
                    me.detail.WRSNo = "WRL/XX/XXXXXX";
                } else {
                    me.detail.WRSNo = "WRN/XX/XXXXXX";
                }

                //if (data.TransType == "4") {
                //    me.detail.WRSNo = "WRN/XX/XXXXXX";
                //} else {
                //    me.detail.WRSNo = "WRL/XX/XXXXXX";
                //}

                me.data.WRSDate = me.currentDate;
                me.data.TotWRSAmt = data.TotBinningAmt;
                me.LoadWRSDetail("", data.SupplierCode, data.TransType, data.BinningNo);
               // me.LoadWRSDetail(data.WRSNo);
               // me.isSave = true;
                //me.Apply();
                me.data.Status = null;

            }
        });
    }


    me.saveData = function (e, param) {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        var param = {
            pil: me.pilihan,
            model: me.savemodel
        }

        $http.post('sp.api/EntryWRS/save', param).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.detail.WRSNo = data.cod;
                    me.data.Status = data.status;
                    me.data.opt = "1";
                    me.LoadGridDetail(data.dataDtl);

                    // Detail langsung dijalankan saat me-save header.
                    //me.SaveDetail(me.data.BinningNo, data.cod);
                    //me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.SaveDetail = function (BinningNo, WRSNo) {
        param = {
            model: {},
            BinningNo: BinningNo,
            WRSNo: WRSNo
        }
        var src = "sp.api/EntryWRS/SaveRecordDetail";
        $http.post(src, param)
            .success(function (data, status, headers, config) {
                me.LoadGridDetail(data.dat);
            }
         );
    }


    me.delete = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/EntryWRS/delete', me.savemodel).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        Wx.Info("Data has been deleted...");

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
        angular.extend(me.savemodel, me.detail);
        $http.post('sp.api/EntryWRS/Print', me.savemodel).
            success(function (data, status, headers, config) {
                if (data.success) {
                    
                    if (data.datdet == null || data.datdet == "") {
                        MsgBox("Document tidak dapat dicetak karena tidak memiliki datad detail", MSG_ERROR);
                        return false;
                    } else {
                        me.data.Status = data.status;
                        var data = me.detail.WRSNo + "," + me.detail.WRSNo + "," + "typeofgoods";
                        var rparam = "admin";

                        Wx.showPdfReport({
                            id: "SpRpTrn004",
                            pparam: data,
                            rparam: rparam,
                            textprint:true,
                            type: "devex"
                        });
                    }
                    
					
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.CloseWRS = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        MsgConfirm("Are you sure to close?", function (result) {
            if (result) {
                $http.post('sp.api/EntryWRS/CloseWRS', me.savemodel).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        //me.init();
                        MsgBox(dl.message);
                        me.data.Status = 3;
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

    me.LoadGridDetail = function (data) {
            me.grid.detail = data;
            me.loadTableData(me.grid1, me.grid.detail);
            me.loadTableData(me.grid2, me.grid.detail);
            
            me.readyToModified();
    }


    me.$watch("pilihan", function (a, b) {
        if (a != b) {
            if (a == "1") {
                me.detail.WRSNo = "WRL/XX/XXXXXX";
            } else {
                me.detail.WRSNo = "WRN/XX/XXXXXX";
            }
            // me.$broadcast(a);
        }
    })


    me.grid1 = new webix.ui({
        container: "detailData",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "PurchasePrice", header: "Harga Beli",format: webix.i18n.intFormat, fillspace: true },
            { id: "DiscPct", header: "Diskon(%)", fillspace: true },
            { id: "ReceivedQty", header: "Jumlah Terima", format: webix.i18n.intFormat, fillspace: true },
            { id: "BoxNo", header: "No. Box", fillspace: true },
            { id: "NmPart", header: "Nama Part", width:300 }
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
        container: "detailData2",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "DocNo", header: "Document No", fillspace: true },
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "PurchasePrice", header: "Harga Beli", format: webix.i18n.intFormat, fillspace: true },
            { id: "DiscPct", header: "Diskon(%)", fillspace: true },
            { id: "ReceivedQty", header: "Jumlah Terima", format: webix.i18n.intFormat, fillspace: true },
            { id: "BoxNo", header: "No. Box", fillspace: true },
            { id: "NmPart", header: "Nama Part", width: 300 }
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


    me.initialize = function () {
        me.grid = {};
        me.clearTable(me.grid1);
        me.detail = {};
        me.detail.WRSNo = "WRL/XX/XXXXXX";
        me.data.HPPDate = me.currentDate;
        me.data.WRSDate = me.currentDate;
        me.data.BinningDate = me.currentDate;
        me.data.TaxDate = me.currentDate;
        me.data.ReferenceDate = me.currentDate;
        me.data.DNSupplierDate = me.currentDate;
        me.pilihan = "1";
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

    me.$watch('data.Status', function (nVal, oVal) {
        if (nVal == 3) {
            me.hasChanged = true;
            me.isSave = false;
            me.isLoadData = false;
            me.isPrintAvailable = false;
            me.isInitialize = false;
        }
        else {
            me.isPrintAvailable = true;
            me.isLoadData = true;
            me.hasChanged = true;
            me.isSave = true;
            me.isInitialize = false;
        }
    }, true);

    me.test = function () {

    }

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Entry WRS",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                    { name: "opt", show: "data2.e = 0" },
                    { name: "Status", show: "data2.e = 0" },
                        {
                            type: "buttons",
                            items: [
                                    { name: "btnCloseHpp1", text: "Closing WRS", icon: "icon icon-remove", cls: "btn btn-warning", click: "CloseWRS()", show: "data.Status != undefined", disable: "data.Status == 0 || data.Status == 3" },
                                    { name: "btnCloseHpp2", text: "OPEN", cls: "btn btn-success", show: "data.Status == 0", disable: "data.Status == 0" },
                                    { name: "btnCloseHpp4", text: "CLOSED WRS", cls: "btn btn-danger", show: "data.Status == 3", disable: "data.Status == 3" },
                                    { name: "btnCloseHpp3", text: "PRINTED", cls: "btn btn-info", show:"data.Status == 1", disable: "data.Status == 1"}
                            ]
                        }, { type: "hr", show: "data.Status != undefined" },
                        {
                            type: "optionbuttons",
                            name: "opsitest",
                            text: "",
                            model: "pilihan",
                            items: [
                                    { name: "1", text: "Pembelian/BPS", cls: "btn-default", click: "renderGrid()", disable: "data.BinningNo != undefined", },
                                    { name: "2", text: "Melalui PINV", cls: "btn-default", click: "renderGrid()", disable: "data.BinningNo != undefined", },
                                    { name: "3", text: "Transfer Transaksi", cls: "btn-default", click: "renderGrid()", disable: "data.BinningNo != undefined", },
                            ]
                        },
                        {
                            name: "WRSNo",
                            text: "WRS No",
                            cls: "span4",
                            model:"detail.WRSNo",
                            readonly: true

                        },
                        {
                            name: "WRSDate",
                            text: "Tgl. WRS",
                            type: "ng-datepicker",
                            cls: "span4",
                            disable: 'data.Status != undefined'
                        },
                        {
                            name: "ReferenceNo",
                            text: "No. Referensi",
                            cls: "span4",
                            readonly: true,
                            disable: 'data.Status != undefined'
                        },
                        {
                            name: "ReferenceDate",
                            text: "Tgl. Referensi",
                            cls: "span4",
                            type: "ng-datepicker",
                            disable: 'data.Status != undefined'
                        },
                        {
                            name: "BinningNo",
                            text: "No. BN",
                            cls: "span4  ",
                            type: "popup",
                            btnName: "btnBinningNo",
                            click: "BinningNoBrowse()",
                            validasi: "required",
                            readonly: true
                        },
                        {
                            name: "BinningDate",
                            text: "Tgl. Binning",
                            type: "ng-datepicker",
                            cls: "span4  ",
                            disable: true
                        },
                        {
                            name: "SupplierCode",
                            text: "Supplier Code",
                            cls: "span3  ",
                            readonly: true
                        },
                        {
                            name: "SupplierName",
                            text: "Supplier Name",
                            cls: "span5  ",
                            readonly: true
                        },
                        {
                            name: "TransType",
                            text: "Trans Type",
                            cls: "span4",
                            readonly: true
                        },
                        {
                            name: "DNSupplierNo",
                            text: "No. DN",
                            cls: "span4",
                            readonly: true,
                        },
                        {
                            name: "DNSupplierDate",
                            text: "Tgl. DN",
                            cls: "span4",
                            type: "ng-datepicker",
                            disable: true
                        }, { type: "hr" },
                        {
                            name: "TotItem",
                            text: "Total Item",
                            cls: "span2 number-int",
                            readonly: true
                        },
                        {
                            name: "ReceivedQty",
                            text: "Total QTY",
                            cls: "span2 number-int",
                            readonly: true
                        },
                        {
                            name: "TotWRSAmt",
                            text: "Total Draft WRS",
                            cls: "span4 number-int",
                            readonly: true,
                        },
                ]
            },
            {
                name: "pnlB",
                title: "Tabel Detail",
                show: "data.opt == 1 && pilihan != 1",
                items: [
                    {
                    title: "Tabel",
                    name: "detailData",
                    type: "wxdiv",
                    },
                ]
            },
            {
                name: "pnlC",
                title: "Tabel Detail",
                show: "data.opt == 1 && pilihan == 1 ",
                items: [
                    {
                        title: "Tabel",
                        name: "detailData2",
                        type: "wxdiv",
                    },
                ]
            }



        ]
    };
    
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spWRSController");
    }





});