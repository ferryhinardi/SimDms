var cabang;
var Company;
var Outlet;
var model='';

$(document).ready(function () {
    var options = {
        title: "Generate ITS - Trend SPK Compare 3 Month  Work Day ",
        xtype: "panels",
        toolbars: [
            { name: "btnExportXls", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                     {
                         text: "Period",
                         type: "controls",
                         cls: "span6 full",
                         items: [
                             { id: "StartDate", name: "StartDate", cls: "span3", type: "datepicker" },
                             { type: "label", text: "S/D", cls: "span1" },
                             { id: "EndDate", name: "EndDate", cls: "span3", type: "datepicker" }
                         ]
                     },
                     //{ name: "GroupArea", type: "select", cls: "span4 full ", text: "Area", opt_text: '' },
                     //{ name: "CompanyCode", type: "select", cls: "span4 full", text: "Dealer", opt_text: '' },
                     { name: "Model", type: "select", cls: "span4 full ", text: "Model", opt_text: '' },
                     //{
                     //    text: "Tipe Kendaraan",
                     //    type: "controls",
                     //    cls: "span4 full",
                     //    items: [
                     //        { name: "CarType", text: "CarType", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                     //    ]
                     //},
                ],
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    var dtdealer = [];
    //widget.select({ selector: "[name=GroupArea]", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" });
    //widget.select({ selector: "[name=CarType]", url: "wh.api/combo/TipeKendaraan", optionalText: "-- SELECT ALL --" });

    widget.render(function () {
        var filter = {
            StartDate: new Date(moment(moment().format('YYYY-MM-') + '01')),
            EndDate: new Date(),
        }


        //widget.post('wh.api/Combo/DealerList', function (result) {
        //    dtdealer = result;
        //    $("#CompanyCode").select2('destroy')
        //    $("#CompanyCode option:first").remove();
        //    $("#CompanyCode").attr('multiple', 'multiple');

        //    $("#CompanyCode").select2()
        //});

        //widget.post('wh.api/Combo/GroupAreas', function (result) {
        //    $("#GroupArea").select2('destroy')
        //    $("#GroupArea option:first").remove();
        //    $("#GroupArea").attr('multiple', 'multiple');

        //    $.each(result, function (k, v) {
        //        $('#GroupArea')
        //            .append($("<option></option>")
        //            .attr("value", v.value)
        //            .text(v.text));
        //    });

        //    $("#GroupArea").select2()
        //});

        //$("#GroupArea").on('change', function (x) {

        //    var mdl = $('#GroupArea').val();
        //    $('#CompanyCode option').remove();
        //    $("#CompanyCode").trigger("change");
        //    console.log(mdl)
        //    if (mdl != null) {

        //        var arr = jQuery.grep(dtdealer, function (a) {
        //            return mdl.indexOf(a.GroupNo) != -1;
        //        });


        //        $.each(arr, function (k, v) {
        //            $('#CompanyCode')
        //                .append($("<option></option>")
        //                .attr("value", v.value)
        //                .text(v.text));
        //        });

        //        $('#CompanyCode option').prop('selected', 'selected');
        //        $("#CompanyCode").trigger("change");

        //    }

        //});

        widget.post('wh.api/Combo/GroupModel', function (result) {
            $("#Model").select2('destroy')
            $("#Model option:first").remove();
            $("#Model").attr('multiple', 'multiple');

            $.each(result, function (k, v) {
                $('#Model')
                    .append($("<option></option>")
                    .attr("value", v.value)
                    .text(v.text));
            });

            $("#Model").select2()
        });

        $("#Model").on('change', function (x) {
            try {
                //console.log(x);
                //console.log(x.added.id);
                if (model != '') {
                    model += "," + x.added.id;
                } else {
                    model = x.added.id;
                }
            }catch(e){
                if (x.removed.id != undefined) {
                    var y = model.split(",");
                    var z = y.length;
                    model = '';
                    for (i = 0; i <= z-1; i++) {
                        //console.log(i);
                        //console.log(y[i]);
                        if (y[i] != x.removed.id) {
                            if (model != '') {
                                model += "," + y[i];
                            } else {
                                model = y[i];
                            }
                        }
                    }
                }
            } 

            console.log(model);

        });

        $("#btnExportXls").on("click", exportXls);
    });

    function exportXls() {
        var data = $("#pnlFilter").serializeObject();
        //console.log($("#Model").select2('val'))
        var a = (moment(data.StartDate).format('MM'));
        var b = parseInt(a) + 2;
        //console.log(b);
        //console.log(moment(data.EndDate).format('MM'));
        if (moment(data.EndDate).format('MM') != b) {
            if (moment(data.StartDate).format('YYYY') == moment(data.EndDate).format('YYYY')) {
                alert("Periode End Date harus 3 bulan lebih besar dari Periode Start Date!");
                return;
            }
        }

        var url = "wh.api/inquiryITS/GenerateITSReportSPKWorkDay?";
        url += "&StartDate=" + moment(data.StartDate).format('YYYYMMDD');
        url += "&EndDate=" + moment(data.EndDate).format('YYYYMMDD');
        //url += "&TipeKendaraan=" + $("#Model").select2('val');
        url += "&TipeKendaraan=" + model;
        url += "&StartDateName=" + moment(data.StartDate).format('DD MMM YYYY');
        url += "&EndDateName=" + moment(data.EndDate).format('DD MMM YYYY');
        window.location = url;
    }
});