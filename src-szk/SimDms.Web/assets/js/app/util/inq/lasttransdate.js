$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Dealer Info with SDMS Version",
        xtype: "panels",
        toolbars: [
            { action: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { action: 'exportexcel', text: 'Export to Excel', icon: 'fa fa-file-excel-o', cls: '', name: 'exportToExcel' },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        name: 'ProductType', text: 'Product Type', type: 'select', cls: 'span5', opt_text: "-- ALL -- ",
                        items: [
                            { text: "2 Wheelers", value: "2W" },
                            { text: "4 Wheelers", value: "4W" },
                        ]
                    },
                    { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span5' }
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
                default:
                    break;
            }
        },
    });

    widget.render(function () {
        widget.setSelect([
            {
                name: "Dealer",
                url: "wh.api/combo/ListDealers",
                optionalText: "-- ALL -- ",
                cascade: {
                    name: "ProductType"
                }

            }
        ]);
    });

    $('#pnlFilter #ProductType').on('change', refresh);
    $('#pnlFilter #Dealer').on('change', refresh);

    refresh();

    function refresh() {
        var filter = widget.serializeObject('pnlFilter');
        widget.kgrid({
            url: "wh.api/inquiry/InqLastTransDateInfo",
            name: "pnlResult",
            params: filter,
            sort: [
                { field: "DealerCode", dir: "asc" },
            ],
            columns: [
                { title: 'Dealer Code', field: 'DealerCode', width: 150 },
                { title: 'Dealer Name', field: 'DealerName', width: 320 },
                { title: 'Dealer Abbr', field: 'DealerAbbr', width: 120 },
                { title: 'Branch Code', field: 'BranchCode', width: 120 },
                { title: 'Branch Abbr', field: 'BranchAbbr', width: 200 },
                { title: 'Product Type', field: 'ProductType', width: 120 },
                { title: 'Go Live Date', field: 'GoLiveDate', width: 150, type: 'date' },
                { title: 'Version', field: 'Version', width: 150 },
                { title: 'Last Sales Date', field: 'LastSalesDate', width: 200, type: 'date' },
                { title: 'Last Spare Date', field: 'LastSpareDate', width: 200, type: 'date' },
                { title: 'Last Service Date', field: 'LastServiceDate', width: 200, type: 'date' },
                { title: 'Last AP Date', field: 'LastAPDate', width: 150, type: 'date' },
                { title: 'Last AR Date', field: 'LastARDate', width: 150, type: 'date' },
                { title: 'Last GL Date', field: 'LastGLDate', width: 150, type: 'date' },
            ],
        });
    }

    $("#exportToExcel").on("click", function (e) {

        var params = widget.serializeObject('pnlFilter');

        e.preventDefault();
        $('.page > .ajax-loader').show();

        $.fileDownload('doreport/DealerInfo.xlsx', {
            httpMethod: "POST",
            //preparingMessageHtml: "We are preparing your report, please wait...",
            //failMessageHtml: "There was a problem generating your report, please try again.",
            data: params
        }).done(function () {
            $('.page > .ajax-loader').hide();
        });

    });

});
