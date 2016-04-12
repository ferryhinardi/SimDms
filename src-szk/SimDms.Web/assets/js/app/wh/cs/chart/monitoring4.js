var widget = new SimDms.Widget({
    title: 'Data - Monitoring',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                { name: "Area", text: "Area", cls: "span6", type: "select" },
                { name: "CompanyCode", text: "Dealer", cls: "span6", type: "select" },
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
                widget.showToolbars(['refresh', 'expand', 'doExport']);
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['refresh', 'collapse', 'doExport']);
                break;
            case 'doExport':
                doExport();
                break;
            default:
                break;
        }
    },
});

widget.setSelect([
    { name: "Area", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT AREA --" },
]);

widget.render(function () {
    var initial = { Year: moment().format('YYYY'), Month: moment().format('M'), years: [], months: [] };

    for (var i = 0; i < 5; i++) { initial.years.push({ value: initial.Year - i, text: initial.Year - i }); }
    for (var i = 1; i <= 12; i++) { initial.months.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') }); }

    //$("[name=Area]").on("change", function () {
    //    widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=Area]").val() }, optionalText: "-- SELECT DEALER --" });
    //    $("[name=CompanyCode]").prop("selectedIndex", 0);
    //    $("[name=CompanyCode]").change();
    //});

    $("[name=Area]").on("change", function () {
        //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ListDealersNew", params: { area: $('#Area option:selected').text() }, optionalText: "-- SELECT ALL --" });
        widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=Area]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=CompanyCode]").prop("selectedIndex", 0);
        $("[name=CompanyCode]").change();

        //console.log($("#pnlFilter [name=Area]").val(), $('#Area option:selected').text());
    });


    //$("[name=CompanyCode]").on("change", function () {
    //    refresh();
    //});

    //widget.post('wh.api/combo/DealerList?LinkedModule=CS', function (result) {
    //widget.bind({ name: 'CompanyCode', onChange: refresh });
    widget.bind({ name: 'Year', text: '-- SELECT YEAR --', data: initial.years }); //, onChange: refresh 
    widget.bind({ name: 'Month', text: '-- SELECT MONTH --', data: initial.months }); //, onChange: refresh
    widget.populate(initial);

    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['PERIODE', 'OUTLET', 'DO DATA', 'DELIVERED', '3 DAYS CALL (BY DELIVERY)', '3 DAYS CALL (BY INPUT)'])
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

    $('#CompanyCode').attr('disabled', 'disabled');
})
//});

$('#Area').on('change', function () {
    if ($('#Area').val() != "") {
        $('#CompanyCode').removeAttr('disabled');
    } else {
        $('#CompanyCode').attr('disabled', 'disabled');
        $('#CompanyCode').select2('val', "");
    }
    $('#CompanyCode').select2('val', "");
});

function refresh(options) {
    var filter = widget.serializeObject('pnlFilter');
    if (filter.CompanyCode && filter.Year && filter.Month) {
        widget.post('wh.api/chart/CsDataTDaysCallDO', filter, function (result) {
            var data = result[0];
            var info = result[1];
            var tbody = d3.select('#pnlData tbody');
            var tdata = { DoData: 0, DeliveryDate: 0, TDaysCallData: 0, TDaysCallByInput: 0 };
            //console.log(data);
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
            d3.select('#pnlData .table-info').html('<strong>Sumber</strong> : Data Dealer Per ' + moment(info[0].LastUpdate).format('DD MMM YYYY HH:mm:ss'));
            console.log(info);
        })
    }
    else {
        d3.select('#pnlData tbody').selectAll('tr').remove();
        d3.select('#pnlData tfoot').selectAll('tr').remove();
    }
}

function doExport() {
    var params = widget.serializeObject('pnlFilter');
    params.CompanyText = $('#CompanyCode option:selected').text();
    if (params.CompanyCode && params.Year && params.Month) {
        widget.post('wh.api/chart/doExport', params, function (result) {
            if (result.message == "") {
                location.href = 'wh.api/chart/DownloadExcelFile?key=' + result.value + '&fileName=DataDoVS3DaysCall';
            }
        })
    };
}