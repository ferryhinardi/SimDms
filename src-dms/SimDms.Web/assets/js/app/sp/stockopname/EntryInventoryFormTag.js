"use strict";

function spEntryInventoryFormTagController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.Select4Lookup = function () {
        
        var clm = [
            { field: "STNo", title: "No. Form/Tag" },
            { field: "WarehouseCode", title: "Gudang" }
        ];

        if (me.StockHeader.Condition == "1" || me.StockHeader == "2") {
            clm.push({ field: "PartNo", title: "No. Part" });
        }

        me.detil = {};
        var lookup = Wx.blookup({
            name: "noformtaglookup",
            title: "Pencarian No. Form/Tag",
            manager: spStockOpnameManager,
            query: "BrowseFormTagNo",
            defaultSort: "STNo asc",
            columns: clm
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data = data;
                //me.data.STNos = data.STNo;
                //me.data.WarehouseCode = data.WarehouseCode;
                me.lookupAfterSelect(data);
                //me.isSave = false;
                //me.Apply();

                $(".ajax-loader").show();
                me.ReloadGrid();
                
                //me.loadTableData(me.grid1, {});
            }
        });
    }

    me.SelectSeq = function () {             
        var lookup = Wx.blookup({
            name: "seqlookup",
            title: "Pencarian No. Sequence",
            manager: spStockOpnameManager,
            query: new breeze.EntityQuery.from("BrowseSeqNo").withParameters({ STNo: me.data.STNo }),
            defaultSort: "SeqNo asc",
            columns:[
              { field: "SeqNo", title: "No. Seq" },
              { field: "PartNo    ", title: "No. Part" },
              { field: "LocationCode", title: "Lokasi" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {                
                me.detil = data;
                $(".ajax-loader").show();
                $http.post('sp.api/StockOpname/StockTackingTemp', data)
                .success(function (rslt, status, headers, config) {
                    if (rslt.PartInfo != undefined)
                        me.detil.PartName = rslt.PartInfo.PartName;
                    
                    if (rslt.data.PartNo == '')
                        $("#btnPartNo").prop('disabled', false)
                    else
                        $("#btnPartNo").prop('disabled', true)

                    me.detil.NewData = true;
                    me.grid1.clearSelection();
                    $(".ajax-loader").hide();
                });               
               
            }
        });
    }
    
    me.SelectPart = function () {
        var lookup = Wx.blookup({
            name: "seqlookup",
            title: "Pencarian No. Part",
            manager: spStockOpnameManager,
            query: new breeze.EntityQuery.from("SelectEntryStockTaking").withParameters({ WarehouseCode: me.data.WarehouseCode }),
            defaultSort: "PartNo asc",
            columns: [
              { field: "PartNo", title: "Nomor Part" },
              { field: "PartName", title: "Nama Part" }              
            ]
        });
        lookup.dblClick(function (data) {            
            if (data != null) {
                $(".ajax-loader").show();
                me.detil.PartNo = data.PartNo.trim();
                me.detil.PartName = data.PartName.trim();
                $http.post('sp.api/StockOpname/CheckPartPrice?Partno=' + data.PartNo)
                 .success(function (rslt, status, headers, config) {
                     if (rslt.success) {                                                 
                         $(".ajax-loader").hide();
                     }
                     else {
                         me.detil.PartNo = "";
                         me.detil.PartName = "";
                         $(".ajax-loader").hide();
                         MsgBox(rslt.message, MSG_ERROR);
                     }                   
                 });
            }
        });
    }

    me.ReloadGrid=function (data)
    {
        $http.post('sp.api/StockOpname/StockTackingDtl', data)
              .success(function (rslt, status, headers, config) {
                  me.UpdateGridDetail(rslt.data);
                  $(".ajax-loader").hide();
              });      

    }

    me.UpdateGridDetail = function(data)
    {
        if (data == undefined) {
        }
        me.grid.detail = data;     
        me.loadTableData(me.grid1, me.grid.detail);
        $scope.renderGrid();
    }

    me.isvalid = function () {

        var ret=true;
        $("#pnlB input").removeClass("error");
        if ($("#SeqNo").val() == "") {
            $("#SeqNo").addClass("error");
            ret = false;
        }


        if ($("#PartNo").val() == "") {
            $("#PartNo").addClass("error");
            ret = false;
        }


        if ($("#STQty").val() == "") {
            $("#STQty").addClass("error");
            ret = false;
        }
        if ($("#STDmgQty").val() == "") {
            $("#STDmgQty").addClass("error");
            ret = false;
        }

        if(!ret)
        MsgBox("Ada informasi belum lengkap!", MSG_ERROR);
        
        return ret;

    }
    me.AddEditModel = function () {
        
        if (!me.isvalid())         
            return;
        
        $(".ajax-loader").show();
        $http.post('sp.api/StockOpname/SaveEntry', me.detil).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");                                        
                    
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
                //me.ReloadGrid(me.data);
                me.UpdateGridDetail(data.data);
                $(".ajax-loader").hide();
                
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                me.ReloadGrid(me.data);
            });
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
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }
     
    me.grid1 = new webix.ui({
        container: "wxsalestarget",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "SeqNo", header: "No.", fillspace: true },
            { id: "PartNo", header: "No. Part", fillspace: true },
            { id: "LocationCode", header: "Lokasi", fillspace: true },
            { id: "STQty", header: "Qty. Total", fillspace: true },
            { id: "STDmgQty", header: "Qty. Rusak", fillspace: true },
            { id: "PartName", header: "Nama Part", fillspace: true },
            {
                id: "isMainLocation", header: "Mn.Lk", fillspace: true, format: function (value) {
                    var ret = (value?"Ya":"Tidak");
                    return ret;
                }
            }
        ],
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detil = this.getItem(me.grid1.getSelectedId().id);
                    me.detil.oid = me.grid1.getSelectedId();
                    me.detail.NewData = false;
                    me.Apply();
                }
            }
        }
    });

    me.initialize = function()
    {
        me.clearTable(me.grid1);
        me.detail = {};
        $("#pnlA").hide();
        $http.post('sp.api/stockopname/SparepartValidation?menuType=ST').
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    me.StockHeader = dl.data;
                    $("#pnlA").show();
                }
                else {
                    MsgBox(dl.message, MSG_ERROR);                    
                }
            });
    }



    webix.event(window, "resize", function () {
        me.grid1.adjust(); 
    })


    $scope.renderGrid = function () {
        setTimeout(function () {
            me.grid1.adjust();            
        }, 1000);
    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Entry Inventory Form/Tag",
        xtype: "panels",
        //toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [                       
                        { name: "STNo", text: "No. Form/Tag", model: "data.STNo", cls: "span3", placeHolder: "Category Code", type: "popup", btnName: "btnSTNo", readonly: true, click: "Select4Lookup()" },
                        { name: "WarehouseCode", text: "Gudang", cls: "span3 full", disable: "!isInitialize" }                        
                    ]   
            },
            {
                name: "pnlB",              
                title: "",
                show: "IsEditing()",
                items: [
                        { name: "SeqNo", text: "No. ", model: "detil.SeqNo", cls: "span2 full", placeHolder: "Category Code", type: "popup", btnName: "btnSeqNo", readonly: true, click: "SelectSeq()" },
                        { name: "PartNo", text: "No. Part", model: "detil.PartNo", cls: "span4 full", placeHolder: "Nama Part", type: "popup", btnName: "btnPartNo", readonly: true, click: "SelectPart()" },
                        { name: "PartName", text: "Nama Part", model: "detil.PartName", cls: "span4 full",disable: true },
                        { name: "LocationCode", text: "Lokasi", model: "detil.LocationCode", cls: "span4 full",disable: "testDisabled" },
                        { name: "STQty", text: "Qty. Total", model: "detil.STQty", cls: "span2 number", disable: "testDisabled" },
                        { name: "STDmgQty", text: "Qty. Rusak", model: "detil.STDmgQty", cls: "span2 number", disable: "testDisabled" },
                        { name: "temp", type:"hidden" },                                               
                        {
                            type: "buttons", cls: "span3", items: [
                            { name: "btnAdd", text: "Save", icon: "icon-save", click: "AddEditModel()", cls: "btn btn-success", disable:"detil.SeqNo == undefined"},
                            { name: "btnDlt", text: "Delete", icon: "icon-remove", click: "delete2()", cls: "btn btn-danger", disable: "detil.oid === undefined" },
                        ]
                        },             
                        {
                            name: "wxsalestarget",           
                            type: "wxdiv"
                        }, 
                ]
            },    

        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {       
        SimDms.Angular("spEntryInventoryFormTagController");
        $("label[for='SeqNo']").html('<b><font color="red">*</font></b>No. ')
        $("label[for='PartNo']").html('<b><font color="red">*</font></b>No. Part')
        $("label[for='STQty']").html('<b><font color="red">*</font></b>Qty. Total')
        $("label[for='STDmgQty']").html('<b><font color="red">*</font></b>Qty. Rusak')
    }

});