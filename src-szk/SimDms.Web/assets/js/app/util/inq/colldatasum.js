$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Collect Data Summary",
        xtype: "panels",
        toolbars: [
            { action: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: 'TableName', text: 'Table Name', type: 'select', cls: 'span5' },
                    {
                        text: "Periode",
                        type: "controls", items: [
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                        ]
                    }
                ]
            },
            {
                name: "pnlResult",
                xtype: "k-grid",
            },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'refresh':
                    refresh();
                    break;
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'collapse']);
                    break;
                case 'exportexcel':
                    exportToExcel();

                default:
                    break;
            }
        },
    });

    widget.render(function () {
        var filter = {
            DateFrom: new Date(),
            DateTo: new Date()
        }
        widget.populate(filter);

        widget.post("wh.api/combo/GnCollDataLogFilter", {}, function (result) {
            widget.bind({
                name: 'TableName',
                text: '-- SELECT ALL --',
                data: result[2],
                onChange: refresh
            });
        });

        setTimeout(refresh, 1000);
    });


    function refresh() {
        var filter = widget.serializeObject('pnlFilter');
        widget.kgrid({
            url: "wh.api/inquiry/GnCollDataSumLog",
            name: "pnlResult",
            params: filter,
            sort: [
                { field: "PkgReceived", dir: "desc" },
            ],
            columns: [
                { field: "DealerCode", width: 180, title: "Dealer Code" },
                { field: "DealerName", title: "Dealer Name" },
                { field: "PkgReceived", width: 100, title: "Pkg Rcv", type: 'number' },
                { field: "PkgSuccess", width: 100, title: "Pkg Success", type: 'number' },
                { field: "PkgError", width: 100, title: "Pkg Fail", type: 'number' },
                { field: "PkgInPogress", width: 120, title: "Pkg InProg", type: 'number' },
            ],
        });
    }
});


