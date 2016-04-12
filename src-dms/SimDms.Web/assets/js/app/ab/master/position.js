$(document).ready(function () {
    var options = {
        title: "Master Position",
        xtype: "grid-form",
        urlList: "ab.api/grid/positions",
        sortings: [[0, "asc"], [4, "asc"]],
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "icon-file" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
        ],
        columns: [
            { mData: "DeptCode", sTitle: "Dept Code", sWidth: "180px" },
            { mData: "PosCode", sTitle: "Pos Code", sWidth: "180px" },
            { mData: "PosName", sTitle: "Pos Name" },
            { mData: "PosHeader", sTitle: "Pos Header", sWidth: "180px" },
            { mData: "PosLevel", sTitle: "Pos Level", sWidth: "100px" },
        ],
        items: [
            {
                text: "Position",
                type: "controls",
                items: [
                    { name: "PosCode", cls: "span2", placeHolder: "Code", maxlength: 20 },
                    { name: "PosName", cls: "span6", placeHolder: "Position Name", maxlength: 150 }
                ]
            },
            { name: "DeptCode", text: "Organization", cls: "span4", type: "select", required: true },
            { name: "PosHeader", text: "Parent Position", cls: "span4", type: "select" },
            { name: "PosLevel", text: "Level", cls: "span4", type: "spinner" },
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.default = {}
    widget.setSelect([
        { name: "DeptCode", url: "ab.api/combo/departments" },
        { name: "PosHeader", url: "ab.api/combo/positions", optionalText: "-- SELECT POSITION --", cascade: { name: "DeptCode" } },
    ]);
    widget.render();

    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        widget.showToolbars(["btnSave", "btnCancel"]);
        widget.populate(widget.default, function () {
            $(".main .gl-form").show();
            $(".main .gl-grid").hide();
            $("[name=DeptCode],[name='PosCode']").attr('disabled', false);
            $("[name=PosHeader]").html("<option value=\"\">-- SELECT POSITION --</option>");
            $("[name=DeptCode]").val(undefined);
            $("[name=PosCode]").focus();
        });
    });

    $("#btnEdit").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            widget.showToolbars(["btnSave", "btnCancel"]);
            widget.populate(row, function () {
                $(".main .gl-form").show();
                $(".main .gl-grid").hide();
                $("[name='DeptCode'],[name='PosCode']").attr('disabled', true);
                widget.select({
                    selector: "[name='PosHeader']",
                    url: "ab.api/combo/positions",
                    selected: row.PosHeader,
                    additionalParams: [{ name: "id", value: row.DeptCode }]
                });
            });
        }
    });

    $("#btnDelete").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            if (confirm("Anda yakin akan menghapus data ini?")) {
                widget.post("ab.api/position/delete", row, function (result) {
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
            widget.post("ab.api/position/save", data, function (result) {
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