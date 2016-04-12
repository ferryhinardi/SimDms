"use strict";
var widget, tableUnitRevenue;
$(document).ready(function () {
    var options = {
        title: "Unit & Revenue Performance",
        xtype: "panels",
        panels: [
            {
                name: 'pnlFilter',
                items: [
                    { name: 'PeriodYear', text: 'Year', type: 'select', cls: 'width33', opt_text: '-- YEAR --' },
                    { name: 'StartMonth', text: 'From', type: 'select', cls: 'width33', opt_text: '-- MONTH --' },
                    {
                        name: "DashType", id: "DashType", text: "Dash Type", cls: "width33", type: "radiobutton", items: [
                        { id: 'inv', value: 'Invoice', label: 'Invoice', checked: true },
                        { id: 'spk', value: 'SPK', label: 'SPK' }
                        ]
                    },
                    { name: 'Sort', text: 'Sort', type: 'select', cls: 'width33', opt_text: 'Descending' },
                    { name: 'EndMonth', text: 'To', type: 'select', cls: 'width33', opt_text: '-- MONTH --' },
                    {
                        name: "BodyRepair", id: "BodyRepair", text: "Body Repair", cls: "width33", type: "radiobutton", items: [
                        { id: 'incl', value: 'incl', label: 'Include', checked: true },
                        { id: 'excl', value: 'excl', label: 'Exclude' }
                        ]
                    },
                ],
            },
            {
                type: 'dashboard',
                cls: 'panel',
                items: [
                    { name: 'pnlTopFive', cls: 'width20' },
                    { name: 'pnlChart', cls: 'width60' },
                    { name: 'pnlKPI', cls: 'width20' },
                ]
            },
        ],
        toolbars: [
            { text: 'Reset', action: 'reset', icon: 'fa fa-repeat' },
            { text: 'Submit', action: 'submit', icon: 'fa fa-save' },
            { text: 'Generate', action: 'generate', icon: 'fa fa-download', additional: "disabled=disabled" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'reset':
                    window.location.reload();
                    break;
                case 'submit':
                    if ($('[name=StartMonth]').val() > $('[name=EndMonth]').val())
                        return false;
                    refreshTopFive();
                    refreshChart();
                    refreshKPI();
                    $('#pnlTopFive, #pnlKPI').css({ "padding": "10px", "background-color": "#CCCCCC" });
                    break;
                case 'generate':
                    break;
            }
        }
    }

    widget = new SimDms.Widget(options);
    widget.render(function () {
        var initial = { Month: moment().format('M'), years: [], StartMonths: [], EndMonths: [] };
        widget.setSelect([{ name: "PeriodYear", url: "wh.api/Combo/ListOfYear10", optionalText: "-- YEAR --" }]);
        for (var i = 1; i <= 12; i++) {
            initial.StartMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
            initial.EndMonths.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
        }
        widget.bind({ name: 'StartMonth', text: '-- MONTH --', data: initial.StartMonths });
        widget.bind({ name: 'EndMonth', text: '-- MONTH --', data: initial.EndMonths });

        widget.bind({ name: 'Sort', text: '-- SORT --', data: [{ value: 'asc', text: 'Ascending' }, { value: 'desc', text: 'Descending' }] });

        $('#pnlChart').append('<div id="chartUnitRevenue" style="margin-top: 0; padding-left: 10px;"></div>');
        $('#pnlChart').append('<div id="tableUnitRevenue" style="margin-top: 0; padding-left: 10px;"></div>');
        $('div.gl-widget .panel').css('max-width', '100%');

        $('#pnlFilter > div:eq(2) > div > div > label').css('padding-top', '6px');
        $('#pnlFilter > div:eq(2)').css('height','40.625px');
    });
});

function refreshTopFive() {
    $('#pnlTopFive').empty();
    TopFive();
    var filter = widget.serializeObject('pnlFilter');
    var option = {}, data = {};
    $.ajax({
        url: 'wh.api/URDashboard/ReloadUnitTopFive',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear, PeriodMonthF: filter.StartMonth, PeriodMonthT: filter.EndMonth, Sort: filter.Sort, DashType: filter.DashType, TargetFlag: 'U', BodyRepair: filter.BodyRepair },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            if (response.message == "Success") {
                successResult("UnitIntake", response.data, ["2%", "78%", "20%"]);
            }
            else {
                sdms.info(response.data, response.message);
            }
        }
    });
    $.ajax({
        url: 'wh.api/URDashboard/ReloadRevenueTopFive',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear, PeriodMonthF: filter.StartMonth, PeriodMonthT: filter.EndMonth, Sort: filter.Sort, DashType: filter.DashType, BodyRepair: filter.BodyRepair },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            if (response.message == "Success") {
                successResult("Revenue", response.data, ["2%", "58%", "40%"]);
            }
            else {
                sdms.info(response.data, response.message);
            }
        }
    });
    $.ajax({
        url: 'wh.api/CSIScore/ReloadCSIScoreTopFive',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear, PeriodMonthF: filter.StartMonth, PeriodMonthT: filter.EndMonth, Sort: filter.Sort },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            if (response.message == "Success") {
                successResult("CSIScore", response.data, ["2%", "73%", "25%"]);
            }
            else {
                sdms.info(response.data, response.message);
            }
        }
    });
    
    function successResult(name, data, colwidth) {
        option.panelbar = 'panelbar';
        option.name = name;
        option.items = data;
        option.colWidth = colwidth;
        widget.panelBarItem(option);
    }
}

function refreshChart() {
    var filter = widget.serializeObject('pnlFilter');
    var month = [], startMonth = parseInt(filter.StartMonth), endMonth = parseInt(filter.EndMonth);
    var start = startMonth, end = endMonth;
    while (start <= end)
    {
        month.push(moment(start.toString(), 'M').format('MMM'));
        start++;
    }
    $.post('wh.api/URDashboard/ChartUnitRevenue', filter, function (response) {
        var series = [], tempURMonth = [], dataTable, option = {};
        if (response.success)
        {
            dataTable = response.data;
            for (var i = 0; i < dataTable.length; i++) {
                var data = [];
                tempURMonth[i] = [];
                tempURMonth[i].push(dataTable[i].Jan);
                tempURMonth[i].push(dataTable[i].Feb);
                tempURMonth[i].push(dataTable[i].Mar);
                tempURMonth[i].push(dataTable[i].Apr);
                tempURMonth[i].push(dataTable[i].May);
                tempURMonth[i].push(dataTable[i].Jun);
                tempURMonth[i].push(dataTable[i].Jul);
                tempURMonth[i].push(dataTable[i].Aug);
                tempURMonth[i].push(dataTable[i].Sep);
                tempURMonth[i].push(dataTable[i].Oct);
                tempURMonth[i].push(dataTable[i].Nov);
                tempURMonth[i].push(dataTable[i].Des);
                for (var j = startMonth; j <= endMonth; j++) {
                    data.push(tempURMonth[i][j - 1]);
                }
                series.push({ name: dataTable[i].TargetFlag, data: data, axis: dataTable[i].Type });
            }
        }

        option.selector = "#chartUnitRevenue";
        option.title = "Unit & Revenue Performance " + filter.PeriodYear;
        option.valueAxes = [
            { name: "U", title: { text: "Unit" }, color: "#007eff", labels: { format: "{0}", template: "#= FormatLongNumber(value) #" }, line: { visible: false } },
            { name: "R", title: { text: "Revenue" }, color: "#73c100", labels: { format: "{0}", template: "#= FormatLongNumber(value) #" }, line: { visible: false } }
        ];
        /*
        option.valueAxes = [
            { name: "U", pane: "unit-pane", color: "#007eff", labels: { format: "{0}", template: "#= FormatLongNumber(value) #" }, line: { visible: false } },
            { name: "R", pane: "revenue-pane", color: "#73c100", labels: { format: "{0}", template: "#= FormatLongNumber(value) #" }, line: { visible: false } }
        ];
        option.panes = [
            { name: "unit-pane", title: { text: "Unit" } },
            { name: "revenue-pane", title: { text: "Revenue" } }
        ];
        */
        option.series = series;
        option.axis = month;
        widget.lineChartMultiAxis(option);

        QTable();
    });
}

function refreshKPI() {
    var filter = widget.serializeObject('pnlFilter');
    var optionUnitIntake = {}, optionServiceCC = {}, optionPassanger = {}, optionCommercial = {};
    $('#pnlKPI').empty();
    $('#pnlKPI').append('<h2 style="text-align:center">Key Indicators</h2>');
    $('#pnlKPI').append('<div id="DivUnitIntake" style="margin-top: 0; padding-left: 0;"></div>');
    $('#pnlKPI').append('<div id="DivServiceCC" style="margin-top: 0; padding-left: 0;"></div>');
    $('#pnlKPI').append('<div id="DivPassanger" style="margin-top: 0; padding-left: 0;"></div>');
    $('#pnlKPI').append('<div id="DivCommercial" style="margin-top: 0; padding-left: 0;"></div>');

    $.ajax({
        url: 'wh.api/URDashboard/ReloadKeyIndicatorUnitIntake',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear, PeriodMonthF: filter.StartMonth, PeriodMonthT: filter.EndMonth },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            if (response.data.length) {
                optionUnitIntake.pointervalue = response.data[0];
                if (response.data[0] > 180) optionUnitIntake.max = (response.data[0]).toFixed();
            }
            else
                optionUnitIntake.pointervalue = 0;
        }
    }).done(function (data, textStatus, jqXHR) {
        optionUnitIntake.labelposition = "Unit Intakes Achv (%)";
        optionUnitIntake.selector = "#DivUnitIntake";
        widget.radialGauge(optionUnitIntake);

        $('#DivUnitIntake svg:first').before('<h4 style="margin-bottom: 0; background-color: #A9B089; opacity: 0.85; text-align: center; line-height: 2;">Unit Intakes Achv (%)</h4>');
        $('svg').css('background-color', '#FFF');
    });

    $.ajax({
        url: 'wh.api/URDashboard/ReloadKeyIndicatorServiceCoverageCombine',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear, PeriodMonthF: filter.StartMonth, PeriodMonthT: filter.EndMonth },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            if (response.data.length) {
                optionServiceCC.pointervalue = response.data[0];
                if (response.data[0] > 180) optionServiceCC.max = (response.data[0]).toFixed();
            }
            else
                optionServiceCC.pointervalue = 0;
        }
    }).done(function (data, textStatus, jqXHR) {
        optionServiceCC.labelposition = "Service Coverage Combine";
        optionServiceCC.selector = "#DivServiceCC";
        widget.radialGauge(optionServiceCC);

        $('#DivServiceCC svg:first').before('<h4 style="margin-bottom: 0; background-color: #A9B089; opacity: 0.85; text-align: center; line-height: 2;">Service Coverage Combine</h4>');
        $('svg').css('background-color', '#FFF');
    });

    $.ajax({
        url: 'wh.api/URDashboard/ReloadKeyIndicatorPassanger',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear, PeriodMonthF: filter.StartMonth, PeriodMonthT: filter.EndMonth },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            if (response.data.length) {
                optionPassanger.pointervalue = response.data[0];
                if (response.data[0] > 180) optionPassanger.max = (response.data[0]).toFixed();
            }
            else
                optionPassanger.pointervalue = 0;
        }
    }).done(function (data, textStatus, jqXHR) {
        optionPassanger.labelposition = "Passanger";
        optionPassanger.selector = "#DivPassanger";
        widget.radialGauge(optionPassanger);

        $('#DivPassanger svg:first').before('<h4 style="margin-bottom: 0; background-color: #A9B089; opacity: 0.85; text-align: center; line-height: 2;">Passanger</h4>');
        $('svg').css('background-color', '#FFF');
    });

    $.ajax({
        url: 'wh.api/URDashboard/ReloadKeyIndicatorCommercial',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear, PeriodMonthF: filter.StartMonth, PeriodMonthT: filter.EndMonth },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            if (response.data.length) {
                optionUnitIntake.pointervalue = response.data[0];
                if (response.data[0] > 180) optionUnitIntake.max = (response.data[0]).toFixed();
            }
            else
                optionUnitIntake.pointervalue = 0;
        }
    }).done(function (data, textStatus, jqXHR) {
        optionCommercial.labelposition = "Commercial";
        optionCommercial.selector = "#DivCommercial";
        widget.radialGauge(optionCommercial);

        $('#DivCommercial svg:first').before('<h4 style="margin-bottom: 0; background-color: #A9B089; opacity: 0.85; text-align: center; line-height: 2;">Commercial</h4>');
        $('svg').css('background-color', '#FFF');
    });
}

function checkMonth(value, monthId)
{
    var filter = widget.serializeObject('pnlFilter');
    if (monthId >= filter.StartMonth && monthId <= filter.EndMonth)
        return value;
    else
        return 0;
}

function jsonToRow(returnObj, startRow) {
    var data = []
    for (var i = 0; i < returnObj.length; i++)
    {
        data.push([startRow, 1, checkMonth((typeof returnObj[i].Jan === 'undefined') ? 0 : returnObj[i].Jan, 1)]);
        data.push([startRow, 2, checkMonth((typeof returnObj[i].Feb === 'undefined') ? 0 : returnObj[i].Feb, 2)]);
        data.push([startRow, 3, checkMonth((typeof returnObj[i].Mar === 'undefined') ? 0 : returnObj[i].Mar, 3)]);
        data.push([startRow, 4, checkMonth((typeof returnObj[i].Apr === 'undefined') ? 0 : returnObj[i].Apr, 4)]);
        data.push([startRow, 5, checkMonth((typeof returnObj[i].May === 'undefined') ? 0 : returnObj[i].May, 5)]);
        data.push([startRow, 6, checkMonth((typeof returnObj[i].Jun === 'undefined') ? 0 : returnObj[i].Jun, 6)]);
        data.push([startRow + 6, 1, checkMonth((typeof returnObj[i].Jul === 'undefined') ? 0 : returnObj[i].Jul, 7)]);
        data.push([startRow + 6, 2, checkMonth((typeof returnObj[i].Aug === 'undefined') ? 0 : returnObj[i].Aug, 8)]);
        data.push([startRow + 6, 3, checkMonth((typeof returnObj[i].Sep === 'undefined') ? 0 : returnObj[i].Sep, 9)]);
        data.push([startRow + 6, 4, checkMonth((typeof returnObj[i].Oct === 'undefined') ? 0 : returnObj[i].Oct, 10)]);
        data.push([startRow + 6, 5, checkMonth((typeof returnObj[i].Nov === 'undefined') ? 0 : returnObj[i].Nov, 11)]);
        data.push([startRow + 6, 6, checkMonth((typeof returnObj[i].Des === 'undefined') ? 0 : returnObj[i].Des, 12)]);
    }
    return data;
}

function QTable() {
    var filter = widget.serializeObject('pnlFilter');
    $('#tableUnitRevenue').empty();
    var ColorRenderer = function (instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.backgroundColor = '#ECECEC';
        td.style.color = '#000';
    };

    $.ajax({
        url: 'wh.api/URDashboard/GetUnitIntakeNational',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear, PeriodMonthF: filter.StartMonth, PeriodMonthT: filter.EndMonth },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            var dataTableUnit = jsonToRow(response.data, 2);
            tableUnitRevenue.setDataAtCell(dataTableUnit, null, null, 'loadData');
        }
    }).done(function (data, textStatus, jqXHR) {
    });
    $.ajax({
        url: 'wh.api/URDashboard/GetRevenueNational',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear, PeriodMonthF: filter.StartMonth, PeriodMonthT: filter.EndMonth },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            var dataTableRevenue = jsonToRow(response.data, 4);
            tableUnitRevenue.setDataAtCell(dataTableRevenue, null, null, 'loadData');
        }
    }).done(function (data, textStatus, jqXHR) {
    });
    $.ajax({
        url: 'wh.api/URDashboard/GetTargetNational',
        type: "POST",
        data: { PeriodYear: filter.PeriodYear },
        dataType: 'JSON',
        async: true,
        success: function (response) {
            var dataTableUnitTarget = jsonToRow(response.dataUnit, 3);
            var dataTableRevTarget = jsonToRow(response.dataRevenue, 5);
            tableUnitRevenue.setDataAtCell(dataTableUnitTarget, null, null, 'loadData');
            tableUnitRevenue.setDataAtCell(dataTableRevTarget, null, null, 'loadData');
        }
    }).done(function (data, textStatus, jqXHR) {
    });

    var data = [
        ["", "Q1", "", "", "Q2", "", ""],
        ["", "Jan", "Feb", "Mar", "Apr", "May", "Jun"],
        ["Unit", "", "", "", "", "", ""],
        ["Target Unit", "", "", "", "", "", ""],
        ["Revenue", "", "", "", "", "", ""],
        ["Target Rev", "", "", "", "", "", ""],
        ["", "Q3", "", "", "Q4", "", ""],
        ["", "Jul", "Aug", "Sep", "Oct", "Nov", "Des"],
        ["Unit", "", "", "", "", "", ""],
        ["Target Unit", "", "", "", "", "", ""],
        ["Revenue", "", "", "", "", "", ""],
        ["Target Rev", "", "", "", "", "", ""],
    ];
    var container = document.getElementById('tableUnitRevenue'),
        settings = {
            data: data,
            minSpareRows: 1,
            contextMenu: true,
            autoWrapRow: true,
            persistentState: true,
            maxRows: data.length,
            colWidths: [80, 105, 105, 105, 105, 105, 105],
            mergeCells: [
                { row: 0, col: 1, rowspan: 1, colspan: 3 },
                { row: 0, col: 4, rowspan: 1, colspan: 3 },
                { row: 6, col: 1, rowspan: 1, colspan: 3 },
                { row: 6, col: 4, rowspan: 1, colspan: 3 },
            ],
            columns: [
                { readOnly: true },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
            ],
            className: "htCenter",
            cells: function (row, col, prop) {
                var cellProperties = {};
                var readOnlyRow = [ 0, 1, 6, 7 ];
                if ($.inArray(row, readOnlyRow) != -1) {
                    this.renderer = ColorRenderer;
                    cellProperties.readOnly = true;
                }

                return cellProperties;
            },
            afterRender: function (TD, row, col, prop, value, cellProperties) {
                $("#tableUnitRevenue table tr td").css("font-size", ".775em");
            }
        };
    tableUnitRevenue = new Handsontable(container, settings);
    tableUnitRevenue.render();
    $(".wtHolder").css("height", "auto");
}

function TopFive(grid) {
    var data = grid || {};
    var option = {};
    option.name = "panelbar";
    option.selector = "#pnlTopFive";
    option.items = [];
    option.items.push({
        name: "UnitIntake",
        text: "Unit Intake",
        subitems: data.UnitIntake || []
    });
    option.items.push({
        name: "Revenue",
        text: "Revenue",
        colWidth: ["2%", "70%", "28%"],
        subitems: data.Revenue || []
    });
    option.items.push({
        name: "CSIScore",
        text: "CSI Score",
        colWidth: ["5%", "75%", "20%"],
        subitems: data.CSIScore || []
    });

    widget.panelBar(option);
    $('#pnlTopFive').prepend('<h2 style="text-align:center">Top Fives</h2>');
}
