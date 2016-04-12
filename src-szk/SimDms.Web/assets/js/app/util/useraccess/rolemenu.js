$(document).ready(function () {
    var options = {
        title: "Role Menu",
        xtype: "panels",
        panels: [
            {
                name: "pnlRoleMenu",
                title: "Role Search",
                items: [
                    {
                        text: "Role",
                        type: "controls",
                        items: [
                            { name: "RoleId", cls: "span4", placeHolder: "Kode Role", type: "select" },
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnFilter", text: "Search", icon: "fa fa-search" },
                            { name: "btnAssign", text: "Assign", icon: "fa fa-file-excel-o" },
                            { name: "btnAssignModule", text: "Assign Module", icon: "fa fa-file-excel-o" },
                            { name: "btnUnAssign", text: "UnAssign", icon: "fa fa-unbolt" }
                        ]
                    }
                ]
            },
            {
                title: "Detail Information",
                xtype: "grid",
                name: "tblRoleMenu",
                tblname: "tblRoleMenu",
                source: "util/Grid/RoleMenus",
                selectable: true,
                multiselect: false,
                columns: [
                    { mData: "MenuID", sTitle: "Menu ID", sWidth: "150px" },
                    { mData: "MenuCaption", sTitle: "Caption" },
                    { mData: "MenuUrl", sTitle: "URL", sWidth: "160px" },
                    { mData: "MenuLevel", sTitle: "Level", sWidth: "160px" },
                    { mData: "MenuIndex", sTitle: "Index", sWidth: "160px" },
                ],
                selectable: true,
                multiselect: false,
                additionalParams: [
                    { name: "RoleId", element: "RoleId", type: "select" },
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "RoleId", url: "util/Role/List" },
    ]);
    widget.setEventList([
        {
            name: "btnFilter",
            type: "button",
            eventType: "click",
            event: function (evt) {
                var data = $("#pnlRoleMenu").serializeObject();
                widget.reloadGridData("tblRoleMenu");
            }
        },
        {
            name: "btnAssign",
            type: "button",
            eventType: "click",
            event: function (evt) {
                if (widget.isNullOrEmpty($("[name='RoleId']").val()) == false) {
                    widget.lookup.init({
                        name: "GridAssignMenu",
                        source: "util/grid/Menus_DataTable",
                        columns: [
                            { mData: "MenuId", sTitle: "Menu ID", sWidth: "180px" },
                            { mData: "MenuCaption", sTitle: "Caption" },
                            { mData: "MenuHeaderDescription", sTitle: "Header", sWidth: "180px" },
                            { mData: "MenuIndex", sTitle: "Index", sWidth: "180px" },
                            { mData: "MenuLevel", sTitle: "Level", sWidth: "180px" },
                            { mData: "MenuUrl", sTitle: "Url", sWidth: "180px" }
                        ],
                        additionalParams: [
                            { name: "RoleId", element: "RoleId", type: "select" }
                        ]
                    });
                    widget.lookup.show();
                }
            }
        },
        {
            name: "btnAssignModule",
            type: "button",
            eventType: "click",
            event: function (evt) {
                if (widget.isNullOrEmpty($("[name='RoleId']").val()) == false) {
                    widget.lookup.init({
                        name: "GridAssignModule",
                        source: "util/grid/Modules_DataTable",
                        columns: [
                            { mData: "ModuleId", sTitle: "Module ID", sWidth: "180px" },
                            { mData: "ModuleCaption", sTitle: "Caption" },
                        ],
                        additionalParams: [
                            { name: "RoleId", element: "RoleId", type: "select" }
                        ]
                    });
                    widget.lookup.show();
                }
            }
        },
        {
            name: "btnUnAssign",
            type: "button",
            eventType: "click",
            event: function (evt) {
                //var row = widget.selectedRow();
                var dataMain = widget.getForms();
                var row = $('#tblRoleMenu tr.row_selected td:nth-child(1)').text();
                dataMain.MenuID = row;
                if (dataMain !== undefined) {
                    if (confirm("Anda yakin akan menghapus data ini?")) {
                        widget.post("util/RoleMenu/Delete", dataMain, function (result) {
                            if (result.status) {
                                widget.reloadGridData("tblRoleMenu");
                                widget.showNotification(result.message || SimDms.defaultInformationMessage);
                            }
                            else {
                                widget.showNotification(result.message || SimDms.defaultErrorMessage);
                            }
                        });
                    };
                }
            }
        }
    ]);
    widget.default = { DateFrom: new Date(), DateTo: new Date() };
    widget.render(function () {
        widget.populate(widget.default);
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.showToolbars(["btnBrowse", "btnProcess"]);
        if (1 == 1) {
            var dataMain = widget.getForms();
            dataMain.MenuID = data.MenuId;
            dataMain.ModuleId = data.ModuleId
            if (name == 'GridAssignMenu') {
                widget.post("util/RoleMenu/Save", dataMain, function (result) {
                    if (result.status) {
                        widget.reloadGridData("tblRoleMenu");
                        widget.showNotification(result.message || SimDms.defaultInformationMessage);
                    }
                    else {
                        widget.showNotification(result.message || SimDms.defaultErrorMessage);
                    }
                });
            }
            else {
                widget.post("util/RoleMenu/SaveModule", dataMain, function (result) {
                    console.log(result);
                    if (result.status) {
                        widget.reloadGridData("tblRoleMenu");
                        widget.showNotification(result.message || SimDms.defaultInformationMessage);
                    }
                    else {
                        widget.showNotification(result.message || SimDms.defaultErrorMessage);
                    }
                });
            }
            
        }
        widget.lookup.hide();
    });

    $("[name=RoleId]").on("change", function () {
        var data = $("#pnlRoleMenu").serializeObject();
        widget.reloadGridData("tblRoleMenu");
    });

    function populateData(data) {
        widget.post("gn.api/rolemenu/search", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblRoleMenu", data: result.list });
                $('td:nth-child(6)').hide();
            }
            else {
                confirm(result.message);
            }
        });
    }

});