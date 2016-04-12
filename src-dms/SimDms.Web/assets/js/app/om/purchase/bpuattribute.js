var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterCollectorController($scope, $http, $injector) {

   var me = $scope;

   $injector.invoke(BaseController, this, { $scope: me });

   $http.post('sp.api/Combo/Years?').
   success(function (data, status, headers, config) {
       me.Years = data;
   });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "BPUAttributeBrowse",
            title: "BPU Attribute Browse",
            manager: spSalesManager,
            query: "BPUAttr",
            defaultSort: "BPUNo asc",
            columns: [
            { field: "BPUNo", title: "BPU No." },
            { field: "DONo", title: "DO No." },
            { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.isSave = false;
                me.Apply();

            }
        });
    }

    me.WarehouseCode = function () {

        var lookup = Wx.blookup({
            name: "WarehouseCodeLookup",
            title: "Lookup Warehouse",
            manager: spPersediaanManager,
            query: "WarehouseCodeMPWH",//"WarehouseCode",
            defaultSort: "warehousecode asc",
            columns: [
                { field: "warehousecode", title: "Warehouse Code" },
                { field: "lookupvaluename", title: "Warehouse Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.WarehouseCode = data.warehousecode;
                me.data.WarehouseName = data.lookupvaluename;
                me.isSave = false;
                me.Apply();
            }
        });
    };

    me.BPU = function () {
        var lookup = Wx.blookup({
            name: "BPULookUp",
            title: "BPU No. Lookup",
            manager: spSalesManager,
            query: "BPULookup",
            defaultSort: "BPUNo asc",
            columns: [
                { field: "BPUNo", title: "BPU No." },
                { field: "BPUDate", title: "BPU Date" },
                { field: "RefferenceDONo", title: "No. DO" },
                { field: "BPUNo", title: "No. SJ" },
                { field: "PONo", title: "No. PO" },
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.BPUNo = data.BPUNo;
                me.data.DONo = data.RefferenceDONo;
                me.Apply();
            }
        });
    };

    $("[name = 'BPUNo']").on('blur', function () {
        if ($('#BPUNo').val() || $('#BPUNo').val() != '') {
            $http.post('om.api/BPUAttribute/BPUView?BPUNo=' + $('#BPUNo').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.DONo = v.TitleName;
                } else {
                    $('#BPUNo').val('');
                    $('#DONo').val('');
                    me.BPU();
                }
            });
        } else {
            me.data.DONo = '';
            me.BPU();
        }
    });

    $("[name = 'WarehouseCode']").on('blur', function () {
        if ($('#WarehouseCode').val() || $('#WarehouseCode').val() != '') {
            $http.post('om.api/BPUAttribute/Warehouse?WarehouseCode=' + $('#WarehouseCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.WarehouseName = v.TitleName;
                } else {
                    $('#WarehouseCode').val('');
                    $('#WarehouseName').val('');
                    me.WarehouseCode();
                }
            });
        } else {
            me.data.WarehouseCode = '';
            me.WarehouseCode();
        }
    });

    $("[name = 'Subsidi1']").on('click', function () {
        if ($(this).val() == 0) {
            $(this).val("");
        }
    });

    $("[name = 'Subsidi1']").on('click', function () {
        if ($(this).val() == 0) {
            $(this).val("");
        }
    });

    $("[name = 'Subsidi2']").on('click', function () {
        if ($(this).val() == 0) {
            $(this).val("");
        }
    });

    $("[name = 'Subsidi3']").on('click', function () {
        if ($(this).val() == 0) {
            $(this).val("");
        }
    });

    $("[name = 'Subsidi4']").on('click', function () {
        if ($(this).val() == 0) {
            $(this).val("");
        }
    });

    $("[name = 'PotSKP']").on('click', function () {
        if ($(this).val() == 0) {
            $(this).val("");
        }
    });

    $("[name = 'Kompensasi']").on('click', function () {
        if ($(this).val() == 0) {
            $(this).val("");
        }
    });

    $("[name = 'HargaAccessories']").on('click', function () {
        if ($(this).val() == 0) {
            $(this).val("");
        }
    });

    me.initialize = function () {
        me.data.FakturPolisiDate = me.now();
        me.hasChanged = false;
        me.Apply();
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('om.api/BPUAttribute/Delete', me.data).
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
        });
    }

    me.saveData = function (e, param) {
        $http.post('om.api/BPUAttribute/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    };

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "BPU Attribute",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "",
                title: "",
                items: [
                            { name: "BPUNo", model: "data.BPUNo", text: "BPU No.", type: "popup", cls: "span4 full", click: "BPU()", required: true, validasi: "required" },
                            { name: "DONo", model: "data.DONo", text: "DO No.", cls: "span4 full", readonly: true },
                ]
            },
            {
                name: "",
                title: "",
                items: [
                           
                            { name: "NoFakturPolisi", model: 'data.SeqNo', type: "text", text: "No. Faktur Polisi", cls: "span4" },
                            { name: "FakturPolisiDate", text: "Faktur Polisi Date", cls: "span4", type: "ng-datepicker" },
                            {
                                 type: "controls", text: "Warehouse", items: [
                                      { name: "WareHouseCode", model: "data.WareHouseCode", text: "Warehouse Code", type: "popup", cls: "span3", click: "WarehouseCode()" },
                                      { name: "WarehouseName", model: "data.WarehouseName", text: "Warehouse Name", cls: "span5", readonly : true },
                                 ]
                            },
                            { name: "Subsidi1", model: 'data.Subsidi1', type: "text", text: "Subsidi 1", cls: "span4 number-int", value : 0 },
                            { name: "PotSKP", model: 'data.PotSKP', type: "text", text: "Potongan SKP", cls: "span4 number-int", value: 0 },
                            { name: "Subsidi2", model: 'data.Subsidi2', type: "text", text: "Subsidi 2", cls: "span4 number-int", value: 0 },
                            { name: "Kompensasi", model: 'data.Kompensasi', type: "text", text: "Kompensasi", cls: "span4 number-int", value: 0 },
                            { name: "Subsidi3", model: 'data.Subsidi3', type: "text", text: "Subsidi 3", cls: "span4 number-int", value: 0 },
                            { name: "HargaAccessories", model: 'data.HargaAccessories', type: "text", text: "Harga Accs", cls: "span4 number-int", value: 0 },
                            { name: "Subsidi4", model: 'data.Subsidi4', type: "text", text: "Subsidi 4", cls: "span4 number-int", value: 0 },


                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterCollectorController");
    }

});
//$(document).ready(function () {
//    var options = {
//        title: "Please be patient - This Page is Under Construction",
//        xtype: "panels",
//        panels: [
//            {
//                name: "pnlA",
//                items: [
//                ]
//            }
//        ]
//    };

//    Wx = new SimDms.Widget(options);
//    Wx.default = {};
//    Wx.render(init);



//    function init(s) {
//    }



//});