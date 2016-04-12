$(document).ready(function () {
    var options = {
        title: "Demographic Condition",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Dealer / Outlet",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "Dealer", cls: "span4", type: "select", opt_text: "-- SELECT ALL -- " },
                            { name: "BranchCode", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Position",
                        type: "controls",
                        items: [
                            //{ name: "Department", text: "Department", cls: "span2", type: "select", opt_text: "-- SELECT ALL -- " },
                            { name: "Position", cls: "span3", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Cut off",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'DateFrom', text: 'Date From', type: 'datepicker', cls: 'span2' },
                        ]
                    },
                    {
                        text: "Category",
                        type: "controls",
                        items: [
                            {
                                name: "Category", cls: "span3", type: "select", opt_text: "-- SELECT ALL --",
                                items: [
                                    { text: "Working Period", value: "1" },
                                    { text: "Age", value: "2" },
                                    { text: "Educational Background", value: "3" },
                                    { text: "Gender", value: "4" },
                                ]
                            }
                        ]
                    },
                ],
            },
            {
                type: 'dashboard',
                cls: 'panel',
                items: [
                    { name: "Graphic", xtype: "k-chart", cls: "span7" }
                ]
            }
        ],
        toolbars: [
            { text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
            { text: "Report Preview", action: 'export', icon: "fa fa-file-excel-o" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'refresh':
                    refreshChart();
                    break;
                case 'export':
                    ExportXls();
                    break;
            }
        }
    }
    var widget = new SimDms.Widget(options);
    var piedata = [];

    widget.setSelect([
        { name: "CompanyCode", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp" }, optionalText: "-- SELECT ALL --" },
        { name: "Position", url: "wh.api/combo/positions", params: { dept: "SALES" }, optionalText: "-- SELECT ALL --" }
    ]);
    widget.default = {
        Status: "1",
        DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01'))
    };

    widget.render(function () {
        widget.populate(widget.default);
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/OutletAbbre", params: { id: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });
        // $("select[name=Position],select[name=Status]").on("change", refreshGrid);

    });

    function refreshChart() {
        piedata = [];
        params = widget.serializeObject('pnlFilter');

        $(".page .ajax-loader").fadeIn();
        $('#Graphic').html('');
        widget.pieChart({
            selector: "#Graphic",
            legendPosition: "right",
            title: ($('#Category option:selected').text() == '-- SELECT ALL --' ? 'PLEASE SELECT CATEGORY' : $('#Category option:selected').text()),
            data: piedata,
            tooltip: "#= value # orang"
        });

        if (params.Category == 1) CalculateWorkPeriod(params);
        if (params.Category == 2) CalculateAge(params);
        if (params.Category == 3) CalculateEducation(params);
        if (params.Category == 4) CalculateGender(params);
        else { $(".page .ajax-loader").fadeOut(); }
    }

    function CalculateWorkPeriod(params) {
        //params.Range1 = 0;
        //params.Range2 = 3;
        $.post("wh.api/Demographic/CalculateWorkPeriod", params, function (result) {
            $(".page .ajax-loader").fadeOut();
            piedata.push({ category: "< 3 bulan", value: result.data[0].value1 });
            piedata.push({ category: "3 - 6 bulan", value: result.data[1].value1 });
            piedata.push({ category: "6 - 12 bulan", value: result.data[2].value1 });
            piedata.push({ category: "12 - 24 bulan", value: result.data[3].value1 });
            piedata.push({ category: "24 - 48 bulan", value: result.data[4].value1 });
            piedata.push({ category: "48 - 60 bulan", value: result.data[5].value1 });
            piedata.push({ category: "> 60 bulan", value: result.data[6].value1 });
            $('#Graphic').data('kendoChart').refresh();
        });

        //params.Range1 = 3;
        //params.Range2 = 6;
        //$.post("wh.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "3 - 6 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});

        //params.Range1 = 6;
        //params.Range2 = 12;
        //$.post("wh.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "6 - 12 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});

        //params.Range1 = 12;
        //params.Range2 = 24;
        //$.post("wh.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "12 - 24 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});

        //params.Range1 = 24;
        //params.Range2 = 48;
        //$.post("wh.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "24 - 48 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});

        //params.Range1 = 48;
        //params.Range2 = 60;
        //$.post("wh.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "48 - 60 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});
    }
    
    function CalculateAge(params) {
        //params.Range1 = 0;
        //params.Range2 = 3;
        $.post("wh.api/Demographic/CalculateAge", params, function (result) {
            $(".page .ajax-loader").fadeOut();
            piedata.push({ category: "< 17 tahun", value: result.data[0].value1 });
            piedata.push({ category: "17 - 25 tahun", value: result.data[1].value1 });
            piedata.push({ category: "25 - 33 tahun", value: result.data[2].value1 });
            piedata.push({ category: "33 - 41 tahun", value: result.data[3].value1 });
            piedata.push({ category: "41 - 48 tahun", value: result.data[4].value1 });
            piedata.push({ category: "> 48 tahun", value: result.data[5].value1 });
            $('#Graphic').data('kendoChart').refresh();
        });
    }

    function CalculateEducation(params) {
        //params.Range1 = 0;
        //params.Range2 = 3;
        $.post("wh.api/Demographic/CalculateEducation", params, function (result) {
            $(".page .ajax-loader").fadeOut();
            piedata.push({ category: "SLTP", value: result.data[0].value1 });
            piedata.push({ category: "SLTA / SMK", value: result.data[1].value1 });
            piedata.push({ category: "Diploma 1 / Diploma 2 / Diploma 3", value: result.data[2].value1 });
            piedata.push({ category: "Sarjana (S1)", value: result.data[3].value1 });
            piedata.push({ category: "Magister (S2)", value: result.data[4].value1 });
            piedata.push({ category: "Doktor (S3)", value: result.data[5].value1 });
            $('#Graphic').data('kendoChart').refresh();
        });
    }

    function CalculateGender(params) {
        //params.Range1 = 0;
        //params.Range2 = 3;
        $.post("wh.api/Demographic/CalculateGender", params, function (result) {
            $(".page .ajax-loader").fadeOut();
            piedata.push({ category: "Male", value: result.data[0].value1 });
            piedata.push({ category: "Female", value: result.data[0].value2 });
            $('#Graphic').data('kendoChart').refresh();
        });
    }

    function ExportXls() {
        var data   = widget.serializeObject('pnlFilter');
        var params = {
            ReportId: "demographic.trdx",
            Parameters: "{CompanyCode:'" + data.CompanyCode + "', BranchCode:'" + data.BranchCode + "', Position:'" + data.Position + "', PeriodFrom:'" + data.DateFrom + "', Category:'" + data.Category + "'}"
        }

        SimDms.openReport(params, "Demographic Condition")
    }

});
