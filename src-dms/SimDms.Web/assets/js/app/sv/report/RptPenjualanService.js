$(document).ready(function () {
    var options = {
        title: "Penjualan Service",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
            {
                title: "Penjualan Service",
                name: "salesservice",
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
                    {
                        name: "ReportType", required: true, cls: "span4", text: "Dicetak per-", type: "select", items: [
                        { value: "1", text: "Kategori" }, { value: "2", text: "Pelanggan" },
                        { value: "3", text: "Basic Model" }, { value: "4", text: "Jenis Pekerjaan" }]
                    }
                ],
            }
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        //$(".frame").css({ top: 110 });
        //$(".panel").css({ 'max-width': 1300 });
        var dateFrom = $('#Tahun').val();
        var date1 = new Date();
        var date2 = date1.getFullYear()
        console.log(date2);
        $('#Tahun').val(date2);
        //widget.populate({ dateFrom: date2});
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
        var reportType;
        var bulan = $("#Bulan").val();
        var tahun = $('#Tahun').val();
        var printType = $('#ReportType').val();
        //console.log(dateFrom);
        switch (printType.toLowerCase()) {
            case "1":
                reportType = "svrpreport00201";
                data = "producttype,"+tahun+","+ bulan+","+ printType;
                break;
            case "2":
                reportType = "svrpreport00202";
                data = "producttype," + tahun + "," + bulan + "," + printType;
                break;
            case "3":
                reportType = "svrpreport00203";
                data = "producttype," + tahun + "," + bulan + "," + printType;
                break;
            case "4":
                reportType = "svrpreport00204";
                data = "producttype," + tahun + "," + bulan + "," + printType;
                break;
            default:
        }

        widget.showPdfReport({
            id: reportType,
            pparam: data,
            rparam: reportType,
            type: "devex"
        });

        //widget.showReport({
        //    id: reportType,
        //    type: "devex",
        //    par: data
        //});
    }
});