var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function CustomerVehicleController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "CustVehBrowse",
            title: "Master Customer Vehicle Lookup",
            manager: MasterService,
            query: "CustVehBrowse",
            defaultSort: "PoliceRegNo asc",
            columns: [
                { field: "PoliceRegNo", title: "No Polisi" },
                { field: "ServiceBookNo", title: "No Buku Service", fillspace: true },
                { field: "CustomerName", title: "Pelanggan", fillspace: true },
                { field: "ChassisCode", title: "Kode Chassis", fillspace: true },
                { field: "ChassisNo", title: "No Chassis", fillspace: true },
                { field: "BasicModel", title: "Basic Model", fillspace: true },
                { field: "EngineCode", title: "Kode Mesin", fillspace: true },
                { field: "EngineNo", title: "No Mesin", fillspace: true }
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            me.LoadBrowse(data);
            localStorage.setItem('BasicModelOld', data.BasicModel);
            localStorage.setItem('EngineCodeOld', data.EngineCode); localStorage.setItem('EngineNoOld', data.EngineNo);
            localStorage.setItem('ServiceBookNoOld', data.ServiceBookNo); localStorage.setItem('TransmissionTypeOld', data.TransmissionType);
            localStorage.setItem('PoliceRegNoOld', data.PoliceRegNo); localStorage.setItem('DealerCodeOld', data.DealerCode);
            localStorage.setItem('FakturPolisiDateOld', data.PoliceRegDate); localStorage.setItem('CustomerCodeOld', data.CustomerCode);
            localStorage.setItem('StatusOld', data.Status);
            $http.post("sv.api/customervehicle/HistCustomerVehicle", data).
                   success(function (data, status, headers, config) {
                       me.loadTableData(me.grid1, data);
                       //me.Apply();
                   });
            $('#ChassisCode').attr('disabled', 'disabled');
            $('#btnChassisCode').attr('disabled', 'disabled');
            $('#ChassisNo').attr('disabled', 'disabled');
            $('#EngineNo').attr('disabled', 'disabled');
            $('#TechnicalModelCode').attr('disabled', 'disabled');
            me.Apply();
        });
    }

    me.LoadBrowse = function (data) {
        $http.post('sv.api/customervehicle/LoadBrowse?BasicModel=' + data.BasicModel + '&ColourCode=' + data.ColourCode + '&DealerCode=' + data.DealerCode).
           success(function (data, status, headers, config) {
               me.data.TechnicalModelCode = data.DescriptionEng;
               me.data.ColourName = data.ColourName;
               me.data.DealerName = data.DealerName;
               localStorage.setItem('TechnicalModelCodeOld', data.DescriptionEng);
           }).
           error(function (e, status, headers, config) {
               console.log(e);
           });
    }

   me.initialize = function () {

       //$('#IsActive').prop('checked', true);
       $('#ChassisCode').removeAttr('disabled');
       $('#btnChassisCode').removeAttr('disabled');
       $('#ChassisNo').removeAttr('disabled');

       $('#wxPTS').hide();
       $('#wxPTK').hide();
       $('#wxPTC').hide();
       $('#pnlPatch').hide();

       me.data = {};
       if (localStorage.getItem('params')) {
       var params = localStorage.getItem('params');
       me.data.ChassisCode = JSON.parse(params).ChassisCode;
       me.data.ChassisNo = JSON.parse(params).ChassisNo;
       }
       if (me.data.ChassisCode != undefined || me.data.ChassisNo != undefined) {
           $http.post('sv.api/customervehicle/CustVehicle', { ChassisCode: me.data.ChassisCode, ChassisNo: me.data.ChassisNo }).
             success(function (data, status, headers, config) {
                 me.data = data[0];
                 localStorage.removeItem('params')
                 $('#ChassisCode').attr('disabled', 'disabled');
                 $('#btnChassisCode').attr('disabled', 'disabled');
                 $('#ChassisNo').attr('disabled', 'disabled');
                 $('#EngineNo').attr('disabled', 'disabled');
                 $('#TechnicalModelCode').attr('disabled', 'disabled');
                 me.Apply();
             }).
             error(function (e, status, headers, config) {
                 console.log(e);
             });
       }

       me.clearTable(me.grid1);
       me.data.IsActive = true;
       //me.Apply();
   }

   me.save = function () {
       var EngineCodeOld = localStorage.getItem('EngineCodeOld');
       var EngineNoOld = localStorage.getItem('EngineNoOld');
       var BasicModelOld = localStorage.getItem('BasicModelOld');
       var TechnicalModelCodeOld = localStorage.getItem('TechnicalModelCodeOld');
       var ServiceBookNoOld = localStorage.getItem('ServiceBookNoOld');
       var TransmissionTypeOld = localStorage.getItem('TransmissionTypeOld');
       var PoliceRegNoOld = localStorage.getItem('PoliceRegNoOld');
       var DealerCodeOld = localStorage.getItem('DealerCodeOld');
       var FakturPolisiDateOld = localStorage.getItem('FakturPolisiDateOld');
       var CustomerCodeOld = localStorage.getItem('CustomerCodeOld');
       var StatusOld = localStorage.getItem('StatusOld');
       
       $http.post('sv.api/customervehicle/save', me.data)
        .success(function (e) {
            if (e.success) {
                me.startEditing();
                if (e.StatusDoc == "old") {
                    //1
                    if (EngineCodeOld != me.data.EngineCode) {
                        $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=EGC&PreviousData=' + EngineCodeOld).
                         success(function (e) {
                             if (e.success) {
                                 localStorage.removeItem('EngineCodeOld');
                                 localStorage.setItem('EngineCodeOld', me.data.EngineCode);
                             } else {
                                 MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                             }
                         })
                         .error(function (e) {
                         });
                    }
                    //2
                    if (EngineNoOld != me.data.EngineNo) {
                        $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=EGN&PreviousData=' + EngineNoOld).
                         success(function (e) {
                             if (e.success) {
                                 localStorage.removeItem('EngineNoOld');
                                 localStorage.setItem('EngineNoOld', me.data.EngineNo);
                             } else {
                                 MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                             }
                         })
                         .error(function (e) {
                         });
                    }
                    //3
                    if (BasicModelOld != me.data.BasicModel) {
                        $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=BAS&PreviousData=' + BasicModelOld).
                         success(function (e) {
                             if (e.success) {
                                 localStorage.removeItem('BasicModelOld');
                                 localStorage.setItem('BasicModelOld', me.data.BasicModel);
                             } else {
                                 MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                             }
                         })
                         .error(function (e) {
                         });
                    }
                    //4
                    if (TechnicalModelCodeOld != me.data.TechnicalModelCode) {
                        //console.log("Technical Model");
                        $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=TCH&PreviousData=' + TechnicalModelCodeOld).
                         success(function (e) {
                             if (e.success) {
                                 localStorage.removeItem('TechnicalModelCodeOld');
                                 localStorage.setItem('TechnicalModelCodeOld', me.data.TechnicalModelCode);
                             } else {
                                 MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                             }
                         })
                         .error(function (e) {
                         });
                    }
                    //5
                    if (ServiceBookNoOld != me.data.ServiceBookNo) {
                        $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=SRV&PreviousData=' + ServiceBookNoOld).
                         success(function (e) {
                             if (e.success) {
                                 localStorage.removeItem('ServiceBookNoOld');
                                 localStorage.setItem('ServiceBookNoOld', me.data.ServiceBookNo);
                             } else {
                                 MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                             }
                         })
                         .error(function (e) {
                         });
                    }
                    //6
                    if (TransmissionTypeOld != me.data.TransmissionType) {
                        $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=TRM&PreviousData=' + TransmissionTypeOld).
                         success(function (e) {
                             if (e.success) {
                                 localStorage.removeItem('TransmissionTypeOld');
                                 localStorage.setItem('TransmissionTypeOld', me.data.TransmissionType);
                             } else {
                                 MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                             }
                         })
                         .error(function (e) {
                         });
                    }
                    //7
                    if (PoliceRegNoOld != me.data.PoliceRegNo) {
                        $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=POL&PreviousData=' + PoliceRegNoOld).
                         success(function (e) {
                             if (e.success) {
                                 localStorage.removeItem('PoliceRegNoOld');
                                 localStorage.setItem('PoliceRegNoOld', me.data.PoliceRegNo);
                             } else {
                                 MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                             }
                         })
                         .error(function (e) {
                         });
                    }
                    //8
                    if (DealerCodeOld != me.data.DealerCode) {
                        $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=DLR&PreviousData=' + DealerCodeOld).
                         success(function (e) {
                             if (e.success) {
                                 localStorage.removeItem('DealerCodeOld');
                                 localStorage.setItem('DealerCodeOld', me.data.DealerCode);
                             } else {
                                 MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                             }
                         })
                         .error(function (e) {
                         });
                    }
                    ////9
                    //if (FakturPolisiDateOld != me.data.FakturPolisiDate) {
                    //    $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=FDP&PreviousData=' + FakturPolisiDateOld.toString()).
                    //     success(function (e) {
                    //         if (e.success) {
                    //             localStorage.setItem('FakturPolisiDateOld', '');
                    //             Wx.Success("Data Saved");
                    //         } else {
                    //             MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                    //         }
                    //     })
                    //     .error(function (e) {
                    //     });
                    //}
                    //10
                    if (CustomerCodeOld != me.data.CustomerCode) {
                        $http.post('sv.api/customervehicle/CustomerVehicleHistoryData?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&ChangeCode=CUS&PreviousData=' + CustomerCodeOld).
                         success(function (e) {
                             if (e.success) {
                                 localStorage.removeItem('CustomerCodeOld');
                                 localStorage.setItem('CustomerCodeOld', me.data.CustomerCode);
                             } else {
                                 MsgBox('Terdapat kesalahan proses save data. Please contact sdms support...', MSG_INFO);
                             }
                         })
                         .error(function (e) {
                         });
                    }
                }
                Wx.Success("Data Save");

                setTimeout(function () {
                    $http.post("sv.api/customervehicle/HistCustomerVehicle?ChassisCode=" + me.data.ChassisCode + "&ChassisNo=" + me.data.ChassisNo).
                           success(function (data, status, headers, config) {
                               me.clearTable(me.grid1);
                               me.loadTableData(me.grid1, data);
                               //me.Apply();
                           });
                }, 3600)
                $('#ChassisCode').attr('disabled', 'disabled');
                $('#btnChassisCode').attr('disabled', 'disabled');
                $('#ChassisNo').attr('disabled', 'disabled');
                $('#EngineNo').attr('disabled', 'disabled');
                $('#TechnicalModelCode').attr('disabled', 'disabled');
                //me.Apply();

            } else {
                MsgBox(e.message, MSG_ERROR);
            }
        })
        .error(function (e) {
            console.log(e);
            MsgBox('Terdapat kesalahan proses data. Please contact sdms support...', MSG_INFO);
        });

   }

   me.ChassisCode = function () {
       var lookup = Wx.blookup({
           name: "Chassis",
           title: "Chassis",
           manager: MasterService,
           query: "ChassicCodeOpen",
           defaultSort: "ChassisCode, ChassisNo",
           columns: [
               { field: "ChassisCode", title: "Chassis Code" },
               { field: "ChassisNo", title: "Chassis No" },
               { field: "EngineCode", title: "Engine Code" },
               { field: "EngineNo", title: "Engine No" },
               { field: "ServiceBookNo", title: "Service Book No" },
               { field: "PoliceRegNo", title: "Police Reg No" },
               { field: "PoliceRegDate", title: "Faktur Police Date" },
               { field: "Status", title: "Is Active" }
           ]
       });
       lookup.dblClick(function (data) {
           if (data != null) {
               me.data = data;

               localStorage.setItem('EngineCodeOld', me.data.EngineCode); localStorage.setItem('EngineNoOld', me.data.EngineNo);
               localStorage.setItem('BasicModelOld', me.data.BasicModel); localStorage.setItem('TechnicalModelCodeOld', me.data.TechnicalModelCode);
               localStorage.setItem('ServiceBookNoOld', me.data.ServiceBookNo); localStorage.setItem('TransmissionTypeOld', me.data.TransmissionType);
               localStorage.setItem('PoliceRegNoOld', me.data.PoliceRegNo); localStorage.setItem('DealerCodeOld', me.data.DealerCode);
               localStorage.setItem('FakturPolisiDateOld', me.data.PoliceRegDate); localStorage.setItem('CustomerCodeOld', me.data.CustomerCode);
               localStorage.setItem('StatusOld', me.data.Status);

               $http.post("sv.api/customervehicle/ChassisAction", data).
                   success(function (v, status, headers, config) {
                       if (v.data != null) {
                           me.data = v.data;

                           localStorage.setItem('EngineCodeOld', me.data.EngineCode); localStorage.setItem('EngineNoOld', me.data.EngineNo);
                           localStorage.setItem('BasicModelOld', me.data.BasicModel); localStorage.setItem('TechnicalModelCodeOld', me.data.TechnicalModelCode);
                           localStorage.setItem('ServiceBookNoOld', me.data.ServiceBookNo); localStorage.setItem('TransmissionTypeOld', me.data.TransmissionType);
                           localStorage.setItem('PoliceRegNoOld', me.data.PoliceRegNo); localStorage.setItem('DealerCodeOld', me.data.DealerCode);
                           localStorage.setItem('FakturPolisiDateOld', me.data.PoliceRegDate); localStorage.setItem('CustomerCodeOld', me.data.CustomerCode);
                           localStorage.setItem('StatusOld', me.data.Status);
                       }
                   });
               $http.post("sv.api/customervehicle/HistCustomerVehicle", data).
                   success(function (data, status, headers, config) {
                       if(data)
                       me.loadTableData(me.grid1, data);
                       //me.Apply();
                   });
               //me.Apply();
           }
       });
   }

   me.delete = function () {
       MsgConfirm("Are you sure to delete current record?", function (result) {
           if (result) {
               $http.post('sv.api/customervehicle/Delete', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.init();
                       Wx.Success("Data deleted...");
                   } else {
                       MsgBox(data.message, MSG_ERROR);
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
           }
       });
   }

   me.printPreview = function () {
       Wx.loadForm();
       Wx.showForm({ url: "sv/master/PrintCustomerVehicle" });
   }

   me.BasicModel = function () {
       var lookup = Wx.blookup({
           name: "BasicModelOpen",
           title: "Basic Model",
           manager: MasterService,
           query: "BasicModelOpen",
           defaultSort: "BasicModel",
           columns: [
               { field: "BasicModel", title: "Basic Model" },
               { field: "TechnicalModelCode", title: "Technical Model" },
               { field: "ModelDescription", title: "Keterangan" },
               { field: "Status", title: "Status" },
           ]
       });
       lookup.dblClick(function (data) {
           if (data != null) {
               me.data.BasicModel = data.BasicModel;
               me.data.TechnicalModelCode = data.TechnicalModelCode;
               me.Apply();
           }
       });
   }

   $("[name='BasicModel']").on('blur', function () {
       if (me.data.BasicModel != undefined) {
           $http.post('sv.api/customervehicle/BasicModel', me.data).
              success(function (data, status, headers, config) {
                  if (data != "") {
                      me.data.BasicModel = data[0].BasicModel;
                      me.data.TechnicalModelCode = data[0].TechnicalModelCode;
                  }
                  else {
                      me.data.BasicModel = "";
                      me.BasicModel();
                  }
              }).
              error(function (data, status, headers, config) {
                  alert('error');
              });
       }
   });

   me.ColourCode = function () {
       var lookup = Wx.blookup({
           name: "ColourCodeOpen",
           title: "Warna",
           manager: MasterService,
           query: "ColourCodeOpen",
           defaultSort: "RefferenceCode",
           columns: [
               { field: "RefferenceCode", title: "Kode Warna" },
               { field: "RefferenceDesc1", title: "Warna" },
               { field: "RefferenceDesc2", title: "Warna di Faktur Polisi" },
           ]
       });
       lookup.dblClick(function (data) {
           if (data != null) {
               me.data.ColourCode = data.RefferenceCode;
               me.data.ColourName = data.RefferenceDesc1;
               me.Apply();
           }
       });
   }

   $("[name='ColourCode']").on('blur', function () {
       if (me.data.ColourCode != undefined) {
           $http.post('sv.api/customervehicle/ColourCode', me.data).
              success(function (data, status, headers, config) {
                  if (data != "") {
                      me.data.ColourCode = data[0].RefferenceCode;
                      me.data.ColourName = data[0].RefferenceDesc1;
                  }
                  else {
                      me.data.ColourCode = "";
                      me.ColourCode();
                  }
              }).
              error(function (data, status, headers, config) {
                  alert('error');
              });
       }
   });

   me.CustomerCode = function () {
       var lookup = Wx.klookup({
           name: "CustomerCodeOpen",
           title: "Pelanggan",
           url: "sv.api/customervehicle/LookUpCustomer",
           serverBinding: true,
           pageSize: 10,
           columns: [
               { field: "CustomerCode", title: "Kode Pelanggan" },
               { field: "CustomerName", title: "Nama Pelanggan" },
               { field: "Address", title: "Alamat" },
               { field: "Status", title: "Status" },
           ]
       });
       lookup.dblClick(function (data) {
           if (data != null) {
               me.data.CustomerCode = data.CustomerCode;
               me.data.CustomerName = data.CustomerName;
               me.data.ContactName = data.ContactName;
               me.data.ContactAddress = data.ContactAddress;
               me.data.ContactPhone = data.ContactPhone;
               me.Apply();
           }
       });
   }

   $("[name='CustomerCode']").on('blur', function () {
       if (me.data.CustomerCode != undefined) {
           $http.post('sv.api/customervehicle/CustomerCode', me.data).
              success(function (data, status, headers, config) {
                  if (data != "") {
                      me.data.CustomerCode = data[0].CustomerCode;
                      me.data.CustomerName = data[0].CustomerName;
                  }
                  else {
                      me.data.CustomerCode = "";
                      me.CustomerCode();
                  }
              }).
              error(function (data, status, headers, config) {
                  alert('error');
              });
       }
   });

   me.DealerCode = function () {
       var lookup = Wx.klookup({
           name: "CustomerCodeOpen",
           title: "Pelanggan",
           url: "sv.api/customervehicle/LookUpCustomer",
           serverBinding: true,
           pageSize: 10,
           columns: [
               { field: "CustomerCode", title: "Kode Dealer" },
               { field: "CustomerName", title: "Nama Dealer" },
               { field: "Address", title: "Alamat" },
               { field: "Status", title: "Status" },
           ]
       });
       lookup.dblClick(function (data) {
           if (data != null) {
               me.data.DealerCode = data.CustomerCode;
               me.data.DealerName = data.CustomerName;
               me.Apply();
           }
       });
   }

   $("[name='DealerCode']").on('blur', function () {
       if (me.data.DealerCode != undefined) {
           $http.post('sv.api/customervehicle/DealerCode', me.data).
              success(function (data, status, headers, config) {
                  if (data != "") {
                      me.data.DealerCode = data.CustomerCode;
                      me.data.DealerName = data.CustomerName;
                  }
                  else {
                      me.data.DealerCode = "";
                      me.DealerCode();
                  }
              }).
              error(function (data, status, headers, config) {
                  alert('error');
              });
       }
   });

   me.initGrid = function () {
       me.grid1 = new webix.ui({
           container: "wxData",
           view: "wxtable", css:"alternating",
           columns: [
               { id: "SeqNo", header: "No." },
               { id: "PreviousData", header: "Data Sebelumnya", fillspace: true },
               { id: "ChangeCode", header: "Kode Perubahan", fillspace: true },
               { id: "LastUpdateBy", header: "Perubahan Oleh", fillspace: true },
               { id: "LastUpdateDate", header: "Tgl. Perubahan", fillspace: true, format: me.dateFormat },

           ]
       });
   }

   me.SavePTS = function () {
       var datDetail = [];
       $.each(me.detail, function (key, val) {
           var arr = {
               "Status": val["Status"],
               "ChassisCode": val["ChassisCode"],
               "ChassisNo": val["ChassisNo"],
               "ServiceBookNoNew": val["ServiceBookNoNew"]
           }
           datDetail.push(arr);
       });

       var dat = {};
       dat["isSPK"] = isSPK
       dat["model"] = datDetail;
       var JSONData = JSON.stringify(dat);

       $http.post('sv.api/customervehicle/SavePacth', JSONData).
       success(function (dl, status, headers, config) {
           if (dl.success) {
               MsgBox("Data Berhasi Di Simpan");
               isSPK = "";
               me.clearTable(me.PTS);
               me.clearTable(me.PTK);
           } else {
               MsgBox(dl.message, MSG_ERROR);
               console.log(dl.error_log);
           }
       }).
       error(function (e, status, headers, config) {
           MsgBox("Connecting server error", MSG_ERROR);
       });
   }


   me.btnPTS = function () {
       $('#pnlPatch').show(); $('#wxPTS').show(); $('#wxPTK').hide(); $('#wxPTC').hide(); $('#btnSavePTS').show(); $('#btnHidePTS').show();
       $http.post('sv.api/customervehicle/GetBookNoNewAndOld?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&isSPK=1').
             success(function (data, status, headers, config) {
                 me.detail = data;
                 me.loadTableData(me.PTS, data);
                 me.Apply();
             }).
             error(function (e, status, headers, config) {
                 console.log(e);
             });
       isSPK = "1";
   }

   me.btnPTK = function () {
       $('#pnlPatch').show(); $('#wxPTS').hide(); $('#wxPTK').show(); $('#wxPTC').hide(); $('#btnSavePTS').show(); $('#btnHidePTS').show();
       $http.post('sv.api/customervehicle/GetBookNoNewAndOld?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&isSPK=0').
             success(function (data, status, headers, config) {
                 me.loadTableData(me.PTK, data);
                 me.Apply();
             }).
             error(function (e, status, headers, config) {
                 console.log(e);
             });
       isSPK = "0";
   }

   me.btnPTC = function () {
       $('#pnlPatch').show(); $('#wxPTS').hide(); $('#wxPTK').show(); $('#wxPTC').hide(); $('#btnSavePTS').show(); $('#btnHidePTS').show();
       $http.post('sv.api/customervehicle/GetBookNoNewAndOld?ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo + '&isSPK=2').
             success(function (data, status, headers, config) {
                 me.loadTableData(me.PTK, data);
                 me.Apply();
             }).
             error(function (e, status, headers, config) {
                 console.log(e);
             });
       isSPK = "2";
   }

   me.HidePTS = function () {
       $('#pnlPatch').hide();
       $('#btnSavePTS').hide(); $('#btnHidePTS').hide();
       $('#wxPTS').hide(); $('#wxPTK').hide(); $('#wxPTC').hide();
   }

   me.PTS = new webix.ui({
       container: "wxPTS",
       view: "wxtable", css:"alternating",
       scrollX: true,
       columns: [
           { id: "Status", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 50 },
           { id: "BranchCode", header: "Branch Code", width: 100 },
           { id: "InvoiceNo", header: "Transaction No", width: 150 },
           { id: "ChassisCode", header: "Chassis Code", width: 150 },
           { id: "ChassisNo", header: "Chassis No", width: 100 },
           { id: "ServiceBookNoOld", header: "Service Book No (Old)", width: 150 },
           { id: "ServiceBookNoNew", header: "Service Book No (New)", width: 150 },
           { id: "EngineCodeOld", header: "Engine Code(Old)", width: 150 },
           { id: "EngineCodeNew", header: "Engine Code(New)", width: 150 },
           { id: "EngineNoOld", header: "Engine No (Old)", width: 150 },
           { id: "EngineNoNew", header: "Engine No (New)", width: 150 },
           { id: "PoliceRegNoOld", header: "PoliceReg No(Old)", width: 150 },
           { id: "PoliceRegNoNew", header: "Police Reg No(New)", width: 150 },
       ]
   });

   me.PTK = new webix.ui({
       container: "wxPTK",
       view: "wxtable", css:"alternating",
       scrollX: true,
       columns: [
           { id: "Status", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 50 },
           { id: "BranchCode", header: "Branch Code", width: 100 },
           { id: "JobOrderNo", header: "Job Order No", width: 150 },
           { id: "ChassisCode", header: "Chassis Code", width: 150 },
           { id: "ChassisNo", header: "Chassis No", width: 100 },
           { id: "ServiceBookNoOld", header: "Service Book No (Old)", width: 150 },
           { id: "ServiceBookNoNew", header: "Service Book No (New)", width: 150 },
           { id: "EngineCodeOld", header: "Engine Code(Old)", width: 150 },
           { id: "EngineCodeNew", header: "Engine Code(New)", width: 150 },
           { id: "EngineNoOld", header: "Engine No (Old)", width: 150 },
           { id: "EngineNoNew", header: "Engine No (New)", width: 150 },
       ]
   });

   me.initGrid();
   webix.event(window, "resize", function () {
       me.grid1.adjust();
   })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Kendaraan dan Pelanggan",
        xtype: "panels",
        //toolbars: WxButtons,
        toolbars:[ 
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlRefService",
                //title: "Service Information",
                items: [
                    {
                        text: "Kode & No. Rangka",
                        type: "controls",
                        items: [
                            { name: "ChassisCode", cls: "span4", text: "Chassis Code", type: "popup", btnName: "btnChassisCode", required: true, validasi: "required", click: "ChassisCode()", maxlength: 15 },
                            { name: "ChassisNo", cls: "span4", text: "Chassis No", required: true, validasi: "required", maxlength: 10 },
                        ]
                    },
                    {
                        text: "Kode & No. Mesin",
                        type: "controls",
                        items: [
                            { name: "EngineCode", cls: "span4", text: "Engine Code", required: true, validasi: "required", maxlength: 15 },
                            { name: "EngineNo", cls: "span4", text: "Engine No", required: true, validasi: "required" },
                        ]
                    },
                    {
                        text: "Basic & Technical Model",
                        type: "controls",
                        items: [
                            { name: "BasicModel", cls: "span4", text: "Basic Model", type: "popup", btnName: "btnBasicModel", required: true, validasi: "required", click: "BasicModel()" },
                            { name: "TechnicalModelCode", cls: "span4", text: "Technical Model Code", required: true, validasi: "required" },
                        ]
                    },
                    { name: "ProductionYear", text: "Tahun Produksi", cls: "span4", type: "select2", datasource: "Year", required: true, validasi: "required" },
                    { name: "ServiceBookNo", text: "No Buku Service", cls: "span4", required: true, validasi: "required", maxlength: 15 },
                    {
                        name: "TransmissionType", text: "Tipe Transmisi", type: "select", cls: "span4",
                        items: [
                            { value: 'AT', text: 'Automatic Transmission' },
                            { value: 'MT', text: 'Manual Transmission' },
                        ]
                    },
                    {
                        text: "Color",
                        type: "controls",
                        items: [
                            { name: "ColourCode", cls: "span4", text: "Warna", type: "popup", btnName: "btnColourCode", required: true, validasi: "required", click: "ColourCode()" },
                            { name: "ColourName", cls: "span4", text: "Description", readonly: true },
                        ]
                    },
                    { name: "PoliceRegNo", text: "No Polisi", cls: "span4", required: true, validasi: "required", maxlength: 15 },
                    { name: "FakturPolisiDate", text: "Tgl. Faktur Polisi", cls: "span4", type: "ng-datepicker" },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span4", text: "Kode Pelanggan", type: "popup", btnName: "btnCustomerCode", required: true, validasi: "required", click: "CustomerCode()" },
                            { name: "CustomerName", cls: "span4", text: "Nama Pelanggan", readonly: true },
                        ]
                    },
                    {
                        text: "Dealer",
                        type: "controls",
                        items: [
                            { name: "DealerCode", cls: "span4", text: "Kode Dealer", type: "popup", btnName: "btnDealerCode", click: "DealerCode()" },
                            { name: "DealerName", cls: "span4", text: "Nama Dealer", readonly: true },
                        ]
                    },
                    {
                         type: "controls",
                         cls: "span4 full",
                         text: "Status",
                         items: [
                             { name: "IsActive", cls: "span1", type: "ng-check" },
                             { type: "label", text: "Aktif", cls: "span7 mylabel" },
                         ]
                     },

                ]
            },
            {
                name: "pnlinfopel",
                //cls: "hide",
                title: "Tambahan Informasi yang bisa dihubungi",
                items: [
                    { name: "ContactName", text: "Nama", required: true, validasi: "required", maxlength: 50 },
                    { name: "ContactAddress", text: "Alamat", type: "textarea", required: true, validasi: "required", maxlength: 250 },
                    { name: "ContactPhone", text: "No. Telepon", required: true, validasi: "required", maxlength: 250 },
                ]
            },

            {
                name: "wxData",
                //cls: "hide",
                xtype: "wxtable",
                tblname: "tblPart",
            },
            {
                name: "pnlinfopel",
                items: [

                    {
                        type: "buttons",
                        items: [
                            { name: "btnPTS", text: "Patch To Service", icon: "icon-gear", cls: "btn btn-info", click: "btnPTS()" },
                            { name: "btnPTK", text: "Patch To KSG", icon: "icon-gear", cls: "btn btn-info", click: "btnPTK()" },
                            { name: "btnPTC", text: "Patch To Clain", icon: "icon-gear", cls: "btn btn-info", click: "btnPTC()" },
                        ]
                    },

                ]
            },
            {
                name: "pnlPatch",
                title: "Update Service Book No",
                items: [
                    {
                        type: "buttons",
                        items: [
                            { name: "btnSavePTS", text: "Save", cls: "btn btn-success", icon: "icon-save", click: "SavePTS()" },
                            { name: "btnHidePTS", text: "Hide", cls: "btn btn-warning", icon: "icon-remove", click: "HidePTS()" },
                        ]
                    },
                ]
            },
            {
                name: "wxPTS",
                xtype: "wxtable",
            },
            {
                name: "wxPTK",
                xtype: "wxtable",
            },
            {
                name: "wxPTC",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("CustomerVehicleController");
    }

});