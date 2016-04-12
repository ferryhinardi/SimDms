var widget = new SimDms.Widget({
    title: "Executive Summary - Current vs Previous Month (Working Day)",
    xtype: "panels",
    toolbars: [
        { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
        { name: "btnFullScr", text: "Full Screen", icon: "fa fa-desktop" },
    ],
    panels: [
        {
            name: "pnlDashboard",
            xtype: "k-grid",
        },
        {
            name: "pnlCloseDash",
            cls: "dashboard"
        },
    ],
});

function NumDigit(n) {

    if (n == null) return 0;

    return n.toFixed(0).replace(/./g, function (c, i, a) {
        return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
    });
}

widget.render(function () {
    widget.default = {
        IndentAll: 0,
        IndentErtiga: 0,
        IndentKarimun: 0
    };
    widget.populate(widget.default);
    $("#pnlCloseDash").html("<div class=\"close-fullscreen\">Close Full Screen</div>");
    $("#btnFullScr").on("click", fullScreen);
    $("#btnRefresh").on("click", renderTable);
    $("#pnlCloseDash > .close-fullscreen").on("click", fullScreenClose);

    renderTable();
    SimDms.onTenSecondChanged = function () { refreshData() };

    //moment.locale('id');

});

function renderTable() {

    var html = "";
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

          + "<tr style='line-height:24px'>"
          + "<td class='blank info2' id='CurrentMonth'>Oktober</td>"
          + "<td class=\"center3\"><span  class=\"animated\"  id='CCDO'>123</span></td>"
          + "<td class=\"blank info\">DO</td>"
          + "<td class='blank center'></td>"
          + "<td class=\"center3\"><span  class=\"animated\"  id='CCFP'>789</span></td>"
          + "<td class=\"blank info\">FP</td>"
          + "</tr>"

          + "<tr style='line-height:24px'>"
          + "<td class='blank info2' id='PreviousMonth'>September</td>"
          + "<td class=\"center3\"><span  class=\"animated\"  id='CPDO'>456</span></td>"
          + "<td class=\"center3\"><span class=\"animated\" id='CPCDO'>456</span></td>"
          + "<td class='blank center'></td>"
          + "<td class=\"center3\"><span  class=\"animated\"  id='CPFP'>789</span></td>"
          + "<td class=\"center3\"><span  class=\"animated\"  id='CPCFP'>123</span></td>"
          + "</tr>"


          + "<tr style='line-height:24px'>"
          + "<td class='blank info2' id='PreviousMonth2'>September</td>"
          + "<td class=\"center3\"><span  class=\"animated\"  id='CPDO2'>456</span></td>"
          + "<td class=\"center3\"><span class=\"animated\" id='CPCDO2'>456</span></td>"
          + "<td class='blank center'></td>"
          + "<td class=\"center3\"><span  class=\"animated\"  id='CPFP2'>789</span></td>"
          + "<td class=\"center3\"><span  class=\"animated\"  id='CPCFP2'>123</span></td>"
          + "</tr>"

          + "<tr style='line-height:8px'>"
          + "<td class='blank' style='line-height:8px'></td>"
          //+ "<td class='blank info2'></td>"
          //+ "<td class='blank center'></td>"
          //+ "<td class='blank center'></td>"
          //+ "<td class='blank info2'></td>"
          //+ "<td class='blank center'></td>"
          + "</tr>"

          + "<tr style='line-height:24px'>"
          + "<td class='blank info2'  id='CurrentMonth2'>Oktober</td>"
          + "<td class=\"center2\"><span  class=\"animated\"  id='CCIN'>123</span></td>"
          + "<td class=\"blank info\">INQ</td>"
          + "<td class='blank center'></td>"
          + "<td class=\"center2\"><span  class=\"animated\"  id='CCSPK'>789</span></td>"
          + "<td class='blank info'>SPK</td>"
          + "</tr>"

          + "<tr style='line-height:24px'>"
          + "<td class='blank info2' id='PreviousMonth3'>September</td>"
          + "<td class=\"center2\"><span  class=\"animated\" id='CPIN'>456</span></td>"
          + "<td class=\"center2\"><span  class=\"animated\" id='CPCPIN'>456</span></td>"
          + "<td class='blank center'></td>"
          + "<td class=\"center2\"><span  class=\"animated\" id='CPSPK'>789</span></td>"
          + "<td class=\"center2\"><span   class=\"animated\" id='CPCSPK'>123</span></td>"
          + "</tr>"

           + "<tr style='line-height:24px'>"
          + "<td class='blank info2' id='PreviousMonth4'>September</td>"
          + "<td class=\"center2\"><span  class=\"animated\" id='CPIN2'>456</span></td>"
          + "<td class=\"center2\"><span  class=\"animated\" id='CPCPIN2'>456</span></td>"
          + "<td class='blank center'></td>"
          + "<td class=\"center2\"><span  class=\"animated\" id='CPSPK2'>789</span></td>"
          + "<td class=\"center2\"><span  class=\"animated\"  id='CPCSPK2'>123</span></td>"
          + "</tr>"

          + "</table>";

    html = table;

    $("#pnlDashboard").html(html);
    refreshData();
}

function fullScreen() {
    $(".page > .body").addClass("fullscreen");
    $("#pnlDashboard").addClass("fullscreen");
    $("#btnFullScr").addClass("fullscreen");

    $(".page > .body.fullscreen").off();
    $(".page > .body.fullscreen").on("dblclick", fullScreenClose);

    widget.requestFullscreen();
}

function fullScreenClose() {
    $(".page > .body").removeClass("fullscreen");
    $("#pnlDashboard").removeClass("fullscreen");
    $("#btnFullScr").removeClass("fullscreen");

    widget.exitFullscreen();
}

function refreshData() {

    widget.xpost("wh.api/inquiry/PmExecSummary5", { param: "value" }, function (rows) {

        var data = rows[0];

        $("#CurrentMonth").text(moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate)
        $("#CurrentMonth2").text(moment(data.Current).format('MMM').toUpperCase() + ' ' + data.CurrDate)

        $("#PreviousMonth").text(moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate)
        $("#PreviousMonth2").text(moment(data.Previous).format('MMM').toUpperCase() + ' SUM')

        $("#PreviousMonth3").text(moment(data.Previous).format('MMM').toUpperCase() + ' ' + data.PrevDate)
        $("#PreviousMonth4").text(moment(data.Previous).format('MMM').toUpperCase() + ' SUM')
        
        console.log(data)

        if (data.CurrentDO == 0 || data.LastDateDO == 0)
            $("#CPCDO").text('-')
        else
            $("#CPCDO").text(NumDigit((data.CurrentDO / data.LastDateDO) * 100) + '%')

        if (data.CurrentFP == 0 || data.LastDateFP == 0)
            $("#CPCFP").text('-')
        else
            $("#CPCFP").text(NumDigit((data.CurrentFP / data.LastDateFP) * 100) + '%')

        if (data.CurrentInq == 0 || data.LastDateInq == 0)
            $("#CPCPIN").text('-')
        else
            $("#CPCPIN").text(NumDigit((data.CurrentInq / data.LastDateInq) * 100) + '%')

        if (data.CurrentSpk == 0 || data.LastDateSpk == 0)
            $("#CPCSPK").text('-')
        else
            $("#CPCSPK").text(NumDigit((data.CurrentSpk / data.LastDateSpk) * 100) + '%')

        $("#CCDO").text(NumDigit((data.CurrentDO)))
        $("#CPDO").text(NumDigit((data.LastDateDO)))
        $("#CCFP").text(NumDigit((data.CurrentFP)))
        $("#CPFP").text(NumDigit((data.LastDateFP)))

        $("#CCIN").text(NumDigit((data.CurrentInq)))
        $("#CPIN").text(NumDigit((data.LastDateInq)))
        $("#CCSPK").text(NumDigit((data.CurrentSpk)))
        $("#CPSPK").text(NumDigit((data.LastDateSpk)))

        $("#CPIN2").text(NumDigit((data.PreviousInq)))
        $("#CPCPIN2").text(NumDigit((data.CurrentInq / data.PreviousInq) * 100) + '%')
        $("#CPSPK2").text(NumDigit((data.PreviousSpk)))
        $("#CPCSPK2").text(NumDigit((data.CurrentSpk / data.PreviousSpk) * 100) + '%')

        $("#CPDO2").text(NumDigit((data.PreviousDO)))
        $("#CPCDO2").text(NumDigit((data.CurrentDO / data.PreviousDO) * 100) + '%')
        $("#CPFP2").text(NumDigit((data.PreviousFP)))
        $("#CPCFP2").text(NumDigit((data.CurrentFP / data.PreviousFP) * 100) + '%')

        $("#pnlDashboard .center3 > span.animated").addClass("fadeOutLeft");
        $("#pnlDashboard .center2 > span.animated").addClass("fadeOutLeft");

        setTimeout(function () {
            $("#pnlDashboard .center3 > span.animated").removeClass("fadeOutLeft");
            $("#pnlDashboard .center3 > span.animated").addClass("fadeInRight");
            $("#pnlDashboard .center2 > span.animated").removeClass("fadeOutLeft");
            $("#pnlDashboard .center2 > span.animated").addClass("fadeInRight");
        }, 500);

    });
}


