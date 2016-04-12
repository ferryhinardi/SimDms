function svReceiveClaimController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.NewData();
    };

    me.NewData = function () {
        me.data.Content = undefined;
        Wx.clearForm();
        $("#pnlInput *").removeAttr("disabled");
    };

    me.start();

    me.LookupSender = function () {
        var params = {
            categoryCodes: "88,01"
        };

        var lookup = Wx.klookup({
            name: "SenderList",
            title: "Lookup Pengirim",
            url: "sv.api/grid/CustomerLookup",
            params: params,
            serverBinding: true,
            pageSize: 10,
            sort: [
                { field: 'CustomerName', dir: 'asc' },
                { field: 'CustomerCode', dir: 'asc' }
            ],
            columns: [
                { field: "CustomerCode", title: "Kode Pengirim", width: 160 },
                { field: "CustomerName", title: "Nama Pengirim", width: 400 }
            ],
        });
        lookup.dblClick(function (data) {
            me.data.CustomerCode = data.CustomerCode;
            me.data.CustomerName = data.CustomerName;
            me.Apply();
            $("#PaymentNo").focus();
        });
    };

    me.ReceiveFile = function () {
        if (Wx.validate()) {
            me.data.Content = fileContentReceive;
            $http.post('sv.api/receiveclaim/ReceiveFile', me.data)
            .success(function (result, status, headers, config) {
                if (result.success) {
                    me.PopulatePanelInfo(result);
                    me.SetViewMode('receive');
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

    me.PrintReceive = function () {
        Wx.loadForm();
        Wx.showForm({ url: 'sv/report/svrptrn014' });
    };

    me.SetViewMode = function (mode) {
        switch (mode) {
            case 'new':
                me.NewData();
                break;
            case 'receive':
                $("#pnlInput *").attr("disabled", "disabled");
                break;
            default:
                break
        }
    }
};

$(document).ready(function () {
    var options = {
        title: "Receive Data Claim",
        xtype: "panels",
        toolbars: [
            { name: "btnCreate", text: "New", icon: "icon-file", cls: "btn btn-primary", click: 'initialize()' },
            { name: "btnPrint", text: "Print", icon: "icon-print", cls: "btn btn-primary", show: '!isSaveMode', click: 'PrintReceive()' }
        ],
        panels: [
            {
                name: "pnlInput",
                items: [
                    { name: "CustomerCode", model: 'data.CustomerCode', text: "Pengirim", cls: "span4 Full", type: "popup", required: true, readonly: true, click: "LookupSender()" },
                    { name: "CustomerName", model: 'data.CustomerName', text: "", cls: "span8", readonly: true },
                    { name: "PaymentNo", model: 'data.PaymentNo', text: "No. Pembayaran", cls: "span4", required: true },
                    { name: "PaymentDate", model: 'data.PaymentDate', text: "Tanggal", cls: "span4", type: "ng-datepicker", readonly: true },
                    { name: "TotalTicket", model: 'data.TotalTicket', text: "Jumlah Kupon", cls: "span4", readonly: true },
                    { name: "FileID", model: 'data.FileID', type: "hidden" },
                    {
                        name: "ReceiveFileName",
                        text: "Nama File",
                        readonly: true,
                        type: "upload",
                        url: "sv.api/receiveclaim/UploadFile",
                        icon: "icon-folder-open",
                        callback: "ReceiveFileCallback",
                        cls: "span4",
                        required: true
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnReceive", text: "Receive File Warranty Claim", icon: "icon-upload", cls: "btn btn-primary", click: "ReceiveFile()" }
                        ]
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);

    Wx.default = {
    };

    Wx.render(function () {
        init();
    });

    function init() {
        SimDms.Angular("svReceiveClaimController");
    }
});

var fileContentReceive = '';
function ReceiveFileCallback(result) {
    if (!result.success) {
        MsgBox(result.message, MSG_ERROR);
        return;
    }
    $('[name=ReceiveFileNameShowed]').val(result.FileName);
    fileContentReceive = result.Content;
}