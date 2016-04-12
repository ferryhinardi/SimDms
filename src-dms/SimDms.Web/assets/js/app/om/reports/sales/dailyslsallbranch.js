"use strict";
function RptDailySalesAllBranch($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });    

    me.printPreview = function () {

        if (me.data.PeriodDateStart > me.data.PeriodDateTo) {
            MsgBox('Periode Awal tidak boleh lebih besar dari periode akhir', MSG_ERROR);
            return;
        }
        //var param = [
        //    moment(me.data.PeriodDateStart).format('DD MMM YYYY'),
        //    moment(me.data.PeriodDateTo).format('DD MMM YYYY'),

        //];
        //Wx.showPdfReport({
        //    id: "OmRpSalRgs033",
        //    pparam: param.join(','),
        //    rparam: "PER : " + moment(me.data.PeriodDateStart).format('DD MMM YYYY') + " S/D " + moment(me.data.PeriodDateTo).format('DD MMM YYYY'),
        //    type: "devex"
        //});

        var startDate = moment(me.data.PeriodDateStart).format('YYYYMMDD');
        var toDate = moment(me.data.PeriodDateTo).format('YYYYMMDD');
        var period1 = moment(me.data.PeriodDateStart).format('DD MMM YYYY');
        var period2 = moment(me.data.PeriodDateTo).format('DD MMM YYYY');
        var reportId = "OmRpSalRgs033";

        var url = "om.api/Report/GenDailySalesBranchV?";
        var params = "&StartDate=" + startDate;
        params += "&ToDate=" + toDate;
        params += "&ReportId=" + reportId;
        params += "&Period1=" + period1;
        params += "&Period2=" + period2;
        url = url + params;
        window.location = url;

    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        //me.data.PeriodDateStart = me.now();
        //me.data.PeriodDateTo = me.now();
        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.PeriodDateStart = e.DateFrom;
              me.data.PeriodDateTo = e.DateTo;
          });

        //$http.get('breeze/sales/CurrentUserInfo').
        //  success(function (dl, status, headers, config) {
        //      me.data.CompanyCode = dl.CompanyCode;
        //      me.data.BranchCode = dl.BranchCode;
        //  });

        me.isPrintAvailable = true;
    }

    me.start();

}

$(document).ready(function () {
    var options = {
        title: "Report Daily Sales All Branch",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        //{ name: "CompanyCode", model: "data.CompanyCode", text: "Kode Perusahaan", cls: "span2 full", disable: "isPrintAvailable", show: false },
                        //{ name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span3 full", disable: "isPrintAvailable", show: false },
                                                
                        {
                            text: "Periode",
                            type: "controls",
                            cls: "span4 full",
                            items: [
                                { name: "PeriodDateStart", model: "data.PeriodDateStart", placeHolder: "Tgl Periode Awal", cls: "span5", type: 'ng-datepicker' },
                            ]
                        },
                        {
                            text: "S/D",
                            type: "controls",
                            cls: "span4 full",
                            items: [
                                { name: "PeriodDateTo", model: "data.PeriodDateTo", placeHolder: "Tgl Periode Akhir", cls: "span5", type: 'ng-datepicker' },
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
        SimDms.Angular("RptDailySalesAllBranch");

    }
});