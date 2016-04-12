"use strict"
var status = 0;

function omReturnController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.optionHPPNo = function () {
        $("#btnNo b:first").attr("class", "fa fa-dot-circle-o");
        $("#btnReff b:first").attr("class", "fa fa-circle-o");
        $('#btnHPPNo').removeAttr('disabled');
        $('#btnRefferenceFakturPajakNo').attr('disabled', 'disabled');
    };

    me.optionReffInvNo = function () {
        $("#btnNo b:first").attr("class", "fa fa-circle-o");
        $("#btnReff b:first").attr("class", "fa fa-dot-circle-o");
        $('#btnHPPNo').attr('disabled', 'disabled');
        $('#btnRefferenceFakturPajakNo').removeAttr('disabled');
    };

    me.HPPNo = function () {
        var lookup = Wx.blookup({
            name: "HPPNoLookup",
            title: "No HPP",
            manager: spSalesManager,
            query: "HPPNoLookup",
            defaultSort: "HPPNo asc",
            columns: [
                { field: "HPPNo", title: "No HPP" },
                { field: "HPPDate", title: "Tgl HPP", template: "#= (HPPDate == undefined) ? '' : moment(HPPDate).format('DD MMM YYYY') #" },
                { field: "PONo", title: "No PO" },
                { field: "SupplierName", title: "Pemasok" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.HPPNo = data.HPPNo;
                me.data.RefferenceFakturPajakNo = data.RefferenceFakturPajakNo;
                me.Apply();
            }
        });
    };

    me.RefferenceFakturPajakNo = function () {
        var lookup = Wx.blookup({
            name: "HPPNoLookup",
            title: "Invoice",
            manager: spSalesManager,
            query: "HPPNoLookup",
            defaultSort: "RefferenceFakturPajakNo asc",
            columns: [
                { field: "RefferenceFakturPajakNo", title: "No Fak. Pajak" },
                { field: "RefferenceFakturPajakDate", title: "Tgl Fak. Pajak", template: "#= (RefferenceFakturPajakDate == undefined) ? '' : moment(RefferenceFakturPajakDate).format('DD MMM YYYY') #" },
                { field: "PONo", title: "No PO" },
                { field: "HPPNo", title: "No HPP" },
                { field: "SupplierName", title: "Pemasok" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.RefferenceFakturPajakNo = data.RefferenceFakturPajakNo;
                me.data.HPPNo = data.HPPNo;
                me.Apply();
            }
        });
    };

    me.BPUNo = function () {
        var lookup = Wx.blookup({
            name: "BPUNoLookup",
            title: "BPU No",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('BPUNoLookup').withParameters({ReturnNo: me.data.ReturnNo, HPPNo: me.data.HPPNo}),
            defaultSort: "BPUNo asc",
            columns: [
                { field: "BPUNo", title: "No. BPU"},
                {
                    field: "BPUDate", title: "Tgl BPU",
                    template: "#= moment(BPUDate).format('DD MMM YYYY') #"
                },
                { field: "SupplierCode", title: "Pemasok" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail1.BPUNo = data.BPUNo;
                me.Apply();
            }
        });
    };

    me.SalesModelCode = function () {
        var lookup = Wx.blookup({
            name: "ModelCodeLookup",
            title: "Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('ModelCodeLookup').withParameters({ HPPNo: me.data.HPPNo, BPUNo: me.detail1.BPUNo }),
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model Code" },
                { field: "SalesModelDesc", title: "Keterangan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail2.SalesModelCode = data.SalesModelCode;
                me.Apply();
            }
        });
    };

    me.SalesModelYear = function () {
        var lookup = Wx.blookup({
            name: "ModelYearLookup",
            title: "Model Year",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('ModelYearLookup').withParameters({ HPPNo: me.data.HPPNo, BPUNo: me.detail1.BPUNo, SalesModelCode: me.detail2.SalesModelCode }),
            defaultSort: "SalesModelYear asc",
            columns: [
                { field: "SalesModelYear", title: "Model Year" },
                { field: "SalesModelDesc", title: "Keterangan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail2.SalesModelYear = data.SalesModelYear;
                me.Apply();
            }
        });
    };

    me.ChassisNo = function () {
        var lookup = Wx.blookup({
            name: "ChassisCodeLookup",
            title: "Chassis",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('ChassisCodeLookup').withParameters({ HPPNo: me.data.HPPNo, BPUNo: me.detail1.BPUNo, SalesModelCode: me.detail2.SalesModelCode, SalesModelYear: me.detail2.SalesModelYear }),
            defaultSort: "ChassisCode asc",
            columns: [
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "ChassisNo", title: "No Rangka" },
                { field: "EngineCode", title: "Kode Mesin" },
                { field: "EngineNo", title: "No Mesin" },
                { field: "ColourCode", title: "Warna" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail2.ChassisCode = data.ChassisCode;
                me.detail2.ChassisNo = data.ChassisNo;
                me.detail2.EngineCode = data.EngineCode;
                me.detail2.EngineNo = data.EngineNo;
                me.detail2.ColourCode = data.ColourCode;
                me.detail2.ReturnSeq = data.HPPSeq;
                me.Apply();
            }
        });
    };

    me.approve = function (e, param) {
        $http.post('om.api/PurchaseReturn/Approve', me.data).
        success(function (data, status, headers, config) {
            if (data.success) {
                $('#ReturStatus').html(data.Status);
                status = data.Status;
                me.isStatus = status == 2;
                me.data.Stat = me.isStatus;
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

    me.printPreview = function () {
        $http.post('om.api/PurchaseReturn/preprint', me.data)
       .success(function (e) {
           if (e.success) {
               $('#ReturStatus').html(e.Status);
               if (e.stat == "1") { $('#btnApprove').removeAttr('disabled'); }
               BootstrapDialog.show({
                   message: $(
                       '<div class="container">' +
                       '<div class="row">' +

                       '<input type="radio" name="sizeType" id="sizeType1" value="full" checked>&nbsp Print Satu Halaman</div>' +

                       '<div class="row">' +

                       '<input type="radio" name="sizeType" id="sizeType2" value="half">&nbsp Print Setengah Halaman</div>'),
                   closable: false,
                   draggable: true,
                   type: BootstrapDialog.TYPE_INFO,
                   title: 'Print',
                   buttons: [{
                       label: ' Print',
                       cssClass: 'btn-primary icon-print',
                       action: function (dialogRef) {
                           me.Print();
                           dialogRef.close();
                       }
                   }, {
                       label: ' Cancel',
                       cssClass: 'btn-warning icon-remove',
                       action: function (dialogRef) {
                           dialogRef.close();
                       }
                   }]
               });
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.Print = function () {
        var sizeType = $('input[name=sizeType]:checked').val() === 'full';

        var ReportId = sizeType ? 'OmRpPurTrn008' : 'OmRpPurTrn008A';
        var par = me.data.ReturnNo + ',' + me.data.ReturnNo ;
        var rparam = 'Print Purchase Return'

        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.saveData = function (e, param) {
        $http.post('om.api/PurchaseReturn/Save', me.data).
            success(function (result, status, headers, config) {
                if (result.success) {
                    $('#ReturnStatus').html(result.Status);
                    $('#pnlDetailBPU').show();
                    $('#wxdetailbpu').show();
                    me.data = result.data;
                    $('#ReturStatus').html("Open");
                    Wx.Success("Data saved...");
                    me.isLoadData = true;
                    me.hasChanged = false;

                    me.griddetaiBPU.adjust();
                } else {
                    MsgBox(result.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.AddBPU = function (e, param) {
        $http.post('om.api/PurchaseReturn/SaveBPU', { model: me.data, bpuModel: me.detail1 }).
        success(function (data, status, headers, config) {
            if (data.success) {
                $('#pnlDetailBPU').show();
                $('#wxdetailbpu').show();
                Wx.Success("Data saved...");
                me.clearTable(me.griddetaiBPU);
                me.loadTableData(me.griddetaiBPU, data.result);
                me.detail1 = {};

                $('#btnUpdateBPU').hide();
                $('#btnDeleteBPU').hide();
                $('#btnAddBPU').show();

                if (data.IsChangeStatus) {
                    $('#ReturStatus').html("Open");
                }
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    };

    me.AddModel = function (e, param) {
        $http.post('om.api/PurchaseReturn/SaveModel', { model: me.data, bpuModel: me.detail1, detailModel: me.detail2 }).
        success(function (result, status, headers, config) {
            if (result.success) {
                $('#pnlDetailModel').show();
                $('#wxdetailmodel').show();
                Wx.Success("Data saved...");
                me.clearTable(me.griddetaiModel);
                me.loadTableData(me.griddetaiModel, result.data);
                me.detail2 = {};

                $('#btnUpdateModel').hide();
                $('#btnDeleteModel').hide();
                $('#btnAddModel').show();

                //if (result.BPUType == "0") {
                //    me.manualChasisNo = true;
                //}
                //else {
                //    me.manualChasisNo = false;
                //}

                if (result.IsChangeStatus) {
                    $('#ReturStatus').html("Open");
                }
            } else {
                MsgBox(result.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/PurchaseReturn/Delete', { model: me.data }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        $('#ReturStatus').html(data.Status);
                        $('#pnlDetailBPU').hide();
                        $('#pnlDetailModel').hide();
                        $('#wxdetailbpu').hide();
                        $('#wxdetailmodel').hide();
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

    me.DeleteBPU = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/PurchaseReturn/DeleteBPU', { model: me.data, bpuModel: me.detail1, listDetailModel: me.detailModelData }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail1 = {};
                        me.clearTable(me.griddetaiBPU);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = data.result;
                        me.loadTableData(me.griddetaiBPU, me.grid.detail);
                        $('#pnlDetailModel').hide();
                        $('#wxdetailmodel').hide();

                        $('#btnUpdateBPU').hide();
                        $('#btnDeleteBPU').hide();
                        $('#btnAddBPU').show();
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

    me.DeleteModel = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/PurchaseReturn/DeleteModel', { model: me.data, bpuModel: me.detail1, detailModel: me.detail2 }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail2 = {};
                        me.clearTable(me.griddetaiModel);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = data.result;
                        me.loadTableData(me.griddetaiModel, me.grid.detail);

                        $('#btnUpdateModel').hide();
                        $('#btnDeleteModel').hide();
                        $('#btnAddModel').show();

                        //if (data.BPUType == "0") {
                        //    me.manualChasisNo = true;
                        //}
                        //else {
                        //    me.manualChasisNo = false;
                        //}

                        if (data.IsChangeStatus) {
                            $('#ReturStatus').html("Open");
                        }
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

    me.browse = function () {
        me.cancelOrClose();
        var lookup = Wx.blookup({
            name: "PurchaseReturnBrowse",
            title: "Purchase Return",
            manager: spSalesManager,
            query: "PurchaseReturnBrowse",
            defaultSort: "ReturnNo desc",
            columns: [
                { field: "ReturnNo", title: "No. Return" },
                {
                    field: "ReturnDate", title: "Tgl. Return",
                    template: "#= (ReturnDate == undefined) ? '' : moment(ReturnDate).format('DD MMM YYYY') #"
                },
                { field: 'RefferenceNo', title: 'No. Ref' },
                {
                    field: 'RefferenceDate', title: 'Tgl.Ref',
                    template: "#= (RefferenceDate == undefined) ? '' : moment(RefferenceDate).format('DD MMM YYYY') #"
                },
                { field: "HPPNo", title: "No. HPP" },
                { field: "RefferenceFakturPajakNo", title: "No.Ref.Inv" },
                { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                $('#btnHPPNo').attr('disabled', true);

                $('#ReturStatus').html(data.Status);
                me.lookupAfterSelect(data);
                status = data.Stat;
                me.isStatus = status == 2;
                me.loadDetail(data);
                switch (data.Stat) {
                    case "1":
                        me.isApprove = false;
                        $('#btnApprove').removeAttr('disabled');
                        $('#pnlDetailBPU').show();
                        $('#wxdetailbpu').show();
                        $('#btnAddBPU').show();
                        break;
                    case "2":
                        $('#btnAddBPU').show();
                        $('#btnAddBPU').attr('disabled', true);
                        $('#btnCancelBPU').attr('disabled', true);
                        $('#pnlDetailBPU').show();
                        $('#wxdetailbpu').show();
                        break;
                    case "3":
                        $('#Remark').attr('disabled', true);
                        break;
                    default:
                        me.isApprove = false;
                        $('#btnApprove').removeAttr('disabled');
                        $('#pnlDetailBPU').show();
                        $('#wxdetailbpu').show();
                        $('#btnAddBPU').show();
                }

                setTimeout(function () {
                    me.isPrintAvailable = true;
                    me.isLoadData = true;
                    $('#btnPrintPreview').removeClass("ng-hide");
                },2000);
            }
        });
    };

    me.loadDetail = function (data) {
        $http.post('om.api/PurchaseReturn/DetailBPU', data)
               .success(function (e) {
                   if (e.success) {
                       me.loadTableData(me.griddetaiBPU, e.grid);
                       me.griddetaiBPU.adjust();
                       me.griddetaiModel.adjust();
                   }
                   else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });
    }

    me.griddetaiBPU = new webix.ui({
        container: "wxdetailbpu",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "BPUNo", header: "No. BPU", width: 200 },
            { id: "Remark", header: "Keterangan", width: 850 }
        ],
        on: {
            onSelectChange: function () {
                me.detailModelData = [];
                if (me.griddetaiBPU.getSelectedId() !== undefined) {
                    me.detail1 = this.getItem(me.griddetaiBPU.getSelectedId().id);
                    me.detail1.oid = me.griddetaiBPU.getSelectedId();
                    me.Apply();

                    var detail = this.getItem(me.griddetaiBPU.getSelectedId().id);
                    var datas = {
                        "ReturnNo": detail.ReturnNo,
                        "BPUNo": detail.BPUNo
                    }
                    $http.post('om.api/PurchaseReturn/DetailModel', datas)
                    .success(function (data, status, headers, config) {
                        switch (status) {
                            case "1":
                                $('#pnlDetailModel').show();
                                $('#wxdetailmodel').show();
                                $('#btnUpdateBPU').show();
                                $('#btnDeleteBPU').show();
                                $('#btnAddModel').show();
                                $('#btnAddBPU').hide();
                                break;
                            case "2":
                                $('#pnlDetailModel').show();
                                $('#wxdetailmodel').show();
                                break;
                            case "3":
                                break;
                            default:
                                $('#pnlDetailModel').show();
                                $('#wxdetailmodel').show();
                                $('#btnUpdateBPU').show();
                                $('#btnDeleteBPU').show();
                                $('#btnAddModel').show();
                                $('#btnAddBPU').hide();
                        }
                        me.detailModelData = data.grid;
                        me.loadTableData(me.griddetaiModel, data.grid);
                        //me.Apply();
                    })
                    .error(function (e) {
                        me.detailModelData = [];
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.griddetaiModel = new webix.ui({
        container: "wxdetailmodel",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 200 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 200 },
            { id: "ChassisCode", header: "Kode Rangka", width: 200 },
            { id: "ChassisNo", header: "No Rangka", width: 200 },
            { id: "EngineCode", header: "Kode Mesin", width: 200 },
            { id: "EngineNo", header: "No Mesin", width: 200 },
            { id: "ColourCode", header: "Warna", width: 200 },
            { id: "Remark", header: "Keterangan", width: 300 },
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetaiModel.getSelectedId() !== undefined) {
                    me.detail2 = this.getItem(me.griddetaiModel.getSelectedId().id);
                    switch (status) {
                        case "1":
                            $('#btnUpdateModel').show();
                            $('#btnDeleteModel').show();
                            $('#btnAddBPU').hide();
                            $('#btnAddModel').hide();
                            break;
                        case "2":
                            $('#btnAddModel').attr('disabled', true);
                            $('#btnCancelModel').attr('disabled', true);
                            break;
                        case "3":
                            break;
                        default:
                            $('#btnUpdateModel').show();
                            $('#btnDeleteModel').show();
                            $('#btnAddModel').hide();
                            $('#btnAddModel').hide();
                    }
                    me.detail2.oid = me.griddetaiModel.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.CancelBPU = function () {
        me.detail1 = {};
        me.griddetaiBPU.clearSelection();
        $('#pnlDetailModel').hide();
        $('#wxdetailmodel').hide();
        $('#btnUpdateBPU').hide();
        $('#btnDeleteBPU').hide();
        $('#btnAddBPU').show();
    }

    me.CancelModel = function () {
        me.detail2 = {};
        me.griddetaiModel.clearSelection();
        $('#btnUpdateModel').hide();
        $('#btnDeleteModel').hide();
        $('#btnAddModel').show();
    }

    me.initialize = function () {
        me.isStatus = false;

        me.detail1 = {};
        me.detail2 = {};
        me.status = "NEW";
        me.optionsTrans = "Manual";
        me.Pilihan = "optionHPPNo";
        $("#btnNo b:first").attr("class", "fa fa-dot-circle-o");
        $("#btnReff b:first").attr("class", "fa fa-circle-o");
        $('#btnHPPNo').removeAttr('disabled');
        $('#btnRefferenceFakturPajakNo').attr('disabled', 'disabled');
        $('#Remark').removeAttr('disabled');
        $('#ReturStatus').html(me.status);
        $('#ReturStatus').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });
        $("#btnNo b:first, #btnReff b:first").css(
        {
            "font-size": "16px",
            "padding-top": "3px"
        });

        $('#pnlDetailBPU').hide();
        $('#pnlDetailModel').hide();
        $('#wxdetailbpu').hide();
        $('#wxdetailmodel').hide();
        $('#btnApprove').attr('disabled', true)

        me.data.ReturnDate = me.now();

        if (me.data.isActiveYear == true) {
            me.data.RefferenceDate = me.now();
        }
        else {
            me.data.RefferenceDate = "";
        }

        $('#btnUpdateBPU').hide();
        $('#btnDeleteBPU').hide();

        $('#btnUpdateModel').hide();
        $('#btnDeleteModel').hide();

        me.isPrintAvailable = true;
        me.isApprove = true;
        me.isCancel = false;
        me.data.isActiveYear = false;

        me.detailModelData = [];

    }
    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Purchase Return",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "!isApprove", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnloptions",
                items: [
                    {
                        type: "optionbuttons",
                        name: "tabpageoptionsTrans",
                        model: "optionsTrans",
                        items: [
                            { name: "Manual", text: "Manual", disable: "!allowEdit", click: "SetManual()" },
                            { name: "Upload", text: "Upload", disable: "!allowEdit", click: "SetUpload()" },
                        ]
                    }
                ]
            },
            {
                name: "pnlStatus",
                items: [
                {
                    type: "buttons", cls: "span3 left", items: [
                        {
                            name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approve()", 
                            disable: "data.Stat == 0 || data.Stat == 2 || data.Stat == 3"
                            //disable: "me.status === NEW || me.status === APPROVED || me.status === OPEN || me.status === CANCELED || me.status === FINISHED"
                        }
                    ]
                },
                { name: "ReturStatus", text: "", cls: "span4 right", readonly: true, type: "label" }
                ]
            },
            {
                name: "pnlReturn",
                items: [
                    { name: "ReturnNo", text: "No. Return", cls: "span4", readonly: true, placeHolder: 'RTP/XX/YYYYYY' },
                    { name: "ReturnDate", text: "Tgl. Return", cls: "span4", type: "ng-datepicker", readonly: true },
                    { name: "RefferenceNo", text: "No. Reff", cls: "span4", type: 'popup', readonly: true, click: "ReferenceNo()", disable: "data.optionsTrans === Manual" },
                    {
                        text: "Tgl. Reff",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "RefferenceDate", placeHolder: "Tgl. Reff", cls: "span6", type: 'ng-datepicker', disable: "data.isActiveYear === false" },
                            { name: 'isActiveYear', type: 'x-switch', cls: "span2", float: 'left' },

                        ]
                    },
                    {
                        name: "btnNo",
                        text: "No. HPP",
                        type: "controls",
                        cls: "span4",
                        items: [
                            {
                                type: "optionbuttons",
                                name: "Pilihan",
                                model: "Pilihan",
                                cls: "span1 active",
                                items: [
                                    { name: "optionHPPNo", text: "<b></b>", click: "optionHPPNo()" }
                                ]
                            },
                            { name: "HPPNo", cls: "span7", required: true, validasi: "required", readonly: true, type: "popup", click: "HPPNo()" }
                        ]
                    },
                    {
                        name: "btnReff",
                        text: "No.Ref.Inv",
                        type: "controls",
                        cls: "span4 left",
                        items: [
                            {
                                type: "optionbuttons",
                                name: "Pilihan",
                                model: "Pilihan",
                                cls: "span1",
                                items: [
                                    { name: "optionReffInvNo", text: "<b></b>", click: "optionReffInvNo()" }
                                ]
                            },
                            { name: "RefferenceFakturPajakNo", cls: "span7", readonly: true, type: "popup", click: "RefferenceFakturPajakNo()" },
                        ]
                    },
                    { name: "Remark", cls: "span8", text: "Keterangan", disable: "isStatus" },
                ]
            },
            {
                name: "pnlDetailBPU",
                title: "Detail BPU",
                items: [
                    { name: "BPUNo", model: "detail1.BPUNo", text: "No.BPU", cls: "span4", required: true, validasi: "required", type: "popup", click: "BPUNo()", disable: "isStatus" },
                    { name: "Remark1", model: "detail1.Remark", text: "Keterangan", type: "textarea", disable: "isStatus" },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddBPU", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddBPU()", disable: "isStatus" },
                                { name: "btnUpdateBPU", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddBPU()", disable: "isStatus" },
                                { name: "btnDeleteBPU", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteBPU()", disable: "isStatus" },
                                { name: "btnCancelBPU", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CancelBPU()", disable: "isStatus" }
                        ]
                    },
                ]
            },
            {
                name: "wxdetailbpu",
                xtype: "wxtable",
            },
            {
                name: "pnlDetailModel",
                title: "Detail Sales Model",
                items: [
                    { name: "SalesModelCode", model: "detail2.SalesModelCode", text: "Sales Model Code", cls: "span4 full", required: true, validasi: "required", type: "popup", click: "SalesModelCode()", disable: "isStatus" },
                    { name: "SalesModelYear", model: "detail2.SalesModelYear", text: "Sales Model Year", cls: "span4 full", required: true, validasi: "required", type: "popup", click: "SalesModelYear()", disable: "isStatus" },
                    { name: "ChassisCode", model: "detail2.ChassisCode", text: "Kode Rangka", cls: "span4", disable: "isStatus" },
                    { name: "ChassisNo", model: "detail2.ChassisNo", text: "No Rangka", cls: "span4", required: true, validasi: "required", type: "popup", click: "ChassisNo()", disable: "isStatus" },
                    { name: "EngineCode", model: "detail2.EngineCode", text: "Kode Mesin", cls: "span4", disable: "isStatus" },
                    { name: "EngineNo", model: "detail2.EngineNo", text: "No Mesin", cls: "span4", disable: "isStatus" },
                    { name: "ColourCode", model: "detail2.ColourCode", text: "Warna", cls: "span4", disable: "isStatus" },
                    { name: "Remark2", model: "detail2.Remark", text: "Keterangan", cls: "span8", disable: "isStatus" },
                    { name: "ReturnSeq", model: "detail2.ReturnSeq", text: "Keterangan", cls: "span8", show: false },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddModel()", disable: "isStatus" },
                                { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddModel()", disable: "isStatus" },
                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteModel()", disable: "isStatus" },
                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CancelModel()", disable: "isStatus" }
                        ]
                    },
                ]
            },
            {
                name: "wxdetailmodel",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omReturnController");
    }



});