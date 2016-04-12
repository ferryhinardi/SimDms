$(document).ready(function () {
    var options = {
        title: 'Input Penerimaan Pekerjaan Luar',
        xtype: 'panels',
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnProcessRcv", text: "Process Receiving", icon: "icon-bolt", cls: "hide" },
            { name: "btnCancelRcv", text: "Cancel Receiving", icon: "icon-undo", cls: "hide" },
            { name: "btnPrintPO", text: "Print", icon: "icon-print" }
        ],
        panels: [
            {
                title: 'Status',
                items: [
                    { name: "ServiceAmt", text: "Price", placeHolder: "Price", cls: "span4 number", format: "{0:#,##0}", readonly: true },
                    { name: "Description", text: "Status", placeHolder: "Status", cls: "span4", readonly: true },
                    { name: "POStatus", cls: "hide", readonly: true }
                ]
            },
            {
                title: 'Receiving',
                items: [
                    { name: "RecNo", text: "Receiving No", placeHolder: "RRO/09/XXXXXX", cls: "span4", readonly: true },
                    { name: "RecDate", text: "Receiving Date", type: "date", cls: "span4", readonly: true },
                ]
            },
            {
                title: "Faktur/Nota",
                items: [
                    { name: "FPJNo", text: "Tax Invoice No", placeHolder: "INV/XX/XXXXXX", cls: "span4", required: true },
                    { name: "FPJDate", text: "Tax Invoice Date", type: "datepicker", placeHolder: "Tax Invoice Date", cls: "span4", required: true },
                    { name: "FPJGovNo", text: "Tax Invoice Serial No", placeHolder: "Tax Invoice Serial No", cls: "span4", required: true }
                ]
            },
            {
                title: 'Service and Vehicle Info',
                items: [
                    { name: "PONo", text: "PO Number", type: "popup", placeHolder: "POT/XX/999999", cls: "span4", btnName: "btnPO" },
                    { name: "PODate", text: "PO Date", type: "date", cls: "span4", readonly: true },
                    { name: "JobOrderNo", text: "Job Order No", cls: "span4", readonly: true },
                    { name: "JobOrderDate", text: "Job Order Date", type: "date", cls: "span4", readonly: true },
                    { name: "PoliceRegNo", text: "Police No", placeHolder: "Police No", cls: "span4 full", readonly: true },
                    {
                        text: "Chassis",
                        type: "controls",
                        items: [
                            { name: "ChassisCode", placeHolder: "Chassis Code", cls: "span4", readonly: true },
                            { name: "ChassisNo", placeHolder: "Chassis No", cls: "span4", readonly: true }
                        ]
                    },
                    {
                        text: "Engine",
                        type: "controls",
                        items: [
                            { name: "EngineCode", placeHolder: "Engine Code", cls: "span4", readonly: true },
                            { name: "EngineNo", placeHolder: "Engine No", cls: "span4", readonly: true }
                        ]
                    },
                    {
                        text: "Color",
                        type: "controls",
                        items: [
                            { name: "ColorName", placeHolder: "Color Name", cls: "span4", readonly: true },
                            { name: "ColorCode", placeHolder: "Color Code", cls: "span4", readonly: true }
                        ]
                    },
                    {
                        text: "Frontman",
                        type: "controls",
                        items: [
                            { name: "ForemanName", placeHolder: "Front Man", cls: "span4", readonly: true },
                            { name: "ForemanID", placeHolder: "Frontman ID", cls: "span4", readonly: true }
                        ]
                    }
                ]
            },
            {
                title: 'Supplier',
                items: [
                    { text: "Supplier", cls: "span4", name: "SupplierCode", display: "SupplierCode", readonly: true },
                    {text: "Nama Supplier", cls: "span4", name: "SupplierName", display: "SupplierName", readonly: true },
                    { name: "Address1", text: "Address", readonly: true },
                    { name: "Address2", text: "", readonly: true },
                    { name: "Address3", text: "", readonly: true },
                    { name: "Address4", text: "", readonly: true },
                    {
                        text: "City",
                        type: "controls",
                        items: [
                            { name: "CityCode", placeHolder: "City Code", cls: "span2", readonly: true },
                            { name: "CityName", placeHolder: "City Name", cls: "span6", readonly: true },
                        ]
                    },
                    { name: "Phone", text: "Phone", cls: "span4", readonly: true },
                    { name: "DiscPct", text: "Disc (%)", cls: "span2", readonly: true },
                    { name: "TOPDays", text: "Jangka Waktu", cls: "span2", readonly: true },
                ]
            },
            {
                title: "Job Details",
                name: "sectionDtl",
                xtype: "table",
                cls: "hide",
                pnlname: "pnlJobDetails",
                tblname: "tblJobDetails",
                items: [
                    {
                        type: "controls",
                        items: [
                            { name: "JobDesc", placeHolder: "Pekerjaan", cls: "span2", readonly: true },
                            { name: "NK", placeHolder: "NK", cls: "span2", readonly: true },
                            { name: "NetPrice", placeHolder: "Harga", cls: "span2" }
                        ]
                    },
                    {
                        type: "buttons", items: [
                            { name: "btnSaveDtl", text: "Save", icon: "icon-save" },
                            { name: "btnCancelDtl", text: "Cancel", icon: "icon-undo" }
                        ]
                    },
                ],
                columns: [
                    { text: "Action", type: "edit", width: 80 },
                    { name: "No", text: "No", width: 40 },
                    { name: "PONo", cls: "hide" },
                    { name: "OperationNo", text: "Pekerjaan", width: 160 },
                    { name: "OperationHour", text: "NK", width: 80 },
                    { name: "POPrice", text: "Harga", cls: "right", width: 120 },
                    { name: "Description", text: "Keterangan" },
                ]
            }
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.post("sv.api/subcon/RcvDefault", function (result) {
            widget.default = result;
            widget.populate(result);
            alterUI('N');
            var year = String(result.Year);
            $("#FPJGovNo").val("010.000-" + year.substring(2,4) + ".000000");
            console.log($("#FPJGovNo").val());
        });
    });

    $('#btnBrowse').on('click', function (e) {
        var lookup = widget.klookup({
            name: "lookupSPKBegin",
            title: "Data Penerimaan Pekerjaan Luar",
            url: "sv.api/grid/subconrcvs",
            serverBinding: true,
            sort: [
                { 'field': 'RecNo', 'dir': 'desc' },
                { 'field': 'RecDate', 'dir': 'desc' }
            ],
            pageSize: 10,
            columns: [
                { field: "RecNo", title: "No. Receiving", width: 150 },
                {
                    field: "RecDate", title: "Nomor Faktur/Nota", width: 130,
                    template: "#= (RecDate == undefined) ? '' : moment(RecDate).format('DD MMM YYYY') #"
                },
                { field: "FPJNo", title: "Nomor Faktur/Nota", width: 130 },
                {
                    field: "FPJDate", title: "Tgl Faktur/Nota", width: 130,
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: "PONo", title: "No. PO", width: 130 },
                {
                    field: "PODate", title: "Tgl PO", width: 130,
                    template: "#= (PODate == undefined) ? '' : moment(PODate).format('DD MMM YYYY') #"
                },
                { field: "JobOrderNo", title: "No. SPK", width: 130 },
                {
                    field: "JobOrderDate", title: "Tgl SPK", width: 130,
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "SupplierCode", title: "Kode Supplier", width: 130 },
                { field: "SupplierName", title: "Nama Supplier", width: 110 },
                { field: "PODisc", title: "Disc", width: 150, format: "{0:#,##0}" },
                { field: "ServiceAmt", title: "Biaya", width: 150, format: "{0:#,##0}" },
                { field: "Description", title: "Status", width: 130 }
            ],
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                widget.populate($.extend({}, widget.default, data));
                getJobOrder(data.JobOrderNo);
                getSupplier(data.SupplierCode);
                alterUI(data.POStatus);
                widget.populateTable({ selector: "#tblJobDetails", url: "sv.api/subcon/GetPODetails?poNo=" + data.PONo });
            }
        })
    });

    $("#PONo").on('blur', function () {
        if ($("#PONo").val() != "") {
            var param = {
                PONo: $("#PONo").val()
            };
            widget.post("sv.api/grid/subcons3", param, function (result) {
                if (!result.Success) {
                    LookUpPONo();
                } else {
                    widget.populate($.extend({}, widget.default, result.data));
                    getJobOrder(result.data.JobOrderNo);
                    getSupplier(result.data.SupplierCode);
                    alterUI(result.data.POStatus);
                }
            });
        }
    });

    $('#btnPONo').on('click', function (e) {
        LookUpPONo();
    });

    function LookUpPONo() {

        var lookup = widget.klookup({
            name: "POList",
            title: "Pesanan",
            url: "sv.api/grid/subcons2",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { 'field': 'PONo', 'dir': 'desc' },
                { 'field': 'PODate', 'dir': 'desc' }
            ],
            columns: [
                { field: "PONo", title: "No PO", width: 140 },
                {
                    field: "PODate", title: "Tgl PO", width: 130,
                    template: "#= (PODate == undefined) ? '' : moment(PODate).format('DD MMM YYYY') #"
                },
                { field: "JobOrderNo", title: "No SPK", width: 160 },
                {
                    field: "JobOrderDate", title: "Tgl SPK", width: 130,
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "SupplierCode", title: "Kode Supplier", width: 140 },
                { field: "SupplierName", title: "Nama Supplier", width: 280 },
                { field: "PODisc", title: "Disc", width: 130, format: "{0:#,##0}", cls: "text-right" },
                { field: "ServiceAmt", title: "Biaya", width: 130, format: "{0:#,##0}", cls: "text-right" },
                { field: "Description", title: "Status", width: 180 }
            ],
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                widget.populate($.extend({}, widget.default, data));
                getJobOrder(data.JobOrderNo);
                getSupplier(data.SupplierCode);
                alterUI(data.POStatus);
            }
        })
    }

    $('#btnCreate').on('click', function (e) {
        widget.clearForm();
        widget.post("sv.api/subcon/rcvdefault", function (result) {
            widget.default = result;
            widget.populate(result);
            alterUI('N');
        });
    });

    $('#btnSave').on('click', function (e) {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            if (data.PONo == '' || data.FPJNo == '' || data.FPJGovNo == '') {
                alert("PO, Nomor dan Seri Pajak harus diisi");
                return;
            }

            widget.post("sv.api/subcon/RcvSave", data, function (result) {
                if (result.Message == "") {
                    $(".main .gl-form").hide();
                    $(".main .gl-grid").show();
                    $('#RecNo').val(result.Record.RecNo);
                    $('#ServiceAmt').val(result.Record.ServiceAmt);
                    $('#POStatus').val(result.Record.POStatus);
                    $('#Description').val(result.Record.Description);
                    alterUI('3');
                    widget.populateTable({ selector: "#tblJobDetails", url: "sv.api/subcon/GetPODetails?poNo=" + result.Record.PONo });
                    widget.refreshGrid();
                } else {
                    alert(result.Message);
                    return;
                }
            });
        }
    });

    $('#btnProcessRcv').on('click', function (e) {
        var valid = $(".main form").valid();
        if (valid) {
            if (confirm('Apakah anda yakin akan melakukan Proses Penerimaan ?')) {
                var data = $(".main form").serializeObject();
                widget.post("sv.api/subcon/RcvProcess", data, function (result) {
                    if (result.Message == '') {
                        $(".main .gl-form").hide();
                        $(".main .gl-grid").show();
                        widget.refreshGrid();
                        $('#POStatus').val(result.Record.POStatus);
                        $('#Description').val(result.Record.Description);
                        alterUI(result.Record.POStatus);
                    } else {
                        alert(result.Message);
                        return;
                    }
                });
            }
        }
    });

    $('#btnCancelRcv').on('click', function (e) {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            var status = $('#POStatus').val();
            if (confirm("Apakah anda yakin akan membatalkan dokumen ?")) {
                widget.post("sv.api/subcon/RcvCancel", data, function (result) {
                    if (result.Message == '') {
                        $(".main .gl-form").hide();
                        $(".main .gl-grid").show();
                        widget.refreshGrid();
                        $('#POStatus').val(result.Record.POStatus);
                        $('#Description').val(result.Record.Description);
                        alterUI(result.Record.POStatus);
                    } else {
                        alert(result.Message);
                        return;
                    }
                });
            }
        }
    });

    $("#btnPrintPO").on("click", function (e) {
        widget.loadForm();
        widget.showForm({ url: "sv/trans/subconrcvprint", params: $("#RecNo").val() });
    });

    widget.onTableClick(function (icon, data) {
        var poStatus = $('#POStatus').val();
        if (poStatus != 'N' && poStatus != '3') {
            return;
        }
        if (icon == "edit") {
            editDetail(data);
        }
    });

    $("#btnSaveDtl").on("click", function () {
        var data = {
            PONo: $('#PONo').val(),
            OperationNo: $('#JobDesc').val(),
            POPrice: $('#NetPrice').val()
        };

        widget.post("sv.api/subcon/savedtl", data, function (result) {
            if (result.Message == '') {
                $(".main .gl-form").hide();
                $(".main .gl-grid").show();
                widget.populateTable({ selector: "#tblJobDetails", url: "sv.api/subcon/GetPODetails?poNo=" + data.PONo });
                $('#ServiceAmt').val(result.Record.ServiceAmt);
            } else {
                alert(result.Message);
            }
        });
        listDetail();
    });

    $("#btnCancelDtl").on("click", function () { listDetail(); });
});

function getJobOrder(jobOrderNo) {
    var data = {
        JobOrderNo: jobOrderNo
    };
    $.ajax({
        type: 'POST',
        data: data,
        url: "sv.api/subcon/getJobOrder",
        success: function (result) {
            if (result.Message == '') {
                $('[name=PoliceRegNo]').val(result.Record.PoliceRegNo);
                $('[name=ChassisCode]').val(result.Record.ChassisCode);
                $('[name=ChassisNo]').val(result.Record.ChassisNo);
                $('[name=EngineCode]').val(result.Record.EngineCode);
                $('[name=EngineNo]').val(result.Record.EngineNo);
                $('[name=ColorName]').val(result.Record.ColorName);
                $('[name=ColorCode]').val(result.Record.ColorCode);
                $('[name=ForemanName]').val(result.Record.ForemanName);
                $('[name=ForemanID]').val(result.Record.ForemanID);
            } else {
                alert(result.Message);
                return;
            }
        }
    });
}

function getSupplier(supplier) {
    var data = {
        supplierCode: supplier
    };
    $.ajax({
        type: 'POST',
        data: data,
        url: "sv.api/subcon/getSupplier",
        success: function (result) {
            $('[name=SupplierName]').val(result.Record.SupplierName);
            $('[name=Address1]').val(result.Record.Address1);
            $('[name=Address2]').val(result.Record.Address2);
            $('[name=Address3]').val(result.Record.Address3);
            $('[name=Address4]').val(result.Record.Address4);
            $('[name=CityCode]').val(result.Record.CityCode);
            $('[name=CityName]').val(result.Record.CityName);
            $('[name=Phone]').val(result.Record.Phone);
            $('[name=DiscPct]').val(result.Record.DiscPct);
            $('[name=TOPDays]').val(result.Record.TOPDays);
        }
    });
}

function listDetail() {
    $("#pnlJobDetails").slideUp();
    $("#tblJobDetails td .icon").addClass("link");
}

function editDetail(data) {
    $("#pnlJobDetails").slideDown();
    $("#tblJobDetails td .icon").removeClass("link");
    $('#JobDesc').val(data[3]);
    $('#NK').val(data[4]);
    $('#NetPrice').val(data[5]);
}

function alterUI(status) {
    if (status == 'N' || status == '2') {
        $('#btnPONo').removeAttr('disabled');
        $('#NetPrice').removeAttr('readonly');
        $('#FPJNo').removeAttr('readonly');
        $('[name=FPJDate]').removeAttr('disabled');
        $('#FPJGovNo').removeAttr('readonly');
        $('#sectionDtl').addClass('hide');
        $('#btnSave').removeClass('hide');
        $('#btnProcessRcv').addClass('hide');
        $('#btnCancelRcv').addClass('hide');
    } else if (status == '3') {
        $('#btnPONo').attr('disabled', 'disabled');
        $('#NetPrice').removeAttr('readonly');
        $('#sectionDtl').removeClass('hide');
        $('#btnSave').removeClass('hide');
        $('#btnProcessRcv').removeClass('hide');
        $('#btnCancelRcv').removeClass('hide');
    } else {
        $('#btnPONo').attr('disabled', 'disabled');
        $('#FPJNo').attr('readonly', 'readonly');
        $('[name=FPJDate]').attr('disabled', 'disabled');
        $('#FPJGovNo').attr('readonly', 'readonly');
        $('#NetPrice').attr('readonly', 'readonly');
        $('#sectionDtl').removeClass('hide');
        $('#btnSave').addClass('hide');
        $('#btnProcessRcv').addClass('hide');
        $('#btnCancelRcv').addClass('hide');
    }
}