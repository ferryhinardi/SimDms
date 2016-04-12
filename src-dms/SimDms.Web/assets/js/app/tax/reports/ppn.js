"use strict"; //Reportid OmRpSalesRgs001
function RptRegisterHarianPenerbitan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    $http.post('tax.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.Month = data;
    });
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
        var Month = me.data.Month;
        var Year = me.data.Year;
        var date = Month + '/1/' + Year;
        var ReportId = 'GnRpTaxTrn006';
        if ($('#isC1').prop('checked') == true && me.data.BranchCodeFrom != "") {
            BranchCode = me.data.BranchCodeFrom
        }
        var prm = [
                    'companycode',
                    BranchCode,
                    "4w",
                    date
                    
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
        me.data.Year = new Date(Date.now()).getFullYear();
        me.data.Month = new Date(Date.now()).getMonth()+1;
        $('#Year').css("text-align", "right");

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
        title: "PPN yang Harus di Bayar Per Periode",
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
                        text: "Periode",
                        type: "controls",
                        cls: "span4 full",
                        items: [
                            { name: "Month", placeHolder: "Month", cls: "span5", type: "select2", datasource: "Month" },
                            { type: "label", text: " - ", cls: "span1 mylabel" },
                            { name: "Year", text: "", cls: "span2" },
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