$(document).ready(function () {
    var options = {
        title: "Personal Mutation",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "GroupArea", text: "Area", cls: "span7", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Organization", cls: "span7", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Dept - Position",
                        type: "controls",
                        items: [
                            { name: "Department", text: "Department", cls: "span2", type: "select", opt_text: "-- SELECT ALL --" },
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
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
        ],
    }
    var widget = new SimDms.Widget(options);
    // widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/organizations", optionalText: "-- SELECT ONE --" }]);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.default = { Status: "1" };
    widget.render(function () {
        widget.populate(widget.default);
        $("#CompanyCode").prop('disabled', true);
        $("[name=GroupArea]").on("change", function () {
            var groupArea = $("[name=GroupArea]").val();
            if (groupArea == '' || groupArea == undefined) {
                $("#CompanyCode").prop('disabled', true);
            }
            else {
                $("#CompanyCode").prop('disabled', false);
            }

            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { LinkedModule: "mp", GroupArea: groupArea }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=Department]", url: "wh.api/combo/departments", params: { comp: $("[name=CompanyCode]").val() } });
            $("[name=Department]").prop("selectedIndex", 0);
            $("[name=Department]").change();
        });
        $("[name=Department]").on("change", function () {
            widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("[name=CompanyCode]").val(), dept: $("[name=Department]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Position]").prop("selectedIndex", 0);
            $("[name=Position]").change();
        });
    });

    $("#btnRefresh").on("click", refreshGrid);
    // $("select[name=Position],select[name=Status]").on("change", refreshGrid);

    function refreshGrid() {
        widget.kgrid({
            url: "wh.api/inquiry/employeesNew",
            name: "InqPers",
            params: $("#pnlFilter").serializeObject(),
            serverBinding: true,
            pageSize: 10,
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
                widget.post("wh.api/inquiry/employeemutations", { CompanyCode: $("[name=CompanyCode]").val(), EmployeeID: e.data.EmployeeID }, function (data) {
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
});
