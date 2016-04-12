var widget = new SimDms.Widget({
    title: 'Service Unit Intake',
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
                    text: "Area / Dealer",
                    type: "controls",
                    cls: 'span8',
                    items: [
                        { name: 'Area', text: 'Area', type: 'select', cls: 'span2', opt_text: '-- ALL AREA --' },
                        { name: 'Dealer', text: 'Dealer', type: 'select', cls: 'span4', opt_text: '-- ALL DEALER --' },
                       
                    ]
                },
                {
                    text: "Outlet",
                    type: "controls",
                    cls: 'span8',
                    items: [
                        { name: 'Outlet', text: 'Outlet', type: 'select', cls: 'span6', opt_text: '-- ALL OUTLET --' },

                    ]
                },
                {
                    text: "Filter By",
                    type: "controls",
                    cls: 'span8',
                    items: [
                        { name: 'NOVIN', text: 'No. VIN',  cls: 'span2' },
                        { name: 'NOPOL', text: 'Nomor Polisi',  cls: 'span2'},
                        { name: 'PELANGGAN', text: 'Nama Pelanggan', cls: 'span2' },
                        { name: "Rework", text: "Rework", type: "check", cls: "span2" },
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
    //$("[name=Dealer]").on("change", function () {
    //    widget.select({ selector: "[name=Outlet]", url: "wh.api/combo/SvBranchList", params: { area: $("#pnlFilter [name=Area]").val(), comp: $("#pnlFilter [name=Dealer]").val() }, optionalText: "-- SELECT ALL --" });
    //    $("[name=Outlet]").prop("selectedIndex", 0);
    //    $("[name=Outlet]").change();
    //});

    //widget.setSelect([
    //       {
    //           name: "Area",
    //           url: "wh.api/combo/GroupAreas",
    //           optionalText: "-- SELECT ALL -- ",
    //       },
    //       {
    //           name: "Dealer",
    //           url: "wh.api/combo/Companies",
    //           optionalText: "-- SELECT ALL -- ",
    //           cascade: {
    //               name: "Area"
    //           }
    //       },
    //       {
    //           name: "Outlet",
    //           url: "wh.api/combo/Branches",
    //           optionalText: "-- SELECT ALL -- ",
    //           cascade: {
    //               name: "Dealer"
    //           }
    //       }
    //]);

    //widget.post("wh.api/combo/UnitIntakeFilter", {}, function (result) {
    //    widget.bind({
    //        name: 'Area',
    //        text: '-- SELECT ALL --',
    //        data: result[0],
    //        defaultAll: true,
    //        //onChange: refreshGrid
    //    });
    //    widget.bind({
    //        name: 'Dealer',
    //        text: '-- SELECT ALL --',
    //        data: result[1],
    //        parent: 'Area',
    //        defaultAll: true,
    //        //onChange: refreshGrid
    //    });
    //    widget.bind({
    //        name: 'Outlet',
    //        text: '-- SELECT ALL --',
    //        data: result[2],
    //        defaultAll: true,
    //        parent: 'Dealer' ,
    //       // onChange: refreshGrid
    //    });
    //});

    $('select').on('change', ResetCombo);

    $('[name="Area"]').on('change', function () {
        //$('[name="Dealer"]').html('<option value="">-- SELECT ALL --</option>');
        $('[name="Outlet"]').html('<option value="">-- SELECT ALL --</option>');
        
    });
    //$('[name="Dealer"]').on('change', function () {
    //    $('[name="Outlet"]').html('<option value="">-- SELECT ALL --</option>');
       
    //});
});

function ResetCombo()
{
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

    //var data = widget.serializeObject('pnlFilter');    

    //if ($("[name=Rework]").prop('checked')) {
    //    test = "Rework";
    //}
    //console.log(data);
   
    widget.kgrid({
        url: "wh.api/inquiry/UnitIntake",
        name: "pnlResult",
        params: filter,
        serverBinding: true,
        sort: [{ field: "JobOrderClosed", dir: "desc" }, { field: "CompanyCode", dir: "asc" }],
        columns: [
            { field: "VinNo", width: 200, title: "No Vin" },
            { field: "JobOrderClosed", width: 160, title: "Tanggal Tutup SPK", template: '#= (JobOrderClosed == null) ? "" : moment(JobOrderClosed).format("DD MMM YYYY")  #' },
            { field: "OutletCode", width: 150, title: "Kode Outlet" },
            { field: "OutletName", width: 400, title: "Nama Outlet" },
            { field: "GroupJobTypeDesc", width: 250, title: "Jenis Service" },
            { field: "Odometer", width: 100, title: "KM", type: 'number' },
            { field: "SalesModelDesc", width: 200, title: "Type" },
            { field: "ProductionYear", width: 150, title: "Tahun Produksi" },
            { field: "PoliceRegNo", width: 150, title: "No. Polisi" },
            { field: "EngineNo", width: 150, title: "No. Mesin" },
            { field: "ChassisNo", width: 150, title: "No. Rangka" },
            { field: "CustomerName", width: 580, title: "Nama Pelanggan" },
            { field: "PhoneNo", width: 150, title: "No. Telp. Rumah" },
            { field: "OfficePhoneNo", width: 150, title: "No. Telp. Kantor" },
            { field: "HPNo", width: 150, title: "No. HP" },
            { field: "Email", width: 150, title: "Email" },
            { field: "BirthDate", width: 150, title: "Tanggal Lahir", type: 'date' },
            { field: "Gender", width: 150, title: "Jenis Kelamin" },
            { field: "Address", width: 1000, title: "Alamat" },
            { field: "JobTypeDesc", width: 500, title: "Keterangan" },
            { field: "Area", width: 250, title: "Area(Service)" },
            { field: "SaNik", width: 150, title: "NIS SA" },
            { field: "SaName", width: 250, title: "Nama SA" },
            { field: "CompanyCode", width: 150, title: "Kode Dealer (Service)" },
            { field: "CompanyName", width: 400, title: "Nama Dealer (Service)" },
            { field: "DealerCode", width: 150, title: "Kode Dealer (Purchase)" },
            { field: "DealerName", width: 400, title: "Nama Dealer (Purchase)" },
            { field: "SalesModelCode", width: 150, title: "Sales Model Code" },
            { field: "BasicModel", width: 150, title: "Basic Model" },
            { field: "DoDate", width: 150, title: "Tanggal Pembelian", type: 'date' }, 
            { field: "ContactName", width: 480, title: "Additional Contact" },
            { field: "JobType", width: 250, title: "Jenis Pekerjaan" },
            { field: "SaNik", width: 150, title: "NIK SA" },
        ],
    });   
    
}

function exportToExcel() {
    //var url = "wh.api/InquiryProd/GenerateUnitIntake?";
    //var filter = widget.serializeObject('pnlFilter');
    //var params = '';


    //$.each(filter || [], function (key, val) {
    //    params += key + '=' + val + '&';
    //});
    //params = params.substring(0, params.length - 1);

    //url += params;
    //window.location = url;

    var params = widget.serializeObject('pnlFilter');
    params.AreaName = $('[name=Area] option:selected').text();
    console.log(params);
    sdms.report({
        url: 'wh.api/inquiry/UnitIntakeXls',
        type: 'xlsx',
        params: params
    });
}