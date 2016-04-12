$(document).ready(function () {
    var options = {
        title: "Management Role",
        xtype: "grid-form",
        urlList: "gn.api/grid/roles",
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
            { name: "RoleId", text: "Kode Role", cls: "span4 full", required: true },
            { name: "RoleName", text: "Nama Role", required: true },
            { name: "Themes", text: "Themes", cls: "span4 full" },
            { name: "IsAdmin", text: "Is Admin", cls: "span4", type: "switch", float: "left" },
            { name: "IsActive", text: "Is Active", cls: "span4 full", type: "switch", float: "left" },
            { name: "IsChangeBranchCode", text: "Is Change Branch", cls: "span4 full", type: "switch", float: "left" },
        ],
        columns: [
            { mData: "RoleId", sTitle: "Kode Role", sWidth: "180px" },
            { mData: "RoleName", sTitle: "Nama Role" },
            { mData: "Themes", sTitle: "Themes", sWidth: "180px" },
            {
                mData: "IsAdmin", sTitle: "Admin", sWidth: "100px",
                mRender: function (data, type, full) {
                    return (data) ? "Admin" : "Non Admin";
                }
            },
            {
                mData: "IsActive", sTitle: "Status", sWidth: "100px",
                mRender: function (data, type, full) {
                    return (data) ? "Active" : "Non Active";
                }
            },
            {
                mData: "IsChangeBranchCode", sTitle: "Change Branch", sWidth: "150px",
                mRender: function (data, type, full) {
                    return (data) ? "Yes" : "No";
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
            widget.post("cs.api/role/save", data, function (result) {
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
                widget.post("cs.api/role/delete", row, function (result) {
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