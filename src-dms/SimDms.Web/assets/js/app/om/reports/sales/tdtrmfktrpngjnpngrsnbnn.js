"use strict";
function RptTandaTerimaFakturPengjPengurusanBnn($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Suppliers = function () {
        var lookup = Wx.blookup({
            name: "DaftarTdTerimaFakturBnnSpl4Report",
            title: "Biro Jasa",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('DaftarTdTerimaFakturBnnSpl4Report'),
            columns: [
                { field: "SupplierCode", title: "Kode Biro Jasa" },
                { field: "SupplierName", title: "Nama Biro Jasa" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCd = data.BranchCode;
                me.data.BranchNm = data.BranchName;
                me.Apply();
            }
        });
        console.log(data);
    }

    me.printPreview = function () {

        if ($('#chkSupplier').prop('checked') == true) {
            var param = [
                    'companycode',
                    me.data.Suppliers,
                    moment(me.data.PeriodeFrom).format('DD MMM YYYY'),
                    moment(me.data.PeriodeTo).format('DD MMM YYYY')
            ];
            var reportId = "OmRpSalRgs032";
        }
        else {
            me.data.Suppliers = "[ SELECT ALL ]";
            var param = [
                    'companycode',
                    me.data.Suppliers="",
                    moment(me.data.PeriodeFrom).format('DD MMM YYYY'),
                    moment(me.data.PeriodeTo).format('DD MMM YYYY')
            ];
            var reportId = "OmRpSalRgs032";
        }

        Wx.showPdfReport({
            id: reportId,
            pparam: param.join(','),
            textprint: true,
            rparam: "Print Tanda Terima Faktur / Pengajuan Pengurusan BBN",
            type: "devex"
        });

    }

    $("[name = 'chkSupplier']").on('change', function () {
        if ($('#chkSupplier').prop('checked') == true) {
            me.data.Suppliers = "";
        }
        else {
            me.data.Suppliers = "[ SELECT ALL ]";
        }

        me.Apply();
    });

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.Suppliers = "[ SELECT ALL ]";
        me.data.PeriodeFrom = me.now();
        me.data.PeriodeTo = me.now();
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
          });

        me.isPrintAvailable = true;

    }


    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Tanda Terima Faktur / Pengajuan Pengurusan BBN",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span2 full", disable: "isPrintAvailable", show: false },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span3 full", disable: "isPrintAvailable", show: false },
                        
                        {
                            text: "Biro Jasa",
                            type: "controls",
                            items: [

                                { name: "chkSupplier", model: "data.chkSupplier", text: "Check Supplier", cls: "span1", type: "ng-check" },
                                { name: "Suppliers", model: "data.Suppliers", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "Suppliers()", disable: "!data.chkSupplier" },
                            ]
                        },
                        {
                            text: "Periode",
                            type: "controls",
                            cls: "span4 full",
                            items: [
                                { name: "PeriodeFrom", model: "data.PeriodeFrom", placeHolder: "Periode Awal", cls: "span4", type: 'ng-datepicker' },
                            ]
                        },
                        {
                            text: "S/D",
                            type: "controls",
                            cls: "span4",
                            items: [
                                { name: "PeriodeTo", model: "data.PeriodeTo", placeHolder: "Periode Akhir", cls: "span4", type: 'ng-datepicker' },
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
        SimDms.Angular("RptTandaTerimaFakturPengjPengurusanBnn");

    }
});