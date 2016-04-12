var totalSrvAmt = 0;
var status = 'N';
var svType = '2';

function PaketServiceController($scope, $http, $injector) {

    var me = $scope;

    $injector.invoke(BaseController, this, { $scope: me });

    me.initialize = function () {
        me.data.IntervalYear = 3;
        me.data.IntervalKM = 50000;
        me.data.PackageSrvSeq = 0;
        me.data.DiscPct = 0;

        me.detail1 = {};
        me.detail2 = {};

        $('#pnlTaskDetail').hide();
        $('#pnlPartDetail').hide();
        $('#wxTaskDetail').hide();
        $('#wxPartDetail').hide();
    }

    me.gridTaskDetail = new webix.ui({
        container: "wxTaskDetail",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
            { id: "OperationNo", header: "Task No", width: 300 },
            { id: "OperationName", header: "Task Name", width: 550 },
            { id: "DiscPct", header: "Discon (%)", width: 200 },
        ],
        on: {
            onSelectChange: function () {
                if (me.gridTaskDetail.getSelectedId() !== undefined) {
                    me.detail1 = this.getItem(me.gridTaskDetail.getSelectedId().id);
                    me.detail1.oid = me.gridTaskDetail.getSelectedId();
                    me.Apply();
                    $('#OperationNo').attr('disabled', 'disabled');
                    $('#btnOperationNo').attr('disabled', 'disabled');
                }
            }
        }
    });

    me.griddPartDetail = new webix.ui({
        container: "wxPartDetail",
        view: "wxtable", css:"alternating",
        scrollX: true,
        columns: [
             { id: "PartNo", header: "Part No", width: 300 },
             { id: "PartName", header: "Part Name", width: 550 },
             { id: "DiscPct", header: "Discon (%)", width: 200 },
        ],
        on: {
            onSelectChange: function () {
                if (me.griddPartDetail.getSelectedId() !== undefined) {
                    me.detail2 = this.getItem(me.griddPartDetail.getSelectedId().id);
                    me.detail2.oid = me.griddPartDetail.getSelectedId();
                    me.Apply();
                    $('#PartNo').attr('disabled', 'disabled');
                    $('#btnPartNo').attr('disabled', 'disabled');
                }
            }
        }
    });

    me.browse = function () {
        var lookup = Wx.blookup({
            name: "PackageBrowse",
            title: "List Package Service Browse",
            manager: MasterService,
            query: "PackageBrowse",
            defaultSort: "PackageCode asc",
            columns: [
                { field: "PackageCode", Title: "Package Code", Width: "110px" },
                { field: "PackageName", Title: "Package Name", Width: "180px" },
                { field: "BasicModel", Title: "Basic Model", Width: "110px" },
                { field: "BillTo", Title: "Bill To", Width: "100px" },
                { field: "IntervalYear", Title: "Interval Year", Width: "80px" },
                { field: "IntervalKM", Title: "IntervalKM", Width: "80px" },
            ]
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.lookupAfterSelect(result);
                me.startEditing();
                me.LoadTask(result);
                me.LoadPart(result);
                $('#pnlTaskDetail').show();
                $('#pnlPartDetail').show();
                $('#wxTaskDetail').show();
                $('#wxPartDetail').show();
                me.Apply();
            }
        });
    }

    me.LoadTask = function (data) {
        $http.post('sv.api/package/LoadTask?PackageCode=' + data.PackageCode).
           success(function (data, status, headers, config) {
               me.grid.detail = data;
               me.loadTableData(me.gridTaskDetail, me.grid.detail);
           }).
           error(function (e, status, headers, config) {
               console.log(e);
           });
    }

    me.LoadPart = function (data) {
        $http.post('sv.api/package/LoadPart?PackageCode=' + data.PackageCode).
           success(function (data, status, headers, config) {
               me.grid.detail = data;
               me.loadTableData(me.griddPartDetail, me.grid.detail);
           }).
           error(function (e, status, headers, config) {
               console.log(e);
           });
    }

    me.BasicModel = function () {
        var lookup = Wx.blookup({
            name: "CBasmodOpen",
            title: "Master Model",
            manager: MasterService,
            query: "CBasmodOpen",
            defaultSort: "BasicModel Asc",
            columns: [
                { field: "BasicModel", Title: "Basic Model", Width: "110px" },
                { field: "TechnicalModelCode", Title: "Technical Model Code", Width: "110px" },
                { field: "ModelDescription", Title: "Description", Width: "80px" },
                { field: "Status", Title: "Status", Width: "80px" },
            ],
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.BasicModel = result.BasicModel;
                me.data.ModelDescription = result.ModelDescription;
                me.Apply();
            }
        });

    }

    me.JobType = function () {
        var lookup = Wx.blookup({
            name: "JobView",
            title: "Master Pekerjaan",
            manager: MasterService,
            query: "JobView",
            defaultSort: "JobType Asc",
            columns: [
                { field: "JobType", Title: "Refference Code", Width: "110px" },
                { field: "JobDescription", Title: "Description", Width: "110px" },
                { field: "DescriptionEng", Title: "Description(Eng.)", Width: "80px" },
                { field: "Status", Title: "Is Active", Width: "80px" },
            ],
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.JobType = result.JobType;
                me.data.JobDescription = result.JobDescription;
                me.Apply();
            }
        });

    }

    me.BillTo = function () {
        var lookup = Wx.blookup({
            name: "PaymentOpen",
            title: "Pembayaran LookUp",
            manager: MasterService,
            query: "PaymentOpen",
            defaultSort: "BillTo Asc",
            columns: [
                { field: "BillTo", Title: "Customer Code", Width: "110px" },
                { field: "CustomerName", Title: "Customer Name", Width: "110px" },
                { field: "Address", Title: "Address", Width: "180px" },
            ],
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.data.BillTo = result.BillTo;
                me.data.CustomerName = result.CustomerName;
                me.Apply();
            }
        });

    }

    me.saveData = function (e, param) {
        $http.post('sv.api/package/save', me.data).
            success(function (data, status, headers, config) {
                if (data.success) {
                    Wx.Success("Data saved...");
                    me.startEditing();
                    $('#pnlTaskDetail').show();
                    $('#pnlPartDetail').show();
                    $('#wxTaskDetail').show();
                    $('#wxPartDetail').show();
                } else {
                    MsgBox(data.message, MSG_ERROR);
                }
            }).
            error(function (e, status, headers, config) {
                console.log(e);
            });
    }

    me.OperationNo = function () {
        var lookup = Wx.blookup({
            name: "OperationPackage",
            title: "Master Pekerjaan Lookup",
            manager: MasterService,
            query: new breeze.EntityQuery().from("OperationPackage").withParameters({ BasicModel: me.data.BasicModel, JobType: me.data.JobType }),
            defaultSort: "OperationNo Asc",
            columns: [
                { field: "OperationNo", title: "Operation No", Width: "110px" },
                { field: "OperationName", title: "Operation Name", Width: "110px" },
                { field: "OperationHour", title: "Operation Hour", Width: "110px" },
                { field: "LaborPrice", title: "LaborPrice", Width: "110px" },
                { field: "IsActive", title: "IsActive", Width: "110px" },
            ],
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.detail1.OperationNo = result.OperationNo;
                me.detail1.OperationName = result.OperationName;
                $('#AddTasks').removeAttr('disabled');
                me.Apply();
            }
        });

    }

    me.PartNo = function () {
        var lookup = Wx.blookup({
            name: "NoPartPack",
            title: "Master Part Lookup",
            manager: MasterService,
            query: "NoPartPack",
            defaultSort: "PartNo Asc",
            columns: [
                { field: "PartNo", title: "PartNo", Width: "110px" },
                { field: "PartName", title: "PartName", Width: "110px" },
                { field: "RetailPriceInclTax", title: "RetailPriceInclTax", Width: "110px" },
                { field: "Status", title: "Status", Width: "110px" },
            ],
        });
        lookup.dblClick(function (result) {
            if (result != null) {
                me.detail2.PartNo = result.PartNo;
                me.detail2.PartName = result.PartName;
                me.Apply();
            }
        });

    }

    me.AddTasks = function (e, param) {
        if (me.detail1.OperationNo == undefined || me.detail1.OperationName == undefined) {
            MsgBox("Ada Informasi Yang Belum Lengkap!", MSG_ERROR);
        } else {
            $http.post('sv.api/package/savetask', { model: me.data, TasksModel: me.detail1 }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.clearTable(me.gridTaskDetail);
                        me.LoadTask(data.data);
                        me.detail1 = {};
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    console.log(e);
                });
        }

    }

    me.AddPart = function (e, param) {
        if (me.detail2.PartNo == undefined || me.detail2.PartName == undefined) {
            MsgBox("Ada Informasi Yang Belum Lengkap!", MSG_ERROR);
        } else {
            $http.post('sv.api/package/savepart', { model: me.data, PartModel: me.detail2 }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data saved...");
                        me.clearTable(me.griddPartDetail);
                        me.LoadPart(data.data);
                        me.detail2 = {};
                    } else {
                        MsgBox(data.message, MSG_ERROR);
                    }
                }).
                error(function (e, status, headers, config) {
                    console.log(e);
                });
        }

    }

    me.CloseTasks = function () {
        me.detail1 = {};
        me.gridTaskDetail.clearSelection();
        $('#OperationNo').removeAttr('disabled');
        $('#btnOperationNo').removeAttr('disabled');

    }

    me.ClosePart = function () {
        me.detail2 = {};
        me.griddPartDetail.clearSelection();
        $('#PatrNo').removeAttr('disabled');
        $('#btnPartNo').removeAttr('disabled');

    }

    me.delete = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sv.api/package/deletedata', me.data).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        me.init();
                        me.initialize();
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

    me.DeleteTasks = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sv.api/package/deletetask', { model: me.data, TasksModel: me.detail1 }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data deleted...");
                        me.clearTable(me.gridTaskDetail);
                        me.LoadTask(data.data[0]);
                        me.CloseTasks()
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

    me.DeletePart = function () {
        MsgConfirm("Are you sure to delete current record?", function (result) {
            if (result) {
                $http.post('sv.api/package/deletepart', { model: me.data, PartModel: me.detail2 }).
                success(function (data, status, headers, config) {
                    if (data.success) {
                        Wx.Success("Data deleted...");
                        me.clearTable(me.griddPartDetail);
                        me.LoadPart(data.data[0]);
                        me.ClosePart();
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
                    { name: "btnSave", text: "Save", cls: "btn bAddTaskstn-success", icon: "icon-save", show: "hasChanged && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "save()", disable: "!isSave" },
                    { name: "btnCancel", text: "Cancel", cls: "btn btn-warning", icon: "icon-remove", show: "(hasChanged || isLoadData) && !isInitialize && (!isEQAvailable || !isEXLSAvailable) ", click: "cancelOrClose()" },
                    { name: "btnPrintPreview", text: "Print", cls: "btn btn-primary", icon: "icon-print", click: "printPreview()", disable: "!isPrintEnable" }
        ],
        panels: [
            {
                name: "pnlPackageInfo",
                title: "Package Information",
                items: [

                    {
                        text: "Service Package",
                        type: "controls",
                        required: true,
                        items: [
                            {
                                name: "PackageCode", cls: "span2", text: "Package Code", required: true, validasi: "required"
                            },
                            {
                                name: "PackageName", text: "Package Name", cls: "span6", required: true, validasi: "required"
                            },

                        ]
                    },
                    {
                        text: "Basic Model",
                        type: "controls",
                        required: true,
                        items: [
                            {

                                name: "BasicModel", cls: "span2", text: "Basic Model", type: "popup", readonly: true, btnName: "btnBasicModel",
                                required: true, validasi: "required", click: "BasicModel()"
                            },
                            {

                                name: "ModelDescription", cls: "span6", text: "Description", readonly: true,
                            },

                        ]
                    },
                    {
                        text: "Job Type",
                        type: "controls",
                        required: true,
                        items: [
                            {
                                name: "JobType", cls: "span2", text: "Job Type", type: "popup", readonly: true, btnName: "btnJobType",
                                required: true, validasi: "required", click: "JobType()"
                            },
                            {
                                name: "JobDescription", cls: "span6", text: "Job Description", readonly: true,
                            },

                        ]
                    },
                    {
                        text: "Payment",
                        type: "controls",
                        required: true,
                        items: [
                            {
                                name: "BillTo", cls: "span2", text: "Customer Code", type: "popup", readonly: true, btnName: "btnBillTo",
                                required: true, validasi: "required", click: "BillTo()"
                            },
                            {

                                name: "CustomerName", cls: "span6", text: "Customer Name", readonly: true,
                            },

                        ]
                    },
                    {
                        name: "IntervalYear", cls: "span2 number", text: "Interval Year", required: true, validasi: "required"
                    },
                    {
                        name: "IntervalKM", cls: "span2 number", text: "Interval(KM)", required: true, validasi: "required"
                    },
                    {
                        name: "PackageSrvSeq", cls: "span2 number", text: "Package Seq", required: true, validasi: "required"
                    },
                    {
                        name: "PackageDesc", text: "Description", type: "textarea",
                    },

                ]
            },
            {
                title: "Tasks Detail",
                name: "pnlTaskDetail",
                items: [
                    {
                        text: "Jobs",
                        type: "controls",
                        cls: "span6",
                        readonly: true,
                        items: [
                            {
                                name: "OperationNo", model: "detail1.OperationNo", cls: "span3", text: "Operation No", type: "popup",
                                readonly: true, btnName: "btnOperationNo", required: true, validasi: "required", click: "OperationNo()"
                            },
                            {

                                name: "OperationName", model: "detail1.OperationName", cls: "span5", text: "Operation Name", readonly: true,
                            },

                        ]
                    },
                    {
                        name: "DiscPct", model: "detail1.DiscPct", cls: "span2 number", text: "Discon (%)", required: true, validasi: "required"
                    },
                    {
                        type: "buttons", cls: "span6", items: [
                                { name: "btnAddTasks", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddTasks()", show: "detail1.oid === undefined" },
                                { name: "btnUpdateTasks", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddTasks()", show: "detail1.oid !== undefined" },
                                { name: "btnDeleteTasks", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeleteTasks()", show: "detail1.oid !== undefined" },
                                { name: "btnCancelTasks", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "CloseTasks()", show: "detail1.oid !== undefined || detail1.oid == undefined" }
                        ]
                    },
                ]
            },
            {
                name: "wxTaskDetail",
                xtype: "wxtable",
            },
            {
                name: "pnlPartDetail",
                title: "Parts Detail",
                items: [
                    {
                        text: "Parts/Material",
                        type: "controls",
                        cls: "span6",
                        readonly: true,
                        items: [
                            {
                                name: "PartNo", model: "detail2.PartNo", cls: "span3", text: "Part No", type: "popup", readonly: true,
                                btnName: "btnPartNo", required: true, validasi: "required", click: "PartNo()"
                            },
                            {

                                name: "PartName", model: "detail2.PartName", cls: "span5", text: "Part Name", readonly: true,
                            },

                        ]
                    },
                    {
                        name: "DiscPct", model: "detail2.DiscPct", cls: "span2 number", text: "Discon (%)", required: true, validasi: "required"
                    },
                    {
                        type: "buttons", cls: "span6", items: [
                                { name: "btnAddPart", text: "Add", icon: "icon-plus", cls: "btn btn-info", click: "AddPart()", show: "detail2.oid === undefined" },
                                { name: "btnUpdatePart", text: "Update", icon: "icon-save", cls: "btn btn-success", click: "AddPart()", show: "detail2.oid !== undefined" },
                                { name: "btnDeletePart", text: "Delete", icon: "icon-remove", cls: "btn btn-danger", click: "DeletePart()", show: "detail2.oid !== undefined" },
                                { name: "btnCancelPart", text: "Cancel", icon: "icon-undo", cls: "btn btn-warning", click: "ClosePart()", show: "detail2.oid !== undefined || detail2.oid == undefined" }
                        ]
                    },
                ]
            },
            {
                name: "wxPartDetail",
                xtype: "wxtable",
            },


        ],
    };

    Wx = new SimDms.Widget(options);
    Wx.default = {};
    Wx.render(init);

    function init(s) {
        $(".mylabel").attr("style", "padding:9px 9px 0px 5px");
        SimDms.Angular("PaketServiceController");
    }

});

//$(document).ready(function () {
//    var options = {
//        title: "Service Package",
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
//                name: "pnlPackageInfo",
//                title: "Package Information",
//                items: [

//                    {
//                        text: "Service Package",
//                        type: "controls",
//                        items: [
//                            {
//                                name: "PackageCode",
//                                cls: "span2",
//                                text: "Package Code",
//                                required: "required"
//                            },
//                            {
//                                name: "PackageName",
//                                text: "Package Name",
//                                cls: "span6",
//                                required: "required"
//                            },

//                        ]
//                    },
//                    {
//                        text: "Basic Model",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "BasicModel",
//                                cls: "span2",
//                                text: "Basic Model",
//                                type: "popup",
//                                readonly: true,
//                                btnName: "btnBasicModel",
//                                required: "required"
//                            },
//                            {

//                                name: "ModelDescription",
//                                cls: "span6",
//                                text: "Description",
//                                readonly: true,
//                            },

//                        ]
//                    },
//                    {
//                        text: "Job Type",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "JobType",
//                                cls: "span2",
//                                text: "Job Type",
//                                type: "popup",
//                                readonly: true,
//                                btnName: "btnJobType",
//                                required: "required"
//                            },
//                            {

//                                name: "JobDescription",
//                                cls: "span6",
//                                text: "Job Description",
//                                readonly: true,
//                            },

//                        ]
//                    },
//                    {
//                        text: "Payment",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "BillTo",
//                                cls: "span2",
//                                text: "Customer Code",
//                                type: "popup",
//                                readonly: true,
//                                btnName: "btnBillTo",
//                                required: "required"
//                            },
//                            {

//                                name: "CustomerName",
//                                cls: "span6",
//                                text: "Customer Name",
//                                readonly: true,
//                            },

//                        ]
//                    },
//                    {
//                        name: "IntervalYear",
//                        cls: "span2 number",
//                        text: "Interval Year",
//                        required: "required"
//                    },
//                    {
//                        name: "IntervalKM",
//                        cls: "span2 number",
//                        text: "Interval(KM)",
//                        required: "required"
//                    },
//                    {
//                        name: "PackageSrvSeq",
//                        cls: "span2 number",
//                        text: "Package Seq",
//                        required: "required"
//                    },
//                    {
//                        name: "PackageDesc",
//                        text: "Description",
//                        type: "textarea",
//                    },

//                ]
//            },
//            {
//                title: "Tasks Detail",
//                xtype: "table",
//                pnlname: "pnlTaskDetail",
//                name: "TaskDetail",
//                cls: "hide",
//                tblname: "tblTaskDetail",
//                items: [
//                    {
//                        text: "Jobs",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "OperationNo",
//                                cls: "span2",
//                                text: "Operation No",
//                                type: "popup",
//                                readonly: true,
//                                btnName: "btnOperationNo",
//                                required: "required"
//                            },
//                            {

//                                name: "OperationName",
//                                cls: "span6",
//                                text: "Operation Name",
//                                readonly: true,
//                            },

//                        ]
//                    },
//                    {
//                        name: "DiscPct",
//                        cls: "span2 number",
//                        text: "Discon (%)",
//                        required: "required"
//                    },
//                    {
//                        type: "buttons", cls: "span6", items: [
//                            { name: "btnAddTask", text: "Add", icon: "icon-save", cls: "span2" },
//                            { name: "btnDltTask", text: "Delete", icon: "icon-remove", cls: "span2 hide" },
//                        ]
//                    },
//                ],
//                columns: [

//                    { text: "Action", type: "action", width: 80 },
//                    { name: "OperationNo", text: "Task No", width: 180 },
//                    { name: "OperationName", text: "Task Name", width: 180 },
//                    { name: "DiscPct", text: "Discon (%)", width: 100 },
//                ]
//            },
//            {
//                pnlname: "pnlPartDetail",
//                name: "PartDetail",
//                cls: "hide",
//                title: "Parts Detail",
//                xtype: "table",
//                tblname: "tblPartDetail",
//                items: [
//                    {
//                        text: "Parts/Material",
//                        type: "controls",
//                        items: [
//                            {

//                                name: "PartNo",
//                                cls: "span2",
//                                text: "Part No",
//                                type: "popup",
//                                readonly: true,
//                                btnName: "btnPartNo",
//                                required: "required"
//                            },
//                            {

//                                name: "PartName",
//                                cls: "span6",
//                                text: "Part Name",
//                                readonly: true,
//                            },

//                        ]
//                    },
//                    {
//                        name: "DiscPct",
//                        cls: "span2 number",
//                        text: "Discon (%)",
//                        required: "required"
//                    },
//                    {
//                        type: "buttons", cls: "span6", items: [
//                            { name: "btnAddPart", text: "Add", icon: "icon-save", cls: "span2" },
//                            { name: "btnDltPart", text: "Delete", icon: "icon-remove", cls: "span2 hide" },
//                        ]
//                    },
//                ],
//                columns: [

//                    { text: "Action", type: "action", width: 80 },
//                    { name: "PartNo", text: "Part No", width: 180 },
//                    { name: "PartName", text: "Part Name", width: 180 },
//                    { name: "DiscPct", text: "Discon (%)", width: 100 },

//                ]
//            }


//        ],
//    }

//    var widget = new SimDms.Widget(options);

//    widget.default = {};

//    widget.render(function () {
//        $.post('sv.api/package/default', function (result) {
//            widget.default = result;
//            widget.populate(result);

//        });
//    });

//    $("#pnlTaskDetail").slideDown();
//    $("#pnlPartDetail").slideDown();

//    $("#btnBasicModel").on("click", function () {
//         loadData('btn3');
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
//         loadData('btn3');
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

//    $("#btnBillTo").on("click", function () {
//         loadData('btn3');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "billto",
//            title: "Job Type",
//            source: "sv.api/grid/PaymentOpen",
//            sortings: [[1, "asc"]],
//            columns: [
//                { mData: "BillTo", sTitle: "Customer Code", sWidth: "110px" },
//                { mData: "CustomerName", sTitle: "Customer Name", sWidth: "110px" },
//                { mData: "Address", sTitle: "Address", sWidth: "180px" },


//            ]
//        });
//        widget.lookup.show();
//    });
//    $("#btnOperationNo").on("click", function () {
//        var param = $(".main .gl-widget").serializeObject();
//        var lookup = widget.klookup({
//            name: "PartNo",
//            title: "PartNo",
//            url: "sv.api/grid/OperationPackage?basmod=" + param.BasicModel + "&jobtype=" + param.JobType,
//            serverBinding: true,
//            pageSize: 20,
//            filters: [
//                {
//                    name: "OperationNo",
//                    text: "Operation No",
//                    cls: "span4",
//                },
//                {
//                    name: "OperationName",
//                    text: "Operation Name",
//                    cls: "span4",
//                },

//            ],
//            columns: [
//                { field: "OperationNo", title: "Operation No", sWidth: "110px" },
//                { field: "OperationName", title: "Operation Name", sWidth: "110px" },
//                { field: "OperationHour", title: "Operation Hour", sWidth: "110px" },
//                { field: "LaborPrice", title: "LaborPrice", sWidth: "110px" },
//                { field: "IsActive", title: "IsActive", sWidth: "110px" },
//            ],
//        });
//        lookup.dblClick(function (data) {
//            widget.populate(data);


//        });
//    });

//    $("#btnPartNo").on("click", function () {
//        var lookup = widget.klookup({
//            name: "PartNo",
//            title: "PartNo",
//            url: "sv.api/grid/NoPartPack",
//            serverBinding: true,
//            pageSize: 20,
//            filters: [
//                {
//                    name: "PartNo",
//                    text: "PartNo",
//                    cls: "span4",
//                },
//                {
//                    name: "PartName",
//                    text: "PartName",
//                    cls: "span4",
//                },

//            ],
//            columns: [
//                { field: "PartNo", title: "PartNo", sWidth: "110px" },
//                { field: "PartName", title: "PartName", sWidth: "110px" },
//                { field: "RetailPriceInclTax", title: "RetailPriceInclTax", sWidth: "110px" },
//                { field: "Status", title: "Status", sWidth: "110px" },
//            ],
//        });
//        lookup.dblClick(function (data) {
//            widget.populate(data);


//        });
//    });



//    $("#btnBrowse").on("click", function () {
//          loadData('browse');
//        var param = $(".main .gl-widget").serializeObject();
//        widget.lookup.init({
//            name: "Browse",
//            title: "Event",
//            source: "sv.api/grid/PackageBrowse",
//            sortings: [[0, "asc"]],
//            columns: [
//                { mData: "PackageCode", sTitle: "Package Code", sWidth: "110px" },
//                { mData: "PackageName", sTitle: "Package Name", sWidth: "180px" },
//                { mData: "BasicModel", sTitle: "Basic Model", sWidth: "110px" },
//                { mData: "BillTo", sTitle: "Bill To", sWidth: "100px" },
//                { mData: "IntervalYear", sTitle: "Interval Year", sWidth: "80px" },
//                { mData: "IntervalKM", sTitle: "IntervalKM", sWidth: "80px" },
//            ]
//        });
//        widget.lookup.show();
//    });



//    widget.lookup.onDblClick(function (e, data, name) {
//        widget.lookup.hide();
//        switch (name) {
//            case "Browse":
//                widget.populate($.extend({}, widget.default, data));
//                widget.lookup.hide();
//                clear("dbclick");
//                BasmodDetail();
//                JobDetail();
//                PayDetail();
//                LoadTask();
//                LoadPart();
//                break;
//            case "basmod":
//                widget.populate($.extend({}, data));
//                break;
//            case "jobtype":
//                widget.populate($.extend({}, data));
//                break;
//            case "billto":
//                widget.populate($.extend({}, data));
//                break;
//            case "PartNo":
//                widget.populate($.extend({}, data));
//                break;
//            default:
//                break;
//        }
//    });

//    function BasmodDetail() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/package/BasmodDetail", param, function (result) {
//            widget.populate($.extend({}, result));
//        });
//    }

//    function JobDetail() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/package/JobDetail", param, function (result) {
//            widget.populate($.extend({}, result));
//        });
//    }

//    function PayDetail() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/package/PayDetail", param, function (result) {
//            widget.populate($.extend({}, result));
//        });
//    }

//    function LoadTask() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/package/LoadTask", param, function (result) {
//            widget.populateTable({ selector: "#tblTaskDetail", data: result });

//        });
//    }

//    function LoadPart() {
//        var param = $(".main .gl-widget").serializeObject();
//        widget.post("sv.api/package/LoadPart", param, function (result) {
//            widget.populateTable({ selector: "#tblPartDetail", data: result });
//        });
//    }

//    widget.onTableClick(function (icon, row, selector) {
//        switch (selector.selector) {
//            case "#tblTaskDetail":
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
//            case "#tblPartDetail":
//                switch (icon) {
//                    case "edit":
//                        editDetail2(row);
//                        break;
//                    case "trash":
//                        deleteDetail2(row);
//                        break;
//                    default:
//                        break;
//                }
//                break;
//            default: break;
//        }

//    });


//    function editDetail(row) {
//        $("#btnDltTask").addClass("hide", "hide");
//        if (row[3] == 0) {
//            var data = {
//                OperationNo: row[1],
//                OperationName: row[2],
//                DiscPct: "0.00",
//            }
//        } else {
//            var data = {
//                OperationNo: row[1],
//                OperationName: row[2],
//                DiscPct: row[3],
//            }
//        }
//        widget.populate(data, "#pnlTaskDetail");

//    }
//    function deleteDetail(row) {
//        $("#btnDltTask").removeClass("hide");
//        if (row[3] == 0) {
//            var data = {
//                OperationNo: row[1],
//                OperationName: row[2],
//                DiscPct: "0.00",
//            }
//        } else {
//            var data = {
//                OperationNo: row[1],
//                OperationName: row[2],
//                DiscPct: row[3],
//            }
//        }
//        widget.populate(data, "#pnlTaskDetail");

//    }

//    function editDetail2(row) {
//        $("#btnDltPart").addClass("hide", "hide");
//        if (row[3] == 0) {
//            var data = {
//                PartNo: row[1],
//                PartName: row[2],
//                DiscPct: "0.00",
//            }
//        } else {
//            var data = {
//                PartNo: row[1],
//                PartName: row[2],
//                DiscPct: row[3],
//            }
//        }
//        widget.populate(data, "#pnlPartDetail");

//    }
//    function deleteDetail2(row) {
//        $("#btnDltPart").removeClass("hide");
//        if (row[3] == 0) {
//            var data = {
//                PartNo: row[1],
//                PartName: row[2],
//                DiscPct: "0.00",
//            }
//        } else {
//            var data = {
//                PartNo: row[1],
//                PartName: row[2],
//                DiscPct: row[3],
//            }
//        }
//        widget.populate(data, "#pnlPartDetail");

//    }
//    $("#btnSave").on("click", saveData);
//    $("#btnAddTask").on("click", saveTask);
//    $("#btnAddPart").on("click", savePart);

//    $("#btnDelete").on("click", deleteData);
//    $("#btnDltTask").on("click", deleteTask);
//    $("#btnDltPart").on("click", deletePart);

//    $('#btnCreate').on('click', function (e) {
//        clear("new");
//    });

//    $('#btnEdit').on('click', function (e) {
//        clear("btnEdit");

//    });
//    function saveData() {
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            param.IntervalYear = parseFloat(param.IntervalYear);
//            param.IntervalKM = parseFloat(param.IntervalKM);
//            param.PackageSrvSeq = parseFloat(param.PackageSrvSeq);
//            widget.post("sv.api/package/save", param, function (result) {
//                if (result.success) {
//                    $("#TaskDetail").removeClass('hide');
//                    $("#PartDetail").removeClass('hide');
//                    SimDms.Success("data saved...");
//                }
//            });
//        }
//    }
//    function saveTask() {
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            param.DiscPct = parseFloat(param.DiscPct);
//            widget.post("sv.api/package/savetask", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data saved...");
//                    $("#OperationNo").val("");
//                    $("#OperationName").val("");
//                    $("#pnlTaskDetail #DiscPct").val("0.00");
//                    LoadTask();
//                }
//            });
//        }
//    }

//    function savePart() {
//        var isValid = $(".main form").valid();
//        if (isValid) {
//            var param = $(".main form").serializeObject();
//            param.DiscPct = parseFloat($("#pnlPartDetail #DiscPct").val());
//            widget.post("sv.api/package/savepart", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data saved...");
//                    $("#PartNo").val("");
//                    $("#PartName").val("");
//                    $("#pnlPartDetail #DiscPct").val("0.00");
//                    LoadPart();
//                }
//            });
//        }
//    }

//    function deleteData() {
//        if (confirm("Apakah anda yakin???")) {
//            var param = $(".main .gl-widget").serializeObject();
//            widget.post("sv.api/package/deletedata", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data deleted...");
//                    clear("new");
//                } else {
//                    SimDms.Error("fail deleted...");
//                }
//            });
//        }
//    }

//    function deleteTask() {
//        if (confirm("Apakah anda yakin???")) {
//            var param = $(".main .gl-widget").serializeObject();
//            widget.post("sv.api/package/deletetask", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data deleted...");
//                    $("#OperationNo").val("");
//                    $("#OperationName").val("");
//                    $("#DiscPct").val("0.00");
//                    $("#btnDltTask").addClass("hide", "hide");
//                    LoadTask();
//                } else {
//                    SimDms.Error("fail deleted...");
//                }
//            });
//        }
//    }

//    function deletePart() {
//        if (confirm("Apakah anda yakin???")) {
//            var param = $(".main .gl-widget").serializeObject();
//            widget.post("sv.api/package/deletepart", param, function (result) {
//                if (result.success) {
//                    SimDms.Success("data deleted...");
//                    $("#PartNo").val("");
//                    $("#PartName").val("");
//                    $("#DiscPct").val("0.00");
//                    $("#btnDltPart").addClass("hide", "hide");
//                    LoadPart();
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

//            $("#TaskDetail").removeClass('hide');
//            $("#PartDetail").removeClass('hide');
//            $("#btnEdit").removeClass('hide');
//            $("#btnDelete").removeClass('hide');

//        } else if (p == "new") {
//             widget.clearForm();
//            $("#TaskDetail").addClass("hide", "hide");
//            $("#PartDetail").addClass("hide", "hide");
//            clearData();
//            $("#btnEdit").addClass("hide");
//            $("#btnDelete").addClass("hide");
//                $("#Description").attr('readonly', 'readonly');


//        } else if (p == "btnEdit") {
//            $("#btnSave").removeClass('hide');
//        }
//    }

//    function clearData() {
//        widget.clearForm();
//        widget.post("sv.api/package/default", function (result) {
//            widget.default = $.extend({}, result);
//            widget.populate(widget.default);

//        });
//    }


//});
