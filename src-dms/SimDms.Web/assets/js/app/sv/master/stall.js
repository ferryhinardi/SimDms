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
            me.lookupAfterSelect(data);
            $('#StallCode').attr('disabled', 'disabled');
            me.isSave = false;
            if (me.data.HaveLift == true) $('#HaveLift').attr('checked', true);
            else {
                $('#HaveLift').removeAttr('checked');
            }
            if (me.data.IsActive == true) $('#IsActive').attr('checked', true);
            else {
                $('#IsActive').removeAttr('checked');
            }
            me.isPrintAvailable = me.isLoadData = true;
            me.Apply();
        });
    }

    me.saveData = function () {
        if ($('#HaveLift').is(':checked')) me.data.HaveLift = true;
        else me.data.HaveLift = false;
        if ($('#IsActive').is(':checked')) me.data.IsActive = true;
        else me.data.IsActive = false;
        $http.post('sv.api/stall/save', me.data).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                me.startEditing();

            } else {
                MsgBox(data.message, MSG_ERROR);
            }
     })
     .error(function (e) {
         console.log(e);
         MsgBox('Terdapat kesalahan proses data. Please contact sdms support...', MSG_INFO);
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
                        MsgBox('Terdapat kesalahan proses data. Please contact sdms support...', MSG_INFO);
                    });
            }
            else return;
        });
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

    $("#HaveLift").on('change', function () {
        if ($('#HaveLift').is(':checked')) {
            me.data.HaveLift = true;
        }
        else {
            me.data.HaveLift = false;
        }
        me.Apply();
    });

    $("#IsActive").on('change', function () {
        if ($('#IsActive').is(':checked')) {
            me.data.IsActive = true;
        }
        else {
            me.data.IsActive = false;
        }
        me.Apply();
    });

    me.initialize = function () {
        $('#StallCode').removeAttr('disabled');
        $('#HaveLift').attr('checked', true);
        $('#IsActive').attr('checked', true);
        me.isPrintAvailable = me.isLoadData = true;
        me.Apply();
    }

    $("#StallCode").on('blur', function () {
        $http.post('sv.api/stall/get', me.data).
        success(function (data, status, headers, cinfig) {
            if (data.success) {
                me.data = data.data;
                
                $('#StallCode').attr('disabled', 'disabled');
                
                if (me.data.HaveLift == true) $('#HaveLift').attr('checked', true); 
                else {
                    $('#HaveLift').removeAttr('checked');
                }
                if (me.data.IsActive == true) $('#IsActive').attr('checked', true);
                else {
                    $('#IsActive').removeAttr('checked');
                }
                me.Apply();

                me.isLoadData = true;

                me.hasChanged = false;
                me.startEditing();
                me.isSave = false;
                $scope.$apply();
            }
        });
    });

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Stall",
        xtype: "panels",
        toolbars: 
             [{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    //{ name: "btnEdit",   text: "Edit",   cls:"btn btn-primary",    icon: "icon-edit",   show: "isLoadData && !isSave", click: "allowEdit()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" }],

        panels: [
            {
                name: "pnlStall",
                items: [
                    { name: "StallCode", text: "Kode Stall", cls: "span4 full", required: true, validasi:"required", maxlength: 15 },
                    { name: "Description", text: "Keterangan", cls: "span4 full", required: true, validasi: "required", maxlength: 100 },
                    { name: "HaveLift", text: "Ada Lift", cls: "span4 full", style: "margin-bottom:15px", type: "check" },
                    { name: "IsActive", text: "Aktif", cls: "span4", type: "check" },
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