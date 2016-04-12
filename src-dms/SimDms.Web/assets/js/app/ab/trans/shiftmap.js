$(document).ready(function () {
    var options = {
        title: "Mapping Shift",
        xtype: "panels",
        panels: [
            {
                name: "pnlFilter",
                title: "Filter Selection",
                items: [
                    { name: "Department", text: "Department", cls: "span4", type: "select", opt_text: "--SELECT ALL--" },
                    { name: "Position", text: "Position", cls: "span4", type: "select", opt_text: "--SELECT ALL--", },
                    { name: "DateFrom", text: "Date From", cls: "span4", type: "datepicker" },
                    { name: "DateTo", text: "Date To", cls: "span4", type: "datepicker" },
                    { name: "Shift", text: "Shift", cls: "span4", type: "select", opt_text: "--SELECT ALL--" },
                    { type: "buttons", items: [{ name: "btnFilter", text: "Search", icon: "icon-search" }, { name: "btnAssign", text: "Assign", icon: "icon-bolt" }] },
                ]
            },
            {
                name: "panelEmployeeShiftList",
                title: "Employee List",
                xtype: "kgrid"
            }
        ],
    }

    var widget = new SimDms.Widget(options);
    widget.setSelect([
        { name: "Department", url: "ab.api/combo/departments" },
        { name: "Position", url: "ab.api/combo/positions", optionalText: "-- SELECT ALL --", cascade: { name: "Department" } },
        { name: "Shift", url: "ab.api/combo/shifts", optionalText: "--SELECT ALL--" },
        { name: "ShiftEdit", url: "ab.api/combo/shifts", optionalText: "--SELECT ONE--" }
    ]);
    widget.setEventList([
        {
            name: "btnFilter",
            type: "button",
            eventType: "click",
            event: function (evt) {
                var data = $("#pnlFilter").serializeObject();
                widget.post("ab.api/emplshift/search", data, function (result) {
                    widget.reloadGridData("tblEmployeeList");
                });
            }
        }, {
            name: "btnAssign",
            type: "button",
            eventType: "click",
            event: function (evt) {
                var lookup = widget.klookup({
                    name: "panelShiftList",
                    title: "Shift List",
                    url: "ab.api/grid/ShiftsKendo",
                    sort: ({ field: "ShiftName", dir: "asc" }),
                    serverBinding: true,
                    filters: [
                        {
                            text: "Filter",
                            type: "controls",
                            left: 80,
                            items: [
                                { name: "ShiftName", text: "ShiftName", cls: "span4" },
                            ]
                        }
                    ],
                    columns: [
                        { field: "ShiftCode", title: "Shift Code", width: 100 },
                        { field: "ShiftName", title: "Shift Name", width: 100 },
                        { field: "OnDutyTime", title: "On Duty Time", width: 100 },
                        { field: "OffDutyTime", title: "Off Duty Time", width: 100 },
                        { field: "OnRestTime", title: "On Rest Time", width: 100 },
                        { field: "OffRestTime", title: "Off Rest Time", width: 100 },
                        { field: "IsActive", title: "Is Active", width: 140, template: "#= (IsActive == true) ? 'Ya' : 'Tidak' #" },
                    ]
                });
                lookup.dblClick(function (data) {
                    var dataMain = widget.getForms();
                    dataMain.TargetShift = data.ShiftCode;
                    widget.post("ab.api/emplshift/assign", dataMain, function (result) {
                        if (result.success) {
                            reloadEmployeeList();
                        }
                        widget.showNotification(result.message);
                    });
                });
            }
        }
    ]);
    widget.default = { DateFrom: new Date(), DateTo: new Date() };
    widget.render(function () {
        widget.populate(widget.default);
    });

    $("#pnlFilter select").on("change", function () {
        var data = $("#pnlFilter").serializeObject();
        widget.post("ab.api/emplshift/search", data, function (result) {
            reloadEmployeeList();
        });
    });

    $('#btnCancel').on('click', function () {
        $('#pnlEmployeeList').slideUp();
    });

    $("#btnSave").on("click", function () {
        if (widget.validate() == true) {
            var data = {
                ShiftEdit: $('#ShiftEdit').val(),
                AttdDateEdit: $('#AttdDateEdit').val(),
                EmployeeIDEdit: $('#EmployeeIDEdit').val()
            };
            widget.post("ab.api/emplshift/UpdAssign", data, function (result) {
                if (result.success) {
                    widget.reloadGridData("tblEmployeeList");
                    $('#pnlEmployeeList').slideUp();
                }
                else {
                    alert(result.message);
                }
            });
        }
    });

    widget.onGridClick(function (icon, data) {
        switch (icon) {
            case "edit":
                editShitmap(data);
                break;
            case "trash":
                deleteDetail(data);
                break;
            default:
                break;
        }
    });

    function editShitmap(data) {
        $('#pnlEmployeeList').slideDown();
        $('#ShiftEdit').val(data.ShiftCode);
        $('#CompanyCodeEdit').val(data.CompanyCode);
        $('#AttdDateEdit').val(data.AttdDate);
        $('#EmployeeIDEdit').val(data.EmployeeID);
        $('#EmployeeNameEdit').val(data.EmployeeName);
    }

    function reloadEmployeeList() {
        var Department = $("[name='Department']").val();
        var Position = $("[name='Position']").val();
        var DateFrom = $("[name='DateFrom']").val();
        var DateTo = $("[name='DateTo']").val();
        var Shift = $("[name='Shift']").val();
        var ShiftEdit = $("[name='ShiftEdit']").val();
        var CompanyCodeEdit = $("[name='CompanyCodeEdit']").val();
        var AttdDateEdit = $("[name='AttdDateEdit']").val();
        var EmployeeIDEdit = $("[name='EmployeeIDEdit']").val();

        var gridEmployeeList = widget.kgrid({
            name: "panelEmployeeShiftList",
            url: "ab.api/grid/employeeshifts",
            params: {
                "Department": Department,
                "Position": Position,
                "DateFrom": DateFrom,
                "DateTo": DateTo,
                "Shift": Shift,
                "ShiftEdit": ShiftEdit,
                "CompanyCodeEdit": CompanyCodeEdit,
                "AttdDateEdit'": AttdDateEdit,
                "EmployeeIDEdit'": EmployeeIDEdit,
            },
            serverBinding: true,
            pageSize: 10,
            columns: [
                { field: "AttdDate", title: "Date", width: 140, template: "#=  (AttdDate == undefined) ? '' : moment(cleanDate(AttdDate)).format('DD MMM YYYY')  #" },
                { field: "EmployeeID", title: "NIK", width: 150 },
                { field: "EmployeeName", title: "Name", width: 150 },
                { field: "Department", title: "Department", width: 150 },
                { field: "Position", title: "Position", width: 150 },
                { field: "Grade", title: "Grade", width: 150 },
                { field: "ShiftCode", title: "Shift", width: 150 },
            ],
            dataBound: function (data) {
                var row;
                $(".k-grid-content tr").on("click", function () { row = $(this); });
                $(".k-grid-content tr").on("dblclick", function () {
                    var grid = $(".kgrid #panelEmployeeShiftList").data("kendoGrid");
                    var params = grid.dataItem(row);
                    console.log(params);

                    var lookup = widget.klookup({
                        name: "panelShiftList",
                        title: "Shift List",
                        url: "ab.api/grid/ShiftsKendo",
                        sort: ({ field: "ShiftName", dir: "asc" }),
                        serverBinding: true,
                        filters: [
                            {
                                text: "Filter",
                                type: "controls",
                                left: 80,
                                items: [
                                    { name: "ShiftName", text: "ShiftName", cls: "span4" },
                                ]
                            }
                        ],
                        columns: [
                            { field: "ShiftCode", title: "Shift Code", width: 100 },
                            { field: "ShiftName", title: "Shift Name", width: 100 },
                            { field: "OnDutyTime", title: "On Duty Time", width: 100 },
                            { field: "OffDutyTime", title: "Off Duty Time", width: 100 },
                            { field: "OnRestTime", title: "On Rest Time", width: 100 },
                            { field: "OffRestTime", title: "Off Rest Time", width: 100 },
                            { field: "IsActive", title: "Is Active", width: 140, template: "#= (IsActive == true) ? 'Ya' : 'Tidak' #" },
                        ]
                    });
                    lookup.dblClick(function (data) {
                        params.TargetShift = data.ShiftCode;
                        var datas = {
                            EmployeeID: params.EmployeeID,
                            AttdDate: params.AttdDate,
                            TargetShift: data.ShiftCode,
                            CompanyCode: params.CompanyCode
                        };
                        widget.post("ab.api/emplshift/UpdateSingleShift", datas, function (result) {
                            if (result.status) {
                                reloadEmployeeList();
                            }
                            widget.showNotification(result.message);
                        });
                    });
                });
            }
        });
    }
});

function cleanDate(rawDate) {
    var clean = rawDate.substring(0, 4) + '-' + rawDate.substring(4, 6) + '-' + rawDate.substring(6, 8);
    return clean;
}
