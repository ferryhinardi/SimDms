var widget;
$(document).ready(function () {
    var options = {
        title: "Customer Birthday Report",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet", type: "select", cls: "span6", opt_text: "-- SELECT ALL --" },
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
                    { name: "ShowAll", text: "Show All", type: "check", cls: "span3" },
                ],
            },
            {
                name: "CustBDay",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
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
            }
        }
    };

    widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" },
        { name: "PeriodYear", url: "wh.api/combo/years" }
    ]);
    widget.render(function () {
        $("label[for=ShowAll]").text("")
        var initial = { StartMonths: [], EndMonths: [] };
        for (var i = 1; i <= 12; i++) {
            initial.StartMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
            initial.EndMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
        }
        widget.bind({ name: 'ParMonth1', data: initial.StartMonths });
        widget.bind({ name: 'ParMonth2', data: initial.EndMonths });

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
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { companyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });

        $('#CompanyCode').attr('disabled', 'disabled');
        $('#BranchCode').attr('disabled', 'disabled');

        refreshGrid();
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


    function refreshGrid() {
        var table = d3.select('#CustBDay').append('table').attr({ 'class': 'table-chart' })
        var thead = table.append('thead');
        thead.append('tr')
            .selectAll('th')
            .data(['No', 'Nama Dealer', 'Bulan', 'Jumlah Customer', 'Input by CRO', 'Gift', 'SMS', 'Telephone'])
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
        var tinfo = d3.select('#CustBDay').append('div').attr({ 'class': 'table-info' })
    }
});

function refresh(options) {
    var filter = widget.serializeObject('pnlFilter');
    if (filter.ShowAll == undefined) filter.ShowAll = false;
    else filter.ShowAll = true;
    
    if ($("[name=PeriodYear]").val() == "") {
        sdms.info("Please Select Year");
        return;
    }
    else if ($("[name=ParMonth1]").val() == "") {
        sdms.info("Please Select Month From");
        return;
    }
    else if ($("[name=ParMonth2]").val() == "") {
        sdms.info("Please Select Month To");
        return;
    }
    widget.post('wh.api/chart/CustBirthdayReport', filter, function (result) {
        var tbody = d3.select('#CustBDay tbody');
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
                html += '<td>' + d.CompanyName + '</td>';
                html += '<td>' + moment(d.Month.toString(), 'MM').format('MMMM') + '</td>';
                html += '<td class="number">' + (isNaN(d.TotalCustomer) ? 0 : d.TotalCustomer)  + '</td>';
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
        var tfoot = d3.select('#CustBDay tfoot');
        tfoot.html(function () {
            var html = '';
            html += '<tr>';
            html += '<td colspan="3">TOTAL </td>';
            html += '<td class="number">' + (isNaN(tdata.JumlahCustomer) ? 0 : tdata.JumlahCustomer) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.InputByCRO) ? 0 : tdata.InputByCRO) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.Gift) ? 0 : tdata.Gift) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.SMS) ? 0 : tdata.SMS) + '</td>';
            html += '<td class="number">' + (isNaN(tdata.Telephone) ? 0 : tdata.Telephone) + '</td>';
            html += '</tr>';
            html += '<tr>';
            html += '<td colspan="3">Persentase </td>';
            html += '<td class="number">' + (isNaN(((tdata.JumlahCustomer / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.JumlahCustomer / tdata.JumlahCustomer) * 100).toFixed(2)) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.InputByCRO / tdata.JumlahCustomer)) * 100) ? 0 : ((tdata.InputByCRO / tdata.JumlahCustomer) * 100).toFixed(2)) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.Gift / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.Gift / tdata.JumlahCustomer) * 100).toFixed(2)) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.SMS / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.SMS / tdata.JumlahCustomer) * 100).toFixed(2)) + ' %' + '</td>';
            html += '<td class="number">' + (isNaN(((tdata.Telephone / tdata.JumlahCustomer) * 100)) ? 0 : ((tdata.Telephone / tdata.JumlahCustomer) * 100).toFixed(2)) + ' %' + '</td>';
            html += '</tr>';
            return html;
        });
    });
}

function doExport() {
    var params = widget.serializeObject('pnlFilter');
    params.CompanyText = $('#CompanyCode option:selected').text();
    params.month1Text = $('#ParMonth1 option:selected').text();
    params.month2Text = $('#ParMonth2 option:selected').text();
    if (params.ShowAll == undefined) params.ShowAll = false;
    else params.ShowAll = true;
    //if (params.GroupArea && params.CompanyCode && params.BranchCode && params.DateFrom && params.DateTo) {
    widget.post('wh.api/chart/doExport3', params, function (result) {
        if (result.message == "") {
            location.href = 'wh.api/chart/DownloadExcelFile?key=' + result.value + '&fileName=ReportCustBirthday';
        }
    })
    //};
}
