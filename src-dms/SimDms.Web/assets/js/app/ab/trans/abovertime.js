$(document).ready(function () {
    console.log("maintain overtime is ready ...");

    var options = {
        name: "panelMain",
        title: "Maintain Overtime",
        xtype: "panels",
        panels: [
            {
                name: "panelFilter",
                title: "Filter Selection",
                items: [
                    { name: "Department", text: "Department", cls: "span4", type: "select", opt_text: "--SELECT ALL--" },
                    { name: "Position", text: "Position", cls: "span4", type: "select", opt_text: "--SELECT ALL--" },
                    { name: "AttdDate", text: "Attendance Date", cls: "span4", type: "datepicker", required: true },
                    { type: "buttons", items: [{ name: "btnFilter", text: "Search", icon: "icon-search" }] },
                ]
            },
            {
                name: "panelEdit",
                title: "Edit Data",
                items: [
                     {
                         text: "NIK - Employee",
                         type: "controls",
                         items: [
                             { name: "EmployeeID", cls: "span2", placeHolder: "NIK", readonly: true },
                             { name: "EmployeeName", cls: "span6", placeHolder: "Name", readonly: true }
                         ]
                     },
                     {
                         text: "Duty Time - Clock Time",
                         type: "controls",
                         items: [
                             { name: "OnDutyTime", cls: "span2", placeHolder: "On Duty Time", readonly: true },
                             { name: "ClockInTime", cls: "span2", placeHolder: "Clock In Time", readonly: false },
                             { name: "OffDutyTime", cls: "span2", placeHolder: "Off Duty Time", readonly: true },
                             { name: "ClockOutTime", cls: "span2", placeHolder: "Clock Out Time", readonly: false }
                         ]
                     },
                     {
                         text: "Rest Time - Overtime",
                         type: "controls",
                         items: [
                             { name: "OnRestTime", cls: "span2", placeHolder: "On Rest Time", readonly: true },
                             { name: "OffRestTime", cls: "span2", placeHolder: "Off Rest Time", readonly: true },
                             { name: "CalcOvertime", cls: "span2 number-int", placeHolder: "Calc Overtime (minutes)", readonly: true },
                             { name: "ApprOvertime", cls: "span2 number-int", placeHolder: "Appr Overtime (minutes)", readonly: false }
                         ]
                     },
                     { type: "buttons", items: [{ name: "btnSaveDetail", text: "Save", icon: "icon-save" }, { name: "btnCancelDetail", text: "cancel", icon: "icon-undo" }] },
                ]
            },
            {
                xtype: "kgrid",
                title: "Overtime Data",
                name: "gridOvertime"
            }
        ]
    };

    var widget = new SimDms.Widget(options);
    widget.render(renderCallback);

    function renderCallback() {    
        widget.setSelect([
            { name: "Department", url: "ab.api/combo/departments" },
            { name: "Position", url: "ab.api/combo/positions", optionalText: "-- SELECT ALL --", cascade: { name: "Department" } },
        ]);

        widget.default = { AttdDate: new Date() };
        widget.populate(widget.default);

        showPanelEdit(false);
        initializeComponentEvent();
    }

    function initializeComponentEvent() {
        $("#Department, #Position, [name='AttdDate']").on("change", function (evt) {
            reloadOvertimeData();
        });
        $("#btnFilter").on("click", function (evt) {
            reloadOvertimeData();
        });
    }

    function showPanelEdit(state) {
        var panelEdit = $("#panelEdit");
        var gridOvertime = $("#gridOvertime").parent().parent();

        if (state) {
            panelEdit.fadeIn();
            gridOvertime.hide();
        }       
        else {
            panelEdit.hide();
            gridOvertime.fadeIn();
        }
    }

    function reloadOvertimeData() {
        var department = $("[name='Department']").val();
        var position = $("[name='Position']").val();
        var dateFrom = $("[name='AttdDate']").val();

        if (widget.isNullOrEmpty(department) == true || widget.isNullOrEmpty(dateFrom) == true) {
            widget.showNotification("You must have to fill at least Department and DateFrom input.");
        }
        else {
            var url = "ab.api/grid/employeeshifts";
            var params = {
                Department: $("#Department").val(),
                Position: $("#Position").val(),
                DateFrom: $("[name='AttdDate']").val(),
                IsGrid: true
            };

            widget.kgrid({
                name: "gridOvertime",
                url: url,
                params: params,
                sort: [{ field: "NextFollowUpDate", dir: "desc" }],
                toolbars: [
                    { name: "btnEditDetail", text: "Edit", icon: "icon-edit" },
                    //{ name: "btnDeleteDetail", text: "Delete", icon: "icon-trash" },
                ],
                columns: [
                    { field: "AttdDate", title: "Attendance Date", width: 150, template: "#=  moment(AttdDate, 'YYYYMMDD').format('DD MMM YYYY') #" },
                    { field: "EmployeeID", title: "NIK", width: 100 },
                    { field: "EmployeeName", title: "Name", width: 250 },
                    { field: "Shift", title: "", width: 150 },
                    { field: "ClockInTime", title: "Clock In", width: 80 },
                    { field: "ClockOutTime", title: "Clock Out", width: 80 },
                    { field: "CalcOvertime", title: "Calc Overtime", width: 150 },
                    { field: "ApprOvertime", title: "Appr Overtime", width: 150 },
                ],
                onDblClick: function () {
                },
                dataBound: function () {
                    options.grid = $("#gridOvertime").data("kendoGrid");
                    $("#gridOvertime .k-grid-content tr").on("click", function (evt) {
                        options.row = $(this);
                    });
                    $("#gridOvertime .k-grid-content tr").on("dblclick", function (evt) {
                        $("#btnEditDetail").click();
                    });
                }
            }, renderGridOvertimeCallback);
        }
    }

    function editOvertime(a, b, c) {
        showPanelEdit(true);

        var grid = options.grid;
        var row = options.row;
        var data = grid.dataItem(row);
        delete data['_events'];
        delete data['parent'];

        data.OnDutyTime = widget.validateTime(data.OnDutyTime);
        data.OffDutyTime = widget.validateTime(data.OffDutyTime);
        data.ClockInTime = widget.validateTime(data.ClockInTime);
        data.ClockOutTime = widget.validateTime(data.ClockOutTime);
        data.OnRestTime = widget.validateTime(data.OnRestTime);
        data.OffRestTime = widget.validateTime(data.OffRestTime);
        widget.populate(data, "#panelEdit", function () {
            var val = widget.timeDiff(data.ClockOutTime, data.OffDutyTime);
            data.CalcOvertime = val;
        });
    }

    function renderGridOvertimeCallback() {
        var btnEditDetail = $("#btnEditDetail");
        var btnDeleteDetail = $("#btnDeleteDetail");
        var btnSaveDetail = $("#btnSaveDetail");
        var btnCancelDetail = $("#btnCancelDetail");

        btnSaveDetail.off();
        btnCancelDetail.off();
        btnEditDetail.off();
        btnDeleteDetail.off();

        btnSaveDetail.on("click", function (evt) {
            saveData();
        });
        btnCancelDetail.on("click", function (evt) {
            showPanelEdit(false);
        });
        btnEditDetail.on("click", function (evt) {
            editOvertime();
        });
        btnDeleteDetail.on("click", function (evt) {
            console.log("delete detail");
        });
    }

    function saveData() {
        var url = "ab.api/EmplShift/UpdateShift";
        var params = widget.getForms();
        params.AttdDate = moment(params.AttdDate).format("YYYYMMDD");

        if (params.CalcOvertime < params.ApprOvertime) {
            widget.showNotification("Approved overtime must equal or less than Calculated Overtime.");
        }
        else if (params.ApprOvertime < 0) {
            widget.showNotification("Approved overtime must be a positive number.");
        }
        else {
            widget.post(url, params, function (result) {
                if (result.success) {
                    widget.clearForm("panelEdit");
                    showPanelEdit(false);
                    reloadOvertimeData();
                }

                widget.showNotification(result.message);
            });
        }
    }
});