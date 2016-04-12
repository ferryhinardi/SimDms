var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

function RegCampaignController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        $("#tanda").attr("disabled", "disabled");
        $("#ProductivityMechanic").attr("disabled", "disabled");
        $("#ProductivityStall").attr("disabled", "disabled");

        me.data.TotalUnitService = "0.00";
        me.data.TotalStall = "0";
        me.data.TotalMechanic = "0.00";
        me.data.TotalWorkingDays = "0.00";
        me.data.ProductivityMechanic = "0.00";
        me.data.ProductivityStall = "0.00";
        me.data.TotalLift = "0";
        me.data.HourlyLaborRate = "0.00";
        me.data.OverheadCost = "0.00";
        me.data.ServiceAmount = "0.00";
        me.data.SMRTarget = "0.00";
        me.data.DasMonthTarget = "0";
        me.data.DasDailyTarget = "0";
        me.data.tanda = "        /";
    }

    $("#PeriodYear").on("blur", function () {
        if (($("#PeriodYear").val() == '')) {
            alert("Tahun tidak boleh kosong atau berisi huruf");
        }
    });
    $("#PeriodMonth").on("blur", function () {
        if (($("#PeriodMonth").val() == '')) {
            alert("Bulan tidak boleh kosong atau berisi huruf");
        }
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "TargetBrowse",
            title: "Master Target Browse",
            manager: MasterService,
            query: "TargetBrowse",
            defaultSort: "Period asc",
            columns: [
                { field: "Period", Title: "Period", Width: "110px" },
                { field: "ProductivityMechanic", Title: "Productivity Mechanic", Width: "80px" },
                { field: "ProductivityStall", Title: "Productivity Stall", Width: "80px" },
                { field: "TotalUnitService", Title: "Total Unit Service", Width: "80px" },
                { field: "TotalWorkingDays", Title: "Total Working Days", Width: "80px" },
                { field: "TotalMechanic", Title: "Total Mechanic", Width: "80px" },
                { field: "TotalStall", Title: "Total Stall", Width: "80px" },
                { field: "TotalLift", Title: "Total Lift", Width: "80px" },
                { field: "HourlyLaborRate", Title: "Hourly Labor Rate", Width: "80px" },
                { field: "OverheadCost", Title: "Overhead Cost", Width: "80px" },
                { field: "ServiceAmount", Title: "Service Amount", Width: "80px" },
                { field: "DasMonthTarget", Title: "Monthly Target", Width: "80px" },
                { field: "DasDailyTarget", Title: "Daily Target", Width: "80px" },
                { field: "SMRTarget", Title: "SMR Target", Width: "80px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.startEditing();
                me.Apply();
            }
        });
    }

    $("#TotalUnitService").on("blur", function () {
        CalculateProductivitas();
    });
    
    $("#TotalStall").on("blur", function () {
        CalculateProductivitas();
    });

    $("#TotalMechanic").on("blur", function () {
        CalculateProductivitas();
    });

    $("#TotalWorkingDays").on("blur", function () {
        CalculateProductivitas();
    });

    function CalculateProductivitas() {
        var param = $(".main .gl-widget").serializeObject();
        var unit = parseFloat(param.TotalUnitService);
        var stall = parseFloat(param.TotalStall);
        var mech = parseFloat(param.TotalMechanic);
        var days = parseFloat(param.TotalWorkingDays);

        jum1 = 0;
        jum2 = 0;
        if (mech > 0 && days > 0) {
            var prodmech = 0;
            prodmech = (unit / (mech * days));
            var temp1 = prodmech.toString();
            var jum1 = temp1.substring(0, 4);
            // alert(jum1);
        }
        if (stall > 0 && days > 0) {
            var prodstal = 0;
            prodstal = (unit / (stall * days));
            var temp2 = prodstal.toString();
            var jum2 = temp2.substring(0, 4);
            //alert(jum2);
        }


        $("#ProductivityMechanic").val(jum1);
        $("#ProductivityStall").val(jum2);
    }

    me.saveData = function (e, param) {
        me.data.TotalUnitService = parseFloat(param.TotalUnitService);
        me.data.TotalStall = parseFloat(param.TotalStall);
        me.data.TotalMechanic = parseFloat(param.TotalMechanic);
        me.data.TotalWorkingDays = parseFloat(param.TotalWorkingDays);
        // param.ProductivityMechanic = (param.ProductivityMechanic).toString();
        //param.ProductivityStall = (param.ProductivityStall).toString();
        me.data.TotalLift = parseFloat(param.TotalLift);
        me.data.HourlyLaborRate = parseFloat(param.HourlyLaborRate);
        me.data.OverheadCost = parseFloat(param.OverheadCost);
        me.data.ServiceAmount = parseFloat(param.ServiceAmount);
        me.data.SMRTarget = parseFloat(param.SMRTarget);
        me.data.DasMonthTarget = parseFloat(param.DasMonthTarget);
        me.data.DasDailyTarget = parseFloat(param.DasDailyTarget);
        $http.post('sv.api/target/save', me.data).
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
                $http.post('sv.api/targer/deletedata', me.data).
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

    me.printPreview = function () {
        BootstrapDialog.show({
            message: $(
                '<div class="container">' +

                '<div class="row">' +

                'Priode &nbsp<input type="textbox" name="Priode" id="Priode"></div>'),
            closable: false,
            draggable: true,
            type: BootstrapDialog.TYPE_INFO,
            title: 'Print Pesanan Penjualan',
            buttons: [{
                label: ' Print',
                cssClass: 'btn-primary icon-print',
                action: function (dialogRef) {
                    alert($('input[name=Priode]').val());
                    var Priode = $('input[name=Priode]').val();
                    var ReportId = 'SvRpMst013';
                    var par = [
                        Priode,
                        1
                    ]
                    var rparam = 'Print Target'

                    Wx.showPdfReport({
                        id: ReportId,
                        pparam: par.join(','),
                        rparam: rparam,
                        type: "devex"
                    });
                    dialogRef.close();
                }
            }, {
                label: ' Cancel',
                cssClass: 'btn-warning icon-remove',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
        });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Target",
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
                            {
                                text: "Period (yyyy/mm)",
                                type: "controls",
                                cls: "span4 full",
                                required: true,
                                items: [
                                    {
                                        name: "PeriodYear", cls: "span3 int", text: "yyyy", maxlength: "4", required: true, validasi: "required"
                                    },
                                    {
                                        name: "tanda", cls: "span2",
                                    },
                                    {

                                        name: "PeriodMonth", cls: "span3 int", text: "mm", maxlength: "2", required: true, validasi: "required"
                                    },

                                ]
                            },
                            {
                                name: "TotalUnitService", text: "Total Unit Service", cls: "span4 number", required: true, validasi: "required"
                            },
                            {
                                name: "TotalStall", text: "Total Stall", cls: "span4 number", required: true, validasi: "required"
                            },
                            {
                                name: "TotalMechanic", text: "Total Mechanic", cls: "span4 number", required: true, validasi: "required"
                            },
                            {
                                name: "TotalWorkingDays", text: "Total Working Days", cls: "span4 number", required: true, validasi: "required"
                            },
                            {
                                name: "ProductivityMechanic", text: "Productivity Mechanic", cls: "span4 number"
                            },
                            {
                                name: "ProductivityStall", text: "Productivity Stall", cls: "span4 number"
                            },
                            {
                                name: "TotalLift", text: "Total Lift", cls: "span4 number", required: true, validasi: "required"
                            },
                            {
                                name: "HourlyLaborRate", text: "Hourly Labor Rate", cls: "span4 number", required: true, validasi: "required"
                            },
                            {
                                name: "OverheadCost", text: "Overhead Cost", cls: "span4 number", required: true, validasi: "required"
                            },
                            {
                                name: "ServiceAmount", text: "Service Amount", cls: "span4 number", required: true, validasi: "required"
                            },
                            {
                                name: "SMRTarget", text: "SMR Target", cls: "number", required: true, validasi: "required"
                            },


                        ]
                    },
                    {
                        name: "pnlTarget",
                        title: "Max Target Dash Board",
                        items: [
                            {
                                name: "DasMonthTarget", cls: "span4 number", text: "Monthly Target", maxlength: "4", required: true, validasi: "required"
                            },
                            {
                                name: "DasDailyTarget", cls: "span4 number", text: "Daily Target", required: true, validasi: "required"
                            },
                        ]
                    }

                ],
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("RegCampaignController");
    }

});

//$(document).ready(function () {
//    var options = {
//        title: "Target",
//        xtype: "panels",
//        toolbars: [
//            { name: "btnCreate", text: "New", icon: "icon-file" },
//            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
//            { name: "btnSave", text: "Save", icon: "icon-save" },
//            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
//            { name: "btnDelete", text: "Delete", icon: "icon-remove", cls: "hide" },
//        ],
//        panels: [
//            {
//                name: "pnlRefService",
//                //title: "Service Information",
//                items: [
//                    {
//                        text: "Period (yyyy/mm)",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "PeriodYear",
//                                cls: "span2",
//                                text: "yyyy",
//                                maxlength: "4",
//                                required: "required"
//                            },
//                            {

//                                name: "tanda",
//                                cls: "span1",
                                
//                            },
//                            {

//                                name: "PeriodMonth",
//                                cls: "span2",
//                                text: "mm",
//                                maxlength: "2",
//                                required: "required"
//                            },

//                        ]
//                    },
//                    {
//                        name: "TotalUnitService",
//                        text: "Total Unit Service",
//                        cls: "span4 number",
//                        required: "required"
//                    },
//                    {
//                        name: "TotalStall",
//                        text: "Total Stall",
//                        cls: "span4 number",
//                        required: "required"
//                    },
//                    {
//                        name: "TotalMechanic",
//                        text: "Total Mechanic",
//                        cls: "span4 number",
//                        required: "required"
//                    },
//                    {
//                        name: "TotalWorkingDays",
//                        text: "Total Working Days",
//                        cls: "span4 number",
//                        required: "required"
//                    },
//                    {
//                        name: "ProductivityMechanic",
//                        text: "Productivity Mechanic",
//                        cls: "span4 number"
//                    },
//                    {
//                        name: "ProductivityStall",
//                        text: "Productivity Stall",
//                        cls: "span4 number"
//                    },
//                    {
//                        name: "TotalLift",
//                        text: "Total Lift",
//                        cls: "span4 number",
//                        required: "required"
//                    },
//                    {
//                        name: "HourlyLaborRate",
//                        text: "Hourly Labor Rate",
//                        cls: "span4 number",
//                        required: "required"
//                    },
//                    {
//                        name: "OverheadCost",
//                        text: "Overhead Cost",
//                        cls: "span4 number",
//                        required: "required"
//                    },
//                    {
//                        name: "ServiceAmount",
//                        text: "Service Amount",
//                        cls: "span4 number",
//                        required: "required"
//                    },
//                    {
//                        name: "SMRTarget",
//                        text: "SMR Target",
//                        cls: "number",
//                        required: "required"
//                    },

                   
//                ]
//            },
//            {
//                name: "pnlTarget",
//                title: "Max Target Dash Board",
//                items: [
//                    {
//                        name: "DasMonthTarget",
//                        cls: "span4 number",
//                        text: "Monthly Target",
//                        maxlength: "4",
//                        required: "required"
//                    },
//                    {
//                        name: "DasDailyTarget",
//                        cls: "span4 number",
//                        text: "Daily Target",
//                        required: "required"
//                    },
//                ]
//            }
            
//        ],
//    }

//    var widget = new SimDms.Widget(options);
//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/target/default', function (result) {
//            widget.default = result;
//            widget.populate(result);

//        });
//    });

//    $("#tanda").attr("disabled", "disabled");
//    $("#ProductivityMechanic").attr("disabled", "disabled");
//    $("#ProductivityStall").attr("disabled", "disabled");
    
//    $("#PeriodYear").on("blur", function () 
//    { 
//        if (($("#PeriodYear").val() == '')) {
//            alert("Tahun tidak boleh kosong atau berisi huruf");
//        }
//    });
//    $("#PeriodMonth").on("blur", function () {
//        if (($("#PeriodMonth").val() == '')) {
//            alert("Bulan tidak boleh kosong atau berisi huruf");
//        }
//    });
//    $("#btnBrowse").on("click", function () {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "Stall",
//            title: "Stall",
//            source: "sv.api/grid/TargetBrowse",
//            sortings: [[0, "DESC"]],
//            columns: [
//                { mData: "Period", sTitle: "Period", sWidth: "110px" },
//                { mData: "ProductivityMechanic", sTitle: "Productivity Mechanic", sWidth: "80px" },
//                { mData: "ProductivityStall", sTitle: "Productivity Stall", sWidth: "80px" },
//                { mData: "TotalUnitService", sTitle: "Total Unit Service", sWidth: "80px" },
//                { mData: "TotalWorkingDays", sTitle: "Total Working Days", sWidth: "80px" },
//                { mData: "TotalMechanic", sTitle: "Total Mechanic", sWidth: "80px" },
//                { mData: "TotalStall", sTitle: "Total Stall", sWidth: "80px" },
//                { mData: "TotalLift", sTitle: "Total Lift", sWidth: "80px" },
//                { mData: "HourlyLaborRate", sTitle: "Hourly Labor Rate", sWidth: "80px" },
//                { mData: "OverheadCost", sTitle: "Overhead Cost", sWidth: "80px" },
//                { mData: "ServiceAmount", sTitle: "Service Amount", sWidth: "80px" },
//                { mData: "DasMonthTarget", sTitle: "Monthly Target", sWidth: "80px" },
//                { mData: "DasDailyTarget", sTitle: "Daily Target", sWidth: "80px" },
//                { mData: "SMRTarget", sTitle: "SMR Target", sWidth: "80px" },
//            ]
//        });
//        widget.lookup.show();
//        });

//        widget.lookup.onDblClick(function (e, data, name) {
//            widget.populate($.extend({}, widget.default, data));
//            widget.lookup.hide();
//            clear("dbclick");
//        });
        
//        $("#TotalUnitService").on("blur", function () {
//            CalculateProductivitas();
//        });
//        $("#TotalStall").on("blur", function () {
//            CalculateProductivitas();
//        });
//        $("#TotalMechanic").on("blur", function () {
//            CalculateProductivitas();
//        });
//        $("#TotalWorkingDays").on("blur", function () {
//            CalculateProductivitas();
//        });
//        function CalculateProductivitas()
//        {
//            var param = $(".main .gl-widget").serializeObject();
//            var unit = parseFloat(param.TotalUnitService);
//            var stall = parseFloat(param.TotalStall);
//            var mech = parseFloat(param.TotalMechanic);
//            var days = parseFloat(param.TotalWorkingDays);
            
//            jum1 = 0;
//            jum2 = 0;
//            if (mech > 0 && days > 0) {
//                var prodmech = 0;
//                prodmech = (unit / (mech * days));
//                var temp1 = prodmech.toString();
//                var jum1 = temp1.substring(0, 4);
//               // alert(jum1);
//            } 
//            if (stall > 0 && days > 0) {
//                var prodstal = 0;
//                prodstal = (unit / (stall * days));
//                var temp2 = prodstal.toString();
//                var jum2 = temp2.substring(0, 4);
//                //alert(jum2);
//            }
            

//            $("#ProductivityMechanic").val(jum1);
//            $("#ProductivityStall").val(jum2);
//        }
//    $("#btnSave").on("click", saveData);
//    $("#btnDelete").on("click", deleteData);
//    function saveData(p) {
        
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            param.TotalUnitService = parseFloat(param.TotalUnitService);
//            param.TotalStall = parseFloat(param.TotalStall);
//            param.TotalMechanic = parseFloat(param.TotalMechanic);
//            param.TotalWorkingDays = parseFloat(param.TotalWorkingDays);
//            // param.ProductivityMechanic = (param.ProductivityMechanic).toString();
//            //param.ProductivityStall = (param.ProductivityStall).toString();
//            param.TotalLift = parseFloat(param.TotalLift);
//            param.HourlyLaborRate = parseFloat(param.HourlyLaborRate);
//            param.OverheadCost = parseFloat(param.OverheadCost);
//            param.ServiceAmount = parseFloat(param.ServiceAmount);
//            param.SMRTarget = parseFloat(param.SMRTarget);
//            param.DasMonthTarget = parseFloat(param.DasMonthTarget);
//            param.DasDailyTarget = parseFloat(param.DasDailyTarget);
//            widget.post("sv.api/target/save", param, function (result) {
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
//            widget.post("sv.api/target/deletedata", param, function (result) {
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
//            $("#StallCode").attr("readonly", "readonly");
//            $("#Description").attr("readonly", "readonly");
//            $("#btnEdit").removeClass('hide');
//            $("#btnDelete").removeClass('hide');
//            $("#btnSave").addClass("hide");
//        } else if (p == "new") {
//            clearData();
//            $("#StallCode").removeAttr('readonly');
//            $("#btnSave").removeClass("hide");
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//            $("#Description").removeAttr('readonly');
//        } else if (p == "btnEdit") {
//            $("#StallCode").removeAttr('readonly');
//            $("#Description").removeAttr('readonly');
//            $("#btnSave").removeClass('hide');
//        } 
//    }
//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/target/default", function (result) {
//            widget.default = $.extend({}, result);
//            widget.populate(widget.default);

//        });
//    }

//});