var widget = new SimDms.Widget({
    title: 'Report - 3 Day Call',
    xtype: 'panels',
    panels: [
        {
            name: 'pnlFilter',
            items: [
                    { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    {
                        text: 'Delivery Date', type: 'controls', cls: 'span6', items: [
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
    //$("[name=GroupArea]").on("change", function () {
    //    widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
    //    $("[name=CompanyCode]").prop("selectedIndex", 0);
    //    $("[name=CompanyCode]").change();
    //});
    //$("[name=CompanyCode]").on("change", function () {
    //    widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
    //    $("[name=BranchCode]").prop("selectedIndex", 0);
    //    $("[name=BranchCode]").change();
    //});

    $("[name=GroupArea]").on("change", function () {
        //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ListDealersNew", params: { area: $('#GroupArea option:selected').text() }, optionalText: "-- SELECT ALL --" });
        widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=CompanyCode]").prop("selectedIndex", 0);
        $("[name=CompanyCode]").change();

        console.log($("#pnlFilter [name=GroupArea]").val(), $("#pnlFilter [name=CompanyCode]").val(), $("#pnlFilter [name=BranchCode]").val(""));
    });
    $("[name=CompanyCode]").on("change", function () {
        //widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ListBranchesNew", params: { area: $('#GroupArea option:selected').text(), comp: $('#CompanyCode option:selected').text(), compText: $("#CompanyCode option:selected").text() }, optionalText: "-- SELECT ALL --" });
        widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { companyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=BranchCode]").prop("selectedIndex", 0);
        $("[name=BranchCode]").change();

        console.log($("#pnlFilter [name=CompanyCode]").val(), $('#CompanyCode option:selected').text());
    });

    var table = d3.select('#pnlData').append('table').attr({ 'class': 'table-chart' })
    var thead = table.append('thead');
    thead.append('tr')
        .selectAll('th')
        .data(['Dealer', 'Outlet', 'DELIVERED', 'Input 3 Days CRO by DELIVERY', 'PERSENTASE'])
        .enter()
        .append('th')
        .attr({
            'style': function (d, i) {
                if (i == 0) return 'width:10%'
                if (i == 1) return 'width:25%'
                else return 'width:12.5%'
            },
            'class': function (d, i) {
            }
        })
        .text(function (d) { return d })
    var tbody = table.append('tbody');
    var tfoot = table.append('tfoot');

    $('#CompanyCode').attr('disabled', 'disabled');
    $('#BranchCode').attr('disabled', 'disabled');
});

$('#GroupArea').on('change', function () {
    if ($('#GroupArea').val() != "") {
        $('#CompanyCode').removeAttr('disabled');
    } else {
        $('#CompanyCode').attr('disabled', 'disabled');
        $('#BranchCode').attr('disabled', 'disabled');
        $('#CompanyCode').select2('val', "");
        $('#BranchCode').select2('val', "");
    }
    $('#CompanyCode').select2('val', "");
    $('#BranchCode').select2('val', "");
});

$('#CompanyCode').on('change', function () {
    if ($('#CompanyCode').val() != "") {
        $('#BranchCode').removeAttr('disabled');
    } else {
        $('#BranchCode').attr('disabled', 'disabled');
    }
    $('#BranchCode').select2('val', "");
});

function refresh(options) {
    var filter = widget.serializeObject('pnlFilter');
    widget.post('wh.api/chart/CsReportTDayCall', filter, function (result) {
        var dealers = result;
        var length = dealers.length;
        //var tdata = { CustomerCount: 0, InputByCRO: 0 };

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

                html += '<td>' + dealer.DealerAbbreviation + '</td>'
                html += '<td>' + dealer.OutletName + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.CustomerCount) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.InputByCRO) + '</td>'
                html += '<td class="number">' + widget.numberFormat(dealer.Percentation) + '%</td>'

                //tdata.CustomerCount += dealer.CustomerCount;
                //tdata.InputByCRO += dealer.InputByCRO;

                return html;
            })

        //var tfoot = d3.select('#pnlData tfoot');
        //tfoot.html(function () {
        //    var total = 0;
        //    if (!isNaN(tdata.InputByCRO / tdata.CustomerCount)) {
        //        total = (tdata.InputByCRO / tdata.CustomerCount) * 100;
        //    }
        //    var html = '';
        //    html += '<tr>';
        //    html += '<td colspan=2>TOTAL </td>';
        //    html += '<td class="number">' + tdata.CustomerCount + '</td>';
        //    html += '<td class="number">' + tdata.InputByCRO + '</td>';
        //    html += '<td class="number">' + total + '%</td>';
        //    html += '</tr>';
        //    return html;
        //});
    });
}

function doExport() {
    var params = widget.serializeObject('pnlFilter');
    //if (params.GroupArea && params.CompanyCode && params.BranchCode && params.DateFrom && params.DateTo) {
        widget.post('wh.api/chart/doExport2', params, function (result) {
            if (result.message == "") {
                location.href = 'wh.api/chart/DownloadExcelFile?key=' + result.value + '&fileName=Report3DaysCall';
            }
        })
    //};
}