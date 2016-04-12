"use strict";
$(document).ready(function () {
    var options = {
        title: "VOR Report Consistency",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer Name", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet Name", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                ],
            },
            {
                name: "DataTable",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { text: "Refresh", action: "refresh", icon: "fa fa-refresh" },
            { text: "Print", action: "export", icon: "fa fa-download" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case "refresh":
                    refreshGrid();
                    break;
                case "export":
                    exportXls();
                    break;
            }
        }
    };

    var widget = new SimDms.Widget(options);
});