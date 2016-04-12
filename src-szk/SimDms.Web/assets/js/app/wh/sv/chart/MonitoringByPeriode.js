var lastShownData = null;

var widget = new SimDms.Widget({
    title: 'Unit Intake by Periode',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: 'Year', text: 'Year', type: 'select', cls: 'span3' },
                { name: 'Month', text: 'Month', type: 'select', cls: 'span3' },
                { name: 'Area', text: 'Area', type: 'select', cls: 'span6', opt_text: '-- SELECT ALL --' },
                { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span6', opt_text: '-- SELECT ALL --' },
                { name: 'Outlet', text: 'Outlet', type: 'select', cls: 'span6', opt_text: '-- SELECT ALL --' },
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
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'refresh':
                refreshChart();
                break;
            case 'collapse':
                widget.exitFullWindow();
                widget.showToolbars(['refresh', 'expand']);
                refreshChart({ height: 400 })
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['refresh', 'collapse']);
                refreshChart({ height: 500 })
                break;
            default:
                break;
        }
    },
});

widget.render(function () {
    initElementEvents();

    var svg = d3.select('#pnlChart').append('svg').attr({ 'height': '10%' });
    svg.append('g').attr({ 'class': 'x axis' });
    svg.append('g').attr({ 'class': 'x axis-info' });
    svg.append('g').attr({ 'class': 'y axis' });
    svg.append('g').attr({ 'class': 'data' });
    svg.append('g').attr({ 'class': 'legend' });
    svg.append('g').attr({ 'class': 'scatter-plot' });
    svg.append('g').attr({ 'class': 'scatter-plot-label' });

    var date = new Date();
    var initial = { Year: moment().format('YYYY'), Month: moment().format('M'), years: [], months: [] };

    for (var i = 0; i < 5; i++) {
        initial.years.push({ value: initial.Year - i, text: initial.Year - i });
    }

    for (var i = 1; i <= 12; i++) {
        initial.months.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
    }

    widget.bind({ name: 'Year', text: '-- SELECT YEAR --', data: initial.years });
    widget.bind({ name: 'Month', text: '-- SELECT MONTH --', data: initial.months });

    var currentMonth = (new Date()).getMonth() + 1;
    var currentYear = (new Date()).getFullYear();

    $('[name="Year"]').val(currentYear);
    $('[name="Month"]').val(currentMonth);

    widget.populate(initial);
});

function initElementEvents() {
    widget.post("wh.api/combo/UnitIntakeFilter", {}, function (result) {
        widget.bind({
            name: 'Area',
            text: '-- SELECT ALL --',
            data: result[0],
            defaultAll: true,
        });
    });
    $("[name=Area]").on("change", function () {
        widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=Area]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=Dealer]").prop("selectedIndex", 0);
        $("[name=Dealer]").change();
    });

    $("[name=Dealer]").on("change", function () {
        widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/branchs", params: { comp: $("[name=Dealer]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=Outlet]").prop("selectedIndex", 0);
        $("[name=Outlet]").change();
    });
}

function refreshChart(options) {
    var filter = widget.serializeObject('pnlFilter');
    var periode = filter.Year + '-' + filter.Month;
    filter.DateFrom = moment(periode, 'YYYY-M').format('YYYY-MM-DD');
    filter.DateTo = moment(periode, 'YYYY-M').add('months', 1).add('days', -1).format('YYYY-MM-DD');

    widget.post('wh.api/chart/SvMonitoringByPeriode', filter, function (result) {
        var s = $('#pnlChart');
        var m = { left: 50, top: 30, right: 30, bottom: 40 };
        var w = s.width() * 1.0;
        var h = 300;
        var colors = d3.scale.category10();
        var interval = $('[name="Month"]').val();
        
        if (result[0].length > 0) {
            var data = result[0];
            lastShownData = data;

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

            var daysCount = eval((filter.DateTo).substr(8));

            x.domain([0, daysCount]);
            y.domain([0, 5 * Math.ceil(d3.max(data, function (d) { return d.DataCount }) / 5.0)]);

            renderSvg();
            renderAxes();
            renderData();
            renderScaterPlot();
            renderDataLabel();

            function renderSvg() {
                var svg = d3.select('#pnlChart > svg').attr({
                    'class': 'debug',
                    width: w,
                    height: h,
                });

                svg.attr({ style: 'margin-top:0px' })
            }

            function renderAxes() {
                var svg = d3.select('#pnlChart > svg');
                svg.select('.x.axis')
                    .attr({ 'transform': 'translate(0, ' + (h - m.bottom) + ')' })
                    .transition()
                    .duration(800)
                    .call(xAxis);

                svg.select('.x.axis-info')
                    .attr({ 'transform': 'translate(' + -(x(1) - x(0)) / 256 + ', ' + (h - m.bottom) + ')' })
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

            function renderData() {
                var svg = d3.select('#pnlChart > svg > .data');
                var max = d3.max(data, function (d) { return d.DataCount });

                var lineFunc = d3.svg.line()
                    .x(function (d, i) {
                        return m.left + (w - m.right - m.left) / daysCount * d.Day;
                    })
                    .y(function (d, i) {
                        return h - m.bottom - ((h - m.bottom - m.top) * d.DataCount / max);
                    })
                    .interpolate("linear");

                var lineTransitionFunc = d3.svg.line()
                    .x(function (d, i) {
                        return m.left + (w - m.right - m.left) / daysCount * d.Day;
                    })
                    .y(function (d, i) {
                        return h - m.bottom;
                    })
                    .interpolate("linear");

                svg.selectAll('path')
                    .transition()
                    .duration(800)
                    .attr({
                        d: lineTransitionFunc(data)
                    })
                    .remove();

                //svg.selectAll('path')
                //   .data(data)
                //   .exit()
                //   .data(data)
                //   .enter()
                //   .append('path')
                //   .transition()
                //   .duration(800)
                //   .attr({
                //       d: lineTransitionFunc(data),
                //       'stroke': 'purple',
                //       'stroke-width': 2,
                //       'fill': 'none'
                //   })
                //   .transition()
                //   .duration(800)
                //   .attr({
                //       d: lineFunc(data),
                //       'stroke': 'purple',
                //       'stroke-width': 2,
                //       'fill': 'none'
                //   });

                svg.selectAll('path')
                   .data(data)
                   .exit()
                   .data(data)
                   .enter()
                   .append('path')
                   .transition()
                   .duration(800)
                   .attr({
                       d: lineTransitionFunc(data),
                       'stroke': 'purple',
                       'stroke-width': 2,
                       'fill': 'none'
                   })
                   .transition()
                   .duration(800)
                   .attr({
                       d: lineFunc(data),
                       'stroke': 'purple',
                       'stroke-width': 2,
                       'fill': 'none'
                   });
            }

            function renderScaterPlot() {
                var svg = d3.select('#pnlChart > svg > .scatter-plot');
                var max = d3.max(data, function (d) { return d.DataCount });

                svg.selectAll('circle')
                    .transition()
                    .duration(800)
                    .attr({
                        cy: function (d) {
                            return h - m.bottom;
                        },
                        stroke: 'white'
                    })
                    .remove();

                svg.selectAll("circle").data(data)
                   .exit()
                   .data(data)
                   .enter()
                   .append('circle')
                   .transition()
                   .duration(800)
                   .attr({
                       cy: function (d) {
                           return h - m.bottom;
                       },
                       r: 0,
                       fill: 'white',
                       stroke: 'white'
                   })
                   .transition()
                   .duration(800)
                   .attr({
                       cx: function (d) {
                           return m.left + (w - m.right - m.left) / daysCount * d.Day;
                       },
                       cy: function (d) {
                           return h - m.bottom - ((h - m.bottom - m.top) * d.DataCount / max);
                       },
                       r: 5,
                       fill: 'yellow',
                       stroke: 'blue'
                   });
            }

            function renderDataLabel() {
                var svg = d3.select('#pnlChart > svg > .scatter-plot-label');
                var max = d3.max(data, function (d) { return d.DataCount });

                svg.selectAll('text')
                    .transition()
                    .duration(800)
                    .attr({
                        y: function (d) {
                            return (h - m.bottom) - 10;
                        },
                        'color': '#ffffff',
                        'font-size': '0px',
                    })
                    .remove();

                svg.selectAll("text").data(data)
                   .exit()
                   .data(data)
                   .enter()
                   .append('text')
                   .text(function (d) {
                       return d.DataCount;
                   })
                   .transition()
                   .duration(800)
                   .attr({
                       y: function (d) {
                           return (h - m.bottom) - 10;
                       },
                       'color': '#ffffff',
                       'font-size': '0px',
                   })
                   .transition()
                   .duration(800)
                   .attr({
                       x: function (d) {
                           return (m.left + (w - m.right - m.left) / daysCount * d.Day) - 5;
                       },
                       y: function (d) {
                           return (h - m.bottom - ((h - m.bottom - m.top) * d.DataCount / max)) - 10;
                       },
                       'color': 'black',
                       'font-size': '10px',
                   });
            }
        }
        else {
            var line = d3.svg.line()
                 .x(function (d, i) { return x(i + 1); })
                 .y(function (d) { return y(d); });

            var old_series = $('svg .data > g');

            var lineTransitionFunc = d3.svg.line()
                    .x(function (d, i) {
                        return m.left + (w - m.right - m.left) / daysCount * d.Day;
                    })
                    .y(function (d, i) {
                        return h - m.bottom;
                    })
                    .interpolate("linear");

            d3.select('#pnlChart > svg > .data').selectAll('path')
                   .transition()
                   .duration(800)
                   .attr({
                       d: lineTransitionFunc(lastShownData)
                   })
                   .remove();

            d3.select('#pnlChart > svg > .scatter-plot').selectAll('circle')
                    .transition()
                    .duration(800)
                    .attr({
                        cy: function (d) {
                            return h - m.bottom;
                        },
                        stroke: 'white'
                    })
                    .remove();

            d3.select('#pnlChart > svg > .scatter-plot-label').selectAll('text')
                    .transition()
                    .duration(800)
                    .attr({
                        y: function (d) {
                            return (h - m.bottom) - 10;
                        },
                        'color': '#ffffff',
                        'font-size': '0px',
                    })
                    .remove();
        }
    });
}
