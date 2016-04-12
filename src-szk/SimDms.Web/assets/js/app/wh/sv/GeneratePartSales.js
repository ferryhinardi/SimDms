var widget = new SimDms.Widget({
    title: 'Generate Part Sales',
    xtype: 'panels',
    toolbars: [
        { text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
        { text: 'Collapse', action: 'collapse', icon: 'fa fa-arrow-up' },
        { text: 'Expand', action: 'expand', icon: 'fa fa-arrow-down', cls: 'hide' },
    ],
    panels: [
        {
            name: "pnlFilter",
            items: [
                {
                    text: "Periode",
                    type: "controls",
                    cls: 'span5',
                    items: [
                        { name: 'DateFrom', text: 'Date From', type: 'datepicker', cls: 'span4' },
                        { name: 'DateTo', text: 'Date To', type: 'datepicker', cls: 'span4' },
                    ]
                },
                { name: 'Area', text: 'Area', type: 'select', cls: 'span5' },
                { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span5', opt_text: '-- SELECT ALL --' },
                { name: 'TypeOfGoods', text: 'Type Of Goods', type: 'select', cls: 'span5' },
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
                refreshGrid();
                break;
            case 'collapse':
                widget.requestFullWindow();
                widget.showToolbars(['refresh', 'expand']);
                break;
            case 'expand':
                widget.exitFullWindow();
                widget.showToolbars(['refresh', 'collapse']);
                break;
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

    widget.post("wh.api/combo/PartSalesFilter", {}, function (result) {
        widget.bind({
            name: 'Area',
            text: '-- SELECT ALL --',
            data: result[0],
            //onChange: refreshGrid
        });
        widget.bind({
            name: 'Dealer',
            text: '-- SELECT ALL --',
            data: result[1],
            parent: 'Area',
            defaultAll: true,
            //onChange: refreshGrid
        });
        widget.bind({
            name: 'TypeOfGoods',
            text: '-- SELECT ALL --',
            data: result[2],
            //onChange: refreshGrid
        });
    });
});

function refreshGrid() {
    var filter = widget.serializeObject('pnlFilter');
    widget.kgrid({
        url: "wh.api/inquiry/PartSalesViews",
        name: "pnlResult",
        params: filter,
        serverBinding: true,
        //sort: [{ field: "UpdatedDate", dir: "desc" }],
        columns: [
            { field: "DealerAbbreviation", width: 100, title: "Dealer" },
            { field: "BranchAbbreviation", width: 240, title: "Outlet" },
            { field: "InvoiceNo", width: 140, title: "Invoice No" },
            { field: "InvoiceDate", width: 120, title: "Invoice Date", type: 'date' },
            { field: "FPJNo", width: 120, title: "FPJ No" },
            { field: "FPJDate", width: 120, title: "FPJ Date", type: 'date' },
            { field: "CustomerName", width: 380, title: "Customer" },
            { field: "CustomerClass", width: 150, title: "Cust Class" },
            { field: "PartNo", width: 150, title: "PartNo" },
            { field: "PartName", width: 380, title: "Part Name" },
            { field: "TypeOfGoodsDesc", width: 180, title: "Type Of Goods" },
            { field: "QtyBill", width: 100, title: "Qty Bill", type: 'number' },
            { field: "CostPrice", width: 160, title: "Cost Price", type: 'number' },
            { field: "RetailPrice", width: 160, title: "Retail Price", type: 'number' },
            { field: "DiscPct", width: 120, title: "Disc Pct", type: 'number' },
            { field: "DiscAmt", width: 160, title: "Disc Amount", type: 'number' },
            { field: "NetSalesAmt", width: 160, title: "Net Sales Amount", type: 'number' },
        ],
    });
}