$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: 'Generate ITS by Perolehan Data',
        xtype: 'panels',
        toolbars: [
            { text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
            { text: 'Expand', action: 'collapse', icon: 'fa fa-expand' },
            { text: 'Collapse', action: 'expand', icon: 'fa fa-compress', cls: 'hide' },
            { text: 'Export to Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o', cls: '' },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Filter",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Filter', text: 'Filter', type: 'select', cls: 'span3', opt_text: '-- SELECT ONE --' },
                        ]
                    },
                    {
                        text: "Periode",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'DateFrom', text: 'Date From', type: 'datepicker', cls: 'span2' },
                            { name: 'DateTo', text: 'Date To', type: 'datepicker', cls: 'span2' },
                        ]
                    },
                    {
                        text: "Area / Dealer",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Area', text: 'Area', type: 'select', cls: 'span2', opt_text: '-- ALL AREA --' },
                            { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span5', opt_text: '-- ALL DEALER --' },
                        ]
                    },
                    {
                        text: "Outlet",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span7', opt_text: '-- ALL OUTLET --' },
                        ]
                    },
                ]
            },
            {
                name: "pnlResult",
                xtype: "k-grid",
            },
        ],
        onToolbarClick: function (action) {
            switch (action) {
                case 'refresh':
                    refreshGrid();
                    break;
                case 'collapse':
                    widget.requestFullWindow();
                    widget.showToolbars(['refresh', 'expand', 'exportToExcel']);
                    break;
                case 'expand':
                    widget.exitFullWindow();
                    widget.showToolbars(['refresh', 'collapse', 'exportToExcel']);
                    break;
                case 'exportToExcel':
                    exportToExcel();
                    break;
                default:
                    break;
            }
        },
    });
    
    widget.render(function () {
        renderCallback();
    });

    function renderCallback() {
        var date = d3.time.format('%Y-%m-%d');
        var initial = { DateFrom: date.parse(date(new Date).substring(0, 8) + '01'), DateTo: new Date() }
        widget.populate(initial);
        initElementState();
        initElementEvent();

        $("[name=Area]").on("change", function () {
            widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=Area]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Dealer]").prop("selectedIndex", 0);
            $("[name=Dealer]").change();
        });
        $("[name=Dealer]").on("change", function () {
            widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/ComboOutletList", params: { companyCode: $("#pnlFilter [name=Dealer]").val() }, optionalText: "-- SELECT ALL --" });
            $("[name=Outlet]").prop("selectedIndex", 0);
            $("[name=Outlet]").change();
        });

        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');
    }

    function initElementState() {
        var setSelectOptions = [
            {
                name: 'Filter',
                url: 'wh.api/Combo/InqITSByPerolehanDataFilter'
            },
            {
                name: "Area",
                url: "wh.api/Combo/GroupAreas",
                optionalText: "-- ALL AREA --"
            },
            //{
            //    name: "Dealer",
            //    //url: "wh.api/Combo/Companies",
            //    url: "wh.api/Combo/ComboDealerList",
            //    optionalText: "--ALL DEALER --",
            //    cascade: {
            //        name: "Area"
            //    }
            //},
            //{
            //    name: "Outlet",
            //    //url: "wh.api/Combo/Branches",
            //    url: "wh.api/Combo/ComboOutletList",
            //    optionalText: "-- ALL OUTLET --",
            //    cascade: {
            //        name: "Dealer"
            //    }
            //},
        ];
        widget.setSelect(setSelectOptions);
    }

    function initElementEvent() {
        //$('select').on('change', refreshGrid);
        //$('[name="Area"]').on('change', function () {
        //    $('[name="Outlet"]').val('');
        //});

        $('#Area').on('change', function () {
            if ($('#Area').val() != "") {
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
                $('#Outlet').removeAttr('disabled');
            } else {
                $('#Outlet').attr('disabled', 'disabled');
            }
            $('#Outlet').select2('val', "");
        });
    }

    function refreshGrid() {
        var filter = widget.serializeObject('pnlFilter');

        if (widget.isNullOrEmpty(filter.Filter)) {
            sdms.info({ type: "warning", text: "Filter tidak boleh kosong" });
            return;
        }

        if (!widget.isNullOrEmpty(filter.Filter) && !widget.isNullOrEmpty(filter.DateFrom) && !widget.isNullOrEmpty(filter.DateTo)) {
            var sorts = [];
            var columns = [];

            if (filter.Filter == "0") {
                sorts = [
                    { field: "Area", dir: "asc" },
                    { field: "DealerAbbreviation", dir: "asc" },
                    { field: "OutletAbbreviation", dir: "asc" },
                    { field: "Period", dir: "asc" },
                    { field: "TipeKendaraan", dir: "asc" },
                    { field: "Variant", dir: "asc" },
                    { field: "Transmisi", dir: "asc" },
                    { field: "PerolehanData", dir: "asc" },
                    { field: "LastProgress", dir: "asc" },
                ];

                columns = [
                    { field: "Area", width: 300, title: "Area" },
                    { field: "CompanyCode", width: 150, title: "Company Code" },
                    { field: "DealerAbbreviation", width: 300, title: "Dealer Abbr." },
                    { field: "BranchCode", width: 150, title: "Branch Code" },
                    { field: "OutletAbbreviation", width: 500, title: "Outlet Abbr." },
                    { field: "Period", width: 150, title: "Period" },
                    { field: "InquiryDate", width: 150, title: "Inquiry Date", type: 'date' },
                    { field: "TipeKendaraan", width: 250, title: "Tipe Kendaraan" },
                    { field: "Variant", width: 250, title: "Varian" },
                    { field: "Transmisi", width: 150, title: "Transmisi" },
                    { field: "PerolehanData", width: 150, title: "Perolehan Data" },
                    { field: "LastProgress", width: 150, title: "Last Progress" },
                    { field: "Total", width: 150, title: "Total" },
                ];
            }
            else if (filter.Filter == "1") {
                columns = [
                    { field: "Area", width: 300, title: "Area" },
                    { field: "CompanyCode", width: 150, title: "Company Code" },
                    { field: "DealerAbbreviation", width: 300, title: "Dealer Abbr." },
                    { field: "BranchCode", width: 150, title: "Branch Code" },
                    { field: "OutletAbbreviation", width: 500, title: "Outlet Abbr." },
                    { field: "Period", width: 150, title: "Period" },
                    { field: "TipeKendaraan", width: 250, title: "Tipe Kendaraan" },
                    { field: "Variant", width: 250, title: "Varian" },
                    { field: "Transmisi", width: 150, title: "Transmisi" },
                    { field: "PerolehanData", width: 150, title: "Perolehan Data" },
                    { field: "LastProgress", width: 150, title: "Last Progress" },
                    { field: "Total", width: 150, title: "Total" },
                ];
            }

            widget.kgrid({
                url: "wh.api/inquiry/PmItsByPerolehanData",
                name: "pnlResult",
                params: filter,
                serverBinding: true,
                sort: sorts,
                columns: columns
            });
        }
    }

    function exportToExcel() {
        var url = "wh.api/inquiryprod/GenerateITSByPerolehanData?";
        var params = "DateFrom=" + $('[name="DateFrom"]').val();
        params += "&DateTo=" + $('[name="DateTo"]').val();
        params += "&Area=" + $('[name="Area"]').val();
        params += "&Dealer=" + $('[name="Dealer"]').val();
        params += "&Outlet=" + $('[name="Outlet"]').val();
        params += "&Filter=" + $('[name="Filter"]').val();

        url = url + params;
        window.location = url;
    }
});
