"use strict"

function svMstBillingTypeController($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.printPreview = function () {
        var ReportId = "SvRpMst002";

        Wx.showPdfReport({
            id: ReportId,
            pparam: 'companycode',
            rparam: me.data.UserId,
            type: "devex"
        });
    }

    me.lookUpCustomer = function () {
        var lookup = Wx.klookup({
            name: "lookupCustomer",
            title: "Pelanggan",
            url: "sv.api/combo/SelectActiveCustomer",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { 'field': 'CustomerName', 'dir': 'asc' }
            ],
            columns: [
                { field: "CustomerCode", title: "Kode Pelanggan", width: 120 },
                { field: "CustomerName", title: "Nama Pelanggan", width: 260 },
                { field: "Address", title: "Alamat", width: 500 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.CustomerCode = data.CustomerCode;
            me.data.CustomerName = data.CustomerName;
            me.data.Address1 = data.Address1;
            me.data.Address3 = data.Address3;
            me.data.Address4 = data.Address4;
            me.Apply();
        });
    };

    $("#BillType").on("blur", function () {
        $http.post('sv.api/billtype/get', me.data)
        .success(function (data, status, headers, config) {
            if (data.success) {
                me.data = data.data;
                $('#BillType').attr('disabled', 'disabled');
                if (me.data.IsActive == true) $('#IsActive').attr('checked', true);
                else {
                    $('#IsActive').removeAttr('checked');
                }
                if (me.data.BillType == "F" || me.data.BillType == "W") {
                    me.data.CustomerName = data.dataCust.CustomerName;
                    me.data.Address1 = data.dataCust.Address1;
                    me.data.Address2 = data.dataCust.Address2;
                    me.data.Address3 = data.dataCust.Address3;
                    me.data.Address4 = data.dataCust.Address4;
                    $("#btnCustomerCode").removeAttr('disabled');
                    $("#CustomerCode").removeAttr('readonly');
                }
                else {
                    $("#btnCustomerCode").attr('disabled', 'disabled');
                    $("#CustomerCode").attr('readonly', 'readonly');
                }
                me.Apply();

                me.isLoadData = true;

                me.hasChanged = false;
                me.startEditing();
                me.isSave = false;
                $scope.$apply();
            }
        })
        .error(function (e) {
            console.log(e);
            MsgBox('Terdapat kesalahan proses data. Please contact sdms support...', MSG_INFO);
        });
    })

    me.delete = function () {
        MsgConfirm("Apakah anda yakin???", function (result) {
            if (result) {
                $http.post('sv.api/billtype/deletedata', me.data)
                    .success(function (e) {
                        if (e.success) {
                            Wx.Success("Data Deleted");
                            me.init();
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    })
                    .error(function (e) {
                        console.log(e);
                        MsgBox('Terdapat kesalahan proses data. Please contact sdms support...', MSG_INFO);
                    });
            }
            else return;
        });
    }

    $("#IsActive").on('change', function () {
        if ($('#IsActive').is(':checked')) {
            me.data.IsActive = true;
        }
        else {
            me.data.IsActive = false;
        }
        me.Apply();
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "BillTypeBrowse",
            title: "Pembayaran - Lookup",
            manager: MasterService,
            query: "BillTypeBrowse",
            defaultSort: "Description asc",
            columns: [
                    { field: "BillType", title: "Tipe Billing", width: 100 },
                    { field: "Description", title: "Keterangan", width: 200 },
                    { field: "CustomerCode", title: "Kode Pelanggan", width: 150 },
                    { field: "CustomerName", title: "Nama Pelanggan", width: 250 },
                    { field: "CustomerAddress", title: "Alamat", width: 800 },
                    { field: "StatusDesc", title: "Status", width: 100 }
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            $('#BillType').attr('disabled', 'disabled');
            me.isSave = false;
            if (me.data.IsActive == true) $('#IsActive').attr('checked', true);
            else {
                $('#IsActive').removeAttr('checked');
            }
            if (me.data.BillType == "F" || me.data.BillType == "W") {
                $("#btnCustomerCode").removeAttr('disabled');
                //$("#CustomerCode").removeAttr('readonly');
                $http.post('sv.api/billtype/getCust', me.data).
                    success(function (data, status, headers, config) {
                        if (data.success) {
                            me.data.CustomerName = data.data.CustomerName;
                            me.data.Address1 = data.data.Address1;
                            me.data.Address2 = data.data.Address2;
                            me.data.Address3 = data.data.Address3;
                            me.data.Address4 = data.data.Address4;

                        } else {
                            MsgBox(data.message, MSG_ERROR);
                        }
                    })
            }
            else {
                $("#btnCustomerCode").attr('disabled', 'disabled');
                $("#CustomerCode").attr('readonly', 'readonly');
            }
            me.isPrintAvailable = me.isLoadData = true;
            me.Apply();
        });
    }

    me.saveData = function () {
        if ($('#IsActive').is(':checked')) me.data.IsActive = true;
        else me.data.IsActive = false;
        $http.post('sv.api/billtype/save', me.data).
        success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                me.startEditing();

            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        })
     .error(function (e) {
         console.log(e);
         MsgBox('Terdapat kesalahan proses data. Please contact sdms support...', MSG_INFO);
     });
    }

    me.initialize = function () {
        $("#btnCustomerCode").attr("disabled", "disabled");
        $("#CustomerCode").attr('readonly', true);
        $('#IsActive').attr('checked', true);
        $('#BillType').removeAttr('disabled');
        me.data = {};
        me.data.IsActive = true;
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Tipe Pembayaran",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" }],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    {name: "CompanyCode", cls: "hide"},
                    { name: "BillType", text: "Tipe Billing", validasi:"required", required: true, maxlength: 15 },
                    { name: "Description", text: "Keterangan", validasi: "required", required: true, maxlength: 100 },
                    {
                        text: "Pelanggan", type: "controls",
                        items: [
                            { name: "CustomerCode", cls: "span4", text: "Kode Pelanggan", type: "popup", btnName: "btnCustomerCode", readonly: true, click: "lookUpCustomer()" },
                            { name: "CustomerName", cls: "span4", text: "Nama Pelanggan", readonly: true },
                        ]
                    },
                    { name: "Address1", text: "Alamat", readonly: true },
                    { name: "Address2", readonly: true },
                    { name: "Address3", readonly: true },
                    { name: "Address4", readonly: true },
                    { name: "IsActive", text: "Status", cls: "span4", type: "check", float: "left" }
                ]
            },
           
        ],
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstBillingTypeController");
    }
});