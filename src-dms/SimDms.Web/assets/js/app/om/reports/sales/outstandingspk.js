"use strict";
function RptTandaTerimaFakturPengjPengurusanBnn($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.SupplierCode = function () {
        var lookup = Wx.blookup({
            name: "OutstandingSpkSpl4Report",
            title: "Pemasok",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('OutstandingSpkSpl4Report'),
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.Apply();
            }
        });
        console.log(data);
    }

    me.printPreview = function () {
        

        if ($('#chkSupplier').prop('checked') == true) {
            if ($('#SupplierCode').val() != '') {
                if ($('#chkSpkDate').prop('checked') == true && $('#chkSupplier').prop('checked') == false) {
                    var param = [
                        moment(me.data.StnkDate).format('DD MMM YYYY'),
                        "",
                        moment(me.data.SpkDateFrom).format('DD MMM YYYY'),
                        moment(me.data.SpkDateTo).format('DD MMM YYYY')
                    ];
                }
                else if ($('#chkSpkDate').prop('checked') == false && $('#chkSupplier').prop('checked') == true) {
                    var param = [
                        moment(me.data.StnkDate).format('DD MMM YYYY'),
                        me.data.SupplierCode,
                        "1900-01-01",
                        "1900-01-01"
                    ];
                }
                else if ($('#chkSpkDate').prop('checked') == true && $('#chkSupplier').prop('checked') == true) {
                    var param = [
                        moment(me.data.StnkDate).format('DD MMM YYYY'),
                        me.data.SupplierCode,
                        moment(me.data.SpkDateFrom).format('DD MMM YYYY'),
                        moment(me.data.SpkDateTo).format('DD MMM YYYY')
                    ];
                }
                else {
                    var param = [
                        moment(me.data.StnkDate).format('DD MMM YYYY'),
                        "",
                        "1900-01-01",
                        "1900-01-01"
                    ];
                }
            }
            else {
                MsgBox('Silahkan isi terlebih dahulu kode Biro Jasa yang ingin ditampilkan', MSG_ERROR);
            }
        }
        else {
            if ($('#chkSpkDate').prop('checked') == true && $('#chkSupplier').prop('checked') == false) {
                var param = [
                        moment(me.data.StnkDate).format('DD MMM YYYY'),
                        "",
                        moment(me.data.SpkDateFrom).format('DD MMM YYYY'),
                        moment(me.data.SpkDateTo).format('DD MMM YYYY')
                ];
            }
            else if ($('#chkSpkDate').prop('checked') == false && $('#chkSupplier').prop('checked') == true) {
                var param = [
                        moment(me.data.StnkDate).format('DD MMM YYYY'),
                        me.data.SupplierCode,
                        "1900-01-01",
                        "1900-01-01"
                ];
            }
            else if ($('#chkSpkDate').prop('checked') == true && $('#chkSupplier').prop('checked') == true) {
                var param = [
                        moment(me.data.StnkDate).format('DD MMM YYYY'),
                        me.data.SupplierCode,
                        moment(me.data.SpkDateFrom).format('DD MMM YYYY'),
                        moment(me.data.SpkDateTo).format('DD MMM YYYY')
                ];
            }
            else {
                var param = [
                        moment(me.data.StnkDate).format('DD MMM YYYY'),
                        me.data.SupplierCode,
                        "1900-01-01",
                        "1900-01-01"
                ];
            }
        }

        Wx.showPdfReport({
            id: "OmRpSalRgs031",
            pparam: param.join(','),
            textprint: true,
            rparam: "Print Laporan Outstanding per Biro Jasa",
            type: "devex"
        });
        

    }    

    me.initialize = function () {
        me.data = {};
        me.change = false;
        me.data.StnkDate = me.now();
        me.data.SpkDateFrom = me.now();
        me.data.SpkDateTo = me.now();
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
        title: "Report Outstanding SPK",
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
                            text: "Tanggal STNK Diterima",
                            type: "controls",
                            cls: "span4 full",
                            items: [
                                { name: "StnkDate", model: "data.StnkDate", placeHolder: "Tgl. STNK Awal", cls: "span4", type: 'ng-datepicker' },
                            ]
                        },
                        
                        {
                            text: "Tgl. Trm Biro Jasa (SPK)",
                            type: "controls",
                            cls: "span6 full",
                            items: [
                                { name: "chkSpkDate", model: "data.chkSpkDate", text: "Check Supplier", cls: "span1", type: "ng-check" },
                                { name: "SpkDateFrom", model: "data.SpkDateFrom", placeHolder: "Spk Awal", cls: "span3", type: 'ng-datepicker', disable: "!data.chkSpkDate" },
                                { name: "SpkDateTo", model: "data.SpkDateTo", placeHolder: "Spk Akhir", cls: "span3", type: 'ng-datepicker', disable: "!data.chkSpkDate" },
                            ]
                        },

                        {
                            text: "Biro Jasa",
                            type: "controls",
                            items: [

                                { name: "chkSupplier", model: "data.chkSupplier", text: "Check Supplier", cls: "span1", type: "ng-check" },
                                { name: "SupplierCode", model: "data.SupplierCode", cls: "span2", placeHolder: " ", readonly: true, type: "popup", click: "SupplierCode()", disable: "!data.chkSupplier" },
                                { name: "SupplierName", model: "data.SupplierName", cls: "span5", placeHolder: " ", readonly: true, disable: "!data.chkSupplier" }
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