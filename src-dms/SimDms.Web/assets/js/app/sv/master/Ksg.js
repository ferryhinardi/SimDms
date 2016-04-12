var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

function KSGController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.hasChanged = false;
        me.data.IsActive = "1";
        me.data.EffectiveDate = me.now();
        me.data.PdiFscSeq = 0;
        me.data.LaborRate = 0;
        me.data.RegularLaborAmount = 0;
        me.data.RegularMaterialAmount = 0;
        me.data.RegularTotalAmount = 0;
        $('#IsCampaign').prop('checked', false);
        me.data.IsCampaign = false;
    }

    $("[name = 'IsCampaign']").on('change', function () {
        me.data.IsCampaign = $('#IsCampaign').prop('checked');
        me.Apply();
    });

    me.TransmissionType = [
       { "value": 'AT', "text": 'Automatic Transmission' },
       { "value": 'MT', "text": 'Manual Transmission' },
    ];

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "KGSBrowse",
            title: "KGS Browse",
            manager: MasterService,
            query: "KSGOpen",
            defaultSort: "BasicModel asc",
            columns: [
                { field: "BasicModel", title: "Basic Model" },
                { field: "IsCampaignType", title: "PDI/FSC Type" },
                { field: "TransType", title: "Tipe Trans" },
                { field: "PdiFscSeq", title: "FS#" },
                { field: "RegularLaborAmount", title: "Nilai Labor" },
                { field: "RegularMaterialAmount", title: "Nilai Material" },
                { field: "RegularTotalAmount", title: "Total Nilai" },
                { field: "EffectiveDate", title: "Tgl. Efektif"},
                { field: "Description", title: "Keterangan" },
                { field: "Status", title: "Status" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.data.IsActive = result.IsActive == true ? "1" : "0";
                me.startEditing();
                me.Apply();
            }
        });
    }

    me.btnBasicModel = function () {
        var lookup = Wx.blookup({
            name: "MasterModul",
            title: "Master Model",
            manager: MasterService,
            query: "BasicKsgOpen",
            defaultSort: "BasicModel asc",
            columns: [
                { field: "BasicModel", title: "Basic Model", Width: "110px" },
                { field: "TechnicalModelCode", title: "Technical Model Code", Width: "110px" },
                { field: "ModelDescription", title: "Description", Width: "110px" },
                { field: "Status", title: "Is Active", Width: "80px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.BasicModel = result.BasicModel;
                me.Apply();
            }
        });
    }

    $("[name='LaborRate']").on('blur', function () {
        CalculateJumlah();
    });

    $("[name='RegularLaborAmount']").on('blur', function () {
        CalculateJumlah();
    });

    $("[name='RegularMaterialAmount']").on('blur', function () {
        CalculateJumlah();
    });

    function CalculateJumlah() {
        x = $("[name='LaborRate']").val();
        y = $("[name='RegularLaborAmount']").val();
        z = $("[name='RegularMaterialAmount']").val();
        x = x.split(',').join('');
        y = y.split(',').join('');
        z = z.split(',').join('');
        var m = (parseFloat(x) * parseFloat(y)) + parseFloat(z);
        $("[name='RegularTotalAmount']").val(m);
        me.data.RegularTotalAmount = (m);
    }

    me.saveData = function (e, param) {
        me.data.IsActive = me.data.IsActive == "1" ? true : false;
        $http.post('sv.api/ksg/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                console.log(e);
            });
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sv.api/ksg/deletedata', me.data).
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

    $("[name='BasicModel']").on('blur', function () {
        if (me.data.BasicModel != null) {
            $http.post('sv.api/ksg/BasicModel', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.BasicModel = data[0].BasicModel;
                       $('#BasicModel').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.BasicModel = "";
                       me.btnBasicModel();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
                   //console.log(e);
                   //alert('error');
               });
        }
    });

    me.printPreview = function () {
        Wx.loadForm();
        Wx.showForm({ url: "sv/master/PrintKsg" });
    }

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Pdi Fsc Rate (KSG)",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" }
        ],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    { name: "ProductType", cls: "hide" },
                    { name: "CompanyCode", cls: "hide" },
                    {
                        name: "BasicModel", cls: "span5", text: "Basic Model", btnName: "btnBasicModel", click: "btnBasicModel()", required: true, validasi: "required", type: "popup"
                    },
                    {
                        name: "IsCampaign", text: "Tipe PDI/FSC", cls: "span5", type: "check", float: "left", model: "data.IsCampaign"
                    },
                    {
                        name: "TransmissionType", text: "Tipe Transmission", type: "select2", cls: "span5", datasource: "TransmissionType", required: true, validasi: "required"
                    },
                    {
                        name: "PdiFscSeq", text: "FS#", cls: "span5 number-int full", required: true, validasi: "required"
                    },
                    { name: "EffectiveDate", text: "Tgl. Efektif", cls: "span4", type: "ng-datepicker" },
                    {
                        name: "LaborRate", text: "Labor Rate", cls: "span5 number full"
                    },
                    {
                        name: "RegularLaborAmount", text: "Nilai Labor", cls: "span5 number-int full", required: true, validasi: "required"
                    },
                    {
                        name: "RegularMaterialAmount", text: "Nilai Material", cls: "span5 number-int full", required: true, validasi: "required"
                    },
                    {
                        name: "RegularTotalAmount", text: "Total Niali", cls: "span5 number-int full", readonly: true,
                    },
                    {
                        name: "Description", text: "Keterangan", placeholder: "Keterangan", required: true, validasi: "required", cls: "span5", type: "textarea"
                    },
                    {
                        type: "optionbuttons", name: "Status", model: "data.IsActive", text: "Is Active",
                        items: [
                            { name: "1", text: "Aktif" },
                            { name: "0", text: "Tidak Aktif" },
                        ]
                    },


                ]
            },
        ]
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("KSGController");
    }

});


//$(document).ready(function () {
//    var options = {
//        title: "Pdi Fsc Rate (KSG)",
//        xtype: "panels",
//        toolbars: [
//            { name: "btnCreate", text: "New", icon: "icon-file" },
//            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
//            { name: "btnSave", text: "Save", icon: "icon-save", cls: "hide" },
//            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
//            { name: "btnDelete", text: "Delete", icon: "icon-remove", cls: "hide" },
//        ],
//        panels: [
//            {
//                name: "pnlRefService",
//                //title: "Service Information",
//                items: [
//                    { name: "ProductType", cls: "hide" },
//                    { name: "CompanyCode", cls: "hide" },
//                    {
//                        name: "BasicModel",
//                        cls: "span5",
//                        text: "Basic Model",
//                        type: "popup",
//                        readonly: true,
//                        btnName: "btnBasicModel",
//                        required: "required"
//                    },
//                    {
//                        name: "IsCampaign",
//                        text: "Tipe PDI/FSC (Is Campaign)",
//                        cls: "span5",
//                        type: "switch",
//                        float: "left"
//                    },
//                    {
//                        name: "TransmissionType",
//                        text: "Transmission Type",
//                        type: "select",
//                        cls: "span5",
//                        required: "required",
//                        items: [
//                            { value: 'AT', text: 'Automatic Transmission' },
//                            { value: 'MT', text: 'Manual Transmission' },
//                        ]
//                    },
//                    {
//                        name: "PdiFscSeq",
//                        text: "FS#",
//                        cls: "span5",
//                        required: "required"
//                    },
//                    { name: "EffectiveDate", text: "Effective Date", cls: "span4", type: "datepicker" },
//                    {
//                        name: "LaborRate",
//                        text: "Labor Rate",
//                        cls: "span5",
//                    },
//                    {
//                        name: "RegularLaborAmount",
//                        text: "Labor Amount",
//                        cls: "span5",
//                        required: "required"
//                    },
//                    {
//                        name: "RegularMaterialAmount",
//                        text: "Material Amount",
//                        cls: "span5",
//                        required: "required"
//                    },
//                    {
//                        name: "RegularTotalAmount",
//                        text: "Total Amount",
//                        cls: "span5",
//                        readonly: true,
//                    },
//                    {
//                        name: "Description",
//                        text: "Description",
//                        placeholder: "Keterangan",
//                        required: "required",
//                        cls: "span5",
//                        type:"textarea"
//                        //readonly: true,
//                    },

//                    {
//                        name: "IsActive",
//                        text: "Is Active",
//                        cls: "span5",
//                        type: "switch",
//                        float: "left"
//                    },


//                ]
//            },

//        ],
//    }

//    var widget = new SimDms.Widget(options);

//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/ksg/default', function (result) {
//            widget.default = result;
//            widget.populate(result);
//        });
//    });
    

    //$("#LaborRate").on("blur", function (e) {
    //    if ($("#LaborRate").val() == '') { document.getElementById("LaborRate").value = 0; } else {
    //        var LaborRate = parseFloat($("#LaborRate").val());
    //        var LaborAmount = parseFloat($("#RegularLaborAmount").val());
    //        var MaterialAmount = parseFloat($("#RegularMaterialAmount").val());

    //        //$("#RegularTotalAmount").val() = (LaborRate * LaborAmount) + MaterialAmount;
    //        var hasil = (LaborRate * LaborAmount) + MaterialAmount;
    //        //alert($("#RegularTotalAmount").val());
    //        //count(hasil);
    //        document.getElementById("RegularTotalAmount").value = hasil;
    //    }
        
    //});
    
//    $("#RegularLaborAmount").on("blur", function (e) {
//        if ($("#RegularLaborAmount").val() == '') { document.getElementById("RegularLaborAmount").value = 0; } else {
//            var LaborRate = parseFloat($("#LaborRate").val());
//            var LaborAmount = parseFloat($("#RegularLaborAmount").val());
//            var MaterialAmount = parseFloat($("#RegularMaterialAmount").val());

//            //$("#RegularTotalAmount").val() = (LaborRate * LaborAmount) + MaterialAmount;
//            var hasil = (LaborRate * LaborAmount) + MaterialAmount;
//            //alert($("#RegularTotalAmount").val());
//            //count(hasil);
//            document.getElementById("RegularTotalAmount").value = hasil;
//        }
//    });

//    $("#RegularMaterialAmount").on("blur", function (e) {
//        if ($("#RegularMaterialAmount").val() == '') { document.getElementById("RegularMaterialAmount").value = 0; } else {
//            var LaborRate = parseFloat($("#LaborRate").val());
//            var LaborAmount = parseFloat($("#RegularLaborAmount").val());
//            var MaterialAmount = parseFloat($("#RegularMaterialAmount").val());

//            //$("#RegularTotalAmount").val() = (LaborRate * LaborAmount) + MaterialAmount;
//            var hasil = (LaborRate * LaborAmount) + MaterialAmount;
//            //alert($("#RegularTotalAmount").val());
//            //count(hasil);
//            document.getElementById("RegularTotalAmount").value = hasil;
//        }
//    });

   

//    $("#btnBasicModel").on("click", function () {
//     //   loadData('btn1');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "BasicModel",
//            title: "Basic Model",
//            source: "sv.api/grid/BasicKsgOpen",
//            sortings: [[0, "asc"]],
//            columns: [
//                { field: "BasicModel", title: "Basic Model", sWidth: "110px" },
//                { field: "TechnicalModelCode", title: "Technical Model Code", sWidth: "110px" },
//                { field: "ModelDescription", title: "Description", sWidth: "110px" },
//                { field: "Status", title: "Is Active", sWidth: "80px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    $("#btnBrowse").on("click", function () {
//     //   loadData('browse');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "Browse",
//            title: "KSG",
//            source: "sv.api/grid/KSGOpen",
//            sortings: [[0, "asc"]],
//            columns: [
//                { field: "BasicModel", title: "Basic Model", sWidth: "110px" },
//                { field: "IsCampaignType", title: "PDI/FSC Type", sWidth: "110px" },
//                { field: "TransType", title: "Trans Type", sWidth: "110px" },
//                { field: "PdiFscSeq", title: "FS#", sWidth: "80px" },
//                { field: "RegularLaborAmount", title: "Labor Amount", sWidth: "80px" },
//                { field: "RegularMaterialAmount", title: "Material Amount", sWidth: "80px" },
//                { field: "RegularTotalAmount", title: "Total Value", sWidth: "80px" },
//                {
//                    field: "EffectiveDate", title: "Effective Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                { field: "Description", title: "Description", sWidth: "110px" },
//                { field: "Status", title: "Is Active", sWidth: "80px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    widget.lookup.onDblClick(function (e, data, name) {
//        widget.lookup.hide();
//        switch (name) {
//            case "Browse":
//                if (data.LaborRate == null) {
//                    data.LaborRate = 0;

//                }
//                if (data.RegularTotalAmount == null) {
//                    data.RegularTotalAmount = 0;
//                }
//                data.RegularTotalAmount = (data.LaborRate * data.RegularLaborAmount) + data.RegularMaterialAmount;
//                widget.populate($.extend({}, widget.default, data));
//                clear("dbclick");
//                widget.lookup.hide();
//                $("#TransmissionType").attr('disabled', 'disabled');
//                $("#PdiFscSeq").attr('readonly', 'readonly');
//                $("#btnOperationNo").attr('readonly', 'readonly');
//                $("#Description").removeAttr('readonly');
//                break;
//            case "BasicModel":
//                widget.populate($.extend({}, widget.default, data));
//                clear("dbclick");
//                $("#TransmissionType").removeAttr('disabled');
//                $("#PdiFscSeq").removeAttr('readonly');
//                $("#btnEdit").addClass('hide');
//                $("#btnSave").removeClass('hide');
//                widget.lookup.hide();
//                break;
//            default:
//                break;
//        }
//    });

    

//    $("#btnSave").on("click", saveData);

//    $("#btnDelete").on("click", deleteData);

//    function saveData(p) {
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            param.ChassisStartNo = parseFloat(param.ChassisStartNo);
//            param.ChassisEndNo = parseFloat(param.ChassisEndNo);
//            widget.post("sv.api/ksg/save", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data saved...");
//                    clear("new");
//                }
//            });
//        }
//    }




//    function deleteData() {
//        if (confirm("Apakah anda yakin???")) {
//            var param = $(".main .gl-widget").serializeObject();
//            widget.post("sv.api/ksg/deletedata", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data deleted...");
//                    clear("new");
//                } else {
//                    SimDms.Error("fail deleted...");
//                }
//            });
//        }
//    }

//    $('#btnCreate').on('click', function (e) {
//        clear("new");
//    });

//    $('#btnEdit').on('click', function (e) {
//        clear("btnEdit");

//    });


//    function clear(p) {
//        if (p == "clear") {
//            $("#btnSave").addClass("hide");
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//        } else if (p == "dbclick") {
//            $("#btnEdit").removeClass('hide');
//            $("#btnDelete").removeClass('hide');
//            $("#btnSave").addClass("hide");
//            $("#Odometer").attr("readonly", "readonly");
//            $("#TimePeriod").attr("readonly", "readonly");
//        } else if (p == "new") {
//            // widget.clearForm();
//            clearData();
//            $("#TransmissionType").removeAttr('disabled');
//            $("#btnSave").addClass("hide");
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//            $("#PdiFscSeq").removeAttr('readonly');
//        } else if (p == "btnEdit") {
//            $("#Description").removeAttr('readonly');
//            $("#OperationNo").removeAttr('readonly');
//            $("#btnEdit").addClass('hide');
//            $("#btnSave").removeClass('hide');
//            $("#btnOperationNo").removeAttr('disabled');
//        }
//    }
  
//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/ksg/default", function (result) {
//            widget.default = $.extend({
//                //Odometer: 0,
//                //TimePeriod: 0,
//            }, result);
//            widget.populate(widget.default);

//        });
//    }


//});