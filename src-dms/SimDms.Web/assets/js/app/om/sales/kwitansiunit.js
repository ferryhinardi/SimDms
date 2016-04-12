"use strict"
var status = 0;
function omSalesKwitansiUnit($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/Combo/ComboSignature').
    success(function (data, status, headers, config) {
        me.Signature = data;
    });

    me.saveData = function (e, param) {
        $http.post('om.api/kwitansiunit/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    $('#btnClose').removeAttr('disabled');

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                MsgBox("Delete Gagal", MSG_ERROR);
                console.log(e);
            });
    }

    me.closed = function () {
        MsgConfirm("Are you sure to close this document?", function (result) {
            if (result) {
                $http.post('om.api/kwitansiunit/closed', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data Closed...");
                        $('#btnFakturPolisiNo').attr('disabled', 'disabled');
                        $('#btnClose').attr('disabled', 'disabled');
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.printPreview = function () {
        if (me.data.ReceiptNo == undefined) {
            MsgBox("Silahkan masukkan No Faktur Pajak terlebih dahulu", MSG_ERROR);
            me.FakturPolisiNo();
        }
        if (me.data.CustomerName == undefined) {
            MsgBox("Silahkan masukkan Nama Faktur Pajak terlebih dahulu", MSG_ERROR);
        }
        if (me.data.Amount == undefined) {
            MsgBox("Silahkan masukkan Nilai Faktur Pajak terlebih dahulu", MSG_ERROR);
        }
        if (me.data.Description == undefined) {
            MsgBox("Silahkan masukkan Keterangan Pembayaran terlebih dahulu", MSG_ERROR);
        }
        me.saveData();
        var ReportId = 'OmRpSalesTrn012';
        var par = [
                       me.data.FakturPolisiNo
        ]
        var rparam = '';

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "KwitansiBrowse",
            title: "Kwitansi Unit Receipt",
            manager: spSalesManager,
            query: "KwitansiBrowse",
            defaultSort: "ReceiptNo asc",
            columns: [
                { field: "ReceiptNo", title: "No Kwitansi" },
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "ChassisNo", title: "No Rangka" },
                { field: "EngineCode", title: "Kode Mesin" },
                { field: "EngineNo", title: "No Mesin" },
                { field: "CustomerName", title: "Nama Customer" },
                { field: "FakturPolisiNo", title: "No Faktur Polisi" },
                { field: "ColourCode", title: "Warna" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                status = result.ReceiptStatus;
                me.isStatus = status == 2
                me.Apply();
            }

        });
    }

    me.FakturPolisiNo = function () {
        var lookup = Wx.blookup({
            name: "LookUpFakturPenjualan",
            title: "Faktur Polisi",
            manager: spSalesManager,
            query: "LookUpFakturPenjualan",
            defaultSort: "FakturPolisiNo asc",
            columns: [
                { field: "FakturPolisiNo", title: "No Faktur" },
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "ChassisNo", title: "No Rangka" },
                { field: "EngineCode", title: "Kode Mesin" },
                { field: "EngineNo", title: "No Mesin" },
                { field: "SalesModelCode", title: "Kode Model" },
                { field: "SalesModelYear", title: "Tahun Model" },
                { field: "ColourCode", title: "Warna" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.data.Description = "1 (Satu) Unit " + result.ProductType + " Merk SUZUKI Tipe " + result.SalesModelCode + " Tahun " + result.SalesModelYear;
                me.readyToSave();
                me.Apply();
                $("#CustomerName, #Amount, #Description, #ColourDescription").removeAttr("disabled");
            }

        });
    }

    function SetBranchInterface() {
        $http.post('om.api/kwitansiunit/SetNoKwitansi', me.data)
                        .success(function (e) {
                            if (e.msg == "") {
                                me.data.ReceiptNo = e.DocNo;
                            } else {
                                MsgBox(e.msg, MSG_ERROR);
                            }
                        })
                        .error(function (e) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                        });
    }

    me.initialize = function () {
        me.isStatus = false;

        SetBranchInterface();
        me.isPrintAvailable = true;
        $("#CustomerName, #Amount, #Description, #ColourDescription").removeAttr("disabled");
        $('#btnFakturPolisiNo').removeAttr('disabled');
        $('#btnClose').attr('disabled', true);


    }
    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Kwitansi Unit",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "ReceiptNo", text: "No.Kwitansi", cls: "span4 full", placeHolder: "KUN/YY/XXXXXX", disable: true },
                    {
                        text: "No. Faktur Polis",
                        type: "controls",
                        cls: "span5",
                        required: true,
                        validasi: "required",
                        items: [
                            { name: "FakturPolisiNo", cls: "span6", disable: "isStatus", type: "popup", click: "FakturPolisiNo()" },
                            {
                                type: "buttons", cls: "span2 left", items: [
                                    { name: "btnClose", text: "Closed", cls: "btn-small btn-info", icon: "icon-ok", click: "closed()", disable: "isStatus" }
                                ]
                            },
                        ]
                    },
                    { name: "CustomerCode", text: "", cls: "span6", show: false, disable: "isStatus" },
                    { name: "CustomerName", text: "Tanda Terima dari", cls: "span6", required: true, validasi: "required", disable: "isStatus" },
                    { name: "Amount", text: "Nilai Uang", cls: "span4 number-int full", value: 0, required: true, validasi: "required", disable: "isStatus" },
                    { name: "Description", text: "Untuk Pembayaran", cls: "span6", type: "textarea", required: true, validasi: "required", disable: "isStatus" },
                    {
                        type: "controls",
                        text: "Kode/No Rangka",
                        cls: "span4 full",
                        items: [
                            { name: "ChassisCode", cls: "span5", disable: true },
                            { name: "ChassisNo", cls: "span3", disable: true }
                        ]
                    },
                      {
                          type: "controls",
                          text: "Kode/No Mesin",
                          cls: "span4 full",
                          items: [
                              { name: "EngineCode", cls: "span5", disable: true },
                              { name: "EngineNo", cls: "span3", disable: true }
                          ]
                      },
                      { name: "ColourCode", text: "Warna", cls: "span6", show: false, disable: "isStatus" },
                      { name: "ColourDescription", text: "Warna", cls: "span6", required: true, validasi: "required", disable: "isStatus" },
                      { name: "Signature", text: "Signature", cls: "span3", type: "select2", datasource: "Signature", disable: "isStatus" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omSalesKwitansiUnit");
    }



});