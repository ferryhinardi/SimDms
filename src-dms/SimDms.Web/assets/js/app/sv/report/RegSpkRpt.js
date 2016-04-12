$(document).ready(function () {
    var options = {
        title: "Register Surat Perintah Kerja",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
            //{ name: "btnProcessPanel", text: "Process with Panel", icon: "icon-bolt" },
        ],
        panels: [
            {
                title: "Kelompok Pekerjaan",
                name: "klmpkpekerjaan",
                items: [
                    { name: "Task", cls: "span4", type: "switch" },
                    { name: "TaskItem", cls: "span4", type: "select" },
                    { name: "SPKDateFrom", text: "Periode Awal", cls: "span4", type: "kdatepicker", required: true },
                    { name: "SPKDateTo", text: "Periode Akhir", cls: "span4", type: "kdatepicker", required: true },
                    { name: "Outstanding", required: true, cls: "span4", text: "Tipe", type: "select", items: [{ value: "true", text: "SPK" }, { value: "false", text: "Outstanding SPK" }] },
                    { name: "LimitService", required: true, cls: "limitservice span4", text: "Limit Nilai Service", type: "text" }
                ]
            },
            ],
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        var dateFrom = $('input[name="SPKDateFrom"]').val();
        var dateTo = $('input[name="SPKDateTo"]').val();
        var dt = new Date();
        var date1 = new Date(dt.getFullYear(), dt.getMonth() + 1, 0);
        var date2 = new Date(dt.getFullYear(), dt.getMonth(), 1);
        widget.populate({ SPKDateFrom: date2, SPKDateTo: date1 });
        widget.select({ selector: "#TaskItem", url: "sv.api/Combo/ServiceRefference" });
    });

    $('#TaskItem').attr('disabled', 'disabled');

    $('input[name="Task"]').on('change', function () {
        if ($('input[name="Task"]').val() =="true") {
            $('#TaskItem').removeAttr('disabled');
        } else {
            $('#TaskItem').val("");
            $('#TaskItem').attr('disabled','disabled');
        }
    });

    $("#btnProcess").on("click", function () {
        var valid = $(".main form").valid();
        if (valid) {
            showReport();
        }
    });
    function showReport() {
        var data;
        var user;
        var nilai = "Nilai Limit Service : >= Rp ";
        var isOutStanding = "0";
        var titleHeader = "REGISTER SURAT PERINTAH KERJA";
        var periodeHeader = "PERIODE : " + $('input[name="SPKDateFrom"]').val() + " s/d " + $('input[name="SPKDateTo"]').val();

        if ($('[name="Task"]').val() == true) {
            data = "producttype" + "," + $('[name="SPKDateFrom"]').val() + "," + $('[name="SPKDateTo"]').val() + "," + $("#TaskY").prop('checked') + "," + $('#TaskItem').val() + "," + $('#LimitService').val();
        } else {
            data = "producttype" + "," + $('[name="SPKDateFrom"]').val() + "," + $('[name="SPKDateTo"]').val() + "," + $("#TaskY").prop('checked') + "," + '' + "," + $('#LimitService').val();
        }

        $.ajax({
            async: false,
            type: "POST",
            url: 'sv.api/Report/Default',
            success: function (dt) {
                user = dt.UserId;
            }
        });

        if ($('[name="Outstanding"]').val() == "true") {
            outStanding = false;
        }
        else {
            outStanding = true;
            titleHeader += " OUTSTANDING";
        }

        if (outStanding) {
            //nilai += (Convert.ToDecimal(txtLimit.Text)).ToString("n0");
            isOutStanding = "1";
        }

        var header = user.toUpperCase() + ',' + titleHeader + ',' + periodeHeader + ',' + isOutStanding + ',' + nilai;
        widget.showPdfReport({
            id: "SvRpReport014",
            pparam: data,
            rparam: header,
            type: "devex"
        });

        //console.log(data);
        //widget.showReport({
        //    id: "SvRpReport014",
        //    type: "devex",
        //    par: data
        //});
    }

    $('.limitservice').slideUp();
    $('.limitservice').val('0');

    $('#Outstanding').on('change', function (e) {
        if (e.currentTarget.value == "true" || e.currentTarget.value == "") {
            $('.limitservice').slideUp();
        }
        else {
            $('.limitservice').slideDown();
            $('.limitservice').val('0');
        }
    });
});