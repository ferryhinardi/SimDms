$(document).ready(function () {
    var options = {
        title: "Process Invoice - Batch",
        xtype: "panels",
        toolbars: [
            { name: "btnQuery", text: "Query" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnProcess", text: "Process Invoice", icon: "icon-bolt" },
            { name: "btnPrint", text: "Print Invoice", icon: "icon-bolt" },
        ],
        panels: [
            {
                items: [
                    {
                        text: "Branch",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                            { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]
                    },
                     { name: 'GroupJobType', text: 'Group Job Type', cls: 'span4', type: 'select' },
                     { name: 'Remark', text: 'Remark', cls: 'span4', type: 'text', readonly: true },
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                   { name: "JobOrder", text: "Job Order" },
                   { name: "Invoice", text: "List of Invoice" },
                ]
            },
            {
                cls: "tabpage1 JobOrder",
                xtype: "table",
                tblname: "tblJobOrder",
                showcheckbox: true,
                columns: [
                    { name: "JobOrderNo", text: "Job Order No" },
                    { name: "JobOrderDate", text: "Job Order Date", type: "date" },
                    { name: "JobType", text: "Job Type" },
                    { name: "LaborAmt", text: "Labor Amount" },
                    { name: "PartAmt", text: "Part Amount" },
                    { name: "MaterialAmt", text: "Material Amount" },
                    { name: "TotalPPnAmt", text: "PPN Amount" },
                    { name: "TotalAmt", text: "Total Amount" },
                    { name: "Remarks", text: "Remark" },
                ]
            },
            {
                cls: "tabpage1 Invoice",
                xtype: "panel",
                name:"PrintInvoice",
                items: [
                     {
                         name: "NoFakturFrom",
                         text: "No Faktur From",
                         cls: "span4",
                         placeholder: "No Faktur",
                         type: "popup",
                         btnName: "btnNoFakturFrom"
                    },
                     {
                         name: "NoFakturTo",
                         text: "No Faktur To",
                         cls: "span4",
                         placeholder: "No Faktur",
                         type: "popup",
                         btnName: "btnNoFakturTo"
                     },
                     {
                         name: "Signature",
                         text: "Penanda Tangan",
                         cls: "span4 full",
                         placeholder: "Penanda Tangan",
                         type: "popup",
                         btnName: "btnSignature"
                     }
                ]
            },
            {
                cls: "tabpage1 Invoice",
                xtype: "table",
                tblname: "tblInvoice",
                columns: [
                    { name: "JobOrderNo", text: "Job Order No", width: 120 },
                    { name: "InvoiceNo", text: "Invoice No", width: 120 },
                    { name: "InvoiceDate", text: "Invoice Date", type: "date" },
                    { name: "JobType", text: "Job Type" },
                    { name: "LaborAt", text: "Labor Amount" },
                    { name: "PartAmt", text: "Part Amount" },
                    { name: "MaterialAmt", text: "Material Amount" },
                    { name: "TotalPPnAmt", text: "PPN Amount" },
                    { name: "TotalAmt", text: "Total Amount" },
                    { name: "Remarks", text: "Remark" },
                ]
            }
        ],
    }

    var paramsSelect = [
      {
          name: "GroupJobType", url: "sv.api/combo/servicerefference",
          optionalText: "--SELECT ALL--",
      }];

    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.setSelect(paramsSelect);
    widget.render(function () {
        $('#btnProcess, #btnPrint, #btnSave').attr('disabled', 'disabled');
        $.post("sv.api/invoicebatch/default", function (result) {
            widget.default = result;
            widget.populate(result);
        });
        RefreshGrid();
    });

    $('#btnQuery').on("click", function () {
        RefreshGrid();
    });

    $('#GroupJobType').on('change', function () {
        RefreshGrid();
    });

    function RefreshGrid() {
        widget.post("sv.api/invoicebatch/InquiryInvoiceBatch?groupjobtype=" + $('#GroupJobType').val(), function (result) {
            widget.populateTable({ selector: "#tblJobOrder", data: result.listJobOrder, selectable: true, multiselect: true });
            widget.populateTable({ selector: "#tblInvoice", data: result.listInvoice, selectable: true, multiselect: false });
            $('.ajax-loader').removeAttr("style");
            $('#PrintInvoice').hide();
            console.log("PrintInvoice");
        });
    }

    $('#tblJobOrder').on("click", function () {
        var jobOrderNo = $('.row_selected').map(function (idx, el) {
            var td = $(el).find('td');
            return td.eq(1).text();

        }).get();

        if (jobOrderNo != "") {
            $('#btnProcess, #btnSave').removeAttr('disabled');
            $('#Remark').removeAttr('readonly');
        }
        else {
            $('#btnProcess, #btnSave').attr('disabled', 'disabled');
            $('#Remark').attr('readonly', true);
        }
    });

    $('#tblInvoice').on("dblclick", function() {
        var jobOrderNo = $('.row_selected').map(function (idx, el) {
            var td = $(el).find('td');

            return td.eq(1).text();

        }).get();
        console.log(jobOrderNo);

        if (jobOrderNo != "")
            $('#btnPrint').removeAttr('disabled');
        else
            $('#btnPrint').attr('disabled', 'disabled');

        $('#PrintInvoice').show();
        $('#NoFakturFrom, #btnNoFakturFrom').removeAttr('disabled');
        $('#NoFakturTo, #btnNoFakturTo').removeAttr('disabled');
        $('#Signature, #btnSignature').removeAttr('disabled');
        $('#NoFakturTo,#NoFakturFrom').val(jobOrderNo);
    });

    $('#btnNoFakturTo,#btnNoFakturFrom').click(function () {
        var btnID = $(this).context.id;
        var lookup = widget.klookup({
            name: "LkpInvoice",
            title: "Invoice - Faktur Pajak Lookup",
            url: "sv.api/lookup/InvoiceFakturPajak",
            sort: ({ field: "InvoiceNo", dir: "desc" }),
            serverBinding: true,
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "InvoiceNo", text: "Faktur Penjualan", cls: "span4" }
                    ]
                }
            ],
            columns: [
                { field: "InvoiceNo", title: "No Faktur Penjualan", width: 150 },
                { field: "InvoiceDate", title: "Tgl Faktur Penjualan", width: 100, filterable: false, template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                { field: "FPJNo", title: "No Faktur Pajak", width: 150 },
                { field: "FPJDate", title: "Tgl Faktur Pajak", width: 150, filterable: false, template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #" },
                { field: "JobOrderNo", title: "No SPK", width: 150 },
                { field: "JobOrderDate", title: "Tgl SPK", width: 150, filterable: false, template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #" },
                { field: "PoliceRegNo", title: "No Polisi", width: 100 },
                { field: "ServiceBookNo", title: "No. Buku Service", width: 120, filterable: false },
                { field: "JobType", title: "Jenis Pekerjaan", width: 120, filterable: false },
                { field: "ChassisCode", title: "Kode Rangka", width: 150, filterable: false },
                { field: "ChassisNo", title: "No Rangka", width: 150, filterable: false },
                { field: "EngineCode", title: "Kode Mesin", width: 150, filterable: false },
                { field: "EngineNo", title: "No Mesin", width: 150, filterable: false },
                { field: "BasicModel", title: "Basic Model", width: 150, filterable: false },
                { field: "CustomerCode", title: "Customer Code", width: 150, filterable: false },
                { field: "CustomerCodeBill", title: "Customer Code Bill", width: 150, filterable: false },
                //{ field: "Remarks", title: "Remarks", width: 80, filterable: false },
                { field: "Customer", title: "Customer", width: 200, filterable: false },
                //{ field: "CustomerBill", title: "Customer Bill", width: 80, filterable: false },
                { field: "ServiceBookNo", title: "Service Book No", width: 150, filterable: false },
                { field: "Odometer", title: "Odometer", width: 80, filterable: false }
            ]
        });
        lookup.dblClick(function (data) {
            if (btnID === "btnNoFakturFrom") {
                $('#NoFakturFrom').val(data.InvoiceNo);
            }
            else {
                $('#NoFakturTo').val(data.InvoiceNo);
            }
        });
    });

    $('#btnSignature').click(function () {
        var lookup = widget.klookup({
            name: "LkpSign",
            title: "Penanda Tangan",
            url: "sv.api/lookup/Signer",
            sort: ({ field: "SignName", dir: "desc" }),
            serverBinding: true,
            columns: [
                { field: "SignName", title: "Nama", width: 150 },
                { field: "TitleSign", title: "Jabatan", width: 150 },
            ]
        });
        lookup.dblClick(function (data) {
            $('#Signature').val(data.SignName);
        });
    });

    $('#btnSave').on("click", function () {
        var jobOrderNo = $('.row_selected').map(function (idx, el) {
            var td = $(el).find('td');

            return td.eq(1).text();

        }).get();

        widget.post("sv.api/invoicebatch/SaveInquiryInvoiceBatch?groupjobtype=" + $('#GroupJobType').val() + "&jobOrderNo=" + jobOrderNo + "&Remarks=" + $('#Remark').val(),
            function (result) {
                widget.populateTable({ selector: "#tblJobOrder", data: result.listJobOrder, selectable: true, multiselect: true });
                $('#btnSave').attr('disabled');
                $('#Remark').val('');
                $('#Remark').attr('disabled');
                $('.ajax-loader').removeAttr("style");
            });
        console.log('asdasd');
    });

    $('#btnProcess').on("click", function () {
        var countSelect = $('.row_selected').length;
        if (confirm("Terdapat " + countSelect + " SPK yang akan di proses, akan dilanjutkan ?")) {
            var spkNoAr = new Array();
            var remarkAr = new Array();
            var jobOrderNo = $('.row_selected').map(function (idx, el) {
                var td = $(el).find('td');
                //return td.eq(1).text();
                spkNoAr[idx] = td.eq(1).text();
                remarkAr[idx] = td.eq(9).text();
            }).get();

            var data = { JobOrderNumbers: spkNoAr, Remarks: remarkAr };
            
            widget.post("sv.api/invoicebatch/ProcessInvoiceBatch?JobOrderNumbers="+spkNoAr +"&Remarks="+remarkAr,
            function (result) {
                RefreshGrid();
            });
        }
    });

    $('#btnPrint').click(function () {
        var data = $('#NoFakturFrom').val() + ";" + $('#NoFakturTo').val();
        var rparam = $("#Signature").val();
        console.log(data);
        widget.ShowReportPopup.show({
            id: "SvRpTrn004",
            type: "devex",
            par: data,
            rparam: rparam
        });
    });

    widget.onTabsChanged(function (obj, parent, name) {
        if (name == "JobOrder") {
            $('#btnPrint').attr('disabled', 'disabled');
            RefreshGrid();
        }
        else if (name == "Invoice") {
            $('#NoFakturFrom, #btnNoFakturFrom').attr('disabled', 'disabled');
            $('#NoFakturTo, #btnNoFakturTo').attr('disabled', 'disabled');
            $('#Signature, #btnSignature').attr('disabled', 'disabled');
            $('#NoFakturTo,#NoFakturFrom, #Signature').val("");
            $('#btnProcess, #btnSave').attr('disabled', 'disabled');
            $('#Remark').attr('readonly', true);
            RefreshGrid();
        }
    });

});