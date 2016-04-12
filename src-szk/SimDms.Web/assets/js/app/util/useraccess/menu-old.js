$(document).ready(function () {
    var options = {
        title: "Management Menu",
        xtype: "grid-form",
        urlList: "util/grid/menus",
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "fa fa-file" },
            { name: "btnEdit", text: "Edit", icon: "fa fa-edit" },
            { name: "btnDelete", text: "Delete", icon: "fa fa-trash-o" },
            { name: "btnSave", text: "Save", icon: "fa fa-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "fa fa-undo", cls: "hide" },
        ],
        items: [
            //{
            //    text: "Company",
            //    type: "controls",
            //    items: [
            //        { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
            //        { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
            //    ]
            //},
            { name: "MenuId", text: "Menu ID", cls: "span4 full ignore-uppercase", required: true },
            { name: "MenuCaption", text: "Caption", cls: "ignore-uppercase", required: true },
            { name: "MenuHeader", text: "Header", cls: "span4", type: "select" },
            { name: "MenuIndex", text: "Index", cls: "span4", type: "spinner" },
            { name: "MenuLevel", text: "Level", cls: "span4", type: "spinner" },
            { name: "MenuUrl", text: "Url", cls: "span4 ignore-uppercase" }
        ],
        columns: [
            { mData: "MenuId", sTitle: "Menu ID", sWidth: "180px" },
            { mData: "MenuCaption", sTitle: "Caption" },
            { mData: "MenuHeaderDescription", sTitle: "Header", sWidth: "180px" },
            { mData: "MenuIndex", sTitle: "Index", sWidth: "180px" },
            { mData: "MenuLevel", sTitle: "Level", sWidth: "180px" },
            { mData: "MenuUrl", sTitle: "Url", sWidth: "180px" }
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        //$.post("util/menu/default", function (result) {
        //    widget.default = result;
        //    widget.populate(result);
        //    widget.select({ selector: "[name=MenuHeader]", url: "util/combo/menus" });
        //});
        widget.select({ selector: "[name=MenuHeader]", url: "util/Menu/Headers" });
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
            widget.post("util.api/menu/save", data, function (result) {
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
                widget.post("util.api/menu/delete", row, function (result) {
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