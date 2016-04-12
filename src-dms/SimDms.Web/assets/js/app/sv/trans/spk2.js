var totalSrvAmt = 0;
var status = 'N';
var svType = '0';
var admin = false;


function svEntrySPKController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    //me.$watch("data.TotalSrvAmt", function (a, b) {
    //    var total = me.data.TotalSrvAmt;
    //    var desc = me.data.amountDesc || "Total Biaya Perawatan";
    //    $("#totalSPK").text(desc + ": " + number_format(total, 0));
    //});

    me.$watch("data.ServiceStatusDesc", function (a, b) {
        $("#StatusSO").text(a != undefined ? a : "NEW");
    });

    me.$watch("detail.ServiceType", function (a, b) {
        console.log(a);
        $('#JobOrderNo').attr('placeHolder', (a == '0') ? 'EST/XX/YYYYY' : ((a == '1') ? 'BOK/XX/YYYYY' : 'SPK/XX/YYYYY'));
    });

    $http.post('sv.api/combo/billtype').
    success(function (data, status, headers, config) {
        me.BillTypeDtSrc = data;
        me.init();
    });

    $('#JobOrderNo').on('blur', function () {
        if ($('#JobOrderNo').val() == "" || me.data.JobOrderNo == null) return;
        $(".ajax-loader").show();
        me.data.ServiceType = me.detail.ServiceType;
        $http.post('sv.api/spk/get', me.data)
         .success(function (rslt, status, headers, config) {
             if (rslt.success) {
                 me.populateData(rslt);
                 me.startEditing();
                 $('#ServiceType').attr('disabled', 'disabled');
                 me.createCookies({
                     ServiceType: rslt.data.ServiceType,
                     JobOrderNo: rslt.data.JobOrderNo
                 });
             }
             else {
                 $(".ajax-loader").hide();
                 me.clearData();
                 me.browse();
             }
         });
    });

    $('#PoliceRegNo').on('blur', function () {
        me.data.JobType = '';
        me.data.JobTypeDesc = '';
        me.Apply();
        if (me.data.PoliceRegNo == "" || me.data.PoliceRegNo == null) return;
        $http.post('sv.api/Spk/GetVeh', me.data).success(function (data) {
            if (data.success) {
                me.populateCustVehicle(data.data);
                me.populateCustBill(data.data);

                $http.post('sv.api/Spk/CheckCampaign', { ChassisCode: data.data.ChassisCode, ChassisNo: data.data.ChassisNo, TrsDate: me.data.JobOrderDate }).success(function (data) {
                    if (data.success) {
                        MsgBox(data.message, MSG_INFO);
                    }
                })
            }
            else {
                me.data.PoliceRegNo = '';
                me.PoliceRegistNo();
            }
        });
    });

    $('#CustomerCodeBill').on('blur', function () {
        if (me.data.CustomerCodeBill == null || me.data.CustomerCodeBill == "") return;
        $http.post('sv.api/Spk/GetCustomerBill', me.data).success(function (data) {
            if (data.success) {
                me.populateCustBill(data.data);
            }
            else {
                me.data.CustomerCodeBill = '';
                me.CustCodeBillLkp();
            }
        });
    });

    $('#JobType').on('blur', function () {
        if (me.data.JobType == "" || me.data.JobType == null) return;
        $http.post('sv.api/Spk/GetJobType', me.data).success(function (data) {
            if (data.success) {
                me.data.JobType = data.data.JobType;
                me.data.JobTypeDesc = data.data.Description;
                me.Apply();
            }
            else {
                me.data.JobType = me.data.JobTypeDesc = '';
                me.JobTypeLkp();
            }
        });
    });

    $('#ForemanID').on('blur', function () {
        if (me.data.ForemanID == "" || me.data.ForemanID == null) return;
        $http.post('sv.api/Spk/GetServiceAdvisor', me.data).success(function (data) {
            if (data.success) {
                me.data.ForemanID = data.data.EmployeeID;
                me.data.ForemanName = data.data.EmployeeName;
            }
            else {
                me.data.ForemanID = me.data.ForemanName = '';
                me.ForemanIDLkp();
            }
        });
    });

    $('#MechanicID').on('blur', function () {
        if (me.data.MechanicID == "" || me.data.MechanicID == null) return;
        $http.post('sv.api/Spk/GetForeman', me.data).success(function (data) {
            if (data.success) {
                me.data.MechanicID = data.data.EmployeeID;
                me.data.MechanicName = data.data.EmployeeName;
            }
            else {
                me.data.MechanicID = me.data.MechanicName = '';
                me.MechanicIDLkp();
            }
        });
    });

    me.batalspk = function () {
        MsgConfirm("SPK akan dibatalkan, apakah anda yakin?", function (result) {
            if (result) {
                var params = {
                    serviceNo: $("[name=ServiceNo]").val()
                }
                $http.post("sv.api/spk/cancelSpk", params)
                            .success(function (result, status, headers, config) {

                                if (result.Message == '') {
                                    me.refreshData();
                                } else {
                                    MsgBox(result.Message, MSG_ERROR);
                                }
                            });
            }
        });
    };

    me.closespk = function () {
        
        if (totalSrvAmt == 0 && me.data.ServiceStatus == '5') {
            MsgConfirm("Total Biaya adalah 0, Apakah akan dilanjutkan?", function (result) {
                if (!result) return; 
            });
        }
        
        var params = {
            serviceNo: $("[name=ServiceNo]").val()
        }

        console.log(me.data.ServiceStatus);

        if (me.data.ServiceStatus != '5') {
            $http.post("sv.api/spk/closeSpk", params)
                          .success(function (result, status, headers, config) {
                              if (result.Message == '') {
                                  me.refreshData();
                              } else {
                                  MsgBox(result.Message, MSG_ERROR);
                                  1
                              }
                          });
        }
        else {
            $http.post("sv.api/spk/OpenSPK", params)
                       .success(function (result, status, headers, config) {
                           if (result.Message == '') {
                               me.refreshData();
                           } else {
                               MsgBox(result.Message, MSG_ERROR);
                           }
                       });
        }
    };

    me.createspk = function () {
        var params = {
            serviceNo: $("[name=ServiceNo]").val()
        }

        $http.post("sv.api/spk/CreateSPK", params)
        .success(function (e) {
            if (e.success) {
                me.data.JobOrderNo = e.joborderno;
                me.data.ServiceType = '2'
                me.detail.ServiceType = 2;
                $('#btnCreate').hide();
                $http.post('sv.api/spk/get', me.data)
                 .success(function (rslt, status, headers, config) {
                     if (rslt.success) {
                         me.populateData(rslt);
                         me.startEditing();
                         $('#ServiceType').attr('disabled', 'disabled');
                         me.createCookies({
                             ServiceType: rslt.data.ServiceType,
                             JobOrderNo: rslt.data.JobOrderNo
                         });
                     }
                 });
            }
        });
    }

    me.gridtaskpart = new webix.ui({
        container: "wxtaskpart",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "SeqNo", header: "No", width: 30 },
            { id: "BillTypeDesc", header: "Ditanggung Oleh", width: 120 },
            { id: "TypeOfGoodsDesc", header: "Jenis Item", width: 100},
            { id: "TaskPartNo", header: "No Part / Pekerjaan", width: 200 },
            { id: "OprHourDemandQty", header: "Qty/NK", format: webix.i18n.numberFormat, width: 90, css: { "text-align": "right" } },
            //{ id: "OprRetailPrice", header: "Price", format: webix.i18n.intFormat, fillspace: true },
            { id: "QtyAvail", header: "Available", format: webix.i18n.numberFormat, width: 90, css: { "text-align": "right" } },
            { id: "SupplyQty", header: "Qty Supply", format: webix.i18n.numberFormat, width: 90, css: { "text-align": "right" } },
            { id: "ReturnQty", header: "Qty Rtn", format: webix.i18n.numberFormat, width: 90, css: { "text-align": "right" } },
            { id: "Price", header: "Harga", format: webix.i18n.intFormat, width: 90, css: { "text-align": "right" } },
            { id: "DiscPct", header: "Diskon", format: webix.i18n.numberFormat, width: 90, css: { "text-align": "right" } },
            { id: "PriceNet", header: "Harga Net", format: webix.i18n.intFormat, width: 90, css: { "text-align": "right" } },
            { id: "SupplySlipNo", header: "No Supply Slip", width: 120 },
            { id: "TaskPartDesc", header: "Keterangan", width: 180 },
            { id: "StatusDesc", header: "Status", width: 120 },
            { id: "TaskPartSeq", header: "Part Seq", width: 180 },

        ],
        on: {
            onSelectChange: function () {
                if (me.gridtaskpart.getSelectedId() !== undefined) {
                    
                    //console.log(this.getItem(me.gridtaskpart.getSelectedId()).BillTypeDesc);
                    me.data.BillType = this.getItem(me.gridtaskpart.getSelectedId()).BillType;
                    me.data.ItemType = this.getItem(me.gridtaskpart.getSelectedId()).ItemType;
                    me.data.TaskPartNo = this.getItem(me.gridtaskpart.getSelectedId()).TaskPartNo;
                    me.data.TaskPartDesc = this.getItem(me.gridtaskpart.getSelectedId()).TaskPartDesc;
                    me.data.TaskPartSeq = this.getItem(me.gridtaskpart.getSelectedId()).TaskPartSeq;
                    me.data.QtyAvail = number_format(this.getItem(me.gridtaskpart.getSelectedId()).QtyAvail, 2);
                    me.data.OprRetailPrice = number_format(this.getItem(me.gridtaskpart.getSelectedId()).OprRetailPrice, 0);
                    me.data.OprHourDemandQty = number_format(this.getItem(me.gridtaskpart.getSelectedId()).OprHourDemandQty, 2);
                    me.data.DiscPct = number_format(this.getItem(me.gridtaskpart.getSelectedId()).DiscPct, 2);
                    me.data.PriceNet = number_format(this.getItem(me.gridtaskpart.getSelectedId()).PriceNet, 0);
                    

                    //me.detil = this.getItem(me.gridtaskpart.getSelectedId().id);
                    me.data.oid = me.gridtaskpart.getSelectedId();
                    //me.detail.NewData = false;
                    //console.log(me.data.Price);
                    $("#btnAddDtl").parent().hide();
                    $(".dtltp").show();
                    
                    $('#ItemType, #btnTaskPartNo, #TaskPartNo').attr('disabled', 'disabled');
                    $('#TaskPartNo').attr('readonly', true);
                    $('#BillType, #btnDlt, #btnUpdNPrice').removeAttr('disabled');
                    if (me.data.ItemType == 'L') {
                        $('#OprRetailPrice, #DiscPct, #btnUpdNPrice').removeAttr('disabled');
                    }
                    else {
                        $('#OprRetailPrice, #btnUpdNPrice').attr('disabled', 'disabled');
                        $('#DiscPct').removeAttr('disabled');
                    }
                    me.validatePackage(me.data.TaskPartNo, me.data.ItemType);

                    $http.post("sv.api/spk/GetPaketSrv?jobType=" + me.data.JobType)
                   .success(function (result, status, headers, config) {
                       if (result.paketSrv != null) {
                           if (result.paketSrv != null) {
                               if (result.paketSrv.ParaValue == "1")
                                   $('#OprHourDemandQty,#DiscPct,#BillType,#btnDlt,#OprRetailPrice,#btnAdd').attr('disabled', 'disabled');
                               if ($("#BillType").val() == "L")
                                   $('#btnDlt').removeAttr('disabled');
                           }
                       }
                   });
                   
                    if (me.IsFSCLock) {
                        $http.post('sv.api/spk/ValidateFSC', { basicModel: me.data.BasicModel, jobType: me.data.JobType, taskPartNo: me.data.TaskPartNo })
                        .success(function (e) {
                            if (e.IsFSCItem) {
                                me.IsFSCItem = true;
                                //$('#btnDlt').attr('disabled', 'disabled');
                            } else {
                                me.IsFSCItem = false;
                                //$('#btnDlt').removeAttr('disabled');
                            }
                        });
                    }

                    setTimeout(function () {                        
                        me.hasChanged = false;
                    }, 50);
                    
                    me.Apply();
                }
            }
        }
    });


    me.alterUI = function (status) {
        console.log(status);
        if (status == 'N') {
            $('#btnProcess').addClass('hide');
            $('#btnCancelSPK').hide();            
            $('#btnClose').hide();
            $('#btnCreate').hide();
            $('#btnOpen').addClass('hide');
            $('#btnCustomerCodeBill, #btnPoliceRegNo, #btnJobType, #CustomerCodeBill').removeAttr('disabled');
            $("#btnAddDtl").parent().hide();
            $(".dtltp").hide();
            //$('#btnMaterialDiscPct, #btnLaborDiscPct, #btnDiscPct, #pLaborDiscPct, #pMaterialDiscPct, #pPartDiscPct').hide();
            $('#ctlDiscP').hide();
            $('#btnInvoiceNo').attr('disabled', 'disabled');
            me.clearDtl();
        } else {
            if (svType == '0' || svType == '1') {
                $('#btnProcess').removeClass('hide');
                $('#btnCancelSPK').hide();
                $('#btnClose').hide();//.addClass('hide');
                $('#btnCreate').show();
                $('#btnOpen').addClass('hide');
                $('#btnPoliceRegNo').attr('disabled', 'disabled');
                $('#btnJobType').attr('disabled', 'disabled');
                $('#CustomerCodeBill').removeAttr('disabled');
                $("#btnAddDtl").parent().show();
                
            } else if (svType == '2') {
                //0,2,3,4 sama
                //6,7,8,9,A,B sama
                if (status == '0') {
                    $('#btnProcess').addClass('hide');
                    $('#btnClose, #btnSave, #btnLkpKendaraan, #btnLkpPelanggan').show();
                    $('#btnClose, #btnCancelSPK').hide();
                    $('#btnOpen').addClass('hide');
                    $("#btnAddDtl").parent().show();
                    
                }
                else if (status == '1') {
                    $('#btnOpen').hide();
                    $('#btnProcess').addClass('hide');
                    $('#btnCancelSPK, #btnClose, #btnSave, #btnLkpKendaraan, #btnLkpPelanggan').show();
                    $('#btnPoliceRegNo').attr('disabled', 'disabled');
                    $('#btnJobType').attr('disabled', 'disabled');
                    $('#CustomerCodeBill').attr('disabled', 'disabled');
                    $("#btnAddDtl").parent().show();
                    
                } else if (status == '5') {
                    $('#btnProcess').addClass('hide');
                    $('#btnCancelSPK, #btnClose, #btnSave, #btnLkpKendaraan, #btnLkpPelanggan').hide();
                    $('#btnOpen').removeClass('hide');
                    $('#btnPoliceRegNo').attr('disabled', 'disabled');
                    $('#btnJobType').attr('disabled', 'disabled');
                    $('#CustomerCodeBill').removeAttr('disabled');
                    $("#btnAddDtl").parent().hide();

                } else if (status == '2' || status == '3' || status == '4') {
                    $('#btnProcess').addClass('hide');
                    $('#btnCancelSPK').show();
                    $('#btnClose').hide();
                    $('#btnOpen').addClass('hide');
                    $('#btnPoliceRegNo').attr('disabled', 'disabled');
                    $('#btnJobType').attr('disabled', 'disabled');
                    $('#CustomerCodeBill').removeAttr('disabled');
                    $("#btnAddDtl").parent().show();
                    
                }
                else {
                    $('#btnProcess').addClass('hide');
                    $('#btnCancelSPK').hide();
                    $('#btnClose').hide();
                    $('#btnOpen').addClass('hide');
                    $('#btnSave').addClass('hide');
                    $('#btnPoliceRegNo').attr('disabled', 'disabled');
                    $('#btnJobType').attr('disabled', 'disabled');
                    $('#CustomerCodeBill').removeAttr('disabled');
                    $("#btnAddDtl").parent().hide();
                    
                }
            }
        }
    }

    me.validatePackage = function (operationNo, itemType) {
        var param = $(".main .gl-widget").serializeObject();
        $http.post("sv.api/spk/validatepackage?taskPartNo=" + operationNo + "&itemType=" + itemType, param)
        .success(function (result, status, headers, config) {
            if (result.data != null) {
                me.data.BillType = "P";
                $('#BillType').attr('disabled', 'disabled');
                me.data.DiscPct = result.data.DiscPct;
            }
        });
    }

    //me.getDateServer = function () {
    //    $http.post('sp.api/EntryReturSupplySlip/getCurrentDate').
    //    success(function (dl, status, headers, config) {
    //        if (dl.success) {
    //            return dl.cDate;
    //        }
    //    }).
    //    error(function (e, status, headers, config) {
    //        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
    //    });
    //}

    me.populateData = function (result) {

        var data = result.data || {};

        me.lookupAfterSelect(data);
        
        me.data.ServiceNo = data.ServiceNo;
        me.data.ContractEndPeriod = data.ContractEndPeriod == null ? '' : moment(data.ContractEndPeriod).format('DD MMM YYYY');
        me.data.ClubEndPeriod = data.ClubEndPeriod == null ? '' : moment(data.ClubEndPeriod).format('DD MMM YYYY');

        //me.detail.ServiceType = data.ServiceType;

        if (me.detail.ServiceType == '0') {
            me.data.JobOrderNo = data.EstimationNo;
            me.data.JobOrderDate = data.EstimationDate == null ? moment(me.data.JobOrderDate).format('DD MMM YYYY HH:mm:ss') : moment(data.EstimationDate).format('DD MMM YYYY HH:mm:ss');
        }
        else if (me.detail.ServiceType == '1') {
            me.data.JobOrderNo = data.BookingNo;
            me.data.JobOrderDate = data.BookingDate == null ? moment(me.data.JobOrderDate).format('DD MMM YYYY HH:mm:ss') : moment(data.BookingDate).format('DD MMM YYYY HH:mm:ss');
        }
        else if (me.detail.ServiceType == '2') {
            me.data.JobOrderNo = data.JobOrderNo;
            //me.data.JobOrderDate = data.JobOrderDate == null ? moment().format('DD MMM YYYY HH:mm:ss') : moment(data.JobOrderDate).format('DD MMM YYYY HH:mm:ss');
            me.data.JobOrderDate = data.JobOrderDate == null ? moment(me.data.JobOrderDate).format('DD MMM YYYY HH:mm:ss') : moment(data.JobOrderDate).format('DD MMM YYYY HH:mm:ss');
        }

        me.data.InvoiceNo = data.InvoiceNo;
        me.data.ServiceStatusDesc = (me.data.ServiceType == '0' ? 'ESTIMASI' : me.data.ServiceType == '1' ? 'BOOKING' : data.ServiceStatusDesc);
        //me.data.ColorCode = data.ColorCode == "" ? data.ColorCode : data.ColorCodeDesc;
        me.data.EstimateFinishDate = data.EstimateFinishDate == null ? moment(me.now()).format('DD MMM YYYY HH:mm:ss') : moment(data.EstimateFinishDate).format('DD MMM YYYY HH:mm:ss');
        
        //me.data.CustomerCode = data.CustomerCode;
        //me.data.CustomerName = data.CustomerName;
        //me.data.CustAddr1 = data.CustAddr1;
        //me.data.CustAddr2 = data.CustAddr2;
        //me.data.CustAddr3 = data.CustAddr3;
        //me.data.PoliceRegNo = data.PoliceRegNo;
        //me.data.CityCode = data.CityCode;
        //me.data.CityName = data.CityName;
        //me.data.PhoneNo = data.PhoneNo;
        //me.data.FaxNo = data.FaxNo;
        //me.data.HPNo = data.HPNo;
        //me.data.ServiceBookNo = data.ServiceBookNo;
        //me.data.BasicModel = data.BasicModel;
        //me.data.TransmissionType = data.TransmissionType;
        //me.data.ChassisCode = data.ChassisCode;
        //me.data.ChassisNo = data.ChassisNo;
        //me.data.EngineCode = data.EngineCode;
        //me.data.EngineNo = data.EngineNo;
        // me.data.ForemanID = data.ForemanID;
        //me.data.ForemanName = data.ForemanName;
        //me.data.MechanicID = data.MechanicID;
        //me.data.MechanicName = data.MechanicName;
        //me.data.Odometer = number_format(data.Odometer, 0);
        //me.data.ContractNo = data.ContractNo;
        //me.data.ContractExpired = ((data.ContractEndPeriod || "").length > 0) ? moment(data.ContractEndPeriod).format(SimDms.dateFormat) : "";
        //me.data.ContractStatus = data.ContractStatusDesc;
        //me.data.ClubNo = data.ClubCode;
        //me.data.ClubExpired = ((data.ClubEndPeriod || "").length > 0) ? moment(data.ClubEndPeriod).format(SimDms.dateFormat) : "";
        //me.data.ClubStatus = data.ClubStatusDesc;
        //me.data.InsurancePayFlag = data.InsurancePayFlag;
        //me.data.InsuranceOwnRisk = data.InsuranceOwnRisk;
        //me.data.InsuranceNo = data.InsuranceNo;
        //me.data.InsuranceJobOrderNo = data.InsuranceJobOrderNo;
        //me.data.CustomerCodeBill = data.CustomerCodeBill;
        //me.data.CustomerNameBill = data.CustomerNameBill;
        //me.data.CustAddr1Bill = data.CustAddr1Bill;
        //me.data.CustAddr2Bill = data.CustAddr2Bill;
        //me.data.CustAddr3Bill = data.CustAddr3Bill;
        //me.data.CityCodeBill = data.CityCodeBill;
        //me.data.CityNameBill = data.CityNameBill;
        //me.data.LaborDiscPct = data.LaborDiscPct;
        //me.data.PartDiscPct = data.PartDiscPct;
        //me.data.MaterialDiscPct = data.MaterialDiscPct;
        //me.data.IsPPN = ((data.TaxCode || "PPN") === "PPN" ? true : false);
        //me.data.ServiceRequestDesc = data.ServiceRequestDesc;
        //me.data.JobType = data.JobType;
        //me.data.JobTypeDesc = data.JobTypeDesc;
        //me.data.ConfirmChangingPart = data.ConfirmChangingPart;
      
        //me.data.EstimateFinishDate = data.EstimateFinishDate;
        //me.data.ClaimType = (data.IsSparepartClaim || true) ? "P" : "S";
        me.data.LaborDppAmt = number_format(data.SrvLaborDppAmt, 0);
        me.data.PartsDppAmt = number_format(data.SrvPartsDppAmt, 0);
        me.data.MaterialDppAmt = number_format(data.SrvMaterialDppAmt, 0);
        me.data.TotalDppAmt = number_format(data.SrvTotalDppAmt, 0);
        me.data.TotalPpnAmt = number_format(data.SrvTotalPpnAmt, 0);
        me.data.SrvTotalSrvAmt = number_format(data.TotalSrvAmt, 0);
        //me.data.IsSparepartClaim = data.IsSparepartClaim
        //totalSrvAmt = data.TotalSrvAmt;
        //me.data.ServiceStatus= data.ServiceStatus;        
        //me.alterUI(data.ServiceStatus);

        $("[name=ServiceNo]").val(data.ServiceNo);

        if (data.IsSparepartClaim == true) {
            $('#btnInvoiceNo').removeAttr('disabled');
            $http.post("sv.api/spk/IsEnableCloseSPK", data)
            .success(function (result1, status, headers, config) {           
                $('#tblInvClaim').show();                     
            });
        }
        else {
            $('#btnInvoiceNo').attr('disabled', 'disabled');
        }
       
        var value = data.IsSparepartClaim;
        $("#IsSparepartClaimY").prop('checked', value).val(value);
        $("#IsSparepartClaimN").prop('checked', !value).val(value);

        var value = data.ConfirmChangingPart;
        $("#ConfirmChangingPartY").prop('checked', value).val(value);
        $("#ConfirmChangingPartN").prop('checked', !value).val(value);

        var value = data.InsurancePayFlag;
        $("#InsurancePayFlagY").prop('checked', value).val(value);
        $("#InsurancePayFlagN").prop('checked', !value).val(value);

        me.grid.data = result.list;

        me.loadTableData(me.gridtaskpart, me.grid.data);
        me.gridtaskpart.adjust();
        if (result.detailList > 0) {
            $('#JobType, #TaskPartNo').attr('readonly', true);
            $('#btnJobType, #btnTaskPartNo').attr('disabled', 'disabled');
        }
        else{
            $('#JobType, #TaskPartNo').removeAttr('readonly');
            $('#btnJobType, #btnTaskPartNo').removeAttr('disabled');
        }

        $(".ajax-loader").hide();
        me.data.JobOrderNo = data.JobOrderNo;

        $('input[name="JobOrderDate"]').attr('disabled', 'disabled');
        $('input[name="EstimationDate"]').attr('disabled', 'disabled');
        $('input[name="BookingDate"]').attr('disabled', 'disabled');

        $("#pnlTaskPart, #pnlInvClaim").slideUp();
        $("#tblInvClaim td .icon").addClass("link");
        $("#btnAddDtl").parent().show();

        if ($("#InsurancePayFlagY").val() == "true") {
            $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").removeAttr('disabled');
        }
        else {
            $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").attr('disabled', 'disabled');
        }

        $http.post("sv.api/spk/IsEnableCloseSPK", data)
       .success(function (result, status, headers, config) {
           if (me.data.ServiceStatus == '5') {               
               $('#btnClose').html($('#btnClose').html().replace('Tutup SPK', 'Buka SPK'))
           }
           else {
               $('#btnClose').html($('#btnClose').html().replace('Buka SPK', 'Tutup SPK'));
           }
           if (result.enabled == true) {               
               $('#btnClose').show();               
           }
           else {
               $('#btnClose').hide();
           }
       });
        $('#PoliceRegNo, #JobOrderNo').attr('readonly', true);
        $('#btnPoliceRegNo, #btnJobOrderNo').attr('disabled', 'disabled');

        if (result.detailList === 0) {
            $('#PoliceRegNo').attr('readonly', false);
            $('#btnPoliceRegNo').removeAttr('disabled');
        }

        if (me.data.ServiceType == '0' || me.data.ServiceType == '1') {
            $('#btnCreate').show();
            $('#btnCancelSPK').hide();
        }

        $(".ajax-loader").show();
        $http.post("sv.api/spk/ListDiscountService", me.data)
        .success(function (rslt, status, headers, config) {
            $(".ajax-loader").hide();
            if (rslt.success) {
                if (rslt.count > 1) {
                    $('#ctlDisc').show();
                    $('#ctlDiscP').hide();
                }
                $('#LaborDiscPct, #pLaborDiscPct').val(number_format(rslt.data.LaborDiscPct, 2));
                $('#PartDiscPct, #pPartDiscPct').val(number_format(rslt.data.PartDiscPct, 2));
                $('#MaterialDiscPct, #pMaterialDiscPct').val(number_format(rslt.data.MaterialDiscPct, 2));
            }
        });

        var total = me.data.TotalSrvAmt;
        var desc = me.data.amountDesc || "Total Biaya Perawatan";
        $("#totalSPK").text(desc + ": " + number_format(total, 0));

        // IsFSCLock
        me.IsFSCLock = (me.IsFSCLock && me.data.GroupJobType == 'FSC');
    }

    me.populateCustBill = function (data) {
        me.data.CustomerCodeBill = data.CustomerCode;
        me.data.CustomerNameBill = data.CustomerName;
        me.data.CustAddr1Bill = data.Address1;
        me.data.CustAddr2Bill = data.Address2;
        me.data.CustAddr3Bill = data.Address3;
        me.data.CityCodeBill = data.CityCode;
        me.data.CityNameBill = data.CityName;
        me.data.CityCodeBill = data.CityCode;
        me.data.CityNameBill = data.CityName;
        me.data.PhoneNo = data.PhoneNo;
        me.data.HPNo = data.HPNo;
        me.data.FaxNo = data.FaxNo;

        $(".ajax-loader").show();
        $http.post("sv.api/spk/ListDiscountService", me.data)
        .success(function (rslt, status, headers, config) {
            $(".ajax-loader").hide();
            if (rslt.success) {
                if (rslt.count > 1) {
                    $('#ctlDisc').show();
                    $('#ctlDiscP').hide();
                }
                $('#LaborDiscPct, #pLaborDiscPct').val(number_format(rslt.data.LaborDiscPct, 2));
                $('#PartDiscPct, #pPartDiscPct').val(number_format(rslt.data.PartDiscPct, 2));
                $('#MaterialDiscPct, #pMaterialDiscPct').val(number_format(rslt.data.MaterialDiscPct, 2));
            }
        });
    }

    me.populateCustVehicle = function (data) {
        me.data.PoliceRegNo = data.PoliceRegNo;
        me.data.ServiceBookNo = data.ServiceBookNo;
        me.data.BasicModel = data.BasicModel;
        me.data.TransmissionType = data.TransmissionType;
        me.data.ChassisCode = data.ChassisCode;
        me.data.ChassisNo = data.ChassisNo;
        me.data.EngineCode = data.EngineCode;
        me.data.EngineNo = data.EngineNo;
        me.data.ColorCode = data.ColorCode;
        me.data.CustomerCode = data.CustomerCode;
        me.data.CustomerName = data.CustomerName;
        me.data.CustAddr1 = data.Address1;
        me.data.CustAddr2 = data.Address2;
        me.data.CustAddr3 = data.Address3;
        me.data.CityCode = data.CityCode;
        me.data.CityName = data.CityName;
        me.data.IsPPN = true;
    }

    me.populate = function (data) {
        me.data.pLaborDiscPct = number_format(data.LaborDiscPct, 2);
        me.data.pPartDiscPct = number_format(data.PartDiscPct, 2);
        me.data.pMaterialDiscPct = number_format(data.MaterialDiscPct, 2);
        me.data.LaborDiscPct = number_format(data.LaborDiscPct, 2);
        me.data.PartDiscPct = number_format(data.PartDiscPct, 2);
        me.data.MaterialDiscPct = number_format(data.MaterialDiscPct, 2);

    }

    me.populatetable = function (data) {
        // console.log('populate tableeee');

    }

    me.lkuTaskPart = function () {
        //console.log('ssddsfsda');
        var data = {
            CompanyCode: me.data.CompanyCode,
            BranchCode: me.data.BranchCode,
            BasicModel: me.data.BasicModel,
            JobType: me.data.JobType,
            ChassisCode: me.data.ChassisCode,
            ChassisNo: me.data.ChassisNo,
            TransType: me.data.TransmissionType,
            ItemType: me.data.ItemType,
            BillType: me.data.BillType
        }


        if (data.BasicModel.length == 0) return;
        //console.log(me.data.ItemType);
        switch (me.data.ItemType) {
            case "L":
                //console.log('L');
                var lookup = Wx.klookup({
                    name: "TaskPart",
                    title: "Task List",
                    url: "sv.api/grid/PartNo?basicModel=" + data.BasicModel + "&jobType=" + data.JobType,
                    serverBinding: true,
                    pageSize: 10,
                    columns: [
                        { field: "OperationNo", title: "Pekerjaan" },
                        { field: "DescriptionTask", title: "Keterangan" },
                        { field: "Qty", title: "NK", template: '<div style="text-align:right;">#= kendo.toString(Qty, "n2") #</div>' },
                        { field: "Price", title: "Nilai Jasa", template: '<div style="text-align:right;">#= kendo.toString(Price, "n0") #</div>' },
                    ]
                });

                //var lookup = Wx.blookup({
                //    name: "TaskPart",
                //    title: "Task List",
                //    manager: svServiceManager,
                //    query: new breeze.EntityQuery.from("PartNo").withParameters({ basicModel: data.BasicModel, jobType: data.JobType }),
                //    //source: "sv.api/grid/partno?basicmodel=" + data.BasicModel + '&jobtype=' + data.JobType,
                //    //data: data,
                //    //sortings: [[0, "asc"]],
                //    columns: [
                //        { field: "OperationNo", title: "Pekerjaan" },
                //        { field: "DescriptionTask", title: "Keterangan" },
                //        { field: "Qty", title: "NK", template: '<div style="text-align:right;">#= kendo.toString(Qty, "n2") #</div>' },
                //        { field: "Price", title: "Nilai Jasa", template: '<div style="text-align:right;">#= kendo.toString(Price, "n0") #</div>' },
                //        //{ mData: "IsActive", sTitle: "Status", sWidth: 120 },
                //    ],
                //});
                //widget.lookup.show();
                break;
            case "0":
                //console.log('0');
                var lookup = Wx.klookup({
                    name: "TaskPart",
                    title: "Task List",
                    url: "sv.api/grid/NoPartOpen",
                    serverBinding: true,
                    pageSize: 10,
                    columns: [
                        { field: "PartNo", title: "No Part", sWidth: 140 },
                        { field: "PartName", title: "Keterangan", sWidth: 200 },
                        { field: "GroupTypeOfGoods", title: "Group", sWidth: 80 },
                        { field: "Available", title: "Available", sWidth: 100, template: '<div style="text-align:right;">#= kendo.toString(Available, "n2") #</div>' },
                        { field: "Price", title: "Harga Jual", sWidth: 100, template: '<div style="text-align:right;">#= kendo.toString(Price, "n0") #</div>' },
                        { field: "Status", title: "Status", sWidth: 100 },
                    ]
                });

                //var lookup = Wx.blookup({
                //    name: "TaskPart",
                //    title: "Task List",
                //    manager: svServiceManager,
                //    query: "NoPartOpen",
                //    //source: "sv.api/grid/NoPartOpen",
                //    //data: data,
                //    //sortings: [[1, "asc"]],
                //    columns: [
                //        { field: "PartNo", title: "No Part", sWidth: 140 },
                //        { field: "PartName", title: "Keterangan", sWidth: 200 },
                //        { field: "GroupTypeOfGoods", title: "Group", sWidth: 80 },
                //        { field: "Available", title: "Available", sWidth: 100, template: '<div style="text-align:right;">#= kendo.toString(Available, "n2") #</div>' },
                //        { field: "Price", title: "Harga Jual", sWidth: 100, template: '<div style="text-align:right;">#= kendo.toString(Price, "n0") #</div>' },
                //        { field: "Status", title: "Status", sWidth: 100 },
                //    ],
                //});
                //widget.lookup.show();
                break;
            default:
                break;
        }

        lookup.dblClick(function (data) {
            if (me.data.ItemType == "L") {
                me.data.TaskPartNo = data.OperationNo;
                me.data.TaskPartDesc = data.DescriptionTask;
                me.data.OprHourDemandQty = number_format(data.Qty, 2);
                me.data.OprRetailPrice = number_format(data.Price, 0);
                me.data.DiscPct = number_format($('#LaborDiscPct').val(), 2);
                me.data.PriceNet = number_format(data.Qty * data.Price * (100 - $('#LaborDiscPct').val()) * 0.01);
                //console.log('validate');
                me.validatePackage(data.OperationNo, "L");
                $('#OprRetailPrice, #DiscPct, #btnUpdNPrice').removeAttr('disabled');
                me.Apply();
            }
            else {
                me.data.TaskPartNo = data.PartNo;
                me.data.TaskPartDesc = data.PartName;
                me.data.QtyAvail = number_format(data.Available, 2);
                me.data.OprRetailPrice = number_format(data.Price, 0);
                me.data.PriceNet = number_format(data.Qty * data.Price * (100 - $('#LaborDiscPct').val()) * 0.01, 0);
                me.data.OprHourDemandQty = number_format(0, 2);



                me.data.TaskPartSeq = data.TaskPartSeq;

                if (data.GroupTypeOfGoods == "SPAREPART") {
                    $("#DiscPct").val($("#PartDiscPct").val());
                    me.data.DiscPct = number_format(me.data.PartDiscPct, 2);
                }
                else {
                    $("#DiscPct").val($("#MaterialDiscPct").val());
                    me.data.DiscPct = number_format(me.data.MaterialDiscPct, 2);
                }
                $('#OprRetailPrice, #btnUpdNPrice').attr('disabled', 'disabled');
                $('#DiscPct').removeAttr('disabled');
                me.Apply();
            }
        });
    }

    me.browse = function () {
        svType = me.detail.ServiceType;
        //console.log(svType);
        if (svType == '0') {
            var lookup = Wx.klookup({
                name: "JobOrderList",
                title: "Estimation List",
                url: "sv.api/spk/LookUpServiceEstimation",
                serverBinding: true,
                pageSize: 10,
                sort: [
                    { 'field': 'EstimationNo', 'dir': 'desc' },
                    { 'field': 'EstimationDate', 'dir': 'desc' },
                ],
                filters: [
                {
                    text: "Semua Status",
                    type: "controls",
                    cls: "span8",
                    items: [
                            {
                                name: "ShowAll", type: "select", text: "", cls: "span2", items: [
                                    { value: "0", text: "Ya" },
                                    { value: "1", text: "Tidak", selected: 'selected' }
                                ]
                            }
                    ]
                },
                { name: "EstimationNo", text: "No Estimasi", cls: "span4" },
                { name: "PoliceRegNo", text: "No. Polisi", cls: "span4" },
                { name: "Customer", text: "Nama pelanggan", cls: "span4" },
                { name: "ServiceBookNo", text: "No Buku Service", cls: "span4" }
                ],
                columns: [
                    { field: "EstimationNo", title: "No. Estimasi", width: 150 },
                    {
                        field: "EstimationDate", title: "Tgl. Estimasi", width: 130,
                        template: "#= (EstimationDate == undefined) ? '' : moment(EstimationDate).format('DD MMM YYYY') #"
                    },
                    { field: "JobOrderNo", title: "No SPK", width: 160 },
                    {
                        field: "JobOrderDate", title: "Tgl SPK", width: 130,
                        template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                    },
                    { field: "PoliceRegNo", title: "No Polisi", width: 160 },
                    { field: "ServiceBookNo", title: "No Buku Service", width: 160 },
                    { field: "BasicModel", title: "Model", width: 130 },
                    { field: "TransmissionType", title: "Tipe Transmisi", width: 130 },
                    { field: "KodeRangka", title: "Kode Rangka", width: 200 },
                    { field: "KodeMesin", title: "Kode Mesin", width: 160 },
                    { field: "ColorCode", title: "Warna", width: 100 },
                    { field: "Customer", title: "Pelanggan", width: 300 },
                    { field: "CustomerBill", title: "Pembayar", width: 300 },
                     { field: "PhoneNo", title: "No. Telp", width: 150 },
                    { field: "HPNo", title: "No. HP", width: 150 },
                    { field: "Odometer", title: "Odometer(KM)", width: 160, template: '<div style="text-align:right;">#= kendo.toString(Odometer, "n0") #</div>' },
                    { field: "JobType", title: "Jenis Pekerjaan", width: 160 },
                    { field: "ForemanID", title: "Foreman", width: 150 },
                    { field: "MechanicID", title: "Mekanik", width: 150 },

                    { field: "ServiceStatus", title: "Status", width: 160 },
                ]
            });
            lookup.dblClick(function (data) {
                if (data != null) {
                    $(".ajax-loader").show();
                    $http.post('sv.api/spk/get', data)
                     .success(function (rslt, status, headers, config) {
                         if (rslt.success) {
                             me.populateData(rslt);
                             me.startEditing();
                             $('#ServiceType').attr('disabled', 'disabled');
                             me.createCookies({
                                 ServiceType: rslt.data.ServiceType,
                                 JobOrderNo: rslt.data.JobOrderNo
                             });
                             $(".ajax-loader").hide();
                         }
                         else {
                             $(".ajax-loader").hide();
                         }
                     });
                }
            });
        }
        else if (svType == '1') {
            var lookup = Wx.klookup({
                name: "JobOrderList",
                title: "Booking List",
                url: "sv.api/spk/LookUpServiceBooking",
                serverBinding: true,
                pageSize: 10,
                sort: [
                    { 'field': 'BookingNo', 'dir': 'desc' },
                    { 'field': 'BookingDate', 'dir': 'desc' },
                ],
                filters: [
                {
                    text: "Semua Status",
                    type: "controls",
                    cls: "span8",
                    items: [
                            {
                                name: "ShowAll", type: "select", text: "", cls: "span2", items: [
                                    { value: "0", text: "Ya" },
                                    { value: "1", text: "Tidak", selected: 'selected' }
                                ]
                            }
                    ]
                },
                { name: "BookingNo", text: "No Booking", cls: "span4" },
                { name: "PoliceRegNo", text: "No. Polisi", cls: "span4" },
                { name: "Customer", text: "Nama pelanggan", cls: "span4" },
                { name: "ServiceBookNo", text: "No Buku Service", cls: "span4" }
            ],
                columns: [
                    { field: "BookingNo", title: "No Booking", width: 150 },
                    {
                        field: "BookingDate", title: "Tgl Booking", width: 150,
                        template: "#= (BookingDate == undefined) ? '' : moment(BookingDate).format('DD MMM YYYY') #"
                    },
                    { field: "Customer", title: "Pelanggan", width: 300 },
                    { field: "PoliceRegNo", title: "No Polisi", width: 160 },
                    { field: "ServiceBookNo", title: "No Buku Service", width: 160 },
                     { field: "PhoneNo", title: "No. Telp", width: 150 },
                    { field: "HPNo", title: "No. HP", width: 150 },
                    { field: "ForemanName", title: "Foreman", width: 160 },
                    { field: "BasicModel", title: "Model", width: 160 },
                    { field: "Odometer", title: "Odometer(KM)", width: 160, template: '<div style="text-align:right;">#= kendo.toString(Odometer, "n0") #</div>' },
                    { field: "JobType", title: "Jenis Pekerjaan", width: 160 },
                    { field: "ServiceStatusDesc", title: "Status", sWidth: "140px" },
                ]
            });

            lookup.dblClick(function (data) {
                if (data != null) {
                    $(".ajax-loader").show();
                    $http.post('sv.api/spk/get', data)
                     .success(function (rslt, status, headers, config) {
                         if (rslt.success) {
                             me.populateData(rslt);
                             me.startEditing();
                             $('#ServiceType').attr('disabled', 'disabled');
                             me.createCookies({
                                 ServiceType: rslt.data.ServiceType,
                                 JobOrderNo: rslt.data.JobOrderNo
                             });
                         }
                         else {
                             $(".ajax-loader").hide();
                         }
                     });
                }
            });

        }
        else if (svType == '2') {

            var lookup = Wx.klookup({
                name: "lookupJobOrder",
                title: "Job Order List",
                url: "sv.api/spk/LookUpServiceJobOrder",
                serverBinding: true,
                pageSize: 10,
                sort: [
                    { 'field': 'JobOrderNo', 'dir': 'desc' },
                    { 'field': 'JobOrderDate', 'dir': 'desc' }
                ],
                filters: [
                {
                    text: "Semua Status",
                    type: "controls",
                    cls: "span8",
                    items: [
                            {
                                name: "ShowAll", type: "select", text: "", cls: "span2", items: [
                                    { value: "0", text: "Ya" },
                                    { value: "1", text: "Tidak", selected: 'selected' }
                                ]
                            }
                    ]
                },
                { name: "JobOrderNo", text: "No SPK", cls: "span4" },
                { name: "PoliceRegNo", text: "No. Polisi", cls: "span4" },
                { name: "CustomerName", text: "Nama pelanggan", cls: "span4" },
                { name: "ServiceBookNo", text: "No Buku Service", cls: "span4" }
                ],
                columns: [
                    { field: "JobOrderNo", title: "No SPK", width: 160 },
                    {
                        field: "JobOrderDate", title: "Tgl SPK", width: 130,
                        template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                    },
                    { field: "CustomerName", title: "Pelanggan", width: 300 },
                    { field: "PoliceRegNo", title: "No Polisi", width: 150 },
                    { field: "ServiceBookNo", title: "No Buku Service", width: 160 },
                     { field: "PhoneNo", title: "No. Telp", width: 150 },
                    { field: "HPNo", title: "No. HP", width: 150 },
                    { field: "ForemanName", title: "SA", width: 200 },
                    { field: "MechanicName", title: "FM", width: 200 },
                    { field: "BasicModel", title: "Model" },
                    { field: "Odometer", title: "Odometer(KM)", width: 160, template: '<div style="text-align:right;">#= kendo.toString(Odometer, "n0") #</div>' },
                    { field: "JobType", title: "Jenis Pekerjaan", width: 160 },
                    { field: "ServiceStatusDesc", title: "Status",width: 140 },
                ]
            });

            lookup.dblClick(function (data) {
                if (data != null) {
                    $(".ajax-loader").show();
                    $http.post('sv.api/spk/get', data)
                     .success(function (rslt, status, headers, config) {
                         if (rslt.success) {
                             me.populateData(rslt);
                             me.startEditing();
                             $('#ServiceType').attr('disabled', 'disabled');
                             me.createCookies({
                                 ServiceType: rslt.data.ServiceType,
                                 JobOrderNo: rslt.data.JobOrderNo
                             });
                         }
                         else {
                             $(".ajax-loader").hide();
                         }
                     });
                }
            });

        }

    };

    me.save = function () {
        if (me.data.PoliceRegNo == "" || me.data.PoliceRegNo == null) {
            MsgBox('No Polisi Harus Diisi', MSG_INFO);
            return;
        }
        if (me.data.CustomerCodeBill == "" || me.data.CustomerCodeBill == null) {
            MsgBox('Kode Customer Harus Diisi', MSG_INFO);
            return;
        }
        if (me.data.JobType == "" || me.data.JobType == null) {
            MsgBox('Job Type Harus Diisi', MSG_INFO);
            return;
        }
        else {
            if (me.data.JobType == "REWORK") {
                if (me.data.ServiceRequestDesc == "") {
                    MsgBox('Job Request Harus Diisi', MSG_INFO);
                    return;
                }
            }
            if (me.data.JobType != "PDI" && me.data.JobType.indexOf("FSC") == -1 && parseInt(me.data.Odometer, 10) <= 0) {
                MsgBox('Odometer harus lebih dari 0', MSG_INFO);

                return;
            }
        }
        if (me.data.ForemanID == "" || me.data.ForemanID == null) {
            MsgBox('SA Harus Diisi', MSG_INFO);
            return;
        }
        if (me.data.MechanicID == "" || me.data.MechanicID == null) {
            MsgBox('Foreman Harus Diisi', MSG_INFO);
            return;
        }
        if (me.data.InsurancePayFlagY == "true") {
            if (me.data.InsuranceNo == "") {
                MsgBox('No Insurance Harus Diisi', MSG_INFO);
                return;
            }
            if (me.data.InsuranceJobOrderNo == "") {
                MsgBox('No Polis Harus Diisi', MSG_INFO);
                return;
            }
            if (me, data, InsuranceOwnRisk == "") {
                MsgBox('Jumlah Own Risk Harus Diisi', MSG_INFO);
                return;
            }
        }

        var overrideDisc = false;
        var param = $(".main .gl-widget").serializeObject();

        console.log(param);


        $http.post('sv.api/spk/CalculateTotal', param)
       .success(function (result, status, headers, config) {
           if (result.success) {

               $http.post('sv.api/spk/ValidateInsertSPK', param)
               .success(function (result0, status, headers, config) {
                   if (result0.success) {

                       $http.post('sv.api/spk/EditDiscPart', param)
                       .success(function (result1, status, headers, config) {
                           if (result1.data != '') {
                               MsgConfirm(result1.data, function (result) {
                                   if (result) {
                                       overrideDisc = true;
                                   }
                               }); 
                           }
                           $http.post("sv.api/spk/save?bOverrideDisc=" + overrideDisc, param)
                          .success(function (result2, status, headers, config) {
                              if (result2.success) {
                                  //me.data.JobOrderNo = result2.data.JobOrderNo
                                  console.log('11111');
                                  console.log(me.detail.ServiceType);

                                  if (me.detail.ServiceType == '0') {
                                      me.data.JobOrderNo = result2.data.EstimationNo;
                                      console.log(me.data.JobOrderNo);
                                  }
                                  else if (me.detail.ServiceType == '1') {
                                      me.data.JobOrderNo = result2.data.BookingNo;
                                  } else {
                                      me.data.JobOrderNo = result2.data.JobOrderNo;
                                  }                       

                                  
                                  //$("#ServiceNo").val(result2.data.ServiceNo);
                                  //me.createCookies({
                                  //    ServiceType: result2.data.ServiceType,
                                  //    JobOrderNo: result2.data.JobOrderNo
                                  //});
                                  //me.populateData(result2.data);
                                  //me.refreshData();
                                  //console.log("Saved test!");
                                  //toastr.success('data saved...');
                                  //$('#ServiceType').attr('disabled', 'disabled');

                                  toastr.success('data saved...');
                                  $('#ServiceType').attr('disabled', 'disabled');
                                  console.log(result2.data);

                                  me.createCookies({
                                      ServiceType: result2.data.ServiceType,
                                      JobOrderNo: result2.data.JobOrderNo
                                  });
                                  me.populateData({ data: result2.data });
                                  //$("#JobOrderNo").val(result2.data.JobOrderNo);
                                  $("[name=ServiceNo]").val(result2.data.ServiceNo),
                                  console.log($("#ServiceNo").val());
                                  me.refreshData();

                              }
                              else {
                                  MsgBox(result2.message, MSG_WARNING);
                              }
                          });
                       });
                   }
                   else {
                       if (result0.confirm) {
                           MsgConfirm(result0.data, function (result) {
                               if (result) {
                                   $http.post("sv.api/spk/EditDiscPart", param)
                                .success(function (result1, status, headers, config) {
                                    if (result1.data != '') {
                                        MsgConfirm(result1.data, function (result) {
                                            if (result) overrideDisc = true;
                                            $http.post("sv.api/spk/save?bOverrideDisc=" + overrideDisc, param)
                                           .success(function (result2, status, headers, config) {
                                               if (result2.success) {
                                                   //console.log(result2.data);     
                                                   console.log('2222');
                                                   if (me.detail.ServiceType == '0') {
                                                       me.data.JobOrderNo = result2.data.EstimationNo;
                                                   }
                                                   else if (me.detail.ServiceType == '1') {
                                                       me.data.JobOrderNo = result2.data.BookingNo;
                                                   } else {
                                                       me.data.JobOrderNo = result2.data.JobOrderNo;
                                                   }
                                                   toastr.success('data saved...');
                                                   $('#ServiceType').attr('disabled', 'disabled');
                                                   console.log(result2.data);

                                                   me.createCookies({
                                                       ServiceType: result2.data.ServiceType,
                                                       JobOrderNo: result2.data.JobOrderNo
                                                   });
                                                   me.populateData({ data: result2.data });
                                                   //$("#JobOrderNo").val(result2.data.JobOrderNo);
                                                   $("[name=ServiceNo]").val(result2.data.ServiceNo),
                                                   console.log($("#ServiceNo").val());
                                                   me.refreshData();

                                               } else {
                                                   MsgBox(result2.message, MSG_WARNING);
                                               }
                                            });
                                        });
                                    } else {
                                        $http.post("sv.api/spk/save?bOverrideDisc=" + overrideDisc, param)
                                           .success(function (result2, status, headers, config) {
                                               if (result2.success) {
                                                   //console.log(result2.data);         
                                                   
                                                   toastr.success('data saved...');
                                                   $('#ServiceType').attr('disabled', 'disabled');
                                                   console.log(result2.data);
                                                   
                                                   me.populateData({ data: result2.data });

                                                   if (me.detail.ServiceType == '0') {
                                                       me.data.JobOrderNo = result2.data.EstimationNo;
                                                   }
                                                   else if (me.detail.ServiceType == '1') {
                                                       me.data.JobOrderNo = result2.data.BookingNo;
                                                   } else {
                                                       me.data.JobOrderNo = result2.data.JobOrderNo;
                                                   }

                                                   me.createCookies({
                                                       ServiceType: result2.data.ServiceType,
                                                       JobOrderNo: me.data.JobOrderNo
                                                   });
                                                  

                                                   //$("#JobOrderNo").val(result2.data.JobOrderNo);
                                                   $("[name=ServiceNo]").val(result2.data.ServiceNo),
                                                   console.log($("#ServiceNo").val());
                                                   me.refreshData();

                                               } else {
                                                   MsgBox(result2.message, MSG_WARNING);
                                               }
                                           });
                                    }
                                   
                                });
                               }
                           });

                       }
                       else {                           
                           MsgBox(result0.data, MSG_ERROR);
                       }
                   }
               });
           }
           else {
               MsgConfirm(result.data, function (result) {
                   if (result) {
                       $http.post("sv.api/spk/ValidateInsertSPK", param)
                        .success(function (result0, status, headers, config) {
                            if (result0.success) {
                                $http.post('sv.api/spk/EditDiscPart', param)
                                     .success(function (result1, status, headers, config) {
                                         if (result1.data != '') {
                                             MsgConfirm(result1.data, function (result) {
                                                 if (result) overrideDisc = true;
                                             });
                                         }
                                         $http.post('sv.api/spk/save?bOverrideDisc=' + overrideDisc, param)
                                           .success(function (result2, status, headers, config) {
                                               if (result2.success) {
                                                   console.log('4444');
                                                   me.populate(result2.data);
                                                   toastr.success('data saved...');
                                                   $('#ServiceType').attr('disabled', 'disabled');
                                                   me.refreshData();
                                               }
                                           });
                                     });
                            }
                            else {
                                if (result0.confirm) {
                                    MsgConfirm(result0.data, function (result) {
                                        if (result) {
                                            $http.post('sv.api/spk/EditDiscPart', param)
                                         .success(function (result1, status, headers, config) {
                                             if (result1.data != '') {
                                                 MsgConfirm(result1.data, function (result) {
                                                     if (result) overrideDisc = true;
                                                 });
                                             }
                                             $http.post('sv.api/spk/save?bOverrideDisc=' + overrideDisc, param)
                                              .success(function (result2, status, headers, config) {
                                                  if (result2.success) {
                                                      me.populate(result2.data);
                                                      toastr.success('data saved...');
                                                      $('#ServiceType').attr('disabled', 'disabled');
                                                      me.refreshData();
                                                  }
                                              });
                                         });
                                        }
                                    });
                                    
                                }
                                else {
                                    toastr.warning(result0.data);

                                }
                            }
                        });
                   }
               });
               
           }
       });
    }

    me.cancelDtl = function () {
        $("#pnlTaskPart").slideUp();
        $("#tblTaskPart td .icon").addClass("link");
        $("#btnAddDtl").parent().show();
        $(".dtltp").hide();
        me.gridtaskpart.clearSelection();
    }

    me.updateNewPrice = function () {
        MsgConfirm("Harga akan diupdate dari data Master, proses dilanjutkan ?", function (result) {
            if (result) {
                var params = {
                    serviceNo: $("[name=ServiceNo]").val(),
                    operationNo: $("[name=TaskPartNo]").val()
                }
                $http.post("sv.api/spk/UpdateNewPrice", params).success(function (result, status, headers, config) {
                    if (result.success == true) {
                        me.refreshData();
                        $("#pnlTaskPart").slideUp();
                        $("#tblTaskPart td .icon").addClass("link");
                        $("#btnAddDtl").parent().show();
                        $(".dtltp").hide();
                        me.gridtaskpart.clearSelection();
                        toastr.success('data updated...');
                    } else {
                        MsgBox(result.message, MSG_ERROR);
                    }
                });
            }
            else {
            me.refreshData();
            $("#pnlTaskPart").slideUp();
            $("#tblTaskPart td .icon").addClass("link");
            $("#btnAddDtl").parent().show();
            $(".dtltp").hide();
            me.gridtaskpart.clearSelection();
        }
        });
    }

    me.addDetail = function () {
        //e.preventDefault();
        $scope.renderGrid();
        me.clearDtl();
        me.gridtaskpart.clearSelection();   
        $("#pnlTaskPart").slideDown();
        $("#btnAddDtl").parent().hide();
        $(".dtltp").show();
        $("#tblTaskPart td .icon").removeClass("link");
        $("#btnDlt, #btnUpdNPrice").attr('disabled', 'disabled');
        $("#btnTaskPartNo").prop('disabled', false);

        var params = {
            BasicModel: me.data.BasicModel,
            JobType: me.data.JobType
        }

        $http.post("sv.api/spk/editdetail", params)
       .success(function (result, status, headers, config) {
           var header = {
               IsSparepartClaim: $('#IsSparepartClaim').val(),
               InsurancePayFlag: $('#InsurancePayFlag').val()
           }
           me.validateJobTypeDetailJob(header, result.lockBill, result.lockInsu, result.lockQtyNK);
       });

    }

    me.saveDetail = function () {
        if ($("[Name=TaskPartNo]").val() == 0) {
            MsgBox("Part / Job wajib diisi", MSG_INFO);
            return;
        }
        
        if ($("[Name=OprHourDemandQty]").val() == 0) {
            MsgBox("Qty / NK wajib diisi", MSG_INFO);
            return;
        }
        var params =
        {
            ServiceNo: me.data.ServiceNo,
            BillType: me.data.BillType,
            ItemType: me.data.ItemType,
            TaskPart: me.data.TaskPartNo.replace(",", ""),
            HourQty: me.data.OprHourDemandQty.toString().replace(",", ""),
            TaskPrice: me.data.OprRetailPrice.toString().replace(",", ""),
            DiscPct: me.data.DiscPct.toString().replace(",", ""),
            PartSeq: me.data.TaskPartSeq,
            //PartSeq: $("[Name=TaskPartSeq]").val().replace(",", ""),// me.data.TaskPartSeq.replace(",", ""),
            ChassisCode: me.data.ChassisCode,
            ChassisNo: me.data.ChassisNo,
            BasicModel: me.data.BasicModel,
            JobType: me.data.JobType,
            JobOrderNo: me.data.JobOrderNo
        }

        if (params.PartSeq == "") params.PartSeq = -1;

        if (params.ItemType == "L") {
            $http.post('sv.api/spk/ServiceValidation', params)
            .success(function (result, status, headers, config) {
                if (result.success == false) {
                    MsgBox(result.message, MSG_ERROR);
                    me.refreshData();
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: 'sv.api/spk/savedetail',
                        dataType: 'json',
                        data: params,
                        success: function (rslt) {
                            if (rslt.success == true) {
                                me.refreshData();
                            }
                            else {
                                MsgBox(rslt.message, MSG_ERROR);
                                me.refreshData();
                            }
                            $(".dtltp").hide();
                            $("#btnAddDtl").parent().show();
                        }
                    });
                }
            });
        }
        else {            

            $.ajax({
                type: "POST",
                url: 'sv.api/spk/savedetail',
                dataType: 'json',
                data: params,
                success: function (rslt) {
                    if (rslt.success == true) {
                        me.refreshData();
                    }
                    else {
                        MsgBox(rslt.message, MSG_ERROR);
                        me.refreshData();
                    }
                    $(".dtltp").hide();
                    $("#btnAddDtl").parent().show();    
                }
            });

        }
    }

    me.editDetail = function (row) {
        if (parseInt(status) >= 5) return;
        var params = {
            BasicModel: $('#BasicModel').val(),
            JobType: $('#JobType').val(),
        }

        $http.post("sv.api/spk/editdetail", data)
        .success(function (result, status, headers, config) {
            if (result.success == true) {

                if (row[0] == "F" && me.data.JobType == "FSC01") {
                    if (row[1] != "L" && result.spkAdmin.ParaValue == "0") {
                        return;
                    }
                }

                $("#pnlTaskPart").slideDown();
                $("#btnAddDtl").parent().hide();
                $(".dtltp").show();
                $("#tblTaskPart td .icon").removeClass("link");

                //var data = {
                me.data.BillType= row[0];
                me.data.ItemType= row[1];
                me.data.TaskPartNo= row[5];
                me.data.TaskPartDesc= row[6];
                me.data.QtyAvail= row[9];
                me.data.Price= row[8];
                me.data.OprHourDemandQty= row[7];
                me.data.DiscPct= row[10];
                me.data.PriceNet= row[11];
                me.data.TaskPartSeq= row[12]
                //}
                $('#BillType, #ItemType, #btnTaskPartNo, #TaskPartNo').attr('disabled', 'disabled');

                //me.populate(data);//, "#pnlTaskPart")

                if (me.data.ItemType == "L") {
                    $('#btnUpdNPrice, #Price').removeAttr('disabled');
                    $('#OprHourDemandQty').attr('disabled', 'disabled');
                }
                else {
                    $('#btnUpdNPrice, #Price').attr('disabled', 'disabled');
                    $('#OprHourDemandQty').removeAttr('disabled');
                }

                if (result.lockBill != null) {
                    if (result.lockBill.ParaValue == "0") {
                        var groups = "FSC,CLM,REWORK";
                        if (((result.job.GroupJobType.indexOf("FSC") != -1 || result.job.GroupJobType.indexOf("CLM") != -1 || result.job.GroupJobType.indexOf("REWORK") != -1) || result.job.JobType == "REWORK") == false) {
                            $('[name=BillType]').removeAttr('disabled');
                        }
                    }
                }

                if (result.lockInsu != null) {
                    if (result.lockInsu.ParaValue == "0") {
                        if (result.job.InsurancePayFlag == true) {
                            $('[name=BillType]').removeAttr('disabled');
                        }
                    }
                }

                if (result.lockPrice != null) {
                    if (result.lockPrice.ParaValue == "1") {
                        $('[name=Price]').attr('disabled');
                    }
                }

                if (result.job.GroupJobType == "CLM" && result.job.IsSparepartClaim == true) {
                    $('[name=BillType]').removeAttr('disabled');
                }

                if (result.spkAdmin != null) {
                    if (result.spkAdmin.ParaValue == "1") {
                        $('[name=BillType]', '[name=OprHourDemandQty]').removeAttr('disabled');
                    }
                }

                validatePackage(me.data.TaskPartNo, me.data.ItemType);

                if (result.lockQtyNK != null) {
                    if (result.lockQtyNK.ParaValue == "1") {
                        if (result.job.GroupJobType.indexOf("PB") == 0 && result.job.GroupJobType == "RTN") {
                            $("#OprHourDemandQty").attr('disabled', 'disabled');
                        }
                        else {
                            $("#OprHourDemandQty").removeAttr('disabled');
                        }
                    }
                    else {
                        $("#OprHourDemandQty").removeAttr('disabled');
                    }
                }
                else {
                    $("#OprHourDemandQty").removeAttr('disabled');
                }
            }
        });
    }

    me.deleteDetail = function () {
        //console.log(me.grid.data);
        
        if (me.gridtaskpart.getSelectedId().Status == '5') return;
        
        var partSeq = 0;
        if (me.gridtaskpart.getSelectedId().TypeOfGoods != "L") partSeq = me.gridtaskpart.getSelectedId().SeqNo;

        MsgConfirm("Anda yakin akan menghapus data ini?", function (result) {
            if (result) {
                var params = {
                    ServiceNo: $("[name=ServiceNo]").val(),
                    TaskPartType: me.data.ItemType,
                    TaskPartNo: me.data.TaskPartNo,
                    PartSeq: me.data.TaskPartSeq,
                    InvoiceNo: me.data.InvoiceNo,
                    JobOrderNo: $('#JobOrderNo').val()
                }

                if (params.InvoiceNo != null && params.InvoiceNo != 0) {

                    $.ajax({
                        type: "POST",
                        url: "sv.api/spk/deleteinvoice",
                        dataType: 'json',
                        data: params,
                        success: function (result) {
                            if (result.success) {
                                me.refreshData();
                                me.cancelDtl();
                            }
                            else {
                                MsgBox(result.message, MSG_ERROR);
                            }
                        }
                    });
                }
                else {
                    console.log(params);
                    $.ajax({
                        type: "POST",
                        url: "sv.api/spk/deletedetail",
                        dataType: 'json',
                        data: params,
                        success: function (result) {
                            if (result.success) {
                                me.refreshData();
                                me.cancelDtl();
                            }
                            else {
                                MsgBox(result.message, MSG_ERROR);
                            }

                        }
                    });

                }
            }
            else {
                me.cancelDtl();
            }
        });

    }

    me.PoliceRegistNo = function () {
        //console.log('police regno');
        //var lookup = Wx.blookup({
        //    name: "CustomerVehicle",
        //    title: "Master Customer Vehicle Lookup",
        //    manager: svServiceManager,
        //    query: "CustomerVehicles",
        //    defaultSort: "VinNo asc",
        //    columns: [
        //        { field: "VinNo", title: "Vin No", width: 150 },
        //        { field: "PoliceRegNo", title: "No Polisi", width: 100 },
        //        { field: "CustomerName", title: "Nama Pelanggan", width: 200 },
        //        { field: "BasicModel", title: "Model", width: 100 },
        //        { field: "TransmissionType", title: "Tipe Transmisi", width: 150 },
        //        { field: "ServiceBookNo", title: "No Buku Service", width: 150 },
        //    ]
        //});
        var lookup = Wx.klookup({
            name: "CustomerVehicle",
            title: "Master Customer Vehicle Lookup",
            url: "sv.api/grid/LookUpCusVeh",
            serverBinding: true,
            pageSize: 10,
            //filterable: false,
            sort: [
		        { 'field': 'PoliceRegNo', 'dir': 'desc' },
            ],
            filters: [
                { name: "PoliceRegNo", text: "No. Polisi", cls: "span4" },
                { name: "CustomerName", text: "Nama pelanggan", cls: "span4" },
                { name: "BasicModel", text: "Model", cls: "span4" }
            ],
            columns: [
		        { field: "VinNo", title: "No. Vin", sWidth: "110px" },
                { field: "PoliceRegNo", title: "No. Polisi", sWidth: "110px" },
                { field: "ServiceBookNo", text: "No. Buku Service", sWidth: "110px" },
                { field: "CustomerName", title: "Nama pelanggan" },
                { field: "Address1", title: "Alamat" },
                { field: "PhoneNo", title: "No. Telepon" },
                { field: "BasicModel", title: "Model", sWidth: "80px" },
                { field: "TransmissionType", title: "MT/AT", sWidth: "80px" },
            ]
        });

        lookup.dblClick(function (data) {
            me.populateCustVehicle(data);
            me.populateCustBill(data);
            me.data.JobType = '';
            me.data.JobTypeDesc = '';
            me.Apply();

            $http.post('sv.api/Spk/CheckCampaign', { ChassisCode: data.ChassisCode, ChassisNo: data.ChassisNo, TrsDate: me.data.JobOrderDate }).success(function (data) {
                if (data.success) {
                    MsgBox(data.message, MSG_INFO);
                }
            })

        });
    }

    me.CustCodeBillLkp = function () {
        var lookup = Wx.blookup({
            name: "CustomerBill",
            title: "Customer List",
            manager: svServiceManager,
            query: "customers",
            //source: "sv.api/grid/customers",
            //sortings: [[0, "asc"]],
            columns: [
                { field: "CustomerCode", title: "Kode Pelanggan", sWidth: "180px" },
                { field: "CustomerName", title: "Nama Pelanggan" },
                { field: "HPNo", title: "HP No", sWidth: "120px" },
                { field: "PhoneNo", title: "Phone No", sWidth: "120px" },
            ]
        });
        lookup.dblClick(function (data) {
            me.populateCustBill(data);
            me.Apply();
        });
    }

    me.JobTypeLkp = function () {
        
        var lookup = Wx.blookup({
            name: "JobType",
            title: "Job Type List",
            manager: svServiceManager,
            query: new breeze.EntityQuery.from("JobTypes").withParameters({ BasicModel: (me.data.BasicModel==undefined?"":me.data.BasicModel) }),
            //source: "sv.api/grid/jobtypes",
            columns: [
                { field: "JobType", title: "Job Type", sWidth: "180px" },
                { field: "Description", title: "Job Type Descr" },
                { field: "BasicModel", title: "Model", sWidth: "110px" },
            ]
        });

        lookup.dblClick(function (data) {
            //console.log(data);
            me.data.JobType = data.JobType;
            me.data.JobTypeDesc = data.Description;
            me.Apply();
        });
    }

    me.ForemanIDLkp = function () {
        var lookup = Wx.blookup({
            name: "Foreman",
            title: "Service Advisor List",
            manager: svServiceManager,
            query: "ServiceAdvisors",
            //source: "sv.api/grid/customers",
            //sortings: [[0, "asc"]],
            columns: [
                	{ field: "EmployeeID", title: "NIK", sWidth: "110px" },
					{ field: "EmployeeName", title: "Name" },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.ForemanID = data.EmployeeID;
            me.data.ForemanName = data.EmployeeName;
            me.Apply();
        });
    }

    me.MechanicIDLkp = function () {
        var lookup = Wx.blookup({
            name: "Mechanic",
            title: "Foreman List",
            manager: svServiceManager,
            query: "Foremans",
            //source: "sv.api/grid/foremans",
            //sortings: [[1, "asc"]],
            columns: [
                { field: "EmployeeID", title: "NIK", sWidth: "110px" },
                { field: "EmployeeName", title: "Name" },
            ],
        });
        lookup.dblClick(function (data) {            
            me.data.MechanicID = data.EmployeeID;
            me.data.MechanicName = data.EmployeeName;            
            me.Apply();
        });
    }

    me.DiscPctLkp = function () {
        if (me.data.JobType == undefined || me.data.jobType == "") {
            MsgBox("Silahkan isi Job Type terlebih dahulu", MSG_ERROR);
            return;
        }

        var lookup = Wx.blookup({
            name: "Discount",
            title: "Discount List",
            manager: svServiceManager,
            query: new breeze.EntityQuery.from("ListDiscountServiceLookup").withParameters(
                { customerCode: me.data.CustomerCode, chassisCode: me.data.ChassisCode, chassisNo: me.data.ChassisNo, jobtype: me.data.JobType }),
            //source: "sv.api/grid/ListDiscountServiceLookup",
            //sortings: [[1, "asc"]],
            columns: [
               { field: "SeqNo", title: "No" },
               { field: "DiscountType", title: "Discount Type" },
               { field: "LaborDiscPct", title: "Labor Disct" },
               { field: "PartDiscPct", title: "Part Disct" },
               { field: "MaterialDiscPct", title: "Material Disct" },
            ],
        });
        lookup.dblClick(function (data) {
            me.populate(data);
        });
    }

    me.InvoiceListLkp = function () {
        var lookup = Wx.blookup({
            name: "InvoiceList",
            title: "Invoice List",
            //source: "sv.api/grid/InvoiceList",
            manager: svServiceManager,
            query: new breeze.EntityQuery.from("InvoiceList").withParameters(
                { ChassisCode: me.data.ChassisCode, ChassisNo: me.data.ChassisNo, JobOrderNo: me.data.JobOrderNo }),

            columns: [
                { field: "InvoiceNo", title: "Invoice No" },
                {
                    field: "InvoiceDate", title: "Invoice Date",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                    }
                },
                { field: "JobOrderNo", title: "Job Order No" },
                {
                    field: "JobOrderDate", title: "Job Order Date",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY');
                    }
                },
                { field: "LaborDppAmt", title: "Labor Dpp Amt" },
                { field: "PartsDppAmt", title: "Parts Dpp Amt" },
                { field: "MaterialDppAmt", title: "Material Dpp Amt" },
                { field: "TotalDppAmt", title: "Total Dpp Amt" },
            ]
        });

        lookup.dblClick(function (data) {
            me.InvoiceListClck(data);
        });


    }

    me.InvoiceListClck = function (data) {
        var params = {
            JobOrderNo: me.data.JobOrderNo,// $('#JobOrderNo').val(),
            InvoiceNo: data.InvoiceNo
        }


        $http.post('sv.api/spk/SaveInvoice', params)
                .success(function (rslt, status, headers, config) {
                    if (rslt.Message == '') {
                        $http.post('sv.api/spk/ClaimLis', params)
                       .success(function (rslt, status, headers, config) {
                           $('#tblInvClaim').show();
                           me.populatetable(result.claimList);
                       });
                    } else {
                        widget.alert(result.Message);
                    }

                });


    }

    me.clearData = function () {
        $http.post('sv.api/spk/default').
       success(function (result, status, headers, config) {
           me.data = $.extend({
               ServiceNo: "0",
               //ServiceType: "0",
               TotalSrvAmt: "0",
               Odometer: "0",
               InsuranceOwnRisk: "0",
               //LaborDiscPct: "0.00",
               //PartDiscPct: "0.00",
               //MaterialDiscPct: "0.00",
               ConfirmChangingPart: true,
               LaborDppAmt: "0",
               PartsDppAmt: "0",
               MaterialDppAmt: "0",
               TotalDppAmt: "0",
               TotalPpnAmt: "0",
               SrvTotalSrvAmt: "0",
               ForemanID: result.ForemanID,
               ForemanName: result.ForemanName
           }, result)
           me.IsFSCLock = result.IsFSCLock;
           me.data.JobOrderDate = me.data.EstimateFinishDate = result.JobOrderDate;
       });

        me.data.JobOrderNo = "";
        me.data.ServiceNo = "";
        $("#JobOrderNo").val("");
        $("[name=ServiceNo]").val("");

        //svType = '2';
        $('#ServiceType').removeAttr('disabled');
        $('#JobOrderNo').attr('placeHolder', 'SPK/XX/YYYYY');
        $('[name=IsPPN]').prop('checked', true);
        $('[name=IsPPN]').attr('disabled', 'disabled');
        $('input[name="JobOrderDate"]').removeAttr('disabled');
        totalSrvAmt = 0;
        status = 'N';
        me.clearDtl();
        me.alterUI(status);

        $('#tblTaskPart, #ctlDiscP, #tblInvClaim').hide();
        $("#pnlTaskPart, #pnlInvClaim").slideUp();
        $("#tblTaskPart td .icon").addClass("link");
        $("#tblInvClaim td .icon").addClass("link");
        $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").attr('disabled', 'disabled');
        $('#ctlDisc').show();

        me.data.IsSparepartClaim = false;
        me.data.ConfirmChangingPart = false;
        me.data.InsurancePayFlag = false;

        var value = false;
        $("#IsSparepartClaimY").prop('checked', value).val(value);
        $("#IsSparepartClaimN").prop('checked', !value).val(value);

        var value = false;
        $("#ConfirmChangingPartY").prop('checked', value).val(value);
        $("#ConfirmChangingPartN").prop('checked', !value).val(value);

        var value = false;
        $("#InsurancePayFlagY").prop('checked', value).val(value);
        $("#InsurancePayFlagN").prop('checked', !value).val(value);


        //me.detil = {};

        me.createCookies({
            ServiceType: svType,
            JobOrderNo: me.data.JobOrderNo
        });
    }

    me.clearDtl = function () {
        me.data.SeqNo = "";
        me.data.TaskPartNo = "";
        me.data.TaskPartDesc = "";
        me.data.QtyAvail = "0.00";
        me.data.OprRetailPrice = "0";
        me.data.OprHourDemandQty = "0.00";
        me.data.DiscPct = "0.00";
        me.data.PriceNet = "0";
        //me.Apply();
    }

    me.validateJobTypeDetailJob = function (header, lockBill, lockInsu, lockQtyNK) {

        var data = {
            BasicModel: me.data.BasicModel,
            JobType: me.data.JobType
        }

        $http.post("sv.api/pekerjaan/get", data)
       .success(function (result, status, headers, config) {           
           if (result.success == true) {
        
               if (result.data.GroupJobType == "CLM" || (result.data.GroupJobType == "FSC" && (parseInt(result.data.PdiFscSeq, 1) <= 1) || (parseInt(result.data.PdiFscSeq, 1) >= 5 && parseInt(result.data.PdiFscSeq, 1) <= 9))) {
                   $('[name=btnCustomerCodeBill]').attr('disabled', 'disabled');
               }
               else {
                   $('[name=btnCustomerCodeBill]').removeAttr('disabled');
               }

               if (result.data.GroupJobType == "CLM") {
                   if (header.IsSparepartClaim == true) {
                       me.data.BillType = "S";
                   }
                   else {
                       me.data.BillType = "W";
                   }
                   $('[name=BillType]').attr('disabled', 'disabled');
                   $('[name=ItemType]').removeAttr('disabled');
               }
               else if (result.data.GroupJobType == "FSC") {
                   if (result.data.PdiFscSeq <= 0 || (result.data.PdiFscSeq >= 8 && result.data.PdiFscSeq <= 9)) {
                       me.data.BillType = "F";
                       $('[name=BillType],[name=ItemType]').removeAttr('disabled');
                       $('[name=btnTaskPartNo],[name=TaskPartNo]').attr('disabled', 'disabled');
                       //$('[name=BillType],[name=ItemType],[name=btnTaskPartNo],[name=TaskPartNo]').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.BillType = "C";
                       $('[name=BillType],[name=ItemType]').removeAttr('disabled');
                       $('[name=btnCustomerCodeBill]').attr('disabled', 'disabled');
                       //$('[name=BillType],[name=ItemType],[name=btnCustomerCodeBill]').attr('disabled', 'disabled');
                   }
               }    
               else if (result.data.GroupJobType == "OTH" || result.data.GroupJobType == "RTN") {
                   if (result.detailList == 0) {
                       me.data.BillType = "C";
                       $('[name=ItemType],[name=BillType]').removeAttr('disabled');
                   }
                   else {
                       me.data.BillType = "C";
                       $('[name=BillType]').attr('disabled', 'disabled');
                       $('[name=ItemType]').removeAttr('disabled');
                   }
                   if (lockQtyNK != null) {
                       if (lockQtyNK.ParaValue == "1") {
                           if (result.data.JobType.indexOf("PB") == 0 && result.data.GroupJobType == "RTN") {
                               $("#OprHourDemandQty").attr('disabled', 'disabled');
                           }
                           else {
                               $("#OprHourDemandQty").removeAttr('disabled');
                           }
                       }
                       else {
                           $("#OprHourDemandQty").removeAttr('disabled');
                       }
                   }
                   else {
                       $("#OprHourDemandQty").removeAttr('disabled');
                   }
               }
               else {
                   if (result.data.JobType == "REWORK") {
                       me.data.BillType = "I";
                       $('[name=BillType]').attr('disabled', 'disabled');
                       $('[name=ItemType]').removeAttr('disabled');
                   }
                   else {
                       me.data.BillType = "C";
                       $('[name=ItemType],[name=BillType]').removeAttr('disabled');
                   }
               }

               if(me.grid.data.length>0){
                   me.data.ItemType = "0";                   
                   $('#OprHourDemandQty').removeAttr('disabled');
                   $('#Price, #DiscPct').attr('disabled', 'disabled');
               }
               else {
                   me.data.ItemType = "L";                  
                   $('#TaskPartNo, #OprHourDemandQty, #Price').removeAttr('disabled');
                   $('#ItemType, #DiscPct').attr('disabled', 'disabled');
               }

               if (lockBill != null) {
                   if (lockBill.ParaValue == "0") {
                       var groups = "FSC,CLM,REWORK";
                       if (((result.data.GroupJobType.indexOf("FSC") != -1 || result.data.GroupJobType.indexOf("CLM") != -1 || result.data.GroupJobType.indexOf("REWORK") != -1) || result.data.JobType == "REWORK") == false) {
                           $('[name=BillType]').removeAttr('disabled');
                       }
                   }
               }


               if (lockInsu != null) {
                   if (lockInsu.ParaValue == "0") {
                       if (header.InsurancePayFlag == true) {
                           $('[name=BillType]').removeAttr('disabled');
                       }
                   }
               }
           }
       });
        //me.Apply();
    }

    me.calculateTotalNew = function () {
        var params = {
            OprHourDemandQty: me.data.OprHourDemandQty,
            Price: me.data.OprRetailPrice,
            DiscPct: me.data.DiscPct
        }

        $http.post('sv.api/spk/CalculateTotalNew', params)
        .success(function (rslt, status, headers, config) {
            if (rslt.success) {
                me.RetrieveData(rslt);
            }
        });
    }

    me.calculateTotal = function () {
        var params = {
            OprHourDemandQty: $("[Name=OprHourDemandQty]").val().replace(",", ""),
            Price: $("[Name=Price]").val().replace(",", ""),
            DiscPct: $("[Name=DiscPct]").val().replace(",", "")
        }


        $http.post('sv.api/spk/CalculateTotal', params)
                    .success(function (rslt, status, headers, config) {
                        if (rslt.success) {
                            $('#PriceNet').val(number_format(rslt.PriceNet));
                        }
                    });


    }

    me.refreshData = function () {
        
        var data = $("#pnlServiceInfo").serializeObject();
        data.ServiceNo = me.data.ServiceNo;
        
        //console.log(data);
        //console.log(me.data.ServiceNo);
        //if (widget.JobOrderNo !== data.JobOrderNo) {
        data.showAjax = false;
        $(".ajax-loader").show();

        $http.post("sv.api/spk/get", data)
        .success(function (result, status, headers, config) {
            if (result.success) {
                totalSrvAmt = result.data.TotalSrvAmt;
                console.log(result.data.TotalSrvAmt);
                me.populateData(result);
            }
            else {
                me.data.JobOrderNo = '';
                $(".ajax-loader").hide();
            }
        });
        //}
    }
   
    //$('#ServiceType').on('change', function () {
    //    if (me.data.JobOrderNo == null) {
    //        me.hasChanged = me.isLoadData = me.isSave = me.isEditable = me.isInProcess = false;
    //        me.isInitialize = true;
    //    }
    //});

    //$('#PoliceRegNo').on('change', function () {
    //    alert("test");
    //    me.hasChanged = me.isLoadData = me.isSave = me.isEditable = me.isInProcess = true;
    //    me.isInitialize = false;
    //});
        
    me.initialize = function () {
        me.IsFSCItem = false;
        me.IsFSCLock = false;

        $('#btnDelete').hide();
        me.gridtaskpart.hideColumn("TaskPartSeq");
        me.detail = {};
        me.Apply();
        me.detail.ServiceType = 2;
        me.data = {};
        me.data.ServiceType = 2;
        me.Apply();
        $(".tabs").on('click', function () {
            me.gridtaskpart.adjust();
            //console.log('tabs');
        });
        me.clearData();
        me.clearTable(me.gridtaskpart);

        $(".tabpage1 .summary").attr('style', 'padding-bottom:10px;')
        $(".tabpage1 .indent").attr('style', 'padding-bottom:10px;padding-left:50px;')

        $('#OprHourDemandQty, #Price, #DiscPct').on('blur', function (e) {
            me.calculateTotalNew();
        });
        $('#ServiceType').removeAttr('ng-model');
        $('#ServiceType').val('2');
        $('#PriceNet, #LaborDppAmt, #PartsDppAmt, #MaterialDppAmt, #TotalDppAmt, #TotalPpnAmt, #SrvTotalSrvAmt, #Odometer').on('blur', function (e) {
            $('#PriceNet').val(number_format($('#PriceNet').val()));
            $('#LaborDppAmt').val(number_format($('#LaborDppAmt').val()));
            $('#PartsDppAmt').val(number_format($('#PartsDppAmt').val()));
            $('#MaterialDppAmt').val(number_format($('#MaterialDppAmt').val()));
            $('#TotalDppAmt').val(number_format($('#TotalDppAmt').val()));
            $('#TotalPpnAmt').val(number_format($('#TotalPpnAmt').val()));
            $('#SrvTotalSrvAmt').val(number_format($('#SrvTotalSrvAmt').val()));
            $('#Odometer').val(number_format($('#Odometer').val()));
        });
        
        $("#InsurancePayFlagN").on('change', function (e) {
            $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").attr('disabled', 'disabled');
        });
        $("#InsurancePayFlagY").on('change', function (e) {
            $("#InsuranceOwnRisk, #InsuranceJobOrderNo, #InsuranceNo").removeAttr('disabled');
        });

        // Create Cookies
        $('#ServiceType').on('change', function (e) {
            me.createCookies({
                ServiceType: $(this).val(),
                JobOrderNo: me.data.JobOrderNo
            });
        });

        $('#JobOrderNo').on('change', function (e) {
            me.createCookies({
                ServiceType: me.data.ServiceType,
                JobOrderNo: $(this).val()
            });
        });
        //
        $('#btnLkpKendaraan, #btnLkpPelanggan').show();

        //me.isSave = true;
        //me.startEditing();
        //me.hasChanged = true;
        //me.isInitialize = true;

        $('#PoliceRegNo, #JobType, #JobOrderNo').removeAttr('readonly');
        $('#btnPoliceRegNo, #btnJobType, #btnJobOrderNo').removeAttr('disabled');
    }

    me.createCookies = function (data) {
        createCookie('ServiceType', data.ServiceType, 1);
        createCookie('JobOrderNo', data.JobOrderNo, 1);
    }
    
    me.showPelanggan = function () {
        Wx.loadForm();
        Wx.showForm({ url: "gn/master/customer", params: me.data.CustomerCode });
    }

    me.showKendaraan = function () {
        //var params = {
        //    chassisCode: me.data.ChassisCode,
        //    chassisNo: me.data.ChassisNo
        //}
        //Wx.loadForm();
        //Wx.showForm({ url: "sv/master/kdanp", params: me.data.ChassisCode });
        var data = {
            ChassisCode: me.data.ChassisCode,
            ChassisNo: me.data.ChassisNo
        }
        Wx.loadForm();
        Wx.showForm({ url: "sv/master/kdanp", params: JSON.stringify(data) });
    }

    me.showHistVeh = function () {
        Wx.loadForm();
        Wx.showForm({ url: "sv/master/historyVehicle", params: me.data.PoliceRegNo });
    }

    me.printspk = function () {
        if (me.detail.ServiceType == '2') {
            Wx.loadForm();
            Wx.showForm({ url: "sv/trans/spkprint", params: me.data.JobOrderNo });
        }
        else if (me.detail.ServiceType == '0') {
            Wx.loadForm();
            Wx.showForm({ url: "sv/trans/spkprintestimasi", params: me.data.JobOrderNo });
        }
    }

    me.$watch('CustVehicle', function (newValue, oldValue) {
        if (!me.isInProcess) {
            var eq = (newValue == oldValue);
            if (!(_.isEmpty(newValue)) && !eq) {
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

    }, true);
    

    webix.event(window, "resize", function () {
        me.gridtaskpart.adjust();
    })


    $scope.renderGrid = function () {        
        setTimeout(function () {
            me.gridtaskpart.adjust();
        }, 50);
    }

    me.RetrieveData = function (value) {
        if (value == null || value == undefined) return;
        me.isLoadData = true;
        setTimeout(function () {
            //me.hasChanged = false;
            //me.startEditing();
            //me.isSave = false;

            me.ReformatNumber();
            var selectorContainer = "";
            $.each(value, function (key, val) {
                var ctrl = $(selectorContainer + " [name=" + key + "]");
                me.data[key] = val;
                ctrl.removeClass("error");
            });

            $scope.$apply();
        }, 60);
    }
    me.start();
}

//me.isSave = true;
//me.startEditing();
//me.hasChanged = false;
//me.isInitialize = true;

$(document).ready(function () {
    var xbtn = [];
    xbtn = WxButtons.slice();
    xbtn.push(
        { name: "btnCreate", text: "Buat SPK", cls: "btn btn-info", icon: "icon-plus", click: "createspk()" },
        { name: "btnClose", text: "Tutup SPK", cls: "btn btn-info", icon: "icon-remove", click: "closespk()" },
        { name: "btnCancelSPK", text: "Batal SPK", cls: "btn btn-info", icon: "icon-remove", click: "batalspk()" },
        { name: "btnPrintSPK", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printspk()" }
    );

    var options = {
        title: "Input SPK (SA)",
        xtype: "panels",
        id: "titleSPK",
        toolbars: xbtn,
        panels: [
             {
                 name: "pnlInfoSPK",
                 items: [
                      {
                          type: "div", name: "StatusSO", cls: "span3 left", style: "font-size:25px;color:blue;text-align:center"
                      },
                      {
                          type: "div", name: "totalSPK", cls: "span5 left", style: "font-size:25px;color:blue;text-align:center"
                      }
                 ]
             },
              {
                  name: "pnlButton",
                  title: "",
                  items: [
                      { name: "StatusSO", text: "", cls: "span5", readonly: true, type: "label" },
                      {
                          type: "buttons", cls: "span6",
                          items: [
                                      { name: "btnHistVeh", text: "History Kendaraan", icon: "icon-user", cls: "btn btn-info", click: "showHistVeh()" },
                                      { name: "btnLkpKendaraan", text: "Kendaraan", icon: "icon-user", cls: "btn btn-info", click: "showKendaraan()" },
                                      { name: "btnLkpPelanggan", text: "Pelanggan", icon: "icon-user", cls: "btn btn-info", click: "showPelanggan()" }
                          ]
                      }

                  ]
              },
            {
                name: "pnlServiceInfo",
                title: "Informasi Service",
                items: [
                    { name: "ServiceNo", model: "data.ServiceNo", type: "hidden" },
                    {
                        name: "ServiceType",
                        text: "Tipe Service",
                        type: "select",
                        cls: "span4 full",
                        model: "detail.ServiceType",
                        items: [
                            { value: '0', text: 'ESTIMASI' },
                            { value: '1', text: 'BOOKING' },
                            { value: '2', text: 'SPK' }
                        ]
                    },
                    { name: "JobOrderNo", model: "data.JobOrderNo", text: "No. SPK", placeHolder: "SPK/YY/9999", cls: "span4", validasi: "required", type: "popup", click: "browse()" },
                    { name: "JobOrderDate", model: "data.JobOrderDate", text: "Tgl. SPK", cls: "span4", type: "ng-datetimepicker" },
                ]
            },
            {
                name: "tabpage1",
                xtype: "tabs",
                items: [
                    { name: "CustVehicle", model: "data.CustVehicle", text: "Kendaraan & Pelanggan" },
                    { name: "TaskPart", model: "data.TaskPart", text: "Pekerjaan" },
                    { name: "InvClaim", model: "data.InvClaim", text: "List Invoice Claim" }
                ]
            },
            {
                name: "CustVehicle",    
                title: "Kendaraan & Pelanggan",
                cls: "tabpage1 CustVehicle",
                items: [
                    { name: "PoliceRegNo", model: "data.PoliceRegNo", text: "No Polisi", cls: "span4", type: "popup", click: "PoliceRegistNo()", validasi: "required" },
                    { name: "ServiceBookNo", model: "data.ServiceBookNo", text: "No Buku Service", cls: "span4", readonly: true },
                    { name: "BasicModel", model: "data.BasicModel", text: "Basic Model", cls: "span4", readonly: true },
                    { name: "TransmissionType", model: "data.TransmissionType", text: "Trans", cls: "span4", readonly: true },
                    { name: "ChassisCode", model: "data.ChassisCode", text: "Kode Rangka", cls: "span4", readonly: true },
                    { name: "ChassisNo", model: "data.ChassisNo", text: "Nomor Rangka", cls: "span4", readonly: true },
                    { name: "EngineCode", model: "data.EngineCode", text: "Kode Mesin", cls: "span4", readonly: true },
                    { name: "EngineNo", model: "data.EngineNo", text: "Nomor Mesin", cls: "span4", readonly: true },
                    { name: "ColorCode", model: "data.ColorCode", text: "Warna Kendaraan", cls: "span4", readonly: true },
                    { name: "Odometer", model: "data.Odometer", text: "KM (Odometer)", cls: "span4 number-int", validasi: "required", maxlength: 10, style: "background-color:#ffcccc;" },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", model: "data.CustomerCode", cls: "span2", placeHolder: "Kode", readonly: true },
                            { name: "CustomerName", model: "data.CustomerName", cls: "span6", placeHolder: "Nama", readonly: true }
                        ]
                    },
                    { name: "CustAddr1", model: "data.CustAddr1", text: "Alamat", maxlength: 100, readonly: true },
                    { name: "CustAddr2", model: "data.CustAddr2", text: "", maxlength: 100, readonly: true },
                    { name: "CustAddr3", model: "data.CustAddr3", text: "", maxlength: 100, readonly: true },
                    {
                        text: "Kota",
                        type: "controls",
                        items: [
                            { name: "CityCode", model: "data.CityCode", cls: "span2", placeHolder: "Kode", readonly: true },
                            { name: "CityName", model: "data.CityName", cls: "span6", placeHolder: "Deskripsi", readonly: true }
                        ]
                    }
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Kontrak & Club",
                items: [
                    { name: "ContractNo", model: "data.ContractNo", text: "No Kontrak", placeHolder: "No", cls: "span2", readonly: true },
                    { name: "ContractEndPeriod", model: "data.ContractEndPeriod", text: "Berlaku s/d Tgl", cls: "span3", readonly: true },
                    { name: "ContractStatus", model: "data.ContractStatus", text: "Status", cls: "span3", readonly: true },
                    { name: "ClubCode", model: "data.ClubCode", text: "Club No", placeHolder: "No Club", cls: "span2", readonly: true },
                    { name: "ClubEndPeriod", model: "data.ClubEndPeriod", text: "Berlaku s/d Tgl", placeHolder: "expired", cls: "span3", readonly: true },
                    { name: "ClubStatus", model: "data.ClubStatus", text: "Status", cls: "span3", readonly: true }
                ]
            },
            {
                cls: "tabpage1 CustVehicle",
                title: "Pembayar",
                items: [
                    { name: "InsurancePayFlag", model: "data.InsurancePayFlag", text: "Asuransi", cls: "span4", type: "switch", float: "left" },
                    { name: "InsuranceOwnRisk", model: "data.InsuranceOwnRisk", text: "Own Risk", cls: "span4 number" },
                    { name: "InsuranceNo", model: "data.InsuranceNo", text: "No Polis", cls: "span4" },
                    { name: "InsuranceJobOrderNo", model: "data.InsuranceJobOrderNo", text: "No SPK Asuransi", cls: "span4" },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        items: [
                            { name: "CustomerCodeBill", model: "data.CustomerCodeBill", cls: "span2", placeHolder: "Kode", validasi: "required", type: "popup", click: "CustCodeBillLkp()" },
                            { name: "CustomerNameBill", model: "data.CustomerNameBill", cls: "span6", placeHolder: "Nama", readonly: true }
                        ]
                    },
                    { name: "CustAddr1Bill", model: "data.CustAddr1Bill", text: "Alamat", maxlength: 100, readonly: true },
                    { name: "CustAddr2Bill", model: "data.CustAddr2Bill", text: "", readonly: true },
                    { name: "CustAddr3Bill", model: "data.CustAddr3Bill", text: "", readonly: true },
                    {
                        text: "Kota",
                        type: "controls",
                        items: [
                            { name: "CityCodeBill", model: "data.CityCodeBill", cls: "span2", placeHolder: "Kode", readonly: true },
                            { name: "CityNameBill", model: "data.CityNameBill", cls: "span6", placeHolder: "Nama", readonly: true }
                        ]
                    },
                    {
                        text: "Kontak",
                        type: "controls",
                        items: [
                            { name: "PhoneNo", model: "data.PhoneNo", cls: "span2", placeHolder: "Telepon", readonly: true },
                            { name: "FaxNo", model: "data.FaxNo", cls: "span3", placeHolder: "Fax", readonly: true },
                            { name: "HPNo", model: "data.HPNo", cls: "span3", placeHolder: "HP", readonly: true }
                        ]
                    },
                    {
                        type: "controls",
                        name: "ctlDiscP",
                        text: "% Diskon",
                        items: [
                            { name: "pLaborDiscPct", model: "data.pLaborDiscPct", cls: "span2 number", placeHolder: "Jasa", type: "popup", btnName: "btnLaborDiscPct", click: "DiscPctLkp()" },
                            { name: "pPartDiscPct", model: "data.pPartDiscPct", placeHolder: "Part", cls: "span2 number", type: "popup", btnName: "btnPartDiscPct", click: "DiscPctLkp()" },
                            { name: "pMaterialDiscPct", model: "data.pMaterialDiscPct", placeHolder: "Material", cls: "span2 number", type: "popup", btnName: "btnMaterialDiscPct", click: "DiscPctLkp()" }
                        ]
                    },
                    {
                        type: "controls",
                        name: "ctlDisc",
                        items: [
                            { name: "LaborDiscPct", model: "data.LaborDiscPct", cls: "span2 number", placeHolder: "Jasa", readonly: true },
                            { name: "PartDiscPct", model: "data.PartDiscPct", placeHolder: "Part", cls: "span2 number", readonly: true },
                            { name: "MaterialDiscPct", model: "data.MaterialDiscPct", placeHolder: "Material", cls: "span2 number", readonly: true }
                        ]
                    },
                    { name: "IsPPN", model: "data.IsPPN", text: "PPN", type: "switch", float: "left" }
                ]
            },
            {
                cls: "tabpage1 TaskPart",
                title: "Pekerjaan",
                items: [
                    { name: "ServiceRequestDesc", model: "data.ServiceRequestDesc", text: "Permintaan Pekerjaan", type: "textarea" },
                    {
                        text: "Jenis Pekerjaan",
                        type: "controls",
                        items: [
                            { name: "JobType", model: "data.JobType", cls: "span2", placeHolder: "Kode", type: "popup", click: "JobTypeLkp()", validasi: "required" },
                            { name: "JobTypeDesc", model: "data.JobTypeDesc", cls: "span6", placeHolder: "Deskripsi", readonly: true }
                        ]
                    },
                    { name: "ConfirmChangingPart", model: "data.ConfirmChangingPart", text: "Pergantian Part", type: "switch", float: "left" },
                    {
                        text: "SA",
                        type: "controls",
                        items: [
                            { name: "ForemanID", cls: "span2", placeHolder: "Kode", type: "popup", click: "ForemanIDLkp()", validasi: "required" },
                            { name: "ForemanName", cls: "span6", placeHolder: "Nama", readonly: true }
                        ]
                    },
                    {
                        text: "FM",
                        type: "controls",
                        items: [
                            { name: "MechanicID", cls: "span2", placeHolder: "Kode", type: "popup", click: "MechanicIDLkp()", validasi: "required" },
                            { name: "MechanicName", cls: "span6", placeHolder: "Nama", readonly: true }
                        ]
                    },
                    {
                        text: "Perkiraan Selesai",
                        type: "controls",
                        items: [
                            { name: "EstimateFinishDate", type: "ng-datetimepicker", cls: "span2" }
                        ]
                    },
                    { name: "IsSparepartClaim", text: "Sparepart Claim?", type: "switch", cls: "span4", float: "left" }
                ]
            },
            {
                cls: "tabpage1 TaskPart",
                title: "Detail Task / Part",
                items: [
                    {
                        type: "buttons", cls: "toolbars", items: [
                            { name: "btnAddDtl", text: "Add New Task Part", cls: "btn btn-success", icon: "icon-plus", click: "addDetail()" }
                        ]
                    },
                    { name: "TaskPartSeq", type: "hidden" },
                    {
                        name: "BillType", model: "data.BillType", text: "Ditanggung Oleh", cls: "span3 dtltp", type: "select2", datasource: 'BillTypeDtSrc'
                    },
                    {
                        name: "ItemType", model: "data.ItemType", text: "Item Type", cls: "span3 dtltp", type: "select",
                        items: [
                            { value: "L", text: "Labor (Jasa)" },
                            { value: "0", text: "Sparepart & Material" }
                        ]
                    },
                    {
                        text: "No Part / Pekerjaan",
                        type: "controls",
                        cls: "span6 dtltp",
                        items: [
                            { name: "TaskPartNo", model: "data.TaskPartNo", cls: "span3", placeHolder: "Kode", type: "popup", readonly: false, click: "lkuTaskPart()", required: true, validasi: "required" },
                            { name: "TaskPartDesc", model: "data.TaskPartDesc", cls: "span5", placeHolder: "Deskripsi", readonly: true }
                        ]
                    },
                    { name: "QtyAvail", model: "data.QtyAvail", text: "Qty Avail", cls: "span3 number dtltp", readonly: true },
                    { name: "OprRetailPrice", model: "data.OprRetailPrice", text: "Harga", cls: "span3 number-int dtltp" },
                    { name: "OprHourDemandQty", model: "data.OprHourDemandQty", text: "Qty / NK", cls: "span3 number dtltp", required: true, validasi: "required" },
                    { name: "DiscPct", model: "data.DiscPct", text: "Discount", cls: "span3 number dtltp" },
                    { name: "PriceNet", model: "data.PriceNet", text: "Harga Net", cls: "span3 number-int dtltp", readonly: true },
                    {
                        type: "buttons", cls: "span6 dtltp", items: [
                        { name: "btnAdd", text: "Save", icon: "icon-save", click: "saveDetail()", cls: "btn btn-success", disable: "data.TaskPartNo === undefined" },
                        { name: "btnDlt", text: "Delete", icon: "icon-remove", click: "deleteDetail()", cls: "btn btn-danger", show: "!IsFSCItem" },
                        { name: "btnCancelDtl", text: "Cancel", cls: "btn btn-warning", icon: "icon-undo", click: "cancelDtl()" },
                        { name: "btnUpdNPrice", text: "Update New Price", icon: "icon-save", cls: "btn btn-success", click: "updateNewPrice()" }
                        ]
                    },
                    {
                        name: "wxtaskpart",
                        type: "wxdiv"
                    },
                    { name: "LaborDppAmt", text: "DPP - Jasa", cls: "span6 summary number-int", readonly: true },
                    { name: "PartsDppAmt", text: "DPP - Part", cls: "span6 summary number-int", readonly: true },
                    { name: "MaterialDppAmt", text: "DPP - Material", cls: "summary span6 number-int", readonly: true },
                    { name: "TotalDppAmt", text: "Total DPP", cls: "span6 indent summary number-int", readonly: true },
                    { name: "TotalPpnAmt", text: "Total PPN", cls: "span6 indent summary number-int", readonly: true },
                    { name: "SrvTotalSrvAmt", text: "Total Biaya Perawatan", cls: "span6 indent summary number-int", readonly: true }
                ]
            },
            {
                cls: "tabpage1 InvClaim",
                title: "Invoice Claim",
                xtype: "table",
                pnlname: "pnlInvClaim",
                tblName: "tblInvClaim",
                name: "tblInvClaim",
                buttons: [{ name: "btnInvoiceNo", text: "Invoice No", icon: "icon-search" }],
                columns: [
                    { text: "Action", type: "action", width: 80 },
                    { name: "InvoiceNo", text: "No Invoice" },
                    { name: "InvoiceDate", text: "Tgl. Invoice", type: "dateTime" },
                    { name: "JobOrderNo", text: "No. SPK" },
                    { name: "JobOrderDate", text: "Tgl. SPK", type: "dateTime" },
                    { name: "LaborDppAmt", text: "DPP Jasa", cls: "right" },
                    { name: "PartsDppAmt", text: "DPP Part", cls: "right" },
                    { name: "MaterialDppAmt", text: "DPP Material", cls: "right" },
                    { name: "TotalDppAmt", text: "Total Dpp", cls: "right" }
                ]
            }
        ]
    };
        
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);


    function init(s) {
        SimDms.Angular  ("svEntrySPKController");
    }

});