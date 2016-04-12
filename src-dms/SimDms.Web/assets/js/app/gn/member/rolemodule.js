﻿$(document).ready(function () {
    var options = {
        title: "Role Module",
        xtype: "panels",
        panels: [
            {
                name: "pnlRoleModule",
                items: [
                    {
                        text: "Role",
                        type: "controls",
                        items: [
                            { name: "RoleId", cls: "span4", placeHolder: "Kode Role", type: "select" },
                        ]
                    },
                    { type: "buttons",
                      items: [
                          { name: "btnFilter", text: "Search", icon: "icon-search" }, 
                          { name: "btnAssign", text: "Assign", icon: "icon-bolt" },
                          { name: "btnUnAssign", text: "UnAssign", icon: "icon-unbolt" }
                      ]
                    }
                ]
            },
            {
                xtype: "k-grid",
                name: "gridRoleModules"
            },
        ]
    }
    
    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "RoleId", url: "gn.api/combo/roles" },
    ]);
    widget.setEventList([
        {
            name: "btnFilter",
            type: "button",
            eventType: "click",
            event: function (evt) {
                var data = $("#pnlRoleModule").serializeObject();
                widget.reloadGridData("tblDetailInfo");
            }
        },
        {
            name: "btnAssign",
            type: "button",
            eventType: "click",
            event: function (evt) {
                //AssignModules();
            }
        },
        {
            name: "btnUnAssign",
            type: "button",
            eventType: "click",
            event: function (evt) {
                UnassignModule();
            }
        }
    ]);
    widget.default = { DateFrom: new Date(), DateTo: new Date() };
    widget.render(function () {
        widget.populate(widget.default);
        reloadGrid();
        AssignModules();
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.showToolbars(["btnBrowse", "btnProcess"]);
        if (1 == 1) {
            var dataMain = widget.getForms();
            dataMain.ModuleID = data.ModuleID;
            widget.post("gn.api/RoleModule/Save", dataMain, function (result) {
                if (result.status) {
                    widget.reloadGridData("tblDetailInfo");
                    widget.showNotification(result.message || SimDms.defaultInformationMessage);
                }
                else {
                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
                }
            });
        }
        widget.lookup.hide();
    });

    $("[name=RoleId]").on("change", function () {
        reloadGrid();
    });

    function populateData(data) {
        widget.post("gn.api/rolemenu/search", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblDetailInfo", data: result.list });
                $('td:nth-child(6)').hide();
            }
            else {
                confirm(result.message);
            }
        });
    }

    function reloadGrid() {
        var data = $("#pnlRoleModule").serializeObject();
        widget.kgrid({
            url: "gn.api/Grid/RoleModules",
            name: "gridRoleModules",
            params: data,
            columns: [
                { field: "ModuleID", title: "Module ID", width: 100 },
                { field: "ModuleCaption", title: "Caption", width: 150 },
                { field: "ModuleIndex", title: "Index", width: 50 },
                { field: "ModuleUrl", title: "Url", width: 150 },
                { field: "IsPublishDescription", title: "Is Publish", width: 100 },
                { field: "InternalLinkDescription", title: "Is Internal Link", width: 100 },
            ],
        });
    }

    function AssignModules() {
        var params = {};
        widget.klookup({
            name: "btnAssign",
            title: "Menu List",
            url: "gn.api/Grid/Modules",
            serverBinding: true,
            multiSelect: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "fltMenuId", text: "Kode Menu", cls: "span2" },
                        { name: "fltMenuCaption", text: "Nama Menu", cls: "span4" },
                    ]
                }
            ],
            columns: [
                { field: "ModuleID", title: "Module ID", width: 100 },
                { field: "ModuleCaption", title: "Caption", width: 150 },
                { field: "ModuleIndex", title: "Index", width: 50 },
                { field: "ModuleUrl", title: "Url", width: 150 },
                { field: "IsPublishDescription", title: "Is Publish", width: 100 },
                { field: "InternalLinkDescription", title: "Is Internal Link", width: 100 },
            ],
            dynamicParams: [
                { name: "RoleID", element: "RoleId" }
            ],
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterModuleID", text: "Kode Menu", cls: "span2" },
                        { name: "filterModuleCaption", text: "Nama Menu", cls: "span4" },
                    ]
                }
            ],
            params: params,
            onSelected: function (doc) { },
            onSelectedRows: function (data) {
                var dataMain = widget.getForms();
                var moduleIDs = new Array();

                $.each(data || [], function (key, val) {
                    moduleIDs.push(val.ModuleID);
                });
                dataMain.ModuleID = moduleIDs;

                widget.post("gn.api/RoleModule/Save", dataMain, function (result) {
                    if (result.status) {
                        reloadGrid();
                        widget.showNotification(result.message || SimDms.defaultInformationMessage);
                    }
                    else {
                        widget.showNotification(result.message || SimDms.defaultErrorMessage);
                    }
                });
            }
        });
    }

    function UnassignModule() {
        var dataMain = widget.getForms();
        widget.selectedRows("gridRoleModules", function (data) {
            if (widget.isNullOrEmpty(data) == false) {
                var moduleIDs = new Array();
                var formData = new FormData();

                $.each(data, function (key, val) {
                    moduleIDs.push(val.ModuleID);
                });
                formData.append("ModuleID", moduleIDs);
                formData.append("RoleID", $("[name='RoleId']").val());

                var params = {
                    RoleID: $("[name='RoleId']").val(),
                    ModuleID: moduleIDs
                };

                widget.post("gn.api/RoleModule/UnassignModules", params, function (result) {
                    if (result.status) {
                        reloadGrid();
                    }
                    widget.showNotification(result.message);
                });
            }
            else {
                widget.showNotification("You have to select modules to be unassigned.");
            }
        });
    }
});