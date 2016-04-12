$(document).ready(function () {
    var options = {
        title: "Generate ITS - Indent",
        xtype: "panels",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
        ],
        panels: [
            {
                name: "panelFilter",
                items: [
                    { name: "Date", type: "datepicker", cls: "span4 ", text: "Date" },
                ]
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {
        initElementEvents();
    }

    function initElementEvents() {
        var btnExportExcel = $('#btnExportExcel');

        btnExportExcel.off();
        btnExportExcel.on('click', function () {
            window.location = SimDms.baseUrl + 'wh.api/InquiryProd/GenerateITSIndent?Date=' + $('[name=Date]').val();
        });
    }
});