$(document).ready(function () {
    var options = {
        title: "Management User",
        xtype: "grid-form",
        urlList: "util/grid/users",
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "fa fa-file" },
            { name: "btnEdit", text: "Edit", icon: "fa fa-edit" },
            { name: "btnDelete", text: "Delete", icon: "fa fa-trash-o" },
            { name: "btnSave", text: "Save", icon: "fa fa-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "fa fa-undo", cls: "hide" },
        ],
        items: [
            { name: "UserId", type: "text", maxlength: "36", text: "", cls: "hide", required: false},
            { name: "Username", maxlength: 250, text: "Username", required: true },
            { name: "Email", type: "text", maxlength: 50, text: "Email", cls: "span4 full" },
            { name: "FirstName", maxlength: 50, text: "First Name", cls: "span4", type: "text", float: "left" },
            { name: "LastName", type: "text", text: "Last Name", cls: "span4 full", float: "left" },
            { name: "DealerCode", type: "select", text: "Company", cls: "span4 full", float: "left" },
            { name: "RoleId", type: "select", text: "Role", cls: "span4 full", float: "left" },
            { name: "IsApproved", type: "switch", text: "Is Approved", cls: "span4 full", type: "switch", float: "left" },
            {
                type: "buttons", items: [
                    { name: "btnReset", text: "Reset Password", icon: "fa fa-eraser" , cls:"hide"}
                ]
            },
        ],
        columns: [
            { mData: "Username", sTitle: "Username" },
            { mData: "FirstName", sTitle: "First Name", sWidth: "200px" },
            { mData: "LastName", sTitle: "Last Name", sWidth: "200px" },
            { mData: "Email", sTitle: "Email", sWidth: "180px" },
            { mData: "RoleName", sTitle: "Role", sWidth: "180px" },
            { mData: "IsApprovedDescription", sTitle: "Approved", sWidth: "180px" }
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.setSelect([
            { name: "DealerCode", url: "util/user/dealerlist" },
            { name: "RoleId", url: "util/role/list" }
        ]);

        $.post("util/user/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        $(".main .gl-form").show();
        $(".main .gl-grid").hide();
        $('#btnReset').hide();
        widget.showToolbars(["btnSave", "btnCancel"]);
        widget.populate(widget.default);
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("util/user/save", data, function (result) {
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
            widget.showToolbars(["btnSave", "btnReset", "btnCancel"]);
            widget.populate(row, function () {
                $(".main .gl-form").show();
                $(".main .gl-grid").hide();
                $('#btnReset').show();
            });
        }
    });

    $("#btnDelete").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            if (confirm("Do you want to delete selected data?")) {
                widget.post("util/user/delete", row, function (result) {
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

    $('#btnReset').on('click', function (e) {
        var row = widget.selectedRow();
        if (row !== undefined) {
            var con = confirm("Apakah anda ingin me-reset password???");
            if (con)
            {                
                widget.post('util/user/reset', row, function (result) {
                    if (result.status) {
                        widget.showNotification(result.message + " , default password = 123456" || SimDms.defaultInformationMessage);
                    }
                    else
                    {
                        widget.showNotification(result.message || SimDms.defaultErrorMessage);
                    }
                });
            }
        }
    });
});