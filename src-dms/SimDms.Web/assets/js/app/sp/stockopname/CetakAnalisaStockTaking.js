var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spCetakAnalisaStockTakingController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.FisrtLoad = function () {
        $http.post('sp.api/StockTakingPrint/FisrtLoad', me.data).
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    me.data.LookUpValue = dl.data.LookUpValue;
                    me.data.LookUpValueName = dl.data.LookUpValueName;
                    me.data.Status = "1";
                } else {
                    me.data.Status = "0";
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.Prosess = function () {
        MsgConfirm("Are you sure?", function (result) {
            if (result) {
                $http.post('sp.api/StockTakingPrint/Proses', me.data).
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    MsgBox(dl.message);
                    me.loadTableData(me.grid1, dl.data);

                } else {
                    MsgBox(dl.message, MSG_ERROR);
                    me.loadTableData(me.grid1, dl.data);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
            } else {
                MsgBox("Update Gagal", MSG_ERROR);
            }
        });
        
    }


    me.initialize = function()
    {
        me.grid = {};
        me.clearTable(me.grid1);
        $scope.renderGrid();
        me.data = {};
        me.FisrtLoad();
        
    }


    me.grid1 = new webix.ui({
        container:"wxsalestarget",
        view:"wxtable", 
        columns:[
            { id: "PartNo", header: "No. Part", fillspace: true },
            { id: "STNo", header: "No. Stock Taking", fillspace: true },
            { id: "SeqNo", header: "No. Seq", fillspace: true },
            { id: "Status", header: "Status", fillspace: true },
        ],
        on:{
            onSelectChange:function(){
                if (me.grid1.getSelectedId() !== undefined) {
                   me.detail = this.getItem(me.grid1.getSelectedId().id);
                   me.detail.oid = me.grid1.getSelectedId();
                   me.Apply();                    
                }
            }
        }          
    });

    webix.event(window, "resize", function () {
        me.grid1.adjust();
    })

    $scope.renderGrid = function () {

        setTimeout(function () {
            me.grid1.adjust();
        }, 200);

    }

    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Print Stock Taking",
        xtype: "panels",
      //  toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                        {
                            name:"Status", show:"data.jing ==2"
                        },
                        {
                            name: "LookUpValue",
                            cls: "span3",
                            text: "Kode Gudang",
                            readonly: true
                        },
                        {
                            name: "LookUpValueName",
                            cls: "span5",
                            text: "Nama Gudang",
                            readonly: true
                        },
                        {
                            type: "buttons",
                            items: [
                                    { name: "btnProsess", text: "Proses", icon: "icon icon-gear", cls: "btn btn-info", click: "Prosess()", disable:"data.Status == 0" },
                                ]
                        }
                    ]   
            },
            {
                name: "pnlB",              
                title: "",
                items: [
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
        SimDms.Angular("spCetakAnalisaStockTakingController");
    }

});