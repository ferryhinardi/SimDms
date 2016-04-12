$(document).ready(function () {
    var options = {
        title: 'Monthly Sales Indicator',
        xtype: 'panels',
        toolbars: [
            { name: "btnExcel", text: "Save to Excel", cls: "btn btn-primary", icon: "fa fa-file-excel-o" }
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
                        text: "Report Type",
                        type: "controls",
                        items: [
                            { name: "ReportType", type: "select", cls: "span4" },
                        ]
                    },
                    {
                        text: "Month to date",
                        type: "controls",
                        items: [
                            { name: "Year", type: "select", cls: "span2" },
                            { name: "Month", type: "select", cls: "span2" },
                        ]
                    },
                ]
            },
        ],
    };

    var widget = new SimDms.Widget(options);
    widget.render(init);

    function initComboArea() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'wh.api/Mssi/Areas',
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
                area: $('#Area').select2('val')
            },
            url: 'wh.api/Mssi/Dealers',
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
                area: $('#Area').select2('val'),
                dealer: $('#Dealer').select2('val')
            },
            url: 'wh.api/Mssi/Outlets',
            success: function (data) {
                widget.setItems({ name: "Outlet", type: "select", data: data, optionalText: "-- SELECT ALL --" });
                if (data.length == 1) $('#Outlet').select2('val', data[0].value);
            }
        });
    }

    function initComboRptType() {
        widget.setItems(
            {
                name: "ReportType", type: "select", data: [
                    { value: "C", text: "Calendar Year (Jan-Dec)" },
                    { value: "F", text: "Fiscal Year (Apr-Mar)" },
                ]
            }
        );

        $('#ReportType').select2('val', 'C');
    }

    function initComboYear() {
        var rptType = $('#ReportType').select2('val');
        $.ajax({
            async: false,
            type: "POST",
            data: {
                rptType: rptType
            },
            url: 'wh.api/Mssi/Years',
            success: function (data) {
                widget.setItems({ name: "Year", type: "select", data: data });

                if (rptType == 'F') {
                    if (new Date().getMonth() < 4)
                        $('#Year').select2('val', new Date().getFullYear());
                    else if (new Date().getMonth() >= 4)
                        $('#Year').select2('val', new Date().getFullYear());
                }
                else $('#Year').select2('val', new Date().getFullYear());
            }
        });
    }

    function initComboMonth() {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                rptType: $('#ReportType').select2('val')
            },
            url: 'wh.api/Mssi/Months',
            success: function (data) {
                widget.setItems({ name: "Month", type: "select", data: data });

                $('#Month').select2('val', new Date().getMonth() + 1);
            }
        });
    }

    function init() {
        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');

        initComboArea();
        initComboYear();
        initComboRptType();
        initComboMonth();
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

    $('#ReportType').on('change', function () {
        initComboYear();
        initComboMonth();
    });

    $('#btnExcel').on('click', function () {
        var year = $('#Year').select2('val');
        var month = $('#Month').select2('val');
        if (year == undefined || year == '' || month == undefined || month == '') {
            sdms.info("Month to date harus diisi");
            return;
        }
        $('#btnExcel').attr('disabled', 'disabled');
        sdms.info("Please wait. The process might take more than 5 minutes.");
        $.ajax({
            async: true,
            type: "POST",
            data: {
                RptType: $('#ReportType').select2('val'),
                AreaCode: $('#Area').val(),
                DealerCode: $('#Dealer').val(),
                OutletCode: $('#Outlet').val(),
                Year: year,
                YTDMonth: month
            },
            url: 'wh.api/Mssi/Query',
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/Mssi/DownloadExcelFile?key=' + data.value + '&r=' + $('#ReportType').select2('val');
                } else {
                    alert(data.message + "\n" + data.inner, "Error");
                }
                $('#btnExcel').removeAttr('disabled');
            }
        });
    });
});
