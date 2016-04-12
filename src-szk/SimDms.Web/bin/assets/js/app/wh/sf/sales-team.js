$(document).ready(function () {
    var options = {
        xtype: "panels",
        title: "Sales Team",
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            //{ name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                name: "pnlFilter",
                //title: "Filter",
                items: [
                    { name: "GroupArea", type: "select", cls: "span4", text: "Area", readonly: false, opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", type: "select", cls: "span6", text: "Dealer Name", readonly: false, opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", type: "select", cls: "span6", text: "Branch Name", readonly: false, opt_text: "-- SELECT ALL --" },
                ]
            },
            {
                name: "gridSalesTeam",
                xtype: "k-grid"
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.default = { CompanyCode: "", ByDate: new Date() };

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
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { CompanyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });

        var buttonRefresh = $("#btnRefresh");
        var buttonExportXls = $("#btnExportXls");

        buttonRefresh.off();
        buttonExportXls.off();

        buttonRefresh.on("click", function (evt) {
            reloadData();
        });
        buttonExportXls.on("click", function (evt) {
            exportXls();
        });
    });

    function reloadData() {
        var params = {
            GroupArea: $("[name='GroupArea']").val(),
            CompanyCode: $("[name='CompanyCode']").val(),
            BranchCode: $("[name='BranchCode']").val(),
            ByDate: $("[name='ByDate']").val(),
        };
        var url = "wh.api/Inquiry/SfmSalesTeamHeaderNew";

        widget.kgrid({
            url: url,
            name: "gridSalesTeam",
            serverBinding: true,
            pageSize: 10,
            params: params,
            columns: [
                //{ field: "Area", title: "Area", width: 400 },
                //{ field: "CompanyName", title: "Dealer Name", width: 400 },
                { field: "BranchName", title: "Outlet Name", width: 400 },
            ],
            detailInit: detailInit
        });
    }

    function detailInit(e) {
        var params = {
            GroupArea: $("[name='GroupArea']").val(),
            CompanyCode: e.data.CompanyCode,
            BranchCode: e.data.BranchCode,
            GroupNoNew: e.data.GroupArea,
        };

        widget.post("wh.api/inquiry/SfmSalesTeamNew", params, function (data) {
            if (data.length > 0) {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: data, pageSize: 10 },
                    pageable: true,
                    columns: [
                        { field: "LeaderName", title: "Leader", width: 250 },
                        { field: "EmployeeID", title: "NIK", width: 150 },
                        { field: "EmployeeName", title: "Employee Name", width: 250 },
                        { field: "Department", title: "Department", width: 100 },
                        { field: "PositionName", title: "Position", width: 150 },
                        { field: "GradeName", title: "Grade", width: 150 },
                    ]
                });
            }
            else {
                $("<div/>").appendTo(e.detailCell).kendoGrid({
                    dataSource: { data: [{ Info: "Cabang ini tidak memiliki karyawan." }] },
                    columns: [{ field: "Info", title: "Info" }]
                });
            }
        })
    }

    function exportXls() {
        var params = {
            GroupArea: $("[name='GroupArea']").val(),
            CompanyCode: $("[name='CompanyCode']").val(),
            BranchCode: $("[name='BranchCode']").val(),
            ByDate: $("[name='ByDate']").val(),
        };
        var url = "wh.api/Report/SfmSalesTeamNew";

        widget.exportXls({
            source: url,
            params: params,
            fileName: "Sales Team",
            items: [
                { name: "CompanyName", text: "Dealer Name", width: 400 },
                { name: "BranchName", text: "Outlet Name", width: 400 },
                { name: "LeaderName", text: "", width: 250 },
                { name: "EmployeeID", text: "", width: 250 },
                { name: "EmployeeName", text: "", width: 250 },
                { name: "Department", text: "", width: 250 },
                { name: "PositionName", text: "", width: 250 },
                { name: "GradeName", text: "", width: 250 },
    ]
        });
    }
});

