$(document).ready(function () {
    var options = {
        title: "Daily SPK Report Per Dealer",
        xtype: "panels",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
        ],
        panels: [
            {
                name: "panelFilter",
                items: [
                    { name: "Date", type: "datepicker", text: "Date", cls: "span3 full " },
                    { name: "ShiftCode", text: "Shift", cls: "span3 full", type: "select" },
                    {
                        text: "Filter By",
                        type: "controls",
                        cls: "span6",
                        items: [
                            { name: "FilterBy", cls: "span3", type: "select", required: true },
                            { name: "cCode", type: "hidden" },
                            { name: "bCode", type: "hidden" }
                        ]
                    }
                ]
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {
        initElementEvents();
    }

    function initElementEvents() {
        widget.post("wh.api/spkexhibition/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);

                widget.select({ selector: "select[name=FilterBy]", url: "wh.api/combo/spkfiltercombo", selected: "1" });
            }
        });
        widget.select({ selector: "select[name=ShiftCode]", url: "wh.api/combo/lookups/shift" + "/?CompanyCode=" + $("[name=CompanyCode]").val(), selected: "I" });

        var btnExportExcel = $('#btnExportExcel');
        btnExportExcel.off();
        btnExportExcel.on('click', function () {
            var dvc = (/android|webos|iphone|ipad|ipod|blackberry|iemobile|operamini/i.test(navigator.userAgent.toLowerCase())) ? "mobile" : "desktop";
            if ($("#ShiftCode").val() == "") {
                alert("Please select Shift!");
                $("#ShiftCode").focus();
            }
            else {
                var rptType = "DailySPKReportPerDealerByInquiryDate";
                if ($("#FilterBy").val() == "2") {
                    rptType = "DailySPKReportPerDealerBySPKDate";
                }
                var param = "?Date=" + $('input[name="Date"]').val() + "&ShiftCode=" + $("#ShiftCode").val() + "&Device=" + dvc;
                var url = SimDms.baseUrl + 'wh.api/spkexhibition/' + rptType + param;
                window.open(url, '_blank')
            }
        });

    }
});