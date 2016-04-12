var widget = new SimDms.Widget({
    title: 'Reviews',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                { name: "CompanyCode", text: "Dealer", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                { name: "BranchCode", text: "Outlet", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                { name: "Plan", text: "Activity", cls: "span6", type: "select", opt_text: "-- SELECT ONE --" },
                {
                    text: 'Ready Date', type: 'controls', cls: 'span6', items: [
                    { name: 'DateFrom', type: 'datepicker', cls: 'span3' },
                    { name: 'DateTo', type: 'datepicker', cls: 'span3' }
                    ]
                },
            ],
        },
        {
            name: 'pnlFilter2',
            items: [
            ],
        },
        {
            name: 'pnlData',
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
                refresh();
                break;
            case 'collapse':
                widget.exitFullWindow();
                widget.showToolbars(['refresh', 'export', 'expand']);
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['refresh', 'export', 'collapse']);
                break;
            default:
                break;
        }
    },
});

widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
widget.render(function () {
    var date = new Date(moment(moment().format('YYYY-MM-')));
    var initial = { DateFrom: date + '01', DateTo: new Date() };

    widget.populate(initial);
    $("[name=GroupArea]").on("change", function () {
        widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=CompanyCode]").prop("selectedIndex", 0);
        $("[name=CompanyCode]").change();
    });
    $("[name=CompanyCode]").on("change", function () {
        widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=BranchCode]").prop("selectedIndex", 0);
        $("[name=BranchCode]").change();
    });

    $('#pnlFilter2').hide();

    //var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    //var thead = table.append('thead');
    //thead.append('tr')
    //    .selectAll('th')
    //    .data(['DEALER', 'READY DATE', 'JUMLAH CUSTOMER', 'Input by CRO', 'Tidak dapat dihubungi', 'PERSENTASE'])
    //    .enter()
    //    .append('th')
    //    .attr({
    //        'style': function (d, i) {
    //            if (i == 0) return 'width:15%'
    //            else return 'width:15%'
    //        },
    //        'class': function (d, i) {
    //            if (i == 0) return 'date'
    //                //else if (i == 4) return 'percent'
    //            else return 'number'
    //        }
    //    })
    //    .text(function (d) { return d })
    //var tbody = table.append('tbody');
    //var tfoot = table.append('tfoot');
});

function refresh(options) {
    $('#pnlData').html('');
    if ($('#Plan').val() == 'STNK EXTENSION') {
        refreshGridStnk();
        refreshStnk();
    }
    else if ($('#Plan').val() == '3 DAYS CALL') {
        refreshGridTday();
        refreshTday();
    }
    else if ($('#Plan').val() == 'BIRTHDAY CALL') {
        refreshGridBday();
        refreshBday();
    }
    else if ($('#Plan').val() == 'BPKB REMINDER') {
        refreshGridBpkb();
        refreshBpkb();
    }

}

function refreshGridStnk() {
    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['No', 'Cabang', 'Jumlah STNK', 'Input STNK by CRO', 'Persentase'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                return 'width:auto'
            },
            'class': function (d, i) {
                if (i != 0 && i != 1)
                    return 'number'
            }
        })
        .text(function (d) { return d })
    var tbody = table.append('tbody');
    var tfoot = table.append('tfoot');
    var tinfo = d3.select('#pnlData').append('div').attr({ 'class': 'table-info' })
}

function refreshStnk(options) {
    var filter = widget.serializeObject('pnlFilter');

    widget.post('cs.api/chart/CsReportSTNKExtention', filter, function (result) {
        var tbody = d3.select('#pnlData tbody');
        tbody.selectAll('tr').data(result).enter().append('tr');
        tbody.selectAll('tr').data(result).exit().remove();
        tbody.selectAll('tr')
            .data(result)
            .html(function (d, i) {
                var html = '';
                html += '<td>' + (i + 1) + '</td>';
                html += '<td>' + d.Outlet + '</td>';
                html += '<td class="number">' + d.CustomerCount + '</td>';
                html += '<td class="number">' + d.InputByCRO + '</td>';
                html += '<td class="number">' + (d.Percentation).toFixed(2) + ' %' + '</td>';
                return html;
            });
    });
}

function refreshGridBday() {
    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['No', 'Periode', 'Jumlah Customer', 'Input by CRO', 'Gift', 'SMS', 'Telephone'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                if (i == 0) return 'width:auto'
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
}

function refreshBday(options) {
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
                html += '<td>' + moment(d.Month.toString(), 'MM').format('MMMM') + " W " + weekly + '</td>';
                html += '<td class="number">' + (isNaN(d.TotalCustomer) ? 0 : d.TotalCustomer) + '</td>';
                html += '<td class="number">' + (isNaN(d.Reminder) ? 0 : d.Reminder) + '</td>';
                html += '<td class="number">' + (isNaN(d.Gift) ? 0 : d.Gift) + '</td>';
                html += '<td class="number">' + (isNaN(d.SMS) ? 0 : d.SMS) + '</td>';
                html += '<td class="number">' + (isNaN(d.Telephone) ? 0 : d.Telephone) + '</td>';

                tdata.JumlahCustomer += d.TotalCustomer;
                tdata.InputByCRO += d.Reminder;
                tdata.Gift += d.Gift;
                tdata.SMS += d.SMS;
                tdata.Telephone += d.Telephone;

                return html;
            })
        var tfoot = d3.select('#pnlData tfoot');
        tfoot.html(function () {
            var html = '';
            html += '<tr>';
            html += '<td colspan="2">TOTAL </td>';
            html += '<td class="number">' + (isNaN(tdata.JumlahCustomer) ? 0 : tdata.JumlahCustomer) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.InputByCRO) ? 0 : tdata.InputByCRO) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.Gift) ? 0 : tdata.Gift) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.SMS) ? 0 : tdata.SMS) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.Telephone) ? 0 : tdata.Telephone) + '</td>';
            html += '</tr>';
            html += '<tr>';
            html += '<td colspan="2">Persentase </td>';
            html += '<td class="number">' + (isNaN(((tdata.JumlahCustomer / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.JumlahCustomer / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.InputByCRO / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.InputByCRO / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.Gift / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.Gift / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.SMS / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.SMS / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.Telephone / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.Telephone / tdata.JumlahCustomer) * 100)).toFixed(2) + ' %' + '</td>';
            html += '</tr>';
            return html;
        });
    });
}

function refreshGridBpkb() {
    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['OUTLET NAME', 'READY DATE', 'JUMLAH CUSTOMER', 'Input by CRO', 'Tidak dapat dihubungi', 'PERSENTASE'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                if (i == 0) return 'width:25%'
                else return 'width:15%'
            },
            'class': function (d, i) {
                if (i == 0) return 'date'
                    //else if (i == 4) return 'percent'
                else return 'number'
            }
        })
        .text(function (d) { return d })
    var tbody = table.append('tbody');
    var tfoot = table.append('tfoot');
}

function refreshBpkb(options) {
    var filter = widget.serializeObject('pnlFilter');
    widget.post('cs.api/chart/CsReportBPKBReminder', filter, function (result) {
        var dealers = result;
        var length = dealers.length;
        var tdata = { CustomerCount: 0, InputByCRO: 0, Unreachable: 0 };

        d3.select('#pnlData tbody').selectAll('tr').remove();
        var rows = d3.select('#pnlData tbody')
            .selectAll('tr')
            .data(d3.range(length))
            .enter()
            .append('tr')
            .attr({
                'class': function (d, i) { return ((Math.floor(i / length)) % 2 == 0) ? 'even' : 'odd' }
            })
            .html(function (d, i) {
                var html = '';
                var intdlr = i % length;
                var dealer = dealers[intdlr];
                //var date = moment(filter.DateReff).add('days', Math.floor(i / length) - filter.Interval + 1)

                html += '<td class="">' + dealer.OutletAbbreviation + '</td>'
                html += '<th class="date">' + moment(dealer.BpkbReadyDate).format('MMM-YYYY') + '</th>'
                html += '<td class="number">' + widget.numberFormat(dealer.CustomerCount) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.InputByCRO) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.Unreachable) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.Percentation) + '%</td>'

                tdata.CustomerCount += dealer.CustomerCount;
                tdata.InputByCRO += dealer.InputByCRO;
                tdata.Unreachable += dealer.Unreachable;

                return html;
            })

        var tfoot = d3.select('#pnlData tfoot');
        tfoot.html(function () {
            var total = 0;
            if (!isNaN(tdata.InputByCRO / tdata.CustomerCount)) {
                total = (tdata.InputByCRO / tdata.CustomerCount) * 100;
            }
            var html = '';
            html += '<tr>';
            html += '<td colspan=2>TOTAL </td>';
            html += '<td class="number">' + tdata.CustomerCount + '</td>';
            html += '<td class="number">' + tdata.InputByCRO + '</td>';
            html += '<td class="number">' + tdata.Unreachable + '</td>';
            html += '<td class="number">' + total + '%</td>';
            html += '</tr>';
            return html;
        });
    });
}

function refreshGridTday() {
    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['Outlet', 'Jumlah BPK', 'Input 3 Days by CRO', 'PERSENTASE'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                if (i == 0) return 'width:15%'
                else return 'width:15%'
            },
            'class': function (d, i) {
            }
        })
        .text(function (d) { return d })
    var tbody = table.append('tbody');
    var tfoot = table.append('tfoot');
}

function refreshTday(options) {
    var filter = widget.serializeObject('pnlFilter');
    widget.post('cs.api/chart/CsReportTDayCall', filter, function (result) {
        var dealers = result;
        var length = dealers.length;
        var tdata = { CustomerCount: 0, InputByCRO: 0 };

        d3.select('#pnlData tbody').selectAll('tr').remove();
        var rows = d3.select('#pnlData tbody')
            .selectAll('tr')
            .data(d3.range(length))
            .enter()
            .append('tr')
            .attr({
                'class': function (d, i) { return ((Math.floor(i / length)) % 2 == 0) ? 'even' : 'odd' }
            })
            .html(function (d, i) {
                var html = '';
                var intdlr = i % length;
                var dealer = dealers[intdlr];
                //var date = moment(filter.DateReff).add('days', Math.floor(i / length) - filter.Interval + 1)

                html += '<td>' + dealer.OutletAbbreviation + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.CustomerCount) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.InputByCRO) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.Percentation) + '%</td>'

                tdata.CustomerCount += dealer.CustomerCount;
                tdata.InputByCRO += dealer.InputByCRO;

                return html;
            })

        var tfoot = d3.select('#pnlData tfoot');
        tfoot.html(function () {
            var total = 0;
            if (!isNaN(tdata.InputByCRO / tdata.CustomerCount)) {
                total = (tdata.InputByCRO / tdata.CustomerCount) * 100;
            }
            var html = '';
            html += '<tr>';
            html += '<td>TOTAL </td>';
            html += '<td class="number">' + tdata.CustomerCount + '</td>';
            html += '<td class="number">' + tdata.InputByCRO + '</td>';
            html += '<td class="number">' + widget.numberFormat(total) + '%</td>';
            html += '</tr>';
            return html;
        });
    });
}