"use strict"

function svMstCampaign($scope, $http, $injector){
    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.printPreview = function () {
        Wx.loadForm();
        Wx.showForm({ url: "sv/master/campaignprint" });
    }

    $('#OperationNo').on('blur', function () {
        if (me.data.OperationNo == "" || me.data.OperationNo == null) return;
        $http.post('sv.api/campaign/GetOperationNo', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.OperationNo = data.data.OperationNo;
            } else {
                me.data.OperationNo = "";
                me.OperationNo();
            }
        });
    });

    $('#ComplainCode').on('blur', function () {
        if (me.data.ComplainCode == "" || me.data.ComplainCode == null) return;
        $http.post('sv.api/campaign/GetComplain', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.ComplainCode = data.data.RefferenceCode;
                if (me.data.DefectCode != "" || me.data.ChassisCode != "" || me.data.ChassisStartNo != "" || me.data.ChassisEndNo != "") {
                    $http.post('sv.api/campaign/Get', me.data).success(function (data, status, headers, config) {
                        if (data.success) {
                            me.lookupAfterSelect(data.data);
                            $('#ComplainCode, #DefectCode, #ChassisCode, #ChassisStartNo, #ChassisEndNo, #btnComplainCode, #btnDefectCode').attr('disabled', true);
                        }
                    });
                }
            } else {
                me.data.ComplainCode = "";
                me.ComplainCode();
            }
        });
    });

    $('#DefectCode').on('blur', function () {
        if (me.data.DefectCode == "" || me.data.DefectCode == null) return;
        $http.post('sv.api/campaign/GetDefect', me.data).success(function (data, status, headers, config) {
            if (data.success) {
                me.data.DefectCode = data.data.RefferenceCode;
                if (me.data.ComplainCode != "" || me.data.ChassisCode != "" || me.data.ChassisStartNo != "" || me.data.ChassisEndNo != "") {
                    $http.post('sv.api/campaign/Get', me.data).success(function (data, status, headers, config) {
                        if (data.success) {
                            me.lookupAfterSelect(data.data);
                            $('#ComplainCode, #DefectCode, #ChassisCode, #ChassisStartNo, #ChassisEndNo, #btnComplainCode, #btnDefectCode').attr('disabled', true);
                        }
                    });
                }
            } else {
                me.data.DefectCode = "";
                me.DefectCode();
            }
        });
    });

    $('#ChassisCode').on('blur', function () {
        if (me.data.ChassisCode == "" || me.data.ChassisCode == null) return;
        if (me.data.ComplainCode != "" || me.data.DefectCode != "" || me.data.ChassisStartNo != "" || me.data.ChassisEndNo != "") {
            $http.post('sv.api/campaign/Get', me.data).success(function (data, status, headers, config) {
                if (data.success) {
                    me.lookupAfterSelect(data.data);
                    $('#ComplainCode, #DefectCode, #ChassisCode, #ChassisStartNo, #ChassisEndNo, #btnComplainCode, #btnDefectCode').attr('disabled', true);
                }
            });
        }
    });

    $('#ChassisStartNo').on('blur', function () {
        if (me.data.ChassisStartNo == "" || me.data.ChassisStartNo == null) return;
        if (me.data.ComplainCode != "" || me.data.DefectCode != "" || me.data.ChassisCode != "" || me.data.ChassisEndNo != "") {
            $http.post('sv.api/campaign/Get', me.data).success(function (data, status, headers, config) {
                if (data.success) {
                    me.lookupAfterSelect(data.data);
                    $('#ComplainCode, #DefectCode, #ChassisCode, #ChassisStartNo, #ChassisEndNo, #btnComplainCode, #btnDefectCode').attr('disabled', true);
                }
            });
        }
    });

    $('#ChassisEndNo').on('blur', function () {
        if (me.data.ChassisEndNo == "" || me.data.ChassisEndNo == null) return;
        if (me.data.ComplainCode != "" || me.data.DefectCode != "" || me.data.ChassisCode != "" || me.data.ChassisStartNo != "") {
            $http.post('sv.api/campaign/Get', me.data).success(function (data, status, headers, config) {
                if (data.success) {
                    me.lookupAfterSelect(data.data);
                    $('#ComplainCode, #DefectCode, #ChassisCode, #ChassisStartNo, #ChassisEndNo, #btnComplainCode, #btnDefectCode').attr('disabled', true);
                }
            });
        }
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "Campaign",
            title: "Campaign - Lookup",
            manager: MasterService,
            query: "CampaignOpen",
            columns: [
                { field: "ComplainCode", title: "Kode Komplain" },
                { field: "DefectCode", title: "Kode Defect" },
                { field: "ChassisCode", title: "Kode Rangka" },
                { field: "ChassisStartNo", title: "Mulai No. Rangka", template: '<div style="text-align:right;">#= kendo.toString(ChassisStartNo, "n0") #</div>' },
                { field: "ChassisEndNo", title: "s/d No. Rangka", template: '<div style="text-align:right;">#= kendo.toString(ChassisEndNo, "n0") #</div>' },
                { field: "OperationNo", title: "Jenis Pekerjaan" },
                { field: "Description", title: "Keterangan" },
                { field: "CloseDate", title: "Tanggal Akhir", type: "date", format: "{0:dd-MMM-yyyy}" },
                { field: "Status", title: "Status" }
            ]
        });
        lookup.dblClick(function (data) {
            me.lookupAfterSelect(data);
            $('#ComplainCode, #DefectCode, #ChassisCode, #ChassisStartNo, #ChassisEndNo, #btnComplainCode, #btnDefectCode').attr('disabled', true);
            me.isPrintAvailable = true;
            me.Apply();
        });
    }

    me.delete = function () {
        MsgConfirm("Apakah anda yakin???", function (result) {
            if (result) {
                $http.post('sv.api/campaign/deletedata', me.data)
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
        $http.post("sv.api/campaign/save", me.data).success(function (data, status, headers, config) {
            if (data.success) {
                Wx.Success("Data saved...");
                me.startEditing();
                $('#ComplainCode, #DefectCode, #ChassisCode, #ChassisStartNo, #ChassisEndNo, #btnComplainCode, #btnDefectCode').attr('disabled', true);
                me.isPrintAvailable = true;
            } else {
                MsgBox(data.message, MSG_ERROR);
            }
        });

    }

    me.OperationNo = function () {
        var lookup = Wx.klookup({
            name: "OperationNo",
            title: "Jenis Pekerjaan",
            url: "sv.api/campaign/OperationNo",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { 'field': 'OperationNo', 'dir': 'asc' }
            ],
            columns:
                [{ field: "OperationNo", title: "Jenis Pekerjaan" },
                { field: "Description", title: "Keterangan" },
                { field: "IsActive", title: "Status" }]
        });
        lookup.dblClick(function (data) {
            me.data.OperationNo = data.OperationNo;
            me.Apply();
        });
    }

    me.DefectCode = function () {
        var lookup = Wx.klookup({
            name: "DefectCode",
            title: "Kode Defect",
            url: "sv.api/campaign/ReffService",
            serverBinding: true,
            pageSize: 10,
            params: { reffType: "DEFECTCD" },
            sort: [
                { 'field': 'RefferenceCode', 'dir': 'asc' }
            ],
            columns:
                [{ field: "RefferenceCode", title: "Kode Defect", width: 110 },
                { field: "Description", title: "Keterangan", width: 110 },
                { field: "DescriptionEng", title: "Keterangan (Eng.)", width: 80 }]
        });
        lookup.dblClick(function (data) {
            me.data.DefectCode = data.RefferenceCode;
            me.Apply();
        });
    }

    me.ComplainCode = function () {
        var lookup = Wx.klookup({
            name: "ComplainCode",
            title: "Kode Complain",
            url: "sv.api/campaign/ReffService",
            serverBinding: true,
            pageSize: 10,
            params: { reffType:  "COMPLNCD"},
            sort: [
                { 'field': 'RefferenceCode', 'dir': 'asc' }
            ],
            columns:
                [{ field: "RefferenceCode", title: "Kode Complain", width: 110 },
                { field: "Description", title: "Keterangan", width: 110 },
                { field: "DescriptionEng", title: "Keterangan (Eng.)", width: 80 }]
        });
        lookup.dblClick(function (data) {
            me.data.ComplainCode = data.RefferenceCode;
            me.Apply();
        });
    }

    me.initialize = function () {
        me.data = {};
        me.data.CloseDate = me.now();
        me.data.IsActive = true;
        $('#ComplainCode, #DefectCode, #ChassisCode, #ChassisStartNo, #ChassisEndNo, #btnComplainCode, #btnDefectCode').removeAttr('disabled');
        me.Apply();
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Campaign",
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
                        name: "ComplainCode",
                        cls: "span4",
                        text: "Kode Complain",
                        type: "popup",
                        required: true, validasi: "required",
                        btnName: "btnComplainCode", click : "ComplainCode()"
                    },
                    {
                        name: "DefectCode",
                        text: "Kode Defect",
                        cls: "span4",
                        type: "popup",
                        required: true, validasi: "required",
                        btnName: "btnDefectCode", click: "DefectCode()"
                    },
                    {
                        text: "Rangka",
                        type: "controls",
                        items: [
                            {
                                name: "ChassisCode",
                                cls: "span4",
                                text: "Rangka",
                                required: true, validasi: "required", maxlength: 15
                            },
                            {
                                name: "ChassisStartNo",
                                cls: "span2 number-int",
                                text: "No Rangka Awal",
                                required: true, validasi: "required", maxlength: 10
                            },
                            {
                                name: "ChassisEndNo",
                                cls: "span2 number-int",
                                text: "No Rangka Akhir",
                                required: true, validasi: "required", maxlength: 10
                            },
                        ]
                    },
                    {
                        name: "OperationNo",
                        text: "Jenis Pekerjaan",
                        type: "popup",
                        required: true, validasi: "required",
                        btnName: "btnOperationNo", click: "OperationNo()"
                    },
                    {
                        name: "Description",
                        text: "Keterangan",
                        placeholder: "Keterangan",
                        cls: "span4",
                        required: true, validasi: "required", maxlength: 100
                    },
                    { name: "CloseDate", text: "Tgl. Akhir", cls: "span4", type: "ng-datepicker" },
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
        SimDms.Angular("svMstCampaign");
    }

    /*
    var widget = new SimDms.Widget(options);

    widget.default = {};

    widget.render(function () {
        $.post('sv.api/campaign/default', function (result) {
            widget.default = result;
            widget.populate(result);

        });
    });

    $("#btnOperationNo").attr("disabled", "disabled");
    $("#btnDefectCode").attr("disabled", "disabled");

    $("#btnComplainCode").on("click", function () {
      //  loadData('btn1');
        var param = $(".main .gl-widget").serializeObject();
        widget.lookup.init({
            name: "ComplainCode",
            title: "Complain Code",
            source: "sv.api/grid/ComplainOpen",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "ComplainCode", sTitle: " Complain Code", sWidth: "110px" },
                { mData: "Keterangan", sTitle: "Description", sWidth: "110px" },
                { mData: "DescriptionEng", sTitle: "Description (Eng.)", sWidth: "80px" },
            ]
        });
        widget.lookup.show();
    });


    $("#btnDefectCode").on("click", function () {
       // loadData('btn2');
        var param = $(".main .gl-widget").serializeObject();
        widget.lookup.init({
            name: "DefectCode",
            title: "Defect Code",
            source: "sv.api/grid/DefectOpen",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "DefectCode", sTitle: "Defect Code", sWidth: "110px" },
                { mData: "Keterangan", sTitle: "Description", sWidth: "110px" },
                { mData: "DescriptionEng", sTitle: "Description (Eng.)", sWidth: "80px" },
            ]
        });
        widget.lookup.show();
    });

    $("#btnOperationNo").on("click", function () {
       // loadData('btn3');
        var param = $(".main .gl-widget").serializeObject();
        widget.lookup.init({
            name: "OperationNo",
            title: "Operation No",
            source: "sv.api/grid/OperationOpen",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "OperationNo", sTitle: "Defect Code", sWidth: "110px" },
                { mData: "Keterangan", sTitle: "Description", sWidth: "110px" },
                { mData: "Status", sTitle: "Is Active", sWidth: "80px" },
            ]
        });
        widget.lookup.show();
    });


    $("#btnBrowse").on("click", function () {
      //  loadData('browse');
        var param = $(".main .gl-widget").serializeObject();
        widget.lookup.init({
            name: "Browse",
            title: "Campaign",
            source: "sv.api/grid/CampaignOpen",
            sortings: [[0, "asc"]],
            columns: [
                { mData: "ComplainCode", sTitle: "Complain Code", sWidth: "110px" },
                { mData: "DefectCode", sTitle: "Defect Code", sWidth: "110px" },
                { mData: "ChassisCode", sTitle: "Chassis Code", sWidth: "110px" },
                { mData: "ChassisStartNo", sTitle: "Chassis Start No", sWidth: "80px" },
                { mData: "ChassisEndNo", sTitle: "Chassis End No", sWidth: "80px" },
                { mData: "OperationNo", sTitle: "Operation No", sWidth: "80px" },
                { mData: "Description", sTitle: "Description", sWidth: "80px" },
                {
                    mData: "CloseDate", sTitle: "Close Date", sWidth: "130px",
                    mRender: function (data, type, full) {
                        return moment(data).format('DD MMM YYYY - HH:mm');
                    }
                },
                { mData: "Status", sTitle: "Is Active", sWidth: "80px" },
            ]
        });
        widget.lookup.show();
    });

    widget.lookup.onDblClick(function (e, data, name) {
        widget.lookup.hide();
        switch (name) {
            case "Browse":
                widget.populate($.extend({}, widget.default, data));
                clear("dbclick");
                var wkt = moment(data["CloseDate"]).format("HH:mm");
                data["CloseDate"] = wkt;
                widget.lookup.hide();
                $("#btnOperationNo").attr('disabled', 'disabled');
                $("#ChassisCode").attr('readonly', 'readonly');
                $("#ChassisStartNo").attr('readonly', 'readonly');
                $("#ChassisEndNo").attr('readonly', 'readonly');
                $("#btnOperationNo").attr('readonly', 'readonly');
                $("#Description").attr('readonly', 'readonly');
                break;
            case "OperationNo":
                widget.populate($.extend({}, widget.default, data));
                //clear("dbclick");
                //$("#ChassisCode").attr('readonly', 'readonly');
                //$("#ChassisStartNo").attr('readonly', 'readonly');
                //$("#ChassisEndNo").attr('readonly', 'readonly');
                $("#Description").removeAttr('readonly');
                $("#btnOperationNo").removeAttr('disabled');
                $("#btnSave").removeClass('hide');
                $("#btnEdit").addClass('hide');
                break;
            case "DefectCode":
                widget.populate($.extend({}, widget.default, data));
                clear("dbclick");
                $("#ChassisCode").removeAttr('readonly');
                $("#ChassisStartNo").removeAttr('readonly');
                $("#ChassisEndNo").removeAttr('readonly');
                $("#btnOperationNo").removeAttr('disabled');
                $("#Description").removeAttr('readonly');
                $("#TimePeriod").removeAttr('readonly');
                $("#Odometer").removeAttr('readonly');
                $("#btnSave").removeClass('hide');
                $("#btnEdit").addClass('hide');
                break;
            case "ComplainCode":
                widget.populate($.extend({}, widget.default, data));
                clear("dbclick");
                $("#btnDefectCode").removeAttr('disabled');
                $("#btnSave").removeClass('hide');
                $("#btnEdit").addClass('hide');
                break;
            default:
                break;
        }
    });


    $("#RefferenceType").on("change", function e() {
        clear("typeonchange");
    });

    $("#btnSave").on("click", saveData);

    $("#btnDelete").on("click", deleteData);

    function saveData(p) {
        var form = $(".main form");
        var param = form.serializeObject();
        var valid = form.valid();
        param.ChassisStartNo = parseFloat(param.ChassisStartNo);
        param.ChassisEndNo = parseFloat(param.ChassisEndNo);
        if ($("#ChassisStartNo").val() == 0 || $("#ChassisStartNo").val() < 0) {
            alert("chassis start no tidak boleh 0 atau lebih kecil!");
            return false;
        }
        else if ($("#ChassisEndNo").val() == 0 || $("#ChassisEndNo").val() < 0) {
            alert("chassis end no tidak boleh 0 atau lebih kecil!");
            return false;
        }
        else if (valid) {
            widget.post("sv.api/campaign/save", param, function (result) {
                if (result.success) {
                    SimDms.Success("data saved...");
                    clear("new");
                }
            });
        }
        //if ($("#ComplainCode").val() == null || $("#ComplainCode").val() == "") {
        //    alert("Complain Code wajib di isi!");
        //    return false;
        //} else if ($("#DefectCode").val() == 0 || $("#DefectCode").val() == null) {
        //    alert("Defect Code wajib di isi!");
        //    return false;
        //}  else if ($("#ChassisCode").val() == 0 || $("#ChassisCode").val() == null) {
        //    alert("Chassis Code wajib di isi!");
        //    return false;
        //} else if ($("#ChassisStartNo").val() == 0 || $("#ChassisStartNo").val() < 0) {
        //    alert("Chassis Start No tidak boleh 0 atau lebih kecil!");
        //    return false;
        //} else if ($("#ChassisEndNo").val() == 0 || $("#ChassisEndNo").val() < 0) {
        //    alert("Chassis End No tidak boleh 0 atau lebih kecil!");
        //    return false; TimeDim
        //} else if ($("#OperationNo").val() == null || $("#OperationNo").val() == "") {
        //    alert("Operation No wajib di isi!");
        //    return false; Odometer
        //} else {
            
        //}

    }



    function deleteData() {
        if (confirm("Apakah anda yakin???")) {
            var param = $(".main .gl-widget").serializeObject();
            widget.post("sv.api/campaign/deletedata", param, function (result) {
                if (result.success) {
                    SimDms.Success("data deleted...");
                    clear("new");
                } else {
                    SimDms.Error("fail deleted...");
                }
            });
        }
    }

    $('#btnCreate').on('click', function (e) {
        clear("new");
    });

    $('#btnEdit').on('click', function (e) {
        clear("btnEdit");
    });


    function clear(p) {
        if (p == "clear") {
            $("#btnSave").addClass("hide");
            $("#btnEdit").addClass("hide");
            $("#btnDelete").addClass("hide");
        } else if (p == "dbclick") {
            $("#btnEdit").removeClass('hide');
            $("#btnDelete").removeClass('hide');
            $("#btnSave").addClass("hide");
            $("#Odometer").attr("readonly", "readonly");
            $("#TimePeriod").attr("readonly", "readonly");
        }  else if (p == "new") {

            // widget.clearForm();
            clearData();
            $("#btnSave").addClass("hide");
            $("#btnEdit").addClass("hide");
            $("#btnDelete").addClass("hide");
            $("#TimePeriod").attr('readonly', 'readonly');
            $("#Odometer").attr('readonly', 'readonly');
            $("#btnOperationNo").attr('disabled', 'disabled');
            $("#ChassisCode").attr('readonly', 'readonly');
            $("#ChassisStartNo").attr('readonly', 'readonly');
            $("#ChassisEndNo").attr('readonly', 'readonly');
            $("#OperationNo").attr('readonly', 'readonly');
            $("#Description").attr('readonly', 'readonly');
            $("#btnEdit").addClass("hide");
        } else if (p == "btnEdit") {
            $("#Description").removeAttr('readonly');
            $("#OperationNo").removeAttr('readonly');
            $("#btnSave").removeClass('hide');
            $("#btnOperationNo").removeAttr('disabled');
            $("#btnEdit").addClass("hide");
        }
    }
    $("#btnOperationNo").attr('disabled', 'disabled');
    $("#ChassisCode").attr('readonly', 'readonly');
    $("#ChassisStartNo").attr('readonly', 'readonly');
    $("#ChassisEndNo").attr('readonly', 'readonly');
    $("#btnOperationNo").attr('readonly', 'readonly');
    $("#Description").attr('readonly', 'readonly');
    function clearData() {
        widget.clearForm();
        widget.post("sv.api/campaign/default", function (result) {
            widget.default = $.extend({
                Odometer: 0,
                TimePeriod: 0,
            }, result);
            widget.populate(widget.default);
           
        });
    }

    */
});