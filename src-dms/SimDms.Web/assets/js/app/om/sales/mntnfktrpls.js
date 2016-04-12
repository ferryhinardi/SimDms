"use strict"
var status = 0;

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
            me.detail.SONo = data.SONo;
            me.detail.BPKNo = data.BPKNo;
            me.detail.ChassisCode = data.ChassisCode;
            me.detail.ChassisNo = data.chassisNo;
            me.detail.SKPKName = data.EndUserName;
            me.detail.SKPKAddress1 = data.EndUserAddress1;
            me.detail.SKPKAddress2 = data.EndUserAddress2;
            me.detail.SKPKAddress3 = data.EndUserAddress3;
            me.detail.SKPKCity = data.CityCode;
            me.detail.SKPKCityName = data.CityName;
            me.detail.SalesmanCode = data.Salesman;
            me.detail.SalesmanName = data.SalesmanName;
            me.detail.SKPKTelp1 = data.PhoneNo;
            me.detail.SKPKHP = data.HPNo;
            me.detail.SKPKBirthday = data.birthDate;
            me.detail.FakturPolisiName = data.CustomerName;
            me.detail.FakturPolisiAddress1 = data.Address1;
            me.detail.FakturPolisiAddress2 = data.Address2;
            me.detail.FakturPolisiAddress3 = data.Address3;
            me.detail.FakturPolisiCity = data.CityCode;
            me.detail.FakturPolisiCityName = data.CityName;

            me.Apply();
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
            me.detail.SONo = data.SONo;
            me.detail.BPKNo = data.BPKNo;
            me.detail.ChassisCode = data.ChassisCode;
            me.detail.ChassisNo = data.chassisNo;
            me.detail.SKPKName = data.EndUserName;
            me.detail.SKPKAddress1 = data.EndUserAddress1;
            me.detail.SKPKAddress2 = data.EndUserAddress2;
            me.detail.SKPKAddress3 = data.EndUserAddress3;
            me.detail.SKPKCity = data.CityCode;
            me.detail.SKPKCityName = data.CityName;
            me.detail.SalesmanCode = data.Salesman;
            me.detail.SalesmanName = data.SalesmanName;
            me.detail.SKPKTelp1 = data.PhoneNo;
            me.detail.SKPKHP = data.HPNo;
            me.detail.SKPKBirthday = data.birthDate;
            me.detail.FakturPolisiName = data.CustomerName;
            me.detail.FakturPolisiAddress1 = data.Address1;
            me.detail.FakturPolisiAddress2 = data.Address2;
            me.detail.FakturPolisiAddress3 = data.Address3;
            me.detail.FakturPolisiCity = data.CityCode;
            me.detail.FakturPolisiCityName = data.CityName;

            me.Apply();
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
        $http.post('om.api/PermohonanFakturPolis/Approve', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    $('#Status').html(data.status);
                    $('#btnApprove').attr('disabled', 'disabled');
                    status = data.Result;
                    me.isStatus = status == 2;
                    Wx.Success("Data approved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
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

                       '<input type="radio" name="PrintType" id="PrintType1" value="0" checked>&nbsp Print Faktur Polis</div>' +

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
            var ReportId = 'OmRpSalesTrn007DNew';
        }
        else if ($('input[name=PrintType]:checked').val() === '1') {
            var ReportId = '';
        }
        else {
            var ReportId = '';
        }

        var par = me.data.ReqNo + ',' + me.data.ReqNo;
        var rparam = 'Print Permohonan Faktur Polis'

        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }


    //me.saveData = function (e, param) {
    //    $http.post('om.api/PermohonanFakturPolis/Save', me.data).
    //        success(function (data, status, headers, config) {
    //            if (data.success) {
    //                if (me.data.ReqNo == null) {
    //                    me.data.ReqNo = data.data.ReqNo;
    //                    //me.saveData();
    //                }
    //                $('#Status').html(data.status);
    //                $('#pnlKendaraan').show();
    //                $('#wxkendaraan').show();
    //                Wx.Success("Data saved...");
    //                me.startEditing();
    //                me.griddetail.adjust();
    //            } else {
    //                MsgBox(data.message, MSG_ERROR);
    //            }
    //        }).
    //        error(function (data, status, headers, config) {
    //            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
    //        });
    //};

    me.AddDetail = function (e, param) {
        $http.post('om.api/PermohonanFakturPolis/SaveDetailMaintenance', { model: me.data, detailModel: me.detail }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.clearTable(me.griddetail);
                    me.loadTableData(me.griddetail, data.data);
                    me.detail = {};
                    $('#btnAddDetail').show();
                    $('#btnUpdateDetail').hide();
                    $('#btnCancelDetail').hide();
                    $('#btnDeleteDetail').hide();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

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
            name: "MntnPermohonanBrowse",
            title: "Permohonan Faktur terima",
            manager: spSalesManager,
            query: "MntnPermohonanBrowse",
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
                $('#Status').html(data.Status);
                me.lookupAfterSelect(data);
                me.loadDetail(data);
                if (me.data.ReffDate == null) {
                    me.data.isActive1 = false;
                }
                else {
                    me.data.isActive1 = true;
                }
                if (data.isCBU == false) {
                    me.isCBU = "0";
                }
                else {
                    me.isCBU = "1";
                }
                status = data.Stat;
                me.isStatus = status == 2;
                //$('#btnAddDetail').show();
                //$('#btnAddDetail').attr('disabled', true);
                //$('#btnCancelDetail').show();
                //$('#btnCancelDetail').attr('disabled', true);
                //$('#btnUpdateDetail').show();
                //$('#btnUpdateDetail').attr('disabled', true);
                $('#wxkendaraan').show();
                me.Apply();
            }
        });
    };

    me.loadDetail = function (data) {
        $http.post('om.api/PermohonanFakturPolis/DetailSalesReq', data)
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
    }

    me.griddetail = new webix.ui({
        container: "wxkendaraan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "SONo", header: "No.SO", width: 200 },
            { id: "ChassisCode", header: "Kode.Rangka", width: 200 },
            { id: "ChassisCode", header: "No. Rangka", width: 200 },
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
                    //$('#btnAddDetail').hide();
                    $('#btnCancelDetail').show();
                    $('#btnUpdateDetail').show();
                    $('#btnCancelDetail').removeAttr('disabled');
                    $('#btnUpdateDetail').removeAttr('disabled');

                    EditDetail();
                    me.detail.oid = me.griddetail.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    function EditDetail() {
        $('#FakturPolisiName').removeAttr('disabled');
        $('#FakturPolisiAddress1').removeAttr('disabled');
        $('#FakturPolisiAddress2').removeAttr('disabled');
        $('#FakturPolisiAddress3').removeAttr('disabled');
        $('#btnFakturPolisiCity').removeAttr('disabled');
        $('#FakturPolisiCity').removeAttr('disabled');
        $('#FakturPolisiCityName').removeAttr('disabled');
        $('#btnPostalCode').removeAttr('disabled');
        $('#PostalCode').removeAttr('disabled');
        $('#PostalCodeDesc').removeAttr('disabled');
        $('#FakturPolisiTelp1').removeAttr('disabled');
        $('#FakturPolisiTelp2').removeAttr('disabled');
        $('#FakturPolisiHP').removeAttr('disabled');
        $('#FakturPolisiBirthday').removeAttr('disabled');
        $('#IDNo').removeAttr('disabled');
        $('#IsCityTransport').removeAttr('disabled');
        $('#IsProject').removeAttr('disabled');

    }

    function UnEditDetail() {
        $('#FakturPolisiName').attr('disabled', 'disabled');
        $('#FakturPolisiAddress1').attr('disabled', 'disabled');
        $('#FakturPolisiAddress2').attr('disabled', 'disabled');
        $('#FakturPolisiAddress3').attr('disabled', 'disabled');
        $('#FakturPolisiCity').attr('disabled', 'disabled');
        $('#FakturPolisiCityName').attr('disabled', 'disabled');
        $('#PostalCode').attr('disabled', 'disabled');
        $('#PostalCodeDesc').attr('disabled', 'disabled');
        $('#FakturPolisiTelp1').attr('disabled', 'disabled');
        $('#FakturPolisiTelp2').attr('disabled', 'disabled');
        $('#FakturPolisiHP').attr('disabled', 'disabled');
        $('#FakturPolisiBirthday').attr('disabled', 'disabled');
        $('#IDNo').attr('disabled', 'disabled');
        $('#IsCityTransport').attr('disabled', 'disabled');
        $('#IsProject').attr('disabled', 'disabled');

    }
    
    me.CancelDetail = function () {
        me.detail = {};
        me.detail.FakturPolisiDate = me.now();
        me.detail.SKPKBirthday = me.now();
        me.detail.FakturPolisiBirthday = me.now();
        me.griddetail.clearSelection();
        UnEditDetail()
        $('#btnAddDetail').show();
        $('#btnCancelDetail').show();
        $('#btnUpdateDetail').hide();
    }


    me.initialize = function () {
        me.isStatus = true;
        me.detail = {};
        me.data.StatusFaktur = true;
        //$('#SubDealerCode').removeAttr('disabled');


        me.status = "NEW";
        $('#Status').html(me.status);
        $('#Status').css(
        {
            "font-size": "32px",
            "color": "red",
            "font-weight": "bold",
            "text-align": "center"
        });

        $('#wxkendaraan').hide();
        $('#btnApprove').attr('disabled', true)

        me.data.ReqDate = me.now();
        me.detail.FakturPolisiDate = me.now();
        me.detail.SKPKBirthday = me.now();
        me.detail.FakturPolisiBirthday = me.now();

        if (me.data.isActive1 == true) {
            me.data.ReffDate = me.now();
        }
        else {
            me.data.ReffDate = undefined;
        }

        $('#btnUpdateDetail').hide();
        $('#btnDeleteDetail').hide();
        $('#btnCancelDetail').hide();

        me.isPrintAvailable = true;
        me.isApprove = true;
        //me.isCancel = false;

        me.griddetail.adjust();

    }
    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Maintenance Permohonan Faktur Polisi",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "!isApprove", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        //toolbars: WxButtons,
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
                            { name: "ReqDate", model: "data.ReqDate", type: "ng-datepicker", cls: "span3", disable: "isStatus" },
                            { name: "Status", text: "", cls: "span5 right", readonly: true, type: "label" },
                        ]
                    },
                    { name: "ReffNo", model: "data.ReffNo", text: "No.Reff", cls: "span3", required: true, validasi: "required", disable: "isStatus" },
                    {
                        text: "Tgl.Reff",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "ReffDate", model: "data.ReffDate", placeHolder: "Tgl. Reff", cls: "span4", type: 'ng-datepicker', disable: "data.isActive1 == false || isStatus" },
                            { name: "isActive1", model: "data.isActive1", type: 'x-switch', cls: "span4", float: 'left', disable: "isStatus" },

                        ]
                    },
                    { name: "StatusFaktur", model: "data.StatusFaktur", text: "IsFaktur", type: 'x-switch', cls: "span2", float: 'left', disable: "isStatus" },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        cls: "span6",
                        required: true,
                        validasi: "required",
                        items: [
                            { name: "SubDealerCode", model: "data.SubDealerCode", cls: "span2", disable: "isStatus", type: "popup", click: "SubDealerCode()" },
                            { name: "CustomerName", model: "data.CustomerName", cls: "span6", readonly: true, disable: "isStatus" }
                        ]
                    },
                    { name: "Remark", model: "data.Remark", text: "Keterangan", cls: "span8", disable: "isStatus" },
                    { type: "hr" },
                    { name: "SONo", model: "detail.SONo", text: "No.SO", cls: "span3", disabled: "isStatus", readonly: true },
                    { name: "BPKNo", model: "detail.BPKNo", text: "No.BPK", cls: "span3", disabled: "isStatus", readonly: true },
                    { name: "ChassisCode", model: "detail.ChassisCode", text: "Kode Rangka", cls: "span3", disable: "isStatus", type: "popup", click: "ChassisCode()", required: true, validasi: "required" },
                    { name: "ChassisNo", model: "detail.ChassisNo", text: "No Rangka", cls: "span3", disable: "isStatus", type: "popup", click: "ChassisNo()", required: true, validasi: "required" },
                    { name: "SKPKNo", model: "detail.SKPKNo", text: "No.SKPK", cls: "span3 full", disable: "isStatus", required: true, validasi: "required" },
                    { name: "SKPKName", model: "detail.SKPKName", text: "SKPK Nama", cls: "span6", disable: "isStatus", required: true, validasi: "required" },
                    { name: "SKPKAddress1", model: "detail.SKPKAddress1", text: "SKPK Alamat", cls: "span6", disable: "isStatus", required: true, validasi: "required" },
                    { name: "SKPKAddress2", model: "detail.SKPKAddress2", text: "", cls: "span6", disable: "isStatus", required: true, validasi: "required" },
                    { name: "SKPKAddress3", model: "detail.SKPKAddress3", text: "", cls: "span6", disable: "isStatus", required: true, validasi: "required" },
                    {
                        text: "SKPK Kota",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        items: [
                            { name: "SKPKCity", model: "detail.SKPKCity", cls: "span2", disable: "isStatus", required: true, validasi: "required", type: "popup", click: "SKPKCity()" },
                            { name: "SKPKCityName", model: "detail.SKPKCityName", cls: "span6", readonly: true, disable: "isStatus" }
                        ]
                    },
                    { name: "DealerCategory", model: "detail.DealerCategory", cls: "span3 full", type: "select2", datasource: "DealerCategory", text: "Kategori Dealer", disable: "isStatus", required: true, validasi: "required" },
                    { name: "FakturPolisiNo", model: "detail.FakturPolisiNo", text: "No Faktur Polis", cls: "span3", disable: "isStatus", type: "popup", click: "FakturPolisiNo()" },
                    { name: "StatusBlanko", model: "detail.StatusBlanko", text: "Status Blanko", cls: "span3", readonly: true, disable: "isStatus" },
                    { name: "FakturPolisiDate", model: "detail.FakturPolisiDate", text: "Tgl. Faktur Polis", cls: "span3 full", type: 'ng-datepicker', disable: "isStatus" },
                    {
                        text: "Salesman",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        items: [
                            { name: "SalesmanCode", model: "detail.SalesmanCode", cls: "span4", disable: "isStatus", required: true, validasi: "required", type: "popup", click: "SalesmanCode()" },
                            { name: "SalesmanName", model: "detail.SalesmanName", cls: "span4", readonly: true, disable: "isStatus" }
                        ]
                    },
                    {
                        text: "SKPK No Telp",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        items: [
                            { name: "SKPKTelp1", model: "detail.SKPKTelp1", text: "SKPK No.Telp", cls: "span4", disable: "isStatus", required: true, validasi: "required" },
                            { name: "SKPKTelp2", model: "detail.SKPKTelp2", text: "SKPK No.Telp", cls: "span4", disable: "isStatus", readonly: true },
                        ]
                    },
                    { name: "SKPKHP", model: "detail.SKPKHP", text: "SKPK No HP", cls: "span3", required: true, validasi: "required", disable: "isStatus" },
                    { name: "SKPKBirthday", model: "detail.SKPKBirthday", text: "SKPK Tgl Lahir", cls: "span3 full", type: 'ng-datepicker', disable: "isStatus" },
                    { type: "hr" },
                    { name: "FakturPolisiName", model: "detail.FakturPolisiName", text: "Faktur Atas Nama", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiAddress1", model: "detail.FakturPolisiAddress1", text: "Alamat", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiAddress2", model: "detail.FakturPolisiAddress2", text: "", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiAddress3", model: "detail.FakturPolisiAddress3", text: "", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    {
                        text: "Kota",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        show: "data.StatusFaktur == true",
                        items: [
                            { name: "FakturPolisiCity", model: "detail.FakturPolisiCity", cls: "span2", disable: "isStatus", required: true, validasi: "required", type: "popup", click: "FakturPolisiCity()", show: "data.StatusFaktur == true" },
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
                            { name: "PostalCode", model: "detail.PostalCode", cls: "span2", disable: "isStatus", required: true, validasi: "required", type: "popup", click: "PostalCode()", show: "data.StatusFaktur == true" },
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
                            { name: "FakturPolisiTelp1", model: "detail.FakturPolisiTelp1", text: "SKPK No.Telp", cls: "span4", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                            { name: "FakturPolisiTelp2", model: "detail.FakturPolisiTelp2", text: "SKPK No.Telp", cls: "span4", disable: "isStatus", readonly: true, show: "data.StatusFaktur == true" },
                        ]
                    },
                    { name: "FakturPolisiHP", model: "detail.FakturPolisiHP", text: "No HP", cls: "span3", required: true, validasi: "required", disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiBirthday", model: "detail.FakturPolisiBirthday", text: "Tgl Lahir", cls: "span3", type: 'ng-datepicker', disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: "IDNo", model: "detail.IDNo", text: "No Identitas", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    { name: "IsCityTransport", model: "detail.IsCityTransport", text: "Untuk Angkot", type: 'x-switch', cls: "span4", float: 'left', disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: "IsProject", model: "detail.IsProject", text: "Untuk Project", type: 'x-switch', cls: "span4", float: 'right', disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: "ReasonCode", model: "detail.ReasonCode", cls: "span3", type: "select2", datasource: "ReasonCode", text: "Reason Code", disable: "isStatus", show: "data.StatusFaktur == false" },
                    { name: "ReasonDesc", model: "detail.ReasonDesc", text: "Reason Description", cls: "span5", disable: "isStatus", show: "data.StatusFaktur == false" },
                    { type: "hr" },
                    {
                        type: "buttons",
                        items: [
                                //{ name: "btnAddDetail", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddDetail()", disable: "isStatus" },
                                { name: "btnUpdateDetail", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddDetail()" },
                                //{ name: "btnDeleteDetail", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteDetail()", disable: "isStatus" },
                                { name: "btnCancelDetail", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CancelDetail()" }
                        ]
                    },
                ]
            },
            {
                name: "wxkendaraan",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omPermohonanFakturPolisController");
    }



});