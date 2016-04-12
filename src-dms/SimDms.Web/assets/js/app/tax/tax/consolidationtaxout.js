"use strict"

function taxConsolidationTaxOutController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('tax.api/Combo/Years').
     success(function (data, status, headers, config) {
         me.comboYear = data;
         var year = document.getElementById('PeriodYear')
         year.options[0].remove();
     });

    $http.post('tax.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    me.$watch('data.IsPKP', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            if (newValue) {
                $('#TaxNo').prop('readonly', false);
            }
            else {
                $('#TaxNo').prop('readonly', true);
                $('#TaxNo').val('');
            }
        }
    });

    me.$watch('options', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    me.default = function () {
        $http.post('sv.api/report/default').
        success(function (data, status, headers, config) {
            $('#PeriodMonth').select2('val', data.Month);
            $('#PeriodYear').select2('val', data.Year);
            me.data.PeriodMonth = data.Month;
            me.data.PeriodYear = data.Year;
        });
    }

    me.taxPeriod = function (year, month) {
        me.period.TaxPeriod = year + "/" + month;
        me.Apply();
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "TaxOutBrowse",
            title: "Pajak Keluaran Per Periode",
            manager: TaxManager,
            query: "TaxOutBrowse",
            columns: [
                { field: 'PeriodYear', title: 'Tahun Pajak' },
                { field: 'PeriodMonth', title: 'Bulan Pajak' },
            ]
        });
        lookup.dblClick(function (data) {

            me.loadDetail(data);
        });
    }

    me.loadDetail = function (data) {
        $http.post('tax.api/consolidationtaxout/gettaxout', data)
       .success(function (e) {
           me.data = e[0];
           me.loadTableData(me.gridTaxOut, e);
           me.hasChanged = false;
           me.isExsist = true;
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });

        $http.post('tax.api/consolidationtaxout/getgrandtotal', data)
       .success(function (e) {
           me.total = e[0];
           me.hasChanged = false;
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });

        me.hasChanged = false;
        setTimeout(function () {
            me.hasChanged = false;
        }, 100);
        me.isPrintAvailable = true;
        me.taxPeriod(data.PeriodYear, data.PeriodMonth);
    };

    me.company = function () {
        var lookup = Wx.blookup({
            name: "CompanyBrowse",
            title: "Pencarian Perusahaan",
            manager: TaxManager,
            query: "CompanyBrowse",
            defaultSort: "CompanyCode asc",
            columns: [
                { field: 'CompanyCode', title: 'Kode Perusahaan' },
                { field: 'CompanyName', title: 'Nama Perusahaan' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.CompanyCode = data.CompanyCode;
            me.Apply();
        });
    }

    me.branch = function () {
        var lookup = Wx.blookup({
            name: "BranchBrowse",
            title: "Pencarian Cabang",
            manager: TaxManager,
            query: "BranchBrowse",
            defaultSort: "BranchCode asc",
            columns: [
                { field: 'BranchCode', title: 'Kode Cabang' },
                { field: 'BranchName', title: 'Nama Cabang' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BranchCode = data.BranchCode;
            me.Apply();
        });
    }

    me.consolidationLookup = function (e) {
        var title = "";
        var value = "";

        switch (e) {
            case 'typeofgoods':
                title = "Pencarian Kategori Item";
                value = "Kode Kategori Item";
                break;
            case 'taxout':
                title = "Pencarian Kode Pajak";
                value = "Kode Pajak";
                break;
            case 'transout':
                title = "Pencarian Kode Transaksi Pajak";
                value = "Kode Transaksi";
                break;
            case 'statusout':
                title = "Pencarian Kode Status Pajak";
                value = "Kode Status";
                break;
            case 'document':
                title = "Pencarian Kode Dokument Pajak";
                value = "Kode Dokument";
                break;
        }

        var lookup = Wx.blookup({
            name: "ConsolidationLookup",
            title: title,
            manager: TaxManager,
            query: new breeze.EntityQuery.from("ConsolidationLookup").withParameters({ type: e }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: 'LookUpValue', title: value },
                { field: 'LookUpValueName', title: 'Deskripsi' },
            ]
        });
        lookup.dblClick(function (data) {
            switch (e) {
                case 'typeofgoods':
                    me.data.TypeOfGoods = data.LookUpValue;
                    me.data.TypeOfGoodDesc = data.LookUpValueName;
                    break;
                case 'taxout':
                    me.data.TaxCode = data.LookUpValue;
                    me.data.TaxDesc = data.LookUpValueName;
                    break;
                case 'transout':
                    me.data.TransactionCode = data.LookUpValue;
                    me.data.TransactionDesc = data.LookUpValueName;
                    break;
                case 'statusout':
                    me.data.StatusCode = data.LookUpValue;
                    me.data.StatusDesc = data.LookUpValueName;
                    break;
                case 'document':
                    me.data.DocumentCode = data.LookUpValue;
                    me.data.DocumentDesc = data.LookUpValueName;
                    break;
            }
            me.Apply();
        });
    }

    me.customer = function () {
        var lookup = Wx.blookup({
            name: "CustomerBrowse",
            title: "Pencarian Pelanggan",
            manager: TaxManager,
            query: "CustomerBrowse",
            defaultSort: "CustomerCode asc",
            columns: [
                { field: 'CustomerCode', title: 'Kode Pelanggan' },
                { field: 'CustomerGovName', title: 'Nama Pelanggan' },
                { field: 'Address', title: 'Alamat', width: 300 },
                { field: 'ProfitCenter', title: 'Profit', width: 100 },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.CustomerCode = data.CustomerCode;
            me.data.CustomerName = data.CustomerGovName;
            me.Apply();
        });
    }

    me.query = function () {
        $http.post('tax.api/consolidationtaxout/query', me.data)
       .success(function (e) {
           if (e.success) {
               me.loadTableData(me.gridTaxOut, e.taxOut);
               me.total = e.grandTotal[0];
               me.taxPeriod(me.data.PeriodYear, me.data.PeriodMonth);

           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.save = function () {
        $http.post('tax.api/consolidationtaxout/save', me.data)
       .success(function (e) {
           if (e.success) {
               MsgBox(e.message);
               me.loadDetail(me.data);
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.saveDetail = function () {
        $http.post('tax.api/consolidationtaxout/validateDetail', me.data)
        .success(function (e) {
            if (e.success) {

                var npwp = me.data.IsPKP && me.data.NPWP.replace(" ", "").trim().length != 20;
                var taxNo = me.data.IsPKP && me.data.TaxNo.replace(" ", "").trim().length != 19;

                if (npwp) {
                    MsgConfirm("No. NPWP kurang atau belum terisi. Lanjut Proses?", function (result) {
                        if (result) {
                            if (taxNo) {
                                MsgConfirm("No. Seri Pajak kurang atau belum terisi. Lanjut Proses?", function (result) {
                                    if (result) {
                                        $http.post('tax.api/consolidationtaxout/saveDetail', me.data)
                                         .success(function (e) {
                                             if (e.success) {
                                                 MsgBox("Berhasil Simpan Data");
                                                 me.loadDetail(me.data);
                                             } else {
                                                 MsgBox(e.message, MSG_ERROR);
                                             }
                                         })
                                         .error(function (e) {
                                             MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                                         });
                                    }
                                    else return;
                                });
                            }
                        }
                        else return;
                    });
                }
                else if (taxNo) {
                    MsgConfirm("No. Seri Pajak kurang atau belum terisi. Lanjut Proses?", function (result) {
                        if (result) {
                            $http.post('tax.api/consolidationtaxout/saveDetail', me.data)
                             .success(function (e) {
                                 if (e.success) {
                                     MsgBox("Berhasil Simpan Data");
                                     me.loadDetail(me.data);
                                 } else {
                                     MsgBox(e.message, MSG_ERROR);
                                 }
                             })
                             .error(function (e) {
                                 MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                             });
                        }
                        else return;
                    });
                }
                else {
                    $http.post('tax.api/consolidationtaxout/saveDetail', me.data)
                             .success(function (e) {
                                 if (e.success) {
                                     MsgBox("Berhasil Simpan Data");
                                     me.loadDetail(me.data);
                                 } else {
                                     MsgBox(e.message, MSG_ERROR);
                                 }
                             })
                             .error(function (e) {
                                 MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                             });
                }
            }
            else {
                MsgBox(e.message, MSG_ERROR);
            }
        })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.deleteDetail = function () {
        MsgConfirm("Hapus Data Detail Pajak, Apakah Anda Yakin ?", function (e) {
            if (e) {
                $http.post('tax.api/consolidationtaxout/deletedetail', me.data)
                .success(function (e) {
                    if (e.success) {
                        me.loadDetail(me.data);
                        Wx.Success("Data Berhasil Dihapus!!!");
                    } else {
                        MsgBox(e.message, MSG_ERROR);
                    }
                })
                 .error(function (e) {
                     MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                 });
            }
        });
    }

    me.printPreview = function () {
        Wx.loadForm();
        Wx.showForm({ url: "tax/report/taxoutespt" });
    }

    me.initialize = function () {
        me.total = {};
        me.period = {};
        me.default();
        me.data.FPJDate = me.now();
        me.data.TaxDate = me.now();
        me.data.ReferenceDate = me.now();
        me.data.SubmissionDate = me.now();
        me.data.RePosting = false;
        me.clearTable(me.gridTaxOut);
        me.clearTable(me.gridTaxOutHist);
        me.isPrintAvailable = false;
        me.isExsist = false;
    }

    me.gridTaxOut = new webix.ui({
        container: "wxTaxOut",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 200,
        columns: [
            { id: "SeqNo", header: "No", width: 45 },
            { id: "CompanyCode", header: "Kode Perusahaan", width: 150 },
            { id: "BranchCode", header: "Kode Cabang", width: 120 },
            { id: "ProductType", header: "Tipe Produk", width: 120 },
            { id: "TypeOfGoods", header: "Kategori Item", width: 150 },
            { id: "TaxCode", header: "Kode Pajak", width: 120 },
            { id: "DocumentType", header: "Tipe Pajak", width: 100 },
            { id: "TransactionCode", header: "Kode Transaksi", width: 150 },
            { id: "StatusCode", header: "Kode Status", width: 130 },
            { id: "DocumentCode", header: "Kode Dokument", width: 140 },
            { id: "CustomerCode", header: "Kode Pelanggan", width: 140 },
            { id: "CustomerName", header: "Nama Pelanggan", width: 250 },
            { id: "IsPKP", header: "PKP", width: 100, template: "{common.checkbox()}" },
            { id: "NPWP", header: "NPWP", width: 150 },
            { id: "FPJNo", header: "No. Faktur Pajak", width: 170 },
            { id: "FPJDate", header: "Tgl. Faktur Pajak", width: 170, format: me.dateFormat },
            { id: "ReferenceNo", header: "No. Faktur Penjualan", width: 190 },
            { id: "ReferenceDate", header: "Tgl. Faktur Penjualan", width: 170, format: me.dateFormat },
            { id: "TaxNo", header: "No. Seri Pajak ", width: 150 },
            { id: "TaxDate", header: "Tgl. Pajak", width: 150, format: me.dateFormat },
            { id: "SubmissionDate", header: "Tgl. Penyerahan", width: 150, format: me.dateFormat },
            { id: "DPPAmt", header: "Nilai DPP", width: 130, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "PPNAmt", header: "Nilai PPn", width: 130, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "PPNBmAmt", header: "Nilai PPnBM", width: 130, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Description", header: "Nama Barang", width: 200 },
            { id: "Quantity", header: "Jumlah", width: 80, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridTaxOut.getSelectedId() !== undefined) {
                    me.data = this.getItem(me.gridTaxOut.getSelectedId().id);

                    me.Apply();
                }
            }
        }
    });

    me.gridTaxOutHist = new webix.ui({
        container: "wxTaxOutHist",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 200,
        columns: [
            { id: "SeqNo", header: "No.", width: 45 },
            { id: "LastupdateBy", header: "User Terakhir", width: 150 },
            { id: "LastupdateDate", header: "Tgl. Update Terakhir", format: me.dateFormat, width: 150 },
            { id: "CompanyCode", header: "Kode Perusahaan", width: 150 },
            { id: "BranchCode", header: "Kode Cabang", width: 150 },
            { id: "ProductType", header: "Tipe Produk", width: 150 },
            { id: "TypeOfGoods", header: "Kategori Item", width: 150 },
            { id: "TaxCode", header: "Kode Pajak", width: 150 },
            { id: "DocumentType", header: "Tipe Pajak", width: 150 },
            { id: "TransactionCode", header: "Kode Transaksi", width: 150 },
            { id: "StatusCode", header: "Kode Status", width: 150 },
            { id: "DocumentCode", header: "Kode Dokumen", width: 150 },
            { id: "CustomerCode", header: "Kode Pelanggan", width: 150 },
            { id: "CustomerName", header: "Nama Pelanggan", width: 150 },
            { id: "IsPKP", header: "PKP", width: 150 },
            { id: "NPWP", header: "NPWP", width: 150 },
            { id: "FPJNo", header: "No. Faktur Pajak", width: 170 },
            { id: "FPJDate", header: "Tgl. Faktur Pajak", format: me.dateFormat, width: 170 },
            { id: "ReferenceNo", header: "No. Faktur Penjualan", width: 170 },
            { id: "ReferenceDate", header: "Tgl. Faktur Penjualan", format: me.dateFormat, width: 170 },
            { id: "TaxNo", header: "No. Seri Pajak", width: 150 },
            { id: "TaxDate", header: "Tgl. Pajak", width: 150 },
            { id: "SubmissionDate", header: "Tgl. Penyerahan", format: me.dateFormat, width: 150 },
            { id: "DPPAmt", header: "Nilai DPP", format: webix.i18n.numberFormat, width: 150 },
            { id: "PPNAmt", header: "Nilai PPn", format: webix.i18n.numberFormat, width: 150 },
            { id: "PPNBmAmt", header: "Nilai PPnBM", format: webix.i18n.numberFormat, width: 150 },
            { id: "Description", header: "Nama Barang", width: 250 },
            { id: "Quantity", header: "Jumlah", format: webix.i18n.numberFormat, width: 80 },
            { id: "IsDeleted", header: "Terhapus", width: 100, template: "{common.checkbox()}" }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridTaxOutHist.getSelectedId() !== undefined) {
                    me.dtlPart = this.getItem(me.gridTaxOutHist.getSelectedId().id);

                    me.Apply();
                }
            }
        }
    });

    webix.event(window, "resize", function () {
        me.gridTaxOut.adjust();
        me.gridTaxOutHist.adjust();
    });

    me.options = "0";
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Konsolidasi Pajak Keluaran",
        xtype: "panels",
        toolbars: [
             { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!isInProcess", click: "browse()" },
             //{ name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "isLoadData && isSave", click: "save()", disable: "!isSave" },
             { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
             { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "isLoadData && isSave || isPrintAvailable", click: "cancelOrClose()" },
             { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlPeriode",
                title: "Berdasarkan Periode",
                items: [
                   { name: "PeriodMonth", cls: "span3", text: "Bulan", type: "select2", datasource: "comboMonth" },
                   { name: "PeriodYear", required: true, cls: "span2", text: "-", type: "select2", datasource: "comboYear" },
                   {
                       type: "buttons", cls: "span2", items: [
                            { name: "btnQuery", text: "Query", click: "query()", cls: "btn" },
                       ]
                   },
                   { name: "RePosting", text: "Re-Posting", cls: "span2 full", type: "x-switch" },
                ]
            },
            {
                name: "pnlTaxOut",
                title: "Pajak Masukan",
                items: [
                    { name: "CompanyCode", text: "Kode Perusahaan", cls: "span4", type: "popup", readonly: true, click: "company()" },
                    {
                        text: "Kode Pajak",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "TaxCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('taxout')" },
                            { name: "TaxDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    { name: "BranchCode", text: "Kode Cabang", cls: "span4", type: "popup", readonly: true, click: "branch()" },
                    {
                        text: "Kode Transaksi",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "TransactionCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('transout')" },
                            { name: "TransactionDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Tipe Produk",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "ProductType", cls: "span2", placeHolder: " ", readonly: true },
                            { name: "ProductDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Kode Status",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "StatusCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('statusout')" },
                            { name: "StatusDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Kategori Item",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "TypeOfGoods", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('typeofgoods')" },
                            { name: "TypeOfGoodDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Kode Dokumen",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "DocumentCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('document')" },
                            { name: "DocumentDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "customer()" },
                            { name: "CustomerName", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    { name: "NPWP", text: "NPWP", cls: "span4", type: "ng-maskedit", mask: "##.###.###.#-###.###" },
                    { name: "IsPKP", text: "PKP?", cls: "span4", type: "x-switch", style: "margin-bottom:15px" },
                    { name: "FPJNo", text: "No. Faktur Pajak", cls: "span4" },
                    { name: "FPJDate", text: "Tgl. Faktur Pajak", cls: "span4", type: "ng-datepicker" },
                    { name: "TaxNo", text: "No. Seri Pajak", cls: "span4", type: "ng-maskedit", mask: "###.###-##.########" },
                    { name: "TaxDate", text: "Tgl. Seri Pajak", cls: "span4", type: "ng-datepicker" },
                    { name: "ReferenceNo", text: "No. Referensi", cls: "span4" },
                    { name: "ReferenceDate", text: "Tgl. Referensi", cls: "span4", type: "ng-datepicker" },
                    { name: "SubmissionDate", text: "Tgl. Penyerahan", cls: "span4", type: "ng-datepicker" },
                    { name: "TaxPeriod",model:"model.TaxPeriod", text: "Masa Pajak", cls: "span4", readonly: true },
                    { name: "Description", text: "Nama Barang", cls: "span6" },
                    { name: "Quantity", text: "Jumlah Barang", cls: "span2 number" },
                    { name: "DPPAmt", text: "Nilai DPP", cls: "span4 full number", readonly: true },
                    { name: "PPNAmt", text: "Nilai PPn", cls: "span4 full number", readonly: true },
                    { name: "PPNBmAmt", text: "Nilai PPnBM", cls: "span4 full number", readonly: true },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnSaveDetail", text: "", icon: "icon-plus", cls: "btn btn-success", click: "saveDetail()", disable: "!isExsist" },
                                { name: "btnDeleteDetail", text: "", icon: "icon-remove", cls: "btn btn-danger", click: "deleteDetail()", disable: "!isExsist" },
                        ]
                    },
                    { type: "div" },
                    { type: "div" },

                     { name: "lblTotal", text: "Total", cls: "span4", type: "label" },
                    { name: "lblGrandTotal", text: "Grand Total", cls: "span4", type: "label" },
                    { cls: "span4 divider", type: "div" },
                    { cls: "span4 divider", type: "div" },
                   { name: "SumDPPStd", model: "total.SumDPPStd", text: "DPP Standar", cls: "span4 number", readonly: true },
                   { name: "DPPAmt", model: "total.DPPAmt", text: "DPP", cls: "span4 number", readonly: true },
                   { name: "SumDPPSdh", model: "total.SumDPPSdh", text: "DPP Sederhana", cls: "span4 number", readonly: true },
                   { name: "PPNAmt", model: "total.PPNAmt", text: "PPn", cls: "span4 number", readonly: true },
                   { name: "SumPPNStd", model: "total.SumPPNStd", text: "PPn Standar", cls: "span4 number", readonly: true },
                   { name: "SumPPnBMAmt", model: "total.SumPPnBMAmt", text: "PPnBM", cls: "span4 number", readonly: true },
                   { name: "SumPPNSdh", model: "total.SumPPNSdh", text: "PPn Sederhana", cls: "span4 number", readonly: true }
                ]
            },
            {
                name: "pnlGrid",
                title: "",
                items: [
                      {
                          type: "optionbuttons",
                          name: "tabpage1",
                          model: "options",
                          items: [
                              { name: "0", text: "Daftar Pajak Masukan" },
                              { name: "1", text: "Riwayat Pajak Masukan" },
                          ]
                      },
                ]
            },
             {
                 name: "Selling",
                 cls: "tabpage1 0 animate-show",
                 show: "options == '0'",
                 items: [
                     {
                         name: "wxTaxOut",
                         type: "wxdiv"
                     }
                 ]
             },
            {
                name: "SellingPORDD",
                cls: "tabpage1 1 animate-show",
                show: "options == '1'",
                items: [
                    {
                        name: "wxTaxOutHist",
                        type: "wxdiv"
                    }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("taxConsolidationTaxOutController");
    }
});