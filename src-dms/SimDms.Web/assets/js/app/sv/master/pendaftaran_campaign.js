var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

function RegCampaignController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data.StartDate = me.now();
        me.data.EndDate = me.now();
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "RegCampaignBrowse",
            title: "Campaign Register Browse",
            manager: MasterService,
            query: "RegCampaignBrowse",
            defaultSort: "PoliceRegNo asc",
            columns: [
                { field: "PoliceRegNo", Title: "Police Reg No", Width: 110 },
                { field: "ChassisCode", Title: "Chassis Code", Width: 110 },
                { field: "ChassisNo", Title: "Chassis No", Width: 110 },
                { field: "EngineCode", Title: "Engine Code", Width: 110 },
                { field: "EngineNo", Title: "Engine No", Width: 110 },
                { field: "CustomerCode", Title: "Customer Code", Width: 110 },
                { field: "CustomerName", Title: "Customer Name", Width: 110 },
                { field: "LookupValueName", Title: "Lookup Value Name", Width: 110 },
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
    
    me.btnPoliceRegNo = function (x) {
        var lookup = Wx.klookup({
            name: "btnCampaign",
            title: "Customer and Vehicle",
            serverBinding: true,
            url: "sv.api/Grid/btnCampaignGrid",
            pageSize: 10,
            columns: [
                { field: "PoliceRegNo", title: "Police Reg No", sWidth: "110px" },
                { field: "ChassisCode", title: "Chassis Code", Width: "110px" },
                { field: "ChassisNo", title: "Chassis No", Width: "110px" },
                { field: "EngineCode", title: "Engine Code", Width: "110px" },
                { field: "EngineNo", title: "Engine No", Width: "110px" },
                { field: "ServiceBookNo", title: "Service Book No", Width: "110px" },
                { field: "CustomerCode", title: "Customer Code", Width: "110px" },
                { field: "CustomerName", title: "Customer Name", Width: "110px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                if (x == undefined) {
                    me.data.PoliceRegNo = result.PoliceRegNo;
                }
                else if (x == 'C')
                {
                    me.data.ChassisCode = result.ChassisCode;
                    me.data.ChassisNo = result.ChassisNo;
                }
                else
                {

                    me.data.EngineCode = result.EngineCode;
                    me.data.EngineNo = result.EngineNo;
                }
                me.Apply();
            }
        });
    }

    me.btnChassisCode = function () {
        me.btnPoliceRegNo('C');
    }

    me.btnEngineCode = function () {
        me.btnPoliceRegNo('E')
    }

    me.saveData = function (e, param) {
        me.data.IsActive = me.data.IsActive == "1" ? true : false;
        $http.post('sv.api/regcampaign/save', me.data).
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
                $http.post('sv.api/regcampaign/deletedata', me.data).
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

    $("[name='PoliceRegNo']").on('blur', function () {
        if (me.data.PoliceRegNo != null) {
            $http.post('sv.api/regcampaign/btnPoliceRegNo', me.data).
               success(function (data, status, headers, config) {
                   if (data.success) {
                       me.data = data.data;
                       me.data.StartDate = me.now();
                       me.data.EndDate = me.now();
                       me.data.Address = data.data.Address1 + "" + data.data.Address2 + "" + data.data.Address3;
                   }
                   else {
                       me.data.BasicModel = "";
                       me.btnPoliceRegNo();
                   }
               }).
               error(function (data, status, headers, config) {
                   MsgBox('Terjadi Kesalahan, Hubungi SDMS Support');
               });
        }
    });

    me.start();
}


$(document).ready(function () {
    var options = {
        title: "Pendaftaran Campaign",
        xtype: "panels",
        toolbars: [
                    { name: "btnBrowse", text: "Browse", cls: "btn btn-info", icon: "icon-search", show: "(!hasChanged || isInitialize) && (!isEQAvailable || !isEXLSAvailable) ", click: "browse()" },
                    { name: "btnDelete", text: "Delete", cls: "btn btn-danger", icon: "icon-remove", show: "isLoadData && (!isEQAvailable || !isEXLSAvailable) ", click: "delete()" },
                    { name: "btnSave", text: "Save", cls: "btn btn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
        ],
        panels: [
            {
                name: "pnlRefVehicle",
                title: "Kendaraan",
                items: [
                    { 
                        name: "PoliceRegNo", text: "No. Polisi", type: "popup", btnName: "btnPoliceRegNo", required: true, validasi: "required", click: "btnPoliceRegNo()"
                    },
                    {
                        name: "StartDate", text: "Tgl. Awal", cls: "span4", type: "ng-datepicker",
                    },
                    {
                        name: "EndDate", text: "Tgl. Akhir", cls: "span4", type: "ng-datepicker",
                    },
                    {
                        text: "No. Rangka",
                        type: "controls",
                        required: true,
                        items: [
                            {
                                name: "ChassisCode", cls: "span3", text: "Chassis Code", type: "popup", btnName: "btnChassisCode", required: true, validasi: "required", click: "btnChassisCode()"
                            },
                            {
                                name: "ChassisNo", cls: "span5", text: "Chassis No", readonly: true
                            },

                        ]
                    },
                    {
                        text: "No. Mesin",
                        type: "controls",
                        required: true,
                        items: [
                            {
                                name: "EngineCode", cls: "span3", text: "Engine Code", type: "popup", btnName: "btnEngineCode", required: true, validasi: "required", click: "btnEngineCode()"
                            },
                            {
                                name: "EngineNo", cls: "span5", text: "Engine No", readonly: true
                            },

                        ]
                    },
                    {
                        name: "ServiceBookNo", text: "No. Buku Service", readonly: true
                    },
                   
                ]
            },
            {
                name: "pnlCustomer",
                title: "Pelanggan",
                items: [
                    {
                        text: "Nama",
                        type: "controls",
                        items: [
                            {
                                name: "CustomerCode", cls: "span3", text: "Customer Code", readonly: true
                            },
                            {
                                name: "CustomerName", cls: "span5", text: "Customer Name", readonly: true
                            },

                        ]
                    },
                    {
                        name: "Address", text: "Alamat", type: "textarea", readonly: true
                    },
                    {
                        text: "Kota",
                        type: "controls",
                        items: [
                            {
                                name: "CityCode", cls: "span3", text: "City Code", readonly: true
                            },
                            {
                                name: "CityName", cls: "span5", text: "City Name", readonly: true
                            },

                        ]
                    },
                    {
                        name: "LookupValueName", text: "Pilihan Campaign", type: "select", cls: "span4",
                        items: [
                            { value: 'ACCESORIES', text: 'ACCESORIES' },
                            { value: 'FREE SERVICE', text: 'FREE SERVICE' },
                        ]
                    },
                    {
                        name: "SalesModel",
                        cls:"hide"
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
        SimDms.Angular("RegCampaignController");
    }

});


//$(document).ready(function () {
//    var options = {
//        title: "Campaign Registration",
//        xtype: "panels",
//        toolbars: [
//            { name: "btnCreate", text: "New", icon: "icon-file" },
//            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
//            { name: "btnSave", text: "Save", icon: "icon-save" },
//           // { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
//            { name: "btnDelete", text: "Delete", icon: "icon-remove", cls: "hide" },
//        ],
//        panels: [
//            {
//                name: "pnlRefVehicle",
//                title: "Vehicle",
//                items: [
//                    {

//                        name: "PoliceRegNo",
//                        text: "Police Reg No",
//                        type: "popup",
//                        btnName: "btnPoliceRegNo",
//                        readonly: true,
//                        required: "required"
//                    },
//                    {
//                        name: "StartDate",
//                        text: "Start Date",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "EndDate",
//                        text: "End Date",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        text: "Chassis",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "ChassisCode",
//                                cls: "span3",
//                                text: "Chassis Code",
//                                type: "popup",
//                                btnName: "btnChassisCode",
//                                readonly: true,
//                                required: "required"
//                            },
//                            {

//                                name: "ChassisNo",
//                                cls: "span5",
//                                text: "Chassis No",
//                                readonly: true
//                            },

//                        ]
//                    },
                    
//                    {
//                        text: "Engine",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "EngineCode",
//                                cls: "span3",
//                                text: "Engine Code",
//                                type: "popup",
//                                btnName: "btnEngineCode",
//                                readonly: true,
//                                required: "required"
//                            },
//                            {

//                                name: "EngineNo",
//                                cls: "span5",
//                                text: "Engine No",
//                                readonly: true
//                            },

//                        ]
//                    },
//                    {
//                        name: "ServiceBookNo",
//                        text: "Service Book No",
//                        readonly: true
//                    },
                   
//                ]
//            },

//            {
//                name: "pnlCustomer",
//                title: "Customer",
//                items: [
//                    {
//                        text: "Customer",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "CustomerCode",
//                                cls: "span3",
//                                text: "Customer Code",
//                                readonly: true
//                            },
//                            {

//                                name: "CustomerName",
//                                cls: "span5",
//                                text: "Customer Name",
//                                readonly: true
//                            },

//                        ]
//                    },
//                    {
//                        name: "Address",
//                        text: "Address",
//                        type: "textarea",
//                        readonly: true
//                    },
//                    {
//                        text: "Town",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "CityCode",
//                                cls: "span3",
//                                text: "City Code",
//                                readonly: true
//                            },
//                            {

//                                name: "CityName",
//                                cls: "span5",
//                                text: "City Name",
//                                readonly: true
//                            },

//                        ]
//                    },
//                    {
//                        name: "LookupValueName",
//                        text: "Lookup Value Name",
//                        type: "select",
//                        cls: "span4",
//                        items: [
//                            { value: 'ACCESORIES', text: 'ACCESORIES' },
//                            { value: 'FREE SERVICE', text: 'FREE SERVICE' },
//                        ]
//                    },
//                    {
//                        name: "SalesModel",
//                        cls:"hide"
//                    },

//                ]
//            },
            
//        ],
//    }

//    var widget = new SimDms.Widget(options);
//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/regcampaign/default', function (result) {
//            widget.default = result;
//            widget.populate(result);

//        });
//    });
//    $("#btnPoliceRegNo").on("click", function () {
//        var lookup = widget.klookup({
//            name: "police",
//            title: "Customer and Vehicle",
//            url: "sv.api/grid/btnCampaign",
//            serverBinding: true,
//            pageSize: 10,
//            filters: [
//                {
//                    name: "PoliceRegNo",
//                    text: "Police Reg No",
//                    cls: "span3",
//                },
//                {
//                    name: "ChassisCode",
//                    text: "Chassis Code",
//                    cls: "span3",
//                },
//                {
//                    name: "EngineCode",
//                    text: "Engine Code",
//                    cls: "span3",
//                },
//                {
//                    name: "CustomerCode",
//                    text: "Customer Code",
//                    cls: "span3",
//                },
                
//            ],
//            columns: [
//                { field: "PoliceRegNo", title: "Police Reg No", sWidth: "110px" },
//                { field: "ChassisCode", title: "Chassis Code", sWidth: "110px" },
//                { field: "ChassisNo", title: "Chassis No", sWidth: "110px" },
//                { field: "EngineCode", title: "Engine Code", sWidth: "110px" },
//                { field: "EngineNo", title: "Engine No", sWidth: "110px" },
//                { field: "ServiceBookNo", title: "Service Book No", sWidth: "110px" },
//                { field: "CustomerCode", title: "Customer Code", sWidth: "110px" },
//                { field: "CustomerName", title: "Customer Name", sWidth: "110px" },
                
//            ],
//        });
//        lookup.dblClick(function (data) {
//            widget.populate(data);
//            getDate();
//        });
//    });
//    $("#btnChassisCode").on("click", function () {
//        var lookup = widget.klookup({
//            name: "police",
//            title: "Customer and Vehicle",
//            url: "sv.api/grid/btnCampaign",
//            serverBinding: true,
//            pageSize: 10,
//            filters: [
//                {
//                    name: "PoliceRegNo",
//                    text: "Police Reg No",
//                    cls: "span3",
//                },
//                {
//                    name: "ChassisCode",
//                    text: "Chassis Code",
//                    cls: "span3",
//                },
//                {
//                    name: "EngineCode",
//                    text: "Engine Code",
//                    cls: "span3",
//                },
//                {
//                    name: "CustomerCode",
//                    text: "Customer Code",
//                    cls: "span3",
//                },

//            ],
//            columns: [
//                { field: "PoliceRegNo", title: "Police Reg No", sWidth: "110px" },
//                { field: "ChassisCode", title: "Chassis Code", sWidth: "110px" },
//                { field: "ChassisNo", title: "Chassis No", sWidth: "110px" },
//                { field: "EngineCode", title: "Engine Code", sWidth: "110px" },
//                { field: "EngineNo", title: "Engine No", sWidth: "110px" },
//                { field: "ServiceBookNo", title: "Service Book No", sWidth: "110px" },
//                { field: "CustomerCode", title: "Customer Code", sWidth: "110px" },
//                { field: "CustomerName", title: "Customer Name", sWidth: "110px" },

//            ],
//        });
//        lookup.dblClick(function (data) {
//            widget.populate(data);
//            getDate();
//        });
//    });
//    $("#btnEngineCode").on("click", function () {
//        var lookup = widget.klookup({
//            name: "police",
//            title: "Customer and Vehicle",
//            url: "sv.api/grid/btnCampaign",
//            serverBinding: true,
//            pageSize: 10,
//            filters: [
//                {
//                    name: "PoliceRegNo",
//                    text: "Police Reg No",
//                    cls: "span3",
//                },
//                {
//                    name: "ChassisCode",
//                    text: "Chassis Code",
//                    cls: "span3",
//                },
//                {
//                    name: "EngineCode",
//                    text: "Engine Code",
//                    cls: "span3",
//                },
//                {
//                    name: "CustomerCode",
//                    text: "Customer Code",
//                    cls: "span3",
//                },

//            ],
//            columns: [
//                { field: "PoliceRegNo", title: "Police Reg No", sWidth: "110px" },
//                { field: "ChassisCode", title: "Chassis Code", sWidth: "110px" },
//                { field: "ChassisNo", title: "Chassis No", sWidth: "110px" },
//                { field: "EngineCode", title: "Engine Code", sWidth: "110px" },
//                { field: "EngineNo", title: "Engine No", sWidth: "110px" },
//                { field: "ServiceBookNo", title: "Service Book No", sWidth: "110px" },
//                { field: "CustomerCode", title: "Customer Code", sWidth: "110px" },
//                { field: "CustomerName", title: "Customer Name", sWidth: "110px" },

//            ],
//        });
//        lookup.dblClick(function (data) {
//            widget.populate(data);
//            getDate();
//        });
//    });
    
//    $("#btnBrowse").on("click", function () {
//        //loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "browse",
//            title: "Campaign Register",
//            source: "sv.api/grid/RegCampaignBrowse",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "PoliceRegNo", sTitle: "Police Reg No", sWidth: "110px" },
//                { mData: "ChassisCode", sTitle: "Chassis Code", sWidth: "110px" },
//                { mData: "ChassisNo", sTitle: "Chassis No", sWidth: "110px" },
//                { mData: "EngineCode", sTitle: "Engine Code", sWidth: "110px" },
//                { mData: "EngineNo", sTitle: "Engine No", sWidth: "110px" },
//                { mData: "CustomerCode", sTitle: "Customer Code", sWidth: "110px" },
//                { mData: "CustomerName", sTitle: "Customer Name", sWidth: "110px" },
//                { mData: "LookupValueName", sTitle: "Lookup Value Name", sWidth: "110px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    widget.lookup.onDblClick(function (e, data, name) {
//        widget.lookup.hide();
//        switch (name) {
//            case "browse":
//                widget.populate($.extend({}, widget.default, data));
//                widget.lookup.hide();
//                clear("dbclick");
//                $("#LookupValueName").attr("disabled", "disabled");
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

//            widget.post("sv.api/regcampaign/save", param, function (result) {
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
//            widget.post("sv.api/regcampaign/deletedata", param, function (result) {
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
//            $("#btnSave").removeClass("hide");
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//            $("#LookupValueName").removeAttr("disabled");
//        } else if (p == "btnEdit") {
//            $("#StallCode").removeAttr('readonly');
//            $("#Description").removeAttr('readonly');
//            $("#btnSave").removeClass('hide');
//        } 
//    }
//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/regcampaign/default", function (result) {
//            widget.default = $.extend({}, result);
//            widget.populate(widget.default);

//        });
//    }

//    function getDate() {
//        widget.post("sv.api/regcampaign/default", function (result) {
//            widget.default = $.extend({}, result);
//            widget.populate(widget.default);
//        });
//    }

//});