$(document).ready(function () {
    var options = {
        title: "Generate ITS By Closing Date",
        xtype: "panels",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
        ],
        panels: [
            {
                name: "panelFilter",
                items: [
                    { name: "Year", type: "select", cls: "span4 full ", text: "Tahun" },
                    { name: "Month", type: "select", cls: "span4 full ", text: "Bulan" },
                    { name: "Type", type: "select", cls: "span6 full ", text: "Generate ITS" },
                ]
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {
        initElementEvents();
        widget.select({ selector: "select[name=Year]", url: "wh.api/inquiryprod/Years" });
        widget.select({ selector: "select[name=Month]", url: "wh.api/inquiryprod/Months" });
        widget.select({ selector: "select[name=Type]", url: "wh.api/GenerateITSToExcel/GenerateITS" });
    }

    function initElementEvents() {
        var btnExportExcel = $('#btnExportExcel');

        btnExportExcel.off();
        btnExportExcel.on('click', function () {
            //if ($('[name=Type]').val() == 0) {
            //    console.log($('[name=Type]').val());
                window.location.href = 'wh.api/GenerateITSToExcel/GenerateITSWithStatusAndTestDrive?Year=' + $('[name=Year]').val() + '&Month=' + $('[name=Month]').val() + '&Type=' + $('[name=Type]').val();             
            //}
        });
    }

    //$("#btnExportExcel").on("click", function (e) {

    //    var params = widget.serializeObject('pnlFilter');
    //    params.Year = $('[name=Year] option:selected').val();
    //    params.Month = $('[name=Month] option:selected').val();

    //    e.preventDefault();
    //    $('.page > .ajax-loader').show();

    //    $.fileDownload('doreport/GenerateITSToExcel.xlsx', {
    //        httpMethod: "POST",
    //        //preparingMessageHtml: "We are preparing your report, please wait...",
    //        //failMessageHtml: "There was a problem generating your report, please try again.",
    //        data: params
    //    }).done(function () {
    //        $('.page > .ajax-loader').hide();
    //    });

    //});
});