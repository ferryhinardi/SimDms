"use strict";

function RptSummaryPermohonanFakPol($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.SalesModelFrom = function () {
        var lookup = Wx.blookup({
            name: "SummaryPermohonanFakPolSM4Report",
            title: "Sales Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('SummaryPermohonanFakPolSM4Report'),
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model" },
                { field: "SalesModelDesc", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelFrom = data.SalesModelCode;
                me.data.SalesModelFromDesc = data.SalesModelDesc;
                me.Apply();
            }
        });
        console.log(data);
    }

    me.SalesModelTo = function () {
        var lookup = Wx.blookup({
            name: "SummaryPermohonanFakPolSM4Report",
            title: "Sales Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('SummaryPermohonanFakPolSM4Report'),
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Model" },
                { field: "SalesModelDesc", title: "Keterangan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesModelTo = data.SalesModelCode;
                me.data.SalesModelToDesc = data.SalesModelDesc;
                me.Apply();
            }
        });
    }

    me.CustomerFrom = function () {
        var lookup = Wx.blookup({
            name: "SummaryPermohonanFakPolC4Report",
            title: "Customer",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('SummaryPermohonanFakPolC4Report'),
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Kode Penjual" },
                { field: "CustomerName", title: "Nama Penjual" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerFrom = data.CustomerCode;
                me.data.CustomerFromDesc = data.CustomerName;
                me.Apply();
            }
        });
    }

    me.CustomerTo = function () {
        var lookup = Wx.blookup({
            name: "SummaryPermohonanFakPolC4Report",
            title: "Customer",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('SummaryPermohonanFakPolC4Report'),
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Kode Penjual" },
                { field: "CustomerName", title: "Nama Penjual" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerTo = data.CustomerCode;
                me.data.CustomerToDesc = data.CustomerName;
                me.Apply();
            }
        });
    }

    me.printPreview = function () {
        var prm = '';
        var paramdesc = '';
        if (me.data.chkDate == true) {
            prm = '1';
            paramdesc = 'PER TANGGAL : ' + moment(me.data.FromReqDate).format('DD MMM YYYY') + ' S/D ' + moment(me.data.ToReqDate).format('DD MMM YYYY');
        }
        else {
            prm = '0';
            paramdesc = 'Show All';
        }

        var param = [
                    moment(me.data.FromReqDate).format('YYYYMMDD'),
                    moment(me.data.ToReqDate).format('YYYYMMDD'),
                    me.data.SalesModelFrom,
                    me.data.SalesModelTo,
                    me.data.CustomerFrom,
                    me.data.CustomerTo,
                    prm
        ];

        Wx.showPdfReport({
            id: 'OmRpSalRgs014',
            pparam: param.join(','),
            textprint: true,
            rparam: paramdesc,
            type: "devex"
        });
    }

    $("[name = 'chkSalesModel']").on('change', function () {
        me.data.isActive = $('#chkSalesModel').prop('checked');
        me.data.SalesModelFrom = "";
        me.data.SalesModelFromDesc = "";
        me.data.SalesModelTo = "";
        me.data.SalesModelToDesc = "";
        me.Apply();
    });

    $("[name = 'chkCustomer']").on('change', function () {
        me.data.isActive = $('#chkCustomer').prop('checked');
        me.data.CustomerFrom = "";
        me.data.CustomerFromDesc = "";
        me.data.CustomerTo = "";
        me.data.CustomerToDesc = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data = {};
        me.currentDate = me.now();
        me.data.chkDate = false;
        me.data.checkReg = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });

        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.FromReqDate = e.DateFrom;
              me.data.ToReqDate = e.DateTo;
          });

        me.isPrintAvailable = true;
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Summary Permohonan Faktur Polisi",
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
                            text: "Tanggal Req",
                            type: "controls",
                            cls: "span6",
                            items: [
                                { name: "chkDate", model: "data.chkDate", cls: "span1", type: "ng-check" },
                                { name: "FromReqDate", model: "data.FromReqDate", placeHolder: "Tgl. Awal", cls: "span3", type: 'ng-datepicker', disable: "!data.chkDate" },
                                { name: "ToReqDate", model: "data.ToReqDate", placeHolder: "Tgl. Akhir", cls: "span3", type: 'ng-datepicker', disable: "!data.chkDate" },
                            ]
                        },
                        { name: "chkSalesModel", model: "data.chkSalesModel", text: "Sales Model", cls: "span4 full", type: "ng-check" },
                        {
                            text: "",
                            type: "controls",
                            items: [
                                

                                { name: "SalesModelFrom", model: "data.SalesModelFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "SalesModelFrom()", disable: "!data.chkSalesModel" },
                                { name: "SalesModelFromDesc", model: "data.SalesModelFromDesc", cls: "span6", placeHolder: " ", readonly: true, disable: "!data.chkSalesModel" }
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [


                                { name: "SalesModelTo", model: "data.SalesModelTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "SalesModelTo()", disable: "!data.chkSalesModel" },
                                { name: "SalesModelToDesc", model: "data.SalesModelToDesc", cls: "span6", placeHolder: " ", readonly: true, disable: "!data.chkSalesModel" }
                            ]
                        },
                        { name: "chkCustomer", model: "data.chkCustomer", text: "Penjual", cls: "span4 full", type: "ng-check" },
                        {
                            text: "",
                            type: "controls",
                            items: [


                                { name: "CustomerFrom", model: "data.CustomerFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "CustomerFrom()", disable: "!data.chkCustomer" },
                                { name: "CustomerFromDesc", model: "data.CustomerFromDesc", cls: "span6", placeHolder: " ", readonly: true, disable: "!data.chkCustomer" }
                            ]
                        },
                        {
                            text: "",
                            type: "controls",
                            items: [


                                { name: "CustomerTo", model: "data.CustomerTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "CustomerTo()", disable: "!data.chkCustomer" },
                                { name: "CustomerToDesc", model: "data.CustomerToDesc", cls: "span6", placeHolder: " ", readonly: true, disable: "!data.chkCustomer" }
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
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("RptSummaryPermohonanFakPol");

    }
});