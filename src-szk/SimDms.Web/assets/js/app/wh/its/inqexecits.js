var widget = new SimDms.Widget({
    title: "ITS Data Received",
    xtype: "panels",
    toolbars: [
        { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
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
    $("#btnRefresh").on("click", refreshTable);
    SimDms.onTenSecondChanged = refreshData;
    refreshTable();
});

function refreshData() {
    widget.xpost("wh.api/inquiry/PmDataIts", renderTable);
}

function refreshTable() {
    widget.post("wh.api/inquiry/PmDataIts", renderTable);
}

function renderTable(docs) {
    var html = "";
    var th = "";
    var td = "";

    if (docs.length > 0) {
        th += "<tr>";
        th += "<th class='title'>Company Code</th>";
        th += "<th class='title'>Company Name</th>";
        th += "<th class='title'>Data Type</th>";
        th += "<th class='title'>Delay Day</th>";
        th += "</tr>";
    }

    for (var i in docs) {
        var doc = docs[i];
        td += "<tr>";
        td += "<td>" + doc["CompanyCode"] + "</td>";
        td += "<td>" + doc["CompanyName"] + "</td>";
        td += "<td>" + doc["DataType"] + "</td>";
        td += "<td class=\"number\"><span class='animated'>" + doc["DelayDate"] + "</span></td>";
        td += "</tr>";
    }

    html = "<table class=\"dashboard\">" + th + td + "</table>";

    setTimeout(function () {
        $("#pnlDashboard").html(html);
        setTimeout(function () {
            //$("#pnlDashboard").html(table1 + table2);
            $("#pnlDashboard .number > span.animated").addClass("fadeInRight");
        }, 500);
    }, 60*1000);
}