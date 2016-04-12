$(document).ready(function () {
    var options = {
        title: "Inquiry - Data Summary",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Dept - Position",
                        type: "controls",
                        items: [
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
                title: "Data",
                xtype: "k-grid",
            },
        ],
    }
    var widget = new SimDms.Widget(options);

    widget.setSelect([
        { name: "Position", url: "ab.api/combo/positions", optionalText: "-- SELECT ALL --" },
    ]);
    widget.render();
});
