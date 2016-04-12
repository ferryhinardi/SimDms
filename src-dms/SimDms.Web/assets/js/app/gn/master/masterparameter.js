var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterTaxController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data = {};

         $http.get('breeze/gnMaster/Parameters').
           success(function (data, status, headers, config) {
               me.data.DbName = data.DbName;
               me.data.Extensions = data.Extensions;
               me.data.Prefix = data.Prefix;
               me.data.FolderPath = data.FolderPath;
               me.data.DcsUrl = data.DcsUrl;
               me.data.TaxUrl = data.TaxUrl;
           }).
          error(function (data, status, headers, config) {
              MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
          });
        
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/Message/Delete', me.data).
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
        });
    }

    me.saveData = function (e, param) {

        $http.post('gn.api/Message/Save', me.data).
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
        title: "Master Parameter",
        xtype: "panels",
        toolbars: "",
        panels: [
            {
                name: "MessageBoards",
                title: "Backup Data Base",
                items: [
                    { name: "DbName", model: "data.DbName", type: "text", text: "DB Name", cls: "span8",  disable :true },
                    { name: "Extensions", model: "data.Extensions", type: "text", text: "Extensions", cls: "span5", disable: true },
                    { name: "Prefix", model: "data.Prefix", type: "text", text: "Prefix", cls: "span3", disable: true },
                    { name: "FolderPath", model: "data.FolderPath", type: "text", text: "Folder Path", cls: "span8", disable: true },
                ]
            },
            {
                name: "MessageBoards",
                title: "Web Service",
                items: [
                    { name: "DcsUrl", model: "data.DcsUrl", type: "text", text: "DCS URL", cls: "span8", disable: true },
                    { name: "TaxUrl", model: "data.TaxUrl", type: "text", text: "TAX URL", cls: "span8", disable: true },
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