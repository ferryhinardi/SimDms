"use strict"

function organization($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
        me.tree1.adjust();
    });

    $("#currentpage").bind("layout", function () {
        me.grid1.adjust();
        me.tree1.adjust();
    })

    me.grid1 = new webix.ui({
        container: "wxEmployees",
        view: "wxtable", css:"alternating",
        width: 520,
        autoHeight: false,
        height: 540,
        scrollY: true,
        scrollX: true,
        resizecolumn: true,
        drag: true,
        columns: [
            { id: "BranchAbv", header: [ { content:"textFilter" }, "Cabang"], width: 180, css: { 'font-size': '9' } },
            { id: "Position", header: [{ content: "textFilter" }, "Posisi"], width: 50, css: { 'font-size': '9' } },
            { id: "Employee", header: [{ content: "textFilter" }, "Karyawan"], width: 220, css: { 'font-size': '9' } },
            { id: "CountKDP", header: [{ content: "textFilter" }, "KDP"], width: 50, css: { 'font-size': '9' } }
        ]
    });

    me.tree1 = new webix.ui({
        container: "wxTree",
        view: "treetable",
        width: 680,
        autoHeight: false,
        height: 540,
        scrollY: true,
        scrollX: true,
        resizeColumn: true,
        drag: true,
        columns: [
            { id: "Position", header: "Posisi", template: "{common.treetable()} #Position#", width: 130 },
            { id: "BranchAbv", header: "Cabang", width: 190 },
            { id: "Employee", header: "Karyawan", width: 280 },
            { id: "CountKDP", header: "KDP", width: 60 }
        ]
    });

    //1st event triggered : LeaveMember
    me.grid1.attachEvent("onBeforeDrop", function (context, e) {
        var item = me.tree1.getItem(context.start);
        if (context.from.config.id == this.config.id) {
            return false;
        }
        var data = {
            source: item
        };
        if (item.Position == 'S') {
            MsgConfirm("Salesman " + item.Employee + " masih memiliki KDP sebanyak " + item.CountKDP +
            ". \r\nTekan CANCEL untuk Transfer KDP terlebih dahulu. \r\nTekan OK untuk melanjutkan.", function (ok) {
                if (!ok) {
                    MsgBox("Silakan lakukan drag n drop dari Salesman " + item.Employee + " ke Salesman lain di bawah team leader yang sama.");
                }
                else {
                    $.ajax({
                        async: false,
                        type: "POST",
                        data: JSON.stringify(data),
                        url: 'its.api/organization/leavemember',
                        contentType: "application/json",
                        success: function (data) {
                            if (data.message != "") {
                                MsgBox(data.message);
                            } else {
                                me.LoadEmployees(me.data.BranchCode);
                                me.LoadTree(me.data.BranchCode);
                            }
                        }
                    });
                }
            });
        } else {
            $.ajax({
                async: false,
                type: "POST",
                data: JSON.stringify(data),
                url: 'its.api/organization/leavemember',
                contentType: "application/json",
                success: function (data) {
                    if (data.message != "") {
                        MsgBox(data.message);
                    } else {
                        me.LoadEmployees(me.data.BranchCode);
                        me.LoadTree(me.data.BranchCode);
                    }
                }
            });
        }        
        return false;
    });

    //1st event triggered
    me.tree1.attachEvent("onBeforeDrop", function (context, e) {
        var success = false;
        var item = [];
        if (context.from.config.id == this.config.id) {
            item = this.getItem(context.start);
            var target = this.getItem(context.target);
            var parent = this.getItem(item.$parent);
            if (target == undefined) return success;
            if (item.Position == target.Position && item.$parent == target.$parent) {
                me.MemberTransfer(item, target);
            } else if (item.Position != target.Position && target.PositionID > item.PositionID
                && target.EmployeeID != item.TeamLeader
                && target.PositionID != parent.PositionID
                && item.lvl != target.lvl
                && item.BranchCode == target.BranchCode) {
                me.Promotion(item, target);
            }
        } else if (context.target == null) {
            item = me.grid1.getItem(context.start);
            me.EmployeeMutation(item);
        } else {
            item = me.grid1.getItem(context.start);
            target = me.tree1.getItem(context.target);
            if (target == undefined) return success;
            success = me.JoinMember(item, target);
        }
        return success;
    });

    //Transfer KDP & Member
    me.MemberTransfer = function (item, target) {
        var branchCode = item.BranchCode;
        var emp1 = item.EmployeeID.toString();
        var emp2 = target.EmployeeID.toString();
        if (emp1 == emp2) return;
        if (item.Position == "S" && item.CountKDP == target.CountKDP) return;

        var members1 = [];
        var members2 = [];
        var name1 = "";
        var name2 = "";

        $.ajax({
            async: false,
            type: "POST",
            data: {
                branchCode: branchCode,
                employee1: item.EmployeeID,
                employee2: target.EmployeeID
            },
            url: 'its.api/organization/selectmembers',
            success: function (data) {
                if (data.message == "") {
                    members1 = data.result1;
                    members2 = data.result2;
                    name1 = item.Employee + " (" + (item.Position == 'S' ? item.CountKDP : members1.length) + ")";
                    name2 = target.Employee + " (" + (item.Position == 'S' ? target.CountKDP : members2.length) + ")";
                } else {
                    MsgBox(data.message);
                }                
            }
        });

        webix.ui({
            view: "window",
            id: "winMemberDist",
            move: false,
            modal: true,
            position: "center",
            width: 800,
            head: {
                view: "toolbar",
                cols: [
                    { view: "label", label: "<h2>Member/KDP Distribution</h2>" },
                    { id: "branchCode", view: "label", label: branchCode, hidden: true },
                    { view: "button", label: "Apply", width: 80, align: 'right', click: "ApplyTransfer()" },
                    { view: "button", label: "Cancel", width: 80, align: 'right', click: "$$('winMemberDist').close();" },
                    { view: "button", label: "Random", width: 80, align: 'right', click: "RandomizeKDP('" + emp1 + "')" }
                ]
            },
            body: {
                cols: [
                    {
                        rows: [
                            { view: "label", label: "<b>" + item.PositionName + ":</b>" },
                            { id: "emp1", view: "text", value: name1, readonly: true },
                            { id: "info1", view: "label", hidden: true, label: item.Employee },
                            { id: "id1", view: "label", hidden: true, label: item.EmployeeID },
                            { view: "label", label: "<b>Members:</b>" },
                            { id: "list1", view: "list", data: members1, template: "#Member#", select: "multiselect", height: 400 }
                        ]
                    },
                    {
                        width: 40,
                        rows: [
                            { name: "btnHidden1", label: "", view: "label" },
                            { name: "btnHidden2", label: "", view: "label" },
                            { name: "btnSwapRight", label: ">", view: "button", click: "SwapRight()" },
                            { name: "btnSwapLeft", label: "<", view: "button", click: "SwapLeft()" },
                            { name: "btnSwapAllRight", label: ">>", view: "button", click: "SwapAllRight()" },
                            { name: "btnSwapAllLeft", label: "<<", view: "button", click: "SwapAllLeft()" },
                        ]
                    },
                    {
                        rows: [
                            { view: "label", label: "<b>" + target.PositionName + ":</b>" },
                            { id: "emp2", view: "text", value: name2, readonly: true },
                            { id: "info2", view: "label", hidden: true, label: target.Employee },
                            { id: "id2", view: "label", hidden: true, label: target.EmployeeID },
                            { view: "label", label: "<b>Members:</b>" },
                            { id: "list2", view: "list", data: members2, template: "#Member#", select: "multiselect", height: 400 }
                        ]
                    }
                ]
            }
        }).show();
    }
    
    //Level Promotion
    me.Promotion = function (item, target) {
        var branchCode = me.data.BranchCode;
        var member = {};
        $.ajax({
            async: false,
            type: "POST",
            data: {
                employeeID: item.EmployeeID
            },
            url: "its.api/Organization/GetPromotionData",
            success: function (data) {
                if (data.message == "") {
                    member = data.member;
                    webix.ui({
                        view: "window",
                        id: "winPromotion",
                        move: false,
                        modal: true,
                        autofit: false,
                        position: "center",
                        head: {
                            view: "toolbar",
                            cols: [
                                { view: "label", label: "<h2>Promosi Karyawan</h2>" },
                                { id: "branchCode", view: "label", label: branchCode, hidden: true },
                                { view: "button", label: "Apply", width: 80, align: 'right', click: "ApplyPromotion()" },
                                { view: "button", label: "Cancel", width: 80, align: 'right', click: "$$('winPromotion').close();" }
                            ]
                        },
                        body: {
                            rows: [
                                {
                                    cols: [
                                        { view: "label", label: "Nama Karyawan", width: 100 },
                                        { id: "promoteEmployee", view: "text", value: member.EmployeeID + " - " + member.EmployeeName, readonly: true, width: 500 },
                                        { id: "promoteEmployeeID", view: "label", label: member.EmployeeID, hidden: true },
                                        { id: "promoteEmployeeName", view: "label", label: member.EmployeeName, hidden: true },
                                    ]
                                },
                                {
                                    cols: [
                                        { view: "label", label: "Posisi Lama", width: 100 },
                                        { id: "promoteOldPos", view: "text", value: member.PositionName, readonly: true, width: 200 },
                                        { view: "label", label: "Posisi Baru", width: 100 },
                                        { id: "promoteNewPos", view: "text", value: member.NewPositionName, readonly: true, width: 200 },
                                    ]
                                },
                                {
                                    cols: [
                                        { view: "label", label: "Leader Lama", width: 100 },
                                        { id: "promoteOldLeader", view: "text", value: member.TeamLeaderID + " - " + member.TeamLeaderName + " (" + member.LeaderPosName + ")", readonly: true, width: 500 },
                                    ]
                                },
                                {
                                    cols: [
                                        { view: "label", label: "Leader Baru", width: 100 },
                                        { id: "promoteNewLeader", view: "text", value: member.NewLeaderID + " - " + member.NewLeaderName + " (" + member.NewLeaderPosName + ")", readonly: true, width: 500 },
                                        { id: "promoteNewLeaderID", view: "label", label: member.NewLeaderID, hidden: true },
                                    ]
                                },
                                {
                                    cols: [
                                        { view: "label", label: "Pengganti", width: 100 },
                                        { id: "btnEmployee", view: "button", label: "...", disabled: member.NoNeedReplacement, width: 40, click: "BrowseReplacement()" },
                                        { id: "noNeedReplacement", view: "label", label: member.NoNeedReplacement.toString(), hidden: true },
                                        { id: "subEmployeeID", view: "text", readonly: true, width: 160 },
                                        { id: "subEmployeeName", view: "text", readonly: true, width: 300 }
                                    ]
                                }
                            ]
                        }
                    }).show();
                } else return;
            }
        });
    }

    //Change Branch for Member
    me.JoinMember = function (item, target) {
        var success = false;
        var data = {
            branchCode: me.data.BranchCode,
            employeeID: item.EmployeeID,
            leaderID: target.EmployeeID
        };
        
        if (item.PositionID < target.PositionID) {
            if (target.BranchCode != item.BranchCode) {
                if (item.RelatedUser != '' && item.RelatedUser != null) {
                    MsgConfirm("Pindahkan User ID (" + item.RelatedUser + ") yang terhubung  dengan Employee " + item.Employee + "ke " + target.BranchAbv + " ?", function (ok) {
                        if (!ok) return success;
                    });
                }
            }
            MsgConfirm("Pindahkan " + item.EmployeeName + " ke " + target.BranchAbv + " dengan Leader " + target.EmployeeName + "?",
                function (ok) {
                    if (ok) {
                        $.ajax({
                            async: false,
                            type: "POST",
                            data: data,
                            url: 'its.api/organization/joinmember',
                            success: function (data) {
                                if (data.message == "") {
                                    me.LoadEmployees(me.data.BranchCode);
                                    me.LoadTree(me.data.BranchCode);
                                } else {
                                    MsgBox(data.message);
                                }
                            }
                        });
                    }
                    return success;
                });
        }
        return success;
    }

    //Select replacement
    me.ConfirmPromotion = function () {
        var noNeedReplacement = $$('noNeedReplacement').data.label;
        if (noNeedReplacement == "false" &&
            ($$('subEmployeeID').data.value == undefined || $$('subEmployeeID').data.value == "")) {
            MsgBox("Pilih karyawan pengganti terlebih dahulu");
            return;
        }

        var data = {
            employeeID: $$('promoteEmployeeID').data.label,
            newLeaderID: $$('promoteNewLeaderID').data.label,
            replacementID: $$('subEmployeeID').data.value
        }

        MsgConfirm("Apakah anda yakin ?", function (ok) {
            if (!ok) return;

            $.ajax({
                async: false,
                type: "POST",
                data: data,
                url: "its.api/organization/confirmPromotion",
                success: function (data) {
                    if (data.message == "") {
                        MsgBox(data.info);
                    } else {
                        MsgBox(data.message);
                    }
                }
            });
            $$('winPromotion').close();
            $('#btnRefreshTree').click();
        });
    }

    //Change Branch for BM
    me.ConfirmMutation = function () {
        var branchCode = $$('Branches').getValue();
        var branchDesc = $$('Branches').getText();
        var employeeID = $$('MutEmpCode').getValue();
        var employeeName = $$('MutEmpName').getValue();

        if (branchCode == "") {
            MsgBox("Silakan isi Kode Cabang tujuan");
            return;
        }

        MsgConfirm("Fungsi ini akan melakukan relokasi karyawan " + employeeName + " ke cabang " +
            branchDesc + ", TANPA LEADER. Apakah anda yakin ?", function (ok) {
                if (!ok) return;

                $.ajax({
                    async: false,
                    type: "POST",
                    data: {
                        branchCode: branchCode,
                        employeeID: employeeID
                    },
                    url: "its.api/organization/EmployeeMutation",
                    success: function (data) {
                        if (data.message == "") {
                            $$('winEmployeeMutation').close();
                            $('#btnRefreshTree').click();
                        } else {
                            MsgBox(data.message);
                        }
                    }
                });
            });
    }

    me.EmployeeMutation = function (item) {
        var data = {
            employeeID: item.EmployeeID
        }
        var comboItems = [];
        var branchCode = "";

        if (me.data.IsAllBranch) {
            $.ajax({
                async: false,
                type: "POST",
                url: "its.api/organization/combobranches",
                success: function (result) {
                    comboItems = result.items;
                }
            });
        } else {
            branchCode = me.data.BranchCode;
        }


        webix.ui({
            view: "window",
            id: "winEmployeeMutation",
            move: false,
            modal: true,
            position: "center",
            width: 500,
            head: {
                view: "toolbar",
                cols: [
                    { view: "label", label: "<h2>Mutasi Karyawan</h2>" },
                    { view: "button", label: "Apply", width: 80, align: 'right', click: "ApplyMutation()" },
                    { view: "button", label: "Cancel", width: 80, align: 'right', click: "$$('winEmployeeMutation').close();" }
                ]
            },
            body: {
                rows: [
                    {
                        cols: [
                            { view: "label", label: "Karyawan", width: 100 },
                            { id: "MutEmpCode", view: "text", value: item.EmployeeID, readonly: true, width: 80 },
                            { id: "MutEmpName", view: "text", value: item.EmployeeName, readonly: true, width: 200 },
                        ]
                    },
                    {
                        cols: [
                            { view: "label", label: "Cabang", width: 100 },
                            { id: "Branches", view: "combo", value: branchCode, disabled: branchCode != "" , options: comboItems }
                        ]
                    }
                ]
            }
        }).show();
    }
    
    me.CloseDistWindow = function () {
        $$('winMemberDist').close();
        me.Refresh(me.data.BranchCode);
    };

    me.AllBranchChanged = function () {
        if (me.data.IsAllBranch) {
            $('#btnBranchCode').attr('disabled', 'disabled');
            me.data.BranchCode = "";
            me.data.BranchName = "[ALL BRANCH]";
            me.LoadEmployees('');
            me.LoadTree('');
        } else {
            $('#btnBranchCode').removeAttr('disabled');
            me.LoadLocalBranch();
        }
    }

    me.BranchLookup = function () {
        var lookup = Wx.blookup({
            name: "BranchLookup",
            title: "Kantor Cabang",
            manager: MasterITS,
            query: "BranchLookup",
            columns: [
                { field: "BranchCode", title: "Kode Cabang" },
                { field: "BranchName", title: "Nama Cabang" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCode = data.BranchCode;
                me.data.BranchName = data.BranchName;
                me.LoadEmployees(data.BranchCode);
                me.LoadTree(data.BranchCode);
            }
        });
    }

    me.LoadLocalBranch = function () {
        $http.post('its.api/organization/loadlocalbranch').
            success(function (data, status, headers, config) {
                me.data.BranchCode = data.BranchCode;
                me.data.BranchName = data.BranchName;
                me.LoadEmployees(data.BranchCode);
                me.LoadTree(data.BranchCode);
        });
    }

    me.LoadEmployees = function (branchCode) {
        $http.post('its.api/organization/loademployees', { branchCode: branchCode, position: '' }).
            success(function (data, status, headers, config) {
                me.grid1.clearAll();
                me.loadTableData(me.grid1, data);
        });
    }

    me.LoadTree = function (branchCode) {
        $http.post('its.api/organization/loadorganization', { branchCode: branchCode }).
            success(function (data, status, headers, config) {
                if (data.result.length == 0) {
                    me.tree1.clearAll();
                    return;
                }
                var str = JSON.stringify(data.result);
                me.tree1.clearAll();
                me.tree1.parse(str);
        });
    }

    me.Refresh = function (branchCode) {
        me.LoadEmployees(branchCode);
        me.LoadTree(branchCode);
    }

    me.ReplacementLookup = function () {
        $$('winPromotion').hide();
        var lookup = Wx.blookup({
            name: "ReplacementLookup",
            title: "Karyawan Pengganti",
            manager: MasterITS,
            query: new breeze.EntityQuery.from("ReplacementLookup").withParameters({ branchCode: me.data.BranchCode, employeeID: $$('promoteEmployeeID').data.label }),
            columns: [
                { field: "EmployeeID", title: "Kode Karyawan" },
                { field: "EmployeeName", title: "Nama Karyawan" },
                { field: "PositionName", title: "Posisi Karyawan" },
                { field: "MemberCount", title: "Jumlah Member" },
                { field: "TeamLeaderName", title: "Nama Leader" },
                { field: "TeamLeaderPosition", title: "Posisi Leader" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                $$('subEmployeeID').setValue(data.EmployeeID);
                $$('subEmployeeName').setValue(data.EmployeeName);
            }
            $$('winPromotion').show();
        });
        lookup.onCancel(function (e) {
            $$('winPromotion').show();
        })       
    }

    me.initialize = function () {
        $('div.webix_ss_filter input').val("");
        me.data.IsAllBranch = false;
        me.LoadLocalBranch();
        me.success = false;
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Organization",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: " Reload", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" },
            { name: "btnRefreshTree", text: "", click: "Refresh(data.BranchCode)" },
            { name: "btnBrowseReplacement", text: "", click: "ReplacementLookup()" },
            { name: "btnConfirmPromotion", text: "", click: "ConfirmPromotion()" },
            { name: "btnConfirmMutation", text: "", click: "ConfirmMutation()" },
            { name: "btnCloseDistWindow", text: "", click: "CloseDistWindow()" },
        ],
        panels: [
             {
                 items: [
                     {
                         text: "Kode Cabang",
                         type: "controls",
                         items: [
                             { name: "BranchCode", placeHolder: "Kode Cabang", cls: "span2", readonly: true, type: "popup", click: "BranchLookup()" },
                             { name: "BranchName", placeHolder: "Nama Cabang", cls: "span4", readonly: true },
                             { name: "IsAllBranch", type: "ng-check", cls: "span1", change: "AllBranchChanged()" },
                             { type: "label", text: "All Branch", cls: "span1", style: "line-height: 33px;" }
                         ]
                     },
                 ]
             },
             {
                 items: [
                     {
                         name: "wxEmployees",
                         type: "wxdiv",
                         cls: "span4",
                     },
                     {
                         name: "wxTree",
                         type: "wxdiv",
                         cls: "span4",
                     }
                 ]
             }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("organization");
    }
});

$('#btnRefreshTree').hide();
$('#btnBrowseReplacement').hide();
$('#btnConfirmPromotion').hide();
$('#btnConfirmMutation').hide();
$('#btnCloseDistWindow').hide();

function SwapRight () {
    var item = $$('list1').getSelectedItem();
    if (item == undefined) return;

    // 1 item
    if (item.length == undefined) {
        var id = $$('list1').getSelectedId();

        $$('list2').add(item);
        $$('list1').remove(id);
        UpdateEmployeeInfo();
    } else {
        var id = $$('list1').getSelectedId();
        for (var i = 0; i < item.length; i++) {
            $$('list2').add(item[i]);
            $$('list1').remove(id[i]);
        }
        UpdateEmployeeInfo();
    }    
}

function SwapLeft () {
    var item = $$('list2').getSelectedItem();
    if (item == undefined) return;

    // 1 item
    if (item.length == undefined) {
        var id = $$('list2').getSelectedId();

        $$('list1').add(item);
        $$('list2').remove(id);
        UpdateEmployeeInfo();
    } else {
        var id = $$('list2').getSelectedId();
        for (var i = 0; i < item.length; i++) {
            $$('list1').add(item[i]);
            $$('list2').remove(id[i]);
        }
        UpdateEmployeeInfo();
    }
}

function SwapAllRight() {
    while (true) {
        var count = $$('list1').count();
        if (count == 0) break;
        var id = $$('list1').getIdByIndex(0);
        var item = $$('list1').getItem(id);
        $$('list2').add(item);
        $$('list1').remove(id);
    }
    UpdateEmployeeInfo();
}

function SwapAllLeft() {
    while (true) {
        var count = $$('list2').count();
        if (count == 0) break;
        var id = $$('list2').getIdByIndex(0);
        var item = $$('list2').getItem(id);
        $$('list1').add(item);
        $$('list2').remove(id);
    }
    UpdateEmployeeInfo();
}

function UpdateEmployeeInfo() {
    $$('emp1').setValue($$('info1').config.label + " (" + $$('list1').count() + ")");
    $$('emp2').setValue($$('info2').config.label + " (" + $$('list2').count() + ")");
}

function ApplyTransfer() {
    var length1 = $$('list1').count();
    var length2 = $$('list2').count();
    
    var data1 = "";
    var data2 = "";
    for (var i = 0; i < length1 ; i++) {
        var id = $$('list1').getIdByIndex(i);
        var item = $$('list1').getItem(id);
        data1 += item.KeyID + ",";
    }
    for (var i = 0; i < length2 ; i++) {
        var id = $$('list2').getIdByIndex(i);
        var item = $$('list2').getItem(id);
        data2 += item.KeyID + ",";
    }

    var data = {
        branchCode: $$('branchCode').data.label,
        id1: $$('id1').data.label,
        id2: $$('id2').data.label,
        data1: data1,
        data2: data2
    }
    $.ajax({
        async: false,
        type: "POST",
        data: JSON.stringify(data),
        url: 'its.api/organization/transfermember',
        contentType: "application/json",
        success: function (data) {
            if (data.message == '') {
                $$('winMemberDist').close();
                $('#btnRefreshTree').click();
            } else {
                MsgBox(data.message);
                return;
            }
            
        }
    });
}

function BrowseReplacement() {
    $('#btnBrowseReplacement').click();
}

function ApplyPromotion() {
    $('#btnConfirmPromotion').click();
}

function ApplyMutation() {
    $('#btnConfirmMutation').click();
}

function RandomizeKDP(employeeID) {
    MsgConfirm("Apakah anda yakin untuk membagikan KDP secara otomatis kepada seluruh anggota team? (Tidak dapat dibatalkan/dikembalikan)",
        function (ok) {
            if (!ok) return;
            $.ajax({
                async: false,
                type: "POST",
                data: {
                    employeeID: employeeID
                },
                url: "its.api/organization/RandomizeKDP",
                success: function (data) {
                    if (data.message != "") MsgBox(data.message);
                    else {
                        $('#btnCloseDistWindow').click();
                    }
                }
            });           
    });    
}