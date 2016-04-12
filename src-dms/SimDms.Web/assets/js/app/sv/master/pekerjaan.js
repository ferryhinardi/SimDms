"use strict"


function svMstJobController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.getTable = function (data) {
        $http.post('sv.api/pekerjaan/getdatatable', data).
               success(function (data, status, headers, config) {
                   me.grid.detail = data;
                   me.loadTableData(me.grid1, me.grid.detail);
               }).
               error(function (e, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   console.log(e);
               });
    }

    me.getData = function () {
        if(me.data.JobType != "" && me.data.BasicModel !=""){
        $http.post('sv.api/pekerjaan/get', me.data).
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    me.lookupAfterSelect(dl.data);
                    $('#BasicModel').attr('disabled', true);
                    $('#JobType').attr('disabled', true);
                    $('#btnBasicModel').attr('disabled', true);
                    $('#btnJobType').attr('disabled', true);
                    me.CheckDtl();

                    // IsFSCLock
                    me.IsFSCLock = (me.IsFSCLock && me.data.GroupJobType == 'FSC');
                    if(me.IsFSCLock)
                    {
                        $('#OperationHour').attr('disabled', true);
                    }
                    else {
                        $('#OperationHour').removeAttr('disabled');
                    }
                } 
        });
        }
    }

    me.CheckDtl = function () {
        if (me.data.JobType && me.data.BasicModel) {
            $http.post('sv.api/pekerjaan/getdata', { model: me.data, groupJobType: me.data.GroupJobType }).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        $("#pnlRincianPekerjaan").removeClass("hide");
                        $('#BillType').removeAttr('disabled');
                        $("#pnlRincianPanel").removeClass("hide");
                        $('#OperationNo').attr('disabled', true);
                        me.detail.oid = true;
                        me.data.ProductType = dl.data.ProductType;
                        me.detail.OperationNo = dl.data.OperationNo;
                        me.detail.Description = dl.data.Description;
                        me.detail.TechnicalModelCode = dl.data.TechnicalModelCode;
                        me.detail.IsActive = dl.data.IsActive;
                        me.detail.IsCampaign = dl.data.IsCampaign;
                        me.detail.IsSubCon = dl.data.IsSubCon;
                        me.detail.OperationHour = dl.data.OperationHour;
                        me.detail.ClaimHour = dl.data.ClaimHour;
                        me.detail.BillType = dl.data.BillType;
                        me.detail.LaborCost = dl.data.LaborCost;
                        me.detail.LaborPrice = dl.data.LaborPrice;
                        //me.Apply();
                        me.data.OperationNo = dl.data.OperationNo;
                        me.getTable(me.data);
                        me.grid1.adjust();


                    } else {
                        $("#pnlRincianPekerjaan").removeClass("hide");
                        me.laborPrice();
                        me.techModel(me.data.BasicModel);
                    };
                });
        }
        
    }

    me.laborPrice = function () {
        $http.post('sv.api/pekerjaan/laborprice?LaborCode=Customer').
                        success(function (dl, status, headers, config) {
                            me.detail.LaborPrice = dl.data.LaborPrice;
                            $('#BillType').val("C");
                            $('#BillType').attr('disabled', true);
                            $('#OperationNo').removeAttr('disabled');
                        });
    }

    me.techModel = function (BasicModel) {
        $http.post('sv.api/pekerjaan/tehnicalmodel?BasicModel=' + BasicModel).
                        success(function (dl, status, headers, config) {
                            me.detail.TechnicalModelCode = dl.data;
                        });
    }

    me.browse = function () {
        me.init();
        var lookup = Wx.blookup({
            name: "JobBrowse",
            title: "Master Job Lookup",
            manager: MasterService,
            query: "JobBrowse",
            defaultSort: "BasicModel asc",
            columns: [
                { field: "BasicModel", title: "Basic Model", fillspace: true },
                { field: "JobType", title: "Pekerjaan", fillspace: true },
                { field: "JobDescription", title: "Deskripsi Pekerjaan", fillspace: true },
                { field: "GroupJobType", title: "Group Pekerjaan", fillspace: true },
                { field: "GroupJobDescription", title: "Deskripsi Group Pekerjaan", width: 150 },
                { field: "WarrantyOdometer", title: "Garansi KM", fillspace: true, template: '<div style="text-align:right;">#= kendo.toString(WarrantyOdometer, "n0") #</div>' },
                { field: "WarrantyTimePeriod", title: "Garansi Waktu", fillspace: true, template: '<div style="text-align:right;">#= kendo.toString(WarrantyTimePeriod, "n0") #</div>' },
                { field: "IsPdiFscStr", title: "PDI/FSC", fillspace: true },
                { field: "PdiFscSeq", title: "FS#", fillspace: true, template: '<div style="text-align:right;">#= kendo.toString(PdiFscSeq, "n0") #</div>' },
                { field: "Status", title: "Status", fillspace: true }
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            me.getData();
            me.isSave = false;
            if (me.Pbpr==true) {
                if (data.JobType.substring(0,2).toUpperCase() == 'PB') {
                    me.disabledAll();
                    $("#btnSave").hide();
                }
                else {
                    me.enableAll();
                    $("#btnSave").show();
                }
            }
            me.Apply();
        });
    }

    me.browse2 = function () {
        var lookup = Wx.blookup({
            name: "JobBrowse",
            title: "Pencarian Master Task",
            manager: MasterService,
            query: new breeze.EntityQuery().from("lookupforJob").withParameters({ productType: me.data.ProductType, basicModel: me.data.BasicModel, jobType: me.data.JobType }),//"lookupforJob",
            defaultSort: "OperationNo asc",
            columns: [
                { field: "OperationNo", title: "Jenis Pekerjaan", fillspace: true },
                { field: "Description", title: "Keterangan", fillspace: true },
                { field: "TechnicalModelCode", title: "Technical Model", fillspace: true },
                { field: "IsSubCon", title: "Sub-Con?", fillspace: true },
                { field: "IsCampaign", title: "Campaign?", fillspace: true },
                { field: "IsActive", title: "Status", fillspace: true },
                { field: "OperationHour", title: "Lama Pengerjaan-Pelangggan", fillspace: true },
                { field: "ClaimHour", title: "ama Pengerjaan-Claim", fillspace: true },
            ]
        });
        lookup.dblClick(function (data) {
            $('#BillType').removeAttr('disabled');
            $("#pnlRincianPanel").removeClass("hide");
            me.detail.oid = true;
            me.detail.Description = data.Description;
            me.detail.OperationNo = data.OperationNo;
            me.detail.TechnicalModelCode = data.TechnicalModelCode;
            me.detail.IsCampaign = data.IsCampaign != "Ya" ? false : true;
            me.detail.IsSubCon = data.IsSubCon != "Ya" ? false : true;
            me.detail.IsActive = data.IsActive != "Aktif" ? false : true;
            me.detail.LaborPrice = data.LaborPrice;
            me.detail.LaborCost = data.LaborCost;
            me.detail.ClaimHour = data.ClaimHour;
            me.detail.OperationHour = data.OperationHour;
            $('#OperationNo').attr('disabled', true);
            me.detail.BasicModel = me.data.BasicModel;
            me.detail.JobType = me.data.JobType;
            if (data.LaborPrice == null || data.LaborCost == null)
            {
                var param = {
                    BasicModel: me.detail.BasicModel,
                    JobType: me.detail.JobType,
                    OperationNo: me.detail.OperationNo
                };
                $http.post("sv.api/pekerjaan/GetTaskPrice", param)
                .success(function (result) {
                    console.log(result[0].LaborPrice);
                    me.detail.LaborPrice = result[0].LaborPrice;
                    me.detail.LaborCost = result[0].LaborCost;
                    me.detail.ClaimHour = result[0].ClaimHour;
                    me.detail.OperationHour = result[0].OperationHour;
                });
            }
            me.getTable(me.detail);
            me.Apply();
        });
    }

    $('#BasicModel').on('blur', function () {
        if (me.data.BasicModel == "" || me.data.BasicModel == null) return;
        $http.post('sv.api/pekerjaan/GetBasicModel', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.BasicModel = data.data.RefferenceCode;
            } else {
                me.data.BasicModel = "";
                me.BasicModel();
            }
        });
    });

    me.BasicModel = function () {
        var lookup = Wx.blookup({
            name: "BasicModelBrowse",
            title: "BasicModel Lookup",
            manager: MasterService,
            query: "BasmodPekerjaan",
            defaultSort: "BasicModel asc",
            columns: [
                { field: "BasicModel", title: "Basic Model", fillspace: true },
                { field: "TechnicalModelCode", title: "Technical Model", fillspace: true },
                { field: "ModelDescription", title: "Keterangan ", fillspace: true },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BasicModel = data.BasicModel;
            me.detail.TechnicalModelCode = data.TechnicalModelCode;
            me.getData();
            me.Apply();
        });
    }

    me.JobType = function () {
        var lookup = Wx.blookup({
            name: "JobTypeBrowse",
            title: "JobType Lookup",
            manager: MasterService,
            query: "JobView",
            defaultSort: "JobType asc",
            columns: [
                { field: "JobType", title: "JobType", fillspace: true },
                { field: "JobDescription", title: "JobDescription", fillspace: true },
                { field: "Status", title: "Is Active", fillspace: true },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.JobType = data.JobType;
            me.data.JobDescription = data.JobDescription;
            me.getData();
            me.Apply();
        });
    }

    me.GroupJobType = function () {
        var lookup = Wx.blookup({
            name: "GroupJobTypeBrowse",
            title: "GroupJobType Lookup",
            manager: MasterService,
            query: "GroupJobView",
            defaultSort: "GroupJobType asc",
            columns: [
                { field: "GroupJobType", title: "Group Pekerjaan", fillspace: true },
                { field: "GroupJobDescription", title: "Deskripsi Group Pekerjaan", fillspace: true },
                { field: "Status", title: "Status", fillspace: true }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.GroupJobType = data.GroupJobType;
            me.data.GroupJobDescription = data.GroupJobDescription;
            me.Apply();
        });
    }
    
    me.AccountNo = function () {
        var lookup = Wx.blookup({
            name: "AccountLookUp",
            title: "Lookup Account",
            manager: MasterService,
            query: "NomorAccView",
            defaultSort: "AccountNo desc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "AccDescription", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ReceivableAccountNo = data.AccountNo;
                me.data.AccDescription = data.AccDescription;
                me.Apply();
            }
        });
    };

    me.PartNo = function () {
        var lookup = Wx.blookup({
            name: "PartNoUp",
            title: "Lookup No.Part",
            manager: MasterService,
            query: "PartInfo4Sv",
            defaultSort: "PartNo asc",
            columns: [
                { field: "PartNo", title: "No. Part" },
                { field: "PartName", title: "Keterangan" },
                { field: "RetailPriceInclTax", title: "Harga Jual + Pajak" },
                { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail2.PartNo = data.PartNo;
                me.detail2.RetailPrice = data.RetailPriceInclTax;
                me.detail2.PartName = data.PartName;
                me.Apply();
            }
        });
    };
    
    //me.Copy = function (e) {
    //    if (confirm("Apakah anda yakin???")) {
    //        var param = $(".main .gl-widget").serializeObject();
    //        $http.post("sv.api/pekerjaan/prosesbasicmodel", param, function (result) {
    //            if (result.success) {
    //                SimDms.Success("data copied...");
    //                widget.clearForm();
    //                clear("new");
    //            }
    //        });
    //        console.log(param);
    //    }
    //}

    me.Copy = function () {
        Wx.loadForm();
        Wx.showForm({ url: "sv/master/copyjobs" });
    }

    me.saveData = function Pertama(p) {
        $http.post('sv.api/pekerjaan/savepertama', me.data)
        .success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                me.startEditing();
                $("#btnSimpanDetail").removeClass('hide');
                $("#pnlRincianPekerjaan").removeClass("hide");
            } else {
                MsgBox(data.message, MSG_INFO);
            }
        }).
            error(function (data, status, headers, config) {
                MsgBox("Connection to the server failed...", MSG_INFO);
            });
    };

    me.save2 = function () {
        console.log(me.data);
            var Field = "OperationNo,Description,LaborCost";
            var Names = "Jenis Pekerjaan,Keterangan,Biaya Pekerjaan Luar";
            var ret = me.CheckMandatory(Field, Names);
            if (ret != "") {
                MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
            } else {
                me.detail.BasicModel = me.data.BasicModel;
                me.detail.JobType = me.data.JobType;
                $http.post('sv.api/pekerjaan/savekedua', me.detail)
                  .success(function (data, status, headers, config) {
                      if (data.success) {
                          Wx.Success("Data saved...");
                          me.startEditing();
                          //me.cancel2();
                          $("#pnlRincianPanel").removeClass("hide");
                          $("#wxRincian").removeClass("hide");
                      } else {
                          MsgBox(data.message, MSG_INFO);
                      }
                  }).
                      error(function (data, status, headers, config) {
                          MsgBox("Connection to the server failed...", MSG_INFO);
                      });
            }
    }

    me.save3 = function (p) {
        var Field = "PartNo,Quantity";
        var Names = "No.Part,Quantity";
        var ret = me.CheckMandatory(Field, Names);
        if (ret != "") {
            MsgBox(ret + " Harus diisi terlebih dahulu !", MSG_INFO);
        } else {
            me.detail2.BasicModel = me.data.BasicModel;
            me.detail2.JobType = me.data.JobType;
            me.detail2.OperationNo = me.detail.OperationNo;
            $http.post('sv.api/pekerjaan/saveketiga', me.detail2)
              .success(function (data, status, headers, config) {
                  if (data.success) {
                      Wx.Success("Data saved...");
                      me.startEditing();
                      me.getTable(me.detail2);
                      me.detail2 = {};
                  } else {
                      MsgBox(data.message, MSG_INFO);
                  }
              }).
                  error(function (data, status, headers, config) {
                      MsgBox("Connection to the server failed...", MSG_INFO);
                  });
        }
        var isValid = $(".main form").valid();
        if (isValid) {
            var param = $(".main form").serializeObject();

            //return false;
            widget.post("sv.api/pekerjaan/saveketiga", param, function (result) {
                if (result.success) {
                    SimDms.Success("data saved...");
                    getTable();
                }
            });

        }
    };

    me.delete = function () {
        $http.post('sv.api/pekerjaan/getData', me.data).
               success(function (data, status, headers, config) {
                   if (data.data != null) {
                       MsgBox("Data tidak boleh di hapus karena ada detail!", MSG_INFO);
                   } else {
                       MsgConfirm("Are you sure to delete current record?", function (result) {
                           $http.post('sv.api/pekerjaan/deletepertama', me.detail).
                           success(function (data, status, headers, config) {
                               if (data.success) {
                                   me.init();
                                   Wx.Success("Data deleted...");
                               } else {
                                   MsgBox("Terjadi Kesalahan Pada Proses Data, Silahkan Hubungi SDMS Support!", MSG_INFO);
                               }
                           }).
                           error(function (data, status, headers, config) {
                               MsgBox("Terjadi Kesalahan Pada Proses Data, Silahkan Hubungi SDMS Support!", MSG_INFO);
                           });
                       });
                   }
               }).
               error(function (e, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   console.log(e);
               });
    }

    me.delete2 = function () {
        me.detail.BasicModel = me.data.BasicModel;
        me.detail.JobType = me.data.JobType;
        $http.post('sv.api/pekerjaan/getdatatable', me.detail).
               success(function (data, status, headers, config) {
                   if (data[0]) {
                       MsgBox("Data tidak boleh di hapus karena ada detail!", MSG_INFO);
                   } else {
                       MsgConfirm("Are you sure to delete current record?", function (result) {
                                   $http.post('sv.api/pekerjaan/deletekedua', me.detail).
                                   success(function (data, status, headers, config) {
                                       if (data.success) {
                                           me.cancel2();
                                           Wx.Success("Data deleted...");
                                           me.techModel(me.data.BasicModel)
                                       } else {
                                           MsgBox("Terjadi Kesalahan Pada Proses Data, Silahkan Hubungi SDMS Support!", MSG_INFO);
                                       }
                                   }).
                                   error(function (data, status, headers, config) {
                                       MsgBox("Terjadi Kesalahan Pada Proses Data, Silahkan Hubungi SDMS Support!", MSG_INFO);
                                   });
                               });
                   }
               }).
               error(function (e, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   console.log(e);
               });
    }

    me.delete3 = function (row) {
        me.detail2.BasicModel = me.data.BasicModel;
        me.detail2.JobType = me.data.JobType;
        me.detail2.OperationNo = me.detail.OperationNo;
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('sv.api/pekerjaan/deleteketiga', me.detail2).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.cancel3();
                    Wx.Success("Data deleted...");
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox("Connection to the server failed...", MSG_INFO);
            });
        });
    }

    me.cancel2 = function () {
        me.detail = {};
        me.detail.oid = undefined;
        me.laborPrice();
        me.detail.LaborCost = 0;
        me.techModel(me.data.BasicModel);
        $("#pnlRincianPanel").addClass("hide", "hide");
        me.detail.IsActive = true;
    };

    me.cancel3 = function () {
        me.detail2.BasicModel = me.data.BasicModel;
        me.detail2.JobType = me.data.JobType;
        me.detail2.OperationNo = me.detail.OperationNo;
        me.getTable(me.detail2);
        me.detail2 = {};
        me.detail2.BillType = "C";
    };

    me.initialize = function () {
        me.enableAll();
        me.data = {};
        me.detail = {};
        me.detail2 = {};
        $http.post('sv.api/pekerjaan/default').success(function (result) {
            me.Pbpr = result.Pbpr;
            me.data = result;
            me.data.WarrantyTimeDim = 'D';
            me.IsFSCLock = result.IsFSCLock;
        });
        //$('#IsActive').prop('checked', true);
        $("#pnlRincianPekerjaan").addClass("hide", "hide");
        $("#pnlRincianPanel").addClass("hide", "hide");
        me.clearTable(me.grid1);
        me.detail.BillType = "C";
        me.detail2.BillType = "C";
        $('#BasicModel').removeAttr('disabled');
        $('#JobType').removeAttr('disabled');
        $('#btnBasicModel').removeAttr('disabled', true);
        $('#btnJobType').removeAttr('disabled', true);
        me.detail.IsActive = true;
        me.isSave = false;

    };

    me.disabledAll = function () {

        $("#btnJobType").attr("disabled", "disabled");
        $("#btnGroupJobType").attr("disabled", "disabled");
        $("#JobDescription").attr("disabled", "disabled");
        $("#GroupJobDescription").attr("disabled", "disabled");
        $("#WarrantyOdometer").attr("disabled", "disabled");
        $("#WarrantyTimePeriod").attr("disabled", "disabled");
        $("#WarrantyTimeDim").attr("disabled", "disabled");
        $("#AccDescription").attr("disabled", "disabled");
        $("#btnAccountNo").attr("disabled", "disabled");
        $("#btnHapus").attr("disabled", "disabled");
        $("#btnBrowseBasmod").attr("disabled", "disabled");
        $("#OperationNo").attr("disabled", "disabled");
        $("#BillType").attr("disabled", "disabled");
        $("#Description").attr("disabled", "disabled");
        $("#TechnicalModelCode").attr("disabled", "disabled");
        $("#OperationHour").attr("disabled", "disabled");
        $("#ClaimHour").attr("disabled", "disabled");
        $("#LaborCost").attr("disabled", "disabled");
        $("#LaborPrice").attr("disabled", "disabled");
        $("#btnPartNo").attr("disabled", "disabled");
        $("#Quantity").attr("disabled", "disabled");
        $("#RetailPrice").attr("disabled", "disabled");
        $("#PartName").attr("disabled", "disabled");
        $("#BillTypePart").attr("disabled", "disabled");
        $("#btnAdd").attr("disabled", "disabled");
        $("#btnDlt").attr("disabled", "disabled");
        $("#GroupJobType,#IsPdiFsc,#ReceivableAccountNo,#btnReceivableAccountNo,#IsActiveP").prop('disabled', 'disabled');
        $("#btnUpdateModel,#btnDeleteModel,#btnCancelModel,#BillType,#IsSubCon").prop('disabled', 'disabled');
        $("#PartNo,#btnAddRincianPart").prop('disabled', 'disabled');
        
    }
    me.enableAll = function () {
        $("#btnJobType").removeAttr("disabled", "disabled");
        $("#btnGroupJobType").removeAttr("disabled", "disabled");
        $("#JobDescription").removeAttr("disabled", "disabled");
        $("#GroupJobDescription").removeAttr("disabled", "disabled");
        $("#WarrantyOdometer").removeAttr("disabled", "disabled");
        $("#WarrantyTimePeriod").removeAttr("disabled", "disabled");
        $("#WarrantyTimeDim").removeAttr("disabled", "disabled");
        $("#AccDescription").removeAttr("disabled", "disabled");
        $("#btnAccountNo").removeAttr("disabled", "disabled");
        $("#btnHapus").removeAttr("disabled", "disabled");
        $("#btnBrowseBasmod").removeAttr("disabled", "disabled");
        $("#OperationNo").removeAttr("disabled", "disabled");
        $("#BillType").removeAttr("disabled", "disabled");
        $("#Description").removeAttr("disabled", "disabled");
        $("#TechnicalModelCode").removeAttr("disabled", "disabled");
        $("#OperationHour").removeAttr("disabled", "disabled");
        $("#ClaimHour").removeAttr("disabled", "disabled");
        $("#LaborCost").removeAttr("disabled", "disabled");
        $("#LaborPrice").removeAttr("disabled", "disabled");
        $("#btnPartNo").removeAttr("disabled", "disabled");
        $("#Quantity").removeAttr("disabled", "disabled");                
        $("#BillTypePart").removeAttr("disabled", "disabled");
        $("#btnAdd").removeAttr("disabled", "disabled");
        $("#btnDlt").removeAttr("disabled", "disabled");
        $("#GroupJobType,#IsPdiFsc,#ReceivableAccountNo,#btnReceivableAccountNo,#IsActiveP").prop('disabled', false);
        $("#btnUpdateModel,#btnDeleteModel,#btnCancelModel,#BillType,#IsSubCon,#LaborCost").prop('disabled', false);
        $("#PartNo,#btnAddRincianPart").prop('disabled', false);
    }

    me.clear = function (p) {
        if (p == "clear") {
            $("#btnSave").addClass("hide");
            $("#btnSimpanDetail").addClass("hide");
            $("#btnEdit").addClass("hide");
            $("#btnDelete").addClass("hide");
        } else if (p == "dbclick") {
            $("#btnSimpanDetail").addClass("hide");
            $("#btnEdit").removeClass('hide');
            $("#btnDelete").removeClass('hide');
            $("#btnSave").addClass("hide");
            $("#Odometer").attr("readonly", "readonly");
            $("#TimePeriod").attr("readonly", "readonly");
        } else if (p == "new") {
            // widget.clearForm();
            // clearData();
            $("#btnSimpanDetail").addClass("hide");
            $("#btnSave").addClass("hide");
            $("#btnEdit").addClass("hide");
            $("#btnDelete").addClass("hide");
            $("#pnlRefService").removeClass("hide");
            $("#pnlRincianPekerjaan").removeClass("hide");
            $("#pnlRincianPanel").removeClass("hide");
            $("#PnlPopup").addClass("hide", "hide");
            $("#PnlTabel").removeClass("hide");

            $("#btnBasicModel").removeAttr("disabled", "disabled");
            $("#btnUpdate").removeAttr("disabled", "disabled");
            $("#btnJobType").removeAttr("disabled", "disabled");
            $("#btnGroupJobType").removeAttr("disabled", "disabled");
            $("#JobDescription").removeAttr("disabled", "disabled");
            $("#GroupJobDescription").removeAttr("disabled", "disabled");
            $("#WarrantyOdometer").removeAttr("disabled", "disabled");
            $("#WarrantyTimePeriod").removeAttr("disabled", "disabled");
            $("#WarrantyTimeDim").removeAttr("disabled", "disabled");
            $("#AccDescription").removeAttr("disabled", "disabled");
            $("#btnAccountNo").removeAttr("disabled", "disabled");
            $("#btnHapus").removeAttr("disabled", "disabled");
            $("#btnBrowseBasmod").removeAttr("disabled", "disabled");
            $("#OperationNo").removeAttr("disabled", "disabled");
            $("#BillType").removeAttr("disabled", "disabled");
            $("#Description").removeAttr("disabled", "disabled");
            $("#TechnicalModelCode").removeAttr("disabled", "disabled");
            $("#OperationHour").removeAttr("disabled", "disabled");
            $("#ClaimHour").removeAttr("disabled", "disabled");
            $("#LaborCost").removeAttr("disabled", "disabled");
            $("#LaborPrice").removeAttr("disabled", "disabled");
            $("#btnPartNo").removeAttr("disabled", "disabled");
            $("#Quantity").removeAttr("disabled", "disabled");
            $("#RetailPrice").removeAttr("disabled", "disabled");
            $("#PartName").removeAttr("disabled", "disabled");
            $("#BillTypePart").removeAttr("disabled", "disabled");
            $("#btnAdd").removeAttr("disabled", "disabled");
            $("#btnDlt").removeAttr("disabled", "disabled");

            $("#pnlRincianPekerjaan").addClass("hide", "hide");
            $("#pnlRincianPanel").addClass("hide", "hide");
            $("#PnlTabel").addClass("hide", "hide");
        } else if (p == "btnEdit") {
            $("#btnSimpanDetail").addClass("hide");
            $("#Description").removeAttr('readonly');
            $("#OperationNo").removeAttr('readonly');
            $("#btnSave").removeClass('hide');
            $("#btnOperationNo").removeAttr('disabled');
        }
    };

    me.grid1 = new webix.ui({
        container: "wxrincian",
        view: "wxtable", css:"alternating",
        columns: [
                { id: "PartNo", header: "No.Part", fillspace: true },
                { id: "Quantity", header: "Quantity", fillspace: true },
                { id: "RetailPrice", header: "Harga Jual", fillspace: true },
                { id: "PartName", header: "Nama Part", fillspace: true },
                { id: "BillTypeDesc", header: "Ditanggung", fillspace: true },
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail2 = this.getItem(me.grid1.getSelectedId().id);
                    me.detail2.oid = me.grid1.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.start();

    $('#btnCreate').on('click', function (e) {
        clear("new");
    });

    $('#btnEdit').on('click', function (e) {
        clear("btnEdit");

    });

    $('#IsPdiFsc').on('change', function (e) {
        if ($('#IsPdiFsc').prop('checked') == true) 
            $('#PdiFscSeq').removeAttr('disabled');
        else $('#PdiFscSeq').attr('disabled', true);
    });

    $('#ReceivableAccountNo').on('blur', function (e) {
        if (me.data.ReceivableAccountNo == null || me.data.ReceivableAccountNo == '') return;
        $http.post('sv.api/pekerjaan/AccountNo?ReceivableAccountNo=' + me.data.ReceivableAccountNo).
        success(function (data) {
            if (data.success) {
                me.data.ReceivableAccountNo = data.data.AccountNo;
                me.data.AccDescription = data.data.AccDescription;
            }
            else {
                me.data.ReceivableAccountNo = me.data.AccDescription = '';
                me.AccountNo();
            }
        })
    });

    $('#JobType').on('blur', function (e) {
        if (me.data.JobType == null || me.data.JobType == '') return;
        $http.post('sv.api/pekerjaan/JobType?JobType=' + me.data.JobType).
        success(function (data) {
            console.log(data.success);
            if (data.success) {
                me.data.JobType = data.data.RefferenceCode;
                me.data.JobDescription = data.data.Description;
            }
            else {
                me.data.JobType = me.data.JobDescription = '';
                me.JobType();
            }
        });
    });

    $('#GroupJobType').on('blur', function (e) {
        if (me.data.GroupJobType == null || me.data.GroupJobType == '') return;
        $http.post('sv.api/pekerjaan/GroupJobType?GroupJobType=' + me.data.GroupJobType).
        success(function (data) {
            if (data.success) {
                me.data.GroupJobType = data.data.GroupJobType;
                me.data.GroupJobDescription = data.data.GroupJobDescription;
            }
            else{
                me.data.GroupJobType = me.data.GroupJobDescription = '';
                me.GroupJobType();
            }
        });
    });

    $('#IsSubCon').on('change', function (e) {
        if ($('#IsSubCon').prop('checked') == true) {
            $('#PartNo').attr('disabled', true);
            $('#LaborCost').attr('disabled', false);
        } else {
            $('#PartNo').attr('disabled', false);
            $('#LaborCost').attr('disabled', true);
        }
    })

    me.printPreview = function () {
        Wx.loadForm();
        Wx.showForm({ url: "sv/master/jobprint" });
    }
}

$(document).ready(function () {
    var options = {
        title: "Pekerjaan",
        xtype: "panels",
        toolbars: [{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                  { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                  { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                  { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                  { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" }],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    {
                        text:"Basic Model",
                        type:"controls",
                        items:[
                            { name: "BasicModel", cls: "span3", text: "Basic Model", type: "popup", required: true, validasi: "required", click: "BasicModel()"},
                            
                            { type: "buttons", items: [
                                    { name: "btnUpdate", text: " Copy pekerjaan untuk Basic Model baru", icon: "icon-gear", cls: "btn btn-info", click: "Copy()" },
                                ]
                            },
                        ]
                    },
                    { type: "separator" },
                    {
                        text: "Pekerjaan",
                        type: "controls",
                        items: [
                            { name: "JobType", cls: "span3", text: "Pekerjaan", type: "popup", required: true, validasi: "required", click: "JobType()" },
                            { name: "JobDescription", text: "Deskripsi Pekerjaan", cls: "span5", readonly: true },
                        ]
                    },
                    {
                        text: "Group Pekerjaan",
                        type: "controls",
                        items: [
                            { name: "GroupJobType", cls: "span3", text: "Group Pekerjaan", type: "popup", required: true, validasi: "required", click: "GroupJobType()" },
                            { name: "GroupJobDescription", text: "Deskripsi Group Pekerjaan", cls: "span5", readonly: true },
                        ]
                    },
                    { name: "WarrantyOdometer", text: "KM Service", cls: "span4 number-int", required: true, validasi: "required", maxlength: 11 },
                    { name: "IsPdiFsc", text: "PDI/FSC", cls: "span2", type: 'check' },
                    { name: "PdiFscSeq", text: "FS#", cls: "span2 number-int", disable: true, maxlength: 6 },
                    { name: "WarrantyTimePeriod", text: "Garansi Waktu", cls: "span4 number-int", required: true, validasi: "required", maxlength: 11 },
                    { name: "WarrantyTimeDim", text: "Satuan Waktu", cls: "span4", type: "select", required:"required",
                        items: [
                            { value: "D", text: "HARI" },
                            { value: "M", text: "BULAN" },
                            { value: "Y", text: "TAHUN" },
                        ]
                    },
                    {
                        text: "No Acc. Penjualan",
                        type: "controls",
                        items: [
                            { name: "ReceivableAccountNo", cls: "span3", text: "No Acc. Penjualan", type: "popup", btnName: "btnAccountNo", click: "AccountNo()" },
                            { name: "AccDescription", model: "data.AccDescription", text: "Deskripsi Account", cls: "span5", readonly: true },
                        ]
                    },
                    { name: "IsActiveP", model: "data.IsActive", text: "Status Aktif", cls: "span4", type: "ng-switch", float: "left" },
                    
                ]
            },
            {
                name: "pnlRincianPekerjaan",
                title: "Rincian Pekerjaan",
                cls:"hide",
                items: [
                    {
                        type: "buttons", items: [
                                { name: "btnBrowseBasmod", text: "Browse", icon: "icon-search", click: "browse2()", type :'popup' },
                                { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "save2()", show: "detail.oid === undefined", disable: "detail.OperationNo === undefined" },
                                { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "save2()", show: "detail.oid !== undefined" },
                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete2()", show: "detail.oid !== undefined" },
                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "cancel2()", show: "detail.oid !== undefined" }
                        ]
                    },
                    { name: "OperationNo", model :"detail.OperationNo", cls: "span4", text: "Jenis Pekerjaan", required: "required", maxlength: 50 },
                    {
                        name: "BillType", model :"detail.BillType", text: "Ditanggung", required:"required", cls: "span4", type: "select",
                        items: [
                            { value: "F", text: "Free" },
                            { value: "C", text: "Customer" },
                            { value: "W", text: "Warranty" },
                            { value: "I", text: "Internal" },

                        ]
                    },
                    { name: "Description", model :"detail.Description", text: "Keterangan", required: "required", maxlength: 100 },
                    { name: "TechnicalModelCode", model :"detail.TechnicalModelCode", text: "Technical Model", cls: "span4", readonly:true },
                    { name: "IsSubCon", model :"detail.IsSubCon", text: "SubCon?", cls: "span2", type: "ng-check", float: "left" },
                    { name: "IsCampaign", model :"detail.IsCampaign", text: "Campaign?", cls: "span2", type: "ng-switch", float: "left" },
                    {
                        text: "",
                        type: "controls",
                        items: [
                             { type: "div", text: "" },
                             { type: "label", text: "Lama Pengerjaan" },
                        ]
                    },
                     { type: "hr" },
                     { name: "OperationHour", model: "detail.OperationHour", text: "Pelanggan", placeholder: "0.00", cls: "span3 number", maxlength: 7 },
                     { type: "label", cls: "span1", text: "Jam" },
                     { name: "LaborCost", model: "detail.LaborCost", text: "Biaya Pekerjaan Luar", placeholder: "0", cls: "span4 number-int", disable: "detail.IsSubCon === undefined", value: 0 },
                     { name: "ClaimHour", model: "detail.ClaimHour", text: "Claim", placeholder: "0.00", cls: "span3 number", maxlength: 7 },
                     { type: "label", cls: "span1", text: "Jam" },
                     { name: "LaborPrice", model: "detail.LaborPrice", text: "Nilai Jasa", placeholder: "0", cls: "span4 number-int", maxlength: 18 },
                     { type: "hr" },
                     { name: "IsActiveR", model :"detail.IsActive", text: "Status", cls: "span4", type: "ng-switch", float: "left" },
                ]
            },
            {
                name: "pnlRincianPanel",
                title: "Rincian Part",
                cls: "hide",
                items: [
                    
                    { name: "PartNo", model :"detail2.PartNo", cls: "span4", text: "No. Part", type: "popup", btnName: "btnPartNo", click :"PartNo()" },
                    { name: "Quantity",  model :"detail2.Quantity", text: "Quantity", cls: "span4 number", style: "background-color: rgb(255, 218, 204)" },
                    { name: "RetailPrice",  model :"detail2.RetailPrice", text: "Harga Jual", cls: "span4" , disable : true},
                    { name: "PartName", model: "detail2.PartName", text: "Nama Part", cls: "span4", disable: true },
                    { name: "BillTypePart",  model :"detail2.BillType", text: "Ditanggung", cls: "span4", type: "select", 
                        items: [
                            { value: "F", text: "Free" },
                            { value: "C", text: "Customer" }

                        ]
                    },
                    {
                        type: "buttons", items: [
                                { name: "btnAddRincianPart", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "save3()"},
                                //{ name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "save3()", show: "detail2.oid !== undefined" },
                                { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete3()", show: "detail2.oid !== undefined" },
                                { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "cancel3()", show: "detail2.oid !== undefined" }
                        ]
                    },
                     {
                         name: "wxrincian",
                         //cls : "hide",
                         type: "wxdiv"
                     },
                ]
            },
            

        ],
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstJobController");
    }

});