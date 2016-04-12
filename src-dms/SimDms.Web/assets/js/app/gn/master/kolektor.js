var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterCollectorController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MasterCollectorrowse",
            title: "Collector Browse",
            manager: gnManager,
            query: "CollectorsBrowse",
            columns: [
            { field: "CollectorCode", title: "Kode Kolektor" },
            { field: "CollectorName", title: "Nama Kolektor" },
            { field: "ProfitCenterCode", title: "Kode Profit Center" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                //me.GetCustomerInfo(data.CustomerCode);
                me.isSave = false;
                $('#ProfitCenterCode').attr('disabled', true);
                me.Apply();

            }
        });
    }

    me.ProfitCenterCodeDisc = function () {
        var lookup = Wx.blookup({
            name: "ProfitCenterCodeDiscLookup",
            title: "Lookup ProfitCenterCodeDisc",
            manager: gnManager,
            query: "ProfitCenters",
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "ProfitCenter Code" },
                { field: "LookUpValueName", title: "ProfitCenter Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;

                me.data.ProfitCenterCode = data.LookUpValue;
                me.data.ProfitCenterNameDisc = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    $("[name = 'ProfitCenterCode']").on('blur', function () {
        if ($('#ProfitCenterCode').val() == '') return;
        if ($('#ProfitCenterCode').val() != '') {
            $http.post('gn.api/Collector/CollectorLoad', me.data).
                   success(function (v, status, headers, config) {
                       if (v.success) {
                           me.lookupAfterSelect(v.data[0]);
                           $('#ProfitCenterCode').attr('disabled', true);
                       } else {
                           $('#ProfitCenterCode').val('');
                           $('#ProfitCenterNameDisc').val('');
                           me.ProfitCenterCodeDisc();
                       }
                   });
        }
    });
    //        $http.post('gn.api/Lookup/getLookupName?CodeId=PFCN&LookupValue=' + $('#ProfitCenterCode').val()).
    //        success(function (v, status, headers, config) {
    //            if (v.TitleName != '') {
    //                me.data.ProfitCenterNameDisc = v.TitleName;
    //            }else{
    //                $('#ProfitCenterCode').val('');
    //                $('#ProfitCenterNameDisc').val('');
    //                me.ProfitCenterCodeDisc();
    //            }
    //        });
    //   } else {
    //        me.data.ProfitCenterNameDisc = '';
    //        me.ProfitCenterCodeDisc();
    //    }
    //});

    me.initialize = function () {
        $('#ProfitCenterCode').removeAttr('disabled');
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('gn.api/Collector/Delete', me.data).
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
            }
        });
    }

    me.saveData = function (e, param) {

        $http.post('gn.api/Collector/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
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
        title: "Collector",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataCustomerClass",
                title: "Collector",
                items: [

                        { name: "CollectorCode", type: "text", text: "Collector Code", cls: "span8", disable: "IsEditing() || testDisabled", validasi: "required", maxlength: 15 },
                        {
                            type: "controls", text: "Profit Center", required: true, items: [
                                  { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Profit Center Code", type: "popup", cls: "span3",  click: "ProfitCenterCodeDisc()", validasi: "required" },
                                  { name: "ProfitCenterNameDisc", model: "data.ProfitCenterNameDisc", text: "Profit Center Name", cls: "span5", readonly: true, required: false },
                             ]
                        },
                        { name: "CollectorName", type: "text", text: "Collector Name", cls: "span8", maxlength: 100 },
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