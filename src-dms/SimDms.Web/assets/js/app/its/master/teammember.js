"use strict"

function teammember($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.grid1 = new webix.ui({
        container: "wxteammembers",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        height: 350,
        width : 350,
        autoHeight: false,
        autowidth: false,
        columns: [
            { id: "EmployeeID", header: "Kode Anggota", width: 120 },
            { id: "EmployeeName", header: "Nama Anggota", width: 230 },
        ]
    });

    me.grid1.attachEvent("onItemDblClick", function (id, e, node) {
        var row = this.getItem(id);
        me.data.EmployeeID = row.EmployeeID;
        me.data.EmployeeName = row.EmployeeName;
        me.Apply();
    });

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
                me.data.BranchCode = data.BranchCode;
                me.data.BranchName = data.BranchName;
                me.data.EmployeeID = "";
                me.data.EmployeeName = "";
                me.showMembers = false;
                me.Apply();
            }
        });
    }

    me.LookupLeader = function () {
        if (me.data.BranchCode == '' || me.data.BranchCode == undefined) {
            MsgBox("Branch Code harus diisi terlebih dahulu");
            return;
        }
        var lookup = Wx.blookup({
            name: "LeaderLookup",
            title: "Leader Lookup",
            manager: MasterITS,
            query: new breeze.EntityQuery.from("LeaderLookup").withParameters({ branchCode: me.data.BranchCode }),
            columns: [
                { field: "EmployeeID", title: "Kode Karyawan" },
                { field: "EmployeeName", title: "Nama Karyawan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.LeaderID = data.EmployeeID;
                me.data.LeaderName = data.EmployeeName;
                me.Apply();
                me.showMembers = true;
                me.GetMembers();
            }
        });
    }

    me.LookupSalesman = function () {
        var lookup = Wx.blookup({
            name: "SalesmanLookup",
            title: "Salesman Lookup",
            manager: MasterITS,
            query: new breeze.EntityQuery.from("SalesmanLookup").withParameters({ branchCode: me.data.BranchCode }),
            columns: [
                { field: "EmployeeID", title: "Kode Karyawan" },
                { field: "EmployeeName", title: "Nama Karyawan" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.EmployeeID = data.EmployeeID;
                me.data.EmployeeName = data.EmployeeName;
                me.Apply();
            }
        });
    }

    me.GetMembers = function () {
        $http.post('its.api/TeamMember/GetMembers', { branchCode: me.data.BranchCode, employeeID: me.data.LeaderID }).
        success(function (data) {
            me.loadTableData(me.grid1, data);
        });
    }

    me.GetProductType = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/TeamMember/GetProductType',
            success: function (data) {
                me.productType = data;
            }
        });

    }

    me.Add = function () {
        if (me.data.EmployeeID == '' || me.data.EmployeeID == undefined) {
            MsgBox("Pilih salesman terlebih dahulu");
            return;
        }
        $.ajax({
            async: false,
            type: "POST",
            data: {
                leaderID: me.data.LeaderID,
                employeeID: me.data.EmployeeID
            },
            url: 'its.api/TeamMember/Add',
            success: function (data) {
                if (data.success) {
                    me.data.EmployeeID = "";
                    me.data.EmployeeName = "";
                    me.GetMembers();
                }
            }
        });

    }

    me.Remove = function () {
        if (me.data.EmployeeID == '' || me.data.EmployeeID == undefined) {
            MsgBox("Pilih salesman terlebih dahulu");
            return;
        }
        $.ajax({
            async: false,
            type: "POST",
            data: {
                leaderID: me.data.LeaderID,
                employeeID: me.data.EmployeeID
            },
            url: 'its.api/TeamMember/Remove',
            success: function (data) {
                if (data.success) {
                    me.data.EmployeeID = "";
                    me.data.EmployeeName = "";
                    me.GetMembers();
                }
            }
        });

    }

    me.initialize = function () {
        me.showMembers = false;
        me.GetProductType();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Team Members",
        xtype: "panels",
        panels: [
            {
                name: "pnlTeamMembers",
                items: [
                    {
                        text: "Kode Cabang",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "BranchCode", cls: "span2", placeHolder: "Kode Cabang", readonly: true, type: "popup", click:"LookupBranch()" },
                            { name: "BranchName", cls: "span6", placeHolder: "Nama Cabang", readonly: true }
                        ]
                    },
                    {
                        text: "Sales Head",
                        type: "controls",
                        show: "productType === '4W'",
                        items: [
                            { name: "LeaderID", cls: "span2", placeHolder: "Kode Karyawan", readonly: true, type: "popup", click: "LookupLeader()" },
                            { name: "LeaderName", cls: "span6", placeHolder: "Nama Karyawan", readonly: true }
                        ]
                    },
                    {
                        text: "Sales Coordinator",
                        type: "controls",
                        show: "productType === '2W'",
                        items: [
                            { name: "LeaderID", cls: "span2", placeHolder: "Kode Karyawan", readonly: true, type: "popup", click: "LookupLeader()" },
                            { name: "LeaderName", cls: "span6", placeHolder: "Nama Karyawan", readonly: true }
                        ]
                    },
                ]
            },
            {
                name: "pnlTeamDetails",
                title: "Rincian Team",
                show: "showMembers === true",
                items: [
                    {
                        text: "Pilih anggota team",
                        type: "controls",
                        items: [
                            { name: "EmployeeID", cls: "span2", placeHolder: "Kode Karyawan", readonly: true, type: "popup", click: "LookupSalesman()" },
                            { name: "EmployeeName", cls: "span6", placeHolder: "Nama Karyawan", readonly: true }
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnAdd", text: "Add", cls: "btn btn-info", icon: "icon-plus", click: "Add()" },
                            { name: "btnRemove", text: "Remove", cls: "btn btn-warning", icon: "icon-remove", click: "Remove()" },
                        ]
                    },
                    {
                        name: "wxteammembers",
                        type: "wxdiv",
                    },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("teammember");
    }
});