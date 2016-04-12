$(document).ready(function () {
    var options = {
        title: "Attendance",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
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
                name: "pnlKGrid",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.default = { Status: "1" };

    widget.render(function () {
        widget.populate(widget.default);
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $("select[name=Position],select[name=Status]").on("change", refreshGrid);

        widget.setSelect([
            { name: "Department", url: "ab.api/combo/departments" },
            { name: "Position", url: "ab.api/combo/positions", optionalText: "-- SELECT ALL --", cascade: { name: "Department" } },
            { name: "Grade", url: "ab.api/combo/grades", optionalText: "--SELECT ALL--" },
            { name: "ShiftCode", url: "ab.api/combo/shifts", optionalText: "--SELECT ALL--" },
        ]);
        $("select").on("change", refreshGrid);
    });

    function refreshGrid() {
        widget.kgrid({
            url: "ab.api/Inquiry/Attendance",
            name: "pnlKGrid",
            params: $("#pnlFilter").serializeObject(),
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
    }

    function detailInit(e) {
        widget.post("ab.api/inquiry/employeeachievements", { EmployeeID: e.data.EmployeeID }, function (data) {
            if (data.length > 0) {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: data },
                    columns: [
                        { field: "AssignDate", title: "Date", width: 160, template: "#= moment(AssignDate).format('DD MMM YYYY') #" },
                        { field: "ActivePosition", title: "Position" },
                        { field: "IsJoinDate", title: "Join", width: 110, template: "#= (IsJoinDate) ? 'Y' : 'N'  #" },
                    ]
                });
            }
            else {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: [{ Info: "Employee ini belum ada data achievement, silahkan dilengkapi datanya" }] },
                    columns: [{ field: "Info", title: "Info" }]
                });
            }
        })
    }

    function exportXls() {
        widget.exportXls({
            name: "pnlKGrid",
            type: "kgrid",
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
});

function cleanDate(rawDate) {
    var clean = rawDate.substring(0, 4) + '-' + rawDate.substring(4, 6) + '-' + rawDate.substring(6, 8);
    return clean;
}
