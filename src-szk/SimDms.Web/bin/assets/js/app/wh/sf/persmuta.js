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
                            { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Organization", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Position",
                        type: "controls",
                        items: [
                            { name: "Position", text: "Position", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
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
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    // widget.setSelect([{ name: "CompanyCode", url: "wh.api/combo/DealerList?LinkedModule=mp", optionalText: "-- SELECT ONE --" }]);
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
            widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("[name=CompanyCode]").val(), dept: "SALES" }, optionalText: "-- SELECT ALL --" });
            $("[name=Position]").prop("selectedIndex", 0);
            $("[name=Position]").change();
        });
    });

    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    // $("select[name=Position],select[name=Status]").on("change", refreshGrid);

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();
        params.Department = "SALES",
        widget.kgrid({
            url: "wh.api/inquiry/employeesNew",
            name: "PersMuta",
            params: params,
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

    function exportXls() {
        var params = $("#pnlFilter").serializeObject();
        params.Department = "SALES";

        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/report/InqEmployeesNew?type=mutation",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Personal Mutation Employee';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });

        /*
        widget.exportXls({
            name: "PersMuta",
            type: "kgrid",
            fileName: "mutation",
            items: [
                { name: "EmployeeID", width: 120, text: "NIK" },
                { name: "EmployeeName", text: "Name" },
                { name: "JoinDate", text: "Join Date", width: 160, type: "date" },
                { name: "LastBranch", width: 120, text: "Last Branch" },
                { name: "LastPosition", text: "Last Position" },
                { name: "Status", width: 100, text: "Status" },
                { name: "IsValid", width: 100, text: "Is Valid" },
            ]
        });
        */
    }
});
