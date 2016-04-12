"use strict"

function omDraftSOController($scope, $http, $injector) {
    var me = $scope;
    var currentDate = moment().format();

    $injector.invoke(BaseController, this, { $scope: me });

    me.BrowseITS = function () {
        var lookup = Wx.klookup({
            name: "lookupITS",
            title: "Pencarian ITS",
            url: "om.api/DraftSalesOrder/BrowseITS",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "InquiryNo", title: "No. ITS", width: 150 },
                {
                    field: "InquiryDate", title: "Tgl. ITS", width: 120,
                    template: "#= (InquiryDate == undefined) ? '' : moment(InquiryDate).format('DD MMM YYYY') #"
                },
                { field: "EmployeeName", title: "Salesman", width: 250 },
                { field: "NamaProspek", title: "Nama Prospek", width: 250 },
                { field: "TipeKendaraan", title: "Tipe Kendaraan", width: 160 },
                { field: "EmployeeID", title: "Employee ID", width: 150 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.ProspectNo = data.InquiryNo;
            me.data.TipeKendaraan = data.TipeKendaraan;
            me.Apply();
        });
    }

    //me.BrowseCustomer =  function(){
    //    var params = {
    //        chkType: me.data.chkType
    //    }
    //    var lookup = Wx.klookup({
    //        name: "lookupCustomer",
    //        title: "Pelanggan",
    //        url: "om.api/DraftSalesOrder/BrowseCustomer",
    //        params: params,
    //        serverBinding: true,
    //        pageSize: 10,
    //        columns: [
    //            { field: "CustomerCode", title: "Kode Pelanggan", width: 120 },
    //            { field: "CustomerName", title: "Nama Pelanggan", width: 260 },
    //            { field: "Address", title: "Alamat", width: 460 },
    //            { field: "TopCode", title: "Kode TOP", width: 150 },
    //            { field: "Salesman", title: "Kelompok AR", width: 120 }
    //        ]
    //    });
    //    lookup.dblClick(function (data) {
    //        me.data.CustomerCode = data.CustomerCode;
    //        me.data.CustomerName = data.CustomerName;
    //        me.data.TOPCode = data.TOPCD;
    //        me.data.GroupPriceCode = data.GroupPriceCode;
    //        me.data.GroupPriceDesc = data.GroupPriceDesc;
    //        me.Apply();

    //        var params = {
    //            TOPCode: me.data.TOPCode
    //        }
    //        $http.post("om.api/DraftSalesOrder/TOPCInterval", params)
    //        .success(function (TOPCInterval) {
    //            me.data.TOPInterval = TOPCInterval;
    //        })
    //        .error(function (result) {
    //            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
    //        });
    //    });
    //}

    me.BrowseCustomer = function () {
        var Query = "";
        if (me.data.chkType == true) Query = 'uspfn_getSelectLookupCustomer2';
        else Query = 'uspfn_getSelectLookupCustomer';
        var lookup = Wx.klookup({
            name: "CustomersBrowse",
            title: "Customers Browse",
            url: "om.api/Grid/CustBrowse?cols=" + 5 + "&spId=" + Query,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "CustomerCode", title: "Kode Customer", width: 100 },
                { field: "CustomerName", title: "Nama Customer", width: 180 },
                { field: "Address1", title: "Alamat", width: 350 },
                { field: "TOPDesc", title: "Kode TOP", width: 125 },
                { field: "SalesCode", title: "Kelompok AR", width: 140 },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.CustomerCode = data.CustomerCode;
            me.data.CustomerName = data.CustomerName;
            me.data.TOPCode = data.TOPCD;
            me.data.GroupPriceCode = data.GroupPriceCode;
            me.data.GroupPriceDesc = data.GroupPriceDesc;
            me.Apply();

            var params = {
                TOPCode: me.data.TOPCode
            }
            $http.post("om.api/DraftSalesOrder/TOPCInterval", params)
            .success(function (TOPCInterval) {
                me.data.TOPInterval = TOPCInterval;
            })
            .error(function (result) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        });
    }

    me.BrowseSalesman = function () {
        var lookup = Wx.klookup({
            name: "lookupSalesman",
            title: "Salesman Lookup",
            url: "om.api/DraftSalesOrder/BrowseSalesman",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "EmployeeID", title: "ID", width: 150 },
                { field: "EmployeeName", title: "Nama", width: 260 },
                { field: "TitleName", title: "Jabatan", width: 360 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.Salesman = data.EmployeeID;
            me.data.SalesName = data.EmployeeName;
            me.Apply();
        });
    };

    me.BrowseTOP = function () {
        var lookup = Wx.klookup({
            name: "lookupTOP",
            title: "Term of Payment",
            url: "om.api/DraftSalesOrder/BrowseTOPC",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "LookUpValue", title: "Kode TOP", width: 80 },
                { field: "LookUpValueName", title: "Nama TOP", width: 260 },
                { field: "ParaValue", title: "Interval", width: 80 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.TOPCode = data.LookUpValue;
            me.data.TOPInterval = data.ParaValue;
            me.Apply();
        });
    };

    me.BrowseLeasing = function () {
        var lookup = Wx.klookup({
            name: "lookupLeasing",
            title: "Leasing",
            url: "om.api/DraftSalesOrder/BrowseLeasing",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "LeasingCode", title: "Kode TOP", width: 150 },
                { field: "LeasingName", title: "Nama TOP", width: 260 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.LeasingCo = data.LeasingCode;
            me.data.LeasingName = data.LeasingName;
            me.Apply();
        });
    };

    me.BrowseModel = function () {
        var params = {
            inquiryNumber: me.data.ProspectNo
        };
        var lookup = Wx.klookup({
            name: "lookupModel",
            title: "Sales Model Code",
            url: "om.api/DraftSalesOrder/BrowseModel",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "SalesModelCode", title: "Kode Sales Model", width: 160 },
                { field: "SalesModelDesc", title: "Deskripsi", width: 260 }
            ]
        });
        lookup.dblClick(function (data) {
            me.ResetPriceModel()
            me.recordModel.SalesModelCode = data.SalesModelCode;
            me.recordModel.SalesModelDesc = "";
            me.allowDeleteSalesModel = false;
            me.Apply();
        });
    };

    me.BrowseModelYear = function () {
        var params = {
            salesModelCode: me.recordModel.SalesModelCode,
            groupPriceCode: me.data.GroupPriceCode
        };
        var lookup = Wx.klookup({
            name: "lookupModelYear",
            title: "Sales Model Year",
            url: "om.api/DraftSalesOrder/BrowseModelYear",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "SalesModelYear", title: "Tahun", width: 160 },
                { field: "SalesModelDesc", title: "Deskripsi", width: 260 },
                { field: "ChassisCode", title: "Kode Rangka", width: 260 }
            ]
        });
        lookup.dblClick(function (data) {
            me.recordModel.SalesModelYear = data.SalesModelYear;
            me.recordModel.SalesModelDesc = data.SalesModelDesc;
            me.TotalPricelistSell();
            me.PopulateRecordModel();
            me.allowDeleteSalesModel = false;
            me.Apply();
        });
    };

    me.BrowseColour = function () {
        var params = {
            salesModelCode: me.recordModel.SalesModelCode
        };
        var lookup = Wx.klookup({
            name: "lookupColour",
            title: "Warna",
            url: "om.api/DraftSalesOrder/BrowseColour",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "ColourCode", title: "Kode", width: 100 },
                { field: "ColourDesc", title: "Warna", width: 260 }
            ]
        });
        lookup.dblClick(function (data) {
            me.recordColour.ColourCode = data.ColourCode;
            me.recordColour.ColourDesc = data.ColourDesc;
            me.Apply();

            me.PopulateRecordModelColour();
        });
    };

    me.BrowseBBN = function () {
        var params = {
            salesModelCode: me.recordModel.SalesModelCode,
            salesModelYear: me.recordModel.SalesModelYear
        };
        var lookup = Wx.klookup({
            name: "lookupBBN",
            title: "Pemasok BBN",
            url: "om.api/DraftSalesOrder/BrowseBBN",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "SupplierBBN", title: "Kode Pemasok", width: 160 },
                { field: "SupplierBBNName", title: "Nama Pemasok", width: 260 }
            ]
        });
        lookup.dblClick(function (data) {
            me.recordVin.SupplierBBN = data.SupplierBBN;
            me.recordVin.SupplierBBNName = data.SupplierBBNName;
            me.Apply();
        });
    };

    me.CustomerShow = function () {
        var custCode = me.data.CustomerCode;
        Wx.loadForm();
        Wx.showForm({ url: "gn/master/customer", params: custCode });
    };

    me.BrowseCity = function () {
        var params = {
            supplierCode: me.recordVin.SupplierBBN,
            salesModelCode: me.recordModel.SalesModelCode,
            salesModelYear: me.recordModel.SalesModelYear
        };
        var lookup = Wx.klookup({
            name: "lookupCity",
            title: "Kota",
            url: "om.api/DraftSalesOrder/BrowseCity",
            params: params,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "CityCode", title: "Kode Kota", width: 100 },
                { field: "CityName", title: "Nama Kota", width: 360 },
                { field: "BBN", title: "BBN", width: 160 },
                { field: "KIR", title: "KIR", width: 160 }
            ]
        });
        lookup.dblClick(function (data) {
            me.recordVin.CityCode = data.CityCode;
            me.recordVin.CityName = data.CityName;
            me.Apply();
        });
    };

    me.BrowseAccesories = function () {
        var lookup = Wx.klookup({
            name: "lookupAcce",
            title: "Aksesori Lain",
            url: "om.api/DraftSalesOrder/BrowseAccesories",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "RefferenceCode", title: "Kode", width: 100 },
                { field: "RefferenceDesc1", title: "Deskripsi", width: 360 }
            ]
        });
        lookup.dblClick(function (data) {
            me.recordOthers.OtherCode = data.RefferenceCode;
            me.recordOthers.AccsName = data.RefferenceDesc1;
            me.Apply();
        });
    };

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SPULookup",
            title: "Sales Order Lookup",
            manager: spSalesManager,
            query: "SPUBrowse",
            defaultSort: "DraftSONo desc",
            columns: [
                { field: "DraftSONo", title: "No. SO", width: 160 },
                { field: "TypeSales", title: "Tipe", width: 120 },
                {
                    field: "DraftSODate", title: "Tanggal SO", width: 120,
                    template: "#= (DraftSODate == undefined) ? '' : moment(DraftSODate).format('DD MMM YYYY') #"
                },
                { field: "RefferenceNo", title: "No. Reff.", width: 160 },
                {
                    field: "RefferenceDate", title: "Tanggal Reff.", width: 120,
                    template: "#= (RefferenceDate == undefined || RefferenceDate == '') ? '' : moment(RefferenceDate).format('DD MMM YYYY') #"
                },
                { field: "Customer", title: "Pelanggan", width: 300 },
                { field: "Address", title: "Alamat", width: 600 },
                { field: "Salesman", title: "Salesman", width: 300 },
                { field: "GroupPriceCode", title: "Group Price Code", width: 200 },
                { field: "Stat", title: "Status", width: 120 }
            ]
        });
        lookup.dblClick(function (record) {
            me.PopulateRecord(record.DraftSONo);
        });
    }

    me.PopulateRecord = function (draftSONo) {
        var params = {
            draftSONo: draftSONo
        }
        $http.post("om.api/DraftSalesOrder/PopulateRecord", params)
        .success(function (result) {
            if (result.success) {
                me.isCheckingQtySO = result.data.isCheckingQtySO;
                var record = result.data.DraftSO;
                me.lookupAfterSelect(record);
                me.ClearRecordDetails();
                me.LoadTableDetails();
                me.loadTableData(me.gridSalesModel, result.data.DraftSOModel);
                me.AdjustGrid();

                $("#btnCustomer, #DraftSODate").attr("disabled", "disabled");

                if (me.data.RefferenceDate == null) me.data.RefferenceDate = "1900/01/01";
                if (me.data.RefferenceDate == "1900/01/01") {
                    me.data.RefferenceDate = currentDate;
                    me.data.chkReffDate = false;
                    $("#chkReffDate").attr("disabled", "disabled");
                }
                else {
                    me.data.chkReffDate = true;
                    $("#chkReffDate").removeAttr("disabled");
                }

                if (me.data.SalesType == "1") {
                    me.data.chkType = true;
                }
                else {
                    me.data.chkType = false;
                }

                if (record.isLeasing == false) {
                    me.data.FinalPaymentDate = currentDate;
                }

                if (me.data.RequestDate == null) me.data.RequestDate = "1900/01/01";

                if (record.RequestDate == "1900/01/01") {
                    record.RequestDate = currentDate;
                    me.data.chkRequestDate = false;
                }
                else { me.data.chkRequestDate = true; me.data.RequestDate = record.RequestDate; }

                $("#SalesModelCode").focus();
                me.StatusLabel(record);
                me.RetrieveNameOrDescription();

                if (result.data.DraftSOModel.length > 0) {
                    me.FormReady(false);
                    me.allowInputDateReff = false;
                }

                $("#btnLeasing, #Installment, #FinalPayment").attr("disabled", "disabled");
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

    };

    me.PopulateRecordModel = function () {
        me.clearTable(me.gridVinInfo);
        me.AdjustGrid();

        var params = {
            draftSONo: me.data.DraftSONo,
            salesModelCode: me.recordModel.SalesModelCode,
            salesModelYear: me.recordModel.SalesModelYear,
            groupPriceCode: me.data.GroupPriceCode
        };

        $http.post("om.api/DraftSalesOrder/PopulateRecordModel", params)
        .success(function (result) {
            if (result.success) {
                me.ClearRecordColour();
                me.ClearRecordVin();
                me.ClearRecordOthers();
                if (result.data != null) {
                    if (result.data.recDraftSOModel != null) {
                        result.data.recDraftSOModel.SalesModelDesc = result.SalesModelDesc;
                        me.RetrieveDataModel(result.data.recDraftSOModel);
                    }
                    else {
                        me.ResetPriceModel_2();
                        if (me.allowInputSalesModel) {
                            me.allowDeleteSalesModel = false;
                        }
                        me.gridSalesModel.clearSelection();
                        me.clearTable(me.gridColourInfo);
                    }

                    if (result.data.recDraftSOColour != null) {
                        me.recordColourCount = result.data.recDraftSOColour.length;
                        me.loadTableData(me.gridColourInfo, result.data.recDraftSOColour);
                    }

                    if (result.data.recDraftSOOthers != null) {
                        me.recordOthersCount = result.data.recDraftSOOthers.length;
                        me.loadTableData(me.gridAccesories, result.data.recDraftSOOthers);
                    }

                    me.AdjustGrid();
                }
                //else {
                //    me.ResetPriceModel();
                //}
            }
            else {
                MsgBox(result.message, MSG_ERROR);
                console.log(result.error_log)
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.PopulateRecordModelColour = function () {
        var params = {
            draftSONo: me.data.DraftSONo,
            salesModelCode: me.recordModel.SalesModelCode,
            salesModelYear: me.recordModel.SalesModelYear,
            colourCode: me.recordColour.ColourCode
        };

        $http.post("om.api/DraftSalesOrder/PopulateRecordModelColour", params)
        .success(function (result) {
            me.ClearRecordVin();
            if (result.success) {
                if (result.data != null) {
                    if (result.data.recDraftSOColour != null) {
                        result.data.recDraftSOColour.ColourDesc = result.ColourDesc;
                        me.RetrieveDataModelColour(result.data.recDraftSOColour);
                    }

                    if (result.data.recDraftSOVin != null) {
                        me.recordVinCount = result.data.recDraftSOVin.length;
                        me.loadTableData(me.gridVinInfo, result.data.recDraftSOVin);
                    }
                }
                else {
                    me.recordColour.Quantity = 0;
                    me.recordColour.Remark = "";
                    me.allowDeleteModelColour = false;
                    me.gridColourInfo.clearSelection();
                    me.clearTable(me.gridVinInfo);
                }
            }
            else {
                MsgBox(result.message, MSG_ERROR);
                console.log(result.error_log)
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.PopulateRecordVin = function () {
        var params = {
            draftSONo: me.data.DraftSONo,
            salesModelCode: me.recordModel.SalesModelCode,
            salesModelYear: me.recordModel.SalesModelYear,
            colourCode: me.recordColour.ColourCode,
            sOSeq: me.recordVin.SeqNo
        };

        $http.post("om.api/DraftSalesOrder/PopulateRecordVin", params)
        .success(function (result) {
            if (result.success) {
                if (result.data != null) {
                    result.data.recDraftSOVin.CityName = result.CityName;
                    result.data.recDraftSOVin.SupplierBBNName = result.SupplierBBNName;
                    me.RetrieveDataVin(result.data.recDraftSOVin);
                    if (me.allowDeleteSalesModel) {
                        me.allowDeleteVin = true;
                    }
                }
                else {
                    me.ClearRecordVin();
                    me.gridVinInfo.clearSelection();
                }
            }
            else {
                MsgBox(result.message, MSG_ERROR);
                console.log(result.error_log)
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

    }

    me.PopulateRecordOthers = function () {
        var params = {
            draftSONo: me.data.DraftSONo,
            salesModelCode: me.recordModel.SalesModelCode,
            salesModelYear: me.recordModel.SalesModelYear,
            otherCode: me.recordOthers.OtherCode
        };

        $http.post("om.api/DraftSalesOrder/PopulateRecordOthers", params)
        .success(function (result) {
            if (result.success) {
                if (result.data != null) {
                    result.data.recDraftSOOthers.AccsName = result.AccsName;

                    me.RetrieveDataOthers(result.data.recDraftSOOthers);
                    if (me.allowDeleteSalesModel) {
                        me.allowDeleteOthers = true;
                    }
                }
                else {
                    me.ClearRecordOthers();
                    me.gridAccesories.clearSelection();
                }
            }
            else {
                MsgBox(result.message, MSG_ERROR);
                console.log(result.error_log)
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

    }

    me.RetriveModelDescription = function () {
        var params = {
            inquiryNumber: me.data.ProspectNo,
            salesModelCode: me.recordModel.SalesModelCode
        };

        $http.post("om.api/DraftSalesOrder/RetriveModelDescription", params)
               .success(function (data) {
                   if (data != null && data != undefined) {
                       me.recordModel.SalesModelDesc = data;
                   }
                   else {
                       me.recordModel.SalesModelDesc = "";
                   }
               })
               .error(function (result) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });
    };

    me.RetriveModelColourDescription = function () {
        var params = {
            salesModelCode: me.recordModel.SalesModelCode,
            colourCode: me.recordColour.ColourCode
        };

        $http.post("om.api/DraftSalesOrder/RetriveModelColourDescription", params)
               .success(function (data) {
                   if (data != null && data != undefined) {
                       me.recordColour.ColourDesc = data;
                   }
                   else {
                       me.recordColour.ColourDesc = "";
                   }
               })
               .error(function (result) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });

    };

    me.RetrieveNameOrDescription = function () {
        var params = {
            customerCode: me.data.CustomerCode,
            salesmanCode: me.data.Salesman,
            topCode: me.data.TOPCode,
            salesModelCode: me.recordModel.SalesModelCode,
            salesModelYear: me.recordModel.SalesModelYear,
            colourCode: me.recordColour.ColourCode,
            supplierBBN: me.recordVin.SupplierBBN,
            cityCode: me.recordVin.CityCode,
            accesoriesCode: me.recordOthers.OtherCode,
            chkType: me.data.chkType,
            inquiryNumber: me.data.ProspectNo
        };

        $http.post("om.api/DraftSalesOrder/RetrieveNameOrDescription", params)
        .success(function (result) {
            if (result.success) {
                me.RetrieveData(result.data);
            }
            else {
                MsgBox(result.message, MSG_ERROR);
                console.log(result.error_log)
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.StatusLabel = function (record) {
        if (record.Status == "0") {
            me.Status = "OPEN";
            me.FormReady(false);
            me.allowInputSalesModel = true;
        }

        if (record.Status == "0" || record.Status == "1") {
            if (record.Status == "1") me.FormReady(true);
            me.allowInputSalesModel = true;
        }
        else if (record.Status == "2" || record.Status == "9") {
            me.allowInputSalesModel = false;
        }
        else {
            me.allowInputSalesModel = false;
        }
        switch (record.Status) {
            case "0":
                me.Status = "OPEN";
                me.FormReady(true);

                //disable cotrol
                $("#btnCustomerCode, #btnSalesman").attr("disabled", "disabled");
                $("#btnApproved, #btnUnApproved").attr("disabled", "disabled");

                //enable control
                $("#btnTOPCode").removeAttr("disabled");
                break;
            case "1":
                me.Status = "PRINTED";
                me.FormReady(true);
                me.CheckBottomPrice();
                //disable cotrol
                $("#btnCustomerCode, #btnSalesman").attr("disabled", "disabled");
                $("#btnUnApproved").attr("disabled", "disabled");

                //enable control
                $("#btnTOPCode").removeAttr("disabled");
                break;
            case "2": me.Status = "APPROVED";
                me.FormReady(false);
                me.allowInput = false;
                me.allowInputSalesModel = false;

                var params = {
                    draftSONo: me.data.DraftSONo
                }
                $http.post("om.api/DraftSalesOrder/CheckingQtySO", params)
                .success(function (result) {
                    if (result.success) {
                        if (result.CheckingQtySO) {
                            $("#btnUnApproved").removeAttr("disabled");
                            $("#btnApproved").attr("disabled", "disabled");
                        }
                        else {
                            $("#btnApproved, #btnUnApproved").attr("disabled", "disabled");
                        }
                    }
                    else {
                        MsgBox(result.message, MSG_WARNING);
                        console.log(result.error_log);
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
                break;
            case "3": me.Status = "DELETED"; me.FormReady(false); me.allowInput = false; $("#btnApproved #btnUnApproved").attr("disabled", "disabled"); break;
            case "4": me.Status = "REJECTED"; me.FormReady(false); me.allowInput = false; $("#btnApproved #btnUnApproved").attr("disabled", "disabled"); break;
            case "9": me.Status = "FINISHED"; me.FormReady(false); me.allowInput = false; $("#btnApproved #btnUnApproved").attr("disabled", "disabled"); break;
            default: me.Status = "NEW"; $("#btnApproved #btnUnApproved, ").attr("disabled", "disabled"); me.allowInput = false; break;
        }

        $("#Status").html(me.Status);
    };

    me.FormReady = function (isReady) {
        if (isReady) {
            $("#chkReffDate, #btnCustomerCode, #btnSalesman").removeAttr("disabled");
            me.allowInput = true;
            me.allowInputDateReff = true;
        }
        else {
            $("#chkReffDate, #btnCustomerCode, #btnSalesman").attr("disabled", "disabled");
        }
    };

    me.CheckBottomPrice = function () {
        var params = {
            draftSONo: me.data.DraftSONo,
            groupPriceCode: me.data.GroupPriceCode
        };

        $http.post("om.api/DraftSalesOrder/CheckBottomPrice", params)
        .success(function (result) {
            var salesModelCodeMessage = "";
            if (result.success) {
                if (result.message != "" && salesModelCodeMessage != message) {
                    MsgBox(message, MSG_WARNING);
                    salesModelCodeMessage = message;
                }
                if (result.allowApprove) {
                    $("#btnApproved").removeAttr("disabled");
                }
                else {
                    $("#btnApproved").attr("disabled", "disabled");
                }
            }
            else {
                MsgBox(result.message, MSG_ERROR);
                console.log(result.error_log)
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.ClearRecordModel = function () {
        me.recordModel = {};
        me.recordModel.SalesModelCode = "";
        me.recordModel.SalesModelYear = "";
        me.recordModel.chkTotal = true;
        me.recordModel.BeforeDiscTotal = 0;
        me.recordModel.DiscIncludePPn = 0;
        me.recordModel.AfterDiscTotal = 0;
        me.recordModel.AfterDiscDPP = 0;
        me.recordModel.AfterDiscPPn = 0;
        me.recordModel.AfterDiscPPnBM = 0;

        me.allowDeleteSalesModel = false;
        me.gridSalesModel.clearSelection();
    };

    me.ClearRecordColour = function () {
        me.recordColour = {};
        me.recordColour.ColourCode = "";
        me.recordColour.ColourDesc = "";
        me.recordColour.Quantity = 0;
        me.recordColour.Remark = "";

        me.allowDeleteModelColour = false;
        me.gridColourInfo.clearSelection();
    };

    me.ClearRecordVin = function () {
        me.recordVin = {};
        me.recordColour.EndUserName = "";
        me.recordVin.SupplierBBN = "";
        me.recordVin.SupplierBBNName = "";
        me.recordVin.CityCode = "";
        me.recordVin.CityName = "";
        me.recordVin.BBN = 0;
        me.recordVin.KIR = 0;
        me.recordVin.Remark = "";
        me.recordVin.SeqNo = 0;

        me.allowDeleteVin = false;
    };

    me.ClearRecordOthers = function () {
        me.recordOthers = {};
        me.recordOthers.OtherCode = "";
        me.recordOthers.AccsName = "";
        me.recordOthers.BeforeDiscTotal = 0;
        me.recordOthers.AfterDiscTotal = 0;
        me.recordOthers.AfterDiscDPP = 0;
        me.recordOthers.AfterDiscPPn = 0;
        me.recordOthers.Remark = "";

        me.allowDeleteOthers = false;
    };

    me.ClearRecordDetails = function () {
        me.ClearRecordModel();
        me.ClearRecordColour();
        me.ClearRecordVin();
        me.ClearRecordOthers();
    };

    me.LoadTableDetails = function () {
        me.clearTable(me.gridSalesModel);
        me.clearTable(me.gridColourInfo);
        me.clearTable(me.gridVinInfo);
        me.clearTable(me.gridAccesories);
        me.AdjustGrid();
    };

    me.ResetPriceModel = function () {
        me.recordModel.SalesModelYear = "";
        me.recordModel.SalesModelDesc = "";
        me.ResetPriceModel_2();
    }

    me.ResetPriceModel_2 = function () {
        me.recordModel.AfterDiscTotal = 0;
        me.recordModel.AfterDiscPPn = 0;
        me.recordModel.AfterDiscPPnBM = 0;
        me.recordModel.AfterDiscDPP = 0;
        me.recordModel.DiscIncludePPn = 0;
        me.recordModel.Remark = "";
        me.TotalPricelistSell();
    }

    me.CodeValidation = function (record) {
        var params = {
            CustomerCode: record.CustomerCode,
            TOPCode: record.TOPCode,
            Salesman: record.Salesman,
            GroupPriceCode: record.GroupPriceCode,
            LeasingCo: record.LeasingCo
        }
        $http.post("om.api/DraftSalesOrder/CodeValidation", params)
        .success(function (data) {
            if (data.length != undefined || data != undefined) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.TOPCode = data.TOPCode;
                me.data.Salesman = data.Salesman;
                me.data.GroupPriceCode = data.GroupPriceCode;
                me.data.LeasingCo = data.LeasingCo;
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.saveData = function () {
        // Validation Before Save 
        if (me.data.chkType) {
            if (me.isLinkToITS)
                if (me.data.ProspectNo == "") return;
        }

        if (me.data.SalesType == "" || me.data.SalesType == undefined || me.data.SalesType == null) {
            MsgBox("Silahkan pilih tipe draft SO terlebih dahulu", MSG_WARNING);
            return;
        }


        if (me.data.RefferenceNo == undefined) me.data.RefferenceNo = "";

        if ((me.data.RefferenceNo != "" && me.data.chkReffDate == false)
            || ((me.data.RefferenceNo == "") && me.data.chkReffDate == true)) {
            MsgBox("No Reff dan Tgl Reff harus diisi", MSG_WARNING);
            return;
        }

        if (me.data.isLeasing) {
            if (me.data.LeasingCo == "") {
                MsgBox("Jika Leasing dicek, maka leasing co harus diisi", MSG_WARNING);
                return;
            }
            if (me.data.Installment == "") {
                MsgBox("Angsuran harus dipilih!", MSG_WARNING);
                return;
            }
        }

        var params = {
            model: me.data,
            chkType: me.data.chkType,
            dtpSO: ($("#DraftSODate").attr("disabled") ? false : true),
            dtpReff: ($("#RefferenceDate").attr("disabled") ? false : true),
            chkLeasing: me.data.isLeasing,
            dtpRequest: ($("#RequestDate").attr("disabled") ? false : true)
        };

        $http.post("om.api/DraftSalesOrder/SaveDraftSO", params)
        .success(function (result) {
            if (result.success) {
                me.RetrieveData(result);
                me.StatusLabel(result);
                Wx.Success(result.message);
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.saveSalesModel = function () {
        if ($(".main form").valid()) {
            me.TotalChecked(me.saveSalesModel_Validated);
        }
    };

    me.saveSalesModel_Validated = function () {
        if (me.recordModel.chkTotal) {
            if (me.recordModel.AfterDiscTotal == undefined) return;
        }
        else {
            if (me.recordModel.AfterDiscDPP == undefined) return;
        }

        if (me.recordModel.AfterDiscTotal <= 0 || me.recordModel.AfterDiscDPP <= 0) {
            MsgBox("Harga Total/DPP tidak boleh kurang atau sama dengan nol", MSG_WARNING);
            return;
        }
        var params = {
            model: me.recordModel,
            draftSONo: me.data.DraftSONo,
            groupPriceCode: me.data.GroupPriceCode,
            customerCode: me.data.CustomerCode,
            discount: me.recordModel.DiscIncludePPn,
            chkType: me.data.chkType
        }

        $http.post("om.api/DraftSalesOrder/SaveDraftSOModel", params)
        .success(function (result) {
            if (result.success) {
                if (result.isNull) {
                    return;
                }
                if (result.isExistDraftSO) {
                    me.PopulateRecord(me.data.DraftSONo);
                    return;
                }
                me.RetrieveData(result);
                me.StatusLabel(result);
                if (result.messageWarning == "") {
                    if (result.messageWarning2 == "") {
                        Wx.Success(result.message);
                    }
                    else {
                        MsgBox(result.messageWarning2, MSG_WARNING);
                    }
                }
                else {
                    MsgBbox(result.messageWarning, MSG_WARNING);
                }

                me.ClearRecordModel();
                me.loadTableData(me.gridSalesModel, result.DraftSOModel);
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.saveSalesModelColour = function () {
        if ($(".main form").valid()) {
            if (me.recordColour.Quantity <= 0) {
                MsgBox("Jumlah tidak boleh kurang atau sama dengan nol", MSG_WARNING);
                return;
            }
            var params = {
                model: me.recordColour,
                draftSONo: me.data.DraftSONo,
                salesModelCode: me.recordModel.SalesModelCode,
                salesModelYear: me.recordModel.SalesModelYear
            }

            $http.post("om.api/DraftSalesOrder/SaveDraftSOModelColour", params)
            .success(function (result) {
                if (result.success) {
                    if (result.allowInput) {
                        me.ClearRecordColour();
                        me.loadTableData(me.gridColourInfo, result.data.DraftSOColour);
                        me.loadTableData(me.gridSalesModel, result.data.DraftSOModel);
                        me.AdjustGrid();

                        me.RetrieveData(result);
                        me.StatusLabel(result);
                        Wx.Success(result.message);
                    }
                    else {
                        if (!result.isNull) {
                            me.PopulateRecord(me.data.DraftSONo);
                            return;
                        }
                        else {
                            return;
                        }
                    }
                }
                else {
                    MsgBox(result.message, MSG_WARNING);
                    console.log(result.error_log);
                }
            })
            .error(function (result) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        }
    };

    me.saveVin = function () {
        if ($(".main form").valid()) {
            var params = {
                model: me.recordVin,
                draftSONo: me.data.DraftSONo,
                salesModelCode: me.recordModel.SalesModelCode,
                salesModelYear: me.recordModel.SalesModelYear,
                colourCode: me.recordColour.ColourCode,
                seqNo: me.recordVin.SeqNo
            }

            $http.post("om.api/DraftSalesOrder/SaveDraftSOVin", params)
            .success(function (result) {
                if (result.success) {
                    if (result.allowInput) {
                        me.ClearRecordVin();
                        me.loadTableData(me.gridVinInfo, result.data.DraftSOVin);
                        me.AdjustGrid();

                        me.RetrieveData(result);
                        me.StatusLabel(result);
                        Wx.Success(result.message);
                    }
                    else {
                        if (!result.isNull) {
                            me.PopulateRecord(me.data.DraftSONo);
                            return;
                        }
                        else {
                            return;
                        }
                    }
                }
                else {
                    MsgBox(result.message, MSG_WARNING);
                    console.log(result.error_log);
                }
            })
            .error(function (result) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        }
    };

    me.saveOthers = function () {
        if ($(".main form").valid()) {
            me.CalculateDiscountOthers(me.saveOthers_Validated);
        }
    };

    me.saveOthers_Validated = function () {
        var params = {
            model: me.recordOthers,
            draftSONo: me.data.DraftSONo,
            salesModelCode: me.recordModel.SalesModelCode,
            salesModelYear: me.recordModel.SalesModelYear,
            otherCode: me.recordOthers.OtherCode
        }

        $http.post("om.api/DraftSalesOrder/SaveDraftSOOthers", params)
        .success(function (result) {
            if (result.success) {
                if (result.allowInput) {
                    me.ClearRecordOthers();
                    me.loadTableData(me.gridAccesories, result.data.DraftSOOthers);
                    me.AdjustGrid();

                    me.RetrieveData(result);
                    me.StatusLabel(result);
                    Wx.Success(result.message);
                }
                else {
                    if (!result.isNull) {
                        me.PopulateRecord(me.data.DraftSONo);
                        return;
                    }
                    else {
                        return;
                    }
                }
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.delete = function () {
        MsgConfirm("Yakin data akan dihapus?", function (e) {
            if (e) {
                var params = {
                    draftSONo: me.data.DraftSONo,
                }
                $http.post("om.api/DraftSalesOrder/DeleteDraftSO", params)
                .success(function (result) {
                    if (result.success) {
                        if (result.isExistDraftSO) {
                            me.ClearRecordModel();
                            me.RetrieveData(result);
                            me.StatusLabel(result);

                            me.clearTable(me.gridSalesModel);
                            Wx.Success(result.message);
                        }
                        else {
                            return;
                        }
                    }
                    else {
                        MsgBox(result.message, MSG_WARNING);
                        console.log(result.error_log);
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
            else {
                me.gridAccesories.clearSelection();
                me.ClearRecordOthers();
                me.Apply();
            }
        });
    }

    me.delete_Validated = function () {
        var params = {
            draftSONo: me.data.DraftSONo
        }
        $http.post("om.api/DraftSalesOrder/DeleteDraftSO_Validated", params)
        .success(function (result) {
            if (result.success) {
                if (result.allowDelete) {
                    me.delete();
                }
                else {
                    me.PopulateRecord(me.data.DraftSONo);
                    return;
                }
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.deleteSalesModel = function () {
        MsgConfirm("Yakin data akan dihapus?", function (e) {
            if (e) {

                var params = {
                    draftSONo: me.data.DraftSONo,
                    salesModelCode: me.recordModel.SalesModelCode,
                    salesModelYear: me.recordModel.SalesModelYear
                }
                $http.post("om.api/DraftSalesOrder/DeleteDraftSOModel", params)
                .success(function (result) {
                    if (result.success) {
                        if (result.isExistDraftSO) {
                            me.ClearRecordModel();
                            me.ClearRecordColour();
                            me.ClearRecordVin();
                            me.ClearRecordOthers()
                            me.clearTable(me.gridColourInfo);
                            me.clearTable(me.gridVinInfo);
                            me.clearTable(me.gridAccesories);

                            me.RetrieveData(result);
                            me.StatusLabel(result);
                            me.loadTableData(me.gridSalesModel, result.DraftSOModel);
                            me.gridSalesModel.clearSelection();
                            Wx.Success(result.message);
                        }
                        else {
                            return;
                        }
                    }
                    else {
                        MsgBox(result.message, MSG_WARNING);
                        console.log(result.error_log);
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
            else {
                me.gridSalesModel.clearSelection();
                me.ClearRecordModel();
                me.ClearRecordColour();
                me.ClearRecordVin();
                me.ClearRecordOthers()
                me.clearTable(me.gridColourInfo);
                me.clearTable(me.gridVinInfo);
                me.clearTable(me.gridAccesories);
                me.Apply();
            }
        });
    }

    me.deleteSalesModel_Validated = function () {
        var params = {
            draftSONo: me.data.DraftSONo
        }
        $http.post("om.api/DraftSalesOrder/DeleteDraftSOModel_Validated", params)
        .success(function (result) {
            if (result.success) {
                if (result.allowDelete) {
                    me.deleteSalesModel();
                }
                else {
                    me.PopulateRecord(me.data.DraftSONo);
                    return;
                }
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.deleteSalesModelColour = function () {
        MsgConfirm("Yakin data akan dihapus?", function (e) {
            if (e) {

                var params = {
                    draftSONo: me.data.DraftSONo,
                    salesModelCode: me.recordModel.SalesModelCode,
                    salesModelYear: me.recordModel.SalesModelYear,
                    colourCode: me.recordColour.ColourCode
                }
                $http.post("om.api/DraftSalesOrder/DeleteDraftSOModelColour", params)
                .success(function (result) {
                    if (result.success) {
                        if (result.isExistDraftSO) {
                            me.ClearRecordColour();
                            me.ClearRecordVin();
                            me.clearTable(me.gridVinInfo);

                            me.RetrieveData(result);
                            me.StatusLabel(result);

                            me.loadTableData(me.gridColourInfo, result.DraftSOModelColour);
                            me.loadTableData(me.gridSalesModel, result.DraftSOModel);
                            me.gridColourInfo.clearSelection();
                            Wx.Success(result.message);
                        }
                        else {
                            return;
                        }
                    }
                    else {
                        MsgBox(result.message, MSG_WARNING);
                        console.log(result.error_log);
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
            else {
                me.gridColourInfo.clearSelection();
                me.ClearRecordColour();
                me.ClearRecordVin();
                me.clearTable(me.gridVinInfo);
                me.Apply();
            }
        });
    }

    me.deleteSalesModelColour_Validated = function () {
        var params = {
            draftSONo: me.data.DraftSONo
        }
        $http.post("om.api/DraftSalesOrder/DeleteDraftSOModelColour_Validated", params)
        .success(function (result) {
            if (result.success) {
                if (result.allowDelete) {
                    me.deleteSalesModelColour();
                }
                else {
                    me.PopulateRecord(me.data.DraftSONo);
                    return;
                }
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.deleteVin = function () {
        MsgConfirm("Yakin data akan dihapus?", function (e) {
            if (e) {
                var params = {
                    draftSONo: me.data.DraftSONo,
                    salesModelCode: me.recordModel.SalesModelCode,
                    salesModelYear: me.recordModel.SalesModelYear,
                    colourCode: me.recordColour.ColourCode,
                    seqNo: me.recordVin.SeqNo
                }
                $http.post("om.api/DraftSalesOrder/DeleteDraftSOVin", params)
                .success(function (result) {
                    if (result.success) {
                        if (result.isExistDraftSO) {
                            me.ClearRecordVin();
                            me.RetrieveData(result);
                            me.StatusLabel(result);

                            me.loadTableData(me.gridVinInfo, result.DraftSOVin);
                            me.loadTableData(me.gridSalesModel, result.DraftSOModel);
                            me.gridVinInfo.clearSelection();
                            Wx.Success(result.message);
                        }
                        else {
                            return;
                        }
                    }
                    else {
                        MsgBox(result.message, MSG_WARNING);
                        console.log(result.error_log);
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
            else {
                me.gridVinInfo.clearSelection();
                me.ClearRecordVin();
                me.Apply();
            }
        });
    }

    me.deleteVin_Validated = function () {
        var params = {
            draftSONo: me.data.DraftSONo
        }
        $http.post("om.api/DraftSalesOrder/DeleteDraftSOVin_Validated", params)
        .success(function (result) {
            if (result.success) {
                if (result.allowDelete) {
                    me.deleteVin();
                }
                else {
                    me.PopulateRecord(me.data.DraftSONo);
                    return;
                }
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.deleteOthers = function () {
        MsgConfirm("Yakin data akan dihapus?", function (e) {
            if (e) {
                var params = {
                    draftSONo: me.data.DraftSONo,
                    salesModelCode: me.recordModel.SalesModelCode,
                    salesModelYear: me.recordModel.SalesModelYear,
                    otherCode: me.recordOthers.OtherCode,
                }
                $http.post("om.api/DraftSalesOrder/DeleteDraftSOOthers", params)
                .success(function (result) {
                    if (result.success) {
                        if (result.isExistDraftSO) {
                            me.ClearRecordOthers();
                            me.RetrieveData(result);
                            me.StatusLabel(result);

                            me.loadTableData(me.gridAccesories, result.DraftSOOthers);
                            me.loadTableData(me.gridSalesModel, result.DraftSOModel);
                            me.gridAccesories.clearSelection();
                            Wx.Success(result.message);
                        }
                        else {
                            return;
                        }
                    }
                    else {
                        MsgBox(result.message, MSG_WARNING);
                        console.log(result.error_log);
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
            else {
                me.gridAccesories.clearSelection();
                me.ClearRecordOthers();
                me.Apply();
            }
        });
    }

    me.deleteOthers_Validated = function () {
        var params = {
            draftSONo: me.data.DraftSONo
        }
        $http.post("om.api/DraftSalesOrder/DeleteDraftSOOthers_Validated", params)
        .success(function (result) {
            if (result.success) {
                if (result.allowDelete) {
                    me.deleteOthers();
                }
                else {
                    me.PopulateRecord(me.data.DraftSONo);
                    return;
                }
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.printPreview = function () {
        var params = {
            draftSONo: me.data.DraftSONo
        }
        $http.post("om.api/DraftSalesOrder/PrintDraftSO", params)
        .success(function (result) {
            if (result.success) {
                if (result.isDataExist) {
                    me.RetrieveData(result);
                    me.StatusLabel(result);
                    me.printPreviewShow();
                }
                else {
                    if (result.message != "") {
                        MsgBox(result.message, MSG_WARNING);
                    }
                    else {
                        return;
                    }
                }
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.printPreviewShow = function () {
        var ReportId = "OmRpSalesTrn013";

        var par = [
            me.data.DraftSONo
        ]
        var rparam = 'Print Draft SO'

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    };

    me.approveDraftSO = function () {
        MsgConfirm("Apakah Anda yakin???", function (e) {
            if (e) {
                var params = {
                    draftSONo: me.data.DraftSONo,
                    isLinkToITS: me.isLinkToITS
                }
                $http.post("om.api/DraftSalesOrder/ApproveDraftSO", params)
                .success(function (result) {
                    if (result.success) {
                        if (result.isExistDraftSO) {
                            me.PopulateRecord(me.data.DraftSONo);
                            if (result.message != "") {
                                Wx.Success(result.message);
                            }
                        }
                        else {
                            return;
                        }
                    }
                    else {
                        MsgBox(result.message, MSG_WARNING);
                        console.log(result.error_log);
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
        });
    }

    me.unApproveDraftSO = function () {
        MsgConfirm("Apakah Anda yakin???", function (e) {
            if (e) {
                var params = {
                    draftSONo: me.data.DraftSONo,
                    isLinkToITS: me.isLinkToITS
                }
                $http.post("om.api/DraftSalesOrder/UnApproveDraftSO", params)
                .success(function (result) {
                    if (result.success) {
                        if (result.isExistDraftSO) {
                            me.PopulateRecord(me.data.DraftSONo);
                            if (result.isUnproved) {
                                Wx.Success(result.message);
                            }
                            else {
                                MsgBox(result.message, WARNING);
                            }
                        }
                        else {
                            return;
                        }
                    }
                    else {
                        MsgBox(result.message, MSG_WARNING);
                        console.log(result.error_log);
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
        });
    }

    me.unApproveDraftSO_Validated = function () {
        var params = {
            draftSONo: me.data.DraftSONo,
        }
        $http.post("om.api/DraftSalesOrder/UnApproveDraftSO_Validated", params)
        .success(function (result) {
            if (result.success) {
                if (result.message == "") {
                    me.unApproveDraftSO();
                }
                else {
                    MsgBox(result.message, WARNING);
                    return;
                }
            }
            else {
                MsgBox(result.message, MSG_WARNING);
                console.log(result.error_log);
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    //
    me.TotalPricelistSell = function () {
        var params = {
            groupPriceCode: me.data.GroupPriceCode,
            salesModelCode: me.recordModel.SalesModelCode,
            salesModelYear: me.recordModel.SalesModelYear
        };
        $http.post("om.api/DraftSalesOrder/TotalPricelistSell", params)
        .success(function (data) {
            if (data != undefined) {
                me.recordModel.BeforeDiscTotal = data;
                me.delayEditing();
            }
            else {
                me.recordModel.BeforeDiscTotal = 0;
                me.delayEditing();
            }
        })
        .error(function (result) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    };

    me.TotalChecked = function (callback) {
        if (me.recordModel.SalesModelCode != "" || me.recordModel.SalesModelYear != "") {
            if (me.recordModel.chkTotal) {
                var params = {
                    model: me.recordModel,
                    chkTotal: me.recordModel.chkTotal,
                    groupPriceCode: me.data.GroupPriceCode,
                    customerCode: me.data.CustomerCode,
                };
                $http.post("om.api/DraftSalesOrder/TotalChecked", params)
                .success(function (data) {
                    if (data != undefined || data.length != undefined) {
                        if (callback != undefined) {
                            callback();
                        }
                        else {
                            me.RetrieveDataModel(data);
                        }
                    }
                })
                .error(function (result) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
            }
            else {
                if (callback != undefined) {
                    callback();
                }
                else {
                    me.RetrieveDataModel(data);
                }
            }
        }
    };

    me.AfterDiscPPn_Validated = function (e) {
        if (me.recordModel.SalesModelCode != "" || me.recordModel.SalesModelYear != "") {
            var totalPriceBeforeDisc = me.recordModel.BeforeDiscTotal * 1;
            var dpp = me.recordModel.AfterDiscDPP * 1;
            var ppnAfter = me.recordModel.AfterDiscPPn * 1;
            var ppnBMAfter = me.recordModel.AfterDiscPPnBM * 1;

            var totalPrice = dpp + ppnAfter + ppnBMAfter;
            var disc = totalPriceBeforeDisc - totalPrice;
            me.recordModel.AfterDiscPPn = ppnAfter;
            me.recordModel.AfterDiscPPnBM = ppnBMAfter;
            me.recordModel.AfterDiscTotal = totalPrice;
            me.recordModel.DiscIncludePPn = disc;
            me.delayEditing();
            me.Apply();
        }
    };

    me.CalculateDiscountOthers = function (callback) {
        if ($(".main form").valid()) {
            var params = {
                model: me.recordOthers,
                customerCode: me.data.CustomerCode,
            };
            $http.post("om.api/DraftSalesOrder/CalculateDiscountOthers", params)
            .success(function (data) {
                if (data != undefined || data.length != undefined) {
                    if (callback != undefined) {
                        callback();
                    }
                    else {
                        me.RetrieveDataOthers(data);
                    }
                }
            })
            .error(function (result) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        }
    };

    me.AdjustGrid = function () {
        me.gridSalesModel.adjust();
        me.gridColourInfo.adjust();
        me.gridVinInfo.adjust();
        me.gridAccesories.adjust();
    };

    me.ClickModelTab = function () {
        //if (me.recordColourCount < 1) {
        //    me.clearTable(me.gridColourInfo);
        //}
        //if (me.recordVinCount < 1) {
        //    me.clearTable(me.gridVinInfo);
        //}

        me.AdjustGrid();
    };

    me.ClickVevicleTab = function () {
        if (me.recordColourCount < 1) {
            me.clearTable(me.gridColourInfo);
        }
        if (me.recordVinCount < 1) {
            me.clearTable(me.gridVinInfo);
        }

        me.AdjustGrid();
    };

    me.ClickAccesoriesTab = function () {
        if (me.recordOthersCount < 1) {
            me.clearTable(me.gridAccesories);
        }

        me.AdjustGrid();
    };

    me.initialize = function () {
        me.delayInit();
        var lookupParams = {
            CodeID: "ITSFL",
            lookUpValue: "STATUS"
        };
        $http.post("om.api/combo/LoadComboData2", lookupParams)
        .success(function (result) {
            if (result.length > 0) {
                if (result[0]["value"] == "1") {
                    me.isLinkToITS = true;
                    $("#TipeKendaraan").css({ "visibility": "visible" });
                    $("label[for='TipeKendaraan']").css({ "visibility": "visible" });
                }
                else {
                    me.isLinkToITS = false;
                    $("#TipeKendaraan").css({ "visibility": "hidden" });
                    $("label[for='TipeKendaraan']").css({ "visibility": "hidden" });
                }
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

        lookupParams = {
            CodeID: "DSOT"
        };

        $http.post("om.api/combo/LoadComboData", lookupParams)
        .success(function (result) {
            me.dsSalesType = result;
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

        lookupParams = {
            CodeID: "INST"
        };

        $http.post("om.api/combo/LoadComboData", lookupParams)
        .success(function (result) {
            me.dsTenor = result;
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

        me.allowInput = true;
        me.allowInputDateReff = true;

        me.allowInputSalesModel = false;
        me.allowDeleteSalesModel = false;
        me.allowDeleteSalesModelColour = false;
        me.allowDeleteVin = false;
        me.allowDeleteOthers = false;

        var initData = {
            SalesType: 1,
            chkType: true,
            chkReffDate: false,
            chkRequestDate: true,
            isLeasing: false,
            DraftSODate: currentDate,
            RefferenceDate: currentDate,
            FinalPaymentDate: currentDate,
            RequestDate: currentDate,
            CommissionAmt: 0,
            dsSalesType: me.loadSalesType
        }

        me.RetrieveData(initData);
        me.FormReady(true);

        //Draft Order Detail
        me.ClearRecordDetails();

        me.recordColourCount = 0;
        me.recordVinCount = 0;
        me.recordOthersCount = 0;

        me.Status = "NEW";
        me.clearTable(me.gridSalesModel);

        $("#btnCustomer").removeAttr("disabled");
        $("#btnApproved, #btnUnApproved").attr("disabled", "disabled");

        $('#Status').html(me.Status);
        $('#Status').css({
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });
        $("div [data-id=tabSPUDetail] p:first").addClass("ng-pristine ng-valid active");
        $("#tabPageSalesModel .divider:first").hide();
        $("#tabPageVehicleInfo .divider:first").hide();
        $("#tabPageAccesories .divider:first").hide();

        $("#AfterDiscTotal, #AfterDiscDPP").on("blur", function () {
            me.TotalChecked();
        });
        $("#AfterDiscPPn, #AfterDiscPPnBM").on("keyup", function (event) {
            me.AfterDiscPPn_Validated(event);
        });

        $("#AfterDiscTotalOthers").on("blur", function () {
            me.CalculateDiscountOthers();
        });
    }

    me.RetrieveData = function (value) {
        if (value == null || value == undefined) return;

        me.isLoadData = true;
        setTimeout(function () {
            me.hasChanged = false;
            me.startEditing();
            me.isSave = true;

            me.ReformatNumber();
            var selectorContainer = "";
            $.each(value, function (key, val) {
                var ctrl = $(selectorContainer + " [name=" + key + "]");
                me.data[key] = val;

                ctrl.removeClass("error");
            });

            $scope.$apply();
            me.isSave = false;
        }, 200);
    }

    me.RetrieveDataModel = function (value) {
        if (value == null || value == undefined) return;

        var selectorContainer = "";
        $.each(value, function (key, val) {
            var ctrl = $(selectorContainer + " [name=" + key + "]");
            me.recordModel[key] = val;

            ctrl.removeClass("error");
        });

        me.delayEditing();
    };

    me.RetrieveDataModelColour = function (value) {
        if (value == null || value == undefined) return;

        var selectorContainer = "";
        $.each(value, function (key, val) {
            var ctrl = $(selectorContainer + " [name=" + key + "]");
            me.recordColour[key] = val;

            ctrl.removeClass("error");
        });

        me.delayEditing();
    };

    me.RetrieveDataVin = function (value) {
        if (value == null || value == undefined) return;

        var selectorContainer = "";
        $.each(value, function (key, val) {
            var ctrl = $(selectorContainer + " [name=" + key + "]");
            me.recordVin[key] = val;

            ctrl.removeClass("error");
        });

        me.delayEditing();
    };

    me.RetrieveDataOthers = function (value) {
        if (value == null || value == undefined) return;

        var selectorContainer = "";
        $.each(value, function (key, val) {
            var ctrl = $(selectorContainer + " [name=" + key + "]");
            me.recordOthers[key] = val;

            ctrl.removeClass("error");
        });

        me.delayEditing();
    };

    me.stdChangedMonitoring = function (n, o) {
        if (me.Status == "CANCELED" || me.Status == "DELETED") {
            me.hasChanged = false;
            me.isSave = true;
            $("#btnDelete").hide();
            me.isPrintAvailable = false;
        }
        else {
            if (me.Status === "APPROVED") {
                $("#btnDelete").hide();
            }
            else {
                $("#btnDelete").show();
            }

            if (me.isLoadData) {
                me.isPrintAvailable = true;
            }

            if (!me.isInProcess) {
                var eq = (n == o);

                // check apakah perubahan data tersebut memiliki nilai atau object kosong (empty object)
                if (!(_.isEmpty(n)) && !eq) {
                    if (!me.hasChanged && !me.isLoadData) {
                        me.hasChanged = true;
                        me.isLoadData = false;
                    }
                    if (!me.isSave) {
                        me.isSave = true;
                        me.hasChanged = true;
                        me.isLoadData = false;
                    }
                } else {
                    me.hasChanged = false;
                    me.isSave = false;
                }
            }
        }
    }

    me.delayEditing = function () {
        setTimeout(function () {
            me.isInProcess = false;
            me.ReformatNumber();
        }, 1);
    };

    me.delayInit = function () {
        setTimeout(function () {
            me.isInProcess = false;
        }, 500);
    };

    webix.event(window, "resize", function () {
        me.gridSalesModel.adjust();
        me.gridColourInfo.adjust();
        me.gridVinInfo.adjust();
        me.gridAccesories.adjust();
    });

    me.gridSalesModel = new webix.ui({
        container: "wxSalesModel",
        view: "wxtable", css: "alternating",
        scrollX: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 140 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 140 },
            { id: "QuantityDraftSO", header: "Jml. Drfat SO.", width: 120, format: me.intFormat, css: "text-right" },
            { id: "AfterDiscTotal", header: "Harga Setelah Diskon", width: 160, format: me.intFormat, css: "text-right" },
            { id: "AfterDiscDPP", header: "DPP Stelah Diskon", width: 160, format: me.intFormat, css: "text-right" },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 160, format: me.intFormat, css: "text-right" },
            { id: "AfterDiscPPnBM", header: "PPnBM Setalah Diskon", width: 160, format: me.intFormat, css: "text-right" },
            { id: "VinDPP", header: "DPP Lain-lain", width: 160, format: me.intFormat, css: "text-right" },
            { id: "VinPPn", header: "PPn Lain-lain", width: 160, format: me.intFormat, css: "text-right" },
            { id: "ShipAmt", header: "Onkos Kirim", width: 160, format: me.intFormat, css: "text-right" },
            { id: "DepositAmt", header: "Deposit", width: 140, format: me.intFormat, css: "text-right" },
            { id: "VinAmt", header: "Lain-lain", width: 140, format: me.intFormat, css: "text-right" },
            { id: "Remark", header: "Keterangan", width: 200, format: me.replaceNull },
            { id: "BeforeDiscTotal", header: "Harga Sebelum Diskon", width: 300, format: me.intFormat, css: "text-right" },
            { id: "DiscIncludePPn", header: "Diskon", width: 150, format: me.intFormat, css: "text-right" },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridSalesModel.getSelectedId() !== undefined) {
                    var rec = this.getItem(me.gridSalesModel.getSelectedId().id);
                    me.recordModel.SalesModelCode = rec.SalesModelCode;
                    me.recordModel.SalesModelYear = rec.SalesModelYear;
                    me.PopulateRecordModel();
                    if (me.allowInputSalesModel) {
                        me.allowDeleteSalesModel = true;
                    }
                }
            }
        }
    });

    me.gridColourInfo = new webix.ui({
        container: "wxColourInfo",
        view: "wxtable", css: "alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 160,
        //width: 400,
        columns: [
            { id: "ColourCode", header: "Warna", width: 160 },
            { id: "Quantity", header: "Jumlah", width: 100, format: me.intFormat, css: "text-right" },
            { id: "Remark", header: "Keterangan", width: 300 }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridColourInfo.getSelectedId() !== undefined) {
                    var rec = this.getItem(me.gridColourInfo.getSelectedId().id);
                    me.recordColour.ColourCode = rec.ColourCode;
                    if (me.allowInputSalesModel) {
                        me.allowDeleteModelColour = true;
                    }
                    me.PopulateRecordModelColour();
                }
            }
        }
    });

    me.gridVinInfo = new webix.ui({
        container: "wxVinInfo",
        view: "wxtable", css: "alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 160,
        width: 520,
        columns: [
            { id: "EndUserName", header: "Nama STNK", width: 140 },
            { id: "SupplierBBN", header: "Pemasok BBN", width: 140 },
            { id: "CityCode", header: "Kota", width: 120 },
            { id: "BBN", header: "BBN", width: 120, format: me.intFormat, css: "text-right" },
            { id: "KIR", header: "KIR", width: 120, format: me.intFormat, css: "text-right" },
            { id: "Remark", header: "Keterangan", width: 200, format: me.replaceNull }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridVinInfo.getSelectedId() !== undefined) {
                    var rec = this.getItem(me.gridVinInfo.getSelectedId().id);
                    me.recordVin.SeqNo = rec.SOSeq;
                    me.PopulateRecordVin();
                }
            }
        }
    });

    me.gridAccesories = new webix.ui({
        container: "wxAccesories",
        view: "wxtable", css: "alternating",
        scrollX: true,
        columns: [
            { id: "OtherCode", header: "Kode Aks. Lain", width: 140 },
            { id: "AccsName", header: "Nama Aks. Lain", width: 200 },
            { id: "BeforeDiscTotal", header: "Total Sebelum Diskon", width: 160, format: me.intFormat, css: "text-right" },
            { id: "AfterDiscTotal", header: "Total Setelah Diskon", width: 160, format: me.intFormat, css: "text-right" },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", width: 160, format: me.intFormat, css: "text-right" },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 160, format: me.intFormat, css: "text-right" },
            { id: "Remark", header: "Keterangan", width: 250, format: me.replaceNull }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridAccesories.getSelectedId() !== undefined) {
                    var rec = this.getItem(me.gridAccesories.getSelectedId().id);
                    me.recordOthers.OtherCode = rec.OtherCode;
                    me.PopulateRecordOthers();
                }
            }
        }
    });

    me.start();
};

$(document).ready(function () {
    var options = {
        title: "Draft Sales Order",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlStatus",
                cls: "span8",
                items: [
                    { name: "SalesType", model: "data.SalesType", text: "Tipe Draft SO", cls: "span3", type: "select2", datasource: "dsSalesType" },
                    { name: "Status", cls: "span2", text: "", readonly: true, type: "label" },
                    {
                        type: "buttons", cls: "span3", items: [
                            {
                                name: "btnApproved", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approveDraftSO()"
                            },
                            {
                                name: "btnUnApproved", text: "UnApprove", cls: "btn btn-info", icon: "icon-ban-circle", click: "unApproveDraftSO_Validated()"
                            }
                        ]
                    }
                ]
            },
            {
                name: "pnlSPU",
                title: " ",
                items: [
                    { name: "DraftSONo", model: "data.DraftSONo", text: "No. Draft SO", cls: "span4", readonly: true, placeHolder: 'SPU/XX/YYYYYY' },
                    {
                        type: "controls",
                        text: "Tgl. Draft SO",
                        cls: "span4",
                        items: [
                            { name: "DraftSODate", model: "data.DraftSODate", text: "Tgl. Draft SO", cls: "span5", type: "ng-datepicker", disable: true },
                            {
                                type: "buttons", cls: "span1", items: [
                                    { name: "btnCustomer", text: " Pelanggan", cls: "btn btn-success", icon: "icon-user", click: "CustomerShow()" }
                                ]
                            }
                        ]
                    },
                    { name: "RefferenceNo", model: "data.RefferenceNo", text: "No. Reff", cls: "span4", maxlength: 15, disable: "!allowInput" },
                    {
                        text: "Tgl. Reff",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "chkReffDate", model: "data.chkReffDate", cls: "span1", type: "ng-check" },
                            {
                                name: "RefferenceDate", model: "data.RefferenceDate", cls: "span7 right", type: 'ng-datepicker',
                                style: "width: 344px;", disable: "!data.chkReffDate || !allowInputDateReff"
                            },
                        ]
                    },
                    {
                        text: "Direct Sales",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "chkType", model: "data.chkType", cls: "span1", type: "ng-check", disable: "!isCheckType || allowInput" },
                            { type: "label", text: "No. ITS", cls: "span1", style: "line-height: 30px;" },
                            {
                                name: "ProspectNo", model: "data.ProspectNo", placeHolder: "No. ITS", cls: "span6 right", type: "popup",
                                click: "BrowseITS()", required: true, validasi: "required", readonly: true, show: "isLinkToITS"
                            },
                            {
                                name: "ProspectNo", model: "data.ProspectNo", placeHolder: "No. ITS", cls: "span6 right", type: "numeric",
                                show: "!isLinkToITS", maxlength: 15, style: "width: 294px;"
                            },
                        ]
                    },
                    { name: "TipeKendaraan", model: "data.TipeKendaraan", text: "Tipe Kendaraan", cls: "span4", readonly: true },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            {
                                name: "CustomerCode", model: "data.CustomerCode", placeHolder: "Pelanggan", cls: "span3", type: "popup",
                                click: "BrowseCustomer()", readonly: true, required: true, validasi: "required"
                            },
                            { name: "CustomerName", model: "data.CustomerName", placeHolder: "Nama Pelanggan", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "TOP",
                        type: "controls",
                        cls: "span4",
                        items: [
                            {
                                name: "TOPCode", model: "data.TOPCode", placeHolder: "TOP", cls: "span3", type: "popup", click: "BrowseTOP()",
                                readonly: true, required: true, validasi: "required", disable: "!allowInput"
                            },
                            { name: "TOPInterval", model: "data.TOPInterval", placeHolder: "TOP Interval", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Salesman",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            {
                                name: "Salesman", model: "data.Salesman", placeHolder: "Selasman", cls: "span3", type: "popup",
                                click: "BrowseSalesman()", readonly: true, required: true, validasi: "required"
                            },
                            { name: "SalesName", model: "data.SalesName", placeHolder: "Nama Selasman", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Group Price Code",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "GroupPriceCode", model: "data.GroupPriceCode", placeHolder: "Group Price Code", cls: "span3", readonly: true },
                            { name: "GroupPriceDesc", model: "data.GroupPriceDesc", placeHolder: "Group Price Desc", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Leasing",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "chkisLeasing", model: "data.isLeasing", cls: "span1", type: "ng-check", disable: "!allowInput" },
                            { name: "LeasingName", model: "data.LeasingName", placeHolder: "Nama Leasing", cls: "span4 right", readonly: true },
                            {
                                name: "LeasingCo", model: "data.LeasingCo", placeHolder: "Leasing", cls: "span3 right", type: "popup",
                                click: "BrowseLeasing()", readonly: true, disable: "!data.isLeasing", required: true, validasi: "required"
                            },
                        ]
                    },
                    {
                        text: "Angsur./Bln & Tgl.Lunas",
                        type: "controls",
                        cls: "span4",
                        items: [
                            {
                                name: "Installment", model: "data.Installment", opt_text: "-- SELECT--", cls: "span3", type: "select2",
                                text: "", datasource: "dsTenor", disable: "!data.isLeasing", required: true, validasi: "required"
                            },
                            { name: "FinalPaymentDate", model: "data.FinalPaymentDate", cls: "span5", type: 'ng-datepicker', disable: "!data.isLeasing" },
                        ]
                    },
                    { name: "CommissionAmt", model: "data.CommissionAmt", text: "Komisi", placeHolder: "0", cls: "span4 number-int", min: 0, disable: "!allowInput" },
                    {
                        text: "Tgl. Dibutuhkan",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "chkRequestDate", model: "data.chkRequestDate", cls: "span1", type: "ng-check", disable: "!allowInput" },
                            { name: "RequestDate", model: "data.RequestDate", cls: "span7 right", type: 'ng-datepicker', disable: "!data.chkRequestDate", style: "width: 344px;" },
                        ]
                    },
                    { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span8", maxlength: 100, disable: "!allowInput" },
                ]
            },
            // Detail SPU
            {
                xtype: "tabs",
                name: "tabSPUDetail",
                items: [
                    { name: "tabPageSalesModel", text: "Sales Model", cls: "active", click: "ClickModelTab()" },
                    { name: "tabPageVehicleInfo", text: "Informasi Kendaraan", click: "ClickVevicleTab()" },
                    { name: "tabPageAccesories", text: "Aksesoris", click: "ClickAccesoriesTab()" },
                ],
            },
            // Tab Sales Model
            {
                name: "tabPageSalesModel",
                cls: "tabSPUDetail tabPageSalesModel",
                items: [
                    {
                        name: "SalesModelCode", model: "recordModel.SalesModelCode", text: "Sales Model Code", cls: "span3",
                        placeHolder: "Sales Model Code", readonly: true, type: "popup", click: "BrowseModel()", required: true, disable: "!allowInputSalesModel"
                    },
                    {
                        type: "controls",
                        text: "Sales Model Year",
                        cls: "span5",
                        items: [
                            {
                                name: "SalesModelYear", model: "recordModel.SalesModelYear", placeHolder: "Sales Model Year",
                                cls: "span1", type: "popup", required: true, readonly: true, click: "BrowseModelYear()", disable: "!allowInputSalesModel"
                            },
                            {
                                name: "SalesModelDesc", model: "recordModel.SalesModelDesc", placeHolder: "Sales Model Desc",
                                cls: "span4", readonly: true, style: "width: 263px;"
                            },
                        ]
                    },
                    { type: "label", text: "Harga Sebelum Diskon", style: "font-size: 14px; color : blue;" },
                    { type: "div", cls: "divider span3 full" },
                    { name: "BeforeDiscTotal", model: "recordModel.BeforeDiscTotal", text: "Harga Total", placeHolder: "0", cls: "span3 number-int", readonly: true },
                    {
                        type: "controls",
                        text: "Diskon",
                        cls: "span5",
                        items: [
                            { name: "DiscIncludePPn", model: "recordModel.DiscIncludePPn", text: "Diskon", cls: "span2 number-int", placeHolder: "0", readonly: true },
                            { type: "label", text: "Keterangan", cls: "span1", maxlength: 100, style: "line-height: 30px;" },
                            {
                                name: "Remark", model: "recordModel.Remark", text: "Keterangan", cls: "span4 right",
                                style: "width: 294px;", maxlength: 100, disable: "!allowInputSalesModel"
                            },
                        ]
                    },
                    { type: "label", text: "Harga Setelah Diskon", style: "font-size: 14px; color : blue;" },
                    { type: "div", cls: "divider span3 full" },
                    {
                        type: "controls",
                        text: "Harga Total",
                        cls: "span3",
                        items: [
                            { name: "chkTotal", model: "recordModel.chkTotal", cls: "span1 number-int", placeHolder: "0", type: "ng-check", disable: "!allowInputSalesModel" },
                            {
                                name: "AfterDiscTotal", model: "recordModel.AfterDiscTotal", text: "0", cls: "span7 number-int right",
                                disable: "!recordModel.chkTotal || !allowInputSalesModel", required: true, style: "width: 210px"
                            },
                        ]
                    },
                    {
                        type: "controls",
                        text: "DPP",
                        cls: "span5",
                        items: [
                            { name: "AfterDiscDPP", model: "recordModel.AfterDiscDPP", cls: "span2 number-int", placeHolder: "0", disable: "recordModel.chkTotal", required: true },
                            { type: "label", text: "PPN", cls: "span1", style: "line-height: 30px;" },
                            { name: "AfterDiscPPn", model: "recordModel.AfterDiscPPn", cls: "span2 number-int", placeHolder: "0", disable: "recordModel.chkTotal" },
                            { type: "label", text: "PPnBM", cls: "span1", style: "line-height: 30px;" },
                            {
                                name: "AfterDiscPPnBM", model: "recordModel.AfterDiscPPnBM", cls: "span2 number-int", placeHolder: "0",
                                disable: "recordModel.chkTotal", style: "width: 163px;"
                            },
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "saveSalesModel()", disable: "!allowInputSalesModel" },
                            {
                                name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger",
                                click: "deleteSalesModel_Validated()", disable: "!allowDeleteSalesModel"
                            },
                        ]
                    },
                    {
                        name: "wxSalesModel",
                        cls: "tabSPUDetail tabPageSalesModel",
                        type: "wxdiv"
                    },
                ]
            },
            // Tab Informasi Kendaraan
            {
                name: "tabPageVehicleInfo",
                cls: "tabSPUDetail tabPageVehicleInfo",
                items: [
                    { type: "label", text: "Detil Warna", cls: "span4", style: "font-size: 14px; color : blue;" },
                    { type: "label", text: "Detil Lain-lain", cls: "span4", style: "font-size: 14px; color : blue;" },
                    { type: "div", cls: "divider span3" },
                    { type: "separator", cls: "span1" },
                    { type: "div", cls: "divider span4" },
                    { name: "ColourCode", model: "recordColour.ColourCode", text: "Warna", cls: "span3", type: "popup", click: "BrowseColour()", readonly: true, required: true, disable: "!allowDeleteSalesModel" },
                    { name: "EndUserName", model: "recordVin.EndUserName", text: "STNK", cls: "span5", required: true, disable: "!allowDeleteModelColour", maxlength: 41 },
                    { name: "ColourDesc", model: "recordColour.ColourDesc", text: "", cls: "span3", readonly: true },
                    {
                        text: "Pemasok BBN",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "SupplierBBN", model: "recordVin.SupplierBBN", text: "Pemasok BBN", cls: "span3", type: "popup", click: "BrowseBBN()", readonly: true, disable: "!allowDeleteModelColour" },
                            { name: "SupplierBBNName", model: "recordVin.SupplierBBNName", text: "", cls: "span5", readonly: true },
                        ]
                    },
                    { name: "Quantity", model: "recordColour.Quantity", text: "Jumlah", cls: "span3 number-int", placeHolder: "0" },
                    {
                        text: "Kota",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "CityCode", model: "recordVin.CityCode", text: "Kota", cls: "span3", type: "popup", click: "BrowseCity()", readonly: true, disable: "!allowDeleteModelColour" },
                            { name: "CityName", model: "recordVin.CityName", text: "", cls: "span5", readonly: true },
                        ]
                    },
                    { name: "Remark", model: "recordColour.Remark", text: "Keterangan", cls: "span3", type: "textarea", maxlength: 100, style: "min-height: 70px; max-height: 70px; max-width: 230px;" },
                    {
                        text: "BBN",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "BBN", model: "recordVin.BBN", cls: "span3 number-int", text: "BBN" },
                            { type: "label", cls: "span2", text: "KIR", style: "line-height: 30px; text-align: right;" },
                            { name: "KIR", model: "recordVin.KIR", cls: "span3 number-int", text: "KIR" },
                        ]
                    },
                    { name: "Remark", model: "recordVin.Remark", cls: "span4", text: "Keterangan", maxlength: 100 },
                    { name: "SeqNo", model: "recordVin.SeqNo", cls: "span3 full", text: "SeqNo", type: "hidden" },
                    {
                        type: "buttons",
                        cls: "span3",
                        items: [
                            { name: "btnAddColour", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "saveSalesModelColour()", disable: "!allowDeleteSalesModel" },
                            { name: "btnDeleteColour", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteSalesModelColour_Validated()", disable: "!allowDeleteModelColour" },
                        ]
                    },
                    {
                        type: "buttons",
                        cls: "span5",
                        items: [
                            { name: "btnAddVin", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "saveVin()", disable: "!allowDeleteModelColour" },
                            { name: "btnDeleteVin", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteVin_Validated()", disable: "!allowDeleteVin" },
                        ]
                    },
                    {
                        name: "wxColourInfo",
                        cls: "span3",
                        type: "wxdiv"
                    },
                    { type: "sparator", cls: "span1" },
                    {
                        name: "wxVinInfo",
                        cls: "span4",
                        type: "wxdiv"
                    },
                ]
            },
            // Tab Aksesoris
            {
                name: "tabPageAccesories",
                cls: "tabSPUDetail tabPageAccesories",
                items: [
                    {
                        type: "controls",
                        cls: "span8",
                        text: "Aks. Lain-lain",
                        required: true,
                        items: [
                            { name: "OtherCode", model: "recordOthers.OtherCode", text: "Aks. Lain-lain", cls: "span2", type: "popup", click: "BrowseAccesories()", readonly: true, required: true, disable: "!allowDeleteSalesModel" },
                            { name: "AccsName", model: "recordOthers.AccsName", text: "", cls: "span6", readonly: true, required: true },
                        ]
                    },
                    { type: "label", text: "Harga Sebelum Diskon", cls: "span4", style: "font-size: 14px; color : blue;" },
                    { type: "label", text: "Harga Setelah Diskon", cls: "span4", style: "font-size: 14px; color : blue;" },
                    { type: "div", cls: "divider span3" },
                    { type: "sparator", cls: "span1" },
                    { type: "div", cls: "divider span4" },
                    { name: "BeforeDiscTotalOthers", model: "recordOthers.BeforeDiscTotal", text: "Total", cls: "span3 number-int", placeHolder: "0", required: true },
                    {
                        type: "controls",
                        cls: "span5",
                        text: "Total",
                        required: true,
                        items: [
                            { name: "AfterDiscTotalOthers", model: "recordOthers.AfterDiscTotal", text: "Total", cls: "span3 number-int", placeHolder: "0", required: true },
                            { type: "label", text: "DPP", cls: "span1", style: "line-height: 30px;" },
                            { name: "AfterDiscDPPOthers", model: "recordOthers.AfterDiscDPP", cls: "span2 number-int", placeHolder: "0", readonly: true },
                            { type: "label", text: "PPn", cls: "span1", style: "line-height: 30px;" },
                            { name: "AfterDiscPPnOthers", model: "recordOthers.AfterDiscPPn", cls: "span2 number-int", placeHolder: "0", readonly: true },
                        ]
                    },
                    { name: "RemarkOthers", model: "recordOthers.Remark", text: "Keterangan", cls: "span8", maxlength: 100 },
                    {
                        type: "buttons",
                        cls: "span5",
                        items: [
                            { name: "btnAddAccesories", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "saveOthers()", disable: "!allowDeleteSalesModel" },
                            { name: "btnDeleteOAccesories", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteOthers_Validated()", disable: "!allowDeleteOthers" },
                        ]
                    },

                    {
                        name: "wxAccesories",
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
        SimDms.Angular("omDraftSOController");
    }
});

