"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });
    me.Report = [
        { "value": '0', "text": 'STANDART' },
        { "value": '1', "text": 'SUMMARY' },

    ];
    me.printPreview = function () {
        var prm = ""; var sparam = "";
        var dateFrom = ""; var dateTo = ""; var reportID ="";
            dateFrom = moment(me.data.DateFrom).format('YYYYMMDD');
            dateTo = moment(me.data.DateTo).format('YYYYMMDD');
            sparam = "PERIODE : " + moment(me.data.DateFrom).format("DD-MMM-YYYY") + " s/d " + moment(me.data.DateTo).format("DD-MMM-YYYY");
            if (me.data.Report == '0') {
                reportID = "OmRpSalRgs019";
                prm = [
                    dateFrom,
                    dateTo
                ];
            } else {
                reportID = "OmRpSalRgs018";
                prm = [
                     dateFrom,
                     dateTo,
                     '',
                     ''
                ];
            }
       
        Wx.showPdfReport({
            id: reportID,
            pparam: prm.join(','),
            textprint: true,
            rparam: sparam,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        //var x = '01/01/' + new Date().getFullYear();
        //alert(x);
        //me.data.DateFrom = x;
        //me.data.DateFrom = new Date().getMonth() + 1 + '/1' + '/' + (new Date().getFullYear()-1);
        //me.data.DateTo = me.now();
        //if (new Date(Date.now()).getMonth() >= 0 || new Date(Date.now()).getMonth() <= 5) {
        //    me.data.DateFrom = 7 + '/' + 1 + '/' + (new Date().getFullYear() - 1);
        //    me.data.DateTo = 12 + '/' + 31 + '/' + (new Date().getFullYear() - 1);
        //}
        //else {
        //    me.data.DateFrom = 1 + '/' + 1 + '/' + (new Date().getFullYear());
        //    me.data.DateTo = 6 + '/' + 30 + '/' + (new Date().getFullYear());
        //}
        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.DateFrom = e.DateFrom;
              me.data.DateTo = e.DateTo;
          });
        me.data.Report = '0';
        me.isPrintAvailable = true;
    }
    
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Report Penggunaan Blanko",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
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
                            cls: "span6",
                            items: [
                                    { name: "DateFrom", text: "", cls: "span3", type: "ng-datepicker" },
                                    { name: "DateTo", text: "", cls: "span3", type: "ng-datepicker" },
                            ]
                        },
                        { name: "Report", opt_text: "", cls: "span4 full", type: "select2", text: "Report", datasource: "Report" },

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