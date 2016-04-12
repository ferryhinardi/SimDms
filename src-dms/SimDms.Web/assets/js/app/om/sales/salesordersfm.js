var totalSrvAmt = 0;
var status = 'N';
var svType = '2';
var taxPct = 0;
var ptype = '';

"use strict";

function omSalesOrderSFMController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.enableHdr();
        me.enableDtlSalesModelColour();
        me.enableDtlSalesVin();
        me.enableDtlSalesModelOther();
        me.enableDtlSalesAccs();
        me.datas = {};
        me.detail = {};
        me.detail2 = {};
        me.detail3 = {};
        me.detail4 = {};
        me.detail5 = {};
        me.isdetail = true;
        me.isdetail2 = true;
        me.isdetail3 = true;
        me.isdetail4 = true;
        me.isdetail5 = true;
        me.isData = true;
        me.data.isC1 = false;
        me.data.isC2 = true;
        me.data.isC3 = false;
        me.data.isC4 = false;
        me.isShow = true;
        me.isDisable = true;
        me.data.isLeasing = false;
        me.detail.chkTotalPriceAfter == true;
        me.detail.AfterDiscDPP = 0;
        me.data.ProspectNo = "";
        //me.detail5.QtyUnit = 3;
        me.tabDetail = "tabPageSalesModel";
        me.data.SODate = me.now();
        me.gridSalesModel.adjust();
        me.clearTable(me.gridSalesModel);
        me.clearTable(me.gridColourInfo);
        me.clearTable(me.gridOthersInfo);
        me.clearTable(me.gridAccesories);
        me.clearTable(me.gridSparepart);
        $('#statusLbl').removeAttr('ng-model');
        $http.post('om.api/SalesOrder/Default').
          success(function (dl, status, headers, config) {
              me.datas.ProfitCenterCode = dl.ProfitCenterCode;
              me.datas.isITSFL = dl.isITSFL;
              me.datas.isDSOL = dl.isDSOL;
              me.data.WareHouseCode = dl.WareHouseCode;
              me.data.ptype = dl.ProductType;
              ptype = dl.ProductType;
              console.log(me.data.ptype);
              $('#WareHouseCode').val(dl.WareHouseCode);
              $('#WareHouseName').val(dl.WareHouseName);
              if (dl.isDSOL == "0") {
                  $('#btnSKPKNo').remove();
              }
              if (dl.isITSFL == "0") {
                  $('#btnProspectNo').remove();
                  $('#ProspectNo').removeClass("error");
                  $('#ProspectNo').removeAttr("data-validate");
                  $('#label1').remove();
                  $('#VehicleType').remove();
              } //else me.isReq = "required";
          });
        //$http.post('om.api/TransferOut/TransferOutDefault', me.detail).
        //  success(function (data, status, headers, config) {
        //     if (data.success) {
        //         $('#WareHouseCode').val(data.data.WareHouseCodeFrom);
        //        $('#WareHouseName').val(data.data.WareHouseNameFrom);
        //    } 
        // });
        $('#RefferenceNo').removeAttr('disabled');
        $('#btnUnapprove').attr('disabled', true);
        $('#btnApprove').attr('disabled', true);
        $('#btnReject').attr('disabled', true);
        $('#btnPelanggan').removeAttr('disabled');
        $(".panel.tabDetail.tabPageSalesModel").show();
        $(".panel.tabDetail.tabPageVehicleInfo").hide();
        $(".panel.tabDetail.tabPageAccesories").hide();
        $("p[data-name='tabPageVehicleInfo']").removeClass('active');
        $("p[data-name='tabPageAccesories']").removeClass('active');
        $("p[data-name='tabPageSalesModel']").addClass('active');
        $('#chkTotalPriceAfter').prop('checked', true);
        $('#statusLbl').text('NEW');
        $('#btnSalesModelCode,#btnSalesModelYear,#btnColourCode,#btnSupplierBBN,#btnPartNo,#btnOtherCode').removeAttr("style");
        me.isSave = false;
        var SONo = localStorage.getItem("SONo") || "";
        //console.log("customerid: ", custid);
    };
    me.default = function () {
        SONo = localStorage.getItem('SONo');

        if (SONo != "") {
            $http.post('om.api/SalesOrder/Status?SONo=' + SONo).
             success(function (data, status, headers, config) {
                 if (data.Status == "2") {
                     me.checkStatus(data.Status);
                     $('#btnUnapprove').removeAttr('disabled');
                     $('#btnReject').removeAttr('disabled');
                     $('#btnApprove').attr('disabled', true);
                     me.disableHdr();
                     me.disableDtlSalesModel();
                     me.disableDtlSalesModelColour();
                     me.disableDtlSalesVin();
                     me.disableDtlSalesModelOther();
                     me.disableDtlSalesAccs();
                     me.isSave = false;
                 }
             });

        }
    }
    me.disableHdr = function () {
        var x = 'isC1,isC2,isC3,isC4,ITSNo,SODate,SalesType,RefferenceNo,RefferenceDate,CustomerCode,btnCustomerCode,TOPCode,btnTOPCode,TOPDays,BillTo,btnBillTo,ShipTo,btnShipTo,ProspectNo,SKPKNo,btnSKPKNo,Salesman,btnSalesman,WareHouseCode,btnWareHouseCode,isLeasing,LeasingCo,GroupPriceCode,Insurance,PaymentType,PrePaymentAmt,PrePaymentDate,PrePaymentBy,btnPrePaymentBy,CommissionBy,CommissionAmt,PONo,ContractNo,RequestDate,Remark,SalesCode,Installment,FinalPaymentDate,SalesCoordinator,SalesHead,BranchManager';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', true);
        }
    };
    me.disableHdrSts1 = function () {
        //var x = 'isC1,isC2,ITSNo,SODate,SalesType,RefferenceNo,RefferenceDate,CustomerCode,TOPDays,ProspectNo,SKPKNo,btnSKPKNo,Salesman,WareHouseCode,GroupPriceCode,SalesCode,Installment,FinalPaymentDate,SalesCoordinator,SalesHead,BranchManager';
        var x = 'isC1,isC2,ITSNo,SODate,SalesType,RefferenceNo,RefferenceDate,CustomerCode,TOPDays,ProspectNo,btnSKPKNo,Salesman,WareHouseCode,GroupPriceCode,SalesCode,Installment,FinalPaymentDate,SalesCoordinator,SalesHead,BranchManager';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', true);
        }
        $('#btnPelanggan').attr('disabled', true);
        $('#btnApprove').removeAttr('disabled');
    };
    me.disableDtlSalesModel = function () {
        me.isData = true;
        me.allowInputDetail = false;
    };
    me.disableDtlSalesModelColour = function () {
        var x = 'ColourCode,Quantity,Remark2';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', true);
        }
        me.isData2 = true;
        me.allowInputDetail2 = false;
    };
    me.disableDtlSalesVin = function () {
        var x = 'ChassisCode,ChassisNo,EngineCode,EngineNo,ServiceBookNo,KeyNo,EndUserName,EndUserAddress1,EndUserAddress2,EndUserAddress3,SupplierBBN,CityCode,BBN,KIR,Remark3,StatusReq';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', true);
        }
        me.isData3 = true;
        me.allowInputDetail3 = false;
    };
    me.disableDtlSalesModelOther = function () {
        var x = 'OtherCode,BeforeDiscDPP,BeforeDiscPPn,BeforeDiscTotal4,DiscExcludePPn,DiscIncludePPn,AfterDiscDPP4,AfterDiscPPn4,AfterDiscTotal4,DPP,PPn,Total,Remark4';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', true);
        }
        me.isData4 = true;
        me.allowInputDetail4 = false;
    };
    me.disableDtlSalesAccs = function () {
        var x = 'PartNo,PartSeq,DemandQty,Qty,SupplyQty,ReturnQty,CostPrice,RetailPrice,DiscExcludePPn,AfterDiscDPP5,AfterDiscPPn5,AfterDiscTotal5,TypeOfGoods,BillType,SupplySlipNo,SupplySlipDate,SSReturnNo,SSReturnDate,isSubstitution,CancelQty,InvoiceQty';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', true);
        }
        me.isData5 = true;
        me.allowInputDetail5 = false;
    };

    me.enableHdr = function () {
        var x = 'isC1,isC2,isC3,isC4,ITSNo,SODate,SalesType,RefferenceNo,RefferenceDate,CustomerCode,btnCustomerCode,TOPCode,btnTOPCode,TOPDays,BillTo,btnBillTo,ShipTo,btnShipTo,ProspectNo,SKPKNo,btnSKPKNo,Salesman,btnSalesman,WareHouseCode,btnWareHouseCode,isLeasing,GroupPriceCode,Insurance,PaymentType,PrePaymentAmt,PrePaymentDate,PrePaymentBy,btnPrePaymentBy,CommissionBy,CommissionAmt,PONo,ContractNo,RequestDate,Remark,SalesCode,Installment,SalesCoordinator,SalesHead,BranchManager';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).removeAttr('disabled', true);
        }
    };
    me.enableHdrCus = function () {
        var x = 'CustomerCode,CustomerName,TOPCode,TOPDays,BillTo,BillName,ShipTo,ShipName,Salesman,SalesmanName,GroupPriceCode,GroupPriceName,Remark,SKPKNo';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).val('');
        }
    };
    me.enableDtlSalesModelColour = function () {
        var x = 'ColourCode,Quantity,Remark2';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).removeAttr('disabled');
        }
    };
    me.enableDtlSalesVin = function () {
        var x = 'ChassisNo,EngineNo,ServiceBookNo,KeyNo,EndUserName,EndUserAddress1,EndUserAddress2,EndUserAddress3,SupplierBBN,CityCode,BBN,KIR,Remark3';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).removeAttr('disabled');
        }
    };
    me.enableDtlSalesModelOther = function () {
        var x = 'OtherCode,BeforeDiscTotal4,AfterDiscTotal4,DPP,PPn,Total,Remark4';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).removeAttr('disabled');
        }
    };
    me.enableDtlSalesAccs = function () {
        var x = 'PartNo,DemandQty,AfterDiscTotal5';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).removeAttr('disabled');
        }
    };

    me.pelanggan = function () {
        Wx.loadForm();
        Wx.showForm({ url: "gn/master/customer" });
    };

    me.linkSONo = function () {
        me.detail.SONo = me.data.SONo;
        me.detail2.SONo = me.data.SONo;
        me.detail3.SONo = me.data.SONo;
        me.detail4.SONo = me.data.SONo;
        me.detail5.SONo = me.data.SONo;
    };

    me.ClickModelTab = function () {
        me.detail.SONo = me.data.SONo;
        if (me.detail2.length == undefined || me.detail2.length < 1) {
            me.clearTable(me.gridSalesModel);
            me.loadDetail(me.detail, 1);
        }
    };

    me.ClickVevicleTab = function () {
        if (me.detail2.length == undefined || me.detail2.length < 1) {
            me.clearTable(me.gridColourInfo);
            me.clearTable(me.gridOthersInfo);
            me.detail2 = {};
            var x = 'EngineNo,ServiceBookNo,KeyNo,EndUserName,EndUserAddress1,EndUserAddress2,EndUserAddress3,SupplierBBN,CityCode,BBN,KIR,Remark3';
            var y = x.split(',', 60);
            var z = y.length;
            for (i = 0; i <= z; i++) {
                $('#' + y[i]).val('');
            }
            me.loadDetail(me.detail, 2);
            if (me.detail.SalesModelCode) {
                $('#btnAddColour').removeAttr('disabled');
            }
        }
    };

    me.ClickAccesoriesTab = function () {
        if (me.detail5.length == undefined || me.detail5.length < 1) {
            me.clearTable(me.gridAccesories);
            me.clearTable(me.gridSparePart);
            me.loadDetail(me.detail4, 4);
            me.loadDetail(me.detail5, 5);
            me.detail4 = {};
            me.detail5 = {};
            me.getQTYUnit();
        }
    };

    me.loadDetail = function (dt, id) {
        if (id == 1) {
            $http.post('om.api/SalesOrder/SalesModelLoad?SONo=' + dt.SONo).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.gridSalesModel, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                   console.log(e);
               });
        } else if (id == 2) {
            $http.post('om.api/SalesOrder/SalesModelColorLoad', dt).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.gridColourInfo, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                   console.log(e);
               });
        } else if (id == 3) {
            me.detail3.SONo = me.data.SONo;
            me.detail3.SalesModelCode = me.detail.SalesModelCode;
            me.detail3.SalesModelYear = me.detail.SalesModelYear;
            me.detail3.ColourCode = me.detail2.ColourCode;

            $http.post('om.api/SalesOrder/SalesSOVinLoad', dt).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.gridOthersInfo, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                   console.log(e);
               });
        } else if (id == 4) {
            me.detail4.SONo = me.data.SONo;
            me.detail4.SalesModelCode = me.detail.SalesModelCode;
            me.detail4.SalesModelYear = me.detail.SalesModelYear;
            $http.post('om.api/SalesOrder/SalesModelOthersLoad', dt).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.gridAccesories, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                   console.log(e);
               });
        } else if (id == 5) {
            me.detail5.SONo = me.data.SONo;
            $http.post('om.api/SalesOrder/SalesSOAccsSeqLoad', dt).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.gridSparepart, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                   console.log(e);
               });
        }
    };

    me.checkbox = function (data) {
        if ((data.RefferenceFakturPajakDate).substring(0, 4) != "1900") {
            $('#isC2').prop('checked', true);
            $('#RefferenceFakturPajakDate').prop('readonly', false);
        } else {
            $('#isC2').prop('checked', false);
            $('#RefferenceFakturPajakDate').prop('readonly', true);
            me.data.RefferenceFakturPajakDate = undefined;
        }

        if (data.DueDate != '' && (data.DueDate).substring(0, 4) != "1900") {
            $('#isC3').prop('checked', true);
            $('#DueDate').prop('readonly', false);
        } else {
            $('#isC3').prop('checked', false);
            $('#DueDate').prop('readonly', true);
            me.data.DueDate = undefined;
        }
    };

    me.checkStatus = function (Status) {
        //me.data.Status = Status;
        switch (parseInt(Status)) {
            case 0:
                $('#statusLbl').text("OPEN");
                me.allowEdit = true;
                //me.allowEditDetail = true;
                break;
            case 1:
                $('#statusLbl').text("PRINTED");
                me.allowEdit = true;
                //me.allowEditDetail = true;
                break;
            case 2:
                $('#statusLbl').text("APPROVED");
                me.allowEdit = false;
                //me.allowEditDetail = false;
                break;
            case 3:
                $('#statusLbl').text("DELETED");
                me.allowEdit = true;
                //me.allowEditDetail = false;
                break;
            case 4:
                $('#statusLbl').text("REJECTED");
                me.allowEdit = false;
                //me.allowEditDetail = false;
                break;
            case 9:
                $('#statusLbl').text("FINISHED");
                me.allowEdit = false;
                break;
        }
    };

    me.browse = function () {
        me.initialize();
        var lookup = Wx.klookup({
            //var lookup = Wx.blookup({
            name: "SOLookup",
            title: "Sales Order Lookup",
            url: "om.api/grid/SlsSOBrowse",
            //manager: spSalesManager,
            //query: "SalesSO",
            //defaultSort: "SONo desc",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "SONo", title: "No. SO", width: 125 },
                { field: "TypeSales", title: "Tipe", width: 100 },
                { field: "SODate", title: "Tanggal SO", template: "#= (SODate == undefined) ? '' : moment(SODate).format('DD MMM YYYY') #", width: 125 },
                { field: 'SKPKNo', title: 'No. SKPK', width: 100 },
                { field: 'RefferenceNo', title: 'No. Reff.', width: 125 },
                { field: 'RefferenceDates', title: 'Tanggal Reff.', width: 100 },//, template: "#= (RefferenceDate == undefined) ? '' : moment(RefferenceDate).format('DD MMM YYYY') #" },
                { field: 'Customer', title: 'Pelanggan', width: 300 },
                { field: 'Address', title: 'Alamat', width: 450 },
                { field: 'Sales', title: 'Salesman', width: 290 },
                { field: 'GroupPrice', title: 'Group Price Code', width: 175 },
                { field: 'Stat', title: 'Status', width: 100 },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.checkStatus(data.Status);
                console.log("Status: " + data.Status);
                me.lookupAfterSelect(data);
                me.isData = false;
                me.allowInputDetail = true;
                me.getTaxPCT(data.CustomerCode);
                if (data.Status == '0') {
                    me.disableHdrSts1();
                    $('#RefferenceNo').removeAttr('disabled');
                    //$('#btnRefferenceNo').removeAttr('disabled')
                    $('#RefferenceDate').removeAttr('disabled');
                    $('#btnApprove').attr('disabled', true);
                }
                if (data.Status == '1' || data.Status >= '2') {
                    if (data.Status == '1') {
                        $http.post('om.api/SalesOrder/checkBottomPrice', { SONo: me.data.SONo }).
                        success(function (data, status, headers, config) {
                            if (data.success) {
                                $('#btnApprove').removeAttr('disabled');
                            } else {
                                MsgBox(data.message, MSG_INFO)
                                $('#btnApprove').attr('disabled', 'disabled');
                            }
                        });
                        //$('#btnApprove').removeAttr('disabled')
                        $('#btnDelete').show();
                        me.disableHdrSts1();
                        //$http.post('om.api/SalesOrder/CheckApproval').
                        //success(function (Result, status, headers, config) {
                        //    if (Result == 1) {
                        //        $('#btnApprove').removeAttr('disabled');
                        //    } else {
                        //        $('#btnApprove').attr('disabled', 'disabled');
                        //    }
                        //});
                    };
                    if (data.Status >= '2') {
                        me.disableHdr();
                        me.disableDtlSalesModel();
                        me.disableDtlSalesModelColour();
                        me.disableDtlSalesVin();
                        me.disableDtlSalesModelOther();
                        me.disableDtlSalesAccs();
                        if (data.Status == '2') {
                            $('#btnUnapprove').removeAttr('disabled');
                            $('#btnReject').removeAttr('disabled');
                            $('#btnPelanggan').attr('disabled', true);
                            $('#btnDelete').removeAttr('ng-show');
                            $('#btnDelete').hide();
                        }
                        if (data.Status > '2') {
                            $('#btnApprove').attr('disabled', true);
                            $('#btnUnapprove').attr('disabled', true);
                            $('#btnReject').attr('disabled', true);
                            $('#btnPelanggan').attr('disabled', true);
                            $('#btnDelete').removeAttr('ng-show');
                            $('#btnDelete').hide();
                            //console.log("Status: " + data.Status);
                        }
                    }
                }
                //if (data.Status == '3') {
                // $('#btnUnapprove').attr('disabled', true);
                // $('#btnApprove').attr('disabled', true);
                // $('#btnReject').attr('disabled', true);
                // $('#btnPelanggan').attr('disabled', true);
                // me.disableHdr();
                // me.disableDtlSalesModel();
                // me.disableDtlSalesModelColour();
                //me.disableDtlSalesVin();
                //me.disableDtlSalesModelOther();
                //me.disableDtlSalesAccs();
                //}
                //me.checkbox(data);
                if (data.isC2 == true) {
                    me.isShow = true;
                } else {
                    me.isShow = false;
                }
                // alert(data.isLeasing);
                if (data.isLeasing == true) {
                    me.isDisable = false;
                } else {
                    me.isDisable = true;
                }
                me.loadDetail(data, 1);
                me.loadDetail(me.detail5, 5);
                me.isSave = false;
                me.isPrintAvailable = true;
                me.isPrintEnable = true;
                //me.detail5.Qty = 5;
                me.Apply();
                //$('#RefferenceFakturPajakDate').prop('readonly', true);
                //$("[name='RefferenceFakturPajakDate']").prop('disabled', true);
            }
        });
    };

    me.BrowseITS = function () {
        var lookup = Wx.blookup({
            name: "SOLookup",
            title: "Browse ITS",
            manager: spSalesManager,
            query: "BrowseITS" + ptype,
            defaultSort: "InquiryNo asc",
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
            me.data.VehicleType = data.TipeKendaraan;
            me.data.Salesman = data.EmployeeID;
            me.data.SalesmanName = data.EmployeeName;
            me.Apply();
        });
    }

    //me.getProductType = function () {
    //    $http.post('om.api/SalesOrder/getProductType', { customerCode: custCode }).
    //           success(function (dt, status, headers, config) {
    //               if (dt.success) {
    //                   taxPct = dt.data;
    //                   console.log(taxPct);
    //               } else {
    //                   MsgBox(dt.message, MSG_INFO);
    //               }
    //           }).
    //           error(function (e, status, headers, config) {
    //               MsgBox(e.message, MSG_ERROR);
    //               //console.log(e);
    //           });
    //}

    me.LeasingCo = function () {
        var lookup = Wx.blookup({
            name: "LeasingCoLookup",
            title: "LeasingCo Looukup",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("LeasingCo").withParameters({ ProfitCenterCode: me.datas.ProfitCenterCode }),
            defaultSort: "CustomerCode desc",
            columns: [
                { field: "CustomerCode", title: "Kode" },
                { field: "CustomerName", title: "Nama" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.LeasingCo = data.CustomerCode;
                me.data.LeasingCoName = data.CustomerName;
                me.Apply();
            }
        });
    };

    me.Reff = function () {
        var lookup = Wx.blookup({
            name: "PONoLookup",
            title: "PO No. Looukup",
            manager: spSalesManager,
            query: "POLookup",
            defaultSort: "PONo desc",
            columns: [
                { field: "RefferenceNo", title: "Reff. No." },
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "SupplierName", title: "Supplier Name" },
                { field: "PONo", title: "PO No." },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
            }
        });
    };

    //me.Customer = function () {
    //    var Query = "";
    //    if (me.data.isC2 == true) Query = 'Select4LookupCustomer2';
    //    else Query = 'Select4LookupCustomer';

    //    var lookup = Wx.blookup({
    //        name: "CustomerBrowse",
    //        title: "Customer",
    //        manager: spSalesManager,
    //        query: new breeze.EntityQuery().from(Query).withParameters({ ProfitCenterCode: me.datas.ProfitCenterCode }),
    //        defaultSort: "CustomerCode desc",
    //        columns: [
    //            { field: "CustomerCode", title: "Kode Customer", width: 100 },
    //            { field: "CustomerName", title: "Nama Customer", width: 180 },
    //            { field: "Address", title: "Alamat", width: 350 },
    //            { field: "TOPDesc", title: "Kode TOP", width: 125 },
    //            { field: "SalesCode", title: "Kelompok AR", width: 140 },
    //        ]
    //    });
    //    lookup.dblClick(function (data) {
    //        if (data != null) {
    //            me.data.CustomerCode = data.CustomerCode;
    //            me.data.CustomerName = data.CustomerName;
    //            me.data.TOPCode = data.TOPCD;
    //            me.data.TOPDays = data.TopCode;
    //            me.data.GroupPriceCode = data.GroupPriceCode;
    //            me.data.GroupPriceName = data.GroupPriceDesc;
    //            me.data.BillTo = data.CustomerCode;
    //            me.data.BillName = data.CustomerName;
    //            me.data.SalesCode = data.SalesCode;
    //            me.detail3.EndUserName = data.CustomerName;
    //            me.detail3.EndUserAddress1 = data.Address1;
    //            me.detail3.EndUserAddress2 = data.Address2;
    //            me.detail3.EndUserAddress3 = data.Address3;
    //            me.Apply();
    //            //console.log(me.data.SalesCode);
    //            //me.getTaxPCT(data.CustomerCode);
    //        }
    //    });
    //};

    me.Customer = function () {
        var Query = "";
        if (me.data.isC2 == true) Query = 'uspfn_getSelectLookupCustomer2';
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
            if (data != null) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
                me.data.TOPCode = data.TOPCD;
                me.data.TOPDays = data.TopCode;
                me.data.GroupPriceCode = data.GroupPriceCode;
                me.data.GroupPriceName = data.GroupPriceDesc;
                me.data.BillTo = data.CustomerCode;
                me.data.BillName = data.CustomerName;
                me.data.SalesCode = data.SalesCode;
                me.detail3.EndUserName = data.CustomerName;
                me.detail3.EndUserAddress1 = data.Address1;
                me.detail3.EndUserAddress2 = data.Address2;
                me.detail3.EndUserAddress3 = data.Address3;
                me.Apply();
            }
        });
    }

    me.getTaxPCT = function (custCode) {
        if (custCode != null || custCode != undefine) {
            $http.post('om.api/SalesOrder/Select4Tax', { customerCode: custCode }).
               success(function (dt, status, headers, config) {
                   if (dt.success) {
                       taxPct = dt.data;
                       console.log(taxPct);
                   } else {
                       MsgBox(dt.message, MSG_INFO);
                   }
               }).
               error(function (e, status, headers, config) {
                   MsgBox(e.message, MSG_ERROR);
                   //console.log(e);
               });
        }
    };

    me.Salesman = function (x) {
        console.log(x);
        if (!x) {
            console.log(1);
            var lookup = Wx.blookup({
                name: "Salesman",
                title: "Salesman",
                manager: spSalesManager,
                query: "Select4LookupSalesman",
                defaultSort: "EmployeeID asc",
                columns: [
                    { field: "EmployeeID", title: "ID" },
                    { field: "EmployeeName", title: "Nama" },
                    { field: "TitleName", title: "Jabatan" },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.data.Salesman = data.EmployeeID;
                    me.data.SalesmanName = data.EmployeeName;
                    me.Apply();
                }
            });
        } else {
            console.log(2);
            var lookup = Wx.blookup({
                name: "Salesman",
                title: "Diterima Oleh",
                manager: spSalesManager,
                query: "Select4LookupSalesman",
                defaultSort: "EmployeeID asc",
                columns: [
                    { field: "EmployeeID", title: "ID" },
                    { field: "EmployeeName", title: "Nama" },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    me.data.PrePaymentBy = data.EmployeeID;
                    me.data.PrePaymentName = data.EmployeeName;
                    me.Apply();
                }
            });
        }

    };

    me.TOP = function () {
        var lookup = Wx.blookup({
            name: "SupplierBrowse",
            title: "TOP",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "TOPC" }),
            defaultSort: "LookUpValue desc",
            columns: [
                { field: "LookUpValue", title: "Kode TOP" },
                { field: "LookUpValueName", title: "Nama TOP" },
                { field: "ParaValue", title: "Interval" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.TOPCode = data.LookUpValue;
                me.data.TOPDays = data.ParaValue;
                me.data.LookUpValueName = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    //me.ShipTo = function () {
    //    var lookup = Wx.blookup({
    //        name: "ShipToLookup",
    //        title: "ShipTo Lookup",
    //        manager: spSalesManager,
    //        query: new breeze.EntityQuery().from("select4LookupTo").withParameters({ ProfitCenterCode: me.datas.ProfitCenterCode }),
    //        defaultSort: "CustomerCode desc",
    //        columns: [
    //            { field: "CustomerCode", title: "Kode." },
    //            { field: "CustomerName", title: "Nama" },
    //        ]
    //    });
    //    lookup.dblClick(function (data) {
    //        if (data != null) {
    //            me.data.ShipTo = data.CustomerCode;
    //            me.data.ShipName = data.CustomerName;
    //            me.Apply();
    //        }
    //    });
    //};

    me.ShipTo = function () {
        var Query = "uspfn_getSelectLookupCustomer2";
        var lookup = Wx.klookup({
            name: "ShipToLookup",
            title: "ShipTo Lookup",
            url: "om.api/Grid/CustBrowse?cols=" + 5 + "&spId=" + Query,
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "CustomerCode", title: "Kode." },
                { field: "CustomerName", title: "Nama" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ShipTo = data.CustomerCode;
                me.data.ShipName = data.CustomerName;
                me.Apply();
            }
        });
    }

    me.SKPKNo = function () {
        var lookup = Wx.blookup({
            name: "ReffDOLookup",
            title: "Reff DO Lookup",
            manager: spSalesManager,
            query: "SelectDraftSO",
            defaultSort: "DraftSONo desc",
            columns: [
                { field: "DraftSONo", title: "No.SKPK" },
                { field: "DraftSODate", title: "Tgl. SKPK", template: "#= (DraftSODate == undefined) ? '' : moment(DraftSODate).format('DD MMM YYYY') #" },
                { field: "CustomerCode", title: "Kd. Pelanggan" },
                { field: "CustomerName", title: "Nama Pelanggan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
                me.data.TOPCode = data.TOPCode;
                me.data.TOPDays = data.TOPDays;
                //me.data.TOPDays = data.TOPDays;
                me.data.BillTo = data.CustomerCode;
                me.data.BillName = data.CustomerName;
                //me.data.ShipTo = data.ShipTo;
                //me.data.ProspectNo = data.ProspectNo;
                me.data.SKPKNo = data.DraftSONo;
                me.data.Salesman = data.Salesman;
                me.data.SalesmanName = data.SalesmanName;
                //me.data.WareHouseCode = data.WareHouseCode;
                //me.data.isLeasing = data.isLeasing;
                me.data.LeasingCo = data.LeasingCo;
                me.data.GroupPriceCode = data.GroupPriceCode;
                me.data.GroupPriceName = data.GroupPriceName;
                //me.data.Insurance = data.Insurance;
                //me.data.PaymentType = data.PaymentType;
                //me.data.PrePaymentAmt = data.PrePaymentAmt;
                //me.data.PrePaymentDate = data.PrePaymentDate;
                //me.data.PrePaymentBy = data.PrePaymentBy;
                //me.data.CommissionBy = data.CommissionBy;
                me.data.CommissionAmt = data.CommissionAmt;
                //me.data.PONo = data.PONo;
                //me.data.ContractNo = data.ContractNo;
                //me.data.RequestDate = data.RequestDate;
                me.data.Remark = data.Remark;
                //me.data.SalesCode = data.SalesCode;
                //me.data.Installment = data.Installment;
                //me.data.FinalPaymentDate = data.FinalPaymentDate;
                //me.data.SalesCoordinator = data.SalesCoordinator;
                //me.data.SalesHead = data.SalesHead;
                //me.data.BranchManager = data.BranchManager
                me.Apply();
            }
        });

    };

    me.WareHouseCode = function () {
        var lookup = Wx.blookup({
            name: "WarehouseCodeLookup",
            title: "Lookup Warehouse",
            manager: spSalesManager,
            query: "WareHouse",
            defaultSort: "seqno asc",
            columns: [
                { field: "LookUpValue", title: "Warehouse Code" },
                { field: "LookUpValueName", title: "Warehouse Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.WareHouseCode = data.LookUpValue;
                me.data.WareHouseName = data.LookUpValue;
                me.Apply();
            }
        });
    };

    me.SalesModelCode = function () {
        var Query = "";
        var number = "";
        if (me.datas.isDSOL == "1" && me.data.isC2 == true) {
            Query = 'Select4ModelSO';
            number = me.data.SKPKNo
            console.log(Query);
        }
        else {
            Query = 'Select4LookupModel';
            number = me.data.ProspectNo != "" ? me.data.ProspectNo : "";
            console.log(Query);
        }

        var lookup = Wx.blookup({
            name: "SalesModelCodeLookup",
            title: "Model",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from(Query).withParameters({ Number: number }),
            defaultSort: "SalesModelCode asc",
            columns: [
                { field: "SalesModelCode", title: "Kode Sales Model" },
                { field: "SalesModelDesc", title: "Deskripsi" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                console.log(data.SalesModelCode);
                me.detail.SalesModelCode = data.SalesModelCode;
                console.log(me.detail.SalesModelCode);

                $('#SalesModelYear').val("");

                //$('#SalesModelCode').val(data.SalesModelCode);
                me.Apply();
            }
        });
    };

    me.SalesModelYear = function () {
        var Query = "";
        var number = "";
        var smcode = "";
        if (me.datas.isDSOL == "1" && me.data.isC2 == true) {
            Query = 'Select4SalesModelYear';
            number = me.data.SKPKNo
        }
        else {
            Query = 'select4LookupModelYear';
            number = me.data.GroupPriceCode;

        }
        smcode = me.detail.SalesModelCode;
        if (smcode == undefined) {
            me.SalesModelCode();
            return;
        }
        console.log(number, smcode);
        //console.log(Query, number, me.data.GroupPriceCode);
        var lookup = Wx.blookup({
            name: "SalesModelYearLookup",
            title: "Model year",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from(Query).withParameters({ Number: number, SalesModelCode: me.detail.SalesModelCode }),
            defaultSort: "SalesModelYear asc",
            columns: [
                { field: "SalesModelYear", title: "Tahun" },
                { field: "SalesModelDesc", title: "Deskripsi" },
                { field: "ChassisCode", title: "Kode Rangka" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.SalesModelYear = data.SalesModelYear;
                me.detail.SalesModelDesc = data.SalesModelDesc;
                me.detail.GroupPriceCode = me.data.GroupPriceCode;
                me.detail.ChassisCode = data.ChassisCode;
                $http.post('om.api/MstPriceListJual/CekData', me.detail).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail.BeforeDiscTotal = data.data.Total;
                        me.detail.TotalMinStaff = data.data.TotalMinStaff;
                        me.ReformatNumber();
                    } else {
                        me.detail.BeforeDiscTotal = 0;
                        me.detail.AfterDiscDPP = 0;
                        me.detail.AfterDiscPPn = 0;
                        me.detail.AfterDiscPPnBM = 0;
                        me.detail.AfterDiscTotal = 0;
                        me.detail.DiscIncludePPn = 0;
                    }
                }).
                error(function (data, status, headers, config) {
                    alert('error');
                });
            }
        });
    };

    me.BrowseColour = function () {
        var number = me.data.ProspectNo != "" ? me.data.ProspectNo : "";
        var lookup = Wx.blookup({
            name: "WarnaLookup",
            title: "Warna",
            manager: spSalesManager,
            //query: "SalesModelYearLookup",
            query: new breeze.EntityQuery().from("SelectColour4SO").withParameters({ SalesModelCode: me.detail.SalesModelCode, InquiryNumber: number }),
            defaultSort: "ColourCode asc",
            columns: [
                { field: "ColourCode", title: "Kode" },
                { field: "ColourDesc", title: "Warna" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail2.ColourCode = data.ColourCode;
                me.detail2.ColourDescription = data.ColourDesc;
                me.Apply();
                if (number != null) {
                    $http.post('om.api/SalesOrder/getQtyITS?NoIts=' + number).
                        success(function (data, status, headers, config) {
                            me.detail2.Quantity = data.qty;
                            $("#Quantity").attr('disabled', 'disabled');
                        });
                } else {
                    $("#Quantity").removeAttr('disabled');
                }
            }
        });
    };

    me.ChassisNo = function () {
        var lookup = Wx.blookup({
            name: "ChassisNoLookup",
            title: "Chassis No",
            manager: spSalesManager,
            //query: "SalesModelYearLookup",
            query: new breeze.EntityQuery().from("SelectChassis4SO").withParameters({ SalesModelCode: me.detail.SalesModelCode, SalesModelYear: me.detail.SalesModelYear, ColourCode: me.detail2.ColourCode, WareHouseCode: me.data.WareHouseCode, ChassisCode: me.detail3.ChassisCode }),
            defaultSort: "ChassisNo asc",
            columns: [
                { field: "ChassisNo", title: "No Rangka" },
                { field: "EngineCode", title: "Kode Mesin" },
                { field: "EngineNo", title: "No Mesin" },
                { field: "ServiceBookNo", title: "No Buku Servis" },
                { field: "KeyNo", title: "No Kunci" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail3.ChassisNo = data.ChassisNo;
                me.detail3.ChassisCode = data.ChassisCode == null ? me.detail3.ChassisCode : data.ChassisCode;
                me.detail3.EngineNo = data.EngineNo;
                me.detail3.EngineCode = data.EngineCode;
                me.detail3.ServiceBookNo = data.ServiceBookNo;
                me.detail3.KeyNo = data.KeyNo;
                me.STNKNameByDefault();
                me.Apply();
            }
        });
    };

    me.STNKNameByDefault = function () {
        $http.post('om.api/SalesOrder/GetCustomer?CustomerCode=' + me.data.CustomerCode).
               success(function (data, status, headers, config) {
                   me.detail3.EndUserName = data.CustomerName;
                   me.detail3.EndUserAddress1 = data.Address1;
                   me.detail3.EndUserAddress2 = data.Address2;
                   me.detail3.EndUserAddress3 = data.Address3;
               }).
               error(function (e, status, headers, config) {
                   console.log(e);
               });
    };

    me.Pemasok = function () {
        var lookup = Wx.blookup({
            name: "PemasokLookup",
            title: "Pemasok",
            manager: spSalesManager,
            //query: "SalesModelYearLookup",
            query: new breeze.EntityQuery().from("SelectPemasok4SO").withParameters({ ProfitCenterCode: me.datas.ProfitCenterCode, SalesModelCode: me.detail.SalesModelCode, SalesModelYear: me.detail.SalesModelYear }),
            defaultSort: "SupplierCode asc",
            columns: [
                { field: "SupplierCode", title: "Kode Pemasok" },
                { field: "SupplierName", title: "Nama Pemasok" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail3.SupplierBBN = data.SupplierCode;
                me.detail3.SupplierName = data.SupplierName;
                me.Apply();
            }
        });
    };

    me.City = function () {
        var lookup = Wx.blookup({
            name: "WarnaLookup",
            title: "Kota",
            manager: spSalesManager,
            //query: "SalesModelYearLookup",
            query: new breeze.EntityQuery().from("SelectCity4SO").withParameters({ SupplierCode: me.detail3.SupplierBBN, SalesModelCode: me.detail.SalesModelCode, SalesModelYear: me.detail.SalesModelYear }),
            defaultSort: "CityCode asc",
            columns: [
                { field: "CityCode", title: "Kode Kota" },
                { field: "CityDesc", title: "Nama Kota" },
                { field: "BBN", title: "BBN" },
                { field: "KIR", title: "KIR" }
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail3.CityCode = data.CityCode;
                me.detail3.CityDesc = data.CityDesc;
                me.detail3.BBN = data.BBN;
                me.detail3.KIR = data.KIR;
                me.Apply();
            }
        });
    };

    me.Accesories = function () {
        var lookup = Wx.blookup({
            name: "AksLookup",
            title: "Aks. Lain-lain",
            manager: spSalesManager,
            query: "Select4GroupPrice",
            //query: new breeze.EntityQuery().from("SelectCity4SO").withParameters({ SupplierCode: me.detail3.SupplierBBN, SalesModelCode: me.detail.SalesModelCode, SalesModelYear: me.detail.SalesModelYear }),
            defaultSort: "RefferenceCode asc",
            columns: [
                { field: "RefferenceCode", title: "Kode" },
                { field: "RefferenceDesc1", title: "Deskripsi" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail4.OtherCode = data.RefferenceCode;
                me.detail4.OtherDesc = data.RefferenceDesc1;
                me.Apply();
            }
        });
    };

    me.PartCode = function () {
        var lookup = Wx.blookup({
            name: "PartLookup",
            title: "Part",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("SOPartAcc").withParameters({ SONo: me.data.SONo }),
            //defaultSort: "PartNo asc",
            //var lookup = Wx.klookup({
            //    name: "PartLookup",
            //    title: "Part",
            //    url: "om.api/grid/SOPartAcc",
            //    //params: { SONo: me.data.SONo },
            //    serverBinding: true,
            //    pageSize: 10,
            //    sortable: false,
            columns: [
                { field: "PartNo", title: "No Part" },
                { field: "Available", title: "Avail Qty." },
                { field: "PartName", title: "Nama Part" },
                { field: "Status", title: "Status" },
                { field: "JenisPart", title: "Jenis Part" },
                { field: "NilaiPart", title: "Nilai" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail5.PartNo = data.PartNo;
                me.detail5.PartName = data.PartName;
                me.detail5.RetailPrice = data.NilaiPart;
                me.detail5.CostPrice = data.CostPrice;
                me.Apply();
                $("#QTY").removeAttr('disabled');
                $("[name='AfterDiscTotal5']").focus();
            }
        });
    };

    me.backStatus = function (status) {
        me.checkStatus(status);
        $('#btnApprove').attr('disabled', true);
    }

    me.saveData = function (e, param) {
        $(".ajax-loader").show();
        console.log(me.data);
        $http.post('om.api/SalesOrder/save', me.data)//{ model: me.data, salesmodelcode: me.detail.SalesModelCode, options: me.options })
        .success(function (data, status, headers, config) {
            $(".ajax-loader").hide();
            if (data.status) {
                //$('#SONo').val(data.data.SONumber);
                me.data.SONo = data.data.SONumber;
                Wx.Success("Data saved...");
                me.startEditing();
                me.isData = false;
                me.allowInputDetail = false;
                $('#btnAddModel').removeAttr('disabled');
                me.data.Status = data.data.SOStatus;
            } else {
                MsgBox(data.message, MSG_INFO);
            }
        })
        .error(function (data, status, headers, config) {
            $(".ajax-loader").hide();
            MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
        });
    };


    me.AddEditDetail = function () {
        var smy = $('#SalesModelYear').val()
        if (smy == null || smy == "" || smy == undefined) {
            me.SalesModelYear();
            return;
        }
        $http.post('om.api/SalesOrder/cekPriceListBranch', { SModelCode: $('#SalesModelCode').val(), SModelYear: $('#SalesModelYear').val(), GrpPrice: $('#SODate').val() }).
        success(function (data, status, headers, config) {
            if (data.success) {
                me.detail.GroupPriceCode = me.data.GroupPriceCode;
                me.data.SONo = $('#SONo').val();
                me.detail.SONo = $('#SONo').val();

                //me.detail.AfterDiscDPP = DPP;
                //me.detail.AfterDiscPPn = PPN;
                //me.detail.DiscExcludePPn = totalBefore - DPP;
                //me.detail.DiscIncludePPn = totalBefore - totalAfter;

                //var a = parseInt(($('#BeforeDiscTotal').val()) / 1.1);
                //var b = parseInt(a / 10);
                //var a = $('#BeforeDiscTotal').val();
                //me.detail.BeforeDiscTotal = a;
                //me.detail.BeforeDiscDPP = a; //ambil dpp omMstPricelistSell
                //me.detail.BeforeDiscPPn = b; // ambil ppn

                //var c = $('#DiscIncludePPn').val();
                //c = c.split(',').join('');
                //var DiscExcludePPn = Math.round(c / 1.1);
                //me.detail.DiscExcludePPn = DiscExcludePPn;
                //console.log(DiscExcludePPn);

                //var d = $('#BeforeDiscTotal').val();
                //d = d.split(',').join('');
                //var BeforeDiscDPP = Math.round(d / 1.1);
                //me.detail.BeforeDiscDPP = BeforeDiscDPP;
                //console.log(BeforeDiscDPP);

                //me.detail.BeforeDiscPPn = Math.round(BeforeDiscDPP / 10);
                //console.log(Math.round(BeforeDiscDPP / 10));

                if ($('#isC5').prop('checked') == true) {
                    var Field = 'SalesModelCode,SalesModelYear,AfterDiscTotal';
                    var Names = 'Sales Model Code,Sales Model Year,Total Harga Setelah Diskon';
                } else {
                    var Field = 'SalesModelCode,SalesModelYear,AfterDiscDPP';
                    var Names = 'Sales Model Code,Sales Model Year,DPP';
                }

                var ret = me.CheckMandatory(Field, Names);

                if (ret != "") {
                    MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
                } else {
                    $http.post('om.api/SalesOrder/save2', me.detail).
                       success(function (data, status, headers, config) {
                           if (data.success) {
                               Wx.Success(data.message);
                               me.startEditing();
                               me.CloseModel();
                               me.backStatus(data.Status);
                           } else {
                               MsgBox(data.message, MSG_INFO);
                           }
                       }).
                       error(function (data, status, headers, config) {
                           MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                       });
                }
            } else {
                MsgBox(data.message, MSG_INFO);
            }
        }).
        error(function (data, status, headers, config) {
            MsgBox(data.message, MSG_INFO);
        });
    };

    me.AddEditDetail2 = function () {
        var Field = 'ColourCode,Quantity';
        var Names = 'Kode Warna,Jumlah';
        var ret = me.CheckMandatory(Field, Names);

        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            if (me.detail2.Quantity < 1 || me.detail2.Quantity == undefined) {
                MsgBox(ret + " Ada Informasi yang belum lengkap !", MSG_INFO);
                return;
            }
            me.detail2.SONo = me.data.SONo;
            me.detail2.SalesModelCode = me.detail.SalesModelCode;
            me.detail2.SalesModelYear = me.detail.SalesModelYear;
            $http.post('om.api/SalesOrder/save3', me.detail2).
                    success(function (data, status, headers, config) {
                        if (data.success) {
                            Wx.Success(data.message);
                            me.startEditing();
                            me.CloseModel2();
                            me.backStatus(data.Status);
                        } else {
                            MsgBox(data.message, MSG_INFO);
                        }
                    }).
                    error(function (data, status, headers, config) {
                        MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                    });
        }

    };

    me.AddEditDetail3 = function () {
        var Field = 'EndUserName';
        var Names = 'Nama STNK,Pemasok';
        var ret = me.CheckMandatory(Field, Names);

        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            me.detail3.SONo = me.data.SONo;
            me.detail3.ColourCode = me.detail2.ColourCode;
            me.detail3.SalesModelCode = me.detail.SalesModelCode;
            me.detail3.SalesModelYear = me.detail.SalesModelYear;
            $http.post('om.api/SalesOrder/save4', me.detail3).
              success(function (data, status, headers, config) {
                  if (data.success) {
                      Wx.Success(data.message);
                      me.startEditing();
                      me.CloseModel3();
                      me.backStatus(data.Status);
                      me.isPrintAvailable = true;
                      me.isLoadData = true;
                  } else {
                      MsgBox(data.message, MSG_INFO);
                  }
              }).
              error(function (data, status, headers, config) {
                  MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
              });
        }
    };

    me.AddEditDetail4 = function () {
        var Field = 'OtherCode,BeforeDiscTotal,AfterDiscTotal';
        var Names = 'Aks. Lain-lain,Total(Harga seblum diskon),Total(Harga setelah diskon)';
        var ret = me.CheckMandatory(Field, Names);

        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            me.detail4.SONo = me.data.SONo;
            me.detail4.SalesModelCode = me.detail.SalesModelCode;
            me.detail4.SalesModelYear = me.detail.SalesModelYear;
            $http.post('om.api/SalesOrder/save5', me.detail4).
              success(function (data, status, headers, config) {
                  if (data.success) {
                      Wx.Success(data.message);
                      me.startEditing();
                      me.CloseModel4();
                      me.backStatus(data.Status);
                  } else {
                      MsgBox(data.message, MSG_INFO);
                  }
              }).
              error(function (data, status, headers, config) {
                  MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
              });
        }
    };

    me.AddEditDetail5 = function () {
        var Field = 'PartNo,AfterDiscTotal';
        var Names = 'No Part,Total(Harga setelah diskon)';
        var ret = me.CheckMandatory(Field, Names);
        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            if ($('#QtyUnit').val() == 0) { return MsgBox("Quantity unit masih bernilai 0!") }
            me.detail5.SONo = me.data.SONo;
            var QtyUnit = $('#QtyUnit').val();
            var Qty = $('#Qty').val() == "" ? 0 : $('#Qty').val();
            if ($('#Qty').val() == 0) { return MsgBox("Quantity masih bernilai 0!") }
            me.detail5.DemandQty = parseInt(QtyUnit) * parseInt(Qty);
            $http.post('om.api/SalesOrder/save6', me.detail5).
              success(function (data, status, headers, config) {
                  if (data.success) {
                      me.startEditing();
                      me.CloseModel5();
                      me.backStatus(data.Status);
                      Wx.Success("Add part berhasil!")
                  } else {
                      MsgBox(data.message, MSG_INFO);
                  }
              }).
              error(function (data, status, headers, config) {
                  MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
              });
        }

    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/salesorder/Delete', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.init();
                        Wx.Success("Data deleted...");
                    } else {
                        MsgBox(data.message, MSG_INFO);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                });
            }
        });
    };

    me.deleteDetail = function () {
        me.detail.SONo = me.data.SONo;
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/SalesOrder/Delete2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data deleted...");
                    me.CloseModel();
                    me.backStatus(data.Status);
                } else {
                    MsgBox(data.message, MSG_INFO);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
            });
            }
        });
    };

    me.deleteDetail2 = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                me.CloseModel3();
                $http.post('om.api/SalesOrder/Delete3', me.detail2).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data deleted...");
                    me.CloseModel2();
                    me.clearTable(me.gridOthersInfo);
                    me.backStatus(data.Status);
                    //me.gridOthersInfo.clearSelection();
                    //me.loadTableData(me.gridOthersInfo, null);
                } else {
                    MsgBox(data.message, MSG_INFO);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
            });
            }
        });
    };

    me.deleteDetail3 = function (x) {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/SalesOrder/Delete4', me.detail3).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data deleted...");
                    me.CloseModel3();
                    me.backStatus(data.Status);
                } else {
                    MsgBox(data.message, MSG_INFO);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
            });
            }
        });

    };

    me.deleteDetail4 = function (x) {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/SalesOrder/Delete5', me.detail4).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data deleted...");
                        me.CloseModel4();
                        me.backStatus(data.Status);
                    } else {
                        MsgBox(data.message, MSG_INFO);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                });
            }
        });
    };

    me.deleteDetail5 = function (x) {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/SalesOrder/Delete6', me.detail5).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data deleted...");
                        me.CloseModel5();
                        me.backStatus(data.Status);
                    } else {
                        MsgBox(data.message, MSG_INFO);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                });
            }
        });
    };

    me.CloseModel = function () {
        //me.gridSalesModel.clearSelection();
        me.loadDetail(me.detail, 1);
        me.detail = {};
    };

    me.CloseModel2 = function () {
        me.CloseModel3();
        me.loadDetail(me.detail2, 2);
        me.detail2 = {};
        $('#btnColourCode').removeAttr('disabled');
        //me.gridColourInfo.clearSelection();
    };

    me.CloseModel3 = function () {
        me.loadDetail(me.detail3, 3);
        var tmp = me.detail3.ChassisCode;
        me.detail3 = {};
        me.detail3.ChassisCode = tmp;
        //me.gridOthersInfo.clearSelection();
    };

    me.CloseModel4 = function () {
        me.loadDetail(me.detail4, 4);
        me.detail4 = {};
        //me.gridAccesories.clearSelection();
    };

    me.CloseModel5 = function () {
        me.loadDetail(me.detail5, 5);
        me.detail5 = {};
        me.getQTYUnit();
        $('#btnPartNo').removeAttr('disabled');
        $('#QTY').removeAttr('disabled');
        //me.gridSparepart.clearSelection();
    };

    $('div > p').click(function () {
        var name = $(this).data("name");
        if (name == "tabSalesModel") {
            me.loadDetail(me.detail, 1);
        } else if (name == "tabOthers") {
            me.loadDetail(me.detail2, 2);
        } else if (name == "tabSubSalesModel") {
            me.loadDetail(me.detail2, 4);
        }
    });

    $('#AfterDiscDPP').on('blur', function (e) {
        if ($('#chkTotalPriceAfter').prop('checked') == false) {
            var totalBefore = $("[name='BeforeDiscTotal']").val();
            var totalAfter = $("[name='AfterDiscTotal']").val();
            $http.post('om.api/SalesOrder/doCount', { CustCode: me.data.CustomerCode, SlsModelCode: me.data.SalesModelCode, val: $('#chkTotalPriceAfter').prop('checked'), beforeDiscTotal: totalBefore, afterDiscTotal: totalAfter, vdpp: me.detail.AfterDiscDPP }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    console.log(data.data.totalPrice)
                    if ((totalBefore - data.data.totalPrice) < 0) {
                        MsgBox("Nilai yang dimasukan lebih besar dari harga batas atas!", MSG_INFO)
                        me.detail.AfterDiscTotal = totalBefore;
                        totalAfter = totalBefore;
                        $http.post('om.api/SalesOrder/doCount', { CustCode: me.data.CustomerCode, SlsModelCode: me.data.SalesModelCode, val: "true", beforeDiscTotal: totalBefore, afterDiscTotal: totalAfter, vdpp: me.detail.AfterDiscDPP }).
                        success(function (data, status, headers, config) {
                            if (data.success) {
                                var DPP = data.data.dpp;
                                var PPN = data.data.ppn;
                                me.detail.AfterDiscDPP = DPP;
                                me.detail.AfterDiscPPn = PPN;
                                me.detail.AfterDiscPPnBM = data.data.ppnBm;
                                me.detail.DiscIncludePPn = totalBefore - totalAfter
                            } else {
                                MsgBox(data.message, MSG_INFO);
                            }
                        }).
                            error(function (data, status, headers, config) {
                                MsgBox("Terjadi kesalahan dalam proses perhitungan, silahkan hubungi SDMS support!", MSG_INFO);
                            });
                    } else {
                        //console.log(data.data.totalPrice)
                        if ((data.data.totalPrice - me.detail.TotalMinStaff) < 0) {
                            MsgBox("Nilai yang dimasukan lebih kecil dari harga batas bawah!", MSG_INFO)
                            //me.detail.AfterDiscTotal = me.detail.TotalMinStaff;
                            //totalAfter = me.detail.TotalMinStaff;
                            $http.post('om.api/SalesOrder/doCount', { CustCode: me.data.CustomerCode, SlsModelCode: me.data.SalesModelCode, val: "true", beforeDiscTotal: totalBefore, afterDiscTotal: totalAfter, vdpp: me.detail.AfterDiscDPP }).
                            success(function (data, status, headers, config) {
                                if (data.success) {
                                    var DPP = data.data.dpp;
                                    var PPN = data.data.ppn;
                                    me.detail.AfterDiscDPP = DPP;
                                    me.detail.AfterDiscPPn = PPN;
                                    me.detail.AfterDiscPPnBM = data.data.ppnBm;
                                    me.detail.DiscIncludePPn = totalBefore - totalAfter
                                } else {
                                    MsgBox(data.message, MSG_INFO);
                                }
                            }).
                                error(function (data, status, headers, config) {
                                    MsgBox("Terjadi kesalahan dalam proses perhitungan, silahkan hubungi SDMS support!", MSG_INFO);
                                });
                        } else {
                            me.detail.AfterDiscTotal = data.data.totalPrice;
                            me.detail.AfterDiscPPn = data.data.ppn;
                            me.detail.AfterDiscPPnBM = data.data.ppnBm;
                            me.detail.DiscIncludePPn = data.data.disc;
                        }
                    }
                } else {
                    MsgBox(data.message, MSG_INFO);
                }
            }).
                error(function (data, status, headers, config) {
                    MsgBox("Terjadi kesalahan dalam proses perhitungan, silahkan hubungi SDMS support!", MSG_INFO);
                });
        }
    });

    $('#isLeasing').on('change', function (e) {
        if ($('#isLeasing').prop('checked') == true) {
            me.isDisable = false;
        } else {
            me.isDisable = true;
            $('#LeasingCo').val("");
            $('#LeasingCoName').val("");
        }
        me.Apply();
    });

    //$('#Installment').on('change', function (e) {
    //    alert($('#Installment').val());
    //});

    $('#isC1').on('change', function (e) {
        if ($('#isC1').prop('checked') == false) {
            me.data.RefferenceDate = undefined;
            $('#RefferenceDate').attr('disabled', true);
        } else {
            me.data.RefferenceDate = me.now();
            $('#RefferenceDate').removeAttr('disabled');
        }
        me.Apply();
    });

    $('#isC2').on('change', function (e) {
        if ($('#isC2').prop('checked') == true) {
            me.isShow = true;
            me.enableHdrCus();
        } else {
            me.isShow = false;
            me.enableHdrCus();
        }
        me.Apply();
    });

    $('#isC3').on('change', function (e) {
        if ($('#isC3').prop('checked') == false) {
            me.data.PrePaymentDate = undefined;
            $('#PrePaymentDate').attr('disabled', true);
        } else {
            me.data.PrePaymentDate = me.now();
            $('#PrePaymentDate').removeAttr('disabled');
        }
        me.Apply();
    });

    $('#isC4').on('change', function (e) {
        if ($('#isC4').prop('checked') == false) {
            me.data.RequestDate = undefined;
            $('#RequestDate').attr('disabled', true);
        } else {
            me.data.RequestDate = me.now();
            $('#RequestDate').removeAttr('disabled');
        }
        me.Apply();
    });

    $("#isC5").on('change', function () {
        if ($('#isC5').prop('checked') == true) {
            $('#AfterDiscTotal').removeAttr('disabled');

            $('#AfterDiscDPP').attr('disabled', true);
            $('#AfterDiscPPn').attr('disabled', true);
            $('#AfterDiscPPnBM').attr('disabled', true);
        } else {
            $('#AfterDiscTotal').attr('disabled', true);

            $('#AfterDiscDPP').removeAttr('disabled');
            $('#AfterDiscPPn').removeAttr('disabled');
            $('#AfterDiscPPnBM').removeAttr('disabled');
        }
    });

    $('#isC6').on('change', function (e) {
        if ($('#isC3').prop('checked') == true) {
            me.data.DueDate = me.now();
            $('#DueDate').prop('readonly', false);
        } else {
            me.data.DueDate = undefined;
            $('#DueDate').prop('readonly', true);
        }
        me.Apply();
    });


    $("[name='AfterDiscTotal']").on('blur', function () {
        var totalBefore = $("[name='BeforeDiscTotal']").val();
        var totalAfter = $("[name='AfterDiscTotal']").val();
        totalAfter = totalAfter.split(',').join('');
        totalBefore = totalBefore.split(',').join('');
        if (totalAfter != 0) {
            if (totalAfter != "") {
                if ((totalBefore - totalAfter) >= 0) {
                    if ((totalAfter - me.detail.TotalMinStaff) < 0) {
                        MsgBox("Nilai yang dimasukan lebih kecil dari harga batas bawah!", MSG_INFO)
                    }
                    //me.detail.afterDiscDpp = 0;
                    $http.post('om.api/SalesOrder/doCount', { CustCode: me.data.CustomerCode, SlsModelCode: me.data.SalesModelCode, val: $('#chkTotalPriceAfter').prop('checked'), beforeDiscTotal: totalBefore, afterDiscTotal: totalAfter, vdpp: me.detail.AfterDiscDPP }).
                        success(function (data, status, headers, config) {
                            if (data.success) {
                                var DPP = data.data.dpp;
                                var PPN = data.data.ppn;
                                me.detail.AfterDiscDPP = DPP;
                                me.detail.AfterDiscPPn = PPN;
                                me.detail.AfterDiscPPnBM = data.data.ppnBm;
                                me.detail.DiscExcludePPn = totalBefore - DPP;
                                me.detail.DiscIncludePPn = totalBefore - totalAfter
                            } else {
                                MsgBox(data.message, MSG_INFO);
                            }
                        }).
                            error(function (data, status, headers, config) {
                                MsgBox("Terjadi kesalahan dalam proses perhitungan, silahkan hubungi SDMS support!", MSG_INFO);
                            });

                    //} else {
                    //    msgbox("Nilai yang dimasukan lebih kecil dari harga batas bawah!", msg_info)
                    //    //me.detail.afterdisctotal = me.detail.totalminstaff;
                    //    //totalafter = me.detail.totalminstaff;
                    //    $http.post('om.api/salesorder/docount', { custcode: me.data.customercode, slsmodelcode: me.data.salesmodelcode, val: $('#chktotalpriceafter').prop('checked'), beforedisctotal: totalbefore, afterdisctotal: totalafter, vdpp: me.detail.afterdiscdpp }).
                    //    success(function (data, status, headers, config) {
                    //        if (data.success) {
                    //            var dpp = data.data.dpp;
                    //            var ppn = data.data.ppn;
                    //            me.detail.afterdiscdpp = dpp;
                    //            me.detail.afterdiscppn = ppn;
                    //            me.detail.afterdiscppnbm = data.data.ppnbm;
                    //            me.detail.discincludeppn = totalbefore - totalafter
                    //        } else {
                    //            msgbox(data.message, msg_info);
                    //        }
                    //    }).
                    //        error(function (data, status, headers, config) {
                    //            msgbox("Terjadi kesalahan dalam proses perhitungan, silahkan hubungi sdms support!", msg_info);
                    //    });
                    //}
                } else {
                    MsgBox("Nilai yang dimasukan lebih besar dari harga batas atas!", MSG_INFO)
                    me.detail.AfterDiscTotal = totalBefore;
                    totalAfter = totalBefore;
                    $http.post('om.api/SalesOrder/doCount', { CustCode: me.data.CustomerCode, SlsModelCode: me.data.SalesModelCode, val: "true", beforeDiscTotal: totalBefore, afterDiscTotal: totalAfter, vdpp: me.detail.AfterDiscDPP }).
                    success(function (data, status, headers, config) {
                        if (data.success) {
                            var DPP = data.data.dpp;
                            var PPN = data.data.ppn;
                            me.detail.AfterDiscDPP = DPP;
                            me.detail.AfterDiscPPn = PPN;
                            me.detail.AfterDiscPPnBM = data.data.ppnBm;
                            me.detail.DiscIncludePPn = totalBefore - totalAfter
                        } else {
                            MsgBox(data.message, MSG_INFO);
                        }
                    }).
                        error(function (data, status, headers, config) {
                            MsgBox("Terjadi kesalahan dalam proses perhitungan, silahkan hubungi SDMS support!", MSG_INFO);
                        });
                    //me.detail.afterDiscTotal = totalBefore;
                    //totalAfter = totalBefore;
                    ////me.detail.afterDiscDpp = 0;
                    //$http.post('om.api/SalesOrder/doCount', { CustCode: me.data.CustomerCode, SlsModelCode: me.data.SalesModelCode, val: $('#chkTotalPriceAfter').prop('checked'), beforeDiscTotal: totalBefore, afterDiscTotal: totalAfter, vdpp: me.detail.AfterDiscDPP }).
                    //    success(function (data, status, headers, config) {
                    //        if (data.success) {
                    //            var DPP = data.data.dpp;
                    //            var PPN = data.data.ppn;
                    //            me.detail.AfterDiscDPP = DPP;
                    //            me.detail.AfterDiscPPn = PPN;
                    //            me.detail.AfterDiscPPnBM = data.data.ppnBm;
                    //            me.detail.DiscExcludePPn = totalBefore - DPP;
                    //            me.detail.DiscIncludePPn = totalBefore - totalAfter
                    //        } else {
                    //            MsgBox(data.message, MSG_INFO);
                    //        }
                    //    }).
                    //        error(function (data, status, headers, config) {
                    //            MsgBox("Terjadi kesalahan dalam proses perhitungan, silahkan hubungi SDMS support!", MSG_INFO);
                    //    });
                }
            }
        }
        me.Apply();
    });

    /*
    $("[name='AfterDiscTotal']").on('blur', function () {
        var totalBefore = $("[name='BeforeDiscTotal']").val();
        var totalAfter = $("[name='AfterDiscTotal']").val();
        totalAfter = totalAfter.split(',').join('');
        totalBefore = totalBefore.split(',').join('');
        if (totalAfter != 0) {
            if (totalAfter != "") {
                me.CheckBatasAtasBatasBawah();
                totalAfter = me.detail.AfterDiscTotal;
                me.detail.AfterDiscDPP = 0;
                $http.post('om.api/SalesOrder/doCount', { CustCode: me.data.CustomerCode, SlsModelCode: me.data.SalesModelCode, val: $('#chkTotalPriceAfter').prop('checked'), beforeDiscTotal: totalBefore, afterDiscTotal: totalAfter, vdpp: me.detail.AfterDiscDPP }).
                        success(function (data, status, headers, config) {
                            if (data.success) {
                                var DPP = data.data.dpp;
                                var PPN = data.data.ppn;
                                me.detail.AfterDiscDPP = DPP;
                                me.detail.AfterDiscPPn = PPN;
                                me.detail.AfterDiscPPnBM = data.data.ppnBm;
                                me.detail.DiscExcludePPn = totalBefore - DPP;
                                me.detail.DiscIncludePPn = totalBefore - totalAfter
                            } else {
                                MsgBox(data.message, MSG_INFO);
                            }
                        }).
                            error(function (data, status, headers, config) {
                                MsgBox("Terjadi kesalahan dalam proses perhitungan, silahkan hubungi SDMS support!", MSG_INFO);
                            });
            }
        }
        me.Apply();
    });
    */

    $("[name='AfterDiscTotal5']").on('blur', function () {
        var totalBefore = $("[name='RetailPrice']").val();
        var totalAfter = $("[name='AfterDiscTotal5']").val();
        if (totalBefore != 0) {
            $http.post('om.api/SalesOrder/doCountPart', { CustCode: me.data.CustomerCode, TotPartAftDisc: totalAfter, TotPartBefDisc: totalBefore }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail5.AfterDiscTotal5 = totalBefore;
                        console.log(data.data.afterDiscDPP);
                        me.detail5.AfterDiscDPP = data.data.aftDiscDPP;
                        me.detail5.AfterDiscPPn = data.data.aftDiscPPn;
                        me.detail5.DiscExcludePPn = data.data.discExcPPn;
                    } else {
                        me.detail5.AfterDiscDPP = 0;
                        me.detail5.AfterDiscPPn = 0;
                        me.detail5.DiscExcludePPn = 0;
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox("Proses perhitungan gagal!", MSG_ERROR);
                });
        }



        //totalAfter = totalAfter.split(',').join('');
        //totalBefore = totalBefore.split(',').join('');
        //if (totalAfter != 0) {
        //    if (totalAfter != "") {
        //        if ((totalBefore - totalAfter) >= 0) {
        //            if ((totalAfter - me.detail.TotalMinStaff) >= 0) {
        //                //var DPP = Math.round((parseFloat(totalAfter) * 10) / 11);
        //                var DPP = Math.round((totalAfter * taxPct) / 11);
        //                var PPN = totalAfter - DPP
        //                me.detail5.AfterDiscDPP = DPP;
        //                me.detail5.AfterDiscPPn = PPN;
        //                me.detail5.DiscExcludePPn = totalBefore - DPP;
        //            } else {
        //                MsgBox("Nilai yang dimasukan lebih kecil dari harga batas bawah!", MSG_INFO)
        //                me.detail.AfterDiscTotal5 = me.detail.TotalMinStaff;
        //                totalAfter = me.detail.TotalMinStaff
        //                var DPP = Math.round((totalAfter * taxPct) / 11);
        //                var PPN = totalAfter - DPP
        //                me.detail5.AfterDiscDPP = DPP;
        //                me.detail5.AfterDiscPPn = PPN;
        //                me.detail5.DiscExcludePPn = totalBefore - DPP;
        //            }
        //        } else {
        //            MsgBox("Nilai yang dimasukan lebih besar dari harga batas atas!", MSG_INFO)
        //            me.detail.AfterDiscTotal5 = totalBefore;
        //            totalAfter = totalBefore;
        //            var DPP = Math.round((totalAfter * taxPct) / 11);
        //            var PPN = totalAfter - DPP
        //            me.detail5.AfterDiscDPP = DPP;
        //            me.detail5.AfterDiscPPn = PPN;
        //            me.detail5.DiscExcludePPn = totalBefore - DPP;
        //        }
        //    }
        //}
    });

    $('#SalesModelCode').on('blur', function (e) {
        if ($('#SalesModelCode').val() || $('#SalesModelCode').val() != '') {
            $http.post('om.api/SalesOrder/Blur?IsApa=SalesModelCode&Key=' + $('#SalesModelCode').val() + '&Key2=').
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.detail.SalesModelCode = v.SalesModelCode;
                } else {
                    $('#SalesModelCode').val('');
                    me.SalesModelCode();
                }
            });
        } else {
            $('#SalesModelCode').val('');
            me.SalesModelCode();
        }
    });

    $('#SalesModelYear').on('blur', function (e) {
        if ($('#SalesModelYear').val() || $('#SalesModelYear').val() != '') {
            $http.post('om.api/SalesOrder/Blur?IsApa=SalesModelYear&Key=' + $('#SalesModelCode').val() + '&Key2=' + $('#SalesModelYear').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.detail2.SalesModelYear = v.SalesModelYear;
                    me.detail2.SalesModelDesc = v.SalesModelDesc;
                } else {
                    $('#SalesModelYear').val('');
                    $('#SalesModelDesc').val('');
                    me.SalesModelYear();
                }
            });
        } else {
            $('#SalesModelYear').val('');
            $('#SalesModelDesc').val('');
            me.SalesModelYear();
        }
    });

    $('#ColourCode').on('blur', function (e) {
        if ($('#ColourCode').val() || $('#ColourCode').val() != '') {
            $http.post('om.api/SalesOrder/Blur?IsApa=ColourCode&Key=COLO&Key2=' + $('#ColourCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.detail2.ColourCode = v.ColourCode;
                    me.detail2.ColourName = v.ColourName;
                } else {
                    $('#ColourCode').val('');
                    $('#ColourName').val('');
                    me.BrowseColour();
                }
            });
        } else {
            $('#ColourCode').val('');
            $('#ColourName').val('');
            me.BrowseColour();
        }
    });

    me.chk_change = function () {
        if (me.detail.chkTotalPriceAfter == true) {
            $('#AfterDiscTotal').attr('disabled', false);
            $('#AfterDiscDPP').attr('disabled', true);
            $('#AfterDiscPPn').attr('disabled', true);
            $('#AfterDiscPPnBM').attr('disabled', true);

        } else {
            $('#AfterDiscTotal').attr('disabled', true);
            $('#AfterDiscDPP').attr('disabled', false);
            //$('#AfterDiscPPn').attr('disabled', false);
            //$('#AfterDiscPPnBM').attr('disabled', false);
        }
    };

    me.CustomerShow = function () {
        var custCode = me.data.CustomerCode;
        Wx.loadForm();
        Wx.showForm({ url: "gn/master/customer", params: custCode });
    };

    me.OwnershipVehicle = function () {
        var SONo = $('#SONo').val();
        localStorage.setItem('SONo', SONo);
        Wx.loadForm();
        Wx.showForm({ url: "om/sales/ownershipvehicle", params: SONo });
        var stop = setInterval(function () {
            var b = localStorage.getItem("RefreshGrid");
            console.log(b);
            if (b == "true") {
                clearInterval(stop);
                me.startEditing();
                me.default();
                me.disableHdr();
                me.disableDtlSalesModel();
                me.disableDtlSalesModelColour();
                me.disableDtlSalesVin();
                me.disableDtlSalesModelOther();
                me.disableDtlSalesAccs();
                $('#btnDelete').removeAttr('ng-show');
                $('#btnDelete').hide();

                Wx.Success('Proses Approve SO berhasil!');
            }
        }, 3000);
    };

    me.CheckBatasAtasBatasBawah = function () {
        var totalBefore = $("[name='BeforeDiscTotal']").val();
        var totalAfter = $("[name='AfterDiscTotal']").val();
        totalAfter = totalAfter.split(',').join('');
        totalBefore = totalBefore.split(',').join('');
        if ((totalBefore - totalAfter) >= 0) {
            if ((totalAfter - me.detail.TotalMinStaff) >= 0) {
            } else {
                MsgBox("Nilai yang dimasukan lebih kecil dari harga batas bawah!", MSG_INFO)
            }
        } else {
            MsgBox("Nilai yang dimasukan lebih besar dari harga batas atas!", MSG_INFO)
            me.detail.AfterDiscTotal = totalBefore;
        }
    };

    me.printPreview = function () {
        me.popupPrintChoose();
    };

    me.popupPrintChoose = function () {
        BootstrapDialog.show({
            message: $(
                '<div class="container">' +
                '<div class="row">' +

                '<input type="checkbox" name="sparePart" id="sizeType0" value="0">&nbsp Print Aksesoris/ Spart Part</div>' +
                '<div class="row">' +

                '<input type="radio" name="sizeType" id="sizeType1" value="1" checked>&nbsp Print Pesanan Penjualan</div>' +
                '<div class="row">' +

                '<input type="radio" name="sizeType" id="sizeType2" value="2">&nbsp Print Pesanan Penjualan dengan Rangka/ Mesin</div>' +
                '<div class="row">' +

                '<input type="radio" name="sizeType" id="sizeType3" value="3">&nbsp Print Catatan Penjualan</div>' +

                '<div class="row">' +

                '<input type="radio" name="sizeType" id="sizeType4" value="4">&nbsp Print Lain-lain</div>'),
            closable: false,
            draggable: true,
            type: BootstrapDialog.TYPE_INFO,
            title: 'Print Pesanan Penjualan',
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {
                    me.popupPrintPage();
                    dialogRef.close();
                }
            }, {
                label: ' Cancel',
                cssClass: 'btn-warning icon-remove',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
        });
    };

    me.popupPrintPage = function () {
        var sizeType = $('input[name=sizeType]:checked').val();
        var issparePart = $('input[name=sparePart]:checked').val();
        var hidePart = true;
        if (!issparePart) {
            hidePart = true;
        } else hidePart = false;

        BootstrapDialog.show({
            message: $(
                '<div class="container">' +
                '<div class="row">' +

                '<input type="radio" name="sizeType" id="sizeType1" value="full" checked>&nbsp Print Satu Halaman</div>' +

                '<div class="row">' +

                '<input type="radio" name="sizeType" id="sizeType2" value="half">&nbsp Print Setengah Halaman</div>'),
            closable: false,
            draggable: true,
            type: BootstrapDialog.TYPE_INFO,
            title: 'Print',
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {
                    //me.CheckBatasAtasBatasBawah();
                    me.printPreviewShow(sizeType, hidePart);
                    dialogRef.close();
                }
            }, {
                label: ' Cancel',
                cssClass: 'btn-warning icon-remove',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
        });
    };

    me.printPreviewShow = function (sizeType, hidePart) {
        var ukuran = $('input[name=sizeType]:checked').val();
        var reportID = "";
        var par = [
           me.data.SONo,
           me.data.SONo
        ];

        if (sizeType == "1") {
            if (ukuran == "full") reportID = "OmRpSalesTrn001";
            else reportID = "OmRpSalesTrn001D";
            par = [
                 me.data.SONo,
                 me.data.SONo,
                 'profitcenter',
                 "",
                 hidePart
            ];
            var params = {
                SONo: me.data.SONo
            }
            $http.post("om.api/SalesOrder/PrintSO", params)
            .success(function (result) {
                if (result.success) {
                    if (result.isDataExist) {
                        //me.RetrieveData(result);
                        me.checkStatus(result.Status);
                        //me.popupPrintChoose();
                        me.disableHdrSts1();
                        if (result.Status == 1) {
                            $http.post('om.api/SalesOrder/checkBottomPrice', { SONo: me.data.SONo }).
                                success(function (data, status, headers, config) {
                                    if (data.success) {
                                        $('#btnApprove').removeAttr('disabled');
                                    } else {
                                        MsgBox(data.message, MSG_INFO)
                                        $('#btnApprove').attr('disabled', 'disabled');
                                    }
                                });
                        }

                        //if ((me.detil.AfterDiscTotal - me.detail.TotalMinStaff) >= 0) {
                        //    $('#btnApprove').removeAttr('disabled');
                        //} else {
                        //    $http.post('om.api/SalesOrder/CheckApproval').
                        //    success(function (Result, status, headers, config) {
                        //        if (Result == 1) {
                        //            $('#btnApprove').removeAttr('disabled');
                        //        } else {
                        //            $('#btnApprove').attr('disabled', 'disabled');
                        //        }
                        //    });
                        //}
                    }
                    else {
                        if (result.message != "") {
                            MsgBox(result.message, MSG_WARNING);
                        }
                        else {
                            // me.popupPrintChoose();
                        }
                    }
                }
                else {
                    MsgBox(result.message, MSG_WARNING);
                    console.log(result.error_log);
                }
            })
            .error(function (result) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_INFO);
            });
        }

        if (sizeType == "2") {
            if (ukuran == "full") reportID = "OmRpSalesTrn001A";
            else reportID = "OmRpSalesTrn001E";
        }

        if (sizeType == "3") {
            if (ukuran == "full") reportID = "OmRpSalesTrn001B";
            else reportID = "OmRpSalesTrn001F";
            par = [
                 me.data.SONo,
                 me.data.SONo,
                 hidePart
            ];
        }

        if (sizeType == "4") {
            if (ukuran == "full") reportID = "OmRpSalesTrn001C";
            else reportID = "OmRpSalesTrn001G";
        }

        var rparam = 'Print SO';
        Wx.showPdfReport({
            id: reportID,
            textprint: true,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    };

    me.approve = function () {
        MsgConfirm("Apakah anda yakin?", function (result) {
            if (result) {
                $http.post('om.api/SalesOrder/Approve', me.data).
                 success(function (data, status, headers, config) {
                     if (data.status) {
                         if (data.data.ProductType == '4W') {
                             me.OwnershipVehicle()
                         } else if (data.data.ProductType == '2W') {
                             me.approveSO();
                         } else {
                             MsgBox(data.message, MSG_INFO);
                             $('#btnUnapprove').removeAttr('disabled');
                             $('#btnReject').removeAttr('disabled');
                         }
                     } else {
                         MsgBox(data.message, MSG_INFO);
                     }
                 }).
                    error(function (data, status, headers, config) {
                        //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                        MsgBox(data.message, MSG_INFO);
                    });
            }
        });
    };

    me.approveSO = function () {
        var params = {
            SONo: me.data.SONo,
            islinkITS: true,
            additionalOwnership: null
        };
        localStorage.setItem('SONo', me.data.SONo);
        $http.post('om.api/SalesOrder/approveSO', params)
        .success(function (data, status, headers, config) {
            if (data.success) {
                me.startEditing();
                me.default();

                Wx.Success(data.message);
            } else {
                MsgBox(data.message, MSG_INFO);
            }
        }).error(function (data, status, headers, config) {
            MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
        });
    };

    me.unapprove = function () {
        MsgConfirm("Apakah anda yakin?", function (result) {

            if (result) {
                me.data.SONumber = me.data.SONo;
                $http.post('om.api/SalesOrder/UnApproveCheck', me.data).
            success(function (data, status, headers, config) {
                if (data.data.state == "warning") {
                    MsgBox(data.message, MSG_INFO);
                } else {
                    var params = {
                        model: me.data,
                        isDO: data.data.isDO

                    };
                    MsgConfirm(data.message, function (result) {
                        if (result) {

                            $http.post('om.api/SalesOrder/UnApprove', params).
                               success(function (data, status, headers, config) {
                                   if (data.status) {
                                       if (data.data.SOStatus == "UNAPPROVED") {
                                           var stat = 0;
                                       }
                                       me.checkStatus(stat);
                                       MsgBox(data.message, MSG_INFO);
                                       $('#btnUnapprove').attr('disabled', true);
                                       $('#btnReject').attr('disabled', true);
                                       $('#btnApprove').attr('disabled', true);
                                       $("#btnCustomerCode,#btnSalesman,#btnWarehouseCode,#btnTOPCode,#btnShipTo").prop('disabled', false);
                                   }
                               }).
                               error(function (data, status, headers, config) {
                                   //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                                   MsgBox(data.message, MSG_INFO);
                               });
                        }
                    });

                }
            }).
            error(function (data, status, headers, config) {
                //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                MsgBox(data.message, MSG_INFO);
            });
            }
        });
    };

    me.reject = function () {
        MsgConfirm("Apakah anda yakin?", function (result) {
            if (result) {
                $http.post('om.api/SalesOrder/Reject', me.data).
             success(function (data, status, headers, config) {
                 if (data.status) {
                     me.checkStatus(data.data.Status);
                     $('#btnUnapprove').attr('disabled', true);
                     $('#btnReject').attr('disabled', true);
                     $('#btnApprove').attr('disabled', true);
                     MsgBox(data.message, MSG_INFO);
                 } else {
                     MsgBox(data.message, MSG_INFO);
                 }
             }).
             error(function (data, status, headers, config) {
                 //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                 MsgBox(data.message, MSG_INFO);
             });
            }
        });
    };

    me.Tenor = [
                   { "text": "12", "value": "12" },
                   { "text": "24", "value": "24" },
                   { "text": "36", "value": "36" },
                   { "text": "48", "value": "48" },
                   { "text": "60", "value": "60" },
    ];

    me.gridSalesModel = new webix.ui({
        container: "wxSalesModel",
        view: "wxtable", css: "alternating",
        scrollX: true,
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 140 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 140 },
            { id: "QuantitySO", header: "Jml. SO.", width: 120 },
            { id: "AfterDiscTotal", header: "Harga Setelah Diskon", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscDPP", header: "DPP Stelah Diskon", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPnBM", header: "PPnBM Setalah Diskon", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersDPP", header: "DPP Lain-lain", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersPPn", header: "PPn Lain-lain", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "ShipAmt", header: "Ongkos Kirim", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "DepositAmt", header: "Deposit", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "OthersAmt", header: "Lain-lain", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", width: 200 },
            { id: "BeforeDiscTotal", header: "Harga Sebelum Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "DiscIncludePPn", header: "Diskon", width: 140, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridSalesModel.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.gridSalesModel.getSelectedId().id);
                    me.detail.oid = me.gridSalesModel.getSelectedId();
                    me.detail3.ChassisCode = me.detail.ChassisCode;
                    me.detail.GroupPriceCode = me.data.GroupPriceCode;
                    me.getQTYUnit();
                    //console.log("status: " + me.data.Status);
                    if (me.data.Status < 2) {
                        me.allowInputDetail = true;
                        me.isdetail = false;
                        //$('#btnAddModel','#btnAddColour').hide()
                        //$('#btnDeleteModel','#btnDeleteColour').hide()
                        //$('#btnCancelModel','#btnCancelColour').hide();
                    }
                    $http.post('om.api/MstPriceListJual/CekData', me.detail).
                    success(function (data, status, headers, config) {
                        if (data.success) {
                            //me.detail.BeforeDiscTotal = data.data.Total;
                            me.detail.TotalMinStaff = data.data.TotalMinStaff;
                            me.ReformatNumber();
                        }
                        //} else {
                        //    me.detail.BeforeDiscTotal = 0;
                        //    me.detail.AfterDiscDPP = 0;
                        //    me.detail.AfterDiscPPn = 0;
                        //    me.detail.AfterDiscPPnBM = 0;
                        //    me.detail.AfterDiscTotal = 0;
                        //    me.detail.DiscIncludePPn = 0;
                        //}
                    }).
                error(function (data, status, headers, config) {
                    alert('error');
                });
                    $('#btnAddColour').removeAttr('disabled')
                    me.Apply();
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
            { id: "ColourCode", header: "Warna", width: 100 },
            { id: "Quantity", header: "Jumlah", width: 100 },
            { id: "Remark", header: "Keterangan", width: 200 }
        ],
        on: {
            onSelectChange: function () {
                var number = me.data.ProspectNo != "" ? me.data.ProspectNo : "";
                if (me.gridColourInfo.getSelectedId() !== undefined) {
                    me.detail2 = this.getItem(me.gridColourInfo.getSelectedId().id);
                    me.detail2.oid = me.gridColourInfo.getSelectedId();
                    me.loadDetail(me.detail3, 3);
                    me.loadDetail(me.detail4, 4);
                    //
                    if (number != null)
                    {
                        $("#Quantity").attr('disabled', 'disabled');
                    } else {
                        $("#Quantity").removeAttr('disabled');
                    }
                    if (me.data.Status < 2) {
                        me.isdetail2 = false;
                        me.isdetail3 = false;
                        me.allowInputDetail2 = true;
                        me.allowInputDetail3 = true;
                        me.allowInputDetail4 = true;
                        me.allowInputDetail5 = true;
                    }
                    else {
                        //$('#btnAddColour').hide();
                        $('#btnAddColour').attr('disabled', 'disabled');
                    }
                    me.Apply();
                    $('#btnColourCode').attr('disabled', 'disabled');
                    //$('#ChassisNo').attr('disabled', 'disabled');
                }
            }
        }
    });

    me.gridOthersInfo = new webix.ui({
        container: "wxOthersInfo",
        view: "wxtable", css: "alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 160,
        width: 520,
        columns: [
            { id: "ChassisCode", header: "Kode Rangka", width: 140, format: me.replaceNull },
            { id: "ChassisNo", header: "No Rangka", width: 140, format: me.replaceNull },
            { id: "EndUserName", header: "Nama STNK", width: 140, format: me.replaceNull },
            { id: "EndUserAddress1", header: "Alamat #1", width: 140, format: me.replaceNull },
            { id: "EndUserAddress2", header: "Alamat #2", width: 140, format: me.replaceNull },
            { id: "EndUserAddress3", header: "Alamat #3", width: 140, format: me.replaceNull },
            { id: "SupplierBBN", header: "Pemasok BNN", width: 140, format: me.replaceNull },
            { id: "CityCode", header: "Kota", width: 120, format: me.replaceNull },
            { id: "BBN", header: "BBN", width: 120, format: me.replaceNull },
            { id: "KIR", header: "KIR", width: 120, format: me.replaceNull },
            { id: "Remark", header: "Keterangan", width: 200, format: me.replaceNull },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridOthersInfo.getSelectedId() !== undefined) {
                    me.detail3 = this.getItem(me.gridOthersInfo.getSelectedId().id);
                    me.detail3.oid = me.gridOthersInfo.getSelectedId();
                    if (me.data.Status < 2) {
                        me.isdetail4 = false;
                        me.isdetail3 = false;
                    }
                    me.Apply();
                    //$('#ChassisNo').attr('disabled', 'disabled');
                }
            }
        }
    });

    me.gridAccesories = new webix.ui({
        container: "wxAccesories",
        view: "wxtable", css: "alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 160,
        columns: [
            { id: "OtherCode", header: "Kode Aks. Lain", width: 140 },
            { id: "OtherDesc", header: "Nama Aks. Lain", width: 200 },
            { id: "BeforeDiscTotal", header: "Total Sebelum Diskon", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscTotal", header: "Total Setelah Diskon", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscDPP", header: "DPP Setelah Diskon", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AfterDiscPPn", header: "PPn Setelah Diskon", width: 160, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Remark", header: "Keterangan", width: 250 }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridAccesories.getSelectedId() !== undefined) {
                    me.detail4 = this.getItem(me.gridAccesories.getSelectedId().id);
                    me.detail4.oid = me.gridAccesories.getSelectedId();
                    if (me.data.Status < 2) {
                        me.isdetail5 = false;
                        me.isdetail4 = false;
                    }
                    me.Apply();
                }
            }
        }
    });

    me.gridSparepart = new webix.ui({
        container: "wxSparepart",
        view: "wxtable", css: "alternating",
        scrollX: true,
        scrollY: true,
        autoHeight: false,
        height: 160,
        width: 520,
        columns: [
            { id: "PartNo", header: "No.Part", width: 140 },
            { id: "PartName", header: "Nama Part", width: 140 },
            { id: "ProductType", header: "Jenis Part", width: 120 },
            { id: "DemandQty", header: "Jumlah", width: 120 },
            { id: "RetailPrice", header: "Harga", width: 120, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "Total", header: "Total", width: 200, format: webix.i18n.numberFormat, css: { "text-align": "right" } }
        ],
        on: {
            onSelectChange: function () {
                if (me.gridSparepart.getSelectedId() !== undefined) {
                    me.detail5 = this.getItem(me.gridSparepart.getSelectedId().id);
                    me.detail5.oid = me.gridSparepart.getSelectedId();
                    if (me.data.Status < 2) me.isdetail5 = false;
                    me.getQTYUnit();
                    me.Apply();
                    $('#btnPartNo').attr('disabled', 'disabled')
                }
            }
        }
    });

    me.getQTYUnit = function () {
        $http.post('om.api/SalesOrder/getQtyUnit', { SONo: me.data.SONo }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    console.log(data.qty);
                    me.detail5.QtyUnit = data.qty;
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Get quantity unit gagal!', MSG_ERROR);
            });
    }

    //me.Test = function () {
    //    $http.post('om.api/SalesOrder/TestOtom').
    //        success(function (data, status, headers, config) {
    //            MsgBox(data, MSG_INFO);
    //        })
    //}

    webix.event(window, "resize", function () {
        me.gridSalesModel.adjust();
        me.gridColourInfo.adjust();
        me.gridOthersInfo.adjust();
        me.gridAccesories.adjust();
        me.gridSparepart.adjust();
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Sales Order",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnloptions",
                items: [
                      {
                          type: "buttons", cls: "span6", items: [
                                 { name: "btnApprove", text: "Approve", cls: "btn btn-info", icon: "icon-ok", click: "approve()", disable: true },
                                 { name: "btnUnapprove", text: "UnApprove", cls: "btn btn-info", icon: "icon-ok", click: "unapprove()", disable: true },
                                 { name: "btnReject", text: "Reject", cls: "btn btn-info", icon: "icon-ok", click: "reject()", disable: true },
                                 { name: "btnPelanggan", text: "Pelanggan", cls: "btn btn-info", icon: "icon-ok", click: "CustomerShow()" },
                                 //{ name: "btnTest", text: "Test", cls: "btn btn-info", icon: "icon-ok", click: "Test()" }
                                 //{ name: "btnApproveSO", text: "ApproveSO", cls: "btn btn-info", icon: "icon-ok", click: "approveSO()" },

                          ]
                      },
                      { type: "label", name: "statusLbl", text: "NEW", cls: "span2", style: 'font-size: 34px; font-weight: bold; color: red;' },
                ]
            },
            {// 
                name: "pnlPO",
                items: [
                    { name: "SONo", model: "data.SONo", text: "No.SO", cls: "span4", readonly: true, placeHolder: 'SOR/YY/XXXXXX' },
                    { name: "SODate", model: "data.SODate", text: "Tgl.SO", cls: "span4", type: "ng-datepicker" },
                    { name: "RefferenceNo", model: "data.RefferenceNo", text: "No.Reff", cls: "span4", type: "popup", click: "ReffInv()", disable: true, maxlength: 15 },
                    {
                        text: "Tgl.Reff",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: 'isC1', model: "data.isC1", type: 'ng-check', text: '', cls: 'span1', float: 'left' },
                            { name: "RefferenceDate", text: "Reff Inv. Date", cls: "span6", type: 'ng-datepicker', disable: true },
                        ]
                    },
                    { name: 'isC2', model: "data.isC2", type: 'ng-check', text: 'Direct Sales', cls: 'span1 full', float: 'left' },
                    {
                        text: "No.ITS",
                        type: "controls",
                        cls: "span8",
                        show: "isShow",
                        items: [
                           { name: "ProspectNo", model: "data.ProspectNo", text: "", cls: "span2", readonly: false, type: "popup", maxlength: 15, click: "BrowseITS()", validasi: "required" },
                           { name: "label1", type: "label", text: "Tipe Kendaraan", cls: "span1 mylabel" },
                           { name: "VehicleType", text: "", cls: "span2", readonly: false, disable: true },
                           { name: "label2", type: "label", text: "No.SKPK", cls: "span1 mylabel" },
                           { name: "SKPKNo", text: "No.SKPK", cls: "span2", type: "popup", click: "SKPKNo()", required: true, validasi: "required", maxlength: 15 },
                        ]
                    },

                   {
                       text: "Pelanggan",
                       type: "controls",
                       cls: "span4",
                       required: true,
                       items: [
                           { name: "CustomerCode", model: "data.CustomerCode", placeHolder: "Pelanggan", cls: "span3", type: "popup", click: "Customer()", required: true, validasi: "required", maxlength: 15 },
                           { name: "CustomerName", model: "data.CustomerName", placeHolder: "Nama Pelanggan", cls: "span5", readonly: true },
                       ]
                   },
                    {
                        text: "TOP",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            { name: "TOPCode", model: "data.TOPCode", placeHolder: "TOP", cls: "span3", type: "popup", click: "TOP()", required: true, validasi: "required", maxlength: 15 },
                            { name: "TOPDays", model: "data.TOPDays", placeHolder: "Nama TOP", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Salesman",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            { name: "Salesman", model: "data.Salesman", placeHolder: "Salesman", cls: "span3", type: "popup", click: "Salesman()", required: true, validasi: "required", maxlength: 15 },
                            { name: "SalesmanName", model: "data.SalesmanName", placeHolder: "Nama Salesman", cls: "span5", readonly: true },
                        ]
                    },
                        {
                            name: "PaymentType", model: "data.LookUpValueName", text: "Di Bayar dengan", cls: "span4"
                        },
                    {
                        text: "Tagih Ke",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "BillTo", model: "data.BillTo", cls: "span3", type: "text", readonly: true },
                            { name: "BillName", model: "data.BillName", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Kirim Ke",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "ShipTo", model: "data.ShipTo", cls: "span3", type: "popup", click: "ShipTo()", maxlength: 15 },
                            { name: "ShipName", model: "data.ShipName", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Gudang",
                        type: "controls",
                        cls: "span4",
                        required: true,
                        items: [
                            { name: "WareHouseCode", model: "data.WareHouseCode", cls: "span3", text: "Gudang", type: "popup", click: "WareHouseCode()", required: true, validasi: "required", maxlength: 15 },
                            { name: "WareHouseName", model: "data.WareHouseName", text: "Nama Gudang", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Group Price Code",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "GroupPriceCode", model: "data.GroupPriceCode", placeHolder: "Group Price Code", cls: "span3", readonly: true },
                            { name: "GroupPriceName", model: "data.GroupPriceName", placeHolder: "Group Price Desc", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Leasing",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: 'isLeasing', model: "data.isLeasing", type: 'ng-check', text: '', cls: 'span1', float: 'left' },
                            { name: "LeasingCo", cls: "span3", disable: "isDisable", type: "popup", click: "LeasingCo()", maxlength: 15 },
                            { name: "LeasingCoName", disable: "isDisable", cls: "span4", readonly: true },
                        ]
                    },
                    { name: "Installment", model: "data.Installment", text: "Angsuran", cls: "span4", disable: "isDisable", type: "select2", datasource: "Tenor" },
                    { name: "FinalPaymentDate", model: "data.FinalPaymentDate", text: "bulan,Tgl.Lunas", cls: "span4", type: 'ng-datepicker', disable: "isDisable" },
                    { name: "Insurance", model: "data.Insurance", text: "Asuransi", cls: "span4" },
                    { name: "PrePaymentAmt", model: "data.PrePaymentAmt", text: "Uang Muka", cls: "span4 number-int", maxlength: 19 },
                    {
                        text: "Pada Tgl.",
                        type: "controls",
                        cls: "span4",
                        items: [
                                  { name: 'isC3', type: 'ng-check', text: '', cls: 'span1', float: 'left' },
                                  { name: "PrePaymentDate", model: "data.PrePaymentDate", text: "", cls: "span7", type: 'ng-datepicker', disable: true },
                        ]
                    },
                    {
                        text: "Diterima oleh",
                        type: "controls",
                        cls: "span8",
                        items: [
                                    { name: "PrePaymentBy", model: "data.PrePaymentBy", cls: "span3", text: "", type: "text", type: "popup", click: "Salesman(2)", maxlength: 15 },
                                    { name: "PrePaymentName", model: "data.PrePaymentName", text: "", cls: "span5", readonly: true },
                        ]
                    },
                    { name: "CommissionBy", model: "data.CommissionBy", text: "Mediator", cls: "span4" },
                    { name: "CommissionAmt", model: "data.CommissionAmt", text: "Komisi", cls: "span4 number-int", maxlength: 19 },
                    { name: "PONo", model: "data.PONo", text: "No.PO", cls: "span3", maxlength: 15 },
                    { name: "ContractNo", model: "data.ContractNo", text: "No.Kontak", cls: "span2", maxlength: 15 },
                    {
                        text: "Tgl.Dibutuhkan",
                        type: "controls",
                        cls: "span3",
                        items: [
                                  { name: 'isC4', type: 'ng-check', text: '', cls: 'span1', float: 'left' },
                                  { name: "RequestDate", model: "data.RequestDate", text: "", cls: "span7", type: 'ng-datepicker', disable: true },
                        ]
                    },

                    { name: "Remark", model: "data.Remark", text: "Remark", cls: "span8" },
                ]
            },
            {
                xtype: "tabs",
                name: "tabDetail",
                items: [
                    { name: "tabPageSalesModel", text: "Sales Model", cls: "active", click: "ClickModelTab()" },
                    { name: "tabPageVehicleInfo", text: "Informasi Kendaraan", click: "ClickVevicleTab()" },
                    { name: "tabPageAccesories", text: "Aksesoris & Sparepart", click: "ClickAccesoriesTab()" },
                ],
            },
            // Tab Sales Model
            {
                name: "tabPageSalesModel",
                cls: "tabDetail tabPageSalesModel",
                items: [
                    { name: "SalesModelCode", model: "detail.SalesModelCode", text: "Sales Model Code", cls: "span3", placeHolder: "Sales Model Code", type: "popup", click: "SalesModelCode()", required: true, style: "background-color: rgb(255, 218, 204)", disable: "isData", maxlength: 20 },
                    {
                        type: "controls",
                        text: "Sales Model Year",
                        cls: "span5",
                        items: [
                            { name: "SalesModelYear", model: "detail.SalesModelYear", placeHolder: "Sales Model Year", cls: "span1", type: "popup", required: true, click: "SalesModelYear()", required: true, style: "background-color: rgb(255, 218, 204)", disable: "isData", maxlength: 4 },
                            { name: "SalesModelDesc", model: "detail.SalesModelDesc", placeHolder: "Sales Model Desc", cls: "span4", readonly: true, style: "width: 263px;" },
                        ]
                    },
                    { name: "ShipAmt", model: "detail.ShipAmt", text: "Ongkos Kirim", placeHolder: "0", cls: "span3 number-int", disable: "isData" },
                    { name: "DepositAmt", model: "detail.DepositAmt", text: "Unit Deposit", placeHolder: "0", cls: "span3 number-int", disable: "isData" },
                    { name: "OthersAmt", model: "detail.OthersAmt", text: "Lain-lain", placeHolder: "0", cls: "span2 number-int", disable: "isData" },
                    { name: "lbl1", type: "label", text: "Harga Sebelum Diskon", style: "font-size: 14px; color: rgb(0, 91, 153)", disable: "isData" },
                    { type: "div", cls: "divider span3 full" },
                    { name: "BeforeDiscTotal", model: "detail.BeforeDiscTotal", text: "Harga Total", placeHolder: "0", cls: "span3 number-int", readonly: true, style: "background-color: rgb(255, 218, 204)", disable: "isData" },
                    {
                        type: "controls",
                        text: "Diskon",
                        cls: "span5",
                        items: [
                            { name: "DiscIncludePPn", model: "detail.DiscIncludePPn", text: "Diskon", cls: "span2 number-int", placeHolder: "0", readonly: true },
                            { type: "label", text: "Keterangan", cls: "span1", style: "line-height: 30px;" },
                            { name: "Remark", model: "detail.Remark", text: "Keterangan", cls: "span4", style: "width: 294px;", disable: "isData" },
                        ]
                    },
                    { name: "lbl2", type: "label", text: "Harga Setelah Diskon", style: "font-size: 14px; color: rgb(0, 91, 153)" },
                    { type: "div", cls: "divider span3 full" },
                    {
                        type: "controls",
                        text: "Harga Total",
                        cls: "span3",
                        items: [
                            { name: "chkTotalPriceAfter", model: "detail.chkTotalPriceAfter", cls: "span1", type: "ng-check", change: "chk_change()", disable: "isData" },
                            { name: "AfterDiscTotal", model: "detail.AfterDiscTotal", text: "0", cls: "span7 number-int", style: "width: 210px", disable: "isData" },
                        ]
                    },
                    {
                        type: "controls",
                        text: "DPP",
                        cls: "span5",
                        items: [
                            { name: "AfterDiscDPP", model: "detail.AfterDiscDPP", cls: "span2 number", placeHolder: "0", disable: true },
                            { type: "label", text: "PPN", cls: "span1", style: "line-height: 30px;" },
                            { name: "AfterDiscPPn", model: "detail.AfterDiscPPn", cls: "span2 number", placeHolder: "0", disable: true },
                            { type: "label", text: "PPnBM", cls: "span1", style: "line-height: 30px;" },
                            { name: "AfterDiscPPnBM", model: "detail.AfterDiscPPnBM", cls: "span2 number", placeHolder: "0", disable: true, style: "width: 163px;" },
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditDetail()", disable: "!allowInputDetail" },
                            { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteDetail()", disable: "isdetail" },
                            { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined" }
                        ]
                    },
                    {
                        name: "wxSalesModel",
                        cls: "tabDetail tabPageSalesModel",
                        type: "wxdiv"
                    },
                ]
            },
            // Tab Informasi Kendaraan
            {
                name: "tabPageVehicleInfo",
                cls: "tabDetail tabPageVehicleInfo",
                items: [
                    { name: "lbl3", type: "label", text: "Detil Warna", cls: "span4", style: "font-size: 14px; color: rgb(0, 91, 153)" },
                    { name: "lbl4", type: "label", text: "Detil Lain-lain", cls: "span4", style: "font-size: 14px; color: rgb(0, 91, 153)" },
                    { type: "div", cls: "divider span3" },
                    { type: "separator", cls: "span1" },
                    { type: "div", cls: "divider span4" },
                    { name: "ColourCode", model: "detail2.ColourCode", text: "Warna", cls: "span3", type: "popup", click: "BrowseColour()", readonly: true, required: true, style: "background-color: rgb(255, 218, 204)" },
                    {
                        text: "Kode/No.Rangka",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "ChassisCode", model: "detail3.ChassisCode", text: "", cls: "span5", readonly: true },
                            { name: "ChassisNo", model: "detail3.ChassisNo", text: "", cls: "span3", type: "popup", click: "ChassisNo()", disable: "isdetail3" },
                        ]
                    },

                    { name: "ColourDescription", model: "detail2.ColourDescription", text: "Nama Warna", cls: "span3", readonly: true },
                    { name: "EndUserName", model: "detail3.EndUserName", text: "Nama STNK", cls: "span5", required: true, disable: "isdetail3", style: "background-color: rgb(255, 218, 204)" },
                    { name: "Quantity", model: "detail2.Quantity", text: "Jumlah", cls: "span3 number-int", placeHolder: "0", style: "background-color: rgb(255, 218, 204)" },
                    { name: "EndUserAddress1", model: "detail3.EndUserAddress1", text: "Alamat STNK", cls: "span5" },
                    { name: "Remark2", model: "detail2.Remark", text: "Keterangan", cls: "span3", type: "textarea", style: "min-height: 70px; max-height: 70px; max-width: 230px;" },
                     {
                         text: "",
                         type: "controls",
                         cls: "span5",
                         items: [
                             { name: "EndUserAddress2", model: "detail3.EndUserAddress2", text: "", cls: "span3" },
                             { name: "EndUserAddress3", model: "detail3.EndUserAddress3", text: "", cls: "span5" },
                         ]
                     },
                    { type: "label", cls: "span3", text: "" },
                    {
                        text: "Pemasok BBN",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "SupplierBBN", model: "detail3.SupplierBBN", text: "", cls: "span3", type: "popup", click: "Pemasok()", disable: "isdetail3" },
                            { name: "SupplierName", model: "detail3.SupplierName", text: "", cls: "span5", readonly: true },
                        ]
                    },
                    { type: "label", cls: "span3", text: "" },
                    {
                        text: "Kota",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "CityCode", model: "detail3.CityCode", text: "Kota", cls: "span3", type: "popup", click: "City()" },
                            { name: "CityDesc", model: "detail3.CityDesc", text: "", cls: "span5", readonly: true },
                        ]
                    },
                    { type: "label", cls: "span3", text: "" },
                    {
                        text: "BBN",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "BBN", model: "detail3.BBN", cls: "span3", text: "BBN" },
                            { type: "label", cls: "span2", text: "KIR", style: "line-height: 30px; text-align: right;" },
                            { name: "KIR", model: "detail3.KIR", cls: "span3", text: "KIR", readonly: true },
                        ]
                    },
                    { type: "label", cls: "span3", text: "" },
                    { name: "Remark3", model: "detail3.Remark", cls: "span5", text: "Keterangan" },
                    {
                        type: "buttons",
                        cls: "span4",
                        items: [
                            { name: "btnAddColour", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditDetail2()", disable: "!allowInputDetail2" },
                            { name: "btnDeleteColour", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteDetail2()", disable: "isdetail2" },
                            { name: "btnCancelColour", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel2()", show: "detail2.oid !== undefined" }
                        ]
                    },
                    {
                        type: "buttons",
                        cls: "span4",
                        items: [
                            { name: "btnAddOthers", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditDetail3()", disable: "!allowInputDetail3" },
                            { name: "btnDeleteOthers", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteDetail3()", disable: "isdetail3" },
                            { name: "btnCancelOthers", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel3()", show: "detail3.oid !== undefined" }
                        ]
                    },
                    {
                        name: "wxColourInfo",
                        cls: "span3",
                        type: "wxdiv"
                    },
                    { type: "sparator", cls: "span1" },
                    {
                        name: "wxOthersInfo",
                        cls: "span4",
                        type: "wxdiv"
                    },
                ]
            },
            // Tab Aksesoris
            {
                name: "tabPageAccesories",
                cls: "tabDetail tabPageAccesories",
                items: [
                    {
                        type: "controls",
                        cls: "span4",
                        text: "Aks. Lain-lain",
                        required: true,
                        items: [
                            { name: "OtherCode", model: "detail4.OtherCode", text: "Aks. Lain-lain", cls: "span3", type: "popup", click: "Accesories()", required: true, style: "background-color: rgb(255, 218, 204)" },
                            { name: "OtherDesc", model: "detail4.OtherDesc", text: "", cls: "span5", readonly: true, required: true },
                        ]
                    },
                     {
                         type: "controls",
                         cls: "span4",
                         text: "No.Part",
                         required: true,
                         items: [
                             { name: "PartNo", model: "detail5.PartNo", text: "Part Code", cls: "span3", type: "popup", click: "PartCode()", required: true, style: "background-color: rgb(255, 218, 204)" },
                             { name: "PartName", model: "detail5.PartName", text: "", cls: "span5", readonly: true, required: true },
                         ]
                     },
                    { name: "lbl5", type: "label", text: "Harga Sebelum Diskon", cls: "span4", style: "font-size: 14px; color: rgb(0, 91, 153)" },
                    { name: "lbl6", type: "label", text: "Harga Sebelum Diskon", cls: "span4", style: "font-size: 14px; color: rgb(0, 91, 153)" },
                    { type: "div", cls: "divider span3" },
                    { type: "sparator", cls: "span1" },
                    { type: "div", cls: "divider span4" },
                    { name: "BeforeDiscTotal4", model: "detail4.TotalBefore", text: "Total", cls: "span4 number-int", placeHolder: "0", required: true, style: "background-color: rgb(255, 218, 204)" },
                    { name: "RetailPrice", model: "detail5.RetailPrice", text: "DPP", cls: "span4 number-int", placeHolder: "0", required: true, readonly: true },
                    { name: "lbl5", type: "label", text: "Harga Setelah Diskon", cls: "span4", style: "font-size: 14px; color: rgb(0, 91, 153)" },
                    { name: "lbl6", type: "label", text: "Harga Setelah Diskon", cls: "span4", style: "font-size: 14px; color: rgb(0, 91, 153)" },
                    { type: "div", cls: "divider span3" },
                    { type: "sparator", cls: "span1" },
                    { type: "div", cls: "divider span4" },
                    { name: "AfterDiscTotal4", model: "detail4.AfterDiscTotal", text: "Total", cls: "span4 number-int", placeHolder: "0", required: true, style: "background-color: rgb(255, 218, 204)" },
                    { name: "AfterDiscTotal5", model: "detail5.AfterDiscTotal", text: "Total", cls: "span4 number-int", placeHolder: "0", required: true, style: "background-color: rgb(255, 218, 204)" },
                    { name: "AfterDiscDPP4", model: "detail4.AfterDiscDPP", text: "DPP", cls: "span4 number-int", placeHolder: "0", readonly: true },
                    { name: "AfterDiscDPP5", model: "detail5.AfterDiscDPP", text: "DPP", cls: "span4 number-int", placeHolder: "0", readonly: true },
                    { name: "AfterDiscPPn4", model: "detail4.AfterDiscPPn", text: "PPN", cls: "span4 number-int", placeHolder: "0", readonly: true },
                    { name: "AfterDiscPPn5", model: "detail5.AfterDiscPPn", text: "PPN", cls: "span4 number-int", placeHolder: "0", readonly: true },
                    { name: "Remark4", model: "detail4.Remark", text: "Keterangan", cls: "span4" },
                     {
                         type: "controls",
                         cls: "span4",
                         text: "Disc",
                         required: true,
                         items: [
                             { name: "DiscExcludePPn", model: "detail5.DiscExcludePPn", text: "Total", cls: "span2 number-int", placeHolder: "0", disable: true },
                             { type: "label", text: "Q.Unit", cls: "span1", style: "line-height: 30px;" },
                             { name: "QtyUnit", model: "detail5.QtyUnit", cls: "span2 number-int", placeHolder: "0", readonly: true },
                             { type: "label", text: "Q.Part", cls: "span1", style: "line-height: 30px;" },
                             { name: "Qty", model: "detail5.Qty", cls: "span2 number-int", placeHolder: "0" },
                             { name: "DemandQty", model: "detail5.DemandQty", cls: "span2 number-int", placeHolder: "0", type: "hidden" },
                         ]
                     },

                    {
                        type: "buttons",
                        cls: "span4",
                        items: [
                            { name: "btnAddAccesories", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditDetail4()", disable: "!allowInputDetail4" },
                            { name: "btnDeleteOAccesories", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteDetail4()", disable: "isdetail4" },
                            { name: "btnCancelAccesories", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel4()", show: "detail4.oid !== undefined" }
                        ]
                    },
                    {
                        type: "buttons",
                        cls: "span4",
                        items: [
                            { name: "btnAddSP", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditDetail5()", disable: "!allowInputDetail5" },
                            { name: "btnDeleteSP", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteDetail5()", disable: "isdetail5" },
                            { name: "btnCancelSP", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseModel5()", show: "detail5.oid !== undefined" }
                        ]
                    },
                    {
                        name: "wxAccesories",
                        cls: "span4",
                        type: "wxdiv"
                    },
                    { type: "sparator", cls: "span1" },
                    {
                        name: "wxSparepart",
                        cls: "span4",
                        type: "wxdiv"
                    },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("omSalesOrderSFMController");
    }

});
