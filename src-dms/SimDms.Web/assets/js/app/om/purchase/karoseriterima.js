"use strict"
var status = 0;

function omKaroseriTerimaController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $('#isTotal').on('change', function (e) {
        me.data.DPPMaterial = me.data.DPPOthers = me.data.DPPFee = me.data.PPh = me.data.PPn = 0;
        $('#PPn').val(0);
        if ($('#isTotal').prop('checked') == true) {
            me.data.Total = '0';
            $('#Total').removeAttr('disabled');
            
            $('#DPPMaterial').attr('disabled', true);
            $('#DPPOthers').attr('disabled', true);
            $('#DPPFee').attr('disabled', true);
            $('#PPh').attr('disabled', true);
            $('#PPn').attr('disabled', true);
        } else {
            me.data.Total = undefined;
            $('#Total').attr('disabled', true);
            $('#DPPMaterial').removeAttr('disabled');
            $('#DPPOthers').removeAttr('disabled');
            $('#DPPFee').removeAttr('disabled');
            $('#PPh').removeAttr('disabled');
            $('#PPn').removeAttr('disabled');
        }
        me.Apply();
    })

    $('#Total').on('blur', function (e) {
        $http.post('om.api/KaroseriTerima/ReCalculateDPPnPPN', me.data)
            .success(function (data) {
                me.data.DPPMaterial = data.data.DPPMaterial;
                me.data.PPn = data.data.PPn;
                $('#btnSave').focus();
            });
    })

    me.Tambah = function () {
        var a = $('#DPPMaterial').val();
        var b = $('#DPPFee').val();
        var c = $('#DPPOthers').val();
        a = a.split(',').join('');
        b = b.split(',').join('');
        c = c.split(',').join('');
        var d = (parseFloat(a) * 10) / 100;
        //var c = parseFloat(a) - parseFloat(b);
        var e = parseInt(a) + parseInt(b) + parseInt(c) + parseInt(d);
        $('#PPn').val(d);
        $('#Total').val(e);
    }

    $('#DPPMaterial').on('blur', function (e) {
        me.Tambah();
    })
    $('#DPPFee').on('blur', function (e) {
        me.Tambah();
    })
    $('#DPPOthers').on('blur', function (e) {
        me.Tambah();
    })

     $('#isTotal').on('change', function (e) {
        me.data.DPPFee = me.data.DPPMaterial = me.data.DPPOthers = me.data.PPn = me.data.PPh = 0;
        $('#PPn').val(0);
        if ($('#isTotal').prop('checked') == true) {
            me.data.Total = '0';
            $('#Total').prop('readonly', false);
            
            $('#DPPMaterial').attr('disabled', true);
            $('#DPPOthers').attr('disabled', true);
            $('#DPPFee').attr('disabled', true);
            $('#PPh').attr('disabled', true);
            $('#PPn').attr('disabled', true);
        } else {
            me.data.Total = undefined;
            $('#Total').prop('readonly', true);

            $('#DPPMaterial').removeAttr('disabled');
            $('#DPPOthers').removeAttr('disabled');
            $('#DPPFee').removeAttr('disabled');
            $('#PPh').removeAttr('disabled');
            $('#PPn').removeAttr('disabled');
            $('#DPPMaterial,#DPPOthers,#DPPFee,#PPh,#PPn').val('0');
        }
        me.Apply();
     })

    me.PerlengkapanCode = function () {
        var lookup = Wx.blookup({
            name: "KaroseriSPKLookup",
            title: "SPK",
            manager: spSalesManager,
            query: "KaroseriSPKLookup",
            defaultSort: "KaroseriSPKNo asc",
            columns: [
                { field: "KaroseriSPKNo", title: "No Karoseri SPK" },
                {
                    field: "KaroseriSPKDate", title: "Tanggal Karoseri SPK",
                    template: "#= (KaroseriSPKDate == undefined) ? '' : moment(KaroseriSPKDate).format('DD MMM YYYY') #"
                },
                { field: "SupplierCode", title: "Kode Supplier" },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.KaroseriSPKNo = data.KaroseriSPKNo;
            me.data.SupplierCode = data.SupplierCode;
            me.data.SalesModelCodeOld = data.SalesModelCodeOld;
            me.data.SalesModelCodeNew = data.SalesModelCodeNew;
            me.data.SalesModelYear = data.SalesModelYear;
            me.data.Total = data.Total;
            me.data.DPPMaterial = data.DPPMaterial;
            me.data.DPPOther = data.DPPOther;
            me.data.DPPFee = data.DPPFee;
            me.data.PPh = data.PPh;
            me.data.PPn = data.PPn;
            me.data.isTotal = false;
            me.Apply();
        });
    }

    me.ChassisNo = function () {
        var lookup = Wx.blookup({
            name: "KaroseriTerimaChassisCodeLookup",
            title: "Rangka",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('KaroseriTerimaChassisCodeLookup').withParameters({KaroseriSPKNo: me.data.KaroseriSPKNo}),
            defaultSort: "ChassisNo asc",
            columns: [
                { field: "ChassisNo", title: "No Rangka" },
                { field: "ChassisCode", title: "Kode Rangka" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.ChassisNo = data.ChassisNo;
            me.detail.ChassisCode = data.ChassisCode;
            me.detail.EngineNo = data.EngineNo;
            me.detail.EngineCode = data.EngineCode;
            me.detail.ColourCodeOld = data.ColourCodeOld;
            me.detail.ColourNameOld = data.ColourNameOld;
            me.detail.ColourCodeNew = data.ColourCodeNew;
            me.detail.ColourNameNew = data.ColourNameNew;
            me.Apply();
        });
    }

    me.ColourCode = function () {
        var lookup = Wx.blookup({
            name: "KaroseriTerimaColourCodeLookup",
            title: "Rangka",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('KaroseriTerimaColourCodeLookup').withParameters({ SalesModelCodeOld: me.data.SalesModelCodeOld }),
            defaultSort: "ColourCodeNew asc",
            columns: [
                { field: "ColourCodeNew", title: "No Rangka" },
                { field: "ColourNameNew", title: "Kode Rangka" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.ColourCodeNew = data.ColourCodeNew;
            me.detail.ColourNameNew = data.ColourNameNew;
            me.Apply();
        });
    }

    me.printPreview = function () {
        $http.post('om.api/KaroseriTerima/preprint', me.data)
       .success(function (e) {
           if (e.success) {
               $('#Status').html(e.Status);
               if (e.stat == "1") { $('#btnApprove').removeAttr('disabled'); }

               var ReportId = 'OmRpPurTrn012';
               var par = me.data.KaroseriTerimaNo;
               var rparam = 'Print Karoseri Terima'

               Wx.showPdfReport({
                   id: ReportId,
                   pparam: par,
                   rparam: rparam,
                   type: "devex"
               });
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.approve = function (e, param) {
        $http.post('om.api/KaroseriTerima/Approve', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    $('#Status').html(data.status);
                    status = data.Result;
                    me.isStatus = status == 2;
                    $('#btnApprove').attr('disabled', true);
                    Wx.Success("Data approved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.saveData = function (e, param) {
        $http.post('om.api/KaroseriTerima/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.data.KaroseriTerimaNo = data.data.KaroseriTerimaNo;
                    //if (me.data.KaroseriTerimaNo == null) {
                    //    me.data.KaroseriTerimaNo = data.data.KaroseriTerimaNo;
                    //    me.saveData();
                    //}
                    $('#Status').html(data.status);
                    $('#pnlDetailKaroseriTerima').show();
                    $('#wxdetailkaroseriterima').show();
                    Wx.Success("Data saved...");
                    //me.startEditing();
                    me.hasChanged = false;
                    me.isPrintAvailable = me.isPrintEnable = me.isLoadData = true;
                    //me.griddetail.adjust();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.AddDetail = function (e, param) {
        $http.post('om.api/KaroseriTerima/SaveDetail', { model: me.data, detailModel: me.detail }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.clearTable(me.griddetail);
                    me.loadTableData(me.griddetail, data.grid);
                    me.detail = {};
                    $('#btnAddDetail').show();
                    $('#btnUpdateDetail').hide();
                    $('#btnDeleteDetail').hide();

                    me.isPrintAvailable = true;
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/KaroseriTerima/Delete', { model: me.data }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.lookupAfterSelect(data.data);
                        $('#Status').html(data.data.Status);
                        //$('#pnlDetailKaroseriTerima').hide();
                        //$('#wxdetailkaroseriterima').hide();
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

    me.DeleteDetail = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/KaroseriTerima/DeleteDetail', { model: me.data, detailModel: me.detail }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail = {};
                        me.clearTable(me.griddetail);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = data.grid;
                        me.loadTableData(me.griddetail, me.grid.detail);
                        me.detail = {};
                        $('#btnAddDetail').show();
                        $('#btnUpdateDetail').hide();
                        $('#btnDeleteDetail').hide();
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

    me.checkbox = function (data) {
        if (data.Total != '0') {
            $('#isTotal').prop('checked', true);
            $('#Total').prop('readonly', false);
            me.data.DPPMaterial = 0;
            //alert((data.RefferenceFakturPajakDate).substring(0, 4));
        } else {
            $('#isTotal').prop('checked', false);
            $('#Total').prop('readonly', true);
            me.data.Total = undefined;
        }
    }

    me.browse = function () {
        me.cancelOrClose();
        var lookup = Wx.blookup({
            name: "KaroseriTerimaBrowse",
            title: "Karoseri terima",
            manager: spSalesManager,
            query: "KaroseriTerimaBrowse",
            defaultSort: "KaroseriTerimaNo desc",
            columns: [
                { field: "KaroseriTerimaNo", title: "No. Terima Karoseri" },
                {
                    field: "KaroseriTerimaDate", title: "Tgl",
                    template: "#= (KaroseriTerimaDate == undefined) ? '' : moment(KaroseriTerimaDate).format('DD MMM YYYY') #"
                },
                { field: "RefferenceInvoiceNo", title: "No.Reff" },
                { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                $('#Status').html(data.Status);
                status = data.Stat;
                me.isStatus = status == 2;
                switch (data.Stat) {
                    case "1":
                        me.isApprove = false;
                        $('#btnApprove').removeAttr('disabled');
                        $('#pnlDetailKaroseriTerima').show();
                        $('#wxdetailkaroseriterima').show();
                        $('#btnAddDetail').show();
                        break;
                    case "2":
                        $('#btnAddDetail').show();
                        $('#btnAddDetail').attr('disabled', true);
                        $('#btnCancelDetail').attr('disabled', true);
                        $('#pnlDetailKaroseriTerima').show();
                        $('#wxdetailkaroseriterima').show();
                        break;
                    case "3":
                        $('#Remark').attr('disabled', true);
                        break;
                    default:
                        me.isApprove = false;
                        $('#btnApprove').removeAttr('disabled');
                        $('#pnlDetailKaroseriTerima').show();
                        $('#wxdetailkaroseriterima').show();
                        $('#btnAddDetail').show();
                }
                me.lookupAfterSelect(data);
                me.loadDetail(data);
                if (me.data.RefferenceInvoiceDate == null) {
                    me.data.isActive1 = false;
                }
                else {
                    me.data.isActive1 = true;
                }
                if (me.data.RefferenceFakturPajakDate == null) {
                    me.data.isActive2 = false;
                }
                else {
                    me.data.isActive2 = true;
                }
                if (me.data.DueDate == null) {
                    me.data.isActive3 = false;
                }
                else {
                    me.data.isActive3 = true;
                }

                setTimeout(function () {
                    me.isPrintAvailable = me.isLoadData = me.isPrintEnable = true;
                    me.Apply();
                }, 2000);
            }
        });
    };

    me.loadDetail = function (data) {
        $http.post('om.api/KaroseriTerima/DetailKaroseriterima', data)
               .success(function (e) {
                   if (e.success) {
                       me.loadTableData(me.griddetail, e.grid);
                       me.griddetail.adjust();
                   }
                   else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });
    }

    me.griddetail = new webix.ui({
        container: "wxdetailkaroseriterima",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "ChassisNo", header: "No.Rangka", width: 200 },
            { id: "EngineNo", header: "No. Mesin", width: 200 },
            { id: "ColourCodeNew", header: "Kode Warna Baru", width: 200 },
            { id: "ColourNameNew", header: "Nama Warna baru", width: 200 },
            { id: "Remark", header: "Keterangan", width: 600 }
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetail.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.griddetail.getSelectedId().id);
                    switch (status) {
                        case "1":
                            $('#btnCancelDetail').removeAttr('disabled');
                            $('#btnUpdateDetail').show();
                            $('#btnDeleteDetail').show();
                            $('#btnAddDetail').hide();
                            break;
                        case "2":
                            $('#btnAddDetail').attr('disabled', true);
                            $('#btnCancelDetail').attr('disabled', true);
                            break;
                        case "3":
                            break;
                        default:
                            $('#btnCancelDetail').removeAttr('disabled');
                            $('#btnUpdateDetail').show();
                            $('#btnDeleteDetail').show();
                            $('#btnAddDetail').hide();
                    }
                    me.detail.oid = me.griddetail.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    webix.event(window, "resize", function () {
        me.griddetail.adjust();
    });

    me.CancelDetail = function () {
        me.detail = {};
        me.griddetail.clearSelection();
        $('#btnUpdateDetail').hide();
        $('#btnDeleteDetail').hide();
        $('#btnAddDetail').show();
        $('#btnCancelDetail').attr('disabled', true);

    }

    me.initialize = function () {
        me.isStatus = false;
        me.data.isActive1 = true;
        me.data.isActive2 = true;
        me.data.isActive3 = true;
        me.data.isTotal = true;
        $('#isTotal').prop('checked', true);
        $('#Total').removeAttr('disabled');

        me.detail = {};
        me.clearTable(me.griddetail);

        me.status = "NEW";
        $('#Status').html(me.status);
        $('#Status').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });

        $('#pnlDetailKaroseriTerima').hide();
        $('#wxdetailkaroseriterima').hide();
        $('#btnApprove').attr('disabled', true)

        me.data.KaroseriTerimaDate = me.now();
        me.data.TaxPeriod = me.now();
        me.data.DueDate = me.now();

        if (me.data.isActive1 == true) {
            me.data.RefferenceInvoiceDate = me.now();
        }
        else {
            me.data.RefferenceInvoiceDate = "";
        }

        if (me.data.isActive2 == true) {
            me.data.RefferenceFakturPajakDate = me.now();
        }
        else {
            me.data.RefferenceFakturPajakDate = "";
        }

        if (me.data.isActive3 == true) {
            me.data.DueDate = me.now();
        }
        else {
            me.data.DueDate = "";
        }

        $('#btnUpdateDetail').hide();
        $('#btnDeleteDetail').hide();

        me.isPrintAvailable = me.isLoadData = me.isPrintEnable = false;
        me.isApprove = true;
        me.isCancel = false;
        me.Apply();
    }
    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Karoseri Terima",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "!isApprove", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        //toolbars: WxButtons,
        panels: [
            {
                name: "pnlStatus",
                items: [
                { name: "Status", text: "", cls: "span4 right", readonly: true, type: "label" },
                {
                    type: "buttons", cls: "span3 left", items: [
                        {
                            name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approve()",
                            disable: "data.Stat == 0 || data.Stat == 2 || data.Stat == 3"
                        }
                    ]
                },
                ]
            },
            {
                name: "pnlKaroseriTerima",
                items: [
                    { name: "KaroseriTerimaNo", text: "No. Terima", cls: "span4", readonly: true, placeHolder: 'KRT/XX/YYYYYY' },
                    { name: "KaroseriTerimaDate", text: "Tgl. Terima", cls: "span4", type: "ng-datepicker", readonly: true },
                    { name: "RefferenceInvoiceNo", text: "No.Ref.Inv", cls: "span4", required: true, validasi: "required", disable: "isStatus" },
                    {
                        text: "Tgl.Ref.Inv",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "RefferenceInvoiceDate", placeHolder: "Tgl. Reff", cls: "span6", type: 'ng-datepicker', disable: "data.isActive1 == false || isStatus" },
                            { name: 'isActive1', type: 'x-switch', cls: "span2", float: 'left', disable: "isStatus" },

                        ]
                    },
                    { name: "RefferenceFakturPajakNo",placeHolder: "___.___-__.________", text: "No.Ref.F.Pajak", cls: "span4", type: "ng-maskedit", mask: "###.###-##.########", disable: "isStatus" },
                    {
                        text: "Tgl.Ref.F.Pajak",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "RefferenceFakturPajakDate", placeHolder: "Tgl.Ref.F.Pajak", cls: "span6", type: 'ng-datepicker', disable: "data.isActive2 == false || isStatus" },
                            { name: 'isActive2', type: 'x-switch', cls: "span2", float: 'left', disable: "isStatus" },

                        ]
                    },
                    { name: "TaxPeriod", text: "Masa pajak", cls: "span4 full", type: 'ng-monthpicker' },
                    { name: "KaroseriSPKNo", text: "No.SPK.Karoseri", cls: "span3", type: "popup", required: true, validasi: "required", click: "PerlengkapanCode()", disable: "isStatus" },
                    { name: "SupplierCode", text: "Pemasok", cls: "span5", readonly: true, disable: "isStatus" },
                    { name: "SalesModelCodeOld", cls: "span3", text: "Sales Model Lama", disable: "isStatus" },
                    { name: "SalesModelYear", cls: "span3", text: "Sales Model Year", disable: "isStatus" },
                    { name: "SalesModelCodeNew", cls: "span3", text: "Sales Model Baru", disable: "isStatus" },
                    {
                        text: "Jatuh Tempo",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "DueDate", text: "Jatuh Tempo", cls: "span3", type: 'ng-datepicker', disable: "data.isActive3 == false || isStatus" },
                            { name: 'isActive3', type: 'x-switch', cls: "span2", float: 'left', disable: "isStatus" },

                        ]
                    },
                ]
            },
            {
                name: "pnlHargaPerUnit",
                title: "Harga Per Unit",
                items: [
                    {
                        text: "Total",
                        type: "controls",
                        cls: "span4 full",
                        items: [
                             { name: 'isTotal', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "Total", cls: "span5 full number-int", value: 0, text: "Total", disable: "!isTotal" },
                               
                        ]
                    },
                    { name: "DPPMaterial", cls: "span3 number-int", value: 0, text: "DPP Material", disable: true },
                    { name: "PPh", cls: "span3 number-int", value: 0, text: "PPh", disable: true },
                    { name: "DPPOthers", cls: "span3 number-int", value: 0, text: "DPP Lain-Lain", disable: true },
                    { name: "PPn", cls: "span3 number-int", value: 0, text: "PPn", disable: true },
                    { name: "DPPFee", cls: "span3 number-int", value: 0, text: "DPP Fee", disable: true },
                    { name: "Remark", cls: "span8", text: "Keterangan", readonly: false, required: false },
                ]
            },
            {
                name: "pnlDetailKaroseriTerima",
                title: "Detail Karoseri Terima",
                items: [
                     {
                         text: "Kode/No.Rangka",
                         type: "controls",
                         cls: "span4",
                         required: true,
                         validasi: "required",
                         items: [
                             { name: "ChassisCode", model: "detail.ChassisCode", cls: "span5", readonly: true, disable: "isStatus" },
                             { name: "ChassisNo", model: "detail.ChassisNo", cls: "span3", disable: "isStatus", type: "popup", click: "ChassisNo()" }
                         ]
                     },
                     {
                         text: "Warna lama",
                         type: "controls",
                         cls: "span4",
                         items: [
                             { name: "ColourCodeOld", model: "detail.ColourCodeOld", cls: "span3", readonly: true, disable: "isStatus" },
                             { name: "ColourNameOld", model: "detail.ColourNameOld", cls: "span5", disable: "isStatus", readonly: true }
                         ]
                     },
                     {
                         text: "Kode/No.Mesin",
                         type: "controls",
                         cls: "span4",
                         items: [
                             { name: "EngineCode", model: "detail.EngineCode", cls: "span5", readonly: true, disable: "isStatus" },
                             { name: "EngineNo", model: "detail.EngineNo", cls: "span3", disable: "isStatus" }
                         ]
                     },
                     {
                         text: "Warna Baru",
                         type: "controls",
                         cls: "span4",
                         required: true,
                         validasi: "required",
                         items: [
                             { name: "ColourCodeNew", model: "detail.ColourCodeNew", cls: "span3", type: "popup", required: true, click: "ColourCode()", disable: "isStatus" },
                             { name: "ColourNameNew", model: "detail.ColourNameNew", cls: "span5", disable: "isStatus", readonly: true }
                         ]
                     },
                     { name: "Remark1", model: "detail.Remark", cls: "span8", text: "Keterangan", disable: "isStatus" },
                     {
                        type: "buttons",
                        items: [
                                { name: "btnAddDetail", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddDetail()", disable: "isStatus" },
                                { name: "btnUpdateDetail", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddDetail()", disable: "isStatus" },
                                { name: "btnDeleteDetail", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteDetail()", disable: "isStatus" },
                                { name: "btnCancelDetail", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CancelDetail()", disable: "isStatus || me.detail.PerlengkapanCode == undefined" }
                        ]
                     },
                ]
            },
            {
                name: "wxdetailkaroseriterima",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omKaroseriTerimaController");
    }



});