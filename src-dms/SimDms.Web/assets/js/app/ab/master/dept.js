$(document).ready(function () {
    var options = {
        title: "Master Department",
        xtype: "grid-form",
        urlList: "ab.api/grid/departments",
        sortings: [[3, "asc"]],
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "icon-file" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
        ],
        columns: [
            { mData: "OrgCode", sTitle: "Dept Code", sWidth: "180px" },
            { mData: "OrgName", sTitle: "Dept Name" },
            { mData: "OrgHeader", sTitle: "Parent", sWidth: "180px" },
            { mData: "OrgSeq", sTitle: "Sequence", sWidth: "100px" },
        ],
        items: [
            {
                text: "Department",
                type: "controls",
                items: [
                    { name: "OrgCode", cls: "span2", placeHolder: "Code", maxlength: 20 },
                    { name: "OrgName", cls: "span6", placeHolder: "Department Name", maxlength: 150 }
                ]
            },
            { name: "OrgHeader", text: "Parent", cls: "span4", type: "select" },
            { name: "OrgSeq", text: "Level", cls: "span4", type: "spinner" },
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.default = {}
    widget.render(function () {
        widget.post("ab.api/department/default", function (result) {
            widget.default = result;
            widget.populate(result);
            widget.select({ selector: "[name=OrgHeader]", url: "ab.api/combo/departments" });
        });
    });

    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        widget.showToolbars(["btnSave", "btnCancel"]);
        widget.populate(widget.default, function () {
            $(".main .gl-form").show();
            $(".main .gl-grid").hide();
            $("[name='OrgCode']").attr('disabled', false);
            $("[name='OrgCode']").focus();
        });
    });

    $("#btnEdit").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            widget.showToolbars(["btnSave", "btnCancel"]);
            widget.populate(row, function () {
                $(".main .gl-form").show();
                $(".main .gl-grid").hide();
                $("[name='OrgCode']").attr('disabled', true);
                $("[name='OrgName']").focus();
            });
        }
    });

    $("#btnDelete").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            if (confirm("Anda yakin akan menghapus data ini?")) {
                widget.post("ab.api/department/delete", row, function (result) {
                    if (result.success) {
                        $(".main .gl-form").hide();
                        $(".main .gl-grid").show();
                        widget.refreshGrid();
                        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
                    }
                });
            };
        }
    });

    $("#btnSave").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("ab.api/department/save", data, function (result) {
                if (result.success) {
                    $(".main .gl-form").hide();
                    $(".main .gl-grid").show();
                    widget.refreshGrid();
                    widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
                }
            });
        }
    });

    $("#btnCancel").on("click", function () {
        $(".main .gl-form").hide();
        $(".main .gl-grid").show();
        widget.showToolbars(["btnCreate", "btnEdit", "btnDelete"]);
    });
});