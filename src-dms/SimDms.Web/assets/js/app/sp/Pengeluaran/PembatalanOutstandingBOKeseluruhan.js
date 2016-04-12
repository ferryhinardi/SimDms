"use strict"

function spPembatalanOutstandingBOAll($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/TransactionType?CodeId=SLTP').
    success(function (data, status, headers, config) {
        me.cmbSales = data;
        me.init();
    });

    //me.initialize = function () {


    $('#SalesType').on('change', function (e) {
        var index = $('#SalesType').select2("val");
        switch (index) {
            case "0":
                $http.post('sp.api/Combo/TransactionType?CodeId=TTPJ').
                success(function (data, status, headers, config) {
                    me.cmbTrans = data;
                });
                break;
            case "1":
                $http.post('sp.api/Combo/TransactionType?CodeId=TTNP').
                success(function (data, status, headers, config) {
                    me.cmbTrans = data;
                });
                break;
            case "2":
                $http.post('sp.api/Combo/TransactionType?CodeId=TTSR').
                success(function (data, status, headers, config) {
                    me.cmbTrans = data;
                });
                break;
            case "3":
                $http.post('sp.api/Combo/TransactionType?CodeId=TTSL').
                success(function (data, status, headers, config) {
                    me.cmbTrans = data;
                });
                break;
        }

        //me.init();
    });


    me.rawQuery = function () {
        var bc = me.data.BranchCode;
        var st = $('#SalesType').select2("val");
        var tt = $('#TransType').select2("val");
        if ($('#CustomerCode').val() == null || $('#CustomerCode').val() == '') {
            MsgBox('Ada data yang belum lengkap', MSG_ERROR);
            return;
        }
        var url = "sp.api/pengeluaran/GetDataOutstandingBO?CompanyCode=" + me.data.CompanyCode
                    + '&BranchCode=' + bc + '&CustomerCode=' + me.data.CustomerCode
                    + '&TrsType=' + tt + '&TypeOfGoods=' + me.data.TypeOfGoods
                    + '&SlsType=' + st + '&ProductType=' + me.data.ProductType;
        layout.loadAjaxLoader();
        $http.post(url)
            .success(function (data, status, headers, config) {
                me.detail = data.data;
                if (data.success == true) {
                    me.loadTableData(me.gridOutsBO, data);
                }
                else {
                    MsgBox(data.message, MSG_ERROR);
                }
                //console.log(data.data);
            }).error(function (e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
            });

        console.log(me.data.CompanyCode, bc, me.data.CustomerCode, tt, me.data.TypeOfGoods, st, me.data.ProductType);
    }

    me.Process = function () {
        var datDetail = [];

        $.each(me.detail, function (key, val) {
            var arr = {
                "chkSelect": val["chkSelect"],
                "DocNo": val["DocNo"],
                "PartNo": val["PartNo"],
                "PartNoOriginal": val["PartNoOriginal"],
                "QtyBOOts": val["QtyBOOts"],
                "QtyBOCancel": val["QtyBOCancel"]
            }
            datDetail.push(arr);
            console.log(datDetail);
        });

        var dat = {};
        dat["User"] = me.data.User;
        dat["CompanyCode"] = me.data.CompanyCode;
        dat["BranchCode"] = me.data.BranchCode;
        dat["SalesType"] = $('#SalesType').select2("val");
        dat["Note"] = $('#Note').val();
        dat["model"] = datDetail;
        var JSONData = JSON.stringify(dat);
        console.log(JSONData);
        var chk = datDetail.chkSelect;

        $http.post('sp.api/CancelAllBoOutStanding/CheckCancelAllBoOuts', JSONData).
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    MsgConfirm("Apakah anda yakin melakukan pembatalan Outstanding BO ?", function (result) {
                        if (result) {
                            $http.post('sp.api/CancelAllBoOutStanding/ProcessCancelAllBoOuts', JSONData).
                                success(function (dl, status, headers, config) {
                                    MsgBox("Proses cancel all BO outstanding berhasil");
                                    me.gridOutsBO.adjust();
                                }).
                            error(function (e, status, headers, config) {
                                MsgBox("Connecting server error", MSG_ERROR);
                                console.log(status);
                            });
                        }
                    });

                } else {
                    MsgBox(dl.message, MSG_ERROR);
                    console.log(dl.error_log);
                }

            }).
      error(function (e, status, headers, config) {
          MsgBox("Connecting server error", MSG_ERROR);
          console.log(status);
      });

    }

    me.CustomerCode = function () {
        var lookupstring = "CustomerCodeLookup";
        var lookup = Wx.blookup({
            name: "CustomerLookup",
            title: "Master Customer Lookup",
            manager: pengeluaranManager,
            query: new breeze.EntityQuery.from(lookupstring).withParameters({ status: '1' }),
            defaultSort: "CustomerCode asc",

            columns: [
                { field: "CustomerCode", title: "Customer Code" },
                { field: "CustomerName", title: "Customer Name" },
                { field: 'Address', title: 'Address' },
                { field: 'ProfitCenter', title: 'Profit Center' },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.CustomerCode = data.CustomerCode;
                me.data.CustomerName = data.CustomerName;
            }
            me.Apply();
        });
    };

    me.gridOutsBO = new webix.ui({
        container: "OutsBO",
        view: "wxtable", css:"alternating",
        scrollX: true,
        height: 500,
        autoheight: true,
        columns: [
            { id: "chkSelect", header: { content: "masterCheckbox", contentId: "chkSelect" }, width: 60, template: "{common.checkbox()}" },

            { id: "DocNo", header: "No. SO", width: 180 },
            { id: "DocDate", header: "Tgl. SO", width: 200, format: me.dateFormat },
            { id: "PartNo", header: "No. Part", width: 200 },
            { id: "PartNoOriginal", header: "No. Part Original", width: 210 },
            { id: "QtyBOOts", header: "BO Outstanding", width: 100 },
            { id: "QtyBOCancel", header: "BO Cancel", width: 100 },
        ],

        on: {
            onSelectChange: function () {
                if (me.gridOutsBO.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.gridOutsBO.getSelectedId().id);
                    me.detail.oid = me.gridOutsBO.getSelectedId();
                    me.Apply();
                    //console.log(me.detail);

                }
            }
        }
    });



    me.initialize = function () {
        me.data = {};
        me.detail = {};
        me.griddetail = [];
        $http.get('breeze/SparePart/CurrentUserInfo').
          success(function (dl, status, headers, config) {
              me.data.CompanyCode = dl.CompanyCode;
              me.data.BranchCode = dl.BranchCode;
              me.data.TypeOfGoods = dl.TypeOfGoods;
              me.data.ProductType = dl.ProductType;
              me.data.User = dl.User
          });
        me.gridOutsBO.adjust();
    }


    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Pembatalan Outstanding BO Keseluruhan",
        xtype: "panels",
        toolbars: [
            { name: "btnQuery", text: "Query", cls: "btn btn-success", icon: "icon-search", click: "rawQuery()" },
        ],
        panels: [

            {

                name: "pnlMain",
                title: "",
                items: [
                    { name: "CompanyCode", model: "data.CompanyCode", text: "Kode Company", cls: "span4 full", show: false },
                    { name: "BranchCode", model: "data.BranchCode", text: "Kode Branch", cls: "span4 full", show: false },
                    { name: "TypeOfGoods", model: "data.TypeOfGoods", text: "Tipe Barang", cls: "span4 full", show: false },
                    { name: "ProductType", model: "data.ProductType", text: "Tipe Produk", cls: "span4 full", show: false },
                    { name: "Ppn", model: "data.Ppn", text: "PPn", cls: "span4 full", show: false },
                    { name: "User", model: "data.User", text: "PPn", cls: "span4 full", show: false },

                    { name: "SalesType", text: "Sales Type", type: "select2", cls: "span4", datasource: 'cmbSales' },
                    { name: "TransType", text: "Transaction Type", type: "select2", cls: "span4", datasource: 'cmbTrans' },
                    {
                        text: "Customer",
                        type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span2", placeHolder: "Customer Code", type: "popup", readonly: false, click: "CustomerCode()", required: true, validasi: "required" },
                            { name: "CustomerName", cls: "span6", placeHolder: "Customer Name", readonly: true }
                        ]
                    },

                ]
            },
            { type: "hr" },
            {
                name: "pnlDetail",
                title: "Data Detail back Order",
                items: [
                       { type: "label", text: "- Pilih/centang Back Order yang akan di process default", cls: "span6 mylabel" },
                       { name: "Note", text: "Note", cls: "span6 full" },
                       {
                           type: "buttons",
                           items: [
                               { name: "btnProcess", text: "Process", icon: "icon-gear", cls: "btn btn-success", click: "Process()" },
                           ]
                       },
                       {
                           name: "gridSODetail",
                           type: "wxdiv",
                       },
                ]
            },
             {
                 xtype: "tabs",
                 name: "tabpage1",
                 items: [
                     { name: "tA", text: "Outstanding BO", cls: "active" },
                 ]
             },
             {
                 name: "OutsBO",
                 title: "OutsBO",
                 cls: "tabpage1 tA",
                 xtype: "wxtable"
             },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        $(".full").attr("style", "padding:9px 9px 6px 5px");
        SimDms.Angular("spPembatalanOutstandingBOAll");
    }
});



//var totalSrvAmt = 0;
//var status = 'N';
//var svType = '2';

//"use strict";

//function spPembatalanOutstandingBOKeseluruhanController($scope, $http, $injector) {

//    var me = $scope;

//    $injector.invoke(BaseController, this, { $scope: me });

//    $http.post('sp.api/Combo/Years').
//    success(function (data, status, headers, config) {
//        me.comboYear = data;
//    });

//    $http.post('sp.api/Combo/Months').
//    success(function (data, status, headers, config) {
//        me.comboMonth = data;
//    });

//    me.browse = function () {
//        var lookup = Wx.blookup({
//            name: "targetPenjualanBrowse",
//            title: "Sales Target Browse",
//            manager: spManager,
//            query: "TargetPenjualanBrowse",
//            defaultSort: "Year asc",
//            columns: [
//            { field: "Year", title: "Year" },
//            { field: "Month", title: "Month" },
//            { field: "QtyTarget", title: "Qty Target" }
//            ]
//        });

//        lookup.dblClick(function (result) {
//            if (result != null) {
//                me.lookupAfterSelect(result);   
//                me.isSave = false;
//                me.Apply();
//                me.loadDetail(result);
//            }
//        });
//    }


//    me.loadDetail = function(data)
//    {
//        var p1 = me.where.create("Year", "eq",  parseInt(data.Year));
//        var p2 = me.where.create("Month", "eq", parseInt(data.Month));
//        var whereClause = p1.and(p2);

//        var query = new breeze.EntityQuery()
//            .from("SalesTargetDetail")
//            .where(whereClause);

//          spManager.executeQuery(query).then(function(data){
//              me.UpdateGridDetail(data.results);
//          }).fail(function(e) {
//              MsgBox(e, MSG_ERROR);  
//          });
//    }


//    me.CategoryCode = function () {
//        var lookup = Wx.blookup({
//            name: "categorycodeLookup",
//            title: "Lookup Category Code",
//            manager: spManager,
//            query: "CategoryCodebrowse",
//            defaultSort: "CategoryCode asc",
//            columns: [
//                { field: "CategoryCode", title: "Category Code" },
//                { field: "CategoryName", title: "Category Name" }
//            ]
//        });
//        lookup.dblClick(function (data) {
//            if (data != null) {
//                me.detail.CategoryCode = data.CategoryCode;
//                me.detail.CategoryName = data.CategoryName;
//                me.isSave = false;
//                me.Apply();
//            }
//        });
//    }


//    me.saveData = function (e, param) {
//        $http.post('sp.api/TargetPenjualan/save', me.data).
//            success(function (data, status, headers, config) {
//                if (data.success) {
//                    Wx.Success("Data saved...");
//                    me.startEditing();
//                    me.loadDetail(me.data);
//                } else {
//                    MsgBox(data.message, MSG_ERROR);
//                }
//            }).
//            error(function (data, status, headers, config) {
//                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
//            });
//    }


//    me.UpdateGridDetail = function(data)
//    {
//        me.grid.detail = data;     
//        me.loadTableData(me.grid1, me.grid.detail); 
//    }

//    me.AddEditModel = function () {

//        if (me.detail.CategoryCode === undefined || me.detail.CategoryCode == null)
//        {
//            MsgBox("CategoryCode is required!!!", MSG_ERROR);
//            return;
//        }

//        me.LinkDetail();

//        $http.post('sp.api/TargetPenjualan/save2', me.detail).
//            success(function (data, status, headers, config) {
//                if (data.success) {
//                    Wx.Success("Data saved...");

//                    me.UpdateGridDetail(data.data);     

//                    me.data.QtyTarget = data.total.QtyTarget;
//                    me.data.AmountTarget = data.total.AmountTarget;
//                    me.data.TotalJaringan = data.total.TotalJaringan;

//                    me.startEditing();
//                    me.detail = {} ;

//                } else {
//                    MsgBox(data.message, MSG_ERROR);
//                }
//            }).
//            error(function (data, status, headers, config) {
//                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
//            });
//    }

//    me.delete = function () {
//        MsgConfirm("Are you sure to delete current record?", function (result) {
//            if (result) {
//                $http.post('sp.api/TargetPenjualan/delete', me.data).
//                success(function (dl, status, headers, config) {
//                    if (dl.success) {
//                        Wx.Info("Record has been deleted...");
//                        me.init();                        
//                    } else {
//                        MsgBox(dl.message, MSG_ERROR);
//                    }
//                }).
//                error(function (e, status, headers, config) {
//                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
//                });
//            }
//        });
//    }

//    me.LinkDetail = function()
//    {
//        me.detail.Month = me.data.Month;
//        me.detail.Year = me.data.Year;
//    }

//    me.delete2 = function () {

//        MsgConfirm("Are you sure to delete current record?", function (result) {

//            if (result) {

//                me.LinkDetail();

//                $http.post('sp.api/TargetPenjualan/delete2', me.detail).
//                success(function (dl, status, headers, config) {
//                    if (dl.success) {

//                        me.detail = {} ;
//                        Wx.Info("Record has been deleted...");

//                        if (dl.count > 0) 
//                            me.UpdateGridDetail(dl.data); 
//                        else
//                            me.clearTable(me.grid1);

//                        me.data.QtyTarget = dl.total.QtyTarget;
//                        me.data.AmountTarget = dl.total.AmountTarget;
//                        me.data.TotalJaringan = dl.total.TotalJaringan;

//                    } else {
//                        MsgBox(dl.message, MSG_ERROR);
//                    }
//                }).
//                error(function (e, status, headers, config) {
//                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
//                });
//            }
//        });
//    }

//    me.initialize = function()
//    {
//        me.clearTable(me.grid1);
//        me.detail = {};
//    }


//    me.grid1 = new webix.ui({
//        container:"wxsalestarget",
//        view:"wxtable", 
//        columns:[
//            { id:"CategoryCode",   header:"Category Code", fillspace: true},
//            { id:"CategoryName",   header:"Category Name", fillspace: true},
//            { id:"QtyTarget",      header:"Qty Target" ,   fillspace: true},
//            { id:"AmountTarget",   header:"Amount Target", fillspace: true},
//            { id:"TotalJaringan",  header:"Amount Network",fillspace: true}
//        ],
//        on:{
//            onSelectChange:function(){
//                if (me.grid1.getSelectedId() !== undefined) {
//                   me.detail = this.getItem(me.grid1.getSelectedId().id);
//                   me.detail.oid = me.grid1.getSelectedId();
//                   me.Apply();                    
//                }
//            }
//        }          
//    });

//    webix.event(window, "resize", function(){ 
//        me.grid1.adjust(); 
//    })

//    me.start();

//}


//$(document).ready(function () {
//    var options = {
//        title: "Cancel All BO Outstanding",
//        xtype: "panels",
//        toolbars: WxButtons,
//        panels: [
//            {
//                name: "pnlA",
//                title: "",
//                items: [

//                        { name: "Month", validasi:"required", opt_text: "MONTH?", cls: "span3", disable: "IsEditing()", type: "select2", text: "Month", datasource: "comboMonth" },
//                        { name: "Year", validasi:"required", opt_text: "YEAR?",cls: "span2", disable: "IsEditing()", type: "select2", text: "Year", datasource: "comboYear"},
//                        { name: "QtyTarget", text: "Qty Target", cls: "span2 number", placeHolder: "0", readonly: true },
//                        { name: "AmountTarget", text: "Amount Target", cls: "span3  number", placeHolder: "0", readonly: true },
//                        { name: "TotalJaringan", text: "Amount Network", cls: "span3  number", placeHolder: "0", readonly: true },                     
//                    ]   
//            },
//            {
//                name: "pnlB",              
//                title: "",
//                show: "IsEditing()",
//                items: [
//                        {
//                            text: "Category Code",
//                            type: "controls",
//                            items: [
//                                { name: "CategoryCode", model: "detail.CategoryCode", cls: "span2", placeHolder: "Category Code", type: "popup", btnName: "btnCategoryCode", readonly: false, click:"CategoryCode()", disable: "data.Month === undefined || data.Year === undefined" },
//                                { name: "CategoryName",  model: "detail.CategoryName", cls: "span6", placeHolder: "Category Name", readonly: true }
//                            ]
//                        },

//                        { name: "addQtyTarget",  model: "detail.QtyTarget", text: "Qty Target", cls: "span2 number", placeHolder: "0" },
//                        { name: "addAmountTarget",  model: "detail.AmountTarget", text: "Amount Target", cls: "span3  number", placeHolder: "0" },
//                        { name: "addTotalJaringan",  model: "detail.TotalJaringan", text: "Amount Network", cls: "span3  number", placeHolder: "0" },
//                    {
//                        type: "buttons", cls: "span3", items: [
//                            { name: "btnAdd", text: "Add", icon: "icon-plus", click:"AddEditModel()", cls: "btn btn-primary", disable: "detail.CategoryCode === undefined" },
//                            { name: "btnDlt", text: "Delete", icon: "icon-remove", click:"delete2()" , cls: "btn btn-danger", disable: "detail.oid === undefined"},
//                        ]
//                    },             
//                    {
//                        name: "wxsalestarget",           
//                        type: "wxdiv",
//                    }, 
//                ]
//            },    

//        ]
//    };

//    Wx = new SimDms.Widget(options);
//    Wx.default = {};
//    Wx.render(init);

//    function init(s) {
//        SimDms.Angular("spPembatalanOutstandingBOKeseluruhanController");
//    }

//});