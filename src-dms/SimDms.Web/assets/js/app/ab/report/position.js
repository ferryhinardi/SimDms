$(document).ready(function () {
    var options = {
        title: "Report - Position",
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
            { name: "DepartmentCode", text: "Department", cls: "span4", type: "select" },
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "DepartmentCode", url: "ab.api/combo/departments", optionalText:"ALL DEPARTMENT" }]);
    widget.render(function () {
        $(".frame").css({ top: 95 });
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

