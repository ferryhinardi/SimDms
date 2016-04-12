var widget = new SimDms.Widget({
    title: 'Promotion Data',
    xtype: 'panels',
    panels: [
        {
            name: "pnlFilter",
            items: [
                    {
                        text: "Periode",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'DateFrom', text: 'Date From', type: 'datepicker', cls: 'span2' },
                            { name: 'DateTo', text: 'Date To', type: 'datepicker', cls: 'span2' },
                        ]
                    },
            ]
        },
        {
            name: "pnlResult",
            xtype: "k-grid",
        },
    ],
    toolbars: [
        { name: 'Refresh', text: "Refresh", icon: "fa fa-refresh" },
        { name: 'ExportXls', text: "Export (Xls)", icon: "fa fa-xls" },
    ],
});

widget.default = {
    DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
    DateTo: new Date()
};

widget.render(function () {

    widget.select({ name: "BranchCode", url: "cs.api/Combo/ListBranchCode" });
    $('#Refresh').on('click', refreshGrid);
    $("#btnExportXls").on("click", exportToExcel);
    widget.populate(widget.default);

    var detail = '<script type="text/x-kendo-template" id="template">\
                    <div id="pnlDetail"></div>\
                  </script>';

    $('#pnlResult').parent().append(detail);

});


function refreshGrid() {
    var filter = widget.serializeObject('pnlFilter');
    var slv = 0, gld = 0, plt = 0, sh = 0, bm = 0;
    var waitToShow = false;

    widget.kgrid({
        url: 'ab.api/Inquiry/Promotion',
        name: "pnlResult",
        params: filter,
        selectable: "row",
        serverBinding: true,
        sort: [{ field: "Outlet", dir: "asc" }],
        detailTemplate: kendo.template($("#template").html()),
        columns: [
            { field: "OutletCode", hidden: true },
            { field: "Outlet", width: 200, title: "Outlet", footerTemplate: 'TOTAL', type: 'align-left' },
            { field: "Silver", width: 130, title: "Trainee to Silver", footerTemplate: '', type: 'align-right' },
            { field: "Gold", width: 150, title: "Silver to Gold", footerTemplate: '', type: 'align-right' },
            { field: "Platinum", width: 150, title: "Gold to Platinum", footerTemplate: '', type: 'align-right' },
            { field: "SH", width: 150, title: "Sales Person to Sales Head", footerTemplate: '', type: 'align-right' },
            { field: "BM", width: 170, title: "Sales Head to Branch Manager", footerTemplate: '', type: 'align-right' }
        ],
        change: function () {
            $(".kgrid #pnlResult .k-grid-content tr a.k-minus").click();

            var filter2 = filter;
            filter2.BranchCode = $(".kgrid #pnlResult .k-grid-content tr.k-state-selected td:eq(1)").text();
            filter2.PromosiType = $(".kgrid #pnlResult .k-grid-content tr.k-state-selected td#pnlResult_active_cell").index();

            if (filter2.PromosiType < 3)
                return;

            var a = $(".kgrid #pnlResult .k-grid-content tr.k-state-selected td:first-child").children('a');

            if (a.hasClass('k-plus')) { a.click(); }

            widget.kgrid({
                url: "ab.api/Inquiry/PromotionDetail",
                name: "pnlDetail",
                params: filter2,
                selectable: "row",
                serverBinding: true,
                sort: [
                    { field: "OutletName", dir: "asc" },
                    { field: "Name", dir: "asc" },
                ],
                columns: [
                    { field: "OutletName", width: 100, title: "Outlet" },
                    { field: "Name", width: 200, title: "Sales Name" },
                    { field: "Position", width: 100, title: "Position" },
                    { field: "Grade", width: 100, title: "Grade" },
                    { field: "Joindate", width: 100, title: "Joindate" },
                ],
            });

        },
        onComplete: function () {
            $('#pnlResult').find('tr.k-footer-template').children('td:eq(2)').css('text-align', 'center');
            //while (!waitToShow && timeToWait > 0) {
            //    setTimeout(function () {
            //        timeToWait -= 1;
            //    }, 50);
            //}
            if (!waitToShow) {
                $.ajax({
                    url: 'ab.api/Inquiry/PromotionTotal',
                    type: "POST",
                    data: filter,
                    dataType: 'JSON',
                    async: true,
                    success: function (response) {
                        if (response != undefined) {
                            slv = response.Silver;
                            gld = response.Gold;
                            plt = response.Platinum;
                            sh = response.SH;
                            bm = response.BM;
                            waitToShow = true;
                            $('#pnlResult').find('tr.k-footer-template').children('td:eq(3)').text(slv);
                            $('#pnlResult').find('tr.k-footer-template').children('td:eq(4)').text(gld);
                            $('#pnlResult').find('tr.k-footer-template').children('td:eq(5)').text(plt);
                            $('#pnlResult').find('tr.k-footer-template').children('td:eq(6)').text(sh);
                            $('#pnlResult').find('tr.k-footer-template').children('td:eq(7)').text(bm);
                        }
                    }
                });
            }
            else {
                $('#pnlResult').find('tr.k-footer-template').children('td:eq(3)').text(slv);
                $('#pnlResult').find('tr.k-footer-template').children('td:eq(4)').text(gld);
                $('#pnlResult').find('tr.k-footer-template').children('td:eq(5)').text(plt);
                $('#pnlResult').find('tr.k-footer-template').children('td:eq(6)').text(sh);
                $('#pnlResult').find('tr.k-footer-template').children('td:eq(7)').text(bm);
            }
            //return
        }
    });

}

function exportToExcel() {
    var params = widget.serializeObject('pnlFilter');
    var url = "ab.api/Inquiry/PromotionExcel";

    $('#btnExportXls').attr('disabled', 'disabled');
    sdms.info("Please wait...");
    $.ajax({
        async: true,
        type: "POST",
        data: params,
        url: url,
        success: function (data) {
            if (data.message == "") {
                console.log(data);
                location.href = 'wh.api/Report/DownloadExcelFile?key=' + data.value;
            } else {
                sdms.info(data.message, "Error");
            }
        }
    });
    $('#btnExportXls').removeAttr('disabled');
}
