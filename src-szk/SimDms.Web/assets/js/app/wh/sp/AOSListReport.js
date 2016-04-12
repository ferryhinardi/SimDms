var widget = new SimDms.Widget({
    title: 'Generate AOS List Report',
    xtype: 'panels',
    toolbars: [
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
                widget.populate({ Outlet: '', Dealer: '' });
            }
        });
        widget.populate(initial);
    });
});


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

function exportToExcel() {
    var url = "wh.api/SpAOSLog/AOSListExport?";
    var params = "DateFrom=" + $('[name="DateFrom"]').val();
    params += "&DateTo=" + $('[name="DateTo"]').val();
    params += "&Area=" + $('[name="Area"]').val();
    params += "&Dealer=" + $('[name="Dealer"]').val();
    params += "&Outlet=" + $('[name="Outlet"]').val();

    url = url + params;
    window.location = url;
}