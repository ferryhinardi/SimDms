"use strict"
var status = 0;

function omPerlengkapanAdjstController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.PerlengkapanCode = function () {
        var lookup = Wx.blookup({
            name: "PerlengkapanCodeLookup",
            title: "Kode perlengkapan",
            manager: spSalesManager,
            query: "PerlengkapanCodeLookup",
            defaultSort: "PerlengkapanCode asc",
            columns: [
                { field: "PerlengkapanCode", title: "Kode Perlengkapan" },
                { field: "PerlengkapanName", title: "Nama Perlengkapan" }
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.PerlengkapanCode = data.PerlengkapanCode;
            me.detail.PerlengkapanName = data.PerlengkapanName;
            me.Apply();
        });
    }

    me.printPreview = function () {
        $http.post('om.api/PerlengkapanAdjust/preprint', me.data)
       .success(function (e) {
           if (e.success) {
               $('#AdjustStatus').html(e.Status);
               if (e.stat == "1") { $('#btnApprove').removeAttr('disabled'); }
               var sizeType = $('input[name=sizeType]:checked').val() === 'full';

               var ReportId = 'OmRpPurTrn006';
               var par = me.data.AdjustmentNo + ',' + me.data.AdjustmentNo + ',' + e.stat;
               var rparam = 'Print Perlengkapan Adjusment'
               
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
        $http.post('om.api/PerlengkapanAdjust/Approve', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    $('#AdjustStatus').html(data.status);
                    status = data.Result;
                    me.isStatus = status == 2;
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

    me.save = function (e, param) {
        $http.post('om.api/PerlengkapanAdjust/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    if (me.data.AdjustmentNo == null) {
                        me.data.AdjustmentNo = data.data.AdjustmentNo;
                        me.saveData();
                    }
                    Wx.Success("Data saved...");
                    me.startEditing();
                    $('#AdjustStatus').html(data.status);
                    $('#pnlDetailAdjustment').show();
                    $('#wxdetailAdjustment').show();
                    me.griddetailAdjust.adjust();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.AddAdjust = function (e, param) {
        if (me.detail.PerlengkapanCode == undefined) {
            me.PerlengkapanCode();
        }
        else {
            $http.post('om.api/PerlengkapanAdjust/SaveDetail', { model: me.data, detailModel: me.detail }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.clearTable(me.griddetailAdjust);
                        me.loadTableData(me.griddetailAdjust, data.result);
                        me.detail = {};
                        $('#AdjustStatus').html(data.Status);
                        $('#btnAddAdjust').show();
                        $('#btnUpdateAdjust').hide();
                        $('#btnDeleteAdjust').hide();
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }
    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/PerlengkapanAdjust/Delete', { model: me.data }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        $('#AdjustStatus').html(data.Status);
                        $('#pnlDetailAdjustment').hide();
                        $('#wxdetailAdjustment').hide();
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

    me.DeleteAdjust = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/PerlengkapanAdjust/DeleteDetail', { model: me.data, detailModel: me.detail }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail = {};
                        me.clearTable(me.griddetailAdjust);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = data.result;
                        me.loadTableData(me.griddetailAdjust, me.grid.detail);
                        me.detail = {};
                        $('#btnAddAdjust').show();
                        $('#btnUpdateAdjust').hide();
                        $('#btnDeleteAdjust').hide();
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
            name: "PerlengkapanAdjustBrowse",
            title: "Perlengkapan Adjustment",
            manager: spSalesManager,
            query: "PerlengkapanAdjustBrowse",
            defaultSort: "AdjustmentNo desc",
            columns: [
                { field: "AdjustmentNo", title: "No. Adjustment" },
                {
                    field: "AdjustmentDate", title: "Tgl. Adjustment",
                    template: "#= (AdjustmentDate == undefined) ? '' : moment(AdjustmentDate).format('DD MMM YYYY') #"
                },
                { field: "RefferenceNo", title: "Referensi" },
                { field: "Remark", title: "Keterangan" },
                { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                $('#AdjustStatus').html(data.Status);
                me.data.isActiveYear = true;
                me.lookupAfterSelect(data);
                me.loadDetail(data);
                status = data.Stat;
                me.isStatus = status == 2;
                switch (data.Stat) {
                    case "1":
                        me.isApprove = false;
                        $('#btnApprove').removeAttr('disabled');
                        $('#pnlDetailAdjustment').show();
                        $('#wxdetailAdjustment').show();
                        $('#btnAddAdjust').show();
                        break;
                    case "2":
                        $('#btnAddAdjust').show();
                        $('#btnAddAdjust').attr('disabled', true);
                        $('#btnCancelAdjust').attr('disabled', true);
                        $('#pnlDetailAdjustment').show();
                        $('#wxdetailAdjustment').show();
                        break;
                    case "3":
                        $('#RefferenceNo').attr('disabled', true);
                        $('#Remark').attr('disabled', true);
                        break;
                    default:
                        me.isApprove = false;
                        $('#btnApprove').removeAttr('disabled');
                        $('#pnlDetailAdjustment').show();
                        $('#wxdetailAdjustment').show();
                        $('#btnAddAdjust').show();
                }
                me.Apply();
            }
        });
    };

    me.loadDetail = function (data) {
        $http.post('om.api/PerlengkapanAdjust/DetailAdjustment', data)
               .success(function (e) {
                   if (e.success) {
                       me.loadTableData(me.griddetailAdjust, e.grid);
                       me.griddetailAdjust.adjust();
                   }
                   else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });
    }

    me.griddetailAdjust = new webix.ui({
        container: "wxdetailAdjustment",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "PerlengkapanName", header: "Perlengkapan", width: 200 },
            { id: "Quantity", header: "Jumlah", width: 100 },
            { id: "Remark", header: "Keterangan", width: 750 }
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetailAdjust.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.griddetailAdjust.getSelectedId().id);
                    switch (status) {
                        case "1":
                            $('#btnCancelAdjust').removeAttr('disabled');
                            $('#btnUpdateAdjust').show();
                            $('#btnDeleteAdjust').show();
                            $('#btnAddAdjust').hide();
                            break;
                        case "2":
                            $('#btnAddAdjust').attr('disabled', true);
                            $('#btnCancelAdjust').attr('disabled', true);
                            break;
                        case "3":
                            break;
                        default:
                            $('#btnCancelAdjust').removeAttr('disabled');
                            $('#btnUpdateAdjust').show();
                            $('#btnDeleteAdjust').show();
                            $('#btnAddAdjust').hide();
                    }
                    me.detail.oid = me.griddetailAdjust.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    webix.event(window, "resize", function () {
        me.griddetailAdjust.adjust();
    });

    me.CancelAdjust = function () {
        me.detail = {};
        me.griddetailAdjust.clearSelection();
        $('#btnUpdateAdjust').hide();
        $('#btnDeleteAdjust').hide();
        $('#btnAddAdjust').show();
        $('#btnCancelAdjust').attr('disabled', true);

    }

    me.initialize = function () {
        me.isStatus = false;

        me.data.isActiveYear = true;
        me.detail = {};
        me.clearTable(me.griddetailAdjust);

        me.status = "NEW";
        $('#AdjustStatus').html(me.status);
        $('#AdjustStatus').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });

        $('#pnlDetailAdjustment').hide();
        $('#wxdetailAdjustment').hide();
        $('#btnApprove').attr('disabled', true)

        me.data.AdjustmentDate = me.now();

        if (me.data.isActiveYear == true) {
            me.data.RefferenceDate = me.now();
        }
        else {
            me.data.RefferenceDate = "";
        }

        $('#btnUpdateAdjust').hide();
        $('#btnDeleteAdjust').hide();

        me.isPrintAvailable = true;
        me.isApprove = true;
        me.isCancel = false;

        me.griddetailAdjust.adjust();


    }
    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Perlengkapan Adjustment",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "!isApprove", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData ", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlStatus",
                items: [
                { name: "AdjustStatus", text: "", cls: "span4 right", readonly: true, type: "label" },
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
                name: "pnlAdjustment",
                items: [
                    { name: "AdjustmentNo", text: "No. Adjust", cls: "span4", readonly: true, placeHolder: 'PAD/XX/YYYYYY' },
                    { name: "AdjustmentDate", text: "Tgl. Adjust", cls: "span4", type: "ng-datepicker", readonly: true },
                    { name: "RefferenceNo", text: "No. Reff", cls: "span4", disable: "isStatus" },
                    {
                        text: "Tgl. Reff",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "RefferenceDate", placeHolder: "Tgl. Reff", cls: "span6", type: 'ng-datepicker', disable: "data.isActiveYear === false || , isStatus" },
                            { name: 'isActiveYear', type: 'x-switch', cls: "span2", float: 'left' },

                        ]
                    },
                    { name: "Remark", cls: "span8", text: "Keterangan", disable: "isStatus" },
                ]
            },
            {
                name: "pnlDetailAdjustment",
                title: "Detail Perlengkapan Adjustment",
                items: [
                     {
                         text: "Perlengkapan",
                         type: "controls",
                         required: true,
                         items: [
                             { name: "PerlengkapanCode", model: "detail.PerlengkapanCode", cls: "span2", placeHolder: "Kode Perlengkapan", readonly: true, type: "popup", click: "PerlengkapanCode()", disable: "isStatus" },
                             { name: "PerlengkapanName", model: "detail.PerlengkapanName", cls: "span6", placeHolder: "Nama Perlengkapan", readonly: true, disable: "isStatus" }
                         ]
                     },
                    { name: "Quantity", text: "Jumlah", model: "detail.Quantity", cls: "span2 number-int full", maxlength: 6, value: 0, required: true, disable: "isStatus" },
                    { name: "Remark1", model: "detail.Remark", text: "Keterangan", disable: "isStatus" },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddAdjust", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddAdjust()", disable: "isStatus" },
                                { name: "btnUpdateAdjust", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddAdjust()", disable: "isStatus" },
                                { name: "btnDeleteAdjust", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteAdjust()", disable: "isStatus" },
                                { name: "btnCancelAdjust", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CancelAdjust()", disable: "isStatus || me.detail.PerlengkapanCode == undefined" }
                        ]
                    },
                ]
            },
            {
                name: "wxdetailAdjustment",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omPerlengkapanAdjstController");
    }



});