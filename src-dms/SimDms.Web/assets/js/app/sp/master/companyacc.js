var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spCompanyAccController($scope,$http,$injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboTPGO = data;          
    });  

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "companyACCBrowse",
            title: "Company Account Browse",
            manager: spManager,
            query: "CompanyaccBrowse",
            defaultSort: "CompanyCode asc",
            columns: [
            { field: "CompanyCode", title: "Company Code" },
            { field: "CompanyCodeTo", title: "CompanyCode To" },
            { field: "WarehouseCodeTo", title: "WarehouseCode To" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.isSave = false;
                me.Apply();
                me.loadDetail(data);

                $('#TPGO').removeAttr('disabled');
                $('#btnAccountNo').removeAttr('disabled');
            }
        });
    }

    me.loadDetail = function (data) {
       
        $http.post('sp.api/Companyacc/CompanyaccDetailsLoad?strCompanyCodeTo=' + data.CompanyCodeTo).
           success(function (data, status, headers, config) {             
                   me.grid.detail = data;
                   me.loadTableData(me.grid1, me.grid.detail);
           }).
           error(function (e, status, headers, config) {
               //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               console.log(e); 
           });
    }
    

    me.InterCompanyAccNoTo = function () {
        var lookup = Wx.blookup({
            name: "companyACCBrowse",
            title: "Lookup Account Code",
            manager: spManager,
            query: "glaccbrowse",
            defaultSort: "AccountNo asc",
            columns: [
            { field: "AccountNo", title: "Account No" },
            { field: "Description", title: "Description" } 
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.AccountNo = data.AccountNo;
                me.detail.AccountName = data.Description;
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.browseCompanyCodeTo = function () {
        var lookup = Wx.blookup({
            name: "mstrCompanyCodeTo",
            title: "Company Lookup",
            manager: spManager,
            query: "ShowCompanyLookup",
            defaultSort: "CompanyCode asc",
            columns: [
            { field: "CompanyCode", title: "CompanyCode" },
            { field: "CompanyName", title: "CompanyName" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CompanyCodeTo = data.CompanyCode;
                me.data.CompanyCodeToDesc = data.CompanyName;
                me.isSave = false;
                me.Apply();
            }
        });
    }

 
    me.browseBranchCodeTo = function () {
        var lookup = Wx.blookup({
            name: "mstrBranchCodeTo",
            title: "Branch Lookup",
            manager: spManager,
            query: "ShowBranchLookup",
            defaultSort: "BranchCode asc",
            columns: [
            { field: "BranchCode", title: "BranchCode" },
            { field: "CompanyName", title: "CompanyName" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.BranchCodeTo = data.BranchCode;
                me.data.BranchCodeToDesc = data.CompanyName;                
                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.browseWarehouse = function () {
        var lookup = Wx.blookup({
            name: "mstrWarehouse",
            title: "Warehouse Lookup",
            manager: spManager,
            query: "ShowWarehouseLookup",
            defaultSort: "LookUpValue asc",
            columns: [
            { field: "LookUpValue", title: "Warehouse Code" },
            { field: "LookUpValueName", title: "Warehouse Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.WarehouseCodeTo = data.LookUpValue;
                me.data.WarehouseCodeToDesc = data.LookUpValueName;
                me.isSave = true;
                me.Apply();
            }
        });
    }

    me.saveData = function (e, param) {
        $http.post('sp.api/Companyacc/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    $('#TPGO').removeAttr('disabled');
                    $('#btnAccountNo').removeAttr('disabled');
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e); 
            });
    }


    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/Companyacc/delete', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        Wx.Info("Record has been deleted...");
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                   // MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    console.log(e); 
                });
            }
        });
    }

    me.delete2 = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                me.LinkDetail();
                $http.post('sp.api/Companyacc/delete2', me.detail).
                success(function (dl, status, headers, config) {
                    if (dl.success) {

                        me.detail = {};
                        me.clearTable(me.grid1);
                        Wx.Info("Record has been deleted..."); 
                       
                        me.grid.detail = dl.result;
                        me.loadTableData (me.grid1, me.grid.detail); 

                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                    console.log(e); 
                });
            }
        });
    }

    me.CloseModel = function()
    {
        me.detail = {};
        me.grid1.clearSelection();
    }

    me.initGrid = function () {
        me.grid1 = new webix.ui({
            container: "wxsalestarget",
            view: "wxtable", css:"alternating", scrollX: true,
            columns: [
                { id: "TPGOName", header: "TPGO", width: 100 },
                { id: "AccountNo", header: "Account No", width: 250 },
                { id: "AccountName", header: "AccountName", width: 800 }
            ],
            on: {
                onSelectChange: function () {
                    if (me.grid1.getSelectedId() !== undefined) 
                    {
                        me.detail = this.getItem(me.grid1.getSelectedId().id);
                        me.detail.oid = me.grid1.getSelectedId();
                        me.Apply();
                    }
                }
            }
        });
    }


    me.LinkDetail = function () {
        me.detail.CompanyCodeTo = me.data.CompanyCodeTo;    
    }

    me.AddEditModel = function () {

        if (me.detail.TPGO === undefined || me.detail.TPGO == null) {
            MsgBox("TPGO is required!!!", MSG_ERROR);
            return;
        }

        me.LinkDetail();

        $http.post('sp.api/Companyacc/save2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success(data.message);
                    me.detail = {};
                    me.clearTable(me.grid1);
                    me.grid.detail = data.result;
                    me.loadTableData(me.grid1, me.grid.detail);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                //MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(e); 
            });
    }

    me.initialize = function () {
        me.clearTable(me.grid1);
        me.detail = {};
        me.data.WarehouseCodeTo = '00';
        $('#TPGO').attr('disabled', 'disabled');
        $('#btnAccountNo').attr('disabled', 'disabled');

        $http.post('gn.api/masteritem/GetLookupValueName?VarGroup=WRCD&varCode=' + me.data.WarehouseCodeTo).
            success(function (v, status, headers, config) {
                if (v != "") {
                    me.data.WarehouseCodeToDesc = v;
                }
            });
    }

    $('#CompanyCodeTo').blur(function () {
        if ($('#CompanyCodeTo').val() || $('#CompanyCodeTo').val() != '') {
            var url = "gn.api/Companyacc/getThisRecord";
            $http.post(url, { CompanyCodeTo: me.data.CompanyCodeTo }).
            success(function (data, status, headers, config) {
                if (data.success) {
                    //me.data.CompanyCodeTo = data.data.CompanyCode;
                    me.data = data.data;
                    me.loadDetail(data.data);
                    $('#CompanyCodeTo').attr('disabled', 'disabled');
                    $('#CompanyCodeToDesc').attr('disabled', 'disabled');
                    $('#BranchCodeTo').attr('disabled', 'disabled');
                    $('#BranchCodeToDesc').attr('disabled', 'disabled');
                    $('#TPGO').removeAttr('disabled');
                    $('#btnAccountNo').removeAttr('disabled');
                } else {
                    console.log(data.success);
                }
            });
        } else {
            me.data.CompanyCodeTo = '';
            me.data.CompanyCodeToDesc = '';
            me.browseCompanyCodeTo();
        }
    });

    $("[name = 'AccountNo']").on('blur', function () {
        if ($('#AccountNo').val() || $('#AccountNo').val() != '') {
            var SegAccNo = $('#AccountNo').val();
            $http.post('gn.api/chart/CheckAccNo?AccountNo=' + SegAccNo).
            success(function (v, status, headers, config) {
                if (!v.success) {
                    $('#AccountNo').val('');
                    me.detail.AccountName = '';
                    me.InterCompanyAccNoTo();
                } else {
                    //alert(v.data.Description);
                    me.detail.AccountName = v.data.Description;
                }
            });
        } else {
            $('#AccountNo').val('');
            me.data.AccountName = '';
            me.InterCompanyAccNoTo();
        }
    });

    me.initGrid();

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    me.start();

    //$("[name = 'BranchCodeTo']").on('blur', function () {
    //    if ($('#BranchCodeTo').val() || $('#BranchCodeTo').val() != '') {
    //        $http.post('gn.api/Lookup/getCompanyCode?branchCode=' + $('#BranchCodeTo').val()).
    //        success(function (v, status, headers, config) {
    //            if (v.TitleName != '') {
    //                me.data.BranchCodeToDesc = v.CompanyName;
    //            } else {
    //                $('#BranchCodeTo').val('');
    //                $('#BranchCodeToDesc').val('');
    //                me.browseBranchCodeTo();
    //            }
    //        });
    //    } else {
    //        me.data.BranchCodeToDesc = '';
    //        me.browseBranchCodeTo();
    //    }
    //});

    //$("[name = 'WarehouseCodeTo']").on('blur', function () {
    //    if ($('#WarehouseCodeTo').val() || $('#WarehouseCodeTo').val() != '') {
    //        $http.post('gn.api/masteritem/GetLookupValueName?VarGroup=WRCD&varCode=' + $('#WarehouseCodeTo').val()).// 2parameter
    //        success(function (v, status, headers, config) {
    //            if (v != "") {
    //                me.data.WarehouseCodeToDesc = v;
    //            } else {
    //                $('#WarehouseCodeTo').val('');
    //                $('#WarehouseCodeToDesc').val('');
    //                me.browseWarehouse();
    //            }
    //        });
    //    } else {
    //        me.data.WarehouseCodeTo = '';
    //        me.data.WarehouseCodeToDesc = '';
    //        me.browseWarehouse();
    //    }
    //});
}


$(document).ready(function () {
    var options = {
        title: "Master Company Account",
        xtype: "panels",
        toolbars:WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
 
                        { name: "CompanyCodeTo", text: "Company Code To", cls: "span3 ", type: "text", btnName: "btnCompanyCodeTo", click: "browseCompanyCodeTo()", required: true, validasi: "required" },
                        { name: "CompanyCodeToDesc", text: "Company Desc", cls: "span5  "}, //required: true, validasi: "required" },
                        { name: "BranchCodeTo", text: "Branch Code To", cls: "span3  ", type: "text", btnName: "btnBranchCodeTo", click: "browseBranchCodeTo()",required: true, validasi: "required"  },
                        { name: "BranchCodeToDesc", text: "Branch Desc", cls: "span5  " },
                        { name: "WarehouseCodeTo", text: "Warehouse Code To", cls: "span3  ", type: "text", btnName: "btnWarehouseCodeTo", click:"browseWarehouse()", readonly : true },
                        { name: "WarehouseCodeToDesc", text: "Warehouse Desc", cls: "span5  ", readonly: true },
                        { name: "UrlAddress", text: "Url Address", cls: "span8" },
                        { name: "isActive", type: "x-switch", text: "Is Active", cls: "span3", float: "left" },                        
                        
                        ]   
            },
            {
                name: "pnlB",
              
                title: "Account Intercompany",
                items: [
                        { name: "TPGO", text: "Type Of Goods", model: "detail.TPGO", type: "select2", cls: "span4", datasource: "comboTPGO"},
                        {
                            text: "Account Code",
                            type: "controls",

                            items: [
                                { name: "AccountNo", model: "detail.AccountNo", cls: "span2", text: "InterCompanyAccNoTo", type: "popup", btnName: "btnInterCompanyAccNoTo", click: "InterCompanyAccNoTo()" },
                                { name: "AccountName", model: "detail.AccountName", cls: "span6", text: "InterCompanyAccNoToName", readonly: true }
                            ]
                        },

                    {
                        type: "buttons",
                        items: [
                                {name: "btnAddModel", text: "Add", icon: "icon-plus", cls:"btn btn-info", click: "AddEditModel()", show: "detail.oid === undefined", disable:"detail.AccountNo === undefined"},
                                {name: "btnUpdateModel", text: "Update", icon: "icon-save", cls:"btn btn-success", click: "AddEditModel()", show: "detail.oid !== undefined" },
                                {name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls:"btn btn-danger", click: "delete2()", show: "detail.oid !== undefined"},
                                {name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls:"btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined"}
                            ]
                    },



                ]
            },
            {
                name: "wxsalestarget",
                xtype: "wxtable",
            },
        ]
    };
  





 




  
 

 

    //widget.onTableClick(function (icon, row, selector) {
    //    switch (selector.selector) {
    //        case "#tblCompanyAccDetail":
    //            switch (icon) {
    //                case "edit":
    //                    editDetail(row);
    //                    break;
    //                case "trash":
    //                    deleteDetail(row);
    //                    break;
    //                default:
    //                    break;
    //            }
    //            break;
    //        default: break;
    //    }

    //});

    

    // function getTable() {
    //     var param = $(".main .gl-widget").serializeObject();
       
    //     widget.post("sp.api/Companyacc/CompanyaccDetailsLoad?strCompanyCodeTo=" + $("#CompanyCodeTo").val(), param, function (result) {
    //    
    //         widget.populateTable({ selector: "#tblCompanyAccDetail", data: result });

    //     });
    // }

    // function editDetail(row) {
    //     $("#btnEdt").addClass("hide", "hide");
    //     var data = {
    //         TPGO: row[1],
    //         //TPGO: row[2],
    //         AccountNo: row[3],
    //         AccountName: row[4],

    //     }
       
    //     //$("#TPGO").val(data.TPGOID);
    //     $("#InterCompanyAccNoTo").val(data.AccountNo);
    //     $("#InterCompanyAccNoToName").val(data.AccountName);
 
    //     widget.populate(data, "#pnlB");
    // }

 


 
 

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spCompanyAccController");
    }



});




