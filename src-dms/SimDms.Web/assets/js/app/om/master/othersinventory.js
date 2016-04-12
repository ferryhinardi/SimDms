var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterOthersInventoryController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "OthersInventoryBrowse",
            title: "Others Inventory Browse",
            manager: spSalesManager,
            query: "OthersInventoryBrowse",
            defaultSort: "OthersNonInventory asc",
            columns: [
                { field: "OthersNonInventory", title: "Code" },
                { field: "OthersNonInventoryDesc", title: "Descriptions" },
                { field: "OthersNonInventoryAccNo", title: "Account No" },
                { field: "Description", title: "Account Desc" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                //me.data.OthersNonInventoryAccNo = data.AccountNo;
                //me.data.OthersNonInventoryAccDesc = data.OthersNonInventoryAccDesc;
                //me.data.OthersNonInventoryAccNo = result.OthersNonInventoryAccNo;
                me.data.Descriptions = result.Description;
                me.isSave = false;
                me.Apply();

            }

        });
    }

    me.OthersInventoryAccNo = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account No",
            manager: spSalesManager,
            query: "AccountNoLookup",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "Account No" },
                { field: "Description", title: "Descriptions" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.OthersNonInventoryAccNo = data.AccountNo;
                me.data.Descriptions = data.Description;
                me.isSave = false;
                me.Apply();
                $('#InterCompanyAccNoTo').attr('disabled', 'disabled');
            }
        });

    }

    me.initialize = function () {
        me.hasChanged = false;
        me.data.IsActive = true;
        $('#OthersNonInventoryAccNo').removeAttr('disabled');
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstOthersInventory/Delete', me.data).
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
        $http.post('om.api/MstOthersInventory/Save', me.data).
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

    $("[name='OthersNonInventoryAccNo']").on('blur', function () {
        if (me.data.OthersNonInventoryAccNo != null) {
            $http.post('om.api/MstOthersInventory/AccNo', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.Descriptions = data.data.Description;
                       $('#InterCompanyAccNoTo').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.OthersNonInventoryAccNo = "";
                       me.data.Descriptions = "";
                       me.OthersInventoryAccNo();
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Master Other Inventory",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "MasterOthersInventory",
                title: "Master Other Inventory",
                items: [
                        { name: "OthersNonInventory ", text: "Code", cls: "span3 full", required: true, validasi: "required", disable: "IsEditing() || testDisabled", maxlength:15 },
                        { name: "OthersNonInventoryDesc", text: "Description", cls: "span6 full", maxlength: 50 },
                        {
                            text: "Account No.",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "OthersNonInventoryAccNo", cls: "span2", type: "popup", btnName: "btnOthersNonInventoryAccNo", click: "OthersInventoryAccNo()", required: true, validasi: "required" },
                                { name: "Descriptions ", cls: "span4", model: "data.Descriptions" },
                            ]
                        }, 
                        { name: "IsActive", text: "Status", type: "x-switch", cls: "span2" }

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterOthersInventoryController");
    }

});