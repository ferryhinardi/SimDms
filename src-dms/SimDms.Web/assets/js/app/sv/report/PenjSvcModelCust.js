$(document).ready(function () {
    var options = {
        title: "Penjualan Service Per Model & Pelanggan",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
            {
                title: "Penjualan Service Per Model & Pelanggan",
                name: "summaryspk",
                items: [
                    { name: "DateFrom", text: "Periode Awal", cls: "span4", type: "kdatepicker", required: true },
                    { name: "DateTo", text: "Periode Akhir", cls: "span4", type: "kdatepicker", required: true },
                ],
            },
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var dateFrom = $('input[name="DateFrom"]').val();
        var dateTo = $('input[name="DateTo"]').val();
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });

        widget.select({ selector: "#PartType", url: "sv.api/Combo/PartTypeLookup" }, function () {
            $("#PartType option[value='']").text("-- SELECT ALL --");
            $("#PartType option[value='']").val("%");
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
        var data;
        data = "producttype," + $('[name="DateFrom"]').val() + "," + $('[name="DateTo"]').val();
        console.log(data);


        widget.showPdfReport({
            id: "SvRpReport016",
            pparam: data,
            rparam: "SvRpReport016",
            type: "devex"
        });
    }
});