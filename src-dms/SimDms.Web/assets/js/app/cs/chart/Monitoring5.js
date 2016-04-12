var widget = new SimDms.Widget({
    title: 'Report - 3 Day Call',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: "BranchCode", text: "Outlet", cls: "span6", type: "select", disable: true },
                {
                    text: 'Delivery Date', type: 'controls', cls: 'span6', items: [
                    { name: 'DateFrom', type: 'datepicker', cls: 'span3' },
                    { name: 'DateTo', type: 'datepicker', cls: 'span3' }
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
    var date = new Date(moment().format('YYYY-MM-') + '01');
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


    var initial = { DateFrom: date, DateTo: new Date() };

    $('#refresh').on('click', refresh);
    $('#export').on('click', exports);

    widget.populate(initial);

    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['Outlet', 'Jumlah Delivery', 'Input 3 Days by CRO', 'PERSENTASE'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                if (i == 0) return 'width:15%'
                else return 'width:15%'
            },
            'class': function (d, i) {
            }
        })
        .text(function (d) { return d })
    var tbody = table.append('tbody');
    var tfoot = table.append('tfoot');
});

function refresh(options) {
    var data = widget.serializeObject('pnlFilter');
    var filter = {
        BranchCode: data.BranchCode,
        DateFrom: data.DateFrom,
        DateTo: data.DateTo
    }
    //console.log(filter);
    widget.post('cs.api/chart/CsReportTDayCall', filter, function (result) {
        var dealers = result;
        var dealers = result;
        var length = dealers.length;
        var tdata = { CustomerCount: 0, InputByCRO: 0 };

        d3.select('#pnlData tbody').selectAll('tr').remove();
        var rows = d3.select('#pnlData tbody')
            .selectAll('tr')
            .data(d3.range(length))
            .enter()
            .append('tr')
            .attr({
                'class': function (d, i) { return ((Math.floor(i / length)) % 2 == 0) ? 'even' : 'odd' }
            })
            .html(function (d, i) {
                var html = '';
                var intdlr = i % length;
                var dealer = dealers[intdlr];
                //var date = moment(filter.DateReff).add('days', Math.floor(i / length) - filter.Interval + 1)

                html += '<td>' + dealer.OutletAbbreviation + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.CustomerCount) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.InputByCRO) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.Percentation) + '%</td>'

                tdata.CustomerCount += dealer.CustomerCount;
                tdata.InputByCRO += dealer.InputByCRO;

                return html;
            })

        var tfoot = d3.select('#pnlData tfoot');
        tfoot.html(function () {
            var total = 0;
            if (!isNaN(tdata.InputByCRO / tdata.CustomerCount)) {
                total = (tdata.InputByCRO / tdata.CustomerCount) * 100;
            }
            var html = '';
            html += '<tr>';
            html += '<td>TOTAL </td>';
            html += '<td class="number">' + tdata.CustomerCount + '</td>';
            html += '<td class="number">' + tdata.InputByCRO + '</td>';
            html += '<td class="number">' + widget.numberFormat(total) + '%</td>';
            html += '</tr>';
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
        url: 'cs.api/chart/CsReportTDayCallExcell',
        type: 'xlsx',
        params: filter
    });
}