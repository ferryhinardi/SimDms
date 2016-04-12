$(document).ready(function () {
    var options = {
        title: "Master Shift",
        xtype: "grid-form",
        urlList: "ab.api/grid/shifts",
        sortings: [[0, "asc"]],
        columns: [
            { mData: "ShiftCode", sTitle: "Shift Code", sWidth: "120px" },
            { mData: "ShiftName", sTitle: "Shift Name" },
            { mData: "OnDutyTime", sTitle: "On DutyTime", sWidth: "120px" },
            { mData: "OffDutyTime", sTitle: "Off DutyTime", sWidth: "140px" },
            { mData: "OnRestTime", sTitle: "On RestTime", sWidth: "120px" },
            { mData: "OffRestTime", sTitle: "Off RestTime", sWidth: "140px" },
            { mData: "WorkingHour", sTitle: "Work Hour", sWidth: "120px" },
            {
                mData: "IsActive", sTitle: "Active", sWidth: "100px",
                mRender: function (data, type, full) {
                    return ((data || false) ? "YA" : "TIDAK");
                }
            },
        ],
        items: [
            {
                text: "Shift",
                type: "controls",
                items: [
                    { name: "ShiftCode", cls: "span2", placeHolder: "Shift Code", maxlength: 10, required: true },
                    { name: "ShiftName", cls: "span6", placeHolder: "Shift Name", required: true }
                ]
            },
            { name: "OnDutyTime", text: "On DutyTime", type: "timepicker", cls: "span4" },
            { name: "OffDutyTime", text: "Off DutyTime", type: "timepicker", cls: "span4" },
            { name: "OnRestTime", text: "On Rest Time", type: "timepicker", cls: "span4" },
            { name: "OffRestTime", text: "Off Rest Time", type: "timepicker", cls: "span4" },
            { name: "WorkingHour", text: "Working Hour", type: "text", cls: "span4 full", readonly: true },
            { name: "IsActive", text: "Aktif", type: "switch", float: "left" },
        ],
        toolbars: [
            { name: "btnCreate", text: "Create", icon: "icon-file" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit" },
            { name: "btnDelete", text: "Delete", icon: "icon-trash" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
        ],

    }

    var widget = new SimDms.Widget(options);
    widget.default = {}
    widget.render(function () {
        $.post("ab.api/shift/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        $(".main .gl-form").show();
        $(".main .gl-grid").hide();
        $("[name='ShiftCode']").attr('disabled', false).focus();
        $("[name='ShiftCode']").focus();
        widget.showToolbars(["btnSave", "btnCancel"]);
        widget.populate(widget.default);
    });

    $("#btnEdit").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            widget.showToolbars(["btnSave", "btnCancel"]);
            row.WorkingHour = widget.timeDiff(row.OnRestTime, row.OnDutyTime) + widget.timeDiff(row.OffDutyTime, row.OffRestTime);
            widget.populate(row, function () {
                $(".main .gl-form").show();
                $(".main .gl-grid").hide();
                $("[name='ShiftCode']").attr('disabled', true);
                $("[name='ShiftName']").focus();
            });
        }
    });

    $("#btnDelete").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            if (confirm("Anda yakin akan menghapus data ini?")) {
                widget.post("ab.api/shift/delete", row, function (result) {
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
            widget.post("ab.api/shift/save", data, function (result) {
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

    $("[name='HolidayCode']").on("blur", function () {
        var val = $("[name='HolidayCode']").val().toUpperCase();
        $("[name='HolidayCode']").val(val);
    });

    $("[name='DateFrom']").on("change", function () {
        var val = $("[name='DateFrom']").val();
        $("[name='DateTo']").val(val);
    });

    $("[name='DateTo']").on("change", function () {
        var val1 = $("[name='DateFrom']").val();
        var val2 = $("[name='DateTo']").val();

        if (val2.length > 0) {
            var val1Ext = moment(val1, "DD-MMM-YYYY").format("YYYYMMDD");
            var val2Ext = moment(val2, "DD-MMM-YYYY").format("YYYYMMDD");

            if (val1Ext > val2Ext) {
                $("[name='DateFrom']").val(val2);
            }
        }
    });

    $(".gl-form select").on("change", function () {
        var data = $(".main form").serializeObject();
        var val = widget.timeDiff(data.OnRestTime, data.OnDutyTime, true) + widget.timeDiff(data.OffDutyTime, data.OffRestTime, true);
        $(".gl-form #WorkingHour").val(val);
    });
});