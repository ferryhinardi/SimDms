var widget = new SimDms.Widget({
    title: 'Register SPK By Periode (bar)',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                {
                    text: "Periode",
                    type: "controls",
                    cls: 'span8',
                    items: [
                        { name: 'Year', text: 'Year', type: 'select', cls: 'span4' },
                        { name: 'Month', text: 'Month', type: 'select', cls: 'span4' },
                    ]
                },
                {
                    text: "Area / Dealer",
                    type: "controls",
                    cls: 'span8',
                    items: [
                        { name: 'Area', text: 'Area', type: 'select', cls: 'span4', opt_text: '-- SELECT ALL --' },
                        { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span4', opt_text: '-- SELECT ALL --' },
                    ]
                },
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
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['refresh', 'collapse']);
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

    widget.setSelect([{
        name: "Area",
        url: "wh.api/combo/GroupAreas",
        optionalText: "-- SELECT ALL --"
    }]);

    $("[name=Area]").on("change", function () {
        widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=Area]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=Dealer]").prop("selectedIndex", 0);
        $("[name=Dealer]").change();
    });

    widget.populate(initial);
});

function refreshChart() {
    var height = $(window).height() - $('.header').height() - $('.navmenu').height() - $('.title').height() - 120;
    var height = $('.gl-widget').height() - 120;
    var filter = widget.serializeObject('pnlFilter');
    var categories = [];
    var series = []

    var filter = widget.serializeObject('pnlFilter');
    var periode = filter.Year + '-' + filter.Month;
    filter.DateFrom = moment(periode, 'YYYY-M').format('YYYY-MM-DD');
    filter.DateTo = moment(periode, 'YYYY-M').add('months', 1).add('days', -1).format('YYYY-MM-DD');
    var days = eval((filter.DateTo).substr(8));

    widget.post('wh.api/chart/SvRegisterSpk1', filter, function (result) {
        var groups = Enumerable.From(result).Select(function (d) { return d.DataSeries }).Distinct().ToArray();
        var categories = [], series = [];

        (groups).forEach(function (d, idx) {
            var serie = { name: d, data: [] }
            for (var i = 0; i < days; i++) {
                var datestring = moment(filter.DateFrom, 'YYYY-MM-DD').add('days', i).format('YYYYMMDD');
                var value = Enumerable.From(result)
                    .Where('x => x["DataSeries"] == "' + d + '" && x["DataKey"] == "' + datestring + '"')
                    .Select(function (d) { return d.DataValue })
                    .FirstOrDefault() || 0;

                categories.push(i + 1);
                serie.data.push(value);
            }

            series.push(serie);
        });

        d3.select("#pnlChart").datum({
            type: 'bar',
            categories: categories,
            series: series,
            attr: { height: (height < 250) ? 250 : height }
        }).call(widget.chart);
    })
}