$(document).ready(function () {
    var options = {
        title: "Customer Birthday Report",
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
    widget.render();
    $("#btnProcess").on("click", function () { showReport(); });
    function showReport() {
        widget.showReport({
            id: "CsBirthday",
            par: ["%", $("input[name='pDateFrom']").val(), $("input[name='pDateTo']").val()],
            type: ""
        });
    }
});