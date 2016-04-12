var widget = new SimDms.Widget({
    title: 'Chart - Monitoring by Periode',
    xtype: 'panels',
    toolbars: [
        { name: 'btnRefresh', action: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
        { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
        { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
        { name: 'export', text: 'Export (xls)', icon: 'fa fa-file-excel-o' }
    ],
    panels: [
        {
            name: 'pnlFilter',
            items: [
                {
                    text: 'Inquiry',
                    type: 'controls',
                    items: [
                        { name: 'Inquiry', type: 'select', cls: 'span6', opt_text: '-- SELECT INQUIRY --' },
                    ]
                },
                {
                    text: 'Year',
                    type: 'controls',
                    items: [
                        { name: 'Year', type: 'select', cls: 'span3', opt_text: '-- SELECT YEAR --' },
                    ]
                },
                {
                    text: 'Month',
                    type: 'controls',
                    items: [
                        { name: 'Month', type: 'select', cls: 'span3', opt_text: '-- SELECT MONTH --' },
                    ]
                },
            ],
        },
        {
            name: 'pnlChart',
            xtype: 'k-grid',
        },
    ],
    //onToolbarClick: function (action) {
    //    switch (action) {
    //        case 'refresh':
    //            refreshChart();
    //            break;
    //        case 'collapse':
    //            widget.exitFullWindow();
    //            widget.showToolbars(['refresh', 'expand']);
    //            refreshChart({ height: 400 })
    //            break;
    //        case 'expand':
    //            widget.requestFullWindow();
    //            widget.showToolbars(['refresh', 'collapse']);
    //            refreshChart({ height: 500 })
    //            break;
    //        default:
    //            break;
    //    }
    //},
});

widget.render(function () {
    var date = new Date();
    var initial = { Year: moment().format('YYYY'), Month: moment().format('M'), years: [], months: [] };
    $(document).on("click", "#btnRefresh", refreshChart);
    $('#export').on('click', exports);

    initial.inquiries = [
        { value: '3DaysCall', text: '3 Days Call' },
        { value: 'Birthday', text: 'Birthday' },
        { value: 'CsCustBpkb', text: 'BPKB Reminder' },
        { value: 'CsStnkExt', text: 'STNK Extension' },
    ]

    for (var i = 0; i < 5; i++) {
        initial.years.push({ value: initial.Year - i, text: initial.Year - i });
    }

    for (var i = 1; i <= 12; i++) {
        initial.months.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
    }

    widget.select({ name: 'Inquiry', data: initial.inquiries });
    widget.select({ name: 'Year', data: initial.years });
    widget.select({ name: 'Month', data: initial.months });

    $("[name='Inquiry'], [name='Year'], [name='Month']").on('change', refreshChart);

    widget.populate(initial);

    var svg = d3.select('#pnlChart').append('svg').attr({ 'height': '10%' });
    svg.append('g').attr({ 'class': 'x axis' });
    svg.append('g').attr({ 'class': 'x axis-info' });
    svg.append('g').attr({ 'class': 'y axis' });
    svg.append('g').attr({ 'class': 'data' });
    svg.append('g').attr({ 'class': 'legend' });
});


function refreshChart(options) {
    var filter = widget.serializeObject('pnlFilter');
    var periode = filter.Year + '-' + filter.Month;
    filter.DateFrom = moment(periode, 'YYYY-M').format('YYYY-MM-DD');
    filter.DateTo = moment(periode, 'YYYY-M').add('months', 1).add('days', -1).format('YYYY-MM-DD');

    widget.post('cs.api/chart/csmonitoring', filter, function (result) {
        if (result.length == 2 && result[0].length > 0) {
            var data = result[0];
            var dlrs = result[1];
            var source = { cols: [], series: [] };
            var interval = eval((filter.DateTo).substr(8));

            for (var i = 0; i < interval; i++) {
                source.cols.push(moment(filter.DateFrom, 'YYYY-MM-DD').add('days', i).format('YYYY-MM-DD'));
            }

            dlrs.forEach(function (dlr) {
                source.series.push({
                    key: dlr.DealerCode,
                    value: dlr.DealerName,
                    data: Enumerable
                        .From(data)
                        .Where('x => x["DealerCode"] == "' + dlr.DealerCode + '"')
                        .Select('x => { Date: eval(x["InputDate"].substr(8)), DataCount: x["DataCount"] } ')
                        .ToArray()
                });
            });
            var s = $('#pnlChart');

            var m = { left: 260, top: 30, right: 30, bottom: 40 };
            var w = s.width() * 1.0;
            var h = 400;
            var colors = d3.scale.category10();

            if (options) {
                if (options.height) h = options.height;
                if (options.margin) {
                    var margin = options.margin;
                    if (margin.left) m.left = margin.left;
                    if (margin.right) m.right = margin.right;
                    if (margin.top) m.top = margin.top;
                    if (margin.bottom) m.bottom = margin.bottom;
                }
            }

            var x = d3.scale.linear().range([m.left, w - m.right]);
            var xAxis = d3.svg.axis().scale(x).orient('bottom').ticks(30);

            var y = d3.scale.linear().range([h - m.bottom, m.top]);
            var yAxis = d3.svg.axis().scale(y).orient('left');

            var colors = d3.scale.category10();

            x.domain([0, eval((filter.DateTo).substr(8))]);
            y.domain([0, 5 * Math.ceil(d3.max(data, function (d) { return d.DataCount }) / 5.0)]);

            renderSvg(m, w, h);
            renderData(m, h, source, x, y, colors);
            renderAxes(m, h, x, y, xAxis, yAxis);
            renderLegend(result, colors);

            //renderSvg();
            //renderData();
            //renderAxes();
            //renderLegend();

            //function renderSvg() {
            //    var svg = d3.select('#pnlChart > svg').attr({
            //        'class': 'debug',
            //        width: w,
            //        height: h,
            //    });

            //    svg.attr({ style: 'margin-top:0px' })
            //}

            //function renderData() {
            //    var svg = d3.select('#pnlChart > svg > .data');
            //    var line = d3.svg.line()
            //        .x(function (d) { return x(d.Date) + i * 6; })
            //        .y(function (d) { return y(d.DataCount); });

            //    svg.selectAll('path')
            //        .transition()
            //        .duration(800)
            //        .attr({ stroke: 'white' })
            //        .remove();

            //    var dealers = [];
            //    for (var i = 0; i < (source.series || []).length; i++) {
            //        dealers.push(source.series[i].key);
            //        var series = source.series[i];

            //        var barChart = svg.select('g.B' + series.key);

            //        if ($('svg g.B' + series.key).length == 0) {
            //            barChart = svg.append('g').attr({ 'class': 'B' + series.key, 'data-dealer': series.key });
            //        }

            //        var barAttr1 = {
            //            x: x(0),
            //            y: y(0),
            //            width: 5,
            //            height: 1,
            //            fill: colors(i),
            //        }

            //        var barAttr2 = {
            //            x: function (d) { return x(d.Date) + (i * 6) + x(0) - x(1) + 1 },
            //            y: function (d) { return y(d.DataCount) },
            //            height: function (d) { return h - y(d.DataCount) - m.bottom - 1 },
            //            fill: colors(i),
            //        }

            //        var barAttr3 = {
            //            x: function (d) { return x(d.Date) + (i * 6) + x(0) - x(1) + 1 },
            //            y: y(0),
            //            height: 0,
            //        }

            //        barChart.selectAll('rect')
            //            .data(series.data)
            //            .exit()
            //            .transition()
            //            .duration(800)
            //            .attr(barAttr3)
            //            .remove();

            //        barChart.selectAll('rect')
            //            .data(series.data)
            //            .transition()
            //            .duration(800)
            //            .attr(barAttr2);

            //        barChart.selectAll('rect')
            //            .data(series.data)
            //            .enter()
            //            .append('rect')
            //            .attr(barAttr1)
            //            .transition()
            //            .duration(800)
            //            .attr(barAttr2);

            //        var lineChart = svg.select('g.C' + series.key);

            //        if ($('svg g.C' + series.key).length == 0) {
            //            lineChart = svg.append('g').attr({ 'class': 'C' + series.key, 'data-dealer': series.key });
            //        }

            //        var circleAttr1 = {
            //            cx: x(0),
            //            cy: y(0),
            //            r: 0,
            //            stroke: colors(i),
            //            'stroke-width': '2',
            //            fill: 'yellow'
            //        }

            //        var circleAttr2 = {
            //            cx: function (d) { return x(d.Date) + (i * 6) + 3.5 + x(0) - x(1) },
            //            cy: function (d) { return y(d.DataCount) + 1 },
            //            r: 3,
            //            stroke: colors(i),
            //            'stroke-width': '1',
            //            fill: 'yellow'
            //        }

            //        var circleAttr3 = {
            //            cx: function (d) { return x(d.Date) + (i * 6) + 3.5 + x(0) - x(1) },
            //            cy: y(0),
            //            'stroke-opacity': 0.25,
            //            'fill-opacity': 0.25,
            //        }

            //        lineChart.selectAll('circle')
            //            .data(series.data)
            //            .exit()
            //            .transition()
            //            .duration(800)
            //            .attr(circleAttr3)
            //            .remove();

            //        lineChart.selectAll('circle')
            //            .data(series.data)
            //            .transition()
            //            .duration(800)
            //            .attr(circleAttr2);

            //        lineChart.selectAll('circle')
            //            .data(series.data)
            //            .enter()
            //            .append('circle')
            //            .attr(circleAttr1)
            //            .transition()
            //            .duration(800)
            //            .attr(circleAttr2);
            //    }

            //    var old_dealers = [];
            //    var old_series = $('svg .data > g');
            //    for (var i = 0; i < old_series.length; i++) {
            //        var dealer = $(old_series[i]).data('dealer').toString();
            //        if (dealers.indexOf(dealer) < 0) {
            //            old_dealers.push(dealer);
            //        }
            //    }

            //    (old_dealers).forEach(function (key) {
            //        svg.select('g.C' + key)
            //            .selectAll('circle')
            //            .remove();

            //        svg.select('g.B' + key)
            //            .selectAll('rect')
            //            .remove();
            //    });
            //}

            //function renderAxes() {
            //    var svg = d3.select('#pnlChart > svg');
            //    svg.select('.x.axis')
            //        .attr({ 'transform': 'translate(0, ' + (h - m.bottom) + ')' })
            //        .transition()
            //        .duration(800)
            //        .call(xAxis);

            //    svg.select('.x.axis-info')
            //        .attr({ 'transform': 'translate(' + -(x(1) - x(0)) / 2 + ', ' + (h - m.bottom) + ')' })
            //        .transition()
            //        .duration(800)
            //        .call(xAxis);

            //    d3.select(svg.select('.x.axis-info').select('.tick')[0][0]).attr('visibility', 'hidden');

            //    svg.select('.y.axis')
            //        .attr({ 'transform': 'translate(' + m.left + ', 0)' })
            //        .transition()
            //        .duration(800)
            //        .call(yAxis);
            //}

            //function renderLegend() {
            //    var dealers = result[1];
            //    var svg = d3.select('#pnlChart > svg > .legend');

            //    svg.selectAll('rect').remove();
            //    svg.selectAll('rect')
            //        .data(dealers)
            //        .enter()
            //        .append('rect')
            //        .attr({
            //            x: 15,
            //            y: function (d, i) { return i * 18 + 26 },
            //            width: 10,
            //            height: 10,
            //            'font-size': 12,
            //            fill: 'white'
            //        })
            //        .transition()
            //        .delay(function (d, i) { return i * 200 })
            //        .duration(800)
            //        .attr({
            //            'fill': function (d, i) { return colors(i) },
            //        })

            //    svg.selectAll('text').remove();
            //    svg.selectAll('text')
            //        .html('')
            //        .data(dealers)
            //        .enter()
            //        .append('text')
            //        .attr({
            //            x: -200,
            //            y: function (d, i) { return i * 18 + 36 },
            //            'font-size': 12,
            //            fill: function (d, i) { return colors(i) }
            //        })
            //        .text(function (d) {
            //            return d.DealerName;
            //        })
            //        .transition()
            //        .delay(function (d, i) { return i * 200 })
            //        .duration(600)
            //        .attr({
            //            x: 32,
            //            'font-size': 12,
            //        })
            //}
        }
        else {
            var line = d3.svg.line()
                 .x(function (d, i) { return x(i + 1); })
                 .y(function (d) { return y(d); });

            var old_series = $('svg .data > g');
            for (var i = 0; i < old_series.length; i++) {
                var key = $(old_series[i]).data('dealer').toString();

                d3.select('svg > .data').select('g.C' + key)
                    .selectAll('circle')
                    .remove();

                d3.select('svg > .data').select('g.B' + key)
                    .selectAll('rect')
                    .remove();

                d3.select('svg > .data')
                    .selectAll('path')
                    .remove();
            }
        }
    });
}

function renderSvg(m, w, h) {
    var svg = d3.select('#pnlChart > svg').attr({
        'class': 'debug',
        width: w,
        height: h,
    });

    svg.attr({ style: 'margin-top:0px' })
}

function renderData(m, h, source, x, y, colors) {
    var svg = d3.select('#pnlChart > svg > .data');
    var line = d3.svg.line()
        .x(function (d) { return x(d.Date) + i * 6; })
        .y(function (d) { return y(d.DataCount); });

    svg.selectAll('path')
        .transition()
        .duration(800)
        .attr({ stroke: 'white' })
        .remove();

    var dealers = [];
    for (var i = 0; i < (source.series || []).length; i++) {
        dealers.push(source.series[i].key);
        var series = source.series[i];

        var barChart = svg.select('g.B' + series.key);

        if ($('svg g.B' + series.key).length == 0) {
            barChart = svg.append('g').attr({ 'class': 'B' + series.key, 'data-dealer': series.key });
        }

        var barAttr1 = {
            x: x(0),
            y: y(0),
            width: 5,
            height: 1,
            fill: colors(i),
        }

        var barAttr2 = {
            x: function (d) { return x(d.Date) + (i * 6) + x(0) - x(1) + 1 },
            y: function (d) { return y(d.DataCount) },
            height: function (d) { return h - y(d.DataCount) - m.bottom - 1 },
            fill: colors(i),
        }

        var barAttr3 = {
            x: function (d) { return x(d.Date) + (i * 6) + x(0) - x(1) + 1 },
            y: y(0),
            height: 0,
        }

        barChart.selectAll('rect')
            .data(series.data)
            .exit()
            .transition()
            .duration(800)
            .attr(barAttr3)
            .remove();

        barChart.selectAll('rect')
            .data(series.data)
            .transition()
            .duration(800)
            .attr(barAttr2);

        barChart.selectAll('rect')
            .data(series.data)
            .enter()
            .append('rect')
            .attr(barAttr1)
            .transition()
            .duration(800)
            .attr(barAttr2);

        var lineChart = svg.select('g.C' + series.key);

        if ($('svg g.C' + series.key).length == 0) {
            lineChart = svg.append('g').attr({ 'class': 'C' + series.key, 'data-dealer': series.key });
        }

        var circleAttr1 = {
            cx: x(0),
            cy: y(0),
            r: 0,
            stroke: colors(i),
            'stroke-width': '2',
            fill: 'yellow'
        }

        var circleAttr2 = {
            cx: function (d) { return x(d.Date) + (i * 6) + 3.5 + x(0) - x(1) },
            cy: function (d) { return y(d.DataCount) + 1 },
            r: 3,
            stroke: colors(i),
            'stroke-width': '1',
            fill: 'yellow'
        }

        var circleAttr3 = {
            cx: function (d) { return x(d.Date) + (i * 6) + 3.5 + x(0) - x(1) },
            cy: y(0),
            'stroke-opacity': 0.25,
            'fill-opacity': 0.25,
        }

        lineChart.selectAll('circle')
            .data(series.data)
            .exit()
            .transition()
            .duration(800)
            .attr(circleAttr3)
            .remove();

        lineChart.selectAll('circle')
            .data(series.data)
            .transition()
            .duration(800)
            .attr(circleAttr2);

        lineChart.selectAll('circle')
            .data(series.data)
            .enter()
            .append('circle')
            .attr(circleAttr1)
            .transition()
            .duration(800)
            .attr(circleAttr2);
    }

    var old_dealers = [];
    var old_series = $('svg .data > g');
    for (var i = 0; i < old_series.length; i++) {
        var dealer = $(old_series[i]).data('dealer').toString();
        if (dealers.indexOf(dealer) < 0) {
            old_dealers.push(dealer);
        }
    }

    (old_dealers).forEach(function (key) {
        svg.select('g.C' + key)
            .selectAll('circle')
            .remove();

        svg.select('g.B' + key)
            .selectAll('rect')
            .remove();
    });
}

function renderAxes(m, h, x, y, xAxis, yAxis) {
    var svg = d3.select('#pnlChart > svg');
    svg.select('.x.axis')
        .attr({ 'transform': 'translate(0, ' + (h - m.bottom) + ')' })
        .transition()
        .duration(800)
        .call(xAxis);

    svg.select('.x.axis-info')
        .attr({ 'transform': 'translate(' + -(x(1) - x(0)) / 2 + ', ' + (h - m.bottom) + ')' })
        .transition()
        .duration(800)
        .call(xAxis);

    d3.select(svg.select('.x.axis-info').select('.tick')[0][0]).attr('visibility', 'hidden');

    svg.select('.y.axis')
        .attr({ 'transform': 'translate(' + m.left + ', 0)' })
        .transition()
        .duration(800)
        .call(yAxis);
}

function renderLegend(result, colors) {
    var dealers = result[1];
    var svg = d3.select('#pnlChart > svg > .legend');

    svg.selectAll('rect').remove();
    svg.selectAll('rect')
        .data(dealers)
        .enter()
        .append('rect')
        .attr({
            x: 15,
            y: function (d, i) { return i * 18 + 26 },
            width: 10,
            height: 10,
            'font-size': 12,
            fill: 'white'
        })
        .transition()
        .delay(function (d, i) { return i * 200 })
        .duration(800)
        .attr({
            'fill': function (d, i) { return colors(i) },
        })

    svg.selectAll('text').remove();
    svg.selectAll('text')
        .html('')
        .data(dealers)
        .enter()
        .append('text')
        .attr({
            x: -200,
            y: function (d, i) { return i * 18 + 36 },
            'font-size': 12,
            fill: function (d, i) { return colors(i) }
        })
        .text(function (d) {
            return d.DealerName;
        })
        .transition()
        .delay(function (d, i) { return i * 200 })
        .duration(600)
        .attr({
            x: 32,
            'font-size': 12,
        })
}

function exports(options) {
    var data = widget.serializeObject('pnlFilter');
    var inquirytext = $('#Inquiry option:selected ').text();
    var periode = data.Year + '-' + data.Month;
    var DateFrom = moment(periode, 'YYYY-M').format('YYYY-MM-DD');
    var DateTo = moment(periode, 'YYYY-M').add('months', 1).add('days', -1).format('YYYY-MM-DD');
    var filter = {
        Inquiry: data.Inquiry,
        InquiryText:inquirytext,
        DateFrom: DateFrom,
        DateTo: DateTo
    }

    widget.XlsxReport({
        url: 'cs.api/chart/CsMonitoringbyperiodeexport',
        type: 'xlsx',
        params: filter
    });
}