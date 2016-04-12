"use strict";
$(document).ready(function () {
    var options = {
        title: "VOR Report Consistency",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer Name", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet Name", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Year", text: "Year", type: "select", required: true },
                    { name: "Month", text: "Month", type: "select", required: true },
                ],
            },
            {
                name: "DataTable",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { text: "Generate", action: "export", icon: "fa fa-download" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case "export":
                    exportXls();
                    break;
            }
        },
    };

    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "GroupArea", url: "wh.api/combo/SrvGroupAreas", optionalText: "-- SELECT ALL --" },
        { name: "Year", url: "wh.api/VOR/YearVOR" }
    ]);
    widget.render(function () {
        var Month = [];
        for (var i = 1; i <= 12; i++) {
            Month.push({ value: i, text: moment(i.toString(), 'M').format('MMMM') });
        }
        $("[name=GroupArea]").on("change", function () {
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/SrvDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            if ($(this).val() == "")
                widget.select({ selector: "[name=BranchCode]", data: [], optionText: "-- SELECT ALL --" });
            else
                widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/SrvBranchList", params: { area: $("#pnlFilter [name=Area]").val(), comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });
        widget.bind({ name: 'Month', data: Month });
    });

    function exportXls() {
        if ($("[name=Year]").val() == "") {
            sdms.info("Please Choose Year");
            return;
        }
        else if ($("[name=Month]").val() == "") {
            sdms.info("Please Choose Month");
            return;
        }
        else {
            var params = $("#pnlFilter").serializeObject();

            $.ajax({
                type: "POST",
                data: params,
                url: "wh.api/report/VORConsistencyReport",
                success: function (data) {
                    if (data.message == "") {
                        location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=VOR Report Consistency';
                    } else {
                        sdms.info(data.message, "Error");
                    }
                }
            });
        }
    }
});