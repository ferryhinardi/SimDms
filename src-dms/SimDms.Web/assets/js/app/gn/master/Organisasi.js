var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

"use strict";

function spItemPriceController($scope, $http, $injector) {

    var me = $scope;
    $injector.invoke(BaseController, this, { $scope: me });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "MasterOrg",
            title: "Master Organisasi Browse",
            manager: gnManager,
            query: "GetOrganization",
            defaultSort: "CompanyCode asc",
            columns: [
                { field: "CompanyCode", title: "Kode Kantor Pusat" },
                { field: "CompanyName", title: "Nama Kantor Pusat" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.lookupAfterSelect(data);
                me.LoadDataDetails();
                $("#CompanyCode").attr('disabled', 'disabled');
                //me.control.DisableCompanyCode = true;
                me.Apply();
            }
        });
    }

    me.AccNo = function (TipeSegAcc) {
        var lookup = Wx.blookup({
            name: "MasterOrg",
            title: "Segment Acc Lookup",
            manager: gnManager,
            query: new breeze.EntityQuery().from("GetSegmentAcc").withParameters({ TipeSegAcc: TipeSegAcc }),//query: "GetSegmentAcc",
            defaultSort: "SegAccNo asc",
            columns: [
                { field: "SegAccNo", title: "Segment Acc No" },
                { field: "Description", title: "Description" }
            ]
        });
        lookup.dblClick(function (data) {
            if (data != null) {
                me.detail.BranchAccNo = data.SegAccNo;
                me.data.CompanyAccNo = data.SegAccNo;
                me.Apply();
            }
        });
    }

    me.LoadDataDetails = function () {
        var url = "gn.api/Organisasi/GetOrganizationDtl";
        $http.post(url, me.data).
        success(function (data, status, headers, config) {
            if (data.success) {
                $('#pnlB').show();
                setTimeout(function () {
                    me.detail.CompanyCode = me.data.CompanyCode;
                    me.grid1.detail = data.data;
                    me.loadTableData(me.grid1, me.grid1.detail);
                }, 1000);
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            console.log(data);
            MsgBox("Terjadi kesalahan dalam proses data. Hubungi SDMS Support", MSG_INFO);
        });
    }

    me.saveData = function (e, param) {
        var url = "gn.api/Organisasi/SaveOrganization";
        $http.post(url, me.data).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data Saved...");
                me.startEditing();
                $('#pnlB').show();
                me.loadTableData(me.grid1, me.grid1.detail);
                
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            console.log(data);
            MsgBox("Terjadi kesalahan dalam proses data. Hubungi SDMS Support", MSG_INFO);
        });
    }

    me.delete = function () {
        MsgConfirm("Apakah anda yakin ?", function (result) {
            if (result) {
                var url = "gn.api/Organisasi/DeleteOrganization";
                $http.post(url, me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.initialize();
                        Wx.Success("Data deleted...");

                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (data, status, headers, config) {
                    console.log(data);
                    MsgBox("Terjadi kesalahan dalam proses data. Hubungi SDMS Support", MSG_INFO);
                });
            }
        });
    }

    me.CancelDtl = function () {
        me.detail = {};
        me.control.DisableEditDtl = true;
        me.control.DisableDelDtl = true;
        me.grid1.clearSelection();
    }

    me.SaveDtl = function () {
        if (me.detail.BranchAccNo == '' || !me.detail.BranchAccNo || me.detail.BranchName == '' || !me.detail.BranchName) {
            if (!me.detail.BranchName && !me.detail.BranchAccNo) {
                SimDms.Warning("Please fill Branch office Name & Branch Acc. No!");
            } else
                if (!me.detail.BranchAccNo) {
                    SimDms.Warning("Please fill Branch Acc. No.!");
                } else if (!me.detail.BranchName) {
                    SimDms.Warning("Please fill Branch office Name!");
                }
        }else{
            $http.post("gn.api/Organisasi/SaveOrganizationDtl", me.detail).
            success(function (data, status, headers, config) {
            if (data.success) {
                me.LoadDataDetails();
                me.detail = {};
                Wx.Success("Data saved...");
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        }).
        error(function (data, status, headers, config) {
            console.log(data);
            MsgBox("Terjadi kesalahan dalam proses data. Hubungi SDMS Support", MSG_INFO);
        });
        }
    }

    me.DeleteDtl = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post("gn.api/Organisasi/DeleteOrganizationDtl", me.detail).
            success(function (data, status, headers, config) {
                if (data.success) {
                    me.LoadDataDetails();
                    me.detail = {};
                    me.control.DisableEditDtl = true;
                    me.control.DisableDelDtl = true;
                    Wx.Success("Data deleted...");
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (data, status, headers, config) {
                console.log(data);
                MsgBox("Terjadi kesalahan dalam proses data. Hubungi SDMS Support", MSG_INFO);
            });
            }
            
        });
    }

    me.initialize = function () {
        $('#pnlB').hide();
        me.data = {};
        me.detail = {};
        me.control = {};
        me.control.DisableEditDtl = true;
        me.control.DisableDelDtl = true;
        $("#CompanyCode").removeAttr('disabled');
        //me.control.DisableCompanyCode = false;
        me.grid1.detail = {};
        me.Apply();
    }

    $("[name = 'CompanyAccNo']").on('blur', function () {
        if ($('#CompanyAccNo').val() || $('#CompanyAccNo').val() != '') {
            var SegAccNo = $('#CompanyAccNo').val();
            $http.post('gn.api/Organisasi/CheckCompanyAccNo?SegAccNo=' + SegAccNo + '&TipeSegAcc=100').
            success(function (v, status, headers, config) {
                if (!v.success) {
                    $('#CompanyAccNo').val('');
                    me.AccNo('100');
                } 
            });
        } else { me.AccNo('100'); }
    });

    $("[name = 'BranchAccNo']").on('blur', function () {
        if ($('#BranchAccNo').val() || $('#BranchAccNo').val() != '') {
            var SegAccNo = $('#BranchAccNo').val();
            $http.post('gn.api/Organisasi/CheckCompanyAccNo?SegAccNo=' + SegAccNo + '&TipeSegAcc=200').
            success(function (v, status, headers, config) {
                if (!v.success) {
                    $('#BranchAccNo').val('');
                    me.AccNo('200');
                }
            });
        } else { me.AccNo('200'); }
    });
    $('#CompanyCode').blur(function () {
        var url = "gn.api/Organisasi/GetOrganizationByCompanyCode";
        $http.post(url, me.data).
        success(function (data, status, headers, config) {
        if (data.success) {
            //setTimeout(function () {
            me.data = data.data;
            me.LoadDataDetails();
            $("#CompanyCode").attr('disabled', 'disabled');
            //me.control.DisableCompanyCode = true;
            //}, 1000);
        } else {
            //MsgBox(data.message, MSG_ERROR);
        }
        }).
        error(function (data, status, headers, config) {
            console.log(data);
            MsgBox("Terjadi kesalahan dalam proses data. Hubungi SDMS Support", MSG_INFO);
        });
    });

    me.grid1 = new webix.ui({
        container: "wxorganizationdtl",
        view: "wxtable", css:"alternating",
        columns: [
            { id: "SeqNo", header: "No.", fillspace: false },
            { id: "BranchCode", header: "Kode Kantor Cabang", fillspace: false },
            { id: "BranchName", header: "Nama Kantor Cabang", fillspace: true },
            { id: "BranchAccNo", header: "Acc. Kantor Cabang", fillspace: true },
        ],
        checkboxRefresh: true,
        on: {
            onSelectChange: function () {
                if (me.grid1.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.grid1.getSelectedId().id);
                    me.control.DisableEditDtl = false;
                    me.control.DisableDelDtl = false;
                    me.Apply();
                }
            }
        }
    });
    me.start();

    Wx.OnValidation(me.saveData);
}

$(document).ready(function () {
    var options = {
        title: "Master Organisasi",
        xtype: "panels",
        toolbars: WxButtons,
        panels: [
            {
                name: "pnlInfo",
                title: "Informasi Kantor Pusat",
                items: [
					{ name: "CompanyCode", text: "Kode Kantor Pusat", cls: "span4", required: true, validasi: "required", maxlength: 15 },
					{ name: "CompanyName", text: "Nama Kantor Pusat", cls: "span4", required: true, validasi: "required", maxlength: 100 },
                    {
                        text: "Acc. Kantor Pusat",
                        type: "controls",
                        required: true,
                        items: [
                            { name: "CompanyAccNo", cls: "span2", placeHolder: "Acc. Kantor Pusat", type: "popup", btnName: "btnCompanyAccNo", required: true, validasi: "required", click: "AccNo(100)" },
                        ]
                    }
                ]
            },
            {
                name: "pnlB",
                title: "Informasi Kantor Cabang",
                items: [
                    { model: "detail.BranchCode", name: "BranchCode", text: "Kode Kantor Cabang", cls: "span4", required: true, validasi: "required", maxlength: 15 },
                    { model: "detail.BranchName", name: "BranchName", text: "Nama Kantor Cabang", cls: "span4", required: true, validasi: "required", maxlength: 100 },
                    {
                        text: "Acc Kantor Cabang",
                        type: "controls",
                        required: true,
                        items: [
                            { model: "detail.BranchAccNo", name: "BranchAccNo", cls: "span2", placeHolder: "Acc. Kantor Cabang", type: "popup", btnName: "btnCompanyAccNo", required: true, validasi: "required", click: "AccNo(200)" },
                            { model: "detail.CompanyCode", name: "CompanyCode", cls: "span4", type: "hidden" },
                        ]
                    },
                    {
                        type: "buttons", cls: "span4", items: [
                            { name: "btnSaveDtl", text: "Save", click: "SaveDtl()", disable: "control.DisableEditDtl && detail.BranchCode == undefined", cls: "btn btn-success", icon: "icon-save", },
                             { name: "btnDelDtl", text: "Delete", click: "DeleteDtl()", disable: "control.DisableDelDtl", cls: "btn btn-danger", icon: "icon-remove" },
                            { name: "btnCancelDtl", text: "Cancel", click: "CancelDtl()", disable: "control.DisableEditDtl && detail.BranchCode == undefined", cls: "btn btn-warning", icon: "icon-undo" },
                           
                        ]
                    },
                    {
                        name: "wxorganizationdtl",
                        type: "wxdiv",
                    }
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