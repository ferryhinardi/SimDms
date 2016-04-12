"use strict";

function RptDaftarCancelFktrPenj($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.InvoiceFrom = function () {
        var lookup = Wx.blookup({
            name: "FakturSrvLkp",
            title: "No. Invoice Awal",
            manager: spManager,
            query: new breeze.EntityQuery.from("FakturSrvLkp"),
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "No. Invoice" },
                { field: "InvoiceDate", title: "Tgl Invoice", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD-MM-YYYY') #" },
                { field: "ReferenceNo", title: "No. Reference" },
                { field: "ReferenceDate", title: "Tgl Reference", template: "#= (ReferenceDate == undefined) ? '' : moment(ReferenceDate).format('DD-MM-YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InvoiceFrom = data.InvoiceNo;
                me.data.InvoiceFromDate = data.InvoiceDate;
                me.Apply();
            }
        });
    }

    me.InvoiceTo = function () {
        var lookup = Wx.blookup({
            name: "FakturSrvLkp",
            title: "No. Invoice Akhir",
            manager: spManager,
            query: new breeze.EntityQuery.from("FakturSrvLkp"),
            defaultSort: "InvoiceNo asc",
            columns: [
                { field: "InvoiceNo", title: "No. Invoice" },
                { field: "InvoiceDate", title: "Tgl Invoice", template: "#= (InvoiceDate == undefined) ? '' : moment(InvoiceDate).format('DD-MM-YYYY') #" },
                { field: "ReferenceNo", title: "No. Reference" },
                { field: "ReferenceDate", title: "Tgl Reference", template: "#= (ReferenceDate == undefined) ? '' : moment(ReferenceDate).format('DD-MM-YYYY') #" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InvoiceTo = data.InvoiceNo;
                me.data.InvoiceToDate = data.InvoiceDate;
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
            me.data.FakturStart,
            me.data.FakturTo,
            me.data.ProfitCenter
        ];

        Wx.showPdfReport({
            id: 'SpRpTrn012A',
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
              me.data.ProfitCenter = dl.ProfitCenter;
          });

        me.isPrintAvailable = true;
    }


    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Daftar Cancel Faktur Service",
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
                    { name: "ProfitCenter", model: "data.ProfitCenter", text: "Profit Center", cls: "span4 full", disable: "isPrintAvailable", show: false },
                    {
                        text: "No Invoice",
                        type: "controls",
                        items: [


                            { name: "InvoiceFrom", model: "data.InvoiceFrom", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "InvoiceFrom()" },
                            { name: "InvoiceFromDate", model: "data.InvoiceFromDate", cls: "span6", placeHolder: " ", readonly: true, show: false }
                        ]
                    },
                    {
                        text: "S/D",
                        type: "controls",
                        items: [


                            { name: "InvoiceTo", model: "data.InvoiceTo", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "InvoiceTo()" },
                            { name: "InvoiceToDate", model: "data.InvoiceToDate", cls: "span6", placeHolder: " ", readonly: true, show: false }
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
        SimDms.Angular("RptDaftarCancelFktrPenj");
    }



});