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
        { name: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
        { name: 'export', text: 'Export (xls)', icon: 'fa fa-file-excel-o' }
    ]
});

widget.render(function () {
    var date = new Date(moment().format('YYYY-MM-') + '01');
    var initial = { DateFrom: date, DateTo: new Date() }

    initial.inquiries = [
        { value: '3DaysCall', text: '3 Days Call' },
        { value: 'Birthday', text: 'Birthday' },
        { value: 'CsCustBpkb', text: 'BPKB Reminder' },
        { value: 'CsStnkExt', text: 'STNK Extension' },
    ]

    widget.select({ name: "Inquiry", data: initial.inquiries });

    $('#Inquiry').on('change', refresh);
    $('#refresh').on('click', refresh);
    $('#export').on('click', exports);

    widget.populate(initial);
});

function refresh() {
    var filter = widget.serializeObject('pnlFilter');
    var interval = moment(filter.DateTo).diff(moment(filter.DateFrom), 'days');
    filter.interval = interval;

    widget.post('cs.api/chart/CsMonitoring1', filter, function (result) {
            generateChartBar(result);
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


function exports(options) {
    var data = widget.serializeObject('pnlFilter');
    var inquirytext = $('#Inquiry option:selected ').text();
    var filter = {
        Inquiry: data.Inquiry,
        InquiryText: inquirytext,
        DateFrom: data.DateFrom,
        DateTo: data.DateTo
    }

    widget.XlsxReport({
        url: 'cs.api/chart/CsMonitoring1export',
        type: 'xlsx',
        params: filter
    });
}