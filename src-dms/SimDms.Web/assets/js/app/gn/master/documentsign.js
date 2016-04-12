var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function gnMasterCustomersController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "SignatureBrowse",
            title: "Document Signature Browse",
            manager: gnManager,
            query: "DocumentSignature",
            defaultSort: "ProfitCenterCode asc",
            columns: [
            { field: "ProfitCenterCode", title: "Profit Center Code" },
            { field: "DocumentType", title: "Document Type" },
            { field: "SeqNo", title: "Sequence No." },
            { field: "SignName", title: "Signature Name" },
            { field: "TitleSign", title: "Signature Position" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                //me.GetLookUpDtlInfo(data.BranchCode, data.ProfitCenterCode, data.DocumentType, data.SeqNo, "PFCN");
                me.data.ProfitCenterNameDisc = data.ProfitCenterName;
               // me.GetDocumentInfo(data.BranchCode, data.ProfitCenterCode, data.DocumentType, data.SeqNo);
                me.isSave = false;
                me.Apply();

            }
        });
    }
    me.GetDocumentInfo = function (BranchCode, ProfitCenterCode, DocumentType, SeqNo) {
        me.initialize();
        var src = "gn.api/DocumentSignature/CheckDocument?BranchCode=" + BranchCode + "&ProfitCenterCode=" + ProfitCenterCode + "&DocumentType=" + DocumentType + "&SeqNo=" + SeqNo;
        $http.post(src)
            .success(function (v, status, headers, config) {
                var param = { DocumentName: v.TitleName };
                angular.extend(me.data, v.data, param);
            }).error(function (e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
            });
    }

    me.GetLookUpDtlInfo = function (BranchCode, LookUpValue, DocumentType, SeqNo, CodeID) {
        me.initialize();
        var src = "gn.api/DocumentSignature/CheckLookUpDtl?DocumentType=" + DocumentType + "&BranchCode=" + BranchCode + "&CodeID=" + CodeID + "&ProfitCenterCode=" + LookUpValue + "&SeqNo=" + SeqNo;
        $http.post(src)
            .success(function (v, status, headers, config) {
                var param = { ProfitCenterNameDisc: v.TitleName };
                    angular.extend(me.data, v.data, param);
            }).error(function (e, status, headers, config) {
                MsgBox(e, MSG_ERROR);
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

    me.document = function () {
        var lookup = Wx.blookup({
            name: "MasterDocumentBrowse",
            title: "Document Browse",
            manager: gnManager,
            query: "Documents",
            defaultSort: "DocumentType asc",
            columns: [
            { field: "DocumentType", title: "Document Type" },
            { field: "DocumentName", title: "Document Name" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.DocumentType = data.DocumentType;
                me.data.DocumentName = data.DocumentName;
                me.Apply();
            }
        });
    }
   
    $("[name = 'ProfitCenterCode']").on('blur', function () {
        if ($('#ProfitCenterCode').val() || $('#ProfitCenterCode').val() != '') {
            $http.post('gn.api/Lookup/getLookupName?CodeId=PFCN&LookupValue=' + $('#ProfitCenterCode').val()).
            success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.ProfitCenterNameDisc = v.TitleName;
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
 
    $("[name = 'DocumentType']").on('blur', function () {
        if ($('#DocumentType').val() || $('#DocumentType').val() != '') {
            var src = "gn.api/DocumentSignature/CheckDocument?DocumentType=" + $('#DocumentType').val();
            $http.post(src)
            .success(function (v, status, headers, config) {
                if (v.TitleName != '') {
                    me.data.DocumentName = v.TitleName;
                } else {
                    $('#DocumentType').val('');
                    $('#DocumentName').val('');
                    me.document();
                }
            });
        } else {
            me.data.ProfitCenterNameDisc = '';
            me.document();
        }
    });
    me.initialize = function () {
        me.hasChanged = false;
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/DocumentSignature/Delete', me.data).
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

        $http.post('gn.api/DocumentSignature/Save', me.data).
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
        title: "Document Signature",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "DataDocumentSignature",
                title: "Document Signature Setting",
                items: [
                    
                    {
                        type: "controls", text: "Profit Center", required: true, items: [
                             { name: "ProfitCenterCode", model: "data.ProfitCenterCode", text: "Profit Center Code", type: "popup", cls: "span3", required: true, validasi :"required", click: "ProfitCenterCodeDisc()" },
                             { name: "ProfitCenterNameDisc", model: "data.ProfitCenterNameDisc", text: "Profit Center Name", cls: "span5", readonly: true, required: false },
                        ]
                    },
                    {
                        type: "controls", text: "Document Type", required: true, items: [
                            { name: "DocumentType", model: "data.DocumentType", type: "popup", text: "Document Type", cls: "span3", required: true, validasi: "required", click: "document()" },
                            { name: "DocumentName", type: "text", text: "Document Name", cls: "span5", readonly: true },
                        ]
                    },
                    { name: "SeqNo", type: "text", text: "Sequence No.", cls: "span6", disable: "IsEditing() || testDisabled", validasi: "required,max(50)" },
                    { name: "SignName", type: "text", text: "Signature Name", cls: "span6" },
                    { name: "TitleSign", type: "text", text: "Position Signature", cls: "span6" }

                ]
            }
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("gnMasterCustomersController");
    }




});