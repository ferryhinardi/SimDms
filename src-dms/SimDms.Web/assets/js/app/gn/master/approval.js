var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterTaxController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MasterApproval",
            title: "Approval Browse",
            manager: gnManager,
            query: "ApprovalBrowse",
            defaultSort: "ApprovalNo asc",
            columns: [
                    { field: "ApprovalNo", title: "No.Approval", width: 150 },
                    { field: "SeqNo", title: "Deskripsi", width: 200 },
                    { field: "DocumentType", title: "Tipe Dokumen", width: 200 },
                    { field: "DocumentName", title: "Nama Dokumen", width: 150 },
                    { field: "UserID", title: "ID User", width: 200 },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                //me.GetCustomerInfo(data.CustomerCode);
                if (me.data.IsActive == true)
                    $('#IsActive').prop('checked', true);
                else $('#IsActive').prop('checked', false);
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.DocumentType = function () {
        var lookup = Wx.blookup({
            name: "MasterDocuments",
            title: "Documents Browse",
            manager: gnManager,
            query: "Documents",
            defaultSort: "DocumentType asc",
            columns: [
                    { field: "DocumentType", title: "Tipe Dokumen", width: 200 },
                    { field: "DocumentName", title: "Nama Dokumen", width: 150 },
                    { field: "DocumentPrefix", title: "Dokumen Prefix", width: 200 },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DocumentType = data.DocumentType;
                me.data.DocumentName = data.DocumentName;
                me.Apply();
            }
        });
    }

    me.UserID = function () {
        var lookup = Wx.blookup({
            name: "MasterUser",
            title: "User Browse",
            manager: gnManager,
            query: "AllUser",
            defaultSort: "UserId asc",
            columns: [
                    { field: "UserId", title: "ID User", width: 50 },
                    { field: "FullName", title: "Nama Dokumen", width: 150 },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.UserID = data.UserId;
                me.data.UserName = data.FullName;
                me.Apply();
            }
        });
    }

    me.initialize = function () {
        $('#ApprovalNo').css("text-align", "right");
        $('#SeqNo').css("text-align", "right");
        $('#IsActive').prop('checked', true);
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('gn.api/Approval/Delete', me.data).
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
        if ($('#IsActive').prop('checked') == true)
            me.data.IsActive = 1;
        else me.data.IsActive = 0;
        $http.post('gn.api/Approval/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    $("[name = 'DocumentType']").on('blur', function () {
        alert('d');
        if ($('#DocumentType').val() || $('#DocumentType').val() != '') {
            $http.post('gn.api/Lookup/documenttype?DocumentType=' + $('#DocumentType').val()).
        success(function (v, status, headers, config) {
            if (v.TitleName != '') {
                me.data.DocumentType = data.DocumentType;
                me.data.DocumentName = data.DocumentName;
            }
            else {
                $('#DocumentType').val('');
                me.DocumentType();
                me.Apply();
            }
        });
        }
    });

    $("[name = 'UserID']").on('blur', function () {
        alert('d');
        if ($('#UserID').val() || $('#UserID').val() != '') {
            $http.post('gn.api/Lookup/getuser?UserId=' + $('#UserID').val()).
        success(function (v, status, headers, config) {
            if (v.TitleName != '') {
                me.data.UserID = data.UserId;
                me.data.UserName = data.FullName;
            }
            else {
                $('#UserID').val('');
                me.UserID();
                me.Apply();
            }
        });
        }
    });
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Approval",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataCustomerClass",
                title: "Setting Approval",
                items: [
                    { name: "ApprovalNo", cls: "span3 full", type: "text", validasi: "required", placeHolder: "0", text: 'Approval No' },
                    { name: "SeqNo", cls: "span3 full", type: "text", validasi: "required", placeHolder: "0", text: 'Approval Seq'},
                     {
                         type: "controls",
                         cls: "span8 full",
                         text: "Tipe Dokumen",
                         required: true,
                         items: [
                             { name: 'DocumentType ', model: 'data.DocumentType', type: 'popup', text: '', cls: 'span3', click: 'DocumentType()', required: true, validasi: "required" },
                             { name: "DocumentName", cls: "span5", type: "text", readonly : true },
                         ]
                     },
                     {
                         type: "controls",
                         cls: "span8 full",
                         text: "User Id",
                         required: true,
                         items: [
                             { name: 'UserID', model: 'data.UserID', type: 'popup', text: '', cls: 'span3', click: 'UserID()', required: true, validasi: "required" },
                             { name: "UserName", cls: "span5", type: "text", readonly: true },
                         ]
                     },
                    {
                        type: "controls",
                        cls: "span4 full",
                        text: "",
                        items: [
                            { name: "IsActive", cls: "span1", type: "check" },
                            { type: "label", text: "Aktif?", cls: "span7 mylabel" },
                        ]
                    },
                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterTaxController");
    }




});