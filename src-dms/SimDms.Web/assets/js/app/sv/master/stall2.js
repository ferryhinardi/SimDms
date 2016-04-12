"use strict"

function svStallController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "StallBrowse",
            title: "Stall",
            manager: MasterService,
            query: "StallBrowse",
            columns: [
                    { field: "StallCode", title: "Stall Code" },
                    { field: "Description", title: "Description" },
                    { field: "HaveLiftString", title: "Have Lift" },
                    { field: "IsActiveString", title: "Is Active" },
            ]
        });
        lookup.dblClick(function (data) {
            me.isLoadData = true;
            me.data = data;
            me.Apply();

            setTimeout(function () {
                me.hasChanged = true;
                me.isSave = true;
                me.isLoadData = true;
                me.Apply();

            }, 200);
        });
    }

    me.save = function () {
        $http.post('sv.api/stall/save', me.data)
     .success(function (e) {
         if (e.success) {
             Wx.Success("Data Save");
         } else {
             MsgBox(e.message, MSG_ERROR);
         }
     })
     .error(function (e) {
         console.log(e);
         MsgBox('Terdapat kesalahan dalam proses data, Hubungi SDMS Support', MSG_INFO);
     });
    }

    me.delete = function () {
        MsgConfirm("Apakah anda yakin???", function (result) {
            if (result) {
                $http.post('sv.api/stall/deleteData', me.data)
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
                        MsgBox('Terdapat kesalahan dalam proses data, Hubungi SDMS Support', MSG_INFO);
                    });
            }
            else return;
        });
    }

    me.default = function () {
        $http.post('sv.api/stall/default').
        success(function (e) {
            me.data.UserId = e.UserId;
        })
    }

    me.printPreview = function () {
        var ReportId = "SvRpMst003";

        Wx.showPdfReport({
            id: ReportId,
            pparam: 'default',
            rparam: me.data.UserId,
            type: "devex"
        });

    }

    me.initialize = function () {
        me.default();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Stall",
        xtype: "panels",
        toolbars: [{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    //{ name: "btnEdit",   text: "Edit",   cls:"btn btn-primary",    icon: "icon-edit",   show: "isLoadData && !isSave", click: "allowEdit()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print",  click: "printPreview()" }],
        panels: [
            {
                name: "pnlStall",
                items: [
                    { name: "StallCode", text: "Kode Stall", cls: "span4 full", required: true },
                    { name: "Description", text: "Keterangan", cls: "span4 full", required: true },
                    { name: "HaveLift", text: "Ada Lift", cls: "span4 full", style:"margin-bottom:15px", type: "x-switch", readonly: false },
                    { name: "IsActive", text: "Aktif", cls: "span4", type: "x-switch", readonly: false },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svStallController");
    }
});