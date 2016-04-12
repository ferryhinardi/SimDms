var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";
function LiveStokDealer($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    me.transmisi = [
        { "value": 'MT', "text": 'MT' },
        { "value": 'AT', "text": 'AT' },
    ]


    $("[name=cmbTipe]").on('click', function () {
        $.ajax({
            async: true,
            type: "POST",
            data: { tipeCombo: "Variant", tipe: $("[name='cmbTipe']").val(), variant: $("[name='cmbVariant']").val() },
            url: 'om.api/livestok/loadCombo',
            success: function (data) {
                if (data.data != "") {
                    me.comboVariant = data.data
                    me.Apply();
                }
            }
        });
    })

    $("[name='cmbTipe'], [name='cmbVariant']").on('change', function () {
        if ($(this).val() != "") {
            $.ajax({
                async: true,
                type: "POST",
                data: { tipeCombo: "Transmission", tipe: $("[name='cmbTipe']").val(), variant: $("[name='cmbVariant']").val() },
                url: 'om.api/livestok/loadCombo',
                success: function (data) {
                    if (data.data != "") {
                        me.transmisi = data.data
                        me.Apply();
                    }
                }
            });
        }
    })

    //$("[name=cmbVariant]").on('change', function () {
    //    $.ajax({
    //        async: true,
    //        type: "POST",
    //        data: { tipeCombo: "Colour", tipe: $("[name='cmbTipe']").val(), variant: $("[name='cmbVariant']").val(), colour: $("[name='cmbColour']").val() },
    //        url: 'om.api/livestok/loadCombo',
    //        success: function (data) {
    //            if (data.data != "") {
    //                me.comboColour = data.data
    //                me.Apply();
    //            }
    //        }
    //    });
    //})

    me.loadData = function() {
        var params = { type: $("[name='cmbTipe']").val(), variant: $("[name='cmbVariant']").val(), transmisi: $("[name='cmbTransmisi']").val() };
        var DataSource = new kendo.data.DataSource({
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            transport: {
                read: {
                    url: SimDms.baseUrl + "om.api/LiveStok/LiveStockDealerSalesTable",
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

        var oTable = $(".kgrid #lvGrid").kendoGrid({
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
                //{ field: "Colour", width: 90, title: "Colour" },
                { field: "Qty", width: 30, headerTemplate: "<div class='right'>Qty</div>", type: "number", template: "<div class='right'>#= (Qty == undefined || Qty == 0) ? '-' : number_format(Qty, 0) #</div>" },
                { headerTemplate: '<div align="center"><input type="checkbox" id="cbAll" style="height: 15px; margin: 0;" /></div>', template: '<div align="center"><input type="checkbox" #= IsVisible ? \'checked="checked" class="cbVisible cbChecked"\' : \'class="cbVisible"\' # style="height: 15px; margin: 0;" /></div>', width: 120 },
            ]
        });

        $("#lvGrid .k-grid-header").on("change", "input#cbAll", function (e) {
            var grid = $("#lvGrid").data("kendoGrid"),
                dataItem = [];

            if ($(this).is(":checked")) {
                $("input.cbVisible").prop("checked", true).addClass("cbChecked");
                $.each($(".k-grid-content tbody tr"), function (index, value) {
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

        $("#lvGrid .k-grid-content").on("change", "input.cbVisible", function (e) {
            var grid = $("#lvGrid").data("kendoGrid"),
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

    me.save = function () {
        $.ajax({
            url: "om.api/LiveStok/SaveVisiblilityDealer",
            type: "POST",
            dataType: "json",
            data: JSON.stringify({ data: SimDms.item }),
            beforeSend: function () {
                $(".page .ajax-loader").fadeIn();
            },
            contentType: "application/json",
            success: function (data) {
                $(".page .ajax-loader").fadeOut();
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.loadData()
                } else {
                    MsgBox(data.message, MSG_ERROR)
                }
            }
        });
    }

    me.initialize = function () {
        me.comboTipe = {};
        me.comboVariant = {};
        me.transmisi = {};
        //me.comboColour = {};
       
        $.ajax({
            async: true,
            type: "POST",
            data: { tipeCombo: "Type", tipe: $("[name='cmbTipe']").val(), variant: $("[name='cmbVariant']").val(), colour: $("[name='cmbColour']").val() },
            url: 'om.api/livestok/loadCombo',
            success: function (data) {
                if (data.data != "") {
                    me.comboTipe = data.data 
                    me.Apply();
                }
            }
        });


    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Live Stok Dealer",
        xtype: "panels",
        toolbars: [
            { name: "btnSave", text: "Save", cls: "btn btn-primary", icon: "icon-file", click: "save()" },
            { name: "btnLoad", text: "Load", cls: "btn btn-primary", icon: "icon-refresh", click: "loadData()" },
        ],
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                        { name: "cmbTipe", opt_text: "-- SELECT ALL --", cls: "span4", type: "select2", text: "Type", datasource: "comboTipe" },
                        { name: "cmbTransmisi", opt_text: "-- SELECT ALL --", cls: "span4", type: "select2", text: "Transmission", datasource: "transmisi" },
                        { name: "cmbVariant", opt_text: "-- SELECT ALL --", cls: "span4", type: "select2", text: "Variant", datasource: "comboVariant" },
                        //{ name: "cmbColour", opt_text: "-- SELECT ALL --", cls: "span4", type: "select2", text: "Colour", datasource: "comboColour" },
                ]
            },
            {
                name: "lvGrid",
                xtype: "k-grid",
            },
        ]
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("LiveStokDealer");
    }
});