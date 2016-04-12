var widget;
var fileData;
var sourceData;
var content;
var header;
var details;

$(document).ready(function () {
    var options = {
        title: "Upload KSG from Branch/Sub-Dealer",
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
                    {
                        name: "FileName",
                        text: "File",
                        readonly: true,
                        type: "upload",
                        url: "sv.api/inputksg/UploadPDIFSC",
                        icon: "icon-folder-open",
                        callback: "uploadCallback"
                    },
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
                title: "Informasi Data PDI dan FSC",
                name: "pnlInfoPDI",
                xtype: "table",
                pnlname: "pnlInfoPDIEdit",
                tblname: "tblInfoPDI",
                cls: "tabpage1 tabInfoPDI",
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
                    { name: "EngineNo", text: "No. Mesin", width: 70 },
                    { name: "LaborAmount", text: "Nilai Jasa", cls: "right", width: 100 },
                    { name: "MaterialAmount", text: "Nilai Material", cls: "right", width: 100 },
                ]
            },
        ]
    }
    widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        alterUI("N");
    });
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "PdiFscList":
                sourceData = data.SourceData;
                getPdiFsc(data);
                break;
            default:
                break;
        }
    });
    $('#btnCreate').on('click', function () { alterUI('N'); });
    $('#btnBrowse, #btnGenerateNo').on('click', browseData);
    $('#btnSenderDealerCode').on('click', browseSenderDealer);
    $('#btnUpload').on('click', uploadData);
    $('#btnClearFile').on('click', clearFile);
    $('#btnSave').on('click', saveFile);
    function browseData() {
        widget.lookup.init({
            name: "PdiFscList",
            title: "Pdi Fsc Lookup",
            source: "sv.api/grid/PdiFscAll",
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
    // untouched
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
    // untouched
    function populateData(result) {
        var data = result.branchInfo[0] || {};
        var rpInfo = result.rpInfo || {};
        var pdiInfo = result.pdiInfo || {};

        var hdr = {
            GenerateNo: data.GenerateNo,
            GenerateDate: data.GenerateDate,
            SenderDealerCode: data.SenderDealerCode,
            SenderDealerName: data.SenderDealerName,
            RefferenceNo: data.RefferenceNo,
            RefferenceDate: data.RefferenceDate
        }

        widget.populate(hdr);
        widget.populateTable({ selector: "#tblInfoRp", data: rpInfo });
        widget.populateTable({ selector: "#tblInfoPDI", data: pdiInfo });
        alterUI('O');
        widget.hideAjaxLoad();
    }
    function uploadData() {
        if (content == undefined) {
            alert("Please choose a file to upload first");
            return;
        }
        var data = {
            content: content,
            senderDealerCode: $('#SenderDealerCode').val(),
            refNo: $('#RefferenceNo').val(),
            refDate: $('[name=RefferenceDate]').val()
        }
        widget.post("sv.api/inputksg/UploadPDIFSCCallback", data, function (result) {
            if (result.Message != "") {
                alert(result.Message);
                clearFile();
                return;
            }
            var fileData = result.details;
            var costs = result.costs;
            if (!confirm("Are you sure to upload this data ? This will overwrite any existing data !")) return;
            var list = [];
            for (var i = 0; i < fileData.length; i++) {
                var row = {
                    "GenerateSeq": i + 1,
                    "BasicModel": fileData[i].BasicModel,
                    "TransmissionType": fileData[i].TransmissionType,
                    "ServiceBookNo": fileData[i].ServiceBookNo,
                    "PdiFscSeq": fileData[i].FS.toString(),
                    "Odometer": fileData[i].Odometer.toString(),
                    "ServiceDate": new Date(parseInt(fileData[i].ServiceDate.substr(6, 13))),
                    "RegisteredDate": new Date(parseInt(fileData[i].RegisteredDate.substr(6, 13))),
                    "DeliveryDate": new Date(parseInt(fileData[i].DeliveryDate.substr(6, 13))),
                    "ChassisCode": fileData[i].ChassisCode,
                    "ChassisNo": fileData[i].ChassisNo.toString(),
                    "EngineCode": fileData[i].EngineCode,
                    "EngineNo": fileData[i].EngineNo.toString(),
                    "LaborAmount": fileData[i].LaborAmount.toString(),
                    "MaterialAmount": fileData[i].MaterialAmount.toString()
                };
                list.push(row);
            }
            var costList = [];
            for (var i = 0; i < costs.length; i++) {
                var row = {
                    "RecNo": costs[i].GenerateSeq,
                    "BasicModel": costs[i].BasicModel,
                    "PdiFscSeq": costs[i].PdiFscSeq,
                    "RecCount": costs[i].Count,
                    "PdiFscAmount": costs[i].Total
                }
                costList.push(row);
            }
            widget.populateTable({ selector: "#tblInfoPDI", data: list });
            widget.populateTable({ selector: "#tblInfoRp", data: costList });
            $('#pnlInfoPDI').show();

            header = result.header;
            details = list;
            alterUI('U');
        });
    }
    function clearFile() {
        fileData = undefined;
        sourceData = undefined;
        content = undefined;
        header = undefined;
        details = undefined;
        $('[name=FileNameShowed]').val('');
        alterUI('C');
    }
    function saveFile() {
        var senderDealer = $('#SenderDealerCode').val();
        var refNo = $('#RefferenceNo').val();
        var filePath = $('#FileName').val();
        
        if (content == undefined || header == undefined) {
            alert("File harus diupload dahulu");
        }
        var data = {
            genDate: $('[name=GenerateDate]').val(),
            dealerCode: senderDealer,
            refNo: refNo
        }
        $.ajax({
            type: 'POST',
            data: data,
            url: 'sv.api/inputksg/UploadPDIFSCSaveValidation',
            success: function (result) {
                if (result.Overwrite) {
                    if (!confirm(result.Message)) return;
                }
                else {
                    if (result.Message != "") {
                        alert(result.Message);
                        return;
                    }
                }
                var hdr = {
                    DealerCode: header.DealerCode,
                    RcvDealerCode: header.RcvDealerCode,
                    DealerName: header.DealerName,
                    TotalItem: header.TotalItem,
                    ReceiptNo: header.ReceiptNo,
                    ReceiptDate: new Date(parseInt(header.ReceiptDate.substr(6, 13))),
                    IsCampaign: header.IsCampaign,
                    PaymentNumber: header.PaymentNumber,
                    PaymentDate: new Date(parseInt(header.PaymentDate.substr(6, 13)))
                }
                var data2 = {
                    genDate: $('[name=GenerateDate]').val(),
                    header: hdr,
                    details: details
                }
                $.ajax({
                    type: 'POST',
                    data: JSON.stringify(data2),
                    contentType: 'application/json;charset=utf8',
                    dataType: 'JSON',
                    url: 'sv.api/inputksg/UploadPDIFSCSaveData',
                    success: function (result2) {
                        if (result2.Message != "") {
                            alert(result2.Message);
                            return;
                        }
                        $('#GenerateNo').val(result2.GenerateNo);
                    }
                });
            }
        });

    }
    function alterUI(status) {
        if (status == 'N') {
            widget.clearForm();
            widget.post("sv.api/inputksg/default", function (result) {
                widget.default = $.extend({}, result);
                widget.populate(widget.default);
            });
            clearFile();
            widget.populateTable({ selector: "#tblInfoPDI", data: {} });
            widget.populateTable({ selector: "#tblInfoRp", data: {} });
        } else if (status == 'O') {
            $('#GenerateNo').attr('disabled', 'disabled');
            $('[name=GenerateDate]').attr('disabled', 'disabled');
            $('#RefferenceNo').attr('disabled', 'disabled');
            $('[name=RefferenceDate]').attr('disabled', 'disabled');
            $('#btnSenderDealerCode').attr('disabled', 'disabled');
            $('#btnFileName').attr('disabled', 'disabled');
            $('#btnUpload').attr('disabled', 'disabled');
            $('#btnClearFile').attr('disabled', 'disabled');
            $('#pnlInfoPDI').show();
        } else if (status == 'U') { //after clicking Upload
            $('#GenerateNo').attr('disabled', 'disabled');
            $('[name=GenerateDate]').attr('disabled', 'disabled');
            $('#RefferenceNo').attr('disabled', 'disabled');
            $('[name=RefferenceDate]').attr('disabled', 'disabled');
            $('#btnSenderDealerCode').attr('disabled', 'disabled');
            $('#btnFileName').removeAttr('disabled');
            $('#btnUpload').removeAttr('disabled');
            $('#btnClearFile').removeAttr('disabled');
            $('#pnlInfoPDI').show();
        } else if (status == 'C') { // after clicking Clear Upload
            $('#GenerateNo').attr('disabled', 'disabled');
            $('[name=GenerateDate]').removeAttr('disabled');
            $('#RefferenceNo').removeAttr('disabled');
            $('[name=RefferenceDate]').removeAttr('disabled');
            $('#btnSenderDealerCode').removeAttr('disabled');
            $('#btnFileName').removeAttr('disabled');
            $('#btnUpload').removeAttr('disabled');
            $('#btnClearFile').removeAttr('disabled');
            $('#pnlInfoPDI').hide();
        }
    }
});

function uploadCallback(result) {
    if (result.Message != "") {
        alert(result.Message);
        clearFile();
        return;
    }
    $('[name=FileNameShowed]').val(result.FileName);
    content = result.Content;
}

