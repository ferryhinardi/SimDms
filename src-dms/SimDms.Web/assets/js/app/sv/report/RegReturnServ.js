$(document).ready(function () {
    var options = {
        title: "Register Return Service",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],panels: [
            {
                title: "Register Return Service",
                name: "faktursvc",
                items: [
                    { name: "PeriodeFrom", text: "Periode", cls: "span4", type: "kdatepicker", required: true },
                    { name: "PeriodeTo", text: "s/d", cls: "span4", type: "kdatepicker", required: true },
                ],
            }
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var dateFrom = $('input[name="PeriodeFrom"]').val();
        var dateTo = $('input[name="PeriodeTo"]').val();
        var dt = new Date()
        var date1 = new Date(dt.getFullYear(), dt.getMonth() + 1, 0);
        var date2 = new Date(dt.getFullYear(), dt.getMonth(), 1);
        widget.populate({ PeriodeFrom: date2, PeriodeTo: date1 });

    });
    
    $("#btnProcess").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            showReport();
        }
    });
    function showReport() {
        var data = $('[name="PeriodeFrom"]').val()+","+ $('[name="PeriodeTo"]').val();
        widget.showPdfReport({
            id: "SvRpReport032",
            pparam: data,
            rparam: "SvRpReport032",
            type: "devex"
        });
        //widget.showReport({
        //    id: "SvRpReport032",
        //    type: "devex",
        //    par: data
        //});
    }
});