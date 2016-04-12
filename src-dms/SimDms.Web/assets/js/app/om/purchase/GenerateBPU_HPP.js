"use strict"

function GenerateBPU_HPP($scope, $http, $injector) {
    var me = $scope;
    var currentDate = moment().format();

    $injector.invoke(BaseController, this, { $scope: me });

    me.lookupInvoiceNo = function () {
        var lookup = Wx.klookup({
            name: "lookupInvoice",
            title: "Invoice No",
            url: "om.api/GenerateBPU_HPP/LookupInvoice",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "InvoiceNo", title: "No. Invoice", width: 100 },
                { field: "InvoiceDate", titel: "Tgl Invoice", width: 100, template: "#= InvoiceDate==null ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                { field: "PONo", title: "No. PO", width: 100 },
                { field: "PODate", titel: "Tgl PO", width: 100, template: "#= InvoiceDate==null ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" }
                //{ field: "CustomerCode", title: "Kode Pelanggan", width: 100 },
                //{ field: "CustomerName", title: "Nama Pelanggan", width: 250 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.InvoiceNo = data.InvoiceNo;
            me.data.InvoiceDate = data.InvoiceDate;
            me.data.PONo = data.PONo;
            me.data.PODate = data.PODate;
            $http.post('om.api/GenerateBPU_HPP/getDetail', { InvoiceNo: data.InvoiceNo })
              .success(function (result) 
              {
                  if (result.success)
                  {
                      me.loadTableData(me.gridDetail, result.DataGrid);
                      me.gridDetail.adjust();
                      $('#btnProcess').removeAttr('disabled');
                      $('#btnCancel').removeAttr('disabled');
                      me.Apply();
                  }else{
                      MsgBox (result.message, MSG_INFO);
                  }  
              })
        });
    }
    me.lookupWarehouse = function () {
        var lookup = Wx.blookup({
            name: "WarehouseCodeLookup",
            title: "Lookup Warehouse",
            manager: spSalesManager,
            query: "WareHouseSD",
            defaultSort: "seqno asc",
            columns: [
                { field: "LookUpValue", title: "Warehouse Code" },
                { field: "LookUpValueName", title: "Warehouse Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.WarehouseCode = data.LookUpValue;
                me.data.WarehouseName = data.LookUpValue;
                me.Apply();
            }
        });
    };

    me.doCancel = function () {
        $('#btnProcess').attr('disabled','disabled');
        $('#btnCancel').attr('disabled', 'disabled');
        me.data.InvoiceNo = '';
        me.data.InvoiceDate = 'dd-MMM-yyyy';
        me.data.PONo = '';
        me.data.PODate = 'dd-MMM-yyyy';
        me.data.WarehouseCode = '';
        me.data.WarehouseName = '';
        me.clearTable(me.gridDetail);
    }

    me.doProcess = function () {
        if (me.data.WarehouseCode == null) { MsgBox("Warehouse harus diisi!", MSG_INFO); return; }
        MsgConfirm("Yakin ingin melakukan proses BPU dan HPP?", function (ok) {
            $http.post('om.api/GenerateBPU_HPP/doProcess', { InvoiceNo: me.data.InvoiceNo, PONo: me.data.PONo, WHouse: me.data.WarehouseCode })
              .success(function (result) {
                  if (result.success) {
                      MsgBox(result.message, MSG_INFO);
                      me.doCancel();
                      me.Apply();
                  } else {
                      MsgBox(result.message, MSG_INFO);
                  }
              })
        });
    }

    me.gridDetail = new webix.ui({
        container: "wxdetail",
        view: "wxtable", css: "alternating",
        columns: [
            { id: "SalesModelCode", header: "Sales Model Code", width: 170 },
            { id: "SalesModelYear", header: "Sales Model Year", width: 130 },
            { id: "ColourCode", header: "Colour Code", width: 80 },
            { id: "ColourDesc", header: "Colour Description", width: 170 },
            { id: "ChassisCode", header: "Chassis Code", width: 120 },
            { id: "ChassisNo", header: "Chassis No", width: 100 },
            { id: "EngineCode", header: "Engine Code", width: 100 },
            { id: "EngineNo", header: "Engine No", width: 100 },
        ],
        on: {
            onSelectChange: function () {
                if (me.griddetil.getSelectedId() !== undefined) {

                }
            }
        }
    });

    webix.event(window, "resize", function () {
        me.gridDetail.adjust();
    });
    
    me.initialize = function () {
        me.data = {};
        $('#btnProcess').attr('disabled', 'disabled');
        $('#btnCancel').attr('disabled', 'disabled');
        me.data.InvoiceNo = '';
        me.data.InvoiceDate = 'dd-MMM-yyyy';
        me.data.PONo = '';
        me.data.PODate = 'dd-MMM-yyyy';
        me.data.WarehouseCode = '';
        me.data.WarehouseName = '';
        me.clearTable(me.gridDetail);
        me.Apply();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Generate BPU & HPP",
        xtype: "panels",
        //toolbars: WxButtons,
        panels: [
                {
                    name: "pnl",
                    items: [
                        { name: "InvoiceNo", model: "data.InvoiceNo", text: "No Invoice", cls: "span4", placeHolder: "No Invoice", readonly: true, type: "popup", click: "lookupInvoiceNo()", validasi: "required" },
                        { name: "InvoiceDate", model: "data.InvoiceDate", text: "Tgl Invoice", cls: "span4", type: "ng-datepicker", readonly: true },
                        { name: "PONo", model: "data.PONo", text: "No PO", cls: "span4", readonly: true },
                        { name: "PODate", model: "data.PODate", text: "Tgl PO", cls: "span4", type: "ng-datepicker", readonly: true },
                        { name: "WarehouseCode", model: "data.WarehouseCode", text: "Warehouse", cls: "span4", placeHolder: "Warehouse", readonly: true, type: "popup", click: "lookupWarehouse()", validasi: "required" },
                        { name: "WarehouseName", model: "data.WarehouseName", text: "Warehouse Name", cls: "span4", placeHolder: "Warehouse Name", readonly: true },
                        {
                            type: "buttons",
                            items: [
                                   { name: "btnProcess", text: "Process", icon: "icon-gear", cls: "btn btn-primary", click: "doProcess()", disable: true },
                                   { name: "btnCancel", text: "Cancel", icon: "icon-refresh", cls: "btn btn-warning", click: "doCancel()", disable: true },
                            ]
                        }
                    ]
                },
                {
                    name: "pnl2",
                    title: "Detail",
                    //text: "Detail",
                    items: [                     
                           {
                                name: "wxdetail",
                                type: "wxdiv"
                           }
                    ]
                },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("GenerateBPU_HPP");
    }
});