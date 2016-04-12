var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterCustomersController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "InqVehicleHistory4Lookup",
            title: "Vehicle History",
            manager: svServiceManager,
            query: "InqVehicleHistory4Lookup",
            defaultSort: "PoliceRegNo asc",
            columns: [
            { field: 'PoliceRegNo', title: 'Police No.' },
                { field: 'BasicModel', title: 'Model' },
                { field: 'TransmissionType', title: 'Trans Type' },
                { field: 'ChassisCode', title: 'Chassis Code' },
                { field: 'ChassisNo', title: 'Chassis No.' },
                { field: 'EngineCode', title: 'Engine Code' },
                { field: 'EngineNo', title: 'Engine No.' },
                //{ field: 'ServiceBookNo', title: 'Service Book No.' },
                //{ field: 'ColourCode', title: 'Colour' },
                //{ field: 'CustomerDesc', title: 'Customer' },
                //{ field: 'FakturPolisiDate', title: 'Police Date Facture' },
                //{ field: 'LastServiceDate', title: 'Service Date' },
                //{ field: 'DealerDesc', title: 'Dealer' },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PoliceRegNo = data.PoliceRegNo;
                me.data.ChassisCode = data.ChassisCode;
                me.data.ChassisNo = data.ChassisNo;
                me.data.ServiceBookNo = data.ServiceBookNo;
                me.data.EngineCode = data.EngineCode;
                me.data.EngineNo = data.EngineNo;
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
                me.data.BasicModel = data.BasicModel;
                //me.lookupAfterSelect(data);
                me.data.isSarviceDate = false;
                me.data.isBranch = false;
                refreshGrid();
            }
        });
    }

    
    me.initialize = function () {
        

        me.hasChanged = false;
        me.isSave = false;
        //me.grid1.clearSelection();
        //me.grid2.clearSelection();

        me.data.PoliceRegNo = localStorage.getItem('params');
        me.data.ServiceBookNo = "";
        me.data.CustomerCode = "";
        me.data.ChassisCode = "";
        me.data.ChassisNo = "";
        me.data.EngineCode = "";
        me.data.EngineNo = "";
        me.data.BasicModel = "";
        me.data.isSarviceDate = false;
        me.data.SarviceDate = "";
        me.data.isBranch = false;

        $('#isSarviceDate').prop('checked', false);
        $('#isBranch').prop('checked', false);

        me.Query();
        me.Apply();

        //refreshGrid();

        kendo.culture("id-ID");
    }

    $("[name = 'isSarviceDate']").on('change', function () {
        me.data.isSarviceDate = $('#isSarviceDate').prop('checked');
        me.Apply();
    });

    $("[name = 'isBranch']").on('change', function () {
        me.data.isBranch = $('#isBranch').prop('checked');
        me.Apply();
    });

    me.Query = function () {
        refreshGrid();
        //$http.post('sv.api/InqVehicleHistory/GetVehicleHistory?PoliceRegNo=' + me.data.PoliceRegNo
        //                                        + '&ServiceBookNo=' + me.data.ServiceBookNo + '&CustomerCode=' + me.data.CustomerCode
        //                                        + '&ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo
        //                                        + '&EngineCode=' + me.data.EngineCode + '&EngineNo=' + me.data.EngineNo
        //                                        + '&BasicModel=' + me.data.BasicModel + '&isSarviceDate=' + me.data.isSarviceDate
        //                                        + '&SarviceDate=' + me.data.SarviceDate + '&isBranch=' + me.data.isBranch).
        //  success(function (data, status, headers, config) {
        //      me.loadTableData(me.grid1, data);
        //      me.detail = data;
              
        //      me.clearTable(me.grid2);
        //  }).
        //  error(function (e, status, headers, config) {
        //      console.log(e);
        //  });
    }

    me.PrintPreview = function () {
        console.log(localStorage.getItem('PoliceRegNo'))
        var ServiceDate = "";
        var PoliceRegNo = localStorage.getItem('PoliceRegNo');
        var ChassisCode = localStorage.getItem('ChassisCode');
        var ChassisNo = localStorage.getItem('ChassisNo');
        var BasicModel = localStorage.getItem('BasicModel');

        if (me.data.isSarviceDate == false) {
            ServiceDate = "1/01/1900";
        } else {
            ServiceDate = moment(Date.now()).format('DD-MMMM-YYYY');
        }

        var ReportId = 'SvRpTrn010';
        var par = [
            '4W', PoliceRegNo, ChassisCode, ChassisNo, BasicModel, ServiceDate
        ]
        var rparam = "";

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });

    }

    function refreshGrid() {
        Wx.kgrid({
            url: "sv.api/InqVehicleHistory/GetVehicleHistory",
            name: "wxinfokendaraan",
            params: me.data,
            //params: $("#pnlRefService").serializeObject(),
            pageSize: 5,
            multiselect: true,
            scrollable: true,
            columns: [
                { field: "check_item", title: "<input type='checkbox' id='chkSelectAll'/>", template: "<input class='check_item' type='checkbox' id='inpchk''/>", width: '60px', sortable: false, filterable: false },
                { field: "BranchCode", title: "BranchCode", width: '150px' },
                { field: "PoliceRegNo", title: "No. Polisi", width: '150px' },
                { field: "BasicModel", title: "Basic Model", width: '150px' },
                { field: "TransmissionType", title: "Trans.", width: '125px' },
                { field: "Chassis", title: "No. Rangka", width: '250px' },
                { field: "Engine", title: "No. Mesin", width: '200px' },
                { field: "ServiceBookNo", title: "No. Buku Service", width: '200px' },
                { field: "ColourCode", title: "Warna", width: '150px' },
                { field: "Customer", title: "Pelanggan", width: '350px' },
                { field: "FakturPolisiDate", title: "Tgl. Pembelian", width: '150px', template: "#= (FakturPolisiDate == undefined) ? '' : moment(FakturPolisiDate).format('DD MMM YYYY') #" },
                { field: "LastServiceDate", title: "Tgl. Service Terakhir", width: '150px', template: "#= (LastServiceDate == undefined) ? '' : moment(LastServiceDate).format('DD MMM YYYY') #" },
                { field: "LastServiceOdometer", title: "KM", width: '150px' },
                { field: "Remarks", title: "Remarks", width: '250px' }
            ],
            detailInit: detailInit,
        });
    }
    
    function detailInit(e) {
        localStorage.setItem('PoliceRegNo', e.data.PoliceRegNo);
        localStorage.setItem('ChassisCode', e.data.ChassisCode);
        localStorage.setItem('ChassisNo', e.data.ChassisNo);
        localStorage.setItem('BasicModel', e.data.BasicModel);
        localStorage.setItem('CustomerCode', e.data.CustomerCode);
        var datas = {
            "PoliceRegNo": e.data.PoliceRegNo,
            "ChassisCode": e.data.ChassisCode,
            "ChassisNo": e.data.ChassisNo,
            "BasicModel": e.data.BasicModel,
            "CustomerCode": e.data.CustomerCode,
            "isBranch": me.data.isBranch,
        }
        $http.post('sv.api/InqVehicleHistory/GetVehicleInfo', datas)
                    .success(function (data, status, headers, config) {
                        $("<div/>").appendTo(e.detailCell).kendoGrid({
                            dataSource: {
                                data: data,
                                pageSize: 5,
                                multiselect: true,
                                scrollable: true
                            },
                            //pageable: true,
                            pageable: {
                                refresh: true,
                                pageSizes: true,
                                buttonCount: 5
                            },
                            columns: [
                                { field: "BranchCode", title: "BranchCode", width: '100px' },
                                { field: "JobOrderNo", title: "No. SPK", width: '120px' },
                                { field: "TotalSrvAmount", title: "Total Biaya", width: '120px', format: "{0:#,##}" },
                                { field: "Remarks", title: "Keterangan", width: '120px' },
                            ],
                            detailInit: SubdetailInit,
                        });
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
    }

    function SubdetailInit(e) {
        var PoliceRegNo = localStorage.getItem('PoliceRegNo');
        var ChassisCode = localStorage.getItem('ChassisCode');
        var ChassisNo = localStorage.getItem('ChassisNo');
        var BasicModel = localStorage.getItem('BasicModel');
        var CustomerCode = localStorage.getItem('CustomerCode');
        
        var datas = {
            "PoliceRegNo": PoliceRegNo,
            "ChassisCode": ChassisCode,
            "ChassisNo": ChassisNo,
            "BasicModel": BasicModel,
            "CustomerCode": CustomerCode,
            "isBranch": me.data.isBranch,
            "JobOrderNo": e.data.JobOrderNo,
        }
        $http.post('sv.api/InqVehicleHistory/GetSubVehicleInfo', datas)
                    .success(function (data, status, headers, config) {
                        $("<div/>").appendTo(e.detailCell).kendoGrid({
                            dataSource: {
                                data: data,
                                pageSize: 5,
                                multiselect: true,
                                scrollable: true
                            },
                            //pageable: true,
                            pageable: {
                                refresh: true,
                                pageSizes: true,
                                buttonCount: 5
                            },
                            columns: [
                                { field: "BranchCode", title: "BranchCode", width: '100px' },
                                { field: "JobOrderNo", title: "No. SPK", width: '120px' },
                                { field: "JobOrderDate", title: "Tgl. SPK", width: '120px', template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #" },
                                { field: "FPJNo", title: "No. Faktur", width: '100px' },
                                { field: "FPJDate", title: "Tgl. Faktur", width: '100px', template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #" },
                                { field: "JobType", title: "Jenis Pekerjaan", width: '120px' },
                                { field: "Odometer", title: "KM", width: '85px' },
                                { field: "NameSA", title: "SA", width: '200px' },
                                { field: "NameForeman", title: "Foreman", width: '200px' },
                                { field: "MechanicId", title: "Mekanik", width: '180px' },
                                { field: "OperationNo", title: "No. Pekerjaan/Sparepart", width: '170px' },
                                { field: "OperationQty", title: "NK/Qty", width: '90px' },
                                { field: "OperationAmt", title: "Total Biaya", width: '120px', format: "{0:#,##}" },
                                { field: "Description", title: "Keterangan", width: '200px' },
                            ]
                        });
                    })
                    .error(function (e) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                    });
    }
    //me.grid1 = new webix.ui({
    //    container: "wxinfokendaraan",
    //    view: "wxtable", css:"alternating",
    //    scrollX: true,
    //    columns: [
    //            { id: "Cetak", header: { content: "masterCheckbox", contentId: "chkSelect" }, width: 50, template: "{common.checkbox()}" },
    //            { id: "BranchCode", header: "BranchCode", width: 100},
    //            { id: "PoliceRegNo", header: "No. Polisi", width: 100},
    //            { id: "BasicModel", header: "Basic Model", width: 100},
    //            { id: "TransmissionType", header: "Trans.", width: 70},
    //            { id: "Chassis", header: "No. Rangka", width: 170},
    //            { id: "Engine", header: "No. Mesin", width: 120},
    //            { id: "ServiceBookNo", header: "No. Buku Service", width: 120},
    //            { id: "ColourCode", header: "Warna", width: 70},
    //            { id: "Customer", header: "Pelanggan", width: 200},
    //            { id: "FakturPolisiDate", header: "Tgl. Pembelian", width: 120, format: me.dateFormat},
    //            { id: "LastServiceDate", header: "Tgl. Service Terakhir", width: 130, format: me.dateFormat },
    //            { id: "LastServiceOdometer", header: "KM", width: 90},
    //            { id: "Remarks", header: "Remarks", width: 120},
    //            { id: "CustomerCode", header: "Kode Customer", width: 120 },
    //    ],
    //    on: {
    //    onSelectChange: function () {
    //        if (me.grid1.getSelectedId() !== undefined) {
    //            var data = this.getItem(me.grid1.getSelectedId().id);

    //            localStorage.setItem('PoliceRegNo', data.PoliceRegNo);
    //            localStorage.setItem('ChassisCode', data.ChassisCode);
    //            localStorage.setItem('ChassisNo', data.ChassisNo);
    //            localStorage.setItem('BasicModel', data.BasicModel);

    //            var datas = {
    //                "PoliceRegNo": data.PoliceRegNo,
    //                "ChassisCode": data.ChassisCode,
    //                "ChassisNo": data.ChassisNo,
    //                "BasicModel": data.BasicModel,
    //                "CustomerCode": data.CustomerCode,
    //                "isBranch": me.data.isBranch,
    //            }
    //            $http.post('sv.api/InqVehicleHistory/GetVehicleInfo', datas)
    //            .success(function (data, status, headers, config) {
    //                me.loadTableData(me.grid2, data);
    //            })
    //            .error(function (e) {
    //                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
    //            });
    //        }
    //    }
    //}
    //});

    //me.grid2 = new webix.ui({
    //    container: "wxinfoperawatan",
    //    view: "wxtable", css:"alternating",
    //    scrollX: true,
    //    columns: [
    //            { id: "BranchCode", header: "BranchCode", width: 100},
    //            { id: "JobOrderNo", header: "No. SPK", width: 120},
    //            { id: "JobOrderDate", header: "Tgl. SPK", width: 120, format: me.dateFormat },
    //            { id: "FpjNo", header: "No. Faktur", width: 100},
    //            { id: "FpjDate", header: "Tgl. Faktur", width: 100, format: me.dateFormat },
    //            { id: "JobType", header: "Jenis Pekerjaan", width: 120},
    //            { id: "Odometer", header: "KM", width: 85},
    //            { id: "NameSA", header: "SA", width: 200},
    //            { id: "NameForeman", header: "Foreman", width: 200},
    //            { id: "Mechanic", header: "Mekanik", width: 180},
    //            { id: "noPekerjaanSparepart", header: "No. Pekerjaan/Sparepart", width: 170},
    //            { id: "opQty", header: "NK/Qty", width: 90},
    //            { id: "TotalSrvAmount", header: "Total Biaya", width: 120, format: webix.i18n.numberFormat },
    //            { id: "Description", header: "Keterangan", width: 200 },
    //    ]
    //});

    //webix.event(window, "resize", function () {
    //    me.grid1.adjust();
    //    me.grid2.adjust();
    //})

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Riwayat Perawatan Kendaraan",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", click: "browse()" }, 
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "PrintPreview()" },
                ],
        panels: [
            {
                name: "pnlRefService",
                title: "Informasi Kerdaraan",
                items: [
                    { name: "PoliceRegNo", cls: "span3", text: "No Polisi" },
                    {
                        text: "No Rangka",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "ChassisNo", cls: "span4", text: "Chassis No" },
                            { name: "ChassisCode", cls: "span4", text: "Chassis Code" },
                        ]
                    },
                    { name: "ServiceBookNo", cls: "span3", text: "No Buku Service" },
                    {
                        text: "No Mesin",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "EngineNo", cls: "span4", text: "Engine No" },
                            { name: "EngineCode", cls: "span4", text: "Engine Code" },
                        ]
                    },
                    {
                        text: "Pelanggan",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "CustomerCode", cls: "span2", text: "Customer Code" },
                            { name: "CustomerName", cls: "span6", text: "Customer Name" },
                        ]
                    },
                    { type: "hr" },
                    { name: "BasicModel", cls: "span4", text: "Basic Model" },
                    {
                        text: "Mulai Tgl Service",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "isSarviceDate", type: "check", text: "", cls: "span1", float: "left" },
                            { name: "SarviceDate", cls: "span7", type: "ng-datepicker", disable: "data.isSarviceDate == false" },
                        ]
                    },
                    {
                        text: "All Branch",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "isBranch", cls: "span1", type: "check" },
                            { type: "label", text: "All Branch", cls: "span1 mylabel" },
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                             { name: "Query", text: "Query", icon: "icon-Search", cls: "span4", click: "Query()" },
                         ]
                    },
                    //{
                    //    name: "wxinfokendaraan",
                    //    type: "wxdiv"
                    //},
                ]
            },
            {
                name: "wxinfokendaraan",
                xtype: "k-grid",
                cls: "expand"
            },
            //{
            //    name: "pnlRefService",
            //    title: "Informasi Perawatan",
            //    items: [
            //            {
            //                name: "wxinfoperawatan",
            //                type: "wxdiv"
            //            },
            //    ]
            //},
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("gnMasterCustomersController");
    }




});