"use strict"
var status = 0;
var userId = '';

function omRevisiPermohonanFakturPolisController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $("[name='FakturPolisiCity']").on('blur', function () {
        if (me.data.FakturPolisiCity != null) {
            $http.post('om.api/FakturPolisiRevisi/LookUpDtls', { CodeID: 'CITY', lookupvalue: me.data.FakturPolisiCity }).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.FakturPolisiCityName = data.data.LookUpValueName;
                   }
                   else {
                       me.data.FakturPolisiCity = "";
                       me.data.FakturPolisiCityName = "";
                       $("[name='FakturPolisiCity']").val("");
                       $("[name='FakturPolisiCityName']").val("");
                       me.FakturPolisiCity();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    $("[name='RevisionCode']").on('blur', function () {
        if (me.data.RevisionCode != null) {
            $http.post('om.api/FakturPolisiRevisi/LookUpDtls', { CodeID: 'REVI', lookupvalue: me.data.RevisionCode }).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.RevisionName = data.data.LookUpValueName;
                       if ($("[name='RevisionCode']").val() != "RC01") {
                           EditDetail();
                       } else {
                           UnEditDetail();
                       }
                   }
                   else {
                       me.data.RevisionCode = "";
                       me.data.RevisionName = "";
                       $("[name='RevisionCode']").val("");
                       $("[name='RevisionName']").val("");
                       me.RevisionCode();
                   }
               }).
               error(function (data, status, headers, config) {
                   alert('error');
               });
        }
    });

    function EditDetail() {
        $('#btnRevisionCode').removeAttr('disabled');
        $('#RevisionCode').removeAttr('disabled');
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
        //$('#btnRevisionCode').attr('disabled', 'disabled');
        $('#btnFakturPolisiCity').attr('disabled', 'disabled');
        $('#btnPostalCode').attr('disabled', 'disabled');
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
        //$('#ChassisCode').attr('disabled', 'disabled');
        $('#ChassisNo').attr('disabled', 'disabled');
        $('#RevisionDate').attr('disabled', 'disabled');
    }

    me.initialize = function () {
        me.isStatus = true;
        me.data.StatusFaktur = true;
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
        me.data.FakturPolisiDate = me.now();
        me.data.FakturPolisiBirthday = me.now();
        me.isPrintAvailable = true;
        me.isApprove = true;
        me.isStatusCh = false;
        me.isDelete = false;
        //$('#PostalCode').removeAttr('disabled');
    }

    me.browse = function () {
        UnEditDetail();
        var lookup = Wx.blookup({
            name: "PermohonanBrowse",
            title: "Revisi Faktur Polisi",
            manager: spSalesManager,
            query: "RevisiPermohonanBrowse",
            defaultSort: "RevisionNo desc",
            columns: [
                { field: "RevisionNo", title: "No. Revisi" },
                { field: "ChassisCode", title: "Chassis Code" },
                { field: "ChassisNo", title: "Chassis No" },
                {
                    field: "RevisionDate", title: "Tgl",
                    template: "#= (RevisionDate == undefined) ? '' : moment(RevisionDate).format('DD MMM YYYY') #"
                },
                //{ field: "Faktur", title: "Status Faktur" },
                //{ field: "SubDealerCode", title: "Sub Dealer" },
                { field: "CustomerName", title: "Customer Name" },
                { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.status = data.Status;
                $('#Status').html(data.Status);
                me.lookupAfterSelect(data);
                me.data.StatusFaktur = true;
                data.StatusFaktur === "1" ? $('#StatusFaktur').prop('checked', true) : $('#StatusFaktur').prop('checked', false);
                status = data.Stat;
                me.isStatus = status == 2;
                me.isStatusCh = true;
                me.isStatusC = true;
                
                switch (data.Stat) {
                    case "1":
                        me.isApprove = false;
                        me.isStatus = true;
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
                        break;
                    case "3":
                        $('#Remark').attr('disabled', true);
                        break;
                    default:
                        me.isApprove = false;
                        me.hasChanged = false; //browse
                        me.isPrintAvailable = true; //print
                        me.isLoadData = true; //print
                        me.isDelete = true; //Delete
                        me.isInitialize = false;
                        $('#btnDelete').show();
                        $('#btnCancel').show();
                        $('#btnAddDetail').show();
                }
                if (data.RevisionCode != "RC01" && data.Stat == "0") {
                    EditDetail();
                }
                me.Apply();
                console.log("Data Status: " + data.Stat);
                console.log("is Status: " + me.isStatus);
                console.log("Print: " + me.isPrintAvailable);
                console.log("Data Load: " + me.isLoadData);
                console.log(data);
            }
        });
    };

    me.ChassisCode = function () {
        var lookup = Wx.blookup({
            name: "PermohonanBrowse",
            title: "Look Up Kode Rangka",
            manager: spSalesManager,
            query: "ChassisCode4Rev",
            defaultSort: "ReqNo desc",
            columns: [
                { field: "ReqNo", title: "No. Permohonan" },
                { field: "ChassisCode", title: "Chassis Code" },
                { field: "ChassisNo", title: "Chassis No" },
                {
                    field: "ReqDate", title: "Tgl",
                    template: "#= (ReqDate == undefined) ? '' : moment(ReqDate).format('DD MMM YYYY') #"
                },
                { field: "Faktur", title: "Status Faktur" },
                //{ field: "SubDealerCode", title: "Sub Dealer" },
                { field: "CustomerName", title: "Customer Name" },
                { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.status = data.Status;
                $('#Status').html("NEW");
                $http.post('om.api/FakturPolisiRevisi/getDataAkhir', { ChassisCode: data.ChassisCode, ChassisNo : data.ChassisNo })
                .success(function (e) {
                    if (e.success && e.dataAkhir != null) {
                        //me.lookupAfterSelect(data);
                        //me.StatusFaktur = true;
                        me.data.ReqNo = data.ReqNo;
                        me.data.ChassisCode = e.dataAkhir.ChassisCode;
                        me.data.ChassisNo = e.dataAkhir.ChassisNo;
                        me.data.FakturPolisiName = e.dataAkhir.FakturPolisiName;
                        me.data.FakturPolisiAddress1 = e.dataAkhir.FakturPolisiAddress1;
                        me.data.FakturPolisiAddress2 = e.dataAkhir.FakturPolisiAddress2;
                        me.data.FakturPolisiAddress3 = e.dataAkhir.FakturPolisiAddress3;
                        me.data.FakturPolisiCity = e.dataAkhir.FakturPolisiCity;
                        me.data.FakturPolisiCityName = e.FakturPolisiCityName;
                        me.data.PostalCode = e.dataAkhir.PostalCode;
                        me.data.PostalCodeDesc = e.dataAkhir.PostalCodeDesc;
                        me.data.FakturPolisiTelp1 = e.dataAkhir.FakturPolisiTelp1;
                        me.data.FakturPolisiTelp2 = e.dataAkhir.FakturPolisiTelp2;
                        me.data.FakturPolisiHP = e.dataAkhir.FakturPolisiHP;
                        me.data.FakturPolisiBirthday = e.dataAkhir.FakturPolisiBirthday;
                        me.data.IDNo = e.dataAkhir.IDNo;

                        data.StatusFaktur === "1" ? $('#StatusFaktur').prop('checked', true) : $('#StatusFaktur').prop('checked', false);
                        status = data.Stat;
                        me.isStatus = status == 2;
                        $('#btnRevisionCode').removeAttr('disabled');
                        me.hasChanged = true;
                        me.isInitialize = false;

                        if (e.dataAkhir.Status == "0") {
                            me.data.RevisionNo = e.dataAkhir.RevisionNo;
                            me.data.RevisionCode = e.dataAkhir.RevisionCode;
                            me.data.RevisionDate = e.dataAkhir.RevisionDate;
                            me.data.RevisionName = e.RevisionName;
                            if (e.dataAkhir.RevisionCode != "RC01") {
                                EditDetail();
                            }
                        }
                        console.log("Data Status: " + data.Stat);
                        console.log("is Status: " + me.isStatus);
                        console.log("Print: " + me.isPrintAvailable);
                        console.log("Data Load: " + me.isLoadData);
                       
                   } else {
                        me.lookupAfterSelect(data);
                        me.data.ReqNo = data.ReqNo;
                        data.StatusFaktur === "1" ? $('#StatusFaktur').prop('checked', true) : $('#StatusFaktur').prop('checked', false);
                        status = data.Stat;
                        me.isStatus = status == 2;
                        $('#btnRevisionCode').removeAttr('disabled');
                        me.hasChanged = true;
                        me.isInitialize = false;
                        
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
               });
                me.Apply();
            }
        });
    }; //udah

    me.RevisionCode = function () {
        var lookup = Wx.blookup({
            name: "Revision",
            title: "Revision Type",
            manager: spSalesManager,
            query: "Revision",
            defaultSort: "SeqNo asc",
            columns: [
                { field: "LookUpValue", title: "Kode Revisi" },
                { field: "LookUpValueName", title: "Deskripsi" },
            ]
        });
        lookup.dblClick(function (data) {
            me.data.RevisionCode = data.LookUpValue;
            me.data.RevisionName = data.LookUpValueName;
            if (data.LookUpValue == "RC01") {
                UnEditDetail();
            } else {
                EditDetail();
            }
            me.hasChanged = true;
            me.isApprove = false;
            me.Apply();
        });
    } //udah

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
            me.data.FakturPolisiName = data.CustomerName;
            me.data.FakturPolisiAddress1 = data.Address1;
            me.data.FakturPolisiAddress2 = data.Address2;
            me.data.FakturPolisiAddress3 = data.Address3;
            me.data.FakturPolisiCity = data.CityCode;
            me.data.FakturPolisiCityName = data.CityName;
            me.Apply();
        });
    } //tidak di pake

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
            me.data.FakturPolisiCity = data.LookUpValue;
            me.data.FakturPolisiCityName = data.LookUpValueName;
            me.Apply();
        });
    } //udah

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
                me.data.PostalCode = data.ZipCode;
                me.data.PostalCodeDesc = data.KelurahanDesa;
                me.Apply();
            }
        });
    } //udah

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
        $http.post('om.api/FakturPolisiRevisi/preprint', me.data)
       .success(function (e) {
           if (e.success) {
               $('#Status').html(e.Status);
               if (e.stat == "1") { $('#btnApprove').removeAttr('disabled'); }
               me.Print();
               UnEditDetail();
               me.isStatus = true;
               me.isDelete = false;
               userId = e.Userid;
               $('#btnRevisionCode').attr('disabled', 'disabled');
               $('#RevisionCode').attr('disabled', 'disabled');
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.Print = function () {
        var ReportId = 'OmTrSalesFPolRevision002';
        if (me.data.RevisionCode != 'RC01') {
            ReportId = 'OmTrSalesFPolRevision001';
        }

        var par = me.data.RevisionNo + ' ' + userId;
        var rparam = 'Print Revisi Faktur Polisi'
        
        Wx.showPdfReport({
            id: ReportId,
            pparam: par,
            rparam: rparam,
            type: "devex"
        });
    }

    me.save = function (e, param) {
        $http.post('om.api/FakturPolisiRevisi/Save', { model: me.data, reqno: me.data.ReqNo }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.hasChanged = false; //browse
                        me.isPrintAvailable = true; //print
                        me.isLoadData = true; //print
                        me.isDelete = true; //Delete
                        if (data.data.RevisionCode != "RC01" && data.data.Status == "0") {
                            EditDetail();
                        } else {
                            UnEditDetail();
                        }
                        var strStatus = data.data.Status == "0" ? 'OPEN' : 'NEW';
                        $('#Status').html(strStatus);
                        $('#RevisionNo').val(data.data.RevisionNo);
                        me.data.RevisionNo = data.data.RevisionNo;
                        me.data.RevisionDate = data.data.RevisionDate;
                        me.data.Status = data.data.Status;
                        //me.Apply();
                        me.isStatusCh = true;
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
    }

    me.cancelOrClose = function () {
        me.data = {};
        me.data.FakturPolisiDate = me.now();
        me.data.FakturPolisiBirthday = me.now();
        UnEditDetail();
        me.isDelete = false;
        me.hasChanged = false;
        me.isPrintAvailable = false;
        me.isInitialize = true;
        me.data.StatusFaktur = true;
        me.isStatusCh = false;
        me.isStatus = true;
        $('#Status').html("NEW");
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Revisi Faktur Polisi",
        xtype: "panels",
        //toolbars: WxButtons,
        toolbars: [
                    //{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search",  click: "browse()" },
                    //{ name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", click: "delete()" },
                    ////{ name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", click: "save()", disable: "!isSave" },
                    //{ name: "btnCancel", text: "Close", cls: "btn btn-warning", icon: "icon-close", click: "cancelOrClose()" },
                    //{ name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || isInitialize", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isDelete", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable && isLoadData", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlPermohonan",
                items: [
                    { name: "RevisionNo", model: "data.RevisionNo", text: "No.Revisi", cls: "span3", placeHolder: "RTS/YY/XXXXXX", disable: true },
                    {
                        type: "controls",
                        text: "Tgl. Revisi",
                        cls: "span5",
                        items: [
                            { name: "RevisionDate", model: "data.RevisionDate", type: "ng-datepicker", cls: "span3", disable: "isStatus" },
                            { name: "Status", text: "", cls: "span5 right", readonly: true, type: "label" },
                        ]
                    },
                  
                    { type: "hr" },
                    { name: "ChassisCode", model: "data.ChassisCode", text: "Kode Rangka", cls: "span3", disable: "isStatusCh", type: "popup", click: "ChassisCode()", required: true, validasi: "required" },
                    { name: "ChassisNo", model: "data.ChassisNo", text: "No Rangka", cls: "span3", disable: true, type: "popup", click: "ChassisNo()", required: true, validasi: "required" },
                    {
                        text: "Format Revisi",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        show: "data.StatusFaktur == true",
                        items: [
                            { name: "RevisionCode", model: "data.RevisionCode", cls: "span2", disable: "isStatus", required: true, validasi: "required", type: "popup", click: "RevisionCode()", show: "data.StatusFaktur == true" },
                            { name: "RevisionName", model: "data.RevisionName", cls: "span6", readonly: true, disable: "isStatus", show: "data.StatusFaktur == true" }
                        ]
                    },
                    { name: "FakturPolisiName", model: "data.FakturPolisiName", text: "Faktur Atas Nama", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiAddress1", model: "data.FakturPolisiAddress1", text: "Alamat", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiAddress2", model: "data.FakturPolisiAddress2", text: "", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiAddress3", model: "data.FakturPolisiAddress3", text: "", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    {
                        text: "Kota",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        show: "data.StatusFaktur == true",
                        items: [
                            { name: "FakturPolisiCity", model: "data.FakturPolisiCity", cls: "span2", disable: "isStatus", required: true, validasi: "required", type: "popup", click: "FakturPolisiCity()", show: "data.StatusFaktur == true" },
                            { name: "FakturPolisiCityName", model: "data.FakturPolisiCityName", cls: "span6", readonly: true, disable: "isStatus", show: "data.StatusFaktur == true" }
                        ]
                    },
                    {
                        text: "Kode Pos",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        show: "data.StatusFaktur == true",
                        items: [
                            { name: "PostalCode", model: "data.PostalCode", cls: "span2", disable: "isStatus", required: true, validasi: "required", type: "popup", click: "PostalCode()", show: "data.StatusFaktur == true" },
                            { name: "PostalCodeDesc", model: "data.PostalCodeDesc", cls: "span6", readonly: true, disable: "isStatus", show: "data.StatusFaktur == true" }
                        ]
                    },
                    {
                        text: "No Telp",
                        type: "controls",
                        required: true,
                        cls: "span6",
                        show: "data.StatusFaktur == true",
                        items: [
                            { name: "FakturPolisiTelp1", model: "data.FakturPolisiTelp1", text: "SKPK No.Telp", cls: "span4", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                            { name: "FakturPolisiTelp2", model: "data.FakturPolisiTelp2", text: "SKPK No.Telp", cls: "span4", disable: "isStatus", readonly: true, show: "data.StatusFaktur == true" },
                        ]
                    },
                    { name: "FakturPolisiHP", model: "data.FakturPolisiHP", text: "No HP", cls: "span3", required: true, validasi: "required", disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: "FakturPolisiBirthday", model: "data.FakturPolisiBirthday", text: "Tgl Lahir", cls: "span3", type: 'ng-datepicker', disable: "isStatus", show: "data.StatusFaktur == true" },
                    { name: "IDNo", model: "data.IDNo", text: "No Identitas", cls: "span6", disable: "isStatus", required: true, validasi: "required", show: "data.StatusFaktur == true" },
                    //{ name: "ReasonCode", model: "data.ReasonCode", cls: "span3", type: "select2", datasource: "ReasonCode", text: "Reason Code", disable: "isStatus", show: "data.StatusFaktur == false" },
                    //{ name: "ReasonDesc", model: "data.ReasonDesc", text: "Reason Description", cls: "span5", disable: "isStatus", show: "data.StatusFaktur == false" },
                    { type: "hr" },
               ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        SimDms.Angular("omRevisiPermohonanFakturPolisController");
    }



});