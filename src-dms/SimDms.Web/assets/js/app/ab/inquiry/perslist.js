$(document).ready(function () {
    var options = {
        title: "Personal List",
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
                name: "InqPers",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "icon-xls" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "Department", url: "ab.api/combo/Departments", optionalText: "-- SELECT ONE --" },
        { name: "Position", url: "ab.api/combo/positions", optionalText: "-- SELECT ALL --", cascade: { name: "Department" } },
    ]);
    widget.default = { Status: "1" };
    widget.render(function () {
        widget.populate(widget.default);
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $("select[name=Position],select[name=Status]").on("change", refreshGrid);
    });

    function refreshGrid() {
        widget.kgrid({
            url: "ab.api/inquiry/employees",
            name: "InqPers",
            params: $("#pnlFilter").serializeObject(),
            columns: [
                { field: "EmployeeID", width: 120, title: "NIK" },
                { field: "EmployeeName", width: 250, title: "Name", filterable: { extra: false, operators: { string: { contains: "Contains", startswith: "Starts with", } } } },
                { field: "TeamLeaderName", width: 250, title: "Leader Name" },
                { field: "LastPosition", title: "Last Position" },
                { field: "SubOrdinates", width: 80, title: "Subs" },
                { field: "Status", width: 100, title: "Status" },
            ],
            detailInit: detailInit
        });
    }

    function detailInit(e) {
        widget.post("ab.api/inquiry/employeesubordinates", {
            EmployeeID: e.data.EmployeeID,
            Status: $("select[name=Status]").val()
        }, function (data) {
            if (data.length > 0) {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: data, pageSize: 10 },
                    pageable: true,
                    columns: [
                        { field: "EmployeeID", width: 120, title: "NIK" },
                        { field: "EmployeeName", width: 280, title: "Name" },
                        { field: "JoinDate", title: "Join Date", width: 160, template: "#= ((JoinDate === undefined) ? \"\" : moment(JoinDate).format('DD MMM YYYY')) #" },
                        { field: "LastPosition", title: "Last Position" },
                    ]
                });
            }
            else {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: [{ Info: "Employee ini tidak memiliki sub-ordinate" }] },
                    columns: [{ field: "Info", title: "Info" }]
                });
            }
        })
    }

    function exportXls() {
        widget.exportXls({
            name: "InqPers",
            type: "kgrid",
            fileName: "personal_list",
            items: [
                { field: "EmployeeID", width: 120, title: "NIK", type: "text" },
                { field: "EmployeeName", width: 250, title: "Name" },
                { field: "TeamLeaderName", width: 250, title: "Leader Name" },
                { field: "LastPosition", title: "Last Position" },
                { field: "SubOrdinates", width: 80, title: "Subs" },
                { field: "Status", width: 100, title: "Status" },
            ]
        });
    }
});
