"use strict";

function PivotTransJN($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    
    me.createPivot = function()
    {       
        var pivot = me.data.JenisPivot;
        if (pivot == "INVGL") {
            me.PartMaterial();
        }
        if (pivot == "GLIFC") {
            me.GlInterface();
        }
        if (pivot == "INVARGL") {
            me.InvArGl();
        }
    }

    me.PartMaterial = function ()
    {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvInvGL",
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
                                "Tanggal Inv.": function(mp) {
                                    return moment(mp["InvoiceDate"]).format('DD-MMM-YYYY');
                                },
                                "Inv Group": function (mp) {
                                    return mp.InvGroup;
                                },
                                "No Invoice": function (mp) {
                                    return mp.InvoiceNo;
                                },
                                "No SPK": function (mp) {
                                    return mp.JobOrderNo;
                                },
                                "Inv vs GL Part": function (mp) {
                                    return mp.InvGLPartDisc;
                                },
                                "Inv vs GL Material": function (mp) {
                                    return mp.InvGLMaterialDisc;
                                },
                                "GL Material Disc": function (mp) {
                                    return mp.GlMaterialDisc;
                                },
                                "Inv Material Disc": function (mp) {
                                    return mp.InvMaterialDisc;
                                },
                                "GL Part Disc": function (mp) {
                                    return mp.GlPartDisc;
                                },
                                "Inv Part Disc": function (mp) {
                                    return mp.InvPartDisc;
                                },
                        },
                        rows: ["Tanggal Inv."],
                        cols: ["Inv Group"],
                        aggregatorName: "Integer Sum",
                        vals: ["Inv Material Disc"],
                        hiddenAttributes: ["InvoiceNo", "JobOrderNo", "InvoiceDate", "InvGroup", "InvGLPartDisc", "InvGLMaterialDisc", "GlMaterialDisc", "InvMaterialDisc","InvPartDisc","GlPartDisc"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.GlInterface = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvGlInterface",
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
                            "Debit": function (mp) {
                                return mp.AmountDb;
                            },
                            "Kredit": function (mp) {
                                return mp.AmountCr;
                            },
                            "Type Trans": function (mp) {
                                return mp.TypeTrans;
                            },
                            "Account No": function (mp) {
                                return mp.AccountNo;
                            },
                            "Invoice Group": function (mp) {
                                return mp.InvGroup;
                            },
                            "No. SPK": function (mp) {
                                return mp.JobOrderNo;
                            },
                            "No. Invoice": function (mp) {
                                return mp.InvoiceNo;
                            },
                        },
                        rows: ["Tanggal Inv.", "Type Trans", "Account No"],
                        aggregatorName: "Integer Sum",
                        vals: ["Debit"],
                        hiddenAttributes: ["InvoiceDate", "AmountDb", "AmountCr", "TypeTrans", "AccountNo", "InvGroup", "JobOrderNo", "InvoiceNo"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.InvArGl = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvSpkMovement",
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
                                return moment(mp["JobOrderDate"]).format('DD-MMM-YYYY');
                            },
                            "Inv - AR": function (mp) {
                                return mp.InvAR;
                            },
                            "Inv - GL": function (mp) {
                                return mp.InvGL;
                            },
                            "Status AR": function (mp) {
                                return mp.StatusFlag;
                            },
                            "No. SPK": function (mp) {
                                return mp.JobOrderNo;
                            },
                            "Total Srv Amt": function (mp) {
                                return mp.TotalSrvAmt;
                            },
                            "No. Invoice": function (mp) {
                                return mp.InvoiceNo;
                            },
                            "Nett AR": function (mp) {
                                return mp.NettAmt;
                            },
                            "No. Dokumen": function (mp) {
                                return mp.DocNo;
                            },
                            "Total Inv Amt": function (mp) {
                                return mp.TotalInvAmt;
                            },
                            "Total GL": function (mp) {
                                return mp.AmountDb;
                            },

                        },
                        rows: ["Tanggal SPK"],
                        cols:["Status AR"],
                        aggregatorName: "Integer Sum",
                        vals: ["Inv - AR"],
                        hiddenAttributes: ["JobOrderDate", "InvAR", "InvGL", "StatusFlag", "JobOrderNo", "InvoiceNo", "TotalSrvAmt", "NettAmt", "DocNo", "TotalInvAmt","AmountDb"],
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
        me.data.JenisPivot = 'INVGL';
        me.data.StartDate = moment().format('YYYY-MM-DD');
        me.data.EndDate = moment().format('YYYY-MM-DD');
    };
    
    me.start();

}



$(document).ready(function () {
    
    var options = {
        title: "Service Journal",
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
                                    { value: 'INVGL', text: 'Part Material (INV vs GL)' },
                                    { value: 'GLIFC', text: 'GL Interface' },
                                    { value: 'INVARGL', text: 'INV vs AR vs GL' },
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
        SimDms.Angular("PivotTransJN");
    }

});