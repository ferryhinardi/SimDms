$(document).ready(function () {
    var options = {
        title: "Attendance Resume",
        xtype: "panels",
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
            { name: "btnBack", text: "Back", icon: "icon-hand-left", cls: "hide" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
        ],
        panels: [
            {
               name: "panelFilter",
               items: [
                   {
                       text: "Date Range",
                       type: "controls",
                       items: [
                           { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                           { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                       ]
                   },
                   {
                       text: "Employee",
                       type: "controls",
                       items: [
                           { name: "EmployeeName", text: "Employee Name", cls: "span3", type: "text" },
                           { name: "EmployeeID", text: "Employee ID", cls: "span1", type: "text" },
                       ]
                   },
                   {
                       text: "Position",
                       type: "controls",
                       items: [
                           { name: "Department", text: "Department", cls: "span3", type: "select" },
                           { name: "Position", text: "Position", cls: "span3", type: "select" },
                           { name: "Grade", text: "Grade", cls: "span2", type: "select" },
                       ]
                   },
                   {
                       text: "Shift",
                       type: "controls",
                       items: [
                           { name: "ShiftCode", text: "Shift", cls: "span4", type: "select" },
                       ]
                   },
               ],
            },
            {
                name: "panelResume",
                title: "Resume",
                items: [
                    { name: "OnTime", text: "On Time", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                    { name: "Late", text: "Late", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                    { name: "ReturnBeforeTheTime", text: "Return Before The Time", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                    { name: "Overtime", text: "Overtime", cls: "span6 full", type: "popup", icon: "icon-hand-right", readonly: true },
                ]
            },
            {
                name: "panelOnTime",
                title: "On Time ",
                xtype: "kgrid",
            },
            {
                name: "panelLate",
                title: "Late ",
                xtype: "kgrid",
            },
            {
                name: "panelReturnBeforeTheTime",
                title: "Return Before The Time",
                xtype: "kgrid",
            },
            {
                name: "panelOvertime",
                title: "Overtime",
                xtype: "kgrid",
            },
        ]
    };
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(init);

    function init() {
        showPanel("panelFilter");
        showPanel("panelResume");
        refresh();
        widget.hideAccordion();

        widget.setSelect([
            { name: "Department", url: "ab.api/combo/departments" },
            { name: "Position", url: "ab.api/combo/positions", optionalText: "-- SELECT ALL --", cascade: { name: "Department" } },
            { name: "Grade", url: "ab.api/combo/grades", optionalText: "--SELECT ALL--" },
            { name: "ShiftCode", url: "ab.api/combo/shifts", optionalText: "--SELECT ALL--" },
        ]);
        $("select").on("change", reloadData);
    }

    function reloadData() {
        var url = "ab.api/Inquiry/AttendanceResume";
        var data = $("#panelFilter").serializeObject();

        widget.post(url, data, function (result) {
            widget.populate(result);
        });
    }

    function showPanel(name) {
        var hide = [];
        var show = [];
        $.each(options.panels, function (idx, val) {
            if (val !== undefined && val.name !== undefined) {
                if (val.name == name) {
                    switch (val.xtype) {
                        case "kgrid":
                            show.push({ name: val.name, type: val.xtype });
                            break;
                        default:
                            show.push(val.name);
                            break;
                    }
                }
                else {
                    switch (val.xtype) {
                        case "kgrid":
                            hide.push({ name: val.name, type: val.xtype });
                            break;
                        default:
                            hide.push(val.name);
                            break;
                    }
                }
            }
        });
        options["ActivePanel"] = name;
        widget.hidePanel(hide);
        widget.showToolbars((show.length > 0 && show[0] == "panelResume") ? ["btnRefresh"] : ["btnBack", "btnExportXls"]);
        setTimeout(function () { widget.showPanel(show); }, 500);
    }

    function back() {
        showPanel(["panelResume"]);
        showPanel(["panelFilter"]);

        widget.showToolbars(["btnRefresh"]);
    }

    function refresh() {
        widget.post("cs.api/summary/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);

                var record = {};
                $.each(result.list, function (idx, val) {
                    record[val.ControlLink] = val.RemValue;
                })
                widget.populate(record);
            }
        });
    }

    function showResumeDetails(state, panel) {
        showPanel([panel]);

        options["visibleGrid"] = panel;
        options["state"] = state;

        var data = $("#panelFilter").serializeObject();
        data.State = state;

        widget.kgrid({
            url: "ab.api/Inquiry/AttendanceResumeDetails",
            name: panel,
            serverBinding: true,
            pageSize: 8,
            params: data,
            columns: [
                { field: "EmployeeID", width: 120, title: "NIK" },
                { field: "EmployeeName", width: 250, title: "Name" },
                { field: "DepartmentName", width: 100, title: "Department" },
                { field: "PositionName", width: 250, title: "Position" },
                { field: "GradeName", width: 120, title: "Grade" },
                { field: "AttendanceDate", width: 100, title: "Attendance Date", width: 160, template: "#= (AttendanceDate == undefined) ? '-' : moment(cleanDate(AttendanceDate)).format('DD MMM YYYY') #" },
                { field: "OnDutyTime", width: 100, title: "On Duty" },
                { field: "OffDutyTime", width: 100, title: "Off Duty" },
                { field: "ClockInTime", width: 100, title: "Clock In" },
                { field: "ClockOutTime", width: 100, title: "Clock Out" },
                { field: "IsAbsence", width: 120, title: "Is Absence" },
                { field: "LateTime", width: 100, title: "Late" },
                { field: "ReturnBeforeTheTime", width: 175, title: "Return Before The Time" },
                { field: "Overtime", width: 100, title: "Overtime" },
            ],
        });
    };

    function exportXls() {
        var data = $("#panelFilter").serializeObject();
        data.State = options["state"];

        widget.exportXls({
            name: options["visibleGrid"],
            type: "kgridlnk",
            source: "ab.api/Inquiry/AttendanceResumeDetails",
            params: data,
            fileName: "attendance",
            items: [
                { field: "EmployeeID", width: 120, title: "NIK" },
                { field: "EmployeeName", width: 250, title: "Name" },
                { field: "DepartmentName", width: 100, title: "Department" },
                { field: "PositionName", width: 250, title: "Position" },
                { field: "GradeName", width: 120, title: "Grade" },
                { field: "AttendanceDate", width: 100, title: "Attendance Date", width: 160, template: "#= (AttendanceDate == undefined) ? '-' : moment(cleanDate(AttendanceDate)).format('DD MMM YYYY') #" },
                { field: "OnDutyTime", width: 100, title: "On Duty" },
                { field: "OffDutyTime", width: 100, title: "Off Duty" },
                { field: "ClockInTime", width: 100, title: "Clock In" },
                { field: "ClockOutTime", width: 100, title: "Clock Out" },
                { field: "IsAbsence", width: 120, title: "Is Absence" },
                { field: "LateTime", width: 100, title: "Late" },
                { field: "ReturnBeforeTheTime", width: 175, title: "Return Before The Time" },
                { field: "Overtime", width: 100, title: "Overtime" },
            ]
        });
    }

    $("#btnRefresh").on("click", reloadData);
    $("#btnExportXls").on("click", exportXls);
    $("#btnBack").on("click", back);

    $("#btnOnTime").on("click", function (evt) {
        showResumeDetails("1", "panelOnTime");
    });
    $("#btnLate").on("click", function (evt) {
        showResumeDetails("2", "panelLate");
    });
    $("#btnReturnBeforeTheTime").on("click", function (evt) {
        showResumeDetails("3", "panelReturnBeforeTheTime");
    });
    $("#btnOvertime").on("click", function (evt) {
        showResumeDetails("4", "panelOvertime");
    });
});

function cleanDate(rawDate) {
    var clean = rawDate.substring(0, 4) + '-' + rawDate.substring(4, 6) + '-' + rawDate.substring(6, 8);
    return clean;
}
