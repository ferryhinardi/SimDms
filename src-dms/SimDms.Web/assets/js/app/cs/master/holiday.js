$(document).ready(function () {
    var options = {
        title: "Master Holiday",
        xtype: "grid-form",
        urlList: "cs.api/grid/holidays",
        sortings: [[3, "asc"]],
        columns: [
            { mData: "HolidayYear", sTitle: "Year", sWidth: "80px" },
            { mData: "HolidayCode", sTitle: "Holiday Code" },
            { mData: "HolidayDesc", sTitle: "Holiday Desc" },
            {
                mData: "DateFrom", sTitle: "Date From", sWidth: "180px",
                mRender: function (data, type, full) {
                    return moment(data).format('DD MMM YYYY');
                }
            },
            {
                mData: "DateTo", sTitle: "Date To", sWidth: "180px",
                mRender: function (data, type, full) {
                    return moment(data).format('DD MMM YYYY');
                }
            },
            {
                mData: "IsHoliday", sTitle: "Is Holiday",
                mRender: function (data, type, full) {
                    return ((data || false) ? "YA" : "TIDAK");
                }
            },
        ],
        columns: [
            { mData: "HolidayYear", sTitle: "Year", sWidth: "80px" },
            { mData: "HolidayCode", sTitle: "Holiday Code" },
            { mData: "HolidayDesc", sTitle: "Holiday Desc" },
            {
                mData: "DateFrom", sTitle: "Date From", sWidth: "180px",
                mRender: function (data, type, full) {
                    return moment(data).format('DD MMM YYYY');
                }
            },
            {
                mData: "DateTo", sTitle: "Date To", sWidth: "180px",
                mRender: function (data, type, full) {
                    return moment(data).format('DD MMM YYYY');
                }
            },
            {
                mData: "IsHoliday", sTitle: "Is Holiday",
                mRender: function (data, type, full) {
                    return ((data || false) ? "YA" : "TIDAK");
                }
            },
        ],
        items: [
            { name: "CompanyCode", text: "CompanyCode", cls: "span4", readonly: true },
            { name: "HolidayYear", text: "Year", cls: "span4", readonly: true },
            { name: "HolidayCode", text: "Holiday Code", required: true },
            { name: "HolidayDesc", text: "Holiday Desc", required: true },
            { name: "DateFrom", text: "Date From", type: "datepicker", cls: "span4", required: true },
            { name: "DateTo", text: "Date To", type: "datepicker", cls: "span4", required: true },
            { name: "IsHoliday", text: "Is Holiday", cls: "span4", type: "switch" },
            { name: "ReligionCode", text: "Religion", cls: "span4 hide", type: "select" },
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
        $.post("cs.api/holiday/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
        $("[name=IsHoliday]").on("change", function () {
            var _isHoliday = $(this);

            setTimeout(function () {
                if( _isHoliday.val() === "true") {
                    $("select[name=ReligionCode]").parent().parent().show();
                }
                else {
                    $("select[name=ReligionCode]").parent().parent().hide();
                }
            }, 500);
        });
        widget.select({ selector: "select[name=ReligionCode]", url: "cs.api/combo/lookups/rlgn" });
    });

    $("#btnCreate").on("click", function () {
        $(".main .gl-form input").val("");
        $(".main .gl-form").show();
        $(".main .gl-grid").hide();
        $("[name='HolidayCode']").attr('disabled', false).focus();
        widget.showToolbars(["btnSave", "btnCancel"]);
        widget.populate(widget.default);
    });

    $("#btnEdit").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            widget.showToolbars(["btnSave", "btnCancel"]);
            widget.populate(row, function () {
                $(".main .gl-form").show();
                $(".main .gl-grid").hide();
                $("[name='HolidayCode']").attr('disabled', true);
                $("[name='HolidayDesc']").focus();

                if (row.IsHoliday) {
                    $("select[name=ReligionCode]").parent().parent().show();
                }
                else {
                    $("select[name=ReligionCode]").parent().parent().hide();
                }
            });
        }
    });

    $("#btnDelete").on("click", function () {
        var row = widget.selectedRow();
        if (row !== undefined) {
            if (confirm("Anda yakin akan menghapus data ini?")) {
                widget.post("cs.api/holiday/delete", row, function (result) {
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
            widget.post("cs.api/holiday/save", data, function (result) {
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
});