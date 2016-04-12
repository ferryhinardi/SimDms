var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function PrintCustomerVehicleController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.ChassisCode = function () {
        var lookup = Wx.blookup({
            name: "ChassisCode4Lookup",
            title: "Kode Rangka",
            manager: MasterService,
            query: "ChassisCode4Lookup",
            defaultSort: "ChassisCode",
            columns: [
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "ChassisNo", title: "No Rangka" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ChassisCode = data.ChassisCode;
                me.data.ChassisNo = data.ChassisNo;
                me.Apply();
            }
        });
    }

    me.EngineCode = function () {
        var lookup = Wx.blookup({
            name: "EngineCode4Lookup",
            title: "Kode Mesin",
            manager: MasterService,
            query: "EngineCode4Lookup",
            defaultSort: "EngineCode",
            columns: [
                { field: "EngineCode", title: "Kode Mesin" },
                { field: "EngineNo", title: "No Mesin" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.EngineCode = data.EngineCode;
                me.data.EngineNo = data.EngineNo;
                me.Apply();
            }
        });
    }

    me.BasicModel = function () {
        var lookup = Wx.blookup({
            name: "BasicModel4Lookup",
            title: "Basic Model",
            manager: MasterService,
            query: "BasicModel4Lookup",
            defaultSort: "BasicModel",
            columns: [
                { field: "BasicModel", title: "Basic Model" },
                { field: "TechnicalModelCode", title: "Kode Technical Model" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BasicModel = data.BasicModel;
                me.Apply();
            }
        });
    }

    me.ServiceBookNo = function () {
        var lookup = Wx.blookup({
            name: "EngineCode4Lookup",
            title: "No Buku Service",
            manager: MasterService,
            query: "EngineCode4Lookup",
            defaultSort: "ServiceBookNo",
            columns: [
                { field: "ServiceBookNo", title: "No Buku Service" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.ServiceBookNo = data.ServiceBookNo;
                me.Apply();
            }
        });
    }

    me.PoliceRegNo = function () {
        var lookup = Wx.blookup({
            name: "EngineCode4Lookup",
            title: "No Polisi",
            manager: MasterService,
            query: "EngineCode4Lookup",
            defaultSort: "PoliceRegNo",
            columns: [
                { field: "PoliceRegNo", title: "No Polisi" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PoliceRegNo = data.PoliceRegNo;
                me.Apply();
            }
        });
    }

    me.CustomerCode = function () {
        var lookup = Wx.blookup({
            name: "CustomerCodeOpen",
            title: "Pelanggan",
            manager: MasterService,
            query: "CustomerCodeOpen",
            defaultSort: "CustomerCode",
            columns: [
                { field: "CustomerCode", title: "Kode Dealer" },
                { field: "CustomerName", title: "Nama Dealer" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
                me.Apply();
            }
        });
    }

    $("[name='CustomerCode']").on('blur', function () {
        if (me.data.CustomerCode != null) {
            $http.post('sv.api/customervehicle/CustomerCode', me.data).
               success(function (data, status, headers, config) {
                   if (data != "") {
                       me.data.CustomerCode = data.CustomerCode;
                       me.data.CustomerName = data.CustomerName;
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
        var lookup = Wx.blookup({
            name: "CustomerCodeOpen",
            title: "Dealer",
            manager: MasterService,
            query: "CustomerCodeOpen",
            defaultSort: "CustomerCode",
            columns: [
                { field: "CustomerCode", title: "Kode Dealer" },
                { field: "CustomerName", title: "Nama Dealer" },
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
        if (me.data.DealerCode != null) {
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


    me.initialize = function () {
        me.data.status = "1";
    }

    me.printPreview = function () {
        var ChassisCode = me.data.ChassisCode == undefined ? "%" : me.data.ChassisCode;
        var EngineCode = me.data.EngineCode == undefined ? "%" : me.data.EngineCode;
        var BasicModel = me.data.BasicModel == undefined ? "%" : me.data.BasicModel;
        var ServiceBookNo = me.data.ServiceBookNo == undefined ? "%" : me.data.ServiceBookNo;
        var PoliceRegNo = me.data.PoliceRegNo == undefined ? "%" : me.data.PoliceRegNo;
        var CustomerCode = me.data.CustomerCode == undefined ? "%" : me.data.CustomerCode;
        var DealerCode = me.data.DealerCode == undefined ? "%" : me.data.DealerCode;
        var status = me.data.start;

        var ReportId = 'SvRpMst015';
        var par = [
           'companycode', ChassisCode
           , EngineCode, BasicModel
           , ServiceBookNo, PoliceRegNo
           , CustomerCode, DealerCode, status
        ]
        var rparam = 'PERIODE : ' + moment(Date.now()).format('DD-MMMM-YYYY');

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Print Master Kendaraan dan Pelanggan",
        xtype: "panels",
        toolbars: [
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    {
                        text: "Kode & No. Rangka",
                        type: "controls",
                        cls: "span5 full",
                        items: [
                            { name: "ChassisCode", cls: "span4", text: "Chassis Code", type: "popup", btnName: "btnChassisCode", click: "ChassisCode()", maxlength: 15 },
                            { name: "ChassisNo", cls: "span4", text: "Chassis No", maxlength: 10 },
                        ]
                    },
                    {
                        text: "Kode & No. Mesin",
                        type: "controls",
                        cls: "span5 full",
                        items: [
                            { name: "EngineCode", cls: "span4", text: "Engine Code", type: "popup", btnName: "btnChassisCode", click: "EngineCode()", maxlength: 15 },
                            { name: "EngineNo", cls: "span4", text: "Engine No" },
                        ]
                    },
                    { name: "BasicModel", cls: "span4 full", text: "Basic Model", type: "popup", btnName: "btnChassisCode", click: "BasicModel()", maxlength: 15 },
                    { name: "ServiceBookNo", cls: "span4 full", text: "No Buku Service", type: "popup", btnName: "btnChassisCode", click: "ServiceBookNo()", maxlength: 15 },
                    { name: "PoliceRegNo", cls: "span4 full", text: "No Polisi", type: "popup", btnName: "btnChassisCode", click: "PoliceRegNo()", maxlength: 15 },
                    {
                        text: "Kode Pelanggan",
                        type: "controls",
                        cls: "span5 full",
                        items: [
                            { name: "CustomerCode", cls: "span4", text: "Kode Pelanggan", type: "popup", btnName: "btnCustomerCode", click: "CustomerCode()" },
                            { name: "CustomerName", cls: "span4", text: "Nama Pelanggan", readonly: true },
                        ]
                    },
                    {
                        text: "Kode Dealer",
                        type: "controls",
                        cls: "span5 full",
                        items: [
                            { name: "DealerCode", cls: "span4", text: "Kode Dealer", type: "popup", btnName: "btnDealerCode", click: "DealerCode()" },
                            { name: "DealerName", cls: "span4", text: "Nama Dealer", readonly: true },
                        ]
                    },
                    {
                        type: "optionbuttons", name: "Status", model: "data.status", text: "Status",
                        items: [
                            { name: "0", text: "Tidak Aktif" },
                            { name: "1", text: "Aktif" },
                        ]
                    },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("PrintCustomerVehicleController");
    }

});