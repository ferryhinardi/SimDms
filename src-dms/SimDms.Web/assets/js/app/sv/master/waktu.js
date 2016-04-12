"use strict"

function svMstWorkingTimeController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.saveData = function () {
        if (me.data.DayCode == "" || me.data.DayCode == null    ) return;
        $http.post("sv.api/waktukerja/save", me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.initialize();
                }
            });
    }   

    me.cancelDtl = function () {
        me.initialize();
    }

    me.printPreview = function () {
        var ReportId = "SvRpMst004";

        Wx.showPdfReport({
            id: ReportId,
            pparam: 'default',
            rparam: me.data.UserId,
            type: "devex"
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.Decription = "";
        me.data.DayCode = "";
        me.data.BeginWorkTime = "00:00";
        me.data.EndWorkTime = "00:00";
        me.data.BeginLunchTime = "00:00";
        me.data.EndLunchTime = "00:00";
        me.data.IsActive = true;
       
        $("#btnSave, #btnCancelDtl").hide();

        $http.post("sv.api/waktukerja/workingtime").
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.loadTableData(me.grid1, data.data);
                }
            });
    }

    me.grid1 = new webix.ui({
        container: "wxWorkingTime",
        view: "wxtable", css:"alternating",
        columns: [
                    { id: "DayCode", header: "" },
                    { id: "Description", header: "Hari", fillspace: true },
                    { id: "BeginWorkTime", header: "Jam Mulai Kerja", format: me.timeFormat, fillspace: true },
                    { id: "EndWorkTime", header: "Jam Akhir Kerja", fillspace: true, format: me.timeFormat },
                    { id: "BeginLunchTime", header: "Jam Mulai Istirahat", fillspace: true, format: me.timeFormat },
                    { id: "EndLunchTime", header: "Jam Akhir Istirahat", fillspace: true, format: me.timeFormat },
                    { id: "Status", header: "Status", fillspace: true }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    $("#btnSave, #btnCancelDtl").show();
                   
                    var date = new Date(parseInt(this.getItem(me.grid1.getSelectedId()).BeginWorkTime.substring(6, 20))).toString();
                    var date1 = new Date(parseInt(this.getItem(me.grid1.getSelectedId()).EndWorkTime.substring(6, 20))).toString();
                    var date2 = new Date(parseInt(this.getItem(me.grid1.getSelectedId()).BeginLunchTime.substring(6, 20))).toString();
                    var date3 = new Date(parseInt(this.getItem(me.grid1.getSelectedId()).EndLunchTime.substring(6, 20))).toString();

                    var timeFormat = date.substring(16,21);
                    var timeFormat1 = date1.substring(16, 21);
                    var timeFormat2 = date2.substring(16, 21);
                    var timeFormat3 = date3.substring(16, 21);

                    me.data.DayCode = this.getItem(me.grid1.getSelectedId()).DayCode;
                    me.data.Description = this.getItem(me.grid1.getSelectedId()).Description;
                    me.data.BeginWorkTime = timeFormat;
                    me.data.EndWorkTime = timeFormat1;
                    me.data.BeginLunchTime = timeFormat2;
                    me.data.EndLunchTime = timeFormat3;

                    $('#DayCode').val(this.getItem(me.grid1.getSelectedId()).DayCode);
                    $('#Description').val(this.getItem(me.grid1.getSelectedId()).Description);
                    $('#BeginWorkTime').val(timeFormat);
                    $('#EndWorkTime').val(timeFormat1); 
                    $('#BeginLunchTime').val(timeFormat2);
                    $('#EndLunchTime').val(timeFormat3);
                    if (this.getItem(me.grid1.getSelectedId()).IsActive) {
                        $('#IsActive').attr('checked', true);
                        me.data.IsActive = true;
                    }
                    else {
                        $('#IsActive').removeAttr('checked');
                        me.data.IsActive = false;
                    }
                    me.Apply();
                }
            }
        }
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Waktu Kerja",
        xtype: "panels",
        toolbars:
            [{ name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" }],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    { name: "DayCode", cls: "hide" },
                    { text:"Hari", name: "Description", cls: "span2", readonly: true },
                    { text: "Jam Mulai Kerja", name: "BeginWorkTime", maxlength: 5, cls: "span2", type: "timepicker" },
                    { text: "Jam Akhir Kerja", name: "EndWorkTime", cls: "span2", maxlength: 5, type: "timepicker" },
                    { text: "Jam Mulai Istirahat", name: "BeginLunchTime", cls: "span2", maxlength: 5, type: "timepicker" },
                    { text: "Jam Akhir Istirahat", name: "EndLunchTime", cls: "span2", maxlength: 5, type: "timepicker" },
                    { text: "Status", name: "IsActive", cls: "span2", type: "x-switch" },
                    {
                        type: "buttons", items: [
                        { name: "btnSave", text: "Save", icon: "icon-save", cls: "btn btn-success", click: "saveData()" },
                        { name: "btnCancelDtl", text: "Cancel", cls: "btn btn-warning", icon: "icon-undo", click: "cancelDtl()" },
                        { name: "btnRefresh", text: "Refresh", icon: "icon-refresh", cls: "btn btn-success", click: "initialize()" }
                        ]
                    },
                ]
            },
            {
                name: "wxWorkingTime",
                xtype: "wxtable",
                tblname: "tblWorkingTime"
            }
        ],
    }
    
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstWorkingTimeController");
    }

});