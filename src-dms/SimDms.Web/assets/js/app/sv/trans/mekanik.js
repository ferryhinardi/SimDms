var operationNo = "";
var serviceNo = "";

$(document).ready(function () {
    var options = {
        title: "Alokasi Mekanik",
        xtype: "panels",
        toolbars: [
            { name: "btnClear", text: "Clear", icon: "icon-refresh" },
            { name: "btnBrowse", text: "Browse", icon: "icon-search" },
        ],
        panels: [
            {
                title: "Service Information",
                items: [
                    {
                        text: "Branch",
                        type: "controls",
                        items: [
                            { name: "BranchCode", cls: "span2", placeHolder: "Branch Code", readonly: true },
                            { name: "BranchName", cls: "span6", placeHolder: "Branch Name", readonly: true }
                        ]
                    },
                    { name: "JobOrderNo", text: "SPK No", placeHolder: "XXX/YY/99999", cls: "span4" },
                    { name: "JobOrderDate", text: "SPK Date", cls: "span4", type: "datetimepicker" },
                    { name: "ServiceStatus", cls: "hide", readonly: true },
                    { name: "ServiceStatusDesc", text: "Service Status", readonly: true },
                    { name: "PoliceRegNo", text: "Police Reg No", cls: "span4", readonly: true },
                    { name: "ServiceBookNo", text: "Service Book No", cls: "span4", readonly: true },
                    { name: "BasicModel", text: "Basic Model", cls: "span4", readonly: true },
                    { name: "TransmissionType", text: "Trans Type", cls: "span4", readonly: true },
                    {
                        text: "Chassis",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "ChassisCode", placeHolder: "Chassis Code", cls: "span4", readonly: true },
                            { name: "ChassisNo", placeHolder: "Chassis No", cls: "span4", readonly: true }
                        ]
                    },
                    {
                        text: "Engine",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "EngineCode", placeHolder: "Engine Code", cls: "span4", readonly: true },
                            { name: "EngineNo", placeHolder: "Engine No", cls: "span4", readonly: true },
                        ]
                    },
                    { name: "ColorCode", text: "Color", cls: "span4", readonly: true },
                    { name: "Odometer", text: "Odometer", cls: "span4", readonly: true },
                ]
            },
            {
                title: "Customer & Vehicle",
                items: [
                    { name: "Customer", text: "Customer", cls: "span4", readonly: true },
                    { name: "CustomerBill", text: "Customer Bill", cls: "span4", readonly: true },
                    {
                        text: "Contact",
                        type: "controls",
                        cls: "span4",
                        items: [
                            { name: "PhoneNo", placeHolder: "Phone", cls: "span4", readonly: true },
                            { name: "FaxNo", placeHolder: "Fax", cls: "span4", readonly: true },
                            
                        ]
                    },
                    { name: "HPNo", text: "HP", cls: "span4", readonly: true },
                    { name: "ForemanName", text: "Foreman (FM)", cls: "span4", readonly: true },
                    { name: "JobType", text: "Job Type", cls: "span4", readonly: true },
                    { name: "ServiceRequestDesc", text: "Service Request", type:'textarea',readonly: true },
                    
                ]
            },
            {
                title: "Daftar Pekerjaan",
                xtype: "table",
                tblname: "tblTask",
                columns: [
                    { text: "&nbsp;&nbsp;#", type: "edit", width: 60 },
                    { name: "ServiceNo", cls: "hide" },
                    { name: "TypeOfGoodsDesc", text: "Type", width: 180 },
                    { name: "TaskPartNo", text: "Task No", width: 180 },
                    { name: "OprHourDemandQty", text: "Qty", width: 120, cls: "right" },
                    { name: "TaskStatus", text: "Status" },
                    { name: "TaskPartDesc", text: "Description" }
                ]
            },
            {
                title: "Alokasi Mekanik",
                xtype: "table",
                pnlname: "pnlTaskMechanic",
                tblname: "tblTaskMechanic",
                buttons: [{ name: "btnAddDtl", text: "Add Mechanic", icon: "icon-plus" }],
                items: [
                    {
                        text: "Mechanic",
                        type: "controls",
                        items: [
                                { name: "MechanicID", cls: "span2", placeHolder: "Code", type: "popup", btnName: "btnMechanicID", readonly: true },
                                { name: "MechanicName", cls: "span6", placeHolder: "Name", readonly: true }
                            ]                        
                    },
                    {
                        type: "controls",
                        text: "Service Start",
                        cls: "span3",
                        items: [
                            { name: "StartService", cls: "span6", type: "datetimepicker", format: "dd-MMM-yyyy HH:mm" },
                        ]
                    },
                    {
                        type: "controls",
                        text: "Service End",
                        cls: "span3",
                        items: [
                            { name: "FinishService", cls: "span6", type: "datetimepicker" },
                        ]
                    },
                    {
                        name: "MechanicStatus", text: "Status", type: "select", cls: "span6",
                        items: [
                            { value: '0', text: 'Open Task' },
                            { value: '1', text: 'Work In Progress' },
                            { value: '2', text: 'Close Task' }
                        ]
                    },
                    {
                        type: "buttons", items: [
                            { name: "btnSaveDtl", text: "Save", icon: "icon-save" },
                            { name: "btnCancelDtl", text: "Cancel", icon: "icon-undo" }
                        ]
                    },
                ],
                columns: [
                    { text: "Action", type: "action", width: 80 },
                    { name: "MechanicId", cls: "hide" },
                    { name: "Mechanic", text: "NIK", width: 180 },
                    { name: "StartService", text: "Service Start", type: "dateTime", width: 200 },
                    { name: "FinishService", text: "Service Finish", type: "dateTime", width: 200 },
                    { name: "MechanicStatus", cls: "hide" },
                    { name: "MechanicStatusDesc", text: "Status", width: 160 },
                    { name: "EmployeeName", text: "Mekanik" },
                ]
            }
        ],
    }
    var widget = new SimDms.Widget(options);

    widget.default = {};

    widget.render(function () {
        SetDefault();
    });


 
    function SetDefault() {
        $("input[type=text]").val("");
        $("#ServiceRequestDesc").val("");
        $("#JobOrderDate, #btnAddDtl").attr("disabled", "disabled");
        widget.populateTable({ selector: "#tblTask", data: "" });
        widget.populateTable({ selector: "#tblTaskMechanic", data: "" });
        $.post("sv.api/mechanic/default", function (result) {
            widget.default = $.extend({}, result, { JobOrderDate: "" });
            widget.populate(widget.default);
        });
    };

    function SetServiceStatus(status) {
        var serviceStatus = "0,1,2,3,4";
        (serviceStatus.indexOf(status) > -1) ? $("#btnAddDtl").removeAttr("disabled") : $("#btnAddDtl").attr("disabled", "disabled");
    };


    widget.onRowClick(function (idx, row, selector) {
        console.log(idx);
        console.log(row);
        console.log(selector);
     
        if (selector == "#tblTask") {
            populateTaskMechanic(row);
        }
    });

    widget.onTableClick(function (icon, row, selector) {
        switch (selector.selector) {
            case "#tblTask":
                console.log(icon);
                console.log(row);
                console.log(selector);
                switch (icon) {
                    case "edit":
                        var data = {
                            ServiceNo: row[1],
                            TaskPartNo: row[3]
                        }
                        populateTaskMechanic(data)
                        break;
                    default:
                        break;
                }
                break;
            case "#tblTaskMechanic":
                var data = {
                    ServiceNo: serviceNo,
                    OperationNo: operationNo,
                    MechanicId: row[1],
                }
                switch (icon) {
                    case "edit":
                        editDetail(row);
                        break;
                    case "trash":
                        deleteDetail(data);
                        break;
                    default:
                        break;
                } 
                break;
            default: break;
        }
        
    });

    $("#btnClear").on("click", function () {
        SetDefault();
    });

    $("#btnBrowse").on("click", function () {
        LookupJobOrder();
    });

    $('#btnMechanicID').on('click', function (e) {
        var lookup = widget.klookup({
            name: "MechanicList",
            title: "Mekanik",
            url: "sv.api/grid/Mechanics",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { field: 'EmployeeName', dir: 'asc' },
                { field: 'EmployeeID', dir: 'asc' }
            ],
            columns: [
                { field: "EmployeeName", title: "Nama Mekanik", width: 200 },
                { field: "EmployeeID", title: "NIK", width: 130 }
            ],
        });
        lookup.dblClick(function (data) {
            $('[name=MechanicID]').val(data.EmployeeID);
            $('[name=MechanicName]').val(data.EmployeeName);
        });
    });

    $("#JobOrderNo").on("blur", function () {
        var parData = { JobOrderNo: $(this).val() };
        widget.post("sv.api/grid/TrnServiceMechanics", parData, function (result) {
            if (result.data != null) {
                widget.populate(result.data);
                populateTask(parData);
                SetServiceStatus(result.data.ServiceStatus);
            }
            else {
                SetDefault();
                LookupJobOrder();
            }
        })
    });

    $('#btnAddDtl').on('click', function (e) {
        var now = new Date();
        var next = moment(now).add('m', 30).toDate();
        $('#MechanicID').val("");
        $('#MechanicName').val("");
        $('#MechanicStatus').val("");
        $('#btnMechanicID').removeAttr('disabled');
        $('#StartService').datetimepicker('setDate', now);
        $('#FinishService').datetimepicker('setDate', next);
        $("#pnlTaskMechanic").slideDown();
        $("#tblTaskMechanic td .icon").removeClass("link");
        $("#btnAddDtl").parent().hide();
    });

    $("#btnSaveDtl").on("click", function () {
        var mechanicID = $('[name=MechanicID]').val();
        if (mechanicID == '') {
            alert('Mekanik belum dipilih');
            return;
        }
        var mechanicStatus = $('#MechanicStatus').val();
        if (mechanicStatus == '') {
            alert('Status mekanik belum dipilih');
            return;
        }

        var start = $('#StartService').datetimepicker('getDate');
        var finish = $('#FinishService').datetimepicker('getDate');

        if (start > finish) {
            alert("Selesai Servis minimal harus lebih besar atau sama \n dengan Mulai Servis");
            return;
        }

        var data = {
            ServiceNo: serviceNo,
            OperationNo: operationNo,
            StartService: convertToNETDate(start),
            FinishService: convertToNETDate(finish),
            MechanicID: mechanicID,
            MechanicStatus: mechanicStatus
        }

        widget.post("sv.api/mechanic/InsertMechanic", data, function (result) {
            if (result.Message == "") {
                var parData = { JobOrderNo: $('#JobOrderNo').val() };
                populateTask(parData);
            }
            else {
                alert(result.Message);
                return;
            }
        });

        listDetail();
    });

    $("#btnCancelDtl").on("click", function () { listDetail(); });

    function LookupJobOrder() {
        var lookup = widget.klookup({
            name: "JobOrderList",
            title: "Input Perawatan Kendaraan (SPK)",
            url: "sv.api/grid/TrnServiceMechanics",
            serverBinding: true,
            pageSize: 10,
            sort: [
                { field: 'JobOrderNo', dir: 'desc' },
                { field: 'JobOrderDate', dir: 'desc' }
            ],
            columns: [
                { field: "JobOrderNo", title: "JobOrder No", width: 130 },
                {
                    field: "JobOrderDate", title: "JobOrder Date", width: 130,
                    template: "#= (JobOrderDate == undefined) ? '' : moment(JobOrderDate).format('DD MMM YYYY') #"
                },
                { field: "Customer", title: "Customer", width: 350 },
                { field: "PoliceRegNo", title: "Police Reg", width: 110 },
                { field: "ServiceBookNo", title: "Service Book", width: 120 },
                { field: "ForemanName", title: "Foreman", width: 160 },
                { field: "ServiceStatus", title: "Status", width: 60 },
                { field: "ServiceStatusDesc", title: "Status Desc.", width: 160 },
            ],
        });
        lookup.dblClick(function (data) {
            widget.populate(data);
            var parData = { JobOrderNo: data.JobOrderNo };
            populateTask(parData);
            SetServiceStatus(data.ServiceStatus);
        });
    }

    function populateTask(data) {
        if (data != undefined) {
            widget.post("sv.api/mechanic/GetTaskDetail", data, function (result) {
                if (result.Message == "") {
                    widget.populateTable({ selector: "#tblTask", data: result.List, selectable: true });
                    
                    populateTaskMechanic(result.List[0])
                } else {
                    $("input[type=text]").not("#BranchCode,#BranchName,#JobOrderNo").val("");
                    MsgBox(result.Message, MSG_WARNING);
                    return;
                }
            });
        }
        else {
            widget.populateTable({ selector: "#tblTask", data: "" });
            populateTaskMechanic(undefined);
        }
    }

    function populateTaskPerRow(data, row) {
        console.log(data);
        console.log(row);
        if (data != undefined) {
            widget.post("sv.api/mechanic/GetTaskDetail", data, function (result) {
                if (result.Message == "") {
                    widget.populateTable({ selector: "#tblTask", data: result.List, selectable: true });
                    populateTaskMechanic(result.List[row])
                } else {
                    $("input[type=text]").not("#BranchCode,#BranchName,#JobOrderNo").val("");
                    MsgBox(result.Message, MSG_WARNING);
                    return;
                }
            });
        }
    }

    function populateTaskMechanic(data) {
        if (data != undefined) {
            serviceNo = data.ServiceNo;
            operationNo = data.TaskPartNo;
            widget.post("sv.api/mechanic/GetTaskMechanic", data, function (result) {
                if (result.Message == "") {
                    widget.populateTable({ selector: "#tblTaskMechanic", data: result.List });
                } else {
                    alert(result.Message);
                    return;
                }
            });
        }
        else {
            widget.populateTable({ selector: "#tblTaskMechanic", data: "" });
        }
    }

    function editDetail(row) {
        $('#btnMechanicID').attr('disabled', 'disabled');
        $("#pnlTaskMechanic").slideDown();
        $("#tblTaskMechanic td .icon").removeClass("link");
        $("#btnAddDtl").parent().hide();

        var data = {
            MechanicID: row[1],
            StartService: row[3],
            FinishService: row[4],
            MechanicStatus: row[5],
            MechanicName: row[7]
        }
        widget.populate(data, "#pnlTaskMechanic");
        var start = moment(data.StartService, 'DD-MMM-YYYY HH:mm:ss');
        var finish = moment(data.FinishService, 'DD-MMM-YYYY HH:mm:ss');
        
        $('#StartService').datetimepicker('setDate', start.toDate());
        $('#FinishService').datetimepicker('setDate', finish.toDate());
    }

    function deleteDetail(data) {
        var status = $('#ServiceStatus').val();
        if (status > 1) {
            alert("Data tidak dapat dihapus karena status SPK sedang dalam proses pengerjaan");
            return;
        }

        if (confirm("Anda yakin akan menghapus data ini?")) {
            widget.post("sv.api/mechanic/RemoveTaskMechanic", data, function (result) {
                if (result.Message == "") {
                    console.log(result.List);
                    var parData = { JobOrderNo: result.List.JobOrderNo };
                    populateTask(parData);
                } else {
                    alert(result.Message);
                    return;
                }
            });
        };
    }

    function listDetail() {
        $("#pnlTaskMechanic").slideUp();
        $("#tblTaskMechanic td .icon").addClass("link");
        $("#btnAddDtl").parent().show();
    }

    // mm/dd/yyyy HH:MM:ss
    function convertToNETDate(date) {
        var m = moment(date);

        var mm = pad(m.month() + 1, 2, '0');
        var dd = pad(m.date(), 2, '0');
        var yyyy = pad(m.year(), 4, '0');
        var HH = pad(m.hour(), 2, '0');
        var MM = pad(m.minute(), 2, '0');
        var ss = pad(m.second(), 2, '0');

        var netDate = mm + "/" + dd + "/" + yyyy + " " + HH + ":" + MM + ":" + ss;
        return netDate;
    }

    function pad(n, width, z) {
        z = z || '0';
        n = n + '';
        return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
    }
});