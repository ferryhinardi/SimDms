var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterSalesTargetController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SalesTargetBrowse",
            title: "Sales Target Browse",
            manager: spSalesManager,
            query: "SalesTargetBrowse",
            defaultSort: "DealerCode asc",
            columns: [
                { field: "DealerCode", title: "Dealer Code" },
                { field: "OutletCode", title: "Outlet Code" },
                { field: "Year", title: "Year" },
                { field: "Month", title: "Month" },
                { field: "SalesmanID", title: "Salesman ID" },
                { field: "MarketModel", title: "Market Model" },
                { field: "TargetUnit", title: "Target Unit" },
                //{ field: "isActive", title: "Status" },
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.isSave = false;
                me.Apply();
                $http.get('breeze/sales/CurrentUserInfo').
                success(function (dl, status, headers, config) {
                me.data.DealerCode = dl.CompanyCode;
                me.data.OutletCode = dl.BranchCode;
                me.data.CompanyName = dl.CompanyGovName;
                me.data.BranchName = dl.CompanyName;
                me.Apply();
                
                $http.post("om.api/MstSalesTarget/SalesTarget", me.data).
                success(function (data, status, headers, config) {
                   me.loadTableData(me.grid1, data);
                   me.Apply();
                });

        });

            }

        });
    }

    me.DealerCode = function () {
        var lookup = Wx.blookup({
            name: "DealerCodeLookup",
            title: "Dealer Code",
            manager: spSalesManager,
            query: "DealerCodeLookup",
            defaultSort: "CompanyCode asc",
            columns: [
                { field: "CompanyCode", title: "Company Code" },
                { field: "CompanyName", title: "Company Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.DealerCode = data.CompanyCode;
                me.data.CompanyName = data.CompanyName;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.OutLetCode = function () {
        var lookup = Wx.blookup({
            name: "OutLetLookup",
            title: "OutLet",
            manager: spSalesManager,
            query: "OutLetLookup",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Branch Code" },
                { field: "BranchName", title: "Branch Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.OutletCode = data.BranchCode;
                me.data.BranchName = data.BranchName;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.SalesmanID = function () {
        var lookup = Wx.blookup({
            name: "SalesmanIDLookup",
            title: "Salesman ID",
            manager: spSalesManager,
            query: "SalesmanIDLookup",
            defaultSort: "EmployeeID asc",
            columns: [
                { field: "EmployeeID", title: "Employee ID" },
                { field: "EmployeeName", title: "Employee Name" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SalesmanID = data.EmployeeID;
                me.data.EmployeeName = data.EmployeeName;
                me.isSave = false;
                me.Apply();
                $('#SalesmanID').attr('disabled', 'disabled');
            }
        });

    }

    me.MarketModel = function () {
        var lookup = Wx.blookup({
            name: "MarketModelLookup",
            title: "Market Model",
            manager: spSalesManager,
            query: "MarketModelLookup",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Tipe No." },
                { field: "LookUpValueName", title: "Market Model" },
            ],
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.MarketModel = data.LookUpValueName;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    $http.post('om.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.Month = data;
    });

    $http.post('om.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.Year = data;
    });

    me.initialize = function () {
        var date = new Date;
        //me.hasChanged = false;
        //me.isSave = false;
        me.data.isActive = true;
        $('#SalesmanID').removeAttr('disabled');
        $('#MarketModel').attr('disabled', 'disabled');
        me.data.Month = date.getMonth() + 1;
        me.data.Year = date.getFullYear();
        $http.get('breeze/sales/CurrentUserInfo').
        success(function (dl, status, headers, config) {
            me.data.DealerCode = dl.CompanyCode;
            me.data.OutletCode = dl.BranchCode;
            me.data.CompanyName = dl.CompanyGovName;
            me.data.BranchName = dl.CompanyName;

      });
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('om.api/MstSalesTarget/Delete', me.data).
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
        $http.post('om.api/MstSalesTarget/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    me.clearTable(me.grid1);
                    $http.post("om.api/MstSalesTarget/SalesTarget", me.data).
                    success(function (data, status, headers, config) {
                        me.loadTableData(me.grid1, data);
                        me.Apply();
                    });
                    me.initialize();

                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e);
            });
    }

    me.printPreview = function () {
        Wx.loadForm();
        Wx.showForm({ url: "om/master/PrintSalesTarget" });
    }

    $("[name='SalesmanID']").on('blur', function () {
        if (me.data.SalesmanID != null) {
            $http.post('om.api/MstSalesTarget/SalesmanID', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.EmployeeName = data.data.EmployeeName;
                       $('#SalesmanID').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.SalesmanID = "";
                       me.data.SupplierName = "";
                       me.SalesmanID();
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    $("[name='SalesmanID']").on('blur', function () {
        if (me.data.SalesmanID != null) {
            $http.post('om.api/MstSalesTarget/SalesmanID', me.data).
               success(function (data, status, headers, config) {
                   //me.data = data.data;
                   if (data.success) {
                       me.data.EmployeeName = data.data.EmployeeName;
                       $('#SalesmanID').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.SalesmanID = "";
                       me.data.SupplierName = "";
                       me.SalesmanID();
                   }
               }).
               error(function (data, status, headers, config) {
                   //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   alert('error');
               });
        }
    });

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxData",
            view: "wxtable", css:"alternating",
            columns: [
                { id: "DealerCode", header: "Dealer" },
                { id: "OutletCode", header: "Outlet", fillspace: true },
                { id: "Year", header: "Year", fillspace: true },
                { id: "Month", header: "Month", fillspace: true },
                { id: "SalesmanName", header: "Sales Name", fillspace: true },
                { id: "MarketModel", header: "Market Model", fillspace: true },
                { id: "TargetUnit", header: "Target Unit", fillspace: true },
                { id: "isActive", header: "Status", fillspace: true, template: "{common.checkbox()}" },
            ]
        });
    }

    me.initGrid();
    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "Sales Target",
        xtype: "panels",
        //toolbars: WxButtons,
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", disable: "!isPrintEnable" },
        ],
        panels: [
            {
                name: "SalesTarget",
                title: "Filter",
                items: [
                        {
                            text: "Dealer",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "DealerCode", cls: "span2", placeHolder: "DealerCode" , type: "popup", btnName: "btnDealerCode", click: "DealerCode()", required: true, validasi: "required", readonly: true},
                                { name: "CompanyName ", cls: "span4", placeHolder: "CompanyName", model: "data.CompanyName", readonly: true },
                            ]
                        },
                        {
                            text: "OutLet",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "OutletCode", cls: "span2", placeHolder: "OutletCode", type: "popup", btnName: "btnOutLetCode", click: "OutLetCode()", required: true, readonly: true },
                                { name: "BranchName ", cls: "span4", placeHolder: "BranchName", model: "data.BranchName", readonly: true },
                            ]
                        },
                        {
                            text: "Periode",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "Month", placeHolder: "Month", cls: "span2", type: "select2", datasource: "Month", opt_text: "MONTH?" },
                                { name: "Year", placeHolder: "Year", cls: "span2", type: "select2", datasource: "Year", opt_text: "YEAR?"},
                            ]
                        },
                        {
                            text: "Salesman",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "SalesmanID", cls: "span2", placeHolder: "SalesmanID", type: "popup", btnName: "btnSalesmanID", click: "SalesmanID()", required: true, validasi: "required" },
                                { name: "EmployeeName ", cls: "span4", placeHolder: "EmployeeName", model: "data.EmployeeName" },
                            ]
                        },
                        {
                            text: "Market Model (Yype)",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "MarketModel", cls: "span4", placeHolder: "MarketModel", type: "popup", btnName: "btnMarketModel", click: "MarketModel()", required: true },
                            ]
                        },
                        { name: "TargetUnit ", cls: "span3 full", text: "Target Unit" },
                        { name: "isActive", text: "Status", type: "x-switch", cls: "span2" },
                         
                ]
            },
            {
                name: "wxData",
                xtype: "wxtable",
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterSalesTargetController");
    }

});