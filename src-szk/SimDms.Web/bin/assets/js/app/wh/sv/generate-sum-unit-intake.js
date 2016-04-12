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

//widget.setSelect([{ name: "Area", url: "wh.api/combo/GroupAreas", optionalText: "-- SELECT ALL --" }]);
widget.setSelect([{ name: "Area", url: "wh.api/combo/SrvGroupAreas", optionalText: "-- SELECT ALL --" }]);
widget.render(function () {

    var filter = {
        DatePeriod: new Date(moment(moment().format('YYYY-MM-') + '01')),
        DateTo: new Date(moment(moment().format('YYYY-MM-DD'))),
        FilterBy: 'INQ',
        ProductType: '4W',
        ReportBy: 'D',
        PDI: true
    }

    widget.populate(filter);
    $('#Dealer, #Outlet').attr('disabled', 'disabled');

    $("[name=Area]").on("change", function () {
        //widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/DealerList", params: { LinkedModule: "mp", GroupArea: $("[name=Area]").val() }, optionalText: "-- SELECT ALL --" });
        widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/SrvDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=Area]").val() }, optionalText: "-- SELECT ALL --" });

        $("[name=Dealer]").prop("selectedIndex", 0);
        $("[name=Dealer]").change();
    });
    $("[name=Dealer]").on("change", function () {
        //widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/Branchs", params: { area: $("#pnlFilter [name=Area]").val(), comp: $("#pnlFilter [name=Dealer]").val() }, optionalText: "-- SELECT ALL --" });
        widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/SrvBranchList", params: { area: $("#pnlFilter [name=Area]").val(), comp: $("#pnlFilter [name=Dealer]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=Outlet]").prop("selectedIndex", 0);
        $("[name=Outlet]").change();
    });
    //widget.setSelect([
    //       {
    //           name: "Area",
    //           url: "wh.api/combo/SrvGroupAreas",
    //           optionalText: "-- ALL AREA -- "
    //       },
    //       {
    //           name: "Dealer",
    //           url: "wh.api/combo/SrvDealerList",
    //           optionalText: "-- ALL DEALER -- ",
    //           cascade: {
    //               name: "Area",
    //               additionalParams: [ 
    //                    { name: "Area", source: "Area", type: "select" }
    //               ]
    //           }
               
    //       },
    //       {
    //           name: "Outlet",
    //           url: "wh.api/combo/SrvBranchList",
    //           optionalText: "-- ALL OUTLET -- ",
    //           cascade: {
    //               name: "Dealer",
    //               additionalParams: [
    //                    { name: "Area", source: "Area", type: "select" },
    //                    { name: "Comp", source: "Dealer", type: "select" }
    //               ]
    //           }
    //       }
    //]);

    $('select').on('change', ResetCombo);
});

function ResetCombo() {
    if ($('#Area').val() == "") {
        $('#Dealer').val('');
        $('[name="Dealer"]').html('<option value="">-- SELECT ALL --</option>');
        $('#Dealer').attr('disabled', 'disabled');
        $('#Outlet').attr('disabled', 'disabled');
    }
    else {
        $('#Dealer').removeAttr('disabled', 'disabled');
    }

    if ($('#Dealer').val() == "") {
        $('#Outlet').attr('disabled', 'disabled');
    }
    else {
        $('#Outlet').removeAttr('disabled', 'disabled');
    }
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
