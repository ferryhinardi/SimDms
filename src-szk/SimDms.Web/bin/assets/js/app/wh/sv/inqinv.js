$(document).ready(function () {
    var options = {
        title: "Inquiry Invoice",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Organization", cls: "span6", type: "select" },
                        ]
                    },
                    {
                        text: "Outlet Name",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span4", type: "select", opt_text: "-- SELECT ONE --" },
                            { name: "Year", text: "Year", cls: "span2", type: "select" },
                        ]
                    },
                ],
            },
            {
                name: "KGrid",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
        ],
    }
    var widget = new SimDms.Widget(options);
    var kgrid;
    widget.setSelect([
        { name: "CompanyCode", url: "wh.api/combo/organizations", optionalText: "-- SELECT ONE --" },
        { name: "Year", url: "wh.api/combo/years", optionalText: "-- SELECT ONE --" }
    ]);
    widget.default = { Year: new Date().getFullYear() };
    widget.render(function () {
        widget.populate(widget.default);
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ONE --" });
        });
        initGrid();
    });

    $("#btnRefresh").on("click", refreshGrid);
    $("#pnlFilter select").on("change", refreshGrid);

    function initGrid() {
        kgrid = widget.kgrid({
            url: "util.api/inquiry/svinvoice",
            name: "KGrid",
            serverBinding: true,
            columns: [
                { field: "CompanyCode", title: "CompanyCode", width: 100 },
                { field: "BranchCode", title: "BranchCode", width: 100 },
                { field: "InvoiceNo", title: "InvoiceNo", width: 100 },
                { field: "InvoiceDate", title: "InvoiceDate", width: 140, filterable: false, template: "#= (InvoiceDate == undefined) ? '-' : moment(InvoiceDate).format('DD MMM YYYY - HH:mm:ss') #" },
                { field: "JobOrderNo", title: "JobOrderNo", width: 100 },
                { field: "JobOrderDate", title: "JobOrderDate", width: 140, filterable: false, template: "#= (JobOrderDate == undefined) ? '-' : moment(JobOrderDate).format('DD MMM YYYY - HH:mm:ss') #" },
                { field: "JobType", title: "JobType", width: 100 },
                { field: "TotalSrvAmt", title: "TotalSrvAmt", width: 140, filterable: false, format: "{0:n0}", attributes: { "class": "right" } },
            ],
            dataBound: function () {
                $("#KGrid thead [data-field=TotalSrvAmt] .k-link").addClass("right");
            }
        });
    }

    function refreshGrid() {
        if (kgrid !== undefined) {
            var params = $("#pnlFilter").serializeObject();
            kgrid.refresh(params);
        }
    }
});
