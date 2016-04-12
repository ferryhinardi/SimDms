$(document).ready(function () {
    var options = {
        title: "Manpower Dashboard",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "GroupArea", text: "Area", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },                    
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
            { name: "btnRefresh", action: 'refresh', text: "Refresh", icon: "fa fa-refresh" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { name: "btnExportXls", action: 'export', text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        onToolbarClick: function (action) {
            //console.log(action);
            switch (action) {
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'export', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'export', 'collapse']);
                    break;
                case 'export':
                    exportXls();
                    break;
                default:
                    break;
            }
        },
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.default = { Status: "1", CompanyCode: "", PeriodeBeg: new Date() , PeriodeEnd:new Date()};

    widget.render(function () {
        widget.populate(widget.default);
        $("#CompanyCode").prop('disabled', true);
        $("[name=GroupArea]").on("change", function () {
            var groupArea = $("[name=GroupArea]").val();
            if (groupArea == '' || groupArea == undefined) {
                $("#CompanyCode").prop('disabled', true);
            }
            else {
                $("#CompanyCode").prop('disabled', false);
            }

            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        }); 

        //$("[name=CompanyCode]").on("change", function () {
        //    widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/branchs", params: { comp: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
        //    widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("#pnlFilter [name=CompanyCode]").val(), dept: "SALES" }, optionalText: "-- SELECT ALL --" });
        //    $("[name=BranchCode]").prop("selectedIndex", 0);
        //    $("[name=Position]").prop("selectedIndex", 0);
        //    $("[name=BranchCode]").change();
        //});

        $("#btnRefresh").off().on("click", refreshGrid);

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
            area: $("[name=GroupArea]").val(),
            CompanyCode: $("[name=CompanyCode]").val(),
            start: $("[name=PeriodeBeg]").val(),
            end: $("[name=PeriodeEnd]").val(),
            Total: 0
        }

        var bm = 0, sh = 0, plt = 0, gld = 0, slv = 0, trn = 0, tsp = 0, tsf = 0;
        var waitToShow = false;

        widget.kgrid({
            url: 'wh.api/manpower/queryNew',
            name: "pnlResult",
            params: params,
            selectable: "row",
            serverBinding: true,
            sort: [{ field: "Outlet", dir: "asc" }],
            detailTemplate: kendo.template($("#template").html()),
            columns: [
               { field: "Outlet", width: 300, title: "Outlet", footerTemplate: 'Total :'},
               { field: "BranchManager", width: 140, title: "Branch Manager", footerTemplate: '', type: 'align-right' },
               { field: "SalesHead", width: 140, title: "Sales Head", footerTemplate: '', type: 'align-right' },
               { field: "Platinum", width: 140, title: "Sales Platinum", footerTemplate: '', type: 'align-right' },
               { field: "PlatinumPct", width: 140, title: "%", footerTemplate: '',type: 'align-right' },
               { field: "Gold", width: 140, title: "Sales Gold", footerTemplate: '', type: 'align-right' },
               { field: "GoldPct", width: 140, title: "%",footerTemplate: '', type: 'align-right' },
               { field: "Silver", width: 140, title: "Sales Silver", footerTemplate: '', type: 'align-right' },
               { field: "SilverPct", width: 140, title: "%", footerTemplate: '',type: 'align-right' },
               { field: "Trainee", width: 140, title: "Sales Trainee", footerTemplate: '', type: 'align-right' },
               { field: "TraineePct", width: 140, title: "Sales Trainee",footerTemplate: '',  type: 'align-right' },
               { field: "TotalSalesPerson", width: 140, title: "Total Sales Person", footerTemplate: '', type: 'align-right' },
               { field: "TotalSalesForce", width: 140, title: "Total Sales Force", footerTemplate: '', type: 'align-right' },
            ],
            onComplete: function () {
                $('#pnlResult').find('tr.k-footer-template').children('td:eq(1)').css('text-align', 'center');
                params.Total = 1;
                if (!waitToShow) {
                    $.ajax({
                        url: 'wh.api/manpower/queryNew',
                        type: "POST",
                        data: params,
                        dataType: 'JSON',
                        async: true,
                        success: function (response) {
                            if (response != undefined) {
                                bm  = response.BranchManager;
                                sh  = response.SalesHead;
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
            CompanyCode: $("[name=CompanyCode]").val(),
            start: $("[name=PeriodeBeg]").val(),
            end: $("[name=PeriodeEnd]").val(),
            Total: 2
        }
        $.ajax({
            async: true,
            type: "POST",
            data: params,
            url: "wh.api/manpower/queryNew",
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/manpower/DownloadExcelFile?key=' + data.value + '&filename=ManpowerDashboard';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
    }
});
