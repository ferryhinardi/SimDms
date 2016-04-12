"use strict"; //Reportid OmRpSalesRgs001
function RptRegisterHarianPenerbitan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.BranchFrom = function () {
        var lookup = Wx.blookup({
            name: "BranchBrowse",
            title: "Pemasok",
            manager: spSalesManager,
            query: "BranchLookup4Report",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "CompanyName", title: "Nama Cabang" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCodeFrom = data.BranchCode;
                me.data.BranchName = data.CompanyName;
                me.Apply();
            }
        });
    };
    me.printPreview = function () {
        var BranchCode = "";
        var DateFrom = new Date(me.data.DateFrom).getMonth() + 1 + '/' + new Date(me.data.DateFrom).getDate() + '/' + new Date(me.data.DateFrom).getFullYear();
        var DateTo = new Date(me.data.DateTo).getMonth() + 1 + '/' + new Date(me.data.DateTo).getDate() + '/' + new Date(me.data.DateTo).getFullYear();
        var ReportId = 'GnRpTaxTrn005';
        if ($('#isC1').prop('checked') == true && me.data.BranchCodeFrom != "") {
            BranchCode = me.data.BranchCodeFrom
        }
        var prm = [
                    'companycode',
                    BranchCode,
                    "4w",
                    DateFrom,
                    DateTo
                    
        ];
        Wx.showPdfReport({
            id: ReportId,
            pparam: prm.join(','),
            rparam: "semua",
            type: "devex"
        });
    }
    me.initialize = function () {
        me.data = {};
        me.data.DateFrom = me.now();
        me.data.DateTo = me.now();

        $http.get('breeze/sales/CurrentUserInfo').
              success(function (dl, status, headers, config) {
                  me.data.CompanyCode = dl.CompanyCode;
                  me.data.BranchCode = dl.BranchCode;
              });
        $('#BranchCodeFrom').attr('disabled', true);
        $('#btnBranchCodeFrom').attr('disabled', true);
        me.Apply();
        me.isPrintAvailable = true;
    }
    $('#isC1').on('change', function (e) {
        if ($('#isC1').prop('checked') == true) {
            $('#BranchCodeFrom').removeAttr('disabled');
            $('#btnBranchCodeFrom').removeAttr('disabled');
        } else {
            $('#BranchCodeFrom').attr('disabled', true);
            $('#btnBranchCodeFrom').attr('disabled', true);
            $('#BranchCodeFrom').val("");
            $('#BranchName').val("");
         }
        me.Apply();
    })
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Akumulasi PPN yang Harus di Bayar",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                    { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    {
                        text: "Periode :",
                        type: "controls",
                        items: [
                                { name: "DateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                { name: "DateTo", text: "", cls: "span3", type: "ng-datepicker" },
                                    ]
                    },
                    {
                        text: "Cabang",
                        type: "controls",
                        items: [
                            { name: "isC1", cls: "span1", type: "check" },
                            { name: "BranchCodeFrom", cls: "span2", type: "popup", click: "BranchFrom()" },
                            { name: "BranchName", cls: "span4", readonly: true },
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
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("RptRegisterHarianPenerbitan");
    }
});