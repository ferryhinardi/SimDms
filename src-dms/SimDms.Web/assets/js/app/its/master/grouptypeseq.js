"use strict"

function grouptypeseq($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    });

    me.grid1 = new webix.ui({
        container: "wxgrouptypeseq",
        view: "wxtable", css:"alternating",
        autowidth: false,
        width: 820,
        autoHeight: false,
        height: 420,
        scrollY: true,
        columns: [
            { id: "GroupCode", header: "Type", width: 300 },
            { id: "GroupCodeSeq", header: "Type-Seq", width: 100 },
            { id: "TypeCode", header: "Variant", width: 300 },
            { id: "TypeCodeSeq", header: "Variant-Seq", width: 100 },
        ],
    });

    me.grid1.attachEvent("onItemClick", function (id, e, node) {
        var row = this.getItem(id);
        me.data.GroupCode = row.GroupCode;
        me.data.TypeCode = row.TypeCode;
        me.data.GroupCodeSeq = row.GroupCodeSeq;
        me.data.TypeCodeSeq = row.TypeCodeSeq;
        me.Apply();
    });

    me.grid1.attachEvent("onItemDblClick", function (id, e, node) {
        var row = this.getItem(id);
        $http.post('its.api/GroupTypeSeq/GetGroupTypeSeq', { groupCode: row.GroupCode }).
        success(function (data) {
            me.clearTable(me.grid1);
            me.loadTableData(me.grid1, data);
            
        });
    });

    me.LookupType = function () {
        me.data.TypeCode = "";
        me.data.GroupCodeSeq = "";
        me.data.TypeCodeSeq = "";
        var lookup = Wx.blookup({
            name: "TypeLookup",
            title: "Type",
            manager: MasterITS,
            query: "TypeLookup",
            columns: [
                { field: "GroupCode", title: "Type" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.GroupCode = data.GroupCode;
                me.LoadTable(me.data.GroupCode);
                me.Apply();
            }
        });
    }

    me.LookupVariant = function () {
        if (me.data.GroupCode == "" || me.data.GroupCode == undefined) {
            MsgBox("Silakan isi Type terlebih dahulu");
            return;
        }
        var lookup = Wx.blookup({
            name: "VariantLookup",
            title: "Variant",
            manager: MasterITS,
            query: new breeze.EntityQuery.from("VariantLookup").withParameters({ groupCode: me.data.GroupCode }),
            columns: [
                { field: "TypeCode", title: "Variant" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.TypeCode = data.TypeCode;
                me.Apply();
            }
        });
    }

    me.Save = function () {
        if (me.data.GroupCode == "" || me.data.GroupCode == undefined ||
            me.data.TypeCode == "" || me.data.TypeCode == undefined) {
            return;
        }
        if (me.data.GroupCodeSeq == null || me.data.GroupCodeSeq == 0 ||
            me.data.TypeCodeSeq == null || me.data.TypeCodeSeq == 0) {
            MsgBox("Nilai urut tidak boleh 0 !");
            return;
        }
        
        $http.post('its.api/GroupTypeSeq/Save', me.data).
        success(function (data, status, headers, config) {
            if (data.message == "") {
                MsgBox("Data Berhasil DiUpdate");
                me.LoadTable(me.data.GroupCode);
            } else {
                MsgBox(data.message);
            }
        }).error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support', MSG_ERROR);
        });
    }

    me.LoadTable = function (groupCode) {
        $http.post('its.api/GroupTypeSeq/GetGroupTypeSeq', { groupCode: groupCode }).
        success(function (data) {
            me.loadTableData(me.grid1, data);
        });
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.LoadTable('');
        me.data.GroupCode = '';
        me.data.TypeCode = '';
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Group-Type Seq",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "initialize()" }
        ],
        panels: [
            {
                name: "pnlGroupTypeSeq",
                items: [
                    {
                        text: "Type",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "GroupCode", placeHolder: "Type", cls: "span6", type: "popup", click: "LookupType()", readonly: true },
                        ]
                    },
                    { name: "GroupCodeSeq", text: "Urutan", placeHolder: "0", cls: "span2 number" },
                    {
                        text: "Variant",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "TypeCode", placeHolder: "Variant", cls: "span6", type: "popup", click: "LookupVariant()", readonly: true },
                        ]
                    },
                    { name: "TypeCodeSeq", text: "Urutan", placeHolder: "0", cls: "span2 number" },
                    {
                        type: "buttons",
                        items: [
                            { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-plus", click: "Save()" }
                        ]
                    },
                    {
                        name: "wxgrouptypeseq",
                        type: "wxdiv",
                    }
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("grouptypeseq");
    }
});