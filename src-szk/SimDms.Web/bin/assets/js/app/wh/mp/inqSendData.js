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
                    text: "Tipe Data",
                    type: "controls",
                    items: [
                        {
                            name: 'TableName', text: 'Table Name', type: 'select', cls: 'span4', opt_text: "-- SELECT TABLE --",
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
                var param = widget.serializeObject('pnlFilter');
                widget.post("wh.api/inquiry/InqDataMp", param, refreshData);
                break;
            default:
                break;
        }
    },
});

widget.render(function () {
    widget.setSelect([{ name: "TableName", url: "wh.api/combo/MpTableList", optionalText: "-- SELECT DATA --" }]);
    $('[name=TableName]').on('change', function () {
        var param = widget.serializeObject('pnlFilter');
        widget.post("wh.api/inquiry/InqDataMp", param, refreshData);
    });
    SimDms.onTenSecondChanged = function () {
        var param = widget.serializeObject('pnlFilter');
        widget.xpost("wh.api/inquiry/InqDataMp", param, refreshData);
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