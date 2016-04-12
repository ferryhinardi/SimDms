$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Dealer Info",
        xtype: "panels",
        toolbars: [
            { action: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { action: 'exportexcel', text: 'Export to Excel', icon: 'fa fa-file-excel-o', cls: '' },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Profit Center",
                        type: "controls",
                        items: [
                            {
                                name: 'ProductType', text: 'Product Type', type: 'select', cls: 'span3', opt_text: "-- ALL DATA -- ",
                                items: [
                                    { text: "2 Wheelers", value: "2W" },
                                    { text: "4 Wheelers", value: "4W" },
                                ]
                            },
                        ]
                    },
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
                    widget.showToolbars(['refresh', 'expand', 'exportexcel']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'collapse', 'exportexcel']);
                    break;
                case 'exportexcel':
                    exportToExcel();

                default:
                    break;
            }
        },
    });

    widget.render(function () {
        setTimeout(refresh, 1000);
        $('#pnlFilter #ProductType').on('change', refresh);
    });

    function refresh() {
        var filter = widget.serializeObject('pnlFilter');
        widget.kgrid({
            url: "wh.api/inquiry/DealerInfo",
            name: "pnlResult",
            params: filter,
            sort: [
                { field: "ProductType", dir: "asc" },
                { field: "DealerCode", dir: "asc" },
                { field: "OutletCode", dir: "asc" },
            ],
            columns: [
                { field: "ProductType", width: 60, title: "Type" },
                { field: "DealerCode", width: 140, title: "Dealer Code" },
                { field: "DealerName", width: 300, title: "Dealer Name" },
                { field: "OutletCode", width: 140, title: "Outlet Code" },
                { field: "OutletName", width: 450, title: "Outlet Name" },
                { field: "SalesDate", width: 170, title: "Sales Date", type: "datetime" },
                { field: "ServiceDate", width: 170, title: "Service Date", type: "datetime" },
                { field: "SparepartDate", width: 170, title: "Sparepart Date", type: "datetime" },
                { field: "ApDate", width: 170, title: "AP Date", type: "datetime" },
                { field: "ArDate", width: 170, title: "AR Date", type: "datetime" },
                { field: "GlDate", width: 170, title: "GL Date", type: "datetime" },
            ],
        });
    }

    function exportToExcel() {
        var url = "wh.api/InquiryProd/DealerInfo?";
        var filter = widget.serializeObject('pnlFilter');
        var params = '';

        $.each(filter || [], function (key, val) {
            params += key + '=' + val + '&';
        });
        params = params.substring(0, params.length - 1);

        url += params;
        window.location = url;
    }
});
