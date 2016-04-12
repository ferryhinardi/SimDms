var widget = new SimDms.Widget({
    title: 'Questionnaire',
    xtype: 'panels',
    toolbars: [
        { text: 'Browse', action: 'browse', icon: 'fa fa-search' },
        { text: 'Save', action: 'save', icon: 'fa fa-save', name: "btnSave" },
        { text: 'Cancel', action: 'cancel', icon: 'fa fa-refresh', cls: 'hide' },
        { text: 'Delete', action: 'delete', icon: 'fa fa-trash-o', cls: 'hide' },
        //{ text: 'Clear', action: 'clear', icon: 'fa fa-gear' },
        //{ text: 'Export to Excel', action: 'exportToExcel', icon: 'fa fa-file-excel-o', cls: '' },
        { text: 'Expand', action: 'expand', icon: 'fa fa-expand' },
        { text: 'Collapse', action: 'collapse', icon: 'fa fa-compress', cls: 'hide' },
    ],
    panels: [
        {
            name: "pnlHeader",
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
                { type: "span" },
                { name: "ChassisCode", text: "Chassis Code", required: true, type: "popup", cls: "span4", lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ChassisNo", text: "Chassis No", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "StatusKonsumen", text: "Status Konsumen", type: 'select', required: true, cls: "span4", lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { name: "ErtigaOrWagonR", text: "Ertiga Or WagonR", type: 'text', cls: "span4", lblstyle: ' style="display:none;"', ctrlstyle: ' style="display:none;"', widthspanstyle: "style='display:none;'" },
                { type: "span" },
                {
                    text: "SalesModel", type: "controls", cls: 'span8', lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:94%; padding-bottom:5px;'", items: [
                        { name: "SalesModelCode", text: "SalesModel Code", type: "text", cls: "span4", disabled: true },
                        { name: "SalesModelReport", text: "SalesModel Report", type: "text", cls: "span4", disabled: true },
                    ]
                },
                { type: "span" },
                {
                    text: "Colour", type: "controls", cls: 'span8', lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:94%; padding-bottom:5px;'", items: [
                        { name: "ColourCode", text: "Colour Code", type: "text", cls: "span4", disabled: true },
                        { name: "ColourDesc", text: "Colour Description", type: "text", cls: "span4", disabled: true },
                    ]
                },
            ]
        },
        {
            name: "pnlQaUniversal",
            items: [
                { name: "EmployeeName", text: "Nama Salesman", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "FleetPembelianAtasNama", text: "Pembelian Atas Nama", cls: "span4", type: 'select', required: true, lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "NamaKonsumen1", text: "Nama Konsumen", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "IndBirthDate1", text: "Tanggal Lahir", cls: "span4", type: "datepicker", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "IndUmurKonsumen1", text: "Usia", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'", disabled: true },
                { type: "span" },
                { name: "IndJenisKelamin1", text: "Jenis Kelamin", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "FleetJenisUsaha", text: "Jenis Usaha", cls: "span4", type: "text", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "CityCode", text: "", cls: "span4 hide", type: "text", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "Kota1", text: "Kota Domisili", cls: "span4", type: "text", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "CashOrCredit1", text: "Pembelian Cash/Credit", type: "select", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { name: "CreditInstalment1", text: "Jikalau Kredit Cicilan Berapa lama", required: true, placeholder: "X Bulan", cls: "span4", lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "FleetPurpose", text: "1. Tujuan pembelian untuk", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "PurposeOther", text: "Lainnya, sebutkan..", cls: "span4 hide", type: "text", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "FleetPeriod", text: "2. Periode pembelian", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "FleetRenovation", text: "3. Penggantian / peremajaan mobil", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "IndOccupation1", text: "1. Pekerjaan", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "OccupationOther1", text: "Lainnya, sebutkan..", cls: "span4 hide", type: "text", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ProductSource1", text: "2. Mengetahui adanya produk Suzuki Ertiga dari", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ProductSourceDetail1", text: "Sebutkan..", cls: "span4 hide", type: "text", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "TestDrive1", text: "3. Apakah melakukan test drive sebelum memutuskan membeli", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "RespondenStatus1", text: "4. Alasan membeli Ertiga", cls: "span4", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "FirstTime1", text: "Sebelumnya..", cls: "span4 hide", type: "select", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ReplacementMerk1", text: "Merk", type: "select", cls: "span4 hide", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ReplacementMerkOther1", text: "Lainnya, Sebutkan..", cls: "span4 hide", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ReplacementType1", text: "Tipe", cls: "span4 hide", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ReplacementYear1", text: "Tahun", cls: "span4 hide", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                {
                    text: "Merk apa", type: "controls", cls: 'span6 hide', lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'", items: [
                        { name: "ReplacementOrAdditionalMerk1", text: "Honda", type: "select", cls: "span4" },
                        { name: "ReplacementOrAdditionalMerkOther1", text: "Merk", cls: "span4 hide" },
                        { name: "ReplacementOrAdditionalType1", text: "Tipe", cls: "span4" },
                        { name: "ReplacementOrAdditionalYear1", text: "Tahun", cls: "span4" },
                        {
                            type: "buttons",
                            cls: "span6",
                            items: [
                                { name: "btnAdd", text: " Add", icon: "fa fa-save", btntype: " type=\"button\" " },
                                { name: "btnEditSv", text: " Save", icon: "fa fa-save", btntype: " type=\"button\" ", cls: "hide" },
                                { name: "btnEditCn", text: " Cancel", icon: "fa fa-cancel", btntype: " type=\"button\" ", additional: " style=\"margin-left:3px;\" ", cls: "hide" },
                            ]
                        },
                        {
                            name: "PassengerCars",
                            type: "table",
                            width: "300px",
                            cls: "span4",
                            dataId: "pcbody",
                            hdnname: "hdnEditID",
                            columns: [
                                    { title: "Merk", width: "150px" },
                                    { title: "Tipe", width: "150px" },
                                    { title: "Tahun", width: "150px" },
                                    { title: "Action", width: "150px" },
                            ],
                        },
                    ]
                },
                { name: "TotalCar1", text: "Total mobil yang ada sekarang", cls: "span4 hide", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ReplacementReason1", text: "5. Alasan mengganti mobil lama", type: "select", cls: "span4 hide", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ReplacementReasonOther1", text: "Alasan lain, sebutkan..", type: "text", cls: "span4 hide", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "Comparison1", text: "7. Pada saat membeli Ertiga, merk/model lain apa yang anda pakai sebagai perbandingan", type: "select", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                { name: "ComparisonOther1", text: "Lainnya, sebutkan..", type: "text", cls: "span4 hide", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                { type: "span" },
                //{ name: "Aspect1", text: "8. Anda memutuskan membeli Ertiga, karena aspek (boleh memilih lebih dari 1 sesuai prioritas)", type: "select", cls: "span4", required: true, lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'" },
                //{ type: "span" },
                {
                    text: "8. Anda memutuskan membeli Ertiga, karena aspek (boleh memilih lebih dari 1 sesuai prioritas)", type: "controls", cls: 'span6', lblstyle: ' style="width:360px; line-height:13px;"', ctrlstyle: ' style="padding-left:380px;"', widthspanstyle: "style='width:65%;'", items: [
                        { name: "AspectBrand", text: "Image/merk Suzuki", type: "check", cls: "span5" },
                        { name: "PriorityAB", text: "Priority", cls: "span3", required: true, disabled: true },
                        { name: "AspectEngine", text: "Performa mesin Suzuki", type: "check", cls: "span5" },
                        { name: "PriorityAE", text: "Priority", cls: "span3", required: true, disabled: true },
                        { name: "AspectExterior", text: "Desain Eksterior Ertiga", type: "check", cls: "span5" },
                        { name: "PriorityAEX", text: "Priority", cls: "span3", required: true, disabled: true },
                        { name: "AspectInterior", text: "Desain Interior Ertiga", type: "check", cls: "span5" },
                        { name: "PriorityAI", text: "Priority", cls: "span3", required: true, disabled: true },
                        { name: "AspectPrice", text: "Harganya terjangkau", type: "check", cls: "span5" },
                        { name: "PriorityAP", text: "Priority", cls: "span3", required: true, disabled: true },
                        { name: "AspectAfterSales", text: "Perawatan murah/parts mudah didapat", type: "check", cls: "span5" },
                        { name: "PriorityAAS", text: "Priority", cls: "span3", required: true, disabled: true },
                        { name: "AspectOutlet", text: "Outlet Suzuki yang tersebar", type: "check", cls: "span5" },
                        { name: "PriorityAO", text: "Priority", cls: "span3", required: true, disabled: true },
                        { name: "AspectResalePrice", text: "Harga jual kembali baik", type: "check", cls: "span5" },
                        { name: "PriorityARP", text: "Priority", cls: "span3", required: true, disabled: true },
                        { name: "AspectOther", text: "Other", type: "check", cls: "span5" },
                        { name: "AspectOtherInput", text: "Sebutkan..", type: "text", cls: "span3", required: true, disabled: true },
                        { name: "AsOtherHdn", text: "Other", type: "check", cls: "span5", spanstyle: "visibility:hidden;", style: "visibility:hidden;" },
                        { name: "PriorityOT", text: "Priority", cls: "span3", required: true, disabled: true },
                    ]
                },
            ]
        },
    ],
    onToolbarClick: function (action) {
        switch (action) {
            case 'save':
                saveQa2();
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
            default:
                break;
        }
    },
});

function saveQa2() {
    var valid = $(".main form").valid();

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

    if ($('#pcbody').children('tr').length > $('#TotalCar1').val()) {
        if ($errorFpol)
            $errorFpol.remove();
        $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">Total Car tidak boleh lebih kecil dari mobil tambahan yang disebutkan.</span>').insertBefore('#TotalCar1');
        valid = false;
    }
    else {
        if ($errorFpol)
            $errorFpol.remove();
        $errorFpol = null;
    }

    if ($('input:checkbox[id^="Aspect"]:checked').length <= 0) {
        if ($errorFpol)
            $errorFpol.remove();
        $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">Aspek harus minimal pilih satu.</span>').insertBefore('#PriorityAB');
        valid = false;
    }
    else {
        if ($errorFpol)
            $errorFpol.remove();
        $errorFpol = null;
    }

    if (valid) {
        var filter = widget.serializeObject();

        var addtCars = new Array();

        if ($('#pcbody').children('tr').length > 0) {
            $('#pcbody').children('tr').each(function () {
                var merk = $(this).children('td:first-child').text();
                var tipe = $(this).children('td:first-child').next('td').text();
                var year = $(this).children('td:first-child').next('td').next('td').text();
                var arr = { merk: merk, tipe: tipe, year: year };
                addtCars.push(arr);
            });
        }

        filter.addtCars = JSON.stringify(addtCars);

        widget.post("wh.api/Questionnaire2/QaTransaction", filter, function (result) {
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
        title: "Daftar Questionnaire Outlet Anda",
        url: "wh.api/lookupgrid/LookUpQuestionnaire2",
        sort: [{ field: 'ChassisCode', dir: 'asc' }, { field: 'ChassisNo', dir: 'asc' }],
        fields: [
            { name: 'ChassisCode', text: 'Chassis Code', width: 200 },
            { name: 'ChassisNo', text: 'Chassis No', width: 200 },
            { name: 'ErtigaOrWagonR', text: 'ErtigaOrWagonR', width: 200 }
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

function removeQa() {
    widget.post("wh.api/Questionnaire2/removeQuestionnaire", { "ChassisCode": $('#ChassisCode').val(), "ChassisNo": $('#ChassisNo').val() }, function (result) {
        widget.showNotification(result.message || SimDms.defaultInformationMessage, function () {
            location.reload();
        });
    });
}

function selectBrowse(row) {
    widget.setValue({ name: "ChassisCode", value: row.ChassisCode });
    $('#ChassisCode').prop("disabled", true);
    widget.setValue({ name: "ChassisNo", value: row.ChassisNo });
    $('#ChassisNo').prop("disabled", true);
    //$('#StatusKonsumen').val(row.StatusKonsumenCode);
    $('#StatusKonsumen').select2("val", row.StatusKonsumenCode).change();
    $('#StatusKonsumen').prop("disabled", true);
    $('#ErtigaOrWagonR').val(row.ErtigaOrWagonR);

    DisplayPanel(row.ErtigaOrWagonR, row.StatusKonsumenCode, function () {

        if ($errorFpol) $errorFpol.remove();

        widget.post("wh.api/Questionnaire2/populateEditQuestionnaire", { "ChassisCode": row.ChassisCode, "ChassisNo": row.ChassisNo }, function (result) {
            var list = [];
            list = Enumerable.From(result[0]).ToArray();

            if ((list.length <= 0 ? '0' : list[0].isEligible) == '0') {
                if ($errorFpol) $errorFpol = null;
                $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">Data tidak dapat diubah karena sudah diinput lewat dari sehari.</span>').insertBefore('#ChassisCode');
                widget.enable({ value: false, items: ["btnSave"] });
                $('.gl-widget *').prop('disabled', true);
                widget.showToolbars(['browse', 'save', 'cancel', 'expand']);
            }
            else {
                widget.showToolbars(['browse', 'save', 'delete', 'cancel', 'expand']);
                widget.enable({ value: true, items: ["btnSave"] });
                $('.gl-widget *').prop('disabled', false);
                widget.enable({ value: false, items: ["Area", "ChassisCode", "ChassisNo", "SalesModelCode", "SalesModelReport", "CompanyCode", "CompanyName", "BranchCode", "BranchName", "ColourCode", "ColourDesc", "StatusKonsumen"] });

                $("input[id^='Priority']").prop("disabled", true);
                $('#AspectOtherInput').prop("disabled", true);

                $('#CashOrCredit').change();
            }

            //widget.setValue({ name: "ChassisCode", value: (list.length <= 0 ? '' : list[0].ChassisCode), isControl: false });
            //$('#ChassisCode').prop("disabled", true);
            //widget.setValue({ name: "ChassisNo", value: (list.length <= 0 ? '' : list[0].ChassisNo), isControl: false });
            //$('#ChassisNo').prop("disabled", true);

            //$('#StatusKonsumen').val(list.length <= 0 ? '' : list[0].StatusKonsumenCode);
            //$('#StatusKonsumen').change();

            $('#SalesModelCode').val(typeof list[0].SalesModelCode != 'undefined' ? list[0].SalesModelCode : '');
            $('#SalesModelReport').val(typeof list[0].SalesModelReport != 'undefined' ? list[0].SalesModelReport : '');
            $('#ColourCode').val(typeof list[0].ColourCode != 'undefined' ? list[0].ColourCode : '');
            $('#ColourDesc').val(typeof list[0].RefferenceDesc1 != 'undefined' ? list[0].RefferenceDesc1 : '');
            $('#NamaKonsumen1').val(typeof list[0].RespondenName != 'undefined' ? list[0].RespondenName == '' ? '' : list[0].RespondenName : '');
            $('#IndUmurKonsumen1').val(typeof list[0].RespondenAge != 'undefined' ? list[0].RespondenAge == '' ? '' : list[0].RespondenAge : '');
            //$('#IndJenisKelamin1').val(typeof list[0].RespondenGender != 'undefined' ? list[0].RespondenGender == '' ? '' : list[0].RespondenGender : '');
            $('#IndJenisKelamin1').select2("val", (typeof list[0].RespondenGender != 'undefined' ? list[0].RespondenGender == '' ? '' : list[0].RespondenGender : '')).change();
            //$('#CashOrCredit1').val(typeof list[0].IsCredit != 'undefined' ? (list[0].IsCredit == true ? '1' : '0') : '');
            $('#CashOrCredit1').select2("val", (typeof list[0].IsCredit != 'undefined' ? (list[0].IsCredit == true ? '1' : '0') : '')).change();
            $('#EmployeeName').val(typeof list[0].EmployeeName != 'undefined' ? list[0].EmployeeName == '' ? '' : list[0].EmployeeName : '');

            var TglOp = typeof list[0].BirthDate != 'undefined' ? list[0].BirthDate == '' ? '' : new Date(parseInt(list[0].BirthDate.replace("/Date(", "").replace(")/", ""))) : '';

            if (TglOp != '')
                $("input[name='IndBirthDate1']").val([("0" + TglOp.getDate()).slice(-2), ("0" + (TglOp.getMonth() + 1)).slice(-2), TglOp.getFullYear()].join('-'));
            else
                $("input[name='IndBirthDate1']").val('');

            $('#CityCode').val(typeof list[0].CityCode != 'undefined' ? list[0].CityCode == '' ? '' : list[0].CityCode : '');
            $('#Kota1').val(typeof list[0].LookUpValueName != 'undefined' ? list[0].LookUpValueName == '' ? '' : list[0].LookUpValueName : '');

            if (list[0].IsCredit == true) {
                $('#CreditInstalment1').prop("required", "required");
                $('#CreditInstalment1').val(list[0].Installment);
            }
            else if (list[0].IsCredit == false) {
                widget.enable({ value: false, items: ["CreditInstalment1"] });
                widget.setValue({ name: "CreditInstalment1", value: "" });
            }

            var occpCode = list.length <= 0 ? '' : list[0].OccupationCode;
            //$('#IndOccupation1').val((list.length <= 0 ? '' : list[0].OccupationCode)).change();
            $('#IndOccupation1').select2("val", (list.length <= 0 ? '' : list[0].OccupationCode)).change();
            $('#OccupationOther1').val((list.length <= 0 ? '' : list[0].OccupationOthers));

            //$('#FleetPembelianAtasNama').val((list.length <= 0 ? '' : list[0].PembelianAtasNamaCode));
            $('#FleetPembelianAtasNama').select2("val", (list.length <= 0 ? '' : list[0].PembelianAtasNamaCode)).change();
            $('#FleetJenisUsaha').val((list.length <= 0 ? '' : list[0].JenisUsaha));

            //$('#FleetPurpose').val((list.length <= 0 ? '' : list[0].PurposeCode));
            $('#FleetPurpose').select2("val", (list.length <= 0 ? '' : list[0].PurposeCode)).change();
            //$('#FleetPurpose').change();
            $('#PurposeOther').val((list.length <= 0 ? '' : list[0].PurposeOthers));
            //$('#FleetPeriod').val((list.length <= 0 ? '' : list[0].PeriodCode));
            $('#FleetPeriod').select2("val", (list.length <= 0 ? '' : list[0].PeriodCode)).change();
            //$('#FleetRenovation').val((list.length <= 0 ? '' : list[0].RenovationCode));
            $('#FleetRenovation').select2("val", (list.length <= 0 ? '' : list[0].RenovationCode)).change();

            //$('#ProductSource1').val((list.length <= 0 ? '' : list[0].ProductSourceCode));
            $('#ProductSource1').select2("val", (list.length <= 0 ? '' : list[0].ProductSourceCode)).change();
            //$('#ProductSource1').change();
            $('#ProductSourceDetail1').val((list.length <= 0 ? '' : list[0].ProductSourceDetail));
            //$('#TestDrive1').val((list.length <= 0 ? '' : list[0].TestDriveCode));
            $('#TestDrive1').select2("val", (list.length <= 0 ? '' : list[0].TestDriveCode)).change();

            var rs = list.length <= 0 ? '' : list[0].RespondenStatusCode;
            //$('#RespondenStatus1').val(rs);
            $('#RespondenStatus1').select2("val", rs).change();
            //$('#RespondenStatus1').change();

            $('#FirstTime1').val('');
            $('#ReplacementMerk1').val('');
            $('#ReplacementMerkOther1').val('');
            $('#ReplacementType1').val('');
            $('#ReplacementYear1').val('');
            $('#pcbody').empty();

            if (rs == 'A') {
                //$('#FirstTime1').val((list.length <= 0 ? '' : list[0].FirstTimeCode));
                $('#FirstTime1').select2("val", (list.length <= 0 ? '' : list[0].FirstTimeCode)).change();
            }
            else if (rs == 'B') {
                //$('#ReplacementMerk1').val((list.length <= 0 ? '' : list[0].IsReplacementMerkCode));
                $('#ReplacementMerk1').select2("val", (list.length <= 0 ? '' : list[0].IsReplacementMerkCode)).change();
                $('#ReplacementMerkOther1').val((list.length <= 0 ? '' : list[0].IsReplacementMerkOthers));
                $('#ReplacementType1').val((list.length <= 0 ? '' : list[0].IsReplacementType));
                $('#ReplacementYear1').val((list.length <= 0 ? '' : list[0].IsReplacementYear));
            }
            else if (rs == 'C') {
                widget.post("wh.api/Questionnaire2/getTrQaSub", { "ChassisCode": $('#ChassisCode').val(), "ChassisNo": $('#ChassisNo').val() }, function (result) {
                    var list = [];
                    list = Enumerable.From(result[0]).ToArray();

                    var oneditclick = $errorFpol != null ? '' : 'editReplAddtCar($(this));';
                    var ondeleteclick = $errorFpol != null ? '' : 'deleteReplAddtCar($(this));';
                    for (var i = 0; i < list.length; i++) {
                        $('#pcbody').append('<tr id="pc' + i + 1 + '"><td>' + list[i].IsAdditionalMerkCode + ". " + (list[i].IsAdditionalMerkCode == 'K' ? list[i].IsAdditionalMerkOthers : list[i].IsAdditionalMerkDescI) + '</td><td>' + list[i].IsAdditionalType + '</td><td>' + list[i].IsAdditionalYear + '</td><td><a href="#" class="icon fa fa-edit" onclick="' + oneditclick + ' return false;"></a> | <a href="#" class="icon fa fa-trash-o" onclick="' + ondeleteclick + ' return false;"></a></td></tr>');
                    }
                });
            }

            $('#TotalCar1').val((list.length <= 0 ? '' : list[0].IsAdditionalTotal));
            //$('#ReplacementReason1').val((list.length <= 0 ? '' : list[0].IsReplacementReasonCode));
            $('#ReplacementReason1').select2("val", (list.length <= 0 ? '' : list[0].IsReplacementReasonCode)).change();
            //$('#ReplacementReason1').change();
            $('#ReplacementReasonOther1').val((list.length <= 0 ? '' : list[0].IsReplacementReasonOthers));
            //$('#Comparison1').val((list.length <= 0 ? '' : list[0].ComparisonCode));
            $('#Comparison1').select2("val", (list.length <= 0 ? '' : list[0].ComparisonCode)).change();
            //$('#Comparison1').change();
            $('#ComparisonOther1').val((list.length <= 0 ? '' : list[0].ComparisonOthers));

            var ab = list.length <= 0 ? '' : list[0].AspectBrand;
            if (ab != '' && ab != null) {
                $('#AspectBrand').prop('checked', true);
                $('#AspectBrand').change();
                $('#PriorityAB').val(ab);
            }
            else {
                $('#AspectBrand').prop('checked', false);
                $('#PriorityAB').val('');
            }

            var ae = list.length <= 0 ? '' : list[0].AspectEngine;
            if (ae != '' && ae != null) {
                $('#AspectEngine').prop('checked', true);
                $('#AspectEngine').change();
                $('#PriorityAE').val(ae);
            }
            else {
                $('#AspectEngine').prop('checked', false);
                $('#PriorityAE').val('');
            }

            var aex = list.length <= 0 ? '' : list[0].AspectExterior;
            if (aex != '' && aex != null) {
                $('#AspectExterior').prop('checked', true);
                $('#AspectExterior').change();
                $('#PriorityAEX').val(aex);
            }
            else {
                $('#AspectExterior').prop('checked', false);
                $('#PriorityAEX').val('');
            }

            var ai = list.length <= 0 ? '' : list[0].AspectInterior;
            if (ai != '' && ai != null) {
                $('#AspectInterior').prop('checked', true);
                $('#AspectInterior').change();
                $('#PriorityAI').val(ai);
            }
            else {
                $('#AspectInterior').prop('checked', false);
                $('#PriorityAI').val('');
            }

            var ap = list.length <= 0 ? '' : list[0].AspectPrice;
            if (ap != '' && ap != null) {
                $('#AspectPrice').prop('checked', true);
                $('#AspectPrice').change();
                $('#PriorityAP').val(ap);
            }
            else {
                $('#AspectPrice').prop('checked', false);
                $('#PriorityAP').val('');
            }

            var aas = list.length <= 0 ? '' : list[0].AspectAfterSales;
            if (aas != '' && aas != null) {
                $('#AspectAfterSales').prop('checked', true);
                $('#AspectAfterSales').change();
                $('#PriorityAAS').val(aas);
            }
            else {
                $('#AspectAfterSales').prop('checked', false);
                $('#PriorityAAS').val('');
            }

            var ao = list.length <= 0 ? '' : list[0].AspectOutlet;
            if (ao != '' && ao != null) {
                $('#AspectOutlet').prop('checked', true);
                $('#AspectOutlet').change();
                $('#PriorityAO').val(ao);
            }
            else {
                $('#AspectOutlet').prop('checked', false);
                $('#PriorityAO').val('');
            }

            var arp = list.length <= 0 ? '' : list[0].AspectResalePrice;
            if (arp != '' && arp != null) {
                $('#AspectResalePrice').prop('checked', true);
                $('#AspectResalePrice').change();
                $('#PriorityARP').val(arp);
            }
            else {
                $('#AspectResalePrice').prop('checked', false);
                $('#PriorityARP').val('');
            }

            var aot = list.length <= 0 ? '' : list[0].AspectOthers;
            if (aot != '' && aot != null) {
                $('#AspectOther').prop('checked', true);
                $('#AspectOther').change();
                $('#PriorityOT').val(aot);

                $('#AspectOtherInput').val(list.length <= 0 ? '' : list[0].AspectOthersDetail);
            }
            else {
                $('#AspectOther').prop('checked', false);
                $('#PriorityOT').val('');
                $('#AspectOtherInput').val('');
            }

            if ((list.length <= 0 ? '0' : list[0].isEligible) == '0') {
                if ($errorFpol) $errorFpol.remove();
                $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">Data tidak dapat diubah karena sudah diinput lewat dari sehari.</span>').insertBefore('#ChassisCode');
                widget.enable({ value: false, items: ["btnSave"] });
                $('.gl-widget *').prop('disabled', true);
                widget.showToolbars(['browse', 'save', 'cancel', 'expand']);
            }
            else {
                if ($errorFpol) $errorFpol = null;
            }

            widget.enable({ value: false, items: ["btnChassisCode"] });
        });
    });
}

function selectedModel(row) {
    $('#ChassisCode').val(row.ChassisCode);
    $('#ChassisNo').val(row.ChassisNo);
    $('#ErtigaOrWagonR').val(row.ErtigaOrWagonR);

    DisplayPanel($('#ErtigaOrWagonR').val(), $('#StatusKonsumen option:selected').val());
}

function DisplayPanel(model, status, callback) {
    //alert(model + '/' + status);
    if (model != '' && status != '') {
        //$('div[id^="pnl"]').hide();
        //$('#pnlHeader').show();
        //console.log('#pnl' + ((status == 'A') ? 'Individu' : (status == 'B')? 'Fleet' : ''));
        //$('#pnl' + ((status == 'A') ? 'Individu' : (status == 'B') ? 'Fleet' : '')).fadeIn();
        $('#pnlQaUniversal').fadeIn();
        if (status == 'A') {
            $('input[name^="Fleet"]').parents('.span4').hide();
            $('input[id^="Fleet"]').parents('.span4').hide();
            $('select[id^="Fleet"]').parents('.span4').hide();

            $('input[name^="Ind"]').parents('.span4').fadeIn();
            $('input[id^="Ind"]').parents('.span4').fadeIn();
            $('select[id^="Ind"]').parents('.span4').fadeIn();

            $('#ProductSource1').parents('.span4').children('label').text($('#ProductSource1').parents('.span4').children('label').text().replace('4', '2'));
            $('#TestDrive1').parents('.span4').children('label').text($('#TestDrive1').parents('.span4').children('label').text().replace('5', '3'));
            $('#RespondenStatus1').parents('.span4').children('label').text($('#RespondenStatus1').parents('.span4').children('label').text().replace('6', '4'));
            $('#ReplacementReason1').parents('.span4').children('label').text($('#ReplacementReason1').parents('.span4').children('label').text().replace('7', '5'));
            $('#Comparison1').parents('.span4').children('label').text($('#Comparison1').parents('.span4').children('label').text().replace('8', '7'));
            $('#AspectBrand').parents('.span6').children('label').text($('#AspectBrand').parents('.span6').children('label').text().replace('9', '8'));
        }
        else if (status == 'B') {
            $('input[name^="Ind"]').parents('.span4').hide();
            $('input[id^="Ind"]').parents('.span4').hide();
            $('select[id^="Ind"]').parents('.span4').hide();

            $('input[name^="Fleet"]').parents('.span4').fadeIn();
            $('input[id^="Fleet"]').parents('.span4').fadeIn();
            $('select[id^="Fleet"]').parents('.span4').fadeIn();

            $('#ProductSource1').parents('.span4').children('label').text($('#ProductSource1').parents('.span4').children('label').text().replace('2', '4'));
            $('#TestDrive1').parents('.span4').children('label').text($('#TestDrive1').parents('.span4').children('label').text().replace('3', '5'));
            $('#RespondenStatus1').parents('.span4').children('label').text($('#RespondenStatus1').parents('.span4').children('label').text().replace('4', '6'));
            $('#ReplacementReason1').parents('.span4').children('label').text($('#ReplacementReason1').parents('.span4').children('label').text().replace('5', '7'));
            $('#Comparison1').parents('.span4').children('label').text($('#Comparison1').parents('.span4').children('label').text().replace('7', '8'));
            $('#AspectBrand').parents('.span6').children('label').text($('#AspectBrand').parents('.span6').children('label').text().replace('8', '9'));
        }
        else {
            $('#pnlQaUniversal').fadeOut();
        }
        //$('.devider').hide();

        if (model.toUpperCase() == 'WAGONR') {
            $('#ProductSource1').parents('.span4').children('label').text($('#ProductSource1').parents('.span4').children('label').text().replace('Ertiga', 'WagonR'));
            $('#ProductSource1').parents('.span4').children('label').text($('#ProductSource1').parents('.span4').children('label').text().replace('CBU', 'WagonR'));
            $('#RespondenStatus1').parents('.span4').children('label').text($('#RespondenStatus1').parents('.span4').children('label').text().replace('Ertiga', 'WagonR'));
            $('#RespondenStatus1').parents('.span4').children('label').text($('#RespondenStatus1').parents('.span4').children('label').text().replace('CBU', 'WagonR'));
            $('#Comparison1').parents('.span4').children('label').text($('#Comparison1').parents('.span4').children('label').text().replace('Ertiga', 'WagonR'));
            $('#Comparison1').parents('.span4').children('label').text($('#Comparison1').parents('.span4').children('label').text().replace('CBU', 'WagonR'));
            $('#AspectBrand').parents('.span6').children('label').text($('#AspectBrand').parents('.span6').children('label').text().replace('Ertiga', 'WagonR'));
            $('#AspectBrand').parents('.span6').children('label').text($('#AspectBrand').parents('.span6').children('label').text().replace('CBU', 'WagonR'));
            $('#AspectExterior').next('span').text($('#AspectExterior').next('span').text().replace('Ertiga', 'WagonR'));
            $('#AspectExterior').next('span').text($('#AspectExterior').next('span').text().replace('CBU', 'WagonR'));
            $('#AspectInterior').next('span').text($('#AspectInterior').next('span').text().replace('Ertiga', 'WagonR'));
            $('#AspectInterior').next('span').text($('#AspectInterior').next('span').text().replace('CBU', 'WagonR'));
        } else if (model.toUpperCase() == 'ERTIGA') {
            $('#ProductSource1').parents('.span4').children('label').text($('#ProductSource1').parents('.span4').children('label').text().replace('WagonR', 'Ertiga'));
            $('#ProductSource1').parents('.span4').children('label').text($('#ProductSource1').parents('.span4').children('label').text().replace('CBU', 'Ertiga'));
            $('#RespondenStatus1').parents('.span4').children('label').text($('#RespondenStatus1').parents('.span4').children('label').text().replace('WagonR', 'Ertiga'));
            $('#RespondenStatus1').parents('.span4').children('label').text($('#RespondenStatus1').parents('.span4').children('label').text().replace('CBU', 'Ertiga'));
            $('#Comparison1').parents('.span4').children('label').text($('#Comparison1').parents('.span4').children('label').text().replace('WagonR', 'Ertiga'));
            $('#Comparison1').parents('.span4').children('label').text($('#Comparison1').parents('.span4').children('label').text().replace('CBU', 'Ertiga'));
            $('#AspectBrand').parents('.span6').children('label').text($('#AspectBrand').parents('.span6').children('label').text().replace('WagonR', 'Ertiga'));
            $('#AspectBrand').parents('.span6').children('label').text($('#AspectBrand').parents('.span6').children('label').text().replace('CBU', 'Ertiga'));
            $('#AspectExterior').next('span').text($('#AspectExterior').next('span').text().replace('WagonR', 'Ertiga'));
            $('#AspectExterior').next('span').text($('#AspectExterior').next('span').text().replace('CBU', 'Ertiga'));
            $('#AspectInterior').next('span').text($('#AspectInterior').next('span').text().replace('WagonR', 'Ertiga'));
            $('#AspectInterior').next('span').text($('#AspectInterior').next('span').text().replace('CBU', 'Ertiga'));
        } else if (model.toUpperCase() == 'CBU') {
            $('#ProductSource1').parents('.span4').children('label').text($('#ProductSource1').parents('.span4').children('label').text().replace('WagonR', 'CBU'));
            $('#ProductSource1').parents('.span4').children('label').text($('#ProductSource1').parents('.span4').children('label').text().replace('Ertiga', 'CBU'));
            $('#RespondenStatus1').parents('.span4').children('label').text($('#RespondenStatus1').parents('.span4').children('label').text().replace('WagonR', 'CBU'));
            $('#RespondenStatus1').parents('.span4').children('label').text($('#RespondenStatus1').parents('.span4').children('label').text().replace('Ertiga', 'CBU'));
            $('#Comparison1').parents('.span4').children('label').text($('#Comparison1').parents('.span4').children('label').text().replace('WagonR', 'CBU'));
            $('#Comparison1').parents('.span4').children('label').text($('#Comparison1').parents('.span4').children('label').text().replace('Ertiga', 'CBU'));
            $('#AspectBrand').parents('.span6').children('label').text($('#AspectBrand').parents('.span6').children('label').text().replace('WagonR', 'CBU'));
            $('#AspectBrand').parents('.span6').children('label').text($('#AspectBrand').parents('.span6').children('label').text().replace('Ertiga', 'CBU'));
            $('#AspectExterior').next('span').text($('#AspectExterior').next('span').text().replace('WagonR', 'CBU'));
            $('#AspectExterior').next('span').text($('#AspectExterior').next('span').text().replace('Ertiga', 'CBU'));
            $('#AspectInterior').next('span').text($('#AspectInterior').next('span').text().replace('WagonR', 'CBU'));
            $('#AspectInterior').next('span').text($('#AspectInterior').next('span').text().replace('Ertiga', 'CBU'));
        }

        widget.post("wh.api/combo/Qa2Filter", { "ErtigaOrWagonR": $('#ErtigaOrWagonR').val(), "StatusKonsumen": $('#StatusKonsumen option:selected').val() }, function (result) {
            widget.bind({
                name: 'IndJenisKelamin1',
                data: result[0]
            });

            widget.bind({
                name: 'IndOccupation1',
                data: result[1],
                onChange: function () {
                    var selected = $('#IndOccupation1 option:selected').val();
                    if (selected == 'L') {
                        $('#OccupationOther1').parents('.span4').fadeIn();
                    }
                    else {
                        $('#OccupationOther1').parents('.span4').fadeOut();
                        $('#OccupationOther1').val('');
                    }
                }
            });

            widget.bind({
                name: 'ProductSource1',
                data: result[2],
                onChange: function () {
                    var selected = $('#ProductSource1 option:selected').val();
                    if (selected != 'A' && selected != '') {
                        $('#ProductSourceDetail1').parents('.span4').fadeIn();
                    }
                    else {
                        $('#ProductSourceDetail1').parents('.span4').fadeOut();
                        $('#ProductSourceDetail1').val('');
                    }
                }
            });

            widget.bind({
                name: 'TestDrive1',
                data: result[3]
            });

            widget.bind({
                name: 'RespondenStatus1',
                data: result[4],
                onChange: function () {
                    var selected = $('#RespondenStatus1 option:selected').val();
                    if (selected == 'A' && $('#StatusKonsumen option:selected').val() != 'B') {
                        $('#FirstTime1').parents('.span4').fadeIn();
                        $('#ReplacementOrAdditionalMerk1').parents('.span6').fadeOut();
                        $('#TotalCar1').parents('.span4').fadeOut();

                        $('#ReplacementMerk1').parents('.span4').fadeOut();
                        $('#ReplacementMerkOther1').parents('.span4').fadeOut();
                        $('#ReplacementType1').parents('.span4').fadeOut();
                        $('#ReplacementYear1').parents('.span4').fadeOut();

                        $('#ReplacementReason1').parents('.span4').fadeOut();
                        $('#ReplacementReasonOther1').parents('.span4').fadeOut();
                    }
                    else if (selected == 'A' && $('#StatusKonsumen option:selected').val() == 'B') {
                        $('#FirstTime1').parents('.span4').fadeOut();
                        $('#ReplacementOrAdditionalMerk1').parents('.span6').fadeOut();
                        $('#TotalCar1').parents('.span4').fadeOut();

                        $('#ReplacementMerk1').parents('.span4').fadeOut();
                        $('#ReplacementMerkOther1').parents('.span4').fadeOut();
                        $('#ReplacementType1').parents('.span4').fadeOut();
                        $('#ReplacementYear1').parents('.span4').fadeOut();

                        $('#ReplacementReason1').parents('.span4').fadeOut();
                        $('#ReplacementReasonOther1').parents('.span4').fadeOut();
                    }
                    else if (selected == '') {
                        $('#FirstTime1').parents('.span4').fadeOut();
                        $('#ReplacementOrAdditionalMerk1').parents('.span6').fadeOut();
                        $('#TotalCar1').parents('.span4').fadeOut();

                        $('#ReplacementMerk1').parents('.span4').fadeOut();
                        $('#ReplacementMerkOther1').parents('.span4').fadeOut();
                        $('#ReplacementType1').parents('.span4').fadeOut();
                        $('#ReplacementYear1').parents('.span4').fadeOut();

                        $('#ReplacementReason1').parents('.span4').fadeOut();
                        $('#ReplacementReasonOther1').parents('.span4').fadeOut();
                    }
                    else if (selected == 'B') {
                        $('#FirstTime1').parents('.span4').fadeOut();
                        $('#ReplacementOrAdditionalMerk1').parents('.span6').fadeOut();
                        $('#TotalCar1').parents('.span4').fadeOut();

                        $('#ReplacementMerk1').parents('.span4').fadeIn();
                        $('#ReplacementMerk1').change();
                        //$('#ReplacementMerkOther1').parents('.span4').fadeIn();
                        $('#ReplacementType1').parents('.span4').fadeIn();
                        $('#ReplacementYear1').parents('.span4').fadeIn();

                        $('#ReplacementReason1').parents('.span4').fadeIn();
                        //$('#ReplacementReasonOther1').parents('.span4').fadeIn();
                    }
                    else if (selected == 'C') {
                        $('#FirstTime1').parents('.span4').fadeOut();
                        $('#ReplacementOrAdditionalMerk1').parents('.span6').fadeIn();
                        $('#TotalCar1').parents('.span4').fadeIn();

                        $('#ReplacementMerk1').parents('.span4').fadeOut();
                        $('#ReplacementMerkOther1').parents('.span4').fadeOut();
                        $('#ReplacementType1').parents('.span4').fadeOut();
                        $('#ReplacementYear1').parents('.span4').fadeOut();

                        $('#ReplacementReason1').parents('.span4').fadeOut();
                        $('#ReplacementReasonOther1').parents('.span4').fadeOut();
                    }
                }
            });

            widget.bind({
                name: 'FirstTime1',
                data: result[5]
            });

            widget.bind({
                name: 'ReplacementReason1',
                data: result[6],
                onChange: function () {
                    var selected = $('#ReplacementReason1 option:selected').val();
                    if (selected == 'F') {
                        $('#ReplacementReasonOther1').parents('.span4').fadeIn();
                    }
                    else {
                        $('#ReplacementReasonOther1').parents('.span4').fadeOut();
                        $('#ReplacementReasonOther1').val('');
                    }
                }
            });

            widget.bind({
                name: 'Comparison1',
                data: result[7],
                onChange: function () {
                    var selected = $('#Comparison1 option:selected').val();
                    if (selected == 'Z' && $('#ErtigaOrWagonR').val().toUpperCase() == 'ERTIGA') {
                        $('#ComparisonOther1').parents('.span4').fadeIn();
                    }
                    else if (selected == 'ZD' && $('#ErtigaOrWagonR').val().toUpperCase() == 'WAGONR') {
                        $('#ComparisonOther1').parents('.span4').fadeIn();
                    }
                    else if (selected == 'T' && $('#ErtigaOrWagonR').val().toUpperCase() == 'CBU') {
                        $('#ComparisonOther1').parents('.span4').fadeIn();
                    }
                    else {
                        $('#ComparisonOther1').parents('.span4').fadeOut();
                        $('#ComparisonOther1').val('');
                    }
                }
            });

            //widget.bind({
            //    name: 'Aspect1',
            //    data: result[8],
            //    onChange: function () {
            //        var selected = $('#Aspect1 option:selected').val();
            //        if (selected == 'I') {
            //            $('#AspectOther1').parents('.span4').fadeIn();
            //        }
            //        else {
            //            $('#AspectOther1').parents('.span4').fadeOut();
            //        }
            //    }
            //});

            widget.bind({
                name: 'FleetPurpose',
                data: result[9],
                onChange: function () {
                    var selected = $('#FleetPurpose option:selected').val();
                    if (selected == 'D') {
                        $('#PurposeOther').parents('.span4').fadeIn();
                    }
                    else {
                        $('#PurposeOther').parents('.span4').fadeOut();
                        $('#PurposeOther').val('');
                    }
                }
            });

            widget.bind({
                name: 'FleetPeriod',
                data: result[10]
            });

            widget.bind({
                name: 'FleetRenovation',
                data: result[11]
            });

            widget.bind({
                name: 'FleetPembelianAtasNama',
                data: result[12]
            });

            widget.bind({
                name: 'ReplacementOrAdditionalMerk1',
                data: result[13],
                onChange: function () {
                    var selected = $('#ReplacementOrAdditionalMerk1 option:selected').val();
                    if (selected == 'K') {
                        $('#ReplacementOrAdditionalMerkOther1').parents('.span4').fadeIn();
                    }
                    else {
                        $('#ReplacementOrAdditionalMerkOther1').parents('.span4').fadeOut();
                        $('#ReplacementOrAdditionalMerkOther1').val('');
                    }
                }
            });

            widget.bind({
                name: 'ReplacementMerk1',
                data: result[14],
                onChange: function () {
                    var selected = $('#ReplacementMerk1 option:selected').val();
                    if (selected == 'K') {
                        $('#ReplacementMerkOther1').parents('.span4').fadeIn();
                    }
                    else {
                        $('#ReplacementMerkOther1').parents('.span4').fadeOut();
                        $('#ReplacementMerkOther1').val('');
                    }
                }
            });

            if (typeof callback == 'undefined')
                getCustomerDetailsByChassis();
            else
                callback();
        });
    }
}

var msgErr = "";
var $errorFpol = null;
function getCustomerDetailsByChassis() {
    widget.post("wh.api/combo/autoFillByChassis2", { ChassisCode: $('#ChassisCode').val(), ChassisNo: $('#ChassisNo').val() }, function (result) {
        var list = [];
        list = Enumerable.From(result[0]).ToArray();

        if (list.length > 0) {
            if (list[0].Result == '2') msgErr = "Nomor rangka tidak ditemukan.";
            else if (list[0].Result == '3') msgErr = "Sales Model tidak masuk dalam kuesioner.";
            else if (list[0].Result == '4') msgErr = "Transaksi sales tidak ditemukan.";
        }

        //$('#pnlProfile *').prop('disabled', false);
        $('#pnl' + $('#ErtigaOrWagonR').val().toUpperCase() + $('#StatusKonsumen option:selected').val()).prop('disabled', false);

        //widget.enable({ value: false, items: ["Area", "SalesModelCode", "SalesModelReport", "CompanyCode", "CompanyName", "BranchCode", "BranchName"] });

        //$('#MerkPsgrCar1').change();
        //$('#MerkPsgrCar2').change();
        //$('#MerkPsgrCar3').change();
        //$('#MerkPsgrCar4').change();
        //$('#MerkPsgrCar5').change();

        if (list.length > 0 && list[0].Result == '1') {
            //alert(new Date(parseInt(list[0].BirthDate.replace("/Date(", "").replace(")/", ""))));
            //$('#ChassisCode').val(list[0].ChassisCode);
            //$('#ChassisNo').val(list[0].ChassisNo);
            $('#SalesModelCode').val(typeof list[0].SalesModelCode != 'undefined' ? list[0].SalesModelCode : '');
            $('#SalesModelReport').val(typeof list[0].SalesModelReport != 'undefined' ? list[0].SalesModelReport : '');
            $('#ColourCode').val(typeof list[0].ColourCode != 'undefined' ? list[0].ColourCode : '');
            $('#ColourDesc').val(typeof list[0].ColourDesc != 'undefined' ? list[0].ColourDesc : '');
            $('#NamaKonsumen1').val(typeof list[0].CustomerName != 'undefined' ? list[0].CustomerName == '' ? '' : list[0].CustomerName : '');
            $('#IndUmurKonsumen1').val(typeof list[0].Age != 'undefined' ? list[0].Age == '' ? '' : list[0].Age : '');
            $('#IndJenisKelamin1').select2('val', typeof list[0].Gender != 'undefined' ? list[0].Gender == '' ? '' : list[0].Gender : '');
            //$('#ContactNo').val(typeof list[0].ContactNo != 'undefined' ? list[0].ContactNo == '' ? '' : list[0].ContactNo : '');
            $('#CashOrCredit1').select2('val', typeof list[0].isLeasing != 'undefined' ? (list[0].isLeasing == true ? '1' : '0') : '').change();
            $('#EmployeeName').val(typeof list[0].EmployeeName != 'undefined' ? list[0].EmployeeName == '' ? '' : list[0].EmployeeName : '');
            var TglOp = typeof list[0].BirthDate != 'undefined' ? list[0].BirthDate == '' ? '' : new Date(parseInt(list[0].BirthDate.replace("/Date(", "").replace(")/", ""))) : '';

            if (TglOp != '')
                $("input[name='IndBirthDate1']").val([TglOp.getDate(), TglOp.getMonth(), TglOp.getFullYear()].join('-'));
            else
                $("input[name='IndBirthDate1']").val('');

            $('#CityCode').val(typeof list[0].CityCode != 'undefined' ? list[0].CityCode == '' ? '' : list[0].CityCode : '');
            $('#Kota1').val(typeof list[0].CityDesc != 'undefined' ? list[0].CityDesc == '' ? '' : list[0].CityDesc : '');
            //$('#BirthDate1').val(typeof list[0].BirthDate != 'undefined' ? list[0].BirthDate == '' ? '' : list[0].BirthDate : '');

            if (list[0].isLeasing == true) {
                $('#CreditInstalment1').prop("required", "required");
                $('#CreditInstalment1').val(list[0].Installment);
            }
            else if (list[0].isLeasing == false) {
                widget.enable({ value: false, items: ["CreditInstalment1"] });
                widget.setValue({ name: "CreditInstalment1", value: "" });
            }

            widget.enable({ value: true, items: ["btnSave"] });
            if ($errorFpol)
                $errorFpol.remove();
            $errorFpol = null;
        }
        else if (list.length > 0 && list[0].Result == '5') { //tr existing
            var param = { ChassisCode: $('#ChassisCode').val(), ChassisNo: $('#ChassisNo').val() };
            //selectBrowse(param);
        }
            //else if (list.length > 0 && list[0].Result == '1') {
            //    //$('#ChassisCode').val(list[0].ChassisCode);
            //    //$('#ChassisNo').val(list[0].ChassisNo);

            //    if ($errorFpol)
            //        $errorFpol.remove();
            //    $errorFpol = null;

            //    widget.enable({ value: true, items: ["btnSave"] });
            //}
        else {
            //$('#ChassisCode').val('');
            //$('#ChassisNo').val('');
            $('#NamaKonsumen1').val('');
            $('#IndUmurKonsumen1').val('');
            $('#IndJenisKelamin1').select2('val', '');
            $('#IndBirthDate1').val('');
            $('#Kota1').val('');
            $('#CityCode').val('');
            $('#CashOrCredit1').select2('val', '');
            $('#CreditInstalment1').val('');

            widget.enable({ value: false, items: ["btnSave"] });
            if ($errorFpol) $errorFpol.remove();
            $errorFpol = $('<span style="margin: -10px 0 18px 0;font-size: 14px;font-style: italic;color:#c60f13;">' + msgErr + '</span>').insertBefore('#ChassisCode');

            $('#pnl' + $('#ErtigaOrWagonR').val().toUpperCase() + $('#StatusKonsumen option:selected').val()).prop('disabled', false);

            //widget.enable({ value: true, items: ["ChassisCode", "ChassisNo", "btnChassisCode"] });
        }
    });
}

function editReplAddtCar(obj) {
    var opt = obj.parents('tr').children('td:first-child').text().substr(0, 1);
    $('#ReplacementOrAdditionalMerk1').val(opt);

    if (opt == 'E') {
        $('#ReplacementOrAdditionalMerkOther1').val(obj.parents('tr').children('td:first-child').text().replace('E.').trim());
    }
    else {
        $('#ReplacementOrAdditionalMerkOther1').val('');
        $('#ReplacementOrAdditionalMerkOther1').parents('div.span4').hide();
    }

    $('#ReplacementOrAdditionalType1').val(obj.parents('tr').children('td:first-child').next('td').text());
    $('#ReplacementOrAdditionalYear1').val(obj.parents('tr').children('td:first-child').next('td').next('td').text());

    $('#btnAdd').hide();
    $('#btnEditSv').show();
    $('#btnEditCn').show();
    $('#hdnEditID').val($(obj).parents('tr').attr('id'));
}

function deleteReplAddtCar(obj) {
    if (confirm("apakah anda yakin mau menghapus data ini?")) {
        obj.parents('tr').remove();
    }
}

function disableBoxAdd() {
    $('#btnAdd').prop('disabled', true);
    $('#ReplacementOrAdditionalMerk1').prop('disabled', true);
    $('#ReplacementOrAdditionalType1').prop('disabled', true);
    $('#ReplacementOrAdditionalYear1').prop('disabled', true);
}

function enableBoxAdd() {
    $('#btnAdd').prop('disabled', false);
    $('#ReplacementOrAdditionalMerk1').prop('disabled', false);
    $('#ReplacementOrAdditionalType1').prop('disabled', false);
    $('#ReplacementOrAdditionalYear1').prop('disabled', false);
}

widget.render(function () {

    $(".datepicker").removeClass('hasDatepicker').removeAttr('id').datepicker({
        dateFormat: "MM dd, yy",
        showOtherMonths: true,
        selectOtherMonths: true,
        changeMonth: true,
        changeYear: true,
        yearRange: "-100:+10",
    });

    $('#pnlQaUniversal').hide();
    $('.devider').hide();

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
            url: 'wh.api/lookupgrid/LookUpQa2CarModel',
            sort: [{ field: 'ChassisCode', dir: 'desc' }],
            fields: [
                { name: 'ChassisCode', text: 'Chassis Code', width: 200 },
                { name: 'ChassisNo', text: 'Chassis No', width: 200 },
                { name: 'SalesModelCode', text: 'SalesModel Code', width: 200 },
                { name: 'ErtigaOrWagonR', text: 'Event', width: 200 }
            ],
            dblclick: function (row) {
                selectedModel(row);
            },
            onclick: function (row) {
                selectedModel(row);
            }
        });
    });

    //binding part
    widget.bind({
        name: 'StatusKonsumen',
        data: [{ text: "Individu", value: "A" }, { text: "Fleet/Perusahaan", value: "B" }],
        onChange: function () {
            var selected = $('#StatusKonsumen option:selected').val();
            DisplayPanel($('#ErtigaOrWagonR').val(), selected);
        }
    });

    widget.bind({
        name: 'CashOrCredit1',
        data: [{ text: "Cash", value: "0" }, { text: "Credit", value: "1" }],
        onChange: function () {
            var selected = $('#CashOrCredit1 option:selected').val();
            if (selected == '0') {
                widget.enable({ value: false, items: ["CreditInstalment1"] });
                widget.setValue({ name: "CreditInstalment1", value: "" });
            }
            else {
                widget.enable({ value: true, items: ["CreditInstalment1"] });
            }
        }
    });

    $('#ChassisCode, #ChassisNo').keyup(function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) {
            getCustomerDetailsByChassis();
        }
    });


    $('#btnAdd').on("click", function () {
        var id = "";
        if ($('#pcbody').children('tr').length > 0) {
            id = 'pc' + (parseInt($('#pcbody').children('tr:last-child').attr('id').replace('pc', '')) + 1);
        }
        else {
            id = 'pc1';
        }

        if ($('#ReplacementOrAdditionalMerk1 option:selected').val() != '' && $('#ReplacementOrAdditionalType1').val() != '' && $('#ReplacementOrAdditionalYear1').val() != '') {

            if ($('#ReplacementOrAdditionalMerk1 option:selected').val() == 'K' && $('#ReplacementOrAdditionalMerkOther1').val() == '')
                return;

            var merk = $('#ReplacementOrAdditionalMerk1 option:selected').val() == 'K' ? 'K. ' + $('#ReplacementOrAdditionalMerkOther1').val() : $('#ReplacementOrAdditionalMerk1 option:selected').text();
            var html = "<tr id=\"" + id + "\"><td>" + merk + "</td><td>" + $('#ReplacementOrAdditionalType1').val() + "</td>" +
                "<td>" + $('#ReplacementOrAdditionalYear1').val() + "</td>" +
                "<td><a href=\"#\" class=\"icon fa fa-edit\" onclick=\"editReplAddtCar($(this)); return false;\"></a> | " +
                "<a href=\"#\" class=\"icon fa fa-trash-o\" onclick=\"deleteReplAddtCar($(this)); return false;\"></a></td></tr>";
            $('#pcbody').append(html);
        }
        $('#ReplacementOrAdditionalMerk1').val('');
        $('#ReplacementOrAdditionalMerkOther1').val('');
        $('#ReplacementOrAdditionalMerkOther1').parents('div.span4').hide();
        $('#ReplacementOrAdditionalType1').val('');
        $('#ReplacementOrAdditionalYear1').val('');
    });

    $('#btnEditSv').on("click", function () {
        var id = $('#hdnEditID').val();
        if ($('#ReplacementOrAdditionalMerk1 option:selected').val() != '' && $('#ReplacementOrAdditionalType1').val() != '' && $('#ReplacementOrAdditionalYear1').val() != '') {
            var merk = $('#ReplacementOrAdditionalMerk1 option:selected').val() == 'K' ? 'K. ' + $('#ReplacementOrAdditionalMerkOther1').val() : $('#ReplacementOrAdditionalMerk1 option:selected').text();
            $('#' + id).children('td:first-child').text(merk);
            $('#' + id).children('td:first-child').next('td').text($('#ReplacementOrAdditionalType1').val());

            $('#btnEditSv').hide(); $('#btnEditCn').hide(); $('#btnAdd').show();
            $('#ReplacementOrAdditionalMerk1').val('');
            $('#ReplacementOrAdditionalType1').val('');
            $('#ReplacementOrAdditionalYear1').val('');
            $('#ReplacementOrAdditionalMerkOther1').val('');
            $('#ReplacementOrAdditionalMerkOther1').parents('div.span4').hide();
        }
    });

    $('#btnEditCn').on("click", function () {
        $('#btnEditSv').hide(); $('#btnEditCn').hide(); $('#btnAdd').show();
        $('#ReplacementOrAdditionalMerk1').val('');
        $('#ReplacementOrAdditionalType1').val('');
        $('#ReplacementOrAdditionalYear1').val('');
        $('#ReplacementOrAdditionalMerkOther1').val('');
        $('#ReplacementOrAdditionalMerkOther1').parents('div.span4').hide();
    });

    $('input[name="IndBirthDate1"]').on("change", function () {
        var dob = new Date($(this).val());
        var ageDimfMs = Date.now() - dob.getTime();

        var ageDate = new Date(ageDimfMs);

        var age = Math.abs(ageDate.getUTCFullYear() - 1970);

        $('#IndUmurKonsumen1').val(age);
    });

    widget.checked("AspectBrand", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["PriorityAB"] });

        }
        else {
            widget.enable({ value: false, items: ["PriorityAB"] });
            widget.setValue({ name: "PriorityAB", value: "" });
        }
    });

    widget.checked("AspectPrice", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["PriorityAP"] });

        }
        else {
            widget.enable({ value: false, items: ["PriorityAP"] });
            widget.setValue({ name: "PriorityAP", value: "" });
        }
    });

    widget.checked("AspectEngine", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["PriorityAE"] });

        }
        else {
            widget.enable({ value: false, items: ["PriorityAE"] });
            widget.setValue({ name: "PriorityAE", value: "" });
        }
    });

    widget.checked("AspectExterior", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["PriorityAEX"] });

        }
        else {
            widget.enable({ value: false, items: ["PriorityAEX"] });
            widget.setValue({ name: "PriorityAEX", value: "" });
        }
    });

    widget.checked("AspectInterior", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["PriorityAI"] });

        }
        else {
            widget.enable({ value: false, items: ["PriorityAI"] });
            widget.setValue({ name: "PriorityAI", value: "" });
        }
    });

    widget.checked("AspectAfterSales", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["PriorityAAS"] });

        }
        else {
            widget.enable({ value: false, items: ["PriorityAAS"] });
            widget.setValue({ name: "PriorityAAS", value: "" });
        }
    });

    widget.checked("AspectOutlet", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["PriorityAO"] });

        }
        else {
            widget.enable({ value: false, items: ["PriorityAO"] });
            widget.setValue({ name: "PriorityAO", value: "" });
        }
    });

    widget.checked("AspectResalePrice", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["PriorityARP"] });

        }
        else {
            widget.enable({ value: false, items: ["PriorityARP"] });
            widget.setValue({ name: "PriorityARP", value: "" });
        }
    });

    widget.checked("AspectOther", function (result) {
        if (result) {
            widget.enable({ value: true, items: ["PriorityOT", "AspectOtherInput"] });

        }
        else {
            widget.enable({ value: false, items: ["PriorityOT", "AspectOtherInput"] });
            widget.setValue({ name: "PriorityOT", value: "" });
            widget.setValue({ name: "AspectOtherInput", value: "" });
        }
    });


    $('#IndUmurKonsumen1, #CreditInstalment1, #ReplacementYear1, #TotalCar1, #ReplacementOrAdditionalYear1').keypress(function (ev) {
        ev = (ev) ? ev : window.event;
        var charcode = (ev.which) ? ev.which : ev.keyCode;

        if (charcode > 31 && (charcode < 48 || charcode > 57)) {
            return false;
        }
        return true;
    });

    $('input[id^="Priority"]').keypress(function (ev) {
        ev = (ev) ? ev : window.event;
        var charcode = (ev.which) ? ev.which : ev.keyCode;

        if (charcode > 31 && (charcode < 48 || charcode > 57)) {
            return false;
        }

        return true;
    });

    $('input[id^="Priority"]').keyup(function (ev) {
        var max = $('input:checkbox[id^="Aspect"]:checked').length;

        if ($(this).val() > max) { sdms.info({ type: "warning", text: "Priority tidak valid" }); $(this).val(''); return false; }

        var objval = $(this).val();

        var sameCounter = 0;

        $('input[id^="Priority"]').each(function () {
            if ($(this).val() != '') {
                if (objval == $(this).val())
                    sameCounter++;
            }
        });

        if (sameCounter > 1) {
            sdms.info({ type: "warning", text: "Priority tidak boleh sama" }); $(this).val(''); return false;
        }
    });
})