function svRpTrn014Controller($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data.SourceData = readCookie('sdc');
    };
    me.start();

    me.lookupRefferenceNo = function () {
        me.lookupClaim().dblClick(function (data) {
            me.data = data;
            me.Apply();
        });
    };

    me.lookupClaim = function () {
        var lookup = Wx.klookup({
            name: "lookupRefferenceNo",
            title: "Transaction - Receive Warranty Claim Lookup",
            url: "sv.api/grid/ReceiveWClaimLookup",
            serverBinding: true,
            sort: [
                { 'field': 'SuzukiRefferenceNo', 'dir': 'desc' },
                { 'field': 'SuzukiRefferenceDate', 'dir': 'desc' }
            ],
            pageSize: 10,
            columns: [
                { field: "SuzukiRefferenceNo", title: "No. Pembayaran", width: 130 },
                { 
                    field: "SuzukiRefferenceDate", title: "Tgl. Pembayaran", width: 130,
                    template: "#= (SuzukiRefferenceDate == undefined) ? '' : moment(SuzukiRefferenceDate).format('DD MMM YYYY') #"
                },
                { 
                    field: "ReceivedDate", title: "Tgl. Penerimaan", width: 130,
                    template: "#= (ReceivedDate == undefined) ? '' : moment(ReceivedDate).format('DD MMM YYYY') #"
                },
                { field: "SenderDealerCode", title: "Dealer Pengirim", width: 100 },
                { field: "SenderDealerName", title: "Nama Dealer Pengirim", width: 160 },
                { field: "ReceiveDealerCode", title: "Dealer Penerima", width: 100 },
                { field: "LotNo", title: "No. Lot", width: 100 },
                { field: "BatchNo", title: "No. Batch", width: 100 }
            ]
        });

        return lookup;
    };

    me.PrintDetail = function () {
        Wx.XlsxReport({
            url: 'sv.api/receiveclaim/SvRpTrn014Detail',
            type: 'xlsx',
            params: me.data.SuzukiRefferenceNo

        });
    };

    me.Print = function () {
        if(me.validate()) {
            Wx.XlsxReport({
                url: 'sv.api/receiveclaim/SvRpTrn014Summary',
                type: 'xlsx',
                params: me.data.SuzukiRefferenceNo
            }, me.PrintDetail);
        }
    };

    me.GenerateFile = function () {
        if(me.validate()) {
            $http.post('sv.api/receiveclaim/GenerateWClaimFile', me.data)
            .success(function (result, status, headers, config) {
                if (result.success) {
                    MsgBox(result.message, MSG_SUCCESS);
                }
                else {
                    MsgBox(result.message, MSG_WARNING);
                    return;
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        }
    };

    me.validate = function () {
        if (!Wx.validate()) {
            MsgBox("Silahkan Pilih No. Pembayaran terlebih dahulu!!", MSG_WARNING);
            return false;
        }
        else {
            return true;
        }
    };
}

$(document).ready(function () {
    var options = {
        title: "Report Receive Warranty Claim",
        xtype: "panels",
        panels: [
            {
                name: "pnlInput",
                items: [
                    {
                        name: "SuzukiRefferenceNo", model: "data.SuzukiRefferenceNo", cls: "span4 full", text: "No. Pembayaran", type: "popup",
                        btnName: "btnSuzukiRefferenceNo", readonly: true, required: "required", click: 'lookupRefferenceNo()'
                    },
                    {
                        name: "SuzukiRefferenceDate", model: 'data.SuzukiRefferenceDate', text: "Tgl. Pembayaran", cls: "span4 full", type: "ng-datepicker", readonly: true
                    },
                    {
                        name: "ReceivedDate", model: 'data.ReceivedDate', text: "Tgl. Penerimaan", cls: "span4 full", type: "ng-datepicker", readonly: true
                    },
                    {
                        name: "BatchNo", model: 'data.BatchNo', text: "No. Batch", cls: "span4 full", readonly: true
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnGenerate", text: "Generate", icon: "icon-gear", cls: "btn btn-info span4", click: 'GenerateFile()' },
                            { name: "btnPrint", text: "Print", icon: "icon-print", cls: "btn btn-primary span4", click: 'Print()' },
                        ]
                    }
                ]
            }
        ]
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svRpTrn014Controller");
    }
});