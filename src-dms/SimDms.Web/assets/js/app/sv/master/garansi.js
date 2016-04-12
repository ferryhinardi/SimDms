"use strict"

function svMstWarranty($scope, $http, $injector) {
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.printPreview = function () {
        Wx.loadForm();
        Wx.showForm({ url: "sv/master/warrantyprint" });
    }

    me.delete = function () {
        MsgConfirm("Apakah anda yakin???", function (result) {
            if (result) {
                $http.post('sv.api/garansi/deletedata', me.data)
                    .success(function (e) {
                        if (e.success) {
                            Wx.Success("Data Deleted");
                            me.init();
                        } else {
                            MsgBox(e.message, MSG_ERROR);
                        }
                    });
            }
            else return;
        });
    }

    me.saveData = function () {
        if (me.data.Odometer == 0) {
            MsgBox('Batasan KM tidak boleh sama dengan nol !!', MSG_INFO);
            return;
        }
        if (me.data.TimePeriod == 0) {
            MsgBox('Waktu tidak boleh sama dengan nol !!', MSG_INFO);
            return;
        }
        $http.post("sv.api/garansi/save", me.data).success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                me.startEditing();
                $('#BasicModel, #btnBasicModel, #OperationNo, #btnOperationNo').attr('disabled', true);
                me.isPrintAvailable = true;
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        });
           
    }

    $('#OperationNo').on('blur', function () {
        if (me.data.OperationNo == "" || me.data.OperationNo == null) return;
        if (me.data.BasicModel == "" || me.data.BasicModel == null) {
            MsgBox('Silahkan pilih Basic Model terlebih dahulu !!', MSG_INFO);
            me.data.OperationNo == "";
            return;
        }
        $http.post('sv.api/garansi/GetOperationNo', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.OperationNo = data.data.OperationNo;
                me.data.Description = data.data.Description;
                $http.post('sv.api/garansi/get', me.data).success(function (data, status, headers, config) {
                    if (data.success) {
                        me.lookupAfterSelect(data.data);
                        $('#BasicModel, #btnBasicModel, #OperationNo, #btnOperationNo').attr('disabled', true);
                        me.isPrintAvailable = true;
                    }
                });
            } else {
                me.data.OperationNo = me.data.Description = "";
                me.OperationNo();
            }
        });
    });

    me.OperationNo = function () {
        if (me.data.BasicModel == "" || me.data.BasicModel == null) {
            MsgBox('Silahkan pilih Basic Model terlebih dahulu !!', MSG_INFO);
            me.data.OperationNo == "";
            return;
        }
        var lookup = Wx.klookup({
            name: "OperationNo",
            title: "Jenis Pekerjaan",
            url: "sv.api/garansi/OperationNoBrowse",
            params: { basicModel: me.data.BasicModel },
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "OperationNo", title: "Jenis Pekerjaan", width: 150 },
                { field: "Description", title: "Keterangan" },
                { field: "Status", title: "Status", width: 90 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.OperationNo = data.OperationNo;
            me.data.Description = data.Description;
            me.Apply();
            $http.post('sv.api/garansi/get', me.data).success(function (data, status, headers, config) {
                if (data.success) {
                    me.lookupAfterSelect(data.data);
                    $('#BasicModel, #btnBasicModel, #OperationNo, #btnOperationNo').attr('disabled', true);
                    me.isPrintAvailable = true;
                }
            });
        });
    }

    $('#BasicModel').on('blur', function () {
        if (me.data.BasicModel == "" || me.data.BasicModel == null) return;
        $http.post('sv.api/garansi/GetBasicModel', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.BasicModel = data.data.RefferenceCode;
            } else {
                me.data.BasicModel = "";
                me.BasicModel();
            }
        });
    });

    me.BasicModel = function () {
        var lookup = Wx.klookup({
            name: "BasicModel",
            title: "Basic Model",
            url: "sv.api/garansi/BasicModelBrowse",
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "BasicModel", title: "Basic Model", width: 110 },
                { field: "TechnicalModelCode", title: "Technical Model Code", width: 110 },
                { field: "ModelDescription", title: "Model Description", width: 80 },
                { field: "Status", title: "Status", width: 80 }
            ]
        });
        lookup.dblClick(function (data) {
            me.data.BasicModel = data.BasicModel;
            me.Apply();
        });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "Warranty",
            title: "Campaign - Lookup",
            manager: MasterService,
            query: "CampaignOpen",
            columns: [
                { field: "BasicModel", title: "Basic Model" },
                { field: "OperationNo", title: "Jenis Pekerjaan" },
                { field: "Description", title: "Keterangan" },
                { field: "Odometer", title: "Batasan KM", template: '<div style="text-align:right;">#= kendo.toString(Odometer, "n0") #</div>' },
                { field: "TimePeriod", title: "Batasan Waktu", template: '<div style="text-align:right;">#= kendo.toString(TimePeriod, "n0") #</div>' },
                { field: "TimeDimDesc", title: "Satuan Waktu" },
                { field: "EffectiveDate", title: "Tanggal Efektif", type: "date", format: "{0:dd-MMM-yyyy}" },
                { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            $('#BasicModel, #btnBasicModel, #OperationNo, #btnOperationNo').attr('disabled', true);
            me.isPrintAvailable = true;
            me.Apply();
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.IsActive = true;
        me.data.Odometer = 0;
        me.data.TimePeriod = 0;
        me.data.EffectiveDate = me.now();
        me.data.TimeDim = "M";
        $('#BasicModel, #btnBasicModel, #OperationNo, #btnOperationNo').removeAttr('disabled');
        me.Apply();
        me.ReformatNumber();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Garansi",
        xtype: "panels",
        toolbars: [{ name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()" }],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    {name:"ProductType", cls: "hide"},
                    { name: "CompanyCode", cls: "hide" },
                    {
                        name: "BasicModel",
                        text: "Basic Model",
                        type: "popup",
                        btnName: "btnBasicModel",
                        required: true, validasi: "required", click: "BasicModel()"
                    },
                    {
                        text: "Jenis Pekerjaan",
                        type: "controls",
                        items: [
                            {
                                name: "OperationNo",
                                text: "Jenis Pekerjaan",
                                cls: "span2",
                                type: "popup",
                                required: true, validasi: "required",
                                btnName: "btnOperationNo", click: "OperationNo()"
                            },
                            { name: "Description", cls: "span6", text: "Keterangan", readonly: true },
                        ]
                    },
                    {
                        name: "Odometer",
                        text: "Batasan KM",        
                        cls: "span4 number-int",
                        required: true, validasi: "required", type: "int", min:0, max: 9999999999
                    },
                    {
                        name: "TimePeriod",
                        text: "Waktu",
                        cls: "span4 number-int",
                        required: true, validasi: "required", type: "int", min: 0, max: 999
                    },
                    {
                        name: "TimeDim", text: "Satuan", cls: "span4", type: "select",
                        items: [
                            { value: "D", text: "Hari" },
                            { value: "M", text: "Bulan" },
                            { value: "Y", text: "Tahun" },

                        ],
                        required:"required"
                    },
                    { name: "EffectiveDate", text: "Tgl. Efektif", cls: "span4", type: "ng-datepicker" },
                    {
                        name: "IsActive",
                        text: "Status",
                        cls: "span4",
                        type: "x-switch",
                        float: "left"
                    },
                ]
            },

        ],
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("svMstWarranty");
    }

});