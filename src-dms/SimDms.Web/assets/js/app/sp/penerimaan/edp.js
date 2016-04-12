var bPPN = 0;
"use strict";

function spEDPController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.Cari = function () {
        //bool bPPN = false;
        //if (chkPPN.Visible == true && chkPPN.Checked == true) bPPN = true;
        console.log(bPPN);

        $http.post('sp.api/edp/GetPINVData?isPPN=' + bPPN, me.data).
            success(function (data, status, headers, config) {
                if (data.success == true) {
                    me.data.DNSupplierDate = data.dataDetail.DeliveryDate;
                    me.data.BinningDate = me.currentDate;
                    me.data.ReferenceNo = data.dataDetail.InvoiceNo;
                    me.data.ReferenceDate = data.dataDetail.DeliveryDate;
                    me.LoadGridDetail(data.data);
                }
                else {
                    if (data.message != "") {
                        if (data.priError = "1") {
                            MsgConfirm(data.message + "\r \n Apakah anda ingin system meng-update harga beli yang berbeda secara otomatis ?", function (result) {
                                if (result) {
                                    $http.post('sp.api/edp/UpdateMasterItemPrice?isPPN=' + bPPN, me.dat).
                                    success(function (data, status, headers, config) {
                                        me.Cari();
                                    });
                                }
                                
                            });
                        }
                        else
                            MsgBox(data.message, MSG_WARNING);
                        $('#btnDelete, #btnSave, #btnPrintPreview').hide();

                        if (data.data != null) {
                            me.data.DNSupplierDate = data.dataDetail.DeliveryDate;
                            me.data.BinningDate = me.currentDate;
                            me.data.ReferenceNo = data.dataDetail.InvoiceNo;
                            me.data.ReferenceDate = data.dataDetail.DeliveryDate;
                            me.LoadGridDetail(data.data);

                            
                        }
                    }
                }

            }).error(function (data, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "browse",
            title: "Binning Lookup",
            manager: spPenerimaanManager,
            query: new breeze.EntityQuery.from("EdpBrowse").withParameters({ "RecType": me.pilihan }),
            defaultSort: "BinningNo desc",
            columns: [
            { field: "BinningNo", title: "No. Binning List" },
            { field: "BinningDate", title: "Tgl. Binning", type: "date", format: "{0:dd-MMM-yyyy}" },
            { field: "ReferenceNo", title: "No. Referensi" },
            { field: "DNSupplierNo", title: "No. DN" },
            ]
        });
        lookup.dblClick(function (data) {
            console.log(data);
            if (data != null) {
                me.isInProcess = true;
                me.data = data;
                me.detail1 = data;
                me.data.TransDate = data.DNSupplierDate;
                me.detail = data;
                me.st = data;
                me.data.IsExt = data.isLocked;
                me.isBrowse = true;
                me.detail4.DocNo = "";
                me.detail5.PartNo = "";
                me.detail3.PurchasePrice = 0;
                me.detail3.DiscPct = 0;
                me.detail3.ReceivedQty = 0;
                me.detail3.BoxNo = "";
                me.LoadEdpDetail(data.BinningNo, data.SupplierCode);
                me.Apply();
            }
        });
    }

    me.LoadEdpDetail = function (BinningNo, SupplierCode) {
        var src = "sp.api/Edp/loadData?BinningNo=" + BinningNo + "&SupplierCode=" + SupplierCode;
        $http.post(src)
            .success(function (data, status, headers, config) {
                me.data.SupplierName = data.Supp.SupplierName;
                me.detail.ReceivedQty = data.QtyTot.ReceivedQty;
                me.st.Stat = data.status;
                if (me.pilihan == 3) {
                    me.DetailTransferStock();
                }
                else {
                    me.LoadGridDetail(data.DatGrid);
                }
                me.ReformatNumber();
                me.readyToModified();
            }
         );
    }

    me.LoadPelanggan = function (CustomerCode, CustomerName) {
        var src = "sp.api/Edp/LoadPelanggan?CustomerCode=" + CustomerCode;
        $http.post(src)
            .success(function (data, status, headers, config) {
                if (data == "" || data == null) {
                    MsgBox("Kode Pelanggan belum ditambahkan di Master Supplier", MSG_ERROR);
                    return false
                } else {
                    me.detail.CustomerCode = CustomerCode;
                    me.detail.CustomerName = CustomerName;
                    me.Apply();
                }

            }
         );
    }

    me.Trans = [
        { value: '0', text: 'TRANSFER STOCK' },
        { value: '1', text: 'PEMINJAMAN' },
        { value: '2', text: 'PENGEMBALIAN' },
        { value: '3', text: 'LAIN-LAIN' },
        { value: '4', text: 'PEMBELIAN' },
        { value: '5', text: 'INTERNAL' },
    ];

    me.SupplierCodeBrowse = function () {
        console.log(me.detail.TransType);
        var transType = me.detail.TransType;
        if (transType === "5" && me.data.DNSupplierNo === "")
            return;

        var columns = [
                { field: "SupplierCode", title: "Kode Supplier" },
                { field: "SupplierName", title: "Nama Supplier" },
                { field: "Alamat", title: "Alamat" },
                { field: "Diskon", title: "Diskon(%)" },
                { field: "ProfitCenterCodeStr", title: "Profit Center" }
        ];
        var url = "sp.api/edp/LookupSupplier";

        if(transType === "5"){
            url = "sp.api/edp/LookupSupplierInternal";
            columns = [
                { field: "SupplierCode", title: "Kode Supplier" },
                { field: "SupplierName", title: "Nama Supplier" }
            ];
        }

        var lookup = Wx.klookup({
            name: "lookupSupplier",
            title: "Supplier Lookup",
            url: url,
            serverBinding: true,
            pageSize: 10,
            columns: columns
        });
        lookup.dblClick(function (data) {
            if (data != null) {

                me.data.SupplierName = data.SupplierName;
                me.data.SupplierCode = data.SupplierCode;
                me.Apply();
            }
        });
    }

    me.CustomerCodeBrowse = function () {

        var lookup = Wx.blookup({
            name: "btnCustomerCode",
            title: "Customer Lookup",
            manager: spPenerimaanManager,
            query: "EdpPelangganBrowse",
            defaultSort: "CustomerCode asc",
            columns: [
            { field: "CustomerCode", title: "Customer Code" },
            { field: "CustomerName", title: "Customer Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.LoadPelanggan(data.CustomerCode, data.CustomerName);
            }
        });
    }

    me.DNSupplierNoBrowse = function () {

        var lookup = Wx.blookup({
            name: "btnDNSupplierNo",
            title: "No DN Lookup",
            manager: spPenerimaanManager,
            query: "EdpDnNoBrowse",
            defaultSort: "DeliveryNo asc",
            columns: [
            { field: "DeliveryNo", title: "No. DN" },
            { field: "SupplierCode", title: "Supplier Code" },
            { field: "SupplierName", title: "Supplier Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DNSupplierNo = data.DeliveryNo;
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.Apply();
                $http.post('sp.api/edp/SelectDtlUtlPINVDtl', me.data).success
                (function (data, status, headers, config) {
                    if (data.success) {
                        me.data.ReferenceNo = data.data.InvoiceNo;
                        me.data.ReferenceDate = data.data.InvoiceDate;
                        me.data.DNSupplierDate = data.data.DeliveryDate;
                        me.detail.TotItem = data.data.TotItem;
                        me.detail.ReceivedQty = number_format(data.data.TotQty, 2);
                        me.detail.TotBinningAmt = number_format(data.data.TotAmount, 2);
                        me.ReformatNumber();
                    }
                });
            }
        });
    }

    me.TransNoBrowse = function () {
        var lookup = Wx.blookup({
            name: "btnDNSupplierNo",
            title: "Trans No Lookup",
            manager: spPenerimaanManager,
            //query: "EdpTransNoBrowse",
            query: new breeze.EntityQuery.from("EdpTransNoBrowse").withParameters({ "isTrex": me.data.IsExt }),
            defaultSort: "LampiranNo asc",
            columns: [
                { field: "LampiranNo", title: "No. DN" },
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "SupplierName", title: "Supplier Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail1.DNSupplierNo = data.LampiranNo;
                me.detail1.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.DetailTransferStock();
                me.Apply();
            }
        });
    }

    me.DetailTransferStock = function () {
        $http.post('sp.api/Edp/Detail_TransferStock?LampiranNo=' + me.detail1.DNSupplierNo + '&DealerCode=' + me.detail1.SupplierCode).
        success(function (isi, status, headers, config) {
            if (isi.success) {
                me.LoadGridDetail(isi.data)
                //me.readyToModified();
            } else {
                MsgBox(isi.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            // console.log(data);
            MsgBox(data.message, MSG_ERROR);
        });
    }

    me.DocumentNoBrowse = function () {
        if (me.data.SupplierCode == "" || me.data.SupplierCode == null) {
            MsgBox("Pemasok tidak boleh kosong", MSG_ERROR);
            return false;
        }
        var lookup = Wx.blookup({
            name: "btnDNSupplierNo",
            title: "POS No Lookup",
            manager: spPenerimaanManager,
            query: new breeze.EntityQuery.from("EdpDocNoBrowse").withParameters({ "SupplierCode": me.data.SupplierCode }),
            defaultSort: "POSNo desc",
            columns: [
            { field: "POSNo", title: "No. POS" },
            { field: "PosDate", title: "Tgl. POS", type: "date", format: "{0:dd-MMM-yyyy}" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail4.DocNo = data.POSNo;
                me.Apply();
            }
        });
    }

    me.BPSNoBrowse = function () {
        if (me.detail.CustomerCode == "" || me.detail.CustomerCode == null) {
            MsgBox("Customer Code tidak boleh kosong", MSG_ERROR);
            return false;
        }
        var lookup = Wx.blookup({
            name: "btnBPSFNo",
            title: "BPS No Lookup",
            manager: spPenerimaanManager,
            query: new breeze.EntityQuery.from("EdpBpsNoBrowse").withParameters({ "CustomerCode": me.detail.CustomerCode }),
            defaultSort: "BPSFNo asc",
            columns: [
            { field: "BPSFNo", title: "No. BPS" },
            { field: "BPSFDate", title: "Tgl. BPS", type: "date", format: "{0:dd-MMM-yyyy}" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail3.BPSFNo = data.BPSFNo;
                me.Apply();
            }
        });
    }

    me.PartNoBrowse = function () {
        if (me.detail.TransType == "2") {
            if (me.detail.CustomerCode == "" || me.detail.CustomerCode == null) {
                MsgBox("Customer Code tidak boleh kosong", MSG_ERROR);
                return false;
            }
            if (me.detail3.BPSFNo == "" || me.detail3.BPSFNo == null) {
                MsgBox("BPSFNo tidak boleh kosong", MSG_ERROR);
                return false;
            }
            var lookup = Wx.blookup({
                name: "btnPartNo",
                title: "Part No Lookup",
                manager: spPenerimaanManager,
                query: new breeze.EntityQuery.from("EdpPartNoBrowse").withParameters({ "CustomerCode": me.detail.CustomerCode, "BPSFNo": me.detail3.BPSFNo }),
                defaultSort: "PartNo asc",
                columns: [
                { field: "PartNo", title: "No. BPS" },
                { field: "QtyBill", title: "Maks. Qty" },
                { field: "CostPrice", title: "Harga" },
                { field: "PartName", title: "Nama Part" },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.detail5.PartNo = data.PartNo;
                    me.detail3.PurchasePrice = data.CostPrice;
                    me.detail3.ReceivedQty = data.QtyBill;
                    me.Apply();
                }
            });
        } else if (me.detail.TransType == "4") {
            if (me.detail4.DocNo == "" || me.detail4.DocNo == null) {
                MsgBox("POSNo tidak boleh kosong", MSG_ERROR);
                return false;
            }
            console.log(bPPN);
            var lookup = Wx.blookup({
                name: "btnPartNo",
                title: "Part No Lookup",
                manager: spPenerimaanManager,
                query: new breeze.EntityQuery.from("EdpPartNo_PembelianBrowse").withParameters({ "DocNo": me.detail4.DocNo, "bPPN" : bPPN }),
                defaultSort: "PartNo asc",
                columns: [
                { field: "PartNo", title: "No. Part" },
                { field: "MaxReceived", title: "Maks. Qty" },
                { field: "ReminQty", title: "Sisa Qty" },
                { field: "PurchasePrice", title: "Harga", format: "{0:n0}" },
                { field: "PartName", title: "Nama Part" },
                { field: "DiscPct", title: "Diskon" },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    //Check Binning Detail Existing Partno 
                    var param = {
                        BinningNo: me.data.BinningNo, 
                        PartNo:  data.PartNo,
                        DocumentNo: me.detail4.DocNo,
                    }
                    $http.post('sp.api/Edp/GetBinnDtlBpsByPartNo', param)
                    .success(function (result) {
                        if (result.success) {
                            me.detail5.PartNo = result.data.PartNo;
                            me.detail3.PurchasePrice = result.data.PurchasePrice;
                            me.detail3.DiscPct = result.data.DiscPct;
                            me.detail3.ReceivedQty = result.data.ReceivedQty;
                            me.detail3.BoxNo = result.data.BoxNo;
                        }
                        else {
                            me.detail5.PartNo = data.PartNo;
                            me.detail3.PurchasePrice = data.PurchasePrice;
                            me.detail3.DiscPct = data.DiscPct;
                            me.detail3.ReceivedQty = data.ReminQty; //data.MaxReceived;
                            //me.Apply();
                        }
                    }).
                    error(function (data, status, headers, config) {
                        MsgBox("Terjadi Kesalahan, Hubungi SDMS Support", MSG_ERROR);
                    });
                }
            });
        } else if (me.detail.TransType == "5") {
            var lookup = Wx.blookup({
                name: "btnPartNo",
                title: "Part No Lookup",
                manager: spPenerimaanManager,
                query: "EdpPartNo_InternalBrowse",
                defaultSort: "PartNo asc",
                columns: [
                { field: "PartNo", title: "No. BPS" },
                { field: "PartName", title: "Nama Part" },
                { field: "Available", title: "Avail Qty." },
                { field: "MovingCode", title: "Moving Code" },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.detail2.PartNo = data.PartNo;
                    me.detail2.ReceivedQty = data.Available;
                    me.Apply();
                }
            });
        } else {
            if (me.detail1.SupplierCode == "" || me.detail1.SupplierCode == null) {
                MsgBox("SupplierCode tidak boleh kosong", MSG_ERROR);
                return false;
            }
            var lookup = Wx.blookup({
                name: "btnPartNo",
                title: "Part No Lookup",
                manager: spPenerimaanManager,
                query: new breeze.EntityQuery.from("EdpPartNo_OthersBrowse").withParameters({ "SupplierCode": me.detail1.SupplierCode }),
                defaultSort: "PartNo asc",
                columns: [
                { field: "PartNo", title: "No. BPS" },
                { field: "PartName", title: "Nama Part" },
                { field: "Available", title: "Avail Qty." },
                { field: "MovingCode", title: "Moving Code" },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.detail2.PartNo = data.PartNo;
                    me.detail2.ReceivedQty = data.Available;
                    me.Apply();
                }
            });
        }
    }
    
    me.saveData = function (e, param) {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        angular.extend(me.savemodel, me.detail1);
       
        var param = {
            model: me.savemodel,
            pil: me.pilihan,
            CustomerCode: me.detail.TransType == "2" ? me.detail.CustomerCode : "",
            isPPN: bPPN,
            IsExt: me.data.IsExt
        }
        
        $http.post('sp.api/Edp/save', param).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                me.data.BinningNo = data.isi.BinningNo;
                me.detail.TotItem = data.isi.TotItem;
                me.detail.TotBinningAmt = data.isi.TotBinningAmt;
                me.LoadEdpDetail(data.isi.BinningNo, data.isi.SupplierCode);
                me.readyToModified();
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            console.log(data);
            MsgBox("Terjadi Kesalahan, Hubungi SDMS Support", MSG_ERROR);
        });
    }

    me.SaveDetail = function (action) {
        if (me.detail3.BoxNo == null) {
            MsgBox("Box No Harus diisi", MSG_ERROR);
            return false;
        }

        if (me.detail3.ReceivedQty == null) {
            MsgBox("Received Qty Harus diisi", MSG_ERROR);
            return false;
        }
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        angular.extend(me.savemodel, me.detail1);
        angular.extend(me.savemodel, me.detail2);
        angular.extend(me.savemodel, me.detail3);
        angular.extend(me.savemodel, me.detail4);
        angular.extend(me.savemodel, me.detail5);
        //  console.log(me.savemodel); return false;
        var src = "sp.api/edp/SaveRecordDetail?pil=" + me.pilihan;
        $http.post(src, me.savemodel)
            .success(function (data, status, headers, config) {

                if (data.success) {
                    var i = 0;
                    Wx.Success("Data saved...");
                    if (me.detail.TotItem == 0) {
                        i = 0;
                    } else {
                        i = parseInt(me.detail.TotItem - 1);
                    }
                    // var i = (me.detail.TotItem = 0) ? me.detail.TotItem : me.detail.TotItem - 1;
                    //                    var i = me.detail.TotItem;

                    me.detail5.PartNo = "";
                    me.detail3.PurchasePrice = 0;
                    me.detail3.DiscPct = 0;
                    me.detail3.ReceivedQty = 0;
                    if (action == 'update') {
                        me.detail4.DocNo = "";
                        me.detail3.BoxNo = "";
                        me.CloseModel();
                    }

                    me.detail.ReceivedQty = data.tqty;
                    //me.detail.ReceivedQty = me.detail.ReceivedQty + data.dat[i].ReceivedQty;
                    me.detail.TotItem = data.totitem;
                    me.detail.TotBinningAmt = data.bintot;
                    me.LoadGridDetail(data.dat);
                    me.readyToModified();

                    setTimeout(function () {
                        me.isPrintAvailable = true;
                    }, 200);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.printPreview = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        angular.extend(me.savemodel, me.detail1);
        $http.post('sp.api/edp/Print', { model: me.savemodel, type: me.pilihan, LampiranNo: me.detail1.DNSupplierNo, DealerCode: me.detail1.SupplierCode }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    if (data.datdet == null || data.datdet == "") {
                        MsgBox("Document tidak dapat dicetak karena tidak memiliki data detail", MSG_ERROR);
                        return false;
                    } else {
                        me.st.Status = data.status;
                        me.readyToModified();
                        var data = me.data.BinningNo + "," + me.data.BinningNo + "," + "300" + "," + "typeofgoods";
                        var rparam = "admin";

                        Wx.showPdfReport({
                            id: "SpRpTrn003A",
                            pparam: data,
                            rparam: rparam,
                            textprint:true,
                            type: "devex"
                        });

                    }

                    me.readyToModified();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.AfterDeleteDtl = function (a, b) {
        var src = "sp.api/Edp/loadData?BinningNo=" + a + "&SupplierCode=" + b;
        $http.post(src)
            .success(function (data, status, headers, config) {

                me.detail.ReceivedQty = data.QtyTot.ReceivedQty;
                me.LoadGridDetail(data.DatGrid);
                me.ReformatNumber();
                me.readyToModified();


            }
         );
    }

    me.delete = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        angular.extend(me.savemodel, me.detail1);
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/Edp/delete?pil=' + me.pilihan, me.savemodel).
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

    me.DeleteDetail = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        angular.extend(me.savemodel, me.detail1);
        angular.extend(me.savemodel, me.detail2);
        angular.extend(me.savemodel, me.detail3);
        angular.extend(me.savemodel, me.detail4);
        angular.extend(me.savemodel, me.detail5);
        MsgConfirm("Are you sure to delete current record?", function (result) {
            //  console.log(me.savemodel); return false;
            if (result) {
                $http.post('sp.api/Edp/deleteDetail', me.savemodel).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        Wx.Info("Data has been deleted...");
                        setTimeout(function () {
                            me.detail.TotItem = dl.totitem;
                            me.detail.TotBinningAmt = dl.bintot;
                            me.AfterDeleteDtl(me.data.BinningNo, me.data.SupplierCode);
                            me.readyToModified();
                            me.CloseModel();
                        }, 1000);
                        me.readyToModified();
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


    me.VerifyBinning = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        MsgConfirm("Are you sure to close?", function (result) {
            if (result) {
                $http.post('sp.api/edp/VerifyBinning', me.savemodel).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        //me.init();
                        //MsgBox(dl.message);
                        MsgConfirm(dl.message, function (res) {
                            if (res) {
                                me.CloseBinning();
                            }
                        });
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

    me.CloseBinning = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        $http.post('sp.api/edp/CloseBinning', me.savemodel).
        success(function (dl, status, headers, config) {
            if (dl.success) {
                //me.init();
                me.st.Status = dl.status;
                MsgBox(dl.message);
            } else {
                MsgBox(dl.message, MSG_ERROR);
            }
        }).
        error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.LoadGridDetail = function (data) {
        me.grid.detail = data;
        if (me.pilihan == "3") {
            me.loadTableData(me.grid1, me.grid.detail);
            //me.readyToModified();
        } else {
            me.loadTableData(me.grid2, me.grid.detail);
            me.readyToModified();
            if (me.pilihan == "2") $('#btnDocNo, #btnPartNo').attr('disabled', true);
        }
    }

    me.$watch("pilihan", function (a, b) {
        if (a != b) {
            if (a == "1") {
                me.detail.TransType = "4";
            } else if (a == "2") {
                me.detail.TransType = "4";
            } else if (a == "3") {
                me.detail.TransType = "0";
            }
        }
    })

    me.grid1 = new webix.ui({
        container: "tabelPertama",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            //{ id: "NoUrut", header: "No", fillspace: true },
            { id: "PartNo", header: "Part No", fillspace: true },
            { id: "PurchasePrice", header: "Harga Beli", fillspace: true },
            { id: "DiscPct", header: "Diskon(%)", fillspace: true },
            { id: "ReceivedQty", header: "Jumlah Terima", fillspace: true },
            { id: "BoxNo", header: "No. Box", fillspace: true },
            { id: "NmPart", header: "Nama Part", width: 300 }
        ],
        //on: {
        //    onSelectChange: function () {
        //        if (me.grid1.getSelectedId() !== undefined) {
        //            me.detail2 = this.getItem(me.grid1.getSelectedId().id);
        //            me.detail2.old = me.grid1.getSelectedId();
        //            me.Apply();
        //        }
        //    }
        //}
    });

    me.grid2 = new webix.ui({
        container: "detailData2",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "DocNo", header: "No Dokumen", fillspace: true },
            { id: "PartNo", header: "No Part", fillspace: true },
            { id: "PurchasePrice", header: "Harga", fillspace: true },
            { id: "DiscPct", header: "Diskon", fillspace: true },
            { id: "ReceivedQty", header: "Qty Received", fillspace: true },
            { id: "BoxNo", header: "No Box", fillspace: true },
            { id: "NmPart", header: "Nama Part", width: 300 }
        ],
        on: {
            onSelectChange: function () {
                if (me.pilihan == 2) return;
                if (me.grid2.getSelectedId() !== undefined) {
                    me.detail3 = this.getItem(me.grid2.getSelectedId().id);
                    me.detail4 = this.getItem(me.grid2.getSelectedId().id);
                    me.detail5 = this.getItem(me.grid2.getSelectedId().id);

                    me.detail3.old = me.grid2.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.CloseModel = function () {
        if (me.pilihan == "3") {
            me.detail2 = {};
            me.grid1.clearSelection();
        } else {
            me.detail3 = {};
            me.detail4 = {};
            me.detail5 = {};
            me.grid2.clearSelection();
        }
    }

    me.initialize = function () {
        me.grid = {};
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);
        me.detail = {};
        me.st = {};
        me.detail1 = {};
        me.detail2 = {};
        me.detail3 = {};
        me.detail4 = {};
        me.detail5 = {};
        //me.data.BinningNo = "BNL/XX/YYYYYY";
  
        me.pilihan = "1";
        me.isPrintAvailable = true;
        me.data.IsPPN = true;
        me.data.IsExt = false;
        bPPN = false;
        $('#btnDocNo, #btnPartNo').attr('disabled', false);
        $scope.renderGrid();
        me.isBrowse = false;
        $("#btnCari").css({
            "visibility": "hidden"
        });

        $http.post('sp.api/edp/default', me.data).success(function (data) {
            if (data.PPNVisible == false) {
                $("#IsPPN, #lblIsPPN").css({
                    "visibility": "hidden"
                });
                bPPN = 0;
            }
            else {
                $("#IsPPN, #lblIsPPN").css({
                    "visibility": "visible"
                });
                bPPN = 1;
            }

            me.currentDate = data.currentDate;

            me.data.HPPDate = me.currentDate;
            me.data.WRSDate = me.currentDate;
            me.data.BinningDate = me.currentDate;
            me.data.TaxDate = me.currentDate;
            me.data.ReferenceDate = me.currentDate;
            me.data.DNSupplierDate = me.currentDate;
            me.data.TransDate = me.currentDate;
        });

        $("#IsPPN, #lblIsPPN").css({
            "visibility": "hidden"
        });
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
        me.grid2.adjust();
    })

    me.$watch("detail.TransType", function (a, b) {
        if (a != b) {
            setTimeout(function () {
                me.grid1.adjust();
                me.grid2.adjust();
            }, 200);
        }
    })

    me.$watch("pilihan", function (a, b) {
        if (a == 2) {
            $("#btnCari").css({
                "visibility": "visible"
            });
        }
        else {
            $("#btnCari").css({
                "visibility": "hidden"
            });
        }

        $http.post('sp.api/edp/default', me.data).success(function (data) {
            if (data.bExt) {
                if (a == 3) {
                    $("#IsExt, #lblExt").css({
                        "visibility": "visible"
                    });
                }
                else {
                    $("#IsExt, #lblExt").css({
                        "visibility": "hidden"
                    });
                }
            }
            else {
                $("#IsExt, #lblExt").css({
                    "visibility": "hidden"
                });
            }
        });
    })

    $scope.renderGrid = function () {
        setTimeout(function () {
            me.grid1.adjust();
            me.grid2.adjust();
        }, 200);

    }

    me.$watch('data', function (nVal, oVal) {
        if (nVal != oVal) {
            me.isLoadData = false;
            me.hasChanged = true;
            me.isSave = true;
            me.hasChanged = true;
            me.isInitialize = false;
        }

        if (me.st != undefined) {
            if (me.st.Status != '2') {
                me.isPrintAvailable = true;
                me.isLoadData = true;
                me.hasChanged = true;
                me.isSave = true;
                me.isInitialize = false;
            }
        }
    }, true);

    me.$watch('st.Status', function (nVal, oVal) {
        console.log(nVal);
        if (nVal == '2') {
            me.isInitialize = true;
            me.isLoadData = false;
            me.isPrintAvailable = false;
        }
    }, true);

    me.$watch('detail3', function (nVal, oVal) {
        if (me.st != undefined) {
            if (me.st.Status == '2') {
                me.isPrintAvailable = false;
                me.isSave = false;
                me.isInitialize = true;
                me.isLoadData = false;
                me.hasChanged = false;
            }
            else {
                me.isLoadData = true;
                me.hasChanged = false;
                me.isSave = false;
                me.isPrintAvailable = true;
            }
        }
    }, true);

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Entry Draft Penerimaan (Binning)",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                   //{ name: "opt", cls: "" },
                    { name: "Status", model: "st.Status", show: "detail1.e == 0" },
                    { name: "Stat", model: "st.Stat", show: "detail1.e == 0" },
                        {
                            type: "buttons",
                            items: [
                                    { name: "btnCloseHpp1", text: "CLOSE", icon: "icon icon-process", cls: "btn btn-warning", click: "VerifyBinning()", show: "st.Stat == false && st.Status != undefined", disable: "st.Status == 2" },
                                    { name: "btnCloseHpp1", text: "Proses WRS", icon: "icon icon-remove", cls: "btn btn-warning", click: "VerifyBinning()", show: "st.Stat == true && st.Status != undefined", disable: "st.Status == 0" },
                                    { name: "btnCloseHpp2", text: "OPEN", cls: "btn btn-success", show: "st.Status == 0", disable: "st.Status == 0" },
                                    { name: "btnCloseHpp3", text: "PRINTED", cls: "btn btn-info", show: "st.Status == 1", disable: "st.Status == 1" },
                                    { name: "btnCloseHpp4", text: "CLOSED/POSTED", cls: "btn btn-danger", show: "st.Status == 2", disable: "st.Status == 2" }
                            ]
                        }, { type: "hr", show: "st.Status != undefined || data.Stat != undefined" },
                        {
                            type: "optionbuttons",
                            name: "opsitest",
                            text: "",
                            model: "pilihan",
                            items: [
                                    { name: "1", text: "Pembelian/BPS", cls: "btn-default", click: "renderGrid()", disable: "data.SupplierCode != undefined" },
                                    { name: "2", text: "Melalui PINV", cls: "btn-default", click: "renderGrid()", disable: "data.SupplierCode != undefined" },
                                    { name: "3", text: "Trans Stock", cls: "btn-default", click: "renderGrid()", disable: "data.SupplierCode != undefined" },
                            ]
                        },
                        {
                            type: "controls",
                            text: "No. BN",
                            cls: "span8 full",
                            items: [
                                { name: "BinningNo", text: "No. BN", placeHolder: "BNL/XX/YYYY", cls: "span2", readonly: true },
                                { type: "label", text: "No. DN", cls: "span1", style: "width: 90px; text-align: right;", show: "pilihan != 3" },
                                { name: "DNSupplierNo", text: "No. DN", cls: "span2", show: "pilihan == 1", validasi: "required" },
                                { name: "DNSupplierNo", text: "No. DN", cls: "span2", type: "popup", btnName: "btnDNSupplierNo", click: "DNSupplierNoBrowse()", validasi: "required", show: "pilihan == 2", readonly: true },
                                { type: "label", text: "No. Transfer", cls: "span1", style: "width: 90px; text-align: right;", show: "pilihan == 3" },
                                { name: "DNSupplierNo", text: "No. Transfer", cls: "span2", type: "popup", btnName: "btnDNSupplierNo", model: "detail1.DNSupplierNo", click: "TransNoBrowse()", validasi: "required", show: "pilihan == 3", readonly: true },
                                { name: "IsExt", cls: "span1", type: "ng-check" , disable:"isBrowse==true" },
                                { type: "label", text: "Ext", cls: "span1", name: "lblExt" },
                                { type: "label", text: "Tgl. DN", cls: "span1", style: "width: 70px; text-align: right;", show: "pilihan != 3" },
                                { name: "DNSupplierDate", cls: "span1 right", type: "ng-datepicker", show: "pilihan == 2 || pilihan == 1", style: "width: 186px;" },
                                { type: "label", text: "Tgl. Transfer ", cls: "span1", style: "width: 100px; text-align: right;", show: "pilihan == 3" },
                                { name: "TransDate", cls: "span1 right", type: "ng-datepicker", show: "pilihan == 3", style: "width: 186px;" },
                            ]
                        },
                        {
                            type: "controls",
                            text: "Tgl. BN",
                            cls: "span8",
                            items: [
                                { name: "BinningDate", type: "ng-datepicker", cls: "span2" },
                                { type: "label", text: "Tipe Transaksi", cls: "span1", style: "width: 90px; text-align: right;" },
                                { name: "TransType", text: "Trans Type", cls: "span2", type: "select2", model: "detail.TransType", disable: "pilihan == 2 || pilihan == 3", datasource: "Trans" },
                                { name: "IsPPN", cls: "span1", type: "check" },
                                { type: "label", text: "PPN", cls: "span1", name: "lblIsPPN" },
                                { type: "label", text: "Total Item", cls: "span1", style: "width: 70px; text-align: right;" },
                                { name: "TotItem", text: "Total Item", model: "detail.TotItem", cls: "span1 right number-int", readonly: true, style: "width: 186px;" },
                            ]
                        },
                        {
                            type: "controls",
                            text: "No. Reff.",
                            cls: "span8 full",
                            items: [
                                { name: "ReferenceNo", text: "No. Referensi", cls: "span2", show: "pilihan == 1", validasi: "required", required: true },
                                { name: "ReferenceNo", text: "No. Referensi", cls: "span2", show: "pilihan != 1", disable: true },
                                { type: "label", text: "Pemasok", cls: "span1", style: "width: 90px; text-align: right;" },
                                { name: "SupplierCode", text: "Supplier Code", cls: "span2  ", model: "detail1.SupplierCode", show: "pilihan == 3", readonly: true },
                                { name: "SupplierCode", text: "Pemasok", cls: "span2", type: "popup", btnName: "btnSupplierCode", click: "SupplierCodeBrowse()", validasi: "required", show: "detail.TransType != 2 && pilihan != 3", readonly: true },
                                {
                                    type: "buttons", cls: "span1", items: [
                                        { name: "btnCari", text: " Cari", cls: "btn btn-warning", icon: "icon-search", click: "Cari()", style: "width: 50px; height: 32px; padding-top: 5px;" }
                                    ]
                                },
                                { type: "label", text: "Total Qty", cls: "span1", style: "width: 59px; text-align: right;" },
                                { name: "ReceivedQty", text: "Total QTY", model: "detail.ReceivedQty", cls: "span1 right number-int", readonly: true, style: "width: 186px;" }
                            ]
                        },
                        {
                            type: "controls",
                            text: "Tgl. Reff.",
                            cls: "span8 full",
                            items: [
                                { name: "ReferenceDate", text: "", cls: "span2", type: "ng-datepicker" },
                                { type: "label", text: "Nm. Pemasok", cls: "span1", style: "width: 90px; text-align: right;" },
                                { name: "SupplierName", text: "Supplier Name", cls: "span2", show: "detail.TransType != 2", readonly: true },
                                { type: "label", text: "Total BN Amount", cls: "span1", style: "width: 120px; text-align: right;" },
                                { name: "TotBinningAmt", text: "Total Binning", model: "detail.TotBinningAmt", cls: "span1 right number-int", readonly: true, style: "width: 186px;" },
                            ]
                        },
                        { type: "hr", show: "detail.TransType == 2" },
                        {
                            type: "controls",
                            text: "Kode Pelanggan",
                            cls: "span8 full",
                            show: "detail.TransType == 2",
                            items: [
                                {
                                    name: "CustomerCode",
                                    text: "Kode Pelanggan",
                                    cls: "span3",
                                    model: "detail.CustomerCode",
                                    type: "popup",
                                    btnName: "btnCustomerCode",
                                    click: "CustomerCodeBrowse()",
                                    validasi: "required",
                                    readonly: true
                                },
                                {
                                    name: "CustomerName",
                                    text: "Nama Pelanggan",
                                    model: "detail.CustomerName",
                                    cls: "span5 right",
                                    readonly: true
                                },
                            ]
                        },
                ]
            },
            {
                name: "pnlB",
                title: "Tabel Detail",
                show: "detail.TransType == 0 || detail.TransType == 1 || detail.TransType == 3 || detail.TransType == 5 && detail.ReceivedQty != undefined",
                items: [
                        {
                            name: "PartNo",
                            text: "Part No",
                            cls: "span4",
                            type: "popup",
                            model: "detail2.PartNo",
                            btnName: "btnPartNo",
                            click: "PartNoBrowse()",
                            disable: "pilihan == 3",
                            readonly: true
                        },
                        {
                            name: "PurchasePrice",
                            text: "Harga",
                            cls: "span4 number-int",
                            model: "detail2.PurchasePrice",
                            disable: "pilihan == 3",
                            readonly: true
                        },
                        {
                            name: "DiscPct",
                            text: "Diskon",
                            cls: "span2 number",
                            model: "detail2.DiscPct",
                            disable: "pilihan == 3",
                            readonly: true
                        },
                        {
                            name: "ReceivedQty",
                            model: "detail2.ReceivedQty",
                            text: "Qty Receive",
                            disable: "pilihan == 3",
                            cls: "span2 number-int",
                        },
                        {
                            name: "BoxNo",
                            text: "NoBox",
                            disable: "pilihan == 3",
                            cls: "span4",
                            model: "detail2.BoxNo",
                        },
                        //{
                        //    type: "buttons",
                        //    items: [
                        //            { name: "btnAddModel", text: "Add Detail", icon: "icon-plus", cls: "btn btn-info", click: "SaveDetail()", show: "detail2.old === undefined", disable: "detail2.PartNo == undefined" },
                        //            { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "SaveDetail()", show: "detail2.old !== undefined" },
                        //            { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteDetail()", show: "detail2.old !== undefined" },
                        //            { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail2.old !== undefined" }
                        //    ]
                        //},
                        {
                            title: "Tabel",
                            name: "tabelPertama",
                            type: "wxdiv",
                        },
                ]
            },
            {
                name: "pnlC",
                title: "Tabel Detail",
                show: "detail.TransType == 2 || detail.TransType == 4",
                items: [
                        {
                            name: "BPSFNo",
                            text: "No BPS",
                            cls: "span4",
                            type: "popup",
                            model: "detail3.BPSFNo",
                            btnName: "btnBPSFNo",
                            show: "detail.TransType == 2",
                            click: "BPSNoBrowse()",
                            readonly: true
                        },
                        {
                            name: "DocNo",
                            text: "No POS",
                            cls: "span4",
                            type: "popup",
                            model: "detail4.DocNo",
                            show: "detail.TransType == 4",
                            disable: "detail3.old != undefined",
                            btnName: "btnPOSNo",
                            click: "DocumentNoBrowse()",
                            readonly: true
                        },
                        {
                            name: "PartNo",
                            text: "Part No",
                            cls: "span4",
                            type: "popup",
                            model: "detail5.PartNo",
                            btnName: "btnPartNo",
                            disable: "detail3.old != undefined",
                            click: "PartNoBrowse()",
                            readonly: true
                        },
                        {
                            name: "PurchasePrice",
                            text: "Harga",
                            cls: "span2 number-int",
                            model: "detail3.PurchasePrice",
                            readonly: true
                        },
                        {
                            name: "DiscPct",
                            text: "Diskon",
                            cls: "span2 number",
                            model: "detail3.DiscPct ",
                            readonly: false
                        },
                        {
                            name: "ReceivedQty",
                            model: "detail3.ReceivedQty",
                            text: "Qty. Received",
                            cls: "span2 number-int",
                        },
                        {
                            name: "BoxNo",
                            text: "No Box",
                            cls: "span2",
                            model: "detail3.BoxNo",
                            disable: "detail3.old != undefined"
                        },
                        {
                            type: "buttons",
                            items: [
                                    { name: "btnAddModel", text: "Add Detail", icon: "icon-plus", cls: "btn btn-info", click: "SaveDetail(\"add\")", show: "detail3.old === undefined", disable: "(detail5.PartNo == undefined || detail5.PartNo == \"\") || st.Status == 2" },
                                    { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "SaveDetail(\"update\")", show: "detail3.old !== undefined", disable: "st.Status == 2" },
                                    { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteDetail()", show: "detail3.old !== undefined", disable: "st.Status == 2" },
                                    { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail3.old !== undefined", disable: "st.Status == 2" }
                            ]
                        },
                        {
                            title: "Tabel",
                            name: "detailData2",
                            type: "wxdiv"
                        },
                ]
            }



        ]
    };



    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spEDPController");
    }





});