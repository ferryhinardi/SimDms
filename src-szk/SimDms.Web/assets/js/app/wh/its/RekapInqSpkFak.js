var cabang;
var Company;
var Outlet;

$(document).ready(function () {
    var options = {
        title: "Generate ITS - Rekap Inquiry SPK Faktur",
        xtype: "panels",
        toolbars: [
            { name: "btnExportXls", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                     {
                         text: "Period",
                         type: "controls",
                         cls: "span6 full",
                         items: [
                             { name: "AccumFrom1", cls: "span3", type: "datepicker" },
                             { type: "label", text: "S/D", cls: "span1" },
                             { name: "AccumTo1", cls: "span3", type: "datepicker" }
                         ]
                     },
                     {
                         text: "Tipe Kendaraan",
                         type: "controls",
                         cls: "span4 full",
                         items: [
                             { name: "CarType", text: "CarType", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                         ]
                     },
                     //{
                     //    text: "Area",
                     //    type: "controls",
                     //    cls: "span4 full",
                     //    items: [
                     //        { name: "GroupArea", text: "Area", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                     //    ]
                     //},
                     //{
                     //    text: "Dealer",
                     //    type: "controls",
                     //    cls: "span6 full",
                     //    items: [
                     //        { name: "CompanyCode", text: "Dealer Name", cls: "span6", type: "select", opt_text: "-- SELECT ALL -- " },
                     //    ]
                     //},
                ],
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    widget.select({ selector: "[name=GroupArea]", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" });
    widget.select({ selector: "[name=CarType]", url: "wh.api/combo/TipeKendaraan", optionalText: "-- SELECT ONE --" },
        function (res) {
            $("#CarType").select2('val', res[0].value);
    });
    //widget.setSelect([{ name: "GroupArea", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    //widget.setSelect([{ name: "CarType", url: "wh.api/combo/CarTypes", optionalText: "-- SELECT ALL --" }]);

    widget.render(function () {
        var filter = {
            AccumFrom1: new Date(moment(moment().format('YYYY-MM-') + '01')),
            AccumTo1: new Date(),
        }
        $("[name=GroupArea]").on("change", function () {
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=GroupArea]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
        });
        widget.populate(filter);

        $("#btnExportXls").on("click", exportXls);
    });

    function exportXls() {
        var data = $("#pnlFilter").serializeObject();
        console.log(moment(data.AccumFrom1).format('DD MMM YY'));

        var url = "wh.api/inquiryprod/GenerateITSReportRekapInqSpkFak?";
        url += "&StartDate=" + moment(data.AccumFrom1).format('YYYYMMDD');
        url += "&EndDate=" + moment(data.AccumTo1).format('YYYYMMDD');
        url += "&TipeKendaraan=" + data.CarType;
        //url += "&GroupArea=" + data.GroupArea;
        //url += "&CompanyCode=" + data.CompanyCode;
        url += "&AccumTo1Name=" + moment(data.AccumTo1).format('DD MMM YYYY');
        window.location = url;
    }
});