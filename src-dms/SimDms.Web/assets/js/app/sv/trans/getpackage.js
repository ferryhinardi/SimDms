
$(document).ready(function () {
    var options = {
        title: "Get Paket Service",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnDelete", text: "Delete", icon: "icon-remove" }
        ],
        panels: [
            {
                name: "pnlPeriode",
                title: "Periode",
                items: [
                    { name: "GenerateNo", text: "No Paket", placeHolder: "No Paket", cls: "span4", type: "popup", readonly: true },
                    { name: "GenerateDate", text: "Tgl.", cls: "span4", type: "datepicker" },
                    {
                        text: "Branch",
                        type: "controls",
                        items: [
                            { name: "BranchFrom", placeHolder: "From", cls: "span4", type: "popup", readonly: true },
                            { name: "BranchTo", placeHolder: "To", cls: "span4", type: "popup", readonly: true },
                        ]
                    },
                    {
                        text: "Periode",
                        type: "controls",
                        items: [
                            { name: "DateFrom", placeHolder: "From", cls: "span4", type: "datepicker" },
                            { name: "DateTo", placeHolder: "To", cls: "span4", type: "datepicker" },
                        ]
                    },
                    { name: "TotalRecords", text: "Total Record", placeHolder: "0", cls: "span4 number", readonly: true },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnQuery", text: "Query", icon: "icon-list-alt" },
                        ]
                    }
                ]
            },
            {
                title: "Informasi Jumlah Rupiah",
                xtype: "table",
                pnlname: "pnlInfoRp",
                tblname: "tblInfoRp",
                columns: [
                    { name: "SeqNo", text: "No", width: 30 },
                    { name: "BasicModel", text: "Basic Model" },
                    { name: "TotalDppAmt", text: "Total Dpp Amount", cls: "right", width: 200 },
                ]
            },
            {
                title: "Informasi Paket",
                name: "pnlInfoPaket",
                xtype: "table",
                tblname: "tblInfoPaket",
                columns: [
                    { name: "BranchCode", text: "Branch Code", width: 30 },
                    { name: "InvoiceNo", text: "Invoice No", width: 90 },
                    { name: "InvoiceDate", text: "Invoice Date", type: "date", width: 50 },
                    { name: "JobOrderDate", text: "Service Date", type: "date", width: 100 },
                    { name: "ServiceBookNo", text: "Service Book No", width: 40 },
                    { name: "BasicModel", text: "Basic Model", width: 70 },
                    { name: "Chassis", text: "Chassis", width: 90 },
                    { name: "Engine", text: "Engine", width: 90 },
                    { name: "JobType", text: "Job Type", width: 90 },
                    { name: "Odometer", text: "Odometer", width: 70 },
                    { name: "TotalDppAmt", text: "Dpp Amount", width: 70 },
                ]
            },
        ]
    }
    widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () { alterUI("N"); });
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "ServicePackageList":
                getPackage(data.GenerateNo);
                break;
            case "BranchList-btnBranchFrom":
                $('#BranchFrom').val(data.BranchCode);
                break;
            case "BranchList-btnBranchTo":
                $('#BranchTo').val(data.BranchCode);
                break;
            default:
                break;
        }
    });
    $('#btnCreate').on('click', function () { alterUI("N"); });
    $('#btnBrowse, #btnGenerateNo').on('click', lookupGenerateNo);
    $('#btnBranchFrom, #btnBranchTo').on('click', lookupBranch);
    $('#btnQuery').on('click', query);
    $('#btnSave').on('click', save);
    $('#btnDelete').on('click', remove);
    function lookupGenerateNo() {
        widget.lookup.init({
            name: "ServicePackageList",
            title: "Paket Service",
            source: "sv.api/grid/servicepackages", //
            sortings: [[0, "desc"]],
            columns: [
                { mData: "GenerateNo", sTitle: "Generate No", sWidth: "150px" },
                {
                    mData: "GenerateDate", sTitle: "Generate Date", sWidth: "170px",
                    mRender: function (data, type, full) {
                        if (data != null) return moment(data).format('DD MMM YYYY - HH:mm');
                        else return "";
                    }
                },
                { mData: "BranchFrom", sTitle: "Branch From", sWidth: "150px" },
                { mData: "BranchTo", sTitle: "Branch To", sWidth: "150px" },
                {
                    mData: "DateFrom", sTitle: "Date From", sWidth: "170px",
                    mRender: function (data, type, full) {
                        if (data != null) return moment(data).format('DD MMM YYYY - HH:mm');
                        else return "";
                    }
                },
                {
                    mData: "DateTo", sTitle: "Date To", sWidth: "170px",
                    mRender: function (data, type, full) {
                        if (data != null) return moment(data).format('DD MMM YYYY - HH:mm');
                        else return "";
                    }
                },
            ]
        });
        widget.lookup.show();
    }
    function getPackage(generateNo) {
        $.ajax({
            type: 'POST',
            data: { generateNo: generateNo },
            url: 'sv.api/getpackage/getPackage',
            success: function (result) {
                if (result.message != "") {
                    alert(result.message);
                    return;
                }
                var header = result.header[0] || {};
                var details = result.details || {};
                var rpInfo = result.rpInfo || {};

                widget.populate(header);
                widget.populateTable({ selector: "#tblInfoRp", data: rpInfo });
                widget.populateTable({ selector: "#tblInfoPaket", data: details });
                alterUI("O");
            }
        });
    }
    function query() {
        var data = {
            branchFrom: $('#BranchFrom').val(),
            branchTo: $('#BranchTo').val(),
            dateFrom: $('[name=DateFrom]').val(),
            dateTo: $('[name=DateTo]').val()
        }
        $.ajax({
            type: 'POST',
            data: data,
            url: 'sv.api/getpackage/query',
            success: function (result) {
                if (result.message != "") {
                    alert(result.message);
                    return;
                }
                var details = result.details || {};
                var rpInfo = result.rpInfo || {};
                widget.populateTable({ selector: "#tblInfoRp", data: rpInfo });
                widget.populateTable({ selector: "#tblInfoPaket", data: details });
                $('#TotalRecords').val(result.total);
            }
        });
    }
    function lookupBranch() {
        widget.lookup.init({
            name: "BranchList-" + $(this)[0].id,
            title: "Cabang Perusahaan",
            source: "sv.api/grid/branch",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "BranchCode", sTitle: "Kode Cabang", sWidth: "120px" },
                { mData: "CompanyName", sTitle: "Nama Cabang" }
            ]
        });
        widget.lookup.show();
    }
    function save() {
        var data = {
            branchFrom: $('#BranchFrom').val(),
            branchTo: $('#BranchTo').val(),
            dateFrom: $('[name=DateFrom]').val(),
            dateTo: $('[name=DateTo]').val()
        }
        if (data.branchFrom == "") {
            alert("Branch From harus diisi");
            return;
        }
        if (data.branchTo == "") {
            alert("Branch To harus diisi");
            return;
        }
        if (!confirm("Apakah benar anda akan menyimpan data ini ?")) return;
        $.ajax({
            type: 'POST',
            data: data,
            url: 'sv.api/getpackage/save',
            success: function (result) {
                if (result.message != "") {
                    alert(result.message);
                    return;
                }
                alert("Save success");
                $('#GenerateNo').val(result.generateNo);
                alterUI("O");
            }
        });
    }
    function remove() {
        var data = {
            generateNo: $('#GenerateNo').val()
        }
        if (!confirm("Apakah anda yakin ingin menghapus data ini?"));
        $.ajax({
            type: 'POST',
            data: data,
            url: 'sv.api/getpackage/delete',
            success: function (result) {
                if (result.message != "") {
                    alert(result.message);
                    return;
                }
                alert("Delete berhasil");
                alterUI('N');
            }
        });
    }
    function alterUI(status) {
        if (status == 'N') {
            widget.clearForm();
            $('[name=GenerateDate]').attr('disabled', 'disabled');
            $('[name=DateFrom]').removeAttr('disabled');
            $('[name=DateTo]').removeAttr('disabled');
            $('#btnBranchFrom').removeAttr('disabled');
            $('#btnBranchTo').removeAttr('disabled');
            $('#pnlInfoPaket').show();
            widget.post("sv.api/getpackage/default", function (result) {
                widget.default = $.extend({}, result);
                widget.populate(widget.default);
            });
            widget.populateTable({ selector: "#tblInfoRp", data: {} });
            widget.populateTable({ selector: "#tblInfoPaket", data: {} });
        } else if (status == "O") {
            $('[name=GenerateDate]').attr('disabled', 'disabled');
            $('[name=DateFrom]').attr('disabled', 'disabled');
            $('[name=DateTo]').attr('disabled', 'disabled');
            $('#btnBranchFrom').attr('disabled', 'disabled');
            $('#btnBranchTo').attr('disabled', 'disabled');
            $('#pnlInfoPaket').show();
        }
    }
});