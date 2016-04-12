$(document).ready(function () {
    var options = {
        title: "Customer Satisfaction Performance",
        xtype: "panels",
        toolbars: [
            //{ name: "btnRefresh", text: "Refresh", cls: "btn btn-info", icon: "fa fa-refresh" },
            { name: "btnSubmit", text: "Submit", cls: "btn btn-info", icon: "fa fa-save" },
            { name: "btnDownload", text: "Generate", cls: "btn btn-info", icon: "fa fa-download", additional: "disabled=disabled" }
        ],
        panels: [
            {
                name: "pnlFilter",
                cls: "full",
                items: [
                    { name: "PeriodYear", id: "PeriodYear", text: "Year", cls: "width33", type: "select" },
                    { name: "PeriodMonthF", id: "PeriodMonthF", text: "Month From", cls: "width33", type: "select" },
                    {
                        name: "DashType", id: "DashType", text: "DashType", cls: "width33", type: "radiobutton", items: [
                        { id: 'inv', value: 'Invoice', label: 'Invoice', checked: true },
                        { id: 'spk', value: 'SPK', label: 'SPK' }
                        ]
                    },
                    { name: "Sort", id: "Sort", text: "Sort", type: "select", cls: "width33", opt_text: "Descending" },
                    { name: "PeriodMonthT", id: "PeriodMonthT", text: "Month To", cls: "width33", type: "select" }
                    //{ name: "DashType", type: 'switch', text: "Is Internal Link", cls: "span2 full ignore-uppercase", required: true },
                ]
            },
            {
                type: 'dashboard',
                cls: 'panel',
                items: [
                    { name: 'pnlTopFive', cls: 'width20' },
                    { name: 'pnlChart', cls: 'width60' },
                    { name: 'pnlKPI', cls: 'width20' },
                ]
            }
        ],
        
    };

    var Wx = new SimDms.Widget(options);
    //var data = {
    //    SIseries: [],
    //    SAseries: [],
    //    SFseries: [],
    //    VPseries: [],
    //    SQseries: []
    //};

    Wx.default = {};
    Wx.render(function (s) {
        Wx.select({ selector: "[name=PeriodYear]", url: "wh.api/combo/years" });
        Wx.select({ selector: "[name=PeriodMonthF]", url: "wh.api/combo/listofmonth" });
        Wx.select({ selector: "[name=PeriodMonthT]", url: "wh.api/combo/listofmonth" });
        Wx.select({
            selector: "[name=Sort]",
            data: [{ text: "Ascending", value: "asc" }, { text: "Descending", value: "desc" }]
        });

        panelInitialization();
    });
    
    $('#btnRefresh').click(function () {
        params = $('#pnlFilter').serializeObject();
        //var data = hotUnitIntake.getData().slice(0);
        //data.splice(0, 1);

    });

    $('#btnSubmit').click(function () {
        params = $('#pnlFilter').serializeObject();

        if (parseInt(params.PeriodMonthF) > parseInt(params.PeriodMonthT)) {
            sdms.info("Bulan awal lebih besar dari bulan akhir", "Error");
            return;
        }
        $.post("wh.api/CSIScore/ReloadScorePerformance", params, function (result) {
            createChart(result);
        });

        $('#pnlTopFive, #pnlKPI').css({ "padding": "10px", "background-color": "#CCCCCC" });
        createTopFive();
        ReloadTopFive(params);
        createQTable();
        createKPI();

        $('#btnDownload').removeAttr('disabled');
    });

    $('#btnDownload').click(function () {
    });

    function panelInitialization() {
        $('#pnlFilter > div:eq(2) > div > div > label').css('padding-top', '6px');
        $('#pnlFilter > div:eq(2)').css('height', '40.625px');
        $('div.gl-widget .panel').css('max-width', '100%');

        $('#pnlChart').append('<div id="wxlinechart" style="margin-top: 0; padding-left: 10px;"></div>');
        $('#pnlChart').append('<div id="tableUnitRevenue" style="margin-top: 0; padding-left: 10px;"></div>');        
    }

    function createChart(chart) {
        var data = chart || {};
        Wx.lineChart({
            selector: "#wxlinechart",
            title: "CS Performance",
            series: [
                {
                    name: "SI",
                    data: data.SIseries
                },
                {
                    name: "SA",
                    data: data.SAseries
                },
                {
                    name: "SF",
                    data: data.SFseries
                },
                {
                    name: "VP",
                    data: data.VPseries
                },
                {
                    name: "SQ",
                    data: data.SQseries
                }
            ],
            axis: ["Jan", "Feb", "Mar", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Des"],
            axisformat: "{0}"
        });
    }

    function createTopFive(grid)  {
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
            subitems: data.Revenue || []
        });
        option.items.push({
            name: "CSIScore",
            text: "CSI Score",
            subitems: data.CSIScore || []
        });

        Wx.panelBar(option);
        $('#pnlTopFive').prepend('<h2 style="text-align:center">Top Fives</h2>');
    }

    function createKPI() {
        var params = Wx.serializeObject('pnlFilter');
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
            data: params,
            dataType: 'JSON',
            async: true,
            success: function (response) {
                if (response.data.length)
                    optionUnitIntake.pointervalue = response.data[0];
                else
                    optionUnitIntake.pointervalue = 0;
            }
        }).done(function (data, textStatus, jqXHR) {
            optionUnitIntake.labelposition = "Unit Intakes Achv (%)";
            optionUnitIntake.selector = "#DivUnitIntake";
            Wx.radialGauge(optionUnitIntake);

            $('#DivUnitIntake svg:first').before('<h4 style="margin-bottom: 0; background-color: #A9B089; opacity: 0.85; text-align: center; line-height: 2;">Unit Intakes Achv (%)</h4>');
            $('svg').css('background-color', '#FFF');
        });

        $.ajax({
            url: 'wh.api/URDashboard/ReloadKeyIndicatorServiceCoverageCombine',
            type: "POST",
            data: params,
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
            Wx.radialGauge(optionServiceCC);

            $('#DivServiceCC svg:first').before('<h4 style="margin-bottom: 0; background-color: #A9B089; opacity: 0.85; text-align: center; line-height: 2;">Service Coverage Combine</h4>');
            $('svg').css('background-color', '#FFF');
        });

        $.ajax({
            url: 'wh.api/URDashboard/ReloadKeyIndicatorPassanger',
            type: "POST",
            data: params,
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
            Wx.radialGauge(optionPassanger);

            $('#DivPassanger svg:first').before('<h4 style="margin-bottom: 0; background-color: #A9B089; opacity: 0.85; text-align: center; line-height: 2;">Passanger</h4>');
            $('svg').css('background-color', '#FFF');
        });

        $.ajax({
            url: 'wh.api/URDashboard/ReloadKeyIndicatorCommercial',
            type: "POST",
            data: params,
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
            Wx.radialGauge(optionCommercial);

            $('#DivCommercial svg:first').before('<h4 style="margin-bottom: 0; background-color: #A9B089; opacity: 0.85; text-align: center; line-height: 2;">Commercial</h4>');
            $('svg').css('background-color', '#FFF');
        });
    }

    function createQTable() {
        var params = Wx.serializeObject('pnlFilter');
        $('#tableUnitRevenue').empty();
        var ColorRenderer = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#ECECEC';
            td.style.color = '#000';
        };

        $.ajax({
            url: 'wh.api/URDashboard/GetUnitIntakeNational',
            type: "POST",
            data: params,
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
            data: params,
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
            data: { PeriodYear: params.PeriodYear },
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
                    var readOnlyRow = [0, 1, 6, 7];
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

    function ReloadTopFive(params) {
        var data = {};
        var option = {};
        option.panelbar = "panelbar";
        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/URDashboard/ReloadUnitTopFive",
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
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/URDashboard/ReloadRevenueTopFive",
            success: function (response) {
                if (response.message == "Success") {
                    successResult("Revenue", response.data, ["2%", "65%", "43%"]);
                }
                else {
                    sdms.info(response.data, response.message);

                }
            }
        });

        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/CSIScore/ReloadCSIScoreTopFive",
            success: function (response) {
                if (response.message == "Success") {
                    successResult("CSIScore", response.data, ["2%", "73%", "25%"]);
                    successResult();
                }
                else {
                    sdms.info(response.data, response.message);

                }
            }
        });

        function successResult(name, data, colwidth) {
            option.name = name;
            option.items = data;
            option.colWidth = colwidth;
            Wx.panelBarItem(option);
            //$(".k-panel > .k-item > .k-link").css("font-size", ".715em");
        }
    }

    function ReloadCallback(result) {

        if (result.data !== null) {
            var i, n = result.data.length;
            var datadump = [];

            for (i = 0; i < n; i++) {
                //datadump.push(me.jsonToRow(response.data[i]));
                var returnObj = result.data[i];
                datadump.push([1 + i, 0, i + 1])
                datadump.push([1 + i, 1, returnObj.AreaDealer])
                datadump.push([1 + i, 2, returnObj.Total])
            }

            hotUnitIntake.alter('remove_row', 1, hotUnitIntake.countRows());
            hotUnitIntake.setDataAtCell(datadump, null, null, 'loadData');

        }
    }

    function checkMonth(value, monthId) {
        var params = Wx.serializeObject('pnlFilter');
        if (monthId >= params.PeriodMonthF && monthId <= params.PeriodMonthT)
            return value;
        else
            return 0;
    }

    function jsonToRow(returnObj, startRow) {
        var data = []
        for (var i = 0; i < returnObj.length; i++) {
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

});
