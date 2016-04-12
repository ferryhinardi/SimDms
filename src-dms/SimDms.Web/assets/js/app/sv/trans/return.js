$(document).ready(function () {
    var options = {
        title: 'Retur Service',
        xtype: 'panels',
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search"},
            { name: "btnSave", text: "Save", icon: "icon-save" },
            { name: "btnPrint", text: "Print", icon: "icon-print" },

        ],
        panels: [
            {
                title: 'Informasi Faktur Penjualan',
                items: [
                    { name: "TotalServiceAmt", text: "Total Biaya Perawatan", cls: "span4 number right", readonly: true },
                    { name: "ServiceStatusDesc", text: "Status", cls: "span4", readonly: true },
                    { name: "ServiceStatus", cls: "span4 hide", readonly: true },
                    { name: "ReturnNo", text: "No. Retur", cls: "span4", readonly: true },
                    {
                        text: "No. Faktur Penjualan", cls: "span4",
                        type: "popup",
                        name: "InvoiceNo",
                        btnName: "btnInvoiceNo",
                        readonly: true,
                        validasi: "required"
                    },
                    { name: "JobOrderNo", text: "No. Retur", cls: "span4 hide", readonly: true },
                    { name: "RefferenceNo", text: "No. Ref", cls: "span4", validasi: "required" },
                    { name: "RefferenceDate", text: "Tgl. Ref", type: "date", cls: "span4", readonly: true },
                    {
                        text: "Sign",
                        cls: "span4",
                        type: "popup",
                        name: "SignName",
                        validasi: "required"
                    },
                    {
                        text: "Print Nota Retur",
                        type: "controls",
                        cls: "span6",
                        items: [
                            { name: "chkNotaRetur", type: "check", cls: "span2", float: "left" },
                            { name: "selectNotaRetur", type: "select", cls: "span2", items: [{ value: "1", text: "Standar" }, { value: "2", text: "Rinci" }, { value: "4", text: "Lampiran" }] }
                        ]
                    },
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "CustVehicle", text: "Kendaraan & Pelanggan" },
                    { name: "TaskService", text: "Pekerjaan" }, 
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Kendaraan",
                items: [
                    { name: "PoliceRegNo", text: "No Polisi", cls: "span4", readonly: true },
                    { name: "ServiceBookNo", text: "No Buku Service", cls: "span4", readonly: true },
                    { name: "BasicModel", text: "Basic Model", cls: "span4", readonly: true },
                    { name: "TransmissionType", text: "Transmisi", cls: "span4", readonly: true },
                    { name: "ChassisCode", text: "Kode Rangka", cls: "span4", readonly: true },
                    { name: "ChassisNo", text: "No. Rangka", cls: "span4", readonly: true },
                    { name: "EngineCode", text: "Kode Mesin", cls: "span4", readonly: true },
                    { name: "EngineNo", text: "No. Mesin", cls: "span4", readonly: true },
                    { name: "ColourCode", text: "Warna Kendaraan", cls: "span4", readonly: true },
                    { name: "Odometer", text: "KM (Odometer)", cls: "span4", readonly: true },
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Pelanggan",
                items: [
                    {
                        text: "Nama",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", placeHolder: "Kode Customer", cls: "span2", readonly: true },
                            { name: "CustomerName", placeHolder: "Nama Customer", cls: "span6", readonly: true },
                        ]
                    },
                    { name: "CustAddr1", text: "Alamat", readonly: true },
                    { name: "CustAddr2", text: "", readonly: true },
                    { name: "CustAddr3", text: "", readonly: true },
                    { name: "CustAddr4", text: "", readonly: true },
                    {
                        text: "Kota",
                        type: "controls",
                        items: [
                            { name: "CityCode", placeHolder: "City Code", cls: "span2", readonly: true },
                            { name: "CityName", placeHolder: "City Name", cls: "span6", readonly: true },
                        ]
                    },
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Kontrak",
                items: [
                    { name: "ContractNo", text: "No Kontrak", cls: "span4", readonly: true },
                    { name: "ContractEndPeriod", text: "Berlaku s/d Tgl", type: "date", cls: "span4", readonly: true },
                    { name: "ContractStatus", text: "Status", cls: "span4 hide", readonly: true },
                    { name: "ContractStatusDesc", text: "Status", cls: "span4", readonly: true }
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Club",
                items: [
                    { name: "ClubCode", text: "No Club", cls: "span4", readonly: true },
                    { name: "ClubEndPeriod", text: "Berlaku s/d Tgl", type: "date", cls: "span4", readonly: true },
                    { name: "ClubStatusDesc", text: "Status", cls: "span4", readonly: true },
                    { name: "ClubStatus", cls: "span4 hide", readonly: true }
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Pembayar",
                items: [
                    { name: "InsurancePayFlag", type: "switch", text: "Asuransi", float: "left", readonly: true },
                    { name: "InsuranceOwnRisk", text: "Own Risk", cls: "span4", readonly: true },
                    { name: "InsuranceNo", text: "No Polis", cls: "span4", readonly: true },
                    { name: "InsuranceJobOrderNo", text: "No SPK Asuransi", cls: "span4", readonly: true },
                    {
                        text: "Nama",
                        type: "controls",
                        items: [
                            { name: "CustomerCodeBill", placeHolder: "Kode Customer", cls: "span2", readonly: true },
                            { name: "CustomerNameBill", placeHolder: "Nama Customer", cls: "span6", readonly: true },
                        ]
                    },
                    { name: "CustAddr1Bill", text: "Alamat", readonly: true },
                    { name: "CustAddr2Bill", text: "", readonly: true },
                    { name: "CustAddr3Bill", text: "", readonly: true },
                    { name: "CustAddr4Bill", text: "", readonly: true },
                    {
                        text: "Kota",
                        type: "controls",
                        items: [
                            { name: "CityCodeBill", placeHolder: "City Code", cls: "span2", readonly: true },
                            { name: "CityNameBill", placeHolder: "City Name", cls: "span6", readonly: true },
                        ]
                    },
                    {
                        text: "Telp/Fax/HP",
                        type: "controls",
                        items: [
                            { name: "PhoneNoBill", placeHolder: "Telepon", cls: "span2", readonly: true },
                            { name: "FaxNoBill", placeHolder: "Fax", cls: "span2", readonly: true },
                            { name: "HPNoBill", placeHolder: "HP", cls: "span2", readonly: true }
                        ]
                    },
                    {
                        text: "% Diskon",
                        type: "controls",
                        items: [
                            { name: "LaborDiscPct", placeHolder: "Jasa", cls: "span2", readonly: true },
                            { name: "PartDiscPct", placeHolder: "Part", cls: "span2", readonly: true },
                            { name: "MaterialDiscPct", placeHolder: "Material", cls: "span2", readonly: true },
                        ]
                    },
                    { name: "IsPPN", type: "switch", text: "PPN", float: "left", readonly: true }
                ]
            },
            {
                cls: "tabpage1 TaskService",
                title: "Pekerjaan",
                items: [
                    { name: "ServiceRequestDesc", text: "Permintaan Perawatan", type: "textarea", cls: "span8", readonly: true },
                    {
                        text: "Jenis Pekerjaan",
                        type: "controls",
                        items: [
                            { name: "JobType", placeHolder: "Jenis", cls: "span2", readonly: true },
                            { name: "JobTypeDesc", placeHolder: "Keterangan", cls: "span6", readonly: true },
                        ]
                    },
                    { name: "ConfirmChangingPart", type: "switch", text: "Izin Pemilik", float: "left", cls: "readonly" },
                    { name: "EstimateFinishDate", text: "Perkiraan Selesai", type: "date", cls: "span4", readonly: true },
                    {
                        text: "SA",
                        type: "controls",
                        items: [
                            { name: "ForemanID", placeHolder: "ID", cls: "span2", readonly: true },
                            { name: "ForemanName", placeHolder: "Nama", cls: "span6", readonly: true },
                        ]
                    },
                    {
                        text: "FM",
                        type: "controls",
                        items: [
                            { name: "MechanicID", placeHolder: "ID", cls: "span2", readonly: true },
                            { name: "MechanicName", placeHolder: "Nama", cls: "span6", readonly: true },
                        ]
                    }
                ]
            },
            {
                cls: "tabpage1 TaskService",
                title: "Total Biaya Perawatan",
                items: [
                    { name: "LaborDppAmt", text: "DPP - Jasa", cls: "span4", readonly: true },
                    { name: "TotalDppAmt", text: "Total DPP", cls: "span4", readonly: true },
                    { name: "PartsDppAmt", text: "DPP - Part", cls: "span4", readonly: true },
                    { name: "TotalPpnAmt", text: "Total PPN", cls: "span4", readonly: true },
                    { name: "MaterialDppAmt", text: "DPP - Material", cls: "span4", readonly: true },
                    { name: "TotalServiceAmt", text: "Total Biaya Perawatan", cls: "span4", readonly: true }
                ]
            },
            {
                title: "Detail Pekerjaan",
                name: "sectionDtl",
                xtype: "table",
                cls: "tabpage1 TaskService",
                tblname: "tblRtJobDetails",
                columns: [
                    { name: "SeqNo", text: "No", width: 40 },
                    { name: "BillTypeDesc", text: "Ditanggung Oleh", width: 160 },
                    { name: "TypeOfGoodsDesc", text: "Jenis Item", width: 80 },
                    { name: "TaskPartNo", text: "No. Part / Pekerjaan", cls: "right", width: 180 },
                    { name: "SupplyQty", text: "Qty Supply" },
                    { name: "OprRetailPriceTotal", text: "Harga" },
                    { name: "SupplySlipNo", text: "No Supply Slip" },
                    { name: "TaskPartDesc", text: "Keterangan" }
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        widget.post("sv.api/return/Default", function (result) {
            widget.default = result;
            widget.populate(result);
            alterUI('N');
            $('[name=ConfirmChangingPart]').attr('disabled', 'disabled');
            $('[name=InsurancePayFlag]').attr('disabled', 'disabled');
            $('[name=IsPPN]').attr('disabled', 'disabled');
        });
    });

    $('#btnCreate').on('click', function (e) {
        location.reload();
    });

    $('#btnBrowse, #btnInvoiceNo').on('click', function (e) {
        widget.lookup.init({
            name: "ReturnList",
            title: "Faktur Pajak Lookup",
            source: "sv.api/grid/ReturnServices",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "InvoiceNo", sTitle: "No. Faktur Penjualan", sWidth: "80px" },
                {
                    mData: "InvoiceDate", sTitle: "Tgl. Faktur Penjualan", sWidth: "80px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD-MMM-YYYY');
                    }
                },
                { mData: "FPJNo", sTitle: "No. Faktur Pajak", sWidth: "120px" },
                {
                    mData: "FPJDate", sTitle: "Tgl. Faktur Pajak", sWidth: "120px",
                    mRender: function (data, type, full) {
                        if (data == null) { return ''; } else {
                            return moment(data).format('DD-MMM-YYYY');
                        }
                    }
                },
                { mData: "ReturnNo", sTitle: "Return No", sWidth: "100px" },
                { mData: "InvoiceStatus", sTitle: "Status", sWidth: "100px" },
                { mData: "JobOrderNo", sTitle: "No. SPK", sWidth: "100px" },
                {
                    mData: "JobOrderDate", sTitle: "Tgl. SPK", sWidth: "90px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD-MMM-YYYY');
                    }
                },
                { mData: "PoliceRegNo", sTitle: "No. Polisi", sWidth: "100px" },
                { mData: "ServiceBookNo", sTitle: "No. Polisi", sWidth: "120px" },
                { mData: "JobType", sTitle: "Jenis Pekerjaan", sWidth: "110px" },
                { mData: "ChassisCode", sTitle: "Kode Rangka", sWidth: "100px" },
                { mData: "ChassisNo", sTitle: "No. Rangka", sWidth: "90px" },
                { mData: "EngineCode", sTitle: "Kode Mesin", sWidth: "100px" },
                { mData: "EngineNo", sTitle: "No. Mesin", sWidth: "90px" },
                { mData: "BasicModel", sTitle: "Basic Model", sWidth: "100px" },
                { mData: "Customer", sTitle: "Pelanggan", sWidth: "250px" },
                { mData: "CustomerBill", sTitle: "Pembayar", sWidth: "250px" },
            ]
        });
        widget.lookup.show();
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.populate($.extend({}, widget.default, data));
        if (name == 'ReturnList') {
            getInvoice(data.InvoiceNo);
        }

        widget.lookup.hide();
    });

    $('#btnSave').on('click', function (e) {
        if ($('#InvoiceNo').val() == "" || $('#ServiceStatus').val() == "8") {
            MsgBox("No. Faktur belum diisi, atau Status faktur tidak bisa diretur");
            return;
        }
        if ($('#RefferenceNo').val().trim() == "" || $('#SignName').val().trim() == "") {
            MsgBox("No.Ref atau Sign belum diisi");
            return;
        }

        MsgConfirm("Apakah anda yakin akan melakukan proses Cancel Faktur ?", function(e) {
            if(e) {
                var data = $(".main form").serializeObject();
                processInvoice(data);
            }
        });
    });


    $('#chkNotaRetur').on('change', function (e) {
        if ($('#chkNotaRetur').prop('checked')) {
            $('#selectNotaRetur').show();
        } else {
            $('#selectNotaRetur').hide();
        }
    });

    $('#btnSignName').on('click', function (e) {
        widget.lookup.init({
            name: "SignerList",
            title: "Penanda Tangan",
            source: "sv.api/grid/Signatures",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "SignName", sTitle: "Nama", sWidth: "120px" },
                { mData: "TitleSign", sTitle: "Jabatan", sWidth: "120px" }
            ]
        });
        widget.lookup.show();
    });

    function getInvoice(InvoiceNo) {
        var data = {
            InvoiceNo: InvoiceNo
        };

        $.ajax({
            type: 'POST',
            data: data,
            url: "sv.api/return/GetInvoiceComplete",
            success: function (result) {
                if (result.Message == '') {
                    var data = result.Record || {};
                    var header = {
                        InvoiceStatus: data.InvoiceStatus,

                        CustomerName: data.CustomerName,
                        CustAddr1: data.CustAddr1,
                        CustAddr2: data.CustAddr2,
                        CustAddr3: data.CustAddr3,
                        CustAddr4: data.CustAddr4,
                        CityCode: data.CityCode,
                        CityName: data.CityName,
                        ServiceStatusDesc: data.ServiceStatusDesc,
                        
                        ContractNo: data.IsContract ? data.ContractNo : "",
                        ContractEndPeriod: data.IsContract ? (data.ContractEndPeriod != null ? data.ContractEndPeriod.toString() : "") : "",
                        ContractStatusDesc: data.IsContract ? data.ContractStatusDesc : "",
                        ContractStatus: data.IsContract ? data.ContractStatus : "",
                        
                        ClubCode: data.IsClub ? data.ClubCode : "",
                        ClubEndPeriod: data.IsClub ? (data.ClubEndPeriod != null ? data.ClubEndPeriod.toString() : "") : "",
                        ClubStatusDesc: data.IsClub ? data.ClubStatusDesc : "",
                        ClubStatus: data.IsClub ? data.ClubStatus : "",

                        CustomerNameBill: data.CustomerNameBill,
                        CustAddr1Bill: data.CustAddr1Bill,
                        CustAddr2Bill: data.CustAddr2Bill,
                        CustAddr3Bill: data.CustAddr3Bill,
                        CustAddr4Bill: data.CustAddr4Bill,
                        CityCodeBill: data.CityCodeBill,
                        CityNameBill: data.CityNameBill,
                        PhoneNoBill: data.PhoneNoBill,
                        FaxNoBill: data.FaxNoBill,
                        HPNoBill: data.HPNoBill,

                        LaborDiscPct: data.LaborDiscPct,
                        PartDiscPct: data.PartDiscPct,
                        MaterialDiscPct: data.MaterialDiscPct,

                        IsPPN: data.TaxCode === "PPN" && data.TaxPct > 0 ? true : false,

                        JobTypeDesc: data.JobTypeDesc,
                        ForemanID: data.ForemanID,
                        ForemanName: data.ForemanName,
                        MechanicID: data.MechanicID,
                        MechanicName: data.MechanicName,
                        ServiceStatus: data.ServiceStatus
                    };
                    widget.populate(header);
                    widget.hideAjaxLoad();

                    getTrnService(data);
                    if(data.InvoiceStatus == '4'){
                        getReturnService(data.InvoiceNo);
                        //$('#btnSave').show();
                    }
                    if (header.ServiceStatus == "8") {
                        alterUI("S");
                        //console.log("Status = " + header.ServiceStatus)
                    } else {
                        alterUI("N");
                        $('#btnSave').show();
                        //console.log("Status = " + header.ServiceStatus)
                    }
                } else {
                    alert(result.Message);
                    return;
                } 
            }
        });
    }

    $('#btnPrint').on('click', function (e) {
        if ($('[name="SignName"]').val() == "") {
            MsgBox("Sign harus diisi!", MSG_INFO)
            return
        }
        var rptID = "SvRpTrn018"
        var rptData = widget.default.ProductType + "," + $('[name="InvoiceNo"]').val() + "," + $('[name="InvoiceNo"]').val()
        var rparam = $('[name="SignName"]').val() + "," + $('[name="SignName"]').val();


        //console.log(rptData);

        if ($('#chkNotaRetur').prop('checked'))
        {
            rptID = "SvRpTrn019"
            if ($('#selectNotaRetur').val() != "")
            {
                rptData = widget.default.ProductType + "," + $('[name="ReturnNo"]').val() + "," + $('[name="ReturnNo"]').val() + "," + $('#selectNotaRetur').val();
            } else {
                MsgBox("Silahkan pilih nota retur dahulu!", MSG_INFO);
                return;
            }
        }

        console.log(rptData);

        widget.showPdfReport({
            id: rptID,
            pparam: rptData,
            rparam: rparam,
            type: "devex"
        });
    });

    function getInvoiceDetails(data) {
        if (data.JobOrderNo == "" || data.JobOrderNo == null) return;
        widget.populateTable({
            selector: "#tblRtJobDetails",
            url: "sv.api/return/GetInvoiceDetails?invoiceNo=" + data.InvoiceNo +
                "&jobType=" + data.JobType + "&serviceNo=" + data.ServiceNo +
                "&jobOrderNo=" + data.JobOrderNo,
        });
    }

    function getTrnService(invoice) {
        var data = {
            jobOrderNo: invoice.JobOrderNo
        };

        $.ajax({
            type: 'POST',
            data: data,
            url: "sv.api/return/GetTrnService",
            success: function (result) {
                if (result.Message == '') {
                    var jobOrder = result.Record || {};
                    var header = {
                        InsurancePayFlag: jobOrder.InsurancePayFlag,
                        InsuranceNo: jobOrder.InsuranceNo,
                        InsuranceJobOrderNo: jobOrder.InsuranceJobOrderNo,
                        CustomerCodeBill: jobOrder.CustomerCodeBill,
                        ServiceRequestDesc: jobOrder.ServiceRequestDesc,
                        ConfirmChangingPart: jobOrder.ConfirmChangingPart,
                        EstimateFinishDate: jobOrder.EstimateFinishDate
                    }
                    widget.populate(header);
                    widget.hideAjaxLoad();

                    getInvoiceDetails(jobOrder);
                    if (jobOrder.InvoiceNo == null || jobOrder.InvoiceNo == "") {
                        var header1 = {
                            LaborDppAmt: invoice.LaborDppAmt,
                            PartsDppAmt: invoice.PartsDppAmt,
                            MaterialDppAmt: invoice.MaterialDppAmt,
                            TotalDppAmt: invoice.TotalDppAmt,
                            TotalPpnAmt: invoice.TotalPpnAmt,
                            TotalServiceAmt: invoice.TotalDppAmt + invoice.TotalPpnAmt
                        }
                        widget.populate(header1);
                        widget.hideAjaxLoad();
                    } else {
                        $.ajax({
                            type: 'POST',
                            data: { invoiceNo: jobOrder.InvoiceNo },
                            url: 'sv.api/return/GetInvoiceHdr',
                            success: function (invHdr) {
                                if (result.Message == '') {
                                    var header1 = {
                                        LaborDppAmt: invHdr.Record.LaborDppAmt,
                                        PartsDppAmt: invHdr.Record.PartsDppAmt,
                                        MaterialDppAmt: invHdr.Record.MaterialDppAmt,
                                        TotalDppAmt: invHdr.Record.TotalDppAmt,
                                        TotalPpnAmt: invHdr.Record.TotalPpnAmt,
                                        TotalServiceAmt: invHdr.Record.TotalDppAmt + invHdr.Record.TotalPpnAmt
                                    }
                                    widget.populate(header1);
                                    widget.hideAjaxLoad();
                                } else {
                                    alert(result.Message);
                                    return;
                                }
                            }
                        });
                    }

                } else {
                    alert(result.Message);
                    return;
                }
            }
        });
    }

    function getReturnService(InvoiceNo) {
        var data = {
            InvoiceNo: InvoiceNo
        };

        $.ajax({
            type: 'POST',
            data: data,
            url: "sv.api/return/GetReturnService",
            success: function (result) {
                if (result.Message == '') {
                    var data = result.Record || {};
                    var header = {
                        RefferenceNo: data.RefferenceNo,
                        ReturnNo: data.ReturnNo
                    }
                    widget.populate(header);
                    widget.hideAjaxLoad();
                } else {
                    alert(result.Message);
                    return;
                }
            }
        });
    }

    function processInvoice(data) {
        if ($('#JobOrderNo').val().trim() == "") return;
        $.ajax({
            type: 'POST',
            data: data,
            url: 'sv.api/return/ProcessInvoice',
            success: function (result) {
                if (result.Message != "") {
                    alert(result.Message);
                    $('#btnPrint').hide();
                    $('#btnSave').hide();
                    return;
                } else {
                    if (result.ReturnNo != undefined) {
                        $('#ReturnNo').val(result.ReturnNo);
                        // PRINT
                        updateStatus(data.JobOrderNo, data.InvoiceNo);
                    }
                }
            }
        });
    }

    function updateStatus(jobOrderNo, invoiceNo) {
        $.ajax({
            type: 'POST',
            data: { jobOrderNo: jobOrderNo, invoiceNo: invoiceNo },
            url: 'sv.api/return/PostSave',
            success: function (result) {
                getInvoice(invoiceNo);
                alterUI("S");
            }
        });
    }
});


function alterUI(status) {
    if (status == "N") {
        $('#SignName').removeAttr('readonly');
        $('#RefferenceNo').removeAttr('readonly');
        $('#btnSignName').removeAttr('disabled');
        $('#InvoiceNo').removeAttr('readonly');
        $('#btnInvoiceNo').removeAttr('disabled');

        if ($('#chkNotaRetur').prop('checked')) {
            $('#selectNotaRetur').show();
        } else {
            $('#selectNotaRetur').hide();
        }
        $('#btnSave').hide();
        $('#btnPrint').hide();
    }
    else if (status == "S") {
        $('#SignName').removeAttr('readonly');
        $('#RefferenceNo').removeAttr('readonly');
        $('#btnSignName').removeAttr('disabled');
        $('#InvoiceNo').attr('readonly', 'readonly');
        $('#RefferenceNo').attr('readonly', 'readonly');
        $('#btnInvoiceNo').attr('disabled', 'disabled');
        $('#btnSave').hide();
        $('#btnPrint').show();
    }
}