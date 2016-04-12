var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterRefferenceController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "RefferenceBrowse",
            title: "Refference Browse",
            manager: spSalesManager,
            query: "RefferenceBrowse",
            defaultSort: "RefferenceType asc",
            columns: [
                { field: "RefferenceType", title: "Tipe Reff." },
                { field: "RefferenceCode", title: "Code Reff." },
                { field: "RefferenceDesc1", title: "Deskripsi" }
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                me.data.Status = result.Status == 0 ? false : true;
                me.Apply();
                $('#RefferenceType').attr('disabled', 'disabled');
                $('#btnRefferenceType').attr('disabled', 'disabled');

            }

        });
    }

    me.TipeReff = function () {
        var lookup = Wx.blookup({
            name: "RefferenceTypeLookup",
            title: "Tipe Referensi",
            manager: spSalesManager,
            query: "RefferenceTypeLookup",
            defaultSort: "RefferenceType asc",
            columns: [
                { field: "RefferenceType", title: "Tipe Reff." },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.RefferenceType = data.RefferenceType;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    $("[name='RefferenceType']").on('blur', function () {
        if (me.data.RefferenceType != null) {
            $http.post('om.api/MstRefference/tiperef', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                   }
                   else {
                       me.data.RefferenceType = "";
                       me.TipeReff();
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    me.initialize = function () {
        me.hasChanged = false;
        me.data.Status = true;
        $('#RefferenceType').removeAttr('disabled');
        $('#btnRefferenceType').removeAttr('disabled');
    }

    me.cancel = function () {
        me.initialize();
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstRefference/Delete', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.init();
                        Wx.Success("Data deleted...");
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.saveData = function (e, param) {
        $http.post('om.api/MstRefference/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e);
            });
    }

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Refference",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "Refference",
                title: "Refference",
                items: [
                        { name: "RefferenceType", cls: "span4", text: "Tipe Reff.", type: "popup", btnName: "btnTipeReff", click: "TipeReff()", required: true, validasi: "required", disable: "IsEditing() || testDisabled" },
                        { name: "RefferenceCode ", text: "Code Reff.", cls: "span6", required: true, validasi: "required", disable: "IsEditing() || testDisabled", maxlength: 15 },
                        { name: "RefferenceDesc1", text: "Deskripsi 1", cls: "span6", required: true, validasi: "required", maxlength: 100 },
                        { name: "RefferenceDesc2", text: "Deskripsi 2", cls: "span6", maxlength: 100 },
                        { name: "Remark", text: "Keterangan", cls: "span4", maxlength: 100 },
                        { name: "Status", text: "Status Active", type: "x-switch", cls: "span2" }

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterRefferenceController");
    }

});