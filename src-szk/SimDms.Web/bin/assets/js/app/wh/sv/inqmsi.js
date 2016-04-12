$(document).ready(function () {
    var options = {
        title: "Inquiry MSI",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Organization", cls: "span6", type: "select", opt_text: '-- SELECT ALL --' },
                        ]
                    },
                    {
                        text: "Outlet Name",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                            { name: "Year", text: "Year", cls: "span2", type: "select" },
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
    widget.setSelect([
        { name: "CompanyCode", url: "wh.api/combo/organizations", optionalText: "-- SELECT ALL --" },
        { name: "Year", url: "wh.api/combo/yearsMSI" }
    ]);
    widget.default = { Year: new Date().getFullYear() };
    widget.render(function () {
        widget.populate(widget.default);
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ONE --" });
        });
    });

    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();
        params.Department = "SERVICE",
        widget.kgrid({
            url: "wh.api/inquiry/svmsi",
            name: "InqPers",
            params: params,
            sortable: false,
            filterable: false,
            pageable: false,
            pageSize: 200,
            columns: [
                { field: "SeqNo", title: "No", width: 60 },
                { field: "MsiDesc", title: "Keterangan", width: 600 },
                { field: "Month01", title: "Jan", width: 180, type: 'decimal' },
                { field: "Month02", title: "Feb", width: 180, type: 'decimal' },
                { field: "Month03", title: "Mar", width: 180, type: 'decimal' },
                { field: "Month04", title: "Apr", width: 180, type: 'decimal' },
                { field: "Month05", title: "May", width: 180, type: 'decimal' },
                { field: "Month06", title: "Jun", width: 180, type: 'decimal' },
                { field: "Month07", title: "Jul", width: 180, type: 'decimal' },
                { field: "Month08", title: "Aug", width: 180, type: 'decimal' },
                { field: "Month09", title: "Sep", width: 180, type: 'decimal' },
                { field: "Month10", title: "Oct", width: 180, type: 'decimal' },
                { field: "Month11", title: "Nov", width: 180, type: 'decimal' },
                { field: "Month12", title: "Dec", width: 180, type: 'decimal' },
            ],
        });
    }

    function exportXls() {
        widget.exportXls({
            name: "InqPers",
            type: "kgrid",
            fileName: "msi_data",
            items: [
                { name: "SeqNo", text: "No" },
                { name: "MsiDesc", text: "Keterangan" },
                { name: "Month01", text: "Jan" },
                { name: "Month02", text: "Feb" },
                { name: "Month03", text: "Mar" },
                { name: "Month04", text: "Apr" },
                { name: "Month05", text: "May" },
                { name: "Month06", text: "Jun" },
                { name: "Month07", text: "Jul" },
                { name: "Month08", text: "Aug" },
                { name: "Month09", text: "Sep" },
                { name: "Month10", text: "Oct" },
                { name: "Month11", text: "Nov" },
                { name: "Month12", text: "Dec" },
            ]
        });
    }
});
