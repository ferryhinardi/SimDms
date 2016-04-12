$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: 'Generate ITS by Lost Case',
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
                            { name: 'Area', text: 'Area', type: 'select', cls: 'span2', opt_text: '-- SELECT ALL --' },
                            { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span5', opt_text: '-- SELECT ALL --' },
                        ]
                    },
                    {
                        text: "Outlet",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span7', opt_text: '-- SELECT ALL --' },
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
                    if ($('[name="Filter"]').val() == '') { alert('Mohon pilih satu filter ! '); break; }
                    if ($('[name="Area"]').val() == '') { alert('Mohon pilih satu Area ! '); break; }
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
        widget.post("wh.api/Combo/InqITSByLostCaseFilter", {}, function (result) {
            widget.bind({
                name: 'Filter', data: result, defaultAll: true
            })
        })

        widget.post("wh.api/combo/GroupAreas", {}, function (result) {
            widget.bind({
                name: 'Area',
                text: '-- SELECT ALL --',
                data: result,
                defaultAll: true,
                onChange: function () {
                    loadDealer();
                    widget.populate({ Outlet: '', Dealer: '' });
                }
            });
            widget.populate(initial);
        });

        //initElementState();
        //initElementEvent();
    }

    function loadDealer() {
        widget.post("wh.api/combo/ComboDealerList", { groupArea: $('[name=Area]').val() }, function (result) {
            widget.bind({
                name: 'Dealer',
                text: '-- SELECT ALL --',
                data: result,
                defaultAll: true,
                onChange: function () {
                    loadOutlet();
                    widget.populate({ Outlet: '' });
                }
            });
        })
    }

    function loadOutlet() {
        widget.post("wh.api/combo/ComboOutletList", { companyCode: $("[name=Dealer] option:selected").val() }, function (result) {
            widget.bind({
                name: 'Outlet',
                text: '-- SELECT ALL --',
                data: result,
                defaultAll: true,
                onChange: function () {
                }
            });
        })
    }

    
    //function initElementState() {
    //    var setSelectOptions = [
    //        {
    //            name: 'Filter',
    //            url: 'wh.api/Combo/InqITSByLostCaseFilter'
    //        },
    //        {
    //            name: "Area",
    //            url: "wh.api/Combo/GroupAreas",
    //            optionalText: "-- SELECT ALL --"
    //        },
    //        {
    //            name: "Dealer",
    //            url: "wh.api/Combo/DealerListNew",
    //            params: {GroupArea:$("[name=Area]").val()},
    //            optionalText: "-- SELECT ALL --",
    //            //cascade: {
    //            //    setTimeout(function() { name: "Area"}, 200)
    //            //}
    //        },
    //        {
    //            name: "Outlet",
    //            url: "wh.api/Combo/ListOutletsNew",
    //            optionalText: "-- SELECT ALL --",
    //            //cascade: {
    //            //    name: "Dealer"
    //            //}
    //        },
    //    ];
    //    widget.setSelect(setSelectOptions);
    //}

    //function initElementEvent() {
    //    //$('select').on('change', refreshGrid);
    //    $('[name="Area"]').on('change', function () {
    //        $('[name="Outlet"]').val('');
    //    });
    //}

    function refreshGrid() {
        var filter = widget.serializeObject('pnlFilter');

        if (!widget.isNullOrEmpty(filter.Filter) && !widget.isNullOrEmpty(filter.DateFrom) && !widget.isNullOrEmpty(filter.DateTo)) {
            widget.kgrid({
                url: "wh.api/inquiry/PmItsByLostCase",
                name: "pnlResult",
                params: filter,
                serverBinding: true,
                sort: [
                    { field: "Area", dir: "asc" },
                    { field: "DealerAbbreviation", dir: "asc" },
                    { field: "OutletAbbreviation", dir: "asc" },
                    { field: "InquiryDate", dir: "asc" },
                    { field: "InquiryNumber", dir: "asc" },
                ],
                columns: [
                    { field: "Area", width: 250, title: "Area" },
                    { field: "CompanyCode", width: 250, title: "Company Code" },
                    { field: "DealerAbbreviation", width: 250, title: "Dealer Abbr." },
                    { field: "BranchCode", width: 250, title: "Branch Code" },
                    { field: "OutletAbbreviation", width: 250, title: "Outlet Abbr." },
                    { field: "InquiryNumber", width: 250, title: "Inquiry Number" },
                    { field: "InquiryDate", width: 250, title: "Inquiry Date", type: 'date' },
                    { field: "ProspectDate", width: 250, title: "Prospect Date", type: 'date' },
                    { field: "HotProspectDate", width: 250, title: "Hot Prospect Date", type: 'date' },
                    { field: "SPKDate", width: 250, title: "SPK Date", type: 'date' },
                    { field: "LostCaseDate", width: 250, title: "Lost Case Date", type: 'date' },
                    { field: "StatusbeforeLOST", width: 250, title: "Status Before Lost" },
                    { field: "P_Outs", width: 250, title: "P Outs" },
                    { field: "P_New", width: 250, title: "P New" },
                    { field: "HP_Outs", width: 250, title: "HP Outs" },
                    { field: "HP_New", width: 250, title: "HP New" },
                    { field: "SPK_Outs", width: 250, title: "SPK Outs" },
                    { field: "SPK_New", width: 250, title: "SPK New" },
                    { field: "TipeKendaraan", width: 250, title: "Tipe Kendaraan" },
                    { field: "Variant", width: 250, title: "Varian" },
                    { field: "Transmisi", width: 250, title: "Transmisi" },
                    { field: "ColourCode", width: 250, title: "Colour Code" },
                    { field: "LostCaseCategoryDesc", width: 400, title: "Lost Case Category" },
                    { field: "LostCaseReasonDesc", width: 750, title: "Lost Case Reason" },
                    { field: "LostCaseOtherReason", width: 550, title: "Lost Case Other Reason" },
                    { field: "LostCaseVoiceOfCustomer", width: 750, title: "Lost Case Voice of Customer" }
                ],
            });
        }
    }

    function exportToExcel() {
        var url = "wh.api/inquiryprod/GenerateITSByLostCase?";
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
