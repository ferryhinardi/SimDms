/*menu ini hanya tampil pada role admin dan BM*/
//$(document).ready(function () {
//    var options = {
var widget = new SimDms.Widget({
    title: "Review GM",
    xtype: "panels",
    panels: [
        {
            name: "pnlFilter",
            title: "Review Activity",
            items: [
                { name: "BranchCode", text: "Outlet", cls: "span6", type: "select", disable: true },
                { name: 'Plan', text: 'Activity', type: 'select', cls: 'span6', required: true },
                {
                    name: "pnlFilter", text: "Input Date", cls: 'flt', type: "controls", items: [
                        { name: "DateFrom", text: "Date From", cls: "span2", type: "datepicker" },
                        { name: "DateTo", text: "Date To", cls: "span2", type: "datepicker" },
                    ]
                },
                {
                    name: "ParStatus", text: "Status", type: "select", cls: "span3 flt2", items: [
                        { value: "0", text: "All Status" },
                        { value: "1", text: "Not Inputted" },
                        { value: "2", text: "Inputted" },
                    ],
                    fullItem: true
                },
                { name: "PeriodYear", text: "Year", type: "select", cls: "span3 flt2" },
                { name: "ParMonth1", text: "Month From", type: "select", cls: "span3 flt2" },
                { name: "ParMonth2", text: "Month To", type: "select", cls: "span3 flt2" },
            ]
        },
        { name: 'pnlData', xtype: 'k-grid', },
        {
            title: "Review Details",
            items: [
                //{
                //    text: "Company",
                //    type: "controls",
                //    cls: "span6",
                //    items: [
                //        { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
                //        { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
                //    ]
                //},
                //{
                //    text: "Branch",
                //    type: "controls",
                //    cls: "span6",
                //    items: [
                //        { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                //        { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                //    ]
                //},
                //{ name: 'CompanyCode', text: 'Dealer', type: 'select', cls: 'span6', readonly: true },
                //{ name: 'BranchCode', text: 'Outlet', type: 'select', cls: 'span6', readonly: true },
                { name: 'InputFrom', text: 'Date From', type: 'datepicker', cls: 'span3', required: true },
                { name: 'InputTo', text: 'Date To', type: 'datepicker', cls: 'span3', required: true },
                { name: 'Check', text: 'Check', type: 'textarea', cls: 'span6' },
                { name: 'Action', text: 'Action', type: 'textarea', cls: 'span6' },
                { name: 'PIC', text: 'PIC', type: 'select', cls: 'span6' },
                { name: 'CommentbyGM', text: 'Comment by GM', type: 'textarea', cls: 'span6' },
                { name: 'CommentbySIS', text: 'Comment by SIS', type: 'textarea', cls: 'span6', readonly: true },
                { name: 'EmployeeID', cls: 'hide' },
            ]
        }
    ],
    toolbars: [
        { name: 'refresh', text: 'Refresh Activity', icon: 'fa fa-refresh' },
        { name: "btnClear", text: "New", icon: "icon-file", cls: "hide" },
        { name: "btnBrowse", text: "Browse", icon: "icon-search" },
        { name: "btnSave", text: "Save", icon: "icon-save", cls: "show" },
        { name: "btnDelete", text: "Delete", icon: "icon-trash", cls: "hide" },
    ],
});

    //var widget = new SimDms.Widget(options);
var signonjs = 0, branchonjs = "";
widget.lookup.onDblClick(function (e, data, name) {
    widget.lookup.hide();
    //console.log(name);
    //console.log(data);

    widget.populate($.extend({}, widget.DefaultData, data));
    widget.showToolbars(["btnClear", "btnDelete"]);
});

widget.render(function () {
    widget.select({ selector: "#Plan", url: "cs.api/Combo/ReviewPlans" });
    widget.select({ name: "PIC", url: "ab.api/Combo/Employees/" });

    widget.post("cs.api/review/default", function (result) {
        widget.default = result;
        widget.populate(widget.default);
        signonjs = result.sign;
        if (signonjs == 1) {
            $('#BranchCode, input[name=InputFrom], input[name=InputTo], #Check, #Action, #PIC').attr('disabled', true);
            widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
            $('#BranchCode').removeAttr('disabled');
        }
        else {
            $('#CommentbyGM').attr('readonly', 'readonly');
            widget.post("cs.api/Combo/CurrentBranchCode", function (result) {
                branchonjs = result[0].value;
                $('#BranchCode option:eq(0)').val(branchonjs);
                $('#BranchCode option:eq(0)').text(result[0].text);
                $('#BranchCode').val(branchonjs);
                $('#BranchCode').attr('disabled', 'disabled');
            });
        }
    });

    var initial = { StartMonths: [], EndMonths: [] };
    widget.select({ name: "PeriodYear", url: "cs.api/Combo/ListOfYear" });
    for (var i = 1; i <= 12; i++) {
        initial.StartMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
        initial.EndMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
    }
    widget.select({ name: 'ParMonth1', data: initial.StartMonths });
    widget.select({ name: 'ParMonth2', data: initial.EndMonths });

    $('.flt, .flt2').addClass('hide');
});

//$('#reviewpanel').html('<script src="~/assets/js/app/cs/monitoring8" type="text/javascript"><script>');

$('#refresh').on('click', function () {
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
});

$('#Plan').on('change', function () {
    PlanChanged();
});

function PlanChanged() {
    if ($('#Plan').val() == '') {
        $('.flt, .flt2').addClass('hide');
    }
    else if ($('#Plan').val() == 'BIRTHDAY CALL') {
        $('.flt').addClass('hide');
        $('.flt2').removeClass('hide');
    }
    else {
        $('.flt2').addClass('hide');
        $('.flt').removeClass('hide');
    }
}

$('#btnBrowse').on('click', function () {
    var data = $(".main form").serializeObject();
    var id = this.id;
    var lookup = widget.klookup({
        name: "LkuReviews",
        title: "Review List",
        url: "cs.api/lookup/CsReviews",
        params: data,
        sort: ({ field: "OutletAbbreviation", dir: "asc" }, { field: "DateFrom", dir: "desc" }),
        serverBinding: true,
        filters: [
            {
                text: "Filter",
                type: "controls",
                left: 80,
                items: [
                    { name: "fltFrom", text: "Date From", cls: "span2", type: "datepicker" },
                    { name: "fltTo", text: "Date To", cls: "span2", type: "datepicker" },
                ]
            }
        ],
        columns: [
            { field: "BranchCode", title: "OutletCode", hide: true },
            { field: "OutletAbbreviation", title: "Outlet", width: "200px" },
            { field: "DateFrom", title: "Date From", width: "110px", template: "#= (DateFrom == undefined) ? '' : moment(DateFrom).format('DD MMM YYYY') #" },
            { field: "DateTo", title: "Date To", width: "110px", template: "#= (DateTo == undefined) ? '' : moment(DateTo).format('DD MMM YYYY') #" },
            { field: "Plan", title: "Plan", width: "150px" },
            { field: "Do", title: "Do", width: "200px" },
            { field: "Check", title: "Check", width: "300px" },
            { field: "Action", title: "Action", width: "300px" },
            { field: "PIC", title: "PIC" },
            { field: "CommentbyGM", title: "Comment by GM", width: "300px" },
            { field: "CommentbySIS", title: "Comment by SIS", width: "300px" },
            { field: "EmployeeID", hide: true }
        ],
    });
    lookup.dblClick(refreshData);
        
});

$('#btnDelete').on('click', function () {
    var data = $(".main form").serializeObject();
    if (confirm("Anda yakin akan menghapus data ini?")) {
        widget.post("cs.api/review/delete", data, function (result) {
            if (result.success) {
                $("#btnClear").click();
            }
        });
    };
});

$("#btnSave").on("click", function () {
    var valid = $(".main form").valid();
    if (valid) {
        var data = $(".main form").serializeObject();
        widget.post("cs.api/review/save", data, function (result) {
            if (result.success) {
                $("#btnClear").click();
                $("#btnClear").hide();
            }
            else {
                widget.showNotification(result.message);
            }
        });
    }
});
    
$("#btnClear").on("click", function () {
    $(".toolbar > button").hide();
    $("#btnBrowse, #btnSave").show();
    $("input[type=text]").val("");
    $('textarea').val("");
    $('#Plan, #PIC, #BranchCode').val("");
    if (signonjs == 0) {
        $('#BranchCode').val(branchonjs);
    }
});

function refreshData(data) {
    data.InputFrom = data.DateFrom;
    data.InputTo = data.DateTo;
        
    widget.populate(data);
    $("#Plan").val(data["Plan"].toUpperCase() || "");
    PlanChanged();
    $('#refresh').click();
    widget.showToolbars(["refresh", "btnBrowse", "btnSave", 'btnDelete', "btnClear"]);
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
