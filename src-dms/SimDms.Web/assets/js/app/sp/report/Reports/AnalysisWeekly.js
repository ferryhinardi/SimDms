"use strict"
var IsBranch = false;

function AnalysisWeekly($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/reportanalysisweekly/Area').
    success(function (data, status, headers, config) {
        me.comboArea = data;
    });

    $http.post('sp.api/reportanalysisweekly/Company').
    success(function (data, status, headers, config) {
        me.comboDealer = data;
        if (data.length == 1) {
            var dealer = document.getElementById('CompanyCode')
            dealer.remove(0);
        }
    });

    $http.post('sp.api/Combo/LoadBranchCode').
    success(function (data, status, headers, config) {
        me.comboBranch = data;
        var part = document.getElementById('BranchCode')
        part.options[0].text = 'SELECT ALL';
    });

    $http.post('sp.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.comboYear = data;
    });

    $http.post('sp.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboPartType = data;
        var part = document.getElementById('TypeOfGoods')
        part.options[0].text = 'SELECT ALL';
    });

    me.default = function () {
        $http.post('sp.api/reportanalysisweekly/default').
        success(function (data, status, headers, config) {
            me.data = data;
            $('#PeriodYear').select2('val', data.Year)
            $('#PeriodMonth').select2('val', data.Month);
        });
    }

    me.load = function () {
        if (me.data.PeriodYear == '' || me.data.PeriodYear == null || me.data.PeriodYear == undefined) {
            MsgBox('Period Year harus diisi');
            return;
        }
        if (me.data.PeriodMonth == '' || me.data.PeriodMonth == null || me.data.PeriodMonth == undefined) {
            MsgBox('Period Month harus diisi');
            return;
        }
        if (me.data.CompanyCode == '' || me.data.CompanyCode == null || me.data.CompanyCode == undefined) {
            MsgBox('Dealer harus diisi');
            return;
        }

        $http.post('sp.api/reportanalysisweekly/loaddata', me.data)
        .success(function (e) {
            if (e.success) {
                me.loadTableData(me.sparepartWeeklyGrid, e.data);
            } else {
                MsgBox(e.message, MSG_ERROR);
            }
        })
        .error(function (e) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    me.btnExportExcel = function () {
        var type = me.data.TypeOfGoods === undefined ? '' : me.data.TypeOfGoods;

        window.location.href = 'sp.api/reportanalysisweekly/GenerateExcel?CompanyCode=' + me.data.CompanyCode + '&BranchCode=' + me.data.BranchCode
        + '&PeriodYear=' + me.data.PeriodYear + '&PeriodMonth=' + me.data.PeriodMonth + '&TypeOfGoods=' + type;
    }

    me.initialize = function () {
        me.clearTable(me.sparepartWeeklyGrid);
        me.default();
    }

    me.sparepartWeeklyGrid = new webix.ui({
        container: "wxSparepartWeeklyGrid",
        view: "wxtable", css: "alternating",
        columns: [
            { id: "PeriodWeek", header: "Minggu Ke -", fillspace: true },
            { id: "Netto_WS", format: webix.i18n.numberFormat, css: { "text-align": "right" }, header: ["Sales Out Bengkel", { text: "Netto", colspan: 1 }], fillspace: true },
            { id: "HPP_WS", format: webix.i18n.numberFormat, css: { "text-align": "right" }, header: ["", { text: "HPP", colspan: 1 }], fillspace: true },
            { id: "Netto_C", format: webix.i18n.numberFormat, css: { "text-align": "right" }, header: ["Sales Out Counter", { text: "Netto", colspan: 1 }], fillspace: true },
            { id: "HPP_C", format: webix.i18n.numberFormat, css: { "text-align": "right" }, header: ["", { text: "HPP", colspan: 1 }], fillspace: true },
            { id: "Netto_PS",  format: webix.i18n.numberFormat, css: { "text-align": "right" },header: ["Sales Out Partshop",{ text:"Netto",colspan:1}], fillspace: true },
            { id: "HPP_PS",  format: webix.i18n.numberFormat, css: { "text-align": "right" },header: ["", { text: "HPP", colspan: 1 }], fillspace: true },
            { id: "NilaiStock", format: webix.i18n.numberFormat, css: { "text-align": "right" }, header: "Stock", fillspace: true },
        ],
    });

    webix.event(window, "resize", function () {
        me.sparepartWeeklyGrid.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Laporan Sparepart Analisis Mingguan",
        xtype: "panels",
        toolbars: [
            { name: "btnGenerate", text: "Load", cls: "btn btn-primary", icon: "icon-spinner", click: "load()" },
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "btn btn-primary", click: "btnExportExcel()" },
        ],
        panels: [
            {
                name: "pnlAnalysisWeekly",
                items: [
                  {
                      text: "Period",
                      type: "controls",
                      items: [
                          { name: "PeriodYear", cls: "span2", text: "Year", type: "select2", datasource: "comboYear" },
                          { name: "PeriodMonth", cls: "span3", text: "Bulan", type: "select2", datasource: "comboMonth" },
                      ]
                  },
                  //{ name: "Area", text: "Area", cls: "span4 full", type: "select2", datasource: "comboArea" },
                  { name: "CompanyCode", text: "Dealer", cls: "span4 full", type: "select2", datasource: "comboDealer" },
                  { name: "BranchCode", text: "Branch", cls: "span6", type: "select2", datasource: "comboBranch", disable: 'data.IsBranch' },
                  { name: "TypeOfGoods", cls: "span4", text: "Tipe", type: "select2", datasource: "comboPartType" },
                  {
                      name: "wxSparepartWeeklyGrid",
                      type: "wxdiv",
                  },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("AnalysisWeekly");
    }
});