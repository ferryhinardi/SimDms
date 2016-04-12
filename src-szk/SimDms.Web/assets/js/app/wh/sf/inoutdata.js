"use strict";
var hotgrid, dataCount, rCount = 0, iHeight = 0, dataTotal, dCount, rdCount;
$(document).ready(function () {
    var options = {
        title: "In Out Data",
        xtype: "panels",
        toolbars: [
            { name: "btnLoadData", text: "Load to Grid", cls: "btn btn-primary", icon: "fa fa-refresh" },
            { name: "btnExcel", text: "Save to Excel", cls: "btn btn-primary", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                items: [
                    {
                        text: "Dealer Name",
                        type: "controls",
                        items: [
                            { name: "Dealer", type: "select", cls: "span4", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Position",
                        type: "controls",
                        items: [
                            { name: "Position", type: "select", cls: "span4", opt_text: "-- SELECT ALL --" },
                        ]
                    },
                    {
                        text: "Periode (From - To)",
                        type: "controls",
                        items: [
                            { name: "FromDate", type: "datepicker", cls: "span2" },
                            { name: "ToDate", type: "datepicker", cls: "span2" },
                        ]
                    },    
                ]
            },
            {
                name: 'wxgrid',
                xtype: 'wxtable'
            },
            {
                name: 'winpopup',
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(init);

    function init() {
        initComboDealer();
        initPosition();
        initDatePicker();
        renderGrid();
    }

    function initComboDealer() {
        $.ajax({
            async: false,
            type: "POST",
            //data: {
            //    area: $('#Area').select2('val')
            //},
            url: 'wh.api/InOutData/Dealers',
            success: function (data) {
                widget.setItems({ name: "Dealer", type: "select", data: data, optionalText: "-- SELECT ALL --" });
                if (data.length == 1) $('#Dealer').select2('val', data[0].value);
            }
        });
    }

    function initPosition() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'wh.api/InOutData/Positions',
            success: function (data) {
                widget.setItems({ name: "Position", type: "select", data: data, optionalText: "-- SELECT ALL --" });
                if (data.length == 1) $('#Position').select2('val', data[0].value);
            }
        });
    }

    function initDatePicker() {
        $("[name='FromDate']").val(widget.toDateFormat(new Date()));
        $("[name='ToDate']").val(widget.toDateFormat(new Date()));
    }

    function renderGrid() {
        //console.log("D")
        var hdrRender = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#C5D9F1';
            td.style.fontSize = "12px";
            td.style.textAlign = 'center';
        };

        var ftrRender = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#EAF1DD';
            td.style.fontSize = "12px";
        };
        var dataRender = function (instance, td, row, col, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.backgroundColor = '#FFFFFF';
            td.style.fontSize = "12px";
        };
        rCount === 0 ? rCount = rCount + 3 : rCount = rCount + 2;
        var clkOld = '';
        var clkNew = '';
        var data = [
              ["OUTLET", "JUMLAH AWAL", "JOIN", "RESIGN", "MUTATION IN", "MUTATION OUT", "JUMLAH AKHIR", ""],
              ["", "", "", "", "", "", "", ""],
              ["TOTAL", "", "", "", "", "", "", ""]
            ],
        container = document.getElementById('wxgrid'),
        settings = {
            data: data,
            width: 900,
            height: iHeight,
            contextMenu: true,
            minSpareRows: 1,
            maxRows: rCount,
            colWidths: [190, 100, 60, 60, 100, 100, 100, 0],
            columns: [
                    //Outlet
                    { readOnly: true },
                    { type: 'numeric', format: '0', className: "htRight", },
                    { type: 'numeric', format: '0', className: "htRight", },
                    { type: 'numeric', format: '0', className: "htRight", },
                    { type: 'numeric', format: '0', className: "htRight", },
                    { type: 'numeric', format: '0', className: "htRight", },
                    { type: 'numeric', format: '0', className: "htRight", },
                    {
                        className: "htContextMenu"
                    }
            ],

            cell: [
                    { row: rCount - 1, col: 0, className: "htCenter" },
                    //{ row: rCount - 1, col: 0, className: "htCenter" }
            ],
            cells: function (row, col, prop) {
                var cellProperties = {};
                if (row === 0) {
                    this.renderer = hdrRender;
                }
                else if (row === rCount - 1) {
                    this.renderer = ftrRender;
                } else {
                    this.renderer = dataRender;
                }
                cellProperties.readOnly = true;
                return cellProperties;
            },
            afterOnCellMouseDown: function (sender, e, val) {
                if (e.row === -1) {
                    this.getInstance().deselectCell();
                }
                clkNew = e.row + '' + e.col
                if (clkNew === clkOld) {
                    var cellValue = hotgrid.getDataAtCell(e.row, e.col);
                    rdCount = cellValue;
                    if (cellValue != '-' && e.row != rCount - 1 && e.row > 0 && e.col > 0 && e.col < 7) {
                        //load detail
                        var cellOutlet = hotgrid.getDataAtCell(e.row, 7);
                        var cellFilter = getFilterName(e.col);
                        $.ajax({
                            async: false,
                            type: "POST",
                            data: {
                                dealer: $('#Dealer').val(),
                                position: $('#Position').val(),
                                fromdate: $("[name='FromDate']").val(),
                                todate: $("[name='ToDate']").val(),
                                outlet: cellOutlet,
                                filter: cellFilter
                            },
                            url: 'wh.api/InOutData/QryToGrid',
                            success: function (data) {
                                if (data.message == "") {
                                    var dataDetail = jsonToRowDetail(data.detail, 1);
                                    var hotDtl;
                                    var data = [
                                                ["BRANCH", "EMPLOYEE", "POSITION", "GRADE", "JOIN DATE"]
                                    ],
                                    containerDtl = document.getElementById('winpopup'),
                                    settingsDtl = {
                                        data: data,
                                        colWidths: [100, 130, 120, 80, 100],
                                        columns: [
                                                { readOnly: true },
                                                { readOnly: true },
                                                { readOnly: true },
                                                { readOnly: true },
                                                { readOnly: true },
                                        ],
                                        //width:500,
                                        //height: rdCount * 25,
                                        contextMenu: true,
                                        minSpareRows: 1,
                                        maxRows: dCount,
                                        persistentState: true,
                                        cells: function (row, col, prop) {
                                            var cellProperties = {};
                                            if (row < 1) {
                                                this.renderer = hdrRender;
                                                cellProperties.readOnly = true;
                                            } else {
                                                this.renderer = dataRender;
                                                cellProperties.readOnly = true;
                                            }
                                            return cellProperties;
                                        }
                                    };
                                    hotDtl = new Handsontable(containerDtl, settingsDtl);
                                    hotDtl.alter('remove_row', 1, hotDtl.countRows());
                                    hotDtl.setDataAtCell(dataDetail, null, null, 'loadData');
                                    $("#winpopup").data("kendoWindow").center().open();
                                    $('#winpopup').scrollTop(0);
                                    $(".k-window-title").css("top", "-1px");
                                    $(".k-window-title").css("fontSize", "12px");
                                    $(".k-window-actions").css("top", "-10px");
                                    hotDtl.render();
                                } else {
                                    sdms.info(data.message, "Error");
                                }
                            }
                        });
                    }
                    clkOld = '';
                } else {
                    clkOld = clkNew;
                }
            }

        };
        hotgrid = new Handsontable(container, settings);
        if (dataCount != undefined && dataTotal != undefined) {
            //console.log(dataCount);
            hotgrid.setDataAtCell(dataCount, null, null, 'loadData');
            hotgrid.setDataAtCell(dataTotal, null, null, 'loadData');
        }
        hotgrid.render();
        //$(".wtHolder").css("top", "40px");
        $(".wtHolder").css("left", "80px");
        //$(".wtHolder").css("width", "850px");


    }

    $("#winpopup").kendoWindow({
        position: {
            top: 100, // or "100px"
            left: "20%"
        },
        width: "580px",
        height: "400px",
        modal: true,
        title: "Detail",
        visible: false,
        resizable: false,
        close: function (e) {
            $(this.element).empty();
        }
    });


    $('#btnLoadData').on('click', function () {
        $('#btnLoadData').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: false,
            type: "POST",
            data: {
                dealer: $('#Dealer').val(),
                position: $('#Position').val(),
                fromdate: $("[name='FromDate']").val(),
                todate: $("[name='ToDate']").val(),
                outlet: '',
                filter: ''
            },
            url: 'wh.api/InOutData/QryToGrid',
            success: function (data) {
                if (data.message == "") {
                    dataCount = jsonToRow(data.data, 1);
                    dataTotal = jsonToRowTotal(data.total, rCount + 1);
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });

        $('#wxgrid').empty();
        rCount === 0 ? iHeight = 400 : iHeight = rCount * 25;
        renderGrid();

        $('#btnLoadData').removeAttr('disabled');
    });

    $('#btnExcel').on('click', function () {
        $('#btnExcel').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: true,
            type: "POST",
            data: {
                dealer: $('#Dealer').val(),
                position: $('#Position').val(),
                fromdate: $("[name='FromDate']").val(),
                todate: $("[name='ToDate']").val(),
                outlet: '',
                filter: ''
            },
            url: 'wh.api/InOutData/Query',
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/InOutData/DownloadExcelFile?key=' + data.value;
                } else {
                    sdms.info(data.message, "Error");
                }
                $('#btnExcel').removeAttr('disabled');
            }
        });
    });

    function jsonToRow(returnObj, startRow) {
        var data = []
        rCount = returnObj.length;
        console.log("Total record: " + rCount);
        for (var i = 0; i < returnObj.length; i++) {
            data.push([startRow + i, 0, returnObj[i].OutletAbbr]);
            data.push([startRow + i, 1, returnObj[i].Start === 0 ? '-' : returnObj[i].Start]);
            data.push([startRow + i, 2, returnObj[i].Join === 0 ? '-' : returnObj[i].Join]);
            data.push([startRow + i, 3, returnObj[i].Resign === 0 ? '-' : returnObj[i].Resign]);
            data.push([startRow + i, 4, returnObj[i].MutationIn === 0 ? '-' : returnObj[i].MutationIn]);
            data.push([startRow + i, 5, returnObj[i].MutationOut === 0 ? '-' : returnObj[i].MutationOut]);
            data.push([startRow + i, 6, returnObj[i].End === 0 ? '-' : returnObj[i].End]);
            data.push([startRow + i, 7, returnObj[i].OutletCode]);
        }
        return data;
    }

    //function jsonToRow(returnObj, xObj) {
    //    var data = []
    //    rCount = returnObj.length;
    //    console.log("Total record: " + rCount);

    //    data.push(["OUTLET", "JUMLAH AWAL", "JOIN", "RESIGN", "MUTATION IN", "MUTATION OUT", "JUMLAH AKHIR", ""])
    //    for (var i = 0; i < returnObj.length; i++) {
    //        data.push([returnObj[i].OutletAbbr, returnObj[i].Start === 0 ? '-' : returnObj[i].Start, returnObj[i].Join === 0 ? '-' : returnObj[i].Join, returnObj[i].Resign === 0 ? '-' : returnObj[i].Resign,
    //        returnObj[i].MutationIn === 0 ? '-' : returnObj[i].MutationIn, returnObj[i].MutationOut === 0 ? '-' : returnObj[i].MutationOut, returnObj[i].End === 0 ? '-' : returnObj[i].End, returnObj[i].OutletCode]);
    //    }
    //    for (var i = 0; i < xObj.length; i++) {
    //        data.push(["TOTAL", xObj[i].Start === 0 ? '-' : xObj[i].Start, xObj[i].Join === 0 ? '-' : xObj[i].Join, xObj[i].Resign === 0 ? '-' : xObj[i].Resign,
    //        xObj[i].MutationIn === 0 ? '-' : xObj[i].MutationIn, xObj[i].MutationOut === 0 ? '-' : xObj[i].MutationOut, xObj[i].End === 0 ? '-' : xObj[i].End]);
    //    }
    //    return data;
    //}


    function jsonToRowTotal(returnObj, startRow) {
        var data = []
        for (var i = 0; i < returnObj.length; i++) {
            data.push([startRow + i, 0, "TOTAL"]);
            data.push([startRow + i, 1, returnObj[i].Start === 0 ? '-' : returnObj[i].Start]);
            data.push([startRow + i, 2, returnObj[i].Join === 0 ? '-' : returnObj[i].Join]);
            data.push([startRow + i, 3, returnObj[i].Resign === 0 ? '-' : returnObj[i].Resign]);
            data.push([startRow + i, 4, returnObj[i].MutationIn === 0 ? '-' : returnObj[i].MutationIn]);
            data.push([startRow + i, 5, returnObj[i].MutationOut === 0 ? '-' : returnObj[i].MutationOut]);
            data.push([startRow + i, 6, returnObj[i].End === 0 ? '-' : returnObj[i].End]);
        }
        return data;
    }

    function jsonToRowDetail(returnObj, startRow) {
        var data = []
        dCount = returnObj.length + 1;
        for (var i = 0; i < returnObj.length; i++) {
            data.push([startRow + i, 0, returnObj[i].OutletAbbr]);
            data.push([startRow + i, 1, returnObj[i].EmployeeName]);
            data.push([startRow + i, 2, returnObj[i].PosName]);
            data.push([startRow + i, 3, returnObj[i].GradeName]);
            data.push([startRow + i, 4, moment(returnObj[i].JoinDate).format("DD/MM/YYYY")]);
        }
        return data;
    }

    function getFilterName(cPos) {
        var fname = ['start', 'join', 'resign', 'in', 'out', 'end'];
        return fname[cPos - 1];
    }
});