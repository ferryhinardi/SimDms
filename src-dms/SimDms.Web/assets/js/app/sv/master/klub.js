var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

function ClubController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.detail = {};
        me.data.CreatedDate = me.now();
        me.data.LaborDiscPct = 0.00;
        me.data.PartDiscPct = 0.00;
        me.data.MaterialDiscPct = 0.00;

        me.detail.ClubDateStart = me.now();
        me.detail.ClubDateFinish = me.now();
        me.detail.ClubSince = me.now();

        me.data.IsActive = true;
        me.detail.IsActiveP = true;

        $('#pnlinfoklub').hide();
        $('#wxDetail').hide();

        me.detail = {};
        me.gridDetail.clearSelection();
    }

    me.gridDetail = new webix.ui({
        container: "wxDetail",
        view: "wxtable", css:"alternating",
        scrollX: true,
        scrollY: true,
        columns: [
            { id: "PoliceRegNo", header: "Police Reg No", width: 150 },
            { id: "CustomerCode", header: "Customer Code", width: 150 },
            { id: "ClubNo", header: "Club No", width: 80 },
            { id: "ClubDateStart", header: "Club Date Start", width: 200, format: me.dateFormat },
            { id: "ClubDateFinish", header: "Club Date Finish", width: 200, format: me.dateFormat },
            { id: "ClubSince", header: "Club Since", width: 200, format: me.dateFormat },
            { id: "IsActiveDesc", header: "Is Active", width: 150 },
            { id: "CustomerName", header: "Customer Name", width: 250 },
            { id: "ChassisCode", header: "Chassis Code", width: 150 },
            { id: "ChassisNo", header: "Chassis No", width: 150 },
            { id: "ServiceBookNo", header: "Service Book No", width: 150 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridDetail.getSelectedId() !== undefined) {
                    me.detail = this.getItem(me.gridDetail.getSelectedId().id);
                    me.detail.oid = me.gridDetail.getSelectedId();
                    me.Apply();
                    $('#PoliceRegNo').attr('disabled', 'disabled');
                }
            }
        }
    });

    function getTable() {
        $http.post('sv.api/club/getdatatable', me.data).
           success(function (data, status, headers, config) {
               me.grid.detail = data;
               me.loadTableData(me.gridDetail, me.grid.detail);
           }).
           error(function (e, status, headers, config) {
               console.log(e);
           });
    }

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "ClubOpen",
            title: "Master Klub Browse",
            manager: MasterService,
            query: "ClubOpen",
            defaultSort: "ClubCodeStr asc",
            columns: [
                { field: "ClubCodeStr", Title: "Club Code", Width: "110px" },
                { field: "CreatedDate", Title: "Created Date", Width: "130px", template: "#= moment(CreatedDate).format('DD MMM YYYY') #" },
                { field: "Description", Title: "Description", Width: "80px" },
                { field: "LaborDiscPct", Title: "Labor Disc", Width: "80px" },
                { field: "PartDiscPct", Title: "Part Disc", Width: "80px" },
                { field: "MaterialDiscPct", Title: "Material Disc", Width: "80px" },
                { field: "IsActiveStr", Title: "Is Active", Width: "80px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.startEditing();
                getTable();
                $('#pnlinfoklub').show();
                $('#wxDetail').show();
                me.detail.IsActiveP = true;
                me.Apply();
            }
        });
    }

    me.PoliceRegNo = function () {
        var lookup = Wx.klookup({
            name: "NoPolisiOpen",
            title: "Master Kendaraan Pelanggan Lookup",                                    
            serverBinding: true,
            url: "sv.api/Grid/NoPolisiOpenGrid",
            pageSize: 10,
            columns: [
                { field: "PoliceRegNo", title: "Police Reg No", Width: "110px" },
                { field: "CustomerName", title: "Description", Width: "110px" },
                { field: "ChassisCode", title: "Chassis Code", Width: "80px" },
                { field: "ChassisNoStr", title: "Chassis No", Width: "80px" },
                { field: "EngineCode", title: "Engine Code", Width: "80px" },
                { field: "EngineNo", title: "Engine No", Width: "80px" },
                { field: "ServiceBookNo", title: "Service Book No", Width: "80px" },
                { field: "ClubCodeStr", title: "Club Code", Width: "80px" },
                { field: "ClubNo", title: "Club No", Width: "80px" },
                { field: "ClubDateStart", title: "Club Date Start", Width: "130px" },
                { field: "ClubDateFinish", title: "Club Date Finish", Width: "130px" },
                { field: "ClubSince", title: "Club Since", Width: "130px" },
                { field: "IsClubStatusDesc", title: "Is Club", Width: "80px" },
                { field: "IsContractStatusDesc", title: "Is Contract", Width: "80px" },
                { field: "IsActiveDesc", title: "Is Active", Width: "80px" }
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
        $http.post('sv.api/club/save', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        $('#ClubCodeStr').val(data.data.ClubCodeStr);
                        Wx.Success("Data saved...");
                        me.startEditing();
                        $('#pnlinfoklub').show();
                        $('#wxDetail').show();
                        me.gridDetail.adjust();
                        me.detail.IsActiveP = true;
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
                $http.post('sv.api/club/deletedata', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data deleted...");
                        me.init();
                        me.data.CreatedDate = me.now();
                        me.data.LaborDiscPct = 0.00;
                        me.data.PartDiscPct = 0.00;
                        me.data.MaterialDiscPct = 0.00;
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

    me.CloseDet = function () {
        me.detail = {};
        me.detail.ClubDateStart = me.now();
        me.detail.ClubDateFinish = me.now();
        me.detail.ClubSince = me.now();
        me.gridDetail.clearSelection();
        $('#btnPoliceRegNo').removeAttr('disabled');

    }

    me.AddDet = function (e, param) {
        if (me.detail.PoliceRegNo == undefined) {
            MsgBox("Ada Informasi Yang Belum Lengkap!", MSG_ERROR);
        } else {
            $http.post('sv.api/club/savekedua', { model: me.data, DetModel: me.detail, IsActiveP: me.detail.IsActiveP }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.clearTable(me.gridDetail);
                        getTable();
                        me.detail = {};
                        me.detail.ClubDateStart = me.now();
                        me.detail.ClubDateFinish = me.now();
                        me.detail.ClubSince = me.now();
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    console.log(e);
                });
        }

    }

    me.DeleteDet = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sv.api/club/deleteDataKedua', { model: me.data, DetModel: me.detail }).
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
        Wx.showForm({ url: "sv/master/PrintKlub" });

    }

    me.start();
}

$(document).ready(function () {
    var options = {
        title: "Master Club",
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
                                name: "ClubCodeStr", cls: "span4", text: "Club Code", readonly: true, placeHolder: "KLB/YY/XXXX"
                            },
                            {
                                name: "CreatedDate", text: "CreatedDate", cls: "span4", type: "ng-datepicker",
                            },
                            {
                                name: "Description", text: "Description", placeholder: "Keterangan", required: true, validasi: "required"
                            },
                            {
                                name: "LaborDiscPct", text: "Labor Disc", cls: "span2 number",
                            },
                            {
                                name: "PartDiscPct", text: "Part Disc", cls: "span2 number",
                            },
                            {
                                name: "MaterialDiscPct", text: "Material Disc", cls: "span2 number",
                            },
                            {
                                name: "IsActive", text: "Is Active", cls: "span2", type: "x-switch", float: "left"
                            },

                        ]
                    },
                    {
                        name: "pnlinfoklub",
                        title: "Club and Customer Information",
                        items: [

                            {
                                name: "PoliceRegNo", model: "detail.PoliceRegNo", cls: "span4", text: "Police Reg No", type: "popup"
                                , btnName: "btnPoliceRegNo", required: true, validasi: "required", click: "PoliceRegNo()"
                            },
                            {
                                name: "CustomerCode", model: "detail.CustomerCode", text: "Customer Code", cls: "span4",
                            },
                            {
                                name: "ClubNo", model: "detail.ClubNo", text: "Club No", cls: "span4", required: "required"
                            },
                            {
                                name: "ClubDateStart", model: "detail.ClubDateStart", text: "Club Date Start", cls: "span4", type: "ng-datepicker",
                            },
                            {
                                name: "ClubDateFinish", model: "detail.ClubDateFinish", text: "Club Date Finish", cls: "span4", type: "ng-datepicker",
                            },
                            {
                                name: "ClubSince", model: "detail.ClubSince", text: "Club Since", cls: "span4", type: "ng-datepicker",
                            },
                            {
                                name: "IsActiveP", model: "detail.IsActiveP", text: "Is Active", cls: "span4 full", type: "x-switch", float: "left"
                            },
                            { type: "hr"},
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
        SimDms.Angular("ClubController");
    }

});


//$(document).ready(function () {
//    var options = {
//        title: "Club",
//        xtype: "panels",
//        toolbars: [
//            { name: "btnCreate", text: "New", icon: "icon-file" },
//            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
//                        { name: "btnSave", text: "Save", icon: "icon-save" },
//            { name: "btnEdit", text: "Edit", icon: "icon-edit", cls: "hide" },
//            { name: "btnDelete", text: "Delete", icon: "icon-remove", cls: "hide" },
//        ],
//        panels: [
//            {
//                name: "pnlRefService",
//                //title: "Service Information",
//                items: [
//                    {
//                        name: "ClubCodeStr",
//                        cls: "span4",
//                        text: "Club Code",
//                        readonly: true,
//                    },
//                    {
//                        name: "CreatedDate",
//                        text: "CreatedDate",
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
//                        name: "LaborDiscPct",
//                        text: "Labor Disc",
//                        cls: "span2",
//                    },
//                    {
//                        name: "PartDiscPct",
//                        text: "Part Disc",
//                        cls: "span2",
//                    },
//                    {
//                        name: "MaterialDiscPct",
//                        text: "Material Disc",
//                        cls: "span2",
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
//                name: "pnlinfoklub",
//                cls: "hide",
//                title: "Club and Customer Information",
//                //  cls: "hide",
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
//                        name: "CustomerCode",
//                        text: "Customer Code",
//                        cls: "span4",
//                    },
//                    {
//                        name: "ClubNo",
//                        text: "Club No",
//                        cls: "span4",
//                        required: "required"
//                    },
//                    {
//                        name: "ClubDateStart",
//                        text: "Club Date Start",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "ClubDateFinish",
//                        text: "Club Date Finish",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "ClubSince",
//                        text: "Club Since",
//                        cls: "span4",
//                        type: "datepicker",
//                    },
//                    {
//                        name: "IsActiveP",
//                        text: "Is Active",
//                        cls: "span2",
//                        type: "switch",
//                        float: "left"
//                    },
//                    {
//                        name: "ChassisCode",cls:"hide",
//                    },
//                    {
//                        name: "ChassisNo", cls: "hide",
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
//                    { text: "Action", type: "action", width: 180 },
//                    { name: "PoliceRegNo", text: "Police Reg No", width: 110 },
//                    { name: "CustomerCode", text: "Customer Code", width: 80 },
//                    { name: "ClubNo", text: "Club No", width: 80 },
//                    { name: "ClubDateStart", text: "Club Date Start",type: "dateTime", width: 200 },
//                    { name: "ClubDateFinish", text: "Club Date Finish", type: "dateTime", width: 200 },
//                    { name: "ClubSince", text: "Club Since", type: "dateTime", width: 200 },
//                    { name: "ClubDateStart", cls: "hide" },
//                    { name: "ClubDateFinish", cls: "hide" },
//                    { name: "ClubSince", cls: "hide" },
//                    { name: "IsActiveP", cls: "hide" },
//                    //{ name: "ChassisCode", cls: "hide" },
//                    //{ name: "ChassisNo", cls: "hide" },
//                    { name: "IsActiveDesc", text: "Is Active", width: 80 },
//                    { name: "CustomerName", text: "Customer Name", width: 180 },
//                    { name: "ChassisCode", text: "Chassis Code", width: 80 },
//                    { name: "ChassisNo", text: "Chassis No", width: 80 },
//                    { name: "ServiceBookNo", text: "Service Book No", width: 80 },
//                ]
//            }

//        ],
//    }

//    var widget = new SimDms.Widget(options);

//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/club/default', function (result) {
//            widget.default = result;
//            widget.populate(result);

//        });
//    });
    
    
//    $("#btnPoliceRegNo").on("click", function () {
//       // loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "NoPolisi",
//            title: "No Polisi",
//            source: "sv.api/grid/NoPolisiOpen",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "PoliceRegNo", sTitle: "Police Reg No", sWidth: "110px" },
//                { mData: "CustomerName", sTitle: "Description", sWidth: "110px" },
//                { mData: "ChassisCode", sTitle: "Chassis Code", sWidth: "80px" },
//                { mData: "ChassisNoStr", sTitle: "Chassis No", sWidth: "80px" },
//                { mData: "EngineCode", sTitle: "Engine Code", sWidth: "80px" },
//                { mData: "EngineNo", sTitle: "Engine No", sWidth: "80px" },
//                { mData: "ServiceBookNo", sTitle: "Service Book No", sWidth: "80px" },
//                { mData: "ClubCodeStr", sTitle: "Club Code", sWidth: "80px" },
//                { mData: "ClubNo", sTitle: "Club No", sWidth: "80px" },
//                {
//                    mData: "ClubDateStart", sTitle: "Club Date Start", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                            return moment(data).format('DD MMM YYYY - HH:mm');
//                        }
//                },
//                {
//                    mData: "ClubDateFinish", sTitle: "Club Date Finish", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                {
//                    mData: "ClubSince", sTitle: "Club Since", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                { mData: "IsClubStatusDesc", sTitle: "Is Club", sWidth: "80px" },
//                { mData: "IsContractStatusDesc", sTitle: "Is Contract", sWidth: "80px" },
//                { mData: "IsActiveDesc", sTitle: "Is Active", sWidth: "80px" },
//            ]
//        });
//        widget.lookup.show();
//    });


//    $("#btnBrowse").on("click", function () {
//      //  loadData('browse');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "Browse",
//            title: "Campaign",
//            source: "sv.api/grid/ClubOpen",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "ClubCodeStr", sTitle: "Club Code", sWidth: "110px" },
//                {
//                    mData: "CreatedDate", sTitle: "Created Date", sWidth: "130px",
//                    mRender: function (data, type, full) {
//                        return moment(data).format('DD MMM YYYY - HH:mm');
//                    }
//                },
//                { mData: "Description", sTitle: "Description", sWidth: "80px" },
//                { mData: "LaborDiscPct", sTitle: "Labor Disc", sWidth: "80px" },
//                { mData: "PartDiscPct", sTitle: "Part Disc", sWidth: "80px" },
//                { mData: "MaterialDiscPct", sTitle: "Material Disc", sWidth: "80px" },
                
//                { mData: "IsActiveStr", sTitle: "Is Active", sWidth: "80px" },
//            ]
//        });
//        widget.lookup.show();
//    });

//    function getTable() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/club/getdatatable", param, function (result) {
//            widget.populateTable({ selector: "#tblPart", data: result });

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
//                $('#btnSave').addClass('hide');
//                var wkt = moment(data["CloseDate"]).format("HH:mm");
//                data["CloseDate"] = wkt;
//                $("#pnlinfoklub").removeClass("hide");
//                $("#PnlTabel").removeClass("hide");
//                widget.lookup.hide();
//                getTable();
//                // $("#Description").attr('readonly', 'readonly');
//                break;
//            case "NoPolisi":
                
//                //clear("dbclick");
//                widget.populate($.extend({}, data));
//                break;
//            default:
//                break;
//        }
//    });

   
//    function editDetail(row) {
//        $("#btnDlt").addClass("hide", "hide");
//        var data = {
//            PoliceRegNo: row[1],
//            CustomerCode: row[2],
//            ClubNo: row[3],
//            ClubDateStart: row[7],
//            ClubDateFinish: row[8],
//            ClubSince: row[9],
//            IsActiveP: row[10],
//            ChassisCode: row[13],
//            ChassisNo: row[14],
//        }
//        widget.populate(data, "#pnlinfoklub");

//    }
//    function deleteDetail(row) {
//        $("#btnDlt").removeClass("hide");
//        var data = {
//            PoliceRegNo: row[1],
//            CustomerCode: row[2],
//            ClubNo: row[3],
//            ClubDateStart: row[7],
//            ClubDateFinish: row[8],
//            ClubSince: row[9],
//            IsActiveP: row[10],
//            ChassisCode: row[13],
//            ChassisNo: row[14],
//        }
//        widget.populate(data, "#pnlinfoklub");

//    }
//    $("#btnSave").on("click", saveData);
//    $("#btnAdd").on("click", saveDataKedua);

//    $("#btnDelete").on("click", deleteData);
//    $("#btnDlt").on("click", deleteKedua);

//    function saveData(p) {
        
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            if (param.Description == '') {
//                alert("Description harus diisi");
//                return false;
//            }
//            widget.post("sv.api/club/save", param, function (result) {
//                if (result.success) {
//                    var code = result["ClubCodeStr"];
//                    $("#ClubCodeStr").val(code);
//                    $("#btnDelete").removeClass('hide');
//                    $("#pnlinfoklub").removeClass("hide");
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
//            alert(param.ClubDateStart);
//            if (param.PoliceRegNo == '') {
//                alert("No Polisi harus diisi");
//                return false;
//            } else if (param.CustomerCode == '') {
//                alert("Costumer code harus diisi");
//                return false;
//            } else if (param.ClubNo == '') {
//                alert("Club No harus diisi");
//                return false;
//            }
//            if (param.ClubDateStart >= param.ClubDateFinish) {
//                alert("Periode Awal Tidak boleh lebih besar atau sama dengan periode akhir");
//            } else {
//                widget.post("sv.api/club/savekedua", param, function (result) {

//                    if (result.success) {
//                        SimDms.Success("data saved...");
//                        getTable();
//                    }
//                });

//            }
//        }

//    }



//    function deleteData() {
//        if (confirm("Apakah anda yakin???")) {
//            var param = $(".main .gl-widget").serializeObject();
//            widget.post("sv.api/club/deletedata", param, function (result) {
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
//            widget.post("sv.api/club/deleteDataKedua", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data removed...");
//                    getTable();
//                } else {
//                    SimDms.Error("fail removed...");
//                    getTable();
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
//            $("#btnSave").removeClass('hide'); $("#btnEdit").addClass('hide');
//        }
//    }
  
//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/club/default", function (result) {
//            widget.default = $.extend({}, result);
//            widget.populate(widget.default);
           
//        });
//    }


//});