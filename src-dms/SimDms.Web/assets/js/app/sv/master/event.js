var totalSrvAmt = 0;
var status = 'N';
var svType = '2';


function EventController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.$watch('data.CheckAll', function (newValue, oldValue) {
        if (newValue !== oldValue) {
            if (newValue) {
                    $('#BasicModel').attr('disabled', true);
                    $('#btnBasicModel').attr('disabled', true);
                    me.data.BasicModel = "ALL";
                    me.isBasicModel == false;
            }
            else {
                if (!me.isBasicModel) {
                    $('#BasicModel').val("");
                    me.isBasicModel == true;
                }
                    $('#BasicModel').attr('disabled', false);
                    $('#btnBasicModel').attr('disabled', false);
                    
            }
        }
    });

    me.initialize = function () {

        me.data.EventDate = me.now();
        me.data.EventStartDate = me.now();
        var EventEndDate = new Date(me.data.EventStartDate).getMonth() + 1 + '/' + ((new Date(me.data.EventStartDate).getDate()) + 1 )+ '/' + new Date(me.data.EventStartDate).getFullYear();
        me.data.EventEndDate = EventEndDate;
        me.data.IsActive = true;
        me.data.CheckAll = false;
        me.isBasicModel == false
        me.data.LaborDiscPct = 0;
        me.data.PartsDiscPct = 0;
        me.data.MaterialDiscPct = 0;

    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "EventBrowse",
            title: "Master Event Browse",
            manager: MasterService,
            query: "EventBrowse",
            defaultSort: "EventNo asc",
            columns: [
                { field: "EventNo", Title: "Event No", Width: "110px" },
                { field: "EventDate", Title: "Event Date", Width: "130px" },
                { field: "BasicModel", Title: "Basic Model", Width: "110px" },
                { field: "JobType", Title: "Job Type", Width: "110px" },
                { field: "EventStartDate", Title: "Event Start Date", Width: "130px" },
                { field: "EventEndDate", Title: "Event End Date", Width: "130px" },
                { field: "LaborDiscPct", Title: "Labor Disc", Width: "80px" },
                { field: "PartsDiscPct", Title: "Part Disc", Width: "80px" },
                { field: "MaterialDiscPct", Title: "Material Disc", Width: "80px" },
                { field: "Description", Title: "Description", Width: "110px" },
                { field: "Status", Title: "Is Active", Width: "80px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.data.isBasicModel == true;
                me.data.BasicModel = result.BasicModel;
                me.startEditing();
                me.Apply();
            }
        });
    }

    me.btnBasicModel = function () {
        var lookup = Wx.blookup({
            name: "CBasmodOpen",
            title: "Master Model",
            manager: MasterService,
            query: "CBasmodOpen",
            defaultSort: "BasicModel asc",
            columns: [
                { field: "BasicModel", Title: "Basic Model", Width: "110px" },
                { field: "TechnicalModelCode", Title: "Technical Model Code", Width: "110px" },
                { field: "ModelDescription", Title: "Description", Width: "80px" },
                { field: "Status", Title: "Status", Width: "80px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.BasicModel = result.BasicModel;
                me.Apply();
            }
        });
    }

    $("[name='BasicModel']").on('blur', function () {
        if (me.data.BasicModel != null) {
            $http.post('sv.api/event/CBasmodOpen', me.data).
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
               });
        }
    });

    me.btnJobType = function () {
        var lookup = Wx.blookup({
            name: "JobView",
            title: "Master Pekerjaan",
            manager: MasterService,
            query: "JobView",
            defaultSort: "JobType asc",
            columns: [
                { field: "JobType", Title: "Refference Code", Width: "110px" },
                { field: "JobDescription", Title: "Description", Width: "110px" },
                { field: "DescriptionEng", Title: "Description(Eng.)", Width: "80px" },
                { field: "Status", Title: "Is Active", Width: "80px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.JobType = result.JobType;
                me.Apply();
            }
        });
    }

    $("[name='JobType']").on('blur', function () {
        if (me.data.JobType != null) {
            $http.post('sv.api/event/JobView', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.JobType = data[0].JobType;
                       $('#JobView').attr('disabled', 'disabled');
                   }
                   else {
                       me.data.JobType = "";
                       me.btnJobType();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
    });

    me.saveData = function (e, param) {
        $http.post('sv.api/event/save', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.startEditing();
                    } else {
                        MsgBox(data.message, MSG_INFO);
                    }
                }).
                error(function (e, status, headers, config) {
                    console.log(e);
                });
    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sv.api/event/deletedata', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data deleted...");
                        me.data = {};
                        me.initialize();
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
        Wx.loadForm();
        Wx.showForm({ url: "sv/master/PrintEvent" });
    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Event",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn bAddTaskstn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" }
        ],
        panels: [
            {
                name: "pnlRefService",
                items: [
                    {
                        name: "EventNo", cls: "span4", text: "Event No", readonly: true, placeHolder: 'EVT/XX/YYYYYY'
                    },
                    {
                        name: "EventDate", text: "Event Date", cls: "span4", type: "ng-datepicker",
                    },
                    {
                        name: "Description", text: "Description", placeholder: "Keterangan", required: true, validasi: "required"
                    },
                    {
                        name: "BasicModel", text: "Basic Model", cls: "span4", type: "popup", btnName: "btnBasicModel", required: true, validasi: "required", click: "btnBasicModel()"
                    },
                    {
                        name: "CheckAll", text: "Check All", cls: "span4", type: "x-switch", float: "left"
                    },
                    {
                        name: "JobType", text: "Job Type", type: "popup", btnName: "btnJobType", required: true, validasi: "required", click: "btnJobType()"
                    },
                    {
                        name: "EventStartDate", text: "Event Start Date", cls: "span4", type: "ng-datepicker",
                    },
                    {
                        name: "EventEndDate", text: "Event End Date", cls: "span4", type: "ng-datepicker",
                    },
                    {
                        name: "IsActive", model: "data.IsActive", text: "Is Active", cls: "span2", type: "x-switch", float: "left"
                    },

                ]
            },
            {
                name: "pnlinfopel",
                title: "Potongan Diskon Berdasarkan",
                items: [

                {
                    name: "LaborDiscPct", cls: "span2 number", text: "Labor Disc Pct", required: "required"
                },
                {
                    name: "PartsDiscPct", cls: "span2 number", text: "Parts Disc Pct", required: "required"
                },
                {
                    name: "MaterialDiscPct", cls: "span2 number", text: "Material Disc Pct", required: "required"
                },


                ]
            },

        ],
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("EventController");
    }

});

//$(document).ready(function () {
//    var options = {
//        title: "Event",
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
//                        name: "EventNo",
//                        cls: "span4",
//                        text: "Event No",
//                        readonly: true,
//                    },
//                    {
//                        name: "EventDate",
//                        text: "Event Date",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "Description",
//                        text: "Description",
//                        placeholder: "Keterangan",
//                        required: "required"
//                    },
//                    {
//                        name: "BasicModel",
//                        text: "Basic Model",
//                        cls: "span4",
//                        type: "popup",
//                        btnName: "btnBasicModel",
//                        readonly: true,
//                        required: "required"
//                    },
//                    {
//                        name: "CheckAll",
//                        text: "Check All",
//                        cls: "span4",
//                        type: "switch",
//                        float: "left"
//                    },
//                    {
//                        name: "JobType",
//                        text: "Job Type",
//                        type: "popup",
//                        btnName: "btnJobType",
//                        readonly: true,
//                        required: "required"
//                    },
//                    {
//                        name: "EventStartDate",
//                        text: "Event Start Date",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "EventEndDate",
//                        text: "Event End Date",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "IsActive",
//                        text: "Is Active",
//                        cls: "span2",
//                        type: "switch",
//                        float: "left"
//                    },

//                ]
//            },
//            {
//                name: "pnlinfopel",
//                //cls: "hide",
//                title: "Potongan Diskon Berdasarkan",
//                items: [

//                {
//                    name: "LaborDiscPct",
//                    cls: "span2 number",
//                    text: "Labor Disc Pct",
//                    required: "required"
//                },
//                {
//                    name: "PartsDiscPct",
//                    cls: "span2 number",
//                    text: "Parts Disc Pct",
//                    required: "required"
//                },
//                {
//                    name: "MaterialDiscPct",
//                    cls: "span2 number",
//                    text: "Material Disc Pct",
//                    required: "required"
//                },


//                ]
//            },

//        ],
//    }

//    var widget = new SimDms.Widget(options);

//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/event/default', function (result) {
//            widget.default = result;
//            widget.populate(result);

//        });
//    });


//    $("#btnBasicModel").on("click", function () {
//        // loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "basmod",
//            title: "Basic Model",
//            source: "sv.api/grid/CBasmodOpen",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "BasicModel", sTitle: "Basic Model", sWidth: "110px" },
//                { mData: "TechnicalModelCode", sTitle: "Technical Model Code", sWidth: "110px" },
//                { mData: "ModelDescription", sTitle: "Description", sWidth: "80px" },
//                { mData: "Status", sTitle: "Status", sWidth: "80px" },


//            ]
//        });
//        widget.lookup.show();
//    });

//    $("#btnJobType").on("click", function () {
//        // loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "jobtype",
//            title: "Job Type",
//            source: "sv.api/grid/JobView",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "JobType", sTitle: "Refference Code", sWidth: "110px" },
//                { mData: "JobDescription", sTitle: "Description", sWidth: "110px" },
//                { mData: "DescriptionEng", sTitle: "Description(Eng.)", sWidth: "80px" },
//                { mData: "Status", sTitle: "Is Active", sWidth: "80px" },


//            ]
//        });
//        widget.lookup.show();
//    });


//    $("#btnBrowse").on("click", function () {
//        //  loadData('browse');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "Browse",
//            title: "Event",
//            source: "sv.api/grid/EventBrowse",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "EventNo", sTitle: "Event No", sWidth: "110px" },
//                {
//                    mData: "EventDate", sTitle: "Event Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                { mData: "BasicModel", sTitle: "Basic Model", sWidth: "110px" },
//                { mData: "JobType", sTitle: "Job Type", sWidth: "110px" },
//                {
//                    mData: "EventStartDate", sTitle: "Event Start Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                {
//                    mData: "EventEndDate", sTitle: "Event End Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                { mData: "LaborDiscPct", sTitle: "Labor Disc", sWidth: "80px" },
//                { mData: "PartsDiscPct", sTitle: "Part Disc", sWidth: "80px" },
//                { mData: "MaterialDiscPct", sTitle: "Material Disc", sWidth: "80px" },
//                { mData: "Description", sTitle: "Description", sWidth: "110px" },

//                { mData: "Status", sTitle: "Is Active", sWidth: "80px" },
//            ]
//        });
//        widget.lookup.show();
//    });



//    widget.lookup.onDblClick(function (e, data, name) {
//        widget.lookup.hide();
//        switch (name) {
//            case "Browse":
//                widget.populate($.extend({}, widget.default, data));
//                clear("dbclick");
//                var wkt = moment(data["CloseDate"]).format("HH:mm");
//                data["CloseDate"] = wkt;
//                $("#pnlinfoklub").removeClass("hide");
//                $("#PnlTabel").removeClass("hide");
//                widget.lookup.hide();
//                //getTable();
//                // $("#Description").attr('readonly', 'readonly');
//                break;
//            case "basmod":
//                widget.populate($.extend({}, data));
//                break;
//            case "jobtype":
//                widget.populate($.extend({}, data));
//                break;
//            default:
//                break;
//        }
//    });

//    $("#CheckAllY").on("change", function () {
//        if ($("#CheckAllY").val() == 'true') {
//            $("#btnBasicModel").attr("disabled", "disabled");
//            $("#BasicModel").val("All");
//        }
//    });

//    $("#CheckAllN").on("change", function () {
//        if ($("#CheckAllY").val() == 'false') {
//            $("#btnBasicModel").removeAttr("disabled");
//            $("#BasicModel").val("");
//        }
//    });
//    $("#btnSave").on("click", saveData);
//    $("#btnDelete").on("click", deleteData);
//    $('#btnCreate').on('click', function (e) {
//        clear("new");
//    });
//    $('#btnEdit').on('click', function (e) {
//        clear("btnEdit");
//    });
//    function BasmodAll() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/event/getBasicModel", param, function (result) {
//            alert(result["record"][0]["BasicModel"]);
//            for (i = 0; i < result["jum"]; i++) {
//                alert(result["record"][i]["BasicModel"]);
//            }
//        });
//    }

//    function saveData(p) {
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            /*if (param.BasicModel == 'All') {
//                //alert("all");
//                param.LaborDiscPct = parseFloat(param.LaborDiscPct);
//                param.PartsDiscPct = parseFloat(param.PartsDiscPct);
//                param.MaterialDiscPct = parseFloat(param.MaterialDiscPct);
//                widget.post("sv.api/event/getBasicModel", param, function (result) {
                    
//                    //alert(result["record"][0]["BasicModel"]);
//                    for (i = 0; i < result["jum"]; i++) {
//                        param.BasicModel = result["record"][i]["BasicModel"];
//                        widget.post("sv.api/event/save", param, function (data) {
                            
//                        });
    
//                    }
//                    SimDms.Success(result["jum"]+" data saved...");
                                
//                });
                
//            } else {*/
//            //alert("single");
//            var param = $(".main .gl-widget").serializeObject();
//            param.LaborDiscPct = parseFloat(param.LaborDiscPct);
//            param.PartsDiscPct = parseFloat(param.PartsDiscPct);
//            param.MaterialDiscPct = parseFloat(param.MaterialDiscPct);
//            widget.post("sv.api/event/save", param, function (result) {
//                if (result.success) {
//                    SimDms.Success(result["jum"] + " data saved...");
//                    clear("new");
//                }
//            });
//        }

//    }

//    function deleteData() {
//        if (confirm("Apakah anda yakin???")) {
//            var param = $(".main .gl-widget").serializeObject();
//            widget.post("sv.api/event/deletedata", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data deleted...");
//                    clear("new");
//                } else {
//                    SimDms.Error("fail deleted...");
//                }
//            });
//        }
//    }





//    function clear(p) {
//        if (p == "clear") {
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//        } else if (p == "dbclick") {
//            $("#btnEdit").removeClass('hide');
//            $("#btnDelete").removeClass('hide');
//        } else if (p == "new") {
//            // widget.clearForm();

//            $("#pnlinfoklub").addClass("hide", "hide");
//            $("#PnlTabel").addClass("hide", "hide");
//            clearData();
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//            //    $("#Description").attr('readonly', 'readonly');


//        } else if (p == "btnEdit") {
//            $("#btnSave").removeClass('hide');
//        }
//    }

//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/event/default", function (result) {
//            widget.default = $.extend({}, result);
//            widget.populate(widget.default);

//        });
//    }


//});