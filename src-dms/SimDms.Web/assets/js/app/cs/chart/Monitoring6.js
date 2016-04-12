var widget = new SimDms.Widget({
    title: 'Customer Birthday Report',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: "BranchCode", text: "Outlet", cls: "span6", type: "select", disable: true },
                {
                    name: "ParStatus", text: "Status", type: "select", cls: "span3", items: [
                        { value: "0", text: "All Status" },
                        { value: "1", text: "Not Inputted" },
                        { value: "2", text: "Inputted" },
                    ],
                    fullItem: true
                },
                { name: "PeriodYear", text: "Year", type: "select", cls: "span3" },
                { name: "ParMonth1", text: "Month From", type: "select", cls: "span3" },
                { name: "ParMonth2", text: "Month To", type: "select", cls: "span3" },
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
    var initial = { StartMonths: [], EndMonths: [] };
    widget.select({ name: "PeriodYear", url: "cs.api/Combo/ListOfYear" });
    widget.post("cs.api/Combo/isGM", function (result) {
        if (result == 1) {
            widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
            $('#BranchCode').removeAttr('disabled');
        }
        else {
            widget.post("cs.api/Combo/CurrentBranchCode", function (result) {
                var branchcode = result;
                $('#BranchCode option:selected ').val(result[0].value);
                $('#BranchCode option:selected ').text(result[0].text);
                $('#BranchCode').attr('disabled', 'disabled');
            });
        }
    });

    for (var i = 1; i <= 12; i++) {
        initial.StartMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
        initial.EndMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
    }
    widget.select({ name: 'ParMonth1', data: initial.StartMonths });
    widget.select({ name: 'ParMonth2', data: initial.EndMonths });

    $('#refresh').on('click', refresh);
    $('#export').on('click', exports);
    refreshGrid();
});

function refreshGrid() {
    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['No', 'Nama Outlet', 'Bulan', 'Jumlah Customer', 'Input by CRO', 'Gift', 'SMS', 'Telephone', 'Letter', 'Souvenir'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                if (i == 0) return 'width:auto'
                else if (i == 1) return 'width:24%'
                else if (i == 2) return 'width:20%'
                else return 'width:auto'
            },
            'class': function (d, i) {
                if (i == 1) return '-'
                else if (i == 2) return '-'
                else return 'number'
            }
        })
        .text(function (d) { return d })
    var tbody = table.append('tbody');
    var tfoot = table.append('tfoot');
    var tinfo = d3.select('#pnlData').append('div').attr({ 'class': 'table-info' })
}

function refresh(options) {
    var filter = widget.serializeObject('pnlFilter');

    widget.post('cs.api/chart/CsReportCustBirthday', filter, function (result) {
        var tbody = d3.select('#pnlData tbody');
        var tdata = { JumlahCustomer: 0, InputByCRO: 0, Gift: 0, SMS: 0, Telephone: 0 }, weekly = 1;
        tbody.selectAll('tr').data(result).enter().append('tr');
        tbody.selectAll('tr').data(result).exit().remove();
        tbody.selectAll('tr')
            .data(result)
            .html(function (d, i) {
                var html = '';
                if (i != 0) {
                    if (result[i].Month != result[i - 1].Month)
                        weekly = 1;
                    else
                        weekly += 1;
                }
                else
                    weekly = (i + 1);
                html += '<td>' + (i + 1) + '</td>';
                html += '<td>' + d.OutletName + '</td>';
                html += '<td>' + moment(d.Month.toString(), 'MM').format('MMMM') + '</td>';
                html += '<td class="number">' + (isNaN(d.TotalCustomer) ? 0 : d.TotalCustomer) + '</td>';
                html += '<td class="number">' + (isNaN(d.Reminder) ? 0 : d.Reminder) + '</td>';
                html += '<td class="number">' + (isNaN(d.Gift) ? 0 : d.Gift) + '</td>';
                html += '<td class="number">' + (isNaN(d.SMS) ? 0 : d.SMS) + '</td>';
                html += '<td class="number">' + (isNaN(d.Telephone) ? 0 : d.Telephone) + '</td>';
                html += '<td class="number">' + (isNaN(d.Letter) ? 0 : d.Letter) + '</td>';
                html += '<td class="number">' + (isNaN(d.Souvenir) ? 0 : d.Souvenir) + '</td>';

                tdata.JumlahCustomer += d.TotalCustomer;
                tdata.InputByCRO += d.Reminder;
                tdata.Gift += d.Gift;
                tdata.SMS += d.SMS;
                tdata.Telephone += d.Telephone;
                tdata.Letter += d.Letter;
                tdata.Souvenir += d.Souvenir;

                return html;
            })
        var tfoot = d3.select('#pnlData tfoot');
        tfoot.html(function () {
            var html = '';
            html += '<tr>';
            html += '<td colspan="3">TOTAL </td>';
            html += '<td class="number">' + (isNaN(tdata.JumlahCustomer) ? 0 : tdata.JumlahCustomer) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.InputByCRO) ? 0 : tdata.InputByCRO) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.Gift) ? 0 : tdata.Gift) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.SMS) ? 0 : tdata.SMS) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.Telephone) ? 0 : tdata.Telephone) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.Letter) ? 0 : tdata.Letter) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.Souvenir) ? 0 : tdata.Souvenir) + '</td>';
            html += '</tr>';
            html += '<tr>';
            html += '<td colspan="3">Persentase </td>';
            html += '<td class="number">' + (isNaN(((tdata.JumlahCustomer / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.JumlahCustomer / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.InputByCRO / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.InputByCRO / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.Gift / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.Gift / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.SMS / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.SMS / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.Telephone / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.Telephone / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.Letter / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.Letter / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.Souvenir / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.Souvenir / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '</tr>';
            return html;
        });
    });
}

function exports(options) {
    var data = widget.serializeObject('pnlFilter');
    console.log(data, $('#ParMonth1 option:selected ').text());
    var filter = {
        BranchCode: data.BranchCode,
        PeriodYear: data.PeriodYear,
        ParMonth1: data.ParMonth1,
        ParMonth2: data.ParMonth2,
        ParStatus: data.ParStatus,
        ParMonth1Text: $('#ParMonth1 option:selected ').text(),
        ParMonth2Text: $('#ParMonth2 option:selected ').text()
    }

    widget.XlsxReport({
        url: 'cs.api/chart/CsReportCustBirthdayexport',
        type: 'xlsx',
        params: filter
    });
}