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

widget.render(function () {
    var filter = {
        DateFrom: new Date(moment(moment().format('YYYY-MM-') + '01')),
        DateTo: new Date()
    }
    widget.populate(filter);
    $('#Dealer, #Outlet').attr('disabled', 'disabled');
    widget.setSelect([
           {
               name: "Area",
               url: "wh.api/combo/GroupAreas",
               optionalText: "-- SELECT ALL -- ",
           },
           {
               name: "Dealer",
               url: "wh.api/combo/Companies",
               optionalText: "-- SELECT ALL -- ",
               cascade: {
                   name: "Area"
               }
           },
           {
               name: "Outlet",
               url: "wh.api/combo/Branches",
               optionalText: "-- SELECT ALL -- ",
               cascade: {
                   name: "Dealer"
               }
           }
    ]);

    //widget.post("wh.api/combo/UnitIntakeFilter", {}, function (result) {
    //    widget.bind({
    //        name: 'Area',
    //        text: '-- SELECT ALL --',
    //        data: result[0],
    //        defaultAll: true,
    //        onChange: refreshGrid
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
    //        onChange: refreshGrid
    //    });
    //});

    $('select').on('change', refreshGrid);
    $('[name="Area"]').on('change', function () {
        $('[name="Dealer"]').html('<option value="">-- SELECT ALL --</option>');
        $('[name="Outlet"]').html('<option value="">-- SELECT ALL --</option>');
        
    });
    $('[name="Dealer"]').on('change', function () {
        $('[name="Outlet"]').html('<option value="">-- SELECT ALL --</option>');
       
    });
});

function refreshGrid() {
    var filter = widget.serializeObject('pnlFilter');
    widget.kgrid({
        url: "wh.api/inquiry/UnitIntaker2",
        name: "pnlResult",
        params: filter,
        serverBinding: true,
        sort: [{ field: "JobOrderClosed", dir: "desc" }, { field: "CompanyCode", dir: "asc" }],
        columns: [
            { field: "VinNo", width: 200, title: "No Vin" },
            { field: "JobOrderClosed", width: 160, title: "Tanggal Tutup SPK", template: '#= (JobOrderClosed == null) ? "" : moment(JobOrderClosed).format("DD MMM YYYY")  #' },
            { field: "DealerCode", width: 150, title: "Kode Dealer (Purchase)" },
            { field: "DealerName", width: 400, title: "Nama Dealer (Purchase)" },
            { field: "CompanyCode", width: 150, title: "Kode Dealer (Service)" },
            { field: "CompanyName", width: 400, title: "Nama Dealer (Service)" },
            { field: "Area", width: 250, title: "Region Dealer" },
            { field: "Odometer", width: 100, title: "KM", type: 'number' },
            { field: "SalesModelDesc", width: 200, title: "Tipe Kendaraan" },
            { field: "SalesModelCode", width: 150, title: "Sales Model Code" },
            { field: "BasicModel", width: 150, title: "Basic Model" },
            { field: "ProductionYear", width: 150, title: "Tahun Produksi" },
            { field: "DoDate", width: 150, title: "Tanggal Pembelian", type: 'date' },
            { field: "PoliceRegNo", width: 150, title: "No. Polisi" },
            { field: "EngineNo", width: 150, title: "No. Mesin" },
            { field: "ChassisNo", width: 150, title: "No. Rangka" },
            { field: "CustomerName", width: 580, title: "Nama Pelanggan" },
            { field: "PhoneNo", width: 150, title: "No. Telp. Rumah" },
            { field: "OfficePhoneNo", width: 150, title: "No. Telp. Kantor" },
            { field: "HPNo", width: 150, title: "No. HP" },
            { field: "ContactName", width: 480, title: "Additional Contact" },
            { field: "Email", width: 150, title: "Email" },
            { field: "BirthDate", width: 150, title: "Tanggal Lahir", type: 'date' },
            { field: "Gender", width: 150, title: "Jenis Kelamin" },
            { field: "Address", width: 1000, title: "Alamat" },
            { field: "GroupJobTypeDesc", width: 250, title: "Jenis Service" },
            { field: "JobType", width: 250, title: "Jenis Pekerjaan" },
            { field: "JobTypeDesc", width: 500, title: "Keterangan" },
            { field: "SaName", width: 250, title: "Nama SA" },
            { field: "SaNik", width: 150, title: "NIK SA" },
        ],
    });
    
    if ($('#Area').val() == "") {
        $('#Dealer').val('');
        $('[name="Outlet"]').html('<option value="">-- SELECT ALL --</option>');
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
        url: 'wh.api/inquiry/UnitIntakeXlsr2',
        type: 'xlsx',
        params: params
    });
}