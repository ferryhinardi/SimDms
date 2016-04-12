$(document).ready(function () {
    var options = {
        title: 'Input Pesanan Pekerjaan Luar',
        xtype: 'panels',       
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnCreatePO", text: "Create PO", icon: "icon-list-alt", cls: "hide" },
            { name: "btnCancelPO", text: "Cancel PO", icon: "icon-undo", cls: "hide" },
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
                title: 'PO',
                items: [
                    { name: "PONo", text: "PO Number", type: "popup", placeHolder: "POT/XX/999999", cls: "span4", readonly: true },
                    { name: "PODate", text: "PO Date", type: "date", cls: "span4", readonly: true },
                ]
            },
            {
                title: 'SPK',
                items: [
                    { name: "JobOrderNo", text: "SPK No", type: "popup", placeHolder: "XXX/YY/99999", cls: "span4" },
                    { name: "JobOrderDate", text: "SPK Date", type: "date", cls: "span4", readonly: true },
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
                    {
                        text: "Supplier", cls: "span8",
                        type: "controls",
                        items: [
                            { name: "SupplierCode", cls: "span2", placeHolder: "Supplier Code", readonly: true, type: "popup", btnName: "btnSupplierCode" },
                            { name: "SupplierName", cls: "span6", placeHolder: "Supplier Name", readonly: true },
                        ]

                    },
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
                title: 'Notes',
                items: [
                    { name: "Remarks", type: "textarea" }
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
                            { name: "NetPrice", placeHolder: "Harga", cls: "span2 number", format: "{0:#,##0}" }
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
                    { text: "Action", type: "action", width: 120 },
                    { name: "No", text: "No", width: 40 },
                    { name: "PONo", cls: "hide" },
                    { name: "OperationNo", text: "Pekerjaan", width: 160 },
                    { name: "OperationHour", text: "NK", width: 80 },
                    { name: "POPrice", text: "Harga", cls: "right", width: 120, format: "{0:#,##0}" },
                    { name: "Description", text: "Keterangan" },
                ]
            }
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.post("sv.api/subcon/default", function (result) {
            widget.default = result;
            widget.populate(result);
            alterUI('N');
        });
    });

    $("#btnPrintPO").on("click", function (e) {
        widget.loadForm();
        widget.showForm({ url: "sv/trans/subconprint" ,params:$("#PONo").val()});
    });

    $('#btnCreate').on('click', function (e) {
        widget.clearForm();
        widget.post("sv.api/subcon/default", function (result) {
            widget.default = result;
            widget.populate(result);
            alterUI('N');
        });
    });

    $('#btnBrowse, #btnPONo').on('click', function (e) {
        var lookup = widget.klookup({
            name: "SubConList",
            title: "Input Pesanan Lookup",
            url: "sv.api/grid/subcons",
            serverBinding: true,
            pageSize: 10,
            filters: [
             {
                 text: "Semua Status",
                 type: "controls",
                 cls: "span8",
                 items: [
                         {
                             name: "ShowAll", type: "select", text: "", cls: "span2", items: [
                                 { value: "1", text: "Ya" },
                                 { value: "0", text: "Tidak", selected: 'selected' }
                             ]
                         }
                 ]
             }],
            sort: [
                { 'field': 'PONo', 'dir': 'desc' },
                { 'field': 'PODate', 'dir': 'desc' },
            ],
            columns: [
                { field: "PONo", title: "No PO", width: 140 },
                {
                    field: "PODate", title: "Tgl PO", width: 130,
                    template: "#= (PODate == undefined) ? '' : moment(PODate).format('DD MMM YYYY') #"
                },
                { field: "JobOrderNo", title: "No SPK", width: 140 },
                {
                    field: "JobOrderDate", title: "Tgl SPK", width: 130,
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "SupplierCode", title: "Kode Supplier", width: 140 },
                { field: "SupplierName", title: "Nama Supplier", width: 280 },
                { field: "PODisc", title: "Disc", width: 160, format: "{0:#,##0}", attributes: { class: "text-right" } },
                { field: "ServiceAmt", title: "Biaya", width: 160, format: "{0:#,##0}", attributes: {class: "text-right"}},
                { field: "Description", title: "Status", width: 200, format: "{0:#,##0}", attributes: { class: "text-right" } }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                widget.populate($.extend({}, widget.default, data));
                getJobOrder(data.JobOrderNo);
                getSupplier(data.SupplierCode);
                widget.populateTable({ selector: "#tblJobDetails", url: "sv.api/subcon/GetPODetails?poNo=" + data.PONo });
                alterUI(data.RefferenceCode);
            }
        });
    });

    $('#btnSave').on('click', function (e) {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            if (data.JobOrderNo == '' || data.SupplierCode == '') {
                alert("No. SPK dan Kode Supplier harus diisi");
                return;
            }

            widget.post("sv.api/subcon/save", data, function (result) {
                if (result.Message == "") {
                    $(".main .gl-form").hide();
                    $(".main .gl-grid").show();
                    $('#PONo').val(result.Record.PONo);
                    $('#ServiceAmt').val(result.Record.ServiceAmt);
                    $('#POStatus').val(result.Record.POStatus);
                    $('#Description').val(result.Record.Description);

                    widget.populateTable({ selector: "#tblJobDetails", url: "sv.api/subcon/GetPODetails?poNo=" + result.Record.PONo });
                    alterUI('0');
                    widget.refreshGrid();
                } else {
                    alert(result.Message);
                    return;
                }
            });
        }
    });

    $('#btnCreatePO').on('click', function (e) {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            widget.post("sv.api/subcon/createPO", data, function (result) {
                if (result.Message == '') {
                    $(".main .gl-form").hide();
                    $(".main .gl-grid").show();
                    widget.refreshGrid();
                    $('#POStatus').val(result.Record.POStatus);
                    $('#Description').val(result.Record.Description);
                    alterUI('2');
                } else {
                    alert(result.Message);
                    return;
                }
            });
        }
    });

    $('#btnCancelPO').on('click', function (e) {
        var valid = $(".main form").valid();
        if (valid) {
            var data = $(".main form").serializeObject();
            var status = $('#POStatus').val();
            var conf = true;
            if (status == '0') {
                conf = confirm("Anda yakin akan melakukan cancel PO?");
            }
            if (conf == true) {
                widget.post("sv.api/subcon/cancelPO", data, function (result) {
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

    $("#JobOrderNo").on('blur', function () {
        if ($("#JobOrderNo").val() != "") {
            var param = {
                JobOrderNo: $("#JobOrderNo").val()
            };
            widget.post("sv.api/subcon/SPKPesananPekerjaanLuar", param, function (result) {
                if (!result.Success) {
                    LookUpJobOrderNo();
                }else {
                    widget.populate($.extend({}, widget.default, result.data));
                    var params = {
                        ColorCode: result.data.ColorCode,
                        ForemanID: result.data.ForemanID
                    };
                    widget.post("sv.api/subcon/PopupateDescription", params, function (result) {
                        result.ColorName = result.ColorName;
                        result.ForemanName = result.ForemanName;
                        widget.populate($.extend({}, widget.default, result));
                    });
                }
            });
        }
    });

    $('#btnJobOrderNo').on('click', function (e) {
        LookUpJobOrderNo();
    });

    function LookUpJobOrderNo() {
        var lookup = widget.klookup({
            name: "JobOrderList",
            title: "Input Perawatan Kendaraan",
            url: "sv.api/subcon/LookUpSPKPesananPekerjaanLuar",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { 'field': 'JobOrderNo', 'dir': 'desc' },
                { 'field': 'JobOrderDate', 'dir': 'desc' },
            ],
            filters: [
            {
                text: "Semua Status",
                type: "controls",
                cls: "span8",
                items: [
                        {
                            name: "ShowAll", type: "select", text: "", cls: "span2", items: [
                                { value: "0", text: "Ya" },
                                { value: "1", text: "Tidak", selected: 'selected' }
                            ]
                        }
                ]
            }
            ],
            columns: [
                { field: "JobOrderNo", title: "No. SPK", width: 150 },
                {
                    field: "JobOrderDate", title: "Tgl. SPK", width: 130,
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "PoliceRegNo", title: "No Polisi", width: 160 },
                { field: "ServiceBookNo", title: "No Buku Service", width: 160 },
                { field: "BasicModel", title: "Model", width: 130 },
                { field: "TransmissionType", title: "Tipe Transmisi", width: 130 },
                { field: "KodeRangka", title: "Kode Rangka", width: 200 },
                { field: "KodeMesin", title: "Kode Mesin", width: 160 },
                { field: "ColorCode", title: "Warna", width: 100 },
                { field: "Customer", title: "Pelanggan", width: 300 },
                { field: "CustomerBill", title: "Pembayar", width: 300 },
                { field: "Odometer", title: "Odometer(KM)", width: 160, format: "{0:#,##0}", filterable: false, attributes: { class: "text-right" } },
                { field: "JobType", title: "Jenis Pekerjaan", width: 160 },
                { field: "ForemanID", title: "Foreman", width: 150 },
                { field: "MechanicID", title: "Mekanik", width: 150 },
                { field: "ServiceStatus", title: "Status", width: 160 },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                widget.populate($.extend({}, widget.default, data));
                var params = {
                    ColorCode: data.ColorCode,
                    ForemanID: data.ForemanID
                };
                widget.post("sv.api/subcon/PopupateDescription", params, function (result) {
                    data.ColorName = result.ColorName;
                    data.ForemanName = result.ForemanName;
                    widget.populate($.extend({}, widget.default, data));
                });

            }
        });
    }

    $('#btnSupplierCode').on('click', function (e) {
        var lookup = widget.klookup({
            name: "SupplierList",
            title: "Supplier Lookup",
            url: "sv.api/grid/suppliersforsubcon",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { 'field': 'SupplierCode', 'dir': 'asc' },
                { 'field': 'SupplierName', 'dir': 'asc' },
            ],
            columns: [
                { field: "SupplierCode", title: "Kode Supplier", width: 160 },
                { field: "SupplierName", title: "Nama Supplier", width: 280 },
                { field: "Status", title: "Status", width: 160 }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                widget.populate($.extend({}, widget.default, data));
            }
        });
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

    widget.onTableClick(function (icon, data) {
        var poStatus = $('#POStatus').val();
        if (poStatus != 'N' && poStatus != '0') {
            return;
        }
        switch (icon) {
            case "edit":
                editDetail(data);
                break;
            case "trash":
                if (confirm("Anda yakin akan menghapus data ini?")) {
                    var dtl = {
                        PONo: data[2],
                        OperationNo: data[3],
                        OperationHour: data[4],
                        POPrice: data[5],
                        Description: data[6]
                    };

                    widget.post("sv.api/subcon/RemoveDtl", dtl, function (result) {
                        widget.populateTable({ selector: "#tblJobDetails", url: "sv.api/subcon/GetPODetails?poNo=" + dtl.PONo });
                    });
                }
                break;
            default:
                break;
        }
    });
});

function listDetail() {
    $("#pnlJobDetails").slideUp();
    $("#tblJobDetails td .icon").addClass("link");
}

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
            if (result != undefined) {
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
        }
    });
}

function editDetail(data) {
    $("#pnlJobDetails").slideDown();
    $("#tblJobDetails td .icon").removeClass("link");
    $('#JobDesc').val(data[3]);
    $('#NK').val(data[4]);
    $('#NetPrice').val(data[5]);
}

function deleteDetail(data) {
    if (confirm("Anda yakin akan menghapus data ini?")) {
        widget.post("sv.api/subcon/RemoveDtl", data, function (result) {
            widget.populateTable({ selector: "#tblJobDetails", url: "sv.api/subcon/GetPODetails?poNo=" + data.PONo });
        });
    }
}



function alterUI(status) {
    if (status == 'N') {
        $('#btnJobOrderNo').removeAttr('disabled');
        $('#btnSupplierCode').removeAttr('disabled');
        $('#NetPrice').removeAttr('readonly');
        $('#sectionDtl').addClass('hide');
        $('#btnSave').removeClass('hide');
        $('#btnCreatePO').addClass('hide');
        $('#btnCancelPO').addClass('hide');
    } else if (status == '0') {
        $('#btnJobOrderNo').attr('disabled', 'disabled');
        $('#btnSupplierCode').removeAttr('disabled');
        $('#NetPrice').removeAttr('readonly');
        $('#sectionDtl').removeClass('hide');
        $('#btnSave').removeClass('hide');
        $('#btnCreatePO').removeClass('hide');
        $('#btnCancelPO').removeClass('hide');
    } else {
        $('#btnJobOrderNo').attr('disabled', 'disabled');
        $('#btnSupplierCode').attr('disabled', 'disabled');
        $('#NetPrice').attr('readonly', 'readonly');
        $('#sectionDtl').removeClass('hide');
        $('#btnSave').addClass('hide');
        if (status == '2') {
            $('#btnCreatePO').addClass('hide');
            $('#btnCancelPO').removeClass('hide');
        } else {
            $('#btnCreatePO').addClass('hide');
            $('#btnCancelPO').addClass('hide');
        }
    }
}