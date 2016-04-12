$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Collect Data Log",
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
                    { name: 'DealerCode', text: 'Dealer', type: 'select', cls: 'span5' },
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
            DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
            DateTo: new Date()
        }
        widget.populate(filter);

        widget.post("wh.api/combo/GnCollDataLogFilter", {}, function (result) {
            widget.bind({
                name: 'DealerCode',
                text: '-- SELECT DEALER --',
                data: result[0],
                onChange: refresh
            });
            widget.bind({
                name: 'TableName',
                text: '-- SELECT TABLE --',
                data: result[1],
                parent: 'DealerCode',
                onChange: refresh
            });
        });

        setTimeout(refresh, 1000);
    });


    function refresh() {
        var filter = widget.serializeObject('pnlFilter');
        widget.kgrid({
            url: "wh.api/inquiry/GnCollDataLog",
            name: "pnlResult",
            params: filter,
            sort: [
                { field: "UploadDate", dir: "asc" },
            ],
            columns: [
                { field: "UploadDate", width: 120, title: "Upload Date", type: 'date' },
                { field: "DealerName", width: 320, title: "Dealer Name" },
                { field: "TableName", width: 220, title: "Table Name" },
                { field: "PkgUpload", width: 80, title: "Upload" },
                { field: "PkgSuccess", width: 80, title: "Success" },
                { field: "PkgError", width: 80, title: "Error" },
                { field: "PkgInPogress", width: 80, title: "In Pogress" },
            ],
        });
    }
});
