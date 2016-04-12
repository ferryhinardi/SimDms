"use strict"

function position($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('its.api/Position/ComboPosition').
        success(function (data) {
            me.dsPosition = data;
        });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "PositionLookup",
            title: "Jabatan",
            manager: MasterITS,
            query: "PositionLookup",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "EmployeeName", title: "Nama Karyawan" },
                { field: "UserName", title: "Username" },
                { field: "Position", title: "Jabatan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data = data;
                me.Apply();
            }
        });
    }

    me.save = function () {
        if (me.data.BranchCode == undefined || me.data.EmployeeID == undefined || me.data.UserID == undefined ||
            me.data.Position == "") {
            MsgBox("Ada data yang belum lengkap");
            return;
        }
        $http.post('its.api/Position/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.message == "") {
                    MsgBox("Simpan data sukses");
                } else {
                    MsgBox(data.message);
                }
            }).error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.LookupBranch = function () {
        var lookup = Wx.blookup({
            name: "BranchLookup",
            title: "Branch Lookup",
            manager: MasterITS,
            query: "BranchLookup",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "BranchName", title: "Nama Cabang" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                //me.data = data;
                me.data.BranchCode = data.BranchCode;
                me.data.BranchName = data.BranchName;
                me.Apply();
            }
        });
    }

    me.LookupEmployee = function () {
        if (me.data.BranchCode == "" || me.data.BranchCode == undefined) {
            MsgBox("Silakan isi Kode Cabang terlebih dahulu");
            return;
        }
        var lookup = Wx.blookup({
            name: "GnMstEmployeeLookup",
            title: "Karyawan",
            manager: MasterITS,
            query: new breeze.EntityQuery.from("GnMstEmployeeLookup").withParameters({ branchCode: me.data.BranchCode }),
            columns: [
                { field: "EmployeeID", title: "Kode Karyawan" },
                { field: "EmployeeName", title: "Nama Karyawan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                //me.data = data;
                me.data.EmployeeID = data.EmployeeID;
                me.data.EmployeeName = data.EmployeeName;
                me.Apply();
            }
        });
    }

    me.LookupUser = function () {
        if (me.data.BranchCode == "" || me.data.BranchCode == undefined) {
            MsgBox("Silakan isi Kode Cabang terlebih dahulu");
            return;
        }
        var lookup = Wx.blookup({
            name: "UserLookup",
            title: "User",
            manager: MasterITS,
            query: new breeze.EntityQuery.from("UserLookup").withParameters({ branchCode: me.data.BranchCode }),
            columns: [
                { field: "UserID", title: "Kode User" },
                { field: "UserName", title: "Nama Lengkap" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.UserID = data.UserID;
                me.data.UserName = data.UserName;
                me.Apply();
            }
        })
    }

    me.initialize = function () {
        me.data.Position = "";
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Jabatan",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlPosition",
                items: [
                    {
                        text: "Kode Cabang",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "BranchCode", placeHolder: "Kode Cabang", cls: "span2", readonly: true, type: "popup", click: "LookupBranch()", validasi: "required" },
                            { name: "BranchName", placeHolder: "Nama Cabang", cls: "span6", readonly: true }
                        ]
                    },
                    {
                        text: "Nama Karyawan",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "EmployeeID", placeHolder: "Kode Karyawan", cls: "span2", readonly: true, type: "popup", click: "LookupEmployee()", validasi: "required" },
                            { name: "EmployeeName", placeHolder: "Nama Karyawan", cls: "span6", readonly: true }
                        ]
                    },
                    {
                        text: "Username",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "UserID", placeHolder: "User ID", cls: "span2", readonly: true, type: "popup", click: "LookupUser()", validasi: "required" },
                            { name: "UserName", placeHolder: "User Name", cls: "span6", readonly: true },
                        ]
                    },
                    { name: "Position", text: "Jabatan", cls: "span4", type: "select2", datasource: "dsPosition" },
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("position");
    }
});