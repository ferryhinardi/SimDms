
$(document).ready(function () {
    var options = {
        title: "Generate KSG",
        xtype: "panels",
        toolbars: [
            { name: "btnQuery", text: "Query", icon: "icon-search" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnGenerate", text: "Generate File (WFREE)", icon: "icon-folder-open", cls: "hide" },
        ],
        panels: [
            {
                name: "pnlGenerate",
                title: "Generate PDI dan FSC",
                items: [
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", placeHolder: "Company Code", cls: "span2", readonly: true },
                            { name: "CompanyName", placeHolder: "Name", cls: "span6", readonly: true }
                        ]
                    },
                    { name: "NoBatch", text: "No. Batch", placeHolder: "BAT/XX/YYYYYY", cls: "span6", required: true },
                    { name: "ReceiptNo", text: "No. Kwitansi", cls: "span4", required: true },
                    { name: "ReceiptDate", text: "Tanggal", cls: "span4", type: "datepicker" },
                    { name: "FPJNo", text: "No. Faktur Pajak", cls: "span4" },
                    { name: "FPJDate", text: "Tanggal", cls: "span4", type: "datepicker" },
                    { name: "FPJGovNo", text: "No. Seri Pajak", cls: "span4" }
        ]
            },
            {
                title: "Informasi data PDI dan FSC",
                xtype: "table",
                pnlname: "pnlInfo",
                tblname: "tblInfo",
                columns: [
                    { name: "BranchCode", text: "No", width: 30 },
                    { name: "GenerateNo", text: "No. PDI-FSC" },
                    { name: "GenerateDate", text: "Tgl. PDI-FSC", width: 80, type: "date" },
                    { name: "SenderDealerCode", text: "Dealer Code", cls: "right", width: 80 },
                    { name: "SenderDealerName", text: "Dealer Name", cls: "right", width: 90 },
                    { name: "TotalNoOfItem", text: "Jml. Kupon", cls: "right", width: 90 },
                    { name: "TotalLaborAmt", text: "Nilai Jasa", cls: "right", width: 90 },
                    { name: "TotalMaterialAmt", text: "Nilai Material", cls: "right", width: 90 },
                    { name: "TotalAmt", text: "Total", cls: "right", width: 90 },
                ]
            }
        ]
    }
    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.JobOrderNo = "";
    widget.render(make_klooup);
    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "PdiFscList":
                getHeader(data);
                break;
            default:
                break;
        }
    });
    $('#btnQuery').on('click', query);
    $('#btnSaveDtl').on('click', saveDetail);
    $('#btnCancelDtl').on('click', cancelDetail);
    //$('#btnGenerateNo').on('click', browseData);
    $('#btnSenderDealerCode').on('click', browseSenderDealer);

    function make_klooup() {
        widget.klookup({
            name: "btnBrowse",
            title: "Pdi Fsc Lookup",
            url: "sv.api/lookup/SqlBatchLookUp",
            serverBinding: true,
            sort: ({ field: "NoBatch", dir: "asc" }),
            filters: [
                {
                    text: "Filter",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "NoBatchlk", text: "No Batch", cls: "span4" },
                        { name: "NoKwitansilk", text: "No Kwitansi", cls: "span4" },
                        { name: "NoFakturPajaklk", text: "No Faktur Pajak", cls: "span4" },
                        { name: "SeriPajak", text: "Seri Pajak", cls: "span4" },
                        { name: "TglBatchlk", text: "Tgl Batch From", cls: "span4", type:"datepicker" },
                        { name: "sdTglBatchlk", text: "Tgl Batch To", cls: "span4" },
                        { name: "TglKwitansilk", text: "Tgl Kwitansi From", cls: "span4" },
                        { name: "sdTglKwitansilk", text: "Tgl Kwitansi To", cls: "span4" },
                        { name: "TglFakturPajaklk", text: "Tgl Faktur Pajak From", cls: "span4" },
                        { name: "sdTglFakturPajaklk", text: "Tgl Faktur Pajak To", cls: "span4" },
                        { name: "NoBatchlka", text: "No Batch", cls: "hide" }
    //{ name: "fltVinNo", text: "Vin No", cls: "span2" },
                        //{ name: "fltPolReg", text: "Police No", cls: "span2" },
                    ]
                }
            ],
            columns: [
                //{ mData: "GenerateNo", sTitle: "No. PDI FSC", sWidth: "100px" },
                //{
                //    mData: "GenerateDate", sTitle: "Tgl. PDI FSC", sWidth: "100px",
                //    mRender: function (data, type, full) {
                //        return moment(data).format('DD MMM YYYY - HH:mm');
                //    }
                //},
                //{ mData: "FPJNo", sTitle: "No. Faktur Pajak" },
                //{
                //    mData: "FPJDate", sTitle: "Tgl. Faktur Pajak", sWidth: "100px",
                //    mRender: function (data, type, full) {
                //        return moment(data).format('DD MMM YYYY - HH:mm');
                //    }
                //},
                    { field: "BatchNo", title: "No. Batch", width: "100px" },
                    {
                        field: "BatchDate", title: "Batch Date", width: "100px", template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                    },
                    { field: "ReceiptNo", title: "No. Kwitansi", width: "100px" },
                    { field: "ReceiptDate", title: "Tanggal", width: "100px" },
                    { field: "FPJNo", title: "No. Faktur Pajak", width: "100px" },
                    { field: "FPJDate", title: "Tanggal", width: "100px" },
                    { field: "FPJGovNo", title: "No. Seri Pajak", width: "100px" },
                    { field: "IsCampaign", title: "Campaign", width: "100px" },
                    { field: "ProcessSeq", title: "Counter Process", width: "100px" },
                    { field: "ItemTotal", title: "Jml. Item", width: "100px" },
                    { field: "ItemTotalAmt", title: "Tot. Nilai", width: "100px" }
            ],
            onSelected: function (data) {
                $('#NoBatch').val(data.BatchNo);
                $('#ReceiptNo').val(data.ReceiptNo);
                $('#ReceiptDate').val(data.ReceiptDate);
                $('#FPJNo').val(data.FPJNo);
                $('#FPJDate').val(data.FPJDate);
                $('#FPJGovNo').val(data.FPJGovNo);

                //refreshData(params);
            }
        });

        clearData();
        alterUI("N");
    }

    function clearData() {
        widget.clearForm();
        widget.post("sv.api/genksg/default", function (result) {
            widget.default = $.extend({}, result);
            widget.populate(widget.default);
        });
    }
    function browseData() {
        lookup.dblClick(getHeader);
    }
    function getHeader(data) {
        data.showAjax = false;
        widget.showAjaxLoad();
        widget.post("sv.api/genksg/get", data, function (result) {
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
        });
    }
    function populateData(result) {
        var data = result.branchInfo || {};
        //var rpInfo = result.rpInfo || {};
        //var pdiInfo = result.pdiInfo || {};

        //widget.populate(header);
        widget.populateTable({ selector: "#tblInfo", data: data });
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
    function saveDetail() {

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