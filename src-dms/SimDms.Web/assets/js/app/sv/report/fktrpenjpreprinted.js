"use strict";
function RptFakturPenjualanPreprinted($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.FakturStart = function () {
        var lookup = Wx.blookup({
            name: "SelectLookupInvoice",
            title: "No. Faktur Awal",
            manager: svServiceManager,
            query: new breeze.EntityQuery.from("SelectLookupInvoice"),
            defaultSort: "InvoiceNo Desc",
            columns: [
                { field: "InvoiceNo", title: "No Faktur" },
                {
                    field: "InvoiceDate", title: "Tanggal Faktur",
                    template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.FakturStart = data.InvoiceNo;
                me.data.FakturStartDate = data.InvoiceDate;
                me.data.FakturTo = data.InvoiceNo;
                me.data.FakturToDate = data.InvoiceDate;
                me.Apply();
            }
        });
    }

    me.FakturTo = function () {
        var lookup = Wx.blookup({
            name: "SelectLookupInvoice",
            title: "No. Faktur Akhir",
            manager: svServiceManager,
            query: new breeze.EntityQuery.from("SelectLookupInvoice"),
            defaultSort: "InvoiceNo Desc",
            columns: [
                { field: "InvoiceNo", title: "No Faktur" },
                {
                    field: "InvoiceDate", title: "Tanggal Faktur",
                    template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #"
                },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.FakturTo = data.InvoiceNo;
                me.data.FakturToDate = data.InvoiceDate;
                me.Apply();
            }
        });
    }

    me.printPreview = function () {
        if ($('#FakturStart').val() == '' || $('#FakturTo').val() == '') {
            MsgBox('Ada data yang belum lengkap', MSG_ERROR);
            return;
        }
        if (me.data.FakturStartDate > me.data.FakturToDate) {
            MsgBox('No. Faktur awal tidak boleh lebih besar dari No. Faktur akhir', MSG_ERROR);
            return;
        }

        var param = [
            me.data.ProductType,
            me.data.FakturStart,
            me.data.FakturTo,
            me.data.UserId

        ];

        Wx.showPdfReport({
            id: 'SvRpTrn020',
            pparam: param.join(','),
            rparam: 'PER TANGGAL : ' + moment(me.data.FakturStartDate).format('DD MMM YYYY') + ' S/D ' + moment(me.data.FakturToDate).format('DD MMM YYYY'),
            type: "devex"
        });

        console.log(param);
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        $http.get('breeze/svtransaksi/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
              me.data.UserId = dl.UserId;
              me.data.ProductType = dl.ProductType;
          });

        me.isPrintAvailable = true;
    }


    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Faktur Penjualan (Pre-Printed)",
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
                        { name: "UserId", model: "data.UserId", text: "UserId", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "ProductType", model: "data.ProductType", text: "UserId", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        {
                            text: "No Faktur",
                            type: "controls",
                            items: [


                                { name: "FakturStart", model: "data.FakturStart", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "FakturStart()" },
                                { name: "FakturStartDate", model: "data.FakturStartDate", cls: "span6", placeHolder: " ", readonly: true, show: false }
                            ]
                        },
                        {
                            text: "S/D",
                            type: "controls",
                            items: [


                                { name: "FakturTo", model: "data.FakturTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "FakturTo()" },
                                { name: "FakturToDate", model: "data.FakturToDate", cls: "span6", placeHolder: " ", readonly: true, show: false }
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
        SimDms.Angular("RptFakturPenjualanPreprinted");

    }
});