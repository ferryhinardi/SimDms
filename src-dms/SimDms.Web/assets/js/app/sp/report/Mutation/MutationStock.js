"use strict";

function spRptMutationStock($scope, $http, $injector, $timeout, blockUI) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadBranchCode').
        success(function (data, status, headers, config) {
            me.comboBranch = data;
        });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
        success(function (data, status, headers, config) {
            me.comboTPGO = data;
        });

    $http.post('sp.api/Combo/Months?').
        success(function (data, status, headers, config) {
            me.MonthsDS = data;
        });

    $http.post('sp.api/Combo/Years?').
        success(function (data, status, headers, config) {
            me.YearsDS = data;
        });

    me.initialize = function () {
        me.isPrintAvailable = true;

        me.data.BranchCode = "";
        me.data.TypeOfGoods = "";
        me.data.FromMonth = new Date().getMonth() + 1;
        me.data.FromYear = new Date().getFullYear();
        me.data.ToMonth = new Date().getMonth() + 1;
        me.data.ToYear = new Date().getFullYear();
    };

    me.printPreview = function () {
        if (me.data.FromMonth == "" || me.data.FromYear == "" || me.data.ToMonth == "" || me.data.ToYear == "")
        {
            sdms.Warning("Please Complate Filter Form!");
            return;
        }
        var data = {
            BranchCodeParam: me.data.BranchCode,
            TypeOfGoodsParam: me.data.TypeOfGoods,
            FromMonthParam: me.data.FromMonth,
            FromYearParam: me.data.FromYear,
            ToMonthParam: me.data.ToMonth,
            ToYearParam: me.data.ToYear,
        };

        blockUI.start("Proccessing...");

        $http.post('sp.api/ReportMutationStock/Printxls', data).
          success(function (response, status, headers, config) {
              blockUI.stop();
              if (response.message == "") {
                  location.href = 'sp.api/ReportMutationStock/DownloadExcelFile?key=' + response.value + '&filename=Mutation Report';
              } else {
                  sdms.info(response.message, "Error");
              }
          }).
          error(function (response, status, headers, config) {
              blockUI.stop();
          });
    };

    me.start();
};

$(document).ready(function () {
    var options = {
        title: "Mutation Stock",
        xtype: "panels",
        toolbars: [
            { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "pnlMutationStock",
                items: [
                    { name: "BranchCode", model: "data.BranchCode", text: "Kode Cabang", cls: "span6 full", opt_text: "-- SELECT ALL --", type: "select2", datasource: "comboBranch" },
                    { name: "TypeOfGoods", model: "data.TypeOfGoods", text: "Tipe Part", cls: "span6 full", opt_text: "-- SELECT ALL --", type: "select2", datasource: "comboTPGO" },
                    {
                        type: "controls",
                        cls: "span6 full",
                        text: "From",
                        items: [
                            { name: 'FromMonth', model: "data.FromMonth", text: "Periode", type: "select2", cls: "span5", opt_text: "-- SELECT MONTH --", datasource: "MonthsDS" },
                            { name: 'FromYear', model: "data.FromYear", text: "Year", type: "select2", cls: "span3", opt_text: "-- SELECT YEAR --", datasource: "YearsDS" },
                        ]
                    },
                    {
                        type: "controls",
                        cls: "span6 full",
                        text: "To",
                        items: [
                            { name: 'ToMonth', model: "data.ToMonth", text: "Periode", type: "select2", cls: "span5", opt_text: "-- SELECT MONTH --", datasource: "MonthsDS" },
                            { name: 'ToYear', model: "data.ToYear", text: "Year", type: "select2", cls: "span3", opt_text: "-- SELECT YEAR --", datasource: "YearsDS" },
                        ]
                    },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("spRptMutationStock");
    }
});