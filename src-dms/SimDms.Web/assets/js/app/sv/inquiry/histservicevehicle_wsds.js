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
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.data.isSarviceDate = false;
                me.data.isBranch = false;;
                me.Apply();
            }
        });
    }

    me.initialize = function () {


        me.hasChanged = false;
        me.isSave = false;
        me.grid1.clearSelection();
        me.grid2.clearSelection();

        me.data.PoliceRegNo = "";
        me.data.ServiceBookNo = "";
        me.data.CustomerCode = "";
        me.data.ChassisCode = "";
        me.data.ChassisNo = "";
        me.data.EngineCode = "";
        me.data.EngineNo = "";
        me.Apply();
    }

    me.Query = function () {
        $http.post('sv.api/InqVehicleHistory/GetVehicleHistoryWSDS?PoliceRegNo=' + me.data.PoliceRegNo
                                                + '&ServiceBookNo=' + me.data.ServiceBookNo + '&BasicModel=' + me.data.BasicModel
                                                + '&CustomerCode=' + me.data.CustomerCode + '&CustomerName=' + me.data.CustomerName
                                                + '&ChassisCode=' + me.data.ChassisCode + '&ChassisNo=' + me.data.ChassisNo
                                                + '&EngineCode=' + me.data.EngineCode + '&EngineNo=' + me.data.EngineNo).
          success(function (data, status, headers, config) {
              me.loadTableData(me.grid1, data);

              me.clearTable(me.grid2);
          }).
          error(function (e, status, headers, config) {
              console.log(e);
          });
    }

    me.grid1 = new webix.ui({
        container: "wxinfokendaraan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
                { id: "IsSelect", header: "Pilih", width: 50, template: "{common.checkbox()}" },
                { id: "PoliceRegNo", header: "No. Polisi", width: 100 },
                { id: "BasicModel", header: "Basic Model", width: 100 },
                { id: "TransmissionType", header: "Trans.", width: 70 },
                { id: "Chassis", header: "No. Rangka", width: 170 },
                { id: "Engine", header: "No. Mesin", width: 120 },
                { id: "ServiceBookNo", header: "No. Buku Service", width: 120 },
                { id: "ColourCode", header: "Warna", width: 70 },
                { id: "Customer", header: "Pelanggan", width: 200 },
                { id: "FakturPolisiDate", header: "Tgl. Pembelian", width: 120, format: me.dateFormat },
                { id: "LastServiceDate", header: "Tgl. Service Terakhir", width: 130, format: me.dateFormat },
                { id: "LastServiceOdometer", header: "KM", width: 90 },
                { id: "Remarks", header: "Remarks", width: 120 },
                { id: "CustomerCode", header: "Kode Customer", width: 120 },
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    var data = this.getItem(me.grid1.getSelectedId().id);

                    localStorage.setItem('PoliceRegNo', data.PoliceRegNo);
                    localStorage.setItem('ChassisCode', data.ChassisCode);
                    localStorage.setItem('ChassisNo', data.ChassisNo);
                    localStorage.setItem('BasicModel', data.BasicModel);

                    var datas = {
                        "PoliceRegNo": data.PoliceRegNo,
                    }
                    $http.post('sv.api/InqVehicleHistory/GetVehicleInfoWSDS', datas)
                    .success(function (data, status, headers, config) {
                        me.loadTableData(me.grid2, data);
                    })
                    .error(function (e) {
                        MsgBox('Connection to the server failed...', MSG_ERROR);
                    });
                }
            }
        }
    });

    me.grid2 = new webix.ui({
        container: "wxinfoperawatan",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
                { id: "JobOrderNo", header: "No. SPK", width: 120 },
                { id: "JobOrderDate", header: "Tgl. SPK", width: 120, format: me.dateFormat },
                { id: "FpjNo", header: "No. Faktur", width: 100 },
                { id: "FpjDate", header: "Tgl. Faktur", width: 100, format: me.dateFormat },
                { id: "JobType", header: "Jenis Pekerjaan", width: 120 },
                { id: "Odometer", header: "KM", width: 85 },
                { id: "Mechanic", header: "Mekanik", width: 180 },
                { id: "noPekerjaanSparepart", header: "No. Pekerjaan/Sparepart", width: 170 },
                { id: "opQty", header: "NK/Qty", width: 90 },
                { id: "TotalSrvAmount", header: "Total Biaya", width: 120, format: webix.i18n.numberFormat },
                { id: "Description", header: "Keterangan", width: 200 },
        ]
    });


    webix.event(window, "resize", function () {
        me.grid1.adjust();
        me.grid2.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "History Vehicle (WSDS)",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", click: "browse()" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
                    //{ name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "PrintPreview()" },
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
                    { name: "BasicModel", cls: "span3", text: "Basic Model" },
                    {
                        text: "No Mesin",
                        type: "controls",
                        cls: "span5",
                        items: [
                            { name: "EngineNo", cls: "span4", text: "Chassis No" },
                            { name: "EngineCode", cls: "span4", text: "Chassis Code" },
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
                    {
                        type: "buttons",
                        items: [
                             { name: "Query", text: "Query", icon: "icon-Search", cls: "span4", click: "Query()" },
                        ]
                    },
                    {
                        name: "wxinfokendaraan",
                        type: "wxdiv"
                    },
                ]
            },
            {
                name: "pnlRefService",
                title: "Informasi Perawatan",
                items: [
                        {
                            name: "wxinfoperawatan",
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
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("gnMasterCustomersController");
    }

});