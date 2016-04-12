﻿"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.printPreview = function () {
        var reportID = ""; 
        var sDateFrom = ''; var sDateTo = "";
        var DateFrom = ''; var DateTo = ''; 
        sDateFrom = moment(me.data.DateFrom).format('YYMMDD');
        sDateTo = moment(me.data.DateTo).format('YYMMDD');
        DateFrom = moment(me.data.DateFrom).format('YYYYMMDD');
        DateTo = moment(me.data.DateTo).format('YYYYMMDD');

        var url = "om.api/Report/vehicleownershipinfo?";
        var params = "&FromDate=" + DateFrom;
        params += "&EndDate=" + DateTo;
        params += "&sFromDate=" + sDateFrom;
        params += "&sEndDate=" + sDateTo;
        url = url + params;
        window.location = url;
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });

        var ym = me.now("YYYY-MM") + "-01";
        me.data.DateFrom = moment(ym);
        me.data.DateTo = moment(ym).add("months", 1).add("days", -1);
        //me.data.DateFrom = me.now();
        //me.data.DateTo = me.now();
        me.isPrintAvailable = true;
    }
    
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Sales Report with Vehicle Ownership Info",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Generate Exel", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            //{ name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span4 full", disable: "isPrintAvailable" },
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span4 full", disable: "isPrintAvailable" },
                        {
                            text: "Periode :",
                            type: "controls",
                            items: [
                        { name: "DateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                        { name: "DateTo", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },


                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);



    function init(s) {
        $(".switchlabel").attr("style", "padding:9px 0px 0px 5px")
        SimDms.Angular("spRptMstSales");

    }
});