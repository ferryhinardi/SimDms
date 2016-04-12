var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var isDetail = false;

"use strict";

function ModelYearController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model",
            manager: spSalesManager,
            query: "MstModelPerlengkapanBrowse",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Des" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCode = data.SalesModelCode;
                me.detail.SalesModelCode = data.SalesModelCode;
                me.data.SalesModelDesc = data.SalesModelDesc;
                me.Apply();
                me.loadDetail(data);
                $('#SalesModelCode').attr('disabled', 'disabled');
                $('#btnSalesModelCode').attr('disabled', 'disabled');
            }
        });
    }

    me.SalesModelCodeLookup = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code" },
                { field: "SalesModelDesc", title: "Sales Model Des" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelCode = data.SalesModelCode;
                me.detail.SalesModelCode = data.SalesModelCode;
                me.data.SalesModelDesc = data.SalesModelDesc;
                me.Apply();
                me.loadDetail(data);
                if (me.detail.SalesModelCode != undefined) {
                    $('#btnAddColour').attr('disabled', false);
                    $('#btnCancelModel').attr('disabled', false);
                }
                else {
                    $('#btnAddColour').attr('disabled', true);
                    $('#btnCancelModel').attr('disabled', true);
                }
                $('#SalesModelCode').attr('disabled', 'disabled');
            }
        });
    }

    me.PerlengkapanCodeLookup = function () {
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
            if (data != null) {
                me.detail.SalesModelCode = me.data.SalesModelCode;
                me.detail.PerlengkapanCode = data.PerlengkapanCode;
                me.detail.PerlengkapanName = data.PerlengkapanName;
                me.Apply();
                $('#PerlengkapanCode').attr('disabled', 'disabled');
                if (me.detail.SalesModelCode != undefined) {
                    $('#btnAddColour').attr('disabled', false);
                    $('#btnCancelModel').attr('disabled', false);
                }
                else {
                    $('#btnAddColour').attr('disabled', true);
                    $('#btnCancelModel').attr('disabled', true);
                }
                $http.post('om.api/MstModelPerlengkapan/ModelPerlengkapanCode', me.detail).
                success(function (data, status, headers, config) {
                   if (data.success) {
                       me.detail.Quantity = data.data.Quantity;
                       me.detail.Remark = data.data.Remark;
                       var stat;
                       if (data.data.Status == 1) { stat = true; } else { stat = false; }
                       me.detail.Status = stat;
                       me.detail.oid = true;
                       me.Apply();
                   }
                }).
                error(function (data, status, headers, config) {
                   alert('error');
                });
            }
        });
    }

    me.loadDetail = function (data) {

        $http.post('om.api/MstModelPerlengkapan/ModelPerlengkapanDetailsLoad?SalesModelCode=' + data.SalesModelCode).
           success(function (data, status, headers, config) {
               me.grid.detail = data;
               me.loadTableData(me.grid1, me.grid.detail);
               if (me.grid.detail != "") {
                   me.isDetail = true;
                   me.isLoadData = true;
               }
               else {
                   me.isDetail = false;
                   me.isLoadData = false;
               }
           }).
           error(function (e, status, headers, config) {
               //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               console.log(e);
           });
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxperlengkapancode",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "PerlengkapanCode", header: "Kode Perlengkapan", width: 150 },
                { id: "PerlengkapanName", header: "Nama Perlengkapan", width: 300 },
                { id: "Quantity", header: "Jumlah", width: 100, format: webix.i18n.numberFormat, css: { "text-align": "right" }},
                //{ id: "Status", header: "Status", width: 100 },
                { id: "Remark", header: "Keterangan", width: 650 }
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        //alert(this.getItem(me.grid1.getSelectedId().id.Quantity));
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        me.Apply();
                        $('#PerlengkapanCode').attr('disabled', 'disabled');
                        $('#btnPerlengkapanCode').attr('disabled', 'disabled');
                        $('#btnCancelModel').removeAttr('disabled');
                    }
                }
            }
        });
    }


    me.AddColour = function () {
        me.detail.SalesModelCode = me.data.SalesModelCode;
        $http.post('om.api/MstModelPerlengkapan/save2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success(data.message);
                    me.detail = {};
                    me.clearTable(me.grid1);
                    me.grid.detail = data.result;
                    me.loadTableData(me.grid1, me.grid.detail);
                    $('#PerlengkapanCode').removeAttr('disabled');
                    $('#btnPerlengkapanCode').removeAttr('disabled');
                    $('#btnAddColour').attr('disabled', 'disabled');
                    $('#btnCancelModel').attr('disabled', 'disabled');
                    me.detail.Status = true;
                    me.isDetail = true;
                    me.isLoadData = true;
                } else {
                    MsgBox("You have to fill Sales Model Code", MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e);
            });
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstModelPerlengkapan/Delete', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.init();
                        me.clearTable(me.grid1);
                        Wx.Success("Data deleted...");

                        $('#btnAddColour').attr('disabled', 'disabled');
                        $('#btnCancelModel').attr('disabled', 'disabled');
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

    me.delete2 = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstModelPerlengkapan/Delete2', me.detail).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail = {};
                        //me.clearTable(me.grid1);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = data.result;
                        me.loadTableData(me.grid1, me.grid.detail);
                        me.detail.Status = true;
                        $('#PerlengkapanCode').removeAttr('disabled');
                        $('#btnPerlengkapanCode').removeAttr('disabled');
                        $('#btnAddColour').attr('disabled', 'disabled');
                        $('#btnCancelModel').attr('disabled', 'disabled');
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

    $("[name='SalesModelCode']").on('blur', function () {
        if (me.data.SalesModelCode != null) {
            $http.post('om.api/MstModelPerlengkapan/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.detail.SalesModelCode = data.data.SalesModelCode;
                       me.data.SalesModelDesc = data.data.SalesModelDesc;
                       me.Apply();
                       me.loadDetail(me.data);
                       $('#SalesModelCode').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.SalesModelCode = "";
                       me.data.SalesModelDesc = "";
                       me.SalesModelCodeLookup();
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    $("[name='PerlengkapanCode']").on('blur', function () {
        if (me.detail.PerlengkapanCode != null) {
            me.detail.SalesModelCode = me.data.SalesModelCode;
            $http.post('om.api/MstModelPerlengkapan/PerlengkapanCode', me.detail).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.detail.PerlengkapanCode = data.data.PerlengkapanCode;
                       me.detail.PerlengkapanName = data.data.PerlengkapanName;
                       me.Apply();
                       $('#PerlengkapanCode').attr('disabled', 'disabled');
                       $('#btnAddColour').removeAttr('disabled');
                       $('#btnCancelModel').removeAttr('disabled');
                   }
                   else {
                       me.detail.PerlengkapanCode = "";
                       me.detail.PerlengkapanName = "";
                       me.PerlengkapanCodeLookup();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });

            $http.post('om.api/MstModelPerlengkapan/ModelPerlengkapanCode', me.detail).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.detail.Quantity = data.data.Quantity;
                       me.detail.Remark = data.data.Remark;
                       var stat;
                       if (data.data.Status == 1) { stat = true; } else { stat = false; }
                       me.detail.Status = stat;
                       me.detail.oid = true;
                       me.Apply();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    me.CloseModel = function () {
        me.detail = {};
        me.grid1.clearSelection();
        me.detail.remark = "";
        me.detail.Status = true;
        $('#PerlengkapanCode').removeAttr('disabled');
        $('#btnPerlengkapanCode').removeAttr('disabled');
        $('#btnAddColour').attr('disabled', 'disabled');
        $('#btnCancelModel').attr('disabled', 'disabled');
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.detail = {};
        me.hasChanged = false;
        me.isDetail = false;
        me.detail.Status = true;
        $('#SalesModelCode').removeAttr('disabled');
        $('#btnSalesModelCode').removeAttr('disabled');
        $('#PerlengkapanCode').removeAttr('disabled');
        $('#btnPerlengkapanCode').removeAttr('disabled');
        $('#btnAddColour').attr('disabled', 'disabled');
        $('#btnCancelModel').attr('disabled', 'disabled');

    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Model Perlengkapan",
        xtype: "panels",
        toolbars: [
            { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
            { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isDetail && isLoadData", click: "delete()" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlModelYear",
                title: "Model Perlengkapan",
                items: [

                         {
                             text: "Sales Model Code",
                             type: "controls",
                             required: true,
                             items: [
                                 { name: "SalesModelCode", model: "data.SalesModelCode", cls: "span2", placeHolder: "Sales Model Code", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCodeLookup()", required: true, validasi: "required" },
                                 { name: "SalesModelDesc", cls: "span4", readonly: true, required: true },
                                 
                             ]
                         },
                         { type: "hr" },
                         {
                             text: "Kode Perlengkapan",
                             type: "controls",
                             required: true,
                             items: [
                                 { name: "PerlengkapanCode", model: "detail.PerlengkapanCode", cls: "span2", placeHolder: "Kode Perlengkapan", type: "popup", btnName: "btnPerlengkapanCode", click: "PerlengkapanCodeLookup()", required: true, validasi: "required" },
                                 { name: "PerlengkapanName", model: "detail.PerlengkapanName", cls: "span4", readonly: true, required: true },
                             ]
                         },
                         { name: "Quantity", model: "detail.Quantity", text: "Jumlah", cls: "span2 number-int full", maxlength: 6, value: 0, required: true },
                         { name: "Remark ", model: "detail.Remark", text: "Keterangan", cls: "span4", maxlength: 100 },
                         { name: "Status", model: "detail.Status", text: "Status", type: "x-switch", cls: "span4" },
                         {
                             type: "buttons",
                             items: [
                                     { name: "btnAddColour", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddColour()", show: "detail.oid === undefined", disable: true },
                                     { name: "btnUpdateColour", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddColour()", show: "detail.oid !== undefined" },
                                     { name: "btnDeleteColour", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                     { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined || detail.oid === undefined", disable: true }
                             ]
                         },
                ]
            },
            {
                name: "wxperlengkapancode",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("ModelYearController");
    }
});