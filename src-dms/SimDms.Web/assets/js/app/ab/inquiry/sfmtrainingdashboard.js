"use strict";
var hotgrid, dataCount, rCount=0, iHeight=0, dataTotal, dCount, rdCount;
$(document).ready(function () {
    var options = {
        title: "Training Dashboard",
        xtype: "panels",
        toolbars: [
            { name: "btnLoadData", text: "Load to Grid", cls: "btn btn-primary", icon: "fa fa-refresh" },
            { name: "btnExcel", text: "Save to Excel", cls: "btn btn-primary", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                items: [
                    {
                        text: "Periode Start",
                        type: "controls",
                        items: [
                            { name: "StartYear", type: "select", cls: "span2" },
                            { name: "StartMonth", type: "select", cls: "span2" },
                        ]
                    },
                    {
                        text: "Periode End",
                        type: "controls",
                        items: [
                            { name: "EndYear", type: "select", cls: "span2" },
                            { name: "EndMonth", type: "select", cls: "span2" },
                        ]
                    }
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
        initComboYear();
        initComboMonth();
        renderGrid();
    }

    function initComboYear() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'ab.api/TrainingDashboard/Years',
            success: function (data) {
                widget.setItems({ name: "StartYear", type: "select", data: data });
                widget.setItems({ name: "EndYear", type: "select", data: data });

                $('#StartYear').select2('val', new Date().getFullYear())
                $('#EndYear').select2('val', new Date().getFullYear());
            }
        });
    }

    function initComboMonth() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'ab.api/TrainingDashboard/Months',
            success: function (data) {
                widget.setItems({ name: "StartMonth", type: "select", data: data });
                widget.setItems({ name: "EndMonth", type: "select", data: data });

                $('#StartMonth').select2('val', new Date().getMonth() + 1);
                $('#EndMonth').select2('val', new Date().getMonth() + 1);
            }
        });
    }

    function renderGrid() {
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
        rCount === 0 ? rCount = rCount + 4 : rCount = rCount + 3
        var clkOld = '';
        var clkNew = '';
        var data = [
              ["OUTLET", "BRANCH MANAGER", "", "", "SALES HEAD", "", "", "SALES PLATINUM", "", "", "SALES GOLD", "", "", "SALES SILVER", "", "", "SALES TRAINEE", ""],
              ["", "JML", "T", "NT", "JML", "T", "NT", "JML", "T", "NT", "JML", "T", "NT", "JML", "T", "NT", "JML", "T", "NT", ""],
              ["", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""],
              ["TOTAL", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""]
        ],
        container = document.getElementById('wxgrid'),

        settings = {
            data: data,
            width: 1000,
            height: iHeight,
            contextMenu: true,
            minSpareRows: 1,
            maxRows: rCount,
            //persistentState: true,
            colWidths: [190, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 40, 0],
            columns: [
                    //Outlet
                    {  },
                    //Branch Manager
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    //Sales Head
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    //Sales Platinum
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    //Sales Gold
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    //Sales Silver
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    //Sales Trainee
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    { type: 'numeric', format: '0', className: "htRight" },
                    { className: "htContextMenu"}
            ],
            mergeCells: [
                    { row: 0, col: 0, rowspan: 2, colspan: 1 },
                    { row: 0, col: 1, rowspan: 1, colspan: 3 },
                    { row: 0, col: 4, rowspan: 1, colspan: 3 },
                    { row: 0, col: 7, rowspan: 1, colspan: 3 },
                    { row: 0, col: 10, rowspan: 1, colspan: 3 },
                    { row: 0, col: 13, rowspan: 1, colspan: 3 },
                    { row: 0, col: 16, rowspan: 1, colspan: 3 },
            ],
            cell: [
                    { row: 0, col: 0, className: "htMiddle htCenter" },
                    { row: rCount - 1, col: 0, className: "htCenter" },
            ],
            cells: function (row, col, prop) {
                var cellProperties = {};
                if (row < 2) {
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
                    if (cellValue != '-' && cellValue != '' && e.row != rCount - 1 && e.row > 1 && e.col > 0 && e.col < 19) {
                        //load detail
                        var cellOutlet = hotgrid.getDataAtCell(e.row, 19);
                        var cellFilter = getFilterName(e.col);
                        var startYear = $('#StartYear').val();
                        var startMonth = $('#StartMonth').val();
                        var endYear = $('#EndYear').val();
                        var endMonth = $('#EndMonth').val();
                        $.ajax({
                            async: false,
                            type: "POST",
                            data: {
                                outlet: $('#Outlet').val(),
                                startYear: parseInt(startYear),
                                startMonth: parseInt(startMonth),
                                endYear: parseInt(endYear),
                                endMonth: parseInt(endMonth),
                                outlet: cellOutlet,
                                filter: cellFilter
                            },
                            url: 'ab.api/TrainingDashboard/QryToGrid',
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
                                        contextMenu: true,
                                        minSpareRows: 1,
                                        maxRows: dCount,
                                        //persistentState: true,
                                        colWidths: [100, 130, 120, 80, 100],
                                        //columns: [
                                        //        { readOnly: true },
                                        //        { readOnly: true },
                                        //        { readOnly: true },
                                        //        { readOnly: true },
                                        //        { readOnly: true },
                                        //],
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
            hotgrid.setDataAtCell(dataCount, null, null, 'loadData');
            hotgrid.setDataAtCell(dataTotal, null, null, 'loadData');
        }
        hotgrid.render();
       // $(".wtHolder").css("left", "50px");
        //$(".wtHider").css("overflow", "scroll")
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
        var startYear = $('#StartYear').val();
        var startMonth = $('#StartMonth').val();
        var endYear = $('#EndYear').val();
        var endMonth = $('#EndMonth').val();
        
        if (startYear == undefined || startMonth == undefined || endYear == undefined || endMonth == undefined ||
            startYear == '' || startMonth == '' || endYear == '' || endMonth == '') {
            sdms.info("Periode harus diisi", "Warning");
            return;
        }

        var startPeriod = new Date(startYear, startMonth);
        var endPeriod = new Date(endYear, endMonth);
        if (startPeriod > endPeriod) {
            sdms.info("Periode akhir harus lebih besar dari periode awal", "Warning");
            return;
        }

        $('#btnLoadData').attr('disabled', 'disabled');
        sdms.info("Please wait...");

        $.ajax({
            async: false,
            type: "POST",
            data: {
                startYear: parseInt(startYear),
                startMonth: parseInt(startMonth),
                endYear: parseInt(endYear),
                endMonth: parseInt(endMonth),
                filter: ''
            },

            url: 'ab.api/TrainingDashboard/QryToGrid',
            success: function (data) {
                if (data.message == "") {
                    dataCount = jsonToRow(data.data, 2);
                    dataTotal = jsonToRowTotal(data.total, rCount + 2);
                } else {
                    sdms.info(data.message, "Error");
                }
            }
        });
        $('#wxgrid').empty();
        rCount === 0 ? iHeight = 400 : rCount === 1 ? iHeight = 150 : iHeight = (rCount + 3) * 25;
        renderGrid();
        
        $('#btnLoadData').removeAttr('disabled');
    });

    $('#btnExcel').on('click', function () {
        var startYear = $('#StartYear').val();
        var startMonth = $('#StartMonth').val();
        var endYear = $('#EndYear').val();
        var endMonth = $('#EndMonth').val();

        if (startYear == undefined || startMonth == undefined || endYear == undefined || endMonth == undefined ||
            startYear == '' || startMonth == '' || endYear == '' || endMonth == '') {
            sdms.info("Periode harus diisi", "Warning");
            return;
        }

        var startPeriod = new Date(startYear, startMonth);
        var endPeriod = new Date(endYear, endMonth);
        if (startPeriod > endPeriod) {
            sdms.info("Periode akhir harus lebih besar dari periode awal", "Warning");
            return;
        }

        $('#btnExcel').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: true,
            type: "POST",
            data: {
                startYear: parseInt(startYear),
                startMonth: parseInt(startMonth),
                endYear: parseInt(endYear),
                endMonth: parseInt(endMonth),
            },
            url: 'ab.api/TrainingDashboard/Query',
            success: function (data) {
                if (data.message == "") {
                    location.href = 'ab.api/TrainingDashboard/DownloadExcelFile?key=' + data.value;
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
        for (var i = 0; i < returnObj.length; i++) {
            data.push([startRow + i, 0, returnObj[i].OutletAbbr]);
            data.push([startRow + i, 1, returnObj[i].BM_jml === 0 ? '-' : returnObj[i].BM_jml]);
            data.push([startRow + i, 2, returnObj[i].BM_t === 0 ? '-' : returnObj[i].BM_t]);
            data.push([startRow + i, 3, returnObj[i].BM_nt === 0 ? '-' : returnObj[i].BM_nt]);
            data.push([startRow + i, 4, returnObj[i].SH_jml === 0 ? '-' : returnObj[i].SH_jml]);
            data.push([startRow + i, 5, returnObj[i].SH_t === 0 ? '-' : returnObj[i].SH_t]);
            data.push([startRow + i, 6, returnObj[i].SH_nt === 0 ? '-' : returnObj[i].SH_nt]);
            data.push([startRow + i, 7, returnObj[i].S4_jml === 0 ? '-' : returnObj[i].S4_jml]);
            data.push([startRow + i, 8, returnObj[i].S4_t === 0 ? '-' : returnObj[i].S4_t]);
            data.push([startRow + i, 9, returnObj[i].S4_nt === 0 ? '-' : returnObj[i].S4_nt]);
            data.push([startRow + i, 10, returnObj[i].S3_jml === 0 ? '-' : returnObj[i].S3_jml]);
            data.push([startRow + i, 11, returnObj[i].S3_t === 0 ? '-' : returnObj[i].S3_t]);
            data.push([startRow + i, 12, returnObj[i].S3_nt === 0 ? '-' : returnObj[i].S3_nt]);
            data.push([startRow + i, 13, returnObj[i].S2_jml === 0 ? '-' : returnObj[i].S2_jml]);
            data.push([startRow + i, 14, returnObj[i].S2_t === 0 ? '-' : returnObj[i].S2_t]);
            data.push([startRow + i, 15, returnObj[i].S2_nt === 0 ? '-' : returnObj[i].S2_nt]);
            data.push([startRow + i, 16, returnObj[i].S1_jml === 0 ? '-' : returnObj[i].S1_jml]);
            data.push([startRow + i, 17, returnObj[i].S1_t === 0 ? '-' : returnObj[i].S1_t]);
            data.push([startRow + i, 18, returnObj[i].S1_nt === 0 ? '-' : returnObj[i].S1_nt]);
            data.push([startRow + i, 19, returnObj[i].OutletCode]);
        }
        return data;
    }

    function jsonToRowTotal(returnObj, startRow) {
        var data = []
        for (var i = 0; i < returnObj.length; i++) {
            data.push([startRow + i, 0, "TOTAL"]);
            data.push([startRow + i, 1, returnObj[i].ΣBM_jml === 0 ? '-' : returnObj[i].ΣBM_jml]);
            data.push([startRow + i, 2, returnObj[i].ΣBM_t === 0 ? '-' : returnObj[i].ΣBM_t]);
            data.push([startRow + i, 3, returnObj[i].ΣBM_nt === 0 ? '-' : returnObj[i].ΣBM_nt]);
            data.push([startRow + i, 4, returnObj[i].ΣSH_jml === 0 ? '-' : returnObj[i].ΣSH_jml]);
            data.push([startRow + i, 5, returnObj[i].ΣSH_t === 0 ? '-' : returnObj[i].ΣSH_t]);
            data.push([startRow + i, 6, returnObj[i].ΣSH_nt === 0 ? '-' : returnObj[i].ΣSH_nt]);
            data.push([startRow + i, 7, returnObj[i].ΣS4_jml === 0 ? '-' : returnObj[i].ΣS4_jml]);
            data.push([startRow + i, 8, returnObj[i].ΣS4_t === 0 ? '-' : returnObj[i].ΣS4_t]);
            data.push([startRow + i, 9, returnObj[i].ΣS4_nt === 0 ? '-' : returnObj[i].ΣS4_nt]);
            data.push([startRow + i, 10, returnObj[i].ΣS3_jml === 0 ? '-' : returnObj[i].ΣS3_jml]);
            data.push([startRow + i, 11, returnObj[i].ΣS3_t === 0 ? '-' : returnObj[i].ΣS3_t]);
            data.push([startRow + i, 12, returnObj[i].ΣS3_nt === 0 ? '-' : returnObj[i].ΣS3_nt]);
            data.push([startRow + i, 13, returnObj[i].ΣS2_jml === 0 ? '-' : returnObj[i].ΣS2_jml]);
            data.push([startRow + i, 14, returnObj[i].ΣS2_t === 0 ? '-' : returnObj[i].ΣS2_t]);
            data.push([startRow + i, 15, returnObj[i].ΣS2_nt === 0 ? '-' : returnObj[i].ΣS2_nt]);
            data.push([startRow + i, 16, returnObj[i].ΣS1_jml === 0 ? '-' : returnObj[i].ΣS1_jml]);
            data.push([startRow + i, 17, returnObj[i].ΣS1_t === 0 ? '-' : returnObj[i].ΣS1_t]);
            data.push([startRow + i, 18, returnObj[i].ΣS1_nt === 0 ? '-' : returnObj[i].ΣS1_nt]);
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
        var fname = ['BM_jml','BM_t','BM_nt','SH_jml','SH_t','SH_nt','S4_jml','S4_t','S4_nt','S3_jml','S3_t','S3_nt','S2_jml','S2_t','S2_nt','S1_jml','S1_t','S1_nt'];
        return fname[cPos - 1];
    }
});