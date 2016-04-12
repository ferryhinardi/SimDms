$(document).ready(function () {
    var options = {
        title: "Claim Detail List",
        //xtype: "report",
        toolbars: [
            { name: "btnProcess", text: "Process", icon: "icon-bolt" }
        ],
        items: [
            { name: "SourceData", text: "Sumber Data", cls: "span4 full", type: "select", required: true, items: [{ text: "SPK", value: "0" }, { text: "Claim", value: "1" }] },
            {text: "Kode Branch", type: "controls", cls: "span4", items: [
                  { name: "IsBranch", cls: "span2", type:"switch" },
                  {
                      name: "BranchCode",
                      text: "Branch",
                      cls: "span2",
                      placeholder: "Branch Code",
                      type: "popup",
                      btnName: "btnBranchCode",
                      required: "required", readonly:true
                  },
            ]},
            { name: "BranchName", text: "Branch Name", cls: "span4", type: "text", readonly: true },
            {
                text: "Batch", type: "controls", name:"test", cls: "batch span4 full", items: [
                     { name: "IsBatch", cls: "span2", type: "switch" },
                     {
                         name: "BatchCode",
                         text: "Batch Code",
                         cls: "span4",
                         placeholder: "Branch Code",
                         type: "popup",
                         btnName: "btnBatchCode",
                         required: "required",
                         readonly:true
                     },
                ]
            },
            {
                text: "Period", type: "controls", name: "test2", cls: "period span4", items: [
                     { name: "DateFrom", cls: "span4", type: "kdatepicker" },
                     { name: "DateTo", cls: "span4", type: "kdatepicker" }
                ]
            },
        ],
    }
    var widget = new SimDms.Widget(options);
    var defaultBranch;
    widget.render(function () {
        $(".frame").css({ top: 130 });
        $(".panel").css({ 'max-width': 1300 });
        $('#PoliceNo,#BasicModel,#RangkaNo,#MesinNo,#PelangganName').attr("ReadOnly", "ReadOnly");
        $('.period,.batch').hide();
        widget.populate({ IsPeriod: false});
        $('#BranchFrom, #BranchTo').attr("readonly", "readonly");

        $.post("sv.api/report/ClaimDetailListDef", function (result) {
            defaultBranch = result;
            widget.default = result;
            widget.populate(result);

            if (result.chkClaim.toString() === "false") {
                $('#SourceData').find("option[value='1']").remove();
            }
            if (result.ChkBranch.toString() === "false") {
                widget.populate({ IsBranch: true });
                $('input[name="IsBranch"]').attr('disabled', 'disabled');
                console.log("ChkBranch"+result.ChkBranch);
            }
            if (result.isChkBranch.toString() === "true") {
                widget.populate({ isBranch: true });
            }
            else {
                widget.populate({ isBranch: true });
            }
        });

        var dateFrom = $('input[name="DateFrom"]').val();
        var dateTo = $('input[name="DateTo"]').val();
        var date1 = new Date();
        var date2 = new Date(date1.getFullYear(), date1.getMonth(), 1);
        widget.populate({ DateFrom: date1, DateTo: date1 });
        $('#btnBatchCode').attr("disabled", "disabled");

        //$('input[name="DateFrom"], input[name="DateTo"]').attr("readonly", "readonly");
    });

    $('#SourceData').on('change', function () {
        var val = $(this).val();
        if (val === "0") {
            $('.batch').hide(function ()
            { $('.period').show(); });
        } else {
            $('.period').hide(function () { $('.batch').show(); });
            
        }
        console.log(val);
    });

    $('#btnProcess').click(function () {
        var valid = $(".main form").valid();
        if (valid) {
            showReport();
        }
    });

    $('input[name="IsBranch"]').on("change", function () {
        var val = $(this).val();

        if (val.toString() == "true") {
            $('#BranchFrom, #BranchTo').attr("readonly", "readonly");
        } else {
            console.log(val);
            $('#BranchFrom, #BranchTo').removeAttr("readonly");
        }
    });

    $('input[name="IsBatch"]').on("change", function () {
        var val = $(this).val();

        if (val.toString() === "true") {
            $('#btnBatchCode').removeAttr("disabled");
        } else {
            console.log(val);
            $('#btnBatchCode').attr("disabled", "disabled");
            $('#BatchCode').val("");
        }
    });

    $("#btnBatchCode").on("click", function () {
        // loadData('browse');
        var param = { "BranchCode": $("#BranchCode").val() };
        widget.lookup.init({
            name: "Browse",
            title: "Refference Type",
            source: "sv.api/grid/WarrantyClaimLookup?BranchCode=" + $("#BranchCode").val(),
            columns: [
                { mData: "BatchNo", sTitle: "Batch No", sWidth: "110px" },
                { mData: "BatchDate", sTitle: "Batch Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                } },
                { mData: "ReceipNo", sTitle: "Receipt No", sWidth: "110px" },
                { mData: "ReceiptDate", sTitle: "Receipt Date", sWidth: "80px" },
                { mData: "FPJNo", sTitle: "FPJNo", sWidth: "80px" },
                { mData: "FPJDate", sTitle: "No Seri Pajak", sWidth: "80px" },
                { mData: "FPJGovNo", sTitle: "No Seri Pajak", sWidth: "80px" },
                { mData: "LotNo", sTitle: "Lot No", sWidth: "80px" },
                { mData: "ProcessSeq", sTitle: "Process Seq", sWidth: "80px" },
                { mData: "TotalNoOfItem", sTitle: "Total No Of Item", sWidth: "80px" },
                { mData: "TotalClaimAmt", sTitle: "Total Claim Amt", sWidth: "80px" },
                { mData: "OtherCompensationAmt", sTitle: "Compensation Amt", sWidth: "80px" }
            ]
        });
        widget.lookup.show();
    });

    widget.lookup.onDblClick(function (e, data, name) {
        console.log(data.BatchNo);
        $('#BatchCode').val(data.BatchNo);
        widget.lookup.hide();
    });

    function showReport() {
        var SourceData = $('#SourceData').val();
        var DateFrom = $('input[name="DateFrom"]').val();
        var day = DateFrom.substring(0, 2);
        var month = DateFrom.substring(3, 6);
        var year = DateFrom.substring(7, 11);

        var vDateFrom = year + '-' + month + '-' + day;

        var DateTo = $('input[name="DateTo"]').val();
        day = DateTo.substring(0, 2);
        month = DateTo.substring(3, 6);
        year = DateTo.substring(7, 11);

        var vDateTo = year + '-' + month + '-' + day;
        var BatchCode = $('#BatchCode').val()

        var rparam = DateFrom + ' s/d ' + DateTo;
        var pparam = [vDateFrom, vDateTo, BatchCode, SourceData];
        widget.showPdfReport({
            id: "SvRpReport031",
            pparam: pparam.join(','),
            rparam: rparam,
            type: "devex"
        });
    }
});