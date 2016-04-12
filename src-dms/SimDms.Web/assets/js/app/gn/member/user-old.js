$(document).ready(function () {
    var options = {
        title: "Management User",
        xtype: "grid-form",
        urlList: "gn.api/grid/users",
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "icon-file" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
        ],
        columns: [
            { mData: "UserId", sTitle: "Kode User", sWidth: "120px" },
            { mData: "FullName", sTitle: "Nama Lengkap" },
            { mData: "Email", sTitle: "Email", sWidth: "180px" },
            { mData: "RoleName", sTitle: "Role", sWidth: "180px" },
            { mData: "BranchCode", sTitle: "Branch", sWidth: "120px" },
            {
                mData: "IsActive", sTitle: "Status", sWidth: "100px",
                mRender: function (data, type, full) {
                    return (data) ? "Active" : "Non Active";
                }
            },
            //{
            //    mData: "IsChangeBranchCode", sTitle: "Change Branch?", sWidth: "150px",
            //    mRender: function (data, type, full) {
            //        return (data) ? "Yes" : "No";
            //    }
            //},
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
            { name: "UserId", text: "Kode User", cls: "span4 full", required: false },
            { name: "FullName", text: "Nama Lengkap", required: true },
            { name: "Email", text: "Email" },
            { name: "RoleId", text: "Role", cls: "span6", type: "select", required: true },
            { name: "BranchCode", text: "BranchCode", cls: "span8", type: "select", required: true },
            { name: "TypeOfGoods", text: "Type Part", cls: "span4", type: "select" },
            { name: "ProfitCenter", text: "Profit Center", cls: "span4", type: "select" },
            { name: "IsActive", text: "Active", cls: "span4", type: "switch", float: "left" },
            //{ name: "IsChangeBranchCode", text: "Is Change Branch", cls: "span4", type: "switch", float: "left" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.select({ selector: "select[name=RoleId]", url: "gn.api/combo/roles" });
        widget.select({ selector: "select[name=BranchCode]", url: "gn.api/combo/branchs" });
        widget.select({ selector: "select[name=TypeOfGoods]", url: "gn.api/combo/typeofgoods" });
        widget.select({ selector: "select[name=ProfitCenter]", url: "gn.api/combo/profitcenters" });

        $.post("gn.api/user/default", function (result) {
            widget.default = result;
            widget.populate(widget.default);
        });
    });


    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        $(".main .gl-form select").val("");
        $(".main .gl-form").show();
        $(".main .gl-grid").hide();
        widget.showToolbars(["btnSave", "btnCancel"]);
        widget.populate(widget.default);
        $("[name=UserId]").focus();
    });

    $("#btnEdit").on("click", function () {
        var row = widget.selectedRow();

        if (row !== undefined) {
            widget.showToolbars(["btnSave", "btnCancel"]);
            widget.populate(widget.default, function () {
                $(".main .gl-form").show();
                $(".main .gl-grid").hide();
                widget.populate(row);
                $("[name=UserName]").focus();
            });
        }
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("gn.api/user/save", data, function (result) {
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

    $("#btnDelete").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            if (confirm("Anda yakin akan menghapus data ini?")) {
                widget.post("gn.api/user/delete", row, function (result) {
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