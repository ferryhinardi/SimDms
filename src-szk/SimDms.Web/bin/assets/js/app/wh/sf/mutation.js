$(document).ready(function () {
    var options = {
        title: "Data Mutation",
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
                            { name: "MutaDate", text: "Refference Date", cls: "span2", type: "datepicker" }
                        ]
                    },
                ],
            },
            { name: "SfmMutation", xtype: "k-grid", },
        ],
        toolbars: [
            { name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }
        //, { name: "CompanyCode", url: "wh.api/combo/DealerList?LinkedModule=mp", optionalText: "-- SELECT ONE --" }
    ]);
    widget.default = { Status: "1", MutaDate: new Date() };
    widget.render(function () {
        widget.populate(widget.default);
        $("[name=GroupArea]").on("change", function () {
            //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList?LinkedModule=mp", params: { dealerType: "", groupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").change();
        });
        $("[name=CompanyCode]").on("change", function () {
            widget.select({ selector: "[name=Position]", url: "wh.api/combo/positions", params: { comp: $("[name=CompanyCode]").val(), dept: "SALES" }, optionalText: "-- SELECT ALL --" });
            $("[name=Position]").change();
        });
        $("#btnRefresh").on("click", refreshGrid);
        $("#btnExportXls").on("click", exportXls);
        //$("select[name=Position],[name=MutaDate]").on("change", refreshGrid);

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

    function refreshGrid() {
        widget.kgrid({
            url: "wh.api/inquiry/sfmmutation",
            name: "SfmMutation",
            params: $("#pnlFilter").serializeObject(),
            serverBinding: true,
            pageable: false,
            pageSize: 1000,
            columns: [
                { field: "BranchName", title: "Branch (Outlet)", width: 380 },
                {
                    field: "Muta01", title: "Awal", width: 80,
                    template: "<div class='right'>#=Muta01#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta02", title: "Join", width: 80,
                    template: "<div class='right'>#=Muta02#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta03", title: "Mutasi (In)", width: 90,
                    template: "<div class='right'>#=Muta03#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta04", title: "Resign", width: 80,
                    template: "<div class='right'>#=Muta04#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta05", title: "Mutasi (Out)", width: 100,
                    template: "<div class='right'>#=Muta05#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
                {
                    field: "Muta06", title: "Akhir", width: 80,
                    template: "<div class='right'>#=Muta06#</div>",
                    aggregates: ["sum"], footerTemplate: "#= sum #"
                },
            ],
            aggregate: [
                   { field: "Muta01", aggregate: "sum" },
                   { field: "Muta02", aggregate: "sum" },
                   { field: "Muta03", aggregate: "sum" },
                   { field: "Muta04", aggregate: "sum" },
                   { field: "Muta05", aggregate: "sum" },
                   { field: "Muta06", aggregate: "sum" },
            ],
            dataBound: function () {
                var list = $(".k-grid-header-wrap th").find(".k-link");
                for (var i = 1; i < list.length; i++) {
                    $(list[i]).addClass("right");
                }
            }
        });
    }

    function exportXls() {
        var url = "wh.api/Report/SfmMutation";

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
                    location.href = 'wh.api/Report/DownloadExcelFile?key=' + data.value + '&filename=DataMutation';
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
        $('#btnExportXls').removeAttr('disabled');
        //widget.exportXls({
        //    name: "SfmMutation",
        //    type: "kgrid",
        //    fileName: "pers_mutation",
        //    items: [
        //        { name: "BranchName", text: "Branch" },
        //        { name: "Muta01", text: "Awal" },
        //        { name: "Muta02", text: "Join" },
        //        { name: "Muta03", text: "Mutasi (In)" },
        //        { name: "Muta04", text: "Resign" },
        //        { name: "Muta05", text: "Mutasi (Out)" },
        //        { name: "Muta06", text: "Akhir" },
        //    ]
        //});
    }
    function getSqlDate(value) {
        return moment(value, "DD-MMM-YYYY").format("YYYYMMDD");
    }
});
