var widget = new SimDms.Widget({
    title: 'Generate Part Sales (4W)',
    xtype: 'panels',
    toolbars: [
        { text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
        { text: 'Expand', action: 'expand', icon: 'fa fa-expand' },
        { text: 'Collapse', action: 'collapse', icon: 'fa fa-compress', cls: 'hide' },
        { text: 'Export to Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o', cls: '' },
    ],
    panels: [
        {
            name: "pnlFilter",
            items: [
                {
                    text: "Periode",
                    type: "controls",
                    cls: 'span6',
                    items: [
                        { name: 'DateFrom', text: 'Date From', type: 'datepicker', cls: 'span3' },
                        { name: 'DateTo', text: 'Date To', type: 'datepicker', cls: 'span3' },
                    ]
                },
                {
                    text: "Area / Dealer",
                    type: "controls",
                    cls: 'span6',
                    items: [
                        { name: 'Area', text: 'Area', type: 'select', cls: 'span3' },
                        { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span5', opt_text: '-- SELECT ALL --' },
                    ]
                },
                { name: 'TypeOfGoods', text: 'Type Of Goods', type: 'select', cls: 'span6' },
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
                widget.exitFullWindow();
                widget.showToolbars(['refresh', 'expand', 'exportToExcel']);
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['refresh', 'collapse', 'exportToExcel']);
                break;
            case 'exportToExcel':
                exportToExcel();
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

    //widget.post("wh.api/combo/PartSalesFilter", function (result) {
    //    widget.bind({
    //        name: 'Area',
    //        text: '-- SELECT ALL --',
    //        data: result[0],
    //       // onChange: function () { setTimeout(refreshGrid, 500) }
    //    });
    //    widget.bind({
    //        name: 'Dealer',
    //        text: '-- SELECT ALL --',
    //        data: result[1],
    //        parent: 'Area',
    //        defaultAll: true,
    //      //  onChange: function () { setTimeout(refreshGrid, 500) }
    //    });
    //    widget.bind({
    //        name: 'TypeOfGoods',
    //        text: '-- SELECT ALL --',
    //        data: result[2],
    //      //  onChange: function () { setTimeout(refreshGrid, 500) }
    //    });
    //});
    $('#Dealer').attr('disabled', 'disabled');
    initComboArea();
    widget.select({ selector: "[name=TypeOfGoods]", url: "wh.api/combo/ListTypeOfGoods", optionalText: "-- SELECT ALL --" });
});

function initComboArea() {
    $.ajax({
        async: false,
        type: "POST",
        //url: 'wh.api/combo/Areas',
        url: 'wh.api/combo/GroupAreas',
        success: function (data) {
            widget.setItems({ name: "Area", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            if (data.length == 1) $('#Area').select2('val', data[0].value);
        }
    });
}

function initComboDealer() {
    $.ajax({
        async: false,
        type: "POST",
        data: {
            //area: $('#Area').select2('val')
            GroupArea: $('#Area').select2('val')
        },
        //url: 'wh.api/combo/ListDealersNew',
        url: 'wh.api/combo/ComboDealerList',
        success: function (data) {
            widget.setItems({ name: "Dealer", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            if (data.length == 1) $('#Dealer').select2('val', data[0].value);
        }
    });
}

$('#Area').on('change', function () {
    if ($('#Area').val() != "") {
        initComboDealer();
        $('#Dealer').removeAttr('disabled');
    } else {
        $('#Dealer').attr('disabled', 'disabled');
        $('#Dealer').select2('val', "");
    }
    $('#Dealer').select2('val', "");
});

function refreshGrid() {
    var filter = widget.serializeObject('pnlFilter');
    widget.kgrid({
        //url: "wh.api/inquiry/PartSalesViews",
        url: "wh.api/inquiry/HistPartSales4W",
        name: "pnlResult",
        params: filter,
        serverBinding: true,
        //sort: [{ field: "UpdatedDate", dir: "desc" }],
        sort: [
            { field: "CompanyCode", dir: "asc" },
            { field: "BranchCode", dir: "asc" },
            { field: "InvoiceDate", dir: "asc" },
            { field: "InvoiceNo", dir: "asc" },
        ],
        columns: [
            { field: "DealerName", width: 300, title: "Dealer" },
            { field: "BranchName", width: 400, title: "Outlet" },
            { field: "InvoiceNo", width: 140, title: "Invoice No" },
            { field: "InvoiceDate", width: 120, title: "Invoice Date", type: 'date' },
            { field: "FPJNo", width: 120, title: "FPJ No" },
            { field: "FPJDate", width: 120, title: "FPJ Date", type: 'date' },
            { field: "CustomerName", width: 420, title: "Customer" },
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

function exportToExcel() {
    var url = "wh.api/InquiryProd/GeneratePartSalesData4W?";
    var filter = widget.serializeObject('pnlFilter');
    var params = ''

    $.each(filter || [], function (key, val) {
        params += key + '=' + val + '&';
    });
    params = params.substring(0, params.length - 1);

    url += params;
    window.location = url;
}