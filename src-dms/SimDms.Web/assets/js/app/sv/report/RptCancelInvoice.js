$(document).ready(function () {
    var options = {
        title: "Laporan Invoice Batal",
        xtype: "report",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" },
            { name: "btnExport", text: "Export Excel", icon: "icon-bolt" }
        ],
        items: [
            {
                text: "Company",
                type: "controls", items: [
                    { name: "CompanyCode", text: "CompanyCode", cls: "span2", type: "text", readonly: true },
                    { name: "CompanyName", text: "CompanyName", cls: "span4", type: "text", readonly: true },
                ]
            },
            {
                text: "Branch",
                type: "controls", items: [
                    { name: "BranchCode", text: "BranchCode", cls: "span6", type: "select", required:true },
                ]
            },
            { name: "InvoiceDateFrom", text: "Periode Awal", cls: "span2", type: "kdatepicker", required: true },
            { name: "InvoiceDateTo", text: "Periode Akhir", cls: "span2", type: "kdatepicker", required: true }
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        $(".frame").css({ top: 150 });
        $(".panel").css({ 'max-width': 1300 });
        var dateFrom = $('input[name="InvoiceDateFrom"]').val();
        var dateTo = $('input[name="InvoiceDateTo"]').val();
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ dateFrom: date2, dateTo: date1 });
        LoadDefault();
    });

    $('#TaskItem').attr('disabled', 'disabled');

    $('input[name="Task"]').on('change', function () {
        if ($('input[name="Task"]').val() == "true") {
            $('#TaskItem').removeAttr('disabled');
        } else {
            $('#TaskItem').val("");
            $('#TaskItem').attr('disabled', 'disabled');
        }
    });

    $("#btnProcess").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            showReport("rdlc");
        }
    });
    $('#btnExport').on('click', function () {
        var data = [$('#CompanyCode').val(), $('#BranchCode').val(), $('[name="InvoiceDateFrom"]').val(), $('[name="InvoiceDateTo"]').val()];
        console.log(data);
        showReport("export");
    });
    function showReport(type) {
        var data = [$('#CompanyCode').val(), $('#BranchCode').val(), $('[name="InvoiceDateFrom"]').val(), $('[name="InvoiceDateTo"]').val()];
        console.log(data);
        widget.showReport({
            id: "SvRptCancelInvoice",
            type: type,
            par: data,
            filename: "SvRptCancelInvoice"
        });
    }
    widget.select({ selector: "#BranchCode", url: "cs.api/Combo/ListBranchCode" }, function () {
        //$("#BranchCode option").each(function () {
        //    if ($(this).text() == "-- SELECT ONE --") {
        //        $(this).val("%");
        //        $(this).text("-- SELECT ALL --");
        //        console.log("SET SELECT ALL");
        //    }
        //});
    });

    function LoadDefault() {
        var url = "sv.api/Report/Default";
        widget.post(url, function (result) {
            if (widget.isNullOrEmpty(result) == false) {
                widget.populate(result);
            }
        });
        console.log("asdasd");
    }

});