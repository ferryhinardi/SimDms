$(document).ready(function () {
    var options = {
        title: "Management Menu",
        xtype: "grid-form",
        urlList: "gn.api/grid/menus",
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
            { name: "MenuId", text: "Kode Menu", cls: "span4 full ignore-uppercase", required: true },
            { name: "MenuCaption", text: "Nama Menu", cls: "ignore-uppercase", required: true },
            { name: "MenuHeader", text: "Header", cls: "span4", type: "select" },
            { name: "MenuIndex", text: "Index", cls: "span4", type: "spinner" },
            { name: "MenuLevel", text: "Level", cls: "span4", type: "spinner" },
            { name: "MenuUrl", text: "Url", cls: "span4 ignore-uppercase" }
        ],
        columns: [
            { mData: "MenuId", sTitle: "Kode Menu", sWidth: "180px" },
            { mData: "MenuCaption", sTitle: "Nama Menu" },
            { mData: "MenuHeader", sTitle: "Header Menu", sWidth: "180px" },
            { mData: "MenuIndex", sTitle: "Index Menu", sWidth: "180px" },
            { mData: "MenuLevel", sTitle: "Level Menu", sWidth: "180px" },
            { mData: "MenuUrl", sTitle: "Url Menu", sWidth: "180px" }
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        $.post("gn.api/menu/default", function (result) {
            widget.default = result;
            widget.populate(result);
            widget.select({ selector: "[name=MenuHeader]", url: "gn.api/combo/menus" });
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
            widget.post("gn.api/menu/save", data, function (result) {
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
            if (confirm("Anda yakin akan menghapus data ini?")) {
                widget.post("gn.api/menu/delete", row, function (result) {
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
            };
        }
    });

    $("#btnCancel").on("click", function () {
        $(".main .gl-form").hide();
        $(".main .gl-grid").show();
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    });
});