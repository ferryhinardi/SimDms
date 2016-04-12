var widget = new SimDms.Widget({
    title: 'Generate ITS by Test Drive',
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
                    text: "Periode",
                    type: "controls",
                    cls: 'span8',
                    items: [
                        { name: 'DateFrom', text: 'Date From', type: 'datepicker', cls: 'span2' },
                        { name: 'DateTo', text: 'Date To', type: 'datepicker', cls: 'span2' },
                    ]
                },
                {
                    text: "Area / Dealer / Outlet",
                    type: "controls",
                    cls: 'span8',
                    items: [
                        { name: 'Area', text: 'Area', type: 'select', cls: 'span2', opt_text: '-- SELECT ALL --' },
                        { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span3', opt_text: '-- SELECT ALL --' },
                        { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span3', opt_text: '-- SELECT ALL --' },
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
    var date = d3.time.format('%Y-%m-%d');
    var initial = {
        DateFrom: date.parse(date(new Date).substring(0, 8) + '01'),
        DateTo: new Date()
    }
    widget.post("wh.api/combo/GroupAreas", {}, function (result) {
        widget.bind({
            name: 'Area',
            text: '-- SELECT ALL --',
            data: result,
            defaultAll: true,
            onChange: function () {
                loadDealer();
                //setTimeout(refreshGrid, 400);
                widget.populate({ Outlet: '', Dealer: '' });
            }
        });
        widget.populate(initial);
        //setTimeout(refreshGrid, 1000);
    });
});


//widget.render(function () {
//    var date = d3.time.format('%Y-%m-%d');
//    var initial = { DateFrom: date.parse(date(new Date).substring(0, 8) + '01'), DateTo: new Date() }
//    widget.post("wh.api/combo/PmItsByTestDriveFilter", {}, function (result) {
//        widget.bind({
//            name: 'Area',
//            text: '-- SELECT ALL --',
//            data: result[0],
//            defaultAll: true,
//            onChange: function () {
//                setTimeout(refreshGrid, 400);
//                widget.populate({ Outlet: '' });
//            }
//        });
//        //widget.bind({
//        //    name: 'Dealer',
//        //    text: '-- SELECT ALL --',
//        //    data: result[1],
//        //    parent: 'Area',
//        //    defaultAll: true,
//        //    onChange: function () {
//        //        setTimeout(refreshGrid, 400);
//        //    }
//        //});
//        //widget.bind({
//        //    name: 'Outlet',
//        //    text: '-- SELECT ALL --',
//        //    data: result[2],
//        //    defaultAll: true,
//        //    parent: 'Dealer',
//        //    onChange: function () {
//        //        setTimeout(refreshGrid, 400);
//        //    }
//        //});

//        widget.populate(initial);
//        setTimeout(refreshGrid, 1000);
//    });
//});

function loadDealer()
{
    widget.post("wh.api/combo/ComboDealerList", { groupArea: $('[name=Area]').val() }, function (result) {
        widget.bind({
            name: 'Dealer',
            text: '-- SELECT ALL --',
            data: result,
            defaultAll: true,
            onChange: function () {
                loadOutlet();
                //setTimeout(refreshGrid, 400);
                widget.populate({ Outlet: '' });
            }
        });
        //$("[name=Dealer] option:first").attr("data-groupno", "");
        //$.each($("[name=Dealer] option:not(:first)") || [], function (idx, item) {
        //    $(item).attr("data-groupno", result[idx].groupNo);
        //})
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
                //setTimeout(refreshGrid, 400);
            }
        });
    })
}

function refreshGrid() {
    var filter = widget.serializeObject('pnlFilter');
    widget.kgrid({
        url: "wh.api/inquiry/PmItsByTestDrive",
        name: "pnlResult",
        params: filter,
        serverBinding: true,
        sort: [
            { field: "DealerAbbreviation", dir: "asc" },
            { field: "BranchCode", dir: "asc" },
            { field: "InquiryNumber", dir: "asc" },
        ],
        columns: [
            { field: "Area", width: 280, title: "Area" },
            { field: "CompanyCode", width: 120, title: "Company Code" },
            { field: "DealerAbbreviation", width: 180, title: "Dealer Abbr" },
            { field: "BranchCode", width: 140, title: "Branch Code" },
            { field: "OutletAbbreviation", width: 180, title: "Outlet Abbr" },
            { field: "InquiryNumber", width: 120, title: "Inq No", type: "number" },
            { field: "InquiryDate", width: 140, title: "Inquiry Date", type: "date" },
            { field: "SPKDate", width: 140, title: "SPK Date", type: "date" },
            { field: "TipeKendaraan", width: 180, title: "TipeKendaraan" },
            { field: "Variant", width: 160, title: "Variant" },
            { field: "ColourCode", width: 120, title: "ColourCode" },
            { field: "Transmisi", width: 120, title: "Transmisi" },
            { field: "Inq", width: 80, title: "Inq", type: "number" },
            { field: "InqTestDrive", width: 120, title: "Inq TestDrive", type: "number" },
            { field: "OutsSPK", width: 80, title: "Out SPK", type: "number" },
            { field: "OutsSPKTestDrive", width: 140, title: "Out SPK TestDrive", type: "number" },
            { field: "NewSPK", width: 150, title: "New SPK", type: "number" },
            { field: "NewSPKTestDrive", width: 150, title: "New SPK TestDrive", type: "number" },
            { field: "TotalSPK", width: 150, title: "Total SPK", type: "number" },
            { field: "TotalSPKTestDrive", width: 150, title: "Total SPK TestDrive", type: "number" },
        ],
    });
}

function exportToExcel() {
    var url = "wh.api/inquiryprod/GenerateITSByTestDrive?";
    var params = "DateFrom=" + $('[name="DateFrom"]').val();
    params += "&DateTo=" + $('[name="DateTo"]').val();
    params += "&Area=" + $('[name="Area"]').val();
    params += "&Dealer=" + $('[name="Dealer"]').val();
    params += "&Outlet=" + $('[name="Outlet"]').val();

    url = url + params;
    window.location = url;
}