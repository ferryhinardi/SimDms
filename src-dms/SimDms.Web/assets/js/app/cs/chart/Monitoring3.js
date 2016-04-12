var widget = new SimDms.Widget({
    title: 'Data - Monitoring',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: 'DateInit', text: 'Initial Date', type: 'datepicker', cls: 'span3' },
                { name: 'DateReff', text: 'Refference Date', type: 'datepicker', cls: 'span3' },
                { name: 'Interval', text: 'Interval Date', type: 'select', cls: 'span6' },
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
    var date = d3.time.format('%Y-%m-%d');
    var initial = {
        DateInit: date.parse(date(new Date()).substring(0, 5) + '01-01'),
        DateReff: date.parse(date(new Date())),
        intervals: []
    }

    for (var i = 1; i <= 7; i++) {
        initial.intervals.push({ value: i, text: i + ((i == 1) ? ' Day' : ' Days') });
    }
    
    widget.select({ name: "Interval", data: initial.intervals });

    $('#Interval').on('change', refresh);
    $('#refresh').on('click', refresh);
    $('#export').on('click', exports);

    widget.populate(initial);

    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['DATE', 'OUTLET', '3 DAYS CALL', 'CUST B`DAY', 'CUST BPKB', 'STNK EXT'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                if (i == 0) return 'width:15%'
                else if (i == 1) return 'width:auto'
                else return 'width:15%'
            },
            'class': function (d, i) {
                if (i == 0) return 'date'
                else if (i == 1) return '-'
                else return 'number'
            }
        })
        .text(function (d) { return d })
    var tbody = table.append('tbody');
});

function refresh(options) {
    var filter = widget.serializeObject('pnlFilter');
    widget.post('cs.api/chart/csdatamonitoring', filter, function (result) {
        var interval = filter.Interval;
        var dealers = result[0];
        var length = dealers.length;

        d3.select('#pnlData tbody').selectAll('tr').remove();
        var rows = d3.select('#pnlData tbody')
            .selectAll('tr')
            .data(d3.range(interval * length))
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
                var date = moment(filter.DateReff).add('days', -Math.floor(i / length) + 1)
                var dateInfo = moment(filter.DateReff).add('days', -Math.floor(i / length))

                var row = Enumerable
                    .From(result[1])
                    .Where('x => x["DealerCode"] == "' + dealer.DealerCode + '" && x["DateInput"] == "' + date.format('YYYY-MM-DD') + '"')
                    .FirstOrDefault()

                if (row) {
                    //dealer.C3DaysCall += row.C3DaysCall;
                    //dealer.CsBirthday += row.CsBirthday;
                    //dealer.CsCustBpkb += row.CsCustBpkb;
                    //dealer.CsStnkExt += row.CsStnkExt;

                    dealer.C3DaysCall = dealer.C3DaysCall - row.C3DaysCall;
                    dealer.CsBirthday = dealer.CsBirthday - row.CsBirthday;
                    dealer.CsCustBpkb = dealer.CsCustBpkb - row.CsCustBpkb;
                    dealer.CsStnkExt = dealer.CsStnkExt - row.CsStnkExt;
                }

                if ((i % length) == 0) {
                    html += '<th class="date" rowspan=' + length + '>' + dateInfo.format('DD-MMM-YYYY') + '</th>'
                }
                html += '<th>' + dealer.DealerName + '</th>'
                html += '<td class="number">' + widget.numberFormat(dealer.C3DaysCall) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.CsBirthday) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.CsCustBpkb) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.CsStnkExt) + '</td>'
                return html;
            })
    });
}

function exports(options) {
    var data = widget.serializeObject('pnlFilter');
    var filter = {
        DateInit: data.DateInit,
        DateReff: data.DateReff,
        Interval: data.Interval
    }

    widget.XlsxReport({
        url: 'cs.api/chart/csdatamonitoringexport',
        type: 'xlsx',
        params: filter
    });
}