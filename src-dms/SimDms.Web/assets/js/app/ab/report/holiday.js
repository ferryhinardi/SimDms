$(document).ready(function () {
    var options = {
        title: "Report - Master Holiday",
        xtype: "report",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" },
        ],
        items: [
            {
                text: "Branch",
                type: "controls",
                items: [
                    { name: "CompanyCode", cls: "span2", placeHolder: "Company Code", readonly: true },
                    { name: "CompanyName", cls: "span6", placeHolder: "Company Name", readonly: true }
                ]
            },
            { name: "HolidayYear", text: "Holiday Year", cls: "span4", type: "select" },
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "HolidayYear", url: "ab.api/combo/holidayyears" }]);
    widget.render(function () {
        $(".frame").css({ top: 95 });
        widget.post("ab.api/holiday/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });
    $("#HolidayYear").on("change", function () { showReport(); })
    $("#btnProcess").on("click", function () { showReport(); });
    function showReport() {
        widget.showReport({
            id: "HrMstEmployee",
            par: [$("#CompanyCode").val(), $("#HolidayYear").val()]
        });
    }
});

