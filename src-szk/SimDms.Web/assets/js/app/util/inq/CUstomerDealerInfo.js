var widget = new SimDms.Widget({
    title: 'Customer Dealer',
    xtype: 'panels',
    toolbars: [
        //{ text: 'Refresh', action: 'refresh', icon: 'fa fa-refresh' },
        //{ text: 'Expand', action: 'collapse', icon: 'fa fa-expand' },
        //{ text: 'Collapse', action: 'expand', icon: 'fa fa-compress', cls: 'hide' },
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
                    text: "Area / Dealer",
                    type: "controls",
                    cls: 'span8',
                    items: [
                        { name: 'Area', text: 'Area', type: 'select', cls: 'span2', opt_text: '-- ALL AREA --' },
                        { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span3', opt_text: '-- ALL DEALER --' },
                        { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span3', opt_text: '-- ALL OUTLET --' },
                    ]
                },
            ]
        },
        //{
        //    name: "pnlResult",
        //    xtype: "k-grid",
        //},
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

//widget.setSelect([
//    {
//        name: "Area",
//        url: "wh.api/combo/GroupAreas",
//        optionalText: "-- SELECT ALL -- "
//    },
//    {
//        name: "Dealer",
//        url: "wh.api/combo/Companies",
//        optionalText: "-- SELECT ALL -- ",
//        //defaultValue: '6159401000',
//        cascade: {
//            name: "Area"
//        }
//    },
//    {
//        name: "Outlet",
//        url: "wh.api/combo/Branches",
//        optionalText: "-- SELECT ALL -- ",
//        cascade: {
//            name: "Dealer"
//        }
//    }
//]);

//widget.setSelect([{ name: "Area", url: "wh.api/combo/Areas", optionalText: "-- ALL AREA --" }]);
widget.setSelect([{ name: "Area", url: "wh.api/combo/GroupAreas", optionalText: "-- ALL AREA --" }]);

widget.render(function () {
    
    var filter = {
        DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
        DateTo: new Date() //, Area: '100', Dealer: '6159401000'
    }

    widget.populate(filter);

    //$('[name=Area').trigger('select');

    $("[name=Area]").on("change", function () {
        //widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/ListDealersNew", params: { area: $("#pnlFilter [name=Area]").val() }, optionalText: "-- ALL DEALER --" });
        widget.select({ selector: "[name=Dealer]", url: "wh.api/combo/ComboDealerList", params: { GroupArea: $("#pnlFilter [name=Area]").val() }, optionalText: "-- ALL DEALER --" });
        $("[name=Dealer]").prop("selectedIndex", 0);
        $("[name=Dealer]").change();

        //console.log($("#pnlFilter [name=Area]").val(), $("#pnlFilter [name=Dealer]").val(), $("#pnlFilter [name=Outlet]").val(""));
    });
    $("[name=Dealer]").on("change", function () {
        //widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/ListBranchesNew", params: { area: $("#pnlFilter [name=Area]").val(), comp: $('#Dealer option:selected').text(), compText: $("#Dealer option:selected").text() }, optionalText: "-- ALL OUTLET --" });
        widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/ComboOutletList", params: { companyCode: $("#pnlFilter [name=Dealer]").val() }, optionalText: "-- ALL OUTLET --" });
        $("[name=Outlet]").prop("selectedIndex", 0);
        $("[name=Outlet]").change();

        //console.log($("#pnlFilter [name=Dealer]").val(), $('#Dealer option:selected').text());
    });

});

function refreshGrid() {

}

function exportToExcel() {
    var params = widget.serializeObject('pnlFilter');
    params.AreaName = $('[name=Area] option:selected').text();
    params.CompanyName = $('[name=Dealer] option:selected').text();
    params.BranchName = $('[name=Outlet] option:selected').text();

    console.log(params);
    //$('.page > .ajax-loader').show();

    $.fileDownload('doreport/CustomerDealerInfo.xlsx', {
        httpMethod: "POST",
        //preparingMessageHtml: "We are preparing your report, please wait...",
        //failMessageHtml: "There was a problem generating your report, please try again.",
        data: params
    }).done(function () {
        $('.page > .ajax-loader').hide();
    });

}