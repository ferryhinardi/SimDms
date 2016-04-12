var pType = "";

function KDPController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        $http.post("its.api/InquiryIts/itsUserPropertiesAdmin").
         success(function (result, status, headers, config) {
             if (result.success) {
                 me.data.LastUpdateStatus = me.now();
                 me.data.InquiryDate = me.now();
                 me.data.LastProgress = "P";
                 me.detail.ActivityDate = me.now();
                 me.detail.NextFollowUpDate = me.now();
                 me.data.BranchCode = result.data.OutletID;
                 me.data.BranchName = result.data.OutletName;
                 me.isComboShow = result.data.ProductType === '4W' ? true : false;
                 me.hasChanged = false;
             }
             else {
                 MsgBox(result.message, MSG_ERROR);
             }
         });
        me.hasChanged = false;
        me.detail = {};
        me.clearTable(me.gridSalesModel);
        me.isdetail = true;
        me.isCash = true;
        me.istrue = false;
        me.Variant = [];
        me.transmissions = [];
        me.modelcolors = [];
        $('#Variant').attr('disabled', 'disabled');
        $('#Transmisi').attr('disabled', 'disabled');
        $('#ColourCode').attr('disabled', 'disabled');

        //me.data.Variant = "";
        //me.data.Transmisi = "";
        //me.data.ColourCode = "";
        
    }

    $http.post('its.api/combo/lookups?id=itsg').
    success(function (data, status, headers, config) {
        me.Grade = data;
    });

    $http.post('its.api/combo/lookups?id=pmsp').
    success(function (data, status, headers, config) {
        me.StatusProspek = data;
    });

    $http.post('its.api/combo/lookups?id=psrc').
    success(function (data, status, headers, config) {
        me.PerolehanData = data;
    });

    $http.post('its.api/combo/cartypes').
    success(function (data, status, headers, config) {
        me.TipeKendaraan = data;
    });

    $http.post('its.api/combo/lookups?id=pmby').
    success(function (data, status, headers, config) {
        me.CaraPembayaran = data;
    });

    $http.post('its.api/combo/lookups?id=lsng').
    success(function (data, status, headers, config) {
        me.Leasing = data;
    });

    $http.post('its.api/combo/lookups?id=dwpm').
    success(function (data, status, headers, config) {
        me.DowmPayment = data;
    });

    $http.post('its.api/combo/lookups?id=Tenor').
    success(function (data, status, headers, config) {
        me.Tenor = data;
    });

    $http.post('its.api/combo/lookups?id=pmop').
    success(function (data, status, headers, config) {
        me.TestDrive = data;
    });

    $http.post('its.api/combo/statusvehicles').
    success(function (data, status, headers, config) {
        me.StatusVehicle = data;
    });

    $http.post('its.api/combo/itsstatus').
    success(function (data, status, headers, config) {
        me.LastProgress = data;
    });

    $http.post('its.api/combo/lookups?id=plcc').
    success(function (data, status, headers, config) {
        me.LostCaseCategory = data;
    });

    $http.post('its.api/combo/lookups?id=pact').
    success(function (data, status, headers, config) {
        me.ActivityType = data;
    });

    me.ChangeCarType = function () {
        var carType = $('#TipeKendaraan').val();
        me.data.TipeKendaraan = carType;
        me.Variant = null;
        me.data.Variant = "";
        me.transmissions = null;
        me.data.Transmisi = "";
        me.modelcolors = null;
        me.data.ColourCode = "";
        me.Apply();
        if (carType == '') {
            $('#Variant').attr('disabled', 'disabled');
            $('#Transmisi').attr('disabled', 'disabled');
            $('#ColourCode').attr('disabled', 'disabled');
            return;
        }
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/combo/carvariants?id=' + carType,
            success: function (data) {
                me.Variant = data;
                me.Apply();
                $('#Variant').removeAttr('disabled');
                me.ChangeCarVariant();
            }
        });
    }

    me.ChangeCarVariant = function () {
        var carType = $('#TipeKendaraan').val();
        var carVariant = $('#Variant').val();
        me.data.Variant = carVariant;
        me.transmissions = null;
        me.data.Transmisi = "";
        me.modelcolors = null;
        me.data.ColourCode = "";
        me.Apply();
        if (carVariant == '') {
            $('#Transmisi').attr('disabled', 'disabled');
            $('#ColourCode').attr('disabled', 'disabled');
            return;
        }
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/combo/transmissions?CarType=' + carType + '&CarVariant=' + carVariant,
            success: function (data) {
                me.transmissions = data;
                me.Apply();
                $('#Transmisi').removeAttr('disabled');
                me.ChangeCarTrans(); alert(message);
            }
        });
    }

    me.ChangeCarTrans = function () {
        var carType = $('#TipeKendaraan').val();
        var carVariant = $('#Variant').val();
        var carTrans = $('#Transmisi').val();
        me.data.Transmisi = carTrans;
        me.modelcolors = null;
        me.data.ColourCode = "";
        me.Apply();
        if (carTrans == '') {
            $('#ColourCode').attr('disabled', 'disabled');
            return;
        }
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/combo/modelcolors?CarType=' + carType + '&CarVariant=' + carVariant + '&CarTrans=' + carTrans,
            success: function (data) {
                me.modelcolors = data;
                me.Apply();
                $('#ColourCode').removeAttr('disabled');
            }
        });
    }

    me.LoadVariantTransColor = function (carType, carVariant, carTrans, carColor) {
        $.ajax({
            async: false,
            type: "POST",
            data: {
                CarType: carType,
                CarVariant: carVariant,
                CarTrans: carTrans
            },
            url: 'its.api/combo/VariantsTransColors',
            success: function (data) {
                me.Variant = data.variants;
                me.transmissions = data.transmissions;
                me.modelcolors = data.colors;
                me.data.TipeKendaraan = carType;
                me.data.Variant = carVariant;
                me.data.Transmisi = carTrans;
                me.data.ColourCode = carColor;
                $('#Variant').removeAttr('disabled');
                $('#Transmisi').removeAttr('disabled');
                $('#ColourCode').removeAttr('disabled');
            }
        });
    }

    $('#LostCaseCategory').on('change', function (e) {
        $http.post('its.api/combo/lostreasons?CodeID=ITLR&LostCtg=' + $('#LostCaseCategory').val()).
            success(function (data, status, headers, config) {
                me.LostCaseReasonID = data;
            });
    });

    $('#CaraPembayaran').on('change', function (e) {
        if ($('#CaraPembayaran').val() == '20') me.isCash = false;
        else me.isCash = true;
    });

    $('#LastProgress').on('change', function (e) {
        if ($('#LastProgress').val() == 'LOST') {
            $('#pnlLostDetail').show();
            me.data.LostCaseDate = me.now();
        } else {
            me.isCash = true;
            $('#pnlLostDetail').hide();
        }

        if ($('#LastProgress').val() == 'SPK') {
            $('#pnlSpkDetail').show();
            //me.data.SPKDate = result.SPKDate;
        } else { $('#pnlSpkDetail').hide() }

        if ($('#LastProgress').val() == 'DO' || $('#LastProgress').val() == 'DELIVERY') {
            $('#btnSave').hide();
        } else { $('#btnSave').show(); }
    });

    me.browse = function () {
        $('#TipeKendaraan').off('change');
        $('#Variant').off('change');
        $('#Transmisi').off('change');
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "Inquiry Kdp",
            url: "its.api/grid/KdpBrowse",
            pageSize: 10,
            serverBinding: true,
            columns: [
                        { field: "InquiryNumber", title: "Inquiry", width: 90 },
                        { field: "TipeKendaraan", title: "Type Kendaraan", width: 160 },
                        { field: "Variant", title: "Varian", width: 160 },
                        { field: "Transmisi", title: "Trans", width: 80 },
                        { field: "ColourName", title: "Warna", width: 240 },
                        { field: "PerolehanData", title: "Perolehan Data", width: 140 },
                        { field: "NamaProspek", title: "Nama Customer", width: 260 },
                        { field: "EmployeeName", title: "Salesman", width: 160 },
                        { field: "TestDrive", title: "Test Drive", width: 80 },
                        { field: "QuantityInquiry", title: "Qty Inq", width: 80 },
                        { field: "LastProgress", title: "Status", width: 100 },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.NikSH = result.LeaderID;
                me.data.NikSHName = result.LeaderName;
                me.data.Grade = result.Grade;
                me.data.InquiryNumber = result.InquiryNumber;
                me.data.BranchCode = result.BranchCode;
                me.data.CompanyCode = result.CompanyCode;
                me.data.NikSales = result.EmployeeID;
                me.data.NikSalesName = result.EmployeeName;
                me.data.NikSales = result.EmployeeID;
                me.data.NikSalesName = result.EmployeeName;
                me.data.EmployeeID = result.EmployeeID;
                me.data.SpvEmployeeID = result.SpvEmployeeID;
                me.data.InquiryDate = result.InquiryDate;
                me.data.OutletID = result.OutletID;
                me.data.StatusProspek = result.StatusProspek;
                me.data.PerolehanData = result.PerolehanData;
                me.data.NamaProspek = result.NamaProspek;
                me.data.AlamatProspek = result.AlamatProspek;
                me.data.TelpRumah = result.TelpRumah;
                me.data.CityID = result.CityID;
                me.data.CityName = result.CityName;
                me.data.NamaPerusahaan = result.NamaPerusahaan;
                me.data.AlamatPerusahaan = result.AlamatPerusahaan;
                me.data.Jabatan = result.Jabatan;
                me.data.Handphone = result.Handphone;
                me.data.Faximile = result.Faximile;
                me.data.Email = result.Email;


                me.data.CaraPembayaran = result.CaraPembayaran;
                me.data.TestDrive = result.TestDrive;
                me.data.QuantityInquiry = result.QuantityInquiry;
                me.data.LastProgress = result.LastProgress;
                me.data.LastUpdateStatus = result.LastUpdateStatus;
                me.data.Leasing = result.Leasing;
                me.data.DownPayment = result.DownPayment;
                me.data.Tenor = result.Tenor;
                me.detail.ActivityDate = me.now();
                me.detail.NextFollowUpDate = me.now();
                if (result.LastProgress == 'LOST') {
                    me.LastProgress = [
                        { "text": "LOST", "value": "LOST" }
                    ];
                    $('#pnlLostDetail').show();
                    me.data.LostCaseDate = result.LostCaseDate;
                    me.data.LostCaseCategory = result.LostCaseCategory;
                    me.data.LostCaseReasonID = result.LostCaseReasonID;
                    me.data.MerkLain = result.MerkLain;
                    me.data.LostCaseOtherReason = result.LostCaseOtherReason;
                    me.data.LostCaseVoiceOfCustomer = result.LostCaseVoiceOfCustomer;
                }
                if (result.LastProgress == 'SPK') {
                    $('#pnlSpkDetail').show();
                    me.data.SPKDate = result.SPKDate;
                }


                me.LoadVariantTransColor(result.TipeKendaraan, result.Variant, result.Transmisi, result.ColourCode);

                setTimeout(function () {
                    me.isEditable = true;
                    me.isSave = false;
                    me.hasChanged = false;
                    me.isLoadData = true;
                    $scope.$apply();
                }, 500);
                me.ClearDtl();

            }
        });
    }

    me.SalesHead = function () {
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "Salesman",
            url: "its.api/grid/SHlist",
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "EmployeeID", title: "ID" },
                { field: "EmployeeName", title: "Salesman" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NikSH = data.EmployeeID;
                me.data.NikSHName = data.EmployeeName;
                me.Apply();
            }
        });
    }

    me.Salesman = function () {
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "Salesman",
            url: "its.api/grid/SMlist",
            params: { name: "controls", items: [{ name: "NikSH", param: "NikSH" }, { name: "NikSC", param: "NikSC" }] },
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "EmployeeID", title: "ID" },
                { field: "EmployeeName", title: "Salesman" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NikSales = data.EmployeeID;
                me.data.NikSalesName = data.EmployeeName;
                $http.post('its.api/combo/Grade?NikSales=' + $('#NikSales').val()).
                success(function (data, status, headers, config) {
                    if (data != '""') {
                        me.data.Grade = data.replace('"', '').replace('"', '');
                    } else {
                        me.data.Grade = 1
                    };
                });
            }
        });
    }

    me.City = function () {
        var lookup = Wx.blookup({
            name: "KotaLookup",
            title: "Kota",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "CITY" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Kode Kota" },
                { field: "LookUpValueName", title: "Nama Kota" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CityID = data.LookUpValue;
                me.data.CityName = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    me.other = function () {
        var lookup = Wx.klookup({
            name: "btnBrandCode",
            title: "Merek Kendaraan",
            url: "its.api/grid/modellist",
            serverBinding: true,
            pageSize: 10,
            filters: [
                {
                    text: "Filter Merek",
                    type: "controls",
                    left: 80,
                    items: [
                        { name: "fltBrand", text: "Merek", cls: "span2" },
                        { name: "fltModel", text: "Keterangan", cls: "span4" },
                    ]
                }
            ],
            params: { name: "controls", items: [{ name: "StatusVehicle", param: "fltStatu" }] },
            columns: [
                { field: "BrandCode", title: "Merek", width: 100 },
                { field: "ModelName", title: "Keterangan" },
                { field: "ModelType", title: "Tipe", width: 250 },
                { field: "Variant", title: "Varian", width: 250 },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BrandCode = data.BrandCode;
                me.data.ModelName = data.ModelName;
                me.Apply();
            }
        });
    }

    me.save = function (e, param) {
        me.data.EmployeeID = me.data.NikSales;
        me.data.SpvEmployeeID = me.data.NikSH;
        if (me.data.CaraPembayaran == '20') {
            if (me.data.Leasing == undefined) {
                MsgBox("Nama Leasing harus diisi");
                return;
            }
            if (me.data.DownPayment == undefined) {
                MsgBox("DP (%) harus diisi");
                return;
            };
            if (me.data.Tenor == undefined) {
                MsgBox("Tenor harus diisi");
                return;
            };
        }
        if ($('#InquiryNumber').val() != '') {
            me.data.InquiryNumber = $('#InquiryNumber').val();
        }

        $http.post('its.api/kdp/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    $('#InquiryNumber').val(data.data.InquiryNumber);
                    $('#pnlD').show();
                    //$.post("its.api/kdp/get", data.data, populate)
                } else {
                    MsgBox(data.message, MSG_ERROR);
                    console.log(data.message);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                me.detail.BranchCode = me.data.BranchCode;
                me.detail.InquiryNumber = $("[name=InquiryNumber]").val();
                $http.post('its.api/kdp/deleteact', me.detail).
                    success(function (dl, status, headers, config) {
                        if (dl.success) {
                            Wx.Info("Record has been deleted...");
                        }
                        me.ClearDtl();
                    }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.deleteDtl = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                me.detail.BranchCode = me.data.BranchCode;
                me.detail.InquiryNumber = $("[name=InquiryNumber]").val();
                $http.post('its.api/kdp/deleteact', me.detail).
                    success(function (dl, status, headers, config) {
                        if (dl.success) {
                            Wx.Info("Record has been deleted...");
                        }
                        me.ClearDtl();
                    }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.saveDtl = function () {
        MsgConfirm("Do you want to save the record?", function (result) {
            if (result) {
                me.detail.BranchCode = me.data.BranchCode;
                me.detail.InquiryNumber = $("[name=InquiryNumber]").val();
                $http.post("its.api/kdp/saveact", me.detail)
                    .success(function (result, status, headers, config) {
                        if (result.success) {
                            Wx.Info("Record has been Added...");
                            me.startEditing();
                            $http.post('its.api/kdp/Activities?InquiryNumber=' + me.detail.InquiryNumber).
                              success(function (data, status, headers, config) {
                                  me.grid.detail = data;
                                  me.loadTableData(me.gridSalesModel, me.grid.detail);
                              }).
                              error(function (e, status, headers, config) {
                                  console.log(e);
                              });
                            me.detail = {};
                            me.detail.ActivityDate = me.now();
                            me.detail.NextFollowUpDate = me.now();
                        } else {
                            MsgBox(result.message, MSG_ERROR);
                        }
                    })
                .error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.ClearDtl = function () {
        $http.post('its.api/kdp/Activities?InquiryNumber=' + me.data.InquiryNumber).
              success(function (data, status, headers, config) {
                  me.grid.detail = data;
                  me.loadTableData(me.gridSalesModel, me.grid.detail);

                  $('#TipeKendaraan').on('change', me.ChangeCarType);
                  $('#Variant').on('change', me.ChangeCarVariant);
                  $('#Transmisi').on('change', me.ChangeCarTrans);
              }).
              error(function (e, status, headers, config) {
                  //MsgBox("Terjadi kesalahan dalam proses data, silahkan hubungi SDMS support!", MSG_INFO);
                  console.log(e);
              });
        me.detail = {};
        me.detail.ActivityDate = me.now();
        me.detail.NextFollowUpDate = me.now();
    }

    me.printPreview = function () {
        $http.post('sp.api/EntryReturSupplySlip/Print', me.data)
        .success(function (v, status, headers, config) {
            if (v.success) {
                var data = me.data.ReturnNo + "," + me.data.ReturnNo + "," + "profitcenter" + "," + "typeofgoods";
                var rparam = "admin";

                Wx.showPdfReport({
                    id: "SpRpTrn013A",
                    pparam: data,
                    rparam: rparam,
                    type: "devex"
                });

                me.CheckReturnSS(me.data.ReturnNo);
            } else {
                // show an error message
                MsgBox(v.message, MSG_ERROR);
            }
        }).error(function (e, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.gridSalesModel = new webix.ui({
        container: "wxFUDtl",
        view: "wxtable", css: "alternating",
        scrollX: true,
        columns: [
            { id: "ActivityDates", header: "Tanggal", width: 150 },
            { id: "ActivityType", header: "Jenis Pertemuan", width: 352 },
            { id: "ActivityDetail", header: "Follow Up Detail", width: 400 },
            { id: "NextFollowUpDates", header: "Next Follow Up Date", width: 150 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridSalesModel.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.gridSalesModel.getSelectedId().id);
                    me.detail.oid = me.gridSalesModel.getSelectedId();
                    me.isdetail = false;
                    me.Apply();
                }
            }
        }
    });
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Kartu Data Prospek (Admin)",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
        ],
        panels: [
            {
                title: "Data Salesman",
                items: [
                    { name: "OutletID", text: "", type: "hidden", cls: "span8", readonly: true },
                    { name: "BranchName", text: "Outlet", cls: "span8", readonly: true },
                    { name: "InquiryNumber", text: "Nomor Inquiry", cls: "span4", readonly: true },
                    { name: "InquiryDate", text: "Tanggal Inquiry", type: "ng-datepicker", cls: "span4" },
                     {
                         text: "Sales Head", type: "controls", items: [
                             { name: "NikSH", text: "Sales Head", cls: "span4", type: "popup", cls: "span2", show: "isComboShow", click: "SalesHead()" },
                             { name: "NikSHName", text: "Sales Head Name", cls: "span6", readonly: true }
                         ]
                     },
                     {
                         text: "Salesman", type: "controls", items: [
                             { name: "NikSales", text: "Sales ID", type: "popup", cls: "span2", click: "Salesman()"},
                             { name: "NikSalesName", text: "Sales Name", cls: "span6", readonly: true }
                         ]
                     },

                      {
                          text: "Sales Head", show: "!isComboShow", type: "controls", items: [
                              { name: "NikSC", text: "Sales Coordinator", cls: "span4", type: "popup", cls: "span2", show: "!isComboShow", click: "SalesCoordinator()" },
                              { name: "NikSCName", text: "Sales Coordinator Name", cls: "span6", readonly: true, show: "!isComboShow", }
                          ]
                      },
                    { name: "Grade", text: "Salesman Grade", cls: "span4", readonly: true, type: "select2", datasource: "Grade" },
                    { name: "PerolehanData", text: "Perolehan Data", type: "select2", cls: "span4", required: true, validasi: "required", validasi: "required", datasource: "PerolehanData" },
                    { name: "StatusProspek", model: "data.StatusProspek", text: "Status Inquiry", type: "select2", cls: "span4", datasource: "StatusProspek" },                    
                    { name: "BranchCode", model: "data.BranchCode", type: "hidden" },
                    { name: "EmployeeID", model: "data.EmployeeID", type: "hidden" },
                    { name: "SpvEmployeeID", model: "data.SpvEmployeeID", type: "hidden" },
                    { name: "Grade", type: "hidden" },
                ]
            },
            {
                title: "Data Prospek",
                items: [
                    {
                        text: "Customer", type: "controls", items: [
                            { name: "NamaProspek", text: "Nama Customer", cls: "span6", required: true, validasi: "required" },
                            { name: "TelpRumah", text: "Telepon", cls: "span2", required: true, validasi: "required" },
                        ]
                    },
                    { name: "AlamatProspek", text: "Alamat", type: "textarea" },
                    {
                        text: "Kota", type: "controls", items: [
                            { name: "CityID", text: "Kode Kota", type: "popup", cls: "span2", click: "City()" },
                            { name: "CityName", text: "Name Kota", cls: "span6", readonly: true }
                        ]
                    },
                    { type: "divider" },
                    {
                        text: "Perusahaan", type: "controls", items: [
                            { name: "NamaPerusahaan", text: "Nama Perusahaan", cls: "span6" },
                            { name: "Jabatan", text: "Jabatan", cls: "span2" },
                        ]
                    },
                    { name: "AlamatPerusahaan", text: "Alamat", type: "textarea" },
                    { name: "Faximile", text: "Faximile", cls: "span4", readonly: false },
                    { name: "Email", text: "Email", cls: "span4", readonly: false },
                ]
            },
            {
                title: "Kendaraan yang dicari",
                items: [

                    {
                        text: "Type / Varian", type: "controls", cls: "span8", items: [
                             { name: "TipeKendaraan", type: "select2", cls: "span3", datasource: "TipeKendaraan" },
                             { name: "Variant", type: "select2", cls: "span3", datasource: "Variant" },
                        ]
                    },
                    {
                        text: "Trans / Warna", type: "controls", items: [
                            { name: "Transmisi", type: "select2", cls: "span3", datasource: "transmissions" },
                            { name: "ColourCode", type: "select2", cls: "span3", datasource: "modelcolors" },
                        ]
                    },
                    {
                        text: "Pembayaran", type: "controls", items: [
                            { name: "CaraPembayaran", type: "select2", cls: "span6", datasource: "CaraPembayaran" },
                        ]
                    },
                    {
                        text: "Nama Leasing", type: "controls", items: [
                            { name: "Leasing", type: "select2", cls: "span6", required: true, validasi: "required", validasi: "required", datasource: "Leasing", disable: "isCash" },
                        ]
                    },
                    {
                        text: "DP (%) - Tenor", type: "controls", items: [
                            { name: "DownPayment", type: "select2", cls: "span3", required: true, validasi: "required", validasi: "required", datasource: "DowmPayment", disable: "isCash" },
                            { name: "Tenor", type: "select2", cls: "span3", required: true, validasi: "required", validasi: "required", datasource: "Tenor", disable: "isCash" },
                        ]
                    },
                    {
                        text: "TestDrive - Quantity", type: "controls", items: [
                            { name: "TestDrive", text: "Test Drive", cls: "span3", type: "select2", datasource: "TestDrive" },
                            { name: "QuantityInquiry", text: "Quantity", cls: "span1", type: "int" },
                        ]
                    },
                ]
            },
            {
                title: "Status Information",
                items: [
                    { name: "LastProgress", text: "Status Terakhir", cls: "span4", type: "select2", required: true, validasi: "required", validasi: "required", datasource: "LastProgress" },
                    { name: "LastUpdateStatus", text: "Updated Date", cls: "span4", type: "ng-datepicker", readonly: true },
                ]
            },
            {
                name: "pnlSpkDetail",
                cls: "hide",
                items: [
                    { name: "SPKDate", text: "Tanggal", cls: "span4", type: "ng-datepicker" },
                    { name: "StatusVehicle", text: "Kategori", cls: "span6", type: "select2", datasource: "StatusVehicle" },
                    {
                        name: "pnlOthersVehicle", text: "Kendaraan yang dimiliki", type: "controls",
                        items: [
                            { name: "BrandCode", text: "Merek", type: "popup", cls: "span2", readonly: true, click: "other()" },
                            { name: "ModelName", text: "Tipe", cls: "span6", readonly: true },
                        ]
                    }
                ]
            },
            {
                name: "pnlLostDetail",
                cls: "hide",
                items: [
                    { name: "LostCaseDate", text: "Tanggal", cls: "span4", type: "ng-datepicker" },
                    { name: "LostCaseCategory", text: "Kategori", cls: "span6", type: "select2", datasource: "LostCaseCategory" },
                    { name: "LostCaseReasonID", text: "Alasan", cls: "span6", type: "select2", datasource: "LostCaseReasonID" },
                    { name: "MerkLain", text: "Merk Lain", cls: "span6" },
                    { name: "LostCaseOtherReason", text: "Alasan Lain", type: "textarea" },
                    { name: "LostCaseVoiceOfCustomer", text: "Voice of Customer", type: "textarea" },
                ]
            },
            {
                name: "pnlFollowUpDtl",
                title: "Follow Up Detail",
                showDivider: true,
                //cls: "hide",
                items: [
                    { name: "ActivityDate", model: "detail.ActivityDate", text: "Tgl Follow Up", cls: "span4", type: "ng-datepicker" },
                    { name: "ActivityType", model: "detail.ActivityType", text: "Jenis Pertemuan", cls: "span4", type: "select2", datasource: "ActivityType" },
                    { name: "ActivityDetail", model: "detail.ActivityDetail", text: "Keterangan", type: "textarea" },
                    { name: "NextFollowUpDate", model: "detail.NextFollowUpDate", text: "Next Follow Up", cls: "span4", type: "ng-datepicker" },
                    { name: "ActivityID", type: "hidden" },
                     {
                         type: "buttons",
                         items: [
                                 { name: "btnAdd", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "saveDtl()", show: "detail.oid === undefined", disable: "data.NamaProspek === undefined" },
                                 { name: "btnUpdate", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "saveDtl()", show: "detail.oid !== undefined" },
                                 { name: "btnDelete", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "deleteDtl()", show: "detail.oid !== undefined" },
                                 { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "ClearDtl()", show: "detail.oid !== undefined" }
                         ]
                     },
                    {
                        title: "Follow Up Detail",
                        name: "wxFUDtl",
                        type: "wxdiv"
                    },
                ]
            },
        ]
    };
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("KDPController");
    }
});