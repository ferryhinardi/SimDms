var widget = new SimDms.Widget({
    title: 'Monitoring Input',
    xtype: 'panels',
    toolbars: [
        { name: 'btnRefresh', text: 'Refresh', icon: 'fa fa-refresh' },
    ],
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: 'Periode', text: 'Periode', type: 'datepicker', cls: 'span3' },
                {
                    name: 'InquiryType', type: 'select', cls: 'span4', text: 'Inquiry Type', opt_text: '-- SELECT DATA --',
                    items: [
                        { value: 'INQ', text: 'Inquiry' },
                        { value: 'SPK', text: 'SPK' },
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
    var initial = { Periode: new Date() };
    widget.populate(initial);
    $('.toolbar #btnRefresh').on('click', refresh);
    $('#pnlFilter input, #pnlFilter select').on('change', refresh);

    function refresh() {
        var filter = widget.serializeObject('pnlFilter');
        widget.post('wh.api/inquiry/PmMonitoring', filter, function (items) {
            generateTable(filter, items);
        });
    }

    function generateTable(filter, items) {
        var data = {};
        (items).forEach(function (item) {
            data[item.key] = item.value;
        });


        var cols = [];
        var rows = [];

        var dateInit = moment(filter.Periode).add('days', -7);
        var datarow = [];
        for (var i = 0; i < 15; i++) {
            var row = [];
            var daterow = moment(dateInit).add('days', i).format('DD');
            for (var j = 0; j < 8; j++) {
                var datecol = moment(dateInit).add('days', j).format('DD');

                var key = datecol + daterow;
                row.push(data[key] || 0);
            }

            if (datarow.length > 0) {
                for (var k = 0; k < datarow.length; k++) {
                    datarow[k] += row[k];
                }
            }
            else {
                datarow = row;
            }

            rows.push({ name: daterow, data: JSON.parse(JSON.stringify(datarow)) });
        }

        // check nilai terbesar
        var lastRow = rows[rows.length - 1].data;

        // looping rows-nya
        (rows).forEach(function (row, idx) {
            // looping perkolom tiap row
            (row.data).forEach(function (d, i) {
                // jika ketemu nilai sama dengan max nilai, nilai idx setelahnya dikosongkan
                if (row.data[i] == lastRow[i]) {
                    for (var j = idx + 1; j < rows.length ; j++) {
                        rows[j].data[i] = null;
                    }
                }
            });
        });

        for (var i = 0; i < 8; i++) {
            cols.push(moment(dateInit).add('days', i).format('DD'));
        }

        var tableinfo = (($("select[name='InquiryType'] option:selected").index() == 0) ? '-' : 'Input Date \\ ' + $('[name=InquiryType] option:selected').text() + ' Date');
        widget.tableChart({
            name: tableinfo,
            cols: cols,
            selector: '.body #pnlChart',
            series: rows,
            hstyle: 'font-size:14px;text-align:right;font-weight:bold;width:10%',
            bstyle: 'font-size:12px;text-align:right;font-weight:normal',
            lstyle: 'font-size:12px;text-align:left;font-weight:bold',
            format: true
        });
    }
});