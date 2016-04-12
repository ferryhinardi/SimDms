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
            name: "MasterfakturPajakBrowse",
            title: "Faktur Pajak Browse",
            manager: gnManager,
            query: "FPJseqNo",
            defaultSort: "BranchCode asc",
            columns: [
            { field: "CompanyCode", title: "Company Code" },
            { field: "Year", title: "Year Tax" },
            { field: "CompanyGovName", title: "Company Name" },
            { field: "BranchCode", title: "Branch Code" },
            { field: "CompanyName", title: "Branch Name" },
            { field: "FPJSeqNo", title: "FPJ Seq. No." },
            { field: "SeqNo", title: "Seq. No." },
            { field: "BeginTaxNo", title: "Begin Tax No." },
            { field: "EndTaxNo", title: "End Tax No." },
            { field: "EffectiveDate", title: "Effective Date" }
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

    me.Company = function () {
        var lookup = Wx.blookup({
            name: "CompanyLookUp",
            title: "Lookup Company",
            manager: gnManager,
            query: "CurrentUserInfo",
            defaultSort: "CompanyCode asc",
            columns: [
                { field: "CompanyCode", title: "Company Code" },
                { field: "CompanyGovName", title: "Company Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.CompanyCode = data.CompanyCode;
                me.data.CompanyGovName = data.CompanyGovName;
                me.Apply();
            }
        });
    };

    me.Branch = function () {
        var lookup = Wx.blookup({
            name: "BranchLookUp",
            title: "Lookup Branch",
            manager: gnManager,
            query: "AllBranch",
            defaultSort: "BranchCode asc",
            columns: [
                { field: "BranchCode", title: "Branch Code" },
                { field: "CompanyName", title: "Branch Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.BranchCode = data.BranchCode;
                me.data.CompanyName = data.CompanyName;
                if (me.data.SeqNo == 0) {
                    console.log($('#Year').val());
                    $http.post('gn.api/FakturPajakSeq/LastSeqNo?branchCode='+ me.data.BranchCode +'&Year=' + $('#Year').val())
                    .success(function (v, status, header, config) {
                        me.data.SeqNo = v.data;
                    });
                }
                me.Apply();
            }
        });
    };

    $("[name = 'CompanyCode']").on('blur', function () {
        if ($('#CompanyCode').val() || $('#CompanyCode').val() != '') {
            $http.post('gn.api/Lookup/getCompanyCode?branchCode=').
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.CompanyGovName = v.CompanyGovName;
                } else {
                    $('#CompanyCode').val('');
                    $('#CompanyGovName').val('');
                    me.Company();
                }
            });
        } else {
            me.data.CompanyGovName = '';
            me.Company();
        }
    });

    $("[name = 'BranchCode']").on('blur', function () {
        if ($('#BranchCode').val() || $('#BranchCode').val() != '') {
            $http.post('gn.api/Lookup/getCompanyCode?branchCode=' + $('#BranchCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.CompanyName = v.CompanyName;
                   
                } else {
                    $('#BranchCode').val('');
                    $('#CompanyName').val('');
                    me.Branch();
                }
            });
        } else {
            me.data.CompanyName = '';
            me.Branch();
        }
    });

    $("[name = 'BeginTaxNo']").on('blur', function () {
        if ($('#BeginTaxNo').val() || $('#BeginTaxNo').val() != '') {
            var x = $('#BeginTaxNo').val();
            console.log(x);
            var y = x - 1;
            console.log(parseInt($('#BeginTaxNo').val()));
            me.data.FPJSeqNo = parseInt($('#BeginTaxNo').val()) - 1;
            me.Apply();
        } else {
            me.data.CompanyGovName = '';
            me.Company();
        }
    });

    me.initialize = function () {
        me.data.Year = new Date().getFullYear();
        me.data.EffectiveDate = me.now();
        me.data.SeqNo = 0;
        me.data.FPJSeqNo = 0;
        $('#isActive').prop('checked', true);
        me.data.isActive = true
        me.hasChanged = false;
        me.Apply();
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/FakturPajakSeq/Delete', me.data).
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

        $http.post('gn.api/FakturPajakSeq/Save', me.data).
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
        title: "Faktur Pajak Seq.",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "aktur Pajak",
                title: "",
                items: [
                        {
                            type: "controls", text: "Company", required: true, items: [
                                 { name: "CompanyCode", model: "data.CompanyCode", text: "CompanyCode", type: "popup", cls: "span3", click: "Company()", disable: "IsEditing() || testDisabled", required: true, validasi: "required" },
                                 { name: "CompanyGovName", model: "data.CompanyGovName", text: "CompanyName", cls: "span5", readonly: true, required: false },
                            ]},
                            {
                                type: "controls", text: "Branch", required: true, items: [
                                     { name: "BranchCode", model: "data.BranchCode", text: "Branch Code", type: "popup", cls: "span3", click: "Branch()", disable: "IsEditing() || testDisabled", required: true, validasi: "required" },
                                     { name: "CompanyName", model: "data.CompanyName", text: "Branch Name", cls: "span5", readonly: true, required: false },
                                ]
                            },
                            { name: 'Year', model: "data.Year", text: "Year", type: "select2", cls: "span3", optionalText: "-- SELECT YEAR --", disable: "IsEditing() || testDisabled", datasource: "Years" },
                            { name: "SeqNo", model: 'data.SeqNo', type: "int", text: "Seq No", cls: "span3 number-int full", disable: "true", maxlength: 11 },
                            { name: "BeginTaxNo", model: 'data.BeginTaxNo', type: "int", text: "Begin Tax No.", cls: "span4 number-int", required: true, validasi: "required", maxlength: 11 },
                            { name: "EndTaxNo", model: 'data.EndTaxNo', type: "int", text: "End Tax No.", cls: "span4 number-int", required: true, validasi: "required", maxlength: 11 },
                            { name: "EffectiveDate", text: "Effective Date", cls: "span4", type: "ng-datepicker" },
                            { name: "FPJSeqNo", model: 'data.FPJSeqNo', type: "int", text: "FPJ Sequence No.", cls: "span4 number-int", maxlength: 11, cls: "span4 number-int", min: 0, max: 9999999999 },
                            { name: 'isActive', model: "data.isActive", type: 'check', text: 'Active', cls: 'span2 full', float: 'left' }
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