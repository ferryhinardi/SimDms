$(document).ready(function () {
    var options = {
        title: "Data Trend",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Position",
                        type: "controls",
                        items: [
                            { name: "Position", text: "Position", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                            { name: "Year", text: "Reference Year", cls: "span2", type: "select", opt_text: moment().format('YYYY') },
                        ]
                    },
                ],
            },
            { name: "SfmTrend", xtype: "k-grid", },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" },
        { name: "Year", url: "wh.api/combo/YearsNoDefaultText", optionalText: new Date().getFullYear() },
        { name: "Position", url: "wh.api/combo/positions", params: { dept: 'SALES' }, optionalText: "-- SELECT ALL --" }
        //{ name: "CompanyCode", url: "wh.api/combo/DealerList?LinkedModule=mp", optionalText: "-- SELECT ONE --" },
    ]);
    widget.default = { Status: "1" };
    widget.render(function () {
        widget.populate(widget.default);
        $("[name=GroupArea]").on("change", function () {
            //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList?LinkedModule=mp", params: { dealerType: "", groupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").change();
        });
        //$("[name=CompanyCode]").on("change", function () {
        //    widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("[name=CompanyCode]").val(), dept: "SALES" }, optionalText: "-- SELECT ALL --" });
        //    $("[name=Position]").change();
        //});
        $('#CompanyCode').attr('disabled', 'disabled');
       
    });

    $('#GroupArea').on('change', function () {
        if ($('#GroupArea').val() != "") {
            $('#CompanyCode').removeAttr('disabled');
        } else {
            $('#CompanyCode').attr('disabled', 'disabled');
            $('#CompanyCode').select2('val', "");
        }
        $('#CompanyCode').select2('val', "");
    });

    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    //$("select[name=Position],select[name=Year]").on("change", refreshGrid);

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();
        widget.kgrid({
            url: "wh.api/inquiry/sfmtrend",
            name: "SfmTrend",
            params: params,
            serverBinding: true,
            pageable: false,
            pageSize: 1000,
            columns: [
                { field: "BranchName", title: "Branch (Outlet)", width: 460 },
                {
                    field: "Month01", title: "Jan", width: 88,
                    template: "<div class='right'>#=Month01#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month02", title: "Feb", width: 88,
                    template: "<div class='right'>#=Month02#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month03", title: "Mar", width: 88,
                    template: "<div class='right'>#=Month03#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month04", title: "Apr", width: 88,
                    template: "<div class='right'>#=Month04#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month05", title: "May", width: 88,
                    template: "<div class='right'>#=Month05#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month06", title: "Jun", width: 88,
                    template: "<div class='right'>#=Month06#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month07", title: "Jul", width: 88,
                    template: "<div class='right'>#=Month07#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month08", title: "Aug", width: 88,
                    template: "<div class='right'>#=Month08#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month09", title: "Sep", width: 88,
                    template: "<div class='right'>#=Month09#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month10", title: "Oct", width: 88,
                    template: "<div class='right'>#=Month10#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month11", title: "Nov", width: 88,
                    template: "<div class='right'>#=Month11#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Month12", title: "Dec", width: 88,
                    template: "<div class='right'>#=Month12#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
            ],
            aggregate: [
                   { field: "Month01", aggregate: "sum" },
                   { field: "Month02", aggregate: "sum" },
                   { field: "Month03", aggregate: "sum" },
                   { field: "Month04", aggregate: "sum" },
                   { field: "Month05", aggregate: "sum" },
                   { field: "Month06", aggregate: "sum" },
                   { field: "Month07", aggregate: "sum" },
                   { field: "Month08", aggregate: "sum" },
                   { field: "Month09", aggregate: "sum" },
                   { field: "Month10", aggregate: "sum" },
                   { field: "Month11", aggregate: "sum" },
                   { field: "Month12", aggregate: "sum" },
            ],
            dataBound: function () {
                //var list = $(".k-grid-header-wrap th").find(".k-link");
                //for (var i = 1; i < list.length; i++) {
                //    $(list[i]).addClass("right");
                //}
                $("#SfmTrend table > tbody td:contains('null')").html('');
            }
        });
    }

    function exportXls() {
        var url = "wh.api/Report/SfmTrend";

        $('#btnExportXls').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: true,
            type: "POST",
            data: $("#pnlFilter").serializeObject(),
            url: url,
            success: function (data) {
                if (data.message == "") {
                    console.log(data);
                    location.href = 'wh.api/Report/DownloadExcelFile?key=' + data.value + '&filename=TrendMutationData';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
        $('#btnExportXls').removeAttr('disabled');
        //widget.exportXls({
        //    name: "SfmTrend",
        //    type: "kgrid",
        //    fileName: "pers_trend",
        //    items: [
        //        { name: "BranchName", text: "Branch" },
        //        { name: "Month01", text: "Jan" },
        //        { name: "Month02", text: "Feb" },
        //        { name: "Month03", text: "Mar" },
        //        { name: "Month04", text: "Apr" },
        //        { name: "Month05", text: "May" },
        //        { name: "Month06", text: "Jun" },
        //        { name: "Month07", text: "Jul" },
        //        { name: "Month08", text: "Aug" },
        //        { name: "Month09", text: "Sep" },
        //        { name: "Month10", text: "Oct" },
        //        { name: "Month11", text: "Nov" },
        //        { name: "Month12", text: "Dec" },
        //    ]
        //});
    }
});
