"use strict";

function PivotTransMSI($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    
    me.createPivot = function()
    {
        var pivot = me.data.JenisPivot;
        if (pivot == "LSR") {
            me.LabourSlsRevenue();
        }
        if (pivot == "SSR") {
            me.SpSlsRevenue();
        }
        if (pivot == "UINTAKE") {
            me.UnitInTake();
        }
        if (pivot == "JTYPE") {
            me.JobType();
        }
        if (pivot == "FSERVICE") {
            me.FreeService();
        }
    }

    me.LabourSlsRevenue = function ()
    {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvMsiP01",
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
                            "SubCon": function (mp) {
                                return mp.IsSubCon;
                            },
                            "Group": function (mp) {
                                return mp.GroupJobType;
                            },
                            "Job Type": function (mp) {
                                return mp.JobType;
                            },
                            "No. Invoice": function (mp) {
                                return mp.InvoiceNo;
                            },
                            "Labor Amt": function (mp) {
                                return mp.TaskAmt;
                            },
                            "Hour Sold": function (mp) {
                                return mp.OperationHour;
                            },
                        },
                        rows: ["SubCon", "Group"],
                        aggregatorName: "Integer Sum",
                        vals: ["Labor Amt"],
                        rendererName: "Table",
                        hiddenAttributes: ["InvoiceDate", "IsSubCon", "GroupJobType", "InvoiceNo", "JobType", "TaskAmt", "OperationHour"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
                console.log(response);
            }
        });
    }

    me.SpSlsRevenue = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvMsiP05",
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
                            "Job Type": function (mp) {
                                return mp.JobType;
                            },
                            "Group Sublet": function (mp) {
                                return mp.GroupPart;
                            },
                            "Group TPGO": function (mp) {
                                return mp.TypeOfGoodDesc;
                            },
                            "Group": function (mp) {
                                return mp.GroupJobType;
                            },
                            "No. Invoice": function (mp) {
                                return mp.InvoiceNo;
                            },
                            "Sparepart Amt": function (mp) {
                                return mp.PartAmount;
                            },
                            "TypeOfGood Name": function (mp) {
                                return mp.TypeOfGoodName;
                            },

                        },
                        rows: ["Group Sublet", "Group TPGO", "TypeOfGood Name"],
                        cols: ["Group"],
                        aggregatorName: "Integer Sum",
                        vals: ["Sparepart Amt"],
                        rendererName: "Table",
                        hiddenAttributes: ["InvoiceDate", "JobType", "GroupPart", "TypeOfGoodDesc", "GroupJobType", "InvoiceNo", "PartAmount", "TypeOfGoodName"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.UnitInTake = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvMsiP29",
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
                            "Jml Inv": function (jmlI) {
                                return jmlI.JmlInvoice;
                            },
                            "Basic Model": function (mp) {
                                return mp.BasicModel;
                            },
                            "Company Code": function (mp) {
                                return mp.CompanyCode;
                            },
                            "Chassis Code": function (mp) {
                                return mp.ChassisCode;
                            },
                            "Group Type": function (mp) {
                                return mp.GroupType;
                            },
                        },
                        rows: ["Company Code","Group Type","Basic Model" ],
                        aggregatorName: "Count",
                        hiddenAttributes: ["JobOrderDate", "JmlInvoice", "BasicModel", "CompanyCode", "ChassisCode", "GroupType"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
            }
        });
    }

    me.JobType = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvMsiP33",
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
                            "Chassis Code": function (mp) {
                                return mp.ChassisCode;
                            },
                            "Chassis No": function (mp) {
                                return mp.ChassisNo;
                            },
                            "VIN No": function (mp) {
                                return mp.VinNo;
                            },
                            "Job Type": function (mp) {
                                return mp.JobType;
                            },
                            "Group Job Type": function (mp) {
                                return mp.GroupJobType;
                            },
                            "Warranty Odometer": function (mp) {
                                return mp.WarrantyOdometer;
                            },
                        },
                        rows: ["Group Job Type","Job Type"],
                        aggregatorName: "Count",
                        hiddenAttributes: ["JobOrderDate", "ChassisCode", "ChassisNo", "VinNo", "JobType", "GroupJobType", "WarrantyOdometer"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }

                console.log(response);
            }
        });
    }

    me.FreeService = function () {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvMsiP44",
        }

        $.ajax({
            type: "POST",
            url: 'sv.api/InquiryPivot/ServiceTransaction',
            dataType: 'json',
            data: params,
            success: function (response) {
                if (response.data.length !== 0 ) {
                    window.pivotdata = response.data;
                    $("#wxpivotgrid").pivotUI(window.pivotdata, {
                        derivedAttributes: {
                            "Tanggal Inv.": function (mp) {
                                return moment(mp["InvoiceDate"]).format('DD-MMM-YYYY');
                            },
                            "Fsc Group": function (mp) {
                                return mp.FscGroup;
                            },
                            "Chassis Code": function (mp) {
                                return mp.ChassisCode;
                            },
                            "Chassis No": function (mp) {
                                return mp.ChassisNo;
                            },
                            "Group Job Type": function (mp) {
                                return mp.GroupJobType;
                            },
                        },
                        rows: ["Group Job Type","Chassis Code"],
                        cols: ["Fsc Group"],
                        aggregatorName: "Count",
                        hiddenAttributes: ["InvoiceDate", "FscGroup", "ChassisCode", "ChassisNo", "GroupJobType"],
                    });
                }
                else {
                    MsgBox("Tidak ada data yang ditampilkan");
                }
                console.log(response);
            }
        });
    }

    $('#JenisPivot').select().on("change", function () {
        var tbl = document.getElementById("wxpivotgrid");
        var chld = tbl.firstElementChild;
        var prnt = tbl.parentElement;
        
        if (chld != null) {
            $("#wxpivotgrid").pivotUI([], {});
            //tbl.firstElementChild.remove();
            //tbl.removeChild(chld);
            $("#wxpivotgrid").html(null);
        }
        console.log(tbl,chld, prnt);
    });
    
    me.initialize = function () {
        me.data = {};
        me.data.JenisPivot = 'LSR';
        me.data.StartDate = moment().format('YYYY-MM-DD');
        me.data.EndDate = moment().format('YYYY-MM-DD');
    };
    
    me.start();

}



$(document).ready(function () {
    
    var options = {
        title: "Pivot Suzuki MSI",
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
                                    { value: 'LSR', text: 'Labor Sales Revenue' },
                                    { value: 'SSR', text: 'Sparepart Sales Revenue' },
                                    { value: 'UINTAKE', text: 'Unit Intake' },
                                    { value: "JTYPE", text: 'Job Types' },
                                    { value: "FSERVICE", text: 'Free Service' },
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
        SimDms.Angular("PivotTransMSI");
    }

});