var widget = new SimDms.Widget({
    title: 'STNK Extension Report',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: "BranchCode", text: "Outlet", cls: "span6", type: "select", disable: true },
                {
                    text: "Input Date",
                    type: "controls", items: [
                        { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                        { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                    ]
                },
            ],
        },
        {
            name: 'pnlData',
            xtype: 'k-grid',
        },
    ],
    toolbars: [
        { name: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
        { name: 'export', text: 'Export (xls)', icon: 'fa fa-file-excel-o' }
    ]
});

widget.render(function () {
    var date1 = new Date();
    var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
    widget.populate({ DateFrom: date2, DateTo: date1 });
    $('#refresh').on('click', refresh);
    $('#export').on('click', exports);
    refreshGrid();

    widget.post("cs.api/Combo/isGM", function (result) {
        if (result == 1) {
            widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
            $('#BranchCode').removeAttr('disabled');
        }
        else {
            widget.post("cs.api/Combo/CurrentBranchCode", function (result) {
                var branchcode = result;
                $('#BranchCode option:selected ').val(result[0].value);
                $('#BranchCode option:selected ').text(result[0].text);
                $('#BranchCode').attr('disabled', 'disabled');
            });
        }
    });

});

function refreshGrid() {
    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['No', 'Cabang', 'Jumlah STNK', 'Input STNK by CRO', 'Persentase'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                return 'width:auto'
            },
            'class': function (d, i) {
                if (i != 0 && i != 1)
                    return 'number'
            }
        })
        .text(function (d) { return d })
    var tbody = table.append('tbody');
    var tfoot = table.append('tfoot');
    var tinfo = d3.select('#pnlData').append('div').attr({ 'class': 'table-info' })
}

function refresh(options) {
    var filter = widget.serializeObject('pnlFilter');

    widget.post('cs.api/chart/CsReportSTNKExtention', filter, function (result) {
        var tbody = d3.select('#pnlData tbody');
        tbody.selectAll('tr').data(result).enter().append('tr');
        tbody.selectAll('tr').data(result).exit().remove();
        tbody.selectAll('tr')
            .data(result)
            .html(function (d, i) {
                var html = '';
                html += '<td>' + (i + 1) + '</td>';
                html += '<td>' + d.Outlet + '</td>';
                html += '<td class="number">' + d.CustomerCount + '</td>';
                html += '<td class="number">' + d.InputByCRO + '</td>';
                html += '<td class="number">' + (d.Percentation).toFixed(2) + ' %' + '</td>';
                return html;
            });
    });
}

function exports(options) {
    var data = widget.serializeObject('pnlFilter');
    var filter = {
        BranchCode: data.BranchCode,
        DateFrom: data.DateFrom,
        DateTo: data.DateTo
    }

    widget.XlsxReport({
        url: 'cs.api/chart/CsChartStnkExtforexport',
        type: 'xlsx',
        params: filter
    });
}