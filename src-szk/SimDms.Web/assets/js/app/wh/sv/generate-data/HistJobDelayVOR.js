"use strict";
$(document).ready(function () {
    var options = {
        title: "History Job Delay VOR",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "CompanyCode", text: "Dealer Name", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "BranchCode", text: "Outlet Name", type: "select", opt_text: "-- SELECT ALL --" },
                ]
            }
        ],
        toolbars: [
            { text: "Print", action: "export", icon: "fa fa-download" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case "export":
                    exportXls();
                    break;
            }
        }
    };

    var widget = new SimDms.Widget(options);
    widget.setSelect([
        //{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" },
        { name: "GroupArea", url: "wh.api/combo/SrvGroupAreas", optionalText: "-- SELECT ALL --" },
    ]);

    widget.render(function () {
        $('#CompanyCode, #BranchCode').attr('disabled', 'disabled');
        $("[name=GroupArea]").on("change", function () {
            //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/SrvDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });

            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            //widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { area: $("#pnlFilter [name=GroupArea]").val(), comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });

            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });
        $('select').on('change', ResetCombo);
    });

    function ResetCombo() {
        if ($('#GroupArea').val() == "") {
            $('#CompanyCode').val('');
            $('[name="CompanyCode"]').html('<option value="">-- SELECT ALL --</option>');
            $('#CompanyCode').attr('disabled', 'disabled');
            $('#BranchCode').attr('disabled', 'disabled');
        }
        else {
            $('#CompanyCode').removeAttr('disabled', 'disabled');
        }

        if ($('#CompanyCode').val() == "") {
            $('#BranchCode').attr('disabled', 'disabled');
        }
        else {
            $('#BranchCode').removeAttr('disabled', 'disabled');
        }
    }

    function exportXls() {
        var params = $("#pnlFilter").serializeObject();

        $.ajax({
            type: "POST",
            data: params,
            url: "wh.api/report/HistJobDelayVOR",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=History Job Delay Report';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
    }
});