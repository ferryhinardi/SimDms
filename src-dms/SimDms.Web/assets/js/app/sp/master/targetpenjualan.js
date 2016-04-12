var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function sptargetPenjualanController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/Years').
    success(function (data, status, headers, config) {
        me.comboYear = data;
    });

    $http.post('sp.api/Combo/Months').
    success(function (data, status, headers, config) {
        me.comboMonth = data;
    });

    me.allowAddEditTargetDetail = false;

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "targetPenjualanBrowse",
            title: "Sales Target Browse",
            manager: spManager,
            query: "TargetPenjualanBrowse",
            defaultSort: "Year asc",
            width: 380,
            columns: [
            { field: "Year", title: "Year", width: 100 },
            { field: "Month", title: "Month", width: 100 },
            { field: "QtyTarget", title: "Qty Target", template: '<div style="text-align:right;">#= kendo.toString(QtyTarget, "n0") #</div>', width: 180 }
            ]
        });

        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);   
                me.isSave = false;
                me.Apply();
                me.loadDetail(result);
            }
        });
    }

    me.loadDetail = function(data)
    {
        var p1 = me.where.create("Year", "eq",  parseInt(data.Year));
        var p2 = me.where.create("Month", "eq", parseInt(data.Month));
        var whereClause = p1.and(p2);

        var query = new breeze.EntityQuery()
            .from("SalesTargetDetail")
            .where(whereClause);
    
          spManager.executeQuery(query).then(function(data){
              me.UpdateGridDetail(data.results);
          }).fail(function(e) {
              MsgBox(e, MSG_ERROR);  
          });
    }

    me.CategoryCode = function () {
        var lookup = Wx.blookup({
            name: "categorycodeLookup",
            title: "Lookup Category Code",
            manager: spManager,
            query: "CategoryCodebrowse",
            defaultSort: "CategoryCode asc",
            columns: [
                { field: "CategoryCode", title: "Category Code" },
                { field: "CategoryName", title: "Category Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.CategoryCode = data.CategoryCode;
                me.detail.CategoryName = data.CategoryName;
                me.isSave = true;
                me.Apply();
            }
        });
    }

    me.saveData = function (e, param) {
        $http.post('sp.api/TargetPenjualan/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    me.loadDetail(me.data);
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.UpdateGridDetail = function(data)
    {
        me.grid.detail = data;     
        me.loadTableData(me.grid1, me.grid.detail); 
    }

    me.CloseModel = function()
    {
        me.detail = {};
        me.grid1.clearSelection();
    }

    me.AddEditModel = function () {

        if (me.detail.CategoryCode === undefined || me.detail.CategoryCode == null)
        {
            MsgBox("CategoryCode is required!!!", MSG_ERROR);
            return;
        }

        me.LinkDetail();
 
        $http.post('sp.api/TargetPenjualan/save2', me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");

                    me.UpdateGridDetail(data.data);     
                   
                    me.data.QtyTarget = data.total.QtyTarget;
                    me.data.AmountTarget = data.total.AmountTarget;
                    me.data.TotalJaringan = data.total.TotalJaringan;
                    me.startEditing();

                    me.detail = {};
                    me.detail.QtyTarget = 0;
                    me.detail.AmountTarget = 0;
                    me.detail.TotalJaringan = 0;
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
        me.ReformatNumber();
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/TargetPenjualan/delete', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        Wx.Info("Record has been deleted...");
                        me.init();                        
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox(e, MSG_ERROR);
                });
            }
        });
    }

    me.LinkDetail = function()
    {
        me.detail.Month = me.data.Month;
        me.detail.Year = me.data.Year;
    }

    me.delete2 = function () {

        MsgConfirm("Are you sure to delete current record?", function (result) {

            if (result) {

                me.LinkDetail();

                $http.post('sp.api/TargetPenjualan/delete2', me.detail).
                success(function (dl, status, headers, config) {
                    if (dl.success) {

                        me.detail = {} ;
                        Wx.Info("Record has been deleted...");

                        if (dl.count > 0) 
                            me.UpdateGridDetail(dl.data); 
                        else
                            me.clearTable(me.grid1);

                        me.data.QtyTarget = dl.total.QtyTarget;
                        me.data.AmountTarget = dl.total.AmountTarget;
                        me.data.TotalJaringan = dl.total.TotalJaringan;
                        me.ReformatNumber();
                        me.detail.QtyTarget = 0;
                        me.detail.AmountTarget = 0;
                        me.detail.TotalJaringan = 0;
                    } else {
                        MsgBox(dl.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    me.initialize = function()
    {
        me.clearTable(me.grid1);
        me.detail = {};
        me.data.Month = new Date().getMonth() + 1;
        me.data.Year = new Date().getFullYear();
        me.detail.QtyTarget = 0;
        me.detail.AmountTarget = 0;
        me.detail.TotalJaringan = 0;
    }


    me.grid1 = new webix.ui({
        container:"wxsalestarget",
        view: "wxtable",
        scroll:'xy',
        columns:[
            { id:"CategoryCode",   header:"Category Code", width: 150},
            { id:"CategoryName",   header:"Category Name", fillspace: true},
            { id: "QtyTarget", header: "Qty Target", width: 180, format: webix.i18n.numberFormat, css: { "text-align": "right" } },
            { id: "AmountTarget", header: "Amount Target", width: 180, format: webix.i18n.intFormat, css: { "text-align": "right" } },
            { id: "TotalJaringan", header: "Amount Network", width: 180, format: webix.i18n.intFormat, css: { "text-align": "right" } }
        ],
        on:{
            onSelectChange:function(){
                if (me.grid1.getSelectedId() !== undefined) {
                   me.detail = this.getItem(me.grid1.getSelectedId().id);
                   me.detail.oid = me.grid1.getSelectedId();                  
                   me.Apply();
                   me.ReformatNumber();
                   me.ReformatNumber();
                }
            }
        }          
    });

    webix.event(window, "resize", function(){ 
        me.grid1.adjust(); 
    })

    me.$on("IsEditingEvent",function()
    {
        me.grid1.adjust();
    });

    me.checkValidation4Btn = function()
    {
        //if (isLoadData == false) return false;
        if (me.detail === undefined) return false;
        return (me.detail.CategoryCode === undefined);
    }

    $("[name = 'CategoryCode']").on('blur', function () {
        if ($('#CategoryCode').val() || $('#CategoryCode').val() != '') {
            $http.post('gn.api/masteritem/GetLookupValueName?VarGroup=CSCT&varCode=' + $('#CategoryCode').val()).// 2parameter
            success(function (v, status, headers, config) {
                if (v != "") {
                    me.detail.CategoryName = v;
                } else {
                    $('#CategoryCode').val('');
                    $('#CategoryName').val('');
                    me.CategoryCode();
                }
            });
        } else {
            me.detail.CategoryCode = '';
            me.detail.CategoryName = '';
            me.CategoryCode();
        }
    });

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Sales Target",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "Sales Target",
                items: [
                       
                        { name: "Month", validasi:"required", opt_text: "MONTH?", cls: "span4", disable: "IsEditing()", type: "select2", text: "Month", datasource: "comboMonth" },
                        { name: "Year", validasi:"required", opt_text: "YEAR?",cls: "span4", disable: "IsEditing()", type: "select2", text: "Year", datasource: "comboYear"},
                        { name: "QtyTarget", type: "text", text: "Qty Target", cls: "span4 number", placeHolder: "0.00", readonly: true},
                        { name: "AmountTarget", type : "text", text: "Amount Target", cls: "span4 number-int", placeHolder: "0", readonly: true },
                        { name: "TotalJaringan", type: "text", text: "Amount Network", cls: "span4 number-int", placeHolder: "0", readonly: true },
                    ]   
            },
            {
                name: "pnlB",              
                title: "Sales Target Details",
                show: "IsEditing()",
                items: [
                        {
                            text: "Category Code",
                            type: "controls",
                            items: [
                                { name: "CategoryCode", model: "detail.CategoryCode", cls: "span2", placeHolder: "Category Code", type: "popup", btnName: "btnCategoryCode", readonly: false, click:"CategoryCode()", disable: "data.Month === undefined || data.Year === undefined" },
                                { name: "CategoryName", model: "detail.CategoryName", cls: "span6", placeHolder: "Category Name", readonly: true },
                            ],
                        },
                        { name: "addQtyTarget", model: "detail.QtyTarget", text: "Qty Target", cls: "span4", placeHolder: "0.00", type: "decimal", max: 9999999999999 },
                        { name: "addAmountTarget", model: "detail.AmountTarget", text: "Amount Target", cls: "span4", placeHolder: "0", type: "int", max: 999999999999999 },
                        { name: "addTotalJaringan", model: "detail.TotalJaringan", text: "Amount Network", cls: "span4", placeHolder: "0", type: "int", max: 999999999 },
                        {
                            type: "buttons",
                            items: [
                                    { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddEditModel()", show: "detail.oid === undefined", disable: "checkValidation4Btn()" },
                                    {name: "btnUpdateModel", text: "Update", icon: "icon-save", cls:"btn btn-success", click: "AddEditModel()", show: "detail.oid !== undefined", disable:"checkValidation4Btn()" },
                                    {name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls:"btn btn-danger", click: "delete2()", show: "detail.oid !== undefined"},
                                    {name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls:"btn btn-warning", click: "CloseModel()", show: "detail.oid !== undefined"}
                                ]
                        },
                    {
                        name: "wxsalestarget",           
                        type: "wxdiv",
                    },
                ]
            },    

        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("sptargetPenjualanController"); 
    }

});