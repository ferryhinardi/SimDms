$(document).ready(function () {
    var options = {
        title: "Management Module",
        xtype: "grid-form",
        urlList: "gn.api/grid/modules",
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "icon-file" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
        ],
        items: [
            {
                text: "Company",
                type: "controls",
                items: [
                    { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
                    { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
                ]
            },
            { name: "ModuleID", type: "text", maxlength: "36", text: "Module ID", cls: "span4 full", required: true },
            { name: "ModuleCaption", maxlength: 250, text: "Caption", required: true },
            { name: "ModuleIndex", type: "spinner", maxlength: 3, text: "Index", cls: "span4 full" },
            { name: "ModuleUrl", maxlength: 250, text: "Url", cls: "span4", type: "text", float: "left" },
            { name: "InternalLink", type: "switch", text: "Internal Link", cls: "span4 full", type: "switch", float: "left" },
            { name: "IsPublish", type: "switch", text: "Is Publish", cls: "span4 full", type: "switch", float: "left" },
        ],
        columns: [
            { mData: "ModuleID", sTitle: "Module ID", sWidth: "180px" },
            { mData: "ModuleCaption", sTitle: "Caption" },
            { mData: "ModuleIndex", sTitle: "Index", sWidth: "180px" },
            { mData: "ModuleUrl", sTitle: "Url", sWidth: "180px" },
            {
                mData: "InternalLink", sTitle: "Internal Link", sWidth: "100px",
                mRender: function (data, type, full) {
                    return (data)==true ? "Yes" : "No";
                }
            },
            {
                mData: "IsPublish", sTitle: "Is Published", sWidth: "100px",
                mRender: function (data, type, full) {
                    return (data)==true ? "Yes" : "No";
                }
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        $.post("gn.api/role/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        $(".main .gl-form").show();
        $(".main .gl-grid").hide();
        widget.showToolbars(["btnSave", "btnCancel"]);
        widget.populate(widget.default);
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("gn.api/module/save", data, function (result) {
                if (result.status) {
                    $(".main .gl-form").hide();
                    $(".main .gl-grid").show();
                    widget.refreshGrid();
                    widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
                    widget.showNotification(result.message || SimDms.defaultInformationMessage);
                }
                else {
                    widget.showNotification(result.message || SimDms.defaultErrorMessage);
                }
            });
        }
    });

    $("#btnEdit").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            widget.showToolbars(["btnSave", "btnCancel"]);
            widget.populate(row, function () {
                $(".main .gl-form").show();
                $(".main .gl-grid").hide();
            });
        }
    });

    $("#btnDelete").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            if (confirm("Do you want to delete selected data?")) {
                widget.post("gn.api/module/delete", row, function (result) {
                    if (result.status) {
                        $(".main .gl-form").hide();
                        $(".main .gl-grid").show();
                        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
                        widget.showNotification(result.message || SimDms.defaultInformationMessage);
                        widget.refreshGrid();
                    }
                    else {
                        widget.showNotification(result.message || SimDms.defaultErrorMessage);
                    }
                });
            };
        }
    });

    $("#btnCancel").on("click", function () {
        $(".main .gl-form").hide();
        $(".main .gl-grid").show();
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    });
});