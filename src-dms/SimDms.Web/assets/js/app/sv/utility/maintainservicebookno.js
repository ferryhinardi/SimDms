"use strict"

function maintainservicebookno($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.comboTransaction = [
        { value: 0, text: "FSC" },
        { value: 1, text: "SPK" },
        { value: 2, text: "CLAIM" }
    ]

    me.ChangeTransaction = function () {
        me.data.Transaction = $('#Transaction').select2('val');
        $http.post('sv.api/MaintainServiceBookNo/GetAllServiceBookNo', { flag: me.data.Transaction }).
        success(function (data, status, headers, config) {
            me.loadTableData(me.grid1, data);
            me.detail = data;
        }).error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });

    }

    me.grid1 = new webix.ui({
        container: "wxservicebooks",
        view: "wxtable", css:"alternating",
        autoHeight: false,
        height: 400,
        autowidth: false,
        width: 1000,
        scrollX: true,
        scrollY: true,
        checkboxRefresh: true,
        columns: [
            { id: "SeqNo", header: { content: "masterCheckbox", contentId: "chkSelect" }, template: "{common.checkbox()}", width: 40 },
            { id: "BranchCode", header: "Branch Code", width: 110 },
            { id: "ChassisCode", header: "Chassis Code", width: 110 },
            { id: "ChassisNo", header: "Chassis No", width: 90 },
            { id: "ServiceBookNoOld", header: "Service Book No (Old)", width: 170 },
            { id: "ServiceBookNoNew", header: "Service Book No (New)", width: 170 },
            { id: "EngineCodeOld", header: "Engine Code(Old)", width: 140},
            { id: "EngineCodeNew", header: "Engine Code(New)", width: 140},
            { id: "EngineNoOld", header: "Engine No (Old)", width: 130},
            { id: "EngineNoNew", header: "Engine No (New)", width: 130},
        ]
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.Save = function () {
        var data = [];
        $.each(me.detail, function (key, val) {
            if (val["SeqNo"] == 1) {
                var arr = {
                    ChassisCode: val["ChassisCode"],
                    ChassisNo: val["ChassisNo"],
                    NewServiceBookNo: val["ServiceBookNoNew"],
                    Flag: me.Transaction
                }
                data.push(arr);
            }
        });
        if (data.length == 0) {
            MsgBox("Data belum dipilih");
            return;
        }
        $http.post('sv.api/MaintainServiceBookNo/SetServiceBookNo', data).
        success(function (data, status, headers, config) {
            MsgBox(data.message == "" ? "Data berhasil di update!" : data.message);
            me.ChangeTransaction();
        }).error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.initialize = function () {
        me.detail = {};
        me.data.Transaction = "";
        me.clearTable(me.grid1);
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Maintain Service Book No",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" },
            { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", click: "Save()" }
        ],
        panels: [
             {
                 title: "Filter",
                 items: [
                     { name: "Transaction", text: "Transaction", cls: "span4", type: "select2", datasource: "comboTransaction", change: "ChangeTransaction()" },
                     {
                         name: "wxservicebooks",
                         title: "Data",
                         type: "wxdiv"
                     }
                 ]
             },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("maintainservicebookno");
    }
});