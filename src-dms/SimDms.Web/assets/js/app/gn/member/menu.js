$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Management Menu",
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
                    { name: "filterMenuId", text: "Menu ID", cls: "span3 full ignore-uppercase" },
                    { name: "filterMenuCaption", text: "Menu Caption", cls: "span5 full ignore-uppercase" },
                    { name: "filterMenuLevel", type: "spinner", text: "Menu Level", cls: "span2 full ignore-uppercase" },
                    { name: "filterMenuIndex", type: "spinner", text: "Menu Index", cls: "span2 full ignore-uppercase" },
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
                name: "gridMenu",
                xtype: "k-grid"
            },
            {
                name: "panelInput",
                items: [
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
                            { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
                        ]
                    },
                    { name: "MenuId", type: "text", maxlength: "36", text: "Menu ID", cls: "span4 full ignore-uppercase", required: true },
                    { name: "MenuCaption", maxlength: 250, text: "Caption", required: true, cls: "ignore-uppercase" },
                    { name: "MenuLevel", type: "spinner", maxlength: 3, text: "Level", cls: "span4 full" },
                    { name: "MenuIndex", type: "spinner", maxlength: 3, text: "Index", cls: "span4 full" },
                    { name: "MenuUrl", maxlength: 250, text: "Url", cls: "span4 ignore-uppercase", type: "text", float: "left" },
                    { name: "MenuHeader", maxlength: 36, type: "select", text: "Header", cls: "span4 full ignore-uppercase", float: "left" },
                    { name: "MenuIcon", maxlength: 50, text: "Icon", cls: "span4 ignore-uppercase", type: "text", float: "left" },
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
        reloadMenuHeader();

        widget.post("gn.api/Menu/Default", function (result) {
            vars.default = result;
        });
    }

    function initElementEvent() {
        $("#btnCreate").on("click", function () {
            vars.callbackValue = undefined;
            widget.clearPanelInputs(["panelInput"]);
            showPanelInput(true);
            widget.showToolbars(["btnSave", "btnCancel"]);
            $("[name='MenuId']").removeAttr("disabled");
            widget.populate(vars.default);
        });
        $("#btnEdit").on("click", function () {
            widget.selectedRow("gridMenu", function (data) {
                showPanelInput(true);
                widget.populate($.extend(data, vars.default));
                $("[name='MenuId']").attr("disabled", true);
                widget.showToolbars(["btnSave", "btnCancel"]);
                vars.callbackValue = data.MenuHeader;
                reloadMenuHeader(data.MenuHeader);
            });
        });
        $("#btnDelete").on("click", function () {
            widget.confirm("Do you want to delete this data?", function (result) {
                if (result == "Yes") {
                    widget.selectedRow("gridMenu", function (data) {
                        var url = "gn.api/Menu/Delete";
                        var params = {
                            MenuId: data.MenuId
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
            var data = $("#panelInput").serializeObject();
            var url = "gn.api/Menu/Save";

            widget.post(url, data, function (result) {
                if (result.status) {
                    reloadGrid();
                    showPanelInput(false);
                    widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
                }

                widget.showNotification(result.message);
            });
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
        data.gridType = "local";
        widget.kgrid({
            url: "gn.api/Grid/Menus",
            name: "gridMenu",
            params: data,
            columns: [
                { field: "MenuId", title: "Menu ID", width: 120 },
                { field: "MenuCaption", title: "Caption", width: 200 },
                { field: "MenuLevel", title: "Level", width: 70 },
                { field: "MenuIndex", title: "Index", width: 70 },
                { field: "MenuHeader", title: "Header", width: 200 },
                { field: "MenuUrl", title: "Url", width: 200 },
                { field: "MenuIcon", title: "Icon", width: 100 },
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
                { name: "gridMenu", type: "k-grid" },
            ]);
            widget.showPanel([
                { name: "panelInput", type: "", hideDivider: true }
            ]);
        }
        else {
            widget.showPanel([
                { name: "panelFilter", type: "" },
                { name: "gridMenu", type: "k-grid" },
            ]);
            widget.hidePanel(["panelInput"]);
        }
    }

    function reloadMenuHeader(callbackValue) {
        vars.currentMenuLevel = $("[name='MenuLevel']").val();
        if (vars.currentMenuLevel != vars.lastMenuLevel) {
            var url = "gn.api/Combo/Menus?currentMenuLevel=" + vars.currentMenuLevel;
            widget.select({
                name: "MenuHeader",
                url: url,
                callbackValue: vars.callbackValue
            });

            vars.lastMenuLevel = vars.currentMenuLevel;
        }

        setTimeout(function () {
            reloadMenuHeader();
        }, 1000);
    }
});