var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spLookupController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.Module = function () {
        var lookup = Wx.blookup({
            name: "ModuleBrowse",
            title: "Module Browse",
            manager: gnManager,
            query: "ListModule",
            defaultSort: "MenuId asc",
            columns: [
            { field: "MenuId", title: "Module Id" },
            { field: "MenuCaption", title: "Module Caption" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ModuleID = data.MenuId;
                me.data.ModuleCaption = data.MenuCaption;
                me.Apply();
            }
        });
    }

    me.Menu = function () {
        var lookup = Wx.blookup({
            name: "ModuleBrowse",
            title: "Module Browse",
            manager: gnManager,
            query: new breeze.EntityQuery().from("ListMenu").withParameters({ ModuleID: me.data.ModuleID }),
            //defaultSort: "MenuId asc",
            columns: [
            { field: "MenuId", title: "Menu Id" },
            { field: "MenuCaption", title: "Menu Caption" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.MenuID = data.MenuId;
                me.data.MenuCaption = data.MenuCaption;
                me.Apply();
            }
        });
    }

    me.Language = function () {
        var lookup = Wx.blookup({
            name: "lookupBrowse",
            title: "Lookup Browse",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "LANG" }),
            defaultSort: "LookUpValue asc",
            columns: [
            { field: "LookUpValue", title: "Language ID" },
            { field: "LookUpValueName", title: "Language Description" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.LangID = data.LookUpValue;
                me.data.LangDesc = data.LookUpValueName;
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

    me.AddEditModel = function () {
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
                { id: "DictionaryID", header: "Dictionary ID", width: 300 },
                { id: "DefaultValue", header: "Default Value", width: 300 },
                { id: "DictionaryValue", header: "Dictionary Value", width: 300 },
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
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.detail = {};
        me.detail.IsVisible = false;
        //$('#pnlB').hide();
        //$('#wxsalestarget').hide();
    }

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Master Language",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "Filter Language",
                items: [
                       {
                           text: "Module Name", type: "controls", type: "controls", required: true, items: [
                             { name: "ModuleID", cls: "span2", placeHolder: "", type: "popup", click: "Module()", validasi: "required,max(15)" },
                             { name: "ModuleCaption", cls: "span4", placeHolder: "", readonly: true }]
                       },
                         {
                             text: "Menu", type: "controls", type: "controls", required: true, items: [
                               { name: "MenuID", cls: "span2", placeHolder: "", type: "popup", click: "Menu()", validasi: "required,max(15)", disable: "data.ModuleID === undefined" },
                               { name: "MenuCaption", cls: "span4", placeHolder: "", readonly: true }]
                         },
                           {
                               text: "Language", type: "controls", type: "controls", required: true, items: [
                                 { name: "LangID", cls: "span2", placeHolder: "", type: "popup", click: "Language()", validasi: "required,max(15)", disable: "data.MenuID === undefined" },
                                 { name: "LangDesc", cls: "span4", placeHolder: "", readonly: true }]
                           },
                ]
            },
            {
                name: "pnlB",
                title: "Language Setup",
                items: [
                     {
                         text: "", type: "controls", type: "controls", items: [
                            { name: "DictID", cls: "span2", placeHolder: "", readonly: true },
                            { name: "DefaValue", cls: "span3", placeHolder: "", readonly: true },
                            { name: "DictValue", cls: "span3", placeHolder: "" },
                         ]
                     },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditModel()", show: "detail.oid === undefined", disable: "detail.ReportID === undefined" },
                                { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddEditModel()", show: "detail.oid !== undefined" },
                                //{ name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
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




