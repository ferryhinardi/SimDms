var widget = new SimDms.Widget({
    title: 'Generate Row Data Dealer',
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
                { name: 'CompanyCode', text: 'Dealer', cls: "span4", type: "select", disabled: true },
                { type: "span" },
                { name: 'StatusKonsumen', text: 'Status Konsumen', cls: "span4", type: "select" },
                { type: "span" },
                { name: 'Event', text: 'Event', cls: "span4", type: "select", required: true },
                { type: "span" },
                //{ name: 'BranchCode', text: 'Outlet', cls: "span4", type: "select" },
                //{ type: "span" },
                { name: 'StartDate', text: 'From Date', cls: "span4", type: "datepicker", placeholder: 'DD-MMM-YYYY' },
                { type: "span" },
                { name: 'EndDate', text: 'To Date', cls: "span4", type: "datepicker", placeholder: 'DD-MMM-YYYY' },
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
    widget.post("wh.api/Combo/QaCompanyBranch", function (result) {
        var list = [];
        list = Enumerable.From(result[0]).ToArray();

        widget.post("util.api/user/dealerlist", function (result) {
            widget.bind({
                name: 'CompanyCode',
                data: result,
                initialValue: list[0].CompanyCode
            });
        });

    });

    widget.bind({
        name: 'StatusKonsumen',
        data: [{ text: "Individu", value: "A" }, { text: "Fleet/Perusahaan", value: "B" }],
        text: '-- SELECT ALL --'
    });

    widget.bind({
        name: 'Event',
        data: [{ text: "Ertiga", value: "A" }, { text: "WagonR", value: "B" }, { text: "CBU", value: "C" }],
        text: '-- SELECT ONE --'
    });

    //$('#CompanyCode').on('change', function (e) {
    //    var dealerCode = $('#CompanyCode').val();
    //    if (dealerCode != '') {
    //        widget.select({ selector: "#BranchCode", url: "util/user/outletlist?dealerCode=" + dealerCode });
    //        $('#BranchCode').removeAttr('disabled');
    //    }
    //    else {
    //        widget.select({ selector: "#BranchCode", url: "util/user/outletlist?dealerCode=" + dealerCode });
    //        $('#BranchCode').attr('disabled', 'disabled');
    //    }
    //});
});

function exportToExcel() {
    var url = "wh.api/Questionnaire2/generateQaRowDataDealer?";
    var filter = widget.serializeObject('pnlFilter');

    if (typeof filter.CompanyCode == 'undefined' || filter.CompanyCode == '') {
        sdms.info({ type: "warning", text: "Harap pilih Dealer" });
        //if ($errParamExport) $errParamExport.remove();
        //$errParamExport = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Tanggal akhir tidak boleh lebih besar dari tanggal mulai.</span>').insertBefore('.modal-footer');
    }
        //else if (typeof filter.BranchCode == 'undefined' || filter.BranchCode == '') {
        //    sdms.info({ type: "warning", text: "Harap pilih Outlet" });
        //    //if ($errParamExport) $errParamExport.remove();
        //    //$errParamExport = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Tanggal akhir tidak boleh lebih besar dari tanggal mulai.</span>').insertBefore('.modal-footer');
        //}
    else if (typeof filter.StartDate == 'undefined' || filter.StartDate == '') {
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