"use strict"; //Reportid OmRpSalesRgs001
function RptRegisterNotaDebet($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.InvoiceNo = function () {
        var lookup = Wx.blookup({
            name: "InvoiceLookup",
            title: "Invoice",
            manager: spSalesManager,
            query: "InvoiceLookup",
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "Invoice No" },
                { field: "InvoiceDate", title: "Invoice Date" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InvoiceNo = data.InvoiceNo;
                me.Apply();
            }
        });

    }

    me.InvoiceNoTo = function () {
        var lookup = Wx.blookup({
            name: "InvoiceLookup",
            title: "Invoice",
            manager: spSalesManager,
            query: "InvoiceLookup",
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "Invoice No" },
                { field: "InvoiceDate", title: "Invoice Date" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InvoiceNoTo = data.InvoiceNo;
                me.Apply();
            }
        });

    }

    me.BranchCode = function () {
        var lookup = Wx.blookup({
            name: "BranchLookup4Report",
            title: "Branch",
            manager: spSalesManager,
            query: "BranchLookup4Report",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "CompanyName", title: "Nama Cabang" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCode = data.BranchCode;
                me.data.BranchName = data.CompanyName;
                me.Apply();
            }
        });

    }

    me.BranchCodeTo = function () {
        var lookup = Wx.blookup({
            name: "BranchLookup4Report",
            title: "Branch",
            manager: spSalesManager,
            query: "BranchLookup4Report",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "CompanyName", title: "Nama Cabang" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCodeTo = data.BranchCode;
                me.data.BranchNameTo = data.CompanyName;
                me.Apply();
            }
        });

    }
    me.printPreview = function () {
        $http.post('om.api/ReportSales/ValidatePrintNotaDebet', me.data)
           .success(function (e) {
               if (e.success) {
                   if (me.data.isBranch == false) {
                       //var BranchCode = 'branchcode';
                       //var BranchCodeTo = 'branchcode';
                       var ReportId = "OmRpSalRgs007";
                       var BranchCode = me.data.BranchCode; //untuk sementara
                       var BranchCodeTo = me.data.BranchCodeTo; //untuk sementara
                   }
                   else {
                       var ReportId = "OmRpSalRgs007HQ";
                       var BranchCode = me.data.BranchCode;
                       var BranchCodeTo = me.data.BranchCodeTo;
                   }

                   var par = [
                       'companycode',
                       BranchCode,
                       BranchCodeTo,
                       moment(me.data.DateFrom).format('YYYYMMDD'),
                       moment(me.data.DateTo).format('YYYYMMDD'),
                       me.data.InvoiceNo,
                       me.data.InvoiceNo
                   ]
                   var rparam = 'DARI TANGGAL : ' + moment(me.data.DateFrom).format('DD-MMM-YYYY') + ' S/D ' + moment(me.data.DateTo).format('DD-MMM-YYYY')

                   Wx.showPdfReport({
                       id: ReportId,
                       pparam: par.join(','),
                       textprint: true,
                       rparam: rparam,
                       type: "devex"
                   });
               } else {
                   MsgBox(e.message, MSG_ERROR);
                   return;
               }
           })
           .error(function (e) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
           });

    }

    $("[name = 'isActiveType']").on('change', function () {
        me.data.isActiveType = $('#isActiveType').prop('checked');
        me.data.Type = "";
        me.Apply();
    });

    $("[name = 'isNoInvoice']").on('change', function () {
        me.data.isNoInvoice = $('#isNoInvoice').prop('checked');
        me.data.InvoiceNo = "";
        me.data.InvoiceNoTo = "";
        me.Apply();
    });

    $("[name = 'isBranch']").on('change', function () {
        me.data.isBranch = $('#isBranch').prop('checked');
        me.data.BranchCode = "";
        me.data.BranchName = "";
        me.data.BranchCodeTo = "";
        me.data.BranchNameTo = "";
        me.Apply();
    });

    me.initialize = function () {

        $('#isActiveDate').prop('checked', true);
        me.data.isActiveDate = true;
        $('#isNoInvoice').prop('checked', false);
        me.data.isNoInvoice = false;
        $('#isBranch').prop('checked', false);
        me.data.isBranch = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              //me.data.BranchCode = dl.BranchCode;
          });
        $http.get('breeze/sales/ProfitCenter').
        success(function (dl, status, headers, config) {
            me.data.ProfitCenterCode = dl.ProfitCenter;
        });
        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.DateFrom = e.DateFrom;
              me.data.DateTo = e.DateTo;
          });
        me.Apply();

    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Report Register Nota Debet",
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
                    //{ name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    {
                        text: "Tanggal",
                        type: "controls",
                        items: [
                                { name: 'isActiveDate', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "DateFrom", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                                { name: "DateTo", text: "", cls: "span2", type: "ng-datepicker", disable: "data.isActiveDate == false" },
                        ]
                    },
                    {
                        text: "No Invoice",
                        type: "controls",
                        items: [
                                { name: 'isNoInvoice', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "InvoiceNo", cls: "span2", type: "popup", click: "InvoiceNo()", disable: "data.isNoInvoice == false" },
                                { name: "InvoiceNoTo", cls: "span2", type: "popup", click: "InvoiceNoTo()", disable: "data.isNoInvoice == false" },
                        ]
                    },
                    { name: 'isBranch', type: 'check', cls: "", text: "Cabang", float: 'left' },
                    {
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span2", type: "popup", btnName: "btnBranchCode", click: "BranchCode()", disable: "data.isBranch == false" },
                            { name: "BranchName", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        type: "controls",
                        items: [
                            { name: "BranchCodeTo", cls: "span2", type: "popup", btnName: "btnBranchCodeTo", click: "BranchCodeTo()", disable: "data.isBranch == false" },
                            { name: "BranchNameTo", cls: "span4", readonly: true },
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
        SimDms.Angular("RptRegisterNotaDebet");
    }



});