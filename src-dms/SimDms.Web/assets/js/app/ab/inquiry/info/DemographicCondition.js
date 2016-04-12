$(document).ready(function () {
    var options = {
        title: "Demographic Condition",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
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
                items: [
                    { name: 'Graphic', type: 'freespace' }
                ]
            }
        ],
        toolbars: [
            { name: "refresh", text: "Refresh", icon: "fa fa-refresh" },
        ],
    }
    var widget = new SimDms.Widget(options);
    var piedata = [];

    widget.setSelect([
        { name: "Position", url: "ab.api/combo/Positions", params: { id: "SALES" }, optionalText: "-- SELECT ALL --" }
    ]);
    widget.default = {
        Status: "1",
        DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01'))
    };

    widget.render(function () {
        widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
        widget.populate(widget.default);

        $("#refresh").on("click", refreshChart);
        // $("select[name=Position],select[name=Status]").on("change", refreshGrid);

    });


    function refreshChart() {
        piedata = [];
        params = widget.serializeObject('pnlFilter');

        $(".page .ajax-loader").fadeIn();
        $('#Graphic').css('margin-top', '0px').html('');
        widget.pieChart({
            selector: "#Graphic",
            legendPosition: "right",
            title: ($('#Category option:selected').text() == '-- SELECT ALL --' ? 'PLEASE SELECT CATEGORY' : $('#Category option:selected').text()),
            data: piedata,
            tooltip: "#= value # orang",
            chartArea: { height: 300 }
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
        $.post("ab.api/Demographic/CalculateWorkPeriod", params, function (result) {
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
        //$.post("ab.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "3 - 6 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});

        //params.Range1 = 6;
        //params.Range2 = 12;
        //$.post("ab.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "6 - 12 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});

        //params.Range1 = 12;
        //params.Range2 = 24;
        //$.post("ab.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "12 - 24 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});

        //params.Range1 = 24;
        //params.Range2 = 48;
        //$.post("ab.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "24 - 48 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});

        //params.Range1 = 48;
        //params.Range2 = 60;
        //$.post("ab.api/Demographic/CalculateWorkPeriod", params, function (result) {
        //    piedata.push({ category: "48 - 60 bulan", value: result.data[0].value1 });
        //    $('#Graphic').data('kendoChart').refresh();
        //});
    }

    function CalculateAge(params) {
        //params.Range1 = 0;
        //params.Range2 = 3;
        $.post("ab.api/Demographic/CalculateAge", params, function (result) {
            $(".page .ajax-loader").fadeOut();
            piedata.push({ category: "< 17 tahun", value: result.data[0].value1 });
            piedata.push({ category: "17 - 25 tahun", value: result.data[1].value1 });
            piedata.push({ category: "25 - 33 tahun", value: result.data[2].value1 });
            piedata.push({ category: "33 - 41 tahun", value: result.data[3].value1 });
            piedata.push({ category: "41 - 48 tahun", value: result.data[4].value1 });
            piedata.push({ category: "> 49 tahun", value: result.data[5].value1 });
            $('#Graphic').data('kendoChart').refresh();
        });
    }

    function CalculateEducation(params) {
        //params.Range1 = 0;
        //params.Range2 = 3;
        $.post("ab.api/Demographic/CalculateEducation", params, function (result) {
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
        $.post("ab.api/Demographic/CalculateGender", params, function (result) {
            $(".page .ajax-loader").fadeOut();
            piedata.push({ category: "Male", value: result.data[0].value1 });
            piedata.push({ category: "Female", value: result.data[0].value2 });
            $('#Graphic').data('kendoChart').refresh();
        });
    }

});
