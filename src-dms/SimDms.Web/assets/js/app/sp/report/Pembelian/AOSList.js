"use strict"
function AOSList($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/ComboBranchCode').
    success(function (data, status, headers, config) {
        me.comboBranch = data;
        var part = document.getElementById('BranchCode')
        part.options[0].text = 'SELECT ALL';
    });

    me.default = function () {
        $http.post('sp.api/aoslist/default').
        success(function (data, status, headers, config) {
            me.data = data;
        });
    }
    
    me.btnExportExcel = function () {
        if (me.data.StartDate == undefined || me.data.StartDate == null || me.data.EndDate == undefined || me.data.EndDate == null) {
            MsgBox("Periode Harus Dipilih terlebih dahulu", MSG_ERROR);
            return;
        }
        $http.post('sp.api/aoslist/validate', me.data )
       .success(function (e) {
           if (e.success) {
               window.location.href = 'sp.api/aoslist/GenerateExcel?BranchCode=' + me.data.BranchCode
               + '&StartDate=' + me.data.StartDate + '&EndDate=' + me.data.EndDate;
           } else {
               MsgBox(e.message, MSG_ERROR);
           }
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }

    me.initialize = function () {
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Report AOS List",
        xtype: "panels",
        toolbars: [
            { name: "btnExportExcel", text: "Generate Excel", icon: "fa fa-file-excel-o", cls: "btn btn-primary", click: "btnExportExcel()" },
        ],
        panels: [
            {
                name: "pnlAOSList",
                items: [
                    { name: "BranchCode", text: "Cabang", cls: "span5", type: "select2", datasource: "comboBranch", disable: 'data.IsBranch' },
                    {
                        text: "Periode",
                        type: "controls",
                        items: [
                            { name: "StartDate", cls: "span2", type: "datepicker" },
                            { text: "to", cls: "span1", type: "label" },
                            { name: "EndDate", cls: "span2", type: "datepicker" }
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
        SimDms.Angular("AOSList");
    }
});