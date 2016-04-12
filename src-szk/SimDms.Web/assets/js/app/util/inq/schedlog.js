$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Schduler Log",
        xtype: "panels",
        toolbars: [
            { action: 'refresh', text: 'Refresh', icon: 'fa fa-refresh' },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [{ name: 'DealerCode', text: 'Dealer', type: 'select', cls: 'span4' }]
            },
            {
                name: "pnlResult",
                xtype: "k-grid",
            },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'refresh':
                    refresh();
                    break;
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'collapse']);
                    break;
                case 'exportexcel':
                    exportToExcel();

                default:
                    break;
            }
        },
    });

    widget.render(function () {
        widget.post("wh.api/combo/GnSchedulerLogFilter", {}, function (result) {
            widget.bind({
                name: 'DealerCode',
                text: '-- SELECT DEALER --',
                data: result[0],
                onChange: refresh
            });
        });

        setTimeout(refresh, 1000);
    });


    function refresh() {
        var filter = widget.serializeObject('pnlFilter');
        widget.kgrid({
            url: "wh.api/inquiry/GnScheduleLog",
            name: "pnlResult",
            params: filter,
            sort: [
                { field: "DealerCode", dir: "asc" },
                { field: "SchedulerName", dir: "asc" },
            ],
            columns: [
                { field: "DealerCode", width: 120, title: "Dealer Code" },
                { field: "DealerName", width: 300, title: "Dealer Name" },
                { field: "ScheduleName", width: 200, title: "Schedule Name" },
                { field: "DateStart", width: 160, title: "Date Start", type: "datetime" },
                { field: "DateFinish", width: 160, title: "Date Finish", type: "datetime" },
                { field: "RunningCount", width: 120, title: "Iterate", type: "number" },
                { field: "IsError", width: 100, title: "Is Error" },
                { field: "Info", width: 400, title: "Info" },
                { field: "ErrorMessage", width: 620, title: "Error Message" },
            ],
            detailInit: detailInit
        });

        function detailInit(e) {
            console.log(e);
            widget.post("wh.api/inquiry/GnScheduleLogDetails", { DealerCode: e.data.DealerCode, ScheduleName: e.data.ScheduleName, DateStart: moment(widget.cleanJsonDate(e.data.DateStart)).format('YYYY-MM-DD') }, function (data) {
                if (data.length > 0) {
                    $("<div/>").appendTo(e.detailCell).kendoGrid({
                        dataSource: { data: data, pageSize: 10 },
                        pageable: true,
                        columns: [
                            { field: "DealerCode", width: 80, title: "Dealer Code" },
                            { field: "ScheduleName", width: 120, title: "Schedule Name" },
                            { field: "DateStart", width: 80, title: "Date Start", type: "datetime", template: "#= ((DateStart === undefined) ? \"\" : moment(DateStart).format('DD MMM YYYY hh:mm:ss')) #" },
                            { field: "DateFinish", width: 80, title: "Date Finish", type: "datetime", template: "#= ((DateFinish === undefined) ? \"\" : moment(DateFinish).format('DD MMM YYYY hh:mm:ss')) #" },
                            { field: "IsError", width: 50, title: "Is Error", template: "#= ((IsError === true) ? \"Yes\" : \"No\") #" },
                            { field: "ErrorMessage", width: 260, title: "Error Message" },
                            { field: "Info", width: 600, title: "Info" },
                        ]
                    });
                }
                else {
                    $("<div/>").appendTo(e.detailCell).kendoGrid({
                        dataSource: { data: [{ Info: "History data tidak ditemukan." }] },
                        columns: [{ field: "Info", title: "Info" }]
                    });
                }
            })
        }
    }
});
