function svUploadClaimController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data.datPartInfo = undefined;
        me.data.Content = undefined;

        Wx.clearForm();
        $http.post('sv.api/uploadclaim/default', '')
        .success(function (result, status, headers, config) {
            me.data = result;
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });

        var datInfo = {
            CostInfo: '',
            DetailInfo: '',
            PartInfo: ''
        }
        me.PopulatePanelInfo(datInfo);
        me.clearTable(me.GridDetail);

        //$("#btnSave").hide();
        me.isSaveMode = false;
        me.isPopulate = false;
        $("#pnlInput *").removeAttr("disabled");
    };
    me.start();

    me.CheckSave = function () {
        var params = me.data;
        $http.post('sv.api/uploadclaim/CheckSave', params)
        .success(function (result, status, headers, config) {
            if (result.success) {
                if (result.exist) {
                    Wx.confirm(result.message, function (con) {
                        if (con == "Yes") {
                            Wx.confirm("Apakah Anda Yakin???", function (con) {
                                if (con == "Yes") {
                                    me.SaveData(params);
                                }
                            });
                        }
                    });
                }
                else {
                    Wx.confirm("Apakah Anda Yakin???", function (con) {
                        if (con == "Yes") {
                            me.SaveData(params);
                        }
                    });
                }
            }
            else {
                MsgBox(result.message, MSG_ERROR);
                return;
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    };

    me.SaveData = function(params){
        $http.post('sv.api/uploadclaim/Save', params).
        success(function (result, status, headers, config) {
            if (result.success) {
                if (result.HeaderInfo != undefined) {
                    me.PopulatePanelInfo(result);
                    me.data.datPartInfo = result.PartInfo;
                    me.isSaveMode = false;
                }
                else{
                    me.SetViewMode('new');
                }
            }
            else {
                SimDms.Error(result.message);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox("Connection to the server failed...., status " + status, MSG_ERROR);
        });
    };

    me.BrowseData = function () {
        var lookup = Wx.klookup({
            name: "trnWarranty",
            title: "Transaction - Warranty Lookup",
            url: "sv.api/grid/claim?" + "SourceData=2&fltStatusDefault=1",
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
                                { value: "", text: "PILIH SEMUA" },
                                { value: "0", text: "BELUM POSTING" },
                                { value: "1", text: "SUDAH POSTING", selected: 'selected' },
                                { value: "2", text: "DRAFT CLAIM" },
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
                    field: "FPJDate", title: "Tax Invoice Date", width: 210,
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
            params = { GenerateNo: data.GenerateNo };
            $http.post('sv.api/uploadclaim/PopulateData', params).
            success(function (result, status, headers, config) {
                if (result.success) {
                    if (result.HeaderInfo != undefined) {
                        me.PopulatePanelInfo(result);
                        me.data.datPartInfo = result.PartInfo;
                        me.isSaveMode = false;
                        me.SetViewMode('browse');
                    }
                    else {
                        me.SetViewMode('new');
                    }
                }
                else {
                    SimDms.Error(result.message);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox("Connection to the server failed...., status " + status, MSG_ERROR);
            });
        });
    };

    me.PrintClaim = function () {
        createCookie('sdc', '2', 1);
        Wx.loadForm();
        Wx.showForm({ url: 'sv/report/svrptrn013' });
    };

    me.LookupDealer = function () {
        var lookup = Wx.klookup({
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
            me.data.SenderDealerCode = data.CustomerCode;
            me.data.SenderDealerName = data.CustomerName;
            me.Apply();
        });
    };

    me.PopulateHeaderInfo = function (data) {
        if (data != undefined) {
            me.isPopulate = true;
            me.data = data;
            me.data.txtGenerateDate = data.GenerateDate;
        }
    };

    me.PopulatePartInfo = function (data) {
        me.data.TroubleDescription = data.TroubleDescription;
        me.data.ProblemExplanation = data.ProblemExplanation;

        if (data.PartInfo != '') {
            var filterPartInfo = data.PartInfo.filter(function (arr) {
                return arr.GenerateSeq == data.GenerateSeq
            });

            Wx.populateTable({ selector: "#tblPartInfo", data: (filterPartInfo == undefined) ? {} : filterPartInfo });
        }
        else {
            Wx.populateTable({ selector: "#tblPartInfo", data: '' });
        }
    };

    me.PopulatePanelInfo = function (result) {
        Wx.populateTable({ selector: "#tblCostInfo", data: result.CostInfo });

        if (result.DetailInfo != undefined) {
            var dataPart = {
                TroubleDescription: (result.DetailInfo[0] == undefined) ? '' : result.DetailInfo[0].TroubleDescription,
                TroubleDescription: (result.DetailInfo[0] == undefined) ? '' : result.DetailInfo[0].TroubleDescription,
                GenerateSeq: (result.DetailInfo[0] == undefined) ? '' : result.DetailInfo[0].GenerateSeq,
                PartInfo: result.PartInfo
            }
            me.PopulateHeaderInfo(result.HeaderInfo);
            me.grid.detail = result.DetailInfo;
            me.loadTableData(me.GridDetail, me.grid.detail);
            me.PopulatePartInfo(dataPart);
        }
    };

    me.Upload = function () {
        if (Wx.validate()) {
            me.data.Content = fileContent;
            var params = {
                Content: me.data.Content,
                SenderDealerCode: me.data.SenderDealerCode,
                RefferenceNo: me.data.RefferenceNo,
                RefferenceDate: me.data.RefferenceDate,
                LotNo: me.data.LotNo
            }
            $http.post('sv.api/uploadclaim/Upload', params)
            .success(function (result, status, headers, config) {
                if (result.success) {
                    me.PopulatePanelInfo(result);
                    me.data.datPartInfo = result.PartInfo;
                    me.SetViewMode('upload');
                }
                else {
                    MsgBox(result.message, MSG_ERROR);
                    return;
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
        else {
            MsgBox('Ada informasi yang tidak lengkap.', MSG_WARNING);
        }
    };

    me.SetViewMode = function (mode) {
        switch (mode) {
            case 'browse':
                me.isSaveMode = false;
                $("#pnlInput *").attr("disabled", "disabled");
                break;
            case 'new':
                init();
                break;
            case 'upload':
                me.isSaveMode = true;
                $("#pnlInput *").attr("disabled", "disabled");
                break;
            default:
                break
        }
    }

    me.GridDetail = new webix.ui({
        container: "partnp",
        view: "wxtable", css:"alternating", scrollX: true,
        columns: [
            { id: "GenerateSeq", header: "No.", width: 50 },
            { id: "CategoryCode", header: "Kd. Kategori", width: 120 },
            { id: "IssueNo", header: "No. Issue", width: 250 },
            { id: "IssueDate", header: "Tgl. Issue", width: 130, format: me.dateFormat },
            { id: "InvoiceNo", header: "No. Faktur", width: 200 },
            { id: "ServiceBookNo", header: "No. Buku Service", width: 160 },
            { id: "ChassisCode", header: "Kode Rangka", width: 160 },
            { id: "ChassisNo", header: "No. Rangka", width: 140 },
            { id: "EngineCode", header: "Kode Mesin", width: 140 },
            { id: "EngineNo", header: "No. Mesin", width: 140 },
            { id: "BasicModel", header: "Basic Model", width: 140 },
            { id: "RegisteredDate", header: "Tgl. Register", width: 130, format: me.dateFormat },
            { id: "RepairedDate", header: "Tgl. Perbaikan", width: 130, format: me.dateFormat },
            { id: "Odometer", header: "Odometer", width: 200, format: webix.i18n.intFormat, css: { 'text-align': 'right' } },
            { id: "ComplainCode", header: "CC", width: 50 },
            { id: "DefectCode", header: "DC", width: 50 },
            { id: "SubletHour", header: "Sublet", width: 100, format: webix.i18n.numberFormat, css: { 'text-align': 'right' } },
            { id: "BasicCode", header: "Basic Code", width: 100 },
            { id: "VarCom", header: "Var/Com", width: 100 },
            { id: "OperationHour", header: "Hours", width: 80, format: webix.i18n.numberFormat, css: { 'text-align': 'right' } },
            { id: "ClaimAmt", header: "Nilai Claim", width: 200, format: webix.i18n.intFormat, css: { 'text-align': 'right' } }
        ],
        on: {
            onSelectChange: function () {
                if (me.GridDetail.getSelectedId() !== undefined) {
                    me.drDetail = this.getItem(me.GridDetail.getSelectedId().id);
                    var data = {
                        TroubleDescription: me.drDetail.TroubleDescription,
                        TroubleDescription: me.drDetail.TroubleDescription,
                        GenerateSeq: me.drDetail.GenerateSeq,
                        PartInfo: me.data.datPartInfo
                    }
                    me.PopulatePartInfo(data);
                    me.Apply();
                }
            }
        }
    });
};

$(document).ready(function () {
    var options = {
        title: "Upload Claim Data (Cabang/Sub-Dealer)",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file", cls: "btn btn-primary", click: 'initialize()' },
            { name: "btnBrowse", text: "Browse", icon: "icon-search", cls: "btn btn-info", show: '!isSaveMode', click: 'BrowseData()' },
            { name: "btnSave", text: "Save", icon: "icon-save", cls: "btn btn-success", show: 'isSaveMode', click: 'CheckSave()' },
            { name: "btnPrint", text: "Print", icon: "icon-print", cls: "btn btn-primary", show: '!isSaveMode', click: 'PrintClaim()' }
        ],
        panels: [
            {
                name: "pnlInput",
                items: [
                    { name: "GenerateNo", model: 'data.GenerateNo', text: "No. Warr. Claim", placeHolder: "CLA/XX/YYYYYY", type: "popup", cls: "span4", readonly: true },
                    { name: "GenerateDate", model: 'data.GenerateDate', text: "Tgl. Warr. Claim", cls: "span4", type: "ng-datepicker", show: "!isPopulate" },
                    { name: "txtGenerateDate", model: 'data.txtGenerateDate', text: "Tgl. Warr. Claim", cls: "span4", type: "ng-datetimepicker", show: "isPopulate" },
                    { name: "SenderDealerCode", model: 'data.SenderDealerCode', text: "Dealer", cls: "span4 Full", type: "popup", required: true, readonly: true, click: "LookupDealer()" },
                    { name: "SenderDealerName", model: 'data.SenderDealerName', text: "", cls: "span8", readonly: true },
                    { name: "RefferenceNo", model: 'data.RefferenceNo', text: "No. Kwitansi", cls: "span4", required: true },
                    { name: "RefferenceDate", model: 'data.RefferenceDate', text: "Tgl. Kwitansi", cls: "span4", type: "ng-datepicker" },
                    { name: "LotNo", model: 'data.LotNo', text: "No. Lot", cls: "span4", required: true, maxlength: 7, type: "numeric" },
                    { name: "FileID", model: 'data.FileID', type: "hidden" },
                    {
                        name: "UploadFileName",
                        text: "File",
                        readonly: true,
                        type: "upload",
                        url: "sv.api/uploadclaim/UploadFile",
                        icon: "icon-folder-open",
                        callback: "uploadCallback",
                        cls: "span4",
                        required: true
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnUpload", text: "Upload", icon: "icon-upload", cls: "btn btn-primary", click: "Upload()" }
                        ]
                    }
                ]
            }, {
                title: "Informasi Jumlah Rupiah",
                xtype: "table",
                tblname: "tblCostInfo",
                cls: "span4",
                columns: [
                    { name: "RecNo", text: "No.", width: 50 },
                    { name: "BasicModel", text: "Basic Model" },
                    { name: "TotalClaimAmt", text: "Total", cls: "right number", type: "numeric" }
                ]
            },{
                name: "pnlDetail",
                title: "Informasi Claim",
                items: [
                    {
                        name: "partnp",
                        type: "wxdiv"
                    }
                ]
            }, {
                name: "pnlTroubleInfo",
                title: "Informasi Masalah",
                items: [
                      { name: "TroubleDescription", text: "Keluhan/Masalah", type: "textarea", readonly: true },
                      { name: "ProblemExplanation", text: "Penjulasan Masalah", type: "textarea", readonly: true }
                ]
            }, {
                title: "Informasi Part",
                xtype: "table",
                tblname: "tblPartInfo",
                cls: "span4",
                columns: [
                    { name: "PartSeq", text: "No.", width: 50 },
                    { name: "IsCausal", text: "Is Causal" },
                    { name: "PartNo", text: "Part No." },
                    { name: "Quantity", text: "Qty", cls: "right number", type: "numeric" },
                    { name: "PartName", text: "Part Name" }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);

    Wx.default = {};

    Wx.render(function () {
        init();
    });

    function init() {
        SimDms.Angular("svUploadClaimController");
    }
});

var fileContent = '';
function uploadCallback(result) {
    if (!result.success) {
        MsgBox(result.message, MSG_ERROR);
        return;
    }
    $('[name=UploadFileNameShowed]').val(result.FileName);
    fileContent = result.Content;
}