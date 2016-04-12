$(document).ready(function () {
    var options = {
        title: "Generate ITS With Status and Test Drive",
        xtype: "panels",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
        ],
        panels: [
            {
                name: "panelFilter",
                items: [
                    { name: "StartDate", type: "datepicker", cls: "span4 ", text: "Start Date" },
                    { name: "EndDate", type: "datepicker", cls: "span4 ", text: "End Date" },
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
            window.location.href = 'wh.api/InquiryProd/GenerateITSWithStatusAndTestDrive?StartDate=' + $('[name=StartDate]').val() + '&EndDate=' + $('[name=EndDate]').val();
        });
    }
});