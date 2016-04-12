$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Management Role",
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "icon-file", cls: "button small" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "button small" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "button small" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "button small" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash", cls: "button small" },
        ],
        panels: [
            {
                name: "panelFilter",
                items: [
                    { name: "filterRoleId", text: "Role ID", cls: "span3 full" },
                    { name: "filterRoleName", text: "Role Name", cls: "span5 full" },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnSearch", text: "Search", cls: "button small", icon: "icon-search" },
                            { name: "btnClear", text: "Clear", cls: "button small", icon: "icon-eraser" },
                        ]
                    }
                ]
            },
            {
                name: "gridRole",
                xtype: "k-grid"
            },
            {
                name: "panelInput",
                items: [
                    { name: "RoleId", text: "Role ID", cls: "span4 full", required: true },
                    { name: "RoleName", text: "Role Name", required: true, cls: "span4 full" },
                    { name: "IsActive", text: "Is Active", type: "switch", cls: "span3 full", float: "left" },
                    { name: "IsAdmin", text: "Is Admin", type: "switch", cls: "span3 full", float: "left" },
                    { name: "IsChangeBranchCode", text: "Is Change Branch", type: "switch", cls: "span3 full", float: "left" },
                ]
            },
        ]
    };

    var vars = {};
    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {
        initElementEvent();
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
        showPanelInput(false);
        reloadGrid();
        vars.default = { IsActive: true };
    }

    function initElementEvent() {
        $("#btnCreate").on("click", function () {
            $("[name='RoleId']").removeAttr("disabled");

            widget.clearValidation();
            widget.clearPanelInputs(["panelInput"]);
            widget.populate(vars.default);
            widget.showToolbars(["btnSave", "btnCancel"]);
            showPanelInput(true);
            widget.clearValidation();
        });
        $("#btnEdit").on("click", function () {
            widget.selectedRow("gridRole", function (data) {
                showPanelInput(true);
                widget.populateData(data);
                $("[name='RoleId']").attr("disabled", true);
                widget.showToolbars(["btnSave", "btnCancel"]);
            });
        });
        $("#btnDelete").on("click", function () {
            widget.confirm("Do you want to delete this data?", function (result) {
                if (result == "Yes") {
                    widget.selectedRow("gridRole", function (data) {
                        var url = "gn.api/Role/Delete";
                        var params = {
                            RoleId: data.RoleId
                        };

                        widget.post(url, params, function (result) {
                            if (result.status) {
                                reloadGrid();
                            }

                            widget.showNotification(result.message);
                        });
                    });
                }
            });
        });
        $("#btnSave").on("click", function () {
            var validation = widget.validate();
            if (validation) {
                var data = $("#panelInput").serializeObject();
                var url = "gn.api/Role/Save";

                widget.post(url, data, function (result) {
                    if (result.status) {
                        reloadGrid();
                        showPanelInput(false);
                        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
                    }

                    widget.showNotification(result.message);
                });
            }
        });
        $("#btnCancel").on("click", function () {
            widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
            showPanelInput(false);
        });
        $("[name='UserId'], [name='FullName'], [name='Email']").on("change", function () {
            reloadGrid();
        });
        $("#btnSearch").on("click", function (evt) {
            reloadGrid();
        });
        $("#btnClear").on("click", function () {
            widget.clearPanelInputs(["panelFilter"]);
            reloadGrid();
        });
    }

    function reloadGrid() {
        var data = $("#panelFilter").serializeObject();
        widget.kgrid({
            url: "gn.api/Grid/Roles",
            name: "gridRole",
            params: data,
            columns: [
                { field: "RoleId", title: "Role ID", width: 100 },
                { field: "RoleName", title: "Role Name", width: 200 },
                { field: "IsActive", title: "Is Active", width: 200, template: "#= (IsActive==true) ? 'Yes':'No' #" },
            ],
            onDblClick: function (a, b, c) {
                $("#btnEdit").click();
            }
        });
    }

    function showPanelInput(state) {
        if (state) {
            widget.hidePanel([
                { name: "panelFilter", type: "" },
                { name: "gridRole", type: "k-grid" },
            ]);
            widget.showPanel([
                { name: "panelInput", type: "", hideDivider: true }
            ]);
        }
        else {
            widget.showPanel([
                { name: "panelFilter", type: "" },
                { name: "gridRole", type: "k-grid" },
            ]);
            widget.hidePanel(["panelInput"]);
        }
    }
});