$(document).ready(function () {
    var options = {
        title: "Summary Surat Perintah Kerja",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
        {
            title: "Summary Surat Perintah Kerja",
            name: "summaryspk",
            items: [
                { name: "DateFrom", text: "Periode Awal", cls: "span4", type: "kdatepicker", required: true },
                { name: "DateTo", text: "Periode Akhir", cls: "span4", type: "kdatepicker", required: true },
                    { name: "Outstanding", required: true, cls: "span4", text: "Tipe", type: "select", items: [{ value: "0", text: "SPK" }, { value: "1", text: "Outstanding SPK" }] },
            ],
        }],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var dateFrom = $('input[name="DateFrom"]').val();
        var dateTo = $('input[name="DateTo"]').val();
        var dt = new Date();
        var date1 = new Date(dt.getFullYear(), dt.getMonth() + 1, 0);
        var date2 = new Date(dt.getFullYear(), dt.getMonth(), 1);
        widget.populate({ DateFrom: date2, DateTo: date1 });
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
        data = "producttype" + "," + $('[name="DateFrom"]').val() + "," + $('[name="DateTo"]').val() + "," + $('#Outstanding').val();
        console.log(data);
        widget.showPdfReport({
            id: "SvRpReport015",
            pparam: data,
            rparam: "SvRpReport015",
            type: "devex"
        });
        //widget.showReport({
        //    id: "SvRpReport015",
        //    type: "devex",
        //    par: data
        //});
    }
});