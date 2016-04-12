"use strict";
var UnitST, RevenueST, widget, rbTargetFlag;
$(document).ready(function () {
    var options = {
        title: "Unit & Revenue Service Target",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "PeriodYear", text: "Period Year", cls: "span3", required: true },
                    { text: "", cls: "errorMsg error", type: "label" },
                    {
                        name: "TargetFlag", type: "radiobutton",
                        items: [
                            { value: "U", label: "Unit", checked: true },
                            { value: "R", label: "Revenue" }
                        ]
                    },
                ],
            },
            {
                name: 'wxgridcellUnit',
                xtype: 'wxtable'
            },
            {
                name: 'wxgridcellRevenue',
                xtype: 'wxtable'
            }
        ],
        toolbars: [
            { text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
            { action: 'save', text: 'Save', icon: 'fa fa-save', name: "save" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
            { name: "btnExportXls", action: 'export', text: "Export (xls)", icon: "fa fa-file-excel-o" },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'refresh':
                    if (checkPeriodYear($("[name=PeriodYear]")))
                    {
                        $(".page .toolbar > button#save").show();
                        refreshTable(rbTargetFlag);
                        $("div.errorMsg label:last").text("");
                    }
                    else
                        $("div.errorMsg label:last").text("This Field Must Be Numeric.").css("font-style", "italic");
                    break;
                case 'save':
                    if (checkPeriodYear($("[name=PeriodYear]"))) {
                        save(this, rbTargetFlag);
                        $("div.errorMsg label:last").text("");
                    }
                    else
                        $("div.errorMsg label:last").text("This Field Must Be Numeric.").css("font-style", "italic");
                    break;
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['save','refresh', 'export', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['save', 'refresh', 'export', 'collapse']);
                    break;
                case 'export':
                    if (checkPeriodYear($("[name=PeriodYear]"))) {
                        ExportXlsx();
                        $("div.errorMsg label:last").text("");
                    }
                    else
                        $("div.errorMsg label:last").text("This Field Must Be Numeric.").css("font-style", "italic");
                    break;
                default:
                    break;
            }
        },
    }
    widget = new SimDms.Widget(options);
    widget.render(function () {
        $("#pnlFilter").css('padding-bottom', '85px');
        UnitRSTTableRender("Unit");
        UnitRSTTableRender("Revenue");
        $(".page .toolbar > button#save").hide();
        $("input[name=TargetFlag]").on("change", function () {
            CheckRadio(this);
        });
    });

    CheckRadio($("input[name=TargetFlag]"));
});

function checkPeriodYear(element)
{
    if (!$.isNumeric(element.val()))
        return false;
    else
        return true;
}

function refreshTable(param)
{
    var postData = {
        PeriodYear: $("input[name=PeriodYear]").val()
    }
    $.ajax({
        type: "POST",
        url: 'wh.api/UnitRevenueTarget/LoadTable',
        dataType: 'json',
        data: postData,
        success: function (response) {
            if (response.dataUnit !== null || response.dataRevenue !== null)
            {
                var dataTableUnit = jsonToRow(response.dataUnit);
                var dataTableRevenue = jsonToRow(response.dataRevenue);
                var totalRow = UnitST.countRows();
                
                // UnitST.alter('remove_row', 2, totalRow);
                // UnitST.alter('insert_row', 2, totalRow);
                UnitST.setDataAtCell(dataTableUnit, null, null, 'loadData');
                
                // RevenueST.alter('remove_row', 2, totalRow);
                // RevenueST.alter('insert_row', 2, totalRow);
                RevenueST.setDataAtCell(dataTableRevenue, null, null, 'loadData');
            }
        }
    });
}

function save(element, param)
{
    // var data = (param == "U") ? UnitST.getData().slice(0) : RevenueST.getData().slice(0);
    var dataUnit = UnitST.getData().slice(0),
        dataRevenue = RevenueST.getData().slice(0);
    
    dataUnit.splice(0, 2);
    dataRevenue.splice(0, 2);
    
    if ($("input[name=PeriodYear]").val() == "") return;

    var listData = [];

    for (var i = 0; i < dataUnit.length; i++) {
        listData.push(rowToJson(dataUnit[i], "U"));
    }
    for (var i = 0; i < dataRevenue.length; i++) {
        listData.push(rowToJson(dataRevenue[i], "R"));
    }
    
    var postData = {
        PeriodYear: $("input[name=PeriodYear]").val(),
        TargetFlag: param,
        Data: JSON.stringify(listData)
    }
    
    $.ajax({
        type: "POST",
        url: 'wh.api/UnitRevenueTarget/Save',
        dataType: 'json',
        data: postData,
        success: function (response) {
            sdms.info({ type: "success", text: "Data saved..." });
        },
        error: function() {
            sdms.info({ type: "error", text: "Save Failed..." });
        }
    });
}

function rowToJson(data, TargetFlag) {
    var returnObj = new Object();
    returnObj.GroupNo = data[0];
    returnObj.TargetFlag = TargetFlag;
    returnObj.target01 = (typeof data[2] === 'undefined') ? 0 : data[2];
    returnObj.target02 = (typeof data[3] === 'undefined') ? 0 : data[3];
    returnObj.target03 = (typeof data[4] === 'undefined') ? 0 : data[4];
    returnObj.target04 = (typeof data[5] === 'undefined') ? 0 : data[5];
    returnObj.target05 = (typeof data[6] === 'undefined') ? 0 : data[6];
    returnObj.target06 = (typeof data[7] === 'undefined') ? 0 : data[7];
    returnObj.target07 = (typeof data[8] === 'undefined') ? 0 : data[8];
    returnObj.target08 = (typeof data[9] === 'undefined') ? 0 : data[9];
    returnObj.target09 = (typeof data[10] === 'undefined') ? 0 : data[10];
    returnObj.target10 = (typeof data[11] === 'undefined') ? 0 : data[11];
    returnObj.target11 = (typeof data[12] === 'undefined') ? 0 : data[12];
    returnObj.target12 = (typeof data[13] === 'undefined') ? 0 : data[13];
    return returnObj;
}

function jsonToRow (returnObj) {
    var data = []
    for (var i = 0; i < returnObj.length; i++)
    {
        data.push([i + 2, 0, returnObj[i].GroupNo]);
        data.push([i + 2, 1, returnObj[i].AreaDealer]);
        data.push([i + 2, 2, returnObj[i].Target01]);
        data.push([i + 2, 3, returnObj[i].Target02]);
        data.push([i + 2, 4, returnObj[i].Target03]);
        data.push([i + 2, 5, returnObj[i].Target04]);
        data.push([i + 2, 6, returnObj[i].Target05]);
        data.push([i + 2, 7, returnObj[i].Target06]);
        data.push([i + 2, 8, returnObj[i].Target07]);
        data.push([i + 2, 9, returnObj[i].Target08]);
        data.push([i + 2, 10, returnObj[i].Target09]);
        data.push([i + 2, 11, returnObj[i].Target10]);
        data.push([i + 2, 12, returnObj[i].Target11]);
        data.push([i + 2, 13, returnObj[i].Target12]);
        data.push([i + 2, 14, returnObj[i].TotalTarget]);
    }
    return data;
}

function CheckRadio(element)
{
    if ($(element).is(':checked')) {
        if ($(element).val() == 'U') {
            $("#wxgridcellUnit").show();
            $("#wxgridcellRevenue").hide();
            rbTargetFlag = 'U';
        }
        else {
            $("#wxgridcellUnit").hide();
            $("#wxgridcellRevenue").show();
            rbTargetFlag = 'R';
        }
    }
}

function UnitRSTTableRender(param)
{
    var ColorRenderer = function (instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.backgroundColor = '#ECECEC';
        td.style.color = '#000';
    };
    var region = [];
    var data = [
        ["idRegion", "Region", "Target "+param, "", "", "", "", "", "", "", "", "", "", "", "Total Target", "", "", ""],
        ["", "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Des", "", "", ""],
    ];
    $.ajax({
        url: "wh.api/combo/LoadSrvGroupAreas",
        dataType: "JSON",
        async: false
    }).done(function (data, textStatus, jqXHR) {
        $.each(data, function (index, element) {
            region[index] = [];
            region[index].push(parseInt(element.value));
            region[index].push(element.text);
        });
    });
    for (var i = 0; i < region.length; i++) {
        data.push(region[i]);
    }

    var container = (param == "Unit") ? document.getElementById('wxgridcellUnit') : document.getElementById('wxgridcellRevenue'),
    settings = {
        data: data,
        minSpareRows: 1,
        colHeaders: false,
        contextMenu: true,
        autoWrapRow: true,
        manualColumnResize: true,
        persistentState: true,
        colWidths: [150, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 100],
        allowInsertRow: false,
        maxRows : data.length,
        columns: [
            { readOnly: true /* "IdRegion" */ },
            { readOnly: true /* "Region" */ },
            { type: 'numeric', format: '0,0' /* "Jan" */ },
            { type: 'numeric', format: '0,0' /* "Feb" */ },
            { type: 'numeric', format: '0,0' /* "Mar" */ },
            { type: 'numeric', format: '0,0' /* "Apr" */ },
            { type: 'numeric', format: '0,0' /* "May" */ },
            { type: 'numeric', format: '0,0' /* "Jun" */ },
            { type: 'numeric', format: '0,0' /* "Jul" */ },
            { type: 'numeric', format: '0,0' /* "Aug" */ },
            { type: 'numeric', format: '0,0' /* "Sep" */ },
            { type: 'numeric', format: '0,0' /* "Oct" */ },
            { type: 'numeric', format: '0,0' /* "Nov" */ },
            { type: 'numeric', format: '0,0' /* "Des" */ },
            { type: 'numeric', format: '0,0', /* readOnly: true data: "Total Target" */ },
        ],
        mergeCells: [
            { row: 0, col: 0, rowspan: 2, colspan: 1 },
            { row: 0, col: 1, rowspan: 2, colspan: 1 },
            { row: 0, col: 2, rowspan: 1, colspan: 12 },
            { row: 0, col: 14, rowspan: 2, colspan: 1 },
        ],
        className: "htCenter",
        cell: [
            { row: 0, col: 1, className: "htMiddle htCenter" },
        ],
        cells: function (row, col, prop) {
            var cellProperties = {};
            /*
            if (row < 1) this.renderer = ColorRenderer;
            if (row <= 2) cellProperties.readOnly = true;
            */
            if (row < 1) {
                this.renderer = ColorRenderer;
                cellProperties.readOnly = true;
            }
            return cellProperties;
        },
        afterChange: function (changes, source) {
            var table = this;
            if (source !== 'edit') return;
            var cRow = changes[0][0];
            var cCol = changes[0][1];
            /*
            if (changes !== null) {
                if (cCol !== 14) {
                    table.setDataAtCell(cRow, 14, updateTotal(changes, table, cRow));
                }
                if (cRow !== 2) {
                    table.setDataAtCell(2, cCol, updateNasional(changes, table, cCol));
                }
            }
            */
        },
        afterRender: function (TD, row, col, prop, value, cellProperties) {
            $("tbody tr").find("td:first").hide();
            $("tbody tr").find("td:nth-child(2)").css("border-left", "1px solid #CCC");
            //$("tbody tr:nth-child(3)").find("td").css("background-color", "#ECECEC");

            $('.ht_clone_top').hide();
        }
    };
    if (param == "Unit")
    {
        UnitST = new Handsontable(container, settings);
        /*
        UnitST.onPasteCustom = function () {
            var allTotalTarget = updateTotalAll(UnitST);
            var allNasionalTarget = updateNasionalAll(UnitST);
            for (var i = 0; i < allTotalTarget.length; i++) {
                UnitST.setDataAtCell(i + 2, 14, allTotalTarget[i]);
            }
            for (var i = 0; i < allNasionalTarget.length; i++) {
                UnitST.setDataAtCell(2, i + 2, allNasionalTarget[i]);
            }
        };
        */
        UnitST.render();

        $('#wxgridcellUnit .ht_master').css("width", "115%");
        $('#wxgridcellUnit .ht_master .wtHolder, .wtHider').css({ "padding-left": "0", "overflow": "hidden", "height": "" });
    }
    else
    {
        RevenueST = new Handsontable(container, settings);
        /*
        RevenueST.onPasteCustom = function () {
            var allTotalTarget = updateTotalAll(RevenueST);
            var allNasionalTarget = updateNasionalAll(RevenueST);
            for (var i = 0; i < allTotalTarget.length; i++) {
                RevenueST.setDataAtCell(i + 2, 14, allTotalTarget[i]);
            }
            for (var i = 0; i < allNasionalTarget.length; i++) {
                RevenueST.setDataAtCell(2, i + 2, allNasionalTarget[i]);
            }
        }
        */
        RevenueST.render();
        $('#wxgridcellRevenue .ht_master').css("width", "115%");
        $('#wxgridcellRevenue .ht_master .wtHolder, .wtHider').css({ "padding-left": "0", "overflow": "hidden", "height": "" });
    }
}

function updateTotal(changes, element, cRow)
{
    var totalTarget = 0;
    var data = element.getDataAtRow(cRow);
    $.each(data, function (index, element) {
        if (index != 0 && index != 1 && index != 14) { // 0 => GroupNo ; 1 => Area ; 14 => Total Target
            totalTarget += (typeof element === 'undefined') ? 0 : element;
        }
    });
    return totalTarget;
}

function updateNasional(changes, element, cCol)
{
    var totalNasional = 0;
    var data = element.getDataAtCol(cCol);
    
    $.each(data, function (index, item) {
        if (index > 2) {
            totalNasional += (typeof item === 'undefined' || item == null) ? 0 : item;
        }
    });
    return totalNasional;
}

function updateTotalAll(element)
{
    var endRow = element.getDataAtCol(1).length;
    var endCol = element.getDataAtRow(1).length;
    var data = element.getData(2, 2, endRow, endCol); // start row, start col, end row, end col
    var totalTarget = [];
    for (var i = 0; i < (endRow-2); i++)
    {
        totalTarget[i] = 0;
        for (var j = 0; j < (endCol - 3) ; j++)
        {
            totalTarget[i] += (typeof data[i][j] === 'undefined') ? 0 : data[i][j];
        }
    }
    return totalTarget;
}

function updateNasionalAll(element) {
    var endRow = element.getDataAtCol(1).length;
    var endCol = element.getDataAtRow(1).length;
    var data = element.getData(2, 2, endRow, endCol); // start row, start col, end row, end col
    var totalNasional = [];
    for (var i = 0; i < endRow ; i++)
    {
        totalNasional[i] = 0;
        for (var j = 0; j < (endCol - 6) ; j++)
        {
            var result = data[j + 1][i];
            totalNasional[i] += (typeof result === 'undefined' || result == null) ? 0 : result;
        }
    }
    return totalNasional;
}

function ExportXlsx()
{
    var PeriodYear = $("input[name=PeriodYear]").val();
    if (PeriodYear == "") return;

    $.ajax({
        async: true,
        type: "POST",
        url: "wh.api/report/GetUnitRevenue?PeriodYear="+PeriodYear,
        success: function (data) {
            if (data.message == "") {
                location.href = 'wh.api/report/DownloadExcelFile?key=' + data.value + '&filename=Unit & Revenue Target Report';
            } else {
                sdms.info(data.message, "Error");
            }
        }
    });
}