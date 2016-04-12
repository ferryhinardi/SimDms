var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

function TarifJasaController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    $http.post('sv.api/Combo/LoadLaborCode').
    success(function (data, status, headers, config) {
       me.LaborCode = data;
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "TafjaOpen",
            title: "Master Labor Rate Browse",
            manager: MasterService,
            query: "TafjaOpen",
            defaultSort: "LaborCode asc",
            columns: [
                { field: "LaborCode", Title: "Labor Code", Width: "80px" },
                { field: "Description", Title: "Description", Width: "110px" },
                { field: "LaborPrice", Title: "Labor Price", Width: "80px" },
                { field: "EffectiveDate", Title: "Effective Date", Width: "130px" },
                { field: "Status", Title: "Is Active", Width: "80px" },
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

    me.saveData = function (e, param) {
        $http.post('sv.api/tarifjasa/save', me.data).
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
                $http.post('sv.api/tarifjasa/deletedata', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data deleted...");
                        me.initialize();
                        me.data = {};
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

    me.btnUpdate = function () {
        MsgConfirm("Are you sure ?", function (result) {
            if (result) {
                $http.post('sv.api/tarifjasa/UpdateAll', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data updated...");
                        me.initialize();
                        me.data = {};
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

    me.initialize = function () {
        me.data.EffectiveDate = me.now();
        me.data.IsActive = true;

    }
    
    me.printPreview = function () {

        var ReportId = 'SvRpMst009';
        var par = [
           '4W'
        ]
        var rparam = 'PERIODE : ' + moment(Date.now()).format('DD-MMMM-YYYY');

        Wx.showPdfReport({
            id: ReportId,
            pparam: par.join(','),
            rparam: rparam,
            type: "devex"
        });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Labor rate",
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
                        name: "LaborCode", text: "Labor Code", cls: "span4 full", type: "select2", required: true, validasi: "required", datasource: "LaborCode"
                    },
                    {
                        name: "Description", text: "Description", placeholder: "Keterangan", required: true, validasi: "required", cls: "span4 full"
                    },
                    {
                        name: "LaborPrice", text: "Labor Price", cls: "span4 number", required: true, validasi: "required"
                    },
                    {
                        name: "EffectiveDate", text: "Effective Date", cls: "span4 full", type: "ng-datepicker"
                    },
                    {
                        name: "IsActive", model: "data.IsActive", text: "Is Active", cls: "span4 full", type: "x-switch", float: "left"
                    },
                    { type: "hr"},
                    {
                        type: "buttons", items: [
                            { name: "btnUpdate", text: "Update Nilai Jasa Pada Master Pekerjaan", icon: "icon-gear", cls: "btn btn-primary", click: "btnUpdate()" },
                        ]
                    }


                ]
            },
        ]
    }

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        SimDms.Angular("TarifJasaController");
    }

});

//$(document).ready(function () {
//    var options = {
//        title: "Labor Rate",
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
//                title: "Service Information",
//                items: [
//                    {
//                        name: "LaborCode",
//                        text: "Labor Code",
//                        cls: "span4",
//                        type: "select",
//                        required: "required"
//                    },
//                    {
//                        name: "Description",
//                        text: "Description",
//                        placeholder: "Keterangan",
//                        required: "required"
//                         cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "LaborPrice",
//                        text: "Labor Price",
//                        cls: "span4 number",
//                        required: "required"
//                    },
//                    {
//                        name: "EffectiveDate",
//                        text: "Effective Date",
//                        cls: "span4",
//                        type: "datepicker"
//                    },
//                    {
//                        name: "IsActive",
//                        text: "Is Active",
//                        cls: "span4",
//                        type: "switch",
//                        float: "left"
//                    },
//                    {
//                        type: "buttons", items: [
//                            { name: "btnUpdate", text: "Update Nilai Jasa Pada Master Pekerjaan", icon: "icon-gear" },
//                        ]
//                    }


//                ]
//            },

//        ],
//    }
    
//    var widget = new SimDms.Widget(options);
	
//    widget.setSelect([
//        { name: "LaborCode", url: "sv.api/Combo/LoadLaborCode", optionalText: "-- SELECT ONE --" },
//    ]);

//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/tarifjasa/default', function (result) {
//            widget.default = result;
//            widget.populate(result);

//        });
//    });

//    $("#LaborCode").on("change", function (e) {
//        $("#btnSave").removeClass('hide');
//    });
 

//    $("#btnBrowse").on("click", function () {
        
//        var param = $(".main .gl-widget").serializeObject();
        
//        widget.lookup.init({
//            name: "CampaignOpen",
//            title: "Tarif Jasa",
//            source: "sv.api/grid/TafjaOpen",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "LaborCode", sTitle: "Labor Code", sWidth: "80px" },
//                { mData: "Description", sTitle: "Description", sWidth: "110px" },
//                { mData: "LaborPrice", sTitle: "Labor Price", sWidth: "80px" },
//                {
//                    mData: "EffectiveDate", sTitle: "Effective Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                { mData: "Status", sTitle: "Is Active", sWidth: "80px" },
//            ]
//        });
//        widget.lookup.show();
//    });

   
//            widget.lookup.onDblClick(function (e, data, name) {
               
//                widget.populate($.extend({}, widget.default, data));
//                clear("dbclick");
//                widget.lookup.hide();
//                $("#LaborCode").attr('disabled', 'disabled');
//                $("#Description").attr('readonly', 'readonly');
//                $("#LaborPrice").attr('readonly', 'readonly');
//             });
       


//    $("#btnSave").on("click", saveData);

//    $("#btnDelete").on("click", deleteData);

//    function saveData(p) {
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            param.LaborPrice = parseFloat(param.LaborPrice);
//            widget.post("sv.api/tarifjasa/save", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data saved...");
//                    clear("new");
//                }
//            });
//        }

//    }

//    $("#btnUpdate").on("click", function (e) {
//            if (confirm("Apakah anda yakin???")) {
//                var param = $(".main .gl-widget").serializeObject();
//                widget.post("sv.api/tarifjasa/UpdateAll", param, function (result) {
//                    if (result.success) {
//                        SimDms.Success("data updated...");
//                        widget.clearForm();
//                    }
//                });
//            }
//    });

//    function deleteData() {
//        if (confirm("Apakah anda yakin???")) {
//            var param = $(".main .gl-widget").serializeObject();
//            widget.post("sv.api/tarifjasa/deletedata", param, function (result) {
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

//             widget.clearForm();
//            clearData();
//            $("#LaborCode").removeAttr('disabled');
//            $("#btnSave").addClass("hide");
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//            $("#Description").removeAttr('readonly');
//            $("#LaborPrice").removeAttr('readonly');
            

//        } else if (p == "btnEdit") {
//            $("#Description").removeAttr('readonly');
//            $("#LaborPrice").removeAttr('readonly');
//            $("#btnSave").removeClass('hide');
//            $("#btnEdit").addClass('hide');
//        }
//    }
  
//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/tarifjasa/default", function (result) {
//            widget.default = $.extend({
//                Odometer: 0,
//                TimePeriod: 0,
//            }, result);
//            widget.populate(widget.default);

//        });
//    }


//});