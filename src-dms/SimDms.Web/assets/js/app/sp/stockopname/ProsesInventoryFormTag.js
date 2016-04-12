"use strict";
var firstLoad = true;

function spProsesInventoryFormTagController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPGO').
    success(function (data, status, headers, config) {
        me.comboTPGO = data;
    });
    
    $http.post('sp.api/Combo/LoadComboData?CodeId=WRCD').
    success(function (data, status, headers, config) {   
        data = $(data).filter(function () {
            return (this.value >= '00' && this.value<='95');            
        });        
        me.comboWRCD = data;
    });

    $http.post('sp.api/Combo/LoadComboData?CodeId=TPCD').
    success(function (data, status, headers, config) {
        me.comboTPCD = data;
    });

    $http.post('sp.api/Combo/LoadComboData?CodeId=COND').
    success(function (data, status, headers, config) {
        data.push({ value: 3, text: "Blank Form/Tag" });
      me.comboCOND = data;
    });
 
    me.loadDetail = function (data) {
    }

    me.process = function () {
        if (me.data.WarehouseCode == '' || me.data.TypeCode == '' || me.data.Condition == '') {
            var message = '';
            if (me.data.WarehouseCode == '') {
                message = "Warehouse Code wajib diisi !";
            }
            else if (me.data.TypeCode == '') {
                message = "Form/Tag wajib diisi !";   
            }
            else if (me.data.Condition == '') {
                message = "Kondisi wajib diisi !";
            }

            MsgBox(message, MSG_INFO);
            return;
        }
        
        $(".ajax-loader").show();
        $http.post('sp.api/StockOpname/ValidationProsesInvTag', me.data).
            success(function (data) {
                if (data.success) {
                    $(".ajax-loader").hide();
                    MsgConfirm("Proses dilanjutkan, Apakah anda yakin ?", function (result) {
                        if (result) {
                            $(".ajax-loader").show();
                            $http.post('sp.api/StockOpname/ProsesInvTag', me.data).success(function (data) {
                                if (data.success) {
                                    if (data.data != undefined) {
                                        $(".ajax-loader").hide();
                                        me.testDisabled = true;
                                        me.isSave = false;
                                        me.isPrintAvailable = true;
                                        me.isPrintEnable = true;
                                        me.readyToModified();
                                        $("#btnDelete,#btnCancel").hide();
                                        $("#lblInfo").html(data.message);
                                        MsgBox(data.message, MSG_INFO);
                                        $(".ajax-loader").hide();
                                    }
                                    else {
                                        $(".ajax-loader").hide();
                                        MsgBox(data.message, MSG_WARNING);
                                    }
                                }
                                else {
                                    $(".ajax-loader").hide();
                                    MsgBox(data.message, MSG_WARNING);
                                }
                            });
                        }
                    });
                }
            });
        }

    me.PrintStockTaking = function () {
        $http.post('sp.api/StockOpname/PrintStockTaking', me.data).
         success(function (data) {
             if (data.success) {
                 var prm = [data.param1, data.param2, data.param3];
                 Wx.showPdfReport({
                     id: data.reportID,
                     pparam: prm.join(','),
                     rparam: 'Stock Taking',
                     type: 'devex'
                 });
             }
             else
                 MsgBox(data.message, MSG_WARNING);
         });
    }

    me.testDisabled = false;
    me.firstLoad = true;

    me.initialize = function()
    {
        if (!me.firstLoad) return;
        me.data = {};
        me.data.LocationCode = "%";

        $('#lblInfo').html("");
        $('#lblInfo').css(
            {
                "font-size": "32px",
                "color": "blue",
                "font-weight": "bold",
                "text-align": "center"
            });

        //me.isInProcess = true;
        $http.post('sp.api/stockopname/CheckButtonValidation').
        success(function (data, status, headers, config) {            
            if (data.data != undefined) {
                $("#lblInfo").html(data.message);
                $("#TypeOfGoods").select2("val", data.data.TypeOfGoods);
                $("#WarehouseCode").select2("val", data.data.WarehouseCode);
                $("#TypeCode").select2("val", data.data.TypeCode);
                $("#Condition").select2("val", data.data.Condition);
                me.testDisabled = true;
                me.isSave = false;
                me.isPrintAvailable = true;
                me.isPrintEnable = true;
                me.readyToModified();
            }
            else {
                me.isInProcess = false;
                me.changedData(true);
                $("#btnBrowse").hide();
            }
        });
        me.firstLoad = false;
    }
    
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Inventory Form/Tag",
        xtype: "panels",
        toolbars: [
            { name: "btnPrint", text: "Print", cls: "btn btn-primary", icon: "icon-print", show: "isPrintAvailable", click: "PrintStockTaking()" }
            ],
        panels: [
            {
                name: "pnlA",
                title: "Proses Inventory Form/Tag",
                items: [
                        { name: "TypeOfGoods", opt_text: "[SELECT ALL]", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Tipe Part", datasource: "comboTPGO" },
                        { name: "WarehouseCode", validasi: "required", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Warehouse Code", datasource: "comboWRCD" },
                        { name: "LocationCode", text: "Lokasi (% = Semua)", cls: "span3 full", disable: "testDisabled", validasi: "required" }, 
                        
                        { name: "TypeCode", validasi: "required", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Form/Tag", datasource: "comboTPCD" },
                        { name: "Condition", validasi: "required", cls: "span3 full", disable: "IsEditing() || testDisabled", type: "select2", text: "Kondisi", datasource: "comboCOND" },
                        
                        {
                            type: "buttons", cls: "span3 full", items: [
                          { name: "btnProcess", text: "Proses Stock Opname", cls: "btn btn-success", disable: "IsEditing() || testDisabled", click: "process()" },
                          ]
                        },
                        { name: "lblInfo", type: "label", text: "", cls: "span3 full" }
                    ]   
            } 

        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("spProsesInventoryFormTagController");
    }

});