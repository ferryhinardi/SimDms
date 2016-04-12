var widget = new SimDms.Widget({
    title: 'Trend Comparison Month & Month-1',
    xtype: 'panels',
    toolbars: [
        { name: 'btnRefresh', text: 'Refresh', icon: 'fa fa-refresh' },
    ],
    panels: [
        {
            name: 'pnlFilter',
            items: [
                {
                    text: 'From',
                    type: 'controls',
                    cls: 'span4',
                    items: [
                        { name: 'YearFrom', text: 'Year', cls: 'span4', type: 'select' },
                        { name: 'MonthFrom', cls: 'span4', type: 'select', },
                    ]
                },
                {
                    text: 'To',
                    type: 'controls',
                    cls: 'span4',
                    items: [
                        { name: 'YearTo', text: 'Year', cls: 'span4', type: 'select' },
                        { name: 'MonthTo', cls: 'span4', type: 'select', },
                    ]
                },
                { name: 'GroupModel', type: 'select', cls: 'span4', text: 'Group Model' },
                {
                    name: 'ChartType', type: 'select', cls: 'span4', text: 'Chart Type', opt_text: '-- SELECT CHART --',
                    items: [
                        { value: 'InqValue', text: 'Inquiry Chart' },
                        { value: 'SpkValue', text: 'SPK Chart' },
                        { value: 'FakturValue', text: 'Faktur Chart' },
                    ]
                },
            ]
        },
        {
            name: 'pnlChart',
            xtype: 'k-chart',
            footer: '<div class="table" id="pnlChartData"/><div>'
        },
    ],
});

widget.render(function () {
    var initial = {
        YearFrom: new Date().getFullYear(),
        MonthFrom: new Date().getMonth() + 1,
        YearTo: new Date().getFullYear(),
        MonthTo: new Date().getMonth() + 1
    };
    widget.setSelect([
        { name: 'YearFrom', url: 'wh.api/combo/ListOfYear', optionalText: '-- SELECT YEAR --' },
        { name: 'MonthFrom', url: 'wh.api/combo/ListOfMonth', optionalText: '-- SELECT MONTH --' },
        { name: 'YearTo', url: 'wh.api/combo/ListOfYear', optionalText: '-- SELECT YEAR --' },
        { name: 'MonthTo', url: 'wh.api/combo/ListOfMonth', optionalText: '-- SELECT MONTH --' },
        { name: 'GroupModel', url: 'wh.api/combo/GroupModelList', optionalText: '-- SELECT GROUP MODEL --' }
    ]);
    widget.populate(initial);
    $('.toolbar #btnRefresh').on('click', refresh);
    $('#pnlFilter select').on('change', refresh);

    function refresh() {
        var chart = { selector: '.body #pnlChart', axis: [], series: [] };
        var filter = widget.serializeObject('pnlFilter');
        if (filter.ChartType && filter.YearFrom && filter.MonthFrom && filter.YearTo && filter.MonthTo) {
            var params = {
                Periode1: filter.YearFrom + ('0' + filter.MonthFrom).substr(filter.MonthFrom.length - 1),
                Periode2: filter.YearTo + ('0' + filter.MonthTo).substr(filter.MonthTo.length - 1),
                GroupModel: filter.GroupModel
            };

            widget.post('wh.api/inquiry/PmDashboardByDay2', params, function (result) {
                var days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
                var colors = ['#993333', '#339933', '#333399', '#999933', '#339999', '#993399'];
                var periods = Enumerable.From(result).Distinct('$.Periode').Select('x => { name: x["Periode"], text: moment(x["Periode"], "YYYYMM").format("MMM YYYY") }').ToArray();

                for (var i = 0; i < 37; i++) { chart.axis.push(days[i % 7]) }
                for (var i = 0; i < periods.length; i++) {
                    var periode = periods[i];
                    var series = {
                        name: periode.text,
                        color: colors[i],
                        data: Enumerable.From(result).Where('$.Periode == ' + periode.name).Select('$.' + filter.ChartType).ToArray()
                    }
                    var day = parseInt(moment(periode.name, 'YYYYMMDD').format('d'));
                    //console.log(day, periode.name);
                    if (day == 0) day = 7;
                    for (var j = 1; j < day; j++) { series.data.unshift(null) }
                    chart.series.push(series);
                }

                if (result.length > 0) {
                    $(chart.selector).fadeIn();
                    $('.body #pnlChartData').fadeIn();
                    generateChart(chart);
                }
                else {
                    $(chart.selector).fadeOut();
                    $('.body #pnlChartData').fadeOut();
                }
            });
        }
        else {
            $(chart.selector).fadeOut();
            $('.body #pnlChartData').fadeOut();
        }
    }

    function generateChart(chart) {
        chart.title = 'Trend Comparison Month & Month-1 by '
            + $('[name=ChartType] option:selected').text()
            + ' (' + $('[name=GroupModel] option:selected').text() + ')';
        widget.lineChart(chart);
        widget.tableChart({
            name: 'Trend',
            cols: chart.axis,
            selector: '.body #pnlChartData',
            series: chart.series,
            hstyle: 'font-size:9px;font-weight:bold;width:2.5%;text-align:center',
            lstyle: 'font-size:11px;font-weight:bold;text-align:left',
            bstyle: 'font-size:9px;text-align:right',
        });
    }
});