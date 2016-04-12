var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spPemeliharaanFakturPenjualanController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        me.LookupFPJ();
    }

    me.FPJCode = function () {
        me.LookupFPJ();
    }
    me.LookupFPJ = function () {
        var lookup = Wx.blookup({
            name: "BrowseFPJ",
            title: "Maintenance Invoice",
            manager: spManager,
            query: "GetSpTrnSFPJHdr",
            defaultSort: "FPJDate desc",
            columns: [
                { field: "FPJNo", title: "No. Faktur Penjualan", width: 180 },
                { field: "FPJDate", title: "Tgl. Faktur Penjualan", template: "#= (FPJDate == undefined) ? '' : moment(FPJDate).format('DD MMM YYYY') #", width: 180 },
                { field: "FPJGovNo", title: "No. Faktur Pajak", width: 200 },
                { field: "InvoiceNo", title: "No. Invoice", width: 180 },
                { field: "Customer", title: "Pelanggan" }
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                $('#btnDelete').hide();
                me.Apply();

                $http.post('sp.api/MaintenanceFakturPenjualan/GetSpTrnSFPJDtl?FPJNo=' + result.FPJNo).
                success(function (data, status, headers, config) {
                    console.log(data);
                    me.grid.detail = data.data;
                    me.loadTableData(me.grid1, me.grid.detail);
                    me.total.DPP = data.TotalDPP;
                    me.total.PPN = data.TotalPPN;
                    me.total.AMT = data.TotalAmount;
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });

                $('#btnCustomerCode').removeAttr('disabled');
            }
        });
    }

    me.CustomerCode = function () {
        var lookup = Wx.blookup({
            name: "BrowseCustomerCode",
            title: "Customer Code",
            manager: spManager,
            query: "GetCustomer",
            defaultSort: "CustomerCode asc",
            columns: [
            { field: "CustomerCode", title: "Customer Code" },
            { field: "CustomerName", title: "Customer Name" },
            { field: "Address", title: "Address" },
            { field: "ProfitCenter", title: "ProfitCenter" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                console.log(result);
                me.data.CustomerCode = result.CustomerCode;
                me.data.CustomerName = result.CustomerName;
                me.data.Address1 = result.Address1;
                me.data.Address2 = result.Address2;
                me.data.Address3 = result.Address3;
                me.data.Address4 = result.Address4;
                me.Apply();
            }
        });
    }


    me.loadDetail = function(data)
    {
        var p1 = me.where.create("Year", "eq",  parseInt(data.Year));
        var p2 = me.where.create("Month", "eq", parseInt(data.Month));
        var whereClause = p1.and(p2);

        var query = new breeze.EntityQuery()
            .from("SalesTargetDetail")
            .where(whereClause);
    
          spManager.executeQuery(query).then(function(data){
              me.UpdateGridDetail(data.results);
          }).fail(function(e) {
              MsgBox(e, MSG_ERROR);  
          });
    }

     
    me.save = function (e, param) {
        $http.post('sp.api/MaintenanceFakturPenjualan/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    //me.loadDetail(me.data);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }


    me.UpdateGridDetail = function(data)
    {
        me.grid.detail = data;     
        me.loadTableData(me.grid1, me.grid.detail); 
    }

   

    me.LinkDetail = function()
    {
        me.detail.Month = me.data.Month;
        me.detail.Year = me.data.Year;
    }

    me.initialize = function()
    {
        me.clearTable(me.grid1);
        me.detail = {};
        me.total = {};
        me.Apply();
    }


    me.grid1 = new webix.ui({
        container: "wxpartdetails",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "PartNo", header: "Category Code", fillspace: true },
            { id: "PartNoOriginal", header: "Part No Original", fillspace: true },
            { id: "DocNo", theader: "Category Name", fillspace: true },
            { id: "QtyBill", header: "Qty Target", format: webix.i18n.numberFormat, css: { "text-align": "right" }, fillspace: true },
            { id: "SalesAmt", header: "Sales Amt", format: webix.i18n.numberFormat, css: { "text-align": "right" }, fillspace: true },
            { id: "DiscPct", header: "Disc %", format: webix.i18n.numberFormat, css: { "text-align": "right" }, fillspace: true },
            { id: "PartName", header: "Part Name", fillspace: true },
            { id: "WarehouseCode", header: "Warehouse Code", fillspace: true },
            { id: "DocNo", header: "Doc No", fillspace: true },
            {
                type: "buttons", cls: "span3", items: [
                               { name: "btnSave", text: "Add", icon: "icon-plus", click: "EditDetails()", cls: "btn btn-primary", disable: "me.PartNo === undefined" },
                               { name: "btnClr", text: "Clear", icon: "icon-remove", click: "ClearDetails()", cls: "btn btn-danger", disable: "me.PartNo === undefined" },
                ]
            }
        ],        
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    //me.data.PartNo = me.detail.PartNo;
                    //me.data.DocNo = me.detail.DocNo;
                    //me.data.QtyBill = me.detail.QtyBill;
                    //me.data.SalesAmt = me.detail.SalesAmt;
                    //me.data.DiscPct = me.detail.DiscPct;
                    //me.data.PartName = me.detail.PartName;
                    //me.data.WarehouseCode = me.detail.WarehouseCode;
                    //me.data.PartNoOriginal = me.detail.PartNoOriginal;
                    //me.data.DocNo = me.detail.DocNo;
                    $('#btnSaveDtl,#btnClrDtl').removeAttr('disabled', 'disabled');
                    //me.lookupAfterSelect(me.detail);
                    me.Apply();
                }
            }
        }
    });

    me.ClearDtl = function () {
        me.detail = {};
        me.loadTableData(me.grid1, me.grid.detail);
        $('#btnSaveDtl,#btnClrDtl').attr('disabled', 'disabled');

    }

    me.SaveDtl = function () {
        me.detail.FPJNo = me.data.FPJNo;
        $http.post('sp.api/MaintenanceFakturPenjualan/SaveFPJDtl', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    //me.startEditing();
                    //me.loadDetail(me.data);
                    $http.post('sp.api/MaintenanceFakturPenjualan/GetSpTrnSFPJDtl?FPJNo=' + me.data.FPJNo).
                    success(function (data, status, headers, config) {
                        console.log(data);
                        me.grid.detail = data.data;
                        me.loadTableData(me.grid1, me.grid.detail);
                        me.total.DPP = data.TotalDPP;
                        me.total.PPN = data.TotalPPN;
                        me.total.AMT = data.TotalAmount;
                    }).
                    error(function (data, status, headers, config) {
                        MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    });
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }


    webix.event(window, "resize", function(){ 
        me.grid1.adjust(); 
    })

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Maintenance Invoice",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                cls :"span4",
                title: "Faktur Penjualan",
                items: [
                        {
                            text: "No Faktur Penjualan",
                            type: "controls",
                            items: [
                                { name: "FPJNo", cls: "span2", placeHolder: "No. FPJ", type: "popup", btnName: "btnFPJ", readonly: true, click: "FPJCode()" },
                            ]
                        },
                        { name: "FPJDate", text: "Tgl. Faktur Penjualan", cls: "span3", placeHolder: "Tgl. Faktur Penjualan", readonly: true },
                        { name: "FPJGovNo", text: "No. Faktur Pajak", cls: "span3", placeHolder: "No. Faktur Pajak", readonly: true },
                        { name: "PickingSlipNo", text: "No. Picking List", cls: "span3", placeHolder: "No. Picking List", readonly: true },
                        { name: "InvoiceNo", text: "No. Invoice", cls: "span3", placeHolder: "No. Invoice", readonly: true },
                    ]   
            },
            {
                name: "pnlB",
                cls: "span4",
                title: "Penagihan",
                items: [
                        {
                            text: "Pelanggan",
                            type: "controls",
                            items: [
                                { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", type: "popup", btnName: "btnCD", readonly: true, click: "CustomerCode()" },
                                { name: "CustomerName", text: "Nama", cls: "span4 full", placeHolder: "Nama", readonly: true },
                            ]
                        },
                        { name: "Address1", text: "Alamat", cls: "span4 full", placeHolder: "Alamat", readonly: true },
                        { name: "Address2", text: "", cls: "span4", placeHolder: "Alamat 2", readonly: true },
                        { name: "Address3", text: "", cls: "span4", placeHolder: "Alamat 3", readonly: true },
                ]
            },
            {
                name: "pnlC",
                cls: "span4",
                title: "Details Part",
                items: [
                        { model: "detail.PartNo", name: "PartNo", text: "Part No.", cls: "span2", placeHolder: "Part No", readonly: true },
                        { model: "detail.PartNoOriginal", name: "PartNoOriginal", text: "Part No Original", cls: "span2", placeHolder: "Part No Original", readonly: true, hidden: true },
                        { model: "detail.DocNo", name: "DocNo", text: "Doc No", cls: "span2", placeHolder: "DocNO", readonly: true, hidden: true },
                        { model: "detail.DocNo", name: "DocNo", text: "SO No", cls: "span2", placeHolder: "SO No", readonly: true },
                        { model: "detail.QtyBill", name: "QtyBill", text: "Qty. Bill", cls: "span2 number-int", placeHolder: "Qty Bill", readonly: true },
                        { model: "detail.SalesAmt", name: "SalesAmt", text: "Sales Amount", cls: "span2 number-int", placeHolder: "Sales Amount", readonly: true },
                        { model: "detail.DiscPct", name: "DiscPct", text: "Disc %", cls: "span2 number-int", placeHolder: "Disc %" },
                        { model: "detail.PartName", name: "PartName", text: "Part Name", cls: "span2", placeHolder: "Part Name", readonly: true },
                        { model: "detail.WarehouseCode", name: "WarehouseCode", text: "Warehouse Code", cls: "span2", placeHolder: "Warehouse Code", readonly: true },
                        {
                            type: "buttons", cls: "span3 full", items: [
                                { name: "btnSaveDtl", text: "Save", icon: "icon-plus", click: "SaveDtl()", cls: "btn btn-primary", disable: "detail.PartNo === undefined" },
                                { name: "btnClrDtl", text: "Clear", icon: "icon-remove", click: "ClearDtl()", cls: "btn btn-danger", disable: "detail.PartNo === undefined" },
                            ]
                        },
                        {
                            name: "wxpartdetails",
                            type: "wxdiv",
                        },
                        { model: "total.DPP", name: "TotalDPP", text: "Total DPP", cls: "span2 number-int", placeHolder: "Total DPP", readonly: true },
                        { model: "total.PPN", name: "TotalPPN", text: "Total PPN", cls: "span2 number-int", placeHolder: "Total PPN", readonly: true },
                        { model: "total.AMT", name: "TotalAmt", text: "Total Amount", cls: "span2 number-int", placeHolder: "Total Amount", readonly: true },
                ]
            },
            
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spPemeliharaanFakturPenjualanController"); 
    }

});