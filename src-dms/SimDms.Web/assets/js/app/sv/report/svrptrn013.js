function svRpTrn013Controller($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data.SourceData = readCookie('sdc');
    };
    me.start();

    me.lookupClaimBegin = function () {
        me.lookupClaim().dblClick(function (data) {
            if (me.data.GenerateNoEnd < data.GenerateNo) {
                me.data.GenerateNo = data.GenerateNo;
                me.data.GenerateNoEnd = data.GenerateNo;
            }
            else {
                me.data.GenerateNo = data.GenerateNo;
            }
            me.Apply();
        });
    };

    me.lookupClaimEnd = function () {
        me.lookupClaim().dblClick(function (data) {
            if (me.data.GenerateNo > data.GenerateNo) {
                me.data.GenerateNo = data.GenerateNo;
                me.data.GenerateNoEnd = data.GenerateNo;
            }
            else {
                me.data.GenerateNoEnd = data.GenerateNo;
            }
            me.Apply();
        });
    };

    me.lookupClaim = function() {
        var lookup = Wx.klookup({
            name: "lookupClaimBegin",
            title: "Transaction - Warranty Claim Lookup",
            url: "sv.api/grid/claim?SourceData=" + me.data.SourceData + "&report=true",
            serverBinding: true,
            sort: [
                { 'field': 'GenerateNo', 'dir': 'desc' },
                { 'field': 'GenerateDate', 'dir': 'desc' }
            ],
            pageSize: 10,
            columns: [
                { field: "GenerateNo", title: "No. Warranty", width: 150 },
                {
                    field: "GenerateDate", title: "Tgl. Warranty", width: 130,
                    template: "#= (GenerateDate == undefined) ? '' : moment(GenerateDate).format('DD MMM YYYY') #"
                },
                { field: "FPJNo", title: "No. Faktur Pajak", width: 150 },
                { field: "FPJDate", title: "Tgl. Faktur Pajak", width: 150 },
                { field: "FPJGovNo", title: "Seri Pajak", width: 150 },
                { field: "SenderDealerName", title: "Nama Dealer Pengirim", width: 200 },
                { field: "SenderDealerCode", title: "Kode Dealer Pengirim", width: 120 },
                { field: "Invoice", title: "No. Faktur Penjualan", width: 260 },
                { field: "SourceDataDesc", title: "Sumber Data", width: 260 },
                { field: "TotalNoOfItem", title: "Total Record", width: 110 },
                { field: "TotalClaimAmt", title: "Total Warranty", width: 110, format: "{0:#,##0}" },
                { field: "RefferenceNo", title: "No. Ref.", width: 100 },
                { field: "RefferenceDate", title: "Tgl. Ref.", width: 120 },
                { field: "RefferenceDate", title: "Tgl. Ref.", width: 120 },
                { field: "PostingFlagDesc", title: "Status", width: 120 }
            ]
        });

        return lookup;
    };

    me.print = function () {
        if (!$('.main form').valid())
            return;

        Wx.XlsxReport({
            url: 'sv.api/getwarrantyclaim/SvRpTrn013',
            type: 'xlsx',
            params: me.data
        });
        eraseCookie('sdc');
    }
}

$(document).ready(function () {
    var docTitle = '';
    switch(readCookie('sdc')){
        case '0':
            docTitle = 'Report Get Warranty Claim';
            break;
        case '1':
            docTitle = 'Report Input Warranty Claim';
            break;
        case '2':
            docTitle = 'Report Upload Warranty Claim';
            break;
        default:
            break;
    };
    var options = {
        title: docTitle,
        xtype: "panels",
        panels: [
            {
                name: "pnlInput",
                items: [
                    {
                        name: "GenerateNo",
                        model: "data.GenerateNo",
                        cls: "span4",
                        text: "No. Warranty ",
                        type: "popup",
                        btnName: "btnGenerateNo",
                        readonly: true,
                        required: "required",
                        click: 'lookupClaimBegin()'
                    }, {
                        name: "GenerateNoEnd",
                        model: "data.GenerateNoEnd",
                        text: "s/d",
                        cls: "span4",
                        type: "popup",
                        btnName: "btnGenerateNoEnd",
                        readonly: true,
                        required: "required",
                        click: 'lookupClaimEnd()'
                    }, {
                        name: "SourceData",
                        model: "data.SourceData",
                        type: 'hide'
                    },{
                        type: "buttons",
                        items: [
                            { name: "btnPrint", text: "Print", icon: "icon-print", cls: "btn btn-primary span4", click: 'print()' },
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
        SimDms.Angular("svRpTrn013Controller");
    }
});