"use strict"

function itsOutletsController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.default = function () {
        $http.post('its.api/teammember/default')
        .success(function (e) {
            me.UserID = e.UserID;
        })
    }

    me.branch = function () {
        me.clearDetail();
        var lookup = Wx.blookup({
            name: "BranchLookup",
            title: "Cabang",
            manager: MasterITS,
            query: "BranchLookup",
            defaultSort: "BranchCode asc",
            columns: [
                   { field: 'BranchCode', title: 'Kode Cabang', width: 150 },
                   { field: 'BranchName', title: 'Nama Cabang' },
            ]
        });
        lookup.dblClick(function (data) {
            me.data = data;
            me.Apply();
            me.loadDetail(data);
        });
    }

    me.loadDetail = function (data) {
        $http.post('its.api/outlet/getgrid', data)
       .success(function (e) {
           me.loadTableData(me.grid1, e);
       })
       .error(function (e) {
           MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
       });
    }

    me.AddOutlet = function () {
        if ($('#BranchCode').val() == '' || $('#BranchCode').val() == null) {
            MsgBox("Kode Cabang tidak boleh kosong!!!");
        }
        else {
            if ($('#OutletName').val() == '' || $('#OutletName').val() == null) {
                MsgBox("Nama Outlet tidak boleh kosong!!!");
            }
            else {
                $http.post('its.api/outlet/savedetail', me.data)
               .success(function (e) {
                   if (e.success) {
                       me.data.OutletID = e.data.OutletID;
                       me.loadDetail(e.data);
                       Wx.Success(e.message);
                   } else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
            }
        }
    }

    me.DeleteOutlet = function () {
        MsgConfirm("Yakin menghapus data ini?", function (result) {
            if (result) {
                $http.post('its.api/outlet/deletedetail', me.data)
               .success(function (e) {
                   if (e.success) {
                       Wx.Success(e.message);
                       me.data.OutletID = '';
                       me.data.OutletName = '';
                       me.Apply();
                       me.loadDetail(me.data);

                   } else {
                       MsgBox(e.message, MSG_ERROR);
                   }
               })
               .error(function (e) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
            }
        });
    }

    me.print = function () {
        var par = 'companycode';

        Wx.showPdfReport({
            id: "PmRpOutlet",
            pparam: par,
            rparam: me.UserID,
            type: "devex"
        });
    }

    me.grid1 = new webix.ui({
        container: "wxOutlets",
        view: "wxtable", css:"alternating",
        autoWitdh: false,
        width: 570,
        columns: [
            { id: "BranchCode", header: "Kode Cabang", width: 150 },
            { id: "OutletID", header: "ID Outlet", width: 120 },
            { id: "OutletName", header: "Nama Outlet", width: 300 },
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.data = this.getItem(me.grid1.getSelectedId().id);
                    me.data.old = me.grid1.getSelectedId();
                    me.Apply();
                }
            }
        }
    });

    me.initialize = function () {
        me.default();
        me.clearDetail();
        me.clearTable(me.grid1);
    }

    me.clearDetail = function () {
        me.data = {};
        me.clearTable(me.grid1);
    }

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Outlets",
        xtype: "panels",
        toolbars: [
            { name: "btnNew", text: "New", cls: "btn btn-success", icon: "icon-refresh", click: "cancelOrClose()" },
            { name: "btnPrint", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "print()"},
        ],
        panels: [
            {
                name: "pnlOutlets",
                items: [
                   { name: "BranchCode", text: "Kode Cabang", cls: "span3 full", placeHolder: "Kode Cabang", readonly: true, type: "popup", required: true, click: "branch()" },
                   { name: "OutletID", text: "Kode Outlet", cls: "span2", readonly: true },
                   { name: "OutletName", text: "Nama Outlet", cls: "span6", required: true },
                   {
                       type: "buttons",
                       items: [
                               { name: "btnAddPart", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddOutlet()" },
                               { name: "btnDeletePart", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteOutlet()" },
                       ]
                   },
                   { name: "wxOutlets", type: "wxdiv" },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("itsOutletsController");
    }
});