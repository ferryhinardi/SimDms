$(document).ready(function () {
    var options = {
        title: "Manpower Dashboard",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Periode",
                        type: "controls",
                        items: [
                            { name: "PeriodeBeg", text: "", cls: "span2", type: "datepicker" },
                            { text: " To ", cls: "span1", type: "label" },
                            { name: "PeriodeEnd", text: "To", cls: "span2", type: "datepicker" },
                        ]
                    },
                ],
            },
            { name: "pnlResult", xtype: "k-grid" },
        ],
        toolbars: [
            { name: 'Refresh', text: "Refresh", icon: "fa fa-refresh" },
            { name: 'ExportXls', text: "Export (Xls)", icon: "fa fa-xls" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "Position", url: "ab.api/combo/positions", params: { dept: "SALES" }, optionalText: "-- SELECT ALL --" }]);
    widget.default = { Status: "1", PeriodeBeg: new Date(), PeriodeEnd: new Date() };

    widget.render(function () {
        widget.populate(widget.default);
        $("#Refresh").off().on("click", refreshGrid);
        $("#ExportXls").off().on("click", exportXls);

        var detail = '<script type="text/x-kendo-template" id="template">\
                    <div id="pnlDetail"></div>\
                  </script>';

        $('#pnlResult').parent().append(detail);

        //$("#btnExportXls").on("click", exportXls);
        //$("#CutOff").val(new Date());
        // $("select[name=Position],select[name=Status], select[name=BranchCode]").on("change", refreshGrid);
        // $("#pnlFilter select").on("change", refreshGrid);
        // setTimeout(function () { $("#pnlFilter [name=CompanyCode]").change() }, 500);
    });

    function refreshGrid() {
        var params = {
            start: $("[name=PeriodeBeg]").val(),
            end: $("[name=PeriodeEnd]").val(),
        }

        var bm = 0, sh = 0, plt = 0, gld = 0, slv = 0, trn = 0, tsp = 0, tsf = 0;
        var waitToShow = false;

        widget.kgrid({
            url: 'ab.api/sfinfo/query',
            name: "pnlResult",
            params: params,
            selectable: "row",
            serverBinding: true,
            sort: [{ field: "Outlet", dir: "asc" }],
            detailTemplate: kendo.template($("#template").html()),
            columns: [
               { field: "Outlet", width: 300, title: "Outlet", footerTemplate: 'Total :' },
               { field: "BranchManager", width: 140, title: "Branch Manager", footerTemplate: '', type: 'align-right' },
               { field: "SalesHead", width: 140, title: "Sales Head", footerTemplate: '', type: 'align-right' },
               { field: "Platinum", width: 140, title: "Sales Platinum", footerTemplate: '', type: 'align-right' },
               { field: "PlatinumPct", width: 140, title: "%", footerTemplate: '', type: 'align-right' },
               { field: "Gold", width: 140, title: "Sales Gold", footerTemplate: '', type: 'align-right' },
               { field: "GoldPct", width: 140, title: "%", footerTemplate: '', type: 'align-right' },
               { field: "Silver", width: 140, title: "Sales Silver", footerTemplate: '', type: 'align-right' },
               { field: "SilverPct", width: 140, title: "%", footerTemplate: '', type: 'align-right' },
               { field: "Trainee", width: 140, title: "Sales Trainee", footerTemplate: '', type: 'align-right' },
               { field: "TraineePct", width: 140, title: "Sales Trainee", footerTemplate: '', type: 'align-right' },
               { field: "TotalSalesPerson", width: 140, title: "Total Sales Person", footerTemplate: '', type: 'align-right' },
               { field: "TotalSalesForce", width: 140, title: "Total Sales Force", footerTemplate: '', type: 'align-right' },
            ],
            onComplete: function () {
                $('#pnlResult').find('tr.k-footer-template').children('td:eq(1)').css('text-align', 'center');
                if (!waitToShow) {
                    $.ajax({
                        url: 'ab.api/sfinfo/Total',
                        type: "POST",
                        data: params,
                        dataType: 'JSON',
                        async: true,
                        success: function (response) {
                            if (response != undefined) {
                                bm = response.BranchManager;
                                sh = response.SalesHead;
                                plt = response.Platinum;
                                gld = response.Gold;
                                slv = response.Silver;
                                trn = response.Trainee;
                                tsp = response.TotalSalesPerson;
                                tsf = response.TotalSalesForce;
                                waitToShow = true;
                                $('#pnlResult').find('tr.k-footer-template').children('td:eq(2)').text(bm);
                                $('#pnlResult').find('tr.k-footer-template').children('td:eq(3)').text(sh);
                                $('#pnlResult').find('tr.k-footer-template').children('td:eq(4)').text(plt);
                                $('#pnlResult').find('tr.k-footer-template').children('td:eq(6)').text(gld);
                                $('#pnlResult').find('tr.k-footer-template').children('td:eq(8)').text(slv);
                                $('#pnlResult').find('tr.k-footer-template').children('td:eq(10)').text(trn);
                                $('#pnlResult').find('tr.k-footer-template').children('td:eq(12)').text(tsp);
                                $('#pnlResult').find('tr.k-footer-template').children('td:eq(14)').text(tsf);
                            }
                        }
                    });
                }
                else {
                    $('#pnlResult').find('tr.k-footer-template').children('td:eq(2)').text(bm);
                    $('#pnlResult').find('tr.k-footer-template').children('td:eq(3)').text(sh);
                    $('#pnlResult').find('tr.k-footer-template').children('td:eq(4)').text(plt);
                    $('#pnlResult').find('tr.k-footer-template').children('td:eq(6)').text(gld);
                    $('#pnlResult').find('tr.k-footer-template').children('td:eq(8)').text(slv);
                    $('#pnlResult').find('tr.k-footer-template').children('td:eq(10)').text(trn);
                    $('#pnlResult').find('tr.k-footer-template').children('td:eq(12)').text(tsp);
                    $('#pnlResult').find('tr.k-footer-template').children('td:eq(14)').text(tsf);
                }
                return
            }
        });

    }

    function exportXls() {
        var params = {
            area: $("[name=GroupArea]").val(),
            comp: $("[name=CompanyCode]").val(),
            start: $("[name=PeriodeBeg]").val(),
            end: $("[name=PeriodeEnd]").val(),
        }
        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "ab.api/sfinfo/excel",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'ab.api/sfinfo/DownloadExcelFile?key=' + data.value + '&filename=ManpowerDashboard';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
    }
});
