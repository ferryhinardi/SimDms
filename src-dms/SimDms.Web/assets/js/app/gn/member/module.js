$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Management Module",
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
                    { name: "filterModuleId", text: "Menu ID", cls: "span3 full" },
                    { name: "filterModuleCaption", text: "Menu Caption", cls: "span5 full" },
                    { name: "filterModuleIndex", type: "spinner", text: "Module Index", cls: "span2 full" },
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
                name: "gridModule",
                xtype: "k-grid"
            },
            {
                name: "panelInput",
                items: [
                    { name: "ModuleId", type: "text", maxlength: "36", text: "Module ID", cls: "span4 full ignore-uppercase", required: true },
                    { name: "ModuleCaption", maxlength: 250, text: "Caption", required: true, cls: "ignore-uppercase" },
                    { name: "ModuleIndex", type: "spinner", maxlength: 3, text: "Index", cls: "span2 full" },
                    { name: "IsPublish", type: "switch", float: "left", text: "Is Publish" },
                    { name: "InternalLink", type: "switch", float: "left", maxlength: 3, text: "Is Internal Link", cls: "span4 full" },
                    { name: "ModuleUrl", maxlength: 128, type: "text", text: "URL", cls: "span8 full ignore-uppercase", float: "left" },
                    { name: "Icon", maxlength: 50, type: "text", text: "Icon", cls: "span8 full ignore-uppercase", float: "left" },
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
        reloadModuleData();
    }

    function initElementEvent() {
        $("#btnCreate").on("click", function () {
            widget.clearPanelInputs(["panelInput"]);
            showPanelInput(true);
            widget.showToolbars(["btnSave", "btnCancel"]);
            $("[name='ModuleId']").removeAttr("disabled");
        });
        $("#btnEdit").on("click", function () {
            widget.selectedRow("gridModule", function (data) {
                console.log(data);
                showPanelInput(true);
                var datas = data;
                datas.ModuleId = datas.ModuleID;
                widget.populate(datas);
                $("[name='ModuleId']").attr("disabled", true);
                widget.showToolbars(["btnSave", "btnCancel"]);
            });
        });
        $("#btnDelete").on("click", function () {
            widget.confirm("Do you want to delete this data?", function (result) {
                if (result == "Yes") {
                    widget.selectedRow("gridModule", function (data) {
                        var url = "gn.api/Module/Delete";
                        var params = {
                            ModuleID: data.ModuleID
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
            var url = "gn.api/Module/Save";

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
            url: "gn.api/Grid/Modules",
            name: "gridModule",
            params: data,
            columns: [
                { field: "ModuleID", title: "Module ID", width: 120 },
                { field: "ModuleCaption", title: "Caption", width: 200 },
                { field: "ModuleIndex", title: "Index", width: 70 },
                { field: "IsPublish", title: "Published", width: 70, template: "#= (IsPublish==true) ? 'Yes' : 'No' #" },
                { field: "InternalLink", title: "Internal Link", width: 70, template: "#= (InternalLink==true) ? 'Yes' : 'No' #" },
                { field: "ModuleUrl", title: "Url", width: 200 },
                { field: "Icon", title: "Icon", width: 200 },
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
                { name: "gridModule", type: "k-grid" },
            ]);
            widget.showPanel([
                { name: "panelInput", type: "", hideDivider: true }
            ]);
        }
        else {
            widget.showPanel([
                { name: "panelFilter", type: "" },
                { name: "gridModule", type: "k-grid" },
            ]);
            widget.hidePanel(["panelInput"]);
        }
    }

    function reloadModuleData() {
        vars.currentModuleIndex = $("[name='filterModuleIndex']").val();

        if (vars.currentModuleIndex != vars.lastModuleIndex) {
            reloadGrid();
            vars.lastModuleIndex = vars.currentModuleIndex;
        }

        setTimeout(function () {
            reloadModuleData();
        }, 1000);
    }
});