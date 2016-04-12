$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Sales Order",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
            { name: "btnCustomer", text: "Pelanggan", icon: "icon-user" },
        ],
        panels: [
            {
                title: "Informasi Pelanggan",
                items: [
                    { text: "No. SO", cls: "span4" },
                    { text: "Tgl. SO", type: "kdatepicker", cls: "span4" },
                    { text: "No. Reff", cls: "span4" },
                    { text: "Tgl. Reff", type: "kdatepicker", cls: "span4" },
                    { name: "IsDirectSales", text: "Direct Sales", type: "switch", float: "left" },
                    {
                        text: "No. ITS",
                        type: "controls",
                        items: [
                            { text: "No. ITS", type: "popup", cls: "span2" },
                            { text: "Tipe Kendaraan", readonly: true, cls: "span3" },
                            { text: "No. SKPK", cls: "span3" },
                        ]
                    },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        items: [
                            { text: "Kode", type: "popup", cls: "span2" },
                            { text: "Nama", readonly: true, cls: "span6" },
                        ]
                    },
                    {
                        text: "Salesman",
                        type: "controls",
                        items: [
                            { text: "Kode", type: "popup", cls: "span2" },
                            { text: "Nama", readonly: true, cls: "span6" },
                        ]
                    },
                    {
                        text: "TOP",
                        type: "controls",
                        items: [
                            { text: "Kode", type: "popup", cls: "span2" },
                            { text: "Nama", readonly: true, cls: "span3" },
                            { text: "Dibayar dengan", cls: "span3" },
                        ]
                    },
                    {
                        text: "Tagih ke",
                        type: "controls",
                        items: [
                            { text: "Kode", type: "popup", cls: "span2" },
                            { text: "Nama", readonly: true, cls: "span6" },
                        ]
                    },
                    {
                        text: "Kirim ke",
                        type: "controls",
                        items: [
                            { text: "Kode", type: "popup", cls: "span2" },
                            { text: "Nama", readonly: true, cls: "span6" },
                        ]
                    },
                    {
                        text: "Gudang",
                        type: "controls",
                        items: [
                            { text: "Kode", type: "popup", cls: "span2" },
                            { text: "Nama", readonly: true, cls: "span6" },
                        ]
                    },
                    {
                        text: "Group Price Code",
                        type: "controls",
                        items: [
                            { text: "Kode", readonly: true, cls: "span2" },
                            { text: "Nama", readonly: true, cls: "span2" },
                        ]
                    },
                ]
            },
            {
                title: "Informasi Leasing",
                items: [
                    { name: "IsLeasing", text: "Leasing", type: "switch", float: "left" },
                    {
                        text: "Info Leasing",
                        type: "controls",
                        items: [
                            { text: "Kode Leasing", type: "popup", cls: "span2" },
                            { text: "Nama Leasing", readonly: true, cls: "span3" },
                            { text: "Angsuran", type: "select", cls: "span3" },
                        ]
                    },
                    {
                        text: "Tanggal Lunas",
                        type: "controls",
                        items: [
                            { text: "Tgl. Lunas", type: "kdatepicker", cls: "span2" },
                            { text: "Asuransi", cls: "span3" },
                            { text: "Uang Muka", cls: "span3" },
                        ]
                    },
                    {
                        text: "Uang Mula",
                        type: "controls",
                        items: [
                            { text: "Uang Muka", type: "decimal", cls: "span5" },
                        ]
                    },
                    {
                        text: "Diterima Oleh",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span2" },
                            { text: "Nama", readonly: true, cls: "span6" },
                        ]
                    },
                ]
            },
            {
                title: "Informasi Penerima",
                items: [
                    {
                        text: "Diterima Oleh",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span2" },
                            { text: "Nama", readonly: true, cls: "span6" },
                        ]
                    },
                    {
                        text: "Pada Tanggal",
                        type: "controls",
                        items: [
                            { type: "kdatepicker", cls: "span2" },
                        ]
                    },
                    {
                        text: "Mediator",
                        type: "controls",
                        items: [
                            { text: "Nama Mediator", cls: "span5" },
                            { text: "Nilai Komisi", type: "decimal", cls: "span3" },
                        ]
                    },
                ]
            },
            {
                title: "Informasi Kontrak",
                items: [
                    {
                        text: "No PO / No Kontrak",
                        type: "controls",
                        items: [
                            { text: "No PO", cls: "span2" },
                            { text: "No Kontrak", cls: "span3" },
                        ]
                    },
                    {
                        text: "Tgl Dibubuhkan",
                        type: "controls",
                        items: [
                            { type: "kdatepicker", cls: "span2" },
                        ]
                    },
                    { text: "Keterangan", type: "textarea" },
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "tab1", text: "Sales Model" },
                    { name: "tab2", text: "Informasi Kendaraan" },
                    { name: "tab3", text: "Aksesories", cls: "active" },
                    { name: "tab4", text: "Sparepart" },
                ]
            },
            {
                cls: "tabpage1 tab1",
                title: "Sales Model",
                items: [
                    { text: "Sales Model Code", cls: "span3", type: "popup" },
                    {
                        text: "Sales Model Year",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { text: "Kode", cls: "span3", type: "popup" },
                            { text: "Nama", cls: "span5", readonly: true },
                        ]
                    },
                    { text: "Ongkos Kirim", cls: "span3" },
                    { text: "Unit Deposit", cls: "span3" },
                    { text: "Lain-lain", cls: "span3" },
                    { text: "Diskon", cls: "span3" },
                    { text: "Keterangan", type: "textarea", cls: "span8" },
                ]
            },
            {
                cls: "tabpage1 tab1",
                title: "Harga",
                items: [
                    { text: "Sebelum Diskon", cls: "span4" },
                    { text: "Setelah Diskon", cls: "span4" },
                    {
                        text: "DPP / PPn /PPnBM",
                        type: "controls",
                        items: [
                            { text: "DPP", cls: "span3", readonly: true },
                            { text: "PPn", cls: "span3", readonly: true },
                            { text: "PPnBM", cls: "span2", readonly: true },
                        ]
                    },
                ]
            },
            {
                cls: "tabpage1 tab2",
                title: "Detail Warna",
                items: [
                    {
                        text: "Warna",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span2", type: "popup" },
                            { text: "Nama", cls: "span4", readonly: true },
                            { text: "Jumlah", cls: "span2", type: "int" },
                        ]
                    },
                    { text: "Keterangan", type: "textarea" },
                ]
            },
            {
                cls: "tabpage1 tab2",
                title: "Lain - lain",
                items: [
                    {
                        text: "Kode / Nomor Rangka",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span4", readonly: true },
                            { text: "Nomor", cls: "span4", type: "popup" },
                        ]
                    },
                    { text: "Nama STNK" },
                    { text: "Alamat STNK" },
                    {
                        type: "controls",
                        items: [
                            { cls: "span4", readonly: true },
                            { cls: "span4", readonly: true },
                        ]
                    },
                    {
                        text: "Kota",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span2", type: "popup" },
                            { text: "Nama", cls: "span6", readonly: true },
                        ]
                    },
                    {
                        text: "Pemasok BBN",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span2", type: "popup" },
                            { text: "Nama", cls: "span6", readonly: true },
                        ]
                    },
                    { text: "BBN", cls: "span4" },
                    { text: "KIR", cls: "span4" },
                    { text: "Keterangan", type: "textarea" },
                ]
            },
            {
                cls: "tabpage1 tab3",
                title: "Aksesories",
                items: [
                    {
                        text: "Aksesories Lain-lain",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span2", type: "popup" },
                            { text: "Nama", cls: "span6", readonly: true },
                        ]
                    },
                    { text: "Sebelum Diskon", cls: "span4", type: "decimal" },
                    { text: "Setelah Diskon", cls: "span4", type: "decimal" },
                    { text: "DPP", cls: "span4", type: "decimal" },
                    { text: "PPN", cls: "span4", type: "decimal" },
                ]
            },
            {
                cls: "tabpage1 tab4",
                title: "Sparepart",
                items: [
                    {
                        text: "Sparepart",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span2", type: "popup" },
                            { text: "Nama", cls: "span6", readonly: true },
                        ]
                    },
                    { text: "Sebelum Diskon", cls: "span4", type: "decimal" },
                    { text: "Setelah Diskon", cls: "span4", type: "decimal" },
                    { text: "DPP", cls: "span4", type: "decimal" },
                    { text: "PPN", cls: "span4", type: "decimal" },
                    { text: "Quantity", cls: "span4", type: "decimal" },
                    { text: "Unit", cls: "span4", type: "decimal" },
                ]
            },
        ]
    });

    widget.default = {};
    widget.render(init);

    function init() {

    }
});