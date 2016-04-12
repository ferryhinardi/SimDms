var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spLookupController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "ReportBrowse",
            title: "Report Browse",
            manager: gnManager,
            query: "GetReport",
            defaultSort: "ReportID asc",
            columns: [
            { field: "ReportID", title: "Report Id" },
            { field: "ReportInfo", title: "Report Info" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.isSave = false;
                me.Apply();
                me.loadDetail(data);
                me.detail.ReportID = data.ReportID;
                $('#pnlB').show();
                $('#wxsalestarget').show();
                $('#btnDelete').hide();
            }
        });
    }

    me.Keyword = function () {
        var lookup = Wx.blookup({
            name: "lookupBrowse",
            title: "Lookup Browse",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "SECR" }),
            defaultSort: "LookUpValue asc",
            columns: [
            { field: "LookUpValue", title: "Name" },
            { field: "LookUpValueName", title: "Description" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.Keyword = data.LookUpValue;
                me.isSave = false;
                me.Apply();

            }
        });
    }

    me.loadDetail = function (data) {
        $http.post('gn.api/ReportSetting/DetailLoad?ReportID=' + data.ReportID).
           success(function (data, status, headers, config) {
               me.grid.detail = data;
               me.loadTableData(me.grid1, me.grid.detail);
           }).
           error(function (e, status, headers, config) {
               //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               console.log(e);
           });
    }

    //me.saveData = function (e, param) {
    //    $http.post('gn.api/MasterLookup/save', me.data).
    //        success(function (data, status, headers, config) {
    //            if (data.status) {
    //                Wx.Success("Data saved...");
    //                me.startEditing();
    //                $('#LookUpValue').removeAttr('disabled');
    //                $('#LookUpValueName').removeAttr('disabled');
    //                $('#ParaValue').removeAttr('disabled');
    //            } else {
    //                MsgBox(data.message, MSG_ERROR);
    //            }
    //        }).
    //        error(function (e, status, headers, config) {
    //            //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
    //            console.log(e);
    //        });
    //}

    //me.delete = function () {
    //    MsgConfirm("Are you sure to delete current record?", function (result) {
    //        if (result) {
    //            $http.post('gn.api/ReportSetting/delete', me.data).
    //            success(function (dl, status, headers, config) {
    //                if (dl.status) {
    //                    me.init();
    //                    Wx.Info("Record has been deleted...");
    //                } else {
    //                    MsgBox(dl.message, MSG_ERROR);
    //                }
    //            }).
    //            error(function (e, status, headers, config) {
    //                // MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
    //                console.log(e);
    //            });
    //        }
    //    });
    //}
    me.AddEditModel = function () {
        var Field = "Keyword";
        var Names = "Keyword";
        var ret = me.CheckMandatory(Field, Names);
        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            me.LinkDetail();
            $http.post('gn.api/ReportSetting/save', me.detail).
                success(function (data, status, headers, config) {
                    if (data.status) {
                        Wx.Success(data.message);
                        me.detail = {};
                        me.clearTable(me.grid1);
                        me.grid.detail = data;
                        me.loadTableData(me.grid1, me.grid.detail);
                        me.LinkDetail();
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

    me.delete2 = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                me.LinkDetail();
                $http.post('gn.api/ReportSetting/delete', me.detail).
                success(function (dl, status, headers, config) {
                    if (dl.success) {

                        me.detail = {};
                        me.clearTable(me.grid1);
                        Wx.Info("Record has been deleted...");

                        me.grid.detail = dl.result;
                        me.loadTableData(me.grid1, me.grid.detail);
                        me.LinkDetail();
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
                { id: "ReportID", header: "Report ID", width: 200 },
                { id: "Keyword", header: "Keyword", width: 250 },
                { id: "IsVisible", header: "Is Visible", width: 800 }
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) {
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        me.Apply();
                    }
                }
            }
        });
    }

    me.LinkDetail = function () {
        me.detail.ReportID = me.data.ReportID;
    }

    me.CloseModel = function () {
        me.detail = {};
        me.detail.oid = undefined;
        me.grid1.clearSelection();
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.detail = {};
        me.detail.IsVisible = false;
        $('#pnlB').hide();
        $('#wxsalestarget').hide();
        $('#btnKeyword').removeAttr("style");
    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Setting Security Report",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "Report Info",
                items: [
                     {
                         text: "Report",
                         type: "controls",
                         cls: "span8 full",
                         required: true,
                         items: [
                                    { name: "ReportID ", model: "data.ReportID", text: "", cls: "span3", required: true, validasi: "required", readonly : true },
                                    { name: "ReportInfo ", model: "data.ReportInfo", type: "text", text: "", cls: "span5", readonly: true },
                         ]
                     }
                ]
            },
            {
                name: "pnlB",
                title: "Detail",
                items: [
                        { name: "ReportID", model: "detail.ReportID", text: "Report", cls: "span3", readonly: true },
                        { name: "Keyword", model: "detail.Keyword", text: "Keyword", cls: "span3", type: "popup", click: "Keyword()", style: "background-color: rgb(255, 218, 204)" },
                        { name: "IsVisible ", model: "detail.IsVisible ", type: "x-switch", text: "Is Visible", cls: "span2", float: "left" },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditModel()", show: "detail.oid === undefined", disable: "detail.ReportID === undefined" },
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




