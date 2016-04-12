var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spPembuatanLampirandokumenNonPenjualanController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.loadDetail = function (data) {

    }

    me.saveData = function (e, param) {
        var form = $('.main form');

        $http.post('sp.api/PembuatanLMPDocNP/save?salesType='+me.jenisTransaksi, me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    me.allowEdit = false;
                    if (me.jenisTransaksi == 1) {
                        me.allowGenerate = true;
                    }
                    else {
                        me.allowGenerate = false;
                    }
                    me.lookupAfterSelect(data.data);
                    me.PopulateCustomerInfo(data.data, "display");
                    me.isPrintAvailable = true;
                    CheckLMPStatus(data.data.LmpNo);
                    $('#btnDelete').hide();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.UpdateGridDetail = function (data) {
        me.grid.detail = data;
        me.loadTableData(me.grid1, me.grid.detail);
    }

    me.pickinglistno = function () {
        var query = "";
        if (me.jenisTransaksi == 2) {
            query = "LookupLMP4Srv?SalesType=" + me.jenisTransaksi;
        } else {
            query = "LookupLMP?SalesType=" + me.jenisTransaksi;
        }
        var lookup = Wx.blookup({
            name: "LookupLMP",
            title: "Pencarian No. Picking List",
            manager: spManager,
            query: query,
            defaultSort: "PickingSlipNo asc",
            columns: [
            { field: "PickingSlipNo", title: "Picking Slip No" },
            { field: "PickingSlipDate", title: "Picking Slip Date", template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #" },
            { field: "BPSFNo", title: "Invoice No" },
            { field: "BPSFDate", title: "Invoice Date", template: "#= (BPSFDate == undefined) ? '' : moment(BPSFDate).format('DD MMM YYYY') #" },
            { field: "ProductType", title: "Product Type" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isInitialize = false;
                me.data = data;
                me.PopulateCustomerInfo(data, "edit");
                me.data.LmpDate = me.now();
                $('#LmpDate').removeAttr('readonly');
                me.GetLmpDtl(data.BPSFNo);
            }
        });
    };

    me.PopulateCustomerInfo = function (data, e) {
        var params = {
            CustomerCodeShip: data.CustomerCodeShip
        }
        $http.post("sp.api/PembuatanLMPDocNP/GetCustomerShip", params)
        .success(function (result) {
            if (e == "edit") {
                me.RetrieveDataForEdit(result.data);
            }
            else {
                me.RetrieveDataForDisplay(result.data);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

        params = {
            CustomerCodeBill: data.CustomerCodeTagih
        }
        $http.post("sp.api/PembuatanLMPDocNP/GetCustomerBill", params)
        .success(function (result) {
            if (e == "edit") {
                me.RetrieveDataForEdit(result.data);
            }
            else {
                me.RetrieveDataForDisplay(result.data);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };
    
    me.GetLmpDtl = function (BPSFNo) {
        $http.post('sp.api/PembuatanLMPDocNP/GetSpTrnSBPSFDtl?BPSFNo=' + BPSFNo).
                success(function (data, status, headers, config) {
                    me.grid.detail = data;
                    me.loadTableData(me.grid1, me.grid.detail);
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
    }

    me.browse = function () {
        me.isInitialize = false;
        if (me.jenisTransaksi != undefined) {
            query = "GetLampiranDokumen?SalesType=" + me.jenisTransaksi;
            var lookup = Wx.blookup({
                name: "GetLMP",
                title: "Pencarian No. Lampiran",
                manager: spManager,
                query: query,
                defaultSort: "LmpNo asc",
                columns: [
                    { field: "LmpNo", title: "No. Lampiran" },
                    { field: "LmpDate", title: "Tgl. Lampiran", template: "#= (LmpDate == undefined) ? '' : moment(LmpDate).format('DD MMM YYYY') #" },
                    { field: "BPSFNo", title: "BPS No" },
                    { field: "BPSFDate", title: "BPS Date", template: "#= (BPSFDate == undefined) ? '' : moment(BPSFDate).format('DD MMM YYYY') #" },
                    { field: "PickingSlipNo", title: "Picking Slip No" },
                    { field: "PickingSlipDate", title: "Picking Slip Date", template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #" }
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.allowEdit = false;
                    if (me.jenisTransaksi == 1) {
                        me.allowGenerate = true;
                    }
                    else {
                        me.allowGenerate = false;
                    }
                    me.lookupAfterSelect(data);
                    me.PopulateCustomerInfo(data, "display");

                    me.isPrintAvailable = true;
                    CheckLMPStatus(data.LmpNo);

                    $http.post('sp.api/PembuatanLMPDocNP/GetSpTrnSLmpDtl?LmpNo=' + data.LmpNo).
                    success(function (data, status, headers, config) {
                        me.grid.detail = data;
                        me.loadTableData(me.grid1, me.grid.detail);
                    }).
                    error(function (data, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
                    $('#btnDelete').hide();
                }
            });
        }
    }

    me.RetrieveDataForEdit = function (value) {
        if (value == null || value == undefined) return;
        setTimeout(function () {
            me.ReformatNumber();
            var selectorContainer = "";
            $.each(value, function (key, val) {
                var ctrl = $(selectorContainer + " [name=" + key + "]");
                me.data[key] = val;

                ctrl.removeClass("error");
            });

            $scope.$apply();
        }, 200);
    }

    me.RetrieveDataForDisplay = function (value) {
        if (value == null || value == undefined) return;

        me.isLoadData = true;
        setTimeout(function () {
            me.hasChanged = false;
            me.startEditing();
            me.isSave = true;

            me.ReformatNumber();
            var selectorContainer = "";
            $.each(value, function (key, val) {
                var ctrl = $(selectorContainer + " [name=" + key + "]");
                me.data[key] = val;

                ctrl.removeClass("error");
            });

            $scope.$apply();
            me.isSave = false;
        }, 200);
    };

    me.GenerateTrsfStockFile = function () {
        window.location = "sp.api/PembuatanLMPDocNP/GenerateTrsfStockFile?LmpNo=" + me.data.LmpNo + "&BPSFNo=" + me.data.BPSFNo;
    };

    me.grid1 = new webix.ui({
        container: "wxtableDtl",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "PartNo", header: "No. Part", fillspace: true },
            { id: "PartNoOriginal", header: "No. Part Original", fillspace: true },
            { id: "DocNo", header: "No. Dokumen", fillspace: true },
            { id: "DocDate", header: "Tgl. Dokumen", fillspace: true, format: me.dateFormat },
            { id: "ReferenceNo", header: "No. Order", fillspace: true },
            { id: "QtyBill", header: "Qty. Lampiran", fillspace: true, format: me.decimalFormat, css: "text-right" },
        ]
    });

    me.default = function () {
        me.clearTable(me.grid1);
        me.detail = {};
        me.data = {};
        me.data.LmpDate = me.data.PickingSlipDate = me.now();
        me.allowEdit = true;
        $('#LmpStatus').html("");
        me.isPrintAvailable = false;
        $('#CustomerNameTagih,#Address1Tagih,#Address2Tagih,#Address3Tagih,#Address4Tagih,#LmpDate').attr('readonly', 'readonly');
    }

    me.initialize = function () {
        me.default();
        me.jenisTransaksi = "1";
        me.data.BPSFNo = "Non Penjualan";
    }

    me.allowEdit = function () {
        me.isLoadData = false;
        me.isEditable = false;
        me.hasChanged = true;
        $("#btnCancel").html("<i class='icon icon-undo'></i>Cancel");
        $('#CustomerNameTagih,#Address1Tagih,#Address2Tagih,#Address3Tagih,#Address4Tagih,#LmpDate').removeAttr('readonly', 'readonly');
    }

    $('div > p').click(function () {
        var name = $(this).data("name");
        if (name === 'tabDP') {
            me.GetLmpDtl(me.data.BPSFNo);
        }
    });

    me.SetDocument = function (val) {
        var jt = "";
        if (val == 1) {
            jt = "Non Penjualan";
        } else if (val == 2) {
            jt = "Service";
        }
        else {
            jt = "Unit Order";
        }
        me.isInitialize = true;
        me.default();
        me.data.BPSFNo = jt;
        me.allowEdit = true;
        me.allowGenerate = false;
    };

    me.printPreview = function () {
        var LmpNo = me.data.LmpNo;
        $http.post('sp.api/PembuatanLMPDocNP/UpdateLampiranDoc', { LmpNo: LmpNo })
                .success(function (v, status, headers, config) {
                    if (v.success) {
                        var data = me.jenisTransaksi + "," + $('#LmpNo').val() + "," + $('#LmpNo').val() + "," + "300" + "," + "typeofgoods" + "," + "1";
                        var rparam = "ga";

                        Wx.showPdfReport({
                            id: "SpRpTrn028",
                            pparam: data,
                            rparam: rparam,
                            textprint:true,
                            type: "devex"
                        });
                        CheckLMPStatus(LmpNo);
                    } else {
                        // show an error message
                        MsgBox(v.message, MSG_ERROR);
                    }
                }).error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
    };

    function CheckLMPStatus(LmpNo) {
        $http.post('sp.api/Pengeluaran/CheckStatus', { WhereValue: LmpNo, Table: "spTrnSLmpHdr", ColumnName: "LmpNo" })
    .success(function (v, status, headers, config) {
        if (v.success) {
            $('#LmpStatus').html('<span style="font-size:28px;color:red;font-weight:bold">' + v.statusPrint.toUpperCase() + "</span>");
        } else {
            // show an error message
            MsgBox(v.message, MSG_ERROR);
        }
    }).error(function (e, status, headers, config) {
        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
    });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Generate Document Non Sales",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlFPStatus",
                title: " ",
                items: [
                { name: "LmpStatus", text: "", cls: "span4", readonly: true, type: "label" },
                {
                    type: "buttons", cls: "span4", items: [
                      { name: "btnGen", text: "Generate Trans Stock File", disable: "!allowGenerate", click: "GenerateTrsfStockFile()" },
                    ]
                },
                ]
            },
            {
                name: "pnlFNP",
                title: "",
                items: [
                        {
                            type: "optionbuttons",
                            name: "jenisTransaksi",
                            model: "jenisTransaksi",
                            text: "Asal Dokumen",
                            items: [
                                { name: "1", text: "Non Penjualan", click: "SetDocument(1)" },
                                { name: "2", text: "Service", click: "SetDocument(2)" },
                                { name: "3", text: "Unit Order", click: "SetDocument(3)" },
                            ]
                        },
                        { name: "LmpNo", model: "data.LmpNo", text: "No. Lampiran", cls: "span3", disable: "IsEditing()", readonly: true, placeHolder: "LMP/XX/YYYYY" },
                        { name: "LmpDate", model: "data.LmpDate", text: "Tgl. Lampiran", cls: "span3", disable: "IsEditing()", type: "ng-datepicker", readonly: true, validasi: "required", required: true },
                        { name: "PickingSlipNo", model: "data.PickingSlipNo", text: "No. Picking List", cls: "span3", type: "popup", btnName: "btnPartNo", readonly: true, click: "pickinglistno()", validasi: "required", required: true, disable: "!allowEdit" },
                        { name: "PickingSlipDate", model: "data.PickingSlipDate", text: "Tgl. Picking List", cls: "span3", type: "ng-datepicker", readonly: true },
                        { name: "BPSFNo", model: "data.BPSFNo", text: "Asal Dokumen", type: "text", cls: "span3", readonly: true },
                        { name: "TransType", text: "Tipe Transaksi", cls: "span3", type: "text", readonly: true },
                        {
                            text: "Pelanggan", type: "controls", type: "controls", items: [
                            { name: "CustomerCode", model: "data.CustomerCode", cls: "span2", placeHolder: "Category Code", readonly: true },
                            { name: "CustomerName", model: "data.CustomerName", cls: "span4", placeHolder: "Category Name", readonly: true }]
                        }
                ]
            },
            {
                xtype: "tabs",
                name: "tabFP",
                items: [
                    { name: "tabPP", text: "Pengiriman dan Penagihan" },
                    { name: "tabDP", text: "Details Pesanan" },
                ]
            },
            {
                title: "Alamat Pengiriman",
                cls: "tabFP tabPP",
                items: [
                    {
                        text: "Pelanggan", type: "controls", type: "controls", items: [
                        { name: "CustomerCodeShip", model: "data.CustomerCodeShip", cls: "span2", placeHolder: "Customer Code", readonly: true },
                        { name: "CustomerNameShip", model: "data.CustomerNameShip", cls: "span4", placeHolder: "Customer Name", readonly: true }]
                    },
                    { name: "Address1", model: "data.Address1", text: "Alamat", cls: "span6", readonly: true },
                    { name: "Address2", model: "data.Address2", text: "", cls: "span6", readonly: true },
                    { name: "Address3", model: "data.Address3", text: "", cls: "span6", readonly: true },
                    { name: "Address4", model: "data.Address4", text: "", cls: "span6", readonly: true },
                ]
            },
            {
                title: "Alamat Penagihan",
                cls: "tabFP tabPP",
                items: [
                    {
                        text: "Pelanggan", type: "controls", type: "controls", items: [
                        { name: "CustomerCodeTagih", model: "data.CustomerCodeTagih", cls: "span2", placeHolder: "Customer Code", readonly: true },
                        { name: "CustomerNameTagih", model: "data.CustomerNameTagih", cls: "span4", placeHolder: "Customer Name", disable: "IsEditing()" }]
                    },
                    { name: "Address1Tagih", model: "data.Address1Tagih", text: "Alamat", cls: "span6", disable: "IsEditing()" },
                    { name: "Address2Tagih", model: "data.Address2Tagih", text: "", cls: "span6", disable: "IsEditing()" },
                    { name: "Address3Tagih", model: "data.Address3Tagih", text: "", cls: "span6", disable: "IsEditing()" },
                    { name: "Address4Tagih", model: "data.Address4Tagih", text: "", cls: "span6", disable: "IsEditing()" }
                ]
            },
            {
                name: "pnlC",
                title: "Details Pesanan",
                cls: "tabFP tabDP",
                items: [
                    {
                        name: "wxtableDtl",
                        type: "wxdiv",
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spPembuatanLampirandokumenNonPenjualanController");
    }

});