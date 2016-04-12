var op;

$(document).ready(function () {
    var options = {
        title: "Inquiry Active VIN",
        xtype: "panels",
        toolbars: [
            { action: "Refresh", text: "Refresh", icon: "fa fa-refresh" },
            { action: 'DownloadReport', text: 'Download Report', icon: 'fa fa-file-excel-o' }
        ],
        panels: [
            {
                name: "pnlFilter",
                cls: "full",
                items: [
                    //{
                    //    name: 'Options', text: "Inquiry Option", cls: "span4 full", opt_text: "-- SELECT ONE -- ", type: "select",
                    //    items: [
                    //        { value: "0", text: "Summary VIN" },
                    //        //{ value: "1", text: "Detail VIN per Tahun" },
                    //        { value: "2", text: "Invalid VIN" },
                    //    ]
                    //},
                    { name: "ActiveVIN", text: "Tipe Active VIN", cls: "span4 full", type: "select", readonly: false, opt_text: "-- SELECT ALL -- " },//  disabled: 'disabled'cls: "hide actv", },
                    {
                        name: "Production",
                        text: "Production Year Period",
                        type: "controls",
                        //cls: "hide prod",
                        items: [
                            { name: "prodYearStart", cls: "span2", type: "select", opt_text: "-- SELECT ALL -- " },
                            { name: "", text:"to", cls: "span1", type: "label", },
                            { name: "prodYearEnd", cls: "span2", type: "select",  }
                        ]
                    },
                    { name: "PeriodYearVin", text: "Period of Active-VIN", cls: "span2", type: "select", readonly: false}, //cls: "hide vin" },
                    {
                        text: "Population Based of Information",
                        type: "controls",
                        items: [
                            { name: "UIOYearStart", cls: "span2", type: "select", },
                            { name: "", text: "to", cls: "span1", type: "label", },
                            { name: "UIOYearEnd", cls: "span2", type: "select", }
                        ]
                    },
                    { name: "Area", text: "Area", cls: "span4 full", type: "select", opt_text: "-- SELECT ALL -- " },
                    { name: "Dealer", text: "Dealer", cls: "span4 full", type: "select", disabled: 'disabled', opt_text: "-- SELECT ALL -- " },
                    { name: "Outlet", text: "Outlet", cls: "span4 full", type: "select", disabled: 'disabled', opt_text: "-- SELECT ALL-- " },
                ]
            }
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'Refresh':
                    break;
                case 'DownloadReport':
                    generateReport();
                    //if (op == undefined) {
                    //    widget.showNotification("Option Harus dipilih terlebih dahulu!");
                    //    return;
                    //}
                    //else {
                    //    generateReport();
                    //}
                    break;
                default:
                    break;
            }
        }
    }

    var widget = new SimDms.Widget(options);
    widget.render(function () {
        initComboYear();
        initComboArea();
        initComboDealer();
        initComboOutlet();
        //$('#Production').hide();
        widget.post('wh.api/TargetVIN/ActiveVINType', { option: 0 }, function (data) {
            widget.setItems({ name: "ActiveVIN", type: "select", data: data });
        });
        $('.prod').hide();
        $('.actv').hide();
        $('.vin').hide();
    });

    $('#Options').on('change', function (e) {
        op = e.val;

        if (op){
            widget.post('wh.api/TargetVIN/ActiveVINType', { option: op }, function (data) {
                widget.setItems({ name: "ActiveVIN", type: "select", data: data });
            });
        }

        if (op == undefined) {
            $('#ActiveVIN').attr('disabled', 'disabled');
        }
        else {
            $('#ActiveVIN').removeAttr('disabled');
            if (op == 0) {
                $('.prod').show();
                $('.actv').show();
                $('.vin').show();
            }
            else if (op == 1) {
                $('.vin').show();
                $('.prod').show();
                $('.actv').hide();
            } else {
                $('.prod').hide();
                $('.actv').hide();
                $('.vin').show();
            }
        }
    });

    function generateReport() {
        var area = $('[name="Area"]').val();
        var dealer = $('[name="Dealer"]').val();
        var outlet = $('[name="Outlet"]').val();
        var prodyearStart = $('[name="prodYearStart"]').val();
        var prodyearend = $('[name="prodYearEnd"]').val();
        var uioyearStart = $('[name="UIOYearStart"]').val();
        var uiodyearend = $('[name="UIOYearEnd"]').val();
        var perodyearvin = $('[name="PeriodYearVin"]').val();
        var typeAktifVin = $('[name="ActiveVIN"]').val();
        //alert('area: ' + area + ' dealer: ' + dealer + ' outlet: ' + outlet + ' prodyearStart: ' + prodyearStart + ' prodyearend: ' + prodyearend + ' uioyearStart: ' + uioyearStart + ' uiodyearend: ' + uiodyearend + ' perodyearvin: ' + perodyearvin);
        //alert(typeAktifVin);
        sdms.info("Please wait...");
        var url = "";
        var params = "&area=" + area;
        params += "&dealer=" + dealer; //+ $('[name="CompanyCode"]').val();
        params += "&outlet=" + outlet;//+ $('[name="BranchCode"]').val();
        params += "&yearStart=" + prodyearStart;
        params += "&yearEnd=" + prodyearend;//+ $('[name="Month"]').val();
        params += "&VIN=" + perodyearvin;//+ $('[name="Year"]').val();
        params += "&uiostart=" + uioyearStart;//+ $('[name="Year"]').val();
        params += "&uioend=" + uiodyearend;
        params += "&typeAktifVin=" + typeAktifVin;
        //params += "&istype=0"; //+ $('[name="Penjualan"]').val();
        if (op == undefined) {
            url = "wh.api/TargetVIN/GenerateSummaryVIN?";
        }
        else {
            if (op == 0) {
                url = 'wh.api/TargetVIN/GenerateSummaryVIN?';
            }else if (op == 2) {
                url = 'wh.api/TargetVIN/GenerateExcelInvalid?';
            }
            else {
                url = 'wh.api/TargetVIN/GenerateDetailVIN?';
            }
        }
        console.log(params);
        url = url + params;
        window.location.href = url
    };

    function initComboYear() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'wh.api/Combo/ListOfYearMin10',
            success: function (data) {
                widget.setItems({ name: "prodYearStart", type: "select", data: data });
                widget.setItems({ name: "prodYearEnd", type: "select", data: data });
                widget.setItems({ name: "PeriodYearVin", type: "select", data: data });
                widget.setItems({ name: "UIOYearStart", type: "select", data: data });
                widget.setItems({ name: "UIOYearEnd", type: "select", data: data });

                $('#prodYearEnd, #PeriodYearVin, #UIOYearStart, #UIOYearEnd').select2('val', new Date().getFullYear())
                $('#UIOYearStart').select2('val', (new Date().getFullYear() - 7))
                //$('#prodYearStart').select2('val', 1980)

                var periodYear = document.getElementById('PeriodYearVin')
                periodYear.remove(0);
            }
        });
    }

    function initComboArea() {
        $.ajax({
            async: false,
            type: "POST",
            url: 'wh.api/Combo/SrvAreas',
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
                groupArea: $('#Area').select2('val')
            },
            url: 'wh.api/combo/SrvDealerListVin',
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
                area: $('#Area').select2('val'),
                comp: $('#Dealer').select2('val')
            },
            url: 'wh.api/combo/SrvBranchListVin',
            success: function (data) {
                widget.setItems({ name: "Outlet", type: "select", data: data });
                if (data.length == 1) $('#Outlet').select2('val', data[0].value);
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

    
});

