var widget = new SimDms.Widget({
    title: 'Questionnaire',
    xtype: 'panels',
    toolbars: [
        { text: 'Browse', action: 'browse', icon: 'fa fa-search' },
        { text: 'Save', action: 'save', icon: 'fa fa-save', name: "btnSave" },
        { text: 'Cancel', action: 'cancel', icon: 'fa fa-refresh' },
        { text: 'Delete', action: 'delete', icon: 'fa fa-trash-o' },
        //{ text: 'Clear', action: 'clear', icon: 'fa fa-gear' },
        //{ text: 'Export to Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o', cls: '' },
        { text: 'Expand', action: 'expand', icon: 'fa fa-expand' },
        { text: 'Collapse', action: 'collapse', icon: 'fa fa-compress', cls: 'hide' },
    ],
    panels: [
        {
            name: "pnlProfile",
            items: [
                { name: "Area", text: "Area", type: "text", cls: "span4", lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'", disabled: true },
                { type: "span" },
                {
                    text: "Dealer", type: "controls", cls: 'span8', lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:94%; padding-bottom:5px;'", items: [
                        { name: "CompanyCode", text: "Dealer Code", type: "text", cls: "span4", disabled: true },
                        { name: "CompanyName", text: "Nama Dealer", type: "text", cls: "span4", disabled: true },
                    ]
                },
                {
                    text: "Outlet", type: "controls", cls: 'span8', lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:94%; padding-bottom:5px;'", items: [
                        { name: "BranchCode", text: "Outlet Code", type: "text", cls: "span4", disabled: true },
                        { name: "BranchName", text: "Nama Outlet", type: "text", cls: "span4", disabled: true },
                    ]
                },

                //{ name: "BranchCode", text: "Branch Code", type: "text", cls: "span4", lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'", disabled: true },
                //{ type: "span" },
                { name: "ChassisCode", text: "Chassis Code", type: "popup", cls: "span4", lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ChassisNo", text: "Chassis No", cls: "span4", lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "StatusKonsumen", text: "Status Konsumen", type: 'select', required: true, cls: "span4", lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                {
                    text: "SalesModel", type: "controls", cls: 'span8', lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:94%; padding-bottom:5px;'", items: [
                        { name: "SalesModelCode", text: "SalesModel Code", type: "text", cls: "span4", disabled: true },
                        { name: "SalesModelReport", text: "SalesModel Report", type: "text", cls: "span4", disabled: true },
                    ]
                },
                //{ name: "SalesModelCode", text: "Sales Model Code", cls: "span4", lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'", disabled: true },
                //{ type: "span" },
                { name: "NamaKonsumen", text: "Nama Konsumen", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "UmurKonsumen", text: "Umur Konsumen", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "JenisKelamin", text: "Jenis Kelamin", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ContactNo", text: "No. Telpon / HP", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "CashOrCredit", text: "Pembelian Cash/Credit", type: "select", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { name: "CreditInstalment", text: "Jikalau Kredit Cicilan Berapa lama", required: true, placeholder: "X Bulan", cls: "span4", lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicle", text: "1. Status Konsumen", cls: 'span4', type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleReplacedMerk", text: "2. Mengganti dari merek", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleReplacedType", text: "Tipe", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleOtherReplacedMerk", text: "Merk", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleOtherReplacedType", text: "Tipe", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleReplacedReason", text: "Jika mengganti dari yang lama, alasannya", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleReplacedReasonOther", text: "Alasan lainnya, sebutkan", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AddLabel", type: "text", text: "3. Jika menambah mobil Pick-up yang ada, berapa unit jumlah mobil Pick-up yang sudah ada", cls: "span4", lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px; display:none;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleAdditionalSuzuki", placeholder: "Unit", text: "Suzuki", cls: "span4", lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleAdditionalDaihatsu", placeholder: "Unit", text: "Daihatsu", cls: "span4", lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleAdditionalMitsubishi", placeholder: "Unit", text: "Mitsubishi", cls: "span4", lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleAdditionalOthers", placeholder: "Unit", text: "Others", cls: "span4", lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleNew", text: "6. Jika baru beli sebelumnya memakai apa", cls: 'span4', type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "AsVehicleNewOther", text: "Lainnya", cls: 'span4', type: "text", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "LoadOneTrip", text: "4. Dari mobil Pick-up yang anda punyai, 1 unit berapa KG sekali angkut", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "LongInKMAnnualTrip", text: "5. Berapa KM per tahun anda mengendarai pick-up anda", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "OccupationPart", text: "7. Bidang usaha anda", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "OccupationDetail", text: "Jenis barang yang diangkut", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "OccupationPartOther", text: "Lainnya, sebutkan bidang usaha anda", cls: "span4", type: "text", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "OccupationDetailOther", text: "Lainnya, sebutkan jenis barang yang diangkut", cls: "span4", type: "text", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "PassengerCar", text: "8. Apakah anda mempunyai kendaraan penumpang", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "PassengerCarYes", text: "Jika ya, berapa unit", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                {
                    text: "Merk apa", type: "controls", cls: 'span6', lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'", items: [
                        { name: "MerkPsgrCar1", text: "Toyota", type: "check", cls: "span3" },
                        { name: "TipePsgrCar1", text: "Tipe", cls: "span3", required: true, disabled: true },
                        { name: "MerkPsgrCar2", text: "Honda", type: "check", cls: "span3" },
                        { name: "TipePsgrCar2", text: "Tipe", cls: "span3", required: true, disabled: true },
                        { name: "MerkPsgrCar3", text: "Daihatsu", type: "check", cls: "span3" },
                        { name: "TipePsgrCar3", text: "Tipe", cls: "span3", required: true, disabled: true },
                        { name: "MerkPsgrCar4", text: "Suzuki", type: "check", cls: "span3" },
                        { name: "TipePsgrCar4", text: "Tipe", cls: "span3", required: true, disabled: true },
                        { name: "MerkPsgrCar5", text: "Others", type: "check", cls: "span3" },
                        { name: "MerkOthersPsgrCar5", text: "Merk", cls: "span3", required: true, disabled: true },
                        { name: "Mpc5Hdn", text: "Others", type: "check", cls: "span3", spanstyle: "visibility:hidden;", style: "visibility:hidden;" },
                        { name: "TipePsgrCar5", text: "Tipe", cls: "span3", required: true, disabled: true },
                    ]
                },
                //{
                //    text: "Merk apa", type: "controls", cls: 'span6', lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'", items: [
                //        { name: "MerkPsgrCar1", text: "Honda", type: "select", cls: "span3" },
                //        { name: "MerkOtherPsgrCar1", text: "Merk", cls: "span3" },
                //        { name: "TipePsgrCar1", text: "Tipe", cls: "span3" },
                //        {
                //            type: "buttons",
                //            cls: "span4",
                //            items: [
                //                { name: "btnAdd", text: " Add", icon: "fa fa-save", btntype: " type=\"button\" " },
                //                { name: "btnEditSv", text: " Save", icon: "fa fa-save", btntype: " type=\"button\" " },
                //                { name: "btnEditCn", text: " Cancel", icon: "fa fa-cancel", btntype: " type=\"button\" ", additional: " style=\"margin-left:3px;\" " },
                //            ]
                //        },
                //    ]
                //},
                //{
                //    name: "PassengerCars",
                //    type: "table",
                //    width: "300px",
                //    cls: "span4",
                //    dataId: "pcbody",
                //    hdnname: "hdnEditID",
                //    columns: [
                //            { title: "Merk", width: "150px" },
                //            { title: "Tipe", width: "150px" },
                //            { title: "Action", width: "150px" },
                //    ],
                //},
                { type: "span" },
                { name: "MotorCycleExists", text: "Apakah mempunyai Sepeda Motor", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },

            ]
        },
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'save':
                saveQa();
                break;
            case 'cancel':
                location.reload();
                break;
            case 'delete':
                if (confirm('Are you sure want to remove this questionnaire?')) {
                    removeQa();
                }
                break;
            case 'browse':
                popupBrowse();
                break;
            case 'collapse':
                widget.exitFullWindow();
                widget.showToolbars(['browse', 'save', 'expand']);
                break;
            case 'expand':
                widget.requestFullWindow();
                widget.showToolbars(['browse', 'save', 'collapse']);
                break;
                //case 'exportToExcel':
                //    sdms.popup({
                //        title: 'Questionnaire to Production',
                //        rows: [
                //            {
                //                name: "pnlExport",
                //                fields: [
                //                       {
                //                           name: 'BranchCode', text: 'Outlet Name', type: 'select', cols: '4',
                //                           source: {
                //                               url: 'wh.api/Combo/QaProdBranchs',
                //                               //cascade: { source: 'CompanyCode', name: 'comp' },
                //                               text: '-- SELECT All --'
                //                           },
                //                           action: function (obj) {
                //                               if ($(obj).val() != '-- SELECT All --') {
                //                                   $('input[name="OutletOption"]').prop('disabled', true);
                //                                   $('input[name="OutletOption"]').first().prop('checked', true);
                //                               }
                //                               else {
                //                                   $('input[name="OutletOption"]').prop('disabled', false);
                //                                   $('input[name="OutletOption"]').first().prop('checked', true);
                //                               }
                //                           }
                //                       },
                //                    { name: 'OutletOption', text: 'Summary all outlet', cols: '4', type: "radio", value: "1", id: "summaryOutlet", checked: true },
                //                    { name: 'OutletOption', text: 'Detail per outlet', cols: '4', type: "radio", value: "2", id: "detailOutlet" },
                //                    { name: 'StartDate', text: 'From Date', cols: '4', type: "datepicker", placeholder: 'DD-MMM-YYYY' },
                //                    { name: 'EndDate', text: 'To Date', cols: '4', type: "datepicker", placeholder: 'DD-MMM-YYYY' },
                //                ],
                //                okbutton: { name: 'Export (.xls)' },
                //            },
                //        ],
                //        onclick: function () {
                //            exportToExcel();
                //        },
                //        ownStyle: {},
                //    });

                //    $(".datepicker").removeClass('hasDatepicker').removeAttr('id').datepicker({
                //        dateFormat: "dd-mm-yy",
                //        showOtherMonths: true,
                //        selectOtherMonths: true,
                //        changeMonth: true,
                //        changeYear: true,
                //    });
                //    break;
            default:
                break;
        }
    },
});

function saveQa() {
    var valid = $(".main form").valid();
    //if ($('#pcbody').children('tr').length <= 0) {
    //    $('#PassengerCars').children('table').prev().empty().after('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">This field is required.</span>');
    //    valid = false;
    //}
    //else {
    //    $('#PassengerCars').children('table').prev().empty();
    //}

    //if ($('#pcbody').children('tr').length > maxpc && maxpc != -1) {
    //    $('#PassengerCars').children('table').prev().empty().after('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Must be same or less than amount units.</span>');
    //    valid = false;
    //}
    //else {
    //    $('#PassengerCars').children('table').prev().empty();
    //}

    if ($errorFpol != null) {
        $errorFpol.remove();
        $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">' + msgErr + '</span>').insertBefore('#ChassisCode');
        valid = false;
    }
    else {
        if ($errorFpol)
            $errorFpol.remove();
        $errorFpol = null;
    }

    if ($('#UmurKonsumen').val() < 17) {
        if ($errorFpol)
            $errorFpol.remove();

        $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">Umur Konsumen tidak boleh lebih kecil dari 17 atau minus.</span>').insertBefore('#UmurKonsumen');
        valid = false;
    }
    else {
        if ($errorFpol)
            $errorFpol.remove();
        $errorFpol = null;
    }

    var unitsuzuki = $('#AsVehicleAdditionalSuzuki').val() == '' ? 0 : $('#AsVehicleAdditionalSuzuki').val();
    var unitdaihatsu = $('#AsVehicleAdditionalDaihatsu').val() == '' ? 0 : $('#AsVehicleAdditionalDaihatsu').val();
    var unitmitsubishi = $('#AsVehicleAdditionalMitsubishi').val() == '' ? 0 : $('#AsVehicleAdditionalMitsubishi').val();
    var unitothers = $('#AsVehicleAdditionalOthers').val() == '' ? 0 : $('#AsVehicleAdditionalOthers').val();

    if ($('#AsVehicle').val() == 'C' && (unitsuzuki + unitdaihatsu + unitmitsubishi + unitothers) <= 0) {
        if ($errorUnits) $errorUnits.remove();
        $errorUnits = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">Must be filled at least one.</span>').insertBefore('#AsVehicleAdditionalSuzuki');
        valid = false;
    }
    else {
        if ($errorUnits) $errorUnits.remove();
    }

    if ($('#PassengerCar').val() == 'A' && $('#PassengerCarYes').val() != '') {
        if ($('input[name^="MerkPsgrCar"]:checked').length > pcunitcount && pcunitcount != -1) {
            if ($errorUnits) $errorUnits.remove();
            $errorUnits = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">Unit tidak boleh kurang dari jumlah merk yang dicentang.</span>').insertBefore('#PassengerCarYes');
            valid = false;
        } else if ($('input[name^="MerkPsgrCar"]:checked').length <= 0) {
            if ($errorUnits) $errorUnits.remove();
            $errorUnits = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">Data unit harus diisi setidaknya satu.</span>').insertBefore('#PassengerCarYes');
            valid = false;
        }
        else {
            if ($errorUnits) $errorUnits.remove();
        }
    }

    if (valid) {
        var filter = widget.serializeObject('pnlProfile');

        //var passengerCars = new Array();

        //if ($('#pcbody').children('tr').length > 0) {
        //    $('#pcbody').children('tr').each(function () {
        //        var merk = $(this).children('td:first-child').text();
        //        var tipe = $(this).children('td:first-child').next('td').text();
        //        var arr = { merk: merk, tipe: tipe };
        //        passengerCars.push(arr);
        //    });
        //}

        //filter.passengerCars = JSON.stringify(passengerCars);

        widget.post("wh.api/Questionnaire/QaTransaction", filter, function (result) {
            widget.showNotification(result.message || SimDms.defaultInformationMessage, function () {
                if (result.isValid) {
                    location.reload();
                }
                else {
                    $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">' + msgErr + '</span>').insertBefore('#ChassisCode');
                }
            });
        });
    }
}

function popupBrowse() {
    sdms.lookup({
        title: 'Daftar Questionnaire Outlet Anda',
        url: 'wh.api/lookupgrid/LookUpQuestionnaire',
        sort: [{ field: 'ChassisCode', dir: 'asc' }, { field: 'ChassisNo', dir: 'asc' }],
        fields: [
            { name: 'ChassisCode', text: 'Chassis Code', width: 200 },
            { name: 'ChassisNo', text: 'Chassis No', width: 200 },
        ],
        dblclick: function (row) {
            selectBrowse(row);
        },
        onclick: function (row) {
            selectBrowse(row);
        }
    });
    $('.wg-modal .modal').css({ 'width': '100%', 'margin-left': '0' });
}

function selectBrowse(row) {
    if ($errorFpol) $errorFpol.remove();
    if ($errorUnits) $errorUnits.remove();

    widget.post("wh.api/Questionnaire/populateEditQuestionnaire", { "ChassisCode": row.ChassisCode, "ChassisNo": row.ChassisNo }, function (result) {
        var list = [];
        list = Enumerable.From(result[0]).ToArray();
        //widget.setValue({ name: "FakPol", value: (list.length <= 0 ? '' : list[0].FPol), isControl: false });
        //$('#FakPol').prop("disabled", true);
        widget.setValue({ name: "ChassisCode", value: (list.length <= 0 ? '' : list[0].ChassisCode), isControl: false });
        $('#ChassisCode').prop("disabled", true);
        widget.setValue({ name: "ChassisNo", value: (list.length <= 0 ? '' : list[0].ChassisNo), isControl: false });
        $('#ChassisNo').prop("disabled", true);

        //$('#StatusKonsumen').val(list.length <= 0 ? '' : list[0].StatusKonsumenCode).change();
        $('#StatusKonsumen').select2("val", (list.length <= 0 ? '' : list[0].StatusKonsumenCode)).change();

        widget.setValue({ name: "SalesModelCode", value: (list.length <= 0 ? '' : list[0].SalesModelCode), isControl: false });

        widget.setValue({ name: "SalesModelReport", value: (list.length <= 0 ? '' : list[0].SalesModelReport), isControl: false });

        widget.setValue({ name: "NamaKonsumen", value: (list.length <= 0 ? '' : list[0].RespondenName), isControl: false });
        widget.setValue({ name: "UmurKonsumen", value: (list.length <= 0 ? '' : list[0].RespondenAge), isControl: false });
        //$('#JenisKelamin').val(list.length <= 0 ? '' : list[0].RespondenGender).change();;
        $('#JenisKelamin').select2("val", (list.length <= 0 ? '' : list[0].RespondenGender)).change();;
        widget.setValue({ name: "ContactNo", value: (list.length <= 0 ? '' : list[0].RespondenPhone), isControl: false });

        //$('#CashOrCredit').val(list.length <= 0 ? '' : (list[0].IsCredit == true ? '1' : '0')).change();

        $('#CashOrCredit').select2("val", (list.length <= 0 ? '' : (list[0].IsCredit == true ? '1' : '0'))).change();
        //$('#CashOrCredit').change();

        widget.setValue({ name: "CreditInstalment", value: (list.length <= 0 ? '' : (list[0].Installment == '0' ? '' : list[0].Installment)), isControl: false });

        initMerkVal = list.length <= 0 ? '' : list[0].IsReplacementMerkCode;

        var asCode = list.length <= 0 ? '' : list[0].RespondenStatusCode;
        //$('#AsVehicle').val(asCode).change();

        $('#AsVehicle').select2("val", asCode).change();

        //$('#AsVehicleReplacedMerk').val((list.length <= 0 ? '' : list[0].IsReplacementMerkCode)).change();

        if (asCode == 'C') {//additional
            $('#AsVehicleAdditionalSuzuki').val(list.length <= 0 ? '' : list[0].IsAdditionalSuzuki);
            $('#AsVehicleAdditionalDaihatsu').val(list.length <= 0 ? '' : list[0].IsAdditionalDaihatsu);
            $('#AsVehicleAdditionalMitsubishi').val(list.length <= 0 ? '' : list[0].IsAdditionalMitsubishi);
            $('#AsVehicleAdditionalOthers').val(list.length <= 0 ? '' : list[0].IsAdditionalOthers);
        }
        else if (asCode == 'A' || asCode == 'B') {
            initTypeVal = list.length <= 0 ? '' : list[0].IsReplacementTypeCode;
        }
        else if (asCode == 'D') {
            //$('#AsVehicleNew').val(list.length <= 0 ? '' : list[0].FirstTimeCode);
            $('#AsVehicleNew').select2("val", list.length <= 0 ? '' : list[0].FirstTimeCode).change();
        }

        if ((list.length <= 0 ? '' : list[0].IsReplacementMerkCode) == 'D') { //others
            //$('#AsVehicleOtherReplacedMerk').val((list.length <= 0 ? '' : list[0].IsReplacementMerkOthers));
            //$('#AsVehicleOtherReplacedType').val((list.length <= 0 ? '' : list[0].IsReplacementTypeOthers));

            $('#AsVehicleOtherReplacedMerk').select2("val", (list.length <= 0 ? '' : list[0].IsReplacementMerkOthers)).change();
            $('#AsVehicleOtherReplacedType').select2("val", (list.length <= 0 ? '' : list[0].IsReplacementTypeOthers)).change();
        }

        //$('#AsVehicleReplacedReason').val((list.length <= 0 ? '' : list[0].IsReplacementReasonCode)).change();
        $('#AsVehicleReplacedReason').select2("val", (list.length <= 0 ? '' : list[0].IsReplacementReasonCode)).change();

        if (list.length <= 0 ? '' : list[0].IsReplacementReasonCode == 'F') //others
            $('#AsVehicleReplacedReasonOther').val((list.length <= 0 ? '' : list[0].IsReplacementReasonOthers));

        //$('#LoadOneTrip').val((list.length <= 0 ? '' : list[0].LoadCapacityCode));
        $('#LoadOneTrip').select2("val", (list.length <= 0 ? '' : list[0].LoadCapacityCode)).change();
        //$('#LongInKMAnnualTrip').val((list.length <= 0 ? '' : list[0].AnnualDriveCode));
        $('#LongInKMAnnualTrip').select2("val", (list.length <= 0 ? '' : list[0].AnnualDriveCode)).change();

        var occpCode = list.length <= 0 ? '' : list[0].OccupationCode;
        if (occpCode == 'K') {
            $('#OccupationPartOther').val((list.length <= 0 ? '' : list[0].OccupationOthers));
            $('#OccupationDetailOther').val((list.length <= 0 ? '' : list[0].OccupationDetailOthers));
        }
        else {
            initOccDetailVal = list.length <= 0 ? '' : list[0].OccupationDetailCode;
            //alert(initOccDetailVal);
        }
        //$('#OccupationPart').val(occpCode).change();
        $('#OccupationPart').select2("val", occpCode).change();

        var pcars = list.length <= 0 ? '' : list[0].PassengerCarCode;

        //$('#PassengerCar').val(pcars).change();
        $('#PassengerCar').select2("val", pcars).change();

        if (pcars == 'A') {
            //$('#PassengerCarYes').val((list.length <= 0 ? '' : list[0].PassengerCarUnitCode)).change();
            $('#PassengerCarYes').select2("val", (list.length <= 0 ? '' : list[0].PassengerCarUnitCode)).change();

            if (list.length <= 0 ? '' : list[0].IsPassengerCarToyota) {
                $('#MerkPsgrCar1').prop('checked', true).change();
                $('#TipePsgrCar1').val(list.length <= 0 ? '' : list[0].PassengerCarToyotaType);
            }

            if (list.length <= 0 ? '' : list[0].IsPassengerCarHonda) {
                $('#MerkPsgrCar2').prop('checked', true).change();
                $('#TipePsgrCar2').val(list.length <= 0 ? '' : list[0].PassengerCarHondaType);
            }

            if (list.length <= 0 ? '' : list[0].IsPassengerCarDaihatsu) {
                $('#MerkPsgrCar3').prop('checked', true).change();
                $('#TipePsgrCar3').val(list.length <= 0 ? '' : list[0].PassengerCarDaihatsuType);
            }

            if (list.length <= 0 ? '' : list[0].IsPassengerCarSuzuki) {
                $('#MerkPsgrCar4').prop('checked', true).change();
                $('#TipePsgrCar4').val(list.length <= 0 ? '' : list[0].PassengerCarSuzukiType);
            }

            if (list.length <= 0 ? '' : list[0].IsPassengerCarOthers) {
                $('#MerkPsgrCar5').prop('checked', true).change();
                $('#MerkOthersPsgrCar5').val(list.length <= 0 ? '' : list[0].PassengerCarOthersMerk);
                $('#TipePsgrCar5').val(list.length <= 0 ? '' : list[0].PassengerCarOthersType);
            }
        }

        //$('#MotorCycleExists').val(list.length <= 0 ? '' : list[0].MotorCycleCode);
        $('#MotorCycleExists').select2("val", (list.length <= 0 ? '' : list[0].MotorCycleCode)).change();

        if ((list.length <= 0 ? '0' : list[0].isEligible) == '0') {
            $errorfpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">data tidak dapat diubah karena sudah diinput lewat dari sehari.</span>').insertBefore('#ChassisCode');
            widget.enable({ value: false, items: ["btnSave"] });
            $('#pnlProfile *').prop('disabled', true);
            widget.showToolbars(['browse', 'save', 'cancel', 'expand']);
        }
        else {
            widget.showToolbars(['browse', 'save', 'delete', 'cancel', 'expand']);
            widget.enable({ value: true, items: ["btnSave"] });
            $('#pnlProfile *').prop('disabled', false);
            widget.enable({ value: false, items: ["Area", "ChassisCode", "ChassisNo", "SalesModelCode", "SalesModelReport", "CompanyCode", "CompanyName", "BranchCode", "BranchName"] });

            $('#CashOrCredit').change();
            $('#MerkPsgrCar1').change();
            $('#MerkPsgrCar2').change();
            $('#MerkPsgrCar3').change();
            $('#MerkPsgrCar4').change();
            $('#MerkPsgrCar5').change();
        }

        widget.enable({ value: false, items: ["btnChassisCode"] });
    });
}

function removeQa() {
    widget.post("wh.api/Questionnaire/removeQuestionnaire", { "ChassisCode": $('#ChassisCode').val(), "ChassisNo": $('#ChassisNo').val() }, function (result) {
        widget.showNotification(result.message || SimDms.defaultInformationMessage, function () {
            location.reload();
        });
    });
}

function InitControls() {
    //replace pick-up
    $('#AsVehicleReplacedMerk').parents('div.span4').hide();
    $('#AsVehicleReplacedMerk').parents('div.span4').next('div').hide();

    $('#AsVehicleReplacedType').parents('div.span4').hide();
    $('#AsVehicleReplacedType').parents('div.span4').next('div').hide();

    $('#AsVehicleOtherReplacedMerk').parents('div.span4').hide();
    $('#AsVehicleOtherReplacedMerk').parents('div.span4').next('div').hide();
    $('#AsVehicleOtherReplacedType').parents('div.span4').hide();
    $('#AsVehicleOtherReplacedType').parents('div.span4').next('div').hide();

    $('#AsVehicleReplacedReason').parents('div.span4').hide();
    $('#AsVehicleReplacedReason').parents('div.span4').next('div').hide();
    $('#AsVehicleReplacedReasonOther').parents('div.span4').hide();
    $('#AsVehicleReplacedReasonOther').parents('div.span4').next('div').hide();
    //end of replace pick-up

    //additional pick-up
    $('#AddLabel').parents('div.span4').hide();
    $('#AddLabel').parents('div.span4').next('div').hide();

    $('#AsVehicleAdditionalSuzuki').parents('div.span4').hide();
    $('#AsVehicleAdditionalSuzuki').parents('div.span4').next('div').hide();
    $('#AsVehicleAdditionalDaihatsu').parents('div.span4').hide();
    $('#AsVehicleAdditionalDaihatsu').parents('div.span4').next('div').hide();
    $('#AsVehicleAdditionalMitsubishi').parents('div.span4').hide();
    $('#AsVehicleAdditionalMitsubishi').parents('div.span4').next('div').hide();
    $('#AsVehicleAdditionalOthers').parents('div.span4').hide();
    $('#AsVehicleAdditionalOthers').parents('div.span4').next('div').hide();
    //end of additional pick-up

    //new vehicle
    $('#AsVehicleNew').parents('div.span4').hide();
    $('#AsVehicleNew').parents('div.span4').next('div').hide();

    $('#AsVehicleNewOther').parents('div.span4').hide();
    $('#AsVehicleNewOther').parents('div.span4').next('div').hide();
    //end of new vehicle

    //passenger car exists
    $('#PassengerCarYes').parents('div.span4').hide();
    $('#PassengerCarYes').parents('div.span4').next('div').hide();

    $('#MerkPsgrCar1').parents('div.span6').hide();
    $('#MerkPsgrCar5').parents('div.span6').hide();
    //$('#MerkOtherPsgrCar1').parents('div.span3').hide();
    $('#PassengerCars').parents('div.span4').hide();
    //end of passenger car exists

    //Occupation
    $('#OccupationDetail').parents('div.span4').hide();
    $('#OccupationDetail').parents('div.span4').next('div').hide();

    $('#OccupationPartOther').parents('div.span4').hide();
    $('#OccupationPartOther').parents('div.span4').next('div').hide();

    $('#OccupationDetailOther').parents('div.span4').hide();
    $('#OccupationDetailOther').parents('div.span4').next('div').hide();
    //end of Occupation

    //btn save editing
    //$('#btnEditSv').hide();
    //$('#btnEditCn').hide();

    //disabled textboxes
    //widget.enable({ value: false, items: ["TipePsgrCar1"] });
    //widget.enable({ value: false, items: ["TipePsgrCar2"] });
    //widget.enable({ value: false, items: ["TipePsgrCar3"] });
    //widget.enable({ value: false, items: ["TipePsgrCar4"] });
    //widget.enable({ value: false, items: ["TipePsgrCar5"] });
    //widget.enable({ value: false, items: ["MerkOthersPsgrCar5"] });

    widget.enable({ value: false, items: ["btnSave"] });

    widget.showToolbars(['browse', 'save', 'expand']);

    $errorFpol = null;
    initTypeVal = null;
    initMerkVal = null;
    initOccDetailVal = null;
}

function slideDownControls(type, subtype) {
    if (type == 0) {//replace
        if (subtype == 0) {
            $('#AsVehicleReplacedMerk').parents('div.span4').slideDown();
            $('#AsVehicleReplacedMerk').parents('div.span4').next('div').show();

            $('#AsVehicleReplacedReason').parents('div.span4').slideDown();
            $('#AsVehicleReplacedReason').parents('div.span4').next('div').show();

            slideUpControls('1', '0'); slideUpControls('0', '4');
            resetChildControls('0', '1');
            resetChildControls('0', '6'); //new
        }
        else if (subtype == 1) {
            $('#AsVehicleReplacedType').parents('div.span4').slideDown();
            $('#AsVehicleReplacedType').parents('div.span4').next('div').show();

            slideUpControls('0', '2');
            resetChildControls('0', '4');
        } else if (subtype == 2) {
            $('#AsVehicleOtherReplacedMerk').parents('div.span4').slideDown();
            $('#AsVehicleOtherReplacedMerk').parents('div.span4').next('div').show();
            $('#AsVehicleOtherReplacedType').parents('div.span4').slideDown();
            $('#AsVehicleOtherReplacedType').parents('div.span4').next('div').show();

            slideUpControls('0', '1');
            resetChildControls('0', '3');
        }
        else if (subtype == 3) {
            $('#AsVehicleReplacedReasonOther').parents('div.span4').slideDown();
            $('#AsVehicleReplacedReasonOther').parents('div.span4').next('div').show();
        }
        else if (subtype == 4) {
            $('#AsVehicleReplacedMerk').parents('div.span4').slideDown();
            $('#AsVehicleReplacedMerk').parents('div.span4').next('div').show();

            $('#AsVehicleReplacedType').parents('div.span4').slideDown();
            $('#AsVehicleReplacedType').parents('div.span4').next('div').show();

            $('#AsVehicleReplacedReason').parents('div.span4').slideDown();
            $('#AsVehicleReplacedReason').parents('div.span4').next('div').show();

            slideUpControls('1', '0'); slideUpControls('0', '4');
            resetChildControls('0', '1');
        }
        else if (subtype == 5) {
            $('#AsVehicleNew').parents('div.span4').slideDown();
            $('#AsVehicleNew').parents('div.span4').next('div').show();

            slideUpControls('0', '5');
            resetChildControls('0', '1'); //additional
            resetChildControls('0', '2'); //replaced
        }
        else if (subtype == 6) {
            $('#AsVehicleNewOther').parents('div.span4').slideDown();
            $('#AsVehicleNewOther').parents('div.span4').next('div').show();
        }
    }
    else if (type == 1) {//additional
        if (subtype == 0) {//all
            $('#AddLabel').parents('div.span4').slideDown();
            $('#AddLabel').parents('div.span4').next('div').show();

            $('#AsVehicleAdditionalSuzuki').parents('div.span4').slideDown();
            $('#AsVehicleAdditionalSuzuki').parents('div.span4').next('div').show();
            $('#AsVehicleAdditionalDaihatsu').parents('div.span4').slideDown();
            $('#AsVehicleAdditionalDaihatsu').parents('div.span4').next('div').show();
            $('#AsVehicleAdditionalMitsubishi').parents('div.span4').slideDown();
            $('#AsVehicleAdditionalMitsubishi').parents('div.span4').next('div').show();
            $('#AsVehicleAdditionalOthers').parents('div.span4').slideDown();
            $('#AsVehicleAdditionalOthers').parents('div.span4').next('div').show();

            slideUpControls('0', '0');
            resetChildControls('0', '2'); //replaced
            resetChildControls('0', '6'); //new
        }
    }
    else if (type == 2) {//passenger car
        if (subtype == 0) {
            $('#PassengerCarYes').parents('div.span4').slideDown();
            $('#PassengerCarYes').parents('div.span4').next('div').show();
        }
        else if (subtype == 1) {
            $('#MerkPsgrCar1').parents('div.span6').slideDown();
            $('#MerkPsgrCar5').parents('div.span6').slideDown();
            $('#PassengerCars').parents('div.span4').slideDown();
        }
        //else if (subtype == 2) {
        //    $('#MerkOtherPsgrCar1').parents('div.span3').slideDown();
        //}
    }
    else if (type == 3) {//occupation
        if (subtype == 0) {
            $('#OccupationDetail').parents('div.span4').slideDown();
            $('#OccupationDetail').parents('div.span4').next('div').show();
            slideUpControls('3', '1');
            resetChildControls('1', '2');
        }
        else if (subtype == 1) {
            $('#OccupationPartOther').parents('div.span4').slideDown();
            $('#OccupationPartOther').parents('div.span4').next('div').show();
            $('#OccupationDetailOther').parents('div.span4').slideDown();
            $('#OccupationDetailOther').parents('div.span4').next('div').show();
            slideUpControls('3', '0');
            resetChildControls('1', '1');
        }
    }
}

function slideUpControls(type, subtype) {
    if (type == 0) {
        if (subtype == 0) {// all
            $('#AsVehicleReplacedMerk').parents('div.span4').slideUp();
            $('#AsVehicleReplacedMerk').parents('div.span4').next('div').hide();

            $('#AsVehicleReplacedType').parents('div.span4').slideUp();
            $('#AsVehicleReplacedType').parents('div.span4').next('div').hide();

            $('#AsVehicleOtherReplacedMerk').parents('div.span4').slideUp();
            $('#AsVehicleOtherReplacedMerk').parents('div.span4').next('div').hide();
            $('#AsVehicleOtherReplacedType').parents('div.span4').slideUp();
            $('#AsVehicleOtherReplacedType').parents('div.span4').next('div').hide();

            $('#AsVehicleReplacedReason').parents('div.span4').slideUp();
            $('#AsVehicleReplacedReason').parents('div.span4').next('div').hide();
            $('#AsVehicleReplacedReasonOther').parents('div.span4').slideUp();
            $('#AsVehicleReplacedReasonOther').parents('div.span4').next('div').hide();

            $('#AsVehicleNew').parents('div.span4').slideUp();
            $('#AsVehicleNew').parents('div.span4').next('div').hide();


            $('#AsVehicleNewOther').parents('div.span4').slideUp();
            $('#AsVehicleNewOther').parents('div.span4').next('div').hide();
        }
        else if (subtype == 1) {
            $('#AsVehicleReplacedType').parents('div.span4').slideUp();
            $('#AsVehicleReplacedType').parents('div.span4').next('div').hide();
        }
        else if (subtype == 2) {
            $('#AsVehicleOtherReplacedMerk').parents('div.span4').slideUp();
            $('#AsVehicleOtherReplacedMerk').parents('div.span4').next('div').hide();
            $('#AsVehicleOtherReplacedType').parents('div.span4').slideUp();
            $('#AsVehicleOtherReplacedType').parents('div.span4').next('div').hide();
        }
        else if (subtype == 3) {
            $('#AsVehicleReplacedReasonOther').parents('div.span4').slideUp();
            $('#AsVehicleReplacedReasonOther').parents('div.span4').next('div').hide();
        } else if (subtype == 4) {
            $('#AsVehicleNew').parents('div.span4').slideUp();
            $('#AsVehicleNew').parents('div.span4').next('div').hide();
        } else if (subtype == 5) {// all
            $('#AsVehicleReplacedMerk').parents('div.span4').slideUp();
            $('#AsVehicleReplacedMerk').parents('div.span4').next('div').hide();

            $('#AsVehicleReplacedType').parents('div.span4').slideUp();
            $('#AsVehicleReplacedType').parents('div.span4').next('div').hide();

            $('#AsVehicleOtherReplacedMerk').parents('div.span4').slideUp();
            $('#AsVehicleOtherReplacedMerk').parents('div.span4').next('div').hide();
            $('#AsVehicleOtherReplacedType').parents('div.span4').slideUp();
            $('#AsVehicleOtherReplacedType').parents('div.span4').next('div').hide();

            $('#AsVehicleReplacedReason').parents('div.span4').slideUp();
            $('#AsVehicleReplacedReason').parents('div.span4').next('div').hide();
            $('#AsVehicleReplacedReasonOther').parents('div.span4').slideUp();
            $('#AsVehicleReplacedReasonOther').parents('div.span4').next('div').hide();
        } else if (subtype == 6) {
            $('#AsVehicleNewOther').parents('div.span4').slideUp();
            $('#AsVehicleNewOther').parents('div.span4').next('div').hide();
            resetChildControls('0', '7');
        }

    }
    else if (type == 1) {
        if (subtype == 0) {//all
            $('#AddLabel').parents('div.span4').slideUp();
            $('#AddLabel').parents('div.span4').next('div').hide();

            $('#AsVehicleAdditionalSuzuki').parents('div.span4').slideUp();
            $('#AsVehicleAdditionalSuzuki').parents('div.span4').next('div').hide();
            $('#AsVehicleAdditionalDaihatsu').parents('div.span4').slideUp();
            $('#AsVehicleAdditionalDaihatsu').parents('div.span4').next('div').hide();
            $('#AsVehicleAdditionalMitsubishi').parents('div.span4').slideUp();
            $('#AsVehicleAdditionalMitsubishi').parents('div.span4').next('div').hide();
            $('#AsVehicleAdditionalOthers').parents('div.span4').slideUp();
            $('#AsVehicleAdditionalOthers').parents('div.span4').next('div').hide();
        }
    }
    else if (type == 2) {
        if (subtype == 0) {
            $('#PassengerCarYes').parents('div.span4').slideUp();
            $('#PassengerCarYes').parents('div.span4').next('div').hide();

            $('#MerkPsgrCar1').parents('div.span6').slideUp();
            $('#MerkPsgrCar5').parents('div.span6').slideUp();
            $('#PassengerCars').parents('div.span4').slideUp();

            resetChildControls('2', '0');
        }
        else if (subtype == 1) {
            $('#MerkPsgrCar1').parents('div.span6').slideUp();
            $('#MerkPsgrCar5').parents('div.span6').slideUp();
            $('#PassengerCars').parents('div.span4').slideUp();
        }
        //else if (subtype == 2) {
        //    $('#MerkOtherPsgrCar1').parents('div.span3').slideUp();
        //}
    }
    else if (type == 3) {//occupation
        if (subtype == 0) {
            $('#OccupationDetail').parents('div.span4').slideUp();
            $('#OccupationDetail').parents('div.span4').next('div').hide();
        }
        else if (subtype == 1) {
            $('#OccupationPartOther').parents('div.span4').slideUp();
            $('#OccupationPartOther').parents('div.span4').next('div').hide();

            $('#OccupationDetailOther').parents('div.span4').slideUp();
            $('#OccupationDetailOther').parents('div.span4').next('div').hide();
        }
    }
}

function resetChildControls(type, subtype) {
    if (type == 0) {//as vehicle
        if (subtype == 0) { // reset all
            $('#AsVehicleReplacedMerk').val('');
            $('#AsVehicleReplacedType').val('');

            $('#AsVehicleAdditionalSuzuki').val('');
            $('#AsVehicleAdditionalDaihatsu').val('');
            $('#AsVehicleAdditionalMitsubishi').val('');
            $('#AsVehicleAdditionalOthers').val('');

            $('#AsVehicleOtherReplacedMerk').val('');
            $('#AsVehicleOtherReplacedType').val('');

            $('#AsVehicleReplacedReason').val('');
            $('#AsVehicleReplacedReasonOther').val('');

            $('#AsVehicleNew').val('');
        }
        else if (subtype == 1) { //reset additional
            $('#AsVehicleAdditionalSuzuki').val('');
            $('#AsVehicleAdditionalDaihatsu').val('');
            $('#AsVehicleAdditionalMitsubishi').val('');
            $('#AsVehicleAdditionalOthers').val('');
        }
        else if (subtype == 2) { //reset replaced
            $('#AsVehicleReplacedMerk').val('');
            $('#AsVehicleReplacedType').val('');
            $('#AsVehicleOtherReplacedMerk').val('');
            $('#AsVehicleOtherReplacedType').val('');

            $('#AsVehicleReplacedReason').val('');
            $('#AsVehicleReplacedReasonOther').val('');
        }
        else if (subtype == 3) { //reset replace type
            $('#AsVehicleReplacedType').val('');
        }
        else if (subtype == 4) {
            $('#AsVehicleOtherReplacedMerk').val('');
            $('#AsVehicleOtherReplacedType').val('');
        }
        else if (subtype == 5) {
            $('#AsVehicleReplacedReasonOther').val('');
        }
        else if (subtype == 6) { // reset new
            $('#AsVehicleNew').val('');
        } else if (subtype == 7) { // reset new
            $('#AsVehicleNewOther').val('');
        }
    }
    else if (type == 1) {//occupation
        if (subtype == 0) {
            $('#OccupationDetail').val('');
            $('#OccupationPartOther').val('');
            $('#OccupationDetailOther').val('');
        } else if (subtype == 1) {
            $('#OccupationDetail').val('');
        }
        else if (subtype == 2) {
            $('#OccupationPartOther').val('');
            $('#OccupationDetailOther').val('');
        }
    }
    else if (type == 2) {//Passenger Cars
        if (subtype == 0) {
            $('#PassengerCarYes').val('');
            $('#MerkPsgrCar1').prop('checked', false).change();
            $('#MerkPsgrCar2').prop('checked', false).change();
            $('#MerkPsgrCar3').prop('checked', false).change();
            $('#MerkPsgrCar4').prop('checked', false).change();
            $('#MerkPsgrCar5').prop('checked', false).change();
        }
    }
}

function selectedModel(row) {
    $('#ChassisCode').val(row.ChassisCode);
    $('#ChassisNo').val(row.ChassisNo);

    getCustomerDetailsByChassis(row);

    //validateInvoiceDate(row);
}

function validateInvoiceDate(row)
{
    var invDate;
    if (row.InvoiceDate != null)
        invDate = moment(row.InvoiceDate).format(SimDms.dateFormat);
    else
        invDate = row.InvoiceDate;
    widget.post("wh.api/questionnaire/ValidateInvoiceDate", { InvoiceDate: invDate, ChassisNo: row.ChassisNo }, function (result) {
        if (result.result == false) {
            //sdms.info({ type: "error", text: result.message });
            //return bErr = true;
        }
        else {
            $('#ChassisCode').val(row.ChassisCode);
            $('#ChassisNo').val(row.ChassisNo);

            getCustomerDetailsByChassis();
        }
    });
}

function getCustomerDetailsByChassis(row) {
    widget.post("wh.api/combo/autoFillByChassis", { ChassisCode: $('#ChassisCode').val(), ChassisNo: $('#ChassisNo').val() }, function (result) {
        var list = [];
        list = Enumerable.From(result[0]).ToArray();

        if (list.length > 0) {
            if (list[0].Result == '2') msgErr = "Nomor rangka tidak ditemukan.";
            else if (list[0].Result == '3') msgErr = "Sales Model tidak masuk dalam kuesioner.";
            else if (list[0].Result == '4') msgErr = "Transaksi sales tidak ditemukan.";
        }

        $('#pnlProfile *').prop('disabled', false);
        widget.enable({ value: false, items: ["Area", "SalesModelCode", "SalesModelReport", "CompanyCode", "CompanyName", "BranchCode", "BranchName"] });
        $('#MerkPsgrCar1').change();
        $('#MerkPsgrCar2').change();
        $('#MerkPsgrCar3').change();
        $('#MerkPsgrCar4').change();
        $('#MerkPsgrCar5').change();

        if (list.length > 0 && list[0].Result == '1') {
           
            var invDate;
            if (row.InvoiceDate != null)
                invDate = moment(row.InvoiceDate).format(SimDms.dateFormat);
            else
                invDate = row.InvoiceDate;
            widget.post("wh.api/questionnaire/ValidateInvoiceDate", { InvoiceDate: invDate, ChassisNo: row.ChassisNo }, function (result) {
                if (result.result == false) {
                    msgErr = result.message;

                    $('#NamaKonsumen').val('');
                    $('#UmurKonsumen').val('');
                    $('#JenisKelamin').select2('val', '');
                    $('#ContactNo').val('');
                    $('#CashOrCredit').select2('val', '');
                    $('#CreditInstalment').val('');

                    widget.enable({ value: false, items: ["btnSave"] });
                    if ($errorFpol) $errorFpol.remove();
                    $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">' + msgErr + '</span>').insertBefore('#ChassisCode');
                    $('#pnlProfile *').prop('disabled', true);
                    widget.enable({ value: true, items: ["ChassisCode", "ChassisNo", "btnChassisCode"] });
                }
                else {
                    $('#SalesModelCode').val(typeof list[0].SalesModelCode != 'undefined' ? list[0].SalesModelCode : '');
                    $('#SalesModelReport').val(typeof list[0].SalesModelReport != 'undefined' ? list[0].SalesModelReport : '');
                    $('#NamaKonsumen').val(typeof list[0].CustomerName != 'undefined' ? list[0].CustomerName == '' ? '' : list[0].CustomerName : '');
                    $('#UmurKonsumen').val(typeof list[0].Age != 'undefined' ? list[0].Age == '' ? '' : list[0].Age : '');
                    $('#JenisKelamin').select2("val", typeof list[0].Gender != 'undefined' ? list[0].Gender == '' ? '' : list[0].Gender : '');
                    $('#ContactNo').val(typeof list[0].ContactNo != 'undefined' ? list[0].ContactNo == '' ? '' : list[0].ContactNo : '');
                    $('#CashOrCredit').select2("val", typeof list[0].isLeasing != 'undefined' ? (list[0].isLeasing == true ? '1' : '0') : '').change();
                    $('#CreditInstalment').val(list[0].Installment);

                    widget.enable({ value: true, items: ["btnSave"] });
                    if ($errorFpol)
                        $errorFpol.remove();
                    $errorFpol = null;
                }
            });
           
        }
        else if (list.length > 0 && list[0].Result == '5') { //tr existing
            var param = { ChassisCode: $('#ChassisCode').val(), ChassisNo: $('#ChassisNo').val() };
            selectBrowse(param);
        }
        else {
            $('#NamaKonsumen').val('');
            $('#UmurKonsumen').val('');
            $('#JenisKelamin').select2('val', '');
            $('#ContactNo').val('');
            $('#CashOrCredit').select2('val', '');
            $('#CreditInstalment').val('');

            widget.enable({ value: false, items: ["btnSave"] });
            if ($errorFpol) $errorFpol.remove();
            $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">' + msgErr + '</span>').insertBefore('#ChassisCode');
            $('#pnlProfile *').prop('disabled', true);
            widget.enable({ value: true, items: ["ChassisCode", "ChassisNo", "btnChassisCode"] });
        }
    });
}

//function editPsrCarList(obj) {
//    $('#MerkPsgrCar1').val($(obj).parents('tr').children('td:first-child').text().substr(0, 1));

//    if ($(obj).parents('tr').children('td:first-child').text().substr(0, 1) == 'E') {
//        $('#MerkOtherPsgrCar1').val($(obj).parents('tr').children('td:first-child').text().replace('E. ', ''));
//        $('#MerkOtherPsgrCar1').parents('div.span3').show();
//    }
//    else {
//        $('#MerkOtherPsgrCar1').parents('div.span3').hide();
//    }
//    $('#TipePsgrCar1').val($(obj).parents('tr').children('td:first-child').next('td').text());

//    $('#btnAdd').hide();
//    $('#btnEditSv').show();
//    $('#btnEditCn').show();
//    $('#hdnEditID').val($(obj).parents('tr').attr('id'));

//    enableBoxAddPC();
//}

//function deletePsrCarList(obj) {
//    if (confirm('Are you sure want to delete this?')) {
//        $(obj).parents('tr').remove();

//        checkMaxPC();
//    }
//}

//function checkMaxPC() {
//    if ($('#pcbody').children('tr').length >= maxpc && maxpc != -1) {
//        disableBoxAddPC();
//    }
//    else {
//        enableBoxAddPC();
//    }
//}

//function disableBoxAddPC() {
//    $('#btnAdd').prop('disabled', true);
//    $('#MerkPsgrCar1').prop('disabled', true);
//    $('#TipePsgrCar1').prop('disabled', true);
//}

//function enableBoxAddPC() {
//    $('#btnAdd').prop('disabled', false);
//    $('#MerkPsgrCar1').prop('disabled', false);
//    $('#TipePsgrCar1').prop('disabled', false);
//}

//var maxpc = 0;

function getinitializeControlValues() {
    widget.post("wh.api/combo/QaFilter", function (result) {
        widget.bind({
            name: 'AsVehicle',
            data: result[0],
            onChange: function () {
                var selected = $('#AsVehicle option:selected').val();
                if (selected == 'A') {
                    widget.post("wh.api/combo/QaMerkByAsCode", { AsCode: $('#AsVehicle option:selected').val() }, function (result) {
                        widget.bind({
                            name: 'AsVehicleReplacedMerk',
                            data: result[0],
                            initialValue: initMerkVal == '' ? null : initMerkVal,
                            onChange: function () {
                                var selectedval = $('#AsVehicleReplacedMerk option:selected').val();
                                if (selectedval == 'A' || selectedval == 'B' || selectedval == 'C') {
                                    widget.post("wh.api/combo/QaTypeByMerk", { MerkCode: selectedval }, function (result) {
                                        widget.bind({
                                            name: 'AsVehicleReplacedType',
                                            data: result[0],
                                            initialValue: initTypeVal
                                        });

                                        initTypeVal = null;
                                    });
                                    slideDownControls('0', '1');
                                }
                                else if (selectedval == 'D') {
                                    slideDownControls('0', '2');
                                }
                                else {
                                    slideUpControls('0', '1'); slideUpControls('0', '2');
                                }
                            },
                            onTriggerAfter: function () {
                                initMerkVal = null;
                                widget.post("wh.api/combo/QaTypeByMerk", { MerkCode: 'A' }, function (result) {
                                    widget.bind({
                                        name: 'AsVehicleReplacedType',
                                        data: result[0],
                                        //initialValue: initTypeVal == '' ? null : initTypeVal
                                    });

                                    //initTypeVal = null;
                                });
                            }
                        });
                    });

                    slideDownControls('0', '4');

                    if ($lblEstimasi2)
                        $lblEstimasi2.remove();
                    if ($lblEstimasi)
                        $lblEstimasi.remove();
                }
                else if (selected == 'B') {
                    widget.post("wh.api/combo/QaMerkByAsCode", { AsCode: $('#AsVehicle option:selected').val() }, function (result) {
                        widget.bind({
                            name: 'AsVehicleReplacedMerk',
                            data: result[0],
                            initialValue: initMerkVal,
                            onChange: function () {
                                var selectedval = $('#AsVehicleReplacedMerk option:selected').val();
                                if (selectedval == 'A' || selectedval == 'B' || selectedval == 'C') {
                                    widget.post("wh.api/combo/QaTypeByMerk", { MerkCode: selectedval }, function (result) {
                                        widget.bind({
                                            name: 'AsVehicleReplacedType',
                                            data: result[0],
                                            initialValue: initTypeVal
                                        });

                                        initTypeVal = null;
                                    });
                                    slideDownControls('0', '1');
                                }
                                else if (selectedval == 'D') {
                                    slideDownControls('0', '2');
                                }
                                else {
                                    slideUpControls('0', '1'); slideUpControls('0', '2');
                                }
                            }
                        });
                        initMerkVal = null;
                    });

                    slideDownControls('0', '0');
                    if ($lblEstimasi2)
                        $lblEstimasi2.remove();
                    if ($lblEstimasi)
                        $lblEstimasi.remove();
                }
                else if (selected == 'C') {
                    slideDownControls('1', '0');
                    if ($lblEstimasi2)
                        $lblEstimasi2.remove();
                    if ($lblEstimasi)
                        $lblEstimasi.remove();
                }
                else {
                    $lblEstimasi = $('<span> ( Estimasi )</span>').appendTo($('#LoadOneTrip').parents('div.span4').children('label'));
                    $lblEstimasi2 = $('<span> ( Estimasi )</span>').appendTo($('#LongInKMAnnualTrip').parents('div.span4').children('label'));
                    resetChildControls('0', '0');
                    slideUpControls('1', '0'); slideUpControls('0', '0');
                    slideDownControls('0', '5');
                }
            }
        });

        //widget.bind({
        //    name: 'AsVehicleReplacedMerk',
        //    data: result[1],
        //    onChange: function () {
        //        if ($(this).val() == 'A' || $(this).val() == 'B' || $(this).val() == 'C') {
        //            widget.post("wh.api/combo/QaTypeByMerk", { MerkCode: $(this).val() }, function (result) {
        //                widget.bind({
        //                    name: 'AsVehicleReplacedType',
        //                    data: result[0],
        //                    initialValue: initTypeVal
        //                });

        //                initTypeVal = '';
        //            });
        //            slideDownControls('0', '1');
        //        }
        //        else if ($(this).val() == 'D') {
        //            slideDownControls('0', '2');
        //        }
        //        else {
        //            slideUpControls('0', '1'); slideUpControls('0', '2');
        //        }
        //    }
        //});

        widget.bind({
            name: 'LoadOneTrip',
            data: result[1]
        });

        widget.bind({
            name: 'LongInKMAnnualTrip',
            data: result[2]
        });

        widget.bind({
            name: 'OccupationPart',
            data: result[3],
            onChange: function () {
                var selected = $('#OccupationPart option:selected').val();
                if (selected == 'K') {
                    slideDownControls('3', '1');
                }
                else if (selected == '') {
                    resetChildControls('1', '0');
                    slideUpControls('3', '0'); slideUpControls('3', '1');
                }
                    //else if ($(this).val() == null) {
                    //    console.log($(this).val());
                    //}
                else {
                    widget.post("wh.api/combo/QaOccupationDetail", { OccupationCode: $('#OccupationPart option:selected').val() }, function (result) {
                        widget.bind({
                            name: 'OccupationDetail',
                            data: result[0],
                            initialValue: initOccDetailVal,
                        });

                        initOccDetailVal = null;
                    });
                    slideDownControls('3', '0');
                }
            }
        });

        widget.bind({
            name: 'PassengerCar',
            data: result[4],
            onChange: function () {
                var selected = $('#PassengerCar option:selected').val();
                if (selected == 'A') {
                    slideDownControls('2', '0');
                }
                else {
                    slideUpControls('2', '0');
                }
            }
        });

        widget.bind({
            name: 'PassengerCarYes',
            data: result[5],
            onChange: function () {
                var selected = $('#PassengerCarYes option:selected').val();
                if (selected != '') {
                    slideDownControls('2', '1');
                }
                else {
                    slideUpControls('2', '1');
                }

                pcunitcount = selected == 'A' ? 1 : selected == 'B' ? 2 : selected == 'C' ? 3 : -1;

                //checkMaxPC();
            }
        });

        widget.bind({
            name: 'MotorCycleExists',
            data: result[6]
        });

        widget.bind({
            name: 'AsVehicleReplacedReason',
            data: result[7],
            onChange: function () {
                var selected = $('#AsVehicleReplacedReason option:selected').val();
                if (selected == '') {
                    slideUpControls('0', '3');
                    resetChildControls('0', '5');
                }
                else if (selected == 'F')//Other
                {
                    slideDownControls('0', '3');
                }
                else {
                    resetChildControls('0', '5');
                    slideUpControls('0', '3');
                }
            }
        });

        widget.bind({
            name: 'AsVehicleNew',
            data: result[9],
            onChange: function () {
                var selected = $('#AsVehicleNew option:selected').val();
                if (selected == 'E') {
                    slideDownControls('0', '6');
                }
                else {
                    slideUpControls('0', '6');
                }
            }
        });

        //widget.bind({
        //    name: 'MerkPsgrCar1',
        //    data: result[9],
        //    onChange: function () {
        //        if ($(this).val() == '') {
        //            slideUpControls('2', '2');
        //        }
        //        else if ($(this).val() == 'E') {
        //            slideDownControls('2', '2');
        //        }
        //        else {
        //            slideUpControls('2', '2');
        //        }
        //    }
        //});

        widget.bind({
            name: 'CashOrCredit',
            data: [{ text: "Cash", value: "0" }, { text: "Credit", value: "1" }],
            onChange: function () {
                var selected = $('#CashOrCredit option:selected').val();
                if (selected == '0') {
                    widget.enable({ value: false, items: ["CreditInstalment"] });
                    widget.setValue({ name: "CreditInstalment", value: "" });
                }
                else {
                    widget.enable({ value: true, items: ["CreditInstalment"] });
                }
            }
        });

        widget.bind({
            name: 'JenisKelamin',
            data: [{ text: "Laki-laki", value: "M" }, { text: "Perempuan", value: "F" }]
        });

        widget.bind({
            name: 'StatusKonsumen',
            data: result[10]
        });
    });
}

var $errorFpol = null;
var initTypeVal = null;
var initOccDetailVal = null;
var $errorUnits = null;
var initMerkVal = null;
var $lblEstimasi = null;
var $lblEstimasi2 = null;
var msgErr = "";
var pcunitcount = 0;
var $errParamExport = null;

widget.render(function () {
    InitControls();

    getinitializeControlValues();

    widget.post("wh.api/Combo/QaCompanyBranch", function (result) {
        var list = [];
        list = Enumerable.From(result[0]).ToArray();

        $('#CompanyCode').val(list[0].CompanyCode);
        $('#CompanyName').val(list[0].CompanyName);
        $('#BranchCode').val(list[0].BranchCode);
        $('#BranchName').val(list[0].BranchName);
        $('#Area').val(list[0].Area);
    });

    $('#btnChassisCode').on('click', function () {
        sdms.lookup({
            title: 'Daftar List Model',
            url: 'wh.api/lookupgrid/LookUpQaCarModel',
            sort: [{ field: 'ChassisCode', dir: 'desc' }],
            fields: [
                { name: 'ChassisCode', text: 'Chassis Code', width: 200 },
                { name: 'ChassisNo', text: 'Chassis No', width: 200 },
                { name: 'SalesModelCode', text: 'SalesModel Code', width: 200 },
                { name: 'SODate', text: 'SalesOrder Date', width: 200, type: 'date' },
                { name: 'InvoiceDate', type:'date', text: 'Invoice Date', width: 200 }
            ],
            dblclick: function (row) {
                selectedModel(row);
            },
            onclick: function (row) {
                selectedModel(row);
            }
        });
    });

    //$('#btnAdd').on("click", function () {
    //    if ($('#pcbody').children('tr').length < maxpc || maxpc == -1) {
    //        var id = "";
    //        if ($('#pcbody').children('tr').length > 0) {
    //            id = 'pc' + (parseInt($('#pcbody').children('tr:last-child').attr('id').replace('pc', '')) + 1);
    //        }
    //        else {
    //            id = 'pc1';
    //        }

    //        if ($('#MerkPsgrCar1 option:selected').val() != '' && $('#TipePsgrCar1').val() != '') {
    //            var merk = $('#MerkPsgrCar1 option:selected').val() == 'E' ? 'E. ' + $('#MerkOtherPsgrCar1').val() : $('#MerkPsgrCar1 option:selected').text();
    //            var html = "<tr id=\"" + id + "\"><td>" + merk + "</td><td>" + $('#TipePsgrCar1').val() + "</td>" +
    //                "<td><a href=\"#\" class=\"icon fa fa-edit\" onclick=\"editPsrCarList($(this)); return false;\"></a> | " +
    //                "<a href=\"#\" class=\"icon fa fa-trash-o\" onclick=\"deletePsrCarList($(this)); return false;\"></a></td></tr>";
    //            $('#pcbody').append(html);
    //        }
    //        $('#MerkPsgrCar1').val('');
    //        $('#MerkOtherPsgrCar1').val('');
    //        $('#MerkOtherPsgrCar1').parents('div.span3').hide();
    //        $('#TipePsgrCar1').val('');

    //        checkMaxPC();
    //    }
    //});

    //$('#btnEditSv').on("click", function () {
    //    var id = $('#hdnEditID').val();
    //    if ($('#MerkPsgrCar1 option:selected').val() != '' && $('#TipePsgrCar1').val() != '') {
    //        var merk = $('#MerkPsgrCar1 option:selected').val() == 'E' ? 'E. ' + $('#MerkOtherPsgrCar1').val() : $('#MerkPsgrCar1 option:selected').text();
    //        $('#' + id).children('td:first-child').text(merk);
    //        $('#' + id).children('td:first-child').next('td').text($('#TipePsgrCar1').val());

    //        $('#btnEditSv').hide(); $('#btnEditCn').hide(); $('#btnAdd').show();
    //        $('#MerkPsgrCar1').val('');
    //        $('#TipePsgrCar1').val('');
    //        $('#MerkOtherPsgrCar1').val('');
    //        $('#MerkOtherPsgrCar1').parents('div.span3').hide();

    //        checkMaxPC();
    //    }
    //});

    //$('#btnEditCn').on("click", function () {
    //    $('#btnEditSv').hide(); $('#btnEditCn').hide(); $('#btnAdd').show();
    //    $('#MerkPsgrCar1').val('');
    //    $('#TipePsgrCar1').val('');
    //    $('#MerkOtherPsgrCar1').val('');
    //    $('#MerkOtherPsgrCar1').parents('div.span3').hide();

    //    checkMaxPC();
    //});

    widget.checked("MerkPsgrCar1", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["TipePsgrCar1"] });

        }
        else {
            widget.enable({ value: false, items: ["TipePsgrCar1"] });
            widget.setValue({ name: "TipePsgrCar1", value: "" });
        }
    });

    widget.checked("MerkPsgrCar2", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["TipePsgrCar2"] });

        }
        else {
            widget.enable({ value: false, items: ["TipePsgrCar2"] });
            widget.setValue({ name: "TipePsgrCar2", value: "" });
        }
    });

    widget.checked("MerkPsgrCar3", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["TipePsgrCar3"] });

        }
        else {
            widget.enable({ value: false, items: ["TipePsgrCar3"] });
            widget.setValue({ name: "TipePsgrCar3", value: "" });
        }
    });

    widget.checked("MerkPsgrCar4", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["TipePsgrCar4"] });

        }
        else {
            widget.enable({ value: false, items: ["TipePsgrCar4"] });
            widget.setValue({ name: "TipePsgrCar4", value: "" });
        }
    });

    widget.checked("MerkPsgrCar5", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["TipePsgrCar5", "MerkOthersPsgrCar5"] });

        }
        else {
            widget.enable({ value: false, items: ["TipePsgrCar5", "MerkOthersPsgrCar5"] });
            widget.setValue({ name: "TipePsgrCar5", value: "" });
            widget.setValue({ name: "MerkOthersPsgrCar5", value: "" });
        }
    });

    $('#UmurKonsumen, #ContactNo, #CreditInstalment, #AsVehicleAdditionalSuzuki, #AsVehicleAdditionalDaihatsu, #AsVehicleAdditionalMitsubishi, #AsVehicleAdditionalOthers').keypress(function (ev) {
        ev = (ev) ? ev : window.event;
        var charcode = (ev.which) ? ev.which : ev.keyCode;

        if (charcode > 31 && (charcode < 48 || charcode > 57)) {
            return false;
        }
        return true;
    });

    //$('#FakPol').keyup(function (e) {
    //    var code = e.keyCode || e.which;
    //    if (code == 13) {
    //        widget.post("wh.api/combo/autoFillByFakPol", { FakPol: $('#FakPol').val() }, function (result) {
    //            var list = [];
    //            list = Enumerable.From(result[0]).ToArray();
    //            if (list.length > 0 && list[0].Result == '2') {
    //                $('#ChassisCode').val(list[0].ChassisCode);
    //                $('#ChassisNo').val(list[0].ChassisNo);
    //                $('#NamaKonsumen').val(list[0].CustomerName);
    //                $('#UmurKonsumen').val(list[0].Age);
    //                $('#JenisKelamin').val(list[0].Gender);
    //                $('#ContactNo').val(list[0].ContactNo);
    //                $('#CashOrCredit').val(list[0].isLeasing == true ? '1' : '0');

    //                if (list[0].isLeasing) {
    //                    $('#CreditInstalment').prop("required", "required");
    //                    $('#CreditInstalment').val(list[0].Installment);
    //                }
    //                else {
    //                    widget.enable({ value: false, items: ["CreditInstalment"] });
    //                    widget.setValue({ name: "CreditInstalment", value: "" });
    //                }

    //                widget.enable({ value: true, items: ["btnSave"] });
    //                if ($errorFpol)
    //                    $errorFpol.remove();
    //                $errorFpol = null;
    //            }
    //            else if (list.length > 0 && list[0].Result == '1') {
    //                $('#ChassisCode').val(list[0].ChassisCode);
    //                $('#ChassisNo').val(list[0].ChassisNo);

    //                if ($errorFpol)
    //                    $errorFpol.remove();
    //                $errorFpol = null;

    //                widget.enable({ value: true, items: ["btnSave"] });
    //            }
    //            else if (list.length > 0 && list[0].Result == '3') {
    //                $('#ChassisCode').val('');
    //                $('#ChassisNo').val('');
    //                $('#NamaKonsumen').val('');
    //                $('#UmurKonsumen').val('');
    //                $('#JenisKelamin').val('');
    //                $('#ContactNo').val('');
    //                $('#CashOrCredit').val('');
    //                $('#CreditInstalment').val('');

    //                widget.enable({ value: false, items: ["btnSave"] });
    //                if ($errorFpol) $errorFpol.remove();
    //                $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Customer does not exists!</span>').insertBefore('#FakPol');
    //            }
    //            else {
    //                $('#ChassisCode').val('');
    //                $('#ChassisNo').val('');
    //                $('#NamaKonsumen').val('');
    //                $('#UmurKonsumen').val('');
    //                $('#JenisKelamin').val('');
    //                $('#ContactNo').val('');
    //                $('#CashOrCredit').val('');
    //                $('#CreditInstalment').val('');

    //                widget.enable({ value: false, items: ["btnSave"] });
    //                if ($errorFpol) $errorFpol.remove();
    //                $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Faktur Polisi not found!</span>').insertBefore('#FakPol');
    //            }
    //        });
    //    }
    //});


    $('#ChassisCode, #ChassisNo').keyup(function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) {
            getCustomerDetailsByChassis();
        }
    });
});

function exportToExcel() {
    var url = "wh.api/Questionnaire/QuestionnaireRekapProd?";
    var filter = widget.serializeObject('pnlExport');

    if (typeof filter.StartDate == 'undefined' || filter.StartDate == '') {
        sdms.info({ type: "warning", text: "Tanggal mulai tidak boleh kosong" });
        //if ($errParamExport) $errParamExport.remove();
        //$errParamExport = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Tanggal akhir tidak boleh lebih besar dari tanggal mulai.</span>').insertBefore('.modal-footer');
    }
    else if (typeof filter.EndDate == 'undefined' || filter.EndDate == '') {
        sdms.info({ type: "warning", text: "Tanggal akhir tidak boleh kosong" });
        //if ($errParamExport) $errParamExport.remove();
        //$errParamExport = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Tanggal akhir tidak boleh lebih besar dari tanggal mulai.</span>').insertBefore('.modal-footer');
    }
    else if (Date.parse(filter.EndDate) - Date.parse(filter.StartDate) < 0) {
        sdms.info({ type: "warning", text: "Tanggal akhir tidak boleh lebih kecil dari tanggal mulai" });
        //if ($errParamExport) $errParamExport.remove();
        //$errParamExport = $('<span style="margin: -10px 0 18px 0;font-size: 12px;font-style: italic;color:#c60f13;">Tanggal akhir tidak boleh lebih besar dari tanggal mulai.</span>').insertBefore('.modal-footer');
    }
    else {
        filter.StartDate = $('input[name="StartDate"]').val();
        filter.EndDate = $('input[name="EndDate"]').val();

        if ($errParamExport) $errParamExport.remove();

        var params = ''

        $.each(filter || [], function (key, val) {
            params += key + '=' + val + '&';
        });
        params = params.substring(0, params.length - 1);

        url += params;
        window.location = url;
    }
}