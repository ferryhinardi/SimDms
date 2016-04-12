$(document).ready(function () {
    $.xhrPool = [];
    var post = {};
    var widget = new SimDms.Widget({
        title: "Executive Summary - Total",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        name: "BranchCode", type: "select", text: "Outlet", opt_text: "-- SELECT ALL --", cls: "span5"
                    }
                ]
            },
            {
                name: "pnlDashboard",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "icon-refresh" }
        ]
    });

    widget.render(init);
    function init() {
        widget.post("its.api/Combo/IsHolding", {}, function (result) {
            if (result)
                widget.select({ selector: "#BranchCode", url: "its.api/Combo/ListBranchCode" }, function (data) {
                    $("#Status").val("0");
                    post.BranchCode = "";
                    renderTable();
                });
            else {
                widget.select({ selector: "#BranchCode", url: "its.api/Combo/CurrentBranchCode" }, function (data) {
                    $("#Status").val("0");
                    if (data.length) {
                        $("#BranchCode").val(data[0].value);
                        post.BranchCode = data[0].value;
                    }
                    else
                        post.BranchCode = "";
                    $("#BranchCode").prop("disabled", true);
                    renderTable();
                });
            }
        });
        $(document).on("click", "#btnRefresh", function () {
            post.BranchCode = $("[name='BranchCode']").val();
            $.xhrPool.abortAll();
            renderTable();
        });
    };

    function renderTable() {
        var table = "";

        table = "<table class=\"dashboard big\">"
            + "<tr style='line-height:2px'>"
            + "<th style='width:12%' class='blank center' style='line-height:2px'></th>"
            + "<th style='width:17%' class='blank info2' style='line-height:2px'></th>"
            + "<th style='width:12%' class='blank center' style='line-height:2px'></th>"
            + "<th style='width:2%' class='blank center' style='line-height:2px'></th>"
            + "<th style='width:17%' class='blank info2' style='line-height:2px'></th>"
            + "<th style='width:12%' class='blank center' style='line-height:2px'></th>"
            + "</tr>"

            + "<tr style='line-height:24px' class='top'>"
            + "<td class='blank info2 CurrentMonth'></td>"
            + "<td class=\"center3\"><span class=\"animated\" id='CurrPeriodStock'></span></td>"
            + "<td class=\"blank info\">STOCK</td>"
            + "<td class='blank center'></td>"
            + "<td class=\"center3\"><span class=\"animated\" id='CurrPeriodInvoice'></span></td>"
            + "<td class=\"blank info\">INV</td>"
            + "</tr>"

            + "<tr style='line-height:24px' class='top'>"
            + "<td class='blank info2 CurrentPreviousMonth'></td>"
            + "<td class=\"center3\"><span  class=\"animated\" id='CurrPrevPeriodStock'></span></td>"
            + "<td class=\"center3\"><span class=\"animated\" id='PersenCurrStock'></span></td>"
            + "<td class='blank center'></td>"
            + "<td class=\"center3\"><span  class=\"animated\" id='CurrPrevPeriodInvoice'></span></td>"
            + "<td class=\"center3\"><span  class=\"animated\" id='PersenCurrInvoice'></span></td>"
            + "</tr>"

            + "<tr style='line-height:24px' class='top'>"
            + "<td class='blank info2 PreviousSumMonth'></td>"
            + "<td class=\"center3\"><span  class=\"animated\" id='SumPrevPeriodStock'></span></td>"
            + "<td class=\"center3\"><span class=\"animated\" id='PersenPrevStock'></span></td>"
            + "<td class='blank center'></td>"
            + "<td class=\"center3\"><span  class=\"animated\" id='SumPrevPeriodInvoice'></span></td>"
            + "<td class=\"center3\"><span  class=\"animated\" id='PersenPrevInvoice'></span></td>"
            + "</tr>"

            + "<tr style='line-height:24px' class='top'>"
            + "<td class='blank info2 PreviousSum2Month'></td>"
            + "<td class=\"center3\"><span  class=\"animated\" id='SumPrev2PeriodStock'></span></td>"
            + "<td class=\"center3\"><span class=\"animated\" id='PersenPrev2Stock'></span></td>"
            + "<td class='blank center'></td>"
            + "<td class=\"center3\"><span  class=\"animated\" id='SumPrev2PeriodInvoice'></span></td>"
            + "<td class=\"center3\"><span  class=\"animated\" id='PersenPrev2Invoice'></span></td>"
            + "</tr>"

            + "<tr style='line-height:8px'>"
            + "<td class='blank' style='line-height:8px'></td>"
            + "</tr>"

            + "<tr style='line-height:24px' class='bottom'>"
            + "<td class='blank info2 CurrentMonth'></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='CurrPeriodINQ'></span></td>"
            + "<td class=\"blank info\">INQ</td>"
            + "<td class='blank center'></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='CurrPeriodSPK'></span></td>"
            + "<td class='blank info'>SPK</td>"
            + "</tr>"

            + "<tr style='line-height:24px' class='bottom'>"
            + "<td class='blank info2 CurrentPreviousMonth'></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='CurrPrevPeriodINQ'></span></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='PersenCurrINQ'></span></td>"
            + "<td class='blank center'></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='CurrPrevPeriodSPK'></span></td>"
            + "<td class=\"center2\"><span   class=\"animated\" id='PersenCurrSPK'></span></td>"
            + "</tr>"

            + "<tr style='line-height:24px' class='bottom'>"
            + "<td class='blank info2 PreviousSumMonth'></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='SumPrevPeriodINQ'></span></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='PersenPrevINQ'></span></td>"
            + "<td class='blank center'></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='SumPrevPeriodSPK'></span></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='PersenPrevSPK'></span></td>"
            + "</tr>"

            + "<tr style='line-height:24px' class='bottom'>"
            + "<td class='blank info2 PreviousSum2Month'></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='SumPrev2PeriodINQ'></span></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='PersenPrev2INQ'></span></td>"
            + "<td class='blank center'></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='SumPrev2PeriodSPK'></span></td>"
            + "<td class=\"center2\"><span  class=\"animated\" id='PersenPrev2SPK'></span></td>"
            + "</tr>"

            + "</table>";

        $("#pnlDashboard").html(table);
        refreshData();
        SimDms.onTenSecondChanged = function () { refreshData() };
    };

    function refreshData() {
        $.ajax({
            url: "its.api/inquiry/PmExecSummary", 
            data: { BranchCode: post.BranchCode },
            type: "POST",
            dataType: "JSON",
            beforeSend: function (jqXHR) { $.xhrPool.push(jqXHR); }, //  and connection to list
            success: function (result) {
                var i = $.xhrPool.indexOf(result);   //  get index for current connection completed
                if (i > -1) $.xhrPool.splice(i, 1);  //  removes from list by index

                var data = result[0];
                $(".CurrentMonth").text(moment(data.CurrentDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth)
                $(".CurrentPreviousMonth").text(moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' ' + data.PeriodOfCurrMonth)
                $(".PreviousSumMonth").text(moment(data.PrevMonthOfCurrDate).format('MMM').toUpperCase() + ' SUM')
                $(".PreviousSum2Month").text(moment(data.Prev2MonthOfCurrDate).format('MMM').toUpperCase() + ' SUM')

                $("#CurrPeriodStock").text(NumDigit(data.currStock));
                $("#CurrPeriodInvoice").text(NumDigit(data.currInvoice));
                $("#CurrPeriodINQ").text(NumDigit(data.currInq));
                $("#CurrPeriodSPK").text(NumDigit(data.currSPK));

                $("#CurrPrevPeriodStock").text(NumDigit(data.currPrevStock));
                $("#CurrPrevPeriodInvoice").text(NumDigit(data.currPrevInvoice));
                $("#CurrPrevPeriodINQ").text(NumDigit(data.currPrevINQ));
                $("#CurrPrevPeriodSPK").text(NumDigit(data.currPrevSPK));

                $("#SumPrevPeriodStock").text(NumDigit(data.prevStock));
                $("#SumPrevPeriodInvoice").text(NumDigit(data.prevInvoice));
                $("#SumPrevPeriodINQ").text(NumDigit(data.prevInq));
                $("#SumPrevPeriodSPK").text(NumDigit(data.prevSPK));

                $("#SumPrev2PeriodStock").text(NumDigit(data.prev2Stock));
                $("#SumPrev2PeriodInvoice").text(NumDigit(data.prev2Invoice));
                $("#SumPrev2PeriodINQ").text(NumDigit(data.prev2Inq));
                $("#SumPrev2PeriodSPK").text(NumDigit(data.prev2SPK));

                if (data.currStock == 0 || data.currPrevStock == 0)
                    $("#PersenCurrStock").text("-");
                else $("#PersenCurrStock").text(NumDigit((data.currStock / data.currPrevStock) * 100) + '%');

                if (data.currInvoice == 0 || data.currPrevInvoice == 0)
                    $("#PersenCurrInvoice").text("-");
                else $("#PersenCurrInvoice").text(NumDigit((data.currInvoice / data.currPrevInvoice) * 100) + '%');

                if (data.currInq == 0 || data.currPrevINQ == 0)
                    $("#PersenCurrINQ").text("-");
                else $("#PersenCurrINQ").text(NumDigit((data.currInq / data.currPrevINQ) * 100) + '%');

                if (data.currSPK == 0 || data.currPrevSPK == 0)
                    $("#PersenCurrSPK").text("-");
                else $("#PersenCurrSPK").text(NumDigit((data.currSPK / data.currPrevSPK) * 100) + '%');

                /*********************************************************************************************/
                if (data.currStock == 0 || data.prevStock == 0)
                    $("#PersenPrevStock").text("-");
                else $("#PersenPrevStock").text(NumDigit((data.currStock / data.prevStock) * 100) + '%');

                if (data.currInvoice == 0 || data.prevInvoice == 0)
                    $("#PersenPrevInvoice").text("-");
                else $("#PersenPrevInvoice").text(NumDigit((data.currInvoice / data.prevInvoice) * 100) + '%');

                if (data.currInq == 0 || data.prevInq == 0)
                    $("#PersenPrevINQ").text("-");
                else $("#PersenPrevINQ").text(NumDigit((data.currInq / data.prevInq) * 100) + '%');

                if (data.currSPK == 0 || data.prevSPK == 0)
                    $("#PersenPrevSPK").text("-");
                else $("#PersenPrevSPK").text(NumDigit((data.currSPK / data.prevSPK) * 100) + '%');

                /*******************************************************************************************/
                if (data.prevStock == 0 || data.prev2Stock == 0)
                    $("#PersenPrev2Stock").text("-");
                else $("#PersenPrev2Stock").text(NumDigit((data.prevStock / data.prev2Stock) * 100) + '%');

                if (data.prevInvoice == 0 || data.prev2Invoice == 0)
                    $("#PersenPrev2Invoice").text("-");
                else $("#PersenPrev2Invoice").text(NumDigit((data.prevInvoice / data.prev2Invoice) * 100) + '%');

                if (data.prevInq == 0 || data.prev2Inq == 0)
                    $("#PersenPrev2INQ").text("-");
                else $("#PersenPrev2INQ").text(NumDigit((data.prevInq / data.prev2Inq) * 100) + '%');

                if (data.prevSPK == 0 || data.prev2SPK == 0)
                    $("#PersenPrev2SPK").text("-");
                else $("#PersenPrev2SPK").text(NumDigit((data.prevSPK / data.prev2SPK) * 100) + '%');

                $("#pnlDashboard .center3 > span.animated").addClass("fadeOutLeft");
                $("#pnlDashboard .center2 > span.animated").addClass("fadeOutLeft");

                setTimeout(function () {
                    $("#pnlDashboard .center3 > span.animated").removeClass("fadeOutLeft");
                    $("#pnlDashboard .center3 > span.animated").addClass("fadeInRight");
                    $("#pnlDashboard .center2 > span.animated").removeClass("fadeOutLeft");
                    $("#pnlDashboard .center2 > span.animated").addClass("fadeInRight");
                }, 500);
            }
        });
    };

    function NumDigit(n) {
        if (n == null) return 0;
        return n.toFixed(0).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        });
    }

    $.xhrPool.abortAll = function () {
        $(this).each(function (i, jqXHR) {   //  cycle through list of recorded connection
            jqXHR.abort();  //  aborts connection
            $.xhrPool.splice(i, 1); //  removes from list by index
        });
    }
});