$(document).ready(function () {
    var options = {
        title: "3 Days Call Report",
        xtype: "report",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" },
        ],
        items: [
            { name: "pDateFrom", text: "Date From", cls: "span4", type: "datepicker" },
            { name: "pDateTo", text: "Date To", cls: "span4", type: "datepicker" },
            { name: "pStatus", text: "Status", cls: "span4", type: "select" },
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "pStatus", url: "cs.api/Combo/Status/" },
    ]);

    widget.render();


    //    $("#HolidayYear").on("change", function () { showReport(); })
    $("#btnProcess").on("click", function () { showReport(); });
    function showReport() {
        widget.showReport({
            id: "CsTDayCall",
            par: ["%", $("input[name='pDateFrom']").val(), $("input[name='pDateTo']").val(), $('#pStatus').val()],
            type: ""
        });
    }
});