"use strict"

function svRptSrvServiceClaimDetailListController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $('#FirstPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod > endPeriod) { $('#EndPeriod').val(firstPeriod) }
    });

    $('#EndPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod < endPeriod) { $('#FirstPeriod').val(endPeriod) }
    });

    me.$watch('options', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            if(newValue == "0")
            {
                me.IsSPK = true;
                me.IsBatch = false;
            }
            else
            {
                me.IsSPK = false;
                me.IsBatch = true;
            }
        }
    });

    me.$watch('data.IsBranch', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            if (newValue) {
                $('#btnBranchCode').prop('disabled', false);
            }
            else {
                $('#btnBranchCode').prop('disabled', true);
                $('#BranchCode, #CompanyName').val('');
            }
        }
    });

    me.$watch('data.IsBatch', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            if (newValue) {
                $('#btnBatchNo').prop('disabled', false);
            }
            else {
                $('#btnBatchNo').prop('disabled', true);
                $('#BatchNo').val('');
            }
        }
    });

    me.browseBranch = function (e) {
        var lookup = Wx.blookup({
            name: "BranchCodeBrowse",
            title: "Cabang",
            manager: ReportService,
            query: "BrowseBranch",
            defaultSort: "BranchCode asc",
            columns: [
               { field: 'BranchCode', title: 'Kode Cabang' },
               { field: 'CompanyName', title: 'Nama Cabang' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BranchCode = data.BranchCode;
            me.data.CompanyName = data.CompanyName;

            me.Apply();
        });
    }

    me.browseBatch = function (e) {
        var lookup = Wx.blookup({
            name: "BatchBrowse",
            title: "Cabang",
            manager: ReportService,
            query: "BatchBrowse",
            defaultSort: "BatchNo desc",
            columns: [
                    { field: 'BatchNo', title: 'No. Batch' },
                    {
                        field: "BatchDate", title: "Tgl. Batch", sWidth: "130px",
                        template: "#= (BatchDate == undefined) ? '' : moment(BatchDate).format('DD MMM YYYY') #"
                    },
                    { field: 'ReceiptNo', title: 'No. Kwitansi' },
                    { field: 'ReceiptDate', title: 'Tgl. Kwitansi' },
                    { field: 'FPJNo', title: 'No. Faktur Pajak' },
                    { field: 'FPJDate', title: 'Tgl. Faktur Pajak' },
                    { field: 'FPJGovNo', title: 'No. Seri Pajak' },
                    { field: 'LotNo', title: 'No. Lot' },
                    { field: 'ProcessSeq', title: 'Counter Proses' },
                    { field: 'TotalNoOfItem', title: 'Jumlah Item' },
                    { field: 'TotalClaimAmt', title: 'Total Nilai' },
                    { field: 'OtherCompensationAmt', title: 'Total Nilai' }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BatchNo = data.BatchNo;

            me.Apply();
        });
    }

    me.printPreview = function () {
        var ReportId = "SvRpReport031";
        var firstPeriod = moment(me.data.FirstPeriod).format('YYYYMMDD');
        var endPeriod = moment(me.data.EndPeriod).format('YYYYMMDD');

        var par = [
            'companycode',
            me.data.IsBranch ? me.data.BranchCode : '%%',
            firstPeriod,
            endPeriod,
            me.data.Batch,
            me.options
        ];

        var rparam = moment(me.data.FirstPeriod).format('DD-MM-YYY') + " s/d " + moment(me.data.EndPeriod).format('DD-MM-YYY');

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data.FirstPeriod = me.now();
        me.data.EndPeriod = me.now();
    }

    me.IsSPK = true;
    me.IsBatch = false;
    me.options = "0";
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Claim Detail List",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
            {
                name: "pnlClaimDetailList",
                items: [
                   {
                       type: "optionbuttons",
                       name: "tabpageoptions",
                       model: "options",
                       text: "Parameter",
                       items: [
                           { name: "0", text: "SPK" },
                           { name: "1", text: "Claim" },
                       ]
                   },
                   {
                       text: "Branch",
                       type: "controls",
                       items: [
                           { name: "IsBranch", cls: "span2", placeHolder: " ", readonly: true, type: "ng-switch" },
                           { name: "BranchCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", disable: true, click: "browseBranch()" },
                           { name: "CompanyName", cls: "span4", placeHolder: " ", readonly: true }
                       ]
                   },
                   { name: "FirstPeriod", text:"Periode",cls: "span3", type: "ng-datepicker",show:"IsSPK" },
                   { name: "EndPeriod", text: "S/D", cls: "span3", type: "ng-datepicker", show: "IsSPK" },
                   {
                       text: "Batch",
                       type: "controls",
                       show: "IsBatch",
                       items: [
                           { name: "IsBatch", cls: "span2", placeHolder: " ", readonly: true, type: "ng-switch" },
                           { name: "BatchNo", cls: "span4", placeHolder: " ", readonly: true, type: "popup", disable: true, click: "browseBatch()" },
                       ]
                   },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svRptSrvServiceClaimDetailListController");
    }
});