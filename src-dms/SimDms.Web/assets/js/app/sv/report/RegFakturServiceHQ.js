$(document).ready(function () {
    var options = {
        title: "Register Faktur Service HQ",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
            {
                title: "Register Faktur Service HQ",
                name: "faktursvc",
                items: [
                    { name: "DateFrom", text: "Periode Awal", cls: "span4", type: "kdatepicker", required: true },
                    { name: "DateTo", text: "s/d", cls: "span4", type: "kdatepicker", required: true },
                    {
                        name: "ReportType", text: "Dicetak per-", type: "select", cls: "span4", required: true, items: [
                              { value: "Invoice", text: "Faktur Penjualan" },
                              { value: "HPPService", text: "HPP Service" },
                              { value: "CancelInv", text: "Cancel Faktur Penjualan" }
                        ]
                    },
                    { name: "FakturNo", text: "Param No Faktur", type: "select", cls: "span4" }

                ],
            }
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var dateFrom = $('input[name="DateFrom"]').val();
        var dateTo = $('input[name="DateTo"]').val();
        var dt = new Date()
        var date1 = new Date(dt.getFullYear(), dt.getMonth() + 1, 0);
        var date2 = new Date(dt.getFullYear(), dt.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });

        widget.select({ selector: "#FakturNo", url: "sv.api/Combo/Select4FakturNo" }, function () {
            $("#FakturNo option[value='']").text("-- SELECT ALL --");
        });
    });

    $('#TaskItem').attr('disabled', 'disabled');
    $("#btnProcess").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            showReport();
        }
    });
    function showReport() {
        //var reportType;
        var data;
        var reportType = $("#ReportType").val();
        var dateFrom = $('input[name="DateFrom"]').val();
        var dateTo = $('input[name="DateTo"]').val();
        var fakturNo = $('#FakturNo').val();
        var user;
        var titleHeader = "REGISTER FAKTUR SERVICE HQ";
        var periodeHeader = "PERIODE : " + dateFrom + " s/d " + dateTo;

        $.ajax({
            async: false,
            type: "POST",
            url: 'sv.api/Report/Default',
            success: function (dt) {
                user = dt.UserId;
            }
        });
        console.log(reportType);
        switch (reportType.toLowerCase()) {
            case "invoice":
                reportType = "svrpreport00101";
                data = dateFrom + "," + dateTo + "," + true + "," + fakturNo;
                break;
            case "hppservice":
                reportType = "SvRpReport00104";
                data = dateFrom + "," + dateTo + "," + true + "," + fakturNo;
                break;
            case "cancelinv":
                reportType = "svrpreport00105";
                data = dateFrom + "," + dateTo + "," + true + "," + fakturNo;
                break;
            default:
        }
        console.log(reportType + '---' + data);
        var param = user.toUpperCase() + ',' + dateFrom + ',' + dateTo;
        widget.showPdfReport({
            id: reportType,
            pparam: data,
            rparam: param,
            type: "devex"
        });
    }
});