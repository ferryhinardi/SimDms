SimDms.Widget.prototype.chart = function (selection) {
    window.selection = selection;
    selection.each(function (source) {
        if (source.type == 'bar') barChart(source);
        if (source.type == 'line') lineChart(source);
    });

    function barChart(source) {
        selection.selectAll('svg').data(d3.range(1)).enter().append('svg');
        selection.selectAll('svg').attr({
            'height': function () {
                var height = '400';
                if (source.attr && source.attr.height) height = source.attr.height;
                return height;
            },
            'class': 'debug',
            'style': 'margin-top: -5px'
        });

        var w = selection.style('width').replace('px', '');
        var h = selection.style('height').replace('px', '');
        var m = { top: 30, right: 30, bottom: 30, left: 50 };
        if (source.attr && source.attr.margin) {
            var margin = source.attr.margin;
            m.top = margin.top || m.top;
            m.right = margin.right || m.right;
            m.bottom = margin.bottom || m.bottom;
            m.left = margin.left || m.left;
        }

        m.bottom = m.bottom + (15 * source.series.length) + 10;

        var x = d3.scale.ordinal().rangeRoundBands([m.left, w - m.right]);
        var xAxis = d3.svg.axis().scale(x).orient('bottom');

        var y = d3.scale.linear().range([h - m.bottom, m.top]);
        var yAxis = d3.svg.axis().scale(y).orient('left');

        var max = 0;
        for (var i = 0; i < source.series.length; i++) {
            max += d3.max(source.series[i].data);
        }

        x.domain(source.categories);
        y.domain([0, max]);

        var svg = selection.select('svg').attr('width', w);

        // initial data
        svg.selectAll('g')
            .data(['x axis', 'x axis-info', 'y axis', 'data', 'legend'])
            .enter()
            .append('g')
            .attr({ 'class': function (d) { return d } });

        // render axis
        svg.select('.x.axis')
            .attr({ 'transform': 'translate(0, ' + (h - m.bottom) + ')' })
            .transition()
            .duration(500)
            .call(xAxis);

        svg.select('.x.axis-info')
            .attr({ 'transform': 'translate(0, ' + (h - m.bottom) + ')' })
            .transition()
            .duration(500)
            .call(xAxis);

        svg.select('.y.axis')
            .attr({ 'transform': 'translate(' + m.left + ', 0)' })
            .transition()
            .duration(500)
            .call(yAxis);

        var yTemp = {};
        var colors = d3.scale.category10();

        selection.select('.data').selectAll('rect').classed('inactive', true);
        for (var idx = 0; idx < source.series.length; idx++) {
            var rect = selection.select('.data').selectAll('rect.rect' + idx).data(source.categories);
            rect.exit().remove();
            rect.enter().append('rect');
            rect.attr({ 'class': 'rect' + idx, fill: colors(idx) });

            rect.transition().attr({
                x: function (d) { return x(d) },
                y: function (d, i) {
                    var value = source.series[idx].data[i] || 0;
                    yTemp[i] = (yTemp[i] || 0) + value;
                    return y(yTemp[i]);
                },
                width: x.rangeBand() - 1,
                height: function (d, i) {
                    var value = source.series[idx].data[i] || 0;
                    var height = h - y(value) - m.bottom - 1;
                    return (height < 0) ? 0 : height;
                },
            });
        }
        selection.select('.data').selectAll('rect.inactive').remove();
        renderLegend(source, m, h);
    }

    function lineChart(source) {
        selection.selectAll('svg').data(d3.range(1)).enter().append('svg');
        selection.selectAll('svg').attr({
            'height': function () {
                var height = '400';
                if (source.attr && source.attr.height) height = source.attr.height;
                return height;
            },
            'class': 'debug',
            'style': 'margin-top: -5px'
        });

        var w = selection.style('width').replace('px', '');
        var h = selection.style('height').replace('px', '');
        var m = { top: 30, right: 30, bottom: 30, left: 50 };
        if (source.attr && source.attr.margin) {
            var margin = source.attr.margin;
            m.top = margin.top || m.top;
            m.right = margin.right || m.right;
            m.bottom = margin.bottom || m.bottom;
            m.left = margin.left || m.left;
        }

        m.bottom = m.bottom + (15 * source.series.length) + 10;

        var x = d3.scale.ordinal().rangeRoundBands([m.left, w - m.right]);
        var xAxis = d3.svg.axis().scale(x).orient('bottom');

        var y = d3.scale.linear().range([h - m.bottom, m.top]);
        var yAxis = d3.svg.axis().scale(y).orient('left');

        var max = 0;
        for (var i = 0; i < source.series.length; i++) {
            var max_t = d3.max(source.series[i].data);
            if (max_t > max) max = max_t;
        }

        x.domain(source.categories);
        y.domain([-max * 0.05, max]);

        var svg = selection.select('svg');

        // initial data
        svg.selectAll('g')
            .data(['x axis', 'x axis-info', 'y axis', 'data', 'legend'])
            .enter()
            .append('g')
            .attr({ 'class': function (d) { return d } });

        // render axis
        svg.select('.x.axis')
            .attr({ 'transform': 'translate(0, ' + (h - m.bottom) + ')' })
            .transition()
            .duration(500)
            .call(xAxis);

        svg.select('.x.axis-info')
            .attr({ 'transform': 'translate(0, ' + (h - m.bottom) + ')' })
            .transition()
            .duration(500)
            .call(xAxis);

        svg.select('.y.axis')
            .attr({ 'transform': 'translate(' + m.left + ', 0)' })
            .transition()
            .duration(500)
            .call(yAxis);

        svg.select('.data').selectAll('g')
            .data(['line', 'circle'])
            .enter()
            .append('g')
            .attr({ 'class': function (d) { return d } });

        var yTemp = {};
        var colors = d3.scale.category10();

        selection.select('.data').selectAll('circle').classed('inactive', true);
        selection.select('.data .line').selectAll('g').classed('inactive', true);
        for (var idx = 0; idx < source.series.length; idx++) {
            var line1 = d3.svg.line()
                .x(function (d) { return x(d.day) + (x.rangeBand() / 2) })
                .y(function (d) { return y(0); });
            var line2 = d3.svg.line()
                .x(function (d) { return x(d.day) + (x.rangeBand() / 2) })
                .y(function (d) { return y(d.value); });

            if (selection.select('.data .line').selectAll('g.line' + idx)[0].length > 1) {
                selection.select('.data .line').selectAll('g.line' + idx).remove();
            }

            var data = [];
            (source.series[idx].data).forEach(function (d, i) {
                if (d && d > 0) {
                    data.push({ day: (i + 1), value: d });
                }
            });

            selection.select('.data .line').append('g').attr({ 'class': 'line' + idx }).append('svg:path').attr({
                d: line1(data),
                fill: 'none',
                stroke: colors(idx),
                'stroke-width': '2',
                'class': 'line'
            }).transition().duration(500).attr({ d: line2(data) });

            var circles = selection.select('.data .circle').selectAll('circle.circle' + idx).data(source.categories);
            circles.exit().remove();
            circles.enter().append('circle');
            circles.attr('class', 'circle' + idx);
            circles.transition().attr({
                cx: function (d) { return x(d) + x.rangeBand() / 2 },
                cy: function (d, i) {
                    var value = source.series[idx].data[i] || 0;
                    return y(value);
                },
                r: function (d, i) {
                    var value = source.series[idx].data[i] || 0;
                    return value > 0 ? 3 : 0;
                },
                stroke: colors(idx),
                'stroke-width': '2',
                fill: 'yellow',
            });
        }
        selection.select('.data').selectAll('circle.inactive').transition().duration(500).attr({ cy: y(0), r: 1 }).remove();
        selection.select('.data .line').selectAll('.inactive').transition().duration(500).attr({ 'visibility': 'hidden' }).remove();
        renderLegend(source, m, h);
    }

    function renderLegend(source, m, h) {
        var colors = d3.scale.category10();

        // render legend
        if (source.series.length == 0) {
            selection.select('.legend').selectAll('rect').remove();
            selection.select('.legend').selectAll('text').remove();
        }

        var pos = { x: m.left, y: h - (15 * source.series.length) - 10 }
        var legend = selection.select('.legend');

        var rect = legend.selectAll('rect').data(source.series);
        rect.exit().remove();
        rect.enter().append('rect');
        rect.transition().attr({
            x: pos.x,
            y: function (d, i) { return i * 15 + pos.y },
            width: 12,
            height: 12,
            fill: function (d, i) { return colors(i) }
        });

        var text = legend.selectAll('text').data(source.series);
        text.exit().remove();
        text.enter().append('text');
        text.transition().attr({
            x: (pos.x + 15),
            y: function (d, i) { return i * 15 + pos.y + 11 },
            fill: function (d, i) { return colors(i) },
            'font-size': 14
        }).text(function (d) { return d.text || d.name });
    }
}
