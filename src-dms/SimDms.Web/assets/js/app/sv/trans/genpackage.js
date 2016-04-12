$(document).ready(function () {
    var options = {
        title: "Generate Paket Service",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" }
        ],
        panels: [
            {
                name: "pnlMain",
                title: "Generate File",
                items: [
                    { name: "BatchNo", text: "No Batch", placeHolder: "No Paket", cls: "span4 full", readonly: true },
                    { name: "ReceiptNo", text: "No Kwitansi", cls: "span4" },
                    { name: "ReceiptDate", text: "Tgl.", cls: "span4", type: "datepicker" },
                    { name: "FPJNo", text: "No Faktur Pajak", cls: "span4" },
                    { name: "FPJDate", text: "Tgl.", cls: "span4", type: "datepicker" },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnQuery", text: "Query", icon: "icon-list-alt" },
                        ]
                    }
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "tabInvoice", text: "Invoice" },
                    { name: "tabGenerate", text: "Generate File" },
                ]
            },
            {
                cls: "tabpage1 tabInvoice",
                title: "Informasi Invoice",
                name: "pnlInvoice",
                xtype: "table",
                tblname: "tblInvoice",
                showcheckbox: true,
                columns: [
                    { name: "GenerateNo", text: "Generate No", width: 120 },
                    { name: "GenerateDate", text: "Generate Date", type: "date", width: 150 },
                    { name: "BranchFrom", text: "Branch From", width: 100 },
                    { name: "BranchTo", text: "Branch To", width: 100 },
                    { name: "DateFrom", text: "Date From", type: "date", width: 150 },
                    { name: "DateTo", text: "Date To", type: "date", width: 150 },
                    { name: "NoOfInvoices", text: "#Invoice", width: 120 },
                ]
            },
            {
                cls: "tabpage1 tabGenerate",
                title: "WPACK Data",
                name: "pnlGenerate",
                items: [
                    { name: "textFile", type: "textarea", readonly: true },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnSaveFile", text: "Save File", icon: "icon-save" },
                            //{ name: "btnSendFile", text: "Send File", icon: "icon-envelope-alt" }
                        ]
                    }
                ]
            }
        ]
    }
    widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () { alterUI("N"); });
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "BatchList":
                getBatch(data);
                break;
            default:
                break;
        }
    });
    $('#btnCreate').on('click', function () { alterUI("N") });
    $('#btnBrowse').on('click', lookupBatch);
    $('#btnSave').on('click', save);
    $('#btnQuery').on('click', query);
    $('[data-name=tabGenerate]').on('click', generate);
    $('#btnSaveFile').on('click', saveFile);
    function lookupBatch() {
        widget.lookup.init({
            name: "BatchList",
            title: "Paket Service",
            source: "sv.api/grid/batchpackages", //
            sortings: [[0, "asc"]],
            columns: [
                { mData: "BatchNo", sTitle: "No Batch", sWidth: "150px" },
                {
                    mData: "BatchDate", sTitle: "Tanggal Batch", sWidth: "170px",
                    mRender: function (data, type, full) {
                        if (data != null) return moment(data).format('DD MMM YYYY - HH:mm');
                        else return "";
                    }
                },
                { mData: "ReceiptNo", sTitle: "No Kwitansi", sWidth: "150px" },
                { mData: "FpjNo", sTitle: "No Faktur", sWidth: "150px" }
            ]
        });
        widget.lookup.show();
    }
    function getBatch(data) {
        var header = {
            BatchNo: data.BatchNo,
            ReceiptNo: data.ReceiptNo, 
            ReceiptDate: data.ReceiptDate,
            FPJNo: data.FpjNo,
            FPJDate: data.FpjDate
        }
        $.ajax({
            type: 'POST',
            data: { batchNo: data.BatchNo } ,
            url: 'sv.api/genpackage/getBatch',
            success: function (result) {
                if (result.message != "") {
                    alert(result.message);
                    return;
                }
                var details = result.details || {};
                widget.populate(header);
                widget.populateTable({ selector: "#tblInvoice", data: details, selectable: true, multiselect: true });
                $('[data-name=tabInvoice]').click();
                alterUI('O');
            }
        });
    }
    function query() {
        $.ajax({
            type: 'POST',
            url: 'sv.api/genpackage/query',
            success: function (result) {
                if (result.message != "") {
                    alert(result.message);
                    return;
                }
                var details = result.data || {};
                widget.populateTable({ selector: "#tblInvoice", data: details, selectable: true, multiselect: true });
            }
        });
    }
    function generate() {
        var data = {
            batchNo: $('#BatchNo').val()
        }

        $.ajax({
            type: 'POST',
            data: data,
            url: 'sv.api/genpackage/generatefile',
            success: function (result) {
                if (result.message != "") {
                    alert(result.message);
                    return;
                }
                var file = result.file;
                $('#textFile').val(file);
            }
        });
    }
    function save() {
        var receiptNo = $('#ReceiptNo').val();
        var fpjNo = $('#FPJNo').val();
        if (receiptNo.trim() == "") {
            alert("No. Kwitansi harus diisi");
            return;
        }
        if (fpjNo.trim() == "") {
            alert("No. Faktur Pajak harus diisi");
            return;
        }
        var data = [];
        for (var i = 0; i <= 7 ; i++) {
            data[i] = $('.row_selected').map(function (idx, el) {
                var td = $(el).find('td');

                return td.eq(i).text();

            }).get();
        }
        var nModel = [];
        var sampleLen = data[0].length;
        for (var i = 0; i < sampleLen; i++) {
            var myModel = new Model(data, i);
            nModel.push(myModel);
        }
        if (nModel.length == 0) {
            alert("Tidak ada Invoice yang dipilih");
            return;
        }
        var header = {
            ReceiptNo: receiptNo,
            ReceiptDate: $('[name=ReceiptDate]').val(),
            FpjNo: $('#FPJNo').val(),
            FpjDate: $('[name=FPJDate]').val()
        }
        var params = {
            header: header,
            details: nModel,
        }
        $.ajax({
            type: 'POST',
            url: 'sv.api/genpackage/validate',
            success: function (result) {
                if (result.message != "") {
                    alert(result.message);
                    return;
                }
                $.ajax({
                    type: 'POST',
                    data: JSON.stringify(nModel),
                    contentType: 'application/json; charset=utf8',
                    dataType: 'json',
                    url: 'sv.api/genpackage/save',
                    success: function (result) {
                        if (result.message != "") {
                            alert(result.message);
                            return;
                        }
                        
                    }
                });
            }
        });
    }
    function saveFile() {
        //var data = {
        //    text: $('#textFile').val()
        //}
        //$.ajax({
        //    type: 'POST',
        //    data: JSON.stringify(data),
        //    contentType: 'application/json; charset=utf8',
        //    dataType: 'json',
        //    url: 'sv.api/genpackage/saveGeneratedFile',
        //    success: function (result) {
        //        console.log(this.href);
        //    }
        //});
        var batchNo = $('#BatchNo').val();
        window.location = 'sv.api/genpackage/saveGeneratedFile?batchNo=' + batchNo;
    }
    function alterUI(status) {
        if (status == 'N') {
            widget.clearForm();
            $('#ReceiptNo').removeAttr('disabled');
            $('#FPJNo').removeAttr('disabled');
            $('#btnQuery').removeAttr('disabled');
            $('#btnSendFile').attr('disabled', 'disabled');
            widget.post("sv.api/genpackage/default", function (result) {
                widget.default = $.extend({}, result);
                widget.populate(widget.default);
            });
            widget.populateTable({ selector: "#tblInvoice", data: {} });
        } else if (status == 'O') {
            $('#ReceiptNo').attr('disabled', 'disabled');
            $('#FPJNo').attr('disabled', 'disabled');
            $('#btnQuery').attr('disabled', 'disabled');
            $('#btnSendFile').attr('disabled', 'disabled');
        }
    }
});

function Model(data, row) {
    this.GenerateNo = data[1][row];
    this.GenerateDate = data[2][row];
    this.BranchFrom = data[3][row];
    this.BranchTo = data[4][row];
    this.DateFrom = data[5][row];
    this.DateTo = data[6][row];
    this.NoOfInvoices = data[7][row];
}