$(document).ready(function () {
    var options = {
        title: "Turn Over Ratio",
        xtype: "panels",
        toolbars: [
            { name: "btnExcel", text: "Save to Excel", cls: "btn btn-primary", icon: "fa fa-file-excel-o" },
        ],
        panels: [
            {
                items: [
                    {
                        text: "Area",
                        type: "controls",
                        items: [
                            { name: "Area", type: "select", opt_text: "-- SELECT ALL --", cls: "span4" },
                        ]
                    },
                    {
                        text: "Dealer",
                        type: "controls",
                        items: [
                            { name: "Dealer", type: "select", opt_text: "-- SELECT ALL --", cls: "span4" },
                        ]
                    },
                    {
                        text: "Outlet",
                        type: "controls",
                        items: [
                            { name: "Outlet", type: "select", opt_text: "-- SELECT ALL --", cls: "span4" },
                        ]
                    },
                    {
                        text: "Sales Force",
                        type: "controls",
                        items: [
                            { name: "SalesForce", type: "select", opt_text: "-- SELECT ALL --", cls: "span4" },
                        ]
                    },
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
                ]
            },
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(init);

    function initComboArea() {
        $.ajax({
            async: false,
            type: "POST",
            //url: 'wh.api/TurnOverRatio/Areas',
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
            //url: 'wh.api/TurnOverRatio/Dealers',
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
            data: {
                //area: $('#Area').select2('val'),
                //dealer: $('#Dealer').select2('val')
                companyCode: $('#Dealer').select2('val')
            },
            //url: 'wh.api/TurnOverRatio/Outlets',
            url: 'wh.api/combo/ComboOutletList',
            success: function (data) {
                widget.setItems({ name: "Outlet", type: "select", data: data, optionalText: "-- SELECT ALL --" });
                if (data.length == 1) $('#Outlet').select2('val', data[0].value);
            }
        });
    }

    function initComboSalesForce() {
        $.ajax({
            async: false,
            type: "POST",
            url: "wh.api/TurnOverRatio/SalesForces",
            success: function (data) {
                widget.setItems({ name: "SalesForce", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            }
        });
    }

    function initComboYear() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'wh.api/TurnOverRatio/Years',
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
            url: 'wh.api/TurnOverRatio/Months',
            success: function (data) {
                widget.setItems({ name: "StartMonth", type: "select", data: data });
                widget.setItems({ name: "EndMonth", type: "select", data: data });
                
                $('#StartMonth').select2('val', new Date().getMonth() + 1);
                $('#EndMonth').select2('val', new Date().getMonth() + 1);
            }
        });
    }

    function init() {
        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');
        
        initComboArea();
        initComboSalesForce();
        initComboYear();
        initComboMonth();
    }

    $('#Area').on('change', function () {
        if ($('#Area').val() != "") {
            initComboDealer();
            $('#Dealer').removeAttr('disabled');
            $('#Outlet').attr('disabled', 'disabled');
            $('#Dealer').select2('val', "");
            $('#Outlet').select2('val', "");
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
            $('#Outlet').select2('val', "");
        }
    });

    $('#btnExcel').on('click', function () {
        var startYear = $('#StartYear').val();
        var startMonth = $('#StartMonth').val();
        var endYear = $('#EndYear').val();
        var endMonth = $('#EndMonth').val();

        if (startYear == undefined || startMonth == undefined || endYear == undefined || endMonth == undefined ||
            startYear == '' || startMonth == '' || endYear == '' || endMonth == ''){
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
                area: $('#Area').val(),
                dealer: $('#Dealer').val(),
                outlet: $('#Outlet').val(),
                startYear: parseInt(startYear),
                startMonth: parseInt(startMonth),
                endYear: parseInt(endYear),
                endMonth: parseInt(endMonth),
                position: $('#SalesForce').val()
            },
            url: 'wh.api/TurnOverRatio/Query',
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/TurnOverRatio/DownloadExcelFile?key=' + data.value;
                } else {
                    sdms.info(data.message, "Error");
                }
                $('#btnExcel').removeAttr('disabled');
            }
        });
    });
});
