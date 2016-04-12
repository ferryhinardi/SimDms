var widget = new SimDms.Widget({
    title: "Indent",
    xtype: "panels",
    toolbars: [
       { action: "add", text: "New", icon: "fa fa-file" },
       { action: "browse", text: "Browse", icon: "fa fa-search" },
       { action: "save", text: "Save", icon: "fa fa-save" },
    ],
    panels: [
        {
            title: "Data Salesman",
            items: [
                { name: "CompanyCode", type: "select", cls: "span4", text: "Dealer", disabled: true },
                { name: "BranchCode", type: "select", cls: "span4", text: "Outlet", disabled: true },
                { name: "IndentNumber", text: "Nomor Indent", cls: "span4", readonly: true },
                { name: "IndentDate", text: "Tanggal Indent", type: "datepicker", readonly: true, cls: "span4" },
                { name: "InquiryNumber", text: "Nomor Exhibition", type: "popup", cls: "span4", action: "lookUpInqExh" },
                { name: "InquiryDate", text: "Tanggal Exhibition", type: "datepicker", readonly: true, cls: "span4" },
                 {
                     text: "Sales Head", type: "controls", cls: "span4", items: [
                         { name: "NikSH", text: "Sales Head", type: "select", disabled: true },
                     ]
                 },
                 {
                     text: "Salesman", type: "controls", cls: "span4", items: [
                         { name: "NikSales", text: "Salesman", type: "select", required: true },
                     ]
                 },
                 {
                     text: "", type: "controls", items: [
                         { name: "NikSC", text: "Sales Coordinator", type: "select", disabled: true },
                     ]
                 },
                { name: "Grade", text: "Salesman Grade", cls: "span4", readonly: true, type: "select", disabled: true },
                { name: "PerolehanData", text: "Perolehan Data", type: "select", cls: "span4", required: true, datasource: "PerolehanData" },
                { name: "StatusProspek", model: "data.StatusProspek", text: "Status Inquiry", type: "select", cls: "span4", datasource: "StatusProspek" },
                { name: "EmployeeID", type: "hidden" },
                { name: "SpvEmployeeID", type: "hidden" },
                { name: "DeliveryMonth", type: "hidden" },
                { name: "DeliveryYear", type: "hidden" },
            ]
        },
        {
            title: "Data Prospek",
            required: true,
            items: [
                {
                    text: "Customer", type: "controls", items: [
                        { name: "NamaProspek", text: "Nama Customer", cls: "span6", required: true},
                        { name: "TelpRumah", text: "Telepon", cls: "span2", required: true },
                    ]
                },
                { name: "AlamatProspek", text: "Alamat", type: "textarea", required: true },
                {
                    text: "Kota", type: "controls", items: [
                        { name: "CityID", text: "Kode Kota", type: "popup", cls: "span2", maxlength: 15, action: "lookUpCity", readonly: true },
                        { name: "CityName", text: "Name Kota", cls: "span6" }
                    ]
                },
                {
                    text: "Perusahaan", type: "controls", items: [
                        { name: "NamaPerusahaan", text: "Nama Perusahaan", cls: "span6" },
                        { name: "Jabatan", text: "Jabatan", cls: "span2" },
                    ]
                },
                { name: "AlamatPerusahaan", text: "Alamat", type: "textarea" },
                { name: "Faximile", text: "Faximile", cls: "span4", readonly: false },
                { name: "Email", text: "Email", cls: "span4", readonly: false },
            ]
        },
            {
                title: "Kendaraan yang dicari",
                name: "pnlKendaraan",
                items: [
                    { name: "TipeKendaraan", text: "Type", type: "select", cls: "span4  ", required: true },
                    { name: "Variant", text: "Variant", type: "select", cls: "span4", required: true },
                    { name: "Transmisi", text: "Transmisi", type: "select", cls: "span4", required: true },
                    { name: "ColourCode", text: "Warna", type: "select", cls: "span4", required: true },
                    { name: "CaraPembayaran", text: "Pembayaran", type: "select", cls: "span8", required: true },
                    {
                        text: "Nama Leasing", type: "controls", cls: "span8", items: [
                            { name: "Leasing", type: "select", cls: "span8", required: true },
                        ]
                    },
                    {
                        text: "DP (%) / Tenor", type: "controls", items: [
                            { name: "DownPayment", type: "select", cls: "span4", required: true },
                            { name: "Tenor", type: "select", cls: "span4", required: true },
                        ]
                    },
                    { name: "TestDrive", text: "Test Drive", cls: "span4", type: "select", required: true },
                    { name: "QuantityInquiry", text: "Quantity", cls: "span4", readonly: true },
                ]
            },
        {
            title: "Status Information",
            items: [
                { name: "LastProgress", text: "Status Terakhir", cls: "span4", type: "select", required: true },
                { name: "LastUpdateStatus", text: "Updated Date", cls: "span4", type: "datepicker" },
                { type: "div"},
                //SPK Information
                //{ text: "Tanggal SPK", type: "controls", cls: "span4", items: [{ name: "SPKDate", type: "datepicker"}] },
                { text: "No. Inquiry", type: "controls", cls: "span4", items: [{ name: "SPKNo" }] },
                { text: "Tanggal Inquiry", id: "SPKDate", name: "SPKDate", type: "datepicker", cls: "span4" },
                { text: "Kategori", type: "controls", cls: "span4", items: [{ name: "StatusVehicle", type: "select" }] },
                {
                    text: "Kendaraan yang dimiliki", type: "controls",
                    items: [
                        { name: "BrandCode", text: "Merek", type: "popup", cls: "span2", action: "lookUpBrandCode" },
                        { name: "ModelName", text: "Tipe", cls: "span6", readonly: true },
                    ]
                },
                { name: "ShiftCode", type: "hidden" },
                //Lost Information
                //{ text: "Tanggal Lost", type: "controls", cls: "span4", items: [{ name: "LostCaseDate", type: "datepicker" }] },
                { text: "Tanggal Lost", id: "LostCaseDate", name: "LostCaseDate", type: "datepicker", cls: "span4" },
                { text: "Kategori", type: "controls", cls: "span6", items: [{ name: "LostCaseCategory", type: "select" }] },
                { text: "Alasan", type: "controls", cls: "span6", items: [{ name: "LostCaseReasonID", type: "select" }] },
                { text: "Merk Lain", type: "controls", cls: "span6", items: [{ name: "MerkLain" }] },
                { text: "Alasan Lain", type: "controls", cls: "span8", items: [{ name: "LostCaseOtherReason", type: "textarea" }] },
                { text: "Voice of Customer", type: "controls", cls: "span8", items: [{ name: "LostCaseVoiceOfCustomer", type: "textarea" }] },
            ]
        },
        {
            name: "pnlFollowUpDtl",
            title: "Follow Up Detail",
            items: [
                { name: "ActivityDate", text: "Tgl Follow Up", cls: "span4", type: "datepicker" },
                { name: "ActivityType", text: "Jenis Pertemuan", cls: "span4", type: "select"},
                { name: "ActivityDetail", text: "Keterangan", type: "textarea" },
                { name: "NextFollowUpDate", text: "Next Follow Up", cls: "span4", type: "datepicker" },
                { name: "ActivityID", cls: "span2" },
                 {
                     type: "buttons",
                     items: [
                         { name: "Add", action: "saveDtl", text: "Add", icon: "fa fa-file" },
                         { name: "Update", action: "saveDtl", text: "Update", icon: "fa fa-save" },
                         { name: "Delete", action: "deleteDtl", text: "Delete", icon: "fa fa-trash" },
                         { name: "Cancel", action: "clearDtl", text: "Cancel", icon: "fa fa-refresh" }
                     ]
                 },
            ]
        },
        {
           name: "KFUDtl",
           xtype: "k-grid",
        },
    ],

    onInit: function (wgt) {
        wgt.call('initial', wgt);
    },

    initial: function (wgt) {
        var Dealer = 'test';
        widget = widget || wgt;
        widget.clear();

        $("#Update").hide();
        $("#ActivityID").hide();
        $("#Hadiah").hide();

        //widget.enable({ value: false, items: ["LastProgress", "LastUpdateStatus"] });

       // widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/dealerabbre" });
        widget.select({ selector: "select[name=Grade]", url: "wh.api/combo/Lookupdtls", params: { id: 'itsg' } });
        widget.select({ selector: "select[name=StatusProspek]", url: "wh.api/combo/Lookupdtls", params: { id: 'pmsp' } },
            function () { $("#StatusProspek").select2('val', '10'); 
        });
        widget.select({ selector: "select[name=PerolehanData]", url: "wh.api/combo/Lookupdtls", params: { id: 'psrc' } },
            function () { $("#PerolehanData").select2('val', 'Referensi');
        });
        widget.select({ selector: "select[name=CaraPembayaran]", url: "wh.api/combo/Lookupdtls", params: { id: 'pmby' } },
            function (res) { $("#CaraPembayaran").select2('val', '10'); widget.call('validateControl', '10'); 
            });
        widget.select({ selector: "select[name=LastProgress]", url: "wh.api/indent/LastStatus" }, function () {
            $('#LastProgress').select2('val', 'SPK'); widget.call('validateControlLastProgress', 'SPK');
        });
        widget.select({ selector: "select[name=Leasing]", url: "wh.api/combo/Lookupdtls", params: { id: 'lsng' } });
        widget.select({ selector: "select[name=DownPayment]", url: "wh.api/combo/Lookupdtls", params: { id: 'dwpm' } });
        widget.select({ selector: "select[name=Tenor]", url: "wh.api/combo/Lookupdtls", params: { id: 'tenor' } });
        widget.select({ selector: "select[name=TestDrive]", url: "wh.api/combo/Lookupdtls", params: { id: 'pmop' } },
            function (res) {
                if (res.length == 1) { $("#TestDrive").select2('val', res[0].value); } else { $("#TestDrive").select2('val', ''); }
            }
        );
        //widget.select({ selector: "select[name=LastProgress]", url: "wh.api/combo/Lookupdtls", params: { id: 'PSTS' } });
        setTimeout(function () {
            widget.select({ selector: "select[name=TipeKendaraan]", url: "wh.api/indent/cartypes" },
                function (res) {
                    if (res.length == 1) {
                        $("#TipeKendaraan").select2('val', res[0].value);
                        widget.select({ selector: "[name=Variant]", url: "wh.api/indent/carvariants", params: { id: $("[name=TipeKendaraan]").val() } },
                            function (res) {
                                if (res.length == 1) {
                                    $("#Variant").select2('val', res[0].value);
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
                    }
                    else {
                        $("#TipeKendaraan").select2('val', '');
                    }
                }
            );
        }, 3600);

        widget.call('onChangeCarType');
        widget.call('onChangeVariant');
        widget.call('onChangeTransmission');

        widget.select({ selector: "select[name=ActivityType]", url: "wh.api/combo/Lookupdtls", params: { id: 'pact' } });
        widget.select({ selector: "select[name=LostCaseCategory]", url: "wh.api/combo/Lookupdtls", params: { id: 'plcc' } });
        widget.select({ selector: "select[name=StatusVehicle]", url: "wh.api/combo/StatusVehicles" });
        widget.select({ selector: "select[name=Hadiah]", url: "wh.api/combo/gift" + "/?CompanyCode=0000000"},
            function (res) {
                if (res.length == 1) {
                    $("#Hadiah").select2('val', res[0].value);
                }
                else {
                    $("#Hadiah").select2('val', '');
                }
            }
        );

        $("#CaraPembayaran").on("change", function () { widget.call('validateControl') });

        setTimeout(function () {
            widget.post("wh.api/Indent/default", function (result) {
                if (result.success) {
                    if (result.data.pType == '4W') {
                        widget.showControl(["NikSH"]);
                        widget.hideControl(["NikSC"]);
                    } else {
                        widget.showControl(["NikSC"]);
                        widget.hideControl(["NikSH"]);
                    }
                    widget.default = result.data;
                    widget.populate(widget.default);
                    Dealer = result.data.CompanyCode;
                    widget.select({ selector: "[name=CompanyCode]", url: "wh.api/combo/DealerCode" });
                    widget.select({
                        selector: "select[name=CompanyCode]",
                        url: "wh.api/combo/DealerCode",
                    }, function () {
                        $('#CompanyCode').select2('val', result.data.CompanyCode);
                    });
                }
                widget.select({ selector: "select[name=NikSales]", url: "wh.api/combo/Sales", params: { Company: Dealer } });
                widget.select({ selector: "select[name=NikSH]", url: "wh.api/combo/SH" });
                widget.select({
                    selector: "[name=BranchCode]", url: "wh.api/combo/outletabbre", params: { id: Dealer }, optionalText: " "
                },
                    function (res) {
                        if (res.length == 1) { $("#BranchCode").select2('val', res[0].value); } else { $("#BranchCode").select2('val', ""); }
                    }
                );
            });
        }, 3600);

        var LastStatus = $("[name='LastProgress']");
        LastStatus.off();
        LastStatus.on("change", function () {
            $(this).select2('val', $(this).val());
            var id = $(this).val();
            if (id == "SPK") {
                $("[name='LostCaseDate']").hide();
                $("[name='SPKDate']").show();
                $("[for='LostCaseDate']").hide();
                $("[for='SPKDate']").show();
                widget.showControl(["SPKDate", "SPKNo", "StatusVehicle", "BrandCode", "ModelName", ]);
                widget.hideControl(["LostCaseDate", "LostCaseCategory", "LostCaseReasonID", "MerkLain", "LostCaseOtherReason", "LostCaseVoiceOfCustomer"]);
            }
            else {
                var tgl = {
                    LostCaseDate: new Date()
                }
                widget.populate(tgl);
                $("[name='SPKDate']").hide();
                $("[name='LostCaseDate']").show();
                $("[for='SPKDate']").hide();
                $("[for='LostCaseDate']").show();
                widget.showControl(["LostCaseDate", "LostCaseCategory", "LostCaseReasonID", "MerkLain", "LostCaseOtherReason", "LostCaseVoiceOfCustomer"]);
                widget.hideControl(["SPKDate", "SPKNo", "StatusVehicle", "BrandCode", "ModelName", ]);
            }
        });

        var Pembayaran = $("[name='CaraPembayaran']");
        Pembayaran.off();
        Pembayaran.on("change", function () {
            $(this).select2('val', $(this).val());
            var id = $(this).val();
            if (id == "20") { widget.showControl(["Leasing", "DownPayment"]); }
            else { widget.hideControl(["Leasing", "DownPayment"]); }
        });

        var LostCaseCategory = $("[name='LostCaseCategory']");
        LostCaseCategory.off();
        LostCaseCategory.on("change", function () {
            $(this).select2('val', $(this).val());
            var id = $(this).val();
            widget.select({
                selector: "[name=LostCaseReasonID]", url: "wh.api/combo/LostReasons", params: { CodeID: "ITLR", LostCtg: id }, optionalText: " "
            },
                function (res) {
                    if (res.length == 1) { $("#LostCaseReasonID").select2('val', res[0].value); } else { $("#LostCaseReasonID").select2('val', ""); }
                }
            );
        });

        var Outlet = $("[name='BranchCode']");
        Outlet.off();
        Outlet.on("change", function () {
            $(this).select2('val', $(this).val());
            var id = $(this).val();
            var Company = $("#CompanyCode").val();
            var Branch = $("#BranchCode").val();
            if (id == "") {
                $("#Grade").select2('val', "");
                $("#NikSH").select2('val', "");
                $("#NikSales").select2('val', "");
            }
            else {
                $("#Grade").select2('val', "");
                $("#NikSH").select2('val', "");
                $("#NikSales").select2('val', "");
                widget.select({ selector: "select[name=NikSales]", url: "wh.api/combo/SM", params: { Company: Company, Branch: Branch } },
                   function (res) {
                       if (res.length == 1) { $("#NickSales").select2('val', res[0].value); } else { $("#NickSales").select2('val', ""); }
                   }
                );
                widget.select({ selector: "select[name=NikSH]", url: "wh.api/combo/SH", params: { Company: Dealer, Branch: $("#BranchCode").val() }, optionalText: " " });
            }        
        });

        var cmbNikSH = $("[name='NikSH']");
        cmbNikSH.off();
        cmbNikSH.on("change", function () {
            $(this).select2('val', $(this).val());
            var id = $(this).val();
            var company = $("#CompanyCode").val();
            if (id == "") {
                $("#EmployeeID").select2('val', "");
                $("#Grade").select2('val', "");
                $("#NikSH").select2('val', "");
            }
            else {
                widget.select({ selector: "select[name=NikSales]", url: "wh.api/Indent/GetTeamLeader", params: { company: company, id: $("#NikSH").val() }, optionalText: " " });
                $("#SpvEmployeeID").val(cmbNikSH);
            }
        });

        var cmbNikSales = $("[name='NikSales']");
        cmbNikSales.off();
        cmbNikSales.on("change", function () {
            $(this).select2('val', $(this).val());
            var id = $(this).val();
            //var company = $("#CompanyCode").val();
            var company = $("#CompanyCode").select2('val');
            console.log(Dealer);
            if (id == "") {
                $("#EmployeeID").select2('val', "");
                $("#Grade").select2('val', "");
                $("#NikSH").select2('val', "");
            }
            else {
                $("#EmployeeID").val(cmbNikSales);

                widget.post("wh.api/spkexhibition/getoutletbysales",
                { Company: Dealer, Employee: id }, function (outlet) {
                    $("#BranchCode").select2('val', outlet);
                });

                widget.post("wh.api/spkexhibition/getgrade", { Company: Dealer, id: id }, function (grade) {
                    $("#Grade").select2('val', grade);
                });

                widget.post("wh.api/spkexhibition/getsaleshead", { Company: Dealer, id: id }, function (EmployeeID) {
                    if (EmployeeID != "") {
                        $("#NikSH").select2('val',EmployeeID);
                        $("#SpvEmployeeID").val(EmployeeID);
                    }
                });
            }
        });

        var cmbHadiah = $("[name='Hadiah']");
        cmbHadiah.off();
        cmbHadiah.on("change", function () {
            $(this).select2('val', $(this).val());
        });
    },

    add: function (wgt) {
        widget = widget || wgt;
        widget.clear();

        $("#Update").hide();

        $("#BranchCode").select2('val', '');
        $("#NikSales").select2('val', '');
        $("#Grade").select2('val', '');
        $("#NikSH").select2('val', '');
        $("#PerolehanData").select2('val', '');
        $("#TipeKendaraan").select2('val', '');
        $("#Variant").select2('val', '');
        $("#Transmisi").select2('val', '');
        $("#ColourCode").select2('val', '');
        $("#Leasing").select2('val', '');
        $("#DownPayment").select2('val', '');
        $("#Tenor").select2('val', '');

        widget.call('initial', wgt);
    },

    lookUpInqExh: function () {
        console.log($("#NikSales").val());
        if ($("#NikSales").val() != null && $("#NikSales").val() != undefined && $("#NikSales").val() != "") {
            var obj = widget.serializeObject();
            sdms.lookup({
                title: 'Nomor Exhibition',
                url: 'wh.api/indent/Exhibition?NikSales=' + $('#NikSales').select2('val') + '&BranchCode=' + $('#BranchCode').select2('val'),
                sort: [{ field: 'InquiryNumber', dir: 'desc' }, { field: 'InquiryDate', dir: 'desc' }],
                fields: [
                    { name: "InquiryNumber", text: "Nomor Exhibition", width: 150 },
                    { name: "InquiryDate", text: "Tanggal Exhibition", width: 150, type: 'date' },
                    { name: "TipeKendaraan", text: "Tipe", width: 250 },
                    { name: "Variant", text: "Varian", width: 250 },
                    { name: "ColourCode", text: "Varian", width: 250 },
                ],
                dblclick: 'loadRecordInqExh',
                onclick: 'loadRecordInqExh'
            });
        } else {
            alert('Silakan pilih salesman terlebih dahulu');
        }
    },

    loadRecordInqExh: function (row) {
        var params = { InquiryNumber: row.InquiryNumber, CompanyCode: row.CompanyCode, BranchCode: row.BranchCode };
        $.post("wh.api/Indent/GetExhibition", params, this.populate);
    },

    lookUpCity: function () {
        var obj = widget.serializeObject();
        sdms.lookup({
            title: 'Kota',
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
            onclick: 'loadRecordCity'
        });
    },

    loadRecordCity: function (row) {
        widget.populate({ CityID: row.CityCode, CityName: row.CityName });
    },

    lookUpBrandCode: function () {
        var obj = widget.serializeObject();
        console.log($('#StatusVehicle').select2('val'));
        sdms.lookup({
            title: 'Merek Kendaraan',
            url: 'wh.api/indent/ModelList?fltStatus=' + $('#StatusVehicle').select2('val'),
            sort: [{ field: 'BrandCode', dir: 'asc' }, { field: 'ModelName', dir: 'asc' }],
            fields: [
                { name: "BrandCode", text: "Merek", width: 100 },
                { name: "ModelName", text: "Keterangan" },
                { name: "ModelType", text: "Tipe", width: 250 },
                { name: "Variant", text: "Varian", width: 250 },
            ],
            dblclick: 'loadRecordBrand',
            onclick: 'loadRecordBrand'
        });
    },

    loadRecordBrand: function (row) {
        widget.populate({ BrandCode: row.BrandCode, ModelName: row.ModelName });
    },

    lookUpSPKNo: function () {
        var obj = widget.serializeObject();
        sdms.lookup({
            title: 'No. SPK',
            url: 'wh.api/Indent/SpkNo?BranchCode=' + $('#BranchCode').select2('val'),
            fields: [
                { name: "SPKNo", text: "No SPK", width: 240 },
                { name: "SPKDate", text: "Tanggal SPK" }
            ],
            dblclick: 'loadRecordSPKNo',
            onclick: 'loadRecordSPKNo'
        });
    },

    loadRecordSPKNo: function (row) {
        widget.populate({ SPKNo: row.SPKNo, SPKDate: row.SPKDate, Hadiah: row.GiftRefferenceValue, ShiftCode: row.ShiftCode });
    },

    onChangeCarType: function () {
        var cmbTipe = $("[name='TipeKendaraan']");
        cmbTipe.off();
        cmbTipe.on("change", function () {
            widget.select({ selector: "[name=Variant]", url: "wh.api/indent/carvariants", params: { id: $("[name=TipeKendaraan]").val() } },
                function (res) {
                    if (res.length == 1) {
                        $("#Variant").select2('val', res[0].value);
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
            selector: "[name=Transmisi]", url: "wh.api/indent/transmissions",
            params: {
                CarType: $("[name=TipeKendaraan]").val(),
                CarVariant: $("[name=Variant]").val()
            }
        },
        function (res) {
            if (res.length == 1) {
                $("#Transmisi").select2('val', res[0].value);
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
            selector: "[name=ColourCode]", url: "wh.api/indent/modelcolors",
            params: {
                CarType: $("[name=TipeKendaraan]").val(),
                CarVariant: $("[name=Variant]").val(),
                CarTrans: $("[name=Transmisi]").val()
            }
        },
            function (res) {
                if (res.length == 1) {
                    $("#ColourCode").select2('val', res[0].value);
                    widget.validate();
                }
                else {
                    widget.validate();
                }
            }
        );
    },

    validateControl: function (data) {
        //data = "20"; //Default pembayaran adalah cash
        var billtype = (data !== undefined) ? data.CaraPembayaran : $("[name=CaraPembayaran]").val();
        if (billtype == "20") { widget.showControl(["Leasing", "DownPayment"]); }
        else { widget.hideControl(["Leasing", "DownPayment"]); }
    },

    validateControlLastProgress: function (data) {
        if (data == "SPK") {
            $("[name='LostCaseDate']").hide();
            $("[name='SPKDate']").show();
            $("[for='LostCaseDate']").hide();
            $("[for='SPKDate']").show();
            widget.showControl(["SPKDate", "SPKNo", "StatusVehicle", "BrandCode", "ModelName", ]);
            widget.hideControl(["LostCaseDate", "LostCaseCategory", "LostCaseReasonID", "MerkLain", "LostCaseOtherReason", "LostCaseVoiceOfCustomer"]);
        }
        else {
            $("[name='LostCaseDate']").show();
            $("[name='SPKDate']").hide();
            $("[for='LostCaseDate']").show();
            $("[for='SPKDate']").hide();
            widget.showControl(["LostCaseDate", "LostCaseCategory", "LostCaseReasonID", "MerkLain", "LostCaseOtherReason", "LostCaseVoiceOfCustomer"]);
            widget.hideControl(["SPKDate", "SPKNo", "StatusVehicle", "BrandCode", "ModelName", ]);
        }
    },

    save: function () {
        var valid = widget.validate();
        if (valid) {
            var data = $(".main form").serializeObject();
            console.log(data);
            widget.post("wh.api/Indent/save", data, function (result) {
                if (result.success) {
                    $("#IndentNumber").val(result.data.IndentNumber);
                    $("#DeliveryMonth").val(result.data.DeliveryMonth);
                    $("#DeliveryYear").val(result.data.DeliveryYear);
                    sdms.info({ type: "success", text: result.message });
                    alert(result.messageDev);
                }
                else {
                    sdms.info({ type: "error", text: "Data Indent gagal disimpan ke database. " + result.message });
                }
            });
        }
    },

    saveDtl: function () {
        var valid = widget.validate();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("wh.api/Indent/SaveAct", data, function (result) {
                if (result.success) {
                    sdms.info({ type: "success", text: "Data has been saved" });
                    widget.call('refreshGrid', result.data);
                }
                else {
                    sdms.info({ type: "error", text: "Data Indent gagal disimpan ke database. " + result.message });
                }
            });
        }
    },

    browse: function () {
        sdms.lookup({
            title: 'Daftar Indent',
            url: 'wh.api/lookupgrid/IndentList',
            sort: [{ field: 'IndentNumber', dir: 'desc' }, { field: 'IndentDate', dir: 'desc' }],
            fields: [
                { name: "LastProgress", text: "Status", width: 80 },
                { name: 'PeriodeDelivery', text: 'Periode Delivery', width: 130 },
                //{ name: 'DeliveryYear', text: 'Delivery Year', width: 100 },
                //{ name: 'DeliveryMonth', text: 'Delivery Month', width: 100 },
                { name: 'DealerName', text: 'Dealer', width: 100 },
                { name: 'OutletName', text: 'Outlet', width: 130 },
                { name: 'IndentNumber', text: 'No Indent', width: 120 },
                { name: 'IndentDate', text: 'Tgl Indent', width: 120, type: 'date' },
                { name: 'TipeKendaraan', text: 'Tipe Kendaraan', width: 220 },
                { name: 'Variant', text: 'Varian', width: 200 },
                { name: "Transmisi", text: "Trans", width: 60 },
                { name: "ColourName", text: "Warna", width: 200 },
                { name: "PerolehanData", text: "Perolehan Data", width: 200 },
                { name: "NamaProspek", text: "Nama Customer", width: 300 },
                { name: "EmployeeName", text: "Salesman", width: 250 },
                { name: "TestDrive", text: "Test Drive", width: 100 },
                { name: "QuantityInquiry", text: "Qty Inq", width: 80 },
            ],
            dblclick: 'loadRecord',
            onclick: 'loadRecord'
        });
    },

    loadRecord: function (row) {
        var params = { InquiryNumber: row.IndentNumber, CompanyCode: row.CompanyCode, BranchCode: row.BranchCode };
        $.post("wh.api/Indent/get", params, this.populate);
        widget.call('validateControl', row.TestDrive);
        widget.call('validateControlLastProgress', row.LastProgress);
        widget.call('refreshGrid', row);
    },

    populate: function (result) {
        $("[name=DealerCode]").val("0000000");
        if (result.success) {
            widget.populate(result.data);

            $('#StatusProspek').select2('val', result.data.StatusProspek);
            $('#SpvEmployeeID').val(result.data.SpvEmployeeID);
            $('#EmployeeID').val(result.data.EmployeeID);

            $('#CompanyCode').select2('val', result.data.CompanyCode);
            $('#BranchCode').select2('val', result.data.BranchCode);

            $('#LostCaseDate').datepicker();

            widget.select({
                selector: "select[name=NikSales]",
                url: "wh.api/combo/sales",
                params: { Company: result.data.CompanyCode }
            }, function () {
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

            $('#Leasing').select2('val', result.data.Leasing);
            $('#DownPayment').select2('val', result.data.DownPayment);
            $('#Tenor').select2('val', result.data.Tenor);
            $("#TestDrive").select2('val', result.data.TestDrive);

            widget.select({
                selector: "[name=LostCaseReasonID]", url: "wh.api/combo/LostReasons", params: { CodeID: "ITLR", LostCtg: result.data.LostCaseCategory },
            }, function () {
                $('#LostCaseReasonID').select2('val', result.data.LostCaseReasonID);
            });
            //widget.call('validateControl', result.data);
            //widget.call('refreshGrid', result.data);
        }
    },

    refreshGrid: function (data) {
        var filter = {
            BranchCode: data.BranchCode,
            InquiryNumber: data.IndentNumber
        }
        widget.kgrid({
        url: "wh.api/Indent/pmActivitiesIndent",
        name: "KFUDtl",
        params: filter,
        serverBinding: true,
        sort: [
            { field: "ActivityID", dir: "asc" },
        ],
        columns: [
            { field: "ActivityID", title: "ID", width: 50 },
            {
                field: "ActivityDate", title: "Tanggal", width: 150,
                template: "#= moment(ActivityDate).format('DD MMM YYYY') #"
            },
            { field: "ActivityType", title: "Jenis Pertemuan", width: 352 },
            { field: "ActivityDetail", title: "Follow Up Detail", width: 400 },
            {
                field: "NextFollowUpDate", title: "Next Follow Up Date", width: 150,
                template: "#= moment(NextFollowUpDate).format('DD MMM YYYY') #"
            },
        ],
        onDblClick: function (a,b,c) {
            widget.selectedRow("KFUDtl", function (data) {
                if (data) {
                    $("#Add").hide();
                    $("#Update").show();
                    console.log(data);
                    var url = "wh.api/Indent/getDetail";
                    var params = {
                        IndentNumber: data.IndentNumber
                        , CompanyCode: data.CompanyCode
                        , BranchCode: data.BranchCode
                        , ActivityID: data.ActivityID
                    };
                    widget.post(url, params, function (result) {
                        if (result.success) {
                            var ActivityDate = moment(result.data.ActivityDate).format(SimDms.dateFormat);
                            $('#ActivityDate').val(ActivityDate);
                            $('#ActivityType').select2('val',result.data.ActivityType);
                            $('#ActivityDetail').val(result.data.ActivityDetail);
                            $('#ActivityID').val(result.data.ActivityID);
                        }
                    });
                }
            });
        }
      });
    },

    clearDtl: function () {
        $('#ActivityDate').val();
        $('#ActivityType').select2('val', '');
        $('#ActivityDetail').val('');
        $('#ActivityID').val();
        $("#Add").show();
        $("#Update").hide();
    },

    //deleteDtl: function () {
    //    var valid = widget.validate();
    //    if (valid) {
    //        console.log($('#ActivityID').val())
    //        var data = {
    //            InquiryNumber: $('#InquiryNumber').val()
    //            , CompanyCode: $('#CompanyCode').select2('val')
    //            , BranchCode: $('#BranchCode').select2('val')
    //            , ActivityID: $('#ActivityID').val()
    //        }
    //        widget.post("wh.api/Indent/DeleteAct", data, function (result) {
    //            if (result.success) {
    //                sdms.info({ type: "success", text: "Data has been deleted" });
    //                widget.call('refreshGrid', result.data);
    //                widget.call('clearDtl');
    //            }
    //            else {
    //                sdms.info({ type: "error", text: "Data Indent gagal di delete dari database. " + result.message });
    //            }
    //        });
    //    }
    //}
});
