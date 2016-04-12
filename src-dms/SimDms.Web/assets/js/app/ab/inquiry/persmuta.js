$(document).ready(function () {
    var options = {
        title: "Personal Mutation",
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
                name: "PersMuta",
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
            url: "ab.api/inquiry/employees",
            name: "PersMuta",
            params: $("#pnlFilter").serializeObject(),
            columns: [
                { field: "EmployeeID", width: 120, title: "NIK" },
                { field: "EmployeeName", title: "Name" },
                { field: "JoinDate", title: "Join Date", width: 160, template: "#= (JoinDate == undefined) ? '-' : moment(JoinDate).format('DD MMM YYYY') #" },
                { field: "LastBranch", width: 120, title: "Last Branch" },
                { field: "LastPosition", title: "Last Position" },
                { field: "Status", width: 100, title: "Status" },
                { field: "IsValid", width: 100, title: "Is Valid" },
            ],
            detailInit: function (e) {
                widget.post("ab.api/inquiry/employeemutations", { EmployeeID: e.data.EmployeeID }, function (data) {
                    if (data.length > 0) {
                        $("<div/>").appendTo(e.detailCell).kendoGrid({
                            dataSource: { data: data },
                            columns: [
                                { field: "MutationDate", title: "Date", width: 160, template: "#= moment(MutationDate).format('DD MMM YYYY') #" },
                                { field: "Branch", title: "Branch / Outlet" },
                                { field: "IsJoinDate", title: "Join", width: 110, template: "#= (IsJoinDate) ? 'Y' : 'N'  #" },
                            ]
                        });
                    }
                    else {
                        $("<div/>").appendTo(e.detailCell).kendoGrid({
                            dataSource: { data: [{ Info: "Employee ini belum pernah mutasi / join, silahkan dilengkapi datanya" }] },
                            columns: [{ field: "Info", title: "Info" }]
                        });
                    }
                })
            },
        });
    }


    function exportXls() {
        widget.exportXls({
            name: "PersMuta",
            type: "kgrid",
            fileName: "mutation",
            items: [
                { name: "EmployeeID", width: 120, text: "NIK", type: "text" },
                { name: "EmployeeName", text: "Name" },
                { name: "JoinDate", text: "Join Date", width: 160, type: "date" },
                { name: "LastBranch", width: 120, text: "Last Branch" },
                { name: "LastPosition", text: "Last Position" },
                { name: "Status", width: 100, text: "Status" },
                { name: "IsValid", width: 100, text: "Is Valid" },
            ]
        });
    }
});
