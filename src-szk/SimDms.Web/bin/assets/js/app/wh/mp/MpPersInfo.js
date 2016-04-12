$(document).ready(function () {
    var options = {
        title: "Personal List",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Dealer", cls: "span7", type: "select" },
                        ]
                    },
                    {
                        text: "Outlet Name",
                        type: "controls",
                        items: [
                            { name: "BranchCode", text: "Outlet", cls: "span7", type: "select" },
                        ]
                    },
                    {
                        text: "Dept - Position",
                        type: "controls",
                        items: [
                            { name: "Department", text: "Department", cls: "span2", type: "select", opt_text: "-- SELECT ALL -- " },
                            { name: "Position", text: "Position", cls: "span3", type: "select", opt_text: "-- SELECT ALL --" },
                            {
                                name: "PersonnelStatus", text: "Status", cls: "span2", type: "select", opt_text: "ALL STATUS",
                                items: [
                                    { text: "AKTIF", value: "1" },
                                    { text: "NON AKTIF", value: "2" },
                                    { text: "KELUAR", value: "3" },
                                    { text: "PENSIUN", value: "4" },
                                ]
                            },
                        ]
                    },
                    {
                        text: "Employee Name",
                        type: "controls",
                        items: [
                            { name: "EmployeeName", text: "Employee Name", cls: "span7", type: "text" },
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
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/DealerList", optionalText: "-- SELECT ONE --", params: { LinkedModule: "MP" } }]);
    widget.default = { Status: "1" };
    widget.render(function () {
        renderCallback();
    });

    function renderCallback() {
        widget.populate(widget.default);

        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/Branchs", params: { comp: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=Department]", url: "wh.api/combo/departments", params: { comp: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Department]").change();
        });
        $("[name=Department]").on("change", function () {
            widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("[name=CompanyCode]").val(), dept: $("[name=Department]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Position]").change();
        });
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        $('select').on('change', refreshGrid);
        $('[name="EmployeeName"]').change(refreshGrid);
    }

    function refreshGrid() {
        if (widget.isNullOrEmpty($('[name="CompanyCode"]').val()) == false) {
            widget.kgrid({
                url: "wh.api/inquiry/MpPersInfo",
                name: "InqPers",
                params: $("#pnlFilter").serializeObject(),
                serverBinding: true,
                columns: [
                    { field: "EmployeeID", width: 150, title: "NIK" },
                    { field: "EmployeeName", width: 300, title: "Name" },
                    { field: "Position", width: 200, title: "Position" },
                    { field: "Grade", width: 150, title: "Grade" },
                    { field: "JoinDate", width: 120, title: "Join Date", template: '#= (JoinDate == undefined) ? "" : moment(JoinDate).format("DD MMM YYYY") #' },
                    { field: "Telephone1", width: 250, title: "Telephone" },
                    { field: "Gender", width: 150, title: "Gender" }
                ],
            });
        }
    }

    function exportXls() {
        console.log('Export Xls');
    }
});
