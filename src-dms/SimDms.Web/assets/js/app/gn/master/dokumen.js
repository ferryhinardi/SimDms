var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterDocumentController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('gn.api/combo/ProfitCenters').
    success(function (data, status, headers, config) {
        me.comboProfitCode = data;
    });

    $http.post('sp.api/combo/Years').
    success(function (data, status, headers, config) {
        me.Years = data;
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MasterDocumentBrowse",
            title: "Lookup Dokumen",
            manager: gnManager,
            query: "Documents",
            defaultSort: "DocumentType asc",
            columns: [
            { field: "DocumentType", title: "Tipe Dokumen" },
            { field: "DocumentPrefix", title: "Prefix Dokumen" },
            { field: "DocumentName", title: "Nama Dokumen" },
            { field: "ProfitCenterCode", title: "Profit Center" },
            { field: "DocumentYear", title: "Tahun Dokumen" },
            { field: "DocumentSequence", title: "No Dokumen Terakhir" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                //me.GetCustomerInfo(data.CustomerCode);
                me.isSave = false;
                me.Apply();

            }
        });
    }

    $("[name='DocumentType']").on('blur', function (result) {
        if (me.data.DocumentType != '') {
            $http.post('gn.api/Document/Get?documentType=' + $('#DocumentType').val()).success(function (data, status, headers, config) {
                if (data.result) {
                    me.lookupAfterSelect(data.data);
                    me.isSave = false;
                    //me.Apply();
                } else {
                    if (data.message != '') {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }
            });
        }
    });

    me.initialize = function () {
        me.hasChanged = false;
        me.DocumentType = me.DocumentPrefix = "";
        //me.data.ProfitCenterCode = "300";
        me.data.DocumentYear = new Date().getFullYear();
        me.data.DocumentSequence = "0";
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('gn.api/Document/Delete', me.data).
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

        $http.post('gn.api/Document/Save', me.data).
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

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Document",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataCustomerClass",
                title: "Document Setting",
                items: [

                    { name: "DocumentType", type: "text", text: "Tipe Dokumen", cls: "span4", disable: "IsEditing() || testDisabled", validasi: "required", maxlength: 15 },
                    { name: "DocumentPrefix", type: "text", text: "Prefix Dokumen", cls: "span4", validasi: "required", maxlength: 15 },
                    { name: "DocumentName", type: "text", text: "Nama Dokumen", cls: "span8", maxlength: 100 },
                    { name: "ProfitCenterCode", model: "data.ProfitCenterCode", type: "select2", text: "Profit Center", cls: "span8", datasource: "comboProfitCode" },
                    { name: "DocumentYear", model: "data.DocumentYear", type: "select2", text: "Tahun Dokument", cls: "span4", datasource: "Years" },
                    { name: "DocumentSequence", type: "int", text: "No Dokument Terakhir", cls: "span4", },

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterDocumentController");
    }




});