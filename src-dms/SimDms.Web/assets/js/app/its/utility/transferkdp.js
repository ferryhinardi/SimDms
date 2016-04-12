"use strict"

function transferkdp($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.grid1 = new webix.ui({
        container: "wxKdpDtl",
        view: "wxtable", css:"alternating",
        checkboxRefresh: true,
        autowidth: false,
        width: 500,
        scrollY: true,
        autoHeight: false,
        height: 260,
        columns: [
            { id: "EmployeeID", header: "Kode Anggota", width: 140 },
            { id: "EmployeeName", header: "Nama Anggota", width: 200 },
            { id: "KDPQty", header: "Qty. KDP", width: 140 }
        ]
    });

    me.grid1.attachEvent("onItemClick", function (id, e, node) {
        var row = this.getItem(id);
        me.data.KDPEmployeeID = row.EmployeeID;
        me.data.KDPEmployeeName = row.EmployeeName;
        me.data.QtyKDP = row.KDPQty;
    });

    me.LookupEmployee = function () {
        var lookup = Wx.blookup({
            name: "EmployeeLookup",
            title: "Employee",
            manager: UtilityITS,
            query: new breeze.EntityQuery.from("EmployeeLookup").withParameters({ spvEmployeeID: me.spvID }),
            columns: [
                { field: "EmployeeID", title: "ID Karyawan" },
                { field: "EmployeeName", title: "Nama Karyawan" },
                { field: "PositionName", title: "Posisi" },
                { field: "BranchName", title: "Branch" },
                { field: "inquiryCount", title: "Tot. Inquiry" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data = data;
                me.Apply();
            }
        });
    };

    me.getUserEmployeeID = function () {
        $.ajax({
            async: false,
            type: "POST",
            url: 'its.api/TransferKdp/GetUserEmployeeID',
            success: function (data) {
                if (data.message == "") {
                    me.spvID = data.employeeID;
                } else {
                    MsgBox(data.message);
                    //
                }
            }
        });

    }

    me.Random = function () {
        $('#btnKDPEmployeeID').attr('disabled', 'disabled');
        $('#btnAddKDP').attr('disabled', 'disabled');
        $('#btnRemoveKDP').attr('disabled', 'disabled');
        $('#QtyKDP').attr('readonly', true);
        me.data.QtyKDP = 0;
    }

    me.Manual = function () {
        $('#btnKDPEmployeeID').removeAttr('disabled');
        $('#btnAddKDP').removeAttr('disabled');
        $('#btnRemoveKDP').removeAttr('disabled');
        $('#QtyKDP').removeAttr('readonly');
    }
    
    me.LookupEmployeeKDP = function () {
        if (me.data.EmployeeID == undefined || me.data.EmployeeID == "") return;
        var lookup = Wx.blookup({
            name: "KDPEmployeeLookup",
            title: "Employee",
            manager: UtilityITS,
            query: new breeze.EntityQuery.from("KDPEmployeeLookup").withParameters({ spvEmployeeID: me.spvID, employeeID: me.data.EmployeeID }),
            columns: [
                { field: "EmployeeID", title: "ID Karyawan" },
                { field: "EmployeeName", title: "Nama Karyawan" },
                { field: "PositionName", title: "Posisi" },
                { field: "TeamLeader", title: "Team Leader" },
                { field: "BranchCode", title: "Kode Cabang" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.KDPEmployeeID = data.EmployeeID;
                me.data.KDPEmployeeName = data.EmployeeName;

                var count = me.grid1.count();
                if (count > 0) {
                    var gridItems = {};
                    for (var i = 0; i < count ; i++) {
                        var idx = me.grid1.getIdByIndex(i);
                        var item = me.grid1.getItem(idx);
                        if (item.EmployeeID == me.data.KDPEmployeeID) {
                            me.data.QtyKDP = item.KDPQty;
                        }
                    }
                }
                me.Apply();
            }
        });
    }

    me.AddKDP = function () {
        if (me.data.KDPEmployeeID == "" || me.data.KDPEmployeeID == undefined) return;
        if (me.data.QtyKDP == 0 || me.data.QtyKDP == undefined) {
            MsgBox("Qty KDP harus lebih dari 0");
            return;
        }
        var inqCount = me.data.inquiryCount == undefined ? 0 : me.data.inquiryCount;
        var totDistInq = 0;

        var count = me.grid1.count();
        if (count > 0) {
            var gridItems = {};
            for (var i = 0; i < count ; i++) {
                var idx = me.grid1.getIdByIndex(i);
                var item = me.grid1.getItem(idx);
                if (item.EmployeeID != me.data.KDPEmployeeID) totDistInq += item.KDPQty;
            }

            if (inqCount < (totDistInq + (me.data.QtyKDP))) {
                MsgBox("Total Distribusi KDP melebihi KDP yang dimiliki oleh Employee : " + me.data.EmployeeName +
                    " (" + me.data.EmployeeID + "). \nTotal KDP yang dimiliki oleh " + me.data.EmployeeName +
                    " = " + inqCount);
                return;
            }

            for (var i = 0; i < count; i++) {
                var idx = me.grid1.getIdByIndex(i);
                var item = me.grid1.getItem(idx);
                if (item.EmployeeID == me.data.KDPEmployeeID) me.grid1.remove(idx);
            }
        }

        me.grid1.add({ EmployeeID: me.data.KDPEmployeeID, EmployeeName: me.data.EmployeeName, KDPQty: me.data.QtyKDP });
        me.data.LessInq = totDistInq + me.data.QtyKDP;
        me.data.KDPEmployeeID = "";
        me.data.KDPEmployeeName = "";
        me.data.QtyKDP = 0;
    }

    me.RemoveKDP = function () {
        MsgConfirm("Apakah anda yakin?", function (ok) {
            if (!ok) return;
            if (me.data.KDPEmployeeID == undefined || me.data.KDPEmployeeID == "") return;
            if (me.grid1.count() == 0) MsgBox("Data tidak ditemukan untuk dihapus");
            for (var i = 0; i < me.grid1.count() ; i++) {
                var idx = me.grid1.getIdByIndex(i);
                var item = me.grid1.getItem(idx);
                if (item.EmployeeID == me.data.KDPEmployeeID) {
                    var kdpDeleted = item.KDPQty;
                    me.grid1.remove(idx);
                    me.data.LessInq -= kdpDeleted;
                    me.data.EmployeeID = "";
                    me.data.EmployeeName = "";
                    me.data.QtyKDP = 0;
                }                
            }
        });
    }

    me.Process = function () {
        
        if (me.data.EmployeeID == undefined || me.data.EmployeeID == "") return;
        if (me.transferMethod == "manual") {
            if (me.grid1.count() == 0) {
                MsgBox("Anda memilih distribusi KDP secara manual.\nSilahkan input pembagian KDP terlebih dahulu");
                return;
            }
            if (me.data.LessInq != me.data.inquiryCount) {
                MsgBox("KDP Employee " + me.data.EmployeeName + " (" + me.data.EmployeeID + ") masih belum terdistribusi semua.");
                return;
            }
            var collection = [];
            me.grid1.eachRow(function (row) {
                collection.push(me.grid1.getItem(row));
            });

            var data = {
                employeeID: me.data.EmployeeID,
                details: collection
            }
            console.log(data);
            $http.post('its.api/TransferKDP/TransferKDP', data).
                success(function (data, status, headers, config) {
                    if (data.message == "") {
                        MsgBox("Transfer KDP sukses");
                        $('#btnProcess').attr("disabled", "disabled");
                    } else {
                        MsgBox("Transfer KDP gagal. \n" + data.message);
                    }
                }).error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
            });
        } else {
            if (me.data.inquiryCount == 0) {
                MsgBox("Pegawai " + me.data.EmployeeName + " (" + me.data.EmployeeID + ") tidak memiliki Inquiry KDP untuk di-distribusikan");
                return;
            }

            var data = {
                employeeID: me.data.EmployeeID,
                details: null
            }
            $http.post('its.api/TransferKDP/TransferKDP', data).
                success(function (data, status, headers, config) {
                    if (data.message == "") {
                        MsgBox("Transfer KDP sukses");
                        $('#btnProcess').attr("disabled", "disabled");
                    } else {
                        MsgBox("Transfer KDP gagal. \n" + data.message);
                    }
                }).error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
                });
        }
    }

    me.initialize = function () {
        me.data = {};
        me.spvID = "";
        me.transferMethod = "random";
        me.Random();
        me.getUserEmployeeID();
        $('#btnProcess').removeAttr("disabled");
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Transfer KDP",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" }
        ],
        panels: [
            {
                items: [
                    {
                        text: "ID Karyawan",
                        type: "controls",
                        items: [
                            { name: "EmployeeID", placeHolder: "ID Karyawan", type: "popup", cls: "span2", readonly: true, click: "LookupEmployee()" },
                            { name: "EmployeeName", placeHolder: "Nama Karyawan", cls: "span6", readonly: true }
                        ]
                    },
                    { name: "Position", text: "Posisi", readonly: true },
                    { name: "TeamLeader", text: "Team Leader", readonly: true },
                    { name: "inquiryCount", text: "Total KDP", cls: "span4 number", readonly: true },
                    {
                        type: "optionbuttons",
                        text: "Transfer KDP :",
                        model: "transferMethod",
                        cls: "span4",
                        items: [
                            { name: "random", text: "Random", click: "Random()" },
                            { name: "manual", text: "Manual", click: "Manual()" }
                        ]
                    }
                ]
            },
            {
                title: "Distribusi KDP",
                items: [
                    {
                        text: "Karyawan",
                        type: "controls",
                        items: [
                            { name: "KDPEmployeeID", placeHolder: "ID Karyawan", type: "popup", cls: "span2", readonly: true, btnName: "btnKDPEmployeeID", click: "LookupEmployeeKDP()" },
                            { name: "KDPEmployeeName", placeHolder: "Nama Karyawan", cls: "span6", readonly: true },
                        ]
                    },
                    {
                        type: "buttons",
                        cls: "span4",
                        items: [
                            { name: "btnAddKDP", text: "Add", cls: "btn btn-success", icon: "icon-plus", click: "AddKDP()" },
                            { name: "btnRemoveKDP", text: "Remove", cls: "btn btn-danger", icon: "icon-remove", click: "RemoveKDP()" },
                        ]
                    },
                    { name: "QtyKDP", text: "Qty KDP", cls: "span2 number", placeHolder: "0.00" },
                    {
                        name: "wxKdpDtl",
                        type: "wxdiv"
                    },
                    {
                        text: "Total Distribusi KDP",
                        type: "controls",
                        items: [
                            { name: "LessInq", placeHolder: "0.00", cls: "span2 number", readonly: true },
                            {
                                type: "buttons",
                                items: [
                                    { name: "btnProcess", text: " Proses", cls: "btn btn-primary", icon: "icon-gear", click: "Process()" }
                                ]
                            }                        
                        ]
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("transferkdp");
    }
});