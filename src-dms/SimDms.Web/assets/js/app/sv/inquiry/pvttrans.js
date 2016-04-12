"use strict";

function PivotTrans($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.createPivot = function()
    {
        var pivot = me.data.JenisPivot;
        if (pivot == "REGSRV") {
            me.RegisterService();
        }
        if (pivot == "PARTSPK")
        {
            me.PartInSpk();
        }
        if (pivot == "LPDIFSC") {
            me.ListPdiFsc();
        }
        if (pivot == "IBOOKING") {
            me.InqBooking();
        }
        if (pivot == "IPDIFSC") {
            me.InqPdiFscBatch();
        }
        if (pivot == "INQCLAIM") {
            me.InqClaimBatch();
        }
    }

    me.RegisterService = function()
    {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvService",
        }

        $.ajax({
            type: "POST",
            url: 'sv.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.data.length !== 0) {
                    window.pivotdata = response.data;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                                "Tanggal SPK": function(mp) {
                                    return moment(mp["JobOrderDate"]).format('DD-MMM-YYYY');
                                },
                                "No SPK": function(mp) {
                                    return mp.JobOrderNo;
                                },
                                "Basic Model": function (mp) {
                                    return mp.BasicModel;
                                },
                                "Status": function (mp) {
                                    return mp.ServiceStatusDesc;
                                },
                                "No Invoice": function (mp) {
                                    return mp.InvoiceNo;
                                },
                                "Service Amount": function (mp) {
                                    return mp.TotalSrvAmount;
                                },
                                "Type Job": function (mp) {
                                    return mp.JobType;
                                },
                        },
                        rows: ["Tanggal SPK"],
                        cols: ["Type Job"],
                        aggregatorName: "Integer Sum",
                        vals: ["Service Amount"],
                        hiddenAttributes: ["JobOrderDate", "JobOrderNo", "BasicModel","ServiceStatusDesc","InvoiceNo","TotalSrvAmount","JobType"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.PartInSpk = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvSpkPart",
        }

        $.ajax({
            type: "POST",
            url: 'sv.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.data.length !== 0) {
                    window.pivotdata = response.data;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "No. SPK": function (mp) {
                                return mp.JobOrderNo;
                            },
                            "No. Part": function (mp) {
                                return mp.PartNo;
                            },
                            "Demand Qty": function (mp) {
                                return mp.DemandQty;
                            },
                            "Supply Qty": function (mp) {
                                return mp.SupplyQty;
                            },
                            "Return Qty": function (mp) {
                                return mp.ReturnQty;
                            },
                            "Cost Price": function (mp) {
                                return mp.CostPrice;
                            },
                        },
                        
                        rows: ["Demand Qty", "Supply Qty", "Return Qty"],
                        aggregatorName: "Integer Sum",
                        vals: ["Cost Price"],
                        hiddenAttributes: ["JobOrderNo", "PartNo", "DemandQty", "SupplyQty", "ReturnQty","CostPrice"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.ListPdiFsc = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvListPdiFsc",
        }

        $.ajax({
            type: "POST",
            url: 'sv.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.data.length !== 0) {
                    window.pivotdata = response.data;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Labor": function (mp) {
                                return mp.LaborDppAmt;
                            },
                            "Part": function (mp) {
                                return mp.PartsDppAmt;
                            },
                            "Material": function (mp) {
                                return mp.MaterialDppAmt;
                            },
                            "Type / Model": function (mp) {
                                return mp.BasicModel;
                            },
                            "Service Amt": function (mp) {
                                return mp.TotalSrvAmt;
                            },
                            "Pekerjaan": function (mp) {
                                return mp.GroupJob;
                            },
                            "Keterangan": function (mp) {
                                return mp.Description;
                            },
                            "No. SPK": function (mp) {
                                return mp.JobOrderNo;
                            },
                        },
                        rows: ["Type / Model", "Keterangan"],
                        cols: ["Pekerjaan"],
                        aggregatorName: "Integer Sum",
                        vals: ["Service Amt"],
                        hiddenAttributes: ["LaborDppAmt", "PartsDppAmt", "MaterialDppAmt", "BasicModel", "TotalSrvAmt", "GroupJob", "Description","JobOrderNo"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.InqBooking = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvInqBooking",
        }

        $.ajax({
            type: "POST",
            url: 'sv.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.data.length !== 0) {
                    window.pivotdata = response.data;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Tanggal Booking": function (mp) {
                                return moment(mp["BookingDate"]).format('DD-MMM-YYYY');
                            },
                            "No. Booking": function (mp) {
                                return mp.BookingNo;
                            },
                            "Jasa DPP Amt": function (mp) {
                                return mp.LaborDppAmt;
                            },
                            "Part DPP Amt": function (mp) {
                                return mp.PartsDppAmt;
                            },
                            "Material DPP Amt": function (mp) {
                                return mp.MaterialDppAmt;
                            },
                            "Type Job": function (mp) {
                                return mp.JobType;
                            },
                            "Total Dpp Amount": function (mp) {
                                return mp.TotalDppAmount;
                            },
                            "Basic Model": function (mp) {
                                return mp.BasicModel;
                            },
                            "No. Reg. Polisi": function (mp) {
                                return mp.PoliceRegNo;
                            },
                        },
                        rows: ["No. Booking", "Tanggal Booking", "Jasa DPP Amt", "Part DPP Amt", "Material DPP Amt"],
                        cols: ["Type Job"],
                        aggregatorName: "Integer Sum",
                        vals: ["Material DPP Amt"],
                        hiddenAttributes: ["BookingDate", "BookingNo", "LaborDppAmt", "PartsDppAmt", "MaterialDppAmt", "JobType","TotalDppAmount","BasicModel","PoliceRegNo"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.InqPdiFscBatch = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvPdiFscBatch",
        }

        $.ajax({
            type: "POST",
            url: 'sv.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.data.length !== 0) {
                    window.pivotdata = response.data;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Tanggal Batch": function (mp) {
                                return moment(mp["BatchDate"]).format('DD-MMM-YYYY');
                            },
                            "No. Batch": function (mp) {
                                return mp.BatchNo;
                            },
                            "Generate No": function (mp) {
                                return mp.GenerateNo;
                            },
                            "Labor Amount": function (mp) {
                                return mp.LaborAmount;
                            },
                            "Seq": function (mp) {
                                return mp.GenerateSeq;
                            },
                            "Pdi Fsc Amt": function (mp) {
                                return mp.PdiFscAmount;
                            },
                            "Receipt No": function (mp) {
                                return mp.ReceiptNo;
                            },
                            "Material Amount": function (mp) {
                                return mp.MaterialAmount;
                            },
                        },
                        rows: ["No. Batch", "Generate No"],
                        cols: ["Receipt No"],
                        aggregatorName: "Integer Sum",
                        vals: ["Pdi Fsc Amt"],
                        hiddenAttributes: ["BatchDate", "BatchNo", "GenerateNo", "GenerateSeq", "LaborAmount", "PdiFscAmount", "ReceiptNo", "MaterialAmount"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.InqClaimBatch = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvClaimBatch",
        }

        $.ajax({
            type: "POST",
            url: 'sv.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.data.length !== 0) {
                    window.pivotdata = response.data;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Tanggal Batch": function (mp) {
                                return moment(mp["BatchDate"]).format('DD-MMM-YYYY');
                            },
                            "No. Batch": function (mp) {
                                return mp.BatchNo;
                            },
                            "Receipt No": function (mp) {
                                return mp.ReceiptNo;
                            },
                            "Generate No": function (mp) {
                                return mp.GenerateNo;
                            },
                            "Issue No": function (mp) {
                                return mp.IssueNo;
                            },
                            "Hour": function (mp) {
                                return mp.OperationHour;
                            },
                            "Operation Amt": function (mp) {
                                return mp.OperationAmt;
                            },
                            "Sublet Amount": function (mp) {
                                return mp.SubletAmt;
                            },
                            "Part Amt": function (mp) {
                                return mp.PartAmt;
                            },
                            "Claim Amt": function (mp) {
                                return mp.ClaimAmt;
                            },
                        },
                        rows: ["No. Batch", "Generate No"],
                        aggregatorName: "Integer Sum",
                        vals: ["Operation Amt"],
                        hiddenAttributes: ["BatchDate", "BatchNo","ReceiptNo","GenerateNo","IssueNo","OperationHour","OperationAmt","SubletAmt","PartAmt","ClaimAmt"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    $('#JenisPivot').select().on("change", function () {
        var tbl = document.getElementById("wxpivotgrid");
        var chld = tbl.firstElementChild;

        if (chld != null) {
            console.log(tbl, chld);
            tbl.innerHTML = "";
        }
    });
    
    me.initialize = function () {
        me.data = {};
        me.data.JenisPivot = 'REGSRV';
        me.data.StartDate = moment().format('YYYY-MM-DD');
        me.data.EndDate = moment().format('YYYY-MM-DD');
    };
    
    me.start();

}

$(document).ready(function () {
    
    var options = {
        title: "Service Transactions",
        xtype: "panels",
        //toolbars: [
        //   { name: "btnPivot", text: "Pivot", cls: "btn btn-info", icon: "icon-search", click:"createPivot()" },
        //],
        panels: [
        {
            name: "pnlA",
            items: [
                    {
                        text: "Jenis",
                        type: "controls",
                        items: [
                            {
                                name: "JenisPivot",
                                type: "select",
                                cls: "span4",
                                readonly: true,
                                items: [
                                    { value: 'REGSRV', text: 'Register Service' },
                                    { value: 'PARTSPK', text: 'Part in SPK' },
                                    { value: 'LPDIFSC', text: 'List PDI & FSC' },
                                    { value: "IBOOKING", text: 'Inquiry Booking' },
                                    { value: "IPDIFSC", text: 'Inquiry PDI FSC Batch' },
                                    { value: "INQCLAIM", text: 'Inquiry Claim Batch' }
                                ]
                            },
                        ]
                    },
                    {
                        text: "Tgl. Transaksi",
                        type: "controls",
                        cls: "span8",
                        items: [
                            { name: "StartDate", cls: "span2", placeHolder: "", type: "ng-datepicker" },
                            { name: "EndDate", cls: "span2", placeHolder: "", type: "ng-datepicker" },
                            {
                                type: "buttons", cls: "span2", items: [
                                    { name: "btnCari", text: "   Query", icon: "icon-search", click: "createPivot()", cls: "button small btn btn-success" },
                                ]
                            },
                        ]
                    },
                   
            ]// end of panel  
        }, {
            name: 'wxpivotgrid',
            xtype: 'wxtable',
            style: 'margin-top: 35px;'
        }
    ] // end of panel
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("PivotTrans");
    }

});