"use strict";
var baseOnModel = "0";
function RptDailySalesAllBranch($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.$watch('optionByModel', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
            baseOnModel = newValue;

            console.log(baseOnModel);
        }
    });

    $http.post('gn.api/combo/Organizations').
    success(function (data, status, headers, config) {
        me.Company = data;
    });

    me.printPreview = function () {
        var tipe = "";        

        if (me.data.PeriodDateStart > me.data.PeriodDateTo) {
            MsgBox('Periode Awal tidak boleh lebih besar dari periode akhir', MSG_ERROR);
            return;
        }
        if (baseOnModel == "0") {
            tipe = "A";
        }
        else {
            tipe = "B";
        }

        var param = "";
        var reportId = "";

        var compCode = me.data.CompanyCode;
        var brncCode = me.data.BranchCode;
        var startDate = moment(me.data.PeriodDateStart).format('YYYYMMDD');
        var toDate = moment(me.data.PeriodDateTo).format('YYYYMMDD');
        var iD = 1;
        var period1 = moment(me.data.PeriodDateStart).format('DD MMM YYYY');
        var period2 = moment(me.data.PeriodDateTo).format('DD MMM YYYY');

        
        if ($('#chkCompany').prop('checked') == true)
        {
            reportId = 'OmRpSalRgs033B';
            //param = [
            //    'companycode',
            //    me.data.BranchCode,
            //    moment(me.data.PeriodDateStart).format('DD MMM YYYY'),
            //    moment(me.data.PeriodDateTo).format('DD MMM YYYY'),
            //    1,
            //    tipe
            //];

            var url = "om.api/Report/GenDailySalesBranchV2?";
            var params = "&CompanyCode=" + compCode;
            params += "&BranchCode=" + brncCode;
            params += "&StartDate=" + startDate;
            params += "&ToDate=" + toDate;
            params += "&ID=" + iD;
            params += "&Tipe=" + tipe;
            params += "&ReportId=" + reportId;
            params += "&Period1=" + period1;
            params += "&Period2=" + period2;
            url = url + params;
            window.location = url;


        }
        else{
            reportId = 'OmRpSalRgs033B';
            //param = [
            //    'companycode',
            //    me.data.BranchCode,
            //    moment(me.data.PeriodDateStart).format('DD MMM YYYY'),
            //    moment(me.data.PeriodDateTo).format('DD MMM YYYY'),
            //    1,
            //    tipe
            //];

            var url = "om.api/Report/GenDailySalesBranchV2?";
            var params = "&CompanyCode=" + compCode;
            params += "&BranchCode=" + brncCode;
            params += "&StartDate=" + startDate;
            params += "&ToDate=" + toDate;
            params += "&ID=" + iD;
            params += "&Tipe=" + tipe;
            params += "&ReportId=" + reportId;
            params += "&Period1=" + period1;
            params += "&Period2=" + period2;
            url = url + params;
            window.location = url;
        }          
        

        //Wx.showPdfReport({
        //    id: reportId,
        //    pparam: param.join(','),
        //    rparam: "PER : " + moment(me.data.PeriodDateStart).format('DD MMM YYYY') + " S/D " + moment(me.data.PeriodDateTo).format('DD MMM YYYY'),
        //    type: "devex"
        //});
    }

    me.initialize = function () {
        me.data = {};
        me.change = false;
        //var d = new Date(Date.now()).getDate();
        //var m = new Date(Date.now()).getMonth();
        //var y = new Date(Date.now()).getFullYear();
        //me.data.PeriodDateStart = new Date(y, m, 1);
        //me.data.PeriodDateTo = new Date(y, m, d);

        $http.post('om.api/ReportSales/Periode').
          success(function (e) {
              me.data.PeriodDateStart = e.DateFrom;
              me.data.PeriodDateTo = e.DateTo;
          });

        $('#CompanyCode').attr('disabled', true);
        $('#BranchCode').attr('disabled', true);

        $http.get('breeze/sales/CurrentUserInfo').
              success(function (dl, status, headers, config) {
                  me.data.CompanyCode = dl.CompanyCode;
                  me.data.BranchCode = dl.BranchCode;
              });

        me.isPrintAvailable = true;
    }

    $('#chkCompany').on('change', function (e) {
        if ($('#chkCompany').prop('checked') == true) {
            $('#CompanyCode').removeAttr('disabled');
            $http.get('breeze/sales/CurrentUserInfo').
              success(function (dl, status, headers, config) {
                  me.data.CompanyCode = dl.CompanyCode;
                  me.data.BranchCode = dl.BranchCode;
              });
        } else {
            $('#CompanyCode').attr('disabled', true);
            //me.data.CompanyCode = undefined;
            //me.data.BranchCode = undefined;
            me.data.CompanyCode = me.data.CompanyCode;
            me.data.BranchCode = me.data.BranchCode;
        }
        me.Apply();
    })

    me.optionByModel = "0";

    me.start();
    me.options = '0';
}

$(document).ready(function () {
    var options = {
        title: "Report Daily Sales All Branch V.2",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
            { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlA",
                items: [
                        { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span3 full", disable: "isPrintAvailable", show: false },

                        { name: "PeriodDateStart", text: "Periode", model: "data.PeriodDateStart", placeHolder: "Tgl Periode Awal", cls: "span3", type: 'ng-datepicker' },
                        { name: "PeriodDateTo", text: "s/d", model: "data.PeriodDateTo", placeHolder: "Tgl Periode Akhir", cls: "span3", type: 'ng-datepicker' },

                        {
                            text: "Berdasarkan",
                            type: "controls",
                            items: [
                                {
                                    type: "optionbuttons",
                                    name: "tabpageoptions",
                                    model: "optionByModel",
                                    items: [
                                        { name: "0", text: "Model Desc" },
                                        { name: "1", text: "Model Code" },
                                    ]
                                },
                            ]
                        },

                        {
                            text: "Company ",
                            type: "controls",
                            items: [

                                { name: "chkCompany", model: "data.chkCompany", text: "Company", cls: "span1", type: "ng-check" },
                                { name: "CompanyCode", opt_text: "", cls: "span5", type: "select2", text: "Company", datasource: "Company", disable: "!data.chkCompany" },
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