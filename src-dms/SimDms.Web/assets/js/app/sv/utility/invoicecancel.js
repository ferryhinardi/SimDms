"use strict"

function invoicecancel($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    var InvoiceNoDefault = 'IXX/XX/YYYYYY';

    me.InvoiceLookup = function (sender) {
        var lookup = Wx.klookup({
            name: "InvoiceCancelLookup",
            title: "Invoice - Faktur Pajak Lookup",
            url: "sv.api/InvoiceCancel/InvoiceForCancellation",
            serverBinding: true,
            sort: [
                { 'field': 'InvoiceDate', 'dir': 'desc' },
                { 'field': 'InvoiceNo', 'dir': 'desc' }
            ],
            pageSize: 10,
            columns: [
                { field: "InvoiceNo", title: "No. Faktur Penjualan", width: 180 },
                {
                    field: "InvoiceDate", title: "Tgl. Faktur Penjualan", width: 180,
                    template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                },
                { field: "FPJNo", title: "No. Faktur Pajak", width: 160 },
                {
                    field: "FPJDate", title: "Tgl. Faktur Pajak", width: 160,
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: "JobOrderNo", title: "No. SPK", width: 150 },
                {
                    field: "JobOrderDate", title: "Tgl. SPK", width: 130,
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "PoliceRegNo", title: "No. Polisi", width: 120 },
                { field: "ServiceBookNo", title: "No. Buku Service", width: 160 },
                { field: "JobType", title: "Jenis Pekerjaan", width: 130 },
                { field: "ChassisCode", title: "Kode Rangka", width: 140 },
                { field: "ChassisNo", title: "No. Rangka", width: 140 },
                { field: "EngineCode", title: "Kode Mesin", width: 140 },
                { field: "EngineNo", title: "No. Mesin", width: 150 },
                { field: "BasicModel", title: "Basic Model", width: 150 },
                { field: "Customer", title: "Pelanggan", width: 250 },
                { field: "CustomerBill", title: "Pembayar", width: 250 },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                if (sender == 'from') {
                    if (me.data.InvoiceNoTo != InvoiceNoDefault) {
                        if (me.data.InvoiceNoTo < data.InvoiceNo) {
                            me.data.InvoiceNoFrom = data.InvoiceNo;
                            me.data.InvoiceNoTo = data.InvoiceNo;
                        }
                        else {
                            me.data.InvoiceNoFrom = data.InvoiceNo;
                        }
                    }
                    else {
                        me.data.InvoiceNoFrom = data.InvoiceNo;
                        me.data.InvoiceNoTo = data.InvoiceNo;
                    }
                }
                else {
                    if (me.data.InvoiceNoTo != InvoiceNoDefault) {
                        if (me.data.InvoiceNoTo > data.InvoiceNo) {
                            me.data.InvoiceNoFrom = data.InvoiceNo;
                            me.data.InvoiceNoTo = data.InvoiceNo;
                        }
                        else {
                            me.data.InvoiceNoTo = data.InvoiceNo;
                        }
                    }
                    else {
                        me.data.InvoiceNoFrom = data.InvoiceNo;
                        me.data.InvoiceNoTo = data.InvoiceNo;
                    }
                }
                me.Apply();

            }
        });
    }

    me.grid1 = new webix.ui({
        container: "table1",
        view: "wxtable", css:"alternating",
        autoHeight: false,
        height: 350,
        autowidth: false,
        width: 1000,
        scrollX: true,
        scrollY: true,
        checkboxRefresh: true,
        columns: [
            { id: "IsSelected", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 40 },
            { id: "InvoiceNo", header: "No Invoice", width: 100 },
            { id: "InvoiceDate", header: "Tgl Invoice", width: 120, format: me.dateFormat },
            { id: "JobOrderNo", header: "No Job Order", width: 120 },
            { id: "JobOrderDate", header: "Tgl Job Order", width: 120, format: me.dateFormat },
            { id: "JobType", header: "Job Type", width: 100 },
            { id: "LaborDppAmt", header: "Labor Amount", width: 120, format: me.intFormat, css: "text-right" },
            { id: "PartsDppAmt", header: "Part Amount", width: 120, format: me.intFormat, css: "text-right" },
            { id: "MaterialDppAmt", header: "Material Amt", width: 120, format: me.intFormat, css: "text-right" },
            { id: "TotalSrvAmt", header: "Total Srv Amt", width: 150, format: me.intFormat, css: "text-right" }
        ]
    });

    me.grid1.attachEvent("onItemDblClick", function (id, e, node) {
        var data = {
            invoiceNo: this.getItem(id).InvoiceNo
        }

        $http.post('sv.api/InvoiceCancel/GetDescInvoice', data).
        success(function (data, status, headers, config) {
            me.loadTableData(me.grid2, data);
            $("p[data-name='tab2']").click();
        }).error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

    }); 

    me.grid2 = new webix.ui({
        container: "table2",
        view: "wxtable", css:"alternating",
        autoHeight: false,
        height: 350,
        autowidth: false,
        width: 1000,
        scrollX: true,
        scrollY: true,
        checkboxRefresh: true,
        columns: [
            { id: "DocNo", header: "No Document", width: 130 },
            { id: "AccountNo", header: "No Account", width: 200 },
            { id: "Description", header: "Deskripsi", width: 350 },
            { id: "TypeTrans", header: "Type Trans", width: 150 },
            { id: "AmountDbOld", header: "Debet Amount", width: 120, format: me.intFormat, css: "text-right" },
            { id: "AmountCrOld", header: "Credit Amount", width: 120, format: me.intFormat, css: "text-right" },
            { id: "AmountDb", header: "Debet Koreksi", width: 120, format: me.intFormat, css: "text-right" },
            { id: "AmountCr", header: "Credit Koreksi", width: 120, format: me.intFormat, css: "text-right" },
        ]
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
        me.grid2.adjust();
    });


    me.btnInquiry = function () {
        if (me.data.InvoiceNoFrom == InvoiceNoDefault || me.data.InvoiceNoTo == InvoiceNoDefault)
            return;


        if (me.data.InvoiceNoFrom.trim() == "" || me.data.InvoiceNoTo.trim() == "") {
            MsgBox("No. Invoice harus diisi");
            return;
        }
        var data = {
            invoice1: me.data.InvoiceNoFrom,
            invoice2: me.data.InvoiceNoTo
        }
        $http.post('sv.api/InvoiceCancel/SelectInqInvCancel', data).
        success(function (data, status, headers, config) {
            me.loadTableData(me.grid1, data);
            me.detail = data;
        }).error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.btnReposting = function () {
        var data = [];
        $.each(me.detail, function (key, val) {
            if (val["IsSelected"] == 1) {
                data.push(me.detail[key]);
            }
        });
        if (data.length == 0) {
            MsgBox("Belum ada data yang dipilih");
            return;
        }
        $http.post('sv.api/InvoiceCancel/RePosting', data).
        success(function (data, status, headers, config) {
            if (data.message == "") {
                MsgBox("Proses Re-Posting Faktur Selesai");
            } else {
                MsgBox(data.message);
            }
        }).error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.btnCancelInvoice = function () {
        var data = [];
        $.each(me.detail, function (key, val) {
            if (val["IsSelected"] == 1) {
                data.push(me.detail[key]);
            }
        });
        if (data.length == 0) {
            MsgBox("Belum ada data yang dipilih");
            return;
        }
        MsgConfirm("Anda memilih untuk membatalkan invoice. Apakah anda yakin?", function (ok) {
            if (!ok) return;
            $http.post('sv.api/InvoiceCancel/CancelInvoice', data).
                success(function (data, status, headers, config) {
                    MsgBox("Proses Cancel Invoice Selesai");
                    me.btnInquiry();
                }).error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
        });
    }

    me.validateInvoiceNoFrom = function (val) {
        $http.post('sv.api/InvoiceCancel/ValidateInvoiceNo', { InvoiceNo: val })
        .success(function (result) {
            if (result.Success) {
                if (me.data.InvoiceNoTo != InvoiceNoDefault) {
                    if (me.data.InvoiceNoTo < val) {
                        me.data.InvoiceNoFrom = val;
                        me.data.InvoiceNoTo = val;
                    }
                    else {
                        me.data.InvoiceNoFrom = val;
                    }
                }
                else {
                    me.data.InvoiceNoFrom = val;
                    me.data.InvoiceNoTo = val;
                }
            }
            else {
                me.InvoiceLookup("from");
            }
        })
        .error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.validateInvoiceNoTo = function (val) {
        $http.post('sv.api/InvoiceCancel/ValidateInvoiceNo', { InvoiceNo: val })
        .success(function (result) {
            if (result.Success) {
                if (me.data.InvoiceNoTo != InvoiceNoDefault) {
                    if (me.data.InvoiceNoFrom > val) {
                        me.data.InvoiceNoFrom = val;
                        me.data.InvoiceNoTo = val;
                    }
                    else {
                        me.data.InvoiceNoTo = val;
                    }
                }
                else {
                    me.data.InvoiceNoFrom = val;
                    me.data.InvoiceNoTo = val;
                }
            }
            else {
                me.InvoiceLookup("to");
            }
        })
        .error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.initialize = function () {
        me.detail = {};
        me.data.InvoiceNoFrom = InvoiceNoDefault;
        me.data.InvoiceNoTo = InvoiceNoDefault;
        me.clearTable(me.grid1);
        me.clearTable(me.grid2);
        $("p[data-name='tab1']").click();

        $("#InvoiceNoFrom").on("blur", function () {
            var val = $(this).val();
            if (val != "") {
                me.validateInvoiceNoFrom(val);
            }
        });

        $("#InvoiceNoFrom").on("keypress", function (e) {
            if (e.keyCode == 13) {
                var val = $(this).val();
                if (val != "") {
                    $("#btnInquiry").focus();
                }
            }
        });

        $("#InvoiceNoTo").on("blur", function () {
            var val = $(this).val();
            if (val != "") {
                me.validateInvoiceNoTo(val);
            }
        });

        $("#InvoiceNoTo").on("keypress", function (e) {
            if (e.keyCode == 13) {
                var val = $(this).val();
                if (val != "") {
                    $("#btnInquiry").focus();
                }
            }
        });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Re-posting Journal dan Pembatalan Invoice",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" }
        ],
        panels: [
            {
                title: "Inquiry Invoice",
                items: [
                    {
                        text: "Invoice No",
                        type: "controls",
                        items: [
                            { name: "InvoiceNoFrom", text: "From", type: "popup", cls: "span3", btnName: "btnInvoiceFrom", validasi: "required", click: "InvoiceLookup('from')" },
                            { name: "InvoiceNoTo", text: "To", type: "popup", cls: "span3", btnName: "btnInvoiceTo", validasi: "required", click: "InvoiceLookup('to')" },
                            {
                                type: "buttons",
                                cls: "span2",
                                items: [
                                    { name: "btnInquiry", text: " Inquiry", cls: "btn btn-success", icon: "icon-search", click: "btnInquiry()", style: "height:32px; line-height:28px; padding-top:0px" }
                                ]
                            }
                        ]
                    },
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "tab1", text: "List Invoice Data" },
                    { name: "tab2", text: "Journal Data" },
                ]
            },
            {
                name: "table1",
                cls: "tabpage1 tab1",
                xtype: "wxtable"
            },
            {
                name: "table2",
                cls: "tabpage1 tab2",
                xtype: "wxtable"
            },
            {
                items: [
                    {
                        type: "buttons",
                        items: [
                            { name: "btnReposting", text: " Re-Posting Invoice", cls: "btn btn-success", icon: "icon-gear", click: "btnReposting()" },
                            { name: "btnCancelInvoice", text: "Pembatalan Invoice", cls: "btn btn-success", icon: "icon-remove", click: "btnCancelInvoice()" }
                        ]
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    $("p[data-name='tab1']").addClass('active');

    function init(s) {
        SimDms.Angular("invoicecancel");
    }
});