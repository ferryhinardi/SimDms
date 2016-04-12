var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

function PenPaketServiceController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data.BeginDate = me.now();
        me.data.EndDate = me.now();
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "RegPackageBrowser",
            title: "Register Package Service Browse",
            manager: MasterService,
            query: "RegPackageBrowser",
            defaultSort: "PoliceRegNo asc",
            columns: [
                { field: "PoliceRegNo", Title: "Police Reg No", Width: "110px" },
                { field: "Chassis", Title: "Chassis", Width: "110px" },
                { field: "Package", Title: "Package Service", Width: "110px" },
                { field: "Customer", Title: "Customer", Width: "110px" },
                { field: "StartDate", Title: "Begin Date", Width: "130px" },
                {field: "FinishDate", Title: "End Date", Width: "130px" },
                { field: "AccountNo", Title: "Virtual Account", Width: "110px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                BrowseAction();
                OpenAction();
                getPackageName();
                getNoAccDesc();
                me.startEditing();
                me.Apply();
            }
        });
    }

    me.PoliceRegNo = function () {
        var lookup = Wx.klookup({
            name: "KendaraanPel",
            title: "No. Polisi",
            serverBinding: true,
            url: "sv.api/Grid/KendaraanPelGrid",
            pageSize: 10,
            columns: [
                { field: "PoliceRegNo", title: "Police Reg No", Width: "110px" },
                { field: "CustomerDesc", title: "Customer Name", Width: "110px" },
                { field: "BasicModel", title: "Basic Model", Width: "110px" },
                { field: "ChassisCode", title: "Chassis Code", Width: "110px" },
                { field: "ChassisNo", title: "Chassis No", Width: "110px" },
                { field: "EngineCode", title: "Engine Code", Width: "110px" },
                { field: "EngineNo", title: "Engine No", Width: "110px" },
                { field: "ServiceBookNo", title: "Service Book No", Width: "110px" },
            ],           
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                BrowseAction();
                me.Apply();
            }
        });
    }

    me.PackageCode = function () {
        var lookup = Wx.blookup({
            name: "RegPackageOpen",
            title: "Service Package",
            manager: MasterService,
            query: "RegPackageOpen?ModelCode=" + me.data.BasicModel,
            defaultSort: "PackageCode Desc",
            columns: [
                { field: "PackageCode", title: "Package Code", Width: "110px" },
                { field: "PackageName", title: "Package Name", Width: "110px" },
                { field: "BasicModel", title: "Basic Model", Width: "80px" },
                { field: "BillTo", title: "Bill To", Width: "110px" },
                { field: "IntervalYear", title: "Interval (Year)", Width: "80px" },
                { field: "IntervalKM", title: "Interval (KM)", Width: "110px" },
            ],

        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.PackageCode = result.PackageCode;
                me.data.PackageName = result.PackageName;
                me.Apply();
            }
        });
    }

    me.AccountNo = function () {
        var lookup = Wx.blookup({
            name: "NomorAccView",
            title: "Nomor Account",
            manager: MasterService,
            query: "NomorAccView",
            defaultSort: "AccountNo asc",
            columns: [
                { field: "AccountNo", title: "AccountNo", Width: "110px" },
                { field: "AccDescription", title: "AccDescription", Width: "110px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.AccountNo = result.AccountNo;
                me.data.AccDescription = result.AccDescription;
                me.Apply();
            }
        });
    }

    $("[name='AccountNo']").on('blur', function () {
        if (me.data.AccountNo != null) {
            $http.post('sv.api/svaccount/SalesAccNo', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data.AccountNo = data[0].AccountNo;
                       me.data.AccDescription = data[0].AccDescription;
                   }
                   else {
                       me.data.AccountNo = "";
                       me.data.AccDescription = "";
                       me.AccountNo();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
    });

    function BrowseAction() {
        $http.post('sv.api/regpackage/BrowseAction', me.data)
        .success(function (data, status, headers, config) {
            me.data = data;
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    function OpenAction() {
        $http.post('sv.api/regpackage/OpenAction', me.data)
        .success(function (data, status, headers, config) {
            me.data.PackageCode = data.PackageCode;
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    function OpenAction() {
        $http.post('sv.api/regpackage/OpenAction', me.data)
        .success(function (data, status, headers, config) {
            me.data.PackageCode = data.PackageCode;
            me.data.BeginDate = data.StartDate;
            me.data.EndDate = data.FinishDate;
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    function getPackageName() {
        $http.post('sv.api/regpackage/getPackageName', me.data)
        .success(function (data, status, headers, config) {
            me.data.PackageName = data.PackageName;
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }

    function getNoAccDesc() {
        $http.post('sv.api/regpackage/getNoAccDesc', me.data)
        .success(function (data, status, headers, config) {
            me.data.AccountNo = data.AccountNo;
            me.data.AccDescription = data.AccDescription;
        }).
        error(function (data, status, headers, config) {
            MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
        });
    }


    me.saveData = function (e, param) {
        $http.post('sv.api/regpackage/save', me.data).
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
                $http.post('sv.api/regpackage/deletedata', me.data).
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

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Service Package Registration",
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
                        name: "pnlVehicle",
                        title: "Vehicle",
                        items: [
                            {
                                name: "PoliceRegNo", text: "Police Reg No", type: "popup", cls: "span4", btnName: "btnPoliceRegNo", readonly: true, required: true, validasi: "required", click: "PoliceRegNo()"
                            },
                            {
                                name: "ServiceBookNo", cls: "span4", text: "Service Book No", readonly: true,
                            },
                            {
                                name: "BasicModel", text: "Basic Model", cls: "span4", readonly: true
                            },
                            {
                                name: "TransmissionType", text: "Transmission Type", cls: "span4", readonly: true,
                            },
                            {
                                name: "ChassisCode", text: "ChassisCode", cls: "span4", readonly: true
                            },
                            {
                                name: "ChassisNo", text: "ChassisNo", cls: "span4", readonly: true,
                            },
                            {
                                name: "EngineCode", text: "Engine Code", cls: "span4", readonly: true
                            },
                            {
                                name: "EngineNo", text: "Engine No", cls: "span4", readonly: true,
                            },

                        ]
                    },
                    {
                        name: "pnlCustomer",
                        title: "Customer",
                        items: [
                        {
                            text: "Customer",
                            type: "controls",
                            items: [
                                {
                                    name: "CustomerCode", cls: "span2", text: "Customer Code", readonly:true
                                },
                                {
                                    name: "CustomerName", text: "Customer Name", cls: "span6", readonly: true
                                },

                            ]
                        },
                        {
                            name: "CustomerAddr", text: "Customer Address", type: "textarea", readonly: true
                        },
                        {
                            text: "Town",
                            type: "controls",
                            items: [
                                {
                                    name: "CityCode", cls: "span2", text: "City Code", readonly: true
                                },
                                {
                                    name: "CityName", text: "City Name", cls: "span6", readonly: true
                                },

                            ]
                        },


                        ]
                    },
                    {
                        name: "pnlPps",
                        title: "Register Package",
                        items: [
                        {
                            text: "Package",
                            type: "controls",
                            items: [
                                {
                                    name: "PackageCode", cls: "span2", text: "Package Code", type: "popup", btnName: "btnPackageCode", readonly: true, required: true, validasi: "required", click: "PackageCode()"
                                },
                                {
                                    name: "PackageName", text: "Package Name", cls: "span6", readonly: true
                                },

                            ]
                        },
                        {
                            name: "BeginDate", text: "Begin Date", cls: "span4", type: "ng-datepicker",
                        },
                        {
                            name: "EndDate", text: "End Date", cls: "span4", type: "ng-datepicker",
                        },
                        {
                            text: "Account",
                            type: "controls",
                            items: [
                                {
                                    name: "AccountNo", cls: "span4", text: "Account No", type: "popup", btnName: "btnAccountNo", readonly: true, click: "AccountNo()"
                                },
                                {
                                    name: "AccDescription", text: "Description", cls: "span4", maxlength: "100", readonly: true
                                },

                            ]
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
        SimDms.Angular("PenPaketServiceController");
    }

});

//$(document).ready(function () {
//    var options = {
//        title: "Service Package Registration",
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
//                name: "pnlVehicle",
//                title: "Vehicle",
//                items: [
//                    {
//                        name: "PoliceRegNo",
//                        text: "Police Reg No",
//                        type: "popup",
//                        cls: "span4",
//                        btnName: "btnPoliceRegNo",
//                        readonly: true,
//                        required: "required"
//                    },
//                    {
//                        name: "ServiceBookNo",
//                        cls: "span4",
//                        text: "Service Book No",
//                        readonly: true,
//                    },
//                    {
//                        name: "BasicModel",
//                        text: "Basic Model",
//                        cls: "span4",
//                        readonly: true
//                    },
//                    {
//                        name: "TransmissionType",
//                        text: "Transmission Type",
//                        cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "ChassisCode",
//                        text: "ChassisCode",
//                        cls: "span4",
//                        readonly: true
//                    },
//                    {
//                        name: "ChassisNo",
//                        text: "ChassisNo",
//                        cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "EngineCode",
//                        text: "Engine Code",
//                        cls: "span4",
//                        readonly: true
//                    },
//                    {
//                        name: "EngineNo",
//                        text: "Engine No",
//                        cls: "span4",
//                        readonly: true,
//                    },
                    
//                ]
//            },
//            {
//                name: "pnlCustomer",
//                //cls: "hide",
//                title: "Customer",
//                items: [
//                {
//                    text: "Customer",
//                    type: "controls",
//                    items: [
//                        {
//                            name: "CustomerCode",
//                            cls: "span2",
//                            text: "Customer Code",
//                            readonly:true
//                        },
//                        {
//                            name: "CustomerName",
//                            text: "Customer Name",
//                            cls: "span6",
//                            readonly: true
//                        },

//                    ]
//                },
//                {
//                    name: "CustomerAddr",
//                    text: "Customer Address",
//                    type: "textarea",
//                    readonly: true
//                },
//                {
//                    text: "Town",
//                    type: "controls",
//                    items: [
//                        {
//                            name: "CityCode",
//                            cls: "span2",
//                            text: "City Code",
//                            readonly: true
//                        },
//                        {
//                            name: "CityName",
//                            text: "City Name",
//                            cls: "span6",
//                            readonly: true
//                        },

//                    ]
//                },


//                ]
//            },
//            {
//                name: "pnlPps",
//                //cls: "hide",
//                title: "Register Package",
//                items: [
//                {
//                    text: "Package",
//                    type: "controls",
//                    items: [
//                        {
//                            name: "PackageCode",
//                            cls: "span2",
//                            text: "Package Code",
//                            type: "popup",
//                            btnName: "btnPackageCode",
//                            readonly: true,
//                            required: "required"
//                        },
//                        {
//                            name: "PackageName",
//                            text: "Package Name",
//                            cls: "span6",
//                            readonly: true
//                        },

//                    ]
//                },
//                {
//                    name: "BeginDate",
//                    text: "Begin Date",
//                    cls: "span4",
//                    type: "datepicker",
//                },
//                {
//                    name: "EndDate",
//                    text: "End Date",
//                    cls: "span4",
//                    type: "datepicker",
//                },
//                {
//                    text: "Account",
//                    type: "controls",
//                    items: [
//                        {
//                            name: "AccountNo",
//                            cls: "span4",
//                            text: "Account No",
//                            type: "popup",
//                            btnName: "btnAccountNo",
//                            readonly: true
//                        },
//                        {
//                            name: "AccDescription",
//                            text: "Description",
//                            cls: "span4",
//                            maxlength: "100",
//                            readonly: true
//                        },

//                    ]
//                },


//                ]
//            },

//        ],
//    }

//    var widget = new SimDms.Widget(options);

//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/RegPackage/default', function (result) {
//            widget.default = result;
//            widget.populate(result);

//        });
//    });
    
   

//    $("#btnPackageCode").on("click", function () {
//        // loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "package",
//            title: "Service Package",
//            source: "sv.api/grid/RegPackageOpen?basmod=" + param.BasicModel,
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "PackageCode", sTitle: "Package Code", sWidth: "110px" },
//                { mData: "PackageName", sTitle: "Package Name", sWidth: "110px" },
//                { mData: "BasicModel", sTitle: "Basic Model", sWidth: "80px" },
//                { mData: "BillTo", sTitle: "Bill To", sWidth: "110px" },
//                { mData: "IntervalYear", sTitle: "Interval (Year)", sWidth: "80px" },
//                { mData: "IntervalKM", sTitle: "Interval (KM)", sWidth: "110px" },

//            ]
//        });
//        widget.lookup.show();
//    });

//    $("#btnAccountNo").on("click", function () {
//        //loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "AccountNo",
//            title: "Account No",
//            source: "sv.api/grid/NomorAccView",
//            sortings: [[0, "desc"]],
//            columns: [
//                { mData: "AccountNo", sTitle: "AccountNo", sWidth: "110px" },
//                { mData: "AccDescription", sTitle: "AccDescription", sWidth: "110px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    $("#btnPoliceRegNo").on("click", function () {
//        var lookup = widget.klookup({
//            name: "PoliceNo",
//            title: "No. Police",
//            url: "sv.api/grid/KendaraanPel",
//            serverBinding: true,
//            pageSize: 10,
//            filters: [
//                {
//                    name: "PoliceRegNo",
//                    text: "Police Reg No",
//                    cls: "span3",
//                },
//                {
//                    name: "ServiceBookNo",
//                    text: "Service Book No",
//                    cls: "span3",
//                },
//                {
//                    name: "CustomerDesc",
//                    text: "Customer Name",
//                    cls: "span3",
//                },
//                {
//                    name: "ChassisCode",
//                    text: "Chassis Code",
//                    cls: "span3",
//                },
//                {
//                    name: "ChassisNo",
//                    text: "Chassis No",
//                    cls: "span3",
//                },
//                {
//                    name: "BasicModel",
//                    text: "Basic Model",
//                    cls: "span3",
//                },
//                {
//                    name: "EngineCode",
//                    text: "EngineCode",
//                    cls: "span3",
//                },
//                {
//                    name: "EngineNo",
//                    text: "Engine No",
//                    cls: "span3",
//                },
//            ],
//            columns: [
//                { field: "PoliceRegNo", title: "Police Reg No", sWidth: "110px" },
//                { field: "CustomerDesc", title: "Customer Name", sWidth: "110px" },
//                { field: "BasicModel", title: "Basic Model", sWidth: "110px" },
//                { field: "ChassisCode", title: "Chassis Code", sWidth: "110px" },
//                { field: "ChassisNo", title: "Chassis No", sWidth: "110px" },
//                { field: "EngineCode", title: "Engine Code", sWidth: "110px" },
//                { field: "EngineNo", title: "Engine No", sWidth: "110px" },
//                { field: "ServiceBookNo", title: "Service Book No", sWidth: "110px" },
              
//            ],
//        });
//        lookup.dblClick(function (data) {
//            widget.populate(data);
//            BrowseAction();
//            clear("dbclick");

//        });
//    });

//    $("#btnBrowse").on("click", function () {
//      //  loadData('browse');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "Browse",
//            title: "Register Package Service",
//            source: "sv.api/grid/RegPackageBrowser",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "PoliceRegNo", sTitle: "Police Reg No", sWidth: "110px" },
//                { mData: "Chassis", sTitle: "Chassis", sWidth: "110px" },
//                { mData: "Package", sTitle: "Package Service", sWidth: "110px" },
//                { mData: "Customer", sTitle: "Customer", sWidth: "110px" },
//                {
//                    mData: "StartDate", sTitle: "Begin Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                {
//                    mData: "FinishDate", sTitle: "End Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                { mData: "AccountNo", sTitle: "Virtual Account", sWidth: "110px" },
                
//            ]
//        });
//        widget.lookup.show();
//    });

//    function BrowseAction() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/regpackage/BrowseAction", param, function (result) {
            
//            widget.populate($.extend({}, result));
            
//        });
//    }

//    function OpenAction() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/regpackage/OpenAction", param, function (result) {

//            widget.populate($.extend({}, result));
//            getPackageName();
//        });
//    }

//    function getPackageName() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/regpackage/getPackageName", param, function (result) {

//            widget.populate($.extend({}, result));

//        });
//    }

//    function getNoAccDesc() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/regpackage/getNoAccDesc", param, function (result) {

//            widget.populate($.extend({}, result));

//        });
//    }

//    widget.lookup.onDblClick(function (e, data, name) {
//        widget.lookup.hide();
//        switch (name) {
//            case "Browse":
//                widget.populate($.extend({}, widget.default, data));
//                BrowseAction();
//                OpenAction();
//                getNoAccDesc();
//                clear("dbclick");
//                widget.lookup.hide();
//                break;
//            case "AccountNo":
//                widget.populate($.extend({}, data));
//                break;
//            case "package":
//                widget.populate($.extend({}, data));
//                break;
//            default:
//                break;
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
    

//    function saveData(p) {
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            widget.post("sv.api/regpackage/save", param, function (result) {
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
//            widget.post("sv.api/regpackage/deletedata", param, function (result) {
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
//        }  else if (p == "new") {
//            // widget.clearForm();
//            $("#pnlinfoklub").addClass("hide", "hide");
//            $("#PnlTabel").addClass("hide", "hide");
//            clearData();
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//        //    $("#Description").attr('readonly', 'readonly');


//        } else if (p == "btnEdit") {
//            $("#btnSave").removeClass('hide');
//        }
//    }
  
//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/RegPackage/default", function (result) {
//            widget.default = $.extend({}, result);
//            widget.populate(widget.default);
           
//        });
//    }


//});