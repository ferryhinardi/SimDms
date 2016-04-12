var widget = new SimDms.Widget({
    title: "SPK Exhibition",
    xtype: "panels",
    toolbars: [
        { action: "add", text: "New", icon: "fa fa-file" },
        { action: "browse", text: "Browse", icon: "fa fa-search" },
        { action: "save", text: "Save", icon: "fa fa-save" },
    ],
    panels: [
            {
                title: "Data Salesman",
                name: "pnlDataSalesman",
                items: [
                    { name: "BranchName", text: "Outlet", cls: "span8 hide", readonly: true },
                    { name: "InquiryNumber", text: "Nomor Inquiry", cls: "span4", readonly: true },
                    { name: "InquiryDate", text: "Tanggal Inquiry", type: "date", cls: "span4", readonly: true },
                    { name: "SPKNo", text: "Nomor SPK", type: "text", cls: "span4", maxlength: 30, required: true },
                    { name: "SPKDate", text: "Tanggal SPK", type: "datepicker", cls: "span4", required: true },
                    { type: "span" },
                    { name: "StatusProspek", text: "Status Inquiry", type: "select", cls: "span4" },
                    { name: "PerolehanData", text: "Perolehan Data", type: "select", cls: "span4", required: true, readonly: true },
                    { name: "CompanyCode", type: "select", cls: "span4", text: "Dealer", required: true },
                    { name: "BranchCode", type: "select", cls: "span4", text: "Outlet", opt_text: " ", readonly: true },
                    { type: "span" },
                    { name: "NikSales", text: "Salesman", cls: "span4", type: "select", required: true },
                    { name: "Grade", text: "Salesman Grade", cls: "span4", type: "select", opt_text: " " },
                    { type: "span" },
                    { name: "ShiftCode", text: "Shift", cls: "span4", type: "select", required: true },
                    { name: "NikSH", text: "Sales Head", cls: "span4", type: "select", opt_text: " " },
                    { name: "DealerCode", type: "hidden" }
                ]
            },
            {
                title: "Data Prospek",
                name: "pnlDataProspek",
                items: [
                    {
                        text: "Customer", type: "controls", items: [
                            { name: "NamaProspek", text: "Nama Customer", cls: "span6", maxlength: 30, required: true },
                            { name: "TelpRumah", text: "Telepon", cls: "span2", maxlength: 15 }
                        ]
                    },
                    { name: "AlamatProspek", text: "Alamat", type: "textarea", maxlength: 200, required: true },
                    {
                        text: "Kota", type: "controls", items: [
                            { name: "CityID", text: "Kode Kota", type: "popup", cls: "span2", maxlength: 15, action: "lookUpCity", readonly: true },
                            { name: "CityName", text: "Name Kota", cls: "span6" }
                        ]
                    },
                    { type: "divider" },
                    {
                        text: "Perusahaan", type: "controls", items: [
                            { name: "NamaPerusahaan", text: "Nama Perusahaan", cls: "span6", maxlength: 50 },
                            { name: "Jabatan", text: "Jabatan", cls: "span2", maxlength: 30 },
                        ]
                    },
                    { name: "AlamatPerusahaan", text: "Alamat Perusahaan", type: "textarea", maxlength: 200 },
                    { name: "Faximile", text: "Faximile", cls: "span4", maxlength: 15 },
                    { name: "Email", text: "Email", cls: "span4", maxlength: 50 }
                ]
            },
            {
                title: "Kendaraan yang dibeli",
                name: "pnlKendaraan",
                items: [
                    { name: "TipeKendaraan", text: "Type", type: "select", cls: "span4  ", required: true },
                    { name: "Variant", text: "Variant", type: "select", cls: "span4", required: true },
                    { name: "Transmisi", text: "Transmisi", type: "select", cls: "span4", required: true },
                    { name: "ColourCode", text: "Warna", type: "select", cls: "span4", required: true },
                    { name: "QuantityInquiry", text: "Quantity", cls: "span4", readonly: true },
                    { name: "CaraPembayaran", text: "Pembayaran", type: "select", cls: "span4", required: true },
                    {
                        text: "Nama Leasing", type: "controls", items: [
                            { name: "Leasing", type: "select", cls: "span6", required: true },
                        ]
                    },
                    {
                        text: "DP (%) / Tenor", type: "controls", items: [
                            { name: "DownPayment", type: "select", cls: "span3", required: true },
                            { name: "Tenor", type: "select", cls: "span3", required: true },
                        ]
                    },
                    { name: "Hadiah", text: "Hadiah", type: "select", cls: "span4", required: true },
                    { name: "TestDrive", text: "Test Drive", cls: "span4", type: "select" },
                ]
            },
            {
                name: "pnlStatus",
                title: "Status Information",
                items: [
                    { name: "LastProgress", text: "Status Terakhir", cls: "span4", required: true },
                    { name: "LastUpdateStatus", text: "Updated Date", cls: "span4", type: "date", readonly: true }
                ]
            }
    ],

    onInit: function (wgt) {
        wgt.call('initial', wgt);
    },

    initial: function (wgt) {
        widget = widget || wgt;
        widget.call('add');

        var comboCompanyCode = $("[name='CompanyCode']");
        comboCompanyCode.off();
        comboCompanyCode.on("change", function () {
            $(this).select2('val', $(this).val());
            var id = $(this).val();
            if (id != "") {
                $("[name=NikSales]").select2('val', '');
                $("[name=Grade]").select2('val', '');
                $("[name=NikSH]").select2('val', '');
                widget.enable({ value: true, items: ["NikSales"] });
                widget.select({
                    selector: "[name=BranchCode]", url: "wh.api/combo/outletabbre",
                    params: {
                        id: $("[name=CompanyCode]").val()
                    },
                    optionalText: " "
                },
                    function (res) {
                        if (res.length == 1) {
                            $("#BranchCode").select2('val', res[0].value);
                        }
                        else {
                            $("#BranchCode").select2('val', "");
                        }
                    }
                );
                widget.select({ selector: "select[name=NikSales]", url: "wh.api/combo/sales", params: { Company: $("[name=CompanyCode]").val() } },
                    function (res) {
                        if (res.length == 1) {
                            $("#NickSales").select2('val', res[0].value);
                        }
                        else {
                            $("#NickSales").select2('val', "");
                        }
                    }
                );
                widget.select({ selector: "select[name=NikSH]", url: "wh.api/combo/saleshead", params: { Company: $("[name=CompanyCode]").val() }, optionalText: " " });
            } else {
                $("[name=BranchCode]").select2('val', '');
                $("[name=NikSales]").select2('val', '');
                $("[name=Grade]").select2('val', '');
                $("[name=NikSH]").select2('val', '');
                widget.enable({ value: false, items: ["NikSales"] });
            }
        });

        widget.select({ selector: "select[name=TipeKendaraan]", url: "wh.api/spkexhibition/cartypes" });
        widget.call('onChangeCarType');
        widget.call('onChangeVariant');
        widget.call('onChangeTransmission');

        //widget.enable({ value: false, items: ["BranchCode", "NikSC", "Grade", "NikSH", "PerolehanData", "CaraPembayaran", "LastProgress"] });
        widget.enable({ value: false, items: ["BranchCode", "NikSC", "Grade", "NikSH", "PerolehanData", "LastProgress", "NikSales"] });

        var Pembayaran = $("[name='CaraPembayaran']");
        Pembayaran.off();
        Pembayaran.on("change", function () {
            $(this).select2('val', $(this).val());
            var id = $(this).val();
            if (id == "20") { widget.showControl(["Leasing", "DownPayment"]); }
            else { widget.hideControl(["Leasing", "DownPayment"]); }
        });

        var cmbNikSales = $("[name='NikSales']");
        cmbNikSales.off();
        cmbNikSales.on("change", function () {
            $(this).select2('val', $(this).val());
            var id = $(this).val();
            var company = $("#CompanyCode").val();
            if (id == "") {
                $("#Grade").select2('val', "");
                $("#NikSH").select2('val', "");
            }
            else {
                widget.post("wh.api/spkexhibition/getoutletbysales",
                { Company: company, Employee: id }, function (outlet) {
                    $("#BranchCode").select2('val', outlet);
                });

                widget.post("wh.api/spkexhibition/getgrade", { Company: company, id: id }, function (grade) {
                    $("#Grade").select2('val', grade);
                });

                widget.post("wh.api/spkexhibition/getsaleshead/?company=" + company + "&id=" + $("#NikSales").val(), function (EmployeeID) {
                    if (EmployeeID != "") {
                        $("#NikSH").select2('val', EmployeeID);
                    }
                });
            }
        });

        //$("#CaraPembayaran").on("change", function () { widget.call('validateControl') });

        var cmbShiftCode = $("[name='ShiftCode']");
        cmbShiftCode.off();
        cmbShiftCode.on("change", function () {
            $(this).select2('val', $(this).val());
        });

        var cmbColour = $("[name='ColourCode']");
        cmbColour.off();
        cmbColour.on("change", function () {
            $(this).select2('val', $(this).val());
        });

        var cmbHadiah = $("[name='Hadiah']");
        cmbHadiah.off();
        cmbHadiah.on("change", function () {
            $(this).select2('val', $(this).val());
        });
    },

    add: function () {
        widget.clear();

        widget.post("wh.api/spkexhibition/default", function (result) {
            if (result.success) {
                widget.default = result.data;
                widget.populate(widget.default);
            }
        });


        $("#DealerCode").val("0000000");
        widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/dealerabbre" });
        widget.select({ selector: "select[name=StatusProspek]", url: "wh.api/combo/lookups/pmsp", params: { CompanyCode: $("[name=DealerCode]").val() } },
        function () {
            $("#StatusProspek").select2('val', '10');
        });
        widget.select({ selector: "select[name=PerolehanData]", url: "wh.api/combo/exhibitionvalue", params: { CompanyCode: $("[name=DealerCode]").val() } },
        function () {
            $("#PerolehanData").select2('val', 'Exhibition');
        });
        widget.select({ selector: "select[name=Grade]", url: "wh.api/combo/lookups/itsg", optionalText: " " });

        widget.select({ selector: "select[name=ShiftCode]", url: "wh.api/combo/lookups/shift", params: { CompanyCode: $("[name=DealerCode]").val() } });

        widget.select({ selector: "select[name=CaraPembayaran]", url: "wh.api/combo/lookups/pmby", params: { CompanyCode: $("[name=DealerCode]").val()} },
            function (res) {
                $("#CaraPembayaran").select2('val', '10');
                widget.call('validateControl', '10');
            }
        );
        widget.select({ selector: "select[name=Leasing]", url: "wh.api/combo/lookups/lsng", params: { CompanyCode: $("[name=DealerCode]").val() }});
        widget.select({ selector: "select[name=DownPayment]", url: "wh.api/combo/lookups/dwpm", params: { CompanyCode: $("[name=DealerCode]").val() }, optionalText: "-- SELECT DP --" });
        widget.select({ selector: "select[name=Tenor]", url: "wh.api/combo/lookups/tenor", params: { CompanyCode: $("[name=DealerCode]").val() }, optionalText: "-- SELECT TENOR --" });
        widget.select({ selector: "select[name=Hadiah]", url: "wh.api/combo/gift" + "/?CompanyCode=" + $("[name=DealerCode]").val() },
            function (res) {
                if (res.length == 1) {
                    $("#Hadiah").select2('val', res[0].value);
                }
                else {
                    $("#Hadiah").select2('val', '');
                }
            }
        );
        widget.select({ selector: "select[name=TestDrive]", url: "wh.api/combo/lookups/pmop", params: { CompanyCode: $("[name=DealerCode]").val() }},
            function (res) {
                if (res.length == 1) {
                    $("#TestDrive").select2('val', res[0].value);
                }
                else {
                    $("#TestDrive").select2('val', '');
                }
            }
        );

        $("#CompanyCode").select2('val', '');
        $("#BranchCode").select2('val', '');
        $("#NikSales").select2('val', '');
        $("#Grade").select2('val', '');
        $("#NikSH").select2('val', '');
        $("#ShiftCode").select2('val', '');
        $("#TipeKendaraan").select2('val', '');
        $("#Variant").select2('val', '');
        $("#Transmisi").select2('val', '');
        $("#ColourCode").select2('val', '');
        $("#Leasing").select2('val', '');
        $("#DownPayment").select2('val', '');
        $("#Tenor").select2('val', '');
    },

    save: function () {
        var valid = widget.validate();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("wh.api/spkexhibition/save", data, function (result) {
                if (result.success) {
                    sdms.info({ type: "success", text: result.message });
                    widget.call('add');
                }
                else {
                    sdms.info({ type: "error", text: "Data SPK gagal disimpan ke database. " + result.message });
                    //alert("Data SPK gagal disimpan ke database. " + result.message);
                }
            });
        }
    },

    browse: function(){
        sdms.lookup({
            title: 'Daftar SPK Exhibition',
            url: 'wh.api/lookupgrid/kdpList',
            sort: [{ field: 'InquiryNumber', dir: 'desc' }, { field: 'InquiryDate', dir: 'desc' }],
            fields: [
                { name: 'InquiryNumber', text: 'No Inquiry', width: 120 },
                { name: 'InquiryDate', text: 'Tgl Inquiry', width: 120, type: 'date' },
                { name: 'SPKNo', text: 'Nomor SPK', width: 120 },
                { name: 'SPKDate', text: 'Tanggal SPK', width: 110, type: 'date' },
                { name: 'DealerName', text: 'Dealer', width: 400 },
                { name: 'OutletName', text: 'Outlet', width: 400 },
                { name: 'TipeKendaraan', text: 'Tipe Kendaraan', width: 220 },
                { name: 'Variant', text: 'Varian', width: 220 },
                { name: "Transmisi", text: "Trans", width: 80 },
                { name: "ColourName", text: "Warna", width: 340 },
                { name: "NamaProspek", text: "Nama Customer", width: 400 },
                { name: "EmployeeName", text: "Salesman", width: 300 },
                { name: "TestDrive", text: "Test Drive", width: 100 },
                { name: "QuantityInquiry", text: "Qty Inq", width: 80 },
                { name: "ShiftCode", text: "Shift", width: 80 }
            ],
            dblclick: 'loadRecordSPK',
            onclick:  'loadRecordSPK'
        });
    },

    loadRecordSPK: function (row) {
        var params = { InquiryNumber: row.InquiryNumber, CompanyCode: row.CompanyCode, BranchCode: row.BranchCode };
        $.post("wh.api/spkexhibition/get", params, this.populate);
    },

    lookUpCity: function(){
        var obj = widget.serializeObject();
        sdms.lookup({
            title: 'Lookup Data Kota',
            url: 'wh.api/lookupgrid/cities',
            filter: [
                { CityID: obj.CityID },
                { CompanyCode: obj.DealerCode }],
            sort: [{ field: 'CityName', dir: 'asc' }, { field: 'CityCode', dir: 'asc' }],
            fields: [
                { name: "CityName", text: "Nama Kota", width: 240 },
                { name: "CityCode", text: "Kode Kota" }
            ],
            dblclick: 'loadRecordCity',
            onclick:  'loadRecordCity'
        });
    },

    loadRecordCity: function (row) {
        widget.populate({ CityID: row.CityCode, CityName: row.CityName });
    },
    
    onChangeCarType: function () {
        var cmbTipe = $("[name='TipeKendaraan']");
        cmbTipe.off();
        cmbTipe.on("change", function () {
            widget.select({ selector: "[name=Variant]", url: "wh.api/spkexhibition/carvariants", params: { id: $("[name=TipeKendaraan]").val() } },
                function (res) {
                    if (res.length == 1) {
                        $("#Variant").select2('val',res[0].value);
                        widget.call('populateTransmission');
                    }
                    else {
                        $("#Variant").select2('val', '');
                        $("#Transmisi").select2('val', '');
                        $("#ColourCode").select2('val', '');
                        widget.call('populateTransmission');
                    }
                }
            );
        });
    },

    onChangeVariant: function () {
        var cmbVariant = $("[name='Variant']");
        cmbVariant.off();
        cmbVariant.on("change", function () {
            $("#Transmisi").select2('val', '');
            $("#ColourCode").select2('val', '');
            widget.call('populateTransmission');
        });
    },

    onChangeTransmission: function () {
        var cmbTransmisi = $("[name='Transmisi']");
        cmbTransmisi.off();
        cmbTransmisi.on("change", function () {
            $("#ColourCode").select2('val', '');
            widget.call('populateColour');
        });
    },

    populateTransmission: function () {
        widget.select({
        selector: "[name=Transmisi]", url: "wh.api/spkexhibition/transmissions",
        params: {
            CarType: $("[name=TipeKendaraan]").val(),
            CarVariant: $("[name=Variant]").val()
        }
        },
        function (res) {
            if (res.length == 1) {
                $("#Transmisi").select2('val',res[0].value);
                widget.call('populateColour');
                widget.validate();
            }
            else {
                widget.call('populateColour');
                widget.validate();
            }
        });
    },

    populateColour: function () {
        widget.select({
            selector: "[name=ColourCode]", url: "wh.api/spkexhibition/modelcolors",
            params: {
                CarType: $("[name=TipeKendaraan]").val(),
                CarVariant: $("[name=Variant]").val(),
                CarTrans: $("[name=Transmisi]").val()
            }
        },
            function (res) {
                if (res.length == 1) {
                    $("#ColourCode").select2('val',res[0].value);
                    widget.validate();
                }
                else {
                    widget.validate();
                }
            }
        );
    },
    
    populate: function (result) {
        $("[name=DealerCode]").val("0000000");
        if (result.success) {
            widget.populate(result.data);

            $('#StatusProspek').select2('val', result.data.StatusProspek);

            widget.select({
                selector: "select[name=CompanyCode]",
                url: "wh.api/combo/dealerabbre",
            }, function () {
                $('#CompanyCode').select2('val', result.data.CompanyCode);
            });

            widget.select({
                selector: "select[name=BranchCode]",
                url: "wh.api/combo/outletabbre",
                params: { id: result.data.CompanyCode },
                optionalText: " "
            }, function () {
                $('#BranchCode').select2('val', result.data.BranchCode);
            });

            widget.select({
                selector: "select[name=NikSales]",
                url: "wh.api/combo/sales",
                params: { Company: result.data.CompanyCode }
            }, function(){
                $('#NikSales').select2('val', result.data.NikSales);
            });

            widget.select({
                selector: "select[name=NikSH]", 
                url: "wh.api/combo/saleshead", 
                params: { Company: result.data.CompanyCode }, 
                optionalText: " " 
            }, function () {
                widget.post("wh.api/spkexhibition/getsaleshead/?company=" + result.data.CompanyCode + "&id=" + result.data.NikSales, function (EmployeeID) {
                    $("[name=NikSH]").select2('val', EmployeeID);
                });
            });

            widget.select({
                selector: "select[name=Grade]",
                url: "wh.api/combo/lookups/itsg" + "/?CompanyCode=" + result.data.CompanyCode,
                optionalText: " "
            },
            function () {
                $('#Grade').select2('val', result.data.Grade)
            });

            widget.select({
                selector: "select[name=ShiftCode]",
                url: "wh.api/combo/lookups/shift" + "/?CompanyCode=" + $("[name=DealerCode]").val()
            }, function () {
                $('#ShiftCode').select2('val', result.data.ShiftCode);
            });

            $('#TipeKendaraan').select2('val', result.data.TipeKendaraan);

            widget.select({
                selector: "select[name=Variant]",
                url: "wh.api/spkexhibition/carvariants",
                params: [
                    { name: "id", value: result.data.TipeKendaraan }

                ],
            }, function () {
                $('#Variant').select2('val', result.data.Variant);
            });

            widget.select({
                selector: "select[name=Transmisi]", url: "wh.api/spkexhibition/transmissions",
                params: [
                    { name: "CarType", value: result.data.TipeKendaraan },
                    { name: "CarVariant", value: result.data.Variant }
                ],
            },
            function (res) {
                $('#Transmisi').select2('val', result.data.Transmisi);
                widget.validate();
            });

            widget.select({
                selector: "select[name=ColourCode]", url: "wh.api/spkexhibition/modelcolors",
                params: [
                    { name: "CarType", value: result.data.TipeKendaraan },
                    { name: "CarVariant", value: result.data.Variant },
                    { name: "CarTrans", value: result.data.Transmisi }
                ]
            }, function () {
                $('#ColourCode').select2('val', result.data.ColourCode);
            });

            widget.select({ selector: "select[name=Leasing]", url: "wh.api/combo/lookups/lsng", params: { CompanyCode: $("[name=DealerCode]").val() } },
                function () {
                    $('#Leasing').select2('val', result.data.Leasing);
            });

            widget.select({ selector: "select[name=DownPayment]", url: "wh.api/combo/lookups/dwpm", params: { CompanyCode: $("[name=DealerCode]").val() }, optionalText: "-- SELECT DP --" },
                function () {
                    $('#DownPayment').select2('val', result.data.DownPayment);
                });

            widget.select({ selector: "select[name=Tenor]", url: "wh.api/combo/lookups/tenor", params: { CompanyCode: $("[name=DealerCode]").val() }, optionalText: "-- SELECT TENOR --" },
                function () {
                    $('#Tenor').select2('val', result.data.Tenor);
                });

            widget.select({ selector: "select[name=Hadiah]", url: "wh.api/combo/gift" + "/?CompanyCode=" + $("[name=DealerCode]").val() },
                function () {
                    $("#Hadiah").select2('val', result.data.Hadiah);
                });

            widget.select({ selector: "select[name=TestDrive]", url: "wh.api/combo/lookups/pmop", params: { CompanyCode: $("[name=DealerCode]").val() } },
                function () {
                    $("#TestDrive").select2('val', result.data.TestDrive);
                });

            //widget.call('validateControl', result.data);
        }
    },

    validateControl: function (data) {
        data = "20"; //Default pembayaran adalah cash
        var billtype = (data !== undefined) ? data.CaraPembayaran : $("[name=CaraPembayaran]").val();
        if (billtype == "20") { widget.showControl(["Leasing", "DownPayment"]); }
        else { widget.hideControl(["Leasing", "DownPayment"]); }
    },
});