var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var isDetail = false;

"use strict";

function ModelColorController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Sales Model Code",
            manager: spSalesManager,
            query: "MstModelColourBrowse",
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
        me.init()
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Model",
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
                $('#SalesModelCode').attr('disabled', 'disabled');
            }
        });
    }

    me.ColourCodeLookup = function () {
        var lookup = Wx.blookup({
            name: "ColourCodeLookup",
            title: "Colour",
            manager: spSalesManager,
            query: "ColourCodeLookup",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Colour Code" },
                { field: "RefferenceDesc1", title: "Keterangan" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.ColourCode = data.RefferenceCode;
                me.detail.RefferenceDesc1 = data.RefferenceDesc1;
                me.Apply();
                $('#ColourCode').attr('disabled', 'disabled');
                console.log(me.detail.SalesModelCode);
                if (me.detail.SalesModelCode != undefined) {
                    $('#btnAddColour').attr('disabled', false);
                    $('#btnCancelModel').attr('disabled', false);
                }
                else
                {
                    $('#btnAddColour').attr('disabled', true);
                    $('#btnCancelModel').attr('disabled', true);
                }
                me.detail.SalesModelCode = me.data.SalesModelCode;
                $http.post('om.api/MstModelColor/ModelColourCode', me.detail).
                success(function (data, status, headers, config) {
                   if (data.success) {
                       me.detail.Remark = data.data.Remark;
                       var stat;
                       if (data.data.Status == 1) { stat = true; } else { stat = false; }
                       me.detail.Status = stat;
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
        
        $http.post('om.api/MstModelColor/ModelColorDetailsLoad?SalesModelCode=' + data.SalesModelCode).
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
               console.log(e);
           });
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxcolorcode",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "ColourCode", header: "Color Code", width: 100 },
                { id: "RefferenceDesc1", header: "Deskripsi Color", width: 250 },
                { id: "Remark", header: "Keterangan", width: 800 }
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined)
                    {
                        //alert(me.grid1.getSelectedId().id.columns.ColourCode);
                        me.detail.Status = me.detail.Status == "0" ? false : true;
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        //console.log(this.getItem(me.grid1.getSelectedId().id))
                        me.Apply();
                        $('#ColourCode').attr('disabled', 'disabled');
                        $('#btnColourCode').attr('disabled', 'disabled');
                        $('#btnCancelModel').removeAttr('disabled');

                    }
                }
            }
        });
    }

    me.AddColour = function () {
        me.detail.SalesModelCode = me.data.SalesModelCode;
        $http.post('om.api/MstModelColor/save2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success(data.message);
                    me.detail = {};
                    me.detail.SalesModelCode = me.data.SalesModelCode;
                    me.clearTable(me.grid1);
                    me.grid.detail = data.result;
                    me.loadTableData(me.grid1, me.grid.detail);
                    me.detail.Status = true;
                    me.isDetail = true;
                    me.isLoadData = true;
                    $('#ColourCode').removeAttr('disabled');
                    $('#btnAddColour').attr('disabled', 'disabled');
                    $('#btnCancelModel').attr('disabled', 'disabled');
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
                $http.post('om.api/MstModelColor/Delete', me.data).
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
                $http.post('om.api/MstModelColor/Delete2', me.detail).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail = {};
                        me.clearTable(me.grid1);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = data.result;
                        me.loadTableData(me.grid1, me.grid.detail);
                        me.detail.Status = true;
                        $('#ColourCode').removeAttr('disabled');
                        $('#btnColourCode').removeAttr('disabled');
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
            $http.post('om.api/MstModelColor/ModelCode', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.detail.SalesModelCode = data.data.SalesModelCode;
                       me.data.SalesModelDesc = data.data.SalesModelDesc;
                       me.Apply();
                       me.loadDetail(data.data);
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

    $("[name='ColourCode']").on('blur', function () {
        if (me.detail.ColourCode) {
            me.detail.SalesModelCode = me.data.SalesModelCode;
            $http.post('om.api/MstModelColor/ColourCode', me.detail).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.detail.RefferenceDesc1 = data.data.RefferenceDesc1;
                       $('#ColourCode').attr('disabled', 'disabled');
                       $('#btnAddColour').removeAttr('disabled');
                       $('#btnCancelModel').removeAttr('disabled');
                   }
                   else {
                       me.detail.ColourCode = "";
                       me.detail.RefferenceDesc1 = "";
                       me.ColourCodeLookup();

                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });

            $http.post('om.api/MstModelColor/ModelColourCode', me.detail).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.detail.Remark = data.data.Remark;
                       var stat;
                       if (data.data.Status == 1) { stat = true; } else { stat = false; }
                       me.detail.Status = stat;
                       me.detail.oid = true;
                       me.Apply();
                       $('#btnAddColour').removeAttr('disabled');
                       $('#btnCancelModel').removeAttr('disabled');
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
        me.detail.Status = true;
        $('#ColourCode').removeAttr('disabled');
        $('#btnColourCode').removeAttr('disabled');
        $('#btnAddColour').attr('disabled', 'disabled');
        $('#btnCancelModel').attr('disabled', 'disabled');
        
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.detail = {};
        me.hasChanged = false;
        me.isDetail = false;
        me.detail.Status = true;
        me.grid1.clearSelection();
        $('#SalesModelCode').removeAttr('disabled');
        $('#btnSalesModelCode').removeAttr('disabled');
        $('#ColourCode').removeAttr('disabled');
        $('#btnColourCode').removeAttr('disabled');
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
        title: "Model Colour",
        xtype: "panels",
        toolbars: [
            { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
            { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isDetail && isLoadData", click: "delete()" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlModelColour",
                title: "Sales Code",
                items: [

                         {
                             text: "Sales Model Code",
                             type: "controls",
                             required: true,
                             items: [
                                 { name: "SalesModelCode", model: "data.SalesModelCode", cls: "span2", placeHolder: "Sales Model Code", type: "popup", btnName: "btnSalesModelCode", click: "SalesModelCodeLookup()", required: true, validasi: "required" },
                                 { name: "SalesModelDesc", cls: "span4", readonly: true, required: true },
                                 { name: "SalesModelCode", model: "detail.SalesModelCode", cls: "span2", placeHolder: "Sales Model Code", type: "hiden" },
                             ]
                         }
                ]
            },
            {
                name: "pnlB",
                title: "Colour Code",
                items: [
                        {
                            text: "Color Code",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "ColourCode", model: "detail.ColourCode", cls: "span2", maxlength: 3, placeHolder: "Color Code", type: "popup", btnName: "btnColourCode", click: "ColourCodeLookup()", required: true, validasi: "required" },
                                { name: "RefferenceDesc1", model: "detail.RefferenceDesc1", cls: "span4", readonly: true, required: true }
                            ]
                        },
                        { name: "Remark ", model: "detail.Remark", text: "Keterangan", cls: "span4", maxlength: 100 },
                        { name: "Status", model: "detail.Status", text: "Status", type: "x-switch", cls: "span2" },

                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddColour", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddColour()", show: "detail.oid === undefined", disable: true },
                                { name: "btnUpdateColour", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddColour()", show: "detail.oid !== undefined" },
                                { name: "btnDeleteColour", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined || detail.oid == undefined", disable: true }
                        ]
                    },
                ]
            },
            {
                name: "wxcolorcode",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("ModelColorController");
    }
});