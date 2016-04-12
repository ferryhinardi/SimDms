var widget = new SimDms.Widget({
    title: 'Moving Average Chart',
    xtype: 'panels',
    toolbars: [
        { name: 'btnRefresh', text: 'Refresh', icon: 'fa fa-refresh' },
    ],
    panels: [
        {
            name: 'pnlFilter',
            items: [
                {
                    text: 'Filter',
                    type: 'controls',
                    cls: 'span8',
                    items: [
                        { name: 'Year', text: 'Year', cls: 'span2', type: 'select' },
                        { name: 'Month', cls: 'span2', type: 'select', },
                        {
                            name: 'ChartType', type: 'select', cls: 'span3', text: 'Chart', opt_text: '-- Select Chart --',
                            items: [
                                { value: 'ALL', text: 'All Moving Average Charts' },
                                { value: 'INQ', text: 'Moving Average Inquiry Chart' },
                                { value: 'SPK', text: 'Moving Average SPK Chart' },
                                { value: 'FKT', text: 'Moving Average Faktur Chart' },
                            ]
                        },
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
    var periode = '';
    var initial = { Year: new Date().getFullYear(), Month: new Date().getMonth() + 1 };
    var rows = undefined;

    widget.setSelect([
        { name: 'Year', url: 'wh.api/combo/ListOfYear', optionalText: '-- SELECT YEAR --' },
        { name: 'Month', url: 'wh.api/combo/ListOfMonth', optionalText: '-- SELECT MONTH --' }
    ]);
    widget.populate(initial);
    $('.toolbar #btnRefresh').on('click', refresh);
    $('[name=ChartType]').on('change', generateChart);
    $('[name=Year],[name=Month]').on('change', refresh);

    function refresh() {
        var filter = widget.serializeObject('pnlFilter');
        if (filter.ChartType && filter.Year && filter.Month) {
            periode = filter.Year + ('0' + filter.Month).substr(filter.Month.length - 1);
            widget.post('wh.api/inquiry/PmDashboardByDay', { Periode: periode }, function (result) {
                rows = result;
                generateChart();

                if (rows.length == 0) widget.showNotification('Tidak ditemukan data di periode tersebut...');
            });
        }
    }

    function generateChart() {
        if (rows) {
            var filter = widget.serializeObject('pnlFilter');
            var chart = {
                selector: '.body #pnlChart',
                title: '',
                axis: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31]
            };

            if (rows.length > 0 && filter.ChartType) {
                var inq = { name: 'Inquiry', color: 'red', data: [] };
                var spk = { name: 'SPK', color: 'green', data: [] };
                var fkt = { name: 'Faktur', color: 'blue', data: [] };
                (rows).forEach(function (row) {
                    inq.data.push(row['InqValue']);
                    spk.data.push(row['SpkValue']);
                    fkt.data.push(row['FakturValue']);
                });

                switch (filter.ChartType) {
                    case 'ALL':
                        chart.series = [inq, spk, fkt];
                        break;
                    case 'INQ':
                        chart.series = [inq];
                        break;
                    case 'SPK':
                        chart.series = [spk];
                        break;
                    case 'FKT':
                        chart.series = [fkt];
                        break;
                    default:
                        break;
                }
                $(chart.selector).fadeIn();
                $('.body #pnlChartData').fadeIn();
                widget.lineChart(chart);
                widget.tableChart({
                    cols: chart.axis,
                    selector: '.body #pnlChartData',
                    series: chart.series,
                    hstyle: 'font-size:12px;font-weight:bold;width:3.0%;text-align:center',
                    lstyle: 'font-size:12px;font-weight:bold;text-align:left',
                    bstyle: 'font-size:10px;text-align:right',
                });
            }
            else {
                $(chart.selector).fadeOut();
                $('.body #pnlChartData').fadeOut();
            }
        }
        else {
            refresh();
        }
    }
});