var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spcampaignPembelianController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "purchcampaignBrowse",
            title: "Purchase Campaign Browse",
            manager: spManager,
            query: "masterpurchcampaignbrowse",
            defaultSort: "PartNo asc",
            columns: [
            { field: "PartNo", title: "PartNo" },
            { field: "PartName", title: "Part Name" },
            { field: "DiscPct", title: "Disc Pct" },
            { field: "BegDate", title: "Beg Date", type: "date", format:"{0:dd-MMM-yyyy}" },
            { field: "SupplierCode", title: "SupplierCode" },
            { field: "SupplierName", title: "SupplierName" }

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
                me.data.SupplierCode = data.SupplierCode;
                me.data.SupplierName = data.SupplierName;

                me.isSave = false;
                me.Apply();
            }
        });
    }

    me.getInfoPart = function (PartNo) {
        $http.post('gn.api/PurchCampaign/getRecord?PartNo=' + PartNo).
            success(function (v, status, headers, config) {
                if (v.success) {
                    me.data.SupplierCode = v.data.SupplierCode;
                    me.data.SupplierName = v.data.SupplierName;
                    me.data.PartName = v.data.PartName;
                    //me.Apply();
                } else {
                    //$('#PartNo').val('');
                    me.data.PartNo = '';
                    me.data.PartName = '';
                    me.data.SupplierCode = '';
                    me.data.SupplierName = '';
                    me.PartNo();
                }
            });
    }

    $("[name = 'PartNo']").on('blur', function () {
        if ($('#PartNo').val() || $('#PartNo').val() != '') {
            me.getInfoPart($('#PartNo').val());
        } else {
            me.data.PartNo = '';
            me.data.PartName = '';
            me.data.SupplierCode = '';
            me.data.SupplierName = '';
            me.PartNo();
        }
    });

    me.saveData = function (e, param) {
        $http.post('sp.api/purchcampaign/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
               // MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                console.log(data); 
            });
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sp.api/purchcampaign/delete', me.data).
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

    me.initialize = function () {
        me.data.DiscPct = 0;
        me.data.BegDate = me.now();
        me.data.EndDate = me.now();
    }
    me.start();

}


$(document).ready(function () {
    var options = {
        title: "Part Campaign for Purchase",
        xtype: "panels",
        toolbars:WxButtons,
        panels: [
            {
                name: "pnlPcP",
                title: "Part Campaign Purchase",
                items: [
                        {
                            text: "NoPart",
                            type: "controls",
                            required: true,
                            items: [
                                { name: "PartNo", cls: "span2", placeHolder: "Part No", type: "popup", btnName: "btnPartNo", readonly: false, click: "PartNo()", required: true, validasi: "required" },
                                { name: "PartName", cls: "span6", placeHolder: "Part Name", readonly: true }
                            ]
                        },
                        {
                            text: "Supplier Code",
                            type: "controls",

                            items: [
                                { name: "SupplierCode", model: "data.SupplierCode", cls: "span2", readonly: true },
                                { name: "SupplierName", cls: "span6", readonly: true }
                            ]
                        },
                    { name: "BegDate", text: "Campaign Date", cls: "span3", type: "ng-datepicker" },
                    { name: "EndDate", text: "S / D", cls: "span3", type: "ng-datepicker" },
                    { name: "DiscPct", text: "Discount", cls: "span2  number" }
                ]
            },
        ]
    };
 
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spcampaignPembelianController");
    }
});