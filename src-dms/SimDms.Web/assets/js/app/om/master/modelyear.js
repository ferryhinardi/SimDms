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
            query: "MstModelYearBrowse",
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
                me.loadDetail(data);
                me.Apply();
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
                $('#SalesModelCode').attr('disabled', 'disabled');
            }
        });
    }

    me.loadDetail = function (data) {

        $http.post('om.api/MstModelYear/ModelYearDetailsLoad?SalesModelCode=' + data.SalesModelCode).
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
                { id: "SalesModelYear", header: "Year", width: 100 },
                { id: "SalesModelDesc", header: "Deskripsi", width: 250 },
                { id: "ChassisCode", header: "Deskripsi Color", width: 250 },
                { id: "Remark", header: "Keterangan", width: 650 }
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        //alert('xxx');
                        me.detail.Status = me.detail.Status == 0 ? false : true;
                        me.detail = this.getItem(me.grid1.getSelectedId().id);                 
                        me.detail.oid = me.grid1.getSelectedId();
                        //console.log(this.getItem(me.grid1.getSelectedId().id))
                        me.Apply();
                        $('#SalesModelYear').attr('disabled', 'disabled');
                    }
                }
            }
        });
    }


    me.saveData= function () {
        me.detail.SalesModelCode = me.data.SalesModelCode;
        //Wx.submit();
        $http.post('om.api/MstModelYear/save2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success(data.message);
                    me.detail = {};
                    me.clearTable(me.grid1);
                    me.grid.detail = data.result;
                    me.loadTableData(me.grid1, me.grid.detail);
                    me.detail.Status = true;
                    me.isDetail = true;
                    me.isLoadData = true;
                    $('#SalesModelYear').removeAttr('disabled');
                } else {
                    MsgBox(data.message, MSG_ERROR);
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
                $http.post('om.api/MstModelYear/Delete', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.init();
                        me.clearTable(me.grid1);
                        Wx.Success("Data deleted...");
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
                $http.post('om.api/MstModelYear/Delete2', me.detail).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail = {};
                        //me.clearTable(me.grid1);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = data.result;
                        me.loadTableData(me.grid1, me.grid.detail);
                        me.detail.Status = true;
                        $('#SalesModelYear').removeAttr('disabled');
                        me.apply();
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
            $http.post('om.api/MstModelYear/ModelCode', me.data).
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

    $("[name='SalesModelYear']").on('blur', function () {
        if (me.data.SalesModelCode != null) {
            me.detail.SalesModelCode = me.data.SalesModelCode;
            $http.post('om.api/MstModelYear/ModelYear', me.detail).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.detail.SalesModelYear = data.data.SalesModelYear;
                       me.detail.SalesModelDesc = data.data.SalesModelDesc;
                       me.detail.ChassisCode = data.data.ChassisCode;
                       me.detail.Remark = data.data.Remark;
                       var stat;
                       if (data.data.Status == 1) { stat = true; } else { stat = false; }
                       me.detail.Status = stat;
                       me.detail.oid = true;
                       me.Apply();
                       $('#SalesModelYear').attr('disabled', 'disabled');
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $http.post('om.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    me.CloseModel = function () {
        me.detail = {};
        me.grid1.clearSelection();
        me.detail.Status = true;
        $('#SalesModelYear').removeAttr('disabled');
        
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
        $('#SalesModelYear').css("text-align", "right");
        $('#SalesModelYear').removeAttr('disabled');


    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Model Year",
        xtype: "panels",
        toolbars: [
            { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
            { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isDetail && isLoadData", click: "delete()" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlModelYear",
                title: "Model Year",
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
                         },
                         { type: "hr" },
                         //{ name: "SalesModelYear", model: "detail.SalesModelYear", text: "Model Year", cls: "span2 full", maxlength: 4, required: true, validasi: "required", },
                         { name: "SalesModelYear", model: "detail.SalesModelYear", cls: "span4", type: "select2", datasource: "Year", text: "Model Year" },
                         { name: "SalesModelDesc", model: "detail.SalesModelDesc", text: "Description", cls: "span4 full", required: true, validasi: "required", maxlength: 100 },
                         { name: "ChassisCode", model: "detail.ChassisCode", text: "Chassis Code", cls: "span4 full", required: true, validasi: "required", maxlength: 15 },
                         { name: "Remark ", model: "detail.Remark", text: "Keterangan", cls: "span4", maxlength: 100 },
                         { name: "Status", model: "detail.Status", text: "Status", type: "x-switch", cls: "span4" },
                         {
                             type: "buttons",
                             items: [
                                     { name: "btnAddColour", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "save()", show: "detail.oid === undefined", disable: "detail.SalesModelYear === undefined" },
                                     { name: "btnUpdateColour", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "save()", show: "detail.oid !== undefined" },
                                     { name: "btnDeleteColour", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                     { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined || detail.oid == undefined", disable: "detail.SalesModelYear === undefined" }
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
        SimDms.Angular("ModelYearController");
    }
});