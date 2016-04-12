"use strict"

function taxEntryManualController($scope, $http, $injector) {

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

    $http.post('sp.api/Combo/LoadLookup?CodeId=PFCN').
    success(function (data, status, headers, config) {
        me.comboPFCN = data;
        me.init();
    });

    $('#ProfitCenter').on('change', function (a) {
        me.data.SupplierCode = '';
        me.data.SupplierName = "";
        me.data.Address = "";
        me.data.NPWP = "";
        me.Apply();
    });

    me.$watch('options', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    me.default = function () {
        $http.post('sv.api/entrytaxmanual/default').
        success(function (data, status, headers, config) {
            $('#PeriodMonth').select2('val', data.Month);
            $('#PeriodYear').select2('val', data.Year);
           

            me.data = data;
            
            me.data.PeriodMonth = data.Month;
            me.data.PeriodYear = data.Year;
            me.data.FPJDate = me.now();
            me.data.TaxDate = me.now();
            me.data.ReferenceDate = me.now();
            me.data.SubmissionDate = me.now();
        });
    }
    
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
            case 'transin':
                title = "Pencarian Kode Transaksi Pajak";
                value = "Kode Transaksi";
                break;
            case 'statusin':
                title = "Pencarian Kode Status Pajak";
                value = "Kode Status";
                break;
            case 'document':
                title = "Pencarian Kode Dokument Pajak";
                value = "Kode Dokument";
                break;
            case 'doctype':
                title = "Pencarian Tipe Dokument Pajak";
                value = "Tipe Dokument";
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
                case 'transin':
                    me.data.TransactionCode = data.LookUpValue;
                    me.data.TransactionDesc = data.LookUpValueName;
                    break;
                case 'statusin':
                    me.data.StatusCode = data.LookUpValue;
                    me.data.StatusDesc = data.LookUpValueName;
                    break;
                case 'document':
                    me.data.DocumentCode = data.LookUpValue;
                    me.data.DocumentDesc = data.LookUpValueName;
                    break;
                case 'doctype':
                    me.data.DocTypeCode = data.LookUpValue;
                    me.data.DocTypeDesc = data.LookUpValueName;
                    break;
            }
            me.Apply();
        });
    }

    me.supplier = function () {
        if (me.data.ProfitCenter == undefined || me.data.ProfitCenter == null)
        {
            MsgBox("Profit Center harus dipilih salah satu")
        }
        else {
            var lookup = Wx.blookup({
                name: "SupplierBrowse",
                title: "Pencarian Data Pemasok",
                manager: TaxManager,
                query: new breeze.EntityQuery.from("SupplierBrowse").withParameters({ ProfitCenter: me.data.ProfitCenter }),
                defaultSort: "SupplierCode asc",
                columns: [
                    { field: 'SupplierCode', title: 'Kode Supplier', width: 70 },
                    { field: 'SupplierGovName', title: 'Nama Supplier', width: 200 },
                    { field: 'Address', title: 'Alamat', width: 300 },
                    { field: 'ProfitCenter', title: 'Profit', width: 100 },
                ]
            });
            lookup.dblClick(function (data) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierGovName;
                me.data.Address = data.Address;
                me.data.NPWP = data.NPWPNo;
                me.Apply();
            });
        }
    }
   
    me.validesave = function () {
        $http.post('tax.api/entrytaxmanual/validatesave', me.data)
       .success(function (e) {
           if (e.success) {
               Wx.confirm(e.msgCon, function (e) {
                   if (e == "Yes") {
                       me.save();
                   }
                   else
                   {
                       console.log(e.isExsistRecord);
                       if (e.isExsistRecord) MsgBox(e.msgExist);
                   }
               })
           } else {
               me.save();
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }

    me.save = function () {
        $http.post('tax.api/entrytaxmanual/save', me.data)
       .success(function (e) {
           if (e.success) {
               Wx.Success("Berhasil simpan data");
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }


    me.initialize = function () {
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Entry Pajak Manual (Pajak Masukan)",
        xtype: "panels",
        toolbars: [
             { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", click: "validesave()" },
             { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "initialize()" },
        ],
        panels: [
            {
                name: "pnlPeriode",
                title: "Berdasarkan Periode",
                items: [
                   { name: "PeriodMonth", cls: "span3", text: "Bulan", type: "select2", datasource: "comboMonth" },
                   { name: "PeriodYear", required: true, cls: "span2", text: "-", type: "select2", datasource: "comboYear" },
                ]
            },
            {
                name: "pnlEntryTax",
                title: "Pajak Masukan",
                items: [
                    { name: "CompanyCode", text: "Kode Perusahaan", cls: "span4", readonly: true},
                    {
                        text: "Kode Pajak",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "TaxCode", cls: "span2", placeHolder: " ", readonly: true },
                            { name: "TaxDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    { name: "BranchCode", required:true,text: "Kode Cabang", cls: "span4", type: "popup", readonly: true, click: "branch()" },
                    {
                        text: "Kode Transaksi",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            { name: "TransactionCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('transin')" },
                            { name: "TransactionDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Tipe Produk",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            { name: "ProductType", cls: "span2", placeHolder: " ", readonly: true },
                            { name: "ProductDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Kode Status",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            { name: "StatusCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('statusin')" },
                            { name: "StatusDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Kategori Item",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            { name: "TypeOfGoods", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('typeofgoods')" },
                            { name: "TypeOfGoodDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Kode Dokumen",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            { name: "DocumentCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('document')" },
                            { name: "DocumentDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    { name: "ProfitCenter", text: "Profit Center", cls: "span4", type: "select2", datasource: "comboPFCN" },
                    {
                        text: "Tipe Dokumen",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            { name: "DocTypeCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "consolidationLookup('doctype')" },
                            { name: "DocTypeDesc", cls: "span6", placeHolder: " ", readonly: true }
                        ]
                    },
                    {
                        text: "Pemasok",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "SupplierCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "supplier()" },
                            { name: "SupplierName", cls: "span6", placeHolder: " " }
                        ]
                    },
                    { name: "Address", text: "Alamaat", readonly: true},
                    { name: "NPWP", text: "NPWP", cls: "span4", type: "ng-maskedit", mask: "##.###.###.#-###.###" },
                    { name: "IsPKP", text: "PKP?", cls: "span4", type: "x-switch", style: "margin-bottom:15px" },
                    { name: "FPJNo", required: true, text: "No. Faktur Pajak", cls: "span4" },
                    { name: "FPJDate", text: "Tgl. Faktur Pajak", cls: "span4", type: "ng-datepicker" },
                    { name: "TaxNo", required: true, text: "No. Seri Pajak", cls: "span4", type: "ng-maskedit", mask: "###.###-##.########", },
                    { name: "TaxDate", text: "Tgl. Seri Pajak", cls: "span4", type: "ng-datepicker" },
                    { name: "ReferenceNo", required: true, text: "No. Referensi", cls: "span4" },
                    { name: "ReferenceDate", text: "Tgl. Referensi", cls: "span4", type: "ng-datepicker" },
                    { name: "SubmissionDate", text: "Tgl. Penyerahan", cls: "span4", type: "ng-datepicker" },
                    { name: "Description", required: true, text: "Nama Barang", cls: "span6" },
                    { name: "Quantity", required: true, text: "Jumlah Barang", cls: "span2 number" },
                    { name: "DPPAmt", required: true, text: "Nilai DPP", cls: "span4 full number" },
                    { name: "PPNAmt", required: true, text: "Nilai PPn", cls: "span4 full number" },
                    { name: "PPNBmAmt", required: true, text: "Nilai PPnBM", cls: "span4 full number" },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("taxEntryManualController");
    }
});