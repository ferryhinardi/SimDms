"use strict"; //Reportid OmRpSalesRgs003
function RptRegisterSalesBPK($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.BPKNo = function () {
        var lookup = Wx.blookup({
            name: "BPKLookup",
            title: "BPK",
            manager: spSalesManager,
            query: "BPKLookup",
            defaultSort: "BPKNo asc",
            columns: [
                { field: "BPKNo", title: "BPK No" },
                { field: "BPKDate", title: "BPK Date" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BPKNo = data.BPKNo;
                me.Apply();
            }
        });

    }

    me.BPKNoTo = function () {
        var lookup = Wx.blookup({
            name: "BPKLookup",
            title: "BPK",
            manager: spSalesManager,
            query: "BPKLookup",
            defaultSort: "BPKNo asc",
            columns: [
                { field: "BPKNo", title: "BPK No" },
                { field: "BPKDate", title: "BPK Date" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BPKNoTo = data.BPKNo;
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

    me.printPreview = function () {
        $http.post('om.api/ReportSales/ValidatePrintRegisterBPK', me.data)
                   .success(function (e) {
                       if (e.success) {
                           if (me.data.branch == false) {
                               var ReportId = "OmRpSalesRgs003";
                               var par = [
                                   moment(me.data.DateFrom).format('YYYYMMDD'),
                                   moment(me.data.DateTo).format('YYYYMMDD'),
                                   me.data.Type,
                                   me.data.CustomerCode,
                                   me.data.CustomerCodeTo,
                                   me.data.ModelCodeFrom,
                                   me.data.ModelCodeTo,
                                   me.data.BPKNo,
                                   me.data.BPKNoTo
                               ]
                               var rparam = 'DARI TANGGAL : ' + moment(me.data.DateFrom).format('DD-MMM-YYYY') + ' S/D ' + moment(me.data.DateTo).format('DD-MMM-YYYY')
                           }
                           else {
                               var ReportId = "OmRpSalesRgs003A";
                               var par = [
                                   moment(me.data.DateFrom).format('YYYYMMDD'),
                                   moment(me.data.DateTo).format('YYYYMMDD'),
                                   me.data.Type,
                                   me.data.CustomerCode,
                                   me.data.CustomerCodeTo,
                                   me.data.ModelCodeFrom,
                                   me.data.ModelCodeTo,
                                   me.data.BPKNo,
                                   me.data.BPKNoTo
                               ]
                               var rparam = 'DARI TANGGAL : ' + moment(me.data.DateFrom).format('DD-MMM-YYYY') + ' S/D ' + moment(me.data.DateTo).format('DD-MMM-YYYY')
                           }

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

    $("[name = 'isNoBPK']").on('change', function () {
        me.data.isNoBPK = $('#isNoBPK').prop('checked');
        me.data.BPKNo = "";
        me.data.BPKNoTo = "";
        me.Apply();
    });

    me.initialize = function () {

        $('#isActiveDate').prop('checked', true);
        me.data.isActiveDate = true;
        $('#isActiveType').prop('checked', false);
        me.data.isActiveType = false;
        $('#isActivePelanggan').prop('checked', false);
        me.data.isActivePelanggan = false;
        $('#isActiveModel').prop('checked', false);
        me.data.isActiveModel = false;
        $('#isNoBPK').prop('checked', false);
        me.data.isNoBPK = false;
        $('#branch').prop('checked', false);
        me.data.branch = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
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
        title: "Report Register Bukti Pengiriman Kendaraan",
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
                        text: "Tipe Pelanggan",
                        type: "controls",
                        items: [
                            { name: 'isActiveType', type: 'check', cls: "span1", text: "Status", float: 'left' },
                            { name: "Type", opt_text: "", cls: "span3", type: "select2", text: "", datasource: "Type", disable: "data.isActiveType == false" },

                        ]
                    },
                    {
                        text: "No BPK",
                        type: "controls",
                        items: [
                                { name: 'isNoBPK', type: 'check', cls: "span1", text: "", float: 'left' },
                                { name: "BPKNo", cls: "span2", type: "popup", click: "BPKNo()", disable: "data.isNoBPK == false" },
                                { name: "BPKNoTo", cls: "span2", type: "popup", click: "BPKNoTo()", disable: "data.isNoBPK == false" },
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
                    {
                        text: "Branch",
                        type: "controls",
                        cls: "span3",
                        items: [
                            { name: "branch", cls: "span1", type: "check" },
                            { type: "label", text: "All Brand", cls: "span7 mylabel" },
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
        SimDms.Angular("RptRegisterSalesBPK");
    }



});