﻿"use strict"; //Reportid OmRpSalesRgs001
function RptRegisterHarianPenerbitan($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('tax.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.Month = data;
    });

    me.UnitUsaha = [
        { "value": '000', "text": 'GENERAL' },
        { "value": '100', "text": 'UNIT' },
        { "value": '200', "text": 'SERVICE' },
        { "value": '300', "text": 'SPAREPART' }
    ];

    me.SupplierCode = function () {
        if (me.data.UnitUsaha == '') {
            me.data.UnitUsaha = '%';
        }
        else {
            me.data.UnitUsaha;
        }
        var lookup = Wx.blookup({
            name: "Supplier4Tax",
            title: "Supplier",
            manager: TaxManager,
            query: new breeze.EntityQuery().from("Supplier4Tax").withParameters({ Year: me.data.Year, Month: me.data.Month, UnitUsaha: me.data.UnitUsaha}),
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Kode Supplier" },
                { field: "SupplierName", title: "Nama Supplier" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.Apply();
            }
        });
    };

    me.Proses = function () {
        if (me.data.isCheck1 == false && me.data.isCheck2 == false && me.data.isCheck3 == false) {
            MsgBox("Silahkan centang pilihan terlebih dahulu, kemudian tekan tombol proses !!!", MSG_INFO)
        } else {
            if (me.data.isCheck1 == true && me.data.isCheck2 == true && me.data.isCheck3 == true) {
                //alert('Upload Data Dalam Rekontruksi');
                me.printUploadData(); me.printPajakMasukan(); me.printBukuPembelian();
            }
            else if (me.data.isCheck1 == true && me.data.isCheck2 == true && me.data.isCheck3 == false) {
                //alert('Upload Data Dalam Rekontruksi'); 
                me.printUploadData(); me.printPajakMasukan();
            }
            else if (me.data.isCheck1 == true && me.data.isCheck2 == false && me.data.isCheck3 == false) {
                //alert('Upload Data Dalam Rekontruksi');
                me.printUploadData();
            }
            else if (me.data.isCheck1 == false && me.data.isCheck2 == true && me.data.isCheck3 == true) {
                me.printPajakMasukan(); me.printBukuPembelian();
            }
            else if (me.data.isCheck1 == false && me.data.isCheck2 == true && me.data.isCheck3 == false) {
                me.printPajakMasukan();
            }
            else if (me.data.isCheck1 == true && me.data.isCheck2 == false && me.data.isCheck3 == true) {
                //alert('Upload Data Dalam Rekontruksi');
                me.printUploadData(); me.printBukuPembelian();
            }
            else if (me.data.isCheck1 == false && me.data.isCheck2 == false && me.data.isCheck3 == true) {
                me.printBukuPembelian();
            }
        }
    }

    me.printUploadData = function () {
        var CompanyCode = $('[name="CompanyCode"]').val();
        var BranchCode = $('[name="BranchCode"]').val();
        var Year = $('[name="Year"]').val();
        var Month = $('[name="Month"]').val();
        $http.post('tax.api/Report/GetData4CSVFileNew?CompanyCode=' + CompanyCode + '&BranchCode=' + BranchCode + '&Year=' + Year + '&Month=' + Month)
          .success(function (dt, status, headers, config) {
              if (dt.success) {
                  MsgBox("Import pajak masukan sukses.", MSG_INFO);
                  var url = "tax.api/Report/GetData4XLSFileNew?";
                  var params = "&CompanyCode=" + CompanyCode;
                  params += "&BranchCode=" + BranchCode;
                  params += "&Year=" + Year;
                  params += "&Month=" + Month;
                  url = url + params;
                  window.location = url;
              } else {
                  MsgBox("Data Tidak Ada!!!", MSG_INFO)
              }
          })
          .error(function (e, status, header, config) {
              MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO)
          });
        
    }

    me.printPajakMasukan = function () {
        var ReportId = 'GnRpTaxTrn003';
        var par = [
                       'producttype',
                       me.data.Month,
                       me.data.Year,
                       1
        ]
        var rparam = 'PERIODE : ' + moment(Date.now()).format('DD-MMMM-YYYY');

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.printBukuPembelian = function () {
        if (me.data.isSupplier == true && me.data.SupplierCode == '') {
            MsgBox('Filed pemasok tidak boleh kosong', MSG_ERROR);
        }
        else {
            var filter = ''
            var profitCenter = ''

            if (me.data.isSupplier == false) { filter = '%'; }
            else { filter = me.data.SupplierCode; }

            if (me.data.UnitUsaha == '') { profitCenter = '%'; }
            else { profitCenter = me.data.UnitUsaha; }

            var order = '-1'
            if (me.data.SortBy == '0') { order = '0'; }
            else if (me.data.SortBy == '1') { order = '1'; }
            else { order = '1'; }

            $http.post('tax.api/report/PrintBukuPembelian', me.data)
                        .success(function (e) {
                            if (e.success) {
                                if (e.isBranch == false) { var isBranch = '0' } else { var isBranch = '1' }
                                if (e.isMultiBranch == false) { var isMultiBranch = '0' } else { var isMultiBranch = '1' }
                                if (e.isFpjCentral == false) { var isFpjCentral = '0' } else { var isFpjCentral = '1' }

                                var ReportId = 'GnRpTaxTrn004';
                                var par = [
                                               'producttype', me.data.Year, me.data.Month, profitCenter, filter, 1, order, isBranch, isMultiBranch, isFpjCentral, 1
                                ]
                                var rparam = 'PERIODE : ' + moment(Date.now()).format('DD-MMMM-YYYY');

                                Wx.showPdfReport({
                                    id: ReportId,
                                    pparam: par.join(','),
                                    rparam: rparam,
                                    type: "devex"
                                });
                            } else {
                                MsgBox(e.message, MSG_ERROR);
                            }
                        })
                        .error(function (e) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                        });
        }
    }

    $("[name = 'isCheck1']").on('change', function () {
        me.data.isCheck1 = $('#isCheck1').prop('checked');
        me.Apply();
    });

    $("[name = 'isCheck2']").on('change', function () {
        me.data.isCheck2 = $('#isCheck2').prop('checked');

        me.Apply();
    });

    $("[name = 'isCheck3']").on('change', function () {
        me.data.isCheck3 = $('#isCheck3').prop('checked');
        me.data.UnitUsaha = "";
        $('#isSupplier').prop('checked', false);
        me.data.isSupplier = false;
        me.data.SupplierCode = "";
        me.data.SupplierName = "";
        me.Apply();
    });

    $("[name = 'isSupplier']").on('change', function () {
        me.data.isSupplier = $('#isSupplier').prop('checked');
        me.data.SupplierCode = "";
        me.data.SupplierName = "";
        me.Apply();
    });

    me.initialize = function () {
        me.data.SortBy = "1";
        me.data.Year = new Date(Date.now()).getFullYear();
        me.data.Month = new Date(Date.now()).getMonth()+1;
        $('#Year').css("text-align", "right");

        $('#isCheck1').prop('checked', false);
        me.data.isCheck1 = false;
        $('#isCheck2').prop('checked', false);
        me.data.isCheck2 = false;
        $('#isCheck3').prop('checked', false);
        me.data.isCheck3 = false;
        $('#isSupplier').prop('checked', false);
        me.data.isSupplier = false;

        $http.get('breeze/sales/CurrentUserInfo').
              success(function (dl, status, headers, config) {
                  me.data.CompanyCode = dl.CompanyCode;
                  me.data.BranchCode = dl.BranchCode;
              });
        $http.get('breeze/sales/ProfitCenter').
        success(function (dl, status, headers, config) {
            me.data.ProfitCenterCode = dl.ProfitCenter;
        });
        me.Apply();
    }

    me.start();
    
}

$(document).ready(function () {
    var options = {
        title: "Report Pajak Masukan eSPT 1111",
        xtype: "panels",
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
                        type: "controls",
                        cls: "span4 full",
                        items: [
                            { name: "isCheck1", cls: "span1", type: "check" },
                            { type: "label", text: "Upload Data To eSPT PPN 1111B", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        items: [
                            { name: "isCheck2", cls: "span1", type: "check" },
                            { type: "label", text: "Pajak Masukan", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        items: [
                            { name: "isCheck3", cls: "span1", type: "check" },
                            { type: "label", text: "Buku Pembelian", cls: "span7 mylabel" },
                        ]
                    },
                    { name: "UnitUsaha", text: "Unit Usaha",  opt_text:'[SELECT ALL]',cls: "span3", type: "select2", datasource: "UnitUsaha", show: "data.isCheck3 == true" },
                    {
                        text: "Pemasok",
                        type: "controls",
                        show: "data.isCheck3 == true",
                        items: [
                            { name: "isSupplier", cls: "span1", type: "check" },
                            { name: "SupplierCode", cls: "span2", type: "popup", click: "SupplierCode()", disable: "data.isSupplier == false" },
                            { name: "SupplierName", cls: "span4", readonly: true},
                        ]
                    },
                    {
                        type: "optionbuttons",
                        name: "SortBy",
                        model: "data.SortBy",
                        text: "Sort By",
                        show: "data.isCheck3 == true",
                        items: [
                            { name: "0", text: "Tgl. Pajak" },
                            { name: "1", text: "No. Pajak" },
                            { name: "2", text: "Nama Pemasok" },
                        ]
                    },
                    {
                        type: "div", cls: "span8"
                    },
                    {
                        type: "buttons", cls: "span2", items: [
                             { name: "Proses", text: "   Proses", icon: "icon-search", click: "Proses()", cls: "button small btn btn-success" },
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