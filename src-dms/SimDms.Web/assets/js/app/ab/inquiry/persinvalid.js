$(document).ready(function () {
    var options = {
        title: "Personal Invalid",
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
    widget.setSelect([
        { name: "Department", url: "ab.api/combo/departments", optionalText: "-- SELECT ONE --" },
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
            url: "ab.api/inquiry/employeeinvalid",
            name: "pnlKGrid",
            params: $("#pnlFilter").serializeObject(),
            columns: [
                { field: "EmployeeID", width: 120, title: "NIK" },
                { field: "EmployeeName", width: 250, title: "Name" },
                { field: "LastPosition", title: "Last Position" },
                { field: "HaveJoined", width: 120, title: "Have Joined?" },
                { field: "HavePosition", width: 180, title: "Have Position?" },
                { field: "Status", width: 100, title: "Status" },
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "pnlKGrid",
            type: "kgrid",
            fileName: "pers_invalid",
            items: [
                { name: "EmployeeID", text: "NIK", type: "text" },
                { name: "EmployeeName", text: "Name" },
                { name: "LastPosition", text: "Last Position" },
                { name: "HaveJoined", text: "Have Joined?" },
                { name: "HavePosition", text: "Have Position?" },
                { name: "Status", text: "Status" },
            ]
        });
    }
});
