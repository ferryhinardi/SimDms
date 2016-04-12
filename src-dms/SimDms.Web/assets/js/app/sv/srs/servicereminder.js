"use strict"

function srsServiceReminderController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $('#FirstPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod > endPeriod) { $('#EndPeriod').val(firstPeriod) }
    });

    $('#EndPeriod').on('change', function (e) {
        var firstPeriod = $('#FirstPeriod').val();
        var endPeriod = $('#EndPeriod').val();

        if (firstPeriod < endPeriod) { $('#FirstPeriod').val(endPeriod) }
    });

    me.browsePolice = function () {
        var lookup = Wx.blookup({
            name: "PoliceBrowse",
            title: "No. Polisi",
            manager: svServiceManager,
            query: "PoliceBrowse",
            defaultSort: "PoliceRegNo asc",
            columns: [
                { field: "PoliceRegNo", title: "No. Polisi" },
                { field: "ServiceBookNo", title: "No. Buku Service" },
                { field: "BasicModel", title: "Model" },
                { field: "TransmissionType", title: "Tipe Trans." },
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "ChassisNo", title: "No. Rangka" },
                { field: "EngineCode", title: "Kode Mesin" },
                { field: "EngineNo", title: "No. Mesin" },
                { field: "Customer", title: "Pelanggan", width: 175 },
                {
                    field: "LastServiceDate", title: "Tgl. Terakhir Service", sWidth: "130px",
                    template: "#= (LastServiceDate == undefined) ? '' : moment(LastServiceDate).format('DD MMM YYYY') #"
                },
                { field: "LastServiceOdometer", title: "Odometer (Km)" },
                { field: "LastJobType", title: "Pekerjaan Terakhir" },
                { field: "ContactName", title: "Nama Contact" }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.PoliceRegNo = data.PoliceRegNo;
            me.Apply();
        });
    }

    me.execQuery = function () {
        var data = {
            "PoliceRegNo": me.data.PoliceRegNo == undefined ? "%" : me.data.PoliceRegNo, 
            "FirstPeriod": me.data.FirstPeriod, 
            "EndPeriod": me.data.EndPeriod, 
            "Active": me.data.Active, 
            "NonActive": me.data.NonActive
        };
        $http.post('sv.api/srsservicereminder/getsrvremindergrid', data)
       .success(function (e) {
           me.loadTableData(me.gridServiceReminder, e)
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
       });
    }

    me.new = function () {
        me.init();
    }

    me.gridServiceReminder = new webix.ui({
        container: "wxServiceReminder",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "BasicModel", header: "Basic Model", width: 100 },
            { id: "PoliceRegNo", header: "No. Polisi", width: 100 },
            { id: "CustomerCode", header: "Kode Pelanggan", width: 150 },
            { id: "CustomerName", header: "Nama Pelanggan", width: 175 },
            { id: "Email", header: "Email", width: 150 },
            { id: "BirthDate", header: "Tgl. Lahir", width: 130, format: me.dateFormat },
            { id: "Address", header: "Alamat", width: 200 },
            { id: "PhoneNo", header: "Telepon", width: 120 },
            { id: "HPNo", header: "HP", width: 120 },
            { id: "LastServiceDate", header: "Tgl. Service Terakhir", width: 130, format: me.dateFormat },
            { id: "LastServiceOdometer", header: "Odometer (KM)", width: 120 },
            { id: "LastJobType", header: "Jenis Pekerjaan", width: 150 },
            { id: "ChassisCode", header: "Kode Rangka", width: 120 },
            { id: "ChassisNo", header: "No. Rangka", width: 120 },
            { id: "EngineCode", header: "Kode Mesin", width: 120 },
            { id: "EngineNo", header: "No. Mesin", width: 120 },
            { id: "ServiceBookNo", header: "No. Buku Service", width: 120 },
            { id: "Color", header: "Warna", width: 100 },
            { id: "FakturPolisiDate", header: "Tgl. Pembelian", width: 130, format: me.dateFormat },
            { id: "ContactName", header: "Nama Kontak", width: 175 },
            { id: "ContactAddress", header: "Alamat Kontak", width: 200 },
            { id: "ContactPhone", header: "Tlp. Kontak", width: 120 }
        ],
        on: {
            onSelectChange: function () {
            }
        }
    });

    me.initialize = function () {
        me.data.FirstPeriod = me.now();
        me.data.EndPeriod = me.now();
        me.clearTable(me.gridServiceReminder);
        me.data.Active = false;
        me.data.NonActive = false;
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Service Reminder",
        xtype: "panels",
        toolbars: [
            { name: "btnExecQuery", text: "Query", cls: "btn btn-warning", icon: "icon-search", click: "execQuery()" },
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "new()" },
        ],
        panels: [
            {
                name: "pnlServiceReminder",
                items: [
                   {  name: "PoliceRegNo", text: "No. Polisi", cls: "span4", type: "popup", readonly: true, click: "browsePolice()" },
                   { name: "Active", text: "Status Aktif", cls: "span2", type: "ng-check" },
                   { name: "NonActive", text: "Tidak Aktif", cls: "span2", type: "ng-check" },
                   { name: "FirstPeriod", text: "Periode Terakhir Service", cls: "span4", type: "ng-datepicker" },
                   { name: "EndPeriod", text: "S/D", cls: "span4", type: "ng-datepicker" },
                   {
                       name: "wxServiceReminder",
                       title: "Detail Informasi",
                       type: "wxdiv"
                   }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("srsServiceReminderController");
    }
});