$(document).ready(function () {
    var widgetOptions = {
        title: "Customer Admin",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file", cls: "" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "" },
            { name: "btnBrowse", text: "Browse", icon: "icon-find", cls: "" },
        ],
        panels: [
            {
                name: "Customer ",
                title: "Personal Information",
                items: [
                    {
                        name: "CustomerCategory", type: "controls", text: "Customer", items: [
                            { name: "CustomerCode", type: "text", text: "Customer Code", cls: "span2", readonly: true },
                            { name: "CustomerName", type: "text", text: "Customer Name", cls: "span6", required: true, required: true },
                        ]
                    },
                    { name: "StandardCode", type: "text", text: "Standard Code", cls: "span4", readonly: true },
                    { name: "CustomerAbbrName", type: "", text: "Customer Abbr Name", cls: "span4", },
                    { name: "IsPKP", type: "switch", text: "Faktur Pajak Std?", cls: "span4", float: "left" },
                    { name: "Gender", type: "select", text: "Gender", cls: "span4", required: true },
                    { name: "CustomerType", type: "select", text: "Customer Type", cls: "span4" },
                    { name: "HPNo", type: "text", text: "HP No", cls: "span4", required: true },
                    {
                        name: "CustomerCategory", type: "controls", text: "Customer Category", items: [
                            { name: "CategoryCode", type: "popup", text: "Category Code", cls: "span3", readonly: true },
                            { name: "CategoryName", type: "text", text: "Category Name", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        name: "Telephone", type: "controls", text: "Phone No", items: [
                            { name: "PhoneNo", type: "text", text: "Phone No", cls: "span3" },
                            { name: "OfficePhoneNo", type: "text", text: "Office Phone No", cls: "span5" },
                        ]
                    },
                    { name: "CustomerGovName", type: "", text: "Customer Gov Name", cls: "span8" },
                    { name: "FaxNo", type: "", text: "Fax No", cls: "span4 full" },
                    { name: "NPWPNo", type: "text", text: "NPWP No", cls: "span4" },
                    { name: "NPWPDate", type: "datepicker", text: "NPWP Date", cls: "span4" },
                    { name: "Email", type: "", text: "Email", cls: "span4" },
                    { name: "BirthDate", type: "datepicker", text: "Birth Date", cls: "span4" },
                    { name: "SKPNo", type: "text", text: "SKP No", cls: "span4" },
                    { name: "SKPDate", type: "datepicker", text: "SKP Date", cls: "span4" },
                    {
                        name: "Kode Pos", type: "controls", text: "Postal Code", items: [
                            { name: "PosCode", type: "popup", text: "Pos Code", cls: "span3", readonly: true, required: true },
                            { name: "PosName", type: "text", text: "Pos Name", cls: "span5", readonly: true },
                        ]
                    },
                    { name: "Address1", type: "", text: "Address 1", cls: "span4", validasi: "max(10)", maxlength: "40" },
                    { name: "ProvinceCode", type: "", text: "Province", cls: "span4", readonly: true },
                    { name: "Address2", type: "", text: "Address 2", cls: "span4", required: true, maxlength: "40" },
                    { name: "CityCode", type: "", text: "City", cls: "span4", readonly: true },
                    { name: "Address3", type: "", text: "Address 3", cls: "span4", maxlength: "40" },
                    { name: "AreaCode", type: "", text: "Area", cls: "span4", readonly: true },
                    { name: "Address4", type: "", text: "Address 4", cls: "span4", maxlength: "40" },
                    { name: "Status", type: "select", text: "Customer Status", cls: "span4" },
                ]
            },
            {
                xtype: "tabs",
                name: "tabCustomer",
                items: [
                    { name: "tabProfitCenter", text: "Profit Center", cls: "active" },
                    { name: "tabProductDiscount", text: "Discount Product" },
                    { name: "tabBank", text: "Bank" },
                ],
                onChanged: showGrid
            },
            {
                name: "tabProfitCenter",
                cls: "tabCustomer tabProfitCenter",
                items: [
                     {
                         type: "controls", text: "Profit Center", items: [
                             { name: "ProfitCenterCode", type: "popup", text: "Profit Center", cls: "span3", readonly: true, required: false },
                             { name: "ProfitCenterName", type: "", text: "Description", cls: "span5", readonly: true },
                         ],
                     },
                    {
                        type: "controls", text: "Customer Class", items: [
                            { name: "CustomerClass", type: "popup", text: "Customer Class", cls: "span3", readonly: true },
                            { name: "CustomerClassName", type: "", text: "Customer Class Name", cls: "span5", readonly: true },
                        ],
                    },
                    { name: "PaymentCode", text: "Payment Tupe", cls: "span4 full", type: "select" },
                    {
                        type: "controls", text: "Tax", items: [
                            { name: "TaxCode", type: "popup", text: "Tax Code", cls: "span3", readonly: true },
                            { name: "TaxDesc", type: "", text: "Tax Desc", cls: "span5", readonly: true },
                        ],
                    },
                    {
                        type: "controls", text: "Collector", items: [
                            { name: "CollectorCode", type: "popup", text: "Collector Code", cls: "span3", readonly: true },
                            { name: "CollectorName", type: "", text: "Collector Name", cls: "span5", readonly: true },
                        ],
                    },
                    {
                        type: "controls", text: "Tax Transaction", items: [
                            { name: "TaxTransCode", type: "popup", text: "Tax Trans Code", cls: "span3", readonly: true },
                            { name: "TaxTransDesc", type: "", text: "Tax Trans Desc", cls: "span5", readonly: true },
                        ],
                    },
                    {
                        type: "controls", text: "Salesman", items: [
                            { name: "Salesman", type: "popup", text: "Salesman", cls: "span3", readonly: true },
                            { name: "SalesmanName", type: "", text: "Salesman Name", cls: "span5", readonly: true },
                        ],
                    },
                    {
                        type: "controls", text: "AR", items: [
                            { name: "KelAR", type: "popup", text: "AR", cls: "span3", readonly: true },
                            { name: "KelARDesc", type: "", text: "AR Desc", cls: "span5", readonly: true },
                        ],
                    },
                    { name: "CreditLimit", text: "Credit Limit", cls: "span4 full", type: "text" },
                    { name: "IsOverDueAllowed", text: "Overdue", cls: "span4 full", type: "switch", float: "left" },
                    { name: "IsBlackList", text: "Black List", cls: "span4 full", type: "switch", float: "left" },
                    {
                        type: "controls", text: "Customer Grade", items: [
                            { name: "CustomerGrade", type: "popup", text: "Customer Grade", cls: "span3", readonly: true },
                            { name: "CustomerGradeName", type: "", text: "Customer Grade Name", cls: "span5", readonly: true },
                        ],
                    },
                    { name: "ContactPerson", type: "text", text: "Contact Person", cls: "span4 full" },
                    { name: "TOPCode", type: "select", text: "TOP", cls: "span4 full" },
                    {
                        type: "controls", text: "Group Price", items: [
                            { name: "GroupPrice", type: "popup", text: "Group Price", cls: "span3", readonly: true },
                            { name: "GroupPriceDesc", type: "", text: "Group Price Desc", cls: "span5", readonly: true },
                        ],
                    },
                    { name: "DiscPct", type: "decimal", text: "Discount", cls: "span4 full" },
                    {
                        name: "SalesType", type: "select", text: "Sales Type", cls: "span4 full", items: [
                            { value: "0", text: "Whole Sales" },
                            { value: "1", text: "Direct Sales" },
                        ]
                    },
                    { name: "LaborDiscPct", type: "decimal", text: "Labor Disc Pct", cls: "span4 full" },
                    { name: "PartDiscPct", type: "decimal", text: "Part Disc Pct", cls: "span4 full" },
                    { name: "MaterialDiscPct", type: "decimal", text: "Material Disc Pct", cls: "span4 full" },

                ]
            },
            {
                name: "tabProductDiscount",
                cls: "tabCustomer tabProductDiscount",
                items: [
                     {
                         type: "controls", text: "Profit Center", items: [
                             { name: "ProfitCenterCodeDisc", text: "Code", type: "popup", cls: "span3", readonly: true, required: false },
                             { name: "ProfitCenterNameDisc", text: "Desc", type: "", cls: "span5", readonly: true, required: false },
                         ]
                     },
                    {
                        type: "controls", text: "Part Type", items: [
                            { name: "TypeOfGoodsDisc", text: "Code", type: "popup", cls: "span3", readonly: true, required: false },
                            { name: "TypeOfGoodsNameDisc", text: "Desc", type: "", cls: "span5", readonly: true, required: false },
                        ]
                    },
                    { name: "DiscPctDisc", type: "decimal", cls: "span3", required: false, text: "Discount" }
                ]
            },
            {
                name: "tabBank",
                cls: "tabCustomer tabBank",
                items: [
                    {
                        type: "controls", text: "Bank", items: [
                            { name: "BankCode", type: "popup", text: "Bank Code", cls: "span3", readonly: true, required: true },
                            { name: "BankName", type: "", text: "Bank Name", cls: "span5", readonly: true, required: true },
                        ]
                    },
                    { name: "AccountName", type: "text", text: "Account Name", cls: "span8", required: false },
                    { name: "AccountBank", type: "text", text: "Account Bank", cls: "span8", required: false },
                ]
            },
            {
                xtype: "kgrid",
                name: "gridProfitCenter"
            },
            {
                xtype: "kgrid",
                name: "gridProductDiscount"
            },
            {
                xtype: "kgrid",
                name: "gridBank"
            }
        ]
    };
    var widget = new SimDms.Widget(widgetOptions);

    var optionSelect = [
        { name: "Gender", url: "gn.api/Combo/CustomerGenders" },
        { name: "CustomerType", url: "gn.api/Combo/CustomerTypes" },
        { name: "Status", url: "gn.api/Combo/CustomerStatuses" },
        { name: "PaymentCode", url: "gn.api/Combo/PaymentTypes" },
        { name: "TOPCode", url: "gn.api/Combo/TOP" },
    ];
    widget.setSelect(optionSelect);

    widget.render(renderCallback);

    function renderCallback() {
        initElementEvents();
        initLookup();
        initGrid();
        initDefaultValue();
        setCustomerCode();
    }

    function setCustomerCode() {
        widget.post("gn.api/Customer/SetCustomerCode", function (result) {
            if (result.data == true) {
                $('#CustomerCode, #StandardCode').attr('disabled', 'disabled');
                $('#CustomerCode, #StandardCode').val('*******');
            }
            else {
                $('#CustomerCode, #StandardCode').removeAttr('disabled');
                $('#CustomerCode, #StandardCode').val('');
            }
        });
    }
   
    function initDefaultValue() {
        widget.populate({
            IsPKP: true,
            TaxCode: "PPN",
            TaxDesc: "PAJAK PERTAMBAHAN NILAI"
        });
    }

    function initElementEvents() {
        var btnNew = $("#btnNew");
        var btnSave = $("#btnSave");
        var btnBrowse = $("#btnBrowse");
        var comboPaymentCode = $("[name='PaymentCode']");

        btnNew.off();
        btnNew.on("click", function () {
            clearAllForm();
            initGrid();
        });

        btnSave.off();
        btnSave.on("click", function () {
            var validation = widget.validate();
            if (validation) {
                var data = widget.getForms();
                var url = "gn.api/Customer/Save";
                widget.post(url, data, function (result) {
                    if (result.status) {
                        if (widget.isNullOrEmpty(result.data) == false) {
                            widget.populate(result.data);
                        }
                    }
                    widget.showNotification(result.message);
                });
            }
        });

        btnBrowse.off();
        btnBrowse.on("click", function () {

        });

        comboPaymentCode.on("change", function () {
            var currentValue = $(this).val();
            var isShowedAll = false;
            if (currentValue == "CR") {
                isShowedAll = true;
            }
            else if (currentValue == "CS") {
                isShowedAll = false;
            }

            widget.select({
                name: "TOPCode",
                url: "gn.api/combo/TOP",
                params: {
                    IsShowedAll: isShowedAll
                }
            });
        });
    }

    function initLookup() {
        widget.klookup({
            name: "btnBrowse",
            title: "Customer",
            url: "gn.api/Lookup/Customers",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterCustomerCode", text: "Customer Code", cls: "span3" },
                        { name: "filterCustomerName", text: "Customer Name", cls: "span5" },
                    ]
                },
            ],
            columns: [
                { field: "CustomerCode", title: "Customer Code", width: 120 },
                { field: "CustomerName", title: "Customer Name", width: 200 },
                { field: "Address", title: "Address", width: 350 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    var url = "gn.api/Customer/Get";
                    var params = {
                        CustomerCode: data.CustomerCode
                    };

                    reloadGridProfitCenter({
                        CustomerCode: data.CustomerCode
                    });
                    reloadGridProductDiscount({
                        CustomerCode: data.CustomerCode
                    });
                    reloadGridBank({
                        CustomerCode: data.CustomerCode
                    });

                    widget.post(url, params, function (result) {
                        if (result.status == true || result.status == "true") {
                            var record = $.extend(result.data, {
                                IsPKP: result.data.isPKP,
                                Status: widget.isNullOrEmpty(result.data.Status) == true ? "" : result.data.Status
                            });
                            clearAllForm("1");
                            widget.populate(record);

                            widget.post("gn.api/Customer/GetCustomerCategory", { CategoryCode: record.CategoryCode }, function (result2) {
                                widget.populate(result2);
                            });
                            widget.post("gn.api/Customer/GetPostalCode", record.ZipNo , function (result2) {
                                widget.populate(result2);
                            });
                        }
                    });
                }
            }
        });

        widget.klookup({
            name: "btnCategoryCode",
            title: "Customer Category",
            url: "gn.api/Lookup/CustomerCategories",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterCategoryCode", text: "Category Code", cls: "span3" },
                        { name: "filterCategoryName", text: "Category Name", cls: "span5" },
                    ]
                },
            ],
            columns: [
                { field: "CategoryCode", title: "Category Code", width: 120 },
                { field: "CategoryName", title: "Category Name", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnPosCode",
            title: "Postal Code",
            url: "gn.api/Lookup/ZipCodes",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterPosCode", text: "Pos Code", cls: "span3" },
                    ]
                },
            ],
            columns: [
                { field: "PosCode", title: "Postal Code", width: 120 },
                { field: "PosName", title: "District", width: 200 },
                { field: "AreaCode", title: "Sub District", width: 200 },
                { field: "CityCode", title: "City", width: 200 },
                { field: "ProvinceCode", title: "Province", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnProfitCenterCode",
            title: "Profit Center",
            url: "gn.api/Lookup/ProfitCenters",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterProfitCenterCode", text: "Profit Center Code", cls: "span3" },
                        { name: "filterProfitCenterName", text: "Profit Center Name", cls: "span3" },
                    ]
                },
            ],
            columns: [
                { field: "ProfitCenterCode", title: "Profit Center Code", width: 120 },
                { field: "ProfitCenterName", title: "Profit Center Name", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnCustomerClass",
            title: "Kelas Pelanggan",
            url: "gn.api/Lookup/CustomerClasses",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterCustomerClass", text: "Customer Class", cls: "span3" },
                        { name: "filterCustomerClassName", text: "Customer Class Name", cls: "span3" },
                    ]
                },
            ],
            dynamicParams: [
                { name: "ProfitCenterCode", element: "ProfitCenterCode" }
            ],
            columns: [
                { field: "CustomerClass", title: "Customer Class", width: 120 },
                { field: "CustomerClassName", title: "Customer Class Name", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnTaxCode",
            title: "Jenis Pajak",
            url: "gn.api/Lookup/Taxes",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterTaxCode", text: "Tax Code", cls: "span3" },
                        { name: "filterTaxDesc", text: "Tax Desc", cls: "span3" },
                    ]
                },
            ],
            dynamicParams: [
                { name: "ProfitCenterCode", element: "ProfitCenterCode" }
            ],
            columns: [
                { field: "TaxCode", title: "Tax Code", width: 120 },
                { field: "TaxDesc", title: "Tax Desc", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnCollectorCode",
            title: "Kolektor",
            url: "gn.api/Lookup/Collectors",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterCollectorCode", text: "Collector Code", cls: "span3" },
                        { name: "filterCollectorName", text: "Collector Name", cls: "span5" },
                    ]
                },
            ],
            dynamicParams: [
                { name: "ProfitCenterCode", element: "ProfitCenterCode" }
            ],
            columns: [
                { field: "CollectorCode", title: "Collector Code", width: 120 },
                { field: "CollectorName", title: "Collector Name", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnTaxTransCode",
            title: "Kode Transaksi Pajak",
            url: "gn.api/Lookup/KodeTransaksiPajak",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterTaxTransCode", text: "Tax Trans Code", cls: "span3" },
                        { name: "filterTaxTransDesc", text: "Tax Trans Desc", cls: "span5" },
                    ]
                },
            ],
            dynamicParams: [
            ],
            columns: [
                { field: "TaxTransCode", title: "Tax Trans Code", width: 120 },
                { field: "TaxTransDesc", title: "Tax Trans Desc", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnSalesman",
            title: "Salesman",
            url: "gn.api/Lookup/Salesmans",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterSalesmanCode", text: "Salesman Code", cls: "span3" },
                        { name: "filterSalesmanName", text: "Salesman Name", cls: "span5" },
                    ]
                },
            ],
            dynamicParams: [
                { name: "Department", value: "SALES" },
                { name: "Position", value: "S" },
            ],
            columns: [
                { field: "EmployeeID", title: "Employee ID", width: 120 },
                { field: "EmployeeName", title: "Employee Name", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    var record = $.extend(data, {
                        Salesman: data.EmployeeID,
                        SalesmanName: data.EmployeeName
                    });
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnKelAR",
            title: "Kelompok AR",
            url: "gn.api/Lookup/KelompokAR",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterKelAR", text: "AR", cls: "span3" },
                        { name: "filterKelARDesc", text: "Description", cls: "span5" },
                    ]
                },
            ],
            dynamicParams: [
            ],
            columns: [
                { field: "KelAR", title: "AR", width: 120 },
                { field: "KelARDesc", title: "Description", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnCustomerGrade",
            title: "Grade Pelanggan",
            url: "gn.api/Lookup/CustomerGrades",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterCustomerGrade", text: "Customer Grade", cls: "span3" },
                        { name: "filterCustomerGradeName", text: "Customer Grade Name", cls: "span5" },
                    ]
                },
            ],
            dynamicParams: [
            ],
            columns: [
                { field: "CustomerGrade", title: "Customer Grade", width: 120 },
                { field: "CustomerGradeName", title: "Customer Grade Name", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

        widget.klookup({
            name: "btnGroupPrice",
            title: "Group Price",
            url: "gn.api/Lookup/GroupPrices",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterGroupPrice", text: "Group Price", cls: "span3" },
                        { name: "filterGroupPriceName", text: "Group Price Name", cls: "span5" },
                    ]
                },
            ],
            dynamicParams: [
            ],
            columns: [
                { field: "GroupPrice", title: "Group Price", width: 120 },
                { field: "GroupPriceDesc", title: "Group Price Desc", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                    console.log(data);
                }
            }
        });

        widget.klookup({
            name: "btnProfitCenterCodeDisc",
            title: "Profit Center",
            url: "gn.api/Lookup/ProfitCenters",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterProfitCenterCode", text: "Profit Center Code", cls: "span3" },
                        { name: "filterProfitCenterName", text: "Profit Center Name", cls: "span3" },
                    ]
                },
            ],
            columns: [
                { field: "ProfitCenterCode", title: "Profit Center Code", width: 120 },
                { field: "ProfitCenterName", title: "Profit Center Name", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    var record = {
                        ProfitCenterCodeDisc: data.ProfitCenterCode,
                        ProfitCenterNameDisc: data.ProfitCenterName
                    };
                    widget.populate(record);
                }
            }
        });

        widget.klookup({
            name: "btnTypeOfGoodsDisc",
            title: "Profit Center",
            url: "gn.api/Lookup/TypeOfGoods",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterTypeOfGoods", text: "Type Of Goods", cls: "span3" },
                        { name: "filterTypeOfGoodsName", text: "Type Of Goods Name", cls: "span3" },
                    ]
                },
            ],
            columns: [
                { field: "TypeOfGoods", title: "Type Of Goods", width: 120 },
                { field: "TypeOfGoodsName", title: "Type Of Goods Name", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    var record = {
                        TypeOfGoodsDisc: data.TypeOfGoods,
                        TypeOfGoodsNameDisc: data.TypeOfGoodsName
                    };
                    widget.populate(record);
                }
            }
        });

        widget.klookup({
            name: "btnBankCode",
            title: "Bank",
            url: "gn.api/Lookup/Banks",
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "filterBankCode", text: "Bank Code", cls: "span3" },
                        { name: "filterBankName", text: "Bank Name", cls: "span5" },
                    ]
                },
            ],
            columns: [
                { field: "BankCode", title: "Bank Code", width: 120 },
                { field: "BankName", title: "Bank Name", width: 200 },
            ],
            onSelected: function (data) {
                if (widget.isNullOrEmpty(data) == false) {
                    widget.populate(data);
                }
            }
        });

    }

    function initGrid() {
        reloadGridProfitCenter();
        reloadGridProductDiscount();
        reloadGridBank();
    }

    function clearForm(state) {
        switch (state) {
            case "1": //Clear all form -> main form and child form
                clearAllForm();
                break;

            case "2":

                break;

            case "3":

                break;

            case "4":

                break;

            case "5":

                break;
            default:
                clearAllForm();
        }
    }

    function clearAllForm() {
        widget.clearForm();
        initDefaultValue();
    }

    function reloadGridProfitCenter(data) {
        var params = {};
        if (widget.isNullOrEmpty(data)) {
            params = {
                CustomerCode: $("[name='CustomerCode']").val(),
            };
        }
        else {
            params = data;
        }

        widget.kgrid({
            url: "gn.api/Grid/CustomerProfitCenters",
            name: "gridProfitCenter",
            params: params,
            columns: [
                { field: "ProfitCenterCode", title: "Profit Center Code", width: 120 },
                { field: "ProfitCenterName", title: "Profit Center Desc", width: 200 },
                { field: "CustomerGovName", title: "Tax Name", width: 200 },
                { field: "CustomerClass", title: "Customer Class", width: 120 },
                { field: "ContactPerson", title: "Contact Person", width: 200 },
            ],
            onDblClick: function (a, b, c) {
                widget.selectedRow("gridProfitCenter", function (data) {
                    if (widget.isNullOrEmpty(data.SalesType)) {
                        data.SalesType = "";
                    }
                    clearFormProfitCenter();
                    widget.populate(data);

                    console.log(data);
                });
            },
            toolbars: [
                { name: "btnGridProfitCenterInsert", text: "Save", icon: "icon-save" },
                { name: "btnGridProfitCenterDelete", text: "Delete", icon: "icon-trash" },
            ],
        }, function () {
            $("#btnGridProfitCenterInsert").on("click", function () {
                var params = widget.getForms();
                var url = "om.api/CustomerProfitCenter/Save";

                widget.post(url, params, function (result) {
                    if (result.status) {
                        clearFormProfitCenter();
                        reloadGridProfitCenter();
                    }

                    widget.showNotification(result.message);
                });

                return false;
            });

            $("#btnGridProfitCenterDelete").on("click", function () {
                widget.selectedRow("gridProfitCenter", function (data) {
                    if (widget.isNullOrEmpty(data) == false) {
                        widget.confirm("Apakah anda akan menghapus data terpilih?", function (confirmation) {
                            if (confirmation == "Yes") {
                                data.parent = null;
                                data.__proto__ = null;
                                data._events = null;

                                widget.post("gn.api/CustomerProfitCenter/Delete", data, function (result) {
                                    if (result.status) {
                                        clearFormProfitCenter();
                                        reloadGridProfitCenter();
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

    function reloadGridProductDiscount(data) {
        var params = {};
        if (widget.isNullOrEmpty(data)) {
            params = {
                CustomerCode: $("[name='CustomerCode']").val(),
            };
        }
        else {
            params = data;
        }

        widget.kgrid({
            url: "gn.api/Grid/CustomerDiscounts",
            name: "gridProductDiscount",
            params: params,
            columns: [
                { field: "ProfitCenterCodeDisc", title: "Profit Center Code", width: 120 },
                { field: "ProfitCenterNameDisc", title: "Profit Center Desc", width: 200 },
                { field: "TypeOfGoodsDisc", title: "Part Type", width: 120 },
                { field: "TypeOfGoodsNameDisc", title: "Part Type Desc", width: 200 },
                { field: "DiscPctDisc", title: "Discount", width: 120 },
            ],
            onDblClick: function (a, b, c) {
                widget.selectedRow("gridProductDiscount", function (data) {
                    widget.populate(data);
                });
            },
            toolbars: [
                { name: "btnGridProductDiscountInsert", text: "Save", icon: "icon-save" },
                { name: "btnGridProductDiscountDelete", text: "Delete", icon: "icon-trash" },
            ],
        }, function () {
            $("#btnGridProductDiscountInsert").on("click", function () {
                var params = widget.getForms();
                var url = "om.api/CustomerDiscount/Save";

                widget.post(url, params, function (result) {
                    if (result.status) {
                        clearFormProductDiscount();
                        reloadGridProductDiscount();
                    }

                    widget.showNotification(result.message);
                });

                return false;
            });

            $("#btnGridProductDiscountDelete").on("click", function () {
                widget.selectedRow("gridProductDiscount", function (data) {
                    if (widget.isNullOrEmpty(data) == false) {
                        widget.confirm("Apakah anda akan menghapus data terpilih?", function (confirmation) {
                            if (confirmation == "Yes") {
                                data.parent = null;
                                data.__proto__ = null;
                                data._events = null;

                                widget.post("gn.api/CustomerDiscount/Delete", data, function (result) {
                                    if (result.status) {
                                        clearFormProductDiscount();
                                        reloadGridProductDiscount();
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

    function reloadGridBank(data) {
        var params = {};
        if (widget.isNullOrEmpty(data)) {
            params = {
                CustomerCode: $("[name='CustomerCode']").val(),
            };
        }
        else {
            params = data;
        }

        widget.kgrid({
            url: "gn.api/Grid/CustomerBanks",
            name: "gridBank",
            params: params,
            columns: [
                { field: "BankCode", title: "Bank Code", width: 120 },
                { field: "BankName", title: "Bank Name", width: 200 },
                { field: "AccountBank", title: "Bank Account", width: 120 },
                { field: "AccountName", title: "Account Name", width: 200 },
            ],
            onDblClick: function (a, b, c) {
                widget.selectedRow("gridBank", function (data) {
                    widget.populate(data);
                });
            },
            toolbars: [
                { name: "btnGridProductBankInsert", text: "Save", icon: "icon-save" },
                { name: "btnGridProductBankDelete", text: "Delete", icon: "icon-trash" },
            ],
        }, function () {
            $("#btnGridProductBankInsert").on("click", function () {
                var params = widget.getForms();
                var url = "om.api/CustomerBank/Save";

                widget.post(url, params, function (result) {
                    if (result.status) {
                        clearFormBank();
                        reloadGridBank();
                    }

                    widget.showNotification(result.message);
                });

                return false;
            });

            $("#btnGridProductBankDelete").on("click", function () {
                widget.selectedRow("gridBank", function (data) {
                    if (widget.isNullOrEmpty(data) == false) {
                        widget.confirm("Apakah anda akan menghapus data terpilih?", function (confirmation) {
                            if (confirmation == "Yes") {
                                data.parent = null;
                                data.__proto__ = null;
                                data._events = null;

                                widget.post("gn.api/CustomerBank/Delete", data, function (result) {
                                    if (result.status) {
                                        clearFormBank();
                                        reloadGridBank();
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

    function showGrid(a, b, c) {
        widget.hidePanel([
            {
                name: "gridProfitCenter",
                type: "kgrid"
            },
            {
                name: "gridProductDiscount",
                type: "kgrid"
            },
            {
                name: "gridBank",
                type: "kgrid"
            }
        ], function () {
            //widget.hidePanel({ name: "tabProfitCenter", unanimated: true });
            //widget.hidePanel("tabProductDiscount");
            //widget.hidePanel("tabBank");
        });

        if (c == "tabProfitCenter") {
            widget.showPanel([
                {
                    name: "gridProfitCenter",
                    type: "kgrid"
                }
            ], function () {
            });
            clearFormProfitCenter();
            $("#tabProfitCenter").css("display", "none");
            console.log("hide profit center");
        }

        if (c == "tabProductDiscount") {
            widget.showPanel([
               {
                   name: "gridProductDiscount",
                   type: "kgrid"
               }
            ]);
            widget.hidePanel("tabProductDiscount");
            clearFormProductDiscount();

            console.log("hide discount");
        } 

        if (c == "tabBank") {
            widget.showPanel([
               {
                   name: "gridBank",
                   type: "kgrid"
               }
            ]);
            widget.hidePanel("tabBank");
            clearFormBank();

            console.log("hide bank");
        }
    }

    function clearFormProfitCenter() {
        var data = {
            ProfitCenterCode: "",
            ProfitCenterName: "",
            CustomerClass: "",
            CustomerClassName: "",
            PaymentCode: "",
            TaxCode: "PPN",
            TaxDesc: "PAJAK PERTAMBAHAN NILAI",
            CollectorCode: "",
            CollectorName: "",
            TaxTransCode: "",
            TaxTransDesc: "",
            Salesman: "",
            SalesmanName: "",
            KelAR: "",
            KelARDesc: "",
            CreditLimit: 0,
            IsOverDueAllowed: false,
            IsBlackList: false,
            CustomerGrade: "",
            CustomerGradeName: "",
            ContactPerson: "",
            TOPCode: "",
            GroupPrice: "",
            GroupPriceDesc: "",
            SalesType: "",
            DiscPct: 0,
            LaborDiscPct: 0,
            PartDiscPct: 0,
            MaterialDiscPct: 0
        };
        widget.populate(data);
    }

    function clearFormProductDiscount() {
        var data = {
            ProfitCenterCodeDisc: "",
            ProfitCenterNameDisc: "",
            TypeOfGoodsDisc: "",
            TypeOfGoodsNameDisc: "",
            DiscPctDisc: ""
        };
        widget.populate(data);
    }

    function clearFormBank() {
        var data = {
            BankCode: "",
            BankName: "",
            AccountName: "",
            AccountBank: "",
        };
        widget.populate(data);
    }
});