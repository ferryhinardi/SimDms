"use strict"
var status = 0;
var pType = "";
var lAllCbo = 0;

function omPermohonanFakturPolisController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('om.api/Combo/LoadComboData?CodeId=FPCT').
    success(function (data, status, headers, config) {
        me.DealerCategory = data;
    });

    $http.post('om.api/Combo/ReffCombo?RefferenceType=RSNF').
    success(function (data, status, headers, config) {
        me.ReasonCode = data;
    });

    me.JenisKelamin = [
       { "value": '0', "text": 'LAKI-LAKI' },
       { "value": '1', "text": 'PEREMPUAN' }
    ];

    me.getProductType = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'om.api/PermohonanFakturPolis/getProductType',
            success: function (dt) {
                if (dt.success) {
                    pType = dt.data;
                }
            }
        });
    }

    me.loadAllCombo = function () {
        $http.post('om.api/Combo/loadAllCombo').
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.cboJenisKelamin = data.jkl;
                        me.cboTmpPembelian = data.tps;
                        me.cboKendaraanDimiliki = data.spd;
                        me.cboSumberPembelian = data.sup;
                        me.cboAsalPembelian = data.asn;
                        me.cboInfoSuzuki = data.sri;
                        me.cboFaktorMemilih = data.fpg;
                        me.cboPendidikanAkhir = data.pdk;
                        me.cboPenghasilan = data.hsl;
                        me.cboPekerjaan = data.pek;
                        me.cboPenggunaan = data.use;
                        me.cboCaraBeli = data.cpb;
                        me.cboLeasing = data.lsg;
                        me.cboJangkaWaktu = data.jwk;
                    }
                })
                .error(function (e) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });

    }

    me.SubDealerCode = function () {
        var lookup = Wx.blookup({
            name: "SubDealerLookup",
            title: "Customer",
            manager: spSalesManager,
            query: "SubDealerLookup",
            defaultSort: "CustomerCode asc",
            columns: [
                { field: "CustomerCode", title: "Kode Customer" },
                { field: "CustomerName", title: "Nama Customer" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.SubDealerCode = data.CustomerCode;
            me.data.CustomerName = data.CustomerName;
            $('#SubDealerCode').attr('disabled', 'disabled');
            me.Apply();
        });
    }

    $("[name='SubDealerCode']").on('blur', function () {
        if (me.data.SubDealerCode != null) {
            $http.post('om.api/PermohonanFakturPolis/SubDealerCode', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       if (data.data != "") {
                           me.data.SubDealerCode = data.data[0].CustomerCode;
                           me.data.CustomerName = data.data[0].CustomerName;
                           $('#SubDealerCode').attr('disabled', 'disabled');
                       }
                       else {
                           me.data.SubDealerCode = "";
                           me.data.CustomerName = "";
                           me.SubDealerCode();
                       }
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    me.ChassisCode = function () {
        var lookup = Wx.blookup({
            name: "ChassisNoSP",
            title: "Rangka",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('ChassisNoSP').withParameters({ CustomerCode: me.data.SubDealerCode, isCBU: me.data.isCBU }),
            defaultSort: "ChassisCode asc",
            columns: [
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "chassisNo", title: "No Rangka" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.ChassisCode = data.ChassisCode;
            me.detail.ChassisNo = data.chassisNo;
            if ((me.detail.ChassisCode != null || me.detail.ChassisCode != undefined) && (me.detail.ChassisCode != null || me.detail.ChassisCode != undefined)) {
                console.log("Load...");
                me.loadFPDetail();
            } else {
                MsgBox("Detail alamat tidak ditemukan!", MSG_INFO);
            }
            me.Apply();
            //me.detail.SONo = data.SONo;
            //me.detail.BPKNo = data.BPKNo;

            //me.detail.SKPKName = data.EndUserName;
            //me.detail.SKPKAddress1 = data.EndUserAddress1;
            //me.detail.SKPKAddress2 = data.EndUserAddress2;
            //me.detail.SKPKAddress3 = data.EndUserAddress3;
            //me.detail.SKPKCity = data.CityCode;
            //me.detail.SKPKCityName = data.CityName;
            //me.detail.SalesmanCode = data.Salesman;
            //me.detail.SalesmanName = data.SalesmanName;
            //me.detail.SKPKTelp1 = data.PhoneNo;
            //me.detail.SKPKHP = data.HPNo;
            //me.detail.SKPKBirthday = data.birthDate;
            //me.detail.FakturPolisiName = data.CustomerName;
            //me.detail.FakturPolisiAddress1 = data.Address1;
            //me.detail.FakturPolisiAddress2 = data.Address2;
            //me.detail.FakturPolisiAddress3 = data.Address3;
            //me.detail.FakturPolisiCity = data.CityCode;
            //me.detail.FakturPolisiCityName = data.CityName;
        });
    }

    me.getLocalBranch = function () {
        $http.post('om.api/PermohonanFakturPolis/loadFPDetail', { vChassisCode: me.detail.ChassisCode, vChassisNo: me.detail.ChassisNo }).
               success(function (dt, status, headers, config) {
                   if (dt.success) {
                       me.detail.LBranch = dt.data.BranchCode;
                       console.log(me.detail.LBranch);
                   }
                   else {
                       MsgBox(dt.message, MSG_INFO);
                   }
               }).
               error(function (dt, status, headers, config) {
                   alert('error');
               });
    }

    me.loadFPDetail = function () {
        $http.post('om.api/PermohonanFakturPolis/loadFPDetail', { vChassisCode: me.detail.ChassisCode, vChassisNo: me.detail.ChassisNo }).
               success(function (dt, status, headers, config) {
                   if (dt.success) {
                       me.detail.SONo = dt.data.SONo;
                       me.detail.BPKNo = dt.data.BPKNo;
                       me.detail.SKPKNo = dt.data.SKPKNo;
                       me.detail.SKPKName = dt.data.EndUserName;
                       me.detail.SKPKAddress1 = dt.data.EndUserAddress1;
                       me.detail.SKPKAddress2 = dt.data.EndUserAddress2;
                       me.detail.SKPKAddress3 = dt.data.EndUserAddress3;
                       me.detail.SKPKCity = dt.data.CityCode;
                       me.detail.SKPKCityName = dt.data.CityName;
                       me.detail.SalesmanCode = dt.data.Salesman;
                       me.detail.SalesmanName = dt.data.SalesmanName;
                       me.detail.SKPKTelp1 = dt.data.PhoneNo;
                       me.detail.SKPKHP = dt.data.HPNo;
                       me.detail.SKPKBirthday = dt.data.birthDate;
                       me.detail.FakturPolisiName = dt.data.CustomerName;
                       me.detail.FakturPolisiAddress1 = dt.data.Address1;
                       me.detail.FakturPolisiAddress2 = dt.data.Address2;
                       me.detail.FakturPolisiAddress3 = dt.data.Address3;
                       me.detail.FakturPolisiCity = dt.data.CityCode;
                       me.detail.FakturPolisiCityName = dt.data.CityName;
                       me.detail.LBranch = dt.data.BranchCode;



                       console.log(me.detail.LBranch);
                   }
                   else {
                       MsgBox(dt.message, MSG_INFO);
                   }
               }).
               error(function (dt, status, headers, config) {
                   alert('error');
               });
    }

    me.ChassisNo = function () {
        var lookup = Wx.blookup({
            name: "ChassisNoSP",
            title: "Rangka",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('ChassisNoSP').withParameters({ CustomerCode: me.data.SubDealerCode, isCBU: me.data.isCBU }),
            defaultSort: "ChassisCode asc",
            columns: [
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "chassisNo", title: "No Rangka" },
                { field: "salesModelCode", title: "Model" },
                { field: "salesModelYear", title: "Year" },
                { field: "BranchCode", title: "Cabang" },
                { field: "CustomerCode", title: "Pelanggan" },
                { field: "CustomerName", title: "Nama Pelanggan" }
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.ChassisCode = data.ChassisCode;
            me.detail.ChassisNo = data.chassisNo;
            if ((me.detail.ChassisCode != null || me.detail.ChassisCode != undefined) && (me.detail.ChassisCode != null || me.detail.ChassisCode != undefined)) {
                me.loadFPDetail();
            } else {
                MsgBox("Detail alamat tidak ditemukan!", MSG_INFO);
            }
            me.Apply();

            //me.detail.SONo = data.SONo;
            //me.detail.BPKNo = data.BPKNo;
            //me.detail.ChassisCode = data.ChassisCode;
            //me.detail.ChassisNo = data.chassisNo;
            //me.detail.SKPKName = data.EndUserName;
            //me.detail.SKPKAddress1 = data.EndUserAddress1;
            //me.detail.SKPKAddress2 = data.EndUserAddress2;
            //me.detail.SKPKAddress3 = data.EndUserAddress3;
            //me.detail.SKPKCity = data.CityCode;
            //me.detail.SKPKCityName = data.CityName;
            //me.detail.SalesmanCode = data.Salesman;
            //me.detail.SalesmanName = data.SalesmanName;
            //me.detail.SKPKTelp1 = data.PhoneNo;
            //me.detail.SKPKHP = data.HPNo;
            //me.detail.SKPKBirthday = data.birthDate;
            //me.detail.FakturPolisiName = data.CustomerName;
            //me.detail.FakturPolisiAddress1 = data.Address1;
            //me.detail.FakturPolisiAddress2 = data.Address2;
            //me.detail.FakturPolisiAddress3 = data.Address3;
            //me.detail.FakturPolisiCity = data.CityCode;
            //me.detail.FakturPolisiCityName = data.CityName;

            //me.Apply();
        });
    }

    me.SKPKCity = function () {
        var lookup = Wx.blookup({
            name: "CityCodeLookup",
            title: "City",
            manager: spSalesManager,
            query: "CityCodeLookup",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Kode Kota" },
                { field: "LookUpValueName", title: "Nama Kota" }
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.SKPKCity = data.LookUpValue;
            me.detail.SKPKCityName = data.LookUpValueName;
            me.Apply();
        });
    }

    me.FakturPolisiCity = function () {
        var lookup = Wx.blookup({
            name: "CityCodeLookup",
            title: "City",
            manager: spSalesManager,
            query: "CityCodeLookup",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Kode Kota" },
                { field: "LookUpValueName", title: "Nama Kota" }
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.FakturPolisiCity = data.LookUpValue;
            me.detail.FakturPolisiCityName = data.LookUpValueName;
            me.Apply();
        });
    }

    $("[name='SKPKCity']").on('blur', function () {
        if (me.detail.SKPKCity != null) {
            $http.post('om.api/PermohonanFakturPolis/SKPKCity', me.detail).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.detail.SKPKCityName = data.data.LookUpValueName;
                   }
                   else {
                       me.detail.SKPKCity = "";
                       me.detail.SKPKCityName = "";
                       me.SKPKCity();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='FakturPolisiCity']").on('blur', function () {
        if (me.detail.FakturPolisiCity != null) {
            $http.post('om.api/PermohonanFakturPolis/FakturPolisiCity', me.detail).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.detail.FakturPolisiCityName = data.data.LookUpValueName;
                   }
                   else {
                       me.detail.FakturPolisiCity = "";
                       me.detail.FakturPolisiCityName = "";
                       me.FakturPolisiCity();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    me.FakturPolisiNo = function () {
        var lookup = Wx.blookup({
            name: "NoFakturPolis",
            title: "No Faktur",
            manager: spSalesManager,
            query: new breeze.EntityQuery.from('NoFakturPolis').withParameters({ ChassisCode: me.detail.ChassisCode, ChassisNo: me.detail.ChassisNo }),
            defaultSort: "FakturPolisiNo asc",
            columns: [
                { field: "FakturPolisiNo", title: "No Faktur" },
                { field: "statusBlanko", title: "Status Blanko" }
            ]
        });
        lookup.dblClick(function (data) {
            me.detail.FakturPolisiNo = data.FakturPolisiNo;
            me.detail.StatusBlanko = data.statusBlanko;
            me.Apply();
        });
    }

    me.SalesmanCode = function () {
        var lookup = Wx.blookup({
            name: "SalesmanIDLookup",
            title: "Salesman ID",
            manager: spSalesManager,
            query: "SalesmanIDLookup",
            defaultSort: "EmployeeID asc",
            columns: [
                { field: "EmployeeID", title: "Employee ID" },
                { field: "EmployeeName", title: "Employee Name" },
                { field: "LookUpValueName", title: "Jabatan" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.SalesmanCode = data.EmployeeID;
                me.detail.SalesmanName = data.EmployeeName;
                me.Apply();
            }
        });

    }

    $("[name='SalesmanCode']").on('blur', function () {
        if (me.detail.SalesmanCode != null) {
            $http.post('om.api/PermohonanFakturPolis/SalesmanCode', me.detail).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.detail.SalesmanName = data.data.EmployeeName;
                   }
                   else {
                       me.detail.SalesmanCode = "";
                       me.detail.SalesmanName = "";
                       me.SalesmanCode();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    me.PostalCode = function () {
        var lookup = Wx.blookup({
            name: "ZipCodeLookUp",
            title: "Kode Pos",
            manager: spSalesManager,
            query: "ZipCodeLookUp",
            defaultSort: "ZipCode asc",
            columns: [
                { field: "ZipCode", title: "Kode Pos" },
                { field: "KelurahanDesa", title: "Kelurahan" },
                { field: "KecamatanDistrik", title: "Kecamatan" },
                { field: "KotaKabupaten", title: "Kabupaten" },
                { field: "IbuKota", title: "Kota" },
                { field: "isCity", title: "isCity" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.PostalCode = data.ZipCode;
                me.detail.PostalCodeDesc = data.KelurahanDesa;
                me.Apply();
            }
        });
    }

    me.approve = function (e, param) {
        $http.post('om.api/PermohonanFakturPolis/Approve', { model: me.data, pType: pType }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    $('#Status').html(data.status);
                    $('#btnApprove').attr('disabled', 'disabled');
                    status = data.Result;
                    me.isStatus = status == 2;
                    Wx.Success("Data approved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_INFO);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.printPreview = function () {
        $http.post('om.api/PermohonanFakturPolis/preprint', me.data)
       .success(function (e) {
           if (e.success) {
               $('#Status').html(e.Status);
               if (e.stat == "1") { $('#btnApprove').removeAttr('disabled'); }
               BootstrapDialog.show({
                   message: $(
                       '<div class="container">' +
                       '<div class="row">' +

                       '<input type="radio" name="PrintType" id="PrintType1" value="0" checked>&nbsp Print Faktur Polisi</div>' +

                       '<div class="row">' +

                       '<input type="radio" name="PrintType" id="PrintType2" value="1">&nbsp Print Blanko</div>' +

                       '<div class="row">' +

                       '<input type="radio" name="PrintType" id="PrintType3" value="2">&nbsp Print Sertifikat</div>'),
                   closable: false,
                   draggable: true,
                   type: BootstrapDialog.TYPE_INFO,
                   title: 'Print',
                   buttons: [{
                       label: ' Print',
                       cssClass: 'btn-primary icon-print',
                       action: function (dialogRef) {
                           me.Print();
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
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.Print = function () {
        if ($('input[name=PrintType]:checked').val() === '0') {
            var ReportId = 'OmRpSalesTrn007DNewWeb';
        }
        else if ($('input[name=PrintType]:checked').val() === '1') {
            //var ReportId = 'OmRpSalesTrn007BWeb';
            var ReportId = 'OmRpSalesTrn007CWeb';
        }
        else {
            //var ReportId = 'OmRpSalesTrn007CWeb';
            var ReportId = 'OmRpSalesTrn007BWeb';
        }

        var par = me.data.ReqNo + ',' + me.data.ReqNo;
        //var par = [
        //    'CompanyCode',
        //    me.detail.LBranch,
        //    me.data.ReqNo,
        //    me.data.ReqNo
        //];
        //var rparam = 'Print Permohonan Faktur Polisi'
        var ccity = "";
        $http.post('om.api/PermohonanFakturPolis/FakturPolisiCity2')
           .success(function (e) {
               if (e.success) {
                   ccity = e.data;
               } else {
                   ccity = "";
               }
               var rparam = ccity + ' ' + moment(me.data.ReqDate).format('DD MMM YYYY');

               console.log(rparam);

               Wx.showPdfReport({
                   id: ReportId,
                   pparam: par,
                   textprint: true,
                   rparam: rparam,
                   type: "devex"
               });
           })
           .error(function (e) {
               MsgBox('Gagal ambil data kota!', MSG_ERROR);
           });
    }

    me.saveData = function () {
        $http.post('om.api/PermohonanFakturPolis/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    //if (me.data.ReqNo == null) {
                    //    me.data.ReqNo = data.data.ReqNo;
                    //    me.saveData();
                    //}
                    me.data.ReqNo = data.data.ReqNo;
                    $('#Status').html(data.status);
                    $('#pnlKendaraan').show();
                    $('#wxkendaraan').show();
                    Wx.Success("Data saved...");
                    me.startEditing();
                    me.griddetail.adjust();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.AddDetail = function (e, param) {
        if (me.detail.SKPKNo == undefined || me.detail.DealerCategory == undefined || me.detail.PostalCode == undefined
            || me.detail.FakturPolisiTelp1 == undefined || me.detail.FakturPolisiHP == undefined || me.detail.IDNo == undefined) {
            MsgBox("Data Detail Belum Lengkap!", MSG_INFO);
        }
        else {
            $http.post('om.api/PermohonanFakturPolis/SaveDetail', { model: me.data, detailModel: me.detail }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.clearTable(me.griddetail)
                        me.detail = {};
                        $('#btnAddDetail').show();
                        $('#btnUpdateDetail').hide();
                        $('#btnDeleteDetail').hide();
                        me.Apply();
                        $http.post('om.api/PermohonanFakturPolis/DetailSalesReq', { ReqNo: me.data.ReqNo })
                            .success(function (e) {
                                if (e.success) {
                                    me.loadTableData(me.griddetail, e.grid);
                                    me.griddetail.adjust();
                                }
                                else {
                                    MsgBox(e.message, MSG_ERROR);
                                }
                            })
                            .error(function (e) {
                                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                            });
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
        }
    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/PermohonanFakturPolis/Delete', { model: me.data }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        $('#Status').html(data.Status);
                        $('#pnlKendaraan').hide();
                        $('#wxkendaraan').hide();
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

    me.DeleteDetail = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/PermohonanFakturPolis/DeleteDetail', { model: me.data, detailModel: me.detail }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.detail = {};
                        me.clearTable(me.griddetail);
                        Wx.Info("Record has been deleted...");
                        me.grid.detail = data.grid;
                        me.loadTableData(me.griddetail, me.grid.detail);
                        me.detail = {};
                        $('#btnAddDetail').show();
                        $('#btnUpdateDetail').hide();
                        $('#btnDeleteDetail').hide();
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

    me.browse = function () {
        me.cancelOrClose();
        var lookup = Wx.blookup({
            name: "PermohonanBrowse",
            title: "Permohonan Faktur terima",
            manager: spSalesManager,
            query: "PermohonanBrowse",
            defaultSort: "ReqNo desc",
            columns: [
                { field: "ReqNo", title: "No. Permohonan" },
                {
                    field: "ReqDate", title: "Tgl",
                    template: "#= (ReqDate == undefined) ? '' : moment(ReqDate).format('DD MMM YYYY') #"
                },
                { field: "Faktur", title: "Status Faktur" },
                { field: "SubDealerCode", title: "Sub Dealer" },
                { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.status = data.Status;
                $('#Status').html(data.Status);
                me.lookupAfterSelect(data);
                //if (data.StatusFaktur === "1") {
                //    $('#StatusFaktur').prop('checked', true);
                //    me.Apply();
                //}
                data.StatusFaktur === "1" ? $('#StatusFaktur').prop('checked', true) : $('#StatusFaktur').prop('checked', false);
                me.loadDetail(data);
                //console.log(data);
                if (me.data.ReffDate == null) {
                    me.data.isActive1 = false;
                }
                else {
                    me.data.isActive1 = true;
                }
                if (data.isCBU == false) {
                    me.data.isCBU = "false";
                }
                else {
                    me.data.isCBU = "true";
                }
                me.isPrintAvailable = true;
                status = data.Stat;
                me.isStatus = status == 2;
                //console.log("Data Status: " + data.Stat);
                //console.log("is Status: " + me.isStatus);
                //console.log("Print: " + me.isPrintAvailable);
                //console.log("Data Load: " + me.isLoadData);

                switch (data.Stat) {
                    case "1":
                        me.isApprove = false;
                        //me.isLoadData = true;
                        $('#btnApprove').removeAttr('disabled');
                        $('#btnDelete').hide();
                        $('#btnCancel').show();
                        $('#btnAddDetail').show();
                        me.showDetails();
                        break;
                    case "2": //approve
                        //me.isApprove = false;
                        //me.isLoadData = true;
                        $('#btnDelete').hide();
                        $('#btnCancel').show();
                        $('#btnAddDetail').show();
                        me.disabledApproved();
                        me.isLoadData = true;
                        me.isPrintAvailable = true;
                        me.Apply();
                        me.showDetails();
                        break;
                    case "3":
                        $('#Remark').attr('disabled', true);
                        break;
                    default:
                        me.isApprove = false;
                        //me.isLoadData = true;
                        //$('#btnApprove').removeAttr('disabled');
                        $('#btnDelete').show();
                        $('#btnCancel').show();
                        $('#btnAddDetail').show();
                        me.showDetails();
                }
                me.Apply();
                //if (pType == "4W")
                //{
                //    $("[data-name='tabPageCustomer']").hide();
                //    $('#pnlDetailCustomer').hide();
                //}
            }
        });
    };

    me.disabledApproved = function () {
        $('#btnAddDetail').attr('disabled', true);
        $('#btnCancelDetail').attr('disabled', true);
        $("[ng-model='data.isCBU']").attr('disabled', true);
        $('#ReqDate').attr('disabled', true);
        $('#isActive1').attr('disabled', true);
        $('#StatusFaktur').attr('disabled', true);
    }

    me.loadDetail = function (data) {
        $http.post('om.api/PermohonanFakturPolis/DetailSalesReq', data)
               .success(function (e) {
                   if (e.success) {
                       me.loadTableData(me.griddetail, e.grid);
                       me.griddetail.adjust();
                       //$http.post('om.api/PermohonanFakturPolis/loadFPDetail', { vChassisCode: e.grid[0].ChassisCode, vChassisNo: e.grid[0].ChassisNo }).
                       // success(function (dt, status, headers, config) {
                       //    if (dt.success) {
                       //        me.detail.LBranch = dt.data.BranchCode;
                       //        console.log(me.detail.LBranch);
                       //    }
                       //    else {
                       //        MsgBox(dt.message, MSG_INFO);
                       //    }
                       //}).
                       //error(function (dt, status, headers, config) {
                       //    alert('error');
                       //});


                       //console.log("Chassis No: " + me.detail.ChassisNo);
                       //me.getLocalBranch();
                   }
                   else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });
    }

    me.griddetail = new webix.ui({
        container: "wxkendaraan",
        view: "wxtable", css: "alternating",
        scrollX: true,
        columns: [
            { id: "SONo", header: "No.SO", width: 200 },
            { id: "ChassisCode", header: "Kode.Rangka", width: 200 },
            { id: "ChassisNo", header: "No. Rangka", width: 200 },
            { id: "FakturPolisiNo", header: "No Faktur Polis", width: 200 },
            { id: "DealerCategoryDesc", header: "Kategori Dealer", width: 200 },
            { id: "SalesmanName", header: "Salesman", width: 200 },
            { id: "SKPKNo", header: "SKPK No", width: 200 },
            { id: "SKPKName", header: "SKPK Nama", width: 200 },
            { id: "SKPKAddress1", header: "SKPK Alamat 1", width: 200 },
            { id: "SKPKAddress2", header: "SKPK Alamat 2", width: 200 },
            { id: "SKPKAddress3", header: "SKPK Alamat 3", width: 200 },
            { id: "SKPKTelp1", header: "SKPK Telp 1", width: 200 },
            { id: "SKPKTelp2", header: "SKPK Telp 2", width: 200 },
            { id: "SKPKHP", header: "SKPK Telp HP", width: 200 },
            { id: "SKPKCity", header: "SKPK Kota", width: 200 },
            { id: "SKPKBirthday", header: "SKPK Tgl Lahir", width: 200 },
            { id: "FakturPolisiName", header: "Faktur Atas Nama", width: 200 },
            { id: "FakturPolisiAddress1", header: "Faktur Alamat 1", width: 200 },
            { id: "FakturPolisiAddress2", header: "Faktur Alamat 2", width: 200 },
            { id: "FakturPolisiAddress3", header: "Faktur Alamat 3", width: 200 },
            { id: "FakturPolisiTelp1", header: "Faktur Telp 1", width: 200 },
            { id: "FakturPolisiTelp2", header: "Faktur Telp 2", width: 200 },
            { id: "FakturPolisiHP", header: "Faktur Telp HP", width: 200 },
            { id: "PostalCode", header: "Faktur Kode Pos", width: 200 },
            { id: "PostalCodeDesc", header: "Faktur Nama kode", width: 200 },
            { id: "FakturPolisiCity", header: "Faktur Kota", width: 200 },
            { id: "FakturPolisiBirthday", header: "Faktur Tgl Lahir", width: 200 },
            { id: "IsCityTransport", header: "Faktur Untuk Angkot", width: 200 },
            { id: "ReasonCode", header: "Reason Code", width: 200 },
            { id: "ReasonDesc", header: "Reason Code Desc", width: 200 },
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetail.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.griddetail.getSelectedId().id);
                    switch (status) {
                        case "1":
                            $('#btnCancelDetail').removeAttr('disabled');
                            $('#btnUpdateDetail').show();
                            $('#btnDeleteDetail').show();
                            $('#btnAddDetail').hide();
                            break;
                        case "2":
                            $('#btnAddDetail').attr('disabled', true);
                            $('#btnCancelDetail').attr('disabled', true);
                            break;
                        case "3":
                            break;
                        default:
                            $('#btnCancelDetail').removeAttr('disabled');
                            $('#btnUpdateDetail').show();
                            $('#btnDeleteDetail').show();
                            $('#btnAddDetail').hide();
                    }
                    me.detail.oid = me.griddetail.getSelectedId();
                    //console.log(pType);
                    if (pType == "2W") {
                        $("[data-name='tabPageCustomer']").show();
                        $('#pnlDetailCustomer').show();
                        //$("[data-name='tabPageCustomer']").click();
                    }
                    me.Apply();
                }
            }
        }
    });

    me.CancelDetail = function () {
        me.detail = {};
        me.detail.FakturPolisiDate = me.now();
        me.detail.SKPKBirthday = me.now();
        me.detail.FakturPolisiBirthday = me.now();
        me.griddetail.clearSelection();
        $('#btnAddDetail').show();
        $('#btnUpdateDetail').hide();
        $('#btnDeleteDetail').hide();
    }

    $("[name = 'StatusFaktur']").on('change', function () {
        me.data.StatusFaktur = $('#StatusFaktur').prop('checked');
        me.Apply();
        console.log(me.data.StatusFaktur);
    });

    $("[name = 'isActive1']").on('change', function () {
        me.data.isActive1 = $('#isActive1').prop('checked');
        me.Apply();
        console.log(me.data.isActive1);
    });


    me.lkpTipeKendaraan = function () {
        var lookup = Wx.blookup({
            name: "LkpTipeKendaraan",
            title: "Tipe Kendaraan",
            manager: spSalesManager,
            query: new breeze.EntityQuery().from("LkpTipeKendaraan").withParameters({ reff: $('#KendaraanDimiliki').val() }),
            defaultSort: "typeName asc",
            columns: [
                { field: "typeID", title: "Kode Tipe" },
                { field: "typeName", title: "Tipe Kendaraan" },
            ]
        });
        lookup.dblClick(function (data) {
            me.detail2.ModelYgPernahDimiliki = data.typeID;
            me.detail2.ModelYgPernahDimilikiDesc = data.typeName;
            me.Apply();
        });
    }

    me.tabCustomer = function () {
        if (me.detail.ChassisCode != "" && me.detail.ChassisNo != "") {
            $http.post('om.api/PermohonanFakturPolis/getDataAngket', { ReqNo: me.data.ReqNo, ChassisCode: me.detail.ChassisCode, ChassisNo: me.detail.ChassisNo })
              .success(function (data, status, headers, config) {
                  if (data.success) {
                      me.detail2 = data.data;
                      me.detail2.ModelYgPernahDimilikiDesc = data.data2;
                      $('#btnClearDetail2').show();
                  } else {
                      me.detail2 = {}
                      me.detail2.ChassisCode = me.detail.ChassisCode;
                      me.detail2.ChassisNo = me.detail.ChassisNo;
                      $('#btnClearDetail2').hide();
                  }
              }).
                error(function (data, status, headers, config) {
                    MsgBox('Proses simpan angket gagal!, Hubungi SDMS Support', MSG_ERROR);
                });
        } else {
            return MsgBox("Chassis Code dan Chassis No harus diisi!", MSG_INFO);
        }
    }

    me.tabPageKendaraan = function () {
        me.griddetail.adjust();
    }

    me.deleteAngket = function () {
        if (confirm('Anda ingin hapus data angket ini?')) {
            $http.post('om.api/PermohonanFakturPolis/deleteAngket', { ReqNo: me.data.ReqNo, ChassisCode: me.detail2.ChassisCode, ChassisNo: me.detail2.ChassisNo })
              .success(function (data, status, headers, config) {
                  if (data.success) {
                      Wx.Success(data.message);
                      me.detail2 = {};
                      $('#btnClearDetail2').hide();
                  }
              }).
                error(function (data, status, headers, config) {
                    MsgBox('Proses delete angket gagal!, Hubungi SDMS Support', MSG_ERROR);
                });
        }
    }

    me.saveAngket = function () {
        if (me.detail2.ChassisCode == "" || me.detail2.ChassisCode == undefined) {
            return MsgBox("Chassis Code masih kosong!", MSG_INFO);
        }
        if (me.detail2.ChassisNo == "" || me.detail2.ChassisNo == undefined) {
            return MsgBox("Chassis No masih kosong!", MSG_INFO);
        }
        if (me.detail2.JenisKelamin == "" || me.detail2.JenisKelamin == undefined) {
            return MsgBox("Jenis Kelamin harus diisi!", MSG_INFO);
        }
        if (me.detail2.TempatPembelian == "" || me.detail2.TempatPembelian == undefined) {
            return MsgBox("Tempat Pembelian harus diisi!", MSG_INFO);
        }
        if (me.detail2.KendaraanYgPernahDimiliki == "" || me.detail2.KendaraanYgPernahDimiliki == undefined) {
            return MsgBox("Kendaraan Yang Pernah Dimiliki harus diisi!", MSG_INFO);
        }
        if (me.detail2.SumberPembelian == "" || me.detail2.SumberPembelian == undefined) {
            return MsgBox("Sumber Pembelian harus diisi!", MSG_INFO);
        }
        if (me.detail2.AsalPembelian == "" || me.detail2.AsalPembelian == undefined) {
            return MsgBox("Asal Pembelian harus diisi!", MSG_INFO);
        }
        if (me.detail2.InfoSuzukiDari == "" || me.detail2.InfoSuzukiDari == undefined) {
            return MsgBox("Info Suzuki Dari harus diisi!", MSG_INFO);
        }
        if (me.detail2.FaktorPentingMemilihMotor == "" || me.detail2.FaktorPentingMemilihMotor == undefined) {
            return MsgBox("Faktor Penting Memilih Motor harus diisi!", MSG_INFO);
        }
        if (me.detail2.PendidikanTerakhir == "" || me.detail2.PendidikanTerakhir == undefined) {
            return MsgBox("Pendidikan Terakhir harus diisi!", MSG_INFO);
        }
        if (me.detail2.PenghasilanPerBulan == "" || me.detail2.PenghasilanPerBulan == undefined) {
            return MsgBox("Penghasilan Per Bulan harus diisi!", MSG_INFO);
        }
        if (me.detail2.Pekerjaan == "" || me.detail2.Pekerjaan == undefined) {
            return MsgBox("Pekerjaan harus diisi!", MSG_INFO);
        }
        if (me.detail2.PenggunaanMotor == "" || me.detail2.PenggunaanMotor == undefined) {
            return MsgBox("Penggunaan Motor harus diisi!", MSG_INFO);
        }
        if (me.detail2.CaraPembelian == "" || me.detail2.CaraPembelian == undefined) {
            return MsgBox("cara Pembelian harus diisi!", MSG_INFO);
        }
        if (me.detail2.ModelYgPernahDimiliki == "" || me.detail2.ModelYgPernahDimiliki == undefined) {
            return MsgBox("Model Yang Pernah Dimiliki harus diisi!", MSG_INFO);
        }
        if (me.detail2.CaraPembelian == 'CRD') {
            if (me.detail2.Leasing == "" || me.detail2.Leasing == undefined) {
                return MsgBox("Leasing harus diisi!", MSG_INFO);
            }
            if (me.detail2.JangkaWaktuKredit == "" || me.detail2.JangkaWaktuKredit == undefined) {
                return MsgBox("Jangka waktu harus diisi!", MSG_INFO);
            }
        }

        $http.post('om.api/PermohonanFakturPolis/saveAngket', { ReqNo: me.data.ReqNo, mdl: me.detail2 })
               .success(function (data, status, headers, config) {
                   if (data.success) {
                       Wx.Success(data.message);
                       $('#btnClearDetail2').show();
                   } else {
                       MsgBox(data.message, MSG_INFO);
                   }
               }).
                 error(function (data, status, headers, config) {
                     MsgBox('Proses simpan angket gagal!, Hubungi SDMS Support', MSG_ERROR);
                 });
    }

    me.initialize = function () {
        me.isStatus = false;
        me.detail = {};
        me.detail2 = {};
        me.cboTmpPembelian = {};
        me.cboKendaraanDimiliki = {};
        me.cboSumberPembelian = {};
        me.cboAsalPembelian = {};
        me.cboInfoSuzuki = {};
        me.cboFaktorMemilih = {};
        me.cboPendidikanAkhir = {};
        me.cboPenghasilan = {};
        me.cboPekerjaan = {};
        me.cboPenggunaan = {};
        me.cboCaraBeli = {};
        me.cboLeasing = {};
        me.cboJangkaWaktu = {};

        if (pType == "") {
            me.getProductType();
        }
        me.hideDetails();
        me.data.StatusFaktur = true;

        $http.post('om.api/PermohonanFakturPolis/checkOnMstLookup')
            .success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success(data.message);
                    $('#StatusFaktur').attr('disabled', true);
                } else {
                    $('#StatusFaktur').removeAttr('disabled');
                }
            });

        $('#SubDealerCode').removeAttr('disabled');
        me.status = "NEW";
        $('#Status').html(me.status);
        $('#Status').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });
        $('#btnApprove').attr('disabled', true)
        me.data.ReqDate = me.now();
        me.data.ReffDate = me.now();
        me.detail.FakturPolisiDate = me.now();
        me.detail.SKPKBirthday = me.now();
        me.detail.FakturPolisiBirthday = me.now();

        //if (me.data.isActive1 == true) {
        //    me.data.ReffDate = me.now();
        //}
        //else {
        //    me.data.ReffDate = "";
        //}

        $('#btnUpdateDetail').hide();
        $('#btnDeleteDetail').hide();
        //$('#btnDelete').hide();
        //$('#btnCancel').hide();
        $('#btnClearDetail2').hide();

        me.isPrintAvailable = false;
        //ir me.isApprove = true;
        //me.isCancel = false;
        //me.griddetail.adjust();

        if (pType == "2W") {
            me.loadAllCombo();
        }

        $('#StatusFaktur').prop('checked', true)
        me.data.StatusFaktur = true
        $("label[btn-radio]:contains('CKD')").trigger('click');
        me.data.isCBU = "false"
    }

    me.hideDetails = function () {
        $("[data-id='tabDetail']").hide()
        $("[data-name='tabPageCustomer']").hide();
        $('#pnlKendaraan').hide();
        $('#pnlDetailCustomer').hide()
        $('#wxkendaraan').hide();
    }

    me.showDetails = function () {
        $("[data-id='tabDetail']").show()
        $('#pnlKendaraan').show();
        $('#wxkendaraan_parent').show();
        $('#wxkendaraan').show();
        //$('#pnlDetailCustomer').show();
        //$("[data-name='tabPageCustomer']").show();
        me.griddetail.adjust();
        $("p[data-name='tabPageKendaraan']").addClass('active');
    }

    me.$watch('isLoadData', function (newData, oldData) {
        console.log(me.status);
        console.log(me.isLoadData);

        if (me.status != "NEW" && me.status != undefined) {
            if (!me.isLoadData) {
                me.isLoadData = true;
            }
            else {
                me.isLoadData = true;
            }
        }
        else {
            me.isLoadData = false;
        }
    });

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Permohonan Faktur Polisi (Sub-Dealer)",
        xtype: "panels",
        /*
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "!isApprove", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
                    //{ name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable", click: "printPreview()", disable: "!isPrintEnable" },
                    //{ name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printPreview()", disable: "!isPrintEnable" }
        ],
        */
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlPermohonan",
                items: [
                    { name: "ReqNo", model: "data.ReqNo", text: "No.Permohonan", cls: "span3", placeHolder: "RTS/YY/XXXXXX", disable: true },
                    {
                        type: "controls",
                        text: "Tgl. Permohonan",
                        cls: "span5",
                        items: [
                            { name: "ReqDate", model: "data.ReqDate", type: "ng-datepicker", cls: "span3" },
                            {
                                type: "buttons", cls: "span3 left", items: [
                                    {
                                        name: "btnApprove", text: "Approve", cls: "btn-small btn-info", icon: "icon-ok", click: "approve()",
                                        disable: "data.Stat == 0 || data.Stat == 2 || data.Stat == 3"
                                    }
                                ]
                            },
                            { name: "Status", text: "", cls: "span2 center", readonly: true, type: "label" },
                        ]
                    },
                    { name: "ReffNo", model: "data.ReffNo", text: "No.Reff", cls: "span3", disable: "isStatus" },
                    {
                        text: "Tgl.Reff",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "ReffDate", model: "data.ReffDate", placeHolder: "Tgl. Reff", cls: "span4", type: 'ng-datepicker', disable: "!data.isActive1 || isStatus" },
                            //{ name: "isActive1", model: "data.isActive1", type: 'x-switch', cls: "span4", float: 'left', disable: "isStatus" },
                            { name: "isActive1", model: "data.isActive1", type: "check", cls: "span4", float: 'left', disable: "isStatus" },
                        ]
                    },
                    //{ name: "StatusFaktur", model: "data.StatusFaktur", text: "IsFaktur", type: 'x-switch', cls: "span2", float: 'left', disable: "isStatus" },
                    { name: "StatusFaktur", model: "data.StatusFaktur", text: "IsFaktur", type: 'check', cls: "span2", float: 'left', disable: "isStatus" },
                    {
                        text: "Penjual",
                        type: "controls",
                        cls: "span6",
                        required: true,
                        validasi: "required",
                        items: [
                            { name: "SubDealerCode", model: "data.SubDealerCode", cls: "span2", validasi: "required", disable: "isStatus", type: "popup", click: "SubDealerCode()" },
                            { name: "CustomerName", model: "data.CustomerName", cls: "span6", readonly: true, disable: "isStatus" }
                        ]
                    },
                    { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span8", disable: "isStatus" },
                    {
                        type: "optionbuttons",
                        name: "isCBU",
                        model: "data.isCBU",
                        items: [
                            { name: "true", text: "CBU" },
                            { name: "false", text: "CKD" },
                        ]
                    },
                ]
            },
            {
                xtype: "tabs",
                name: "tabDetail",
                items: [
                    { name: "tabPageKendaraan", text: "Data Kendaraan", cls: "active", click: "tabKendaraan()" },
                    { name: "tabPageCustomer", text: "Detail Customer", click: "tabCustomer()" },
                ],
            },
            //{
            //    name: "tabPageKendaraan",
            //    cls: "tabDetail tabPageKendaraan",
            //}
            {
                name: "pnlKendaraan",
                title: "Kendaraan",
                cls: "tabDetail tabPageKendaraan",
                items: [
                    { name: "SONo", model: "detail.SONo", text: "No.SO", cls: "span3", disable: "isStatus", readonly: true },
                    { name: "BPKNo", model: "detail.BPKNo", text: "No.BPK", cls: "span3", disable: "isStatus", readonly: true },
                    { name: "ChassisCode", model: "detail.ChassisCode", text: "Kode Rangka", cls: "span3", disable: "isStatus", type: "popup", click: "ChassisCode()", required: true },
                    { name: "ChassisNo", model: "detail.ChassisNo", text: "No Rangka", cls: "span3", disable: "isStatus", type: "popup", click: "ChassisNo()", required: true },
                    { name: "SKPKNo", model: "detail.SKPKNo", text: "No.SKPK", cls: "span3 full", disable: "isStatus", required: true },
                    { name: "SKPKName", model: "detail.SKPKName", text: "SKPK Nama", cls: "span6", disable: "isStatus", required: true },
                    { name: "SKPKAddress1", model: "detail.SKPKAddress1", text: "SKPK Alamat", cls: "span6", disable: "isStatus", required: true, maxlength: "40" },
                    { name: "SKPKAddress2", model: "detail.SKPKAddress2", text: "", cls: "span6", disable: "isStatus", required: true, maxlength: "40" },
                    { name: "SKPKAddress3", model: "detail.SKPKAddress3", text: "", cls: "span6", disable: "isStatus", required: true, maxlength: "34" },
                    {
                        text: "SKPK Kota",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        items: [
                            { name: "SKPKCity", model: "detail.SKPKCity", cls: "span2", disable: "isStatus", required: true, type: "popup", click: "SKPKCity()" },
                            { name: "SKPKCityName", model: "detail.SKPKCityName", cls: "span6", readonly: true, disable: "isStatus" }
                        ]
                    },
                    { name: "DealerCategory", model: "detail.DealerCategory", cls: "span3 full", type: "select2", datasource: "DealerCategory", text: "Kategori Dealer", disable: "isStatus", required: true },
                    { name: "FakturPolisiNo", model: "detail.FakturPolisiNo", text: "No Faktur Polis", cls: "span3", disable: "isStatus", type: "popup", click: "FakturPolisiNo()" },
                    { name: "StatusBlanko", model: "detail.StatusBlanko", text: "Status Blanko", cls: "span3", readonly: true, disable: "isStatus" },
                    { name: "FakturPolisiDate", model: "detail.FakturPolisiDate", text: "Tgl. Faktur Polis", cls: "span3 full", type: 'ng-datepicker', disable: "isStatus" },
                    { name: "SalesmanName", model: "detail.SalesmanName", text: "Salesman", cls: "span6", disable: "isStatus", required: true },
                    {
                        text: "SKPK No Telp",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        items: [
                            { name: "SKPKTelp1", model: "detail.SKPKTelp1", text: "SKPK No.Telp", cls: "span4", disable: "isStatus", required: true },
                            { name: "SKPKTelp2", model: "detail.SKPKTelp2", text: "SKPK No.Telp", cls: "span4", disable: "isStatus", readonly: true },
                        ]
                    },
                    { name: "SKPKHP", model: "detail.SKPKHP", text: "SKPK No HP", cls: "span3", required: true, disable: "isStatus" },
                    { name: "SKPKBirthday", model: "detail.SKPKBirthday", text: "SKPK Tgl Lahir", cls: "span3 full", type: 'ng-datepicker', disable: "isStatus" },
                    { type: "hr" },
                    { name: "FakturPolisiName", model: "detail.FakturPolisiName", text: "Faktur Atas Nama", cls: "span6", disable: "isStatus", required: true, show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiAddress1", model: "detail.FakturPolisiAddress1", text: "Alamat", cls: "span6", disable: "isStatus", required: true, show: "data.StatusFaktur == true", maxlength: "40" },
                    { name: "FakturPolisiAddress2", model: "detail.FakturPolisiAddress2", text: "", cls: "span6", disable: "isStatus", required: true, show: "data.StatusFaktur == true", maxlength: "40" },
                    { name: "FakturPolisiAddress3", model: "detail.FakturPolisiAddress3", text: "", cls: "span6", disable: "isStatus", required: true, show: "data.StatusFaktur == true", maxlength: "34" },
                    {
                        text: "Kota",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        show: "data.StatusFaktur == true",
                        items: [
                            { name: "FakturPolisiCity", model: "detail.FakturPolisiCity", cls: "span2", disable: "isStatus", required: true, type: "popup", click: "FakturPolisiCity()", show: "data.StatusFaktur == true" },
                            { name: "FakturPolisiCityName", model: "detail.FakturPolisiCityName", cls: "span6", readonly: true, disable: "isStatus", show: "data.StatusFaktur == true" }
                        ]
                    },
                    {
                        text: "Kode Pos",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        show: "data.StatusFaktur == true",
                        items: [
                            { name: "PostalCode", model: "detail.PostalCode", cls: "span2", disable: "isStatus", required: true, type: "popup", click: "PostalCode()", show: "data.StatusFaktur == true" },
                            { name: "PostalCodeDesc", model: "detail.PostalCodeDesc", cls: "span6", readonly: true, disable: "isStatus", show: "data.StatusFaktur == true" }
                        ]
                    },
                    {
                        text: "No Telp",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        show: "data.StatusFaktur == true",
                        items: [
                            { name: "FakturPolisiTelp1", model: "detail.FakturPolisiTelp1", text: "No.Telp", cls: "span4", disable: "isStatus", required: true, show: "data.StatusFaktur == true" },
                            { name: "FakturPolisiTelp2", model: "detail.FakturPolisiTelp2", text: "No.Telp", cls: "span4", disable: "isStatus", readonly: true, show: "data.StatusFaktur == true" },
                        ]
                    },
                    { name: "FakturPolisiHP", model: "detail.FakturPolisiHP", text: "No HP", cls: "span3", required: true, disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiBirthday", model: "detail.FakturPolisiBirthday", text: "Tgl Lahir", cls: "span3", type: 'ng-datepicker', disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: "IDNo", model: "detail.IDNo", text: "No Identitas", cls: "span6", disable: "isStatus", required: true, show: "data.StatusFaktur == true" },
                    { name: 'IsCityTransport', model: "detail.IsCityTransport", text: "Untuk Angkot", type: 'x-switch', cls: "span4", float: 'left', disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: 'IsProject', model: "detail.IsProject", text: "Untuk Project", type: 'x-switch', cls: "span4", float: 'right', disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: "ReasonCode", model: "detail.ReasonCode", cls: "span3", type: "select2", datasource: "ReasonCode", text: "Reason Code", disable: "isStatus", show: "data.StatusFaktur == false" },
                    { name: "ReasonDesc", model: "detail.ReasonDesc", text: "Reason Description", cls: "span5", disable: "isStatus", show: "data.StatusFaktur == false" },
                    { type: "hr" },
                    {
                        type: "buttons",
                        items: [
                                { name: "btnAddDetail", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddDetail()", disable: "isStatus" },
                                { name: "btnUpdateDetail", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddDetail()", disable: "isStatus" },
                                { name: "btnDeleteDetail", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteDetail()", disable: "isStatus" },
                                { name: "btnCancelDetail", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CancelDetail()", disable: "isStatus" }
                        ]
                    },
                ]
            },
            {
                name: "wxkendaraan",
                xtype: "wxtable",
                cls: "tabDetail tabPageKendaraan",
            },
            {
                name: "pnlDetailCustomer",
                title: "Detail Customer",
                cls: "tabDetail tabPageCustomer",
                items: [
                    { name: "ChassisCode", model: "detail2.ChassisCode", text: "Chassis Code", cls: "span6", readonly: true },
                    { name: "ChassisNo", model: "detail2.ChassisNo", text: "Chassis No", cls: "span6", readonly: true },
                    {
                        text: "Jenis Kelamin",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "JenisKelamin", model: "detail2.JenisKelamin", text: "Jenis Kelamin", cls: "span4", type: "select2", datasource: "cboJenisKelamin" },
                        ]
                    },
                    {
                        text: "Tempat Pembelian",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "TempatPembelian", model: "detail2.TempatPembelian", cls: "span4", type: "select2", datasource: "cboTmpPembelian" },
                            { name: "TempatPembelianOther", model: "detail2.TempatPembelianOther", cls: "span4", maxlength: "30" },
                        ]
                    },
                    {
                        text: "Kend. yang dimiliki",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "KendaraanDimiliki", model: "detail2.KendaraanYgPernahDimiliki", text: "Yang Pernah Dimiliki", cls: "span4", type: "select2", datasource: "cboKendaraanDimiliki" },
                            { name: "ModelYgPernahDimiliki", model: "detail2.ModelYgPernahDimiliki", text: "", cls: "span2", type: "popup", click: "lkpTipeKendaraan()" },
                            { name: "ModelYgPernahDimilikiDesc", model: "detail2.ModelYgPernahDimilikiDesc", text: "", cls: "span2", readonly: true },
                        ]
                    },
                    {
                        text: "Sumber Pembelian",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "SumberPembelian", model: "detail2.SumberPembelian", text: "Sumber Pembelian", cls: "span4", type: "select2", datasource: "cboSumberPembelian" },
                            { name: "SumberPembelianOther", model: "detail2.SumberPembelianOther", cls: "span4", maxlength: "30" },
                        ]
                    },
                    {
                        text: "Asal Pembelian",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "AsalPembelian", model: "detail2.AsalPembelian", text: "Asal Pembelian", cls: "span4", type: "select2", datasource: "cboAsalPembelian" },
                            { name: "AsalPembelianOther", model: "detail2.AsalPembelianOther", cls: "span4", maxlength: "30" },
                        ]
                    },
                    {
                        text: "Info Suzuki Dari",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "InfoSuzukiDari", model: "detail2.InfoSuzukiDari", text: "Info Suzuki Dari", cls: "span4", type: "select2", datasource: "cboInfoSuzuki" },
                            { name: "InfoSuzukiDariOther", model: "detail2.InfoSuzukiDariOther", cls: "span4", maxlength: "30" },
                        ]
                    },
                    {
                        text: "Faktor Penting Memilih",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "FaktorPentingMemilihMotor", model: "detail2.FaktorPentingMemilihMotor", text: "Faktor Memilih Kend.", cls: "span4", type: "select2", datasource: "cboFaktorMemilih" },
                        ]
                    },
                    {
                        text: "Pendidikan Terakhir",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "PendidikanTerakhir", model: "detail2.PendidikanTerakhir", text: "Pendidikan Terakhir", cls: "span4", type: "select2", datasource: "cboPendidikanAkhir" },
                            { name: "PendidikanTerakhirOther", model: "detail2.PendidikanTerakhirOther", cls: "span4", maxlength: "30" },
                        ]
                    },
                    {
                        text: "Penghasilan",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "Penghasilan", model: "detail2.PenghasilanPerBulan", text: "Penghasilan", cls: "span4", type: "select2", datasource: "cboPenghasilan" },
                        ]
                    },
                    {
                        text: "Pekerjaan",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "Pekerjaan", model: "detail2.Pekerjaan", text: "Pekerjaan", cls: "span4", type: "select2", datasource: "cboPekerjaan" },
                            { name: "PekerjaanOther", model: "detail2.PekerjaanOther", cls: "span4", maxlength: "30" },
                        ]
                    },
                    {
                        text: "Penggunaan",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "PenggunaanMotor", model: "detail2.PenggunaanMotor", text: "Penggunaan", cls: "span4", type: "select2", datasource: "cboPenggunaan" },
                            { name: "PenggunaanMotorOther", model: "detail2.PenggunaanMotorOther", cls: "span4", maxlength: "30" },
                        ]
                    },
                    {
                        text: "Cara Pembelian",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "CaraPembelian", model: "detail2.CaraPembelian", text: "Cara Pembelian", cls: "span4", type: "select2", datasource: "cboCaraBeli" },
                        ]
                    },
                    {
                        text: "Leasing",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "Leasing", model: "detail2.Leasing", text: "Leasing", cls: "span4", type: "select2", datasource: "cboLeasing" },
                            { name: "LeasingOther", model: "detail2.LeasingOther", cls: "span4", maxlength: "30" },
                        ]
                    },
                    {
                        text: "Jangka Waktu",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "JangkaWaktuKredit", model: "detail2.JangkaWaktuKredit", text: "Jangka Waktu", cls: "span4", type: "select2", datasource: "cboJangkaWaktu" },
                            { name: "JangkaWaktuKreditOther", model: "detail2.JangkaWaktuKreditOther", cls: "span4", maxlength: "30" },
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnSimpanDetail2", text: "Simpan", icon: "icon-save", cls: "btn btn-success", click: "saveAngket()" },
                            { name: "btnClearDetail2", text: "Hapus", icon: "icon-remove", cls: "btn btn-danger", click: "deleteAngket()" },
                        ]
                    },
                ]
            },


        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    $("p[data-name='tabPageKendaraan']").addClass('active');

    function init(s) {
        SimDms.Angular("omPermohonanFakturPolisController");
    }



});