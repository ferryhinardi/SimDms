"use strict";
$(document).ready(function () {
    SimDms.item = [], SimDms.data = [];
    var options = {
        title: "LIVE STOCK MAINTENANCE",
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

    var widget = new SimDms.Widget(options);

    widget.render(function () {
        $("#pager").remove();
        $("#LiveStockTable").parent().append("<div id='pager'></div>");
        widget.post("wh.api/LiveStock/SelectCombo", { TypeCombo: "Type" }, function (result) {
            widget.bind({
                name: 'Type',
                text: '-- SELECT ALL --',
                data: result[0]
            });
        });
        widget.post("wh.api/LiveStock/SelectCombo", { TypeCombo: "Variant" }, function (result) {
            widget.bind({
                name: 'Variant',
                text: '-- SELECT ALL --',
                data: result[0]
            });
        });
        widget.post("wh.api/LiveStock/SelectCombo", { TypeCombo: "Colour" }, function (result) {
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
        var DataSource = new kendo.data.DataSource({
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            transport: {
                read: {
                    url: SimDms.baseUrl + "wh.api/LiveStock/LiveStockSalesTable",
                    dataType: "json",
                    type: "POST",
                    data: params
                },
            },
            schema: {
                data: "data",
                total: "total"
            },
        });

        var oTable = $(".kgrid #LiveStockTable").kendoGrid({
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
                { field: "Transmission", width: 120, title: "Transmission" },
                { field: "Colour", width: 120, title: "Colour" },
                { field: "Qty", width: 120, title: "Qty" },
                { headerTemplate: '<input type="checkbox" id="cbAll" style="height: 15px; margin: 0;" />', template: '<input type="checkbox" #= IsVisible ? \'checked="checked" class="cbVisible cbChecked"\' : \'class="cbVisible"\' # style="height: 15px; margin: 0;" />', width: 120 },
            ]
        });

        $("#LiveStockTable .k-grid-header").on("change", "input#cbAll", function (e) {
            var grid = $("#LiveStockTable").data("kendoGrid"),
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

        $("#LiveStockTable .k-grid-content").on("change", "input.cbVisible", function (e) {
            var grid = $("#LiveStockTable").data("kendoGrid"),
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
            url: "wh.api/LiveStock/SaveVisiblility",
            type: "POST",
            dataType: "json",
            data: JSON.stringify({ data: SimDms.item }),
            contentType: "application/json",
            success: function (data) {
                if (data.success) {
                    sdms.info("Data Saved", "Success");
                    refreshGrid();
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
        //console.log(SimDms.item.length);
    }
});