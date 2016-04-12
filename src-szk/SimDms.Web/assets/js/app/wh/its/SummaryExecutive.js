var widget = new SimDms.Widget({
    title: "ITS Executive Summary",
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
    ]
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
    $("#btnRefresh").on("click", refreshTable);
    $("#pnlCloseDash > .close-fullscreen").on("click", fullScreenClose);

    SimDms.onTenSecondChanged = function () { refreshData(); }
    refreshTable();
});

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
    widget.xpost("wh.api/inquiry/PmExecSummary", renderTable);
}

function refreshTable() {
    widget.post("wh.api/inquiry/PmExecSummary", renderTable);
}

function renderTable(docs) {
    var html = "";
    var table1 = "";
    var table2 = "";

    for (var i in docs.header) {
        var th = docs.header[i];
        html += "<tr>";
        if (i == 0) {
            html += "<th rowspan=" + (docs.header.length || 1) + " class=\"span30 blank info\">INDENT</th>";
        }
        html += "<th colspan=3 class=\"span35 title\">" + th.FieldDesc + "</th>";
        html += "<th colspan=3 class=\"span35 number\">" + widget.numberFormat(th.InqValAll) + "</th>";
        html += "</tr>";
    }

    table1 = "<table class=\"dashboard\">" + html + "</table>";

    html = "";
    html += "<tr>";
    html += "<th rowspan=2 class=\"span30 title blank\">&nbsp;</th>";
    html += "<th colspan=3 class=\"span35 title center\">Inquiry</th>";
    html += "<th colspan=3 class=\"span35 title center\">SPK</th>";
    html += "</tr>";
    html += "<tr>";
    html += "<th class=\"number title\">Avg L/M</th>";
    html += "<th class=\"number title\">D - 1</th>";
    html += "<th class=\"number title\">%</th>";
    html += "<th class=\"number title\">Avg L/M</th>";
    html += "<th class=\"number title\">D</th>";
    html += "<th class=\"number title\">%</th>";
    html += "</tr>";
    (docs.detail).forEach(function (td) {
        html += "<tr>";
        html += "<td class=\"strong title\">" + td.FieldDesc + "</td>";
        html += "<td class=\"number\"><span class=\"animated\">" + widget.numberFormat(td.InqValue1) + "<span></td>";
        html += "<td class=\"number\"><span class=\"animated\">" + widget.numberFormat(td.InqValue2) + "<span></td>";
        html += "<td class=\"number\"><span class=\"animated\">" + widget.numberFormat((td.InqValue2 / (1.0 * td.InqValue1)) * 100, 1) + "<span></td>";
        html += "<td class=\"number\"><span class=\"animated\">" + widget.numberFormat(td.SpkValue1) + "<span></td>";
        html += "<td class=\"number\"><span class=\"animated\">" + widget.numberFormat(td.SpkValue2) + "<span></td>";
        html += "<td class=\"number\"><span class=\"animated\">" + widget.numberFormat((td.SpkValue2 / (1.0 * td.SpkValue1)) * 100, 1) + "<span></td>";
        html += "</tr>";
    });

    table2 = "<table class=\"dashboard\">" + html + "</table>";

    $("#pnlDashboard .number > span.animated").addClass("fadeOutLeft");
    setTimeout(function () {
        console.log("fefresh", new Date());
        $("#pnlDashboard").html(table1 + table2);
        $("#pnlDashboard .number > span.animated").addClass("fadeInRight");
    }, 500);
}