$(document).ready(function () {
    var options = {
        title: "Penjualan Service Tahunan",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
            {
                title: "Penjualan Service Tahunan",
                name: "summaryspk",
                items: [
                    {
                        text: "Kelompok Pekerjaan",
                        type: "controls",
                        items: [
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
        
        var dateFrom = $('#Tahun').val();
        var date1 = new Date();
        var date2 = date1.getFullYear()
        console.log(date2);
        $('#Tahun').val(date2);
        //widget.populate({ dateFrom: date2});
    });

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
        var tahun = $('#Tahun').val();
        var printType = $('#ReportType').val();
        //console.log(dateFrom);
        switch (printType.toLowerCase()) {
            case "1":
                reportType = "SvRpReport00401";
                data = "producttype," + tahun + "," + printType;
                break;
            case "2":
                reportType = "SvRpReport00402";
                data = "producttype," + tahun + "," + printType;
                break;
            case "3":
                reportType = "SvRpReport00403";
                data = "producttype," + tahun+ "," +  printType;
                break;
            default:
        }


        widget.showPdfReport({
            id: reportType,
            pparam: data,
            rparam: "SvRpReport007",
            type: "devex"
        });

    }
});