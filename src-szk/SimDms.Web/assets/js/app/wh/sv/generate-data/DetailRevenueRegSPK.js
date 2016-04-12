var rbDataSource;
var widget = new SimDms.Widget({
    title: 'Detail Revenue Register SPK',
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
                    text: "Area",
                    type: "controls",
                    cls: 'span6',
                    items: [
                        { name: 'Area', text: 'Area', type: 'select', opt_text: '-- ALL AREA --' },
                    ]
                },
                {
                    text: "Dealer",
                    type: "controls",
                    cls: 'span6',
                    items: [
                        { name: 'Dealer', text: 'Dealer', type: 'select', opt_text: '-- ALL DEALER --' },
                    ]
                },
                {
                    text: "Outlet",
                    type: "controls",
                    cls: 'span6',
                    items: [
                        { name: 'Outlet', text: 'outlet', type: 'select', opt_text: '-- ALL OUTLET --' },
                    ]
                },

                {
                    text: "Revenue",
                    type: "controls",
                    cls: 'span6',
                    items: [
                        { name: 'Revenue', text: 'Revenue', type: 'select', opt_text: '-- ALL REVENUE --' },
                    ]
                },

                {
                    name: "pdi", id: "pdi", text: "PDI", cls: "span4", type: "radiobutton", items: [
                    { id: 'inc', value: 'include', label: 'Include', checked: true },
                    { id: 'exc', value: 'exclude', label: 'Exclude' }
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
    var filter = {
        DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
        //DateFrom: new Date(),
        DateTo: new Date()
    }
    $('#Dealer, #Outlet').attr('disabled', 'disabled');
    widget.setSelect([
          {
              name: "Area",
              //url: "wh.api/combo/GroupAreas",
              url: "wh.api/combo/SrvGroupAreas",
              optionalText: "-- ALL AREA -- "
          },
          //{
          //    name: "Dealer",
          //    //url: "wh.api/combo/CompaniesR2",
          //    url: "wh.api/combo/SrvDealerList",
          //    optionalText: "-- ALL DEALER -- ",
          //    cascade: {
          //        name: "Area"
          //    }
          //},
          //{
          //    name: "Outlet",
          //    //url: "wh.api/combo/Branches",
          //    url: "wh.api/combo/SrvBranchList",
          //    optionalText: "-- ALL OUTLET -- ",
          //    cascade: {
          //        name: "Dealer",
          //        additionalParams: [
          //             { name: "Area", source: "Area", type: "select" }
          //        ]
          //    }
          //},
          {
              name: "Revenue",
              url: "wh.api/combo/FilterRevenue",
              optionalText: "-- ALL REVENUE -- ",

          },
    ]);

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

    $('select').on('change', ResetCombo);

    $("input[name=pdi]").on("change", function () {

        rbDataSource = $(this).val();
        console.log(rbDataSource);
    });

    widget.populate(filter);
    //setTimeout(function () { refreshGrid() }, 800)
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
function refreshGrid() {
    var filter = widget.serializeObject('pnlFilter');
    filter.AreaName = $('[name=Area] option:selected').text();

    widget.kgrid({
        url: "wh.api/inquiry/SvDtlRevenueRegSpkGrid",
        name: "pnlResult",
        params: filter,
        //sortable: false,
        serverBinding: true,
        sort: [{ field: "JobOrderNo", dir: "asc" }, { field: "DealerCode", dir: "asc" }],
        columns: [
            { field: "DealerName", width: 120, title: "Dealer" },
            { field: "BranchName", width: 120, title: "Outlet" },
            { field: "JobOrderNo", width: 140, title: "No Spk" },
            { field: "JobOrderDate", width: 140, title: "Tgl SPK", type: 'date' },
            { field: "BasicModel", width: 120, title: "Model" },
            { field: "PoliceRegNo", width: 120, title: "No Polisi" },
            //{ field: "Odometer", width: 120, title: "Odometer", type: 'number' },
            //{ field: "JobType", width: 140, title: "Kode Pekerjaan" },
            { field: "JobTypeDesc", width: 320, title: "Nama Pekerjaan" },
            { field: "TaskPartNo", width: 160, title: "Kode Jasa / Part" },
            //{ field: "NamaJasaPart", width: 320, title: "Nama Jasa / Part" },
            { field: "DemandQty", width: 110, title: "Demand Qty", type: 'decimal' },
            { field: "SupplyQty", width: 110, title: "Supply Qty", type: 'decimal' },
            { field: "ReturnQty", width: 110, title: "Return Qty", type: 'decimal' },
            { field: "SupplySlipNo", width: 160, title: "Supply Slip No" },
            { field: "SSReturnNo", width: 160, title: "SS Return No" },
            { field: "SaName", width: 250, title: "SA" },
            { field: "FmName", width: 250, title: "Foreman" },
            { field: "ServiceStatusDesc", width: 220, title: "Status" },
        ],
    });
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
    console.log(filter);
    sdms.report({
        url: 'wh.api/inquiry/SvDtlRevenueRegSpkXls',
        type: 'xlsx',
        params: filter
    });
}