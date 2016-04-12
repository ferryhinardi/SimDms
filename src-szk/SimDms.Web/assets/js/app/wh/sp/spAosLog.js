$(document).ready(function () {
    var options = {
        title: 'AOS Log',
        xtype: 'panels',
        toolbars: [
            { name: "btnExcel", text: "Save to Excel", cls: "btn btn-primary", icon: "fa fa-file-excel-o" }
        ],
        panels: [
            {
                items: [
                    {
                        text: "Prod. Type",
                        type: "controls",
                        items: [
                            { name: "ProdType", type: "select", cls: "span2" },
                        ]
                    },
                    {
                        text: "Tahun",
                        type: "controls",
                        items: [
                            { name: "Year", type: "select", cls: "span2" },
                        ]
                    },
                    {
                        text: "Bulan",
                        type: "controls",
                        items: [
                            { name: "Month", type: "select", cls: "span2" },
                        ]
                    },
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
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.render(init);

    function init() {
        $('#Area').attr('disabled', 'disabled');
        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');
        
        initComboProdType();
        initComboYear();
        initComboMonth();
        initComboArea();
    }

    function initComboProdType() {
        var data = [
            { value: '2W', text: '2W' },
            { value: '4W', text: '4W' },
        ]
        widget.setItems({ name: "ProdType", type: "select", data: data });
    }

    function initComboYear() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'wh.api/SpAosLog/Years',
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
            url: 'wh.api/SpAosLog/Months',
            success: function (data) {
                widget.setItems({ name: "Month", type: "select", data: data });

                $('#Month').select2('val', new Date().getMonth() + 1);
            }
        });
    }

    function initComboArea() {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                prodType: $('#ProdType').select2('val')
            },
            url: 'wh.api/Combo/GroupAreas',
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
                prodType: $('#ProdType').select2('val'),
                GroupArea: $('#Area').select2('val')
            },
            url: 'wh.api/combo/ComboDealerList',
            success: function (data) {
                //console.log($('#Dealer').select2('val', data))
                widget.setItems({ name: "Dealer", type: "select", data: data, optionalText: "-- SELECT ALL --" });
                //  (data.length == 1) $('#Dealer').select2('val', data[0].value);
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
                prodType: $('#ProdType').select2('val'),
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

    $('#ProdType').on('change', function () {
        var prodType = $('#ProdType').select2('val');
        if (prodType == "") {
            $('#Area').attr('disabled', 'disabled');
        } else {
            $('#Area').removeAttr('disabled');
        }
        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');
        $('#Area').select2('val', "");
        $('#Dealer').select2('val', "");
        $('#Outlet').select2('val', "");
        initComboArea();
    });

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
        var prodType = $('#ProdType').select2('val');
        if (prodType == undefined || prodType == '') {
            sdms.info("Product Type harus dipilih");
            return;
        }
        var year = $('#Year').select2('val');
        var month = $('#Month').select2('val');
        if (year == undefined || year == '' || month == undefined || month == '') {
            sdms.info("Tahun dan bulan harus diisi");
            return;
        }
        var area = $('#Area').select2('val');
        var dealer = $('#Dealer').select2('val');
        var outlet = $('#Outlet').select2('val');

        $('#btnExcel').attr('disabled', 'disabled');
        sdms.info("Please wait...");
        $.ajax({
            async: false,
            type: "POST",
            data: {
                ProdType: prodType,
                Year: year,
                Month: month,
                AreaCode: area,
                DealerCode: dealer,
                OutletCode: outlet
            },
            url: 'wh.api/SpAosLog/Query',
            success: function (data) {
                if (data.message == "") {
                    location.href = 'wh.api/SpAosLog/DownloadExcelFile?key=' + data.value;
                } else {
                    alert(data.message + "\n" + data.inner, "Error");
                }
                $('#btnExcel').removeAttr('disabled');
            }
        });

    });
});