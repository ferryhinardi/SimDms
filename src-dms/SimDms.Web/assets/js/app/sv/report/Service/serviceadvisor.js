"use strict"

var IsSA = false;

function svRptSrvServiceAdvisorController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/Combo/Years').
     success(function (data, status, headers, config) {
         me.comboYear = data;
     });

    $http.post('sv.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    $http.post('sv.api/Combo/ServiceAdvisor').
   success(function (data, status, headers, config) {
       me.comboServiceAdvisor = data;
       var sa = document.getElementById('ServiceAdvisor')
       sa.options[0].remove();
   });

    me.default = function () {
        $http.post('sv.api/report/default').
        success(function (data, status, headers, config) {
            $('#Month').select2('val', data.Month);
            $('#Year').select2('val', data.Year);
        });
    }

    me.$watch('options', function (newValue, oldValue) {
        me.init();
        if (newValue !== oldValue) {
            me.$broadcast(newValue);
        }
    });

    $("#SASwitchN").on('change', function (e) {
        IsSA = false;
        console.log($("#ServiceAdvisor").select2('val'));
        $("#ServiceAdvisor").prop("disabled", true);
    });
    $("#SASwitchY").on('change', function (e) {
        IsSA = true;
        $("#ServiceAdvisor").prop("disabled", false);
    });

    me.printPreview = function () {
        var data = $(".main form").serializeObject();
        Wx.XlsxReport({
            url: 'sv.api/report/reviewserviceadvisor',
            type: 'xlsx',
            params: {
                month: data.Month,
                year: data.Year,
                options: me.options,
                employeeId: IsSA ? data.ServiceAdvisor : ""
            }
        });
    }

    me.initialize = function () {
        me.isPrintAvailable = true;
        $("#ServiceAdvisor").prop("disabled", true);
        me.default();
    }

    me.options = "0";

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Review Service Advisor",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" },
        ],
        panels: [
             {
                 name: "pnlPerformaService",
                 items: [
                   { name: "Month", cls: "span4", text: "Bulan", type: "select2", datasource: "comboMonth" },
                   { name: "Year", required: true, cls: "span4", text: "-", type: "select2", datasource: "comboYear" },
                   { name: "SASwitch", text: "ServiceAdvisor", cls: "span2 full", type: "switch" },
                   { name: "ServiceAdvisor", cls: "span4", type: "select2", datasource: "comboServiceAdvisor" },
                   {
                       type: "optionbuttons",
                       name: "tabpageoptions",
                       model: "options",
                       items: [
                           { name: "0", text: "Daily" },
                           { name: "1", text: "Weekly" },
                           { name: "2", text: "Monthly" },
                           { name: "3", text: "Range" },
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
        SimDms.Angular("svRptSrvServiceAdvisorController");
    }
});