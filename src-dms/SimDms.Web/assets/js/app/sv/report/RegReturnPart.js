$(document).ready(function () {
    var options = {
        title: "Register Return Part",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        panels: [
            {
                title: "Register Return Part",
                name: "summaryspk",
                items: [
                    { name: "DateFrom", text: "Periode Tanggal Awal", cls: "span4", type: "kdatepicker", required: true },
                    { name: "DateTo", text: "Periode Tanggal Akhir", cls: "span4", type: "kdatepicker", required: true },
                    { name: "PartType", cls: "span4", text: "Tipe Part", type: "select"},
                ],
            },
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
        data = $('[name="DateFrom"]').val()+","+ $('[name="DateTo"]').val()+","+ $('#PartType').val();
        console.log(data);


        widget.showPdfReport({
            id: "SvRpTrn017",
            pparam: data,
            rparam: "SvRpTrn017",
            type: "devex"
        });

        //widget.showReport({
        //    id: "SvRpTrn017",
        //    type: "devex",
        //    par: data
        //});
    }
});