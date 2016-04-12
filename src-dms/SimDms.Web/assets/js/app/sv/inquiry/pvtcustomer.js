"use strict";

function PivotTransCustomer($scope, $http, $injector) {
    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    
    me.createPivot = function()
    {
        me.RegisterService();
    }

    me.RegisterService = function()
    {
        var params = {
            StartDate: moment(me.data.StartDate).format('YYYYMMDD'),
            EndDate: moment(me.data.EndDate).format('YYYYMMDD'),
            PivotId: "usppvt_SvCustVProb",
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
                            "Company Code": function (mp) {
                                return mp.CompanyCode;
                            },
                            "Chassis Code": function (mp) {
                                return mp.ChassisCode;
                            },
                            "Chassis No": function (mp) {
                                return mp.ChassisNo;
                            },
                            "Field": function (mp) {
                                return mp.FieldProblem;
                            },
                            "Field Value": function (mp) {
                                return mp.FieldValue;
                            },
                            "Description": function (mp) {
                                return mp.ProblemDesc;
                            },
                        },
                        rows: ["Field", "Field Value"],
                        cols: ["Company Code"],
                        aggregatorName: "Count",
                        vals: ["Chassis"],
                        hiddenAttributes: ["CompanyCode", "ChassisCode", "ChassisNo", "FieldProblem", "FieldValue","ProblemDesc"],
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
        me.data.JenisPivot = 'CVP';
        me.data.StartDate = moment().format('YYYY-MM-DD');
        me.data.EndDate = moment().format('YYYY-MM-DD');
    };
    
    me.start();

}



$(document).ready(function () {
    
    var options = {
        title: "Customer Vehicle",
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
                                    { value: 'CVP', text: 'Customer Vehicle Problem' },
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
        SimDms.Angular("PivotTransCustomer");
    }

});