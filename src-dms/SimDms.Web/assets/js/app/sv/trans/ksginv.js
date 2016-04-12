$(document).ready(function () {
    var isBranch;

    var options = {
        title: 'Get KSG From Invoice',
        xtype: 'panels',
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: 'btnBrowse', text: 'Browse', icon: 'icon-search' },
            { name: 'btnQuery', text: 'Query', icon: 'icon-search' },
            { name: 'btnSave', text: 'Save', icon: 'icon-save', cls: 'hide' },
            { name: 'btnDelete', text: 'Delete', cls: 'hide' }
        ],
        panels: [
            {

                name: "pnlPeriode",
                title: "Periode",
                items: [
                    { name: "GenerateNo", text: "PDI / FSC No.", placeHolder: "FSC/XX/YYYYYY", type: "popup", cls: "span4", readonly: true },
                    { name: "GenerateDate", text: "PDI / FSC Date", cls: "span4", type: "datetimepicker", readonly: true },
                    { name: "InvoiceFrom", text: "Invoice No", cls: "span4 branch hide", type: "popup", readonly: true },
                    { name: "InvoiceTo", text: "s/d", cls: "span4 branch hide", type: "popup", readonly: true },
                    { name: "BranchFrom", text: "Branch", cls: "span4 holding hide", type: "popup", readonly: true },
                    { name: "BranchTo", text: "s/d", cls: "span4 holding hide", type: "popup", readonly: true },
                    { name: "PeriodeDateFrom", text: "Periode", type: "datepicker", cls: "span4 holding hide" },
                    { name: "PeriodeDateTo", text: "s/d", type: "datepicker", cls: "span4 holding hide" },
                    { name: "TotalRecord", text: "Total Record", cls: "span4", readonly: true },
                    { name: "isCampaign", text: "Campaign", type: "switch", cls: "span4", readonly: true },
                    {
                        name: 'JobType', text: 'View by', cls: 'span4 full', type: 'select',
                        items: [
                            { value: '0', text: 'PDI' },
                            { value: '1', text: 'FSC' },
                            { value: '2', text: 'FSC Campaign' }
                        ]
                    },
                ]
            },
            {
                title: "Total Amount Info",
                xtype: "table",
                tblname: "tblTotal",
                columns: [
                    { name: "RecNo", text: "No", width: 20 },
                    { name: "BasicModel", text: "Basic Model", width: 50 },
                    { name: "PdiFscSeq", text: "FS#", width: 20 },
                    { name: "RecCount", text: "Amount", width: 20, type: "numeric", cls: "text-right" },
                    { name: "PdiFscAmount", text: "Total", width: 50, type: "numeric", cls: "text-right" }
                ]
            },
            {
                title: "PDI and FSC Info",
                xtype: "table",
                tblname: "tblInfo",
                columns: [
                    { name: "RecNo", text: "No" },
                    { name: "BranchCode", text: "Branch" },
                    { name: "JobOrderNo", text: "Job Order No." },
                    { name: "JobOrderDate", text: "Job Order Date", type: "date" },
                    { name: "BasicModel", text: "Basic Model" },
                    { name: "ServiceBookNo", text: "Service Book No" },
                    { name: "PdiFscSeq", text: "FS#" },
                    { name: "Odometer", text: "Odometer", type: "numeric", cls: "text-right" },
                    { name: "LaborGrossAmt", text: "Labor", type: "numeric", cls: "text-right" },
                    { name: "MaterialGrossAmt", text: "Material", type: "numeric", cls: "text-right" },
                    { name: "PdiFscAmount", cls: "hide" },
                    { name: "FakturPolisiDate", text: "Polisi Date", type: "date" },
                    { name: "BPKDate", text: "Delivery Date", type: "date" },
                    { name: "ChassisCode", text: "Chassis Code" },
                    { name: "ChassisNo", text: "Chassis No" },
                    { name: "EngineCode", text: "Engine Code" },
                    { name: "EngineNo", text: "Engine No" },
                    { name: "InvoiceNo", cls: "hide" },
                    { name: "FPJNo", cls: "hide" },
                    { name: "FPJDate", type: "date", cls: "hide" },
                    { name: "FPJGovNo", cls: "hide" },
                    { name: "TransmissionType", cls: "hide" },
                    { name: "ServiceStatus", cls: "hide" },
                    { name: "CompanyCode", cls: "hide" },
                    { name: "ProductType", cls: "hide" },
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);

    widget.default = {};

    widget.render(function () {
        $.post('sv.api/ksginv/default', function (result) {
            widget.default = result;
            widget.populate(result);


            isBranch = result.IsBranch;
            if (!isBranch) {
                $('.holding').show();
                $('.Branch').hide();
            }
            else {
                $('.holding').hide();
                $('.branch').show();
            }
        });
    });

    $('#btnInvoiceFrom, #btnInvoiceTo').on('click', function (e) {
        widget.lookup.init({
            name: this.id == "btnInvoiceFrom" ? "IFrom" : "ITo",
            title: "Select Invoice",
            source: "sv.api/grid/invoiceluview?jobtype=" + $('#JobType').val(),
            sortings: [[2, "asc"]],
            columns: [
                { mData: "BranchCode", sTitle: "Branch Code" },
                { mData: "InvoiceNo", sTitle: "Invoice No" },
                {
                    mData: "InvoiceDate", sTitle: "Invoice Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        if (data != null) {
                            return moment(data).format('DD-MMM-YYYY');
                        }
                        return moment(data);
                    }
                },
                { mData: "FPJNo", sTitle: "Tax Invoice No."},
                {
                    mData: "FPJDate", sTitle: "Tax Invoice Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        if (data != null) {
                            return moment(data).format('DD-MMM-YYYY');
                        }
                        return moment(data);
                    }
                },
                { mData: "JobOrderNo", sTitle: "Job Order No." },
                {
                    mData: "JobOrderDate", sTitle: "Job Order Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        if (data != null) {
                            return moment(data).format('DD-MMM-YYYY');
                        }
                        return moment(data);
                    }
                 },
                 { mData: "PoliceRegNo", sTitle: "Police No."},
                 { mData: "ServiceBookNo", sTitle: "Service Book No" },
                 { mData: "JobType", sTitle: "JobType" },
                 { mData: "ChassisCode", sTitle: "Chassis Code" },
                 { mData: "ChassisNo", sTitle: "Chassis No" },
                 { mData: "EngineCode", sTitle: "Engine Code" },
                 { mData: "EngineNo", sTitle: "Engine No" },
                 { mData: "BasicModel", sTitle: "Basic Model" },
                 { mData: "Customer", sTitle: "Customer" },
                 { mData: "CustomerBill", sTitle: "Customer Bill" },
            ]
        });
        widget.lookup.show();
    });

    $('#btnBranchFrom, #btnBranchTo').on('click', function (e) {
        widget.lookup.init({
            name: this.id == "btnBranchFrom" ? "BFrom" : "IFrom",
            title: "Select Branch", 
            source: "sv.api/grid/branch",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "BranchCode", sTitle: "Branch Code" },
                { mData: "CompanyName", STitle: "Branch Name" }
            ]
        });
        widget.lookup.show();
    });

    widget.lookup.onDblClick(function (e, data, name) {
        //console.log(name);

        switch (name) {
            case "PdiFscList":
                $.post('sv.api/ksginv/GetKsg', { genno: data.GenerateNo }, function (result) {
                    if (result.success) {
                        var form = result.form[0];

                        $('#GenerateNo').val(form.GenerateNo);
                        $('#InvoiceFrom').val(form.FromInvoiceNo);
                        $('#InvoiceTo').val(form.ToInvoiceNo);
                        $('#TotalRecord').val(form.TotalNoOfItem);
                        $('#isCampaign').val(form.IsCampaign);

                        widget.populateTable({ selector: "#tblTotal", data: result.total });
                        widget.populateTable({ selector: "#tblInfo", data: result.info });
                    }
                });
                break;
            case "IFrom":
                $('#InvoiceFrom').val(data.InvoiceNo);
                if ($('#InvoiceFrom').val() == "") {
                    $('#InvoiceTo').val("");
                }
                else {
                    if ($('#InvoiceTo').val() == "" || parseFloat($('#InvoiceFrom').val().substring(7, 13)) > parseFloat($('#InvoiceTo').val().substring(7, 13))) {
                        $('#InvoiceTo').val($('#InvoiceFrom').val());
                    }
                }
                break;
            case "ITo":
                $('#InvoiceTo').val(data.InvoiceNo);
                if ($('#InvoiceTo').val() == "") {
                    $('#InvoiceFrom').val("");
                }
                else {
                    if ($('#InvoiceFrom').val() == "" || parseFloat($('#InvoiceFrom').val().substring(7, 13)) > parseFloat($('#InvoiceTo').val().substring(7, 13))) {
                        $('#InvoiceFrom').val($('#InvoiceTo').val());
                    }
                }
                break;
            case "BFrom":
                $('#BranchFrom').val(data.BranchCode);
                if ($('#BranchFrom').val() == "") {
                    $('#BranchTo').val("");
                }
                else {
                    if ($('#BranchTo').val() == "" || parseFloat($('#BranchFrom').val()) > parseFloat($('#BranchTo').val())) {
                        $('#BranchTo').val($('#BranchFrom').val());
                    }
                }
                break;
            case "BTo":
                $('#BranchTo').val(data.BranchCode);
                if ($('#BranchTo').val() == "") {
                    $('#BranchFrom').val("");
                }
                else {
                    if ($('#BranchFrom').val() == "" || parseFloat($('#BranchFrom').val()) > parseFloat($('#BranchTo').val())) {
                        $('#BranchFrom').val($('#BranchTo').val());
                    }
                }
                break;
        }
        //if (name == 'PdiFscList')
        //{
        //    $.post('sv.api/ksginv/GetKsg', { genno: data.GenerateNo }, function (result) {
        //        if (result.success) {
        //            var form = result.form[0];

        //            $('#GenerateNo').val(form.GenerateNo);
        //            $('#InvoiceFrom').val(form.FromInvoiceNo);
        //            $('#InvoiceTo').val(form.ToInvoiceNo);
        //            $('#TotalRecord').val(form.TotalNoOfItem);
        //            $('#isCampaign').val(form.IsCampaign);

        //            widget.populateTable({ selector: "#tblTotal", data: result.total });
        //            widget.populateTable({ selector: "#tblInfo", data: result.info });
        //        }
        //    });
        //}
        //else {
        //    if (name == 'IFrom') {
        //        $('#InvoiceFrom').val(data.InvoiceNo);
        //        if ($('#InvoiceFrom').val() == "") {
        //            $('#InvoiceTo').val("");
        //        }
        //        else {
        //            if ($('#InvoiceTo').val() == "" || parseFloat($('#InvoiceFrom').val().substring(7, 13)) > parseFloat($('#InvoiceTo').val().substring(7, 13))) {
        //                $('#InvoiceTo').val($('#InvoiceFrom').val());
        //            }
        //        }
        //    }
        //    else {
        //        $('#InvoiceTo').val(data.InvoiceNo);
        //        if ($('#InvoiceTo').val() == "") {
        //            $('#InvoiceFrom').val("");
        //        }
        //        else {
        //            if ($('#InvoiceFrom').val() == "" || parseFloat($('#InvoiceFrom').val().substring(7, 13)) > parseFloat($('#InvoiceTo').val().substring(7, 13))) {
        //                $('#InvoiceFrom').val($('#InvoiceTo').val());
        //            }
        //        }
        //    }
        //}
        widget.lookup.hide();
    });

    $('#btnQuery').on('click', getPdiFsc);

    $('#btnBrowse, #btnGenerateNo').on('click', browseData);

    $('#btnSave').on('click', function (e) {
        var data = $(".main .gl-widget").serializeObject();
        if (!isBranch) {
            widget.post("sv.api/ksginv/saveall", data, function (result) {
                if (result.success != "") {
                    $('#GenerateNo').val(result.genno[0]);
                    widget.showNotification(result.message);
                    return;
                }
                widget.showNotification("Data berhasil disimpan.");
            });
        }
        else {
            widget.post("sv.api/ksginv/save", data, function (result) {
                if (result.success != "") {
                    $('#GenerateNo').val(result.genno[0]);
                    widget.showNotification(result.message);
                    return;
                }
                widget.showNotification("Data berhasil disimpan.");
            });
        }
    });

    $("#btnNew").on("click", function () { newData() });

    $('#btnDelete').on('click', function (e) {
        var data = $(".main .gl-widget").serializeObject();
        $.post('sv.api/ksgspk/delete', data, function (result) {
            if (result.success != "") {
                widget.showNotification(result.message);
            }
        });
        newData();
    });

    $('#JobType').val(0);

    function newData() {
        widget.clearForm();
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery"]);
        widget.populateTable({ selector: "#tblTotal", data: {} });
        widget.populateTable({ selector: "#tblInfo", data: {} });
        widget.populate(widget.default);
    };

    function browseData() {
        widget.lookup.init({
            name: "PdiFscList",
            title: "Pdi Fsc Lookup",
            source: "sv.api/grid/pdifscs?source=0",
            sortings: [[0, "desc"]],
            columns: [
                { mData: "GenerateNo", sTitle: "No. PDI FSC", sWidth: "100px" },
                {
                    mData: "GenerateDate", sTitle: "Tgl. PDI FSC", sWidth: "100px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY - HH:mm');
                    }
                },
                { mData: "FPJNo", sTitle: "No. Faktur Pajak" },
                {
                    mData: "FPJDate", sTitle: "Tgl. Faktur Pajak", sWidth: "100px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY - HH:mm');
                    }
                },
                { mData: "FPJGovNo", sTitle: "Seri Pajak", sWidth: "100px" },
                { mData: "SourceDataDesc", sTitle: "Sumber Data", sWidth: "160px" },
                { mData: "TotalNoOfItem", sTitle: "Total Record", sWidth: "100px" },
                { mData: "TotalAmt", sTitle: "Total PDI FSC", sWidth: "100px" },
                { mData: "SenderDealerName", sTitle: "Pengirim", sWidth: "100px" },
                { mData: "RefferenceNo", sTitle: "No. Referensi", sWidth: "100px" },
                { mData: "RefferenceDate", sTitle: "Tgl. Referensi", sWidth: "100px" },
                { mData: "PostingFlagDesc", sTitle: "Status", sWidth: "100px" },
            ]
        });
        widget.lookup.show();
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery", "btnDelete"]);
    }

    function getPdiFsc()
    {
        var data = $(".main .gl-widget").serializeObject();
        if (!isBranch) {
            widget.post("sv.api/ksginv/GetPdiFscAll", data, function (result) {

            });
        }
        else {
            $.post('sv.api/ksginv/GetPdiFsc', data, function (result) {
                if (result.success) {
                    $('#TotalRecord').val(result.totalitem);

                    widget.populateTable({ selector: "#tblTotal", data: result.total });
                    widget.populateTable({ selector: "#tblInfo", data: result.info });
                }
            });
        }
        widget.showToolbars(["btnNew", "btnBrowse", "btnQuery", "btnSave"]);
    }
});