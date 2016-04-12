var widget = new SimDms.Widget({
    title: "Inquiry Data Coupon",
    xtype: 'panels',
    toolbars: [
        { text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
        { name: "btnProcess", text: "Export (xls)", action: 'Process', icon: "fa fa-file-excel-o" },
    ],
    panels: [
        {
            name: "pnlFilter",
            items: [
                 {
                     text: "Test Drive Date",
                     type: "controls",
                     cls: 'span6',
                     items: [
                         { name: 'DateFrom', type: 'datepicker', cls: 'span3' },
                         { name: 'label2', text: 's/d', type: 'label', cls: 'span1' },
                         { name: 'DateTo', type: 'datepicker', cls: 'span3' },
                     ]
                 },
                 {type: 'hr'},
                 {
                     text: "Area / Dealer / Outlet",
                     type: "controls",
                     cls: 'span8',
                     items: [
                         { name: 'Area', text: 'Area', type: 'select', cls: 'span2', opt_text: '-- ALL AREA --' },
                         { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span3', opt_text: '--ALL DEALER --' },
                         { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span3', opt_text: '-- ALL OUTLET --' },
                     ]
                 },
                 {
                     text: "Coupon No",
                     type: "controls",
                     cls: 'span6',
                     items: [
                         { name: 'BeginCoupon', text: 'BeginCoupon', type: 'select', cls: 'span3', opt_text: '-- ALL COUPON --' },
                         { name: 'label1', text: 's/d', type: 'label', cls: 'span1' },
                         { name: 'EndCoupon', text: 'EndCoupon', type: 'select', cls: 'span3', opt_text: '-- ALL COUPON --' },
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
            case 'Process':
                exportToExcel();
                break;
            default:
                break;
        }
    },
});

function initElementState() {
    var setSelectOptions = [
        {
            name: "Area",
            url: "wh.api/Combo/GroupAreas",
            optionalText: "-- ALL AREA --"
        },
        {
            name: "Dealer",
            url: "wh.api/Combo/Companies",
            optionalText: "--ALL DEALER --",
            cascade: {
                name: "Area"
            }
        },
        {
            name: "Outlet",
            url: "wh.api/Combo/Branches",
            optionalText: "-- ALL OUTLET --",
            cascade: {
                name: "Dealer"
            }
        },
        {
            name: "BeginCoupon",
            url: "wh.api/Combo/CouponNo",
            optionalText: "-- ALL COUPON --",
            cascade: {
                name: "Outlet",
                additionalParams: [
                    { name: "DateFrom", source: "DateFrom", type: "datepicker" },
                    { name: "DateTo", source: "DateTo", type: "datepicker" },
                    { name: "Dealer", source: "Dealer", type: "select" },
                    { name: "Outlet", source: "Outlet", type: "select" }
                ]
            }
        },
        {
            name: "EndCoupon",
            url: "wh.api/Combo/CouponNo",
            optionalText: "-- ALL COUPON --",
            cascade: {
                name: "Outlet",
                additionalParams: [
                    { name: "DateFrom", source: "DateFrom", type: "datepicker" },
                    { name: "DateTo", source: "DateTo", type: "datepicker" },
                    { name: "Dealer", source: "Dealer", type: "select" },
                    { name: "Outlet", source: "Outlet", type: "select" }
                ]
            },
        },
    ];
    widget.setSelect(setSelectOptions);
}

function initElementEvent() {

    $('#Area').on('change', function (e) {
        if ($('#Area').val() == "") {
            $('#Dealer, #Outlet').attr('disabled', 'disabled');
        }
        else {
            $('#Dealer').removeAttr('disabled', 'disabled');
        }
    });

    $('#Dealer').on('change', function (e) {
        if ($('#Dealer').val() == "") {
            $('#Outlet').attr('disabled', 'disabled');
        }
        else {
            $('#Outlet').removeAttr('disabled', 'disabled');
        }
    });

    $('#Outlet').on('change', function (e) {
        if ($('#Outlet').val() == "") {
            $('#BeginCoupon, #EndCoupon').attr('disabled', 'disabled');
        }
        else {
            $('#BeginCoupon, #EndCoupon').removeAttr('disabled', 'disabled');
        }
    });
}

widget.render(function () {
    $('#Dealer, #Outlet, #BeginCoupon, #EndCoupon').attr('disabled', 'disabled');
    $('#pnlFilter input').on('change', function() {
        $('#Outlet').select2('val', '');
        $('#BeginCoupon, #EndCoupon').attr('disabled', 'disabled');
    });
    renderCallback();
});

function renderCallback() {
    var date = d3.time.format('%Y-%m-%d');
    var initial = { DateFrom: date.parse(date(new Date).substring(0, 8) + '01'), DateTo: new Date() }
    widget.populate(initial);
    initElementState();
    initElementEvent();
}

function refreshGrid() {
    var filter = widget.serializeObject('pnlFilter');
    widget.kgrid({
        url: "wh.api/inquiry/InqKdpDataCoupon",
        name: "pnlResult",
        params: filter,
        serverBinding: true,
        sort: [
            { field: "CoupunNumber", dir: "asc" },
            { field: "TestDriveDate", dir: "asc" },
        ],
        columns: [
            { field: "CoupunNumber", width: 120, title: "Coupon No" },
            { field: "TestDriveDate", width: 150, title: "Test Drive Date" },
            { field: "NamaProspek", width: 250, title: "Customer Name" },
            { field: "ProspekIdentityNo", width: 150, title: "No SIM A" },
            { field: "AlamatProspek", width: 400, title: " Customer Address" },
            { field: "TelpRumah", width: 150, title: "Telp/HP" },
            { field: "Email", width: 200, title: "Email" },
            { field: "EmployeeName", width: 150, title: "Saleman" },
            { field: "SalesID", width: 100, title: "ID SIS(ITS)" },
            { field: "IdentityNo", width: 150, title: "No. KTP" },
            { field: "OutletName", width: 350, title: "Dealer" },
            { field: "OutletArea", width: 120, title: "Daerah" },
            { field: "Remark", width: 200, title: "Remark" },
        ],
    });
}

function exportToExcel() {
    var url = "wh.api/inquiryprod/GenerateInqDataCoupon?";
    var params = "DateFrom=" + $('[name="DateFrom"]').val();
    params += "&DateTo=" + $('[name="DateTo"]').val();
    params += "&Area=" + $('[name="Area"]').val();
    params += "&Dealer=" + $('[name="Dealer"]').val();
    params += "&Outlet=" + $('[name="Outlet"]').val();
    params += "&BeginCoupon=" + $('[name="BeginCoupon"]').val();
    params += "&EndCoupon=" + $('[name="EndCoupon"]').val();

    url = url + params;
    window.location = url;
}


