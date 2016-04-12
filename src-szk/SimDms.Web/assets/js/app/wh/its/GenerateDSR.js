$(document).ready(function () {
    var options = {
        title: "Generate Daily Sales Reports",
        xtype: "panels",
        toolbars: [
            { name: "btnGenerate", text: "Generate Excel", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                        //{ name: "isTable", text: "Update table", cls: "span2", type: "check" },
                        { name: "DateToDSR", text: "Date (Day - 1)", cls: "span4 Full", type: "datepicker" },
                        { name: "isModel", text: "Update Model", cls: "span4 full ", type: "check" },
                     ]
            },
        ]
    }

    var widget = new SimDms.Widget(options);

    var nationalSLS = '';

    widget.render(function () {
        //$('[name="DateToDSR"]').attr('disabled', 'disabled');
        $(".frame").css({ top: 240 });
        widget.post("wh.api/inquiryprod/default", function (result) {
            if (result.success) {
                console.log(result.data);
                widget.default = result.data;
                widget.populate(widget.default);
            }
        });
        $("label[for='isModel']").hide();
        $("label[for='isTable']").hide();
        console.log("ini isModel : " + $("#isModel").is(":checked") == true ? 1 : 0);
        console.log("ini isTable : " + $("#isTable").is(":checked") == true ? 1 : 0);
        //AllDay = $("#isTable").is(":checked") ? "true" : "false";
    });
    $('#isTable').change(function () {
        //if ($(this).is(":checked") == true) {
        //    $('[name="DateToDSR"]').removeAttr('disabled');
        //} else {
        //    $('[name="DateToDSR"]').attr('disabled', 'disabled');
        //}
    });
    function getSqlDate(value) {
        console.log(value);
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
   
    $("#btnGenerate").on('click', function (e) {
        var dateini = $('[name="DateToDSR"]').val(); //getSqlDate(new Date().getDate());
        var fltrDate = getSqlDate($('[name="DateToDSR"]').val());
        //if ($("#isTable").is(":checked") == false) {
        //    fltrDate = getSqlDate(new Date().getDate());
        //    dateini = moment(new Date().getDate(), "DD-MMM-YYYY").format("DD-MMM-YYYY");
        //}
        var url = "wh.api/inquiryMRSR/DSR_WEB?";
        var params = 'strdate=' + dateini;
        params += "&fltrDate=" + fltrDate;
        params += "&upslsmdlcd=" + ($("#isModel").is(":checked") == true ? "1" : "0");
        params += "&uptblrgstr=" + ($("#isTable").is(":checked") == true ? "1" : "0");
        url = url + params;
        console.log(params);
        window.location = url;
        console.log("ini isModel : " + $("#isModel").is(":checked") == true ? 1 : 0);
        console.log("ini isTable : " + $("#isTable").is(":checked") == true ? 1 : 0);
        //window.location.href = "wh.api/inquiryprod/CreateExcelExportGenerateITS?area=" + $('[name="Area"]').val() + '&dealerCode=' + $('[name="Dealer"]').val()
        //+ '&outletCode=' + $('[name="Outlet"]').val() + '&startDate=' + $('[name="DateToDSR"]').val() + '&endDate=' + $('[name="DateToDSR"]').val()
        //+ '&reportType=' + $('[name="Filter"]').val();   
    });
});