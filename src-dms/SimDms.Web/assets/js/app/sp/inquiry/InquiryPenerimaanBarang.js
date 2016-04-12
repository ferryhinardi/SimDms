var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spInquiryPenerimaanBarangController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.start();

    me.initialize = function()
    {
        me.data.Flag = "opsi1";  
        var ym = me.now("YYYY-MM") + "-01";   
        me.data.StartDate = moment(ym);
        me.data.EndDate = moment(ym).add("months",1).add("days",-1);   
    }

    me.CariData = function()
    {
        var src = "sp.api/inquiry/InquiryPenerimaanBarang";    

        $http.post(src, me.data)
            .success(function(v, status, headers, config){
                if(v.success)
                {
                    switch(me.data.Flag)
                    {
                        case "opsi1": me.refreshGridPemasok(v.data);
                        break;
                       case "opsi2": me.refreshGridBINNING(v.data);
                        break;
                       case "opsi3": me.refreshGridWRS(v.data);
                        break;
                       case "opsi4": me.refreshGridHPP(v.data);
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


    me.refreshGridPemasok = function(result) {
        Wx.kgrid({
            data: result,
            name: "InqRcv",
            serverBinding: false,
            columns: [
                { field: "RowNumber",  title: "No.", width:100, align:"right" },
                { field: "SupplierName",  title: "Nama Pemasok", width:350},
                { field: "BinningNo",  title: "No Binning", width:175 },
                { field: "BinningDate",  title: "Tgl. Binning", width:110, template: "#= (BinningDate===null) ? '' : moment(BinningDate).format('DD MMM YYYY') #" },
                { field: "StatusBinning",  title: "Status Binning", width:180 },
                { field: "WRSNo",  title: "No. WRS", width:150 },
                { field: "WRSDate",  title: "Tanggal WRS", width:150, template: "#= (WRSDate===null) ? '' :  moment(WRSDate).format('DD MMM YYYY') #" },
                { field: "StatusWRS",  title: "Status WRS", width:150 },
                { field: "HPPNo",  title: "No. HPP", width:180 },
                { field: "HPPDate",  title: "Tanggal HPP", width:150, template: "#= (HPPDate===null) ? '' :  moment(HPPDate).format('DD MMM YYYY') #" },
                { field: "StatusHPP",  title: "Status HPP", width:200 },
                { field: "DueDate",  title: "Tgl. Jatuh Tempo", width:150, template: "#=  (DueDate===null) ? '' : moment(DueDate).format('DD MMM YYYY') #" },
                { field: "TaxNo",  title: "No. Seri Pajak", width:190 },
                { field: "TaxDate",  title: "Tgl. Pajak", width:150, template: "#=  (TaxDate===null) ? '' : moment(TaxDate).format('DD MMM YYYY') #" },
                { field: "TaxPeriod",  title: "Masa Pajak", width:150 },
                { field: "PayableAccNo",  title: "Acc. Hutang Dagang", width:250 },
                { field: "TotPurchAmt",  title: "Nilai Kotor", width:150 },
                { field: "TotNetPurchAmt",  title: "DPP", width:150 },
                { field: "TotTaxAmt",  title: "PPN", width:150 },
                { field: "DiffNetPurchAmt",  title: "Selisih DPP", width:150 },
                { field: "DiffTaxAmt",  title: "Selisih PPN", width:150 },
                { field: "TotHPPAmt",  title: "Total HPP", width:150 },
                { field: "IsTransferGL",  title: "Trf. GL", width:100, template: "#= (IsTransferGL) ? 'Y' : 'N'  #"  }
            ],
             detailInit: detailInit
        });
    }


    me.refreshGridWRS = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "InqRcv",
            serverBinding: false,
            resizable: true,
            columns: [
                { field: "RowNumber",  title: "No.", width:100, align:"right" },
                { field: "WRSNo",  title: "No. WRS", width:150 },
                { field: "WRSDate",  title: "Tanggal WRS", width:150, template: "#= (WRSDate===null) ? '' :  moment(WRSDate).format('DD MMM YYYY') #" },
                { field: "StatusWRS",  title: "Status WRS", width:150 },
                
                { field: "BinningNo",  title: "No Binning", width:175 },
                { field: "BinningDate",  title: "Tgl. Binning", width:110, template: "#= (BinningDate===null) ? '' : moment(BinningDate).format('DD MMM YYYY') #" },
                { field: "StatusBinning",  title: "Status Binning", width:180 },
                { field: "HPPNo",  title: "No. HPP", width:180 },
                { field: "HPPDate",  title: "Tanggal HPP", width:150, template: "#= (HPPDate===null) ? '' :  moment(HPPDate).format('DD MMM YYYY') #" },
                { field: "StatusHPP",  title: "Status HPP", width:200 },
                { field: "SupplierName",  title: "Nama Pemasok", width:450 },
               
                { field: "DueDate",  title: "Tgl. Jatuh Tempo", width:150, template: "#=  (DueDate===null) ? '' : moment(DueDate).format('DD MMM YYYY') #" },
                { field: "TaxNo",  title: "No. Seri Pajak", width:190 },
                { field: "TaxDate",  title: "Tgl. Pajak", width:150, template: "#=  (TaxDate===null) ? '' : moment(TaxDate).format('DD MMM YYYY') #" },
                { field: "TaxPeriod",  title: "Masa Pajak", width:150 },
                { field: "PayableAccNo",  title: "Acc. Hutang Dagang", width:250 },
                { field: "TotPurchAmt",  title: "Nilai Kotor", width:150 },
                { field: "TotNetPurchAmt",  title: "DPP", width:150 },
                { field: "TotTaxAmt",  title: "PPN", width:150 },
                { field: "DiffNetPurchAmt",  title: "Selisih DPP", width:150 },
                { field: "DiffTaxAmt",  title: "Selisih PPN", width:150 },
                { field: "TotHPPAmt",  title: "Total HPP", width:150 },
                { field: "IsTransferGL",  title: "Trf. GL", width:100, template: "#= (IsTransferGL) ? 'Y' : 'N'  #"  }
            ],
             detailInit: detailInit
        });
    }


    me.refreshGridBINNING = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "InqRcv",
            serverBinding: false,
            resizable: true,
            columns: [
                { field: "RowNumber",  title: "No.", width:100, align:"right" },
                { field: "BinningNo",  title: "No Binning", width:175 },
                { field: "BinningDate",  title: "Tgl. Binning", width:110, template: "#= (BinningDate===null) ? '' : moment(BinningDate).format('DD MMM YYYY') #" },
                { field: "StatusBinning",  title: "Status Binning", width:180 },
                { field: "WRSNo",  title: "No. WRS", width:150 },
                { field: "WRSDate",  title: "Tanggal WRS", width:150, template: "#= (WRSDate===null) ? '' :  moment(WRSDate).format('DD MMM YYYY') #" },
                { field: "StatusWRS",  title: "Status WRS", width:150 },
                { field: "HPPNo",  title: "No. HPP", width:180 },
                { field: "HPPDate",  title: "Tanggal HPP", width:150, template: "#= (HPPDate===null) ? '' :  moment(HPPDate).format('DD MMM YYYY') #" },
                { field: "StatusHPP",  title: "Status HPP", width:200 },
                { field: "SupplierName",  title: "Nama Pemasok", width:350 },                
                { field: "DueDate",  title: "Tgl. Jatuh Tempo", width:150, template: "#=  (DueDate===null) ? '' : moment(DueDate).format('DD MMM YYYY') #" },
                { field: "TaxNo",  title: "No. Seri Pajak", width:190 },
                { field: "TaxDate",  title: "Tgl. Pajak", width:150, template: "#=  (TaxDate===null) ? '' : moment(TaxDate).format('DD MMM YYYY') #" },
                { field: "TaxPeriod",  title: "Masa Pajak", width:150 },
                { field: "PayableAccNo",  title: "Acc. Hutang Dagang", width:250 },
                { field: "TotPurchAmt",  title: "Nilai Kotor", width:150 },
                { field: "TotNetPurchAmt",  title: "DPP", width:150 },
                { field: "TotTaxAmt",  title: "PPN", width:150 },
                { field: "DiffNetPurchAmt",  title: "Selisih DPP", width:150 },
                { field: "DiffTaxAmt",  title: "Selisih PPN", width:150 },
                { field: "TotHPPAmt",  title: "Total HPP", width:150 },
                { field: "IsTransferGL",  title: "Trf. GL", width:100, template: "#= (IsTransferGL) ? 'Y' : 'N'  #"  }
            ],
             detailInit: detailInit
        });
    }


    me.refreshGridHPP = function(result) {
        Wx.kgrid({
            data: result,
            scrollable:true,
            name: "InqRcv",
            serverBinding: false,
            resizable: true,
            columns: [
                { field: "RowNumber",  title: "No.", width:100, align:"right" },
                { field: "HPPNo",  title: "No. HPP", width:180 },
                { field: "HPPDate",  title: "Tanggal HPP", width:150, template: "#= (HPPDate===null) ? '' :  moment(HPPDate).format('DD MMM YYYY') #" },
                { field: "StatusHPP",  title: "Status HPP", width:200 },
                { field: "BinningNo",  title: "No Binning", width:175 },
                { field: "BinningDate",  title: "Tgl. Binning", width:110, template: "#= (BinningDate===null) ? '' : moment(BinningDate).format('DD MMM YYYY') #" },
                { field: "StatusBinning",  title: "Status Binning", width:180 },
                { field: "WRSNo",  title: "No. WRS", width:150 },
                { field: "WRSDate",  title: "Tanggal WRS", width:150, template: "#= (WRSDate===null) ? '' :  moment(WRSDate).format('DD MMM YYYY') #" },
                { field: "StatusWRS",  title: "Status WRS", width:150 },
               { field: "SupplierName",  title: "Nama Pemasok", width:350 },
                
                { field: "DueDate",  title: "Tgl. Jatuh Tempo", width:150, template: "#=  (DueDate===null) ? '' : moment(DueDate).format('DD MMM YYYY') #" },
                { field: "TaxNo",  title: "No. Seri Pajak", width:190 },
                { field: "TaxDate",  title: "Tgl. Pajak", width:150, template: "#=  (TaxDate===null) ? '' : moment(TaxDate).format('DD MMM YYYY') #" },
                { field: "TaxPeriod",  title: "Masa Pajak", width:150 },
                { field: "PayableAccNo",  title: "Acc. Hutang Dagang", width:250 },
                { field: "TotPurchAmt",  title: "Nilai Kotor", width:150 },
                { field: "TotNetPurchAmt",  title: "DPP", width:150 },
                { field: "TotTaxAmt",  title: "PPN", width:150 },
                { field: "DiffNetPurchAmt",  title: "Selisih DPP", width:150 },
                { field: "DiffTaxAmt",  title: "Selisih PPN", width:150 },
                { field: "TotHPPAmt",  title: "Total HPP", width:150 },
                { field: "IsTransferGL",  title: "Trf. GL", width:100, template: "#= (IsTransferGL) ? 'Y' : 'N'  #"  }
            ],
             detailInit: detailInit
        });
    }

    function detailInit(e) {
        var src = "sp.api/inquiry/InquiryPenerimaanBarangDetail?BinningNo=" + e.data.BinningNo;

        $http.post(src, me.data)
            .success(function(v, status, headers, config){
                if(v.success)
                {
                    if (v.data.length > 0) {
                        $("<div style=\"width:33.33%;\"/>").appendTo(e.detailCell).kendoGrid({
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
                                { field:"RowNumber",  title:"No.", width:20},
                                { field:"BinningNo",  title:"Binning No", width:60 },
                                { field:"DocNo",  title:"Doc No", width:60 },
                                { field:"PartNo",  title:"Part No", width:75 },
                                { field:"PartName",  title:"Part Name", width:145 },
                                { field:"ReceivedBinning",  title:"Jml. Binning", width:40 , format: "{0:#,###}"},
                                { field:"ReceivedWRS",  title:"Jml. WRS", width:40 , format: "{0:#,###}"},
                                { field:"PurchasePrice",  title:"Harga Beli", width:40 , format: "{0:#,###.00}"},
                                { field:"PurchaseAmt",  title:"Nilai Beli", width:40 , format: "{0:#,###.00}"},
                                { field:"DiscPct",  title:"Disk (%)", width:40 , format: "{0:#,###.00}"},
                                { field:"DiscAmt",  title:"Disk (Nilai)", width:40 , format: "{0:#,###.00}"},
                                { field:"TotalAmt",  title:"Total", width:40 , format: "{0:#,###.00}"},
                            ]
                        });
                    }
                    else {
                        $("<div  style=\"width:33.33%;\"/>").appendTo(e.detailCell).kendoGrid({
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
        title: "Inquiry Receiving",
        xtype: "panels",
        panels: [
            {
                name: "pnlA",
                items: [
                        {
                            type: "optionbuttons",
                            name: "jenisIndexs",
                            model: "data.Flag",
                            text: "Indexs",
                            items: [
                                { name: "opsi1", text: "PEMASOK" },
                                { name: "opsi2", text: "BINING LIST" },
                                { name: "opsi3", text: "WRS" },
                                { name: "opsi4", text: "HPP" },
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
                name: "InqRcv",
                xtype: "k-grid",
            },  
            {
                name: "pnlC",              
                title: "TOTAL",
                cls: "span8",
                items: [
                        { name: "NilaiKotor", text: "Nilai Kotor", cls: "span3 number", placeHolder: "0", model:"total.TotPurchAmt" },
                        { name: "DPP", text: "DPP", cls: "span2  number", placeHolder: "0", model:"total.TotNetPurchAmt"  },
                        { name: "PPN",  text: "PPN", cls: "span2 number", placeHolder: "0" , model:"total.TotTaxAmt" },
                        { name: "Total", text: "Total HPP", cls: "span3 number", placeHolder: "0" , model:"total.TotHPPAmt" },                       
                        { name: "sDpp", text: "Selisih DPP", cls: "span2 number", placeHolder: "0", model:"total.DiffNetPurchAmt"  },
                        { name: "sPPN", text: "Selisih PPN", cls: "span2  number", placeHolder: "0", model:"total.DiffTaxAmt"  },
                ]
            }            
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spInquiryPenerimaanBarangController"); 
    }

});