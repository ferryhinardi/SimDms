"use strict"; //Reportid OmRpMst001
function spRptMstSales($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data = {};
        me.change = false;
        $http.get('breeze/sales/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;

          });
        //me.data.DateFrom = me.now();
        //me.data.DateTo = me.now();
        if (new Date(Date.now()).getMonth() >= 0 || new Date(Date.now()).getMonth() <= 5) {
            me.data.DateFrom = 1 + '/' + 1 + '/' + (new Date().getFullYear() - 1);
            me.data.DateTo = 12 + '/' + 31 + '/' + (new Date().getFullYear() - 1);
        }
        else {
            me.data.DateFrom = 1 + '/' + 1 + '/' + (new Date().getFullYear());
            me.data.DateTo = 12 + '/' + 31 + '/' + (new Date().getFullYear());
        }

        var date = new Date();
        me.data.DateFrom = new Date(date.getFullYear(), date.getMonth(), 1);
        me.data.DateTo = new Date(date.getFullYear(), date.getMonth() + 1, 0);
        
        me.isPrintAvailable = true;
    }
    
    me.start();
    me.options = '0';

    $('#btnProcess').on('click', function (e) {
        var reportID = ""; var dateFrom = ""; var dateTo = ""; var SDate = ""; var EDate = "";
        dateFrom = moment(me.data.DateFrom).format('YYYYMMDD');
        dateTo = moment(me.data.DateTo).format('YYYYMMDD');
        SDate = moment(me.data.DateFrom).format('DD-MMM-YYYY');
        EDate = moment(me.data.DateTo).format('DD-MMM-YYYY');
        if (me.options == '0') {
            reportID = "OmRpSalRgs020";
        }
        else if (me.options == '1') {
            reportID = "OmRpSalRgs021"; 
        }
        else if (me.options == '2') {
            reportID = "OmRpSalRgs022";
        }
        else if (me.options == '3') {
            reportID = "OmRpSalRgs023";
        }
        else if (me.options == '4') {
            reportID = "OmRpSalRgs024"
        }
        else MsgBox("Under Construction", MSG_ERROR);

        var url = "om.api/Report/LaporanFaktur?";
        var params = "&CompanyCode=" + $('[name="CompanyCode"]').val();
        params += "&BranchCode=" + $('[name="BranchCode"]').val();
        params += "&DateFrom=" + dateFrom;
        params += "&DateTo=" + dateTo;
        params += "&ReportID=" + reportID;
        params += "&SDate=" + SDate;
        params += "&EDate=" + EDate;
        url = url + params;
        window.location = url;
    });

}


$(document).ready(function () {
    var options = {
        title: "Report Faktur",
        xtype: "panels",
        toolbars: [
            { name: "btnProcess", text: "Generate Excel", icon: "fa fa-file-excel-o" },
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
                        {
                            type: "optionbuttons",
                            name: "tabpageoptions",
                            model: "options",
                            cls: "span8",
                            items: [
                                { name: "0", text: "by Dealer by Wilayah(City)" },
                                { name: "1", text: "by Dealer by Kecamatan Area" },
                                { name: "2", text: "by Type by Dealer" },
                                { name: "3", text: "by Type by Wilayah(City)" },
                                { name: "4", text: "by Type by Kecamatan Area" },

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