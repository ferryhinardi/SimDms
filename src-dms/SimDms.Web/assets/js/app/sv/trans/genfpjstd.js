$(document).ready(function () {
    var options = {
        title: 'Generate Standard Tax Invoice',
        xtype: 'panels',
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: 'btnBrowse', text: 'Browse', icon: 'icon-search' },
            { name: 'btnQuery', text: 'Query', icon: 'icon-search' },
            { name: 'btnGenerate', text: 'Generate', icon: 'icon-bolt', cls: "hide" },
        ],
        panels: [
        {
            title: 'Standard Tax Invoice',
            items: [
                { name: 'FPJNo', text: 'Tax Invoice', cls: 'span4 full', readonly: true },
                { name: "InvoiceNoStart", text: "Invoice No", type: "popup", cls: "span4" },
                { name: "InvoiceNoEnd", text: "s/d", type: "popup", cls: "span4" },
            ]
        },
        {
            title: 'List',
            xtype: 'table',
            tblname: 'tblFPJStd',
            columns: [
                     { name: "RowNum", text: "No" },
                     { name: "InvoiceNo", text: "Invoice No" },
                     { name: "InvoiceDate", text: "Invoice Date", type: "date" },
                     { name: "JobOrderNo", text: "Job Order No" },
                     { name: "JobOrderDate", text: "Job Order Date", type: "date" },
                     { name: "DueDate", text: "Due Date", type: "date" },
                     { name: "TotalDPPAmt", text: "DPP" },
                     { name: "TotalPpnAmt", text: "PPN" },
                     { name: "TotalSrvAmt", text: "Total" },
                     { name: "JobType", text: "Job Type" },
                     { name: "PoliceRegNo", text: "Police No" },
                     { name: "BasicModel", text: "Basic Model" },
                     { name: "ChassisCode", text: "Chassis Code" },
                     { name: "ChassisNo", text: "Chassis No" },
                     { name: "EngineCode", text: "Engine Code" },
                     { name: "EngineNo", text: "Engine No" },
                     { name: "Pelanggan", text: "Customer" },
            ]
        }
        ]
    };

    var widget = new SimDms.Widget(options);

    widget.default = {};

    widget.render(function () {
        $.post('sv.api/genfpj/default', function (result) {
            widget.default = result;
            widget.populate(result);
        });
    });

    widget.lookup.onDblClick(function (e, data, name) {
        if (name == 'TaxInvoice') {
            $('#InvoiceNoStart').val(data.InvoiceStart);
            $('#InvoiceNoEnd').val(data.InvoiceEnd);
            $('#FPJNo').val(data.FPJNo);

            var datas = {
                FPJNo: data.FPJNo,
                TaxInvoiceNoStart: data.InvoiceStart,
                TaxInvoiceNoEnd: data.InvoiceEnd
            };
            populateData(datas)
        }
        else {
            if (name == 'Tax Invoice No Start') {
                if ($('#InvoiceNoEnd').val() == '' && $('#InvoiceNoEnd').val() == '') {
                    $('#InvoiceNoStart').val(data.InvoiceNo);
                    $('#InvoiceNoEnd').val(data.InvoiceNo);
                }
                else {
                    var a = parseInt(data.InvoiceNo.substring(7));
                    var b = parseInt($('#InvoiceNoEnd').val().substring(7));
                    if (b < a) {
                        $('#InvoiceNoStart').val(data.InvoiceNo);
                        $('#InvoiceNoEnd').val(data.InvoiceNo);
                    } else {
                        $('#InvoiceNoStart').val(data.InvoiceNo);
                    }
                }
            }
            else {
                if ($('#InvoiceNoEnd').val() == '' && $('#InvoiceNoEnd').val() == '') {
                    $('#InvoiceNoStart').val(data.InvoiceNo);
                    $('#InvoiceNoEnd').val(data.InvoiceNo);
                }
                else {
                    var a = parseInt($('#InvoiceNoStart').val().substring(7));
                    var b = parseInt(data.InvoiceNo.substring(7));
                    if (b < a) {
                        $('#InvoiceNoEnd').val(data.InvoiceNo);
                        $('#InvoiceNoStart').val(data.InvoiceNo);
                    } else {
                        $('#InvoiceNoEnd').val(data.InvoiceNo);
                    }
                }
            }
        }
        widget.lookup.hide();
    });

    $('#btnNew').on('click', function (e) {
        widget.clearForm();
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery"]);
        widget.populateTable({ selector: "#tblFPJStd", data: {} });
        widget.populate(widget.default);
    });

    $("#btnBrowse").on("click", function () {
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery"]);
        widget.lookup.init({
            name: "TaxInvoice",
            title: "Faktur Pajak List",
            source: "sv.api/grid/taxinvoicestd",
            columns: [
                { mData: "FPJNo", sTitle: "No Faktur", sWidth: "110px" },
                {
                    mData: "FPJDate", sTitle: "Tanggal Faktur", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD-MMM-YYYY');
                    }
                },
                { mData: "InvoiceNo", sTitle: "No Invoice" },
                { mData: "Customer", sTitle: "Pelanggan" },
                { mData: "CustomerBill", sTitle: "Pembayar" },
                { mData: "InvoiceStart", cls: "hide" },
                { mData: "InvoiceEnd", cls: "hide" }
            ],
        });
        widget.lookup.show();
    });

    $('#btnQuery').on('click', function () {
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery", "btnGenerate"]);
        var dataMain = $(".main form").serializeObject();
        var data = {
            FPJNo: '',
            TaxInvoiceNoStart: dataMain.InvoiceNoStart,
            TaxInvoiceNoEnd: dataMain.InvoiceNoEnd
        };
        populateData(data);
    });

    $('#btnGenerate').on('click', function () {
        widget.showToolbars(["btnNew"]);
        var dataMain = $(".main form").serializeObject();
        var data = {
            FPJNo: '',
            TaxInvoiceNoStart: dataMain.InvoiceNoStart,
            TaxInvoiceNoEnd: dataMain.InvoiceNoEnd
        };

        widget.post("sv.api/Genfpjstd/Save", data, function (result) {
            if (result.success) {
                $('#FPJNo').val(result.FPJNo);
                widget.showNotification(result.message);
            }
            else {
                widget.showNotification(result.message);
            }
        });
    });

    function populateData(data) {
        widget.post("sv.api/Genfpjstd/getData", data, function (result) {
            if (result.success) {
                widget.populateTable({ selector: "#tblFPJStd", data: result.list });
            }
            else {
                widget.showNotification(result.message);
            }
        });
    }

    $('#btnInvoiceNoStart,#btnInvoiceNoEnd').on('click', function (e) {
        widget.clearForm();
        widget.lookup.init({
            name: e.currentTarget.id == 'btnInvoiceNoStart' ? 'Tax Invoice No Start' : 'Tax Invoice No End',
            source: 'sv.api/grid/taxinvoicestdlookup',
            columns: [
                { mData: 'InvoiceNo', sTitle: 'Invoice No', sWidth: '110px' },
                {
                    mData: 'InvoiceDate', sTitle: 'Invoice Date',
                    mRender: function (data, type, full) {
                        return moment(data).format('DD-MMM-YYYY');
                    }
                },
                { mData: 'JobOrderNo', sTitle: 'Job Order No' },
                {
                    mData: 'JobOrderDate', sTitle: 'Job Order Date',
                    mRender: function (data, type, full) {
                        return moment(data).format('DD-MMM-YYYY');
                    }
                },
                { mData: 'PoliceRegNo', sTitle: 'Police No' },
                { mData: 'JobType', sTitle: 'Job Type' },
                { mData: 'Customer', sTitle: 'Customer' },
                { mData: 'CustomerBill', sTitle: 'Payer' }
            ]
        });
        widget.lookup.show();
        widget.populateTable({ selector: "#tblFPJStd", data: {} });
    });
});