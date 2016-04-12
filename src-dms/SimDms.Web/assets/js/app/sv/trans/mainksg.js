
$(document).ready(function () {
    var options = {
        title: "Maintenance KSG",
        xtype: "panels",
        toolbars: [
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnDelete", text: "Delete", icon: "icon-delete", cls: "hide" },
        ],
        panels: [
            {
                name: "pnlMaintenance",
                title: "Maintenance Data PDI/KSG",
                items: [
                    { name: "NoBatch", text: "No. Batch", placeHolder: "BAT/XX/YYYYYY", cls: "span4" },
                    { name: "BatchDate", text: "Batch Date", cls: "span4" },
                    { name: "NoReceipt", text: "No. Receipt", cls: "span4" },
                    { name: "ReceiptDate", text: "Receipt Date", cls: "span4" },
                    { name: "FPJNo", text: "No. Kwitansi", cls: "span4" },
                    { name: "FPJDate", text: "Tanggal", cls: "span4", type: "datepicker" },
                    { name: "FPJGovNo", text: "FPJ Gov No", cls: "span4" }
                ]
            },
            {
                //title: "Informasi data PDI dan FSC",
                xtype: "table",
                pnlname: "pnlInfo",
                tblname: "tblInfo",
                columns: [
                    { name: "BranchCode", text: "Branch Code", width: 30 },
                    { name: "GenerateNo", text: "Generate No" },
                    { name: "DocumentNo", text: "Document No", width: 80 },
                    { name: "ServiceBook", text: "Service Book", cls: "right", width: 80 },
                    { name: "BasicMode", text: "Basic Code", cls: "right", width: 90 },
                    { name: "ChassisCode", text: "Chassis Code", cls: "right", width: 90 },
                    { name: "ChassisNo", text: "Chassis No", cls: "right", width: 90 },
                    { name: "Odometer", text: "Odometer", cls: "right", width: 90 },
                    { name: "TransAmount", text: "Trans. Amount", cls: "right", width: 90 },
                ]
            }
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.JobOrderNo = "";
    widget.render(function () {
        clearData();
        alterUI("N");
    });
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "KSGList":
                getHeader(data);
                break;
            case "SenderDealerList":
                break;
            default:
                break;
        }
    });
    $('#btnBrowse').on('click', browseData);
    $('#btnDelete').on('click', delDetail);
    $('#btnCancelDtl').on('click', cancelDetail);
    //$('#btnBrowse, #btnGenerateNo').on('click', browseData);
    $('#btnSenderDealerCode').on('click', browseSenderDealer);

    function clearData() {
        widget.clearForm();
        widget.post("sv.api/mainksg/default", function (result) {
            widget.default = $.extend({}, result);
            widget.populate(widget.default);
        });
    }
    function browseData() {
        widget.lookup.init({
            name: "KSGList",
            title: "Inquiry data KSG",
            source: "sv.api/mainksg/mainksglookups",
            sortings: [[0, "desc"]],
            columns: [
                { mData: "BranchCode", sTitle: "Branch Code", sWidth: "100px" },
                { mData: "NoBatch", sTitle: "BatchNo", sWidth: "100px" },
                {
                    mData: "BatchDate", sTitle: "Batch Date", sWidth: "100px",
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
                { mData: "FPJGovNo", sTitle: "Seri Pajak", sWidth: "100px" }
            ]
        });
        widget.lookup.show();
    }
    function getHeader(data) {
        data.showAjax = false;
        widget.showAjaxLoad();
        widget.post("sv.api/mainksg/get", data, function (result) {
            if (result.success) {
                $('#btnDelete').show();
                populateData(result);
            }
            else {
                widget.hideAjaxLoad();
            }
        });
    }
    function browseSenderDealer() {
        widget.lookup.init({
            name: "SenderDealerList",
            title: "Dealer Lookup",
            source: "sv.api/grid/senderdealers",
            sortings: [[1, "asc"]],
            columns: [
                { mData: "CustomerCode", sTitle: "Kode Dealer", sWidth: "100px" },
                { mData: "CustomerName", sTitle: "Nama Dealer", sWidth: "240px" },
                { mData: "CustomerAbbrName", sTitle: "Abbreviation Name", sWidth: "70px" },
                { mData: "Status", sTitle: "Status", sWidth: "100px" }
            ]
        });
        widget.lookup.show();
    }
    function populateData(result) {
        var data2 = result.list || {};
        var data = result.header || {};
        //var rpInfo = result.rpInfo || {};
        //var pdiInfo = result.pdiInfo || {};

        var header = {
            BranchCode: data.BranchCode,
            NoBatch: data.NoBatch,
            BatchDate: data.BatchDate,
            NoReceipt: data.NoReceipt,
            ReceiptDate: data.ReceiptDate,
            FPJNo: data.FPJNo,
            FPJDate: data.FPJDate,
            FPJGovNo: data.FPJGovNo,
        }

        widget.populate(header);
        widget.populateTable({ selector: "#tblInfo", data: data2 });
        //widget.populateTable({ selector: "#tblInfoPDI", data: pdiInfo });
        alterUI('O');
        widget.hideAjaxLoad();
    }
    function query() {
        var data = $(".main .gl-widget").serializeObject();
        getHeader(data);
    }
    function cancelDetail() {
        $("#pnlInfoPDI").slideUp();
        $("#tblInfoPDI td .icon").addClass("link");
        $("#btnAddDtl").parent().show();
    }
    function delDetail() {
        if (!confirm("Really delete this detail data ?")) return;
        var params = {
            BatchNo: $('#NoBatch').val(),
            NoBatch: $('#NoBatch').val()
        }
        widget.post("sv.api/mainksg/deldetail", params, function (result) {
            if (result.Message != "") {
                alert(result.Message);
                return;
            }

            //var params = {
              //  BatchNo: data.BatchNo,
            //}
            getHeader(params);
        });
    }
    function alterUI(status) {
        if (status == 'N') {
            $('#btnUpload').addClass('hide');
            $('#btnOpen').addClass('hide');
            $('[data-id=tabpage1]').addClass('hide');
            $('[name=pnlInfoPDI]').addClass('hide');
        } else if (status == 'O') {
            $('#btnUpload').removeClass('hide');
            $('#btnOpen').removeClass('hide');
            $('[data-id=tabpage1]').removeClass('hide');
            $('[name=pnlInfoPDI]').removeClass('hide');
        }
    }
});