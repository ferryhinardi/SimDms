var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spInquirySupplySlipController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function()
    {
        me.data.Flag = "PELANGGAN";  
        var ym = me.now("YYYY-MM") + "-01";   
        me.data.StartDate = moment(ym);
        me.data.EndDate = moment(ym).add("months",1).add("days",-1);   
    }

    me.start();

    me.CariData = function()
    {
        var src = "sp.api/inquiry/InquirySupplySlip";    
        layout.loadAjaxLoader();
        $http.post(src, me.data)
            .success(function(v, status, headers, config){
                if(v.success)
                {
                    switch(me.data.Flag)
                    {
                        case "PELANGGAN": me.refreshGrid1(v.data);
                        break;
                       case "SUPPLYSLIP": me.refreshGrid2(v.data);
                        break;
                       case "SPK": me.refreshGrid3(v.data);
                        break;
                       case "POLICEREGNO": me.refreshGrid4(v.data);
                        break;
                        case "LAMPIRAN": me.refreshGrid5(v.data);
                        break;
                        default:
                    }
                    me.mtotal = v.total[0];
                } 
                else 
                {
                    // show an error message
                    MsgBox(v.message, MSG_ERROR);
                }
            }).error(function(e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
        }); 
    }

    Wx.kgrid({
        scrollable:true,
        name: "wXgrid1",
        serverBinding: false,
        resizable: true,
        columns: [
                { field:"Nomor",  title:"No.", width:50 },
                { field:"FinishCon",  title:"Finish", width:70, template: "#= (FinishCon) ? 'Y' : 'N'  #" },
                { field:"Type",  title:"Type", width:100 },
                { field:"CustomerName",  title:"Nama Pelanggan", width:300 },

                { field:"SSNo",  title:"No. Suply Slip", width:120 },                   
                { field:"SSDate",  title:"Tgl. Suply Slip", width:110 , template: "#=  (SSDate===null) ? '' : moment(SSDate).format('DD MMM YYYY') #"},

                { field:"SPKNo",  title:"No. SPK", width:140 },
                { field:"SPKDate",  title:"Tgl. SPK", width:100 , template: "#=  (SPKDate===null) ? '' : moment(SPKDate).format('DD MMM YYYY') #"},

                { field:"LmpNo",  title:"No. Lampiran", width:120 },
                { field:"LmpDate",  title:"Tgl. Lampiran", width:110 , template: "#=  (LmpDate===null) ? '' : moment(LmpDate).format('DD MMM YYYY') #"},                    

            ]
    });

    Wx.kgrid({
        scrollable:true,
        name: "wXgrid2",
        serverBinding: false,
        resizable: true,
        columns: [
                { field:"JobOrderNo",  title:"Nomor SPK", width:110 },
                { field:"InvoiceNo",  title:"No. Invoice", width:110 },                
                { field:"ProductType",  title:"PT", width:50 },
                { field:"PartNo",  title:"Nomor Part", width:120 },
                { field:"PartName",  title:"Nama Part", width:200 },
                { field:"DemandQty",  title:"Demand", width:100 },
                { field:"SupplyQty",  title:"Supply", width:100 },
                { field:"ReturnQty",  title:"Return", width:100 },
                { field:"RetailPrice",  title:"RetailPrice", width:100 }
            ]
    });

    me.refreshGrid1 = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "wXgrid1",
            serverBinding: false,
            resizable: true,
            change: grid_change,
            columns: [
                    { field:"Nomor",  title:"No.", width:50 },
                    { field:"FinishCon",  title:"Finish", width:70, 
                    template: "#= (FinishCon) ? 'Y' : 'N'  #" },
                    { field:"Type",  title:"Type", width:100 },
                    { field:"CustomerName",  title:"Nama Pelanggan", width:300 },

                    { field:"SSNo",  title:"No. Suply Slip", width:120 },                   
                    { field:"SSDate",  title:"Tgl. Suply Slip", width:110 , 
                    template: "#=  (SSDate===null) ? '' : moment(SSDate).format('DD MMM YYYY') #"},

                    { field:"SPKNo",  title:"No. SPK", width:140 },
                    { field:"SPKDate",  title:"Tgl. SPK", width:110 , 
                    template: "#=  (SPKDate===null) ? '' : moment(SPKDate).format('DD MMM YYYY') #"},

                    { field:"LmpNo",  title:"No. Lampiran", width:120 },
                    { field:"LmpDate",  title:"Tgl. Lampiran", width:110 , 
                    template: "#=  (LmpDate===null) ? '' : moment(LmpDate).format('DD MMM YYYY') #"},                    

                    // { field:"PickingNo",  title:"PickingNo", width:0, visible: false },
                    // { field:"PickingDate",  title:"PickingDate", width:0 , visible: false },

                    // { field:"SP",  title:"SP", width:100, visible: false  },
                    // { field:"finish",  title:"finish", width:100 , visible: false },
                    // { field:"PoliceNo",  title:"PoliceNo", width:100 },
                   
                    // { field:"PartsGrossAmtTot",  title:"PartsGrossAmtTot", width:100 },
                    // { field:"PartsDiscAmtTot",  title:"PartsDiscAmtTot", width:100 },
                    // { field:"PartsDPPAmtTot",  title:"PartsDPPAmtTot", width:100 },
                ]
        });

    }

    me.refreshGrid2 = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "wXgrid1",
            serverBinding: false,
            resizable: true,
            change: grid_change,
            columns: [
                    { field:"Nomor",  title:"No.", width:50 },
                    { field:"FinishCon",  title:"Finish", width:50, template: "#= (FinishCon) ? 'Y' : 'N'  #" },
                    { field:"Type",  title:"Type", width:100 },
                    { field:"SSNo",  title:"No. Suply Slip", width:120 },                   
                    { field:"SSDate",  title:"Tgl. Suply Slip", width:140 , 
                    template: "#=  (SSDate===null) ? '' : moment(SSDate).format('DD MMM YYYY') #"},
                    { field:"LmpNo",  title:"No. Lampiran", width:120 },
                    { field:"LmpDate",  title:"Tgl. Lampiran", width:140 , 
                    template: "#=  (LmpDate===null) ? '' : moment(LmpDate).format('DD MMM YYYY') #"},   
                    { field:"SPKNo",  title:"No. SPK", width:140 },
                    { field:"SPKDate",  title:"Tgl. SPK", width:100 , 
                    template: "#=  (SPKDate===null) ? '' : moment(SPKDate).format('DD MMM YYYY') #"},
                    { field:"CustomerName",  title:"Nama Pelanggan", width:300 },
                ]
        });
    }

    me.refreshGrid3 = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "wXgrid1",
            serverBinding: false,
            resizable: true,
            change: grid_change,
            columns: [
                    { field:"Nomor",  title:"No.", width:50 },
                    { field:"FinishCon",  title:"Finish", width:50, template: "#= (FinishCon) ? 'Y' : 'N'  #" },
                    { field:"Type",  title:"Type", width:100 },
                     { field:"SPKNo",  title:"No. SPK", width:140 },
                    { field:"SPKDate",  title:"Tgl. SPK", width:100 , 
                    template: "#=  (SPKDate===null) ? '' : moment(SPKDate).format('DD MMM YYYY') #"},
                    { field:"LmpNo",  title:"No. Lampiran", width:120 },
                    { field:"LmpDate",  title:"Tgl. Lampiran", width:140 , 
                    template: "#=  (LmpDate===null) ? '' : moment(LmpDate).format('DD MMM YYYY') #"},                    
                    { field:"SSNo",  title:"No. Suply Slip", width:120 },                   
                    { field:"SSDate",  title:"Tgl. Suply Slip", width:140 , 
                    template: "#=  (SSDate===null) ? '' : moment(SSDate).format('DD MMM YYYY') #"},
                    { field:"CustomerName",  title:"Nama Pelanggan", width:300 },
                ]
        });
    }

    me.refreshGrid4 = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "wXgrid1",
            serverBinding: false,
            resizable: true,
            change: grid_change,
            columns: [
                   { field:"Nomor",  title:"No.", width:50 },
                    { field:"FinishCon",  title:"Finish", width:50, template: "#= (FinishCon) ? 'Y' : 'N'  #" },
                    { field:"Type",  title:"Type", width:100 },
                    { field:"PoliceNo",  title:"No. Police", width:100 },
                    { field:"SSNo",  title:"No. Suply Slip", width:120 },                   
                    { field:"SSDate",  title:"Tgl. Suply Slip", width:140 , 
                    template: "#=  (SSDate===null) ? '' : moment(SSDate).format('DD MMM YYYY') #"},
                    { field:"LmpNo",  title:"No. Lampiran", width:120 },
                    { field:"LmpDate",  title:"Tgl. Lampiran", width:140 , 
                    template: "#=  (LmpDate===null) ? '' : moment(LmpDate).format('DD MMM YYYY') #"},  
                    { field:"SPKNo",  title:"No. SPK", width:140 },
                    { field:"SPKDate",  title:"Tgl. SPK", width:100 , 
                    template: "#=  (SPKDate===null) ? '' : moment(SPKDate).format('DD MMM YYYY') #"},
                    { field:"CustomerName",  title:"Nama Pelanggan", width:300 },
                ]
        });
    }

    me.refreshGrid5 = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "wXgrid1",
            serverBinding: false,
            resizable: true,
            change: grid_change,
            columns: [
                    { field:"Nomor",  title:"No.", width:50 },
                    { field:"FinishCon",  title:"Finish", width:50, template: "#= (FinishCon) ? 'Y' : 'N'  #" },
                    { field:"Type",  title:"Type", width:100 },
                    { field:"LmpNo",  title:"No. Lampiran", width:120 },
                    { field:"LmpDate",  title:"Tgl. Lampiran", width:140 , 
                    template: "#=  (LmpDate===null) ? '' : moment(LmpDate).format('DD MMM YYYY') #"},                    
                    { field:"PoliceNo",  title:"No. Police", width:100 },
                    { field:"SSNo",  title:"No. Suply Slip", width:120 },                   
                    { field:"SSDate",  title:"Tgl. Suply Slip", width:140 , 
                    template: "#=  (SSDate===null) ? '' : moment(SSDate).format('DD MMM YYYY') #"},
                    { field:"SPKNo",  title:"No. SPK", width:140 },
                    { field:"SPKDate",  title:"Tgl. SPK", width:100 , 
                    template: "#=  (SPKDate===null) ? '' : moment(SPKDate).format('DD MMM YYYY') #"},
                    { field:"CustomerName",  title:"Nama Pelanggan", width:300 },
                ]
        });
    }

    function grid_change(e) {
        e.preventDefault();
        var grid = e.sender;
        var d = grid.dataItem(this.select());
        detailInit(d);
    }

   function detailInit(d) {
        var src = "sp.api/inquiry/InquirySupplySlipDetail?JO=" + d.SPKNo + "&SS=" + d.SSNo;
        $http.post(src)
            .success(function(v, status, headers, config){
                if(v.success) {                    

                       Wx.kgrid({
                            data: v.data,
                            name: "wXgrid2",
                            pageable: true,
                            scrollable:true,
                            serverBinding: false,
                            resizable: true,
                            selectable: "multiple",   
                            pageable: {
                                refresh: true,
                                pageSizes: true,
                                buttonCount: 5
                            },                         
                            columns: [
                                { field:"JobOrderNo",  title:"Nomor SPK", width:110 },
                                { field:"InvoiceNo",  title:"No. Invoice", width:110 },                
                                { field:"ProductType",  title:"PT", width:50 },
                                { field:"PartNo",  title:"Nomor Part", width:120 },
                                { field:"PartName",  title:"Nama Part", width:200 },
                                { field:"DemandQty",  title:"Demand", width:100 , format: "{0:#,##0}"},
                                { field:"SupplyQty",  title:"Supply", width:100 , format: "{0:#,##0}"},
                                { field:"ReturnQty",  title:"Return", width:100, format: "{0:#,##0}" },
                                { field:"RetailPrice",  title:"RetailPrice", width:100 , format: "{0:#,##0}"}
                            ]
                        });

                        me.dtotal = v.total[0];

                } 
                else 
                {
                    // show an error message
                    MsgBox(v.message, MSG_ERROR);
                }
            }).error(function(e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
        }); 
    }
}


$(document).ready(function () {
    var options = {
        title: "Inquiry Supply Slip",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                title: "Parameters",
                items: [
                        {
                            
                            type: "optionbuttons",
                            name: "jenisIndexs",
                            model: "data.Flag",
                            text: "Indexs",
                            items: [
                                { name: "PELANGGAN", text: "PELANGGAN" },
                                { name: "SUPPLYSLIP", text: "SUPPLY SLIP" },
                                { name: "SPK", text: "SPK" },
                                { name: "POLICEREGNO", text: "POLISI REG. NO" },
                                { name: "LAMPIRAN", text: "LAMPIRAN" },
                            ]
                        }, 
                        { name: "parameter",  text: "Parameter", cls: "span8", placeHolder: "" },
                        {
                            text: "Tgl. Pencarian",
                            type: "controls",
                            cls: "span8",
                            items: [
                                { name: "StartDate", cls: "span2", placeHolder: "", type: "ng-datepicker"},
                                { name: "EndDate", cls: "span2", placeHolder: "", type: "ng-datepicker"},
                                {
                                    type: "buttons", cls: "span2", items: [
                                        { name: "btnCari", text: "   Cari", icon: "icon-search", click:"CariData()", cls: "button small btn btn-success"},
                                    ]
                                },  
                            ]
                        },
                    ]   
            },
            {
                name: "wXgrid1",
                xtype: "k-grid",                            
            },             
            {
                name: "pnlB",              
                cls: "span8",
                items: [ 
                        { type: "label", text: "<b>GRAND TOTAL SUPPLY SLIP :</b>", cls: "span2 number label-valign"}, 
                        { name: "Kotor",  model: "mtotal.PartsGrossAmt",  text: "Kotor", cls: "span2 int", placeHolder: "0" },                          
                        { name: "Diskon",  model: "mtotal.PartsDiscAmt", text: "Diskon", cls: "span2 int", placeHolder: "0" },                                              
                        { name: "DPP",  model: "mtotal.PartsDPPAmt", text: "DPP", cls: "span2 int", placeHolder: "0" },                          
                ]
            },       
            {
                name: "wXgrid2",
                xtype: "k-grid",                            
            },                  
            {
                name: "pnlD",              
                cls: "span8",
                items: [
    
                        { type: "label", text: "<b>TOTAL PER SUPPLY SLIP :</b>", cls: "span2 number label-valign"}, 
                        { name: "Kotor2",  model: "dtotal.SalesAmt",  text: "Kotor", cls: "span2 int", placeHolder: "0" },                         
                        { name: "Diskon2", model: "dtotal.DiscAmt", text: "Diskon", cls: "span2 int", placeHolder: "0" },                                              
                        { name: "DPP2",  model: "dtotal.DPPAmt", text: "DPP", cls: "span2 int", placeHolder: "0" },                                             
                ]
            },             
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spInquirySupplySlipController"); 
    }

});