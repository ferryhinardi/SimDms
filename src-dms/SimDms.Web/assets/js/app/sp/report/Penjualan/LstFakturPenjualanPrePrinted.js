"use strict";
function RptDaftarFktrPenjPreprinted($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.FakturStart = function () {
        var lookup = Wx.blookup({
            name: "FakturLkp",
            title: "No. Faktur Awal",
            manager: spManager,
            query: new breeze.EntityQuery.from("FakturLkp"),
            defaultSort: "FPJNo asc",
            columns: [
                { field: "FPJNo", title: "No. Faktur" },
                { field: "FPJDate", title: "Tgl Faktur", template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD-MM-YYYY') #" },
                { field: "PickingSlipNo", title: "No. Pick. Slip" },
                { field: "PickingSlipDate", title: "Tgl Pick. Slip", template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD-MM-YYYY') #" },
                { field: "InvoiceNo", title: "No. Invoice" },
                { field: "InvoiceDate", title: "Tgl Invoice", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD-MM-YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.FakturStart = data.FPJNo;
                me.data.FakturStartDate = data.FPJDate;
                me.Apply();
            }
        });
    }

    me.FakturTo = function () {
        var lookup = Wx.blookup({
            name: "FakturLkp",
            title: "No. Faktur Akhir",
            manager: spManager,
            query: new breeze.EntityQuery.from("FakturLkp"),
            defaultSort: "FPJNo asc",
            columns: [
                { field: "FPJNo", title: "No. Faktur" },
                { field: "FPJDate", title: "Tgl Faktur", template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD-MM-YYYY') #" },
                { field: "PickingSlipNo", title: "No. Pick. Slip" },
                { field: "PickingSlipDate", title: "Tgl Pick. Slip", template: "#= (PickingSlipDate == undefined) ? '' : moment(PickingSlipDate).format('DD-MM-YYYY') #" },
                { field: "InvoiceNo", title: "No. Invoice" },
                { field: "InvoiceDate", title: "Tgl Invoice", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD-MM-YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.FakturTo = data.FPJNo;
                me.data.FakturToDate = data.FPJDate;
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
            id: 'SpRpTrn043',
            pparam: param.join(','),
            rparam: 'PER TANGGAL : ' + moment(me.data.FakturStartDate).format('DD MMM YYYY') + ' S/D ' + moment(me.data.FakturToDate).format('DD MMM YYYY'),
            type: "devex"
        });

        console.log(param);
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        $http.get('breeze/SparePart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
              me.data.ProductType = dl.ProductType;
              me.data.UserId = dl.UserId;
          });

        me.isPrintAvailable = true;
    }


    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Daftar Faktur Penjualan (Pre-Printed)",
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
                        { name: "ProductType", model: "data.ProductType", text: "Tipe Product", cls: "span4 full", disable: "isPrintAvailable", show: false },
                        { name: "UserId", model: "data.UserId", text: "User ID", cls: "span4 full", disable: "isPrintAvailable", show: false },
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
        SimDms.Angular("RptDaftarFktrPenjPreprinted");

    }



});