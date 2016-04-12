$(document).ready(function () {
    var vars = {};
    var options = {
        title: "Print Sales Order",
        xtype: "report",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        items: [
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        //$(".frame").css({ top: 150 });
        //$(".panel").css({ 'max-width': 1310 });
        vars["SONumber"] = getParam("SONumber");

    });

    $("#btnProcess").on("click", function () {
        showReport();
    });
    function showReport() {
        widget.post("its.api/SalesOrder/Default", function (result) {
            if (widget.isNullOrEmpty(result) == false) {
                var data = [result.CompanyCode, result.BranchCode, vars.SONumber];
                widget.showReport({
                    id: "OmRpSalesTrn001A",
                    type: "devex",
                    par: data
                });
            }
        });
    }

    $('.limitservice').slideUp();
    $('.limitservice').val('0');

    $('#Outstanding').on('change', function (e) {
        if (e.currentTarget.value == 0 || e.currentTarget.value == "") {
            $('.limitservice').slideUp();
        }
        else {
            $('.limitservice').slideDown();
            $('.limitservice').val('0');
        }
    });

    function getParam(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.href);
        if (results == null)
            return "";
        else
            return results[1];
    }
});