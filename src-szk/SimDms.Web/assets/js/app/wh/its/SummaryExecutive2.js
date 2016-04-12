var widget = new SimDms.Widget({
    title: "Executive Summary Report (2)",
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
    var table1 = "";
    var table2 = "";

    table1 = "<table class=\"dashboard\">"

           + "<tr>"
           + "<th style='width:25%' class='blank info' rowspan='4'>INDENT</th>"
           + "<th style='width:20%' class='header title'>Ertiga</th>"
           + "<th style='width:15%' class='header number' id='H1'></th>"
           + "<th style='width:5%'  class='header blank'></th>"
           + "<th style='width:20%' class='header title'>PU Futura</th>"
           + "<th style='width:15%' class='header number' id='H3'></th>"
           + "</tr>"

           + "<tr>"
           + "<th class='header title'>Karimun Wagon R</th>"
           + "<th class='header number' id='H2'></th>"
           + "<th class='header blank'></th>"
           + "<th class='header title'>PU Mega Carry</th>"
           + "<th class='header number' id='H4'></th>"
           + "</tr>"

           + "<tr>"
           + "<th class='header title'>CELERIO</th>"
           + "<th class='header number' id='H7'></th>"
           + "<th class='header blank'></th>"
           + "<th class='header title'>Others</th>"
           + "<th class='header number' id='H5'></th>"
           + "</tr>"

           + "<tr>"
           + "<th class='header title'></th>"
           + "<th class='header number' id='H100'></th>"
           + "<th class='header blank'></th>"
           + "<th class='header title'>All Model</th>"
           + "<th class='header number' id='H6'></th>"
           + "</tr>"

           + "</table>";

    table2 = "<table class=\"dashboard\">"

           + "<tr>"
           + "<th style='width:25%' class='blank center' rowspan='2'></th>"
           + "<th style='width:25%' class='title center' colspan='3'>Inquiry</th>"
           + "<th style='width:25%' class='title center' colspan='3'>SPK</th>"
           + "<th style='width:25%' class='title center' colspan='3'>Faktur</th>"
           + "</tr>"

           + "<tr>"
           + "<th style='width:8.33%' class='title small'>AVG L/M</th>"
           + "<th style='width:8.33%' class='title small'>D-31 <br/> s/d D-2</th>"
           + "<th style='width:8.33%' class='title small'>D-1</th>"
           + "<th style='width:8.33%' class='title small'>AVG L/M</th>"
           + "<th style='width:8.33%' class='title small'>D-30 <br/> s/d D-1</th>"
           + "<th style='width:8.33%' class='title small'>D</th>"
           + "<th style='width:8.33%' class='title small'>AVG L/M</th>"
           + "<th style='width:8.33%' class='title small'>D-30 <br/> s/d D-1</th>"
           + "<th style='width:8.33%' class='title small'>D</th>"
           + "</tr>"

           + "<tr>"
           + "<td class='strong title'>Ertiga</td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI11'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI21'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI31'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS11'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS21'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS31'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF11'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF21'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF31'></span></td>"
           + "</td>"

           + "<tr>"
           + "<td class='strong title'>Karimun Wagon R</td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI12'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI22'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI32'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS12'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS22'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS32'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF12'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF22'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF32'></span></td>"
           + "</td>"


           + "<tr>"
           + "<td class='strong title'>Celerio</td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI17'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI27'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI37'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS17'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS27'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS37'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF17'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF27'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF37'></span></td>"
           + "</td>"


           + "<tr>"
           + "<td class='strong title'>PU Futura</td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI13'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI23'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI33'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS13'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS23'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS33'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF13'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF23'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF33'></span></td>"
           + "</td>"

           + "<tr>"
           + "<td class='strong title'>PU Mega Carry</td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI14'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI24'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI34'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS14'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS24'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS34'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF14'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF24'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF34'></span></td>"
           + "</td>"

           + "<tr>"
           + "<td class='strong title'>Others</td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI15'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI25'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI35'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS15'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS25'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS35'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF15'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF25'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF35'></span></td>"
           + "</td>"

           + "<tr>"
           + "<td class='strong title'>All Model</td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI16'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI26'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DI36'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS16'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS26'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DS36'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF16'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF26'></span></td>"
           + "<td class=\"number\"><span class=\"animated\" id='DF36'></span></td>"
           + "</td>"

           + "</table>";

    html = table1 + table2;

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
    widget.xpost("wh.api/inquiry/PmDashboardData", { name: "PmExecutiveSummary2" }, function (rows) {
        rows.forEach(function (row) {
            $("#" + row.GroupType + row.GroupSeq).text(widget.numberFormat(row.DataCount));
        });

        $("#pnlDashboard .number > span.animated").addClass("fadeOutLeft");
        setTimeout(function () {
            console.log("fefresh", new Date());
            $("#pnlDashboard .number > span.animated").removeClass("fadeOutLeft");
            $("#pnlDashboard .number > span.animated").addClass("fadeInRight");
        }, 500);
    });
}


