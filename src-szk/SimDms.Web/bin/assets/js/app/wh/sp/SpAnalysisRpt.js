var widget = new SimDms.Widget({
    title: 'Sparepart Analysis Report SGP',
    xtype: 'panels',
    toolbars: [
        { text: 'Export to Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o', cls: '' },
    ],
    panels: [
        {
            name: "pnlFilter",
            items: [
                {
                    text: "Periode Awal",
                    type: "controls",
                    items: [
                        { name: "StartYear", type: "select", cls: "span2" },
                        { name: "StartMonth", type: "select", cls: "span2" },
                    ]
                },
                {
                    text: "Periode Akhir",
                    type: "controls",
                    items: [
                        { name: "EndYear", type: "select", cls: "span2" },
                        { name: "EndMonth", type: "select", cls: "span2" },
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
            ]
        },
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'exportToExcel':
                exportToExcel();
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
});

function initComboArea() {
    $.ajax({
        async: false,
        type: "POST",
        //url: 'wh.api/SparepartReport/Areas',
        //url: 'wh.api/combo/Areas',
        url: 'wh.api/combo/GroupAreas',
        success: function (data) {
            widget.setItems({ name: "Area", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            if (data.length == 1) $('#Area').select2('val', data[0].value);
        }
    });
}

function initComboDealer() {
    $.ajax({
        async: false,
        type: "POST",
        data: {
            //area: $('#Area').select2('val')
            GroupArea: $('#Area').select2('val')
        },
        //url: 'wh.api/SparepartReport/Dealers',
        //url: 'wh.api/combo/ListDealersNew',
        url: 'wh.api/combo/ComboDealerList',
        success: function (data) {
            widget.setItems({ name: "Dealer", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            if (data.length == 1) $('#Dealer').select2('val', data[0].value);
        }
    });
}

function initComboOutlet() {
    $.ajax({
        async: false,
        type: "POST",
        //data: {
        //    area: $('#Area').select2('val'),
        //    dealer: $('#Dealer').select2('val')
        //},
        //url: 'wh.api/SparepartReport/Outlets',
        data: {
            //area: $('#Area').select2('val'),
            //comp: $('#Dealer').select2('val')
            companyCode: $('#Dealer').select2('val'),
        },
        //url: 'wh.api/combo/ListBranchesNew',
        url: 'wh.api/combo/ComboOutletList',
        success: function (data) {
            widget.setItems({ name: "Outlet", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            if (data.length == 1) $('#Outlet').select2('val', data[0].value);
        }
    });
}

function initComboYear() {
    $.ajax({
        async: false,
        type: "POST",
        url: 'wh.api/SparepartReport/Years',
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
        url: 'wh.api/SparepartReport/Months',
        success: function (data) {
            widget.setItems({ name: "StartMonth", type: "select", data: data });
            widget.setItems({ name: "EndMonth", type: "select", data: data });

            $('#StartMonth').select2('val', new Date().getMonth() + 1);
            $('#EndMonth').select2('val', new Date().getMonth() + 1);
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
    $('#Dealer').select2('val', "");
    $('#Outlet').select2('val', "");
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
    var areaCode = $('#Area').val();
    var dealerCode = $('#Dealer').val();
    var outletCode = $('#Outlet').val();
    var startYear = $('#StartYear').val();
    var startMonth = $('#StartMonth').val();
    var endYear = $('#EndYear').val();
    var endMonth = $('#EndMonth').val();

    if ((dealerCode == undefined || dealerCode == '') && (areaCode != undefined && areaCode != '')) {
        sdms.info("Dealer & Area harus diisi", "Warning");
        return;
    }

    //if (outletCode == undefined || outletCode == '') {
    //    sdms.info("Outlet harus diisi", "Warning");
    //    return;
    //}

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
            Area : areaCode,
            DealerCode: dealerCode,
            OutletCode: outletCode,
            StartYear: startYear,
            EndYear: endYear,
            StartMonth: startMonth,
            EndMonth: endMonth
        },
        url: 'wh.api/sparepartreport/ExportToExcel',
        success: function (data) {
            if (data.message == "") {
                sdms.info("Processing... Please Wait");
                location.href = 'wh.api/sparepartreport/DownloadExcelFile?key=' + data.value;
            } else {
                sdms.info(data.message, "Error");
            }
            $('#btnExcel').removeAttr('disabled');
        }
    });


}