function x() {
    //$('#s2id_NikSales').val(result.data.NikSales);
    //$('#s2id_NikSC').val(result.data.NikSC);
    //$('#s2id_NikSH').val(result.data.NikSH);
    //$('#s2id_Grade').val(result.data.Grade);
    //me.data.NikSales = result.data.NikSales;
    //me.data.NikSC = result.data.NikSC;
    //me.data.NikSH = result.data.NikSH;
    //me.data.BranchName = result.data.BranchName;
    //me.data.NikSales = 52450;
    //me.data.NikSC = 52084;
    //me.data.NikSH = 76;
    //me.data.BranchName = 'PT. BUANA INDOMOBIL TRADA - GADING SERPONG';
    //me.data.Grade = "1";
    //if (result.data.Position == "S") { widget.enable({ value: false, items: ["NikSH", "NikSC", "NikSales"] }); };
    //if (result.data.Position == "SC") { widget.enable({ value: false, items: ["NikSH", "NikSC"] }); };
    //if (result.data.Position == "BM") { widget.enable({ value: false, items: ["NikSH", "NikSC"] }); };
    //if (result.data.Position == "CT") { widget.enable({ value: false, items: ["NikSH", "NikSC"] }); };
    //if (result.data.Position == "SH") {
    //    widget.enable({ value: false, items: ["NikSH"] });
    //    widget.selectparam({ name: "NikSales", url: "its.api/combo/employee", param: "NikSC" });
    //};

    //widget.populate(widget.default, function () {
    //    var selectedIndex = $("[name='NikSales']").prop("selectedIndex");
    //    if (selectedIndex < 0) {
    //        $("[name='NikSales']").prop("selectedIndex", "0");
    //    }

    //    selectedIndex = $("[name='NikSC']").prop("selectedIndex");
    //    if (selectedIndex < 0) {
    //        $("[name='NikSC']").prop("selectedIndex", "0");
    //    }
    //});
}

var pType = "";
var InquiryNumber = "";
var postion = "";
function KDPController($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    me.disableAll = function () {
        var x = 'InquiryNumber,BranchCode,CompanyCode,EmployeeID,SpvEmployeeID,InquiryDate,OutletID,StatusProspek,PerolehanData,NamaProspek,AlamatProspek,TelpRumah,CityID,CityName,NamaPerusahaan,AlamatPerusahaan,Jabatan,Handphone,Faximile,Email,TipeKendaraan,Variant,Transmisi,ColourCode,CaraPembayaran,TestDrive,QuantityInquiry,LastProgress,LastUpdateStatus,SPKDate,LostCaseDate,LostCaseCategory,LostCaseReasonID,LostCaseOtherReason,LostCaseVoiceOfCustomer,LastUpdateBy,LastUpdateDate,Leasing,DownPayment,Tenor,MerkLain,ActivityID,ActivityDate,ActivityType,ActivityDetail,NextFollowUpDate';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', 'disabled');
        }
    }
    me.getUserProperties = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/InquiryIts/itsUserProperties',
            success: function (dt) {
                if (dt.success) {
                    pType = dt.data.ProductType;
                    postion = dt.data.Position;
                } else {
                    MsgBox(dt.message, MSG_ERROR);
                }
            }
        });
    }

    me.getDataKDP = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/grid/InputKDP?InquiryNumber=' + InquiryNumber,
            success: function (dt) {
                if (dt.success) {
                    //console.log(dt.data);
                    if (dt.data[0].CaraPembayaran == "10") { me.isCash = true } else { me.isCash = false }
                    me.Variant = dt.Variant;
                    me.transmissions = [
                           { "value": "AT", "text": "AUTOMATIC TRANSMISSION" },
                           { "value": "MT", "text": "MANUAL TRANSMISSION" }
                    ];
                    me.modelcolors = dt.ModelColour;
                    me.data.Grade = dt.data[0].Grade;
                    me.data.InquiryNumber = dt.data[0].InquiryNumber;
                    me.data.BranchCode = dt.data[0].BranchCode;
                    me.data.BranchName = dt.data[0].BranchName;
                    me.data.CompanyCode = dt.data[0].CompanyCode;
                    me.data.NikSales = dt.data[0].EmployeeID;
                    me.data.NikSalesName = dt.data[0].EmployeeName;
                    me.data.NikSH = dt.data[0].LeaderID;
                    me.data.NikSHName = dt.data[0].LeaderName;
                    me.data.EmployeeID = dt.data[0].EmployeeID;
                    me.data.SpvEmployeeID = dt.data[0].SpvEmployeeID;
                    me.data.InquiryDate = dt.data[0].InquiryDate;
                    me.data.OutletID = dt.data[0].OutletID;
                    me.data.StatusProspek = dt.data[0].StatusProspek;
                    me.data.PerolehanData = dt.data[0].PerolehanData;
                    me.data.NamaProspek = dt.data[0].NamaProspek;
                    me.data.AlamatProspek = dt.data[0].AlamatProspek;
                    me.data.TelpRumah = dt.data[0].TelpRumah;
                    me.data.CityID = dt.data[0].CityID;
                    me.data.CityName = dt.data[0].CityName;
                    me.data.NamaPerusahaan = dt.data[0].NamaPerusahaan;
                    me.data.AlamatPerusahaan = dt.data[0].AlamatPerusahaan;
                    me.data.Jabatan = dt.data[0].Jabatan;
                    me.data.Handphone = dt.data[0].Handphone;
                    me.data.Faximile = dt.data[0].Faximile;
                    me.data.Email = dt.data[0].Email;
                    me.data.TipeKendaraan = dt.data[0].TipeKendaraan;
                    me.data.Variant = dt.data[0].Variant;
                    me.data.Transmisi = dt.data[0].Transmisi;
                    me.data.ColourCode = dt.data[0].ColourCode;
                    me.data.CaraPembayaran = dt.data[0].CaraPembayaran;
                    me.data.TestDrive = dt.data[0].TestDrive;
                    me.data.QuantityInquiry = dt.data[0].QuantityInquiry;
                    me.data.LastProgress = dt.data[0].LastProgress;
                    me.data.LastUpdateStatus = dt.data[0].LastUpdateStatus;
                    me.data.Leasing = dt.data[0].Leasing;
                    me.data.DownPayment = dt.data[0].DownPayment;
                    me.data.Tenor = dt.data[0].Tenor;
                    me.detail.ActivityDate = me.now();
                    me.detail.NextFollowUpDate = me.now();
                    if (dt.data[0].LastProgress == 'LOST') {
                        me.LastProgress = [
                            { "text": "LOST", "value": "LOST" }
                        ];
                        $('#pnlLostDetail').show();
                        me.data.LostCaseDate = dt.data[0].LostCaseDate;
                        me.data.LostCaseCategory = dt.data[0].LostCaseCategory;
                        me.data.LostCaseReasonID = dt.data[0].LostCaseReasonID;
                        me.data.MerkLain = dt.data[0].MerkLain;
                        me.data.LostCaseOtherReason = dt.data[0].LostCaseOtherReason;
                        me.data.LostCaseVoiceOfCustomer = dt.data[0].LostCaseVoiceOfCustomer;
                    }
                    if (dt.data[0].LastProgress == 'SPK') {
                        $('#pnlSpkDetail').show();
                        me.data.SPKDate = dt.data[0].SPKDate;
                        me.data.StatusVehicle = dt.data[0].StatusVehicle;
                        me.data.BrandCode = dt.data[0].OthersBrand;
                        me.data.ModelName = dt.data[0].OthersType;
                    }
                } else {
                    MsgBox(console.log(dt), MSG_ERROR);
                }
            }
        });
    }

    me.initialize = function () {
        me.detail = {};
        me.getUserProperties();
        me.isComboShow = pType === '4W' ? true : false;
        me.isShow = false;
        $http.post('its.api/combo/itsstatus').
        success(function (data, status, headers, config) {
            me.LastProgress = data;
        });
        $('#pnlSpkDetail').hide();
        $('#pnlLostDetail').hide();
        $('#InquiryDate').removeAttr("disabled");
        if (localStorage.getItem('params')) {
            var params = localStorage.getItem('params');
            InquiryNumber = JSON.parse(params).InquiryNumber;
            localStorage.removeItem('params');
            me.getDataKDP();
            me.ClearDtl();
            $('#Variant').attr('disabled', false);
            $('#Transmisi').attr('disabled', false);
            $('#ColourCode').attr('disabled', false);
            $('#btnBrowse').hide();
        }
        else {
            me.hasChanged = false;
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

            me.data.Variant = "";
            me.data.Transmisi = "";
            me.data.ColourCode = "";
            $http.post("its.api/kdp/default?ProductType=" + pType).
             success(function (result, status, headers, config) {
                 if (result.success) {
                     me.data = result.data;
                     me.NikSH = result.data.NikSHList;
                     me.NikSC = result.data.NikSCList;
                     //me.NikSales = result.data.NikSlList;
                     //alert(result.data.NikSH);
                     me.data.LastUpdateStatus = result.data.CurrentDate;
                     me.data.InquiryDate = result.data.CurrentDate;
                     me.data.LastProgress = "P";
                     me.detail.ActivityDate = result.data.CurrentDate;
                     me.detail.NextFollowUpDate = result.data.CurrentDate;
                     me.istrue = true;
                     me.data.BranchCode = result.data.OutletID;
                     //setTimeout(function () {
                     //    $('[name=NikSH]').select2().select2('val', $('[name=NikSH] option:eq(1)').val());
                     //}, 500)
                     if (result.data.NikSales == '') {
                         $('#btnNikSales').removeAttr("disabled");
                         //    $http.post('its.api/kdp/SalesmanID?EmployeeID=' + result.data.NikSH).
                         //   success(function (data, status, headers, config) {
                         //    me.NikSales = data;
                         //});
                     }
                 } else {
                     me.disableAll();
                     MsgBox(result.message);
                 }
                 if (postion == 'BM') {
                     me.data.NikSales = undefined;
                 }
             });

            //$('#TipeKendaraan').on('change', me.ChangeCarType);
            //$('#Variant').on('change', me.ChangeCarVariant);
            //$('#Transmisi').on('change', me.ChangeCarTrans);
        }
        if (postion == 'GM' || postion == 'BM' || postion == 'COO' || postion == 'CEO' ||postion == 'GMBM') {
            me.disableAll();
            $('#btnDuplicateData').attr("disabled", "disabled");
            $('#btnBrowse').attr("disabled", "disabled");
        }

        
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
        console.log(carType);
        me.data.Variant = "";
        //me.transmissions = null;
        me.data.Transmisi = "";
        //me.modelcolors = null;
        me.data.ColourCode = "";
        //me.Apply();
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
                $('#Variant').removeAttr('disabled');
                $('#Transmisi').attr('disabled', 'disabled');
                $('#ColourCode').attr('disabled', 'disabled');
                //me.ChangeCarVariant();
            }
        });
    }

    me.ChangeCarVariant = function () {
        var carType = $('#TipeKendaraan').val();
        var carVariant = $('#Variant').val();
        me.data.Variant = carVariant;
        //me.transmissions = null;
        me.data.Transmisi = "";
        //me.modelcolors = null;
        me.data.ColourCode = "";
        //me.Apply();
        if (carVariant == '') {
            $('#Transmisi').attr('disabled', 'disabled');
            $('#ColourCode').attr('disabled', 'disabled');
            return;
        } else {
            $('#Transmisi').removeAttr('disabled');
            $('#ColourCode').attr('disabled', 'disabled');
        }
        var long = carVariant.length;
        console.log("Ini adalah pangjang saya : " + long);
        var Trans = carVariant.substr((long - 2), 2);
        console.log("Ini adalah Code Transmisinya : " + Trans);
        if (Trans == "AT") {
            me.transmissions = [
                 { "value": "AT", "text": "AUTOMATIC TRANSMISSION" }
            ];
            me.data.Transmisi = "AT";
            $.ajax({
                async: false,
                type: "POST",
                url: 'its.api/combo/modelcolors?CarType=' + carType + '&CarVariant=' + carVariant + '&CarTrans=' + Trans,
                success: function (data) {
                    me.modelcolors = data;
                    //me.Apply();
                    $('#ColourCode').removeAttr('disabled');
                }
            });
        } else if (Trans == "MT") {
            me.transmissions = [
                { "value": "MT", "text": "MANUAL TRANSMISSION" },
            ];
            me.data.Transmisi = "MT";
            $.ajax({
                async: false,
                type: "POST",
                url: 'its.api/combo/modelcolors?CarType=' + carType + '&CarVariant=' + carVariant + '&CarTrans=' + Trans,
                success: function (data) {
                    me.modelcolors = data;
                    //me.Apply();
                    $('#ColourCode').removeAttr('disabled');
                }
            });
        } else {
            me.transmissions = [
                { "value": "AT", "text": "AUTOMATIC TRANSMISSION" },
                { "value": "MT", "text": "MANUAL TRANSMISSION" }
            ];
        }
        
        //$.ajax({
        //    async: false,
        //    type: "POST",
        //    url: 'its.api/combo/transmissions?CarType=' + carType + '&CarVariant=' + carVariant,
        //    success: function (data) {
        //        me.transmissions = data;
        //        //me.Apply();
        //        $('#Transmisi').removeAttr('disabled');
        //        $('#ColourCode').attr('disabled', 'disabled');
        //        //me.ChangeCarTrans();
        //    }
        //});
    }
    
    me.ChangeCarTrans = function () {
        var carType = $('#TipeKendaraan').val();
        var carVariant = $('#Variant').val();
        var carTrans = $('#Transmisi').val();
        me.data.Transmisi = carTrans;
        //me.modelcolors = null;
        me.data.ColourCode = "";
        //me.Apply();
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
                //me.Apply();
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
                //me.transmissions = data.transmissions;
                me.transmissions = [
                    { "value": "AT", "text": "AUTOMATIC TRANSMISSION" },
                    { "value": "MT", "text": "MANUAL TRANSMISSION" }
                ];
                me.modelcolors = data.colors;
                me.data.TipeKendaraan = carType;
                me.data.Variant = carVariant;
                me.data.Transmisi = carTrans;
                me.data.ColourCode = carColor;
                console.log($('#Variant').val());
                console.log(carVariant);
               //console.log(carType);
               //console.log(carVariant);
               //console.log(carTrans);
                $('#ColourCode').val(carColor);
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
                if (data.length > 0) {
                    console.log("Ini panjang array alasan : " + data.length);
                    console.log("Ini first data : " + data[0].value);

                    if (data.length == 1) me.data.LostCaseReasonID = data[0].value;
                    
                }
                
        });
    });

    $('#CaraPembayaran').on('change', function (e) {
        if ($('#CaraPembayaran').val() == '20') {
            me.isCash = false;
        }
        else {
            me.isCash = true;
            me.data.Leasing = "";
            me.data.DownPayment = "";
            me.data.Tenor = "";
        }
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
            me.data.SPKDate = me.now();
        } else { $('#pnlSpkDetail').hide() }

        if ($('#LastProgress').val() == 'DO' || $('#LastProgress').val() == 'DELIVERY') {
            $('#btnSave').hide();            
        } else { $('#btnSave').show(); }
    });

    me.browse = function () {
        //$('#TipeKendaraan').off('change');
        //$('#Variant').off('change');
        //$('#Transmisi').off('change');
        me.data.NikSH = me.data.NikSH == undefined ? "" : me.data.NikSH;
        me.data.NikSales = me.data.NikSales == undefined ? "" : me.data.NikSales;
        //var lookup = Wx.blookup({
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "Inquiry Kdp",
            //manager: MasterITS,
            //query: new breeze.EntityQuery.from('kdplist').withParameters({ NikSH: me.data.NikSH, NikSales: me.data.NikSales }),
            //defaultSort: "InquiryNumber Desc",
            url: "its.api/grid/kdplist",
            params: { name: "controls", items: [{ name: "NikSales", param: "NikSales" }, { name: "NikSC", param: "NikSC" }, { name: "NikSH", param: "NikSH" }] },
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
                $('#InquiryDate').attr("disabled", "disabled");
                $('#btnDuplicateData').removeAttr("disabled");
                me.isEditable = true;
                me.isSave = false;
                me.hasChanged = false;
                if (result.CaraPembayaran == "10") { me.isCash = true } else { me.isCash = false }
                me.data.Grade = result.Grade;
                me.data.InquiryNumber = result.InquiryNumber;
                me.data.BranchCode = result.BranchCode;
                me.data.CompanyCode = result.CompanyCode;
                me.data.NikSales = result.EmployeeID;
                me.data.NikSalesName = result.EmployeeName;
                //me.data.NikSH = result.SpvEmployeeID;
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
                        { "text": "LOST", "value" : "LOST" }
                    ];
                    $('#pnlLostDetail').show();
                    me.data.LostCaseDate = result.LostCaseDate;
                    me.data.LostCaseCategory = result.LostCaseCategory;
                    me.data.LostCaseReasonID = result.LostCaseReasonID;
                    me.data.MerkLain = result.MerkLain;
                    me.data.LostCaseOtherReason = result.LostCaseOtherReason;
                    me.data.LostCaseVoiceOfCustomer = result.LostCaseVoiceOfCustomer;
                }
                me.Apply();
                if (result.LastProgress == 'SPK') {
                    $('#pnlSpkDetail').show();
                    me.data.SPKDate = result.SPKDate;
                    me.data.StatusVehicle = result.StatusVehicle;
                    me.data.BrandCode = result.OthersBrand;
                    me.data.ModelName = result.OthersType;
                }
                me.Apply();
                me.LoadVariantTransColor(result.TipeKendaraan, result.Variant, result.Transmisi, result.ColourCode);
                
                //setTimeout(function () {
                //    me.isEditable = true;
                //    me.isSave = false;
                //    me.hasChanged = false;
                //    me.isLoadData = true;
                //    $scope.$apply();
                //}, 500);
                me.ClearDtl();

            }
        });
    }

    me.Salesman = function () {
        //var lookup = Wx.blookup({
        //    name: "Salesman",
        //    title: "Salesman",
        //    manager: spSalesManager,
        //    query: "Select4LookupSalesman",
        //    defaultSort: "EmployeeID asc",
        var lookup = Wx.klookup({
            name: "btnBrowse",
            title: "Salesman",
            //url: "its.api/grid/SMlist?NikSH=" + me.data.NikSH,
            url: "its.api/grid/SMlist",
            params: { name: "controls", items: [{ name: "NikSH", param: "NikSH" }, { name: "NikSC", param: "NikSC" }] },
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "EmployeeID", title: "ID" },
                { field: "EmployeeName", title: "Salesman" },
                { field: "TitleName", title: "Jabatan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.NikSales = data.EmployeeID;
                me.data.NikSalesName = data.EmployeeName;
                me.Apply();
                $http.post('its.api/combo/Grade?NikSales=' + $('#NikSales').val()).
                success(function (data, status, headers, config) {
                    if (data != '""') {
                        me.data.Grade = data.replace('"','').replace('"','');
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

    me.saveKDP = function (e, param) {
        var msg = "";
        var Field = 'PerolehanData,LastProgress,TipeKendaraan,Variant,Transmisi,ColourCode,CaraPembayaran';
        var Names = 'Perolehan Data,Status Terakhir,Type,Varian,Transmisi,Warna,Pembayaran,Kode Kota,Nama Kota';
        var Name = "";
        var q = Field.split(',', 25);
        var m = Names.split(',', 25);
        var r = q.length; // r panjang field
        var n = m.length; // n panjang nama
        var s = r; // untuk ambil nama

        for (i = 0; i < r; i++) {
            var x = $('#' + q[i]).select2("data").text;
            if (x != '-- SELECT ONE --') {
                s = s - 1;
            } else {
                if (Name == "") {
                    Name = m[i];
                } else { 
                    Name += ', ' + m[i];
                }
            }
        }

        var Field1 = 'NikSales,NamaProspek,TelpRumah,CityID,CityName';
        var Names1 = 'Salesman,Customer,Telepon,Kode Kota,Nama Kota';
        var ret = me.CheckMandatory(Field1, Names1);

        if (ret != "" || Name != "") {
            if (ret != "" && Name != "") msg = ret + ", " + Name + " Harus diisi terlebih dahulu !";
            else if (ret != "" && Name == "") msg = ret + " Harus diisi terlebih dahulu !";
            else if (ret == "" && Name != "") msg = Name + " Harus diisi terlebih dahulu !";
        } else {
            if (me.data.LastProgress == 'LOST') {
                var Field2 = 'LostCaseDate,LostCaseCategory,LostCaseReasonID,LostCaseVoiceOfCustomer';
                var Names2 = 'Tanggal,Kategori,Alasan,Voice of Customer';
                var Namelost = me.CheckMandatory(Field2, Names2);
                if (Namelost != "") msg = Namelost + " Harus diisi terlebih dahulu !";
                console.log("ini lost case category event click : " + $("#LostCaseCategory").val());
                console.log($("#MerkLain").val());

                if ($("#LostCaseCategory").val() == "A") {
                    if ($("#MerkLain").val() == "" || $("#MerkLain").val() == "undefined") {
                        msg ="Merk lain harus diisi terlebih dahulu !";
                    }
                }
                if ($("#LostCaseReasonID").val() == "70" || $("#LostCaseReasonID").select2("data").text == "OTHERS") {
                    if ($("#LostCaseOtherReason").val() == "" || $("#LostCaseOtherReason").val() == "undefined") {
                        msg="Alasan lain harus diisi terlebih dahulu !";
                    }
                }
            } else {
                if (me.data.CaraPembayaran == '20') {
                    if (me.data.Leasing == undefined || me.data.Leasing == "") {
                        msg = "Nama Leasing harus diisi";
                    }
                    else if (me.data.DownPayment == undefined || me.data.DownPayment == "") {
                        msg = "DP (%) harus diisi";
                    }
                    else if (me.data.Tenor == undefined || me.data.Tenor == "") {
                        msg = "Tenor harus diisi";
                    };
                }  
            }
        }

        me.saveHeader(msg);
    }

    me.saveHeader = function (msg) {
        if (msg != "") {
            MsgBox(msg, MSG_INFO);
            return;
        } else {
            me.data.EmployeeID = me.data.NikSales;
            me.data.SpvEmployeeID = me.data.NikSH;
           

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
                    //me.data.InquiryNumber = data.data.InquiryNumber;
                    //$.post("its.api/kdp/get", data.data, populate)
                } else {
                    MsgBox(data.message, MSG_INFO);
                    console.log(data.message);
                   // $http.post('its.api/kdp/save', me.data).
                   // success(function (data, status, headers, config) {
                   //     if (data.success) {
                   //         Wx.Success("Data saved...");
                   //         me.startEditing();
                   //         $('#InquiryNumber').val(data.data.InquiryNumber);
                   //         $('#pnlD').show();
                   //         //me.data.InquiryNumber = data.data.InquiryNumber;
                   //         //$.post("its.api/kdp/get", data.data, populate)
                   //     } else {
                   //         MsgBox(data.message, MSG_INFO);
                   //         console.log(data.message);
                   //     }
                   // }).
                   //error(function (data, status, headers, config) {
                   //    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //});
                }
            }).
           error(function (data, status, headers, config) {
               MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
           });
        }
        
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

    me.LinkDetail = function()
    {
    }

    me.deleteDtl = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result)
            {
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
            if(result)
            {
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
                            MsgBox(result.message, MSG_INFO);
                        }
                    })
                .error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_INFO);
                });
            }
        });     
    }

    me.ClearDtl = function () {
        $http.post('its.api/kdp/Activities?InquiryNumber=' + me.data.InquiryNumber).
              success(function (data, status, headers, config) {
                  me.grid.detail = data;
                  me.loadTableData(me.gridSalesModel, me.grid.detail);

                  //$('#Variant').attr('disabled', 'disabled');
                  //$('#Transmisi').attr('disabled', 'disabled');
                  //$('#ColourCode').attr('disabled', 'disabled');
                 
                  //$('#TipeKendaraan').on('change', me.ChangeCarType);
                  //$('#Variant').on('change', me.ChangeCarVariant);
                  //$('#Transmisi').on('change', me.ChangeCarTrans);

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
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "ActivityDates", header: "Tanggal", width: 150 },
            { id: "ActivityType", header: "Jenis Pertemuan", width: 352, format: me.replaceNull },
            { id: "ActivityDetail", header: "Follow Up Detail", width: 400, format: me.replaceNull },
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

    me.DuplicateData = function () {
        console.log(me.data.InquiryNumber);
        console.log($('#InquiryNumber').val());
        var statusakhir = $('#LastProgress').select2("data").text;
        statusakhir = statusakhir.toLowerCase(); 
        if (me.data.LastProgress == 'P' || me.data.LastProgress == 'HP' || me.data.LastProgress == 'SPK') {
            if ((me.data.InquiryNumber != "" && me.data.InquiryNumber != undefined) || $('#InquiryNumber').val() != '') {
                me.isShow = true;
            } else {
                me.isShow = false;
                MsgBox('Number Inquiry Kosong, Silakan Pilih Terlebih Dahulu Melalui Browse', MSG_INFO);
            }
        } else {
            MsgBox('Data tidak bisa di duplicate karena status sudah ' + statusakhir + '!', MSG_INFO);
        }
    }

    me.Duplicate = function (e, param) {
        if (me.data.Qty != '0' && me.data.Qty != undefined) {
            MsgConfirm("Anda akan duplicate data dengan No Inquiry " + me.data.InquiryNumber + " sebanyak " + me.data.Qty + ". Apakah anda yakin ?", function (result) {
                if(result)
                {
                    $http.post('its.api/kdp/Duplicate', me.data).
                        success(function (data, status, headers, config) {
                            if (data.success) {
                                //MsgBox(data.message, MSG_INFO);
                                Wx.Success(data.message);
                                me.isShow = false;
                                me.startEditing();
                            } else {
                                MsgBox(data.message, MSG_INFO);
                                console.log(data.message);
                            }
                        }).
                        error(function (data, status, headers, config) {
                            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                        });
                }
            });
        } else {
            MsgBox('Jumlah Copy Tidak Boleh Kosong Atau Nol (0)');
        }
    }

    me.start();
}

$(document).ready(function () {
    //var widget = new SimDms.Widget({
    //    title: "Kartu Data Prospek",
    //    xtype: "panels",
    //    toolbars: [
    //        { name: "btnNew", text: "New", icon: "icon-file" },
    //        { name: "btnBrowse", text: "Browse", icon: "icon-search" },
    //        { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
    //        { name: "btnSave", text: "Save", icon: "icon-save" },
    //        { name: "btnCancel", text: "Cancel", icon: "icon-undo", cls: "hide" },
    //    ],
    var options = {
        title: "Kartu Data Prospek",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "saveKDP()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
        ],
        //toolbars: WxButtons,
        panels: [
            {
                items: [
                     { name: "Qty", text: "Jumlah Copy", cls: "span4 full", show: "isShow" },
                     {
                         type: "buttons",
                         cls: "span4 full",
                         items: [
                             { name: "btnDuplicate", text: "Duplicate", cls: "btn btn-success", click: "Duplicate()", show: "isShow" },
                         ]
                     },
                     {
                         type: "buttons",
                         cls: "span4 full",
                         items: [
                            { name: "btnDuplicateData", text: "Duplicate Data", cls: "btn btn-info", click: "DuplicateData()", show: "!isShow" },
                         ]
                     },
                ]
            },
            {
                title: "Data Salesman",
                items: [
                    { name: "OutletID", text: "", type: "hidden", cls: "span8", readonly: true },
                    { name: "BranchName", text: "Outlet", cls: "span8", readonly: true },
                    { name: "InquiryNumber", text: "Nomor Inquiry", cls: "span4", readonly: true },
                    { name: "InquiryDate", text: "Tanggal Inquiry", type: "ng-datepicker", cls: "span4" },
                     {
                         text: "Sales Head", type: "controls", items: [
                             { name: "NikSH", text: "Sales Head", cls: "span4", type: "popup", cls: "span2", show: "isComboShow", click: "Salesman()", disable: true },
                             { name: "NikSHName", text: "Sales Head Name", cls: "span6", readonly: true }
                         ]
                     },
                     {
                         text: "Sales Koordinator", show: "!isComboShow", type: "controls", items: [
                             { name: "NikSC", text: "Sales Koordinator", cls: "span4", type: "popup", cls: "span2", show: "!isComboShow", click: "Salesman()", disable: true },
                             { name: "NikSCName", text: "Sales Koordinator Name", cls: "span6", readonly: true, show: "!isComboShow", }
                         ]
                     },
                     {
                         text: "Salesman", type: "controls", items: [
                             { name: "NikSales", text: "Sales ID", type: "popup", cls: "span2", click: "Salesman()", disable: true },
                             { name: "NikSalesName", text: "Sales Name", cls: "span6", readonly: true }
                         ]
                     },
                    { name: "Grade", text: "Salesman Grade", cls: "span4", readonly: true, type: "select2", datasource: "Grade", disable: true },
                    //{ name: "NikSC", text: "Sales Koordinator", cls: "span4", readonly: true, type: "select2", datasource: "NikSC", show: "!isComboShow", disable: true },
                    { name: "PerolehanData", text: "Perolehan Data", type: "select2", cls: "span4", required: true, validasi: "required", validasi: "required", datasource: "PerolehanData" },
                    { name: "StatusProspek", model: "data.StatusProspek", text: "Status Inquiry", type: "select2", cls: "span4", datasource: "StatusProspek" },
                    //{ name: "NikSales", text: "Salesman", cls: "span4", readonly: true, type: "select2", required: true, validasi: "required", datasource: "NikSales" },
                    { name: "BranchCode", model: "data.BranchCode", type: "hidden" },
                    { name: "EmployeeID", model: "data.EmployeeID", type: "hidden" },
                    { name: "SpvEmployeeID", model: "data.SpvEmployeeID", type: "hidden" },
                    { name: "Grade", type: "hidden" },
                ]
            },
            {
                title: "Data Prospek",
                required: true,
                items: [
                    {
                        text: "Customer", type: "controls", items: [
                            { name: "NamaProspek", text: "Nama Customer", cls: "span6", required: true, validasi: "required" },
                            { name: "TelpRumah", text: "Telepon", cls: "span2", required: true, validasi: "required" },
                        ]
                    },
                    { name: "AlamatProspek", text: "Alamat", type: "textarea" },
                    {
                        text: "Kota", type: "controls", required: true, validasi: "required", items: [
                            { name: "CityID", text: "Kode Kota", type: "popup", cls: "span2", click: "City()", required: true, validasi: "required" },
                            { name: "CityName", text: "Nama Kota", cls: "span6", readonly: true }
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
                            { name: "TipeKendaraan", type: "select2", cls: "span3", datasource: "TipeKendaraan", change: "ChangeCarType()" },
                            { name: "Variant", type: "select2", cls: "span3", datasource: "Variant", change: "ChangeCarVariant()" },
                        ]
                    },
                    {
                        text: "Trans / Warna", type: "controls", items: [
                            { name: "Transmisi", type: "select2", cls: "span3", datasource: "transmissions", change: "ChangeCarTrans()" },
                            { name: "ColourCode", type: "select2", cls: "span3", datasource: "modelcolors" },
                        ]
                    },
                    {
                        text: "Pembayaran", type: "controls", items: [
                            { name: "CaraPembayaran", type: "select2", cls: "span6", datasource: "CaraPembayaran" },
                        ]
                    },
                    {
                        text: "Nama Leasing", type: "controls", required: true, validasi: "required", items: [
                            { name: "Leasing", type: "select2", cls: "span6", required: true, validasi: "required", validasi: "required", datasource: "Leasing", disable: "isCash" },
                        ]
                    },
                    {
                        text: "DP (%) - Tenor", type: "controls", required: true, validasi: "required", items: [
                            { name: "DownPayment", type: "select2", cls: "span3", required: true, validasi: "required", validasi: "required", datasource: "DowmPayment", disable: "isCash" },
                            { name: "Tenor", type: "select2", cls: "span3", required: true, validasi: "required", validasi: "required", datasource: "Tenor", disable: "isCash" },
                        ]
                    },
                    {
                        text: "TestDrive - Quantity", type: "controls", items: [
                            { name: "TestDrive", text: "Test Drive", cls: "span3", type: "select2", datasource: "TestDrive" },
                            { name: "QuantityInquiry", text: "Quantity", cls: "span1", type: "int", readonly: true },
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
                            { name: "BrandCode", text: "Merek", type: "popup", cls: "span2", readonly: true, click : "other()" },
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
                title : "Follow Up Detail",
                showDivider: true,
                //cls: "hide",
                items: [
                    { name: "ActivityDate", model: "detail.ActivityDate", text: "Tgl Follow Up", cls: "span4", type: "ng-datepicker"},//, required: true, validasi: "required" },
                    { name: "ActivityType", model: "detail.ActivityType", text: "Jenis Pertemuan", cls: "span4", type: "select2", datasource: "ActivityType"}, //required: true, validasi: "required", validasi: "required" },
                    { name: "ActivityDetail", model: "detail.ActivityDetail", text: "Keterangan", type: "textarea"}, //required: true, validasi: "required" },
                    { name: "NextFollowUpDate", model: "detail.NextFollowUpDate", text: "Next Follow Up", cls: "span4", type: "ng-datepicker"}, //required: true, validasi: "required" },
                    //{ type: "buttons", items: [{ name: "btnSaveDtl", text: "Save" }, { name: "btnCancelDtl", text: "Cancel" }] },
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

