"use strict";
var widget, oTable;
$(document).ready(function () {
    var options = {
        title: "MASTER MONTHLY SALES INDICATOR",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                items: [
                    { name: "PeriodYear", text: "Period Year", type: "select", cls: "span8" }, 
                ],
            },
            {
                name: 'wxgridcell',
                xtype: 'wxtable'
            }
        ],
        toolbars: [
            { text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
            { action: 'save', text: 'Save', icon: 'fa fa-save', name: "save" },
            { action: 'expand', text: 'Expand', icon: 'fa fa-expand' },
            { action: 'collapse', text: 'Collapse', icon: 'fa fa-compress', cls: 'hide' },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'refresh':
                    if (checkPeriodYear($("[name=PeriodYear]"))) {
                        $(".page .toolbar > button#save").show();
                        refreshTable();
                        $("div.errorMsg label:last").text("");
                    }
                    else
                        $("div.errorMsg label:last").text("This Field Must Be Numeric.").css("font-style", "italic");
                    break;
                case 'save':
                    //if (checkPeriodYear($("[name=PeriodYear]"))) {
                        save();
                    //    $("div.errorMsg label:last").text("");
                    //}
                    //else
                    //    $("div.errorMsg label:last").text("This Field Must Be Numeric.").css("font-style", "italic");
                    break;
                case 'collapse':
                    widget.exitFullWindow();
                    widget.showToolbars(['save', 'refresh', 'export', 'expand']);
                    break;
                case 'expand':
                    widget.requestFullWindow();
                    widget.showToolbars(['save', 'refresh', 'export', 'collapse']);
                    break;
                default:
                    break;
            }
        }
    };

    widget = new SimDms.Widget(options);
    widget.render(function () {
        $.ajax({
            type: "POST",
            url: 'wh.api/Mssi/Years',
            success: function (data) {
                widget.setItems({ name: "PeriodYear", type: "select", data: data });
                $('#PeriodYear').select2('val', new Date().getFullYear())
            }
        });
        $("#pnlFilter").css('padding-bottom', '85px');
        GenerateTable();
        $(".page .toolbar > button#save").hide();
    });
});

function refreshTable() {
    var postData = {
        PeriodYear: $("#PeriodYear").val()
    }
    $.ajax({
        type: "POST",
        url: 'wh.api/Mssi/LoadTableMaster',
        dataType: 'json',
        data: postData,
        success: function (response) {
            if (response.data !== null) {
                var data = response.data.sort(function (a, b) {
                    if (a.MSSICode < b.MSSICode) {
                        return 1;
                    }
                    if (a.MSSICode > b.MSSICode) {
                        return -1;
                    }
                    // a must be equal to b
                    return 0;
                });
                data = jsonToRow(data);
                var totalRow = oTable.countRows();
                if (data.length == 0)
                    data = initTable(totalRow - 1);
                oTable.setDataAtCell(data, null, null, 'loadData');
            }
        }
    });
}

function save() {
    var data = oTable.getData().slice(0);
    data.splice(0, 1);
    var listData = [];

    for (var i = 0; i < data.length; i++) {
        listData.push(rowToJson(data[i]));
    }

    $.ajax({
        type: "POST",
        url: 'wh.api/Mssi/SaveMSSI',
        dataType: 'json',
        data: { Data: JSON.stringify(listData) },
        success: function (response) {
            sdms.info({ type: "success", text: "Data saved..." });
        },
        error: function () {
            sdms.info({ type: "error", text: "Save Failed..." });
        }
    });
}

function rowToJson(data) {
    var returnObj = new Object();
    returnObj.MSSICode = ((data[0].search("SU-WSR") != -1) ? "SU-WSR" : ((data[0].search("SU-RSR") != -1) ? "SU-RSR" : ""));
    returnObj.CompanyCode = (typeof data[1] === 'undefined') ? "" : data[1];
    returnObj.Year = (typeof data[2] === 'undefined') ? 0 : data[2];
    returnObj.Jan = (typeof data[3] === 'undefined') ? 0 : data[3];
    returnObj.Feb = (typeof data[4] === 'undefined') ? 0 : data[4];
    returnObj.Mar = (typeof data[5] === 'undefined') ? 0 : data[5];
    returnObj.Apr = (typeof data[6] === 'undefined') ? 0 : data[6];
    returnObj.May = (typeof data[7] === 'undefined') ? 0 : data[7];
    returnObj.Jun = (typeof data[8] === 'undefined') ? 0 : data[8];
    returnObj.Jul = (typeof data[9] === 'undefined') ? 0 : data[9];
    returnObj.Aug = (typeof data[10] === 'undefined') ? 0 : data[10];
    returnObj.Sep = (typeof data[11] === 'undefined') ? 0 : data[11];
    returnObj.Oct = (typeof data[12] === 'undefined') ? 0 : data[12];
    returnObj.Nov = (typeof data[13] === 'undefined') ? 0 : data[13];
    returnObj.Des = (typeof data[14] === 'undefined') ? 0 : data[14];
    return returnObj;
}

function jsonToRow(returnObj) {
    var data = []
    for (var i = 0; i < returnObj.length; i++) {
        //data.push([i + 1, 0, returnObj[i].MSSICode]);
        data.push([i + 1, 1, returnObj[i].CompanyCode]);
        data.push([i + 1, 2, returnObj[i].Year]);
        data.push([i + 1, 3, returnObj[i].Jan]);
        data.push([i + 1, 4, returnObj[i].Feb]);
        data.push([i + 1, 5, returnObj[i].Mar]);
        data.push([i + 1, 6, returnObj[i].Apr]);
        data.push([i + 1, 7, returnObj[i].May]);
        data.push([i + 1, 8, returnObj[i].Jun]);
        data.push([i + 1, 9, returnObj[i].Jul]);
        data.push([i + 1, 10, returnObj[i].Aug]);
        data.push([i + 1, 11, returnObj[i].Sep]);
        data.push([i + 1, 12, returnObj[i].Oct]);
        data.push([i + 1, 13, returnObj[i].Nov]);
        data.push([i + 1, 14, returnObj[i].Dec]);
    }
    return data;
}

function initTable(length) {
    var data = []
    for (var i = 0; i < length; i++) {
        //data.push([i + 1, 0, returnObj[i].MSSICode]);
        //data.push([i + 1, 1, returnObj[i].CompanyCode]);
        //data.push([i + 1, 2, returnObj[i].Year]);
        data.push([i + 1, 3, 0]);
        data.push([i + 1, 4, 0]);
        data.push([i + 1, 5, 0]);
        data.push([i + 1, 6, 0]);
        data.push([i + 1, 7, 0]);
        data.push([i + 1, 8, 0]);
        data.push([i + 1, 9, 0]);
        data.push([i + 1, 10, 0]);
        data.push([i + 1, 11, 0]);
        data.push([i + 1, 12, 0]);
        data.push([i + 1, 13, 0]);
        data.push([i + 1, 14, 0]);
    }
    return data;
}

function checkPeriodYear(element) {
    if (!$.isNumeric(element.val()))
        return false;
    else
        return true;
}

function GenerateTable(param) {
    var ColorRenderer = function (instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.backgroundColor = '#ECECEC';
        td.style.color = '#000';
    };
    var year = [];
    var data = [
        ["MSSI", "Dealer", "Year", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Des"],
        ["SALES UNIT WHOLESALES RATIO (SU-WSR)", "0000000", new Date().getFullYear(), "", "", "", "", "", "", "", "", "", "", "", ""],
        ["SALES UNIT RETAIL SALES RATIO (SU-RSR)", "0000000", new Date().getFullYear(), "", "", "", "", "", "", "", "", "", "", "", ""],
    ];
    $.ajax({
        url: 'wh.api/Mssi/YearMaster',
        dataType: "JSON",
        type: "POST",
        async: false
    }).done(function (data, textStatus, jqXHR) {
        $.map(data || [], function(item, index) {
            year.push(item.value); 
        });
    });
    var container = document.getElementById('wxgridcell'),
    settings = {
        data: data,
        minSpareRows: 1,
        colHeaders: false,
        contextMenu: true,
        autoWrapRow: true,
        manualColumnResize: true,
        persistentState: true,
        colWidths: [150, 80, 60, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75],
        allowInsertRow: false,
        maxRows: data.length,
        columns: [
            { readOnly: true /* "MSSI" */ },
            { /* "Dealer" */ },
            { editor: 'dropdown', source: year },
            { type: 'numeric', format: '0,0.00' /* "Jan" */ },
            { type: 'numeric', format: '0,0.00' /* "Feb" */ },
            { type: 'numeric', format: '0,0.00' /* "Mar" */ },
            { type: 'numeric', format: '0,0.00' /* "Apr" */ },
            { type: 'numeric', format: '0,0.00' /* "May" */ },
            { type: 'numeric', format: '0,0.00' /* "Jun" */ },
            { type: 'numeric', format: '0,0.00' /* "Jul" */ },
            { type: 'numeric', format: '0,0.00' /* "Aug" */ },
            { type: 'numeric', format: '0,0.00' /* "Sep" */ },
            { type: 'numeric', format: '0,0.00' /* "Oct" */ },
            { type: 'numeric', format: '0,0.00' /* "Nov" */ },
            { type: 'numeric', format: '0,0.00' /* "Des" */ },
        ],
        className: "htCenter",
        cell: [
            { row: 0, col: 1, className: "htMiddle htCenter" },
        ],
        cells: function (row, col, prop) {
            var cellProperties = {};

            if (row < 1) {
                this.renderer = ColorRenderer;
                cellProperties.readOnly = true;
            }

            return cellProperties;
        }
    };

    oTable = new Handsontable(container, settings);
    oTable.render();

    oTable.onPasteCustom = function () {
        
    };

    $('#wxgridcell .ht_master').css("width", "115%");
    $('#wxgridcell .ht_master .wtHolder, .wtHider').css({ "padding-left": "0", "overflow": "hidden", "height": "" });
}