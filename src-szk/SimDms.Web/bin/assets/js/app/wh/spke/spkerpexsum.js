var widget = new SimDms.Widget({
    title: 'Eksekutif Summary - SPK Exhibition',
    xtype: 'panels',
    toolbars: [
        { action: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
        { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
        { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
    ],
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: 'DateFrom', text: 'DATE FROM', type: 'datepicker', cls: 'span4' },
                { name: 'DateTo', text: 'DATE TO', type: 'datepicker', cls: 'span4' },
                {
                    text: "SUMMARY TYPE",
                    type: "controls",
                    cls: "span6",
                    items: [
                        { name: "SummaryType", cls: "span4", type: "select", required: true }
                    ]
                }
            ]
        },
        {
            name: 'pnlModel',
            items: [
                { name: 'Ertiga', text: 'ERTIGA', cls: 'span4' },
                { name: 'PuMegaCarry', text: 'PU MEGA CARRY', cls: 'span4' },
                { name: 'KarimunWgn', text: 'KARIMUN WAGON R', cls: 'span4' },
                { name: 'PuFutura', text: 'PU FUTURA', cls: 'span4' },
                { name: 'KarimunWgnGs', text: 'KARIMUN WAGON R GS', cls: 'span4' },
                { name: 'Others', text: 'OTHERS', cls: 'span4' },
                { name: 'Celerio', text: 'CELERIO', cls: 'span4' },
                { name: 'AllModels', text: 'ALL MODELS', cls: 'span4' },
                { name: 'Ciaz', text: 'CIAZ', cls: 'span4' },
            ]
        },
        {
            name: 'pnlChart',
        },
    ],
    onInit: function (wgt) {
        $('.panel label').css({ 'font-size': '12px', 'text-align': 'left' });
        $('.panel > div > div').css({ 'padding-left': '140px', 'padding-right': '60px' });
        $('#pnlModel input').css({ 'text-align': 'right' });
        $('#pnlModel input').attr({ 'placeholder': '', 'readonly': 'readonly' });

        $('#pnlChart').html('<div class="span8"><svg></svg></div>');
        $('#pnlChart svg').css({ border: '1px solid #0098ff', height: 240, 'margin-top': '5px' });

        var init = {
            DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
            DateTo: new Date()
        }

        wgt.populate(init);
        wgt.select({ selector: "select[name=SummaryType]", url: "wh.api/combo/spkfiltercombo", selected: "1" });
        setTimeout(function () { wgt.call('refresh'); }, 1000);
    },
    onTbrClick: function (wgt, name) {
        switch (name) {
            case 'refresh':
                wgt.call('refresh');
                break;
            case 'collapse':
                wgt.exitFullWindow();
                wgt.showToolbars(['refresh', 'export', 'expand']);
                $('#pnlChart svg').css({ height: 240 });
                widget.call('refresh');
                break;
            case 'expand':
                wgt.requestFullWindow();
                wgt.showToolbars(['refresh', 'collapse']);
                $('#pnlChart svg').css({ height: 340 });
                widget.call('refresh');
                break;
            default:
                break;
        }
    },
    refresh: function () {
        widget.post('wh.api/spkexhibition/summary', widget.serializeObject('pnlFilter'), function (r) {
            if (r.success) {
                if (r.row && r.row.KarimunWgn && r.row.KarimunWgnGs) {
                    r.row.KarimunWgn = (r.row.KarimunWgn - r.row.KarimunWgnGs);
                }

                for (var key in r.row) {
                    r.row[key] = widget.numberFormat(r.row[key], 0);
                }

                var rDaily = [], rAccum = [];

                for (var key in r.data) {
                    var daily = [moment(r.data[key].TrxDate).format("YYYY-MM-DD"), r.data[key].TotalPerDay];
                    var accum = [moment(r.data[key].TrxDate).format("YYYY-MM-DD"), r.data[key].TotalAccum];
                    rDaily.push(daily);
                    rAccum.push(accum);
                }

                var data = [
                    {
                        'key': 'DAILY',
                        'bar': true,
                        'color': '#0971B2',
                        'values': rDaily
                    },
                    {
                        'key': 'ACUM',
                        'color': '#FF5300',
                        'values': rAccum
                    }
                ];

                widget.populate(r.row);
                widget.call('generateChart', data);
            }
        });
    },
    generateChart: function (data) {
        nv.addGraph(function () {
            var chart = nv.models.linePlusBarChart()
                  .margin({ top: 30, right: 60, bottom: 50, left: 70 })
                  .x(function (d, i) { return i })
                  .y(function (d, i) { return d[1] })
            ;

            chart.xAxis.tickFormat(function (d) {
                var dx = data[0].values[d] && data[0].values[d][0];
                if (dx) return d3.time.format('%m/%d')(new Date(dx))
            });

            chart.y1Axis.tickFormat(d3.format(',f'));
            chart.y2Axis.tickFormat(d3.format(',f'));

            chart.bars.forceY([0]);

            d3.select('#pnlChart svg').selectAll('rect').remove();
            d3.select('#pnlChart svg').datum(data).transition().duration(500).call(chart);

            nv.utils.windowResize(chart.update);

            return chart;
        });
    }
});


$("#pnlFilter [name=DateFrom],#pnlFilter [name=DateTo],#pnlFilter [name=SummaryType]").on("change", function () {
    widget.call('refresh');
    nv.utils.windowResize(widget.chart.update);
});