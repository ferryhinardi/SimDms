var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

$(document).ready(function () {
    var FPJDateDefault = '';
    var ProductType = '';
    var UserFullName = '';
    var options = {
        title: "Generate Data Claim",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
            { name: "btnSave", text: "Save", icon: "icon-save", cls:"" },
            { name: "btnDelete", text: "Delete", icon: "icon-remove", cls: "hide" },
            { name: "btnPrint", text: "Print", icon: "icon-print", cls: "hide" }
        ],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    {
                        text: "Company",
                        type: "controls",
                        items: [
                            { name: "CompanyCode", text: "CompanyCode", cls: "span2", readonly: true },
                            { name: "CompanyName",text:"CompanyName", cls: "span6", readonly: true }
                        ]
                    },
                    {
                        name: "BatchNo",
                        cls: "span4",
                        text: "Batch No",
                        readonly: true,
                        required:"required"
                    },
                    {
                        name: "LotNo",
                        cls: "span4",
                        text: "Lot No",
                        readonly: true,
                    },
                    {
                        name: "LockingBy",
                        text: "Claim Flag",
                        type: "select",
                        required:"required",
                        items: [
                            { value: 'WS', text: 'SERVICE' },
                            { value: 'WP', text: 'SPAREPART' },
                        ]
                    },
                    {
                        name: "ReceiptNo",
                        cls: "span4",
                        text: "Kwitansi No",
                        required: "required"
                    },
                    {
                        name: "ReceiptDate",
                        text: "Kwitansi Date",
                        cls: "span4",
                        type: "datepicker",
                    },
                    {
                        name: "FPJNo",
                        cls: "span4",
                        text: "FPJ No",
                    },
                    {
                        name: "FPJDate",
                        text: "FPJ Date",
                        cls: "span4",
                        type: "datepicker"
                    },
                    {
                        name: "FPJGovNo",
                        text: "No Seri Pajak",
                    }, {
                        name: "GenerateNo",
                        cls:"hide"
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnQuery", text: "QUERY", icon: "icon-gear", cls: "span4" },
                            { name: "btnGenereate", text: "GENERATE", icon: "icon-gear", cls: "span4" },
                        ]
                    },
                ]
            },
            {
                name: "PnlInfoPart",
                xtype: "table",
                tblname: "tblInfoPart",
                showcheckbox: true,
                columns: [
                    //{ text: "Action", type: "check", width: 50 },
                    { name: "BranchCode", text: "Branch Code", width: 80 },
                    { name: "GenerateNo", text: "No. Warr. Claim", width: 100 },
                    { name: "GenerateDate", text: "Warr. Claim Date", type: "dateTime", width: 110 },
                    { name: "TotalNoOfItem", text: "Claim Amount", width: 110, type: 'number' },
                    { name: "TotalClaimAmt", text: "Rupiah Amount", width: 110, type: 'price' },
                    { name: "SenderDealerCode", text: "Dealer Code", width: 110 },
                    { name: "SenderDealerName", text: "Dealer Name", width: 110 },
                    
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
        widget.clearForm();
        $.post('sv.api/GenerateClaim/default', function (result) {
            widget.default = $.extend({}, result);
            widget.populate(result);
            FPJDateDefault = result.FPJDate;
            ProductType = result.ProductType;
            UserFullName = result.UserFullName
            getDataQuery("uncheck");
            $("#pnlRefService *, #PnlInfoPart *").removeAttr("disabled");
            $("#btnGenereate").attr("disabled", "disabled");
            $("#btnDelete, #btnPrint, #btnSave").addClass("hide", "hide");
            $("#btnJobOrderNo").attr("disabled", "disabled");
            $("#btnJobOrderNoEnd").attr("disabled", "disabled");
            $(".datepicker-wrapper input[name='FPJDate']").attr("disabled", "disabled");
        });
    };

    $('#btnCreate').on('click', function (e) {
        SetViewMode('new');
    });

    $("#btnSave").on("click", function () {
        var isValid = $(".main form").valid();
        if (isValid) {
            var data = [];
            for (var i = 0; i <= 7 ; i++) {
                data[i] = $('.row_selected').map(function (idx, el) {
                    var td = $(el).find('td');
                    return td.eq(i).text();
                }).get();
            }

            var nModel = [];
            var datLen = data[0].length;
            if (datLen == 0) {
                MsgBox("Tidak/Belum ada data yang dipilih.", MSG_WARNING);
                return;
            }

            var claimNo = "";
            for (var i = 0; i < datLen; i++) {
                var myModel = new Model(data, i);
                if (i == datLen - 1) {
                    claimNo += "'" + myModel.GenerateNo + "'";
                } else {
                    claimNo += "'" + myModel.GenerateNo + "',";
                }
            }

            $("#GenerateNo").val(claimNo);
            SaveQuery();
        }
    });

    $("#btnDelete").on("click", DeleteData);

    $("#btnPrint").on("click", function (e) {
        var prm = [
            ProductType,
            $('#BatchNo').val()
        ];

        widget.showPdfReport({
            id: 'SvRpTrn013',
            pparam: prm.join(','),
            rparam: UserFullName,
            type: "devex"
        });
    });

    $("#btnBrowse").on("click", function () {
        var sentralize = GnMstLookUpDtl();
        var param = $(".main .gl-widget").serializeObject();
        var lookup = widget.klookup({
            name: "GenerateClm",
            title: "Generate Claim",
            url: "sv.api/grid/GenerateClm?sentralize=" + sentralize,
            serverBinding: true,
            isServerFiltering: true,
            pageSize: 10,
            filters: [
                { name: "BatchNo", text: "Batch No", cls: "span4" },
                { name: "ReceiptNo", text: "Kwitansi No", cls: "span4" },
                { name: "FPJNo", text: "No. Faktur Pajak", cls: "span4" },
                { name: "FPJGovNo", text: "No Seri Pajak", cls: "span4" },
            ],
            columns: [
                { field: "BatchNo", title: "Batch No", width: 110 },
                {
                    field: "BatchDate", title: "Batch Date", width: 130,
                    template: "#= (BatchDate == undefined) ? '' : moment(BatchDate).format('DD MMM YYYY') #"
                },
                { field: "ReceiptNo", title: "Kwitansi No", width: 110 },
                {
                    field: "ReceiptDate", title: "Kwitansi Date", width: 130,
                    template: "#= (ReceiptDate == undefined) ? '' : moment(ReceiptDate).format('DD MMM YYYY') #"
                },
                { field: "FPJNo", title: "No. Faktur Pajak", width: 110 },
                {
                    field: "FPJDate", title: "Tgl. Faktur Pajak", width: 130,
                    template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #"
                },
                { field: "FPJGovNo", title: "No Seri Pajak", width: 110 },
                { field: "LotNo", title: "Lot No", width: 110 },
                { field: "ProcessSeq", title: "Counter Proses", width: 110 },
                { field: "TotalNoOfItem", title: "Amount Item", width: 110, type: 'number' },
                { field: "TotalClaimAmt", title: "Total Amount", width: 150, format: "{0:#,##0}" },
                { field: "OtherCompensationAmt", title: "Total Amount", width: 150, format: "{0:#,##0}" },
                
            ],
        });
        lookup.dblClick(function (data) {
            console.log(data);
            widget.populate(data);
            var data2 = { FPJDate: FPJDateDefault, FPJNo: '', FPJGovNo: '' };
            widget.populate(data2);
            getDataQuery("check");
            SetViewMode('browse');
        });
    });

    $("#btnQuery").on("click", function (e) {
        var param = $(".main .gl-widget").serializeObject();
        widget.post("sv.api/GenerateClaim/GetClaimBatchQuery", param, function (result) {
            widget.populateTable({ selector: "#tblInfoPart", data: result, selectable: true, multiselect: true });
            SetViewMode('query');
            //$("#tblInfoPart tr td i, #tblInfoPart th i").removeClass("icon-check-empty");
            //$("#tblInfoPart tr td i, #tblInfoPart th i").addClass("icon-check");
        });
    });

    $("#btnGenereate").on("click", function () {
        var isValid = $(".main form").valid();
        console.log(isValid);
        if (isValid) {
            var param = { BatchNo: $('#BatchNo').val() };
            var contents = "";
            widget.post("sv.api/GenerateClaim/Generate", param, function (result) {
                contents = result.data;
                console.log(contents);
                BootstrapDialog.show({
                    //BootstrapDialog:'SIZE_LARGE',
                    title: '<b><i class="icon icon-gear"></i> WCLAM DATA</b>',
                    message: result.data,
                    buttons: [{
                        id: 'btnSaveFile',
                        label: 'Save File',
                        cssClass: 'button',
                        icon: 'icon icon-save',
                        action: function (dialogItself) {
                            var par = 'BatchNo='+ $('#BatchNo').val();
                            var url = SimDms.baseUrl + 'sv.api/GenerateClaim/SaveFile?' + par;
                            window.location = url;
                        }
                    },
                    {
                        id: 'btnSendFile',
                        label: 'Send File',
                        cssClass: 'button',
                        icon: 'icon icon-location-arrow',
                        action: function (dialogItself) {
                            BootstrapDialog.confirm('Ingin Mengirim File?', function (result) {
                                if (result) {
                                    var param = { Contents: contents };
                                    $.post("sv.api/GenerateClaim/SendFile", param, function (result) {
                                        if (result.success) {
                                            MsgBox(result.message);
                                        }
                                        else {
                                            MsgBox(result.message, MSG_ERROR);
                                        }
                                    }).
                                    error(function (data, status, headers, config) {
                                        MsgBox("Connection to the server failed...., status " + status, MSG_ERROR);
                                    });
                                    console.log(contents);
                                }
                                //else {
                                //    BootstrapDialog.alert('Testing');
                                //}
                            });
                            dialogItself.close();
                        }
                    },
                    {
                        label: 'Close',
                        cssClass: 'button',
                        icon: 'icon icon-undo',
                        action: function (dialogItself) {
                            dialogItself.close();
                        }
                    }]
                });
            });
        }
    });

    $("#FPJNo").on("keyup", function (e) {
        if ($(this).val() == '') {
            $(".datepicker-wrapper input[name='FPJDate']").attr("disabled", "disabled");
        } else {
            $(".datepicker-wrapper input[name='FPJDate']").removeAttr("disabled");
        }
    });

    function GnMstLookUpDtl() {
        var sentralize = 0;
        var param = $(".main .gl-widget").serializeObject();
        widget.post("sv.api/GenerateClaim/GnMstLookUpDtl", param, function (result) {
            if (result == null) {
                sentralize = 1;
            } else if (result["ParaValue"] == "1") {
                sentralize = 1;
            }
        });
        return sentralize;
    }

    function getDataQuery(p) {
        if (p == "check") {
            var param = $(".main .gl-widget").serializeObject();
            widget.post("sv.api/GenerateClaim/getDataQuery", param, function (result) {
                widget.populateTable({ selector: "#tblInfoPart", data: result, selectable: true, multiselect: true });
                $("#tblInfoPart tr td i, #tblInfoPart th i").removeClass("icon-check-empty");
                $("#tblInfoPart tr td i, #tblInfoPart th i").addClass("icon-check");
            });
        } else if (p == "uncheck") {
            var param = $(".main .gl-widget").serializeObject();
            widget.post("sv.api/GenerateClaim/getDataQuery", param, function (result) {
                widget.populateTable({ selector: "#tblInfoPart", data: result, selectable: true, multiselect: true });
                $("#tblInfoPart tr td i, #tblInfoPart th i").removeClass("icon-check");
                $("#tblInfoPart tr td i, #tblInfoPart th i").addClass("icon-check-empty");
            });
        }
    }

    function SaveQuery() {
        var param = $(".main .gl-widget").serializeObject();
        $(".ajax-loader").show();
        $.post('sv.api/GenerateClaim/Save', param).
        success(function (result, status, headers, config) {
            if (result.success) {
                $(".ajax-loader").hide();
                $("#BatchNo").val(result.data.Batch);
                getDataQuery("check");
                SetViewMode('browse');
                SimDms.Success(result.message);
           }
            else {
                $(".ajax-loader").hide();
               SimDms.Error(result.message);
           }
       }).
       error(function (data, status, headers, config) {
           $(".ajax-loader").hide();
           MsgBox("Connection to the server failed...., status " + status, MSG_ERROR);
       });
    }
 
    function DeleteData() {
        widget.confirm("Apakah Anda Yakin???", function (con) {
            if (con == "Yes") {
                param = { BatchNo: $('#BatchNo').val() };
                $.post('sv.api/GenerateClaim/DeleteData', param).
                success(function (result, status, headers, config) {
                    if (result.success) {
                        SetViewMode("new");
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
    };

    function Model(data, row) {
        //this.BranchCode = data[1][row];
        this.GenerateNo = data[2][row];
        //this.GenerateDate = data[3][row];
        //this.TotalNoOfItem = data[4][row];
        //this.TotalClaimAmt = data[5][row];
        //this.SenderDealerCode = data[6][row];
        //this.SenderDealerName = data[7][row];

    }

    function SetViewMode(mode) {
        switch (mode) {
            case 'browse':
                $("#pnlRefService *, #PnlInfoPart *").attr("disabled", "disabled");
                $("#btnGenereate").removeAttr("disabled");
                $("#btnDelete, #btnPrint").removeClass("hide");
                $("#btnSave").addClass("hide", "hide");
                break;
            case 'new':
                init();
                break;
            case 'query':
                $("#btnSave").removeClass("hide");
                break;
            default:
                break
        }
    }
});