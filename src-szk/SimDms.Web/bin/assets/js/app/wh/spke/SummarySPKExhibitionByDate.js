$(document).ready(function () {
    var options = {
        title: "Report Summary SPK By Date",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o" },
        ],
        items: [
            {
                text: "Period From",
                type: "controls",
                cls: "span6",
                items: [
                    { name: "DateFrom", cls: "span4", type: "datepicker" }
                ]
            },
            {
                text: "Period To",
                type: "controls",
                cls: "span6",
                items: [
                    { name: "DateTo", cls: "span4", type: "datepicker" }
                ]
            },
             {
                text: "Filter By",
                type: "controls",
                cls: "span6",
                items: [
                    { name: "FilterBy", cls: "span4", type: "select", required: true },
                    { name: "cCode", type: "hidden" },
                    { name: "bCode", type: "hidden" }
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.render(function () {
        //$(".frame").css({ top: 240 });
        //widget.post("wh.api/spkexhibition/default", function (result) {
        //    if (result.success) {
        //        widget.default = result.data;
        //        widget.populate(widget.default);
        //    }
        //});
        widget.post("wh.api/inquiryprod/defaultrawdatspk", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);

                widget.select({ selector: "select[name=FilterBy]", url: "wh.api/combo/spkfiltercombo", selected: "1" });
            }
        });

        var btnExportExcel = $('#btnExportExcel');
        btnExportExcel.off();
        btnExportExcel.on('click', function () {
            var dvc = (/android|webos|iphone|ipad|ipod|blackberry|iemobile|operamini/i.test(navigator.userAgent.toLowerCase())) ? "mobile" : "desktop";
            var rptType = "SummarySPKExhibitionByInquiryDate";
            if ($("#FilterBy").val() == "2") {
                rptType = "SummarySPKExhibitionBySpkDate";
            }
            var param = "?DateFrom=" + $('input[name="DateFrom"]').val() + "&DateTo=" + $('input[name="DateTo"]').val() + "&Device=" + dvc;
            var url = SimDms.baseUrl + 'wh.api/spkexhibition/' + rptType + param;
            window.open(url, '_blank')
        });
    });
});