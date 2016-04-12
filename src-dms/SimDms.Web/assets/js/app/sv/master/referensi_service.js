"use strict"

function svMstReffSrvController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/Combo/ListLookupDtlByCodeID?CodeID=RFTP').
      success(function (data, status, headers, config) {
          me.ReffSrv = data;
      });

    me.CekData = function () {
        if (me.data.RefferenceType != "") {
            $http.post('sv.api/refservice/get', me.data).success(function (data, status, headers, confiq) {
                if (data.success) {
                    me.lookupAfterSelect(data.data);
                    $('#RefferenceType, #RefferenceCode').attr('disabled', 'disabled');
                    me.isSave = false;
                    if (me.data.IsActive == true) $('#IsActive').attr('checked', true);
                    else {
                        $('#IsActive').removeAttr('checked');
                    }
                    me.isPrintAvailable = me.isLoadData = true;
                    me.Apply();
                }
            }).error(function (e) {
                console.log(e);
                MsgBox('Terdapat kesalahan proses data. Please contact sdms support...', MSG_INFO);
            });
        }
    }

    me.RefferenceCode = function () {
        var lookup = Wx.klookup({
            name: "ReffService",
            title: "Refference Service",
            url: "sv.api/grid/reffcode",
            params: { refcode: me.data.RefferenceType },
            serverBinding: true,
            columns: [
                { field: "RefferenceCode", title: "Refference Code" },
                { field: "Description", title: "Description" },
                { field: "DescriptionEng", title: "Description (Eng.)" },
                { field: "IsActiveDesc", title: "Is Active" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.RefferenceCode = data.RefferenceCode;
                me.data.Description = data.Description;
                me.data.DescriptionEng = data.DescriptionEng;

                me.CekData();
            }
        });
    }

    me.saveData = function () {
        if ($('#IsActive').is(':checked')) me.data.IsActive = true;
        else me.data.IsActive = false;
        $http.post("sv.api/refservice/save", me.data).success(function (data, status, headers, config) {
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
                $http.post('sv.api/refservice/deletedata', me.data)
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

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "ReffService",
            title: "Tipe Refference",
            manager: MasterService,
            query: "ReffService",
            columns: [
                { field: "LookupValueName", title: "Tipe Refference" },
                { field: "RefferenceCode", title: "Kode Refference" },
                { field: "Description", title: "Keterangan" },
                { field: "DescriptionEng", title: "Keterangan (Eng.)" },
                { field: "IsActiveDesc", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            $('#RefferenceType, #RefferenceCode').attr('disabled', 'disabled');
            me.isSave = false;
            if (me.data.IsActive == true) $('#IsActive').attr('checked', true);
            else {
                $('#IsActive').removeAttr('checked');
            }
            me.isPrintAvailable = me.isLoadData = true;
            me.Apply();
        });
    }

    $("#IsActive").on('change', function () {
        if ($('#IsActive').is(':checked')) {
            me.data.IsActive = true;
        }
        else {
            me.data.IsActive = false;
        }
        me.Apply();
    });

    me.printReffSrv = function () {
        Wx.loadForm();
        Wx.showForm({ url: "sv/master/reffsrvprint" });
    }

    me.initialize = function () {
        $('#IsActive').attr('checked', true);
        $('#RefferenceType, #RefferenceCode').removeAttr('disabled', 'disabled');
        me.isPrintAvailable = true;
        me.Apply();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Refference Service",
        xtype: "panels",
        toolbars:
             [{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printReffSrv()" }],
        panels: [
            {
                name: "pnlRefService",
                items: [

                    { name: "CompanyCode", cls: "hide" },
                    { name: "RefferenceType", text: "Tipe Refference", type: "select2", cls: "span4", required: true, validasi: "required", datasource: "ReffSrv" },
                    { name: "RefferenceCode", text: "Kode Refference", cls: "span4", placeholder: "Kode Referensi", type: "popup", btnName: "btnRefferenceCode",
                        required: true, validasi: "required", click: "RefferenceCode()", maxlength: 15 },
                    { name: "Description", text: "Keterangan", placeholder: "Keterangan", required: true, validasi: "required", maxlength: 100 },
                    { name: "DescriptionEng", text: "Keterangan (Eng.)", placeholder: "Keterangan", maxlength: 100 },
                    { name: "IsActive", text: "Status", cls: "span4", type: "check", float: "left" }
                ]
            },
            
        ],
    }
    
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstReffSrvController");
    }
    
});