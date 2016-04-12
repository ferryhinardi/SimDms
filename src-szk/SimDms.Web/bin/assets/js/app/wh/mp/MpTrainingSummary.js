$(document).ready(function () {
    var options = {
        title: "Training Summary",
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
                        text: "Periode",
                        type: "controls",
                        items: [
                            { name: "Periode", text: "Periode", cls: "span2", type: "datepicker" },
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
        $('[name="Periode"]').val(moment(new Date()).format('DD-MMM-YYYY'));
    }

    function refreshGrid() {
        if (widget.isNullOrEmpty($('[name="CompanyCode"]').val()) == false) {
            widget.kgrid({
                url: "wh.api/inquiry/MpTrainingSummary",
                name: "InqPers",
                params: $("#pnlFilter").serializeObject(),
                //serverBinding: true,
                columns: [
                    { field: "BranchName", width: 500, title: "Outlet" },
                    { field: "BM", width: 140, title: "BM", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "BMT", width: 140, title: "BM Trained", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "BMN", width: 140, title: "BM Non Trained", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SH", width: 140, title: "SH", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SHT", width: 140, title: "SH Trained", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SHN", width: 140, title: "SH Non Trained", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SC", width: 140, title: "SC", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SCT", width: 140, title: "SC Trained", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SCN", width: 140, title: "SC Non Trained", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "S", width: 140, title: "S", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "ST", width: 140, title: "S Trained", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "SN", width: 140, title: "S Non Trained", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "T", width: 140, title: "Total Man Power", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "TT", width: 140, title: "Total Trained", footerTemplate: '#=sum#', type: 'align-right' },
                    { field: "TN", width: 140, title: "Total Non Trained", footerTemplate: '#=sum#', type: 'align-right' },
                ],
                aggregate: [
                    { field: "BM", aggregate: "sum" },
                    { field: "BMT", aggregate: "sum" },
                    { field: "BMN", aggregate: "sum" },
                    { field: "SH", aggregate: "sum" },
                    { field: "SHT", aggregate: "sum" },
                    { field: "SHN", aggregate: "sum" },
                    { field: "SC", aggregate: "sum" },
                    { field: "SCT", aggregate: "sum" },
                    { field: "SCN", aggregate: "sum" },
                    { field: "S", aggregate: "sum" },
                    { field: "ST", aggregate: "sum" },
                    { field: "SN", aggregate: "sum" },
                    { field: "T", aggregate: "sum" },
                    { field: "TT", aggregate: "sum" },
                    { field: "TN", aggregate: "sum" },
                ],
            });
        }
    }

    function exportXls() {
        console.log('Exporting xls ...');
    }
});
