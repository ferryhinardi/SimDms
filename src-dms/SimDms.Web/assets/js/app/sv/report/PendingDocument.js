$(document).ready(function () {
    var options = {
        title: "Dokumen Pending",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Print", icon: "icon-print" }
        ],
        panels: [
            {
                title: "Dokumen Pending",
                name: "perincianintensif",
                items: [
                    { name: "Month", required: true, cls: "span4", text: "Bulan", type: "select" },
                    { name: "Year", required: true, cls: "span4", text: "-" },
                ],
            },
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.render(function () {
        renderCallback();
        widget.post('sv.api/report/default', function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    function renderCallback() {
        widget.select({ name: "Month", url: "sv.api/combo/Months" });
    }

    //$('#PoliceNo,#BasicModel,#RangkaNo,#MesinNo,#PelangganName').attr("ReadOnly");
    $("#btnProcess").on("click", function () {
        showReport();
    });

    function showReport() {
        var data = $(".main .gl-widget").serializeObject();
        console.log(data);
        var par = 'producttype' + "," + data.Month + ',' + data.Year;

        var ReportType = 'SvRpReport017';
        widget.showPdfReport({
            id: ReportType,
            pparam: par,
            rparam: "Print SvRpReport017",
            type: "devex"
        });
    }
});