$(document).ready(function () {
    var isBrowse = false;
    var isHolding = true;

    var options = {
        title: "Get Warranty Claim (SPK/INV)",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", icon: "icon-file" },
            { name: 'btnBrowse', text: 'Browse', icon: 'icon-search' },
            { name: 'btnSave', text: 'Save', icon: 'icon-save', cls: 'hide' },
            { name: 'btnDelete', text: 'Delete', icon: 'icon-remove', cls: 'hide' },
            { name: 'btnPrint', text: 'Print', icon: 'icon-print' }
        ],
        panels: [
            {
                name: "pnlSubDealerBranch",
                title: "Sub Dealer / Branch",
                items: [
                     {
                         name: "ClaimType", text: "Claim Type", type: "select", cls: "span4 full",
                         items: [
                                { value: '0', text: 'Service Claim' },
                                { value: '1', text: 'Sparepart Claim' }
                         ]
                     },
                      { name: "GenerateNo", text: "Claim No.", placeHolder: "CLA/XX/YYYYYY", type: "popup", cls: "span4", readonly: true },
                      { name: "GenerateDate", text: "Claim Date", cls: "span4", type: "datetimepicker" },
                      { name: "BranchFrom", text: "Branch", cls: "span4", type: "popup", readonly: true },
                      { name: "BranchTo", text: "s/d", cls: "span4", type: "popup", readonly: true },
                      { name: "InvoiceFrom", text: "SPK No.", cls: "span4 hide", type: "popup", readonly: true },
                      { name: "InvoiceTo", text: "s/d", cls: "span4 hide", type: "popup", readonly: true },
                      { name: "PeriodFrom", text: "Period From", cls: "span4 hide", type: "datepicker" },
                      { name: "PeriodTo", text: "s/d", cls: "span4 hide", type: "datepicker" },
                      { name: "OtherCompensationAmt", text: "Compensation Amount", cls: "span4 number-int", required: "required" },
                      { name: "TotalNoOfItem", text: "Record Total", cls: "span4", readonly: true },
                      {
                          type: "buttons",
                          items: [
                              { name: "btnQuery", text: "QUERY", icon: "icon-search", cls: "span4" },
                          ]
                      }
                ]
            },
            {
                name: "KGridTotalAmountInfo",
                title: "Total Amount Info",
                xtype: "kgrid",
            },
            {
                name: "KGridClaimDataInfo",
                title: "Claim Data Info",
                xtype: "kgrid"
            },
            {
                name: "pnlTroubleInfo",
                title: "Trouble Information",
                items: [
                      { name: "TroubleDescription", text: "Trouble", type: "textarea", readonly: true },
                      { name: "ProblemExplanation", text: "Explain", type: "textarea", readonly: true }
                ]
            },
            {
                title: "Part Info",
                xtype: "table",
                tblname: "tblPartInfo",
                columns: [
                    { name: "SeqNo", text: "No.", width: 20 },
                    { name: "IsCausalPart", text: "Casual Part", width: 20 },
                    { name: "PartNo", text: "Part No.", width: 50 },
                    { name: "PartQty", text: "Qty", width: 50, cls: "right number", type: "numeric" },
                    { name: "PartName", text: "Part Name", width: 250 }
                ]
            }
        ]
    }

    var widget = new SimDms.Widget(options);
    widget.default = {};
    widget.render(function () {
        init();
    });

    function init() {
        $.post("sv.api/getwarrantyclaim/default", function (result) {
            if (result.RecHoding >= 1) {
                if (result.OGnMstLookUpDtl == null || (result.OGnMstLookUpDtl != null && result.OGnMstLookUpDtl.ParaValue == "1")) {
                    if (result.IsBranch) {
                        widget.alert("User bukan termasuk user Holding !");
                        $('#pnlSubDealerBranch button, #pnlSubDealerBranch input, #pnlSubDealerBranch select, .toolbar button').attr('disabled', 'disabled');
                        //$('input[name=PeriodFrom]').parents().eq(2).show();
                        //$('input[name=PeriodTo]').parents().eq(2).show();
                        $('#InvoiceFrom').parents().eq(2).show();
                        $('#InvoiceTo').parents().eq(2).show();
                    }
                    else {
                        widget.populate(result);

                        $('#BranchFrom').val("[All Branch]");
                        $('#BranchTo').val("[All Branch]");
                        $('#btnBranchFrom, #btnBranchTo').attr('disabled', 'disabled')

                        $('#InvoiceFrom').val((result.ClaimMode == "SPK") ? "[All SPK]" : "[All Invoice]");
                        $('#InvoiceTo').val((result.ClaimMode == "SPK") ? "[All SPK]" : "[All Invoice]");

                        isHolding = true;
                        $('input[name=PeriodFrom]').parents().eq(2).show();
                        $('input[name=PeriodTo]').parents().eq(2).show();
                    }
                }
                else{
                    widget.default = result;
                    widget.populate(result);

                    isHolding = false;
                    $('#InvoiceFrom').parents().eq(2).show();
                    $('#InvoiceTo').parents().eq(2).show();

                    $('#btnBranchFrom, #btnBranchTo, #btnQuery').attr('disabled', 'disabled')
                    $('#btnInvoiceFrom, #btnInvoiceFrom').removeAttr('disabled')
                    $('#btnSave, #btnDelete').addClass('hide', 'hide');
                }

                console.log(result.RecHoding);
            }
            else{
                widget.alert("tidak ada data holding yang di setup, tolong disetup dahulu!");
                $('#pnlSubDealerBranch button, #pnlSubDealerBranch input, #pnlSubDealerBranch select, .toolbar button').attr('disabled', 'disabled');

                $('#InvoiceFrom').parents().eq(2).show();
                $('#InvoiceTo').parents().eq(2).show();
            }
        });
    }

    $('#btnNew').on('click', NewClaim);

    $('#btnBrowse, #btnGenerateNo').on('click', LookUpClaim);

    $('#btnQuery').on('click', function (e) {
        isBrowse = false;
        if ($('#InvoiceFrom').val() == "" || $('#InvoiceTo').val() == "") {
            MsgBox('Ada informasi yang belum lengkap', MSG_WARNING);
            return;
        }
        if (isHolding==true) {
            $('#InvoiceFrom').val($('input[name=PeriodFrom]').val());
            $('#InvoiceTo').val($('input[name=PeriodTo]').val());
        }
        
        var param = $('#pnlSubDealerBranch').serializeObject();
        //console.log(param);
        $.post('sv.api/getwarrantyclaim/InquiryClaim', param, function (result) {
            if (result.ClaimInfo.length != 0) {
                widget.populate(result.ClaimInfo);
                PopulateClaimAndTotalAmountInfo(result);
                $('#btnSave').removeClass('hide');
            }
            else {
                $('#btnSave').addClass('hide', 'hide');
            }
        });
    });

    $('#btnSave').on('click', function (e) {
        if (isBrowse) 
            return;
        
        if ($('#KGridClaimDataInfo tbody > tr').length == 0) {
            MsgBox('Informasi Claim masih belum tersedia !!', MSG_WARNING);
            return;
        }

        if (isHolding) {
            SaveClaimHolding(PopulateRecord);
        }
        else {
            SaveClaim(PopulateRecord);
        }
    })

    $('#btnDelete').on('click', function (e) {
        DeleteClaim();
    });

    $('#btnPrint').on('click', PrintClaim);

    $('#btnInvoiceFrom').on('click', function (e) {
        lookupInvoice().dblClick(function (data) {
            $('#InvoiceFrom').val(data.InvoiceNo);

            $("#btnQuery").removeAttr("disabled");
            if (data.length == 1) {
                $('#InvoiceTo').val(data.InvoiceNo);
            }
            else {
                $('#InvoiceTo').val('');
            }

        });
    });

    $('#btnInvoiceTo').on('click', function (e) {
        lookupInvoice().dblClick(function (data) {
            $('#InvoiceTo').val(data.InvoiceNo);
        });
    });

    $("#KGridClaimDataInfo").on("click", "table", function (e) {
        widget.selectedRow("KGridClaimDataInfo", function (e) {
            if (e != undefined) {
                var data = {
                    branchCode: e.BranchCode,
                    invoiceNo: e.InvoiceNo,
                    causalPartNo: e.CausalPartNo,
                    TroubleDescription: e.TroubleDescription,
                    ProblemExplanation: e.ProblemExplanation
                };
                PopulatePartInfo(data);
            }
        });
    });

    function LookUpClaim() {
        isBrowse = true;
        var lookup = widget.klookup({
            name: "trnWarranty",
            title: "Transaction - Warranty Lookup",
            url: "sv.api/grid/claim?claimType=" + $('#ClaimType').val() + "&SourceData=0",
            serverBinding: true,
            pageSize: 12,
            sort: [
                { 'field': 'GenerateNo', 'dir': 'desc' },
                { 'field': 'GenerateDate', 'dir': 'desc' }
            ],
            filters: [
                {
                    text: "Status",
                    type: "controls",
                    items: [
                        {
                            name: "fltStatus", type: "select", text: "Status", cls: "span2", items: [
                                { value: "" , text: "PILIH SEMUA" },
                                { value: "0", text: "BELUM POSTING" },
                                { value: "1", text: "SUDAH POSTING" },
                                { value: "2", text: "DRAFT CLAIM", selected: 'selected' },
                                { value: "3", text: "GENERATE FILE" },
                                { value: "4", text: "RECEIVE HASIL CLAIM" },
                                { value: "5", text: "SEND HASIL CLAIM" }
                            ]
                        },
                    ]
                },
            ],
            columns: [
                { field: "GenerateNo", title: "Warranty Claim No.", width: 160 },
                {
                    field: "GenerateDate", title: "Warranty Claim Date", width: 160,
                    template: "#= (GenerateDate == undefined) ? '' : moment(GenerateDate).format('DD MMM YYYY') #"
                },
                { field: 'Invoice', title: 'Invoice No.', width: 250 },
                { field: 'FPJNo', title: 'Tax Invoice No.', width: 160 },
                {
                    field: "FPJDate", title: "Tax Invoice Date",  width: 210,
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: 'FPJGovNo', title: 'Tax Invoice GOV No.', width: 160 },
                { field: 'SourceDataDesc', title: 'Source Data', width: 160 },
                { field: 'TotalNoOfItem', title: 'Record Total', width: 160 },
                { field: 'TotalClaimAmt', title: 'Warranty Claim Total', width: 160, format: "{0:#,##0}" },
                { field: 'SenderDealerName', title: 'Sender', width: 160 },
                { field: 'RefferenceNo', title: 'Refference No.', width: 160 },
                {
                    field: "RefferenceDate", title: "Refference Date", width: 160,
                    template: "#= (RefferenceDate == undefined) ? '' : moment(RefferenceDate).format('DD MMM YYYY') #"
                },
                { field: 'PostingFlagDesc', title: 'Status', width: 160 }
            ],
        });

        lookup.dblClick(function (data) {
            $('#GenerateNo').val(data.GenerateNo);
            param = { 'GenerateNo': data.GenerateNo }
            PopulateRecord(param);
        });
    };

    function NewClaim() {
        widget.clearForm();
        init();
        PopulateClaimAndTotalAmountInfo('');
    };

    function SaveClaim(callback) {
        param = widget.serializeObject();
        $.post('sv.api/getwarrantyclaim/SaveClaim', param).
        success(function (result, status, headers, config) {
            if (result.success) {
                if (callback != undefined) {
                    var GenerateNo= result.data.GenerateNo
                    $('#GenerateNo').val(GenerateNo);
                    callback({ 'GenerateNo': GenerateNo });
                    SimDms.Success(result.message);
                }
            }
            else {
                SimDms.Error(result.message);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox("Connection to the server failed...., status " + status, MSG_ERROR);
        });
    }

    function SaveClaimHolding(callback) {
        param = widget.serializeObject();
        $.post('sv.api/getwarrantyclaim/SaveClaimHolding', param).
        success(function (result, status, headers, config) {
            if (result.success) {
                if (callback != undefined) {
                    var GenerateNo = result.data.GenerateNo
                    $('#GenerateNo').val(GenerateNo);
                    callback({ 'GenerateNo': GenerateNo });
                    SimDms.Success(result.message);
                }
            }
            else {
                SimDms.Error(result.message);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox("Connection to the server failed...., status " + status, MSG_ERROR);
        });
    }

    function DeleteClaim(callback) {
        widget.confirm("Apakah Anda Yakin???", function (con) {
            if (con == "Yes") {
                param = { GenerateNo: $('#GenerateNo').val() };
                $.post('sv.api/getwarrantyclaim/DeleteClaim', param).
                success(function (result, status, headers, config) {
                    if (result.success) {
                        NewClaim();
                        SimDms.Success(result.message);
                    }
                    else {
                        SimDms.Error(result.message);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox("Connection to the server failed...., status " + status, MSG_ERROR);
                });
            }
        });
    }

    function PrintClaim() {
        createCookie('sdc', '0', 1);
        widget.loadForm();
        widget.showForm({ url: 'sv/report/svrptrn013' });
    }

    function PopulateRecord(param) {
        $.post('sv.api/getwarrantyclaim/GetClaim', param, function (result) {
            if (result.ClaimInfo != undefined) {
                $('#btnDelete').removeClass('hide');
                $('#btnSave').addClass('hide', 'hide');
                $('#btnInvoiceFrom, #btnInvoiceFrom, #btnQuery').attr('disabled', 'disabled');
            }
            else {
                $('#btnDelete, #btnSave').addClass('hide', 'hide');
            }
            if (result.HeaderInfo[0] != undefined) {
                var dataH = {
                    BranchFrom: result.HeaderInfo[0].BranchCodeTo,
                    BranchTo: result.HeaderInfo[0].BranchCodeTo,
                    InvoiceFrom: result.HeaderInfo[0].FromInvoiceNo,
                    InvoiceTo: result.HeaderInfo[0].ToInvoiceNo,
                    OtherCompensationAmt: result.HeaderInfo[0].OtherCompensationAmt
                }
                widget.populate(dataH);
            }
            PopulateClaimAndTotalAmountInfo(result);
        });
    };

    function PopulateClaimAndTotalAmountInfo(data) {
        var x = widget.kgrid({
            data: data.ClaimInfo,
            name: "KGridClaimDataInfo",
            columns: [
                  { field: "SeqNo", title: "No", width: 80 },
                  { field: "BranchCode", title: "Branch", width: 120 },
                  { field: "InvoiceNo", title: "Invoice No.", width: 160 },
                  {
                      field: "InvoiceDate", title: "Invoice Date", type: "date",
                      template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                      , width: 160
                  },
                  { field: "CategoryCode", title: "Category", width: 100 },
                  { field: "ComplainCode", title: "CC", width: 100 },
                  { field: "DefectCode", title: "DC", width: 100 },
                  { field: "SubletHour", title: "Sublet", format: "{0:#,##0.00}", width: 100 },
                  { field: "isCbu", title: "CBU", width: 80 },
                  { field: "BasicModel", title: "Basic Model", width: 150 },
                  { field: "ServiceBookNo", title: "Service Book No.", width: 150 },
                  { field: "ChassisCode", title: "Chassis Code", width: 200 },
                  { field: "ChassisNo", title: "Chassis No.", width: 200 },
                  { field: "EngineCode", title: "Engine Code", width: 150 },
                  { field: "EngineNo", title: "Engine No", width: 150 },
                  { field: "Odometer", title: "Odometer", format: "{0:#,##0}", width: 150 },
                  { field: "OperationHour", title: "Hours" },
                  { field: "ClaimAmt", title: "Claim Amount", format: "{0:#,##0}", width: 150 },
            ],
        });

        widget.kgrid({
            data: data.CostInfo,
            name: "KGridTotalAmountInfo",
            columns: [
                 { field: "SeqNo", title: "No", width: 20 },
                 { field: "BasicModel", title: "Model", width: 50 },
                 { field: "Qty", title: "Qty", width: 50, format: "{0:#,##0}" },
                 { field: "TotalSrvAmt", title: "Total", width: 50, format: "{0:#,##0}" }
            ],
        });

        if (data == "") {
            widget.populateTable({ selector: "#tblPartInfo", data: {} });
        }
        else {
            $('#TotalNoOfItem').val(data.ClaimInfo.length);
            var datClaim = (data.ClaimInfo == undefined) ? {} : data.ClaimInfo[0];
            if (datClaim != undefined) {
                var dataClaim = {
                    branchCode: datClaim.BranchCode,
                    invoiceNo: datClaim.InvoiceNo,
                    causalPartNo: datClaim.CausalPartNo,
                    TroubleDescription: datClaim.TroubleDescription,
                    ProblemExplanation: datClaim.ProblemExplanation
                };

            }
            PopulatePartInfo(dataClaim);
        }
    }

    function PopulatePartInfo(data) {
        $('#TroubleDescription').val(data.TroubleDescription);
        $('#ProblemExplanation').val(data.ProblemExplanation);
        widget.post('sv.api/getwarrantyclaim/CasualParts', data, function (result) {
            widget.populateTable({ selector: "#tblPartInfo", data: result });
        });
    }

    function lookupInvoice() {
        var data = $(".main .gl-widget").serializeObject();
        var lookup = widget.klookup({
            name: "lookupInvoice",
            title: "List of Claim",
            url: "sv.api/grid/InquiryClaimLku?branchFrom=" + data.BranchFrom + "&branchTo=" + data.BranchTo + "&claimType=" + data.ClaimType,
            serverBinding: true,
            sort: [
                { 'field': 'InvoiceNo', 'dir': 'desc' },
                { 'field': 'InvoiceDate', 'dir': 'desc' }
            ],
            pageSize: 10,
            columns: [
                { field: "BranchCode", title: "BranchCode", width: 130 },
                { field: "InvoiceNo", title: "Document No.", width: 200 },
                {
                    field: "InvoiceDate", title: "Document Date", width: 150,
                    template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                },
                { field: "CategoryCode", title: "Category", width: 130 },
                { field: "ComplainCode", title: "CC", width: 130 },
                { field: "DefectCode", title: "DC", width: 130 },
                { field: "SubletHour", title: "Sublet Hour", width: 130 },
                { field: "SubletAmt", title: "Sublet Amount", width: 130, format: "{0:#,##0}" },
                { field: "CausalPartNo", title: "Causal Part", width: 130 },
                { field: "TroubleDescription", title: "Description", width: 450 },
            ],
        });

        return lookup;
    }
});