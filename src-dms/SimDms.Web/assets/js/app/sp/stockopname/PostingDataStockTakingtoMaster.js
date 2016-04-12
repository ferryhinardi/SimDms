var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spPostingDataStockTakingtoMasterController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.FisrtLoad = function () {
        me.data.Adjustment = me.data.StockTaking = true;
        
        $http.post('sp.api/StockTakingPost/FisrtLoad', me.data).
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
        if (me.data.Adjustment == undefined || me.data.Adjustment == false) {
            MsgBox("Adjustment harus dipilih (ya)", MSG_ERROR);
            return false
        }
        if (me.data.StockTaking == undefined || me.data.StockTaking == false) {
            MsgBox("Stock Taking harus dipilih (ya)", MSG_ERROR);
            return false;
        }
        MsgConfirm("Apakah anda yakin?", function (result) {
            if (result) {
                $http.post('sp.api/StockTakingPost/Proccess_Master', me.data).
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    me.NextProsess(dl.data);
                } else {
                    alert(dl.message);
                    MsgBox("Gagal simpan data", MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
            } else {
                MsgBox("Gagal simpan data", MSG_ERROR);
            }
        });
        
    }
    me.setInformationStock = function(){
        $http.post('sp.api/StockTakingPost/setInformationStock').
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    me.data.InfoStockTaking = dl.data;
                } else {
                    MsgBox(dl.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.NextProsess = function (data) {
        $http.post('sp.api/StockTakingPost/ProcToMasterTuning?WarehouseCode='+me.data.LookUpValue, data).
            success(function (dl, status, headers, config) {
                if (dl.success) {
                    MsgBox("Save Success");
                    me.setInformationStock();
                } else {
                    MsgBox("Save Fail", MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }



    me.initialize = function () {
        me.data = {};
        me.FisrtLoad();

    }



    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Posting Stock Taking to Master",
        xtype: "panels",
        //  toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                        {
                            name: "Status", show: "data.jing ==2"
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
                            name: "Adjustment",
                            text: "Adjustment",
                            type: "x-switch",
                            cls: "span4"
                        },
                        {
                            name: "StockTaking",
                            text: "Stock Taking",
                            type: "x-switch",
                            cls: "span4"
                        },{type:"div"},
                        {
                            type: "buttons",
                            items: [
                                    { name: "btnProsess", text: "Proses", icon: "icon icon-gear", cls: "btn btn-info", click: "Prosess()", disable: "data.Status == 0" },
                            ]
                        }
                ]
            },
            {
                name: "pnlB",
                title: "Informasi Stok Taking",
                items: [
                    {
                        name: "InfoStockTaking",
                        type: "textarea",
                        readonly: true
                    },
                ]
            },

        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spPostingDataStockTakingtoMasterController");
    }

});