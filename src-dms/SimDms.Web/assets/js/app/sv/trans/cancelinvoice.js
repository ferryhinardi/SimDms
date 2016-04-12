$(document).ready(function () {
    var options = {
        title: "Cancel Invoice",
        xtype: "panels",
        toolbars: [
           { name: "btnClear", text: "Clear", icon: "icon-file" },
           { name: "btnQuery", text: "Query" },
           { name: "btnReposting", text: "Re-posting Invoice", icon: "icon-bolt" },
           { name: "btnCancelInv", text: "Cancel Invoice", icon: "icon-bolt" },
        ],
        panels: [
            {
                name: "pnlFilter",
                items: [
                    {
                        text: "Branch",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                            { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]                        
                    },                   
                    { name: 'InvoiceNo1', text: 'Invoice No From', cls: 'span3', type: "popup", btnName: "btnInvoiceNo1", readonly: true },
                    { name: 'InvoiceNo2', text: 'Invoice No To', placeholder: 'Invoice No', cls: 'span3', type: "popup", btnName: "btnInvoiceNo2", readonly: true },
                ]
            },         
            {
                name: "CancelInv",
                xtype: "k-grid",            
                columns: [
                    { field: "InvoiceNo", title: "Invoice No" },
                    { field: "InvoiceDate", title: "Invoice Date", type: "date" },
                    { field: "JobOrderNo", title: "Job Order No" },
                    { field: "JobOrderDate", title: "Job Order Date", type: "date" },
                    { field: "JobType", title: "Job Type" },
                    { field: "LaborDppAmt", title: "Labor Amount", cls: "right", type: "price" },
                    { field: "PartsDppAmt", title: "Part Amount", cls: "right", type: "price" },
                    { field: "MaterialDppAmt", title: "Material Amount", cls: "right", type: "price" },
                    { field: "TotalSrvAmt", title: "Total Srv Amount", cls: "right", type: "price" },
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);

    widget.default = {};

    widget.render(function () {
        $.post('sv.api/cancelinv/default', function (result) {
            widget.default = result;
            widget.populate(result);

            $('#btnReposting, #btnCancelInv').attr('disabled', 'disabled');
        });
    });

    $('#btnInvoiceNo1').on('click', function (e) {
        lookup('Invoice1');
    });

    $('#btnInvoiceNo2').on('click', function (e) {
        lookup('Invoice2');
    });

    function lookup(button) {
        widget.lookup.init({
            name: button,
            title: "Invoice",
            source: "sv.api/grid/invoicecancelview",
            columns: [
                { mData: "InvoiceNo", sTitle: "Invoice No", sWidth: "110px" },
                {
                    mData: "InvoiceDate", sTitle: "Tanggal Faktur",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD-MMM-YYYY');
                    }
                },
                { mData: "FPJNo", sTitle: "FPJ No", sWidth: "110px" },
                 {
                     mData: "FPJDate", sTitle: "FPJ Date",
                     mRender: function (data, type, full) {
                         return moment(data).format('DD-MMM-YYYY');
                     }
                 },
                  { mData: "JobOrderNo", sTitle: "JobOrder No", sWidth: "110px" },
                 {
                     mData: "JobOrderDate", sTitle: "JobOrder Date",
                     mRender: function (data, type, full) {
                         return moment(data).format('DD-MMM-YYYY');
                     }
                 },
                 { mData: "PoliceRegNo", sTitle: "Police No." },
                 { mData: "ServiceBookNo", sTitle: "Service Book No" },
                 { mData: "JobType", sTitle: "JobType" },
                 { mData: "ChassisCode", sTitle: "Chassis Code" },
                 { mData: "ChassisNo", sTitle: "Chassis No" },
                 { mData: "EngineCode", sTitle: "Engine Code" },
                 { mData: "EngineNo", sTitle: "Engine No" },
                 { mData: "BasicModel", sTitle: "Basic Model" },
                 { mData: "Customer", sTitle: "Customer" },
                 { mData: "CustomerBill", sTitle: "Customer Bill" },
            ],
        });
        widget.lookup.show();
    }

    $('#btnClear').on('click', function (e) {
        $('#InvoiceNo1, #InvoiceNo2').val('');
        refreshGridCancelInv();        
    });

    widget.lookup.onDblClick(function (e, data, name) {
        if (name == 'Invoice1') {
            $('#InvoiceNo1').val(data.InvoiceNo);
            if ($('#InvoiceNo2').val() == '') {
                $('#InvoiceNo2').val(data.InvoiceNo);
            }           
        }
        else if (name == 'Invoice2') {
            $('#InvoiceNo2').val(data.InvoiceNo);
            if ($('#InvoiceNo1').val() == '') {
                $('#InvoiceNo1').val(data.InvoiceNo);
            }
        }
        widget.lookup.hide();
    });

    $('#btnQuery').on('click', function (e) {        
        refreshGridCancelInv();
    });

    $("#CancelInv").on("change", ".check_item", function (e) {
        var row = $(e.target).closest("tr");
        var checkbox = $(this);

        if (checkbox.is(':checked')) {
            row.addClass("k-state-selected");
            row.attr("aria-selected", true);
            $('#btnReposting, #btnCancelInv').removeAttr('disabled', 'disabled');
        }
        else {
            row.removeClass("k-state-selected");

            var grid = $("#CancelInv").data("kendoGrid");
            var row = grid.select();
           
            if (row.length == 0) {
                $('#btnReposting, #btnCancelInv').attr('disabled', 'disabled');
            }
            else {
                $('#btnReposting, #btnCancelInv').removeAttr('disabled', 'disabled');
            }
        }
    });

    function selectDeselectAll() {
        var grid = $("#CancelInv").data("kendoGrid");
        var allData = grid.dataSource.data();

        if ($('#chkSelectAll').is(':checked')) {
            $('.check_item').prop('checked', true);
            $("#CancelInv [role=row]").addClass("k-state-selected");
            $("#CancelInv [role=row]").attr("aria-selected", true);
            $('#btnReposting, #btnCancelInv').removeAttr('disabled', 'disabled');
        }
        else {
            $('.check_item').prop('checked', false);
            $("#CancelInv [role=row]").removeClass("k-state-selected");
            $('#btnReposting, #btnCancelInv').attr('disabled', 'disabled');
        }
    }

    function refreshGridCancelInv() {
        //$(".ajax-loader").hide();
        widget.kgrid({
            url: "sv.api/cancelinv/SelectInqInvCancel?inv1=" + $("#InvoiceNo1").val() + '&inv2=' + $("#InvoiceNo2").val(),
            name: "CancelInv",          
            serverBinding: true,
            columns: [
                { field: "check_item", title: "<input type='checkbox' id='chkSelectAll'/>", template: "<input class='check_item' type='checkbox' id='inpchk''/>", width: '40px', sortable: false, filterable: false },
                { field: "InvoiceNo", title: "Invoice No" },
                { field: "InvoiceDate", title: "Invoice Date", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                { field: "JobOrderNo", title: "Job Order No" },
                { field: "JobOrderDate", title: "Job Order Date", template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #" },
                { field: "JobType", title: "Job Type" },
                { field: "LaborDppAmt", title: "Labor Amount", align: "right" },
                { field: "PartsDppAmt", title: "Part Amount", align: "right" },
                { field: "MaterialDppAmt", title: "Material Amount", align: "right" },
                { field: "TotalSrvAmt", title: "Total Srv Amount", align: "right" },
            ],
            detailInit: detailInit
        });

        $("#chkSelectAll").on("change", selectDeselectAll);
        $("#inpchk").on("change", selectDeselectAll);

        $('#btnReposting, #btnCancelInv').attr('disabled', 'disabled');
    }    

    function detailInit(e) {
        widget.post("sv.api/cancelinv/GetDescInvoice", { InvoiceNo: e.data.InvoiceNo }, function (result) {           
            $("<div/>").appendTo(e.detailCell).kendoGrid({
                dataSource: { data: result.data, pageSize: 10 },
                pageable: true,
                columns: [
                    { field: "AccountNo", title: "No Account", width: 230 },
                    { field: "Description", title: "Deskripsi", width: 500 },
                    { field: "TypeTrans", title: "Type Trans" },
                    { field: "AmountDbOld", title: "Amount Debet", align: "right" },
                    { field: "AmountCrOld", title: "Amount Credit", align: "right" },
                    { field: "AmountDb", title: "Correction Debet", align: "right" },
                    { field: "AmountCr", title: "Correction Credit", align: "right" },
                ]
            });             
        })
    }

    $('#btnReposting').on('click', function (e) {
        var grid = $("#CancelInv").data("kendoGrid");
        var row = grid.select();
        var invoice = new Array();
        $.each(row, function (index, content) {
            var dataRow = grid.dataItem(content);
            invoice.push(dataRow.InvoiceNo);
        });
        widget.post("sv.api/cancelinv/RePosting?invoiceNo=" + invoice, function (result) {
            if (result.Message == '') {
                widget.showNotification("Proses Re-Posting Faktur Selesai");
                refreshGridCancelInv();
            }
            else {
                widget.alert(result.Message);
            }
        });
    });

    $('#btnCancelInv').on('click', function (e) {
        var grid = $("#CancelInv").data("kendoGrid");
        var row = grid.select();
        var invoice = new Array();
        $.each(row, function (index, content) {
            var dataRow = grid.dataItem(content);
            invoice.push(dataRow.InvoiceNo);
        });

        widget.confirm("Anda yakin akan Cancel Invice tersebut?", function (e) {
            if (e == "Yes") {
                if ($('#chkSelectAll').is(':checked')) {
                    widget.confirm('Anda memilih untuk membatalkan semua Invoice, apakah Anda yakin?', function (i) {
                        if (i == "Yes") {
                            CancelInvoice(invoice);
                        }
                    });
                }
                else {
                    CancelInvoice(invoice);
                }
            }
        });
    });

    function CancelInvoice(invoice) {
        widget.post("sv.api/cancelinv/CancelInvoice?invoiceNo=" + invoice, function (result) {
            if (result.Message == '') {
                widget.showNotification("Proses Cancel Invoice Selesai");
                refreshGridCancelInv();
            }
            else {
                widget.alert(result.Message);
            }
        });
    }
});