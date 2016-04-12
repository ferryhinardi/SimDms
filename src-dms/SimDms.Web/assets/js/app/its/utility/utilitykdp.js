"use strict"

function utilitykdp($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
        me.grid2.adjust();
        me.grid3.adjust();
        me.grid4.adjust();
    });

    me.grid1 = new webix.ui({
        container: "wxteammembers",
        view: "wxtable", css:"alternating",
        autowidth: false,
        width: 320,
        scrollY: true,
        columns: [
            { id: "EmployeeID", header: "Kode Anggota", width: 120 },
            { id: "EmployeeName", header: "Nama Anggota", width: 200 },
        ]
    });

    me.grid2 = new webix.ui({
        container: "wxassignkdp",
        view: "wxtable", css:"alternating",
        autowidth: false,
        width: 440,
        scrollY: true,
        columns: [
            { id: "EmployeeID", header: "Kode Anggota", width: 120 },
            { id: "EmployeeName", header: "Nama Anggota", width: 200 },
            { id: "QtyKdp", header: "Qty. KDP", width: 120 },
        ]
    });

    me.grid3 = new webix.ui({
        container: "wxOldBmMembers",
        view: "wxtable", css:"alternating",
        autowidth: false,
        width: 320,
        scrollY: true,
        columns: [
            { id: "EmployeeID", header: "Kode Anggota", width: 120 },
            { id: "EmployeeName", header: "Nama Anggota", width: 200 },
        ]
    });

    me.grid4 = new webix.ui({
        container: "wxNewBmMembers",
        view: "wxtable", css:"alternating",
        autowidth: false,
        width: 320,
        scrollY: true,
        columns: [
            { id: "EmployeeID", header: "Kode Anggota", width: 120 },
            { id: "EmployeeName", header: "Nama Anggota", width: 200 },
        ]
    });

    me.BranchLookup = function () {
        var lookup = Wx.blookup({
            name: "BranchLookup",
            title: "Cabang",
            manager: MasterITS,
            query: "BranchLookup",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "BranchName", title: "Nama Cabang" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data = {};
                me.data.BranchCode = data.BranchCode;
                me.data.BranchName = data.BranchName;
                me.Apply();
                $.ajax({
                    async: false,
                    type: "POST",
                    data: {
                        branchCode: data.BranchCode
                    },
                    url: 'its.api/utilitykdp/GetBranchManagerInfo',
                    success: function (data1) {
                        me.data.OldBranchManager = data1.manager.EmployeeName;
                        me.loadTableData(me.grid3, data1.team);
                        me.Apply();
                    }
                });
            }
        });
    }

    me.EmployeeLookup = function () {
        if (me.data.BranchCode == "" || me.data.BranchCode == undefined) {
            MsgBox("Silakan pilih Kode Cabang terlebih dahulu");
            return;
        }

        var lookup = Wx.blookup({
            name: "EmployeeLookup",
            title: "Cabang",
            manager: MasterITS,
            query: "EmployeeLookup",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "BranchName", title: "Nama Cabang" },
            ]
        });

    }

    me.NewLeaderLookup = function () {
        
    }

    me.TeamSwitch = function () {
    }

    me.MemberLookup = function () {
        
    }

    me.AddMember = function () {
        
    }

    me.RemoveMember = function () {

    }

    me.MemberKdpLookup = function () {
        
    }

    me.AddKdpMember = function () {

    }

    me.RemoveKdpMember = function () {

    }

    me.PositionSwitch = function () {

    }

    me.initialize = function () {
        me.data = {};
        me.AssignMode = "Random";
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Utility KDP",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" }            
        ],
        panels: [
            {
                items: [
                    {
                        text: "Kode Cabang",
                        type: "controls",
                        items: [
                            { name: "BranchCode", placeHolder: "Kode Cabang", cls: "span2", readonly: true, type: "popup", click: "BranchLookup()" },
                            { name: "BranchName", placeHolder: "Nama Cabang", cls: "span6", readonly: true },
                        ]
                    },
                    {
                        text: "Kode Karyawan",
                        type: "controls",
                        items: [
                            { name: "EmployeeID", placeHolder: "Kode Karyawan", cls: "span2", readonly: true, type: "popup", click: "EmployeeLookup()" },
                            { name: "EmployeeName", placeHolder: "Nama Karyawan", cls: "span4", readonly: true },
                            { name: "Position", placeHolder: "Posisi", cls: "span2", readonly: true },
                        ]
                    },
                    {
                        text: "Kode Leader",
                        type: "controls",
                        items: [
                            { name: "LeaderID", placeHolder: "Kode Leader", cls: "span2", readonly: true },
                            { name: "LeaderName", placeHolder: "Nama Leader", cls: "span4", readonly: true },
                            { name: "LeaderPosition", placeHolder: "Posisi", cls: "span2", readonly: true },
                        ]
                    },                    
                ]
            },
            {
                xtype: "tabs",
                name: "tabpage1",
                items: [
                    { name: "TeamSwitch", text: "Pindah Team" },
                    { name: "PositionSwitch", text: "Pindah Jabatan" },
                    { name: "BranchManagerSwitch", text: "Pindah Kepala Cabang" },
                ]
            },
            {
                name: "TeamSwitch",
                cls: "tabpage1 TeamSwitch",
                items: [
                    {
                        text: "Leader Baru",
                        type: "controls",
                        items: [
                            { name: "NewLeaderID", placeHolder: "Kode Leader", cls: "span2", readonly: true, type: "popup", click: "NewLeaderLookup()" },
                            { name: "NewLeaderName", placeHolder: "Nama Leader", cls: "span4", readonly: true },
                            { name: "NewLeaderPosition", placeHolder: "Posisi", cls: "span2", readonly: true },
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnTeamSwitch", text: " Proses Pindah Team", cls: "btn btn-success", icon: "icon-exchange", click: "TeamSwitch()" }
                        ]
                    }
                ]
            },
            {
                name: "PositionSwitch",
                cls: "tabpage1 PositionSwitch",
                items: [
                    {
                        text: "Kode Jabatan Baru",
                        type: "controls",
                        items: [
                            { name: "NewPosition", placeHolder: "Kode", cls: "span1", readonly: true },
                            { name: "NewPositionName", placeHolder: "Posisi", cls: "span2", readonly: true },
                        ]                            
                    },
                    {
                        text: "Anggota Team",
                        type: "controls",
                        items: [
                            { name: "MemberID", placeHolder: "Kode Member", cls: "span2", readonly: true, type: "popup", click: "MemberLookup()" },
                            { name: "MemberName", placeHolder: "Nama Member", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnAddMember", text: "Add", cls: "btn btn-info", icon: "icon-plus", click: "AddMember()" },
                            { name: "btnRemoveMember", text: "Remove", cls: "btn btn-warning", icon: "icon-remove", click: "RemoveMember()" },
                        ]
                    },
                    {
                        name: "wxteammembers",
                        type: "wxdiv",
                    },
                    {
                        type: "optionbuttons",
                        text: "Assign KDP",
                        model: "AssignMode",
                        cls: "span4",
                        items: [
                            { name: "Random", text: "Randomly" },
                            { name: "Manual", text: "Manually" }
                        ]
                    },
                    {
                        text: "Kode Karyawan",
                        type: "controls",
                        items: [
                            { name: "KdpMemberID", placeHolder: "Kode Member", cls: "span2", readonly: true, type: "popup", click: "MemberKdpLookup()" },
                            { name: "KdpMemberName", placeHolder: "Nama Member", cls: "span4", readonly: true },
                        ]
                    },
                    { name: "QtyKdp", text: "Qty. KDP", placeHolder: "0.00", cls: "span2 number", readonly: true },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnAddKdp", text: "Add", cls: "btn btn-info", icon: "icon-plus", click: "AddKdpMember()" },
                            { name: "btnRemoveKdp", text: "Remove", cls: "btn btn-warning", icon: "icon-remove", click: "RemoveKdpMember()" },
                        ]
                    },
                    {
                        name: "wxassignkdp",
                        type: "wxdiv",
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnPositionSwitch", text: " Proses Pindah Jabatan", cls: "btn btn-success", icon: "icon-exchange", click: "PositionSwitch()" }
                        ]
                    }
                ]
            },
            {
                name: "BranchManagerSwitch",
                cls: "tabpage1 BranchManagerSwitch",
                items: [
                    {
                        text: "Kepala Cabang Lama",
                        type: "controls",
                        items: [
                            { name: "OldBranchManager", placeHolder: "Kepala Cabang", cls: "span2", readonly: true },
                        ]
                    },
                    {
                        name: "wxOldBmMembers",
                        type: "wxdiv"
                    },
                    {
                        text: "Kepala Cabang Baru",
                        type: "controls",
                        items: [
                            { name: "NewBranchManagerID", placeHolder: "Kode Manager", cls: "span2", readonly: true, type: "popup", click: "ManagerLookup()" },
                            { name: "NewBranchManagerName", placeHolder: "Nama Manager", cls: "span4", readonly: true },
                        ]
                    },
                    {
                        name: "wxNewBmMembers",
                        type: "wxdiv"
                    },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnManagerSwitch", text: " Proses Pindah Kepala Cabang", cls: "btn btn-success", icon: "icon-exchange", click: "ManagerSwitch()" }
                        ]
                    }
                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    $("p[data-name='TeamSwitch']").addClass('active');

    function init(s) {
        SimDms.Angular("utilitykdp");
    }
});