$(document).ready(function () {
    var options = {
        title: "Personal Position",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Dept - Position",
                        type: "controls",
                        items: [
                            { name: "Department", text: "Department", cls: "span2", type: "select" },
                            { name: "Position", text: "Position", cls: "span3", type: "select", opt_text: "-- SELECT ALL --" },
                            {
                                name: "Status", text: "Status", cls: "span2", type: "select", opt_text: "ALL STATUS",
                                items: [
                                    { text: "AKTIF", value: "1" },
                                    { text: "NON AKTIF", value: "2" },
                                    { text: "KELUAR", value: "3" },
                                    { text: "PENSIUN", value: "4" },
                                ]
                            },
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
    widget.setSelect([
        { name: "Department", url: "ab.api/combo/departments", optionalText: "-- SELECT ONE --" },
        { name: "Position", url: "ab.api/combo/positions", optionalText: "-- SELECT ALL --", cascade: { name: "Department" } },
    ]);
    widget.render(function () {
        widget.populate(widget.default);
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $("select[name=Position],select[name=Status]").on("change", refreshGrid);
    });

    function refreshGrid() {
        widget.kgrid({
            url: "ab.api/inquiry/employees",
            name: "pnlKGrid",
            params: $("#pnlFilter").serializeObject(),
            columns: [
                { field: "EmployeeID", width: 120, title: "NIK" },
                { field: "EmployeeName", title: "Name" },
                { field: "JoinDate", title: "Join Date", width: 160, template: "#= (JoinDate == undefined) ? '-' : moment(JoinDate).format('DD MMM YYYY') #" },
                { field: "LastPosition", title: "Last Position" },
                { field: "AchieveTimes", width: 100, title: "Achieves", template: "<div class='right'>#= (AchieveTimes == undefined || AchieveTimes == 0) ? '-' : AchieveTimes #</div>" },
                { field: "Status", width: 100, title: "Status" },
                { field: "IsValidAchieve", width: 100, title: "Is Valid" },
            ],
            detailInit: detailInit,
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
            fileName: "pers_position",
            items: [
                { field: "EmployeeID", width: 120, title: "NIK", type: "text" },
                { field: "EmployeeName", title: "Name" },
                { field: "JoinDate", title: "Join Date", width: 160, type: "date" },
                { field: "LastPosition", title: "Last Position" },
                { field: "AchieveTimes", width: 100, title: "Achieves", type: "date" },
                { field: "Status", width: 100, title: "Status" },
                { field: "IsValidAchieve", width: 100, title: "Is Valid" },
            ]
        });
    }
});
