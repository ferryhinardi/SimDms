"use strict"

function svMstAvailableMechanicController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.delete = function () {
        MsgConfirm("Apakah anda yakin???", function (result) {
            if (result) {
                $http.post('sv.api/mechanicavb/deleteData', me.data)
                    .success(function (e) {
                        if (e.success) {
                            Wx.Success("Data Deleted");
                            me.init();
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        console.log(e);
                        MsgBox('Terdapat kesalahan proses data. Please contact sdms support...', MSG_INFO);
                    });
            }
            else return;
        });
    }

    me.saveData = function () {
        $http.post("sv.api/mechanicavb/save", me.data).success(function (result) {
            if (result.success) {
                Wx.Success("Data saved...");
                $('#EmployeeID, #btnEmployeeID').attr('disabled', true);
                me.isPrintAvailable = true;
                me.startEditing();
            } else {
                MsgBox(result.message, MSG_ERROR);
            }
        });
    }

    me.AvailableAll = function () {
        MsgConfirm("Apakah anda yakin???", function (result) {
            if (result) {
                $http.post("sv.api/mechanicavb/UpdateAll", me.data).success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data updated...");
                        me.init();
                    }
                });
            }
        });
    }

    me.GetEmployee = function () {
        $http.post('sv.api/mechanicavb/get', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.lookupAfterSelect(data.data);
                me.data.EmployeeName = data.employee;
                $('#EmployeeID, #btnEmployeeID').attr('disabled', true);
                me.isPrintAvailable = true;
            } 
        });
    }

    $('#EmployeeID').on('blur', function () {
        if (me.data.EmployeeID == "" || me.data.EmployeeID == null) return;
        $http.post('sv.api/mechanicavb/getEmpMekanik', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.EmployeeID = data.data.EmployeeID;
                me.data.EmployeeName = data.data.EmployeeName;
                me.GetEmployee();
            } else {
                me.data.EmployeeID = "";
                me.EmployeeID();
            }
        });
    });

    me.EmployeeID = function () {
        var lookup = Wx.klookup({
            name: "lookupEmployee",
            title: "Master Employee",
            url: "sv.api/mechanicavb/getmekanik",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "EmployeeID", title: "Kode Pelanggan", width: 120 },
                { field: "EmployeeName", title: "Nama Pelanggan", width: 260 },
                { field: "PersonnelStatusDesc", title: "Status", width: 500 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.EmployeeID = data.EmployeeID;
            me.data.EmployeeName = data.EmployeeName;
            me.GetEmployee();
        });
    }

    me.printPreview = function () {
        var ReportId = "SvRpMst005";

        Wx.showPdfReport({
            id: ReportId,
            pparam: 'default',
            rparam: me.data.UserId,
            type: "devex"
        });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MecAvbOpen",
            title: "Kehadiran Mekanik - Lookup",
            manager: MasterService,
            query: "MecAvbOpen",
            columns: [
                { field: "EmployeeID", title: "Nomor Induk" },
                { field: "EmployeeName", title: "Nama Karyawan" },
                { field: "AttendStatus", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            $('#EmployeeID, #btnEmployeeID').attr('disabled', true);
            me.isPrintAvailable = true;
            me.Apply();
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.IsAvailable = true;
        me.isPrintAvailable = true;
        $('#EmployeeID, #btnEmployeeID').removeAttr('disabled');
        me.Apply();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Kehadiran Mekanik",
        xtype: "panels",
        toolbars: 
            [{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" }],
        
        panels: [
            {
                name: "pnlRefService",
                items: [
                    { name: "CompanyCode", cls: "hide" },
                    { name: "BranchCode", cls: "hide" },
                    {
                        name: "EmployeeID",
                        text: "No Induk Karyawan",
                        type: "popup",
                        btnName: "btnEmployeeID",
                        required: true, validasi: "required", click: "EmployeeID()"
                    },
                    {
                        name: "EmployeeName",
                        text: "Nama Karyawan",
                        readonly: "true"
                    },
                    {
                        name: "IsAvailable",
                        text: "Hadir",
                        cls: "span4",
                        type: "x-switch",
                        style: "margin-bottom: 15px"
                    },
                    {
                        type: "buttons", items: [
                            { name: "btnAbsen", text: "Set Semua Hadir", icon: "icon-gear", cls: "btn btn-success", click: "AvailableAll()" },
                        ]
                    }
                   
                   
                ]
            }
            
        ]
    }
    
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstAvailableMechanicController");
    }
});