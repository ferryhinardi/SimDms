"use strict"; //Reportid OmRpSalesRgs001
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
                me.data.BranchCode1 = data.BranchCode;
                me.data.BranchName1 = data.CompanyName;
                me.Apply();
            }
        });

    }

    me.Proses = function () {
        if (me.data.isCheck1 == false && me.data.isCheck2 == false && me.data.isCheck3 == false) {
            MsgBox("Silahkan centang pilihan terlebih dahulu, kemudian tekan tombol proses !!!", MSG_INFO)
        } else {
            if (me.data.isCheck1 == true && me.data.isCheck2 == true && me.data.isCheck3 == true) {
                me.printUploadData(); me.printPajakKeluaran(); me.printBukuPembelian();
            }
            else if (me.data.isCheck1 == true && me.data.isCheck2 == true && me.data.isCheck3 == false) {
                me.printUploadData(); me.printPajakKeluaran();
            }
            else if (me.data.isCheck1 == true && me.data.isCheck2 == false && me.data.isCheck3 == false) {
                me.printUploadData();
            }
            else if (me.data.isCheck1 == false && me.data.isCheck2 == true && me.data.isCheck3 == true) {
                me.printPajakKeluaran(); me.printBukuPembelian();
            }
            else if (me.data.isCheck1 == false && me.data.isCheck2 == true && me.data.isCheck3 == false) {
                me.printPajakKeluaran();
            }
            else if (me.data.isCheck1 == true && me.data.isCheck2 == false && me.data.isCheck3 == true) {
                me.printUploadData(); me.printBukuPembelian();
            }
            else if (me.data.isCheck1 == false && me.data.isCheck2 == false && me.data.isCheck3 == true) {
                me.printBukuPembelian();
            }
        }
    }

    me.printUploadData = function () {
        var CompanyCode = $('[name="CompanyCode"]').val();
        var BranchCode1 = $('[name="BranchCode1"]').val();
        var Year = $('[name="Year"]').val();
        var Month = $('[name="Month"]').val();
        $http.post('tax.api/Report/GetData4CSVFileNewOut?CompanyCode=' + CompanyCode + '&BranchCode1=' + BranchCode1 + '&Year=' + Year + '&Month=' + Month )
          .success(function (dt, status, headers, config) {
              if (dt.success) {
                  MsgBox("Import Pajak Keluaran sukses.", MSG_INFO);
                  var url = "tax.api/Report/GetData4XLSFileNewOut?";
                  var params = "&CompanyCode=" + $('[name="CompanyCode"]').val();
                  params += "&BranchCode1=" + $('[name="BranchCode1"]').val();
                  params += "&Year=" + $('[name="Year"]').val();
                  params += "&Month=" + $('[name="Month"]').val();
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

    me.printPajakKeluaran = function () {
        var ReportId = 'GnRpTaxTrn001';
        var par = [
                       'producttype', me.data.Month, me.data.Year, 0
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
        if ((me.data.isSupplier == true && me.data.SupplierCode == '') || (me.data.isBranch == true && me.data.BranchCode1 == '')) {
            MsgBox('Ada Data Yang Masih Kosong', MSG_ERROR);
        }
        else {
            var isMultiBranch = localStorage.getItem('isMultiBranch');
            var isBranch = localStorage.getItem('isBranch');
            var isFpjCentral = localStorage.getItem('isFpjCentral');
            var CekAllowedBranchCode = localStorage.getItem('CekAllowedBranchCode');

            var profitCenter = ''
            if (me.data.UnitUsaha == '') { profitCenter = '%'; }
            else { profitCenter = me.data.UnitUsaha; }

            var filter = ''
            if (me.data.isSupplier == false) { filter = '%'; }
            else { filter = me.data.SupplierCode; }

            var order = '-1'
            if (me.data.SortBy == '0') { order = '0'; }
            else if (me.data.SortBy == '1') { order = '1'; }
            else { order = '1'; }

            var allBranch = ''
            if (isBranch == true) { allBranch = '0'; }
            else {
                if (me.data.isBranch == true) { allBranch = '0'; }
                else { allBranch = '1'; }
            }

            if (CekAllowedBranchCode == "1") {
                var ReportId = 'GnRpTaxTrn002A';
            } else {
                var ReportId = 'GnRpTaxTrn002';
            }
            var par = [
                          'companycode', me.data.BranchCode1, 'producttype', me.data.Year, me.data.Month, profitCenter, filter, 1, order, isMultiBranch, isBranch, isFpjCentral, allBranch
            ]
            var rparam = 'PERIODE : ' + moment(Date.now()).format('DD-MMMM-YYYY');

            Wx.showPdfReport({
                id: ReportId,
                pparam: par.join(','),
                rparam: rparam,
                type: "devex"
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

    $("[name = 'isBranch']").on('change', function () {
        me.data.isBranch = $('#isBranch').prop('checked');
        me.data.BranchCode1 = "";
        me.data.BranchName1 = "";
        me.Apply();
    });

    function SetBranchInterface() {
        $http.post('tax.api/report/SetBranchInterface', me.data)
                        .success(function (e) {
                            if (e.success) {
                                localStorage.setItem('isBranch', e.isBranch);
                                localStorage.setItem('isMultiBranch', e.isMultiBranch);
                                localStorage.setItem('isFpjCentral', e.isFpjCentral);
                                localStorage.setItem('CekAllowedBranchCode', e.CekAllowedBranchCode);

                                if (e.isMultiBranch == true) {
                                    if (e.isBranch == true) {
                                        $('#isBranch').attr('disabled', 'disabled');
                                        me.data.BranchCode1 = e.Branch.BranchCode;
                                        me.data.BranchName1 = e.Branch.BranchName;
                                    }
                                } else {
                                    $('#isBranch').attr('disabled', 'disabled');
                                    me.data.BranchCode1 = e.Branch[0].BranchCode;
                                    me.data.BranchName1 = e.Branch[0].BranchName;
                                }
                            } else {
                                MsgBox(e.message, MSG_ERROR);
                            }
                        })
                        .error(function (e) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                        });
    }

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
        $('#isBranch').prop('checked', false);
        me.data.isBranch = false;

        SetBranchInterface();

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
        title: "Report Pajak Keluaran eSPT 1111",
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
                            { type: "label", text: "Upload Data To eSPT PPN 1111A", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        items: [
                            { name: "isCheck2", cls: "span1", type: "check" },
                            { type: "label", text: "Pajak Keluaran", cls: "span7 mylabel" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span3 full",
                        items: [
                            { name: "isCheck3", cls: "span1", type: "check" },
                            { type: "label", text: "Buku Penjualan", cls: "span7 mylabel" },
                        ]
                    },
                    { name: "UnitUsaha", text: "Unit Usaha", cls: "span3", opt_text:'[SELECT ALL]',type: "select2", datasource: "UnitUsaha", show: "data.isCheck3 == true" },
                    {
                        text: "Cabang",
                        type: "controls",
                        show: "data.isCheck3 == true",
                        items: [
                            { name: "isBranch", cls: "span1", type: "check" },
                            { name: "BranchCode1", cls: "span2", type: "popup", click: "BranchCode()", disable: "data.isBranch == false" },
                            { name: "BranchName1", cls: "span4", readonly: true },
                        ]
                    },
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