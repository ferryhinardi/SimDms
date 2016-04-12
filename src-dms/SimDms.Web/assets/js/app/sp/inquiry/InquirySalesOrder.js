var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spInquirySalesOrderController($scope, $http, $injector) {

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
        var src = "sp.api/inquiry/InquirySalesOrder";    

        $http.post(src, me.data)
            .success(function(v, status, headers, config){
                if(v.success)
                {
                    switch(me.data.Flag)
                    {
                        case "PELANGGAN": me.refreshGridPLG(v.data);
                        break;
                       case "PICKINGLIST": me.refreshGridPL(v.data);
                        break;
                       case "FAKTURPENJUALAN": me.refreshGridFK(v.data);
                        break;
                       case "FAKTURPAJAK": me.refreshGridPJK(v.data);
                        break;
                        default:
                    }
                    me.total = v.total[0];
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


    me.refreshGridPLG = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "inqSO",
            serverBinding: false,
            resizable: true,
            columns: [
                    { field:"RowNumber",  title:"No.", width:50 },
                    { field:"PaymentBy",  title:"Cara Bayar", width:120 },
                    { field:"CustomerName",  title:"Nama Pelanggan", width:300 },                    
                    { field:"PickingSlipNo",  title:"No. P/L", width:120 },
                    { field:"PickingSlipDate",  title:"Tgl. P/L", width:120 , template: "#=  (PickingSlipDate===null) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #" },
                    { field:"StatusPickingSlip",  title:"Status Picking", width:180 },
                    { field:"FPJNo",  title:"No. Faktur", width:150 },
                    { field:"FPJDate",  title:"Tgl. Faktur", width:120, template: "#=  (FPJDate===null) ? '' : moment(FPJDate).format('DD MMM YYYY') #"  },
                    { field:"FPJGovNo",  title:"No. Seri Pajak", width:120 },                    
                    { field:"StatusFPJ",  title:"Status Faktur", width:175 },
                    { field:"DueDate",  title:"Jth. Tempo", width:120, template: "#=  (DueDate===null) ? '' : moment(DueDate).format('DD MMM YYYY') #"  },                   
                    { field:"InvoiceNo",  title:"No. Invoice", width:120 },
                    { field:"InvoiceDate",  title:"Tgl. Invoice", width:120 , template: "#=  (InvoiceDate===null) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                    { field:"StatusInvoice",  title:"Status Invoice", width:200 },
                    { field:"FPJSignature",  title:"Tgl. Pajak", width:120 , template: "#=  (FPJSignature===null) ? '' : moment(FPJSignature).format('DD MMM YYYY') #"},
                    { field:"TOPDays",  title:"TOP", width:100 },
                    { field:"TotSalesAmt",  title:"Nilai Kotor", width:100 , format: "{0:#,###.00}"},
                    { field:"TotDiscAmt",  title:"Diskon", width:100 , format: "{0:#,###.00}"},
                    { field:"TotDPPAmt",  title:"DPP", width:100 , format: "{0:#,###.00}"},
                    { field:"TotPPNAmt",  title:"PPN", width:100, format: "{0:#,###.00}" },
                    { field:"TotFinalSalesAmt",  title:"Total", width:100 , format: "{0:#,###.00}"},
                    { field:"IsTransferGL",  title:"Trf. GL", width:100 , template: "#= (IsTransferGL) ? 'Y' : 'N'  #" },
                ],
                 detailInit: detailInit
        });
    }


    me.refreshGridPL = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "inqSO",
            serverBinding: false,
            resizable: true,
            columns: [
                    { field:"RowNumber",  title:"No.", width:50 },
                    { field:"PaymentBy",  title:"Cara Bayar", width:120 },
                    { field:"PickingSlipNo",  title:"No. P/L", width:120 },
                    { field:"PickingSlipDate",  title:"Tgl. P/L", width:120 , template: "#=  (PickingSlipDate===null) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #" },
                    { field:"StatusPickingSlip",  title:"Status Picking", width:180 },
                    { field:"InvoiceNo",  title:"No. Invoice", width:120 },
                    { field:"InvoiceDate",  title:"Tgl. Invoice", width:120 , template: "#=  (InvoiceDate===null) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                    { field:"StatusInvoice",  title:"Status Invoice", width:200 },
                    { field:"FPJNo",  title:"No. Faktur", width:150 },
                    { field:"FPJDate",  title:"Tgl. Faktur", width:120, template: "#=  (FPJDate===null) ? '' : moment(FPJDate).format('DD MMM YYYY') #"  },
                    { field:"StatusFPJ",  title:"Status Faktur", width:175 },
                    { field:"CustomerName",  title:"Nama Pelanggan", width:300 },
                    { field:"DueDate",  title:"Jth. Tempo", width:120, template: "#=  (DueDate===null) ? '' : moment(DueDate).format('DD MMM YYYY') #"  },
                    { field:"FPJGovNo",  title:"No. Seri Pajak", width:120 },
                    { field:"FPJSignature",  title:"Tgl. Pajak", width:120, template: "#=  (FPJSignature===null) ? '' : moment(FPJSignature).format('DD MMM YYYY') #" },
                    { field:"TOPDays",  title:"TOP", width:100 },
                    { field:"TotSalesAmt",  title:"Nilai Kotor", width:100 , format: "{0:#,###.00}"},
                    { field:"TotDiscAmt",  title:"Diskon", width:100 , format: "{0:#,###.00}"},
                    { field:"TotDPPAmt",  title:"DPP", width:100 , format: "{0:#,###.00}"},
                    { field:"TotPPNAmt",  title:"PPN", width:100, format: "{0:#,###.00}" },
                    { field:"TotFinalSalesAmt",  title:"Total", width:100 , format: "{0:#,###.00}"},
                    { field:"IsTransferGL",  title:"Trf. GL", width:100 , template: "#= (IsTransferGL) ? 'Y' : 'N'  #" },
            ],
             detailInit: detailInit
        });
    }


    me.refreshGridFK = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "inqSO",
            serverBinding: false,
            resizable: true,
            columns: [
                    { field:"RowNumber",  title:"No.", width:50 },
                    { field:"PaymentBy",  title:"Cara Bayar", width:120 },
                    { field:"FPJNo",  title:"No. Faktur", width:150 },
                    { field:"FPJDate",  title:"Tgl. Faktur", width:120, template: "#=  (FPJDate===null) ? '' : moment(FPJDate).format('DD MMM YYYY') #"  },
                                      
                    { field:"StatusFPJ",  title:"Status Faktur", width:175 },                    
                    { field:"PickingSlipNo",  title:"No. P/L", width:120 },
                    { field:"PickingSlipDate",  title:"Tgl. P/L", width:120 , template: "#=  (PickingSlipDate===null) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #" },
                    { field:"StatusPickingSlip",  title:"Status Picking", width:180 },
                    { field:"FPJGovNo",  title:"No. Seri Pajak", width:120 }, 
                    { field:"FPJSignature",  title:"Tgl. Pajak", width:120 , template: "#=  (FPJSignature===null) ? '' : moment(FPJSignature).format('DD MMM YYYY') #"},

                    { field:"InvoiceNo",  title:"No. Invoice", width:120 },
                    { field:"InvoiceDate",  title:"Tgl. Invoice", width:120 , template: "#=  (InvoiceDate===null) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                    { field:"StatusInvoice",  title:"Status Invoice", width:200 },
                    { field:"CustomerName",  title:"Nama Pelanggan", width:300 },                    
                    { field:"DueDate",  title:"Jth. Tempo", width:120, template: "#=  (DueDate===null) ? '' : moment(DueDate).format('DD MMM YYYY') #"  },                   
                    { field:"TOPDays",  title:"TOP", width:100 },
                    { field:"TotSalesAmt",  title:"Nilai Kotor", width:100 , format: "{0:#,###.00}"},
                    { field:"TotDiscAmt",  title:"Diskon", width:100 , format: "{0:#,###.00}"},
                    { field:"TotDPPAmt",  title:"DPP", width:100 , format: "{0:#,###.00}"},
                    { field:"TotPPNAmt",  title:"PPN", width:100, format: "{0:#,###.00}" },
                    { field:"TotFinalSalesAmt",  title:"Total", width:100 , format: "{0:#,###.00}"},
                    { field:"IsTransferGL",  title:"Trf. GL", width:100 , template: "#= (IsTransferGL) ? 'Y' : 'N'  #" },
            ],
             detailInit: detailInit
        });
    }


    me.refreshGridPJK = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "inqSO",
            serverBinding: false,
            resizable: true,
            columns: [
                    { field:"RowNumber",  title:"No.", width:50 },
                    { field:"PaymentBy",  title:"Cara Bayar", width:120 },

                    { field:"FPJGovNo",  title:"No. Seri Pajak", width:120 }, 
                    { field:"FPJSignature",  title:"Tgl. Pajak", width:120 , template: "#=  (FPJSignature===null) ? '' : moment(FPJSignature).format('DD MMM YYYY') #"},

                    { field:"PickingSlipNo",  title:"No. P/L", width:120 },
                    { field:"PickingSlipDate",  title:"Tgl. P/L", width:120 , template: "#=  (PickingSlipDate===null) ? '' : moment(PickingSlipDate).format('DD MMM YYYY') #" },
                    { field:"StatusPickingSlip",  title:"Status Picking", width:180 },
 
                    { field:"FPJNo",  title:"No. Faktur", width:150 },
                    { field:"FPJDate",  title:"Tgl. Faktur", width:120, template: "#=  (FPJDate===null) ? '' : moment(FPJDate).format('DD MMM YYYY') #"  },                                      
                    { field:"StatusFPJ",  title:"Status Faktur", width:175 },   
                    { field:"InvoiceNo",  title:"No. Invoice", width:120 },
                    { field:"InvoiceDate",  title:"Tgl. Invoice", width:120 , template: "#=  (InvoiceDate===null) ? '' : moment(InvoiceDate).format('DD MMM YYYY') #" },
                    { field:"StatusInvoice",  title:"Status Invoice", width:200 },
                    { field:"CustomerName",  title:"Nama Pelanggan", width:300 },                    
                    { field:"DueDate",  title:"Jth. Tempo", width:120, template: "#=  (DueDate===null) ? '' : moment(DueDate).format('DD MMM YYYY') #"  },                   
                    { field:"TOPDays",  title:"TOP", width:100 },
                    { field:"TotSalesAmt",  title:"Nilai Kotor", width:100 , format: "{0:#,###.00}"},
                    { field:"TotDiscAmt",  title:"Diskon", width:100 , format: "{0:#,###.00}"},
                    { field:"TotDPPAmt",  title:"DPP", width:100 , format: "{0:#,###.00}"},
                    { field:"TotPPNAmt",  title:"PPN", width:100, format: "{0:#,###.00}" },
                    { field:"TotFinalSalesAmt",  title:"Total", width:100 , format: "{0:#,###.00}"},
                    { field:"IsTransferGL",  title:"Trf. GL", width:100 , template: "#= (IsTransferGL) ? 'Y' : 'N'  #" },
            ],
             detailInit: detailInit
        });
    }

    function detailInit(e) {
        var src = "sp.api/inquiry/InquirySalesOrderDetail?pickingSlipNo=" + e.data.PickingSlipNo;

        $http.post(src, me.data)
            .success(function(v, status, headers, config){
                if(v.success)
                {
                    if (v.data.length > 0) {
                        $("<div style=\"width:45%;\"/>").appendTo(e.detailCell).kendoGrid({
                            dataSource: { data: v.data, pageSize: 10 },
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
                                { field:"RowNumber",  title:"No.", width:40 },
                                { field:"PickingSlipNo",  title:"No. P/L", width:90 },
                                { field:"DocNo",  title:"No. Dokumen", width:100 },
                                { field:"PartNo",  title:"No. Part", width:100 },
                                { field:"PartNoOriginal",  title:"No. Part Original", width:100 },
                                { field:"QtyOrder",  title:"Jml. SO", width:70, format: "{0:#,###.00}" },
                                { field:"QtyPicked",  title:"Jml. PL", width:70, format: "{0:#,###.00}" },
                                { field:"QtyBill",  title:"Jml. Invoice", width:70 , format: "{0:#,###.00}"},
                                { field:"QtyFPJ",  title:"Jml. Faktur", width:70, format: "{0:#,###.00}" },
                                { field:"RetailPrice",  title:"Harga Jual", width:70 , format: "{0:#,###.00}"},
                                { field:"SalesAmt",  title:"Nilai Kotor", width:70 , format: "{0:#,###.00}"},
                                { field:"DiscAmt",  title:"Diskon", width:70, format: "{0:#,###.00}" },
                                { field:"NetSalesAmt",  title:"DPP", width:70 , format: "{0:#,###.00}"},
                            ]
                        });
                    }
                    else {
                        $("<div  style=\"width:45%;\"/>").appendTo(e.detailCell).kendoGrid({
                            dataSource: { data: [{ Info: "No items to display" }] },
                            columns: [{ field: "Info", title: "Info" }]
                        });
                    }
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
        title: "Inquiry Sales Order",
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
                                { name: "PICKINGLIST", text: "PICKING LIST" },
                                { name: "FAKTURPENJUALAN", text: "FAKTUR PENJUALAN" },
                                { name: "FAKTURPAJAK", text: "FAKTUR PAJAK" },
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
                name: "inqSO",
                xtype: "k-grid",                           

            },  
            {
                name: "pnlC",              
                title: "GRAND TOTAL",
                cls: "span8",
                items: [
                        { name: "t1", model: "total.TotSalesAmt", text: "Nilai Kotor", cls: "span3 number", placeHolder: "0" },
                        { name: "t2", model: "total.TotDiscAmt", text: "Diskon", cls: "span2 number", placeHolder: "0" },                        
                        { name: "t3", model: "total.TotDPPAmt", text: "DPP", cls: "span2  number", placeHolder: "0" },
                        { name: "t4", model: "total.TotFinalSalesAmt", text: "Total", cls: "span3 number", placeHolder: "0" },                       
                        { name: "t5", model: "total.TotPPNAmt",  text: "PPN", cls: "span2 number", placeHolder: "0" },
                ]
            },              
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spInquirySalesOrderController"); 
    }

});