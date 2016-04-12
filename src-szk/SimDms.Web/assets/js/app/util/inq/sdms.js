var widget = new SimDms.Widget({
    title: "Inquiry SDMS",
    xtype: "panels",
    toolbars: [
        { name: "btnRefresh", text: "Refresh", action: "refresh", icon: "fa fa-refresh" },
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
                            name: 'ProfitCenter', text: 'Profit Center', type: 'select', cls: 'span3', opt_text: "-- ALL PROFIT CENTER -- ",
                            items: [
                                { text: "General", value: "gn" },
                                { text: "Sales", value: "sl" },
                                { text: "Man Power", value: "mp" },
                                { text: "Service", value: "sv" },
                            ]
                        },
                        { name: 'TableName', text: 'Table Name', type: 'select', cls: 'span5', opt_text: "-- SELECT TABLE --" },
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
                var param = widget.serializeObject('pnlFilter');
                widget.post("wh.api/inquiry/InqDataSdms", param, refreshData);
                break;
            default:
                break;
        }
    },
});

widget.render(function () {
    widget.selectparam({
        name: "TableName", url: "wh.api/combo/tablelist",
        params: [{ name: "ProfitCenter", param: "pcenter" }],
        optionalText: "-- SELECT TABLE --"
    });
    $('[name=ProfitCenter]').change();
    $('[name=TableName]').on('change', function () {
        var param = widget.serializeObject('pnlFilter');
        widget.post("wh.api/inquiry/InqDataSdms", param, refreshData);
    });
    SimDms.onTenSecondChanged = function () {
        var param = widget.serializeObject('pnlFilter');
        widget.xpost("wh.api/inquiry/InqDataSdms", param, refreshData);
    };
    renderInitTable();
});

function renderInitTable() {
    var html = '';
    html += '<table class="dashboard">';
    html += ' <thead>';
    html += '  <tr>';
    html += '   <th class="title" style="width:200px">Company Code</th>';
    html += '   <th class="title">Company Name</th>';
    html += '   <th class="title" style="width:320px">Table Name</th>';
    html += '   <th class="title" style="width:120px">Delay</th>';
    html += '  </tr>';
    html += ' </thead>';
    html += ' <tbody></tbody>';
    html += '</table>';
    $('#pnlResult').html(html)
}

function refreshData(data) {
    var html = '';
    (data || []).forEach(function (row) {
        html += '<tr>';
        html += '<td>' + row['CompanyCode'] + '</td>';
        html += '<td>' + row['CompanyName'] + '</td>';
        html += '<td>' + (row['TableName'] || row['DataType']) + '</td>';
        html += '<td class="number"><span class="animated">' + row['DelayDate'] + '</span></td>';
        html += '</tr>';
    });
    $('#pnlResult table tbody').html(html)
}