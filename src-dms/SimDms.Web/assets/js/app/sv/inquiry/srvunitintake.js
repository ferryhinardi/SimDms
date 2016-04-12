var status = 'N';

"use strict";

function svUnitIntakeController($scope, $http, $injector) {
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

        //if (firstPeriod < endPeriod) { $('#FirstPeriod').val(endPeriod) }
    });

    $("[name='Rework']").on('change', function () {
        me.data.Rework = $('#Rework').prop('checked');
    });

    me.loadData = function () {
       
        me.refreshGrid();
    }
    me.exportExcel = function () {
        me.exportXls();
    }

    me.refreshGrid = function () {
        var rwk='';
        if (me.data.Rework==true) {
            rwk='Rework'    
        }

        var prms = {
            FirstPeriod: moment(me.data.FirstPeriod).format("YYYY-MM-DD") + " 00:00:00",
            EndPeriod: moment(me.data.EndPeriod).format("YYYY-MM-DD") + " 23:59:59",
            NoVin: me.data.noVin,
            NoPol: me.data.noPol,
            CustName: me.data.custName,
            Rework: rwk
        };
        console.log(prms);

        var lookup = Wx.kgrid({
            url: "sv.api/ServiceUnitIntake/SrvUnitIntakeGrid",
            name: "srvUnitIntake",
            params: prms,
            scrollable: true,
            columns: [
            { field: "VinNo", width: 200, title: "Nomor Vin" },
            {
                field: "JobOrderClosed", width: 160, title: "Tgl. Tutup SPK",
                template: '#= (JobOrderClosed == null) ? "" : moment(JobOrderClosed).format("DD MMM YYYY")  #'
            },
            { field: "OutletCode", width: 150, title: "Kode Outlet" },
            { field: "OutletName", width: 400, title: "Nama Outlet" },
            { field: "GroupJobTypeDesc", width: 250, title: "Jenis Service" },
            { field: "Odometer", width: 100, title: "KM", type: 'number' },
            { field: "SalesModelDesc", width: 200, title: "Type" },
            { field: "ProductionYear", width: 150, title: "Tahun Produksi" },
            { field: "PoliceRegNo", width: 150, title: "No. Polisi" },
            { field: "EngineNo", width: 150, title: "No. Mesin" },
            { field: "ChassisNo", width: 150, title: "No. Rangka" },
            { field: "CustomerName", width: 580, title: "Nama Pelanggan" },
            { field: "PhoneNo", width: 150, title: "No. Telp. Rumah" },
            { field: "OfficePhoneNo", width: 150, title: "No. Telp. Kantor" },
            { field: "HPNo", width: 150, title: "No. HP" },
            { field: "Email", width: 150, title: "Email" },
            { field: "BirthDate", width: 150, title: "Tanggal Lahir", type: 'date' },
            { field: "Gender", width: 150, title: "Jenis Kelamin" },
            { field: "Address", width: 1000, title: "Alamat" },
            { field: "JobTypeDesc", width: 500, title: "Keterangan" },
            { field: "Area", width: 250, title: "Area(Service)" },
            { field: "SaNik", width: 150, title: "NIS SA" },
            { field: "SaName", width: 250, title: "Nama SA" },
            { field: "CompanyCode", width: 150, title: "Kode Dealer (Service)" },
            { field: "CompanyName", width: 400, title: "Nama Dealer (Service)" },
            { field: "DealerCode", width: 150, title: "Kode Dealer (Purchase)" },
            { field: "DealerName", width: 400, title: "Nama Dealer (Purchase)" },
            { field: "SalesModelCode", width: 150, title: "Sales Model Code" },
            { field: "BasicModel", width: 150, title: "Basic Model" },
            { field: "DoDate", width: 150, title: "Tanggal Pembelian", type: 'date' },
            { field: "ContactName", width: 480, title: "Additional Contact" },
            { field: "JobType", width: 250, title: "Jenis Pekerjaan" },
            { field: "SaNik", width: 150, title: "NIK SA" },
            ],
        });

    }

    me.exportXls = function () {
        var rwk = '';
        if (me.data.Rework == true) {
            rwk = 'Rework'
        }
        Wx.XlsxReport({
            url: 'sv.api/report/srvUnitIntakeXlsx',
            type: 'xlsx',
            params: {
                FirstPeriod: moment(me.data.FirstPeriod).format("YYYY-MM-DD") + " 00:00:00",
                EndPeriod: moment(me.data.EndPeriod).format("YYYY-MM-DD") + " 23:59:59",
                NoVin: me.data.noVin,
                NoPol: me.data.noPol,
                CustName: me.data.custName,
                Rework: rwk
            }
        });
    }

    me.default = function () {
        $http.post('sv.api/ServiceUnitIntake/Default').
            success(function (e) {
                me.data.FirstPeriod = e.FirstPeriod;
                me.data.EndPeriod = e.EndPeriod;
            });
    }


    me.initialize = function () {
        me.default();
        if ($('#Rework').prop('checked', false)) {
            me.data.Rework = false;
        }
    }

    me.start();

    me.HideForm = function () {
        $(".body > .panel").fadeOut();
    }
}

$(document).ready(function () {
    var options = {
        title: "Service Unit Intake",
        xtype: "panels",

        panels: [
            {
                name: "pnlFilter",
                items: [
                   {
                       text: "Periode",
                       type: "controls",
                       items: [
                           { name: "FirstPeriod", text: "Periode", cls: "span2", type: "ng-datepicker" },
                           { name: "EndPeriod", text: "S/D", cls: "span2", type: "ng-datepicker" },
                       ]
                   },
                   {
                       text: "Filter By",
                       type: "controls",
                       items: [
                           { name: "noVin", text: 'No Vin', cls: "span2", type: "text" },
                           { name: "noPol", text: 'No Polisi', cls: "span2", type: "text" },
                           { name: "custName", text: 'Nama Pelanggan', cls: "span2", type: "text" },

                       ]
                   },
                   { name: "Rework", text: "Rework", type: "check", cls: "span2" },
                ],
            },
            {
                name: "srvUnitIntake",
                xtype: "k-grid",
            },
        ],
        toolbars: [
           { name: "btnRefresh", text: "Load Data", icon: "fa fa-search", click: "loadData()" },
           { name: "btnExportXls", text: "Export (xls)", icon: "fa fa-file-excel-o", click: "exportExcel()" },
        ],
    }



    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("svUnitIntakeController");
    }
});
