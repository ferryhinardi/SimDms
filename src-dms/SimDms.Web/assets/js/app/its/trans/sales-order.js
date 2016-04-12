$(document).ready(function () {
    var vars = {};

    var widget = new SimDms.Widget({
        title: "Sales Order",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
            { name: "btnCustomer", text: "Pelanggan", icon: "icon-user" },
            { name: "btnApprove", text: "Approve", icon: "icon-check", cls: "hide" },
            { name: "btnPrint", text: "Print", icon: "icon-print", cls: "hide" },
            { name: "btnUnapprove", text: "Unapprove", icon: "icon-", cls: "hide" },
            { name: "btnReject", text: "Reject", icon: "icon-", cls: "hide" },
        ],
        panels: [
            {
                title: "Status",
                items: [
                    { type: "text", text: "Status", name: "SOStatus", cls: "span4", readonly: true }
                ]
            },
            {
                title: "Informasi Pelanggan",
                items: [
                    { name: "SONumber", text: "No. SO", cls: "span4", readonly: true },
                    { name: "SODate", text: "Tgl. SO", type: "kdatepicker", cls: "span4", required: true, readonly: true },
                    { name: "ReffNumber", text: "No. Reff", cls: "span4" },
                    { name: "ReffDate", text: "Tgl. Reff", type: "kdatepicker", cls: "span4" },
                    { name: "IsDirectSales", text: "Direct Sales", type: "switch", float: "left", onChanged: isDirectSales_changed },
                    {
                        text: "No. ITS",
                        type: "controls",
                        items: [
                            { name: "ITSNumber", text: "No. ITS", type: "popup", cls: "span2", readonly: true, required: false },
                            { name: "VehicleType", text: "Tipe Kendaraan", readonly: true, cls: "span3", required: false },
                            { name: "SKPKNumber", text: "No. SKPK", cls: "span3", required: true },
                        ]
                    },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", text: "Kode", type: "popup", cls: "span2", readonly: true, required: true },
                            { name: "CustomerName", text: "Nama", readonly: true, cls: "span6", required: false },
                        ]
                    },
                    {
                        text: "Salesman",
                        type: "controls",
                        items: [
                            { name: "SalesmanCode", text: "Kode", type: "popup", cls: "span2", readonly: true, required: true },
                            { name: "SalesmanName", text: "Nama", readonly: true, cls: "span6", required: false },
                        ]
                    },
                    {
                        text: "TOP",
                        type: "controls",
                        items: [
                            { name: "TOPCode", text: "Kode", type: "popup", cls: "span2", readonly: true },
                            { name: "TOPName", text: "Nama", readonly: true, cls: "span3" },
                            { name: "TOPPaidWith", text: "Dibayar dengan", cls: "span3" },
                        ]
                    },
                    {
                        text: "Tagih ke",
                        type: "controls",
                        items: [
                            { name: "ChargedToCode", text: "Kode", type: "text", cls: "span2", readonly: true },
                            { name: "ChargedToName", text: "Nama", readonly: true, cls: "span6" },
                        ]
                    },
                    {
                        text: "Kirim ke",
                        type: "controls",
                        items: [
                            { name: "ShipToCode", text: "Kode", type: "popup", cls: "span2", readonly: true },
                            { name: "ShipToName", text: "Nama", readonly: true, cls: "span6" },
                        ]
                    },
                    {
                        text: "Gudang",
                        type: "controls",
                        items: [
                            { name: "WarehouseCode", text: "Kode", type: "popup", cls: "span2", readonly: true, required: true },
                            { name: "WarehouseName", text: "Nama", readonly: true, cls: "span6", required: false },
                        ]
                    },
                    {
                        text: "Group Price",
                        type: "controls",
                        items: [
                            { name: "GroupPriceCode", text: "Kode", readonly: true, cls: "span2", required: true },
                            { name: "GroupPriceName", text: "Nama", readonly: true, cls: "span2", required: false },
                        ]
                    },
                ]
            },
            {
                title: "Informasi Leasing",
                items: [
                    { name: "IsLeasing", text: "Leasing", type: "switch", float: "left", onChanged: IsLeasing_changed },
                    {
                        text: "Info Leasing",
                        type: "controls",
                        items: [
                            { name: "LeasingCode", text: "Kode Leasing", type: "popup", cls: "span2", readonly: true },
                            { name: "LeasingName", text: "Nama Leasing", readonly: true, cls: "span3" },
                            {
                                name: "Tenor", text: "Angsuran", type: "select", cls: "span3",
                                items: [
                                    { text: "12", value: "12" },
                                    { text: "24", value: "24" },
                                    { text: "36", value: "36" },
                                    { text: "48", value: "48" },
                                    { text: "60", value: "60" },
                                ]
                            },
                        ]
                    },
                    {
                        text: "Tanggal Lunas",
                        type: "controls",
                        items: [
                            { name: "PaidPaymentDate", text: "Tgl. Lunas", type: "kdatepicker", cls: "span2" },
                            { name: "Insurance", text: "Asuransi", cls: "span3" },
                            { name: "Advance", text: "Uang Muka", cls: "span3 number" },
                        ]
                    },
                    {
                        text: "Diterima Oleh",
                        type: "controls",
                        items: [
                            { name: "PayeeCode", text: "Kode Penerima", cls: "span2", type: "popup" },
                            { name: "PayeeName", text: "Nama Penerima", readonly: true, cls: "span4" },
                            { name: "ReceivingDate", type: "kdatepicker", cls: "span2" },
                        ]
                    },
                ]
            },
            {
                title: "Informasi Kontrak",
                items: [
                    {
                        text: "No PO / No Kontrak",
                        type: "controls",
                        items: [
                            { name: "PONumber", text: "No PO", cls: "span2" },
                            { name: "ContractNumber", text: "No Kontrak", cls: "span3" },
                        ]
                    },
                    {
                        text: "Tgl Dibubuhkan",
                        type: "controls",
                        items: [
                            { name: "ContractDate", type: "kdatepicker", cls: "span2" },
                        ]
                    },
                    { name: "ContractNote", text: "Keterangan", type: "textarea" },
                ]
            },
            {
                xtype: "tabs",
                name: "tabSalesOrder",
                items: [
                    { name: "tabSalesModel", text: "Sales Model", cls: "active" },
                    { name: "tabVehicleInformation", text: "Informasi Kendaraan" },
                    { name: "tabAccessories", text: "Aksesories" },
                    { name: "tabSparepart", text: "Sparepart" },
                ],
                onChanged: showSubGrid
            },
            {
                name: "panelSalesModel1",
                cls: "tabSalesOrder tabSalesModel",
                title: "Sales Model",
                items: [
                    { name: "SalesModelCode", text: "Sales Model Code", cls: "span3", type: "popup", readonly: true },
                    {
                        text: "Sales Model Year",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "SalesModelYear", text: "Kode", cls: "span3", type: "popup", readonly: true },
                            { name: "SalesModelDesc", text: "Nama", cls: "span5", readonly: true },
                        ]
                    },
                    { name: "ShipAmt", text: "Ongkos Kirim", cls: "span3 number" },
                    { name: "DepositAmt", text: "Unit Deposit", cls: "span3 number" },
                    { name: "OthersAmt", text: "Lain-lain", cls: "span3 number" },
                    { name: "Discount", text: "Diskon (%)", cls: "span3 number", readonly: true },
                    { name: "Remark", text: "Keterangan", type: "textarea", cls: "span8" },
                ]
            },
            {
                name: "panelSalesModel2",
                cls: "tabSalesOrder tabSalesModel",
                title: "Harga",
                items: [
                    { name: "BeforeDiscTotal", text: "Sebelum Diskon", cls: "span4 number", readonly: true },
                    { name: "AfterDiscTotal", text: "Setelah Diskon", cls: "span4 number" },
                    {
                        text: "DPP / PPn /PPnBM",
                        type: "controls",
                        items: [
                            { name: "DPP", text: "DPP", cls: "span3 number", readonly: true },
                            { name: "PPn", text: "PPn", cls: "span3 number", readonly: true },
                            { name: "PPnBM", text: "PPnBM", cls: "span2 number", readonly: true },
                        ]
                    },
                ]
            },
            {
                cls: "tabSalesOrder tabVehicleInformation",
                title: "Detail Warna",
                items: [
                    {
                        text: "Warna",
                        type: "controls",
                        items: [
                            { name: "ColourCode", text: "Kode", cls: "span2", type: "popup", readonly: true, required: true },
                            { name: "ColourDesc", text: "Nama", cls: "span4", readonly: true },
                            { name: "Quantity", text: "Jumlah", cls: "span2", type: "int", required: true },
                        ]
                    },
                    { name: "RemarkColour", text: "Keterangan", type: "textarea" },
                ]
            },
            {
                xtype: "kgrid",
                name: "gridColourModel"
            },
            {
                cls: "tabSalesOrder tabVehicleInformation",
                name: "tabVehicleInformation",
                title: "Lain - lain",
                items: [
                    {
                        text: "Kode / Nomor Rangka",
                        type: "controls",
                        items: [
                            { name: "ChassisCode", text: "Kode", cls: "span4", readonly: true },
                            { name: "ChassisNo", text: "Nomor", cls: "span4", type: "popup", readonly: true, required: true },
                        ]
                    },
                    { name: "STNKName", text: "Nama STNK", required: true },
                    { name: "STNKAddress1", text: "Alamat STNK", required: true },
                    {
                        type: "controls",
                        items: [
                            { name: "STNKAddress2", cls: "span4", readonly: false },
                            { name: "STNKAddress3", cls: "span4", readonly: false },
                        ]
                    },
                    {
                        text: "Pemasok BBN",
                        type: "controls",
                        items: [
                            { name: "SupplierCode", text: "Kode", cls: "span2", type: "popup", readonly: true },
                            { name: "SupplierName", text: "Nama", cls: "span6", readonly: true },
                        ]
                    },
                    {
                        text: "Kota",
                        type: "controls",
                        items: [
                            { name: "CityCode", text: "Kode", cls: "span2", type: "popup", readonly: true },
                            { name: "CityName", text: "Nama", cls: "span6", readonly: true },
                        ]
                    },
                    { name: "BBN", text: "BBN", cls: "span4", readonly: true },
                    { name: "KIR", text: "KIR", cls: "span4", readonly: true },
                    { name: "RemarkOther", text: "Keterangan", type: "textarea" },
                ]
            },
            {
                xtype: "kgrid",
                name: "gridOthers"
            },
            {
                cls: "tabSalesOrder tabAccessories",
                title: "Aksesories",
                items: [
                    {
                        text: "Aksesories Lain-lain",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span2", type: "popup", readonly: true },
                            { text: "Nama", cls: "span6", readonly: true },
                        ]
                    },
                    { text: "Sebelum Diskon", cls: "span4", type: "decimal" },
                    { text: "Setelah Diskon", cls: "span4", type: "decimal" },
                    { text: "DPP", cls: "span4", type: "decimal" },
                    { text: "PPN", cls: "span4", type: "decimal" },
                ]
            },
            {
                cls: "tabSalesOrder tabSparepart",
                title: "Sparepart",
                items: [
                    {
                        text: "Sparepart",
                        type: "controls",
                        items: [
                            { text: "Kode", cls: "span2", type: "popup", readonly: true },
                            { text: "Nama", cls: "span6", readonly: true },
                        ]
                    },
                    { text: "Sebelum Diskon", cls: "span4", type: "decimal" },
                    { text: "Setelah Diskon", cls: "span4", type: "decimal" },
                    { text: "DPP", cls: "span4", type: "decimal" },
                    { text: "PPN", cls: "span4", type: "decimal" },
                    { text: "Quantity", cls: "span4", type: "decimal" },
                    { text: "Unit", cls: "span4", type: "decimal" },
                ]
            },
            {
                xtype: "kgrid",
                name: "gridSalesModel"
            },
            //{
            //    xtype: "kgrid",
            //    name: "gridAccesories"
            //},
            //{
            //    xtype: "kgrid",
            //    name: "gridSpareparts"
            //}
        ]
    });

    widget.default = {};
    widget.render(renderCallback);

    function renderCallback() {
        initEvent();
        initLookup();
        setDefaultState();
        initGrid();
    }

    function setDefaultState() {
        var btnITSNumber = $("#btnITSNumber");
        var textITSNumber = $("[name='ITSNumber']");
        var textLeasingCode = $("[name='LeasingCode']");
        var btnLeasingCode = $("#btnLeasingCode");
        var textLeasingName = $("[name='LeasingName']");
        var cmbTenor = $("[name='Tenor']");
        var textPayeeCode = $("[name='PayeeCode']");
        var btnPayeeCode = $("[name='btnPayeeCode']");
        var textPayeeName = $("[name='PayeeName']");
        var textSOStatus = $("[name='SOStatus']");

        btnITSNumber.attr("disabled", true);
        textITSNumber.attr("readonly", true);
        textLeasingCode.attr("readonly", true);
        btnLeasingCode.attr("disabled", true);
        textLeasingName.attr("readonly", true);
        textPayeeCode.attr("readonly", true);
        textPayeeName.attr("readonly", true);
        cmbTenor.attr("disabled", true);
        textSOStatus.val("NEW");

        showTab(false);
        vars["EditStatusDetail"] = false;

        widget.post("its.api/SalesOrder/Default", function (result) {
            if (widget.isNullOrEmpty(result) == false) {
                vars = $.extend(vars, result);
                widget.populate(vars);
            };
        });
    }

    function initEvent() {
        var btnNew = $("#btnNew");
        var btnSave = $("#btnSave");
        var btnApprove = $("#btnApprove");
        var btnUnapprove = $("#btnUnapprove");
        var btnReject = $("#btnReject");
        var btnPrint = $("#btnPrint");
        var textAfterDiscTotal = $("[name='AfterDiscTotal']");

        btnNew.off();
        btnNew.on("click", function () {
            widget.clearForm();
            $("input").removeClass("error");
            initGrid();
            widget.showToolbars(["btnNew", "btnBrowse", "btnSave", "btnCustomer", "btnCustomer"]);

            var textSOStatus = $("[name='SOStatus']");
            textSOStatus.val("NEW");

            var textSKPKNumber = $("[name='SKPKNumber']");
            textSKPKNumber.attr("readonly", true);

            btnITSNumber = $("#btnITSNumber");
            btnITSNumber.attr("disabled", true);

            showTab(false);
            vars["EditStatusDetail"] = false;
            $("[data-id='tabSalesOrder'] > p[data-name='tabSalesModel'] > a").click();
        });

        btnSave.off();
        btnSave.on("click", function () {
            saveSalesOrder();
        });

        btnApprove.off();
        btnApprove.on("click", function () {
            var data = widget.getForms();
            widget.post("its.api/SalesOrder/Approve", data, function (result) {
                if (result.status) {
                    widget.populate(result.data);
                }

                widget.showNotification(result.message);
            });
        });

        btnUnapprove.off();
        btnUnapprove.on("click", function () {
            console.log("Unapprove");
        });

        btnReject.off();
        btnReject.on("click", function () {
            console.log("Reject");
        });

        btnPrint.off();
        btnPrint.on("click", function () {
            var url = SimDms.baseUrl + "layout#lnk/its/trans/SalesOrderPrint?SONumber=" + $("[name='SONumber']").val();
            window.open(url, "_blank");
        });

        textAfterDiscTotal.keyup(function () {
            var _this = $(this);
            var textDPP = $("[name='DPP']");
            var textPPn = $("[name='PPn']");
            var textPPnBM = $("[name='PPnBM']");
            var textShipAmt = $("[name='ShipAmt']");
            var textDepositAmt = $("[name='DepositAmt']");
            var textOthersAmt = $("[name='OthersAmt']");
            var textBeforeDiscTotal = $("[name='BeforeDiscTotal']");
            var textDiscount = $("[name='Discount']");

            var finalPriceBeforeDiscount = removeNumberFormat(number_format(textBeforeDiscTotal.val().toString(), 2));
            var finalPrice = removeNumberFormat(number_format(_this.val().toString(), 2));
            var DPP = removeNumberFormat(number_format(textDPP.val().toString(), 2));
            var PPn = removeNumberFormat(number_format(textPPn.val().toString(), 2));
            var PPnBM = removeNumberFormat(number_format(textPPnBM.val().toString(), 2));
            var shipCost = removeNumberFormat(number_format(textShipAmt.val().toString(), 2));
            var depositCost = removeNumberFormat(number_format(textOthersAmt.val().toString(), 2));
            var otherCost = removeNumberFormat(number_format(textBeforeDiscTotal.val().toString(), 2));

            var newDPP = finalPrice * 100 / 110;
            var newPPn = finalPrice * 10 / 110;
            var newPPnBM = 0;
            var discount = number_format(Math.abs(finalPriceBeforeDiscount - finalPrice) * 100 / finalPrice, 2);

            textDPP.val(number_format(newDPP, 2));
            textPPn.val(number_format(newPPn, 2));
            textPPnBM.val(number_format(newPPnBM, 2));
            textDiscount.val(discount);
        });
    }

    function initLookup() {
        widget.klookup({
            name: "btnITSNumber",
            title: "ITS",
            url: "its.api/grid/ItsList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterInquiryNo", text: "No. ITS", cls: "span4" },
                        { name: "filterTipeKendaraan", text: "Tipe Kendaraan", cls: "span4" },
                        { name: "filterSalesman", text: "Salesman", cls: "span4" },
                        { name: "filterNamaProspek", text: "Nama Prospek", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "InquiryNo", title: "No. ITS", width: 100 },
                { field: "InquiryDate", title: "Tanggal ITS", width: 100, template: "#= InquiryDate == undefined ? '-' : moment(InquiryDate).format('DD MMM YYYY') #" },
                { field: "EmployeeName", title: "Salesman", width: 100 },
                { field: "NamaProspek", title: "Nama Prospek", width: 100 },
                { field: "TipeKendaraan", title: "Tipe Kendaraan", width: 100 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='SalesmanCode']").val(data.EmployeeID || "");
                    $("[name='SalesmanName']").val(data.EmployeeName || "");
                    $("[name='ITSNumber']").val(data.InquiryNo || "");
                    $("[name='VehicleType']").val(data.TipeKendaraan || "");
                }
            }
        });

        widget.klookup({
            name: "btnCustomerCode",
            title: "Customers",
            url: "its.api/grid/SOCustomers",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterCustomerCode", text: "Kode Pelanggan", cls: "span2" },
                        { name: "filterCustomerName", text: "Nama Pelanggan", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "CustomerCode", title: "Kode Pelanggan", width: 100 },
                { field: "CustomerName", title: "Nama Pelanggan", width: 150 },
                { field: "Address", title: "Alamat", width: 250 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='CustomerCode']").val(data.CustomerCode || "");
                    $("[name='CustomerName']").val(data.CustomerName || "");
                    $("[name='ChargedToCode']").val(data.CustomerCode || "");
                    $("[name='ChargedToName']").val(data.CustomerName || "");
                    $("[name='GroupPriceCode']").val(data.GroupPriceCode || "");
                    $("[name='GroupPriceName']").val(data.GroupPriceDesc || "");
                }
            }
        });

        widget.klookup({
            name: "btnSalesmanCode",
            title: "Salesman",
            url: "its.api/grid/SalesmanList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterEmployeeID", text: "Kode Salesman", cls: "span2" },
                        { name: "filterEmployeeName", text: "Nama Salesman", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "EmployeeID", title: "Kode Salesman", width: 100 },
                { field: "EmployeeName", title: "Nama Salesman", width: 150 },
                { field: "PositionName", title: "Posisi", width: 250 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='SalesmanCode']").val(data.EmployeeID || "");
                    $("[name='SalesmanName']").val(data.EmployeeName || "");
                }
            },
        });

        widget.klookup({
            name: "btnTOPCode",
            title: "TOP",
            url: "its.api/grid/TopCList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterTOPCode", text: "Kode TOP", cls: "span2" },
                        { name: "filterTOPName", text: "Nama TOP", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "LookUpValue", title: "Kode TOP", width: 100 },
                { field: "LookUpValueName", title: "Nama TOP", width: 150 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='TOPCode']").val(data.LookUpValue || "");
                    $("[name='TOPName']").val(data.ParaValue || "");
                }
            }
        });

        widget.klookup({
            name: "btnShipToCode",
            title: "Ship To",
            url: "its.api/grid/SOCustomers",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterCustomerCode", text: "Kode Pelanggan", cls: "span2" },
                        { name: "filterCustomerName", text: "Nama Pelanggan", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "CustomerCode", title: "Kode Pelanggan", width: 100 },
                { field: "CustomerName", title: "Nama Pelanggan", width: 150 },
                { field: "Address", title: "Alamat", width: 250 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='ShipToCode']").val(data.CustomerCode || "");
                    $("[name='ShipToName']").val(data.CustomerName || "");
                }
            }
        });

        widget.klookup({
            name: "btnWarehouseCode",
            title: "Warehouse",
            url: "its.api/grid/WarehouseList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterWarehouseCode", text: "Kode Gudang", cls: "span2" },
                        { name: "filterWarehouseName", text: "Nama Gudang", cls: "span6" },
                    ]
                },
            ],
            columns: [
                { field: "LookUpValue", title: "Kode Gudang", width: 100 },
                { field: "LookUpValueName", title: "Nama Gudang", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='WarehouseCode']").val(data.LookUpValue || "");
                    $("[name='WarehouseName']").val(data.LookUpValueName || "");
                }
            }
        });

        widget.klookup({
            name: "btnReceiverCode",
            title: "Penerima",
            url: "its.api/grid/SOCustomers",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterCustomerCode", text: "Kode Pelanggan", cls: "span2" },
                        { name: "filterCustomerName", text: "Nama Pelanggan", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "CustomerCode", title: "Kode Pelanggan", width: 100 },
                { field: "CustomerName", title: "Nama Pelanggan", width: 150 },
                { field: "Address", title: "Alamat", width: 250 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='ReceiverCode']").val(data.CustomerCode || "");
                    $("[name='ReceiverName']").val(data.CustomerName || "");
                }
            }
        });

        widget.klookup({
            name: "btnLeasingCode",
            title: "Leasing",
            url: "its.api/grid/LeasingList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterLeasingCode", text: "Kode Leasing", cls: "span2" },
                        { name: "filterLeasingName", text: "Nama Leasing", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "LeasingCode", title: "Kode Leasing", width: 100 },
                { field: "LeasingName", title: "Nama Leasing", width: 150 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            },
            dynamicParams: [
                { name: "InquiryNumber", element: "ITSNumber" }
            ]
        });

        widget.klookup({
            name: "btnPayeeCode",
            title: "Employee List",
            url: "its.api/grid/EmployeeList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterEmployeeID", text: "Kode Karyawan", cls: "span2" },
                        { name: "filterEmployeeName", text: "Nama Karyawan", cls: "span3" },
                        { name: "filterEmployeePosition", text: "Posisi karyawan", cls: "span3" },
                    ]
                },
            ],
            columns: [
                { field: "EmployeeID", title: "Kode Salesman", width: 100 },
                { field: "EmployeeName", title: "Nama Salesman", width: 150 },
                { field: "PositionName", title: "Posisi", width: 250 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='PayeeCode']").val(data.EmployeeID || "");
                    $("[name='PayeeName']").val(data.EmployeeName || "");
                }
            }
        });

        widget.klookup({
            name: "btnSalesModelCode",
            title: "Sales Model",
            url: "its.api/grid/SalesModelList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterSalesModelCode", text: "Kode", cls: "span2" },
                        { name: "filterSalesModelDesc", text: "Keterangan", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "SalesModelCode", title: "Kode Sales Model", width: 100 },
                { field: "SalesModelDesc", title: "Keterangan", width: 150 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='SalesModelCode']").val(data.SalesModelCode || "");
                    $("[name='SalesModelDesc']").val(data.SalesModelDesc || "");
                }
            },
            dynamicParams: [
                { name: "InquiryNumber", element: "ITSNumber" }
            ]
        });

        widget.klookup({
            name: "btnSalesModelYear",
            title: "Sales Model Year",
            url: "its.api/grid/SalesModelYearList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterSalesModelYear", text: "Tahun", cls: "span2" },
                        { name: "filterChassisCode", text: "Kode Rangka", cls: "span2" },
                    ]
                },
            ],
            columns: [
                { field: "SalesModelYear", title: "Tahun", width: 100 },
                { field: "SalesModelCode", title: "Deskripsi", width: 150 },
                { field: "ChassisCode", title: "Kode Rangka", width: 150 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    clearFormSalesModel("2");
                    $("[name='SalesModelYear']").val(data.SalesModelYear || "");
                    $("[name='SalesModelDesc']").val(data.SalesModelDesc || "");
                    vars["ChassisCode"] = data.ChassisCode;

                    var params = {
                        SalesModelCode: $("[name='SalesModelCode']").val(),
                        SalesModelYear: $("[name='SalesModelYear']").val(),
                        GroupPriceCode: $("[name='GroupPriceCode']").val()
                    };
                    var url = "its.api/SalesModel/LoadPriceList";

                    widget.post(url, params, function (result) {
                        if (widget.isNullOrEmpty(result.data) == false) {
                            var beforeDiscTotal = $("[name='BeforeDiscTotal']");
                            var beforeDiscTotalPrice = result.data.Total.toString();

                            $("[name='DPP']").val(result.data.DPP.toString());
                            $("[name='PPn']").val(result.data.PPn.toString());
                            $("[name='PPnBM']").val(result.data.PPnBM.toString());
                            beforeDiscTotal.val(beforeDiscTotalPrice);

                            vars["BeforeDiscPPn"] = result.data.PPn.toString();
                            vars["BeforeDiscPPnBM"] = result.data.PPnBM.toString();
                            vars["BeforeDiscDPP"] = result.data.DPP.toString();
                            vars["BeforeDiscTotal"] = result.data.Total.toString();

                            $(".number").blur();
                        }
                    });
                }
            },
            dynamicParams: [
                { name: "SalesModelCode", element: "SalesModelCode" },
                { name: "GroupPriceCode", element: "GroupPriceCode" }
            ]
        });
        widget.klookup({
            name: "btnBrowse",
            title: "Sales Order List",
            url: "its.api/grid/SalesOrderList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterSONumber", text: "No. SO", cls: "span2" },
                    ]
                },
            ],
            columns: [
                { field: "SONumber", title: "No. SO", width: 120 },
                { field: "SaleType", title: "Tipe", width: 100 },
                { field: "SODate", title: "Tanggal SO", width: 120, template: "#= SODate==null ? '' : moment(SODate).format('DD MMM YYYY') #" },
                { field: "SKPKNumber", title: "No. SKPK", width: 120 },
                { field: "ReffNumber", title: "No. Reff.", width: 120 },
                { field: "ReffDate", title: "Tanggal Reff.", width: 100, template: "#= ReffDate==null ? '' : moment(ReffDate).format('DD MMM YYYY') #" },
                { field: "Customer", title: "Pelanggan", width: 300 },
                { field: "CustomerAddress", title: "Alamat", width: 500 },
                { field: "SOStatus", title: "Status", width: 100 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    if (widget.isNullOrEmpty(data.SODate) == false) {
                        data.SODate = moment(data.SODate).format("DD-MMM-YYYY");
                    }
                    if (widget.isNullOrEmpty(data.ReffDate) == false) {
                        data.ReffDate = moment(data.ReffDate).format("DD-MMM-YYYY");
                    }
                    widget.populate(data, function () {
                        //if (widget.isNullOrEmpty(data.ContractDate) == false) {
                        //    $("[name='ContractDate']").val(moment(data.RequestDate).format(SimDms.dateFormat || "DD-MMM-YYYY"));
                        //}
                        //if (widget.isNullOrEmpty(data.SODate) == false) {
                        //    $("[name='SODate']").val(moment(data.SODate).format(SimDms.dateFormat || "DD-MMM-YYYY"));
                        //}
                        //if (widget.isNullOrEmpty(data.ReffDate) == false) {
                        //    $("[name='ReffDate']").val(moment(data.ReffDate).format(SimDms.dateFormat || "DD-MMM-YYYY"));
                        //}
                        //if (widget.isNullOrEmpty(data.FinalPaymentDate) == false) {
                        //    $("[name='PaidPaymentDate']").val(moment(data.FinalPaymentDate).format(SimDms.dateFormat || "DD-MMM-YYYY"));
                        //}
                    });

                    if (widget.isNullOrEmpty(data.ITSNumber)) {
                        var btnITSNumber = $("#btnITSNumber");
                        var textITSNumber = $("[name='ITSNumber']");

                        btnITSNumber.attr("disabled", true);
                        textITSNumber.removeAttr("required");

                        $("[name='ITSNumber']").val("");
                        $("[name='VehicleType']").val("");
                        $("[name='SKPKNumber']").val("");
                        $("[name='ReceiverCode']").val(data.ShipToCode);
                        $("[name='ReceiverName']").val(data.ShipToName);
                    }

                    initGrid();
                    widget.showToolbars(["btnNew", "btnBrowse", "btnSave", "btnCustomer", "btnApprove", "btnUnapprove", "btnReject", "btnPrint"]);

                    vars["EditStatusDetailSalesModel"] = false;
                    vars["EditStatusDetailColourModel"] = false;
                    showTab(false);

                    console.log(data);
                }
            },
            dynamicParams: []
        });

        widget.klookup({
            name: "btnColourCode",
            title: "Colour Model",
            url: "its.api/grid/ColourModelList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterColourCode", text: "Kode", cls: "span2" },
                        { name: "filterColourDesc", text: "Deskripsi", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "ColourCode", title: "Tahun", width: 100 },
                { field: "ColourDesc", title: "Deskripsi", width: 150 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='ColourCode']").val(data.ColourCode);
                    $("[name='ColourDesc']").val(data.ColourDesc);
                }
            },
            dynamicParams: [
                { name: "InquiryNumber", element: "ITSNumber" },
                { name: "SalesModelCode", element: "SalesModelCode" },
            ]
        });

        widget.klookup({
            name: "btnChassisNo",
            title: "Chassis List",
            url: "its.api/grid/ChassisList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterChassisNo", text: "Kode Rangka", cls: "span4" },
                        { name: "filterEngineCode", text: "Kode Mesin", cls: "span4" },
                        { name: "filterEngineNo", text: "No. Mesin", cls: "span4" },
                        { name: "filterServiceBookNo", text: "No. Buku Service", cls: "span4" },
                        { name: "filterKeyNo", text: "No. Kunci", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "ChassisNo", title: "Kode Rangka", width: 150 },
                { field: "EngineCode", title: "Kode Mesin", width: 150 },
                { field: "EngineNo", title: "No. Mesin", width: 150 },
                { field: "ServiceBookNo", title: "No. Buku Service", width: 150 },
                { field: "KeyNo", title: "No. Kunci", width: 150 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    $("[name='ChassisNo']").val(data.ChassisNo);
                }
            },
            dynamicParams: [
                { name: "SalesModelCode", element: "SalesModelCode" },
                { name: "SalesModelYear", element: "SalesModelYear" },
                { name: "ChassisCode", element: "ChassisCode" },
                { name: "ColourCode", element: "ColourCode" },
                { name: "WarehouseCode", element: "WarehouseCode" },
            ]
        });

        widget.klookup({
            name: "btnSupplierCode",
            title: "Supplier BNN",
            url: "its.api/grid/SupplierBNNList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterSupplierCode", text: "Kode Supplier", cls: "span4" },
                        { name: "filterSupplierName", text: "Nama Supplier", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "SupplierCode", title: "Kode Supplier", width: 150 },
                { field: "SupplierName", title: "Nama Supplier", width: 350 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            },
            dynamicParams: [
                { name: "SalesModelCode", element: "SalesModelCode" },
                { name: "SalesModelYear", element: "SalesModelYear" },
            ]
        });

        widget.klookup({
            name: "btnCityCode",
            title: "City List",
            url: "its.api/grid/CityList",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterCityCode", text: "Kode Kota", cls: "span4" },
                        { name: "filterCityName", text: "Nama Kota", cls: "span4" },
                    ]
                },
            ],
            columns: [
                { field: "CityCode", title: "Kode Kota", width: 150 },
                { field: "CityName", title: "Nama Kota", width: 250 },
                { field: "BBN", title: "BBN", width: 150 },
                { field: "KIR", title: "KIR", width: 150 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            },
            dynamicParams: [
                { name: "SalesModelCode", element: "SalesModelCode" },
                { name: "SalesModelYear", element: "SalesModelYear" },
                { name: "SupplierCode", element: "SupplierCode" },
            ]
        });
    }

    function initGrid() {
        reloadGridSalesModel();
        reloadGridColourModel();
        reloadGridOthers();
    }

    function reloadGridSalesModel() {
        var params = {
            SONumber: $("[name='SONumber']").val()
        };

        widget.kgrid({
            url: "its.api/Grid/SavedSalesModelList",
            name: "gridSalesModel",
            params: params,
            columns: [
                { field: "SalesModelCode", title: "Sales Model Code", width: 150 },
                { field: "SalesModelYear", title: "Sales Model Year", width: 150 },
                { field: "QuantitySO", title: "Jumlah SO", width: 100 },
                { field: "AfterDiscTotal", title: "Harga Setelah Diskon", width: 150 },
                { field: "AfterDiscDPP", title: "DPP Setelah Diskon", width: 150 },
                { field: "AfterDiscPPn", title: "PPn Setelah Diskon", width: 150 },
                { field: "AfterDiscPPnBM", title: "PPnBM Setelah Diskon", width: 150 },
                { field: "OthersDPP", title: "DPP Lain-Lain", width: 150 },
                { field: "OthersPPn", title: "PPn Lain-Lain", width: 150 },
                { field: "ShipAmt", title: "Ongkos Kirim", width: 150 },
                { field: "DepositAmt", title: "Deposit", width: 150 },
                { field: "OthersAmt", title: "Lain-Lain", width: 150 },
                { field: "Remark", title: "Keterangan", width: 350 },
            ],
            onDblClick: function (a, b, c) {
                $("#btnGridSalesModelEdit").click();
            },
            toolbars: [
                { name: "btnGridSalesModelInsert", text: "Save", icon: "icon-save" },
                { name: "btnGridSalesModelDelete", text: "Delete", icon: "icon-trash" },
                { name: "btnGridSalesModelEdit", text: "Modify Details", icon: "icon-edit" },
            ],
        }, function () {
            $("#btnGridSalesModelInsert").on("click", function () {
                var rawData = widget.getForms();
                var data = $.extend(rawData, vars);
                var url = "its.api/SalesModel/Save";
                data.ShipAmt = removeNumberFormat(data.ShipAmt);
                data.DepositAmt = removeNumberFormat(data.DepositAmt);
                data.OthersAmt = removeNumberFormat(data.OthersAmt);
                data.AfterDiscTotal = removeNumberFormat(data.AfterDiscTotal);
                data.BeforeDiscTotal = removeNumberFormat(data.BeforeDiscTotal);
                data.DPP = removeNumberFormat(data.DPP);
                data.PPn = removeNumberFormat(data.PPn);
                data.PPnBM = removeNumberFormat(data.PPnBM);
                data["ChassisCode"] = vars["ChassisCode"];
                data.SalesModelCode = $("[name='SalesModelCode']").val();

                widget.post(url, data, function (result) {
                    if (result.status) {
                        reloadGridSalesModel();
                        clearFormSalesModel();
                    }

                    widget.showNotification(result.message);
                });

                return false;
            });

            $("#btnGridSalesModelDelete").on("click", function () {
                widget.selectedRow("gridSalesModel", function (data) {
                    data.parent = null;
                    data.__proto__ = null;
                    data._events = null;

                    widget.confirm("Apakah anda yakin akan menghapus data ini ?", function (result) {
                        if (result == "Yes") {
                            widget.post("its.api/SalesModel/Delete", data, function (result) {
                                if (result.status) {
                                    reloadGridSalesModel();
                                    clearFormSalesModel();
                                    $("[name='SalesModelCode']").val("");
                                    $("[name='SalesModelYear']").val("");
                                    $("[name='SalesModelDesc']").val("");
                                }

                                widget.showNotification(result.message);
                            });
                        }
                    })
                });
            });

            $("#btnGridSalesModelEdit").on("click", function () {
                var _this = $(this);
                widget.selectedRow("gridSalesModel", function (data) {
                    if (data != undefined && widget.isNullOrEmpty(data.SONo) == false) {
                        clearFormSalesModel();
                        showTab(true);
                        vars["SalesModelCode"] = data.SalesModelCode;
                        $("[name='SalesModelCode']").val(data.SalesModelCode);
                        $("[name='SalesModelYear']").val(data.SalesModelYear);
                        $("[name='ChassisCode']").val(data.ChassisCode);
                        reloadGridColourModel();
                    }
                    else {
                        showTab(false);
                    }
                });

                return false;
            });
        });
    }

    function reloadGridColourModel() {
        var params = {
            SalesModelCode: $("[name='SalesModelCode']").val(),
            SalesModelYear: $("[name='SalesModelYear']").val(),
            SONumber: $("[name='SONumber']").val()
        };

        widget.kgrid({
            url: "its.api/Grid/SavedColourModelList",
            name: "gridColourModel",
            params: params,
            columns: [
                { field: "ColourCode", title: "Warna", width: 150 },
                { field: "Quantity", title: "Jumlah", width: 120 },
                { field: "Remark", title: "Keterangan", width: 250 },
            ],
            onDblClick: function (a, b, c) {
                $("#btnGridColourModelEdit").click();
            },
            toolbars: [
                { name: "btnGridColourModelInsert", text: "Save", icon: "icon-save" },
                { name: "btnGridColourModelDelete", text: "Delete", icon: "icon-trash" },
                { name: "btnGridColourModelEdit", text: "Modify Details", icon: "icon-edit" },
            ],
        }, function () {
            $("#btnGridColourModelInsert").on("click", function () {
                var params = widget.getForms();
                var url = "its.api/ColourModel/Save";

                widget.post(url, params, function (result) {
                    if (result.status) {
                        reloadGridSalesModel();
                        reloadGridColourModel();
                        clearFormColourModel();
                    }

                    widget.showNotification(result.message);
                });

                return false;
            });

            $("#btnGridColourModelDelete").on("click", function () {
                widget.selectedRow("gridColourModel", function (data) {
                    if (widget.isNullOrEmpty(data) == false) {
                        widget.confirm("Apakah anda ingin menghapus data ini ?", function (result) {
                            if (result == "Yes") {
                                data.parent = null;
                                data.__proto__ = null;
                                data._events = null;
                                var url = "its.api/ColourModel/Delete";

                                widget.post(url, data, function (result) {
                                    if (result.status) {
                                        reloadGridSalesModel();
                                        reloadGridColourModel();

                                        clearFormColourModel();
                                    }

                                    widget.showNotification(result.message);
                                });
                            }
                        });
                    }
                });

                return false;
            });

            $("#btnGridColourModelEdit").on("click", function () {
                widget.selectedRow("gridColourModel", function (data) {
                    if (widget.isNullOrEmpty(data) == false) {
                        clearFormColourModel();
                        $("[name='ColourCode']").val(data.ColourCode || "");
                        //$("[name='ColourDesc']").val(data.ColourDesc || "");
                        //$("[name='RemarkColour']").val(data.Remark || "");
                        //widget.populate({
                        //    Quantity: data.Quantity || 0
                        //});
                        reloadGridOthers();
                    }
                });

                return false;
            });
        });
    }

    function reloadGridOthers() {
        var params = {
            SalesModelCode: $("[name='SalesModelCode']").val(),
            SalesModelYear: $("[name='SalesModelYear']").val(),
            SONumber: $("[name='SONumber']").val(),
            ColourCode: $("[name='ColourCode']").val()
        };

        widget.kgrid({
            url: "its.api/Grid/OthersList",
            name: "gridOthers",
            params: params,
            columns: [
                { field: "ChassisCode", title: "Kode Rangka", width: 120 },
                { field: "ChassisNo", title: "Nomor Rangka", width: 120 },
                { field: "EndUserName", title: "STNK Name", width: 200 },
                { field: "EndUserAddress1", title: "STNK Address 1", width: 250 },
                { field: "EndUserAddress2", title: "STNK Address 2", width: 250 },
                { field: "EndUserAddress3", title: "STNK Address 3", width: 250 },
                { field: "SupplierBBN", title: "Supplier BBN", width: 200 },
                { field: "CityCode", title: "Kota", width: 100 },
                { field: "BBN", title: "BBN", width: 100 },
                { field: "Remark", title: "Keterangan", width: 250 },
            ],
            onDblClick: function (a, b, c) {
            },
            toolbars: [
                { name: "btnGridOthersInsert", text: "Save", icon: "icon-save" },
                { name: "btnGridOthersDelete", text: "Delete", icon: "icon-trash" },
            ],
        }, function () {
            $("#btnGridOthersInsert").on("click", function () {
                var params = widget.getForms();
                var url = "its.api/SOOther/Save";

                widget.post(url, params, function (result) {
                    if (result.status) {
                        reloadGridSalesModel();
                        reloadGridColourModel();
                        reloadGridOthers();
                        clearFormOthers();
                    }

                    widget.showNotification(result.message);
                });

                return false;
            });

            $("#btnGridOthersDelete").on("click", function () {
                widget.selectedRow("gridOthers", function (data) {
                    if (widget.isNullOrEmpty(data) == false) {
                        widget.confirm("Apakah anda ingin menghapus data ini ?", function (result) {
                            if (result == "Yes") {
                                data.parent = null;
                                data.__proto__ = null;
                                data._events = null;
                                var url = "its.api/SOOther/Delete";

                                widget.post(url, data, function (result) {
                                    if (result.status) {
                                        reloadGridSalesModel();
                                        reloadGridColourModel();
                                        reloadGridOthers();
                                        clearFormOthers();
                                    }

                                    widget.showNotification(result.message);
                                });
                            }
                        });
                    }
                });

                return false;
            });
        });
    }

    function reloadGridAccesories() {

    }

    function reloadGridSpareparts() {

    }

    function isDirectSales_changed(e, val) {
        var btnITSNumber = $("#btnITSNumber");
        var textITSNumber = $("[name='ITSNumber']");

        if (val == true || val == "true") {
            btnITSNumber.removeAttr("disabled");
            textITSNumber.attr("required", true);
        }
        else if (val == false || val == "false") {
            btnITSNumber.attr("disabled", true);
            textITSNumber.removeAttr("required");

            $("[name='SalesmanCode']").val("");
            $("[name='SalesmanName']").val("");
            $("[name='ITSNumber']").val("");
            $("[name='VehicleType']").val("");
        }
    }

    function IsLeasing_changed(e, val) {
        var textLeasingCode = $("[name='LeasingCode']");
        var btnLeasingCode = $("#btnLeasingCode");
        var textLeasingName = $("[name='LeasingName']");
        var cmbTenor = $("[name='Tenor']");

        if (val == true || val == "true") {
            btnLeasingCode.removeAttr("disabled");
            cmbTenor.removeAttr("disabled");
            textLeasingCode.attr("required", true);
        }
        else if (val == false || val == "false") {
            btnLeasingCode.attr("disabled", true);
            cmbTenor.attr("disabled", true);
            textLeasingCode.removeAttr("required");
            textLeasingCode.removeClass("error");

            textLeasingCode.val("");
            textLeasingName.val("");
            cmbTenor.val("");
        }
    }

    function saveSalesOrder() {
        var reffNumber = $("[name='ReffNumber']").val();
        var reffDateElement = $("[name='ReffDate']");
        if (widget.isNullOrEmpty(reffNumber)) {
            reffDateElement.removeAttr("required");
            reffDateElement.removeClass("error");
        }
        else {
            reffDateElement.attr("required", true);
        }

        var validation = widget.validate();
        if (validation) {
            var url = "its.api/SalesOrder/Save";
            var params = widget.getForms();
            params.Advance = removeNumberFormat(params.Advance || "0");

            widget.post(url, params, function (result) {
                if (result.status == true || result.status == "true") {
                    widget.populate(result.data);
                    widget.showToolbars(["btnNew", "btnBrowse", "btnSave", "btnApprove", "btnUnapprove", "btnReject", "btnPrint", "btnCustomer"]);
                    setTimeout(function () {
                        initGrid();
                    }, 500);
                }

                widget.showNotification(result.message);
            });
        }
    }

    function showSubGrid(e, name) {
        var tabName = $(e).attr("data-name");

        widget.hidePanel([
            {
                name: "gridSalesModel",
                type: "kgrid"
            },
            {
                name: "gridColourModel",
                type: "kgrid"
            },
            {
                name: "gridOthers",
                type: "kgrid"
            }
        ]);

        if (tabName == "tabSalesModel") {
            widget.showPanel([
                {
                    name: "gridSalesModel",
                    type: "kgrid"
                }
            ]);
        }

        if (tabName == "tabVehicleInformation") {
            widget.showPanel([
                {
                    name: "gridColourModel",
                    type: "kgrid"
                },
                {
                    name: "gridOthers",
                    type: "kgrid"
                }
            ]);
        }

        if (tabName == "tabAccessories") {
            widget.showPanel([
                {
                    name: "gridAccesories",
                    type: "kgrid"
                }
            ]);
        }

        if (tabName == "tabSparepart") {
            widget.showPanel([
                {
                    name: "gridSpareparts",
                    type: "kgrid"
                }
            ]);
        }
    }

    function showTab(state) {
        if (state) {
            $("[data-name='tabVehicleInformation']").show();
            //$("[data-name='tabAccessories']").show();
            //$("[data-name='tabSparepart']").show();
        }
        else {
            $("[data-name='tabVehicleInformation']").hide();
            $("[data-name='tabAccessories']").hide();
            $("[data-name='tabSparepart']").hide();
        }
    }

    function removeNumberFormat(paramValue) {
        return paramValue.toString().replace(",", "").replace(",", "").replace(",", "").replace(",", "").replace(",", "");
    }

    function clearFormColourModel() {
        $("[name='ColourCode']").val("");
        $("[name='ColourDesc']").val("");
        $("[name='RemarkColour']").val("");
        widget.populate({
            Quantity: 0
        });
    }

    function clearFormSalesModel(state) {
        if (state == "1") {
            $("#panelSalesModel1 input, #panelSalesModel1 textarea, [name='SalesModelDesc']").val("");
            $("[name='SalesModelDesc']").val("");
            var params = {
                SalesModelCode: "",
                SalesModelYear: "",
                SalesModelDesc: ""
            };
            widget.populate(params);
        }
        else if (state == "2") {
            $("#panelSalesModel2 input, #panelSalesModel2 textarea").val("");
        }
        else {
            $("[name='SalesModelDesc']").val("");
            $("#panelSalesModel1 input, #panelSalesModel2 input, #panelSalesModel1 textarea, #panelSalesModel2 textarea, [name='SalesModelDesc']").val("");
            var params = {
                SalesModelCode: "",
                SalesModelYear: "",
                SalesModelDesc: ""
            };
            widget.populate(params);
        }
    }

    function clearFormOthers() {
        $("[name='ChassisNo']").val("");
        $("[name='STNKName']").val("");
        $("[name='STNKAddress1']").val("");
        $("[name='STNKAddress2']").val("");
        $("[name='STNKAddress3']").val("");
        $("[name='SupplierCode']").val("");
        $("[name='SupplierName']").val("");
        $("[name='BBN']").val("");
        $("[name='KIR']").val("");
        $("[name='RemarkOther']").val("");
        $("[name='CityCode']").val("");
        $("[name='CityName']").val("");
    }
});