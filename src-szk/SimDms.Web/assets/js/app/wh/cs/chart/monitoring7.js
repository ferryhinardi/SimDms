var widget = new SimDms.Widget({
    title: 'Report - BPKB Reminder',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                { name: "CompanyCode", text: "Dealer", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                { name: "BranchCode", text: "Outlet", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                {
                    text: 'Ready Date', type: 'controls', cls: 'span6', items: [
                    { name: 'DateFrom', type: 'datepicker', cls: 'span3' },
                    { name: 'DateTo', type: 'datepicker', cls: 'span3' },
                    { name: "SelectAll", text: "Show All", cls: "span2", type: "check" },
                    ]
                },
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
        { action: 'doExport', text: 'Export', icon: "fa fa-file-excel-o" },
        { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'refresh':
                refresh();
                break;
            case 'collapse':
                widget.exitFullWindow();
                widget.showToolbars(['refresh', 'doExport', 'expand']);
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['refresh', 'doExport', 'collapse']);
                break;
            case 'doExport':
                doExport();
                break;
            default:
                break;
        }
    },
});

widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
widget.render(function () {
    var date = new Date(moment(moment().format('YYYY-MM')));
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

    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['DEALER', 'OUTLET', 'READY DATE', 'JUMLAH CUSTOMER', 'Input by CRO', 'Tidak dapat dihubungi', 'PERSENTASE'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                if (i == 0) return 'width:15%'
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
});

function refresh(options) {
    var filter = widget.serializeObject('pnlFilter');
    widget.post('wh.api/chart/CsReportBPKBReminder', filter, function (result) {
        var dealers = result;
        var length = dealers.length;
        var tdata = { CustomerCount: 0, InputByCRO: 0, Unreachable: 0 };

        d3.select('#pnlData tbody').selectAll('tr').remove();
        
        var abbr = 0, abbr2 = 0;
        var rows = d3.select('#pnlData tbody')
            .selectAll('tr')
            .data(d3.range(length))
            .enter()
            .append('tr')
            .attr({
                'class': function (d, i) { return ((Math.floor(i / length)) % 2 == 0) ? 'even' : 'odd' }
            })
            .html(function (d, i) {
                var html = '', rowspan = 1, breaks = false, rowspan2 = 1, breaks2 = false;
                var intdlr = i % length;
                var dealer = dealers[intdlr];
                //var date = moment(filter.DateReff).add('days', Math.floor(i / length) - filter.Interval + 1)
                
                if (intdlr + rowspan < length) {
                    while (intdlr + rowspan < length && breaks == false) {
                        if (dealer.DealerAbbreviation == dealers[intdlr + rowspan].DealerAbbreviation) {
                            rowspan += 1;
                        }
                        else {
                            breaks = true;
                        }
                    }
                }

                if (intdlr + rowspan2 < length) {
                    while (intdlr + rowspan2 < length && breaks2 == false) {
                        if (dealer.OutletAbbreviation == dealers[intdlr + rowspan2].OutletAbbreviation) {
                            rowspan2 += 1;
                        }
                        else {
                            breaks2 = true;
                        }
                    }
                }

                if (intdlr == abbr) {
                    html += '<th rowspan=' + rowspan + '>' + dealer.DealerAbbreviation + '</th>'
                    abbr += rowspan;
                }
                if (intdlr == abbr2) {
                    html += '<th rowspan=' + rowspan2 + '>' + dealer.OutletAbbreviation + '</th>'
                    abbr2 += rowspan2;
                }
                //html += '<th class="">' + dealer.OutletAbbreviation + '</th>'
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
            html += '<td colspan=3>TOTAL </td>';
            html += '<td class="number">' + tdata.CustomerCount + '</td>';
            html += '<td class="number">' + tdata.InputByCRO + '</td>';
            html += '<td class="number">' + tdata.Unreachable + '</td>';
            html += '<td class="number">' + widget.numberFormat(total) + '%</td>';
            html += '</tr>';
            return html;
        });
    });

}

function doExport() {
    var params = widget.serializeObject('pnlFilter');
    widget.post('wh.api/chart/doExport4', params, function (result) {
        if (result.message == "") {
            location.href = 'wh.api/chart/DownloadExcelFile?key=' + result.value + '&fileName=BpkbReminder';
        }
    })
}