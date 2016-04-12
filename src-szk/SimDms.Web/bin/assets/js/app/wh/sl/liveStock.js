"use strict";
$(document).ready(function () {
    var options = {
        title: "LIVE STOCK",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "Type", text: "Type", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Transmission", text: "Transmission", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Variant", text: "Variant", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Colour", text: "Colour", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
                    { name: "Status", text: "Status", cls: "span4", type: "div" },
                ]
            },
            {
                name: "LiveStockTable",
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
        $("#pager").remove();
        $("#LiveStockTable").parent().append("<div id='pager'></div>");
        widget.post("wh.api/LiveStock/SelectCombo", { TypeCombo: "Type", IsVisible: 1 }, function (result) {
            widget.bind({
                name: 'Type',
                text: '-- SELECT ALL --',
                data: result[0]
            });
        });
        widget.post("wh.api/LiveStock/SelectCombo", { TypeCombo: "Variant", IsVisible: 1 }, function (result) {
            widget.bind({
                name: 'Variant',
                text: '-- SELECT ALL --',
                data: result[0]
            });
        });
        widget.post("wh.api/LiveStock/SelectCombo", { TypeCombo: "Colour", IsVisible: 1 }, function (result) {
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
        widget.post("wh.api/LiveStock/SelectCombo", { TypeCombo: "Status" }, function (result) {
            $("#Status").html(result[0][0].Status).css({
                "margin-top": "10px",
                "font-weight": "bold"
            }).parent().css("padding-bottom", "10px");
        });

        $("[name=Type]").change(function (e) {
            widget.select({ selector: "[name=Variant]", url: "wh.api/LiveStock/SelectCombo", params: { TypeCombo: "Variant", Type: $(this).val(), Colour: $("[name=Colour]").val() }, optionalText: "-- SELECT ALL --" },
                function (result) {
                    var data = result[0];
                    widget.select({ selector: "[name=Variant]", data: data, optionText: "-- SELECT ALL --" });
                });
            widget.select({ selector: "[name=Colour]", url: "wh.api/LiveStock/SelectCombo", params: { TypeCombo: "Colour", Type: $(this).val(), Variant: $("[name=Variant]").val() }, optionalText: "-- SELECT ALL --" },
                function (result) {
                    var data = result[0];
                    widget.select({ selector: "[name=Colour]", data: data, optionText: "-- SELECT ALL --" });
                });
            $("[name=Variant]").prop("selectedIndex", 0);
            $("[name=Variant]").change();
            $("[name=Colour]").prop("selectedIndex", 0);
            $("[name=Colour]").change();
        });

        $("[name=Variant]").change(function (e) {
            widget.select({ selector: "[name=Colour]", url: "wh.api/LiveStock/SelectCombo", params: { TypeCombo: "Colour", Variant: $(this).val(), Type: $("[name=Type]").val() }, optionalText: "-- SELECT ALL --" },
                function (result) {
                    var data = result[0];
                    widget.select({ selector: "[name=Colour]", data: data, optionText: "-- SELECT ALL --" });
                });
        });
        refreshGrid();
    });

    function refreshGrid() {
        var params = $("#pnlFilter").serializeObject();
        params.IsVisible = true;
        widget.kgrid({
            url: "wh.api/LiveStock/LiveStockSalesTable",
            name: "LiveStockTable",
            params: params,
            serverBinding: true,
            filterable: true,
            pageSize: 10,
            pageSizes: true,
            columns: [
                { field: "Type", width: 120, title: "Type" },
                { field: "Variant", width: 120, title: "Variant" },
                { field: "Transmission", width: 120, title: "Transmission" },
                { field: "Colour", width: 120, title: "Colour" },
                { field: "Qty", width: 120, title: "Qty" },
            ]
        });
    }
});