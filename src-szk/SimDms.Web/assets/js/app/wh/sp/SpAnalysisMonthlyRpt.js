var dataCount, rCount;
$(document).ready(function () {
    var options = {
        title: "Sparepart Analysis Monthly Report",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "Area", text: "Areas", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
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
                        text: "Outlet Name",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span6", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Periode",
                        type: "controls",
                        items: [
                            { name: "Year", text: "Year", cls: "span2", type: "select" },
                            { name: "Month", text: "", cls: "span4", type: "select" },
                        ]
                    },

                    {
                        text: "Type Of Goods",
                        type: "controls",
                        items: [
                            { name: "TypeOfGoods", cls: "span6 full", type: "select", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                ],
            },
            {
                name: "wxSparepartMonthlyGrid",
                xtype: "wxtable",
            },
        ],
        toolbars: [
            //{ name: "btnRefresh", text: "Refresh", icon: "fa fa-refresh" },
            { name: "btnRefresh", text: "Load Data", icon: "fa fa-search" },
            { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
    }


    var widget = new SimDms.Widget(options);

    widget.setSelect([{ name: "Area", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
    widget.select({ selector: "[name=Year]", url: "wh.api/combo/years", optionalText: "-- SELECT ONE --" });
    widget.select({ selector: "[name=Month]", url: "wh.api/combo/ListOfMonth", optionalText: "-- SELECT ONE --" });
    widget.select({ selector: "[name=TypeOfGoods]", url: "wh.api/combo/ListTypeOfGoods", optionalText: "-- SELECT ALL --" });
    widget.default = { Year: new Date().getFullYear(), Month: (new Date().getMonth()) + 1 };

    var area = "";

    console.log(new Date().getFullYear(), (new Date().getMonth()) + 1);
    widget.render(function () {
        widget.populate(widget.default);
        $("[name=Area]").on("change", function () {
            //widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ListDealersNew", params: { area: $("#pnlFilter [name=Area]").val() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=Area]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=CompanyCode]").prop("selectedIndex", 0);
            $("[name=CompanyCode]").change();
            $('#Dealer').removeAttr('disabled');

            console.log($("#pnlFilter [name=Area]").val(), $("#pnlFilter [name=CompanyCode]").val(), $("#pnlFilter [name=BranchCode]").val(""));
        });
        $("[name=CompanyCode]").on("change", function () {
            //widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ListBranchesNew", params: { area: $("#pnlFilter [name=Area]").val(), comp: $('#CompanyCode option:selected').text(), compText: $("#CompanyCode option:selected").text() }, optionalText: "-- SELECT ALL --" });
            widget.select({ selector: "[name=BranchCode]", url: "wh.api/combo/ComboOutletList", params: { companyCode: $("#pnlFilter [name=CompanyCode]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=BranchCode]").prop("selectedIndex", 0);
            $("[name=BranchCode]").change();

            console.log($("#pnlFilter [name=CompanyCode]").val(), $('#CompanyCode option:selected').text());
        });

        renderGrid();

        $('#CompanyCode').attr('disabled', 'disabled');
        $('#BranchCode').attr('disabled', 'disabled');

    });

    $("#btnRefresh").on("click", refreshGrid);
    $("#btnExportXls").on("click", exportXls);
    $("#pnlFilter select").on("change", clearGrid);

    $('#Area').on('change', function () {
        if ($('#Area').val() != "") {
            $('#CompanyCode').removeAttr('disabled');
        } else {
            $('#CompanyCode').attr('disabled', 'disabled');
            $('#BranchCode').attr('disabled', 'disabled');
            $('#CompanyCode').select2('val', "");
            $('#BranchCode').select2('val', "");
        }
        $('#CompanyCode').select2('val', "");
        $('#BranchCode').select2('val', "");
    });

    $('#CompanyCode').on('change', function () {
        if ($('#CompanyCode').val() != "") {
            $('#BranchCode').removeAttr('disabled');
        } else {
            $('#BranchCode').attr('disabled', 'disabled');
        }
        $('#BranchCode').select2('val', "");
    });

    function clearGrid() {
        $("#InqPers").empty();
    }

    function refreshGrid() {
        var date = new Date()
        if ($('[name=Year]').val() == null || $('[name=Year]').val() == '') {
            $('[name=Year]').val(date.getFullYear());
        }
        if ($('[name=Month]').val() == null || $('[name=Month]').val() == '') {
            $('[name=Month]').val(date.getMonth() + 1);
        }

        var params = $("#pnlFilter").serializeObject();
        console.log(params);
        $.ajax({
            async: false,
            type: "POST",
            data: params,
            url: 'wh.api/SpAnalysisMonthly/SpAnalisisBulananGrid',
            success: function (data) {
                dataCount = jsonToRow(data, 3);
                console.log(data);
            }
        });

        $('#wxSparepartMonthlyGrid').empty();
        renderGrid();
    }

    function renderGrid() {

        var ColorRenderer = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#ECECEC';
            td.style.color = '#000';
        };
        var region = [];
        var data = [
            ["Bulan", "Jumlah Jaringan", "Penjualan Kotor ", "", "", "", "Penjualan Bersih ", "", "", "", "HPP ", "", "", "", "", "Margin ", "", "", "", "Penerimaan Pembelian", "Nilai Stock", "ITO", "Demand", "", "", "Supply", "", "", "Service Ratio (%)", "", "", "DataStock", "", "", "", "", "", "", "", "", "", "", "", "Slow Moving", "Lead Time Order", ""],
            ["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "MC 0", "", "MC 1", "", "MC 2", "", "MC 3", "", "MC 4", "", "MC 5", "", "", "", ""],
            ["", "", "Workshop", "Counter", "Partshop", "SubDealer", "Workshop", "Counter", "Partshop", "SubDealer", "Workshop", "Counter", "Partshop", "SubDealer", "Total", "Workshop", "Counter", "Partshop", "SubDealer", "", "", "", "Line", "Qty", "Nilai", "Line", "Qty", "Nilai", "Line", "Qty", "Nilai", "Amount", "Qty", "Amount", "Qty", "Amount", "Qty", "Amount", "Qty", "Amount", "Qty", "Amount", "Qty", "", "Reguler", "Emergency"],
        ];

        rCount === 0 ? rCount = rCount + 4 : rCount = rCount + 3;
        var container = document.getElementById('wxSparepartMonthlyGrid');
        settings = {
            data: data,
            width: 1550,
            height: 380,
            contextMenu: true,
            minSpareRows: 1,
            maxRows: rCount,
            //colHeaders: false,
            colWidths: [80, 100, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 80, 80, 80, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 120, 80, 80],
            columns: [
                { type: 'text', readOnly: true  /* "Bulan" */ },
                { type: 'text', format: '0,0', readOnly: true /* "JumlahJaringan" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Workshop_PK" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Counter_PK" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Partshop_PK" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "SubDealer_PK" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Workshop_PB" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Counter_PB" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Partshop_PB" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "SubDealer_PB" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Workshop_HPP" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Counter_HPP" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Partshop_HPP" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "SubDealer_HPP" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Total_HPP" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Workshop_Margin" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Counter_Margin" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Partshop_Margin" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "SubDealer_Margin" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Penerimaan pembelian" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Nilai Stock" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "ITO" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Line Demand" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Qty Demand" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Nilai Demand" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Line Supply" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Qty Supply" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Nilai Supply" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Line Supply" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Qty Supply" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Nilai Supply" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Amount MC0" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Qty MC0" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Amount MC1" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Qty MC1" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Amount MC2" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Qty MC2" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Amount MC3" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Qty MC3" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Amount MC4" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Qty MC4" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Amount MC5" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Qty MC5" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Slow Moving" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Reguler" */ },
                { type: 'numeric', format: '0,0', readOnly: true /* "Emergency" */ },
            ],
            mergeCells: [
                { row: 0, col: 0, rowspan: 3, colspan: 1 },
                { row: 0, col: 1, rowspan: 3, colspan: 1 },
                { row: 0, col: 2, rowspan: 2, colspan: 4 },
                { row: 0, col: 6, rowspan: 2, colspan: 4 },
                { row: 0, col: 10, rowspan: 2, colspan: 5 },
                { row: 0, col: 15, rowspan: 2, colspan: 4 },
                { row: 0, col: 19, rowspan: 3, colspan: 1 },
                { row: 0, col: 20, rowspan: 3, colspan: 1 },
                { row: 0, col: 21, rowspan: 3, colspan: 1 },
                { row: 0, col: 22, rowspan: 2, colspan: 3 },
                { row: 0, col: 25, rowspan: 2, colspan: 3 },
                { row: 0, col: 28, rowspan: 2, colspan: 3 },
                { row: 0, col: 31, rowspan: 1, colspan: 12 },
                { row: 0, col: 43, rowspan: 3, colspan: 1 },
                { row: 0, col: 44, rowspan: 2, colspan: 2 },

                { row: 1, col: 31, rowspan: 1, colspan: 2 },
                { row: 1, col: 33, rowspan: 1, colspan: 2 },
                { row: 1, col: 35, rowspan: 1, colspan: 2 },
                { row: 1, col: 37, rowspan: 1, colspan: 2 },
                { row: 1, col: 39, rowspan: 1, colspan: 2 },
                { row: 1, col: 41, rowspan: 1, colspan: 2 },
                
            ],
            className: "htCenter",
            cell: [
                { row: 0, col: 0, className: "htMiddle htCenter" },
                { row: 0, col: 1, className: "htMiddle htCenter" },
                { row: 0, col: 15, className: "htMiddle htCenter" },
            ],
            cells: function (row, col, prop) {
                var cellProperties = {};

                if (row < 1) {
                    this.renderer = ColorRenderer;
                    cellProperties.readOnly = true;
                }

                return cellProperties;
            },
            afterRender: function (TD, row, col, prop, value, cellProperties) {
                $("tbody tr").find("td:nth-child(1)").css("background-color", "rgb(236, 236, 236)");
            }

        };

        var hdt = new Handsontable(container, settings);
        if (dataCount != undefined) {
            //console.log(dataCount);
            hdt.setDataAtCell(dataCount, null, null, 'loadData');
        }
        $('#wxSparepartMonthlyGrid').css("padding-top", "2px");
        hdt.render();
    };


    function exportXls() {
        var date = new Date()
        if ($('[name=Year]').val() == null || $('[name=Year]').val() == '') {
            $('[name=Year]').val(date.getFullYear());
        }
        if ($('[name=Month]').val() == null || $('[name=Month]').val() == '') {
            $('[name=Month]').val(date.getMonth() + 1);
        }

        var url = "wh.api/SpAnalysisMonthly/exportExcel?";
        var spID = "uspfn_spAnalisisBulananViewgrid";
        var area = $('[name=Area]').val();
        var dealer = $('[name=CompanyCode]').val();
        var outlet = $('[name=BranchCode]').val();
        var year = $('[name=Year]').val();
        var month = $('[name=Month]').val();
        var typeOfGoods = $('[name=TypeOfGoods]').val();
        var textArea = $('#Area option:selected').text();
        var textDealer = $('#CompanyCode option:selected').text();
        var textOutlet = $('#BranchCode option:selected ').text();
        var textMonth = $('#Month option:selected ').text();
        var textTypeOfGoods = $('#TypeOfGoods option:selected ').text();

        var
        params = "&Area=" + area;
        params += "&Dealer=" + dealer;
        params += "&Outlet=" + outlet;
        params += "&SpID=" + spID;
        params += "&Year=" + year;
        params += "&Month=" + month;
        params += "&TypeOfGoods=" + typeOfGoods;
        params += "&AreaText=" + textArea;
        params += "&DealerText=" + textDealer;
        params += "&OutletText=" + textOutlet;
        params += "&MonthText=" + textMonth;
        params += "&TypeOfGoodsText=" + textTypeOfGoods;

        url = url + params;
        window.location = url;

        console.log(url);
    }

    function jsonToRow(returnObj, startRow) {
        var data = []
        rCount = returnObj.length;
        console.log("Total record: " + rCount);
        for (var i = 0; i < returnObj.length; i++) {
            data.push([startRow + i, 0, returnObj[i].Bulan]);
            data.push([startRow + i, 1, returnObj[i].JumlahJaringan === 0 ? '0' : returnObj[i].JumlahJaringan]);
            data.push([startRow + i, 2, returnObj[i].Workshop_PK === 0 ? '0' : returnObj[i].Workshop_PK]);
            data.push([startRow + i, 3, returnObj[i].Counter_PK === 0 ? '0' : returnObj[i].Counter_PK]);
            data.push([startRow + i, 4, returnObj[i].Partshop_PK === 0 ? '0' : returnObj[i].Partshop_PK]);
            data.push([startRow + i, 5, returnObj[i].SubDealer_PK === 0 ? '0' : returnObj[i].SubDealer_PK]);
            data.push([startRow + i, 6, returnObj[i].Workshop_PB === 0 ? '0' : returnObj[i].Workshop_PB]);
            data.push([startRow + i, 7, returnObj[i].Counter_PB === 0 ? '0' : returnObj[i].Counter_PB]);
            data.push([startRow + i, 8, returnObj[i].Partshop_PB === 0 ? '0' : returnObj[i].Partshop_PB]);
            data.push([startRow + i, 9, returnObj[i].SubDealer_PB === 0 ? '0' : returnObj[i].SubDealer_PB]);
            data.push([startRow + i, 10, returnObj[i].Workshop_HPP === 0 ? '0' : returnObj[i].Workshop_HPP]);
            data.push([startRow + i, 11, returnObj[i].Counter_HPP === 0 ? '0' : returnObj[i].Counter_HPP]);
            data.push([startRow + i, 12, returnObj[i].Partshop_HPP === 0 ? '0' : returnObj[i].Partshop_HPP]);
            data.push([startRow + i, 13, returnObj[i].SubDealer_HPP === 0 ? '0' : returnObj[i].SubDealer_HPP]);
            data.push([startRow + i, 14, returnObj[i].Total_HPP === 0 ? '0' : returnObj[i].Total_HPP]);
            data.push([startRow + i, 15, returnObj[i].Workshop_Margin === 0 ? '0' : returnObj[i].Workshop_Margin]);
            data.push([startRow + i, 16, returnObj[i].Counter_Margin === 0 ? '0' : returnObj[i].Counter_Margin]);
            data.push([startRow + i, 17, returnObj[i].Partshop_Margin === 0 ? '0' : returnObj[i].Partshop_Margin]);
            data.push([startRow + i, 18, returnObj[i].SubDealer_Margin === 0 ? '0' : returnObj[i].SubDealer_Margin]);
            data.push([startRow + i, 19, returnObj[i].Penerimaan_Pembelian === 0 ? '0' : returnObj[i].Penerimaan_Pembelian]);
            data.push([startRow + i, 20, returnObj[i].Nilai_Stock === 0 ? '0' : returnObj[i].Nilai_Stock]);
            data.push([startRow + i, 21, returnObj[i].ITO === 0 ? '0' : returnObj[i].ITO]);
            data.push([startRow + i, 22, returnObj[i].Line_Demand === 0 ? '0' : returnObj[i].Line_Demand]);
            data.push([startRow + i, 23, returnObj[i].Quantity_Demand === 0 ? '0' : returnObj[i].Quantity_Demand]);
            data.push([startRow + i, 24, returnObj[i].Nilai_Demand === 0 ? '0' : returnObj[i].Nilai_Demand]);
            data.push([startRow + i, 25, returnObj[i].Line_Supply === 0 ? '0' : returnObj[i].Line_Supply]);
            data.push([startRow + i, 26, returnObj[i].Quantity_Supply === 0 ? '0' : returnObj[i].Quantity_Supply]);
            data.push([startRow + i, 27, returnObj[i].Nilai_Supply === 0 ? '0' : returnObj[i].Nilai_Supply]);
            data.push([startRow + i, 28, returnObj[i].Line_Service_Ratio === 0 ? '0' : returnObj[i].Line_Service_Ratio]);
            data.push([startRow + i, 29, returnObj[i].Quantity_Service_Ratio === 0 ? '0' : returnObj[i].Quantity_Service_Ratio]);
            data.push([startRow + i, 30, returnObj[i].Nilai_Service_Ratio === 0 ? '0' : returnObj[i].Nilai_Service_Ratio]);
            data.push([startRow + i, 31, returnObj[i].Ammount_MC0 === 0 ? '0' : returnObj[i].Ammount_MC0]);
            data.push([startRow + i, 32, returnObj[i].Qty_MC0 === 0 ? '0' : returnObj[i].Qty_MC0]);
            data.push([startRow + i, 33, returnObj[i].Ammount_MC1 === 0 ? '0' : returnObj[i].Ammount_MC1]);
            data.push([startRow + i, 34, returnObj[i].Qty_MC1 === 0 ? '0' : returnObj[i].Qty_MC1]);
            data.push([startRow + i, 35, returnObj[i].Ammount_MC2 === 0 ? '0' : returnObj[i].Ammount_MC2]);
            data.push([startRow + i, 36, returnObj[i].Qty_MC2 === 0 ? '0' : returnObj[i].Qty_MC2]);
            data.push([startRow + i, 37, returnObj[i].Ammount_MC3 === 0 ? '0' : returnObj[i].Ammount_MC3]);
            data.push([startRow + i, 38, returnObj[i].Qty_MC3 === 0 ? '0' : returnObj[i].Qty_MC3]);
            data.push([startRow + i, 39, returnObj[i].Ammount_MC4 === 0 ? '0' : returnObj[i].Ammount_MC4]);
            data.push([startRow + i, 40, returnObj[i].Qty_MC4 === 0 ? '0' : returnObj[i].Qty_MC4]);
            data.push([startRow + i, 41, returnObj[i].Ammount_MC5 === 0 ? '0' : returnObj[i].Ammount_MC5]);
            data.push([startRow + i, 42, returnObj[i].Qty_MC5 === 0 ? '0' : returnObj[i].Qty_MC5]);
            data.push([startRow + i, 43, returnObj[i].Slow_Moving === 0 ? '0' : returnObj[i].Slow_Moving]);
            data.push([startRow + i, 44, returnObj[i].LT_Reguler === 0 ? '0' : returnObj[i].LT_Reguler]);
            data.push([startRow + i, 45, returnObj[i].LT_Emergency === 0 ? '0' : returnObj[i].LT_Emergency]);
        }
        //console.log(data);
        return data;
    }
});
