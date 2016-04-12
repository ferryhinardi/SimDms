var widget = new SimDms.Widget({
    title: 'Field Technical Information Report - Inquiry',
    xtype: 'panels',
    toolbars: [
            { text: 'Rekap FTIR', action: 'exportToExcel1', icon: 'fa fa-file-excel-o', cls: '', name: 'exportToExcel1' },
            { text: 'Rekap FTIR Detail', action: 'exportToExcel2', icon: 'fa fa-file-excel-o', cls: '', name: 'exportToExcel2' },
        //{ text: 'Form Claim Tag Warranty', action: 'exportToExcel3', icon: 'fa fa-file-excel-o', cls: '', name: 'exportToExcel3' }
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
                    text: "Filter By",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        {
                            name: 'FilterBy', text: "Filter", cls: "span4", type: "select",
                            items: [
                                { value: "Created", text: "Tanggal Dibuat" },
                                { value: "Issue", text: "Tanggal Kejadian" },
                                { value: "Registration", text: "Tanggal Registrasi" },
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
                    text: "Dealer",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span8', opt_text: '-- ALL DEALER --' },
                    ]
                },
                {
                    text: "Model/Type",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'Model', text: 'Model/Type', cls: 'span8' },
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
                {
                    text: "No. FTIR",
                    type: "controls",
                    cls: 'span4',
                    items: [
                        { name: 'FTIRNO', text: 'No. FTIR', cls: 'span8' },
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

widget.render(function () {

    var filter = {
        PeriodStart: new Date(moment(moment().format('YYYY-MM-') + '01')),
        PeriodEnd: new Date(moment().format('YYYY-MM-DD')),
        FilterBy: 'Created',
        ProductType: '4W',
        ReportBy: 'D'
    }

    widget.post('wh.api/svtrans/GetCurrentDealer', function (r) {
        if (r.DealerCode && r.OutletCode) {
            filter.Dealer = r.OutletCode;
            filter.Outlet = r.DealerName;
            $("[name=Area],[name=Dealer],[name=Outlet]").attr("disabled", "disabled");
        }
    });


    widget.populate(filter);

    widget.setSelect([
           {
               name: "Area",
               url: "wh.api/combo/GroupAreas",
               optionalText: "-- ALL -- "
           },
            {
                name: "JobGroup",
                url: "wh.api/combo/JobGroup",
                optionalText: "-- ALL -- "
            },
           {
               name: "Dealer",
               url: "wh.api/combo/Companies",
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
                        { name: "ProductType", source: "ProductType", type: "select" }
                   ]
               }
           }
    ]);

});

function obj_to_query(obj) {
    var parts = [];
    for (var key in obj) {
        if (obj.hasOwnProperty(key)) {
            parts.push(encodeURIComponent(key) + '=' + encodeURIComponent(obj[key]));
        }
    }
    return "?" + parts.join('&');
}


$("#exportToExcel1").on("click", function (e) {
    var params = widget.serializeObject('pnlFilter');
    params.AreaName = $('[name=Area] option:selected').text();
    params.FilterByName = $('[name=FilterBy] option:selected').text();
    params.DealerName = $('[name=Dealer] option:selected').text();
    params.OutletName = $('[name=Outlet] option:selected').text();

    e.preventDefault();

    var url = "wh.api/svtrans/RekapFTIR" + obj_to_query(params);
    window.location = url;

});

$("#exportToExcel2").on("click", function (e) {
    var params = widget.serializeObject('pnlFilter');
    params.AreaName = $('[name=Area] option:selected').text();
    params.FilterByName = $('[name=FilterBy] option:selected').text();
    params.DealerName = $('[name=Dealer] option:selected').text();
    params.OutletName = $('[name=Outlet] option:selected').text();

    e.preventDefault();

    var url = "wh.api/svtrans/RekapFTIR_Dtl" + obj_to_query(params);
    window.location = url;

});
