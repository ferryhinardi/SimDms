$(document).ready(function () {
    var options = {
        title: "Penjualan Service Tahunan Per Kelompok Pekerjaan",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
            {
                title: "Penjualan Service Tahunan Per Kelompok Pekerjaan",
                name: "summaryspk",
                items: [
                    {
                        text: "Tahun",
                        type: "controls",
                        items: [
                            { name: "Tahun", cls: "span2", type: "text" }
                        ]
                    },
                ],
            },
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var dateFrom = $('#Tahun').val();
        var date1 = new Date();
        var date2 = date1.getFullYear()
        var month = date1.getMonth() + 1;
        $('#Bulan').val(month);
        $('#Tahun').val(date2);

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
        data = "producttype," + $('#Tahun').val();
        console.log(data);


        widget.showPdfReport({
            id: "SvRpReport005",
            pparam: data,
            rparam: "SvRpReport005",
            type: "devex"
        });
    }
});