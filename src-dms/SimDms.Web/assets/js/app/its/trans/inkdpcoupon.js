var isCEO = false;
var empID = "";
var empName = "";
var SISId = "";
var IdentityNo = "";
var Position = "";
var pType = "";
var outletID = "";
var outletName = "";
var branch = "";
"use strict";

function itsInputKDPCoupon($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.getUserProperties = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/Coupon/itsUserDefault',
            success: function (dt) {
                if (dt.success) {
                    isCEO = dt.data.isCOO;
                    empID = dt.data.EmployeeID;
                    empName = dt.data.EmployeeName;
                    SISId = dt.data.SISID;
                    IdentityNo = dt.data.IdentityNo;
                    Position = dt.data.Position;
                    outletID = dt.data.outletID;
                    outletName = dt.data.outletName;
                    branch = dt.data.Branch;
                } else {
                    MsgBox(dt.message, MSG_INFO);
                }
            }
        });
    }

    me.disableAll = function () {
        var x = 'InquiryNumber,CoupunNumber,NamaProspek,ProspekIdentityNo,TelpRumah,Email,AlamatProspek,TestDriveDate,OutlatName,EmployeeID,EmployeeName,SalesID,IdentityNo,Remark';
        var y = x.split(',', 60);
        var z = y.length;
        for (i = 0; i <= z; i++) {
            $('#' + y[i]).attr('disabled', 'disabled');
        }
    }

    me.initialize = function () {
        me.data = {};
        me.getUserProperties();
        var date = new Date;
        me.hasChanged = false;
        me.data.TestDriveDate = date;
        if (Position == "S") {
            me.data.EmployeeName = empName;
            me.data.EmployeeID = empID;
            me.data.SalesID = SISId;
            me.data.IdentityNo = IdentityNo;
            me.data.OutlatName = outletName;
        } else {
            me.data.OutlatName = outletName;
        }
        $('#btnInquiryNumber').removeAttr("disabled");
        $('#InquiryNumber').removeAttr("disabled");
        $('#CoupunNumber').removeAttr("disabled");
        //if (Position == 'GM' || Position == 'BM' || Position == 'COO' || Position == 'CEO' || Position == 'GMBM') {
        //    me.disableAll();
        //    $('#btnBrowse').attr("disabled", "disabled");
        //    $('#btnInquiryNumber').attr("disabled", "disabled");
        //}
    }

    me.browse = function () {
        var lookup = Wx.klookup({
            name: "btnbrowse",
            title: "Coupon Test Drive",
            url: "its.api/Coupon/browse",
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "InquiryNumber", title: "No.Inquiry" },
                { field: "CoupunNumber", title: "No.Kupon" },
                { field: "NamaProspek", title: "Pelanggan" },
                { field: "EmployeeName", title: "Saleman" },
                {
                    field: "TestDriveDate", title: "Tgl Test Drive"
                    , template: "#= (TestDriveDate == undefined) ? '' : moment(TestDriveDate).format('DD MMM YYYY') #"
                },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                $('#InquiryNumber').attr('disabled', 'disabled');
                $('#CoupunNumber').attr('disabled', 'disabled');
                me.data.InquiryNumber = data.InquiryNumber;
                me.data.CoupunNumber = data.CoupunNumber;
                me.data.NamaProspek = data.NamaProspek;
                me.data.ProspekIdentityNo = data.ProspekIdentityNo;
                me.data.TelpRumah = data.TelpRumah;
                me.data.AlamatProspek = data.AlamatProspek;
                me.data.TestDriveDate = data.TestDriveDate;
                me.data.Email = data.Email;
                me.data.EmployeeName = data.EmployeeName;
                me.data.EmployeeID = data.EmployeeID;
                me.data.SalesID = data.SalesID;
                me.data.IdentityNo = data.IdentityNo;
                me.data.Remark = data.Remark;
                me.Apply();
            }
        });
    }

    me.InquiryNumber = function () {
        me.initialize();
        var lookup = Wx.klookup({
            name: "btnInquiryNumber",
            title: "Inquiry Number",
            url: "its.api/Coupon/InqNumber",
            pageSize: 10,
            serverBinding: true,
            columns: [
                { field: "InquiryNumber", title: "Inquiry Number" },
                {
                    field: "InquiryDate", title: "Inquiry Date"
                    , template: "#= (InquiryDate == undefined) ? '' : moment(InquiryDate).format('DD MMM YYYY') #"
                },
                { field: "NamaProspek", title: "Pelanggan" },
                { field: "EmployeeName", title: "Saleman" },
                { field: "TipeKendaraan", title: "Tipe Kendaraan" },
                { field: "Variant", title: "Variant" },
                { field: "TestDrive", title: "TestDrive" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.InquiryNumber = data.InquiryNumber;
                me.data.NamaProspek = data.NamaProspek;
                me.data.TelpRumah = data.TelpRumah;
                me.data.AlamatProspek = data.AlamatProspek;
                me.data.TestDriveDate = data.InquiryDate;
                me.data.EmployeeName = data.EmployeeName;
                me.data.EmployeeID = data.EmployeeID;
                me.data.SalesID = data.SalesID;
                me.data.IdentityNo = data.IdentityNo;
                me.Apply();
            }
        });
    }

    me.saveData = function (e, param) {
        //if (me.data.CoupunNumber == undefined || me.data.CoupunNumber == "") {
        //    MsgBox("No Kupon Wajib diisi", MSG_INFO);
        //}
        //else if (me.data.ProspekIdentityNo == undefined || me.data.ProspekIdentityNo == "") {
        //    MsgBox("No SIM A Wajib diisi", MSG_INFO);
        //}
        //else {
            $http.post('om.api/Coupon/Save', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.startEditing();

                    } else {
                        MsgBox(data.message, MSG_INFO);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox(e.message, MSG_ERROR);
                    console.log(e);
                });
        //}
    }

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Test Drive Coupon",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
        ],
        //toolbars: WxButtons,
        panels: [
            {
                name: "Coupon",
                items: [
                        { name: "InquiryNumber", cls: "span3 full", type: "popup", text: "No.Inquiry", click: "InquiryNumber()", required: true, validasi: "required" },
                        { name: "CoupunNumber", cls: "span3 first-zero-number-only full", text: "No.Kupon", maxlength: 15, required: true, validasi: "required" },
                        { name: "NamaProspek", cls: "span4", text: "Nama", disable : true },
                        { name: "ProspekIdentityNo", cls: "span4", text: "No.SIM A", required: true, validasi: "required" },
                        { name: "TelpRumah", cls: "span4", text: "Telepon/HP", disable: true },
                        { name: "Email", cls: "span4", text: "Alamat Email", maxlength: 50 },
                        { name: "AlamatProspek", cls: "span8 full", text: "Alamat", disable: true },
                        { name: "TestDriveDate", text: "Tanggal Test Drive", type: "ng-datepicker", cls: "span4 full" },
                        { type: "hr" },
                        { name: "OutlatName", cls: "span4 full", text: "Nama Dealer", disable: true },
                        { name: "EmployeeID", cls: "span4", text: "NIK Salesman", disable: true },
                        { name: "EmployeeName", cls: "span4", text: "Nama Saleman", disable: true },
                        { name: "SalesID", cls: "span4", text: "ID SIS Salesman", disable: true },
                        { name: "IdentityNo", cls: "span4", text: "No.KTP Salesman", disable: true },
                        { name: "Remark", cls: "span8 full", text: "Keterangan", maxlength: 100 },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("itsInputKDPCoupon");
    }

});