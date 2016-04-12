"use strict";

function gnMasterCustomersController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('gn.api/Combo/CustomerGenders').
    success(function (data, status, headers, config) {
        me.comboGender = data;
    });

    $http.post('gn.api/Combo/CustomerTypes').
    success(function (data, status, headers, config) {
        me.comboCustomerType = data;
    });
    $http.post('gn.api/Combo/CustomerStatuses').
    success(function (data, status, headers, config) {
        me.comboStatus = data;
    });
    $http.post('gn.api/Combo/PaymentTypes').
    success(function (data, status, headers, config) {
        me.comboPaymentCode = data;
    });

    me.carabayar = function (IsShowedAll) {
        $http.post('gn.api/Combo/TOP?IsShowedAll=' + IsShowedAll).
        success(function (data, status, headers, config) {
            me.comboTOPCode = data;
        });
    }

    me.comboSalesType = [{ value: "0", text: "Whole Sales" },
                     { value: "1", text: "Direct Sales" }];

    //me.browse = function () {
    //    var lookup = Wx.blookup({
    //        name: "CustomersBrowse",
    //        title: "Customers Browse",
    //        manager: gnManager,
    //        //query: "CustomerBrowse",
    //        query: "CustBrowse",
    //        defaultSort: "CustomerCode asc",
    //        columns: [
    //        { field: "CustomerCode", title: "Customer Code" },
    //        { field: "CustomerName", title: "Customer Name" },
    //        //{ field: "AddressGab", title: "Address" },
    //        { field: "Address1", title: "Address" },
    //        ]
    //    });
    //    lookup.dblClick(function (data) {
    //        if (data != null) {
    //            me.lookupAfterSelect(data);
    //            me.GetCustomerInfo(data.CustomerCode);
    //            me.isSave = false;
    //            me.Apply();
    //        }
    //    });
    //}

    me.browse = function () {

        var lookup = Wx.klookup({
            name: "CustomersBrowse",
            title: "Customers Browse",
            url: "gn.api/Customer/CustBrowse?cols=" + 3,
            serverBinding: true,
            pageSize: 10,
            columns: [
            { field: "CustomerCode", title: "Customer Code" },
            { field: "CustomerName", title: "Customer Name" },
            //{ field: "AddressGab", title: "Address" },
            { field: "Address1", title: "Address" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.GetCustomerInfo(data.CustomerCode);
                me.data.PosName = data.KelurahanDesa;
                me.isSave = false;
                $('#StandardCode').removeAttr("disabled");
                //me.disableFiil();
                me.Apply();
            }
        });
                
    }

    $('#CategoryCode').on('blur', function () {
        if (me.data.CategoryCode == null || me.data.CategoryCode == '') return;
        $http.post('gn.api/Customer/CategoryCode', me.data).success(function (data) {
            if (data.success) {
                me.data.CategoryCode = data.data.LookUpValue;
                me.data.CategoryName = data.data.LookUpValueName;
            }
            else {
                me.data.CategoryCode = me.data.CategoryName = '';
                me.CategoryCode();
            }
        });
    });



    me.GetCustomerInfo = function (CustomerCode) {
        //me.initialize();
        var src = "gn.api/Customer/CheckCustomer?CustomerCode=" + CustomerCode;
        $http.post(src)
            .success(function (v, status, headers, config) {
                if (v.success) {
                    //var param = { CategoryName: v.CategoryName };
                    //angular.extend(me.data, v.data, param);
                    //me.data = v.data;
                    me.grid.pcmodel = v.CustomerProfitCenters;
                    me.loadTableData(me.grid1, me.grid.pcmodel);

                    me.grid.pdmodel = v.CustomerDiscs;
                    me.loadTableData(me.grid2, me.grid.pdmodel);

                    me.grid.bkmodel = v.CustomerBanks;
                    me.loadTableData(me.grid3, me.grid.bkmodel);
                }
            }).error(function (e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
            });
    }

    me.GetCustomerInfoFromTrx = function (CustomerCode) {
        //me.initialize();
        var src = "gn.api/Customer/CheckCustomer?CustomerCode=" + CustomerCode;
        $http.post(src)
            .success(function (v, status, headers, config) {
                if (v.success) {
                    //var param = { CategoryName: v.CategoryName };
                    //angular.extend(me.data, v.data, param);
                    me.data = v.data;
                    me.grid.pcmodel = v.CustomerProfitCenters;
                    me.loadTableData(me.grid1, me.grid.pcmodel);

                    me.grid.pdmodel = v.CustomerDiscs;
                    me.loadTableData(me.grid2, me.grid.pdmodel);

                    me.grid.bkmodel = v.CustomerBanks;
                    me.loadTableData(me.grid3, me.grid.bkmodel);
                }
            }).error(function (e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
            });
    }

    me.CategoryCode = function () {
        var lookup = Wx.blookup({
            name: "CategoryCodeLookup",
            title: "Lookup CategoryCode",
            manager: gnManager,
            query: "CustomerCategories",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Category Code" },
                { field: "LookUpValueName", title: "Category Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.CategoryCode = data.LookUpValue;
                me.data.CategoryName = data.LookUpValueName;
                me.Apply();
            }
        }
        );
    };

    $('#ZipNo').on('blur', function () {
        if (me.data.ZipNo == null || me.data.ZipNo == '') return;
        $http.post('gn.api/Customer/ZipCodes', me.data).success(function (data) {
            if (data.success) {
                me.data.ZipNo = data.data.ZipCode;
                me.data.PosName = data.data.KelurahanDesa;
            }
            else {
                me.data.ZipNo = me.data.PosName = '';
                me.PosCode();
            }
        });
    });

    me.PosCode = function () {
        var lookup = Wx.klookup({
            name: "PosCodeLookup",
            title: "Lookup PosCode",
            url: "gn.api/grid/ZipCodes",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { field: 'ZipCode', dir: 'asc' }
            ],
            columns: [
                { field: "IbuKota", title: "Province" },
                { field: "KotaKabupaten", title: "City" },
                { field: "KecamatanDistrik", title: "Area" },
                { field: "ZipCode", title: "Pos Code" },
                { field: "KelurahanDesa", title: "Pos Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.ZipNo = data.ZipCode;
                me.data.PosName = data.KelurahanDesa;
                me.data.IbuKota = data.IbuKota;
                me.data.CityCode = data.CityCode;
                me.data.AreaCode = data.AreaCode;
                me.data.KelurahanDesa = data.KelurahanDesa;
                me.data.KecamatanDistrik = data.KecamatanDistrik;
                me.data.KotaKabupaten = data.KotaKabupaten;
                //me.data.IbuKota = data.IbuKota;
                me.Apply();
            }
        });
    };



    me.ProfitCenterCode = function () {
        if (me.data.ProfitCenterCode) {
            me.pcmodel = {};
            me.pcmodel.TaxCode = "PPN";
            me.pcmodel.TaxDesc = "PAJAK PERTAMBAHAN NILAI";
            me.pcmodel.TaxTransCode = "01";
            me.pcmodel.TaxTransDesc = "Pemungut Selain Pajak";
        }
        var lookup = Wx.blookup({
            name: "ProfitCenterCodeLookup",
            title: "Lookup ProfitCenterCode",
            manager: gnManager,
            query: "ProfitCenters",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "ProfitCenter Code" },
                { field: "LookUpValueName", title: "ProfitCenter Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pcmodel.ProfitCenterCode = data.LookUpValue;
                me.pcmodel.ProfitCenterName = data.LookUpValueName;
                if (data.LookUpValue == '100') {
                    $('#GroupPrice').attr("style", "background-color: rgb(255, 218, 204)");
                    $('#btnGroupPrice').removeAttr("style");
                    $('#s2id_SalesType').attr("style", "background-color: rgb(255, 218, 204)");
                } else {
                    $('#GroupPrice').removeAttr("style");
                    $('#s2id_SalesType').removeAttr("style");
                }
                me.detailprofit(me.data.CustomerCode, data.LookUpValue);
                me.Apply();
            }
        });
    };

    me.ProfitCenterCodeDisc = function () {
        var lookup = Wx.blookup({
            name: "ProfitCenterCodeDiscLookup",
            title: "Lookup ProfitCenterCodeDisc",
            manager: gnManager,
            query: "ProfitCenters",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "ProfitCenter Code" },
                { field: "LookUpValueName", title: "ProfitCenter Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;

                me.pdmodel.ProfitCenterCode = data.LookUpValue;
                me.pdmodel.ProfitCenterName = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    $('#CustomerClass').on('blur', function () {
        if (me.pcmodel.CustomerClass == null || me.pcmodel.CustomerClass == '') return;
        $http.post('gn.api/CustomerProfitCenter/CustomerClasses', me.pcmodel).success(function (data) {
            if (data.success) {
                me.pcmodel.CustomerClasses = data.data.CustomerClasses;
                me.pcmodel.CustomerClassName = data.data.CustomerClassName;
            }
            else {
                me.pcmodel.CustomerClasses = me.pcmodel.CustomerClassName = '';
                me.CustomerClass();
            }
        });
    });

    me.CustomerClass = function () {
        if (me.pcmodel.ProfitCenterCode != null) {
            var lookup = Wx.blookup({
                name: "CustomerClassLookup",
                title: "Lookup CustomerClass",
                manager: gnManager,
                query: new breeze.EntityQuery().from("CustomerClasses").withParameters({ profitCenterCode: me.pcmodel.ProfitCenterCode }),
                defaultSort: "CustomerClass asc",
                columns: [
                    { field: "CustomerClass", title: "CustomerClass" },
                    { field: "CustomerClassName", title: "CustomerClass Name" }
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.isSave = false;
                    me.pcmodel.CustomerClass = data.CustomerClass;
                    me.pcmodel.CustomerClassName = data.CustomerClassName;
                    me.Apply();
                }
            });
        }
    };

    me.TaxCode = function () {
        var lookup = Wx.blookup({
            name: "TaxCodeLookup",
            title: "Lookup TaxCode",
            manager: gnManager,
            query: "Taxes",
            defaultSort: "TaxCode asc",
            columns: [
                { field: "TaxCode", title: "Tax Code" },
                { field: "Description", title: "Tax Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;

                me.pcmodel.TaxCode = data.TaxCode;
                me.pcmodel.TaxDesc = data.Description;
                me.Apply();
            }
        });
    };

    me.CollectorCode = function () {
        if (me.pcmodel.ProfitCenterCode != null) {

            var lookup = Wx.blookup({
                name: "CollectorsLookup",
                title: "Lookup Collectors",
                manager: gnManager,
                query: new breeze.EntityQuery().from("Collectors").withParameters({ profitCenterCode: me.pcmodel.ProfitCenterCode }),
                defaultSort: "CollectorCode asc",
                columns: [
                    { field: "CollectorCode", title: "Collector Code" },
                    { field: "CollectorName", title: "Collector Name" }
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.isSave = false;
                    me.pcmodel.CollectorCode = data.CollectorCode;
                    me.pcmodel.CollectorName = data.CollectorName;
                    me.Apply();
                }
            });
        }
    };

    me.TaxTransCode = function () {
        var lookup = Wx.blookup({
            name: "KodeTransaksiPajakLookup",
            title: "Lookup  TransaksiPajak Code",
            manager: gnManager,
            query: "KodeTransaksiPajak",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "TaxTrans Code" },
                { field: "LookUpValueName", title: "TaxTrans Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                //alert(data.LookUpValue);
                me.pcmodel.TaxTransCode = data.LookUpValue;
                me.pcmodel.TaxTransDesc = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    me.Salesman = function () {
        var lookup = Wx.blookup({
            name: "SalesmanLookup",
            title: "Lookup Salesman",
            manager: gnManager,
            query: "Salesmans",
            defaultSort: "EmployeeID asc",
            columns: [
                { field: "EmployeeID", title: "EmployeeID" },
                { field: "EmployeeName", title: "Employee Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pcmodel.Salesman = data.EmployeeID;
                me.pcmodel.SalesmanName = data.EmployeeName;
                me.Apply();
            }
        });
    };

    $('#KelAR').on('blur', function () {
        if (me.pcmodel.KelAR == null || me.pcmodel.KelAR == '') return;
        $http.post('gn.api/CustomerProfitCenter/KelompokAR', me.pcmodel).success(function (data) {
            if (data.success) {
                me.pcmodel.KelAR = data.data.LookUpValue;
                me.pcmodel.KelARDesc = data.data.LookUpValueName;
            }
            else {
                me.pcmodel.KelAR = me.pcmodel.KelARDesc = '';
                me.KelAR();
            }
        });
    });

    me.KelAR = function () {
        var lookup = Wx.blookup({
            name: "KelARLookup",
            title: "Lookup KelAR",
            manager: gnManager,
            query: "KelompokAR",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "KelAR" },
                { field: "LookUpValueName", title: "KelAR Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pcmodel.KelAR = data.LookUpValue;
                me.pcmodel.KelARDesc = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    me.CustomerGrade = function () {
        var lookup = Wx.blookup({
            name: "CustomerGradeLookup",
            title: "Lookup CustomerGrade",
            manager: gnManager,
            query: "CustomerGrades",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "CustomerGrade" },
                { field: "LookUpValueName", title: "CustomerGrade Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pcmodel.CustomerGrade = data.LookUpValue;
                me.pcmodel.CustomerGradeName = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    me.GroupPrice = function () {
        var lookup = Wx.blookup({
            name: "GroupPriceLookup",
            title: "Lookup GroupPrice",
            manager: gnManager,
            query: "GroupPrices",
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "GroupPrice" },
                { field: "RefferenceDesc1", title: "GroupPrice Desc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {

                me.isSave = false;
                me.pcmodel.GroupPriceCode = data.RefferenceCode;
                me.pcmodel.GroupPriceDesc = data.RefferenceDesc1;
                me.Apply();
            }
        });
    };

    me.TypeOfGoodsDisc = function () {
        var lookup = Wx.blookup({
            name: "TypeOfGoodsDiscLookup",
            title: "Lookup TypeOfGoodsDisc",
            manager: gnManager,
            query: "TypeOfGoods",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "TypeOfGoodsDisc" },
                { field: "LookUpValueName", title: "TypeOfGoodsNameDisc" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.pdmodel.TypeOfGoods = data.LookUpValue;
                me.pdmodel.TypeOfGoodsName = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    me.BankCode = function () {
        var lookup = Wx.blookup({
            name: "BankCodeLookup",
            title: "Lookup BankCode",
            manager: gnManager,
            query: "Banks",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Bank Code" },
                { field: "LookUpValueName", title: "Bank Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.bkmodel.BankCode = data.LookUpValue;
                me.bkmodel.BankName = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    me.setCustomerCode = function () {
        $http.post('gn.api/Customer/SetCustomerCode').
         success(function (data, status, headers, config) {

             if (data.data == true) {
                 $('#CustomerCode').attr("disabled", "disabled");
                 $('#StandardCode').attr("disabled", "disabled");
                 me.data.CustomerCode = "*******";
                 me.data.StandardCode = "*******";
             }
             else {
                 me.data.CustomerCode = "";
                 me.data.StandardCode = "";
             }
         });
    }

    me.initialize = function () {
        //me.setCustomerCode();
        //me.carabayar(true);
        me.grid = {};
        me.pcmodel = {};
        me.pdmodel = {};
        me.bkmodel = {};
        me.data.Status = '1';
        $('#CreditLimit').css("text-align", "right");
        me.grid.pcmodel = {};
        me.clearTable(me.grid1);

        //$('#isPKP').attr("disabled", true);
        me.grid.pdmodel = {};
        me.clearTable(me.grid2);

        me.grid.bkmodel = {};
        me.clearTable(me.grid3);
        me.data.NPWPDate = me.now();
        me.data.SKPDate = me.now();
        me.data.BirthDate = me.now();
        me.pcmodel.DiscPct = 0;
        me.pcmodel.isOverDueAllowed = false;
        me.pcmodel.isBlackList = false;
        me.data.isPKP = true;
        $('#btnProfitCenterCode,#btnCustomerClass,#btnCustomerClass,#btnTaxCode,#btnTaxTransCode,#btnKelAR').removeAttr("style");
        $('#btnProfitCenterCodeDisc,#btnTypeOfGoodsDisc').removeAttr("style");
        $('#btnBankCode').removeAttr("style");

        me.isSave = false;
        $http.post('gn.api/Customer/DefaultPKP').
           success(function (data, status, headers, config) {
               if (data) {
                   var EDITABLE_TXCODE = data[0].ParaValue;
                   var EDITABLE_TXTRNC = data[1].ParaValue;
                   var ADMIN_TXCODE = data[2].ParaValue;
                   var ADMIN_TXTRNC = data[3].ParaValue;
                   var EDITABLE = data[4].ParaValue;
                   var CUSTOMER = data[5].ParaValue;
                   //var SUPPLIER = data[6].ParaValue;
                   var mstCustomerAdmin = "0";

                   $http.post('gn.api/Customer/AccessCustomer').
                    success(function (data, status, headers, config) {
                        if (data.success) {
                            mstCustomerAdmin = "1";
                        } else {
                            mstCustomerAdmin = mstCustomerAdmin;
                        }
                        if (EDITABLE_TXCODE == "1" || mstCustomerAdmin == "1") {
                            $('#TaxCode').removeAttr("disabled");
                            $('#btnTaxCode').removeAttr("disabled");
                        }
                        if (EDITABLE_TXTRNC == "1" || mstCustomerAdmin == "1") {
                            $('#TaxTransCode').removeAttr("disabled");
                            $('#btnTaxTransCode').removeAttr("disabled");
                        }
                        if (CUSTOMER == "1") {
                            me.data.isPKP = true;
                            me.data.NPWPNo = "00.000.000.0-000.000"
                            me.data.SKPNo = "00.000.000.0-000.000"
                            me.isSave = false;
                        } else {
                            me.data.isPKP = false;
                        }
                        me.pcmodel.TaxCode = "PPN";
                        me.pcmodel.TaxDesc = "PAJAK PERTAMBAHAN NILAI";
                        me.pcmodel.TaxTransCode = "01";
                        me.pcmodel.TaxTransDesc = "Pemungut Selain Pajak";
                    });
               }
           });

        var custid = localStorage.getItem("params") || "";
        //console.log("customerid: ", custid);

        if (custid != "") {
            me.data.CustomerCode = custid;
            setTimeout(function () {
                me.lookupAfterSelect(me.data);
                me.GetCustomerInfoFromTrx(custid);
                me.isSave = false;
                me.Apply();
            }, 500)

        } else {
            me.setCustomerCode();
        }

        localStorage.setItem("params", "");
    }

    me.detailprofit = function (x, y) {
        $http.post('gn.api/CustomerProfitCenter/CustomerProfitCenter?CustomerCode=' + x + '&pc=' + y).
            success(function (data, status, headers, config) {
                if (data.data != false) {
                    me.pcmodel.CreditLimit = data.CreditLimit;
                    me.pcmodel.PaymentCode = data.PaymentCode;
                    me.pcmodel.CustomerClass = data.CustomerClass;
                    me.pcmodel.TaxCode = data.TaxCode;
                    me.pcmodel.TaxTransCode = data.TaxTransCode;
                    me.pcmodel.DiscPct = data.DiscPct;
                    me.pcmodel.LaborDiscPct = data.LaborDiscPct;
                    me.pcmodel.PartDiscPct = data.PartDiscPct;
                    me.pcmodel.MaterialDiscPct = data.MaterialDiscPct;
                    me.pcmodel.TOPCode = data.TOPCode;
                    me.pcmodel.CustomerGrade = data.CustomerGrade;
                    me.pcmodel.ContactPerson = data.ContactPerson;
                    me.pcmodel.CollectorCode = data.CollectorCode;
                    me.pcmodel.GroupPriceCode = data.GroupPriceCode;
                    me.pcmodel.isOverDueAllowed = data.isOverDueAllowed;
                    me.pcmodel.SalesCode = data.SalesCode;
                    me.pcmodel.SalesType = data.SalesType;
                    me.pcmodel.Salesman = data.Salesman;
                    me.pcmodel.isBlackList = data.isBlackList;
                    me.pcmodel.oid = true;
                    me.showProfitCenterInfo(x, y);
                } else {
                    me.pcmodel.TaxCode = "PPN";
                    me.pcmodel.TaxDesc = "PAJAK PERTAMBAHAN NILAI";
                    me.pcmodel.TaxTransCode = "01";
                    me.pcmodel.TaxTransDesc = "Pemungut Selain Pajak";
                }
            });
    }

    me.showProfitCenterInfo = function (CustomerCode, ProfitCenterCode) {
        $http.post('gn.api/Customer/ProfitCenterInfo?CustomerCode=' + CustomerCode + '&ProfitCenter=' + ProfitCenterCode).
        success(function (data, status, headers, config) {
            me.pcmodel.ProfitCenterName = data.ProfitCenterName;
            me.pcmodel.CustomerClassName = data.CustomerClassName;
            me.pcmodel.TaxDesc = data.TaxDesc;
            me.pcmodel.CollectorName = data.CollectorName;
            me.pcmodel.TaxTransDesc = data.TaxTransDesc;
            me.pcmodel.SalesmanName = data.SalesmanName;
            me.pcmodel.KelAR = data.KelAR;
            me.pcmodel.KelARDesc = data.KelARDesc;
            me.pcmodel.CustomerGradeName = data.CustomerGradeName;
            me.pcmodel.GroupPriceDesc = data.GroupPriceDesc;
        });
    }

    me.grid1 = new webix.ui({
        container: "wxProfitCenter",
        view: "wxtable", css: "alternating",
        columns: [
                { id: "ProfitCenterCode", header: "ProfitCenter Code", width: 120 },
                { id: "ProfitCenterName", header: "ProfitCenter Name", fillspace: true },
                { id: "CustomerGovName", header: "Customer GovName", fillspace: true },
                { id: "CustomerClass", header: "Customer Class", width: 120 },
                { id: "ContactPerson", header: "Contact Person", fillspace: true, format: me.replaceNull },

        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.pcmodel = this.getItem(me.grid1.getSelectedId().id);
                    me.pcmodel.oid = me.grid1.getSelectedId();
                    me.showProfitCenterInfo(me.data.CustomerCode, me.pcmodel.ProfitCenterCode);
                    me.Apply();
                }
            }
        }
    });

    me.grid2 = new webix.ui({
        container: "wxDiskonProduk",
        view: "wxtable", css: "alternating",
        columns: [

                { id: "ProfitCenterCode", header: "ProfitCenter Code", width: 120 },
                { id: "ProfitCenterName", header: "ProfitCenter Name", fillspace: true },
                { id: "TypeOfGoods", header: "Type Of Goods", width: 120 },
                { id: "TypeOfGoodsName", header: "TypeOf GoodsName", fillspace: true },
                { id: "DiscPct", header: "Discount", width: 120 },
        ],
        on: {
            onSelectChange: function () {
                if (me.grid2.getSelectedId() !== undefined) {
                    me.pdmodel = this.getItem(me.grid2.getSelectedId().id);
                    me.pdmodel.oid = me.grid2.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.grid3 = new webix.ui({
        container: "wxBank",
        view: "wxtable", css: "alternating",
        columns: [
                { id: "BankCode", header: "Bank Code", width: 120 },
                { id: "BankName", header: "Bank Name", width: 300 },
                { id: "AccountBank", header: "Account Bank", width: 120 },
                { id: "AccountName", header: "Account Name", fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                if (me.grid3.getSelectedId() !== undefined) {
                    me.bkmodel = this.getItem(me.grid3.getSelectedId().id);
                    me.bkmodel.old = me.grid3.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.saveData = function (e, param) {

        console.log(me.data)

        if (me.data.CustomerType === undefined) {
            MsgBox("Harap pilih Tipe Customer terlebih dahulu !", MSG_WARNING);
            return;
        }

        $http.post('gn.api/Customer/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    me.data.CustomerCode = data.data.CustomerCode;
                    $('#CustomerCode').val(data.data.CustomerCode);
                    $('#StandardCode').val(data.data.StandardCode);
                    me.data.CustomerCode = data.data.CustomerCode;
                    me.data.StandardCode = data.data.StandardCode;
                    me.startEditing();
                } else {
                    console.log(data.message)
                    MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                    MsgBox("Terdapat Kesalahan pada proses data," + data.message + ", hubungi SDMS support !", MSG_INFO);
                }
            }).
            error(function (data, status, headers, config) {
                console.log(data.message)
                MsgBox("Terdapat Kesalahan pada proses data," + data.message + ", hubungi SDMS support !", MSG_INFO);
            });
    };
    me.saveProfitCenter = function () {
        if ($('#ProfitCenterCode').val() == "100") {
            var Field = 'ProfitCenterCode,CustomerClass,PaymentCode,TaxCode,TaxTransCode,KelAR,TOPCode,GroupPrice,SalesType';
            var Names = 'Profit Center Code,Customer Class,Cara Pembayaran,Kode Pajak,Kode Transaksi Pajak,Kelompok AR,TOP,Group Price,Sales Type';
        } else {
            var Field = 'ProfitCenterCode,CustomerClass,PaymentCode,TaxCode,TaxTransCode,KelAR,TOPCode';
            var Names = 'Profit Center Code,Customer Class,Cara Pembayaran,Kode Pajak,Kode Transaksi Pajak,Kelompok AR,TOP';
        }

        var ret = me.CheckMandatory(Field, Names);

        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            me.pcmodel.CustomerCode = $('#CustomerCode').val();
            $http.post('gn.api/CustomerProfitCenter/Save', me.pcmodel).
                success(function (data, status, headers, config) {
                    if (data.status) {
                        Wx.Success(data.message);
                        me.closeProfitCenter();
                        me.startEditing();
                        me.clearTable(me.grid1);
                        me.grid.model = data.data;
                        me.loadTableData(me.grid1, me.grid.model);
                    } else {
                        console.log(data.message)
                        MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                    }
                }).
                error(function (data, status, headers, config) {
                    console.log(data.message)
                    MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                });
        }
    };
    me.saveCustomerDiscount = function () {
        var Field = "ProfitCenterCodeDisc,TypeOfGoodsDisc";
        var Names = "Profit Center Code,Type Of Goods";
        var ret = me.CheckMandatory(Field, Names);
        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            me.pdmodel.CustomerCode = me.data.CustomerCode;
            $http.post('gn.api/CustomerDiscount/Save', me.pdmodel).
                success(function (data, status, headers, config) {
                    if (data.status) {
                        Wx.Success("Data saved...");
                        me.closeCustomerDiscount();
                        me.startEditing();
                        me.clearTable(me.grid2);
                        me.grid.model = data.data;
                        me.loadTableData(me.grid2, me.grid.model);
                    } else {
                        console.log(data.message)
                        MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                    }
                }).
                error(function (data, status, headers, config) {
                    console.log(data.message)
                    MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                });
        }
    };
    me.saveCustomerBank = function () {
        me.bkmodel.CustomerCode = me.data.CustomerCode;
        $http.post('gn.api/CustomerBank/Save', me.bkmodel).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success(data.message);
                    me.closeCustomerBank();
                    me.startEditing();
                    me.grid.bkmodel = data.data;
                    me.loadTableData(me.grid3, me.grid.bkmodel);
                } else {
                    console.log(data.message)
                    MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                }
            }).
            error(function (data, status, headers, config) {
                console.log(data.message)
                MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
            });
    };
    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/Customer/Delete', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.init();
                    Wx.Success("Data deleted...");
                } else {
                    //MsgBox(data.message, MSG_ERROR);
                    console.log(data.message)
                    MsgBox("Gagal Hapus data karena masih ada detail !", MSG_INFO);
                }
            }).
            error(function (data, status, headers, config) {
                console.log(data.message)
                MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
            });
        });
    };
    me.deleteProfitCenter = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/CustomerProfitCenter/Delete', me.pcmodel).
                success(function (data, status, headers, config) {
                    if (data.status) {
                        Wx.Success(data.message);
                        me.closeProfitCenter();
                        me.clearTable(me.grid1);
                        me.grid.model = data.data;
                        me.loadTableData(me.grid1, me.grid.model);
                    } else {
                        //MsgBox(data.message, MSG_ERROR);
                        console.log(data.message)
                        MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                    }
                }).
                error(function (data, status, headers, config) {
                    console.log(data.message)
                    MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                });
        });
    };
    me.deleteCustomerDiscount = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/CustomerDiscount/Delete', me.pdmodel).
                success(function (data, status, headers, config) {
                    if (data.status) {
                        Wx.Success(data.message);
                        me.closeCustomerDiscount();
                        me.clearTable(me.grid2);
                        me.grid.model = data.data;
                        me.loadTableData(me.grid2, me.grid.model);
                    } else {
                        //MsgBox(data.message, MSG_ERROR);
                        console.log(data.message)
                        MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                    }
                }).
                error(function (data, status, headers, config) {
                    console.log(data.message)
                    MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                });
        });
    };
    me.deleteCustomerBank = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            var customerCode = { CustomerCode: me.data.CustomerCode };
            var maindata = $.extend(customerCode, me.bkmodel);
            $http.post('gn.api/customerbank/delete', maindata).
                success(function (data, status, headers, config) {
                    if (data.status) {
                        Wx.Success(data.message);
                        me.closeCustomerBank();
                        me.clearTable(me.grid3);
                        me.grid.model = data.data;
                        me.loadTableData(me.grid3, me.grid.model);
                        me.bkmodel = {};
                    } else {
                        console.log(data.message)
                        MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                    }
                }).
                error(function (data, status, headers, config) {
                    console.log(data.message)
                    MsgBox("Terdapat Kesalahan pada proses data, hubungi SDMS support !", MSG_INFO);
                });
        });
    };

    //me.cancelOrClose = function () {
    //    me.init();
    //    me.save();
    //}
    me.closeProfitCenter = function () {
        me.pcmodel = {};
        me.grid1.clearSelection();
        me.pcmodel.TaxCode = "PPN";
        me.pcmodel.TaxDesc = "PAJAK PERTAMBAHAN NILAI";
        me.pcmodel.TaxTransCode = "01";
        me.pcmodel.TaxTransDesc = "Pemungut Selain Pajak";
    };
    me.closeCustomerDiscount = function () {
        me.pdmodel = {};
        me.grid2.clearSelection();
    };
    me.closeCustomerBank = function () {
        me.bkmodel = {};
        me.grid3.clearSelection();
    };

    webix.event(window, "resize", function () {
        me.grid1.adjust();
        me.grid2.adjust();
        me.grid3.adjust();
    });

    me.OnTabChange = function (e, id) {
        if (id === "tabProfitCenter") {
            me.grid1.adjust();
        }

        if (id === "tabProductDiscount") {
            me.grid2.adjust();
        }
        if (id === "tabBank") {
            me.grid3.adjust();
        }

    };

    setTimeout(function () {
        me.grid1.adjust();
        me.grid2.adjust();
        me.grid3.adjust();
    }, 555);

    $('#PaymentCode').on('change', function (e) {
        if ($('#PaymentCode').val() == 'CS' || $('#PaymentCode').val() == 'DC') {
            me.carabayar(false);
        } else {
            me.carabayar(true);
        }
    });

    $('#ProfitCenterCode').on('blur', function (e) {
        if (me.pcmodel.ProfitCenterCode == null || me.pcmodel.ProfitCenterCode == '') return;
        $http.post('gn.api/CustomerProfitCenter/ProfitCenterCode', me.pcmodel).success(function (data) {
            if (data.success) {
                me.pcmodel.ProfitCenterCode = data.data.LookUpValue;
                me.pcmodel.ProfitCenterName = data.data.LookUpValueName;
            }
            else {
                me.pcmodel.ProfitCenterCode = me.pcmodel.ProfitCenterName = '';
                me.ProfitCenterCode();
            }
        });
        if ($('#ProfitCenterCode').val() == '100') {
            $('#GroupPrice').attr("style", "background-color: rgb(255, 218, 204)");
            $('#btnGroupPrice').removeAttr("style");
            $('#s2id_SalesType').attr("style", "background-color: rgb(255, 218, 204)");
        }
    });

    me.disableFiil = function () {
        var p = 'CustomerName,CustomerAbbrName,isPKP,Gender,CustomerType,HPNo,CategoryCode,CategoryName,PhoneNo,OfficePhoneNo,';
        p += 'CustomerGovName,FaxNo,NPWPNo,NPWPDate,Email,BirthDate,SKPNo,SKPDate,ZipNo,PosName,';
        p += 'Address1,IbuKota,Address2,CityCode,Address3,AreaCode,Address4,Status';
        var q = p.split(',', 30);
        console.log(q);
        var r = q.length;
        var a = 0;
        while (a <= r) {
            $('#' + q[a]).attr('disabled', true);
            a++;
        }
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Customer",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "Data Customer",
                title: "Data Pribadi",
                items: [
                    {
                        name: "CustomerCategory", type: "controls", text: "Customer", items: [
                            { name: "CustomerCode", type: "text", text: "Customer Code", cls: "span2", validasi: "max(15)" }, //, disable: "data.CustomerCode != ''|| (IsEditing() && length(data.CustomerCode)<15) " },
                            { name: "CustomerName", type: "text", text: "Customer Name", cls: "span6", validasi: "required,max(50)" },
                        ]
                    },

                    { name: "StandardCode", type: "text", text: "Kode Standar", cls: "span4", validasi: "max(15)" }, //, disable: "data.StandardCode != ''|| (IsEditing() && length(data.StandardCode)<15) " },
                    { name: "CustomerAbbrName", text: "Nama Inisial", cls: "span4", maxlength: 15 },
                    { name: "isPKP", type: "x-switch", text: "Faktur Pjk Std?", cls: "span4", float: "left", disable: true },
                    { name: "Gender", type: "select2", text: "Gender", cls: "span4", validasi: "required", datasource: "comboGender" },
                    { name: "CustomerType", type: "select2", text: "Tipe Customer", cls: "span4", datasource: "comboCustomerType", validasi: "required", required: true },
                    { name: "HPNo", type: "text", text: "HP No", cls: "span4", validasi: "required", required: true, maxlength: 15 },
                    {
                        name: "CustomerCategory", type: "controls", text: "Kode Kategori", required: true, items: [
                            { name: "CategoryCode", type: "popup", text: "Category Code", cls: "span3", click: "CategoryCode()", validasi: "required", required: true },
                            { name: "CategoryName", type: "text", text: "Category Name", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        name: "Telephone", type: "controls", text: "PhoneNo", required: true, items: [
                            { name: "PhoneNo", type: "text", text: "Phone No", cls: "span3", validasi: "required", required: true, maxlength: 15 },
                            { name: "OfficePhoneNo", type: "text", text: "Office Phone No", cls: "span5", validasi: "max(15)" },
                        ]
                    },
                    { name: "CustomerGovName", type: "text", text: "Nama Pajak", cls: "span8", validasi: "required", required: true },
                    { name: "FaxNo", type: "text", text: "Fax No", cls: "span4 full", maxlength: 15 },
                    { name: "NPWPNo", type: "ng-maskedit", mask: "##.###.###.#-###.###", text: "NPWP No", cls: "span4", validasi: "required", required: true },
                    { name: "NPWPDate", type: "ng-datepicker", text: "NPWP Date", cls: "span4" },
                    { name: "Email", text: "Email", cls: "span4", validasi: "max(50)" },
                    { name: "BirthDate", type: "ng-datepicker", text: "Tgl.Lahir", cls: "span4" },
                    { name: "SKPNo", type: "ng-maskedit", mask: "##.###.###.#-###.###", text: "SKP No", cls: "span4", validasi: "required", required: true },
                    { name: "SKPDate", type: "ng-datepicker", text: "SKP Date", cls: "span4" },
                    {
                        name: "KodePos", type: "controls", text: "Kode Pos", items: [
                            { name: "ZipNo", type: "popup", text: "Pos Code", cls: "span3", validasi: "required", click: "PosCode()" },
                            { name: "PosName", type: "text", text: "Pos Name", cls: "span5", readonly: true },
                        ]
                    },
                    { name: "Address1", text: "Address 1", cls: "span4", validasi: "required", required: true, maxlength: "40" },
                    { name: "IbuKota", text: "IbuKota", cls: "span4", readonly: true },
                    { name: "Address2", text: "Address 2", cls: "span4", validasi: "required", required: true, maxlength: "40" },
                    { name: "CityCode", text: "Kode Kota", cls: "span4", readonly: true },
                    { name: "Address3", text: "Address 3", cls: "span4", validasi: "max(40)", maxlength: "40" },
                    { name: "AreaCode", text: "Kode Area", cls: "span4", readonly: true },
                    { name: "Address4", text: "Address 4", cls: "span4", validasi: "max(40)", maxlength: "40" },
                    { name: "Status", type: "select2", text: "Status", cls: "span4", datasource: "comboStatus" },
                ]
            },
            {
                xtype: "tabs",
                name: "tabCustomer",
                items: [
                    { name: "tabProfitCenter", text: "Profit Center", cls: "active" },
                    { name: "tabProductDiscount", text: "Discount Product" },
                    { name: "tabBank", text: "Bank" },
                ],

            },
            {
                name: "tabProfitCenter",
                cls: "tabCustomer tabProfitCenter",
                items: [
                     {
                         type: "controls", text: "Profit Center", required: true, items: [
                             { name: "ProfitCenterCode", model: "pcmodel.ProfitCenterCode", type: "popup", text: "Profit Center", cls: "span3", click: "ProfitCenterCode()", style: "background-color: rgb(255, 218, 204)" },
                             { name: "ProfitCenterName", model: "pcmodel.ProfitCenterName", text: "Profit Center Name", cls: "span5", readonly: true },
                         ],
                     },
                    {
                        type: "controls", text: "Customer Class", required: true, items: [
                            { name: "CustomerClass", model: "pcmodel.CustomerClass", type: "popup", text: "Customer Class", cls: "span3", click: "CustomerClass()", style: "background-color: rgb(255, 218, 204)" },
                            { name: "CustomerClassName", model: "pcmodel.CustomerClassName", type: "", text: "Customer Class Name", cls: "span5", readonly: true },
                        ],
                    },
                    {
                        type: "controls", text: "Cara Pembayaran", required: true, items: [
                            { name: "PaymentCode", model: "pcmodel.PaymentCode", text: "Payment Code", cls: "span3", type: "select2", datasource: "comboPaymentCode", style: "background-color: rgb(255, 218, 204)" },
                        ],
                    },

                    {
                        type: "controls", text: "Kode Pajak", required: true, items: [
                            { name: "TaxCode", model: "pcmodel.TaxCode", type: "popup", text: "Tax Code", cls: "span3", click: "TaxCode()", disable: true },
                            { name: "TaxDesc", model: "pcmodel.TaxDesc", type: "", text: "Tax Desc", cls: "span5", readonly: true },
                        ],
                    },
                    {
                        type: "controls", text: "Kode Kollector", items: [
                            { name: "CollectorCode", model: "pcmodel.CollectorCode", type: "popup", text: "Collector Code", cls: "span3", readonly: true, click: "CollectorCode()" },
                            { name: "CollectorName", model: "pcmodel.CollectorName", text: "Collector Name", cls: "span5", readonly: true },
                        ],
                    },
                    {
                        type: "controls", text: "Kode Trans.Pajak", required: true, items: [
                            { name: "TaxTransCode", model: "pcmodel.TaxTransCode", type: "popup", text: "TaxTrans Code", cls: "span3", click: "TaxTransCode()", maxlength: 3, disable: true },
                            { name: "TaxTransDesc", model: "pcmodel.TaxTransDesc", text: "TaxTrans Desc", cls: "span5", readonly: true },
                        ],
                    },
                    {
                        type: "controls", text: "Kode Salesman", items: [
                            { name: "Salesman", model: "pcmodel.Salesman", type: "popup", text: "Salesman Code", cls: "span3", readonly: true, click: "Salesman()" },
                            { name: "SalesmanName", model: "pcmodel.SalesmanName", text: "Salesman Name", cls: "span5", readonly: true },
                        ],
                    },
                    {
                        type: "controls", text: "Kelompok AR", required: true, items: [
                            { name: "KelAR", model: "pcmodel.KelAR", type: "popup", text: "Group AR", cls: "span3", click: "KelAR()", style: "background-color: rgb(255, 218, 204)" },
                            { name: "KelARDesc", model: "pcmodel.KelARDesc", type: "", text: "Group AR Desc", cls: "span5", readonly: true },
                        ],
                    },
                    { name: "CreditLimit", model: "pcmodel.CreditLimit", text: "Kredit Limit", cls: "span4 number-int", type: "text", placeHolder: "0.00", maxlength: 16, disable: true },
                    { name: "isOverDueAllowed", model: "pcmodel.isOverDueAllowed", text: "Overdue", cls: "span2 ", type: "x-switch", float: "left" },
                    { name: "isBlackList", model: "pcmodel.isBlackList", text: "Black List", cls: "span2 ", type: "x-switch", float: "left" },

                    {
                        type: "controls", text: "Customer Grade", items: [
                            { name: "CustomerGrade", model: "pcmodel.CustomerGrade", type: "popup", text: "Customer Grade", cls: "span3", readonly: true, click: "CustomerGrade()" },
                            { name: "CustomerGradeName", model: "pcmodel.CustomerGradeName", text: "Customer Grade Name", cls: "span5", readonly: true },
                        ],
                    },

                    { name: "ContactPerson", model: "pcmodel.ContactPerson", type: "text", text: "Contact Person", cls: "span4 " },
                    { name: "TOPCode", model: "pcmodel.TOPCode", type: "select2", required: true, text: "TOP", cls: "span4 ", datasource: "comboTOPCode", style: "background-color: rgb(255, 218, 204)" },

                     {
                         type: "controls", text: "Group Price", items: [
                             { name: "GroupPrice", model: "pcmodel.GroupPriceCode", type: "popup", text: "Group Price", cls: "span3", readonly: true, click: "GroupPrice()" },
                             { name: "GroupPriceDesc", model: "pcmodel.GroupPriceDesc", type: "", text: "Group Price Desc", cls: "span5", readonly: true },
                         ],
                     },

                    { name: "DiscPct", model: "pcmodel.DiscPct", type: "text", text: "Discount", placeHolder: "0", cls: "span4 number-int", maxlength: 7, disable: true },
                    {
                        name: "SalesType", model: "pcmodel.SalesType", type: "select2", text: "Sales Type", cls: "span4 ", datasource: "comboSalesType"
                    },

                    {
                        type: "controls", text: "Labor Discount", show: "pcmodel.ProfitCenterCode=='200'", items: [
                            { name: "LaborDiscPct", model: "pcmodel.LaborDiscPct", type: "text", text: "Labor Discount", placeHolder: "0", cls: "span2 number", maxlength: 7, disable: true },
                            { cls: "span1 label-valign", text: "&nbsp;  Diskon Part  &nbsp;", type: "label" },
                            { name: "PartDiscPct", model: "pcmodel.PartDiscPct", type: "text", text: "Part Discount", placeHolder: "0", cls: "span2 number", maxlength: 7, disable: true },
                            { cls: "span1 label-valign", text: "&nbsp;  Diskon Material  &nbsp;", type: "label" },
                            { name: "MaterialDiscPct", model: "pcmodel.MaterialDiscPct", type: "text", text: "Material Discount", placeHolder: "0", cls: "span2 number", maxlength: 7, disable: true },
                        ],
                    },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "saveProfitCenter()", show: "pcmodel.oid === undefined", disable: "pcmodel.ProfitCenterCode === undefined" },
                                { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "saveProfitCenter()", show: "pcmodel.oid !== undefined" },
                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteProfitCenter()", show: "pcmodel.oid !== undefined" },
                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "closeProfitCenter()", show: "pcmodel.oid !== undefined" }
                        ]
                    },


                    {
                        name: "wxProfitCenter",
                        cls: "tabpage1 tabProfitCenter",
                        type: "wxdiv"
                    },
                ]
            },


            {
                name: "tabProductDiscount",
                cls: "tabCustomer tabProductDiscount",
                items: [
                     {
                         type: "controls", text: "Profit Center", items: [
                             { name: "ProfitCenterCodeDisc", model: "pdmodel.ProfitCenterCode", text: "Profit Center Code", type: "popup", cls: "span3", required: false, click: "ProfitCenterCodeDisc()", style: "background-color: rgb(255, 218, 204)" },
                             { name: "ProfitCenterNameDisc", model: "pdmodel.ProfitCenterName", text: "Profit Center Name", cls: "span5", readonly: true, required: false },
                         ]
                     },
                    {
                        type: "controls", text: "Type Of Goods", items: [
                            { name: "TypeOfGoodsDisc", model: "pdmodel.TypeOfGoods", text: "Type Of Goods Disc", type: "popup", cls: "span3", required: false, click: "TypeOfGoodsDisc()", style: "background-color: rgb(255, 218, 204)" },
                            { name: "TypeOfGoodsNameDisc", model: "pdmodel.TypeOfGoodsName", text: "Type Of Goods Name", cls: "span5", readonly: true, required: false },
                        ]
                    },
                    {
                        type: "controls", text: "Discount", items: [
                            { name: "DiscPctDisc", model: "pdmodel.DiscPct", type: "text", cls: "span3 number-int", required: false, text: "% Discount", maxlength: 7 }
                        ]
                    },



                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "saveCustomerDiscount()", show: "pdmodel.oid === undefined", disable: "pdmodel.ProfitCenterCode === undefined" },
                                 { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "saveCustomerDiscount()", show: "pdmodel.oid !== undefined" },
                                 { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteCustomerDiscount()", show: "pdmodel.oid !== undefined" },
                                 { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "closeCustomerDiscount()", show: "pdmodel.oid !== undefined" }
                        ]
                    },


                    {
                        name: "wxDiskonProduk",
                        cls: "tabpage2 tabProductDiscount",
                        type: "wxdiv"
                    },
                ]
            },
            {
                name: "tabBank",
                cls: "tabCustomer tabBank",
                items: [
                    {
                        type: "controls", text: "Bank", items: [
                            { name: "BankCode", model: "bkmodel.BankCode", type: "popup", text: "Bank Code", cls: "span3", readonly: true, click: "BankCode()", style: "background-color: rgb(255, 218, 204)" },
                            { name: "BankName", model: "bkmodel.BankName", type: "", text: "Bank Name", cls: "span5", readonly: true, required: true },
                        ]
                    },
                    { name: "AccountName", model: "bkmodel.AccountName", type: "text", text: "Account Name", cls: "span8", required: false },
                    { name: "AccountBank", model: "bkmodel.AccountBank", type: "text", text: "Account Bank", cls: "span8", required: false },

                     {
                         type: "buttons",
                         items: [
                                 { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "saveCustomerBank()", show: "bkmodel.old === undefined", disable: "bkmodel.BankCode === undefined" },
                                 { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "saveCustomerBank()", show: "bkmodel.old !== undefined" },
                                 { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteCustomerBank()", show: "bkmodel.old !== undefined" },
                                 { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "closeCustomerBank()", show: "bkmodel.old !== undefined" }
                         ]
                     },
                    {
                        name: "wxBank",
                        cls: "tabpage3 tabBank",
                        type: "wxdiv"
                    },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterCustomersController");
    }

});