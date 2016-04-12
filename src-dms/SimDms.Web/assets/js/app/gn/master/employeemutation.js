var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    $http.post('sp.api/combo/LoadLookup?CodeId=PERS').
            success(function (data, status, headers, config) {
                me.Personnels = data;
            });

    me.Branch = function () {
        var lookup = Wx.blookup({
            name: "BranchLookUp",
            title: "Lookup Branch",
            manager: gnManager,
            query: "AllBranch",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Branch Code" },
                { field: "CompanyName", title: "Branch Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.MutationTo = data.BranchCode;
                me.data.CompanyName = data.CompanyName;
                me.Apply();
            }
        });
    };

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/ZipCode/Delete', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.init();
                    Wx.Success("Data zip code deleted...");
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
        $http.post('gn.api/Employee/Save2', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    localStorage.setItem("CloseInterval", true);
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.initialize = function () {
        me.data = {};
        //$('#Browse').prop('disabled', true);
        $('#browse').prop('disabled', true);
        //var EmployeeID = localStorage.getItem('EmployeeID');
        //var EmployeeName = localStorage.getItem('EmployeeName');
        me.data.EmployeeID = localStorage.getItem('EmployeeID');
        me.data.EmployeeName = localStorage.getItem('EmployeeName');
        me.data.BranchCode = localStorage.getItem('BranchCode');
        me.hasChanged = false;
    }

    $("[name = 'MutationTo']").on('blur', function () {
        if ($('#MutationTo').val() || $('#MutationTo').val() != '') {
            var BranchCode = $('#MutationTo').val();
            $http.post('gn.api/Lookup/getCompanyCode?BranchCode=' + BranchCode).
            success(function (v, status, headers, config) {
                if (v.CompanyName != '') {
                    me.data.CompanyName = v.CompanyName;
                } else {
                    $('#MutationTo').val('');
                    $('#CompanyName').val('');
                    me.Branch();
                }
            });
        } else { me.Branch(); }
    });

    me.start();
}
$(document).ready(function () {
    var options = {
        title: "Employee Mutation",
        xtype: "panels",
        toolbars: [{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "isInitialize", click: "browse()" }],
        panels: [
            {
                name: "pnlInfo",
                title: "Employee Information",
                items: [
					{ name: "BranchCode", type: "hidden", text: "", cls: "span8" },
                     {
                         type: "controls", text: "Employee", cls: "span8", items: [
                             { name: "EmployeeID", model: "data.EmployeeID", text: "Employee ID", cls: "span4", readonly: true },
                             { name: "EmployeeName", model: "data.EmployeeName", text: "Employee Name", cls: "span4", readonly: true }
                         ]
                     },
                      {
                          type: "controls", text: "Mutation to", required: true,
                          items: [
                               { name: 'MutationTo', model: 'data.MutationTo', type: 'popup', text: 'Branch Code', cls: 'span4', required: true, validasi: "required", click: 'Branch()' },
                               { name: 'CompanyName', model: 'data.CompanyName', type: 'text', text: 'Branch Name', cls: 'span4', readonly: true }
                          ]
                      },
                     { name: "PersonnelStatus", model: "data.PersonnelStatus", type: "select2", text: "Personnel Status", cls: "span4 full", datasource: "Personnels" },
                      {
                          type: "buttons",
                          cls: "span4",
                          items: [
                                  { name: "btnSave", text: "Save", icon: "icon-plus", cls: "btn btn-info", click: "save()" },
                                  //{ name: "btnCancel", text: "Cancel", icon: "icon-plus", cls: "button small btn btn-danger", click: "cancel()" }
                          ]
                      },
                ]
            },

        ]
    };
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spItemPriceController");
    }
});