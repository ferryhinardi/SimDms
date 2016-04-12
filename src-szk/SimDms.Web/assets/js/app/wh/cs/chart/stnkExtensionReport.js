var widget;
$(document).ready(function () {
    var options = {
        title: "STNK Extension Report",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    {
                        text: "Input Date",
                        type: "controls", items: [
                            { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                            { name: "SelectAll", text: "Show All", cls: "span2", type: "check" },
                        ]
                    },
                ],
            },
            {
                name: "StnkExt",
                xtype: "k-grid",
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
    }
    widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.render(function () {
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);

        widget.populate({ DateFrom: date2, DateTo: date1 });
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
        refreshGrid();
    });

    function refreshGrid() {
        var table = d3.select('#StnkExt').append('table').attr({ 'class': 'table-chart' })
        var thead = table.append('thead');
        thead.append('tr')
            .selectAll('th')
            .data(['No', 'Dealer', 'Cabang', 'Jumlah STNK', 'Input STNK by CRO', 'Persentase'])
            .enter()
            .append('th')
            .attr({
                'style': function (d, i) {
                    return 'width:auto'
                },
                'class': function (d, i) {
                    if (i != 0 && i != 1 && i != 2)
                        return 'number'
                }
            })
            .text(function (d) { return d })
        var tbody = table.append('tbody');
        var tfoot = table.append('tfoot');
        var tinfo = d3.select('#StnkExt').append('div').attr({ 'class': 'table-info' })
    }
});

function refresh(options) {
    var filter = widget.serializeObject('pnlFilter');
    console.log(filter)
    widget.post('wh.api/chart/StnkExtension', filter, function (result) {
        var tbody = d3.select('#StnkExt tbody');
        var tdata = { CustomerCount: 0, InputByCRO: 0 };

        tbody.selectAll('tr').data(result).enter().append('tr');
        tbody.selectAll('tr').data(result).exit().remove();
        tbody.selectAll('tr')
            .data(result)
            .html(function (d, i) {
                var html = '';
                //console.log(d);
                html += '<td>' + (i + 1) + '</td>';
                html += '<td>' + d.Dealer + '</td>';
                html += '<td>' + d.Outlet + '</td>';
                html += '<td class="number">' + (isNaN(d.CustomerCount) ? 0 : d.CustomerCount) + '</td>';
                html += '<td class="number">' + (isNaN(d.InputByCRO) ? 0 : d.InputByCRO) + '</td>';
                html += '<td class="number">' + (isNaN(d.Percentage) ? 0 : d.Percentage) + ' %' + '</td>';

                tdata.CustomerCount += d.CustomerCount;
                tdata.InputByCRO += d.InputByCRO;

                return html;
            });


        var tfoot = d3.select('#StnkExt tfoot');
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
            html += '<td class="number">' + widget.numberFormat(total) + '%</td>';
            html += '</tr>';
            return html;
        });
    });
}

function doExport() {
    var params = widget.serializeObject('pnlFilter');
    widget.post('wh.api/chart/doExport5', params, function (result) {
        if (result.message == "") {
            location.href = 'wh.api/chart/DownloadExcelFile?key=' + result.value + '&fileName=STNKExtension';
        }
    })
}