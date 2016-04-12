var widget;

$(document).ready(function () {
    var options = {
        title: "Maintain Overtime",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilterOvertime",
                title: "Filter Selection",
                items: [
                    { name: "Department", text: "Department", cls: "span4", type: "select", opt_text: "--SELECT ALL--" },
                    { name: "Position", text: "Position", cls: "span4", type: "select", opt_text: "--SELECT ALL--" },
                    { name: "AttdDate", text: "Attendance Date", cls: "span4", type: "datepicker", required: true },
                    { type: "buttons", items: [{ name: "btnFilter", text: "Search", icon: "icon-search" }] },
                ]
            },
            {
                name: "pnlEditOvertime",
                title: "Edit Data",
                items: [
                    {
                        text: "NIK - Employee",
                        type: "controls",
                        items: [
                            { name: "EmployeeID", cls: "span2", placeHolder: "NIK", readonly: true },
                            { name: "EmployeeName", cls: "span6", placeHolder: "Name", readonly: true }
                        ]
                    },
                    {
                        text: "Duty Time - Clock Time",
                        type: "controls",
                        items: [
                            { name: "OnDutyTime", cls: "span2", placeHolder: "On Duty Time", readonly: true },
                            { name: "ClockInTime", cls: "span2", placeHolder: "Clock In Time", readonly: false },
                            { name: "OffDutyTime", cls: "span2", placeHolder: "Off Duty Time", readonly: true },
                            { name: "ClockOutTime", cls: "span2", placeHolder: "Clock Out Time", readonly: false }
                        ]
                    },
                    {
                        text: "Rest Time - Overtime",
                        type: "controls",
                        items: [                                                                    
                            { name: "OnRestTime", cls: "span2", placeHolder: "On Rest Time", readonly: true },
                            { name: "OffRestTime", cls: "span2", placeHolder: "Off Rest Time", readonly: true },
                            { name: "CalcOvertime", cls: "span2 number-int", placeHolder: "Calc Overtime (minutes)", readonly: true },
                            { name: "ApprOvertime", cls: "span2 number-int", placeHolder: "Appr Overtime (minutes)", readonly: false }
                        ]
                    },
                    { type: "buttons", items: [{ name: "btnSaveDetail", text: "Save", icon: "icon-save" }, { name: "btnCancelDetail", text: "cancel", icon: "icon-undo" }] },
                ]
            },
            {
                xtype: "kgrid",
                title: "Overtime Data",
                name: "gridOvertime"
            }
        ],
    }

    widget = new SimDms.Widget(options);
    widget.default = { AttdDate: new Date() };
    widget.render(callbackRender);

    widget.setSelect([
        { name: "Department", url: "ab.api/combo/departments" },
        { name: "Position", url: "ab.api/combo/positions", optionalText: "-- SELECT ALL --", cascade: { name: "Department" } },
    ]);
    widget.render(function () {
        widget.populate(widget.default);
    });

    $("#Department").on("change", function (evt) {
        reloadOvertimeData();
    });
    $("#Position").on("change", function (evt) {
        reloadOvertimeData();
    });
    $("[name='AttdDate']").on("change", function (evt) {
        reloadOvertimeData();
    });
    $("#btnFilter").on("click", function (evt) {
        reloadOvertimeData();
    });
});                                       

function reloadOvertimeData() {
    var department = $("[name='Department']").val();
    var position = $("[name='Position']").val();
    var dateFrom = $("[name='AttdDate']").val();

    if (widget.isNullOrEmpty(department) == true || widget.isNullOrEmpty(dateFrom) == true) {
        widget.showNotification("You must have to fill at least Department and DateFrom input.");
    }
    else {
        var url = "ab.api/grid/employeeshifts";
        var params = {
            Department: $("#Department").val(),
            Position: $("#Position").val(),
            DateFrom: $("[name='AttdDate']").val(),
            IsGrid: true
        };

        widget.kgrid({
            name: "gridOvertime",
            url: url,
            params: params,
            sort: [{ field: "NextFollowUpDate", dir: "desc" }],
            toolbars: [
                { name: "btnEditDetail", text: "Edit", icon: "icon-edit" },
                { name: "btnDeleteDetail", text: "Delete", icon: "icon-trash" },
            ],
            columns: [
                { field: "AttdDate", title: "Attendance Date", width: 150, template: "#=  moment(AttdDate, 'YYYYMMDD').format('DD MMM YYYY') #" },
                { field: "EmployeeID", title: "NIK", width: 100 },
                { field: "EmployeeName", title: "Name", width: 250 },
                { field: "Shift", title: "", width: 150 },
                { field: "ClockInTime", title: "Clock In", width: 80 },
                { field: "ClockOutTime", title: "Clock Out", width: 80 },
                { field: "CalcOvertime", title: "Calc Overtime", width: 150 },
                { field: "ApprOvertime", title: "Appr Overtime", width: 150 },
            ],
            onDblClick: editOvertime,
        }, callbackGridOvertime);
    }
}

function editOvertime() {
    console.log("Edit overtime");
}

function callbackGridOvertime() {
    var btnEditDetail = $("#btnEditDetail");
    var btnDeleteDetail = $("#btnDeleteDetail");
    var btnSaveDetail = $("#btnSaveDetail");
    var btnCancelDetail = $("#btnCancelDetail");

    btnSaveDetail.off();
    btnCancelDetail.off();
    btnEditDetail.off();
    btnDeleteDetail.off();

    btnSaveDetail.on("click", function (evt) {
        console.log("save detail");
    });
    btnCancelDetail.on("click", function (evt) {
        console.log("cancel detail");
    });
    btnEditDetail.on("click", function (evt) {
        console.log("edit detail");
    });
    btnDeleteDetail.on("click", function (evt) {
        console.log("delete detail");
    });
}

function callbackRender() {
    console.log("Render callback ...");
    showEditForm(false);
}

function showEditForm(state) {
    var panelEdit = $("div.panel#pnlFilterOvertime");
    var gridOvertime = $("#gridOvertime").parent().parent();

    console.log(panelEdit, gridOvertime);
    gridOvertime.html("");

    panelEdit.children("subtitle").click();

    if (state) {
        console.log("show form edit");
    }
    else {
        console.log("hide form edit");
    }
}

function evt_gridEdit(evt, data) {
    $("#pnlEmployeeList").slideDown();
    data.OnDutyTime = widget.validateTime(data.OnDutyTime);
    data.OffDutyTime = widget.validateTime(data.OffDutyTime);
    data.ClockInTime = widget.validateTime(data.ClockInTime);
    data.ClockOutTime = widget.validateTime(data.ClockOutTime);
    data.OnRestTime = widget.validateTime(data.OnRestTime);
    data.OffRestTime = widget.validateTime(data.OffRestTime);
    widget.populate(data, "#pnlEmployeeList", function () {
        var val = widget.timeDiff(data.ClockOutTime, data.OffDutyTime);
        data.CalcOvertime = val;
    });
}