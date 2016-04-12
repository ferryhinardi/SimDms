"use strict"; //Reportid OmRpSalesRgs002
function RptRegisterFakturPenjualanUnit($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.ReturnNo = function () {
        var lookup = Wx.blookup({
            name: "SalesReturnLookup",
            title: "Return",
            manager: spSalesManager,
            query: "SalesReturnLookup",
            defaultSort: "ReturnNo asc",
            columns: [
                { field: "ReturnNo", title: "Return No" },
                { field: "ReturnDate", title: "Return Date" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReturnNo = data.ReturnNo;
                me.Apply();
            }
        });

    }

    me.ReturnNoTo = function () {
        var lookup = Wx.blookup({
            name: "SalesReturnLookup",
            title: "Return",
            manager: spSalesManager,
            query: "SalesReturnLookup",
            defaultSort: "ReturnNo asc",
            columns: [
                { field: "ReturnNo", title: "Return No" },
                { field: "ReturnDate", title: "Return Date" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReturnNoTo = data.ReturnNo;
                me.Apply();
            }
        });

    }

    me.CustomerCode = function () {
        var lookup = Wx.blookup({
            name: "CustomerLookup4Report",
            title: "Pelanggan",
            manager: spSalesManager,
            query: "CustomerLookup4Report",
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Kode Pelanggan" },
                { field: "CustomerName", title: "Nama Pelanggan" },
                { field: "Address1", title: "Alamat 1" },
                { field: "Address2", title: "Alamat 2" },
                { field: "Address3", title: "Alamat 3" },
                { field: "ProfitCenter", title: "Profit Center" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
                me.Apply();
            }
        });

    }

    me.CustomerCodeTo = function () {
        var lookup = Wx.blookup({
            name: "CustomerLookup4Report",
            title: "Pelanggan",
            manager: spSalesManager,
            query: "CustomerLookup4Report",
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Kode Pelanggan" },
                { field: "CustomerName", title: "Nama Pelanggan" },
                { field: "Address1", title: "Alamat 1" },
                { field: "Address2", title: "Alamat 2" },
                { field: "Address3", title: "Alamat 3" },
                { field: "ProfitCenter", title: "Profit Center" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCodeTo = data.CustomerCode;
                me.data.CustomerNameTo = data.CustomerName;
                me.Apply();
            }
        });

    }

    me.ModelCodeFrom = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Model",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model" },
                { field: "SalesModelDesc", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ModelCodeFrom = data.SalesModelCode;
                me.data.ModelNameFrom = data.SalesModelDesc;
                me.Apply();
            }
        });

    }

    me.ModelCodeTo = function () {
        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Model",
            manager: spSalesManager,
            query: "SalesModelCodeLookup",
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model" },
                { field: "SalesModelDesc", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ModelCodeTo = data.SalesModelCode;
                me.data.ModelNameTo = data.SalesModelDesc;
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
                me.Apply();
            }
        });

    }

    me.printPreview = function () {
        $http.post('om.api/ReportSales/ValidatePrintRegisterReturn', me.data)
                   .success(function (e) {
                       if (e.success) {
                           var param = "";
                           if (me.data.isActiveDate == true && me.data.isActivePelanggan == true && me.data.isActiveModel == true && me.data.isNoReturn == true)
                           { param = "1"; }
                           else if (me.data.isActiveDate == true && me.data.isActivePelanggan == true && me.data.isActiveModel == true && me.data.isNoReturn == false)
                           { param = "2"; }
                           else if (me.data.isActiveDate == true && me.data.isActivePelanggan == true && me.data.isActiveModel == false && me.data.isNoReturn == true)
                           { param = "3"; }
                           else if (me.data.isActiveDate == true && me.data.isActivePelanggan == true && me.data.isActiveModel == false && me.data.isNoReturn == false)
                           { param = "4"; }
                           else if (me.data.isActiveDate == true && me.data.isActivePelanggan == false && me.data.isActiveModel == true && me.data.isNoReturn == true)
                           { param = "5"; }
                           else if (me.data.isActiveDate == true && me.data.isActivePelanggan == false && me.data.isActiveModel == true && me.data.isNoReturn == false)
                           { param = "6"; }
                           else if (me.data.isActiveDate == true && me.data.isActivePelanggan == false && me.data.isActiveModel == false && me.data.isNoReturn == true)
                           { param = "7"; }
                           else if (me.data.isActiveDate == true && me.data.isActivePelanggan == false && me.data.isActiveModel == false && me.data.isNoReturn == false)
                           { param = "8"; }
                           else if (me.data.isActiveDate == false && me.data.isActivePelanggan == true && me.data.isActiveModel == true && me.data.isNoReturn == true)
                           { param = "9"; }
                           else if (me.data.isActiveDate == false && me.data.isActivePelanggan == true && me.data.isActiveModel == true && me.data.isNoReturn == false)
                           { param = "10"; }
                           else if (me.data.isActiveDate == false && me.data.isActivePelanggan == true && me.data.isActiveModel == false && me.data.isNoReturn == true)
                           { param = "11"; }
                           else if (me.data.isActiveDate == false && me.data.isActivePelanggan == true && me.data.isActiveModel == false && me.data.isNoReturn == false)
                           { param = "12"; }
                           else if (me.data.isActiveDate == false && me.data.isActivePelanggan == false && me.data.isActiveModel == true && me.data.isNoReturn == true)
                           { param = "13"; }
                           else if (me.data.isActiveDate == false && me.data.isActivePelanggan == false && me.data.isActiveModel == true && me.data.isNoReturn == false)
                           { param = "14"; }
                           else if (me.data.isActiveDate == false && me.data.isActivePelanggan == false && me.data.isActiveModel == false && me.data.isNoReturn == true)
                           { param = "15"; }
                           else if (me.data.isActiveDate == false && me.data.isActivePelanggan == false && me.data.isActiveModel == false && me.data.isNoReturn == false)
                           { param = "16"; }

                           if (me.data.isBranch == false) {
                               //var BranchCode = 'branchcode';
                               //var BranchCodeTo = 'branchcode';
                               var BranchCode = me.data.BranchCode; //untuk sementara
                               var BranchCodeTo = me.data.BranchCodeTo; //untuk sementara
                           }
                           else {
                               var BranchCode = me.data.BranchCode;
                               var BranchCodeTo = me.data.BranchCodeTo;
                           }

                           var ReportId = 'OmRpSalRgs005';
                           var par = [
                               'companycode',
                               BranchCode,
                               BranchCodeTo,
                               moment(me.data.DateFrom).format('YYYYMMDD'),
                               moment(me.data.DateTo).format('YYYYMMDD'),
                               me.data.Type,
                               me.data.CustomerCode,
                               me.data.CustomerCodeTo,
                               me.data.ModelCodeFrom,
                               me.data.ModelCodeTo,
                               me.data.InvoiceNo,
                               me.data.InvoiceNoTo,
                               param,
                               '0'
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

    me.Type = [
      { "value": '0', "text": 'WHOLE SALES' },
      { "value": '1', "text": 'DIRECT' }
    ];

    $("[name = 'isActiveType']").on('change', function () {
        me.data.isActiveType = $('#isActiveType').prop('checked');
        me.data.Type = "";
        me.Apply();
    });

    $("[name = 'isActivePelanggan']").on('change', function () {
        me.data.isActivePelanggan = $('#isActivePelanggan').prop('checked');
        me.data.CustomerCode = "";
        me.data.CustomerName = "";
        me.data.CustomerCodeTo = "";
        me.data.CustomerNameTo = "";
        me.Apply();
    });

    $("[name = 'isActiveModel']").on('change', function () {
        me.data.isActiveModel = $('#isActiveModel').prop('checked');
        me.data.ModelCodeFrom = "";
        me.data.ModelNameFrom = "";
        me.data.ModelCodeTo = "";
        me.data.ModelNameTo = "";
        me.Apply();
    });

    $("[name = 'isNoReturn']").on('change', function () {
        me.data.isNoReturn = $('#isNoReturn').prop('checked');
        me.data.ReturnNo = "";
        me.data.ReturnNoTo = "";
        me.Apply();
    });

    $("[name = 'isBranch']").on('change', function () {
        me.data.isBranch = $('#isBranch').prop('checked');
        me.data.BranchCode = "";
        me.data.BranchCodeTo = "";
        me.Apply();
    });

    me.initialize = function () {

        if ((new Date(Date.now()).getMonth() + 1) >= 1 || (new Date(Date.now()).getMonth() + 1) <= 6) {
            me.data.DateFrom = 7 + '/' + 1 + '/' + (new Date().getFullYear() - 1);
            me.data.DateTo = 12 + '/' + 31 + '/' + (new Date().getFullYear() - 1);
        }
        else {
            me.data.DateFrom = 1 + '/' + 1 + '/' + (new Date().getFullYear());
            me.data.DateTo = 6 + '/' + 30 + '/' + (new Date().getFullYear());
        }
        var date = new Date();
        me.data.DateFrom = new Date(date.getFullYear(), date.getMonth(), 1);
        me.data.DateTo = new Date(date.getFullYear(), date.getMonth() + 1, 0);

        $('#isActiveDate').prop('checked', true);
        me.data.isActiveDate = true;
        $('#isActiveType').prop('checked', false);
        me.data.isActiveType = false;
        $('#isActivePelanggan').prop('checked', false);
        me.data.isActivePelanggan = false;
        $('#isActiveModel').prop('checked', false);
        me.data.isActiveModel = false;
        $('#isNoReturn').prop('checked', false);
        me.data.isNoReturn = false;
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

    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Report Register Return Invoice",
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
                        text: "Cabang",
                        type: "controls",
                        items: [
                                { name: 'isBranch', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "BranchCode", cls: "span2", type: "popup", btnName: "btnBranchCode", click: "BranchCode()", disable: "data.isBranch == false" },
                                { name: "BranchCodeTo", cls: "span2", type: "popup", btnName: "btnBranchCodeTo", click: "BranchCodeTo()", disable: "data.isBranch == false" },
                        ]
                    },
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
                        text: "Tipe Pelanggan",
                        type: "controls",
                        items: [
                            { name: 'isActiveType', type: 'check', cls: "span1", text: "Status", float: 'left' },
                            { name: "Type", opt_text: "", cls: "span3", type: "select2", text: "", datasource: "Type", disable: "data.isActiveType == false" },

                        ]
                    },
                    {
                        text: "No. Return",
                        type: "controls",
                        items: [
                                { name: 'isNoReturn', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "ReturnNo", cls: "span2", type: "popup", click: "ReturnNo()", disable: "data.isNoReturn == false" },
                                { name: "ReturnNoTo", cls: "span2", type: "popup", click: "ReturnNoTo()", disable: "data.isNoReturn == false" },
                        ]
                    },
                    { name: 'isActivePelanggan', type: 'check', cls: "", text: "Pelanggan", float: 'left' },
                    {
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", type: "popup", btnName: "btnCustomerCode", click: "CustomerCode()", disable: "data.isActivePelanggan == false" },
                            { name: "CustomerName", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        type: "controls",
                        items: [
                            { name: "CustomerCodeTo", cls: "span2", type: "popup", btnName: "btnCustomerCodeTo", click: "CustomerCodeTo()", disable: "data.isActivePelanggan == false" },
                            { name: "CustomerNameTo", cls: "span4", readonly: true },
                        ]
                    },
                    { name: 'isActiveModel', type: 'check', cls: "", text: "Model", float: 'left' },
                    {
                        type: "controls",
                        items: [
                            { name: "ModelCodeFrom", cls: "span2", type: "popup", click: "ModelCodeFrom()", disable: "data.isActiveModel == false" },
                            { name: "ModelNameFrom", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        type: "controls",
                        items: [
                            { name: "ModelCodeTo", cls: "span2", type: "popup", click: "ModelCodeTo()", disable: "data.isActiveModel == false" },
                            { name: "ModelNameTo", cls: "span4", readonly: true },
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
        SimDms.Angular("RptRegisterFakturPenjualanUnit");
    }



});