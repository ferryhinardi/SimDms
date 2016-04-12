$(document).ready(function () {
    var options = {
        title: "Master Refference Type",
        xtype: "grid-form",
        urlList: "util/grid/RefferenceType",
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
            { name: "ProductType", type: "text", maxlength: "36", text: "Product Type", cls: "span4 full", required: true },
            { name: "RefferenceType", type: "text", maxlength: "36", text: "Refference Type", cls: "span4 full", required: true },
            { name: "RefferenceCode", type: "text", maxlength: 36, text: "Refference Code/ Index", cls: "span4 full", required: true },
            { name: "Description", maxlength: 250, text: "Description" },
            
        ],
        columns: [
            //{ mData: "ProductType", sTitle: "Product Type", sWidth: "180px" },
            { mData: "RefferenceType", sTitle: "Refference Type", sWidth: "180px" },
            { mData: "RefferenceCode", sTitle: "Refference Code/ Index", sWidth: "180px" },
            { mData: "Description", sTitle: "Description" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        $.post("wh.api/SvMaster/GetSvMStMRSRData", function (result) {
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
            widget.post("wh.api/SvMaster/SvMStMRSRSave", data, function (result) {
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
                widget.post("wh.api/SvMaster/SvMStMRSRdelete", row, function (result) {
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