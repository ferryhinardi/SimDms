"use strict";
$(document).ready(function () {
    var options = {
        title: "LIVE STOCK DEALER",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "Type", text: "Type", cls: "span4", type: "select" },
                    { name: "Transmission", text: "Transmission", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Variant", text: "Variant", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Colour", text: "Colour", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                ]
            },
            {
                name: "LiveStockDealerTable",
                xtype: "k-grid",
            },
        ],
        toolbars: [
            { text: "Refresh", name: "refresh", action: "refresh", icon: "fa fa-refresh" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case "refresh":
                    refreshGrid();
                    break;
            }
        }
    };

    var widget = new SimDms.Widget(options);

    widget.render(function () {
        widget.post("wh.api/LiveStock/SelectComboDealer", { TypeCombo: "Type" }, function (result) {
            widget.bind({
                name: 'Type',
                data: result[0]
            });
        });
        widget.post("wh.api/LiveStock/SelectComboDealer", { TypeCombo: "Variant" }, function (result) {
            widget.bind({
                name: 'Variant',
                text: '-- SELECT ALL --',
                data: result[0]
            });
        });
        widget.post("wh.api/LiveStock/SelectComboDealer", { TypeCombo: "Colour" }, function (result) {
            widget.bind({
                name: 'Colour',
                text: '-- SELECT ALL --',
                data: result[0]
            });
        });
        widget.bind({
            name: 'Transmission',
            text: '-- SELECT ALL --',
            data: [{
                value: "AT",
                text: "AT"
            }, {
                value: "MT",
                text: "MT"
            }]
        });

        $("[name=Type]").change(function (e) {
            widget.select({ selector: "[name=Variant]", url: "wh.api/LiveStock/SelectComboDealer", params: { TypeCombo: "Variant", Type: $(this).val(), Colour: $("[name=Colour]").val() }, optionalText: "-- SELECT ALL --" },
                function (result) {
                    var data = result[0];
                    widget.select({ selector: "[name=Variant]", data: data, optionText: "-- SELECT ALL --" });
                });
            $("[name=Variant]").prop("selectedIndex", 0);
            $("[name=Variant]").change();
            $("[name=Colour]").prop("selectedIndex", 0);
            $("[name=Colour]").change();
        });

        $("[name=Variant]").change(function (e) {
            widget.select({ selector: "[name=Colour]", url: "wh.api/LiveStock/SelectComboDealer", params: { TypeCombo: "Colour", Variant: $(this).val(), Type: $("[name=Type]").val() }, optionalText: "-- SELECT ALL --" },
                function (result) {
                    var data = result[0];
                    widget.select({ selector: "[name=Colour]", data: data, optionText: "-- SELECT ALL --" });
                });
        });
    });

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();
        params.IsVisible = true;
        params.IsMaintain = false;
        widget.kgrid({
            url: "wh.api/LiveStock/LiveStockDealerSalesTable",
            name: "LiveStockDealerTable",
            params: params,
            serverBinding: true,
            filterable: true,
            pageSize: 10,
            pageSizes: true,
            columns: [
                { field: "Type", width: 120, title: "Type" },
                { field: "Variant", width: 120, title: "Variant" },
                { field: "Transmission", width: 90, title: "Transmission" },
                { field: "Colour", width: 90, title: "Colour" },
                { field: "Qty", width: 60, title: "Qty", type: "number", template: "<div class='right'>#= (Qty == undefined || Qty == 0) ? '-' : Qty #</div>" },
                { field: "CompanyName", width: 150, title: "Dealer" },
            ]
        });
    }
});