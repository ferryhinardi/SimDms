$(document).ready(function () {
    var options = {
        title: "SDMS Last Update",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "CompanyCode", text: "Organization", cls: "span6", type: "select" },
                    //{ name: "DataType", text: "Data Type", cls: "span4", type: "select" },
                    {
                        name: "DataStatus", text: "Status", cls: "span3", type: "select", opt_text: "ALL STATUS",
                        items: [
                            { text: "A - Initial", value: "A" },
                            { text: "P - Processed", value: "P" },
                            { text: "X - Data Fail", value: "X" },
                        ]
                    },
                ]
            },
            {
                name: "KGrid",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "CompanyCode", url: "wh.api/combo/organizations", optionalText: "-- SELECT ALL --" },
        //{ name: "DataType", url: "util.api/combo/datatypes", optionalText: "-- SELECT ALL --" }
    ]);
    widget.render(function () {
        $("#btnRefresh").on("click", refreshGrid);
        $("select").on("change", refreshGrid);
        refreshGrid();
    });

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();
        widget.kgrid({
            url: "util.api/inquiry/lastupdate",
            name: "KGrid",
            params: params,
            pageSize: 1500,
            pageable: false,
            sort: [{ field: "UpdatedDate", dir: "desc" }],
            columns: [
                { field: "CompanyCode", width: 100, title: "Company Code" },
                { field: "CompanyName", width: 300, title: "Company Name" },
                { field: "DataType", width: 100, title: "Data Type" },
                { field: "LastSendDate", width: 180, title: "Send Date", template: "#= (LastSendDate == undefined) ? '-' : moment(LastSendDate).format('DD MMM YYYY  HH:mm:ss') #" },
                { field: "UpdatedDate", width: 180, title: "Updated Date", template: "#= (UpdatedDate == undefined) ? '-' : moment(UpdatedDate).format('DD MMM YYYY  HH:mm:ss') #" },
            ],
        });
    }
});