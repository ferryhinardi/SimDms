"use strict";

var today;

function spHPPController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "browse",
            title: "Entry HPP",
            manager: spPenerimaanManager,
            query: "EntryHPPBrowse",
            defaultSort: "HPPNo desc",
            columns: [
                { field: "HPPNo", title: "No. HPP" },
                { field: "HPPDate", title: "Tgl. HPP", type: "date", format: "{0:dd-MMM-yyyy}" },
                { field: "WRSNo", title: "No. WRS" },
                { field: "WRSDate", title: "Tgl. WRS", type: "date", format: "{0:dd-MMM-yyyy}" },
                { field: "ReferenceNo", title: "Reference No" },
                { field: "DNSupplierNo", title: "DN No" },
                { field: "TaxNo", title: "Tax No" },
                { field: "TaxDate", title: "Tax Date", type: "date", format: "{0:dd-MMM-yyyy}" },
                { field: "SupplierName", title: "Supplier Name" },
                { field: "StatusStr", title: "Status" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data = data;
                me.detail = data;
                me.data.tot = data["TotHPPAmt"];
                me.data.Status = data["Status"];
                me.lookupAfterSelect(data);
                me.isLoadData = me.hasChanged = true;
                me.isInitialize = false;
                me.Apply();
                me.startEditing();
            }
        });
    }
    
    me.LoadWRSDetail = function (WRSNo) {
        var src = "sp.api/EntryClaimSupplier/WRSNoDetail?WRSNo=" + WRSNo;
        $http.post(src)
            .success(function (data, status, headers, config) {
                me.detail = data.WRSDet;
                me.data.WRSNo = data.WRSDet["WRSNo"];
                //me.data.Status = data.WRSDet.Status;
                me.data.ReferenceNo = data.WRSDet["ReferenceNo"];
                me.data.DNSupplierNo = data.WRSDet["DNSupplierNo"];
                me.data.WRSDate = data.WRSDet["WRSDate"];
                me.data.SupplierName = data.SuppDet["SupplierName"];
                me.data.TotNetPurchAmt = data.WRSDet["TotWRSAmt"];
                var angka = data.WRSDet["TotWRSAmt"].toString();
                me.data.TotTaxAmt = angka.substring(0, angka.length - 1);
                me.data.TotHPPAmt = data.WRSDet["TotWRSAmt"] + parseInt(angka.substring(0, angka.length - 1));
                me.data.tot = data.WRSDet["TotWRSAmt"] + parseInt(angka.substring(0, angka.length - 1));
                me.ReformatNumber();
            }
         );
        
    }

    me.LoadData = function (HPPNo) {
        var src = "sp.api/EntryHPP/loadData?HPPNo=" + HPPNo;
        $http.post(src)
            .success(function (data, status, headers, config) {
                me.data = data;
                me.detail = data;
                me.data.tot = data["TotHPPAmt"];
                me.data.Status = data["Status"];
            }
         );
    }
    
    me.onWrsBlur = function () {
        var val = $(this).val();
        if (val == '') {
            me.WRSNoBrowse();
            me.data.WRSNo = '';
            $(this).val('');
        }
        else {
            $http.post('sp.api/EntryHPP/getWrsByNo', { wrsNO: val })
            .success(function (data, status, headers, config) {
                if (data != undefined) {
                    if (data.length > 0) {
                        $http.post('sp.api/EntryHPP/getHppByWrs', { wrsNO: data[0].WRSNo })
                        .success(function (result, status, headers, config) {
                            if (result != undefined) {
                                if (result.length > 0) {
                                   me.data = result[0];
                                    me.detail = result[0];
                                    me.data.tot = result[0].TotHPPAmt;
                                    me.data.Status = result[0].Status;
                                    me.lookupAfterSelect(result[0]);
                                    me.isLoadData = me.hasChanged = true;
                                    me.isInitialize = false;
                                    me.$apply;
                                    me.startEditing();
                                }
                                else {
                                    me.WRSNoBrowse();
                                    me.data.WRSNo = '';
                                    $(this).val('');
                                }
                            }
                            else {
                                me.WRSNoBrowse();
                                me.data.WRSNo = '';
                                $(this).val('');
                            }
                        });
                    }
                    else {
                        me.WRSNoBrowse();
                        me.data.WRSNo = '';
                        $(this).val('');
                    }
                }
                else {
                    me.WRSNoBrowse();
                    me.data.WRSNo = '';
                    $(this).val('');
                }
            });
        }
    };

    me.WRSNoBrowse = function () {
        var lookup = Wx.blookup({
            name: "btnWRSNo",
            title: "WRS Lookup",
            manager: spPenerimaanManager,
            query: "WRSHppBrowse",
            defaultSort: "WRSNo DESC",
            columns: [
                { field: "WRSNo", title: "WRS No" },
                { field: "WRSDate", title: "WRS Date", type: "date", format: "{0:dd-MMM-yyyy}" },
                { field: "ReferenceNo", title: "Reference No" },
                { field: "DNSupplierNo", title: "DN No" },
                { field: "SupplierName", title: "Supplier Name" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                $http.post('sp.api/EntryHPP/getHppByWrs', { wrsNO: data.WRSNo })
               .success(function (result, status, headers, config) {
                   if (result != undefined) {
                       if (result.length > 0) {
                           me.data = result[0];
                           me.detail = result[0];
                           me.data.tot = result[0].TotHPPAmt;
                           me.data.Status = result[0].Status;
                           me.lookupAfterSelect(result[0]);
                           me.isLoadData = me.hasChanged = true;
                           me.isInitialize = false;
                           me.$apply;
                           me.startEditing();
                       }
                       else {
                           data.HPPNo = me.data.HPPNo;
                           data.TaxNo = me.data.TaxNo;
                           data.TaxDate = me.data.TaxDate;
                           data.DueDate = me.data.DueDate;
                           data.YearTax = me.data.YearTax;
                           data.MonthTax = me.data.MonthTax;
                           data.DiffNetPurchAmt = 0;
                           data.DiffTaxAmt = 0;

                           me.lookupAfterSelect(data);
                           me.detail = data;
                           me.LoadWRSDetail(data.WRSNo);
                           me.isSave = true;
                           me.data.HPPDate = today;

                           me.$apply;
                       }
                   }
                   else {
                       me.lookupAfterSelect(data);
                       me.detail = data;
                       me.LoadWRSDetail(data.WRSNo);
                       me.isSave = true;
                       me.$apply;
                   }
               });
            }
        });
    }

    $http.post('sp.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.Month = data;
    });

    $http.post('sp.api/Combo/YearsOld').
    success(function (data, status, headers, config) {
        me.Years = data;
    });

    $http.post('sp.api/EntryHPP/Default').
   success(function (data, status, headers, config) {
       today = data.Today;
   });

    me.calc = function () {
        var jum = parseInt(me.data.DiffNetPurchAmt) + parseInt(me.data.DiffTaxAmt) + parseInt(me.data.TotTaxAmt) + parseInt(me.data.TotNetPurchAmt);
        me.data.TotHPPAmt = isNaN(jum) ? 0 : jum;
        console.log(me.data.TotHPPAmt);
        //me.ReformatNumber();
        me.$apply;
    }

    me.saveData = function (e, param) {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        if (me.data.TotTaxAmt == 0) {
            if (me.data.DiffTaxAmt != 0) {
                MsgBox("Selisih nilai PPn tidak dapat di input, nilai PPn Pembelian tidak ada", MSG_ERROR);
                return false;
            }
        }
        if (me.data.TotNetPurchAmt == 0) {
            if (me.data.DiffNetPurchAmt != 0) {
                MsgBox("Selisih nilai DPP tidak dapat di input, nilai DPP Pembelian tidak ada", MSG_ERROR);
                return false;
            }
        }
        if (parseInt(me.data.DiffTaxAmt) > parseInt(me.data.TotTaxAmt)) {
            MsgBox("Selisih nilai PPn harus lebih kecil atau sama dengan nilai PPn" + me.data.TotTaxAmt, MSG_ERROR);
            return false;
        }
        if (parseInt(me.data.DiffNetPurchAmt) > parseInt(me.data.TotNetPurchAmt)) {
            MsgBox("Selisih nilai DPP harus lebih kecil atau sama dengan nilai DPP", MSG_ERROR);
            return false;
        }
        if (me.data.TaxDate == "") {
            MsgBox("Silahkan isi Tgl. Faktur Pajak.", MSG_ERROR);
            return false;
        }
        if (me.data.MonthTax == "") {
            MsgBox("Silahkan pilih Bulan.", MSG_ERROR);
            return false;
        }
        if (me.data.YearTax == "") {
            MsgBox("Silahkan pilih Tahun.", MSG_ERROR);
            return false;
        }

        $http.post('sp.api/EntryHPP/save', me.savemodel).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.data.HPPNo = data.clm;
                    me.LoadData(data.clm);
                    // me.lookupAfterSelect(data.clm);
                    me.isSave = false;
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.CloseHpp = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        MsgConfirm("Are you sure to close?", function (result) {
            if (result) {
                $http.post('sp.api/EntryHPP/CloseHPP', me.savemodel).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        //me.init();
                        MsgBox("No HPP : "+dl.data["HPPNo"]+" Berhasil di close")
                        //Wx.Info("Record has been closed...");
                        me.LoadData(dl.data["HPPNo"]);
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.printPreview = function () {
        me.savemodel = angular.copy(me.data);
        angular.extend(me.savemodel, me.detail);
        if (me.data.Status != 2) {
            $http.post('sp.api/EntryHPP/Print', me.savemodel).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.data.Status = data.status;

                        var data = me.data.HPPNo + "," + me.data.HPPNo + "," + "profitcenter" + "," + "typeofgoods";
                        var rparam = "admin";
                        console.log(data);
                        Wx.showPdfReport({
                            id: "SpRpTrn026",
                            pparam: data,
                            rparam: rparam,
                            textprint: true,
                            type: "devex"
                        });

                        me.isLoadData = me.hasChanged = true;
                        me.isInitialize = false;
                        me.$apply;
                        me.startEditing();

                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        } else {
            var data = me.data.HPPNo + "," + me.data.HPPNo + "," + "profitcenter" + "," + "typeofgoods";
            var rparam = "admin";

            Wx.showPdfReport({
                id: "SpRpTrn026",
                pparam: data,
                rparam: rparam,
                textprint: true,
                type: "devex"
            });
        }
    };

    me.initialize = function () {
        var date = new Date;
        me.data.MonthTax = date.getMonth() + 1;
        me.data.YearTax = date.getFullYear();
        me.detail = {};
        me.data.HPPNo = "HPP/XX/XXXXXX";
        me.data.HPPDate = me.now();
        me.data.WRSDate = me.now();
        me.detail.BinningDate = me.now();
        me.data.TaxDate = me.now();
        me.data.ReferenceDate = me.now();
        me.data.DueDate = me.now();
        me.data.DiffNetPurchAmt = 0;
        me.data.DiffTaxAmt = 0;
        me.data.tot = 0;
        me.isPrintAvailable = true;
        me.data.WRSNo = "";
        me.data.TotTaxAmt = 0;
        $('#WRSNo').removeAttr('readonly');

        $('#WRSNo').unbind('blur');
        $('#WRSNo').on('blur', me.onWrsBlur);
    }

    me.$watch('data.Status', function (nVal, oVal) {
        if (nVal == 2) {
            me.hasChanged = true;
            me.isSave = false;
            me.isLoadData = false;
            me.isPrintAvailable = false;
            me.isInitialize = false;
        }
        else {
            me.isPrintAvailable = true;
            me.isLoadData = true;
            me.hasChanged = true;
            me.isSave = true;
            me.isInitialize = false;
        }
    });
    
    me.$watch('data.DiffNetPurchAmt', function (newVal, oldVal) {
        if (newVal == '') {
            me.data.DiffNetPurchAmt = 0;
        }
        me.calc();
    });

    me.$watch('data.DiffTaxAmt', function (newVal, oldVal) {
        if (newVal == '') {
            me.data.DiffTaxAmt = 0;
        }
        me.calc();
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Entry HPP",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                    { name: "Status", show: "data2.e = 0" },
                        {
                            type: "buttons",
                            items: [
                                    { name: "btnCloseHpp1", text: "Closing HPP", cls: "btn btn-warning", click: "CloseHpp()", show: "data.Status != undefined", disable: "data.Status == 2 || data.Status == 0" },
                                    { name: "btnCloseHpp2", text: "CLOSED", cls: "btn btn-danger", click: "CloseHpp()", show: "data.Status == 2 ", disable: "detail5.PartNo === undefined" },
                                    { name: "btnCloseHpp3", text: "PRINTED", cls: "btn btn-info", show: "data.Status == 1", disable: "data.Status == 1" },
                                    { name: "btnCloseHpp4", text: "OPEN", cls: "btn btn-success", click: "CloseHpp()", show: "data.Status == 0", disable: "detail5.PartNo === undefined" }
                            ]
                        },{type:"div"},
                        {
                            name: "HPPNo",
                            text: "HPP No",
                            cls: "span4",
                            disable: "data.Status == 2",
                            readonly: true
                        },
                        {
                            name: "HPPDate",
                            text: "HPP Date",
                            cls: "span4",
                            type: "ng-datepicker",
                            disable: "data.Status == 2"
                        }, 
                        {
                            name: "WRSNo",
                            text: "WRS No",
                            cls: "span4  ",
                            type: "popup",
                            btnName: "btnWRSNo",
                            click: "WRSNoBrowse()",
                            validasi: "required",
                            disable: "data.Status != undefined",
                            readonly: true,
                        },
                        {
                            name: "WRSDate",
                            text: "WRS Date",
                            type: "ng-datepicker",
                            cls: "span4  ",
                            readonly: true,
                            disable: "data.Status == 2"
                        }, 
                        {
                            name: "SupplierCode",
                            text: "SupplierCode",
                            cls: "span3  ",
                            model:"detail.SupplierCode",
                            readonly: true,
                            disable: "data.Status == 2"
                        },
                        {
                            name: "SupplierName",
                            text: "Supplier Name",
                            cls: "span5  ",
                            readonly: true,
                            disable: "data.Status == 2"
                        }, 
                        {
                            name: "BinningNo",
                            text: "No. BN",
                            cls: "span4  ",
                            model: "detail.BinningNo",
                            readonly: true,
                            disable: "data.Status == 2"
                        },
                        {
                            name: "BinningDate",
                            text: "Tgl. BN",
                            type: "ng-datepicker",
                            cls: "span4  ",
                            model: "detail.BinningDate",
                            readonly: true,
                            disable: "data.Status == 2"
                        },
                        {
                            name: "TotNetPurchAmt",
                            text: "Nilai DPP",
                            cls: "span4 number-int",
                            readonly: true,
                            disable: "data.Status == 2"
                        },
                        {
                            name: "TotTaxAmt",
                            text: "Nilai PPN",
                            cls: "span4 number-int",
                            readonly: true,
                            disable: "data.Status == 2"
                        }, { type: "hr" },
                        {
                            name: "TaxNo",
                            text: "No. Faktur Pajak",
                            cls: "span4 maskedit",
                            type: "ng-maskedit",
                            mask: "###.###-##.########",
                            validasi: "required",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "TaxDate",
                            text: "Tgl. Faktur Pajak",
                            cls: "span4",
                            type: "ng-datepicker",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "ReferenceNo",
                            text: "No. Reff/F. Penj",
                            cls: "span4",
                            validasi: "required",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "ReferenceDate",
                            text: "Tgl. Referensi",
                            cls: "span4",
                            type: "ng-datepicker",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "DNSupplierNo",
                            text: "No. DN",
                            cls: "span4",
                            readonly: true,
                            disable: "data.Status == 2"
                        },
                        {
                            name: "DueDate",
                            text: "Tgl. Jatuh Tempo",
                            cls: "span4",
                            type: "ng-datepicker",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "MonthTax",
                            text: "Bulan",
                            opt_text: "-- Pilih Bulan --",
                            cls: "span4",
                            type: "select2",
                            datasource: "Month",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "YearTax",
                            text: "Tahun",
                            opt_text: "-- Pilih Tahun --",
                            cls: "span4",
                            type: "select2",
                            datasource: "Years",
                            disable: "data.Status == 2"
                        }, { type: "hr" },
                        {
                            name: "DiffNetPurchAmt",
                            text: "Nilai DPP",
                            model: "data.DiffNetPurchAmt",
                            //type: 'int',
                            cls: "span2 number",
                            //style: "text-align:right",
                            //format: "",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "DiffTaxAmt",
                            //type: 'int',
                            text: "Nilai PPN",
                            model: "data.DiffTaxAmt",
                            cls: "span2 number",
                            //style: "text-align:right",
                            disable: "data.Status == 2"
                        },
                        {
                            name: "TotHPPAmt",
                            text: "Total",
                            cls: "number-int span4",
                            readonly: true,
                            disable: "data.Status == 2"
                        },
                        {
                            name: "tot",
                            text: "Total",
                            show: "data.f == 0",
                            readonly: true,
                            disable: "data.Status == 2"
                        },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spHPPController");
    }
});