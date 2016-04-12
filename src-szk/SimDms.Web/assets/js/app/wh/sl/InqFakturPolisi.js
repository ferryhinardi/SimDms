$(document).ready(function () {
    var options = {
        title: "Trend & Graph Faktur Polisi, SPK & Stock",
        xtype: "panels",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "small" },
        ],
        panels: [
            {
                name: "panelFilter",
                items: [
                    { name: "EndDate", type: "datepicker", cls: "span4 ", text: "Date" },
                    {
                        text: "Area / Dealer",
                        type: "controls",
                        cls: 'span6',
                        items: [
                            { name: 'Area', text: 'Area', type: 'select', cls: 'span3' },
                            { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span5', opt_text: '-- SELECT ALL --' },
                        ]
                    }
                ]
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);
    widget.select({ selector: "[name=Area]", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ONE --" });

    function renderCallback() {
        initElementEvents();
        $("[name=Area]").on("change", function () {
            widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#panelFilter [name=Area]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Dealer]").prop("selectedIndex", 0);
            $("[name=Dealer]").change();
        });
        //widget.post("wh.api/combo/PartSalesFilter", function (result) {
        //    widget.bind({
        //        name: 'Area',
        //        text: '-- SELECT ALL --',
        //        data: result[0],
        //        // onChange: function () { setTimeout(refreshGrid, 500) }
        //    });
        //    widget.bind({
        //        name: 'Dealer',
        //        text: '-- SELECT ALL --',
        //        data: result[1],
        //        parent: 'Area',
        //        defaultAll: true,
        //        //  onChange: function () { setTimeout(refreshGrid, 500) }
        //    });
        //});
    }

    function initElementEvents() {
        var btnExportExcel = $('#btnExportExcel');

        btnExportExcel.off();
        btnExportExcel.on('click', function () {
            var data = $("#panelFilter").serializeObject();
            console.log(data);
            var Date = moment(data.EndDate).format('YYYY-MM-DD');
            console.log(Date);
            if ($('[name=Area]').val() == "") {
                alert("Silakan pilih Dealer terlebih dahulu!");
                return;
            }
            else if ($('[name=Dealer]').val() == "") {
                alert("Silakan pilih Dealer terlebih dahulu!");
                return;
            }
            else {
                window.location.href = 'wh.api/DataFakturPolisi/GenerateDataFakturPolisi?EndDate=' + Date + '&Area=' + $('[name=Area]').val() + '&Dealer=' + $('[name=Dealer]').val() + '&Periode=' + $('[name=EndDate]').val();
            }
        });
    }
});