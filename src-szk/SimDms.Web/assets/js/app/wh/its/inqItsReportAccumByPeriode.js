var cabang;
var Company;
var Outlet;

$(document).ready(function () {
    var options = {
        title: "Generate ITS - Report Accum By Periode",
        xtype: "panels",
        toolbars: [
            { name: "btnExportXls", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
            //{ name: "btnRefresh", text: "Refresh", cls: "small" },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                     {
                         text: "Area",
                         type: "controls",
                         cls: "span4 full",
                         items: [
                             { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                         ]
                     },
                     {
                         text: "Dealer",
                         type: "controls",
                         cls: "span6 full",
                         items: [
                             { name: "CompanyCode", text: "Dealer Name", cls: "span6", type: "select", opt_text: "-- SELECT ALL -- " },
                         ]
                     },
                     {
                         text: "Period Accum",
                         type: "controls",
                         cls: "span8 full",
                         items: [
                             { type: "label", text: "Periode ke 1", cls: "span3" },
                             { type: "label", text: "Periode ke 2 (Periode ke 1 - 1)", cls: "span3" },
                             { type: "label", text: "Periode ke 3 (Periode ke 2 - 1)", cls: "span2" },
                         ]
                     },
                     {
                         text: "",
                         type: "controls",
                         cls: "span8 full",
                         items: [
                             { name: "AccumFrom1", cls: "span3", type: "multidates" },
                             { name: "AccumFrom2", cls: "span3", type: "multidates" },
                             { name: "AccumFrom3", cls: "span2", type: "multidates" },
                         ]
                     },
                ],
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);

    widget.render(function () {
        
        $('#AccumFrom1').multiDatesPicker({
            dateFormat: "yymmdd",
            changeMonth: true,
            changeYear: true,
            onChangeMonthYear: function (month) {
                $('#AccumFrom1').multiDatesPicker('resetDates');
            }
        });
        $('#AccumFrom1').multiDatesPicker('removeDates', new Date());
        $('#AccumFrom2').multiDatesPicker({
            dateFormat: "yymmdd",
            changeMonth: true,
            changeYear: true,
            onChangeMonthYear: function (month) {
                $('#AccumFrom2').multiDatesPicker('resetDates');
            }
        });
        $('#AccumFrom3').multiDatesPicker({
            dateFormat: "yymmdd",
            changeMonth: true,
            changeYear: true,
            onChangeMonthYear: function (month) {
                $('#AccumFrom3').multiDatesPicker('resetDates');
            }
        });
        $("[name=GroupArea]").on("change", function () {
            //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/Branches", params: { id: $("[name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
        });

        $("#btnExportXls").on("click", exportXls);
        //$("#btnRefresh").on("click", Refresh);
    });

    function exportXls() {
        var data = $("#pnlFilter").serializeObject();
        console.log(moment(data.AccumFrom1).format('DD MMM YY'));

        var date1 = $('#AccumFrom1').multiDatesPicker('value');
        var date2 = $('#AccumFrom2').multiDatesPicker('value');
        var date3 = $('#AccumFrom3').multiDatesPicker('value');


        if ($('#AccumFrom1').multiDatesPicker('value') == '') {
            sdms.info('Periode ke 1 harus di isi', "Informasi");
            return;
        }
        if ($('#AccumFrom2').multiDatesPicker('value') == '') {
            sdms.info('Periode ke 2 harus di isi', "Informasi");
            return;
        }
        if ($('#AccumFrom3').multiDatesPicker('value') == '') {
            sdms.info('Periode ke 3 harus di isi', "Informasi");
            return;
        }

        var dt1 = date1.substring(0, 8);
        var year1 = dt1.substring(0, 4);
        var month1 = dt1.substring(4, 6);
        var day1 = dt1.substring(7, 8);
        var p1 = new Date(year1, month1 - 1, day1);
        var m1 = new Date(year1, month1 - 2, day1);
        var m2 = new Date(year1, month1 - 3, day1);

        var dt2 = date2.substring(0, 8);
        var year2 = dt2.substring(0, 4);
        var month2 = dt2.substring(4, 6);
        var day2 = dt2.substring(7, 8);
        var p2 = new Date(year2, month2 - 1, day2);

        var dt3 = date3.substring(0, 8);
        var year3 = dt3.substring(0, 4);
        var month3 = dt3.substring(4, 6);
        var day3 = dt3.substring(7, 8);
        var p3 = new Date(year3, month3 - 1, day3);

        var period1 = moment(p1).format('MMM YY');
        var period2 = moment(p2).format('MMM YY');
        var period3 = moment(p3).format('MMM YY');
        var Month1 = moment(m1).format('MMM YY');
        var Month2 = moment(m2).format('MMM YY');

        console.log(period1, period2, period3);

        if (period2 != Month1) {
            sdms.info('Periode ke 2 harus (Periode 1) - 1 ', "Informasi");
            return;
        }

        if (period3 != Month2) {
            sdms.info('Periode ke 3 harus (Periode 2) - 1 ', "Informasi");
            return;
        }

        console.log($('#AccumFrom1').multiDatesPicker('value'))
        var url = "wh.api/inquiryprod/GenerateITSReportAccumByPeriode?";
        url += "&GroupArea=" + data.GroupArea;
        url += "&CompanyCode=" + data.CompanyCode;
        url += "&DateAccum1=" + $('#AccumFrom1').multiDatesPicker('value');
        url += "&DateAccum2=" + $('#AccumFrom2').multiDatesPicker('value');
        url += "&DateAccum3=" + $('#AccumFrom3').multiDatesPicker('value');
        window.location = url;
    }
});