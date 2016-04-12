function KontrakServiceController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data.ContractDate = me.now();
        me.data.BeginPeriod = me.now();
        me.data.EndPeriod = me.now();
        me.data.RefferenceDate = me.now();

        me.data.LaborDiscPct = 0.00;
        me.data.PartDiscPct = 0.00;
        me.data.MaterialDiscPct = 0.00;

        me.data.IsActive = true;

        $('#pnlinfoken').hide();
        $('#wxDetail').hide();

        me.detail = {};
        me.gridDetail.clearSelection();
    }

    me.gridDetail = new webix.ui({
        container: "wxDetail",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "PoliceRegNo", header: "Police Reg No", width: 150 },
            { id: "ChassisCode", header: "Chassis Code", width: 150 },
            { id: "ChassisNo", header: "Chassis No", width: 150 },
            { id: "EngineCode", header: "EngineCode", width: 150 },
            { id: "EngineNo", header: "Engine No", width: 150 },
            { id: "ServiceBookNo", header: "Service Book No", width: 150 },
            { id: "FakturPolisiDate", header: "Faktur Polisi Date", width: 150 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDetail.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.gridDetail.getSelectedId().id);
                    me.detail.oid = me.gridDetail.getSelectedId();
                    me.Apply();
                    $('#PoliceRegNo').attr('disabled', 'disabled');
                    $('#btnPoliceRegNo').attr('disabled', 'disabled');
                }
            }
        }
    });

    function getTable() {
        $http.post('sv.api/kontrakservice/getdatatable', me.data).
           success(function (data, status, headers, config) {
               me.grid.detail = data;
               me.loadTableData(me.gridDetail, me.grid.detail);
           }).
           error(function (e, status, headers, config) {
               console.log(e);
           });
    }

    function getInfoPelanggan() {
        $http.post('sv.api/kontrakservice/getPelangganDetail', me.data).
           success(function (data, status, headers, config) {
               me.data.Address = data.Address;
               me.data.HPNo = data.HPNo;
               me.data.FaxNo = data.FaxNo;
           }).
           error(function (e, status, headers, config) {
               console.log(e);
           });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "KontrakServiceOpen",
            title: "Master Kontrak Browse",
            manager: MasterService,
            query: "KontrakServiceOpen",
            defaultSort: "ContractNo asc",
            columns: [
                { field: "ContractNo", Title: "Contract No", Width: "110px" },
                { field: "ContractDate", Title: "Contract Date", Width: "130px" },
                { field: "BeginPeriod", Title: "Begin Period", Width: "130px" },
                { field: "EndPeriod", Title: "End Period", Width: "130px" },
                { field: "RefferenceNo", Title: "Refference No", Width: "80px" },
                { field: "RefferenceDate", Title: "Refference Date", Width: "130px" },
                { field: "LaborDiscPct", Title: "Labor Disc", Width: "80px" },
                { field: "PartDiscPct", Title: "Part Disc", Width: "80px" },
                { field: "MaterialDiscPct", Title: "Material Disc", Width: "80px" },
                { field: "Description", Title: "Description", Width: "110px" },
                { field: "CustomerCode", Title: "Customer Code", Width: "110px" },
                { field: "CustomerName", Title: "Customer Name", Width: "110px" },
                { field: "Status", Title: "Is Active", Width: "80px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.startEditing();
                getInfoPelanggan();
                getTable();
                $('#pnlinfoken').show();
                $('#wxDetail').show();
                $('#btnCustomerCode').attr('disabled', 'disabled');
                me.Apply();
            }
        });
    }

    me.CustomerCode = function () {
        var lookup = Wx.blookup({
            name: "CutomerDetailOpen",
            title: "Master Pelanggan Lookup",
            manager: MasterService,
            query: "CutomerDetailOpen",
            defaultSort: "CustomerCode Asc",
            columns: [
                { field: "CustomerCode", title: "Customer Code", Width: "110px" },
                { field: "CustomerName", title: "Customer Name", Width: "110px" },
                { field: "Address", title: "Address", Width: "180px" },
            ],
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.CustomerCode = result.CustomerCode;
                me.data.CustomerName = result.CustomerName;
                me.data.Address = result.Address;
                me.data.HPNo = result.HPNo;
                me.data.FaxNo = result.FaxNo;
                me.Apply();
            }
        });

    }

    me.PoliceRegNo = function () {
        var lookup = Wx.blookup({
            name: "VehicleDetailOpen",
            title: "Master Kendaraan Lookup",
            manager: MasterService,
            query: new breeze.EntityQuery().from("VehicleDetailOpen").withParameters({ CustomerCode: me.data.CustomerCode }),
            defaultSort: "CustomerCode Asc",
            columns: [
                { field: "CustomerCode", title: "Customer Code", Width: "110px" },
                { field: "PoliceRegNo", title: "Police Reg No", Width: "110px" },
                { field: "ChassisCode", title: "Chassis Code", Width: "110px" },
                { field: "ChassisNo", title: "Chassis No", Width: "180px" },
                { field: "EngineCode", title: "Engine Code", Width: "110px" },
                { field: "EngineNo", title: "Engine No", Width: "110px" },
                { field: "ServiceBookNo", title: "Service Book No", Width: "110px" },
                { field: "FakturPolisiDate", title: "Faktur Polisi Date", Width: "130px" },
                { field: "ClubCode", title: "Club Code", Width: "110px" },
                { field: "ClubNo", title: "Club No", Width: "110px" },
            ],
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.detail = result;
                me.Apply();
            }
        });

    }

    me.saveData = function (e, param) {
        $http.post('sv.api/kontrakservice/save', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.startEditing();
                        $('#pnlinfoken').show();
                        $('#wxDetail').show();
                        me.gridDetail.adjust();
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
                $http.post('sv.api/kontrakservice/deletedata', me.data).
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

    me.AddDet = function (e, param) {
        if (me.detail.PoliceRegNo == undefined) {
            MsgBox("Ada Informasi Yang Belum Lengkap!", MSG_ERROR);
        } else {
            $http.post('sv.api/kontrakservice/savekedua', { model: me.data, DetModel: me.detail }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.clearTable(me.gridDetail);
                        getTable();
                        me.detail = {};
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    console.log(e);
                });
        }

    }

    me.CloseDet = function () {
        me.detail = {};
        me.gridDetail.clearSelection();
        $('#btnPoliceRegNo').removeAttr('disabled');

    }

    me.DeleteDet = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sv.api/kontrakservice/deleteDataKedua', { model: me.data, DetModel: me.detail }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data deleted...");
                        me.detail = {};
                        me.clearTable(me.griddetaiBPU);
                        getTable();
                        $('#btnPoliceRegNo').removeAttr('disabled');
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
        Wx.showForm({ url: "sv/master/PrintKontrakService" });

    }
    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Kontrak Service",
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
                        name: "ContractNo", cls: "span4", text: "Contract No", readonly: true, placeHolder: "KT/YY/XXXX"
                    },
                    {
                        name: "ContractDate", text: "Contract Date", cls: "span4", type: "ng-datepicker",
                    },
                    {
                        name: "BeginPeriod", text: "Begin Period", cls: "span4", type: "ng-datepicker",
                    },
                    {
                        name: "EndPeriod", text: "End Period", cls: "span4", type: "ng-datepicker",
                    },
                    {
                        name: "Description", text: "Description",
                    },
                    {
                        name: "RefferenceNo", cls: "span4", text: "Refference No",
                    },
                    {
                        name: "LaborDiscPct", text: "Labor Disc (%)", cls: "span4 number",
                    },
                    {
                        name: "RefferenceDate", text: "Refference Date", cls: "span4", type: "ng-datepicker",
                    },
                    {
                        name: "PartDiscPct", text: "Part Disc (%)", cls: "span4 number",
                    },
                    {
                        name: "IsActive", model: "data.IsActive", text: "Is Active", cls: "span4", type: "x-switch", float: "left"
                    },
                    {
                        name: "MaterialDiscPct", text: "Material Disc (%)", cls: "span4 number",
                    },


                ]
            },
            {
                name: "pnlinfopel",
                title: "Informasi Pelanggan",
                items: [

                    {
                        name: "CustomerCode", cls: "span4", text: "Customer Code", type: "popup", readonly: true, btnName: "btnCustomerCode",
                        required: true, validasi: "required", click: "CustomerCode()"
                    },
                    {
                        name: "CustomerName", text: "Customer Name", cls: "span4", readonly: true,
                    },
                    {
                        name: "Address", text: "Customer Address", type: "textarea", readonly: true,
                    },
                    {
                        name: "HPNo", text: "Phone", cls: "span4", readonly: true,
                    },
                    {
                        name: "FaxNo", text: "Fax", cls: "span4", readonly: true,
                    },

                ]
            },
            {
                name: "pnlinfoken",
                title: "Informasi Kendaraan",
                items: [

                    {
                        name: "PoliceRegNo", model: "detail.PoliceRegNo", cls: "span4", text: "Police Reg No", type: "popup", readonly: true,
                        btnName: "btnPoliceRegNo", required: true, validasi: "required", click: "PoliceRegNo()"
                    },
                    {
                        name: "ChassisCode", model: "detail.ChassisCode", text: "Chassis Code", cls: "span4", readonly: true,
                    },
                    {
                        name: "ChassisNo", model: "detail.ChassisNo", text: "Chassis No", cls: "span4", readonly: true,
                    },
                    {
                        name: "EngineCode", model: "detail.EngineCode", text: "Engine Code", cls: "span4", readonly: true,
                    },
                    {
                        name: "EngineNo", model: "detail.EngineNo", text: "Engine No", cls: "span4", readonly: true,
                    },
                    {
                        name: "ServiceBookNo", model: "detail.ServiceBookNo", text: "Service Book No", cls: "span4", readonly: true,
                    },
                    {
                        name: "FakturPolisiDate", model: "detail.FakturPolisiDate", text: "Faktur Polisi Date", cls: "span4", type: "ng-datepicker", readonly: true,
                    },
                    {
                        type: "buttons", cls: "span6", items: [
                                { name: "btnAddDet", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddDet()", show: "detail.oid === undefined" },
                                { name: "btnUpdateDet", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddDet()", show: "detail.oid !== undefined" },
                                { name: "btnDeleteDet", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteDet()", show: "detail.oid !== undefined" },
                                { name: "btnCancelDet", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseDet()", show: "detail.oid !== undefined || detail2.oid == undefined" }
                        ]
                    },

                ]
            },
            {
                name: "wxDetail",
                xtype: "wxtable",
            },
        ],
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("KontrakServiceController");
    }

});

//var totalSrvAmt = 0;
//var status = 'N';
//var svType = '2';

//$(document).ready(function () {
//    var options = {
//        title: "Contract",
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
//                    {
//                        name: "ContractNo",
//                        cls: "span4",
//                        text: "Contract No",
//                        readonly: true,
//                    },
//                    {
//                        name: "ContractDate",
//                        text: "Contract Date",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "BeginPeriod",
//                        text: "Begin Period",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "EndPeriod",
//                        text: "End Period",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "Description",
//                        text: "Description",
//                    },
//                    {
//                        name: "RefferenceNo",
//                        cls: "span4",
//                        text: "Refference No",
//                    },
//                    {
//                        name: "LaborDiscPct",
//                        text: "Labor Disc (%)",
//                        cls: "span4",
//                    },
//                    {
//                        name: "RefferenceDate",
//                        text: "Refference Date",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "PartDiscPct",
//                        text: "Part Disc (%)",
//                        cls: "span4",
//                    },
//                    {
//                        name: "IsActive",
//                        text: "Is Active",
//                        cls: "span4",
//                        type: "switch",
//                        float: "left"
//                    },
//                    {
//                        name: "MaterialDiscPct",
//                        text: "Material Disc (%)",
//                        cls: "span4",
//                    },


//                ]
//            },
//            {
//                name: "pnlinfopel",
//                //cls: "hide",
//                title: "Customer Information",
//                items: [

//                    {
//                        name: "CustomerCode",
//                        cls: "span4",
//                        text: "Customer Code",
//                        type: "popup",
//                        readonly: true,
//                        btnName: "btnCustomerCode",
//                        required: "required"
//                    },
//                    {
//                        name: "CustomerName",
//                        text: "Customer Name",
//                        cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "Address",
//                        text: "Customer Address",
//                        type: "textarea",
//                        readonly: true,
//                    },
//                    {
//                        name: "HPNo",
//                        text: "Phone",
//                        cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "FaxNo",
//                        text: "Fax",
//                        cls: "span4",
//                        readonly: true,
//                    },

//                ]
//            },
//            {
//                name: "pnlinfoken",
//                cls: "hide",
//                title: "Vehicle Information",
//                items: [

//                    {
//                        name: "PoliceRegNo",
//                        cls: "span4",
//                        text: "Police Reg No",
//                        type: "popup",
//                        readonly: true,
//                        btnName: "btnPoliceRegNo",
//                        required: "required"
//                    },
//                    {
//                        name: "ChassisCode",
//                        text: "Chassis Code",
//                        cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "ChassisNo",
//                        text: "Chassis No",
//                        cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "EngineCode",
//                        text: "Engine Code",
//                        cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "EngineNo",
//                        text: "Engine No",
//                        cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "ServiceBookNo",
//                        text: "Service Book No",
//                        cls: "span4",
//                        readonly: true,
//                    },
//                    {
//                        name: "FakturPolisiDate",
//                        text: "Faktur Polisi Date",
//                        cls: "span4",
//                        type: "datepicker",
//                        readonly: true,
//                    },
//                    {
//                        type: "buttons", items: [
//                            { name: "btnAdd", text: "Add", icon: "icon-plus" },
//                            { name: "btnDlt", text: "Delete", icon: "icon-remove", cls: "hide" },
//                        ]
//                    },

//                ]
//            },
//            {
//                name: "PnlTabel",
//                cls: "hide",
//                xtype: "table",
//                tblname: "tblPart",
//                columns: [
//                    { text: "Action", type: "action", width: 80 },
//                    { name: "PoliceRegNo", text: "Police Reg No", width: 110 },
//                    { name: "ChassisCode", text: "Chassis Code", width: 110 },
//                    { name: "ChassisNo", text: "Chassis No", width: 110 },
//                    { name: "EngineCode", text: "EngineCode", width: 110 },
//                    { name: "EngineNo", text: "Engine No", width: 110 },
//                    { name: "ServiceBookNo", text: "Service Book No", width: 110 },
//                    { name: "FakturPolisiDate", text: "Faktur Polisi Date", type: "dateTime", width: 110 },
//                    { name: "FakturPolisiDate", cls: "hide" },

//                ]
//            }

//        ],
//    }

//    var widget = new SimDms.Widget(options);

//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/kontrakservice/default', function (result) {
//            widget.default = result;
//            widget.populate(result);

//        });
//    });

//    $("#btnPoliceRegNo").on("click", function () {
//        // loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "polici",
//            title: "Veicle Detail",
//            source: "sv.api/grid/VehicleDetailOpen?cuscode=" + param.CustomerCode,
//            sortings: [[1, "asc"]],
//            columns: [
//                { mData: "PoliceRegNo", sTitle: "Police Reg No", sWidth: "110px" },
//                { mData: "ChassisCode", sTitle: "Chassis Code", sWidth: "110px" },
//                { mData: "ChassisNo", sTitle: "Chassis No", sWidth: "180px" },
//                { mData: "EngineCode", sTitle: "Engine Code", sWidth: "110px" },
//                { mData: "EngineNo", sTitle: "Engine No", sWidth: "110px" },
//                { mData: "ServiceBookNo", sTitle: "Service Book No", sWidth: "110px" },
//                {
//                    mData: "FakturPolisiDate", sTitle: "Faktur Polisi Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        if (data == null) {
//                            return '-';
//                        } else {
//                            return moment(data).format('DD MMM YYYY - HH:mm');
//                        }

//                    }
//                },
//                { mData: "ClubCode", sTitle: "Club Code", sWidth: "110px" },
//                { mData: "ClubNo", sTitle: "Club No", sWidth: "110px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    $("#btnCustomerCode").on("click", function () {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "Customer",
//            title: "Cutomer Detail",
//            source: "sv.api/grid/CutomerDetailOpen",
//            sortings: [[1, "asc"]],
//            columns: [
//                { mData: "CustomerCode", sTitle: "Customer Code", sWidth: "110px" },
//                { mData: "CustomerName", sTitle: "Customer Name", sWidth: "110px" },
//                { mData: "Address", sTitle: "Address", sWidth: "180px" },
//            ]
//        });
//        widget.lookup.show();
//    });


//    $("#btnBrowse").on("click", function () {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "Browse",
//            title: "Service Contract",
//            source: "sv.api/grid/KontrakServiceOpen",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "ContractNo", sTitle: "Contract No", sWidth: "110px" },
//                {
//                    mData: "ContractDate", sTitle: "Contract Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                {
//                    mData: "BeginPeriod", sTitle: "Begin Period", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                {
//                    mData: "EndPeriod", sTitle: "End Period", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                { mData: "RefferenceNo", sTitle: "Refference No", sWidth: "80px" },
//                {
//                    mData: "RefferenceDate", sTitle: "Refference Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                { mData: "LaborDiscPct", sTitle: "Labor Disc", sWidth: "80px" },
//                { mData: "PartDiscPct", sTitle: "Part Disc", sWidth: "80px" },
//                { mData: "MaterialDiscPct", sTitle: "Material Disc", sWidth: "80px" },
//                { mData: "Description", sTitle: "Description", sWidth: "110px" },
//                { mData: "CustomerCode", sTitle: "Customer Code", sWidth: "110px" },
//                { mData: "CustomerName", sTitle: "Customer Name", sWidth: "110px" },
//                { mData: "Status", sTitle: "Is Active", sWidth: "80px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    function getTable() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/kontrakservice/getdatatable", param, function (result) {
//            widget.populateTable({ selector: "#tblPart", data: result });

//        });
//    }
//    function getInfoPelanggan() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/kontrakservice/getPelangganDetail", param, function (result) {
//            widget.populate($.extend({}, result));

//        });
//    }

//    widget.onTableClick(function (icon, row, selector) {
//        switch (selector.selector) {
//            case "#tblPart":
//                switch (icon) {
//                    case "edit":
//                        editDetail(row);
//                        break;
//                    case "trash":
//                        deleteDetail(row);
//                        break;
//                    default:
//                        break;
//                }
//                break;
//            default: break;
//        }

//    });


//    widget.lookup.onDblClick(function (e, data, name) {
//        widget.lookup.hide();
//        switch (name) {
//            case "Browse":
//                widget.populate($.extend({}, widget.default, data));
//                clear("dbclick");
//                $("#PnlTabel").removeClass("hide");
//                widget.lookup.hide();
//                getInfoPelanggan();
//                getTable();
//                $("#pnlinfoken").removeClass("hide");
//                $("#btnCustomerCode").attr('disabled', 'disabled');
//                $("#btnSave").addClass("hide");
//                break;
//            case "Customer":
//                $("#btnSave").removeClass("hide");
//                //clear("dbclick");
//                widget.populate($.extend({}, data));
//                break;
//            case "polici":
//                widget.populate($.extend({}, data));
//                break;
//            default:
//                break;
//        }
//    });


//    function editDetail(row) {
//        $("#btnDlt").addClass("hide", "hide");
//        $("#pnlinfoken").removeClass("hide");
//        var data = {
//            PoliceRegNo: row[1],
//            ChassisCode: row[2],
//            ChassisNo: row[3],
//            EngineCode: row[4],
//            EngineNo: row[5],
//            ServiceBookNo: row[6],
//            FakturPolisiDate: row[8],
//        }
//        widget.populate(data, "#pnlinfoken");

//    }
//    function deleteDetail(row) {
//        $("#btnDlt").removeClass("hide");
//        $("#pnlinfoken").removeClass("hide");
//        var data = {
//            PoliceRegNo: row[1],
//            ChassisCode: row[2],
//            ChassisNo: row[3],
//            EngineCode: row[4],
//            EngineNo: row[5],
//            ServiceBookNo: row[6],
//            FakturPolisiDate: row[8],
//        }
//        widget.populate(data, "#pnlinfoken");

//    }
//    $("#btnSave").on("click", saveData);
//    $("#btnAdd").on("click", saveDataKedua);

//    $("#btnDelete").on("click", deleteData);
//    $("#btnDlt").on("click", deleteKedua);

//    function saveData(p) {

//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            if (param.BeginPeriod >= param.EndPeriod) {
//                alert("Period awal tidak boleh lebih besar sama dengan period akhir");
//                return false;
//            } else if (param.CustomerCode == '') {
//                alert("Customer Code harus diisi");
//                return false;
//            }
//            widget.post("sv.api/kontrakservice/save", param, function (result) {
//                if (result.success) {
//                    var code = result["ContractNo"];
//                    $("#ContractNo").val(code);
//                    $("#btnDelete").removeClass('hide');
//                    $("#pnlinfoken").removeClass("hide");
//                    $("#PnlTabel").removeClass("hide");
//                    SimDms.Success("data saved...");
//                    //clear("new");
//                }
//            });
//        }
//    }
//    function saveDataKedua(p) {
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            widget.post("sv.api/kontrakservice/savekedua", param, function (result) {

//                if (result.success) {
//                    SimDms.Success("data saved...");
//                    getTable();
//                    $("#pnlinfoken").addClass("hide", "hide");
//                }
//            });
//        }
//    }



//    function deleteData() {
//        if (confirm("Apakah anda yakin???")) {
//            var param = $(".main .gl-widget").serializeObject();
//            widget.post("sv.api/kontrakservice/deletedata", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data deleted...");
//                    clear("new");
//                } else {
//                    SimDms.Error("fail deleted...");
//                }
//            });
//        }
//    }

//    function deleteKedua() {
//        if (confirm("Apakah anda yakin???")) {
//            var param = $(".main .gl-widget").serializeObject();
//            widget.post("sv.api/kontrakservice/deleteDataKedua", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data removed...");
//                    getTable();
//                } else {
//                    SimDms.Error("fail removed...");
//                    getTable();
//                }
//                $("#btnDlt").addClass("hide", "hide");
//                $("#pnlinfoken").addClass("hide", "hide");
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
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//        } else if (p == "dbclick") {
//            $("#btnEdit").removeClass('hide');
//            $("#btnDelete").removeClass('hide');
//        } else if (p == "new") {
//            // widget.clearForm();
//            $("#btnCustomerCode").removeAttr("disabled");
//            $("#pnlinfoken").addClass("hide", "hide");
//            $("#PnlTabel").addClass("hide", "hide");
//            clearData();
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//            //    $("#Description").attr('readonly', 'readonly');


//        } else if (p == "btnEdit") {
//            $("#btnSave").removeClass('hide');
//            $("#btnEdit").addClass('hide');
//        }
//    }

//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/kontrakservice/default", function (result) {
//            widget.default = $.extend({}, result);
//            widget.populate(widget.default);

//        });
//    }
//});
