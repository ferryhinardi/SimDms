var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spitemKonversiController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "itemkonversiBrowse",
            title: "Item Convertion Browse",
            manager: spManager,
            query: "Itemkonversibrowse",
            defaultSort: "PartNo asc",
            columns: [
            { field: "PartNo", title: "PartNo" },
            { field: "PartName", title: "Part Name" },
            { field: "FromQty", title: "FromQty" },
            { field: "ToQty", title: "ToQty" },
            { field: "IsActive", title: "IsActive" } 
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

    me.PartNo = function () {
        var lookup = Wx.blookup({
            name: "categorycodeLookup",
            title: "Lookup Category Code",
            manager: spManager,
            query: "MasterPartView",
            defaultSort: "PartNo asc",
            columns: [
                { field: "PartNo", title: "Part No" },
                { field: "PartName", title: "Part Name" },
                { field: "SupplierCode", title: "Supplier Code" },
                { field: "IsGenuinePart", title: "IsGenuinePart" },
                { field: "ProductType", title: "ProductType" },
                { field: "PartCategory", title: "Part Category" },
                { field: "CategoryName", title: "Category Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.data.PartNo = data.PartNo;
                me.data.PartName = data.PartName;

                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.saveData = function () {
        $http.post('sp.api/itemkonversi/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
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
                $http.post('sp.api/itemkonversi/delete', me.data).
                success(function (dl, status, headers, config) {
                    if (dl.success) {
                        me.init();
                        Wx.Info("Record has been deleted...");

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

    me.start();

    $("[name = 'PartNo']").on('blur', function () {
        if ($('#PartNo').val() || $('#PartNo').val() != '') {
            $http.post('gn.api/masteritem/CheckItem?PartNo=' + $('#PartNo').val()).
            success(function (v, status, headers, config) {
                if (v.masterinfo) {
                    me.data.PartName = v.masterinfo.PartName;
                } else {
                    $('#PartNo').val('');
                    $('#PartName').val('');
                    me.PartNo();
                }
            });
        } else {
            me.data.PartNo = '';
            me.data.PartName = '';
            me.PartNo();
        }
    });

    me.initialize = function () {
        me.data.FromQty = number_format(1, 2);
        me.data.ToQty = number_format(0, 2);
        //me.ReformatNumber();
    };
}


$(document).ready(function () {
    var options = {
        title: "Item Convertion",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlA",
                title: "",
                items: [
                    {
                        text: "Part No",
                        type: "controls",
                        items: [
                            { name: "PartNo", cls: "span2", placeHolder: "Part No", type: "popup", btnName: "btnPartNo", required: true, validasi: "required", click: "PartNo()" },
                            { name: "PartName", cls: "span6", placeHolder: "Part Name", readonly: true }
                        ]
                    },
                        { name: "FromQty", cls: "span4 number", text: "From Qty", placeHolder: "0", validasi: "required, minValue[1]", required: true },
                        { name: "ToQty", cls: "span4 number-int", text: "To Qty", placeHolder: "0", validasi: "required, minValue[1]", required: true },
                        { name: "IsActive", type: "x-switch", text: "Is Active", float: "left" }
                    ]
            },
        ]
    }


 




 
 
 


    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spitemKonversiController");
    }



 


});