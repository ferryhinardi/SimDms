var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterCollectorController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });


    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MasterFPJSignatureBrowse",
            title: "FPJ Signature Date Browse",
            manager: gnManager,
            query: "FPJSignature",
            defaultSort: "ProfitCenterCode asc",
            columns: [
            { field: "ProfitCenterCode", title: "Profit Center Code" },
            { field: "FPJOption", title: "Option" },
            { field: "FPJOptionDescription", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.data.ProfitCenterNameDisc = data.LookUpValueName;
                me.isSave = false;
                me.Apply();

            }
        });
    }

    me.FPJOption = [{ "value": "1", "text": "1" }, { "value": "2", "text": "2" }];

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
                //me.data.ProfitCenterNameDisc = data.LookUpValueName;
                me.checkData(data.LookUpValue, data.LookUpValueName);
                //me.data.FPJOption = "2";
                //me.data.FPJOptionDescription = "Tanggal transaksi";
                me.Apply();
            }
        });
    };

    me.checkData = function (ProfitCenterCode, ProfitCenterNameDisc) {
        $http.post('gn.api/FPJSignatureDate/checkData?ProfitCenterCode=' + ProfitCenterCode).
            success(function (data, status, headers, config) {
                if(data.success){
                me.data.ProfitCenterCode = data.ProfitCenterCode;
                me.data.ProfitCenterNameDisc = ProfitCenterNameDisc; 
                me.data.FPJOption = data.FPJOption;
                me.data.FPJOptionDescription = data.FPJOptionDescription;
                me.isLoadData = true;
                setTimeout(function () {
                    me.hasChanged = false;
                    me.startEditing();
                    me.isSave = false;
                    $scope.$apply();
                }, 200);
            }else{
                    me.data.ProfitCenterCode = ProfitCenterCode;
                    me.data.ProfitCenterNameDisc = ProfitCenterNameDisc;
    } 
            });
        //me.data.ProfitCenterNameDisc = ProfitCenterNameDisc;
    }

    $("[name = 'ProfitCenterCode']").on('blur', function () {
        if ($('#ProfitCenterCode').val() || $('#ProfitCenterCode').val() != '') {
            $http.post('gn.api/Lookup/getLookupName?CodeId=PFCN&LookupValue=' + $('#ProfitCenterCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    //me.data.ProfitCenterNameDisc = v.TitleName;
                    me.checkData($('#ProfitCenterCode').val(), v.TitleName);
                } else {
                    $('#ProfitCenterCode').val('');
                    $('#ProfitCenterNameDisc').val('');
                    me.ProfitCenterCodeDisc();
                }
            });
        } else {
            me.data.ProfitCenterNameDisc = '';
            me.ProfitCenterCodeDisc();
        }
    });

    me.initialize = function () {
        me.data = {};
        //me.data.FPJOption = "";
        //me.data.FPJOptionDescription = "";
        me.hasChanged = false;
        me.Apply();
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/FPJSignatureDate/Delete', me.data).
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

        $http.post('gn.api/FPJSignatureDate/Save', me.data).
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

    $("[name = 'FPJOption']").on('change', function () {
        //console.log(me.data.FPJOption);
        //console.log($(this).val());
        me.data.FPJOption = $(this).val();
        if (me.data.FPJOption == "1") {
            me.data.FPJOptionDescription = "Tanggal 1 dan (bulan transaksi + 1)";
        } else if (me.data.FPJOption == "2") {
            me.data.FPJOptionDescription = "Tanggal transaksi";
        } else { me.data.FPJOptionDescription = ""; }
    });

    me.start();
}



$(document).ready(function () {
    var options = {
        title: "FPJ Signature Date",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataCustomerClass",
                title: "",
                items: [

                        {
                            type: "controls", text: "Profit Center", required: true, items: [
                                 { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Profit Center Code", type: "popup", cls: "span3", click: "ProfitCenterCodeDisc()", validasi: "required" },
                                 { name: "ProfitCenterNameDisc", model: "data.ProfitCenterNameDisc", text: "Profit Center Name", cls: "span5", readonly: true, required: false },
                            ]
                        },
                        {
                            type: "controls", text: "Opsi", items: [
                            { name: "FPJOption", model: 'data.FPJOption', type: "select2", text: "", cls: "span3", datasource: "FPJOption"},
                            { name: "FPJOptionDescription", model: 'data.FPJOptionDescription', type: "text", text: "", cls: "span5", readonly: true, },
                            ]
                        }
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