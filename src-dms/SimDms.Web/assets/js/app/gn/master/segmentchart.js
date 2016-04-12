var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });
    
    $http.post('sp.api/Combo/LoadComboData?CodeId=SEGM').
   success(function (data, status, headers, config) {
       me.Segment = data;
   });
    $http.post('sp.api/Combo/LoadComboData?CodeId=ACCT').
    success(function (data, status, headers, config) {
        me.account = data;
    });
    
    me.browse = function () {
        var lookup = Wx.blookup({
            name: "BranchLookUp",
            title: "Lookup Branch",
            manager: gnManager,
            query: new breeze.EntityQuery().from("SegmentAcc").withParameters({ param: me.data.TipeSegAcc }),
            defaultSort: "SegAccNo asc",
            columns: [
                { field: "Type", title: "Type" },
                { field: "SegAccNo", title: "Account No." },
                { field: "AccountType", title: "Account Type" },
                { field: "Description", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.TipeSegAcc = data.TipeSegAcc;
                me.data.SegAccNo = data.SegAccNo;
                me.data.AccountName = data.SegAccNo.replace(/\s/g, '');
                me.data.AccountCode = data.SegAccNo;
                me.data.Parent = data.Parent;
                me.data.Description = data.Description;
                me.data.FromDate = data.FromDate;
                me.data.EndDate = data.EndDate;
                me.data.oid = true;
                getAccount();
                $("[name = 'TipeSegAcc']").prop("disabled", true);
                me.Apply();
            }
        });
    };

    me.Parent = function () {
        var lookup = Wx.blookup({
            name: "AccountLookUp",
            title: "Lookup Account Type",
            manager: gnManager,
            query: new breeze.EntityQuery().from("LookUpDtlAll").withParameters({ param: "ACCT" }),
            defaultSort: "LookUpValue asc",
            columns: [
                { field: "LookUpValue", title: "Account No." },
                { field: "LookUpValueName", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.isSave = false;
                me.data.Parent = data.LookUpValue;
                me.data.ParentName = data.LookUpValueName;
                me.Apply();
            }
        });
    };

    me.initialize = function () {
        me.data.TipeSegAcc = '100';
        me.data.FromDate = me.now();
        me.data.EndDate = me.now();
        me.data.Description = "";
        $("label[for='ParentID']").hide();
        $("#Parent", "#ParentName").hide();
        //$("#ParentName").hide();
        $("#btnParent").hide();
        $("label[for='Account']").hide();
        $("#s2id_AccountName").hide();
        $("#AccountCode").hide();
        me.hasChanged = false;
    }

    $("[name = 'AccountName']").on('change', function () {
        me.data.AccountCode = $(this).val();
        
        if ($(this).val()) {
            $("[name = 'TipeSegAcc']").prop("disabled", true);
            me.data.Description = $('#AccountName').select2("data").text;
        }
    });

    $("[name = 'Parent']").on('blur', function () {
        me.data.Parent = $(this).val();

        if ($(this).val() || $(this).val() !='') {
            getAccount();
        } else {
            me.data.Parent = '';
            me.data.ParentName = '';
            me.Parent();
        }
    });

    function getAccount() {
        if (me.data.Parent) {
            $http.post('gn.api/SegmentChart/CheckLookUpDtl?&CodeID=ACCT&LookUpValue=' + me.data.Parent).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.data.ParentName = data.data.LookUpValueName;
                    } else {
                        me.data.Parent = '';
                        me.data.ParentName = '';
                        me.Parent();
                    }
                });
        }
    }

    $("[name = 'TipeSegAcc']").on('change', function () {
        me.data.TipeSegAcc = $(this).val();
        me.data.SegAccNo = "";
        //alert($("#label").text())
        switch ($(this).val()) {
            case "300":
                $("#label").text("Length 5 digit");
                $("#SegAccNo").prop("maxlength", "5");
                $("label[for='ParentID']").hide();
                $("#Parent").hide();
                $("#ParentName").hide();
                $("#btnParent").hide();

                $("label[for='Account']").hide();
                $("#s2id_AccountName").hide();
                $("#AccountCode").hide();

                $("label[for='SegAccNo']").show();
                $("#label").show();
                $("#SegAccNo").show();
                break;
            case "400":
                $("#label").text("Length 6 digit");
                $("#SegAccNo").prop("maxlength", "6");
                $("label[for='ParentID']").show();
                $("#Parent").show();
                $("#ParentName").show();
                $("#btnParent").show();

                $("label[for='Account']").hide();
                $("#s2id_AccountName").hide();
                $("#AccountCode").hide();

                $("label[for='SegAccNo']").show();
                $("#label").show();
                $("#SegAccNo").show();
                break;
            case "700":
                $("#label").text("Length 3 digit");
                $("#SegAccNo").prop("maxlength", "3");
                $("label[for='ParentID']").hide();
                $("#Parent").hide();
                $("#ParentName").hide();
                $("#btnParent").hide();

                $("label[for='SegAccNo']").hide();
                $("#label").hide();
                $("#SegAccNo").hide();

                $("label[for='Account']").show();
                $("#s2id_AccountName").show();
                $("#AccountCode").show();
                break;
            default:
                $("#label").text("Length 3 digit");
                $("#SegAccNo").prop("maxlength", "3");
                $("label[for='ParentID']").hide();
                $("#Parent").hide();
                $("#ParentName").hide();
                $("#btnParent").hide();

                $("label[for='Account']").hide();
                $("#s2id_AccountName").hide();
                $("#AccountCode").hide();

                $("label[for='SegAccNo']").show();
                $("#label").show();
                $("#SegAccNo").show();
        }
    });
    me.cancelOrClose = function () {
        me.init();
        $("[name = 'TipeSegAcc']").prop("disabled", false);
    }
    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            $http.post('gn.api/SegmentChart/Delete', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.init();
                    $("[name = 'TipeSegAcc']").prop("disabled", false);
                    Wx.Success("Data zip code deleted...");
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
        if (me.data.AccountCode) { me.data.SegAccNo = me.data.AccountCode }
        $http.post('gn.api/SegmentChart/Save', me.data).
            success(function (data, status, headers, config) {
                if (data.status) {
                    Wx.Success("Data saved...");
                    localStorage.setItem("CloseInterval", true);
                    me.cancelOrClose();
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
        title: "Segment Chart of Account GL",
        xtype: "panels",
        toolbars: //WxButtons,
        [
            { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "!hasChanged || hasChanged || isInitialize || isLoadData", click: "browse()" },
            //{ name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData", click: "delete()" },
            //{ name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize", click: "save()" },
            //{ name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize", click: "cancelOrClose()" }
        ],
        panels: [
            {
                name: "pnlInfo",
                title: "",
                items: [
                    { name: "TipeSegAcc", model: "data.TipeSegAcc", type: "select2", text: "Tipe Segment", cls: "span4 full", datasource: "Segment" },
                    { type: "divider" },
                    {
                        name: "Account",
                        text: "No.Account",
                        type: "controls",
                        cls: 'span8',
                        items: [
                            { name: "AccountName", model: "data.AccountName", type: "select2", text: "Nama Account ", cls: "span4", datasource: "account" },
                            { name: "AccountCode", model: "data.AccountCode", text: "Account Code", cls: "span4", readonly: true, required: false },
                        ]
                    },
					{ name: "SegAccNo", type: "text", text: "No. Account", cls: "span4", maxlength: 3, placeholder: "0", require: true, validasi: "required" },
                    { name: "label", type: "label", text: "Length 3 digit", cls: "span4" },
                    { name: "Description", model: "data.Description", text: "Keterangan", cls: "span8", require: true, validasi: "required" },
                    {
                        name: "ParentID", type: "controls", text: "Tipe Account ", required: true, items: [
                              { name: "Parent", model: "data.Parent", text: "Account Code", type: "popup", cls: "span3", click: "Parent()", disable: "IsEditing() || testDisabled", require: true, validasi: "required" },
                              { name: "ParentName", model: "data.ParentName", text: "Account Name", cls: "span5", readonly: true, required: false },
                         ]
                    },
                    { name: "FromDate", type: "ng-datepicker", text: "Tgl.Dari", cls: "span4 full" },
                    { name: "EndDate", type: "ng-datepicker", text: "Tgl.Ke", cls: "span4 full" },
                     {
                         type: "buttons",
                         items: [
                                 { name: "btnAddModel", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "save()", show: "data.oid === undefined", disable: "data.TipeSegAcc === undefined" },
                                 { name: "btnUpdateModel", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "save()", show: "data.oid !== undefined" },
                                 { name: "btnDeleteModel", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "delete()", show: "data.oid !== undefined" },
                                 { name: "btnCancelModel", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "cancelOrClose()", show: "data.oid !== undefined" }
                         ]
                     },
                ]
            },

        ]
    };
    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init() {
        SimDms.Angular("spItemPriceController");
    }
});