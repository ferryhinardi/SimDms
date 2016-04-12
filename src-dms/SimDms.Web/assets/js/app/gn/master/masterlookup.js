var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spLookupController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "lookupBrowse",
            title: "Lookup Browse",
            manager: gnManager,
            query: "lookupBrowse",
            defaultSort: "CodeName asc",
            columns: [
            { field: "CodeID", title: "Code Id" },
            { field: "CodeName", title: "Code Name" },
            { field: "FieldLength", title: "Field Length" },
            { field: "isNumber", title: "is Number" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.isSave = false;
                me.Apply();
                me.loadDetail(data);
                $('#LookUpValue').removeAttr('disabled');
                $('#LookUpValueName').removeAttr('disabled');
                $('#ParaValue').removeAttr('disabled');
            }
        });
    }

    me.loadDetail = function (data) {

        $http.post('gn.api/MasterLookUp/LookUpDetailsLoad?CodeID=' + data.CodeID).
           success(function (data, status, headers, config) {
               me.grid.detail = data;
               me.loadTableData(me.grid1, me.grid.detail);
           }).
           error(function (e, status, headers, config) {
               //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               console.log(e);
           });
    }
  
    me.saveData = function (e, param) {
        $http.post('gn.api/MasterLookup/save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    $('#LookUpValue').removeAttr('disabled');
                    $('#LookUpValueName').removeAttr('disabled');
                    $('#ParaValue').removeAttr('disabled');
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
                $http.post('gn.api/MasterLookup/delete', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.status) {
                        me.init();
                        Wx.Info("Record has been deleted...");
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    // MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    console.log(e);
                });
            }
        });
    }

    me.delete2 = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                me.LinkDetail();
                $http.post('gn.api/MasterLookup/deleteDtl', me.detail).
                success(function (dl, status, headers, config) {
                    if (dl.success) {

                        me.detail = {};
                        me.clearTable(me.grid1);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = dl.result;
                        me.loadTableData(me.grid1, me.grid.detail);

                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    console.log(e);
                });
            }
        });
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxsalestarget",
            view: "wxtable", css:"alternating", scrollX: true, 
            columns: [
                { id: "LookUpValue", header: "Lookup Value", width: 200 },
                { id: "ParaValue", header: "Para Value", width: 250 },
                { id: "LookUpValueName", header: "Lookup Value Name", width: 800 }
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        $('#LookUpValue').attr('disabled', true);
                        me.Apply();
                    }
                }
            }
        });
    }

    me.LinkDetail = function () {
        me.detail.CodeID = me.data.CodeID;
    }

    me.AddEditModel = function () {
        var Field = 'LookUpValue,ParaValue,LookUpValueName';
        var Names = 'Lookup Value,Para Value,LookUp Value Name';
       
        var ret = me.CheckMandatory(Field, Names);

        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            me.LinkDetail();
            $http.post('gn.api/MasterLookup/savedtl', me.detail).
                success(function (data, status, headers, config) {
                    if (data.status) {
                        Wx.Success(data.message);
                        me.detail = {};
                        me.clearTable(me.grid1);
                        me.grid.detail = data.result;
                        me.loadTableData(me.grid1, me.grid.detail);
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    console.log(e);
                });
        }
    }

    me.CloseModel = function () {
        me.detail = {};
        me.detail.oid = undefined;
        me.grid1.clearSelection();
        $('#LookUpValue').removeAttr('disabled');
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.detail = {};
        $('#LookUpValue').attr('disabled', 'disabled');
        $('#LookUpValueName').attr('disabled', 'disabled');
        $('#ParaValue').attr('disabled', 'disabled');
        $('#btnDelete').hide();
    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Master Lookup",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "Lookup Header",
                items: [
                        { name: "CodeID", model: "data.CodeID", text: "Code ID", cls: "span3", require: true, validasi: "required" },
                        { name: "FieldLength", model: "data.FieldLength", type: "int", text: "FieldLength", cls: "span3", float: "left" },
                        { name: "isNumber", model: "data.isNumber", type: "x-switch", text: "Is Number", cls: "span2", float: "left" },
                        { name: "CodeName", model: "data.CodeName", text: "Code Name", cls: "span8" }
                ]
            },
            {
                name: "pnlB",
                title: "Lookup Detail",
                items: [
                        { name: "LookUpValue", model: "detail.LookUpValue", text: "Lookup Value", cls: "span2", style: "background-color: rgb(255, 218, 204)" },
                        { name: "ParaValue", model: "detail.ParaValue", text: "Para Value", cls: "span3", style: "background-color: rgb(255, 218, 204)" },
                        { name: "LookUpValueName", model: "detail.LookUpValueName", text: "Lookup Value Name", cls: "span3", style: "background-color: rgb(255, 218, 204)" },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditModel()", show: "detail.oid === undefined", disable: "detail.LookUpValue === undefined" },
                                { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddEditModel()", show: "detail.oid !== undefined" },
                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined" }
                        ]
                    },
                ]
            },
            {
                name: "wxsalestarget",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spLookupController");
    }
});




