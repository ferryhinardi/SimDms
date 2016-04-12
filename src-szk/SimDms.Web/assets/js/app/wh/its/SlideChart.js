var widget = new SimDms.Widget({
    title: 'Slide Chart',
    xtype: 'panels',
    toolbars: [
        { name: 'btnPlay', text: 'Play', icon: 'fa fa-play' },
        { name: 'btnPause', text: 'Pause', icon: 'fa fa-pause', cls: 'hide' },
        { name: 'btnStop', text: 'Stop', icon: 'fa fa-stop', cls: 'hide' },
    ],
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: 'summary', text: 'Executive Summary', type: 'switch', float: 'left' },
                { name: 'average', text: 'Moving Average', type: 'switch', float: 'left' },
                {
                    panel: 'average',
                    text: 'Periode',
                    type: 'controls',
                    cls: 'hide',
                    items: [
                        { name: 'Year', cls: 'span2', type: 'select' },
                        { name: 'Month', cls: 'span2', type: 'select', },
                    ]
                },
                { name: 'trend', text: 'Trend Chart', type: 'switch', float: 'left' },
                {
                    panel: 'trend',
                    text: 'From',
                    type: 'controls',
                    cls: 'hide',
                    items: [
                        { name: 'YearFrom', cls: 'span2', type: 'select' },
                        { name: 'MonthFrom', cls: 'span2', type: 'select', },
                    ]
                },
                {
                    panel: 'trend',
                    text: 'To',
                    type: 'controls',
                    cls: 'hide',
                    items: [
                        { name: 'YearTo', cls: 'span2', type: 'select' },
                        { name: 'MonthTo', cls: 'span2', type: 'select', },
                    ]
                },
                { name: 'spke', text: 'SPK Exhibition', type: 'switch', float: 'left' },
                {
                    panel: 'spke',
                    text: 'Periode',
                    type: 'controls',
                    cls: 'hide',
                    items: [
                        { name: 'SpkeDateFrom', cls: 'span2', type: 'datepicker' },
                        { name: 'SpkeDateTo', cls: 'span2', type: 'datepicker', },
                    ]
                },
                {
                    text: 'Slide Interval (sec)',
                    type: 'controls',
                    items: [
                        { name: 'SlideInterval', text: 'Slide Interval ', type: 'int', cls: 'span2' },
                    ]
                },
            ]
        },
        {
            name: 'pnlSlide',
            type: 'slide',
            cls: 'presenter',
        }
    ],
    onSwitchChange: function (name, value) {
        var panel = $('[data-panel=' + name + ']');
        if (value) {
            panel.removeClass('hide');
        }
        else {
            panel.addClass('hide');
        }
    }
});

widget.render(function () {
    var busy = false;
    var initial = {
        Year: new Date().getFullYear(),
        Month: new Date().getMonth() + 1,
        YearFrom: new Date().getFullYear(),
        MonthFrom: new Date().getMonth() + 1,
        YearTo: new Date().getFullYear(),
        MonthTo: new Date().getMonth() + 1,
        SpkeDateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
        SpkeDateTo: new Date(),
        SlideInterval: 15
    };

    widget.setSelect([
        { name: 'Year', url: 'wh.api/combo/ListOfYear', optionalText: '-- SELECT YEAR --' },
        { name: 'Month', url: 'wh.api/combo/ListOfMonth', optionalText: '-- SELECT MONTH --' },
        { name: 'YearFrom', url: 'wh.api/combo/ListOfYear', optionalText: '-- SELECT YEAR --' },
        { name: 'MonthFrom', url: 'wh.api/combo/ListOfMonth', optionalText: '-- SELECT MONTH --' },
        { name: 'YearTo', url: 'wh.api/combo/ListOfYear', optionalText: '-- SELECT YEAR --' },
        { name: 'MonthTo', url: 'wh.api/combo/ListOfMonth', optionalText: '-- SELECT MONTH --' },
        { name: 'GroupModel', url: 'wh.api/combo/GroupModelList', optionalText: '-- SELECT GROUP MODEL --' }
    ]);
    setTimeout(function () { widget.populate(initial) }, 3000);
    widget.populate(initial);
    widget.onTbrClick('btnPlay', function () {
        drawChart();
    });
    widget.onTbrClick('btnPause', function () {
        if (SimDms.activeTimer) clearInterval(SimDms.activeTimer);
    });
    widget.onTbrClick('btnStop', function () {
        if (SimDms.activeTimer) clearInterval(SimDms.activeTimer);

        widget.showToolbars(['btnPlay']);
        widget.exitFullscreen();
        //$('body').removeClass('play');
        $('#pnlFilter').show();
        $('.slide').hide();
        $('.page > .title > h3').text('Slide Chart');
    });

    function drawChart() {
        var filter = widget.serializeObject('pnlFilter');
        var periode = filter.Year + ('0' + filter.Month).substr(filter.Month.length - 1);
        var periodeFrom = filter.YearFrom + ('0' + filter.MonthFrom).substr(filter.MonthFrom.length - 1);
        var periodeTo = filter.YearTo + ('0' + filter.MonthTo).substr(filter.MonthTo.length - 1);
        var initChartPanel = false;
        var chartloop = 0;
        var firstLoad = true;

        // set minimum internal 3 secs
        if (!filter.SlideInterval || filter.SlideInterval < 3) {
            filter.SlideInterval = 3;
            widget.populate(filter);
        }

        var charts = ['INQ', 'SPK', 'FKT'];
        var groups = ['ALL', 'ERTIGA', 'OTHER', 'PU FUTURA', 'PU MEGA CARRY', 'WAGON R'];
        var slides = [];

        console.log(filter);


        // Executive Summary Chart
        if (filter.summary === "true") {
            slides.push({ type: 'summary', text: 'Executive Summary Chart', api: 'inquiry/PmDashboardData', params: { name: 'PmExecutiveSummary2' } });
        }

        // Moving Average
        if (filter.average ===   "true") {
            for (var i = 0; i < charts.length; i++) {
                slides.push({
                    type: 'average', api: 'inquiry/PmDashboardByDay',
                    text: 'Moving Average Chart',
                    params: { Periode: periode, ChartType: charts[i] }
                });
            }
        }

        // Trend
        if (filter.trend === "true") {
            for (var i = 0; i < groups.length; i++) {
                for (var j = 0; j < charts.length; j++) {
                    slides.push({
                        type: 'trend', api: 'inquiry/PmDashboardByDay2',
                        text: 'Trend Comparison Month & Month - 1',
                        params: { Periode1: periodeFrom, Periode2: periodeTo, GroupModel: groups[i], ChartType: charts[j] }
                    });
                }
            }
        }

        // Executive Summary Chart
        if (filter.spke === "true") {
            slides.push({
                type: 'spke',
                text: 'Suzuki Exhibition by SPK Date',
                api: 'spkexhibition/summary',
                params: {
                    DateFrom: moment(filter.SpkeDateFrom).format('YYYY-MM-DD'),
                    DateTo: moment(filter.SpkeDateTo).format('YYYY-MM-DD'),
                    SummaryType: 1
                }
            });

            slides.push({
                type: 'spke',
                text: 'Suzuki Exhibition by Inquiry Date',
                api: 'spkexhibition/summary',
                params: {
                    DateFrom: moment(filter.SpkeDateFrom).format('YYYY-MM-DD'),
                    DateTo: moment(filter.SpkeDateTo).format('YYYY-MM-DD'),
                    SummaryType: 2
                }
            });
        }

        if (slides.length == 0) {
            widget.showNotification('Tidak terdapat chart yang dipilih...!');
            $('.slide.presenter').html('');
            return;
        }
        else {
            $('#pnlFilter').hide();
            $('.slide.presenter').show();
            widget.requestFullscreen();
            widget.showToolbars(['btnStop']);
        }

        if (!initChartPanel) {
            var i = 0;
            var html = '';
            //var width = $(window).width() + 'px';
            slides.forEach(function (slide) {
                slide.name = 'divslide' + (++i);
                switch (slide.type) {
                    case 'summary':
                        html += '<div class="' + slide.name + ' step">Summary</div>';
                        break;
                    case 'average':
                        html += '<div class="' + slide.name + ' step"><div class="chart"></div><div class="table"></div></div>';
                        break;
                    case 'trend':
                        html += '<div class="' + slide.name + ' step"><div class="chart"></div><div class="table"></div></div>';
                        break;
                    case 'spke':
                        html += '<div class="' + slide.name + ' step"><div class="info"></div><div class="chart"></div></div>';
                        break;
                    default:
                        html += '<div class="' + slide.name + ' step">' + JSON.stringify(slide) + '</div>';
                        break;
                }
            });
            $('.slide.presenter').html(html);

            initChartPanel = true;
        }

        widget.setInterval(filter.SlideInterval * 1000, function () {
            callSlide();
        });

        callSlide();

        function callSlide() {
            if (chartloop >= slides.length) chartloop = 0;
            if (firstLoad) {
                loadChart(slides[0]);
                firstLoad = false;
            };

            var slide_prev = ((chartloop - 1) < 0) ? (slides.length - 1) : (chartloop - 1);
            var slide_curr = chartloop
            var slide_next = ((chartloop + 1) >= slides.length) ? 0 : (chartloop + 1);

            widget.switchSlide({
                slide1: '.presenter.slide .step.' + slides[slide_prev].name,
                slide2: '.presenter.slide .step.' + slides[slide_curr].name
            });

            loadChart(slides[slide_next]);

            $('.page > .title > h3').text(slides[chartloop].text);

            chartloop++;
        }

        function loadChart(slide) {
            if (!busy) {
                if (!firstLoad) busy = true;
                widget.xpost('wh.api/' + slide.api, (slide.params || {}), function (result) {
                    busy = false;

                    if (slide.type == 'summary') drawChart1(slide, result);
                    if (slide.type == 'average') drawChart2(slide, result);
                    if (slide.type == 'trend') drawChart3(slide, result);
                    if (slide.type == 'spke') drawChart4(slide, result);
                })
            }
        }
    }

    function drawChart1(slide, rows) {
        var html = "";
        var table1 = "";
        var table2 = "";

        table1 = "<table class=\"dashboard\">"

               + "<tr>"
               + "<th style='width:25%' class='blank info' rowspan='3'>INDENT</th>"
               + "<th style='width:20%' class='header title'>Ertiga</th>"
               + "<th style='width:15%' class='header number' id='H1'></th>"
               + "<th style='width:5%'  class='header blank'></th>"
               + "<th style='width:20%' class='header title'>PU Mega Carry</th>"
               + "<th style='width:15%' class='header number' id='H4'></th>"
               + "</tr>"

               + "<tr>"
               + "<th class='header title'>Karimun Wagon R</th>"
               + "<th class='header number' id='H2'></th>"
               + "<th class='header blank'></th>"
               + "<th class='header title'>Others</th>"
               + "<th class='header number' id='H5'></th>"
               + "</tr>"

               + "<tr>"
               + "<th class='header title'>PU Futura</th>"
               + "<th class='header number' id='H3'></th>"
               + "<th class='header blank'></th>"
               + "<th class='header title'>All Model</th>"
               + "<th class='header number' id='H6'></th>"
               + "</tr>"

               + "</table>";

        table2 = "<table class=\"dashboard\">"

               + "<tr>"
               + "<th style='width:25%' class='blank center' rowspan='2'></th>"
               + "<th style='width:25%' class='title center' colspan='3'>Inquiry</th>"
               + "<th style='width:25%' class='title center' colspan='3'>SPK</th>"
               + "<th style='width:25%' class='title center' colspan='3'>Faktur</th>"
               + "</tr>"

               + "<tr>"
               + "<th style='width:8.33%' class='title small'>AVG L/M</th>"
               + "<th style='width:8.33%' class='title small'>D-31 <br/> s/d D-2</th>"
               + "<th style='width:8.33%' class='title small'>D-1</th>"
               + "<th style='width:8.33%' class='title small'>AVG L/M</th>"
               + "<th style='width:8.33%' class='title small'>D-30 <br/> s/d D-1</th>"
               + "<th style='width:8.33%' class='title small'>D</th>"
               + "<th style='width:8.33%' class='title small'>AVG L/M</th>"
               + "<th style='width:8.33%' class='title small'>D-30 <br/> s/d D-1</th>"
               + "<th style='width:8.33%' class='title small'>D</th>"
               + "</tr>"

               + "<tr>"
               + "<td class='strong title'>Ertiga</td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI11'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI21'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI31'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS11'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS21'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS31'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF11'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF21'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF31'></span></td>"
               + "</td>"

               + "<tr>"
               + "<td class='strong title'>Karimun Wagon R</td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI12'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI22'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI32'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS12'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS22'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS32'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF12'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF22'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF32'></span></td>"
               + "</td>"

               + "<tr>"
               + "<td class='strong title'>PU Futura</td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI13'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI23'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI33'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS13'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS23'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS33'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF13'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF23'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF33'></span></td>"
               + "</td>"

               + "<tr>"
               + "<td class='strong title'>PU Mega Carry</td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI14'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI24'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI34'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS14'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS24'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS34'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF14'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF24'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF34'></span></td>"
               + "</td>"

               + "<tr>"
               + "<td class='strong title'>Others</td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI15'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI25'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI35'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS15'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS25'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS35'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF15'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF25'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF35'></span></td>"
               + "</td>"

               + "<tr>"
               + "<td class='strong title'>All Model</td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI16'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI26'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DI36'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS16'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS26'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DS36'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF16'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF26'></span></td>"
               + "<td class=\"number\"><span class=\"animated\" id='DF36'></span></td>"
               + "</td>"

               + "</table>";

        html = table1 + table2;
        $('.slide.presenter .' + slide.name).html(html);
        rows.forEach(function (row) {
            $('.slide.presenter .' + slide.name + ' #' + row.GroupType + row.GroupSeq).text(widget.numberFormat(row.DataCount));
        });
    }

    function drawChart2(slide, rows) {
        var maps = {
            SPK: 'SPK',
            INQ: 'Inquiry',
            FKT: 'Faktur'
        }
        var title = 'Moving Average Chart ' + maps[slide.params.ChartType] + ', Periode ' + moment(slide.params.Periode, 'YYYYMM').format('MMM YYYY');
        var chart = {
            selector: '.slide.presenter .' + slide.name + ' .chart',
            title: title,
            axis: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31]
        };

        if (rows.length > 0 && slide.params.ChartType) {
            var inq = { name: 'Inquiry', color: 'red', data: [] };
            var spk = { name: 'SPK', color: 'green', data: [] };
            var fkt = { name: 'Faktur', color: 'blue', data: [] };
            (rows).forEach(function (row) {
                inq.data.push(row['InqValue']);
                spk.data.push(row['SpkValue']);
                fkt.data.push(row['FakturValue']);
            });

            switch (slide.params.ChartType) {
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
            widget.lineChart(chart);

            widget.tableChart({
                cols: chart.axis,
                name: '-',
                selector: '.slide.presenter .' + slide.name + ' .table',
                series: chart.series,
                hstyle: 'font-size:13.5px;font-weight:normal;width:3.0%;text-align:center;padding:0 4px',
                lstyle: 'font-size:13.5px;font-weight:normal;text-align:left;padding:0 8px',
                bstyle: 'font-size:12.5px;text-align:right;padding-left:0;padding-right:5px;margin:0',
            });
        }
    }

    function drawChart3(slide, rows) {
        var maps = {
            SPK: 'SPK',
            INQ: 'Inquiry',
            FKT: 'Faktur'
        }
        var title = 'Trend Comparison Month & Month -1 (' + slide.params.GroupModel + ' / ' + maps[slide.params.ChartType] + ')';
        var chart = {
            selector: '.slide.presenter .' + slide.name + ' .chart',
            title: title,
            series: [],
            axis: [],
        };

        var days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
        var colors = ['#993333', '#339933', '#333399', '#999933', '#339999', '#993399'];
        var periods = Enumerable.From(rows).Distinct('$.Periode').Select('x => { name: x["Periode"], text: moment(x["Periode"], "YYYYMM").format("MMM YY") }').ToArray();
        var chartTypeMap = {
            SPK: 'SpkValue',
            INQ: 'InqValue',
            FKT: 'FakturValue',
        }

        for (var i = 0; i < 37; i++) { chart.axis.push(days[i % 7]) }
        for (var i = 0; i < periods.length; i++) {
            var periode = periods[i];
            var series = {
                name: periode.text,
                color: colors[i],
                data: Enumerable.From(rows).Where('$.Periode == ' + periode.name).Select('$.' + chartTypeMap[slide.params.ChartType]).ToArray()
            }

            var day = parseInt(moment(periode.name, 'YYYYMMDD').format('d'));
            if (day == 0) day = 7;
            for (var j = 1; j < day; j++) { series.data.unshift(null) }
            chart.series.push(series);
        }

        widget.lineChart(chart);
        widget.tableChart({
            name: '-',
            cols: chart.axis,
            selector: '.slide.presenter .' + slide.name + ' .table',
            series: chart.series,
            hstyle: 'font-size:13px;font-weight:normal;width:2.5%;text-align:center;padding: 0 4px',
            lstyle: 'font-size:13px;font-weight:normal;text-align:left;padding: 0 4px',
            bstyle: 'font-size:12px;text-align:right;padding-left:0;padding-right:2px',
        });
    }

    function drawChart4(slide, r) {
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

            generateInfo(r.row);
            generateChart(data);
        }

        function generateInfo(row) {
            var selector = '.slide.presenter .' + slide.name;

            var html = '';
            html += '<table style="width: 100%; padding-top: 20px">';
            html += '<tr>';
            html += ' <td style="width:30%">ERTIGA</td>';
            html += ' <td class="number">' + widget.numberFormat(row.Ertiga) + '</td>';
            html += ' <td style="width:30%">PU MEGA CARRY</td>';
            html += ' <td class="number">' + widget.numberFormat(row.PuMegaCarry) + '</td>';
            html += '</tr>';
            html += '<tr>';
            html += ' <td>KARIMUN WAGON R</td>';
            html += ' <td class="number">' + widget.numberFormat(row.KarimunWgn) + '</td>';
            html += ' <td>OTHERS</td>';
            html += ' <td class="number">' + widget.numberFormat(row.Others) + '</td>';
            html += '</tr>';
            html += '<tr>';
            html += ' <td>KARIMUN WAGON R GS</td>';
            html += ' <td class="number">' + widget.numberFormat(row.KarimunWgnGs) + '</td>';
            html += ' <td>ALL MODEL</td>';
            html += ' <td class="number">' + widget.numberFormat(row.AllModels) + '</td>';
            html += '</tr>';
            html += '<tr>';
            html += ' <td>PU FUTURA</td>';
            html += ' <td class="number">' + widget.numberFormat(row.PuFutura) + '</td>';
            html += ' <td></td>';
            html += ' <td class="number"></td>';
            html += '</tr>';
            html += '</table>';

            $(selector + ' .info').html(html);
            $(selector + ' .info table td').css({
                'font-size': '20px',
                'padding': '5px 0',
                'color': '#0971B2',
                'background-color': 'white',
                'border': 'none',
                'font-weight': 'bold'
            });
            $(selector + ' .info table td.number').css({
                'text-align': 'right',
                'padding-right': '50px',
            });
        }

        function generateChart(data) {
            var selector = '.slide.presenter .' + slide.name;
            $(selector + ' .chart').html('<div class="span8"><svg></svg></div>');
            $(selector + ' .chart svg').css({ border: '1px solid #0098ff', height: 400, 'margin-top': '5px' });

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

                chart.bars.forceY([0]);

                d3.select(selector + ' .chart svg').selectAll('rect').remove();
                d3.select(selector + ' .chart svg').datum(data).transition().duration(500).call(chart);

                return chart;
            });
        }
    }
});