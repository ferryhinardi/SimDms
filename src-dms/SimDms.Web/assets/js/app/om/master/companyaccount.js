var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterCompanyAccountController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "CompanyAccountBrowse",
            title: "CompanyAccount Browse",
            manager: spSalesManager,
            query: "CompanyAccountBrowse",
            defaultSort: "CompanyCode asc",
            columns: [
                { field: "CompanyCode", title: "Company Code" },
                { field: "BranchCodeTo", title: "Branch Code To" },
                { field: "WarehouseCodeTo", title: "Warehouse Code To" },
                { field: "Status", title: "Status" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.data.isActive = result.isActive == false ? "0" : "1";
                me.isSave = false;
                me.Apply();

            }

        });
    }

    me.InterCompanyAccNoToLookup = function () {
        var lookup = Wx.blookup({
            name: "AccountNoLookup",
            title: "Account",
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
                me.data.InterCompanyAccNoTo = data.AccountNo;
                me.data.InterCompanyAccNoToDesc = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.initialize = function () {
        me.hasChanged = false;
        me.data.isActive = "1";
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstCompanyAccount/Delete', me.data).
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
        if (me.data.isActive == "1") { me.data.isActive = true } else { me.data.isActive = false}
        $http.post('om.api/MstCompanyAccount/Save', me.data).
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

    $("[name='InterCompanyAccNoTo']").on('blur', function () {
        if (me.data.InterCompanyAccNoTo != null) {
            $http.post('om.api/MstCompanyAccount/AccNo', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.InterCompanyAccNoToDesc = data.data.Description;
                       $('#InterCompanyAccNoTo').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.InterCompanyAccNoTo = "";
                       me.data.InterCompanyAccNoToDesc = "";
                       me.InterCompanyAccNoToLookup();
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
        title: "Company Account",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "CompanyAccount",
                title: "Company Account",
                items: [
                        { name: "CompanyCodeTo", text: "Company Code To", cls: "span4", required: true, validasi: "required", maxlength:15 },
                        { name: "CompanyCodeToDesc", text: "Description", cls: "span4", maxlength:100 },
                        { name: "BranchCodeTo", text: "Branch Code To", cls: "span4", required: true, validasi: "required", maxlength:15 },
                        { name: "BranchCodeToDesc", text: "Description", cls: "span4", maxlength:100 },
                        { name: "WarehouseCodeTo", text: "Warehouse Code To", cls: "span4", required: true, validasi: "required", maxlength:15 },
                        { name: "WarehouseCodeToDesc", text: "Description", cls: "span4", maxlength:15 },
                        {
                            text: "Inter Company Acc No To",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "InterCompanyAccNoTo", cls: "span4 full", placeHolder: "InterCompanyAccNoTo", type: "popup", btnName: "btnInterCompanyAccNoTo", click: "InterCompanyAccNoToLookup()", required: true },
                                { name: "InterCompanyAccNoToDesc ", cls: "span8", placeHolder: "InterCompanyAccNoToDesc", model: "data.InterCompanyAccNoToDesc", readonly: true },
                            ]
                        },
                        { name: "UrlAddress", text: "Alamat URL", cls: "span8", maxlength: 100, required: true, validasi: "required" },
                        {
                            type: "optionbuttons", name: "isActive", model: "data.isActive",
                            items: [
                                { name: "1", text: "Aktif" },
                                { name: "0", text: "Tidak Aktif" },
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
        SimDms.Angular("gnMasterCompanyAccountController");
    }

});