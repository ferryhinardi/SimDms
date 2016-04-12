$(document).ready(function () {
    var options = {
        title: "Penjualan Service Perkelompok Pekerjaan",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
            {
                title: "Penjualan Service Perkelompok Pekerjaan",
                name: "summaryspk",
                items: [
                    {
                        text: "Kelompok Pekerjaan",
                        type: "controls",
                        items: [
                            {
                                name: "Bulan", required: true, cls: "span4", text: "Tipe", type: "select", items: [
                                { value: "1", text: "Januari" }, { value: "2", text: "Februari" },
                                { value: "3", text: "Maret" }, { value: "4", text: "April" },
                                { value: "5", text: "Mei" }, { value: "6", text: "Juni" },
                                { value: "7", text: "Juli" }, { value: "8", text: "Agustus" },
                                { value: "9", text: "September" }, { value: "10", text: "Oktober" },
                                { value: "11", text: "November" }, { value: "12", text: "Desember" }]
                            },
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
        data = "producttype,"+$('#Bulan').val() + "," + $('#Tahun').val();
        console.log(data);


        widget.showPdfReport({
            id: "SvRpReport003",
            pparam: data,
            rparam: "SvRpReport003",
            type: "devex"
        });
    }
});