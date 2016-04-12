var widget;
var fileData;
var sourceData;

$(document).ready(function () {
    var options = {
        title: "Input KSG",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save" }
        ],
        panels: [
            {
                name: "pnlBranch",
                title: "Sub Dealer / Branch",
                items: [
                    { name: "GenerateNo", text: "No. PDI & FSC", placeHolder: "FSC/XX/YYYYYY", cls: "span4", type: "popup" },
                    { name: "GenerateDate", text: "Tgl PDI & FSC", cls: "span4", type: "datepicker" },
                    {
                        text: "Dealer",
                        type: "controls",
                        items: [
                            { name: "SenderDealerCode", placeHolder: "Dealer Code", cls: "span2", type: "popup", readonly: true },
                            { name: "SenderDealerName", placeHolder: "Name", cls: "span6", readonly: true }
                        ]
                    },
                    { name: "RefferenceNo", text: "No. Kwitansi", cls: "span4" },
                    { name: "RefferenceDate", text: "Tgl. Kwitansi", cls: "span4", type: "datepicker" },
                    { name: "FPJNo", text: "No. Faktur Pajak", cls: "span4" },
                    { name: "FPJDate", text: "Tgl. Faktur Pajak", cls: "span4", type: "datepicker" },
                    { name: "FPJGovNo", text: "No. Seri Pajak", cls: "span4" },
                    { name: "IsCampaign", text: "Campaign", cls: "span4", type: "switch" }
                ]
            },
            {
                title: "Informasi Jumlah Rupiah",
                xtype: "table",
                pnlname: "pnlInfoRp",
                tblname: "tblInfoRp",
                columns: [
                    { name: "RecNo", text: "No", width: 30 },
                    { name: "BasicModel", text: "Basic Model" },
                    { name: "PdiFscSeq", text: "FS#", width: 80 },
                    { name: "RecCount", text: "Jumlah", cls: "right", width: 80 },
                    { name: "PdiFscAmount", text: "Total", cls: "right", width: 90 },
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "tabInfoPDI", text: "Informasi Data PDI FSC", cls: "active" },
                    { name: "tabUpload", text: "Upload PDI FSC" },
                ]
            },
            {
                title: "PDI FSC Details",
                name: "pnlInfoPDI",
                xtype: "table",
                pnlname: "pnlInfoPDIEdit",
                tblname: "tblInfoPDI",
                cls: "tabpage1 tabInfoPDI",
                buttons: [{ name: "btnAddDtl", text: "Add New PDI FSC", icon: "icon-plus" }],
                items: [
                    { name: "GenerateSeq", cls: "hide" },
                    { name: "BasicModel", text: "Basic Model", cls: "span3", type: "popup" },
                    { name: "TransmissionType", text: "Trans", cls: "span3", type: "select" },
                    { name: "ServiceBookNo", text: "No. Buku Service", cls: "span3" },
                    { name: "PdiFscSeq", text: "FS#", cls: "span3 number" },
                    { name: "Odometer", text: "Odometer", cls: "span3 number" },
                    { name: "ServiceDate", text: "Tgl. Service", type: "datepicker", cls: "span3" },
                    { name: "RegisteredDate", text: "Tgl. Registrasi", type: "datepicker", cls: "span3" },
                    { name: "DeliveryDate", text: "Tgl. Delivery", type: "datepicker", cls: "span3" },
                    { name: "ChassisCode", text: "Kd. Rangka", cls: "span3" },
                    { name: "ChassisNo", text: "No. Rangka", cls: "span3 number" },
                    { name: "EngineCode", text: "Kd. Mesin", cls: "span3" },
                    { name: "EngineNo", text: "No. Mesin", cls: "span3 number" },
                    {
                        type: "buttons", items: [
                            { name: "btnSaveDtl", text: "Save", icon: "icon-save" },
                            { name: "btnCancelDtl", text: "Cancel", icon: "icon-undo" }
                        ]
                    },
                ],
                columns: [
                    { text: "Action", type: "action", width: 80 },
                    { name: "GenerateSeq", text: "No", width: 30 },
                    { name: "BasicModel", text: "Basic Model", width: 90 },
                    { name: "TransmissionType", text: "Trans.", width: 50 },
                    { name: "ServiceBookNo", text: "No. Buku Service", width: 100 },
                    { name: "PdiFscSeq", text: "FS#", width: 40 },
                    { name: "Odometer", text: "Odometer", width: 70 },
                    { name: "ServiceDate", text: "Tgl. Service", type: "date", width: 90 },
                    { name: "RegisteredDate", text: "Tgl. Registrasi", type: "date", width: 90 },
                    { name: "DeliveryDate", text: "Tgl. Delivery", type: "date", width: 90 },
                    { name: "ChassisCode", text: "Kd. Rangka", width: 70 },
                    { name: "ChassisNo", text: "No. Rangka", width: 70 },
                    { name: "EngineCode", text: "Kd. Mesin", width: 70 },
                    { name: "EngineNo", text: "No. Mesin", width: 70 },
                    { name: "LaborAmount", text: "Nilai Jasa", cls: "right", width: 100 },
                    { name: "MaterialAmount", text: "Nilai Material", cls: "right", width: 100 },
                    { name: "PdiFscAmount", text: "Nilai Total", cls: "right", width: 100 },
                ]
            },
            {
                cls: "tabpage1 tabUpload",
                title: "File Information",
                name: "pnlFile",
                items: [
                    { name: "FileName", text: "Excel File", readonly: true, type: "upload", url: "sv.api/inputksg/UploadFile", icon: "icon-folder-open", callback: "uploadCallback" },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnUpload", text: "Upload", icon: "icon-upload" },
                            { name: "btnClearFile", text: "Clear", icon: "icon-refresh" }
                        ]
                    }
                ]
            },
            {
                name: "pnlUpload",
                xtype: "table",
                tblname: "tblUpload",
                cls: "tabpage1 tabUpload",
                columns: [
                    { name: "GenerateSeq", text: "No", width: 30 },
                    { name: "BasicModel", text: "Basic Model", width: 90 },
                    { name: "TransmissionType", text: "Trans.", width: 50 },
                    { name: "ServiceBookNo", text: "No. Buku Service", width: 100 },
                    { name: "PdiFscSeq", text: "FS#", width: 40 },
                    { name: "Odometer", text: "Odometer", width: 70 },
                    { name: "ServiceDate", text: "Tgl. Service", type: "date", width: 90 },
                    { name: "RegisteredDate", text: "Tgl. Registrasi", type: "date", width: 90 },
                    { name: "DeliveryDate", text: "Tgl. Delivery", type: "date", width: 90 },
                    { name: "ChassisCode", text: "Kd. Rangka", width: 70 },
                    { name: "ChassisNo", text: "No. Rangka", width: 70 },
                    { name: "EngineCode", text: "Kd. Mesin", width: 70 },
                    { name: "EngineNo", text: "No. Mesin", width: 70 }
                ]
            }
        ]
    }
    widget = new SimDms.Widget(options);
    widget.default = {};
    widget.JobOrderNo = "";
    widget.render(function () {
        alterUI("N");
    });
    widget.onTableClick(function (icon, row, selector) {
        switch (selector.selector) {
            case "#tblInfoPDI":
                switch (icon) {
                    case "edit":
                        editDetail(row);
                        break;
                    case "trash":
                        deleteDetail(row);
                        break;
                    default:
                        break;
                }
                break;
            default: break;
        }
    });
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "PdiFscList":
                sourceData = data.SourceData;
                getPdiFsc(data);
                break;
            case "BasicModelList":
                $('#BasicModel').val(data.RefferenceCode);
                break;
            default:
                break;
        }
    });
    $('#btnCreate').on('click', function () { alterUI('N'); });
    $('#btnAddDtl').on('click', addDetail);
    $('#btnSaveDtl').on('click', saveDetail);
    $('#btnCancelDtl').on('click', cancelDetail);
    $('#btnBrowse, #btnGenerateNo').on('click', browseData);
    $('#btnSenderDealerCode').on('click', browseSenderDealer);
    $('#btnUpload').on('click', uploadData);
    $('#btnClearFile').on('click', clearFile);
    $('#btnBasicModel').on('click', browseBasicModel);
    $('#btnSave').on('click', saveHeader);

    function browseData() {
        widget.lookup.init({
            name: "PdiFscList",
            title: "Pdi Fsc Lookup",
            source: "sv.api/grid/pdifscs?source=1",
            sortings: [[0, "desc"]],
            columns: [
                { mData: "GenerateNo", sTitle: "No. PDI FSC", sWidth: "100px" },
                {
                    mData: "GenerateDate", sTitle: "Tgl. PDI FSC", sWidth: "100px",
                    mRender: function (data, type, full) {
                        if (data != null) return moment(data).format('DD MMM YYYY - HH:mm');
                        else return "";
                    }
                },
                { mData: "FPJNo", sTitle: "No. Faktur Pajak" },
                {
                    mData: "FPJDate", sTitle: "Tgl. Faktur Pajak", sWidth: "100px",
                    mRender: function (data, type, full) {
                        if (data != null) return moment(data).format('DD MMM YYYY - HH:mm');
                        else return "";
                    }
                },
                { mData: "FPJGovNo", sTitle: "Seri Pajak", sWidth: "100px" },
                { mData: "SourceDataDesc", sTitle: "Sumber Data", sWidth: "160px" },
                { mData: "TotalNoOfItem", sTitle: "Total Record", sWidth: "100px" },
                { mData: "TotalAmt", sTitle: "Total PDI FSC", sWidth: "100px" },
                { mData: "SenderDealerName", sTitle: "Pengirim", sWidth: "100px" },
                { mData: "RefferenceNo", sTitle: "No. Referensi", sWidth: "100px" },
                {
                    mData: "RefferenceDate", sTitle: "Tgl. Referensi", sWidth: "100px",
                    mRender: function (data, type, full) {
                        if (data != null) return moment(data).format('DD MMM YYYY - HH:mm');
                        else return "";
                    }
                },
                { mData: "PostingFlagDesc", sTitle: "Status", sWidth: "100px" },
            ]
        });
        widget.lookup.show();
    }
    function getPdiFsc(data) {
        data.showAjax = false;
        widget.showAjaxLoad();
        widget.post("sv.api/inputksg/get", data, function (result) {
            if (result.success) {
                populateData(result);
            }
            else {
                widget.hideAjaxLoad();
            }
        });
    }
    function browseSenderDealer() {
        var lookup = widget.klookup({
            name: "SenderDealerList",
            title: "Dealer Lookup",
            url: "sv.api/grid/senderdealers",
            serverBinding: true,
            pageSize: 12,
            sort: [
                { field: 'CustomerName', dir: 'asc' },
                { field: 'Status', dir: 'asc' }
            ],
            columns: [
                { field: "CustomerCode", title: "Kode Dealer", width: 160 },
                { field: "CustomerName", title: "Nama Dealer", width: 200 },
                { field: "CustomerAbbrName", title: "Abbreviation Name", width: 200 },
                { field: "Status", title: "Status", width: 130 },
            ],
        });
        lookup.dblClick(function (data) {
            $('#SenderDealerCode').val(data.CustomerCode);
            $('#SenderDealerName').val(data.CustomerName);
        });
    }
    function browseBasicModel() {
        widget.lookup.init({
            name: "BasicModelList",
            title: "Master Model",
            source: "sv.api/grid/basicmodel",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "RefferenceCode", sTitle: "Basic Model", sWidth: "100px" },
                { mData: "DescriptionEng", sTitle: "Technical Model", sWidth: "120px" },
                { mData: "Description", sTitle: "Description", sWidth: "200px" },
                { mData: "IsActive", sTitle: "Status", sWidth: "80px" }
            ]
        });
        widget.lookup.show();
    }
    function saveHeader() {
        var data = $(".main .gl-widget").serializeObject();
        var msg = "";
        if (data.FPJGovNo.trim() == "") msg = "No.Seri Pajak tidak boleh kosong !";
        if (data.FPJNo.trim() == "") msg = "No. Faktur Pajak tidak boleh kosong !";
        if (data.RefferenceNo.trim() == "") msg = "No. Kwitansi tidak boleh kosong !";
        if (data.SenderDealerCode.trim() == "") msg = "Dealer tidak boleh kosong !";
        if (msg != "") {
            alert(msg);
            return;
        }

        widget.post("sv.api/inputksg/saveheader", data, function (result) {
            if (result.Message != "") {
                alert(result.Message);
                return;
            }
            alert("Data berhasil disimpan.");
        });
    }
    function uploadData() {
        if (fileData == undefined) {
            alert("Please open an excel (.XLS or .XLSX) file first.")
            return;
        }
        if (!confirm("Are you sure to upload this data ? This will overwrite any existing data !")) return;

        var data = [];
        for (var i = 0; i < fileData.length; i++) {
            var row = {
                "GenerateSeq": fileData[i].GenerateSeq.toString(),
                "BasicModel": fileData[i].BasicModel,
                "TransmissionType": fileData[i].TransmissionType,
                "ServiceBookNo": fileData[i].ServiceBookNo,
                "PdiFscSeq": fileData[i].PdiFscSeq.toString(),
                "Odometer": fileData[i].Odometer.toString(),
                "ServiceDate": new Date(parseInt(fileData[i].ServiceDate.substr(6, 13))),
                "RegisteredDate": new Date(parseInt(fileData[i].RegisteredDate.substr(6, 13))),
                "DeliveryDate": new Date(parseInt(fileData[i].DeliveryDate.substr(6, 13))),
                "ChassisCode": fileData[i].ChassisCode,
                "ChassisNo": fileData[i].ChassisNo.toString(),
                "EngineCode": fileData[i].EngineCode,
                "EngineNo": fileData[i].EngineNo.toString()
            };
            data.push(row);
        }
        
        var wrapper = {
            Data: data,
            GenerateNo: $('#GenerateNo').val(),
            IsCampaign: $('[name=IsCampaign]').attr('value')
        }
        $.ajax({
            type: 'POST',
            data: JSON.stringify(wrapper),
            url: 'sv.api/inputksg/saveupload',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                if (result.Message != "") {
                    alert(result.Message);
                    return;
                }
                alert("Upload file success");
                $('[data-name=tabInfoPDI]').click();
                getPdiFsc({ GenerateNo: $('#GenerateNo').val(), SourceData: sourceData });
                //clearFile();
            }
        });
    }
    function populateData(result) {
        var data = result.branchInfo[0] || {};
        var rpInfo = result.rpInfo || {};
        var pdiInfo = result.pdiInfo || {};

        var header = {
            GenerateNo: data.GenerateNo,
            GenerateDate: data.GenerateDate,
            SenderDealerCode: data.SenderDealerCode,
            SenderDealerName: data.SenderDealerName,
            RefferenceNo: data.RefferenceNo,
            RefferenceDate: data.RefferenceDate,
            FPJNo: data.FPJNo,
            FPJDate: data.FPJDate,
            FPJGovNo: data.FPJGovNo,
            IsCampaign: data.IsCampaign
        }

        widget.populate(header);
        widget.populateTable({ selector: "#tblInfoRp", data: rpInfo });
        widget.populateTable({ selector: "#tblInfoPDI", data: pdiInfo });
        alterUI('O');
        widget.hideAjaxLoad();
    }
    function addDetail() {
        $("#pnlInfoPDIEdit").slideDown();
        $("#btnAddDtl").parent().hide();
        $("#tblInfoPDI td .icon").removeClass("link");
    }
    function cancelDetail() {
        clearPanelDetail();
        $("#pnlInfoPDIEdit").slideUp();
        $("#tblInfoPDI td .icon").addClass("link");
        $("#btnAddDtl").parent().show();
    }
    function editDetail(row) {
        addDetail();
        var data = {
            GenerateSeq: row[1],
            BasicModel: row[2],
            TransmissionType: row[3],
            ServiceBookNo: row[4],
            PdiFscSeq: row[5].trim() == "" ? "0" : row[5],
            Odometer: row[6],
            ServiceDate: row[7],
            RegisteredDate: row[8],
            DeliveryDate: row[9],
            ChassisCode: row[10],
            ChassisNo: row[11],
            EngineCode: row[12],
            EngineNo: row[13]
        }
        widget.populate(data, "#pnlInfoPDIEdit");

        $('[name=ServiceDate]').val(data.ServiceDate);
        $('[name=RegisteredDate]').val(data.RegisteredDate);
        $('[name=DeliveryDate]').val(data.DeliveryDate);
    }
    function deleteDetail(row) {
        if (!confirm("Really delete this detail data ?")) return;
        var data = {
            GenerateNo: $('#GenerateNo').val(),
            GenerateSeq: row[1]
        }
        widget.post("sv.api/inputksg/deletedetail", data, function (result) {
            if (result.Message != "") {
                alert(result.Message);
                return;
            }

            var params = {
                GenerateNo: data.GenerateNo,
                SourceData: sourceData
            }
            getPdiFsc(params);
        });
    }
    function saveDetail() {
        var data = wrapDetail();
        var msg = "";
        if (data.Odometer <= 0) msg = "Odometer harus lebih besar dari 0 !";
        if (data.PdiFscSeq == "") msg = "FS# tidak boleh kosong !";
        if (data.ServiceBookNo == "") msg = "No Buku Service tidak boleh kosong !";
        if (data.TransmissionType == "") msg = "Transmission Type tidak boleh kosong !";
        if (data.BasicModel == "") msg = "Basic Model tidak boleh kosong !";
        if (msg != "") {
            alert(msg);
            return;
        }

        widget.post("sv.api/inputksg/savedetailvalidation", data, function (result1) {
            if (!result1.Success) {
                alert(result1.Message);
                return;
            }
            else if (result1.Success && result1.Message != "") {
                if (!confirm(result1.Message)) return;
            }
            widget.post("sv.api/inputksg/savedetail", data, function (result2) {
                if (result2.Message != "") {
                    alert(result2.Message);
                    return;
                }
                var params = {
                    GenerateNo: data.GenerateNo,
                    SourceData: sourceData
                }
                cancelDetail();
                getPdiFsc(params);
            });
        });
    }
    function wrapDetail() {
        var data = {
            GenerateNo: $('#GenerateNo').val(),
            IsCampaign: $('[name=IsCampaign]').val(),
            GenerateSeq: $('#GenerateSeq').val(),
            BasicModel: $('#BasicModel').val(),
            TransmissionType: $('#TransmissionType').val(),
            ServiceBookNo: $('#ServiceBookNo').val(),
            PdiFscSeq: $('#PdiFscSeq').val().trim() == "" ? "0" : $('#PdiFscSeq').val(),
            Odometer: $('#Odometer').val(),
            ServiceDate: $('[name=ServiceDate]').val(),
            RegisteredDate: $('[name=RegisteredDate]').val(),
            DeliveryDate: $('[name=DeliveryDate]').val(),
            ChassisCode: $('#ChassisCode').val(),
            ChassisNo: $('#ChassisNo').val(),
            EngineCode: $('#EngineCode').val(),
            EngineNo: $('#EngineNo').val()
        }
        return data;
    }
    function clearFile() {
        fileData = undefined;
        sourceData = undefined;
        widget.populateTable({ selector: "#tblUpload", data: {} });
        $('[name=FileNameShowed]').val('');
    }
    function clearPanelDetail() {
        $('#BasicModel').val("");
        $('#TransmissionType').val("");
        $('#ServiceBookNo').val("");
        $('#PdiFscSeq').val("");
        $('#Odometer').val("");
        $('[name=ServiceDate]').val(moment().format("DD-MMM-YYYY"));
        $('[name=RegisteredDate]').val(moment().format("DD-MMM-YYYY"));
        $('[name=DeliveryDate]').val(moment().format("DD-MMM-YYYY"));
        $('#ChassisCode').val("");
        $('#ChassisNo').val("");
        $('#EngineCode').val("");
        $('#EngineNo').val("");
        $('#GenerateSeq').val("");
    }
    function alterUI(status) {
        if (status == 'N') {
            widget.clearForm();
            widget.post("sv.api/inputksg/default", function (result) {
                widget.default = $.extend({}, result);
                widget.populate(widget.default);
                widget.select({ name: "TransmissionType", url: "sv.api/combo/TransmissionTypes" });
            });
            clearFile();
            clearPanelDetail();
            widget.populateTable({ selector: "#tblInfoPDI", data: {} });
            widget.populateTable({ selector: "#tblInfoRp", data: {} });
            $('[data-name=tabInfoPDI]').click();
            $('[data-id=tabpage1]').addClass('hide');
            $('#pnlInfoPDI').hide();
        } else if (status == 'O') {
            $('[data-id=tabpage1]').removeClass('hide');
            $('#pnlInfoPDI').show();
        }
    }
});

function uploadCallback(result, obj) {
    if (result.message != "") {
        alert(result.message);
        return;
    }
    $("[name=FileNameShowed]").val(result.fileName);
    fileData = result.data;
    widget.populateTable({ selector: "#tblUpload", data: fileData });
}