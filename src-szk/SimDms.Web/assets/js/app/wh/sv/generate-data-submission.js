var widget = new SimDms.Widget({
    title: 'Generate Data - Database Submission Based on Unit Entry',
    xtype: 'panels',
    toolbars: [
        { text: 'Generate Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o', cls: '', name:'exportToExcel' }
    ],
    panels: [
        {
            name: "pnlFilter",
            items: [
                {
                    text: "Periode Start",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'PeriodStart', text: 'Date From', type: 'datepicker', cls: 'span4', format: 'DD/MMM/YYYY' },
                        { name: 'PeriodEnd', text: 'Date End', type: 'datepicker', cls: 'span4', format: 'DD/MMM/YYYY' }
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
                    text: "Group Job",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'JobGroup', text: 'Group Job', type: 'select', cls: 'span8', opt_text: '-- ALL GROUP JOB --' },
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
                    text: "Inquiry By",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        {
                            name: 'FilterBy', text: "Filter", cls: "span4", type: "select",
                            items: [
                                { value: "INQ", text: "Job Order Date" },
                                { value: "SPK", text: "SPK Close Date" },
                            ]
                        },
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
                    text: "Outlet",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span8', opt_text: '-- ALL OUTLET --' },
                    ]
                },
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

widget.setSelect([
       {
           name: "Area",
           url: "wh.api/combo/SrvGroupAreas",
           optionalText: "-- ALL -- "
       },
       {
           name: "JobGroup",
           url: "wh.api/combo/JobGroup",
           optionalText: "-- ALL -- "
       }//,
       //{
       //    name: "Dealer",
       //    url: "wh.api/combo/SrvDealerList",
       //    optionalText: "-- ALL -- ",
       //    cascade: {
       //        name: "Area",
       //        additionalParams: [ 
       //             { name: "ProductType", source: "ProductType", type: "select" }
       //        ]
       //    }

       //},
       //{
       //    name: "Outlet",
       //    url: "wh.api/combo/Branches",
       //    optionalText: "-- ALL -- ",
       //    cascade: {
       //        name: "Dealer",
       //        additionalParams: [
       //             { name: "ProductType", source: "ProductType", type: "select" }
       //        ]
       //    }
       //}
]);
widget.render(function () {

    var filter = {
        PeriodStart: new Date(moment(moment().format('YYYY-MM-') + '01')),
        PeriodEnd: new Date(moment().format('YYYY-MM-DD')),
        FilterBy: 'INQ',
        ProductType: '4W',
        ReportBy: 'D'
    }

    widget.populate(filter);

    $('#Dealer, #Outlet').attr('disabled', 'disabled');
    $("[name=Area]").on("change", function () {
        widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/SrvDealerList", params: { LinkedModule: "mp", GroupArea: $("[name=Area]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=Dealer]").prop("selectedIndex", 0);
        $("[name=Dealer]").change();
    });
    $("[name=Dealer]").on("change", function () {
        widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/SrvBranchList", params: { area: $("#pnlFilter [name=Area]").val(), comp: $("#pnlFilter [name=Dealer]").val() }, optionalText: "-- SELECT ALL --" });
        $("[name=Outlet]").prop("selectedIndex", 0);
        $("[name=Outlet]").change();
    });

    $('select').on('change', ResetCombo);

    $('[name="Area"]').on('change', function () {
        $('[name="Outlet"]').html('<option value="">-- SELECT ALL --</option>');

    });

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
    params.JobName = $('[name=JobGroup] option:selected').text();

    e.preventDefault();
    $('.page > .ajax-loader').show();

    $.fileDownload('doreport/DatabaseSubmission.xlsx', {
        httpMethod: "POST",
        //preparingMessageHtml: "We are preparing your report, please wait...",
        //failMessageHtml: "There was a problem generating your report, please try again.",
        data: params
    }).done(function () {
        $('.page > .ajax-loader').hide();
    });

});
