$(document).ready(function () {
    var options = {
        title: "Master Settings",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "Create", cls: "btn small", icon: "icon-file" },
            { name: "btnSave", text: "Save", cls: "btn small", icon: "icon-save" },
            { name: "btnCancel", text: "Cancel", cls: "btn small", icon: "icon-undo" },
            { name: "btnEdit", text: "Edit", cls: "btn small", icon: "icon-edit" },
            { name: "btnDelete", text: "Delete", cls: "btn small", icon: "icon-trash" },
        ],
        panels: [
            {
                name: "panelInput",
                title: "Input Data",
                items: [
                    { text: "Code", name: "SettingCode", cls: "span4 ignore-uppercase", required: true },
                    { text: "Description", name: "SettingDesc", cls: "span4 ignore-uppercase", required: true },
                    { text: "Param 1", name: "SettingParam1", cls: "span4 ignore-uppercase" },
                    { text: "Param 4", name: "SettingParam4", cls: "span4 ignore-uppercase" },
                    { text: "Param 2", name: "SettingParam2", cls: "span4 ignore-uppercase" },
                    { text: "Param 5", name: "SettingParam5", cls: "span4 ignore-uppercase" },
                    { text: "Param 3", name: "SettingParam3", cls: "span4 ignore-uppercase" },
                    { text: "Setting Link 1", name: "SettingLink1", cls: "span8 full ignore-uppercase" },
                    { text: "Setting Link 2", name: "SettingLink2", cls: "span8 full ignore-uppercase" },
                    { text: "Setting Link 3", name: "SettingLink3", cls: "span8 full ignore-uppercase" },
                ]
            },
            {
                name: "panelFilter",
                title: "Filters",
                items: [
                    { name: "filterSettingCode", type: "text", text: "Setting Code", cls: "span4" },
                    { name: "filterSettingDesc", type: "text", text: "Setting Description", cls: "span4" },
                ]
            },
            {
                name: "gridSettings",
                xtype: "kgrid"
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {
        $("#panelInput").fadeOut();
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);

        clearForm();
        reloadGridData();
        initializeEvents();
    }

    function reloadGridData() {
        var grid = widget.kgrid({
            name: "gridSettings",
            url: "cs.api/grid/Settings",
            params: {
                filterSettingCode: $("#filterSettingCode").val(),
                filterSettingDesc: $("#filterSettingDesc").val(),
            },
            serverBinding: true,
            columns: [
                { field: "SettingCode", title: "Setting Code", width: 175, maxlength: 20 },
                { field: "SettingDesc", title: "Setting Description", width: 250, maxlength: 250 },
                { field: "SettingParam1", title: "Setting Param 1", width: 140, maxlength: 20 },
                { field: "SettingParam2", title: "Setting Param 2", width: 140, maxlength: 20 },
                { field: "SettingParam3", title: "Setting Param 3", width: 140, maxlength: 20 },
                { field: "SettingParam4", title: "Setting Param 4", width: 140, maxlength: 20 },
                { field: "SettingParam5", title: "Setting Param 5", width: 140, maxlength: 20 },
                { field: "SettingLink1", title: "Setting Link 1", width: 140, maxlength: 20 },
                { field: "SettingLink2", title: "Setting Link 2", width: 140, maxlength: 20 },
                { field: "SettingLink3", title: "Setting Link 3", width: 140, maxlength: 20 },
            ],
            onDblClick: edit,
        });
    }

    function clearForm() {
        widget.clearForm();
    }

    function initializeEvents() {
        $("#panelFilter input").keypress(function (e) {
            if (e.which == 13) {
                reloadGridData();
            }
        });
        $("#btnCreate").on("click", create);
        $("#btnEdit").on("click", edit);
        $("#btnDelete").on("click", del);
        $("#btnCancel").on("click", cancel);
        $("#btnSave").on("click", save);
    }

    function create() {
        widget.clearForm();
        widget.showPanel("panelInput");
        widget.hidePanel("panelFilter");
        widget.hidePanel("gridSettings");
        widget.showToolbars(["btnSave", "btnCancel"]);
    }

    function del() {
        widget.selectedRow("gridSettings", function (result) {
            if (result !== undefined) {
                var params = {
                    CompanyCode: result.CompanyCode,
                    SettingCode: result.SettingCode
                }
                widget.confirm("Do you want to delete this data?", function (result) {
                    if (result == "Yes") {
                        var url = "cs.api/Settings/Delete";
                        widget.post(url, params, function (result) {
                            widget.showNotification(result.message || "");

                            if (result.success) {
                                reloadGridData();
                            }
                        });
                    }
                });
            }
        });
    }

    function edit() {
        widget.selectedRow("gridSettings", function (result) {
            if (result !== undefined) {
                widget.populate(result, "#panelInput");
                widget.showPanel("panelInput");
                widget.hidePanel("panelFilter");
                widget.hidePanel("gridSettings");
                widget.showToolbars(["btnSave", "btnCancel"]);
            }
        });
    }

    function cancel() {
        widget.hidePanel("panelInput");
        widget.showPanel("panelFilter");
        widget.showPanel("gridSettings");
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    }

    function save() {
        var url = "cs.api/Settings/Save";
        var params = widget.getForms();
        var validation = widget.validate();

        if (validation) {
            widget.post(url, params, function (result) {
                if (result.success) {
                    widget.hidePanel("panelInput");
                    widget.showPanel("panelFilter");
                    widget.showPanel("gridSettings");
                    widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);

                    reloadGridData();
                }

                widget.showNotification(result.message);
            });
        }
    }
});