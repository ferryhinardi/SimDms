var widget = new SimDms.Widget({
    title: 'Unit Intake Summary',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: 'Year', text: 'Year', type: 'select', cls: 'span3' },
                { name: 'Month', text: 'Month', type: 'select', cls: 'span3' },
                { name: 'Area', text: 'Area', type: 'select', cls: 'span6', opt_text: '-- SELECT ALL --' },
                { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span6', opt_text: '-- SELECT ALL --' },
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
    var initial = { Year: moment().format('YYYY'), Month: moment().format('M'), years: [], months: [] };

    for (var i = 0; i < 5; i++) { initial.years.push({ value: initial.Year - i, text: initial.Year - i }) }
    for (var i = 1; i <= 12; i++) { initial.months.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') }) }

    widget.bind({ name: 'Year', text: '-- SELECT YEAR --', data: initial.years });
    widget.bind({ name: 'Month', text: '-- SELECT MONTH --', data: initial.months });

    widget.post("wh.api/combo/UnitIntakeFilter", {}, function (result) {
        widget.bind({ name: 'Area', text: '-- SELECT ALL --', data: result[0], defaultAll: true });
    });

    $("[name=Area]").on("change", function () {
        widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=Area]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=Dealer]").prop("selectedIndex", 0);
        $("[name=Dealer]").change();
    });
    widget.populate(initial);
});

function refreshChart() {
    d3.select('#pnlChart').append('svg').attr({ 'class': 'chart debug', height: 300 });
    var filter = widget.serializeObject('pnlFilter');
    widget.post('wh.api/chart/SvUnitIntakeSummary', filter, function (result) {
        var data = result[0];
        if (data.length > 0) {
            var width = $('#pnlChart > svg.chart').width(),
                height = 370;

            var chart = nv.models.pieChart()
                .margin({ left: 30 })
                .x(function (d) { return d.DataKey })
                .y(function (d) { return d.DataValue })
                .color(d3.scale.category10().range())
                .showLegend(true)
                .width(width)
                .height(height);

            d3.select("#pnlChart > svg.chart")
                .datum(data)
                .transition().duration(1200)
                .attr('width', width)
                .attr('height', height)
                .call(chart);
        }
        else {
            d3.select("#pnlChart > svg.chart").selectAll('g').remove();
        }
    });
}