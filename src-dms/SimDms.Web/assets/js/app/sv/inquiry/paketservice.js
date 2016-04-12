var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function InqPaketService($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {

        me.GridDetail.adjust();
        me.GridPelanggan.adjust();

    }
    me.Package = function () {
        var lookup = Wx.blookup({
            name: "PackageLookUp",
            title: "List Master Paket Service",
            manager: MasterService,
            query: "PackageLookUp",
            defaultSort: "Package asc",
            columns: [
                { field: "Package", title: "Paket Service" },
                { field: "BasicMod", title: "Basic Model" },
                { field: "CustomerBill", title: "Pembayar" },
                { field: "IntervalYear", title: "Interval (Year)" },
                { field: "IntervalKM", title: "Interval (KM)" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                $http.post('sv.api/InquiryPaketService/DetailPackage?packageCode=' + data.PackageCode + '&BasicModel=' + data.BasicModel)
                    .success(function (e) {
                        if (e.data != "" && e.data2 == "") {
                            me.loadTableData(me.GridDetail, e.data);
                            me.loadTableData(me.GridPelanggan, e.data2);
                        } else {
                            MsgBox('Tidak ada data detail di temukan', MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
                me.Apply();
            }
        });

    }

    me.GridDetail = new webix.ui({
        container: "GridDetail",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        height: 500,
        autoheight: false,
        columns: [
            { id: "OperationNo", header: "Pekerjaan", width: 100 },
            { id: "DiscTask", header: "Diskon Pekerjaan (%)", width: 150 },
            { id: "PartNo", header: "No Part", width: 150 },
            { id: "PartName", header: "Nama Part", width: 240 },
            { id: "DiscPart", header: "Diskon Part (%)", width: 150 },
        ]
    });

    me.GridPelanggan = new webix.ui({
        container: "GridPelanggan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        height: 500,
        autoheight: false,
        columns: [
            { id: "CustomerCode", header: "Cust Code", width: 100 },
            { id: "CustomerCode", header: "Kode Pelanggan", width: 100 },
            { id: "CustomerName", header: "Nama Pelanggan", width: 200 },
            { id: "PoliceRegNo", header: "No Polisi", width: 100 },
            { id: "ServiceBookNo", header: "No Buku Service", width: 100 },
            { id: "ChassisCode", header: "Kode Chassis", width: 100 },
            { id: "ChassisNo", header: "No Chassis", width: 100 },            
            { id: "BeginDate", header: "Periode Awal", width: 100 },
            { id: "EndDate", header: "Periode Akhir", width: 100 },
            { id: "VirtualAccount", header: "NaturalAccount", width: 200 },
        ]
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inquiry Paket Service",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-warning", icon: "icon-plus", click: "cancelOrClose()" },
        ],
        panels: [
            {
                title: "Paket Service",
                name: "pnlPrint",
                items: [
                    {
                        text: "Paket Service",
                        type: "controls",
                        items: [
                            { name: "Package", cls: "span2", type: "popup", click: "Package()", readonly: true },
                            { name: "PackageName", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        text: "Basic Model",
                        type: "controls",
                        items: [
                            { name: "BasicModel", cls: "span2", readonly: true },
                            { name: "BasicMod", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        text: "Pembayaran",
                        type: "controls",
                        items: [
                            { name: "BillTo", cls: "span2", readonly: true },
                            { name: "CustomerName", cls: "span4", readonly: true },
                        ]
                    },
                    { name: "IntervalYear", text: "Interval (Year)", cls: "span3", readonly: true },
                    { name: "IntervalKM", text: "Interval (KM)", cls: "span3", readonly: true },
                    { name: "PackageDesc", text: "Keterangan", cls: "span6", type: "textarea", readonly: true },
                ]
            },
            {
                name: "GridDetail",
                xtype: "wxtable"
            },
            {type: "hr"},
            {
                name: "GridPelanggan",
                xtype: "wxtable"
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 5px 5px", "align: center");
        SimDms.Angular("InqPaketService");
    }

});