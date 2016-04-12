$(document).ready(function () {
    var options = {
        title: "PDI FSC Dealer Claim",
        xtype: "report",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        items: [
            {text:"Period Tanggal", type:"controls", cls:"span4", items:[
                { name: "IsPeriod", cls: "span2", type: "switch" },
                { name: "DateFrom", required: true, cls: "span4 full", text: "Periode Tanggal", type: "kdatepicker" },
            ]},
            { name: "DateTo", text: "s/d", cls: "span4", type: "kdatepicker", required: true },
            {
                text: "Kode Branch", type: "controls", cls: "span4 full", items: [
                   { name: "IsBranch", cls: "span2", type:"switch" },
                   { name: "BranchFrom", required: true, cls: "span4 full"},
                ]
            },
            { name: "BranchTo", text: "s/d", cls: "span4", required: true },
        ],
    }
    var widget = new SimDms.Widget(options);
    var defaultBranch;
    widget.render(function () {
        $(".frame").css({ top: 130 });
        $(".panel").css({ 'max-width': 1300 });
        $('#PoliceNo,#BasicModel,#RangkaNo,#MesinNo,#PelangganName').attr("ReadOnly", "ReadOnly");

        widget.populate({ IsPeriod: false, IsBranch: true });
        $('#BranchFrom, #BranchTo').attr("readonly", "readonly");
        $.post("sv.api/report/default", function (result) {
            defaultBranch = result;
            widget.default = result;
            widget.populate(result);
        });

        var dateFrom = $('input[name="DateFrom"]').val();
        var dateTo = $('input[name="DateTo"]').val();
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date1, DateTo: date1 });
        $('input[name="DateFrom"], input[name="DateTo"]').attr("readonly", "readonly");
    });

    $('#btnProcess').click(function () {
        var valid = $(".main form").valid();
        if (valid) {
            showReport();
        }
    });

    $('input[name="IsBranch"]').on("change", function () {
        var val = $(this).val();

        if (val.toString() == "true") {
            $('#BranchFrom, #BranchTo').attr("readonly", "readonly");
            widget.populate(defaultBranch);
        } else {
            console.log(val);
            $('#BranchFrom, #BranchTo').removeAttr("readonly");
        }
    });

    $('input[name="IsPeriod"]').on("change", function () {
        var val = $(this).val();

        if (val.toString() === "true") {
            $('input[name="DateFrom"], input[name="DateTo"]').removeAttr("readonly");
        } else {
            console.log(val);
            $('input[name="DateFrom"], input[name="DateTo"]').attr("readonly", "readonly");
            var date1 = new Date();
            widget.populate({ DateFrom: date1, DateTo: date1 });
        }
    });

    function showReport() {
        var BranchFrom = $('#BranchFrom').val();
        var BranchTo = $('#BranchTo').val();
        var DateFrom = " ";
        var DateTo = " ";
        var source = 0;
        if ($('input[name="IsPeriod"]').val()=="true") {
            DateFrom = $('input[name="DateFrom"]').val();
            DateTo = $('input[name="DateTo"]').val();
        } else {
            DateFrom = " ";
            DateTo = " ";
        }
        console.log($('input[name="IsPeriod"]').val());
        
        var data = [BranchFrom, BranchTo, DateFrom, DateTo,source];
        widget.showReport({
            id: "SvRpReport028",
            type: "devex",
            par: data
        });
    }
});