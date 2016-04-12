$(document).ready(function () {
    var options = {
        title: "Perincian Intensif",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
           {
               title: "Perincian Intensif",
               name: "perincianintensif",
               items: [
                   {
                       name: "Year", required: true, cls: "span4 full", text: "Tahun", type: "select", items: [
                         { value: "2012", text: "2012" },
                         { value: "2013", text: "2013" },
                         { value: "2014", text: "2014" },
                         { value: "2015", text: "2015" },
                       ]
                   },
               ],
           }]
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var date1 = new Date();
        var year = date1.getFullYear();
        $('#Year').val(year);
    });

    //$('#PoliceNo,#BasicModel,#RangkaNo,#MesinNo,#PelangganName').attr("ReadOnly");
    $("#btnProcess").on("click", function () { showReport(); });
    

    $('#btnProcess').click(function () {
        var valid = $(".main form").valid();
        if (valid) {
            showReport();
        }
    });

    function showReport() {
        var data = $('#Year').val();
        var ReportType = 'SvRpReport024';
        widget.showPdfReport({
            id: ReportType,
            pparam: data,
            rparam: "admin",
            type: "devex"
        });
    }
});