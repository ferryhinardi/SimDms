var widget = new SimDms.Widget({
    title: 'Sparepart Analysis Weekly Report',
    xtype: 'panels',
    toolbars: [
         { text: 'Load', action: 'load', icon: 'fa fa-refresh' },
         { text: 'Export to Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o' },
    ],
    panels: [
        {
            name: "pnlFilter",
            items: [
                {
                    text: "Periode",
                    type: "controls",
                    items: [
                        { name: "Year", type: "select", cls: "span2" },
                        { name: "Month", type: "select", cls: "span2" },
                    ]
                },
                {
                    text: "Area",
                    type: "controls",
                    items: [
                        { name: 'Area', text: 'Area', cls: 'span4', type: 'select' },
                    ]
                },
                {
                    text: "Dealer",
                    type: "controls",
                    items: [
                        { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span4' },
                    ]
                },
                {
                    text: "Outlet",
                    type: "controls",
                    items: [
                        { name: 'Outlet', text: 'Outlet', type: 'select', cls: 'span4', opt_text: '-- SELECT ALL --' },
                    ]
                },
                {
                    text: "Type Of Goods",
                    type: "controls",
                    items: [
                        { name: 'TypeOfGoods', text: 'Type', type: 'select', cls: 'span4', opt_text: '-- SELECT ALL --' },
                    ]
                },
            ]
        },
        {
            name: "wxSparepartWeeklyGrid",
            xtype: "wxtable",
        },
         //{
         //    name: "SpAlaysisWeeklyGrid",
         //    xtype: "k-grid",
         //},
         
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'exportToExcel':
                exportToExcel();
                break;
            case 'load':
                refreshTable();
                break;
            default:
                break;
        }
    }
});
widget.render(function () {
    $('#Dealer').attr('disabled', 'disabled');
    $('#Outlet').attr('disabled', 'disabled');

    initComboArea();
    initComboDealer();
    initComboOutlet();
    initComboYear();
    initComboMonth();
    initComboType();

    //gridRender();
    //clearTable(sparepartWeeklyGrid);
    
    //$(".webix_column .webix_property_line").attr("style", "background-color:#f7f7f7");
});

function jsonToRow(returnObj) {
    var data = []
    for (var i = 0; i < returnObj.length; i++) {
        data.push([i + 2 , 0, returnObj[i].PeriodWeek]);
        data.push([i + 2 , 1, returnObj[i].Netto_WS]);
        data.push([i + 2 , 2, returnObj[i].HPP_WS]);
        data.push([i + 2 , 3, returnObj[i].Netto_C]);
        data.push([i + 2 , 4, returnObj[i].HPP_C]);
        data.push([i + 2 , 5, returnObj[i].Netto_PS]);
        data.push([i + 2 , 6, returnObj[i].HPP_PS]);
        data.push([i + 2 , 7, returnObj[i].NilaiStock]);
    }
    return data;
}

function refreshTable() {
    var data = widget.serializeObject("pnlFilter");

    if (data.Area == null || data.Area == undefined || data.Area == "") {
        sdms.info("Area harus diisi", "Warning");
        return;
    }
    if (data.Dealer == null || data.Dealer == undefined || data.Dealer == "")
    {
        sdms.info("Dealer harus diisi", "Warning");
        return;
    }
    //if (data.Outlet == null || data.Outlet == undefined || data.Outlet == "") {
    //    sdms.info("Outlet ", "Warning");
    //    return;
    //}

    var postData = {
        CompanyCode: data.Dealer,
        BranchCode: data.Outlet,
        PeriodYear: data.Year,
        PeriodMonth: data.Month,
        TypeOfGoods: data.TypeOfGoods
    }

    $.ajax({
        async: true,
        type: "POST",
        data: postData,
        url: "wh.api/sparepartreport/loadDataWeekly",
        success: function (e) {
            if (e.data.length == 0) {
                return;
            }
            gridRender(e.data);
        }
    });
}

function gridRender(resData) {
    var dataTable = jsonToRow(resData);

    var ColorRenderer = function (instance, td, row, col, prop, value, cellProperties) {
        Handsontable.renderers.TextRenderer.apply(this, arguments);
        td.style.backgroundColor = '#ECECEC';
        td.style.color = '#000';
    };
    var data = [
        ["Minggu Ke", "Sales Out Bengkel", "", "Sales Out Center", "", "Sales Out Partshop", "", "Nilai Stock"],
        ["", "Netto", "HPP", "Netto", "HPP", "Netto", "HPP", ""],
    ];
    var container = document.getElementById('wxSparepartWeeklyGrid'),
        settings = {
            data: data,
            colHeaders: false,
            contextMenu: true,
            autoWrapRow: true,
            manualColumnResize: true,
            persistentState: true,
            colWidths: [80, 100, 100, 100, 100, 100, 100, 150],
            columns: [
                { readOnly: true },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
                { type: 'numeric', format: '0,0' },
            ],
            mergeCells: [
                { row: 0, col: 0, rowspan: 2, colspan: 1 },
                { row: 0, col: 1, rowspan: 1, colspan: 2 },
                { row: 0, col: 3, rowspan: 1, colspan: 2 },
                { row: 0, col: 5, rowspan: 1, colspan: 2 },
                { row: 0, col: 7, rowspan: 2, colspan: 1 },
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
            },
        };
    weeklyTable = new Handsontable(container, settings);
    weeklyTable.setDataAtCell(dataTable,null,null,'loadData');
    weeklyTable.render();
}

function initComboArea() {
    $.ajax({
        async: false,
        type: "POST",
        url: 'wh.api/combo/GroupAreas',
        success: function (data) {
            widget.setItems({ name: "Area", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            //if (data.length == 1) $('#Area').select2('val', data[0].value);
            $('#Area').prop('selectedIndex', 0)
            $('#Area').change();

        }
    });
}

function initComboDealer() {
    $.ajax({
        async: false,
        type: "POST",
        data: {
            GroupArea : $('#Area').select2('val')
        },
        //url: 'wh.api/SparepartReport/Dealers',
        url: 'wh.api/combo/ComboDealerList',
        success: function (data) {
            widget.setItems({ name: "Dealer", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            //if (data.length == 1) $('#Dealer').select2('val', data[0].value);
            $('#Dealer').prop('selectedIndex', 0)
            $('#Dealer').change();
        }
    });
}

function initComboOutlet() {
    $.ajax({
        async: false,
        type: "POST",
        data: {
            //area: $('#Area').select2('val'),
            companyCode: $('#Dealer').select2('val')
        },
        url: 'wh.api/combo/ComboOutletList',
        success: function (data) {
            widget.setItems({ name: "Outlet", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            //if (data.length == 1) $('#Outlet').select2('val', data[0].value);
            $('#Outlet').prop('selectedIndex', 0)
            $('#Outlet').change();
        }
    });
}

function initComboYear() {
    $.ajax({
        async: false,
        type: "POST",
        url: 'wh.api/SparepartReport/Years',
        success: function (data) {
            widget.setItems({ name: "Year", type: "select", data: data });

            $('#Year').select2('val', new Date().getFullYear())
        }
    });
}

function initComboMonth() {
    $.ajax({
        async: false,
        type: "POST",
        url: 'wh.api/SparepartReport/Months',
        success: function (data) {
            widget.setItems({ name: "Month", type: "select", data: data });

            $('#Month').select2('val', new Date().getMonth() + 1);
        }
    });
    function initComboMonth() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'wh.api/SparepartReport/Months',
            success: function (data) {
                widget.setItems({ name: "Month", type: "select", data: data });

                $('#Month').select2('val', new Date().getMonth() + 1);
            }
        });
    }
}

function initComboType() {
    $.ajax({
        async: false,
        type: "POST",
        url: 'wh.api/Combo/LoadComboData?CodeId=TPGO',
        success: function (data) {
            widget.setItems({ name: "TypeOfGoods", type: "select", data: data });
        }
    });
}


$('#Area').on('change', function () {
    if ($('#Area').val() != "") {
        initComboDealer();
        $('#Dealer').removeAttr('disabled');
    } else {
        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');
        $('#Dealer').select2('val', "");
        $('#Outlet').select2('val', "");
    }
});

$('#Dealer').on('change', function () {
    if ($('#Dealer').val() != "") {
        initComboOutlet();
        $('#Outlet').removeAttr('disabled');
    } else {
        $('#Outlet').attr('disabled', 'disabled');
    }
    $('#Outlet').select2('val', "");
});

function exportToExcel() {
    var data = widget.serializeObject("pnlFilter");

    var areaCode = data.Area;
    var dealerCode = data.Dealer;
    var outletCode = data.Outlet;
    var Year = data.Year;
    var Month = data.Month;
    var Type = data.TypeOfGoods;


    if (data.Area == null || data.Area == undefined || data.Area == "") {
        sdms.info("Area harus diisi", "Warning");
        return;
    }
    if (data.Dealer == null || data.Dealer == undefined || data.Dealer == "") {
        sdms.info("Dealer harus diisi", "Warning");
        return;
    }

    sdms.info("Please wait...");

    window.location.href = 'wh.api/SparepartReport/GenerateExcelWeekly?CompanyCode=' + dealerCode + '&BranchCode=' + outletCode
    + '&PeriodYear=' + Year + '&PeriodMonth=' + Month + '&TypeOfGoods=' + Type;
};


