var company;
var widget = new SimDms.Widget({
    title: 'Generate Monitoring Per Outlet v2',
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
                { name: 'Area', text: 'Area', cls: "span4", type: "select", disabled:true },
                { type: "span" },
                { name: 'CompanyCode', text: 'Dealer', cls: "span4", type: "select", disabled: true },
                { type: "span" },
                { name: 'StartDate', text: 'From Date', cls: "span4", type: "datepicker", placeholder: 'DD-MMM-YYYY' },
                { type: "span" },
                { name: 'EndDate', text: 'To Date', cls: "span4", type: "datepicker", placeholder: 'DD-MMM-YYYY' },
                //{ type: "span" },
                //{ name: 'IncludeZero', text: 'Include Zero', cls: "span4", type: "check", lblstyle: 'style="visibility:hidden;"' },
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
    //$('#CompanyCode').prop('disabled', true);

    widget.post("wh.api/Combo/QaCompanyBranchV2", function (result) {
        var list = [];
        list = Enumerable.From(result[0]).ToArray();
        company = list[0].Company;

        widget.post("util.api/user/dealerlist", function (result) {
            widget.bind({
                name: 'CompanyCode',
                data: result,
                initialValue: list[0].CompanyCode
            });
        });  

        widget.post("wh.api/Combo/GroupAreas", function (result) {
            widget.bind({
                name: 'Area',
                data: result,
                initialValue: list[0].GroupNo
            });
        });
    });

    //widget.post("wh.api/Combo/QaCompanyBranch", function (result) {
    //    var list = [];
    //    list = Enumerable.From(result[0]).ToArray();

    //    $('#CompanyCode').val(list[0].CompanyCode);
    //});


    //$('#Area').on('change', function (e) {
    //    var gno = $('#Area').val();
    //    if (gno != '') {
    //        widget.post("wh.api/Combo/Companies", { "id": gno }, function (result) {
    //            widget.bind({
    //                name: 'CompanyCode',
    //                data: result,
    //                text: '-- Select All --',
    //            });
    //        });
    //        $('#CompanyCode').prop('disabled', false);
    //    }
    //    else {
    //        widget.post("wh.api/Combo/Companies", { "id": gno }, function (result) {
    //            widget.bind({
    //                name: 'CompanyCode',
    //                data: result,
    //                text: '-- Select All --',
    //            });
    //        });
    //        $('#CompanyCode').prop('disabled', true);
    //    }
    //});
});

function exportToExcel() {
    var url = "wh.api/Questionnaire/generateMonitoringPerOutletv2?";
    var filter = widget.serializeObject('pnlFilter');

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
    else {
        //filter.StartDate = $('input[name="StartDate"]').val();
        //filter.EndDate = $('input[name="EndDate"]').val();

        var params = ''

        //$.each(filter || [], function (key, val) {
        //    params += key + '=' + val + '&';
        //});
        //params = params.substring(0, params.length - 1);

        var 

        params = 'Area=' + filter.Area;
        params += '&CompanyCode=' + company;
        params += '&StartDate=' + filter.StartDate;
        params += '&EndDate=' + filter.EndDate;
        params += '&CompanyText=' + $('#CompanyCode option:selected').text();

        console.log(params)
        url += params;
        window.location = url;
    }
}