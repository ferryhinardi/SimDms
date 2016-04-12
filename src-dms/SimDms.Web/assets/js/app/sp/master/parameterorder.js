var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";
function spParameterOrderController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me })

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "paramcodeBrowse",
            title: "Param Code Browse",
            manager: spManager,
            query: "ParamCode",
            defaultSort: "SupplierCode asc",
            columns: [
             { field: 'SupplierCode', title: 'Supplier Code' },
             { field: 'SupplierName', title: 'Supplier Name' },
             { field: 'MovingCode', title: 'Moving Code' },
            ]
        });

        lookup.dblClick(function (p) {
            if (p != null) {
                me.lookupAfterSelect(p);
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.supplierCode = function () {
        var lookup = Wx.blookup({
            name: "suppliercodeBrowse",
            title: "Supplier Code Browse",
            manager: spManager,
            query: "Suppliers",
            defaultSort: "SupplierCode asc",
            columns: [
             { field: 'SupplierCode', title: 'Supplier Code' },
             { field: 'SupplierName', title: 'Supplier Name' }
            ]
        });

        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.movingCode = function () {
        var lookup = Wx.blookup({
            name: "movingcodeBrowse",
            title: "Moving Code Browse",
            manager: spManager,
            query: "ParamCode",
            defaultSort: "MovingCode asc",
            columns: [
             { field: 'MovingCode', title: 'Moving Code' },
             { field: 'MovingCodeName', title: 'MovingCode Name' }
            ]
        })

        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.MovingCode = data.MovingCode;
                me.data.MovingCodeName = data.MovingCodeName;
                me.isSave = false;
                me.Apply();
            }
        });

    }

    me.saveData = function (e,param)
    {
        $http.post('sp.api/parameterorder/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success)
                {
                    Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
            });
    }

    me.delete = function ()
    {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/parameterorder/delete', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        Wx.Info("Record has been deleted...");
                      
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

    me.applyAll = function () {
        MsgConfirm("Are you sure to Update ALl Part?", function (result) {
            if(result){
                $http.post('sp.api/ParameterOrder/UpdateAll', me.data).success(function (x, status, headers, config) {
                    if (x.success) {
                        Wx.success("Data Update All..");
                        me.startEditing();
                        } else {
                        MsgBox(x.message, MSG_ERROR);
                    }
                }).error(function (e, status, headers, config) {
                    MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                });
            }
        });
    }

    $("[name = 'SupplierCode']").on('blur', function () {
        if ($('#SupplierCode').val() || $('#SupplierCode').val() != '') {
            $http.post('gn.api/Lookup/SupplierName?SupplierClass=' + $('#SupplierCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.SupplierName = v.TitleName;
                } else {
                    $('#SupplierCode').val('');
                    $('#SupplierName').val('');
                    me.supplierCode();
                }
            });
        } else {
            me.data.SupplierCode = '';
            me.data.SupplierName = '';
            me.supplierCode();
        }
    });

    $("[name = 'MovingCode']").on('blur', function () {
        if ($('#MovingCode').val() || $('#MovingCode').val() != '') {
            $http.post('gn.api/MovingCode/MovingCodeName?MovingCode=' + $('#MovingCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.MovingCodeName = v.TitleName;
                } else {
                    $('#MovingCode').val('');
                    $('#MovingCodeName').val('');
                    me.movingCode();
                }
            });
        } else {
            me.data.MovingCode = '';
            me.data.MovingCodeName = '';
            me.movingCode();
        }
    });

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Parameter Order",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                        {
                            type: "buttons", items: [
                                { name: "btnApplyAll", text: "Apply All Part", icon: "icon-gear", disable: "!isLoadData", click: "applyAll()" },
                            ]
                        },


                        {
                            text: "Supplier Code",
                            type: "controls",
                            required: true ,
                            items: [
                                {
                                    name: "SupplierCode", cls: "span2", placeHolder: "Supplier Code", type: "popup",
                                    btnName: "btnSupplierCode", readonly: false, click: "supplierCode()",  required: true , validasi: "required"
                                },
                                { name: "SupplierName", cls: "span6", placeHolder: "Supplier Name", readonly: true}
                            ]
                        },
                        {
                            text: "Moving Code",
                            type: "controls",
                            required: true ,
                            items: [
                                { name: "MovingCode", cls: "span2", placeHolder: "Moving Code", type: "popup", btnName: "btnMovingCode", readonly: false, click: "movingCode()", required: true , validasi: "required" },
                                { name: "MovingCodeName", cls: "span6", placeHolder: "MovingCode Name", readonly: true}
                            ]
                        },

                ]
            },
                      {
                          name: "pnlB",
                          title: "Order Parameter (day)",
                          items: [

                              { name: "LeadTime", text: "Lead Time", cls: "span2  number", placeHolder: "0" },
                              { name: "OrderCycle", text: "Order Cycle", cls: "span2  number", placeHolder: "0" },
                              { name: "SafetyStock", text: "Safety Stock", cls: "span2  number", placeHolder: "0" }


                          ]
                      },




        ]
    };


 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spParameterOrderController");
    }



});