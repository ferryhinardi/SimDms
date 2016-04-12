"use strict";
var widget;
$(document).ready(function () {
    SimDms.item = [], SimDms.data = [];
    var options = {
        title: "LIVE STOCK DEALER MAINTENANCE",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "Type", text: "Type", cls: "span4", type: "select", opt_text: "-- SELECT ALL --" },
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
            { action: 'save', text: 'Save', icon: 'fa fa-save', name: "save" },
            { text: "Refresh", name: "refresh", action: "refresh", icon: "fa fa-refresh" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case "save":
                    save();
                    break;
                case "refresh":
                    refreshGrid();
                    break;
            }
        }
    };

    widget = new SimDms.Widget(options);

    widget.render(function () {
        widget.post("wh.api/LiveStock/SelectComboDealer", { TypeCombo: "Type" }, function (result) {
            widget.bind({
                name: 'Type',
                text: '-- SELECT ALL --',
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
        params.IsMaintain = true;
        var DataSource = new kendo.data.DataSource({
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            transport: {
                read: {
                    url: SimDms.baseUrl + "wh.api/LiveStock/LiveStockDealerSalesTable",
                    dataType: "json",
                    type: "POST",
                    timeout: 150000,
                    data: params
                },
            },
            schema: {
                data: "data",
                total: "total"
            },
        });

        var oTable = $(".kgrid #LiveStockDealerTable").kendoGrid({
            dataSource: DataSource,
            pageable: false,
            dataBinding: function (data) {
                SimDms.data = data.items;
                SimDms.item = data.items;
            },
            dataBound: function (e) {
                checkCbAll($("input#cbAll"));
            },
            columns: [
                { field: "Type", width: 120, title: "Type" },
                { field: "Variant", width: 120, title: "Variant" },
                { field: "Transmission", width: 90, title: "Transmission" },
                { field: "Colour", width: 90, title: "Colour" },
                { field: "Qty", width: 30, headerTemplate: "<div class='right'>Qty</div>", type: "number", template: "<div class='right'>#= (Qty == undefined || Qty == 0) ? '-' : number_format(Qty, 0) #</div>" },
                { headerTemplate: '<div align="center"><input type="checkbox" id="cbAll" style="height: 15px; margin: 0;" /></div>', template: '<div align="center"><input type="checkbox" #= IsVisible ? \'checked="checked" class="cbVisible cbChecked"\' : \'class="cbVisible"\' # style="height: 15px; margin: 0;" /></div>', width: 120 },
            ]
        });

        $("#LiveStockDealerTable .k-grid-header").on("change", "input#cbAll", function (e) {
            var grid = $("#LiveStockDealerTable").data("kendoGrid"),
                dataItem = [];

            if ($(this).is(":checked")) {
                $("input.cbVisible").prop("checked", true).addClass("cbChecked");
                $.each($(".k-grid-content tbody tr"), function (index, value) {
                    // grid.dataItem(value).set("IsVisible", true);
                    grid.dataItem(value).IsVisible = true;
                    dataItem.push(grid.dataItem(value));
                })
            }
            else {
                var data = (!checkStatusVisible(dataItem.uid)) ? dataItem : SimDms.data;
                $("input.cbVisible").prop("checked", false).removeClass("cbChecked");
                $.each(data, function (index, value) {
                    value.IsVisible = false;
                    dataItem.push(value);
                });
            }
            SimDms.item = dataItem;
            checkCbAll($(this));
        });

        $("#LiveStockDealerTable .k-grid-content").on("change", "input.cbVisible", function (e) {
            var grid = $("#LiveStockDealerTable").data("kendoGrid"),
                dataItem = grid.dataItem($(e.target).closest("tr"));
            if ($(this).is(":checked")) {
                $(this).addClass("cbChecked");
                dataItem.IsVisible = true;
                if (!checkStatusVisible(dataItem.uid)) {
                    SimDms.item.push(dataItem);
                }
            }
            else {
                $(this).removeClass("cbChecked");
                dataItem.IsVisible = false;
                if (!checkStatusVisible(dataItem.uid)) {
                    SimDms.item = $.grep(SimDms.item, function (value, index) {
                        return (value.uid != dataItem.uid);
                    });
                }
            }
            checkCbAll($("input#cbAll"));
        });

        function checkStatusVisible(uid) {
            $.each(SimDms.data, function (index, value) {
                if (value.uid == uid) {
                    return false;
                }
            });
            return true;
        }

        function checkCbAll(element) {
            var maxCb = $("input.cbVisible").length,
                checked = $("input.cbChecked").length;
            if (checked == maxCb)
                element.prop("checked", true);
            else
                element.prop("checked", false);
        }
    }

    function save() {
        $.ajax({
            url: "wh.api/LiveStock/SaveVisiblilityDealer",
            type: "POST",
            dataType: "json",
            data: JSON.stringify({ data: SimDms.item }),
            beforeSend: function() {
                $(".page .ajax-loader").fadeIn();
            },
            contentType: "application/json",
            success: function (data) {
                $(".page .ajax-loader").fadeOut();
                if (data.success) {
                    sdms.info("Data Saved", "Success");
                    refreshGrid();
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
    }
});