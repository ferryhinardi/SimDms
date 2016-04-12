$(document).ready(function () {
    var options = {
        title: "Invalid Employee Data",
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
                            { name: "Area", type: "select", cls: "span4" },
                        ]
                    },
                    {
                        text: "Dealer",
                        type: "controls",
                        items: [
                            { name: "Dealer", type: "select", cls: "span4" },
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
                        text: "Status",
                        type: "controls",
                        items: [
                            { name: "PersonnelStatus", type: "select", opt_text: "-- SELECT ALL --", cls: "span4" },
                        ]
                    },
                    {
                        text: "Case",
                        type: "controls",
                        items: [
                            { name: "Case", type: "select", opt_text: "-- SELECT ALL --", cls: "span6" }
                        ]
                    }
                ]
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(init);

    function init() {
        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');
        initComboArea();
        initComboCase();
        initComboStatus();
    }

    function initComboArea() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'wh.api/combo/GroupAreas',
            success: function (data) {
                widget.setItems({ name: "Area", type: "select", data: data });
                if (data.length == 1) $('#Area').select2('val', data[0].value);
            }
        });
    }

    function initComboDealer() {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                GroupArea: $('#Area').select2('val')
            },
            url: 'wh.api/combo/ComboDealerList',
            success: function (data) {
                widget.setItems({ name: "Dealer", type: "select", data: data });
                if (data.length == 1) $('#Dealer').select2('val', data[0].value);
            }
        });
    }

    function initComboOutlet() {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                companyCode: $('#Dealer').select2('val')
            },
            url: 'wh.api/combo/ComboOutletList',
            success: function (data) {
                widget.setItems({ name: "Outlet", type: "select", data: data, optionalText: "-- SELECT ALL --" });
                if (data.length == 1) $('#Outlet').select2('val', data[0].value);
            }
        });
    }

    function initComboStatus() {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                companyCode: $('#Dealer').select2('val')
            },
            url: 'wh.api/InvalidEmployee/PersonnelStatus',
            success: function (data) {
                widget.setItems({ name: "PersonnelStatus", type: "select", data: data, optionalText: "-- SELECT ALL --" });
            }
        });

    }

    function initComboCase() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'wh.api/InvalidEmployee/Cases',
            success: function (data) {
                widget.setItems({ name: "Case", type: "select", data: data, optionalText: "-- SELECT ALL --" });
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
            $('#Outlet').select2('val', "");
        }
    });

    $('#btnExcel').on('click', function () {
        if ($('#Area').val() == '') {
            sdms.info("Area harus dipilih", "Warning");
            return;
        }

        $('#btnExcel').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: true,
            type: "POST",
            data: {
                groupArea: $('#Area').select2('val'),
                companyCode: $('#Dealer').val(),
                outlet: $('#Outlet').val(),
                status: $('#PersonnelStatus').val(),
                caseNo: $('#Case').val()
            },
            url: 'wh.api/InvalidEmployee/QueryNew',
            success: function (data) {
                if (data.message == "") {
                    console.log(data);
                    location.href = 'wh.api/InvalidEmployee/DownloadExcelFile?key=' + data.value;
                } else {
                    sdms.info(data.message, "Error");
                }
                $('#btnExcel').removeAttr('disabled');
            }
        });
    });
});