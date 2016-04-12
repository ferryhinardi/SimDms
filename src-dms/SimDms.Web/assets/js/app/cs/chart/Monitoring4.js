var widget = new SimDms.Widget({
    title: 'Data - Monitoring',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: "BranchCode", text: "Outlet", cls: "span6", type: "select" },
                { name: 'Year', text: 'Year', type: 'select', cls: 'span3' },
                { name: 'Month', text: 'Month', type: 'select', cls: 'span3' },
            ],
        },
        {
            name: 'pnlData',
            xtype: 'k-grid',
        },
    ],
    toolbars: [
        { name: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
        { name: 'export', text: 'Export (xls)', icon: 'fa fa-file-excel-o' }
    ]
});

widget.render(function () {
    widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
    widget.select({ name: "Year", url: "cs.api/Combo/ListOfYear" });
    widget.select({ name: "Month", url: "cs.api/Combo/MonthList" });

    $('#BranchCode, #Year, #Month').on('change', refresh);
    $('#refresh').on('click', refresh);
    $('#export').on('click', exports);

    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['PERIODE', 'OUTLET', 'DO DATA', 'DELIVERED', '3 DAYS CALL (BY DO)', '3 DAYS CALL (BY INPUT)'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                if (i == 0) return 'width:160px'
                else if (i == 1) return 'width:auto'
                else return 'width:16%'
            },
            'class': function (d, i) {
                if (i == 0) return 'date'
                else if (i == 1) return '-'
                else return 'number'
            }
        })
        .text(function (d) { return d })
    var tbody = table.append('tbody');
    var tfoot = table.append('tfoot');
    var tinfo = d3.select('#pnlData').append('div').attr({ 'class': 'table-info' })

});

function refresh(options) {
    var filter = widget.serializeObject('pnlFilter');
    if (filter.BranchCode && filter.Year && filter.Month) {
        widget.post('cs.api/chart/CsDataTDaysCallDO', filter, function (result) {
            var data = result[0];
            var info = result[1];
            var tbody = d3.select('#pnlData tbody');
            var tdata = { DoData: 0, DeliveryDate: 0, TDaysCallData: 0, TDaysCallByInput: 0 };
            tbody.selectAll('tr').data(data).enter().append('tr');
            tbody.selectAll('tr').data(data).exit().remove();
            tbody.selectAll('tr')
                .data(data)
                .html(function (d, i) {
                    var html = '';
                    html += '<td>' + moment(filter.Year + '-' + filter.Month, 'YYYY-M').format('MMMM YYYY') + '</td>';
                    html += '<td>' + d.BranchCode + ' - ' + d.BranchName + '</td>';
                    html += '<td class="number">' + d.DoData + '</td>';
                    html += '<td class="number">' + d.DeliveryDate + '</td>';
                    html += '<td class="number">' + d.TDaysCallData + '</td>';
                    html += '<td class="number">' + d.TDaysCallByInput + '</td>';

                    tdata.DoData += d.DoData;
                    tdata.DeliveryDate += d.DeliveryDate;
                    tdata.TDaysCallData += d.TDaysCallData;
                    tdata.TDaysCallByInput += d.TDaysCallByInput;

                    return html;
                })

            var tfoot = d3.select('#pnlData tfoot');
            tfoot.html(function () {
                var html = '';
                html += '<tr>';
                html += '<td colspan="2">TOTAL </td>';
                html += '<td class="number">' + tdata.DoData + '</td>';
                html += '<td class="number">' + tdata.DeliveryDate + '</td>';
                html += '<td class="number">' + tdata.TDaysCallData + '</td>';
                html += '<td class="number">' + tdata.TDaysCallByInput + '</td>';
                html += '</tr>';
                return html;
            });
            //d3.select('#pnlData .table-info').html('<strong>Sumber</strong> : Data Outlet Per ' + moment(info[0].LastUpdate).format('DD MMM YYYY HH:mm:ss'));
            //console.log(info);
        })
    }
    else {
        d3.select('#pnlData tbody').selectAll('tr').remove();
        d3.select('#pnlData tfoot').selectAll('tr').remove();
    }
}

function exports(options) {
    var data = widget.serializeObject('pnlFilter');
    var today = new Date();
    
    var Periode;
    if (data.Year == '' || data.Year == null || data.Year == undefined || data.Month == '' || data.Month ==  null || data.Month == undefined) {
        Periode = moment(today).format('MMMM-YYYY')
    }
    else {
        Periode = moment(data.Year + '-' + data.Month, 'YYYY-M').format('MMMM YYYY')
    }
    console.log(data.Year, today, moment(today).format("MMMM YYYY"), Periode);
   
    var filter = {
        BranchCode: data.BranchCode,
        Year: data.Year,
        Month: data.Month,
        Periode:Periode
    }

    widget.XlsxReport({
        url: 'cs.api/chart/CsDataTDaysCallDOexport',
        type: 'xlsx',
        params: filter
    });
}
