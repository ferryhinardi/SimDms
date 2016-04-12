"use strict";

function PivotTransINV($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    
    me.createPivot = function()
    {
        var pivot = me.data.JenisPivot;
        if (pivot == "REGINV") {
            me.RegisterInvoice();
        }
        if (pivot == "SRVINV") {
            me.SvInvoiceUnit();
        }
        if (pivot == "SRVMSI") {
            me.SvActivityMsi();
        }
    }

    me.RegisterInvoice = function ()
    {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvInvoice",
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
                            "Tanggal Inv.": function (mp) {
                                return moment(mp["InvoiceDate"]).format('DD-MMM-YYYY');
                            },
                            "Group Inv.": function (mp) {
                                return mp.InvGroup;
                            },
                            "No. Invoice": function (mp) {
                                return mp.InvoiceNo;
                            },
                            "No. SPK": function (mp) {
                                return mp.JobOrderNo;
                            },
                            "Labor Calc Amt": function (mp) {
                                return mp.LaborCalcAmt;
                            },
                            "Labor Dpp Amt": function (mp) {
                                return mp.LaborDppAmt;
                            },
                            "Parts Dpp Amt": function (mp) {
                                return mp.PartsDppAmt;
                            },
                            "Material Dpp Amt": function (mp) {
                                return mp.MaterialDppAmt;
                            },
                            "Dpp Amt": function (mp) {
                                return mp.TotalDppAmt;
                            },
                            "Tax Amt": function (mp) {
                                return mp.TotalTaxAmt;
                            },
                            "Total Srv Amt": function (mp) {
                                return mp.TotalSrvAmt;
                            }
                        },
                        rows: ["Tanggal Inv."],
                        cols: ["Group Inv."],
                        aggregatorName: "Integer Sum",
                        vals: ["Total Srv Amt"],
                        hiddenAttributes: ["InvoiceDate", "InvGroup","InvoiceNo","JobOrderNo","LaborCalcAmt","LaborDppAmt","PartsDppAmt","MaterialDppAmt","TotalDppAmt","TotalTaxAmt","TotalSrvAmt"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.SvInvoiceUnit = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvInvUnit",
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
                            "Tanggal SPK.": function (mp) {
                                return moment(mp["JobOrderDate"]).format('DD-MMM-YYYY');
                            },
                            "Job Group": function (mp) {
                                return mp.JobGroup;
                            },
                            "Job Type Desc": function (mp) {
                                return mp.JobTypeDesc;
                            },
                            "Job Type": function (mp) {
                                return mp.JobType;
                            },
                            "Chassis Code": function (mp) {
                                return mp.ChassisCode;
                            },
                            "Chassis No": function (mp) {
                                return mp.ChassisNo;
                            },
                        },
                        rows: ["Job Type Desc"],
                        cols: ["Job Group"],
                        aggregatorName: "Count",
                        vals: ["Unit"],
                        hiddenAttributes: ["JobOrderDate", "JobGroup", "JobTypeDesc", "JobType","ChassisCode","ChassisNo"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.SvActivityMsi = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvAktMsi",
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
                            "Tanggal SPK": function (mp) {
                                return moment(mp["JobOrderDate"]).format('MMM-DD');
                            },
                            "Tanggal Inv.": function (mp) {
                                return moment(mp["InvoiceDate"]).format('MMM-DD');
                            },
                            "Chassis Code": function (mp) {
                                return mp.ChassisCode;
                            },
                            "Chassis No": function (mp) {
                                return mp.ChassisNo;
                            },
                            "Job Type": function (mp) {
                                return mp.JobType;
                            },
                            "Invoice No": function (mp) {
                                return mp.InvoiceNo;
                            },
                        },
                        rows: ["Tanggal SPK"],
                        cols: ["Job Type"],
                        aggregatorName: "Count",
                        vals: ["Job Type"],
                        hiddenAttributes: ["JobOrderDate", "InvoiceDate", "ChassisCode", "JobType", "InvoiceNo"],
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
        me.data.JenisPivot = 'REGINV';
        me.data.StartDate = moment().format('YYYY-MM-DD');
        me.data.EndDate = moment().format('YYYY-MM-DD');
    };
    
    me.start();

}



$(document).ready(function () {
    
    var options = {
        title: "Service Invoice",
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
                                    { value: 'REGINV', text: 'Register Invoice' },
                                    { value: 'SRVINV', text: 'Service Invoice vs Unit' },
                                    { value: 'SRVMSI', text: 'Service Aktivity vs MSI' },
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
        SimDms.Angular("PivotTransINV");
    }

});