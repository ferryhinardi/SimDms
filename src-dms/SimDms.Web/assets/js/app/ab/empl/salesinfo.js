$(document).ready(function () {
    var widget = new SimDms.Widget({
        title: "Sales Information",
        xtype: "panels",
        toolbars: [],
        panels: [
            {
                title: "Contact Information",
                items: [
                    {
                        text: "Employee",
                        type: "controls",
                        items: [
                            { name: "EmployeeID", cls: "span2", placeHolder: "NIK", type: "popup", readonly: true },
                            { name: "EmployeeName", cls: "span6 ignore-uppercase", placeHolder: "Name", required: true, readonly: true }
                        ]
                    },
                    { text: "ATPM ID", cls: "span4", readonly: true, type: "popup", icon: "icon-bolt", hint: "clik here to get ATPM ID", name: "SalesID" },
                    { text: "Email", cls: "span4", name: "Email", readonly: true },
                    { text: "Handphone 1", cls: "span4", name: "Handphone1", readonly: true },
                    { text: "Handphone 2", cls: "span4", name: "Handphone2", readonly: true },
                    { text: "Office Location", type: "textarea", name: "OfficeLocation", readonly: true },
                ]
            },
            {
                title: "Job Information",
                items: [
                    { text: "Join Date", cls: "span4", name: "JoinDate", readonly: true },
                    { text: "Resign Date", cls: "span4", name: "ResignDate", readonly: true },
                    { text: "Department", cls: "span4", name: "DepartmentName", readonly: true },
                    { text: "Position", cls: "span4", name: "PositionName", readonly: true },
                    { text: "GradeName", cls: "span4", name: "GradeName", readonly: true },
                    { text: "Rank", cls: "span4", name: "RankName", readonly: true },
                    { text: "Manager", cls: "span4", name: "TeamLeaderName", readonly: true },
                    { text: "Additional Job 1", cls: "span4", name: "AdditionalJob1Name", readonly: true },
                    { text: "Additional Job 2", cls: "span4", name: "AdditionalJob2Name", readonly: true },
                    { text: "Personal Status", cls: "span4", name: "Status", readonly: true },
                ]
            },
        ],
    });

    widget.render(function () {
        widget.enableElement([{ name: "btnSalesID", type: "button", status: false }]);

        $("#btnEmployeeID").on("click", function () {
            var lookup = widget.klookup({
                name: "Employee",
                title: "Employee List",
                url: "ab.api/kgrid/Employees",
                params: { Department: "SALES", Status: "Aktif" },
                serverBinding: true,
                pageSize: 14,
                filters: [
                    {
                        text: "Filter Employee",
                        type: "controls",
                        items: [
                            { name: "fltNik", text: "Employee Nik", cls: "span2" },
                            { name: "fltSlsID", text: "Sales ID", cls: "span2" },
                            { name: "fltEmplName", text: "Employee Name", cls: "span4" },
                        ]
                    },
                ],
                columns: [
                    { field: "EmployeeID", title: "NIK", width: 100 },
                    { field: "EmployeeName", title: "Name", width: 250 },
                    { field: "Department", title: "Department", width: 100 },
                    { field: "SalesID", title: "ATPM ID", width: 100 },
                    { field: "Position", title: "Position", width: 100 },
                    { field: "JoinDate", title: "Join Date", width: 100, template: "#= (JoinDate == undefined) ? '' : moment(JoinDate).format('DD MMM YYYY') #" },
                    { field: "ResignDate", title: "Resign Date", width: 100, template: "#= (ResignDate == undefined) ? '' : moment(ResignDate).format('DD MMM YYYY') #" },
                    { field: "Status", title: "Status", width: 100 },
                ],
            });

            lookup.dblClick(function (data) {
                if (widget.isNullOrEmpty(data.JoinDate) == false) {
                    try {
                        data["JoinDate"] = widget.toDateFormat(widget.cleanJsonDate(data.JoinDate));
                        data["ResignDate"] = widget.toDateFormat(widget.cleanJsonDate(data.ResignDate));
                    } catch (ex) { }
                }
                widget.populate(data, populateCallback(data));
                widget.lookup.hide();
                transactionStatus = true;
            });
        });

        $("#btnSalesID").on("click", function () {
            var params = { EmployeeID: widget.getValue({ name: "EmployeeID", type: "text" }) };
            var url = "ab.api/Employee/GetSalesID";

            widget.post(url, params, function (result) {
                if (result.status) {
                    var salesIDElement = widget.getObject("SalesID");
                    salesIDElement.val(result.atpmID);
                    widget.enableElement([
                        { name: "btnSalesID", type: "button", status: false }
                    ]);
                }
                widget.showNotification(result.message);
            });
        });
    });

    function populateCallback(data) {
        var btnSalesIDStatus = false;
        if (widget.isNullOrEmpty(data.SalesID) == false) {
            btnSalesIDStatus = false;
        }
        else {
            btnSalesIDStatus = true;
        }
        widget.enableElement([{ name: "btnSalesID", type: "button", status: btnSalesIDStatus }]);
    }
});

