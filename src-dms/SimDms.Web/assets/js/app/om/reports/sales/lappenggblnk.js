"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.printPreview = function () {
        var prm = ""; var sparam = "";
        var dateFrom = ""; var dateTo = "";
            dateFrom = moment(me.data.DateFrom).format('YYYYMMDD');
            dateTo = moment(me.data.DateTo).format('YYYYMMDD');
            sparam = "PERIODE : " + moment(me.data.DateFrom).format("DD-MMM-YYYY") + " s/d " + moment(me.data.DateTo).format("DD-MMM-YYYY");
            console.log(sparam);
        prm = [
                  dateFrom,
                  dateTo
                ];
        Wx.showPdfReport({
            id: "OmRpSalRgs025",
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
        me.data.DateFrom = me.now();
        me.data.DateTo = me.now();
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