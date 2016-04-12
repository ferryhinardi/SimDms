var widget = new SimDms.Widget({
    title: 'Generate Monitoring Per Outlet',
    xtype: 'panels',
    toolbars: [
        { text: 'Export to Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o', cls: '' },
        { text: 'Expand', action: 'expand', icon: 'fa fa-expand' },
        { text: 'Collapse', action: 'collapse', icon: 'fa fa-compress', cls: 'hide' },
    ],
    panels: [
        {
            name: "pnlFilter",
            items: [
                { name: 'Event', text: 'Event', cls: "span4", type: "select", required: true },
                { type: "span" },
                { name: 'Area', text: 'Area', cls: "span4", type: "select" },
                { type: "span" },
                { name: 'CompanyCode', text: 'Dealer', cls: "span4", type: "select" },
                { type: "span" },
                { name: 'StartDate', text: 'From Date', cls: "span4", type: "datepicker", placeholder: 'DD-MMM-YYYY' },
                { type: "span" },
                { name: 'EndDate', text: 'To Date', cls: "span4", type: "datepicker", placeholder: 'DD-MMM-YYYY' },
                { type: "span" },
                { name: 'IncludeZero', text: 'Include Zero', cls: "span4", type: "check", lblstyle: 'style="visibility:hidden;"' },
            ]
        },
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'collapse':
                widget.exitFullWindow();
                widget.showToolbars(['expand', 'exportToExcel']);
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['collapse', 'exportToExcel']);
                break;
            case 'exportToExcel':
                exportToExcel();
                break;
            default:
                break;
        }
    },
});

widget.render(function () {
    //widget.setSelect([
    //        { name: "CompanyCode", url: "util.api/user/dealerlist" },
    //]);

    //widget.setSelect([
    //        { name: "Area", url: "wh.api/Combo/GroupAreas" },
    //]);

    var date = new Date(moment(moment().format('YYYY-MM')));
    var initial = { StartDate: date + '01', EndDate: new Date() };
    widget.populate(initial);

    $('#CompanyCode').prop('disabled', true);
    widget.post("wh.api/Combo/GroupAreas", function (result) {
        widget.bind({
            name: 'Area',
            data: result,
            text: '-- Select All --',
        });
    });

    //widget.post("util.api/user/dealerlist", function (result) {
    widget.post("wh.api/Combo/ComboDealerList", function (result) {
        widget.bind({
            name: 'CompanyCode',
            data: result,
            text: '-- Select All --',
        });
    });

    widget.bind({
        name: 'Event',
        data: [{ text: "Ertiga", value: "A" }, { text: "WagonR", value: "B" }, { text: "CBU", value: "C" }],
        text: '-- SELECT ONE --'
    });

    //widget.post("wh.api/Combo/QaCompanyBranch", function (result) {
    //    var list = [];
    //    list = Enumerable.From(result[0]).ToArray();

    //    $('#CompanyCode').val(list[0].CompanyCode);
    //});


    $('#Area').on('change', function (e) {
        var gno = $('#Area').val();
        if (gno != '') {
            //widget.post("wh.api/Combo/Companies", { "id": gno }, function (result) {
            widget.post("wh.api/Combo/ComboDealerList", { "GroupArea": gno }, function (result) {
                widget.bind({
                    name: 'CompanyCode',
                    data: result,
                    text: '-- Select All --',
                });
            });
            $('#CompanyCode').prop('disabled', false);
            $('#CompanyCode').select2('val', "");
        }
        else {
            //widget.post("wh.api/Combo/Companies", { "id": gno }, function (result) {
            widget.post("wh.api/Combo/ComboDealerList", { "GroupArea": gno }, function (result) {
                widget.bind({
                    name: 'CompanyCode',
                    data: result,
                    text: '-- Select All --',
                });
            });
            $('#CompanyCode').prop('disabled', true);
            $('#CompanyCode').select2('val', "");
        }
    });
});

function exportToExcel() {
    var url = "wh.api/Questionnaire2/generateMonitoringPerOutlet?";
    var filter = widget.serializeObject('pnlFilter');
    filter.CompanyText = $('#CompanyCode option:selected').text();

    if (typeof filter.StartDate == 'undefined' || filter.StartDate == '') {
        sdms.info({ type: "warning", text: "Tanggal mulai tidak boleh kosong" });
        //if ($errParamExport) $errParamExport.remove();
        //$errParamExport = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Tanggal akhir tidak boleh lebih besar dari tanggal mulai.</span>').insertBefore('.modal-footer');
    }
    else if (typeof filter.EndDate == 'undefined' || filter.EndDate == '') {
        sdms.info({ type: "warning", text: "Tanggal akhir tidak boleh kosong" });
        //if ($errParamExport) $errParamExport.remove();
        //$errParamExport = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Tanggal akhir tidak boleh lebih besar dari tanggal mulai.</span>').insertBefore('.modal-footer');
    }
    else if (Date.parse(filter.EndDate) - Date.parse(filter.StartDate) < 0) {
        sdms.info({ type: "warning", text: "Tanggal akhir tidak boleh lebih kecil dari tanggal mulai" });
        //if ($errParamExport) $errParamExport.remove();
        //$errParamExport = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Tanggal akhir tidak boleh lebih besar dari tanggal mulai.</span>').insertBefore('.modal-footer');
    }
    else if (filter.Event == '') {
        sdms.info({ type: "warning", text: "Event harus dipilih" });
        //if ($errParamExport) $errParamExport.remove();
        //$errParamExport = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Tanggal akhir tidak boleh lebih besar dari tanggal mulai.</span>').insertBefore('.modal-footer');
    }
    else {
        //filter.StartDate = $('input[name="StartDate"]').val();
        //filter.EndDate = $('input[name="EndDate"]').val();

        var params = ''

        $.each(filter || [], function (key, val) {
            params += key + '=' + val + '&';
        });
        params = params.substring(0, params.length - 1);

        url += params;
        window.location = url;
    }
}