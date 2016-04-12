$(document).ready(function () {
    var options = {
        title: "Report - Master Shift",
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
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        $(".frame").css({ top: 58 });
        widget.post("ab.api/shift/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });
    $("#btnProcess").on("click", function () { showReport(); });
    function showReport() {
        widget.showReport({
            id: "HrMstShift",
            par: [$("#CompanyCode").val()]
        });
    }
});

