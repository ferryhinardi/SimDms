"use strict";
$(document).ready(function () {
    var options = {
        title: "Data Retensi Harian",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "GroupArea", text: "Area", type: "select", opt_text: "-- ALL AREA --" },
                    { name: "CompanyCode", text: "Dealer Name", type: "select", opt_text: "-- ALL DEALER --" },
                    { name: "BranchCode", text: "Outlet Name", type: "select", opt_text: "-- ALL OUTLET --" },
                    {
                        text: "Periode Visit",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "DateVisitFrom", text: "Date From", cls: "span4", type: "datepicker" },
                            { name: "DateVisitTo", text: "Date To", cls: "span4", type: "datepicker" },
                        ]
                    },
                    { name: "PMVisit", text: "P/M Saat Visit", type: "spinner" },
                    {
                        name: 'Activity', text: 'Aktivitas', type: 'select', opt_text: "-- SELECT ALL --",
                        items: [
                            { text: "Reminder", value: "R" },
                            { text: "Follow Up", value: "F" },
                        ]
                    },
                    {
                        text: "Periode",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "DateFrom", text: "Date From", cls: "span4", type: "datepicker" },
                            { name: "DateTo", text: "Date To", cls: "span4", type: "datepicker" },
                        ]
                    },
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
        { name: "GroupArea", url: "wh.api/combo/SrvGroupAreas", optionalText: "-- ALL AREA --" },
    ]);
    
    widget.render(function () {
        var filter = {
            DateVisitFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
            DateVisitTo: new Date(),
            DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
            DateTo: new Date(),
        }
        widget.populate(filter);
        $('#CompanyCode, #BranchCode').attr('disabled', 'disabled');
        $("[name=GroupArea]").on("change", function () {
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/SrvDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- ALL DEALER --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/SrvBranchList", params: { area: $("#pnlFilter [name=GroupArea]").val(), comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();
        });

        $("[name=Activity]").on("change", function () {
            if ($(this).val() == "") {
                $("[name=DateFrom]").prop("disabled", true);
                $("[name=DateTo]").prop("disabled", true);
            }
            else {
                $("[name=DateFrom]").prop("disabled", false);
                $("[name=DateTo]").prop("disabled", false);
            }
        }).trigger("change");
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
        $(".ajax-loader").show();
        $.ajax({
            type: "POST",
            data: params,
            url: "wh.api/report/GenerateDRH",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Data Retensi Harian Report';
                    $(".ajax-loader").hide();
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
    }
});