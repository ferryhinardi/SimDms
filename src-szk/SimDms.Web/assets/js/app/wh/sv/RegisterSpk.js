var widget = new SimDms.Widget({
    title: 'Service History',
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
                        { name: 'Area', text: 'Area', type: 'select', cls: 'span2', opt_text: '-- ALL AREA --' },
                        { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span3', opt_text: '-- ALL DEALER --' },
                        { name: 'Outlet', text: 'outlet', type: 'select', cls: 'span3', opt_text: '-- ALL OUTLET --' },
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

widget.setSelect([{ name: "Area", url: "wh.api/combo/SrvGroupAreas", optionalText: "-- SELECT ALL --" }]);
widget.render(function () {
    var filter = {
        DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
        //DateFrom: new Date(),
        DateTo: new Date()
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
    //widget.post("wh.api/combo/RegisterSpkFilter", {}, function (result) {
    //    widget.bind({
    //        name: 'Area',
    //        text: '-- ALL AREA --',
    //        data: result[0],
    //        defaultAll: true,
    //        //onChange: function () {
    //        //    setTimeout(function () { refreshGrid() }, 800)
    //        //}
    //    });
    //    widget.bind({
    //        name: 'Dealer',
    //        text: '-- ALL DEALER --',
    //        data: result[1],
    //        parent: 'Area',
    //        defaultAll: true,
    //        //onChange: function () {
    //        //    setTimeout(function () { refreshGrid() }, 800)
    //        //}
    //    });
    //    widget.bind({
    //        name: 'Outlet',
    //        text: '-- ALL OUTLET --',
    //        data: result[2],
    //        defaultAll: true,
    //        parents: ['Area', 'Dealer'],
    //        onChange: refreshGrid
    //    });
    //});

    //widget.setSelect([
    //      {
    //          name: "Area",
    //          url: "wh.api/combo/SrvGroupAreas",
    //          optionalText: "-- ALL AREA -- "
    //      },
    //      {
    //          name: "Dealer",
    //          url: "wh.api/combo/SrvDealerList",
    //          optionalText: "-- ALL DEALER  -- ",
    //          cascade: {
    //              name: "Area", source: "Area", type: "select"
    //          }

    //      },
    //      {
    //          name: "Outlet",
    //          url: "wh.api/combo/SrvBranchList",
    //          optionalText: "-- ALL OUTLET -- ",
    //          cascade: {
    //              name: "Dealer",
    //              additionalParams: [
    //                   { name: "Area", source: "Area", type: "select" },
    //                   { name: "Comp", source: "Dealer", type: "select" }
    //              ]
    //          }
    //      }
    //]);

    $('select').on('change', ResetCombo);

    widget.populate(filter);
    $('#Dealer, #Outlet').attr('disabled', 'disabled');
    //setTimeout(function () { refreshGrid() }, 800)
});

function refreshGrid() {
    var filter = widget.serializeObject('pnlFilter');
    filter.AreaName = $('[name=Area] option:selected').text();

    widget.kgrid({
        url: "wh.api/inquiry/SvRegisterSpk",
        name: "pnlResult",
        params: filter,
        //sortable: false,
        serverBinding: true,
        sort: [{ field: "JobOrderNo", dir: "asc" }, { field: "DealerCode", dir: "asc" }],
        columns: [
            { field: "DealerName", width: 240, title: "Dealer" },
            { field: "BranchName", width: 380, title: "Outlet" },
            { field: "JobOrderNo", width: 140, title: "No Spk" },
            { field: "JobOrderDate", width: 180, title: "Tgl SPK", type: 'datetime' },
            { field: "BasicModel", width: 140, title: "Model" },
            { field: "PoliceRegNo", width: 140, title: "No Polisi" },
            { field: "Odometer", width: 140, title: "Odometer", type: 'number' },
            { field: "JobType", width: 140, title: "Kode Pekerjaan" },
            { field: "JobTypeDesc", width: 380, title: "Nama Pekerjaan" },
            { field: "TaskPartNo", width: 160, title: "Kode Jasa / Part" },
            { field: "TaskPartName", width: 340, title: "Nama Jasa / Part" },
            { field: "DemandQty", width: 110, title: "Demand Qty", type: 'decimal' },
            { field: "SupplyQty", width: 110, title: "Supply Qty", type: 'decimal' },
            { field: "ReturnQty", width: 110, title: "Return Qty", type: 'decimal' },
            { field: "SupplySlipNo", width: 160, title: "Supply Slip No" },
            { field: "SSReturnNo", width: 160, title: "SS Return No" },
            { field: "SaName", width: 320, title: "SA" },
            { field: "FmName", width: 320, title: "Foreman" },
            { field: "ServiceStatusDesc", width: 220, title: "Status" },
        ],
    });
}


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


function exportToExcel() {
    //var url = 'wh.api/InquiryProd/SvRegisterSpk?';
    //url += 'DateFrom=' + $('[name="DateFrom"]').val();
    //url += '&DateTo=' + $('[name="DateTo"]').val();
    //url += '&Area=' + $('[name="Area"]').val();
    //url += '&Dealer=' + $('[name="Dealer"]').val();
    //url += '&Outlet=' + $('[name="Outlet"]').val();

    //window.location = url;
    var filter = widget.serializeObject('pnlFilter');
    filter.AreaName = $('[name=Area] option:selected').text();

    sdms.report({
        url: 'wh.api/inquiry/SvRegisterSpkXls',
        type: 'xlsx',
        params: filter
    });
}