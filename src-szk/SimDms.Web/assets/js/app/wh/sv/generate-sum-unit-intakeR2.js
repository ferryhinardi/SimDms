var widget = new SimDms.Widget({
    title: 'Generate Summary Unit Intake',
    xtype: 'panels',
    toolbars: [
        { text: 'Generate Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o', cls: '', name:'exportToExcel' }
    ],
    panels: [
        {
            name: "pnlFilter",
            items: [
                {
                    text: "Periode",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'DatePeriod', text: 'Date From', type: 'datepicker', cls: 'span4', format: 'MMM-YYYY' },
                        { name: 'DateTo', text: 'Date To', type: 'datepicker', cls: 'span4 hide' },
                    ]
                },
                {
                    text: "Area",
                    type: "controls",
                    cls: 'span4',
                    items: [
                         { name: 'Area', text: 'Area', type: 'select', cls: 'span8', opt_text: '-- ALL AREA --' },
                    ]
                },
                {
                    text: "Filter",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        {
                            name: 'FilterBy', text: "Filter", cls: "span4", type: "select",
                            items: [
                                { value: "INQ", text: "Job Order Date" },
                                { value: "SPK", text: "SPK Close Date" },
                            ]
                        }
                    ]
                },
                {
                    text: "Dealer",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span8', opt_text: '-- ALL DEALER --' },
                    ]
                },
                {
                    text: "PDI",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'PDI', text: 'PDI', type: 'radio-switch', cls: 'span4', option: { Y: 'Include', N: 'Exclude' } },
                        {

                            name: 'ProductType', text: "Product Type", cls: "span4 hide", type: "select",
                            items: [
                                { value: "2W", text: "2W" },
                                { value: "4W", text: "4W" },
                            ]
                        },
                        {
                            name: 'ReportBy', text: "Report By", cls: "span4 hide", type: "select",
                            items: [
                                { value: "D", text: "Daily" },
                                { value: "W", text: "Weekly" },
                                { value: "M", text: "Monthly" },
                            ]
                        }
                    ]
                },
                {
                    text: "Area / Dealer",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span8', opt_text: '-- ALL OUTLET --' },
                    ]
                },
                //{
                //    text: "Product Type",
                //    type: "controls",
                //    cls: 'span4',
                //    items: [
                //        {
                //            name: 'ProductType', text: "Product Type", cls: "span4", type: "select",
                //            items: [
                //                { value: "2W", text: "2W" },
                //                { value: "4W", text: "4W" },
                //            ]
                //        }
                //    ]
                //}
            ]
        }
    ],
    //onToolbarClick: function (action) {
    //    switch (action) {
    //        case 'exportToExcel':
    //            exportToExcel();
    //            break;
    //        default:
    //            break;
    //    }
    //},
});

widget.render(function () {

    var filter = {
        DatePeriod: new Date(moment(moment().format('YYYY-MM-') + '01')),
        DateTo: new Date(moment(moment().format('YYYY-MM-DD'))),
        FilterBy: 'INQ',
        ProductType: '2W',
        ReportBy: 'D',
        PDI: true
    }


    widget.setSelect([
           {
               name: "Area",
               url: "wh.api/combo/GroupAreas",
               optionalText: "-- ALL -- "
           },
           {
               name: "Dealer",
               url: "wh.api/combo/CompaniesR2",
               optionalText: "-- ALL -- ",
               cascade: {
                   name: "Area",
                   additionalParams: [ 
                        { name: "ProductType", source: "ProductType", type: "select" }
                   ]
               }
               
           },
           {
               name: "Outlet",
               url: "wh.api/combo/Branches",
               optionalText: "-- ALL -- ",
               cascade: {
                   name: "Dealer",
                   additionalParams: [
                        { name: "Area", source: "Area", type: "select" }
                   ]
               }
           }
    ]);
    widget.populate(filter);

});

function exportToExcel()
{
    var params = widget.serializeObject('pnlFilter');
    params.AreaName = $('[name=Area] option:selected').text();
    params.FilterName = $('[name=FilterBy] option:selected').text();
    params.CompanyName = $('[name=Dealer] option:selected').text();
    params.BranchName = $('[name=Outlet] option:selected').text();

    $.fileDownload('doreport/SumUnitIntake.xlsx', {
        httpMethod: "POST",
        //preparingMessageHtml: "We are preparing your report, please wait...",
        //failMessageHtml: "There was a problem generating your report, please try again.",
        data: params
    });
}

$("#exportToExcel").on("click", function (e) {
    var params = widget.serializeObject('pnlFilter');
    params.AreaName = $('[name=Area] option:selected').text();
    params.FilterName = $('[name=FilterBy] option:selected').text();
    params.CompanyName = $('[name=Dealer] option:selected').text();
    params.BranchName = $('[name=Outlet] option:selected').text();

    e.preventDefault();
    $('.page > .ajax-loader').show();

    $.fileDownload('doreport/SumUnitIntake.xlsx', {
        httpMethod: "POST",
        //preparingMessageHtml: "We are preparing your report, please wait...",
        //failMessageHtml: "There was a problem generating your report, please try again.",
        data: params
    }).done(function () {
        $('.page > .ajax-loader').hide();
    });

});
