var widget = new SimDms.Widget({
    title: 'Chart - Monitoring by Date',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: 'Inquiry', text: 'Inquiry', type: 'select', cls: 'span6' },
                { name: 'DateFrom', text: 'Date From', type: 'datepicker', cls: 'span3' },
                { name: 'DateTo', text: 'Date To', type: 'datepicker', cls: 'span3' },
            ],
        },
        {
            name: 'pnlChart',
            xtype: 'k-grid',
        },
    ],
    toolbars: [
        { action: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
        { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
        { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
        { action: 'print', text: 'Print', icon: 'fa fa-print' },
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'refresh':
                refresh();
                break;
                break;
            case 'collapse':
                widget.exitFullWindow();
                widget.showToolbars(['refresh', 'expand']);
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['refresh', 'collapse']);
                break;
            case 'print':
                ExportXls();
                break;
            default:
                break;
        }
    },
});

widget.render(function () {
    var date = d3.time.format('%Y-%m-%d');
    var initial = { DateFrom: date.parse(date(new Date).substring(0, 8) + '01'), DateTo: new Date() }

    initial.inquiries = [
        { value: '3DaysCall', text: '3 Days Call' },
        { value: 'Birthday', text: 'Birthday' },
        { value: 'CsCustBpkb', text: 'BPKB Reminder' },
        { value: 'CsStnkExt', text: 'STNK Extension' },
    ]

    widget.bind({ name: 'Inquiry', text: '-- SELECT INQUIRY --', data: initial.inquiries, onChange: refresh });
    widget.populate(initial);
});

function refresh() {
    var filter = widget.serializeObject('pnlFilter');
    var interval = moment(filter.DateTo).diff(moment(filter.DateFrom), 'days');

    widget.post('wh.api/chart/csmonitoring', filter, function (result) {
        if (result.length == 2 && result[0].length > 0) {
            var data = result[0];
            var dlrs = result[1];
            var source = { cols: [], series: [], data: [] };

            for (var i = 0; i <= interval; i++) {
                source.cols.push(moment(filter.DateFrom, 'YYYY-MM-DD').add('days', i).format('YYYY-MM-DD'));
            }

            dlrs.forEach(function (dlr) {
                var series = { key: dlr.DealerCode, value: dlr.DealerName, data: [] };
                for (var i = 0; i <= interval; i++) {
                    var date = moment(filter.DateFrom, 'YYYY-MM-DD').add('days', i).format('YYYY-MM-DD');
                    var row = Enumerable
                        .From(data)
                        .Where('x => x["DealerCode"] == "' + series.key + '" && x["InputDate"] == "' + date + '"')
                        .FirstOrDefault();
                    series.data.push((row == undefined) ? 0 : row.DataCount);

                    if (row) source.data.push(row.DataCount);
                }

                source.series.push(series);
            });

            generateChartBar(source);
        }
    });
}

function generateChartBar(source) {
    var s = $('#pnlChart');

    var m = { left: 50, top: (source.series.length * 18 + 50), right: 30, bottom: 40 };
    var w = s.width() * 0.8;
    var h = source.cols.length * 50;
    var colors = d3.scale.category10();

    var svg = d3.select('#pnlChart').html('').append('svg').attr({
        //'class': 'debug',
        width: w,
        height: h,
    });

    var cols = [];
    for (var i = 0; i < source.cols.length; i++) {
        cols.push(moment(source.cols[i], 'YYYY-MM-DD').format('MM/DD'));
    }

    var x = d3.scale.linear().range([m.left, w - m.right], 0.1);
    var xAxis = d3.svg.axis().scale(x).orient('top');

    var y = d3.scale.ordinal().rangeRoundBands([h - m.bottom, m.top]);
    var yAxis = d3.svg.axis().scale(y).orient('left');

    var max = 10 * Math.ceil(d3.max(source.data) / 10);
    x.domain([0, max]);
    y.domain(cols);

    appendData();
    appendAxes();
    appendLegend();

    function appendAxes() {
        svg.append('g').attr({
            'class': 'x axis',
            'transform': 'translate(0, ' + (m.top) + ')'
        }).call(xAxis);
        svg.append('g').attr({
            'class': 'y axis',
            'transform': 'translate(' + m.left + ', 0)'
        }).call(yAxis);
    }

    function appendData() {
        for (var i = 0; i < (source.series || []).length; i++) {
            var series = source.series[i];

            var lineChart = svg.append('g').attr({ 'class': series.name });

            lineChart.selectAll('rect')
                .data(series.data)
                .enter()
                .append('rect')
                .attr({
                    y: function (d, idx) { return y(cols[idx]) + (y.rangeBand() / 10) + (0.75 * (y.rangeBand() / source.series.length) * i); },
                    x: x(0),
                    width: function (d) {
                        return x(d) - m.left;
                    },
                    height: 0.7 * (y.rangeBand() / source.series.length),
                    fill: colors(i)
                });
        }
    }

    function appendLegend() {
        var legend = svg.append('g').attr({ 'class': 'legend' });
        legend.selectAll('text')
            .data(source.series)
            .enter()
            .append('text')
            .attr({
                x: 66,
                y: function (d, i) { return i * 18 + 30 },
                fill: function (d, i) { return colors(i) }
            })
            .text(function (x) {
                return x.value;
            });

        legend.selectAll('rect')
            .data(source.series)
            .enter()
            .append('rect')
            .attr({
                x: 50,
                y: function (d, i) { return i * 18 + 18 },
                width: 12,
                height: 12,
                fill: function (d, i) { return colors(i) }
            })
            .text(function (x) {
                return x.value;
            });
    }
}

function generateChart(source) {
    var s = $('#pnlChart');

    var m = { left: 40, top: 20, right: 30, bottom: 40 };
    var w = s.width();
    var h = 400;

    var svg = d3.select('#pnlChart').html('').append('svg').attr({
        'class': 'debug',
        width: w,
        height: h,
    });

    var cols = [];
    for (var i = 0; i < source.cols.length; i++) {
        cols.push(moment(source.cols[i], 'YYYY-MM-DD').format('MM/DD'));
    }

    var x = d3.scale.ordinal().rangeRoundBands([m.left, w - m.right], 0.1);
    var xAxis = d3.svg.axis().scale(x).orient('bottom');

    var y = d3.scale.linear().range([h - m.bottom, m.top]);
    var yAxis = d3.svg.axis().scale(y).orient('left');

    x.domain(cols);
    y.domain([0, 10 * Math.ceil(d3.max(source.data) / 10)]);

    var line = d3.svg.line()
        .x(function (d, i) { return x(cols[i]) + x.rangeBand() / 2; })
        .y(function (d) { return y(d); });

    var colors = d3.scale.category10();

    for (var i = 0; i < (source.series || []).length; i++) {
        var series = source.series[i];

        var lineChart = svg.append('g').attr({ 'class': line.name });
        lineChart.append('svg:path').attr({
            d: line(series.data),
            fill: 'none',
            stroke: colors(i),
            'stroke-width': '2',
        });

        lineChart.selectAll('circle')
            .data(series.data)
            .enter()
            .append('circle')
            .attr({
                cx: function (d, i) { return x(cols[i]) + x.rangeBand() / 2; },
                cy: function (d) { return y(d) },
                r: 3,
                stroke: colors(i),
                'stroke-width': '2',
                fill: 'yellow'
            });
    }

    svg.append('g').attr({
        'class': 'x axis',
        'transform': 'translate(0, ' + (h - m.bottom) + ')'
    }).call(xAxis);
    svg.append('g').attr({
        'class': 'y axis',
        'transform': 'translate(' + m.left + ', 0)'
    }).call(yAxis);
}

function ExportXls() {
    var data = widget.serializeObject('pnlFilter');
    var params = {
        ReportId: "ChartMonitoringByDate.trdx",
        Parameters: "{Inquiry:'" + data.Inquiry + "', DateFrom:'" + data.DateFrom + "', DateTo:'" + data.DateTo + "', Dealer:'"+null+"'}"
    }

    SimDms.openReport(params, "Chart Monitoring By Date")
}