$(document).ready(function () {
    var options = {
        title: "Unit & Revenue Service Target",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "PeriodYear", text: "Period Year", cls: "span6", required: true },
                ],
            },
        ],
        toolbars: [
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { name: "btnExportXls", action: 'export', text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'export', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'export', 'collapse']);
                    break;
                default:
                    break;
            }
        },
    }
    var widget = new SimDms.Widget(options);
    widget.render(function () {
        
    });

    function refreshGrid() {
        
    }
});