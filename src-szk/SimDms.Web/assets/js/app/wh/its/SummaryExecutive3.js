var widget = new SimDms.Widget({
    title: "Executive Summary by Month",
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
});

function renderTable() {
    var html = "";
    var table = "";

    table = "<table class=\"dashboard big\">"

          + "<tr>"
          + "<th style='width:15%' class='blank info' rowspan='7'>INDENT</th>"
          + "<th style='width:25%' class='blank center'></th>"
          + "<th style='width:15%' class='title center'> < M - 1 </th>"
          + "<th style='width:15%' class='title center'> M - 1 </th>"
          + "<th style='width:15%' class='title center'> M </th>"
          + "<th style='width:15%' class='title center'> Total </th>"
          + "</tr>"

          + "<tr>"
          + "<td class='strong title'>Ertiga</td>"
          + "<td class=\"number\"><span class=\"animated\" id='R1C1'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R1C2'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R1C3'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R1C4'></span></td>"
          + "</td>"

          + "<tr>"
          + "<td class='strong title'>Karimun Wagon R</td>"
          + "<td class=\"number\"><span class=\"animated\" id='R2C1'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R2C2'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R2C3'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R2C4'></span></td>"
          + "</td>"

          + "<tr>"
          + "<td class='strong title'>PU Futura</td>"
          + "<td class=\"number\"><span class=\"animated\" id='R3C1'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R3C2'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R3C3'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R3C4'></span></td>"
          + "</td>"

          + "<tr>"
          + "<td class='strong title'>PU Mega Carry</td>"
          + "<td class=\"number\"><span class=\"animated\" id='R4C1'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R4C2'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R4C3'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R4C4'></span></td>"
          + "</td>"

          + "<tr>"
          + "<td class='strong title'>Others</td>"
          + "<td class=\"number\"><span class=\"animated\" id='R5C1'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R5C2'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R5C3'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R5C4'></span></td>"
          + "</td>"

          + "<tr>"
          + "<td class='strong title'>All Model</td>"
          + "<td class=\"number\"><span class=\"animated\" id='R6C1'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R6C2'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R6C3'></span></td>"
          + "<td class=\"number\"><span class=\"animated\" id='R6C4'></span></td>"
          + "</td>"

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
    widget.xpost("wh.api/inquiry/PmExecSummaryByMonth", { name: "PmExecutiveSummary2" }, function (rows) {
        rows.forEach(function (row) {
            $("#" + row.FieldRow + row.FieldCol).text(widget.numberFormat(row.FieldVal));
        });

        $("#pnlDashboard .number > span.animated").addClass("fadeOutLeft");

        setTimeout(function () {
            console.log("fefresh", new Date());
            $("#pnlDashboard .number > span.animated").removeClass("fadeOutLeft");
            $("#pnlDashboard .number > span.animated").addClass("fadeInRight");
        }, 500);
    });
}


